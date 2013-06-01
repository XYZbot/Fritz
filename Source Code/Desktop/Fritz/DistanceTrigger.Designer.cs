namespace Fritz
{
    partial class DistanceTrigger
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
            this.distance = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkSonar = new System.Windows.Forms.CheckBox();
            this.checkIR = new System.Windows.Forms.CheckBox();
            this.enableTrigger = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.distance)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(209, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "cm";
            // 
            // distance
            // 
            this.distance.Enabled = false;
            this.distance.Location = new System.Drawing.Point(131, 85);
            this.distance.Name = "distance";
            this.distance.Size = new System.Drawing.Size(75, 20);
            this.distance.TabIndex = 59;
            this.distance.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(76, 185);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 58;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(157, 185);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 57;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(220, 13);
            this.label2.TabIndex = 61;
            this.label2.Text = "Specify distance that below which will trigger ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 13);
            this.label3.TabIndex = 62;
            this.label3.Text = "the current sequence to play.";
            // 
            // checkSonar
            // 
            this.checkSonar.AutoSize = true;
            this.checkSonar.Checked = true;
            this.checkSonar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSonar.Enabled = false;
            this.checkSonar.Location = new System.Drawing.Point(18, 121);
            this.checkSonar.Name = "checkSonar";
            this.checkSonar.Size = new System.Drawing.Size(133, 17);
            this.checkSonar.TabIndex = 63;
            this.checkSonar.Text = "Check Sonar Distance";
            this.checkSonar.UseVisualStyleBackColor = true;
            // 
            // checkIR
            // 
            this.checkIR.AutoSize = true;
            this.checkIR.Enabled = false;
            this.checkIR.Location = new System.Drawing.Point(18, 144);
            this.checkIR.Name = "checkIR";
            this.checkIR.Size = new System.Drawing.Size(116, 17);
            this.checkIR.TabIndex = 64;
            this.checkIR.Text = "Check IR Distance";
            this.checkIR.UseVisualStyleBackColor = true;
            // 
            // enableTrigger
            // 
            this.enableTrigger.AutoSize = true;
            this.enableTrigger.Location = new System.Drawing.Point(18, 13);
            this.enableTrigger.Name = "enableTrigger";
            this.enableTrigger.Size = new System.Drawing.Size(111, 17);
            this.enableTrigger.TabIndex = 65;
            this.enableTrigger.Text = "Trigger is Enabled";
            this.enableTrigger.UseVisualStyleBackColor = true;
            this.enableTrigger.CheckedChanged += new System.EventHandler(this.enableTrigger_CheckedChanged);
            // 
            // DistanceTrigger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 220);
            this.Controls.Add(this.enableTrigger);
            this.Controls.Add(this.checkIR);
            this.Controls.Add(this.checkSonar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.distance);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Name = "DistanceTrigger";
            this.Text = "DistanceTrigger";
            ((System.ComponentModel.ISupportInitialize)(this.distance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown distance;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkSonar;
        private System.Windows.Forms.CheckBox checkIR;
        private System.Windows.Forms.CheckBox enableTrigger;
    }
}