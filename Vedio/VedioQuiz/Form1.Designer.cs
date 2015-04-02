namespace VedioQuiz
{
    partial class Form1
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
            this.cameraControl1 = new Camera_NET.CameraControl();
            this.SuspendLayout();
            // 
            // cameraControl1
            // 
            this.cameraControl1.DirectShowLogFilepath = "";
            this.cameraControl1.Location = new System.Drawing.Point(319, 243);
            this.cameraControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cameraControl1.Name = "cameraControl1";
            this.cameraControl1.Size = new System.Drawing.Size(665, 498);
            this.cameraControl1.TabIndex = 0;
            this.cameraControl1.Load += new System.EventHandler(this.cameraControl1_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1481, 1051);
            this.Controls.Add(this.cameraControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private Camera_NET.CameraControl cameraControl1;

    }
}

