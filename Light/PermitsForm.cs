using Infinium.Modules.Permits;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PermitsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;
        bool CallFromLightStartForm = true;
        public bool BindingOk = false;

        int FormEvent = 0;
        int BindType = 0;
        int[] Dispatches = new int[1];
        int UnloadID = -1;
        object ZOVDispatchDate = null;

        Form MainForm;
        Form TopForm;
        LightStartForm LightStartForm;

        //RoleTypes RoleType = RoleTypes.OrdinaryRole;

        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1,
            LogisticsRole = 2,
            ConfirmRole = 3,
            DispatchRole = 4
        }

        DataTable RolePermissionsDataTable;
        Permits PermitsManager;

        public PermitsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);

            Initialize();
            UpdatePermits();

            while (!SplashForm.bCreated) ;
        }

        public PermitsForm(Form tMainForm, object oZOVDispatchDate)
        {
            InitializeComponent();
            MainForm = tMainForm;
            BindType = 2;
            btnBindPermit.Visible = true;
            MinimizeButton.Visible = false;
            cmiBindPermit.Visible = true;
            CallFromLightStartForm = false;
            if (BindType == 2)
                ZOVDispatchDate = oZOVDispatchDate;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);

            Initialize();
            UpdatePermits();

            while (!SplashForm.bCreated) ;
        }

        public PermitsForm(Form tMainForm, int[] iDispatches)
        {
            InitializeComponent();
            MainForm = tMainForm;
            BindType = 1;
            btnBindPermit.Visible = true;
            MinimizeButton.Visible = false;
            cmiBindPermit.Visible = true;
            CallFromLightStartForm = false;
            Dispatches = new int[iDispatches.Count()];
            Dispatches = iDispatches;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);

            Initialize();
            UpdatePermits();

            while (!SplashForm.bCreated) ;
        }

        public PermitsForm(Form tMainForm, int ID)
        {
            InitializeComponent();
            MainForm = tMainForm;
            BindType = 3;
            btnBindPermit.Visible = true;
            MinimizeButton.Visible = false;
            cmiBindPermit.Visible = true;
            CallFromLightStartForm = false;
            UnloadID = ID;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);

            Initialize();
            UpdatePermits();

            while (!SplashForm.bCreated) ;
        }


        private void PermitsForm_Shown(object sender, EventArgs e)
        {
            NeedSplash = true;
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
                        if (CallFromLightStartForm)
                            LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        NeedSplash = false;
                        if (CallFromLightStartForm)
                            LightStartForm.HideForm(this);
                        else
                        {
                            MainForm.Activate();
                            this.Hide();
                        }
                    }


                    return;
                }

                if (FormEvent == eShow)
                {
                    NeedSplash = true;
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
                        if (CallFromLightStartForm)
                            LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        NeedSplash = false;
                        if (CallFromLightStartForm)
                            LightStartForm.HideForm(this);
                        else
                        {
                            MainForm.Activate();
                            this.Hide();
                        }
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
                    NeedSplash = true;
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

        private bool PermissionGranted(int PermissionID)
        {
            DataRow[] Rows = RolePermissionsDataTable.Select("PermissionID = " + PermissionID);

            if (Rows.Count() > 0)
            {
                return Convert.ToBoolean(Rows[0]["Granted"]);
            }

            return false;
        }

        private void Initialize()
        {
            DataTable MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));

            DataTable YearsDT = new DataTable();
            YearsDT.Columns.Add(new DataColumn("YearID", Type.GetType("System.Int32")));
            YearsDT.Columns.Add(new DataColumn("YearName", Type.GetType("System.String")));

            for (int i = 1; i <= 12; i++)
            {
                DataRow NewRow = MonthsDT.NewRow();
                NewRow["MonthID"] = i;
                NewRow["MonthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToString();
                MonthsDT.Rows.Add(NewRow);
            }
            cbxMonths.DataSource = MonthsDT.DefaultView;
            cbxMonths.ValueMember = "MonthID";
            cbxMonths.DisplayMember = "MonthName";

            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            System.Collections.ArrayList Years = new System.Collections.ArrayList();
            for (int i = 2013; i <= LastDay.Year; i++)
            {
                DataRow NewRow = YearsDT.NewRow();
                NewRow["YearID"] = i;
                NewRow["YearName"] = i;
                YearsDT.Rows.Add(NewRow);
                Years.Add(i);
            }
            cbxYears.DataSource = YearsDT.DefaultView;
            cbxYears.ValueMember = "YearID";
            cbxYears.DisplayMember = "YearName";

            cbxMonths.SelectedValue = DateTime.Now.Month;
            cbxYears.SelectedValue = DateTime.Now.Year;

            PermitsManager = new Permits();
            PermitsManager.Initialize();
            dgvPermitsDates.DataSource = PermitsManager.PermitsDatesList;
            dgvPermitsDatesSetting();
            dgvPermits.DataSource = PermitsManager.PermitsList;
            dgvPermits.Columns.Add(PermitsManager.CreateUserColumn);
            dgvPermits.Columns.Add(PermitsManager.SignUserColumn);
            dgvPermitsSettings(ref dgvPermits);
            dgvMDispatch.DataSource = PermitsManager.MDispatchList;
            dgvDispatchSetting(ref dgvMDispatch);
            dgvZDispatch.DataSource = PermitsManager.ZDispatchList;
            dgvDispatchSetting(ref dgvZDispatch);
            dgvUnloads.DataSource = PermitsManager.UnloadsList;
            dgvUnloadsSetting(ref dgvUnloads);
        }

        private void dgvPermitsDatesSetting()
        {
            dgvPermitsDates.AutoGenerateColumns = false;

            if (dgvPermitsDates.Columns.Contains("PrepareDispatchDateTime"))
            {
                dgvPermitsDates.Columns["PrepareDispatchDateTime"].DefaultCellStyle.Format = "dd MMMM dddd";
                dgvPermitsDates.Columns["PrepareDispatchDateTime"].MinimumWidth = 150;
                dgvPermitsDates.Columns["PrepareDispatchDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvPermitsDates.Columns["PrepareDispatchDateTime"].DisplayIndex = 0;
            }
            if (dgvPermitsDates.Columns.Contains("WeekNumber"))
            {
                dgvPermitsDates.Columns["WeekNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvPermitsDates.Columns["WeekNumber"].Width = 70;
                dgvPermitsDates.Columns["WeekNumber"].DisplayIndex = 1;
            }
        }

        private void dgvDispatchSetting(ref PercentageDataGrid Grid)
        {
            //foreach (DataGridViewColumn Column in Grid.Columns)
            //{
            //    Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //}

            Grid.AutoGenerateColumns = false;

            Grid.Columns["ClientID"].Visible = false;
            Grid.Columns["ConfirmExpUserID"].Visible = false;
            Grid.Columns["ConfirmDispUserID"].Visible = false;
            Grid.Columns["PrepareDispatchDateTime"].Visible = false;

            Grid.Columns["CreationDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            Grid.Columns["ConfirmDispDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            Grid.Columns["RealDispDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            Grid.Columns["ClientName"].HeaderText = "Клиент";
            Grid.Columns["Weight"].HeaderText = "Вес, кг";
            Grid.Columns["CreationDateTime"].HeaderText = "Дата\r\nсоздания";
            Grid.Columns["DispPackagesCount"].HeaderText = "Кол-во\r\nупаковок";
            Grid.Columns["DispatchStatus"].HeaderText = "Статус";
            Grid.Columns["ConfirmExpDateTime"].HeaderText = "Эксп-ция\r\nутверждена";
            Grid.Columns["ConfirmDispDateTime"].HeaderText = "Отгрузка\r\nутверждена";
            Grid.Columns["ConfirmExpUser"].HeaderText = "Утвердил\r\nэксп-цию";
            Grid.Columns["ConfirmDispUser"].HeaderText = "Утвердил\r\nотгрузку";
            Grid.Columns["RealDispDateTime"].HeaderText = "Дата отгрузки";
            //Grid.Columns["ClientName"].HeaderText = "Клиент";
            //Grid.Columns["Weight"].HeaderText = "Вес, кг";
            //Grid.Columns["CreationDateTime"].HeaderText = "   Дата\r\nсоздания";
            //Grid.Columns["DispPackagesCount"].HeaderText = "  Кол-во\r\nупаковок";
            //Grid.Columns["DispatchStatus"].HeaderText = "Статус";
            //Grid.Columns["ConfirmExpDateTime"].HeaderText = "  Эксп-ция\r\nутверждена";
            //Grid.Columns["ConfirmDispDateTime"].HeaderText = "  Отгрузка\r\nутверждена";
            //Grid.Columns["ConfirmExpUser"].HeaderText = "Утвердил\r\nэксп-цию";
            //Grid.Columns["ConfirmDispUser"].HeaderText = "Утвердил\r\n отгрузку";
            //Grid.Columns["RealDispDateTime"].HeaderText = "Дата отгрузки";

            Grid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["Weight"].Width = 80;
            Grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["ClientName"].MinimumWidth = 200;
            Grid.Columns["RealDispDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["RealDispDateTime"].Width = 130;
            Grid.Columns["CreationDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["CreationDateTime"].Width = 130;
            Grid.Columns["DispPackagesCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["DispPackagesCount"].Width = 90;
            Grid.Columns["DispatchStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid.Columns["DispatchStatus"].MinimumWidth = 100;
            Grid.Columns["ConfirmExpDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["ConfirmExpDateTime"].Width = 130;
            Grid.Columns["ConfirmExpUser"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["ConfirmExpUser"].MinimumWidth = 150;
            Grid.Columns["ConfirmDispDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["ConfirmDispDateTime"].Width = 130;
            Grid.Columns["ConfirmDispUser"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["ConfirmDispUser"].MinimumWidth = 150;
            Grid.Columns["DispatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["DispatchID"].Width = 1;

            int DisplayIndex = 0;
            Grid.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            Grid.Columns["CreationDateTime"].DisplayIndex = DisplayIndex++;
            Grid.Columns["DispPackagesCount"].DisplayIndex = DisplayIndex++;
            Grid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            Grid.Columns["DispatchStatus"].DisplayIndex = DisplayIndex++;
            Grid.Columns["ConfirmExpDateTime"].DisplayIndex = DisplayIndex++;
            Grid.Columns["ConfirmExpUser"].DisplayIndex = DisplayIndex++;
            Grid.Columns["ConfirmDispDateTime"].DisplayIndex = DisplayIndex++;
            Grid.Columns["ConfirmDispUser"].DisplayIndex = DisplayIndex++;
            Grid.Columns["RealDispDateTime"].DisplayIndex = DisplayIndex++;
            Grid.Columns["DispatchID"].DisplayIndex = DisplayIndex++;

            Grid.Columns["DispPackagesCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void dgvPermitsSettings(ref PercentageDataGrid Grid)
        {
            if (Grid.Columns.Contains("ZOVDispatchDate"))
                Grid.Columns["ZOVDispatchDate"].Visible = false;
            if (Grid.Columns.Contains("PermitID"))
                Grid.Columns["PermitID"].Visible = false;
            if (Grid.Columns.Contains("MarketingDispatchID"))
                Grid.Columns["MarketingDispatchID"].Visible = false;
            if (Grid.Columns.Contains("ZOVDispatchID"))
                Grid.Columns["ZOVDispatchID"].Visible = false;
            if (Grid.Columns.Contains("UnloadID"))
                Grid.Columns["UnloadID"].Visible = false;
            if (Grid.Columns.Contains("UserID"))
                Grid.Columns["UserID"].Visible = false;
            if (Grid.Columns.Contains("SignUserID"))
                Grid.Columns["SignUserID"].Visible = false;
            if (Grid.Columns.Contains("Visitor"))
                Grid.Columns["Visitor"].Visible = false;

            Grid.Columns["SecurityCheckDate"].HeaderText = "Дата прохождения\r\nпроходной";
            Grid.Columns["CreateDate"].HeaderText = "Дата создания";
            Grid.Columns["SignedDate"].HeaderText = "Дата утверждения";
            Grid.Columns["SecurityChecked"].HeaderText = "Отметка на\r\nпроходной";
            //Grid.Columns["SecurityCheckDate"].HeaderText = "Дата прохождения\r\n       проходной";
            Grid.Columns["Name"].HeaderText = "Кому выдан";
            Grid.Columns["Purpose"].HeaderText = "Цель выдачи";

            Grid.Columns["CreateDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["CreateDate"].MinimumWidth = 100;
            Grid.Columns["SignedDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["SignedDate"].MinimumWidth = 100;
            Grid.Columns["SecurityCheckDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["SecurityCheckDate"].MinimumWidth = 100;
            Grid.Columns["SecurityChecked"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["SecurityChecked"].MinimumWidth = 100;
            Grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["Name"].MinimumWidth = 100;
            Grid.Columns["Purpose"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Grid.Columns["Purpose"].MinimumWidth = 100;

            Grid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            Grid.Columns["CreateDate"].DisplayIndex = DisplayIndex++;
            Grid.Columns["CreateUserColumn"].DisplayIndex = DisplayIndex++;
            Grid.Columns["Name"].DisplayIndex = DisplayIndex++;
            Grid.Columns["Purpose"].DisplayIndex = DisplayIndex++;
            Grid.Columns["SignedDate"].DisplayIndex = DisplayIndex++;
            Grid.Columns["SignUserColumn"].DisplayIndex = DisplayIndex++;
            Grid.Columns["SecurityChecked"].DisplayIndex = DisplayIndex++;
            Grid.Columns["SecurityCheckDate"].DisplayIndex = DisplayIndex++;
        }

        private void dgvUnloadsSetting(ref PercentageDataGrid Grid)
        {
            //foreach (DataGridViewColumn Column in Grid.Columns)
            //{
            //    Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //}

            Grid.AutoGenerateColumns = false;
            Grid.Columns["UserName"].HeaderText = "ФИО";
            Grid.Columns["UnloadID"].HeaderText = "№\r\nнакладной";
            Grid.Columns["UnloadDateTime"].HeaderText = "Дата возврата";
            Grid.Columns["OrderedDateTime"].HeaderText = "Когда вернул";
            Grid.Columns["NeedReturnObject"].HeaderText = "С возвратом";
            Grid.Columns["OutObject"].HeaderText = "Проверка на\r\nпроходной";
            Grid.Columns["FactReturnDateTime"].HeaderText = "Фактическая\r\nдата возврата";
            Grid.Columns["ReturnObject"].HeaderText = "Подтвержд.\r\nвозврата";
            Grid.Columns["Notes"].HeaderText = "Примечание";

            Grid.Columns["UserID"].Visible = false;
            Grid.Columns["ResponsibleUserID"].Visible = false;
            Grid.Columns["OrderedDateTime"].Visible = false;
            Grid.Columns["ReturnNotes"].Visible = false;
            Grid.Columns["Notes"].Visible = false;

            Grid.Columns["UnloadID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["UnloadID"].Width = 100;
            Grid.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid.Columns["UnloadDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["UnloadDateTime"].Width = 130;
            Grid.Columns["FactReturnDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["FactReturnDateTime"].Width = 130;
            Grid.Columns["NeedReturnObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["NeedReturnObject"].Width = 120;
            Grid.Columns["ReturnObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["ReturnObject"].Width = 120;
            Grid.Columns["OutObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["OutObject"].Width = 120;

        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void PermitsForm_Load(object sender, EventArgs e)
        {
        }

        private void btnNewPermit_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            bool Visitor = false;
            object Name = DBNull.Value;
            object Purpose = DBNull.Value;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewPermitMenu NewPermitMenu = new NewPermitMenu(this);
            TopForm = NewPermitMenu;
            NewPermitMenu.ShowDialog();

            Visitor = NewPermitMenu.Visitor;
            PressOK = NewPermitMenu.PressOK;
            Name = NewPermitMenu.sName;
            Purpose = NewPermitMenu.sPurpose;

            PhantomForm.Close();
            PhantomForm.Dispose();
            NewPermitMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.NewPermit(Visitor, Name.ToString(), Purpose.ToString());
            PermitsManager.SavePermits();
            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void UpdatePermits()
        {
            DateTime Date = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            object CreateDate = PermitsManager.CurrentCreateDate;
            int PermitID = 0;
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["PermitID"].Value != DBNull.Value)
                PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["PermitID"].Value);
            PermitsManager.ClearPermits();
            PermitsManager.UpdatePermitsDates(Date);
            if (CreateDate != DBNull.Value)
            {
                PermitsManager.MoveToCreateDate(Convert.ToDateTime(CreateDate));
                PermitsManager.MoveToPermit(PermitID);
            }
            //PermitsManager.FilterPermitsByCreateDate(Date);
            if (!PermitsManager.HasPermits)
            {
                cbtnMDispatch.Visible = false;
                cbtnZDispatch.Visible = false;
                cbtnUnloads.Visible = false;
                cbtnVisitors.Visible = false;
                pnlMDispatch.Visible = false;
                pnlZDispatch.Visible = false;
                pnlUnloads.Visible = false;
                pnlVisitors.Visible = false;
            }
        }

        private void mcmcCreateDate_DateChanged(object sender, DateRangeEventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnSignPermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0)
                return;
            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["PermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.SignPermit(PermitID);
            PermitsManager.SavePermits();
            UpdatePermits();
            PermitsManager.MoveToPermit(PermitID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Пропуск утвержден", 1700);
        }

        private void dgvPermits_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dgvPermits.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnRemovePermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Пропуск будет удален. Продолжить?",
                    "Удаление пропуска");

            if (!OKCancel)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["PermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.RemovePermit(PermitID);
            PermitsManager.SavePermits();
            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Пропуск удален", 1700);
        }

        private void btnBindPermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0)
                return;

            //bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //        "Пропуск будет удален. Продолжить?",
            //        "Удаление пропуска");

            //if (!OKCancel)
            //    return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["PermitID"].Value);
            if (BindType == 1)
                PermitsManager.BindPermitToMarketingDispatch(PermitID, Dispatches);
            if (BindType == 2)
            {
                if (ZOVDispatchDate != null)
                    PermitsManager.BindPermitToZOVDispatch(PermitID, Convert.ToDateTime(ZOVDispatchDate));
            }
            if (BindType == 3)
                PermitsManager.BindPermitToUnload(PermitID, UnloadID);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.SavePermits();
            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            BindingOk = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void dgvPermits_SelectionChanged(object sender, EventArgs e)
        {
            label2.Text = string.Empty;
            label5.Text = string.Empty;
            PermitsManager.ClearZDispatch();
            PermitsManager.ClearMDispatch();
            PermitsManager.ClearUnloads();
            if (dgvPermits.SelectedRows.Count == 0)
                return;

            bool Visitor = false;
            int UnloadID = 0;
            int MarketingDispatchID = 0;
            int PermitID = 0;

            if (dgvPermits.SelectedRows[0].Cells["Visitor"].Value != DBNull.Value)
                Visitor = Convert.ToBoolean(dgvPermits.SelectedRows[0].Cells["Visitor"].Value);
            if (dgvPermits.SelectedRows[0].Cells["UnloadID"].Value != DBNull.Value)
                UnloadID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["UnloadID"].Value);
            if (dgvPermits.SelectedRows[0].Cells["MarketingDispatchID"].Value != DBNull.Value)
                MarketingDispatchID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["MarketingDispatchID"].Value);
            if (dgvPermits.SelectedRows[0].Cells["PermitID"].Value != DBNull.Value)
                PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["PermitID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;


                if (PermitID != 0)
                {
                    cbtnMDispatch.Visible = true;
                    pnlMDispatch.Visible = true;
                    cbtnMDispatch.Checked = true;
                    PermitsManager.GetMDispatch(PermitID);
                }
                else
                {
                    cbtnMDispatch.Visible = false;
                    pnlMDispatch.Visible = false;
                }

                if (dgvPermits.SelectedRows[0].Cells["ZOVDispatchDate"].Value != DBNull.Value)
                {
                    cbtnZDispatch.Visible = true;
                    pnlZDispatch.Visible = true;
                    cbtnZDispatch.Checked = true;
                    PermitsManager.GetZDispatch(Convert.ToDateTime(dgvPermits.SelectedRows[0].Cells["ZOVDispatchDate"].Value));
                }
                else
                {
                    cbtnZDispatch.Visible = false;
                    pnlZDispatch.Visible = false;
                }
                if (UnloadID != 0)
                {
                    cbtnUnloads.Checked = true;
                    PermitsManager.GetUnoads(UnloadID);
                }
                else
                {
                    cbtnUnloads.Visible = false;
                    pnlUnloads.Visible = false;
                }
                if (Visitor)
                {
                    cbtnVisitors.Visible = true;
                    pnlVisitors.Visible = true;
                    cbtnVisitors.Checked = true;
                    label2.Text = "Посетитель: " + dgvPermits.SelectedRows[0].Cells["Name"].Value.ToString();
                    label5.Text = "Цель: " + dgvPermits.SelectedRows[0].Cells["Purpose"].Value.ToString();
                }
                else
                {
                    cbtnVisitors.Visible = false;
                    pnlVisitors.Visible = false;
                }

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                if (PermitID != 0)
                {
                    cbtnMDispatch.Visible = true;
                    pnlMDispatch.Visible = true;
                    cbtnMDispatch.Checked = true;
                    PermitsManager.GetMDispatch(PermitID);
                }
                else
                {
                    cbtnMDispatch.Visible = false;
                    pnlMDispatch.Visible = false;
                }

                if (dgvPermits.SelectedRows[0].Cells["ZOVDispatchDate"].Value != DBNull.Value)
                {
                    cbtnZDispatch.Visible = true;
                    pnlZDispatch.Visible = true;
                    cbtnZDispatch.Checked = true;
                    PermitsManager.GetZDispatch(Convert.ToDateTime(dgvPermits.SelectedRows[0].Cells["ZOVDispatchDate"].Value));
                }
                else
                {
                    cbtnZDispatch.Visible = false;
                    pnlZDispatch.Visible = false;
                }
                if (UnloadID != 0)
                {
                    cbtnUnloads.Checked = true;
                    PermitsManager.GetUnoads(UnloadID);
                }
                else
                {
                    cbtnUnloads.Visible = false;
                    pnlUnloads.Visible = false;
                }
                if (Visitor)
                {
                    cbtnVisitors.Visible = true;
                    pnlVisitors.Visible = true;
                    cbtnVisitors.Checked = true;
                    label2.Text = "Посетитель: " + dgvPermits.SelectedRows[0].Cells["Name"].Value.ToString();
                    label5.Text = "Цель: " + dgvPermits.SelectedRows[0].Cells["Purpose"].Value.ToString();
                }
                else
                {
                    cbtnVisitors.Visible = false;
                    pnlVisitors.Visible = false;
                }
            }
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (PermitsManager == null)
                return;
            if (kryptonCheckSet1.CheckedButton == cbtnMDispatch)
            {
                pnlMDispatch.BringToFront();
                pnlMDispatch.Visible = true;
            }
            if (kryptonCheckSet1.CheckedButton == cbtnZDispatch)
            {
                pnlZDispatch.BringToFront();
                pnlZDispatch.Visible = true;
            }
            if (kryptonCheckSet1.CheckedButton == cbtnUnloads)
            {
                pnlUnloads.BringToFront();
                pnlUnloads.Visible = true;
            }
            if (kryptonCheckSet1.CheckedButton == cbtnVisitors)
            {
                pnlVisitors.BringToFront();
                pnlVisitors.Visible = true;
            }
        }

        private void dgvUnloads_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!CallFromLightStartForm)
                return;
            if (!CallFromLightStartForm || dgvUnloads.SelectedRows.Count == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            WealthForm WealthForm = new WealthForm(this);

            TopForm = WealthForm;

            WealthForm.ShowDialog();

            WealthForm.Close();
            WealthForm.Dispose();

            TopForm = null;
        }

        private void dgvZDispatch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!CallFromLightStartForm || dgvZDispatch.SelectedRows.Count == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;
            int ZOVDispatchID = 0;
            object PrepareDispatchDateTime = dgvZDispatch.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;

            if (dgvZDispatch.SelectedRows[0].Cells["DispatchID"].Value != DBNull.Value)
                ZOVDispatchID = Convert.ToInt32(dgvZDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            if (PrepareDispatchDateTime == DBNull.Value)
                return;
            ZOVExpeditionForm ZOVExpeditionForm = new ZOVExpeditionForm(this,
                Convert.ToDateTime(dgvZDispatch.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value), ZOVDispatchID);

            TopForm = ZOVExpeditionForm;

            ZOVExpeditionForm.ShowDialog();

            ZOVExpeditionForm.Close();
            ZOVExpeditionForm.Dispose();

            TopForm = null;
        }

        private void dgvMDispatch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!CallFromLightStartForm)
                return;
            if (!CallFromLightStartForm || dgvMDispatch.SelectedRows.Count == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;
            int MarketingDispatchID = 0;
            object PrepareDispatchDateTime = dgvMDispatch.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;

            if (dgvMDispatch.SelectedRows[0].Cells["DispatchID"].Value != DBNull.Value)
                MarketingDispatchID = Convert.ToInt32(dgvMDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            if (PrepareDispatchDateTime == DBNull.Value)
                return;
            MarketingExpeditionForm MarketingExpeditionForm = new MarketingExpeditionForm(this,
                Convert.ToDateTime(dgvMDispatch.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value), MarketingDispatchID);

            TopForm = MarketingExpeditionForm;

            MarketingExpeditionForm.ShowDialog();

            MarketingExpeditionForm.Close();
            MarketingExpeditionForm.Dispose();

            TopForm = null;
        }

        private void cbxMonths_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbxYears_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvPermitsDates_SelectionChanged(object sender, EventArgs e)
        {
            if (PermitsManager == null)
                return;
            PermitsManager.ClearPermits();
            if (dgvPermitsDates.SelectedRows.Count == 0)
            {
                return;
            }

            object Date = PermitsManager.CurrentCreateDate;

            if (Date != DBNull.Value)
            {
                if (NeedSplash)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;
                    NeedSplash = false;
                    PermitsManager.FilterPermitsByCreateDate(Convert.ToDateTime(Date));
                    NeedSplash = true;
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
                else
                {
                    PermitsManager.FilterPermitsByCreateDate(Convert.ToDateTime(Date));
                }
            }
        }

    }
}