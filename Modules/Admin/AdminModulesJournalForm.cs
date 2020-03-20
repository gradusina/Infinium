using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AdminModulesJournalForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;
        public AdminModulesJournal AdminModulesJournal = null;
        AdminModulesJournalToExcel AdminModulesJournalToExcel = null;

        public AdminModulesJournalForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();


            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void AdminModulesJournalForm_Shown(object sender, EventArgs e)
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
            AdminModulesJournal = new AdminModulesJournal(ref JournalDataGrid);

            ResultUsersTimeDataGrid.DataSource = AdminModulesJournal.ResultUsersTimeBingingSource;
            ResultUsersTimeDataGrid.Columns["UserName"].HeaderText = "Пользователь";
            ResultUsersTimeDataGrid.Columns["ComputerName"].HeaderText = "Компьютер";
            ResultUsersTimeDataGrid.Columns["TotalTime"].HeaderText = "Общее время\r\nв программе";
            ResultUsersTimeDataGrid.Columns["AVGTime"].HeaderText = "Среднее время\r\nв программе";
            ResultUsersTimeDataGrid.Columns["TotalTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ResultUsersTimeDataGrid.Columns["TotalTime"].Width = 130;
            ResultUsersTimeDataGrid.Columns["AVGTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ResultUsersTimeDataGrid.Columns["AVGTime"].Width = 130;

            TotalResultUsersDataGrid.DataSource = AdminModulesJournal.TotalResultUsersBingingSource;
            if (TotalResultUsersDataGrid.Columns.Contains("TotalMinutes"))
                TotalResultUsersDataGrid.Columns["TotalMinutes"].Visible = false;
            TotalResultUsersDataGrid.Columns["UserName"].HeaderText = "Пользователь";
            TotalResultUsersDataGrid.Columns["ModuleName"].HeaderText = "Модуль";
            TotalResultUsersDataGrid.Columns["Rating"].HeaderText = "Рейтинг";
            TotalResultUsersDataGrid.Columns["TotalTime"].HeaderText = "Общее время";
            TotalResultUsersDataGrid.Columns["Rating"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TotalResultUsersDataGrid.Columns["Rating"].Width = 85;
            TotalResultUsersDataGrid.Columns["TotalTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TotalResultUsersDataGrid.Columns["TotalTime"].Width = 130;

            ResultModulesDataGrid.DataSource = AdminModulesJournal.ResultModulesBingingSource;
            ResultModulesDataGrid.Columns["UserName"].HeaderText = "Пользователь";
            ResultModulesDataGrid.Columns["ModuleName"].HeaderText = "Модуль";
            ResultModulesDataGrid.Columns["Rating"].HeaderText = "Рейтинг";
            ResultModulesDataGrid.Columns["Rating"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ResultModulesDataGrid.Columns["Rating"].Width = 85;

            TotalResultModulesDataGrid.DataSource = AdminModulesJournal.TotalResultModulesBingingSource;
            TotalResultModulesDataGrid.Columns["ModuleName"].HeaderText = "Модуль";
            TotalResultModulesDataGrid.Columns["TotalCount"].HeaderText = "Рейтинг";
            TotalResultModulesDataGrid.Columns["TotalCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TotalResultModulesDataGrid.Columns["TotalCount"].Width = 85;

            UsersDataGrid.DataSource = AdminModulesJournal.UsersBingingSource;
            UsersDataGrid.Columns["UserID"].Visible = false;
            UsersDataGrid.Columns["ShortName"].Visible = false;
            UsersDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            UsersDataGrid.Columns["Name"].MinimumWidth = 150;

            ModulesDataGrid.DataSource = AdminModulesJournal.ModulesBingingSource;
            ModulesDataGrid.Columns["ModuleID"].Visible = false;
            ModulesDataGrid.Columns["ModuleName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ModulesDataGrid.Columns["ModuleName"].MinimumWidth = 150;

            int UserID = 0;
            int ModuleID = 0;

            if (!AllUsersCheckBox.Checked)
                UserID = Convert.ToInt32(UsersDataGrid.SelectedRows[0].Cells["UserID"].Value);

            if (!AllModulesCheckBox.Checked)
                ModuleID = Convert.ToInt32(ModulesDataGrid.SelectedRows[0].Cells["ModuleID"].Value);

            AdminModulesJournal.FilterLoginJournal(UserID, ModuleID, CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, CodersCheckBox.Checked);
            AdminModulesJournal.FilterModulesJournal(UserID, ModuleID, CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, CodersCheckBox.Checked);
            AdminModulesJournal.GetTotalResultUsersTable();
            AdminModulesJournal.GetResultModulesTable();
            AdminModulesJournal.GetResultUsersTimeTable(CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd);

            Chart.DataSource = AdminModulesJournal.ChartDataTable;

            Chart.SeriesDataMember = "Hour";
            Chart.SeriesTemplate.ArgumentDataMember = "Hour";
            Chart.SeriesTemplate.ValueDataMembersSerializable = "Percent";

            Chart.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;

            //Chart.SeriesTemplate.Label.Font = new Font("Segoe UI", 12.0f, FontStyle.Bold);
            //Chart.SeriesTemplate.Label.TextColor = Color.White;
            //Chart.SeriesTemplate.Label.Border.Visible = false;
            //Chart.SeriesTemplate.Label.BackColor = Color.Transparent;
            //Chart.SeriesTemplate.Label.Antialiasing = true;

            ((DevExpress.XtraCharts.XYDiagram)Chart.Diagram).AxisX.Label.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.True;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            int UserID = 0;
            int ModuleID = 0;

            if (!AllUsersCheckBox.Checked)
                UserID = Convert.ToInt32(UsersDataGrid.SelectedRows[0].Cells["UserID"].Value);

            if (!AllModulesCheckBox.Checked)
                ModuleID = Convert.ToInt32(ModulesDataGrid.SelectedRows[0].Cells["ModuleID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            AdminModulesJournal.FilterLoginJournal(UserID, ModuleID, CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, CodersCheckBox.Checked);
            AdminModulesJournal.FilterModulesJournal(UserID, ModuleID, CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, CodersCheckBox.Checked);
            AdminModulesJournal.GetTotalResultUsersTable();
            AdminModulesJournal.GetResultModulesTable();
            AdminModulesJournal.GetResultUsersTimeTable(CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd);

            Chart.RefreshData();

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

        private void btnToExcel_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            AdminModulesJournalToExcel = new AdminModulesJournalToExcel();
            AdminModulesJournalToExcel.CreateSheet1(AdminModulesJournal.ModulesJournalDT);
            AdminModulesJournalToExcel.CreateSheet2(AdminModulesJournal.ResultUsersTimeDT, AdminModulesJournal.TotalResultUsersDT);
            AdminModulesJournalToExcel.CreateSheet3(AdminModulesJournal.ResultModulesDT, AdminModulesJournal.TotalResultModulesDT);
            AdminModulesJournalToExcel.SaveOpenReport(CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
