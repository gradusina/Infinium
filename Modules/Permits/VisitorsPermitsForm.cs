using Infinium.Modules.Permits;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class VisitorsPermitsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        const int iAdminRole = 70;
        const int iAgreedRole = 71;
        const int iApprovedRole = 72;

        bool bC;

        bool bSearchVisitor = false;
        string SearchVisitorName = string.Empty;
        bool bSearchAddressee = false;
        string SearchAddresseeName = string.Empty;

        int FormEvent = 0;

        Form TopForm;
        LightStartForm LightStartForm;
        NewPermit NewPermit;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        RoleTypes RoleType = RoleTypes.OrdinaryRole;
        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1,
            AgreedRole = 2,
            ApprovedRole = 3
        }

        ScanTypes ScanType = ScanTypes.Input;
        public enum ScanTypes
        {
            Input = 0,
            Output = 1,
            Dispatch = 2,
            Unload = 3,
            Machines = 4
        }

        PrintVisitorPermits PrintPermitsManager;
        VisitorsPermits PermitsManager;

        public VisitorsPermitsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            //UpdatePermitDates();

            while (!SplashForm.bCreated) ;
        }

        private void VisitorsPermitsForm_Shown(object sender, EventArgs e)
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
                        LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        LightStartForm.HideForm(this);
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

            PermitsManager = new VisitorsPermits();
            PermitsManager.Initialize();
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
            PermitsManager.CreateFilterTable(RoleType == RoleTypes.AdminRole || RoleType == RoleTypes.ApprovedRole);

            dgvPermitsDatesSetting();
            dgvPermitsSettings();

            InfiniumDocumentsMenu.ItemsDataTable = PermitsManager.DtFilterMenu;
            InfiniumDocumentsMenu.InitializeItems();
            InfiniumDocumentsMenu.Selected = 0;
            dgvPermits.SelectionChanged -= dgvPermits_SelectionChanged;
            dgvPermitsDates.SelectionChanged -= dgvPermitsDates_SelectionChanged;

            NewPermit = new Modules.Permits.NewPermit();
            PrintPermitsManager = new PrintVisitorPermits();
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
            //dgvPermits.Columns.Add(PermitsManager.CreateUserColumn);
            if (dgvPermits.Columns.Contains("AddresseeID"))
                dgvPermits.Columns["AddresseeID"].Visible = false;
            if (dgvPermits.Columns.Contains("AddresseeName"))
                dgvPermits.Columns["AddresseeName"].Visible = false;
            if (dgvPermits.Columns.Contains("InputDeniedTime"))
                dgvPermits.Columns["InputDeniedTime"].Visible = false;
            if (dgvPermits.Columns.Contains("InputDeniedUserID"))
                dgvPermits.Columns["InputDeniedUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("InputTime"))
                dgvPermits.Columns["InputTime"].Visible = false;
            if (dgvPermits.Columns.Contains("OutputAllowedTime"))
                dgvPermits.Columns["OutputAllowedTime"].Visible = false;
            if (dgvPermits.Columns.Contains("OutputAllowedUserID"))
                dgvPermits.Columns["OutputAllowedUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("OutputDeniedTime"))
                dgvPermits.Columns["OutputDeniedTime"].Visible = false;
            if (dgvPermits.Columns.Contains("OutputDeniedUserID"))
                dgvPermits.Columns["OutputDeniedUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("OutputTime"))
                dgvPermits.Columns["OutputTime"].Visible = false;
            if (dgvPermits.Columns.Contains("CreateTime"))
                dgvPermits.Columns["CreateTime"].Visible = false;
            if (dgvPermits.Columns.Contains("CreateUserID"))
                dgvPermits.Columns["CreateUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("PrintTime"))
                dgvPermits.Columns["PrintTime"].Visible = false;
            if (dgvPermits.Columns.Contains("PrintUserID"))
                dgvPermits.Columns["PrintUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("AgreedTime"))
                dgvPermits.Columns["AgreedTime"].Visible = false;
            if (dgvPermits.Columns.Contains("AgreedUserID"))
                dgvPermits.Columns["AgreedUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("ApprovedTime"))
                dgvPermits.Columns["ApprovedTime"].Visible = false;
            if (dgvPermits.Columns.Contains("ApprovedUserID"))
                dgvPermits.Columns["ApprovedUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("DeleteUserID"))
                dgvPermits.Columns["DeleteUserID"].Visible = false;
            if (dgvPermits.Columns.Contains("PermitEnable"))
                dgvPermits.Columns["PermitEnable"].Visible = false;
            if (dgvPermits.Columns.Contains("DeleteTime"))
                dgvPermits.Columns["DeleteTime"].Visible = false;
            if (dgvPermits.Columns.Contains("Overdued"))
                dgvPermits.Columns["Overdued"].Visible = false;

            foreach (DataGridViewColumn column in dgvPermits.Columns)
            {
                //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //column.MinimumWidth = 60;
            }

            dgvPermits.Columns["Validity"].DefaultCellStyle.Format = "dd.MM.yyyy";
            dgvPermits.Columns["InputDeniedTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["InputTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["OutputDeniedTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["OutputTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["CreateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["PrintTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["AgreedTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPermits.Columns["ApprovedTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvPermits.Columns["VisitorPermitID"].HeaderText = "№";
            dgvPermits.Columns["VisitorName"].HeaderText = "Кому выдан";
            dgvPermits.Columns["VisitMission"].HeaderText = "Цель визита";
            dgvPermits.Columns["AddresseeName"].HeaderText = "К кому";
            dgvPermits.Columns["Validity"].HeaderText = "Срок\nдействия";

            dgvPermits.Columns["InputEnable"].HeaderText = "Вход\nразрешен";
            dgvPermits.Columns["InputDeniedTime"].HeaderText = "Вход\nзапрещен";
            //dgvPermits.Columns["InputDeniedUserName"].HeaderText = "Вход\nзапретил";
            dgvPermits.Columns["InputDone"].HeaderText = "Вход\nпроизведен";
            dgvPermits.Columns["InputTime"].HeaderText = "Вход\nпроизведен";

            dgvPermits.Columns["OutputEnable"].HeaderText = "Выход\nразрешен";
            dgvPermits.Columns["OutputDeniedTime"].HeaderText = "Выход\nзапрещен";
            //dgvPermits.Columns["OutputDeniedUserName"].HeaderText = "Выход\nзапретил";
            dgvPermits.Columns["OutputDone"].HeaderText = "Выход\nпроизведен";
            dgvPermits.Columns["OutputTime"].HeaderText = "Выход\nпроизведен";

            dgvPermits.Columns["CreateTime"].HeaderText = "Дата\nрегистрации";
            //dgvPermits.Columns["CreateUserName"].HeaderText = "Кто\nрегистрировал";
            dgvPermits.Columns["PrintTime"].HeaderText = "Дата\nпечати";
            //dgvPermits.Columns["PrintUserName"].HeaderText = "Кто\nраспечатал";
            dgvPermits.Columns["AgreedTime"].HeaderText = "Дата\nсогласования";
            //dgvPermits.Columns["AgreedUserName"].HeaderText = "Кто\nсогласовал";
            dgvPermits.Columns["ApprovedTime"].HeaderText = "Дата\nутверждения";
            //dgvPermits.Columns["AprovedUserName"].HeaderText = "Кто\nутвердил";

            dgvPermits.Columns["VisitorPermitID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPermits.Columns["VisitorPermitID"].Width = 60;
            dgvPermits.Columns["InputEnable"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPermits.Columns["InputEnable"].Width = 110;
            dgvPermits.Columns["InputDone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPermits.Columns["InputDone"].Width = 110;
            dgvPermits.Columns["OutputEnable"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPermits.Columns["OutputEnable"].Width = 110;
            dgvPermits.Columns["OutputDone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPermits.Columns["OutputDone"].Width = 110;
            dgvPermits.Columns["Validity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPermits.Columns["Validity"].MinimumWidth = 100;
            //dgvPermits.Columns["CreateUserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvPermits.Columns["CreateUserName"].MinimumWidth = 100;
            //dgvPermits.Columns["VisitorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvPermits.Columns["VisitorName"].MinimumWidth = 100;
            //dgvPermits.Columns["VisitMission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvPermits.Columns["VisitMission"].MinimumWidth = 100;

            dgvPermits.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvPermits.Columns["VisitorPermitID"].DisplayIndex = DisplayIndex++;
            //dgvPermits.Columns["CreateTime"].DisplayIndex = DisplayIndex++;
            //dgvPermits.Columns["CreateUserColumn"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["VisitorName"].DisplayIndex = DisplayIndex++;
            dgvPermits.Columns["VisitMission"].DisplayIndex = DisplayIndex++;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void PermitsForm_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel5.Visible = false;
            flowLayoutPanel7.Visible = false;
            flowLayoutPanel8.Visible = false;
            flowLayoutPanel9.Visible = false;
            flowLayoutPanel10.Visible = false;
            flowLayoutPanel11.Visible = false;
            flowLayoutPanel12.Visible = false;
            flowLayoutPanel13.Visible = false;
            flowLayoutPanel14.Visible = false;
            flowLayoutPanel15.Visible = false;
            flowLayoutPanel16.Visible = false;
            pnlScanInput.Visible = false;

            dgvPermits.SelectionChanged += new EventHandler(dgvPermits_SelectionChanged);
            dgvPermitsDates.SelectionChanged += new EventHandler(dgvPermitsDates_SelectionChanged);
            GetPermitInformation();
            pnlScanInput.SendToBack();

            //Создать пропуск
            btnNewPermit.Visible = true;
            kryptonContextMenuSeparator2.Visible = true;
            //Согласовать пропуск
            btnAgreePermit.Visible = true;
            kryptonContextMenuSeparator1.Visible = true;
            //Утвердить пропуск
            btnApprovePermit.Visible = true;
            kryptonContextMenuSeparator3.Visible = true;
            //Распечатать пропуск
            btnPrintPermit.Visible = true;
            kryptonContextMenuSeparator4.Visible = true;
            //Удалить пропуск
            btnRemovePermit.Visible = true;
            //Разрешить вход
            btnAllowInput.Visible = true;
            kryptonContextMenuSeparator7.Visible = true;
            //Запретить вход
            btnDenyInput.Visible = true;
            kryptonContextMenuSeparator5.Visible = true;
            //Разрешить выход
            btnAllowOutput.Visible = true;
            kryptonContextMenuSeparator8.Visible = true;
            //Запретить выход
            btnDenyOutput.Visible = true;
            kryptonContextMenuSeparator6.Visible = true;
            /*if (RoleType == RoleTypes.AdminRole)
            {
                //Создать пропуск
                btnNewPermit.Visible = true;
                kryptonContextMenuSeparator2.Visible = true;
                //Согласовать пропуск
                btnAgreePermit.Visible = true;
                kryptonContextMenuSeparator1.Visible = true;
                //Утвердить пропуск
                btnApprovePermit.Visible = true;
                kryptonContextMenuSeparator3.Visible = true;
                //Распечатать пропуск
                btnPrintPermit.Visible = true;
                kryptonContextMenuSeparator4.Visible = true;
                //Удалить пропуск
                btnRemovePermit.Visible = true;
                //Разрешить вход
                btnAllowInput.Visible = true;
                kryptonContextMenuSeparator7.Visible = true;
                //Запретить вход
                btnDenyInput.Visible = true;
                kryptonContextMenuSeparator5.Visible = true;
                //Разрешить выход
                btnAllowOutput.Visible = true;
                kryptonContextMenuSeparator8.Visible = true;
                //Запретить выход
                btnDenyOutput.Visible = true;
                kryptonContextMenuSeparator6.Visible = true;
            }
            if (RoleType == RoleTypes.AgreedRole)
            {
                //Создать пропуск
                btnNewPermit.Visible = true;
                kryptonContextMenuSeparator2.Visible = true;
                //Согласовать пропуск
                btnAgreePermit.Visible = true;
                kryptonContextMenuSeparator1.Visible = true;
                //Распечатать пропуск
                btnPrintPermit.Visible = true;
                kryptonContextMenuSeparator4.Visible = true;
                //Разрешить вход
                btnAllowInput.Visible = true;
                kryptonContextMenuSeparator7.Visible = true;
                //Разрешить выход
                btnAllowOutput.Visible = true;
                kryptonContextMenuSeparator8.Visible = true;
            }
            if (RoleType == RoleTypes.ApprovedRole)
            {
                //Создать пропуск
                btnNewPermit.Visible = true;
                kryptonContextMenuSeparator2.Visible = true;
                //Согласовать пропуск
                btnAgreePermit.Visible = true;
                kryptonContextMenuSeparator1.Visible = true;
                //Утвердить пропуск
                btnApprovePermit.Visible = true;
                kryptonContextMenuSeparator3.Visible = true;
                //Распечатать пропуск
                btnPrintPermit.Visible = true;
                kryptonContextMenuSeparator4.Visible = true;
                //Разрешить вход
                btnAllowInput.Visible = true;
                kryptonContextMenuSeparator7.Visible = true;
                //Разрешить выход
                btnAllowOutput.Visible = true;
                kryptonContextMenuSeparator8.Visible = true;
            }
            if (RoleType == RoleTypes.OrdinaryRole)
            {

            }*/
        }

        private void CheckPermitsColumns(ref ComponentFactory.Krypton.Toolkit.KryptonDataGridView Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }

            if (Grid.Columns.Contains("PermitEnable"))
                Grid.Columns["PermitEnable"].Visible = false;
        }

        private void btnNewPermit_Click(object sender, EventArgs e)
        {
            NewPermit = new Modules.Permits.NewPermit();
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewVisitorPermitForm NewPermitForm = new NewVisitorPermitForm(this, PermitsManager, ref NewPermit);
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

            PermitsManager.NewPermit(NewPermit.VisitorName, NewPermit.VisitMission, NewPermit.AddresseeID, NewPermit.AddresseeName, NewPermit.Validity);

            PermitsManager.SendApproveNotifications(iApprovedRole);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void UpdatePermitDates()
        {
            PermitsManager.ClearPermitDates();
            PermitsManager.ClearPermits();
            DateTime Date = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            object VisitDateTime = PermitsManager.CurrentVisitDateTime;
            int VisitorPermitID = 0;
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value != DBNull.Value)
                VisitorPermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);


            if (bSearchVisitor || bSearchAddressee)
            {
                PermitsManager.UpdatePermitsDatesByPerson(Date, bSearchVisitor, SearchVisitorName, bSearchAddressee, SearchAddresseeName);
            }
            else
            {
                if (InfiniumDocumentsMenu.SelectedName == "Все пропуска")
                    PermitsManager.UpdatePermitsDates(Date, false, false, false, false, false, false,
                        true, btnNew.Checked, btnActive.Checked, btnOverdued.Checked, btnClose.Checked, false);
                if (InfiniumDocumentsMenu.SelectedName == "Мои")
                    PermitsManager.UpdatePermitsDates(Date, cbOnlyActivePermits.Checked, true, btnImCreated.Checked, btnImAddressee.Checked, btnImAgreed.Checked, btnImApproved.Checked,
                        false, false, false, false, false, false);
                if (InfiniumDocumentsMenu.SelectedName == "На утверждение")
                    PermitsManager.UpdatePermitsDates(Date, false, false, false, false, false, false, false, false, false, false, false, true);
            }

            if (VisitDateTime != DBNull.Value)
            {
                PermitsManager.MoveToVisitDateTime(Convert.ToDateTime(VisitDateTime));
                PermitsManager.MoveToPermit(VisitorPermitID);
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

        private void btnRemovePermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            if (RoleType != RoleTypes.AdminRole && RoleType != RoleTypes.ApprovedRole)
                return;

            if (Convert.ToBoolean(dgvPermits.SelectedRows[0].Cells["OutputDone"].Value))
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Пропуск будет удален. Продолжить?",
                    "Удаление пропуска");

            if (!OKCancel)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PermitsManager.RemovePermit(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Пропуск удален", 1700);
        }

        private void dgvPermits_SelectionChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel5.Visible = false;
            flowLayoutPanel7.Visible = false;
            flowLayoutPanel8.Visible = false;
            flowLayoutPanel9.Visible = false;
            flowLayoutPanel10.Visible = false;
            flowLayoutPanel11.Visible = false;
            flowLayoutPanel12.Visible = false;
            flowLayoutPanel13.Visible = false;
            flowLayoutPanel14.Visible = false;
            flowLayoutPanel15.Visible = false;
            flowLayoutPanel16.Visible = false;
            if (dgvPermits.SelectedRows.Count == 0)
                return;

            if (PermitsManager.CurrentInputEnable)
            {
                btnAllowInput.Enabled = false;

                if (PermitsManager.CurrentInputDone)
                    btnDenyInput.Enabled = false;
            }
            else
            {
                btnAllowInput.Enabled = true;
                btnDenyInput.Enabled = false;
            }
            if (PermitsManager.CurrentOutputEnable)
            {
                btnAllowOutput.Enabled = false;
                btnDenyOutput.Enabled = true;

                if (PermitsManager.CurrentOutputDone)
                    btnDenyOutput.Enabled = false;
            }
            else
            {
                btnAllowOutput.Enabled = true;
                btnDenyOutput.Enabled = false;
            }
            if (PermitsManager.CurrentAgreedTime != DBNull.Value)
            {
                btnAgreePermit.Enabled = false;
            }
            else
            {
                btnAgreePermit.Enabled = true;
            }
            if (PermitsManager.CurrentApprovedTime != DBNull.Value)
            {
                btnApprovePermit.Enabled = false;
            }
            else
            {
                btnApprovePermit.Enabled = true;
            }
            int AddresseeID = 0;
            int InputDeniedUserID = 0;
            int OutputAllowedUserID = 0;
            int OutputDeniedUserID = 0;
            int CreateUserID = 0;
            int PrintUserID = 0;
            int AgreedUserID = 0;
            int ApprovedUserID = 0;
            int DeleteUserID = 0;
            string AddresseeName = string.Empty;

            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["AddresseeID"].Value != DBNull.Value)
                AddresseeID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["AddresseeID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["InputDeniedUserID"].Value != DBNull.Value)
                InputDeniedUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["InputDeniedUserID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["OutputAllowedUserID"].Value != DBNull.Value)
                OutputAllowedUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["OutputAllowedUserID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["OutputDeniedUserID"].Value != DBNull.Value)
                OutputDeniedUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["OutputDeniedUserID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["CreateUserID"].Value != DBNull.Value)
                CreateUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["CreateUserID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["PrintUserID"].Value != DBNull.Value)
                PrintUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["PrintUserID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["AgreedUserID"].Value != DBNull.Value)
                AgreedUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["AgreedUserID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["ApprovedUserID"].Value != DBNull.Value)
                ApprovedUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["ApprovedUserID"].Value);
            if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["DeleteUserID"].Value != DBNull.Value)
                DeleteUserID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["DeleteUserID"].Value);
            if (AddresseeID == 0)
            {
                if (dgvPermits.SelectedRows.Count > 0 && dgvPermits.SelectedRows[0].Cells["AddresseeName"].Value != DBNull.Value)
                    AddresseeName = dgvPermits.SelectedRows[0].Cells["AddresseeName"].Value.ToString();
            }

            PermitsManager.GetUsersInformation(AddresseeID, AddresseeName, InputDeniedUserID, OutputAllowedUserID, OutputDeniedUserID, CreateUserID, PrintUserID, AgreedUserID, ApprovedUserID, DeleteUserID);
            GetPermitInformation();
        }

        private void cbxMonths_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbxYears_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvPermitsDates_SelectionChanged(object sender, EventArgs e)
        {
            if (PermitsManager == null)
                return;

            if (dgvPermitsDates.SelectedRows.Count == 0)
            {
                flowLayoutPanel1.Visible = false;
                flowLayoutPanel5.Visible = false;
                flowLayoutPanel7.Visible = false;
                flowLayoutPanel8.Visible = false;
                flowLayoutPanel9.Visible = false;
                flowLayoutPanel10.Visible = false;
                flowLayoutPanel11.Visible = false;
                flowLayoutPanel12.Visible = false;
                flowLayoutPanel13.Visible = false;
                flowLayoutPanel14.Visible = false;
                flowLayoutPanel15.Visible = false;
                flowLayoutPanel16.Visible = false;
                PermitsManager.ClearPermits();
                return;
            }

            object VisitDateTime = PermitsManager.CurrentVisitDateTime;
            if (VisitDateTime != DBNull.Value)
            {

                //if (NeedSplash)
                //{
                //    Thread T = new Thread(delegate()
                //    {
                //        SplashWindow.CreateCoverSplash(pnlMainContent.Top, pnlMainContent.Left,
                //                                       pnlMainContent.Height, pnlMainContent.Width);
                //    });
                //    T.Start();

                //    while (!SplashWindow.bSmallCreated) ;

                //    PermitsManager.FilterPermitsByValidity(Convert.ToDateTime(VisitDateTime));
                //    //CheckPermitsColumns(ref dgvPermits);
                //    bC = true;
                //}
                //else
                //{
                //    PermitsManager.FilterPermitsByValidity(Convert.ToDateTime(VisitDateTime));
                //    //CheckPermitsColumns(ref dgvPermits);
                //}
                UpdatePermits();
            }
        }

        private void GetPermitInformation()
        {
            if (PermitsManager.CurrentVisitorName.Length > 0)
            {
                lbVisitorName.Text = PermitsManager.CurrentVisitorName;
                flowLayoutPanel5.Visible = true;
            }
            else
            {
                flowLayoutPanel5.Visible = false;
            }
            if (PermitsManager.CurrentVisitMission.Length > 0)
            {
                lbVisitMission.Text = PermitsManager.CurrentVisitMission;
                flowLayoutPanel7.Visible = true;
            }
            else
            {
                flowLayoutPanel7.Visible = false;
            }
            if (PermitsManager.CurrentAddresseeName.Length > 0)
            {
                lbAddresseeName.Text = PermitsManager.CurrentAddresseeName;
                flowLayoutPanel8.Visible = true;
            }
            else
            {
                flowLayoutPanel8.Visible = false;
            }
            if (PermitsManager.CurrentValidity != DBNull.Value)
            {
                lbValidity.Text = Convert.ToDateTime(PermitsManager.CurrentValidity).ToString("dd.MM.yyyy");
                flowLayoutPanel9.Visible = true;
            }
            else
            {
                flowLayoutPanel9.Visible = false;
            }
            if (PermitsManager.CurrentInputDeniedTime != DBNull.Value)
            {
                lbInputDeniedTime.Text = Convert.ToDateTime(PermitsManager.CurrentInputDeniedTime).ToString("dd.MM.yyyy HH:mm");
                lbInputDeniedUserName.Text = PermitsManager.CurrentInputDeniedUserName;
                flowLayoutPanel10.Visible = true;
            }
            else
            {
                flowLayoutPanel10.Visible = false;
            }
            if (PermitsManager.CurrentInputTime != DBNull.Value)
            {
                lbInputTime.Text = Convert.ToDateTime(PermitsManager.CurrentInputTime).ToString("dd.MM.yyyy HH:mm");
                flowLayoutPanel11.Visible = true;
            }
            else
            {
                flowLayoutPanel11.Visible = false;
            }
            if (PermitsManager.CurrentOutputAllowedTime != DBNull.Value)
            {
                lbOutputAllowedTime.Text = Convert.ToDateTime(PermitsManager.CurrentOutputAllowedTime).ToString("dd.MM.yyyy HH:mm");
                lbOutputAllowedUserName.Text = PermitsManager.CurrentOutputAllowedUserName;
                flowLayoutPanel1.Visible = true;
            }
            else
            {
                flowLayoutPanel1.Visible = false;
            }
            if (PermitsManager.CurrentOutputDeniedTime != DBNull.Value)
            {
                lbOutputDeniedTime.Text = Convert.ToDateTime(PermitsManager.CurrentOutputDeniedTime).ToString("dd.MM.yyyy HH:mm");
                lbOutputDeniedUserName.Text = PermitsManager.CurrentOutputDeniedUserName;
                flowLayoutPanel12.Visible = true;
            }
            else
            {
                flowLayoutPanel12.Visible = false;
            }
            if (PermitsManager.CurrentOutputTime != DBNull.Value)
            {
                lbOutputTime.Text = Convert.ToDateTime(PermitsManager.CurrentOutputTime).ToString("dd.MM.yyyy HH:mm");
                flowLayoutPanel13.Visible = true;
            }
            else
            {
                flowLayoutPanel13.Visible = false;
            }
            if (PermitsManager.CurrentCreateTime != DBNull.Value)
            {
                lbCreateTime.Text = Convert.ToDateTime(PermitsManager.CurrentCreateTime).ToString("dd.MM.yyyy HH:mm");
                lbCreateUserName.Text = PermitsManager.CurrentCreateUserName;
                flowLayoutPanel14.Visible = true;
            }
            else
            {
                flowLayoutPanel14.Visible = false;
            }
            if (PermitsManager.CurrentAgreedTime != DBNull.Value)
            {
                lbAgreedTime.Text = Convert.ToDateTime(PermitsManager.CurrentAgreedTime).ToString("dd.MM.yyyy HH:mm");
                lbAgreedUserName.Text = PermitsManager.CurrentAgreedUserName;
                flowLayoutPanel15.Visible = true;
            }
            else
            {
                flowLayoutPanel15.Visible = false;
            }
            if (PermitsManager.CurrentApprovedTime != DBNull.Value)
            {
                lbApprovedTime.Text = Convert.ToDateTime(PermitsManager.CurrentApprovedTime).ToString("dd.MM.yyyy HH:mm");
                lbAprovedUserName.Text = PermitsManager.CurrentAprovedUserName;
                flowLayoutPanel16.Visible = true;
            }
            else
            {
                flowLayoutPanel16.Visible = false;
            }
        }

        private void btnOpenMenu_Click(object sender, EventArgs e)
        {
            //if (RoleType == RoleTypes.OrdinaryRole)
            //    return;
            kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;
            }
        }

        private void btnAllowOutput_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PermitsManager.AllowOutput(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Выход разрешен", 1700);
        }

        private void btnDenyOutput_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PermitsManager.DenyOutput(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Выход запрещен", 1700);
        }

        private void btnAllowInput_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PermitsManager.AllowInput(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Вход разрешен", 1700);
        }

        private void btnDenyInput_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PermitsManager.DenyInput(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Вход запрещен", 1700);
        }

        private void btnPrintPermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

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
                LabelInfo.BarcodeNumber = PermitsManager.GetBarcodeNumber(13, PermitID);
                LabelInfo.VisitorName = PermitsManager.CurrentVisitorName;
                LabelInfo.VisitMission = PermitsManager.CurrentVisitMission;
                LabelInfo.AddresseeName = PermitsManager.CurrentAddresseeName;
                if (PermitsManager.CurrentInputTime != DBNull.Value)
                    LabelInfo.InputTime = Convert.ToDateTime(PermitsManager.CurrentInputTime).ToString("dd.MM.yyyy HH:mm");
                if (PermitsManager.CurrentPrintTime != DBNull.Value)
                    LabelInfo.PrintTime = Convert.ToDateTime(PermitsManager.CurrentPrintTime).ToString("dd.MM.yyyy HH:mm");
                LabelInfo.PrintUserName = PermitsManager.CurrentPrintUserName;
                if (PermitsManager.CurrentCreateTime != DBNull.Value)
                    LabelInfo.CreateTime = Convert.ToDateTime(PermitsManager.CurrentCreateTime).ToString("dd.MM.yyyy HH:mm");
                LabelInfo.CreateUserName = PermitsManager.CurrentCreateUserName;
                if (PermitsManager.CurrentAgreedTime != DBNull.Value)
                    LabelInfo.AgreedTime = Convert.ToDateTime(PermitsManager.CurrentAgreedTime).ToString("dd.MM.yyyy HH:mm");
                LabelInfo.AgreedUserName = PermitsManager.CurrentAgreedUserName;
                if (PermitsManager.CurrentApprovedTime != DBNull.Value)
                    LabelInfo.ApprovedTime = Convert.ToDateTime(PermitsManager.CurrentApprovedTime).ToString("dd.MM.yyyy HH:mm");
                LabelInfo.AprovedUserName = PermitsManager.CurrentAprovedUserName;

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

        void dgvPermits_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            if (dgvPermits.RowCount == 0)
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                e.Graphics.DrawString("Нет данных", new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular), new SolidBrush(Color.FromArgb(140, 140, 140)),
                            (this.ClientRectangle.Width - e.Graphics.MeasureString("Нет данных", new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular)).Width) / 2 + 4,
                            (this.ClientRectangle.Height - e.Graphics.MeasureString("Нет данных", new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular)).Height) / 2);
            }
        }

        private void btnAprovePermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PermitsManager.AprovePermit(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Пропуск утвержден", 1700);
        }

        private void btnAgreePermit_Click(object sender, EventArgs e)
        {
            if (dgvPermits.SelectedRows.Count == 0 || dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value == DBNull.Value)
                return;

            int PermitID = Convert.ToInt32(dgvPermits.SelectedRows[0].Cells["VisitorPermitID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PermitsManager.AgreePermit(PermitID);
            PermitsManager.SavePermits();
            UpdatePermitDates();
            PermitsManager.SendApproveNotifications(iApprovedRole);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Пропуск согласован", 1700);
        }

        private void dgvPermits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
                btnNewPermit_Click(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
                btnRemovePermit_Click(null, null);
        }

        private void InfiniumDocumentsMenu_SelectedChanged(object sender, string Name, int index)
        {
            bSearchVisitor = false;
            bSearchAddressee = false;
            UpdatePermitDates();
            if (Name == "Все пропуска")
            {
                pnlAllPermits.BringToFront();
            }
            if (Name == "Мои")
            {
                pnlMyPermits.BringToFront();
            }
            if (Name == "На утверждение")
            {
                pnlForApproval.BringToFront();
            }
            if (Name == "Поиск")
            {
                pnlSearchPermits.BringToFront();
            }

        }

        private void UpdatePermits()
        {
            object VisitDateTime = PermitsManager.CurrentVisitDateTime;
            if (bSearchVisitor || bSearchAddressee)
            {
                PermitsManager.FilterPermitsByPerson(Convert.ToDateTime(VisitDateTime), bSearchVisitor, SearchVisitorName, bSearchAddressee, SearchAddresseeName);
            }
            else
            {
                if (InfiniumDocumentsMenu.SelectedName == "Все пропуска")
                    PermitsManager.FilterPermits(Convert.ToDateTime(VisitDateTime), false, false, false, false, false, false,
                        true, btnNew.Checked, btnActive.Checked, btnOverdued.Checked, btnClose.Checked, false);
                if (InfiniumDocumentsMenu.SelectedName == "Мои")
                    PermitsManager.FilterPermits(Convert.ToDateTime(VisitDateTime), cbOnlyActivePermits.Checked, true, btnImCreated.Checked, btnImAddressee.Checked, btnImAgreed.Checked, btnImApproved.Checked,
                        false, false, false, false, false, false);
                if (InfiniumDocumentsMenu.SelectedName == "На утверждение")
                    PermitsManager.FilterPermits(Convert.ToDateTime(VisitDateTime), false, false, false, false, false, false, false, false, false, false, false, true);
            }
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            //panel10.Enabled = false;
            btnNew.Checked = false;
            btnOverdued.Checked = false;
            btnClose.Checked = false;
            UpdatePermitDates();
        }

        private void btnOverdued_Click(object sender, EventArgs e)
        {
            //panel10.Enabled = false;
            btnNew.Checked = false;
            btnActive.Checked = false;
            btnClose.Checked = false;
            UpdatePermitDates();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //panel10.Enabled = true;
            btnNew.Checked = false;
            btnActive.Checked = false;
            btnOverdued.Checked = false;
            UpdatePermitDates();
        }

        private void btnImCreated_Click(object sender, EventArgs e)
        {
            btnImAddressee.Checked = false;
            btnImAgreed.Checked = false;
            btnImApproved.Checked = false;
            UpdatePermitDates();
        }

        private void btnImAddressee_Click(object sender, EventArgs e)
        {
            btnImAgreed.Checked = false;
            btnImApproved.Checked = false;
            btnImCreated.Checked = false;
            UpdatePermitDates();
        }

        private void btnImAgreed_Click(object sender, EventArgs e)
        {
            btnImAddressee.Checked = false;
            btnImApproved.Checked = false;
            btnImCreated.Checked = false;
            UpdatePermitDates();
        }

        private void btnImApproved_Click(object sender, EventArgs e)
        {
            btnImAddressee.Checked = false;
            btnImAgreed.Checked = false;
            btnImCreated.Checked = false;
            UpdatePermitDates();
        }

        private void cbOnlyActivePermits_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePermitDates();
        }

        private void btnSearch1_Click(object sender, EventArgs e)
        {
            bSearchVisitor = true;
            SearchVisitorName = tbSearchVisitor.Text;
            UpdatePermitDates();
            tbSearchAddressee.Text = string.Empty;
            tbSearchVisitor.Text = string.Empty;
        }

        private void btnSearch2_Click(object sender, EventArgs e)
        {
            bSearchAddressee = true;
            SearchAddresseeName = tbSearchAddressee.Text;
            UpdatePermitDates();
            tbSearchAddressee.Text = string.Empty;
            tbSearchVisitor.Text = string.Empty;
        }

        private void btnBeginScanInput_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируется вход";
            label37.ForeColor = Color.FromArgb(82, 169, 24);
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            CheckTimer.Enabled = true;
            ScanType = ScanTypes.Input;
            btnOpenMenu.Visible = false;
            btnBeginScanInput.Visible = false;
            btnBeginScanOutput.Visible = false;
            btnBeginScanInput1.Visible = true;
            btnBeginScanOutput1.Visible = true;
            btnBeginScanDispatch.Visible = false;
            btnBeginScanDispatch1.Visible = true;
            btnBackToPermits.Visible = true;
            pnlScanInput.BringToFront();
            pnlScanInput.Visible = true;
        }

        private void btnBeginScanOutput_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируется выход";
            label37.ForeColor = Color.FromArgb(240, 0, 0);
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            CheckTimer.Enabled = true;
            ScanType = ScanTypes.Output;
            btnOpenMenu.Visible = false;
            btnBeginScanInput.Visible = false;
            btnBeginScanOutput.Visible = false;
            btnBeginScanInput1.Visible = true;
            btnBeginScanOutput1.Visible = true;
            btnBeginScanDispatch.Visible = false;
            btnBeginScanDispatch1.Visible = true;
            btnBackToPermits.Visible = true;
            pnlScanInput.BringToFront();
            pnlScanInput.Visible = true;
        }

        private void btnBackToPermits_Click(object sender, EventArgs e)
        {
            label37.Visible = false;
            CheckTimer.Enabled = false;

            //if (RoleType != RoleTypes.OrdinaryRole)
            btnOpenMenu.Visible = true;
            btnBeginScanInput.Visible = true;
            btnBeginScanOutput.Visible = true;
            btnBeginScanInput1.Visible = false;
            btnBeginScanOutput1.Visible = false;
            btnBeginScanDispatch.Visible = true;
            btnBeginScanDispatch1.Visible = false;
            btnBackToPermits.Visible = false;
            pnlScanInput.SendToBack();
            pnlScanInput.Visible = false;
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                flowLayoutPanel2.Visible = false;
                panel2.Visible = false;
                ErrorPackLabel.Visible = false;
                BarcodeLabel.Text = "";
                CheckPicture.Visible = false;
                string Message = "Неверный штрихкод";

                if (BarcodeTextBox.Text.Length < 12)
                {
                    BarcodeTextBox.Clear();
                    ErrorPackLabel.Text = Message;
                    ErrorPackLabel.Visible = true;
                    ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    CheckPicture.Visible = true;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    CheckPicture.Image = Properties.Resources.cancel;
                    InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                    return;
                }

                if (string.Equals(BarcodeTextBox.Text, "000000000000"))
                {
                    return;
                }

                BarcodeLabel.Text = BarcodeTextBox.Text;

                BarcodeTextBox.Clear();

                string Prefix = BarcodeLabel.Text.Substring(0, 3);

                if (ScanType == ScanTypes.Input || ScanType == ScanTypes.Output)
                {
                    int VisitorPermitID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));

                    if (!PermitsManager.IsPermitExist(VisitorPermitID))
                    {
                        Message = "Пропуска не существует";
                        ErrorPackLabel.Text = Message;
                        ErrorPackLabel.Visible = true;
                        ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Visible = true;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Image = Properties.Resources.cancel;
                        InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                        return;
                    }

                    VisitorsPermits.ScanedPermitsInfo Struct = new VisitorsPermits.ScanedPermitsInfo();

                    if (ScanType == ScanTypes.Input)
                    {
                        Struct = PermitsManager.InputDone(VisitorPermitID);
                        GetScanningPermitInformation(Struct);
                        if (!Struct.InputEnable)
                        {
                            Message = "Вход запрещен. Требуется утверждение";
                            ErrorPackLabel.Text = Message;
                            ErrorPackLabel.Visible = true;
                            ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                            CheckPicture.Visible = true;
                            BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                            CheckPicture.Image = Properties.Resources.cancel;
                            InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                            return;
                        }
                        flowLayoutPanel2.Visible = true;
                        if (Struct.InputDone)
                            Message = "Вход уже был выполнен ранее";
                        else
                        {
                            Message = "Вход выполнен";
                            PermitsManager.ScanVisitorPermit(VisitorPermitID, 0);
                            UpdatePermitDates();
                        }
                        CheckPicture.Visible = true;
                        BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                        CheckPicture.Image = Properties.Resources.OK;
                        ErrorPackLabel.Text = Message;
                        ErrorPackLabel.Visible = true;
                        ErrorPackLabel.ForeColor = Color.FromArgb(82, 169, 24);
                        InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                        return;
                    }
                    if (ScanType == ScanTypes.Output)
                    {
                        Struct = PermitsManager.OutputDone(VisitorPermitID);
                        GetScanningPermitInformation(Struct);
                        if (!Struct.OutputEnable)
                        {
                            Message = "Выход запрещен. Требуется разрешение";
                            ErrorPackLabel.Text = Message;
                            ErrorPackLabel.Visible = true;
                            ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                            CheckPicture.Visible = true;
                            BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                            CheckPicture.Image = Properties.Resources.cancel;
                            InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                            return;
                        }
                        flowLayoutPanel2.Visible = true;
                        if (Struct.OutputDone)
                            Message = "Выход уже был выполнен ранее";
                        else
                        {
                            Message = "Выход выполнен";
                            PermitsManager.ScanVisitorPermit(VisitorPermitID, 1);
                            UpdatePermitDates();
                        }
                        CheckPicture.Visible = true;
                        BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                        CheckPicture.Image = Properties.Resources.OK;
                        ErrorPackLabel.Text = Message;
                        ErrorPackLabel.Visible = true;
                        ErrorPackLabel.ForeColor = Color.FromArgb(82, 169, 24);
                        InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                        return;
                    }
                }
                if (ScanType == ScanTypes.Dispatch)
                {
                    int PermitID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));

                    if (!PermitsManager.IsDispatchPermitExist(PermitID))
                    {
                        Message = "Пропуска не существует";
                        ErrorPackLabel.Text = Message;
                        ErrorPackLabel.Visible = true;
                        ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Visible = true;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Image = Properties.Resources.cancel;
                        InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                        return;
                    }
                    PermitsManager.ScanPermit(PermitID);
                    label38.Text = PermitsManager.GetDispatchInfo(PermitID);
                    panel2.Visible = true;
                    Message = "Отгрузка сканирована";
                    CheckPicture.Visible = true;
                    BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    CheckPicture.Image = Properties.Resources.OK;
                    ErrorPackLabel.Text = Message;
                    ErrorPackLabel.Visible = true;
                    ErrorPackLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                    return;
                }
                if (ScanType == ScanTypes.Unload)
                {
                    int UnloadID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));

                    if (!PermitsManager.IsUnloadExist(UnloadID))
                    {
                        Message = "Пропуска не существует";
                        ErrorPackLabel.Text = Message;
                        ErrorPackLabel.Visible = true;
                        ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Visible = true;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Image = Properties.Resources.cancel;
                        InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                        return;
                    }
                    bool OutObject = false;
                    label38.Text = PermitsManager.GetUnloadInfo(UnloadID, ref OutObject);
                    PermitsManager.ScanUnload(UnloadID, OutObject);
                    panel2.Visible = true;
                    Message = "Пропуск сканирован";
                    CheckPicture.Visible = true;
                    BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    CheckPicture.Image = Properties.Resources.OK;
                    ErrorPackLabel.Text = Message;
                    ErrorPackLabel.Visible = true;
                    ErrorPackLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                    return;
                }

                if (ScanType == ScanTypes.Machines)
                {
                    int MachinePermitID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));

                    if (!PermitsManager.IsMachinesPermitExist(MachinePermitID))
                    {
                        Message = "Пропуска не существует";
                        ErrorPackLabel.Text = Message;
                        ErrorPackLabel.Visible = true;
                        ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Visible = true;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Image = Properties.Resources.cancel;
                        InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                        return;
                    }

                    VisitorsPermits.ScanedMachinesPermitsInfo Struct = new VisitorsPermits.ScanedMachinesPermitsInfo();
                    Struct = PermitsManager.MachinesPermitDone(MachinePermitID);
                    GetScanningMachinesPermitInformation(Struct);
                    if (!Struct.OutputEnable)
                    {
                        Message = "Выезд запрещен. Требуется разрешение";
                        ErrorPackLabel.Text = Message;
                        ErrorPackLabel.Visible = true;
                        ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Visible = true;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        CheckPicture.Image = Properties.Resources.cancel;
                        InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                        return;
                    }
                    flowLayoutPanel2.Visible = true;
                    if (Struct.OutputDone)
                        Message = "Выезд уже был выполнен ранее";
                    else
                    {
                        Message = "Выезд выполнен";
                        PermitsManager.ScanMachinesPermit(MachinePermitID);
                    }
                    CheckPicture.Visible = true;
                    BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    CheckPicture.Image = Properties.Resources.OK;
                    ErrorPackLabel.Text = Message;
                    ErrorPackLabel.Visible = true;
                    ErrorPackLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    InfiniumTips.ShowTip(this, 50, 85, Message, 3700);
                    return;
                }
                ErrorPackLabel.Text = Message;
                ErrorPackLabel.Visible = true;
                ErrorPackLabel.ForeColor = Color.FromArgb(240, 0, 0);
                CheckPicture.Visible = true;
                BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                CheckPicture.Image = Properties.Resources.cancel;
                return;
            }
        }

        private void GetScanningPermitInformation(VisitorsPermits.ScanedPermitsInfo Struct)
        {
            if (Struct.VisitorName.Length > 0)
            {
                label6.Text = Struct.VisitorName;
                flowLayoutPanel3.Visible = true;
            }
            else
            {
                flowLayoutPanel3.Visible = false;
            }
            if (Struct.VisitMission.Length > 0)
            {
                label8.Text = Struct.VisitMission;
                flowLayoutPanel6.Visible = true;
            }
            else
            {
                flowLayoutPanel6.Visible = false;
            }
            if (Struct.AddresseeName.Length > 0)
            {
                label10.Text = Struct.AddresseeName;
                flowLayoutPanel17.Visible = true;
            }
            else
            {
                flowLayoutPanel17.Visible = false;
            }
            if (Struct.Validity.Length > 0)
            {
                label12.Text = Struct.Validity.ToString();
                flowLayoutPanel18.Visible = true;
            }
            else
            {
                flowLayoutPanel18.Visible = false;
            }
            if (Struct.InputDeniedTime.Length > 0)
            {
                label24.Text = Struct.InputDeniedTime.ToString();
                label25.Text = Struct.InputDeniedUserName;
                flowLayoutPanel22.Visible = true;
            }
            else
            {
                flowLayoutPanel22.Visible = false;
            }
            if (Struct.InputTime.Length > 0)
            {
                label27.Text = Struct.InputTime.ToString();
                flowLayoutPanel23.Visible = true;
            }
            else
            {
                flowLayoutPanel23.Visible = false;
            }
            if (Struct.OutputAllowedTime.Length > 0)
            {
                label29.Text = Struct.OutputAllowedTime.ToString();
                label31.Text = Struct.OutputAllowedUserName;
                flowLayoutPanel24.Visible = true;
            }
            else
            {
                flowLayoutPanel24.Visible = false;
            }
            if (Struct.OutputDeniedTime.Length > 0)
            {
                label33.Text = Struct.OutputDeniedTime.ToString();
                label34.Text = Struct.OutputDeniedUserName;
                flowLayoutPanel25.Visible = true;
            }
            else
            {
                flowLayoutPanel25.Visible = false;
            }
            if (Struct.OutputTime.Length > 0)
            {
                label36.Text = Struct.OutputTime.ToString();
                flowLayoutPanel26.Visible = true;
            }
            else
            {
                flowLayoutPanel26.Visible = false;
            }
            if (Struct.CreateTime.Length > 0)
            {
                label15.Text = Struct.CreateTime.ToString();
                label16.Text = Struct.CreateUserName;
                flowLayoutPanel19.Visible = true;
            }
            else
            {
                flowLayoutPanel19.Visible = false;
            }
            if (Struct.AgreedTime.Length > 0)
            {
                label18.Text = Struct.AgreedTime.ToString();
                label19.Text = Struct.AgreedUserName;
                flowLayoutPanel20.Visible = true;
            }
            else
            {
                flowLayoutPanel20.Visible = false;
            }
            if (Struct.ApprovedTime.Length > 0)
            {
                label21.Text = Struct.ApprovedTime.ToString();
                label22.Text = Struct.AprovedUserName;
                flowLayoutPanel21.Visible = true;
            }
            else
            {
                flowLayoutPanel21.Visible = false;
            }
        }

        private void GetScanningMachinesPermitInformation(VisitorsPermits.ScanedMachinesPermitsInfo Struct)
        {
            if (Struct.Name.Length > 0)
            {
                label6.Text = Struct.Name;
                flowLayoutPanel3.Visible = true;
            }
            else
            {
                flowLayoutPanel3.Visible = false;
            }
            if (Struct.VisitMission.Length > 0)
            {
                label8.Text = Struct.VisitMission;
                flowLayoutPanel6.Visible = true;
            }
            else
            {
                flowLayoutPanel6.Visible = false;
            }
            //if (Struct.AddresseeName.Length > 0)
            //{
            //    label10.Text = Struct.AddresseeName;
            //    flowLayoutPanel17.Visible = true;
            //}
            //else
            //{
            flowLayoutPanel17.Visible = false;
            //}
            if (Struct.Validity.Length > 0)
            {
                label12.Text = Struct.Validity.ToString();
                flowLayoutPanel18.Visible = true;
            }
            else
            {
                flowLayoutPanel18.Visible = false;
            }
            //if (Struct.InputDeniedTime.Length > 0)
            //{
            //    label24.Text = Struct.InputDeniedTime.ToString();
            //    label25.Text = Struct.InputDeniedUserName;
            //    flowLayoutPanel22.Visible = true;
            //}
            //else
            //{
            flowLayoutPanel22.Visible = false;
            //}
            //if (Struct.InputTime.Length > 0)
            //{
            //    label27.Text = Struct.InputTime.ToString();
            //    flowLayoutPanel23.Visible = true;
            //}
            //else
            //{
            flowLayoutPanel23.Visible = false;
            //}
            //if (Struct.OutputAllowedTime.Length > 0)
            //{
            //    label29.Text = Struct.OutputAllowedTime.ToString();
            //    label31.Text = Struct.OutputAllowedUserName;
            //    flowLayoutPanel24.Visible = true;
            //}
            //else
            //{
            flowLayoutPanel24.Visible = false;
            //}
            if (Struct.OutputDeniedTime.Length > 0)
            {
                label33.Text = Struct.OutputDeniedTime.ToString();
                label34.Text = Struct.OutputDeniedUserName;
                flowLayoutPanel25.Visible = true;
            }
            else
            {
                flowLayoutPanel25.Visible = false;
            }
            if (Struct.OutputTime.Length > 0)
            {
                label36.Text = Struct.OutputTime.ToString();
                flowLayoutPanel26.Visible = true;
            }
            else
            {
                flowLayoutPanel26.Visible = false;
            }
            if (Struct.CreateTime.Length > 0)
            {
                label15.Text = Struct.CreateTime.ToString();
                label16.Text = Struct.CreateUserName;
                flowLayoutPanel19.Visible = true;
            }
            else
            {
                flowLayoutPanel19.Visible = false;
            }
            if (Struct.AgreedTime.Length > 0)
            {
                label18.Text = Struct.AgreedTime.ToString();
                label19.Text = Struct.AgreedUserName;
                flowLayoutPanel20.Visible = true;
            }
            else
            {
                flowLayoutPanel20.Visible = false;
            }
            //if (Struct.ApprovedTime.Length > 0)
            //{
            //    label21.Text = Struct.ApprovedTime.ToString();
            //    label22.Text = Struct.AprovedUserName;
            //    flowLayoutPanel21.Visible = true;
            //}
            //else
            //{
            flowLayoutPanel21.Visible = false;
            //}
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!BarcodeTextBox.Focused)
            {
                //if (GetActiveWindow() != this.Handle)
                //{
                //    this.Activate();
                //}
                BarcodeTextBox.Focus();
            }
        }

        private void btnBeginScanInput1_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируется вход";
            label37.ForeColor = Color.FromArgb(82, 169, 24);
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            ScanType = ScanTypes.Input;
        }

        private void btnBeginScanOutput1_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируется выход";
            label37.ForeColor = Color.FromArgb(240, 0, 0);
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            ScanType = ScanTypes.Output;
        }

        private void btnBeginScanDispatch_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируется отгрузка";
            label37.ForeColor = Color.Blue;
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            flowLayoutPanel2.Visible = false;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            CheckTimer.Enabled = true;
            ScanType = ScanTypes.Dispatch;
            btnOpenMenu.Visible = false;
            btnBeginScanInput.Visible = false;
            btnBeginScanOutput.Visible = false;
            btnBeginScanInput1.Visible = true;
            btnBeginScanOutput1.Visible = true;
            btnBeginScanDispatch.Visible = false;
            btnBeginScanDispatch1.Visible = true;
            btnBackToPermits.Visible = true;
            pnlScanInput.BringToFront();
            pnlScanInput.Visible = true;
        }

        private void btnBeginScanDispatch1_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируется отгрузка";
            label37.ForeColor = Color.Blue;
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            ScanType = ScanTypes.Dispatch;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            btnClose.Checked = false;
            btnActive.Checked = false;
            btnOverdued.Checked = false;
            UpdatePermitDates();
        }

        private void btnUnloads_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируются ТМЦ";
            label37.ForeColor = Color.DarkGoldenrod;
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            CheckTimer.Enabled = true;
            ScanType = ScanTypes.Unload;
            btnOpenMenu.Visible = false;
            btnBeginScanInput.Visible = false;
            btnBeginScanOutput.Visible = false;
            btnBeginScanInput1.Visible = true;
            btnBeginScanOutput1.Visible = true;
            btnBeginScanDispatch.Visible = false;
            btnBeginScanDispatch1.Visible = true;
            btnBackToPermits.Visible = true;
            pnlScanInput.BringToFront();
            pnlScanInput.Visible = true;
        }

        private void btnMachinesPermits_Click(object sender, EventArgs e)
        {
            label37.Visible = true;
            label37.Text = "Сканируется транспорт";
            label37.ForeColor = Color.DarkGoldenrod;
            label37.Left = pnlTopMenu.Width / 2 - label37.Width / 2;
            ErrorPackLabel.Visible = false;
            BarcodeLabel.Text = "";
            CheckPicture.Visible = false;
            flowLayoutPanel2.Visible = false;
            panel2.Visible = false;
            CheckTimer.Enabled = true;
            ScanType = ScanTypes.Machines;
            btnOpenMenu.Visible = false;
            btnBeginScanInput.Visible = false;
            btnBeginScanOutput.Visible = false;
            btnBeginScanInput1.Visible = true;
            btnBeginScanOutput1.Visible = true;
            btnBeginScanDispatch.Visible = false;
            btnBeginScanDispatch1.Visible = true;
            btnBackToPermits.Visible = true;
            pnlScanInput.BringToFront();
            pnlScanInput.Visible = true;
        }
    }
}