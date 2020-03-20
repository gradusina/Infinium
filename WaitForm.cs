using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class WaitForm : Form
    {
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Thread Thread = null;


        public static bool CloseS = false;

        public void CloseSplash()
        {
            FormEvent = 3;
            AnimateTimer.Enabled = true;
        }

        public WaitForm(Color BackColor, string LabelText, ref Thread tThread)
        {
            Thread = tThread;

            InitializeComponent();
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            this.BackColor = BackColor;

            label1.Text = LabelText;
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (FormEvent == eClose)
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
                }

                return;
            }


            if (FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                }

                return;
            }
        }

        private void ThreadLifeTimer_Tick(object sender, EventArgs e)
        {
            if (CloseS == true || Thread.IsAlive == false)
            {
                CloseSplash();
                CloseS = false;
                ThreadLifeTimer.Enabled = false;
            }
        }

    }
}
