using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;
using SharpDX.DirectInput;

namespace Fritz
{
    public partial class MicrophoneConfiguration : Form
    {
        NAudio.Wave.WaveIn sourceStream = null;
        int deviceNumber = 0;
        int sensitivity = 0;

        Conductor conductor;

        int skipCheck = 0;
        int bytesPerSample; 

        bool reAcquireMicrophone = false;

        public bool calibrating = false;

        public MicrophoneConfiguration()
        {
            InitializeComponent();
        }

        public void SetConductor(ref Conductor c)
        {
            conductor = c;
        }

        public void RefreshDeviceList()
        {
/*
            // If Gamepad not found, look for a Microphone
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Microphone, DeviceEnumerationFlags.AllDevices))
            {
                comboBoxMicrophones.Items.Add(deviceInstance.InstanceName);
                if (!devices.ContainsKey(deviceInstance.InstanceName))
                    devices.Add(deviceInstance.InstanceName, deviceInstance.InstanceGuid);
            }
 */ 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (conductor==null) return;

            if (skipCheck > 0)
            {
                --skipCheck;
                return;
            }

            if (reAcquireMicrophone)
            {
                reAcquireMicrophone = false;

                sourceStream = new NAudio.Wave.WaveIn();
                sourceStream.DeviceNumber = deviceNumber;
                sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels);
                bytesPerSample = (sourceStream.WaveFormat.BitsPerSample / 8) * sourceStream.WaveFormat.Channels;

                sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);

                sourceStream.StartRecording();
            }
/*
            if (reAcquireMicrophone||(!directInput.IsDeviceAttached(MicrophoneGuid)))
            {
                if (Microphone != null)
                    Microphone.Unacquire();

                IList<DeviceInstance> deviceInstance = directInput.GetDevices(DeviceType.Microphone, DeviceEnumerationFlags.AllDevices);
                if (deviceInstance.Count > 0)
                    MicrophoneGuid = deviceInstance[0].InstanceGuid;
                else
                {
                    // don't check for another second
                    skipCheck = 100;
                    return;
                }

                reAcquireMicrophone = false;

                // Instantiate the Microphone
                Microphone = new Microphone(directInput, MicrophoneGuid);

                // Set BufferSize in order to use buffered data.
                Microphone.Properties.BufferSize = 128;

                // Acquire the Microphone
                Microphone.Acquire();

                // allow values to stabilize before using them!
                skipCount = 50;
            }
 */ 
        }

        private void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (calibrating) return;

            byte[] waveData = e.Buffer;

            int windowSize = 1000 * bytesPerSample;
            int globalMax = 0;
            int globalMin = 0;
            long n, p, q;
            for (n = p = 0; p < waveData.Length; p += bytesPerSample * 500, n++)
            {
                for (q = p; (q < waveData.Length) && (q < p + (bytesPerSample * 500)); q += bytesPerSample)
                {
                    int val = BitConverter.ToInt16(waveData, (int)q);
                    if (val < globalMin) globalMin = val;
                    if (val > globalMax) globalMax = val;
                }
            }

            globalMin = -globalMin;
            if (globalMin > globalMax) globalMax = globalMin;
            float factor = globalMax / ((float)(32768>>sensitivity));
            if (factor > 1.0f) factor = 1.0f;

            conductor.SetJaw(factor);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        public void Stop()
        {
            reAcquireMicrophone = false;
            if (sourceStream!=null)
                sourceStream.StopRecording();
        }

        public void Start(int device, int level)
        {
            deviceNumber = device;
            sensitivity = level;
            reAcquireMicrophone = true;
        }
    }
}
