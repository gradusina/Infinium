using Infinium.Modules.Permits;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewVisitorPermitForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        NewPermit NewPermit;

        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        public NewVisitorPermitForm(Form tMainForm, VisitorsPermits tPermitsManager, ref NewPermit tNewPermit)
        {
            MainForm = tMainForm;
            NewPermit = tNewPermit;
            InitializeComponent();
            Initialize();
            cmbAddresseFromInfinium.DataSource = tPermitsManager.BsAddressees;
            cmbAddresseFromInfinium.DisplayMember = "ShortName";
            cmbAddresseFromInfinium.ValueMember = "UserID";
        }

        private void btnCreatePermit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbVisitorName.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Введите имя посетителя",
                    "Предупреждение");
                tbVisitorName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tbVisitMission.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Введите цель посещения",
                    "Предупреждение");
                tbVisitMission.Focus();
                return;
            }

            if (cbAddresseeFromInfinium.Checked)
            {
                if (string.IsNullOrEmpty(cmbAddresseFromInfinium.Text))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Введите адресата",
                        "Предупреждение");
                    cmbAddresseFromInfinium.Focus();
                    return;
                }
            }

            if (cbAddresseeOutInfinium.Checked)
            {
                if (string.IsNullOrEmpty(tbAddresseeOutInfinium.Text))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Введите адресата",
                        "Предупреждение");
                    tbAddresseeOutInfinium.Focus();
                    return;
                }
            }

            NewPermit.VisitorName = tbVisitorName.Text;
            NewPermit.VisitMission = tbVisitMission.Text;
            NewPermit.AddresseeID = 0;
            NewPermit.AddresseeName = string.Empty;
            if (cbAddresseeFromInfinium.Checked)
            {
                NewPermit.AddresseeID = Convert.ToInt32(cmbAddresseFromInfinium.SelectedValue);
                NewPermit.AddresseeName = cmbAddresseFromInfinium.Text;
            }
            if (cbAddresseeOutInfinium.Checked)
            {
                NewPermit.AddresseeName = tbAddresseeOutInfinium.Text;
            }
            NewPermit.Validity = dtpValidity.Value;
            NewPermit.bCreatePermit = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelCreatePermit_Click(object sender, EventArgs e)
        {
            NewPermit.bCreatePermit = false;
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void Initialize()
        {
        }

        private void cbAddresseeFromInfinium_CheckedChanged(object sender, EventArgs e)
        {
            cmbAddresseFromInfinium.Enabled = cbAddresseeFromInfinium.Checked;
            if (cbAddresseeOutInfinium.Checked)
                cbAddresseeOutInfinium.Checked = !cbAddresseeFromInfinium.Checked;
        }

        private void cbAddresseeOutInfinium_CheckedChanged(object sender, EventArgs e)
        {
            tbAddresseeOutInfinium.Enabled = cbAddresseeOutInfinium.Checked;
            if (cbAddresseeFromInfinium.Checked)
                cbAddresseeFromInfinium.Checked = !cbAddresseeOutInfinium.Checked;
        }
    }
}
