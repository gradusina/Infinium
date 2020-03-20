using Infinium.Modules.WorkAssignments;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class TafelSplitMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;
        //int SourceCount = 0;
        //int FirstCount = 0;
        //int SecondCount = 0;
        //int PosCount = 0;

        Form MainForm = null;
        Form TopForm = null;

        TafelSplitStruct TafelSplitStruct;

        public TafelSplitMenu(Form tMainForm, ref TafelSplitStruct tTafelSplitStruct)
        {
            MainForm = tMainForm;
            TafelSplitStruct = tTafelSplitStruct;
            InitializeComponent();
            Initialize();
        }

        private void btnPressOK_Click(object sender, EventArgs e)
        {
            if (TafelSplitStruct.bEqual)
            {
                if (string.IsNullOrWhiteSpace(tbPosCount.Text))
                    return;
                bool OkTryParse = int.TryParse(tbPosCount.Text, out TafelSplitStruct.FirstCount);
                if (!OkTryParse)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Неверное значение",
                        "Ошибка");
                    return;
                }

                decimal d = Math.Floor(Convert.ToDecimal(TafelSplitStruct.SourceCount) / Convert.ToDecimal(TafelSplitStruct.FirstCount));
                if (d < 1)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Неверное значение",
                        "Ошибка");
                    return;
                }
                TafelSplitStruct.PosCount = Convert.ToInt32(d);
                TafelSplitStruct.SecondCount = TafelSplitStruct.SourceCount - (TafelSplitStruct.FirstCount * TafelSplitStruct.PosCount);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(tbFirstCount.Text))
                    return;
                bool OkTryParse = int.TryParse(tbFirstCount.Text, out TafelSplitStruct.FirstCount);
                if (!OkTryParse)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Неверное значение",
                        "Ошибка");
                    return;
                }

                decimal d = Math.Floor(Convert.ToDecimal(TafelSplitStruct.SourceCount) / Convert.ToDecimal(TafelSplitStruct.FirstCount));
                if (d < 1)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Неверное значение",
                        "Ошибка");
                    return;
                }
                TafelSplitStruct.PosCount = Convert.ToInt32(d);
                TafelSplitStruct.SecondCount = TafelSplitStruct.SourceCount - (TafelSplitStruct.FirstCount * TafelSplitStruct.PosCount);
            }

            TafelSplitStruct.bOk = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnPressCancel_Click(object sender, EventArgs e)
        {
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

        private void TafelSplitMenu_Load(object sender, EventArgs e)
        {
            //if (TafelSplitStruct.bEqual)
            //{
            //    //panel2.BringToFront();
            //    tbPosCount.Focus();
            //}
            //else
            //{
            //    //panel3.BringToFront();
            //    tbFirstCount.Focus();
            //}
        }

        private void TafelSplitMenu_Shown(object sender, EventArgs e)
        {
            if (TafelSplitStruct.bEqual)
            {
                panel2.BringToFront();
                tbPosCount.Focus();
            }
            else
            {
                panel3.BringToFront();
                tbFirstCount.Focus();
            }
        }
    }
}
