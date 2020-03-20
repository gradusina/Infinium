using Infinium.Modules.ZOV.ClientErrors;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewClientErrorForm : Form
    {
        public static bool IsOKPress = true;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form MainForm = null;
        ClientErrorsWriteOffs ClientErrorsWriteOffs;

        public NewClientErrorForm(Form tMainForm, ref ClientErrorsWriteOffs tClientErrorsWriteOffs)
        {
            InitializeComponent();

            ClientErrorsWriteOffs = tClientErrorsWriteOffs;
            MainForm = tMainForm;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
            cbxClients.DataSource = ClientErrorsWriteOffs.ClientsList;
            cbxClients.DisplayMember = "ClientName";
            cbxClients.ValueMember = "ClientID";
            cbxClients.SelectedIndex = 0;

            cbxDocNumbers.DataSource = ClientErrorsWriteOffs.DocNumbersList;
            cbxDocNumbers.DisplayMember = "DocNumber";
            cbxDocNumbers.ValueMember = "MainOrderID";
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
            if (ClientErrorsWriteOffs.HasCurrentClient)
                ClientErrorsWriteOffs.GetDocNumbers(Convert.ToInt32(cbxClients.SelectedValue));
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
            string Product = string.Empty;
            string Reason = string.Empty;

            if (ClientErrorsWriteOffs.HasCurrentClient)
                ClientID = Convert.ToInt32(cbxClients.SelectedValue);
            if (ClientErrorsWriteOffs.HasCurrentDocNumber)
                MainOrderID = Convert.ToInt32(cbxDocNumbers.SelectedValue);
            decimal.TryParse(tbxCost.Text, out decimal Cost);
            DocNumber = cbxDocNumbers.Text;
            Product = tbProduct.Text;
            Reason = rtbxReason.Text;
            ClientErrorsWriteOffs.AddClientError(ClientID, MainOrderID, DocNumber, Product, dtpOrderDate.Value, Cost, Reason);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void AddNewClientErrorForm_Load(object sender, EventArgs e)
        {
            if (ClientErrorsWriteOffs.HasCurrentClient)
                ClientErrorsWriteOffs.GetDocNumbers(Convert.ToInt32(cbxClients.SelectedValue));
        }

        private void cbxClients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                cbxClients_SelectionChangeCommitted(sender, e);
        }

    }
}
