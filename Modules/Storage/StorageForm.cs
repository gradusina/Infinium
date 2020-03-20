using Infinium.Store;

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class StorageForm : Form
    {
        const int iMoveProducts = 51;
        const int iCreateInvoice = 52;
        const int iInventory = 53;
        const int iPrintLabel = 55;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool bMoveProducts = false;
        bool bCreateInvoice = false;
        bool bInventory = false;
        bool bPrintLabel = false;

        bool bNeedUpdate = true;
        bool NeedSplash = false;
        bool bC;

        RoleTypes RoleType = RoleTypes.OrdinaryRole;
        int BarcodeType = 13;
        int FormEvent = 0;
        int FactoryID = 1;
        int CurrentRowIndex = -1;

        DataTable MonthsDT;
        DataTable YearsDT;
        DataTable RolePermissionsDataTable;
        LightStartForm LightStartForm;
        //Connection Connection;
        //Security Security = null;
        Form TopForm = null;

        PersonalStorageManager PersonalStorageManager;
        WriteOffStoreManager WriteOffStoreManager;
        ManufactureStoreManager ManufactureStoreManager;
        ReadyStoreManager ReadyStoreManager;
        MainStoreManager MainStoreManager;
        ReportParameters ReportParameters;
        StoreGeneticsLabel StoreGeneticsLabel;
        GeneticsManager GeneticsManager;

        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1,
            DeliveryRole = 2,
            TechnologyRole = 3,
            LogisticsRole = 4
        }

        public StorageForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            LightStartForm = tLightStartForm;

            Initialize();
            //StorageManager.MoveToPersonalStore();
            while (!SplashForm.bCreated) ;
        }

        private void StorageForm_Shown(object sender, EventArgs e)
        {
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
                    }


                    return;
                }

                if (FormEvent == eShow)
                {
                    NeedSplash = true;
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
                        this.Close();
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
                    NeedSplash = true;
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void Initialize()
        {
            StoreGeneticsLabel = new Store.StoreGeneticsLabel();

            GeneticsManager = new Store.GeneticsManager();
            GeneticsManager.Initialize();

            ReportParameters = new ReportParameters();

            MainStoreManager = new MainStoreManager();

            PersonalStorageManager = new PersonalStorageManager();

            cmbInvoiceSellers.DataSource = MainStoreManager.InvoiceSellersList;
            cmbInvoiceSellers.DisplayMember = "SellerName";
            cmbInvoiceSellers.ValueMember = "SellerID";

            cmbMovInvoicePersons.DataSource = MainStoreManager.MovInvoiceSellersList;
            cmbMovInvoicePersons.DisplayMember = "Name";
            cmbMovInvoicePersons.ValueMember = "UserID";

            cmbMovInvoiceRecipients.DataSource = MainStoreManager.RecipientsStoreAllocList;
            cmbMovInvoiceRecipients.DisplayMember = "StoreAlloc";
            cmbMovInvoiceRecipients.ValueMember = "StoreAllocID";

            cmbMovInvoiceSellers.DataSource = MainStoreManager.SellerStoreAllocList;
            cmbMovInvoiceSellers.DisplayMember = "StoreAlloc";
            cmbMovInvoiceSellers.ValueMember = "StoreAllocID";

            //cmbManInvoicePersons.DataSource = ManufactureStoreManager.FilterPersonsList;
            //cmbManInvoicePersons.DisplayMember = "Name";
            //cmbManInvoicePersons.ValueMember = "UserID";

            //cmbManInvoiceRecipients.DataSource = ManufactureStoreManager.RecipientsStoreAllocList;
            //cmbManInvoiceRecipients.DisplayMember = "StoreAlloc";
            //cmbManInvoiceRecipients.ValueMember = "StoreAllocID";

            //cmbManInvoiceSellers.DataSource = ManufactureStoreManager.SellersStoreAllocList;
            //cmbManInvoiceSellers.DisplayMember = "StoreAlloc";
            //cmbManInvoiceSellers.ValueMember = "StoreAllocID";

            cmbPersonalPersons.DataSource = PersonalStorageManager.FilterPersonsList;
            cmbPersonalPersons.DisplayMember = "Name";
            cmbPersonalPersons.ValueMember = "UserID";

            cmbPersonalOtherPersons.DataSource = PersonalStorageManager.FilterOtherPersonsList;
            cmbPersonalOtherPersons.DisplayMember = "PersonName";
            cmbPersonalOtherPersons.ValueMember = "PersonName";

            cmbPersonalSellers.DataSource = PersonalStorageManager.SellersStoreAllocList;
            cmbPersonalSellers.DisplayMember = "StoreAlloc";
            cmbPersonalSellers.ValueMember = "StoreAllocID";

            MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));

            YearsDT = new DataTable();
            YearsDT.Columns.Add(new DataColumn("YearID", Type.GetType("System.Int32")));
            YearsDT.Columns.Add(new DataColumn("YearName", Type.GetType("System.String")));

            for (int i = 1; i <= 12; i++)
            {
                DataRow NewRow = MonthsDT.NewRow();
                NewRow["MonthID"] = i;
                NewRow["MonthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToString();
                MonthsDT.Rows.Add(NewRow);
            }
            cbxMonths.DataSource = MonthsDT.DefaultView;
            cbxMonths.ValueMember = "MonthID";
            cbxMonths.DisplayMember = "MonthName";

            cbxPMonths.DataSource = MonthsDT.DefaultView;
            cbxPMonths.ValueMember = "MonthID";
            cbxPMonths.DisplayMember = "MonthName";

            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            System.Collections.ArrayList Years = new System.Collections.ArrayList();
            for (int i = 2013; i <= LastDay.Year; i++)
            {
                DataRow NewRow = YearsDT.NewRow();
                NewRow["YearID"] = i;
                NewRow["YearName"] = i;
                YearsDT.Rows.Add(NewRow);
                Years.Add(i);
            }
            cbxYears.DataSource = YearsDT.DefaultView;
            cbxYears.ValueMember = "YearID";
            cbxYears.DisplayMember = "YearName";

            cbxPYears.DataSource = YearsDT.DefaultView;
            cbxPYears.ValueMember = "YearID";
            cbxPYears.DisplayMember = "YearName";
            cbxMonths.SelectedValue = DateTime.Now.Month;
            cbxYears.SelectedValue = DateTime.Now.Year;

            //dtpMovInvoiceFilterFrom.Value = DateTime.Now.AddMonths(-1);
            //dtpMovInvoiceFilterTo.Value = DateTime.Now;

            dgvMainStore.DataSource = MainStoreManager.StoreList;
            dgvMainStoreSubGroups.DataSource = MainStoreManager.SubGroupsList;
            dgvMainStoreGroups.DataSource = MainStoreManager.GroupsList;

            dgvPurchInvoices.DataSource = MainStoreManager.InvoicesList;
            dgvPurchInvoiceItems.DataSource = MainStoreManager.InvoiceItemsList;
            dgvMovInvoices.DataSource = MainStoreManager.MovementInvoicesList;
            dgvMovInvoiceItems.DataSource = MainStoreManager.MovementInvoiceItemsList;

            dgvMainStoreGroups.Columns["TechStoreGroupID"].Visible = false;

            dgvMainStoreSubGroups.Columns["TechStoreGroupID"].Visible = false;
            dgvMainStoreSubGroups.Columns["TechStoreSubGroupID"].Visible = false;
            //dgvMainStoreSubGroups.Columns["Notes"].Visible = false;
            //dgvMainStoreSubGroups.Columns["Notes1"].Visible = false;
            //dgvMainStoreSubGroups.Columns["Notes2"].Visible = false;

            dgvMainStoreSettings(ref dgvMainStore);
            dgvMainStorePurchInvoicesSettings();
            dgvWriteOffStoreMovInvoicesSettings(ref dgvMovInvoices);
            dgvMainStoreMovInvoiceItemsSettings(ref dgvMovInvoiceItems);

            dgvPersStore.DataSource = PersonalStorageManager.StoreList;
            dgvPersSubGroups.DataSource = PersonalStorageManager.SubGroupsList;
            dgvPersGroups.DataSource = PersonalStorageManager.GroupsList;

            dgvPersInvoices.DataSource = PersonalStorageManager.InvoicesList;
            dgvPersInvoiceItems.DataSource = PersonalStorageManager.InvoiceItemsList;
            PersonalStoreGridSettings();
            PersonalInvoiceGridSettings();
            FilterMainStore();

            ManufactureStoreInitialize();
            ReadyStoreInitialize();
            WriteOffStoreInitialize();
            kryptonCheckSet3.CheckedButton = cbtnMainStore;

            dgvMainStoreGroups.SelectionChanged -= dgvMainStoreGroups_SelectionChanged;
            dgvMainStoreSubGroups.SelectionChanged -= dgvMainStoreSubGroups_SelectionChanged;
            dgvPurchInvoices.SelectionChanged -= dgvPurchInvoices_SelectionChanged;
            dgvDispGroups.SelectionChanged -= dgvDispGroups_SelectionChanged;
            dgvDispInvoices.SelectionChanged -= dgvDispInvoices_SelectionChanged;
            dgvManufactreStoreGroups.SelectionChanged -= dgvManufactreStoreGroups_SelectionChanged;
            dgvManufactreStoreInvoices.SelectionChanged -= dgvManufactreStoreInvoices_SelectionChanged;
            dgvMovInvoices.SelectionChanged -= dgvMovInvoices_SelectionChanged;
            dgvPersGroups.SelectionChanged -= dgvPersGroups_SelectionChanged;
            dgvPersInvoices.SelectionChanged -= dgvPersInvoices_SelectionChanged;
            dgvReadyStoreGroups.SelectionChanged -= dgvReadyStoreGroups_SelectionChanged;
            dgvReadyStoreInvoices.SelectionChanged -= dgvReadyStoreInvoices_SelectionChanged;
        }

        private void ManufactureStoreInitialize()
        {
            ManufactureStoreManager = new ManufactureStoreManager();

            dgvManufactreStore.DataSource = ManufactureStoreManager.StoreList;
            dgvManufactreStoreSubGroups.DataSource = ManufactureStoreManager.SubGroupsList;
            dgvManufactreStoreGroups.DataSource = ManufactureStoreManager.GroupsList;

            dgvManufactreStoreInvoices.DataSource = ManufactureStoreManager.MovementInvoicesList;
            dgvManufactreStoreInvoiceItems.DataSource = ManufactureStoreManager.MovementInvoiceItemsList;

            dgvManufactreStoreGroups.Columns["TechStoreGroupID"].Visible = false;

            dgvManufactreStoreSubGroups.Columns["TechStoreGroupID"].Visible = false;
            dgvManufactreStoreSubGroups.Columns["TechStoreSubGroupID"].Visible = false;
            dgvManufactreStoreSubGroups.Columns["Notes"].Visible = false;
            dgvManufactreStoreSubGroups.Columns["Notes1"].Visible = false;
            dgvManufactreStoreSubGroups.Columns["Notes2"].Visible = false;

            dgvManufactureStoreSettings(ref dgvManufactreStore);

            dgvWriteOffStoreMovInvoicesSettings(ref dgvManufactreStoreInvoices);
            dgvManufactureStoreMovInvoiceItemsSettings(ref dgvManufactreStoreInvoiceItems);
        }

        private void ReadyStoreInitialize()
        {
            ReadyStoreManager = new ReadyStoreManager();

            dgvReadyStore.DataSource = ReadyStoreManager.StoreList;
            dgvReadyStoreSubGroups.DataSource = ReadyStoreManager.SubGroupsList;
            dgvReadyStoreGroups.DataSource = ReadyStoreManager.GroupsList;

            dgvReadyStoreInvoices.DataSource = ReadyStoreManager.MovementInvoicesList;
            dgvReadyStoreInvoiceItems.DataSource = ReadyStoreManager.MovementInvoiceItemsList;

            dgvReadyStoreGroups.Columns["TechStoreGroupID"].Visible = false;

            dgvReadyStoreSubGroups.Columns["TechStoreGroupID"].Visible = false;
            dgvReadyStoreSubGroups.Columns["TechStoreSubGroupID"].Visible = false;
            dgvReadyStoreSubGroups.Columns["Notes"].Visible = false;
            dgvReadyStoreSubGroups.Columns["Notes1"].Visible = false;
            dgvReadyStoreSubGroups.Columns["Notes2"].Visible = false;

            dgvReadyStoreSettings(ref dgvReadyStore);

            dgvWriteOffStoreMovInvoicesSettings(ref dgvReadyStoreInvoices);
            dgvReadyStoreMovInvoiceItemsSettings(ref dgvReadyStoreInvoiceItems);
        }

        private void WriteOffStoreInitialize()
        {
            WriteOffStoreManager = new WriteOffStoreManager();

            dgvDispStore.DataSource = WriteOffStoreManager.StoreList;
            dgvDispSubGroups.DataSource = WriteOffStoreManager.SubGroupsList;
            dgvDispGroups.DataSource = WriteOffStoreManager.GroupsList;

            dgvDispInvoices.DataSource = WriteOffStoreManager.MovementInvoicesList;
            dgvDispInvoiceItems.DataSource = WriteOffStoreManager.MovementInvoiceItemsList;

            dgvDispGroups.Columns["TechStoreGroupID"].Visible = false;

            dgvDispSubGroups.Columns["TechStoreGroupID"].Visible = false;
            dgvDispSubGroups.Columns["TechStoreSubGroupID"].Visible = false;
            dgvDispSubGroups.Columns["Notes"].Visible = false;
            dgvDispSubGroups.Columns["Notes1"].Visible = false;
            dgvDispSubGroups.Columns["Notes2"].Visible = false;

            dgvWriteOffStoreSettings(ref dgvDispStore);

            dgvWriteOffStoreMovInvoicesSettings(ref dgvDispInvoices);
            dgvWriteOffStoreMovInvoiceItemsSettings(ref dgvDispInvoiceItems);
        }

        public void dgvMainStoreSettings(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(MainStoreManager.ColorsColumn);
            grid.Columns.Add(MainStoreManager.PatinaColumn);
            grid.Columns.Add(MainStoreManager.CoversColumn);
            grid.Columns.Add(MainStoreManager.CurrencyColumn);
            grid.Columns.Add(MainStoreManager.ManufacturerColumn);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            grid.Columns["Price"].DefaultCellStyle.Format = "N";
            grid.Columns["Price"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["VAT"].DefaultCellStyle.Format = "N";
            grid.Columns["VAT"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Cost"].DefaultCellStyle.Format = "N";
            grid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["VATCost"].DefaultCellStyle.Format = "N";
            grid.Columns["VATCost"].DefaultCellStyle.FormatProvider = nfi1;

            grid.Columns["Thickness"].DefaultCellStyle.Format = "N";
            grid.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Length"].DefaultCellStyle.Format = "N";
            grid.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Height"].DefaultCellStyle.Format = "N";
            grid.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Width"].DefaultCellStyle.Format = "N";
            grid.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Admission"].DefaultCellStyle.Format = "N";
            grid.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Diameter"].DefaultCellStyle.Format = "N";
            grid.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Capacity"].DefaultCellStyle.Format = "N";
            grid.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Weight"].DefaultCellStyle.Format = "N";
            grid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            grid.Columns["InvoiceCount"].Visible = false;
            grid.Columns["StoreID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["CoverID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["StoreItemID"].Visible = false;
            grid.Columns["CurrencyTypeID"].Visible = false;
            grid.Columns["FactoryID"].Visible = false;
            grid.Columns["PurchaseInvoiceID"].Visible = false;
            grid.Columns["EndMonthCount"].Visible = false;
            grid.Columns["PriceEUR"].Visible = false;
            grid.Columns["ManufacturerID"].Visible = false;
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;

            grid.Columns["IncomeDate"].HeaderText = "Дата прихода";
            grid.Columns["StoreItemColumn"].HeaderText = "Наименование";
            grid.Columns["SellerCode"].HeaderText = "Код поставщика";
            grid.Columns["Length"].HeaderText = "Длина, мм";
            grid.Columns["Width"].HeaderText = "Ширина, мм";
            grid.Columns["Height"].HeaderText = "Высота, мм";
            grid.Columns["Thickness"].HeaderText = "Толщина, мм";
            grid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            grid.Columns["Admission"].HeaderText = "Допуск, мм";
            grid.Columns["Weight"].HeaderText = "Вес, кг";
            grid.Columns["Capacity"].HeaderText = "Емкость, л";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["IsArrived"].HeaderText = "Проведено";
            grid.Columns["Price"].HeaderText = "Цена";
            grid.Columns["VAT"].HeaderText = "НДС";
            grid.Columns["Cost"].HeaderText = "Сумма";
            grid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            grid.Columns["StartMonthCount"].HeaderText = "ОСТн";
            grid.Columns["MonthInvoiceCount"].HeaderText = "Приход";
            grid.Columns["ExpenseCount"].HeaderText = "Расход";
            grid.Columns["SellingCount"].HeaderText = "Реализация";
            //grid.Columns["EndMonthCount"].HeaderText = "ОСТк";
            grid.Columns["BatchNumber"].HeaderText = "№ партии";
            grid.Columns["Produced"].HeaderText = "Произведено";
            grid.Columns["BestBefore"].HeaderText = "Срок годности";
            grid.Columns["CurrentCount"].HeaderText = "ОСТк";
            grid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";

            grid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            grid.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            grid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            grid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            grid.Columns["StartMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["MonthInvoiceCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["ExpenseCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellingCount"].DisplayIndex = DisplayIndex++;
            //grid.Columns["EndMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Price"].DisplayIndex = DisplayIndex++;
            grid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            grid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            grid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["IsArrived"].DisplayIndex = DisplayIndex++;
            grid.Columns["ManufacturerColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["BatchNumber"].DisplayIndex = DisplayIndex++;
            grid.Columns["Produced"].DisplayIndex = DisplayIndex++;
            grid.Columns["BestBefore"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["StoreID"].DisplayIndex = DisplayIndex++;

            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CreateDateTime"].Width = 120;
            grid.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["StoreItemColumn"].MinimumWidth = 100;
            grid.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["SellerCode"].MinimumWidth = 70;
            grid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ColorsColumn"].MinimumWidth = 100;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].MinimumWidth = 100;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CoversColumn"].MinimumWidth = 100;
            grid.Columns["IncomeDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["IncomeDate"].MinimumWidth = 100;

            grid.Columns["StoreID"].MinimumWidth = 60;
            grid.Columns["StoreID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length"].MinimumWidth = 60;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].MinimumWidth = 60;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].MinimumWidth = 60;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Admission"].MinimumWidth = 60;
            grid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Diameter"].MinimumWidth = 60;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Capacity"].MinimumWidth = 60;
            grid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Weight"].MinimumWidth = 60;
            grid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Price"].MinimumWidth = 60;
            grid.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MonthInvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MonthInvoiceCount"].Width = 90;
            grid.Columns["StartMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StartMonthCount"].Width = 90;
            //grid.Columns["EndMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //grid.Columns["EndMonthCount"].Width = 90;
            grid.Columns["ExpenseCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpenseCount"].Width = 90;
            grid.Columns["SellingCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["SellingCount"].Width = 90;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 90;
            grid.Columns["VAT"].MinimumWidth = 60;
            grid.Columns["VAT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Cost"].MinimumWidth = 60;
            grid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["VATCost"].MinimumWidth = 130;
            grid.Columns["VATCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CurrencyColumn"].MinimumWidth = 60;
            grid.Columns["CurrencyColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["IsArrived"].MinimumWidth = 60;
            grid.Columns["IsArrived"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 60;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["BatchNumber"].MinimumWidth = 60;
            grid.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Produced"].MinimumWidth = 60;
            grid.Columns["Produced"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["BestBefore"].MinimumWidth = 60;
            grid.Columns["BestBefore"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        public void dgvMainStorePurchInvoicesSettings()
        {
            dgvPurchInvoices.Columns.Add(MainStoreManager.PurchInvoiceSellersColumn);
            //dgvPurchInvoices.Columns["PurchaseInvoiceID"].Visible = false;
            dgvPurchInvoices.Columns["FactoryID"].Visible = false;
            dgvPurchInvoices.Columns["SellerID"].Visible = false;
            dgvPurchInvoices.Columns["CurrencyTypeID"].Visible = false;
            dgvPurchInvoices.Columns["Rate"].Visible = false;

            dgvPurchInvoices.Columns["PurchaseInvoiceID"].HeaderText = "№п\\п";
            dgvPurchInvoices.Columns["IncomeDate"].HeaderText = "Дата прихода";
            dgvPurchInvoices.Columns["DocNumber"].HeaderText = "№ документа";
            dgvPurchInvoices.Columns["SellersColumn"].HeaderText = "Поставщик";
            dgvPurchInvoices.Columns["Reason"].HeaderText = "Основание";
            dgvPurchInvoices.Columns["Notes"].HeaderText = "Примечание";

            dgvPurchInvoices.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvPurchInvoices.Columns["IncomeDate"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoices.Columns["DocNumber"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoices.Columns["SellersColumn"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoices.Columns["Reason"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoices.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoices.Columns["PurchaseInvoiceID"].DisplayIndex = DisplayIndex++;

            dgvPurchInvoices.Columns["PurchaseInvoiceID"].MinimumWidth = 50;
            dgvPurchInvoices.Columns["PurchaseInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoices.Columns["IncomeDate"].MinimumWidth = 150;
            dgvPurchInvoices.Columns["IncomeDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoices.Columns["DocNumber"].MinimumWidth = 150;
            dgvPurchInvoices.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoices.Columns["SellersColumn"].MinimumWidth = 150;
            dgvPurchInvoices.Columns["SellersColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoices.Columns["Reason"].MinimumWidth = 150;
            dgvPurchInvoices.Columns["Reason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoices.Columns["Notes"].MinimumWidth = 150;
            dgvPurchInvoices.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvPurchInvoiceItems.AutoGenerateColumns = false;

            dgvPurchInvoiceItems.Columns.Add(MainStoreManager.PurchInvoiceColorsColumn);
            dgvPurchInvoiceItems.Columns.Add(MainStoreManager.PurchInvoicePatinaColumn);
            dgvPurchInvoiceItems.Columns.Add(MainStoreManager.PurchInvoiceCoversColumn);
            dgvPurchInvoiceItems.Columns.Add(MainStoreManager.PurchInvoiceCurrencyColumn);
            dgvPurchInvoiceItems.Columns.Add(MainStoreManager.ManufacturerColumn);

            if (dgvPurchInvoiceItems.Columns.Contains("CreateUserID"))
                dgvPurchInvoiceItems.Columns["CreateUserID"].Visible = false;
            dgvPurchInvoiceItems.Columns["StoreID"].Visible = false;
            dgvPurchInvoiceItems.Columns["ColorID"].Visible = false;
            dgvPurchInvoiceItems.Columns["CoverID"].Visible = false;
            dgvPurchInvoiceItems.Columns["PatinaID"].Visible = false;
            dgvPurchInvoiceItems.Columns["StoreItemID"].Visible = false;
            dgvPurchInvoiceItems.Columns["CurrencyTypeID"].Visible = false;
            dgvPurchInvoiceItems.Columns["FactoryID"].Visible = false;
            dgvPurchInvoiceItems.Columns["PurchaseInvoiceID"].Visible = false;
            dgvPurchInvoiceItems.Columns["MovementInvoiceID"].Visible = false;
            dgvPurchInvoiceItems.Columns["ManufacturerID"].Visible = false;

            dgvPurchInvoiceItems.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvPurchInvoiceItems.Columns["SellerCode"].HeaderText = "Код поставщика";
            dgvPurchInvoiceItems.Columns["Length"].HeaderText = "Длина, мм";
            dgvPurchInvoiceItems.Columns["Width"].HeaderText = "Ширина, мм";
            dgvPurchInvoiceItems.Columns["Height"].HeaderText = "Высота, мм";
            dgvPurchInvoiceItems.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvPurchInvoiceItems.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvPurchInvoiceItems.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvPurchInvoiceItems.Columns["Weight"].HeaderText = "Вес, кг";
            dgvPurchInvoiceItems.Columns["Capacity"].HeaderText = "Емкость, л";
            dgvPurchInvoiceItems.Columns["Notes"].HeaderText = "Примечание";
            dgvPurchInvoiceItems.Columns["IsArrived"].HeaderText = "Проведено";
            dgvPurchInvoiceItems.Columns["Price"].HeaderText = "Цена";
            dgvPurchInvoiceItems.Columns["VAT"].HeaderText = "НДС";
            dgvPurchInvoiceItems.Columns["Cost"].HeaderText = "Сумма";
            dgvPurchInvoiceItems.Columns["VATCost"].HeaderText = "Сумма с НДС";
            dgvPurchInvoiceItems.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            dgvPurchInvoiceItems.Columns["CurrentCount"].HeaderText = "Остаток";
            dgvPurchInvoiceItems.Columns["BatchNumber"].HeaderText = "№ партии";
            dgvPurchInvoiceItems.Columns["Produced"].HeaderText = "Произведено";
            dgvPurchInvoiceItems.Columns["BestBefore"].HeaderText = "Срок годности";
            dgvPurchInvoiceItems.Columns["CreateDateTime"].HeaderText = "Дата сохранения";

            DisplayIndex = 0;
            dgvPurchInvoiceItems.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Admission"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Price"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Cost"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["VAT"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["IsArrived"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["ManufacturerColumn"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["BatchNumber"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Produced"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["BestBefore"].DisplayIndex = DisplayIndex++;
            dgvPurchInvoiceItems.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvPurchInvoiceItems.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvPurchInvoiceItems.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["SellerCode"].MinimumWidth = 70;
            dgvPurchInvoiceItems.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvPurchInvoiceItems.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvPurchInvoiceItems.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["CoversColumn"].MinimumWidth = 100;
            dgvPurchInvoiceItems.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPurchInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Length"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Height"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Width"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Admission"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Diameter"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Capacity"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Weight"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Price"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["InvoiceCount"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["CurrentCount"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["VAT"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["VAT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Cost"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["VATCost"].MinimumWidth = 130;
            dgvPurchInvoiceItems.Columns["VATCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["CurrencyColumn"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["CurrencyColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["IsArrived"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["IsArrived"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Notes"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["BatchNumber"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["Produced"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["Produced"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPurchInvoiceItems.Columns["BestBefore"].MinimumWidth = 60;
            dgvPurchInvoiceItems.Columns["BestBefore"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPurchInvoiceItems.Columns["Price"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Price"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["VAT"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["VAT"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Cost"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["VATCost"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["VATCost"].DefaultCellStyle.FormatProvider = nfi1;

            dgvPurchInvoiceItems.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPurchInvoiceItems.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvPurchInvoiceItems.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;
        }

        public void dgvMainStoreMovInvoicesSettings(ref PercentageDataGrid dgvInvoices)
        {
            //MovementInvoicesDataGrid.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoices.Columns["CreateUserID"].Visible = false;
            dgvInvoices.Columns["CreateDateTime"].Visible = false;
            dgvInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientSectorID"].Visible = false;
            dgvInvoices.Columns["PersonID"].Visible = false;
            dgvInvoices.Columns["StoreKeeperID"].Visible = false;
            dgvInvoices.Columns["SellerID"].Visible = false;
            dgvInvoices.Columns["ClientID"].Visible = false;
            dgvInvoices.Columns["ClientName"].Visible = false;

            dgvInvoices.Columns["DateTime"].HeaderText = "Дата перемещения";
            dgvInvoices.Columns["SellerStoreAlloc"].HeaderText = "Откуда";
            dgvInvoices.Columns["RecipientStoreAlloc"].HeaderText = "Куда";
            dgvInvoices.Columns["Sector"].HeaderText = "Участок";
            dgvInvoices.Columns["PersonName"].HeaderText = "МОЛ";
            dgvInvoices.Columns["StoreKeeper"].HeaderText = "Кладовщик";
            dgvInvoices.Columns["Notes"].HeaderText = "Примечание";

            dgvInvoices.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvInvoices.Columns["DateTime"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["SellerStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["RecipientStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Sector"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["PersonName"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["StoreKeeper"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["MovementInvoiceID"].DisplayIndex = DisplayIndex++;

            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 100;
            dgvInvoices.Columns["DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["DateTime"].MinimumWidth = 100;
            dgvInvoices.Columns["SellerStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["SellerStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["RecipientStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["RecipientStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["Sector"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["Sector"].MinimumWidth = 100;
            dgvInvoices.Columns["PersonName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["PersonName"].MinimumWidth = 100;
            dgvInvoices.Columns["StoreKeeper"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["StoreKeeper"].MinimumWidth = 100;
            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 100;
        }

        public void dgvMainStoreMovInvoiceItemsSettings(ref PercentageDataGrid dgvInvoiceItems)
        {
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceColorsColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoicePatinaColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceCoversColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.ManufacturerColumn);

            dgvInvoiceItems.AutoGenerateColumns = false;

            if (dgvInvoiceItems.Columns.Contains("CreateUserID"))
                dgvInvoiceItems.Columns["CreateUserID"].Visible = false;
            dgvInvoiceItems.Columns["StoreID"].Visible = false;
            dgvInvoiceItems.Columns["ColorID"].Visible = false;
            dgvInvoiceItems.Columns["CoverID"].Visible = false;
            dgvInvoiceItems.Columns["PatinaID"].Visible = false;
            dgvInvoiceItems.Columns["StoreItemID"].Visible = false;
            dgvInvoiceItems.Columns["CurrencyTypeID"].Visible = false;
            dgvInvoiceItems.Columns["FactoryID"].Visible = false;
            dgvInvoiceItems.Columns["PurchaseInvoiceID"].Visible = false;
            dgvInvoiceItems.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoiceItems.Columns["PriceEUR"].Visible = false;
            dgvInvoiceItems.Columns["IsArrived"].Visible = false;
            dgvInvoiceItems.Columns["ManufacturerID"].Visible = false;
            dgvInvoiceItems.Columns["DecorAssignmentID"].Visible = false;
            dgvInvoiceItems.Columns["Price"].Visible = false;
            dgvInvoiceItems.Columns["VAT"].Visible = false;
            dgvInvoiceItems.Columns["Cost"].Visible = false;
            dgvInvoiceItems.Columns["VATCost"].Visible = false;
            if (dgvInvoiceItems.Columns.Contains("CreateUserID"))
                dgvInvoiceItems.Columns["CreateUserID"].Visible = false;

            dgvInvoiceItems.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvInvoiceItems.Columns["SellerCode"].HeaderText = "Код поставщика";
            dgvInvoiceItems.Columns["Length"].HeaderText = "Длина, мм";
            dgvInvoiceItems.Columns["Width"].HeaderText = "Ширина, мм";
            dgvInvoiceItems.Columns["Height"].HeaderText = "Высота, мм";
            dgvInvoiceItems.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvInvoiceItems.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvInvoiceItems.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvInvoiceItems.Columns["Weight"].HeaderText = "Вес, кг";
            dgvInvoiceItems.Columns["Capacity"].HeaderText = "Емкость, л";
            dgvInvoiceItems.Columns["Notes"].HeaderText = "Примечание";
            dgvInvoiceItems.Columns["IsArrived"].HeaderText = "Проведено";
            dgvInvoiceItems.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            dgvInvoiceItems.Columns["CurrentCount"].HeaderText = "Остаток";
            dgvInvoiceItems.Columns["BatchNumber"].HeaderText = "№ партии";
            dgvInvoiceItems.Columns["Produced"].HeaderText = "Произведено";
            dgvInvoiceItems.Columns["BestBefore"].HeaderText = "Срок годности";
            dgvInvoiceItems.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            dgvInvoiceItems.Columns["Price"].HeaderText = "Цена";
            dgvInvoiceItems.Columns["VAT"].HeaderText = "НДС";
            dgvInvoiceItems.Columns["Cost"].HeaderText = "Сумма";
            dgvInvoiceItems.Columns["VATCost"].HeaderText = "Сумма с НДС";
            dgvInvoiceItems.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            dgvInvoiceItems.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Admission"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["IsArrived"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["ManufacturerColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["BatchNumber"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Produced"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["BestBefore"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvInvoiceItems.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].MinimumWidth = 70;
            dgvInvoiceItems.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CoversColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Length"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Height"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Width"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Admission"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Diameter"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Capacity"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Weight"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["InvoiceCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CurrentCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["IsArrived"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["IsArrived"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Notes"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            dgvInvoiceItems.Columns["BatchNumber"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Produced"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Produced"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["BestBefore"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["BestBefore"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        public void dgvManufactureStoreSettings(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(MainStoreManager.ColorsColumn);
            grid.Columns.Add(MainStoreManager.PatinaColumn);
            grid.Columns.Add(MainStoreManager.CoversColumn);
            grid.Columns.Add(MainStoreManager.CurrencyColumn);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            grid.Columns["Thickness"].DefaultCellStyle.Format = "N";
            grid.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Length"].DefaultCellStyle.Format = "N";
            grid.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Height"].DefaultCellStyle.Format = "N";
            grid.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Width"].DefaultCellStyle.Format = "N";
            grid.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Admission"].DefaultCellStyle.Format = "N";
            grid.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Diameter"].DefaultCellStyle.Format = "N";
            grid.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Capacity"].DefaultCellStyle.Format = "N";
            grid.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Weight"].DefaultCellStyle.Format = "N";
            grid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            if (grid.Columns.Contains("ReturnedRollerID"))
                grid.Columns["ReturnedRollerID"].Visible = false;
            if (grid.Columns.Contains("Produced"))
                grid.Columns["Produced"].Visible = false;
            grid.Columns["InvoiceCount"].Visible = false;
            grid.Columns["ManufactureStoreID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["CoverID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["StoreItemID"].Visible = false;
            grid.Columns["FactoryID"].Visible = false;
            grid.Columns["EndMonthCount"].Visible = false;

            grid.Columns["StoreItemColumn"].HeaderText = "Наименование";
            grid.Columns["SellerCode"].HeaderText = "Код поставщика";
            grid.Columns["Length"].HeaderText = "Длина, мм";
            grid.Columns["Width"].HeaderText = "Ширина, мм";
            grid.Columns["Height"].HeaderText = "Высота, мм";
            grid.Columns["Thickness"].HeaderText = "Толщина, мм";
            grid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            grid.Columns["Admission"].HeaderText = "Допуск, мм";
            grid.Columns["Weight"].HeaderText = "Вес, кг";
            grid.Columns["Capacity"].HeaderText = "Емкость, л";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["StartMonthCount"].HeaderText = "ОСТн";
            grid.Columns["MonthInvoiceCount"].HeaderText = "Приход";
            grid.Columns["ExpenseCount"].HeaderText = "Расход";
            grid.Columns["SellingCount"].HeaderText = "Реализация";
            //grid.Columns["EndMonthCount"].HeaderText = "ОСТк";
            grid.Columns["CurrentCount"].HeaderText = "ОСТк";
            grid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";

            grid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            grid.Columns["StoreItemID"].DisplayIndex = DisplayIndex++;
            grid.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            grid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            grid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            grid.Columns["StartMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["MonthInvoiceCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["ExpenseCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellingCount"].DisplayIndex = DisplayIndex++;
            //grid.Columns["EndMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["ManufactureStoreID"].DisplayIndex = DisplayIndex++;

            grid.Columns["StoreItemID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StoreItemID"].Width = 120;
            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CreateDateTime"].Width = 120;
            grid.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["StoreItemColumn"].MinimumWidth = 100;
            grid.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["SellerCode"].MinimumWidth = 70;
            grid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ColorsColumn"].MinimumWidth = 100;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].MinimumWidth = 100;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CoversColumn"].MinimumWidth = 100;

            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length"].MinimumWidth = 60;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].MinimumWidth = 60;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].MinimumWidth = 60;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Admission"].MinimumWidth = 60;
            grid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Diameter"].MinimumWidth = 60;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Capacity"].MinimumWidth = 60;
            grid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Weight"].MinimumWidth = 60;
            grid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MonthInvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MonthInvoiceCount"].Width = 90;
            grid.Columns["StartMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StartMonthCount"].Width = 90;
            //grid.Columns["EndMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //grid.Columns["EndMonthCount"].Width = 90;
            grid.Columns["ExpenseCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpenseCount"].Width = 90;
            grid.Columns["SellingCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["SellingCount"].Width = 90;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 90;
            grid.Columns["ManufactureStoreID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ManufactureStoreID"].Width = 90;
            grid.Columns["Notes"].MinimumWidth = 60;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        public void dgvManufactureStoreMovInvoicesSettings(ref PercentageDataGrid dgvInvoices)
        {
            //MovementInvoicesDataGrid.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoices.Columns["TypeCreation"].Visible = false;
            dgvInvoices.Columns["CreateUserID"].Visible = false;
            dgvInvoices.Columns["CreateDateTime"].Visible = false;
            dgvInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientSectorID"].Visible = false;
            dgvInvoices.Columns["PersonID"].Visible = false;
            dgvInvoices.Columns["StoreKeeperID"].Visible = false;
            dgvInvoices.Columns["SellerID"].Visible = false;
            dgvInvoices.Columns["ClientID"].Visible = false;
            dgvInvoices.Columns["ClientName"].Visible = false;

            dgvInvoices.Columns["DateTime"].HeaderText = "Дата перемещения";
            dgvInvoices.Columns["SellerStoreAlloc"].HeaderText = "Откуда";
            dgvInvoices.Columns["RecipientStoreAlloc"].HeaderText = "Куда";
            dgvInvoices.Columns["Sector"].HeaderText = "Участок";
            dgvInvoices.Columns["PersonName"].HeaderText = "МОЛ";
            dgvInvoices.Columns["StoreKeeper"].HeaderText = "Кладовщик";
            dgvInvoices.Columns["Notes"].HeaderText = "Примечание";

            dgvInvoices.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvInvoices.Columns["DateTime"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["SellerStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["RecipientStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Sector"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["PersonName"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["StoreKeeper"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["MovementInvoiceID"].DisplayIndex = DisplayIndex++;

            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 100;
            dgvInvoices.Columns["DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["DateTime"].MinimumWidth = 100;
            dgvInvoices.Columns["SellerStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["SellerStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["RecipientStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["RecipientStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["Sector"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["Sector"].MinimumWidth = 100;
            dgvInvoices.Columns["PersonName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["PersonName"].MinimumWidth = 100;
            dgvInvoices.Columns["StoreKeeper"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["StoreKeeper"].MinimumWidth = 100;
            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 100;
        }

        public void dgvManufactureStoreMovInvoiceItemsSettings(ref PercentageDataGrid dgvInvoiceItems)
        {
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceColorsColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoicePatinaColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceCoversColumn);

            dgvInvoiceItems.AutoGenerateColumns = false;

            if (dgvInvoiceItems.Columns.Contains("CreateUserID"))
                dgvInvoiceItems.Columns["CreateUserID"].Visible = false;
            dgvInvoiceItems.Columns["ManufactureStoreID"].Visible = false;
            dgvInvoiceItems.Columns["ColorID"].Visible = false;
            dgvInvoiceItems.Columns["CoverID"].Visible = false;
            dgvInvoiceItems.Columns["PatinaID"].Visible = false;
            dgvInvoiceItems.Columns["StoreItemID"].Visible = false;
            dgvInvoiceItems.Columns["FactoryID"].Visible = false;
            dgvInvoiceItems.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoiceItems.Columns["DecorAssignmentID"].Visible = false;

            dgvInvoiceItems.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvInvoiceItems.Columns["SellerCode"].HeaderText = "Код поставщика";
            dgvInvoiceItems.Columns["SellerCode"].HeaderText = "Наименование";
            dgvInvoiceItems.Columns["Length"].HeaderText = "Длина, мм";
            dgvInvoiceItems.Columns["Width"].HeaderText = "Ширина, мм";
            dgvInvoiceItems.Columns["Height"].HeaderText = "Высота, мм";
            dgvInvoiceItems.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvInvoiceItems.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvInvoiceItems.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvInvoiceItems.Columns["Weight"].HeaderText = "Вес, кг";
            dgvInvoiceItems.Columns["Capacity"].HeaderText = "Емкость, л";
            dgvInvoiceItems.Columns["Notes"].HeaderText = "Примечание";
            dgvInvoiceItems.Columns["Produced"].HeaderText = "Произведено";
            dgvInvoiceItems.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            dgvInvoiceItems.Columns["CurrentCount"].HeaderText = "Остаток";
            dgvInvoiceItems.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            //dgvInvoiceItems.Columns["Price"].HeaderText = "Цена";
            //dgvInvoiceItems.Columns["VAT"].HeaderText = "НДС";
            //dgvInvoiceItems.Columns["Cost"].HeaderText = "Сумма";
            //dgvInvoiceItems.Columns["VATCost"].HeaderText = "Сумма с НДС";
            dgvInvoiceItems.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            dgvInvoiceItems.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Admission"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvInvoiceItems.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].MinimumWidth = 70;
            dgvInvoiceItems.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CoversColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Length"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Height"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Width"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Admission"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Diameter"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Capacity"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Weight"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["InvoiceCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CurrentCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Notes"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;
        }

        public void dgvReadyStoreSettings(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(MainStoreManager.ColorsColumn);
            grid.Columns.Add(MainStoreManager.PatinaColumn);
            grid.Columns.Add(MainStoreManager.CoversColumn);
            grid.Columns.Add(MainStoreManager.CurrencyColumn);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            grid.Columns["Thickness"].DefaultCellStyle.Format = "N";
            grid.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Length"].DefaultCellStyle.Format = "N";
            grid.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Height"].DefaultCellStyle.Format = "N";
            grid.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Width"].DefaultCellStyle.Format = "N";
            grid.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Admission"].DefaultCellStyle.Format = "N";
            grid.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Diameter"].DefaultCellStyle.Format = "N";
            grid.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Capacity"].DefaultCellStyle.Format = "N";
            grid.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Weight"].DefaultCellStyle.Format = "N";
            grid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            grid.Columns["InvoiceCount"].Visible = false;
            grid.Columns["ReadyStoreID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["CoverID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["StoreItemID"].Visible = false;
            grid.Columns["FactoryID"].Visible = false;
            grid.Columns["EndMonthCount"].Visible = false;

            grid.Columns["StoreItemColumn"].HeaderText = "Наименование";
            grid.Columns["SellerCode"].HeaderText = "Код поставщика";
            grid.Columns["Length"].HeaderText = "Длина, мм";
            grid.Columns["Width"].HeaderText = "Ширина, мм";
            grid.Columns["Height"].HeaderText = "Высота, мм";
            grid.Columns["Thickness"].HeaderText = "Толщина, мм";
            grid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            grid.Columns["Admission"].HeaderText = "Допуск, мм";
            grid.Columns["Weight"].HeaderText = "Вес, кг";
            grid.Columns["Capacity"].HeaderText = "Емкость, л";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["StartMonthCount"].HeaderText = "ОСТн";
            grid.Columns["MonthInvoiceCount"].HeaderText = "Приход";
            grid.Columns["ExpenseCount"].HeaderText = "Расход";
            grid.Columns["SellingCount"].HeaderText = "Реализация";
            //grid.Columns["EndMonthCount"].HeaderText = "ОСТк";
            grid.Columns["CurrentCount"].HeaderText = "ОСТк";
            grid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";

            grid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            grid.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            grid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            grid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            grid.Columns["StartMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["MonthInvoiceCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["ExpenseCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellingCount"].DisplayIndex = DisplayIndex++;
            //grid.Columns["EndMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CreateDateTime"].Width = 120;
            grid.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["StoreItemColumn"].MinimumWidth = 100;
            grid.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["SellerCode"].MinimumWidth = 70;
            grid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ColorsColumn"].MinimumWidth = 100;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].MinimumWidth = 100;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CoversColumn"].MinimumWidth = 100;

            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length"].MinimumWidth = 60;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].MinimumWidth = 60;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].MinimumWidth = 60;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Admission"].MinimumWidth = 60;
            grid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Diameter"].MinimumWidth = 60;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Capacity"].MinimumWidth = 60;
            grid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Weight"].MinimumWidth = 60;
            grid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MonthInvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MonthInvoiceCount"].Width = 90;
            grid.Columns["StartMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StartMonthCount"].Width = 90;
            //grid.Columns["EndMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //grid.Columns["EndMonthCount"].Width = 90;
            grid.Columns["ExpenseCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpenseCount"].Width = 90;
            grid.Columns["SellingCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["SellingCount"].Width = 90;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 90;
            grid.Columns["Notes"].MinimumWidth = 60;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        public void dgvReadyStoreMovInvoicesSettings(ref PercentageDataGrid dgvInvoices)
        {
            //MovementInvoicesDataGrid.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoices.Columns["CreateUserID"].Visible = false;
            dgvInvoices.Columns["CreateDateTime"].Visible = false;
            dgvInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientSectorID"].Visible = false;
            dgvInvoices.Columns["PersonID"].Visible = false;
            dgvInvoices.Columns["StoreKeeperID"].Visible = false;
            dgvInvoices.Columns["SellerID"].Visible = false;
            dgvInvoices.Columns["ClientID"].Visible = false;
            dgvInvoices.Columns["ClientName"].Visible = false;

            dgvInvoices.Columns["DateTime"].HeaderText = "Дата перемещения";
            dgvInvoices.Columns["SellerStoreAlloc"].HeaderText = "Откуда";
            dgvInvoices.Columns["RecipientStoreAlloc"].HeaderText = "Куда";
            dgvInvoices.Columns["Sector"].HeaderText = "Участок";
            dgvInvoices.Columns["PersonName"].HeaderText = "МОЛ";
            dgvInvoices.Columns["StoreKeeper"].HeaderText = "Кладовщик";
            dgvInvoices.Columns["Notes"].HeaderText = "Примечание";

            dgvInvoices.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvInvoices.Columns["DateTime"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["SellerStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["RecipientStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Sector"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["PersonName"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["StoreKeeper"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["MovementInvoiceID"].DisplayIndex = DisplayIndex++;

            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 100;
            dgvInvoices.Columns["DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["DateTime"].MinimumWidth = 100;
            dgvInvoices.Columns["SellerStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["SellerStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["RecipientStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["RecipientStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["Sector"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["Sector"].MinimumWidth = 100;
            dgvInvoices.Columns["PersonName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["PersonName"].MinimumWidth = 100;
            dgvInvoices.Columns["StoreKeeper"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["StoreKeeper"].MinimumWidth = 100;
            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 100;
        }

        public void dgvReadyStoreMovInvoiceItemsSettings(ref PercentageDataGrid dgvInvoiceItems)
        {
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceColorsColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoicePatinaColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceCoversColumn);

            dgvInvoiceItems.AutoGenerateColumns = false;

            if (dgvInvoiceItems.Columns.Contains("CreateUserID"))
                dgvInvoiceItems.Columns["CreateUserID"].Visible = false;
            dgvInvoiceItems.Columns["ReadyStoreID"].Visible = false;
            dgvInvoiceItems.Columns["ColorID"].Visible = false;
            dgvInvoiceItems.Columns["CoverID"].Visible = false;
            dgvInvoiceItems.Columns["PatinaID"].Visible = false;
            dgvInvoiceItems.Columns["StoreItemID"].Visible = false;
            dgvInvoiceItems.Columns["FactoryID"].Visible = false;
            dgvInvoiceItems.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoiceItems.Columns["DecorAssignmentID"].Visible = false;

            dgvInvoiceItems.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvInvoiceItems.Columns["SellerCode"].HeaderText = "Код поставщика";
            dgvInvoiceItems.Columns["Length"].HeaderText = "Длина, мм";
            dgvInvoiceItems.Columns["Width"].HeaderText = "Ширина, мм";
            dgvInvoiceItems.Columns["Height"].HeaderText = "Высота, мм";
            dgvInvoiceItems.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvInvoiceItems.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvInvoiceItems.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvInvoiceItems.Columns["Weight"].HeaderText = "Вес, кг";
            dgvInvoiceItems.Columns["Capacity"].HeaderText = "Емкость, л";
            dgvInvoiceItems.Columns["Notes"].HeaderText = "Примечание";
            dgvInvoiceItems.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            dgvInvoiceItems.Columns["CurrentCount"].HeaderText = "Остаток";
            dgvInvoiceItems.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            dgvInvoiceItems.Columns["Price"].HeaderText = "Цена";
            //dgvInvoiceItems.Columns["VAT"].HeaderText = "НДС";
            //dgvInvoiceItems.Columns["Cost"].HeaderText = "Сумма";
            //dgvInvoiceItems.Columns["VATCost"].HeaderText = "Сумма с НДС";
            //dgvInvoiceItems.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            dgvInvoiceItems.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Admission"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvInvoiceItems.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].MinimumWidth = 70;
            dgvInvoiceItems.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CoversColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Length"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Height"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Width"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Admission"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Diameter"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Capacity"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Weight"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["InvoiceCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CurrentCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Notes"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;
        }

        public void dgvWriteOffStoreSettings(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(MainStoreManager.ColorsColumn);
            grid.Columns.Add(MainStoreManager.PatinaColumn);
            grid.Columns.Add(MainStoreManager.CoversColumn);
            grid.Columns.Add(MainStoreManager.CurrencyColumn);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            grid.Columns["Price"].DefaultCellStyle.Format = "N";
            grid.Columns["Price"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["VAT"].DefaultCellStyle.Format = "N";
            grid.Columns["VAT"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Cost"].DefaultCellStyle.Format = "N";
            grid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["VATCost"].DefaultCellStyle.Format = "N";
            grid.Columns["VATCost"].DefaultCellStyle.FormatProvider = nfi1;

            grid.Columns["Thickness"].DefaultCellStyle.Format = "N";
            grid.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Length"].DefaultCellStyle.Format = "N";
            grid.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Height"].DefaultCellStyle.Format = "N";
            grid.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Width"].DefaultCellStyle.Format = "N";
            grid.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Admission"].DefaultCellStyle.Format = "N";
            grid.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Diameter"].DefaultCellStyle.Format = "N";
            grid.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Capacity"].DefaultCellStyle.Format = "N";
            grid.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            grid.Columns["Weight"].DefaultCellStyle.Format = "N";
            grid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            grid.Columns["InvoiceCount"].Visible = false;
            grid.Columns["WriteOffStoreID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["CoverID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["StoreItemID"].Visible = false;
            grid.Columns["CurrencyTypeID"].Visible = false;
            grid.Columns["FactoryID"].Visible = false;
            grid.Columns["EndMonthCount"].Visible = false;

            grid.Columns["StoreItemColumn"].HeaderText = "Наименование";
            grid.Columns["SellerCode"].HeaderText = "Код поставщика";
            grid.Columns["Length"].HeaderText = "Длина, мм";
            grid.Columns["Width"].HeaderText = "Ширина, мм";
            grid.Columns["Height"].HeaderText = "Высота, мм";
            grid.Columns["Thickness"].HeaderText = "Толщина, мм";
            grid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            grid.Columns["Admission"].HeaderText = "Допуск, мм";
            grid.Columns["Weight"].HeaderText = "Вес, кг";
            grid.Columns["Capacity"].HeaderText = "Емкость, л";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Price"].HeaderText = "Цена";
            grid.Columns["VAT"].HeaderText = "НДС";
            grid.Columns["Cost"].HeaderText = "Сумма";
            grid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            grid.Columns["StartMonthCount"].HeaderText = "ОСТн";
            grid.Columns["MonthInvoiceCount"].HeaderText = "Приход";
            grid.Columns["ExpenseCount"].HeaderText = "Расход";
            grid.Columns["SellingCount"].HeaderText = "Реализация";
            //grid.Columns["EndMonthCount"].HeaderText = "ОСТк";
            grid.Columns["CurrentCount"].HeaderText = "ОСТк";
            grid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";

            grid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            grid.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            grid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            grid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            grid.Columns["StartMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["MonthInvoiceCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["ExpenseCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["SellingCount"].DisplayIndex = DisplayIndex++;
            //grid.Columns["EndMonthCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Price"].DisplayIndex = DisplayIndex++;
            grid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            grid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            grid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CreateDateTime"].Width = 120;
            grid.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["StoreItemColumn"].MinimumWidth = 100;
            grid.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["SellerCode"].MinimumWidth = 70;
            grid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ColorsColumn"].MinimumWidth = 100;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].MinimumWidth = 100;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CoversColumn"].MinimumWidth = 100;

            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length"].MinimumWidth = 60;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness"].MinimumWidth = 60;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].MinimumWidth = 60;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].MinimumWidth = 60;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Admission"].MinimumWidth = 60;
            grid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Diameter"].MinimumWidth = 60;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Capacity"].MinimumWidth = 60;
            grid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Weight"].MinimumWidth = 60;
            grid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Price"].MinimumWidth = 60;
            grid.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MonthInvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MonthInvoiceCount"].Width = 90;
            grid.Columns["StartMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StartMonthCount"].Width = 90;
            //grid.Columns["EndMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //grid.Columns["EndMonthCount"].Width = 90;
            grid.Columns["ExpenseCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpenseCount"].Width = 90;
            grid.Columns["SellingCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["SellingCount"].Width = 90;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 90;
            grid.Columns["VAT"].MinimumWidth = 60;
            grid.Columns["VAT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Cost"].MinimumWidth = 60;
            grid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["VATCost"].MinimumWidth = 130;
            grid.Columns["VATCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CurrencyColumn"].MinimumWidth = 60;
            grid.Columns["CurrencyColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 60;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        public void dgvWriteOffStoreMovInvoicesSettings(ref PercentageDataGrid dgvInvoices)
        {
            //MovementInvoicesDataGrid.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoices.Columns["TypeCreation"].Visible = false;
            dgvInvoices.Columns["CreateUserID"].Visible = false;
            dgvInvoices.Columns["CreateDateTime"].Visible = false;
            dgvInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientStoreAllocID"].Visible = false;
            dgvInvoices.Columns["RecipientSectorID"].Visible = false;
            dgvInvoices.Columns["PersonID"].Visible = false;
            dgvInvoices.Columns["StoreKeeperID"].Visible = false;
            dgvInvoices.Columns["SellerID"].Visible = false;
            dgvInvoices.Columns["ClientID"].Visible = false;
            dgvInvoices.Columns["ClientName"].Visible = false;

            dgvInvoices.Columns["MovementInvoiceID"].HeaderText = "№п\\п";
            dgvInvoices.Columns["DateTime"].HeaderText = "Дата перемещения";
            dgvInvoices.Columns["SellerStoreAlloc"].HeaderText = "Откуда";
            dgvInvoices.Columns["RecipientStoreAlloc"].HeaderText = "Куда";
            dgvInvoices.Columns["Sector"].HeaderText = "Участок";
            dgvInvoices.Columns["PersonName"].HeaderText = "МОЛ";
            dgvInvoices.Columns["StoreKeeper"].HeaderText = "Кладовщик";
            dgvInvoices.Columns["Notes"].HeaderText = "Примечание";

            dgvInvoices.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvInvoices.Columns["DateTime"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["SellerStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["RecipientStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Sector"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["PersonName"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["StoreKeeper"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvInvoices.Columns["MovementInvoiceID"].DisplayIndex = DisplayIndex++;

            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 50;
            dgvInvoices.Columns["DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["DateTime"].MinimumWidth = 100;
            dgvInvoices.Columns["SellerStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["SellerStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["RecipientStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["RecipientStoreAlloc"].MinimumWidth = 100;
            dgvInvoices.Columns["Sector"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["Sector"].MinimumWidth = 100;
            dgvInvoices.Columns["PersonName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["PersonName"].MinimumWidth = 100;
            dgvInvoices.Columns["StoreKeeper"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["StoreKeeper"].MinimumWidth = 100;
            dgvInvoices.Columns["MovementInvoiceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoices.Columns["MovementInvoiceID"].MinimumWidth = 100;
        }

        public void dgvWriteOffStoreMovInvoiceItemsSettings(ref PercentageDataGrid dgvInvoiceItems)
        {
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceColorsColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoicePatinaColumn);
            dgvInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceCoversColumn);

            dgvInvoiceItems.AutoGenerateColumns = false;

            if (dgvInvoiceItems.Columns.Contains("CreateUserID"))
                dgvInvoiceItems.Columns["CreateUserID"].Visible = false;
            dgvInvoiceItems.Columns["WriteOffStoreID"].Visible = false;
            dgvInvoiceItems.Columns["ColorID"].Visible = false;
            dgvInvoiceItems.Columns["CoverID"].Visible = false;
            dgvInvoiceItems.Columns["PatinaID"].Visible = false;
            dgvInvoiceItems.Columns["StoreItemID"].Visible = false;
            dgvInvoiceItems.Columns["CurrencyTypeID"].Visible = false;
            dgvInvoiceItems.Columns["FactoryID"].Visible = false;
            dgvInvoiceItems.Columns["MovementInvoiceID"].Visible = false;
            dgvInvoiceItems.Columns["DecorAssignmentID"].Visible = false;
            dgvInvoiceItems.Columns["Price"].Visible = false;
            dgvInvoiceItems.Columns["VAT"].Visible = false;
            dgvInvoiceItems.Columns["Cost"].Visible = false;
            dgvInvoiceItems.Columns["VATCost"].Visible = false;

            dgvInvoiceItems.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvInvoiceItems.Columns["SellerCode"].HeaderText = "Код поставщика";
            dgvInvoiceItems.Columns["Length"].HeaderText = "Длина, мм";
            dgvInvoiceItems.Columns["Width"].HeaderText = "Ширина, мм";
            dgvInvoiceItems.Columns["Height"].HeaderText = "Высота, мм";
            dgvInvoiceItems.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvInvoiceItems.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvInvoiceItems.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvInvoiceItems.Columns["Weight"].HeaderText = "Вес, кг";
            dgvInvoiceItems.Columns["Capacity"].HeaderText = "Емкость, л";
            dgvInvoiceItems.Columns["Notes"].HeaderText = "Примечание";
            dgvInvoiceItems.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            dgvInvoiceItems.Columns["CurrentCount"].HeaderText = "Остаток";
            dgvInvoiceItems.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            //dgvInvoiceItems.Columns["Price"].HeaderText = "Цена";
            //dgvInvoiceItems.Columns["VAT"].HeaderText = "НДС";
            //dgvInvoiceItems.Columns["Cost"].HeaderText = "Сумма";
            //dgvInvoiceItems.Columns["VATCost"].HeaderText = "Сумма с НДС";
            //dgvInvoiceItems.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            dgvInvoiceItems.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Admission"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            dgvInvoiceItems.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvInvoiceItems.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["SellerCode"].MinimumWidth = 70;
            dgvInvoiceItems.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CoversColumn"].MinimumWidth = 100;
            dgvInvoiceItems.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Length"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Height"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Width"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Admission"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Diameter"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Capacity"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Weight"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["InvoiceCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["CurrentCount"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvInvoiceItems.Columns["Notes"].MinimumWidth = 60;
            dgvInvoiceItems.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvInvoiceItems.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;
        }

        public void PersonalStoreGridSettings()
        {
            dgvPersGroups.Columns["TechStoreGroupID"].Visible = false;

            dgvPersSubGroups.Columns["TechStoreGroupID"].Visible = false;
            dgvPersSubGroups.Columns["TechStoreSubGroupID"].Visible = false;
            dgvPersSubGroups.Columns["Notes"].Visible = false;
            dgvPersSubGroups.Columns["Notes1"].Visible = false;
            dgvPersSubGroups.Columns["Notes2"].Visible = false;

            dgvPersStore.Columns.Add(MainStoreManager.ColorsColumn);
            dgvPersStore.Columns.Add(MainStoreManager.PatinaColumn);
            dgvPersStore.Columns.Add(MainStoreManager.CoversColumn);
            dgvPersStore.Columns.Add(MainStoreManager.CurrencyColumn);

            foreach (DataGridViewColumn Column in dgvPersStore.Columns)
            {
                Column.ReadOnly = true; ;
            }

            if (dgvPersStore.Columns.Contains("CreateUserID"))
                dgvPersStore.Columns["CreateUserID"].Visible = false;
            dgvPersStore.Columns["PersonalStoreID"].Visible = false;
            dgvPersStore.Columns["ColorID"].Visible = false;
            dgvPersStore.Columns["PatinaID"].Visible = false;
            dgvPersStore.Columns["StoreItemID"].Visible = false;
            dgvPersStore.Columns["FactoryID"].Visible = false;
            dgvPersStore.Columns["EndMonthCount"].Visible = false;
            dgvPersStore.Columns["PriceEUR"].Visible = false;

            dgvPersStore.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvPersStore.Columns["SellerCode"].HeaderText = "Код поставщика";
            dgvPersStore.Columns["Length"].HeaderText = "Длина, мм";
            dgvPersStore.Columns["Width"].HeaderText = "Ширина, мм";
            dgvPersStore.Columns["Height"].HeaderText = "Высота, мм";
            dgvPersStore.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvPersStore.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvPersStore.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvPersStore.Columns["Weight"].HeaderText = "Вес, кг";
            dgvPersStore.Columns["Capacity"].HeaderText = "Емкость, л";
            dgvPersStore.Columns["Notes"].HeaderText = "Примечание";
            dgvPersStore.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            dgvPersStore.Columns["CurrentCount"].HeaderText = "ОСТк";
            dgvPersStore.Columns["Price"].HeaderText = "Цена";
            dgvPersStore.Columns["VAT"].HeaderText = "НДС";
            dgvPersStore.Columns["Cost"].HeaderText = "Сумма";
            dgvPersStore.Columns["VATCost"].HeaderText = "Сумма с НДС";
            dgvPersStore.Columns["StartMonthCount"].HeaderText = "ОСТн";
            dgvPersStore.Columns["MonthInvoiceCount"].HeaderText = "Приход";
            dgvPersStore.Columns["ExpenseCount"].HeaderText = "Расход";
            dgvPersStore.Columns["SellingCount"].HeaderText = "Реализация";
            //dgvPersStore.Columns["EndMonthCount"].HeaderText = "ОСТк";

            dgvPersStore.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvPersStore.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Admission"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["StartMonthCount"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["MonthInvoiceCount"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["ExpenseCount"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["SellingCount"].DisplayIndex = DisplayIndex++;
            //dgvPersStore.Columns["EndMonthCount"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Price"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Cost"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["VAT"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            dgvPersStore.Columns["Notes"].DisplayIndex = DisplayIndex++;

            dgvPersStore.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvPersStore.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["SellerCode"].MinimumWidth = 70;
            dgvPersStore.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvPersStore.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvPersStore.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["CoversColumn"].MinimumWidth = 100;
            dgvPersStore.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPersStore.Columns["MonthInvoiceCount"].MinimumWidth = 60;
            dgvPersStore.Columns["MonthInvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["StartMonthCount"].MinimumWidth = 60;
            dgvPersStore.Columns["StartMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvPersStore.Columns["EndMonthCount"].MinimumWidth = 60;
            //dgvPersStore.Columns["EndMonthCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["ExpenseCount"].MinimumWidth = 60;
            dgvPersStore.Columns["ExpenseCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["SellingCount"].MinimumWidth = 60;
            dgvPersStore.Columns["SellingCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["CurrentCount"].MinimumWidth = 60;
            dgvPersStore.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPersStore.Columns["Thickness"].MinimumWidth = 60;
            dgvPersStore.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Length"].MinimumWidth = 60;
            dgvPersStore.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Thickness"].MinimumWidth = 60;
            dgvPersStore.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Height"].MinimumWidth = 60;
            dgvPersStore.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Width"].MinimumWidth = 60;
            dgvPersStore.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Admission"].MinimumWidth = 60;
            dgvPersStore.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Diameter"].MinimumWidth = 60;
            dgvPersStore.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Capacity"].MinimumWidth = 60;
            dgvPersStore.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Weight"].MinimumWidth = 60;
            dgvPersStore.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Price"].MinimumWidth = 60;
            dgvPersStore.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["VAT"].MinimumWidth = 60;
            dgvPersStore.Columns["VAT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Cost"].MinimumWidth = 60;
            dgvPersStore.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["VATCost"].MinimumWidth = 130;
            dgvPersStore.Columns["VATCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["CurrencyColumn"].MinimumWidth = 60;
            dgvPersStore.Columns["CurrencyColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersStore.Columns["Notes"].MinimumWidth = 60;
            dgvPersStore.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvPersStore.Columns["Price"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Price"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["VAT"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["VAT"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Cost"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["VATCost"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["VATCost"].DefaultCellStyle.FormatProvider = nfi1;

            dgvPersStore.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersStore.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvPersStore.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;
        }

        public void PersonalInvoiceGridSettings()
        {
            //PInvoicesDataGrid.Columns["MovementInvoiceID"].Visible = false;
            dgvPersInvoices.Columns["SellerStoreAllocID"].Visible = false;
            dgvPersInvoices.Columns["RecipientStoreAllocID"].Visible = false;
            dgvPersInvoices.Columns["RecipientSectorID"].Visible = false;
            dgvPersInvoices.Columns["PersonID"].Visible = false;
            dgvPersInvoices.Columns["StoreKeeperID"].Visible = false;
            dgvPersInvoices.Columns["ClientName"].Visible = false;
            dgvPersInvoices.Columns["SellerID"].Visible = false;
            dgvPersInvoices.Columns["ClientID"].Visible = false;
            dgvPersInvoices.Columns["Sector"].Visible = false;

            dgvPersInvoices.Columns["DateTime"].HeaderText = "Дата перемещения";
            dgvPersInvoices.Columns["SellerStoreAlloc"].HeaderText = "Откуда";
            dgvPersInvoices.Columns["RecipientStoreAlloc"].HeaderText = "Куда";
            dgvPersInvoices.Columns["Sector"].HeaderText = "Участок";
            dgvPersInvoices.Columns["PersonName"].HeaderText = "МОЛ";
            dgvPersInvoices.Columns["StoreKeeper"].HeaderText = "Кладовщик";
            dgvPersInvoices.Columns["ClientName"].HeaderText = "Клиент";
            dgvPersInvoices.Columns["Notes"].HeaderText = "Примечание";

            dgvPersInvoices.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvPersInvoices.Columns["DateTime"].DisplayIndex = DisplayIndex++;
            dgvPersInvoices.Columns["SellerStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvPersInvoices.Columns["RecipientStoreAlloc"].DisplayIndex = DisplayIndex++;
            dgvPersInvoices.Columns["Sector"].DisplayIndex = DisplayIndex++;
            dgvPersInvoices.Columns["PersonName"].DisplayIndex = DisplayIndex++;
            dgvPersInvoices.Columns["StoreKeeper"].DisplayIndex = DisplayIndex++;
            dgvPersInvoices.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            dgvPersInvoices.Columns["Notes"].DisplayIndex = DisplayIndex++;

            dgvPersInvoices.Columns["DateTime"].MinimumWidth = 100;
            dgvPersInvoices.Columns["DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoices.Columns["SellerStoreAlloc"].MinimumWidth = 100;
            dgvPersInvoices.Columns["SellerStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoices.Columns["RecipientStoreAlloc"].MinimumWidth = 100;
            dgvPersInvoices.Columns["RecipientStoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoices.Columns["Sector"].MinimumWidth = 100;
            dgvPersInvoices.Columns["Sector"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoices.Columns["PersonName"].MinimumWidth = 100;
            dgvPersInvoices.Columns["PersonName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoices.Columns["ClientName"].MinimumWidth = 100;
            dgvPersInvoices.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoices.Columns["StoreKeeper"].MinimumWidth = 100;
            dgvPersInvoices.Columns["StoreKeeper"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPersInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceColorsColumn);
            dgvPersInvoiceItems.Columns.Add(MainStoreManager.MovInvoicePatinaColumn);
            dgvPersInvoiceItems.Columns.Add(MainStoreManager.MovInvoiceCoversColumn);
            dgvPersInvoiceItems.Columns.Add(MainStoreManager.ManufacturerColumn);

            dgvPersInvoiceItems.AutoGenerateColumns = false;

            dgvPersInvoiceItems.Columns["PersonalStoreID"].Visible = false;
            dgvPersInvoiceItems.Columns["ColorID"].Visible = false;
            dgvPersInvoiceItems.Columns["CoverID"].Visible = false;
            dgvPersInvoiceItems.Columns["PatinaID"].Visible = false;
            dgvPersInvoiceItems.Columns["StoreItemID"].Visible = false;
            dgvPersInvoiceItems.Columns["FactoryID"].Visible = false;
            dgvPersInvoiceItems.Columns["MovementInvoiceID"].Visible = false;
            dgvPersInvoiceItems.Columns["PriceEUR"].Visible = false;

            dgvPersInvoiceItems.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvPersInvoiceItems.Columns["SellerCode"].HeaderText = "Код поставщика";
            dgvPersInvoiceItems.Columns["Length"].HeaderText = "Длина, мм";
            dgvPersInvoiceItems.Columns["Width"].HeaderText = "Ширина, мм";
            dgvPersInvoiceItems.Columns["Height"].HeaderText = "Высота, мм";
            dgvPersInvoiceItems.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvPersInvoiceItems.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvPersInvoiceItems.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvPersInvoiceItems.Columns["Weight"].HeaderText = "Вес, кг";
            dgvPersInvoiceItems.Columns["Capacity"].HeaderText = "Емкость, л";
            dgvPersInvoiceItems.Columns["Notes"].HeaderText = "Примечание";
            dgvPersInvoiceItems.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            dgvPersInvoiceItems.Columns["CurrentCount"].HeaderText = "Остаток";
            dgvPersInvoiceItems.Columns["Price"].HeaderText = "Цена";
            dgvPersInvoiceItems.Columns["VAT"].HeaderText = "НДС";
            dgvPersInvoiceItems.Columns["Cost"].HeaderText = "Сумма";
            dgvPersInvoiceItems.Columns["VATCost"].HeaderText = "Сумма с НДС";

            DisplayIndex = 0;
            dgvPersInvoiceItems.Columns["StoreItemColumn"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Admission"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Price"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Cost"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["VAT"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            //dgvPersInvoiceItems.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            dgvPersInvoiceItems.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvPersInvoiceItems.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvPersInvoiceItems.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["SellerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["SellerCode"].MinimumWidth = 70;
            dgvPersInvoiceItems.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvPersInvoiceItems.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvPersInvoiceItems.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["CoversColumn"].MinimumWidth = 100;
            dgvPersInvoiceItems.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPersInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Length"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Thickness"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Height"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Width"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Admission"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Diameter"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Capacity"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Weight"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["InvoiceCount"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["CurrentCount"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Notes"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Price"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["VAT"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["VAT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["Cost"].MinimumWidth = 60;
            dgvPersInvoiceItems.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPersInvoiceItems.Columns["VATCost"].MinimumWidth = 130;
            dgvPersInvoiceItems.Columns["VATCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvPersInvoiceItems.Columns["CurrencyColumn"].MinimumWidth = 60;
            //dgvPersInvoiceItems.Columns["CurrencyColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPersInvoiceItems.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            dgvPersInvoiceItems.Columns["Price"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Price"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["VAT"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["VAT"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["Cost"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvPersInvoiceItems.Columns["VATCost"].DefaultCellStyle.Format = "N";
            dgvPersInvoiceItems.Columns["VATCost"].DefaultCellStyle.FormatProvider = nfi1;
        }

        private void CheckStoreColumns(bool bItemsGrid, ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }

            if (Grid.Columns.Contains("CreateUserID"))
                Grid.Columns["CreateUserID"].Visible = false;
            if (Grid.Columns.Contains("StartMonthCount"))
                Grid.Columns["StartMonthCount"].Visible = true;
            if (Grid.Columns.Contains("SellingCount"))
                Grid.Columns["SellingCount"].Visible = true;
            if (Grid.Columns.Contains("CurrentCount"))
                Grid.Columns["CurrentCount"].Visible = true;
            if (Grid.Columns.Contains("ExpenseCount"))
                Grid.Columns["ExpenseCount"].Visible = true;
            if (Grid.Columns.Contains("EndMonthCount"))
                Grid.Columns["EndMonthCount"].Visible = false;
            if (Grid.Columns.Contains("MonthInvoiceCount"))
                Grid.Columns["MonthInvoiceCount"].Visible = true;
            Grid.Columns["DecorAssignmentID"].Visible = false;
            Grid.Columns["ManufacturerID"].Visible = false;
            //Grid.Columns["StoreID"].Visible = false;
            Grid.Columns["ColorID"].Visible = false;
            Grid.Columns["CoverID"].Visible = false;
            Grid.Columns["PatinaID"].Visible = false;
            Grid.Columns["StoreItemID"].Visible = false;
            Grid.Columns["CurrencyTypeID"].Visible = false;
            Grid.Columns["FactoryID"].Visible = false;
            Grid.Columns["PurchaseInvoiceID"].Visible = false;
            Grid.Columns["MovementInvoiceID"].Visible = false;
            Grid.Columns["PriceEUR"].Visible = false;
            if (Grid.Columns.Contains("IsArrived"))
                Grid.Columns["IsArrived"].Visible = false;
            if (!bItemsGrid)
                Grid.Columns["InvoiceCount"].Visible = false;
        }

        private void CheckManufactureStoreColumns(bool bItemsGrid, ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }

            if (Grid.Columns.Contains("CreateUserID"))
                Grid.Columns["CreateUserID"].Visible = false;
            if (Grid.Columns.Contains("ReturnedRollerID"))
                Grid.Columns["ReturnedRollerID"].Visible = false;
            if (Grid.Columns.Contains("Produced"))
                Grid.Columns["Produced"].Visible = false;
            if (Grid.Columns.Contains("StartMonthCount"))
                Grid.Columns["StartMonthCount"].Visible = true;
            if (Grid.Columns.Contains("SellingCount"))
                Grid.Columns["SellingCount"].Visible = true;
            if (Grid.Columns.Contains("CurrentCount"))
                Grid.Columns["CurrentCount"].Visible = true;
            if (Grid.Columns.Contains("ExpenseCount"))
                Grid.Columns["ExpenseCount"].Visible = true;
            if (Grid.Columns.Contains("EndMonthCount"))
                Grid.Columns["EndMonthCount"].Visible = false;
            if (Grid.Columns.Contains("MonthInvoiceCount"))
                Grid.Columns["MonthInvoiceCount"].Visible = true;
            Grid.Columns["DecorAssignmentID"].Visible = false;
            //Grid.Columns["ManufactureStoreID"].Visible = false;
            Grid.Columns["ColorID"].Visible = false;
            Grid.Columns["CoverID"].Visible = false;
            Grid.Columns["PatinaID"].Visible = false;
            //Grid.Columns["StoreItemID"].Visible = false;
            Grid.Columns["FactoryID"].Visible = false;
            Grid.Columns["MovementInvoiceID"].Visible = false;
            if (!bItemsGrid)
                Grid.Columns["InvoiceCount"].Visible = false;
        }

        private void CheckReadyStoreColumns(bool bItemsGrid, ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }

            if (Grid.Columns.Contains("CreateUserID"))
                Grid.Columns["CreateUserID"].Visible = false;
            if (Grid.Columns.Contains("StartMonthCount"))
                Grid.Columns["StartMonthCount"].Visible = true;
            if (Grid.Columns.Contains("SellingCount"))
                Grid.Columns["SellingCount"].Visible = true;
            if (Grid.Columns.Contains("CurrentCount"))
                Grid.Columns["CurrentCount"].Visible = true;
            if (Grid.Columns.Contains("ExpenseCount"))
                Grid.Columns["ExpenseCount"].Visible = true;
            if (Grid.Columns.Contains("EndMonthCount"))
                Grid.Columns["EndMonthCount"].Visible = false;
            if (Grid.Columns.Contains("MonthInvoiceCount"))
                Grid.Columns["MonthInvoiceCount"].Visible = true;
            Grid.Columns["DecorAssignmentID"].Visible = false;
            Grid.Columns["ReadyStoreID"].Visible = false;
            Grid.Columns["ColorID"].Visible = false;
            Grid.Columns["CoverID"].Visible = false;
            Grid.Columns["PatinaID"].Visible = false;
            Grid.Columns["StoreItemID"].Visible = false;
            Grid.Columns["FactoryID"].Visible = false;
            Grid.Columns["MovementInvoiceID"].Visible = false;
            if (!bItemsGrid)
                Grid.Columns["InvoiceCount"].Visible = false;
        }

        private void CheckWriteOffStoreColumns(bool bItemsGrid, ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }

            if (Grid.Columns.Contains("CreateUserID"))
                Grid.Columns["CreateUserID"].Visible = false;
            if (Grid.Columns.Contains("StartMonthCount"))
                Grid.Columns["StartMonthCount"].Visible = true;
            if (Grid.Columns.Contains("SellingCount"))
                Grid.Columns["SellingCount"].Visible = true;
            if (Grid.Columns.Contains("CurrentCount"))
                Grid.Columns["CurrentCount"].Visible = true;
            if (Grid.Columns.Contains("ExpenseCount"))
                Grid.Columns["ExpenseCount"].Visible = true;
            if (Grid.Columns.Contains("EndMonthCount"))
                Grid.Columns["EndMonthCount"].Visible = false;
            if (Grid.Columns.Contains("MonthInvoiceCount"))
                Grid.Columns["MonthInvoiceCount"].Visible = true;
            Grid.Columns["DecorAssignmentID"].Visible = false;
            Grid.Columns["WriteOffStoreID"].Visible = false;
            Grid.Columns["ColorID"].Visible = false;
            Grid.Columns["CoverID"].Visible = false;
            Grid.Columns["PatinaID"].Visible = false;
            Grid.Columns["StoreItemID"].Visible = false;
            Grid.Columns["CurrencyTypeID"].Visible = false;
            Grid.Columns["FactoryID"].Visible = false;
            Grid.Columns["MovementInvoiceID"].Visible = false;
            if (!bItemsGrid)
                Grid.Columns["InvoiceCount"].Visible = false;
        }

        private void CheckPersonalStoreColumns(bool bItemsGrid, ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Column.Name == "StartMonthCount"
                        || Column.Name == "SellingCount"
                        || Column.Name == "CurrentCount"
                        || Column.Name == "ExpenseCount"
                        //|| Column.Name == "EndMonthCount"
                        || Column.Name == "MonthInvoiceCount")
                        continue;
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }

            if (Grid.Columns.Contains("CreateUserID"))
                Grid.Columns["CreateUserID"].Visible = false;
            if (Grid.Columns.Contains("IsArrived"))
                Grid.Columns["IsArrived"].Visible = false;
            Grid.Columns["PersonalStoreID"].Visible = false;
            Grid.Columns["ColorID"].Visible = false;
            Grid.Columns["CoverID"].Visible = false;
            Grid.Columns["PatinaID"].Visible = false;
            Grid.Columns["StoreItemID"].Visible = false;
            Grid.Columns["FactoryID"].Visible = false;
            Grid.Columns["MovementInvoiceID"].Visible = false;
            Grid.Columns["CurrencyTypeID"].Visible = false;
            Grid.Columns["PriceEUR"].Visible = false;
            if (!bItemsGrid)
                Grid.Columns["InvoiceCount"].Visible = false;
        }

        private void CheckInvoicesColumns(ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }
            if (Grid.Columns.Contains("CreateUserID"))
                Grid.Columns["CreateUserID"].Visible = false;
            Grid.Columns["TypeCreation"].Visible = false;
            Grid.Columns["CreateUserID"].Visible = false;
            Grid.Columns["CreateDateTime"].Visible = false;
            Grid.Columns["SellerStoreAllocID"].Visible = false;
            Grid.Columns["SellerStoreAllocID"].Visible = false;
            Grid.Columns["RecipientStoreAllocID"].Visible = false;
            Grid.Columns["RecipientSectorID"].Visible = false;
            Grid.Columns["PersonID"].Visible = false;
            Grid.Columns["StoreKeeperID"].Visible = false;
            Grid.Columns["SellerID"].Visible = false;
            Grid.Columns["ClientID"].Visible = false;
            Grid.Columns["ClientName"].Visible = false;
        }

        private void dgvMainStoreGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (MainStoreManager == null)
                return;

            int TechStoreGroupID = 0;
            if (dgvMainStoreGroups.SelectedRows.Count != 0 && dgvMainStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvMainStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);
            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                MainStoreManager.UpdateStore(FactoryID, FilterDate, TechStoreGroupID);
                MainStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                MainStoreManager.UpdateStore(FactoryID, FilterDate, TechStoreGroupID);
                MainStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
            }
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewArrivalForm NewArrivalForm = new NewArrivalForm();

            TopForm = NewArrivalForm;

            NewArrivalForm.ShowDialog();

            NewArrivalForm.Close();
            NewArrivalForm.Dispose();

            TopForm = null;
            FilterMainStore();
            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //NeedSplash = false;
            //FilterMainStore();
            //MainStoreManager.MoveToLastInvoice();
            //NeedSplash = true;
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void ChangeItemButton_Click(object sender, EventArgs e)
        {
            int PurchaseInvoiceID = 0;
            if (dgvPurchInvoices.SelectedRows.Count != 0 && dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
                PurchaseInvoiceID = Convert.ToInt32(dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            if (PurchaseInvoiceID == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewArrivalForm NewArrivalForm = new NewArrivalForm(PurchaseInvoiceID);

            TopForm = NewArrivalForm;

            NewArrivalForm.ShowDialog();

            NewArrivalForm.Close();
            NewArrivalForm.Dispose();

            TopForm = null;
            FilterMainStore();
            MainStoreManager.MoveToInvoice(PurchaseInvoiceID);
            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //FilterMainStore();
            //MainStoreManager.MoveToInvoice(PurchaseInvoiceID);
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (MainStoreManager == null)
                return;

            pnlPurchInvoicesMenu.Visible = false;
            pnlMovementInvoicesMenu.Visible = false;
            pnlMainStoreMenu.Visible = false;

            if (kryptonCheckSet1.CheckedButton == cbtnMainStoreProduct)
            {
                pnlMainStoreProduct.BringToFront();
                pnlMainStoreInventoryTools.BringToFront();
                pnlMainStoreMenu.Visible = MenuButton.Checked;
            }

            if (kryptonCheckSet1.CheckedButton == cbtnPurchaseInvoices)
            {
                pnlMainStorePurchInvoices.BringToFront();
                pnlMainStorePurchInvoiceTools.BringToFront();

                pnlPurchInvoicesMenu.Visible = MenuButton.Checked;
            }

            if (kryptonCheckSet1.CheckedButton == cbtnMovementInvoices)
            {
                pnlMainStoreMovInvoices.BringToFront();
                pnlMainStoreMovInvoiceTools.BringToFront();
                pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
            }
        }

        private void dgvPurchInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (MainStoreManager == null)
                return;
            int PurchaseInvoiceID = 0;
            if (dgvPurchInvoices.SelectedRows.Count != 0 && dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
                PurchaseInvoiceID = Convert.ToInt32(dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            MainStoreManager.FilterByInvoice(PurchaseInvoiceID, FactoryID);

            CheckStoreColumns(true, ref dgvPurchInvoiceItems);
        }

        //private void MegaFilter()
        //{
        //    FactoryID = 0;

        //    if (ProfilCheckButton.Checked && !TPSCheckButton.Checked)
        //        FactoryID = 1;
        //    if (!ProfilCheckButton.Checked && TPSCheckButton.Checked)
        //        FactoryID = 2;
        //    if (!ProfilCheckButton.Checked && !TPSCheckButton.Checked)
        //        FactoryID = -1;

        //    DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
        //    DateTime PFilterDate = new DateTime(Convert.ToInt32(cbxPYears.SelectedValue), Convert.ToInt32(cbxPMonths.SelectedValue), 1);

        //    if (NeedSplash)
        //    {
        //        Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
        //        T.Start();

        //        while (!SplashWindow.bSmallCreated) ;

        //        NeedSplash = false;
        //        MainStoreManager.GetStoreCount(FilterDate);
        //        PersonalStorageManager.GetStoreCount(PFilterDate);

        //        MainStoreManager.FilterGroups(FactoryID);
        //        MainStoreManager.FilterStore(FactoryID, MainStoreManager.CurrentSubGroup, FilterDate);
        //        MainStoreManager.FillStoreCount();
        //        MainStoreManager.ShowNoEmptyStoreItems();

        //        MainStoreManager.FilterInvoices(
        //            cbxInvoiceDateFilter.Checked, cbxInvoiceSeller.Checked,
        //            dtpInvoiceFilterFrom.Value, dtpInvoiceFilterTo.Value,
        //            Convert.ToInt32(cmbInvoiceSellers.SelectedValue), FactoryID);
        //        MainStoreManager.FilterMovementInvoices(
        //            cbxMovInvoiceDateFilter.Checked, cbxMovInvoicePerson.Checked, cbxMovInvoiceSeller.Checked, cbxMovInvoiceRecipient.Checked,
        //            dtpMovInvoiceFilterFrom.Value, dtpMovInvoiceFilterTo.Value,
        //            Convert.ToInt32(cmbMovInvoicePersons.SelectedValue), Convert.ToInt32(cmbMovInvoiceSellers.SelectedValue), Convert.ToInt32(cmbMovInvoiceRecipients.SelectedValue), FactoryID);

        //        ManufactureStorageManager.FilterGroups(FactoryID);
        //        ManufactureStorageManager.FilterStore(FactoryID, ManufactureStorageManager.CurrentSubGroup);

        //        ManufactureStorageManager.FilterInvoices(
        //            cbxManInvoiceDateFilter.Checked, cbxManInvoicePerson.Checked, cbxManInvoiceSeller.Checked, cbxManInvoiceRecipient.Checked,
        //            dtpManInvoiceFilterFrom.Value, dtpManInvoiceFilterTo.Value,
        //            Convert.ToInt32(cmbManInvoicePersons.SelectedValue), Convert.ToInt32(cmbManInvoiceSellers.SelectedValue), Convert.ToInt32(cmbManInvoiceRecipients.SelectedValue), FactoryID);

        //        WriteOffStorageManager.FilterGroups(FactoryID);
        //        WriteOffStorageManager.FilterStore(FactoryID, WriteOffStorageManager.CurrentSubGroup);
        //        WriteOffStorageManager.FilterInvoices(FactoryID);

        //        PersonalStorageManager.FilterGroups(FactoryID,
        //             true, cbxPersonalPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
        //             Convert.ToInt32(cmbPersonalPersons.SelectedValue), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
        //        PersonalStorageManager.FilterStore(FactoryID, PersonalStorageManager.CurrentSubGroup,
        //             true, cbxPersonalPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
        //             Convert.ToInt32(cmbPersonalPersons.SelectedValue), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
        //        PersonalStorageManager.FillStoreCount();
        //        PersonalStorageManager.ShowNoEmptyStoreItems();
        //        PersonalStorageManager.FilterInvoices(
        //             true, cbxPersonalPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
        //             Convert.ToInt32(cmbPersonalPersons.SelectedValue), Convert.ToInt32(cmbPersonalSellers.SelectedValue), FactoryID);

        //        NeedSplash = true;
        //        while (SplashWindow.bSmallCreated)
        //            SmallWaitForm.CloseS = true;
        //    }
        //    else
        //    {
        //        MainStoreManager.GetStoreCount(FilterDate);
        //        PersonalStorageManager.GetStoreCount(PFilterDate);

        //        MainStoreManager.FilterGroups(FactoryID);
        //        MainStoreManager.FilterStore(FactoryID, MainStoreManager.CurrentSubGroup, FilterDate);
        //        MainStoreManager.FillStoreCount();
        //        MainStoreManager.ShowNoEmptyStoreItems();

        //        MainStoreManager.FilterInvoices(
        //            cbxInvoiceDateFilter.Checked, cbxInvoiceSeller.Checked,
        //            dtpInvoiceFilterFrom.Value, dtpInvoiceFilterTo.Value,
        //            Convert.ToInt32(cmbInvoiceSellers.SelectedValue), FactoryID);
        //        MainStoreManager.FilterMovementInvoices(
        //            cbxMovInvoiceDateFilter.Checked, cbxMovInvoicePerson.Checked, cbxMovInvoiceSeller.Checked, cbxMovInvoiceRecipient.Checked,
        //            dtpMovInvoiceFilterFrom.Value, dtpMovInvoiceFilterTo.Value,
        //            Convert.ToInt32(cmbMovInvoicePersons.SelectedValue), Convert.ToInt32(cmbMovInvoiceSellers.SelectedValue), Convert.ToInt32(cmbMovInvoiceRecipients.SelectedValue), FactoryID);

        //        ManufactureStorageManager.FilterGroups(FactoryID);
        //        ManufactureStorageManager.FilterStore(FactoryID, ManufactureStorageManager.CurrentSubGroup);

        //        ManufactureStorageManager.FilterInvoices(
        //            cbxManInvoiceDateFilter.Checked, cbxManInvoicePerson.Checked, cbxManInvoiceSeller.Checked, cbxManInvoiceRecipient.Checked,
        //            dtpManInvoiceFilterFrom.Value, dtpManInvoiceFilterTo.Value,
        //            Convert.ToInt32(cmbManInvoicePersons.SelectedValue), Convert.ToInt32(cmbManInvoiceSellers.SelectedValue), Convert.ToInt32(cmbManInvoiceRecipients.SelectedValue), FactoryID);

        //        WriteOffStorageManager.FilterGroups(FactoryID);
        //        WriteOffStorageManager.FilterStore(FactoryID, WriteOffStorageManager.CurrentSubGroup);
        //        WriteOffStorageManager.FilterInvoices(FactoryID);

        //        PersonalStorageManager.FilterGroups(FactoryID,
        //             true, cbxPersonalPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
        //             Convert.ToInt32(cmbPersonalPersons.SelectedValue), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
        //        PersonalStorageManager.FilterStore(FactoryID, PersonalStorageManager.CurrentSubGroup,
        //             true, cbxPersonalPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
        //             Convert.ToInt32(cmbPersonalPersons.SelectedValue), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
        //        PersonalStorageManager.FillStoreCount();
        //        PersonalStorageManager.ShowNoEmptyStoreItems();
        //        PersonalStorageManager.FilterInvoices(
        //             true, cbxPersonalPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
        //             Convert.ToInt32(cmbPersonalPersons.SelectedValue), Convert.ToInt32(cmbPersonalSellers.SelectedValue), FactoryID);
        //    }
        //}

        private void InvoiceToExcelButton_Click(object sender, EventArgs e)
        {
            int PurchaseInvoiceID = 0;
            if (dgvPurchInvoices.SelectedRows.Count != 0 && dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
                PurchaseInvoiceID = Convert.ToInt32(dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            if (PurchaseInvoiceID == -1)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PurchaseInvoiceToExcel ddd = new PurchaseInvoiceToExcel(PurchaseInvoiceID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void FilterMainStore()
        {
            FactoryID = 0;

            if (ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = 1;
            if (!ProfilCheckButton.Checked && TPSCheckButton.Checked)
                FactoryID = 2;
            if (!ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = -1;

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            dgvMainStoreGroups.SelectionChanged -= dgvMainStoreGroups_SelectionChanged;
            dgvMainStoreSubGroups.SelectionChanged -= dgvMainStoreSubGroups_SelectionChanged;
            int TechStoreGroupID = 0;
            if (dgvMainStoreGroups.SelectedRows.Count != 0 && dgvMainStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvMainStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(panel3.Top, panel3.Left,
                                                   panel3.Height, panel3.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                NeedSplash = false;
                MainStoreManager.GetStoreCount(FilterDate);

                MainStoreManager.FilterGroups(FactoryID);
                MainStoreManager.MoveToTechStoreGroup(TechStoreGroupID);

                int TechStoreSubGroupID = 0;
                if (dgvMainStoreSubGroups.SelectedRows.Count != 0 && dgvMainStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvMainStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                MainStoreManager.UpdateStore(FactoryID, FilterDate, TechStoreGroupID);
                MainStoreManager.FillStoreCount(FilterDate);
                //MainStoreManager.ShowNoEmptyStoreItems();

                MainStoreManager.FilterInvoices(
                    cbxInvoiceDateFilter.Checked, cbxInvoiceSeller.Checked,
                    dtpInvoiceFilterFrom.Value, dtpInvoiceFilterTo.Value,
                    Convert.ToInt32(cmbInvoiceSellers.SelectedValue), FactoryID);
                MainStoreManager.FilterMovementInvoices(
                    cbxMovInvoiceDateFilter.Checked, cbxMovInvoicePerson.Checked, cbxMovInvoiceSeller.Checked, cbxMovInvoiceRecipient.Checked,
                    dtpMovInvoiceFilterFrom.Value, dtpMovInvoiceFilterTo.Value,
                    Convert.ToInt32(cmbMovInvoicePersons.SelectedValue), Convert.ToInt32(cmbMovInvoiceSellers.SelectedValue), Convert.ToInt32(cmbMovInvoiceRecipients.SelectedValue), FactoryID);
                CheckInvoicesColumns(ref dgvMovInvoices);
            }
            else
            {
                MainStoreManager.GetStoreCount(FilterDate);
                MainStoreManager.FilterGroups(FactoryID);
                MainStoreManager.MoveToTechStoreGroup(TechStoreGroupID);

                int TechStoreSubGroupID = 0;
                if (dgvMainStoreSubGroups.SelectedRows.Count != 0 && dgvMainStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvMainStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                MainStoreManager.UpdateStore(FactoryID, FilterDate, TechStoreGroupID);
                MainStoreManager.FillStoreCount(FilterDate);
                //MainStoreManager.ShowNoEmptyStoreItems();

                MainStoreManager.FilterInvoices(
                    cbxInvoiceDateFilter.Checked, cbxInvoiceSeller.Checked,
                    dtpInvoiceFilterFrom.Value, dtpInvoiceFilterTo.Value,
                    Convert.ToInt32(cmbInvoiceSellers.SelectedValue), FactoryID);
                MainStoreManager.FilterMovementInvoices(
                    cbxMovInvoiceDateFilter.Checked, cbxMovInvoicePerson.Checked, cbxMovInvoiceSeller.Checked, cbxMovInvoiceRecipient.Checked,
                    dtpMovInvoiceFilterFrom.Value, dtpMovInvoiceFilterTo.Value,
                    Convert.ToInt32(cmbMovInvoicePersons.SelectedValue), Convert.ToInt32(cmbMovInvoiceSellers.SelectedValue), Convert.ToInt32(cmbMovInvoiceRecipients.SelectedValue), FactoryID);
                CheckInvoicesColumns(ref dgvMovInvoices);
            }
            dgvMainStoreSubGroups.SelectionChanged += dgvMainStoreSubGroups_SelectionChanged;
            dgvMainStoreGroups.SelectionChanged += dgvMainStoreGroups_SelectionChanged;
            dgvMainStoreGroups_SelectionChanged(null, null);
            NeedSplash = true;
            bC = true;
        }

        private void FilterManufactureStore()
        {
            FactoryID = 0;

            if (ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = 1;
            if (!ProfilCheckButton.Checked && TPSCheckButton.Checked)
                FactoryID = 2;
            if (!ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = -1;

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);

            int TechStoreGroupID = 0;
            if (dgvManufactreStoreGroups.SelectedRows.Count != 0 && dgvManufactreStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvManufactreStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(panel3.Top, panel3.Left,
                                                   panel3.Height, panel3.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                NeedSplash = false;
                ManufactureStoreManager.GetStoreCount(FilterDate);
                ManufactureStoreManager.FilterGroups(FactoryID);
                ManufactureStoreManager.MoveToTechStoreGroup(TechStoreGroupID);
                int TechStoreSubGroupID = 0;
                if (dgvManufactreStoreSubGroups.SelectedRows.Count != 0 && dgvManufactreStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvManufactreStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                ManufactureStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ManufactureStoreManager.FillStoreCount(FilterDate);
                //ManufactureStoreManager.FilterMovementInvoices(FactoryID);
                ManufactureStoreManager.FilterMovementInvoices(
                    cbxMovInvoiceDateFilter.Checked, cbxMovInvoicePerson.Checked, cbxMovInvoiceSeller.Checked, cbxMovInvoiceRecipient.Checked,
                    dtpMovInvoiceFilterFrom.Value, dtpMovInvoiceFilterTo.Value,
                    Convert.ToInt32(cmbMovInvoicePersons.SelectedValue), Convert.ToInt32(cmbMovInvoiceSellers.SelectedValue), Convert.ToInt32(cmbMovInvoiceRecipients.SelectedValue), FactoryID);
                ManufactureStoreManager.ShowNoEmptyStoreItems();
                CheckInvoicesColumns(ref dgvManufactreStoreInvoices);
                NeedSplash = true;
                bC = true;
            }
            else
            {
                ManufactureStoreManager.GetStoreCount(FilterDate);
                ManufactureStoreManager.FilterGroups(FactoryID);
                ManufactureStoreManager.MoveToTechStoreGroup(TechStoreGroupID);
                int TechStoreSubGroupID = 0;
                if (dgvManufactreStoreSubGroups.SelectedRows.Count != 0 && dgvManufactreStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvManufactreStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                ManufactureStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ManufactureStoreManager.FillStoreCount(FilterDate);
                //ManufactureStoreManager.FilterMovementInvoices(FactoryID);
                ManufactureStoreManager.FilterMovementInvoices(
                    cbxMovInvoiceDateFilter.Checked, cbxMovInvoicePerson.Checked, cbxMovInvoiceSeller.Checked, cbxMovInvoiceRecipient.Checked,
                    dtpMovInvoiceFilterFrom.Value, dtpMovInvoiceFilterTo.Value,
                    Convert.ToInt32(cmbMovInvoicePersons.SelectedValue), Convert.ToInt32(cmbMovInvoiceSellers.SelectedValue), Convert.ToInt32(cmbMovInvoiceRecipients.SelectedValue), FactoryID);
                ManufactureStoreManager.ShowNoEmptyStoreItems();
                CheckInvoicesColumns(ref dgvManufactreStoreInvoices);
            }
        }

        private void FilterReadyStore()
        {
            FactoryID = 0;

            if (ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = 1;
            if (!ProfilCheckButton.Checked && TPSCheckButton.Checked)
                FactoryID = 2;
            if (!ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = -1;

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);

            int TechStoreGroupID = 0;
            if (dgvReadyStoreGroups.SelectedRows.Count != 0 && dgvReadyStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvReadyStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(panel3.Top, panel3.Left,
                                                   panel3.Height, panel3.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                NeedSplash = false;
                ReadyStoreManager.GetStoreCount(FilterDate);
                ReadyStoreManager.FilterGroups(FactoryID);
                ReadyStoreManager.MoveToTechStoreGroup(TechStoreGroupID);
                int TechStoreSubGroupID = 0;
                if (dgvReadyStoreSubGroups.SelectedRows.Count != 0 && dgvReadyStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvReadyStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                ReadyStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ReadyStoreManager.FillStoreCount(FilterDate);
                ReadyStoreManager.FilterMovementInvoices(FactoryID);
                CheckInvoicesColumns(ref dgvReadyStoreInvoices);
                NeedSplash = true;
                bC = true;
            }
            else
            {
                ReadyStoreManager.GetStoreCount(FilterDate);
                ReadyStoreManager.FilterGroups(FactoryID);
                ReadyStoreManager.MoveToTechStoreGroup(TechStoreGroupID);
                int TechStoreSubGroupID = 0;
                if (dgvReadyStoreSubGroups.SelectedRows.Count != 0 && dgvReadyStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvReadyStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                ReadyStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ReadyStoreManager.FillStoreCount(FilterDate);
                ReadyStoreManager.FilterMovementInvoices(FactoryID);
                CheckInvoicesColumns(ref dgvReadyStoreInvoices);
            }
        }

        private void FilterWriteOffStore()
        {
            FactoryID = 0;

            if (ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = 1;
            if (!ProfilCheckButton.Checked && TPSCheckButton.Checked)
                FactoryID = 2;
            if (!ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = -1;

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);

            int TechStoreGroupID = 0;
            if (dgvDispGroups.SelectedRows.Count != 0 && dgvDispGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvDispGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(panel3.Top, panel3.Left,
                                                   panel3.Height, panel3.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                NeedSplash = false;

                WriteOffStoreManager.GetStoreCount(FilterDate);
                WriteOffStoreManager.FilterGroups(FactoryID);
                WriteOffStoreManager.MoveToTechStoreGroup(TechStoreGroupID);
                int TechStoreSubGroupID = 0;
                if (dgvDispSubGroups.SelectedRows.Count != 0 && dgvDispSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvDispSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                WriteOffStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                WriteOffStoreManager.FillStoreCount();
                WriteOffStoreManager.FilterMovementInvoices(FactoryID);
                CheckInvoicesColumns(ref dgvDispInvoices);
                NeedSplash = true;
                bC = true;
            }
            else
            {
                WriteOffStoreManager.GetStoreCount(FilterDate);
                WriteOffStoreManager.FilterGroups(FactoryID);
                WriteOffStoreManager.MoveToTechStoreGroup(TechStoreGroupID);
                int TechStoreSubGroupID = 0;
                if (dgvDispSubGroups.SelectedRows.Count != 0 && dgvDispSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(dgvDispSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                WriteOffStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                WriteOffStoreManager.FillStoreCount();
                WriteOffStoreManager.FilterMovementInvoices(FactoryID);
                CheckInvoicesColumns(ref dgvDispInvoices);
            }
        }

        private void FilterPersonalStore()
        {
            FactoryID = 0;

            if (ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = 1;
            if (!ProfilCheckButton.Checked && TPSCheckButton.Checked)
                FactoryID = 2;
            if (!ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = -1;

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxPYears.SelectedValue), Convert.ToInt32(cbxPMonths.SelectedValue), 1);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(panel3.Top, panel3.Left,
                                                   panel3.Height, panel3.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                NeedSplash = false;
                PersonalStorageManager.GetStoreCount(FilterDate);
                PersonalStorageManager.FilterGroups(FactoryID,
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, FilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
                PersonalStorageManager.FilterStore(FactoryID, PersonalStorageManager.CurrentSubGroup,
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, FilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
                PersonalStorageManager.FillStoreCount();
                PersonalStorageManager.ShowNoEmptyStoreItems();
                PersonalStorageManager.FilterInvoices(
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, FilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue), FactoryID);
                NeedSplash = true;
                bC = true;
            }
            else
            {
                PersonalStorageManager.GetStoreCount(FilterDate);
                PersonalStorageManager.FilterGroups(FactoryID,
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, FilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
                PersonalStorageManager.FilterStore(FactoryID, PersonalStorageManager.CurrentSubGroup,
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, FilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
                PersonalStorageManager.FillStoreCount();
                PersonalStorageManager.ShowNoEmptyStoreItems();
                PersonalStorageManager.FilterInvoices(
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, FilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue), FactoryID);
            }
        }

        private void kryptonCheckSet3_CheckedButtonChanged(object sender, EventArgs e)
        {
            pnlPurchInvoicesMenu.Visible = false;
            pnlMovementInvoicesMenu.Visible = false;
            pnlMainStoreMenu.Visible = false;
            pnlPersonalStoreMenu.Visible = false;

            if (kryptonCheckSet3.CheckedButton == cbtnMainStore)
            {
                if (MainStoreManager != null && bNeedUpdate)
                    FilterMainStore();
                pnlMainStoreClientArea.BringToFront();
                pnlMainStoreTools.BringToFront();
                if (kryptonCheckSet1.CheckedButton == cbtnMainStoreProduct)
                {
                    pnlMainStoreProduct.BringToFront();
                    pnlMainStoreInventoryTools.BringToFront();
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }

                if (kryptonCheckSet1.CheckedButton == cbtnPurchaseInvoices)
                {
                    pnlMainStorePurchInvoices.BringToFront();
                    pnlMainStorePurchInvoiceTools.BringToFront();

                    pnlPurchInvoicesMenu.Visible = MenuButton.Checked;
                }

                if (kryptonCheckSet1.CheckedButton == cbtnMovementInvoices)
                {
                    pnlMainStoreMovInvoices.BringToFront();
                    pnlMainStoreMovInvoiceTools.BringToFront();
                    pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                }
            }

            if (kryptonCheckSet3.CheckedButton == cbtnManufactureStore)
            {
                if (ManufactureStoreManager != null)
                    FilterManufactureStore();
                ManufactureClientAreaPanel.BringToFront();
                ManufactureToolsPanel.BringToFront();

                if (kryptonCheckSet4.CheckedButton == cbtnManufactureStoreProducts)
                {
                    ManGroupsPanel.BringToFront();
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet4.CheckedButton == cbtnManufactureStoreInvoices)
                {
                    ManInvoicesPanel.BringToFront();
                    pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                }
            }

            if (kryptonCheckSet3.CheckedButton == cbtnReadyStore)
            {
                if (ReadyStoreManager != null)
                    FilterReadyStore();
                ReadyStoreClientAreaPanel.BringToFront();
                ReadyStoreToolsPanel.BringToFront();

                if (kryptonCheckSet7.CheckedButton == cbtnReadyStoreProducts)
                {
                    pnlReadyStoreProducts.BringToFront();
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet7.CheckedButton == cbtnReadyStoreInvoices)
                {
                    ReadyStoreToolsPanel.BringToFront();
                    pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                }
            }

            if (kryptonCheckSet3.CheckedButton == cbtnWriteOffStore)
            {
                if (WriteOffStoreManager != null)
                    FilterWriteOffStore();
                DispatchClientAreaPanel.BringToFront();
                DispatchToolsPanel.BringToFront();

                if (kryptonCheckSet5.CheckedButton == cbtnWriteOffStoreProduct)
                {
                    DispGroupsPanel.BringToFront();
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet5.CheckedButton == cbtnWriteOffStoreInvoices)
                {
                    DispInvoicesPanel.BringToFront();
                }
            }
            if (kryptonCheckSet3.CheckedButton == cbtnPersonalStore)
            {
                if (PersonalStorageManager != null)
                    FilterPersonalStore();
                PersonalClientAreaPanel.BringToFront();
                PersonalToolsPanel.BringToFront();

                if (kryptonCheckSet6.CheckedButton == cbtnPersonalStoreProducts)
                {
                    PersGroupsPanel.BringToFront();
                }
                if (kryptonCheckSet6.CheckedButton == cbtnPersonalStoreInvoices)
                {
                    PersInvoicesPanel.BringToFront();
                    pnlPersonalStoreMenu.Visible = MenuButton.Checked;
                }
            }
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (MainStoreManager == null)
                return;

            if (kryptonCheckSet2.CheckedButton == ProfilCheckButton)
            {
                FactoryID = 1;
            }

            if (kryptonCheckSet2.CheckedButton == TPSCheckButton)
            {
                FactoryID = 2;
            }

            NeedSplash = false;

            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(panel3.Top, panel3.Left,
                                               panel3.Height, panel3.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (kryptonCheckSet3.CheckedButton == cbtnMainStore)
            {
                FilterMainStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnManufactureStore)
            {
                FilterManufactureStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnReadyStore)
            {
                FilterReadyStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnWriteOffStore)
            {
                FilterWriteOffStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnPersonalStore)
            {
                FilterPersonalStore();
            }

            NeedSplash = true;
            bC = true;
        }

        private void dgvManufactreStoreGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (ManufactureStoreManager == null)
                return;

            int TechStoreGroupID = 0;
            if (dgvManufactreStoreGroups.SelectedRows.Count != 0 && dgvManufactreStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvManufactreStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ManufactureStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                ManufactureStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
        }

        private void dgvManufactreStoreInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (ManufactureStoreManager == null)
                return;

            int MovementInvoiceID = -1;
            int RecipientStoreAllocID = 0;
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                RecipientStoreAllocID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            //if (MovementInvoiceID == 0)
            //    return;

            int StoreType = 1;
            if (RecipientStoreAllocID == 1 || RecipientStoreAllocID == 2)
                StoreType = 1;
            if (RecipientStoreAllocID == 3 || RecipientStoreAllocID == 4)
                StoreType = 2;
            if (RecipientStoreAllocID == 12 || RecipientStoreAllocID == 13)
                StoreType = 3;
            if (RecipientStoreAllocID == 9)
                StoreType = 4;
            if (RecipientStoreAllocID == 10 || RecipientStoreAllocID == 11)
                StoreType = 5;
            ManufactureStoreManager.FilterByMovementInvoice(MovementInvoiceID, FactoryID, StoreType);
            CheckManufactureStoreColumns(true, ref dgvManufactreStoreInvoiceItems);
        }

        private void kryptonCheckSet4_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ManufactureStoreManager == null)
                return;

            pnlMovementInvoicesMenu.Visible = false;
            kryptonPanel3.Visible = false;
            ManufactureInvoicesEditPanel.Visible = false;
            if (kryptonCheckSet4.CheckedButton == cbtnManufactureStoreProducts)
            {
                ManGroupsPanel.BringToFront();
                pnlMainStoreMenu.Visible = MenuButton.Checked;
                pnlMovementInvoicesMenu.Visible = false;
                kryptonPanel3.Visible = true;
                //ManufactureStoreManager.MoveToStore();
            }

            if (kryptonCheckSet4.CheckedButton == cbtnManufactureStoreInvoices)
            {
                ManInvoicesPanel.BringToFront();
                if (bPrintLabel)
                    ManufactureInvoicesEditPanel.Visible = true;

                int MovementInvoiceID = 0;
                if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                    MovementInvoiceID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
                ManufactureStoreManager.MoveToMovementInvoice(MovementInvoiceID);
                pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                pnlMainStoreMenu.Visible = false;
            }
        }

        private void kryptonCheckSet5_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (WriteOffStoreManager == null)
                return;

            if (kryptonCheckSet5.CheckedButton == cbtnWriteOffStoreProduct)
            {
                DispGroupsPanel.BringToFront();
            }

            if (kryptonCheckSet5.CheckedButton == cbtnWriteOffStoreInvoices)
            {
                DispInvoicesPanel.BringToFront();
                //WriteOffStoreManager.MoveToInvoice();
            }
        }

        private void dgvDispGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (WriteOffStoreManager == null)
                return;

            int TechStoreGroupID = 0;
            if (dgvDispGroups.SelectedRows.Count != 0 && dgvDispGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvDispGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                WriteOffStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                WriteOffStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
        }

        private void dgvDispInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (WriteOffStoreManager == null)
                return;

            int MovementInvoiceID = -1;
            int RecipientStoreAllocID = 0;
            if (dgvDispInvoices.SelectedRows.Count != 0 && dgvDispInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvDispInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (dgvDispInvoices.SelectedRows.Count != 0 && dgvDispInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                RecipientStoreAllocID = Convert.ToInt32(dgvDispInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            //if (MovementInvoiceID == 0)
            //    return;

            int StoreType = 1;
            if (RecipientStoreAllocID == 1 || RecipientStoreAllocID == 2)
                StoreType = 1;
            if (RecipientStoreAllocID == 3 || RecipientStoreAllocID == 4)
                StoreType = 2;
            if (RecipientStoreAllocID == 12 || RecipientStoreAllocID == 13)
                StoreType = 3;
            if (RecipientStoreAllocID == 9)
                StoreType = 4;
            if (RecipientStoreAllocID == 10 || RecipientStoreAllocID == 11)
                StoreType = 5;
            WriteOffStoreManager.FilterByMovementInvoice(MovementInvoiceID, FactoryID, StoreType);
            CheckWriteOffStoreColumns(true, ref dgvDispInvoiceItems);
        }

        private void FilterInvoices()
        {
            if (MainStoreManager == null)
                return;

            FactoryID = 0;

            if (ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = 1;
            if (!ProfilCheckButton.Checked && TPSCheckButton.Checked)
                FactoryID = 2;
            if (!ProfilCheckButton.Checked && !TPSCheckButton.Checked)
                FactoryID = -1;

            MainStoreManager.FilterInvoices(
                cbxInvoiceDateFilter.Checked, cbxInvoiceSeller.Checked,
                dtpInvoiceFilterFrom.Value, dtpInvoiceFilterTo.Value,
                Convert.ToInt32(cmbInvoiceSellers.SelectedValue), FactoryID);
        }

        private void MovementAddItemButton_Click(object sender, EventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters()
            {
                FactoryID = FactoryID
            };
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SelectMovementMenu SelectMovementMenu = new SelectMovementMenu(this, MainStoreManager, ref Parameters);

            TopForm = SelectMovementMenu;
            SelectMovementMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            SelectMovementMenu.Dispose();
            TopForm = null;

            if (!Parameters.OKPress)
                return;

            //новая накладная на движение

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterMainStore();
            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //FilterMainStore();
            //MainStoreManager.MoveToLastMovementInvoice();

            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void MovementChangeItemButton_Click(object sender, EventArgs e)
        {
            //редактирование накладной на движение

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementParameters Parameters = new NewMovementParameters();
            int MovementInvoiceID = -1;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;
            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvMovInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvMovInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvMovInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvMovInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;
            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterMainStore();
            MainStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //FilterMainStore();
            //MainStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void dgvMovInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (MainStoreManager == null)
                return;

            int MovementInvoiceID = -1;
            int RecipientStoreAllocID = 0;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                RecipientStoreAllocID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            //if (MovementInvoiceID == 0)
            //    return;

            int StoreType = 1;
            if (RecipientStoreAllocID == 1 || RecipientStoreAllocID == 2)
                StoreType = 1;
            if (RecipientStoreAllocID == 3 || RecipientStoreAllocID == 4)
                StoreType = 2;
            if (RecipientStoreAllocID == 12 || RecipientStoreAllocID == 13)
                StoreType = 3;
            if (RecipientStoreAllocID == 9)
                StoreType = 4;
            if (RecipientStoreAllocID == 10 || RecipientStoreAllocID == 11)
                StoreType = 5;
            MainStoreManager.FilterByMovementInvoice(MovementInvoiceID, FactoryID, StoreType);

            CheckStoreColumns(true, ref dgvMovInvoiceItems);
        }

        private void ManufactureAddItemButton_Click(object sender, EventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters()
            {
                FactoryID = FactoryID
            };
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SelectMovementMenu SelectMovementMenu = new SelectMovementMenu(this, MainStoreManager, ref Parameters);

            TopForm = SelectMovementMenu;
            SelectMovementMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            SelectMovementMenu.Dispose();
            TopForm = null;

            if (!Parameters.OKPress)
                return;

            //новая накладная на движение

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            FilterManufactureStore();
            ManufactureStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);

            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void ManufactureChangeItemButton_Click(object sender, EventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters();
            int MovementInvoiceID = -1;
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;
            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvManufactreStoreInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvManufactreStoreInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;

            //редактирование накладной на движение

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterManufactureStore();
            ManufactureStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
        }

        private void InventoryListButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewInventoryParameters NewInventoryParameters = new Store.NewInventoryParameters();
            NewInventoryForm NewInventoryForm = new NewInventoryForm(this, ref NewInventoryParameters);
            TopForm = NewInventoryForm;
            NewInventoryForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            NewInventoryForm.Dispose();
            TopForm = null;
            if (!NewInventoryParameters.OKPress)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            int TechStoreGroupID = 0;
            if (dgvMainStoreGroups.SelectedRows.Count != 0 && dgvMainStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvMainStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);

            InventoryForm InventoryForm = new InventoryForm(TechStoreGroupID, FactoryID, NewInventoryParameters.Month, NewInventoryParameters.Year);

            TopForm = InventoryForm;

            InventoryForm.ShowDialog();

            //FilterMainStore();
            //MainStoreManager.MoveToTechStoreGroup(TechStoreGroupID);

            InventoryForm.Close();
            InventoryForm.Dispose();

            TopForm = null;
            FilterMainStore();
        }

        private void dgvMainStoreSubGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (MainStoreManager == null)
                return;

            int TechStoreSubGroupID = 0;
            if (dgvMainStoreSubGroups.SelectedRows.Count != 0 && dgvMainStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(dgvMainStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                MainStoreManager.FilterStore(TechStoreSubGroupID);
                MainStoreManager.FillStoreCount(FilterDate);
                //MainStoreManager.ShowNoEmptyStoreItems();
                CheckStoreColumns(false, ref dgvMainStore);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                MainStoreManager.FilterStore(TechStoreSubGroupID);
                MainStoreManager.FillStoreCount(FilterDate);
                //MainStoreManager.ShowNoEmptyStoreItems();
                CheckStoreColumns(false, ref dgvMainStore);
            }
        }

        private void DSubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (WriteOffStoreManager == null)
                return;

            int TechStoreSubGroupID = 0;
            if (dgvDispSubGroups.SelectedRows.Count != 0 && dgvDispSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(dgvDispSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                WriteOffStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                CheckWriteOffStoreColumns(false, ref dgvDispStore);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                WriteOffStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
            CheckWriteOffStoreColumns(false, ref dgvDispStore);

        }

        private void MSubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ManufactureStoreManager == null)
                return;

            int TechStoreSubGroupID = 0;
            if (dgvManufactreStoreSubGroups.SelectedRows.Count != 0 && dgvManufactreStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(dgvManufactreStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ManufactureStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ManufactureStoreManager.FillStoreCount(FilterDate);
                ManufactureStoreManager.ShowNoEmptyStoreItems();
                CheckManufactureStoreColumns(false, ref dgvManufactreStore);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                ManufactureStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ManufactureStoreManager.FillStoreCount(FilterDate);
                ManufactureStoreManager.ShowNoEmptyStoreItems();
                CheckManufactureStoreColumns(false, ref dgvManufactreStore);
            }
        }

        private void InventoryReportButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MovementReportMenu MovementReportMenu = new MovementReportMenu(this, FactoryID, 1, ref ReportParameters);

            TopForm = MovementReportMenu;
            MovementReportMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            MovementReportMenu.Dispose();
            TopForm = null;

            if (!ReportParameters.OKPress)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ReportToExcel MovementReport = new ReportToExcel(FactoryID, ReportParameters);

            string ExcelName = string.Empty;
            string ReportName = string.Empty;

            MovementReport.GetStoreCount();
            MovementReport.FillStoreCount();

            if (ReportParameters.DatePeriodType == 1)
            {
                ReportName = ReportParameters.FirstDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
            }
            if (ReportParameters.DatePeriodType == 2)
            {
                ReportName = ReportParameters.QuarterNumber + " квартал " + ReportParameters.FirstDate.ToString("yyyy", CultureInfo.CurrentCulture);
            }
            if (ReportParameters.ReportType == 1)
            {
                ExcelName = "Отчет в натуральных единицах, ЗОВ-Профиль, " + ReportName;
                if (FactoryID == 2)
                    ExcelName = "Отчет в натуральных единицах, ЗОВ-ТПС, " + ReportName;
                MovementReport.NaturalUnitsReport(ExcelName);
            }
            if (ReportParameters.ReportType == 2)
            {
                ExcelName = "Отчет в денежном выражении, ЗОВ-Профиль, " + ReportName;
                if (FactoryID == 2)
                    ExcelName = "Отчет в денежном выражении, ЗОВ-ТПС, " + ReportName;
                MovementReport.FinancialReport(ExcelName);
            }
            if (ReportParameters.ReportType == 3)
            {
                ExcelName = "Отчет со всеми параметрами, ЗОВ-Профиль, " + ReportName;
                if (FactoryID == 2)
                    ExcelName = "Отчет со всеми параметрами, ЗОВ-ТПС, " + ReportName;
                MovementReport.StoreParametersReport(ExcelName);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (kryptonCheckSet3.CheckedButton == cbtnMainStore)
            {
                if (kryptonCheckSet1.CheckedButton == cbtnPurchaseInvoices)
                {
                    pnlPurchInvoicesMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet1.CheckedButton == cbtnMainStoreProduct)
                {
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet1.CheckedButton == cbtnMovementInvoices)
                {
                    pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                }
            }
            if (kryptonCheckSet3.CheckedButton == cbtnManufactureStore)
            {
                if (kryptonCheckSet4.CheckedButton == cbtnManufactureStoreInvoices)
                {
                    pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet4.CheckedButton == cbtnManufactureStoreProducts)
                {
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }
            }
            if (kryptonCheckSet3.CheckedButton == cbtnPersonalStore)
            {
                pnlPersonalStoreMenu.Visible = MenuButton.Checked;
                //if (kryptonCheckSet6.CheckedButton == PInvoicesSelectButton)
                //{
                //    PersonalMenuPanel.Visible = MenuButton.Checked;
                //}
            }
            if (kryptonCheckSet3.CheckedButton == cbtnReadyStore)
            {
                if (kryptonCheckSet7.CheckedButton == cbtnReadyStoreProducts)
                {
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet7.CheckedButton == cbtnReadyStoreInvoices)
                {
                    pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                }
            }
            if (kryptonCheckSet3.CheckedButton == cbtnWriteOffStore)
            {
                if (kryptonCheckSet5.CheckedButton == cbtnWriteOffStoreProduct)
                {
                    pnlMainStoreMenu.Visible = MenuButton.Checked;
                }
                if (kryptonCheckSet5.CheckedButton == cbtnWriteOffStoreInvoices)
                {
                }
            }
        }

        private bool PermissionGranted(int PermissionID)
        {
            DataRow[] Rows = RolePermissionsDataTable.Select("PermissionID = " + PermissionID);

            if (Rows.Count() > 0)
            {
                return Convert.ToBoolean(Rows[0]["Granted"]);
            }

            return false;
        }

        private void StorageForm_Load(object sender, EventArgs e)
        {
            dgvMainStoreGroups.SelectionChanged += new EventHandler(dgvMainStoreGroups_SelectionChanged);
            dgvMainStoreSubGroups.SelectionChanged += new EventHandler(dgvMainStoreSubGroups_SelectionChanged);
            dgvPurchInvoices.SelectionChanged += new EventHandler(dgvPurchInvoices_SelectionChanged);
            dgvDispGroups.SelectionChanged += new EventHandler(dgvDispGroups_SelectionChanged);
            dgvDispInvoices.SelectionChanged += new EventHandler(dgvDispInvoices_SelectionChanged);
            dgvManufactreStoreGroups.SelectionChanged += new EventHandler(dgvManufactreStoreGroups_SelectionChanged);
            dgvManufactreStoreInvoices.SelectionChanged += new EventHandler(dgvManufactreStoreInvoices_SelectionChanged);
            dgvMovInvoices.SelectionChanged += new EventHandler(dgvMovInvoices_SelectionChanged);
            dgvPersGroups.SelectionChanged += new EventHandler(dgvPersGroups_SelectionChanged);
            dgvPersInvoices.SelectionChanged += new EventHandler(dgvPersInvoices_SelectionChanged);
            dgvReadyStoreGroups.SelectionChanged += new EventHandler(dgvReadyStoreGroups_SelectionChanged);
            dgvReadyStoreInvoices.SelectionChanged += new EventHandler(dgvReadyStoreInvoices_SelectionChanged);

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID, this.Name);
            if (PermissionGranted(iInventory))
            {
                bInventory = true;
            }
            if (PermissionGranted(iPrintLabel))
            {
                bPrintLabel = true;
            }
            if (PermissionGranted(iCreateInvoice))
            {
                bCreateInvoice = true;
            }
            if (PermissionGranted(iMoveProducts))
            {
                bMoveProducts = true;
            }

            if (bMoveProducts && bInventory && bCreateInvoice && bPrintLabel)
            {
                RoleType = RoleTypes.AdminRole;
            }
            if (bMoveProducts && bInventory && bCreateInvoice && !bPrintLabel)
            {
                RoleType = RoleTypes.LogisticsRole;
            }
            if (bMoveProducts && !bInventory && bCreateInvoice && bPrintLabel)
            {
                RoleType = RoleTypes.DeliveryRole;
            }
            if (!bMoveProducts && !bInventory && !bCreateInvoice && bPrintLabel)
            {
                RoleType = RoleTypes.TechnologyRole;
            }

            if (RoleType == RoleTypes.OrdinaryRole)
            {
                MoveToPersonButton.Visible = false;
                SelectStoreButton.Visible = false;
                pnlMainStorePurchInvoiceTools.Visible = false;
                pnlMainStoreMovInvoiceTools.Visible = false;
                kryptonPanel3.Visible = false;
                PersonalInvoicesEditPanel.Visible = false;
                ManufactureInvoicesEditPanel.Visible = false;
                ReadyStoreInvoicesEditPanel.Visible = false;
                pnlMainStoreInventoryTools.Visible = false;
            }
            if (RoleType == RoleTypes.AdminRole)
            {

            }
            if (RoleType == RoleTypes.DeliveryRole)
            {
                InventoryListButton.Visible = false;
                InventoryReportButton.Visible = false;
            }
            if (RoleType == RoleTypes.LogisticsRole)
            {

            }
            if (RoleType == RoleTypes.TechnologyRole)
            {

            }

            dgvMainStoreGroups.Columns["TechStoreGroupID"].Visible = false;

            dgvMainStoreSubGroups.Columns["TechStoreGroupID"].Visible = false;
            dgvMainStoreSubGroups.Columns["TechStoreSubGroupID"].Visible = false;
            //dgvMainStoreSubGroups.Columns["Notes"].Visible = false;
            //dgvMainStoreSubGroups.Columns["Notes1"].Visible = false;
            //dgvMainStoreSubGroups.Columns["Notes2"].Visible = false;

            CheckStoreColumns(true, ref dgvMovInvoiceItems);
            CheckManufactureStoreColumns(true, ref dgvManufactreStoreInvoiceItems);
            CheckReadyStoreColumns(true, ref dgvReadyStoreInvoiceItems);
            CheckWriteOffStoreColumns(true, ref dgvDispInvoiceItems);
            CheckPersonalStoreColumns(true, ref dgvPersInvoiceItems);
        }

        private void dgvPersInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (PersonalStorageManager == null)
                return;

            if (!PersonalStorageManager.HasInvoices)
            {
                PersonalStorageManager.ClearItems();
                return;
            }

            int StoreType = 1;
            if (PersonalStorageManager.CurrentRecipientStoreAlloc == 1 || PersonalStorageManager.CurrentRecipientStoreAlloc == 2)
                StoreType = 1;
            if (PersonalStorageManager.CurrentRecipientStoreAlloc == 3 || PersonalStorageManager.CurrentRecipientStoreAlloc == 4)
                StoreType = 2;
            if (PersonalStorageManager.CurrentRecipientStoreAlloc == 12 || PersonalStorageManager.CurrentRecipientStoreAlloc == 13)
                StoreType = 3;
            if (PersonalStorageManager.CurrentRecipientStoreAlloc == 9)
                StoreType = 4;
            if (PersonalStorageManager.CurrentRecipientStoreAlloc == 10 || PersonalStorageManager.CurrentRecipientStoreAlloc == 11)
                StoreType = 5;

            PersonalStorageManager.FilterByInvoice(PersonalStorageManager.CurrentMovementInvoice, FactoryID, StoreType);
            CheckPersonalStoreColumns(true, ref dgvPersInvoiceItems);
        }

        private void dgvPersGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (PersonalStorageManager == null)
                return;

            DateTime PFilterDate = new DateTime(Convert.ToInt32(cbxPYears.SelectedValue), Convert.ToInt32(cbxPMonths.SelectedValue), 1);
            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                PersonalStorageManager.FilterSubGroups(FactoryID, PersonalStorageManager.CurrentGroup,
                 true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
                 Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                PersonalStorageManager.FilterSubGroups(FactoryID, PersonalStorageManager.CurrentGroup,
                 true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
                 Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));

            }
        }

        private void PSubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PersonalStorageManager == null)
                return;

            DateTime PFilterDate = new DateTime(Convert.ToInt32(cbxPYears.SelectedValue), Convert.ToInt32(cbxPMonths.SelectedValue), 1);
            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                PersonalStorageManager.FilterStore(FactoryID, PersonalStorageManager.CurrentSubGroup,
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
                PersonalStorageManager.FillStoreCount();
                PersonalStorageManager.ShowNoEmptyStoreItems();

                CheckPersonalStoreColumns(false, ref dgvPersStore);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                PersonalStorageManager.FilterStore(FactoryID, PersonalStorageManager.CurrentSubGroup,
                     true, cbxPersonalPerson.Checked, cbxPersonalOtherPerson.Checked, cbxPersonalSeller.Checked, PFilterDate,
                     Convert.ToInt32(cmbPersonalPersons.SelectedValue), cmbPersonalOtherPersons.SelectedValue.ToString(), Convert.ToInt32(cmbPersonalSellers.SelectedValue));
                PersonalStorageManager.FillStoreCount();
                PersonalStorageManager.ShowNoEmptyStoreItems();

                CheckPersonalStoreColumns(false, ref dgvPersStore);
            }
        }

        private void kryptonCheckSet6_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (PersonalStorageManager == null)
                return;

            if (kryptonCheckSet6.CheckedButton == cbtnPersonalStoreProducts)
            {
                PersGroupsPanel.BringToFront();
                PersonalStoreToolsPanel.BringToFront();
            }

            if (kryptonCheckSet6.CheckedButton == cbtnPersonalStoreInvoices)
            {
                PersInvoicesPanel.BringToFront();
                PersonalInvoicesEditPanel.BringToFront();
                PersonalStorageManager.MoveToInvoice();
            }
        }

        private void btnInvoicesFilter_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            FilterInvoices();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnMainStoreFilter_Click(object sender, EventArgs e)
        {
            pnlMainStoreMenu.Visible = false;
            MenuButton.Checked = false;
            if (kryptonCheckSet3.CheckedButton == cbtnMainStore)
            {
                FilterMainStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnManufactureStore)
            {
                FilterManufactureStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnReadyStore)
            {
                FilterReadyStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnWriteOffStore)
            {
                FilterWriteOffStore();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnPersonalStore)
            {
                FilterPersonalStore();
            }
        }

        private void btnMovInvoicesFilter_Click(object sender, EventArgs e)
        {
            btnMainStoreFilter_Click(null, null);
        }

        private void PersonalChangeItemButton_Click(object sender, EventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters();
            int MovementInvoiceID = -1;
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;
            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvPersInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvPersInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvPersInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvPersInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterPersonalStore();
        }

        private void PersonalAddItemButton_Click(object sender, EventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters()
            {
                FactoryID = FactoryID
            };
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SelectMovementMenu SelectMovementMenu = new SelectMovementMenu(this, MainStoreManager, ref Parameters);

            TopForm = SelectMovementMenu;
            SelectMovementMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            SelectMovementMenu.Dispose();
            TopForm = null;

            if (!Parameters.OKPress)
                return;

            //новая накладная на движение

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterPersonalStore();
        }

        private void btnManInvoicesFilter_Click(object sender, EventArgs e)
        {
            FilterManufactureStore();
        }

        private void StoreDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            //if (StorageManager == null || dgvSubGroups.SelectedRows.Count == 0)
            //    return;

            //int PurchaseInvoiceID = 0;
            //if (FactoryID == 1 && dgvProfilStore.SelectedRows.Count != 0 && dgvProfilStore.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
            //    PurchaseInvoiceID = Convert.ToInt32(dgvProfilStore.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            //if (FactoryID == 2 && dgvProfilStore.SelectedRows.Count != 0 && dgvProfilStore.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
            //    PurchaseInvoiceID = Convert.ToInt32(dgvTPSStore.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            //StorageManager.MoveToInvoice(PurchaseInvoiceID);
        }

        private void btnPersonalFilter_Click(object sender, EventArgs e)
        {
            FilterPersonalStore();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void InvoiceItemsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && (RoleType == RoleTypes.AdminRole || RoleType == RoleTypes.DeliveryRole || RoleType == RoleTypes.TechnologyRole))
            {
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnPrintStoreGenetics_Click(object sender, EventArgs e)
        {
            if (CurrentRowIndex < 0)
                return;
            //StorageManager.MoveToInvoiceItemPosition(CurrentRowIndex);
            //int CurrentStoreID = StorageManager.CurrentInvoiceItem;

            int PurchaseInvoiceID = Convert.ToInt32(dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            int[] StoreID = new int[dgvPurchInvoiceItems.SelectedRows.Count];
            int StoreGeneticsID = -1;
            int[] TechStoreID = new int[dgvPurchInvoiceItems.SelectedRows.Count];

            for (int i = 0; i < dgvPurchInvoiceItems.SelectedRows.Count; i++)
            {
                StoreID[i] = Convert.ToInt32(dgvPurchInvoiceItems.SelectedRows[i].Cells["StoreID"].Value);
                TechStoreID[i] = Convert.ToInt32(dgvPurchInvoiceItems.SelectedRows[i].Cells["StoreItemID"].Value);
                if (MainStoreManager.ExistStoreGenetics(StoreID[i]) == -1)
                    MainStoreManager.AddStoreGenetics(StoreID[i], TechStoreID[i], FactoryID);
            }

            StoreGeneticsLabel.ClearLabelInfo();
            GeneticsManager.ClearGeneticsInfo();
            GeneticsManager.ClearPurchaseInvoices();
            GeneticsManager.ClearStore();
            GeneticsManager.ClearTechStore();
            GeneticsManager.FillPurchaseInvoices(PurchaseInvoiceID);
            GeneticsManager.FillStore(StoreID);
            GeneticsManager.FillStoreGenetics(StoreID);
            GeneticsManager.FillTechStore(TechStoreID);

            for (int j = 0; j < StoreID.Count(); j++)
            {
                //Проверка
                //if (!MarketingPackagesPrintManager.IsMainOrderPacked(MainOrders[j]))
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false,
                //        "Этикетки распечатать нельзя. Подзаказ № " + MainOrders[j] + " должен быть полностью распределен на обеих фирмах",
                //        "Ошибка печати");

                //    return;
                //}
                StoreGeneticsID = MainStoreManager.ExistStoreGenetics(StoreID[j]);
                if (FactoryID == 1)
                    BarcodeType = 13;
                else
                    BarcodeType = 14;
                GeneticsManager.SetBarcodeNumber(BarcodeType, StoreGeneticsID);
                GeneticsManager.SetPurchaseInvoiceInfo(PurchaseInvoiceID);
                GeneticsManager.SetStoreGeneticsInfo(StoreID[j]);
                GeneticsManager.SetStoreInfo(StoreID[j]);
                GeneticsManager.SetTechStoreInfo(TechStoreID[j]);

                GeneticsInfo LabelInfo = new GeneticsInfo();

                LabelInfo = GeneticsManager.GetGeneticsInfo;
                LabelInfo.FactoryID = FactoryID;

                StoreGeneticsLabel.AddLabelInfo(ref LabelInfo);
            }

            PrintDialog.Document = StoreGeneticsLabel.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                StoreGeneticsLabel.Print();
                if (StoreID.Count() > 0)
                    MainStoreManager.PrintStoreGenetics(StoreID);
            }
        }

        private void PStoreDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PersonalStorageManager == null || dgvPersSubGroups.SelectedRows.Count < 1)
                return;

            //PersonalStorageManager.MoveToInvoice();
        }

        private void AddCheckColumn()
        {
            DataGridViewCheckBoxColumn CheckColumn = new DataGridViewCheckBoxColumn()
            {
                Name = "CheckColumn",
                HeaderText = "",
                DataPropertyName = "Check",
                DisplayIndex = 0,
                SortMode = DataGridViewColumnSortMode.Automatic,
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 60
            };
            dgvPersStore.Columns.Add(CheckColumn);
        }

        private void RemoveCheckColumn()
        {
            if (dgvPersStore.Columns.Contains("CheckColumn"))
                dgvPersStore.Columns.Remove("CheckColumn");
        }

        private void SelectStoreButton_Click(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(SelectStoreButton.Tag))
            {
                MoveToPersonButton.Visible = false;
                SelectStoreButton.Tag = false;
                SelectStoreButton.Text = "Выбрать";
                RemoveCheckColumn();
                dgvPersStore.Columns["CurrentCount"].ReadOnly = true;
            }
            else
            {
                MoveToPersonButton.Visible = true;
                SelectStoreButton.Tag = true;
                SelectStoreButton.Text = "Отмена";
                AddCheckColumn();
                dgvPersStore.Columns["CheckColumn"].ReadOnly = false;
                dgvPersStore.Columns["CurrentCount"].ReadOnly = false;
            }
        }

        private void MoveToPersonButton_Click(object sender, EventArgs e)
        {
            ArrayList Rows = new ArrayList();
            for (int i = 0; i < dgvPersStore.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvPersStore.Rows[i].Cells["CheckColumn"].Value))
                {
                    if (Convert.ToDecimal(dgvPersStore.Rows[i].Cells["CurrentCount"].Value) > 0)
                        Rows.Add(Convert.ToInt32(dgvPersStore.Rows[i].Cells["PersonalStoreID"].Value));
                }
            }
            if (Rows.Count == 0)
                return;

            bool PressOK = false;
            int LastMovementInvoiceID = -1;
            int PersonID = 0;
            int[] SelectedRows = Rows.OfType<int>().ToArray();
            string PersonName = string.Empty;
            object IncomeDateTime = null;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MoveToPersonMenu PurchaseToPersonlMenu = new MoveToPersonMenu(this);
            TopForm = PurchaseToPersonlMenu;
            PurchaseToPersonlMenu.ShowDialog();

            PressOK = PurchaseToPersonlMenu.PressOK;
            PersonID = PurchaseToPersonlMenu.PersonID;
            PersonName = PurchaseToPersonlMenu.PersonName;
            IncomeDateTime = PurchaseToPersonlMenu.IncomeDateTime;

            PhantomForm.Close();
            PhantomForm.Dispose();
            PurchaseToPersonlMenu.Dispose();
            TopForm = null;

            if (PressOK)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                PersonalStorageManager.SaveMovementInvoices(Convert.ToDateTime(IncomeDateTime), 9, 9, 0,
                        PersonID, PersonName, Security.CurrentUserID, 0, 0, string.Empty, string.Empty);
                LastMovementInvoiceID = PersonalStorageManager.GetLastMovementInvoiceID();
                PersonalStorageManager.MoveToPersonalStore(LastMovementInvoiceID, SelectedRows);
                FilterPersonalStore();
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void PersonalInventoryButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewInventoryParameters NewInventoryParameters = new Store.NewInventoryParameters();
            NewInventoryForm NewInventoryForm = new NewInventoryForm(this, ref NewInventoryParameters);
            TopForm = NewInventoryForm;
            NewInventoryForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            NewInventoryForm.Dispose();
            TopForm = null;
            if (!NewInventoryParameters.OKPress)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            PersonalInventoryForm InventoryForm = new PersonalInventoryForm(FactoryID, Security.CurrentUserID, NewInventoryParameters.Month, NewInventoryParameters.Year);

            TopForm = InventoryForm;

            InventoryForm.ShowDialog();

            InventoryForm.Close();
            InventoryForm.Dispose();

            TopForm = null;

            FilterPersonalStore();
        }

        private void MovementInvoiceItemsDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;

            if (grid.Columns.Contains("StoreItemColumn") && (e.ColumnIndex == grid.Columns["StoreItemColumn"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int TechDecorID = -1;
                string StoreName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["StoreItemID"].Value != DBNull.Value)
                {
                    TechDecorID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["StoreItemID"].Value);
                    StoreName = MainStoreManager.StoreName(TechDecorID);
                }
                cell.ToolTipText = StoreName;
            }
        }


        private void InvoicesDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;

            int PurchaseInvoiceID = 0;
            if (dgvPurchInvoices.SelectedRows.Count != 0 && dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
                PurchaseInvoiceID = Convert.ToInt32(dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);

            if (PurchaseInvoiceID == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewArrivalForm NewArrivalForm = new NewArrivalForm(PurchaseInvoiceID);

            TopForm = NewArrivalForm;

            NewArrivalForm.ShowDialog();

            NewArrivalForm.Close();
            NewArrivalForm.Dispose();

            TopForm = null;

            FilterMainStore();
            MainStoreManager.MoveToInvoice(PurchaseInvoiceID);
            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //FilterMainStore();
            //MainStoreManager.MoveToInvoice(PurchaseInvoiceID);
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void MovementInvoicesDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            //редактирование накладной на движение

            NewMovementParameters Parameters = new NewMovementParameters();
            int MovementInvoiceID = -1;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;
            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvMovInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvMovInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvMovInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvMovInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterMainStore();
            MainStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //FilterMainStore();
            //MainStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void dgvProfilStore_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiFindPurchInvoice_Click(object sender, EventArgs e)
        {
            if (MainStoreManager == null || dgvMainStore.SelectedRows.Count == 0)
                return;

            int PurchaseInvoiceID = 0;
            if (dgvMainStore.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
                PurchaseInvoiceID = Convert.ToInt32(dgvMainStore.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            MainStoreManager.MoveToInvoice(PurchaseInvoiceID);
            kryptonCheckSet1.CheckedButton = cbtnPurchaseInvoices;
        }

        private void dgvManGroups_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }

        private void dgvManInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters();
            int MovementInvoiceID = -1;
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;
            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvManufactreStoreInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvManufactreStoreInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvManufactreStoreInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;
            FilterManufactureStore();
            ManufactureStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;
            }
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewInventoryParameters NewInventoryParameters = new Store.NewInventoryParameters();
            NewInventoryForm NewInventoryForm = new NewInventoryForm(this, ref NewInventoryParameters);
            TopForm = NewInventoryForm;
            NewInventoryForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            NewInventoryForm.Dispose();
            TopForm = null;
            if (!NewInventoryParameters.OKPress)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            int TechStoreGroupID = 0;
            if (dgvManufactreStoreGroups.SelectedRows.Count != 0 && dgvManufactreStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvManufactreStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);

            ManufactureStoreInventoryForm InventoryForm = new ManufactureStoreInventoryForm(TechStoreGroupID, FactoryID, NewInventoryParameters.Month, NewInventoryParameters.Year);

            TopForm = InventoryForm;

            InventoryForm.ShowDialog();

            //FilterMainStore();
            //MainStoreManager.MoveToTechStoreGroup(TechStoreGroupID);

            InventoryForm.Close();
            InventoryForm.Dispose();

            TopForm = null;
            FilterManufactureStore();
        }

        private void kryptonCheckSet7_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ReadyStoreManager == null)
                return;

            pnlMovementInvoicesMenu.Visible = false;
            pnlMainStoreMenu.Visible = false;
            kryptonPanel5.Visible = false;
            ReadyStoreInvoicesEditPanel.Visible = false;
            if (kryptonCheckSet7.CheckedButton == cbtnReadyStoreProducts)
            {
                pnlReadyStoreProducts.BringToFront();
                pnlMainStoreMenu.Visible = MenuButton.Checked;
                kryptonPanel5.Visible = true;
                //ManufactureStoreManager.MoveToStore();
            }

            if (kryptonCheckSet7.CheckedButton == cbtnReadyStoreInvoices)
            {
                pnlReadyStoreInvoices.BringToFront();
                if (bPrintLabel)
                    ReadyStoreInvoicesEditPanel.Visible = true;

                int MovementInvoiceID = 0;
                if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                    MovementInvoiceID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
                ReadyStoreManager.MoveToMovementInvoice(MovementInvoiceID);
                pnlMovementInvoicesMenu.Visible = MenuButton.Checked;
                pnlMainStoreMenu.Visible = false;
            }
        }

        private void dgvReadyStoreGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (ReadyStoreManager == null)
                return;

            int TechStoreGroupID = 0;
            if (dgvReadyStoreGroups.SelectedRows.Count != 0 && dgvReadyStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvReadyStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ReadyStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                ReadyStoreManager.FilterSubGroups(FactoryID, TechStoreGroupID);
        }

        private void dgvReadyStoreSubGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (ReadyStoreManager == null)
                return;

            int TechStoreSubGroupID = 0;
            if (dgvReadyStoreSubGroups.SelectedRows.Count != 0 && dgvReadyStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(dgvReadyStoreSubGroups.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ReadyStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ReadyStoreManager.FillStoreCount(FilterDate);
                CheckReadyStoreColumns(false, ref dgvReadyStore);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                ReadyStoreManager.FilterStore(FactoryID, TechStoreSubGroupID);
                ReadyStoreManager.FillStoreCount(FilterDate);
                CheckReadyStoreColumns(false, ref dgvReadyStore);
            }
        }

        private void dgvReadyStoreInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (ReadyStoreManager == null)
                return;

            int MovementInvoiceID = -1;
            int RecipientStoreAllocID = 0;
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                RecipientStoreAllocID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            //if (MovementInvoiceID == 0)
            //    return;

            int StoreType = 1;
            if (RecipientStoreAllocID == 1 || RecipientStoreAllocID == 2)
                StoreType = 1;
            if (RecipientStoreAllocID == 3 || RecipientStoreAllocID == 4)
                StoreType = 2;
            if (RecipientStoreAllocID == 12 || RecipientStoreAllocID == 13)
                StoreType = 3;
            if (RecipientStoreAllocID == 9)
                StoreType = 4;
            if (RecipientStoreAllocID == 10 || RecipientStoreAllocID == 11)
                StoreType = 5;
            ReadyStoreManager.FilterByMovementInvoice(MovementInvoiceID, FactoryID, StoreType);
            CheckReadyStoreColumns(true, ref dgvReadyStoreInvoiceItems);
        }

        private void dgvReadyStoreInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters();
            int MovementInvoiceID = -1;
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;

            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvReadyStoreInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvReadyStoreInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterReadyStore();
            ReadyStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewInventoryParameters NewInventoryParameters = new Store.NewInventoryParameters();
            NewInventoryForm NewInventoryForm = new NewInventoryForm(this, ref NewInventoryParameters);
            TopForm = NewInventoryForm;
            NewInventoryForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            NewInventoryForm.Dispose();
            TopForm = null;
            if (!NewInventoryParameters.OKPress)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            int TechStoreGroupID = 0;
            if (dgvReadyStoreGroups.SelectedRows.Count != 0 && dgvReadyStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(dgvReadyStoreGroups.SelectedRows[0].Cells["TechStoreGroupID"].Value);

            ReadyStoreInventoryForm InventoryForm = new ReadyStoreInventoryForm(TechStoreGroupID, FactoryID, NewInventoryParameters.Month, NewInventoryParameters.Year);

            TopForm = InventoryForm;

            InventoryForm.ShowDialog();

            InventoryForm.Close();
            InventoryForm.Dispose();

            TopForm = null;
            FilterReadyStore();
        }

        private void kryptonButton7_Click(object sender, EventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters()
            {
                FactoryID = FactoryID
            };
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SelectMovementMenu SelectMovementMenu = new SelectMovementMenu(this, MainStoreManager, ref Parameters);

            TopForm = SelectMovementMenu;
            SelectMovementMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            SelectMovementMenu.Dispose();
            TopForm = null;

            if (!Parameters.OKPress)
                return;

            //новая накладная на движение

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterReadyStore();
            ReadyStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
        }

        private void kryptonButton6_Click(object sender, EventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters();
            int MovementInvoiceID = -1;
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;

            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvReadyStoreInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvReadyStoreInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvReadyStoreInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterReadyStore();
            ReadyStoreManager.MoveToMovementInvoice(Parameters.MovementInvoiceID);
        }

        private void kryptonButton5_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление пустой накладной.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            int MovementInvoiceID = -1;
            if (dgvReadyStoreInvoices.SelectedRows.Count != 0 && dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvReadyStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            MovementInvoices.RemoveMovementInvoice(MovementInvoiceID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void RemoveMovementInvoiceButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление пустой накладной.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            int MovementInvoiceID = -1;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            MovementInvoices.RemoveMovementInvoice(MovementInvoiceID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ManufactureInvoiceToExcelButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление пустой накладной.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            int MovementInvoiceID = -1;
            if (dgvManufactreStoreInvoices.SelectedRows.Count != 0 && dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvManufactreStoreInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            MovementInvoices.RemoveMovementInvoice(MovementInvoiceID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void PersonalInvoiceToExcelButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление пустой накладной.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            int MovementInvoiceID = -1;
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            MovementInvoices.RemoveMovementInvoice(MovementInvoiceID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvPersInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NewMovementParameters Parameters = new NewMovementParameters();

            int MovementInvoiceID = -1;
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            if (MovementInvoiceID == -1)
                return;
            Parameters.MovementInvoiceID = MovementInvoiceID;
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["SellerStoreAllocID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["RecipientStoreAllocID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value != DBNull.Value)
                Parameters.RecipientSectorID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["RecipientSectorID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["PersonID"].Value != DBNull.Value)
                Parameters.PersonID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["PersonID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value != DBNull.Value)
                Parameters.StoreKeeperID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["StoreKeeperID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["DateTime"].Value != DBNull.Value)
                Parameters.DateTime = Convert.ToDateTime(dgvPersInvoices.SelectedRows[0].Cells["DateTime"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                Parameters.PersonName = dgvPersInvoices.SelectedRows[0].Cells["PersonName"].Value.ToString();
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                Parameters.ClientName = dgvPersInvoices.SelectedRows[0].Cells["ClientName"].Value.ToString();
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                Parameters.Notes = dgvPersInvoices.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["SellerID"].Value != DBNull.Value)
                Parameters.SellerID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["SellerID"].Value);
            if (dgvPersInvoices.SelectedRows.Count != 0 && dgvPersInvoices.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                Parameters.ClientID = Convert.ToInt32(dgvPersInvoices.SelectedRows[0].Cells["ClientID"].Value);
            Parameters.FactoryID = FactoryID;
            Parameters.OKPress = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewMovementInvoiceForm NewMovementInvoiceForm = new NewMovementInvoiceForm(Parameters);

            TopForm = NewMovementInvoiceForm;

            NewMovementInvoiceForm.ShowDialog();

            NewMovementInvoiceForm.Close();
            NewMovementInvoiceForm.Dispose();

            TopForm = null;

            FilterPersonalStore();
        }

        private void dgvPersStore_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            if (PersonalStorageManager == null || dgvPersStore.SelectedRows.Count == 0)
                return;

            int MovementInvoiceID = 0;
            int PurchaseInvoiceID = 0;
            if (dgvPersStore.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvPersStore.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            PurchaseInvoiceID = MainStoreManager.FindPurchaseInvoiceID(MovementInvoiceID);
            if (PurchaseInvoiceID != 0)
            {
                bNeedUpdate = false;
                MainStoreManager.MoveToInvoice(PurchaseInvoiceID);
                kryptonCheckSet3.CheckedButton = cbtnMainStore;
                kryptonCheckSet1.CheckedButton = cbtnPurchaseInvoices;
                bNeedUpdate = true;
            }
            else
                InfiniumTips.ShowTip(this, 50, 85, "Накладная не найдена", 2700);
        }

        private void btnSelectMyself_Click(object sender, EventArgs e)
        {
            cbxPersonalPerson.Checked = true;
            cmbPersonalPersons.SelectedValue = Security.CurrentUserID;
        }

        private void dgvManufactreStoreGroups_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            FacingRollersStocks obj = new FacingRollersStocks();
            obj.ClearTables();
            obj.F();
            obj.UpdateSubGroups();
            obj.UpdateStoreItems();
            obj.UpdateManufactureStore();
            obj.CreateReport("Склад роликов");
        }

        private void cbxPersonalPerson_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxPersonalPerson.Checked)
                if (cbxPersonalOtherPerson.Checked)
                    cbxPersonalOtherPerson.Checked = false;
        }

        private void cbxPersonalOtherPerson_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxPersonalOtherPerson.Checked)
                if (cbxPersonalPerson.Checked)
                    cbxPersonalPerson.Checked = false;
        }

        private void kryptonButton8_Click(object sender, EventArgs e)
        {
            int PurchaseInvoiceID = 0;
            if (dgvPurchInvoices.SelectedRows.Count != 0 && dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value != DBNull.Value)
                PurchaseInvoiceID = Convert.ToInt32(dgvPurchInvoices.SelectedRows[0].Cells["PurchaseInvoiceID"].Value);
            if (PurchaseInvoiceID == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Выгрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            StoreInvoiceManager.GetPurchaseInvoiceData1(PurchaseInvoiceID);
            //StoreInvoiceManager.GetYourData();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MovementReportMenu MovementReportMenu = new MovementReportMenu(this, FactoryID, 2, ref ReportParameters);

            TopForm = MovementReportMenu;
            MovementReportMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            MovementReportMenu.Dispose();
            TopForm = null;

            if (!ReportParameters.OKPress)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ManufactureReportToExcel MovementReport = new ManufactureReportToExcel(FactoryID, ReportParameters);

            string ExcelName = string.Empty;
            string ReportName = string.Empty;

            MovementReport.GetStoreCount();
            MovementReport.FillStoreCount();

            if (ReportParameters.DatePeriodType == 1)
            {
                ReportName = ReportParameters.FirstDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
            }
            if (ReportParameters.DatePeriodType == 2)
            {
                ReportName = ReportParameters.QuarterNumber + " квартал " + ReportParameters.FirstDate.ToString("yyyy", CultureInfo.CurrentCulture);
            }
            if (ReportParameters.ReportType == 1)
            {
                ExcelName = "Склад пр-ва. Отчет в натуральных единицах, ЗОВ-Профиль, " + ReportName;
                if (FactoryID == 2)
                    ExcelName = "Склад пр-ва. Отчет в натуральных единицах, ЗОВ-ТПС, " + ReportName;
                MovementReport.NaturalUnitsReport(ExcelName);
            }
            if (ReportParameters.ReportType == 2)
            {
                ExcelName = "Склад пр-ва. Отчет в денежном выражении, ЗОВ-Профиль, " + ReportName;
                if (FactoryID == 2)
                    ExcelName = "Склад пр-ва. Отчет в денежном выражении, ЗОВ-ТПС, " + ReportName;
                MovementReport.FinancialReport(ExcelName);
            }
            if (ReportParameters.ReportType == 3)
            {
                ExcelName = "Склад пр-ва. Отчет со всеми параметрами, ЗОВ-Профиль, " + ReportName;
                if (FactoryID == 2)
                    ExcelName = "Склад пр-ва. Отчет со всеми параметрами, ЗОВ-ТПС, " + ReportName;
                MovementReport.StoreParametersReport(ExcelName);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MovementInvoiceToExcelButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Экспорт в Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            int MovementInvoiceID = -1;
            DateTime DateTime = Convert.ToDateTime(dgvMovInvoices.SelectedRows[0].Cells["DateTime"].Value);
            object CreateDateTime = DBNull.Value;
            string PersonName = string.Empty;
            string StoreKeeper = string.Empty;
            string SellerStoreAlloc = string.Empty;
            string RecipientStoreAlloc = string.Empty;
            string RecipientSector = string.Empty;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["SellerStoreAlloc"].Value != DBNull.Value)
                SellerStoreAlloc = dgvMovInvoices.SelectedRows[0].Cells["SellerStoreAlloc"].FormattedValue.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAlloc"].Value != DBNull.Value)
                RecipientStoreAlloc = dgvMovInvoices.SelectedRows[0].Cells["RecipientStoreAlloc"].FormattedValue.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["Sector"].Value != DBNull.Value)
                RecipientSector = dgvMovInvoices.SelectedRows[0].Cells["Sector"].FormattedValue.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["PersonName"].Value != DBNull.Value)
                PersonName = dgvMovInvoices.SelectedRows[0].Cells["PersonName"].FormattedValue.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["StoreKeeper"].Value != DBNull.Value)
                StoreKeeper = dgvMovInvoices.SelectedRows[0].Cells["StoreKeeper"].FormattedValue.ToString();
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["CreateDateTime"].Value != DBNull.Value)
                CreateDateTime = dgvMovInvoices.SelectedRows[0].Cells["CreateDateTime"].Value;
            if (dgvMovInvoices.SelectedRows.Count != 0 && dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value != DBNull.Value)
                MovementInvoiceID = Convert.ToInt32(dgvMovInvoices.SelectedRows[0].Cells["MovementInvoiceID"].Value);
            MovementInvoiceToExcel MovementInvoiceToExcel = new Store.MovementInvoiceToExcel(MovementInvoiceID);

            MovementInvoiceToExcel.ToExcel(DateTime, CreateDateTime, SellerStoreAlloc, RecipientStoreAlloc, RecipientSector, PersonName, StoreKeeper);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvMainStoreSubGroups_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvMainStore_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvMainStoreGroups_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvMovInvoices_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvMovInvoiceItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvPurchInvoices_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvPurchInvoiceItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
    }
}
