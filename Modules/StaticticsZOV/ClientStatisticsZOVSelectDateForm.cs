using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ClientStatisticsZOVSelectDateForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;


        ClientStatisticsZOVDetailForm ClientStatisticsZOVDetailForm;

        Modules.StaticticsZOV.ClientStatisticsZOV ClientStatisticsZOV;

        System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ru-RU");

        public ClientStatisticsZOVSelectDateForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void DispatchZOVDateForm_Shown(object sender, EventArgs e)
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

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
        }

        private void DispatchZOVDispatchesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            ClientStatisticsZOV = new Modules.StaticticsZOV.ClientStatisticsZOV(ref ClientsDataGrid);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            NoDispatchLabel.Text = ClientStatisticsZOV.CheckClientAndPeriod(
                Convert.ToInt32(((DataRowView)ClientStatisticsZOV.ClientsBindingSource.Current)["ClientID"]), CalendarFrom.SelectionEnd,
                                                                  CalendarTo.SelectionEnd);

            if (NoDispatchLabel.Text.Length > 0)
            {
                NoDispatchLabel.Visible = true;
                LabelTimer.Enabled = true;
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            if (ClientStatisticsZOVDetailForm == null)
                ClientStatisticsZOVDetailForm = new ClientStatisticsZOVDetailForm(this, ref ClientStatisticsZOV, CalendarFrom.SelectionEnd,
                                                                  CalendarTo.SelectionEnd);

            TopForm = ClientStatisticsZOVDetailForm;
            ClientStatisticsZOVDetailForm.ShowDialog();
            TopForm = null;

            if (ClientStatisticsZOVDetailForm.AccessibleName == "true")
            {
                ClientStatisticsZOVDetailForm.Dispose();
                ClientStatisticsZOVDetailForm = null;
                GC.Collect();
            }
        }

        private void PriorButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void LabelTimer_Tick(object sender, EventArgs e)
        {
            NoDispatchLabel.Visible = false;
            LabelTimer.Enabled = false;
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            DateTime DateFrom = DateTime.Now;
            DateTime DateTo = DateTime.Now;

            ClientStatisticsZOV.GetAllPeriod(ref DateFrom, ref DateTo);

            //Infragistics.Win.UltraWinSchedule.Day Day;

            //Day = ultraCalendarInfo1.GetDay(DateFrom, true);

            CalendarFrom.SelectionEnd = DateFrom;
            //CalendarTo.TodayDate = DateFrom;

            //Day = ultraCalendarInfo1.GetDay(DateTo, true);

            CalendarTo.SelectionEnd = DateTo;
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
