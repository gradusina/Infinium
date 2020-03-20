using System;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CoverWaitForm : Form
    {
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        public static bool bIamCreated = false;

        public static bool CloseS = false;

        public void CloseSplash()
        {
            FormEvent = 3;
            AnimateTimer.Enabled = true;
        }

        public CoverWaitForm(bool bSmall)
        {
            InitializeComponent();

            if (bSmall)
                panel1.Visible = false;
            else
                pictureBox1.Visible = false;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            //this.BackColor = BackColor;
            StandardSpinner.BringToFront();
            label1.ForeColor = Color.Black;
            CloseS = false;
        }

        public CoverWaitForm(bool bSmall, Color backColor)
        {
            InitializeComponent();

            if (bSmall)
                panel1.Visible = false;
            else
                pictureBox1.Visible = false;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            this.BackColor = backColor;
            WhiteSpinner.BringToFront();
            label1.ForeColor = Color.White;
            CloseS = false;
        }


        private void Form2_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            //without animation
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                AnimateTimer.Enabled = false;
                SplashWindow.bSmallCreated = true;

                if (FormEvent == eClose)
                {

                    this.Close();
                    return;
                }
            }

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
                    SplashWindow.bSmallCreated = true;
                    AnimateTimer.Enabled = false;
                }

                return;
            }
        }

        private void ThreadLifeTimer_Tick(object sender, EventArgs e)
        {
            if (CloseS == true)
            {
                CloseSplash();
                CloseS = false;
                ThreadLifeTimer.Enabled = false;
            }
        }

    }
}
