using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CatalogForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;
        const int iEditor = 93;

        int FormEvent = 0;

        bool bCanEdit = false;
        bool NeedSplash = false;

        LightStartForm LightStartForm;

        Form TopForm = null;
        Infinium.FrontsCatalog FrontsCatalog = null;
        Infinium.DecorCatalog DecorCatalog = null;
        CabFurLabel CabFurLabelManager = null;
        CubeLabel CubeLabelManager = null;
        SampleLabel SampleLabelManager = null;
        DataTable AttachmentsDT = null;
        DataTable FrontsDT = null;
        DataTable DecorDT = null;
        DataTable CabFurDT;

        public CatalogForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            btnSetDescription.Enabled = false;
            kryptonButton1.Enabled = false;
            SetPermissions();

            while (!SplashForm.bCreated) ;
        }

        private void SetPermissions()
        {
            DataTable DT = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);

            if (DT.Rows.Count == 0)
                return;

            if (DT.Select("RoleID = 12").Count() == 0)
                return;
            bCanEdit = true;
            btnSetDescription.Enabled = true;
            kryptonButton1.Enabled = true;
            //kryptonContextMenuItem7.Enabled = true;
            //kryptonContextMenuItem9.Enabled = true;
            //kryptonContextMenuItem10.Enabled = true;
            //kryptonContextMenuItem11.Enabled = true;
        }

        private void CatalogForm_Shown(object sender, EventArgs e)
        {
            FrontsCatalogPanel.BringToFront();
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedSplash = true;
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                if (FormEvent == eClose || FormEvent == eHide)
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {
                        NeedSplash = false;
                        LightStartForm.HideForm(this);
                    }


                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    return;
                }

            }

            if (FormEvent == eClose || FormEvent == eHide)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.05));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {
                        NeedSplash = false;
                        LightStartForm.HideForm(this);
                    }

                }

                return;
            }


            if (FormEvent == eShow || FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
        }


        private void Initialize()
        {
            CabFurLabelManager = new CabFurLabel();
            CubeLabelManager = new CubeLabel();
            CabFurDT = new DataTable();
            CabFurDT.Columns.Add(new DataColumn("TechStoreName", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("TechStoreSubGroupName", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("SubGroupNotes", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("SubGroupNotes1", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("SubGroupNotes2", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Color", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Color2", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Height", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Width", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Length", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("PositionsCount", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("LabelsCount", Type.GetType("System.Int32")));
            CabFurDT.Columns.Add(new DataColumn("FactoryType", Type.GetType("System.Int32")));
            CabFurDT.Columns.Add(new DataColumn("DecorConfigID", Type.GetType("System.Int32")));

            AttachmentsDT = new DataTable();
            AttachmentsDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            AttachmentsDT.Columns.Add(new DataColumn("Extension", Type.GetType("System.String")));
            AttachmentsDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));
            if (FrontsDT == null)
            {
                FrontsDT = new DataTable();
                FrontsDT.Columns.Add(new DataColumn("Front", Type.GetType("System.String")));
                FrontsDT.Columns.Add(new DataColumn("FrameColor", Type.GetType("System.String")));
                FrontsDT.Columns.Add(new DataColumn("InsetType", Type.GetType("System.String")));
                FrontsDT.Columns.Add(new DataColumn("InsetColor", Type.GetType("System.String")));
                FrontsDT.Columns.Add(new DataColumn("PositionsCount", Type.GetType("System.String")));
                FrontsDT.Columns.Add(new DataColumn("LabelsCount", Type.GetType("System.Int32")));
                FrontsDT.Columns.Add(new DataColumn("FactoryType", Type.GetType("System.Int32")));
                FrontsDT.Columns.Add(new DataColumn("FrontConfigID", Type.GetType("System.Int32")));
                FrontsDT.Columns.Add(new DataColumn("FrontID", Type.GetType("System.Int32")));
                FrontsDT.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int32")));
                FrontsDT.Columns.Add(new DataColumn("TechnoColorID", Type.GetType("System.Int32")));
            }
            if (DecorDT == null)
            {
                DecorDT = new DataTable();
                DecorDT.Columns.Add(new DataColumn("Product", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Decor", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Color", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Length", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Height", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Width", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("PositionsCount", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("LabelsCount", Type.GetType("System.Int32")));
                DecorDT.Columns.Add(new DataColumn("FactoryType", Type.GetType("System.Int32")));
                DecorDT.Columns.Add(new DataColumn("DecorConfigID", Type.GetType("System.Int32")));
            }
            SampleLabelManager = new SampleLabel();

            FrontsCatalog = new FrontsCatalog(1);
            DecorCatalog = new DecorCatalog(1);

            FrontsCatalogBinding();
            DecorCatalogBinding();
            FrontsGridsSettings();
            DecorGridsSettings();
        }

        #region FRONTS FUNCTIONS
        private void FrontsCatalogBinding()
        {
            FrontsDataGrid.SelectionChanged -= FrontsDataGrid_SelectionChanged;
            FrameColorsDataGrid.SelectionChanged -= FrameColorsDataGrid_SelectionChanged;
            TechnoFrameColorsDataGrid.SelectionChanged -= TechnoFrameColorsDataGrid_SelectionChanged;
            InsetTypesDataGrid.SelectionChanged -= InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;

            FrontsDataGrid.DataSource = FrontsCatalog.FrontsBindingSource;
            FrameColorsDataGrid.DataSource = FrontsCatalog.FrameColorsBindingSource;
            TechnoFrameColorsDataGrid.DataSource = FrontsCatalog.TechnoFrameColorsBindingSource;
            PatinaDataGrid.DataSource = FrontsCatalog.PatinaBindingSource;
            InsetTypesDataGrid.DataSource = FrontsCatalog.InsetTypesBindingSource;
            InsetColorsDataGrid.DataSource = FrontsCatalog.InsetColorsBindingSource;
            TechnoInsetTypesDataGrid.DataSource = FrontsCatalog.TechnoInsetTypesBindingSource;
            TechnoInsetColorsDataGrid.DataSource = FrontsCatalog.TechnoInsetColorsBindingSource;
            FrontsHeightDataGrid.DataSource = FrontsCatalog.HeightBindingSource;
            FrontsWidthDataGrid.DataSource = FrontsCatalog.WidthBindingSource;

            FrontsCatalog.FilterFronts();
            GetFrameColors();
            GetTechnoFrameColors();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();

            FrontsDataGrid.SelectionChanged += FrontsDataGrid_SelectionChanged;
            FrameColorsDataGrid.SelectionChanged += FrameColorsDataGrid_SelectionChanged;
            TechnoFrameColorsDataGrid.SelectionChanged += TechnoFrameColorsDataGrid_SelectionChanged;
            InsetTypesDataGrid.SelectionChanged += InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void FrontsGridsSettings()
        {
            FrontsDataGrid.Columns["FrontID"].Visible = false;
            FrameColorsDataGrid.Columns["ColorID"].Visible = false;
            PatinaDataGrid.Columns["PatinaID"].Visible = false;
            InsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            InsetTypesDataGrid.Columns["GroupID"].Visible = false;
            InsetColorsDataGrid.Columns["table_id"].Visible = false;
            InsetColorsDataGrid.Columns["InsetColorID"].Visible = false;
            InsetColorsDataGrid.Columns["GroupID"].Visible = false;
            TechnoFrameColorsDataGrid.Columns["ColorID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["GroupID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["InsetColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["GroupID"].Visible = false;

            FrontsDataGrid.Columns["FrontName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrameColorsDataGrid.Columns["ColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PatinaDataGrid.Columns["PatinaName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetTypesDataGrid.Columns["InsetTypeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetColorsDataGrid.Columns["InsetColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoFrameColorsDataGrid.Columns["ColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetTypesDataGrid.Columns["InsetTypeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetColorsDataGrid.Columns["InsetColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void GetFrameColors()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();

            FrontsCatalog.FilterCatalogFrameColors(FrontName);
        }

        public void GetTechnoFrameColors()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);

            FrontsCatalog.FilterCatalogTechnoFrameColors(FrontName, ColorID);
        }

        public void GetPatina()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            FrontsCatalog.FilterCatalogPatina(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
        }

        public void GetInsetTypes()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);

            FrontsCatalog.FilterCatalogInsetTypes(FrontName, ColorID, TechnoColorID);
        }

        public void GetInsetColors()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);

            FrontsCatalog.FilterCatalogInsetColors(FrontName, ColorID, TechnoColorID, InsetTypeID);
        }

        public void GetTechnoInsetTypes()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            FrontsCatalog.FilterCatalogTechnoInsetTypes(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID);

        }

        public void GetTechnoInsetColors()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);

            FrontsCatalog.FilterCatalogTechnoInsetColors(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID);

        }

        Bitmap layer;
        Bitmap backgr;
        Bitmap backgr2;
        Bitmap newBitmap;
        Bitmap newBitmap2;

        public void GetFrontHeight()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            FrontsCatalog.FilterCatalogHeight(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);

            pcbxFrontImage.Image = null;
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int ConfigID = -1;
                if (cbFrontImage.Checked)
                {
                    ConfigID = FrontsCatalog.GetFrontAttachments(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                    if (ConfigID != -1)
                        pcbxFrontImage.Image = FrontsCatalog.GetFrontConfigImage(ConfigID);

                    if (pcbxFrontImage.Image == null)
                        pcbxFrontImage.Cursor = Cursors.Default;
                    else
                        pcbxFrontImage.Cursor = Cursors.Hand;
                }
                pcbxFrontTechStore.Image = null;
                if (cbFrontTechStore.Checked)
                {
                    if (FrontName.Length > 0)
                    {
                        int FrontID = FrontsCatalog.GetFrontID(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                        if (FrontID != -1)
                            pcbxFrontTechStore.Image = FrontsCatalog.GetTechStoreImage(FrontID);
                    }

                    if (pcbxFrontTechStore.Image == null)
                        pcbxFrontTechStore.Cursor = Cursors.Default;
                    else
                        pcbxFrontTechStore.Cursor = Cursors.Hand;
                }
                pcbxVisualConfig.Image = null;
                if (cbVisualConfig.Checked)
                {
                    if (FrontName.Length > 0)
                    {
                        layer = null;
                        backgr = null;
                        backgr2 = null;
                        newBitmap = null;
                        newBitmap2 = null;
                        ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
                        Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                        if (ConfigID != -1 && img != null)
                            layer = new Bitmap(img);
                        ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                        img = FrontsCatalog.GetInsetColorImage(ConfigID);
                        if (ConfigID != -1 && img != null)
                            backgr = new Bitmap(img);

                        ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                        img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                        if (ConfigID != -1 && img != null)
                            backgr2 = new Bitmap(img);

                        if (layer != null)
                        {
                            if (backgr != null)
                            {
                                if (backgr2 != null)
                                {
                                    int width = layer.Width;
                                    int height = layer.Height;
                                    int width1 = backgr.Width;
                                    int height1 = backgr.Height;
                                    int width2 = backgr2.Width;
                                    int height2 = backgr2.Height;
                                    newBitmap = new Bitmap(width, height);
                                    newBitmap2 = new Bitmap(width1, height1);
                                    using (var canvas = Graphics.FromImage(newBitmap2))
                                    {
                                        if (height2 >= height1)
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }
                                        else
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }

                                        int width3 = newBitmap2.Width;
                                        int height3 = newBitmap2.Height;
                                        using (var canvas1 = Graphics.FromImage(newBitmap))
                                        {
                                            if (height3 >= height)
                                            {
                                                canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                                canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                                canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                                canvas1.Save();
                                            }
                                            else
                                            {
                                                canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                                canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                                canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                                canvas1.Save();
                                            }
                                        }
                                    }

                                    try
                                    {
                                        pcbxVisualConfig.Image = newBitmap;
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                }
                                else
                                {
                                    int width = layer.Width;
                                    int height = layer.Height;
                                    int width1 = backgr.Width;
                                    int height1 = backgr.Height;
                                    newBitmap = new Bitmap(width, height);
                                    using (var canvas = Graphics.FromImage(newBitmap))
                                    {
                                        if (height1 >= height)
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }
                                        else
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }
                                    }

                                    try
                                    {
                                        pcbxVisualConfig.Image = newBitmap;
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                }
                            }
                            else
                                pcbxVisualConfig.Image = layer;
                        }
                        else
                        {
                            if (backgr != null)
                                pcbxVisualConfig.Image = backgr;
                        }
                        if (pcbxVisualConfig.Image == null)
                            pcbxVisualConfig.Cursor = Cursors.Default;
                        else
                            pcbxVisualConfig.Cursor = Cursors.Hand;
                    }

                }
                //cbFrontToSite.Checked = IsConfigImageToSite;
                //BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

            }
            else
            {
                int ConfigID = -1;

                if (cbFrontImage.Checked)
                {
                    ConfigID = FrontsCatalog.GetFrontAttachments(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                    if (ConfigID != -1)
                        pcbxFrontImage.Image = FrontsCatalog.GetFrontConfigImage(ConfigID);

                    if (pcbxFrontImage.Image == null)
                        pcbxFrontImage.Cursor = Cursors.Default;
                    else
                        pcbxFrontImage.Cursor = Cursors.Hand;
                }
                pcbxFrontTechStore.Image = null;
                if (cbFrontTechStore.Checked)
                {
                    if (FrontName.Length > 0)
                    {
                        int FrontID = FrontsCatalog.GetFrontID(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                        if (FrontID != -1)
                            pcbxFrontTechStore.Image = FrontsCatalog.GetTechStoreImage(FrontID);
                    }

                    if (pcbxFrontTechStore.Image == null)
                        pcbxFrontTechStore.Cursor = Cursors.Default;
                    else
                        pcbxFrontTechStore.Cursor = Cursors.Hand;
                }
                pcbxVisualConfig.Image = null;
                if (cbVisualConfig.Checked)
                {
                    if (FrontName.Length > 0)
                    {
                        layer = null;
                        backgr = null;
                        backgr2 = null;
                        newBitmap = null;
                        newBitmap2 = null;
                        ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
                        Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                        if (ConfigID != -1 && img != null)
                            layer = new Bitmap(img);
                        ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                        img = FrontsCatalog.GetInsetColorImage(ConfigID);
                        if (ConfigID != -1 && img != null)
                            backgr = new Bitmap(img);

                        ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                        img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                        if (ConfigID != -1 && img != null)
                            backgr2 = new Bitmap(img);

                        if (layer != null)
                        {
                            if (backgr != null)
                            {
                                if (backgr2 != null)
                                {
                                    int width = layer.Width;
                                    int height = layer.Height;
                                    int width1 = backgr.Width;
                                    int height1 = backgr.Height;
                                    int width2 = backgr2.Width;
                                    int height2 = backgr2.Height;
                                    newBitmap = new Bitmap(width, height);
                                    newBitmap2 = new Bitmap(width1, height1);
                                    using (var canvas = Graphics.FromImage(newBitmap2))
                                    {
                                        if (height2 >= height1)
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }
                                        else
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }

                                        int width3 = newBitmap2.Width;
                                        int height3 = newBitmap2.Height;
                                        using (var canvas1 = Graphics.FromImage(newBitmap))
                                        {
                                            if (height3 >= height)
                                            {
                                                canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                                canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                                canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                                canvas1.Save();
                                            }
                                            else
                                            {
                                                canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                                canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                                canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                                canvas1.Save();
                                            }
                                        }
                                    }

                                    try
                                    {
                                        pcbxVisualConfig.Image = newBitmap;
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                }
                                else
                                {
                                    int width = layer.Width;
                                    int height = layer.Height;
                                    int width1 = backgr.Width;
                                    int height1 = backgr.Height;
                                    newBitmap = new Bitmap(width, height);
                                    using (var canvas = Graphics.FromImage(newBitmap))
                                    {
                                        if (height1 >= height)
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }
                                        else
                                        {
                                            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                            canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                            canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                            canvas.Save();
                                        }
                                    }

                                    try
                                    {
                                        pcbxVisualConfig.Image = newBitmap;
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                }
                            }
                            else
                                pcbxVisualConfig.Image = layer;
                        }
                        else
                        {
                            if (backgr != null)
                                pcbxVisualConfig.Image = backgr;
                        }
                    }
                    if (pcbxVisualConfig.Image == null)
                        pcbxVisualConfig.Cursor = Cursors.Default;
                    else
                        pcbxVisualConfig.Cursor = Cursors.Hand;
                }
                //cbFrontToSite.Checked = IsConfigImageToSite;
            }
        }

        public void GetFrontWidth()
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;
            int Height = 0;
            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (FrontsHeightDataGrid.SelectedRows.Count > 0 && FrontsHeightDataGrid.SelectedRows[0].Cells["Height"].Value != DBNull.Value)
                Height = Convert.ToInt32(FrontsHeightDataGrid.SelectedRows[0].Cells["Height"].Value);

            FrontsCatalog.FilterCatalogWidth(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID, Height);
        }

        #endregion

        #region DECOR FUNCTIONS
        public void DecorCatalogBinding()
        {
            ProductsDataGrid.SelectionChanged -= ProductsDataGrid_SelectionChanged;
            DecorDataGrid.SelectionChanged -= DecorDataGrid_SelectionChanged;
            ColorsDataGrid.SelectionChanged -= ColorsDataGrid_SelectionChanged;
            DecorPatinaDataGrid.SelectionChanged -= DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;

            ProductsDataGrid.DataSource = DecorCatalog.DecorProductsBindingSource;
            DecorDataGrid.DataSource = DecorCatalog.DecorItemBindingSource;
            ColorsDataGrid.DataSource = DecorCatalog.ItemColorsBindingSource;
            DecorPatinaDataGrid.DataSource = DecorCatalog.ItemPatinaBindingSource;
            DecorInsetTypesDataGrid.DataSource = DecorCatalog.ItemInsetTypesBindingSource;
            DecorInsetColorsDataGrid.DataSource = DecorCatalog.ItemInsetColorsBindingSource;
            LengthDataGrid.DataSource = DecorCatalog.LengthBindingSource;
            HeightDataGrid.DataSource = DecorCatalog.HeightBindingSource;
            WidthDataGrid.DataSource = DecorCatalog.WidthBindingSource;

            GetDecorItems();
            GetDecorColors();
            GetDecorPatina();
            GetDecorInsetTypes();
            GetDecorInsetColors();
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            ProductsDataGrid.SelectionChanged += ProductsDataGrid_SelectionChanged;
            DecorDataGrid.SelectionChanged += DecorDataGrid_SelectionChanged;
            ColorsDataGrid.SelectionChanged += ColorsDataGrid_SelectionChanged;
            DecorPatinaDataGrid.SelectionChanged += DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void DecorGridsSettings()
        {
            ProductsDataGrid.Columns["ProductID"].Visible = false;
            ProductsDataGrid.Columns["MeasureID"].Visible = false;
            ProductsDataGrid.Columns["ReportParam"].Visible = false;

            //DecorDataGrid.Columns["ProductID"].Visible = false;
            //DecorDataGrid.Columns["DecorID"].Visible = false;

            ColorsDataGrid.Columns["ColorID"].Visible = false;
            //ColorsDataGrid.Columns["GroupID"].Visible = false;

            DecorPatinaDataGrid.Columns["PatinaID"].Visible = false;
            DecorInsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            DecorInsetColorsDataGrid.Columns["InsetColorID"].Visible = false;
            DecorInsetTypesDataGrid.Columns["GroupID"].Visible = false;
            DecorInsetColorsDataGrid.Columns["GroupID"].Visible = false;

            ProductsDataGrid.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColorsDataGrid.Columns["ColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorPatinaDataGrid.Columns["PatinaName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorInsetTypesDataGrid.Columns["InsetTypeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorInsetColorsDataGrid.Columns["InsetColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void GetDecorItems()
        {
            if (DecorCatalog != null)
                if (DecorCatalog.DecorProductsBindingSource.Count > 0)
                {
                    if (((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"] != DBNull.Value)
                    {
                        DecorCatalog.FilterItems(Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]));
                        DecorCatalog.DecorItemBindingSource.MoveFirst();
                    }
                }
        }

        public void GetDecorColors()
        {
            if (DecorCatalog.DecorItemBindingSource.Current == null)
                return;
            if (DecorCatalog.DecorItemBindingSource.Count > 0)
            {
                DecorCatalog.FilterColors(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString());
            }
        }

        public void GetDecorPatina()
        {
            if (DecorCatalog.DecorItemBindingSource.Current == null)
                return;
            if (DecorCatalog.ItemColorsBindingSource.Count > 0)
            {
                DecorCatalog.FilterPatina(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]));
            }
        }

        public void GetDecorInsetTypes()
        {
            if (DecorCatalog.DecorItemBindingSource.Current == null)
                return;
            if (DecorCatalog.ItemPatinaBindingSource.Count > 0)
            {
                DecorCatalog.FilterInsetType(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]));
            }
        }

        public void GetDecorInsetColors()
        {
            if (DecorCatalog.DecorItemBindingSource.Current == null)
                return;
            if (DecorCatalog.ItemInsetTypesBindingSource.Count > 0)
            {
                DecorCatalog.FilterInsetColor(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]));
            }
        }

        public void GetDecorLength()
        {
            if (DecorCatalog.DecorItemBindingSource.Current == null)
                return;
            if (DecorCatalog.ItemInsetColorsBindingSource.Count > 0)
            {

                DecorCatalog.FilterLength(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]));
            }
        }

        public void GetDecorHeight()
        {
            if (DecorCatalog.DecorProductsBindingSource.Current == null || DecorCatalog.DecorItemBindingSource.Current == null)
                return;
            if (DecorCatalog.LengthBindingSource.Count > 0)
            {
                int Length = Convert.ToInt32(((DataRowView)DecorCatalog.LengthBindingSource.Current)["Length"]);
                DecorCatalog.FilterHeight(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]), Length);
            }

            int ProductID = -1;
            string DecorName = string.Empty;
            int ColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;

            if ((DataRowView)DecorCatalog.DecorProductsBindingSource.Current != null)
                ProductID = Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]);
            if ((DataRowView)DecorCatalog.DecorItemBindingSource.Current != null)
                DecorName = ((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString();
            if ((DataRowView)DecorCatalog.ItemColorsBindingSource.Current != null)
                ColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]);
            if ((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current != null)
                PatinaID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]);
            if ((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current != null)
                InsetTypeID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]);
            if ((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current != null)
                InsetColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]);

            if (cbDecorTechStore.Checked)
            {
                //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                //T.Start();

                //while (!SplashWindow.bSmallCreated) ;

                //int DecorID = DecorCatalog.GetDecorID(ProductID, DecorName, ColorID, PatinaID);
                //if (DecorID != -1)
                //    pcbxDecorTechStore.Image = DecorCatalog.GetTechStoreImage(DecorID);

                //while (SplashWindow.bSmallCreated)
                //    SmallWaitForm.CloseS = true;

                int DecorID = DecorCatalog.GetDecorID(ProductID, DecorName, ColorID, PatinaID, InsetTypeID, InsetColorID);
                if (DecorID != -1)
                    pcbxDecorTechStore.Image = DecorCatalog.GetTechStoreImage(DecorID);

                if (pcbxDecorTechStore.Image == null)
                    pcbxDecorTechStore.Cursor = Cursors.Default;
                else
                    pcbxDecorTechStore.Cursor = Cursors.Hand;
            }
            pcbxDecorImage.Image = null;
            if (cbDecorImage.Checked)
            {
                //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                //T.Start();

                //while (!SplashWindow.bSmallCreated) ;

                //int ConfigID = DecorCatalog.SaveDecorAttachments(ProductID, DecorName, ColorID, PatinaID);
                //if (ConfigID != -1)
                //    pcbxDecorImage.Image = DecorCatalog.GetDecorConfigImage(ConfigID);

                //while (SplashWindow.bSmallCreated)
                //    SmallWaitForm.CloseS = true;

                int ConfigID = DecorCatalog.GetDecorAttachments(ProductID, DecorName, ColorID, PatinaID, InsetTypeID, InsetColorID);
                if (ConfigID != -1)
                    pcbxDecorImage.Image = DecorCatalog.GetDecorConfigImage(ConfigID, ProductID);

                if (pcbxDecorImage.Image == null)
                    pcbxDecorImage.Cursor = Cursors.Default;
                else
                    pcbxDecorImage.Cursor = Cursors.Hand;
            }
        }

        public void GetDecorWidth()
        {
            if (DecorCatalog.DecorItemBindingSource.Current == null)
                return;
            if (DecorCatalog.HeightBindingSource.Count > 0)
            {
                int Length = Convert.ToInt32(((DataRowView)DecorCatalog.LengthBindingSource.Current)["Length"]);
                int Height = Convert.ToInt32(((DataRowView)DecorCatalog.HeightBindingSource.Current)["Height"]);

                DecorCatalog.FilterWidth(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]), Length, Height);
            }

        }

        #endregion


        private void ProductsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DecorDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void DecorPatinaDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void LengthDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void HeightDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }


        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (FrontsCatalog == null || DecorCatalog == null)
                return;

            if (kryptonCheckSet1.CheckedButton.Name == "DecorCheckButton")
            {
                ProductsDataGrid.SelectionChanged -= ProductsDataGrid_SelectionChanged;
                DecorDataGrid.SelectionChanged -= DecorDataGrid_SelectionChanged;
                ColorsDataGrid.SelectionChanged -= ColorsDataGrid_SelectionChanged;
                DecorPatinaDataGrid.SelectionChanged -= DecorPatinaDataGrid_SelectionChanged;
                DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
                DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
                LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
                HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
                WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
                DecorCatalog.FilterProducts();
                GetDecorItems();
                GetDecorColors();
                GetDecorPatina();
                GetDecorInsetTypes();
                GetDecorInsetColors();
                GetDecorLength();
                GetDecorHeight();
                GetDecorWidth();
                ProductsDataGrid.SelectionChanged += ProductsDataGrid_SelectionChanged;
                DecorDataGrid.SelectionChanged += DecorDataGrid_SelectionChanged;
                ColorsDataGrid.SelectionChanged += ColorsDataGrid_SelectionChanged;
                DecorPatinaDataGrid.SelectionChanged += DecorPatinaDataGrid_SelectionChanged;
                DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
                DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
                LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
                HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
                WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
                DecorCatalogPanel.BringToFront();
            }
            else
            {
                FrontsDataGrid.SelectionChanged -= FrontsDataGrid_SelectionChanged;
                FrameColorsDataGrid.SelectionChanged -= FrameColorsDataGrid_SelectionChanged;
                TechnoFrameColorsDataGrid.SelectionChanged -= TechnoFrameColorsDataGrid_SelectionChanged;
                InsetTypesDataGrid.SelectionChanged -= InsetTypesDataGrid_SelectionChanged;
                InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
                TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
                TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
                PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
                FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;

                FrontsCatalog.FilterFronts();
                GetFrameColors();
                GetTechnoFrameColors();
                GetInsetTypes();
                GetInsetColors();
                GetTechnoInsetTypes();
                GetTechnoInsetColors();
                GetPatina();
                GetFrontHeight();
                GetFrontWidth();

                FrontsDataGrid.SelectionChanged += FrontsDataGrid_SelectionChanged;
                FrameColorsDataGrid.SelectionChanged += FrameColorsDataGrid_SelectionChanged;
                TechnoFrameColorsDataGrid.SelectionChanged += TechnoFrameColorsDataGrid_SelectionChanged;
                InsetTypesDataGrid.SelectionChanged += InsetTypesDataGrid_SelectionChanged;
                InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
                TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
                TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
                PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
                FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
                FrontsCatalogPanel.BringToFront();
            }
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (FrontsCatalog == null || DecorCatalog == null)
                return;

            if (kryptonCheckSet2.CheckedButton.Name == "ProfilCheckButton")
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                FrontsCatalog.FilterCatalog(1);
                DecorCatalog.FilterCatalog(1);

                FrontsDataGrid.SelectionChanged -= FrontsDataGrid_SelectionChanged;
                FrameColorsDataGrid.SelectionChanged -= FrameColorsDataGrid_SelectionChanged;
                TechnoFrameColorsDataGrid.SelectionChanged -= TechnoFrameColorsDataGrid_SelectionChanged;
                InsetTypesDataGrid.SelectionChanged -= InsetTypesDataGrid_SelectionChanged;
                InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
                TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
                TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
                PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
                FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
                FrontsCatalog.FilterFronts();
                GetFrameColors();
                GetTechnoFrameColors();
                GetInsetTypes();
                GetInsetColors();
                GetTechnoInsetTypes();
                GetTechnoInsetColors();
                GetPatina();
                GetFrontHeight();
                GetFrontWidth();
                FrontsDataGrid.SelectionChanged += FrontsDataGrid_SelectionChanged;
                FrameColorsDataGrid.SelectionChanged += FrameColorsDataGrid_SelectionChanged;
                TechnoFrameColorsDataGrid.SelectionChanged += TechnoFrameColorsDataGrid_SelectionChanged;
                InsetTypesDataGrid.SelectionChanged += InsetTypesDataGrid_SelectionChanged;
                InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
                TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
                TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
                PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
                FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;

                ProductsDataGrid.SelectionChanged -= ProductsDataGrid_SelectionChanged;
                DecorDataGrid.SelectionChanged -= DecorDataGrid_SelectionChanged;
                ColorsDataGrid.SelectionChanged -= ColorsDataGrid_SelectionChanged;
                DecorPatinaDataGrid.SelectionChanged -= DecorPatinaDataGrid_SelectionChanged;
                DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
                DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
                HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
                WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
                LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
                DecorCatalog.FilterProducts();
                GetDecorItems();
                GetDecorColors();
                GetDecorPatina();
                GetDecorInsetTypes();
                GetDecorInsetColors();
                GetDecorLength();
                GetDecorHeight();
                GetDecorWidth();
                ProductsDataGrid.SelectionChanged += ProductsDataGrid_SelectionChanged;
                DecorDataGrid.SelectionChanged += DecorDataGrid_SelectionChanged;
                ColorsDataGrid.SelectionChanged += ColorsDataGrid_SelectionChanged;
                DecorPatinaDataGrid.SelectionChanged += DecorPatinaDataGrid_SelectionChanged;
                DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
                DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
                HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
                WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
                LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                FrontsCatalog.FilterCatalog(2);
                DecorCatalog.FilterCatalog(2);

                FrontsDataGrid.SelectionChanged -= FrontsDataGrid_SelectionChanged;
                FrameColorsDataGrid.SelectionChanged -= FrameColorsDataGrid_SelectionChanged;
                TechnoFrameColorsDataGrid.SelectionChanged -= TechnoFrameColorsDataGrid_SelectionChanged;
                InsetTypesDataGrid.SelectionChanged -= InsetTypesDataGrid_SelectionChanged;
                InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
                TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
                TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
                PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
                FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
                FrontsCatalog.FilterFronts();
                GetFrameColors();
                GetTechnoFrameColors();
                GetInsetTypes();
                GetInsetColors();
                GetTechnoInsetTypes();
                GetTechnoInsetColors();
                GetPatina();
                GetFrontHeight();
                GetFrontWidth();
                FrontsDataGrid.SelectionChanged += FrontsDataGrid_SelectionChanged;
                FrameColorsDataGrid.SelectionChanged += FrameColorsDataGrid_SelectionChanged;
                TechnoFrameColorsDataGrid.SelectionChanged += TechnoFrameColorsDataGrid_SelectionChanged;
                InsetTypesDataGrid.SelectionChanged += InsetTypesDataGrid_SelectionChanged;
                InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
                TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
                TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
                PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
                FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;

                ProductsDataGrid.SelectionChanged -= ProductsDataGrid_SelectionChanged;
                DecorDataGrid.SelectionChanged -= DecorDataGrid_SelectionChanged;
                ColorsDataGrid.SelectionChanged -= ColorsDataGrid_SelectionChanged;
                DecorPatinaDataGrid.SelectionChanged -= DecorPatinaDataGrid_SelectionChanged;
                DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
                DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
                HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
                WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
                LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
                DecorCatalog.FilterProducts();
                GetDecorItems();
                GetDecorColors();
                GetDecorPatina();
                GetDecorInsetTypes();
                GetDecorInsetColors();
                GetDecorLength();
                GetDecorHeight();
                GetDecorWidth();
                ProductsDataGrid.SelectionChanged += ProductsDataGrid_SelectionChanged;
                DecorDataGrid.SelectionChanged += DecorDataGrid_SelectionChanged;
                ColorsDataGrid.SelectionChanged += ColorsDataGrid_SelectionChanged;
                DecorPatinaDataGrid.SelectionChanged += DecorPatinaDataGrid_SelectionChanged;
                DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
                DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
                HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
                WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
                LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void AddPictureButton_Click(object sender, EventArgs e)
        {
            if (DecorDataGrid.SelectedRows.Count == 0)
                return;

            PictureEditForm PictureEditForm = new PictureEditForm(ref DecorCatalog, Convert.ToInt32(DecorDataGrid.SelectedRows[0].Cells["DecorID"].Value));

            TopForm = PictureEditForm;

            PictureEditForm.ShowDialog();

            TopForm = null;

            PictureEditForm.Dispose();
        }

        private void PrintPackageContextMenuItem_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            SampleFrontsLabelInfoMenu SampleLabelInfoMenu = new SampleFrontsLabelInfoMenu(this);
            TopForm = SampleLabelInfoMenu;
            SampleLabelInfoMenu.ShowDialog();
            PressOK = SampleLabelInfoMenu.PressOK;
            LabelsCount = SampleLabelInfoMenu.LabelsCount;
            PositionsCount = SampleLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            SampleLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int FrontID = -1;
            int FactoryID = -1;
            string Front = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].FormattedValue.ToString();
            string FrameColor = FrameColorsDataGrid.SelectedRows[0].Cells["ColorName"].FormattedValue.ToString();
            string Patina = PatinaDataGrid.SelectedRows[0].Cells["PatinaName"].FormattedValue.ToString();
            string InsetType = InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeName"].FormattedValue.ToString();
            string InsetColor = InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorName"].FormattedValue.ToString();
            string TechnoInsetType = TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeName"].FormattedValue.ToString();
            string TechnoInsetColor = TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorName"].FormattedValue.ToString();

            if (Patina != "-")
                FrameColor += "/" + Patina;
            if (TechnoInsetType != "-")
                InsetType += "/" + TechnoInsetType;
            if (TechnoInsetColor != "-")
                InsetColor += "/" + TechnoInsetColor;

            int Height = 0;
            int Width = 0;
            int FrontConfigID = 0;

            if (FrontsCatalog.HeightBindingSource.Count > 0)
                Height = Convert.ToInt32(((DataRowView)FrontsCatalog.HeightBindingSource.Current).Row["Height"]);

            if (FrontsCatalog.WidthBindingSource.Count > 0)
                Width = Convert.ToInt32(((DataRowView)FrontsCatalog.WidthBindingSource.Current).Row["Width"]);

            if (FrontsCatalog != null)
                if (FrontsCatalog.WidthBindingSource.Count > 0)
                {
                    if (((DataRowView)FrontsCatalog.WidthBindingSource.Current)["Width"] != DBNull.Value)
                    {
                        FrontConfigID = FrontsCatalog.GetFrontConfigID(((DataRowView)FrontsCatalog.FrontsBindingSource.Current)["FrontName"].ToString(),
                            Convert.ToInt32(((DataRowView)FrontsCatalog.FrameColorsBindingSource.Current)["ColorID"]),
                            Convert.ToInt32(((DataRowView)FrontsCatalog.TechnoFrameColorsBindingSource.Current)["ColorID"]),
                            Convert.ToInt32(((DataRowView)FrontsCatalog.PatinaBindingSource.Current)["PatinaID"]),
                            Convert.ToInt32(((DataRowView)FrontsCatalog.InsetTypesBindingSource.Current)["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)FrontsCatalog.InsetColorsBindingSource.Current)["InsetColorID"]),
                            Convert.ToInt32(((DataRowView)FrontsCatalog.TechnoInsetTypesBindingSource.Current)["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)FrontsCatalog.TechnoInsetColorsBindingSource.Current)["InsetColorID"]),
                            Height, Width, ref FrontID, ref FactoryID);
                    }
                }

            DataRow NewRow = FrontsDT.NewRow();

            NewRow["FrameColor"] = FrameColor;
            NewRow["InsetType"] = InsetType;
            NewRow["InsetColor"] = InsetColor;
            NewRow["Front"] = Front;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["FrontConfigID"] = FrontConfigID;
            NewRow["FrontID"] = FrontID;
            NewRow["ColorID"] = Convert.ToInt32(((DataRowView)FrontsCatalog.FrameColorsBindingSource.Current)["ColorID"]);
            NewRow["TechnoColorID"] = Convert.ToInt32(((DataRowView)FrontsCatalog.TechnoFrameColorsBindingSource.Current)["ColorID"]);

            if (kryptonCheckSet2.CheckedIndex == 0)
                NewRow["FactoryType"] = 2;
            if (kryptonCheckSet2.CheckedIndex == 1)
                NewRow["FactoryType"] = 1;
            FrontsDT.Rows.Add(NewRow);
        }

        private void KryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            SampleLabelManager.ClearOrderData();
            FrontsDT.Clear();
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            SampleLabelManager.ClearLabelInfo();

            //Проверка
            if (FrontsDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < FrontsDT.Rows.Count; i++)
            {
                int LabelsCount = Convert.ToInt32(FrontsDT.Rows[i]["LabelsCount"]);
                int FrontConfigID = Convert.ToInt32(FrontsDT.Rows[i]["FrontConfigID"]);
                int FrontID = 0;
                int ColorID = 0;
                int TechnoColorID = 0;
                int SampleLabelID = 0;
                if (FrontsDT.Rows[i]["FrontID"] != DBNull.Value)
                    FrontID = Convert.ToInt32(FrontsDT.Rows[i]["FrontID"]);
                if (FrontsDT.Rows[i]["ColorID"] != DBNull.Value)
                    ColorID = Convert.ToInt32(FrontsDT.Rows[i]["ColorID"]);
                if (FrontsDT.Rows[i]["TechnoColorID"] != DBNull.Value)
                    TechnoColorID = Convert.ToInt32(FrontsDT.Rows[i]["TechnoColorID"]);
                for (int j = 0; j < LabelsCount; j++)
                {
                    SampleInfo LabelInfo = new SampleInfo();

                    DataTable DT = FrontsDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in FrontsDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = FrontsDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.ProductType = 0;
                    LabelInfo.Impost = string.Empty;
                    if ((FrontID == 3630 || FrontID == 15003) && (ColorID == TechnoColorID))
                        LabelInfo.Impost = " с импостом";
                    SampleLabelID = SampleLabelManager.SaveSampleLabel(FrontConfigID, DateTime.Now, Security.CurrentUserID, 0);
                    LabelInfo.BarcodeNumber = SampleLabelManager.GetBarcodeNumber(17, SampleLabelID);
                    LabelInfo.FactoryType = Convert.ToInt32(FrontsDT.Rows[i]["FactoryType"]);
                    LabelInfo.DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    SampleLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = SampleLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                SampleLabelManager.Print();
            }
        }

        private void FrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (bCanEdit && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            string Front = string.Empty;
            string FrameColor = string.Empty;
            string Patina = string.Empty;
            string InsetType = string.Empty;
            string InsetColor = string.Empty;
            string TechnoInsetType = string.Empty;
            string TechnoInsetColor = string.Empty;

            string Height = string.Empty;
            string Width = string.Empty;

            bool PressOK = false;
            string LabelsCount = string.Empty;
            string PositionsCount = string.Empty;
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            SampleFrontsManually SampleLabelInfoMenu = new SampleFrontsManually(this);
            TopForm = SampleLabelInfoMenu;
            SampleLabelInfoMenu.ShowDialog();
            Front = SampleLabelInfoMenu.Front;
            FrameColor = SampleLabelInfoMenu.FrameColor;
            Patina = SampleLabelInfoMenu.Patina;
            InsetType = SampleLabelInfoMenu.InsetType;
            InsetColor = SampleLabelInfoMenu.InsetColor;
            TechnoInsetType = SampleLabelInfoMenu.TechnoInsetType;
            TechnoInsetColor = SampleLabelInfoMenu.TechnoInsetColor;
            Height = SampleLabelInfoMenu.LabelsHeight;
            Width = SampleLabelInfoMenu.LabelsWidth;
            LabelsCount = SampleLabelInfoMenu.LabelsCount;
            PositionsCount = SampleLabelInfoMenu.PositionsCount;
            PressOK = SampleLabelInfoMenu.PressOK;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SampleLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;
            if (LabelsCount.Length == 0)
                return;
            int FrontConfigID = 0;
            DataRow NewRow = FrontsDT.NewRow();

            NewRow["FrameColor"] = FrameColor;
            NewRow["InsetType"] = InsetType;
            NewRow["InsetColor"] = InsetColor;
            NewRow["Front"] = Front;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["FrontConfigID"] = FrontConfigID;
            //NewRow["FrontID"] = Convert.ToInt32(((DataRowView)FrontsCatalog.FrontsBindingSource.Current)["FrontID"]);
            //NewRow["ColorID"] = Convert.ToInt32(((DataRowView)FrontsCatalog.FrameColorsBindingSource.Current)["ColorID"]);
            //NewRow["TechnoColorID"] = Convert.ToInt32(((DataRowView)FrontsCatalog.TechnoFrameColorsBindingSource.Current)["ColorID"]);

            if (kryptonCheckSet2.CheckedIndex == 0)
                NewRow["FactoryType"] = 2;
            if (kryptonCheckSet2.CheckedIndex == 1)
                NewRow["FactoryType"] = 1;
            FrontsDT.Rows.Add(NewRow);
        }

        private void ProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (bCanEdit && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
                int ProductID = -1;
                if (ProductsDataGrid.SelectedRows.Count > 0 && ProductsDataGrid.SelectedRows[0].Cells["ProductID"].Value != DBNull.Value)
                    ProductID = Convert.ToInt32(ProductsDataGrid.SelectedRows[0].Cells["ProductID"].Value);
                kryptonContextMenuItem21.Enabled = false;
                kryptonContextMenuItem22.Enabled = false;
                kryptonContextMenuItem23.Enabled = false;
                kryptonContextMenuItem24.Enabled = false;
                kryptonContextMenuItem25.Enabled = false;
                kryptonContextMenuItem26.Enabled = false;
                kryptonContextMenuItem28.Enabled = false;
                kryptonContextMenuItem29.Enabled = false;
                if (ProductID == 46) // патриция 1.x
                {
                    kryptonContextMenuItem21.Enabled = true;
                }
                if (ProductID == 61) // куб
                {
                    kryptonContextMenuItem22.Enabled = true;
                }
                if (ProductID == 63) // норманн
                {
                    kryptonContextMenuItem23.Enabled = true;
                }
                if (ProductID == 73) // патриция 1.0
                {
                    kryptonContextMenuItem24.Enabled = true;
                }
                if (ProductID == 74) // патриция 2.x
                {
                    kryptonContextMenuItem25.Enabled = true;
                }
                if (ProductID == 75) // патриция 3.x
                {
                    kryptonContextMenuItem26.Enabled = true;
                }
                if (ProductID == 80) // ПАТРИЦИЯ-4.0 (С)
                {
                    kryptonContextMenuItem28.Enabled = true;
                }
                if (ProductID == 82) // КРИСМАР
                {
                    kryptonContextMenuItem29.Enabled = true;
                }
            }
        }

        private void kryptonContextMenuItem8_Click(object sender, EventArgs e)
        {
            int DecorConfigID = 0;
            bool PressOK = false;

            string LabelsCount = string.Empty;
            string PositionsCount = string.Empty;
            string LabelsLength = string.Empty;
            string LabelsHeight = string.Empty;
            string LabelsWidth = string.Empty;
            string Product = string.Empty;
            string Decor = string.Empty;
            string Color = string.Empty;
            string Patina = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            SampleDecorManually SampleLabelInfo = new SampleDecorManually(this);
            TopForm = SampleLabelInfo;
            SampleLabelInfo.ShowDialog();
            PressOK = SampleLabelInfo.PressOK;

            Product = SampleLabelInfo.Product;
            Decor = SampleLabelInfo.Decor;
            Color = SampleLabelInfo.Color;
            Patina = SampleLabelInfo.Patina;
            LabelsCount = SampleLabelInfo.LabelsCount;
            LabelsLength = SampleLabelInfo.LabelsLength;
            LabelsHeight = SampleLabelInfo.LabelsHeight;
            LabelsWidth = SampleLabelInfo.LabelsWidth;
            PositionsCount = SampleLabelInfo.PositionsCount;

            if (Patina.Length > 0 || Patina != "-")
                Color += " " + Patina;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SampleLabelInfo.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            DataRow NewRow = DecorDT.NewRow();

            NewRow["Decor"] = Decor;
            NewRow["Color"] = Color;
            NewRow["Product"] = Product;
            NewRow["Length"] = LabelsLength.ToString();
            NewRow["Height"] = LabelsHeight.ToString();
            NewRow["Width"] = LabelsWidth.ToString();
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            if (kryptonCheckSet2.CheckedIndex == 0)
                NewRow["FactoryType"] = 2;
            if (kryptonCheckSet2.CheckedIndex == 1)
                NewRow["FactoryType"] = 1;
            DecorDT.Rows.Add(NewRow);
        }

        private void ColorsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (bCanEdit && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
                int ProductID = -1;
                if (ProductsDataGrid.SelectedRows.Count > 0 && ProductsDataGrid.SelectedRows[0].Cells["ProductID"].Value != DBNull.Value)
                    ProductID = Convert.ToInt32(ProductsDataGrid.SelectedRows[0].Cells["ProductID"].Value);
                kryptonContextMenuItem21.Enabled = false;
                kryptonContextMenuItem22.Enabled = false;
                kryptonContextMenuItem23.Enabled = false;
                kryptonContextMenuItem24.Enabled = false;
                kryptonContextMenuItem25.Enabled = false;
                kryptonContextMenuItem26.Enabled = false;
                kryptonContextMenuItem28.Enabled = false;
                kryptonContextMenuItem29.Enabled = false;
                if (ProductID == 46) // патриция 1.x
                {
                    kryptonContextMenuItem21.Enabled = true;
                }
                if (ProductID == 61) // куб
                {
                    kryptonContextMenuItem22.Enabled = true;
                }
                if (ProductID == 63) // норманн
                {
                    kryptonContextMenuItem23.Enabled = true;
                }
                if (ProductID == 73) // патриция 1.0
                {
                    kryptonContextMenuItem24.Enabled = true;
                }
                if (ProductID == 74) // патриция 2.x
                {
                    kryptonContextMenuItem25.Enabled = true;
                }
                if (ProductID == 75) // патриция 3.x
                {
                    kryptonContextMenuItem26.Enabled = true;
                }
                if (ProductID == 80) // ПАТРИЦИЯ-4.0 (С)
                {
                    kryptonContextMenuItem28.Enabled = true;
                }
                if (ProductID == 82) // КРИСМАР
                {
                    kryptonContextMenuItem29.Enabled = true;
                }
            }
        }

        private void DecorDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (bCanEdit && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));

                int ProductID = -1;
                if (ProductsDataGrid.SelectedRows.Count > 0 && ProductsDataGrid.SelectedRows[0].Cells["ProductID"].Value != DBNull.Value)
                    ProductID = Convert.ToInt32(ProductsDataGrid.SelectedRows[0].Cells["ProductID"].Value);
                kryptonContextMenuItem21.Enabled = false;
                kryptonContextMenuItem22.Enabled = false;
                kryptonContextMenuItem23.Enabled = false;
                kryptonContextMenuItem24.Enabled = false;
                kryptonContextMenuItem25.Enabled = false;
                kryptonContextMenuItem26.Enabled = false;
                kryptonContextMenuItem28.Enabled = false;
                kryptonContextMenuItem29.Enabled = false;
                if (ProductID == 46) // патриция 1.x
                {
                    kryptonContextMenuItem21.Enabled = true;
                }
                if (ProductID == 61) // куб
                {
                    kryptonContextMenuItem22.Enabled = true;
                }
                if (ProductID == 63) // норманн
                {
                    kryptonContextMenuItem23.Enabled = true;
                }
                if (ProductID == 73) // патриция 1.0
                {
                    kryptonContextMenuItem24.Enabled = true;
                }
                if (ProductID == 74) // патриция 2.x
                {
                    kryptonContextMenuItem25.Enabled = true;
                }
                if (ProductID == 75) // патриция 3.x
                {
                    kryptonContextMenuItem26.Enabled = true;
                }
                if (ProductID == 80) // ПАТРИЦИЯ-4.0 (С)
                {
                    kryptonContextMenuItem28.Enabled = true;
                }
                if (ProductID == 82) // КРИСМАР
                {
                    kryptonContextMenuItem29.Enabled = true;
                }
            }
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            int Length = 0;
            int Height = 0;
            int Width = 0;
            int DecorConfigID = 0;

            if (DecorCatalog.LengthBindingSource.Count > 0)
                Length = Convert.ToInt32(((DataRowView)DecorCatalog.LengthBindingSource.Current).Row["Length"]);

            if (DecorCatalog.HeightBindingSource.Count > 0)
                Height = Convert.ToInt32(((DataRowView)DecorCatalog.HeightBindingSource.Current).Row["Height"]);

            if (DecorCatalog.WidthBindingSource.Count > 0)
                Width = Convert.ToInt32(((DataRowView)DecorCatalog.WidthBindingSource.Current).Row["Width"]);

            bool bNeedLength = false;
            bool bNeedHeight = false;
            bool bNeedWidth = false;
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            int LabelsLength = 0;
            int LabelsHeight = 0;
            int LabelsWidth = 0;
            if (Length == 0)
                bNeedLength = true;
            if (Height == 0)
                bNeedHeight = true;
            if (Width == 0)
                bNeedWidth = true;
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            SampleDecorLabelInfoMenu SampleLabelInfoMenu = new SampleDecorLabelInfoMenu(this, bNeedLength, bNeedHeight, bNeedWidth);
            TopForm = SampleLabelInfoMenu;
            SampleLabelInfoMenu.ShowDialog();
            PressOK = SampleLabelInfoMenu.PressOK;
            LabelsCount = SampleLabelInfoMenu.LabelsCount;
            LabelsLength = SampleLabelInfoMenu.LabelsLength;
            LabelsHeight = SampleLabelInfoMenu.LabelsHeight;
            LabelsWidth = SampleLabelInfoMenu.LabelsWidth;
            PositionsCount = SampleLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            SampleLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            if (Length > 0)
                LabelsLength = Length;
            if (Height > 0)
                LabelsHeight = Height;
            if (Width > 0)
                LabelsWidth = Width;
            string Product = ProductsDataGrid.SelectedRows[0].Cells["ProductName"].Value.ToString();
            string Decor = DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString();
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();
            string Patina = DecorPatinaDataGrid.SelectedRows[0].Cells["PatinaName"].Value.ToString();
            if (Patina.Length > 0 && Patina != "-")
                Color += " " + Patina;

            if (DecorCatalog != null)
                if (DecorCatalog.WidthBindingSource.Count > 0)
                {
                    if (((DataRowView)DecorCatalog.WidthBindingSource.Current)["Width"] != DBNull.Value)
                    {
                        DecorConfigID = DecorCatalog.GetDecorConfigID(Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]),
                                                      ((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                                                      Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                                                      Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]),
                                                      Length, Height, Width);
                    }
                }

            DataRow NewRow = DecorDT.NewRow();

            NewRow["Decor"] = Decor;
            NewRow["Color"] = Color;
            NewRow["Product"] = Product;
            NewRow["Length"] = LabelsLength.ToString();
            NewRow["Height"] = LabelsHeight.ToString();
            NewRow["Width"] = LabelsWidth.ToString();
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            if (kryptonCheckSet2.CheckedIndex == 0)
                NewRow["FactoryType"] = 2;
            if (kryptonCheckSet2.CheckedIndex == 1)
                NewRow["FactoryType"] = 1;
            DecorDT.Rows.Add(NewRow);
            //DecorDT = DecorCatalog.GetBagetDT();
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            SampleLabelManager.ClearOrderData();
            DecorDT.Clear();
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {
            SampleLabelManager.ClearLabelInfo();

            //Проверка
            if (DecorDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < DecorDT.Rows.Count; i++)
            {
                int LabelsCount = Convert.ToInt32(DecorDT.Rows[i]["LabelsCount"]);
                int DecorConfigID = Convert.ToInt32(DecorDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    SampleInfo LabelInfo = new SampleInfo();

                    DataTable DT = DecorDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in DecorDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = DecorDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.ProductType = 1;
                    SampleLabelID = SampleLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 1);
                    LabelInfo.BarcodeNumber = SampleLabelManager.GetBarcodeNumber(17, SampleLabelID);
                    LabelInfo.FactoryType = Convert.ToInt32(DecorDT.Rows[i]["FactoryType"]);
                    LabelInfo.DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    SampleLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = SampleLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                SampleLabelManager.Print();
            }
        }

        private void FrontsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void FrameColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void TechnoFrameColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void PatinaDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void InsetTypesDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void InsetColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void TechnoInsetTypesDataGridDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void TechnoInsetColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void FrontsHeightDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void openFileDialog4_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog4.FileName);

            //if (fileInfo.Length > 1500000)
            //{
            //    MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
            //    return;
            //}
            AttachmentsDT.Clear();

            DataRow NewRow = AttachmentsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog4.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog4.FileName;
            AttachmentsDT.Rows.Add(NewRow);

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            string Description = FrontsCatalog.GetFrontDescription(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
            //if (Description.Length == 0)
            //{
            //    MessageBox.Show("Отсутствует Description");
            //    return;
            //}
            int ConfigID = FrontsCatalog.SaveFrontAttachments(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
            if (ConfigID != -1)
                FrontsCatalog.AttachConfigImage(AttachmentsDT, ConfigID, 0, Description, FrontName,
                    FrameColorsDataGrid.SelectedRows[0].Cells["ColorName"].FormattedValue.ToString(),
                    PatinaDataGrid.SelectedRows[0].Cells["PatinaName"].FormattedValue.ToString());

            pcbxFrontImage.Image = null;

            if (cbFrontImage.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                if (ConfigID != -1)
                    pcbxFrontImage.Image = FrontsCatalog.GetFrontConfigImage(ConfigID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxFrontImage.Image == null)
                    pcbxFrontImage.Cursor = Cursors.Default;
                else
                    pcbxFrontImage.Cursor = Cursors.Hand;
            }
        }

        private void cbFrontImage_CheckedChanged(object sender, EventArgs e)
        {
            if (FrontsCatalog == null)
                return;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            pcbxFrontImage.Image = null;

            if (cbFrontImage.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int ConfigID = FrontsCatalog.GetFrontAttachments(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                if (ConfigID != -1)
                    pcbxFrontImage.Image = FrontsCatalog.GetFrontConfigImage(ConfigID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxFrontImage.Image == null)
                    pcbxFrontImage.Cursor = Cursors.Default;
                else
                    pcbxFrontImage.Cursor = Cursors.Hand;
            }
        }

        private void pcbxFrontImage_Click(object sender, EventArgs e)
        {
            if (pcbxFrontImage.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxFrontImage.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void btnAddFrontImageButton_Click(object sender, EventArgs e)
        {
            if (FrontsDataGrid.SelectedRows.Count == 0)
                return;

            openFileDialog4.ShowDialog();
        }

        private void btnRemoveFrontImageButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                   "Вы уверены, что хотите удалить?",
                   "Удаление");

            if (!OKCancel)
                return;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            int ConfigID = FrontsCatalog.GetFrontAttachments(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
            if (ConfigID != -1)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                FrontsCatalog.DetachConfigImage(ConfigID);
                pcbxFrontImage.Image = FrontsCatalog.GetFrontConfigImage(ConfigID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxFrontImage.Image == null)
                    pcbxFrontImage.Cursor = Cursors.Default;
                else
                    pcbxFrontImage.Cursor = Cursors.Hand;
            }
        }

        private void cbFrontTechStore_CheckedChanged(object sender, EventArgs e)
        {
            if (FrontsCatalog == null)
                return;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            pcbxFrontTechStore.Image = null;

            if (cbFrontTechStore.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int FrontID = FrontsCatalog.GetFrontID(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                if (FrontID != -1)
                    pcbxFrontTechStore.Image = FrontsCatalog.GetTechStoreImage(FrontID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxFrontTechStore.Image == null)
                    pcbxFrontTechStore.Cursor = Cursors.Default;
                else
                    pcbxFrontTechStore.Cursor = Cursors.Hand;
            }
        }

        private void pcbxFrontTechStore_Click(object sender, EventArgs e)
        {
            if (pcbxFrontTechStore.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxFrontTechStore.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void btnAddDecorImageButton_Click(object sender, EventArgs e)
        {
            if (ProductsDataGrid.SelectedRows.Count == 0)
                return;

            openFileDialog1.ShowDialog();
        }

        private void btnRemoveDecorImageButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                   "Вы уверены, что хотите удалить?",
                   "Удаление");

            if (!OKCancel)
                return;

            int ProductID = Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]);
            string DecorName = ((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString();
            int ColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]);

            int ConfigID = DecorCatalog.GetDecorAttachments(ProductID, DecorName, ColorID, PatinaID, InsetTypeID, InsetColorID);
            if (ConfigID != -1)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                DecorCatalog.DetachConfigImage(ConfigID);
                pcbxDecorImage.Image = DecorCatalog.GetDecorConfigImage(ConfigID, ProductID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxDecorImage.Image == null)
                    pcbxDecorImage.Cursor = Cursors.Default;
                else
                    pcbxDecorImage.Cursor = Cursors.Hand;
            }
        }

        private void pcbxDecorImage_Click(object sender, EventArgs e)
        {
            if (pcbxDecorImage.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxDecorImage.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void pcbxDecorTechStore_Click(object sender, EventArgs e)
        {
            if (pcbxDecorTechStore.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxDecorTechStore.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void cbDecorImage_CheckedChanged(object sender, EventArgs e)
        {
            if (DecorCatalog == null)
                return;

            int ProductID = Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]);
            string DecorName = ((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString();
            int ColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]);

            pcbxDecorImage.Image = null;

            if (cbDecorImage.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int ConfigID = DecorCatalog.GetDecorAttachments(ProductID, DecorName, ColorID, PatinaID, InsetTypeID, InsetColorID);
                if (ConfigID != -1)
                    pcbxDecorImage.Image = DecorCatalog.GetDecorConfigImage(ConfigID, ProductID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxDecorImage.Image == null)
                    pcbxDecorImage.Cursor = Cursors.Default;
                else
                    pcbxDecorImage.Cursor = Cursors.Hand;
            }
        }

        private void cbDecorTechStore_CheckedChanged(object sender, EventArgs e)
        {
            if (DecorCatalog == null)
                return;

            int ProductID = Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]);
            string DecorName = ((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString();
            int ColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]);

            pcbxDecorTechStore.Image = null;

            if (cbDecorTechStore.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int DecorID = DecorCatalog.GetDecorID(ProductID, DecorName, ColorID, PatinaID, InsetTypeID, InsetColorID);
                if (DecorID != -1)
                    pcbxDecorTechStore.Image = DecorCatalog.GetTechStoreImage(DecorID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxDecorTechStore.Image == null)
                    pcbxDecorTechStore.Cursor = Cursors.Default;
                else
                    pcbxDecorTechStore.Cursor = Cursors.Hand;
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog1.FileName);

            //if (fileInfo.Length > 1500000)
            //{
            //    MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
            //    return;
            //}
            AttachmentsDT.Clear();

            DataRow NewRow = AttachmentsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog1.FileName;
            AttachmentsDT.Rows.Add(NewRow);

            int ProductID = Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]);
            string DecorName = ((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString();
            int ColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]);
            int ProductType = 1;

            //if (ProductID == 46 || ProductID == 61 || ProductID == 62 || ProductID == 63)
            if (CheckOrdersStatus.IsCabFurniture(ProductID))
                ProductType = 2;
            int ConfigID = DecorCatalog.SaveDecorAttachments(ProductID, DecorName, ColorID, PatinaID, InsetTypeID, InsetColorID);
            if (ConfigID != -1)
                DecorCatalog.AttachConfigImage(AttachmentsDT, ConfigID, ProductType, ((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductName"].ToString(),
                    DecorName, ((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorName"].ToString(),
                    ((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaName"].ToString(),
                    ((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorName"].ToString());

            pcbxDecorImage.Image = null;
            if (cbDecorImage.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                if (ConfigID != -1)
                    pcbxDecorImage.Image = DecorCatalog.GetDecorConfigImage(ConfigID, ProductID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxDecorImage.Image == null)
                    pcbxDecorImage.Cursor = Cursors.Default;
                else
                    pcbxDecorImage.Cursor = Cursors.Hand;
            }
        }

        private void FrontsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            FrameColorsDataGrid.SelectionChanged -= FrameColorsDataGrid_SelectionChanged;
            TechnoFrameColorsDataGrid.SelectionChanged -= TechnoFrameColorsDataGrid_SelectionChanged;
            InsetTypesDataGrid.SelectionChanged -= InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetFrameColors();
            GetTechnoFrameColors();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();
            FrameColorsDataGrid.SelectionChanged += FrameColorsDataGrid_SelectionChanged;
            TechnoFrameColorsDataGrid.SelectionChanged += TechnoFrameColorsDataGrid_SelectionChanged;
            InsetTypesDataGrid.SelectionChanged += InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void FrameColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            TechnoFrameColorsDataGrid.SelectionChanged -= TechnoFrameColorsDataGrid_SelectionChanged;
            InsetTypesDataGrid.SelectionChanged -= InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetTechnoFrameColors();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();
            TechnoFrameColorsDataGrid.SelectionChanged += TechnoFrameColorsDataGrid_SelectionChanged;
            InsetTypesDataGrid.SelectionChanged += InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void TechnoFrameColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            InsetTypesDataGrid.SelectionChanged -= InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();
            InsetTypesDataGrid.SelectionChanged += InsetTypesDataGrid_SelectionChanged;
            InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void InsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            InsetColorsDataGrid.SelectionChanged -= InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();
            InsetColorsDataGrid.SelectionChanged += InsetColorsDataGrid_SelectionChanged;
            TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void InsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            TechnoInsetTypesDataGrid.SelectionChanged -= TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();
            TechnoInsetTypesDataGrid.SelectionChanged += TechnoInsetTypesDataGrid_SelectionChanged;
            TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void TechnoInsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            TechnoInsetColorsDataGrid.SelectionChanged -= TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetTechnoInsetColors();
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();
            TechnoInsetColorsDataGrid.SelectionChanged += TechnoInsetColorsDataGrid_SelectionChanged;
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void TechnoInsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            PatinaDataGrid.SelectionChanged -= PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetPatina();
            GetFrontHeight();
            GetFrontWidth();
            PatinaDataGrid.SelectionChanged += PatinaDataGrid_SelectionChanged;
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void PatinaDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            FrontsHeightDataGrid.SelectionChanged -= FrontsHeightDataGrid_SelectionChanged;
            GetFrontHeight();
            GetFrontWidth();
            FrontsHeightDataGrid.SelectionChanged += FrontsHeightDataGrid_SelectionChanged;
        }

        private void FrontsHeightDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            GetFrontWidth();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            DecorDataGrid.SelectionChanged -= DecorDataGrid_SelectionChanged;
            ColorsDataGrid.SelectionChanged -= ColorsDataGrid_SelectionChanged;
            DecorPatinaDataGrid.SelectionChanged -= DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorItems();
            GetDecorColors();
            GetDecorPatina();
            GetDecorInsetTypes();
            GetDecorInsetColors();
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            DecorDataGrid.SelectionChanged += DecorDataGrid_SelectionChanged;
            ColorsDataGrid.SelectionChanged += ColorsDataGrid_SelectionChanged;
            DecorPatinaDataGrid.SelectionChanged += DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void DecorDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            ColorsDataGrid.SelectionChanged -= ColorsDataGrid_SelectionChanged;
            DecorPatinaDataGrid.SelectionChanged -= DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorColors();
            GetDecorPatina();
            GetDecorInsetTypes();
            GetDecorInsetColors();
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            ColorsDataGrid.SelectionChanged += ColorsDataGrid_SelectionChanged;
            DecorPatinaDataGrid.SelectionChanged += DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void ColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            DecorPatinaDataGrid.SelectionChanged -= DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorPatina();
            GetDecorInsetTypes();
            GetDecorInsetColors();
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            DecorPatinaDataGrid.SelectionChanged += DecorPatinaDataGrid_SelectionChanged;
            DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void DecorPatinaDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            DecorInsetTypesDataGrid.SelectionChanged -= DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorInsetTypes();
            GetDecorInsetColors();
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            DecorInsetTypesDataGrid.SelectionChanged += DecorInsetTypesDataGrid_SelectionChanged;
            DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void DecorInsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            DecorInsetColorsDataGrid.SelectionChanged -= DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorInsetColors();
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            DecorInsetColorsDataGrid.SelectionChanged += DecorInsetColorsDataGrid_SelectionChanged;
            LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }


        private void DecorInsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            LengthDataGrid.SelectionChanged -= LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            LengthDataGrid.SelectionChanged += LengthDataGrid_SelectionChanged;
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void LengthDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            HeightDataGrid.SelectionChanged -= HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorHeight();
            GetDecorWidth();
            HeightDataGrid.SelectionChanged += HeightDataGrid_SelectionChanged;
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void HeightDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            WidthDataGrid.SelectionChanged -= WidthDataGrid_SelectionChanged;
            GetDecorWidth();
            WidthDataGrid.SelectionChanged += WidthDataGrid_SelectionChanged;
        }

        private void WidthDataGrid_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void kryptonContextMenuItem13_Click(object sender, EventArgs e)
        {
            if (InsetColorsDataGrid.SelectedRows.Count == 0)
                return;

            openFileDialog3.ShowDialog();
        }

        private void kryptonContextMenuItem15_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                      "Вы уверены, что хотите удалить?",
                      "Удаление");

            if (!OKCancel)
                return;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
            FrontsCatalog.DetachInsetColorImage(ConfigID);
            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                layer = null;
                backgr = null;
                backgr2 = null;
                newBitmap = null;
                newBitmap2 = null;
                ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);

                ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr2 = new Bitmap(img);

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                      "Вы уверены, что хотите удалить?",
                      "Удаление");

            if (!OKCancel)
                return;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
            FrontsCatalog.DetachInsetTypeImage(ConfigID);
            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                layer = null;
                backgr = null;
                backgr2 = null;
                newBitmap = null;
                newBitmap2 = null;
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);

                ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr2 = new Bitmap(img);

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;


                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void kryptonContextMenuItem12_Click(object sender, EventArgs e)
        {
            if (InsetTypesDataGrid.SelectedRows.Count == 0)
                return;

            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog2.FileName);

            //if (fileInfo.Length > 1500000)
            //{
            //    MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
            //    return;
            //}
            AttachmentsDT.Clear();

            DataRow NewRow = AttachmentsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog2.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog2.FileName;
            AttachmentsDT.Rows.Add(NewRow);

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            int ConfigID = FrontsCatalog.SaveInsetTypesConfig(FrontName, ColorID, TechnoColorID);
            if (ConfigID != -1)
                FrontsCatalog.AttachInsetTypeImage(AttachmentsDT, ConfigID);

            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                layer = null;
                backgr = null;
                newBitmap = null;
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);
                if (layer != null)
                {
                    if (backgr != null)
                    {
                        int width = layer.Width;
                        int height = layer.Height;
                        int width1 = backgr.Width;
                        int height1 = backgr.Height;
                        newBitmap = new Bitmap(width, height);
                        using (var canvas = Graphics.FromImage(newBitmap))
                        {
                            if (height1 >= height)
                            {
                                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                canvas.Save();
                            }
                            else
                            {
                                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                canvas.Save();
                            }
                        }
                        try
                        {
                            pcbxVisualConfig.Image = newBitmap;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;
            }
        }

        private void openFileDialog3_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog3.FileName);

            AttachmentsDT.Clear();

            DataRow NewRow = AttachmentsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog3.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog3.FileName;
            AttachmentsDT.Rows.Add(NewRow);
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            int ConfigID = FrontsCatalog.SaveInsetColorsConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
            if (ConfigID != -1)
                FrontsCatalog.AttachInsetColorImage(AttachmentsDT, ConfigID);

            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                layer = null;
                backgr = null;
                backgr2 = null;
                newBitmap = null;
                newBitmap2 = null;
                ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);

                ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr2 = new Bitmap(img);

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;
            }
        }

        private void pcbxVisualConfig_Click(object sender, EventArgs e)
        {
            if (pcbxVisualConfig.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxVisualConfig.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void cbVisualConfig_CheckedChanged(object sender, EventArgs e)
        {
            if (FrontsCatalog == null)
                return;
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            pcbxVisualConfig.Image = null;

            if (cbVisualConfig.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                layer = null;
                backgr = null;
                backgr2 = null;
                newBitmap = null;
                newBitmap2 = null;
                int ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);

                ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr2 = new Bitmap(img);

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;
            }
        }

        private void kryptonContextMenuItem16_Click(object sender, EventArgs e)
        {
            if (TechnoFrameColorsDataGrid.SelectedRows.Count == 0)
                return;

            openFileDialog5.ShowDialog();
        }

        private void TechnoFrameColorsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (bCanEdit && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void openFileDialog5_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog5.FileName);

            //if (fileInfo.Length > 1500000)
            //{
            //    MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
            //    return;
            //}
            AttachmentsDT.Clear();

            DataRow NewRow = AttachmentsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog5.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog5.FileName;
            AttachmentsDT.Rows.Add(NewRow);

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            int ConfigID = FrontsCatalog.SaveInsetTypesConfig(FrontName, ColorID, TechnoColorID);
            if (ConfigID != -1)
                FrontsCatalog.AttachInsetTypeImage(AttachmentsDT, ConfigID);

            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                layer = null;
                backgr = null;
                newBitmap = null;
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);
                if (layer != null)
                {
                    if (backgr != null)
                    {
                        int width = layer.Width;
                        int height = layer.Height;
                        int width1 = backgr.Width;
                        int height1 = backgr.Height;
                        newBitmap = new Bitmap(width, height);
                        using (var canvas = Graphics.FromImage(newBitmap))
                        {
                            if (height1 >= height)
                            {
                                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                canvas.Save();
                            }
                            else
                            {
                                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                canvas.Save();
                            }
                        }
                        try
                        {
                            pcbxVisualConfig.Image = newBitmap;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;
            }
        }

        private void kryptonContextMenuItem17_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                         "Вы уверены, что хотите удалить?",
                         "Удаление");

            if (!OKCancel)
                return;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
            FrontsCatalog.DetachInsetTypeImage(ConfigID);
            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                layer = null;
                backgr = null;
                backgr2 = null;
                newBitmap = null;
                newBitmap2 = null;
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);

                ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr2 = new Bitmap(img);

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void kryptonContextMenuItem18_Click(object sender, EventArgs e)
        {
            if (TechnoInsetColorsDataGrid.SelectedRows.Count == 0)
                return;

            openFileDialog6.ShowDialog();
        }

        private void kryptonContextMenuItem19_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                         "Вы уверены, что хотите удалить?",
                         "Удаление");

            if (!OKCancel)
                return;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
            FrontsCatalog.DetachTechnoInsetColorImage(ConfigID);
            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                layer = null;
                backgr = null;
                backgr2 = null;
                newBitmap = null;
                newBitmap2 = null;
                ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);

                ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr2 = new Bitmap(img);

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void openFileDialog6_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog6.FileName);

            AttachmentsDT.Clear();

            DataRow NewRow = AttachmentsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog6.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog6.FileName;
            AttachmentsDT.Rows.Add(NewRow);
            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            int ConfigID = FrontsCatalog.SaveTechnoInsetColorsConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
            if (ConfigID != -1)
                FrontsCatalog.AttachTechnoInsetColorImage(AttachmentsDT, ConfigID);

            pcbxVisualConfig.Image = null;
            if (cbVisualConfig.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                layer = null;
                backgr = null;
                backgr2 = null;
                newBitmap = null;
                newBitmap2 = null;
                ConfigID = FrontsCatalog.GetInsetTypesConfig(FrontName, ColorID, TechnoColorID);
                Image img = FrontsCatalog.GetInsetTypeImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    layer = new Bitmap(img);
                ConfigID = FrontsCatalog.GetInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr = new Bitmap(img);

                ConfigID = FrontsCatalog.GetTechnoInsetColorConfig(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID);
                img = FrontsCatalog.GetTechnoInsetColorImage(ConfigID);
                if (ConfigID != -1 && img != null)
                    backgr2 = new Bitmap(img);

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                pcbxVisualConfig.Image = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        pcbxVisualConfig.Image = layer;
                }
                else
                {
                    if (backgr != null)
                        pcbxVisualConfig.Image = backgr;
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                if (pcbxVisualConfig.Image == null)
                    pcbxVisualConfig.Cursor = Cursors.Default;
                else
                    pcbxVisualConfig.Cursor = Cursors.Hand;
            }
        }

        private void kryptonContextMenuItem20_Click(object sender, EventArgs e)
        {
            bool PressOK = false;

            string LabelsCount = string.Empty;
            string PositionsCount = string.Empty;
            string LabelsLength = string.Empty;
            string Serviceman = string.Empty;
            string Milling = string.Empty;
            string DocDateTime = string.Empty;
            string Batch = string.Empty;
            string Pallet = string.Empty;
            string Profile = string.Empty;

            string LabelsCount1 = string.Empty;
            string PositionsCount1 = string.Empty;
            string LabelsLength1 = string.Empty;
            string Serviceman1 = string.Empty;
            string Milling1 = string.Empty;
            string DocDateTime1 = string.Empty;
            string Batch1 = string.Empty;
            string Pallet1 = string.Empty;
            string Profile1 = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            SamplesLabelsForm SampleLabelInfo = new SamplesLabelsForm(this);
            TopForm = SampleLabelInfo;
            SampleLabelInfo.ShowDialog();
            PressOK = SampleLabelInfo.PressOK;

            DocDateTime = SampleLabelInfo.DocDateTime;
            Batch = SampleLabelInfo.Batch;
            Pallet = SampleLabelInfo.Pallet;
            Profile = SampleLabelInfo.Profile;
            Serviceman = SampleLabelInfo.Serviceman;
            Milling = SampleLabelInfo.Milling;
            LabelsCount = SampleLabelInfo.LabelsCount;

            DocDateTime1 = SampleLabelInfo.DocDateTime1;
            Batch1 = SampleLabelInfo.Batch1;
            Pallet1 = SampleLabelInfo.Pallet1;
            Profile1 = SampleLabelInfo.Profile1;
            Serviceman1 = SampleLabelInfo.Serviceman1;
            Milling1 = SampleLabelInfo.Milling1;
            LabelsCount1 = SampleLabelInfo.LabelsCount1;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SampleLabelInfo.Dispose();
            TopForm = null;

            if (!PressOK)
                return;
            SamplesLabels SamplesLabelsManager = new SamplesLabels();
            SamplesLabelsManager.ClearLabelInfo();

            LabelContent LabelInfo = new LabelContent()
            {
                DocDateTime = DocDateTime,
                Batch = Batch,
                Pallet = Pallet,
                Profile = Profile,
                Serviceman = Serviceman,
                Milling = Milling,

                DocDateTime1 = DocDateTime1,
                Batch1 = Batch1,
                Pallet1 = Pallet1,
                Profile1 = Profile1,
                Serviceman1 = Serviceman1,
                Milling1 = Milling1
            };
            SamplesLabelsManager.AddLabelInfo(ref LabelInfo);

            PrintDialog.Document = SamplesLabelsManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                SamplesLabelsManager.Print();
            }
        }

        private void btnSetDescription_Click(object sender, EventArgs e)
        {
            bool PressOK = false;

            string FrontName = "";
            int ColorID = -1;
            int TechnoColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            if (FrontsDataGrid.SelectedRows.Count > 0 && FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value != DBNull.Value)
                FrontName = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].Value.ToString();
            if (FrameColorsDataGrid.SelectedRows.Count > 0 && FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(FrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (TechnoFrameColorsDataGrid.SelectedRows.Count > 0 && TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                TechnoColorID = Convert.ToInt32(TechnoFrameColorsDataGrid.SelectedRows[0].Cells["ColorID"].Value);
            if (PatinaDataGrid.SelectedRows.Count > 0 && PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(PatinaDataGrid.SelectedRows[0].Cells["PatinaID"].Value);
            if (InsetTypesDataGrid.SelectedRows.Count > 0 && InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (InsetColorsDataGrid.SelectedRows.Count > 0 && InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);
            if (TechnoInsetTypesDataGrid.SelectedRows.Count > 0 && TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (TechnoInsetColorsDataGrid.SelectedRows.Count > 0 && TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value);

            bool IsConfigImageToSite = false;
            bool bLatest = false;
            string Category = string.Empty;
            string NameProd = string.Empty;
            string Description = string.Empty;
            string Sizes = string.Empty;
            string Material = string.Empty;

            int ConfigID = FrontsCatalog.GetFrontAttachments(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
            if (ConfigID != -1)
                IsConfigImageToSite = FrontsCatalog.IsConfigImageToSite(ConfigID, ref bLatest, ref Category, ref NameProd, ref Description, ref Sizes, ref Material);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            ProductDescriptionForm ProductDescriptionForm = new ProductDescriptionForm(this, IsConfigImageToSite, bLatest, Category, NameProd, Description, Sizes, Material);
            TopForm = ProductDescriptionForm;
            ProductDescriptionForm.ShowDialog();

            PressOK = ProductDescriptionForm.PressOK;
            IsConfigImageToSite = ProductDescriptionForm.ToSite;
            Category = ProductDescriptionForm.Category;
            NameProd = ProductDescriptionForm.NameProd;
            Description = ProductDescriptionForm.Description;
            Sizes = ProductDescriptionForm.Sizes;
            Material = ProductDescriptionForm.Material;
            bLatest = ProductDescriptionForm.Latest;

            PhantomForm.Close();
            PhantomForm.Dispose();
            ProductDescriptionForm.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (ConfigID != -1)
            {

                FrontsCatalog.ConfigImageToSite(ConfigID, IsConfigImageToSite, bLatest, Category, NameProd, Description, Sizes, Material);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                int FrontID = FrontsCatalog.GetFrontID(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);

                string Front = FrontsDataGrid.SelectedRows[0].Cells["FrontName"].FormattedValue.ToString();
                string FrameColor = FrameColorsDataGrid.SelectedRows[0].Cells["ColorName"].FormattedValue.ToString();
                string Patina = PatinaDataGrid.SelectedRows[0].Cells["PatinaName"].FormattedValue.ToString();
                string InsetType = InsetTypesDataGrid.SelectedRows[0].Cells["InsetTypeName"].FormattedValue.ToString();
                string InsetColor = InsetColorsDataGrid.SelectedRows[0].Cells["InsetColorName"].FormattedValue.ToString();

                if (IsConfigImageToSite && !FrontsCatalog.CreateFotoFromVisualConfig(FrontID, ColorID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID,
                    Category, Front, FrameColor, Patina, InsetType, InsetColor))
                {
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                    MessageBox.Show("Изображение не найдено!");
                }
                else
                {

                    ConfigID = FrontsCatalog.GetFrontAttachments(FrontName, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                    FrontsCatalog.ConfigImageToSite(ConfigID, IsConfigImageToSite, bLatest, Category, NameProd, Description, Sizes, Material);

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
            }
        }

        private void KryptonButton1_Click(object sender, EventArgs e)
        {
            bool PressOK = false;

            int ProductID = -1;
            string DecorName = string.Empty;
            int ColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;

            if ((DataRowView)DecorCatalog.DecorProductsBindingSource.Current != null)
                ProductID = Convert.ToInt32(((DataRowView)DecorCatalog.DecorProductsBindingSource.Current)["ProductID"]);
            if ((DataRowView)DecorCatalog.DecorItemBindingSource.Current != null)
                DecorName = ((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString();
            if ((DataRowView)DecorCatalog.ItemColorsBindingSource.Current != null)
                ColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]);
            if ((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current != null)
                PatinaID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]);
            if ((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current != null)
                InsetTypeID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]);
            if ((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current != null)
                InsetColorID = Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]);

            string Category = string.Empty;
            string Description = string.Empty;
            string NameProd = string.Empty;
            string Sizes = string.Empty;
            string Material = string.Empty;
            bool IsConfigImageToSite = false;
            bool bLatest = false;

            int ConfigID = DecorCatalog.GetDecorAttachments(ProductID, DecorName, ColorID, PatinaID, InsetTypeID, InsetColorID);
            if (ConfigID != -1)
                IsConfigImageToSite = DecorCatalog.IsConfigImageToSite(ConfigID, ref bLatest, ref Category, ref NameProd, ref Description, ref Sizes, ref Material);
            else
            {
                MessageBox.Show("Изображение не найдено!");
                return;
            }
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            ProductDescriptionForm ProductDescriptionForm = new ProductDescriptionForm(this, IsConfigImageToSite, bLatest, Category, NameProd, Description, Sizes, Material);
            TopForm = ProductDescriptionForm;
            ProductDescriptionForm.ShowDialog();

            PressOK = ProductDescriptionForm.PressOK;
            IsConfigImageToSite = ProductDescriptionForm.ToSite;
            Category = ProductDescriptionForm.Category;
            Description = ProductDescriptionForm.Description;
            NameProd = ProductDescriptionForm.NameProd;
            Sizes = ProductDescriptionForm.Sizes;
            Material = ProductDescriptionForm.Material;
            bLatest = ProductDescriptionForm.Latest;

            PhantomForm.Close();
            PhantomForm.Dispose();
            ProductDescriptionForm.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            if (ConfigID != -1)
            {
                DecorCatalog.ConfigImageToSite(ConfigID, IsConfigImageToSite, bLatest, Category, NameProd, Description, Sizes, Material);
            }

        }

        private void KryptonContextMenuItem21_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            string DocDateTime = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int DecorID = DecorCatalog.GetDecorIdByName(DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString());
            int LabelsLength = Convert.ToInt32(LengthDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(HeightDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(WidthDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            DecorCatalog.TechStoreGroupInfo techStoreGroupInfo = DecorCatalog.GetSubGroupInfo(DecorID);

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = Color;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CabFurLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelManager.GetBarcodeNumber(19, SampleLabelID);
                    int FactoryId = 1;
                    if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
                        FactoryId = 2;
                    LabelInfo.FactoryType = FactoryId;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelManager.Print();
            }
        }

        private void KryptonContextMenuItem22_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            string DocDateTime = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int DecorID = DecorCatalog.GetDecorIdByName(DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString());
            int LabelsLength = Convert.ToInt32(LengthDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(HeightDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(WidthDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();
            string Color2 = DecorInsetColorsDataGrid.SelectedRows[0].Cells["InsetColorName"].Value.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            DecorCatalog.TechStoreGroupInfo techStoreGroupInfo = DecorCatalog.GetSubGroupInfo(DecorID);

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = Color;
            NewRow["Color2"] = Color2;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CubeLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CubeLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CubeLabelManager.GetBarcodeNumber(19, SampleLabelID);
                    int FactoryId = 1;
                    if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
                        FactoryId = 2;
                    LabelInfo.FactoryType = FactoryId;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CubeLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CubeLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CubeLabelManager.Print();
            }
        }

        private void KryptonContextMenuItem23_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            string DocDateTime = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int DecorID = DecorCatalog.GetDecorIdByName(DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString());
            int LabelsLength = Convert.ToInt32(LengthDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(HeightDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(WidthDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            DecorCatalog.TechStoreGroupInfo techStoreGroupInfo = DecorCatalog.GetSubGroupInfo(DecorID);

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = Color;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CabFurLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelManager.GetBarcodeNumber(19, SampleLabelID);
                    int FactoryId = 1;
                    if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
                        FactoryId = 2;
                    LabelInfo.FactoryType = FactoryId;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelManager.Print();
            }
        }

        private void kryptonContextMenuItem24_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            string DocDateTime = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int DecorID = DecorCatalog.GetDecorIdByName(DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString());
            int LabelsLength = Convert.ToInt32(LengthDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(HeightDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(WidthDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            DecorCatalog.TechStoreGroupInfo techStoreGroupInfo = DecorCatalog.GetSubGroupInfo(DecorID);

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = Color;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CabFurLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelManager.GetBarcodeNumber(19, SampleLabelID);
                    int FactoryId = 1;
                    if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
                        FactoryId = 2;
                    LabelInfo.FactoryType = FactoryId;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelManager.Print();
            }
        }

        private void kryptonContextMenuItem25_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            string DocDateTime = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int DecorID = DecorCatalog.GetDecorIdByName(DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString());
            int LabelsLength = Convert.ToInt32(LengthDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(HeightDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(WidthDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            DecorCatalog.TechStoreGroupInfo techStoreGroupInfo = DecorCatalog.GetSubGroupInfo(DecorID);

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = Color;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CabFurLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelManager.GetBarcodeNumber(19, SampleLabelID);
                    int FactoryId = 1;
                    if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
                        FactoryId = 2;
                    LabelInfo.FactoryType = FactoryId;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelManager.Print();
            }
        }

        private void kryptonContextMenuItem26_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            string DocDateTime = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int DecorID = DecorCatalog.GetDecorIdByName(DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString());
            int LabelsLength = Convert.ToInt32(LengthDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(HeightDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(WidthDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            DecorCatalog.TechStoreGroupInfo techStoreGroupInfo = DecorCatalog.GetSubGroupInfo(DecorID);

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = Color;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }


                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CabFurLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelManager.GetBarcodeNumber(19, SampleLabelID);

                    int FactoryId = 1;
                    if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
                        FactoryId = 2;
                    LabelInfo.FactoryType = FactoryId;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelManager.Print();
            }
        }

        private void kryptonContextMenuItem27_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            string DocDateTime = string.Empty;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            int DecorID = DecorCatalog.GetDecorIdByName(DecorDataGrid.SelectedRows[0].Cells["Name"].Value.ToString());
            int LabelsLength = Convert.ToInt32(LengthDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(HeightDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(WidthDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = ColorsDataGrid.SelectedRows[0].Cells["ColorName"].Value.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            DecorCatalog.TechStoreGroupInfo techStoreGroupInfo = DecorCatalog.GetSubGroupInfo(DecorID);

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = Color;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }


                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CabFurLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelManager.GetBarcodeNumber(19, SampleLabelID);

                    int FactoryId = 1;
                    if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
                        FactoryId = 2;
                    LabelInfo.FactoryType = FactoryId;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelManager.Print();
            }
        }
    }
}
