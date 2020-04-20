namespace Infinium
{
    partial class ReportCalculationsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportCalculationsForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.NavigateMenuCloseButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.NavigateMenuButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.NavigatePanel = new System.Windows.Forms.Panel();
            this.MenuButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.ButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.PasswordButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.panel6 = new System.Windows.Forms.Panel();
            this.AssemblyOrdersPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvReportView = new Infinium.PercentageDataGrid();
            this.MainMenuPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.MainMenuPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.ToolsPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.ErrorsToolsPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCreateReport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnSaveReportCalculations = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonCheckSet1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckSet(this.components);
            this.MenuPanel = new System.Windows.Forms.Panel();
            this.btnFilter = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.StandardButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.CalendarTo = new ComponentFactory.Krypton.Toolkit.KryptonMonthCalendar();
            this.label10 = new System.Windows.Forms.Label();
            this.CalendarFrom = new ComponentFactory.Krypton.Toolkit.KryptonMonthCalendar();
            this.label9 = new System.Windows.Forms.Label();
            this.kryptonBorderEdge38 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge39 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge40 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge41 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.NavigatePanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.AssemblyOrdersPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolsPanel)).BeginInit();
            this.ToolsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorsToolsPanel)).BeginInit();
            this.ErrorsToolsPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).BeginInit();
            this.MenuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // NavigateMenuCloseButton
            // 
            this.NavigateMenuCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NavigateMenuCloseButton.Location = new System.Drawing.Point(1220, 8);
            this.NavigateMenuCloseButton.Margin = new System.Windows.Forms.Padding(4);
            this.NavigateMenuCloseButton.Name = "NavigateMenuCloseButton";
            this.NavigateMenuCloseButton.Palette = this.NavigateMenuButtonsPalette;
            this.NavigateMenuCloseButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.NavigateMenuCloseButton.Size = new System.Drawing.Size(41, 39);
            this.NavigateMenuCloseButton.TabIndex = 12;
            this.NavigateMenuCloseButton.TabStop = false;
            this.NavigateMenuCloseButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("NavigateMenuCloseButton.Values.Image")));
            this.NavigateMenuCloseButton.Values.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.NavigateMenuCloseButton.Values.Text = "";
            this.NavigateMenuCloseButton.Click += new System.EventHandler(this.NavigateMenuCloseButton_Click);
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
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.Color1 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.Color2 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.Rounding = 0;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Back.Color1 = System.Drawing.Color.Transparent;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.Color1 = System.Drawing.Color.Silver;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.NavigateMenuButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.Rounding = 0;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 32.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(760, 45);
            this.label1.TabIndex = 31;
            this.label1.Text = "Infinium. Отчет по расчетам с ЗОВ-ЛенЕвроМебель";
            // 
            // NavigatePanel
            // 
            this.NavigatePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.NavigatePanel.Controls.Add(this.MenuButton);
            this.NavigatePanel.Controls.Add(this.kryptonBorderEdge3);
            this.NavigatePanel.Controls.Add(this.NavigateMenuCloseButton);
            this.NavigatePanel.Controls.Add(this.label1);
            this.NavigatePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NavigatePanel.Location = new System.Drawing.Point(0, 0);
            this.NavigatePanel.Name = "NavigatePanel";
            this.NavigatePanel.Size = new System.Drawing.Size(1270, 54);
            this.NavigatePanel.TabIndex = 34;
            // 
            // MenuButton
            // 
            this.MenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MenuButton.Location = new System.Drawing.Point(1167, 8);
            this.MenuButton.Name = "MenuButton";
            this.MenuButton.Palette = this.NavigateMenuButtonsPalette;
            this.MenuButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.MenuButton.Size = new System.Drawing.Size(41, 39);
            this.MenuButton.TabIndex = 337;
            this.MenuButton.TabStop = false;
            this.MenuButton.Values.ImageStates.ImageCheckedNormal = global::Infinium.Properties.Resources.MenuUp;
            this.MenuButton.Values.ImageStates.ImageCheckedPressed = global::Infinium.Properties.Resources.MenuUp;
            this.MenuButton.Values.ImageStates.ImageCheckedTracking = global::Infinium.Properties.Resources.MenuUp;
            this.MenuButton.Values.ImageStates.ImageDisabled = global::Infinium.Properties.Resources.MenuDown;
            this.MenuButton.Values.ImageStates.ImageNormal = global::Infinium.Properties.Resources.MenuDown;
            this.MenuButton.Values.ImageStates.ImagePressed = global::Infinium.Properties.Resources.MenuDown;
            this.MenuButton.Values.ImageStates.ImageTracking = global::Infinium.Properties.Resources.MenuDown;
            this.MenuButton.Values.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.MenuButton.Values.Text = "";
            this.MenuButton.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(0, 53);
            this.kryptonBorderEdge3.Margin = new System.Windows.Forms.Padding(0);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(1270, 1);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.Silver;
            this.kryptonBorderEdge3.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // ButtonsPalette
            // 
            this.ButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(150)))), ((int)(((byte)(201)))));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.Black;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color2 = System.Drawing.Color.Black;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Rounding = 0;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(150)))), ((int)(((byte)(201)))));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.Black;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLine = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLineH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Linear;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color1 = System.Drawing.Color.Silver;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color2 = System.Drawing.Color.Silver;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Rounding = 0;
            // 
            // PasswordButton
            // 
            this.PasswordButton.Location = new System.Drawing.Point(0, 7);
            this.PasswordButton.Margin = new System.Windows.Forms.Padding(0);
            this.PasswordButton.Name = "PasswordButton";
            this.PasswordButton.Size = new System.Drawing.Size(241, 65);
            this.PasswordButton.TabIndex = 25;
            this.PasswordButton.Tag = "0";
            this.PasswordButton.Values.Text = "Сменить пароль";
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.AssemblyOrdersPanel);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 99);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1270, 641);
            this.panel6.TabIndex = 69;
            // 
            // AssemblyOrdersPanel
            // 
            this.AssemblyOrdersPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AssemblyOrdersPanel.Controls.Add(this.panel2);
            this.AssemblyOrdersPanel.Location = new System.Drawing.Point(0, 0);
            this.AssemblyOrdersPanel.Name = "AssemblyOrdersPanel";
            this.AssemblyOrdersPanel.Size = new System.Drawing.Size(1268, 639);
            this.AssemblyOrdersPanel.TabIndex = 70;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvReportView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1268, 639);
            this.panel2.TabIndex = 398;
            // 
            // dgvReportView
            // 
            this.dgvReportView.AllowUserToAddRows = false;
            this.dgvReportView.AllowUserToDeleteRows = false;
            this.dgvReportView.AllowUserToResizeColumns = false;
            this.dgvReportView.AllowUserToResizeRows = false;
            this.dgvReportView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReportView.BackText = "Нет данных";
            this.dgvReportView.ColumnHeadersHeight = 40;
            this.dgvReportView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvReportView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReportView.HideOuterBorders = true;
            this.dgvReportView.Location = new System.Drawing.Point(0, 0);
            this.dgvReportView.Name = "dgvReportView";
            this.dgvReportView.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.dgvReportView.PercentLineWidth = 0;
            this.dgvReportView.RowHeadersVisible = false;
            this.dgvReportView.RowTemplate.Height = 30;
            this.dgvReportView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReportView.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Blue;
            this.dgvReportView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReportView.Size = new System.Drawing.Size(1268, 639);
            this.dgvReportView.StandardStyle = false;
            this.dgvReportView.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.dgvReportView.StateCommon.Background.Color2 = System.Drawing.Color.Transparent;
            this.dgvReportView.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvReportView.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.dgvReportView.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.dgvReportView.StateCommon.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvReportView.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.dgvReportView.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvReportView.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvReportView.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.dgvReportView.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dgvReportView.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.dgvReportView.StateCommon.HeaderColumn.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvReportView.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvReportView.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.dgvReportView.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvReportView.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvReportView.StateCommon.HeaderColumn.Border.Width = 1;
            this.dgvReportView.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.dgvReportView.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.dgvReportView.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.dgvReportView.StateSelected.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvReportView.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvReportView.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.dgvReportView.StateSelected.HeaderRow.Border.Color1 = System.Drawing.Color.White;
            this.dgvReportView.StateSelected.HeaderRow.Border.Color2 = System.Drawing.Color.White;
            this.dgvReportView.StateSelected.HeaderRow.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvReportView.TabIndex = 56;
            this.dgvReportView.UseCustomBackColor = true;
            // 
            // MainMenuPalette
            // 
            this.MainMenuPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.Transparent;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(206)))), ((int)(((byte)(255)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Rounding = 0;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedPressed.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(206)))), ((int)(((byte)(255)))));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Content.Padding = new System.Windows.Forms.Padding(-1, -1, -1, -4);
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.MainMenuPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Symbol", 21F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            // 
            // MainMenuPalette1
            // 
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.Transparent;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Border.Rounding = 0;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedPressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedPressed.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedPressed.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedPressed.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCheckedTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.Padding = new System.Windows.Forms.Padding(-1, -1, -1, -4);
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLineH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            // 
            // ToolsPanel
            // 
            this.ToolsPanel.Controls.Add(this.ErrorsToolsPanel);
            this.ToolsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolsPanel.Location = new System.Drawing.Point(0, 54);
            this.ToolsPanel.Name = "ToolsPanel";
            this.ToolsPanel.Size = new System.Drawing.Size(1270, 45);
            this.ToolsPanel.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(178)))), ((int)(((byte)(0)))));
            this.ToolsPanel.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ToolsPanel.TabIndex = 303;
            // 
            // ErrorsToolsPanel
            // 
            this.ErrorsToolsPanel.Controls.Add(this.flowLayoutPanel1);
            this.ErrorsToolsPanel.Location = new System.Drawing.Point(0, 0);
            this.ErrorsToolsPanel.Name = "ErrorsToolsPanel";
            this.ErrorsToolsPanel.Size = new System.Drawing.Size(594, 43);
            this.ErrorsToolsPanel.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(178)))), ((int)(((byte)(0)))));
            this.ErrorsToolsPanel.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ErrorsToolsPanel.TabIndex = 367;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.btnCreateReport);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveReportCalculations);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(592, 43);
            this.flowLayoutPanel1.TabIndex = 368;
            // 
            // btnCreateReport
            // 
            this.btnCreateReport.Location = new System.Drawing.Point(3, 3);
            this.btnCreateReport.Name = "btnCreateReport";
            this.btnCreateReport.Palette = this.MainMenuPalette1;
            this.btnCreateReport.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnCreateReport.Size = new System.Drawing.Size(93, 40);
            this.btnCreateReport.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlLight;
            this.btnCreateReport.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnCreateReport.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnCreateReport.TabIndex = 366;
            this.btnCreateReport.Values.Text = "Отчет";
            this.btnCreateReport.Click += new System.EventHandler(this.btnCreateReport_Click);
            // 
            // btnSaveReportCalculations
            // 
            this.btnSaveReportCalculations.Location = new System.Drawing.Point(102, 3);
            this.btnSaveReportCalculations.Name = "btnSaveReportCalculations";
            this.btnSaveReportCalculations.Palette = this.MainMenuPalette1;
            this.btnSaveReportCalculations.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnSaveReportCalculations.Size = new System.Drawing.Size(118, 40);
            this.btnSaveReportCalculations.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlLight;
            this.btnSaveReportCalculations.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnSaveReportCalculations.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSaveReportCalculations.TabIndex = 364;
            this.btnSaveReportCalculations.Values.Text = "Сохранить";
            this.btnSaveReportCalculations.Click += new System.EventHandler(this.btnSaveReportCalculations_Click);
            // 
            // MenuPanel
            // 
            this.MenuPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MenuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.MenuPanel.Controls.Add(this.btnFilter);
            this.MenuPanel.Controls.Add(this.CalendarTo);
            this.MenuPanel.Controls.Add(this.label10);
            this.MenuPanel.Controls.Add(this.CalendarFrom);
            this.MenuPanel.Controls.Add(this.label9);
            this.MenuPanel.Controls.Add(this.kryptonBorderEdge38);
            this.MenuPanel.Controls.Add(this.kryptonBorderEdge39);
            this.MenuPanel.Controls.Add(this.kryptonBorderEdge40);
            this.MenuPanel.Controls.Add(this.kryptonBorderEdge41);
            this.MenuPanel.Location = new System.Drawing.Point(701, 52);
            this.MenuPanel.Name = "MenuPanel";
            this.MenuPanel.Size = new System.Drawing.Size(571, 282);
            this.MenuPanel.TabIndex = 359;
            this.MenuPanel.Visible = false;
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(411, 229);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Palette = this.StandardButtonsPalette;
            this.btnFilter.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnFilter.Size = new System.Drawing.Size(141, 40);
            this.btnFilter.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnFilter.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnFilter.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.btnFilter.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnFilter.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnFilter.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.btnFilter.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.btnFilter.TabIndex = 404;
            this.btnFilter.TabStop = false;
            this.btnFilter.Values.Text = "Применить";
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
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
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCheckedNormal.Back.Color1 = System.Drawing.Color.Transparent;
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
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15.94F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            // CalendarTo
            // 
            this.CalendarTo.Location = new System.Drawing.Point(293, 35);
            this.CalendarTo.MaxSelectionCount = 1;
            this.CalendarTo.Name = "CalendarTo";
            this.CalendarTo.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.CalendarTo.ShowWeekNumbers = true;
            this.CalendarTo.Size = new System.Drawing.Size(259, 188);
            this.CalendarTo.StateCommon.Day.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CalendarTo.StateCommon.DayOfWeek.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CalendarTo.StateCommon.Header.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CalendarTo.TabIndex = 403;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 17.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(288, 4);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 25);
            this.label10.TabIndex = 401;
            this.label10.Text = "по...";
            // 
            // CalendarFrom
            // 
            this.CalendarFrom.Location = new System.Drawing.Point(24, 35);
            this.CalendarFrom.MaxSelectionCount = 1;
            this.CalendarFrom.Name = "CalendarFrom";
            this.CalendarFrom.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.CalendarFrom.ShowWeekNumbers = true;
            this.CalendarFrom.Size = new System.Drawing.Size(259, 188);
            this.CalendarFrom.StateCommon.Day.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CalendarFrom.StateCommon.DayOfWeek.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CalendarFrom.StateCommon.Header.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CalendarFrom.TabIndex = 402;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 17.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(22, 4);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 25);
            this.label9.TabIndex = 400;
            this.label9.Text = "Период с...";
            // 
            // kryptonBorderEdge38
            // 
            this.kryptonBorderEdge38.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonBorderEdge38.AutoSize = false;
            this.kryptonBorderEdge38.Location = new System.Drawing.Point(569, 1);
            this.kryptonBorderEdge38.Name = "kryptonBorderEdge38";
            this.kryptonBorderEdge38.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge38.Size = new System.Drawing.Size(1, 280);
            this.kryptonBorderEdge38.StateCommon.Color1 = System.Drawing.Color.White;
            this.kryptonBorderEdge38.Text = "kryptonBorderEdge38";
            // 
            // kryptonBorderEdge39
            // 
            this.kryptonBorderEdge39.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonBorderEdge39.AutoSize = false;
            this.kryptonBorderEdge39.Location = new System.Drawing.Point(1, 280);
            this.kryptonBorderEdge39.Name = "kryptonBorderEdge39";
            this.kryptonBorderEdge39.Size = new System.Drawing.Size(569, 1);
            this.kryptonBorderEdge39.StateCommon.Color1 = System.Drawing.Color.White;
            this.kryptonBorderEdge39.Text = "kryptonBorderEdge39";
            // 
            // kryptonBorderEdge40
            // 
            this.kryptonBorderEdge40.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonBorderEdge40.AutoSize = false;
            this.kryptonBorderEdge40.Location = new System.Drawing.Point(1, 1);
            this.kryptonBorderEdge40.Name = "kryptonBorderEdge40";
            this.kryptonBorderEdge40.Size = new System.Drawing.Size(569, 1);
            this.kryptonBorderEdge40.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(123)))), ((int)(((byte)(97)))));
            this.kryptonBorderEdge40.Text = "kryptonBorderEdge40";
            // 
            // kryptonBorderEdge41
            // 
            this.kryptonBorderEdge41.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonBorderEdge41.AutoSize = false;
            this.kryptonBorderEdge41.Location = new System.Drawing.Point(1, 1);
            this.kryptonBorderEdge41.Name = "kryptonBorderEdge41";
            this.kryptonBorderEdge41.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge41.Size = new System.Drawing.Size(1, 280);
            this.kryptonBorderEdge41.StateCommon.Color1 = System.Drawing.Color.White;
            this.kryptonBorderEdge41.Text = "kryptonBorderEdge41";
            // 
            // ReportCalculationsForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1270, 740);
            this.Controls.Add(this.MenuPanel);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.ToolsPanel);
            this.Controls.Add(this.NavigatePanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportCalculationsForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.ClientErrorsWriteOffsForm_Shown);
            this.NavigatePanel.ResumeLayout(false);
            this.NavigatePanel.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.AssemblyOrdersPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolsPanel)).EndInit();
            this.ToolsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorsToolsPanel)).EndInit();
            this.ErrorsToolsPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).EndInit();
            this.MenuPanel.ResumeLayout(false);
            this.MenuPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonButton NavigateMenuCloseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel NavigatePanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette ButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton PasswordButton;
        private System.Windows.Forms.Panel panel6;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette MainMenuPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette MainMenuPalette1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel ToolsPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel ErrorsToolsPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCreateReport;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSaveReportCalculations;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckSet kryptonCheckSet1;
        private System.Windows.Forms.Panel AssemblyOrdersPanel;
        private System.Windows.Forms.Panel MenuPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge38;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge39;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge40;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge41;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette StandardButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton MenuButton;
        private ComponentFactory.Krypton.Toolkit.KryptonMonthCalendar CalendarTo;
        private System.Windows.Forms.Label label10;
        private ComponentFactory.Krypton.Toolkit.KryptonMonthCalendar CalendarFrom;
        private System.Windows.Forms.Label label9;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnFilter;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette NavigateMenuButtonsPalette;
        private System.Windows.Forms.Panel panel2;
        private PercentageDataGrid dgvReportView;
    }
}