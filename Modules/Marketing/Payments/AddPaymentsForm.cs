using Infinium.Modules.Marketing.Payments;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddPaymentsForm : Form
    {
        public bool Ok = false;
        bool Add;
        int FormEvent = 0;
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        Form TopForm = null;

        string ClientID;
        string TypePayments, DocNumber, Cost, CurrencyTypeID, ClientPaymentsID, FirmID;
        DateTime dataTime;

        DataTable TableCurrency, TableClients, TableContract, TableFactory;
        BindingSource tClientContractBindingSource;

        ClientPayments ClientPayments;

        public AddPaymentsForm(string tClientID, ref ClientPayments tClientPayments, ref DataTable tTableCurrency, ref DataTable tTableClients, ref Form tTopForm, ref DataTable tTableContract, ref DataTable tTableFactory)
        {
            InitializeComponent();
            Add = true;
            TopForm = tTopForm;

            TableClients = tTableClients;
            TableCurrency = tTableCurrency;
            ClientPayments = tClientPayments;
            TableFactory = tTableFactory;
            ClientID = tClientID;
            TableContract = tTableContract;

            Initialize();
            ClientComboBox.SelectedValue = ClientID;
            ClientComboBox_SelectedValueChanged(null, null);
        }

        public AddPaymentsForm(string tClientID, ref ClientPayments tClientPayments, ref DataTable tTableCurrency, ref DataTable tTableClients, ref Form tTopForm, string tTypePayments, string tDocNumber, string tCost, string tCurrencyTypeID, DateTime tdataTime, string tClientPaymentsID, ref DataTable tTableFactory, string tFirmID)
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

            TypePayments = tTypePayments;
            DocNumber = tDocNumber;
            Cost = tCost;
            CurrencyTypeID = tCurrencyTypeID;
            dataTime = tdataTime;
            ClientPaymentsID = tClientPaymentsID;

            Initialize();

            ClientComboBox.SelectedValue = ClientID;

            if (TypePayments == "Оплачено")
                DebitRadioButton.Checked = true;
            else
                CreditRadioButton.Checked = true;

            DocNumberTextBox.Text = DocNumber;
            DateFromPicker.Value = dataTime;
            CostTextBox.Text = Cost;
            CurrencyComboBox.SelectedValue = CurrencyTypeID;
            FirmComboBox.SelectedValue = FirmID;
            ClientComboBox_SelectedValueChanged(null, null);
        }

        private void Initialize()
        {
            tClientContractBindingSource = new BindingSource();
            ClientComboBox.DataSource = TableClients;
            ClientComboBox.DisplayMember = "ClientName";
            ClientComboBox.ValueMember = "ClientID";

            CurrencyComboBox.DataSource = TableCurrency;
            CurrencyComboBox.DisplayMember = "CurrencyType";
            CurrencyComboBox.ValueMember = "CurrencyTypeID";

            //tClientContractBindingSource = ClientPayments.ClientContractBindingSource.Filter = "ClientID = " + ClientComboBox.SelectedValue.ToString();
            ContractComboBox.DataSource = ClientPayments.Filter(ClientComboBox.SelectedValue.ToString());
            ContractComboBox.DisplayMember = "ContractNumber";
            ContractComboBox.ValueMember = "ContractId";

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

                    ClientPayments.AddPayments(ClientComboBox.SelectedValue.ToString(), CreditRadioButton.Checked, DocNumberTextBox.Text, DateFromPicker.Value, CostTextBox.Text, CurrencyComboBox.SelectedValue.ToString(), ContractComboBox.SelectedValue.ToString(), FirmComboBox.SelectedValue.ToString());

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

                    ClientPayments.UpdatePayments(ClientPaymentsID, CreditRadioButton.Checked, DocNumberTextBox.Text, DateFromPicker.Value, CostTextBox.Text, CurrencyComboBox.SelectedValue.ToString(), FirmComboBox.SelectedValue.ToString(), ContractComboBox.SelectedValue.ToString());

                    DocNumberTextBox.Clear();
                    CostTextBox.Clear();
                }
            }

            this.Close();
            ClientPayments.UpdateClientsPaymentsDataGrid();
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

        private void CreditRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CreditRadioButton.Checked)
            {
                label3.Text = "Укажите дату отгрузки:";
                label4.Text = "Сумма отгрузки:";
            }
            else
            {
                label3.Text = "Укажите дату оплаты:";
                label4.Text = "Сумма оплаты:";
            }
        }

        private void ClientComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ContractComboBox.DataSource = ClientPayments.Filter(((DataRowView)ClientComboBox.SelectedItem).Row["ClientID"].ToString());
        }
    }
}
