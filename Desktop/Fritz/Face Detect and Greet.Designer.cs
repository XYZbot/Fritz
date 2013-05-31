namespace Fritz
{
    partial class Face_Detect_and_Greet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Face_Detect_and_Greet));
            this.label1 = new System.Windows.Forms.Label();
            this.morning = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxVoice = new System.Windows.Forms.ComboBox();
            this.afternoon = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.evening = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Activate = new System.Windows.Forms.Button();
            this.Disable = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter Morning Greeting Text:";
            // 
            // morning
            // 
            this.morning.Location = new System.Drawing.Point(12, 27);
            this.morning.Multiline = true;
            this.morning.Name = "morning";
            this.morning.Size = new System.Drawing.Size(268, 58);
            this.morning.TabIndex = 1;
            this.morning.Text = "Good morning Kerwin! How are you?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 267);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Use Voice";
            // 
            // comboBoxVoice
            // 
            this.comboBoxVoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVoice.FormattingEnabled = true;
            this.comboBoxVoice.Location = new System.Drawing.Point(12, 283);
            this.comboBoxVoice.Name = "comboBoxVoice";
            this.comboBoxVoice.Size = new System.Drawing.Size(268, 21);
            this.comboBoxVoice.TabIndex = 3;
            this.comboBoxVoice.SelectedIndexChanged += new System.EventHandler(this.comboBoxVoice_SelectedIndexChanged);
            // 
            // afternoon
            // 
            this.afternoon.Location = new System.Drawing.Point(12, 110);
            this.afternoon.Multiline = true;
            this.afternoon.Name = "afternoon";
            this.afternoon.Size = new System.Drawing.Size(268, 58);
            this.afternoon.TabIndex = 5;
            this.afternoon.Text = "Good afternoon Kerwin! How\'s the day going?";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(151, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Enter Afternoon Greeting Text:";
            // 
            // evening
            // 
            this.evening.Location = new System.Drawing.Point(14, 195);
            this.evening.Multiline = true;
            this.evening.Name = "evening";
            this.evening.Size = new System.Drawing.Size(268, 58);
            this.evening.TabIndex = 7;
            this.evening.Text = "Hey Kerwin, bedtime is in sight!";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Enter Evening Greeting Text:";
            // 
            // Activate
            // 
            this.Activate.Enabled = false;
            this.Activate.Location = new System.Drawing.Point(45, 322);
            this.Activate.Name = "Activate";
            this.Activate.Size = new System.Drawing.Size(75, 23);
            this.Activate.TabIndex = 8;
            this.Activate.Text = "Activate";
            this.Activate.UseVisualStyleBackColor = true;
            this.Activate.Click += new System.EventHandler(this.Activate_Click);
            // 
            // Disable
            // 
            this.Disable.Location = new System.Drawing.Point(126, 322);
            this.Disable.Name = "Disable";
            this.Disable.Size = new System.Drawing.Size(75, 23);
            this.Disable.TabIndex = 9;
            this.Disable.Text = "Disable";
            this.Disable.UseVisualStyleBackColor = true;
            this.Disable.Click += new System.EventHandler(this.Disable_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(207, 322);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 10;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.button3_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Face_Detect_and_Greet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 357);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Disable);
            this.Controls.Add(this.Activate);
            this.Controls.Add(this.evening);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.afternoon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxVoice);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.morning);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Face_Detect_and_Greet";
            this.Text = "Face Detect and Greet";
            this.Load += new System.EventHandler(this.Face_Detect_and_Greet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox morning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxVoice;
        private System.Windows.Forms.TextBox afternoon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox evening;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Activate;
        private System.Windows.Forms.Button Disable;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Timer timer1;
    }
}