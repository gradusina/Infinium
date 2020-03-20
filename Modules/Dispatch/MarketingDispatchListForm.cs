using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingDispatchListForm : Form
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


        private Modules.Dispatch.MarketingDispatchList DispatchList;


        public MarketingDispatchListForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void MarketingDispatchListForm_Shown(object sender, EventArgs e)
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
                        this.Close();
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
                        this.Close();
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
            DispatchList = new Modules.Dispatch.MarketingDispatchList(ref MegaOrdersDataGrid);

            FilterClientsDataGrid.DataSource = DispatchList.FilterClientsBindingSource;
            FilterClientsDataGrid.Columns["ClientID"].Visible = false;
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

        private void ProfilPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int ClientID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["ClientID"]);
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = false;

            bool PressOK = false;
            bool ColorFullName = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            ColorFullName = MarketingDispatchInfoMenu.ColorFullName;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, ClientID, NeedProfilList, NeedTPSList, false, ColorFullName);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TPSPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int ClientID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["ClientID"]);
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = false;
            bool NeedTPSList = true;

            bool PressOK = false;
            bool ColorFullName = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            ColorFullName = MarketingDispatchInfoMenu.ColorFullName;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, ClientID, NeedProfilList, NeedTPSList, false, ColorFullName);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void AllPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int ClientID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["ClientID"]);
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = true;

            bool PressOK = false;
            bool ColorFullName = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            ColorFullName = MarketingDispatchInfoMenu.ColorFullName;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, ClientID, NeedProfilList, NeedTPSList, false, ColorFullName);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
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
            int ClientID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["ClientID"]);
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = true;

            bool PressOK = false;
            bool ColorFullName = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            ColorFullName = MarketingDispatchInfoMenu.ColorFullName;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, ClientID, NeedProfilList, NeedTPSList, true, ColorFullName);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ProfilAttach_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int ClientID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["ClientID"]);
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = true;
            bool NeedTPSList = false;

            bool PressOK = false;
            bool ColorFullName = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            ColorFullName = MarketingDispatchInfoMenu.ColorFullName;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, ClientID, NeedProfilList, NeedTPSList, true, ColorFullName);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TPSAttach_Click(object sender, EventArgs e)
        {
            DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int ClientID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["ClientID"]);
            int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            bool NeedProfilList = false;
            bool NeedTPSList = true;

            bool PressOK = false;
            bool ColorFullName = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            ColorFullName = MarketingDispatchInfoMenu.ColorFullName;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.CreateReport(MegaOrderID, ClientID, NeedProfilList, NeedTPSList, true, ColorFullName);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        //private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        //{
        //    DispatchList.MegaOrdersBindingSource.Position = CurrentRowIndex;
        //    int MegaOrderID = Convert.ToInt32(((DataRowView)DispatchList.MegaOrdersBindingSource.Current)["MegaOrderID"]);
        //    bool NeedProfilList = true;
        //    bool NeedTPSList = true;

        //    Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
        //    T.Start();

        //    while (!SplashWindow.bSmallCreated) ;

        //    DispatchList.CreateClientReport(MegaOrderID, NeedProfilList, NeedTPSList, true);
        //    while (SplashWindow.bSmallCreated)
        //        SmallWaitForm.CloseS = true;
        //}

        private void ShowDispatchedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DispatchList != null)
                Filter();
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void Filter()
        {
            bool bClient = ClientCheckBox.Checked;
            bool bByDispatchSchedule = DispatchScheduleCheckBox.Checked;
            bool bShowDispatched = ShowDispatchedCheckBox.Checked;

            int ClientID = -1;

            if (ClientCheckBox.Checked)
                ClientID = Convert.ToInt32(FilterClientsDataGrid.SelectedRows[0].Cells["ClientID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            DispatchList.Filter(bClient, bShowDispatched, bByDispatchSchedule, ClientID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

        }

        private void FilterClientsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (DispatchList != null && ClientCheckBox.Checked)
                Filter();
        }

        private void ClientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DispatchList != null)
                Filter();
        }

        private void DispatchScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ClientCheckBox.Enabled = !DispatchScheduleCheckBox.Checked;
            ShowDispatchedCheckBox.Enabled = !DispatchScheduleCheckBox.Checked;

            if (DispatchList != null)
                Filter();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }
    }
}
