namespace Infinium
{
    partial class TechStorageItemsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TechStorageItemsForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.NavigateMenuCloseButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.NavigateMenuButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.NavigatePanel = new System.Windows.Forms.Panel();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.panel1 = new System.Windows.Forms.Panel();
            this.GroupsDataGrid = new Infinium.PercentageDataGrid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ItemsDataGrid = new Infinium.PercentageDataGrid();
            this.StandardButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.AddGroupButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.AddItemButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.RemoveGroupButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.RemoveItemButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.GroupTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.ChangeItemButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.EditGroupButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.SubGroupsDataGrid = new Infinium.PercentageDataGrid();
            this.EditSubGroupButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SubGroupTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.RemoveSubGroupButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.AddSubGroupButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.kryptonButton3 = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonButton2 = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnMoveTechStoreIDs = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnCopyTechStoreIDs = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.AddTechSubGroupToStoreDetailButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.AddTechStoreToStoreDetailButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ToolsPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddInsetTypes = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.MainMenuPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.btnSaveInsetTypes = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnAddInsetColors = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnSaveInsetColors = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnStoreColors = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonContextMenu1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems8 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem59 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenu2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.NavigatePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GroupsDataGrid)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemsDataGrid)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SubGroupsDataGrid)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ToolsPanel)).BeginInit();
            this.ToolsPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.label1.Location = new System.Drawing.Point(2, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(455, 45);
            this.label1.TabIndex = 31;
            this.label1.Text = "Infinium. Склад. Номенклатура";
            // 
            // NavigatePanel
            // 
            this.NavigatePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.NavigatePanel.Controls.Add(this.kryptonBorderEdge3);
            this.NavigatePanel.Controls.Add(this.NavigateMenuCloseButton);
            this.NavigatePanel.Controls.Add(this.label1);
            this.NavigatePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NavigatePanel.Location = new System.Drawing.Point(0, 0);
            this.NavigatePanel.Name = "NavigatePanel";
            this.NavigatePanel.Size = new System.Drawing.Size(1270, 54);
            this.NavigatePanel.TabIndex = 34;
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(0, 53);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(1270, 1);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(123)))), ((int)(((byte)(97)))));
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.Silver;
            this.kryptonBorderEdge3.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.GroupsDataGrid);
            this.panel1.Location = new System.Drawing.Point(24, 141);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 508);
            this.panel1.TabIndex = 36;
            // 
            // GroupsDataGrid
            // 
            this.GroupsDataGrid.AllowUserToAddRows = false;
            this.GroupsDataGrid.AllowUserToDeleteRows = false;
            this.GroupsDataGrid.AllowUserToResizeColumns = false;
            this.GroupsDataGrid.AllowUserToResizeRows = false;
            this.GroupsDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GroupsDataGrid.BackText = "Нет данных";
            this.GroupsDataGrid.ColumnHeadersHeight = 40;
            this.GroupsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.GroupsDataGrid.ColumnHeadersVisible = false;
            this.GroupsDataGrid.Location = new System.Drawing.Point(0, -1);
            this.GroupsDataGrid.MultiSelect = false;
            this.GroupsDataGrid.Name = "GroupsDataGrid";
            this.GroupsDataGrid.PercentLineWidth = 0;
            this.GroupsDataGrid.RowHeadersVisible = false;
            this.GroupsDataGrid.RowTemplate.Height = 30;
            this.GroupsDataGrid.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Blue;
            this.GroupsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GroupsDataGrid.Size = new System.Drawing.Size(281, 507);
            this.GroupsDataGrid.StandardStyle = false;
            this.GroupsDataGrid.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.GroupsDataGrid.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.GroupsDataGrid.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.GroupsDataGrid.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.GroupsDataGrid.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.GroupsDataGrid.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.GroupsDataGrid.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.GroupsDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.GroupsDataGrid.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.GroupsDataGrid.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.GroupsDataGrid.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.GroupsDataGrid.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.GroupsDataGrid.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.GroupsDataGrid.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.GroupsDataGrid.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.GroupsDataGrid.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.GroupsDataGrid.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.GroupsDataGrid.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.GroupsDataGrid.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.GroupsDataGrid.TabIndex = 35;
            this.GroupsDataGrid.UseCustomBackColor = false;
            this.GroupsDataGrid.SelectionChanged += new System.EventHandler(this.GroupsDataGrid_SelectionChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.ItemsDataGrid);
            this.panel2.Location = new System.Drawing.Point(600, 141);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(647, 509);
            this.panel2.TabIndex = 37;
            // 
            // ItemsDataGrid
            // 
            this.ItemsDataGrid.AllowUserToAddRows = false;
            this.ItemsDataGrid.AllowUserToDeleteRows = false;
            this.ItemsDataGrid.AllowUserToResizeColumns = false;
            this.ItemsDataGrid.AllowUserToResizeRows = false;
            this.ItemsDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ItemsDataGrid.BackText = "Нет данных";
            this.ItemsDataGrid.ColumnHeadersHeight = 50;
            this.ItemsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ItemsDataGrid.Location = new System.Drawing.Point(0, -1);
            this.ItemsDataGrid.MultiSelect = false;
            this.ItemsDataGrid.Name = "ItemsDataGrid";
            this.ItemsDataGrid.PercentLineWidth = 0;
            this.ItemsDataGrid.RowHeadersVisible = false;
            this.ItemsDataGrid.RowTemplate.Height = 30;
            this.ItemsDataGrid.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Orange;
            this.ItemsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ItemsDataGrid.Size = new System.Drawing.Size(646, 508);
            this.ItemsDataGrid.StandardStyle = false;
            this.ItemsDataGrid.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.ItemsDataGrid.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ItemsDataGrid.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.ItemsDataGrid.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.ItemsDataGrid.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.ItemsDataGrid.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ItemsDataGrid.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ItemsDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.ItemsDataGrid.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ItemsDataGrid.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.ItemsDataGrid.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ItemsDataGrid.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.ItemsDataGrid.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ItemsDataGrid.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ItemsDataGrid.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.ItemsDataGrid.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.ItemsDataGrid.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.ItemsDataGrid.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ItemsDataGrid.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.ItemsDataGrid.TabIndex = 35;
            this.ItemsDataGrid.UseCustomBackColor = false;
            this.ItemsDataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ItemsDataGrid_CellDoubleClick);
            this.ItemsDataGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ItemsDataGrid_CellMouseDown);
            this.ItemsDataGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.ItemsDataGrid_DataError);
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
            // AddGroupButton
            // 
            this.AddGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddGroupButton.Location = new System.Drawing.Point(144, 33);
            this.AddGroupButton.Name = "AddGroupButton";
            this.AddGroupButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddGroupButton.Palette = this.StandardButtonsPalette;
            this.AddGroupButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.AddGroupButton.Size = new System.Drawing.Size(42, 39);
            this.AddGroupButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddGroupButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.AddGroupButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddGroupButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddGroupButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddGroupButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddGroupButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.AddGroupButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AddGroupButton.TabIndex = 275;
            this.AddGroupButton.TabStop = false;
            this.AddGroupButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("AddGroupButton.Values.Image")));
            this.AddGroupButton.Values.Text = "";
            this.AddGroupButton.Click += new System.EventHandler(this.AddGroupButton_Click);
            // 
            // AddItemButton
            // 
            this.AddItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddItemButton.Location = new System.Drawing.Point(1085, 3);
            this.AddItemButton.Name = "AddItemButton";
            this.AddItemButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddItemButton.Palette = this.StandardButtonsPalette;
            this.AddItemButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.AddItemButton.Size = new System.Drawing.Size(42, 39);
            this.AddItemButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddItemButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.AddItemButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddItemButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddItemButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddItemButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddItemButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.AddItemButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AddItemButton.TabIndex = 276;
            this.AddItemButton.TabStop = false;
            this.AddItemButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("AddItemButton.Values.Image")));
            this.AddItemButton.Values.Text = "";
            this.AddItemButton.Click += new System.EventHandler(this.AddItemButton_Click);
            // 
            // RemoveGroupButton
            // 
            this.RemoveGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveGroupButton.Location = new System.Drawing.Point(240, 33);
            this.RemoveGroupButton.Name = "RemoveGroupButton";
            this.RemoveGroupButton.OverrideDefault.Back.Color1 = System.Drawing.Color.Red;
            this.RemoveGroupButton.Palette = this.StandardButtonsPalette;
            this.RemoveGroupButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.RemoveGroupButton.Size = new System.Drawing.Size(42, 39);
            this.RemoveGroupButton.StateCommon.Back.Color1 = System.Drawing.Color.Red;
            this.RemoveGroupButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.RemoveGroupButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.RemoveGroupButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.RemoveGroupButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RemoveGroupButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.RemoveGroupButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.RemoveGroupButton.TabIndex = 277;
            this.RemoveGroupButton.TabStop = false;
            this.RemoveGroupButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("RemoveGroupButton.Values.Image")));
            this.RemoveGroupButton.Values.Text = "";
            this.RemoveGroupButton.Click += new System.EventHandler(this.RemoveGroupButton_Click);
            // 
            // RemoveItemButton
            // 
            this.RemoveItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveItemButton.Location = new System.Drawing.Point(1181, 3);
            this.RemoveItemButton.Name = "RemoveItemButton";
            this.RemoveItemButton.OverrideDefault.Back.Color1 = System.Drawing.Color.Red;
            this.RemoveItemButton.Palette = this.StandardButtonsPalette;
            this.RemoveItemButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.RemoveItemButton.Size = new System.Drawing.Size(42, 39);
            this.RemoveItemButton.StateCommon.Back.Color1 = System.Drawing.Color.Red;
            this.RemoveItemButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.RemoveItemButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.RemoveItemButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.RemoveItemButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RemoveItemButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.RemoveItemButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.RemoveItemButton.TabIndex = 278;
            this.RemoveItemButton.TabStop = false;
            this.RemoveItemButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("RemoveItemButton.Values.Image")));
            this.RemoveItemButton.Values.Text = "";
            this.RemoveItemButton.Click += new System.EventHandler(this.RemoveItemButton_Click);
            // 
            // GroupTextBox
            // 
            this.GroupTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GroupTextBox.Location = new System.Drawing.Point(0, 3);
            this.GroupTextBox.Name = "GroupTextBox";
            this.GroupTextBox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.GroupTextBox.Size = new System.Drawing.Size(282, 27);
            this.GroupTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.GroupTextBox.TabIndex = 279;
            // 
            // ChangeItemButton
            // 
            this.ChangeItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ChangeItemButton.Location = new System.Drawing.Point(1133, 3);
            this.ChangeItemButton.Name = "ChangeItemButton";
            this.ChangeItemButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.ChangeItemButton.Palette = this.StandardButtonsPalette;
            this.ChangeItemButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.ChangeItemButton.Size = new System.Drawing.Size(42, 39);
            this.ChangeItemButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.ChangeItemButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ChangeItemButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.ChangeItemButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.ChangeItemButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChangeItemButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.ChangeItemButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.ChangeItemButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ChangeItemButton.TabIndex = 280;
            this.ChangeItemButton.TabStop = false;
            this.ChangeItemButton.Values.Text = "C";
            this.ChangeItemButton.Click += new System.EventHandler(this.ChangeItemButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 17.51F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(19, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 25);
            this.label2.TabIndex = 282;
            this.label2.Text = "Группа";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 17.51F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.Location = new System.Drawing.Point(596, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 25);
            this.label3.TabIndex = 283;
            this.label3.Text = "Наименование";
            // 
            // EditGroupButton
            // 
            this.EditGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditGroupButton.Location = new System.Drawing.Point(192, 33);
            this.EditGroupButton.Name = "EditGroupButton";
            this.EditGroupButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.EditGroupButton.Palette = this.StandardButtonsPalette;
            this.EditGroupButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.EditGroupButton.Size = new System.Drawing.Size(42, 39);
            this.EditGroupButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.EditGroupButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.EditGroupButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditGroupButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditGroupButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EditGroupButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditGroupButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditGroupButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.EditGroupButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.EditGroupButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.EditGroupButton.TabIndex = 345;
            this.EditGroupButton.TabStop = false;
            this.EditGroupButton.Values.Text = "C";
            this.EditGroupButton.Click += new System.EventHandler(this.EditGroupButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 17.51F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label4.Location = new System.Drawing.Point(307, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 25);
            this.label4.TabIndex = 347;
            this.label4.Text = "Подгруппа";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.SubGroupsDataGrid);
            this.panel3.Location = new System.Drawing.Point(312, 141);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(282, 508);
            this.panel3.TabIndex = 346;
            // 
            // SubGroupsDataGrid
            // 
            this.SubGroupsDataGrid.AllowUserToAddRows = false;
            this.SubGroupsDataGrid.AllowUserToDeleteRows = false;
            this.SubGroupsDataGrid.AllowUserToResizeColumns = false;
            this.SubGroupsDataGrid.AllowUserToResizeRows = false;
            this.SubGroupsDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SubGroupsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.SubGroupsDataGrid.BackText = "Нет данных";
            this.SubGroupsDataGrid.ColumnHeadersHeight = 40;
            this.SubGroupsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.SubGroupsDataGrid.ColumnHeadersVisible = false;
            this.SubGroupsDataGrid.Location = new System.Drawing.Point(0, -1);
            this.SubGroupsDataGrid.MultiSelect = false;
            this.SubGroupsDataGrid.Name = "SubGroupsDataGrid";
            this.SubGroupsDataGrid.PercentLineWidth = 0;
            this.SubGroupsDataGrid.RowHeadersVisible = false;
            this.SubGroupsDataGrid.RowTemplate.Height = 30;
            this.SubGroupsDataGrid.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Blue;
            this.SubGroupsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.SubGroupsDataGrid.Size = new System.Drawing.Size(281, 507);
            this.SubGroupsDataGrid.StandardStyle = false;
            this.SubGroupsDataGrid.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.SubGroupsDataGrid.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.SubGroupsDataGrid.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.SubGroupsDataGrid.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.SubGroupsDataGrid.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.SubGroupsDataGrid.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.SubGroupsDataGrid.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.SubGroupsDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.SubGroupsDataGrid.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.SubGroupsDataGrid.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.SubGroupsDataGrid.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.SubGroupsDataGrid.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.SubGroupsDataGrid.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.SubGroupsDataGrid.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.SubGroupsDataGrid.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.SubGroupsDataGrid.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.SubGroupsDataGrid.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.SubGroupsDataGrid.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.SubGroupsDataGrid.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.SubGroupsDataGrid.TabIndex = 35;
            this.SubGroupsDataGrid.UseCustomBackColor = false;
            this.SubGroupsDataGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.SubGroupsDataGrid_CellMouseDown);
            this.SubGroupsDataGrid.SelectionChanged += new System.EventHandler(this.SubGroupsDataGrid_SelectionChanged);
            // 
            // EditSubGroupButton
            // 
            this.EditSubGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditSubGroupButton.Location = new System.Drawing.Point(480, 33);
            this.EditSubGroupButton.Name = "EditSubGroupButton";
            this.EditSubGroupButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.EditSubGroupButton.Palette = this.StandardButtonsPalette;
            this.EditSubGroupButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.EditSubGroupButton.Size = new System.Drawing.Size(42, 39);
            this.EditSubGroupButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.EditSubGroupButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.EditSubGroupButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditSubGroupButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditSubGroupButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EditSubGroupButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditSubGroupButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.EditSubGroupButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.EditSubGroupButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.EditSubGroupButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.EditSubGroupButton.TabIndex = 351;
            this.EditSubGroupButton.TabStop = false;
            this.EditSubGroupButton.Values.Text = "C";
            this.EditSubGroupButton.Click += new System.EventHandler(this.EditSubGroupButton_Click);
            // 
            // SubGroupTextBox
            // 
            this.SubGroupTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SubGroupTextBox.Location = new System.Drawing.Point(288, 3);
            this.SubGroupTextBox.Name = "SubGroupTextBox";
            this.SubGroupTextBox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.SubGroupTextBox.Size = new System.Drawing.Size(282, 27);
            this.SubGroupTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.SubGroupTextBox.TabIndex = 350;
            // 
            // RemoveSubGroupButton
            // 
            this.RemoveSubGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveSubGroupButton.Location = new System.Drawing.Point(528, 33);
            this.RemoveSubGroupButton.Name = "RemoveSubGroupButton";
            this.RemoveSubGroupButton.OverrideDefault.Back.Color1 = System.Drawing.Color.Red;
            this.RemoveSubGroupButton.Palette = this.StandardButtonsPalette;
            this.RemoveSubGroupButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.RemoveSubGroupButton.Size = new System.Drawing.Size(42, 39);
            this.RemoveSubGroupButton.StateCommon.Back.Color1 = System.Drawing.Color.Red;
            this.RemoveSubGroupButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.RemoveSubGroupButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.RemoveSubGroupButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.RemoveSubGroupButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RemoveSubGroupButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.RemoveSubGroupButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.RemoveSubGroupButton.TabIndex = 349;
            this.RemoveSubGroupButton.TabStop = false;
            this.RemoveSubGroupButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("RemoveSubGroupButton.Values.Image")));
            this.RemoveSubGroupButton.Values.Text = "";
            this.RemoveSubGroupButton.Click += new System.EventHandler(this.RemoveSubGroupButton_Click);
            // 
            // AddSubGroupButton
            // 
            this.AddSubGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddSubGroupButton.Location = new System.Drawing.Point(432, 33);
            this.AddSubGroupButton.Name = "AddSubGroupButton";
            this.AddSubGroupButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddSubGroupButton.Palette = this.StandardButtonsPalette;
            this.AddSubGroupButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.AddSubGroupButton.Size = new System.Drawing.Size(42, 39);
            this.AddSubGroupButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddSubGroupButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.AddSubGroupButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddSubGroupButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddSubGroupButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddSubGroupButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddSubGroupButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.AddSubGroupButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AddSubGroupButton.TabIndex = 348;
            this.AddSubGroupButton.TabStop = false;
            this.AddSubGroupButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("AddSubGroupButton.Values.Image")));
            this.AddSubGroupButton.Values.Text = "";
            this.AddSubGroupButton.Click += new System.EventHandler(this.AddSubGroupButton_Click);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Controls.Add(this.kryptonButton3);
            this.panel4.Controls.Add(this.kryptonButton2);
            this.panel4.Controls.Add(this.btnMoveTechStoreIDs);
            this.panel4.Controls.Add(this.btnCopyTechStoreIDs);
            this.panel4.Controls.Add(this.EditSubGroupButton);
            this.panel4.Controls.Add(this.GroupTextBox);
            this.panel4.Controls.Add(this.SubGroupTextBox);
            this.panel4.Controls.Add(this.AddGroupButton);
            this.panel4.Controls.Add(this.RemoveSubGroupButton);
            this.panel4.Controls.Add(this.AddItemButton);
            this.panel4.Controls.Add(this.AddSubGroupButton);
            this.panel4.Controls.Add(this.RemoveGroupButton);
            this.panel4.Controls.Add(this.RemoveItemButton);
            this.panel4.Controls.Add(this.ChangeItemButton);
            this.panel4.Controls.Add(this.EditGroupButton);
            this.panel4.Location = new System.Drawing.Point(24, 656);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1223, 74);
            this.panel4.TabIndex = 352;
            // 
            // kryptonButton3
            // 
            this.kryptonButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonButton3.Location = new System.Drawing.Point(667, 3);
            this.kryptonButton3.Name = "kryptonButton3";
            this.kryptonButton3.Palette = this.StandardButtonsPalette;
            this.kryptonButton3.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.kryptonButton3.Size = new System.Drawing.Size(84, 39);
            this.kryptonButton3.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.kryptonButton3.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.kryptonButton3.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.kryptonButton3.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.kryptonButton3.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.kryptonButton3.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.kryptonButton3.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonButton3.TabIndex = 444;
            this.kryptonButton3.TabStop = false;
            this.kryptonButton3.Values.Text = "Отмена";
            this.kryptonButton3.Visible = false;
            this.kryptonButton3.Click += new System.EventHandler(this.kryptonButton3_Click);
            // 
            // kryptonButton2
            // 
            this.kryptonButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonButton2.Location = new System.Drawing.Point(577, 3);
            this.kryptonButton2.Name = "kryptonButton2";
            this.kryptonButton2.Palette = this.StandardButtonsPalette;
            this.kryptonButton2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.kryptonButton2.Size = new System.Drawing.Size(84, 39);
            this.kryptonButton2.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.kryptonButton2.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.kryptonButton2.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.kryptonButton2.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.kryptonButton2.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.kryptonButton2.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.kryptonButton2.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonButton2.TabIndex = 443;
            this.kryptonButton2.TabStop = false;
            this.kryptonButton2.Values.Text = "ОК";
            this.kryptonButton2.Visible = false;
            this.kryptonButton2.Click += new System.EventHandler(this.kryptonButton2_Click);
            // 
            // btnMoveTechStoreIDs
            // 
            this.btnMoveTechStoreIDs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveTechStoreIDs.Location = new System.Drawing.Point(989, 3);
            this.btnMoveTechStoreIDs.Name = "btnMoveTechStoreIDs";
            this.btnMoveTechStoreIDs.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.btnMoveTechStoreIDs.Palette = this.StandardButtonsPalette;
            this.btnMoveTechStoreIDs.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnMoveTechStoreIDs.Size = new System.Drawing.Size(42, 39);
            this.btnMoveTechStoreIDs.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.btnMoveTechStoreIDs.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnMoveTechStoreIDs.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnMoveTechStoreIDs.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnMoveTechStoreIDs.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMoveTechStoreIDs.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnMoveTechStoreIDs.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnMoveTechStoreIDs.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.btnMoveTechStoreIDs.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.btnMoveTechStoreIDs.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.btnMoveTechStoreIDs.TabIndex = 413;
            this.btnMoveTechStoreIDs.TabStop = false;
            this.btnMoveTechStoreIDs.Tag = "41";
            this.btnMoveTechStoreIDs.Values.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveTechStoreIDs.Values.Image")));
            this.btnMoveTechStoreIDs.Values.Text = "";
            this.btnMoveTechStoreIDs.Click += new System.EventHandler(this.btnMoveTechStoreIDs_Click);
            // 
            // btnCopyTechStoreIDs
            // 
            this.btnCopyTechStoreIDs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyTechStoreIDs.Location = new System.Drawing.Point(1037, 3);
            this.btnCopyTechStoreIDs.Name = "btnCopyTechStoreIDs";
            this.btnCopyTechStoreIDs.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.btnCopyTechStoreIDs.Palette = this.StandardButtonsPalette;
            this.btnCopyTechStoreIDs.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnCopyTechStoreIDs.Size = new System.Drawing.Size(42, 39);
            this.btnCopyTechStoreIDs.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.btnCopyTechStoreIDs.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnCopyTechStoreIDs.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnCopyTechStoreIDs.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnCopyTechStoreIDs.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCopyTechStoreIDs.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnCopyTechStoreIDs.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.btnCopyTechStoreIDs.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.btnCopyTechStoreIDs.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.btnCopyTechStoreIDs.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.btnCopyTechStoreIDs.TabIndex = 412;
            this.btnCopyTechStoreIDs.TabStop = false;
            this.btnCopyTechStoreIDs.Tag = "41";
            this.btnCopyTechStoreIDs.Values.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyTechStoreIDs.Values.Image")));
            this.btnCopyTechStoreIDs.Values.Text = "";
            this.btnCopyTechStoreIDs.Click += new System.EventHandler(this.btnCopyTechStoreIDs_Click);
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.Controls.Add(this.AddTechSubGroupToStoreDetailButton);
            this.panel5.Controls.Add(this.AddTechStoreToStoreDetailButton);
            this.panel5.Location = new System.Drawing.Point(24, 656);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1223, 74);
            this.panel5.TabIndex = 353;
            // 
            // AddTechSubGroupToStoreDetailButton
            // 
            this.AddTechSubGroupToStoreDetailButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddTechSubGroupToStoreDetailButton.Location = new System.Drawing.Point(528, 3);
            this.AddTechSubGroupToStoreDetailButton.Name = "AddTechSubGroupToStoreDetailButton";
            this.AddTechSubGroupToStoreDetailButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddTechSubGroupToStoreDetailButton.Palette = this.StandardButtonsPalette;
            this.AddTechSubGroupToStoreDetailButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.AddTechSubGroupToStoreDetailButton.Size = new System.Drawing.Size(42, 39);
            this.AddTechSubGroupToStoreDetailButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddTechSubGroupToStoreDetailButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.AddTechSubGroupToStoreDetailButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddTechSubGroupToStoreDetailButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddTechSubGroupToStoreDetailButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddTechSubGroupToStoreDetailButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddTechSubGroupToStoreDetailButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.AddTechSubGroupToStoreDetailButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AddTechSubGroupToStoreDetailButton.TabIndex = 350;
            this.AddTechSubGroupToStoreDetailButton.TabStop = false;
            this.AddTechSubGroupToStoreDetailButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("AddTechSubGroupToStoreDetailButton.Values.Image")));
            this.AddTechSubGroupToStoreDetailButton.Values.Text = "";
            this.AddTechSubGroupToStoreDetailButton.Click += new System.EventHandler(this.AddTechSubGroupToStoreDetailButton_Click);
            // 
            // AddTechStoreToStoreDetailButton
            // 
            this.AddTechStoreToStoreDetailButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddTechStoreToStoreDetailButton.Location = new System.Drawing.Point(1181, 3);
            this.AddTechStoreToStoreDetailButton.Name = "AddTechStoreToStoreDetailButton";
            this.AddTechStoreToStoreDetailButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddTechStoreToStoreDetailButton.Palette = this.StandardButtonsPalette;
            this.AddTechStoreToStoreDetailButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.AddTechStoreToStoreDetailButton.Size = new System.Drawing.Size(42, 39);
            this.AddTechStoreToStoreDetailButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddTechStoreToStoreDetailButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.AddTechStoreToStoreDetailButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddTechStoreToStoreDetailButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AddTechStoreToStoreDetailButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddTechStoreToStoreDetailButton.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.AddTechStoreToStoreDetailButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.AddTechStoreToStoreDetailButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AddTechStoreToStoreDetailButton.TabIndex = 348;
            this.AddTechStoreToStoreDetailButton.TabStop = false;
            this.AddTechStoreToStoreDetailButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("AddTechStoreToStoreDetailButton.Values.Image")));
            this.AddTechStoreToStoreDetailButton.Values.Text = "";
            this.AddTechStoreToStoreDetailButton.Click += new System.EventHandler(this.AddTechStoreToStoreDetailButton_Click);
            // 
            // ToolsPanel
            // 
            this.ToolsPanel.Controls.Add(this.flowLayoutPanel1);
            this.ToolsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolsPanel.Location = new System.Drawing.Point(0, 54);
            this.ToolsPanel.Name = "ToolsPanel";
            this.ToolsPanel.Size = new System.Drawing.Size(1270, 45);
            this.ToolsPanel.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(178)))), ((int)(((byte)(0)))));
            this.ToolsPanel.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ToolsPanel.TabIndex = 354;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.btnAddInsetTypes);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveInsetTypes);
            this.flowLayoutPanel1.Controls.Add(this.btnAddInsetColors);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveInsetColors);
            this.flowLayoutPanel1.Controls.Add(this.btnStoreColors);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(573, 43);
            this.flowLayoutPanel1.TabIndex = 368;
            // 
            // btnAddInsetTypes
            // 
            this.btnAddInsetTypes.Location = new System.Drawing.Point(3, 3);
            this.btnAddInsetTypes.Name = "btnAddInsetTypes";
            this.btnAddInsetTypes.Palette = this.MainMenuPalette1;
            this.btnAddInsetTypes.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnAddInsetTypes.Size = new System.Drawing.Size(93, 40);
            this.btnAddInsetTypes.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlLight;
            this.btnAddInsetTypes.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnAddInsetTypes.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnAddInsetTypes.TabIndex = 367;
            this.btnAddInsetTypes.Values.ExtraText = "Добавить\r\nвставку";
            this.btnAddInsetTypes.Values.Text = "";
            this.btnAddInsetTypes.Click += new System.EventHandler(this.btnAddInsetTypes_Click);
            // 
            // MainMenuPalette1
            // 
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.Transparent;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.SystemColors.ControlLight;
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
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.LongText.Color1 = System.Drawing.Color.White;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.LongText.Font = new System.Drawing.Font("Segoe UI Symbol", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.Padding = new System.Windows.Forms.Padding(-1, -1, -1, -4);
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Symbol", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLineH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.MainMenuPalette1.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            // 
            // btnSaveInsetTypes
            // 
            this.btnSaveInsetTypes.Location = new System.Drawing.Point(102, 3);
            this.btnSaveInsetTypes.Name = "btnSaveInsetTypes";
            this.btnSaveInsetTypes.Palette = this.MainMenuPalette1;
            this.btnSaveInsetTypes.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnSaveInsetTypes.Size = new System.Drawing.Size(93, 40);
            this.btnSaveInsetTypes.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlLight;
            this.btnSaveInsetTypes.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnSaveInsetTypes.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSaveInsetTypes.TabIndex = 364;
            this.btnSaveInsetTypes.Values.ExtraText = "Сохранить\r\nвставку";
            this.btnSaveInsetTypes.Values.Text = "";
            this.btnSaveInsetTypes.Visible = false;
            this.btnSaveInsetTypes.Click += new System.EventHandler(this.btnSaveInsetTypes_Click);
            // 
            // btnAddInsetColors
            // 
            this.btnAddInsetColors.Location = new System.Drawing.Point(218, 3);
            this.btnAddInsetColors.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.btnAddInsetColors.Name = "btnAddInsetColors";
            this.btnAddInsetColors.Palette = this.MainMenuPalette1;
            this.btnAddInsetColors.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnAddInsetColors.Size = new System.Drawing.Size(93, 40);
            this.btnAddInsetColors.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlLight;
            this.btnAddInsetColors.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnAddInsetColors.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnAddInsetColors.TabIndex = 367;
            this.btnAddInsetColors.Tag = "";
            this.btnAddInsetColors.Values.ExtraText = "Добавить\r\nцвет";
            this.btnAddInsetColors.Values.Text = "";
            this.btnAddInsetColors.Click += new System.EventHandler(this.btnAddInsetColors_Click);
            // 
            // btnSaveInsetColors
            // 
            this.btnSaveInsetColors.Location = new System.Drawing.Point(317, 3);
            this.btnSaveInsetColors.Name = "btnSaveInsetColors";
            this.btnSaveInsetColors.Palette = this.MainMenuPalette1;
            this.btnSaveInsetColors.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnSaveInsetColors.Size = new System.Drawing.Size(93, 40);
            this.btnSaveInsetColors.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlLight;
            this.btnSaveInsetColors.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnSaveInsetColors.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSaveInsetColors.TabIndex = 364;
            this.btnSaveInsetColors.Values.ExtraText = "Сохранить\r\nцвет";
            this.btnSaveInsetColors.Values.Text = "";
            this.btnSaveInsetColors.Visible = false;
            this.btnSaveInsetColors.Click += new System.EventHandler(this.btnSaveInsetColors_Click);
            // 
            // btnStoreColors
            // 
            this.btnStoreColors.Location = new System.Drawing.Point(433, 3);
            this.btnStoreColors.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.btnStoreColors.Name = "btnStoreColors";
            this.btnStoreColors.Palette = this.MainMenuPalette1;
            this.btnStoreColors.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.btnStoreColors.Size = new System.Drawing.Size(93, 40);
            this.btnStoreColors.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlLight;
            this.btnStoreColors.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnStoreColors.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnStoreColors.TabIndex = 368;
            this.btnStoreColors.Tag = "";
            this.btnStoreColors.Values.ExtraText = "Каталог\r\nцветов";
            this.btnStoreColors.Values.Text = "";
            this.btnStoreColors.Visible = false;
            this.btnStoreColors.Click += new System.EventHandler(this.btnStoreColors_Click);
            // 
            // kryptonContextMenu1
            // 
            this.kryptonContextMenu1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItems8});
            // 
            // kryptonContextMenuItems8
            // 
            this.kryptonContextMenuItems8.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItem59});
            // 
            // kryptonContextMenuItem59
            // 
            this.kryptonContextMenuItem59.Text = "Копировать";
            this.kryptonContextMenuItem59.Click += new System.EventHandler(this.kryptonContextMenuItem59_Click);
            // 
            // kryptonContextMenu2
            // 
            this.kryptonContextMenu2.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItems1});
            // 
            // kryptonContextMenuItems1
            // 
            this.kryptonContextMenuItems1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItem2,
            this.kryptonContextMenuItem1});
            // 
            // kryptonContextMenuItem2
            // 
            this.kryptonContextMenuItem2.Text = "Вставить без замены";
            this.kryptonContextMenuItem2.Click += new System.EventHandler(this.kryptonContextMenuItem2_Click);
            // 
            // kryptonContextMenuItem1
            // 
            this.kryptonContextMenuItem1.Text = "Вставить с заменой";
            this.kryptonContextMenuItem1.Click += new System.EventHandler(this.kryptonContextMenuItem1_Click);
            // 
            // TechStorageItemsForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1270, 740);
            this.Controls.Add(this.ToolsPanel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.NavigatePanel);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TechStorageItemsForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.TechStorageItemsForm_Load);
            this.Shown += new System.EventHandler(this.TechStorageItemsForm_Shown);
            this.NavigatePanel.ResumeLayout(false);
            this.NavigatePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GroupsDataGrid)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ItemsDataGrid)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SubGroupsDataGrid)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ToolsPanel)).EndInit();
            this.ToolsPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonButton NavigateMenuCloseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel NavigatePanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette NavigateMenuButtonsPalette;
        private PercentageDataGrid GroupsDataGrid;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private PercentageDataGrid ItemsDataGrid;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette StandardButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonButton AddGroupButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton AddItemButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton RemoveGroupButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton RemoveItemButton;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox GroupTextBox;
        private ComponentFactory.Krypton.Toolkit.KryptonButton ChangeItemButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private ComponentFactory.Krypton.Toolkit.KryptonButton EditGroupButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private PercentageDataGrid SubGroupsDataGrid;
        private ComponentFactory.Krypton.Toolkit.KryptonButton EditSubGroupButton;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox SubGroupTextBox;
        private ComponentFactory.Krypton.Toolkit.KryptonButton RemoveSubGroupButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton AddSubGroupButton;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private ComponentFactory.Krypton.Toolkit.KryptonButton AddTechStoreToStoreDetailButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton AddTechSubGroupToStoreDetailButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel ToolsPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAddInsetTypes;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette MainMenuPalette1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSaveInsetTypes;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAddInsetColors;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSaveInsetColors;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnStoreColors;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu kryptonContextMenu1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems8;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem59;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu kryptonContextMenu2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCopyTechStoreIDs;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnMoveTechStoreIDs;
        private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButton3;
        private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButton2;
    }
}