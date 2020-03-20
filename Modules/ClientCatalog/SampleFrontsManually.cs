using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SampleFrontsManually : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public bool PressOK = false;
        public string Front = string.Empty;
        public string FrameColor = string.Empty;
        public string Patina = string.Empty;
        public string InsetType = string.Empty;
        public string InsetColor = string.Empty;
        public string TechnoInsetType = string.Empty;
        public string TechnoInsetColor = string.Empty;
        public string LabelsHeight = string.Empty;
        public string LabelsWidth = string.Empty;
        public string LabelsCount = "1";
        public string PositionsCount = "1";
        int FormEvent = 0;

        Form MainForm = null;

        public SampleFrontsManually(Form tMainForm)
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

        private void btnOKInput_Click(object sender, EventArgs e)
        {
            PressOK = true;
            Front = textBox1.Text;
            FrameColor = textBox2.Text;
            InsetType = textBox3.Text;
            InsetColor = textBox4.Text;
            TechnoInsetType = textBox5.Text;
            TechnoInsetColor = textBox6.Text;
            Patina = textBox7.Text;
            LabelsHeight = textBox8.Text;
            LabelsWidth = textBox9.Text;
            PositionsCount = textBox10.Text;
            LabelsCount = textBox11.Text;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelInput_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
