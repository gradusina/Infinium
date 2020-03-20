using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class WebServiceForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        AdminWebServiceManager AdminWebServiceManager;

        public WebServiceForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }


        private void AdminJournalDetailForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }


        private void Initialize()
        {
            AdminWebServiceManager = new Infinium.AdminWebServiceManager();

            string sStartDate = "";
            string sEndDate = "";
            string LastCycle = "";

            if (AdminWebServiceManager.GetOnlineControlStatus(ref sStartDate, ref sEndDate, ref LastCycle))
                label3.Text = "Работает";
            else
                label3.Text = "Остановлен";

            label4.Text = sStartDate;
            label6.Text = LastCycle;
            label8.Text = sEndDate;
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

        private void OnlineTimer_Tick(object sender, EventArgs e)
        {
            //string sStartDate = "";
            //string sEndDate = "";

            //if (AdminWebServiceManager.GetOnlineControlStatus(ref sStartDate, ref sEndDate))
            //    label2.Text = "Онлайн-контроль запущен " + sStartDate;
            //else
            //    label2.Text = "Онлайн-контроль остановлен " + sEndDate;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminWebServiceManager.StartOnlineControl();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminWebServiceManager.StopOnlineControl();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sStartDate = "";
            string sEndDate = "";
            string LastCycle = "";

            if (AdminWebServiceManager.GetOnlineControlStatus(ref sStartDate, ref sEndDate, ref LastCycle))
                label3.Text = "Работает";
            else
                label3.Text = "Остановлен";

            label4.Text = sStartDate;
            label6.Text = LastCycle;
            label8.Text = sEndDate;

        }
    }
}
