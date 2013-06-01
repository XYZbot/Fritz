using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.IO;
using SpeechLib;

namespace Fritz
{
    class Audio
    {
        public static SpeechAudioFormatType ConvertWaveFormatToSpeechFormat(WaveFormat wf)
        {
            int num=0;

            if ((wf.Channels != 1)&&(wf.Channels !=2)) return SpeechAudioFormatType.SAFT16kHz16BitStereo;
            else
            if ((wf.BitsPerSample != 16) && (wf.Channels != 8)) return SpeechAudioFormatType.SAFT16kHz16BitStereo;
            else
            {
                switch (wf.SampleRate)
                {
                    case 8000: num = 4; break;
                    case 11000: num = 8; break;
                    case 12000: num = 12; break;
                    case 16000: num = 16; break;
                    case 22000: num = 20; break;
                    case 24000: num = 24; break;
                    case 32000: num = 28; break;
                    case 44000: num = 32; break;
                    case 48000: num = 36; break;
                    default: num = 16; break;
                }

                num += (wf.BitsPerSample / 8);
                num += (wf.Channels - 1);
            }
            return (SpeechAudioFormatType)num;
        }

    }
}
