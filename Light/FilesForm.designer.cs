namespace Infinium
{
    partial class FilesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilesForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.NavigateMenuButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.ButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.PasswordButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.CheckMultipleButton = new InfiniumLightButton();
            this.UncheckMultipleButton = new InfiniumLightButton();
            this.UpdatePanel = new System.Windows.Forms.Panel();
            this.SendMailButton = new InfiniumLightButton();
            this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.kryptonBorderEdge6 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.InfiniumDocumentAttributesView = new InfiniumFilesAttributesView();
            this.DocumentsPermissionsUsersList = new InfiniumFilesPermissionsUsersList();
            this.kryptonBorderEdge5 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.LoadWithSampleButton = new InfiniumLightButton();
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.kryptonBorderEdge4 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.HeaderLastModifiedLabel = new System.Windows.Forms.Label();
            this.HeaderCreateLabel = new System.Windows.Forms.Label();
            this.UploadFileIButton = new InfiniumLightButton();
            this.RemoveButton = new InfiniumLightButton();
            this.AddFolderButton = new InfiniumLightButton();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.InfiniumFileList = new InfiniumFileList();
            this.panel1 = new System.Windows.Forms.Panel();
            this.InfiniumDocumentsMenu = new InfiniumFilesMenu();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.NavigatePanel = new System.Windows.Forms.Panel();
            this.MinimizeButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.NavigateMenuCloseButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.label1 = new System.Windows.Forms.Label();
            this.UploadFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FileContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.MenuFileOpenFile = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.MenuFileSaveFile = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.MenuFileReplaceFile = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.MenuFileDeleteFile = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.ContextMenuPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.kryptonContextMenuItem3 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.UpdatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
            this.kryptonSplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
            this.kryptonSplitContainer1.Panel2.SuspendLayout();
            this.kryptonSplitContainer1.SuspendLayout();
            this.HeaderPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.NavigatePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
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
            // CheckMultipleButton
            // 
            this.CheckMultipleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckMultipleButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.CheckMultipleButton.Caption = " ";
            this.CheckMultipleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CheckMultipleButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CheckMultipleButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.CheckMultipleButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("CheckMultipleButton.Image")));
            this.CheckMultipleButton.Location = new System.Drawing.Point(945, 6);
            this.CheckMultipleButton.Name = "CheckMultipleButton";
            this.CheckMultipleButton.Size = new System.Drawing.Size(52, 48);
            this.CheckMultipleButton.TabIndex = 336;
            this.CheckMultipleButton.Text = "infiniumLightButton1";
            this.CheckMultipleButton.TextYMargin = 0;
            this.toolTip1.SetToolTip(this.CheckMultipleButton, "Отметить несколько");
            this.CheckMultipleButton.Visible = false;
            this.CheckMultipleButton.Click += new System.EventHandler(this.CheckMultipleButton_Click);
            // 
            // UncheckMultipleButton
            // 
            this.UncheckMultipleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UncheckMultipleButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.UncheckMultipleButton.Caption = " ";
            this.UncheckMultipleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UncheckMultipleButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.UncheckMultipleButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.UncheckMultipleButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("UncheckMultipleButton.Image")));
            this.UncheckMultipleButton.Location = new System.Drawing.Point(945, 6);
            this.UncheckMultipleButton.Name = "UncheckMultipleButton";
            this.UncheckMultipleButton.Size = new System.Drawing.Size(52, 48);
            this.UncheckMultipleButton.TabIndex = 335;
            this.UncheckMultipleButton.Text = "infiniumLightButton1";
            this.UncheckMultipleButton.TextYMargin = 0;
            this.toolTip1.SetToolTip(this.UncheckMultipleButton, "Отметить несколько");
            this.UncheckMultipleButton.Visible = false;
            this.UncheckMultipleButton.Click += new System.EventHandler(this.UncheckMultipleButton_Click);
            // 
            // UpdatePanel
            // 
            this.UpdatePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdatePanel.Controls.Add(this.SendMailButton);
            this.UpdatePanel.Controls.Add(this.kryptonSplitContainer1);
            this.UpdatePanel.Controls.Add(this.kryptonBorderEdge5);
            this.UpdatePanel.Controls.Add(this.LoadWithSampleButton);
            this.UpdatePanel.Controls.Add(this.HeaderPanel);
            this.UpdatePanel.Controls.Add(this.CheckMultipleButton);
            this.UpdatePanel.Controls.Add(this.UncheckMultipleButton);
            this.UpdatePanel.Controls.Add(this.UploadFileIButton);
            this.UpdatePanel.Controls.Add(this.RemoveButton);
            this.UpdatePanel.Controls.Add(this.AddFolderButton);
            this.UpdatePanel.Controls.Add(this.kryptonBorderEdge2);
            this.UpdatePanel.Controls.Add(this.InfiniumFileList);
            this.UpdatePanel.Location = new System.Drawing.Point(237, 115);
            this.UpdatePanel.Name = "UpdatePanel";
            this.UpdatePanel.Size = new System.Drawing.Size(1005, 594);
            this.UpdatePanel.TabIndex = 313;
            this.UpdatePanel.Tag = "5";
            // 
            // SendMailButton
            // 
            this.SendMailButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.SendMailButton.Caption = "Уведомить";
            this.SendMailButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SendMailButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.SendMailButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.SendMailButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("SendMailButton.Image")));
            this.SendMailButton.Location = new System.Drawing.Point(648, 6);
            this.SendMailButton.Name = "SendMailButton";
            this.SendMailButton.Size = new System.Drawing.Size(128, 48);
            this.SendMailButton.TabIndex = 348;
            this.SendMailButton.Text = "infiniumLightButton1";
            this.SendMailButton.TextYMargin = 0;
            this.SendMailButton.Visible = false;
            this.SendMailButton.Click += new System.EventHandler(this.SendMailButton_Click);
            // 
            // kryptonSplitContainer1
            // 
            this.kryptonSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.kryptonSplitContainer1.IsSplitterFixed = true;
            this.kryptonSplitContainer1.Location = new System.Drawing.Point(689, 67);
            this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
            this.kryptonSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // kryptonSplitContainer1.Panel1
            // 
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.kryptonBorderEdge6);
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.InfiniumDocumentAttributesView);
            this.kryptonSplitContainer1.Panel1.StateCommon.Color1 = System.Drawing.Color.Transparent;
            // 
            // kryptonSplitContainer1.Panel2
            // 
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.DocumentsPermissionsUsersList);
            this.kryptonSplitContainer1.Panel2.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonSplitContainer1.Size = new System.Drawing.Size(308, 514);
            this.kryptonSplitContainer1.SplitterDistance = 246;
            this.kryptonSplitContainer1.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this.kryptonSplitContainer1.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonSplitContainer1.TabIndex = 345;
            // 
            // kryptonBorderEdge6
            // 
            this.kryptonBorderEdge6.AutoSize = false;
            this.kryptonBorderEdge6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge6.Location = new System.Drawing.Point(0, 245);
            this.kryptonBorderEdge6.Name = "kryptonBorderEdge6";
            this.kryptonBorderEdge6.Size = new System.Drawing.Size(308, 1);
            this.kryptonBorderEdge6.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge6.StateCommon.Color2 = System.Drawing.Color.Gainsboro;
            this.kryptonBorderEdge6.StateCommon.ColorAngle = 0F;
            this.kryptonBorderEdge6.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Sigma;
            this.kryptonBorderEdge6.Text = "kryptonBorderEdge6";
            // 
            // InfiniumDocumentAttributesView
            // 
            this.InfiniumDocumentAttributesView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InfiniumDocumentAttributesView.AttributesDataTable = null;
            this.InfiniumDocumentAttributesView.Location = new System.Drawing.Point(0, 0);
            this.InfiniumDocumentAttributesView.Name = "InfiniumDocumentAttributesView";
            this.InfiniumDocumentAttributesView.Size = new System.Drawing.Size(308, 245);
            this.InfiniumDocumentAttributesView.TabIndex = 342;
            this.InfiniumDocumentAttributesView.Text = "infiniumDocumentsSampleView1";
            // 
            // 
            // 
            this.InfiniumDocumentAttributesView.VerticalScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InfiniumDocumentAttributesView.VerticalScrollBar.Location = new System.Drawing.Point(296, 0);
            this.InfiniumDocumentAttributesView.VerticalScrollBar.Name = "";
            this.InfiniumDocumentAttributesView.VerticalScrollBar.Offset = 0;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.ScrollWheelOffset = 30;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.Size = new System.Drawing.Size(12, 245);
            this.InfiniumDocumentAttributesView.VerticalScrollBar.TabIndex = 0;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.TotalControlHeight = 245;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.VerticalScrollCommonShaftBackColor = System.Drawing.Color.LightBlue;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.VerticalScrollCommonThumbButtonColor = System.Drawing.Color.DarkGray;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.VerticalScrollTrackingShaftBackColor = System.Drawing.Color.LightBlue;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.VerticalScrollTrackingThumbButtonColor = System.Drawing.Color.Gray;
            this.InfiniumDocumentAttributesView.VerticalScrollBar.Visible = false;
            this.InfiniumDocumentAttributesView.SignButtonClicked += new InfiniumFilesAttributesView.OnSignButtonClickedEventHandler(this.InfiniumDocumentAttributesView_SignButtonClicked);
            this.InfiniumDocumentAttributesView.ReadButtonClicked += new InfiniumFilesAttributesView.OnSignButtonClickedEventHandler(this.InfiniumDocumentAttributesView_ReadButtonClicked);
            // 
            // DocumentsPermissionsUsersList
            // 
            this.DocumentsPermissionsUsersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DocumentsPermissionsUsersList.Location = new System.Drawing.Point(0, 0);
            this.DocumentsPermissionsUsersList.Name = "DocumentsPermissionsUsersList";
            this.DocumentsPermissionsUsersList.Size = new System.Drawing.Size(308, 263);
            this.DocumentsPermissionsUsersList.TabIndex = 0;
            this.DocumentsPermissionsUsersList.Text = "infiniumDocumentsPermissionsUsersList1";
            // 
            // 
            // 
            this.DocumentsPermissionsUsersList.VerticalScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentsPermissionsUsersList.VerticalScrollBar.Location = new System.Drawing.Point(296, 0);
            this.DocumentsPermissionsUsersList.VerticalScrollBar.Name = "";
            this.DocumentsPermissionsUsersList.VerticalScrollBar.Offset = 0;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.ScrollWheelOffset = 30;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.Size = new System.Drawing.Size(12, 263);
            this.DocumentsPermissionsUsersList.VerticalScrollBar.TabIndex = 0;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.TotalControlHeight = 263;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.VerticalScrollCommonShaftBackColor = System.Drawing.Color.LightBlue;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.VerticalScrollCommonThumbButtonColor = System.Drawing.Color.DarkGray;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.VerticalScrollTrackingShaftBackColor = System.Drawing.Color.LightBlue;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.VerticalScrollTrackingThumbButtonColor = System.Drawing.Color.Gray;
            this.DocumentsPermissionsUsersList.VerticalScrollBar.Visible = false;
            // 
            // kryptonBorderEdge5
            // 
            this.kryptonBorderEdge5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonBorderEdge5.AutoSize = false;
            this.kryptonBorderEdge5.Location = new System.Drawing.Point(682, 61);
            this.kryptonBorderEdge5.Name = "kryptonBorderEdge5";
            this.kryptonBorderEdge5.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge5.Size = new System.Drawing.Size(1, 532);
            this.kryptonBorderEdge5.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.kryptonBorderEdge5.Text = "kryptonBorderEdge5";
            // 
            // LoadWithSampleButton
            // 
            this.LoadWithSampleButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.LoadWithSampleButton.Caption = "Создать документ";
            this.LoadWithSampleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoadWithSampleButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LoadWithSampleButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.LoadWithSampleButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("LoadWithSampleButton.Image")));
            this.LoadWithSampleButton.Location = new System.Drawing.Point(11, 6);
            this.LoadWithSampleButton.Name = "LoadWithSampleButton";
            this.LoadWithSampleButton.Size = new System.Drawing.Size(179, 48);
            this.LoadWithSampleButton.TabIndex = 340;
            this.LoadWithSampleButton.Text = "Создать документ";
            this.LoadWithSampleButton.TextYMargin = 0;
            this.LoadWithSampleButton.Click += new System.EventHandler(this.LoadWithSampleButton_Click);
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HeaderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HeaderPanel.Controls.Add(this.kryptonBorderEdge4);
            this.HeaderPanel.Controls.Add(this.HeaderLastModifiedLabel);
            this.HeaderPanel.Controls.Add(this.HeaderCreateLabel);
            this.HeaderPanel.Location = new System.Drawing.Point(11, 62);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(669, 35);
            this.HeaderPanel.TabIndex = 338;
            // 
            // kryptonBorderEdge4
            // 
            this.kryptonBorderEdge4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge4.Location = new System.Drawing.Point(0, 34);
            this.kryptonBorderEdge4.Name = "kryptonBorderEdge4";
            this.kryptonBorderEdge4.Size = new System.Drawing.Size(669, 1);
            this.kryptonBorderEdge4.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.kryptonBorderEdge4.Text = "kryptonBorderEdge4";
            // 
            // HeaderLastModifiedLabel
            // 
            this.HeaderLastModifiedLabel.AutoSize = true;
            this.HeaderLastModifiedLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.HeaderLastModifiedLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.HeaderLastModifiedLabel.Location = new System.Drawing.Point(479, 13);
            this.HeaderLastModifiedLabel.Name = "HeaderLastModifiedLabel";
            this.HeaderLastModifiedLabel.Size = new System.Drawing.Size(167, 20);
            this.HeaderLastModifiedLabel.TabIndex = 1;
            this.HeaderLastModifiedLabel.Text = "Последнее изменение";
            // 
            // HeaderCreateLabel
            // 
            this.HeaderCreateLabel.AutoSize = true;
            this.HeaderCreateLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.HeaderCreateLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.HeaderCreateLabel.Location = new System.Drawing.Point(352, 13);
            this.HeaderCreateLabel.Name = "HeaderCreateLabel";
            this.HeaderCreateLabel.Size = new System.Drawing.Size(59, 20);
            this.HeaderCreateLabel.TabIndex = 0;
            this.HeaderCreateLabel.Text = "Создан";
            // 
            // UploadFileIButton
            // 
            this.UploadFileIButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.UploadFileIButton.Caption = "Загрузить файл";
            this.UploadFileIButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UploadFileIButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.UploadFileIButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.UploadFileIButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("UploadFileIButton.Image")));
            this.UploadFileIButton.Location = new System.Drawing.Point(198, 6);
            this.UploadFileIButton.Name = "UploadFileIButton";
            this.UploadFileIButton.Size = new System.Drawing.Size(160, 48);
            this.UploadFileIButton.TabIndex = 333;
            this.UploadFileIButton.Text = "infiniumLightButton1";
            this.UploadFileIButton.TextYMargin = 0;
            this.UploadFileIButton.Click += new System.EventHandler(this.UploadFileIButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.RemoveButton.Caption = "Удалить";
            this.RemoveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RemoveButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.RemoveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.RemoveButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("RemoveButton.Image")));
            this.RemoveButton.Location = new System.Drawing.Point(529, 6);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(108, 48);
            this.RemoveButton.TabIndex = 331;
            this.RemoveButton.Text = "infiniumLightButton1";
            this.RemoveButton.TextYMargin = 0;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // AddFolderButton
            // 
            this.AddFolderButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.AddFolderButton.Caption = "Создать папку";
            this.AddFolderButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AddFolderButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.AddFolderButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.AddFolderButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("AddFolderButton.Image")));
            this.AddFolderButton.Location = new System.Drawing.Point(367, 6);
            this.AddFolderButton.Name = "AddFolderButton";
            this.AddFolderButton.Size = new System.Drawing.Size(153, 48);
            this.AddFolderButton.TabIndex = 329;
            this.AddFolderButton.Text = "infiniumLightButton1";
            this.AddFolderButton.TextYMargin = 0;
            this.AddFolderButton.Click += new System.EventHandler(this.AddFolderButton_Click);
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonBorderEdge2.AutoSize = false;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(11, 60);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(982, 1);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.Gainsboro;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // InfiniumFileList
            // 
            this.InfiniumFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InfiniumFileList.CheckVisible = false;
            this.InfiniumFileList.Entered = -1;
            this.InfiniumFileList.ItemsDataTable = null;
            this.InfiniumFileList.Location = new System.Drawing.Point(11, 98);
            this.InfiniumFileList.Name = "InfiniumFileList";
            this.InfiniumFileList.Selected = -1;
            this.InfiniumFileList.Size = new System.Drawing.Size(669, 483);
            this.InfiniumFileList.TabIndex = 0;
            this.InfiniumFileList.Text = "z";
            // 
            // 
            // 
            this.InfiniumFileList.VerticalScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.InfiniumFileList.VerticalScrollBar.Location = new System.Drawing.Point(657, 0);
            this.InfiniumFileList.VerticalScrollBar.Name = "";
            this.InfiniumFileList.VerticalScrollBar.Offset = 0;
            this.InfiniumFileList.VerticalScrollBar.ScrollWheelOffset = 30;
            this.InfiniumFileList.VerticalScrollBar.Size = new System.Drawing.Size(12, 483);
            this.InfiniumFileList.VerticalScrollBar.TabIndex = 0;
            this.InfiniumFileList.VerticalScrollBar.TotalControlHeight = 483;
            this.InfiniumFileList.VerticalScrollBar.VerticalScrollCommonShaftBackColor = System.Drawing.Color.LightBlue;
            this.InfiniumFileList.VerticalScrollBar.VerticalScrollCommonThumbButtonColor = System.Drawing.Color.DarkGray;
            this.InfiniumFileList.VerticalScrollBar.VerticalScrollTrackingShaftBackColor = System.Drawing.Color.LightBlue;
            this.InfiniumFileList.VerticalScrollBar.VerticalScrollTrackingThumbButtonColor = System.Drawing.Color.Gray;
            this.InfiniumFileList.VerticalScrollBar.Visible = false;
            this.InfiniumFileList.SelectedChanged += new InfiniumFileList.ItemRightClickedEventHandler(this.InfiniumFileList_SelectedChanged);
            this.InfiniumFileList.ItemRightClicked += new InfiniumFileList.ItemRightClickedEventHandler(this.InfiniumFileList_ItemRightClicked);
            this.InfiniumFileList.ItemDoubleClick += new InfiniumFileList.ItemDoubleClickedEventHandler(this.InfiniumFileList_ItemDoubleClick);
            this.InfiniumFileList.RootDoubleClick += new InfiniumFileList.ItemDoubleClickedEventHandler(this.InfiniumFileList_RootDoubleClick);
            this.InfiniumFileList.FolderEntered += new InfiniumFileList.ItemDoubleClickedEventHandler(this.InfiniumFileList_FolderEntered);
            this.InfiniumFileList.SizeChanged += new System.EventHandler(this.InfiniumFileList_SizeChanged);
            this.InfiniumFileList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.InfiniumFileList_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.InfiniumDocumentsMenu);
            this.panel1.Controls.Add(this.kryptonBorderEdge1);
            this.panel1.Location = new System.Drawing.Point(29, 115);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(208, 594);
            this.panel1.TabIndex = 296;
            // 
            // InfiniumDocumentsMenu
            // 
            this.InfiniumDocumentsMenu.BackColor = System.Drawing.Color.Transparent;
            this.InfiniumDocumentsMenu.ItemsDataTable = null;
            this.InfiniumDocumentsMenu.Location = new System.Drawing.Point(11, 13);
            this.InfiniumDocumentsMenu.Name = "InfiniumDocumentsMenu";
            this.InfiniumDocumentsMenu.Selected = -1;
            this.InfiniumDocumentsMenu.Size = new System.Drawing.Size(188, 438);
            this.InfiniumDocumentsMenu.TabIndex = 16;
            this.InfiniumDocumentsMenu.Text = "infiniumDocumentsMenu1";
            // 
            // 
            // 
            this.InfiniumDocumentsMenu.VerticalScrollBar.Location = new System.Drawing.Point(188, 0);
            this.InfiniumDocumentsMenu.VerticalScrollBar.Name = "";
            this.InfiniumDocumentsMenu.VerticalScrollBar.Offset = 0;
            this.InfiniumDocumentsMenu.VerticalScrollBar.ScrollWheelOffset = 30;
            this.InfiniumDocumentsMenu.VerticalScrollBar.TabIndex = 0;
            this.InfiniumDocumentsMenu.VerticalScrollBar.TotalControlHeight = 438;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollCommonShaftBackColor = System.Drawing.Color.LightBlue;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollCommonThumbButtonColor = System.Drawing.Color.DarkGray;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollTrackingShaftBackColor = System.Drawing.Color.Blue;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollTrackingThumbButtonColor = System.Drawing.Color.Gray;
            this.InfiniumDocumentsMenu.ItemClicked += new InfiniumFilesMenu.ItemClickedEventHandler(this.InfiniumDocumentsMenu_ItemClicked);
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonBorderEdge1.AutoSize = false;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(205, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(1, 594);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.Gainsboro;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(29, 68);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1213, 46);
            this.panel2.TabIndex = 293;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(770, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_6);
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
            this.NavigatePanel.Margin = new System.Windows.Forms.Padding(4);
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
            this.MinimizeButton.TabIndex = 35;
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
            this.kryptonBorderEdge3.Margin = new System.Windows.Forms.Padding(4);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(1270, 1);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.Silver;
            this.kryptonBorderEdge3.StateCommon.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 32.81F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(452, 45);
            this.label1.TabIndex = 31;
            this.label1.Text = "Infinium. Файловый менеджер";
            // 
            // UploadFileDialog
            // 
            this.UploadFileDialog.Multiselect = true;
            // 
            // FileContextMenu
            // 
            this.FileContextMenu.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItems1});
            this.FileContextMenu.Palette = this.ContextMenuPalette;
            this.FileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.SaveFileMenu_Opening);
            // 
            // kryptonContextMenuItems1
            // 
            this.kryptonContextMenuItems1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.MenuFileOpenFile,
            this.MenuFileSaveFile,
            this.MenuFileReplaceFile,
            this.MenuFileDeleteFile});
            // 
            // MenuFileOpenFile
            // 
            this.MenuFileOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileOpenFile.Image")));
            this.MenuFileOpenFile.Text = "Открыть";
            this.MenuFileOpenFile.Click += new System.EventHandler(this.MenuFileOpenFile_Click);
            // 
            // MenuFileSaveFile
            // 
            this.MenuFileSaveFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileSaveFile.Image")));
            this.MenuFileSaveFile.Text = "Сохранить";
            this.MenuFileSaveFile.Click += new System.EventHandler(this.MenuFileSaveFile_Click);
            // 
            // MenuFileReplaceFile
            // 
            this.MenuFileReplaceFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileReplaceFile.Image")));
            this.MenuFileReplaceFile.Text = "Заменить";
            this.MenuFileReplaceFile.Click += new System.EventHandler(this.MenuFileReplaceFile_Click);
            // 
            // MenuFileDeleteFile
            // 
            this.MenuFileDeleteFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileDeleteFile.Image")));
            this.MenuFileDeleteFile.Text = "Удалить";
            this.MenuFileDeleteFile.Click += new System.EventHandler(this.MenuFileDeleteFile_Click);
            // 
            // ContextMenuPalette
            // 
            this.ContextMenuPalette.ContextMenu.StateCommon.ControlOuter.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ContextMenuPalette.ContextMenu.StateCommon.ControlOuter.Border.Rounding = 0;
            this.ContextMenuPalette.ContextMenu.StateCommon.ItemHighlight.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.ContextMenuPalette.ContextMenu.StateCommon.ItemHighlight.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.ContextMenuPalette.ContextMenu.StateCommon.ItemHighlight.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.ContextMenuPalette.ContextMenu.StateCommon.ItemHighlight.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ContextMenuPalette.ContextMenu.StateCommon.ItemHighlight.Border.Rounding = 0;
            this.ContextMenuPalette.ContextMenu.StateCommon.ItemImageColumn.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.ContextMenuPalette.ContextMenu.StateCommon.ItemImageColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ContextMenuPalette.ContextMenu.StateNormal.ItemTextStandard.LongText.Color1 = System.Drawing.Color.Black;
            this.ContextMenuPalette.ContextMenu.StateNormal.ItemTextStandard.LongText.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ContextMenuPalette.ContextMenu.StateNormal.ItemTextStandard.Padding = new System.Windows.Forms.Padding(-1, 6, -1, 6);
            // 
            // kryptonContextMenuItem3
            // 
            this.kryptonContextMenuItem3.Text = "Menu Item";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Multiselect = true;
            // 
            // FilesForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1270, 720);
            this.Controls.Add(this.UpdatePanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.NavigatePanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilesForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ANSUpdate += new Infinium.InfiniumForm.ANSUpdateEventHandler(this.ProjectsForm_ANSUpdate);
            this.Shown += new System.EventHandler(this.LightNewsForm_Shown);
            this.UpdatePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
            this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
            this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
            this.kryptonSplitContainer1.ResumeLayout(false);
            this.HeaderPanel.ResumeLayout(false);
            this.HeaderPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.NavigatePanel.ResumeLayout(false);
            this.NavigatePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonButton NavigateMenuCloseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel NavigatePanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette ButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette NavigateMenuButtonsPalette;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton PasswordButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private System.Windows.Forms.Panel UpdatePanel;
        private System.Windows.Forms.ToolTip toolTip1;
        private InfiniumFileList InfiniumFileList;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private InfiniumLightButton AddFolderButton;
        private InfiniumLightButton RemoveButton;
        private InfiniumLightButton UploadFileIButton;
        private System.Windows.Forms.OpenFileDialog UploadFileDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu FileContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileOpenFile;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileSaveFile;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem3;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette ContextMenuPalette;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileReplaceFile;
        private InfiniumLightButton CheckMultipleButton;
        private InfiniumLightButton UncheckMultipleButton;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileDeleteFile;
        private System.Windows.Forms.Panel HeaderPanel;
        private System.Windows.Forms.Label HeaderLastModifiedLabel;
        private System.Windows.Forms.Label HeaderCreateLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge4;
        private System.Windows.Forms.Timer timer1;
        private InfiniumLightButton LoadWithSampleButton;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge5;
        private InfiniumFilesAttributesView InfiniumDocumentAttributesView;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
        private InfiniumFilesPermissionsUsersList DocumentsPermissionsUsersList;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge6;
        private ComponentFactory.Krypton.Toolkit.KryptonButton MinimizeButton;
        private InfiniumFilesMenu InfiniumDocumentsMenu;
        private InfiniumLightButton SendMailButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}