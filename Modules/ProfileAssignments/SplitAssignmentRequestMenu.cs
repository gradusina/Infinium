using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SplitAssignmentRequestMenu : Form
    {
        public bool OKSplit = true;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool OnlyCount = false;
        int FormEvent = 0;

        public decimal Diameter = 0;
        public decimal iWidth = 0;
        public int Count = 0;

        Form TopForm = null;

        public SplitAssignmentRequestMenu(bool bOnlyCount, int iCurrentCount)
        {
            InitializeComponent();
            OnlyCount = bOnlyCount;
            Count = iCurrentCount;

            Initialize();

            if (bOnlyCount)
            {
                this.ClientSize = new System.Drawing.Size(405, 137);

                tbCount.Text = Count.ToString();
                tbCount.Focus();
                tbCount.SelectAll();
            }
            else
            {
                this.ClientSize = new System.Drawing.Size(405, 237);
                label2.Visible = true;
                label3.Visible = true;
                tbDiameter.Visible = true;
                tbWidth.Visible = true;
            }

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
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

        private void AddInventoryRestForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void AddInventoryRestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void OKSplitButton_Click(object sender, EventArgs e)
        {
            if (!OnlyCount)
            {
                if (string.IsNullOrWhiteSpace(tbDiameter.Text))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Введите диаметр",
                        "Ошибка");
                    tbDiameter.Focus();
                    tbDiameter.SelectAll();
                    return;
                }
                if (string.IsNullOrWhiteSpace(tbWidth.Text))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Введите ширину",
                        "Ошибка");
                    tbWidth.Focus();
                    tbWidth.SelectAll();
                    return;
                }
                Diameter = Convert.ToDecimal(tbDiameter.Text);
                iWidth = Convert.ToInt32(tbWidth.Text);
            }
            if (string.IsNullOrWhiteSpace(tbCount.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Введите кол-во",
                    "Ошибка");
                tbCount.Focus();
                tbCount.SelectAll();
                return;
            }

            if (OnlyCount)
            {
                if (Count < Convert.ToInt32(tbCount.Text))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Недопустимое значение",
                        "Ошибка");
                    return;
                }
            }

            Count = Convert.ToInt32(tbCount.Text);

            OKSplit = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelSplitButton_Click(object sender, EventArgs e)
        {
            OKSplit = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void tbCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OKSplitButton_Click(null, null);
        }
    }
}
