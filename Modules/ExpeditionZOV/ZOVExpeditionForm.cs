using Infinium.Modules.ZOV.Expedition;

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVExpeditionForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;
        int CurrentRowIndex = -1;
        int CurrentMainOrder = 0;
        int CurrentPackNumber = 0;
        int ZDispatchID = 0;

        bool bCheckBoxShow = false;

        bool MoveFromPayments = false;
        bool MoveFromPermits = false;
        bool NeedRefresh = false;
        bool NeedSplash = false;

        DateTime PrepareDispatchDateTime;

        string PaymentsDocNumber = string.Empty;

        Form MainForm;
        Form TopForm;
        LightStartForm LightStartForm;

        private Infinium.Modules.ZOV.Expedition.ZOVExpeditionManager ZOVExpeditionManager;

        const int iSetDispatchDate = 18;
        const int iCreateDispatch = 61;
        const int iConfirmDispatch = 62;
        const int iPrintDispReport = 63;

        bool bCreateDispatch = false;
        bool bSetDispatchDate = false;
        bool bConfirmDispatch = false;
        bool bPrintDispReport = false;

        RoleTypes RoleType = RoleTypes.OrdinaryRole;

        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1,
            LogisticsRole = 2,
            ConfirmRole = 3,
            DispatchRole = 4
        }

        DataTable MonthsDT;
        DataTable YearsDT;
        DataTable RolePermissionsDataTable;

        DispatchReportZOV DispatchReport;
        ZOVDispatch ZOVDispatchManager;

        public ZOVExpeditionForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);
            if (PermissionGranted(iCreateDispatch))
            {
                bCreateDispatch = true;
            }
            if (PermissionGranted(iSetDispatchDate))
            {
                bSetDispatchDate = true;
            }
            if (PermissionGranted(iConfirmDispatch))
            {
                bConfirmDispatch = true;
            }
            if (PermissionGranted(iPrintDispReport))
            {
                bPrintDispReport = true;
            }

            if (bCreateDispatch && bSetDispatchDate && bPrintDispReport && bConfirmDispatch)
                RoleType = RoleTypes.AdminRole;
            if (bCreateDispatch && bSetDispatchDate && bPrintDispReport && !bConfirmDispatch)
                RoleType = RoleTypes.LogisticsRole;
            if (!bCreateDispatch && !bSetDispatchDate && bPrintDispReport && !bConfirmDispatch)
                RoleType = RoleTypes.DispatchRole;
            if (!bCreateDispatch && !bSetDispatchDate && !bPrintDispReport && bConfirmDispatch)
                RoleType = RoleTypes.ConfirmRole;

            if (RoleType == RoleTypes.OrdinaryRole)
            {
                //btnAddDispatch.Visible = false;
                //btnEditDispatch.Visible = false;
                btnChangeDispatchDate.Visible = false;
                cmiChangeDispatchDate.Visible = false;
                btnConfirmExpedition.Visible = false;
                ConfirmExpContextMenuItem.Visible = false;
                btnConfirmDispatch.Visible = false;
                ConfirmDispatchContextMenuItem.Visible = false;
                PrintDispatchListMenuItem.Visible = false;
                AttachContextMenuItem.Visible = false;
                cmiBindToPermit.Visible = false;
                cmiRemoveDispatch.Visible = false;
            }
            if (RoleType == RoleTypes.AdminRole)
            {
                //btnAddDispatch.Visible = true;
                //btnEditDispatch.Visible = true;
                btnChangeDispatchDate.Visible = true;
                cmiChangeDispatchDate.Visible = true;
                btnConfirmExpedition.Visible = true;
                ConfirmExpContextMenuItem.Visible = true;
                btnConfirmDispatch.Visible = true;
                cmiBindToPermit.Visible = true;
                ConfirmDispatchContextMenuItem.Visible = true;
                PrintDispatchListMenuItem.Visible = true;
                AttachContextMenuItem.Visible = true;
                cmiRemoveDispatch.Visible = true;
                cmChangeDispatchDate1.Visible = true;
                cmChangeDispatchDate2.Visible = true;
            }
            if (RoleType == RoleTypes.ConfirmRole)
            {
                btnConfirmExpedition.Visible = false;
                btnChangeDispatchDate.Visible = false;
                cmiChangeDispatchDate.Visible = false;
                ConfirmExpContextMenuItem.Visible = false;
                btnConfirmDispatch.Visible = true;
                cmiBindToPermit.Visible = false;
                ConfirmDispatchContextMenuItem.Visible = true;
                PrintDispatchListMenuItem.Visible = false;
                AttachContextMenuItem.Visible = false;
                cmiRemoveDispatch.Visible = false;
            }
            if (RoleType == RoleTypes.LogisticsRole)
            {
                //btnAddDispatch.Visible = true;
                //btnEditDispatch.Visible = true;
                btnChangeDispatchDate.Visible = true;
                cmiChangeDispatchDate.Visible = true;
                btnConfirmExpedition.Visible = true;
                ConfirmExpContextMenuItem.Visible = true;
                btnConfirmDispatch.Visible = false;
                cmiBindToPermit.Visible = true;
                ConfirmDispatchContextMenuItem.Visible = false;
                cmiRemoveDispatch.Visible = true;
                cmChangeDispatchDate1.Visible = true;
                cmChangeDispatchDate2.Visible = true;
            }
            if (RoleType == RoleTypes.DispatchRole)
            {
                //btnAddDispatch.Visible = false;
                //btnEditDispatch.Visible = false;
                btnChangeDispatchDate.Visible = false;
                cmiChangeDispatchDate.Visible = false;
                btnConfirmExpedition.Visible = false;
                ConfirmExpContextMenuItem.Visible = false;
                btnConfirmDispatch.Visible = false;
                cmiBindToPermit.Visible = false;
                ConfirmDispatchContextMenuItem.Visible = false;
                PrintDispatchListMenuItem.Visible = true;
                AttachContextMenuItem.Visible = true;
                cmiRemoveDispatch.Visible = false;
            }

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        public ZOVExpeditionForm(Form tMainForm, string sPaymentsDocNumber)
        {
            InitializeComponent();
            MainForm = tMainForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);

            Initialize();
            MoveFromPayments = true;
            PaymentsDocNumber = sPaymentsDocNumber;

            while (!SplashForm.bCreated) ;
        }

        public ZOVExpeditionForm(Form tMainForm, DateTime dPrepareDispatchDateTime, int iDispatchID)
        {
            InitializeComponent();
            MainForm = tMainForm;
            PrepareDispatchDateTime = dPrepareDispatchDateTime;
            ZDispatchID = iDispatchID;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);
            MoveFromPermits = true;
            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void ZOVExpeditionForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            NeedSplash = true;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            //SetDispatchedButton.Enabled = PermissionGranted(Convert.ToInt32(SetDispatchedButton.Tag));
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
                        if (!MoveFromPayments && !MoveFromPermits)
                            LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        if (!MoveFromPayments && !MoveFromPermits)
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
                        if (!MoveFromPayments && !MoveFromPermits)
                            LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        if (!MoveFromPayments && !MoveFromPermits)
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
            NeedSplash = false;
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
            ZOVExpeditionManager = new Modules.ZOV.Expedition.ZOVExpeditionManager(
                ref MegaOrdersDataGrid, ref MainOrdersDataGrid, ref PackagesDataGrid,
                ref MainOrdersFrontsOrdersDataGrid, ref MainOrdersDecorOrdersDataGrid, ref MainOrdersTabControl);

            //if (PermissionGranted(Convert.ToInt32(kryptonContextMenu1.Tag)))
            //{
            //    CanDispatchDebts = true;
            //}

            DispatchReport = new DispatchReportZOV();

            MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));

            YearsDT = new DataTable();
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

            ZOVDispatchManager = new ZOVDispatch();
            ZOVDispatchManager.Initialize();
            dgvDispatchSetting();
            dgvDispatchDatesSetting();
            dgvMainOrdersSetting();
        }

        private void MegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ZOVExpeditionManager != null)
                if (ZOVExpeditionManager.MegaOrdersBS.Count > 0)
                {
                    if (((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        bool bShowDispatched = ShowDispatchedCheckBox.Checked;
                        int FactoryID = -1;

                        if (ProfilCheckBox.Checked && TPSCheckBox.Checked)
                            FactoryID = 0;
                        if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                            FactoryID = 1;
                        if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                            FactoryID = 2;

                        if (NeedSplash)
                        {
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;
                            NeedSplash = false;
                            if (AllPackagesCheckBox.Checked)
                                ZOVExpeditionManager.FilterPackagesByMegaOrder(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]),
                                    FactoryID);
                            else
                                ZOVExpeditionManager.FilterMainOrdersByMegaOrder(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]),
                                    true, true, true, true, bShowDispatched, FactoryID);
                            ZOVExpeditionManager.GetAllPackages(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]), FactoryID);
                            //ZOVExpeditionManager.SetMainOrderDispatchStatus();
                            NeedSplash = true;
                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                        }
                        else
                        {
                            if (AllPackagesCheckBox.Checked)
                                ZOVExpeditionManager.FilterPackagesByMegaOrder(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]),
                                    FactoryID);
                            else
                                ZOVExpeditionManager.FilterMainOrdersByMegaOrder(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]),
                                    true, true, true, true, bShowDispatched, FactoryID);
                            ZOVExpeditionManager.GetAllPackages(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]), FactoryID);
                            //ZOVExpeditionManager.SetMainOrderDispatchStatus();
                        }
                    }
                }
                else
                    ZOVExpeditionManager.MainOrdersDT.Clear();
        }

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ZOVExpeditionManager != null)
                if (ZOVExpeditionManager.MainOrdersBS.Count > 0)
                {
                    if (((DataRowView)ZOVExpeditionManager.MainOrdersBS.Current)["MainOrderID"] != DBNull.Value)
                    {
                        int FactoryID = -1;

                        if (ProfilCheckBox.Checked && TPSCheckBox.Checked)
                            FactoryID = 0;
                        if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                            FactoryID = 1;
                        if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                            FactoryID = 2;

                        ZOVExpeditionManager.FilterProductsByPackage(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MainOrdersBS.Current)["MainOrderID"]), FactoryID);
                        ZOVExpeditionManager.FilterPackagesByMainOrder(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MainOrdersBS.Current)["MainOrderID"]), FactoryID);
                        ZOVExpeditionManager.SetPackageDispatchStatus();
                    }
                }
                else
                {
                    ZOVExpeditionManager.FilterProductsByPackage(-1, 0);
                    ZOVExpeditionManager.PackagesDT.Clear();
                    ZOVExpeditionManager.PackedMainOrdersFrontsOrders.FrontsOrdersDataTable.Clear();
                    ZOVExpeditionManager.PackedMainOrdersDecorOrders.DecorOrdersDataTable.Clear();
                }
        }

        private void MegaOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MegaOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void MegaOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void PackagesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ZOVExpeditionManager != null)
                if (ZOVExpeditionManager.PackagesBS.Count > 0)
                {
                    if (((DataRowView)ZOVExpeditionManager.PackagesBS.Current)["PackNumber"] != DBNull.Value)
                    {
                        //if (AllPackages)
                        //{
                        //    ZOVExpeditionManager.Filter(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.PackagesBindingSource.Current)["MainOrderID"]));
                        //}

                        int MainOrderID = Convert.ToInt32(((DataRowView)ZOVExpeditionManager.PackagesBS.Current)["MainOrderID"]);
                        int PackNumber = Convert.ToInt32(((DataRowView)ZOVExpeditionManager.PackagesBS.Current)["PackNumber"]);
                        int ProductType = Convert.ToInt32(((DataRowView)ZOVExpeditionManager.PackagesBS.Current)["ProductType"]);
                        CurrentMainOrder = MainOrderID;
                        CurrentPackNumber = PackNumber;

                        if (ProductType == 0)
                        {
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];
                            ZOVExpeditionManager.PackedMainOrdersFrontsOrders.MoveToFrontOrder(MainOrderID, PackNumber);
                            ZOVExpeditionManager.PackedMainOrdersFrontsOrders.SetColor(MainOrderID, PackNumber);
                        }
                        if (ProductType == 1)
                        {
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[1];
                            ZOVExpeditionManager.PackedMainOrdersDecorOrders.MoveToDecorOrder(MainOrderID, PackNumber);
                            ZOVExpeditionManager.PackedMainOrdersDecorOrders.SetColor(MainOrderID, PackNumber);
                        }
                    }
                }
        }

        private void MainOrdersDataGrid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Name == "PackPercentage")
            {
                e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            if (e.Column.Name == "DispPercentage")
            {
                e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }

        private void MegaOrdersDataGrid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Name == "PackPercentage")
            {
                e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            if (e.Column.Name == "DispPercentage")
            {
                e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (cbtnExpedition.Checked)
            {
                MenuPanel.BringToFront();
                MenuPanel.Visible = !MenuPanel.Visible;
            }
            if (cbtnDispatch.Checked)
            {
                MenuPanel1.BringToFront();
                MenuPanel1.Visible = !MenuPanel1.Visible;
            }
        }

        private void AllPackagesCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (AllPackagesCheckBox.Checked)
            {
                MainOrdersDataGrid.SelectionChanged -= MainOrdersDataGrid_SelectionChanged;

                if (NeedSplash)
                {
                    NeedSplash = false;
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    int FactoryID = -1;

                    if (ProfilCheckBox.Checked && TPSCheckBox.Checked)
                        FactoryID = 0;
                    if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                        FactoryID = 1;
                    if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                        FactoryID = 2;

                    ZOVExpeditionManager.FilterPackagesByMegaOrder(Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]), FactoryID);

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;

                    NeedSplash = true;
                }
            }
            else
            {
                MainOrdersDataGrid.SelectionChanged += MainOrdersDataGrid_SelectionChanged;
                MegaOrdersDataGrid_SelectionChanged(null, null);
            }

        }

        private void Filter()
        {
            if (NeedSplash)
            {
                bool bShowDispatched = ShowDispatchedCheckBox.Checked;

                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                int FactoryID = -1;

                if (ProfilCheckBox.Checked && TPSCheckBox.Checked)
                    FactoryID = 0;
                if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                    FactoryID = 1;
                if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                    FactoryID = 2;

                ZOVExpeditionManager.Filter(true, true, true, true, bShowDispatched, FactoryID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void ProfilCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ZOVExpeditionManager != null)
                Filter();
        }

        private void TPSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ZOVExpeditionManager != null)
                Filter();
        }

        private void ShowDispatchedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ZOVExpeditionManager != null)
                Filter();
        }

        private void SetDispatchedButton_Click(object sender, EventArgs e)
        {
            if (ZOVExpeditionManager != null)
                if (ZOVExpeditionManager.MegaOrdersBS.Count > 0)
                {
                    if (((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        int MegaOrderID = Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]);

                        if (NeedSplash)
                        {
                            bool bShowDispatched = ShowDispatchedCheckBox.Checked;

                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;
                            NeedSplash = false;

                            ZOVExpeditionManager.SetDispatched(MegaOrderID);

                            int FactoryID = -1;

                            if (ProfilCheckBox.Checked && TPSCheckBox.Checked)
                                FactoryID = 0;
                            if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                                FactoryID = 1;
                            if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                                FactoryID = 2;

                            ZOVExpeditionManager.Filter(true, true, true, true, bShowDispatched, FactoryID);
                            NeedSplash = true;
                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                        }
                    }
                }
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            ZOVExpeditionManager.SearchPackedOrders();
        }

        private void DocNumbersComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (DocNumbersComboBox.Items.Count > 0)
            {
                ZOVExpeditionManager.FindDocNumber(DocNumbersComboBox.SelectedValue.ToString());
            }
        }

        private void DocNumbersComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ZOVExpeditionManager.FindDocNumber(DocNumbersComboBox.SelectedValue.ToString());
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ZOVExpeditionManager.SearchPartDocNumber(SearchTextBox.Text);
        }

        private void SearchPartDocNumberComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (SearchPartDocNumberComboBox.Items.Count > 0)
                ZOVExpeditionManager.SearchDocNumber(Convert.ToInt32(SearchPartDocNumberComboBox.SelectedValue));
        }

        private void SearchDocNumberCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SearchDocNumberCheckBox.Checked)
            {
                //MenuPanel.SuspendLayout();
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                if (DocNumbersComboBox.DataSource == null)
                {
                    DocNumbersComboBox.DataSource = ZOVExpeditionManager.DocNumbersBS;
                    DocNumbersComboBox.DisplayMember = "DocNumber";
                    DocNumbersComboBox.ValueMember = "DocNumber";

                    SearchPartDocNumberComboBox.DataSource = ZOVExpeditionManager.SearchPartDocNumberBS;
                    SearchPartDocNumberComboBox.DisplayMember = "DocNumber";
                    SearchPartDocNumberComboBox.ValueMember = "MainOrderID";
                }

                this.MenuPanel.Size = new System.Drawing.Size(573, 384);
                DocNumberSearchPanel.Visible = true;

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                //MenuPanel.ResumeLayout();
            }
            else
            {
                this.MenuPanel.Size = new System.Drawing.Size(573, 72);
                DocNumberSearchPanel.Visible = false;
            }
        }

        private void MenuPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType != RoleTypes.AdminRole && RoleType != RoleTypes.LogisticsRole)
            {
                kryptonContextMenuItem3.Visible = false;
                kryptonContextMenuItem4.Visible = false;
                kryptonContextMenuItem11.Visible = false;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
                ZOVExpeditionManager.MainOrdersBS.Position = CurrentRowIndex;
                //int DebtTypeID = 0;

                //if (ZOVExpeditionManager.MainOrdersBS != null &&
                //    ((DataRowView)ZOVExpeditionManager.MainOrdersBS.Current != null))
                //    DebtTypeID = Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MainOrdersBS.Current).Row["DebtTypeID"]);

                //if (CanDispatchDebts)
                //    kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MainOrdersDecorOrdersDataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void ZOVExpeditionForm_Load(object sender, EventArgs e)
        {
            UpdateDispatchDate();
            MenuPanel.BringToFront();
            if (MoveFromPayments)
            {
                MenuButton.Location = MinimizeButton.Location;
                MinimizeButton.Visible = false;
                ZOVExpeditionManager.FindDocNumber(PaymentsDocNumber);
            }
            if (MoveFromPermits)
            {
                MenuButton.Location = MinimizeButton.Location;
                MinimizeButton.Visible = false;
                cbtnDispatch.Checked = true;
                ZOVDispatchManager.MoveToDispatchDate(PrepareDispatchDateTime);
                ZOVDispatchManager.MoveToDispatch(ZDispatchID);
            }
            if (!MoveFromPayments && !MoveFromPermits)
            {
                ZOVExpeditionManager.FilterMainOrdersByMegaOrder(
                    Convert.ToInt32(((DataRowView)ZOVExpeditionManager.MegaOrdersBS.Current)["MegaOrderID"]),
                    true, true, true, true, true, 0);
            }
            ZOVExpeditionManager.PackedMainOrdersFrontsOrders.SetColor(CurrentMainOrder, CurrentPackNumber);
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ZOVDispatchManager == null)
                return;
            if (kryptonCheckSet1.CheckedButton == cbtnExpedition)
            {
                flowLayoutPanel3.Visible = false;
                pnlExpedition.BringToFront();
                if (MenuButton.Checked)
                {
                    MenuPanel.BringToFront();
                    MenuPanel.Visible = cbtnExpedition.Checked;
                    MenuPanel1.Visible = cbtnDispatch.Checked;
                }
            }
            if (kryptonCheckSet1.CheckedButton == cbtnDispatch)
            {
                flowLayoutPanel3.Visible = true;
                pnlDispatch.BringToFront();
                if (MenuButton.Checked)
                {
                    MenuPanel1.BringToFront();
                    MenuPanel1.Visible = cbtnDispatch.Checked;
                    MenuPanel.Visible = cbtnExpedition.Checked;
                }
            }
        }

        private void dgvDispatchSetting()
        {
            dgvDispatch.DataSource = ZOVDispatchManager.DispatchList;

            foreach (DataGridViewColumn Column in dgvDispatch.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgvDispatch.AutoGenerateColumns = false;

            //dgvDispatch.Columns["DispatchID"].Visible = false;
            dgvDispatch.Columns["ClientID"].Visible = false;
            dgvDispatch.Columns["ConfirmExpUserID"].Visible = false;
            dgvDispatch.Columns["ConfirmDispUserID"].Visible = false;
            dgvDispatch.Columns["PrepareDispatchDateTime"].Visible = false;

            dgvDispatch.Columns["CreationDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvDispatch.Columns["ConfirmDispDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvDispatch.Columns["RealDispDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvDispatch.Columns["ClientName"].HeaderText = "Клиент";
            dgvDispatch.Columns["Weight"].HeaderText = "Вес, кг";
            dgvDispatch.Columns["CreationDateTime"].HeaderText = "   Дата\r\nсоздания";
            dgvDispatch.Columns["DispPackagesCount"].HeaderText = "  Кол-во\r\nупаковок";
            dgvDispatch.Columns["DispatchStatus"].HeaderText = "Статус";
            dgvDispatch.Columns["ConfirmExpDateTime"].HeaderText = "  Эксп-ция\r\nутверждена";
            dgvDispatch.Columns["ConfirmDispDateTime"].HeaderText = "  Отгрузка\r\nутверждена";
            dgvDispatch.Columns["ConfirmExpUser"].HeaderText = "Утвердил\r\nэксп-цию";
            dgvDispatch.Columns["ConfirmDispUser"].HeaderText = "Утвердил\r\n отгрузку";
            dgvDispatch.Columns["RealDispDateTime"].HeaderText = "Дата отгрузки";
            dgvDispatch.Columns["DispatchID"].HeaderText = "№ отгр.";

            dgvDispatch.Columns["DispatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDispatch.Columns["DispatchID"].Width = 80;
            dgvDispatch.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDispatch.Columns["Weight"].Width = 80;
            dgvDispatch.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDispatch.Columns["ClientName"].MinimumWidth = 200;
            dgvDispatch.Columns["RealDispDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDispatch.Columns["RealDispDateTime"].Width = 130;
            dgvDispatch.Columns["CreationDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDispatch.Columns["CreationDateTime"].Width = 130;
            dgvDispatch.Columns["DispPackagesCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDispatch.Columns["DispPackagesCount"].Width = 90;
            dgvDispatch.Columns["DispatchStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDispatch.Columns["DispatchStatus"].MinimumWidth = 200;
            dgvDispatch.Columns["ConfirmExpDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDispatch.Columns["ConfirmExpDateTime"].Width = 130;
            dgvDispatch.Columns["ConfirmExpUser"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDispatch.Columns["ConfirmExpUser"].MinimumWidth = 150;
            dgvDispatch.Columns["ConfirmDispDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDispatch.Columns["ConfirmDispDateTime"].Width = 130;
            dgvDispatch.Columns["ConfirmDispUser"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDispatch.Columns["ConfirmDispUser"].MinimumWidth = 150;
            //dgvDispatch.Columns["DispatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvDispatch.Columns["DispatchID"].Width = 1;

            int DisplayIndex = 0;
            dgvDispatch.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["CreationDateTime"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["DispPackagesCount"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["Weight"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["DispatchStatus"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["ConfirmExpDateTime"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["ConfirmExpUser"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["ConfirmDispDateTime"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["ConfirmDispUser"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["RealDispDateTime"].DisplayIndex = DisplayIndex++;
            dgvDispatch.Columns["DispatchID"].DisplayIndex = DisplayIndex++;

            dgvDispatch.Columns["DispPackagesCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void dgvDispatchDatesSetting()
        {
            dgvDispatchDates.DataSource = ZOVDispatchManager.DispatchDatesList;

            dgvDispatchDates.AutoGenerateColumns = false;

            if (dgvDispatchDates.Columns.Contains("PrepareDispatchDateTime"))
            {
                dgvDispatchDates.Columns["PrepareDispatchDateTime"].DefaultCellStyle.Format = "dd MMMM dddd";
                dgvDispatchDates.Columns["PrepareDispatchDateTime"].MinimumWidth = 150;
                dgvDispatchDates.Columns["PrepareDispatchDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvDispatchDates.Columns["PrepareDispatchDateTime"].DisplayIndex = 0;
            }
            if (dgvDispatchDates.Columns.Contains("WeekNumber"))
            {
                dgvDispatchDates.Columns["WeekNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvDispatchDates.Columns["WeekNumber"].Width = 70;
                dgvDispatchDates.Columns["WeekNumber"].DisplayIndex = 1;
            }
        }

        private void dgvMainOrdersSetting()
        {
            dgvMainOrders.DataSource = ZOVDispatchManager.DispatchContentList;

            foreach (DataGridViewColumn Column in dgvMainOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgvMainOrders.AutoGenerateColumns = false;

            if (dgvMainOrders.Columns.Contains("ClientID"))
            {
                dgvMainOrders.Columns["ClientID"].Visible = false;
            }
            if (dgvMainOrders.Columns.Contains("MegaOrderID"))
            {
                dgvMainOrders.Columns["MegaOrderID"].Visible = false;
            }
            if (dgvMainOrders.Columns.Contains("DocNumber"))
            {
                dgvMainOrders.Columns["DocNumber"].HeaderText = "№ документа";
                dgvMainOrders.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvMainOrders.Columns["DocNumber"].MinimumWidth = 100;
                dgvMainOrders.Columns["DocNumber"].DisplayIndex = 1;
            }
            if (dgvMainOrders.Columns.Contains("MainOrderID"))
            {
                dgvMainOrders.Columns["MainOrderID"].HeaderText = "№ подзаказа";
                dgvMainOrders.Columns["MainOrderID"].Width = 100;
                dgvMainOrders.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvMainOrders.Columns["MainOrderID"].DisplayIndex = 2;
            }
            if (dgvMainOrders.Columns.Contains("MainOrderID"))
            {
                dgvMainOrders.Columns["DoNotDispatch"].HeaderText = "   Отгрузка\r\nбез фасадов";
                dgvMainOrders.Columns["DoNotDispatch"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvMainOrders.Columns["DoNotDispatch"].Width = 105;
                dgvMainOrders.Columns["DoNotDispatch"].DisplayIndex = 3;
            }
            dgvMainOrders.Columns["MegaBatchID"].HeaderText = "Группа партий";
            dgvMainOrders.Columns["Square"].HeaderText = "Квадратура";
            dgvMainOrders.Columns["Weight"].HeaderText = "Вес";
            dgvMainOrders.Columns["AllPackCount"].HeaderText = "  Кол-во\r\nупаковок";
            dgvMainOrders.Columns["PackPercentage"].HeaderText = "Упаковано, %";
            dgvMainOrders.Columns["StorePercentage"].HeaderText = "Склад, %";
            dgvMainOrders.Columns["ExpPercentage"].HeaderText = "Экспедиция, %";
            dgvMainOrders.Columns["DispPercentage"].HeaderText = "Отгружено, %";

            dgvMainOrders.Columns["MegaBatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["MegaBatchID"].Width = 125;
            dgvMainOrders.Columns["AllPackCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["AllPackCount"].Width = 85;
            dgvMainOrders.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["Weight"].Width = 105;
            dgvMainOrders.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["Square"].Width = 105;

            dgvMainOrders.Columns["PackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("PackPercentage");
            dgvMainOrders.Columns["StorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("StorePercentage");
            dgvMainOrders.Columns["ExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("ExpPercentage");
            dgvMainOrders.Columns["DispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("DispPercentage");

        }

        private void UpdateDispatchDate()
        {
            DateTime FilterDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
            ZOVDispatchManager.ClearDispatchDates();
            ZOVDispatchManager.UpdateDispatchDates(FilterDate);
        }

        private void cbxMonths_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateDispatchDate();
        }

        private void cbxYears_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateDispatchDate();
        }

        private void dgvDispatch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDispatch.Rows.Count == 0)
                return;

            //if (RoleType != RoleTypes.AdminRole && RoleType != RoleTypes.LogisticsRole)
            //{
            //    CanEditDispatch = false;
            //}

            //Thread T = new Thread(delegate() { SplashWindow.CreateSplash(); });
            //T.Start();

            //while (!SplashForm.bCreated) ;
            //int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            //int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            //DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            //ZOVNewDispatchForm MarketingNewDispatchForm = new ZOVNewDispatchForm(CanEditDispatch, false, null, ClientID, DispatchID);

            //TopForm = MarketingNewDispatchForm;

            //MarketingNewDispatchForm.ShowDialog();

            //MarketingNewDispatchForm.Close();
            //MarketingNewDispatchForm.Dispose();

            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //NeedSplash = false;
            //UpdateDispatchDate();
            //ZOVDispatchManager.MoveToDispatchDate(DispatchDate);
            //ZOVDispatchManager.MoveToDispatch(DispatchID);
            //NeedSplash = true;
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;

            //TopForm = null;
        }

        private void dgvDispatch_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            switch (RoleType)
            {
                case RoleTypes.OrdinaryRole:
                    cmiChangeDispatchDate.Visible = false;
                    ConfirmExpContextMenuItem.Visible = false;
                    ConfirmDispatchContextMenuItem.Visible = false;
                    AttachContextMenuItem.Visible = false;
                    PrintDispatchListMenuItem.Visible = false;
                    cmiBindToPermit.Visible = false;
                    cmiRemoveDispatch.Visible = false;
                    break;
                case RoleTypes.AdminRole:
                    break;
                case RoleTypes.LogisticsRole:
                    ConfirmDispatchContextMenuItem.Visible = false;
                    break;
                case RoleTypes.ConfirmRole:
                    cmiChangeDispatchDate.Visible = false;
                    ConfirmExpContextMenuItem.Visible = false;
                    cmiBindToPermit.Visible = false;
                    cmiRemoveDispatch.Visible = false;
                    break;
                case RoleTypes.DispatchRole:
                    cmiChangeDispatchDate.Visible = false;
                    ConfirmExpContextMenuItem.Visible = false;
                    ConfirmDispatchContextMenuItem.Visible = false;
                    cmiBindToPermit.Visible = false;
                    cmiRemoveDispatch.Visible = false;
                    break;
                default:
                    break;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //if (dgvDispatch.Rows[e.RowIndex].Cells["ConfirmDispDateTime"].Value == DBNull.Value)
                //{
                //    if (dgvDispatch.Rows[e.RowIndex].Cells["ConfirmExpDateTime"].Value == DBNull.Value)
                //    {
                //        btnConfirmExpedition.Text = "Разрешить эксп-цию";
                //        ConfirmExpContextMenuItem.Text = "Разрешить эксп-цию";
                //    }
                //    else
                //    {
                //        btnConfirmExpedition.Text = "Запретить эксп-цию";
                //        ConfirmExpContextMenuItem.Text = "Запретить эксп-цию";
                //    }
                //    btnConfirmDispatch.Text = "Разрешить отгрузку";
                //    ConfirmDispatchContextMenuItem.Text = "Разрешить отгрузку";
                //}
                //else
                //{
                //    btnConfirmDispatch.Text = "Запретить отгрузку";
                //    ConfirmDispatchContextMenuItem.Text = "Запретить отгрузку";
                //}
                dgvDispatch.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvDispatch_SelectionChanged(object sender, EventArgs e)
        {
            if (ZOVDispatchManager == null)
                return;
            ZOVDispatchManager.ClearDispatchContent();
            if (dgvDispatch.SelectedRows.Count == 0)
            {
                return;
            }
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);

            //if (dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value != DBNull.Value
            //    && (RoleType == RoleTypes.AdminRole || RoleType == RoleTypes.LogisticsRole))
            //    CanEditDispatch = false;
            //else
            //    CanEditDispatch = true;

            if (RoleType != RoleTypes.OrdinaryRole && RoleType != RoleTypes.DispatchRole)
            {
                if (dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value == DBNull.Value)
                {
                    if (RoleType == RoleTypes.AdminRole || RoleType == RoleTypes.LogisticsRole)
                    {
                        cmiChangeDispatchDate.Visible = true;
                        btnChangeDispatchDate.Visible = true;
                        btnConfirmExpedition.Visible = true;
                        ConfirmExpContextMenuItem.Visible = true;
                    }
                    if (dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value == DBNull.Value)
                    {
                        btnConfirmDispatch.Visible = false;
                        ConfirmDispatchContextMenuItem.Visible = false;
                        btnConfirmExpedition.Text = "Разрешить эксп-цию";
                        ConfirmExpContextMenuItem.Text = "Разрешить эксп-цию";
                    }
                    else
                    {
                        if (RoleType == RoleTypes.AdminRole || RoleType == RoleTypes.ConfirmRole)
                        {
                            btnConfirmDispatch.Visible = true;
                            ConfirmDispatchContextMenuItem.Visible = true;
                        }
                        btnConfirmExpedition.Text = "Запретить эксп-цию";
                        ConfirmExpContextMenuItem.Text = "Запретить эксп-цию";
                    }
                    btnConfirmDispatch.Text = "Разрешить отгрузку";
                    ConfirmDispatchContextMenuItem.Text = "Разрешить отгрузку";
                }
                else
                {
                    cmiChangeDispatchDate.Visible = false;
                    btnChangeDispatchDate.Visible = false;
                    if (RoleType == RoleTypes.AdminRole || RoleType == RoleTypes.ConfirmRole)
                    {
                        btnConfirmDispatch.Visible = true;
                        ConfirmDispatchContextMenuItem.Visible = true;
                    }
                    btnConfirmExpedition.Visible = false;
                    ConfirmExpContextMenuItem.Visible = false;
                    if (dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value == DBNull.Value)
                    {
                        btnConfirmExpedition.Text = "Разрешить эксп-цию";
                        ConfirmExpContextMenuItem.Text = "Разрешить эксп-цию";
                    }
                    else
                    {
                        btnConfirmExpedition.Text = "Запретить эксп-цию";
                        ConfirmExpContextMenuItem.Text = "Запретить эксп-цию";
                    }
                    btnConfirmDispatch.Text = "Запретить отгрузку";
                    ConfirmDispatchContextMenuItem.Text = "Запретить отгрузку";
                }
            }

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ZOVDispatchManager.FilterDispatchContent(DispatchID);
                ZOVDispatchManager.FillPercColumns(DispatchID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                ZOVDispatchManager.FilterDispatchContent(DispatchID);
                ZOVDispatchManager.FillPercColumns(DispatchID);
            }
            if (dgvDispatch.SelectedRows.Count > 0 && dgvDispatch.SelectedRows[0].Cells["DispatchStatus"].Value.ToString() == "Отгружена")
            {
                btnConfirmDispatch.Visible = false;
                ConfirmDispatchContextMenuItem.Visible = false;
            }
        }

        private void dgvMainOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (RoleType != RoleTypes.AdminRole && RoleType != RoleTypes.LogisticsRole)
            //{
            //    CanEditDispatch = false;
            //}

            //Thread T = new Thread(delegate() { SplashWindow.CreateSplash(); });
            //T.Start();

            //while (!SplashForm.bCreated) ;
            //int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            //int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            int MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);
            int MegaOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MegaOrderID"].Value);
            //DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            //ZOVNewDispatchForm ZOVNewDispatchForm = new ZOVNewDispatchForm(CanEditDispatch, ClientID, DispatchID, MegaOrderID, MainOrderID);

            //TopForm = ZOVNewDispatchForm;

            //ZOVNewDispatchForm.ShowDialog();

            //ZOVNewDispatchForm.Close();
            //ZOVNewDispatchForm.Dispose();

            //Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            //T1.Start();

            //while (!SplashWindow.bSmallCreated) ;

            //NeedSplash = false;
            //UpdateDispatchDate();
            //ZOVDispatchManager.MoveToDispatchDate(DispatchDate);
            //ZOVDispatchManager.MoveToDispatch(DispatchID);
            //NeedSplash = true;
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;

            //TopForm = null;


            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            ZOVExpeditionManager.MoveToMegaOrder(MegaOrderID);
            ZOVExpeditionManager.MoveToMainOrder(MainOrderID);
            cbtnExpedition.Checked = true;
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

        }

        private void dgvDispatchDates_SelectionChanged(object sender, EventArgs e)
        {
            if (ZOVDispatchManager == null)
                return;
            ZOVDispatchManager.ClearDispatch();
            if (dgvDispatchDates.SelectedRows.Count == 0)
            {
                return;
            }

            bool bNotPacked = cbNotPackedDispatch.Checked;
            bool bPacked = cbPackedDispatch.Checked;
            bool bStore = cbStoreDispatch.Checked;
            bool bExp = cbExpDispatch.Checked;
            bool bDisp = cbDispDispatch.Checked;
            object Date = ZOVDispatchManager.CurrentDispatchDate;

            if (Date != DBNull.Value)
            {
                if (NeedSplash)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;
                    NeedSplash = false;
                    ZOVDispatchManager.GetMegaBatchNumbers(Convert.ToDateTime(Date));
                    ZOVDispatchManager.GetMainOrdersSquareAndWeight(Convert.ToDateTime(Date));
                    ZOVDispatchManager.FilterDispatchByDate(Convert.ToDateTime(Date), bNotPacked, bPacked, bStore, bExp, bDisp);
                    NeedSplash = true;
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
                else
                {
                    ZOVDispatchManager.GetMegaBatchNumbers(Convert.ToDateTime(Date));
                    ZOVDispatchManager.GetMainOrdersSquareAndWeight(Convert.ToDateTime(Date));
                    ZOVDispatchManager.FilterDispatchByDate(Convert.ToDateTime(Date), bNotPacked, bPacked, bStore, bExp, bDisp);
                }
            }
            if (dgvDispatch.SelectedRows.Count > 0 && dgvDispatch.SelectedRows[0].Cells["DispatchStatus"].Value.ToString() == "Отгружена")
            {
                btnConfirmDispatch.Visible = false;
                ConfirmDispatchContextMenuItem.Visible = false;
            }
        }

        //private void btnEditDispatch_Click(object sender, EventArgs e)
        //{
        //    if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
        //        return;
        //    int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
        //    int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
        //    DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

        //    Thread T = new Thread(delegate() { SplashWindow.CreateSplash(); });
        //    T.Start();

        //    while (!SplashForm.bCreated) ;
        //    ZOVNewDispatchForm NewDispatchForm = new ZOVNewDispatchForm(CanEditDispatch, false, null, ClientID, DispatchID);

        //    TopForm = NewDispatchForm;

        //    NewDispatchForm.ShowDialog();

        //    NewDispatchForm.Close();
        //    NewDispatchForm.Dispose();

        //    Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
        //    T1.Start();

        //    while (!SplashWindow.bSmallCreated) ;

        //    NeedSplash = false;
        //    UpdateDispatchDate();
        //    ZOVDispatchManager.MoveToDispatchDate(DispatchDate);
        //    ZOVDispatchManager.MoveToDispatch(DispatchID);
        //    NeedSplash = true;
        //    while (SplashWindow.bSmallCreated)
        //        SmallWaitForm.CloseS = true;

        //    TopForm = null;
        //}

        //private void btnAddDispatch_Click(object sender, EventArgs e)
        //{
        //    bool PressOK = false;
        //    int ClientID = 0;
        //    object DispatchDate = null;

        //    PhantomForm PhantomForm = new Infinium.PhantomForm();
        //    PhantomForm.Show();

        //    MarketingNewDispatchMenu NewDispatchMenu = new MarketingNewDispatchMenu(this, ZOVExpeditionManager.ClientsDT, false);
        //    TopForm = NewDispatchMenu;
        //    NewDispatchMenu.ShowDialog();

        //    PressOK = NewDispatchMenu.PressOK;
        //    ClientID = NewDispatchMenu.ClientID;
        //    DispatchDate = NewDispatchMenu.DispatchDate;

        //    PhantomForm.Close();
        //    PhantomForm.Dispose();
        //    NewDispatchMenu.Dispose();
        //    TopForm = null;

        //    if (!PressOK)
        //        return;

        //    Thread T = new Thread(delegate() { SplashWindow.CreateSplash(); });
        //    T.Start();

        //    while (!SplashForm.bCreated) ;
        //    int DispatchID = -1;

        //    ZOVNewDispatchForm NewDispatchForm = new ZOVNewDispatchForm(CanEditDispatch, true, DispatchDate, ClientID, DispatchID);

        //    TopForm = NewDispatchForm;

        //    NewDispatchForm.ShowDialog();

        //    NewDispatchForm.Close();
        //    NewDispatchForm.Dispose();

        //    Thread T1 = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
        //    T1.Start();

        //    while (!SplashWindow.bSmallCreated) ;

        //    NeedSplash = false;
        //    UpdateDispatchDate();
        //    NeedSplash = true;
        //    while (SplashWindow.bSmallCreated)
        //        SmallWaitForm.CloseS = true;

        //    TopForm = null;
        //}

        private void btnConfirmDispatch_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            //int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            //if (!ZOVDispatchManager.HasPackages(DispatchID))
            //{
            //    InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
            //    return;
            //}

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            bool Confirm = false;
            if (dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value == DBNull.Value)
                Confirm = true;

            int[] DispatchID = new int[dgvDispatch.SelectedRows.Count];
            for (int i = 0; i < dgvDispatch.SelectedRows.Count; i++)
            {
                if (dgvDispatch.SelectedRows[i].Cells["ConfirmExpDateTime"].Value != DBNull.Value)
                    DispatchID[i] = Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value);
            }

            ZOVDispatchManager.SaveConfirmDispInfo(DispatchID, Confirm);
            UpdateDispatchDate();
            ZOVDispatchManager.MoveToDispatchDate(DispatchDate);
            //ZOVDispatchManager.MoveToDispatch(DispatchID);
            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnConfirmExpedition_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;

            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            //if (!ZOVDispatchManager.HasPackages(DispatchID))
            //{
            //    InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
            //    return;
            //}

            bool Confirm = false;
            if (dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value == DBNull.Value)
                Confirm = true;

            //if (Confirm && !ZOVDispatchManager.IsDispatchCanExp(DispatchID))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Запрещено: не вся продукция принята на склад.",
            //            "Разрешить экспедицию");
            //    return;
            //}

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            int[] DispatchID = new int[dgvDispatch.SelectedRows.Count];
            for (int i = 0; i < dgvDispatch.SelectedRows.Count; i++)
            {
                DispatchID[i] = Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value);
            }
            ZOVDispatchManager.SaveConfirmExpInfo(DispatchID, Confirm);
            UpdateDispatchDate();
            ZOVDispatchManager.MoveToDispatchDate(DispatchDate);
            //ZOVDispatchManager.MoveToDispatch(DispatchID);
            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnChangeDispatchDate_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0)
                return;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            object DispatchDate = null;

            bool PressOK = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingNewDispatchMenu MarketingNewDispatchMenu = new MarketingNewDispatchMenu(this, ZOVExpeditionManager.ClientsDT, true);
            TopForm = MarketingNewDispatchMenu;
            MarketingNewDispatchMenu.ShowDialog();

            PressOK = MarketingNewDispatchMenu.PressOK;
            DispatchDate = MarketingNewDispatchMenu.DispatchDate;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingNewDispatchMenu.Dispose();
            TopForm = null;

            if (PressOK && DispatchDate != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                NeedSplash = false;
                ZOVDispatchManager.ChangeDispatchDate(DispatchID, DispatchDate);
                UpdateDispatchDate();
                ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(DispatchDate));
                ZOVDispatchManager.MoveToDispatch(DispatchID);
                NeedSplash = true;

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void cmiChangeDispatchDate_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0)
                return;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            object DispatchDate = null;

            bool PressOK = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingNewDispatchMenu MarketingNewDispatchMenu = new MarketingNewDispatchMenu(this, ZOVExpeditionManager.ClientsDT, true);
            TopForm = MarketingNewDispatchMenu;
            MarketingNewDispatchMenu.ShowDialog();

            PressOK = MarketingNewDispatchMenu.PressOK;
            DispatchDate = MarketingNewDispatchMenu.DispatchDate;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingNewDispatchMenu.Dispose();
            TopForm = null;

            if (PressOK && DispatchDate != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                NeedSplash = false;
                ZOVDispatchManager.ChangeDispatchDate(DispatchID, DispatchDate);
                UpdateDispatchDate();
                ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(DispatchDate));
                ZOVDispatchManager.MoveToDispatch(DispatchID);
                NeedSplash = true;

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void ConfirmExpContextMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;

            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            //if (!ZOVDispatchManager.HasPackages(DispatchID))
            //{
            //    InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
            //    return;
            //}

            bool Confirm = false;
            if (dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value == DBNull.Value)
                Confirm = true;

            //if (Confirm && !ZOVDispatchManager.IsDispatchCanExp(DispatchID))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Запрещено: не вся продукция принята на склад.",
            //            "Разрешить экспедицию");
            //    return;
            //}

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            NeedSplash = false;
            int[] DispatchID = new int[dgvDispatch.SelectedRows.Count];
            for (int i = 0; i < dgvDispatch.SelectedRows.Count; i++)
            {
                DispatchID[i] = Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value);
            }
            ZOVDispatchManager.SaveConfirmExpInfo(DispatchID, Confirm);
            UpdateDispatchDate();
            ZOVDispatchManager.MoveToDispatchDate(DispatchDate);

            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            if (kryptonContextMenuItem15.Text == "Разрешить экспедицию")
                kryptonContextMenuItem15.Text = "Запретить экспедицию";
            else
                kryptonContextMenuItem15.Text = "Разрешить экспедицию";
        }

        private void ConfirmDispatchContextMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            //int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            //if (!ZOVDispatchManager.HasPackages(DispatchID))
            //{
            //    InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
            //    return;
            //}

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            bool Confirm = false;

            int[] DispatchID = new int[dgvDispatch.SelectedRows.Count];
            for (int i = 0; i < dgvDispatch.SelectedRows.Count; i++)
            {
                if (dgvDispatch.SelectedRows[i].Cells["ConfirmExpDateTime"].Value != DBNull.Value)
                    DispatchID[i] = Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value);
            }

            ZOVDispatchManager.SaveConfirmDispInfo(DispatchID, Confirm);
            UpdateDispatchDate();
            ZOVDispatchManager.MoveToDispatchDate(DispatchDate);
            //ZOVDispatchManager.MoveToDispatch(DispatchID);
            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            if (kryptonContextMenuItem16.Text == "Разрешить отгрузку")
                kryptonContextMenuItem16.Text = "Запретить отгрузку";
            else
                kryptonContextMenuItem16.Text = "Разрешить отгрузку";
        }

        private void ProfilPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            bool NeedProfilList = true;
            bool NeedTPSList = false;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            if (!ZOVDispatchManager.HasPackages(DispatchID))
            {
                InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
                return;
            }

            bool PressOK = false;
            object MachineName = DBNull.Value;
            object PermitNumber = DBNull.Value;
            object SealNumber = DBNull.Value;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            MachineName = MarketingDispatchInfoMenu.MachineName;
            PermitNumber = MarketingDispatchInfoMenu.PermitNumber;
            SealNumber = MarketingDispatchInfoMenu.SealNumber;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            object CreationDateTime = dgvDispatch.SelectedRows[0].Cells["CreationDateTime"].Value;
            object ConfirmExpDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value;
            object ConfirmDispDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value;
            object PrepareDispDateTime = dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;
            object ConfirmExpUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmExpUserID"].Value;
            object ConfirmDispUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmDispUserID"].Value;
            object RealDispDateTime = DBNull.Value;
            object DispUserID = DBNull.Value;
            ZOVDispatchManager.GetRealDispDateTime(DispatchID, ref RealDispDateTime, ref DispUserID);

            DispatchReport.GetDispatchInfo(ref CreationDateTime, ref ConfirmExpDateTime, ref ConfirmDispDateTime, ref RealDispDateTime, ref PrepareDispDateTime,
                ref ConfirmExpUserID, ref ConfirmDispUserID, ref DispUserID, ref MachineName, ref PermitNumber, ref SealNumber);
            //DispatchReport.CurrentClient = ClientID;
            DispatchReport.CurrentDispatchDateTime = PrepareDispDateTime;
            DispatchReport.Initialize();
            DispatchReport.CreateReport(NeedProfilList, NeedTPSList, false, PrepareDispDateTime);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TPSPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            bool NeedProfilList = false;
            bool NeedTPSList = true;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            if (!ZOVDispatchManager.HasPackages(DispatchID))
            {
                InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
                return;
            }

            bool PressOK = false;
            object MachineName = DBNull.Value;
            object PermitNumber = DBNull.Value;
            object SealNumber = DBNull.Value;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            MachineName = MarketingDispatchInfoMenu.MachineName;
            PermitNumber = MarketingDispatchInfoMenu.PermitNumber;
            SealNumber = MarketingDispatchInfoMenu.SealNumber;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            object CreationDateTime = dgvDispatch.SelectedRows[0].Cells["CreationDateTime"].Value;
            object ConfirmExpDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value;
            object ConfirmDispDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value;
            object PrepareDispDateTime = dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;
            object ConfirmExpUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmExpUserID"].Value;
            object ConfirmDispUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmDispUserID"].Value;
            object RealDispDateTime = DBNull.Value;
            object DispUserID = DBNull.Value;
            ZOVDispatchManager.GetRealDispDateTime(DispatchID, ref RealDispDateTime, ref DispUserID);

            DispatchReport.GetDispatchInfo(ref CreationDateTime, ref ConfirmExpDateTime, ref ConfirmDispDateTime, ref RealDispDateTime, ref PrepareDispDateTime,
                ref ConfirmExpUserID, ref ConfirmDispUserID, ref DispUserID, ref MachineName, ref PermitNumber, ref SealNumber);
            //DispatchReport.CurrentClient = ClientID;
            DispatchReport.CurrentDispatchDateTime = PrepareDispDateTime;
            DispatchReport.Initialize();
            DispatchReport.CreateReport(NeedProfilList, NeedTPSList, false, PrepareDispDateTime);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void AllPrintContextMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            bool NeedProfilList = true;
            bool NeedTPSList = true;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            if (!ZOVDispatchManager.HasPackages(DispatchID))
            {
                InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
                return;
            }

            bool PressOK = false;
            object MachineName = DBNull.Value;
            object PermitNumber = DBNull.Value;
            object SealNumber = DBNull.Value;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            MachineName = MarketingDispatchInfoMenu.MachineName;
            PermitNumber = MarketingDispatchInfoMenu.PermitNumber;
            SealNumber = MarketingDispatchInfoMenu.SealNumber;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            object CreationDateTime = dgvDispatch.SelectedRows[0].Cells["CreationDateTime"].Value;
            object ConfirmExpDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value;
            object ConfirmDispDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value;
            object PrepareDispDateTime = dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;
            object ConfirmExpUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmExpUserID"].Value;
            object ConfirmDispUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmDispUserID"].Value;
            object RealDispDateTime = DBNull.Value;
            object DispUserID = DBNull.Value;
            ZOVDispatchManager.GetRealDispDateTime(DispatchID, ref RealDispDateTime, ref DispUserID);

            DispatchReport.GetDispatchInfo(ref CreationDateTime, ref ConfirmExpDateTime, ref ConfirmDispDateTime, ref RealDispDateTime, ref PrepareDispDateTime,
                ref ConfirmExpUserID, ref ConfirmDispUserID, ref DispUserID, ref MachineName, ref PermitNumber, ref SealNumber);
            //DispatchReport.CurrentClient = ClientID;
            DispatchReport.CurrentDispatchDateTime = PrepareDispDateTime;
            DispatchReport.Initialize();
            DispatchReport.CreateReport(NeedProfilList, NeedTPSList, false, PrepareDispDateTime);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ProfilAttach_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            bool NeedProfilList = true;
            bool NeedTPSList = false;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            if (!ZOVDispatchManager.HasPackages(DispatchID))
            {
                InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
                return;
            }

            bool PressOK = false;
            object MachineName = DBNull.Value;
            object PermitNumber = DBNull.Value;
            object SealNumber = DBNull.Value;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            MachineName = MarketingDispatchInfoMenu.MachineName;
            PermitNumber = MarketingDispatchInfoMenu.PermitNumber;
            SealNumber = MarketingDispatchInfoMenu.SealNumber;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            object CreationDateTime = dgvDispatch.SelectedRows[0].Cells["CreationDateTime"].Value;
            object ConfirmExpDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value;
            object ConfirmDispDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value;
            object PrepareDispDateTime = dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;
            object ConfirmExpUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmExpUserID"].Value;
            object ConfirmDispUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmDispUserID"].Value;
            object RealDispDateTime = DBNull.Value;
            object DispUserID = DBNull.Value;
            ZOVDispatchManager.GetRealDispDateTime(DispatchID, ref RealDispDateTime, ref DispUserID);

            DispatchReport.GetDispatchInfo(ref CreationDateTime, ref ConfirmExpDateTime, ref ConfirmDispDateTime, ref RealDispDateTime, ref PrepareDispDateTime,
                ref ConfirmExpUserID, ref ConfirmDispUserID, ref DispUserID, ref MachineName, ref PermitNumber, ref SealNumber);
            //DispatchReport.CurrentClient = ClientID;
            DispatchReport.CurrentDispatchDateTime = PrepareDispDateTime;
            DispatchReport.Initialize();
            DispatchReport.CreateReport(NeedProfilList, NeedTPSList, true, PrepareDispDateTime);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TPSAttach_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            bool NeedProfilList = false;
            bool NeedTPSList = true;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            if (!ZOVDispatchManager.HasPackages(DispatchID))
            {
                InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
                return;
            }

            bool PressOK = false;
            object MachineName = DBNull.Value;
            object PermitNumber = DBNull.Value;
            object SealNumber = DBNull.Value;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            MachineName = MarketingDispatchInfoMenu.MachineName;
            PermitNumber = MarketingDispatchInfoMenu.PermitNumber;
            SealNumber = MarketingDispatchInfoMenu.SealNumber;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            object CreationDateTime = dgvDispatch.SelectedRows[0].Cells["CreationDateTime"].Value;
            object ConfirmExpDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value;
            object ConfirmDispDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value;
            object PrepareDispDateTime = dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;
            object ConfirmExpUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmExpUserID"].Value;
            object ConfirmDispUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmDispUserID"].Value;
            object RealDispDateTime = DBNull.Value;
            object DispUserID = DBNull.Value;
            ZOVDispatchManager.GetRealDispDateTime(DispatchID, ref RealDispDateTime, ref DispUserID);

            DispatchReport.GetDispatchInfo(ref CreationDateTime, ref ConfirmExpDateTime, ref ConfirmDispDateTime, ref RealDispDateTime, ref PrepareDispDateTime,
                ref ConfirmExpUserID, ref ConfirmDispUserID, ref DispUserID, ref MachineName, ref PermitNumber, ref SealNumber);
            //DispatchReport.CurrentClient = ClientID;
            DispatchReport.CurrentDispatchDateTime = PrepareDispDateTime;
            DispatchReport.Initialize();
            DispatchReport.CreateReport(NeedProfilList, NeedTPSList, true, PrepareDispDateTime);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void AllAttach_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            bool NeedProfilList = true;
            bool NeedTPSList = true;
            int ClientID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["ClientID"].Value);
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            if (!ZOVDispatchManager.HasPackages(DispatchID))
            {
                InfiniumTips.ShowTip(this, 50, 85, "Отгрузка пуста", 1700);
                return;
            }

            bool PressOK = false;
            object MachineName = DBNull.Value;
            object PermitNumber = DBNull.Value;
            object SealNumber = DBNull.Value;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingDispatchInfoMenu MarketingDispatchInfoMenu = new MarketingDispatchInfoMenu(this);
            TopForm = MarketingDispatchInfoMenu;
            MarketingDispatchInfoMenu.ShowDialog();

            PressOK = MarketingDispatchInfoMenu.PressOK;
            MachineName = MarketingDispatchInfoMenu.MachineName;
            PermitNumber = MarketingDispatchInfoMenu.PermitNumber;
            SealNumber = MarketingDispatchInfoMenu.SealNumber;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MarketingDispatchInfoMenu.Dispose();
            TopForm = null;

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            object CreationDateTime = dgvDispatch.SelectedRows[0].Cells["CreationDateTime"].Value;
            object ConfirmExpDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value;
            object ConfirmDispDateTime = dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value;
            object PrepareDispDateTime = dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value;
            object ConfirmExpUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmExpUserID"].Value;
            object ConfirmDispUserID = dgvDispatch.SelectedRows[0].Cells["ConfirmDispUserID"].Value;
            object RealDispDateTime = DBNull.Value;
            object DispUserID = DBNull.Value;
            ZOVDispatchManager.GetRealDispDateTime(DispatchID, ref RealDispDateTime, ref DispUserID);

            DispatchReport.GetDispatchInfo(ref CreationDateTime, ref ConfirmExpDateTime, ref ConfirmDispDateTime, ref RealDispDateTime, ref PrepareDispDateTime,
                ref ConfirmExpUserID, ref ConfirmDispUserID, ref DispUserID, ref MachineName, ref PermitNumber, ref SealNumber);
            //DispatchReport.CurrentClient = ClientID;
            DispatchReport.CurrentDispatchDateTime = PrepareDispDateTime;
            DispatchReport.Initialize();
            DispatchReport.CreateReport(NeedProfilList, NeedTPSList, true, PrepareDispDateTime);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            bCheckBoxShow = true;
            kryptonContextMenuItem1.Visible = false;
            kryptonContextMenuItem2.Visible = true;
            ZOVExpeditionManager.ShowMainOrdersCheckBoxColumn(bCheckBoxShow);
            ZOVExpeditionManager.ShowPackagesCheckBoxColumn(bCheckBoxShow);
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            bCheckBoxShow = false;
            kryptonContextMenuItem1.Visible = true;
            kryptonContextMenuItem2.Visible = false;
            ZOVExpeditionManager.ShowMainOrdersCheckBoxColumn(bCheckBoxShow);
            ZOVExpeditionManager.ShowPackagesCheckBoxColumn(bCheckBoxShow);
        }

        private void MainOrdersDataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PercentageDataGrid DataGrid = (PercentageDataGrid)sender;
            DataGridViewColumn newColumn = DataGrid.Columns[e.ColumnIndex];
            if (e.RowIndex == -1 && newColumn.Name == "CheckBoxColumn")
            {
                DataGridViewColumn oldColumn = DataGrid.SortedColumn;
                ListSortDirection direction;

                if (oldColumn != null)
                {
                    if (oldColumn == newColumn &&
                        DataGrid.SortOrder == SortOrder.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                        oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                }
                else
                {
                    direction = ListSortDirection.Ascending;
                }

                DataGrid.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private void PackagesDataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PercentageDataGrid DataGrid = (PercentageDataGrid)sender;
            DataGridViewColumn newColumn = DataGrid.Columns[e.ColumnIndex];
            if (e.RowIndex == -1 && newColumn.Name == "CheckBoxColumn")
            {
                DataGridViewColumn oldColumn = DataGrid.SortedColumn;
                ListSortDirection direction;

                if (oldColumn != null)
                {
                    if (oldColumn == newColumn &&
                        DataGrid.SortOrder == SortOrder.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                        oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                }
                else
                {
                    direction = ListSortDirection.Ascending;
                }

                DataGrid.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private void MainOrdersDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (MainOrdersDataGrid.Columns[e.ColumnIndex].Name == "CheckBoxColumn" && e.RowIndex != -1)
            {
                MainOrdersDataGrid.EndEdit();
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)MainOrdersDataGrid.
                    Rows[e.RowIndex].Cells["CheckBoxColumn"];
                int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.Rows[e.RowIndex].Cells["MainOrderID"].Value);

                ZOVExpeditionManager.FlagPackages(MainOrderID, Convert.ToBoolean(checkCell.Value));
            }
        }

        private void btnToExistingDispatch_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            int ClientID = 0;
            int DispatchID = 0;
            int MainOrderID = 0;
            if (dgvDispatch.SelectedRows.Count > 0)
                DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);

            int ArrayCount = 0;
            for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
            {
                if (MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value != DBNull.Value
                    && Convert.ToBoolean(MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value))
                    ArrayCount++;
            }
            if (ArrayCount > 0)
            {
                for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
                {
                    if (MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value != DBNull.Value
                        && Convert.ToBoolean(MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value))
                    {
                        ClientID = Convert.ToInt32(MainOrdersDataGrid.Rows[i].Cells["ClientID"].Value);
                        MainOrderID = Convert.ToInt32(MainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                        ZOVExpeditionManager.SavePackages(MainOrderID, DispatchID);
                    }
                }
            }

            btnToExistingDispatch.Visible = false;
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Добавлено", 1700);

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            UpdateDispatchDate();
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            TopForm = null;
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            //if (!ZOVExpeditionManager.IsPackagesCheck())
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Не выбрана ни одна упаковка",
            //            "Проверка");
            //    return;
            //}

            if (MainOrdersDataGrid.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);
            int DispatchID = 0;

            //if (ZOVDispatchManager.IsDoNotDispatch(MainOrderID))
            //{
            //    bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //        "Подзаказ помечен как \"Отгрузка без фасадов\". Продолжить?",
            //        "Проверка");

            //    if (!OKCancel)
            //        return;
            //}

            if (ZOVDispatchManager.IsMainOrderInDispatch(MainOrderID, ref DispatchID))
            {
                object PrepareDispatchDateTime = ZOVDispatchManager.GetPrepareDispatchDateTime(DispatchID);
                if (PrepareDispatchDateTime != DBNull.Value)
                {
                    Thread T2 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T2.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    NeedSplash = false;

                    ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(PrepareDispatchDateTime));
                    ZOVDispatchManager.MoveToDispatch(DispatchID);
                    ZOVDispatchManager.MoveToMainOrder(MainOrderID);
                    cbtnDispatch.Checked = true;

                    NeedSplash = true;

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;

                    Infinium.LightMessageBox.Show(ref TopForm, false,
                            "Подзаказ частично либо полностью включен в отгрузку",
                            "Проверка");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            int ClientID = 0;
            object DispatchDate = MegaOrdersDataGrid.SelectedRows[0].Cells["DispatchDate"].Value;

            int ArrayCount = 0;
            for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
            {
                if (MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value != DBNull.Value
                    && Convert.ToBoolean(MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value))
                    ArrayCount++;
            }
            if (ArrayCount > 0)
            {
                for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
                {
                    if (MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value != DBNull.Value
                        && Convert.ToBoolean(MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value))
                    {
                        ClientID = Convert.ToInt32(MainOrdersDataGrid.Rows[i].Cells["ClientID"].Value);
                        MainOrderID = Convert.ToInt32(MainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);
                        bool DoNotDispatch = Convert.ToBoolean(MainOrdersDataGrid.Rows[i].Cells["DoNotDispatch"].Value);
                        if (DoNotDispatch)
                        {
                            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                                "Подзаказ помечен как \"Отгрузка без фасадов\". Продолжить?",
                                "Проверка");

                            if (!OKCancel)
                                return;
                        }
                        ZOVDispatchManager.AddDispatch(ClientID, DispatchDate);
                        DispatchID = ZOVDispatchManager.MaxDispatchID();
                        ZOVExpeditionManager.SavePackages(MainOrderID, DispatchID);
                    }
                }
            }

            btnToExistingDispatch.Visible = false;
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            //InfiniumTips.ShowTip(this, 50, 85, "Добавлено", 1700);

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            UpdateDispatchDate();
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            TopForm = null;
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            //if (!ZOVExpeditionManager.IsPackagesCheck())
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Не выбрана ни одна упаковка",
            //            "Проверка");
            //    return;
            //}

            if (MainOrdersDataGrid.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);
            int DispatchID = 0;

            //if (ZOVDispatchManager.IsDoNotDispatch(MainOrderID))
            //{
            //    bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //        "Подзаказ помечен как \"Отгрузка без фасадов\". Продолжить?",
            //        "Проверка");

            //    if (!OKCancel)
            //        return;
            //}

            if (ZOVDispatchManager.IsMainOrderInDispatch(MainOrderID, ref DispatchID))
            {
                object PrepareDispatchDateTime = ZOVDispatchManager.GetPrepareDispatchDateTime(DispatchID);
                if (PrepareDispatchDateTime != DBNull.Value)
                {
                    Thread T2 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T2.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    NeedSplash = false;

                    ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(PrepareDispatchDateTime));
                    ZOVDispatchManager.MoveToDispatch(DispatchID);
                    ZOVDispatchManager.MoveToMainOrder(MainOrderID);
                    cbtnDispatch.Checked = true;

                    NeedSplash = true;

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;

                    Infinium.LightMessageBox.Show(ref TopForm, false,
                            "Подзаказ частично либо полностью включен в отгрузку",
                            "Проверка");
                    return;
                }
            }

            cbtnDispatch.Checked = true;
            btnToExistingDispatch.Visible = true;
        }

        private void cmiRemoveDispatch_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0)
                return;
            int DispatchID = Convert.ToInt32(dgvDispatch.SelectedRows[0].Cells["DispatchID"].Value);

            //if (!ZOVDispatchManager.IsDispatchEmpty(DispatchID))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Удаление запрещено, т.к. отгрузка не пуста",
            //            "Удаление отгрузки");
            //    return;
            //} 
            //if (dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value != DBNull.Value)
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Удаление запрещено, т.к. отгрузка уже утверждена",
            //            "Удаление отгрузки");
            //    return;
            //}
            //bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //        "Вы собираетесь удалить отгрузку. Продолжить?",
            //        "Удаление отгрузки");

            //if (!OKCancel)
            //    return;
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление отгрузки.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            ZOVDispatchManager.RemoveDispatch(DispatchID);
            UpdateDispatchDate();
            ZOVDispatchManager.MoveToDispatchDate(DispatchDate);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            if (MainOrdersDataGrid.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);
            int DispatchID = 0;

            if (ZOVDispatchManager.IsMainOrderInDispatch(MainOrderID, ref DispatchID))
            {
                object PrepareDispatchDateTime = ZOVDispatchManager.GetPrepareDispatchDateTime(DispatchID);
                if (PrepareDispatchDateTime != DBNull.Value)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    NeedSplash = false;

                    ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(PrepareDispatchDateTime));
                    ZOVDispatchManager.MoveToDispatch(DispatchID);
                    ZOVDispatchManager.MoveToMainOrder(MainOrderID);
                    cbtnDispatch.Checked = true;

                    NeedSplash = true;

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
            }
            else
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Подзаказ не включен в отгрузку",
                        "Поиск отгрузки");
                return;
            }
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {
            bCheckBoxShow = true;
            kryptonContextMenuItem6.Visible = false;
            kryptonContextMenuItem7.Visible = true;
            ZOVExpeditionManager.ShowPackagesCheckBoxColumn(bCheckBoxShow);
        }

        private void kryptonContextMenuItem7_Click(object sender, EventArgs e)
        {
            bCheckBoxShow = false;
            kryptonContextMenuItem6.Visible = true;
            kryptonContextMenuItem7.Visible = false;
            ZOVExpeditionManager.ShowPackagesCheckBoxColumn(bCheckBoxShow);
        }

        private void kryptonContextMenuItem8_Click(object sender, EventArgs e)
        {
            //if (!ZOVExpeditionManager.IsPackagesCheck())
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Не выбрана ни одна упаковка",
            //            "Проверка");
            //    return;
            //}

            if (MainOrdersDataGrid.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);
            int PackageID = Convert.ToInt32(PackagesDataGrid.SelectedRows[0].Cells["PackageID"].Value);
            int DispatchID = 0;

            if (ZOVDispatchManager.IsDoNotDispatch(MainOrderID))
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Подзаказ помечен как \"Отгрузка без фасадов\". Продолжить?",
                    "Проверка");

                if (!OKCancel)
                    return;
            }

            if (ZOVDispatchManager.IsPackageInDispatch(PackageID, ref DispatchID))
            {
                object PrepareDispatchDateTime = ZOVDispatchManager.GetPrepareDispatchDateTime(DispatchID);
                if (PrepareDispatchDateTime != DBNull.Value)
                {
                    Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T1.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    NeedSplash = false;

                    ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(PrepareDispatchDateTime));
                    ZOVDispatchManager.MoveToDispatch(DispatchID);
                    ZOVDispatchManager.MoveToMainOrder(MainOrderID);
                    cbtnDispatch.Checked = true;

                    NeedSplash = true;

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;

                    Infinium.LightMessageBox.Show(ref TopForm, false,
                            "Упаковка включена в отгрузку",
                            "Поиск отгрузки");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            int ClientID = 0;
            object DispatchDate = MegaOrdersDataGrid.SelectedRows[0].Cells["DispatchDate"].Value;

            if (MainOrdersDataGrid.SelectedRows.Count > 0)
                ClientID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["ClientID"].Value);
            ZOVDispatchManager.AddDispatch(ClientID, DispatchDate);
            DispatchID = ZOVDispatchManager.MaxDispatchID();
            ZOVExpeditionManager.SavePackages(DispatchID);

            btnToExistingDispatch.Visible = false;
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Добавлено", 1700);

            Thread T2 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T2.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            UpdateDispatchDate();
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            TopForm = null;
        }

        private void kryptonContextMenuItem9_Click(object sender, EventArgs e)
        {
            //if (!ZOVExpeditionManager.IsPackagesCheck())
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //            "Не выбрана ни одна упаковка",
            //            "Проверка");
            //    return;
            //}

            if (MainOrdersDataGrid.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);
            int PackageID = Convert.ToInt32(PackagesDataGrid.SelectedRows[0].Cells["PackageID"].Value);
            int DispatchID = 0;

            if (ZOVDispatchManager.IsDoNotDispatch(MainOrderID))
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Подзаказ помечен как \"Отгрузка без фасадов\". Продолжить?",
                    "Проверка");

                if (!OKCancel)
                    return;
            }

            if (ZOVDispatchManager.IsPackageInDispatch(PackageID, ref DispatchID))
            {
                object PrepareDispatchDateTime = ZOVDispatchManager.GetPrepareDispatchDateTime(DispatchID);
                if (PrepareDispatchDateTime != DBNull.Value)
                {
                    Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T1.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    NeedSplash = false;

                    ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(PrepareDispatchDateTime));
                    ZOVDispatchManager.MoveToDispatch(DispatchID);
                    ZOVDispatchManager.MoveToMainOrder(MainOrderID);
                    cbtnDispatch.Checked = true;

                    NeedSplash = true;

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;

                    cbtnDispatch.Checked = true;
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                            "Упаковка включена в отгрузку",
                            "Поиск отгрузки");
                    return;
                }
            }

            cbtnDispatch.Checked = true;
            btnToExistingDispatch.Visible = true;
        }

        private void kryptonContextMenuItem10_Click(object sender, EventArgs e)
        {
            if (MainOrdersDataGrid.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);
            int PackageID = Convert.ToInt32(PackagesDataGrid.SelectedRows[0].Cells["PackageID"].Value);
            int DispatchID = 0;

            if (ZOVDispatchManager.IsPackageInDispatch(PackageID, ref DispatchID))
            {
                object PrepareDispatchDateTime = ZOVDispatchManager.GetPrepareDispatchDateTime(DispatchID);
                if (PrepareDispatchDateTime != DBNull.Value)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    NeedSplash = false;

                    ZOVDispatchManager.MoveToDispatchDate(Convert.ToDateTime(PrepareDispatchDateTime));
                    ZOVDispatchManager.MoveToDispatch(DispatchID);
                    ZOVDispatchManager.MoveToMainOrder(MainOrderID);
                    cbtnDispatch.Checked = true;

                    NeedSplash = true;

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
            }
            else
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Упаковка не включена в отгрузку",
                        "Поиск отгрузки");
                return;
            }
        }

        private void PackagesDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType != RoleTypes.AdminRole && RoleType != RoleTypes.LogisticsRole)
            {
                kryptonContextMenuItem9.Visible = false;
                kryptonContextMenuItem8.Visible = false;
                kryptonContextMenuItem12.Visible = false;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ZOVExpeditionManager.PackagesBS.Position = e.RowIndex;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem11_Click(object sender, EventArgs e)
        {
            if (MainOrdersDataGrid.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Подзаказ будет исключен из отгрузки. Продолжить?",
                "Удаление подзаказа");

            if (!OKCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            bool IsOK = ZOVDispatchManager.ExcludeMainOrderFromDispatch(MainOrderID);
            UpdateDispatchDate();

            ZOVExpeditionManager.FlagPackages(MainOrderID, false);
            MainOrdersDataGrid.SelectedRows[0].Cells["CheckBoxColumn"].Value = false;
            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            if (IsOK)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Подзаказ исключен из отгрузки",
                        "Удаление подзаказа");
            }
        }

        private void kryptonContextMenuItem12_Click(object sender, EventArgs e)
        {
            if (PackagesDataGrid.SelectedRows.Count == 0)
                return;
            int PackageID = Convert.ToInt32(PackagesDataGrid.SelectedRows[0].Cells["PackageID"].Value);

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Упаковка будет исключена из отгрузки. Продолжить?",
                "Удаление упаковки");

            if (!OKCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            bool IsOK = ZOVDispatchManager.ExcludePackageFromDispatch(PackageID);
            UpdateDispatchDate();
            PackagesDataGrid.SelectedRows[0].Cells["CheckBoxColumn"].Value = false;
            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            if (IsOK)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Упаковка исключена из отгрузки",
                        "Удаление упаковки");
            }
        }

        private void kryptonContextMenuItem17_Click(object sender, EventArgs e)
        {
            int MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);
            int MegaOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MegaOrderID"].Value);

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            ZOVExpeditionManager.MoveToMegaOrder(MegaOrderID);
            ZOVExpeditionManager.MoveToMainOrder(MainOrderID);
            cbtnExpedition.Checked = true;
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem18_Click(object sender, EventArgs e)
        {
            if (dgvMainOrders.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Подзаказ будет исключен из отгрузки. Продолжить?",
                "Удаление подзаказа");

            if (!OKCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            bool IsOK = ZOVDispatchManager.ExcludeMainOrderFromDispatch(MainOrderID);
            UpdateDispatchDate();

            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            if (IsOK)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Подзаказ исключен из отгрузки",
                        "Удаление подзаказа");
            }
        }

        private void dgvMainOrders_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dgvMainOrders.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem13_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
            {
                MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value = true;
            }
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
            {
                MainOrdersDataGrid.Rows[i].Cells["CheckBoxColumn"].Value = false;
            }
        }

        private void dgvDispatchDates_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((RoleType != RoleTypes.OrdinaryRole) && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dgvDispatchDates.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenu6_Opening(object sender, CancelEventArgs e)
        {
            if (RoleType == RoleTypes.AdminRole)
            {
                kryptonContextMenuItem15.Visible = true;
                kryptonContextMenuItem16.Visible = true;
                kryptonContextMenuItem29.Visible = true;
                kryptonContextMenuItem30.Visible = true;
            }
            if (RoleType == RoleTypes.ConfirmRole)
            {
                kryptonContextMenuItem16.Visible = true;
                kryptonContextMenuItem29.Visible = true;
                kryptonContextMenuItem30.Visible = true;
            }
            if (RoleType == RoleTypes.DispatchRole)
            {

            }
            if (RoleType == RoleTypes.LogisticsRole)
            {
                kryptonContextMenuItem15.Visible = true;
            }
        }

        private void kryptonContextMenuItem15_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;

            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            bool Confirm = false;
            if (dgvDispatch.SelectedRows[0].Cells["ConfirmExpDateTime"].Value == DBNull.Value)
                Confirm = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            ZOVDispatchManager.SaveConfirmExpInfo(DispatchDate, Confirm);
            UpdateDispatchDate();
            ZOVDispatchManager.MoveToDispatchDate(DispatchDate);

            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            if (kryptonContextMenuItem15.Text == "Разрешить экспедицию")
                kryptonContextMenuItem15.Text = "Запретить экспедицию";
            else
                kryptonContextMenuItem15.Text = "Разрешить экспедицию";
        }

        private void kryptonContextMenuItem16_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0 || dgvDispatchDates.SelectedRows.Count == 0)
                return;
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            bool Confirm = false;
            if (dgvDispatch.SelectedRows[0].Cells["ConfirmDispDateTime"].Value == DBNull.Value)
                Confirm = true;

            ZOVDispatchManager.SaveConfirmDispInfo(DispatchDate, Confirm);
            UpdateDispatchDate();
            ZOVDispatchManager.MoveToDispatchDate(DispatchDate);

            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MegaOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.AdminRole || RoleType == RoleTypes.LogisticsRole)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    CurrentRowIndex = e.RowIndex;
                    ZOVExpeditionManager.MegaOrdersBS.Position = CurrentRowIndex;
                    kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
                }
            }
        }

        private void kryptonContextMenuItem19_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                int MegaOrderID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value);

                CheckOrdersStatus.FilterMainOrdersByMegaOrder(MegaOrderID,
                    true, true, true, true, false, 0);
                ZOVExpeditionManager.DispatchDebts(MegaOrderID);
            }
            NeedSplash = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cmiDispatchPackages_Click(object sender, EventArgs e)
        {
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);
            if (!ZOVDispatchManager.IsDispatchBindToPermit(DispatchDate))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Необходимо привязать отгрузку к пропуску.",
                    "Привязка к пропуску");
                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;

                bool BindingOk = false;

                PermitsForm PermitsForm = new PermitsForm(this, DispatchDate);

                TopForm = PermitsForm;

                PermitsForm.ShowDialog();
                BindingOk = PermitsForm.BindingOk;
                PermitsForm.Close();
                PermitsForm.Dispose();

                TopForm = null;
                //if (BindingOk)
                //    InfiniumTips.ShowTip(this, 50, 85, "Пропуск привязан", 1700);
            }

            ArrayList DispatchIDs = new ArrayList();
            string s = string.Empty;
            for (int i = 0; i < dgvDispatch.Rows.Count; i++)
            {
                if (dgvDispatch.Rows[i].Cells["ConfirmDispDateTime"].Value == DBNull.Value)
                {
                    s += Convert.ToInt32(dgvDispatch.Rows[i].Cells["DispatchID"].Value) + ",";
                    continue;
                }
                DispatchIDs.Add(Convert.ToInt32(dgvDispatch.Rows[i].Cells["DispatchID"].Value));
            }
            if (DispatchIDs.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Ни одна отгрузка не может быть отгружена.",
                    "Ошибка");
                return;
            }
            if (s.Length > 0)
            {
                s = "Следующие отгрузки не могут быть отгружены, так как не утверждены: " + s.Substring(0, s.Length - 1);
                Infinium.LightMessageBox.Show(ref TopForm, false, s, "Ошибка");
            }
            Thread T1 = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T1.Start();

            while (!SplashForm.bCreated) ;

            ZOVDispatchLabelCheckForm DispatchLabelCheckForm = new ZOVDispatchLabelCheckForm(this, DispatchIDs);

            TopForm = DispatchLabelCheckForm;

            DispatchLabelCheckForm.ShowDialog();

            DispatchLabelCheckForm.Close();
            DispatchLabelCheckForm.Dispose();

            TopForm = null;
        }

        private void cmiBindToPermit_Click(object sender, EventArgs e)
        {
            if (dgvDispatch.SelectedRows.Count == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            bool BindingOk = false;
            object Date = ZOVDispatchManager.CurrentDispatchDate;
            PermitsForm PermitsForm = new PermitsForm(this, Date);

            TopForm = PermitsForm;

            PermitsForm.ShowDialog();
            BindingOk = PermitsForm.BindingOk;
            PermitsForm.Close();
            PermitsForm.Dispose();

            TopForm = null;
            if (BindingOk)
                InfiniumTips.ShowTip(this, 50, 85, "Пропуск привязан", 1700);
        }

        private void kryptonContextMenuItem28_Click(object sender, EventArgs e)
        {
            ArrayList DispatchIDs = new ArrayList();
            string s = string.Empty;
            for (int i = 0; i < dgvDispatch.SelectedRows.Count; i++)
            {
                if (dgvDispatch.SelectedRows[i].Cells["ConfirmDispDateTime"].Value == DBNull.Value)
                {
                    s += Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value) + ",";
                    continue;
                }
                DispatchIDs.Add(Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value));
            }
            if (DispatchIDs.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Ни одна отгрузка не может быть отгружена.",
                    "Ошибка");
                return;
            }
            if (s.Length > 0)
            {
                s = "Следующие отгрузки не могут быть отгружены, так как не утверждены: " + s.Substring(0, s.Length - 1);
                Infinium.LightMessageBox.Show(ref TopForm, false, s, "Ошибка");
            }
            Thread T1 = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T1.Start();

            while (!SplashForm.bCreated) ;

            ZOVDispatchLabelCheckForm DispatchLabelCheckForm = new ZOVDispatchLabelCheckForm(this, DispatchIDs);

            TopForm = DispatchLabelCheckForm;

            DispatchLabelCheckForm.ShowDialog();

            DispatchLabelCheckForm.Close();
            DispatchLabelCheckForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem29_Click_1(object sender, EventArgs e)
        {
            int[] DispatchID = new int[dgvDispatch.Rows.Count];
            if (DispatchID.Count() == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            Infinium.Modules.ZOV.ReportToDBF.ReportToDBF DBFReport = new Modules.ZOV.ReportToDBF.ReportToDBF();
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);
            decimal TotalCost = 0;
            for (int i = 0; i < dgvDispatch.Rows.Count; i++)
            {
                DispatchID[i] = Convert.ToInt32(dgvDispatch.Rows[i].Cells["DispatchID"].Value);
            }
            if (DispatchID.Count() > 0)
            {
                DBFReport.NotSampleOrdersReport(DispatchDate, DispatchID, TotalCost, 1);
                DBFReport.SampleOrdersReport(DispatchDate, DispatchID, TotalCost, 1);
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem30_Click_1(object sender, EventArgs e)
        {
            int[] DispatchID = new int[dgvDispatch.Rows.Count];
            if (DispatchID.Count() == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            Infinium.Modules.ZOV.ReportToDBF.ReportToDBF DBFReport = new Modules.ZOV.ReportToDBF.ReportToDBF();
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);
            decimal TotalCost = 0;
            for (int i = 0; i < dgvDispatch.Rows.Count; i++)
            {
                DispatchID[i] = Convert.ToInt32(dgvDispatch.Rows[i].Cells["DispatchID"].Value);
            }
            if (DispatchID.Count() > 0)
            {
                DBFReport.NotSampleOrdersReport(DispatchDate, DispatchID, TotalCost, 5);
                DBFReport.SampleOrdersReport(DispatchDate, DispatchID, TotalCost, 5);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem31_Click(object sender, EventArgs e)
        {
            int[] DispatchID = new int[dgvDispatch.Rows.Count];
            if (DispatchID.Count() == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            Infinium.Modules.ZOV.ReportToDBF.ReportToDBF DBFReport = new Modules.ZOV.ReportToDBF.ReportToDBF();
            DateTime DispatchDate = Convert.ToDateTime(dgvDispatchDates.SelectedRows[0].Cells["PrepareDispatchDateTime"].Value);
            decimal TotalCost = 0;
            for (int i = 0; i < dgvDispatch.Rows.Count; i++)
            {
                DispatchID[i] = Convert.ToInt32(dgvDispatch.Rows[i].Cells["DispatchID"].Value);
            }
            if (DispatchID.Count() > 0)
            {
                DBFReport.AllOrdersReport(DispatchDate, DispatchID, TotalCost, 5);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem32_Click(object sender, EventArgs e)
        {
            bool ColVisible = false;
            if (PackagesDataGrid.SelectedRows.Count > 0 && PackagesDataGrid.SelectedRows[0].Cells["PackUsersColumn"].Value != DBNull.Value)
                ColVisible = Convert.ToBoolean(PackagesDataGrid.SelectedRows[0].Cells["PackUsersColumn"].Visible);
            ZOVExpeditionManager.ShowPackUsersColumn(!ColVisible);
        }

        private void kryptonContextMenuItem33_Click(object sender, EventArgs e)
        {
            bool ColVisible = false;
            if (PackagesDataGrid.SelectedRows.Count > 0 && PackagesDataGrid.SelectedRows[0].Cells["StoreUsersColumn"].Value != DBNull.Value)
                ColVisible = Convert.ToBoolean(PackagesDataGrid.SelectedRows[0].Cells["StoreUsersColumn"].Visible);
            ZOVExpeditionManager.ShowStoreUsersColumn(!ColVisible);
        }

        private void kryptonContextMenuItem34_Click(object sender, EventArgs e)
        {
            bool ColVisible = false;
            if (PackagesDataGrid.SelectedRows.Count > 0 && PackagesDataGrid.SelectedRows[0].Cells["ExpUsersColumn"].Value != DBNull.Value)
                ColVisible = Convert.ToBoolean(PackagesDataGrid.SelectedRows[0].Cells["ExpUsersColumn"].Visible);
            ZOVExpeditionManager.ShowExpUsersColumn(!ColVisible);
        }

        private void kryptonContextMenuItem35_Click(object sender, EventArgs e)
        {
            bool ColVisible = false;
            if (PackagesDataGrid.SelectedRows.Count > 0 && PackagesDataGrid.SelectedRows[0].Cells["DispUsersColumn"].Value != DBNull.Value)
                ColVisible = Convert.ToBoolean(PackagesDataGrid.SelectedRows[0].Cells["DispUsersColumn"].Visible);
            ZOVExpeditionManager.ShowDispUsersColumn(!ColVisible);
        }

        public int[] GetSelectedPackages()
        {
            System.Collections.ArrayList array = new System.Collections.ArrayList();

            for (int i = 0; i < PackagesDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(PackagesDataGrid.SelectedRows[i].Cells["PackageID"].Value));

            int[] rows = array.OfType<int>().ToArray();
            Array.Sort(rows);

            return rows;
        }

        private void kryptonButton1_Click_1(object sender, EventArgs e)
        {
            CheckOrdersStatus.SetToDispatchZOV(kryptonMonthCalendar1.SelectionStart, GetSelectedPackages(),
                Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value));
        }

        private void kryptonContextMenuCheckButton1_Click(object sender, EventArgs e)
        {
            int[] Dispatches = new int[dgvDispatch.SelectedRows.Count];
            for (int i = 0; i < dgvDispatch.SelectedRows.Count; i++)
                Dispatches[i] = Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value);
            if (Dispatches.Count() > 0)
            {
                ZOVExpeditionManager.ChangeDispatchDate(Dispatches, kryptonContextMenuMonthCalendar2.SelectionStart);
                UpdateDispatchDate();
            }
        }

        private void kryptonContextMenuCheckButton2_Click(object sender, EventArgs e)
        {
            int[] Dispatches = new int[dgvDispatch.SelectedRows.Count];
            for (int i = 0; i < dgvDispatch.SelectedRows.Count; i++)
                Dispatches[i] = Convert.ToInt32(dgvDispatch.SelectedRows[i].Cells["DispatchID"].Value);
            if (Dispatches.Count() > 0)
            {
                ZOVExpeditionManager.ChangeDispatchDate(Dispatches, kryptonContextMenuMonthCalendar2.SelectionStart);
                UpdateDispatchDate();
            }
        }

    }
}