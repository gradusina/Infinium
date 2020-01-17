namespace Infinium
{
    partial class SplitOrdersForm
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
            this.ProductCountTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.EqualPanel = new System.Windows.Forms.Panel();
            this.EqualCountTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.NotEqualOrdersButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.PanelSelectPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.EqualOrdersButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.kryptonCheckSet1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckSet(this.components);
            this.CancelPackButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.StandardButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.OKButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge4 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge5 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.NotEqualPanel.SuspendLayout();
            this.EqualPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // NotEqualPanel
            // 
            this.NotEqualPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NotEqualPanel.BackColor = System.Drawing.Color.White;
            this.NotEqualPanel.Controls.Add(this.ProductCountTextBox);
            this.NotEqualPanel.Controls.Add(this.kryptonLabel1);
            this.NotEqualPanel.Location = new System.Drawing.Point(1, 40);
            this.NotEqualPanel.Name = "NotEqualPanel";
            this.NotEqualPanel.Size = new System.Drawing.Size(370, 87);
            this.NotEqualPanel.TabIndex = 87;
            // 
            // ProductCountTextBox
            // 
            this.ProductCountTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ProductCountTextBox.Location = new System.Drawing.Point(105, 46);
            this.ProductCountTextBox.Name = "ProductCountTextBox";
            this.ProductCountTextBox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.ProductCountTextBox.Size = new System.Drawing.Size(160, 36);
            this.ProductCountTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProductCountTextBox.TabIndex = 69;
            this.ProductCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(80, 11);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.kryptonLabel1.Size = new System.Drawing.Size(264, 32);
            this.kryptonLabel1.StateCommon.ShortText.Color1 = System.Drawing.Color.Black;
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.kryptonLabel1.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.kryptonLabel1.TabIndex = 68;
            this.kryptonLabel1.Values.Text = "Введите кол-во элементов";
            // 
            // EqualPanel
            // 
            this.EqualPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EqualPanel.BackColor = System.Drawing.Color.White;
            this.EqualPanel.Controls.Add(this.EqualCountTextBox);
            this.EqualPanel.Controls.Add(this.kryptonLabel2);
            this.EqualPanel.Location = new System.Drawing.Point(1, 40);
            this.EqualPanel.Name = "EqualPanel";
            this.EqualPanel.Size = new System.Drawing.Size(370, 87);
            this.EqualPanel.TabIndex = 88;
            // 
            // EqualCountTextBox
            // 
            this.EqualCountTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.EqualCountTextBox.Location = new System.Drawing.Point(105, 46);
            this.EqualCountTextBox.Name = "EqualCountTextBox";
            this.EqualCountTextBox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.EqualCountTextBox.Size = new System.Drawing.Size(160, 28);
            this.EqualCountTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 17.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.EqualCountTextBox.TabIndex = 69;
            this.EqualCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(62, 11);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
            this.kryptonLabel2.Size = new System.Drawing.Size(233, 24);
            this.kryptonLabel2.StateCommon.ShortText.Color1 = System.Drawing.Color.Black;
            this.kryptonLabel2.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.kryptonLabel2.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.kryptonLabel2.TabIndex = 68;
            this.kryptonLabel2.Values.Text = "Введите кол-во в одном заказе";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.NotEqualOrdersButton);
            this.panel3.Controls.Add(this.kryptonBorderEdge2);
            this.panel3.Controls.Add(this.EqualOrdersButton);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(373, 40);
            this.panel3.TabIndex = 89;
            // 
            // NotEqualOrdersButton
            // 
            this.NotEqualOrdersButton.Location = new System.Drawing.Point(189, 5);
            this.NotEqualOrdersButton.Name = "NotEqualOrdersButton";
            this.NotEqualOrdersButton.Palette = this.PanelSelectPalette;
            this.NotEqualOrdersButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.NotEqualOrdersButton.Size = new System.Drawing.Size(176, 31);
            this.NotEqualOrdersButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.NotEqualOrdersButton.TabIndex = 1;
            this.NotEqualOrdersButton.Values.Text = "Неравные заказы";
            this.NotEqualOrdersButton.Click += new System.EventHandler(this.NotEqualOrdersButton_Click);
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
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(0, 39);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(373, 1);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.Silver;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // EqualOrdersButton
            // 
            this.EqualOrdersButton.Checked = true;
            this.EqualOrdersButton.Location = new System.Drawing.Point(7, 5);
            this.EqualOrdersButton.Name = "EqualOrdersButton";
            this.EqualOrdersButton.Palette = this.PanelSelectPalette;
            this.EqualOrdersButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.EqualOrdersButton.Size = new System.Drawing.Size(176, 31);
            this.EqualOrdersButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.EqualOrdersButton.TabIndex = 0;
            this.EqualOrdersButton.Values.Text = "Равные заказы";
            this.EqualOrdersButton.Click += new System.EventHandler(this.EqualOrdersButton_Click);
            // 
            // kryptonCheckSet1
            // 
            this.kryptonCheckSet1.CheckButtons.Add(this.NotEqualOrdersButton);
            this.kryptonCheckSet1.CheckButtons.Add(this.EqualOrdersButton);
            this.kryptonCheckSet1.CheckedButton = this.EqualOrdersButton;
            // 
            // CancelPackButton
            // 
            this.CancelPackButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelPackButton.Location = new System.Drawing.Point(189, 141);
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
            this.CancelPackButton.TabIndex = 113;
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
            this.OKButton.Location = new System.Drawing.Point(16, 141);
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
            this.OKButton.TabIndex = 112;
            this.OKButton.TabStop = false;
            this.OKButton.Values.Text = "ОК";
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.kryptonBorderEdge3);
            this.panel1.Controls.Add(this.kryptonBorderEdge4);
            this.panel1.Controls.Add(this.kryptonBorderEdge5);
            this.panel1.Controls.Add(this.kryptonBorderEdge1);
            this.panel1.Controls.Add(this.EqualPanel);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.CancelPackButton);
            this.panel1.Controls.Add(this.OKButton);
            this.panel1.Controls.Add(this.NotEqualPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(373, 204);
            this.panel1.TabIndex = 114;
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(0, 1);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(1, 202);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // kryptonBorderEdge4
            // 
            this.kryptonBorderEdge4.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge4.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge4.Name = "kryptonBorderEdge4";
            this.kryptonBorderEdge4.Size = new System.Drawing.Size(372, 1);
            this.kryptonBorderEdge4.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge4.Text = "kryptonBorderEdge4";
            // 
            // kryptonBorderEdge5
            // 
            this.kryptonBorderEdge5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge5.Location = new System.Drawing.Point(0, 203);
            this.kryptonBorderEdge5.Name = "kryptonBorderEdge5";
            this.kryptonBorderEdge5.Size = new System.Drawing.Size(372, 1);
            this.kryptonBorderEdge5.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge5.Text = "kryptonBorderEdge5";
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(372, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(1, 204);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // SplitOrdersForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(373, 204);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplitOrdersForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MarketingSplitPackagesForm_FormClosing);
            this.Shown += new System.EventHandler(this.SplitPackagesForm_Shown);
            this.NotEqualPanel.ResumeLayout(false);
            this.NotEqualPanel.PerformLayout();
            this.EqualPanel.ResumeLayout(false);
            this.EqualPanel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private System.Windows.Forms.Panel NotEqualPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox ProductCountTextBox;
        private System.Windows.Forms.Panel EqualPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox EqualCountTextBox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private System.Windows.Forms.Panel panel3;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton NotEqualOrdersButton;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton EqualOrdersButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckSet kryptonCheckSet1;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette PanelSelectPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonButton CancelPackButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton OKButton;
        private System.Windows.Forms.Panel panel1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge4;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge5;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette StandardButtonsPalette;
    }
}