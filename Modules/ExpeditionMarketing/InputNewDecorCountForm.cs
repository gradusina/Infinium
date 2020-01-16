using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class InputNewDecorCountForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public int Result = 1;
        public int NewCount = 0;

        int FormEvent = 0;

        Form MainForm = null;

        public InputNewDecorCountForm(Form tMainForm, int OldCount)
        {
            MainForm = tMainForm;
            InitializeComponent();
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

        private void btnSaveReport_Click(object sender, EventArgs e)
        {
            if (CountTextBox.Text.Length > 0)
            {
                bool b = Int32.TryParse(CountTextBox.Text, out NewCount);
                if (!b)
                    return;
            }
            else
                return;
            Result = 1;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelReport_Click(object sender, EventArgs e)
        {
            Result = 2;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CountTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ComponentFactory.Krypton.Toolkit.KryptonTextBox TxtBox = (ComponentFactory.Krypton.Toolkit.KryptonTextBox)sender;

            if (!Char.IsDigit(e.KeyChar))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }
    }
}
