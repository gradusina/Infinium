using Infinium.Modules.Marketing.Clients;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewMarketingClientForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int ClientID = 0;
        int FormEvent = 0;
        int OldManagerID = -1;

        Form MainForm = null;
        Form TopForm = null;

        Clients Clients = null;

        public NewMarketingClientForm(Form tMainForm, Clients tClients, int iClientID, bool bPriceGroup)
        {
            MainForm = tMainForm;
            Clients = tClients;

            InitializeComponent();
            ClientID = iClientID;

            if (Clients.NewClient)
            {
                tbPriceGroup.Text = "0";
                label2.Text = "Новый клиент";
                ClientNameTextBox.Enabled = true;
            }
            else
            {
                label2.Text = "Редактирование клиента";
                ClientNameTextBox.Enabled = false;
                if (bPriceGroup)
                    ClientNameTextBox.Enabled = true;
            }

            NonStandardComboBox.SelectedIndex = 0;

            if (!bPriceGroup)
            {
                tbPriceGroup.Enabled = false;
                btnEditClientRates.Enabled = false;
                cbClientEnable.Enabled = false;
                tbDelayOfPayment.Enabled = false;
            }

            Initialize();
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
                        MainForm.Activate();
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
                        MainForm.Activate();
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

        private void Initialize()
        {
            if (Clients.NewClient == true)
                Clients.CreateNewContactsDataTable(ref ClientsContactsDataGrid, ref ShopsAddressesDataGrid);

            cbCountry.DataSource = Clients.CountriesBindingSource;
            cbCountry.DisplayMember = "Name";
            cbCountry.ValueMember = "CountryID";

            cbManager.DataSource = Clients.UsersBindingSource;
            cbManager.DisplayMember = "ShortName";
            cbManager.ValueMember = "ManagerID";

            cbClientGroups.DataSource = Clients.ClientGroupsBindingSource;
            cbClientGroups.DisplayMember = "ClientGroupName";
            cbClientGroups.ValueMember = "ClientGroupID";

            if (Clients.NewClient == false)
            {
                NonStandardComboBox.SelectedIndex = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["NonStandard"]);
            }
        }

        public void EditClient()
        {
            string ClientName = string.Empty;
            int CountryID = 0;
            int ClientGroupID = 0;
            string City = string.Empty;
            string Email = string.Empty;
            string Site = string.Empty;
            string UNN = string.Empty;
            int ManagerID = 0;
            decimal PriceGroup = 0;
            int DelayOfPayment = 0;
            bool Enabled = false;

            Clients.CreateNewContactsDataTable(ref ClientsContactsDataGrid, ref ShopsAddressesDataGrid);

            Clients.EditClient(ref ClientName, ref CountryID, ref City, ref ClientGroupID, ref Email, ref Site, ref UNN, ref ManagerID, ref PriceGroup, ref DelayOfPayment, ref Enabled);

            OldManagerID = ManagerID;

            ClientNameTextBox.Text = ClientName;
            cbCountry.SelectedValue = CountryID;
            cbClientGroups.SelectedValue = ClientGroupID;
            CityTextBox.Text = City;
            EmailTextBox.Text = Email;
            SiteTextBox.Text = Site;
            tbUNN.Text = UNN;
            cbManager.SelectedValue = ManagerID;
            tbPriceGroup.Text = PriceGroup.ToString();
            tbDelayOfPayment.Text = DelayOfPayment.ToString();
            cbClientEnable.Checked = Enabled;

            Clients.NewClient = false;
        }

        private void ClientsCancelButton_Click(object sender, EventArgs e)
        {
            Clients.NewClient = false;

            ClientNameTextBox.Text = string.Empty;
            CityTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            SiteTextBox.Text = string.Empty;
            tbUNN.Text = string.Empty;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ClientsSaveButton_Click(object sender, EventArgs e)
        {
            if (ClientNameTextBox.Text.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Введены не все данные: Клиент!",
                    "Сохранение клиента");
                return;
            }
            if (CityTextBox.Text.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Введены не все данные Город!",
                    "Сохранение клиента");
                return;
            }

            string UNN = tbUNN.Text;
            string Name = ClientNameTextBox.Text;
            int CountryID = Convert.ToInt32(cbCountry.SelectedValue);
            int ClientGroupID = Convert.ToInt32(cbClientGroups.SelectedValue);
            string City = CityTextBox.Text;
            string Site = SiteTextBox.Text;
            string Email = EmailTextBox.Text;
            int ManagerID = Convert.ToInt32(cbManager.SelectedValue);
            int NonStandard = Convert.ToInt32(NonStandardComboBox.SelectedIndex);
            decimal PriceGroup = Convert.ToDecimal(tbPriceGroup.Text);
            int DelayOfPayment = 0;
            bool Enabled = cbClientEnable.Checked;

            if (tbDelayOfPayment.Text.Length > 0)
                DelayOfPayment = Convert.ToInt32(tbDelayOfPayment.Text);
            if (Clients.NewClient == true)
            {
                InfiniumFiles InfiniumFiles = new InfiniumFiles();
                InfiniumFiles.CreateClientFolders(ClientNameTextBox.Text);

                Clients.AddClient(Name,
                    CountryID, City, ClientGroupID,
                    Site, Email, ManagerID, UNN,
                    NonStandard, PriceGroup, DelayOfPayment, Enabled);
                Clients.SaveShopAddresses();
            }
            else
            {
                Clients.SaveClient(Name,
                    CountryID, City, ClientGroupID,
                    Site, Email, ManagerID, UNN,
                    NonStandard, PriceGroup, DelayOfPayment, Enabled, ClientID);
                Clients.SaveShopAddresses();
            }

            if (OldManagerID != ManagerID && ManagerID != 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "У клиента поменялся менеджер. Уведомить клиента письмом на почту?",
                        "Уведомлению клиенту");
                if (OKCancel)
                {
                    string result = string.Empty;
                    result = Clients.NotifyClient(Email, ManagerID);
                    InfiniumTips.ShowTip(this, 50, 85, result, 2500);
                }
            }

            ClientNameTextBox.Text = string.Empty;
            CityTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            SiteTextBox.Text = string.Empty;
            tbUNN.Text = string.Empty;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NewMarketingClientForm_Load(object sender, EventArgs e)
        {
        }

        private void RemoveContactButton_Click(object sender, EventArgs e)
        {
            if (Clients.NewContactsBindingSource.Count > 0)
                Clients.NewContactsBindingSource.RemoveCurrent();
        }

        private void btnEditClientRates_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            ClientRatesForm ClientRatesForm = new ClientRatesForm(this, Clients, ClientID);

            TopForm = ClientRatesForm;
            ClientRatesForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            ClientRatesForm.Dispose();
            TopForm = null;
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (Clients.NewShopAddressesBindingSource.Count > 0)
                Clients.NewShopAddressesBindingSource.RemoveCurrent();
        }

        private void ShopsAddressesDataGrid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["ClientID"].Value = ClientID;
        }

        private void kryptonComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void btnEditCountries_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            CountriesForm CountriesForm = new CountriesForm(this, Clients);

            TopForm = CountriesForm;
            CountriesForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            CountriesForm.Dispose();
            TopForm = null;
        }
    }
}
