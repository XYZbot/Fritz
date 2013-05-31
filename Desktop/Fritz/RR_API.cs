using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

namespace RoboRealm
{
    public class Program
    {
        // test routine to exercise the API below. This is a collection of routines to use
        // as examples. They do not perform any logical action and are for examples
        // only. You can keep these examples as reference 
/*
        static void Main(string[] args)
        {
            RR_API rr = new RR_API();

            if (!rr.connect("localhost"))
            {
              Console.WriteLine("Could not connect to localhost!");
              return;
            }

            Dimension d = rr.getDimension();
            if (d != null)
            {
                Console.WriteLine("Dimension " + d.width + "x" + d.height);
            }
             
            // VARIABLES

            // set a custom variable to test
            rr.setVariable("custom_var", "test");

            // read back our custom variable ... should be equal to 'test'
            String res = rr.getVariable("custom_var");
            if (!res.Equals("test"))
            {
              Console.WriteLine("Error in custom_var");
              return;
            }

            // delete our custom variable
            if (!rr.deleteVariable("custom_var"))
            {
              Console.WriteLine("Error in delete variable");
              return;
            }

            // try to get it back again ... should be empty
            res = rr.getVariable("custom_var");
            if (res!=null)
            {
              Console.WriteLine("Error in delete custom_var");
              return;
            }

            // set multiple variables
            String[] names = new String[2];
            String[] values = new String[2];
            names[0]="custom_var_1";
            names[1]="custom_var_2";
            values[0] = "test1";
            values[1] = "test2";
            rr.setVariables(names, values, 2);

            // get multiple variables
            List<string> v = rr.getVariables("custom_var_1, custom_var_2");
            if (v==null)
            {
              Console.WriteLine("Error in GetVariables, did not return any results");
              return;
            }
            else
            {
              if (!v[0].Equals("test1"))
              {
                Console.WriteLine("Error in get/set multiple variables. Got "+v[0]);
                return;
              }
              if (!v[1].Equals("test2"))
              {
                Console.WriteLine("Error in get/set multiple variables. Got "+v[1]);
                return;
              }
            }

            // IMAGES
            byte[] image = new byte[1280*960];

            // ensure that the camera is on and processing images
            rr.setCamera("on");
            rr.run("on");

            // execute a RGB filter on the loaded image
            rr.execute("<head><version>1.50</version></head><RGB_Filter><min_value>40</min_value><channel>3</channel></RGB_Filter>");

            // get the current processed image from RoboRealm and save as a PPM
            d = rr.getImage(image, 1280 * 960);
            if (d != null)
            {
                rr.savePPM("c:\\temp\\test.ppm", image, d.width, d.height);
            }

            // get the current source image from RoboRealm and save as a PPM
            d = rr.getImage("source", image, 1280 * 960);
            if (d != null)
            {
                rr.savePPM("c:\\temp\\test2.ppm", image, d.width, d.height);
            }

            // turn off live camera
            rr.setCamera("off");

            // load an image for experimentation
            d = rr.loadPPM("c:\\www\\RoboRealm\\bin\\remo.ppm", image, 320 * 240 * 3);

            // change the current image
            rr.setImage(image, d.width, d.height);

            // add a marker image called mt_new_image
            rr.setImage("my_new_image", image, d.width, d.height);

            // run a .robo program
            rr.loadProgram("c:\\www\\RoboRealm\\scripts\\red.robo");

            // load an image from disk
            rr.loadImage(null, "c:\\www\\RoboRealm\\bin\\remo.gif");

            // save that image back to disk .. note that we can switch extensions
            rr.saveImage(null, "c:\\temp\\remo.jpg");

            rr.setCamera("on");
            // change the camera to another one
            rr.setCamera("CompUSA PC Camera");
            System.Threading.Thread.Sleep(2000);
            // now set it back
            rr.setCamera("Logitech");

            // turn off processing
            rr.run("off");
            System.Threading.Thread.Sleep(2000);
            // run once
            rr.run("once");
            System.Threading.Thread.Sleep(2000);
            // run for 100 frames (~3.3 seconds) .. note that if your frame rate is different this
            // may be longer than 4 seconds
            rr.run("100");
            System.Threading.Thread.Sleep(4000);
            // turn processing back on
            rr.run("on");

            // wait for the image count to exceed 1000 (assuming a 30 fps here)
            rr.waitVariable("image_count", "500", 100000);

            // wait for a new image
            rr.waitImage(5000);

            // close the RoboRealm application .. if you want too ... otherwise leave it running
            rr.close();

            // disconnect from API Server
            rr.disconnect();

            Console.ReadKey();
        }
 */ 
    }

    /* Small class used when both width and height are needed as return values */
    public class Dimension
    {
        public int width;
        public int height;

        public Dimension(int w, int h)
        {
            width = w;
            height = h;
        }
    }

    /* The following XML class is a simple XML class meant to process the primitive XML
     * that comes from RoboRealm. This class can be removed and replaced with a more
     * extensive XML processing class as needed but is guaranteed to work with the RR
     * XML. Do NOT use this class for generic XML processing as it is included for
     * completeness and is intentionally kept simplistic to ease understanding 
     * */
    class XML
    {
        Dictionary<string, string> table = new Dictionary<string, string>();
        List<string> list = new List<string>();

        /*
        Unescapes strings that have been included in an XML message. This can be
        accomplished by a sequence of replace statements.
          &amp; -> &
          &quote; -> "
          &lt; -> <
          &gt; -> >
        */
        private String unescape(String txt)
        {
            txt = txt.Replace("&amp;", "&");
            txt = txt.Replace("&quote;", "\"");
            txt = txt.Replace("&lt;", "<");
            txt = txt.Replace("&gt;", ">");
            return txt;
        }

        public bool parse(String s)
        {
            table.Clear();
            return parse(s, table, null);
        }

        public List<string> parseVector(String s)
        {
            list.Clear();
            if (parse(s, null, list))
                return list;
            else
                return null;
        }

        public bool parse(String s, Dictionary<string,string> h, List<string> v)
        {
            bool isEndTag;
            byte[] txt = Encoding.ASCII.GetBytes(s);
            int i, j;
            int len = s.Length;
            StringBuilder[] keys = new StringBuilder[10];
            StringBuilder value = new StringBuilder();
            for (i=0;i<10;i++)
              keys[i] = new StringBuilder();
            int keyTop=-1;

            for (i=0;i<len;)
            {
              // read in key
              if (txt[i]=='<')
              {
                i++;
                if (txt[i]=='/')
                {
                  isEndTag = true;
                  i++;
                }
                else
                  isEndTag = false;

                keyTop++;
                keys[keyTop].Length =0;
                while ((i<len)&&(txt[i]!='>'))
                {
                  keys[keyTop].Append((char)txt[i]);
                  i++;
                }
                if (txt[i++]!='>')
                {
                  Console.WriteLine("Missing close > tag");
                  return false;
                }

                if (isEndTag)
                {
                  if (!keys[keyTop].ToString().Equals(keys[keyTop-1].ToString()))
                  {
                    Console.WriteLine("Mismatched XML tags " + keys[keyTop] + " -> " + keys[keyTop - 1]);
                    return false;
                  }
                  keyTop-=2;
                }
              }
              else
              {
                // read in value
                value.Length = 0;

                while ((i<len)&&(txt[i]!='<'))
                {
                  value.Append((char)txt[i]);
                  i++;
                }

                StringBuilder key = new StringBuilder();
                for (j=0;j<=keyTop;j++)
                {
                  if (j>0) key.Append('.');
                  key.Append(keys[j]);
                }

                String escapedValue = unescape(value.ToString());
                if (h!=null) h.Add(key.ToString(), escapedValue);
                if (v!=null) v.Add(escapedValue);
              }
            }

            return true;
        }

        public int getInt(String txt)
        {
            String s = (String)table[txt];
            if (s != null)
            {
                return Convert.ToInt32(s);
            }
            return 0;
        }

        public String getFirst()
        {
            if (table.Count == 0)
                return null;
            else
            {
                Dictionary<string, string>.Enumerator tableEnum = table.GetEnumerator();
                tableEnum.MoveNext();
                return tableEnum.Current.Value;
            }
        }
        /*
          // Test for XML class
          public static void main(String[] args)
          {
            XML xml = new XML();
            xml.parse("<response><width>100</width><height>200</height></response>");
            Console.WriteLine(xml.getInt("response.width"));
            Console.WriteLine(xml.getInt("response.height"));
          }
        */
    }

    /* Following starts the main RoboRealm API. These are wrapper routines around the XML
     * messages sent to RoboRealm. See the online documentation at http://www.roborealm.com/help/API.php
     * for details about those messages */
    public class RR_API
    {
        [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint WaitForSingleObject(uint hHandle, int dwMilliseconds);

        [DllImport("kernel32", EntryPoint = "CreateEvent", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint CreateEvent(String dummy1, bool defaultSecurity, bool initialState, String name);

        // default read and write socket timeout
        public const int DEFAULT_TIMEOUT = 60000;

        // the port number to listen on ... needs to match that used in RR interface
        public const int SERVER_PORTNUM = 6060;

        // indicates that the application is connected to RoboRealm Server
        bool connected = false;

        // holds the previously read data size
        int lastDataTop = 0;

        // holds the previously read data buffer
        int lastDataSize = 0;

        // general buffer for data manipulation and socket reading
        byte[] buffer = new byte[4096];

        // our instance of our primitive XML parser
        XML xml = new XML();

        // socket based reader and writer objects
        BinaryReader binaryReader;
        BinaryWriter binaryWriter;

        // out main socket handle
        Socket handle;

        private const int DEFAULT_ROBOREALM_TIMEOUT = 10000;

        /******************************************************************************/
        /* Text string manipulation routines */
        /******************************************************************************/

        /*
        Escapes strings to be included in XML message. This can be accomplished by a
        sequence of replace statements.
          & -> &amp;
          " -> &quote;
          < -> &lt;
          > -> &gt;
        */
        private String escape(String txt)
        {
            txt = txt.Replace("&", "&amp;");
            txt = txt.Replace("\"", "&quote;");
            txt = txt.Replace("<", "&lt;");
            txt = txt.Replace(">", "&gt;");
            return txt;
        }

        /******************************************************************************/
        /* Socket Routines */
        /******************************************************************************/

        /* Initiates a socket connection to the RoboRealm server */
        public bool connect(String hostname)
        {
            connected = false;

            try
            {
                handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, DEFAULT_TIMEOUT);
                handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, DEFAULT_TIMEOUT);

                handle.Connect(hostname, SERVER_PORTNUM);

                NetworkStream networkStream = new NetworkStream(handle);
                binaryWriter = new BinaryWriter(networkStream);
                binaryReader = new BinaryReader(networkStream);
            }
            catch (SocketException e2)
            {
                //Unable to open connection to RoboRealm port 6060
                Console.WriteLine(e2.ToString());
                return false;
            }

            connected = true;

            return true;
        }

        public bool IsConnected()
        {
            return connected;
        }

        /* close the socket handle */
        public void disconnect()
        {
            try
            {
                if (connected)
                {
                    binaryReader.Close();
                    binaryWriter.Close();
                    handle.Close();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // cause the roborealm application to close
        public bool close()
        {
            if (!connected) return false;

            if (send("<request><close/></request>"))
            {
                // read in variable length
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    return buffer.Equals("<response>ok</response>");
                }
            }

            return false;
        }

        // sends a String over the socket port to RoboRealm
        private bool send(String txt)
        {
            try
            {
                binaryWriter.Write(Encoding.ASCII.GetBytes(txt));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /*
        Buffered socket image read. Since we don't know how much data was read from a
        previous socket operation we have to add in any previously read information
        that may still be in our buffer. We detect the end of XML messages by the
        </response> tag but this may require reading in part of the image data that
        follows a message. Thus when reading the image data we have to move previously
        read data to the front of the buffer and continuing reading in the
        complete image size from that point.
        */

        public int readImageData(byte[] pixels, int len)
        {
            int num;

            // check if we have any information left from the previous read
            num = lastDataSize - lastDataTop;
            if (num > len)
            {
                Buffer.BlockCopy(buffer, lastDataTop, pixels, 0, len);
                lastDataTop += num;
                return num;
            }
            Buffer.BlockCopy(buffer, lastDataTop, pixels, 0, num);
            len -= num;
            lastDataSize = lastDataTop = 0;

            // then keep reading until we're read in the entire image length
            do
            {
                int res;
                try
                {
                    res = binaryReader.Read(pixels, num, len);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    return 0;
                }

                if (res < 0)
                {
                    lastDataSize = lastDataTop = 0;
                    return -1;
                }
                num += res;
                len -= res;
            }
            while (len > 0);

            return num;
        }

        /* If an image is too large for the provided buffer the rest of the data needs
        to be skipped so we can continue to interact with the XML API. This routine
        will remove that additional data from the socket*/
        public int skipData(int len)
        {
            int num;

            // check if we have any information left from the previous read
            num = lastDataSize - lastDataTop;
            if (num > len)
            {
                lastDataTop += num;
                return num;
            }
            len -= num;
            lastDataSize = lastDataTop = 0;

            try
            {
                int skip = len;
                while (skip > 0)
                    skip -= binaryReader.Read(buffer, 0, skip > 4096 ? 4096 : skip);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                return 0;
            }

            return num + len;
        }

        /* Read's in an XML message from the RoboRealm Server. The message is always
        delimited by a </response> tag. We need to keep reading in information until
        this tag is seen. Sometimes this will accidentally read more than needed
        into the buffer such as when the message is followed by image data. We
        need to keep this information for the next readImage call.*/
        private String readMessage()
        {
            int num = 0;
            byte[] delimiter = Encoding.ASCII.GetBytes("</response>");
            int top = 0;
            int i;

            // read in blocks of data looking for the </response> delimiter
            while (true)
            {
                int res;
                try
                {
                    res = binaryReader.Read(buffer, num, 4096 - num);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }

                if (res < 0)
                {
                    lastDataSize = lastDataTop = 0;
                    return null;
                }

                lastDataSize = num + res;
                for (i = num; i < num + res; i++)
                {
                    if (buffer[i] == delimiter[top])
                    {
                        top++;
                        if (top >= delimiter.Length)
                        {
                            num = i + 1;
                            buffer[num] = 0;
                            lastDataTop = num;
                            return System.Text.Encoding.ASCII.GetString(buffer, 0, num);
                        }
                    }
                    else
                        top = 0;
                }
                num += res;
            }
        }

        /******************************************************************************/
        /* API Routines */
        /******************************************************************************/

        /* Returns the current image dimension */
        public Dimension getDimension()
        {
            if (!connected) return null;

            if (send("<request><get_dimension/></request>"))
            {
                // read in variable length
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (xml.parse(buffer))
                    {
                        return new Dimension(xml.getInt("response.width"), xml.getInt("response.height"));
                    }
                }
            }

            return null;
        }

        /*
        Returns the current processed image.
          pixels  - output - contains RGB 8 bit byte.
          width - output - contains grabbed image width
          height - output - contains image height
          len - input - maximum size of pixels to read
        */

        public Dimension getImage(byte[] pixels, int len)
        {
            return getImage((String)"processed", pixels, len);
        }

        /*
        Returns the named image.
          name - input - name of image to grab. Can be source, processed, or marker name.
          pixels  - output - contains RGB 8 bit byte.
          width - output - contains grabbed image width
          height - output - contains image height
          len - input - maximum size of pixels to read
        */

        public Dimension getImage(String name, byte[] pixels, int max)
        {
            if (!connected) return null;
            if (name == null) name = "";

            // create the message request
            if (send("<request><get_image>" + escape(name) + "</get_image></request>"))
            {
                String buffer;
                // read in response which contains image information
                if ((buffer = readMessage()) != null)
                {
                    // parse image width and height
                    xml.parse(buffer);
                    int len = xml.getInt("response.length");
                    int width = xml.getInt("response.width");
                    int height = xml.getInt("response.height");
                    // ensure that we have enough room in pixels
                    if (len > max)
                    {
                        skipData(len);
                        return null;
                    }

                    // actual image data follows the message
                    if (readImageData(pixels, len) == len)
                        return new Dimension(width, height);
                }
            }

            return null;
        }

        // Create RGB24 bitmap from Byte array 
        public Bitmap getImageBitmap(String marker, int maxWidth, int maxHeight) 
        { 

            byte[] bytes = new byte[maxWidth * maxHeight * 3];
            
            Dimension d = getImage(marker, bytes, maxWidth*maxHeight*3);
            if (d==null) return null;
            
            //swap RGB to BGR 
            byte tmp; 
            int len = d.width*d.height*3;
            for (int x = 0; x < len; x += 3) 
            { 
                tmp = bytes[x + 2]; 
                bytes[x + 2] = bytes[x]; 
                bytes[x] = tmp; 
            } 

            Bitmap bmp = new Bitmap(d.width, d.height, PixelFormat.Format24bppRgb); 
            int i; 

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat); 

            if (data.Stride == d.width * 3) 
            { 
                Marshal.Copy(bytes, 0, data.Scan0, d.width * d.height * 3); 
            } 
            else 
            { 
                for (i = 0; i < bmp.Height; i++) 
                { 
                    IntPtr p = new IntPtr(data.Scan0.ToInt32() + data.Stride * i); 
                    Marshal.Copy(bytes, i * bmp.Width * 3, p, bmp.Width * 3); 
                } 
            } 

            bmp.UnlockBits(data); 

            return bmp; 
        } 


        /*
        Sets the current source image.
          pixels  - input - contains RGB 8 bit byte.
          width - input - contains grabbed image width
          height - input - contains image height
        */

        public bool setImage(byte[] pixels, int width, int height)
        {
            return setImage(null, pixels, width, height);
        }

        /*
        Sets the current source image.
          name - input - the name of the image to set. Can be source or marker name
          pixels  - input - contains RGB 8 bit byte.
          width - input - contains grabbed image width
          height - input - contains image height
        */

        public bool setImage(String name, byte[] pixels, int width, int height)
        {
            if (!connected) return false;
            if (name == null) name = "";

            // setup the message request
            if (send("<request><set_image><source>" + escape(name) + "</source><width>" + width + "</width><height>" + height + "</height></set_image></request>"))
            {
                // send the RGB triplet pixels after message
                try
                {
                    binaryWriter.Write(pixels, 0, width * height * 3);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    return false;
                }

                // read message response
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }
            return false;
        }

        /*
        Returns the value of the specified variable.
          name - input - the name of the variable to query
          result - output - contains the current value of the variable
          max - input - the maximum size of what the result can hold
        */

        public String getVariable(String name)
        {
            if (!connected) return null;
            if ((name == null) || (name.Length == 0)) return null;

            if (send("<request><get_variable>" + escape(name) + "</get_variable></request>"))
            {
                // read in variable length
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (xml.parse(buffer))
                    {
                        return xml.getFirst();
                    }
                }
            }

            return null;
        }

        /*
        Returns the value of the specified variables.
          name - input - the names of the variable to query
          result - output - contains the current values of the variables
          max - input - the maximum size of what the result can hold
        */

        public List<string> getVariables(String names)
        {
            if (!connected) return null;
            if ((names == null) || (names.Length == 0)) return null;

            if (send("<request><get_variables>" + escape(names) + "</get_variables></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    return xml.parseVector(buffer);
                }
            }

            return null;
        }

        /*
        Sets the value of the specified variable.
          name - input - the name of the variable to set
          value - input - contains the current value of the variable to be set
        */

        public bool setVariable(String name, String value)
        {
            if (!connected) return false;
            if ((name == null) || (name.Length == 0)) return false;

            if (send("<request><set_variable><name>" + escape(name) + "</name><value>" + escape(value) + "</value></set_variable></request>"))
            {
                // read in confirmation
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        Sets the value of the specified variables.
          names - input - the name of the variable to set
          values - input - contains the current value of the variable to be set
        */

        public bool setVariables(String[] names, String[] values, int num)
        {
            if (!connected) return false;
            if ((names == null) || (values == null) || (names[0].Length == 0)) return false;

            int i;

            StringBuilder sb = new StringBuilder();

            // create request message
            sb.Append("<request><set_variables>");
            for (i = 0; (i < num); i++)
            {
                sb.Append("<variable><name>");
                sb.Append(escape(names[i]));
                sb.Append("</name><value>");
                sb.Append(escape(values[i]));
                sb.Append("</value></variable>");
            }
            sb.Append("</set_variables></request>");

            // send that message to RR Server
            if (send(sb.ToString()))
            {
                // read in confirmation
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        Deletes the specified variable
          name - input - the name of the variable to delete
        */

        public bool deleteVariable(String name)
        {
            if (!connected) return false;
            if ((name == null) || (name.Length == 0)) return false;

            if (send("<request><delete_variable>" + escape(name) + "</delete_variable></request>"))
            {
                // read in variable length
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        Executes the provided image processing pipeline
          source - the XML .robo file string
        */

        public bool execute(String source)
        {
            if (!connected) return false;
            if ((source == null) || (source.Length == 0)) return false;

            //send the string
            if (send("<request><execute>" + escape(source) + "</execute></request>"))
            {
                // read in result
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }
            return false;
        }

        /*
        Executes the provided .robo file. Note that the file needs to be on the machine
        running RoboRealm. This is similar to pressing the 'open program' button in the
        main RoboRealm dialog.
          filename - the XML .robo file to run
        */
        public bool loadProgram(String filename)
        {
            if (!connected) return false;
            if ((filename == null) || (filename.Length == 0)) return false;

            if (send("<request><load_program>" + escape(filename) + "</load_program></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        Loads an image into RoboRealm. Note that the image needs to exist
        on the machine running RoboRealm. The image format must be one that
        RoboRealm using the freeimage.dll component supports. This includes
        gif, pgm, ppm, jpg, png, bmp, and tiff. This is
        similar to pressing the 'load image' button in the main RoboRealm
        dialog.
          name - name of the image. Can be "source" or a marker name,
          filename - the filename of the image to load
        */
        public bool loadImage(String name, String filename)
        {
            if (!connected) return false;

            if ((filename == null) || (filename.Length == 0)) return false;
            if ((name == null) || (name.Length == 0)) name = "source";

            if (send("<request><load_image><filename>" + escape(filename) + "</filename><name>" + escape(name) + "</name></load_image></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        Saves the specified image in RoboRealm to disk. Note that the filename is relative
        to the machine that is running RoboRealm. The image format must be one that
        RoboRealm using the freeimage.dll component supports. This includes
        gif, pgm, ppm, jpg, png, bmp, and tiff. This is
        similar to pressing the 'save image' button in the main RoboRealm
        dialog.
          name - name of the image. Can be "source","processed", or a marker name,
          filename - the filename of the image to save
        */
        public bool saveImage(String source, String filename)
        {
            if (!connected) return false;

            if ((filename == null) || (filename.Length == 0)) return false;
            if ((source == null) || (source.Length == 0)) source = "processed";

            // create the save image message
            if (send("<request><save_image><filename>" + escape(filename) + "</filename><source>" + escape(source) + "</source></save_image></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        Sets the current camera driver. This can be used to change the current viewing camera
        to another camera installed on the same machine. Note that this is a small delay
        when switching between cameras. The specified name needs only to partially match
        the camera driver name seen in the dropdown picklist in the RoboRealm options dialog.
        For example, specifying "Logitech" will select any installed Logitech camera including
        "Logitech QuickCam PTZ".
        */
        public bool setCamera(String name)
        {
            if (!connected) return false;
            if ((name == null) || (name.Length == 0)) return false;

            // create the save image message
            if (send("<request><set_camera>" + escape(name) + "</set_camera></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        This routine provides a way to stop processing incoming video. Some image processing
        tasks can be very CPU intensive and you may only want to enable processing when
        required but otherwise not process any incoming images to release the CPU for other
        tasks. The run mode can also be used to processing individual frames or only run
        the image processing pipeline for a short period. This is similar to pressing the
        "run" button in the main RoboRealm dialog.
          mode - can be toggle, on, off, once, or a number of frames to process
          */
        public bool run(String mode)
        {
            if (!connected) return false;
            if ((mode == null) || (mode.Length == 0)) return false;

            // create the save image message
            if (send("<request><run>" + escape(mode) + "</run></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /*
        There is often a need to pause your own Robot Controller program to wait for
        RoboRealm to complete its task. The eaisest way to accomplish this is to wait
        on a specific variable that is set to a specific value by RoboRealm. Using the
        waitVariable routine you can pause processing and then continue when a variable
        changes within RoboRealm.
          name - name of the variable to wait for
          value - the value of that variable which will cancel the wait
          timeout - the maximum time to wait for the variable value to be set
        */

        public bool waitVariable(String name, String value, int timeout)
        {
            if (timeout == 0) timeout = 100000000;

            if (!connected) return false;
            if ((name == null) || (name.Length == 0)) return false;

            if (send("<request><wait_variable><name>" + escape(name) + "</name><value>" + escape(value) + "</value><timeout>" + timeout + "</timeout></wait_variable></request>"))
            {
                try
                {
                    handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.ToString());
                    return false;
                }
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    try
                    {
                        handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, DEFAULT_TIMEOUT);
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.ToString());
                        return false;
                    }
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
                try
                {
                    handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, DEFAULT_TIMEOUT);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.ToString());
                    return false;
                }
            }

            return false;
        }

        /*
        If you are rapdily grabbing images you will need to wait inbetween each
        get_image for a new image to be grabbed from the video camera. The wait_image
        request ensures that a new image is available to grab. Without this routine
        you may be grabbing the same image more than once.
        */

        public bool waitImage(int timeout)
        {
            if (!connected) return false;

            if (send("<request><wait_image><timeout>"+timeout+"</timeout></wait_image></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /* Pauses RoboRealm processing to ensure a stable state while quering variables */
        public bool pause()
        {
            if (send("<request><pause></pause></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }

        /* Resumes RoboRealm processing after a pause */
        public bool resume()
        {
            if (send("<request><resume></resume></request>"))
            {
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (buffer.Equals("<response>ok</response>"))
                        return true;
                }
            }

            return false;
        }


        /* If you are running RoboRealm on the same machine as your API program you can use
        this routine to start RoboRealm if it is not already running.
          filename - the path to RoboRealm on your machine
        */

        //////////////////////////////////// Basic Image Load/Save routines ////////////////////////
        // Utility routine to save a basic PPM
        public bool savePPM(String filename, byte[] buffer, int width, int height)
        {
            try
            {
                BinaryWriter bw = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate));
                String header = "P6\n" + width + " " + height + "\n255\n";
                bw.Write(header.ToCharArray());
                bw.Write(buffer, 0, width * height * 3);
                bw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            };

            return true;
        }

        private String readLine(BinaryReader tr)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                try
                {
                    int c = tr.Read();
                    if (c == '\n')
                    {
                        if (!sb.ToString().StartsWith("#"))
                            return sb.ToString();

                        sb.Length = 0;
                    }
                    sb.Append((char)c);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }
            }
        }

        // Utility routine to load a basic PPM. Note that this routine does NOT handle
        // comments and is only included as a quick example.
        public Dimension loadPPM(String filename, byte[] buffer, int max)
        {
            int width = 0, height = 0;

            try
            {
                BinaryReader br = new BinaryReader(new FileStream(filename, FileMode.Open)); 

                // read in P6 header skipping comments
                String header = readLine(br);
                if (!header.Equals("P6")) return null;

                // read in width height header skipping comments
                String size = readLine(br);
                int ind = size.IndexOf(' ');
                if (ind < 0) return null;
                width = Convert.ToInt32(size.Substring(0, ind));
                height = Convert.ToInt32(size.Substring(ind + 1));

                if ((width * height * 3) > max) return null;
                br.Read(buffer, 0, width * height * 3);
                br.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            };

            return new Dimension(width, height);
        }

        public bool doubleWordAlignImage(byte[] buffer, int width, int height)
        {
        int offset = (width*3)%4;
            if (offset!=0)
            {
                int old3Width = width * 3;
                int new3Width = old3Width + 4 - offset;

                int y;
                for (y = height-1; y >=0; y--)
                {
                    int yindex = y * new3Width;
                    Buffer.BlockCopy(buffer, y * old3Width, buffer, yindex, old3Width);
                    for (int i=yindex+old3Width;i<yindex+new3Width;i++)
                        buffer[i]=0;
                }
                return true;
            }
            return false;
      }
      
        /*
        Returns the value of the specified parameter.
          module - input - the name of the module which contains the parameter
          module_number - input - module count in case you have more than one of the same module
          name - input - the name of the parameter to query
        */
        public String getParameter(String module, int count, String name)
        {
            if (!connected) return null;
            if ((name == null) || (name.Length == 0)) return null;

            if (send("<request><get_parameter><module>" + escape(module) + "</module><module_number>"+count+"</module_number><name>"+name+"</name></get_parameter></request>"))
            {
                // read in variable length
                String buffer;
                if ((buffer = readMessage()) != null)
                {
                    if (xml.parse(buffer))
                    {
                        return xml.getFirst();
                    }
                }
            }

            return null;
        }

        private static string Read(string subKey, string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = Registry.CurrentUser;

            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);

            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)sk1.GetValue(KeyName);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool StartInstance(string executablePath, string arguments, int port)
        {
            try
            {
                uint serverReady;
                if (port == 6060)
                {
                    serverReady = CreateEvent(
                    null,         //use default security attributes
                    false,                  //event will be auto reset
                    false,                  //initial state is non-signalled
                    "RoboRealm_Server_Event");
                }
                else
                {
                    serverReady = CreateEvent(
                    null,         //use default security attributes
                    false,                  //event will be auto reset
                    false,                  //initial state is non-signalled
                    "RoboRealm_Server_Event_" + port);
                }

                Process roboRealmProcess = Process.Start(executablePath, arguments);

                uint res = WaitForSingleObject(serverReady, DEFAULT_ROBOREALM_TIMEOUT);
                if (res == 0)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ExecutablePath:{0}{1}", executablePath, Environment.NewLine);
                sb.AppendFormat("Arguments:{0}{1}", arguments, Environment.NewLine);
                sb.AppendFormat("ExceptionMessage:{0}", ex.Message);
                MessageBox.Show(sb.ToString(), "ERROR: Starting RoboRealm Process");
            }
            catch (Win32Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ExecutablePath:{0}{1}", executablePath, Environment.NewLine);
                sb.AppendFormat("Arguments:{0}{1}", arguments, Environment.NewLine);
                sb.AppendFormat("ExceptionMessage:{0}", ex.Message);
                MessageBox.Show(sb.ToString(), "ERROR: Starting RoboRealm Process");
            }
            catch (FileNotFoundException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ExecutablePath:{0}{1}", executablePath, Environment.NewLine);
                sb.AppendFormat("Arguments:{0}{1}", arguments, Environment.NewLine);
                sb.AppendFormat("ExceptionMessage:{0}", ex.Message);
                MessageBox.Show(sb.ToString(), "ERROR: Starting RoboRealm Process");
            }

            return false;
        }

        public bool start()
        {
            if (!connect("localhost"))
            {
                String roboPath = Read("Software\\RoboRealm", "Install_Dir");
                if (roboPath == null)
                {
                    MessageBox.Show("RoboRealm does not appear to be installed.\nPlease install RoboRealm prior to running any Behaviours.", "RoboRealm Error");
                    return false;
                }
                //SG - added ability to specify different port number on startup
                if (StartInstance(roboPath + "\\RoboRealm.exe", "-faceless -api_port " + SERVER_PORTNUM, SERVER_PORTNUM))
                {
                    if (!connect("localhost"))
                    {
                        MessageBox.Show("Cannot start RoboRealm. Please check that RoboRealm is installed!");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Unable to Connect to RoboRealm via 'localhost'", "RoboRealm Connect Error");
                    return false;
                }
            }

            return true;
        }
    }
}
