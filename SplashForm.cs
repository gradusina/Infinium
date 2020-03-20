using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SplashForm : Form
    {
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        public static bool CloseS = false;

        public static bool bCreated = false;

        public void CloseSplash()
        {
            FormEvent = 3;
            AnimateTimer.Enabled = true;
        }

        public SplashForm()
        {
            bCreated = false;
            CloseS = false;
            InitializeComponent();
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            this.BackColor = BackColor;
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
                    bCreated = true;
                    AnimateTimer.Enabled = false;
                }

                return;
            }
        }

        private void ThreadLifeTimer_Tick(object sender, EventArgs e)
        {
            if (CloseS == true && bCreated == true)
            {
                CloseSplash();
                CloseS = false;
                ThreadLifeTimer.Enabled = false;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.preloaders.net");
            }
            catch
            { }
        }

    }
}
