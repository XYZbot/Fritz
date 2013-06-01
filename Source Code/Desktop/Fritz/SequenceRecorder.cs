using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System.Threading;

namespace Fritz
{
    public partial class SequenceRecorder : Form
    {
        NAudio.Wave.WaveIn sourceStream = null;
        int bytesPerSample = 1;
        int bytesPerChannel = 1;
        int level = 0;
        int deviceNumber = 0;
        Conductor conductor;
        KeyboardConfiguration keyboard;
        MemoryStream recordingStream;
        bool isRecording=false;
        bool recordingFirstSkip =false;

        public SequenceRecorder(Conductor cond, KeyboardConfiguration key)
        {
            conductor = cond;
            keyboard = key;

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
            sourceStream.WaveFormat = conductor.GetWaveFormat();// new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(0).Channels);
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

            if (isRecording)
            {
                if (!recordingFirstSkip)
                {
                    conductor.StartRecording(waveData.Length*4);
                    recordingFirstSkip = true;
                }

                recordingStream.Write(waveData, 0, waveData.Length);
            }
        }
        
        private void microphoneList_SelectedIndexChanged(object sender, EventArgs e)
        {
            deviceNumber = microphoneList.SelectedIndex;
            if (sourceStream!=null) sourceStream.StopRecording();
            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.WaveFormat = conductor.GetWaveFormat();// new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels);
            bytesPerSample = (sourceStream.WaveFormat.BitsPerSample / 8) * sourceStream.WaveFormat.Channels;
            sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
            sourceStream.StartRecording();
        }

        private void sensitivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            level = sensitivity.SelectedIndex;
        }

        private void recordBtn_Click(object sender, EventArgs e)
        {
            recordingStream = new MemoryStream(8192);
            isRecording = true;
            recordingFirstSkip = false;

            recordBtn.Enabled = false;
            stopBtn.Enabled = true;
            pauseBtn.Enabled = true;
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            recordBtn.Enabled = true;
            conductor.StopRecording();
            stopBtn.Enabled = false;
            pauseBtn.Enabled = false;
            isRecording = false;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            conductor.PauseRecording();
            recordBtn.Enabled = true;
            stopBtn.Enabled = false;
            pauseBtn.Enabled = false;
            isRecording = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            conductor.StopRecording();
            stopBtn.Enabled = false;
            pauseBtn.Enabled = false;
            isRecording = false;

            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            conductor.StopRecording();
            stopBtn.Enabled = false;
            pauseBtn.Enabled = false;
            isRecording = false;

            this.DialogResult = DialogResult.Cancel;
        }

        public MemoryStream getAudio()
        {
            return recordingStream;
        }

        public List<RobotState> getFrames()
        {
            return conductor.getRecordedFrames();
        }

        private void SequenceRecorder_KeyDown(object sender, KeyEventArgs e)
        {
            keyboard.ProcessKey(e, true);
        }

        private void SequenceRecorder_KeyUp(object sender, KeyEventArgs e)
        {
            keyboard.ProcessKey(e, false);
        }
    }
}
