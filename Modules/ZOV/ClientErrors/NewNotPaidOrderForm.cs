using Infinium.Modules.ZOV.ClientErrors;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewNotPaidOrderForm : Form
    {
        public static bool IsOKPress = true;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form MainForm = null;
        NotPaidOrders NotPaidOrders;

        public NewNotPaidOrderForm(Form tMainForm, ref NotPaidOrders tNotPaidOrders)
        {
            InitializeComponent();

            NotPaidOrders = tNotPaidOrders;
            MainForm = tMainForm;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
            cbClients.DataSource = NotPaidOrders.ClientsList;
            cbClients.DisplayMember = "ClientName";
            cbClients.ValueMember = "ClientID";
            cbClients.SelectedIndex = 0;

            cbDocNumbers.DataSource = NotPaidOrders.DocNumbersList;
            cbDocNumbers.DisplayMember = "DocNumber";
            cbDocNumbers.ValueMember = "DocNumber";
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

        private void AddNewClientErrorForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void AddNewClientErrorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void cbxClients_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (NotPaidOrders.HasCurrentClient)
                NotPaidOrders.GetDocNumbers(Convert.ToInt32(cbClients.SelectedValue));
        }

        private void btnCancelAdd_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnOkAdd_Click(object sender, EventArgs e)
        {
            int ClientID = 0;
            int MainOrderID = 0;
            string DocNumber = string.Empty;
            string Notes = string.Empty;

            if (NotPaidOrders.HasCurrentClient)
                ClientID = Convert.ToInt32(cbClients.SelectedValue);
            if (NotPaidOrders.HasDocNumbers)
                MainOrderID = Convert.ToInt32(((DataRowView)cbDocNumbers.SelectedItem).Row["MainOrderID"]);
            decimal.TryParse(tbCost.Text, out decimal Cost);
            DocNumber = cbDocNumbers.Text;
            Notes = rtbNotes.Text;
            NotPaidOrders.AddNotPaidOrder(ClientID, MainOrderID, DocNumber, dtpDispatchDate.Value, Cost, Notes);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void AddNewClientErrorForm_Load(object sender, EventArgs e)
        {
            if (NotPaidOrders.HasCurrentClient)
                NotPaidOrders.GetDocNumbers(Convert.ToInt32(cbClients.SelectedValue));
        }

        private void cbxClients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                cbxClients_SelectionChangeCommitted(sender, e);
        }

    }
}
