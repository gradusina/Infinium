using Infinium.Modules.Permits;

using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class HistoryScanPermitsForm : Form
    {
        const int iAdmin = 89;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int _formEvent = 0;

        bool NeedSplash = false;

        Form _topForm = null;
        LightStartForm _lightStartForm;

        HistoryScanPermits _historyScanPermits;
        HistoryDispatch historyDispatchManager;

        //RoleTypes _roleType = RoleTypes.Ordinary;

        public enum RoleTypes
        {
            Ordinary = 0,
            Admin = 1
        }

        public HistoryScanPermitsForm()
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            historyDispatchManager = new HistoryDispatch();
            historyDispatchManager.Initialize();

            _historyScanPermits = new HistoryScanPermits();
            //_historyScanPermits.GetPermissions(Security.CurrentUserID, this.Name);
            //if (_historyScanPermits.PermissionGranted(iAdmin))
            //{
            //    _roleType = RoleTypes.Admin;
            //}
        }

        public HistoryScanPermitsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            _lightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            historyDispatchManager = new HistoryDispatch();
            historyDispatchManager.Initialize();

            _historyScanPermits = new HistoryScanPermits();
            //_historyScanPermits.GetPermissions(Security.CurrentUserID, this.Name);
            //if (_historyScanPermits.PermissionGranted(iAdmin))
            //{
            //    _roleType = RoleTypes.Admin;
            //}

            while (!SplashForm.bCreated) ;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (_topForm != null)
                    _topForm.Activate();
            }
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                if (_formEvent == eClose || _formEvent == eHide)
                {
                    AnimateTimer.Enabled = false;

                    if (_formEvent == eClose)
                    {

                        _lightStartForm.CloseForm(this);
                    }

                    if (_formEvent == eHide)
                    {

                        _lightStartForm.HideForm(this);
                    }


                    return;
                }

                if (_formEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    return;
                }

            }

            if (_formEvent == eClose || _formEvent == eHide)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.05));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (_formEvent == eClose)
                    {

                        _lightStartForm.CloseForm(this);
                    }

                    if (_formEvent == eHide)
                    {

                        _lightStartForm.HideForm(this);
                    }

                }

                return;
            }


            if (_formEvent == eShow || _formEvent == eShow)
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

        private void HistoryScanPermitsForm_Shown(object sender, EventArgs e)
        {
            _formEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedSplash = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            _formEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void HistoryScanPermitsForm_Load(object sender, EventArgs e)
        {
            dgvMainOrders.DataSource = historyDispatchManager.MainOrdersList;
            dgvPackages.DataSource = historyDispatchManager.PackagesList;
            dgvFrontsOrders.DataSource = historyDispatchManager.FrontsOrdersList;
            dgvDecorOrders.DataSource = historyDispatchManager.DecorOrdersList;
            dgvMainOrdersSetting();
            dgvPackagesSetting();
            dgvFrontsOrdersSetting();
            dgvDecorOrdersSetting();
            dgvScanUsersSettings();
            dgvInputScanningSettings();
            dgvOutputScanningSettings();
            dgvDispatchScanningSettings();
            dgvUnloadsScanningSettings();
            dgvMachinesPermitscanningSettings();
            CalendarFrom.SelectionStart = DateTime.Now.AddDays(-30);
            _historyScanPermits.UpdateDispatchScans(cbUsers.Checked, true, CalendarFrom.SelectionStart, CalendarTo.SelectionStart);
        }

        private void dgvScanUsersSettings()
        {
            dgvScanUsers.DataSource = _historyScanPermits.ScanUsersBs;
            if (dgvScanUsers.Columns.Contains("ScanUserID"))
                dgvScanUsers.Columns["ScanUserID"].Visible = false;
            dgvScanUsers.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvScanUsers.Columns["Check"].Width = 50;
            int DisplayIndex = 0;
            dgvScanUsers.Columns["Check"].DisplayIndex = DisplayIndex++;
            dgvScanUsers.Columns["Name"].DisplayIndex = DisplayIndex++;
        }


        private void dgvInputScanningSettings()
        {
            dgvInputScanning.DataSource = _historyScanPermits.ScanInputPermitsBs;
            if (dgvInputScanning.Columns.Contains("ScanVisitorPermitID"))
                dgvInputScanning.Columns["ScanVisitorPermitID"].Visible = false;
            if (dgvInputScanning.Columns.Contains("ScanUserID"))
                dgvInputScanning.Columns["ScanUserID"].Visible = false;
            if (dgvInputScanning.Columns.Contains("ScanDateTime"))
                dgvInputScanning.Columns["ScanDateTime"].Visible = false;
            if (dgvInputScanning.Columns.Contains("ScanType"))
                dgvInputScanning.Columns["ScanType"].Visible = false;

            foreach (DataGridViewColumn Column in dgvInputScanning.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.ReadOnly = true;
            }

            dgvInputScanning.Columns["PermitID"].HeaderText = "№п/п";
            dgvInputScanning.Columns["ScanDateTime"].HeaderText = "Дата сканирования";
            dgvInputScanning.Columns["VisitorName"].HeaderText = "Посетитель";
            dgvInputScanning.Columns["VisitMission"].HeaderText = "Цель визита";
            dgvInputScanning.Columns["AddresseeName"].HeaderText = "К кому";
            dgvInputScanning.Columns["InputTime"].HeaderText = "Вход";
            int DisplayIndex = 0;
            dgvInputScanning.Columns["PermitID"].DisplayIndex = DisplayIndex++;
            dgvInputScanning.Columns["ScanDateTime"].DisplayIndex = DisplayIndex++;
            dgvInputScanning.Columns["VisitorName"].DisplayIndex = DisplayIndex++;
            dgvInputScanning.Columns["VisitMission"].DisplayIndex = DisplayIndex++;
            dgvInputScanning.Columns["AddresseeName"].DisplayIndex = DisplayIndex++;
            dgvInputScanning.Columns["InputTime"].DisplayIndex = DisplayIndex++;
        }

        private void dgvOutputScanningSettings()
        {
            dgvOutputScanning.DataSource = _historyScanPermits.ScanOutputPermitsBs;
            if (dgvInputScanning.Columns.Contains("ScanVisitorPermitID"))
                dgvOutputScanning.Columns["ScanVisitorPermitID"].Visible = false;
            if (dgvOutputScanning.Columns.Contains("ScanUserID"))
                dgvOutputScanning.Columns["ScanUserID"].Visible = false;
            if (dgvOutputScanning.Columns.Contains("ScanDateTime"))
                dgvOutputScanning.Columns["ScanDateTime"].Visible = false;
            if (dgvOutputScanning.Columns.Contains("ScanType"))
                dgvOutputScanning.Columns["ScanType"].Visible = false;

            foreach (DataGridViewColumn Column in dgvOutputScanning.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.ReadOnly = true;
            }

            dgvOutputScanning.Columns["PermitID"].HeaderText = "№п/п";
            dgvOutputScanning.Columns["ScanDateTime"].HeaderText = "Дата сканирования";
            dgvOutputScanning.Columns["VisitorName"].HeaderText = "Посетитель";
            dgvOutputScanning.Columns["VisitMission"].HeaderText = "Цель визита";
            dgvOutputScanning.Columns["AddresseeName"].HeaderText = "К кому";
            dgvOutputScanning.Columns["OutputTime"].HeaderText = "Выход";
            int DisplayIndex = 0;
            dgvOutputScanning.Columns["PermitID"].DisplayIndex = DisplayIndex++;
            dgvOutputScanning.Columns["ScanDateTime"].DisplayIndex = DisplayIndex++;
            dgvOutputScanning.Columns["VisitorName"].DisplayIndex = DisplayIndex++;
            dgvOutputScanning.Columns["VisitMission"].DisplayIndex = DisplayIndex++;
            dgvOutputScanning.Columns["AddresseeName"].DisplayIndex = DisplayIndex++;
            dgvOutputScanning.Columns["OutputTime"].DisplayIndex = DisplayIndex++;
        }

        private void dgvDispatchScanningSettings()
        {
            dgvDispatchScanning.DataSource = _historyScanPermits.ScanDispatchPermitsBs;
            dgvDispatchScanning.Columns.Add(_historyScanPermits.ScanUserNameColumn);
            if (dgvDispatchScanning.Columns.Contains("ScanPermitID"))
                dgvDispatchScanning.Columns["ScanPermitID"].Visible = false;
            if (dgvDispatchScanning.Columns.Contains("ScanUserID"))
                dgvDispatchScanning.Columns["ScanUserID"].Visible = false;

            foreach (DataGridViewColumn Column in dgvDispatchScanning.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.ReadOnly = true;
            }

            dgvDispatchScanning.Columns["PermitID"].HeaderText = "№п/п";
            dgvDispatchScanning.Columns["ScanDateTime"].HeaderText = "Дата сканирования";
            int DisplayIndex = 0;
            dgvDispatchScanning.Columns["PermitID"].DisplayIndex = DisplayIndex++;
            dgvDispatchScanning.Columns["ScanDateTime"].DisplayIndex = DisplayIndex++;
            dgvDispatchScanning.Columns["ScanUserName"].DisplayIndex = DisplayIndex++;
        }

        private void dgvUnloadsScanningSettings()
        {
            dgvUnloadsScanning.DataSource = _historyScanPermits.ScanUnloadsPermitsBs;
            dgvUnloadsScanning.Columns.Add(_historyScanPermits.ScanUserNameColumn);
            if (dgvUnloadsScanning.Columns.Contains("ScanUnloadID"))
                dgvUnloadsScanning.Columns["ScanUnloadID"].Visible = false;
            if (dgvUnloadsScanning.Columns.Contains("ScanUserID"))
                dgvUnloadsScanning.Columns["ScanUserID"].Visible = false;
            if (dgvUnloadsScanning.Columns.Contains("Subject"))
                dgvUnloadsScanning.Columns["Subject"].Visible = false;
            if (dgvUnloadsScanning.Columns.Contains("ScanType"))
                dgvUnloadsScanning.Columns["ScanType"].Visible = false;

            foreach (DataGridViewColumn Column in dgvUnloadsScanning.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.ReadOnly = true;
            }

            dgvUnloadsScanning.Columns["UnloadID"].HeaderText = "№п/п";
            dgvUnloadsScanning.Columns["ScanDateTime"].HeaderText = "Дата сканирования";
            dgvUnloadsScanning.Columns["SubjectName"].HeaderText = "Предмет";
            dgvUnloadsScanning.Columns["Count"].HeaderText = "Кол-во";
            dgvUnloadsScanning.Columns["Measure"].HeaderText = "Ед.изм.";
            dgvUnloadsScanning.Columns["Subject"].HeaderText = "Предмет";
            int DisplayIndex = 0;
            dgvUnloadsScanning.Columns["UnloadID"].DisplayIndex = DisplayIndex++;
            dgvUnloadsScanning.Columns["ScanDateTime"].DisplayIndex = DisplayIndex++;
            dgvUnloadsScanning.Columns["ScanUserName"].DisplayIndex = DisplayIndex++;
            dgvUnloadsScanning.Columns["SubjectName"].DisplayIndex = DisplayIndex++;
            dgvUnloadsScanning.Columns["Count"].DisplayIndex = DisplayIndex++;
            dgvUnloadsScanning.Columns["Measure"].DisplayIndex = DisplayIndex++;
        }

        private void dgvMachinesPermitscanningSettings()
        {
            dgvMachinesPermits.DataSource = _historyScanPermits.ScanMachinesPermitsBs;
            if (dgvMachinesPermits.Columns.Contains("ScanMachinePermitID"))
                dgvMachinesPermits.Columns["ScanMachinePermitID"].Visible = false;
            if (dgvMachinesPermits.Columns.Contains("ScanUserID"))
                dgvMachinesPermits.Columns["ScanUserID"].Visible = false;
            if (dgvMachinesPermits.Columns.Contains("ScanDateTime"))
                dgvMachinesPermits.Columns["ScanDateTime"].Visible = false;

            foreach (DataGridViewColumn Column in dgvOutputScanning.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.ReadOnly = true;
            }

            dgvMachinesPermits.Columns["MachinePermitID"].HeaderText = "№п/п";
            dgvMachinesPermits.Columns["ScanDateTime"].HeaderText = "Дата сканирования";
            dgvMachinesPermits.Columns["Name"].HeaderText = "Авто";
            dgvMachinesPermits.Columns["VisitMission"].HeaderText = "Цель";
            dgvMachinesPermits.Columns["OutputTime"].HeaderText = "Выход";
            int DisplayIndex = 0;
            dgvMachinesPermits.Columns["MachinePermitID"].DisplayIndex = DisplayIndex++;
            dgvMachinesPermits.Columns["ScanDateTime"].DisplayIndex = DisplayIndex++;
            dgvMachinesPermits.Columns["Name"].DisplayIndex = DisplayIndex++;
            dgvMachinesPermits.Columns["VisitMission"].DisplayIndex = DisplayIndex++;
            dgvMachinesPermits.Columns["OutputTime"].DisplayIndex = DisplayIndex++;
        }


        private void dgvMainOrdersSetting()
        {
            dgvMainOrders.Columns["FactoryID"].Visible = false;
            dgvMainOrders.Columns["ProfilPackCount"].Visible = false;
            dgvMainOrders.Columns["TPSPackCount"].Visible = false;
            foreach (DataGridViewColumn Column in dgvMainOrders.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            dgvMainOrders.Columns["MainOrderID"].HeaderText = "№ п\\п";
            dgvMainOrders.Columns["Weight"].HeaderText = "Вес, кг.";
            dgvMainOrders.Columns["ClientName"].HeaderText = "Клиент";
            dgvMainOrders.Columns["Notes"].HeaderText = "Примечание";
            dgvMainOrders.Columns["ProfilDispPercentage"].HeaderText = " Отгружено\r\nПрофиль, %";
            dgvMainOrders.Columns["ProfilDispatchedCount"].HeaderText = "Отгружено\r\n Профиль, кол-во";
            dgvMainOrders.Columns["TPSDispPercentage"].HeaderText = "Отгружено\r\nТПС, %";
            dgvMainOrders.Columns["TPSDispatchedCount"].HeaderText = " Отгружено\r\nТПС, кол-во";

            dgvMainOrders.Columns["ClientName"].MinimumWidth = 155;
            dgvMainOrders.Columns["Notes"].MinimumWidth = 155;

            //dgvMainOrders.Columns["ProfilDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvMainOrders.Columns["ProfilDispPercentage"].Width = 155;
            //dgvMainOrders.Columns["ProfilDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvMainOrders.Columns["ProfilDispatchedCount"].Width = 155;
            //dgvMainOrders.Columns["TPSDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvMainOrders.Columns["TPSDispPercentage"].Width = 125;
            //dgvMainOrders.Columns["TPSDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvMainOrders.Columns["TPSDispatchedCount"].Width = 125;

            dgvMainOrders.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvMainOrders.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            dgvMainOrders.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            dgvMainOrders.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvMainOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvMainOrders.Columns["ProfilDispatchedCount"].DisplayIndex = DisplayIndex++;
            dgvMainOrders.Columns["TPSDispatchedCount"].DisplayIndex = DisplayIndex++;
            dgvMainOrders.Columns["ProfilDispPercentage"].DisplayIndex = DisplayIndex++;
            dgvMainOrders.Columns["TPSDispPercentage"].DisplayIndex = DisplayIndex++;

            dgvMainOrders.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvMainOrders.Columns["ProfilDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["ProfilDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("ProfilDispPercentage");
            dgvMainOrders.Columns["TPSDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["TPSDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("TPSDispPercentage");
        }

        private void dgvPackagesSetting()
        {
            dgvPackages.Columns["ProductType"].Visible = false;
            dgvPackages.Columns["MainOrderID"].Visible = false;
            dgvPackages.Columns["PackingDateTime"].Visible = false;
            dgvPackages.Columns["StorageDateTime"].Visible = false;
            dgvPackages.Columns["ExpeditionDateTime"].Visible = false;
            dgvPackages.Columns["TrayID"].Visible = false;
            dgvPackages.Columns["DispatchID"].Visible = false;

            dgvPackages.Columns["PackingDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPackages.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPackages.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPackages.Columns["DispatchDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            foreach (DataGridViewColumn Column in dgvPackages.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            dgvPackages.Columns["DispatchID"].HeaderText = "  №\r\nотгр.";
            dgvPackages.Columns["PackNumber"].HeaderText = "  №\r\nупак.";
            dgvPackages.Columns["PackageStatus"].HeaderText = "Статус";
            dgvPackages.Columns["PackingDateTime"].HeaderText = "   Дата\r\nупаковки";
            dgvPackages.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            dgvPackages.Columns["ExpeditionDateTime"].HeaderText = "      Дата\r\nэкспедиции";
            dgvPackages.Columns["DispatchDateTime"].HeaderText = "    Дата\r\nотгрузки";
            dgvPackages.Columns["FactoryName"].HeaderText = "Участок";
            dgvPackages.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackNumber"].Width = 70;
            dgvPackages.Columns["PackageStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackageStatus"].Width = 140;
            dgvPackages.Columns["PackingDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackingDateTime"].Width = 150;
            dgvPackages.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["StorageDateTime"].Width = 150;
            dgvPackages.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["ExpeditionDateTime"].Width = 150;
            dgvPackages.Columns["DispatchDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["DispatchDateTime"].Width = 150;
            dgvPackages.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["FactoryName"].Width = 100;
            dgvPackages.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackageID"].Width = 100;
            dgvPackages.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["TrayID"].Width = 100;
        }

        private void dgvFrontsOrdersSetting()
        {
            if (!dgvFrontsOrders.Columns.Contains("FrontsColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.FrontsColumn);
            if (!dgvFrontsOrders.Columns.Contains("FrameColorsColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.FrameColorsColumn);
            if (!dgvFrontsOrders.Columns.Contains("PatinaColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.PatinaColumn);
            if (!dgvFrontsOrders.Columns.Contains("InsetTypesColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.InsetTypesColumn);
            if (!dgvFrontsOrders.Columns.Contains("InsetColorsColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.InsetColorsColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoProfilesColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.TechnoProfilesColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoFrameColorsColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.TechnoFrameColorsColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoInsetTypesColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.TechnoInsetTypesColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoInsetColorsColumn"))
                dgvFrontsOrders.Columns.Add(historyDispatchManager.TechnoInsetColorsColumn);

            if (dgvFrontsOrders.Columns.Contains("ImpostMargin"))
                dgvFrontsOrders.Columns["ImpostMargin"].Visible = false;
            if (dgvFrontsOrders.Columns.Contains("CreateDateTime"))
            {
                dgvFrontsOrders.Columns["CreateDateTime"].HeaderText = "Добавлено";
                dgvFrontsOrders.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dgvFrontsOrders.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvFrontsOrders.Columns["CreateDateTime"].Width = 100;
            }
            if (dgvFrontsOrders.Columns.Contains("CreateUserID"))
                dgvFrontsOrders.Columns["CreateUserID"].Visible = false;
            dgvFrontsOrders.Columns["FrontsOrdersID"].Visible = false;
            dgvFrontsOrders.Columns["MainOrderID"].Visible = false;
            dgvFrontsOrders.Columns["FrontID"].Visible = false;
            dgvFrontsOrders.Columns["ColorID"].Visible = false;
            dgvFrontsOrders.Columns["InsetColorID"].Visible = false;
            dgvFrontsOrders.Columns["PatinaID"].Visible = false;
            dgvFrontsOrders.Columns["InsetTypeID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoProfileID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoColorID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoInsetTypeID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoInsetColorID"].Visible = false;

            foreach (DataGridViewColumn Column in dgvFrontsOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            dgvFrontsOrders.Columns["Height"].HeaderText = "Высота";
            dgvFrontsOrders.Columns["Width"].HeaderText = "Ширина";
            dgvFrontsOrders.Columns["Count"].HeaderText = "Кол-во";
            dgvFrontsOrders.Columns["Notes"].HeaderText = "Примечание";
            dgvFrontsOrders.Columns["Square"].HeaderText = "Площадь";

            dgvFrontsOrders.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Height"].Width = 85;
            dgvFrontsOrders.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Width"].Width = 85;
            dgvFrontsOrders.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Count"].Width = 65;
            dgvFrontsOrders.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Square"].Width = 100;
            dgvFrontsOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["Notes"].MinimumWidth = 105;

            dgvFrontsOrders.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvFrontsOrders.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Count"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Square"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.CellFormatting += FrontsOrdersDataGrid_CellFormatting;
        }

        private void dgvDecorOrdersSetting()
        {
            if (!dgvDecorOrders.Columns.Contains("ProductColumn"))
                dgvDecorOrders.Columns.Add(historyDispatchManager.ProductColumn);
            if (!dgvDecorOrders.Columns.Contains("ItemColumn"))
                dgvDecorOrders.Columns.Add(historyDispatchManager.ItemColumn);
            if (!dgvDecorOrders.Columns.Contains("ColorColumn"))
                dgvDecorOrders.Columns.Add(historyDispatchManager.ColorColumn);
            if (!dgvDecorOrders.Columns.Contains("PatinaColumn"))
                dgvDecorOrders.Columns.Add(historyDispatchManager.DecorPatinaColumn);
            if (dgvDecorOrders.Columns.Contains("CreateDateTime"))
            {
                dgvDecorOrders.Columns["CreateDateTime"].HeaderText = "Добавлено";
                dgvDecorOrders.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dgvDecorOrders.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvDecorOrders.Columns["CreateDateTime"].Width = 100;
            }
            if (dgvDecorOrders.Columns.Contains("CreateUserID"))
                dgvDecorOrders.Columns["CreateUserID"].Visible = false;
            dgvDecorOrders.Columns["DecorOrderID"].Visible = false;
            dgvDecorOrders.Columns["ProductID"].Visible = false;
            dgvDecorOrders.Columns["DecorID"].Visible = false;
            dgvDecorOrders.Columns["ColorID"].Visible = false;
            dgvDecorOrders.Columns["PatinaID"].Visible = false;

            foreach (DataGridViewColumn Column in dgvDecorOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            dgvDecorOrders.Columns["Length"].HeaderText = "Длина";
            dgvDecorOrders.Columns["Height"].HeaderText = "Высота";
            dgvDecorOrders.Columns["Width"].HeaderText = "Ширина";
            dgvDecorOrders.Columns["Count"].HeaderText = "Кол-во";
            dgvDecorOrders.Columns["Notes"].HeaderText = "Примечание";

            dgvDecorOrders.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ProductColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ItemColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ColorsColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["PatinaColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Height"].Width = 85;
            dgvDecorOrders.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Width"].Width = 85;
            dgvDecorOrders.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Count"].Width = 85;
            dgvDecorOrders.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Length"].Width = 85;
            dgvDecorOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecorOrders.Columns["Notes"].MinimumWidth = 145;

            dgvDecorOrders.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvDecorOrders.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Count"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        void FrontsOrdersDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns.Contains("PatinaColumn") && (e.ColumnIndex == grid.Columns["PatinaColumn"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int PatinaID = -1;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["PatinaID"].Value != DBNull.Value)
                {
                    PatinaID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PatinaID"].Value);
                    DisplayName = historyDispatchManager.PatinaDisplayName(PatinaID);
                }
                cell.ToolTipText = DisplayName;
            }
        }

        private void dgvMainOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (historyDispatchManager == null)
                return;

            dgvMainOrders.Columns["ProfilDispatchedCount"].Visible = false;
            dgvMainOrders.Columns["ProfilDispPercentage"].Visible = false;
            dgvMainOrders.Columns["TPSDispatchedCount"].Visible = false;
            dgvMainOrders.Columns["TPSDispPercentage"].Visible = false;
            int PermitID = -1;
            if (dgvDispatchScanning.SelectedRows.Count > 0 && dgvDispatchScanning.SelectedRows[0].Cells["PermitID"].Value != DBNull.Value)
                PermitID = Convert.ToInt32(dgvDispatchScanning.SelectedRows[0].Cells["PermitID"].Value);
            int MainOrderID = -1;
            if (dgvMainOrders.SelectedRows.Count > 0 &&
                dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
            {
                MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);
                if (Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FactoryID"].Value) == 1 ||
                    Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FactoryID"].Value) == 0)
                {
                    dgvMainOrders.Columns["ProfilDispatchedCount"].Visible = true;
                    dgvMainOrders.Columns["ProfilDispPercentage"].Visible = true;
                }
                if (Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FactoryID"].Value) == 2 ||
                    Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FactoryID"].Value) == 0)
                {
                    dgvMainOrders.Columns["TPSDispatchedCount"].Visible = true;
                    dgvMainOrders.Columns["TPSDispPercentage"].Visible = true;
                }
            }
            if (cbByPackages.Checked)
            {
                if (NeedSplash)
                {
                    Thread T =
                        new Thread(
                            delegate ()
                            {
                                SplashWindow.CreateSmallSplash(ref _topForm,
                                    "Загрузка данных с сервера.\r\nПодождите...");
                            });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;
                    NeedSplash = false;
                    historyDispatchManager.FilterPackages(PermitID, MainOrderID);
                    NeedSplash = true;
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
                else
                    historyDispatchManager.FilterPackages(PermitID, MainOrderID);
            }
            else
            {
                historyDispatchManager.ClearPackages();
                if (NeedSplash)
                {
                    Thread T =
                        new Thread(
                            delegate ()
                            {
                                SplashWindow.CreateSmallSplash(ref _topForm,
                                    "Загрузка данных с сервера.\r\nПодождите...");
                            });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;
                    NeedSplash = false;
                    FilterOrders(MainOrderID);
                    NeedSplash = true;
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
                else
                    FilterOrders(MainOrderID);
            }
        }

        private void dgvPackages_SelectionChanged(object sender, EventArgs e)
        {
            if (historyDispatchManager == null)
                return;

            int MainOrderID = -1;
            if (dgvMainOrders.SelectedRows.Count > 0 && dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);
            int PackageID = -1;
            if (dgvPackages.SelectedRows.Count > 0 && dgvPackages.SelectedRows[0].Cells["PackageID"].Value != DBNull.Value)
                PackageID = Convert.ToInt32(dgvPackages.SelectedRows[0].Cells["PackageID"].Value);
            int ProductType = -1;
            if (dgvPackages.SelectedRows.Count > 0 && dgvPackages.SelectedRows[0].Cells["ProductType"].Value != DBNull.Value)
                ProductType = Convert.ToInt32(dgvPackages.SelectedRows[0].Cells["ProductType"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                FilterOrders(PackageID, ProductType, MainOrderID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                FilterOrders(PackageID, ProductType, MainOrderID);
            }
        }

        private void FilterOrders(int MainOrderID)
        {
            if (dgvMainOrders.SelectedRows.Count == 0)
                return;

            historyDispatchManager.ClearFrontsOrders();
            historyDispatchManager.ClearDecorOrders();
            tabOrders.TabPages[0].PageVisible = historyDispatchManager.FilterFrontsOrders(MainOrderID);
            tabOrders.TabPages[1].PageVisible = historyDispatchManager.FilterDecorOrders(MainOrderID);
        }

        private void FilterOrders(int PackageID, int ProductType, int MainOrderID)
        {
            if (dgvMainOrders.SelectedRows.Count == 0)
                return;

            historyDispatchManager.ClearFrontsOrders();
            historyDispatchManager.ClearDecorOrders();
            if (dgvPackages.SelectedRows.Count == 0)
            {
                return;
            }
            else
            {
                tabOrders.TabPages[0].PageVisible = historyDispatchManager.FilterFrontsOrders(PackageID, MainOrderID);
                tabOrders.TabPages[1].PageVisible = historyDispatchManager.FilterDecorOrders(PackageID, MainOrderID);

                if (ProductType == 0)
                {
                    tabOrders.SelectedTabPage = tabOrders.TabPages[0];
                }
                if (ProductType == 1)
                {
                    tabOrders.SelectedTabPage = tabOrders.TabPages[1];
                }
            }
        }

        private void UpdateDispatches()
        {
            //dgvDispatchScanning.SelectionChanged -= dgvDispatchScanning_SelectionChanged;
            _historyScanPermits.UpdateDispatchScans(cbUsers.Checked, cbDate.Checked, CalendarFrom.SelectionStart, CalendarTo.SelectionStart);
            //dgvDispatchScanning.SelectionChanged += dgvDispatchScanning_SelectionChanged;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            CommonMenuPanel.Visible = !CommonMenuPanel.Visible;
        }

        private void cbDate_CheckedChanged(object sender, EventArgs e)
        {
            CalendarFrom.Enabled = cbDate.Checked;
            CalendarTo.Enabled = cbDate.Checked;
        }

        private void cbSelectAllUsers_CheckedChanged(object sender, EventArgs e)
        {
            _historyScanPermits.SelectAllUsers(cbSelectAllUsers.Checked);
        }

        private void cbUsers_CheckedChanged(object sender, EventArgs e)
        {
            cbSelectAllUsers.Visible = cbUsers.Checked;
            panel4.Enabled = cbUsers.Checked;
        }

        private void btnFilterWorks_Click(object sender, EventArgs e)
        {
            Thread T =
                new Thread(
                    delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Обновление данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            UpdateDispatches();
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvDispatchScanning_SelectionChanged(object sender, EventArgs e)
        {
            if (historyDispatchManager == null)
                return;

            int PermitID = -1;
            if (dgvDispatchScanning.SelectedRows.Count > 0 && dgvDispatchScanning.SelectedRows[0].Cells["PermitID"].Value != DBNull.Value)
                PermitID = Convert.ToInt32(dgvDispatchScanning.SelectedRows[0].Cells["PermitID"].Value);

            if (NeedSplash)
            {
                Thread T =
                    new Thread(
                        delegate ()
                        {
                            SplashWindow.CreateSmallSplash(ref _topForm, "Загрузка данных с сервера.\r\nПодождите...");
                        });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                historyDispatchManager.FilterMainOrders(PermitID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                historyDispatchManager.FilterMainOrders(PermitID);
            }
        }

        private void cbByPackages_CheckedChanged(object sender, EventArgs e)
        {
            dgvMainOrders_SelectionChanged(null, null);
        }

    }
}
