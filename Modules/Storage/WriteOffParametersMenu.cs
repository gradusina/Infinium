using Infinium.Store;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class WriteOffParametersMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form MainForm = null;
        WriteOffParameters Parameters;

        public WriteOffParametersMenu(Form tMainForm, ref WriteOffParameters tParameters)
        {
            MainForm = tMainForm;
            Parameters = tParameters;
            InitializeComponent();
        }

        private void OKReportButton_Click(object sender, EventArgs e)
        {
            Parameters.WriteOffDate = dtpWriteOff.Value;
            Parameters.User1 = txtUser1.Text;
            Parameters.User2 = txtUser2.Text;
            Parameters.User3 = txtUser3.Text;
            Parameters.User4 = txtUser4.Text;
            Parameters.User5 = txtUser5.Text;
            Parameters.OKPress = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelReportButton_Click(object sender, EventArgs e)
        {
            Parameters.OKPress = false;
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

        }

        private void WriteOffParametersMenu_Load(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}
