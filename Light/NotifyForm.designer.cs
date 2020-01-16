namespace Infinium
{
    partial class NotifyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge4 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.WatchTimer = new System.Windows.Forms.Timer(this.components);
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.ModuleNameLabel = new System.Windows.Forms.Label();
            this.NewUpdatesLabel = new System.Windows.Forms.Label();
            this.MoreUpdatesLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(324, 1);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.DimGray;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(0, 149);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(324, 1);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.DimGray;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(0, 1);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(1, 148);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.DimGray;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // kryptonBorderEdge4
            // 
            this.kryptonBorderEdge4.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge4.Location = new System.Drawing.Point(323, 1);
            this.kryptonBorderEdge4.Name = "kryptonBorderEdge4";
            this.kryptonBorderEdge4.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge4.Size = new System.Drawing.Size(1, 148);
            this.kryptonBorderEdge4.StateCommon.Color1 = System.Drawing.Color.DimGray;
            this.kryptonBorderEdge4.Text = "kryptonBorderEdge4";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(328, 28);
            this.panel1.TabIndex = 20;
            this.panel1.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(2, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 23);
            this.label2.TabIndex = 267;
            this.label2.Text = "Infinium";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // WatchTimer
            // 
            this.WatchTimer.Enabled = true;
            this.WatchTimer.Interval = 10;
            this.WatchTimer.Tick += new System.EventHandler(this.WatchTimer_Tick);
            // 
            // PictureBox
            // 
            this.PictureBox.Location = new System.Drawing.Point(17, 45);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(82, 82);
            this.PictureBox.TabIndex = 278;
            this.PictureBox.TabStop = false;
            this.PictureBox.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // ModuleNameLabel
            // 
            this.ModuleNameLabel.AutoEllipsis = true;
            this.ModuleNameLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ModuleNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.ModuleNameLabel.Location = new System.Drawing.Point(105, 43);
            this.ModuleNameLabel.Name = "ModuleNameLabel";
            this.ModuleNameLabel.Size = new System.Drawing.Size(210, 23);
            this.ModuleNameLabel.TabIndex = 279;
            this.ModuleNameLabel.Text = "Сообщения клиентов";
            this.ModuleNameLabel.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // NewUpdatesLabel
            // 
            this.NewUpdatesLabel.AutoSize = true;
            this.NewUpdatesLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.NewUpdatesLabel.ForeColor = System.Drawing.Color.Gray;
            this.NewUpdatesLabel.Location = new System.Drawing.Point(106, 69);
            this.NewUpdatesLabel.Name = "NewUpdatesLabel";
            this.NewUpdatesLabel.Size = new System.Drawing.Size(79, 23);
            this.NewUpdatesLabel.TabIndex = 280;
            this.NewUpdatesLabel.Text = "Новых: 1";
            this.NewUpdatesLabel.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // MoreUpdatesLabel
            // 
            this.MoreUpdatesLabel.AutoSize = true;
            this.MoreUpdatesLabel.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MoreUpdatesLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.MoreUpdatesLabel.Location = new System.Drawing.Point(106, 109);
            this.MoreUpdatesLabel.Name = "MoreUpdatesLabel";
            this.MoreUpdatesLabel.Size = new System.Drawing.Size(164, 19);
            this.MoreUpdatesLabel.TabIndex = 281;
            this.MoreUpdatesLabel.Text = "и еще (235) обновлений";
            this.MoreUpdatesLabel.Visible = false;
            this.MoreUpdatesLabel.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(297, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 22);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 269;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.MenuCloseButton_Click);
            // 
            // NotifyForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(324, 150);
            this.Controls.Add(this.MoreUpdatesLabel);
            this.Controls.Add(this.NewUpdatesLabel);
            this.Controls.Add(this.ModuleNameLabel);
            this.Controls.Add(this.PictureBox);
            this.Controls.Add(this.kryptonBorderEdge4);
            this.Controls.Add(this.kryptonBorderEdge3);
            this.Controls.Add(this.kryptonBorderEdge2);
            this.Controls.Add(this.kryptonBorderEdge1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotifyForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Infinium";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.NewsCommentsForm_Shown);
            this.Click += new System.EventHandler(this.PictureBox_Click);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer WatchTimer;
        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.Label ModuleNameLabel;
        private System.Windows.Forms.Label NewUpdatesLabel;
        private System.Windows.Forms.Label MoreUpdatesLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}