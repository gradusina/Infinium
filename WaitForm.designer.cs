namespace Infinium
{
    partial class WaitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ThreadLifeTimer = new System.Windows.Forms.Timer(this.components);
            this.kryptonBorderEdge139 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 13.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(57, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 38);
            this.label1.TabIndex = 32;
            this.label1.Text = "Загрузка данных с сервера\r\nПодождите...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(263, 55);
            this.panel1.TabIndex = 34;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 31;
            this.pictureBox1.TabStop = false;
            // 
            // ThreadLifeTimer
            // 
            this.ThreadLifeTimer.Enabled = true;
            this.ThreadLifeTimer.Interval = 1;
            this.ThreadLifeTimer.Tick += new System.EventHandler(this.ThreadLifeTimer_Tick);
            // 
            // kryptonBorderEdge139
            // 
            this.kryptonBorderEdge139.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge139.Location = new System.Drawing.Point(285, 0);
            this.kryptonBorderEdge139.Name = "kryptonBorderEdge139";
            this.kryptonBorderEdge139.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge139.Size = new System.Drawing.Size(1, 86);
            this.kryptonBorderEdge139.StateCommon.Color1 = System.Drawing.Color.White;
            this.kryptonBorderEdge139.StateCommon.Color2 = System.Drawing.Color.White;
            this.kryptonBorderEdge139.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge139.Text = "kryptonBorderEdge139";
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(1, 86);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.White;
            this.kryptonBorderEdge1.StateCommon.Color2 = System.Drawing.Color.White;
            this.kryptonBorderEdge1.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(1, 0);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(284, 1);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.White;
            this.kryptonBorderEdge2.StateCommon.Color2 = System.Drawing.Color.White;
            this.kryptonBorderEdge2.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(1, 85);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(284, 1);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.White;
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.White;
            this.kryptonBorderEdge3.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // WaitForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.ClientSize = new System.Drawing.Size(286, 86);
            this.Controls.Add(this.kryptonBorderEdge3);
            this.Controls.Add(this.kryptonBorderEdge2);
            this.Controls.Add(this.kryptonBorderEdge1);
            this.Controls.Add(this.kryptonBorderEdge139);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.Shown += new System.EventHandler(this.Form2_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer ThreadLifeTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge139;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
    }
}