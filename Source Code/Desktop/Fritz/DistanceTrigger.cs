using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fritz
{
    public partial class DistanceTrigger : Form
    {
        public DistanceTrigger()
        {
            InitializeComponent();
        }

        public void Set(bool e, bool s, bool i, int d)
        {
            enableTrigger.Checked = e;
            checkIR.Checked = i;
            checkSonar.Checked = s;
            distance.Text = Convert.ToString(d);
            distance.Enabled = enableTrigger.Checked;
            checkSonar.Enabled = enableTrigger.Checked;
            checkIR.Enabled = enableTrigger.Checked;
        }

        public int GetDistance()
        {
            return Convert.ToInt32(distance.Value);
        }

        public bool CheckSonar()
        {
            return checkSonar.Checked;
        }

        public bool CheckIR()
        {
            return checkIR.Checked;
        }

        public bool IsEnabled()
        {
            return enableTrigger.Checked;
        }

        private void enableTrigger_CheckedChanged(object sender, EventArgs e)
        {
            distance.Enabled = enableTrigger.Checked;
            checkSonar.Enabled = enableTrigger.Checked;
            checkIR.Enabled = enableTrigger.Checked;
        }
    }
}
