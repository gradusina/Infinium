namespace Infinium
{
    partial class UsersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsersForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.NavigateMenuCloseButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.NavigateMenuButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.NavigatePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DefaultViewButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.ShortViewButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.OfflineButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.OnlineButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.AlphabeticalButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.kryptonBorderEdge8 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonCheckSet2 = new ComponentFactory.Krypton.Toolkit.KryptonCheckSet(this.components);
            this.kryptonCheckSet1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckSet(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.kryptonBorderEdge10 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge11 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge12 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge13 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.DepartmentsDataGrid = new Infinium.PercentageDataGrid();
            this.UsersList = new Infinium.LightUsersList();
            this.NavigatePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DepartmentsDataGrid)).BeginInit();
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
            this.NavigateMenuCloseButton.Location = new System.Drawing.Point(1220, 10);
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
            // NavigatePanel
            // 
            this.NavigatePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.NavigatePanel.Controls.Add(this.label1);
            this.NavigatePanel.Controls.Add(this.kryptonBorderEdge3);
            this.NavigatePanel.Controls.Add(this.NavigateMenuCloseButton);
            this.NavigatePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NavigatePanel.Location = new System.Drawing.Point(0, 0);
            this.NavigatePanel.Name = "NavigatePanel";
            this.NavigatePanel.Size = new System.Drawing.Size(1270, 58);
            this.NavigatePanel.TabIndex = 34;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 32.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(313, 45);
            this.label1.TabIndex = 34;
            this.label1.Text = "Infinium. Сотрудники";
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(0, 57);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(1270, 1);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.Silver;
            this.kryptonBorderEdge3.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(166)))), ((int)(((byte)(69)))));
            this.panel1.Controls.Add(this.DefaultViewButton);
            this.panel1.Controls.Add(this.ShortViewButton);
            this.panel1.Controls.Add(this.OfflineButton);
            this.panel1.Controls.Add(this.OnlineButton);
            this.panel1.Controls.Add(this.AlphabeticalButton);
            this.panel1.Location = new System.Drawing.Point(369, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(874, 58);
            this.panel1.TabIndex = 215;
            // 
            // DefaultViewButton
            // 
            this.DefaultViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefaultViewButton.Checked = true;
            this.DefaultViewButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DefaultViewButton.Location = new System.Drawing.Point(814, 3);
            this.DefaultViewButton.Name = "DefaultViewButton";
            this.DefaultViewButton.OverrideDefault.Back.Color1 = System.Drawing.Color.Transparent;
            this.DefaultViewButton.OverrideDefault.Border.Color1 = System.Drawing.Color.White;
            this.DefaultViewButton.OverrideDefault.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.DefaultViewButton.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.DefaultViewButton.OverrideDefault.Border.Rounding = 0;
            this.DefaultViewButton.Size = new System.Drawing.Size(53, 52);
            this.DefaultViewButton.StateCheckedNormal.Back.Color1 = System.Drawing.Color.Transparent;
            this.DefaultViewButton.StateCheckedNormal.Border.Color1 = System.Drawing.Color.White;
            this.DefaultViewButton.StateCheckedNormal.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.DefaultViewButton.StateCheckedNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.DefaultViewButton.StateCheckedNormal.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DefaultViewButton.StateCheckedTracking.Back.Color1 = System.Drawing.Color.Transparent;
            this.DefaultViewButton.StateCheckedTracking.Border.Color1 = System.Drawing.Color.White;
            this.DefaultViewButton.StateCheckedTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.DefaultViewButton.StateCheckedTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.DefaultViewButton.StateCheckedTracking.Border.Rounding = 0;
            this.DefaultViewButton.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.DefaultViewButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DefaultViewButton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.DefaultViewButton.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DefaultViewButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.DefaultViewButton.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right;
            this.DefaultViewButton.StateCommon.Border.Rounding = 0;
            this.DefaultViewButton.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.DefaultViewButton.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DefaultViewButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DefaultViewButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.DefaultViewButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.DefaultViewButton.StateTracking.Back.Color1 = System.Drawing.Color.Transparent;
            this.DefaultViewButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DefaultViewButton.StateTracking.Border.Color1 = System.Drawing.Color.White;
            this.DefaultViewButton.StateTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.DefaultViewButton.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.DefaultViewButton.StateTracking.Border.Rounding = 0;
            this.DefaultViewButton.TabIndex = 223;
            this.DefaultViewButton.TabStop = false;
            this.DefaultViewButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("DefaultViewButton.Values.Image")));
            this.DefaultViewButton.Values.Text = "";
            // 
            // ShortViewButton
            // 
            this.ShortViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShortViewButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ShortViewButton.Location = new System.Drawing.Point(760, 3);
            this.ShortViewButton.Name = "ShortViewButton";
            this.ShortViewButton.OverrideDefault.Back.Color1 = System.Drawing.Color.Transparent;
            this.ShortViewButton.OverrideDefault.Border.Color1 = System.Drawing.Color.White;
            this.ShortViewButton.OverrideDefault.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.ShortViewButton.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ShortViewButton.OverrideDefault.Border.Rounding = 0;
            this.ShortViewButton.Size = new System.Drawing.Size(53, 52);
            this.ShortViewButton.StateCheckedNormal.Back.Color1 = System.Drawing.Color.Transparent;
            this.ShortViewButton.StateCheckedNormal.Border.Color1 = System.Drawing.Color.White;
            this.ShortViewButton.StateCheckedNormal.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.ShortViewButton.StateCheckedNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ShortViewButton.StateCheckedNormal.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ShortViewButton.StateCheckedTracking.Back.Color1 = System.Drawing.Color.Transparent;
            this.ShortViewButton.StateCheckedTracking.Border.Color1 = System.Drawing.Color.White;
            this.ShortViewButton.StateCheckedTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.ShortViewButton.StateCheckedTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ShortViewButton.StateCheckedTracking.Border.Rounding = 0;
            this.ShortViewButton.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.ShortViewButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ShortViewButton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.ShortViewButton.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ShortViewButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.ShortViewButton.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right;
            this.ShortViewButton.StateCommon.Border.Rounding = 0;
            this.ShortViewButton.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.ShortViewButton.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ShortViewButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ShortViewButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.ShortViewButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.ShortViewButton.StateTracking.Back.Color1 = System.Drawing.Color.Transparent;
            this.ShortViewButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ShortViewButton.StateTracking.Border.Color1 = System.Drawing.Color.White;
            this.ShortViewButton.StateTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.ShortViewButton.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ShortViewButton.StateTracking.Border.Rounding = 0;
            this.ShortViewButton.TabIndex = 222;
            this.ShortViewButton.TabStop = false;
            this.ShortViewButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("ShortViewButton.Values.Image")));
            this.ShortViewButton.Values.Text = "";
            // 
            // OfflineButton
            // 
            this.OfflineButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.OfflineButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OfflineButton.Location = new System.Drawing.Point(318, 0);
            this.OfflineButton.Name = "OfflineButton";
            this.OfflineButton.Size = new System.Drawing.Size(159, 58);
            this.OfflineButton.StateCheckedNormal.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.OfflineButton.StateCheckedNormal.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.OfflineButton.StateCheckedTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.OfflineButton.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.OfflineButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OfflineButton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.OfflineButton.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OfflineButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.OfflineButton.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right;
            this.OfflineButton.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.OfflineButton.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OfflineButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.OfflineButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OfflineButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OfflineButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.OfflineButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OfflineButton.TabIndex = 197;
            this.OfflineButton.TabStop = false;
            this.OfflineButton.Values.Text = "Оффлайн";
            // 
            // OnlineButton
            // 
            this.OnlineButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.OnlineButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OnlineButton.Location = new System.Drawing.Point(159, 0);
            this.OnlineButton.Name = "OnlineButton";
            this.OnlineButton.Size = new System.Drawing.Size(159, 58);
            this.OnlineButton.StateCheckedNormal.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.OnlineButton.StateCheckedNormal.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.OnlineButton.StateCheckedTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.OnlineButton.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.OnlineButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OnlineButton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.OnlineButton.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OnlineButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.OnlineButton.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right;
            this.OnlineButton.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.OnlineButton.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OnlineButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.OnlineButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OnlineButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OnlineButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.OnlineButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OnlineButton.TabIndex = 196;
            this.OnlineButton.TabStop = false;
            this.OnlineButton.Values.Text = "Онлайн";
            // 
            // AlphabeticalButton
            // 
            this.AlphabeticalButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.AlphabeticalButton.Checked = true;
            this.AlphabeticalButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AlphabeticalButton.Location = new System.Drawing.Point(0, 0);
            this.AlphabeticalButton.Name = "AlphabeticalButton";
            this.AlphabeticalButton.Size = new System.Drawing.Size(159, 58);
            this.AlphabeticalButton.StateCheckedNormal.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.AlphabeticalButton.StateCheckedNormal.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.AlphabeticalButton.StateCheckedTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.AlphabeticalButton.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.AlphabeticalButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AlphabeticalButton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.AlphabeticalButton.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AlphabeticalButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.AlphabeticalButton.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right;
            this.AlphabeticalButton.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.AlphabeticalButton.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AlphabeticalButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI Light", 19.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.AlphabeticalButton.StateCommon.Content.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AlphabeticalButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.AlphabeticalButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.AlphabeticalButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.AlphabeticalButton.TabIndex = 195;
            this.AlphabeticalButton.TabStop = false;
            this.AlphabeticalButton.Values.Text = "По алфавиту";
            // 
            // kryptonBorderEdge8
            // 
            this.kryptonBorderEdge8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonBorderEdge8.AutoSize = false;
            this.kryptonBorderEdge8.Location = new System.Drawing.Point(359, 72);
            this.kryptonBorderEdge8.Name = "kryptonBorderEdge8";
            this.kryptonBorderEdge8.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge8.Size = new System.Drawing.Size(4, 656);
            this.kryptonBorderEdge8.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(150)))), ((int)(((byte)(201)))));
            this.kryptonBorderEdge8.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge8.Text = "kryptonBorderEdge8";
            // 
            // kryptonCheckSet2
            // 
            this.kryptonCheckSet2.CheckButtons.Add(this.ShortViewButton);
            this.kryptonCheckSet2.CheckButtons.Add(this.DefaultViewButton);
            this.kryptonCheckSet2.CheckedButton = this.DefaultViewButton;
            this.kryptonCheckSet2.CheckedButtonChanged += new System.EventHandler(this.kryptonCheckSet2_CheckedButtonChanged);
            // 
            // kryptonCheckSet1
            // 
            this.kryptonCheckSet1.CheckButtons.Add(this.AlphabeticalButton);
            this.kryptonCheckSet1.CheckButtons.Add(this.OnlineButton);
            this.kryptonCheckSet1.CheckButtons.Add(this.OfflineButton);
            this.kryptonCheckSet1.CheckedButton = this.AlphabeticalButton;
            this.kryptonCheckSet1.CheckedButtonChanged += new System.EventHandler(this.kryptonCheckSet1_CheckedButtonChanged);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.kryptonBorderEdge10);
            this.panel3.Controls.Add(this.kryptonBorderEdge11);
            this.panel3.Controls.Add(this.kryptonBorderEdge12);
            this.panel3.Controls.Add(this.kryptonBorderEdge13);
            this.panel3.Controls.Add(this.DepartmentsDataGrid);
            this.panel3.Location = new System.Drawing.Point(12, 72);
            this.panel3.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(341, 656);
            this.panel3.TabIndex = 439;
            // 
            // kryptonBorderEdge10
            // 
            this.kryptonBorderEdge10.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge10.Location = new System.Drawing.Point(340, 1);
            this.kryptonBorderEdge10.Name = "kryptonBorderEdge10";
            this.kryptonBorderEdge10.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge10.Size = new System.Drawing.Size(1, 654);
            this.kryptonBorderEdge10.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge10.Text = "kryptonBorderEdge10";
            // 
            // kryptonBorderEdge11
            // 
            this.kryptonBorderEdge11.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge11.Location = new System.Drawing.Point(0, 1);
            this.kryptonBorderEdge11.Name = "kryptonBorderEdge11";
            this.kryptonBorderEdge11.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge11.Size = new System.Drawing.Size(1, 654);
            this.kryptonBorderEdge11.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge11.Text = "kryptonBorderEdge11";
            // 
            // kryptonBorderEdge12
            // 
            this.kryptonBorderEdge12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge12.Location = new System.Drawing.Point(0, 655);
            this.kryptonBorderEdge12.Name = "kryptonBorderEdge12";
            this.kryptonBorderEdge12.Size = new System.Drawing.Size(341, 1);
            this.kryptonBorderEdge12.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge12.Text = "kryptonBorderEdge12";
            // 
            // kryptonBorderEdge13
            // 
            this.kryptonBorderEdge13.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge13.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge13.Name = "kryptonBorderEdge13";
            this.kryptonBorderEdge13.Size = new System.Drawing.Size(341, 1);
            this.kryptonBorderEdge13.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge13.Text = "kryptonBorderEdge13";
            // 
            // DepartmentsDataGrid
            // 
            this.DepartmentsDataGrid.AllowUserToAddRows = false;
            this.DepartmentsDataGrid.AllowUserToDeleteRows = false;
            this.DepartmentsDataGrid.AllowUserToResizeColumns = false;
            this.DepartmentsDataGrid.AllowUserToResizeRows = false;
            this.DepartmentsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DepartmentsDataGrid.BackText = "Нет данных";
            this.DepartmentsDataGrid.ColumnHeadersHeight = 40;
            this.DepartmentsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DepartmentsDataGrid.ColumnHeadersVisible = false;
            this.DepartmentsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DepartmentsDataGrid.Location = new System.Drawing.Point(0, 0);
            this.DepartmentsDataGrid.MultiSelect = false;
            this.DepartmentsDataGrid.Name = "DepartmentsDataGrid";
            this.DepartmentsDataGrid.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.DepartmentsDataGrid.PercentLineWidth = 0;
            this.DepartmentsDataGrid.ReadOnly = true;
            this.DepartmentsDataGrid.RowHeadersVisible = false;
            this.DepartmentsDataGrid.RowTemplate.Height = 30;
            this.DepartmentsDataGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DepartmentsDataGrid.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Green;
            this.DepartmentsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DepartmentsDataGrid.Size = new System.Drawing.Size(341, 656);
            this.DepartmentsDataGrid.StandardStyle = false;
            this.DepartmentsDataGrid.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.DepartmentsDataGrid.StateCommon.Background.Color2 = System.Drawing.Color.Transparent;
            this.DepartmentsDataGrid.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DepartmentsDataGrid.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.DepartmentsDataGrid.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.DepartmentsDataGrid.StateCommon.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.DepartmentsDataGrid.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.DepartmentsDataGrid.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DepartmentsDataGrid.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.DepartmentsDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.DepartmentsDataGrid.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Back.Color2 = System.Drawing.Color.Transparent;
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Border.Width = 1;
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.DepartmentsDataGrid.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.DepartmentsDataGrid.StateDisabled.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DepartmentsDataGrid.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(158)))), ((int)(((byte)(0)))));
            this.DepartmentsDataGrid.StateSelected.DataCell.Back.Color2 = System.Drawing.Color.Transparent;
            this.DepartmentsDataGrid.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.DepartmentsDataGrid.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.DepartmentsDataGrid.TabIndex = 52;
            this.DepartmentsDataGrid.UseCustomBackColor = true;
            this.DepartmentsDataGrid.SelectionChanged += new System.EventHandler(this.DepartmentsDataGrid_SelectionChanged);
            // 
            // UsersList
            // 
            this.UsersList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UsersList.InfoItemColor = System.Drawing.Color.Gray;
            this.UsersList.InfoItemFont = new System.Drawing.Font("Segoe UI Semilight", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.UsersList.InfoLabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(124)))), ((int)(((byte)(174)))));
            this.UsersList.InfoLabelFont = new System.Drawing.Font("Segoe UI Semilight", 16.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.UsersList.InOfficeColor = System.Drawing.Color.Green;
            this.UsersList.InOfficeFont = new System.Drawing.Font("Segoe UI Semilight", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.UsersList.ItemLineColor = System.Drawing.Color.DarkGray;
            this.UsersList.Location = new System.Drawing.Point(369, 139);
            this.UsersList.Name = "UsersList";
            this.UsersList.PositionItemColor = System.Drawing.Color.Gray;
            this.UsersList.PositionItemFont = new System.Drawing.Font("Segoe UI Semilight", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.UsersList.PositionLabelColor = System.Drawing.Color.Black;
            this.UsersList.PositionLabelFont = new System.Drawing.Font("Segoe UI Semilight", 16.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.UsersList.ShortView = true;
            this.UsersList.Size = new System.Drawing.Size(874, 589);
            this.UsersList.TabIndex = 217;
            this.UsersList.Text = "lightUsersList1";
            this.UsersList.UserLineColor = System.Drawing.Color.LightGray;
            this.UsersList.UserNameColor = System.Drawing.Color.Black;
            this.UsersList.UserNameFont = new System.Drawing.Font("Segoe UI", 22.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.UsersList.UsersDataTable = null;
            this.UsersList.VerticalScrollCommonShaftBackColor = System.Drawing.Color.White;
            this.UsersList.VerticalScrollCommonThumbButtonColor = System.Drawing.Color.Gray;
            this.UsersList.UserClick += new Infinium.LightUsersList.UserClickEventHandler(this.UsersList_UserClick);
            // 
            // UsersForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1270, 740);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.kryptonBorderEdge8);
            this.Controls.Add(this.UsersList);
            this.Controls.Add(this.NavigatePanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UsersForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.UsersForm_Shown);
            this.NavigatePanel.ResumeLayout(false);
            this.NavigatePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DepartmentsDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonButton NavigateMenuCloseButton;
        private System.Windows.Forms.Panel NavigatePanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette NavigateMenuButtonsPalette;
        private System.Windows.Forms.Panel panel1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge8;
        private LightUsersList UsersList;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckSet kryptonCheckSet1;
        private System.Windows.Forms.Label label1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckSet kryptonCheckSet2;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton AlphabeticalButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton ShortViewButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton OfflineButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton OnlineButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton DefaultViewButton;
        private System.Windows.Forms.Panel panel3;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge10;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge11;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge12;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge13;
        private PercentageDataGrid DepartmentsDataGrid;
    }
}