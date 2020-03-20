using Infinium.Modules.Marketing.WeeklyPlanning;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingBatchSelectMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FactoryID = 1;
        int FormEvent = 0;

        Form MainForm = null;

        BatchManager BatchManager;

        public MarketingBatchSelectMenu(Form tMainForm,
            BatchManager tBatchManager, int iFactoryID)
        {
            MainForm = tMainForm;
            BatchManager = tBatchManager;
            FactoryID = iFactoryID;
            InitializeComponent();
        }

        private void OKMoveButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;

            if (kryptonCheckSet1.CheckedButton == NewBatchCheck)
            {
                BatchManager.NewBatch = true;
                BatchManager.OldBatch = false;
                BatchManager.CancelMovement = false;
            }

            if (kryptonCheckSet1.CheckedButton == OldBatchCheck)
            {
                BatchManager.NewBatch = false;
                BatchManager.OldBatch = true;
                BatchManager.CancelMovement = false;

            }
        }

        private void CancelMoveButton_Click(object sender, EventArgs e)
        {
            BatchManager.NewBatch = false;
            BatchManager.OldBatch = false;
            BatchManager.CancelMovement = true;

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
    }
}
