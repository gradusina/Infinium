using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SetOrderStatusForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public int AgreementStatusID = 1;
        int FormEvent = 0;

        Form MainForm = null;

        DataTable AgreementStatusesDT;

        public SetOrderStatusForm(Form tMainForm, int iClientID)
        {
            MainForm = tMainForm;
            AgreementStatusID = iClientID;
            InitializeComponent();
            Initialize();
        }

        private void NewOrderButton_Click(object sender, EventArgs e)
        {
            AgreementStatusID = Convert.ToInt32(cbOrderStatuses.SelectedValue);
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelOrderButton_Click(object sender, EventArgs e)
        {
            AgreementStatusID = -1;
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
            AgreementStatusesDT = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter(
                "SELECT * FROM AgreementStatuses WHERE AgreementStatusID<> 2",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(AgreementStatusesDT);
            }

            cbOrderStatuses.DataSource = new DataView(AgreementStatusesDT);
            cbOrderStatuses.DisplayMember = "AgreementStatus";
            cbOrderStatuses.ValueMember = "AgreementStatusID";

        }
    }
}
