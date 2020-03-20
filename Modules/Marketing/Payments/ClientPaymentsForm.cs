using Infinium.Modules.Marketing.Payments;

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ClientPaymentsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        FileManager FM = null;

        bool bStopTransfer = false;

        public int AttachsCount = 0;

        int FormEvent = 0;

        LightStartForm LightStartForm;


        Form TopForm = null;

        ClientPayments ClientPayments;
        DataTable TableCurrency, TableClients, TableContract, TableFactory;

        System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ru-RU");
        System.Globalization.NumberFormatInfo nfi1;

        public DataTable AttachmentDocumentsDataTable;
        public BindingSource AttachmentDocumentsBindingSource;

        public ClientPaymentsForm(LightStartForm tLightStartForm)
        {
            TableCurrency = new DataTable();
            TableClients = new DataTable();
            TableContract = new DataTable();
            TableFactory = new DataTable();

            InitializeComponent();
            ClientPayments = new ClientPayments();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            CreateAttachments();
            Initialize();

            nfi1 = new System.Globalization.NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            while (!SplashForm.bCreated) ;
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


            if (FormEvent == eShow)
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

        private void CreateAttachments()
        {
            AttachmentDocumentsDataTable = new DataTable();
            AttachmentDocumentsDataTable.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            AttachmentDocumentsDataTable.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));

            AttachmentDocumentsBindingSource = new BindingSource()
            {
                DataSource = AttachmentDocumentsDataTable
            };
            AttachmentsDocumentGrid.DataSource = AttachmentDocumentsBindingSource;

            AttachmentsDocumentGrid.Columns["Path"].Visible = false;
            AttachmentsDocumentGrid.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            AttachmentsDocumentGrid.Columns["FileName"].Width = 250;
        }

        private void CopyAttachs(string ContractID)
        {
            using (DataTable DT = ClientPayments.GetAttachments(ContractID))
            {
                if (DT.Rows.Count == 0)
                    return;

                AttachsCount = DT.Rows.Count;

                foreach (DataRow Row in DT.Rows)
                {
                    DataRow NewRow = AttachmentDocumentsDataTable.NewRow();
                    NewRow["FileName"] = Row["FileName"];
                    NewRow["Path"] = "server";
                    AttachmentDocumentsDataTable.Rows.Add(NewRow);
                }
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
            ErrorLabel.Visible = false;
            ErrorContractLabel.Visible = false;
            AllSumCheckBox.Visible = false;
            ClientPayments = new ClientPayments(ref ClientsPaymentsDataGrid, ref ClientContractDataGrid);

            TableCurrency = ClientPayments.TableCurrency();
            TableClients = ClientPayments.TableClients();
            TableFactory = ClientPayments.TableFactory();

            ClientComboBox.DataSource = TableClients;
            ClientComboBox.DisplayMember = "ClientName";
            ClientComboBox.ValueMember = "ClientID";

            ClientContractComboBox.DataSource = TableClients;
            ClientContractComboBox.DisplayMember = "ClientName";
            ClientContractComboBox.ValueMember = "ClientID";

            ContractFilteComboBox.DataSource = ClientPayments.ClientContractBindingSource;
            ContractFilteComboBox.DisplayMember = "ContractNumber";
            ContractFilteComboBox.ValueMember = "ContractId";

            ContractFilteComboBox.Visible = false;
            PeriodButton.Visible = false;
            CalendarTo.Visible = false;
            FilterContractButton.Visible = false;
            ContractCalendarTo.Visible = false;
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

        private void RemovePaymentsButton_Click(object sender, EventArgs e)
        {
            if (ClientsPaymentsDataGrid.SelectedRows.Count == 1)
            {
                if (Infinium.LightMessageBox.Show(ref TopForm, true, "Вы уверены, что хотите удалить?", "Удаление"))
                {
                    ClientPayments.DeletePayments(ClientsPaymentsDataGrid.SelectedRows[0].Cells["ClientPaymentsID"].Value.ToString());
                    ClientPayments.UpdateClientsPaymentsDataGrid();

                    UpdateLabelPayments();
                    ClientPayments.Record();

                    ClientPayments.UpdateTableContract();
                }
            }
        }

        private void AddPaymentsButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddPaymentsForm AddPaymentsForm = new AddPaymentsForm(ClientComboBox.SelectedValue.ToString(), ref ClientPayments, ref TableCurrency, ref TableClients, ref TopForm, ref TableContract, ref TableFactory);

            TopForm = AddPaymentsForm;

            AddPaymentsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (ClientCheckBox.Checked)
                UpdateLabelPayments();
            ClientPayments.Record();
            ClientPayments.UpdateTableContract();
        }

        private void PeriodButton_Click(object sender, EventArgs e)
        {
            if (PeriodCheckBox.Checked)
            {
                if (CalendarTo.SelectionEnd < CalendarFrom.SelectionEnd)
                {
                    ErrorLabel.Visible = true;
                    return;
                }
                else
                {
                    ErrorLabel.Visible = false;
                }

                ClientPayments.CheckClientAndPeriod(ClientComboBox.SelectedValue.ToString(), CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, ClientCheckBox.Checked, TPSCheckBox.Checked);

                UpdateLabelPayments();
            }
        }

        private void ClientComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (ClientCheckBox.Checked)
            {
                ContractFilteComboBox.DataSource = ClientPayments.Filter(((DataRowView)ClientComboBox.SelectedItem).Row["ClientID"].ToString());
                if (TPSCheckBox.Checked)
                {
                    FilterContractCheckBox.Checked = false;
                    ProfilCheckBox.Checked = false;
                    ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                }
                else
                {
                    if (!ProfilCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                        UpdateLabelPayments();
                    }
                }

                if (ProfilCheckBox.Checked)
                {
                    FilterContractCheckBox.Checked = false;
                    TPSCheckBox.Checked = false;
                    ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                }
                else
                {
                    if (!TPSCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                        UpdateLabelPayments();
                    }
                }
            }
        }

        private void UpdatePaymentsButton_Click(object sender, EventArgs e)
        {
            if (ClientsPaymentsDataGrid.SelectedRows.Count == 1)
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                AddPaymentsForm AddPaymentsForm = new AddPaymentsForm(ClientsPaymentsDataGrid.SelectedRows[0].Cells["ClientID"].Value.ToString(), ref ClientPayments, ref TableCurrency, ref TableClients, ref TopForm, ClientsPaymentsDataGrid.SelectedRows[0].Cells["TypePayments"].Value.ToString(), ClientsPaymentsDataGrid.SelectedRows[0].Cells["DocNumber"].Value.ToString(), ClientsPaymentsDataGrid.SelectedRows[0].Cells["Cost"].Value.ToString(), ClientsPaymentsDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value.ToString(), Convert.ToDateTime(ClientsPaymentsDataGrid.SelectedRows[0].Cells["Date"].Value), ClientsPaymentsDataGrid.SelectedRows[0].Cells["ClientPaymentsID"].Value.ToString(), ref TableFactory, ClientsPaymentsDataGrid.SelectedRows[0].Cells["FactoryID"].Value.ToString());

                TopForm = AddPaymentsForm;

                AddPaymentsForm.ShowDialog();

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;

                UpdateLabelPayments();
                ClientPayments.Record();
                ClientPayments.UpdateTableContract();
            }
        }

        private void ClientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ClientCheckBox.Checked)
            {
                ContractFilteComboBox.DataSource = ClientPayments.Filter(((DataRowView)ClientComboBox.SelectedItem).Row["ClientID"].ToString());
                if (TPSCheckBox.Checked)
                {
                    ProfilCheckBox.Checked = false;
                    ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                    UpdateLabelPayments();
                }
                else
                {
                    if (!ProfilCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                        //UpdateLabelPayments();
                    }
                }

                if (ProfilCheckBox.Checked)
                {
                    TPSCheckBox.Checked = false;
                    ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                    UpdateLabelPayments();
                }
                else
                {
                    if (!TPSCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                        //UpdateLabelPayments();
                    }
                }
            }
            else
            {
                if (TPSCheckBox.Checked)
                {
                    ProfilCheckBox.Checked = false;
                    ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                    UpdateLabelPayments();
                }
                else
                {
                    if (!ProfilCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                        NameLabel.Text = "";
                        TotalLabel.Text = "";
                        SumShipmentLabel.Text = "";
                        SumPaymentLabel.Text = "";
                    }
                }

                if (ProfilCheckBox.Checked)
                {
                    TPSCheckBox.Checked = false;
                    ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                    UpdateLabelPayments();
                }
                else
                {
                    if (!TPSCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                        NameLabel.Text = "";
                        TotalLabel.Text = "";
                        SumShipmentLabel.Text = "";
                        SumPaymentLabel.Text = "";
                    }
                }
            }
        }

        private void UpdateLabelPayments()
        {
            if (ClientPayments.ClientPaymentsTable.Rows.Count >= 1)
            {
                foreach (DataRow Row in ClientPayments.ClientPaymentsTable.Rows)
                {
                    if (Row["CurrencyTypeID"].ToString() == "1")
                    {
                        ClientPayments.SummMoneyEURPayments();
                        NameLabel.Text = "Евро";

                        TotalLabel.Text = ClientPayments.CostEUR.ToString("N", nfi1);
                        SumShipmentLabel.Text = ClientPayments.CostEURShipping.ToString("N", nfi1);
                        SumPaymentLabel.Text = ClientPayments.CostEURPayment.ToString("N", nfi1);
                    }

                    if (Row["CurrencyTypeID"].ToString() == "2")
                    {
                        ClientPayments.SummMoneyUSDPayments();
                        NameLabel.Text = "Доллар";
                        TotalLabel.Text = ClientPayments.CostUSD.ToString("N", nfi1);
                        SumShipmentLabel.Text = ClientPayments.CostUSDShipping.ToString("N", nfi1);
                        SumPaymentLabel.Text = ClientPayments.CostUSDPayment.ToString("N", nfi1);
                    }

                    if (Row["CurrencyTypeID"].ToString() == "3")
                    {
                        ClientPayments.SummMoneyRURPayments();
                        NameLabel.Text = "Рубль(RUR)";
                        TotalLabel.Text = ClientPayments.CostRUR.ToString("N", nfi1);
                        SumShipmentLabel.Text = ClientPayments.CostRURShipping.ToString("N", nfi1);
                        SumPaymentLabel.Text = ClientPayments.CostRURPayment.ToString("N", nfi1);
                    }

                    if (Row["CurrencyTypeID"].ToString() == "5")
                    {
                        ClientPayments.SummMoneyBYRPayments();
                        NameLabel.Text = "Рубль(BYN)";
                        TotalLabel.Text = ClientPayments.CostBYR.ToString("N", nfi1);
                        SumShipmentLabel.Text = ClientPayments.CostBYRShipping.ToString("N", nfi1);
                        SumPaymentLabel.Text = ClientPayments.CostBYRPayment.ToString("N", nfi1);
                    }
                }
            }
            else
            {
                NameLabel.Text = "";
                TotalLabel.Text = "";
                SumShipmentLabel.Text = "";
                SumPaymentLabel.Text = "";
            }
        }

        private void UpdateLabelContracts()
        {
            if (ClientPayments.ClientContractTable.Rows.Count >= 1)
            {
                foreach (DataRow Row in ClientPayments.ClientContractTable.Rows)
                {
                    if (Row["CurrencyTypeID"].ToString() == "1")
                    {
                        ClientPayments.SummMoneyEURContracts();
                        CurrencyContractLabel.Text = "Евро";
                        SumContractLabel.Text = ClientPayments.CostEUR.ToString("N", nfi1);
                    }

                    if (Row["CurrencyTypeID"].ToString() == "2")
                    {
                        ClientPayments.SummMoneyUSDContracts();
                        CurrencyContractLabel.Text = "Доллар";
                        SumContractLabel.Text = ClientPayments.CostUSD.ToString("N", nfi1);
                    }

                    if (Row["CurrencyTypeID"].ToString() == "3")
                    {
                        ClientPayments.SummMoneyRURContracts();
                        CurrencyContractLabel.Text = "Рубль(RUR)";
                        SumContractLabel.Text = ClientPayments.CostRUR.ToString("N", nfi1);
                    }

                    if (Row["CurrencyTypeID"].ToString() == "5")
                    {
                        ClientPayments.SummMoneyBYRContracts();
                        CurrencyContractLabel.Text = "Рубль(BYN)";
                        SumContractLabel.Text = ClientPayments.CostBYR.ToString("N", nfi1);
                    }
                }
            }
        }

        private void UpdateLabelContractsWith()
        {
            if (ClientContractDataGrid.SelectedRows.Count == 1)
            {
                decimal SUMContract;
                SUMContract = Convert.ToDecimal(ClientContractDataGrid.SelectedRows[0].Cells["Cost"].Value);
                SumContractLabel.Text = SUMContract.ToString("N", nfi1);
                if (ClientContractDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value.ToString() == "1")
                    CurrencyContractLabel.Text = "Евро";
                if (ClientContractDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value.ToString() == "2")
                    CurrencyContractLabel.Text = "Доллар";
                if (ClientContractDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value.ToString() == "3")
                    CurrencyContractLabel.Text = "Рубль(RUR)";
                if (ClientContractDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value.ToString() == "5")
                    CurrencyContractLabel.Text = "Рубль(BYN)";
            }
        }

        //private void Warning()
        //{
        //    if (ClientContractDataGrid.SelectedRows.Count == 1)
        //    {
        //        TimeSpan ts = (DateTime)ClientContractDataGrid.SelectedRows[0].Cells["EndDateContract"].Value - DateTime.Now;
        //        int DayCount = ts.Days;
        //        if (DayCount < 6)
        //        {
        //            PhantomForm PhantomForm = new Infinium.PhantomForm();
        //            PhantomForm.Show();

        //            WarningForm WarningForm = new WarningForm(DayCount, ClientContractDataGrid.SelectedRows[0].Cells["ContractNumber"].Value.ToString());

        //            TopForm = WarningForm;
        //            WarningForm.ShowDialog();

        //            PhantomForm.Close();

        //            PhantomForm.Dispose();
        //            WarningForm.Dispose();
        //            TopForm = null;
        //        }
        //    }
        //}

        private void PeriodCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PeriodCheckBox.Checked)
            {
                PeriodButton.Visible = true;
                CalendarTo.Visible = true;
            }
            else
            {
                PeriodButton.Visible = false;
                CalendarTo.Visible = false;
            }
        }

        private void AddContractButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddContractForm AddContractForm = new AddContractForm(ClientComboBox.SelectedValue.ToString(), ref ClientPayments, ref TableCurrency, ref TableClients, ref TopForm, ref TableFactory);

            TopForm = AddContractForm;

            AddContractForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (ClientContractCheckBox.Checked)
                UpdateLabelContracts();
        }

        private void UpdateContractButton_Click(object sender, EventArgs e)
        {
            if (ClientContractDataGrid.SelectedRows.Count == 1)
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                AddContractForm AddContractForm = new AddContractForm(ClientContractDataGrid.SelectedRows[0].Cells["ClientID"].Value.ToString(), ref ClientPayments, ref TableCurrency, ref TableClients, ref TopForm, ClientContractDataGrid.SelectedRows[0].Cells["ContractNumber"].Value.ToString(), ClientContractDataGrid.SelectedRows[0].Cells["Cost"].Value.ToString(), ClientContractDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value.ToString(), Convert.ToDateTime(ClientContractDataGrid.SelectedRows[0].Cells["StartDateContract"].Value), Convert.ToDateTime(ClientContractDataGrid.SelectedRows[0].Cells["EndDateContract"].Value.ToString()), ClientContractDataGrid.SelectedRows[0].Cells["ContractId"].Value.ToString(), ref TableFactory, ClientContractDataGrid.SelectedRows[0].Cells["FactoryID"].Value.ToString());

                TopForm = AddContractForm;

                AddContractForm.ShowDialog();

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
                ClientPayments.Record();
            }
        }

        //private void DeleteContractButton_Click(object sender, EventArgs e)
        //{
        //    if (ClientContractDataGrid.SelectedRows.Count == 1)
        //    {
        //        ClientPayments.CloseContract(ClientContractDataGrid.SelectedRows[0].Cells["ContractId"].Value.ToString());
        //        ClientPayments.UpdateClientsContractDataGrid();
        //        UpdateLabelContracts();

        //        //ClientPayments.GetFilter(ClientContractDataGrid.SelectedRows[0].Cells["ContractId"].Value.ToString());
        //        //if (ClientPayments.FilterContract.Rows.Count == 0)
        //        //{
        //        //    if (Infinium.LightMessageBox.Show(ref TopForm, true, "Вы уверены, что хотите удалить?", "Удаление"))
        //        //    {
        //        //        ClientPayments.DeleteContracts(ClientContractDataGrid.SelectedRows[0].Cells["ContractId"].Value.ToString());
        //        //        ClientPayments.UpdateClientsContractDataGrid();

        //        //        UpdateLabelContracts();
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    if (Infinium.LightMessageBox.Show(ref TopForm, true, "Вы пытаетесь удалить договор на который прикреплена накладная!", "Удаление"))
        //        //    {
        //        //        return;
        //        //    }
        //        //}
        //    }
        //}

        private void ClientContractComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (ClientContractCheckBox.Checked)
            {
                if (TPSContractCheckBox.Checked)
                {
                    ProfilContractCheckBox.Checked = false;
                    ClientPayments.UpdateClientsContractDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                }
                else
                {
                    if (!ProfilContractCheckBox.Checked)
                    {
                        AllSumCheckBox.Visible = true;
                        ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                        if (AllSumCheckBox.Checked)
                            UpdateLabelContracts();
                        ClientPayments.Record();
                    }
                }

                if (ProfilContractCheckBox.Checked)
                {
                    TPSContractCheckBox.Checked = false;
                    ClientPayments.UpdateClientsPaymentsDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                }
                else
                {
                    if (!TPSContractCheckBox.Checked)
                    {
                        AllSumCheckBox.Visible = true;
                        ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                        if (AllSumCheckBox.Checked)
                            UpdateLabelContracts();
                        ClientPayments.Record();
                    }
                }
            }
        }

        private void ClientContractCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ClientContractCheckBox.Checked)
            {
                if (TPSContractCheckBox.Checked)
                {
                    ProfilContractCheckBox.Checked = false;
                    ClientPayments.UpdateClientsContractDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                }
                else
                {
                    if (!ProfilContractCheckBox.Checked)
                    {
                        AllSumCheckBox.Visible = true;
                        ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                        ClientPayments.Record();
                    }
                }

                if (ProfilContractCheckBox.Checked)
                {
                    TPSContractCheckBox.Checked = false;
                    ClientPayments.UpdateClientsContractDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                }
                else
                {
                    if (!TPSContractCheckBox.Checked)
                    {
                        AllSumCheckBox.Visible = true;
                        ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                        ClientPayments.Record();
                    }
                }
            }
            else
            {
                if (TPSContractCheckBox.Checked)
                {
                    ProfilContractCheckBox.Checked = false;
                    ClientPayments.UpdateClientsContractDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                }
                else
                {
                    if (!ProfilContractCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                        CurrencyContractLabel.Text = "";
                        SumContractLabel.Text = "";
                        AllSumCheckBox.Visible = false;
                        UpdateLabelContractsWith();
                    }
                }

                if (ProfilContractCheckBox.Checked)
                {
                    TPSContractCheckBox.Checked = false;
                    ClientPayments.UpdateClientsContractDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                }
                else
                {
                    if (!TPSContractCheckBox.Checked)
                    {
                        ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                        CurrencyContractLabel.Text = "";
                        SumContractLabel.Text = "";
                        AllSumCheckBox.Visible = false;
                        UpdateLabelContractsWith();
                    }
                }
            }
        }

        private void PeriodContractCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PeriodContractCheckBox.Checked)
            {
                FilterContractButton.Visible = true;
                ContractCalendarTo.Visible = true;
            }
            else
            {
                FilterContractButton.Visible = false;
                ContractCalendarTo.Visible = false;
            }
        }

        private void FilterContractButton_Click(object sender, EventArgs e)
        {
            if (PeriodCheckBox.Checked)
            {
                if (ContractCalendarTo.SelectionEnd < ContractCalendarFrom.SelectionEnd)
                {
                    ErrorContractLabel.Visible = true;
                    return;
                }
                else
                {
                    ErrorContractLabel.Visible = false;
                }

                ClientPayments.CheckClientContractAndPeriod(ClientContractComboBox.SelectedValue.ToString(), ContractCalendarFrom.SelectionEnd, ContractCalendarTo.SelectionEnd, ClientContractCheckBox.Checked);

            }
        }

        private void ClientPaymentsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void ClientContractDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ClientContractDataGrid.SelectedRows.Count == 1)
            {
                UpdateLabelContractsWith();
                AllSumCheckBox.Checked = false;

                if (ClientPayments != null)
                    AttachmentDocumentsDataTable.Clear();
                CopyAttachs(ClientContractDataGrid.SelectedRows[0].Cells["ContractId"].Value.ToString());
            }

        }

        private void AllSumCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AllSumCheckBox.Checked)
            {
                UpdateLabelContracts();
                ClientPayments.Record();
            }
            else
            {
                UpdateLabelContractsWith();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2)
            {
                int j = 0;
                ClientPayments.Record();
                FilterContractCheckBox.Checked = false;
                for (int i = 0; i < ClientPayments.ClientContractTable.Rows.Count; i++)
                {
                    TimeSpan ts = (DateTime)ClientPayments.ClientContractTable.Rows[i]["EndDateContract"] - DateTime.Now;
                    int DayCount = ts.Days;

                    if (Convert.ToBoolean(ClientPayments.ClientContractTable.Rows[i]["Terminated"]) != true)
                    {
                        if (DayCount < 10 && DayCount > -1)
                        {
                            j++;
                            //ClientContractDataGrid.Rows[i].Selected = true;
                            ClientContractDataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                            //PhantomForm PhantomForm = new Infinium.PhantomForm();
                            //PhantomForm.Show();

                            //WarningForm WarningForm = new WarningForm(DayCount, ClientPayments.ClientContractTable.Rows[i]["ContractNumber"].ToString());

                            //TopForm = WarningForm;
                            //WarningForm.ShowDialog();

                            //PhantomForm.Close();

                            //PhantomForm.Dispose();
                            //WarningForm.Dispose();
                            //TopForm = null;
                        }
                    }

                    if (j == 0)
                    {
                        label4.Visible = false;
                        CountLabel.Text = "";
                    }
                    else
                    {
                        label4.Visible = true;
                        if (j == 1)
                            CountLabel.Text = j.ToString() + " договоре";
                        else
                            CountLabel.Text = j.ToString() + " договорах";
                    }
                }
            }

            if (tabControl1.SelectedTab == tabPage1)
            {
                TPSContractCheckBox.Checked = false;
                ProfilContractCheckBox.Checked = false;
                ClientContractCheckBox.Checked = false;
                PeriodContractCheckBox.Checked = false;
                AllSumCheckBox.Checked = false;
            }
        }

        private void CloseContractButton_Click(object sender, EventArgs e)
        {
            if (ClientContractDataGrid.SelectedRows.Count == 1)
            {
                if (Infinium.LightMessageBox.Show(ref TopForm, true, "Вы уверены, что закрыть договор?", "Закрытие"))
                {
                    ClientPayments.CloseContract(ClientContractDataGrid.SelectedRows[0].Cells["ContractId"].Value.ToString());
                    ClientPayments.UpdateClientsContractDataGrid();
                    UpdateLabelContracts();
                }
            }
        }

        private void TPSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (TPSCheckBox.Checked)
            {
                FilterContractCheckBox.Checked = false;
                ProfilCheckBox.Checked = false;
                ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                UpdateLabelPayments();
            }
            else
            {
                ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                NameLabel.Text = "";
                TotalLabel.Text = "";
                SumShipmentLabel.Text = "";
                SumPaymentLabel.Text = "";
            }
        }

        private void ProfilCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfilCheckBox.Checked)
            {
                FilterContractCheckBox.Checked = false;
                TPSCheckBox.Checked = false;
                ClientPayments.UpdateClientsPaymentsDataGrid(ClientComboBox.SelectedValue.ToString(), TPSCheckBox.Checked, ProfilCheckBox.Checked, ClientCheckBox.Checked);
                UpdateLabelPayments();
            }
            else
            {
                ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                NameLabel.Text = "";
                TotalLabel.Text = "";
                SumShipmentLabel.Text = "";
                SumPaymentLabel.Text = "";
            }
        }

        private void TPSContractCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (TPSContractCheckBox.Checked)
            {
                ProfilContractCheckBox.Checked = false;
                ClientPayments.UpdateClientsContractDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                UpdateLabelContracts();
                ClientPayments.Record();
            }
            else
            {
                ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                CurrencyContractLabel.Text = "";
                SumContractLabel.Text = "";
                AllSumCheckBox.Visible = false;
                UpdateLabelContractsWith();
                ClientPayments.Record();
            }
        }

        private void ProfilContractCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfilContractCheckBox.Checked)
            {
                TPSContractCheckBox.Checked = false;
                ClientPayments.UpdateClientsContractDataGrid(ClientContractComboBox.SelectedValue.ToString(), TPSContractCheckBox.Checked, ProfilContractCheckBox.Checked, ClientContractCheckBox.Checked);
                UpdateLabelContracts();
                ClientPayments.Record();
            }
            else
            {
                ClientPayments.GetAllPeriodContracts(ClientContractCheckBox.Checked, ClientContractComboBox.SelectedValue.ToString());
                CurrencyContractLabel.Text = "";
                SumContractLabel.Text = "";
                AllSumCheckBox.Visible = false;
                UpdateLabelContractsWith();
                ClientPayments.Record();
            }
        }

        private void FilterContractCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterContractCheckBox.Checked)
            {
                //ContractFilteComboBox.DataSource = ClientPayments.Filter(((DataRowView)ClientComboBox.SelectedItem).Row["ClientID"].ToString());
                ContractFilteComboBox.Visible = true;
                ProfilCheckBox.Checked = false;
                TPSCheckBox.Checked = false;
                ClientPayments.UpdateClientsPaymentsDataGrid(ContractFilteComboBox.SelectedValue.ToString());
                UpdateLabelPayments();
            }
            else
            {
                ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                NameLabel.Text = "";
                TotalLabel.Text = "";
                SumShipmentLabel.Text = "";
                SumPaymentLabel.Text = "";
                ContractFilteComboBox.Visible = false;
            }
        }

        private void ContractFilteComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (FilterContractCheckBox.Checked)
            {
                ClientPayments.UpdateClientsPaymentsDataGrid(ContractFilteComboBox.SelectedValue.ToString());
                UpdateLabelPayments();
            }
            else
            {
                ClientPayments.GetAllPeriodPayments(ClientCheckBox.Checked, ClientComboBox.SelectedValue.ToString());
                NameLabel.Text = "";
                TotalLabel.Text = "";
                SumShipmentLabel.Text = "";
                SumPaymentLabel.Text = "";
            }
        }

        private void ToExcelButton_Click(object sender, EventArgs e)
        {
            if (ClientsPaymentsDataGrid.ColumnCount != 0)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated)

                    ClientPayments.ExportToExcel();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void AddDocumentsButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog1.FileName);

            if (fileInfo.Length > 20971520)
            {
                MessageBox.Show("Файл больше 20 МБ и не может быть загружен");
                return;
            }

            DataRow NewRow = AttachmentDocumentsDataTable.NewRow();
            NewRow["FileName"] = openFileDialog1.SafeFileName;
            NewRow["Path"] = openFileDialog1.FileName;
            AttachmentDocumentsDataTable.Rows.Add(NewRow);

            if (AttachmentDocumentsDataTable.Rows.Count != 0)
            {
                if (ClientContractDataGrid.SelectedRows.Count == 1)
                    ClientPayments.AttachDocuments(AttachmentDocumentsDataTable, ClientContractDataGrid.SelectedRows[0].Cells["ContractId"].Value.ToString());
            }
        }

        private void RemoveDocumentsButton_Click(object sender, EventArgs e)
        {
            if (AttachmentDocumentsBindingSource.Count == 0)
                return;

            AttachmentDocumentsBindingSource.RemoveCurrent();
            AttachmentDocumentsDataTable.AcceptChanges();

            ClientPayments.RemoveAttachmentsDocuments(ClientContractDataGrid.SelectedRows[0].Cells["ContractID"].Value.ToString());

        }

        System.Threading.Thread T;

        private void AttachmentsDocumentGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string temppath = "";

            {
                T = new System.Threading.Thread(delegate ()
                { temppath = ClientPayments.SaveFile(AttachmentsDocumentGrid.SelectedRows[0].Cells["FileName"].Value.ToString()); });
                T.Start();

                while (T.IsAlive)
                {
                    T.Join(50);
                    Application.DoEvents();

                    if (bStopTransfer)
                    {
                        FM.bStopTransfer = true;
                        bStopTransfer = false;
                        timer1.Enabled = false;
                        return;
                    }
                }

                if (!bStopTransfer && temppath != null)
                    System.Diagnostics.Process.Start(temppath);
            }
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
    }
}
