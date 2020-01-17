namespace Infinium
{
    partial class ZOVFilterProductsForm
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
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.MainPanel = new System.Windows.Forms.Panel();
            this.OKButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.StandardButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.CancelPackButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CheckAllDecorButton = new System.Windows.Forms.CheckBox();
            this.DecorCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CheckAllFrontsButton = new System.Windows.Forms.CheckBox();
            this.FrontsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.MainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.White;
            this.MainPanel.Controls.Add(this.OKButton);
            this.MainPanel.Controls.Add(this.CancelPackButton);
            this.MainPanel.Controls.Add(this.groupBox2);
            this.MainPanel.Controls.Add(this.groupBox1);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(481, 417);
            this.MainPanel.TabIndex = 87;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKButton.Location = new System.Drawing.Point(96, 360);
            this.OKButton.Name = "OKButton";
            this.OKButton.Palette = this.StandardButtonsPalette;
            this.OKButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.OKButton.Size = new System.Drawing.Size(124, 49);
            this.OKButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.OKButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OKButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.OKButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.OKButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OKButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.OKButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OKButton.TabIndex = 100;
            this.OKButton.TabStop = false;
            this.OKButton.Values.Text = "ОК";
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // StandardButtonsPalette
            // 
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(167)))), ((int)(((byte)(214)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.Transparent;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color2 = System.Drawing.Color.Gray;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Rounding = 0;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color2 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.Transparent;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color2 = System.Drawing.Color.Black;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLine = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLineH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color2 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color1 = System.Drawing.Color.Transparent;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color2 = System.Drawing.Color.Black;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Rounding = 0;
            // 
            // CancelPackButton
            // 
            this.CancelPackButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelPackButton.Location = new System.Drawing.Point(261, 360);
            this.CancelPackButton.Name = "CancelPackButton";
            this.CancelPackButton.Palette = this.StandardButtonsPalette;
            this.CancelPackButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.CancelPackButton.Size = new System.Drawing.Size(124, 49);
            this.CancelPackButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.CancelPackButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelPackButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.CancelPackButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CancelPackButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelPackButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.CancelPackButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CancelPackButton.TabIndex = 99;
            this.CancelPackButton.TabStop = false;
            this.CancelPackButton.Values.Text = "ОТМЕНА";
            this.CancelPackButton.Click += new System.EventHandler(this.CancelPackButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CheckAllDecorButton);
            this.groupBox2.Controls.Add(this.DecorCheckedListBox);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.groupBox2.ForeColor = System.Drawing.Color.Black;
            this.groupBox2.Location = new System.Drawing.Point(243, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(223, 339);
            this.groupBox2.TabIndex = 62;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Декор";
            // 
            // CheckAllDecorButton
            // 
            this.CheckAllDecorButton.AutoSize = true;
            this.CheckAllDecorButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CheckAllDecorButton.Location = new System.Drawing.Point(9, 18);
            this.CheckAllDecorButton.Name = "CheckAllDecorButton";
            this.CheckAllDecorButton.Size = new System.Drawing.Size(119, 22);
            this.CheckAllDecorButton.TabIndex = 4;
            this.CheckAllDecorButton.Text = "Выбрать все";
            this.CheckAllDecorButton.UseVisualStyleBackColor = true;
            this.CheckAllDecorButton.CheckedChanged += new System.EventHandler(this.CheckAllDecorButton_CheckedChanged);
            // 
            // DecorCheckedListBox
            // 
            this.DecorCheckedListBox.BackColor = System.Drawing.Color.White;
            this.DecorCheckedListBox.CheckOnClick = true;
            this.DecorCheckedListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DecorCheckedListBox.ForeColor = System.Drawing.Color.Black;
            this.DecorCheckedListBox.FormattingEnabled = true;
            this.DecorCheckedListBox.Location = new System.Drawing.Point(6, 42);
            this.DecorCheckedListBox.Name = "DecorCheckedListBox";
            this.DecorCheckedListBox.Size = new System.Drawing.Size(211, 274);
            this.DecorCheckedListBox.TabIndex = 2;
            this.DecorCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.DecorCheckedListBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CheckAllFrontsButton);
            this.groupBox1.Controls.Add(this.FrontsCheckedListBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(223, 339);
            this.groupBox1.TabIndex = 61;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Фасады";
            // 
            // CheckAllFrontsButton
            // 
            this.CheckAllFrontsButton.AutoSize = true;
            this.CheckAllFrontsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CheckAllFrontsButton.Location = new System.Drawing.Point(9, 18);
            this.CheckAllFrontsButton.Name = "CheckAllFrontsButton";
            this.CheckAllFrontsButton.Size = new System.Drawing.Size(119, 22);
            this.CheckAllFrontsButton.TabIndex = 3;
            this.CheckAllFrontsButton.Text = "Выбрать все";
            this.CheckAllFrontsButton.UseVisualStyleBackColor = true;
            this.CheckAllFrontsButton.CheckedChanged += new System.EventHandler(this.CheckAllFrontsButton_CheckedChanged);
            // 
            // FrontsCheckedListBox
            // 
            this.FrontsCheckedListBox.BackColor = System.Drawing.Color.White;
            this.FrontsCheckedListBox.CheckOnClick = true;
            this.FrontsCheckedListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FrontsCheckedListBox.ForeColor = System.Drawing.Color.Black;
            this.FrontsCheckedListBox.FormattingEnabled = true;
            this.FrontsCheckedListBox.Location = new System.Drawing.Point(6, 42);
            this.FrontsCheckedListBox.Name = "FrontsCheckedListBox";
            this.FrontsCheckedListBox.Size = new System.Drawing.Size(211, 274);
            this.FrontsCheckedListBox.TabIndex = 2;
            this.FrontsCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.FrontsCheckedListBox_SelectedIndexChanged);
            // 
            // ZOVFilterProductsForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(481, 417);
            this.Controls.Add(this.MainPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ZOVFilterProductsForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MarketingSplitPackagesForm_FormClosing);
            this.Shown += new System.EventHandler(this.SplitPackagesForm_Shown);
            this.MainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.CheckedListBox FrontsCheckedListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox DecorCheckedListBox;
        private System.Windows.Forms.CheckBox CheckAllFrontsButton;
        private System.Windows.Forms.CheckBox CheckAllDecorButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton OKButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton CancelPackButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette StandardButtonsPalette;
    }
}