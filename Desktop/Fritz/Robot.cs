using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml;
using System.IO;

namespace Fritz
{
    //load configuration here ...

    public class Robot
    {
        /*
                const int ARDUINO_SET_DIGITAL_STREAM; //
                const int ARDUINO_SET_DIGITAL_HIGH; //
                const int ARDUINO_SET_DIGITAL_LOW; //
                const int ARDUINO_SET_ANALOG_STREAM; //
                const int ARDUINO_DIGITAL_STREAM;
                const int ARDUINO_ANALOG_STREAM;
                const int ARDUINO_SET_ANALOG;
                const int ARDUINO_RESET;
        */

        // send packets
        const int ARDUINO_GET_ID = 0; // returns ARDU
        const int ARDUINO_RESET = 1; //
        const int ARDUINO_SET_OBJECT = 2; //
        const int ARDUINO_SET_SERVO = 3; //
        const int ARDUINO_HEARTBEAT = 4;
        const int ARDUINO_RELEASE_SERVO = 5;
        const int ARDUINO_GET_IR = 6;
        const int ARDUINO_GET_SONAR = 7;

        const int ARDUINO_LOAD = 32;
        const int ARDUINO_SAVE = 33;

        Serial serial = new Serial();

        const int minPin = 2;
        const int maxPin = 19;

        float irValue=1000.0f;
        float sonarValue=1000.0f;

        CalibrationData cd = new CalibrationData();

        public Robot()
        {
            serial.ReadCallback += new EventHandler(serial_ReadCallback);
        }

        public void Stop()
        {
            serial.Stop();
        }

        public bool IsConnected()
        {
            return serial.IsConnected();
        }

        public void SetState(RobotState ss, RobotState lastState)
        {
            if (lastState.rightEyebrow != ss.rightEyebrow)
                SetServo(cd.rightEyebrowPin, 1.0f-ss.rightEyebrow, cd.rightEyebrowMax, cd.rightEyebrowMin, cd.rightEyebrowTrim);

            if (lastState.leftEyebrow != ss.leftEyebrow)
                SetServo(cd.leftEyebrowPin, ss.leftEyebrow, cd.leftEyebrowMax, cd.leftEyebrowMin, cd.leftEyebrowTrim);

            if (lastState.rightHorizontalEye != ss.rightHorizontalEye)
                SetServo(cd.rightHorizontalEyePin, ss.rightHorizontalEye, cd.rightHorizontalEyeMax, cd.rightHorizontalEyeMin, cd.rightHorizontalEyeTrim);

            if (lastState.leftHorizontalEye != ss.leftHorizontalEye)
                SetServo(cd.leftHorizontalEyePin, ss.leftHorizontalEye, cd.leftHorizontalEyeMax, cd.leftHorizontalEyeMin, cd.leftHorizontalEyeTrim);

            if (lastState.rightVerticalEye != ss.rightVerticalEye)
                SetServo(cd.rightVerticalEyePin, ss.rightVerticalEye, cd.rightVerticalEyeMax, cd.rightVerticalEyeMin, cd.rightVerticalEyeTrim);

            if (lastState.leftVerticalEye != ss.leftVerticalEye)
                SetServo(cd.leftVerticalEyePin, 1.0f-ss.leftVerticalEye, cd.leftVerticalEyeMax, cd.leftVerticalEyeMin, cd.leftVerticalEyeTrim);

            if (lastState.rightEyelid != ss.rightEyelid)
                SetServo(cd.rightLidPositionPin, 1.0f - ss.rightEyelid, cd.rightLidPositionMax, cd.rightLidPositionMin, cd.rightLidPositionTrim);

            if (lastState.leftEyelid != ss.leftEyelid)
                SetServo(cd.leftLidPositionPin, 1.0f - ss.leftEyelid, cd.leftLidPositionMax, cd.leftLidPositionMin, cd.leftLidPositionTrim);

            if (lastState.neckTilt != ss.neckTilt)
                SetServo(cd.neckTiltPin, 1.0f - ss.neckTilt, cd.neckTiltMax, cd.neckTiltMin, cd.neckTiltTrim);

            if (lastState.neckTwist != ss.neckTwist)
                SetServo(cd.neckTwistPin, ss.neckTwist, cd.neckTwistMax, cd.neckTwistMin, cd.neckTwistTrim);

            if (lastState.leftLip != ss.leftLip)
                SetServo(cd.leftLipPin, 1.0f - ss.leftLip, cd.leftLipMax, cd.leftLipMin, cd.leftLipTrim);

            if (lastState.rightLip != ss.rightLip)
                SetServo(cd.rightLipPin, 1.0f - ss.rightLip, cd.rightLipMax, cd.rightLipMin, cd.rightLipTrim);

            if (lastState.jaw != ss.jaw)
                SetServo(cd.jawPin, 1.0f - ss.jaw, cd.jawMax, cd.jawMin, cd.jawTrim);
        }

        public void ReleaseServo(int pin)
        {
            serial.SendCommand(ARDUINO_RELEASE_SERVO, pin);
        }

        public void SetServo(int pin, float value, int max, int min, int trim, bool inverted=false)
        {
            int val;
            if (inverted)
                val = (short)(((1.0f-value) * (max - min)) + min + trim);
            else
                val = (short)((value * (max - min)) + min + trim);

            if (val > max) val = max;
            if (val<min) val=min;
            serial.SendCommand(ARDUINO_SET_SERVO, pin, (short)val+1500);
        }

        public void SetServoCenter(int pin, int max, int min, int trim)
        {
            int val = (short)((0.5f * (max - min)) + min + trim);

            if (val > max) val = max;
            if (val < min) val = min;
            serial.SendCommand(ARDUINO_SET_SERVO, pin, (short)val + 1500);
        }

        public void SetServo(int pin, int value)
        {
            serial.SendCommand(ARDUINO_SET_SERVO, pin, (short)value+1500);
        }

        public void Save(short []data)
        {
            cd.leftEyebrowTrim = data[0];
            cd.leftEyebrowMax = data[1];
            cd.leftEyebrowMin = data[2];
            cd.leftEyebrowPin = data[3];
            cd.leftEyebrowEnabled = (data[4] != 0);

            cd.rightEyebrowTrim = data[5];
            cd.rightEyebrowMax = data[6];
            cd.rightEyebrowMin = data[7];
            cd.rightEyebrowPin = data[8];
            cd.rightEyebrowEnabled = (data[9] != 0);

            cd.leftLidPositionTrim = data[10];
            cd.leftLidPositionMax = data[11];
            cd.leftLidPositionMin = data[12];
            cd.leftLidPositionPin = data[13];
            cd.leftLidPositionEnabled = (data[14] != 0);

            cd.rightLidPositionTrim = data[15];
            cd.rightLidPositionMax = data[16];
            cd.rightLidPositionMin = data[17];
            cd.rightLidPositionPin = data[18];
            cd.rightLidPositionEnabled = (data[19] != 0);

            cd.leftHorizontalEyeTrim = data[20];
            cd.leftHorizontalEyeMax = data[21];
            cd.leftHorizontalEyeMin = data[22];
            cd.leftHorizontalEyePin = data[23];
            cd.leftHorizontalEyeEnabled = (data[24] != 0);

            cd.rightHorizontalEyeTrim = data[25];
            cd.rightHorizontalEyeMax = data[26];
            cd.rightHorizontalEyeMin = data[27];
            cd.rightHorizontalEyePin = data[28];
            cd.rightHorizontalEyeEnabled = (data[29] != 0);

            cd.leftVerticalEyeTrim = data[30];
            cd.leftVerticalEyeMax = data[31];
            cd.leftVerticalEyeMin = data[32];
            cd.leftVerticalEyePin = data[33];
            cd.leftVerticalEyeEnabled = (data[34] != 0);

            cd.rightVerticalEyeTrim = data[35];
            cd.rightVerticalEyeMax = data[36];
            cd.rightVerticalEyeMin = data[37];
            cd.rightVerticalEyePin = data[38];
            cd.rightVerticalEyeEnabled = (data[39] != 0);

            cd.neckTwistTrim = data[40];
            cd.neckTwistMax = data[41];
            cd.neckTwistMin = data[42];
            cd.neckTwistPin = data[43];
            cd.neckTwistEnabled = (data[44] != 0);

            cd.neckTiltTrim = data[45];
            cd.neckTiltMax = data[46];
            cd.neckTiltMin = data[47];
            cd.neckTiltPin = data[48];
            cd.neckTiltEnabled = (data[49] != 0);

            cd.leftLipTrim = data[50];
            cd.leftLipMax = data[51];
            cd.leftLipMin = data[52];
            cd.leftLipPin = data[53];
            cd.leftLipEnabled = (data[54] != 0);

            cd.rightLipTrim = data[55];
            cd.rightLipMax = data[56];
            cd.rightLipMin = data[57];
            cd.rightLipPin = data[58];
            cd.rightLipEnabled = (data[59] != 0);

            cd.jawTrim = data[60];
            cd.jawMax = data[61];
            cd.jawMin = data[62];
            cd.jawPin = data[63];
            cd.jawEnabled = (data[64] != 0);

            //cd.leftEarTrim = data[65];
            //cd.leftEarMax = data[66];
            //cd.leftEarMin = data[67];
            //cd.leftEarPin = data[68];
            //cd.leftEarEnabled = (data[69] != 0);

            //cd.rightEarTrim = data[70];
            //cd.rightEarMax = data[71];
            //cd.rightEarMin = data[72];
            //cd.rightEarPin = data[73];
            //cd.rightEarEnabled = (data[74] != 0);

            //cd.dummy = data[75];
            //cd.dummy = data[76];
            cd.sonarTriggerPin = data[77];
            cd.sonarEchoPin = data[78];
            cd.sonarEnabled = (data[79] != 0);

            //cd.dummy = data[80];
            //cd.dummy = data[81];
            //cd.dummy = data[82];
            cd.irPin = data[83];
            cd.irEnabled = (data[84] != 0);

            serial.SendCommand(ARDUINO_SAVE, data);
        }

        public void LoadConfig()
        {
            serial.SendCommand(ARDUINO_LOAD, 85);
        }

        public CalibrationData GetCalibration()
        {
            return cd;
        }

        int MinMax(int d, int min, int max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }

        int MinMaxNeg(int d, int min, int max)
        {
            if (d > 8192) d -= 16384;
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }

        public void serial_ReadCallback(object sender, EventArgs e)
        {
            int defVal;

            switch (serial.buffer[0]&127)
            {
                case ARDUINO_GET_IR:
                {
                    int x = (int)(serial.buffer[3] | (serial.buffer[4] << 7));
                    // convert voltage value to cm
                    if (irValue>10000)
                        irValue = (float)(57.653 * Math.Pow((double)x * (5.0 / 1023.0), -0.9891));
                    else
                        irValue = ((irValue * 9) + (float)(57.653 * Math.Pow((double)x * (5.0 / 1023.0), -0.9891)))/10;
                }
                break;
                case ARDUINO_GET_SONAR:
                {
                    int x = ((int)serial.buffer[3] | (serial.buffer[4] << 7));
                    // convert distance to cm
                    if (sonarValue > 10000)
                        sonarValue = (float)((float)x / 29.10f);
                    else
                        sonarValue = ((sonarValue * 9) + (float)((float)x / 29.10f)) / 10;
                }
                break;
                case ARDUINO_LOAD:
                {
                    defVal = serial.buffer[9];

                    // if many values are the same, this Arduino has not been initialized ... return to and just use pre configured values
                    if ((serial.buffer[10] == defVal) && (serial.buffer[19] == defVal) && (serial.buffer[20] == defVal) && (serial.buffer[29] == defVal) && (serial.buffer[30] == defVal)) return;

                    cd.leftEyebrowTrim = MinMaxNeg((int)serial.buffer[3]|(serial.buffer[4]<<7), -1500, 1500);
                    cd.leftEyebrowMax = MinMaxNeg((int)serial.buffer[5] | (serial.buffer[6] << 7), -1000, 1000);
                    cd.leftEyebrowMin = MinMaxNeg((int)serial.buffer[7] | (serial.buffer[8] << 7), -1000, 1000);
                    cd.leftEyebrowPin = MinMaxNeg((int)serial.buffer[9] | (serial.buffer[10] << 7), minPin, maxPin);
                    cd.leftEyebrowEnabled = (serial.buffer[11]!=0);

                    cd.rightEyebrowTrim = MinMaxNeg((int)serial.buffer[13] | (serial.buffer[14] << 7), -1500, 1500);
                    cd.rightEyebrowMax = MinMaxNeg((int)serial.buffer[15] | (serial.buffer[16] << 7), -1000, 1000);
                    cd.rightEyebrowMin = MinMaxNeg((int)serial.buffer[17] | (serial.buffer[18] << 7), -1000, 1000);
                    cd.rightEyebrowPin = MinMaxNeg((int)serial.buffer[19] | (serial.buffer[20] << 7), minPin, maxPin);
                    cd.rightEyebrowEnabled = (serial.buffer[21]!=0);

                    cd.leftLidPositionTrim = MinMaxNeg((int)serial.buffer[23] | (serial.buffer[24] << 7), -1500, 1500);
                    cd.leftLidPositionMax = MinMaxNeg((int)serial.buffer[25] | (serial.buffer[26] << 7), -1000, 1000);
                    cd.leftLidPositionMin = MinMaxNeg((int)serial.buffer[27] | (serial.buffer[28] << 7), -1000, 1000);
                    cd.leftLidPositionPin = MinMaxNeg((int)serial.buffer[29] | (serial.buffer[30] << 7), minPin, maxPin);
                    cd.leftLidPositionEnabled = (serial.buffer[31]!=0);

                    cd.rightLidPositionTrim = MinMaxNeg((int)serial.buffer[33] | (serial.buffer[34] << 7), -1500, 1500);
                    cd.rightLidPositionMax = MinMaxNeg((int)serial.buffer[35] | (serial.buffer[36] << 7), -1000, 1000);
                    cd.rightLidPositionMin = MinMaxNeg((int)serial.buffer[37] | (serial.buffer[38] << 7), -1000, 1000);
                    cd.rightLidPositionPin = MinMaxNeg((int)serial.buffer[39] | (serial.buffer[40] << 7), minPin, maxPin);
                    cd.rightLidPositionEnabled = (serial.buffer[41]!=0);

                    cd.leftHorizontalEyeTrim = MinMaxNeg((int)serial.buffer[43] | (serial.buffer[44] << 7), -1500, 1500);
                    cd.leftHorizontalEyeMax = MinMaxNeg((int)serial.buffer[45] | (serial.buffer[46] << 7), -1000, 1000);
                    cd.leftHorizontalEyeMin = MinMaxNeg((int)serial.buffer[47] | (serial.buffer[48] << 7), -1000, 1000);
                    cd.leftHorizontalEyePin = MinMaxNeg((int)serial.buffer[49] | (serial.buffer[50] << 7), minPin, maxPin);
                    cd.leftHorizontalEyeEnabled = (serial.buffer[51]!=0);

                    cd.rightHorizontalEyeTrim = MinMaxNeg((int)serial.buffer[53] | (serial.buffer[54] << 7), -1500, 1500);
                    cd.rightHorizontalEyeMax = MinMaxNeg((int)serial.buffer[55] | (serial.buffer[56] << 7), -1000, 1000);
                    cd.rightHorizontalEyeMin = MinMaxNeg((int)serial.buffer[57] | (serial.buffer[58] << 7), -1000, 1000);
                    cd.rightHorizontalEyePin = MinMaxNeg((int)serial.buffer[59] | (serial.buffer[60] << 7), minPin, maxPin);
                    cd.rightHorizontalEyeEnabled = (serial.buffer[61]!=0);

                    cd.leftVerticalEyeTrim = MinMaxNeg((int)serial.buffer[63] | (serial.buffer[64] << 7), -1500, 1500);
                    cd.leftVerticalEyeMax = MinMaxNeg((int)serial.buffer[65] | (serial.buffer[66] << 7), -1000, 1000);
                    cd.leftVerticalEyeMin = MinMaxNeg((int)serial.buffer[67] | (serial.buffer[68] << 7), -1000, 1000);
                    cd.leftVerticalEyePin = MinMaxNeg((int)serial.buffer[69] | (serial.buffer[70] << 7), minPin, maxPin);
                    cd.leftVerticalEyeEnabled = (serial.buffer[71]!=0);

                    cd.rightVerticalEyeTrim = MinMaxNeg((int)serial.buffer[73] | (serial.buffer[74] << 7), -1500, 1500);
                    cd.rightVerticalEyeMax = MinMaxNeg((int)serial.buffer[75] | (serial.buffer[76] << 7), -1000, 1000);
                    cd.rightVerticalEyeMin = MinMaxNeg((int)serial.buffer[77] | (serial.buffer[78] << 7), -1000, 1000);
                    cd.rightVerticalEyePin = MinMaxNeg((int)serial.buffer[79] | (serial.buffer[80] << 7), minPin, maxPin);
                    cd.rightVerticalEyeEnabled = (serial.buffer[81]!=0);

                    cd.neckTwistTrim = MinMaxNeg((int)serial.buffer[83] | (serial.buffer[84] << 7), -1500, 1500);
                    cd.neckTwistMax = MinMaxNeg((int)serial.buffer[85] | (serial.buffer[86] << 7), -1000, 1000);
                    cd.neckTwistMin = MinMaxNeg((int)serial.buffer[87] | (serial.buffer[88] << 7), -1000, 1000);
                    cd.neckTwistPin = MinMaxNeg((int)serial.buffer[89] | (serial.buffer[90] << 7), minPin, maxPin);
                    cd.neckTwistEnabled = (serial.buffer[91]!=0);

                    cd.neckTiltTrim = MinMaxNeg((int)serial.buffer[93] | (serial.buffer[94] << 7), -1500, 1500);
                    cd.neckTiltMax = MinMaxNeg((int)serial.buffer[95] | (serial.buffer[96] << 7), -1000, 1000);
                    cd.neckTiltMin = MinMaxNeg((int)serial.buffer[97] | (serial.buffer[98] << 7), -1000, 1000);
                    cd.neckTiltPin = MinMaxNeg((int)serial.buffer[99] | (serial.buffer[100] << 7), minPin, maxPin);
                    cd.neckTiltEnabled = (serial.buffer[101]!=0);

                    cd.leftLipTrim = MinMaxNeg((int)serial.buffer[103] | (serial.buffer[104] << 7), -1500, 1500);
                    cd.leftLipMax = MinMaxNeg((int)serial.buffer[105] | (serial.buffer[106] << 7), -1000, 1000);
                    cd.leftLipMin = MinMaxNeg((int)serial.buffer[107] | (serial.buffer[108] << 7), -1000, 1000);
                    cd.leftLipPin = MinMaxNeg((int)serial.buffer[109] | (serial.buffer[110] << 7), minPin, maxPin);
                    cd.leftLipEnabled = (serial.buffer[111] != 0);

                    cd.rightLipTrim = MinMaxNeg((int)serial.buffer[113] | (serial.buffer[114] << 7), -1500, 1500);
                    cd.rightLipMax = MinMaxNeg((int)serial.buffer[115] | (serial.buffer[116] << 7), -1000, 1000);
                    cd.rightLipMin = MinMaxNeg((int)serial.buffer[117] | (serial.buffer[118] << 7), -1000, 1000);
                    cd.rightLipPin = MinMaxNeg((int)serial.buffer[119] | (serial.buffer[120] << 7), minPin, maxPin);
                    cd.rightLipEnabled = (serial.buffer[121] != 0);

                    cd.jawTrim = MinMaxNeg((int)serial.buffer[123] | (serial.buffer[124] << 7), -1500, 1500);
                    cd.jawMax = MinMaxNeg((int)serial.buffer[125] | (serial.buffer[126] << 7), -1000, 1000);
                    cd.jawMin = MinMaxNeg((int)serial.buffer[127] | (serial.buffer[128] << 7), -1000, 1000);
                    cd.jawPin = MinMaxNeg((int)serial.buffer[129] | (serial.buffer[130] << 7), minPin, maxPin);
                    cd.jawEnabled = (serial.buffer[131] != 0);

                    //cd.leftEarTrim = MinMaxNeg((int)serial.buffer[133] | (serial.buffer[134] << 7), -1500, 1500);
                    //cd.leftEarMax = MinMaxNeg((int)serial.buffer[135] | (serial.buffer[136] << 7), -1000, 1000);
                    //cd.leftEarMin = MinMaxNeg((int)serial.buffer[137] | (serial.buffer[138] << 7), -1000, 1000);
                    //cd.leftEarPin = MinMaxNeg((int)serial.buffer[139] | (serial.buffer[140] << 7), minPin, maxPin);
                    //cd.leftEarEnabled = (serial.buffer[141] != 0);
                    
                    //cd.rightEarTrim = MinMaxNeg((int)serial.buffer[143] | (serial.buffer[144] << 7), -1500, 1500);
                    //cd.rightEarMax = MinMaxNeg((int)serial.buffer[145] | (serial.buffer[146] << 7), -1000, 1000);
                    //cd.rightEarMin = MinMaxNeg((int)serial.buffer[147] | (serial.buffer[148] << 7), -1000, 1000);
                    //cd.rightEarPin = MinMaxNeg((int)serial.buffer[149] | (serial.buffer[150] << 7), minPin, maxPin);
                    //cd.rightEarEnabled = (serial.buffer[151] != 0);

                    //cd.dummy = MinMaxNeg((int)serial.buffer[153] | (serial.buffer[154] << 7), -1500, 1500);
                    //cd.dummy = MinMaxNeg((int)serial.buffer[155] | (serial.buffer[156] << 7), -1000, 1000);
                    cd.sonarTriggerPin = MinMaxNeg((int)serial.buffer[157] | (serial.buffer[158] << 7), minPin, maxPin);
                    cd.sonarEchoPin = MinMaxNeg((int)serial.buffer[159] | (serial.buffer[160] << 7), minPin, maxPin);
                    cd.sonarEnabled = (serial.buffer[161] != 0);

                    //cd.dummy = MinMaxNeg((int)serial.buffer[163] | (serial.buffer[164] << 7), -1500, 1500);
                    //cd.dummy = MinMaxNeg((int)serial.buffer[165] | (serial.buffer[166] << 7), -1000, 1000);
                    //cd.dummy = MinMaxNeg((int)serial.buffer[167] | (serial.buffer[168] << 7), minPin, maxPin);
                    cd.irPin = MinMaxNeg((int)serial.buffer[169] | (serial.buffer[170] << 7), minPin, maxPin);
                    cd.irEnabled = (serial.buffer[171] != 0);
                }
                break;
            }
        }

        public void SetJaw(float val)
        {
            SetServo(cd.jawPin, 1.0f - val, cd.jawMax, cd.jawMin, cd.jawTrim);
        }

        public void SetNeckTwist(float val)
        {
            SetServo(cd.neckTwistPin, val, cd.neckTwistMax, cd.neckTwistMin, cd.neckTwistTrim);
        }

        public void SetNeckTilt(float val)
        {
            SetServo(cd.neckTiltPin, 1.0f - val, cd.neckTiltMax, cd.neckTiltMin, cd.neckTiltTrim);
        }

        public void SetEyelids(float val)
        {
            SetServo(cd.rightLidPositionPin, 1.0f - val, cd.rightLidPositionMax, cd.rightLidPositionMin, cd.rightLidPositionTrim);
            SetServo(cd.leftLidPositionPin, 1.0f - val, cd.leftLidPositionMax, cd.leftLidPositionMin, cd.leftLidPositionTrim);
        }

        public void SetLeftEyelid(float val)
        {
            SetServo(cd.leftLidPositionPin, 1.0f - val, cd.leftLidPositionMax, cd.leftLidPositionMin, cd.leftLidPositionTrim);
        }

        public void SetRightEyelid(float val)
        {
            SetServo(cd.rightLidPositionPin, 1.0f - val, cd.rightLidPositionMax, cd.rightLidPositionMin, cd.rightLidPositionTrim);
        }

        public void SetEyebrows(float val)
        {
            SetServo(cd.rightEyebrowPin, 1.0f-val, cd.rightEyebrowMax, cd.rightEyebrowMin, cd.rightEyebrowTrim);
            SetServo(cd.leftEyebrowPin, val, cd.leftEyebrowMax, cd.leftEyebrowMin, cd.leftEyebrowTrim);
        }

        public void SetLeftEyebrow(float val)
        {
            SetServo(cd.leftEyebrowPin, val, cd.leftEyebrowMax, cd.leftEyebrowMin, cd.leftEyebrowTrim);
        }

        public void SetRightEyebrow(float val)
        {
            SetServo(cd.rightEyebrowPin, 1.0f-val, cd.rightEyebrowMax, cd.rightEyebrowMin, cd.rightEyebrowTrim);
        }

        public void SetEyesHorizontal(float val)
        {
            SetServo(cd.rightHorizontalEyePin, val, cd.rightHorizontalEyeMax, cd.rightHorizontalEyeMin, cd.rightHorizontalEyeTrim);
            SetServo(cd.leftHorizontalEyePin, val, cd.leftHorizontalEyeMax, cd.leftHorizontalEyeMin, cd.leftHorizontalEyeTrim);
        }

        public void SetLeftEyeHorizontal(float val)
        {
            SetServo(cd.leftHorizontalEyePin, val, cd.leftHorizontalEyeMax, cd.leftHorizontalEyeMin, cd.leftHorizontalEyeTrim);
        }

        public void SetRightEyeHorizontal(float val)
        {
            SetServo(cd.rightHorizontalEyePin, val, cd.rightHorizontalEyeMax, cd.rightHorizontalEyeMin, cd.rightHorizontalEyeTrim);
        }

        public void SetEyesVertical(float val)
        {
            SetServo(cd.rightVerticalEyePin, val, cd.rightVerticalEyeMax, cd.rightVerticalEyeMin, cd.rightVerticalEyeTrim);
            SetServo(cd.leftVerticalEyePin, 1.0f-val, cd.leftVerticalEyeMax, cd.leftVerticalEyeMin, cd.leftVerticalEyeTrim);
        }

        public void SetLips(float val)
        {
            SetServo(cd.rightLipPin, 1.0f-val, cd.rightLipMax, cd.rightLipMin, cd.rightLipTrim);
            SetServo(cd.leftLipPin, val, cd.leftLipMax, cd.leftLipMin, cd.leftLipTrim);
        }

        public void SetLeftLip(float val)
        {
            SetServo(cd.leftLipPin, val, cd.leftLipMax, cd.leftLipMin, cd.leftLipTrim);
        }

        public void SetRightLip(float val)
        {
            SetServo(cd.rightLipPin, 1.0f - val, cd.rightLipMax, cd.rightLipMin, cd.rightLipTrim);
        }

        public void ResetDistances()
        {
            irValue = 1000.0f;
            sonarValue = 1000.0f;
        }

        public float GetIRValue()
        {
            serial.SendCommand(ARDUINO_GET_IR, cd.irPin - 14, (short)0);
            // yes, the value is not exactly the current value ... but is the last value
            return irValue;
        }

        public float GetIRValue(int pin)
        {
            serial.SendCommand(ARDUINO_GET_IR, pin-14, (short)0);
            // yes, the value is not exactly the current value ... but is the last value
            return irValue;
        }

        public float GetSonarValue()
        {
            serial.SendCommand(ARDUINO_GET_IR, cd.sonarTriggerPin, (short)cd.sonarEchoPin);
            // yes, the value is not exactly the current value ... but is the last value
            return sonarValue;
        }

        public float GetSonarValue(int triggerPin, int echoPin)
        {
            serial.SendCommand(ARDUINO_GET_SONAR, triggerPin, (short)echoPin);
            // yes, the value is not exactly the current value ... but is the last value
            return sonarValue;
        }
    }
}

