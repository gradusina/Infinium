using Infinium.Store;

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewMovementInvoiceForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        //bool EditMode = false;

        Form TopForm = null;

        NewMovementParameters Parameters;
        StoreMovementManager StoreMovementManager;
        MovementInvoices MovementInvoices;

        public NewMovementInvoiceForm(NewMovementParameters tParameters)
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            Parameters = tParameters;

            Initialize();

            //на склад
            if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
            {
                if (Parameters.RecipientStoreAllocID == 1 || Parameters.RecipientStoreAllocID == 2)
                    StorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                    ManufactureStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                    WriteOffStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 9)
                    PersonalStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                    ReadyStorePanel.BringToFront();
            }
            //на производство
            if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
            {
                if (Parameters.RecipientStoreAllocID == 1 || Parameters.RecipientStoreAllocID == 2)
                    StorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                    ManufactureStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                    WriteOffStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 9)
                    PersonalStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                    ReadyStorePanel.BringToFront();
            }
            //на отгрузку
            if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
            {
            }
            //на Работника
            if (Parameters.SellerStoreAllocID == 9)
            {
                if (Parameters.RecipientStoreAllocID == 1 || Parameters.RecipientStoreAllocID == 2)
                    StorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                    ManufactureStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                    WriteOffStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 9)
                    PersonalStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                    ReadyStorePanel.BringToFront();
            }
            //на склад готовой продукции
            if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
            {
                if (Parameters.RecipientStoreAllocID == 1 || Parameters.RecipientStoreAllocID == 2)
                    StorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                    ManufactureStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                    WriteOffStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 9)
                    PersonalStorePanel.BringToFront();
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                    ReadyStorePanel.BringToFront();
            }

            StoreMovementManager.UpdateStore();
            StoreMovementManager.UpdateReadyStore();
            StoreMovementManager.UpdateManufactureStore();
            StoreMovementManager.UpdateWriteOffStore();
            StoreMovementManager.UpdatePersonalStore();

            CheckStoreColumns(ref StoreDataGrid);
            CheckStoreColumns(ref ReadyStoreDataGrid);
            CheckStoreColumns(ref ManufactureStoreDataGrid);
            CheckStoreColumns(ref WriteOffStoreDataGrid);
            CheckStoreColumns(ref PersonalStoreDataGrid);

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
            StoreMovementManager = new StoreMovementManager()
            {
                CurrentMovementInvoiceID = Parameters.MovementInvoiceID,
                CurrentFactoryID = Parameters.FactoryID
            };
            if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
                StoreMovementManager.CurrentStoreName = "Store";
            if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
                StoreMovementManager.CurrentStoreName = "ManufactureStore";
            if (Parameters.SellerStoreAllocID == 9)
            {
                StoreMovementManager.CurrentStoreName = "PersonalStore";
            }
            if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
                StoreMovementManager.CurrentStoreName = "ReadyStore";

            StoreMovementManager.Initialize();

            MovementInvoices = new MovementInvoices()
            {
                CurrentMovementInvoiceID = StoreMovementManager.CurrentMovementInvoiceID
            };
            MovementInvoices.Initialize();

            ItemsDataGrid.DataSource = StoreMovementManager.StoreItemsList;
            SubGroupsDataGrid.DataSource = StoreMovementManager.StoreSubGroupsList;
            GroupsDataGrid.DataSource = StoreMovementManager.StoreGroupsList;

            StoreDataGrid.DataSource = StoreMovementManager.StoreList;
            ReadyStoreDataGrid.DataSource = StoreMovementManager.ReadyStoreList;
            ManufactureStoreDataGrid.DataSource = StoreMovementManager.ManufactureStoreList;
            WriteOffStoreDataGrid.DataSource = StoreMovementManager.WriteOffStoreList;
            PersonalStoreDataGrid.DataSource = StoreMovementManager.PersonalStoreList;

            CurrencyTypesComboBox.DataSource = StoreMovementManager.CurrencyTypesList;
            CurrencyTypesComboBox.DisplayMember = "CurrencyType";
            CurrencyTypesComboBox.ValueMember = "CurrencyTypeID";

            StoreGridSettings(ref StoreDataGrid);
            ManufactureStoreDataGridSettings(ref ManufactureStoreDataGrid);
            ReadyStoreDataGridSettings(ref ReadyStoreDataGrid);
            WriteOffStoreDataGridSettings(ref WriteOffStoreDataGrid);
            PersonalStoreDataGridSettings(ref PersonalStoreDataGrid);

            GridSettings();
            if (Parameters.SellerStoreAllocID == 9)
            {
                StoreMovementManager.UpdateStoreGroups(true, Parameters.PersonID);
                StoreMovementManager.UpdateStoreSubGroups(true, Parameters.PersonID);
                StoreMovementManager.UpdateStoreItems(true, Parameters.PersonID);
            }
            else
            {
                StoreMovementManager.UpdateStoreGroups(false, Security.CurrentUserID);
                StoreMovementManager.UpdateStoreSubGroups(false, Security.CurrentUserID);
                StoreMovementManager.UpdateStoreItems(false, Security.CurrentUserID);
            }
            CountTextBox.Text = string.Empty;
        }

        private void StoreGridSettings(ref PercentageDataGrid tPercentageDataGrid)
        {
            tPercentageDataGrid.Columns.Add(StoreMovementManager.ColorsColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.PatinaColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CoversColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CurrencyColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.ManufacturerColumn);

            tPercentageDataGrid.AutoGenerateColumns = false;

            if (tPercentageDataGrid.Columns.Contains("CreateUserID"))
                tPercentageDataGrid.Columns["CreateUserID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("StoreID"))
                tPercentageDataGrid.Columns["StoreID"].Visible = false;
            tPercentageDataGrid.Columns["ColorID"].Visible = false;
            tPercentageDataGrid.Columns["CoverID"].Visible = false;
            tPercentageDataGrid.Columns["PatinaID"].Visible = false;
            tPercentageDataGrid.Columns["StoreItemID"].Visible = false;
            tPercentageDataGrid.Columns["CurrencyTypeID"].Visible = false;
            tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            tPercentageDataGrid.Columns["IsArrived"].Visible = false;
            tPercentageDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            tPercentageDataGrid.Columns["MovementInvoiceID"].Visible = false;
            tPercentageDataGrid.Columns["ManufacturerID"].Visible = false;
            tPercentageDataGrid.Columns["DecorAssignmentID"].Visible = false;

            tPercentageDataGrid.Columns["TechStoreName"].HeaderText = "Наименование";
            tPercentageDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            tPercentageDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            tPercentageDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            tPercentageDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            tPercentageDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            tPercentageDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            tPercentageDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            tPercentageDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            tPercentageDataGrid.Columns["Notes"].HeaderText = "Примечание";
            tPercentageDataGrid.Columns["Price"].HeaderText = "Цена";
            tPercentageDataGrid.Columns["VAT"].HeaderText = "НДС";
            tPercentageDataGrid.Columns["Cost"].HeaderText = "Сумма";
            tPercentageDataGrid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            tPercentageDataGrid.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            tPercentageDataGrid.Columns["CurrentCount"].HeaderText = "Остаток";
            tPercentageDataGrid.Columns["BatchNumber"].HeaderText = "№ партии";
            tPercentageDataGrid.Columns["Produced"].HeaderText = "Произведено";
            tPercentageDataGrid.Columns["BestBefore"].HeaderText = "Срок годности";
            tPercentageDataGrid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";

            int DisplayIndex = 0;
            tPercentageDataGrid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;

            tPercentageDataGrid.Columns["Price"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["ManufacturerColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["BatchNumber"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Produced"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["BestBefore"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            tPercentageDataGrid.Columns["Price"].DefaultCellStyle.Format = "N";
            tPercentageDataGrid.Columns["VAT"].DefaultCellStyle.Format = "N";
            tPercentageDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            tPercentageDataGrid.Columns["VATCost"].DefaultCellStyle.Format = "N";

            tPercentageDataGrid.Columns["TechStoreName"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["ColorsColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["PatinaColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CoversColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Length"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Height"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Width"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Admission"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Diameter"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Capacity"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Weight"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["InvoiceCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CurrentCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Notes"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            tPercentageDataGrid.Columns["BatchNumber"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Produced"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Produced"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["BestBefore"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["BestBefore"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void ManufactureStoreDataGridSettings(ref PercentageDataGrid tPercentageDataGrid)
        {
            tPercentageDataGrid.Columns.Add(StoreMovementManager.ColorsColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.PatinaColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CoversColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CurrencyColumn);

            tPercentageDataGrid.AutoGenerateColumns = false;

            if (tPercentageDataGrid.Columns.Contains("CreateUserID"))
                tPercentageDataGrid.Columns["CreateUserID"].Visible = false;
            tPercentageDataGrid.Columns["ColorID"].Visible = false;
            tPercentageDataGrid.Columns["CoverID"].Visible = false;
            tPercentageDataGrid.Columns["PatinaID"].Visible = false;
            tPercentageDataGrid.Columns["StoreItemID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTypeID"))
                tPercentageDataGrid.Columns["CurrencyTypeID"].Visible = false;
            tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("IsArrived"))
                tPercentageDataGrid.Columns["IsArrived"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PurchaseInvoiceID"))
                tPercentageDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            tPercentageDataGrid.Columns["MovementInvoiceID"].Visible = false;

            tPercentageDataGrid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            tPercentageDataGrid.Columns["TechStoreName"].HeaderText = "Наименование";
            tPercentageDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            tPercentageDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            tPercentageDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            tPercentageDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            tPercentageDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            tPercentageDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            tPercentageDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            tPercentageDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            tPercentageDataGrid.Columns["Notes"].HeaderText = "Примечание";
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].HeaderText = "Цена";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].HeaderText = "НДС";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].HeaderText = "Сумма";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            tPercentageDataGrid.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            tPercentageDataGrid.Columns["CurrentCount"].HeaderText = "Остаток";
            if (tPercentageDataGrid.Columns.Contains("BatchNumber"))
            {
                tPercentageDataGrid.Columns["BatchNumber"].HeaderText = "№ партии";
            }
            if (tPercentageDataGrid.Columns.Contains("Produced"))
                tPercentageDataGrid.Columns["Produced"].HeaderText = "Произведено";
            if (tPercentageDataGrid.Columns.Contains("BestBefore"))
                tPercentageDataGrid.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            tPercentageDataGrid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;

            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("CurrencyColumn"))
                tPercentageDataGrid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DefaultCellStyle.Format = "N";

            tPercentageDataGrid.Columns["TechStoreName"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["ColorsColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["PatinaColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CoversColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Length"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Height"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Width"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Admission"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Diameter"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Capacity"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Weight"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["InvoiceCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CurrentCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Notes"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void ReadyStoreDataGridSettings(ref PercentageDataGrid tPercentageDataGrid)
        {
            tPercentageDataGrid.Columns.Add(StoreMovementManager.ColorsColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.PatinaColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CoversColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CurrencyColumn);

            tPercentageDataGrid.AutoGenerateColumns = false;

            if (tPercentageDataGrid.Columns.Contains("CreateUserID"))
                tPercentageDataGrid.Columns["CreateUserID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ReadyStoreID"))
                tPercentageDataGrid.Columns["ReadyStoreID"].Visible = false;
            tPercentageDataGrid.Columns["ColorID"].Visible = false;
            tPercentageDataGrid.Columns["CoverID"].Visible = false;
            tPercentageDataGrid.Columns["PatinaID"].Visible = false;
            tPercentageDataGrid.Columns["StoreItemID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTypeID"))
                tPercentageDataGrid.Columns["CurrencyTypeID"].Visible = false;
            tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("IsArrived"))
                tPercentageDataGrid.Columns["IsArrived"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PurchaseInvoiceID"))
                tPercentageDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            tPercentageDataGrid.Columns["MovementInvoiceID"].Visible = false;

            tPercentageDataGrid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            tPercentageDataGrid.Columns["TechStoreName"].HeaderText = "Наименование";
            tPercentageDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            tPercentageDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            tPercentageDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            tPercentageDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            tPercentageDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            tPercentageDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            tPercentageDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            tPercentageDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            tPercentageDataGrid.Columns["Notes"].HeaderText = "Примечание";
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].HeaderText = "Цена";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].HeaderText = "НДС";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].HeaderText = "Сумма";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            tPercentageDataGrid.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            tPercentageDataGrid.Columns["CurrentCount"].HeaderText = "Остаток";
            if (tPercentageDataGrid.Columns.Contains("BatchNumber"))
            {
                tPercentageDataGrid.Columns["BatchNumber"].HeaderText = "№ партии";
            }
            if (tPercentageDataGrid.Columns.Contains("Produced"))
                tPercentageDataGrid.Columns["Produced"].HeaderText = "Произведено";
            if (tPercentageDataGrid.Columns.Contains("BestBefore"))
                tPercentageDataGrid.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            tPercentageDataGrid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;

            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("CurrencyColumn"))
                tPercentageDataGrid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DefaultCellStyle.Format = "N";

            tPercentageDataGrid.Columns["TechStoreName"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["ColorsColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["PatinaColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CoversColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Length"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Height"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Width"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Admission"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Diameter"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Capacity"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Weight"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["InvoiceCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CurrentCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Notes"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void WriteOffStoreDataGridSettings(ref PercentageDataGrid tPercentageDataGrid)
        {
            tPercentageDataGrid.Columns.Add(StoreMovementManager.ColorsColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.PatinaColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CoversColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CurrencyColumn);

            tPercentageDataGrid.AutoGenerateColumns = false;

            if (tPercentageDataGrid.Columns.Contains("CreateUserID"))
                tPercentageDataGrid.Columns["CreateUserID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PersonalStoreID"))
                tPercentageDataGrid.Columns["PersonalStoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ManufactureStoreID"))
                tPercentageDataGrid.Columns["ManufactureStoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("StoreID"))
                tPercentageDataGrid.Columns["StoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("WriteOffStoreID"))
                tPercentageDataGrid.Columns["WriteOffStoreID"].Visible = false;
            tPercentageDataGrid.Columns["ColorID"].Visible = false;
            tPercentageDataGrid.Columns["CoverID"].Visible = false;
            tPercentageDataGrid.Columns["PatinaID"].Visible = false;
            tPercentageDataGrid.Columns["StoreItemID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTypeID"))
                tPercentageDataGrid.Columns["CurrencyTypeID"].Visible = false;
            tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("IsArrived"))
                tPercentageDataGrid.Columns["IsArrived"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PurchaseInvoiceID"))
                tPercentageDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            tPercentageDataGrid.Columns["MovementInvoiceID"].Visible = false;

            tPercentageDataGrid.Columns["TechStoreName"].HeaderText = "Наименование";
            tPercentageDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            tPercentageDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            tPercentageDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            tPercentageDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            tPercentageDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            tPercentageDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            tPercentageDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            tPercentageDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            tPercentageDataGrid.Columns["Notes"].HeaderText = "Примечание";
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].HeaderText = "Цена";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].HeaderText = "НДС";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].HeaderText = "Сумма";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            tPercentageDataGrid.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            tPercentageDataGrid.Columns["CurrentCount"].HeaderText = "Остаток";
            tPercentageDataGrid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            if (tPercentageDataGrid.Columns.Contains("BatchNumber"))
            {
                tPercentageDataGrid.Columns["BatchNumber"].HeaderText = "№ партии";
            }
            if (tPercentageDataGrid.Columns.Contains("Produced"))
                tPercentageDataGrid.Columns["Produced"].HeaderText = "Произведено";
            if (tPercentageDataGrid.Columns.Contains("BestBefore"))
                tPercentageDataGrid.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            tPercentageDataGrid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;

            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("CurrencyColumn"))
                tPercentageDataGrid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DefaultCellStyle.Format = "N";

            tPercentageDataGrid.Columns["TechStoreName"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["ColorsColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["PatinaColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CoversColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Length"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Height"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Width"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Admission"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Diameter"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Capacity"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Weight"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["InvoiceCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CurrentCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Notes"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void PersonalStoreDataGridSettings(ref PercentageDataGrid tPercentageDataGrid)
        {
            tPercentageDataGrid.Columns.Add(StoreMovementManager.ColorsColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.PatinaColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CoversColumn);
            tPercentageDataGrid.Columns.Add(StoreMovementManager.CurrencyColumn);

            tPercentageDataGrid.AutoGenerateColumns = false;

            if (tPercentageDataGrid.Columns.Contains("CreateUserID"))
                tPercentageDataGrid.Columns["CreateUserID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PersonalStoreID"))
                tPercentageDataGrid.Columns["PersonalStoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ManufactureStoreID"))
                tPercentageDataGrid.Columns["ManufactureStoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("StoreID"))
                tPercentageDataGrid.Columns["StoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("WriteOffStoreID"))
                tPercentageDataGrid.Columns["WriteOffStoreID"].Visible = false;
            tPercentageDataGrid.Columns["ColorID"].Visible = false;
            tPercentageDataGrid.Columns["CoverID"].Visible = false;
            tPercentageDataGrid.Columns["PatinaID"].Visible = false;
            tPercentageDataGrid.Columns["StoreItemID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTypeID"))
                tPercentageDataGrid.Columns["CurrencyTypeID"].Visible = false;
            tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("IsArrived"))
                tPercentageDataGrid.Columns["IsArrived"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PurchaseInvoiceID"))
                tPercentageDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            tPercentageDataGrid.Columns["MovementInvoiceID"].Visible = false;

            tPercentageDataGrid.Columns["TechStoreName"].HeaderText = "Наименование";
            tPercentageDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            tPercentageDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            tPercentageDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            tPercentageDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            tPercentageDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            tPercentageDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            tPercentageDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            tPercentageDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            tPercentageDataGrid.Columns["Notes"].HeaderText = "Примечание";
            tPercentageDataGrid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].HeaderText = "Цена";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].HeaderText = "НДС";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].HeaderText = "Сумма";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            tPercentageDataGrid.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            tPercentageDataGrid.Columns["CurrentCount"].HeaderText = "Остаток";
            if (tPercentageDataGrid.Columns.Contains("BatchNumber"))
                tPercentageDataGrid.Columns["BatchNumber"].HeaderText = "№ партии";
            if (tPercentageDataGrid.Columns.Contains("Produced"))
                tPercentageDataGrid.Columns["Produced"].HeaderText = "Произведено";
            if (tPercentageDataGrid.Columns.Contains("BestBefore"))
                tPercentageDataGrid.Columns["BestBefore"].HeaderText = "Срок годности";

            int DisplayIndex = 0;
            tPercentageDataGrid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;

            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            if (tPercentageDataGrid.Columns.Contains("CurrencyColumn"))
                tPercentageDataGrid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            tPercentageDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            if (tPercentageDataGrid.Columns.Contains("Price"))
                tPercentageDataGrid.Columns["Price"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VAT"))
                tPercentageDataGrid.Columns["VAT"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("Cost"))
                tPercentageDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            if (tPercentageDataGrid.Columns.Contains("VATCost"))
                tPercentageDataGrid.Columns["VATCost"].DefaultCellStyle.Format = "N";

            tPercentageDataGrid.Columns["TechStoreName"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["ColorsColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["PatinaColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CoversColumn"].MinimumWidth = 100;
            tPercentageDataGrid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Length"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Thickness"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Height"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Width"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Admission"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Diameter"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Capacity"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Weight"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["InvoiceCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["CurrentCount"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Notes"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void GridSettings()
        {
            GroupsDataGrid.Columns["TechStoreGroupID"].Visible = false;

            SubGroupsDataGrid.Columns["TechStoreSubGroupID"].Visible = false;
            SubGroupsDataGrid.Columns["TechStoreGroupID"].Visible = false;
            SubGroupsDataGrid.Columns["Notes"].Visible = false;
            SubGroupsDataGrid.Columns["Notes1"].Visible = false;
            SubGroupsDataGrid.Columns["Notes2"].Visible = false;

            ItemsDataGrid.Columns.Add(StoreMovementManager.ColorsColumn);
            ItemsDataGrid.Columns.Add(StoreMovementManager.PatinaColumn);
            ItemsDataGrid.Columns.Add(StoreMovementManager.CoversColumn);
            ItemsDataGrid.Columns.Add(StoreMovementManager.CurrencyColumn);

            if (ItemsDataGrid.Columns.Contains("CreateUserID"))
                ItemsDataGrid.Columns["CreateUserID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("PersonalStoreID"))
                ItemsDataGrid.Columns["PersonalStoreID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("ManufactureStoreID"))
                ItemsDataGrid.Columns["ManufactureStoreID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("StoreID"))
                ItemsDataGrid.Columns["StoreID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("ReadyStoreID"))
                ItemsDataGrid.Columns["ReadyStoreID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("WriteOffStoreID"))
                ItemsDataGrid.Columns["WriteOffStoreID"].Visible = false;
            ItemsDataGrid.Columns["ColorID"].Visible = false;
            ItemsDataGrid.Columns["CoverID"].Visible = false;
            ItemsDataGrid.Columns["PatinaID"].Visible = false;
            ItemsDataGrid.Columns["StoreItemID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("CurrencyTypeID"))
                ItemsDataGrid.Columns["CurrencyTypeID"].Visible = false;
            ItemsDataGrid.Columns["FactoryID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("IsArrived"))
                ItemsDataGrid.Columns["IsArrived"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("PurchaseInvoiceID"))
                ItemsDataGrid.Columns["PurchaseInvoiceID"].Visible = false;

            ItemsDataGrid.Columns["TechStoreName"].HeaderText = "Наименование";
            ItemsDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            ItemsDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            ItemsDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            ItemsDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            ItemsDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            ItemsDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            ItemsDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            ItemsDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            ItemsDataGrid.Columns["Notes"].HeaderText = "Примечание";
            ItemsDataGrid.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            ItemsDataGrid.Columns["CurrentCount"].HeaderText = "Остаток";
            if (ItemsDataGrid.Columns.Contains("Price"))
                ItemsDataGrid.Columns["Price"].HeaderText = "Цена";
            if (ItemsDataGrid.Columns.Contains("VAT"))
                ItemsDataGrid.Columns["VAT"].HeaderText = "НДС";
            if (ItemsDataGrid.Columns.Contains("Cost"))
                ItemsDataGrid.Columns["Cost"].HeaderText = "Сумма";
            if (ItemsDataGrid.Columns.Contains("VATCost"))
                ItemsDataGrid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            if (ItemsDataGrid.Columns.Contains("Produced"))
                ItemsDataGrid.Columns["Produced"].HeaderText = "Произведено";
            if (ItemsDataGrid.Columns.Contains("BestBefore"))
                ItemsDataGrid.Columns["BestBefore"].HeaderText = "Срок годности";
            ItemsDataGrid.Columns["CreateDateTime"].HeaderText = "Дата сохранения";

            int DisplayIndex = 0;
            ItemsDataGrid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;

            if (ItemsDataGrid.Columns.Contains("Price"))
                ItemsDataGrid.Columns["Price"].DisplayIndex = DisplayIndex++;
            if (ItemsDataGrid.Columns.Contains("Cost"))
                ItemsDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            if (ItemsDataGrid.Columns.Contains("VAT"))
                ItemsDataGrid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            if (ItemsDataGrid.Columns.Contains("VATCost"))
                ItemsDataGrid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            if (ItemsDataGrid.Columns.Contains("CurrencyColumn"))
                ItemsDataGrid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;

            ItemsDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            //ItemsDataGrid.Columns["StoreID"].DisplayIndex = DisplayIndex++;
            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            if (ItemsDataGrid.Columns.Contains("Price"))
            {
                ItemsDataGrid.Columns["Price"].DefaultCellStyle.Format = "N";
                ItemsDataGrid.Columns["Price"].DefaultCellStyle.FormatProvider = nfi1;
            }
            if (ItemsDataGrid.Columns.Contains("VAT"))
            {
                ItemsDataGrid.Columns["VAT"].DefaultCellStyle.Format = "N";
                ItemsDataGrid.Columns["VAT"].DefaultCellStyle.FormatProvider = nfi1;
            }
            if (ItemsDataGrid.Columns.Contains("Cost"))
            {
                ItemsDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
                ItemsDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            }
            if (ItemsDataGrid.Columns.Contains("VATCost"))
            {
                ItemsDataGrid.Columns["VATCost"].DefaultCellStyle.Format = "N";
                ItemsDataGrid.Columns["VATCost"].DefaultCellStyle.FormatProvider = nfi1;
            }

            ItemsDataGrid.Columns["Thickness"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            ItemsDataGrid.Columns["Length"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            ItemsDataGrid.Columns["Height"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            ItemsDataGrid.Columns["Width"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            ItemsDataGrid.Columns["Admission"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            ItemsDataGrid.Columns["Diameter"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            ItemsDataGrid.Columns["Capacity"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            ItemsDataGrid.Columns["Weight"].DefaultCellStyle.Format = "N";
            ItemsDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            ItemsDataGrid.Columns["TechStoreName"].MinimumWidth = 100;
            ItemsDataGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["ColorsColumn"].MinimumWidth = 100;
            ItemsDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["PatinaColumn"].MinimumWidth = 100;
            ItemsDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["CoversColumn"].MinimumWidth = 100;
            ItemsDataGrid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ItemsDataGrid.Columns["Thickness"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Length"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Thickness"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Height"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Width"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Admission"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Diameter"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Capacity"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Weight"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["InvoiceCount"].MinimumWidth = 60;
            ItemsDataGrid.Columns["InvoiceCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["CurrentCount"].MinimumWidth = 60;
            ItemsDataGrid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ItemsDataGrid.Columns["Notes"].MinimumWidth = 60;
            ItemsDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        this.Hide();
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        this.Hide();
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

        private void NewMovementInvoiceForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            bool OkCancel = Infinium.LightMessageBox.Show(ref TopForm, true, "Точно выйти? Все действия сохранены?", "Внимание");
            if (!OkCancel)
                return;
            MovementInvoices.RemoveMovementInvoice(StoreMovementManager.CurrentMovementInvoiceID);
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }


        private void GroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreMovementManager == null || GroupsDataGrid.SelectedRows.Count == 0)
                return;

            int TechStoreGroupID = 0;
            if (GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value);

            StoreMovementManager.FilterStoreSubGroups(TechStoreGroupID);
        }

        private void CheckItemsColumns()
        {
            foreach (DataGridViewColumn Column in ItemsDataGrid.Columns)
            {
                foreach (DataGridViewRow Row in ItemsDataGrid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        ItemsDataGrid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        ItemsDataGrid.Columns[Column.Index].Visible = false;
                }
            }

            if (ItemsDataGrid.Columns.Contains("ReturnedRollerID"))
                ItemsDataGrid.Columns["ReturnedRollerID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("CreateUserID"))
                ItemsDataGrid.Columns["CreateUserID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("TechStoreSubGroupID"))
                ItemsDataGrid.Columns["TechStoreSubGroupID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("DecorAssignmentID"))
                ItemsDataGrid.Columns["DecorAssignmentID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("PersonalStoreID"))
                ItemsDataGrid.Columns["PersonalStoreID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("ManufactureStoreID"))
                ItemsDataGrid.Columns["ManufactureStoreID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("StoreID"))
                ItemsDataGrid.Columns["StoreID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("WriteOffStoreID"))
                ItemsDataGrid.Columns["WriteOffStoreID"].Visible = false;
            ItemsDataGrid.Columns["ColorID"].Visible = false;
            ItemsDataGrid.Columns["CoverID"].Visible = false;
            ItemsDataGrid.Columns["PatinaID"].Visible = false;
            ItemsDataGrid.Columns["MovementInvoiceID"].Visible = false;
            ItemsDataGrid.Columns["StoreItemID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("CurrencyTypeID"))
                ItemsDataGrid.Columns["CurrencyTypeID"].Visible = false;
            ItemsDataGrid.Columns["FactoryID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("PurchaseInvoiceID"))
                ItemsDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("IsArrived"))
                ItemsDataGrid.Columns["IsArrived"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("PriceEUR"))
                ItemsDataGrid.Columns["PriceEUR"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("ManufacturerID"))
                ItemsDataGrid.Columns["ManufacturerID"].Visible = false;
            if (ItemsDataGrid.Columns.Contains("BatchNumber"))
                ItemsDataGrid.Columns["BatchNumber"].Visible = false;
        }


        private void CheckStoreColumns(ref PercentageDataGrid tPercentageDataGrid)
        {
            foreach (DataGridViewColumn Column in tPercentageDataGrid.Columns)
            {
                foreach (DataGridViewRow Row in tPercentageDataGrid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        tPercentageDataGrid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        tPercentageDataGrid.Columns[Column.Index].Visible = false;
                }
            }

            if (tPercentageDataGrid.Columns.Contains("ReturnedRollerID"))
                tPercentageDataGrid.Columns["ReturnedRollerID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CreateUserID"))
                tPercentageDataGrid.Columns["CreateUserID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TechStoreSubGroupID"))
                tPercentageDataGrid.Columns["TechStoreSubGroupID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("DecorAssignmentID"))
                tPercentageDataGrid.Columns["DecorAssignmentID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PersonalStoreID"))
                tPercentageDataGrid.Columns["PersonalStoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ReadyStoreID"))
                tPercentageDataGrid.Columns["ReadyStoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ManufactureStoreID"))
                tPercentageDataGrid.Columns["ManufactureStoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("StoreID"))
                tPercentageDataGrid.Columns["StoreID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("WriteOffStoreID"))
                tPercentageDataGrid.Columns["WriteOffStoreID"].Visible = false;
            tPercentageDataGrid.Columns["ColorID"].Visible = false;
            tPercentageDataGrid.Columns["CoverID"].Visible = false;
            tPercentageDataGrid.Columns["PatinaID"].Visible = false;
            tPercentageDataGrid.Columns["MovementInvoiceID"].Visible = false;
            tPercentageDataGrid.Columns["StoreItemID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTypeID"))
                tPercentageDataGrid.Columns["CurrencyTypeID"].Visible = false;
            tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PurchaseInvoiceID"))
                tPercentageDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("IsArrived"))
                tPercentageDataGrid.Columns["IsArrived"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("Name"))
                tPercentageDataGrid.Columns["Name"].Visible = false;
        }

        public int GetCurrentStoreID()
        {
            int CurrentStoreID = -1;

            string StoreID = "StoreID";

            if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
                StoreID = "ManufactureStoreID";
            if (Parameters.SellerStoreAllocID == 9)
                StoreID = "PersonalStoreID";
            if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
                StoreID = "ReadyStoreID";

            if (ItemsDataGrid.SelectedRows.Count != 0 && ItemsDataGrid.SelectedRows[0].Cells[StoreID].Value != DBNull.Value)
                CurrentStoreID = Convert.ToInt32(ItemsDataGrid.SelectedRows[0].Cells[StoreID].Value);

            return CurrentStoreID;
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            int TechStoreGroupID = 0;
            int TechStoreSubGroupID = 0;
            if (GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value);
            if (SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

            int StoreIDFrom = GetCurrentStoreID();
            int CurrencyTypeID = 0;
            decimal Count = 0;
            decimal Price = -1;

            string PersonName = string.Empty;

            if (CountTextBox.Text.Length == 0 || Convert.ToDecimal(CountTextBox.Text) <= 0)
            {
                CountTextBox.Text = string.Empty;
                return;
            }

            Count = Convert.ToDecimal(CountTextBox.Text);

            if (ItemsDataGrid.SelectedRows.Count > 0 && ItemsDataGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value
                && Convert.ToDecimal(ItemsDataGrid.SelectedRows[0].Cells["CurrentCount"].Value) < Count)
            {
                CountTextBox.Clear();
                InfiniumTips.ShowTip(this, 50, 85, "Остаток меньше введенного количества", 1700);
                return;
            }

            if (PriceTextBox.Text.Length > 0)
                Price = Convert.ToDecimal(PriceTextBox.Text);

            CurrencyTypeID = Convert.ToInt32(CurrencyTypesComboBox.SelectedValue);

            int DestinationFactoryID = 1;
            bool bSummary = cbxSummProducts.Checked;

            if (StoreMovementManager.CurrentMovementInvoiceID != -1)
            {
                //MovementInvoices.SaveMovementInvoices(StoreMovementManager.CurrentMovementInvoiceID,
                //    Parameters.SellerStoreAllocID, Parameters.RecipientStoreAllocID, Parameters.RecipientSectorID,
                //    Parameters.PersonID, Parameters.PersonName, Parameters.StoreKeeperID, Parameters.ClientID, Parameters.SellerID, Parameters.ClientName, Parameters.Notes);
            }
            else
            {
                int LastMovementInvoiceID = MovementInvoices.SaveMovementInvoices(Convert.ToDateTime(Parameters.DateTime), Parameters.SellerStoreAllocID, Parameters.RecipientStoreAllocID,
                    Parameters.RecipientSectorID, Parameters.PersonID, Parameters.PersonName, Parameters.StoreKeeperID, Parameters.ClientID, Parameters.SellerID, Parameters.ClientName, Parameters.Notes);
                StoreMovementManager.CurrentMovementInvoiceID = LastMovementInvoiceID;
                MovementInvoices.CurrentMovementInvoiceID = LastMovementInvoiceID;
            }

            //со склада профиля или тпс
            if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
            {
                //на склад
                if (Parameters.RecipientStoreAllocID == 2 || Parameters.RecipientStoreAllocID == 1)
                {
                    StorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 2)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 1)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveBetweenStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int StoreID = StoreMovementManager.GetLastStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, StoreID, Count);
                    StoreMovementManager.UpdateStore();
                }
                //на склад производства
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                {
                    ManufactureStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 4)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 3)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromStoreToManufacture(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ManufactureStoreID = StoreMovementManager.GetLastManufactureStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ManufactureStoreID, Count);
                    StoreMovementManager.UpdateManufactureStore();
                }
                //на отгрузку
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                {
                    WriteOffStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 13)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 12)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromStoreToWriteOffStore(bSummary, StoreIDFrom, Price, CurrencyTypeID, Count);
                    int WriteOffStoreID = StoreMovementManager.GetLastWriteOffStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, WriteOffStoreID, Count);
                    StoreMovementManager.UpdateWriteOffStore();
                }
                //на Работника
                if (Parameters.RecipientStoreAllocID == 9)
                {
                    PersonalStorePanel.BringToFront();

                    bool Deduction = StoreMovementManager.MoveFromStoreToPersonal(bSummary, StoreIDFrom, StoreMovementManager.CurrentFactoryID, Count);
                    int PersonalStoreID = StoreMovementManager.GetLastPersonalStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, PersonalStoreID, Count);
                    StoreMovementManager.UpdatePersonalStore();
                }
                //на склад готовой продукции
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                {
                    ReadyStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 11)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 10)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromStoreToReadyStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ReadyStoreID = StoreMovementManager.GetLastReadyStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ReadyStoreID, Count);
                    StoreMovementManager.UpdateReadyStore();
                }
            }

            //со склада производства профиля или тпс
            if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
            {
                //на склад
                if (Parameters.RecipientStoreAllocID == 2 || Parameters.RecipientStoreAllocID == 1)
                {
                    StorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 2)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 1)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromManufactureToStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int StoreID = StoreMovementManager.GetLastStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, StoreID, Count);
                    StoreMovementManager.UpdateStore();
                }
                //на склад производства
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                {
                    ManufactureStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 4)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 3)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveBetweenManufactureStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ManufactureStoreID = StoreMovementManager.GetLastManufactureStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ManufactureStoreID, Count);
                    StoreMovementManager.UpdateManufactureStore();
                }
                //на отгрузку
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                {
                    WriteOffStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 13)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 12)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromManufactureToWriteOffStore(bSummary, StoreIDFrom, Price, CurrencyTypeID, Count);
                    int WriteOffStoreID = StoreMovementManager.GetLastWriteOffStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, WriteOffStoreID, Count);
                    StoreMovementManager.UpdateWriteOffStore();
                }
                //на Работника
                if (Parameters.RecipientStoreAllocID == 9)
                {
                    PersonalStorePanel.BringToFront();

                    bool Deduction = StoreMovementManager.MoveFromManufactureToPersonal(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int PersonalStoreID = StoreMovementManager.GetLastPersonalStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, PersonalStoreID, Count);
                    StoreMovementManager.UpdatePersonalStore();
                }
                //на склад готовой продукции
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                {
                    ReadyStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 11)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 10)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromManufactureToReadyStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ReadyStoreID = StoreMovementManager.GetLastReadyStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ReadyStoreID, Count);
                    StoreMovementManager.UpdateReadyStore();
                }
            }
            if (Parameters.SellerStoreAllocID == 9)
            {
                //на склад
                if (Parameters.RecipientStoreAllocID == 2 || Parameters.RecipientStoreAllocID == 1)
                {
                    StorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 2)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 1)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromPersonalToStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int StoreID = StoreMovementManager.GetLastStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, StoreID, Count);
                    StoreMovementManager.UpdateStore();
                }
                //на склад производства
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                {
                    ManufactureStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 4)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 3)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromPersonalToManufacture(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ManufactureStoreID = StoreMovementManager.GetLastManufactureStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ManufactureStoreID, Count);
                    StoreMovementManager.UpdateManufactureStore();
                }
                //на отгрузку
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                {
                    WriteOffStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 13)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 12)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromPersonalToWriteOffStore(bSummary, StoreIDFrom, Price, CurrencyTypeID, Count);
                    int WriteOffStoreID = StoreMovementManager.GetLastWriteOffStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, WriteOffStoreID, Count);
                    StoreMovementManager.UpdateWriteOffStore();
                }
                //на Работника
                if (Parameters.RecipientStoreAllocID == 9)
                {
                    PersonalStorePanel.BringToFront();

                    bool Deduction = StoreMovementManager.MoveBetweenPersonalStore(bSummary, StoreIDFrom, StoreMovementManager.CurrentFactoryID, Count);
                    int PersonalStoreID = StoreMovementManager.GetLastPersonalStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, PersonalStoreID, Count);
                    StoreMovementManager.UpdatePersonalStore();
                }
                //на склад готовой продукции
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                {
                    ReadyStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 11)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 10)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromPersonalToReadyStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ReadyStoreID = StoreMovementManager.GetLastReadyStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ReadyStoreID, Count);
                    StoreMovementManager.UpdateReadyStore();
                }
            }
            //со склада готовой продукции профиля или тпс
            if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
            {
                //на склад
                if (Parameters.RecipientStoreAllocID == 2 || Parameters.RecipientStoreAllocID == 1)
                {
                    StorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 2)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 1)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromReadyStoreToStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int StoreID = StoreMovementManager.GetLastStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, StoreID, Count);
                    StoreMovementManager.UpdateStore();
                }
                //на склад производства
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                {
                    ManufactureStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 4)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 3)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromReadyStoreToManufacture(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ManufactureStoreID = StoreMovementManager.GetLastManufactureStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ManufactureStoreID, Count);
                    StoreMovementManager.UpdateManufactureStore();
                }
                //на отгрузку
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                {
                    WriteOffStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 13)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 12)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveFromReadyStoreToWriteOffStore(bSummary, StoreIDFrom, Price, CurrencyTypeID, Count);
                    int WriteOffStoreID = StoreMovementManager.GetLastWriteOffStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, WriteOffStoreID, Count);
                    StoreMovementManager.UpdateWriteOffStore();
                }
                //на Работника
                if (Parameters.RecipientStoreAllocID == 9)
                {
                    PersonalStorePanel.BringToFront();

                    bool Deduction = StoreMovementManager.MoveFromReadyStoreToPersonal(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int PersonalStoreID = StoreMovementManager.GetLastPersonalStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, PersonalStoreID, Count);
                    StoreMovementManager.UpdatePersonalStore();
                }
                //на склад готовой продукции
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                {
                    ReadyStorePanel.BringToFront();

                    if (Parameters.RecipientStoreAllocID == 11)
                        DestinationFactoryID = 2;
                    if (Parameters.RecipientStoreAllocID == 10)
                        DestinationFactoryID = 1;

                    bool Deduction = StoreMovementManager.MoveBetweenReadyStore(bSummary, StoreIDFrom, DestinationFactoryID, Count);
                    int ReadyStoreID = StoreMovementManager.GetLastReadyStoreID();
                    if (Deduction)
                        MovementInvoices.AddMovementInvoiceDetail(bSummary, StoreIDFrom, ReadyStoreID, Count);
                    StoreMovementManager.UpdateReadyStore();
                }
            }
            CheckStoreColumns(ref StoreDataGrid);
            CheckStoreColumns(ref ReadyStoreDataGrid);
            CheckStoreColumns(ref ManufactureStoreDataGrid);
            CheckStoreColumns(ref WriteOffStoreDataGrid);
            CheckStoreColumns(ref PersonalStoreDataGrid);
            InfiniumTips.ShowTip(this, 50, 85, "Позиция добавлена", 1700);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!MovementInvoices.DBConnectionStatus())
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            try
            {
                int TechStoreGroupID = 0;
                int TechStoreSubGroupID = 0;
                if (GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                    TechStoreGroupID = Convert.ToInt32(GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value);
                if (SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                MovementInvoices.SaveMovementInvoices(StoreMovementManager.CurrentMovementInvoiceID,
                    Parameters.SellerStoreAllocID, Parameters.RecipientStoreAllocID, Parameters.RecipientSectorID,
                    Parameters.PersonID, Parameters.PersonName, Parameters.StoreKeeperID, Parameters.ClientID, Parameters.SellerID, Parameters.ClientName, Parameters.Notes);

                if (Parameters.SellerStoreAllocID == 9)
                {
                    StoreMovementManager.UpdateStoreGroups(true, Parameters.PersonID);
                    StoreMovementManager.UpdateStoreSubGroups(true, Parameters.PersonID);
                    StoreMovementManager.UpdateStoreItems(true, Parameters.PersonID);
                }
                else
                {
                    StoreMovementManager.UpdateStoreGroups(false, Security.CurrentUserID);
                    StoreMovementManager.UpdateStoreSubGroups(false, Security.CurrentUserID);
                    StoreMovementManager.UpdateStoreItems(false, Security.CurrentUserID);
                }
                StoreMovementManager.MoveToStoreGroup(TechStoreGroupID);
                StoreMovementManager.MoveToStoreSubGroup(TechStoreSubGroupID);
                InfiniumTips.ShowTip(this, 50, 85, "Сохранение выполнено", 1700);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". ПОВТОРИТЕ СОХРАНЕНИЕ");
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Позиция будет удалена. Продолжить?",
                    "Редактирование накладной");

            if (OKCancel)
            {
                int TechStoreGroupID = 0;
                int TechStoreSubGroupID = 0;
                if (GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                    TechStoreGroupID = Convert.ToInt32(GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value);
                if (SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                    TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

                decimal Count = 0;
                int StoreIDFrom = 0;
                int StoreIDTo = 0;
                if (Parameters.RecipientStoreAllocID == 1 || Parameters.RecipientStoreAllocID == 2)
                {
                    if (StoreDataGrid.SelectedRows.Count > 0 && StoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                        Count = Convert.ToDecimal(StoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                    if (StoreDataGrid.SelectedRows.Count > 0 && StoreDataGrid.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                        StoreIDFrom = Convert.ToInt32(StoreDataGrid.SelectedRows[0].Cells["Name"].Value);
                    if (StoreDataGrid.SelectedRows.Count > 0 && StoreDataGrid.SelectedRows[0].Cells[0].Value != DBNull.Value)
                        StoreIDTo = Convert.ToInt32(StoreDataGrid.SelectedRows[0].Cells[0].Value);
                    if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
                        StoreMovementManager.ReturnToStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
                        StoreMovementManager.ReturnToManufactureStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 12 || Parameters.SellerStoreAllocID == 13)
                    {
                    }
                    if (Parameters.SellerStoreAllocID == 9)
                        StoreMovementManager.ReturnToPersonalStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
                        StoreMovementManager.ReturnToReadyStore(StoreIDFrom, Count);
                    StoreMovementManager.RemoveStoreItem(StoreIDTo);
                    StoreMovementManager.UpdateStore();
                }
                if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
                {
                    if (ManufactureStoreDataGrid.SelectedRows.Count > 0 && ManufactureStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                        Count = Convert.ToDecimal(ManufactureStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                    if (ManufactureStoreDataGrid.SelectedRows.Count > 0 && ManufactureStoreDataGrid.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                        StoreIDFrom = Convert.ToInt32(ManufactureStoreDataGrid.SelectedRows[0].Cells["Name"].Value);
                    if (ManufactureStoreDataGrid.SelectedRows.Count > 0 && ManufactureStoreDataGrid.SelectedRows[0].Cells[0].Value != DBNull.Value)
                        StoreIDTo = Convert.ToInt32(ManufactureStoreDataGrid.SelectedRows[0].Cells[0].Value);
                    if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
                        StoreMovementManager.ReturnToStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
                        StoreMovementManager.ReturnToManufactureStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 12 || Parameters.SellerStoreAllocID == 13)
                    {
                    }
                    if (Parameters.SellerStoreAllocID == 9)
                        StoreMovementManager.ReturnToPersonalStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
                        StoreMovementManager.ReturnToReadyStore(StoreIDFrom, Count);
                    StoreMovementManager.RemoveManufactureStoreItem(StoreIDTo);
                    StoreMovementManager.UpdateManufactureStore();
                }
                if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
                {
                    if (WriteOffStoreDataGrid.SelectedRows.Count > 0 && WriteOffStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                        Count = Convert.ToDecimal(WriteOffStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                    if (WriteOffStoreDataGrid.SelectedRows.Count > 0 && WriteOffStoreDataGrid.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                        StoreIDFrom = Convert.ToInt32(WriteOffStoreDataGrid.SelectedRows[0].Cells["Name"].Value);
                    if (WriteOffStoreDataGrid.SelectedRows.Count > 0 && WriteOffStoreDataGrid.SelectedRows[0].Cells[0].Value != DBNull.Value)
                        StoreIDTo = Convert.ToInt32(WriteOffStoreDataGrid.SelectedRows[0].Cells[0].Value);
                    if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
                        StoreMovementManager.ReturnToStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
                        StoreMovementManager.ReturnToManufactureStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 12 || Parameters.SellerStoreAllocID == 13)
                    {
                    }
                    if (Parameters.SellerStoreAllocID == 9)
                        StoreMovementManager.ReturnToPersonalStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
                        StoreMovementManager.ReturnToReadyStore(StoreIDFrom, Count);
                    StoreMovementManager.RemoveWriteOffStoreItem(StoreIDTo);
                    StoreMovementManager.UpdateWriteOffStore();
                }
                if (Parameters.RecipientStoreAllocID == 9)
                {
                    if (PersonalStoreDataGrid.SelectedRows.Count > 0 && PersonalStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                        Count = Convert.ToDecimal(PersonalStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                    if (PersonalStoreDataGrid.SelectedRows.Count > 0 && PersonalStoreDataGrid.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                        StoreIDFrom = Convert.ToInt32(PersonalStoreDataGrid.SelectedRows[0].Cells["Name"].Value);
                    if (PersonalStoreDataGrid.SelectedRows.Count > 0 && PersonalStoreDataGrid.SelectedRows[0].Cells[0].Value != DBNull.Value)
                        StoreIDTo = Convert.ToInt32(PersonalStoreDataGrid.SelectedRows[0].Cells[0].Value);
                    if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
                        StoreMovementManager.ReturnToStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
                        StoreMovementManager.ReturnToManufactureStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 12 || Parameters.SellerStoreAllocID == 13)
                    {
                    }
                    if (Parameters.SellerStoreAllocID == 9)
                        StoreMovementManager.ReturnToPersonalStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
                        StoreMovementManager.ReturnToReadyStore(StoreIDFrom, Count);
                    StoreMovementManager.RemovePersonalStoreItem(StoreIDTo);
                    StoreMovementManager.UpdatePersonalStore();
                }
                if (Parameters.RecipientStoreAllocID == 10 || Parameters.RecipientStoreAllocID == 11)
                {
                    if (ReadyStoreDataGrid.SelectedRows.Count > 0 && ReadyStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                        Count = Convert.ToDecimal(ReadyStoreDataGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                    if (ReadyStoreDataGrid.SelectedRows.Count > 0 && ReadyStoreDataGrid.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                        StoreIDFrom = Convert.ToInt32(ReadyStoreDataGrid.SelectedRows[0].Cells["Name"].Value);
                    if (ReadyStoreDataGrid.SelectedRows.Count > 0 && ReadyStoreDataGrid.SelectedRows[0].Cells[0].Value != DBNull.Value)
                        StoreIDTo = Convert.ToInt32(ReadyStoreDataGrid.SelectedRows[0].Cells[0].Value);
                    if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 2)
                        StoreMovementManager.ReturnToStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 4)
                        StoreMovementManager.ReturnToManufactureStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 12 || Parameters.SellerStoreAllocID == 13)
                    {
                    }
                    if (Parameters.SellerStoreAllocID == 9)
                        StoreMovementManager.ReturnToPersonalStore(StoreIDFrom, Count);
                    if (Parameters.SellerStoreAllocID == 10 || Parameters.SellerStoreAllocID == 11)
                        StoreMovementManager.ReturnToReadyStore(StoreIDFrom, Count);
                    StoreMovementManager.RemoveReadyStoreItem(StoreIDTo);
                    StoreMovementManager.UpdateReadyStore();
                }

                MovementInvoices.RemoveDetails(StoreIDFrom, StoreIDTo, Count);
                MovementInvoices.SaveMovementInvoiceDetails(MovementInvoices.CurrentMovementInvoiceID);
                CheckStoreColumns(ref StoreDataGrid);
                CheckStoreColumns(ref ReadyStoreDataGrid);
                CheckStoreColumns(ref ManufactureStoreDataGrid);
                CheckStoreColumns(ref WriteOffStoreDataGrid);
                CheckStoreColumns(ref PersonalStoreDataGrid);

                InfiniumTips.ShowTip(this, 50, 85, "Позиция удалена", 1700);

                //if (Parameters.SellerStoreAllocID == 9)
                //{
                //    StoreMovementManager.UpdateStoreGroups(true, Parameters.PersonID);
                //    StoreMovementManager.UpdateStoreSubGroups(true, Parameters.PersonID);
                //    StoreMovementManager.UpdateStoreItems(true, Parameters.PersonID);
                //}
                //else
                //{
                //    StoreMovementManager.UpdateStoreGroups(false, Security.CurrentUserID);
                //    StoreMovementManager.UpdateStoreSubGroups(false, Security.CurrentUserID);
                //    StoreMovementManager.UpdateStoreItems(false, Security.CurrentUserID);
                //}
                //StoreMovementManager.MoveToStoreGroup(TechStoreGroupID);
                //StoreMovementManager.MoveToStoreSubGroup(TechStoreSubGroupID);
            }
        }

        private void SubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreMovementManager == null || SubGroupsDataGrid.SelectedRows.Count == 0)
                return;
            int TechStoreSubGroupID = 0;
            if (SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

            StoreMovementManager.FilterItems(TechStoreSubGroupID, StoreMovementManager.CurrentFactoryID);

            CheckItemsColumns();
        }

        private void HeightTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void WidthTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void LengthTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ThicknessTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void DiameterTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void AdmissionTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void WeightTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void CountTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void StoreDataGrid_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void TabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
        }

        public void MoveToClientID(int ClientID)
        {
            //if (((DataRowView)StoreAllocToBindingSource.Current == null))
            //    return;

            //StoreAllocToBindingSource.Position = StoreAllocToBindingSource.Find("ClientID", ClientID);
        }

        public void MoveToStoreAlloc(int RecipientStoreAllocID)
        {
            //if (((DataRowView)StoreAllocToBindingSource.Current == null))
            //    return;

            //StoreAllocToBindingSource.Position = StoreAllocToBindingSource.Find("StoreAllocID", RecipientStoreAllocID);
        }

        public void MoveToClient(string ClientName)
        {
            //if (((DataRowView)ClientsBS.Current == null))
            //    return;

            //ClientsBS.Position = ClientsBS.Find("ClientName", ClientName);
        }

        private void SetAutoComplete(ComponentFactory.Krypton.Toolkit.KryptonComboBox ComboBox, bool Autocomplete)
        {
            if (Autocomplete)
            {
                if (ComboBox.Focused)
                {
                    ComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    ComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                }
            }
            else
            {
                ComboBox.AutoCompleteMode = AutoCompleteMode.None;
                ComboBox.AutoCompleteSource = AutoCompleteSource.None;
            }
        }

        private void CountTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddItemButton_Click(null, null);
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void ItemsDataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //ItemsDataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
        }

        private void SectorsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
