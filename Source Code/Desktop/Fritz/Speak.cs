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
    public class Speak
    {
        SpVoiceClass spVoice = new SpVoiceClass();
        ISpeechObjectTokens tokens;
        Conductor conductor;

        public Speak(Conductor c, String text, int voiceIndex)
        {
            conductor = c;

            tokens = spVoice.GetVoices("", "");

            //currentVoiceIndex = comboBoxVoice.SelectedIndex;
            if (voiceIndex < 0) voiceIndex = 0;
            if (tokens.Count > 0)
            {
                spVoice.SetVoice((ISpObjectToken)tokens.Item(voiceIndex));
                spVoice.Viseme += new _ISpeechVoiceEvents_VisemeEventHandler(voice_Viseme);
                spVoice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            }
        }

        void voice_Viseme(int StreamNumber, object StreamPosition, int Duration, SpeechVisemeType NextVisemeId, SpeechVisemeFeature Feature, SpeechVisemeType CurrentVisemeId)
        {
            conductor.SetState(conductor.CreateStateFromViseme((int)CurrentVisemeId));
        }
    }
}
