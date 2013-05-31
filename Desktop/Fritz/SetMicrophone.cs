using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Management.Instrumentation;

namespace Fritz
{
    public partial class SetMicrophone : Form
    {
        NAudio.Wave.WaveIn sourceStream = null;
        int bytesPerSample = 1;
        int bytesPerChannel = 1;
        int level = 0;
        int deviceNumber = 0;

        public SetMicrophone()
        {
            InitializeComponent();

            int waveInDevices = NAudio.Wave.WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                NAudio.Wave.WaveInCapabilities deviceInfo = NAudio.Wave.WaveIn.GetCapabilities(waveInDevice);
                microphoneList.Items.Add(waveInDevice+": "+deviceInfo.ProductName);
            }

            microphoneList.SelectedIndex = 0;
            sensitivity.SelectedIndex = 0;

            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = 0;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(0).Channels);
            bytesPerChannel = (sourceStream.WaveFormat.BitsPerSample / 8);
            bytesPerSample = bytesPerChannel * sourceStream.WaveFormat.Channels;

            sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);

            sourceStream.StartRecording();
        }

        public int getDeviceNumber()
        {
            return deviceNumber;
        }

        public int getSensitivity()
        {
            return level;
        }

        private void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            byte[] waveData = e.Buffer;

            int globalMax = 0;
            int globalMin = 0;
            long p;
            for (p = 0; p < waveData.Length; p += bytesPerChannel)
            {
                int val = BitConverter.ToInt16(waveData, (int)p);
                if (val < globalMin) globalMin = val;
                if (val > globalMax) globalMax = val;
            }

            globalMin = -globalMin;
            if (globalMin > globalMax) globalMax = globalMin;

            int l = (int)((globalMax * 100) / (32768 >> level));
            if (l > 100) l = 100;
            if (l < 0) l = 0;
            audioLevel.Value = l;
        }
        
        private void microphoneList_SelectedIndexChanged(object sender, EventArgs e)
        {
            deviceNumber = microphoneList.SelectedIndex;
            if (sourceStream!=null) sourceStream.StopRecording();
            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels);
            bytesPerSample = (sourceStream.WaveFormat.BitsPerSample / 8) * sourceStream.WaveFormat.Channels;
            sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
            sourceStream.StartRecording();
        }

        private void sensitivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            level = sensitivity.SelectedIndex;
        }
    }
}
