using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using NAudio.Wave;
using System.IO;
using SpeechLib;

namespace Fritz
{
    public partial class Face_Detect_and_Greet : Form
    {
        bool isRunning=true;
        RoboRealm.RR_API rr;
        Conductor conductor;
        SpVoiceClass spVoice = new SpVoiceClass();
        ISpeechObjectTokens tokens;
        int SelectedIndex = 0;
        bool triggerFace = false;

        public Face_Detect_and_Greet(RoboRealm.RR_API robo, Conductor cond)
        {
            rr = robo;
            conductor = cond;

            InitializeComponent();

            Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread.Start();
        }

        private void Face_Detect_and_Greet_Load(object sender, EventArgs e)
        {
            tokens = spVoice.GetVoices("", "");

            for (int i = 0; i < tokens.Count; i++)
                comboBoxVoice.Items.Add(tokens.Item(i).GetAttribute("Name"));

            if (tokens.Count > 0)
                comboBoxVoice.SelectedIndex = 0;
        }

        private void Activate_Click(object sender, EventArgs e)
        {
            isRunning = true;

            Disable.Enabled = true;
            Activate.Enabled = false;
        }

        private void Disable_Click(object sender, EventArgs e)
        {
            isRunning = false;

            Disable.Enabled = false;
            Activate.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isRunning = false;

            this.DialogResult = DialogResult.Cancel;
        }

        private void comboBoxVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndex = comboBoxVoice.SelectedIndex;
        }

        private void WorkThreadFunction()
        {
            while (isRunning)
            {
                // wait for transition from 0 to 1
                rr.waitVariable("FACES_COUNT", "0", 0);
                if (isRunning)
                {
                    rr.waitVariable("FACES_COUNT", "1", 0);
                    if (isRunning)
                    {
                        triggerFace = true;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (triggerFace)
            {
                triggerFace = false;

                Speak speak;

                if (DateTime.Now.Hour < 12)
                    speak = new Speak(conductor, morning.Text, SelectedIndex);
                else
                    if (DateTime.Now.Hour < 19)
                        speak = new Speak(conductor, afternoon.Text, SelectedIndex);
                    else
                        speak = new Speak(conductor, evening.Text, SelectedIndex);
            }
        }
    }
}
