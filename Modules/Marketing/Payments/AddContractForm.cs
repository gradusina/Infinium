using Infinium.Modules.Marketing.Payments;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddContractForm : Form
    {
        public bool Ok = false;
        bool Add;
        int FormEvent = 0;
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        Form TopForm = null;

        string ClientID;
        string DocNumber, Cost, CurrencyTypeID, ContractId, FirmID;
        DateTime EndDateContract, StartDateContract;

        DataTable TableCurrency, TableClients, TableFactory;

        ClientPayments ClientPayments;

        public AddContractForm(string tClientID, ref ClientPayments tClientPayments, ref DataTable tTableCurrency, ref DataTable tTableClients, ref Form tTopForm, ref DataTable tTableFactory)
        {
            InitializeComponent();
            Add = true;
            TopForm = tTopForm;

            TableClients = tTableClients;
            TableCurrency = tTableCurrency;
            ClientPayments = tClientPayments;
            TableFactory = tTableFactory;
            ClientID = tClientID;

            Initialize();
            ClientComboBox.SelectedValue = ClientID;
        }

        public AddContractForm(string tClientID, ref ClientPayments tClientPayments, ref DataTable tTableCurrency, ref DataTable tTableClients, ref Form tTopForm, string tDocNumber, string tCost, string tCurrencyTypeID, DateTime tStartDateContract, DateTime tEndDateContract, string tContractId, ref DataTable tTableFactory, string tFirmID)
        {
            InitializeComponent();
            Add = false;
            TopForm = tTopForm;

            TableClients = tTableClients;
            TableCurrency = tTableCurrency;
            ClientPayments = tClientPayments;
            TableFactory = tTableFactory;
            ClientID = tClientID;
            FirmID = tFirmID;

            DocNumber = tDocNumber;
            Cost = tCost;
            CurrencyTypeID = tCurrencyTypeID;
            StartDateContract = tStartDateContract;
            EndDateContract = tEndDateContract;
            ContractId = tContractId;

            Initialize();

            ClientComboBox.SelectedValue = ClientID;
            FirmComboBox.SelectedValue = FirmID;

            DocNumberTextBox.Text = DocNumber;
            DateFromPicker.Value = StartDateContract;
            DateToPicker.Value = EndDateContract;
            CostTextBox.Text = Cost;
            CurrencyComboBox.SelectedValue = CurrencyTypeID;
        }

        private void Initialize()
        {
            ClientComboBox.DataSource = TableClients;
            ClientComboBox.DisplayMember = "ClientName";
            ClientComboBox.ValueMember = "ClientID";

            CurrencyComboBox.DataSource = TableCurrency;
            CurrencyComboBox.DisplayMember = "CurrencyType";
            CurrencyComboBox.ValueMember = "CurrencyTypeID";

            FirmComboBox.DataSource = TableFactory;
            FirmComboBox.DisplayMember = "FactoryName";
            FirmComboBox.ValueMember = "FactoryID";
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (Add)
            {
                if (DocNumberTextBox.Text == "")
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Введите название документа!",
                        "Ошибка");
                else
                {
                    try
                    {
                        Convert.ToDecimal(CostTextBox.Text);
                    }
                    catch
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Некорректно задана цена!",
                           "Ошибка");
                        return;
                    }

                    ClientPayments.AddContracts(ClientComboBox.SelectedValue.ToString(), DocNumberTextBox.Text, DateFromPicker.Value, DateToPicker.Value, CostTextBox.Text, CurrencyComboBox.SelectedValue.ToString(), FirmComboBox.SelectedValue.ToString());

                    DocNumberTextBox.Clear();
                    CostTextBox.Clear();
                }
            }
            else
            {
                if (DocNumberTextBox.Text == "")
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Введите название документа!",
                        "Ошибка");
                else
                {
                    try
                    {
                        Convert.ToDecimal(CostTextBox.Text);
                    }
                    catch
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Некорректно задана цена!",
                           "Ошибка");
                        return;
                    }

                    ClientPayments.UpdateContracts(ContractId, DocNumberTextBox.Text, DateFromPicker.Value, DateToPicker.Value, CostTextBox.Text, CurrencyComboBox.SelectedValue.ToString(), FirmComboBox.SelectedValue.ToString());

                    DocNumberTextBox.Clear();
                    CostTextBox.Clear();
                }
            }

            this.Close();
            ClientPayments.UpdateClientsContractDataGrid();
        }

        private void CancelDateButton_Click(object sender, EventArgs e)
        {
            this.Close();
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
    }
}
