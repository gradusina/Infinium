using Infinium.Modules.Marketing.MutualSettlements;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MutualSettlementsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        const int iAdminRole = 94;
        const int iAccountantRole = 95;
        const int iMarketerRole = 96;

        bool NeedFilterClients = false;
        int FactoryID = 1;
        int FormEvent = 0;

        MutualSettlements MutualSettlementsManager;

        FileManager FM;
        Form TopForm;
        LightStartForm LightStartForm;
        MutualSettlementsFilterMenu MutualSettlementsFilterMenu;
        System.Globalization.NumberFormatInfo nfi1;
        //DocumentTypes RoleType = DocumentTypes.InvoiceExcel;

        public MutualSettlementsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void MutualSettlementsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedFilterClients = true;
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
                        LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        LightStartForm.HideForm(this);
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

        private void Initialize()
        {
            nfi1 = new System.Globalization.NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ","
            };


            FM = new FileManager();
            MutualSettlementsManager = new MutualSettlements();
            MutualSettlementsManager.Initialize();

            MutualSettlementsManager.GetPermissions(Security.CurrentUserID, this.Name);

            cbClients.DataSource = MutualSettlementsManager.ClientsBS;
            cbClients.DisplayMember = "ClientName";
            cbClients.ValueMember = "ClientID";
            dgvClientsDispatchesSettings();
            dgvClientsIncomesSettings();
            dgvMutualSettlementsSettings();
            dgvNewMutualSettlementsSettings();
            dgvNewClientsDispatchesSettings();


            MutualSettlementsManager.GetAllClientsDispatches();
            MutualSettlementsManager.GetAllClientsIncomes();
            for (int i = 0; i < cbClients.Items.Count; i++)
            {
                int c = Convert.ToInt32(((DataRowView)cbClients.Items[i]).Row["ClientID"]);
                MutualSettlementsManager.CalcClosingBalance(c, 1);
                MutualSettlementsManager.CalcClosingBalance(c, 2);
            }
            MutualSettlementsManager.FilterMutualSettlements(true, true, true, true, 5);
            NeedFilterClients = true;
            MutualSettlementsManager.FillNewMutualSettlements(FactoryID);
            MutualSettlementsManager.FillNewClientsDispatches(FactoryID);

            bool b = MutualSettlementsManager.FillSubscribes();
            if (b)
            {
                int MutualSettlementID = Convert.ToInt32(MutualSettlementsManager.SubscribesDT.Rows[0]["MutualSettlementID"]);
                int ClientID = Convert.ToInt32(MutualSettlementsManager.SubscribesDT.Rows[0]["ClientID"]);
                //int FactoryID = Convert.ToInt32(MutualSettlementsManager.SubscribesDT.Rows[0]["FactoryID"]);
                MutualSettlementsManager.MoveToClient(ClientID);
                if (Convert.ToInt32(MutualSettlementsManager.SubscribesDT.Rows[0]["FactoryID"]) == 1)
                    rbProfil.Checked = true;
                if (Convert.ToInt32(MutualSettlementsManager.SubscribesDT.Rows[0]["FactoryID"]) == 2)
                    rbTPS.Checked = true;
                MutualSettlementsManager.MoveToMutualSettlement(MutualSettlementID);
                MutualSettlementsManager.ClearSubscribes(MutualSettlementID);
            }
        }

        private void dgvClientsDispatchesSettings()
        {
            dgvClientsDispatches.DataSource = MutualSettlementsManager.ClientsDispatchesBS;
            dgvClientsDispatches.Columns.Add(MutualSettlementsManager.DispatchDateTimeColumn);
            dgvClientsDispatches.Columns.Add(MutualSettlementsManager.CurrencyTypeColumn);

            if (dgvClientsDispatches.Columns.Contains("ClientDispatchID"))
                dgvClientsDispatches.Columns["ClientDispatchID"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("MutualSettlementID"))
                dgvClientsDispatches.Columns["MutualSettlementID"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("CreateDateTime"))
                dgvClientsDispatches.Columns["CreateDateTime"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("CreateUserID"))
                dgvClientsDispatches.Columns["CreateUserID"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("DispatchDateTime"))
                dgvClientsDispatches.Columns["DispatchDateTime"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("DispatchExcel"))
                dgvClientsDispatches.Columns["DispatchExcel"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("DispatchDbf"))
                dgvClientsDispatches.Columns["DispatchDbf"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("ConfirmUserID"))
                dgvClientsDispatches.Columns["ConfirmUserID"].Visible = false;
            if (dgvClientsDispatches.Columns.Contains("CurrencyTypeID"))
                dgvClientsDispatches.Columns["CurrencyTypeID"].Visible = false;

            foreach (DataGridViewColumn column in dgvClientsDispatches.Columns)
            {
                //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //column.MinimumWidth = 60;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 2
            };
            dgvClientsDispatches.Columns["ConfirmDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvClientsDispatches.Columns["DispatchSum"].DefaultCellStyle.Format = "C";
            dgvClientsDispatches.Columns["DispatchSum"].DefaultCellStyle.FormatProvider = nfi1;

            dgvClientsDispatches.Columns["Notes"].HeaderText = "Примечание";
            dgvClientsDispatches.Columns["DispatchExcelName"].HeaderText = "Excel";
            dgvClientsDispatches.Columns["DispatchDbfName"].HeaderText = "Dbf";
            dgvClientsDispatches.Columns["DispatchWaybills"].HeaderText = "ТТН";
            dgvClientsDispatches.Columns["DispatchSum"].HeaderText = "Сумма счета";
            dgvClientsDispatches.Columns["ConfirmVAT"].HeaderText = "Подтверждение\nНДС";
            dgvClientsDispatches.Columns["ConfirmDateTime"].HeaderText = "Дата\nподтверждения";

            dgvClientsDispatches.Columns["DispatchExcelName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsDispatches.Columns["DispatchExcelName"].MinimumWidth = 110;
            dgvClientsDispatches.Columns["DispatchDbfName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsDispatches.Columns["DispatchDbfName"].MinimumWidth = 110;
            dgvClientsDispatches.Columns["DispatchWaybills"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsDispatches.Columns["DispatchWaybills"].MinimumWidth = 110;
            dgvClientsDispatches.Columns["DispatchSum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsDispatches.Columns["DispatchSum"].MinimumWidth = 110;
            dgvClientsDispatches.Columns["ConfirmDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsDispatches.Columns["ConfirmDateTime"].MinimumWidth = 100;
            dgvClientsDispatches.Columns["ConfirmVAT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsDispatches.Columns["ConfirmVAT"].MinimumWidth = 60;
            dgvClientsDispatches.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsDispatches.Columns["Notes"].MinimumWidth = 110;

            dgvClientsDispatches.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvClientsDispatches.Columns["DispatchWaybills"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["DispatchDateTimeColumn"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["DispatchSum"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["CurrencyTypeColumn"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["ConfirmVAT"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["ConfirmDateTime"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["DispatchExcelName"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["DispatchDbfName"].DisplayIndex = DisplayIndex++;
            dgvClientsDispatches.Columns["Notes"].DisplayIndex = DisplayIndex++;


            dgvClientsDispatches.ReadOnly = true;
            if (MutualSettlementsManager.PermissionGranted(iAdminRole) || MutualSettlementsManager.PermissionGranted(iAccountantRole) || MutualSettlementsManager.PermissionGranted(iMarketerRole))
            {
                dgvClientsDispatches.ReadOnly = false;
            }
            dgvClientsDispatches.Columns["DispatchExcelName"].ReadOnly = true;
            dgvClientsDispatches.Columns["DispatchDbfName"].ReadOnly = true;
            dgvClientsDispatches.Columns["ConfirmVAT"].ReadOnly = true;
            dgvClientsDispatches.Columns["ConfirmDateTime"].ReadOnly = true;
        }

        private void dgvClientsIncomesSettings()
        {
            dgvClientsIncomes.DataSource = MutualSettlementsManager.ClientsIncomesBS;
            dgvClientsIncomes.Columns.Add(MutualSettlementsManager.IncomeDateTimeColumn);
            dgvClientsIncomes.Columns.Add(MutualSettlementsManager.CurrencyTypeColumn);

            if (dgvClientsIncomes.Columns.Contains("ClientIncomeID"))
                dgvClientsIncomes.Columns["ClientIncomeID"].Visible = false;
            if (dgvClientsIncomes.Columns.Contains("MutualSettlementID"))
                dgvClientsIncomes.Columns["MutualSettlementID"].Visible = false;
            if (dgvClientsIncomes.Columns.Contains("CreateDateTime"))
                dgvClientsIncomes.Columns["CreateDateTime"].Visible = false;
            if (dgvClientsIncomes.Columns.Contains("CreateUserID"))
                dgvClientsIncomes.Columns["CreateUserID"].Visible = false;
            if (dgvClientsIncomes.Columns.Contains("IncomeDateTime"))
                dgvClientsIncomes.Columns["IncomeDateTime"].Visible = false;
            if (dgvClientsIncomes.Columns.Contains("CurrencyTypeID"))
                dgvClientsIncomes.Columns["CurrencyTypeID"].Visible = false;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 2
            };
            dgvClientsIncomes.Columns["IncomeSum"].DefaultCellStyle.Format = "C";
            dgvClientsIncomes.Columns["IncomeSum"].DefaultCellStyle.FormatProvider = nfi1;

            dgvClientsIncomes.Columns["IncomeSum"].HeaderText = "Сумма поступления";
            dgvClientsIncomes.Columns["Notes"].HeaderText = "Примечание";

            dgvClientsIncomes.Columns["IncomeSum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsIncomes.Columns["IncomeSum"].MinimumWidth = 110;
            dgvClientsIncomes.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientsIncomes.Columns["Notes"].MinimumWidth = 110;

            dgvClientsIncomes.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvClientsIncomes.Columns["IncomeDateTimeColumn"].DisplayIndex = DisplayIndex++;
            dgvClientsIncomes.Columns["IncomeSum"].DisplayIndex = DisplayIndex++;
            dgvClientsIncomes.Columns["CurrencyTypeColumn"].DisplayIndex = DisplayIndex++;
            dgvClientsIncomes.Columns["Notes"].DisplayIndex = DisplayIndex++;


            dgvClientsIncomes.ReadOnly = true;
            if (MutualSettlementsManager.PermissionGranted(iAdminRole) || MutualSettlementsManager.PermissionGranted(iAccountantRole))
            {
                dgvClientsIncomes.ReadOnly = false;
            }
        }

        private void dgvMutualSettlementsSettings()
        {
            dgvMutualSettlements.DataSource = MutualSettlementsManager.MutualSettlementsBS;
            //dgvMutualSettlements.Columns.Add(MutualSettlementsManager.ClientsColumn);
            dgvMutualSettlements.Columns.Add(MutualSettlementsManager.CurrencyTypeColumn);
            dgvMutualSettlements.Columns.Add(MutualSettlementsManager.InvoiceDateTimeColumn);
            dgvMutualSettlements.Columns.Add(MutualSettlementsManager.PaymentConditionColumn);

            if (dgvMutualSettlements.Columns.Contains("DelayOfPayment"))
                dgvMutualSettlements.Columns["DelayOfPayment"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("ClientID"))
                dgvMutualSettlements.Columns["ClientID"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("FactoryID"))
                dgvMutualSettlements.Columns["FactoryID"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("CurrencyTypeID"))
                dgvMutualSettlements.Columns["CurrencyTypeID"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("DiscountPaymentConditionID"))
                dgvMutualSettlements.Columns["DiscountPaymentConditionID"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("InvoiceDateTime"))
                dgvMutualSettlements.Columns["InvoiceDateTime"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("CreateDateTime"))
                dgvMutualSettlements.Columns["CreateDateTime"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("CreateUserID"))
                dgvMutualSettlements.Columns["CreateUserID"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("TotalIncomeSum"))
                dgvMutualSettlements.Columns["TotalIncomeSum"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("InvoiceExcel"))
                dgvMutualSettlements.Columns["InvoiceExcel"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("InvoiceDbf"))
                dgvMutualSettlements.Columns["InvoiceDbf"].Visible = false;
            if (dgvMutualSettlements.Columns.Contains("CompareBalance"))
                dgvMutualSettlements.Columns["CompareBalance"].Visible = false;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 2,
                CurrencyNegativePattern = 1
            };
            dgvMutualSettlements.Columns["OpeningBalance"].DefaultCellStyle.Format = "C";
            dgvMutualSettlements.Columns["OpeningBalance"].DefaultCellStyle.FormatProvider = nfi1;
            dgvMutualSettlements.Columns["TotalInvoiceSum"].DefaultCellStyle.Format = "C";
            dgvMutualSettlements.Columns["TotalInvoiceSum"].DefaultCellStyle.FormatProvider = nfi1;
            dgvMutualSettlements.Columns["ClosingBalance"].DefaultCellStyle.Format = "C";
            dgvMutualSettlements.Columns["ClosingBalance"].DefaultCellStyle.FormatProvider = nfi1;

            dgvMutualSettlements.Columns["Notes"].HeaderText = "Примечание";
            dgvMutualSettlements.Columns["MutualSettlementID"].HeaderText = "№";
            dgvMutualSettlements.Columns["OpeningBalance"].HeaderText = "Начальное\nсальдо";
            dgvMutualSettlements.Columns["OrderNumbers"].HeaderText = "№ заказов";
            dgvMutualSettlements.Columns["InvoiceExcelName"].HeaderText = "Excel";
            dgvMutualSettlements.Columns["InvoiceDbfName"].HeaderText = "Dbf";
            dgvMutualSettlements.Columns["InvoiceNumber"].HeaderText = "№ счета";
            dgvMutualSettlements.Columns["SpecificationNumber"].HeaderText = "№\nспецификации";
            dgvMutualSettlements.Columns["TotalInvoiceSum"].HeaderText = "Сумма счета";
            dgvMutualSettlements.Columns["ClosingBalance"].HeaderText = "Конечное\nсальдо";
            dgvMutualSettlements.Columns["IsSample"].HeaderText = "Образцы";

            dgvMutualSettlements.Columns["MutualSettlementID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMutualSettlements.Columns["MutualSettlementID"].Width = 60;
            dgvMutualSettlements.Columns["OpeningBalance"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMutualSettlements.Columns["OpeningBalance"].Width = 110;
            dgvMutualSettlements.Columns["OrderNumbers"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMutualSettlements.Columns["OrderNumbers"].MinimumWidth = 100;
            dgvMutualSettlements.Columns["InvoiceExcelName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMutualSettlements.Columns["InvoiceExcelName"].MinimumWidth = 110;
            dgvMutualSettlements.Columns["InvoiceDbfName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMutualSettlements.Columns["InvoiceDbfName"].MinimumWidth = 110;
            dgvMutualSettlements.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMutualSettlements.Columns["Notes"].MinimumWidth = 110;
            dgvMutualSettlements.Columns["InvoiceNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMutualSettlements.Columns["InvoiceNumber"].Width = 110;
            dgvMutualSettlements.Columns["SpecificationNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMutualSettlements.Columns["SpecificationNumber"].Width = 110;
            dgvMutualSettlements.Columns["TotalInvoiceSum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMutualSettlements.Columns["TotalInvoiceSum"].Width = 110;
            dgvMutualSettlements.Columns["ClosingBalance"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMutualSettlements.Columns["ClosingBalance"].Width = 110;
            dgvMutualSettlements.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMutualSettlements.Columns["IsSample"].Width = 70;

            dgvMutualSettlements.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvMutualSettlements.Columns["MutualSettlementID"].DisplayIndex = DisplayIndex++;
            //dgvMutualSettlements.Columns["ClientsColumn"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["CurrencyTypeColumn"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["OpeningBalance"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["OrderNumbers"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["PaymentConditionColumn"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["InvoiceNumber"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["SpecificationNumber"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["InvoiceDateTimeColumn"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["TotalInvoiceSum"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["ClosingBalance"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["InvoiceExcelName"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["InvoiceDbfName"].DisplayIndex = DisplayIndex++;
            dgvMutualSettlements.Columns["Notes"].DisplayIndex = DisplayIndex++;

            dgvMutualSettlements.ReadOnly = true;
            if (MutualSettlementsManager.PermissionGranted(iAdminRole) || MutualSettlementsManager.PermissionGranted(iAccountantRole) || MutualSettlementsManager.PermissionGranted(iMarketerRole))
            {
                dgvMutualSettlements.ReadOnly = false;
            }

            dgvMutualSettlements.Columns["InvoiceExcelName"].ReadOnly = true;
            dgvMutualSettlements.Columns["InvoiceDbfName"].ReadOnly = true;

        }

        private void dgvNewMutualSettlementsSettings()
        {
            dgvNewMutualSettlements.DataSource = MutualSettlementsManager.NewMutualSettlementsBS;

            if (dgvNewMutualSettlements.Columns.Contains("ClientID"))
                dgvNewMutualSettlements.Columns["ClientID"].Visible = false;

            dgvNewMutualSettlements.Columns["MutualSettlementID"].HeaderText = "№";
            dgvNewMutualSettlements.Columns["ClientName"].HeaderText = "Клиент";

            dgvNewMutualSettlements.Columns["MutualSettlementID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvNewMutualSettlements.Columns["MutualSettlementID"].Width = 60;
            dgvNewMutualSettlements.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvNewMutualSettlements.Columns["ClientName"].MinimumWidth = 100;

            dgvNewMutualSettlements.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvNewMutualSettlements.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            dgvNewMutualSettlements.Columns["MutualSettlementID"].DisplayIndex = DisplayIndex++;
        }

        private void dgvNewClientsDispatchesSettings()
        {
            dgvNewClientsDispatches.DataSource = MutualSettlementsManager.NewClientsDispatchesBS;

            if (dgvNewClientsDispatches.Columns.Contains("ClientID"))
                dgvNewClientsDispatches.Columns["ClientID"].Visible = false;

            dgvNewClientsDispatches.Columns["MutualSettlementID"].HeaderText = "№";
            dgvNewClientsDispatches.Columns["ClientName"].HeaderText = "Клиент";

            dgvNewClientsDispatches.Columns["MutualSettlementID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvNewClientsDispatches.Columns["MutualSettlementID"].Width = 60;
            dgvNewClientsDispatches.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvNewClientsDispatches.Columns["ClientName"].MinimumWidth = 100;

            dgvNewClientsDispatches.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvNewClientsDispatches.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            dgvNewClientsDispatches.Columns["MutualSettlementID"].DisplayIndex = DisplayIndex++;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void MutualSettlementsForm_Load(object sender, EventArgs e)
        {

        }

        private void dgvMutualSettlements_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            openFileDialog1.ShowDialog();
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            openFileDialog2.ShowDialog();
        }

        private void kryptonContextMenuItem7_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            openFileDialog3.ShowDialog();
        }

        private void kryptonContextMenuItem8_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            openFileDialog4.ShowDialog();
        }

        private void dgvClientsDispatches_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnAddMutualSettlement_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы собираетесь добавить новый счет. Продолжить?",
                "Создание нового счета");
            if (!OKCancel)
                return;

            int ClientID = -1;
            if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            if (ClientID == -1)
                return;
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;

            int CurrencyTypeID = 5;
            if (kryptonRadioButton2.Checked)
                CurrencyTypeID = 3;
            if (kryptonRadioButton3.Checked)
                CurrencyTypeID = 2;
            if (kryptonRadioButton4.Checked)
                CurrencyTypeID = 1;
            MutualSettlementsManager.AddMutualSettlement(ClientID, FactoryID, CurrencyTypeID);
            //MutualSettlementsManager.SaveMutualSettlements();
            //MutualSettlementsManager.GetMutualSettlements(ClientID, date1, date2);
            InfiniumTips.ShowTip(this, 50, 85, "Запись добавлена", 1700);
        }

        private void btnAddClientIncome_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            int CurrencyTypeID = 1;
            if (dgvMutualSettlements.SelectedRows[0].Cells["CurrencyTypeID"].Value != DBNull.Value)
                CurrencyTypeID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["CurrencyTypeID"].Value);
            int MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            MutualSettlementsManager.AddClientIncome(MutualSettlementID, CurrencyTypeID);
            InfiniumTips.ShowTip(this, 50, 85, "Запись добавлена", 1700);
        }

        private void btnAddClientDispatch_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            int CurrencyTypeID = 1;
            if (dgvMutualSettlements.SelectedRows[0].Cells["CurrencyTypeID"].Value != DBNull.Value)
                CurrencyTypeID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["CurrencyTypeID"].Value);
            int MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            MutualSettlementsManager.AddClientDispatch(MutualSettlementID, CurrencyTypeID);
            InfiniumTips.ShowTip(this, 50, 85, "Запись добавлена", 1700);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            int MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            var fileInfo = new System.IO.FileInfo(openFileDialog1.FileName);

            if (fileInfo.Length > 1500000)
            {
                MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
                return;
            }
            bool bOk = MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(openFileDialog1.FileName),
                fileInfo.Extension, openFileDialog1.FileName, DocumentTypes.InvoiceExcel, MutualSettlementID);
            RefreshMutualSettlements();
            MutualSettlementsManager.MoveToMutualSettlement(MutualSettlementID);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ прикреплен", 1700);
        }

        private void openFileDialog2_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            int MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            var fileInfo = new System.IO.FileInfo(openFileDialog2.FileName);

            if (fileInfo.Length > 1500000)
            {
                MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
                return;
            }
            bool bOk = MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(openFileDialog2.FileName),
                fileInfo.Extension, openFileDialog2.FileName, DocumentTypes.InvoiceDbf, MutualSettlementID);
            RefreshMutualSettlements();
            MutualSettlementsManager.MoveToMutualSettlement(MutualSettlementID);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ прикреплен", 1700);
        }

        private void openFileDialog3_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dgvClientsDispatches.SelectedRows.Count == 0 || dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value == DBNull.Value)
                return;
            int ClientDispatchID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value);
            var fileInfo = new System.IO.FileInfo(openFileDialog3.FileName);

            if (fileInfo.Length > 1500000)
            {
                MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
                return;
            }
            bool bOk = MutualSettlementsManager.AttachDispatchDocument(Path.GetFileNameWithoutExtension(openFileDialog3.FileName),
                fileInfo.Extension, openFileDialog3.FileName, DocumentTypes.DispatchExcel, ClientDispatchID);
            RefreshClientsDispatches();
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ прикреплен", 1700);
        }

        private void openFileDialog4_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dgvClientsDispatches.SelectedRows.Count == 0 || dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value == DBNull.Value)
                return;
            int ClientDispatchID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value);
            var fileInfo = new System.IO.FileInfo(openFileDialog4.FileName);

            if (fileInfo.Length > 1500000)
            {
                MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
                return;
            }
            bool bOk = MutualSettlementsManager.AttachDispatchDocument(Path.GetFileNameWithoutExtension(openFileDialog4.FileName),
                fileInfo.Extension, openFileDialog4.FileName, DocumentTypes.DispatchDbf, ClientDispatchID);
            RefreshClientsDispatches();
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ прикреплен", 1700);
        }

        private void btnRemoveMutualSettlement_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы собираетесь удалить счет. Продолжить?",
                "Удаление счета");
            if (!OKCancel)
                return;
            ///РЕАЛИЗОВАТЬ КАСКАДНОЕ УДАЛЕНИЕ ФАЙЛОВ
            int CurrentRowIndex = dgvMutualSettlements.SelectedRows[0].Index;
            int MutualSettlementID = 0;
            if (dgvMutualSettlements.SelectedRows.Count > 0 && dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value != DBNull.Value)
                MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            MutualSettlementsManager.AddDocumentsToDeleteTable(MutualSettlementID);
            MutualSettlementsManager.RemoveMutualSettlement();
            MutualSettlementsManager.RemoveClientsIncomes(MutualSettlementID);
            MutualSettlementsManager.RemoveClientsDispatches(MutualSettlementID);
            MutualSettlementsManager.RemoveMutualSettlementOrders(MutualSettlementID);
            if (CurrentRowIndex == 0)
            {
                if (dgvMutualSettlements.Rows.Count > 0)
                    dgvMutualSettlements.Rows[0].Selected = true;
            }
            else
            {
                if (CurrentRowIndex >= dgvMutualSettlements.Rows.Count)
                    dgvMutualSettlements.Rows[dgvMutualSettlements.Rows.Count - 1].Selected = true;
                else
                    dgvMutualSettlements.Rows[CurrentRowIndex].Selected = true;
            }
            InfiniumTips.ShowTip(this, 50, 85, "Запись удалена", 1700);
        }

        private void btnRemoveClientIncome_Click(object sender, EventArgs e)
        {
            if (dgvClientsIncomes.SelectedRows.Count == 0)
                return;
            //bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //    "Подтвердите удаление",
            //    "Удалить выбранную позицию");
            //if (!OKCancel)
            //    return;
            int CurrentRowIndex = dgvClientsIncomes.SelectedRows[0].Index;
            MutualSettlementsManager.RemoveClientsIncomes();
            if (CurrentRowIndex == 0)
            {
                if (dgvClientsIncomes.Rows.Count > 0)
                    dgvClientsIncomes.Rows[0].Selected = true;
            }
            else
            {
                if (CurrentRowIndex >= dgvClientsIncomes.Rows.Count)
                    dgvClientsIncomes.Rows[dgvClientsIncomes.Rows.Count - 1].Selected = true;
                else
                    dgvClientsIncomes.Rows[CurrentRowIndex].Selected = true;
            }
            InfiniumTips.ShowTip(this, 50, 85, "Запись удалена", 1700);
        }

        private void btnRemoveClientDispatch_Click(object sender, EventArgs e)
        {
            if (dgvClientsDispatches.SelectedRows.Count == 0)
                return;
            int ClientDispatchID = 0;
            if (dgvClientsDispatches.SelectedRows.Count > 0 && dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value != DBNull.Value)
            {
                ClientDispatchID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value);
                MutualSettlementsManager.AddDispatchDocumentToDeleteTable(DocumentTypes.DispatchExcel, ClientDispatchID);
                MutualSettlementsManager.AddDispatchDocumentToDeleteTable(DocumentTypes.DispatchDbf, ClientDispatchID);
            }
            int CurrentRowIndex = dgvClientsDispatches.SelectedRows[0].Index;
            MutualSettlementsManager.RemoveClientsDispatches();
            if (CurrentRowIndex == 0)
            {
                if (dgvClientsDispatches.Rows.Count > 0)
                    dgvClientsDispatches.Rows[0].Selected = true;
            }
            else
            {
                if (CurrentRowIndex >= dgvClientsDispatches.Rows.Count)
                    dgvClientsDispatches.Rows[dgvClientsDispatches.Rows.Count - 1].Selected = true;
                else
                    dgvClientsDispatches.Rows[CurrentRowIndex].Selected = true;
            }
            InfiniumTips.ShowTip(this, 50, 85, "Запись удалена", 1700);
        }

        private void RefreshMutualSettlements()
        {
            int ClientID = -1;
            if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            if (ClientID == -1)
                return;
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            MutualSettlementsManager.GetClientsIncomes(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetClientsDispatches(ClientID, FactoryID, date1, date2);
            //lbTotalClientDispatch.Text = MutualSettlementsManager.TotalClientDispatch();
            //lbTotalClientIncomes.Text = MutualSettlementsManager.TotalClientIncomes();
            MutualSettlementsManager.GetMutualSettlementOrders(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetMutualSettlements(ClientID, FactoryID, date1, date2);
        }

        private void RefreshClientsDispatches()
        {
            int ClientID = -1;
            if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            if (ClientID == -1)
                return;
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            MutualSettlementsManager.SaveClientsDispatches();
            MutualSettlementsManager.GetClientsDispatches(ClientID, FactoryID, date1, date2);
            //lbTotalClientDispatch.Text = MutualSettlementsManager.TotalClientDispatch();
        }

        private void cbClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshMutualSettlements();
        }

        private void btnSaveMutualSettlements_Click(object sender, EventArgs e)
        {
            int MutualSettlementID = 0;
            if (dgvMutualSettlements.SelectedRows.Count > 0 && dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value != DBNull.Value)
                MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            MutualSettlementsManager.SaveMutualSettlements();
            MutualSettlementsManager.SaveMutualSettlementOrders();
            int ClientID = -1;
            if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            if (ClientID == -1)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            if (MutualSettlementsManager.NeedCreateIncome)
            {
                int LastMutualSettlementID = MutualSettlementsManager.LastMutualSettlementID(ClientID, FactoryID);
                MutualSettlementsManager.EditClientIncome(LastMutualSettlementID);
            }
            MutualSettlementsManager.SaveClientsIncomes();
            MutualSettlementsManager.SaveClientsDispatches();
            MutualSettlementsManager.DetachInocomeDocuments();
            MutualSettlementsManager.DetachDispatchDocuments();
            MutualSettlementsManager.GetAllClientsDispatches();
            MutualSettlementsManager.GetAllClientsIncomes();
            for (int i = 0; i < cbClients.Items.Count; i++)
            {
                int c = Convert.ToInt32(((DataRowView)cbClients.Items[i]).Row["ClientID"]);
                MutualSettlementsManager.CalcClosingBalance(c, 1);
                MutualSettlementsManager.CalcClosingBalance(c, 2);
            }
            MutualSettlementsManager.GetClientsIncomes(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetClientsDispatches(ClientID, FactoryID, date1, date2);
            //lbTotalClientDispatch.Text = MutualSettlementsManager.TotalClientDispatch();
            //lbTotalClientIncomes.Text = MutualSettlementsManager.TotalClientIncomes();
            MutualSettlementsManager.GetMutualSettlements(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetMutualSettlementOrders(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.MoveToMutualSettlement(MutualSettlementID);

            MutualSettlementsManager.FillNewMutualSettlements(FactoryID);
            MutualSettlementsManager.FillNewClientsDispatches(FactoryID);
            MutualSettlementsManager.NewMutualSettlementsBS.MoveFirst();
            MutualSettlementsManager.NewClientsDispatchesBS.MoveFirst();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvMutualSettlements_SelectionChanged(object sender, EventArgs e)
        {
            if (MutualSettlementsManager == null)
                return;
            int MutualSettlementID = 0;
            if (dgvMutualSettlements.SelectedRows.Count > 0 && dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value != DBNull.Value)
                MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            MutualSettlementsManager.FilterClientsIncomes(MutualSettlementID);
            MutualSettlementsManager.FilterClientsDispatches(MutualSettlementID);
        }

        private void kryptonContextMenuItem9_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            int MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            bool bOk = MutualSettlementsManager.AddInvoiceDocumentToDeleteTable(DocumentTypes.InvoiceExcel, MutualSettlementID);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ будет удалён после сохранения", 1700);
        }

        private void kryptonContextMenuItem11_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            int MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            bool bOk = MutualSettlementsManager.AddInvoiceDocumentToDeleteTable(DocumentTypes.InvoiceDbf, MutualSettlementID);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ будет удалён после сохранения", 1700);
        }

        private void kryptonContextMenuItem12_Click(object sender, EventArgs e)
        {
            if (dgvClientsDispatches.SelectedRows.Count == 0 || dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value == DBNull.Value)
                return;
            int ClientDispatchID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value);
            bool bOk = MutualSettlementsManager.AddDispatchDocumentToDeleteTable(DocumentTypes.DispatchExcel, ClientDispatchID);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ будет удалён после сохранения", 1700);
        }

        private void kryptonContextMenuItem13_Click(object sender, EventArgs e)
        {
            if (dgvClientsDispatches.SelectedRows.Count == 0 || dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value == DBNull.Value)
                return;
            int ClientDispatchID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value);
            bool bOk = MutualSettlementsManager.AddDispatchDocumentToDeleteTable(DocumentTypes.DispatchDbf, ClientDispatchID);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Документ будет удалён после сохранения", 1700);
        }

        private void dgvMutualSettlements_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if ((grid.Columns[e.ColumnIndex].Name == "InvoiceDbfName" || grid.Columns[e.ColumnIndex].Name == "InvoiceExcelName") && grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
                grid.Cursor = Cursors.Hand;
            else
                grid.Cursor = Cursors.Default;
        }

        private void dgvMutualSettlements_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            grid.Cursor = Cursors.Default;
        }

        private void dgvClientsDispatches_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if ((grid.Columns[e.ColumnIndex].Name == "DispatchDbfName" || grid.Columns[e.ColumnIndex].Name == "DispatchExcelName") && grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
                grid.Cursor = Cursors.Hand;
            else
                grid.Cursor = Cursors.Default;
        }

        private void dgvClientsDispatches_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            grid.Cursor = Cursors.Default;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (FM.TotalFileSize == 0)
                return;

            if (FM.Position == FM.TotalFileSize || FM.Position > FM.TotalFileSize)
            {
                timer1.Enabled = false;
                return;
            }
        }

        private void dgvMutualSettlements_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex == -1)
            //    return;
            //PercentageDataGrid grid = (PercentageDataGrid)sender;
            //if (grid.Columns[e.ColumnIndex].Name != "InvoiceDbfName" && grid.Columns[e.ColumnIndex].Name != "InvoiceExcelName")
            //    return;
            //int MutualSettlementDocumentID = -1;
            //string FileName = string.Empty;
            //if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
            //{
            //    if (grid.Columns[e.ColumnIndex].Name == "InvoiceExcelName" && dgvMutualSettlements.SelectedRows[0].Cells["InvoiceExcel"].Value != DBNull.Value)
            //    {
            //        MutualSettlementDocumentID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["InvoiceExcel"].Value);
            //        FileName = dgvMutualSettlements.SelectedRows[0].Cells["InvoiceExcelName"].Value.ToString();
            //    }
            //    if (grid.Columns[e.ColumnIndex].Name == "InvoiceDbfName" && dgvMutualSettlements.SelectedRows[0].Cells["InvoiceDbf"].Value != DBNull.Value)
            //    {
            //        MutualSettlementDocumentID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["InvoiceDbf"].Value);
            //        FileName = dgvMutualSettlements.SelectedRows[0].Cells["InvoiceDbfName"].Value.ToString();
            //    }
            //}
            //string temppath = string.Empty;
            //if (MutualSettlementDocumentID != -1)
            //{
            //    PhantomForm PhantomForm = new Infinium.PhantomForm();
            //    PhantomForm.Show();

            //    MuttlementDownloadForm MuttlementDownloadForm = new MuttlementDownloadForm(MutualSettlementDocumentID, FileName, ref MutualSettlementsManager);

            //    TopForm = MuttlementDownloadForm;

            //    MuttlementDownloadForm.ShowDialog();

            //    PhantomForm.Close();

            //    PhantomForm.Dispose();

            //    TopForm = null;

            //    MuttlementDownloadForm.Dispose();
            //}
        }

        private void dgvClientsDispatches_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex == -1)
            //    return;
            //PercentageDataGrid grid = (PercentageDataGrid)sender;
            //if (grid.Columns[e.ColumnIndex].Name != "DispatchDbfName" && grid.Columns[e.ColumnIndex].Name != "DispatchExcelName")
            //    return;
            //int MutualSettlementDocumentID = -1;
            //string FileName = string.Empty;
            //if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
            //{
            //    if (grid.Columns[e.ColumnIndex].Name == "DispatchExcelName" && dgvClientsDispatches.SelectedRows[0].Cells["DispatchExcel"].Value != DBNull.Value)
            //    {
            //        MutualSettlementDocumentID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["DispatchExcel"].Value);
            //        FileName = dgvClientsDispatches.SelectedRows[0].Cells["DispatchExcelName"].Value.ToString();
            //    }
            //    if (grid.Columns[e.ColumnIndex].Name == "DispatchDbfName" && dgvClientsDispatches.SelectedRows[0].Cells["DispatchDbf"].Value != DBNull.Value)
            //    {
            //        MutualSettlementDocumentID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["DispatchDbf"].Value);
            //        FileName = dgvClientsDispatches.SelectedRows[0].Cells["DispatchDbfName"].Value.ToString();
            //    }
            //}
            //string temppath = string.Empty;
            //if (MutualSettlementDocumentID != -1)
            //{
            //    PhantomForm PhantomForm = new Infinium.PhantomForm();
            //    PhantomForm.Show();

            //    MuttlementDownloadForm MuttlementDownloadForm = new MuttlementDownloadForm(MutualSettlementDocumentID, FileName, ref MutualSettlementsManager);

            //    TopForm = MuttlementDownloadForm;

            //    MuttlementDownloadForm.ShowDialog();

            //    PhantomForm.Close();

            //    PhantomForm.Dispose();

            //    TopForm = null;

            //    MuttlementDownloadForm.Dispose();
            //}
        }

        private void rbProfil_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbProfil.Checked)
                return;
            NeedFilterClients = false;
            FactoryID = 1;
            int ClientID = -1;
            if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            if (ClientID == -1)
                return;
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            MutualSettlementsManager.FillNewMutualSettlements(FactoryID);
            MutualSettlementsManager.FillNewClientsDispatches(FactoryID);
            MutualSettlementsManager.GetClientsIncomes(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetClientsDispatches(ClientID, FactoryID, date1, date2);
            //lbTotalClientDispatch.Text = MutualSettlementsManager.TotalClientDispatch();
            //lbTotalClientIncomes.Text = MutualSettlementsManager.TotalClientIncomes();
            MutualSettlementsManager.GetMutualSettlementOrders(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetMutualSettlements(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.NewMutualSettlementsBS.MoveFirst();
            MutualSettlementsManager.NewClientsDispatchesBS.MoveFirst();
            NeedFilterClients = true;
        }

        private void rbTPS_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbTPS.Checked)
                return;
            NeedFilterClients = false;
            FactoryID = 2;
            int ClientID = -1;
            if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            if (ClientID == -1)
                return;
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            MutualSettlementsManager.FillNewMutualSettlements(FactoryID);
            MutualSettlementsManager.FillNewClientsDispatches(FactoryID);
            MutualSettlementsManager.GetClientsIncomes(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetClientsDispatches(ClientID, FactoryID, date1, date2);
            //lbTotalClientDispatch.Text = MutualSettlementsManager.TotalClientDispatch();
            //lbTotalClientIncomes.Text = MutualSettlementsManager.TotalClientIncomes();
            MutualSettlementsManager.GetMutualSettlementOrders(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.GetMutualSettlements(ClientID, FactoryID, date1, date2);
            MutualSettlementsManager.NewMutualSettlementsBS.MoveFirst();
            MutualSettlementsManager.NewClientsDispatchesBS.MoveFirst();
            NeedFilterClients = true;
        }

        private void HelpCheckButton_Click(object sender, EventArgs e)
        {
            HelpPanel.Visible = !HelpPanel.Visible;
        }

        private void kryptonContextMenuItem15_Click(object sender, EventArgs e)
        {
            if (dgvMutualSettlements.SelectedRows.Count == 0 || dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value == DBNull.Value)
                return;
            int MutualSettlementID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            if (dgvClientsDispatches.SelectedRows.Count == 0 || dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value == DBNull.Value)
                return;
            int ClientDispatchID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["ClientDispatchID"].Value);
            MutualSettlementsManager.ConfirmVAT(ClientDispatchID);
            RefreshMutualSettlements();
            MutualSettlementsManager.MoveToMutualSettlement(MutualSettlementID);
        }

        private void btnCreateReportVAT_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            VATReportToExcel VATReportToExcel = new Modules.Marketing.MutualSettlements.VATReportToExcel();

            int RowIndex = 2;
            decimal TotalDispatchSum = 0;
            VATReportToExcel.ZOVProfilReport(MutualSettlementsManager.BYRReportVAT(1, ref TotalDispatchSum), "BYN", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            VATReportToExcel.ZOVProfilReport(MutualSettlementsManager.RUBReportVAT(1, ref TotalDispatchSum), "RUB", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            VATReportToExcel.ZOVProfilReport(MutualSettlementsManager.EURReportVAT(1, ref TotalDispatchSum), "EUR", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            VATReportToExcel.ZOVProfilReport(MutualSettlementsManager.USDReportVAT(1, ref TotalDispatchSum), "USD", TotalDispatchSum, ref RowIndex);

            RowIndex = 2;
            TotalDispatchSum = 0;
            VATReportToExcel.ZOVTPSReport(MutualSettlementsManager.BYRReportVAT(2, ref TotalDispatchSum), "BYN", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            VATReportToExcel.ZOVTPSReport(MutualSettlementsManager.RUBReportVAT(2, ref TotalDispatchSum), "RUB", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            VATReportToExcel.ZOVTPSReport(MutualSettlementsManager.EURReportVAT(2, ref TotalDispatchSum), "EUR", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            VATReportToExcel.ZOVTPSReport(MutualSettlementsManager.USDReportVAT(2, ref TotalDispatchSum), "USD", TotalDispatchSum, ref RowIndex);

            VATReportToExcel.SaveOpenReport();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnCreateInternationalContract_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ICReportToExcel ICReportToExcel = new Modules.Marketing.MutualSettlements.ICReportToExcel();

            int RowIndex = 2;
            decimal TotalDispatchSum = 0;
            ICReportToExcel.ZOVProfilReport(MutualSettlementsManager.BYRReportIC(1, ref TotalDispatchSum), "BYN", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            ICReportToExcel.ZOVProfilReport(MutualSettlementsManager.RUBReportIC(1, ref TotalDispatchSum), "RUB", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            ICReportToExcel.ZOVProfilReport(MutualSettlementsManager.EURReportIC(1, ref TotalDispatchSum), "EUR", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            ICReportToExcel.ZOVProfilReport(MutualSettlementsManager.USDReportIC(1, ref TotalDispatchSum), "USD", TotalDispatchSum, ref RowIndex);

            RowIndex = 2;
            TotalDispatchSum = 0;
            ICReportToExcel.ZOVTPSReport(MutualSettlementsManager.BYRReportIC(2, ref TotalDispatchSum), "BYN", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            ICReportToExcel.ZOVTPSReport(MutualSettlementsManager.RUBReportIC(2, ref TotalDispatchSum), "RUB", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            ICReportToExcel.ZOVTPSReport(MutualSettlementsManager.EURReportIC(2, ref TotalDispatchSum), "EUR", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            ICReportToExcel.ZOVTPSReport(MutualSettlementsManager.USDReportIC(2, ref TotalDispatchSum), "USD", TotalDispatchSum, ref RowIndex);
            ICReportToExcel.SaveOpenReport1();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnCreateReportDelayOfPayment_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DelayOfPaymentReportToExcel DelayOfPaymentReportToExcel = new Modules.Marketing.MutualSettlements.DelayOfPaymentReportToExcel();

            int RowIndex = 2;
            decimal TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVProfilReportDelayOfpayment(MutualSettlementsManager.BYRReportDelayOfPayment(1, ref TotalDispatchSum), "BYN", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVProfilReportDelayOfpayment(MutualSettlementsManager.RUBReportDelayOfPayment(1, ref TotalDispatchSum), "RUB", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVProfilReportDelayOfpayment(MutualSettlementsManager.EURReportDelayOfPayment(1, ref TotalDispatchSum), "EUR", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVProfilReportDelayOfpayment(MutualSettlementsManager.USDReportDelayOfPayment(1, ref TotalDispatchSum), "USD", TotalDispatchSum, ref RowIndex);

            RowIndex = 2;
            TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVTPSReportDelayOfpayment(MutualSettlementsManager.BYRReportDelayOfPayment(2, ref TotalDispatchSum), "BYN", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVTPSReportDelayOfpayment(MutualSettlementsManager.RUBReportDelayOfPayment(2, ref TotalDispatchSum), "RUB", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVTPSReportDelayOfpayment(MutualSettlementsManager.EURReportDelayOfPayment(2, ref TotalDispatchSum), "EUR", TotalDispatchSum, ref RowIndex);
            TotalDispatchSum = 0;
            DelayOfPaymentReportToExcel.ZOVTPSReportDelayOfpayment(MutualSettlementsManager.USDReportDelayOfPayment(2, ref TotalDispatchSum), "USD", TotalDispatchSum, ref RowIndex);
            DelayOfPaymentReportToExcel.SaveOpenReport2();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnCreateCashReport_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            CashReportParameters CashReportParameters = new CashReportParameters();
            MutualSettlementsFilterMenu = new Infinium.MutualSettlementsFilterMenu(this, MutualSettlementsManager);

            TopForm = MutualSettlementsFilterMenu;
            MutualSettlementsFilterMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            CashReportParameters = MutualSettlementsFilterMenu.CashReportParameters;
            MutualSettlementsFilterMenu.Dispose();
            TopForm = null;

            if (CashReportParameters.Cancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            CashReportToExcel CashReportToExcel = new Modules.Marketing.MutualSettlements.CashReportToExcel();

            if (CashReportParameters.AllClients)
            {
                decimal TotalOpeningBalance = 0;
                decimal TotalDispatchSum = 0;
                decimal TotalIncomeSum = 0;
                decimal TotalClosingBalance = 0;
                int RowIndex = 2;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.BYRReportCash(1, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "BYN", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.RUBReportCash(1, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "RUB", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.EURReportCash(1, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "EUR", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.USDReportCash(1, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "USD", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);

                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                RowIndex = 2;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.BYRReportCash(2, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "BYN", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.RUBReportCash(2, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "RUB", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.EURReportCash(2, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "EUR", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.USDReportCash(2, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "USD", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
            }
            else
            {
                decimal TotalOpeningBalance = 0;
                decimal TotalDispatchSum = 0;
                decimal TotalIncomeSum = 0;
                decimal TotalClosingBalance = 0;
                int RowIndex = 2;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.BYRReportCash(1, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "BYN", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.RUBReportCash(1, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "RUB", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.EURReportCash(1, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "EUR", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVProfilReport(MutualSettlementsManager.USDReportCash(1, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "USD", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);

                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                RowIndex = 2;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.BYRReportCash(2, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "BYN", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.RUBReportCash(2, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "RUB", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.EURReportCash(2, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "EUR", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
                TotalOpeningBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                TotalClosingBalance = 0;
                CashReportToExcel.ZOVTPSReport(MutualSettlementsManager.USDReportCash(2, CashReportParameters.Clients, CashReportParameters.Date1, CashReportParameters.Date2,
                    ref TotalOpeningBalance, ref TotalDispatchSum, ref TotalIncomeSum, ref TotalClosingBalance), "USD", TotalOpeningBalance, TotalDispatchSum, TotalIncomeSum, TotalClosingBalance, ref RowIndex);
            }
            CashReportToExcel.SaveOpenReport2();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvMutualSettlements_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int Result = 0;
            if (grid.Rows[e.RowIndex].Cells["CompareBalance"].Value != DBNull.Value)
                Result = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["CompareBalance"].Value);
            if (Result == 1)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Blue;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Blue;
            }
            if (Result == 3)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Green;
            }
            if (Result == 2)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
            if (Result == 0)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        public void FilterMutualSettlements()
        {
            bool Result0 = cbResult0.Checked;
            bool Result1 = cbResult1.Checked;
            bool Result2 = cbResult2.Checked;
            bool Result3 = cbResult3.Checked;
            int CurrencyTypeID = 5;
            if (kryptonRadioButton2.Checked)
                CurrencyTypeID = 3;
            if (kryptonRadioButton3.Checked)
                CurrencyTypeID = 2;
            if (kryptonRadioButton4.Checked)
                CurrencyTypeID = 1;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            MutualSettlementsManager.FilterMutualSettlements(Result0, Result1, Result2, Result3, CurrencyTypeID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbResult0_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void cbResult3_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void cbResult1_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void cbResult2_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void dgvNewMutualSettlements_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void dgvNewClientsDispatches_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void dgvNewMutualSettlements_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (MutualSettlementsManager == null || !NeedFilterClients)
                return;
            int ClientID = 0;
            int MutualSettlementID = 0;
            if (dgvNewMutualSettlements.SelectedRows.Count > 0 && dgvNewMutualSettlements.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                ClientID = Convert.ToInt32(dgvNewMutualSettlements.SelectedRows[0].Cells["ClientID"].Value);
            if (dgvNewMutualSettlements.SelectedRows.Count > 0 && dgvNewMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value != DBNull.Value)
                MutualSettlementID = Convert.ToInt32(dgvNewMutualSettlements.SelectedRows[0].Cells["MutualSettlementID"].Value);
            MutualSettlementsManager.MoveToClient(ClientID);
            MutualSettlementsManager.MoveToMutualSettlement(MutualSettlementID);
        }

        private void dgvNewClientsDispatches_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (MutualSettlementsManager == null || !NeedFilterClients)
                return;
            int ClientID = 0;
            int MutualSettlementID = 0;
            if (dgvNewClientsDispatches.SelectedRows.Count > 0 && dgvNewClientsDispatches.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                ClientID = Convert.ToInt32(dgvNewClientsDispatches.SelectedRows[0].Cells["ClientID"].Value);
            if (dgvNewClientsDispatches.SelectedRows.Count > 0 && dgvNewClientsDispatches.SelectedRows[0].Cells["MutualSettlementID"].Value != DBNull.Value)
                MutualSettlementID = Convert.ToInt32(dgvNewClientsDispatches.SelectedRows[0].Cells["MutualSettlementID"].Value);
            MutualSettlementsManager.MoveToClient(ClientID);
            MutualSettlementsManager.MoveToMutualSettlement(MutualSettlementID);
        }

        private void dgvClientsIncomes_SelectionChanged(object sender, EventArgs e)
        {
            decimal S1 = 0;
            decimal S2 = 0;
            decimal S3 = 0;
            decimal S4 = 0;

            for (int i = 0; i < dgvClientsIncomes.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(dgvClientsIncomes.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 1 && dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value != DBNull.Value)
                    S1 += Convert.ToDecimal(dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value);
                if (Convert.ToInt32(dgvClientsIncomes.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 2 && dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value != DBNull.Value)
                    S2 += Convert.ToDecimal(dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value);
                if (Convert.ToInt32(dgvClientsIncomes.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 3 && dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value != DBNull.Value)
                    S3 += Convert.ToDecimal(dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value);
                if (Convert.ToInt32(dgvClientsIncomes.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 5 && dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value != DBNull.Value)
                    S4 += Convert.ToDecimal(dgvClientsIncomes.SelectedRows[i].Cells["IncomeSum"].Value);
            }
            S1 = Decimal.Round(S1, 3, MidpointRounding.AwayFromZero);
            S2 = Decimal.Round(S2, 3, MidpointRounding.AwayFromZero);
            S3 = Decimal.Round(S3, 3, MidpointRounding.AwayFromZero);
            S4 = Decimal.Round(S4, 3, MidpointRounding.AwayFromZero);

            label9.Text = S1.ToString("C", nfi1) + " EUR";
            label8.Text = S2.ToString("C", nfi1) + " USD";
            label6.Text = S3.ToString("C", nfi1) + " RUB";
            lbTotalClientIncomes.Text = S4.ToString("C", nfi1) + " BYN";
            if (S1 == 0)
                label9.Visible = false;
            else
                label9.Visible = true;
            if (S2 == 0)
                label8.Visible = false;
            else
                label8.Visible = true;
            if (S3 == 0)
                label6.Visible = false;
            else
                label6.Visible = true;
            if (S4 == 0)
                lbTotalClientIncomes.Visible = false;
            else
                lbTotalClientIncomes.Visible = true;
        }

        private void dgvClientsDispatches_SelectionChanged(object sender, EventArgs e)
        {
            decimal S1 = 0;
            decimal S2 = 0;
            decimal S3 = 0;
            decimal S4 = 0;

            for (int i = 0; i < dgvClientsDispatches.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(dgvClientsDispatches.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 1)
                    S1 += Convert.ToDecimal(dgvClientsDispatches.SelectedRows[i].Cells["DispatchSum"].Value);
                if (Convert.ToInt32(dgvClientsDispatches.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 2)
                    S2 += Convert.ToDecimal(dgvClientsDispatches.SelectedRows[i].Cells["DispatchSum"].Value);
                if (Convert.ToInt32(dgvClientsDispatches.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 3)
                    S3 += Convert.ToDecimal(dgvClientsDispatches.SelectedRows[i].Cells["DispatchSum"].Value);
                if (Convert.ToInt32(dgvClientsDispatches.SelectedRows[i].Cells["CurrencyTypeID"].Value) == 5)
                    S4 += Convert.ToDecimal(dgvClientsDispatches.SelectedRows[i].Cells["DispatchSum"].Value);
            }
            S1 = Decimal.Round(S1, 3, MidpointRounding.AwayFromZero);
            S2 = Decimal.Round(S2, 3, MidpointRounding.AwayFromZero);
            S3 = Decimal.Round(S3, 3, MidpointRounding.AwayFromZero);
            S4 = Decimal.Round(S4, 3, MidpointRounding.AwayFromZero);

            label10.Text = S1.ToString("C", nfi1) + " EUR";
            label11.Text = S2.ToString("C", nfi1) + " USD";
            label12.Text = S3.ToString("C", nfi1) + " RUB";
            label13.Text = S4.ToString("C", nfi1) + " BYN";
            if (S1 == 0)
                label10.Visible = false;
            else
                label10.Visible = true;
            if (S2 == 0)
                label11.Visible = false;
            else
                label11.Visible = true;
            if (S3 == 0)
                label12.Visible = false;
            else
                label12.Visible = true;
            if (S4 == 0)
                label13.Visible = false;
            else
                label13.Visible = true;
        }

        private void kryptonRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void kryptonRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void kryptonRadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void kryptonRadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            FilterMutualSettlements();
        }

        private void dgvMutualSettlements_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns[e.ColumnIndex].Name != "InvoiceDbfName" && grid.Columns[e.ColumnIndex].Name != "InvoiceExcelName")
                return;
            int MutualSettlementDocumentID = -1;
            string FileName = string.Empty;
            if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
            {
                if (grid.Columns[e.ColumnIndex].Name == "InvoiceExcelName" && dgvMutualSettlements.SelectedRows[0].Cells["InvoiceExcel"].Value != DBNull.Value)
                {
                    MutualSettlementDocumentID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["InvoiceExcel"].Value);
                    FileName = dgvMutualSettlements.SelectedRows[0].Cells["InvoiceExcelName"].Value.ToString();
                }
                if (grid.Columns[e.ColumnIndex].Name == "InvoiceDbfName" && dgvMutualSettlements.SelectedRows[0].Cells["InvoiceDbf"].Value != DBNull.Value)
                {
                    MutualSettlementDocumentID = Convert.ToInt32(dgvMutualSettlements.SelectedRows[0].Cells["InvoiceDbf"].Value);
                    FileName = dgvMutualSettlements.SelectedRows[0].Cells["InvoiceDbfName"].Value.ToString();
                }
            }
            string temppath = string.Empty;
            if (MutualSettlementDocumentID != -1)
            {
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                MuttlementDownloadForm MuttlementDownloadForm = new MuttlementDownloadForm(MutualSettlementDocumentID, FileName, ref MutualSettlementsManager);

                TopForm = MuttlementDownloadForm;

                MuttlementDownloadForm.ShowDialog();

                PhantomForm.Close();

                PhantomForm.Dispose();

                TopForm = null;

                MuttlementDownloadForm.Dispose();
            }
        }

        private void dgvClientsDispatches_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns[e.ColumnIndex].Name != "DispatchDbfName" && grid.Columns[e.ColumnIndex].Name != "DispatchExcelName")
                return;
            int MutualSettlementDocumentID = -1;
            string FileName = string.Empty;
            if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
            {
                if (grid.Columns[e.ColumnIndex].Name == "DispatchExcelName" && dgvClientsDispatches.SelectedRows[0].Cells["DispatchExcel"].Value != DBNull.Value)
                {
                    MutualSettlementDocumentID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["DispatchExcel"].Value);
                    FileName = dgvClientsDispatches.SelectedRows[0].Cells["DispatchExcelName"].Value.ToString();
                }
                if (grid.Columns[e.ColumnIndex].Name == "DispatchDbfName" && dgvClientsDispatches.SelectedRows[0].Cells["DispatchDbf"].Value != DBNull.Value)
                {
                    MutualSettlementDocumentID = Convert.ToInt32(dgvClientsDispatches.SelectedRows[0].Cells["DispatchDbf"].Value);
                    FileName = dgvClientsDispatches.SelectedRows[0].Cells["DispatchDbfName"].Value.ToString();
                }
            }
            string temppath = string.Empty;
            if (MutualSettlementDocumentID != -1)
            {
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                MuttlementDownloadForm MuttlementDownloadForm = new MuttlementDownloadForm(MutualSettlementDocumentID, FileName, ref MutualSettlementsManager);

                TopForm = MuttlementDownloadForm;

                MuttlementDownloadForm.ShowDialog();

                PhantomForm.Close();

                PhantomForm.Dispose();

                TopForm = null;

                MuttlementDownloadForm.Dispose();
            }
        }

        private void dgvMutualSettlements_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            PercentageDataGrid grid = (PercentageDataGrid)sender;

            if (e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int ClientID = -1;
                if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                    ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
                string DisplayName = string.Empty;
                DisplayName = MutualSettlementsManager.ManagerName(ClientID);
                cell.ToolTipText = DisplayName;
            }
        }

        private void dgvNewMutualSettlements_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            PercentageDataGrid grid = (PercentageDataGrid)sender;

            if (e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int ClientID;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["ClientID"].Value != DBNull.Value)
                {
                    ClientID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["ClientID"].Value);
                    DisplayName = MutualSettlementsManager.ManagerName(ClientID);
                }
                cell.ToolTipText = DisplayName;
            }
        }

        private void dgvNewClientsDispatches_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            PercentageDataGrid grid = (PercentageDataGrid)sender;

            if (e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int ClientID;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["ClientID"].Value != DBNull.Value)
                {
                    ClientID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["ClientID"].Value);
                    DisplayName = MutualSettlementsManager.ManagerName(ClientID);
                }
                cell.ToolTipText = DisplayName;
            }
        }
    }
}