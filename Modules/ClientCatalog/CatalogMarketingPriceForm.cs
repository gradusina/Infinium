using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CatalogMarketingPriceForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        LightStartForm LightStartForm;
        Form TopForm = null;
        Infinium.FrontsCatalog FrontsCatalog = null;
        Infinium.DecorCatalog DecorCatalog = null;

        public CatalogMarketingPriceForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void CatalogMarketingPriceForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
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

            FrontsMarketPriceDataGrid.DataSource = FrontsCatalog.MarketingPriceBindingSource;

            GetFrameColors();
            GetTechnoFrameColors();
            GetPatina();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void FrontsGridsSettings()
        {
            FrontsDataGrid.Columns["FrontID"].Visible = false;
            FrameColorsDataGrid.Columns["ColorID"].Visible = false;
            TechnoFrameColorsDataGrid.Columns["ColorID"].Visible = false;
            PatinaDataGrid.Columns["PatinaID"].Visible = false;
            InsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            InsetTypesDataGrid.Columns["GroupID"].Visible = false;
            InsetColorsDataGrid.Columns["InsetColorID"].Visible = false;
            InsetColorsDataGrid.Columns["GroupID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["GroupID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["InsetColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["GroupID"].Visible = false;
            FrontsMarketPriceDataGrid.Columns["MeasureID"].Visible = false;

            FrontsMarketPriceDataGrid.Columns["MarketingPrice"].HeaderText = "Цена";
            FrontsMarketPriceDataGrid.Columns["ZOVNonStandMargin"].HeaderText = "Наценка, %";
            //FrontsMarketPriceDataGrid.Columns["Measure"].HeaderText = "";

            FrontsDataGrid.Columns["FrontName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrameColorsDataGrid.Columns["ColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PatinaDataGrid.Columns["PatinaName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetTypesDataGrid.Columns["InsetTypeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetColorsDataGrid.Columns["InsetColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoFrameColorsDataGrid.Columns["ColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetTypesDataGrid.Columns["InsetTypeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetColorsDataGrid.Columns["InsetColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            FrontsMarketPriceDataGrid.Columns["MarketingPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsMarketPriceDataGrid.Columns["MarketingPrice"].MinimumWidth = 15;
            FrontsMarketPriceDataGrid.Columns["ZOVNonStandMargin"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsMarketPriceDataGrid.Columns["ZOVNonStandMargin"].MinimumWidth = 60;
            //FrontsMarketPriceDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrontsMarketPriceDataGrid.Columns["Measure"].Width = 45;
            FrontsMarketPriceDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            //FrontsMarketingPriceDataGrid.Columns["MeasureID"].Visible = false;
            //ExtraPriceDataGrid.Columns["MeasureID"].Visible = false;

            //FrontsMarketingPriceDataGrid.Columns["Measure"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //ExtraPriceDataGrid.Columns["Measure"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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

        public void GetFrontsMarketPrice()
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
            int Width = 0;
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
            if (FrontsWidthDataGrid.SelectedRows.Count > 0 && FrontsWidthDataGrid.SelectedRows[0].Cells["Width"].Value != DBNull.Value)
                Width = Convert.ToInt32(FrontsWidthDataGrid.SelectedRows[0].Cells["Width"].Value);

            FrontsCatalog.FilterCatalogMarketingPrice(FrontName, ColorID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width);
        }

        #endregion

        private void FrontsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetFrameColors();
            GetTechnoFrameColors();
            GetPatina();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void FrameColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetTechnoFrameColors();
            GetPatina();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void TechnoFrameColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetPatina();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
        }

        private void PatinaDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void InsetTypesDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void InsetColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void TechnoInsetTypesDataGridDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void TechnoInsetColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void FrontsHeightDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetFrontWidth();
            GetFrontsMarketPrice();
        }

        private void FrontsWidthDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetFrontsMarketPrice();
        }

        #region DECOR FUNCTIONS
        public void DecorCatalogBinding()
        {
            ProductsDataGrid.DataSource = DecorCatalog.DecorProductsBindingSource;

            DecorDataGrid.DataSource = DecorCatalog.DecorItemBindingSource;

            ColorsDataGrid.DataSource = DecorCatalog.ItemColorsBindingSource;

            DecorPatinaDataGrid.DataSource = DecorCatalog.ItemPatinaBindingSource;

            LengthDataGrid.DataSource = DecorCatalog.LengthBindingSource;

            HeightDataGrid.DataSource = DecorCatalog.HeightBindingSource;

            WidthDataGrid.DataSource = DecorCatalog.WidthBindingSource;

            DecorZOVPriceDataGrid.DataSource = DecorCatalog.MarketingPriceBindingSource;

            GetDecorItems();
            GetDecorColors();
            GetDecorPatina();
            GetDecorHeight();
            GetDecorWidth();
            GetDecorLength();
            GetDecorMarketingPrice();
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

            DecorZOVPriceDataGrid.Columns["MeasureID"].Visible = false;
            DecorZOVPriceDataGrid.Columns["Measure"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            ProductsDataGrid.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColorsDataGrid.Columns["ColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PatinaDataGrid.Columns["PatinaName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            if (DecorCatalog.DecorItemBindingSource.Count > 0)
            {
                DecorCatalog.FilterColors(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString());
            }

            //if (DecorCatalog.ItemColorsBindingSource.Count == 1)
            //    if (((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"].ToString() == "0")
            //    {
            //        DecorCatalogTableLayout.ColumnStyles[4].Width = 0;
            //        DecorColorsPanel.Visible = false;
            //        //ColorsDataGrid.Visible = false;
            //        ColorLabel.Visible = false;
            //        return;
            //    }
            //ColorsDataGrid.Columns["ColorID"].Visible = false;
            //DecorCatalogTableLayout.ColumnStyles[4].Width = 350;
            //DecorColorsPanel.Visible = true;
            ////ColorsDataGrid.Visible = true;
            //ColorLabel.Visible = true;
        }

        public void GetDecorPatina()
        {
            if (DecorCatalog.ItemColorsBindingSource.Count > 0)
            {
                DecorCatalog.FilterPatina(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]));
            }

            //if (DecorCatalog.ItemPatinaBindingSource.Count == 1)
            //    if (((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"].ToString() == "0")
            //    {
            //        DecorCatalogTableLayout.ColumnStyles[4].Width = 0;
            //        DecorColorsPanel.Visible = false;
            //        //ColorsDataGrid.Visible = false;
            //        ColorLabel.Visible = false;
            //        return;
            //    }
            //PatinaDataGrid.Columns["PatinaID"].Visible = false;
            //DecorCatalogTableLayout.ColumnStyles[4].Width = 350;
            //DecorColorsPanel.Visible = true;
            ////ColorsDataGrid.Visible = true;
            //ColorLabel.Visible = true;
        }

        public void GetDecorLength()
        {
            if (DecorCatalog.WidthBindingSource.Count > 0)
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
            if (DecorCatalog.ItemPatinaBindingSource.Count > 0)
            {
                int Length = Convert.ToInt32(((DataRowView)DecorCatalog.LengthBindingSource.Current)["Length"]);

                DecorCatalog.FilterHeight(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]), Length);
            }
        }

        public void GetDecorWidth()
        {
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

        public void GetDecorMarketingPrice()
        {
            if (DecorCatalog.WidthBindingSource.Count > 0)
            {
                int Length = Convert.ToInt32(((DataRowView)DecorCatalog.LengthBindingSource.Current)["Length"]);
                int Height = Convert.ToInt32(((DataRowView)DecorCatalog.HeightBindingSource.Current)["Height"]);
                int Width = Convert.ToInt32(((DataRowView)DecorCatalog.WidthBindingSource.Current)["Width"]);

                DecorCatalog.FilterCatalogMarketingPrice(((DataRowView)DecorCatalog.DecorItemBindingSource.Current)["Name"].ToString(),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemColorsBindingSource.Current)["ColorID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemPatinaBindingSource.Current)["PatinaID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetTypesBindingSource.Current)["InsetTypeID"]),
                    Convert.ToInt32(((DataRowView)DecorCatalog.ItemInsetColorsBindingSource.Current)["InsetColorID"]), Length, Height, Width);
            }
        }
        #endregion

        private void ProductsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetDecorItems();
            GetDecorColors();
            GetDecorPatina();
            GetDecorHeight();
            GetDecorWidth();
            GetDecorLength();
            GetDecorMarketingPrice();
        }

        private void DecorDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetDecorColors();
            GetDecorPatina();
            GetDecorHeight();
            GetDecorWidth();
            GetDecorLength();
            GetDecorMarketingPrice();
        }

        private void ColorsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetDecorPatina();
            GetDecorHeight();
            GetDecorWidth();
            GetDecorLength();
            GetDecorMarketingPrice();
        }

        private void DecorPatinaDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetDecorLength();
            GetDecorHeight();
            GetDecorWidth();
            GetDecorLength();
            GetDecorMarketingPrice();
        }

        private void LengthDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetDecorWidth();
            GetDecorLength();
            GetDecorMarketingPrice();
        }

        private void HeightDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetDecorLength();
            GetDecorMarketingPrice();
        }

        private void FrontsCheckButton_Click(object sender, EventArgs e)
        {
            GetFrameColors();
            GetTechnoFrameColors();
            GetPatina();
            GetInsetTypes();
            GetInsetColors();
            GetTechnoInsetTypes();
            GetTechnoInsetColors();
            GetFrontHeight();
            GetFrontWidth();
            GetFrontsMarketPrice();
            FrontsCatalogPanel.BringToFront();
        }

        private void DecorCheckButton_Click(object sender, EventArgs e)
        {
            GetDecorItems();
            GetDecorColors();
            GetDecorPatina();
            GetDecorHeight();
            GetDecorWidth();
            GetDecorLength();
            GetDecorMarketingPrice();
            DecorCatalogPanel.BringToFront();
        }

        private void TPSCheckButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            FrontsCatalog = new FrontsCatalog(2);
            DecorCatalog = new DecorCatalog(2);

            FrontsCatalogBinding();
            DecorCatalogBinding();
            FrontsGridsSettings();
            DecorGridsSettings();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ProfilCheckButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            FrontsCatalog = new FrontsCatalog(1);
            DecorCatalog = new DecorCatalog(1);

            FrontsCatalogBinding();
            DecorCatalogBinding();
            FrontsGridsSettings();
            DecorGridsSettings();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void WidthDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetDecorMarketingPrice();
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

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (FrontsCatalog == null || DecorCatalog == null)
                return;

            if (kryptonCheckSet1.CheckedButton.Name == "DecorCheckButton")
            {
                GetDecorItems();
                GetDecorColors();
                GetDecorPatina();
                GetDecorHeight();
                GetDecorWidth();
                GetDecorLength();
                GetDecorMarketingPrice();
                DecorCatalogPanel.BringToFront();
            }
            else
            {
                GetFrameColors();
                GetTechnoFrameColors();
                GetPatina();
                GetInsetTypes();
                GetInsetColors();
                GetTechnoInsetTypes();
                GetTechnoInsetColors();
                GetFrontHeight();
                GetFrontWidth();
                GetFrontsMarketPrice();
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

                FrontsCatalog = new FrontsCatalog(1);
                DecorCatalog = new DecorCatalog(1);

                FrontsCatalogBinding();
                DecorCatalogBinding();
                FrontsGridsSettings();
                DecorGridsSettings();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                FrontsCatalog = new FrontsCatalog(2);
                DecorCatalog = new DecorCatalog(2);

                FrontsCatalogBinding();
                DecorCatalogBinding();
                FrontsGridsSettings();
                DecorGridsSettings();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

    }
}
