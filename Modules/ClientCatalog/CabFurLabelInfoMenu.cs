using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CabFurLabelInfoMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedLength = false;
        bool NeedHeight = false;
        bool NeedWidth = false;
        public bool PressOK = false;
        public int LabelsCount = 1;
        //public int LabelsLength = 0;
        //public int LabelsHeight = 0;
        //public int LabelsWidth = 0;
        public string DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
        public int PositionsCount = 1;
        int FormEvent = 0;

        Form MainForm = null;

        public CabFurLabelInfoMenu(Form tMainForm, bool bNeedLength, bool bNeedHeight, bool bNeedWidth)
        {
            MainForm = tMainForm;
            NeedLength = bNeedLength;
            NeedHeight = bNeedHeight;
            NeedWidth = bNeedWidth;
            InitializeComponent();
            if (!bNeedLength)
                tbLength.Enabled = false;
            if (!bNeedHeight)
                tbHeight.Enabled = false;
            if (!bNeedWidth)
                tbWidth.Enabled = false;
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
            //if (NeedLength)
            //    int.TryParse(tbLength.Text, out LabelsLength);
            //if (NeedHeight)
            //    int.TryParse(tbHeight.Text, out LabelsHeight);
            //if (NeedWidth)
            //    int.TryParse(tbWidth.Text, out LabelsWidth);
            int.TryParse(tbLabelsCount.Text, out LabelsCount);
            int.TryParse(tbPositionsCount.Text, out PositionsCount);
            DocDateTime = kryptonDateTimePicker1.Value.ToString("dd.MM.yyyy");
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
