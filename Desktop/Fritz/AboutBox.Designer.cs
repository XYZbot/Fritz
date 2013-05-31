namespace Fritz
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelProductTitle = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.goWebsite = new System.Windows.Forms.Button();
            this.labelCopyright = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(302, 128);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelProductTitle
            // 
            this.labelProductTitle.Location = new System.Drawing.Point(13, 147);
            this.labelProductTitle.Name = "labelProductTitle";
            this.labelProductTitle.Size = new System.Drawing.Size(179, 17);
            this.labelProductTitle.TabIndex = 0;
            this.labelProductTitle.Text = "Fritz Control Editor, Version 1.5";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(13, 196);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(301, 41);
            this.textBoxDescription.TabIndex = 3;
            this.textBoxDescription.Text = "The Fritz Control Editor is an application for use in controlling the Fritz anima" +
                "tronic head created and produced by XYZbot LLC.";
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(239, 248);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 4;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            // 
            // goWebsite
            // 
            this.goWebsite.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.goWebsite.Location = new System.Drawing.Point(12, 248);
            this.goWebsite.Name = "goWebsite";
            this.goWebsite.Size = new System.Drawing.Size(111, 23);
            this.goWebsite.TabIndex = 5;
            this.goWebsite.Text = "Goto Website";
            this.goWebsite.UseVisualStyleBackColor = true;
            this.goWebsite.Click += new System.EventHandler(this.goWebsite_Click);
            // 
            // labelCopyright
            // 
            this.labelCopyright.Location = new System.Drawing.Point(13, 171);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(179, 17);
            this.labelCopyright.TabIndex = 6;
            this.labelCopyright.Text = "Copyright 2013, XYZBot LLC";
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 283);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.goWebsite);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.labelProductTitle);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutBox";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelProductTitle;
        private System.Windows.Forms.Label textBoxDescription;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button goWebsite;
        private System.Windows.Forms.Label labelCopyright;


    }
}
