namespace Infinium
{
    partial class InfiniumTipForm
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
            this.CheckStateTimer = new System.Windows.Forms.Timer(this.components);
            this.TimeStayTimer = new System.Windows.Forms.Timer(this.components);
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.TipsLabel1 = new Infinium.InfiniumTipsLabel();
            this.SuspendLayout();
            // 
            // CheckStateTimer
            // 
            this.CheckStateTimer.Enabled = true;
            this.CheckStateTimer.Interval = 150;
            this.CheckStateTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TimeStayTimer
            // 
            this.TimeStayTimer.Interval = 150;
            this.TimeStayTimer.Tick += new System.EventHandler(this.TimeStayTimer_Tick);
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 5;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // TipsLabel1
            // 
            this.TipsLabel1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TipsLabel1.ForeColor = System.Drawing.Color.White;
            this.TipsLabel1.Location = new System.Drawing.Point(0, 0);
            this.TipsLabel1.MaxWidth = 0;
            this.TipsLabel1.Name = "TipsLabel1";
            this.TipsLabel1.Size = new System.Drawing.Size(91, 35);
            this.TipsLabel1.TabIndex = 0;
            this.TipsLabel1.SizeChanged += new System.EventHandler(this.TipsLabel1_SizeChanged);
            this.TipsLabel1.Click += new System.EventHandler(this.TipsLabel1_Click);
            this.TipsLabel1.Paint += new System.Windows.Forms.PaintEventHandler(this.TipsLabel1_Paint);
            this.TipsLabel1.MouseLeave += new System.EventHandler(this.TipsLabel1_MouseLeave);
            this.TipsLabel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TipsLabel1_MouseMove);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(130)))), ((int)(((byte)(130)))));
            this.ClientSize = new System.Drawing.Size(90, 35);
            this.Controls.Add(this.TipsLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form2";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Form2";
            this.Shown += new System.EventHandler(this.Form2_Shown);
            this.Click += new System.EventHandler(this.Form2_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer CheckStateTimer;
        private System.Windows.Forms.Timer TimeStayTimer;
        private InfiniumTipsLabel TipsLabel1;
        private System.Windows.Forms.Timer AnimateTimer;
    }
}