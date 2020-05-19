namespace Infinium
{
    partial class SplashForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.ThreadLifeTimer = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.StandardSpinner = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StandardSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // ThreadLifeTimer
            // 
            this.ThreadLifeTimer.Enabled = true;
            this.ThreadLifeTimer.Interval = 1;
            this.ThreadLifeTimer.Tick += new System.EventHandler(this.ThreadLifeTimer_Tick);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(244)))));
            this.label2.Location = new System.Drawing.Point(569, 681);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "http://preloaders.net";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.StandardSpinner);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(453, 299);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 102);
            this.panel1.TabIndex = 35;
            // 
            // StandardSpinner
            // 
            this.StandardSpinner.BackColor = System.Drawing.Color.Transparent;
            this.StandardSpinner.Image = ((System.Drawing.Image)(resources.GetObject("StandardSpinner.Image")));
            this.StandardSpinner.Location = new System.Drawing.Point(3, 3);
            this.StandardSpinner.Name = "StandardSpinner";
            this.StandardSpinner.Size = new System.Drawing.Size(96, 96);
            this.StandardSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.StandardSpinner.TabIndex = 31;
            this.StandardSpinner.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 13.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(105, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 38);
            this.label1.TabIndex = 32;
            this.label1.Text = "Загрузка данных с сервера\r\nПодождите...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SplashForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1270, 700);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.Form2_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StandardSpinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private System.Windows.Forms.Timer ThreadLifeTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox StandardSpinner;
        private System.Windows.Forms.Label label1;
    }
}