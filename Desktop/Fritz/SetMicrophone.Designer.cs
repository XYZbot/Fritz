namespace Fritz
{
    partial class SetMicrophone
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.microphoneList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.audioLevel = new System.Windows.Forms.ProgressBar();
            this.sensitivity = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "Microphone Sensitivity";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(124, 166);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 58;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(205, 166);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 57;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // microphoneList
            // 
            this.microphoneList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.microphoneList.FormattingEnabled = true;
            this.microphoneList.Location = new System.Drawing.Point(16, 36);
            this.microphoneList.Name = "microphoneList";
            this.microphoneList.Size = new System.Drawing.Size(264, 21);
            this.microphoneList.TabIndex = 61;
            this.microphoneList.SelectedIndexChanged += new System.EventHandler(this.microphoneList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 62;
            this.label2.Text = "Use Microphone";
            // 
            // audioLevel
            // 
            this.audioLevel.Location = new System.Drawing.Point(16, 78);
            this.audioLevel.Name = "audioLevel";
            this.audioLevel.Size = new System.Drawing.Size(264, 23);
            this.audioLevel.TabIndex = 63;
            // 
            // sensitivity
            // 
            this.sensitivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sensitivity.FormattingEnabled = true;
            this.sensitivity.Items.AddRange(new object[] {
            "1 - low",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7 - high"});
            this.sensitivity.Location = new System.Drawing.Point(132, 126);
            this.sensitivity.Name = "sensitivity";
            this.sensitivity.Size = new System.Drawing.Size(72, 21);
            this.sensitivity.TabIndex = 64;
            this.sensitivity.SelectedIndexChanged += new System.EventHandler(this.sensitivity_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 65;
            this.label3.Text = "Mouth Closed";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(214, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 66;
            this.label4.Text = "Mouth Open";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 68;
            this.label6.Text = "Audio Level";
            // 
            // SetMicrophone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 203);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sensitivity);
            this.Controls.Add(this.audioLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.microphoneList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetMicrophone";
            this.Text = "Set Microphone";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox microphoneList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar audioLevel;
        private System.Windows.Forms.ComboBox sensitivity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
    }
}