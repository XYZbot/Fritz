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

/*
Viseme Phoneme(s)
0 silence
1 ae, ax, ah
2 aa
3 ao
4 ey, eh, uh
5 er
6 y, iy, ih, ix
7 w, uw
8 ow
9 aw
10 oy
11 ay
12 h
13 r
14 l
15 s, z
16 sh, ch, jh, zh
17 th, dh
18 f, v
19 d, t, n
20 k, g, ng
21 p, b, m
*/

namespace Fritz
{
    public partial class InsertTextToSpeech : Form
    {
        SpVoiceClass spVoice = new SpVoiceClass();
        ISpeechObjectTokens tokens;

        SpMemoryStream spMemoryStream;

        MemoryStream waveStream = null;
        SpeechAudioFormatType waveType;

        List<Viseme> visemes = new List<Viseme>();

        int currentVoiceIndex = 0;

        int lastVisemeId = -1;

        public InsertTextToSpeech()
        {
            InitializeComponent();
        }

        void voice_Viseme(int StreamNumber, object StreamPosition, int Duration, SpeechVisemeType NextVisemeId, SpeechVisemeFeature Feature, SpeechVisemeType CurrentVisemeId)
        {
            if (lastVisemeId!=(int)CurrentVisemeId)
                visemes.Add(new Viseme(lastVisemeId = (int)CurrentVisemeId, (int)((Decimal)StreamPosition)));

            //Console.WriteLine(string.Format("Viseme event received: {0} {1} {2} {3} {4} {5}", i, o, i2, t, f, t2));
        }

        void SpeechDone(int StreamNumber, object StreamPosition)
        {
            byte[] data = (byte[])spMemoryStream.GetData();
            waveStream = new MemoryStream(data, 0, data.Length, false);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            currentVoiceIndex = comboBoxVoice.SelectedIndex;
            if (currentVoiceIndex < 0) currentVoiceIndex = 0;

            if (tokens.Count <= 0) return;

            spVoice.SetVoice((ISpObjectToken)tokens.Item(currentVoiceIndex));
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

        private void InsertTextToSpeech_Load(object sender, EventArgs e)
        {
            tokens = spVoice.GetVoices("", "");

            for (int i = 0; i < tokens.Count; i++)
                comboBoxVoice.Items.Add(tokens.Item(i).GetAttribute("Name"));

            if (tokens.Count>0)
                comboBoxVoice.SelectedIndex = 0;
        }

        private void buttonSpeak_Click(object sender, EventArgs e)
        {
            spVoice.SetVoice((ISpObjectToken)tokens.Item(comboBoxVoice.SelectedIndex));
            spVoice.Speak(textBoxSpeak.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }

        /*
            SAFT8kHz8BitMono = 4
            SAFT8kHz8BitStereo = 5
            SAFT8kHz16BitMono = 6
            SAFT8kHz16BitStereo = 7
            SAFT11kHz8BitMono = 8
            SAFT11kHz8BitStereo = 9
            SAFT11kHz16BitMono = 10
            SAFT11kHz16BitStereo = 11
            SAFT12kHz8BitMono = 12
            SAFT12kHz8BitStereo = 13
            SAFT12kHz16BitMono = 14
            SAFT12kHz16BitStereo = 15
            SAFT16kHz8BitMono = 16
            SAFT16kHz8BitStereo = 17
            SAFT16kHz16BitMono = 18
            SAFT16kHz16BitStereo = 19
            SAFT22kHz8BitMono = 20
            SAFT22kHz8BitStereo = 21
            SAFT22kHz16BitMono = 22
            SAFT22kHz16BitStereo = 23
            SAFT24kHz8BitMono = 24
            SAFT24kHz8BitStereo = 25
            SAFT24kHz16BitMono = 26
            SAFT24kHz16BitStereo = 27
            SAFT32kHz8BitMono = 28
            SAFT32kHz8BitStereo = 29
            SAFT32kHz16BitMono = 30
            SAFT32kHz16BitStereo = 31
            SAFT44kHz8BitMono = 32
            SAFT44kHz8BitStereo = 33
            SAFT44kHz16BitMono = 34
            SAFT44kHz16BitStereo = 35
            SAFT48kHz8BitMono = 36
            SAFT48kHz8BitStereo = 37
            SAFT48kHz16BitMono = 38
            SAFT48kHz16BitStereo = 39
         */
        public void SetWaveFormat(WaveFormat wf)
        {
            waveType = Audio.ConvertWaveFormatToSpeechFormat(wf);
        }

        public MemoryStream GetWaveStream()
        {
            return waveStream;
        }

        public List<Viseme> GetVisemes()
        {
            return visemes;
        }
    }

    public class Viseme
    {
        public Viseme(int v, int p)
        {
            viseme = v;
            position = p;
        }

        public int viseme { get; set; }
        public int position { get; set; }
    }
}
