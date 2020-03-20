using Infinium.Store;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewArrivalForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        int PurchaseInvoiceID = -1;

        Form TopForm = null;

        StoreInvoiceManager StoreInvoiceManager;

        //BindingSource GroupsBS;
        //BindingSource SubGroupsBS;
        //BindingSource ItemsBS;

        public NewArrivalForm()
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        public NewArrivalForm(int iPurchaseInvoiceID)
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            StoreInvoiceManager.EditInvoice(iPurchaseInvoiceID);

            EditData(iPurchaseInvoiceID);

            PurchaseInvoiceID = iPurchaseInvoiceID;

            while (!SplashForm.bCreated) ;
        }

        DataGridViewComboBoxColumn MeasuresColumn1;
        DataGridViewComboBoxColumn ColorsColumn1;
        DataGridViewComboBoxColumn CoversColumn1;
        DataGridViewComboBoxColumn PatinaColumn1;

        DataGridViewComboBoxColumn ColorsColumn;
        DataGridViewComboBoxColumn CoversColumn;
        DataGridViewComboBoxColumn PatinaColumn;
        DataGridViewComboBoxColumn StoreItemColumn;
        DataGridViewComboBoxColumn CurrencyColumn;

        private void EditData(int PurchaseInvoiceID)
        {
            string DocNumber = string.Empty;
            string Reason = string.Empty;
            int SellerID = -1;
            int FactoryID = -1;
            DateTime IncomeDate = DateTime.Now;
            string Notes = string.Empty;

            StoreInvoiceManager.GetInvoiceParams(PurchaseInvoiceID, ref DocNumber, ref SellerID, ref FactoryID, ref Reason, ref IncomeDate, ref Notes);

            InvoiceDocNumberTextBox.Text = DocNumber;
            InvoiceReasonTextBox.Text = Reason;
            ArrivalDateTimePicker.Value = IncomeDate;
            NotesRichTextBox.Text = Notes;
            SellerComboBox.SelectedValue = SellerID;
            FactoryIDComboBox.SelectedValue = FactoryID;

            CheckStoreColumns();
        }

        private void Initialize()
        {
            StoreInvoiceManager = new StoreInvoiceManager(ref GroupsDataGrid,
                ref SubGroupsDataGrid, ref StoreDataGrid);

            cbManufacturers.DataSource = StoreInvoiceManager.ManufacturersList;
            cbManufacturers.DisplayMember = "SellerName";
            cbManufacturers.ValueMember = "SellerID";

            SellerComboBox.DataSource = new DataView(StoreInvoiceManager.SellersDataTable);
            SellerComboBox.DisplayMember = "SellerName";
            SellerComboBox.ValueMember = "SellerID";

            FactoryIDComboBox.DataSource = new DataView(StoreInvoiceManager.FactoryDataTable);
            FactoryIDComboBox.DisplayMember = "FactoryName";
            FactoryIDComboBox.ValueMember = "FactoryID";

            CurrencyTypesComboBox.DataSource = new DataView(StoreInvoiceManager.CurrencyTypesDataTable);
            CurrencyTypesComboBox.DisplayMember = "CurrencyType";
            CurrencyTypesComboBox.ValueMember = "CurrencyTypeID";

            ColorsComboBox.DataSource = new DataView(StoreInvoiceManager.ColorsDT);
            ColorsComboBox.DisplayMember = "ColorName";
            ColorsComboBox.ValueMember = "ColorID";

            CoversComboBox.DataSource = new DataView(StoreInvoiceManager.CoversDataTable);
            CoversComboBox.DisplayMember = "CoverName";
            CoversComboBox.ValueMember = "CoverID";

            PatinaComboBox.DataSource = new DataView(StoreInvoiceManager.PatinaDataTable);
            PatinaComboBox.DisplayMember = "PatinaName";
            PatinaComboBox.ValueMember = "PatinaID";

            GroupsDataGrid.DataSource = StoreInvoiceManager.GroupsList;

            SubGroupsDataGrid.DataSource = StoreInvoiceManager.SubGroupsList;

            ItemsDataGrid.DataSource = StoreInvoiceManager.StoreItemsList;

            //fields initialize:
            for (int i = 0; i < 10; i++)
                ShowColumn(i, false);

            HeightTextBox.Text = string.Empty;
            WidthTextBox.Text = string.Empty;
            LengthTextBox.Text = string.Empty;
            ThicknessTextBox.Text = string.Empty;
            DiameterTextBox.Text = string.Empty;
            AdmissionTextBox.Text = string.Empty;
            CapacityTextBox.Text = string.Empty;
            WeightTextBox.Text = string.Empty;
            ColorsComboBox.SelectedIndex = 0;
            CoversComboBox.SelectedIndex = 0;
            PatinaComboBox.SelectedIndex = 0;
            CountTextBox.Text = string.Empty;
            /////////////////////////////////////////////

            GridSettings();
            GroupsDataGrid.SelectionChanged -= GroupsDataGrid_SelectionChanged;
            SubGroupsDataGrid.SelectionChanged -= SubGroupsDataGrid_SelectionChanged;
        }

        private void GridSettings()
        {
            GroupsDataGrid.Columns["TechStoreGroupID"].Visible = false;

            SubGroupsDataGrid.Columns["TechStoreSubGroupID"].Visible = false;
            SubGroupsDataGrid.Columns["TechStoreGroupID"].Visible = false;
            SubGroupsDataGrid.Columns["Notes"].Visible = false;
            SubGroupsDataGrid.Columns["Notes1"].Visible = false;
            SubGroupsDataGrid.Columns["Notes2"].Visible = false;

            MeasuresColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "MeasuresColumn",
                HeaderText = "Ед. измерения",
                DataPropertyName = "MeasureID",
                DataSource = StoreInvoiceManager.MeasuresDataTable,
                ValueMember = "MeasureID",
                DisplayMember = "Measure",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ColorsColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "ColorsColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",
                DataSource = StoreInvoiceManager.ColorsDT,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            CoversColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "CoversColumn",
                HeaderText = "Облицовка",
                DataPropertyName = "CoverID",
                DataSource = StoreInvoiceManager.CoversDataTable,
                ValueMember = "CoverID",
                DisplayMember = "CoverName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = StoreInvoiceManager.PatinaDataTable,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ColorsColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",
                DataSource = StoreInvoiceManager.ColorsDT,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            CoversColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CoversColumn",
                HeaderText = "Облицовка",
                DataPropertyName = "CoverID",
                DataSource = StoreInvoiceManager.CoversDataTable,
                ValueMember = "CoverID",
                DisplayMember = "CoverName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = StoreInvoiceManager.PatinaDataTable,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            CurrencyColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CurrencyColumn",
                HeaderText = "Валюта",
                DataPropertyName = "CurrencyTypeID",
                DataSource = StoreInvoiceManager.CurrencyTypesDataTable,
                ValueMember = "CurrencyTypeID",
                DisplayMember = "CurrencyType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            StoreItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "StoreItemColumn",
                HeaderText = "Наименование",
                DataPropertyName = "StoreItemID",
                DataSource = StoreInvoiceManager.StoreItemsDataTable,
                ValueMember = "TechStoreID",
                DisplayMember = "TechStoreName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn ProducedColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                CalendarTodayDate = DateTime.Now,
                Checked = false,
                DataPropertyName = "Produced",
                HeaderText = "Произведено",
                Name = "ProducedColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn BestBeforeColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                CalendarTodayDate = DateTime.Now,
                Checked = false,
                DataPropertyName = "BestBefore",
                HeaderText = "Срок годности",
                Name = "BestBeforeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ItemsDataGrid.Columns.Add(MeasuresColumn1);
            ItemsDataGrid.Columns.Add(ColorsColumn1);
            ItemsDataGrid.Columns.Add(PatinaColumn1);
            ItemsDataGrid.Columns.Add(CoversColumn1);

            StoreDataGrid.Columns.Add(ProducedColumn);
            StoreDataGrid.Columns.Add(BestBeforeColumn);
            StoreDataGrid.Columns.Add(ColorsColumn);
            StoreDataGrid.Columns.Add(PatinaColumn);
            StoreDataGrid.Columns.Add(CoversColumn);
            //StoreDataGrid.Columns.Add(StoreItemColumn);
            StoreDataGrid.Columns.Add(CurrencyColumn);
            StoreDataGrid.Columns.Add(StoreInvoiceManager.ManufacturerColumn);

            ItemsDataGrid.Columns["TechStoreSubGroupID"].Visible = false;
            ItemsDataGrid.Columns["TechStoreID"].Visible = false;
            ItemsDataGrid.Columns["MeasureID"].Visible = false;
            ItemsDataGrid.Columns["ColorID"].Visible = false;
            ItemsDataGrid.Columns["CoverID"].Visible = false;
            ItemsDataGrid.Columns["PatinaID"].Visible = false;

            ItemsDataGrid.Columns["TechStoreName"].HeaderText = "Название";
            ItemsDataGrid.Columns["SellerCode"].HeaderText = "Кодировка поставщика";
            ItemsDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            ItemsDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            ItemsDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            ItemsDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            ItemsDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            ItemsDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            ItemsDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            ItemsDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            ItemsDataGrid.Columns["Notes"].HeaderText = "Примечание";

            ItemsDataGrid.AutoGenerateColumns = false;
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
            ItemsDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["MeasuresColumn"].DisplayIndex = DisplayIndex++;
            ItemsDataGrid.Columns["SellerCode"].DisplayIndex = DisplayIndex++;

            StoreDataGrid.AutoGenerateColumns = false;

            StoreDataGrid.Columns["TechStoreName"].ReadOnly = true;

            StoreDataGrid.Columns["StoreID"].Visible = false;
            StoreDataGrid.Columns["ColorID"].Visible = false;
            StoreDataGrid.Columns["CoverID"].Visible = false;
            StoreDataGrid.Columns["PatinaID"].Visible = false;
            StoreDataGrid.Columns["StoreItemID"].Visible = false;
            StoreDataGrid.Columns["CurrencyTypeID"].Visible = false;
            StoreDataGrid.Columns["FactoryID"].Visible = false;
            StoreDataGrid.Columns["IsArrived"].Visible = false;
            StoreDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            StoreDataGrid.Columns["PriceEUR"].Visible = false;
            StoreDataGrid.Columns["Produced"].Visible = false;
            StoreDataGrid.Columns["BestBefore"].Visible = false;
            StoreDataGrid.Columns["DecorAssignmentID"].Visible = false;
            StoreDataGrid.Columns["ManufacturerID"].Visible = false;
            StoreDataGrid.Columns["MovementInvoiceID"].Visible = false;

            StoreDataGrid.Columns["TechStoreName"].HeaderText = "Наименование";
            StoreDataGrid.Columns["Length"].HeaderText = "Длина, мм";
            StoreDataGrid.Columns["Width"].HeaderText = "Ширина, мм";
            StoreDataGrid.Columns["Height"].HeaderText = "Высота, мм";
            StoreDataGrid.Columns["Thickness"].HeaderText = "Толщина, мм";
            StoreDataGrid.Columns["Diameter"].HeaderText = "Диаметр, мм";
            StoreDataGrid.Columns["Admission"].HeaderText = "Допуск, мм";
            StoreDataGrid.Columns["Weight"].HeaderText = "Вес, кг";
            StoreDataGrid.Columns["Capacity"].HeaderText = "Емкость, л";
            StoreDataGrid.Columns["Notes"].HeaderText = "Примечание";
            StoreDataGrid.Columns["Price"].HeaderText = "Цена";
            StoreDataGrid.Columns["VAT"].HeaderText = "НДС";
            StoreDataGrid.Columns["Cost"].HeaderText = "Сумма";
            StoreDataGrid.Columns["VATCost"].HeaderText = "Сумма с НДС";
            StoreDataGrid.Columns["InvoiceCount"].HeaderText = "Прих. кол-во";
            StoreDataGrid.Columns["CurrentCount"].HeaderText = "Остаток";
            StoreDataGrid.Columns["BatchNumber"].HeaderText = "№ партии";
            StoreDataGrid.Columns["Produced"].HeaderText = "Произведено";
            StoreDataGrid.Columns["BestBefore"].HeaderText = "Срок годности";
            StoreDataGrid.Columns["CreateDateTime"].HeaderText = "Дата создания";

            DisplayIndex = 0;
            StoreDataGrid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Admission"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["InvoiceCount"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Price"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["VAT"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["VATCost"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["CurrencyColumn"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["ManufacturerColumn"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["BatchNumber"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["ProducedColumn"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["BestBeforeColumn"].DisplayIndex = DisplayIndex++;
            StoreDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            StoreDataGrid.Columns["Price"].DefaultCellStyle.Format = "N";
            StoreDataGrid.Columns["Price"].DefaultCellStyle.FormatProvider = nfi1;
            StoreDataGrid.Columns["VAT"].DefaultCellStyle.Format = "N";
            StoreDataGrid.Columns["VAT"].DefaultCellStyle.FormatProvider = nfi1;
            StoreDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            StoreDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            StoreDataGrid.Columns["VATCost"].DefaultCellStyle.Format = "N";
            StoreDataGrid.Columns["VATCost"].DefaultCellStyle.FormatProvider = nfi1;

            StoreDataGrid.Columns["BatchNumber"].MinimumWidth = 60;
            StoreDataGrid.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            StoreDataGrid.Columns["Produced"].MinimumWidth = 60;
            StoreDataGrid.Columns["Produced"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            StoreDataGrid.Columns["BestBefore"].MinimumWidth = 60;
            StoreDataGrid.Columns["BestBefore"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void ShowColumn(int index, bool Show)
        {
            if (!Show)
            {
                switch (index)
                {
                    case 0: panel8.Visible = false; break;
                    case 1: panel10.Visible = false; break;
                    case 2: panel3.Visible = false; break;
                    case 3: panel4.Visible = false; break;
                    case 4: panel5.Visible = false; break;
                    case 5: panel6.Visible = false; break;
                    case 6: panel7.Visible = false; break;
                    case 7: panel15.Visible = false; break;
                    case 8: panel2.Visible = false; break;
                    case 9: panel1.Visible = false; break;
                    case 10: panel11.Visible = false; break;
                    case 11: panel12.Visible = false; break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0: panel8.Visible = true; break;
                    case 1: panel10.Visible = true; break;
                    case 2: panel3.Visible = true; break;
                    case 3: panel4.Visible = true; break;
                    case 4: panel5.Visible = true; break;
                    case 5: panel6.Visible = true; break;
                    case 6: panel7.Visible = true; break;
                    case 7: panel15.Visible = true; break;
                    case 8: panel2.Visible = true; break;
                    case 9: panel1.Visible = true; break;
                    case 10: panel11.Visible = true; break;
                    case 11: panel12.Visible = true; break;
                }
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

        private void NewArrivalForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            if (StoreInvoiceManager == null)
                return;

            decimal SummaryCost = 0;
            GetSummaryCost(ref SummaryCost);
            if (SummaryCost > 0)
            {
                NumberFormatInfo nfi1 = new NumberFormatInfo()
                {
                    NumberGroupSeparator = " ",
                    CurrencySymbol = "",
                    NumberDecimalDigits = 2,
                    NumberDecimalSeparator = ","
                };
                SummaryCostLabel.Text = "Общая сумма: " + SummaryCost.ToString("N", nfi1);
                SummaryCostLabel.Visible = true;
            }
            else
                SummaryCostLabel.Visible = false;
        }

        public bool GetSummaryCost(ref decimal SummaryCost)
        {
            for (int i = 0; i < StoreDataGrid.Rows.Count; i++)
            {
                if (StoreDataGrid.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    SummaryCost += Convert.ToDecimal(StoreDataGrid.Rows[i].Cells["Cost"].Value);
            }

            SummaryCost = Decimal.Round(SummaryCost, 3, MidpointRounding.AwayFromZero);

            return SummaryCost > 0;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            bool OkCancel = Infinium.LightMessageBox.Show(ref TopForm, true, "Точно выйти? Все действия сохранены?", "Внимание");
            if (!OkCancel)
                return;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void GroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreInvoiceManager == null)
                return;
            if (GroupsDataGrid.SelectedRows.Count == 0)
                return;
            int TechStoreGroupID = 0;
            if (GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(GroupsDataGrid.SelectedRows[0].Cells["TechStoreGroupID"].Value);
            StoreInvoiceManager.FilterSubGroups(TechStoreGroupID);
        }

        private void ItemsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreInvoiceManager == null)
                return;
            if (ItemsDataGrid.SelectedRows.Count == 0)
                return;
            int TechStoreID = 0;
            if (ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            DataTable DT = StoreInvoiceManager.FilterParams(TechStoreID);

            if (DT == null)
                return;

            flowLayoutPanel1.SuspendLayout();
            for (int i = 0; i < 11; i++)
                ShowColumn(i, false);

            for (int i = 0; i < 11; i++)
                ShowColumn(i, true);

            for (int i = 0; i < 11; i++)
                ShowColumn(i, false);
            HeightTextBox.Text = string.Empty;
            WidthTextBox.Text = string.Empty;
            LengthTextBox.Text = string.Empty;
            ThicknessTextBox.Text = string.Empty;
            DiameterTextBox.Text = string.Empty;
            AdmissionTextBox.Text = string.Empty;
            CapacityTextBox.Text = string.Empty;
            WeightTextBox.Text = string.Empty;
            if (ColorsComboBox.Items.Count > 0)
                ColorsComboBox.SelectedIndex = 0;
            if (PatinaComboBox.Items.Count > 0)
                PatinaComboBox.SelectedIndex = 0;
            if (CoversComboBox.Items.Count > 0)
                CoversComboBox.SelectedIndex = 0;
            CountTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Param"].ToString() == "Diameter")
                {
                    ShowColumn(0, true);
                }
                if (Row["Param"].ToString() == "Capacity")
                {
                    ShowColumn(1, true);
                }
                if (Row["Param"].ToString() == "Thickness")
                {
                    ShowColumn(2, true);
                }
                if (Row["Param"].ToString() == "Length")
                {
                    ShowColumn(3, true);
                }
                if (Row["Param"].ToString() == "Height")
                {
                    ShowColumn(4, true);
                }
                if (Row["Param"].ToString() == "Width")
                {
                    ShowColumn(5, true);
                }
                if (Row["Param"].ToString() == "Admission")
                {
                    ShowColumn(6, true);
                }
                if (Row["Param"].ToString() == "CoverID")
                {
                    ShowColumn(7, true);
                }
                if (Row["Param"].ToString() == "PatinaID")
                {
                    ShowColumn(8, true);
                }
                if (Row["Param"].ToString() == "ColorID")
                {
                    ShowColumn(9, true);
                }
                if (Row["Param"].ToString() == "Weight")
                {
                    ShowColumn(10, true);
                }
                if (Row["Param"].ToString() == "Count")
                {
                    ShowColumn(11, true);
                }
            }

            int ColorsGroupID = StoreInvoiceManager.FilterColors(TechStoreID);
            if (ColorsGroupID == -1)
                ((DataView)ColorsComboBox.DataSource).RowFilter = "GroupID = 0 OR GroupID = -1";
            else
            {
                if (ColorsGroupID == 19 || ColorsGroupID == 18 || ColorsGroupID == 17 || ColorsGroupID == 16)
                    ((DataView)ColorsComboBox.DataSource).RowFilter = "GroupID = 3";
                else
                    ((DataView)ColorsComboBox.DataSource).RowFilter = "GroupID = " + ColorsGroupID;
            }
            flowLayoutPanel1.ResumeLayout();
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

            ItemsDataGrid.Columns["TechStoreSubGroupID"].Visible = false;
            ItemsDataGrid.Columns["TechStoreID"].Visible = false;
            ItemsDataGrid.Columns["MeasureID"].Visible = false;
            ItemsDataGrid.Columns["ColorID"].Visible = false;
            ItemsDataGrid.Columns["CoverID"].Visible = false;
            ItemsDataGrid.Columns["PatinaID"].Visible = false;
        }


        private void CheckStoreColumns()
        {
            foreach (DataGridViewColumn Column in StoreDataGrid.Columns)
            {
                foreach (DataGridViewRow Row in StoreDataGrid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        StoreDataGrid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        StoreDataGrid.Columns[Column.Index].Visible = false;
                }
            }

            StoreDataGrid.Columns["ManufacturerColumn"].Visible = true;
            StoreDataGrid.Columns["Notes"].Visible = true;
            StoreDataGrid.Columns["BestBeforeColumn"].Visible = true;
            StoreDataGrid.Columns["ProducedColumn"].Visible = true;
            StoreDataGrid.Columns["BestBefore"].Visible = false;
            StoreDataGrid.Columns["BatchNumber"].Visible = true;
            StoreDataGrid.Columns["Produced"].Visible = false;
            StoreDataGrid.Columns["StoreID"].Visible = false;
            StoreDataGrid.Columns["ColorID"].Visible = false;
            StoreDataGrid.Columns["CoverID"].Visible = false;
            StoreDataGrid.Columns["PatinaID"].Visible = false;
            StoreDataGrid.Columns["PurchaseInvoiceID"].Visible = false;
            StoreDataGrid.Columns["MovementInvoiceID"].Visible = false;
            StoreDataGrid.Columns["StoreItemID"].Visible = false;
            StoreDataGrid.Columns["CurrencyTypeID"].Visible = false;
            StoreDataGrid.Columns["FactoryID"].Visible = false;
            StoreDataGrid.Columns["PriceEUR"].Visible = false;
            StoreDataGrid.Columns["IsArrived"].Visible = false;
            StoreDataGrid.Columns["ManufacturerID"].Visible = false;
            StoreDataGrid.Columns["DecorAssignmentID"].Visible = false;
        }

        private void AddItemButton_Click_1(object sender, EventArgs e)
        {
            if (ItemsDataGrid.SelectedRows.Count == 0)
                return;

            int StoreItemID = -1;
            decimal Length = -1;
            decimal Width = -1;
            decimal Height = -1;
            decimal Thickness = -1;
            decimal Diameter = -1;
            decimal Admission = -1;
            decimal Capacity = -1;
            decimal Weight = -1;
            int ColorID = -1;
            int PatinaID = -1;
            int CoverID = -1;
            decimal Count = -1;
            decimal Price = -1;
            int CurrencyTypeID = -1;
            int ManufacturerID = -1;
            int FactoryID = -1;
            string BatchNumber = string.Empty;
            string TechStoreName = string.Empty;
            string Notes = string.Empty;

            if (ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                StoreItemID = Convert.ToInt32(ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            if (ItemsDataGrid.SelectedRows[0].Cells["TechStoreName"].Value != DBNull.Value)
                TechStoreName = ItemsDataGrid.SelectedRows[0].Cells["TechStoreName"].Value.ToString();

            if (panel1.Visible)
            {
                if (ColorsComboBox.Items.Count == 0)
                    return;

                ColorID = Convert.ToInt32(ColorsComboBox.SelectedValue);
            }

            if (panel2.Visible)
            {
                if (PatinaComboBox.Items.Count == 0)
                    return;

                PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);
            }

            if (panel15.Visible)
            {
                if (CoversComboBox.Items.Count == 0)
                    return;

                CoverID = Convert.ToInt32(CoversComboBox.SelectedValue);
            }

            if (panel3.Visible)
            {
                if (ThicknessTextBox.Text.Length == 0 || Convert.ToDecimal(ThicknessTextBox.Text) < 0)
                {
                    ThicknessTextBox.Focus();
                    return;
                }

                Thickness = Convert.ToDecimal(ThicknessTextBox.Text);
            }

            if (panel4.Visible)
            {
                if (LengthTextBox.Text.Length == 0 || Convert.ToDecimal(LengthTextBox.Text) < 0)
                {
                    LengthTextBox.Focus();
                    return;
                }

                Length = Convert.ToDecimal(LengthTextBox.Text);
            }

            if (panel5.Visible)
            {
                if (HeightTextBox.Text.Length == 0 || Convert.ToDecimal(HeightTextBox.Text) < 0)
                {
                    HeightTextBox.Focus();
                    return;
                }

                Height = Convert.ToDecimal(HeightTextBox.Text);
            }

            if (panel6.Visible)
            {
                if (WidthTextBox.Text.Length == 0 || Convert.ToDecimal(WidthTextBox.Text) < 0)
                {
                    WidthTextBox.Focus();
                    return;
                }

                Width = Convert.ToDecimal(WidthTextBox.Text);
            }

            if (panel7.Visible)
            {
                if (AdmissionTextBox.Text.Length == 0 || Convert.ToDecimal(AdmissionTextBox.Text) < 0)
                {
                    AdmissionTextBox.Focus();
                    return;
                }

                Admission = Convert.ToDecimal(AdmissionTextBox.Text);
            }

            if (panel8.Visible)
            {
                if (DiameterTextBox.Text.Length == 0 || Convert.ToDecimal(DiameterTextBox.Text) < 0)
                {
                    DiameterTextBox.Focus();
                    return;
                }

                Diameter = Convert.ToDecimal(DiameterTextBox.Text);
            }

            if (panel10.Visible)
            {
                if (CapacityTextBox.Text.Length == 0 || Convert.ToDecimal(CapacityTextBox.Text) < 0)
                {
                    CapacityTextBox.Focus();
                    return;
                }

                Capacity = Convert.ToDecimal(CapacityTextBox.Text);
            }

            if (panel11.Visible)
            {
                if (WeightTextBox.Text.Length == 0 || Convert.ToDecimal(WeightTextBox.Text) < 0)
                {
                    WeightTextBox.Focus();
                    return;
                }

                Weight = Convert.ToDecimal(WeightTextBox.Text);
            }

            if (panel12.Visible)
            {
                if (CountTextBox.Text.Length == 0 || Convert.ToDecimal(CountTextBox.Text) < 0)
                {
                    CountTextBox.Focus();
                    return;
                }

                Count = Convert.ToDecimal(CountTextBox.Text);
            }

            if (PriceTextBox.Text.Length == 0)
            {
                PriceTextBox.Focus();
                return;
            }

            if (PriceTextBox.Text.Length > 0)
                Price = Convert.ToDecimal(PriceTextBox.Text);
            if (tbBatchNumber.Text.Length > 0)
                BatchNumber = tbBatchNumber.Text;

            ManufacturerID = Convert.ToInt32(cbManufacturers.SelectedValue);
            CurrencyTypeID = Convert.ToInt32(CurrencyTypesComboBox.SelectedValue);
            FactoryID = Convert.ToInt32(FactoryIDComboBox.SelectedValue);
            Notes = tbNotes.Text;

            StoreInvoiceManager.AddItem(StoreItemID, Length, Width, Height, Thickness, Diameter, Admission,
                Capacity, Weight, ColorID, PatinaID, CoverID,
                Price, Count, CurrencyTypeID, FactoryID,
                ProducedCheckBox.Checked, BestBeforeCheckBox.Checked, ProducedDatePicker.Value, BestBeforeDatePicker.Value, ManufacturerID, BatchNumber, Notes,
                TechStoreName, ArrivalDateTimePicker.Value);

            InfiniumTips.ShowTip(this, 50, 85, "Добавлено", 1700);
            CheckStoreColumns();

            if (StoreInvoiceManager == null)
                return;

            if (StoreDataGrid.Rows.Count > 0)
            {
                NumberFormatInfo nfi1 = new NumberFormatInfo()
                {
                    NumberGroupSeparator = " ",
                    CurrencySymbol = "",
                    NumberDecimalDigits = 2,
                    NumberDecimalSeparator = ","
                };
                decimal Cost = 0;
                for (int i = 0; i < StoreDataGrid.Rows.Count; i++)
                {
                    if (StoreDataGrid.Rows[i].Cells["Cost"].Value != DBNull.Value)
                        Cost += Convert.ToDecimal(StoreDataGrid.Rows[i].Cells["Cost"].Value);
                }

                Cost = Decimal.Round(Cost, 3, MidpointRounding.AwayFromZero);
                SummaryCostLabel.Text = "Общая сумма: " + Cost.ToString("N", nfi1);
                SummaryCostLabel.Visible = true;
            }
            else
                SummaryCostLabel.Visible = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (StoreDataGrid.Rows.Count == 0 && PurchaseInvoiceID == -1)
                return;

            int SellerID = Convert.ToInt32(SellerComboBox.SelectedValue);

            if (SellerID == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Поставщика нужно выбирать из списка. Если в списке нет нужного поставщика, то его нужно сначала занести в базу, а потом сохранять накладную. Обратитетесь к программистам",
                        "Сохранение накладной");
                return;
            }

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Сохранить на МОЛ?",
                    "Сохранение накладной");

            DateTime IncomeDateTime = ArrivalDateTimePicker.Value;
            string DocNumber = InvoiceDocNumberTextBox.Text;
            string Reason = InvoiceReasonTextBox.Text;
            int FactoryID = Convert.ToInt32(FactoryIDComboBox.SelectedValue);
            int CurrencyTypeID = Convert.ToInt32(CurrencyTypesComboBox.SelectedValue);
            int LastMovementInvoiceID = -1;
            int LastPurchaseInvoiceID = -1;
            string Notes = NotesRichTextBox.Text;

            if (PurchaseInvoiceID == -1)
            {
                StoreInvoiceManager.SaveInvoice(IncomeDateTime, SellerID, DocNumber, FactoryID, CurrencyTypeID, Reason, Notes);
                LastPurchaseInvoiceID = StoreInvoiceManager.GetLastPurchaseInvoiceID();
            }
            else
            {
                StoreInvoiceManager.SaveInvoice(PurchaseInvoiceID, IncomeDateTime, SellerID, DocNumber, FactoryID, CurrencyTypeID, Reason, Notes);
                LastPurchaseInvoiceID = PurchaseInvoiceID;
            }

            StoreInvoiceManager.StoreDataTable.Clear();
            if (OKCancel)
            {
                bool PressOK = false;
                int SellerStoreAllocID = 0;
                int PersonID = 0;
                string PersonName = string.Empty;
                if (FactoryID == 1)
                    SellerStoreAllocID = 1;
                if (FactoryID == 2)
                    SellerStoreAllocID = 2;

                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                MoveToPersonMenu PurchaseToPersonlMenu = new MoveToPersonMenu(this);
                TopForm = PurchaseToPersonlMenu;
                PurchaseToPersonlMenu.ShowDialog();

                PressOK = PurchaseToPersonlMenu.PressOK;
                PersonID = PurchaseToPersonlMenu.PersonID;
                PersonName = PurchaseToPersonlMenu.PersonName;

                PhantomForm.Close();
                PhantomForm.Dispose();
                PurchaseToPersonlMenu.Dispose();
                TopForm = null;

                if (PressOK)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    StoreInvoiceManager.SaveMovementInvoices(IncomeDateTime, SellerStoreAllocID, 9, 0,
                            PersonID, PersonName, Security.CurrentUserID, 0, 0, string.Empty, string.Empty);
                    LastMovementInvoiceID = StoreInvoiceManager.GetLastMovementInvoiceID();
                    StoreInvoiceManager.MoveToPersonalStore(LastMovementInvoiceID, LastPurchaseInvoiceID);
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
            }
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Позиция будет удалена. Продолжить?",
                    "Редактирование накладной");

            if (OKCancel)
            {
                StoreInvoiceManager.RemoveItem();
                CheckStoreColumns();

                if (StoreInvoiceManager == null)
                    return;

                decimal SummaryCost = 0;
                GetSummaryCost(ref SummaryCost);
                if (SummaryCost > 0)
                {
                    NumberFormatInfo nfi1 = new NumberFormatInfo()
                    {
                        NumberGroupSeparator = " ",
                        CurrencySymbol = "",
                        NumberDecimalDigits = 2,
                        NumberDecimalSeparator = ","
                    };
                    SummaryCostLabel.Text = "Общая сумма: " + SummaryCost.ToString("N", nfi1);
                    SummaryCostLabel.Visible = true;
                }
                else
                    SummaryCostLabel.Visible = false;
            }
        }

        private void ProducedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ProducedDatePicker.Enabled = ProducedCheckBox.Checked;
        }

        private void BestBeforeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BestBeforeDatePicker.Enabled = BestBeforeCheckBox.Checked;
        }

        private void SubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreInvoiceManager == null)
                return;
            if (SubGroupsDataGrid.SelectedRows.Count == 0)
                return;
            int TechStoreSubGroupID = 0;
            if (SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);
            StoreInvoiceManager.UpdateStoreItems(TechStoreSubGroupID);

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

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            //StoreInvoiceManager.AddSize();
        }

        private void PriceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddItemButton_Click_1(null, null);
            }
        }

        private void NewArrivalForm_Load(object sender, EventArgs e)
        {

            GroupsDataGrid.SelectionChanged += new EventHandler(GroupsDataGrid_SelectionChanged);
            SubGroupsDataGrid.SelectionChanged += new EventHandler(SubGroupsDataGrid_SelectionChanged);
        }

        private void ColorsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; } // added this line thanks to Andrew's comment
            Point p = new Point(ColorsComboBox.Location.X + 120, 0);
            string text = ColorsComboBox.GetItemText(ColorsComboBox.Items[e.Index]);
            string tooltipText = StoreInvoiceManager.GetSellerCode(Convert.ToInt32(ColorsComboBox.SelectedValue));

            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            { e.Graphics.DrawString(text, e.Font, br, e.Bounds); }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                toolTip1.Show(tooltipText, ColorsComboBox, p);
            }
            e.DrawFocusRectangle();
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(ColorsComboBox);
        }
    }
}
