namespace Infinium
{
    partial class CoverWaitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoverWaitForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.WhiteSpinner = new System.Windows.Forms.PictureBox();
            this.StandardSpinner = new System.Windows.Forms.PictureBox();
            this.ThreadLifeTimer = new System.Windows.Forms.Timer(this.components);
            this.kryptonBorderEdge139 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WhiteSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StandardSpinner)).BeginInit();
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
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(105, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 40);
            this.label1.TabIndex = 32;
            this.label1.Text = "Загрузка данных с сервера\r\nПодождите...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.WhiteSpinner);
            this.panel1.Controls.Add(this.StandardSpinner);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(403, 197);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 102);
            this.panel1.TabIndex = 34;
            // 
            // WhiteSpinner
            // 
            this.WhiteSpinner.BackColor = System.Drawing.Color.Transparent;
            this.WhiteSpinner.Image = ((System.Drawing.Image)(resources.GetObject("WhiteSpinner.Image")));
            this.WhiteSpinner.Location = new System.Drawing.Point(3, 3);
            this.WhiteSpinner.Name = "WhiteSpinner";
            this.WhiteSpinner.Size = new System.Drawing.Size(96, 96);
            this.WhiteSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.WhiteSpinner.TabIndex = 33;
            this.WhiteSpinner.TabStop = false;
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
            // ThreadLifeTimer
            // 
            this.ThreadLifeTimer.Enabled = true;
            this.ThreadLifeTimer.Interval = 1;
            this.ThreadLifeTimer.Tick += new System.EventHandler(this.ThreadLifeTimer_Tick);
            // 
            // kryptonBorderEdge139
            // 
            this.kryptonBorderEdge139.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge139.Location = new System.Drawing.Point(1169, 0);
            this.kryptonBorderEdge139.Name = "kryptonBorderEdge139";
            this.kryptonBorderEdge139.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge139.Size = new System.Drawing.Size(1, 497);
            this.kryptonBorderEdge139.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge139.StateCommon.Color2 = System.Drawing.Color.Black;
            this.kryptonBorderEdge139.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge139.Text = "kryptonBorderEdge139";
            this.kryptonBorderEdge139.Visible = false;
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(1, 497);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge1.StateCommon.Color2 = System.Drawing.Color.Black;
            this.kryptonBorderEdge1.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Visible = false;
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(1, 0);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(1168, 1);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge2.StateCommon.Color2 = System.Drawing.Color.Black;
            this.kryptonBorderEdge2.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Visible = false;
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(1, 496);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(1168, 1);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.Black;
            this.kryptonBorderEdge3.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(553, 216);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 37;
            this.pictureBox1.TabStop = false;
            // 
            // CoverWaitForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1170, 497);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.kryptonBorderEdge3);
            this.Controls.Add(this.kryptonBorderEdge2);
            this.Controls.Add(this.kryptonBorderEdge1);
            this.Controls.Add(this.kryptonBorderEdge139);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoverWaitForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form2";
            this.Shown += new System.EventHandler(this.Form2_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WhiteSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StandardSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox StandardSpinner;
        private System.Windows.Forms.Timer ThreadLifeTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge139;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private System.Windows.Forms.PictureBox WhiteSpinner;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}