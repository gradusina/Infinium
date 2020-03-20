using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVDispatchListForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;
        int CurrentRowIndex = 0;
        int CurrentColumnIndex = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;


        private Modules.Dispatch.ZOVDispatchList DispatchList;


        public ZOVDispatchListForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void ZOVDispatchListForm_Shown(object sender, EventArgs e)
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


        private void Initialize()
        {
            DateTime FirstDay = DateTime.Now.AddDays(-8);
            DateTime Today = DateTime.Now;

            CalendarFrom.SelectionStart = FirstDay;
            DispatchList = new Modules.Dispatch.ZOVDispatchList(ref MegaOrdersDataGrid);
        }

        private void MegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void MegaOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentColumnIndex = e.ColumnIndex;
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }


        //private void AttachContextMenuItem_Click(object sender, EventArgs e)
        //{
        //    DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
        //    int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);

        //    Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
        //    T.Start();

        //    while (!SplashWindow.bSmallCreated) ;

        //    DispatchList.CreateReport(MegaOrderID);
        //    while (SplashWindow.bSmallCreated)
        //        SmallWaitForm.CloseS = true;
        //}

        private void ProfilPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = false;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, NeedProfilList, NeedTPSList, false);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TPSPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = false;
            bool NeedTPSList = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, NeedProfilList, NeedTPSList, false);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void AllPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, NeedProfilList, NeedTPSList, false);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
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

        private void AllAttach_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, NeedProfilList, NeedTPSList, true);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ProfilAttach_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = false;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, NeedProfilList, NeedTPSList, true);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TPSAttach_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = false;
            bool NeedTPSList = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, NeedProfilList, NeedTPSList, true);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            sw.Stop();
            double G = sw.Elapsed.TotalSeconds;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void ExpFilterButton_Click(object sender, EventArgs e)
        {
            DateTime FirstDate = CalendarFrom.SelectionStart;
            DateTime SecondDate = CalendarTo.SelectionStart;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.Filter(FirstDate, SecondDate);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
