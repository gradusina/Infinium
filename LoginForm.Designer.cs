namespace Infinium
{
    partial class LoginForm
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
            this.LoginButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.CurrentTimeTimer = new System.Windows.Forms.Timer(this.components);
            this.glassPanel1 = new Infinium.GlassPanel();
            this.LoginComboBox = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.PasswordComboBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LogInButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.label2 = new System.Windows.Forms.Label();
            this.LogOutButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.CurrentDayOfWeekLabel = new System.Windows.Forms.Label();
            this.CurrentDayMonthLabel = new System.Windows.Forms.Label();
            this.CurrentTimeLabel = new System.Windows.Forms.Label();
            this.NoAccessPanel = new Infinium.GlassPanel();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.glassPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoginComboBox)).BeginInit();
            this.NoAccessPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoginButtonsPalette
            // 
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.Transparent;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color2 = System.Drawing.Color.Transparent;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color2 = System.Drawing.Color.Transparent;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.Padding = new System.Windows.Forms.Padding(-1, 4, -1, -1);
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Back.Color1 = System.Drawing.Color.Transparent;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Back.Color2 = System.Drawing.Color.DarkGray;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.Rounding = 0;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.Transparent;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color2 = System.Drawing.Color.DarkGray;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(130)))), ((int)(((byte)(130)))));
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.LoginButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Rounding = 0;
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // CurrentTimeTimer
            // 
            this.CurrentTimeTimer.Enabled = true;
            this.CurrentTimeTimer.Interval = 500;
            this.CurrentTimeTimer.Tick += new System.EventHandler(this.CurrentTimeTimer_Tick);
            // 
            // glassPanel1
            // 
            this.glassPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.glassPanel1.BackColor = System.Drawing.Color.Transparent;
            this.glassPanel1.Controls.Add(this.LoginComboBox);
            this.glassPanel1.Controls.Add(this.PasswordComboBox);
            this.glassPanel1.Controls.Add(this.label1);
            this.glassPanel1.Controls.Add(this.LogInButton);
            this.glassPanel1.Controls.Add(this.label2);
            this.glassPanel1.Controls.Add(this.LogOutButton);
            this.glassPanel1.Location = new System.Drawing.Point(664, 401);
            this.glassPanel1.Name = "glassPanel1";
            this.glassPanel1.Size = new System.Drawing.Size(501, 233);
            this.glassPanel1.TabIndex = 9;
            // 
            // LoginComboBox
            // 
            this.LoginComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LoginComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.LoginComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.LoginComboBox.DropDownWidth = 241;
            this.LoginComboBox.Location = new System.Drawing.Point(46, 46);
            this.LoginComboBox.Name = "LoginComboBox";
            this.LoginComboBox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Black;
            this.LoginComboBox.Size = new System.Drawing.Size(409, 29);
            this.LoginComboBox.StateCommon.ComboBox.Content.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LoginComboBox.StateCommon.Item.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LoginComboBox.StateTracking.Item.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.LoginComboBox.StateTracking.Item.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.LoginComboBox.StateTracking.Item.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.LoginComboBox.TabIndex = 1;
            // 
            // PasswordComboBox
            // 
            this.PasswordComboBox.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.PasswordComboBox.Location = new System.Drawing.Point(46, 126);
            this.PasswordComboBox.Name = "PasswordComboBox";
            this.PasswordComboBox.Size = new System.Drawing.Size(409, 30);
            this.PasswordComboBox.TabIndex = 2;
            this.PasswordComboBox.UseSystemPasswordChar = true;
            this.PasswordComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 17.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(41, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "ФИО";
            // 
            // LogInButton
            // 
            this.LogInButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LogInButton.Location = new System.Drawing.Point(82, 177);
            this.LogInButton.Name = "LogInButton";
            this.LogInButton.Palette = this.LoginButtonsPalette;
            this.LogInButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.LogInButton.Size = new System.Drawing.Size(150, 33);
            this.LogInButton.TabIndex = 3;
            this.LogInButton.Values.Text = "ВХОД";
            this.LogInButton.Click += new System.EventHandler(this.LogInButton_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 17.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(41, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Пароль";
            // 
            // LogOutButton
            // 
            this.LogOutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LogOutButton.Location = new System.Drawing.Point(266, 177);
            this.LogOutButton.Name = "LogOutButton";
            this.LogOutButton.Palette = this.LoginButtonsPalette;
            this.LogOutButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.LogOutButton.Size = new System.Drawing.Size(150, 33);
            this.LogOutButton.TabIndex = 4;
            this.LogOutButton.Values.Text = "ОТМЕНА";
            this.LogOutButton.Click += new System.EventHandler(this.LogOutButton_Click);
            // 
            // CurrentDayOfWeekLabel
            // 
            this.CurrentDayOfWeekLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CurrentDayOfWeekLabel.AutoSize = true;
            this.CurrentDayOfWeekLabel.BackColor = System.Drawing.Color.Transparent;
            this.CurrentDayOfWeekLabel.Font = new System.Drawing.Font("Segoe UI Semilight", 22.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CurrentDayOfWeekLabel.ForeColor = System.Drawing.Color.White;
            this.CurrentDayOfWeekLabel.Location = new System.Drawing.Point(272, 594);
            this.CurrentDayOfWeekLabel.Name = "CurrentDayOfWeekLabel";
            this.CurrentDayOfWeekLabel.Size = new System.Drawing.Size(149, 31);
            this.CurrentDayOfWeekLabel.TabIndex = 7;
            this.CurrentDayOfWeekLabel.Text = "понедельник";
            // 
            // CurrentDayMonthLabel
            // 
            this.CurrentDayMonthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CurrentDayMonthLabel.AutoSize = true;
            this.CurrentDayMonthLabel.BackColor = System.Drawing.Color.Transparent;
            this.CurrentDayMonthLabel.Font = new System.Drawing.Font("Segoe UI Semilight", 22.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CurrentDayMonthLabel.ForeColor = System.Drawing.Color.White;
            this.CurrentDayMonthLabel.Location = new System.Drawing.Point(272, 637);
            this.CurrentDayMonthLabel.Name = "CurrentDayMonthLabel";
            this.CurrentDayMonthLabel.Size = new System.Drawing.Size(132, 31);
            this.CurrentDayMonthLabel.TabIndex = 6;
            this.CurrentDayMonthLabel.Text = "04 февраля";
            // 
            // CurrentTimeLabel
            // 
            this.CurrentTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CurrentTimeLabel.AutoSize = true;
            this.CurrentTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.CurrentTimeLabel.Font = new System.Drawing.Font("Segoe UI Light", 90F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CurrentTimeLabel.ForeColor = System.Drawing.Color.White;
            this.CurrentTimeLabel.Location = new System.Drawing.Point(33, 563);
            this.CurrentTimeLabel.Name = "CurrentTimeLabel";
            this.CurrentTimeLabel.Size = new System.Drawing.Size(240, 120);
            this.CurrentTimeLabel.TabIndex = 5;
            this.CurrentTimeLabel.Text = "21:29";
            // 
            // NoAccessPanel
            // 
            this.NoAccessPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NoAccessPanel.BackColor = System.Drawing.Color.Transparent;
            this.NoAccessPanel.Controls.Add(this.kryptonBorderEdge1);
            this.NoAccessPanel.Controls.Add(this.label5);
            this.NoAccessPanel.Controls.Add(this.label4);
            this.NoAccessPanel.Location = new System.Drawing.Point(664, 230);
            this.NoAccessPanel.Name = "NoAccessPanel";
            this.NoAccessPanel.Size = new System.Drawing.Size(501, 163);
            this.NoAccessPanel.TabIndex = 10;
            this.NoAccessPanel.Visible = false;
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(22, 50);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(455, 1);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(25, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(446, 80);
            this.label5.TabIndex = 4;
            this.label5.Text = "Пользователь с таким именем уже работает в системе.\r\nВозможно, Вы не закрыли прог" +
    "рамму на другом компьютере,\r\nлибо под Вашим логином работает другой человек.\r\nОб" +
    "ратитесь к администратору программы";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 19.69F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(92, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(301, 28);
            this.label4.TabIndex = 3;
            this.label4.Text = "Невозможно осуществить вход";
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(1254, 711);
            this.Controls.Add(this.CurrentDayOfWeekLabel);
            this.Controls.Add(this.CurrentDayMonthLabel);
            this.Controls.Add(this.NoAccessPanel);
            this.Controls.Add(this.CurrentTimeLabel);
            this.Controls.Add(this.glassPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.Opacity = 0D;
            this.Text = "Form3";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.glassPanel1.ResumeLayout(false);
            this.glassPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoginComboBox)).EndInit();
            this.NoAccessPanel.ResumeLayout(false);
            this.NoAccessPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton LogOutButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette LoginButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonButton LogInButton;
        private System.Windows.Forms.Label CurrentTimeLabel;
        private System.Windows.Forms.Label CurrentDayMonthLabel;
        private System.Windows.Forms.Label CurrentDayOfWeekLabel;
        private GlassPanel glassPanel1;
        private System.Windows.Forms.Timer CurrentTimeTimer;
        private GlassPanel NoAccessPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PasswordComboBox;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox LoginComboBox;
    }
}