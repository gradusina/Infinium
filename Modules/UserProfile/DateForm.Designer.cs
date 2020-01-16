namespace Infinium
{
    partial class DateForm
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
            this.label40 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.YearTextBox = new System.Windows.Forms.TextBox();
            this.YearTrackBar = new DevExpress.XtraEditors.TrackBarControl();
            this.MonthTextBox = new System.Windows.Forms.TextBox();
            this.DayTextBox = new System.Windows.Forms.TextBox();
            this.DayTrackBar = new DevExpress.XtraEditors.TrackBarControl();
            this.MonthTrackBar = new DevExpress.XtraEditors.TrackBarControl();
            this.kryptonBorderEdge20 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge21 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge22 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge23 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.OKDateButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.StandardButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.CancelDateButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonBorderEdge19 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            ((System.ComponentModel.ISupportInitialize)(this.YearTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YearTrackBar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DayTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DayTrackBar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonthTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonthTrackBar.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Segoe UI", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label40.ForeColor = System.Drawing.Color.Black;
            this.label40.Location = new System.Drawing.Point(306, 206);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(33, 20);
            this.label40.TabIndex = 165;
            this.label40.Text = "Год";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Segoe UI", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label38.ForeColor = System.Drawing.Color.Black;
            this.label38.Location = new System.Drawing.Point(295, 104);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(54, 20);
            this.label38.TabIndex = 164;
            this.label38.Text = "Месяц";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Segoe UI", 14.06F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label37.ForeColor = System.Drawing.Color.Black;
            this.label37.Location = new System.Drawing.Point(300, 8);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(44, 20);
            this.label37.TabIndex = 163;
            this.label37.Text = "День";
            // 
            // YearTextBox
            // 
            this.YearTextBox.BackColor = System.Drawing.Color.White;
            this.YearTextBox.Font = new System.Drawing.Font("Segoe UI", 17.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.YearTextBox.Location = new System.Drawing.Point(259, 227);
            this.YearTextBox.Name = "YearTextBox";
            this.YearTextBox.ReadOnly = true;
            this.YearTextBox.Size = new System.Drawing.Size(126, 31);
            this.YearTextBox.TabIndex = 161;
            this.YearTextBox.Text = "2013";
            this.YearTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // YearTrackBar
            // 
            this.YearTrackBar.EditValue = 2013;
            this.YearTrackBar.Location = new System.Drawing.Point(167, 254);
            this.YearTrackBar.Name = "YearTrackBar";
            this.YearTrackBar.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.YearTrackBar.Properties.LookAndFeel.SkinName = "Office 2010 Silver";
            this.YearTrackBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.YearTrackBar.Properties.Maximum = 2013;
            this.YearTrackBar.Properties.Minimum = 1913;
            this.YearTrackBar.Properties.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.YearTrackBar.Size = new System.Drawing.Size(311, 45);
            this.YearTrackBar.TabIndex = 162;
            this.YearTrackBar.Value = 2013;
            this.YearTrackBar.EditValueChanged += new System.EventHandler(this.YearTrackBar_EditValueChanged);
            // 
            // MonthTextBox
            // 
            this.MonthTextBox.BackColor = System.Drawing.Color.White;
            this.MonthTextBox.Font = new System.Drawing.Font("Segoe UI", 17.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MonthTextBox.Location = new System.Drawing.Point(259, 127);
            this.MonthTextBox.Name = "MonthTextBox";
            this.MonthTextBox.ReadOnly = true;
            this.MonthTextBox.Size = new System.Drawing.Size(126, 31);
            this.MonthTextBox.TabIndex = 159;
            this.MonthTextBox.Text = "Январь";
            this.MonthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DayTextBox
            // 
            this.DayTextBox.BackColor = System.Drawing.Color.White;
            this.DayTextBox.Font = new System.Drawing.Font("Segoe UI", 17.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DayTextBox.Location = new System.Drawing.Point(295, 31);
            this.DayTextBox.Name = "DayTextBox";
            this.DayTextBox.ReadOnly = true;
            this.DayTextBox.Size = new System.Drawing.Size(54, 31);
            this.DayTextBox.TabIndex = 158;
            this.DayTextBox.Text = "01";
            this.DayTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DayTrackBar
            // 
            this.DayTrackBar.EditValue = 1;
            this.DayTrackBar.Location = new System.Drawing.Point(220, 56);
            this.DayTrackBar.Name = "DayTrackBar";
            this.DayTrackBar.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.DayTrackBar.Properties.LookAndFeel.SkinName = "Office 2010 Silver";
            this.DayTrackBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.DayTrackBar.Properties.Maximum = 31;
            this.DayTrackBar.Properties.Minimum = 1;
            this.DayTrackBar.Properties.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.DayTrackBar.Size = new System.Drawing.Size(204, 45);
            this.DayTrackBar.TabIndex = 157;
            this.DayTrackBar.Value = 1;
            this.DayTrackBar.EditValueChanged += new System.EventHandler(this.DayTrackBar_EditValueChanged);
            // 
            // MonthTrackBar
            // 
            this.MonthTrackBar.EditValue = null;
            this.MonthTrackBar.Location = new System.Drawing.Point(220, 152);
            this.MonthTrackBar.Name = "MonthTrackBar";
            this.MonthTrackBar.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.MonthTrackBar.Properties.LookAndFeel.SkinName = "Office 2010 Silver";
            this.MonthTrackBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MonthTrackBar.Properties.Maximum = 11;
            this.MonthTrackBar.Properties.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.MonthTrackBar.Size = new System.Drawing.Size(204, 45);
            this.MonthTrackBar.TabIndex = 160;
            this.MonthTrackBar.EditValueChanged += new System.EventHandler(this.MonthTrackBar_EditValueChanged);
            // 
            // kryptonBorderEdge20
            // 
            this.kryptonBorderEdge20.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge20.Location = new System.Drawing.Point(644, 0);
            this.kryptonBorderEdge20.Name = "kryptonBorderEdge20";
            this.kryptonBorderEdge20.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge20.Size = new System.Drawing.Size(1, 427);
            this.kryptonBorderEdge20.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge20.Text = "kryptonBorderEdge20";
            // 
            // kryptonBorderEdge21
            // 
            this.kryptonBorderEdge21.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge21.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge21.Name = "kryptonBorderEdge21";
            this.kryptonBorderEdge21.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge21.Size = new System.Drawing.Size(1, 427);
            this.kryptonBorderEdge21.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge21.Text = "kryptonBorderEdge21";
            // 
            // kryptonBorderEdge22
            // 
            this.kryptonBorderEdge22.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge22.Location = new System.Drawing.Point(1, 426);
            this.kryptonBorderEdge22.Name = "kryptonBorderEdge22";
            this.kryptonBorderEdge22.Size = new System.Drawing.Size(643, 1);
            this.kryptonBorderEdge22.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge22.Text = "kryptonBorderEdge22";
            // 
            // kryptonBorderEdge23
            // 
            this.kryptonBorderEdge23.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge23.Location = new System.Drawing.Point(1, 0);
            this.kryptonBorderEdge23.Name = "kryptonBorderEdge23";
            this.kryptonBorderEdge23.Size = new System.Drawing.Size(643, 1);
            this.kryptonBorderEdge23.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge23.Text = "kryptonBorderEdge23";
            // 
            // OKDateButton
            // 
            this.OKDateButton.Location = new System.Drawing.Point(163, 350);
            this.OKDateButton.Name = "OKDateButton";
            this.OKDateButton.Palette = this.StandardButtonsPalette;
            this.OKDateButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.OKDateButton.Size = new System.Drawing.Size(140, 35);
            this.OKDateButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.OKDateButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OKDateButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.OKDateButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OKDateButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OKDateButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.OKDateButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OKDateButton.TabIndex = 174;
            this.OKDateButton.TabStop = false;
            this.OKDateButton.Values.Text = "OK";
            this.OKDateButton.Click += new System.EventHandler(this.OKDateButton_Click);
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
            // CancelDateButton
            // 
            this.CancelDateButton.Location = new System.Drawing.Point(347, 350);
            this.CancelDateButton.Name = "CancelDateButton";
            this.CancelDateButton.Palette = this.StandardButtonsPalette;
            this.CancelDateButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.CancelDateButton.Size = new System.Drawing.Size(140, 35);
            this.CancelDateButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.CancelDateButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelDateButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.CancelDateButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelDateButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelDateButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.CancelDateButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CancelDateButton.TabIndex = 175;
            this.CancelDateButton.TabStop = false;
            this.CancelDateButton.Values.Text = "ОТМЕНА";
            this.CancelDateButton.Click += new System.EventHandler(this.CancelDateButton_Click);
            // 
            // kryptonBorderEdge19
            // 
            this.kryptonBorderEdge19.AutoSize = false;
            this.kryptonBorderEdge19.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.kryptonBorderEdge19.Location = new System.Drawing.Point(3, 333);
            this.kryptonBorderEdge19.Name = "kryptonBorderEdge19";
            this.kryptonBorderEdge19.Size = new System.Drawing.Size(644, 1);
            this.kryptonBorderEdge19.StateCommon.Color1 = System.Drawing.Color.DarkGray;
            this.kryptonBorderEdge19.Text = "kryptonBorderEdge19";
            // 
            // DateForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(645, 427);
            this.Controls.Add(this.kryptonBorderEdge19);
            this.Controls.Add(this.CancelDateButton);
            this.Controls.Add(this.OKDateButton);
            this.Controls.Add(this.kryptonBorderEdge23);
            this.Controls.Add(this.kryptonBorderEdge22);
            this.Controls.Add(this.kryptonBorderEdge21);
            this.Controls.Add(this.kryptonBorderEdge20);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.YearTextBox);
            this.Controls.Add(this.YearTrackBar);
            this.Controls.Add(this.MonthTextBox);
            this.Controls.Add(this.DayTextBox);
            this.Controls.Add(this.DayTrackBar);
            this.Controls.Add(this.MonthTrackBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Infinium";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Infinium";
            ((System.ComponentModel.ISupportInitialize)(this.YearTrackBar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YearTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DayTrackBar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DayTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonthTrackBar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonthTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox YearTextBox;
        private DevExpress.XtraEditors.TrackBarControl YearTrackBar;
        private System.Windows.Forms.TextBox MonthTextBox;
        private System.Windows.Forms.TextBox DayTextBox;
        private DevExpress.XtraEditors.TrackBarControl DayTrackBar;
        private DevExpress.XtraEditors.TrackBarControl MonthTrackBar;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge20;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge21;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge22;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge23;
        private ComponentFactory.Krypton.Toolkit.KryptonButton OKDateButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton CancelDateButton;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge19;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette StandardButtonsPalette;
    }
}