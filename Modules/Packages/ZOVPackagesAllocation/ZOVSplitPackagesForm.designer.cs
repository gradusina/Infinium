namespace Infinium
{
    partial class ZOVSplitPackagesForm
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
            this.NotEqualPanel = new System.Windows.Forms.Panel();
            this.SecondPackageCountLabel = new System.Windows.Forms.Label();
            this.SecondPackageCountTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonBorderEdge18 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonCheckSet1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckSet(this.components);
            this.NotEqualPackagesButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.PanelSelectPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.EqualPackagesButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.EqualPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FirstPositionTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.PackCountTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CancelPackButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.StandardButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.OKButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.panel3 = new System.Windows.Forms.Panel();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.NavigateMenuButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.NotEqualPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).BeginInit();
            this.MainPanel.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.EqualPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // NotEqualPanel
            // 
            this.NotEqualPanel.Controls.Add(this.SecondPackageCountLabel);
            this.NotEqualPanel.Controls.Add(this.SecondPackageCountTextBox);
            this.NotEqualPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NotEqualPanel.Location = new System.Drawing.Point(0, 0);
            this.NotEqualPanel.Name = "NotEqualPanel";
            this.NotEqualPanel.Size = new System.Drawing.Size(358, 141);
            this.NotEqualPanel.TabIndex = 70;
            // 
            // SecondPackageCountLabel
            // 
            this.SecondPackageCountLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SecondPackageCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.SecondPackageCountLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SecondPackageCountLabel.ForeColor = System.Drawing.Color.Black;
            this.SecondPackageCountLabel.Location = new System.Drawing.Point(23, 29);
            this.SecondPackageCountLabel.Name = "SecondPackageCountLabel";
            this.SecondPackageCountLabel.Size = new System.Drawing.Size(312, 20);
            this.SecondPackageCountLabel.TabIndex = 81;
            this.SecondPackageCountLabel.Text = "Введите кол-во элементов во второй упаковке";
            this.SecondPackageCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SecondPackageCountTextBox
            // 
            this.SecondPackageCountTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SecondPackageCountTextBox.Location = new System.Drawing.Point(108, 68);
            this.SecondPackageCountTextBox.Name = "SecondPackageCountTextBox";
            this.SecondPackageCountTextBox.Size = new System.Drawing.Size(143, 32);
            this.SecondPackageCountTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SecondPackageCountTextBox.TabIndex = 73;
            this.SecondPackageCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // kryptonBorderEdge18
            // 
            this.kryptonBorderEdge18.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.kryptonBorderEdge18.Location = new System.Drawing.Point(-44, 72);
            this.kryptonBorderEdge18.Name = "kryptonBorderEdge18";
            this.kryptonBorderEdge18.Size = new System.Drawing.Size(452, 1);
            this.kryptonBorderEdge18.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge18.Text = "kryptonBorderEdge18";
            // 
            // kryptonCheckSet1
            // 
            this.kryptonCheckSet1.CheckButtons.Add(this.NotEqualPackagesButton);
            this.kryptonCheckSet1.CheckButtons.Add(this.EqualPackagesButton);
            this.kryptonCheckSet1.CheckedButton = this.EqualPackagesButton;
            // 
            // NotEqualPackagesButton
            // 
            this.NotEqualPackagesButton.Location = new System.Drawing.Point(182, 2);
            this.NotEqualPackagesButton.Name = "NotEqualPackagesButton";
            this.NotEqualPackagesButton.Palette = this.PanelSelectPalette;
            this.NotEqualPackagesButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.NotEqualPackagesButton.Size = new System.Drawing.Size(176, 31);
            this.NotEqualPackagesButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.NotEqualPackagesButton.TabIndex = 1;
            this.NotEqualPackagesButton.Values.Text = "Неравные упаковки";
            this.NotEqualPackagesButton.Click += new System.EventHandler(this.NotEqualPackagesButton_Click);
            // 
            // PanelSelectPalette
            // 
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color2 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.Color2 = System.Drawing.Color.Transparent;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.Color1 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.Color2 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Border.Color1 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Border.Color2 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.Color1 = System.Drawing.Color.DarkGray;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.Color2 = System.Drawing.Color.DarkGray;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color2 = System.Drawing.Color.Transparent;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color2 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLine = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLineH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.DarkGray;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color2 = System.Drawing.Color.Gray;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color1 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color2 = System.Drawing.Color.Black;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.PanelSelectPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Rounding = 0;
            // 
            // EqualPackagesButton
            // 
            this.EqualPackagesButton.Checked = true;
            this.EqualPackagesButton.Location = new System.Drawing.Point(0, 2);
            this.EqualPackagesButton.Name = "EqualPackagesButton";
            this.EqualPackagesButton.Palette = this.PanelSelectPalette;
            this.EqualPackagesButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.EqualPackagesButton.Size = new System.Drawing.Size(176, 31);
            this.EqualPackagesButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EqualPackagesButton.TabIndex = 0;
            this.EqualPackagesButton.Values.Text = "Равные упаковки";
            this.EqualPackagesButton.Click += new System.EventHandler(this.EqualPackagesButton_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.White;
            this.MainPanel.Controls.Add(this.tableLayoutPanel4);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(364, 254);
            this.MainPanel.TabIndex = 87;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.17899F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.82101F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(364, 254);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // panel1
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.EqualPanel);
            this.panel1.Controls.Add(this.NotEqualPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 141);
            this.panel1.TabIndex = 82;
            // 
            // EqualPanel
            // 
            this.EqualPanel.Controls.Add(this.kryptonBorderEdge18);
            this.EqualPanel.Controls.Add(this.label1);
            this.EqualPanel.Controls.Add(this.label2);
            this.EqualPanel.Controls.Add(this.FirstPositionTextBox);
            this.EqualPanel.Controls.Add(this.PackCountTextBox);
            this.EqualPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EqualPanel.Location = new System.Drawing.Point(0, 0);
            this.EqualPanel.Name = "EqualPanel";
            this.EqualPanel.Size = new System.Drawing.Size(358, 141);
            this.EqualPanel.TabIndex = 87;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(74, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 17);
            this.label1.TabIndex = 81;
            this.label1.Text = "Введите кол-во в одной упаковке";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(88, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(182, 17);
            this.label2.TabIndex = 83;
            this.label2.Text = "Введите начальную позицию";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FirstPositionTextBox
            // 
            this.FirstPositionTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FirstPositionTextBox.Location = new System.Drawing.Point(108, 102);
            this.FirstPositionTextBox.Name = "FirstPositionTextBox";
            this.FirstPositionTextBox.Size = new System.Drawing.Size(143, 32);
            this.FirstPositionTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FirstPositionTextBox.TabIndex = 82;
            this.FirstPositionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PackCountTextBox
            // 
            this.PackCountTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PackCountTextBox.Location = new System.Drawing.Point(108, 28);
            this.PackCountTextBox.Name = "PackCountTextBox";
            this.PackCountTextBox.Size = new System.Drawing.Size(143, 32);
            this.PackCountTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PackCountTextBox.TabIndex = 73;
            this.PackCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel2
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.panel2, 2);
            this.panel2.Controls.Add(this.CancelPackButton);
            this.panel2.Controls.Add(this.OKButton);
            this.panel2.Controls.Add(this.kryptonBorderEdge1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 191);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(358, 60);
            this.panel2.TabIndex = 83;
            // 
            // CancelPackButton
            // 
            this.CancelPackButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelPackButton.Location = new System.Drawing.Point(182, 6);
            this.CancelPackButton.Name = "CancelPackButton";
            this.CancelPackButton.Palette = this.StandardButtonsPalette;
            this.CancelPackButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.CancelPackButton.Size = new System.Drawing.Size(167, 49);
            this.CancelPackButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.CancelPackButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelPackButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.CancelPackButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelPackButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelPackButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.CancelPackButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CancelPackButton.TabIndex = 109;
            this.CancelPackButton.TabStop = false;
            this.CancelPackButton.Values.Text = "Отмена";
            this.CancelPackButton.Click += new System.EventHandler(this.CancelPackButton_Click);
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
            // OKButton
            // 
            this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKButton.Location = new System.Drawing.Point(9, 6);
            this.OKButton.Name = "OKButton";
            this.OKButton.Palette = this.StandardButtonsPalette;
            this.OKButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.OKButton.Size = new System.Drawing.Size(167, 49);
            this.OKButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.OKButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OKButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.OKButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OKButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OKButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.OKButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OKButton.TabIndex = 108;
            this.OKButton.TabStop = false;
            this.OKButton.Values.Text = "ОК";
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(358, 1);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.Silver;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // panel3
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.panel3, 2);
            this.panel3.Controls.Add(this.NotEqualPackagesButton);
            this.panel3.Controls.Add(this.kryptonBorderEdge2);
            this.panel3.Controls.Add(this.EqualPackagesButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(358, 35);
            this.panel3.TabIndex = 84;
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(0, 34);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(358, 1);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.Silver;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // NavigateMenuButtonsPalette
            // 
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color2 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.DimGray;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color2 = System.Drawing.Color.DimGray;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Rounding = 0;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color2 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color2 = System.Drawing.Color.DimGray;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Back.Color1 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Back.Color2 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.Rounding = 0;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StatePressed.Border.Width = 1;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color2 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color1 = System.Drawing.Color.Silver;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color2 = System.Drawing.Color.Silver;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Rounding = 0;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Width = 1;
            // 
            // ZOVSplitPackagesForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(364, 254);
            this.Controls.Add(this.MainPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ZOVSplitPackagesForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Разделение позиции";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MarketingSplitPackagesForm_FormClosing);
            this.Shown += new System.EventHandler(this.SplitPackagesForm_Shown);
            this.NotEqualPanel.ResumeLayout(false);
            this.NotEqualPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).EndInit();
            this.MainPanel.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.EqualPanel.ResumeLayout(false);
            this.EqualPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private System.Windows.Forms.Panel NotEqualPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge18;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox SecondPackageCountTextBox;
        private System.Windows.Forms.Label SecondPackageCountLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckSet kryptonCheckSet1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton NotEqualPackagesButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton EqualPackagesButton;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel EqualPanel;
        private System.Windows.Forms.Label label1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox PackCountTextBox;
        private System.Windows.Forms.Label label2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox FirstPositionTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private System.Windows.Forms.Panel panel3;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette NavigateMenuButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette PanelSelectPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonButton CancelPackButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton OKButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette StandardButtonsPalette;
    }
}