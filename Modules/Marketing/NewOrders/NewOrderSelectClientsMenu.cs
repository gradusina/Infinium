using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewOrderSelectClientsMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public int ClientID = 1;
        public bool FromExcel = false;
        int FormEvent = 0;

        Form MainForm = null;

        DataTable ClientsDT;

        public NewOrderSelectClientsMenu(Form tMainForm, int iClientID)
        {
            MainForm = tMainForm;
            ClientID = iClientID;
            InitializeComponent();
            Initialize();
        }

        private void NewOrderButton_Click(object sender, EventArgs e)
        {
            ClientID = Convert.ToInt32(ClientsComboBox.SelectedValue);
            FromExcel = cbFromExcel.Checked;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelOrderButton_Click(object sender, EventArgs e)
        {
            ClientID = 0;
            FromExcel = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
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
            ClientsDT = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter(
                "SELECT * FROM Clients WHERE Enabled = 1 ORDER BY ClientName",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
            }

            ClientsComboBox.DataSource = new DataView(ClientsDT);
            ClientsComboBox.DisplayMember = "ClientName";
            ClientsComboBox.ValueMember = "ClientID";

            if (ClientID > 0)
                ClientsComboBox.SelectedValue = ClientID;

        }
    }
}
