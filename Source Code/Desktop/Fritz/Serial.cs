using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.Management;

namespace Fritz
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            m_value = value;
        }

        private T m_value;

        public T Value
        {
            get { return m_value; }
        }
    }

    public class EventArgs<T, U> : EventArgs<T>
    {

        public EventArgs(T value, U value2)
            : base(value)
        {
            m_value2 = value2;
        }

        private U m_value2;

        public U Value2
        {
            get { return m_value2; }
        }
    }

    public class Serial
    {
        private SerialPort commPort = new SerialPort();
        String serialPort = null;

        const int BAUD_RATE = 57600;
        const int SEND_QUEUE_MAX = 4096;

        byte[] sendQueue = new byte[SEND_QUEUE_MAX];
        public byte[] buffer = new byte[SEND_QUEUE_MAX];

	    int sendQueueTop;
	    int sendQueueBottom;
        bool isRunning = true;

        public bool foundBoard = false;

        Thread thread;

        Object queueLock = new Object();

        public event EventHandler ReadCallback = null;

        private AutoResetEvent newData = new AutoResetEvent(false);

        public Serial()
        {
            thread = (new Thread(new ThreadStart(WorkThreadFunction)));
            thread.Start();
        }

        void TestPortFunction(object port)
        {
            SerialPort tstPort = new SerialPort();

            tstPort.PortName = (string)port;

            tstPort.BaudRate = BAUD_RATE;
            tstPort.DataBits = 8;
            tstPort.Parity = System.IO.Ports.Parity.None;
            tstPort.StopBits = System.IO.Ports.StopBits.One;

            // Set the read/write timeouts
            tstPort.ReadTimeout = 5000;
            tstPort.WriteTimeout = 1000;

            try
            {
                tstPort.Open();

                byte[] buffer = new byte[2];

                buffer[0] = 128;
                buffer[1] = 0;

                tstPort.Write(buffer, 0, 2);
/*
                char []tst = new char[32];
                int cnt=0;

                while (true)
                {
                    tst[cnt++] = (char)tstPort.ReadByte();
                }
 */ 
                byte b1 = (byte)tstPort.ReadByte();
                byte b2 = (byte)tstPort.ReadByte();
                byte b3 = (byte)tstPort.ReadByte();
                byte b4 = (byte)tstPort.ReadByte();
                byte v1 = (byte)tstPort.ReadByte();
                byte v2 = (byte)tstPort.ReadByte();
                byte v3 = (byte)tstPort.ReadByte();

                int version = ((v1-'0')*100)+((v2-'0')*10)+(v3-'0');

                if ((b1 == 'A') && (b2 == 'R') && (b3 == 'D') && (b4 == 'U'))
                {
                    if (version < 4)
                    {
                        MessageBox.Show("Fritz found but firmware version is too old!\nFound "+version+" but this application requires 3 and above.");
                        tstPort.Close();
                        tstPort.Dispose();
                    }
                    else
                    {
                        serialPort = (String)port;
                        foundBoard = true;
                        commPort = tstPort;
                    }
                }
                else
                {
                    tstPort.Close();
                    tstPort.Dispose();
                }
            }
            catch (System.Exception)
            {
                tstPort.Close();
                tstPort.Dispose();
            }
        }

        public bool IsConnected()
        {
            return foundBoard;
        }

        public bool FindBoard()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            serialPort = null;

            int count = 0;

            foundBoard = false;

            Thread[] thread = new Thread[20];

            // Display each port name to the console.
            foreach(string port in ports)
            {
                thread[count] = (new Thread(new ParameterizedThreadStart(TestPortFunction)));
                thread[count].Start(port);
                count++;

                // if we've open up many threads given them a chance to finish
                if ((count % 20) == 0)
                {
                    for (int i = 0; i < 20; i++)
                        thread[i].Join();

                    count = 0;
                }
            }

            // wait for all threads to get done
            for (int i = 0; (i < count)&&(!foundBoard); i++)
                thread[i].Join();

            return foundBoard;
        }

        public bool Open()
        {
            foundBoard = false;

            CloseCommPort();

            commPort.Dispose();

            // if connection was lost, try using same port as before
            if (serialPort != null)
            {
                TestPortFunction(serialPort);
                if (foundBoard)
                    return true;
            }

            // check last saved value before checking all ports (can take a couple seconds)
            String port = Fritz.Properties.Settings.Default.Last_COM_PORT;
            if ((port!=null)&&(port.Length != 0))
            {
                TestPortFunction(port);
                if (foundBoard)
                    return true;
            }

            // check all possible COM ports
            if (FindBoard())
            {
                Fritz.Properties.Settings.Default.Last_COM_PORT = serialPort;
                Fritz.Properties.Settings.Default.Save();
                return true;
            }

            // Nothing found, but keep trying 
            return false;
        }

        public bool CloseCommPort()
        {
            if (commPort.IsOpen)
            {
                //commPort.DiscardOutBuffer();
                commPort.Close();
                return true;
            }
            return false;
        }

        void LogText(String text)
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter("Serial.log", true);

            // write a line of text to the file
            text = text.Replace("\f", "\\f");
            text = text.Replace("\r", "\\r");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\t", "\\t");
            tw.WriteLine(DateTime.Now + " " + text);

            // close the stream
            tw.Close();
        }

        bool SendPacket(byte[] buffer, int pl, int checkLen)
        {
	        int space;
	        if (sendQueueBottom<sendQueueTop)
		        space = SEND_QUEUE_MAX - (sendQueueTop - sendQueueBottom);
	        else
	        if (sendQueueBottom>sendQueueTop)
		        space = SEND_QUEUE_MAX - (sendQueueTop + (SEND_QUEUE_MAX - sendQueueBottom));
	        else
		        space = SEND_QUEUE_MAX;

	        if (space<pl) 
                return false;

	        lock (queueLock)
            {
	            // check that packet is not already in queue in case of slow MCU
	            int i=sendQueueBottom;
	            while (i!=sendQueueTop)
	            {
		            int l = sendQueue[i++];
		            if (i>=SEND_QUEUE_MAX) i=0;
		            l|= sendQueue[i++]<<8;
		            if (i>=SEND_QUEUE_MAX) i=0;

		            if (l==pl)
		            {
			            int stop = i+pl;
			            int p=0, j;
			            if (stop>=SEND_QUEUE_MAX) stop-=SEND_QUEUE_MAX;
			            for (j=i;(j!=stop)&&(sendQueue[j]==buffer[p])&&(p<checkLen);p++)
				            if (++j>=SEND_QUEUE_MAX) j=0;

                        if (p > checkLen)
                        {
                            for (p = 0, j = i; p < pl; p++)
                            {
                                sendQueue[j] = buffer[p];
                                if (++j >= SEND_QUEUE_MAX) j = 0;
                            }

                            newData.Set();
                            return true;
                        }
		            }
		            i+=l;
		            if (i>=SEND_QUEUE_MAX) i-=SEND_QUEUE_MAX;
	            }
	            sendQueue[sendQueueTop++] = (byte)(pl&255);
	            if (sendQueueTop>=SEND_QUEUE_MAX) sendQueueTop=0;
	            sendQueue[sendQueueTop++] = (byte)((pl>>8)&255);
	            if (sendQueueTop>=SEND_QUEUE_MAX) sendQueueTop=0;

	            int end = sendQueueTop+pl;
	            if (end>=SEND_QUEUE_MAX) end-=SEND_QUEUE_MAX;
	            int pp=0;
	            while (sendQueueTop!=end)
	            {
		            sendQueue[sendQueueTop++] = buffer[pp++];
		            if (sendQueueTop>=SEND_QUEUE_MAX) sendQueueTop=0;
	            }
            }

            newData.Set();

            return true;
        }

        public void Stop()
        {
            isRunning = false;
        }

        void WorkThreadFunction()
        {
            byte[] reply = new byte[16];

            while (isRunning)
            {
                //Thread.Sleep(10);
                newData.WaitOne(1000);

                if (!commPort.IsOpen)
                {
                    Open();
                    if (!commPort.IsOpen)
                        continue;
                }

                lock (queueLock)
                {
                    while ((sendQueueBottom != sendQueueTop) && isRunning)
                    {
                        int l = sendQueue[sendQueueBottom++];
                        if (sendQueueBottom >= SEND_QUEUE_MAX) sendQueueBottom = 0;
                        l |= sendQueue[sendQueueBottom++] << 8;
                        if (sendQueueBottom >= SEND_QUEUE_MAX) sendQueueBottom = 0;

                        int stop = sendQueueBottom + l;
                        if (stop >= SEND_QUEUE_MAX) stop -= SEND_QUEUE_MAX;
                        if (sendQueue[sendQueueBottom] == 0)
                        {
                            // if zero is the first byte then ignore this packet
                            sendQueueBottom = stop;
                        }
                        else
                        {
                            int i = 0;
                            // add command
                            buffer[i++] = sendQueue[sendQueueBottom++];
                            if (sendQueueBottom >= SEND_QUEUE_MAX) sendQueueBottom -= SEND_QUEUE_MAX;

                            int ml = l - 2;
                            if (ml < 0) ml = 0;

                            // add length (-1 is for CRC which is not included in length)
                            buffer[i++] = (byte)(ml&127);

                            // if its a command > 32 it has a two byte length
                            if ((buffer[0] & 127) >= 32)
                                buffer[i++] = (byte)(ml >> 7);

                            while ((sendQueueBottom != stop) && (i < SEND_QUEUE_MAX))
                            {
                                buffer[i++] = sendQueue[sendQueueBottom++];
                                if (sendQueueBottom >= SEND_QUEUE_MAX) sendQueueBottom -= SEND_QUEUE_MAX;
                            }

                            try
                            {
                                commPort.Write(buffer, 0, i);
                                Read(buffer, 0, 3);
                                int len;

                                if ((buffer[0] & 127) >= 32)
                                    len = (buffer[1] | (buffer[2]<<7));
                                else
                                    len = buffer[1];

                                if (len > 0)
                                    Read(buffer, 3, len);

                                // read in response
                                if (ReadCallback != null)
                                    ReadCallback(this, EventArgs.Empty);
                            }
                            catch (Exception)
                            {
                                sendQueueBottom = sendQueueTop = 0;
                                try
                                {
                                    commPort.Close();
                                }
                                catch (Exception)
                                {
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Read(byte []buffer, int offset, int len)
        {
            while (true)
            {
                int l = commPort.Read(buffer, offset, len);
                offset+=l;
                len-=l;
                if (len <= 0) return;
            }
        }

        public void SendCommand(int cmd)
        {
	        byte[] buffer = new byte[1];

	        buffer[0] = (byte)(128|cmd);

	        SendPacket(buffer, 1, 1);
        }

        public void SendCommand(int cmd, int pin)
        {
	        byte[] buffer = new byte[3];

	        buffer[0] = (byte)(128|cmd);
	        buffer[1] = (byte)(pin&127);
            buffer[2] = (byte)((buffer[0] ^ buffer[1] ^ 1) & 127);

	        SendPacket(buffer, 3, 2);
        }

        public void SendCommand(int cmd, int pin, int value)
        {
	        byte[] buffer = new byte[10];

	        buffer[0] = (byte)(128|cmd);
	        buffer[1] = (byte)(pin&127);
	        buffer[2] = (byte)(value&127);
	        buffer[3] = (byte)((value>>7)&127);
	        buffer[4] = (byte)((value>>14)&127);
	        buffer[5] = (byte)((value>>21)&127);
	        buffer[6] = (byte)((value>>28)&127);
            buffer[7] = (byte)((buffer[0] ^ buffer[1] ^ buffer[2] ^ buffer[3] ^ buffer[4] ^ buffer[5] ^ buffer[6] ^ 6) & 127);

            SendPacket(buffer, 8, 2);
        }

        public void SendCommand(int cmd, int pin, short value)
        {
            byte[] buffer = new byte[5];

            buffer[0] = (byte)(128 | cmd);
            buffer[1] = (byte)(pin & 127);
            buffer[2] = (byte)(value & 127);
            buffer[3] = (byte)((value >> 7) & 127);
            buffer[4] = (byte)((buffer[0] ^ buffer[1] ^ buffer[2] ^ buffer[3] ^ 3) & 127);

            SendPacket(buffer, 5, 2);
        }

        public void SendCommand(int cmd, int pin, byte value)
        {
	        byte[] buffer = new byte[4];

	        buffer[0] = (byte)(128|cmd);
	        buffer[1] = (byte)(pin&127);
	        buffer[2] = (byte)(value&127);
	        buffer[3] = (byte)((buffer[0]^buffer[1]^buffer[2]^2)&127);

            SendPacket(buffer, 4, 2);
        }

        public void SendCommand(int cmd, short[] dat)
        {
            byte[] buffer = new byte[4096];

            buffer[0] = (byte)(128 | cmd);
           
            int i, j;
            for (j = 1, i = 0; (i < dat.Length); i++)
            {
                buffer[j++] = (byte)(dat[i] & 127);
                buffer[j++] = (byte)((dat[i] >> 7)&127);
            }

            int crc = buffer[0];
            crc ^= (j - 1) & 127;
            crc ^= (j - 1) >> 7;
            for (i = 1; i < j; i++) crc ^= buffer[i];

            buffer[j++] = (byte)(crc & 127);

            SendPacket(buffer, j, 1);
        }

/*
        public void SendCommand(int cmd, int pin, byte[] txt)
        {
	        byte[] buffer = new byte[4096];

	        buffer[0] = (byte)(128|cmd);
	        buffer[1] = (byte)(pin);
	        int i,j;
	        for (j=2,i=0;(txt[i]!=0)&&(j<4090);i++)
		        buffer[j++]=(byte)(txt[i]&127);

	        buffer[j++]=0;

	        int crc=0;
	        for (i=0;i<j;i++) crc^=buffer[i];
	        buffer[j++] = (byte)(crc&127);

            SendPacket(buffer, j - 2);
        }
 */ 
    }
}
