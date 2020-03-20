using Infinium.Modules.Permits;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MachinesPermitsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        const int iAdminRole = 70;
        const int iAgreedRole = 71;
        const int iApprovedRole = 72;

        bool NeedFilter = false;
        bool NeedSplash = false;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        NewMachinePermit NewPermit;
        MachinesPermits PermitsManager;
        PrintMachinesPermits PrintPermitsManager;

        RoleTypes RoleType = RoleTypes.OrdinaryRole;
        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1,
            AgreedRole = 2,
            ApprovedRole = 3
        }

        public MachinesPermitsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void MachinesPermitsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedSplash = true;
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
                    NeedSplash = true;
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
                    NeedSplash = true;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            PermitsManager = new MachinesPermits();
            PermitsManager.Initialize();

            cbxYears.DataSource = PermitsManager.YearsBS;
            cbxYears.ValueMember = "YearID";
            cbxYears.DisplayMember = "YearName";

            cbxMonths.DataSource = PermitsManager.MonthsBS;
            cbxMonths.ValueMember = "MonthID";
            cbxMonths.DisplayMember = "MonthName";

            cbxMonths.SelectedValue = DateTime.Now.Month;
            cbxYears.SelectedValue = DateTime.Now.Year;

            PrintPermitsManager = new PrintMachinesPermits();
            PermitsManager.GetPermissions(Security.CurrentUserID, this.Name);
            if (PermitsManager.PermissionGranted(iAdminRole))
            {
                RoleType = RoleTypes.AdminRole;
            }
            if (PermitsManager.PermissionGranted(iAgreedRole))
            {
                RoleType = RoleTypes.AgreedRole;
            }
            if (PermitsManager.PermissionGranted(iApprovedRole))
            {
                RoleType = RoleTypes.ApprovedRole;
            }

            dgvPermitsDatesSetting();
            dgvPermitsSettings();
            NeedFilter = true;
            UpdatePermitDates();
        }

        private void dgvPermitsDatesSetting()
        {
            dgvPermitsDates.DataSource = PermitsManager.BsPermitsDates;
            dgvPermitsDates.AutoGenerateColumns = false;

            if (dgvPermitsDates.Columns.Contains("VisitDateTime"))
            {
                dgvPermitsDates.Columns["VisitDateTime"].DefaultCellStyle.Format = "dd MMMM dddd";
                dgvPermitsDates.Columns["VisitDateTime"].MinimumWidth = 150;
                dgvPermitsDates.Columns["VisitDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvPermitsDates.Columns["VisitDateTime"].DisplayIndex = 0;
            }
            if (dgvPermitsDates.Columns.Contains("WeekNumber"))
            {
                dgvPermitsDates.Columns["WeekNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvPermitsDates.Columns["WeekNumber"].Width = 70;
                dgvPermitsDates.Columns["WeekNumber"].DisplayIndex = 1;
            }
        }

        private void dgvPermitsSettings()
        {
            dgvPermits.DataSource = PermitsManager.BsPermits;
            dgvPermits.Columns.Add(PermitsManager.AgreedUserColumn);
            //dgvPermits.Columns.Add(PermitsManager.CreateUserColumn);

            //if (dgvPermits.Columns.Contains("AgreedTime"))
            //    dgvPermits.Columns["AgreedTime"].Visible = false;
            //if (dgvPermits.Columns.Contains("OutputTime"))
            //    dgvPermits.Columns["OutputTime"].Visible = false;

            if (dgvPermits.Columns.Contains("SecurityCheckDate"))
                dgvPermits.Columns["SecurityCheckDate"].Visible = false;

            if (dgvPermits.Columns.Contains("OutputDeniedTime"))
                dgvPermits.Columns["OutputDeniedTime"].Visible = false;
            if (dgvPermits.Columns.Contains("CreateTime"))
                dgvPermits.Columns["CreateTime"].Visible = false;
            if (dgvPermits.Columns.Contains("DeleteTime"))
                dgvPermits.Columns["DeleteTime"].Visible = false;

            if (dgvPermits.Columns.Contains("OutputDeniedUserID"))
                dgvPermits.Columns["OutputDeniedUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("CreateUserID"))
                dgvPermits.Columns["CreateUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("PrintTime"))
                dgvPermits.Columns["PrintTime"].Visible = false;
            if (dgvPermits.Columns.Contains("PrintUserID"))
                dgvPermits.Columns["PrintUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("AgreedUserID"))
                dgvPermits.Columns["AgreedUserID"].Visible = false;

            if (dgvPermits.Columns.Contains("DeleteUserID"))
                dgvPermits.Columns["DeleteUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("PermitEnable"))
                dgvPermits.Columns["PermitEnable"].Visible = false;
            if (dgvPermits.Columns.Contains("Overdued"))
                dgvPermits.Columns["Overdued"].Visible = false;

            foreach (DataGridViewColumn column in dgvPermits.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //column.MinimumWidth = 60;
            }

            dgvPermits.Columns["Validity"].DefaultCellStyle.Format = "dd.MM.yyyy";
            dgvPermits.Columns["OutputDeniedTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["OutputTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["CreateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["PrintTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["AgreedTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvPermits.Columns["MachinePermitID"].HeaderText = "№";
            dgvPermits.Columns["Name"].HeaderText = "Описание";
            dgvPermits.Columns["SecurityChecked"].HeaderText = "Отметка\nна проходной";
            dgvPermits.Columns["VisitMission"].HeaderText = "Цель визита";
            dgvPermits.Columns["Validity"].HeaderText = "Срок\nдействия";

            dgvPermits.Columns["OutputEnable"].HeaderText = "Выход\nразрешен";
            dgvPermits.Columns["OutputDeniedTime"].HeaderText = "Выезд\nзапрещен";
            dgvPermits.Columns["OutputDone"].HeaderText = "Выезд\nпроизведен";
            dgvPermits.Columns["OutputTime"].HeaderText = "Дата\nвыезда";

            dgvPermits.Columns["CreateTime"].HeaderText = "Дата\nвъезда";
            dgvPermits.Columns["PrintTime"].HeaderText = "Дата\nпечати";
            dgvPermits.Columns["AgreedTime"].HeaderText = "Дата\nутверждения";

            dgvPermits.Columns["MachinePermitID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPermits.Columns["MachinePermitID"].MinimumWidth = 60;
            dgvPermits.Columns["OutputEnable"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPermits.Columns["OutputEnable"].MinimumWidth = 110;
            dgvPermits.Columns["OutputDone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPermits.Columns["AgreedUserColumn"].MinimumWidth = 110;
            dgvPermits.Columns["Validity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPermits.Columns["Validity"].MinimumWidth = 100;

            dgvPermits.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvPermits.Columns["MachinePermitID"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["Name"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["VisitMission"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["OutputEnable"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["AgreedUserColumn"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["AgreedTime"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["OutputDone"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["OutputTime"].DisplayIndex = DisplayIndex++;
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

        private void dgvPermits_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.RowIndex > -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dgvPermits.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void UpdatePermitDates()
        {
            if (!NeedFilter)
                return;

            DateTime Date = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            object VisitDateTime = PermitsManager.CurrentVisitDateTime;
            int MachinePermitID = 0;
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value != DBNull.Value)
                MachinePermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                PermitsManager.FilterPermitsDates(Date);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                PermitsManager.FilterPermitsDates(Date);

            if (VisitDateTime != DBNull.Value)
            {
                PermitsManager.MoveToVisitDateTime(Convert.ToDateTime(VisitDateTime));
                PermitsManager.MoveToPermit(MachinePermitID);
            }
        }

        private void UpdatePermits()
        {
            object VisitDateTime = PermitsManager.CurrentVisitDateTime;
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                PermitsManager.FilterPermits(Convert.ToDateTime(VisitDateTime));

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                PermitsManager.FilterPermits(Convert.ToDateTime(VisitDateTime));
        }

        private void cbxMonths_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePermitDates();
        }

        private void cbxYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePermitDates();
        }

        private void dgvPermitsDates_SelectionChanged(object sender, EventArgs e)
        {
            object VisitDateTime = PermitsManager.CurrentVisitDateTime;
            if (VisitDateTime == DBNull.Value)
            {
                PermitsManager.ClearPermits();
                return;
            }
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                PermitsManager.FilterPermits(Convert.ToDateTime(VisitDateTime));

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                PermitsManager.FilterPermits(Convert.ToDateTime(VisitDateTime));
        }

        private void btnCreatePermit_Click(object sender, EventArgs e)
        {
            NewPermit = new Modules.Permits.NewMachinePermit();
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewMachinesPermitForm NewPermitForm = new NewMachinesPermitForm(this, ref NewPermit);
            TopForm = NewPermitForm;
            NewPermitForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            NewPermitForm.Dispose();
            TopForm = null;

            if (!NewPermit.bCreatePermit)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.NewPermit(NewPermit.VisitorName, NewPermit.VisitMission, NewPermit.Validity);

            PermitsManager.SendApproveNotifications(iApprovedRole);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Пропуск создан", 1700);
        }

        private void btnAgreePermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.AgreePermit(PermitID);
            PermitsManager.SavePermits();
            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Выезд утвержден", 1700);
        }

        private void btnDenyPermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.DenyOutput(PermitID);
            PermitsManager.SavePermits();
            UpdatePermits();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Выезд запрещен", 1700);
        }

        private void btnDeletePermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value == DBNull.Value)
                return;

            //if (RoleType != RoleTypes.AdminRole && RoleType != RoleTypes.ApprovedRole)
            //    return;

            if (Convert.ToBoolean(dgvPermits.SelectedRows[0].Cells["OutputDone"].Value))
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Пропуск будет удален. Продолжить?",
                    "Удаление пропуска");

            if (!OKCancel)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PermitsManager.RemovePermit(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Пропуск удален", 1700);
        }

        private void btnPrintPermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["MachinePermitID"].Value);

            PrintPermitsManager.ClearLabelInfo();
            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                PermitsManager.PrintPermit(PermitID);

                PrintPermitsInfo LabelInfo = new PrintPermitsInfo()
                {
                    BarcodeNumber = string.Empty,
                    VisitorName = string.Empty,
                    VisitMission = string.Empty,
                    AddresseeName = string.Empty,
                    InputTime = string.Empty,
                    PrintTime = string.Empty,
                    PrintUserName = string.Empty,
                    CreateTime = string.Empty,
                    CreateUserName = string.Empty,
                    AgreedTime = string.Empty,
                    AgreedUserName = string.Empty,
                    ApprovedTime = string.Empty,
                    AprovedUserName = string.Empty
                };
                LabelInfo.BarcodeNumber = PermitsManager.GetBarcodeNumber(22, PermitID);
                LabelInfo.VisitorName = PermitsManager.CurrentVisitorName;
                LabelInfo.VisitMission = PermitsManager.CurrentVisitMission;
                if (PermitsManager.CurrentPrintTime != DBNull.Value)
                    LabelInfo.PrintTime = Convert.ToDateTime(PermitsManager.CurrentPrintTime).ToString("dd.MM.yyyy HH:mm");
                LabelInfo.PrintUserName = PermitsManager.CurrentPrintUserName;
                if (PermitsManager.CurrentCreateTime != DBNull.Value)
                    LabelInfo.CreateTime = Convert.ToDateTime(PermitsManager.CurrentCreateTime).ToString("dd.MM.yyyy HH:mm");
                LabelInfo.CreateUserName = PermitsManager.CurrentCreateUserName;
                if (PermitsManager.CurrentAgreedTime != DBNull.Value)
                    LabelInfo.AgreedTime = Convert.ToDateTime(PermitsManager.CurrentAgreedTime).ToString("dd.MM.yyyy HH:mm");
                LabelInfo.AgreedUserName = PermitsManager.CurrentAgreedUserName;

                PrintPermitsManager.AddLabelInfo(ref LabelInfo);
                PrintDialog.Document = PrintPermitsManager.PD;

                PrintPermitsManager.Print();

                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Печать данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                PermitsManager.SavePermits();
                UpdatePermitDates();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void dgvPermits_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            bool OutputEnable = false;
            bool OutputDone = false;
            if (grid.Rows[e.RowIndex].Cells["OutputEnable"].Value != DBNull.Value)
                OutputEnable = Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["OutputEnable"].Value);
            if (grid.Rows[e.RowIndex].Cells["OutputDone"].Value != DBNull.Value)
                OutputDone = Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["OutputDone"].Value);

            if (OutputEnable && !OutputDone)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Green;
            }
            if (!OutputEnable)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }
    }
}
