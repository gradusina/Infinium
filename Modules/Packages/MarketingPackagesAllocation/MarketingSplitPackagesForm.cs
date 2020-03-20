using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingSplitPackagesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        Infinium.Modules.Packages.Marketing.SplitStruct SS;

        int FormEvent = 0;
        int TotalCount = 0;
        Form TopForm = null;
        Form MainForm = null;

        public MarketingSplitPackagesForm()
        {
            InitializeComponent();
            EqualPackagesButton.Checked = true;

            while (!SplashForm.bCreated) ;
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

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (NotEqualPackagesButton.Checked)
            {
                if (string.IsNullOrWhiteSpace(SecondPackageCountTextBox.Text))
                    return;
                SS.FCount = TotalCount - Convert.ToInt32(SecondPackageCountTextBox.Text);
                SS.SCount = Convert.ToInt32(SecondPackageCountTextBox.Text);

                if (SS.SCount >= TotalCount)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Неверное значение", "Ошибка");
                    return;
                }

                SS.IsSplit = true;
                SS.IsEqual = false;
            }

            if (EqualPackagesButton.Checked)
            {
                if (string.IsNullOrWhiteSpace(PackCountTextBox.Text))
                    return;
                SS.FCount = Convert.ToInt32(PackCountTextBox.Text);

                decimal PackagesCount = Math.Floor(Convert.ToDecimal(TotalCount) / Convert.ToDecimal(SS.FCount));

                if (string.IsNullOrWhiteSpace(FirstPositionTextBox.Text))
                    return;

                if (PackagesCount < 1)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Неверное значение", "Ошибка");
                    return;
                }

                SS.PackagesCount = Convert.ToInt32(PackagesCount);
                SS.SCount = TotalCount - (SS.FCount * SS.PackagesCount);
                SS.FirstPosition = Convert.ToInt32(FirstPositionTextBox.Text);
                SS.IsSplit = true;
                SS.IsEqual = true;
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelPackButton_Click(object sender, EventArgs e)
        {
            SS.IsSplit = false;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        public void SetSplitPositions(int Count, ref Infinium.Modules.Packages.Marketing.SplitStruct tSS)
        {
            TotalCount = Count;
            SS = tSS;
        }

        private void EqualPackagesButton_Click(object sender, EventArgs e)
        {
            EqualPanel.BringToFront();
            PackCountTextBox.Focus();
        }

        private void NotEqualPackagesButton_Click(object sender, EventArgs e)
        {
            NotEqualPanel.BringToFront();
            SecondPackageCountTextBox.Focus();
        }

        private void SplitPackagesForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            PackCountTextBox.Focus();

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MarketingSplitPackagesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
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
    }
}
