using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.StatisticsMarketing;

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CommonStatisticsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;
        bool bStoreSummaryClient = false;
        bool bExpSummaryClient = false;
        bool NeedSelectionChange = true;

        int FormEvent = 0;
        int CurrentWeekNumber = 1;


        LightStartForm LightStartForm;

        Form TopForm = null;

        MarketingBatchStatistics BatchStatistics;
        Infinium.Modules.ZOV.DecorCatalogOrder DecorCatalogOrder;
        CommonStatistics AllProductsStatistics;
        ZOVOrdersStatistics ZOVOrdersStatistics;
        StorageStatistics StorageStatistics;
        ExpeditionStatistics ExpeditionStatistics;
        DispatchStatistics DispatchStatistics;

        ConditionOrdersStatistics ConditionOrdersStatistics;
        BatchExcelReport MarketingBatchReport;

        NumberFormatInfo nfi1;
        NumberFormatInfo nfi2;

        public CommonStatisticsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;

            if (!Security.PriceAccess)
            {
                FrontsCostPanel.Visible = false;
                DecorCostPanel.Visible = false;
                StoreFrontsCostPanel.Visible = false;
                StoreDecorCostPanel.Visible = false;
            }
        }

        private void CommonStatisticsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            NeedSplash = true;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void CommonStatisticsForm_Load(object sender, EventArgs e)
        {
            DateTime FirstDay = DateTime.Now.AddDays(-1);
            DateTime Today = DateTime.Now;

            CalendarFrom.SelectionStart = FirstDay;
            CalendarFrom.SelectionEnd = FirstDay;
            CalendarTo.SelectionStart = Today;
            CalendarTo.SelectionEnd = Today;

            CalendarFrom.TodayDate = Today;
            CalendarTo.TodayDate = Today;

            ClientsDataGrid.Enabled = false;
            dgvMarketManagers.Enabled = false;
            AllProductsStatistics.CheckAllManagers(false);
            AllProductsStatistics.CheckAllClients(false);

            //AllProductsStatistics.ShowCheckColumn(ClientGroupsDataGrid, true);
            //AllProductsStatistics.ShowCheckColumn(ClientsDataGrid, false);

            //kryptonButton1_Click(null, null);

            if (!Security.PriceAccess)
            {
                FrontsCostPanel.Visible = false;
                DecorCostPanel.Visible = false;
                StoreFrontsCostPanel.Visible = false;
                StoreDecorCostPanel.Visible = false;
                DispFrontsCostPanel.Visible = false;
                DispDecorCostPanel.Visible = false;
                panel74.Visible = false;
                panel76.Visible = false;
                panel88.Visible = false;
                panel81.Visible = false;
                panel93.Visible = false;
                panel96.Visible = false;
            }

            BatchFilter();
            pnlGeneralSummary.BringToFront();
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                Opacity = 1;

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
                if (Convert.ToDecimal(Opacity) != Convert.ToDecimal(0.00))
                    Opacity = Convert.ToDouble(Convert.ToDecimal(Opacity) - Convert.ToDecimal(0.05));
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
                if (Opacity != 1)
                    Opacity += 0.05;
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

        private void Initialize()
        {
            MarketingBatchReport = new BatchExcelReport();

            AllProductsStatistics = new Modules.StatisticsMarketing.CommonStatistics();

            ZOVOrdersStatistics = new Modules.StatisticsMarketing.ZOVOrdersStatistics();

            CommonStatisticsRebind();
            CommonStatisticsGridSettings();

            DateTime FirstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            mcBatchFirstDate.SelectionStart = FirstDate;

            DateTime FirstDay = DateTime.Now.AddDays(-1);
            DateTime DispFirstDay = DateTime.Now.AddDays(-3);
            DateTime Today = DateTime.Now;

            DispatchDateFrom.SelectionStart = DispFirstDay;

            BatchStatistics = new Modules.StatisticsMarketing.MarketingBatchStatistics();
            dgvGeneralSummary.DataSource = BatchStatistics.MegaBatchBS;
            dgvSimpleFronts.DataSource = BatchStatistics.SimpleFrontsSummaryBS;
            dgvCurvedFronts.DataSource = BatchStatistics.CurvedFrontsSummaryBS;
            dgvDecor.DataSource = BatchStatistics.DecorProductsSummaryBS;
            MegaBatchGridSettings();
            ProductGridSettings();

            StorageStatistics = new StorageStatistics(
                ref StoreMFSummaryDG, ref StoreMCurvedFSummaryDG, ref StoreMDSummaryDG, ref StorePrepareFSummaryDG, ref StorePrepareCurvedFSummaryDG, ref StorePrepareDSummaryDG,
                ref StoreFrontsDataGrid, ref StoreCurvedFrontsDataGrid,
                ref StoreDecorProductsDataGrid, ref StoreDecorItemsDataGrid);

            dgvClientsGroups.DataSource = StorageStatistics.ClientGroupsBS;
            dgvClientsGroups.Columns["ClientGroupID"].Visible = false;
            dgvClientsGroups.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientsGroups.Columns["Check"].Width = 50;
            dgvClientsGroups.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvClientsGroups.Columns["ClientGroupName"].MinimumWidth = 110;
            dgvClientsGroups.Columns["Check"].DisplayIndex = 0;
            dgvClientsGroups.Columns["ClientGroupName"].DisplayIndex = 1;
            dgvClientsGroups.Columns["ClientGroupName"].ReadOnly = true;

            ExpeditionStatistics = new ExpeditionStatistics(
                ref ExpMFSummaryDG, ref ExpMDSummaryDG, ref ExpPrepareFSummaryDG, ref ExpPrepareDSummaryDG,
                ref ExpZFSummaryDG, ref ExpZDSummaryDG, ref ExpFrontsDataGrid,
                ref ExpDecorProductsDataGrid, ref ExpDecorItemsDataGrid);

            DispatchStatistics = new DispatchStatistics(
                ref DispMFSummaryDG, ref DispMDSummaryDG,
                ref DispZFSummaryDG, ref DispZDSummaryDG, ref DispFrontsDataGrid,
                ref DispDecorProductsDataGrid, ref DispDecorItemsDataGrid);

            ConditionOrdersStatistics = new ConditionOrdersStatistics();
            ConditionGroupsDG.DataSource = ConditionOrdersStatistics.ClientGroupsBS;
            ConditionOrdersGrids1();
            ConditionOrdersGrids2();
            ConditionOrdersGrids3();
            CurrentWeekNumber = GetWeekNumber(DateTime.Now);

            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            ArrayList Years = new ArrayList();
            for (int i = 2013; i <= LastDay.Year; i++)
            {
                Years.Add(i);
            }
            cbxYears.DataSource = Years.ToArray();
            cbxYears.SelectedIndex = cbxYears.Items.Count - 1;

            DateTime Monday = GetMonday(CurrentWeekNumber);
            DateTime Wednesday = GetWednesday(CurrentWeekNumber);
            DateTime Friday = GetFriday(CurrentWeekNumber);
            int WeeksCount = GetWeekNumber(LastDay);

            for (int i = 1; i <= WeeksCount; i++)
            {
                WeeksOfYearListBox.Items.Add(i);
            }
            WeeksOfYearListBox.SelectedIndex = CurrentWeekNumber - 1;

            FMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            FWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            FFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");
            DMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            DWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            DFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");

            nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ",",
                NumberDecimalDigits = 3
            };
            nfi2 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ",",
                NumberDecimalDigits = 3
            };
        }

        private void CommonStatisticsGridSettings()
        {
            dgvMarketManagers.DataSource = AllProductsStatistics.ManagersBindingSource;
            ClientGroupsDataGrid.DataSource = AllProductsStatistics.ClientGroupsBindingSource;
            ClientsDataGrid.DataSource = AllProductsStatistics.ClientsBindingSource;

            foreach (DataGridViewColumn Column in dgvMarketManagers.Columns)
                Column.ReadOnly = true;

            dgvMarketManagers.Columns["ManagerID"].Visible = false;
            dgvMarketManagers.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMarketManagers.Columns["Check"].Width = 40;
            dgvMarketManagers.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMarketManagers.AutoGenerateColumns = false;
            dgvMarketManagers.Columns["Check"].ReadOnly = false;
            dgvMarketManagers.Columns["Check"].DisplayIndex = 0;
            dgvMarketManagers.Columns["Name"].DisplayIndex = 1;

            foreach (DataGridViewColumn Column in ClientGroupsDataGrid.Columns)
                Column.ReadOnly = true;

            ClientGroupsDataGrid.Columns["ClientGroupID"].Visible = false;
            ClientGroupsDataGrid.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientGroupsDataGrid.Columns["Check"].Width = 40;
            ClientGroupsDataGrid.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupsDataGrid.AutoGenerateColumns = false;
            ClientGroupsDataGrid.Columns["Check"].ReadOnly = false;
            ClientGroupsDataGrid.Columns["Check"].DisplayIndex = 0;
            ClientGroupsDataGrid.Columns["ClientGroupName"].DisplayIndex = 1;

            foreach (DataGridViewColumn Column in ClientsDataGrid.Columns)
                Column.ReadOnly = true;
            ClientsDataGrid.Columns["ManagerID"].Visible = false;
            ClientsDataGrid.Columns["ClientID"].Visible = false;
            ClientsDataGrid.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsDataGrid.Columns["Check"].Width = 40;
            ClientsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientsDataGrid.Columns["Check"].ReadOnly = false;
            ClientsDataGrid.Columns["ClientName"].HeaderText = string.Empty;
            ClientsDataGrid.Columns["Check"].HeaderText = string.Empty;
            ClientsDataGrid.AutoGenerateColumns = false;
            ClientsDataGrid.Columns["Check"].DisplayIndex = 0;
            ClientsDataGrid.Columns["ClientName"].DisplayIndex = 1;

            ZOVClientGroupsDataGrid.DataSource = ZOVOrdersStatistics.ZOVClientGroupsBindingSource;
            ZOVClientsDataGrid.DataSource = ZOVOrdersStatistics.ZOVClientsBindingSource;

            foreach (DataGridViewColumn Column in ZOVClientGroupsDataGrid.Columns)
                Column.ReadOnly = true;
            ZOVClientGroupsDataGrid.Columns["ClientGroupID"].Visible = false;
            ZOVClientGroupsDataGrid.Columns["Color"].Visible = false;
            ZOVClientGroupsDataGrid.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ZOVClientGroupsDataGrid.Columns["Check"].Width = 40;
            ZOVClientGroupsDataGrid.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ZOVClientGroupsDataGrid.Columns["ClientGroupName"].HeaderText = string.Empty;
            ZOVClientGroupsDataGrid.Columns["Check"].HeaderText = string.Empty;
            ZOVClientGroupsDataGrid.AutoGenerateColumns = false;
            ZOVClientGroupsDataGrid.Columns["Check"].ReadOnly = false;
            ZOVClientGroupsDataGrid.Columns["Check"].DisplayIndex = 0;
            ZOVClientGroupsDataGrid.Columns["ClientGroupName"].DisplayIndex = 1;

            foreach (DataGridViewColumn Column in ZOVClientsDataGrid.Columns)
                Column.ReadOnly = true;
            ZOVClientsDataGrid.Columns["ClientGroupName"].Visible = false;
            ZOVClientsDataGrid.Columns["ClientID"].Visible = false;
            ZOVClientsDataGrid.Columns["Color"].Visible = false;
            ZOVClientsDataGrid.Columns["ClientGroupID"].Visible = false;
            ZOVClientsDataGrid.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ZOVClientsDataGrid.Columns["Check"].Width = 40;
            ZOVClientsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ZOVClientsDataGrid.Columns["Check"].ReadOnly = false;
            ZOVClientsDataGrid.Columns["ClientName"].HeaderText = string.Empty;
            ZOVClientsDataGrid.Columns["Check"].HeaderText = string.Empty;
            ZOVClientsDataGrid.AutoGenerateColumns = false;
            ZOVClientsDataGrid.Columns["Check"].DisplayIndex = 0;
            ZOVClientsDataGrid.Columns["ClientName"].DisplayIndex = 1;

            if (!Security.PriceAccess)
            {
                FrontsDataGrid.Columns["Cost"].Visible = false;
                FrameColorsDataGrid.Columns["Cost"].Visible = false;
                DecorProductsDataGrid.Columns["Cost"].Visible = false;
                DecorItemsDataGrid.Columns["Cost"].Visible = false;
                DecorColorsDataGrid.Columns["Cost"].Visible = false;
                DecorSizesDataGrid.Columns["Cost"].Visible = false;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 3,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            FrontsDataGrid.ColumnHeadersHeight = 38;
            FrameColorsDataGrid.ColumnHeadersHeight = 38;
            TechnoColorsDataGrid.ColumnHeadersHeight = 38;
            InsetTypesDataGrid.ColumnHeadersHeight = 38;
            InsetColorsDataGrid.ColumnHeadersHeight = 38;
            TechnoInsetTypesDataGrid.ColumnHeadersHeight = 38;
            TechnoInsetColorsDataGrid.ColumnHeadersHeight = 38;
            SizesDataGrid.ColumnHeadersHeight = 38;
            DecorProductsDataGrid.ColumnHeadersHeight = 38;
            DecorItemsDataGrid.ColumnHeadersHeight = 38;
            DecorColorsDataGrid.ColumnHeadersHeight = 38;
            DecorSizesDataGrid.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in FrontsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in FrameColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in TechnoColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in InsetTypesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in InsetColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in TechnoInsetTypesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in TechnoInsetColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in SizesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }


            foreach (DataGridViewColumn Column in DecorProductsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in DecorItemsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in DecorColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in DecorSizesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            FrontsDataGrid.Columns["FrontID"].Visible = false;
            FrontsDataGrid.Columns["Width"].Visible = false;

            FrameColorsDataGrid.Columns["FrontID"].Visible = false;
            FrameColorsDataGrid.Columns["ColorID"].Visible = false;
            FrameColorsDataGrid.Columns["Width"].Visible = false;
            FrameColorsDataGrid.Columns["PatinaID"].Visible = false;

            TechnoColorsDataGrid.Columns["FrontID"].Visible = false;
            TechnoColorsDataGrid.Columns["ColorID"].Visible = false;
            TechnoColorsDataGrid.Columns["TechnoColorID"].Visible = false;
            TechnoColorsDataGrid.Columns["Width"].Visible = false;
            TechnoColorsDataGrid.Columns["PatinaID"].Visible = false;

            InsetTypesDataGrid.Columns["FrontID"].Visible = false;
            InsetTypesDataGrid.Columns["Width"].Visible = false;
            InsetTypesDataGrid.Columns["PatinaID"].Visible = false;
            InsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            InsetTypesDataGrid.Columns["ColorID"].Visible = false;
            InsetTypesDataGrid.Columns["TechnoColorID"].Visible = false;

            InsetColorsDataGrid.Columns["FrontID"].Visible = false;
            InsetColorsDataGrid.Columns["Width"].Visible = false;
            InsetColorsDataGrid.Columns["InsetTypeID"].Visible = false;
            InsetColorsDataGrid.Columns["PatinaID"].Visible = false;
            InsetColorsDataGrid.Columns["ColorID"].Visible = false;
            InsetColorsDataGrid.Columns["TechnoColorID"].Visible = false;
            InsetColorsDataGrid.Columns["InsetColorID"].Visible = false;

            TechnoInsetTypesDataGrid.Columns["FrontID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["Width"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["PatinaID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["InsetColorID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["ColorID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["TechnoColorID"].Visible = false;

            TechnoInsetColorsDataGrid.Columns["FrontID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["Width"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["PatinaID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["ColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["TechnoColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["InsetTypeID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["InsetColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            SizesDataGrid.Columns["FrontID"].Visible = false;
            SizesDataGrid.Columns["PatinaID"].Visible = false;
            SizesDataGrid.Columns["Height"].Visible = false;
            SizesDataGrid.Columns["Width"].Visible = false;
            SizesDataGrid.Columns["ColorID"].Visible = false;
            SizesDataGrid.Columns["TechnoColorID"].Visible = false;
            SizesDataGrid.Columns["InsetColorID"].Visible = false;
            SizesDataGrid.Columns["InsetTypeID"].Visible = false;
            SizesDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            SizesDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            FrontsDataGrid.Columns["Front"].HeaderText = "Фасад";
            FrontsDataGrid.Columns["Cost"].HeaderText = " € ";
            FrontsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            FrontsDataGrid.Columns["Count"].HeaderText = "шт.";

            FrameColorsDataGrid.Columns["FrameColor"].HeaderText = "Цвет профиля";
            FrameColorsDataGrid.Columns["Cost"].HeaderText = " € ";
            FrameColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            FrameColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            TechnoColorsDataGrid.Columns["TechnoColor"].HeaderText = "Цвет профиля-2";
            TechnoColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            TechnoColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            SizesDataGrid.Columns["Size"].HeaderText = "Размер";
            SizesDataGrid.Columns["Square"].HeaderText = "м.кв.";
            SizesDataGrid.Columns["Count"].HeaderText = "шт.";

            InsetTypesDataGrid.Columns["InsetType"].HeaderText = "Тип наполнителя";
            InsetTypesDataGrid.Columns["Square"].HeaderText = "м.кв.";
            InsetTypesDataGrid.Columns["Count"].HeaderText = "шт.";

            InsetColorsDataGrid.Columns["InsetColor"].HeaderText = "Цвет наполнителя";
            InsetColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            InsetColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            TechnoInsetTypesDataGrid.Columns["TechnoInsetType"].HeaderText = "Тип наполнителя-2";
            TechnoInsetTypesDataGrid.Columns["Square"].HeaderText = "м.кв.";
            TechnoInsetTypesDataGrid.Columns["Count"].HeaderText = "шт.";

            TechnoInsetColorsDataGrid.Columns["TechnoInsetColor"].HeaderText = "Цвет наполнителя-2";
            TechnoInsetColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            TechnoInsetColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            FrontsDataGrid.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsDataGrid.Columns["Front"].MinimumWidth = 110;
            FrameColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrameColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrameColorsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrontsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrontsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrontsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrontsDataGrid.Columns["Square"].Width = 100;
            //FrontsDataGrid.Columns["Cost"].Width = 100;
            //FrontsDataGrid.Columns["Count"].Width = 90;

            FrameColorsDataGrid.Columns["FrameColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrameColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrameColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrameColorsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrameColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrameColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrameColorsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrameColorsDataGrid.Columns["Square"].Width = 100;
            //FrameColorsDataGrid.Columns["Cost"].Width = 100;
            //FrameColorsDataGrid.Columns["Count"].Width = 90;

            TechnoColorsDataGrid.Columns["TechnoColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechnoColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //TechnoColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoColorsDataGrid.Columns["Square"].Width = 100;
            //TechnoColorsDataGrid.Columns["Count"].Width = 90;

            SizesDataGrid.Columns["Size"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SizesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //SizesDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //SizesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //SizesDataGrid.Columns["Square"].Width = 100;
            //SizesDataGrid.Columns["Count"].Width = 90;

            InsetTypesDataGrid.Columns["InsetType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetTypesDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            InsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //InsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //InsetTypesDataGrid.Columns["Square"].Width = 100;
            //InsetTypesDataGrid.Columns["Count"].Width = 90;

            InsetColorsDataGrid.Columns["InsetColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            InsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //InsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //InsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //InsetColorsDataGrid.Columns["Square"].Width = 100;
            //InsetColorsDataGrid.Columns["Count"].Width = 90;

            TechnoInsetTypesDataGrid.Columns["TechnoInsetType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetTypesDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechnoInsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //TechnoInsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoInsetTypesDataGrid.Columns["Square"].Width = 100;
            //TechnoInsetTypesDataGrid.Columns["Count"].Width = 90;

            TechnoInsetColorsDataGrid.Columns["TechnoInsetColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechnoInsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //TechnoInsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoInsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoInsetColorsDataGrid.Columns["Square"].Width = 100;
            //TechnoInsetColorsDataGrid.Columns["Count"].Width = 90;

            FrameColorsDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            FrameColorsDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FrameColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            FrameColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;
            TechnoColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            TechnoColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;
            FrontsDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            FrontsDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FrontsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            FrontsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            SizesDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            SizesDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            InsetTypesDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            InsetTypesDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            InsetColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            InsetColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            TechnoInsetTypesDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            TechnoInsetTypesDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            TechnoInsetColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            TechnoInsetColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            DecorProductsDataGrid.Columns["ProductID"].Visible = false;
            DecorProductsDataGrid.Columns["MeasureID"].Visible = false;

            DecorItemsDataGrid.Columns["ProductID"].Visible = false;
            DecorItemsDataGrid.Columns["DecorID"].Visible = false;
            DecorItemsDataGrid.Columns["MeasureID"].Visible = false;

            DecorColorsDataGrid.Columns["ProductID"].Visible = false;
            DecorColorsDataGrid.Columns["DecorID"].Visible = false;
            DecorColorsDataGrid.Columns["MeasureID"].Visible = false;
            DecorColorsDataGrid.Columns["ColorID"].Visible = false;

            DecorSizesDataGrid.Columns["ProductID"].Visible = false;
            DecorSizesDataGrid.Columns["DecorID"].Visible = false;
            DecorSizesDataGrid.Columns["MeasureID"].Visible = false;
            DecorSizesDataGrid.Columns["ColorID"].Visible = false;
            DecorSizesDataGrid.Columns["Height"].Visible = false;
            DecorSizesDataGrid.Columns["Length"].Visible = false;
            DecorSizesDataGrid.Columns["Width"].Visible = false;

            DecorProductsDataGrid.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DecorProductsDataGrid.Columns["DecorProduct"].MinimumWidth = 100;
            DecorProductsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorProductsDataGrid.Columns["Cost"].Width = 100;
            DecorProductsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorProductsDataGrid.Columns["Count"].Width = 100;
            DecorProductsDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorProductsDataGrid.Columns["Measure"].Width = 90;

            DecorProductsDataGrid.Columns["DecorProduct"].HeaderText = "Продукт";
            DecorProductsDataGrid.Columns["Cost"].HeaderText = " € ";
            DecorProductsDataGrid.Columns["Count"].HeaderText = "Кол-во";
            DecorProductsDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            DecorProductsDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            DecorProductsDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            DecorProductsDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            DecorProductsDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            DecorItemsDataGrid.Columns["DecorID"].Visible = false;

            DecorItemsDataGrid.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DecorItemsDataGrid.Columns["DecorItem"].MinimumWidth = 100;
            DecorItemsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorItemsDataGrid.Columns["Cost"].Width = 100;
            DecorItemsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorItemsDataGrid.Columns["Count"].Width = 100;
            DecorItemsDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorItemsDataGrid.Columns["Measure"].Width = 90;

            DecorItemsDataGrid.Columns["DecorItem"].HeaderText = "Наименование";
            DecorItemsDataGrid.Columns["Cost"].HeaderText = " € ";
            DecorItemsDataGrid.Columns["Count"].HeaderText = "Кол-во";
            DecorItemsDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            DecorItemsDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            DecorItemsDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            DecorItemsDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            DecorItemsDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            DecorColorsDataGrid.Columns["ColorID"].Visible = false;

            DecorColorsDataGrid.Columns["Color"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DecorColorsDataGrid.Columns["Color"].MinimumWidth = 150;
            DecorColorsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorColorsDataGrid.Columns["Cost"].Width = 100;
            DecorColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorColorsDataGrid.Columns["Count"].Width = 100;
            DecorColorsDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorColorsDataGrid.Columns["Measure"].Width = 90;

            DecorColorsDataGrid.Columns["Color"].HeaderText = "Цвет";
            DecorColorsDataGrid.Columns["Cost"].HeaderText = " € ";
            DecorColorsDataGrid.Columns["Count"].HeaderText = "Кол-во";
            DecorColorsDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            DecorColorsDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            DecorColorsDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            DecorColorsDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            DecorColorsDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;


            DecorSizesDataGrid.Columns["Size"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorSizesDataGrid.Columns["Size"].MinimumWidth = 100;
            DecorSizesDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorSizesDataGrid.Columns["Cost"].Width = 100;
            DecorSizesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorSizesDataGrid.Columns["Count"].Width = 100;
            DecorSizesDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorSizesDataGrid.Columns["Measure"].Width = 90;

            DecorSizesDataGrid.Columns["Size"].HeaderText = "Размер";
            DecorSizesDataGrid.Columns["Cost"].HeaderText = " € ";
            DecorSizesDataGrid.Columns["Count"].HeaderText = "Кол-во";
            DecorSizesDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            DecorSizesDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            DecorSizesDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            DecorSizesDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            DecorSizesDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            FrontsDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrontsDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrontsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrameColorsDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrameColorsDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrameColorsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            SizesDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            SizesDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetTypesDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetTypesDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetColorsDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetColorsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            DecorProductsDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorProductsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorItemsDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorItemsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorColorsDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorColorsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorSizesDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorSizesDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void CommonStatisticsRebind()
        {
            FrontsDataGrid.DataSource = AllProductsStatistics.FrontsSummaryBindingSource;
            FrameColorsDataGrid.DataSource = AllProductsStatistics.FrameColorsSummaryBindingSource;
            TechnoColorsDataGrid.DataSource = AllProductsStatistics.TechnoColorsSummaryBindingSource;
            InsetTypesDataGrid.DataSource = AllProductsStatistics.InsetTypesSummaryBindingSource;
            InsetColorsDataGrid.DataSource = AllProductsStatistics.InsetColorsSummaryBindingSource;
            TechnoInsetTypesDataGrid.DataSource = AllProductsStatistics.TechnoInsetTypesSummaryBindingSource;
            TechnoInsetColorsDataGrid.DataSource = AllProductsStatistics.TechnoInsetColorsSummaryBindingSource;
            SizesDataGrid.DataSource = AllProductsStatistics.SizesSummaryBindingSource;
            DecorProductsDataGrid.DataSource = AllProductsStatistics.DecorProductsSummaryBindingSource;
            DecorItemsDataGrid.DataSource = AllProductsStatistics.DecorItemsSummaryBindingSource;
            DecorColorsDataGrid.DataSource = AllProductsStatistics.DecorColorsSummaryBindingSource;
            DecorSizesDataGrid.DataSource = AllProductsStatistics.DecorSizesSummaryBindingSource;
        }

        private void ZOVOrdersStatisticsRebind()
        {
            FrontsDataGrid.DataSource = ZOVOrdersStatistics.FrontsSummaryBindingSource;
            FrameColorsDataGrid.DataSource = ZOVOrdersStatistics.FrameColorsSummaryBindingSource;
            TechnoColorsDataGrid.DataSource = ZOVOrdersStatistics.TechnoColorsSummaryBindingSource;
            InsetTypesDataGrid.DataSource = ZOVOrdersStatistics.InsetTypesSummaryBindingSource;
            InsetColorsDataGrid.DataSource = ZOVOrdersStatistics.InsetColorsSummaryBindingSource;
            TechnoInsetTypesDataGrid.DataSource = ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource;
            TechnoInsetColorsDataGrid.DataSource = ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource;
            SizesDataGrid.DataSource = ZOVOrdersStatistics.SizesSummaryBindingSource;
            DecorProductsDataGrid.DataSource = ZOVOrdersStatistics.DecorProductsSummaryBindingSource;
            DecorItemsDataGrid.DataSource = ZOVOrdersStatistics.DecorItemsSummaryBindingSource;
            DecorColorsDataGrid.DataSource = ZOVOrdersStatistics.DecorColorsSummaryBindingSource;
            DecorSizesDataGrid.DataSource = ZOVOrdersStatistics.DecorSizesSummaryBindingSource;
        }

        private void MegaBatchGridSettings()
        {
            foreach (DataGridViewColumn Column in dgvGeneralSummary.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //dgvGeneralSummary.Columns["Firm"].Visible = false;
            dgvGeneralSummary.Columns["GroupType"].Visible = false;
            dgvGeneralSummary.Columns["CreateUserID"].Visible = false;
            dgvGeneralSummary.Columns["ProfilReady"].Visible = false;
            dgvGeneralSummary.Columns["TPSReady"].Visible = false;
            dgvGeneralSummary.Columns["ProfilCloseUserID"].Visible = false;
            dgvGeneralSummary.Columns["TPSCloseUserID"].Visible = false;

            dgvGeneralSummary.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["TPSEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["ProfilPackingDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["TPSPackingDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvGeneralSummary.Columns["Firm"].HeaderText = "Участок";
            dgvGeneralSummary.Columns["Notes"].HeaderText = "Примечание";
            dgvGeneralSummary.Columns["CreateDateTime"].HeaderText = "Партия\n\rсоздана";
            dgvGeneralSummary.Columns["MegaBatchID"].HeaderText = "№ группы\n\rпартий";
            dgvGeneralSummary.Columns["TPSEntryDateTime"].HeaderText = "Вход на пр-во\n\rТПС";
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].HeaderText = "На пр-ве\n\rТПС, %";
            dgvGeneralSummary.Columns["TPSInProductionPerc"].HeaderText = "В пр-ве\n\rТПС, %";
            dgvGeneralSummary.Columns["FilenkaPerc"].HeaderText = "Филенка\n\rТПС, %";
            dgvGeneralSummary.Columns["TrimmingPerc"].HeaderText = "Торцовка\n\rТПС, %";
            dgvGeneralSummary.Columns["AssemblyPerc"].HeaderText = "Сборка\n\rТПС, %";
            dgvGeneralSummary.Columns["DeyingPerc"].HeaderText = "Покраска\n\rТПС, %";
            dgvGeneralSummary.Columns["TPSReadyPerc"].HeaderText = "Готовность\n\rТПС, %";
            dgvGeneralSummary.Columns["TPSPackingDate"].HeaderText = "Выход с пр-ва\n\rТПС";
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].HeaderText = "Вход на пр-во\n\rПрофиль";
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].HeaderText = "На пр-ве\n\rПрофиль, %";
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].HeaderText = "В пр-ве\n\rПрофиль, %";
            dgvGeneralSummary.Columns["ProfilReadyPerc"].HeaderText = "Готовность\n\r Профиль, %";
            dgvGeneralSummary.Columns["ProfilPackingDate"].HeaderText = "Выход с пр-ва\n\rПрофиль";

            dgvGeneralSummary.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["Notes"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["Firm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["Firm"].MinimumWidth = 70;
            dgvGeneralSummary.Columns["MegaBatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["MegaBatchID"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["CreateDateTime"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["TPSEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["TPSEntryDateTime"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["FilenkaPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["FilenkaPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["TrimmingPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["TrimmingPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["AssemblyPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["AssemblyPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["DeyingPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["DeyingPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["TPSReadyPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["TPSReadyPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["TPSPackingDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["TPSPackingDate"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilPackingDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["ProfilPackingDate"].MinimumWidth = 120;

            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("ProfilOnProductionPerc");
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("ProfilInProductionPerc");
            dgvGeneralSummary.Columns["ProfilReadyPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("ProfilReadyPerc");

            dgvGeneralSummary.Columns["TPSOnProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("TPSOnProductionPerc");
            dgvGeneralSummary.Columns["TPSInProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("TPSInProductionPerc");
            dgvGeneralSummary.Columns["FilenkaPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("FilenkaPerc");
            dgvGeneralSummary.Columns["TrimmingPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("TrimmingPerc");
            dgvGeneralSummary.Columns["AssemblyPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("AssemblyPerc");
            dgvGeneralSummary.Columns["DeyingPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("DeyingPerc");
            dgvGeneralSummary.Columns["TPSReadyPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("TPSReadyPerc");

            dgvGeneralSummary.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvGeneralSummary.Columns["Firm"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["MegaBatchID"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["CreateDateTime"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSEntryDateTime"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["FilenkaPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TrimmingPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["AssemblyPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["DeyingPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSReadyPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSPackingDate"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilPackingDate"].DisplayIndex = DisplayIndex++;
        }

        public void ShowBatchColumns(bool Profil, bool TPS)
        {
            dgvGeneralSummary.Columns["TPSEntryDateTime"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["FilenkaPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["TrimmingPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["AssemblyPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["DeyingPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSReadyPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSPackingDate"].Visible = TPS;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilPackingDate"].Visible = Profil;
            if (!Profil && !TPS)
            {
                dgvGeneralSummary.Columns["MegaBatchID"].Visible = false;
                dgvGeneralSummary.Columns["Notes"].Visible = false;
            }
            else
            {
                dgvGeneralSummary.Columns["MegaBatchID"].Visible = true;
                dgvGeneralSummary.Columns["Notes"].Visible = true;
            }
        }

        private void ProductGridSettings()
        {
            dgvSimpleFronts.Columns["AllCount"].Visible = false;
            dgvSimpleFronts.Columns["OnProdCount"].Visible = false;
            dgvSimpleFronts.Columns["InProdCount"].Visible = false;
            dgvSimpleFronts.Columns["ReadyCount"].Visible = false;
            dgvSimpleFronts.Columns["Ready"].Visible = false;

            dgvCurvedFronts.Columns["Ready"].Visible = false;

            dgvDecor.Columns["Ready"].Visible = false;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            foreach (DataGridViewColumn Column in dgvSimpleFronts.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in dgvDecor.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgvSimpleFronts.Columns["FrontID"].Visible = false;
            dgvSimpleFronts.Columns["Width"].Visible = false;
            dgvCurvedFronts.Columns["FrontID"].Visible = false;
            dgvCurvedFronts.Columns["Width"].Visible = false;

            dgvSimpleFronts.Columns["Front"].HeaderText = "Фасад";
            dgvSimpleFronts.Columns["AllSquare"].HeaderText = "Общая площадь, м.кв.";
            dgvSimpleFronts.Columns["InProdSquare"].HeaderText = "В пр-ве, м.кв.";
            dgvSimpleFronts.Columns["OnProdSquare"].HeaderText = "На пр-ве, м.кв.";
            dgvSimpleFronts.Columns["ReadySquare"].HeaderText = "Готово, м.кв.";
            dgvSimpleFronts.Columns["AllCount"].HeaderText = "Общее кол-во, шт.";
            dgvSimpleFronts.Columns["InProdCount"].HeaderText = "В пр-ве, шт.";
            dgvSimpleFronts.Columns["OnProdCount"].HeaderText = "На пр-ве, шт.";
            dgvSimpleFronts.Columns["ReadyCount"].HeaderText = "Готово, шт.";

            dgvCurvedFronts.Columns["Front"].HeaderText = "Фасад";
            dgvCurvedFronts.Columns["AllCount"].HeaderText = "Общее кол-во, шт.";
            dgvCurvedFronts.Columns["InProdCount"].HeaderText = "В пр-ве, шт.";
            dgvCurvedFronts.Columns["OnProdCount"].HeaderText = "На пр-ве, шт.";
            dgvCurvedFronts.Columns["ReadyCount"].HeaderText = "Готово, шт.";

            dgvSimpleFronts.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["Front"].MinimumWidth = 245;
            dgvSimpleFronts.Columns["AllSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["AllSquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["InProdSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["InProdSquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["OnProdSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["OnProdSquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["ReadySquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["ReadySquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["InProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["InProdCount"].MinimumWidth = 110;
            dgvSimpleFronts.Columns["OnProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["OnProdCount"].MinimumWidth = 110;
            dgvSimpleFronts.Columns["ReadyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["ReadyCount"].MinimumWidth = 110;
            dgvSimpleFronts.Columns["AllCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["AllCount"].MinimumWidth = 110;

            dgvSimpleFronts.Columns["AllSquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["AllSquare"].DefaultCellStyle.FormatProvider = nfi1;
            dgvSimpleFronts.Columns["InProdSquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["InProdSquare"].DefaultCellStyle.FormatProvider = nfi1;
            dgvSimpleFronts.Columns["OnProdSquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["OnProdSquare"].DefaultCellStyle.FormatProvider = nfi1;
            dgvSimpleFronts.Columns["ReadySquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["ReadySquare"].DefaultCellStyle.FormatProvider = nfi1;

            dgvDecor.Columns["ProductID"].Visible = false;
            dgvDecor.Columns["MeasureID"].Visible = false;

            dgvDecor.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["DecorProduct"].MinimumWidth = 245;
            dgvDecor.Columns["AllCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["AllCount"].MinimumWidth = 150;
            dgvDecor.Columns["InProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["InProdCount"].MinimumWidth = 150;
            dgvDecor.Columns["OnProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["OnProdCount"].MinimumWidth = 150;
            dgvDecor.Columns["ReadyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["ReadyCount"].MinimumWidth = 150;
            dgvDecor.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecor.Columns["Measure"].MinimumWidth = 140;

            dgvDecor.Columns["DecorProduct"].HeaderText = "Продукт";
            dgvDecor.Columns["AllCount"].HeaderText = "Общее кол-во";
            dgvDecor.Columns["InProdCount"].HeaderText = "В пр-ве, кол-во";
            dgvDecor.Columns["OnProdCount"].HeaderText = "На пр-ве, кол-во";
            dgvDecor.Columns["ReadyCount"].HeaderText = "Готово, кол-во";
            dgvDecor.Columns["Measure"].HeaderText = "Ед.изм.";

            dgvDecor.Columns["AllCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["AllCount"].DefaultCellStyle.FormatProvider = nfi1;
            dgvDecor.Columns["InProdCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["InProdCount"].DefaultCellStyle.FormatProvider = nfi1;
            dgvDecor.Columns["OnProdCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["OnProdCount"].DefaultCellStyle.FormatProvider = nfi1;
            dgvDecor.Columns["ReadyCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["ReadyCount"].DefaultCellStyle.FormatProvider = nfi1;

            dgvSimpleFronts.Columns["AllSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["InProdSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["OnProdSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["ReadySquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["AllCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["InProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["OnProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["ReadyCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCurvedFronts.Columns["AllCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCurvedFronts.Columns["InProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCurvedFronts.Columns["OnProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCurvedFronts.Columns["ReadyCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDecor.Columns["AllCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDecor.Columns["InProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDecor.Columns["OnProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDecor.Columns["ReadyCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConditionOrdersGrids1()
        {
            foreach (DataGridViewColumn Column in ConditionGroupsDG.Columns)
            {
                Column.ReadOnly = true;
            }

            ConditionGroupsDG.Columns["ClientGroupID"].Visible = false;
            ConditionGroupsDG.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ConditionGroupsDG.Columns["Check"].Width = 40;
            ConditionGroupsDG.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ConditionGroupsDG.AutoGenerateColumns = false;
            ConditionGroupsDG.Columns["Check"].ReadOnly = false;
            ConditionGroupsDG.Columns["Check"].DisplayIndex = 0;
            ConditionGroupsDG.Columns["ClientGroupName"].DisplayIndex = 1;

            MondayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT);
            MondayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT);
            MondayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT);

            if (!Security.PriceAccess)
            {
                MondayFrontsDG.Columns["Cost"].Visible = false;
                MondayDecorProductsDG.Columns["Cost"].Visible = false;
                MondayDecorItemsDG.Columns["Cost"].Visible = false;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            MondayFrontsDG.ColumnHeadersHeight = 38;
            MondayDecorProductsDG.ColumnHeadersHeight = 38;
            MondayDecorItemsDG.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in MondayFrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in MondayDecorProductsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in MondayDecorItemsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            MondayFrontsDG.Columns["FrontID"].Visible = false;
            MondayFrontsDG.Columns["Width"].Visible = false;

            MondayFrontsDG.Columns["Front"].HeaderText = "Фасад";
            MondayFrontsDG.Columns["Cost"].HeaderText = " € ";
            MondayFrontsDG.Columns["Square"].HeaderText = "м.кв.";
            MondayFrontsDG.Columns["Count"].HeaderText = "шт.";

            MondayFrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MondayFrontsDG.Columns["Front"].MinimumWidth = 110;
            MondayFrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayFrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayFrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayFrontsDG.Columns["Square"].Width = 100;
            MondayFrontsDG.Columns["Cost"].Width = 100;
            MondayFrontsDG.Columns["Count"].Width = 90;

            MondayFrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            MondayFrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            MondayFrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            MondayFrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            MondayDecorProductsDG.Columns["ProductID"].Visible = false;
            MondayDecorProductsDG.Columns["MeasureID"].Visible = false;

            MondayDecorItemsDG.Columns["ProductID"].Visible = false;
            MondayDecorItemsDG.Columns["DecorID"].Visible = false;
            MondayDecorItemsDG.Columns["MeasureID"].Visible = false;

            MondayDecorProductsDG.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MondayDecorProductsDG.Columns["DecorProduct"].MinimumWidth = 100;
            MondayDecorProductsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorProductsDG.Columns["Cost"].Width = 100;
            MondayDecorProductsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorProductsDG.Columns["Count"].Width = 100;
            MondayDecorProductsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorProductsDG.Columns["Measure"].Width = 90;

            MondayDecorProductsDG.Columns["DecorProduct"].HeaderText = "Продукт";
            MondayDecorProductsDG.Columns["Cost"].HeaderText = " € ";
            MondayDecorProductsDG.Columns["Count"].HeaderText = "Кол-во";
            MondayDecorProductsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            MondayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            MondayDecorProductsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            MondayDecorProductsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            MondayDecorProductsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            MondayDecorItemsDG.Columns["DecorID"].Visible = false;

            MondayDecorItemsDG.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MondayDecorItemsDG.Columns["DecorItem"].MinimumWidth = 100;
            MondayDecorItemsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorItemsDG.Columns["Cost"].Width = 100;
            MondayDecorItemsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorItemsDG.Columns["Count"].Width = 100;
            MondayDecorItemsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorItemsDG.Columns["Measure"].Width = 90;

            MondayDecorItemsDG.Columns["DecorItem"].HeaderText = "Наименование";
            MondayDecorItemsDG.Columns["Cost"].HeaderText = " € ";
            MondayDecorItemsDG.Columns["Count"].HeaderText = "Кол-во";
            MondayDecorItemsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            MondayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            MondayDecorItemsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            MondayDecorItemsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            MondayDecorItemsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            MondayFrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayFrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayFrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MondayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayDecorProductsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayDecorItemsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConditionOrdersGrids2()
        {
            WednesdayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT);
            WednesdayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT);
            WednesdayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT);

            if (!Security.PriceAccess)
            {
                WednesdayFrontsDG.Columns["Cost"].Visible = false;
                WednesdayDecorProductsDG.Columns["Cost"].Visible = false;
                WednesdayDecorItemsDG.Columns["Cost"].Visible = false;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            WednesdayFrontsDG.ColumnHeadersHeight = 38;
            WednesdayDecorProductsDG.ColumnHeadersHeight = 38;
            WednesdayDecorItemsDG.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in WednesdayFrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in WednesdayDecorProductsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in WednesdayDecorItemsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            WednesdayFrontsDG.Columns["FrontID"].Visible = false;
            WednesdayFrontsDG.Columns["Width"].Visible = false;

            WednesdayFrontsDG.Columns["Front"].HeaderText = "Фасад";
            WednesdayFrontsDG.Columns["Cost"].HeaderText = " € ";
            WednesdayFrontsDG.Columns["Square"].HeaderText = "м.кв.";
            WednesdayFrontsDG.Columns["Count"].HeaderText = "шт.";

            WednesdayFrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WednesdayFrontsDG.Columns["Front"].MinimumWidth = 110;
            WednesdayFrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayFrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayFrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayFrontsDG.Columns["Square"].Width = 100;
            WednesdayFrontsDG.Columns["Cost"].Width = 100;
            WednesdayFrontsDG.Columns["Count"].Width = 90;

            WednesdayFrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            WednesdayFrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            WednesdayFrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            WednesdayFrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            WednesdayDecorProductsDG.Columns["ProductID"].Visible = false;
            WednesdayDecorProductsDG.Columns["MeasureID"].Visible = false;

            WednesdayDecorItemsDG.Columns["ProductID"].Visible = false;
            WednesdayDecorItemsDG.Columns["DecorID"].Visible = false;
            WednesdayDecorItemsDG.Columns["MeasureID"].Visible = false;

            WednesdayDecorProductsDG.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WednesdayDecorProductsDG.Columns["DecorProduct"].MinimumWidth = 100;
            WednesdayDecorProductsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorProductsDG.Columns["Cost"].Width = 100;
            WednesdayDecorProductsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorProductsDG.Columns["Count"].Width = 100;
            WednesdayDecorProductsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorProductsDG.Columns["Measure"].Width = 90;

            WednesdayDecorProductsDG.Columns["DecorProduct"].HeaderText = "Продукт";
            WednesdayDecorProductsDG.Columns["Cost"].HeaderText = " € ";
            WednesdayDecorProductsDG.Columns["Count"].HeaderText = "Кол-во";
            WednesdayDecorProductsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            WednesdayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            WednesdayDecorProductsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            WednesdayDecorProductsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            WednesdayDecorProductsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            WednesdayDecorItemsDG.Columns["DecorID"].Visible = false;

            WednesdayDecorItemsDG.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WednesdayDecorItemsDG.Columns["DecorItem"].MinimumWidth = 100;
            WednesdayDecorItemsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorItemsDG.Columns["Cost"].Width = 100;
            WednesdayDecorItemsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorItemsDG.Columns["Count"].Width = 100;
            WednesdayDecorItemsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorItemsDG.Columns["Measure"].Width = 90;

            WednesdayDecorItemsDG.Columns["DecorItem"].HeaderText = "Наименование";
            WednesdayDecorItemsDG.Columns["Cost"].HeaderText = " € ";
            WednesdayDecorItemsDG.Columns["Count"].HeaderText = "Кол-во";
            WednesdayDecorItemsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            WednesdayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            WednesdayDecorItemsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            WednesdayDecorItemsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            WednesdayDecorItemsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            WednesdayFrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayFrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayFrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            WednesdayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayDecorProductsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayDecorItemsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConditionOrdersGrids3()
        {
            FridayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT);
            FridayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT);
            FridayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT);

            if (!Security.PriceAccess)
            {
                FridayFrontsDG.Columns["Cost"].Visible = false;
                FridayDecorProductsDG.Columns["Cost"].Visible = false;
                FridayDecorItemsDG.Columns["Cost"].Visible = false;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            FridayFrontsDG.ColumnHeadersHeight = 38;
            FridayDecorProductsDG.ColumnHeadersHeight = 38;
            FridayDecorItemsDG.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in FridayFrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in FridayDecorProductsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in FridayDecorItemsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            FridayFrontsDG.Columns["FrontID"].Visible = false;
            FridayFrontsDG.Columns["Width"].Visible = false;

            FridayFrontsDG.Columns["Front"].HeaderText = "Фасад";
            FridayFrontsDG.Columns["Cost"].HeaderText = " € ";
            FridayFrontsDG.Columns["Square"].HeaderText = "м.кв.";
            FridayFrontsDG.Columns["Count"].HeaderText = "шт.";

            FridayFrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FridayFrontsDG.Columns["Front"].MinimumWidth = 110;
            FridayFrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayFrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayFrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayFrontsDG.Columns["Square"].Width = 100;
            FridayFrontsDG.Columns["Cost"].Width = 100;
            FridayFrontsDG.Columns["Count"].Width = 90;

            FridayFrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FridayFrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FridayFrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            FridayFrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            FridayDecorProductsDG.Columns["ProductID"].Visible = false;
            FridayDecorProductsDG.Columns["MeasureID"].Visible = false;

            FridayDecorItemsDG.Columns["ProductID"].Visible = false;
            FridayDecorItemsDG.Columns["DecorID"].Visible = false;
            FridayDecorItemsDG.Columns["MeasureID"].Visible = false;

            FridayDecorProductsDG.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FridayDecorProductsDG.Columns["DecorProduct"].MinimumWidth = 100;
            FridayDecorProductsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorProductsDG.Columns["Cost"].Width = 100;
            FridayDecorProductsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorProductsDG.Columns["Count"].Width = 100;
            FridayDecorProductsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorProductsDG.Columns["Measure"].Width = 90;

            FridayDecorProductsDG.Columns["DecorProduct"].HeaderText = "Продукт";
            FridayDecorProductsDG.Columns["Cost"].HeaderText = " € ";
            FridayDecorProductsDG.Columns["Count"].HeaderText = "Кол-во";
            FridayDecorProductsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            FridayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FridayDecorProductsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FridayDecorProductsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            FridayDecorProductsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            FridayDecorItemsDG.Columns["DecorID"].Visible = false;

            FridayDecorItemsDG.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FridayDecorItemsDG.Columns["DecorItem"].MinimumWidth = 100;
            FridayDecorItemsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorItemsDG.Columns["Cost"].Width = 100;
            FridayDecorItemsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorItemsDG.Columns["Count"].Width = 100;
            FridayDecorItemsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorItemsDG.Columns["Measure"].Width = 90;

            FridayDecorItemsDG.Columns["DecorItem"].HeaderText = "Наименование";
            FridayDecorItemsDG.Columns["Cost"].HeaderText = " € ";
            FridayDecorItemsDG.Columns["Count"].HeaderText = "Кол-во";
            FridayDecorItemsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            FridayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FridayDecorItemsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FridayDecorItemsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            FridayDecorItemsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            FridayFrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayFrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayFrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            FridayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayDecorProductsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayDecorItemsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void StoreFrontsInfo()
        {
            StoreFrontsSquareLabel.Text = string.Empty;
            StoreFrontsCostLabel.Text = string.Empty;
            StoreFrontsCountLabel.Text = string.Empty;

            decimal ExpFrontCost = 0;
            decimal ExpFrontSquare = 0;
            int ExpFrontsCount = 0;

            StorageStatistics.GetFrontsInfo(ref ExpFrontSquare, ref ExpFrontCost, ref ExpFrontsCount);

            StoreFrontsSquareLabel.Text = ExpFrontSquare.ToString("N", nfi2);
            StoreFrontsCostLabel.Text = ExpFrontCost.ToString("N", nfi2);
            StoreFrontsCountLabel.Text = ExpFrontsCount.ToString();
        }

        private void StoreCurvedFrontsInfo()
        {
            StoreCurvedFrontsCostLabel.Text = string.Empty;
            StoreCurvedCountLabel.Text = string.Empty;

            decimal ExpFrontCost = 0;
            int ExpCurvedCount = 0;

            StorageStatistics.GetCurvedFrontsInfo(ref ExpFrontCost, ref ExpCurvedCount);

            StoreCurvedFrontsCostLabel.Text = ExpFrontCost.ToString("N", nfi2);
            StoreCurvedCountLabel.Text = ExpCurvedCount.ToString();
        }

        private void StoreDecorInfo()
        {
            StoreDecorPogonLabel.Text = string.Empty;
            StoreDecorCostLabel.Text = string.Empty;
            StoreDecorCountLabel.Text = string.Empty;

            decimal ExpDecorPogon = 0;
            decimal ExpDecorCost = 0;
            int ExpDecorCount = 0;

            StorageStatistics.GetDecorInfo(ref ExpDecorPogon, ref ExpDecorCost, ref ExpDecorCount);

            StoreDecorPogonLabel.Text = ExpDecorPogon.ToString("N", nfi2);
            StoreDecorCostLabel.Text = ExpDecorCost.ToString("N", nfi2);
            StoreDecorCountLabel.Text = ExpDecorCount.ToString();

            StoreDecorProductsDataGrid_SelectionChanged(null, null);
        }

        private void DispFrontsInfo()
        {
            DispFrontsSquareLabel.Text = string.Empty;
            DispFrontsCostLabel.Text = string.Empty;
            DispFrontsCountLabel.Text = string.Empty;
            DispCurvedCountLabel.Text = string.Empty;

            decimal DispFrontCost = 0;
            decimal DispFrontSquare = 0;
            int DispFrontsCount = 0;
            int DispCurvedCount = 0;

            DispatchStatistics.GetFrontsInfo(ref DispFrontSquare, ref DispFrontCost, ref DispFrontsCount, ref DispCurvedCount);

            DispFrontsSquareLabel.Text = DispFrontSquare.ToString("N", nfi2);
            DispFrontsCostLabel.Text = DispFrontCost.ToString("N", nfi2);
            DispFrontsCountLabel.Text = DispFrontsCount.ToString();
            DispCurvedCountLabel.Text = DispCurvedCount.ToString();
        }

        private void DispDecorInfo()
        {
            DispDecorPogonLabel.Text = string.Empty;
            DispDecorCostLabel.Text = string.Empty;
            DispDecorCountLabel.Text = string.Empty;

            decimal DispDecorPogon = 0;
            decimal DispDecorCost = 0;
            int DispDecorCount = 0;

            DispatchStatistics.GetDecorInfo(ref DispDecorPogon, ref DispDecorCost, ref DispDecorCount);

            DispDecorPogonLabel.Text = DispDecorPogon.ToString("N", nfi2);
            DispDecorCostLabel.Text = DispDecorCost.ToString("N", nfi2);
            DispDecorCountLabel.Text = DispDecorCount.ToString();

            DispDecorProductsDataGrid_SelectionChanged(null, null);
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

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            bool PlanDispDate = PlanDispDateRadioButton.Checked;
            //bool PrepareZOV = PrepareRadioButton.Checked;
            bool OrderDate = OrderDateRadioButton.Checked;
            bool ConfirmDate = ConfirmDateRadioButton.Checked;
            bool OnAgreement = OnAgreementRadioButton.Checked;
            bool PackDate = PackDateRadioButton.Checked;
            bool StoreDate = StoreDateRadioButton.Checked;
            bool ExpDate = ExpDateRadioButton.Checked;
            bool FactDispDate = FactDispDateRadioButton.Checked;
            bool OnProduction = OnProductionRadioButton.Checked;

            int PackageStatusID = -1;

            if (PackDate)
                PackageStatusID = 1;
            if (StoreDate)
                PackageStatusID = 2;
            if (FactDispDate)
                PackageStatusID = 3;
            if (ExpDate)
                PackageStatusID = 4;

            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;

            int FactoryID = 0;

            if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = 1;
            if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                FactoryID = 2;
            if (!ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = -1;

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                if (rbMarketing.Checked)
                {
                    if (PlanDispDate)
                        AllProductsStatistics.FilterByPlanDispatch(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    if (OrderDate)
                        AllProductsStatistics.FilterByOrderDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    if (ConfirmDate)
                        AllProductsStatistics.FilterByConfirmDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    if (OnAgreement)
                        AllProductsStatistics.FilterByOnAgreement(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    //if (PrepareZOV)
                    //    AllProductsStatistics.FilterByPrepare(From, To);

                    if (OnProduction)
                        AllProductsStatistics.FilterByOnProduction(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    if (PackDate || StoreDate || ExpDate || FactDispDate)
                        AllProductsStatistics.FilterByPackages(From, To, PackageStatusID, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);
                }
                if (rbZOV.Checked)
                {
                    if (PlanDispDate)
                        ZOVOrdersStatistics.FilterByPlanDispatch(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    if (OrderDate)
                        ZOVOrdersStatistics.FilterByOrderDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    if (ConfirmDate)
                        ZOVOrdersStatistics.FilterByConfirmDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    //if (OnAgreement)
                    //    ZOVOrdersStatistics.FilterByOnConfirmDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    //if (PrepareZOV)
                    //    ZOVOrdersStatistics.FilterByPrepare(From, To);

                    if (OnProduction)
                        ZOVOrdersStatistics.FilterByOnProduction(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    if (PackDate || StoreDate || ExpDate || FactDispDate)
                        ZOVOrdersStatistics.FilterByPackages(From, To, PackageStatusID, FactoryID, cbSamples.Checked, cbNotSamples.Checked);
                }
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                NeedSplash = true;
            }
            else
            {
                if (rbMarketing.Checked)
                {
                    if (PlanDispDate)
                        AllProductsStatistics.FilterByPlanDispatch(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    if (OrderDate)
                        AllProductsStatistics.FilterByOrderDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    if (ConfirmDate)
                        AllProductsStatistics.FilterByConfirmDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    //if (PrepareZOV)
                    //    AllProductsStatistics.FilterByPrepare(From, To);

                    if (OnProduction)
                        AllProductsStatistics.FilterByOnProduction(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);

                    if (PackDate || StoreDate || ExpDate || FactDispDate)
                        AllProductsStatistics.FilterByPackages(From, To, PackageStatusID, FactoryID, cbSamples.Checked, cbNotSamples.Checked, cbTransport.Checked);
                }
                if (rbZOV.Checked)
                {
                    if (PlanDispDate)
                        ZOVOrdersStatistics.FilterByPlanDispatch(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    if (OrderDate)
                        ZOVOrdersStatistics.FilterByOrderDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    if (ConfirmDate)
                        ZOVOrdersStatistics.FilterByConfirmDate(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    //if (PrepareZOV)
                    //    ZOVOrdersStatistics.FilterByPrepare(From, To);

                    if (OnProduction)
                        ZOVOrdersStatistics.FilterByOnProduction(From, To, FactoryID, cbSamples.Checked, cbNotSamples.Checked);

                    if (PackDate || StoreDate || ExpDate || FactDispDate)
                        ZOVOrdersStatistics.FilterByPackages(From, To, PackageStatusID, FactoryID, cbSamples.Checked, cbNotSamples.Checked);
                }
            }


            FrontsDataGrid_SelectionChanged(null, null);
            FrameColorsDataGrid_SelectionChanged(null, null);
            InsetTypesDataGrid_SelectionChanged(null, null);
            InsetColorsDataGrid_SelectionChanged(null, null);
            DecorProductsDataGrid_SelectionChanged(null, null);
            DecorItemsDataGrid_SelectionChanged(null, null);
            DecorColorsDataGrid_SelectionChanged(null, null);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (!MenuButton.Checked)
            {
                BatchMenuPanel.Visible = false;
                CommonMenuPanel.Visible = false;
                StoreMenuPanel.Visible = false;
                DispMenuPanel.Visible = false;
                ConditionMenuPanel.Visible = false;
                ExpMenuPanel.Visible = false;

                if (HelpCheckButton.Checked)
                    HelpPanel.Visible = true;
            }
            else
            {
                if (HelpPanel.Visible)
                    HelpPanel.Visible = false;

                if (tabControl6.SelectedIndex == 0)
                {
                    CommonMenuPanel.Visible = true;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }

                if (tabControl6.SelectedIndex == 1)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = true;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }

                if (tabControl6.SelectedIndex == 6)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 2)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = true;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 4)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = true;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 5)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = true;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 3)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = true;
                }
            }
        }

        private void FrontsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.FrontsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.FrontsSummaryBindingSource.Current)["FrontID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterFrameColors(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.FrontsSummaryBindingSource.Current).Row["FrontID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.FrontsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.FrontsSummaryBindingSource.Current)["FrontID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterFrameColors(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.FrontsSummaryBindingSource.Current).Row["FrontID"]));
                        }
                    }
            }
            FrontsSquareLabel.Text = string.Empty;
            FrontsCostLabel.Text = string.Empty;
            FrontsCountLabel.Text = string.Empty;
            CurvedCountLabel.Text = string.Empty;

            decimal FrontCost = 0;
            decimal FrontSquare = 0;
            int FrontsCount = 0;
            int CurvedCount = 0;

            if (rbMarketing.Checked && AllProductsStatistics != null)
            {
                AllProductsStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
            }
            if (rbZOV.Checked && ZOVOrdersStatistics != null)
            {
                ZOVOrdersStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
            }
            for (int i = 0; i < FrontsDataGrid.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(FrontsDataGrid.SelectedRows[i].Cells["Width"].Value) == -1)
                    CurvedCount += Convert.ToInt32(FrontsDataGrid.SelectedRows[i].Cells["Count"].Value);
                else
                {
                    FrontSquare += Convert.ToDecimal(FrontsDataGrid.SelectedRows[i].Cells["Square"].Value);
                    FrontsCount += Convert.ToInt32(FrontsDataGrid.SelectedRows[i].Cells["Count"].Value);
                }
                FrontCost += Convert.ToDecimal(FrontsDataGrid.SelectedRows[i].Cells["Cost"].Value);
            }

            FrontCost = Decimal.Round(FrontCost, 3, MidpointRounding.AwayFromZero);
            FrontSquare = Decimal.Round(FrontSquare, 3, MidpointRounding.AwayFromZero);

            FrontsSquareLabel.Text = FrontSquare.ToString("N", nfi2);
            FrontsCostLabel.Text = FrontCost.ToString("N", nfi2) + " €";
            FrontsCountLabel.Text = FrontsCount.ToString();
            CurvedCountLabel.Text = CurvedCount.ToString() + " шт.";

        }

        private void FrameColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.FrameColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.FrameColorsSummaryBindingSource.Current)["ColorID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterTechnoColors(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.FrameColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.FrameColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.FrameColorsSummaryBindingSource.Current).Row["PatinaID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.FrameColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.FrameColorsSummaryBindingSource.Current)["ColorID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterTechnoColors(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.FrameColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.FrameColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.FrameColorsSummaryBindingSource.Current).Row["PatinaID"]));
                        }
                    }
            }
        }

        private void TechnoColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.TechnoColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.TechnoColorsSummaryBindingSource.Current)["TechnoColorID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterInsetTypes(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoColorsSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoColorsSummaryBindingSource.Current).Row["TechnoColorID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.TechnoColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.TechnoColorsSummaryBindingSource.Current)["TechnoColorID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterInsetTypes(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoColorsSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoColorsSummaryBindingSource.Current).Row["TechnoColorID"]));
                        }
                    }
            }
        }

        private void InsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.InsetTypesSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.InsetTypesSummaryBindingSource.Current)["InsetTypeID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterInsetColors(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.InsetTypesSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.InsetTypesSummaryBindingSource.Current)["InsetTypeID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterInsetColors(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]));
                        }
                    }
            }
        }

        private void InsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.InsetColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.InsetColorsSummaryBindingSource.Current)["InsetColorID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterTechnoInsetTypes(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.InsetColorsSummaryBindingSource.Current).Row["InsetColorID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Current)["InsetColorID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterTechnoInsetTypes(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.InsetColorsSummaryBindingSource.Current).Row["InsetColorID"]));
                        }
                    }
            }
        }

        private void TechnoInsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current)["FrontID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterTechnoInsetColors(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["InsetColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["TechnoInsetTypeID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current)["FrontID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterTechnoInsetColors(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["InsetColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetTypesSummaryBindingSource.Current).Row["TechnoInsetTypeID"]));
                        }
                    }
            }
        }

        private void TechnoInsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current)["TechnoInsetColorID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterSizes(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["InsetColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetTypeID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetColorID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current)["TechnoInsetColorID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterSizes(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["InsetColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetTypeID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetColorID"]));
                        }
                    }
            }
        }

        private void DecorProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.DecorProductsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.DecorProductsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterDecorProducts(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorProductsSummaryBindingSource.Current).Row["ProductID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorProductsSummaryBindingSource.Current).Row["MeasureID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.DecorProductsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.DecorProductsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterDecorProducts(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorProductsSummaryBindingSource.Current).Row["ProductID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorProductsSummaryBindingSource.Current).Row["MeasureID"]));
                        }
                    }
            }
            decimal DecorPogon = 0;
            decimal DecorCost = 0;
            decimal DecorCount = 0;
            DecorPogonLabel.Text = string.Empty;
            DecorCostLabel.Text = string.Empty;
            DecorCountLabel.Text = string.Empty;

            for (int i = 0; i < DecorProductsDataGrid.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(DecorProductsDataGrid.SelectedRows[i].Cells["MeasureID"].Value) != 2)
                    DecorCount += Convert.ToDecimal(DecorProductsDataGrid.SelectedRows[i].Cells["Count"].Value);
                else
                {
                    DecorPogon += Convert.ToDecimal(DecorProductsDataGrid.SelectedRows[i].Cells["Count"].Value);
                }
                DecorCost += Convert.ToDecimal(DecorProductsDataGrid.SelectedRows[i].Cells["Cost"].Value);
            }

            DecorCost = Decimal.Round(DecorCost, 3, MidpointRounding.AwayFromZero);
            DecorPogon = Decimal.Round(DecorPogon, 3, MidpointRounding.AwayFromZero);

            DecorPogonLabel.Text = DecorPogon.ToString("N", nfi2);
            DecorCostLabel.Text = DecorCost.ToString("N", nfi2);
            DecorCountLabel.Text = DecorCount.ToString();
        }

        private void DecorItemsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.DecorItemsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.DecorItemsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterDecorItems(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorItemsSummaryBindingSource.Current).Row["ProductID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorItemsSummaryBindingSource.Current).Row["DecorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorItemsSummaryBindingSource.Current).Row["MeasureID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.DecorItemsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.DecorItemsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterDecorItems(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorItemsSummaryBindingSource.Current).Row["ProductID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorItemsSummaryBindingSource.Current).Row["DecorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorItemsSummaryBindingSource.Current).Row["MeasureID"]));
                        }
                    }
            }
        }

        private void DecorColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (rbMarketing.Checked)
            {
                if (AllProductsStatistics != null)
                    if (AllProductsStatistics.DecorColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)AllProductsStatistics.DecorColorsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                        {
                            AllProductsStatistics.FilterDecorSizes(
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorColorsSummaryBindingSource.Current).Row["ProductID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorColorsSummaryBindingSource.Current).Row["DecorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)AllProductsStatistics.DecorColorsSummaryBindingSource.Current).Row["MeasureID"]));
                        }
                    }
            }
            if (rbZOV.Checked)
            {
                if (ZOVOrdersStatistics != null)
                    if (ZOVOrdersStatistics.DecorColorsSummaryBindingSource.Count > 0)
                    {
                        if (((DataRowView)ZOVOrdersStatistics.DecorColorsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                        {
                            ZOVOrdersStatistics.FilterDecorSizes(
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorColorsSummaryBindingSource.Current).Row["ProductID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorColorsSummaryBindingSource.Current).Row["DecorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorColorsSummaryBindingSource.Current).Row["ColorID"]),
                                Convert.ToInt32(((DataRowView)ZOVOrdersStatistics.DecorColorsSummaryBindingSource.Current).Row["MeasureID"]));
                        }
                    }
            }
        }

        private void kryptonButton1_Click_1(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DecorCatalogOrder = new Modules.ZOV.DecorCatalogOrder();

            DateTime DateFrom = CalendarFrom.SelectionEnd;
            DateTime DateTo = CalendarTo.SelectionEnd;

            bool PlanDispDate = PlanDispDateRadioButton.Checked;
            //bool PrepareZOV = PrepareRadioButton.Checked;
            bool OrderDate = OrderDateRadioButton.Checked;
            bool ConfirmDate = ConfirmDateRadioButton.Checked;
            bool PackDate = PackDateRadioButton.Checked;
            bool StoreDate = StoreDateRadioButton.Checked;
            bool ExpDate = ExpDateRadioButton.Checked;
            bool FactDispDate = FactDispDateRadioButton.Checked;
            bool OnProduction = OnProductionRadioButton.Checked;

            string FileName = string.Empty;

            if (OnProduction)
            {
                FileName = "Вошло на пр-во " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Вошло на пр-во за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (PlanDispDate)
            {
                FileName = "Плановая отгрузка " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Плановая отгрузка за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (FactDispDate)
            {
                FileName = "Фактическая отгрузка " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Фактическая отгрузка за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (PackDate)
            {
                FileName = "Упакованная продукция " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Упакованная продукция за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (StoreDate)
            {
                FileName = "Продукция, принятая на на склад " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Продукция, принятая на склад за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (ExpDate)
            {
                FileName = "Продукция, принятая на экспедицию " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Продукция, принятая на экспедицию за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (OrderDate)
            {
                FileName = "Заказы, созданные " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Заказы, созданные за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (ConfirmDate)
            {
                FileName = "Заказы, согласованные " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Заказы, согласованные за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            int FactoryID = 0;

            if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = 1;
            if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                FactoryID = 2;
            if (!ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = -1;

            Modules.StatisticsMarketing.StatisticsReportByClient StatisticsReport = new Modules.StatisticsMarketing.StatisticsReportByClient(ref DecorCatalogOrder);

            if (rbMarketing.Checked)
            {
                StatisticsReport.CreateReport(DateFrom, DateTo, FactoryID, AllProductsStatistics.FrontsOrdersDataTable, AllProductsStatistics.DecorOrdersDataTable, FileName, false);
            }
            if (rbZOV.Checked)
            {
                StatisticsReport.CreateReport(DateFrom, DateTo, FactoryID, ZOVOrdersStatistics.FrontsOrdersDataTable, ZOVOrdersStatistics.DecorOrdersDataTable, FileName, true);
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DecorCatalogOrder = new Modules.ZOV.DecorCatalogOrder();

            DateTime DateFrom = CalendarFrom.SelectionEnd;
            DateTime DateTo = CalendarTo.SelectionEnd;

            bool PlanDispDate = PlanDispDateRadioButton.Checked;
            //bool PrepareZOV = PrepareRadioButton.Checked;
            bool OrderDate = OrderDateRadioButton.Checked;
            bool ConfirmDate = ConfirmDateRadioButton.Checked;
            bool PackDate = PackDateRadioButton.Checked;
            bool StoreDate = StoreDateRadioButton.Checked;
            bool ExpDate = ExpDateRadioButton.Checked;
            bool FactDispDate = FactDispDateRadioButton.Checked;
            bool OnProduction = OnProductionRadioButton.Checked;

            string FileName = string.Empty;

            if (OnProduction)
            {
                FileName = "Вошло на пр-во " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
                if (DateFrom != DateTo)
                    FileName = "Вошло на пр-во за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            }

            if (PlanDispDate)
            {
                FileName = "Плановая отгрузка " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
                if (DateFrom != DateTo)
                    FileName = "Плановая отгрузка за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            }

            //if (PrepareZOV)
            //{
            //    FileName = "Предварительно ЗОВ " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            //    if (DateFrom != DateTo)
            //        FileName = "Предварительно ЗОВ за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            //}

            if (FactDispDate)
            {
                FileName = "Фактическая отгрузка " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
                if (DateFrom != DateTo)
                    FileName = "Фактическая отгрузка за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            }

            if (PackDate)
            {
                FileName = "Упакованная продукция " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
                if (DateFrom != DateTo)
                    FileName = "Упакованная продукция за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            }

            if (StoreDate)
            {
                FileName = "Продукция, принятая на склад " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
                if (DateFrom != DateTo)
                    FileName = "Продукция, принятая на склад за период с " + DateFrom.ToString("yyyy-MM-dd") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            }

            if (ExpDate)
            {
                FileName = "Продукция, принятая на экспедицию " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
                if (DateFrom != DateTo)
                    FileName = "Продукция, принятая на экспедицию за период с " + DateFrom.ToString("yyyy-MM-dd") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по клиентам.";
            }

            if (OrderDate)
            {
                FileName = "Заказы, созданные " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
                if (DateFrom != DateTo)
                    FileName = "Заказы, созданные за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            }

            if (ConfirmDate)
            {
                FileName = "Заказы, согласованные " + DateFrom.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
                if (DateFrom != DateTo)
                    FileName = "Заказы, согласованные за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + ". Отчет по ассортименту.";
            }

            int FactoryID = 0;

            if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = 1;
            if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                FactoryID = 2;
            if (!ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = -1;

            Modules.StatisticsMarketing.StatisticsReportByProduction StatisticsReport = new Modules.StatisticsMarketing.StatisticsReportByProduction(ref DecorCatalogOrder);

            if (rbMarketing.Checked)
            {
                StatisticsReport.CreateReport(DateFrom, DateTo, FactoryID, AllProductsStatistics.FrontsOrdersDataTable, AllProductsStatistics.DecorOrdersDataTable, FileName);
            }
            if (rbZOV.Checked)
            {
                StatisticsReport.CreateReport(DateFrom, DateTo, FactoryID, ZOVOrdersStatistics.FrontsOrdersDataTable, ZOVOrdersStatistics.DecorOrdersDataTable, FileName);
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void AllClientsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ClientsDataGrid.Enabled = AllClientsCheckBox.Checked;
            dgvMarketManagers.Enabled = AllClientsCheckBox.Checked;
            AllProductsStatistics.CheckAllClientGroups(false);
            //AllProductsStatistics.ShowCheckColumn(ClientGroupsDataGrid, !AllClientsCheckBox.Checked);
            //AllProductsStatistics.ShowCheckColumn(ClientsDataGrid, AllClientsCheckBox.Checked);
            ClientGroupsCheckBox.Checked = !AllClientsCheckBox.Checked;
        }

        private void ClientGroupsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ClientGroupsDataGrid.Enabled = ClientGroupsCheckBox.Checked;
            AllProductsStatistics.CheckAllManagers(false);
            AllProductsStatistics.CheckAllClients(false);
            //AllProductsStatistics.ShowCheckColumn(ClientGroupsDataGrid, ClientGroupsCheckBox.Checked);
            //AllProductsStatistics.ShowCheckColumn(ClientsDataGrid, !ClientGroupsCheckBox.Checked);
            AllClientsCheckBox.Checked = !ClientGroupsCheckBox.Checked;
            kryptonCheckBox3.CheckState = CheckState.Unchecked;
            kryptonCheckBox3.Enabled = !ClientGroupsCheckBox.Checked;
        }

        public int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private void StoreDecorProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StorageStatistics != null)
                if (StorageStatistics.DecorProductsSummaryBS.Count > 0)
                {
                    if (((DataRowView)StorageStatistics.DecorProductsSummaryBS.Current)["ProductID"] != DBNull.Value)
                    {
                        StorageStatistics.FilterDecorProducts(
                            Convert.ToInt32(((DataRowView)StorageStatistics.DecorProductsSummaryBS.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)StorageStatistics.DecorProductsSummaryBS.Current).Row["MeasureID"]));
                    }
                }
        }

        private void ExpDecorProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ExpeditionStatistics != null)
                if (ExpeditionStatistics.DecorProductsSummaryBS.Count > 0)
                {
                    if (((DataRowView)ExpeditionStatistics.DecorProductsSummaryBS.Current)["ProductID"] != DBNull.Value)
                    {
                        ExpeditionStatistics.FilterDecorProducts(
                            Convert.ToInt32(((DataRowView)ExpeditionStatistics.DecorProductsSummaryBS.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)ExpeditionStatistics.DecorProductsSummaryBS.Current).Row["MeasureID"]));
                    }
                }
        }

        private void DispDecorProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (DispatchStatistics != null)
                if (DispatchStatistics.DecorProductsSummaryBS.Count > 0)
                {
                    if (((DataRowView)DispatchStatistics.DecorProductsSummaryBS.Current)["ProductID"] != DBNull.Value)
                    {
                        DispatchStatistics.FilterDecorProducts(
                            Convert.ToInt32(((DataRowView)DispatchStatistics.DecorProductsSummaryBS.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)DispatchStatistics.DecorProductsSummaryBS.Current).Row["MeasureID"]));
                    }
                }
        }

        private void StoreMarketingRB_CheckedChanged(object sender, EventArgs e)
        {
            if (StoreMarketingRB.Checked)
            {
                StoreClientSummaryCheckBox.Enabled = true;
            }
            else
            {
                bStoreSummaryClient = false;
                StoreClientSummaryCheckBox.Checked = false;
                StoreClientSummaryCheckBox.Enabled = true;
            }
        }

        private void StorePrepareRB_CheckedChanged(object sender, EventArgs e)
        {
            if (StorePrepareRB.Checked)
            {
                bStoreSummaryClient = false;
                StoreClientSummaryCheckBox.Checked = false;
                StoreClientSummaryCheckBox.Enabled = false;
            }
        }

        private void FilterStorage()
        {
            bool Profil = StoreProfilCheckBox.Checked;
            bool TPS = StoreTPSCheckBox.Checked;

            int FactoryID = 0;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                if (StoreMarketingRB.Checked)
                {
                    StorageStatistics.ShowColumns(ref StoreMFSummaryDG, ref StoreMCurvedFSummaryDG, ref StoreMDSummaryDG, Profil, TPS, bStoreSummaryClient);
                    StorageStatistics.FillMarketingTables();
                    StorageStatistics.CurvedFMarketingOrders(FactoryID, -1);
                    StorageStatistics.FMarketingOrders(FactoryID, -1);
                    StorageStatistics.DMarketingOrders(FactoryID, -1);

                    if (!StoreClientSummaryCheckBox.Checked)
                        StorageStatistics.MarketingSummary(FactoryID);
                    else
                        StorageStatistics.ClientSummary(FactoryID);

                    StoreMFSummaryDG.BringToFront();
                    StoreMCurvedFSummaryDG.BringToFront();
                    StoreMDSummaryDG.BringToFront();
                    StoreFrontsMarketLabel.Visible = true;
                    StoreFrontsPrepareLabel.Visible = false;
                    label68.Visible = true;
                    label69.Visible = false;
                    StoreDecorMarketLabel.Visible = true;
                    StoreDecorPrepareLabel.Visible = false;
                    if (!StorageStatistics.HasFronts)
                        StorageStatistics.ClearFrontsOrders(1);
                    if (!StorageStatistics.HasDecor)
                        StorageStatistics.ClearDecorOrders(1);
                }

                if (StorePrepareRB.Checked)
                {
                    StorageStatistics.ShowColumns(ref StorePrepareFSummaryDG, ref StorePrepareCurvedFSummaryDG, ref StorePrepareDSummaryDG, Profil, TPS, bStoreSummaryClient);
                    StorageStatistics.FillZOVPrepareTables();
                    StorageStatistics.PrepareOrders(FactoryID);
                    StorePrepareCurvedFSummaryDG.BringToFront();
                    StorePrepareFSummaryDG.BringToFront();
                    StorePrepareDSummaryDG.BringToFront();
                    StoreFrontsMarketLabel.Visible = false;
                    StoreFrontsPrepareLabel.Visible = true;
                    StoreDecorMarketLabel.Visible = false;
                    StoreDecorPrepareLabel.Visible = true;
                    label68.Visible = false;
                    label69.Visible = true;
                    if (!StorageStatistics.HasCurvedFronts)
                        StorageStatistics.ClearCurvedFrontsOrders(2);
                    if (!StorageStatistics.HasFronts)
                        StorageStatistics.ClearFrontsOrders(2);
                    if (!StorageStatistics.HasDecor)
                        StorageStatistics.ClearDecorOrders(2);
                }

                StoreFrontsInfo();
                StoreCurvedFrontsInfo();
                StoreDecorInfo();


                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void FilterDispatch()
        {
            bool Profil = DispProfilCheckBox.Checked;
            bool TPS = DispTPSCheckBox.Checked;
            bool DispClientSummary = DispClientSummaryCheckBox.Checked;

            int FactoryID = 0;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;

            DateTime FirstDate = DispatchDateFrom.SelectionEnd;
            DateTime SecondDate = DispatchDateTo.SelectionEnd;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            if (DispMarketingRB.Checked)
            {
                DispatchStatistics.FillMarketingTables(FirstDate, SecondDate);
                DispatchStatistics.ShowColumns(ref DispMFSummaryDG, ref DispMDSummaryDG, Profil, TPS);
                DispatchStatistics.FMarketingOrders(FirstDate, SecondDate, FactoryID, -1);
                DispatchStatistics.DMarketingOrders(FirstDate, SecondDate, FactoryID, -1);
                if (DispClientSummary)
                    DispatchStatistics.ClientSummary(FactoryID);
                else
                    DispatchStatistics.MarketingSummary(FactoryID);
                DispMFSummaryDG.BringToFront();
                DispMDSummaryDG.BringToFront();
                DispFrontsMarketLabel.Visible = true;
                DispDecorMarketLabel.Visible = true;
                DispFrontsZOVLabel.Visible = false;
                DispDecorZOVLabel.Visible = false;
            }
            if (DispZOVRB.Checked)
            {
                DispatchStatistics.FillZOVTables(FirstDate, SecondDate);
                DispatchStatistics.ShowColumns(ref DispZFSummaryDG, ref DispZDSummaryDG, Profil, TPS);
                DispatchStatistics.ZOVOrders(FirstDate, SecondDate, FactoryID);
                DispZFSummaryDG.BringToFront();
                DispZDSummaryDG.BringToFront();
                DispFrontsMarketLabel.Visible = false;
                DispDecorMarketLabel.Visible = false;
                DispFrontsZOVLabel.Visible = true;
                DispDecorZOVLabel.Visible = true;
            }

            DispFrontsInfo();
            DispDecorInfo();

            if (!DispatchStatistics.HasFronts)
                DispatchStatistics.ClearFrontsOrders(1);
            if (!DispatchStatistics.HasDecor)
                DispatchStatistics.ClearDecorOrders(1);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void DispProfilCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!DispMarketingRB.Checked)
                DispClientSummaryCheckBox.Enabled = false;
        }

        private void DispTPSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!DispMarketingRB.Checked)
                DispClientSummaryCheckBox.Enabled = false;
        }

        private void DispMarketingRB_CheckedChanged(object sender, EventArgs e)
        {
            DispClientSummaryCheckBox.Enabled = true;
        }

        private void DispZOVRB_CheckedChanged(object sender, EventArgs e)
        {
            DispClientSummaryCheckBox.Checked = false;
            DispClientSummaryCheckBox.Enabled = false;
        }

        private void StoreMFSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreOrdersSummaryCheckBox.Checked)
                return;

            if (StorageStatistics != null)
                if (StorageStatistics.MFSummaryBS.Count > 0)
                {
                    if (((DataRowView)StorageStatistics.MFSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = StoreProfilCheckBox.Checked;
                        bool TPS = StoreTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            if (StoreClientSummaryCheckBox.Checked)
                                StorageStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasFronts)
                                StorageStatistics.ClearFrontsOrders(1);

                            StoreFrontsInfo();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (StoreClientSummaryCheckBox.Checked)
                                StorageStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasFronts)
                                StorageStatistics.ClearFrontsOrders(1);

                            StoreFrontsInfo();
                        }
                    }
                }
        }

        private void ExpMFSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ExpOrdersSummaryCheckBox.Checked)
                return;

            if (ExpeditionStatistics != null)
                if (ExpeditionStatistics.MFSummaryBS.Count > 0)
                {
                    if (((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = ExpProfilCheckBox.Checked;
                        bool TPS = ExpTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            if (ExpClientSummaryCheckBox.Checked)
                                ExpeditionStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasFronts)
                                ExpeditionStatistics.ClearFrontsOrders(1);

                            ExpFrontsInfo();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (ExpClientSummaryCheckBox.Checked)
                                ExpeditionStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasFronts)
                                ExpeditionStatistics.ClearFrontsOrders(1);

                            ExpFrontsInfo();
                        }
                    }
                }
        }

        private void StoreMDSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreOrdersSummaryCheckBox.Checked)
                return;

            if (StorageStatistics != null)
                if (StorageStatistics.MDSummaryBS.Count > 0)
                {
                    if (((DataRowView)StorageStatistics.MDSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = StoreProfilCheckBox.Checked;
                        bool TPS = StoreTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            if (StoreClientSummaryCheckBox.Checked)
                                StorageStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasDecor)
                                StorageStatistics.ClearDecorOrders(1);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (StoreClientSummaryCheckBox.Checked)
                                StorageStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasDecor)
                                StorageStatistics.ClearDecorOrders(1);
                        }
                        StoreDecorInfo();
                    }
                }
        }

        private void ExpMDSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ExpOrdersSummaryCheckBox.Checked)
                return;

            if (ExpeditionStatistics != null)
                if (ExpeditionStatistics.MDSummaryBS.Count > 0)
                {
                    if (((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = ExpProfilCheckBox.Checked;
                        bool TPS = ExpTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            if (ExpClientSummaryCheckBox.Checked)
                                ExpeditionStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasDecor)
                                ExpeditionStatistics.ClearDecorOrders(1);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (ExpClientSummaryCheckBox.Checked)
                                ExpeditionStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasDecor)
                                ExpeditionStatistics.ClearDecorOrders(1);
                        }
                        ExpDecorInfo();
                    }
                }
        }

        private void DispMFSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (!NeedSelectionChange)
                return;

            if (!DispClientSummaryCheckBox.Checked)
                return;

            if (DispatchStatistics != null)
                if (DispatchStatistics.MFSummaryBS.Count > 0)
                {
                    if (((DataRowView)DispatchStatistics.MFSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = DispProfilCheckBox.Checked;
                        bool TPS = DispTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        DateTime FirstDate = DispatchDateFrom.SelectionEnd;
                        DateTime SecondDate = DispatchDateTo.SelectionEnd;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            DispatchStatistics.FMarketingOrders(FirstDate, SecondDate, FactoryID, Convert.ToInt32(((DataRowView)DispatchStatistics.MFSummaryBS.Current)["ClientID"]));

                            if (!DispatchStatistics.HasFronts)
                                DispatchStatistics.ClearFrontsOrders(1);

                            DispFrontsInfo();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            DispatchStatistics.FMarketingOrders(FirstDate, SecondDate, FactoryID, Convert.ToInt32(((DataRowView)DispatchStatistics.MFSummaryBS.Current)["ClientID"]));

                            if (!DispatchStatistics.HasFronts)
                                DispatchStatistics.ClearFrontsOrders(1);

                            DispFrontsInfo();
                        }
                    }
                }
        }

        private void DispMDSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (!NeedSelectionChange)
                return;

            if (!DispClientSummaryCheckBox.Checked)
                return;

            if (DispatchStatistics != null)
                if (DispatchStatistics.MDSummaryBS.Count > 0)
                {
                    if (((DataRowView)DispatchStatistics.MDSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = DispProfilCheckBox.Checked;
                        bool TPS = DispTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        DateTime FirstDate = DispatchDateFrom.SelectionEnd;
                        DateTime SecondDate = DispatchDateTo.SelectionEnd;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            DispatchStatistics.DMarketingOrders(FirstDate, SecondDate, FactoryID, Convert.ToInt32(((DataRowView)DispatchStatistics.MDSummaryBS.Current)["ClientID"]));

                            if (!DispatchStatistics.HasDecor)
                                DispatchStatistics.ClearDecorOrders(1);

                            DispDecorInfo();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            DispatchStatistics.DMarketingOrders(FirstDate, SecondDate, FactoryID, Convert.ToInt32(((DataRowView)DispatchStatistics.MDSummaryBS.Current)["ClientID"]));

                            if (!DispatchStatistics.HasDecor)
                                DispatchStatistics.ClearDecorOrders(1);

                            DispDecorInfo();
                        }
                    }
                }
        }

        private void DispatchFilterButton_Click(object sender, EventArgs e)
        {
            if (DispatchStatistics == null)
                return;
            FilterDispatch();
        }

        private void ConditionOrdersFilter_Click(object sender, EventArgs e)
        {
            bool OnAgreementOrders = rbtnOnAgreementOrders.Checked;
            bool AgreedOrders = rbtnAgreedOrders.Checked;
            bool OnProductionOrders = rbtnOnProduction.Checked;
            bool InProductionOrders = rbtnInProduction.Checked;

            decimal FrontCost = 0;
            decimal FrontSquare = 0;
            int FrontsCount = 0;
            int CurvedCount = 0;

            decimal DecorPogon = 0;
            decimal DecorCost = 0;
            int DecorCount = 0;

            DateTime Monday = GetMonday(CurrentWeekNumber);
            DateTime Wednesday = GetWednesday(CurrentWeekNumber);
            DateTime Friday = GetFriday(CurrentWeekNumber);

            FMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            FWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            FFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");
            DMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            DWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            DFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");

            int FactoryID = 0;

            if (CProfilCheckBox.Checked && !CTPSCheckBox.Checked)
                FactoryID = 1;
            if (!CProfilCheckBox.Checked && CTPSCheckBox.Checked)
                FactoryID = 2;
            if (!CProfilCheckBox.Checked && !CTPSCheckBox.Checked)
                FactoryID = -1;

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                ConditionOrdersStatistics.ClearOrders();
                if (Monday < DateTime.Now && MondayCB.Checked)
                {
                    if (OnAgreementOrders)
                        ConditionOrdersStatistics.GetOnAgreementOrders(Monday, FactoryID);
                    if (AgreedOrders)
                        ConditionOrdersStatistics.GetAgreedOrders(Monday, FactoryID);
                    if (OnProductionOrders)
                        ConditionOrdersStatistics.GetOnProductionOrders(Monday, FactoryID);
                    if (InProductionOrders)
                        ConditionOrdersStatistics.GetInProductionOrders(Monday, FactoryID);
                }

                MondayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT.Copy());
                MondayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT.Copy());
                MondayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT.Copy());
                ((DataView)MondayFrontsDG.DataSource).Sort = "Front, Square DESC";
                ((DataView)MondayDecorProductsDG.DataSource).Sort = "DecorProduct, Measure ASC, Count DESC";
                ((DataView)MondayDecorItemsDG.DataSource).Sort = "DecorItem, Count DESC";

                FrontCost = 0;
                FrontSquare = 0;
                FrontsCount = 0;
                CurvedCount = 0;
                MondayFrontsSquareLabel.Text = string.Empty;
                MondayFrontsCostLabel.Text = string.Empty;
                MondayFrontsCountLabel.Text = string.Empty;
                MondayCurvedCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
                MondayFrontsSquareLabel.Text = FrontSquare.ToString("N", nfi2);
                MondayFrontsCostLabel.Text = FrontCost.ToString("N", nfi2);
                MondayFrontsCountLabel.Text = FrontsCount.ToString();
                MondayCurvedCountLabel.Text = CurvedCount.ToString();

                DecorPogon = 0;
                DecorCost = 0;
                DecorCount = 0;
                MondayDecorPogonLabel.Text = string.Empty;
                MondayDecorCostLabel.Text = string.Empty;
                MondayDecorCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetDecorInfo(ref DecorPogon, ref DecorCost, ref DecorCount);
                MondayDecorPogonLabel.Text = DecorPogon.ToString("N", nfi2);
                MondayDecorCostLabel.Text = DecorCost.ToString("N", nfi2);
                MondayDecorCountLabel.Text = DecorCount.ToString();

                ConditionOrdersStatistics.ClearOrders();
                if (Wednesday < DateTime.Now && WednesdayCB.Checked)
                {
                    if (OnAgreementOrders)
                        ConditionOrdersStatistics.GetOnAgreementOrders(Wednesday, FactoryID);
                    if (AgreedOrders)
                        ConditionOrdersStatistics.GetAgreedOrders(Wednesday, FactoryID);
                    if (OnProductionOrders)
                        ConditionOrdersStatistics.GetOnProductionOrders(Wednesday, FactoryID);
                    if (InProductionOrders)
                        ConditionOrdersStatistics.GetInProductionOrders(Wednesday, FactoryID);
                }

                WednesdayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT.Copy());
                WednesdayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT.Copy());
                WednesdayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT.Copy());
                ((DataView)WednesdayFrontsDG.DataSource).Sort = "Front, Square DESC";
                ((DataView)WednesdayDecorProductsDG.DataSource).Sort = "DecorProduct, Measure ASC, Count DESC";
                ((DataView)WednesdayDecorItemsDG.DataSource).Sort = "DecorItem, Count DESC";

                FrontCost = 0;
                FrontSquare = 0;
                FrontsCount = 0;
                CurvedCount = 0;
                WednesdayFrontsSquareLabel.Text = string.Empty;
                WednesdayFrontsCostLabel.Text = string.Empty;
                WednesdayFrontsCountLabel.Text = string.Empty;
                WednesdayCurvedCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
                WednesdayFrontsSquareLabel.Text = FrontSquare.ToString("N", nfi2);
                WednesdayFrontsCostLabel.Text = FrontCost.ToString("N", nfi2);
                WednesdayFrontsCountLabel.Text = FrontsCount.ToString();
                WednesdayCurvedCountLabel.Text = CurvedCount.ToString();

                DecorPogon = 0;
                DecorCost = 0;
                DecorCount = 0;
                WednesdayDecorPogonLabel.Text = string.Empty;
                WednesdayDecorCostLabel.Text = string.Empty;
                WednesdayDecorCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetDecorInfo(ref DecorPogon, ref DecorCost, ref DecorCount);
                WednesdayDecorPogonLabel.Text = DecorPogon.ToString("N", nfi2);
                WednesdayDecorCostLabel.Text = DecorCost.ToString("N", nfi2);
                WednesdayDecorCountLabel.Text = DecorCount.ToString();

                ConditionOrdersStatistics.ClearOrders();
                if (Friday < DateTime.Now && FridayCB.Checked)
                {
                    if (OnAgreementOrders)
                        ConditionOrdersStatistics.GetOnAgreementOrders(Friday, FactoryID);
                    if (AgreedOrders)
                        ConditionOrdersStatistics.GetAgreedOrders(Friday, FactoryID);
                    if (OnProductionOrders)
                        ConditionOrdersStatistics.GetOnProductionOrders(Friday, FactoryID);
                    if (InProductionOrders)
                        ConditionOrdersStatistics.GetInProductionOrders(Friday, FactoryID);
                }

                FridayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT.Copy());
                FridayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT.Copy());
                FridayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT.Copy());
                ((DataView)FridayFrontsDG.DataSource).Sort = "Front, Square DESC";
                ((DataView)FridayDecorProductsDG.DataSource).Sort = "DecorProduct, Measure ASC, Count DESC";
                ((DataView)FridayDecorItemsDG.DataSource).Sort = "DecorItem, Count DESC";

                FrontCost = 0;
                FrontSquare = 0;
                FrontsCount = 0;
                CurvedCount = 0;
                FridayFrontsSquareLabel.Text = string.Empty;
                FridayFrontsCostLabel.Text = string.Empty;
                FridayFrontsCountLabel.Text = string.Empty;
                FridayCurvedCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
                FridayFrontsSquareLabel.Text = FrontSquare.ToString("N", nfi2);
                FridayFrontsCostLabel.Text = FrontCost.ToString("N", nfi2);
                FridayFrontsCountLabel.Text = FrontsCount.ToString();
                FridayCurvedCountLabel.Text = CurvedCount.ToString();

                DecorPogon = 0;
                DecorCost = 0;
                DecorCount = 0;
                FridayDecorPogonLabel.Text = string.Empty;
                FridayDecorCostLabel.Text = string.Empty;
                FridayDecorCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetDecorInfo(ref DecorPogon, ref DecorCost, ref DecorCount);
                FridayDecorPogonLabel.Text = DecorPogon.ToString("N", nfi2);
                FridayDecorCostLabel.Text = DecorCost.ToString("N", nfi2);
                FridayDecorCountLabel.Text = DecorCount.ToString();

                MondayDecorProductsDG_SelectionChanged(null, null);
                WednesdayDecorProductsDG_SelectionChanged(null, null);
                FridayDecorProductsDG_SelectionChanged(null, null);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                NeedSplash = true;
            }
            else
            {

            }
        }

        private DateTime GetFriday(int WeekNumber)
        {
            DateTime Friday;

            DateTime StartDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 1, 1);
            int OffsetToFirstDay = 0;
            //if (StartDate.DayOfWeek == DayOfWeek.Thursday)
            //    OffsetToFirstDay = 6;
            //else
            OffsetToFirstDay = Convert.ToInt32(StartDate.DayOfWeek) - 5;
            int OffsetToDemandedDay = 7 * (WeekNumber - 1) - OffsetToFirstDay;
            Friday = StartDate + new TimeSpan(OffsetToDemandedDay, 0, 0, 0);

            return Friday;
        }

        private DateTime GetMonday(int WeekNumber)
        {
            DateTime Monday;

            DateTime StartDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 1, 1);
            int OffsetToFirstDay = 0;
            //if (StartDate.DayOfWeek == DayOfWeek.Thursday)
            //    OffsetToFirstDay = 6;
            //else
            OffsetToFirstDay = Convert.ToInt32(StartDate.DayOfWeek) - 1;
            int OffsetToDemandedDay = 7 * (WeekNumber - 1) - OffsetToFirstDay;
            Monday = StartDate + new TimeSpan(OffsetToDemandedDay, 16, 0, 0);

            return Monday;
        }

        private DateTime GetWednesday(int WeekNumber)
        {
            DateTime Wednesday;

            DateTime StartDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 1, 1);
            int OffsetToFirstDay = 0;
            //if (StartDate.DayOfWeek == DayOfWeek.Thursday)
            //    OffsetToFirstDay = 6;
            //else
            OffsetToFirstDay = Convert.ToInt32(StartDate.DayOfWeek) - 3;
            int OffsetToDemandedDay = 7 * (WeekNumber - 1) - OffsetToFirstDay;
            Wednesday = StartDate + new TimeSpan(OffsetToDemandedDay, 0, 0, 0);

            return Wednesday;
        }

        private void HelpCheckButton_Click(object sender, EventArgs e)
        {
            if (!HelpCheckButton.Checked)
            {
                HelpPanel.Visible = false;

                if (MenuButton.Checked)
                {
                    if (tabControl6.SelectedIndex == 0)
                    {
                        CommonMenuPanel.Visible = true;
                        BatchMenuPanel.Visible = false;
                        StoreMenuPanel.Visible = false;
                        DispMenuPanel.Visible = false;
                        ConditionMenuPanel.Visible = false;
                        ExpMenuPanel.Visible = false;
                    }

                    if (tabControl6.SelectedIndex == 1)
                    {
                        CommonMenuPanel.Visible = false;
                        BatchMenuPanel.Visible = true;
                        StoreMenuPanel.Visible = false;
                        DispMenuPanel.Visible = false;
                        ConditionMenuPanel.Visible = false;
                        ExpMenuPanel.Visible = false;
                    }

                    if (tabControl6.SelectedIndex == 6)
                    {
                        CommonMenuPanel.Visible = false;
                        BatchMenuPanel.Visible = false;
                        StoreMenuPanel.Visible = false;
                        DispMenuPanel.Visible = false;
                        ConditionMenuPanel.Visible = false;
                        ExpMenuPanel.Visible = false;
                    }
                    if (tabControl6.SelectedIndex == 2)
                    {
                        CommonMenuPanel.Visible = false;
                        BatchMenuPanel.Visible = false;
                        StoreMenuPanel.Visible = true;
                        DispMenuPanel.Visible = false;
                        ConditionMenuPanel.Visible = false;
                        ExpMenuPanel.Visible = false;
                    }
                    if (tabControl6.SelectedIndex == 4)
                    {
                        CommonMenuPanel.Visible = false;
                        BatchMenuPanel.Visible = false;
                        StoreMenuPanel.Visible = false;
                        DispMenuPanel.Visible = true;
                        ConditionMenuPanel.Visible = false;
                        ExpMenuPanel.Visible = false;
                    }
                    if (tabControl6.SelectedIndex == 5)
                    {
                        CommonMenuPanel.Visible = false;
                        BatchMenuPanel.Visible = false;
                        StoreMenuPanel.Visible = false;
                        DispMenuPanel.Visible = false;
                        ConditionMenuPanel.Visible = true;
                        ExpMenuPanel.Visible = false;
                    }
                    if (tabControl6.SelectedIndex == 3)
                    {
                        CommonMenuPanel.Visible = false;
                        BatchMenuPanel.Visible = false;
                        StoreMenuPanel.Visible = false;
                        DispMenuPanel.Visible = false;
                        ConditionMenuPanel.Visible = false;
                        ExpMenuPanel.Visible = true;
                    }
                }
            }
            else
            {
                BatchMenuPanel.Visible = false;
                CommonMenuPanel.Visible = false;
                StoreMenuPanel.Visible = false;
                DispMenuPanel.Visible = false;
                ConditionMenuPanel.Visible = false;
                ExpMenuPanel.Visible = false;

                HelpPanel.Visible = true;
            }
        }

        private void MondayDecorProductsDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ConditionOrdersStatistics != null)
                if (MondayDecorProductsDG.SelectedRows.Count > 0)
                {
                    int ProductID = Convert.ToInt32(MondayDecorProductsDG.SelectedRows[0].Cells["ProductID"].Value);
                    int MeasureID = Convert.ToInt32(MondayDecorProductsDG.SelectedRows[0].Cells["MeasureID"].Value);

                    ((DataView)MondayDecorItemsDG.DataSource).RowFilter = "ProductID = " + ProductID + " AND MeasureID = " + MeasureID;
                }
        }

        private void WednesdayDecorProductsDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ConditionOrdersStatistics != null)
                if (WednesdayDecorProductsDG.SelectedRows.Count > 0)
                {
                    int ProductID = Convert.ToInt32(WednesdayDecorProductsDG.SelectedRows[0].Cells["ProductID"].Value);
                    int MeasureID = Convert.ToInt32(WednesdayDecorProductsDG.SelectedRows[0].Cells["MeasureID"].Value);

                    ((DataView)WednesdayDecorItemsDG.DataSource).RowFilter = "ProductID = " + ProductID + " AND MeasureID = " + MeasureID;
                }
        }

        private void FridayDecorProductsDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ConditionOrdersStatistics != null)
                if (FridayDecorProductsDG.SelectedRows.Count > 0)
                {
                    int ProductID = Convert.ToInt32(FridayDecorProductsDG.SelectedRows[0].Cells["ProductID"].Value);
                    int MeasureID = Convert.ToInt32(FridayDecorProductsDG.SelectedRows[0].Cells["MeasureID"].Value);

                    ((DataView)FridayDecorItemsDG.DataSource).RowFilter = "ProductID = " + ProductID + " AND MeasureID = " + MeasureID;
                }
        }

        private void StoreFilterButton_Click(object sender, EventArgs e)
        {
            if (StorageStatistics == null)
                return;
            FilterStorage();
        }

        private void ExpFilterButton_Click(object sender, EventArgs e)
        {
            if (ExpeditionStatistics == null)
                return;
            FilterExpedition();
        }

        private void FrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            if (rbMarketing.Checked)
            {
                MarketingBatchReport.CreateReport(AllProductsStatistics.FrontsOrdersDataTable,
                    ((DataTable)((BindingSource)DecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Общая статистика.Общие сведения");
            }
            if (rbZOV.Checked)
            {
                MarketingBatchReport.CreateReport(ZOVOrdersStatistics.FrontsOrdersDataTable,
                    ((DataTable)((BindingSource)DecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Общая статистика.Общие сведения");
            }
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void DecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void StoreFrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void StoreDecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void ExpFrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void ExpDecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void DispFrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void DispDecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataTable)((BindingSource)StoreFrontsDataGrid.DataSource).DataSource).DefaultView.Table,
                ((DataTable)((BindingSource)StoreDecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Общая статистика.Склад");

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataTable)((BindingSource)DispFrontsDataGrid.DataSource).DataSource).DefaultView.Table,
                ((DataTable)((BindingSource)DispDecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Общая статистика.Отгрузка");

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void tabControl6_Selecting(object sender, TabControlCancelEventArgs e)
        {
            NeedSelectionChange = false;
        }

        private void tabControl6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MenuButton.Checked)
            {
                if (HelpPanel.Visible)
                    HelpPanel.Visible = false;

                if (tabControl6.SelectedIndex == 0)
                {
                    CommonMenuPanel.Visible = true;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }

                if (tabControl6.SelectedIndex == 1)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = true;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }

                if (tabControl6.SelectedIndex == 6)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 2)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = true;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 4)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = true;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 5)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = true;
                    ExpMenuPanel.Visible = false;
                }
                if (tabControl6.SelectedIndex == 3)
                {
                    CommonMenuPanel.Visible = false;
                    BatchMenuPanel.Visible = false;
                    StoreMenuPanel.Visible = false;
                    DispMenuPanel.Visible = false;
                    ConditionMenuPanel.Visible = false;
                    ExpMenuPanel.Visible = true;
                }
            }
            NeedSelectionChange = true;
        }

        private void WeeksOfYearListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentWeekNumber = WeeksOfYearListBox.SelectedIndex + 1;

            DateTime Monday = GetMonday(CurrentWeekNumber);
            DateTime Wednesday = GetWednesday(CurrentWeekNumber);
            DateTime Friday = GetFriday(CurrentWeekNumber);

            FMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            FWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            FFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");
            DMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            DWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            DFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");

        }

        private void cbxYears_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DateTime LastDay = new System.DateTime(Convert.ToInt32(cbxYears.SelectedValue), 12, 31);

            int WeeksCount = GetWeekNumber(LastDay);
            WeeksOfYearListBox.Items.Clear();
            for (int i = 1; i <= WeeksCount; i++)
            {
                WeeksOfYearListBox.Items.Add(i);
            }
            WeeksOfYearListBox.SelectedIndex = CurrentWeekNumber - 1;

        }

        private void cbxYears_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cbxYears_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            DateTime Monday = GetMonday(CurrentWeekNumber);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataView)MondayFrontsDG.DataSource).Table,
                ((DataView)MondayDecorProductsDG.DataSource).Table, "Состояние заказов. " + Monday.ToString("dd MMMM yyyy"));

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MondayFrontsDG_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu7.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            DateTime Wednesday = GetWednesday(CurrentWeekNumber);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataView)WednesdayFrontsDG.DataSource).Table,
                ((DataView)WednesdayDecorProductsDG.DataSource).Table, "Состояние заказов. " + Wednesday.ToString("dd MMMM yyyy"));

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {
            DateTime Friday = GetFriday(CurrentWeekNumber);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataView)FridayFrontsDG.DataSource).Table,
                ((DataView)FridayDecorProductsDG.DataSource).Table, "Состояние заказов. " + Friday.ToString("dd MMMM yyyy"));

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void WednesdayFrontsDG_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu8.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void FridayFrontsDG_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;

            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void StoreClientSummaryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            StoreOrdersSummaryCheckBox.Enabled = !StoreClientSummaryCheckBox.Checked;
            StoreOrdersSummaryCheckBox.Checked = !StoreClientSummaryCheckBox.Checked;
        }

        private void ExpClientSummaryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ExpOrdersSummaryCheckBox.Enabled = !ExpClientSummaryCheckBox.Checked;
            ExpOrdersSummaryCheckBox.Checked = !ExpClientSummaryCheckBox.Checked;
        }

        private void ExpFrontsInfo()
        {
            ExpFrontsSquareLabel.Text = string.Empty;
            ExpFrontsCostLabel.Text = string.Empty;
            ExpFrontsCountLabel.Text = string.Empty;
            ExpCurvedCountLabel.Text = string.Empty;

            decimal ExpFrontCost = 0;
            decimal ExpFrontSquare = 0;
            int ExpFrontsCount = 0;
            int ExpCurvedCount = 0;

            ExpeditionStatistics.GetFrontsInfo(ref ExpFrontSquare, ref ExpFrontCost, ref ExpFrontsCount, ref ExpCurvedCount);

            ExpFrontsSquareLabel.Text = ExpFrontSquare.ToString("N", nfi2);
            ExpFrontsCostLabel.Text = ExpFrontCost.ToString("N", nfi2);
            ExpFrontsCountLabel.Text = ExpFrontsCount.ToString();
            ExpCurvedCountLabel.Text = ExpCurvedCount.ToString();
        }

        private void ExpDecorInfo()
        {
            ExpDecorPogonLabel.Text = string.Empty;
            ExpDecorCostLabel.Text = string.Empty;
            ExpDecorCountLabel.Text = string.Empty;

            decimal ExpDecorPogon = 0;
            decimal ExpDecorCost = 0;
            int ExpDecorCount = 0;

            ExpeditionStatistics.GetDecorInfo(ref ExpDecorPogon, ref ExpDecorCost, ref ExpDecorCount);

            ExpDecorPogonLabel.Text = ExpDecorPogon.ToString("N", nfi2);
            ExpDecorCostLabel.Text = ExpDecorCost.ToString("N", nfi2);
            ExpDecorCountLabel.Text = ExpDecorCount.ToString();

            ExpDecorProductsDataGrid_SelectionChanged(null, null);
        }

        private void FilterExpedition()
        {
            bool Profil = ExpProfilCheckBox.Checked;
            bool TPS = ExpTPSCheckBox.Checked;

            int FactoryID = 0;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                ExpeditionStatistics.FillMarketingTables();
                ExpeditionStatistics.ShowColumns(ref ExpMFSummaryDG, ref ExpMDSummaryDG, Profil, TPS, bExpSummaryClient);

                ExpeditionStatistics.FMarketingOrders(FactoryID, -1);
                ExpeditionStatistics.DMarketingOrders(FactoryID, -1);

                if (!ExpClientSummaryCheckBox.Checked)
                    ExpeditionStatistics.MarketingSummary(FactoryID);
                else
                    ExpeditionStatistics.ClientSummary(FactoryID);

                ExpMFSummaryDG.BringToFront();
                ExpMDSummaryDG.BringToFront();
                ExpFrontsMarketLabel.Visible = true;
                ExpFrontsPrepareLabel.Visible = false;
                ExpFrontsZOVLabel.Visible = false;
                ExpDecorMarketLabel.Visible = true;
                ExpDecorPrepareLabel.Visible = false;
                ExpDecorZOVLabel.Visible = false;
                if (!ExpeditionStatistics.HasFronts)
                    ExpeditionStatistics.ClearFrontsOrders(1);
                if (!ExpeditionStatistics.HasDecor)
                    ExpeditionStatistics.ClearDecorOrders(1);

                ExpFrontsInfo();
                ExpDecorInfo();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void kryptonContextMenuItem7_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataTable)((BindingSource)ExpFrontsDataGrid.DataSource).DataSource).DefaultView.Table,
                ((DataTable)((BindingSource)ExpDecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Общая статистика.Экспедиция");

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnBatchFilter_Click(object sender, EventArgs e)
        {
            if (BatchStatistics == null)
                return;
            BatchFilter();
            MenuButton.Checked = false;
            BatchMenuPanel.Visible = !BatchMenuPanel.Visible;
        }

        private void dgvGeneralSummary_SelectionChanged(object sender, EventArgs e)
        {
            lbAllSFSquare.Text = string.Empty;
            lbAllSFCount.Text = string.Empty;
            lbAllCFCount.Text = string.Empty;

            lbOnProdSFSquare.Text = string.Empty;
            lbOnProdSFCount.Text = string.Empty;
            lbOnCFCount.Text = string.Empty;

            lbInProdSFSquare.Text = string.Empty;
            lbInProdSFCount.Text = string.Empty;
            lbInCFCount.Text = string.Empty;

            lbReadySFSquare.Text = string.Empty;
            lbReadySFCount.Text = string.Empty;
            lbReadyCFCount.Text = string.Empty;

            lbAllDecorPogon.Text = string.Empty;
            lbAllDecorCount.Text = string.Empty;

            lbOnProdDecorPogon.Text = string.Empty;
            lbOnProdDecorCount.Text = string.Empty;

            lbInProdDecorPogon.Text = string.Empty;
            lbInProdDecorCount.Text = string.Empty;

            lbReadyDecorPogon.Text = string.Empty;
            lbReadyDecorCount.Text = string.Empty;

            if (BatchStatistics == null)
                return;
            if (dgvGeneralSummary.Rows.Count == 0)
            {
                BatchStatistics.ClearProductTables();
                return;
            }

            bool Profil = BatchProfilCheckBox.Checked;
            bool TPS = BatchTPSCheckBox.Checked;
            int FactoryID = 0;

            decimal AllSquare = 0;
            decimal OnProdSquare = 0;
            decimal InProdSquare = 0;
            decimal ReadySquare = 0;

            int AllCount = 0;
            int OnProdCount = 0;
            int InProdCount = 0;
            int ReadyCount = 0;

            int AllCurvedCount = 0;
            int OnProdCurvedCount = 0;
            int InProdCurvedCount = 0;
            int ReadyCurvedCount = 0;

            decimal AllDecorPogon = 0;
            decimal OnProdDecorPogon = 0;
            decimal InProdDecorPogon = 0;
            decimal ReadyDecorPogon = 0;

            int AllDecorCount = 0;
            int OnProdDecorCount = 0;
            int InProdDecorCount = 0;
            int ReadyDecorCount = 0;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;
            int GroupType = -1;
            int MegaBatchID = -1;
            if (dgvGeneralSummary.SelectedRows.Count != 0 && dgvGeneralSummary.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvGeneralSummary.SelectedRows[0].Cells["GroupType"].Value);
            if (dgvGeneralSummary.SelectedRows.Count != 0 && dgvGeneralSummary.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvGeneralSummary.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                if (cbtnSimpleFronts.Checked)
                {
                    BatchStatistics.FilterSimpleFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetSimpleFrontsInfo(ref AllSquare, ref OnProdSquare, ref InProdSquare, ref ReadySquare,
                        ref AllCount, ref OnProdCount, ref InProdCount, ref ReadyCount);
                }
                if (cbtnCurvedFronts.Checked)
                {
                    BatchStatistics.FilterCurvedFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetCurvedFrontsInfo(ref AllCurvedCount, ref OnProdCurvedCount, ref InProdCurvedCount, ref ReadyCurvedCount);
                }
                if (cbtnDecor.Checked)
                {
                    BatchStatistics.FilterDecorOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetDecorInfo(ref AllDecorPogon, ref OnProdDecorPogon, ref InProdDecorPogon, ref ReadyDecorPogon,
                        ref AllDecorCount, ref OnProdDecorCount, ref InProdDecorCount, ref ReadyDecorCount);
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                if (cbtnSimpleFronts.Checked)
                {
                    BatchStatistics.FilterSimpleFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetSimpleFrontsInfo(ref AllSquare, ref OnProdSquare, ref InProdSquare, ref ReadySquare,
                        ref AllCount, ref OnProdCount, ref InProdCount, ref ReadyCount);
                }
                if (cbtnCurvedFronts.Checked)
                {
                    BatchStatistics.FilterCurvedFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetCurvedFrontsInfo(ref AllCurvedCount, ref OnProdCurvedCount, ref InProdCurvedCount, ref ReadyCurvedCount);
                }
                if (cbtnDecor.Checked)
                {
                    BatchStatistics.FilterDecorOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetDecorInfo(ref AllDecorPogon, ref OnProdDecorPogon, ref InProdDecorPogon, ref ReadyDecorPogon,
                        ref AllDecorCount, ref OnProdDecorCount, ref InProdDecorCount, ref ReadyDecorCount);
                }
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ","
            };
            lbAllSFSquare.Text = AllSquare.ToString("N", nfi1);
            lbAllSFCount.Text = AllCount.ToString();
            lbAllCFCount.Text = AllCurvedCount.ToString();

            lbOnProdSFSquare.Text = OnProdSquare.ToString("N", nfi1);
            lbOnProdSFCount.Text = OnProdCount.ToString();
            lbOnCFCount.Text = OnProdCurvedCount.ToString();

            lbInProdSFSquare.Text = InProdSquare.ToString("N", nfi1);
            lbInProdSFCount.Text = InProdCount.ToString();
            lbInCFCount.Text = InProdCurvedCount.ToString();

            lbReadySFSquare.Text = ReadySquare.ToString("N", nfi1);
            lbReadySFCount.Text = ReadyCount.ToString();
            lbReadyCFCount.Text = ReadyCurvedCount.ToString();

            lbAllDecorPogon.Text = AllDecorPogon.ToString("N", nfi1);
            lbAllDecorCount.Text = AllDecorCount.ToString();

            lbOnProdDecorPogon.Text = OnProdDecorPogon.ToString("N", nfi1);
            lbOnProdDecorCount.Text = OnProdDecorCount.ToString();

            lbInProdDecorPogon.Text = InProdDecorPogon.ToString("N", nfi1);
            lbInProdDecorCount.Text = InProdDecorCount.ToString();

            lbReadyDecorPogon.Text = ReadyDecorPogon.ToString("N", nfi1);
            lbReadyDecorCount.Text = ReadyDecorCount.ToString();
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (BatchStatistics == null)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            bool Profil = BatchProfilCheckBox.Checked;
            bool TPS = BatchTPSCheckBox.Checked;
            bool DoNotShowReady = DoNotShowReadyCheckBox.Checked;
            if (cbtnSimpleFronts.Checked)
            {
                dgvGeneralSummary.Columns["FilenkaPerc"].Visible = true;
                dgvGeneralSummary.Columns["TrimmingPerc"].Visible = true;
                dgvGeneralSummary.Columns["AssemblyPerc"].Visible = true;
                dgvGeneralSummary.Columns["DeyingPerc"].Visible = true;
                BatchStatistics.SimpleFrontsGeneralSummary();
                BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, true, false, false);
                pnlSimpleFronts.BringToFront();
            }
            if (cbtnCurvedFronts.Checked)
            {
                dgvGeneralSummary.Columns["FilenkaPerc"].Visible = false;
                dgvGeneralSummary.Columns["TrimmingPerc"].Visible = false;
                dgvGeneralSummary.Columns["AssemblyPerc"].Visible = false;
                dgvGeneralSummary.Columns["DeyingPerc"].Visible = false;
                BatchStatistics.CurvedFrontsGeneralSummary();
                BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, false, true, false);
                pnlCurvedFronts.BringToFront();
            }
            if (cbtnDecor.Checked)
            {
                dgvGeneralSummary.Columns["FilenkaPerc"].Visible = false;
                dgvGeneralSummary.Columns["TrimmingPerc"].Visible = false;
                dgvGeneralSummary.Columns["AssemblyPerc"].Visible = false;
                dgvGeneralSummary.Columns["DeyingPerc"].Visible = false;
                BatchStatistics.DecorGeneralSummary();
                BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, false, false, true);
                pnlDecor.BringToFront();
            }
            dgvGeneralSummary_SelectionChanged(null, null);
            cbtnGeneralSummary.Checked = true;
            pnlGeneralSummary.BringToFront();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void BatchFilter()
        {
            bool Profil = BatchProfilCheckBox.Checked;
            bool TPS = BatchTPSCheckBox.Checked;
            bool DoNotShowReady = DoNotShowReadyCheckBox.Checked;
            DateTime FirstDate = mcBatchFirstDate.SelectionStart;
            DateTime SecondDate = mcBatchSecondDate.SelectionStart;
            if (NeedSplash)
            {
                dgvGeneralSummary.SelectionChanged -= dgvGeneralSummary_SelectionChanged;
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                BatchStatistics.UpdateMegaBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, FirstDate, SecondDate);
                BatchStatistics.UpdateFrontsOrders(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, FirstDate, SecondDate);
                BatchStatistics.UpdateDecorOrders(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, FirstDate, SecondDate);

                ShowBatchColumns(Profil, TPS);
                if (cbtnSimpleFronts.Checked)
                {
                    BatchStatistics.SimpleFrontsGeneralSummary();
                    BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, true, false, false);
                }
                if (cbtnCurvedFronts.Checked)
                {
                    BatchStatistics.CurvedFrontsGeneralSummary();
                    BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, false, true, false);
                }
                if (cbtnDecor.Checked)
                {
                    BatchStatistics.DecorGeneralSummary();
                    BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, false, false, true);
                }
                dgvGeneralSummary.SelectionChanged += dgvGeneralSummary_SelectionChanged;
                dgvGeneralSummary_SelectionChanged(null, null);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                NeedSplash = true;
            }
            else
            {
                dgvGeneralSummary.SelectionChanged -= dgvGeneralSummary_SelectionChanged;
                BatchStatistics.UpdateMegaBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, FirstDate, SecondDate);
                BatchStatistics.UpdateFrontsOrders(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, FirstDate, SecondDate);
                BatchStatistics.UpdateDecorOrders(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, FirstDate, SecondDate);

                ShowBatchColumns(Profil, TPS);
                if (cbtnSimpleFronts.Checked)
                {
                    BatchStatistics.SimpleFrontsGeneralSummary();
                    BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, true, false, false);
                }
                if (cbtnCurvedFronts.Checked)
                {
                    BatchStatistics.CurvedFrontsGeneralSummary();
                    BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, false, true, false);
                }
                if (cbtnDecor.Checked)
                {
                    BatchStatistics.DecorGeneralSummary();
                    BatchStatistics.FilterBatch(BatchMarketingCheckBox.Checked, BatchZOVCheckBox.Checked, Profil, TPS, DoNotShowReady, false, false, true);
                }
                dgvGeneralSummary.SelectionChanged += dgvGeneralSummary_SelectionChanged;
                dgvGeneralSummary_SelectionChanged(null, null);
            }
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (BatchStatistics == null)
                return;
            if (cbtnGeneralSummary.Checked)
            {
                pnlGeneralSummary.BringToFront();
            }
            if (cbtnProductSummary.Checked)
            {
                if (cbtnSimpleFronts.Checked)
                {
                    pnlSimpleFronts.BringToFront();
                }
                if (cbtnCurvedFronts.Checked)
                {
                    pnlCurvedFronts.BringToFront();
                }
                if (cbtnDecor.Checked)
                {
                    pnlDecor.BringToFront();
                }
            }
        }

        private void dgvSimpleFronts_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["Ready"].Value))
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(209, 232, 204);
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
            else
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void StoreCurvedFrontsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreOrdersSummaryCheckBox.Checked)
                return;

            if (StorageStatistics != null)
                if (StorageStatistics.MCurvedFSummaryBS.Count > 0)
                {
                    if (((DataRowView)StorageStatistics.MCurvedFSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = StoreProfilCheckBox.Checked;
                        bool TPS = StoreTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            if (StoreClientSummaryCheckBox.Checked)
                                StorageStatistics.CurvedFMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MCurvedFSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.CurvedFMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MCurvedFSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasCurvedFronts)
                                StorageStatistics.ClearCurvedFrontsOrders(1);

                            StoreCurvedFrontsInfo();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (StoreClientSummaryCheckBox.Checked)
                                StorageStatistics.CurvedFMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MCurvedFSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.CurvedFMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MCurvedFSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasCurvedFronts)
                                StorageStatistics.ClearCurvedFrontsOrders(1);

                            StoreCurvedFrontsInfo();
                        }
                    }
                }
        }

        private void kryptonContextMenuItem8_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            decimal EURBYRCurrency = 1000000;
            DateTime date = new DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, 1);
            string FileName = "Вышло с пр-ва " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy");
            if (CalendarFrom.SelectionStart != CalendarTo.SelectionStart)
                FileName = "Вышло с пр-ва за период с " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy") + " по " + CalendarTo.SelectionStart.ToString("dd.MM.yyyy");

            ArrayList MClients = AllProductsStatistics.SelectedMarketingClients;
            ArrayList MClientGroups = AllProductsStatistics.SelectedMarketingClientGroups;
            DecorCatalogOrder = new Modules.ZOV.DecorCatalogOrder();
            ProducedProducts ProducedProducts = new ProducedProducts(ref DecorCatalogOrder);
            ProducedProducts.GetDateRates(date, ref EURBYRCurrency);
            //ProducedProducts.CreateZOVReport(CalendarFrom.SelectionStart, CalendarTo.SelectionStart, FileName, EURBYRCurrency);
            ProducedProducts.CreateMarketingReport(CalendarFrom.SelectionStart, CalendarTo.SelectionStart, cbSamples.Checked, cbNotSamples.Checked, FileName, EURBYRCurrency, MClients, MClientGroups);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem9_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            decimal EURBYRCurrency = 1000000;
            DateTime date = new DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, 1);
            string FileName = "Отгружено " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy");
            if (CalendarFrom.SelectionStart != CalendarTo.SelectionStart)
                FileName = "Отгружено за период с " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy") + " по " + CalendarTo.SelectionStart.ToString("dd.MM.yyyy");

            ArrayList MClients = AllProductsStatistics.SelectedMarketingClients;
            ArrayList MClientGroups = AllProductsStatistics.SelectedMarketingClientGroups;
            DecorCatalogOrder = new Modules.ZOV.DecorCatalogOrder();
            ProducedProducts ProducedProducts = new ProducedProducts(ref DecorCatalogOrder);
            ProducedProducts.GetDateRates(date, ref EURBYRCurrency);
            //ProducedProducts.CreateZOVDispReport(CalendarFrom.SelectionStart, CalendarTo.SelectionStart, FileName, EURBYRCurrency);
            ProducedProducts.CreateMarketingDispReport(CalendarFrom.SelectionStart, CalendarTo.SelectionStart, cbSamples.Checked, cbNotSamples.Checked,
                FileName, EURBYRCurrency, MClients, MClientGroups);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ExportContextMenuButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            ExportContextMenu.Show(ExportContextMenuButton);
        }

        private void kryptonContextMenuItem10_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            DateTime date = new DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, 1);
            string FileName = "Отчет по движению готовой продукции " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy");
            if (CalendarFrom.SelectionStart != CalendarTo.SelectionStart)
                FileName = "Отчет по движению готовой продукции за период с " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy") + " по " + CalendarTo.SelectionStart.ToString("dd.MM.yyyy");

            ArrayList MClients = AllProductsStatistics.SelectedMarketingClients;
            ArrayList MClientGroups = AllProductsStatistics.SelectedMarketingClientGroups;
            DecorCatalogOrder = new Modules.ZOV.DecorCatalogOrder();
            ProducedProducts ProducedProducts = new ProducedProducts(ref DecorCatalogOrder);
            ProducedProducts.StartEndProducedReport(CalendarFrom.SelectionStart, CalendarTo.SelectionStart, cbSamples.Checked, cbNotSamples.Checked, FileName, MClients, MClientGroups);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem13_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            DateTime date = new DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, 1);
            string FileName = "Отчет по весу " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy");
            if (CalendarFrom.SelectionStart != CalendarTo.SelectionStart)
                FileName = "Отчет по весу за период с " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy") + " по " + CalendarTo.SelectionStart.ToString("dd.MM.yyyy");

            WeightDispatchReport WeightDispatchReport = new WeightDispatchReport();
            WeightDispatchReport.WeightReport(CalendarFrom.SelectionStart, CalendarTo.SelectionStart, FileName);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            string FileName = "Остатки (декор) " + DateTime.Now.ToString("dd.MM.yyyy");

            DecorBalancesReport BalancesReport = new DecorBalancesReport(FileName);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem15_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            string FileName = "Состояние склада на " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy");

            ClientStoreReport report = new ClientStoreReport();
            ArrayList MClients = AllProductsStatistics.SelectedMarketingClients;
            ArrayList MClientGroups = AllProductsStatistics.SelectedMarketingClientGroups;
            report.FillTables(CalendarFrom.SelectionStart, MClients, MClientGroups);
            if (report.HasData)
            {
                report.Report(FileName);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказов на складе нет",
                       "Склад клиента");
            }
        }

        private void ZOVClientGroupsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (ZOVClientGroupsDataGrid.Columns[e.ColumnIndex].Name == "Check" && e.RowIndex != -1)
            {
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)ZOVClientGroupsDataGrid.
                    Rows[e.RowIndex].Cells["Check"];

                bool Checked = Convert.ToBoolean(checkCell.Value);

                if (NeedSplash)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();
                    while (!SplashWindow.bSmallCreated) ;
                    NeedSplash = false;

                    ZOVOrdersStatistics.SetCheckClients(Checked);
                    ZOVClientGroupsDataGrid.Invalidate();

                    NeedSplash = true;
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
                else
                {
                    ZOVOrdersStatistics.SetCheckClients(Checked);
                    ZOVClientGroupsDataGrid.Invalidate();
                }
            }
        }

        private void ZOVClientGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ZOVOrdersStatistics == null)
                return;

            ZOVOrdersStatistics.GetCurrentZOVGroup();
        }

        private void ZOVClientGroupsDataGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(ZOVClientGroupsDataGrid.Rows[e.RowIndex].Cells["Color"].Value.ToString()))
            {
                Color col = ColorTranslator.FromHtml(ZOVClientGroupsDataGrid.Rows[e.RowIndex].Cells["Color"].Value.ToString());
                ZOVClientGroupsDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = col;
            }
        }

        private void ClientsDataGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

        }

        private void ZOVClientsDataGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(ZOVClientsDataGrid.Rows[e.RowIndex].Cells["Color"].Value.ToString()))
            {
                Color col = ColorTranslator.FromHtml(ZOVClientsDataGrid.Rows[e.RowIndex].Cells["Color"].Value.ToString());
                ZOVClientsDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = col;
            }
        }

        private void ZOVClientsDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            string storeName = string.Empty;
            if (grid.Rows[e.RowIndex].Cells["ClientID"].Value != DBNull.Value)
            {
                //ClientID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["ClientID"].Value);
                storeName = grid.Rows[e.RowIndex].Cells["ClientGroupName"].Value.ToString();
                cell.ToolTipText = storeName;
            }
        }

        private void rbZOV_CheckedChanged(object sender, EventArgs e)
        {
            if (rbZOV.Checked)
            {
                ZOVOrdersStatisticsRebind();
                panel33.BringToFront();
            }

            if (rbMarketing.Checked)
            {
                CommonStatisticsRebind();
                panel32.BringToFront();
            }
        }

        private void kryptonCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            ZOVOrdersStatistics.checkbox2_CheckedChanged(kryptonCheckBox1.Checked);
        }

        private void kryptonCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            ZOVOrdersStatistics.checkbox1_CheckedChanged(kryptonCheckBox2.Checked);
        }

        private void kryptonCheckBox3_CheckedChanged(object sender, EventArgs e)
        {

            AllProductsStatistics.checkbox3_CheckedChanged(kryptonCheckBox3.Checked);
        }

        private void kryptonContextMenuItem16_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            string FileName = "Не упакованная продукция с " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy") + " по "
                + CalendarTo.SelectionStart.ToString("dd.MM.yyyy");

            ClientNotPackedReport report = new ClientNotPackedReport();
            ArrayList MClients = AllProductsStatistics.SelectedMarketingClients;
            ArrayList MClientGroups = AllProductsStatistics.SelectedMarketingClientGroups;
            report.FillTables(CalendarFrom.SelectionStart, CalendarTo.SelectionStart, MClients, MClientGroups);
            if (report.HasData)
            {
                report.Report(FileName);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Не упакованных заказов нет",
                       "Не упаковано под клиента");
            }
        }

        private void kryptonContextMenuItem17_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            string FileName = "Упаковано, но не принято на склад. Состояние на " + CalendarFrom.SelectionStart.ToString("dd.MM.yyyy");

            ClientNotStoredReport report = new ClientNotStoredReport();
            ArrayList MClients = AllProductsStatistics.SelectedMarketingClients;
            ArrayList MClientGroups = AllProductsStatistics.SelectedMarketingClientGroups;
            report.FillTables(CalendarFrom.SelectionStart, MClients, MClientGroups);
            if (report.HasData)
            {
                report.Report(FileName);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказов нет",
                       "Упаковано под клиента");
            }
        }

        private void kryptonContextMenuItem18_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool PlanDispDate = PlanDispDateRadioButton.Checked;
            bool OrderDate = OrderDateRadioButton.Checked;
            bool ConfirmDate = ConfirmDateRadioButton.Checked;
            bool OnAgreement = OnAgreementRadioButton.Checked;
            bool PackDate = PackDateRadioButton.Checked;
            bool StoreDate = StoreDateRadioButton.Checked;
            bool ExpDate = ExpDateRadioButton.Checked;
            bool FactDispDate = FactDispDateRadioButton.Checked;
            bool OnProduction = OnProductionRadioButton.Checked;

            DateTime DateFrom = CalendarFrom.SelectionEnd;
            DateTime DateTo = CalendarTo.SelectionEnd;

            int PackageStatusID = -1;

            if (PackDate)
                PackageStatusID = 1;
            if (StoreDate)
                PackageStatusID = 2;
            if (FactDispDate)
                PackageStatusID = 3;
            if (ExpDate)
                PackageStatusID = 4;

            //int FactoryID = 0;

            //if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
            //    FactoryID = 1;
            //if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
            //    FactoryID = 2;
            //if (!ProfilCheckBox.Checked && !TPSCheckBox.Checked)
            //    FactoryID = -1;

            string FileName = "Транспорт за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + "";

            AdditionalCostReport report = new AdditionalCostReport();

            if (PlanDispDate)
                report.FilterByPlanDispatch(DateFrom, DateTo);

            if (OrderDate)
                report.FilterByOrderDate(DateFrom, DateTo);

            if (ConfirmDate)
                report.FilterByConfirmDate(DateFrom, DateTo);

            if (OnAgreement)
                report.FilterByOnAgreement(DateFrom, DateTo);

            if (OnProduction)
                report.FilterByOnProduction(DateFrom, DateTo);

            if (PackDate || StoreDate || ExpDate || FactDispDate)
                report.FilterByPackages(DateFrom, DateTo, PackageStatusID);

            if (report.HasData)
            {
                report.Report(FileName);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Рекламаций за выбранный период нет",
                       "Рекламации");
            }
        }

        private void kryptonContextMenuItem19_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool PlanDispDate = PlanDispDateRadioButton.Checked;
            bool OrderDate = OrderDateRadioButton.Checked;
            bool ConfirmDate = ConfirmDateRadioButton.Checked;
            bool OnAgreement = OnAgreementRadioButton.Checked;
            bool PackDate = PackDateRadioButton.Checked;
            bool StoreDate = StoreDateRadioButton.Checked;
            bool ExpDate = ExpDateRadioButton.Checked;
            bool FactDispDate = FactDispDateRadioButton.Checked;
            bool OnProduction = OnProductionRadioButton.Checked;

            DateTime DateFrom = CalendarFrom.SelectionEnd;
            DateTime DateTo = CalendarTo.SelectionEnd;

            int PackageStatusID = -1;

            if (PackDate)
                PackageStatusID = 1;
            if (StoreDate)
                PackageStatusID = 2;
            if (FactDispDate)
                PackageStatusID = 3;
            if (ExpDate)
                PackageStatusID = 4;

            string FileName = "Рекламации за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + "";

            ComplaintCostReport report = new ComplaintCostReport();

            if (PlanDispDate)
                report.FilterByPlanDispatch(DateFrom, DateTo);

            if (OrderDate)
                report.FilterByOrderDate(DateFrom, DateTo);

            if (ConfirmDate)
                report.FilterByConfirmDate(DateFrom, DateTo);

            if (OnAgreement)
                report.FilterByOnAgreement(DateFrom, DateTo);

            if (OnProduction)
                report.FilterByOnProduction(DateFrom, DateTo);

            if (PackDate || StoreDate || ExpDate || FactDispDate)
                report.FilterByPackages(DateFrom, DateTo, PackageStatusID);

            if (report.HasData)
            {
                report.Report(FileName);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказов с транспортом за выбранный период нет",
                       "Транспорт");
            }
        }

        private void dgvMarketManagers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMarketManagers.Columns[e.ColumnIndex].Name == "Check" && e.RowIndex != -1)
            {
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)dgvMarketManagers.
                    Rows[e.RowIndex].Cells["Check"];

                bool Checked = Convert.ToBoolean(checkCell.Value);

                if (NeedSplash)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();
                    while (!SplashWindow.bSmallCreated) ;
                    NeedSplash = false;

                    AllProductsStatistics.SetCheckClientsByManager(Checked);
                    ClientsDataGrid.Invalidate();

                    NeedSplash = true;
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
                else
                {
                    AllProductsStatistics.SetCheckClientsByManager(Checked);
                    ClientsDataGrid.Invalidate();
                }
            }
        }

        private void kryptonContextMenuItem20_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool PlanDispDate = PlanDispDateRadioButton.Checked;
            bool OrderDate = OrderDateRadioButton.Checked;
            bool ConfirmDate = ConfirmDateRadioButton.Checked;
            bool OnAgreement = OnAgreementRadioButton.Checked;
            bool PackDate = PackDateRadioButton.Checked;
            bool StoreDate = StoreDateRadioButton.Checked;
            bool ExpDate = ExpDateRadioButton.Checked;
            bool FactDispDate = FactDispDateRadioButton.Checked;
            bool OnProduction = OnProductionRadioButton.Checked;

            DateTime DateFrom = CalendarFrom.SelectionEnd;
            DateTime DateTo = CalendarTo.SelectionEnd;

            int PackageStatusID = -1;

            if (PackDate)
                PackageStatusID = 1;
            if (StoreDate)
                PackageStatusID = 2;
            if (FactDispDate)
                PackageStatusID = 3;
            if (ExpDate)
                PackageStatusID = 4;

            string FileName = "Транспорт за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy") + "";

            AdditionalCostReport report = new AdditionalCostReport();

            if (PlanDispDate)
                report.FilterByPlanDispatch(DateFrom, DateTo);

            if (OrderDate)
                report.FilterByOrderDate(DateFrom, DateTo);

            if (ConfirmDate)
                report.FilterByConfirmDate(DateFrom, DateTo);

            if (OnAgreement)
                report.FilterByOnAgreement(DateFrom, DateTo);

            if (OnProduction)
                report.FilterByOnProduction(DateFrom, DateTo);

            if (PackDate || StoreDate || ExpDate || FactDispDate)
                report.FilterByPackages(DateFrom, DateTo, PackageStatusID);

            if (report.HasData)
            {
                report.Report(FileName);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказов с дополнительной стоимостью за выбранный период нет",
                       "Дополнительно");
            }
        }
    }
}
