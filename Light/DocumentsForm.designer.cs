namespace Infinium
{
    partial class DocumentsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentsForm));
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.NavigateMenuButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.ButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.PasswordButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.MenuFilterButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.CategoriesButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.CreateDocumentButton = new Infinium.InfiniumLightButton();
            this.CurrentDocumentBackButton = new Infinium.InfiniumLightButton();
            this.UpdatePanel = new System.Windows.Forms.Panel();
            this.UpdatesPanel = new System.Windows.Forms.Panel();
            this.DateTypePanel = new System.Windows.Forms.Panel();
            this.MonthUpdatesButton = new Infinium.InfiniumDocumentsCheckButton();
            this.WeekUpdatesButton = new Infinium.InfiniumDocumentsCheckButton();
            this.TodayUpdatesButton = new Infinium.InfiniumDocumentsCheckButton();
            this.NewUpdatesButton = new Infinium.InfiniumDocumentsCheckButton();
            this.MyDocumentsPanel = new System.Windows.Forms.Panel();
            this.MyDocRecipientButton = new Infinium.InfiniumDocumentsCheckButton();
            this.MyDocCreatorButton = new Infinium.InfiniumDocumentsCheckButton();
            this.CurrentDocumentPanel = new System.Windows.Forms.Panel();
            this.DocumentsUpdatesList = new Infinium.InfiniumDocumentsUpdatesList();
            this.StatusPanel = new System.Windows.Forms.Panel();
            this.YourSignButton = new Infinium.InfiniumDocumentsCheckButton();
            this.CanceledButton = new Infinium.InfiniumDocumentsCheckButton();
            this.ConfirmedButton = new Infinium.InfiniumDocumentsCheckButton();
            this.NotConfirmedButton = new Infinium.InfiniumDocumentsCheckButton();
            this.AllDocumentsPanel = new System.Windows.Forms.Panel();
            this.InnerDocumentsList = new Infinium.InfiniumDocumentsList();
            this.OuterDocumentsList = new Infinium.InfiniumDocumentsList();
            this.IncomeDocumentsList = new Infinium.InfiniumDocumentsList();
            this.DocumentsListsPanel = new System.Windows.Forms.Panel();
            this.FilterPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.CategoriesComboBox = new System.Windows.Forms.ComboBox();
            this.kryptonBorderEdge12 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.DayCheckBox = new System.Windows.Forms.CheckBox();
            this.DayPicker = new System.Windows.Forms.DateTimePicker();
            this.DocTypeCheckBox = new System.Windows.Forms.CheckBox();
            this.DocTypeComboBox = new System.Windows.Forms.ComboBox();
            this.FactoryCheckBox = new System.Windows.Forms.CheckBox();
            this.FactoryComboBox = new System.Windows.Forms.ComboBox();
            this.CorrespondentCheckBox = new System.Windows.Forms.CheckBox();
            this.CorrespondentComboBox = new System.Windows.Forms.ComboBox();
            this.LeftPanel = new System.Windows.Forms.Panel();
            this.InfiniumDocumentsMenu = new Infinium.InfiniumDocumentsMenu();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
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
            this.TopMenuPanel = new System.Windows.Forms.Panel();
            this.EditButtonsPanel = new System.Windows.Forms.Panel();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.UpdatePanel.SuspendLayout();
            this.UpdatesPanel.SuspendLayout();
            this.DateTypePanel.SuspendLayout();
            this.MyDocumentsPanel.SuspendLayout();
            this.CurrentDocumentPanel.SuspendLayout();
            this.StatusPanel.SuspendLayout();
            this.AllDocumentsPanel.SuspendLayout();
            this.DocumentsListsPanel.SuspendLayout();
            this.FilterPanel.SuspendLayout();
            this.LeftPanel.SuspendLayout();
            this.NavigatePanel.SuspendLayout();
            this.TopMenuPanel.SuspendLayout();
            this.EditButtonsPanel.SuspendLayout();
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
            // MenuFilterButton
            // 
            this.MenuFilterButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MenuFilterButton.Location = new System.Drawing.Point(924, 27);
            this.MenuFilterButton.Name = "MenuFilterButton";
            this.MenuFilterButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.MenuFilterButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MenuFilterButton.OverrideDefault.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.MenuFilterButton.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MenuFilterButton.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MenuFilterButton.OverrideDefault.Border.Rounding = 3;
            this.MenuFilterButton.Size = new System.Drawing.Size(106, 34);
            this.MenuFilterButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.MenuFilterButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MenuFilterButton.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(184)))), ((int)(((byte)(238)))));
            this.MenuFilterButton.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MenuFilterButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MenuFilterButton.StateCommon.Border.Rounding = 3;
            this.MenuFilterButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.MenuFilterButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.MenuFilterButton.StateCommon.Content.Padding = new System.Windows.Forms.Padding(2, -1, -1, 0);
            this.MenuFilterButton.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.MenuFilterButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MenuFilterButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.MenuFilterButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(177)))), ((int)(((byte)(231)))));
            this.MenuFilterButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MenuFilterButton.StateTracking.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(177)))), ((int)(((byte)(231)))));
            this.MenuFilterButton.StateTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.MenuFilterButton.StateTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.MenuFilterButton.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.MenuFilterButton.StateTracking.Border.Rounding = 3;
            this.MenuFilterButton.TabIndex = 367;
            this.MenuFilterButton.TabStop = false;
            this.toolTip1.SetToolTip(this.MenuFilterButton, "Меню");
            this.MenuFilterButton.Values.Text = "Применить";
            this.MenuFilterButton.Click += new System.EventHandler(this.MenuFilterButton_Click);
            // 
            // CategoriesButton
            // 
            this.CategoriesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CategoriesButton.Location = new System.Drawing.Point(16, 11);
            this.CategoriesButton.Name = "CategoriesButton";
            this.CategoriesButton.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(81)))), ((int)(((byte)(55)))));
            this.CategoriesButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CategoriesButton.OverrideDefault.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(81)))), ((int)(((byte)(55)))));
            this.CategoriesButton.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CategoriesButton.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.CategoriesButton.OverrideDefault.Border.Rounding = 3;
            this.CategoriesButton.Size = new System.Drawing.Size(41, 41);
            this.CategoriesButton.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(81)))), ((int)(((byte)(55)))));
            this.CategoriesButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CategoriesButton.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(81)))), ((int)(((byte)(55)))));
            this.CategoriesButton.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CategoriesButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.CategoriesButton.StateCommon.Border.Rounding = 3;
            this.CategoriesButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CategoriesButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CategoriesButton.StateCommon.Content.Padding = new System.Windows.Forms.Padding(2, -1, -1, 0);
            this.CategoriesButton.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.CategoriesButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CategoriesButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CategoriesButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(48)))));
            this.CategoriesButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CategoriesButton.StateTracking.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(36)))), ((int)(((byte)(17)))));
            this.CategoriesButton.StateTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CategoriesButton.StateTracking.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.CategoriesButton.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.CategoriesButton.StateTracking.Border.Rounding = 3;
            this.CategoriesButton.TabIndex = 372;
            this.CategoriesButton.TabStop = false;
            this.toolTip1.SetToolTip(this.CategoriesButton, "Меню");
            this.CategoriesButton.Values.Image = ((System.Drawing.Image)(resources.GetObject("CategoriesButton.Values.Image")));
            this.CategoriesButton.Values.Text = "";
            this.CategoriesButton.Visible = false;
            // 
            // CreateDocumentButton
            // 
            this.CreateDocumentButton.BackColor = System.Drawing.Color.Transparent;
            this.CreateDocumentButton.BackTrackColor = System.Drawing.Color.White;
            this.CreateDocumentButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(194)))), ((int)(((byte)(194)))));
            this.CreateDocumentButton.Caption = "";
            this.CreateDocumentButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CreateDocumentButton.Disabled = false;
            this.CreateDocumentButton.DisabledImage = null;
            this.CreateDocumentButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CreateDocumentButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(205)))), ((int)(((byte)(252)))));
            this.CreateDocumentButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("CreateDocumentButton.Image")));
            this.CreateDocumentButton.Location = new System.Drawing.Point(5, 7);
            this.CreateDocumentButton.Name = "CreateDocumentButton";
            this.CreateDocumentButton.Size = new System.Drawing.Size(48, 48);
            this.CreateDocumentButton.TabIndex = 340;
            this.CreateDocumentButton.Text = "Создать документ";
            this.CreateDocumentButton.TextYMargin = 0;
            this.toolTip1.SetToolTip(this.CreateDocumentButton, "Создать новый документ");
            this.CreateDocumentButton.ToolTipText = "";
            this.CreateDocumentButton.Click += new System.EventHandler(this.CreateDocumentButton_Click);
            // 
            // CurrentDocumentBackButton
            // 
            this.CurrentDocumentBackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentDocumentBackButton.BackColor = System.Drawing.Color.Transparent;
            this.CurrentDocumentBackButton.BackTrackColor = System.Drawing.Color.White;
            this.CurrentDocumentBackButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(194)))), ((int)(((byte)(194)))));
            this.CurrentDocumentBackButton.Caption = "Закрыть";
            this.CurrentDocumentBackButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CurrentDocumentBackButton.Disabled = false;
            this.CurrentDocumentBackButton.DisabledImage = null;
            this.CurrentDocumentBackButton.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CurrentDocumentBackButton.ForeColor = System.Drawing.Color.DimGray;
            this.CurrentDocumentBackButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("CurrentDocumentBackButton.Image")));
            this.CurrentDocumentBackButton.Location = new System.Drawing.Point(901, 7);
            this.CurrentDocumentBackButton.Name = "CurrentDocumentBackButton";
            this.CurrentDocumentBackButton.Size = new System.Drawing.Size(106, 40);
            this.CurrentDocumentBackButton.TabIndex = 341;
            this.CurrentDocumentBackButton.Text = "Закрыть документ";
            this.CurrentDocumentBackButton.TextYMargin = 0;
            this.toolTip1.SetToolTip(this.CurrentDocumentBackButton, "Закрыть документ");
            this.CurrentDocumentBackButton.ToolTipText = "";
            this.CurrentDocumentBackButton.Click += new System.EventHandler(this.CurrentDocumentBackButton_Click);
            // 
            // UpdatePanel
            // 
            this.UpdatePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdatePanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.UpdatePanel.Controls.Add(this.UpdatesPanel);
            this.UpdatePanel.Controls.Add(this.AllDocumentsPanel);
            this.UpdatePanel.Location = new System.Drawing.Point(242, 118);
            this.UpdatePanel.Name = "UpdatePanel";
            this.UpdatePanel.Size = new System.Drawing.Size(1028, 603);
            this.UpdatePanel.TabIndex = 313;
            this.UpdatePanel.Tag = "5";
            // 
            // UpdatesPanel
            // 
            this.UpdatesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdatesPanel.BackColor = System.Drawing.Color.Gainsboro;
            this.UpdatesPanel.Controls.Add(this.DateTypePanel);
            this.UpdatesPanel.Controls.Add(this.MyDocumentsPanel);
            this.UpdatesPanel.Controls.Add(this.CurrentDocumentPanel);
            this.UpdatesPanel.Controls.Add(this.DocumentsUpdatesList);
            this.UpdatesPanel.Controls.Add(this.StatusPanel);
            this.UpdatesPanel.Location = new System.Drawing.Point(0, 0);
            this.UpdatesPanel.Name = "UpdatesPanel";
            this.UpdatesPanel.Size = new System.Drawing.Size(1028, 603);
            this.UpdatesPanel.TabIndex = 383;
            this.UpdatesPanel.Tag = "5";
            // 
            // DateTypePanel
            // 
            this.DateTypePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DateTypePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DateTypePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.DateTypePanel.Controls.Add(this.MonthUpdatesButton);
            this.DateTypePanel.Controls.Add(this.WeekUpdatesButton);
            this.DateTypePanel.Controls.Add(this.TodayUpdatesButton);
            this.DateTypePanel.Controls.Add(this.NewUpdatesButton);
            this.DateTypePanel.Location = new System.Drawing.Point(0, 0);
            this.DateTypePanel.Name = "DateTypePanel";
            this.DateTypePanel.Size = new System.Drawing.Size(1028, 53);
            this.DateTypePanel.TabIndex = 385;
            // 
            // MonthUpdatesButton
            // 
            this.MonthUpdatesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.MonthUpdatesButton.Caption = "Месяц";
            this.MonthUpdatesButton.Checked = false;
            this.MonthUpdatesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MonthUpdatesButton.Location = new System.Drawing.Point(352, 8);
            this.MonthUpdatesButton.Name = "MonthUpdatesButton";
            this.MonthUpdatesButton.Size = new System.Drawing.Size(100, 37);
            this.MonthUpdatesButton.TabIndex = 384;
            this.MonthUpdatesButton.Text = "v";
            this.MonthUpdatesButton.ToolTipText = "";
            this.MonthUpdatesButton.Click += new System.EventHandler(this.MonthUpdatesButton_Click);
            // 
            // WeekUpdatesButton
            // 
            this.WeekUpdatesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.WeekUpdatesButton.Caption = "Неделя";
            this.WeekUpdatesButton.Checked = false;
            this.WeekUpdatesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.WeekUpdatesButton.Location = new System.Drawing.Point(241, 8);
            this.WeekUpdatesButton.Name = "WeekUpdatesButton";
            this.WeekUpdatesButton.Size = new System.Drawing.Size(100, 37);
            this.WeekUpdatesButton.TabIndex = 383;
            this.WeekUpdatesButton.Text = "v";
            this.WeekUpdatesButton.ToolTipText = "";
            this.WeekUpdatesButton.Click += new System.EventHandler(this.WeekUpdatesButton_Click);
            // 
            // TodayUpdatesButton
            // 
            this.TodayUpdatesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.TodayUpdatesButton.Caption = "Сегодня";
            this.TodayUpdatesButton.Checked = false;
            this.TodayUpdatesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TodayUpdatesButton.Location = new System.Drawing.Point(130, 8);
            this.TodayUpdatesButton.Name = "TodayUpdatesButton";
            this.TodayUpdatesButton.Size = new System.Drawing.Size(100, 37);
            this.TodayUpdatesButton.TabIndex = 382;
            this.TodayUpdatesButton.Text = "v";
            this.TodayUpdatesButton.ToolTipText = "";
            this.TodayUpdatesButton.Click += new System.EventHandler(this.TodayUpdatesButton_Click);
            // 
            // NewUpdatesButton
            // 
            this.NewUpdatesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.NewUpdatesButton.Caption = "Новые";
            this.NewUpdatesButton.Checked = true;
            this.NewUpdatesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NewUpdatesButton.Location = new System.Drawing.Point(19, 8);
            this.NewUpdatesButton.Name = "NewUpdatesButton";
            this.NewUpdatesButton.Size = new System.Drawing.Size(100, 37);
            this.NewUpdatesButton.TabIndex = 381;
            this.NewUpdatesButton.Text = "v";
            this.NewUpdatesButton.ToolTipText = "";
            this.NewUpdatesButton.Click += new System.EventHandler(this.NewUpdatesButton_Click);
            // 
            // MyDocumentsPanel
            // 
            this.MyDocumentsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MyDocumentsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MyDocumentsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.MyDocumentsPanel.Controls.Add(this.MyDocRecipientButton);
            this.MyDocumentsPanel.Controls.Add(this.MyDocCreatorButton);
            this.MyDocumentsPanel.Location = new System.Drawing.Point(0, 0);
            this.MyDocumentsPanel.Name = "MyDocumentsPanel";
            this.MyDocumentsPanel.Size = new System.Drawing.Size(1028, 53);
            this.MyDocumentsPanel.TabIndex = 387;
            // 
            // MyDocRecipientButton
            // 
            this.MyDocRecipientButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.MyDocRecipientButton.Caption = "Я получатель";
            this.MyDocRecipientButton.Checked = false;
            this.MyDocRecipientButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MyDocRecipientButton.Location = new System.Drawing.Point(161, 8);
            this.MyDocRecipientButton.Name = "MyDocRecipientButton";
            this.MyDocRecipientButton.Size = new System.Drawing.Size(131, 37);
            this.MyDocRecipientButton.TabIndex = 382;
            this.MyDocRecipientButton.Text = "v";
            this.MyDocRecipientButton.ToolTipText = "";
            this.MyDocRecipientButton.Click += new System.EventHandler(this.MyDocRecipientButton_Click);
            // 
            // MyDocCreatorButton
            // 
            this.MyDocCreatorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.MyDocCreatorButton.Caption = "Я создатель";
            this.MyDocCreatorButton.Checked = true;
            this.MyDocCreatorButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MyDocCreatorButton.Location = new System.Drawing.Point(19, 8);
            this.MyDocCreatorButton.Name = "MyDocCreatorButton";
            this.MyDocCreatorButton.Size = new System.Drawing.Size(131, 37);
            this.MyDocCreatorButton.TabIndex = 381;
            this.MyDocCreatorButton.Text = "v";
            this.MyDocCreatorButton.ToolTipText = "";
            this.MyDocCreatorButton.Click += new System.EventHandler(this.MyDocCreatorButton_Click);
            // 
            // CurrentDocumentPanel
            // 
            this.CurrentDocumentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentDocumentPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CurrentDocumentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.CurrentDocumentPanel.Controls.Add(this.CurrentDocumentBackButton);
            this.CurrentDocumentPanel.Location = new System.Drawing.Point(0, 0);
            this.CurrentDocumentPanel.Name = "CurrentDocumentPanel";
            this.CurrentDocumentPanel.Size = new System.Drawing.Size(1028, 53);
            this.CurrentDocumentPanel.TabIndex = 386;
            // 
            // DocumentsUpdatesList
            // 
            this.DocumentsUpdatesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentsUpdatesList.Location = new System.Drawing.Point(0, 55);
            this.DocumentsUpdatesList.Name = "DocumentsUpdatesList";
            this.DocumentsUpdatesList.Size = new System.Drawing.Size(1027, 546);
            this.DocumentsUpdatesList.TabIndex = 380;
            this.DocumentsUpdatesList.Text = "infiniumDocumentsUpdatesList1";
            // 
            // 
            // 
            this.DocumentsUpdatesList.VerticalScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentsUpdatesList.VerticalScrollBar.Location = new System.Drawing.Point(1015, 0);
            this.DocumentsUpdatesList.VerticalScrollBar.Name = "";
            this.DocumentsUpdatesList.VerticalScrollBar.Offset = 0;
            this.DocumentsUpdatesList.VerticalScrollBar.ScrollWheelOffset = 70;
            this.DocumentsUpdatesList.VerticalScrollBar.Size = new System.Drawing.Size(12, 546);
            this.DocumentsUpdatesList.VerticalScrollBar.TabIndex = 0;
            this.DocumentsUpdatesList.VerticalScrollBar.TotalControlHeight = 546;
            this.DocumentsUpdatesList.VerticalScrollBar.VerticalScrollCommonShaftBackColor = System.Drawing.Color.Transparent;
            this.DocumentsUpdatesList.VerticalScrollBar.VerticalScrollCommonThumbButtonColor = System.Drawing.Color.Gray;
            this.DocumentsUpdatesList.VerticalScrollBar.VerticalScrollTrackingShaftBackColor = System.Drawing.Color.Silver;
            this.DocumentsUpdatesList.VerticalScrollBar.VerticalScrollTrackingThumbButtonColor = System.Drawing.Color.DimGray;
            this.DocumentsUpdatesList.VerticalScrollBar.Visible = false;
            this.DocumentsUpdatesList.CommentsTextBoxFileLabelClicked += new System.EventHandler(this.DocumentsUpdatesList_CommentsTextBoxFileLabelClicked);
            this.DocumentsUpdatesList.CommentsSendButtonClicked += new Infinium.InfiniumDocumentsUpdatesList.CommentsSendButtonClickedEventHandler(this.DocumentsUpdatesList_CommentsSendButtonClicked);
            this.DocumentsUpdatesList.EditCommentClicked += new Infinium.InfiniumDocumentsUpdatesList.CommentEditEventHandler(this.DocumentsUpdatesList_EditCommentClicked);
            this.DocumentsUpdatesList.DeleteCommentClicked += new Infinium.InfiniumDocumentsUpdatesList.CommentEditEventHandler(this.DocumentsUpdatesList_DeleteCommentClicked);
            this.DocumentsUpdatesList.FileClicked += new Infinium.InfiniumDocumentsUpdatesList.FileClickEventHandler(this.DocumentsUpdatesList_FileClicked);
            this.DocumentsUpdatesList.CommentFileClicked += new Infinium.InfiniumDocumentsUpdatesList.CommentFileClickEventHandler(this.DocumentsUpdatesList_CommentFileClicked);
            this.DocumentsUpdatesList.AddConfirmClicked += new Infinium.InfiniumDocumentsUpdatesList.AddConfirmClickEventHandler(this.DocumentsUpdatesList_AddConfirmClicked);
            this.DocumentsUpdatesList.AddUserClicked += new Infinium.InfiniumDocumentsUpdatesList.AddConfirmClickEventHandler(this.DocumentsUpdatesList_AddUserClicked);
            this.DocumentsUpdatesList.ConfirmItemConfirmClicked += new Infinium.InfiniumDocumentsUpdatesList.CheckConfirmClickedEventHandler(this.DocumentsUpdatesList_ConfirmItemConfirmClicked);
            this.DocumentsUpdatesList.ConfirmItemCancelClicked += new Infinium.InfiniumDocumentsUpdatesList.CheckConfirmClickedEventHandler(this.DocumentsUpdatesList_ConfirmItemCancelClicked);
            this.DocumentsUpdatesList.ConfirmItemEditClicked += new Infinium.InfiniumDocumentsUpdatesList.CheckConfirmClickedEventHandler(this.DocumentsUpdatesList_ConfirmItemEditClicked);
            this.DocumentsUpdatesList.ConfirmEditClicked += new Infinium.InfiniumDocumentsUpdatesList.EditConfirmClickedEventHandler(this.DocumentsUpdatesList_ConfirmEditClicked);
            this.DocumentsUpdatesList.ConfirmDeleteClicked += new Infinium.InfiniumDocumentsUpdatesList.EditConfirmClickedEventHandler(this.DocumentsUpdatesList_ConfirmDeleteClicked);
            // 
            // StatusPanel
            // 
            this.StatusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.StatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.StatusPanel.Controls.Add(this.YourSignButton);
            this.StatusPanel.Controls.Add(this.CanceledButton);
            this.StatusPanel.Controls.Add(this.ConfirmedButton);
            this.StatusPanel.Controls.Add(this.NotConfirmedButton);
            this.StatusPanel.Location = new System.Drawing.Point(0, 0);
            this.StatusPanel.Name = "StatusPanel";
            this.StatusPanel.Size = new System.Drawing.Size(1028, 53);
            this.StatusPanel.TabIndex = 385;
            // 
            // YourSignButton
            // 
            this.YourSignButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.YourSignButton.Caption = "Ваша подпись";
            this.YourSignButton.Checked = true;
            this.YourSignButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.YourSignButton.Location = new System.Drawing.Point(19, 8);
            this.YourSignButton.Name = "YourSignButton";
            this.YourSignButton.Size = new System.Drawing.Size(156, 37);
            this.YourSignButton.TabIndex = 384;
            this.YourSignButton.Text = "v";
            this.YourSignButton.ToolTipText = "";
            this.YourSignButton.Click += new System.EventHandler(this.YourSignButton_Click);
            // 
            // CanceledButton
            // 
            this.CanceledButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.CanceledButton.Caption = "Отклонены";
            this.CanceledButton.Checked = false;
            this.CanceledButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CanceledButton.Location = new System.Drawing.Point(520, 8);
            this.CanceledButton.Name = "CanceledButton";
            this.CanceledButton.Size = new System.Drawing.Size(156, 37);
            this.CanceledButton.TabIndex = 383;
            this.CanceledButton.Text = "v";
            this.CanceledButton.ToolTipText = "";
            this.CanceledButton.Click += new System.EventHandler(this.CanceledButton_Click);
            // 
            // ConfirmedButton
            // 
            this.ConfirmedButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ConfirmedButton.Caption = "Согласованы";
            this.ConfirmedButton.Checked = false;
            this.ConfirmedButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ConfirmedButton.Location = new System.Drawing.Point(353, 8);
            this.ConfirmedButton.Name = "ConfirmedButton";
            this.ConfirmedButton.Size = new System.Drawing.Size(156, 37);
            this.ConfirmedButton.TabIndex = 382;
            this.ConfirmedButton.Text = "v";
            this.ConfirmedButton.ToolTipText = "";
            this.ConfirmedButton.Click += new System.EventHandler(this.ConfirmedButton_Click);
            // 
            // NotConfirmedButton
            // 
            this.NotConfirmedButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.NotConfirmedButton.Caption = "Не согласованы";
            this.NotConfirmedButton.Checked = false;
            this.NotConfirmedButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NotConfirmedButton.Location = new System.Drawing.Point(186, 8);
            this.NotConfirmedButton.Name = "NotConfirmedButton";
            this.NotConfirmedButton.Size = new System.Drawing.Size(156, 37);
            this.NotConfirmedButton.TabIndex = 381;
            this.NotConfirmedButton.Text = "v";
            this.NotConfirmedButton.ToolTipText = "";
            this.NotConfirmedButton.Click += new System.EventHandler(this.NotConfirmedButton_Click);
            // 
            // AllDocumentsPanel
            // 
            this.AllDocumentsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AllDocumentsPanel.BackColor = System.Drawing.Color.Gainsboro;
            this.AllDocumentsPanel.Controls.Add(this.InnerDocumentsList);
            this.AllDocumentsPanel.Controls.Add(this.OuterDocumentsList);
            this.AllDocumentsPanel.Controls.Add(this.IncomeDocumentsList);
            this.AllDocumentsPanel.Controls.Add(this.DocumentsListsPanel);
            this.AllDocumentsPanel.Location = new System.Drawing.Point(0, 0);
            this.AllDocumentsPanel.Name = "AllDocumentsPanel";
            this.AllDocumentsPanel.Size = new System.Drawing.Size(1028, 603);
            this.AllDocumentsPanel.TabIndex = 382;
            this.AllDocumentsPanel.Tag = "5";
            // 
            // InnerDocumentsList
            // 
            this.InnerDocumentsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InnerDocumentsList.Location = new System.Drawing.Point(0, 76);
            this.InnerDocumentsList.Name = "InnerDocumentsList";
            this.InnerDocumentsList.Size = new System.Drawing.Size(1026, 525);
            this.InnerDocumentsList.TabIndex = 378;
            this.InnerDocumentsList.Text = "infiniumDocumentsList1";
            this.InnerDocumentsList.EditClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.InnerDocumentsList_EditClicked);
            this.InnerDocumentsList.DeleteClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.InnerDocumentsList_DeleteClicked);
            this.InnerDocumentsList.ItemClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.InnerDocumentsList_ItemClicked);
            // 
            // OuterDocumentsList
            // 
            this.OuterDocumentsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OuterDocumentsList.Location = new System.Drawing.Point(0, 76);
            this.OuterDocumentsList.Name = "OuterDocumentsList";
            this.OuterDocumentsList.Size = new System.Drawing.Size(1026, 525);
            this.OuterDocumentsList.TabIndex = 380;
            this.OuterDocumentsList.Text = "infiniumOuterDocumentsList1";
            this.OuterDocumentsList.EditClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.OuterDocumentsList_EditClicked);
            this.OuterDocumentsList.DeleteClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.OuterDocumentsList_DeleteClicked);
            this.OuterDocumentsList.ItemClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.OuterDocumentsList_ItemClicked);
            // 
            // IncomeDocumentsList
            // 
            this.IncomeDocumentsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IncomeDocumentsList.Location = new System.Drawing.Point(0, 76);
            this.IncomeDocumentsList.Name = "IncomeDocumentsList";
            this.IncomeDocumentsList.Size = new System.Drawing.Size(1026, 525);
            this.IncomeDocumentsList.TabIndex = 379;
            this.IncomeDocumentsList.Text = "infiniumIncomeDocumentsList1";
            this.IncomeDocumentsList.EditClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.IncomeDocumentsList_EditClicked);
            this.IncomeDocumentsList.DeleteClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.IncomeDocumentsList_DeleteClicked);
            this.IncomeDocumentsList.ItemClicked += new Infinium.InfiniumDocumentsList.EditClickedEventHandler(this.IncomeDocumentsList_ItemClicked);
            // 
            // DocumentsListsPanel
            // 
            this.DocumentsListsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentsListsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DocumentsListsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.DocumentsListsPanel.Controls.Add(this.FilterPanel);
            this.DocumentsListsPanel.Location = new System.Drawing.Point(0, 0);
            this.DocumentsListsPanel.Name = "DocumentsListsPanel";
            this.DocumentsListsPanel.Size = new System.Drawing.Size(1026, 77);
            this.DocumentsListsPanel.TabIndex = 383;
            // 
            // FilterPanel
            // 
            this.FilterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FilterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.FilterPanel.Controls.Add(this.label2);
            this.FilterPanel.Controls.Add(this.CategoriesComboBox);
            this.FilterPanel.Controls.Add(this.MenuFilterButton);
            this.FilterPanel.Controls.Add(this.kryptonBorderEdge12);
            this.FilterPanel.Controls.Add(this.DayCheckBox);
            this.FilterPanel.Controls.Add(this.DayPicker);
            this.FilterPanel.Controls.Add(this.DocTypeCheckBox);
            this.FilterPanel.Controls.Add(this.DocTypeComboBox);
            this.FilterPanel.Controls.Add(this.FactoryCheckBox);
            this.FilterPanel.Controls.Add(this.FactoryComboBox);
            this.FilterPanel.Controls.Add(this.CorrespondentCheckBox);
            this.FilterPanel.Controls.Add(this.CorrespondentComboBox);
            this.FilterPanel.Location = new System.Drawing.Point(0, 0);
            this.FilterPanel.Name = "FilterPanel";
            this.FilterPanel.Size = new System.Drawing.Size(1026, 76);
            this.FilterPanel.TabIndex = 382;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.Location = new System.Drawing.Point(11, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 19);
            this.label2.TabIndex = 370;
            this.label2.Text = "Категория";
            // 
            // CategoriesComboBox
            // 
            this.CategoriesComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CategoriesComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CategoriesComboBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CategoriesComboBox.FormattingEnabled = true;
            this.CategoriesComboBox.Location = new System.Drawing.Point(15, 35);
            this.CategoriesComboBox.Name = "CategoriesComboBox";
            this.CategoriesComboBox.Size = new System.Drawing.Size(155, 23);
            this.CategoriesComboBox.TabIndex = 369;
            // 
            // kryptonBorderEdge12
            // 
            this.kryptonBorderEdge12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge12.Location = new System.Drawing.Point(0, 75);
            this.kryptonBorderEdge12.Name = "kryptonBorderEdge12";
            this.kryptonBorderEdge12.Size = new System.Drawing.Size(1026, 1);
            this.kryptonBorderEdge12.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.kryptonBorderEdge12.Text = "kryptonBorderEdge12";
            // 
            // DayCheckBox
            // 
            this.DayCheckBox.AutoSize = true;
            this.DayCheckBox.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DayCheckBox.Location = new System.Drawing.Point(734, 8);
            this.DayCheckBox.Name = "DayCheckBox";
            this.DayCheckBox.Size = new System.Drawing.Size(63, 23);
            this.DayCheckBox.TabIndex = 303;
            this.DayCheckBox.Text = "День";
            this.DayCheckBox.UseVisualStyleBackColor = true;
            // 
            // DayPicker
            // 
            this.DayPicker.CalendarFont = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DayPicker.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DayPicker.Location = new System.Drawing.Point(734, 35);
            this.DayPicker.Name = "DayPicker";
            this.DayPicker.Size = new System.Drawing.Size(164, 23);
            this.DayPicker.TabIndex = 302;
            // 
            // DocTypeCheckBox
            // 
            this.DocTypeCheckBox.AutoSize = true;
            this.DocTypeCheckBox.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DocTypeCheckBox.Location = new System.Drawing.Point(192, 8);
            this.DocTypeCheckBox.Name = "DocTypeCheckBox";
            this.DocTypeCheckBox.Size = new System.Drawing.Size(126, 23);
            this.DocTypeCheckBox.TabIndex = 304;
            this.DocTypeCheckBox.Text = "Тип документа";
            this.DocTypeCheckBox.UseVisualStyleBackColor = true;
            // 
            // DocTypeComboBox
            // 
            this.DocTypeComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.DocTypeComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.DocTypeComboBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.DocTypeComboBox.FormattingEnabled = true;
            this.DocTypeComboBox.Location = new System.Drawing.Point(192, 35);
            this.DocTypeComboBox.Name = "DocTypeComboBox";
            this.DocTypeComboBox.Size = new System.Drawing.Size(172, 23);
            this.DocTypeComboBox.TabIndex = 302;
            // 
            // FactoryCheckBox
            // 
            this.FactoryCheckBox.AutoSize = true;
            this.FactoryCheckBox.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FactoryCheckBox.Location = new System.Drawing.Point(580, 8);
            this.FactoryCheckBox.Name = "FactoryCheckBox";
            this.FactoryCheckBox.Size = new System.Drawing.Size(74, 23);
            this.FactoryCheckBox.TabIndex = 308;
            this.FactoryCheckBox.Text = "Участок";
            this.FactoryCheckBox.UseVisualStyleBackColor = true;
            // 
            // FactoryComboBox
            // 
            this.FactoryComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.FactoryComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.FactoryComboBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FactoryComboBox.FormattingEnabled = true;
            this.FactoryComboBox.Location = new System.Drawing.Point(580, 35);
            this.FactoryComboBox.Name = "FactoryComboBox";
            this.FactoryComboBox.Size = new System.Drawing.Size(132, 23);
            this.FactoryComboBox.TabIndex = 307;
            // 
            // CorrespondentCheckBox
            // 
            this.CorrespondentCheckBox.AutoSize = true;
            this.CorrespondentCheckBox.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CorrespondentCheckBox.Location = new System.Drawing.Point(386, 8);
            this.CorrespondentCheckBox.Name = "CorrespondentCheckBox";
            this.CorrespondentCheckBox.Size = new System.Drawing.Size(129, 23);
            this.CorrespondentCheckBox.TabIndex = 306;
            this.CorrespondentCheckBox.Text = "Корреспондент";
            this.CorrespondentCheckBox.UseVisualStyleBackColor = true;
            // 
            // CorrespondentComboBox
            // 
            this.CorrespondentComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CorrespondentComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CorrespondentComboBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CorrespondentComboBox.FormattingEnabled = true;
            this.CorrespondentComboBox.Location = new System.Drawing.Point(386, 35);
            this.CorrespondentComboBox.Name = "CorrespondentComboBox";
            this.CorrespondentComboBox.Size = new System.Drawing.Size(172, 23);
            this.CorrespondentComboBox.TabIndex = 305;
            // 
            // LeftPanel
            // 
            this.LeftPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LeftPanel.BackColor = System.Drawing.Color.White;
            this.LeftPanel.Controls.Add(this.InfiniumDocumentsMenu);
            this.LeftPanel.Controls.Add(this.kryptonBorderEdge1);
            this.LeftPanel.Location = new System.Drawing.Point(0, 118);
            this.LeftPanel.Name = "LeftPanel";
            this.LeftPanel.Size = new System.Drawing.Size(242, 604);
            this.LeftPanel.TabIndex = 296;
            // 
            // InfiniumDocumentsMenu
            // 
            this.InfiniumDocumentsMenu.BackColor = System.Drawing.Color.White;
            this.InfiniumDocumentsMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfiniumDocumentsMenu.ItemsDataTable = null;
            this.InfiniumDocumentsMenu.Location = new System.Drawing.Point(0, 0);
            this.InfiniumDocumentsMenu.Name = "InfiniumDocumentsMenu";
            this.InfiniumDocumentsMenu.Selected = 0;
            this.InfiniumDocumentsMenu.Size = new System.Drawing.Size(241, 604);
            this.InfiniumDocumentsMenu.TabIndex = 1;
            this.InfiniumDocumentsMenu.Text = "infiniumDocumentsMenu1";
            // 
            // 
            // 
            this.InfiniumDocumentsMenu.VerticalScrollBar.Location = new System.Drawing.Point(241, 0);
            this.InfiniumDocumentsMenu.VerticalScrollBar.Name = "";
            this.InfiniumDocumentsMenu.VerticalScrollBar.Offset = 0;
            this.InfiniumDocumentsMenu.VerticalScrollBar.ScrollWheelOffset = 30;
            this.InfiniumDocumentsMenu.VerticalScrollBar.TabIndex = 0;
            this.InfiniumDocumentsMenu.VerticalScrollBar.TotalControlHeight = 604;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollCommonShaftBackColor = System.Drawing.Color.LightBlue;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollCommonThumbButtonColor = System.Drawing.Color.DarkGray;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollTrackingShaftBackColor = System.Drawing.Color.Blue;
            this.InfiniumDocumentsMenu.VerticalScrollBar.VerticalScrollTrackingThumbButtonColor = System.Drawing.Color.Gray;
            this.InfiniumDocumentsMenu.SelectedChanged += new Infinium.InfiniumDocumentsMenu.SelectedChangedEventHandler(this.InfiniumDocumentsMenu_SelectedChanged);
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.AutoSize = false;
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(241, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(1, 604);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
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
            this.MinimizeButton.TabIndex = 33;
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
            this.label1.Size = new System.Drawing.Size(308, 45);
            this.label1.TabIndex = 31;
            this.label1.Text = "Infinium. Документы";
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
            // 
            // MenuFileSaveFile
            // 
            this.MenuFileSaveFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileSaveFile.Image")));
            this.MenuFileSaveFile.Text = "Сохранить";
            // 
            // MenuFileReplaceFile
            // 
            this.MenuFileReplaceFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileReplaceFile.Image")));
            this.MenuFileReplaceFile.Text = "Заменить";
            // 
            // MenuFileDeleteFile
            // 
            this.MenuFileDeleteFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileDeleteFile.Image")));
            this.MenuFileDeleteFile.Text = "Удалить";
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
            // TopMenuPanel
            // 
            this.TopMenuPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TopMenuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.TopMenuPanel.Controls.Add(this.EditButtonsPanel);
            this.TopMenuPanel.Controls.Add(this.CategoriesButton);
            this.TopMenuPanel.Controls.Add(this.kryptonBorderEdge2);
            this.TopMenuPanel.Location = new System.Drawing.Point(0, 54);
            this.TopMenuPanel.Margin = new System.Windows.Forms.Padding(4);
            this.TopMenuPanel.Name = "TopMenuPanel";
            this.TopMenuPanel.Size = new System.Drawing.Size(1270, 64);
            this.TopMenuPanel.TabIndex = 314;
            // 
            // EditButtonsPanel
            // 
            this.EditButtonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EditButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EditButtonsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.EditButtonsPanel.Controls.Add(this.CreateDocumentButton);
            this.EditButtonsPanel.Location = new System.Drawing.Point(1202, 2);
            this.EditButtonsPanel.Name = "EditButtonsPanel";
            this.EditButtonsPanel.Size = new System.Drawing.Size(57, 59);
            this.EditButtonsPanel.TabIndex = 383;
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.AutoSize = false;
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(0, 63);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(1270, 1);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // DocumentsForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1270, 720);
            this.Controls.Add(this.TopMenuPanel);
            this.Controls.Add(this.UpdatePanel);
            this.Controls.Add(this.LeftPanel);
            this.Controls.Add(this.NavigatePanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocumentsForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ANSUpdate += new Infinium.InfiniumForm.ANSUpdateEventHandler(this.ProjectsForm_ANSUpdate);
            this.Shown += new System.EventHandler(this.LightNewsForm_Shown);
            this.UpdatePanel.ResumeLayout(false);
            this.UpdatesPanel.ResumeLayout(false);
            this.DateTypePanel.ResumeLayout(false);
            this.MyDocumentsPanel.ResumeLayout(false);
            this.CurrentDocumentPanel.ResumeLayout(false);
            this.StatusPanel.ResumeLayout(false);
            this.AllDocumentsPanel.ResumeLayout(false);
            this.DocumentsListsPanel.ResumeLayout(false);
            this.FilterPanel.ResumeLayout(false);
            this.FilterPanel.PerformLayout();
            this.LeftPanel.ResumeLayout(false);
            this.NavigatePanel.ResumeLayout(false);
            this.NavigatePanel.PerformLayout();
            this.TopMenuPanel.ResumeLayout(false);
            this.EditButtonsPanel.ResumeLayout(false);
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
        private System.Windows.Forms.Panel LeftPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private System.Windows.Forms.Panel UpdatePanel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog UploadFileDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu FileContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileOpenFile;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileSaveFile;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem3;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette ContextMenuPalette;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileReplaceFile;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuFileDeleteFile;
        private System.Windows.Forms.Timer timer1;
        private InfiniumDocumentsMenu InfiniumDocumentsMenu;
        private System.Windows.Forms.CheckBox DayCheckBox;
        private System.Windows.Forms.DateTimePicker DayPicker;
        private ComponentFactory.Krypton.Toolkit.KryptonButton MenuFilterButton;
        private System.Windows.Forms.CheckBox FactoryCheckBox;
        private System.Windows.Forms.ComboBox FactoryComboBox;
        private System.Windows.Forms.CheckBox CorrespondentCheckBox;
        private System.Windows.Forms.ComboBox CorrespondentComboBox;
        private System.Windows.Forms.CheckBox DocTypeCheckBox;
        private System.Windows.Forms.ComboBox DocTypeComboBox;
        private InfiniumDocumentsList InnerDocumentsList;
        private InfiniumDocumentsList IncomeDocumentsList;
        private InfiniumDocumentsList OuterDocumentsList;
        private InfiniumDocumentsUpdatesList DocumentsUpdatesList;
        private System.Windows.Forms.Panel AllDocumentsPanel;
        private System.Windows.Forms.Panel FilterPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge12;
        private System.Windows.Forms.Panel UpdatesPanel;
        private System.Windows.Forms.Panel TopMenuPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton CategoriesButton;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private InfiniumLightButton CreateDocumentButton;
        private System.Windows.Forms.Panel DocumentsListsPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CategoriesComboBox;
        private System.Windows.Forms.Panel EditButtonsPanel;
        private InfiniumDocumentsCheckButton NewUpdatesButton;
        private InfiniumDocumentsCheckButton MonthUpdatesButton;
        private InfiniumDocumentsCheckButton WeekUpdatesButton;
        private InfiniumDocumentsCheckButton TodayUpdatesButton;
        private System.Windows.Forms.Panel DateTypePanel;
        private System.Windows.Forms.Panel StatusPanel;
        private InfiniumDocumentsCheckButton CanceledButton;
        private InfiniumDocumentsCheckButton ConfirmedButton;
        private InfiniumDocumentsCheckButton NotConfirmedButton;
        private InfiniumDocumentsCheckButton YourSignButton;
        private System.Windows.Forms.Panel CurrentDocumentPanel;
        private InfiniumLightButton CurrentDocumentBackButton;
        private System.Windows.Forms.Panel MyDocumentsPanel;
        private InfiniumDocumentsCheckButton MyDocRecipientButton;
        private InfiniumDocumentsCheckButton MyDocCreatorButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton MinimizeButton;
    }
}