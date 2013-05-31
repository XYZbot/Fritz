#define MIN_PULSE_WIDTH       500     // the shortest pulse sent to a servo
#define MAX_PULSE_WIDTH      2500     // the longest pulse sent to a servo
#define REFRESH_INTERVAL    20000     // minumim time to refresh servos in microseconds

#define SERVOS_PER_BANK       10     // the maximum number of servos controlled by one timer bank
#define MAX_SERVOS   20				// the maximum number of possible servos to control on the UNO

#define INTERRUPT_OVERHEAD 2

#include <avr/interrupt.h>
#include <Arduino.h>

// identifies which servo pin is currently being pulsed
static volatile char ss_currentServo = -1;
static volatile unsigned char ss_pulseBank = 0;
static volatile int ss_pulseDiff = -1;
static volatile unsigned int ss_ServoTicks[MAX_SERVOS];
static volatile unsigned int ss_TurnOff[MAX_SERVOS];
static volatile unsigned int ss_refreshInterval;
static volatile unsigned int ss_bankClk;

/************ ISP (Interrupt Service Proceedure) ***********************/
SIGNAL (TIMER1_COMPA_vect)
{
  // if we are starting a new 20ms frame, initialize the timer counter to zero
  if(ss_currentServo < 0 )
    TCNT1 = 0;
  else
  {
    int index = ss_currentServo+ss_pulseBank;
    // stop the pulse on this servo if it is activated
    if(ss_ServoTicks[index] > 0)
    {
      digitalWrite(index, LOW);
      if (ss_TurnOff[index])
      {
        ss_ServoTicks[index]=0;
        ss_TurnOff[index]=0;
        pinMode(index, INPUT);
      }
    }    

    if (ss_pulseDiff == 0)
    {
      // stop the pulse on this servo if it is activated
      int index = ss_currentServo+(ss_pulseBank^SERVOS_PER_BANK);
      if(ss_ServoTicks[index] > 0)
      {
        digitalWrite(index, LOW);
        if (ss_TurnOff[index])
        {
          ss_ServoTicks[index]=0;
          ss_TurnOff[index]=0;
          pinMode(index, INPUT);
        }
      }
    }
    else
    // if we need to wait for the sibling pulse to trigger do so
    if (ss_pulseDiff == 1)
    {
      // if we ran out of time ... stop the second pulse
      if (((unsigned)TCNT1)>(ss_bankClk+4))
      {
        int index = ss_currentServo+(ss_pulseBank^SERVOS_PER_BANK);
        // stop the pulse on this servo if it is activated
        if(ss_ServoTicks[index] > 0)
        {
          digitalWrite(index, LOW);
          if (ss_TurnOff[index])
          {
            ss_ServoTicks[index]=0;
            ss_TurnOff[index]=0;
            pinMode(index, INPUT);
          }
        }
      }
      else
      {
        // setup timer callback
        OCR1A = ss_bankClk;

        // indicate we are done with both servos
        ss_pulseDiff = -1;

        // switch to the other servo bank
        ss_pulseBank ^=SERVOS_PER_BANK;

        return;
      }
    }
  }

  // increment to the next pin
  if ((++ss_currentServo) < SERVOS_PER_BANK)
  {
    // keep track of which pulse is longer, bank0 or bank1
    unsigned int bankClk = 0;
    unsigned int altBankClk = 0;

    ss_pulseDiff = -1;
    ss_pulseBank = 0;

    int val = ss_ServoTicks[ss_currentServo];
    // setup to callback this function after the pulse size of the current servo
    if (val > 0)
    {
      // start the pulse
      digitalWrite(ss_currentServo, HIGH);

      bankClk = ((unsigned)TCNT1) + val;
    }

    int secondCurrentServo = ss_currentServo+SERVOS_PER_BANK;
    val = ss_ServoTicks[secondCurrentServo];
    if (val > 0)
    {
      // start the second pulse
      digitalWrite(secondCurrentServo, HIGH);

      altBankClk = ((unsigned)TCNT1)+val;

      // if this pulse is shorter than the first use it!
      if ((altBankClk<bankClk)||(bankClk==0))
      {
        // save the difference between the pulse sizes
        ss_pulseDiff = 1;

        ss_bankClk = bankClk;

        bankClk = altBankClk;

        // remember that we are referring to the second bank
        ss_pulseBank = SERVOS_PER_BANK;

      }
      else
      {
        ss_bankClk = altBankClk;

        if (altBankClk==bankClk)
          ss_pulseDiff = 0;
        else
          ss_pulseDiff = 1;
      }
    }

    if (bankClk > (((unsigned)TCNT1)+ INTERRUPT_OVERHEAD))
      OCR1A = bankClk - INTERRUPT_OVERHEAD;
    else
      OCR1A = ((unsigned)TCNT1) + 4;
  }
  else
  {
    // we finished servicing all servos so wait to complete the 20ms period before starting over
    // the - 4 is to ensure that we are not about to miss a OCR interrupt
    if ((unsigned)TCNT1 <  (ss_refreshInterval - 4))
      OCR1A = ss_refreshInterval;
    else
      // for whatever reason, we've exceeded the 20ms time period ... just setup to call back asap
      OCR1A = ((unsigned)TCNT1) + 4;

    // indicate that we are done with this period and to start a new one on the next callback
    ss_currentServo = -1;
  }
}

/****************** end of static functions ******************************/

void ss_Init()
{
  int i;
  for (i=0;i<MAX_SERVOS;i++)
  {
    ss_ServoTicks[i] = 0;
    ss_TurnOff[i] = 0;
  }
  
  TCCR1A = 0;             // normal counting mode
  TCCR1B = _BV(CS11);     // set prescaler of 8
  TCNT1 = 0;              // clear the timer count

  TIFR1 |= _BV(OCF1A);     // clear any pending interrupts;
  TIMSK1 |=  _BV(OCIE1A) ; // enable the output compare interrupt

  // cache the REFRESH_INTERVAL value in ticks instead of microseconds
  ss_refreshInterval = ((clockCyclesPerMicrosecond() * REFRESH_INTERVAL)>>3);
}

void ss_Destroy()
{
  // disable timer 1 output compare interrupt
  TIMSK1 &=  ~_BV(OCIE1A) ;
}

int ss_SetPosition(int pin, int value)
{
  // check that we have a valid pin (note that 0 and 1 are used for serial!)
  if ((pin<0)||(pin>=MAX_SERVOS)) return 0;

  // an out of bounds value indicates a servo shutdown
  if ((value < MIN_PULSE_WIDTH)||(value> MAX_PULSE_WIDTH))
  {
    if (ss_ServoTicks[pin]>0)
      ss_TurnOff[pin] = 1;
      
    return 0;
  }

  // convert from microseconds to clock ticks assuming prescaler of 8
  value = ((clockCyclesPerMicrosecond() * (value - INTERRUPT_OVERHEAD))>>3);

  // check that pin has been initialized
  int hold = ss_ServoTicks[pin];

  //unsigned char oldSREG = SREG;
  //cli();
  ss_ServoTicks[pin] = value;
  //SREG = oldSREG;

  // if value was zero, the pin had not been set to OUTPUT
  if (hold == 0)
  {
    // set servo pin to output
    pinMode( pin, OUTPUT);
  }
}

//////////////

#include <EEPROM.h>

int highestPin = 20;

unsigned char g_message[256];
unsigned char g_config[256];

unsigned int g_messageTop;
unsigned int g_command;
unsigned int g_length;
unsigned int g_crc;
unsigned int g_count;
unsigned int g_channel;
unsigned int g_value;
long g_calc;

unsigned long g_heartBeat[20];

#define ARDUINO_GET_ID 0
#define ARDUINO_RESET 1
#define ARDUINO_SET_OBJECT 2
#define ARDUINO_SET_SERVO 3
#define ARDUINO_HEARTBEAT 4
#define ARDUINO_RELEASE_SERVO 5
#define ARDUINO_GET_IR 6
#define ARDUINO_GET_SONAR 7

#define ARDUINO_LOAD 32
#define ARDUINO_SAVE 33

#define MAX_CONFIG (65*2)

void initialize()
{
  int i;
  for (i=0;i<highestPin;i++) g_heartBeat[i]=(long)0;
  
  for (i=0;i<MAX_CONFIG;i++)
    g_config[i]=EEPROM.read(i);
}

void setup()
{
  Serial.begin(57600);

  ss_Init();
}

int readPacket()
{
  // get header byte
  // 128 (bit 8) flag indicates a new g_command packet ..
  //		that means the g_value bytes can never have 128 set!
  // next byte is the g_command 0-8
  // next byte is single byte data length
  // for command >= 32 next byte is high byte data length
  // data
  // crc

  int high;
  int crc;
  g_messageTop=2;

  do
  {
    while (Serial.available() <= 0) continue;
    g_command = Serial.read();
  }
  while ((g_command&128)==0);

  g_message[0] = crc = g_command;
  g_command^=128;

  while (Serial.available() <= 0) continue;
  g_length = g_message[1] = Serial.read();
  crc ^= g_length;

  if (g_command>=32)
  {
    while (Serial.available() <= 0) continue;
    g_message[2] = high = Serial.read();
    g_length = g_length|(high<<7);
    crc ^= high;
    g_messageTop=3;
  }

  // read in entire message
  if (g_length>0)
  {
    int count = g_length;
    while (count>0)
    {
      while (Serial.available() <= 0) continue;
      g_message[g_messageTop] = Serial.read();
      crc ^= g_message[g_messageTop++];
      count--;
    }

    while (Serial.available() <= 0) continue;
    g_message[g_messageTop++] = g_crc = Serial.read();

    if ((crc&127)!=(g_crc&127)) return 0;
  }
  return 1;
}

void echoPacket()
{
  Serial.write(g_message, g_messageTop);
}

void loop()
{
  //ss_SetPosition(4, 1500);
  //return;
/*
  int p, q;
  for (p=700, q=800;p<2300;p++, q++)
  {
    //ss_SetPosition(14, p);
    ss_SetPosition(19, q);
    delay(5);
  }

  for (;p>=700;p--,q--)
  {
    //ss_SetPosition(14, p);
    ss_SetPosition(19, q);
    delay(5);
  }
  return;
*/

  int crc;
  int val;
  int i,j;
  int trimVal, maxVal, minVal, pin;

  while (Serial.available()>0)
  {
    if (readPacket()==0) 
      return;

    switch (g_command)
    {
      // init
      case  ARDUINO_GET_ID:
        initialize();
        Serial.print("ARDU004");
      break;
      // servo
      case  ARDUINO_RELEASE_SERVO:
        g_channel = g_message[2];
        ss_SetPosition(g_channel, 0);
        echoPacket();
      break;
      case  ARDUINO_SET_SERVO:
        g_channel = g_message[2];
        g_value = g_message[3] | (g_message[4]<<7);

        ss_SetPosition(g_channel, g_value);

        g_heartBeat[g_channel]=millis();

        echoPacket();
      break;
      case ARDUINO_SET_OBJECT:
        // sets the value of a specific part of the face. This uses configuration
        // memory to determine the pin and appropriate scaled min/max values.
        g_channel = g_message[2];
        val = g_message[3] | (g_message[4]<<7);
        if (val > 8192) val -= 16384;

        i = g_channel*10;
        
        trimVal = g_config[i] | (g_config[i+1]<<7);
        if (trimVal > 8192) trimVal -= 16384;
        maxVal = g_config[i+2] | (g_config[i+3]<<7);
        if (maxVal > 8192) maxVal -= 16384;
        minVal = g_config[i+4] | (g_config[i+5]<<7);
        if (minVal > 8192) minVal -= 16384;

        pin = g_config[i+6] | (g_config[i+7]<<7);

        if (pin!=0)
        {
          g_calc = (((long)val+1000)* ((long)(maxVal - minVal)))/(long)2000;
          
          val = g_calc + trimVal + minVal;
          if (val<-1000) val=-1000;
          if (val>1000) val=1000;
          if (val<minVal) val=minVal;
          if (val>maxVal) val=maxVal;
          ss_SetPosition(pin, val+1500);
          g_heartBeat[pin]=millis();
        }
        
        echoPacket();
      break;
      case ARDUINO_HEARTBEAT:
      break;
      case ARDUINO_LOAD:
        g_count = g_message[3]*2;

        g_message[0]=g_command|128;
        g_message[1]=g_count+1;
        g_message[2]=0;

        crc=g_message[0]^g_message[1]^g_message[2];

	for (j=3,i=0;i<g_count;i+=2,j+=2)
	{
	  g_message[j] = g_config[i];
	  g_message[j+1] = g_config[i+1];
	  crc=crc^g_message[j]^g_message[j+1];
	}
	g_message[j]=crc&127;

	for (i=3;i<j;i+=10)
        {
           trimVal = g_message[i] | (g_message[i+1]<<7);
           if (trimVal > 8192) trimVal -= 16384;
           maxVal = g_message[i+2] | (g_message[i+3]<<7);
           if (maxVal > 8192) maxVal -= 16384;
           minVal = g_message[i+4] | (g_message[i+5]<<7);
           if (minVal > 8192) minVal -= 16384;
           pin = g_message[i+6] | (g_message[i+7]<<7);

          if (pin!=0)
          {
            val = ((maxVal-minVal)/2)+minVal+trimVal;
            if (val<-1000) val=-1000;
            if (val>1000) val=1000;
            if (val<minVal) val=minVal;
            if (val>maxVal) val=maxVal;
            ss_SetPosition(pin, val+1500);
           g_heartBeat[pin]=millis();
          }
        }

	Serial.write(g_message, j+1);

      break;
      case ARDUINO_SAVE:

	for (j=3,i=0;i<g_length;i+=2,j+=2)
	{
          EEPROM.write(i, g_message[j]);
          g_config[i] = g_message[j];
          EEPROM.write(i+1, g_message[j+1]);
          g_config[i+1] = g_message[j+1];
        }

        g_message[0]=g_command|128;
        g_message[1]=0;
        g_message[2]=0;
	Serial.write(g_message, 3);
      break;
      case ARDUINO_RESET:

	for (i=0;i<MAX_CONFIG;i+=10)
        {
           trimVal = g_config[i] | (g_config[i+1]<<7);
           if (trimVal > 8192) trimVal -= 16384;
           maxVal = g_config[i+2] | (g_config[i+3]<<7);
           if (maxVal > 8192) maxVal -= 16384;
           minVal = g_config[i+4] | (g_config[i+5]<<7);
           if (minVal > 8192) minVal -= 16384;
           pin = g_config[i+6] | (g_config[i+7]<<7);

          if (pin!=0)
          {
            val = ((maxVal-minVal)/2)+minVal+trimVal;
            if (val<-1000) val=-1000;
            if (val>1000) val=1000;
            if (val<minVal) val=minVal;
            if (val>maxVal) val=maxVal;
            ss_SetPosition(pin, val+1500);
            g_heartBeat[pin]=millis();
          }
        }

      break;
      case ARDUINO_GET_IR:
        g_channel = g_message[2];
        
        g_value = analogRead(g_channel);

        g_message[3] = g_value&127;
        g_message[4] = (g_value>>7)&127;

        echoPacket();

      break;
      case ARDUINO_GET_SONAR:
        digitalWrite(g_message[2], LOW);
        delayMicroseconds(2);
        digitalWrite(g_message[2], HIGH);
        delayMicroseconds(10); // Added this line
        digitalWrite(g_message[2], LOW);

        g_value = pulseIn(g_message[3], HIGH);
  
        g_message[3] = g_value&127;
        g_message[4] = (g_value>>7)&127;

        echoPacket();
      break;
    }
  }

  long sec = millis();
  for (i=0;i<highestPin;i++)
  {
    if (g_heartBeat[i]!=0)
    {
      if ((sec - g_heartBeat[i])>2000)
      {
        ss_SetPosition(i, 0);
        g_heartBeat[i]=0;
      }
    }
  }
}


