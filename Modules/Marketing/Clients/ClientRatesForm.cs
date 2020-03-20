using Infinium.Modules.Marketing.Clients;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ClientRatesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int ClientID = 0;
        int FormEvent = 0;

        Form MainForm = null;

        Clients Clients = null;

        public ClientRatesForm(Form tMainForm, Clients tClients, int iClientID)
        {
            MainForm = tMainForm;
            ClientID = iClientID;
            Clients = tClients;

            InitializeComponent();

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
            Clients.GetClientRates(ClientID);
            dgvClientRates.DataSource = Clients.ClientRatesBindingSource;

            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn DateTimeColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                CalendarTodayDate = DateTime.Now,
                Checked = false,
                DataPropertyName = "Date",
                HeaderText = "Дата",
                Name = "DateTimeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dgvClientRates.Columns.Add(DateTimeColumn);

            foreach (DataGridViewColumn Column in dgvClientRates.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgvClientRates.Columns["ClientRateID"].Visible = false;
            dgvClientRates.Columns["ClientID"].Visible = false;
            dgvClientRates.Columns["Date"].Visible = false;

            dgvClientRates.Columns["DateTimeColumn"].HeaderText = "Дата";
            //dgvClientRates.Columns["USD"].HeaderText = "Ценовая\r\nгруппа";
            //dgvClientRates.Columns["RUB"].HeaderText = "Отсрочка";
            //dgvClientRates.Columns["BYN"].HeaderText = "ID";

            dgvClientRates.AutoGenerateColumns = false;

            dgvClientRates.Columns["DateTimeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvClientRates.Columns["USD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientRates.Columns["USD"].Width = 120;
            dgvClientRates.Columns["RUB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientRates.Columns["RUB"].Width = 120;
            dgvClientRates.Columns["BYN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientRates.Columns["BYN"].Width = 120;

            int DisplayIndex = 0;
            dgvClientRates.Columns["DateTimeColumn"].DisplayIndex = DisplayIndex++;
            dgvClientRates.Columns["USD"].DisplayIndex = DisplayIndex++;
            dgvClientRates.Columns["RUB"].DisplayIndex = DisplayIndex++;
            dgvClientRates.Columns["BYN"].DisplayIndex = DisplayIndex++;
        }

        private void ClientsCancelButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ClientsSaveButton_Click(object sender, EventArgs e)
        {
            Clients.SaveClientRates();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ClientRatesForm_Load(object sender, EventArgs e)
        {
        }

        private void btnRemoveRate_Click(object sender, EventArgs e)
        {
            if (Clients.ClientRatesBindingSource.Count > 0)
                Clients.ClientRatesBindingSource.RemoveCurrent();
        }

        private void dgvClientRates_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["ClientID"].Value = ClientID;
        }
    }
}
