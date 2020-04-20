namespace Infinium
{
    partial class DoubleOrdersStatisticsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DoubleOrdersStatisticsForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.NavigateMenuCloseButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.NavigateMenuButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.NavigatePanel = new System.Windows.Forms.Panel();
            this.MinimizeButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.PasswordButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.panel6 = new System.Windows.Forms.Panel();
            this.FirstOperatorPanel = new System.Windows.Forms.Panel();
            this.dgvFirstOperator = new Infinium.PercentageDataGrid();
            this.SecondOperatorPanel = new System.Windows.Forms.Panel();
            this.dgvSecondOperator = new Infinium.PercentageDataGrid();
            this.kryptonBorderEdge6 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge7 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge4 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge5 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.MainMenuPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cbtnFirstOperator = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.cbtnSecondOperator = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.kryptonCheckSet1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckSet(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbTotalErrors = new System.Windows.Forms.Label();
            this.lbTotalTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpSecondDate = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.dtpFirstDate = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.cbxShowCorrectOrders = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbFirstOperators = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.cmbSecondOperators = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.NavigatePanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.FirstOperatorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFirstOperator)).BeginInit();
            this.SecondOperatorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSecondOperator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbFirstOperators)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSecondOperators)).BeginInit();
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
            this.label1.Size = new System.Drawing.Size(591, 45);
            this.label1.TabIndex = 31;
            this.label1.Text = "Infinium. Статистика двойного вбивания";
            // 
            // NavigatePanel
            // 
            this.NavigatePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.NavigatePanel.Controls.Add(this.MinimizeButton);
            this.NavigatePanel.Controls.Add(this.kryptonBorderEdge3);
            this.NavigatePanel.Controls.Add(this.NavigateMenuCloseButton);
            this.NavigatePanel.Controls.Add(this.label1);
            this.NavigatePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NavigatePanel.Location = new System.Drawing.Point(0, 0);
            this.NavigatePanel.Name = "NavigatePanel";
            this.NavigatePanel.Size = new System.Drawing.Size(1270, 54);
            this.NavigatePanel.TabIndex = 34;
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MinimizeButton.Location = new System.Drawing.Point(1167, 7);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Palette = this.NavigateMenuButtonsPalette;
            this.MinimizeButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.MinimizeButton.Size = new System.Drawing.Size(44, 40);
            this.MinimizeButton.TabIndex = 342;
            this.MinimizeButton.TabStop = false;
            this.MinimizeButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("MinimizeButton.Values.Image")));
            this.MinimizeButton.Values.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.MinimizeButton.Values.Text = "";
            this.MinimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
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
            this.panel6.Controls.Add(this.FirstOperatorPanel);
            this.panel6.Controls.Add(this.SecondOperatorPanel);
            this.panel6.Controls.Add(this.kryptonBorderEdge6);
            this.panel6.Controls.Add(this.kryptonBorderEdge7);
            this.panel6.Controls.Add(this.kryptonBorderEdge4);
            this.panel6.Controls.Add(this.kryptonBorderEdge5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 203);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1270, 537);
            this.panel6.TabIndex = 69;
            // 
            // FirstOperatorPanel
            // 
            this.FirstOperatorPanel.Controls.Add(this.dgvFirstOperator);
            this.FirstOperatorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FirstOperatorPanel.Location = new System.Drawing.Point(1, 1);
            this.FirstOperatorPanel.Name = "FirstOperatorPanel";
            this.FirstOperatorPanel.Size = new System.Drawing.Size(1268, 535);
            this.FirstOperatorPanel.TabIndex = 70;
            // 
            // dgvFirstOperator
            // 
            this.dgvFirstOperator.AllowUserToAddRows = false;
            this.dgvFirstOperator.AllowUserToDeleteRows = false;
            this.dgvFirstOperator.AllowUserToResizeColumns = false;
            this.dgvFirstOperator.AllowUserToResizeRows = false;
            this.dgvFirstOperator.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFirstOperator.BackText = "Нет данных";
            this.dgvFirstOperator.ColumnHeadersHeight = 40;
            this.dgvFirstOperator.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFirstOperator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFirstOperator.HideOuterBorders = true;
            this.dgvFirstOperator.Location = new System.Drawing.Point(0, 0);
            this.dgvFirstOperator.Name = "dgvFirstOperator";
            this.dgvFirstOperator.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.dgvFirstOperator.PercentLineWidth = 0;
            this.dgvFirstOperator.ReadOnly = true;
            this.dgvFirstOperator.RowHeadersVisible = false;
            this.dgvFirstOperator.RowTemplate.Height = 30;
            this.dgvFirstOperator.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFirstOperator.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Blue;
            this.dgvFirstOperator.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFirstOperator.Size = new System.Drawing.Size(1268, 535);
            this.dgvFirstOperator.StandardStyle = false;
            this.dgvFirstOperator.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.dgvFirstOperator.StateCommon.Background.Color2 = System.Drawing.Color.Transparent;
            this.dgvFirstOperator.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvFirstOperator.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.dgvFirstOperator.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.dgvFirstOperator.StateCommon.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvFirstOperator.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.dgvFirstOperator.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvFirstOperator.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvFirstOperator.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.dgvFirstOperator.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dgvFirstOperator.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.dgvFirstOperator.StateCommon.HeaderColumn.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvFirstOperator.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvFirstOperator.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.dgvFirstOperator.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvFirstOperator.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvFirstOperator.StateCommon.HeaderColumn.Border.Width = 1;
            this.dgvFirstOperator.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.dgvFirstOperator.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.dgvFirstOperator.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.dgvFirstOperator.StateSelected.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvFirstOperator.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvFirstOperator.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.dgvFirstOperator.StateSelected.HeaderRow.Border.Color1 = System.Drawing.Color.White;
            this.dgvFirstOperator.StateSelected.HeaderRow.Border.Color2 = System.Drawing.Color.White;
            this.dgvFirstOperator.StateSelected.HeaderRow.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvFirstOperator.TabIndex = 54;
            this.dgvFirstOperator.UseCustomBackColor = true;
            // 
            // SecondOperatorPanel
            // 
            this.SecondOperatorPanel.Controls.Add(this.dgvSecondOperator);
            this.SecondOperatorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondOperatorPanel.Location = new System.Drawing.Point(1, 1);
            this.SecondOperatorPanel.Name = "SecondOperatorPanel";
            this.SecondOperatorPanel.Size = new System.Drawing.Size(1268, 535);
            this.SecondOperatorPanel.TabIndex = 71;
            // 
            // dgvSecondOperator
            // 
            this.dgvSecondOperator.AllowUserToAddRows = false;
            this.dgvSecondOperator.AllowUserToDeleteRows = false;
            this.dgvSecondOperator.AllowUserToResizeColumns = false;
            this.dgvSecondOperator.AllowUserToResizeRows = false;
            this.dgvSecondOperator.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSecondOperator.BackText = "Нет данных";
            this.dgvSecondOperator.ColumnHeadersHeight = 40;
            this.dgvSecondOperator.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSecondOperator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSecondOperator.HideOuterBorders = true;
            this.dgvSecondOperator.Location = new System.Drawing.Point(0, 0);
            this.dgvSecondOperator.Name = "dgvSecondOperator";
            this.dgvSecondOperator.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.dgvSecondOperator.PercentLineWidth = 0;
            this.dgvSecondOperator.ReadOnly = true;
            this.dgvSecondOperator.RowHeadersVisible = false;
            this.dgvSecondOperator.RowTemplate.Height = 30;
            this.dgvSecondOperator.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSecondOperator.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Blue;
            this.dgvSecondOperator.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSecondOperator.Size = new System.Drawing.Size(1268, 535);
            this.dgvSecondOperator.StandardStyle = false;
            this.dgvSecondOperator.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.dgvSecondOperator.StateCommon.Background.Color2 = System.Drawing.Color.Transparent;
            this.dgvSecondOperator.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvSecondOperator.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.dgvSecondOperator.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.dgvSecondOperator.StateCommon.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvSecondOperator.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.dgvSecondOperator.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvSecondOperator.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvSecondOperator.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.dgvSecondOperator.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dgvSecondOperator.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.dgvSecondOperator.StateCommon.HeaderColumn.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvSecondOperator.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvSecondOperator.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.dgvSecondOperator.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvSecondOperator.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvSecondOperator.StateCommon.HeaderColumn.Border.Width = 1;
            this.dgvSecondOperator.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.dgvSecondOperator.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.dgvSecondOperator.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.dgvSecondOperator.StateSelected.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.dgvSecondOperator.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvSecondOperator.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.dgvSecondOperator.StateSelected.HeaderRow.Border.Color1 = System.Drawing.Color.White;
            this.dgvSecondOperator.StateSelected.HeaderRow.Border.Color2 = System.Drawing.Color.White;
            this.dgvSecondOperator.StateSelected.HeaderRow.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvSecondOperator.TabIndex = 54;
            this.dgvSecondOperator.UseCustomBackColor = true;
            // 
            // kryptonBorderEdge6
            // 
            this.kryptonBorderEdge6.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge6.Location = new System.Drawing.Point(0, 1);
            this.kryptonBorderEdge6.Name = "kryptonBorderEdge6";
            this.kryptonBorderEdge6.Size = new System.Drawing.Size(1, 535);
            this.kryptonBorderEdge6.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge6.Text = "kryptonBorderEdge6";
            // 
            // kryptonBorderEdge7
            // 
            this.kryptonBorderEdge7.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge7.Location = new System.Drawing.Point(1269, 1);
            this.kryptonBorderEdge7.Name = "kryptonBorderEdge7";
            this.kryptonBorderEdge7.Size = new System.Drawing.Size(1, 535);
            this.kryptonBorderEdge7.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge7.Text = "kryptonBorderEdge7";
            // 
            // kryptonBorderEdge4
            // 
            this.kryptonBorderEdge4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge4.Location = new System.Drawing.Point(0, 536);
            this.kryptonBorderEdge4.Name = "kryptonBorderEdge4";
            this.kryptonBorderEdge4.Size = new System.Drawing.Size(1270, 1);
            this.kryptonBorderEdge4.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge4.Text = "kryptonBorderEdge4";
            // 
            // kryptonBorderEdge5
            // 
            this.kryptonBorderEdge5.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge5.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge5.Name = "kryptonBorderEdge5";
            this.kryptonBorderEdge5.Size = new System.Drawing.Size(1270, 1);
            this.kryptonBorderEdge5.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge5.Text = "kryptonBorderEdge5";
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
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.flowLayoutPanel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 54);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(1270, 45);
            this.kryptonPanel1.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.kryptonPanel1.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonPanel1.TabIndex = 296;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.cbtnFirstOperator);
            this.flowLayoutPanel1.Controls.Add(this.cbtnSecondOperator);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1270, 45);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // cbtnFirstOperator
            // 
            this.cbtnFirstOperator.Checked = true;
            this.cbtnFirstOperator.Location = new System.Drawing.Point(3, 3);
            this.cbtnFirstOperator.Name = "cbtnFirstOperator";
            this.cbtnFirstOperator.Palette = this.MainMenuPalette;
            this.cbtnFirstOperator.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.cbtnFirstOperator.Size = new System.Drawing.Size(143, 40);
            this.cbtnFirstOperator.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Times New Roman", 21F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cbtnFirstOperator.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(197)))), ((int)(((byte)(252)))));
            this.cbtnFirstOperator.TabIndex = 0;
            this.cbtnFirstOperator.Values.Text = "Оператор №1";
            // 
            // cbtnSecondOperator
            // 
            this.cbtnSecondOperator.Location = new System.Drawing.Point(152, 3);
            this.cbtnSecondOperator.Name = "cbtnSecondOperator";
            this.cbtnSecondOperator.Palette = this.MainMenuPalette;
            this.cbtnSecondOperator.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.cbtnSecondOperator.Size = new System.Drawing.Size(143, 40);
            this.cbtnSecondOperator.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Times New Roman", 21F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cbtnSecondOperator.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(197)))), ((int)(((byte)(252)))));
            this.cbtnSecondOperator.TabIndex = 1;
            this.cbtnSecondOperator.Values.Text = "Оператор №2";
            // 
            // kryptonCheckSet1
            // 
            this.kryptonCheckSet1.CheckButtons.Add(this.cbtnFirstOperator);
            this.kryptonCheckSet1.CheckButtons.Add(this.cbtnSecondOperator);
            this.kryptonCheckSet1.CheckedButton = this.cbtnFirstOperator;
            this.kryptonCheckSet1.CheckedButtonChanged += new System.EventHandler(this.kryptonCheckSet1_CheckedButtonChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbSecondOperators);
            this.panel1.Controls.Add(this.lbTotalErrors);
            this.panel1.Controls.Add(this.lbTotalTime);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dtpSecondDate);
            this.panel1.Controls.Add(this.dtpFirstDate);
            this.panel1.Controls.Add(this.cbxShowCorrectOrders);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.cmbFirstOperators);
            this.panel1.Controls.Add(this.kryptonBorderEdge2);
            this.panel1.Controls.Add(this.kryptonBorderEdge1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 99);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1270, 104);
            this.panel1.TabIndex = 297;
            // 
            // lbTotalErrors
            // 
            this.lbTotalErrors.AutoSize = true;
            this.lbTotalErrors.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lbTotalErrors.ForeColor = System.Drawing.Color.Black;
            this.lbTotalErrors.Location = new System.Drawing.Point(517, 59);
            this.lbTotalErrors.Name = "lbTotalErrors";
            this.lbTotalErrors.Size = new System.Drawing.Size(123, 21);
            this.lbTotalErrors.TabIndex = 471;
            this.lbTotalErrors.Text = "Кол-во ошибок:";
            // 
            // lbTotalTime
            // 
            this.lbTotalTime.AutoSize = true;
            this.lbTotalTime.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lbTotalTime.ForeColor = System.Drawing.Color.Black;
            this.lbTotalTime.Location = new System.Drawing.Point(517, 27);
            this.lbTotalTime.Name = "lbTotalTime";
            this.lbTotalTime.Size = new System.Drawing.Size(183, 21);
            this.lbTotalTime.TabIndex = 470;
            this.lbTotalTime.Text = "Общее время вбивания:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(248, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 21);
            this.label4.TabIndex = 469;
            this.label4.Text = "Оператор:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(19, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 21);
            this.label3.TabIndex = 468;
            this.label3.Text = "по";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(30, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 21);
            this.label2.TabIndex = 467;
            this.label2.Text = "с";
            // 
            // dtpSecondDate
            // 
            this.dtpSecondDate.Location = new System.Drawing.Point(53, 59);
            this.dtpSecondDate.Name = "dtpSecondDate";
            this.dtpSecondDate.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.dtpSecondDate.Size = new System.Drawing.Size(168, 26);
            this.dtpSecondDate.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dtpSecondDate.StateNormal.Content.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dtpSecondDate.TabIndex = 466;
            this.dtpSecondDate.ValueChanged += new System.EventHandler(this.dtpSecondDate_ValueChanged);
            // 
            // dtpFirstDate
            // 
            this.dtpFirstDate.Location = new System.Drawing.Point(53, 27);
            this.dtpFirstDate.Name = "dtpFirstDate";
            this.dtpFirstDate.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.dtpFirstDate.Size = new System.Drawing.Size(168, 26);
            this.dtpFirstDate.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dtpFirstDate.TabIndex = 465;
            this.dtpFirstDate.ValueChanged += new System.EventHandler(this.dtpFirstDate_ValueChanged);
            // 
            // cbxShowCorrectOrders
            // 
            this.cbxShowCorrectOrders.Checked = true;
            this.cbxShowCorrectOrders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxShowCorrectOrders.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.cbxShowCorrectOrders.Location = new System.Drawing.Point(252, 59);
            this.cbxShowCorrectOrders.Name = "cbxShowCorrectOrders";
            this.cbxShowCorrectOrders.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.cbxShowCorrectOrders.Size = new System.Drawing.Size(224, 25);
            this.cbxShowCorrectOrders.StateCommon.ShortText.Color1 = System.Drawing.Color.Black;
            this.cbxShowCorrectOrders.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cbxShowCorrectOrders.TabIndex = 464;
            this.cbxShowCorrectOrders.Text = "Не показывать правильные";
            this.cbxShowCorrectOrders.Values.Text = "Не показывать правильные";
            this.cbxShowCorrectOrders.CheckedChanged += new System.EventHandler(this.cbxShowCorrectOrders_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(49, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 21);
            this.label9.TabIndex = 462;
            this.label9.Text = "Период:";
            // 
            // cmbFirstOperators
            // 
            this.cmbFirstOperators.DropDownWidth = 121;
            this.cmbFirstOperators.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cmbFirstOperators.FormattingEnabled = true;
            this.cmbFirstOperators.Location = new System.Drawing.Point(252, 27);
            this.cmbFirstOperators.Name = "cmbFirstOperators";
            this.cmbFirstOperators.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.cmbFirstOperators.Size = new System.Drawing.Size(227, 26);
            this.cmbFirstOperators.StateCommon.ComboBox.Content.Color1 = System.Drawing.Color.Black;
            this.cmbFirstOperators.StateCommon.ComboBox.Content.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cmbFirstOperators.StateCommon.Item.Content.ShortText.Color1 = System.Drawing.Color.Black;
            this.cmbFirstOperators.StateCommon.Item.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cmbFirstOperators.TabIndex = 461;
            this.cmbFirstOperators.SelectedIndexChanged += new System.EventHandler(this.cmbOperators_SelectedIndexChanged);
            this.cmbFirstOperators.SelectionChangeCommitted += new System.EventHandler(this.cmbOperators_SelectionChangeCommitted);
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(1269, 0);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(1, 104);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(1, 104);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // cmbSecondOperators
            // 
            this.cmbSecondOperators.DropDownWidth = 121;
            this.cmbSecondOperators.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cmbSecondOperators.FormattingEnabled = true;
            this.cmbSecondOperators.Location = new System.Drawing.Point(252, 27);
            this.cmbSecondOperators.Name = "cmbSecondOperators";
            this.cmbSecondOperators.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.cmbSecondOperators.Size = new System.Drawing.Size(227, 26);
            this.cmbSecondOperators.StateCommon.ComboBox.Content.Color1 = System.Drawing.Color.Black;
            this.cmbSecondOperators.StateCommon.ComboBox.Content.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cmbSecondOperators.StateCommon.Item.Content.ShortText.Color1 = System.Drawing.Color.Black;
            this.cmbSecondOperators.StateCommon.Item.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cmbSecondOperators.TabIndex = 474;
            this.cmbSecondOperators.SelectedIndexChanged += new System.EventHandler(this.cmbSecondOperators_SelectedIndexChanged);
            // 
            // DoubleOrdersStatisticsForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1270, 740);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.kryptonPanel1);
            this.Controls.Add(this.NavigatePanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DoubleOrdersStatisticsForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.DoubleOrdersStatisticsForm_Load);
            this.Shown += new System.EventHandler(this.DoubleOrdersStatisticsForm_Shown);
            this.NavigatePanel.ResumeLayout(false);
            this.NavigatePanel.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.FirstOperatorPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFirstOperator)).EndInit();
            this.SecondOperatorPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSecondOperator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbFirstOperators)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSecondOperators)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonButton NavigateMenuCloseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel NavigatePanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette NavigateMenuButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton PasswordButton;
        private System.Windows.Forms.Panel panel6;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette MainMenuPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton cbtnFirstOperator;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton cbtnSecondOperator;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckSet kryptonCheckSet1;
        private System.Windows.Forms.Panel FirstOperatorPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton MinimizeButton;
        private System.Windows.Forms.Panel panel1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge6;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge7;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge4;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge5;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private System.Windows.Forms.Panel SecondOperatorPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox cbxShowCorrectOrders;
        private System.Windows.Forms.Label label9;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker dtpFirstDate;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cmbFirstOperators;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker dtpSecondDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbTotalErrors;
        private System.Windows.Forms.Label lbTotalTime;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private PercentageDataGrid dgvFirstOperator;
        private PercentageDataGrid dgvSecondOperator;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cmbSecondOperators;
    }
}