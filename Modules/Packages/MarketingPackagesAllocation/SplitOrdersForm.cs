using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SplitOrdersForm : Form
    {
        Infinium.Modules.Packages.SplitOrders SplitStruct;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form TopForm = null;
        Form MainForm = null;

        public SplitOrdersForm(Form tMainForm, ref Infinium.Modules.Packages.SplitOrders tSplitStruct)
        {
            InitializeComponent();
            MainForm = tMainForm;
            SplitStruct = tSplitStruct;
            TopForm = this;
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

        private void SplitPackagesForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            EqualCountTextBox.Focus();
        }

        private void MarketingSplitPackagesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (NotEqualOrdersButton.Checked)
            {
                if (!Int32.TryParse(ProductCountTextBox.Text, out SplitStruct.EqualCount) || SplitStruct.TotalCount <= SplitStruct.EqualCount)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Введено неверное значение", "Ошибка");

                    ProductCountTextBox.Clear();
                    ProductCountTextBox.Focus();
                    return;
                }

                SplitStruct.LastCount = SplitStruct.TotalCount - SplitStruct.EqualCount;

                SplitStruct.OrdersCount = 1;
                SplitStruct.IsEqual = false;
            }

            if (EqualOrdersButton.Checked)
            {
                if (!Int32.TryParse(EqualCountTextBox.Text, out SplitStruct.EqualCount) || SplitStruct.TotalCount <= SplitStruct.EqualCount)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Введено неверное значение", "Ошибка");

                    EqualCountTextBox.Clear();
                    EqualCountTextBox.Focus();
                    return;
                }

                //decimal OrdersCount = Math.Floor(Convert.ToDecimal(SplitStruct.TotalCount) / Convert.ToDecimal(SplitStruct.EqualCount));

                SplitStruct.OrdersCount = SplitStruct.TotalCount / SplitStruct.EqualCount;
                SplitStruct.LastCount = SplitStruct.TotalCount - SplitStruct.OrdersCount * SplitStruct.EqualCount;

                SplitStruct.IsEqual = true;
            }

            SplitStruct.IsSplit = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelPackButton_Click(object sender, EventArgs e)
        {
            SplitStruct.IsSplit = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void EqualOrdersButton_Click(object sender, EventArgs e)
        {
            EqualPanel.BringToFront();
            EqualCountTextBox.Focus();
        }

        private void NotEqualOrdersButton_Click(object sender, EventArgs e)
        {
            NotEqualPanel.BringToFront();
            ProductCountTextBox.Focus();
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
