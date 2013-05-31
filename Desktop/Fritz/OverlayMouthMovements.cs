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
using SpeechLib;
using System.Threading;

namespace Fritz
{
    public partial class OverlayMouthMovements : Form
    {
        SpVoiceClass spVoice = new SpVoiceClass();
        ISpeechObjectTokens tokens;
        SpeechAudioFormatType waveType;

        SpMemoryStream spMemoryStream;

        List<Viseme> visemes = new List<Viseme>();

        int lastVisemeId = -1;

        public OverlayMouthMovements()
        {
            InitializeComponent();
        }

        void voice_Viseme(int StreamNumber, object StreamPosition, int Duration, SpeechVisemeType NextVisemeId, SpeechVisemeFeature Feature, SpeechVisemeType CurrentVisemeId)
        {
            if (lastVisemeId != (int)CurrentVisemeId)
                visemes.Add(new Viseme(lastVisemeId = (int)CurrentVisemeId, (int)((Decimal)StreamPosition)));
            //Console.WriteLine(string.Format("Viseme event received: {0} {1} {2} {3} {4} {5}", i, o, i2, t, f, t2));
        }

        void SpeechDone(int StreamNumber, object StreamPosition)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            tokens = spVoice.GetVoices("", "");

            spVoice.SetVoice((ISpObjectToken)tokens.Item(0));
            spMemoryStream = new SpMemoryStream();
            SpAudioFormat spAudioFormat = new SpAudioFormat();
            spAudioFormat.Type = waveType;
            spMemoryStream.Format = spAudioFormat;
            spVoice.AudioOutputStream = spMemoryStream;

            spVoice.EndStream += new _ISpeechVoiceEvents_EndStreamEventHandler(SpeechDone);
            spVoice.Viseme += new _ISpeechVoiceEvents_VisemeEventHandler(voice_Viseme);
            spVoice.Speak(textBoxSpeak.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void SetWaveFormat(WaveFormat wf)
        {
            waveType = Audio.ConvertWaveFormatToSpeechFormat(wf);
        }

        public List<Viseme> GetVisemes()
        {
            return visemes;
        }
    }
}
