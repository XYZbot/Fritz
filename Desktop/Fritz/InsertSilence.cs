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
    public partial class InsertSilence : Form
    {
        public InsertSilence()
        {
            InitializeComponent();
        }

        public float GetSeconds()
        {
            return (float)amount.Value;
        }
    }
}
