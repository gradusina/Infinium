using Infinium.Modules.ZOV.Clients;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewZOVClientForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int ClientID = 0;
        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        Clients Clients = null;

        public NewZOVClientForm(Form tMainForm, Clients tClients, int iClientID, bool bPriceGroup)
        {
            MainForm = tMainForm;
            Clients = tClients;

            InitializeComponent();
            ClientID = iClientID;

            if (Clients.NewClient)
            {
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

            cbClientsGroups.DataSource = Clients.ClientsGroupsBindingSource;
            cbClientsGroups.DisplayMember = "ClientGroupName";
            cbClientsGroups.ValueMember = "ClientGroupID";

            cbManager.DataSource = Clients.ManagersBindingSource;
            cbManager.DisplayMember = "Name";
            cbManager.ValueMember = "ManagerID";
        }

        public void EditClient()
        {
            string ClientName = string.Empty;
            int ClientGroupID = 0;
            int ManagerID = 0;

            Clients.CreateNewContactsDataTable(ref ClientsContactsDataGrid, ref ShopsAddressesDataGrid);

            Clients.EditClient(ref ClientName, ref ClientGroupID, ref ManagerID);

            ClientNameTextBox.Text = ClientName;
            cbClientsGroups.SelectedValue = ClientGroupID;
            cbManager.SelectedValue = ManagerID;

            Clients.NewClient = false;
        }

        private void ClientsCancelButton_Click(object sender, EventArgs e)
        {
            Clients.NewClient = false;

            ClientNameTextBox.Text = string.Empty;

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

            string Name = ClientNameTextBox.Text;
            int ClientGroupID = Convert.ToInt32(cbClientsGroups.SelectedValue);
            int ManagerID = Convert.ToInt32(cbManager.SelectedValue);

            if (Clients.NewClient == true)
            {
                Clients.AddClient(Name, ClientGroupID, ManagerID);
                Clients.SaveShopAddresses();
            }
            else
            {
                Clients.SaveClient(Name, ClientGroupID, ManagerID, ClientID);
                Clients.SaveShopAddresses();
            }

            ClientNameTextBox.Text = string.Empty;

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

        private void ShopsAddressesDataGrid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["ClientID"].Value = ClientID;
        }

        private void RemoveShopAddressButton_Click(object sender, EventArgs e)
        {
            if (Clients.NewShopAddressesBindingSource.Count > 0)
                Clients.NewShopAddressesBindingSource.RemoveCurrent();
        }
    }
}
