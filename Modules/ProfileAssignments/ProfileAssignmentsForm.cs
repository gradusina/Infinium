using ComponentFactory.Krypton.Toolkit;

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ProfileAssignmentsForm : Form
    {
        const int iAdmin = 64;
        const int iToolsConfirm = 65;
        const int iTechnologyConfirm = 66;
        const int iMaterialConfirm = 67;
        const int iTechnicalConfirm = 68;
        const int iBatchEnable = 69;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;

        int FormEvent = 0;
        int PanelActive = 0;

        //DataTable RolePermissionsDataTable;
        Bitmap Lock_BW = new Bitmap(Properties.Resources.LockSmallBlack);
        Bitmap Unlock_BW = new Bitmap(Properties.Resources.UnlockSmallBlack);
        ImageList ImageList1;
        LightStartForm LightStartForm;
        Form TopForm = null;

        ProfileAssignmentsToExcel DecorlAssignmentsToExcel;
        ProfileAssignments ProfileAssignmentsManager;

        RoleTypes RoleType = RoleTypes.Admin;
        //Connection Connection;
        //Security Security = null;

        public enum RoleTypes
        {
            Ordinary = 0,
            Admin = 1,
            ToolsConfirm = 2,
            TechnologyConfirm = 3,
            MaterialConfirm = 4,
            TechnicalConfirm = 5,
            BatchEnable = 6
        }

        public ProfileAssignmentsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;
            panel62.BackColor = Security.GridsBackColor;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            //Connection = new Connection();

            //ConnectionStrings.CatalogConnectionString = Connection.GetConnectionString(CommonVariables.CatalogConnectionString);
            //ConnectionStrings.LightConnectionString = Connection.GetConnectionString(CommonVariables.LightConnectionString);
            //ConnectionStrings.MarketingOrdersConnectionString = Connection.GetConnectionString(CommonVariables.MarketingOrdersConnectionString);
            //ConnectionStrings.MarketingReferenceConnectionString = Connection.GetConnectionString(CommonVariables.MarketingReferenceConnectionString);
            //ConnectionStrings.StorageConnectionString = Connection.GetConnectionString(CommonVariables.StorageConnectionString);
            //ConnectionStrings.UsersConnectionString = Connection.GetConnectionString(CommonVariables.UsersConnectionString);
            //ConnectionStrings.ZOVOrdersConnectionString = Connection.GetConnectionString(CommonVariables.ZOVOrdersConnectionString);
            //ConnectionStrings.ZOVReferenceConnectionString = Connection.GetConnectionString(CommonVariables.ZOVReferenceConnectionString);

            //Security = new Infinium.Security();
            //if (!Security.Initialize())
            //{
            //    MessageBox.Show("Не удалось подключится к базе данных. Возможные причины: не работает сервер баз данных, нет доступа к сети или интернет. Обратитесь к системному администратору");
            //    this.Close();
            //    Application.Exit();
            //    return;
            //}

            //Security.Enter(322, "gradus");

            ImageList1 = new ImageList()
            {

                // (the default is 16 x 16).
                ImageSize = new Size(24, 24)
            };
            ImageList1.Images.Add(Lock_BW);
            ImageList1.Images.Add(Unlock_BW);

            Initialize();

            //RolePermissionsDataTable = ProfileAssignmentsManager.GetPermissions(Security.CurrentUserID, this.Name);

            //if (PermissionGranted(iAdmin))
            //{
            //    RoleType = RoleTypes.Admin;
            //}
            //if (PermissionGranted(iToolsConfirm))
            //{
            //    RoleType = RoleTypes.ToolsConfirm;
            //    kryptonContextMenuItem53.Visible = false;
            //    kryptonContextMenuItem42.Visible = false;
            //    kryptonContextMenuItem43.Visible = false;
            //    kryptonContextMenuItem44.Visible = false;
            //    kryptonContextMenuItem45.Visible = false;
            //    kryptonContextMenuItem2.Visible = false;
            //    kryptonContextMenuItem4.Visible = false;
            //}
            //if (PermissionGranted(iTechnologyConfirm))
            //{
            //    RoleType = RoleTypes.TechnologyConfirm;
            //    kryptonContextMenuItem53.Visible = false;
            //    kryptonContextMenuItem42.Visible = false;
            //    kryptonContextMenuItem43.Visible = false;
            //    kryptonContextMenuItem45.Visible = false;
            //    kryptonContextMenuItem46.Visible = false;
            //    kryptonContextMenuItem2.Visible = false;
            //    kryptonContextMenuItem4.Visible = false;
            //}
            //if (PermissionGranted(iMaterialConfirm))
            //{
            //    RoleType = RoleTypes.MaterialConfirm;
            //    kryptonContextMenuItem53.Visible = false;
            //    kryptonContextMenuItem42.Visible = false;
            //    kryptonContextMenuItem43.Visible = false;
            //    kryptonContextMenuItem44.Visible = false;
            //    kryptonContextMenuItem46.Visible = false;
            //    kryptonContextMenuItem2.Visible = false;
            //    kryptonContextMenuItem4.Visible = false;
            //}
            //if (PermissionGranted(iTechnicalConfirm))
            //{
            //    RoleType = RoleTypes.TechnicalConfirm;
            //    kryptonContextMenuItem53.Visible = false;
            //    kryptonContextMenuItem42.Visible = false;
            //    kryptonContextMenuItem43.Visible = false;
            //    kryptonContextMenuItem44.Visible = false;
            //    kryptonContextMenuItem45.Visible = false;
            //    kryptonContextMenuItem46.Visible = false;
            //    kryptonContextMenuItem4.Visible = false;
            //}
            //if (PermissionGranted(iBatchEnable))
            //{
            //    RoleType = RoleTypes.BatchEnable;
            //    kryptonContextMenuItem53.Visible = false;
            //    kryptonContextMenuItem42.Visible = false;
            //    kryptonContextMenuItem43.Visible = false;
            //    kryptonContextMenuItem44.Visible = false;
            //    kryptonContextMenuItem45.Visible = false;
            //    kryptonContextMenuItem46.Visible = false;
            //    kryptonContextMenuItem2.Visible = false;
            //}
            //if (RoleType == RoleTypes.Ordinary)
            //{
            //    kryptonContextMenuItem53.Visible = false;
            //    kryptonContextMenuItem41.Visible = false;
            //    kryptonContextMenuItem42.Visible = false;
            //    kryptonContextMenuItem43.Visible = false;
            //    kryptonContextMenuItem44.Visible = false;
            //    kryptonContextMenuItem45.Visible = false;
            //    kryptonContextMenuItem46.Visible = false;
            //    kryptonContextMenuItem2.Visible = false;
            //    kryptonContextMenuItem4.Visible = false;
            //}

            while (!SplashForm.bCreated) ;
        }

        private void ProfileAssignmentsForm_Shown(object sender, EventArgs e)
        {
            if (PanelActive == 0)
            {
                pnlEnveloping.BringToFront();
                pnlBatchAssignmentsMainPage.BringToFront();
            }
            if (PanelActive == 1)
            {
                //pnlEnveloping.BringToFront();
                pnlBatchAssignmentsBrowsing.BringToFront();
            }
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            NeedSplash = true;
            if (kryptonCheckSet1.CheckedButton == cbtnHolzma)
            {
                pnlCutting.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnBarberan)
            {
                pnlEnveloping.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnFrezer)
            {
                pnlMeeling.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnAssembly)
            {
                pnlAssembly.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnKashir)
            {
                pnlLaminating.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnPaperCutting)
            {
                pnlPaperCutting.BringToFront();
            }
        }

        //private bool PermissionGranted(int RoleID)
        //{
        //    DataRow[] Rows = RolePermissionsDataTable.Select("RoleID = " + RoleID);

        //    return Rows.Count() > 0;
        //}

        private void ProfileAssignmentsForm_Load(object sender, EventArgs e)
        {
            //ProfileAssignmentsManager.GetMdfPlateOnStorage();
            //ProfileAssignmentsManager.GroupMdfPlate();
            //ProfileAssignmentsManager.GetMilledProfilesOnStorage();
            //ProfileAssignmentsManager.GroupMilledProfiles();
            //ProfileAssignmentsManager.GetSawnStripsOnStorage();
            //ProfileAssignmentsManager.GroupSawnStrips();
            //ProfileAssignmentsManager.GetShroudedProfilesOnStorage();
            //ProfileAssignmentsManager.GroupShroudedProfiles();
            //FilterAssignments();
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
                        NeedSplash = false;
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
                        NeedSplash = false;
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void Initialize()
        {
            DecorlAssignmentsToExcel = new Infinium.ProfileAssignmentsToExcel();
            DecorlAssignmentsToExcel.Initialize();

            ProfileAssignmentsManager = new Infinium.ProfileAssignments();
            ProfileAssignmentsManager.Initialize();
            DataBinding();

            CommonGridAssignmentsSetting(ref dgvMilledProfileAssignments1);
            CommonGridAssignmentsSetting(ref dgvMilledProfileAssignments2);
            CommonGridAssignmentsSetting(ref dgvMilledProfileAssignments3);
            CommonGridRequestsSetting(ref dgvMilledProfileRequests);
            CommonGridAssignmentsSetting(ref dgvShroudedProfileAssignments1);
            CommonGridAssignmentsSetting(ref dgvShroudedProfileAssignments2);
            CommonGridRequestsSetting(ref dgvShroudedProfileRequests);
            CommonGridAssignmentsSetting(ref dgvSawnStripsAssignments);
            CommonGridRequestsSetting(ref dgvSawnStripsRequests);
            CommonGridAssignmentsSetting(ref dgvAssembledProfileAssignments);
            CommonGridRequestsSetting(ref dgvAssembledProfileRequests);
            CommonGridAssignmentsSetting(ref dgvKashirAssignments);
            CommonGridRequestsSetting(ref dgvKashirRequests);

            dgvFacingMaterialAssignmentsSetting(ref dgvFacingMaterialAssignments);
            dgvFacingRollersAssignmentsSetting(ref dgvFacingRollersAssignments);

            dgvFacingMaterialOnStorageSetting(ref dgvFacingMaterialOnStorage);
            dgvFacingRollersOnStorageSetting(ref dgvFacingRollersOnStorage);

            dgvBatchAssignmentsSetting(ref dgvBatchAssignments);
            dgvMdfPlateOnStorageSetting(ref dgvMdfPlateOnStorage);

            dgvMilledProfileAssignmentsSetting(ref dgvMilledProfileAssignments1);
            dgvMilledProfileAssignmentsSetting(ref dgvMilledProfileAssignments2);
            dgvMilledProfileAssignmentsSetting(ref dgvMilledProfileAssignments3);
            dgvMilledProfilesOnStorageSetting(ref dgvMilledProfilesOnStorage1);
            dgvMilledProfilesOnStorageSetting(ref dgvMilledProfilesOnStorage2);
            dgvMilledProfileRequestsSetting(ref dgvMilledProfileRequests);

            dgvSawnStripsAssignmentSetting(ref dgvSawnStripsAssignments);
            dgvSawnStripsOnStorageSetting(ref dgvSawnStripsOnStorage1);
            dgvSawnStripsOnStorageSetting(ref dgvSawnStripsOnStorage2);
            dgvSawnStripsRequestsSetting(ref dgvSawnStripsRequests);

            dgvShroudedProfileAssignmentsSetting(ref dgvShroudedProfileAssignments1);
            dgvShroudedProfileAssignmentsSetting(ref dgvShroudedProfileAssignments2);
            dgvShroudedProfilesOnStorageSetting(ref dgvShroudedProfilesOnStorage);
            dgvShroudedProfieRequestsSetting(ref dgvShroudedProfileRequests);

            dgvAssembledProfileAssignmentsSetting(ref dgvAssembledProfileAssignments);
            dgvMilledProfilesOnStorageSetting(ref dgvMilledProfilesOnStorage3);
            dgvAssembledProfieRequestsSetting(ref dgvAssembledProfileRequests);

            dgvKashirAssignmentSetting(ref dgvKashirAssignments);
            dgvKashirOnStorageSetting(ref dgvKashirOnStorage);
            dgvSawnStripsOnStorageSetting(ref dgvSawnStripsOnStorage3);
            dgvKashirRequestsSetting(ref dgvKashirRequests);

            dgvFacingRollersRequestsSetting(ref dgvFacingRollersRequests);
            dgvFacingMaterialRequestsSetting(ref dgvFacingMaterialRequests);
        }

        private void DataBinding()
        {
            cbFilterFacingMaterial.DataSource = ProfileAssignmentsManager.FilterFacingMaterialList;
            cbFilterFacingMaterial.ValueMember = "TechStoreName";
            cbFilterFacingMaterial.DisplayMember = "TechStoreName";

            cbFilterFacingRollers.DataSource = ProfileAssignmentsManager.FilterFacingRollersList;
            cbFilterFacingRollers.ValueMember = "TechStoreName";
            cbFilterFacingRollers.DisplayMember = "TechStoreName";

            dgvBatchAssignments.DataSource = ProfileAssignmentsManager.BatchAssignmentsList;

            dgvFacingMaterialAssignments.DataSource = ProfileAssignmentsManager.FacingMaterialAssignmentsList;
            dgvFacingRollersAssignments.DataSource = ProfileAssignmentsManager.FacingRollersAssignmentsList;

            dgvFacingMaterialOnStorage.DataSource = ProfileAssignmentsManager.FacingMaterialOnStorageList;
            dgvFacingRollersOnStorage.DataSource = ProfileAssignmentsManager.FacingRollersOnStorageList;

            dgvMdfPlateOnStorage.DataSource = ProfileAssignmentsManager.MdfPlateOnStorageList;

            dgvMilledProfileAssignments1.DataSource = ProfileAssignmentsManager.MilledProfileAssignmentsList1;
            dgvMilledProfilesOnStorage1.DataSource = ProfileAssignmentsManager.MilledProfilesOnStorageList;
            dgvMilledProfilesOnStorage2.DataSource = ProfileAssignmentsManager.MilledProfilesOnStorageList;
            dgvMilledProfileRequests.DataSource = ProfileAssignmentsManager.MilledProfileRequestsList;

            //dgvPackingAssignments.DataSource = ProfileAssignmentsManager.PackingAssignmentsList;
            //dgvPackingRequests.DataSource = ProfileAssignmentsManager.PackingRequestsList;

            dgvKashirAssignments.DataSource = ProfileAssignmentsManager.KashirAssignmentsList;
            dgvSawnStripsOnStorage3.DataSource = ProfileAssignmentsManager.SawnStripsOnStorageList;
            dgvKashirOnStorage.DataSource = ProfileAssignmentsManager.KashirOnStorageList;
            dgvKashirRequests.DataSource = ProfileAssignmentsManager.KashirRequestsList;

            dgvSawnStripsAssignments.DataSource = ProfileAssignmentsManager.SawnStripsAssignmentsList;
            dgvSawnStripsOnStorage1.DataSource = ProfileAssignmentsManager.SawnStripsOnStorageList;
            dgvSawnStripsOnStorage2.DataSource = ProfileAssignmentsManager.SawnStripsOnStorageList;
            dgvSawnStripsRequests.DataSource = ProfileAssignmentsManager.SawStripsRequestsList;

            dgvShroudedProfileAssignments1.DataSource = ProfileAssignmentsManager.ShroudedProfileAssignmentsList1;
            dgvShroudedProfilesOnStorage.DataSource = ProfileAssignmentsManager.ShroudedProfilesOnStorageList;
            dgvShroudedProfileRequests.DataSource = ProfileAssignmentsManager.ShroudedProfileRequestsList;

            dgvAssembledProfileAssignments.DataSource = ProfileAssignmentsManager.AssembledProfileAssignmentsList;
            dgvMilledProfilesOnStorage3.DataSource = ProfileAssignmentsManager.MilledProfilesOnStorageList;
            dgvAssembledProfileRequests.DataSource = ProfileAssignmentsManager.AssembledProfileRequestsList;

            dgvMilledProfileAssignments2.DataSource = ProfileAssignmentsManager.MilledProfileAssignmentsList2;
            dgvMilledProfileAssignments3.DataSource = ProfileAssignmentsManager.MilledProfileAssignmentsList3;
            dgvShroudedProfileAssignments2.DataSource = ProfileAssignmentsManager.ShroudedProfileAssignmentsList2;

            dgvFacingRollersRequests.DataSource = ProfileAssignmentsManager.FacingRollersRequestsList;
            dgvFacingMaterialRequests.DataSource = ProfileAssignmentsManager.FacingMaterialRequestsList;
        }

        #region GridSettings

        private void CommonGridAssignmentsSetting(ref PercentageDataGrid grid)
        {
            if (grid.Columns.Contains("SaveToStore"))
                grid.Columns["SaveToStore"].Visible = false;
            if (grid.Columns.Contains("WriteOffFromStore"))
                grid.Columns["WriteOffFromStore"].Visible = false;
            if (grid.Columns.Contains("SawName"))
                grid.Columns["SawName"].Visible = false;
            if (grid.Columns.Contains("FacingMachine"))
                grid.Columns["FacingMachine"].Visible = false;
            if (grid.Columns.Contains("ProfilOrderStatusID"))
                grid.Columns["ProfilOrderStatusID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            //if (grid.Columns.Contains("DecorAssignmentID"))
            //    grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("AddToPlanDateTime"))
                grid.Columns["AddToPlanDateTime"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("NextLinkAssignmentID"))
                grid.Columns["NextLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("MegaOrderID"))
                grid.Columns["MegaOrderID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.MinimumWidth = 50;
            }

            grid.Columns["DecorAssignmentID"].HeaderText = "№";
            grid.Columns["MegaOrderID"].HeaderText = "Заказ";
            grid.Columns["MainOrderID"].HeaderText = "Подзаказ";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["AddToPlanDateTime"].HeaderText = "Включено\r\n  в план";
            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["Height2"].HeaderText = "Высота";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Length1"].HeaderText = "Длина";
            grid.Columns["Width1"].HeaderText = "Ширина";
            grid.Columns["Count1"].HeaderText = "Листы";
            grid.Columns["ComplexSawing"].HeaderText = "Сложный\r\n  распил";
            grid.Columns["SawName"].HeaderText = "     Толщина\r\nпильного диска";
            grid.Columns["Thickness2"].HeaderText = "Толщина";
            grid.Columns["Diameter2"].HeaderText = "Диаметр";

            grid.Columns["DecorAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DecorAssignmentID"].Width = 60;
            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;

            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
        }

        private void CommonGridRequestsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.OrderStatusColumn);
            grid.Columns.Add(ProfileAssignmentsManager.ClientNameColumn);

            if (grid.Columns.Contains("SaveToStore"))
                grid.Columns["SaveToStore"].Visible = false;
            if (grid.Columns.Contains("WriteOffFromStore"))
                grid.Columns["WriteOffFromStore"].Visible = false;
            if (grid.Columns.Contains("SawName"))
                grid.Columns["SawName"].Visible = false;
            if (grid.Columns.Contains("FacingMachine"))
                grid.Columns["FacingMachine"].Visible = false;
            if (grid.Columns.Contains("ProfilOrderStatusID"))
                grid.Columns["ProfilOrderStatusID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            //if (grid.Columns.Contains("DecorAssignmentID"))
            //    grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("AddToPlanDateTime"))
                grid.Columns["AddToPlanDateTime"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("NextLinkAssignmentID"))
                grid.Columns["NextLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("FactCount"))
                grid.Columns["FactCount"].Visible = false;
            if (grid.Columns.Contains("DisprepancyCount"))
                grid.Columns["DisprepancyCount"].Visible = false;
            if (grid.Columns.Contains("DefectCount"))
                grid.Columns["DefectCount"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("Notes"))
                grid.Columns["Notes"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.MinimumWidth = 50;
            }

            grid.Columns["MegaOrderID"].HeaderText = "Заказ";
            grid.Columns["DecorAssignmentID"].HeaderText = "№";
            grid.Columns["MainOrderID"].HeaderText = "Подзаказ";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["AddToPlanDateTime"].HeaderText = "Включено\r\n  в план";
            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["Height2"].HeaderText = "Высота";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Length1"].HeaderText = "Длина";
            grid.Columns["Width1"].HeaderText = "Ширина";
            grid.Columns["Count1"].HeaderText = "Листы";
            grid.Columns["ComplexSawing"].HeaderText = "Сложный\r\n  распил";
            grid.Columns["Thickness2"].HeaderText = "Толщина";
            grid.Columns["Diameter2"].HeaderText = "Диаметр";

            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;

            grid.Columns["DecorAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DecorAssignmentID"].Width = 60;
            grid.Columns["ClientNameColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ClientNameColumn"].MinimumWidth = 80;
            grid.Columns["OrderStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["OrderStatusColumn"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
        }

        private void dgvFacingMaterialAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.FacingRollersColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("SawName"))
                grid.Columns["SawName"].Visible = false;
            if (grid.Columns.Contains("SaveToStore"))
                grid.Columns["SaveToStore"].Visible = false;
            if (grid.Columns.Contains("WriteOffFromStore"))
                grid.Columns["WriteOffFromStore"].Visible = false;
            if (grid.Columns.Contains("MegaOrderID"))
                grid.Columns["MegaOrderID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            //if (grid.Columns.Contains("DecorAssignmentID"))
            //    grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("MegaOrderID"))
                grid.Columns["MegaOrderID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("Length2"))
                grid.Columns["Length2"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("NextLinkAssignmentID"))
                grid.Columns["NextLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["AddToPlanDateTime"].HeaderText = "Включено\r\n  в план";
            grid.Columns["Thickness2"].HeaderText = "Толщина";
            grid.Columns["Diameter2"].HeaderText = "Диаметр";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["FacingMachine"].HeaderText = "Станок";

            grid.Columns["Thickness2"].ReadOnly = false;
            grid.Columns["Diameter2"].ReadOnly = false;
            grid.Columns["Width2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["FacingRollersColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["FacingRollersColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["FacingMachine"].DisplayIndex = DisplayIndex++;

            grid.Columns["DecorAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DecorAssignmentID"].Width = 50;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["FacingRollersColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["FacingRollersColumn"].MinimumWidth = 150;

            grid.Columns["FacingMachine"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FacingMachine"].MinimumWidth = 80;
            grid.Columns["Thickness2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness2"].MinimumWidth = 80;
            grid.Columns["Diameter2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Diameter2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvFacingMaterialOnStorageSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "TF-1300",
                Name = "Column1",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            KryptonDataGridViewButtonColumn Column2 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Пила",
                Name = "Column2",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(Column2);
            grid.AutoGenerateColumns = false;

            //if (grid.Columns.Contains("StoreID"))
            //    grid.Columns["StoreID"].Visible = false;
            if (grid.Columns.Contains("StoreItemID"))
                grid.Columns["StoreItemID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Название";
            grid.Columns["Thickness"].HeaderText = "Толщина";
            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["CreateDateTime"].HeaderText = "Дата создания";

            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CreateDateTime"].Width = 120;
            grid.Columns["StoreID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StoreID"].Width = 50;
            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Thickness"].Width = 120;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Length"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;
            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
            grid.Columns["Column2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column2"].Width = 100;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreateDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column2"].DisplayIndex = DisplayIndex++;
        }

        private void dgvFacingRollersAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.FacingRollersColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("SaveToStore"))
                grid.Columns["SaveToStore"].Visible = false;
            if (grid.Columns.Contains("WriteOffFromStore"))
                grid.Columns["WriteOffFromStore"].Visible = false;
            if (grid.Columns.Contains("FacingMachine"))
                grid.Columns["FacingMachine"].Visible = false;
            if (grid.Columns.Contains("MegaOrderID"))
                grid.Columns["MegaOrderID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            //if (grid.Columns.Contains("DecorAssignmentID"))
            //    grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("Length2"))
                grid.Columns["Length2"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("SawName"))
                grid.Columns["SawName"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("NextLinkAssignmentID"))
                grid.Columns["NextLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["AddToPlanDateTime"].HeaderText = "Включено\r\n  в план";
            grid.Columns["Thickness2"].HeaderText = "Толщина";
            grid.Columns["Diameter2"].HeaderText = "Диаметр";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["Thickness2"].ReadOnly = false;
            grid.Columns["Diameter2"].ReadOnly = false;
            grid.Columns["Width2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["FacingRollersColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["FacingRollersColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["FacingRollersColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["FacingRollersColumn"].MinimumWidth = 150;

            grid.Columns["DecorAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DecorAssignmentID"].Width = 50;
            grid.Columns["Thickness2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness2"].MinimumWidth = 80;
            grid.Columns["Diameter2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Diameter2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvFacingRollersOnStorageSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "TF-1300",
                Name = "Column1",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("ManufactureStoreID"))
                grid.Columns["ManufactureStoreID"].Visible = false;
            if (grid.Columns.Contains("StoreItemID"))
                grid.Columns["StoreItemID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Название";
            grid.Columns["Thickness"].HeaderText = "Толщина";
            grid.Columns["Diameter"].HeaderText = "Диаметр";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["CreateDateTime"].HeaderText = "Дата создания";

            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CreateDateTime"].Width = 120;
            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Thickness"].Width = 120;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Diameter"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;
            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreateDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;
        }

        private void dgvBatchAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.BatchEnabledColumn);
            grid.Columns.Add(ProfileAssignmentsManager.CloseUserColumn);
            grid.Columns.Add(ProfileAssignmentsManager.ToolsConfirmUserColumn);
            grid.Columns.Add(ProfileAssignmentsManager.TechnologyConfirmUserColumn);
            grid.Columns.Add(ProfileAssignmentsManager.MaterialConfirmUserColumn);
            grid.Columns.Add(ProfileAssignmentsManager.TechnicalConfirmUserColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("BatchEnable"))
                grid.Columns["BatchEnable"].Visible = false;
            if (grid.Columns.Contains("ToolsConfirmUserID"))
                grid.Columns["ToolsConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("TechnologyConfirmUserID"))
                grid.Columns["TechnologyConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("MaterialConfirmUserID"))
                grid.Columns["MaterialConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("TechnicalConfirmUserID"))
                grid.Columns["TechnicalConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("DispatchDate"))
                grid.Columns["DispatchDate"].Visible = false;
            if (grid.Columns.Contains("CloseUserID"))
                grid.Columns["CloseUserID"].Visible = false;
            if (grid.Columns.Contains("ProductionUserID"))
                grid.Columns["ProductionUserID"].Visible = false;
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.Columns["CloseDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["Notes"].ReadOnly = false;

            grid.Columns["BatchAssignmentID"].HeaderText = "№ партии";
            grid.Columns["CreateDate"].HeaderText = "Дата создания";
            grid.Columns["CloseDate"].HeaderText = "Дата закрытия";
            grid.Columns["ProductionDate"].HeaderText = "Вход в пр-во";
            grid.Columns["ToolsConfirmDate"].HeaderText = "Инструмент:\r\nдата утверждения";
            grid.Columns["TechnologyConfirmDate"].HeaderText = "Технология:\r\nдата утверждения";
            grid.Columns["MaterialConfirmDate"].HeaderText = "Материал:\r\nдата утверждения";
            grid.Columns["TechnicalConfirmDate"].HeaderText = "Техническое состояние:\r\nдата утверждения";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["ReadyPerc"].HeaderText = "Готовность";

            grid.Columns["BatchEnabledColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["BatchEnabledColumn"].Width = 40;
            grid.Columns["BatchAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["BatchAssignmentID"].Width = 90;
            grid.Columns["CreateDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CreateDate"].MinimumWidth = 100;
            grid.Columns["CloseDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CloseDate"].MinimumWidth = 100;
            grid.Columns["ProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ProductionDate"].MinimumWidth = 100;
            grid.Columns["ToolsConfirmDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ToolsConfirmDate"].MinimumWidth = 140;
            grid.Columns["TechnologyConfirmDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnologyConfirmDate"].MinimumWidth = 140;
            grid.Columns["MaterialConfirmDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MaterialConfirmDate"].MinimumWidth = 140;
            grid.Columns["TechnicalConfirmDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnicalConfirmDate"].MinimumWidth = 140;
            grid.Columns["CloseUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CloseUserColumn"].MinimumWidth = 100;
            grid.Columns["ToolsConfirmUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ToolsConfirmUserColumn"].MinimumWidth = 100;
            grid.Columns["TechnologyConfirmUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnologyConfirmUserColumn"].MinimumWidth = 100;
            grid.Columns["MaterialConfirmUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MaterialConfirmUserColumn"].MinimumWidth = 70;
            grid.Columns["TechnicalConfirmUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnicalConfirmUserColumn"].MinimumWidth = 70;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["Notes"].MinimumWidth = 150;
            grid.Columns["ReadyPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ReadyPerc"].Width = 120;

            int DisplayIndex = 0;
            grid.Columns["BatchAssignmentID"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreateDate"].DisplayIndex = DisplayIndex++;
            grid.Columns["BatchEnabledColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["CloseUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["CloseDate"].DisplayIndex = DisplayIndex++;
            grid.Columns["ProductionDate"].DisplayIndex = DisplayIndex++;
            grid.Columns["ReadyPerc"].DisplayIndex = DisplayIndex++;
            grid.Columns["ToolsConfirmUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ToolsConfirmDate"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnologyConfirmUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnologyConfirmDate"].DisplayIndex = DisplayIndex++;
            grid.Columns["MaterialConfirmUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MaterialConfirmDate"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnicalConfirmUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnicalConfirmDate"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["ReadyPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.AddPercentageColumn("ReadyPerc");
        }

        private void dgvMdfPlateOnStorageSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("CoverID"))
                grid.Columns["CoverID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "МДФ";
            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Length"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
        }

        private void dgvMilledProfileAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.SawStripsColumn1);
            grid.Columns.Add(ProfileAssignmentsManager.MilledProfileColumn2);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            grid.Columns["Length1"].ReadOnly = false;
            grid.Columns["Width1"].ReadOnly = false;
            grid.Columns["Length2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["SawStripsColumn"].ReadOnly = false;
            grid.Columns["MilledProfileColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["SawStripsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length1"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width1"].DisplayIndex = DisplayIndex++;
            grid.Columns["MilledProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["SawStripsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["SawStripsColumn"].MinimumWidth = 150;
            grid.Columns["MilledProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["MilledProfileColumn"].MinimumWidth = 150;

            grid.Columns["Length1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length1"].MinimumWidth = 80;
            grid.Columns["Width1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width1"].MinimumWidth = 80;
            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvMilledProfilesOnStorageSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("CoverID"))
                grid.Columns["CoverID"].Visible = false;
            if (grid.Columns.Contains("Width"))
                grid.Columns["Width"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Профиль";
            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Length"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
        }

        private void dgvMilledProfileRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Frezer1Column = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Фрезер 1",
                Name = "Frezer1Column",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            KryptonDataGridViewButtonColumn Frezer2Column = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Фрезер 2",
                Name = "Frezer2Column",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            KryptonDataGridViewButtonColumn Frezer3Column = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Фрезер 3",
                Name = "Frezer3Column",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Frezer1Column);
            grid.Columns.Add(Frezer2Column);
            grid.Columns.Add(Frezer3Column);

            grid.Columns.Add(ProfileAssignmentsManager.MilledProfileColumn2);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            int DisplayIndex = 0;
            grid.Columns["ClientNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["OrderStatusColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MilledProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Frezer1Column"].DisplayIndex = DisplayIndex++;
            grid.Columns["Frezer2Column"].DisplayIndex = DisplayIndex++;
            grid.Columns["Frezer3Column"].DisplayIndex = DisplayIndex++;

            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;
            grid.Columns["Frezer1Column"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Frezer1Column"].Width = 100;
            grid.Columns["Frezer2Column"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Frezer2Column"].Width = 100;
            grid.Columns["Frezer3Column"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Frezer3Column"].Width = 100;

            grid.Columns["MilledProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["MilledProfileColumn"].MinimumWidth = 200;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
        }

        private void dgvPackingAssignmentSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.CoversColumn);
            grid.Columns.Add(ProfileAssignmentsManager.PackingProfileColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentID"))
                grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["AddToPlanDateTime"].HeaderText = "Включено\r\n  в план";
            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["Length1"].ReadOnly = false;
            grid.Columns["Width1"].ReadOnly = false;
            grid.Columns["Count1"].ReadOnly = false;
            grid.Columns["Length2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["CoversColumn"].ReadOnly = false;
            grid.Columns["PackingProfileColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["PackingProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["PackingProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["PackingProfileColumn"].MinimumWidth = 150;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["CoversColumn"].MinimumWidth = 150;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvPackingRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Упаковка",
                Name = "Column1",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(ProfileAssignmentsManager.CoversColumn);
            grid.Columns.Add(ProfileAssignmentsManager.PackingProfileColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            if (grid.Columns.Contains("Notes"))
                grid.Columns["Notes"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentID"))
                grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("AddToPlanDateTime"))
                grid.Columns["AddToPlanDateTime"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("FactCount"))
                grid.Columns["FactCount"].Visible = false;
            if (grid.Columns.Contains("DisprepancyCount"))
                grid.Columns["DisprepancyCount"].Visible = false;
            if (grid.Columns.Contains("DefectCount"))
                grid.Columns["DefectCount"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.MinimumWidth = 50;
            }

            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["PlanCount"].HeaderText = "План";

            int DisplayIndex = 0;
            grid.Columns["PackingProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;

            grid.Columns["PackingProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["PackingProfileColumn"].MinimumWidth = 150;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["CoversColumn"].MinimumWidth = 150;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;

            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
        }

        private void dgvKashirAssignmentSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.PVAColumn);
            grid.Columns.Add(ProfileAssignmentsManager.CoversColumn);
            grid.Columns.Add(ProfileAssignmentsManager.KashirColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            grid.Columns["PVAColumn"].ReadOnly = false;
            grid.Columns["Length1"].ReadOnly = false;
            grid.Columns["Width1"].ReadOnly = false;
            grid.Columns["Count1"].ReadOnly = false;
            grid.Columns["Length2"].ReadOnly = false;
            grid.Columns["Width2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["CoversColumn"].ReadOnly = false;
            grid.Columns["KashirColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["PVAColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["KashirColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["PVAColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["PVAColumn"].MinimumWidth = 150;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["KashirColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["KashirColumn"].MinimumWidth = 150;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["CoversColumn"].MinimumWidth = 150;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvKashirOnStorageSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("CoverID"))
                grid.Columns["CoverID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Вставка";
            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Length"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
        }

        private void dgvKashirRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Кашир",
                Name = "Column1",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(ProfileAssignmentsManager.CoversColumn);
            grid.Columns.Add(ProfileAssignmentsManager.KashirColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            int DisplayIndex = 0;
            grid.Columns["ClientNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["OrderStatusColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["KashirColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;

            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;
            grid.Columns["KashirColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["KashirColumn"].MinimumWidth = 150;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["CoversColumn"].MinimumWidth = 150;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;

            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
        }

        private void dgvSawnStripsAssignmentSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.MdfPlateColumn);
            grid.Columns.Add(ProfileAssignmentsManager.SawStripsColumn2);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("SawName"))
                grid.Columns["SawName"].Visible = true;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = true;

            grid.Columns["Length1"].ReadOnly = false;
            grid.Columns["Width1"].ReadOnly = false;
            grid.Columns["Count1"].ReadOnly = false;
            grid.Columns["Length2"].ReadOnly = false;
            grid.Columns["Width2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["MdfPlateColumn"].ReadOnly = false;
            grid.Columns["SawStripsColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["MdfPlateColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length1"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width1"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count1"].DisplayIndex = DisplayIndex++;
            grid.Columns["SawStripsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["ComplexSawing"].DisplayIndex = DisplayIndex++;
            grid.Columns["SawName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["MdfPlateColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["MdfPlateColumn"].MinimumWidth = 150;
            grid.Columns["SawStripsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["SawStripsColumn"].MinimumWidth = 150;

            grid.Columns["SawName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["SawName"].MinimumWidth = 80;
            grid.Columns["ComplexSawing"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ComplexSawing"].MinimumWidth = 80;
            grid.Columns["Length1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length1"].MinimumWidth = 80;
            grid.Columns["Width1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width1"].MinimumWidth = 80;
            grid.Columns["Count1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Count1"].MinimumWidth = 150;
            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvSawnStripsOnStorageSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("CoverID"))
                grid.Columns["CoverID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Полоса МДФ";
            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Length"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
        }

        private void dgvSawnStripsRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "3,2 мм",
                Name = "Column1",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            KryptonDataGridViewButtonColumn Column2 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "4,5 мм",
                Name = "Column2",
                Text = "В план",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(Column2);
            grid.Columns.Add(ProfileAssignmentsManager.SawStripsColumn2);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            int DisplayIndex = 0;
            grid.Columns["ClientNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["OrderStatusColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["SawStripsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column2"].DisplayIndex = DisplayIndex++;

            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;
            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
            grid.Columns["Column2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column2"].Width = 100;

            grid.Columns["SawStripsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["SawStripsColumn"].MinimumWidth = 200;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
        }

        private void dgvShroudedProfileAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.ShroudedProfileColumn2);
            grid.Columns.Add(ProfileAssignmentsManager.CoversColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            grid.Columns["Length2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["ShroudedProfileColumn"].ReadOnly = false;
            grid.Columns["CoversColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["ShroudedProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["ShroudedProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ShroudedProfileColumn"].MinimumWidth = 150;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["CoversColumn"].MinimumWidth = 150;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvShroudedProfilesOnStorageSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("CoverID"))
                grid.Columns["CoverID"].Visible = false;
            if (grid.Columns.Contains("Width"))
                grid.Columns["Width"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Профиль";
            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["CoverName"].HeaderText = "Цвет";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["CoverName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["CoverName"].MinimumWidth = 100;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Length"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoverName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
        }

        private void dgvShroudedProfieRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Barberan1Column = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Барберан 1",
                Text = "В план",
                Name = "Barberan1Column",
                UseColumnTextForButtonValue = true
            };
            KryptonDataGridViewButtonColumn Barberan2Column = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Барберан 2",
                Text = "В план",
                Name = "Barberan2Column",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Barberan1Column);
            grid.Columns.Add(Barberan2Column);

            grid.Columns.Add(ProfileAssignmentsManager.ShroudedProfileColumn2);
            grid.Columns.Add(ProfileAssignmentsManager.CoversColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            grid.Columns["Barberan1Column"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Barberan1Column"].Width = 100;
            grid.Columns["Barberan2Column"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Barberan2Column"].Width = 100;

            int DisplayIndex = 0;
            grid.Columns["ClientNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["OrderStatusColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ShroudedProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Barberan1Column"].DisplayIndex = DisplayIndex++;
            grid.Columns["Barberan2Column"].DisplayIndex = DisplayIndex++;

            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;
            grid.Columns["Barberan1Column"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Barberan1Column"].Width = 100;
            grid.Columns["Barberan2Column"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Barberan2Column"].Width = 100;

            grid.Columns["ShroudedProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ShroudedProfileColumn"].MinimumWidth = 200;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["CoversColumn"].MinimumWidth = 200;

            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
        }

        private void dgvAssembledProfileAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.ShroudedProfileColumn1);
            grid.Columns.Add(ProfileAssignmentsManager.AssembledProfileColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            grid.Columns["Length2"].ReadOnly = false;
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["FactCount"].ReadOnly = false;
            grid.Columns["DisprepancyCount"].ReadOnly = false;
            grid.Columns["DefectCount"].ReadOnly = false;
            grid.Columns["ShroudedProfileColumn"].ReadOnly = false;
            grid.Columns["AssembledProfileColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["AddToPlanDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["ShroudedProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length1"].DisplayIndex = DisplayIndex++;
            grid.Columns["AssembledProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FactCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DisprepancyCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["DefectCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 200;
            grid.Columns["AddToPlanDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AddToPlanDateTime"].MinimumWidth = 50;
            grid.Columns["ShroudedProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ShroudedProfileColumn"].MinimumWidth = 150;
            grid.Columns["AssembledProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["AssembledProfileColumn"].MinimumWidth = 150;

            grid.Columns["Length1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length1"].MinimumWidth = 80;
            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["FactCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FactCount"].MinimumWidth = 80;
            grid.Columns["DisprepancyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DisprepancyCount"].MinimumWidth = 80;
            grid.Columns["DefectCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DefectCount"].MinimumWidth = 80;
        }

        private void dgvAssembledProfilesOnStorageSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("Width"))
                grid.Columns["Width"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Профиль";
            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Length"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
        }

        private void dgvAssembledProfieRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Сборка",
                Text = "В план",
                Name = "Column1",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(ProfileAssignmentsManager.ShroudedProfileColumn1);
            grid.Columns.Add(ProfileAssignmentsManager.AssembledProfileColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Width2"))
                grid.Columns["Width2"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;

            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;

            int DisplayIndex = 0;
            grid.Columns["ClientNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["OrderStatusColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ShroudedProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length1"].DisplayIndex = DisplayIndex++;
            grid.Columns["AssembledProfileColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;

            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;
            grid.Columns["ShroudedProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ShroudedProfileColumn"].MinimumWidth = 200;
            grid.Columns["AssembledProfileColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["AssembledProfileColumn"].MinimumWidth = 200;

            grid.Columns["Length1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length1"].MinimumWidth = 80;
            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
        }

        private void dgvFacingRollersRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Готовность",
                Name = "Column1",
                Text = "Списать",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(ProfileAssignmentsManager.FacingRollersColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("SaveToStore"))
                grid.Columns["SaveToStore"].Visible = false;
            if (grid.Columns.Contains("WriteOffFromStore"))
                grid.Columns["WriteOffFromStore"].Visible = false;
            if (grid.Columns.Contains("FacingMachine"))
                grid.Columns["FacingMachine"].Visible = false;
            if (grid.Columns.Contains("MegaOrderID"))
                grid.Columns["MegaOrderID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            if (grid.Columns.Contains("Notes"))
                grid.Columns["Notes"].Visible = false;
            //if (grid.Columns.Contains("DecorAssignmentID"))
            //    grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("Length2"))
                grid.Columns["Length2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("AddToPlanDateTime"))
                grid.Columns["AddToPlanDateTime"].Visible = false;
            if (grid.Columns.Contains("FactCount"))
                grid.Columns["FactCount"].Visible = false;
            if (grid.Columns.Contains("DisprepancyCount"))
                grid.Columns["DisprepancyCount"].Visible = false;
            if (grid.Columns.Contains("DefectCount"))
                grid.Columns["DefectCount"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawName"))
                grid.Columns["SawName"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("NextLinkAssignmentID"))
                grid.Columns["NextLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.MinimumWidth = 50;
            }
            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["Thickness2"].HeaderText = "Толщина";
            grid.Columns["Diameter2"].HeaderText = "Диаметр";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";

            int DisplayIndex = 0;
            grid.Columns["FacingRollersColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;

            grid.Columns["FacingRollersColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["FacingRollersColumn"].MinimumWidth = 200;

            grid.Columns["DecorAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DecorAssignmentID"].Width = 50;
            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
            grid.Columns["Thickness2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness2"].MinimumWidth = 80;
            grid.Columns["Diameter2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Diameter2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
        }

        private void dgvFacingMaterialRequestsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn Column1 = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = "Готовность",
                Name = "Column1",
                Text = "Списать",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(ProfileAssignmentsManager.FacingMaterialColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("SawName"))
                grid.Columns["SawName"].Visible = false;
            if (grid.Columns.Contains("SaveToStore"))
                grid.Columns["SaveToStore"].Visible = false;
            if (grid.Columns.Contains("WriteOffFromStore"))
                grid.Columns["WriteOffFromStore"].Visible = false;
            if (grid.Columns.Contains("MegaOrderID"))
                grid.Columns["MegaOrderID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            //if (grid.Columns.Contains("DecorAssignmentID"))
            //    grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("AddToPlanDateTime"))
                grid.Columns["AddToPlanDateTime"].Visible = false;
            if (grid.Columns.Contains("FactCount"))
                grid.Columns["FactCount"].Visible = false;
            if (grid.Columns.Contains("DisprepancyCount"))
                grid.Columns["DisprepancyCount"].Visible = false;
            if (grid.Columns.Contains("DefectCount"))
                grid.Columns["DefectCount"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("PrevLinkAssignmentID"))
                grid.Columns["PrevLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("NextLinkAssignmentID"))
                grid.Columns["NextLinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.MinimumWidth = 50;
            }

            grid.Columns["PlanCount"].ReadOnly = false;
            grid.Columns["Thickness2"].HeaderText = "Толщина";
            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["PlanCount"].HeaderText = "План";
            grid.Columns["FactCount"].HeaderText = "Факт";
            grid.Columns["DisprepancyCount"].HeaderText = "Некондиция";
            grid.Columns["DefectCount"].HeaderText = "Брак";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["FacingMachine"].HeaderText = "Станок";

            int DisplayIndex = 0;
            grid.Columns["FacingMaterialColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["FacingMachine"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;

            grid.Columns["FacingMaterialColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["FacingMaterialColumn"].MinimumWidth = 200;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 80;

            grid.Columns["DecorAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DecorAssignmentID"].Width = 50;
            grid.Columns["FacingMachine"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FacingMachine"].MinimumWidth = 80;
            grid.Columns["Thickness2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Thickness2"].MinimumWidth = 80;
            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 80;
            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
        }

        #endregion

        private void dgvShroudedProfileAssignments_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["BarberanNumber"].Value = 1;
            e.Row.Cells["ProductType"].Value = 1;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void dgvMilledProfileAssignments_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["FrezerNumber"].Value = 1;
            e.Row.Cells["ProductType"].Value = 2;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void dgvSawnStripsAssigments_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["ProductType"].Value = 4;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;
            if (kryptonCheckSet1.CheckedButton == cbtnHolzma)
            {
                pnlCutting.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnBarberan)
            {
                pnlEnveloping.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnFrezer)
            {
                pnlMeeling.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnAssembly)
            {
                pnlAssembly.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnKashir)
            {
                pnlLaminating.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnPaperCutting)
            {
                pnlPaperCutting.BringToFront();
            }
            if (MenuButton.Checked)
                pnlFilterAssignments.BringToFront();
        }

        private void dgvMilledProfileRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            int ClientID = 0;
            int MegaOrderID = 0;
            int MainOrderID = 0;
            int DecorOrderID = 0;
            int TechStoreID1 = 0;
            int TechStoreID2 = 0;
            int Length2 = 0;
            decimal Width2 = 0;
            int PlanCount = 0;
            bool AlreadyAdded = false;

            if (dgvMilledProfileRequests.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                ClientID = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["ClientID"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["MegaOrderID"].Value != DBNull.Value)
                MegaOrderID = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["MegaOrderID"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["MainOrderID"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["DecorOrderID"].Value != DBNull.Value)
                DecorOrderID = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["DecorOrderID"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["TechStoreID2"].Value != DBNull.Value)
                TechStoreID2 = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["TechStoreID2"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                Length2 = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["Length2"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0)
                return;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Frezer1Column" && e.RowIndex >= 0)
            {
                TechStoreID1 = ProfileAssignmentsManager.FindSawStripID(TechStoreID2, 1, ref Width2);
                if (TechStoreID1 != 0)
                {
                    AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                    if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                }
                else
                {
                    TechStoreID1 = ProfileAssignmentsManager.FindShroudedAssembledProfileID(TechStoreID2, 1);
                    if (TechStoreID1 != 0)
                    {
                        AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                        if (!AlreadyAdded)
                            ProfileAssignmentsManager.AddAssembledProfileToRequestFromMil(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                    }
                }
                ProfileAssignmentsManager.AddMilledProfileToPlan(DecorAssignmentID, 1);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Frezer2Column" && e.RowIndex >= 0)
            {
                TechStoreID1 = ProfileAssignmentsManager.FindSawStripID(TechStoreID2, 2, ref Width2);
                if (TechStoreID1 != 0)
                {
                    AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                    if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                }
                else
                {
                    TechStoreID1 = ProfileAssignmentsManager.FindShroudedAssembledProfileID(TechStoreID2, 2);
                    if (TechStoreID1 != 0)
                    {
                        AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                        if (!AlreadyAdded)
                            ProfileAssignmentsManager.AddAssembledProfileToRequestFromMil(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                    }
                }
                ProfileAssignmentsManager.AddMilledProfileToPlan(DecorAssignmentID, 2);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Frezer3Column" && e.RowIndex >= 0)
            {
                TechStoreID1 = ProfileAssignmentsManager.FindSawStripID(TechStoreID2, 3, ref Width2);
                if (TechStoreID1 != 0)
                {
                    AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                    if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                }
                else
                {
                    TechStoreID1 = ProfileAssignmentsManager.FindShroudedAssembledProfileID(TechStoreID2, 3);
                    if (TechStoreID1 != 0)
                    {
                        AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                        if (!AlreadyAdded)
                            ProfileAssignmentsManager.AddAssembledProfileToRequestFromMil(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                    }
                }
                ProfileAssignmentsManager.AddMilledProfileToPlan(DecorAssignmentID, 3);
            }
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;
            if (kryptonCheckSet2.CheckedButton == cbtnBarberan1)
            {
                panel19.BringToFront();
                lbSumBarberan1.Visible = true;
                lbSumBarberan2.Visible = false;
            }
            if (kryptonCheckSet2.CheckedButton == cbtnBarberan2)
            {
                panel2.BringToFront();
                lbSumBarberan1.Visible = false;
                lbSumBarberan2.Visible = true;
            }
        }

        private void dgvShroudedProfileRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            int ClientID = 0;
            int MegaOrderID = 0;
            int MainOrderID = 0;
            int DecorOrderID = 0;
            int TechStoreID2 = 0;
            int TechStoreID1 = 0;
            int CoverID2 = 0;
            int Length2 = 0;
            int PlanCount = 0;
            decimal Width2 = 0;
            bool AlreadyAdded = false;

            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                ClientID = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["ClientID"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["MegaOrderID"].Value != DBNull.Value)
                MegaOrderID = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["MegaOrderID"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["MainOrderID"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorOrderID"].Value != DBNull.Value)
                DecorOrderID = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorOrderID"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["TechStoreID2"].Value != DBNull.Value)
                TechStoreID2 = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["TechStoreID2"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                Length2 = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["Length2"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["CoverID2"].Value != DBNull.Value)
                CoverID2 = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["CoverID2"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "Barberan1Column" && e.RowIndex >= 0)
            {
                ProfileAssignmentsManager.AddShroudedProfileToPlan(DecorAssignmentID, 1);
                TechStoreID1 = ProfileAssignmentsManager.FindMilledProfileID(TechStoreID2, 1);
                if (TechStoreID1 != 0)
                {
                    AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                    if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddMilledProfileToRequestFromEnv(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                }
                else
                {
                    TechStoreID1 = ProfileAssignmentsManager.FindShroudedAssembledProfileID(TechStoreID2, 1);
                    if (TechStoreID1 != 0)
                    {
                        AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                        if (!AlreadyAdded)
                            ProfileAssignmentsManager.AddAssembledProfileToRequestFromEnv(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                    }
                    else
                    {
                        TechStoreID1 = ProfileAssignmentsManager.FindMilledAssembledProfileID(TechStoreID2, 1);
                        if (TechStoreID1 != 0)
                        {
                            AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                            if (!AlreadyAdded)
                                ProfileAssignmentsManager.AddAssembledProfileToRequestFromMil(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                        }
                        else
                        {
                            TechStoreID1 = ProfileAssignmentsManager.FindSawStripID(TechStoreID2, 1, ref Width2);
                            if (TechStoreID1 != 0)
                            {
                                AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                                if (!AlreadyAdded)
                                    ProfileAssignmentsManager.AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                            }
                        }
                    }
                }
            }

            if (senderGrid.Columns[e.ColumnIndex].Name == "Barberan2Column" && e.RowIndex >= 0)
            {
                ProfileAssignmentsManager.AddShroudedProfileToPlan(DecorAssignmentID, 2);
                TechStoreID1 = ProfileAssignmentsManager.FindMilledProfileID(TechStoreID2, 2);
                if (TechStoreID1 != 0)
                {
                    AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                    if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddMilledProfileToRequestFromEnv(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                }
                else
                {
                    TechStoreID1 = ProfileAssignmentsManager.FindShroudedAssembledProfileID(TechStoreID2, 2);
                    if (TechStoreID1 != 0)
                    {
                        AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                        if (!AlreadyAdded)
                            ProfileAssignmentsManager.AddAssembledProfileToRequestFromEnv(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                    }
                    else
                    {
                        TechStoreID1 = ProfileAssignmentsManager.FindMilledAssembledProfileID(TechStoreID2, 2);
                        if (TechStoreID1 != 0)
                        {
                            AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                            if (!AlreadyAdded)
                                ProfileAssignmentsManager.AddAssembledProfileToRequestFromMil(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                        }
                        else
                        {
                            TechStoreID1 = ProfileAssignmentsManager.FindSawStripID(TechStoreID2, 2, ref Width2);
                            if (TechStoreID1 != 0)
                            {
                                AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                                if (!AlreadyAdded)
                                    ProfileAssignmentsManager.AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                            }
                        }
                    }
                }
            }
        }

        private void dgvShroudedProfileAssignments2_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["BarberanNumber"].Value = 2;
            e.Row.Cells["ProductType"].Value = 1;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void dgvMilledProfileAssignments2_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["FrezerNumber"].Value = 2;
            e.Row.Cells["ProductType"].Value = 2;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void dgvMilledProfileAssignments3_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["FrezerNumber"].Value = 3;
            e.Row.Cells["ProductType"].Value = 2;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void kryptonCheckSet3_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;
            if (kryptonCheckSet3.CheckedButton == cbtnFrezer1)
            {
                lbSumFrezer1.Visible = true;
                lbSumFrezer2.Visible = false;
                lbSumFrezer3.Visible = false;
                panel27.BringToFront();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnFrezer2)
            {
                lbSumFrezer1.Visible = false;
                lbSumFrezer2.Visible = true;
                lbSumFrezer3.Visible = false;
                panel28.BringToFront();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnFrezer3)
            {
                lbSumFrezer1.Visible = false;
                lbSumFrezer2.Visible = false;
                lbSumFrezer3.Visible = true;
                panel29.BringToFront();
            }
        }

        private void dgvSawnStripsRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            int PrevLinkAssignmentID = 0;
            int TechStoreID1 = 0;
            int TechStoreID2 = 0;
            decimal Width2 = 0;
            decimal Width1 = 0;

            if (dgvSawnStripsRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvSawnStripsRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvSawnStripsRequests.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                PrevLinkAssignmentID = Convert.ToInt32(dgvSawnStripsRequests.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            if (dgvSawnStripsRequests.SelectedRows[0].Cells["TechStoreID2"].Value != DBNull.Value)
                TechStoreID2 = Convert.ToInt32(dgvSawnStripsRequests.SelectedRows[0].Cells["TechStoreID2"].Value);
            if (DecorAssignmentID == 0)
                return;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                TechStoreID1 = ProfileAssignmentsManager.FindMdfID(TechStoreID2, ref Width1);
                if (PrevLinkAssignmentID != 0)
                    Width2 = ProfileAssignmentsManager.EditFrezerWidth(PrevLinkAssignmentID, 1);
                ProfileAssignmentsManager.AddSawnStripsToPlan(DecorAssignmentID, 1, "3,2 мм", Width2);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Column2" && e.RowIndex >= 0)
            {
                TechStoreID1 = ProfileAssignmentsManager.FindMdfID(TechStoreID2, ref Width1);
                if (PrevLinkAssignmentID != 0)
                    Width2 = ProfileAssignmentsManager.EditFrezerWidth(PrevLinkAssignmentID, 2);
                ProfileAssignmentsManager.AddSawnStripsToPlan(DecorAssignmentID, 2, "4,5 мм", Width2);
            }
        }

        private void dgvShroudedProfileAssignments1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            string Parameter = string.Empty;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Length2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Width2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        private void dgvShroudedProfileAssignments2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            string Parameter = string.Empty;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Length2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Width2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        public void FilterSawStripsAssignments()
        {
            bool bOnProd = cbFilterOnProd.Checked;
            bool bInProd = cbFilterInProd.Checked;
            bool bOnStorage = cbFilterOnStorage.Checked;

            //ProfileAssignmentsManager.FilterSawStripsAssignments(bOnProd, bInProd, bOnStorage);
        }

        public void FilterMilledProfileAssignments()
        {
            bool bOnProd = cbFilterOnProd.Checked;
            bool bInProd = cbFilterInProd.Checked;
            bool bOnStorage = cbFilterOnStorage.Checked;

            //ProfileAssignmentsManager.FilterMilledProfileAssignments(bOnProd, bInProd, bOnStorage);

            decimal SumCount = 0;
            for (int i = 0; i < dgvMilledProfileAssignments1.Rows.Count; i++)
            {
                if (dgvMilledProfileAssignments1.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvMilledProfileAssignments1.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvMilledProfileAssignments1.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvMilledProfileAssignments1.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumFrezer1.Text = SumCount.ToString() + " м.п.";

            SumCount = 0;
            for (int i = 0; i < dgvMilledProfileAssignments2.Rows.Count; i++)
            {
                if (dgvMilledProfileAssignments2.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvMilledProfileAssignments2.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvMilledProfileAssignments2.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvMilledProfileAssignments2.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumFrezer2.Text = SumCount.ToString() + " м.п.";

            SumCount = 0;
            for (int i = 0; i < dgvMilledProfileAssignments3.Rows.Count; i++)
            {
                if (dgvMilledProfileAssignments3.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvMilledProfileAssignments3.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvMilledProfileAssignments3.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvMilledProfileAssignments3.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumFrezer3.Text = SumCount.ToString() + " м.п.";

            SumCount = 0;
            for (int i = 0; i < dgvMilledProfileRequests.Rows.Count; i++)
            {
                if (dgvMilledProfileRequests.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvMilledProfileRequests.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvMilledProfileRequests.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvMilledProfileRequests.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumFrezerRequests.Text = SumCount.ToString() + " м.п.";
        }

        private void FilterByProductionStatus()
        {
            bool bsNotInProduction = cbFilterNotInProd.Checked;
            bool bsOnProduction = cbFilterOnProd.Checked;
            bool bsInProduction = cbFilterInProd.Checked;
            bool bsInStorage = cbFilterOnStorage.Checked;
            bool bsOnExpedition = cbFilterOnExp.Checked;
            bool bsIsDispatched = cbFilterDisp.Checked;
            ProfileAssignmentsManager.FilterByProductionStatus(bsNotInProduction, bsOnProduction, bsInProduction,
                bsInStorage, bsOnExpedition, bsIsDispatched);
        }

        private void cbFilterOnProd_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbFilterInProd_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbFilterOnStorage_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        public void FilterShroudedProfileAssignments()
        {
            bool bOnProd = cbFilterOnProd.Checked;
            bool bInProd = cbFilterInProd.Checked;
            bool bOnStorage = cbFilterOnStorage.Checked;

            //ProfileAssignmentsManager.FilterShroudedProfileAssignments(bOnProd, bInProd, bOnStorage);

            decimal SumCount = 0;
            for (int i = 0; i < dgvShroudedProfileAssignments1.Rows.Count; i++)
            {
                if (dgvShroudedProfileAssignments1.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvShroudedProfileAssignments1.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvShroudedProfileAssignments1.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvShroudedProfileAssignments1.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumBarberan1.Text = SumCount.ToString() + " м.п.";

            SumCount = 0;
            for (int i = 0; i < dgvShroudedProfileAssignments2.Rows.Count; i++)
            {
                if (dgvShroudedProfileAssignments2.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvShroudedProfileAssignments2.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvShroudedProfileAssignments2.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvShroudedProfileAssignments2.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumBarberan2.Text = SumCount.ToString() + " м.п.";

            SumCount = 0;
            for (int i = 0; i < dgvShroudedProfileRequests.Rows.Count; i++)
            {
                if (dgvShroudedProfileRequests.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvShroudedProfileRequests.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvShroudedProfileRequests.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvShroudedProfileRequests.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumBarberanRequests.Text = SumCount.ToString() + " м.п.";
        }

        private void dgvShroudedProfileAssignments1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvShroudedProfileAssignments2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private bool CheckShroudedProfiles()
        {
            for (int i = 0; i < dgvShroudedProfileAssignments1.Rows.Count; i++)
            {
                if (dgvShroudedProfileAssignments1.Rows[i].IsNewRow)
                    continue;
                if (dgvShroudedProfileAssignments1.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan RP-30. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvShroudedProfileAssignments1.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan RP-30. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvShroudedProfileAssignments1.Rows[i].Cells["CoverID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan RP-30. Не введен параметр: Цвет", "Ошибка сохранения");
                    return false;
                }
                if (dgvShroudedProfileAssignments1.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan RP-30. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }
            for (int i = 0; i < dgvShroudedProfileAssignments2.Rows.Count; i++)
            {
                if (dgvShroudedProfileAssignments2.Rows[i].IsNewRow)
                    continue;
                if (dgvShroudedProfileAssignments2.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan PUR-33-L. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvShroudedProfileAssignments2.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan PUR-33-L. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvShroudedProfileAssignments2.Rows[i].Cells["CoverID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan PUR-33-L. Не введен параметр: Цвет", "Ошибка сохранения");
                    return false;
                }
                if (dgvShroudedProfileAssignments2.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Barberan PUR-33-L. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }

            return true;
        }

        private bool CheckMilledProfiles()
        {
            for (int i = 0; i < dgvMilledProfileAssignments1.Rows.Count; i++)
            {
                if (dgvMilledProfileAssignments1.Rows[i].IsNewRow)
                    continue;
                if (dgvMilledProfileAssignments1.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "SCM Superset XL. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvMilledProfileAssignments1.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "SCM Superset XL. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvMilledProfileAssignments1.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "SCM Superset XL. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }
            for (int i = 0; i < dgvMilledProfileAssignments2.Rows.Count; i++)
            {
                if (dgvMilledProfileAssignments2.Rows[i].IsNewRow)
                    continue;
                if (dgvMilledProfileAssignments2.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Weinig Powermat 1200. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvMilledProfileAssignments2.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Weinig Powermat 1200. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvMilledProfileAssignments2.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Weinig Powermat 1200. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }
            for (int i = 0; i < dgvMilledProfileAssignments3.Rows.Count; i++)
            {
                if (dgvMilledProfileAssignments3.Rows[i].IsNewRow)
                    continue;
                if (dgvMilledProfileAssignments3.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Casolin F45. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvMilledProfileAssignments3.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Casolin F45. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvMilledProfileAssignments3.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Casolin F45. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }

            return true;
        }

        private bool CheckAssembledProfiles()
        {
            for (int i = 0; i < dgvAssembledProfileAssignments.Rows.Count; i++)
            {
                if (dgvAssembledProfileAssignments.Rows[i].IsNewRow)
                    continue;
                if (dgvAssembledProfileAssignments.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Сборка. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvAssembledProfileAssignments.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Сборка. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvAssembledProfileAssignments.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Сборка. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }

            return true;
        }

        private bool CheckSawnStrips()
        {
            for (int i = 0; i < dgvSawnStripsAssignments.Rows.Count; i++)
            {
                if (dgvSawnStripsAssignments.Rows[i].IsNewRow)
                    continue;
                if (dgvSawnStripsAssignments.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "HOLZMA HPP 350. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvSawnStripsAssignments.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "HOLZMA HPP 350. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvSawnStripsAssignments.Rows[i].Cells["Width2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "HOLZMA HPP 350. Не введен параметр: Ширина", "Ошибка сохранения");
                    return false;
                }
                if (dgvSawnStripsAssignments.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "HOLZMA HPP 350. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }

            return true;
        }

        private bool CheckKashir()
        {
            for (int i = 0; i < dgvKashirAssignments.Rows.Count; i++)
            {
                if (dgvKashirAssignments.Rows[i].IsNewRow)
                    continue;
                if (dgvKashirAssignments.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Кашир. Не введен параметр: Наименование", "Ошибка сохранения");
                    return false;
                }
                if (dgvKashirAssignments.Rows[i].Cells["CoverID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Кашир. Не введен параметр: Цвет", "Ошибка сохранения");
                    return false;
                }
                if (dgvKashirAssignments.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Кашир. Не введен параметр: Длина", "Ошибка сохранения");
                    return false;
                }
                if (dgvKashirAssignments.Rows[i].Cells["Width2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Кашир. Не введен параметр: Ширина", "Ошибка сохранения");
                    return false;
                }
                if (dgvKashirAssignments.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Кашир. Не введен параметр: Количество", "Ошибка сохранения");
                    return false;
                }
            }

            return true;
        }

        private void UpdateFacingMaterial()
        {
            for (int i = 0; i < dgvFacingMaterialRequests.Rows.Count; i++)
            {
                if (dgvFacingMaterialRequests.Rows[i].IsNewRow)
                    continue;
                if (dgvFacingMaterialRequests.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: Наименование", "Ошибка сохранения");
                    return;
                }
                //if (dgvFacingMaterialRequests.Rows[i].Cells["Thickness2"].Value == DBNull.Value)
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: Толщина", "Ошибка сохранения");
                //    return;
                //}
                if (dgvFacingMaterialRequests.Rows[i].Cells["Length2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: Длина", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingMaterialRequests.Rows[i].Cells["Width2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: Ширина", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingMaterialRequests.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: Количество", "Ошибка сохранения");
                    return;
                }
            }
            for (int i = 0; i < dgvFacingMaterialAssignments.Rows.Count; i++)
            {
                if (dgvFacingMaterialAssignments.Rows[i].IsNewRow)
                    continue;
                if (dgvFacingMaterialAssignments.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Наименование", "Ошибка сохранения");
                    return;
                }
                //if (dgvFacingMaterialAssignments.Rows[i].Cells["Thickness2"].Value == DBNull.Value)
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Толщина", "Ошибка сохранения");
                //    return;
                //}
                if (dgvFacingMaterialAssignments.Rows[i].Cells["Diameter2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Диаметр", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingMaterialAssignments.Rows[i].Cells["Width2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Ширина", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingMaterialAssignments.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Количество", "Ошибка сохранения");
                    return;
                }
            }
            int DecorAssignmentID = 0;

            if (dgvFacingMaterialRequests.SelectedRows.Count > 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            string Name = string.Empty;

            if (cbFilterFacingMaterial.SelectedItem != null && ((DataRowView)cbFilterFacingMaterial.SelectedItem).Row["TechStoreName"] != DBNull.Value)
                Name = ((DataRowView)cbFilterFacingMaterial.SelectedItem).Row["TechStoreName"].ToString();

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                ProfileAssignmentsManager.SaveFacingMaterialAssignments();
                ProfileAssignmentsManager.UpdateFacingMaterialAssignments();

                ProfileAssignmentsManager.SaveFacingRollersAssignments();
                ProfileAssignmentsManager.UpdateFacingRollersAssignments();

                if (DecorAssignmentID == 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                FilterFacingMaterialAssignments(DecorAssignmentID);

                ProfileAssignmentsManager.MoveToFacingMaterialRequest(DecorAssignmentID);
                FilterFacingMaterialAssignments(DecorAssignmentID);

                FilterFacingMaterialOnStorage();
                ProfileAssignmentsManager.MoveToFacingMaterialOnStorage(Name);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
                NeedSplash = true;
            }
            else
            {
                ProfileAssignmentsManager.SaveFacingMaterialAssignments();
                ProfileAssignmentsManager.UpdateFacingMaterialAssignments();

                ProfileAssignmentsManager.SaveFacingRollersAssignments();
                ProfileAssignmentsManager.UpdateFacingRollersAssignments();

                if (DecorAssignmentID == 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                FilterFacingMaterialAssignments(DecorAssignmentID);

                ProfileAssignmentsManager.MoveToFacingMaterialRequest(DecorAssignmentID);
                FilterFacingMaterialAssignments(DecorAssignmentID);

                FilterFacingMaterialOnStorage();
                ProfileAssignmentsManager.MoveToFacingMaterialOnStorage(Name);
            }
        }

        private void UpdateFacingRollers()
        {
            for (int i = 0; i < dgvFacingRollersRequests.Rows.Count; i++)
            {
                if (dgvFacingRollersRequests.Rows[i].IsNewRow)
                    continue;
                if (dgvFacingRollersRequests.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Наименование", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingRollersRequests.Rows[i].Cells["Diameter2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Диаметр", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingRollersRequests.Rows[i].Cells["Width2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Ширина", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingRollersRequests.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Количество", "Ошибка сохранения");
                    return;
                }
            }
            for (int i = 0; i < dgvFacingRollersAssignments.Rows.Count; i++)
            {
                if (dgvFacingRollersAssignments.Rows[i].IsNewRow)
                    continue;
                if (dgvFacingRollersAssignments.Rows[i].Cells["TechStoreID2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Наименование", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingRollersAssignments.Rows[i].Cells["Diameter2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Диаметр", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingRollersAssignments.Rows[i].Cells["Width2"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Ширина", "Ошибка сохранения");
                    return;
                }
                if (dgvFacingRollersAssignments.Rows[i].Cells["PlanCount"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовка ролики. Не введен параметр: Количество", "Ошибка сохранения");
                    return;
                }
            }

            int DecorAssignmentID = 0;
            if (dgvFacingRollersRequests.SelectedRows.Count > 0 && dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            string Name = string.Empty;

            if (cbFilterFacingRollers.SelectedItem != null && ((DataRowView)cbFilterFacingRollers.SelectedItem).Row["TechStoreName"] != DBNull.Value)
                Name = ((DataRowView)cbFilterFacingRollers.SelectedItem).Row["TechStoreName"].ToString();

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                ProfileAssignmentsManager.SaveFacingRollersAssignments();
                ProfileAssignmentsManager.UpdateFacingRollersAssignments();
                ProfileAssignmentsManager.MoveToFacingRollersRequest(DecorAssignmentID);
                ProfileAssignmentsManager.GetFacingRollersOnStorage();
                ProfileAssignmentsManager.GetFilterFacingRollers();

                if (DecorAssignmentID == 0 && dgvFacingRollersRequests.SelectedRows.Count > 0 && dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                    return;
                FilterFacingRollersAssignments(DecorAssignmentID);

                FilterFacingRollersOnStorage();
                ProfileAssignmentsManager.MoveToFacingRollersOnStorage(Name);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
                NeedSplash = true;
            }
            else
            {
                ProfileAssignmentsManager.SaveFacingRollersAssignments();
                ProfileAssignmentsManager.UpdateFacingRollersAssignments();
                ProfileAssignmentsManager.MoveToFacingRollersRequest(DecorAssignmentID);
                ProfileAssignmentsManager.GetFacingRollersOnStorage();
                ProfileAssignmentsManager.GetFilterFacingRollers();

                if (DecorAssignmentID == 0 && dgvFacingRollersRequests.SelectedRows.Count > 0 && dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                    return;
                FilterFacingRollersAssignments(DecorAssignmentID);

                FilterFacingRollersOnStorage();
                ProfileAssignmentsManager.MoveToFacingRollersOnStorage(Name);
            }
        }

        private void Save(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton != cbtnPaperCutting)
            {

                if (!CheckShroudedProfiles())
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Не сохранено", 1700);
                    return;
                }
                if (!CheckMilledProfiles())
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Не сохранено", 1700);
                    return;
                }
                if (!CheckAssembledProfiles())
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Не сохранено", 1700);
                    return;
                }
                if (!CheckSawnStrips())
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Не сохранено", 1700);
                    return;
                }
                if (!CheckKashir())
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Не сохранено", 1700);
                    return;
                }

                if (NeedSplash)
                {
                    NeedSplash = false;
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                    T.Start();
                    while (!SplashWindow.bSmallCreated) ;

                    ProfileAssignmentsManager.PreSaveAssigments();
                    ProfileAssignmentsManager.SaveAssignments();
                    ProfileAssignmentsManager.UpdateAssignments();
                    ProfileAssignmentsManager.CompareAssignments();
                    ProfileAssignmentsManager.SaveAssignments();
                    ProfileAssignmentsManager.UpdateAssignments();
                    ProfileAssignmentsManager.SetPrevLinks();
                    ProfileAssignmentsManager.SaveAssignments();
                    ProfileAssignmentsManager.UpdateAssignments();
                    ProfileAssignmentsManager.SetOrderStatus();
                    FilterShroudedProfileAssignments();
                    FilterMilledProfileAssignments();
                    FilterAssembledProfileAssignments();
                    FilterSawStripsAssignments();
                    FilterKashirAssignments();
                    ProfileAssignmentsManager.GetKashirOnStorage();
                    ProfileAssignmentsManager.GetMdfPlateOnStorage();
                    ProfileAssignmentsManager.GetMilledProfilesOnStorage();
                    ProfileAssignmentsManager.GetSawnStripsOnStorage();
                    ProfileAssignmentsManager.GetShroudedProfilesOnStorage();
                    ProfileAssignmentsManager.GetAssembledProfilesOnStorage();
                    ProfileAssignmentsManager.GroupKashir();
                    ProfileAssignmentsManager.GroupMdfPlate();
                    ProfileAssignmentsManager.GroupMilledProfiles();
                    ProfileAssignmentsManager.GroupSawnStrips();
                    ProfileAssignmentsManager.GroupShroudedProfiles();
                    ProfileAssignmentsManager.GroupAssembledProfiles();

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                    NeedSplash = true;
                }
                else
                {
                    ProfileAssignmentsManager.PreSaveAssigments();
                    ProfileAssignmentsManager.SaveAssignments();
                    ProfileAssignmentsManager.UpdateAssignments();
                    ProfileAssignmentsManager.CompareAssignments();
                    ProfileAssignmentsManager.SaveAssignments();
                    ProfileAssignmentsManager.UpdateAssignments();
                    ProfileAssignmentsManager.SetPrevLinks();
                    ProfileAssignmentsManager.SaveAssignments();
                    ProfileAssignmentsManager.UpdateAssignments();
                    ProfileAssignmentsManager.SetOrderStatus();
                    FilterShroudedProfileAssignments();
                    FilterMilledProfileAssignments();
                    FilterAssembledProfileAssignments();
                    FilterSawStripsAssignments();
                    FilterKashirAssignments();
                    ProfileAssignmentsManager.GetKashirOnStorage();
                    ProfileAssignmentsManager.GetMdfPlateOnStorage();
                    ProfileAssignmentsManager.GetMilledProfilesOnStorage();
                    ProfileAssignmentsManager.GetSawnStripsOnStorage();
                    ProfileAssignmentsManager.GetShroudedProfilesOnStorage();
                    ProfileAssignmentsManager.GetAssembledProfilesOnStorage();
                    ProfileAssignmentsManager.GroupKashir();
                    ProfileAssignmentsManager.GroupMdfPlate();
                    ProfileAssignmentsManager.GroupMilledProfiles();
                    ProfileAssignmentsManager.GroupSawnStrips();
                    ProfileAssignmentsManager.GroupShroudedProfiles();
                    ProfileAssignmentsManager.GroupAssembledProfiles();
                }
            }
            if (kryptonCheckSet1.CheckedButton == cbtnPaperCutting)
            {
                if (kryptonCheckSet4.CheckedButton == cbtnFacingMaterial)
                {
                    UpdateFacingMaterial();
                }
                if (kryptonCheckSet4.CheckedButton == cbtnFacingRollers)
                {
                    UpdateFacingRollers();
                }
            }

            FilterByProductionStatus();
            ProfileAssignmentsManager.GetAssignmentReadyTable();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
            NeedSplash = true;
        }

        public void SaveFacingRollersToStorage1()
        {
            for (int i = 0; i < dgvFacingMaterialAssignments.Rows.Count; i++)
            {
                if (dgvFacingMaterialAssignments.Rows[i].IsNewRow)
                    continue;

                if (dgvFacingMaterialAssignments.Rows[i].Cells["FactCount"].Value == DBNull.Value)
                    continue;

                int DecorAssignmentID = 0;
                int StoreItemID = -1;
                decimal Length = -1;
                decimal Width = -1;
                decimal Height = -1;
                decimal Thickness = -1;
                decimal Diameter = -1;
                decimal Admission = -1;
                decimal Capacity = -1;
                decimal Weight = -1;
                int ColorID = -1;
                int PatinaID = -1;
                int CoverID = -1;
                int Count = 0;
                int FactoryID = 1;
                string Notes = string.Empty;

                if (dgvFacingMaterialAssignments.Rows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialAssignments.Rows[i].Cells["DecorAssignmentID"].Value);
                if (dgvFacingMaterialAssignments.Rows[i].Cells["TechStoreID2"].Value != DBNull.Value)
                    StoreItemID = Convert.ToInt32(dgvFacingMaterialAssignments.Rows[i].Cells["TechStoreID2"].Value);
                if (dgvFacingMaterialAssignments.Rows[i].Cells["Thickness2"].Value != DBNull.Value)
                    Thickness = Convert.ToDecimal(dgvFacingMaterialAssignments.Rows[i].Cells["Thickness2"].Value);
                if (dgvFacingMaterialAssignments.Rows[i].Cells["Diameter2"].Value != DBNull.Value)
                    Diameter = Convert.ToDecimal(dgvFacingMaterialAssignments.Rows[i].Cells["Diameter2"].Value);
                if (dgvFacingMaterialAssignments.Rows[i].Cells["Width2"].Value != DBNull.Value)
                    Width = Convert.ToDecimal(dgvFacingMaterialAssignments.Rows[i].Cells["Width2"].Value);
                if (dgvFacingMaterialAssignments.Rows[i].Cells["FactCount"].Value != DBNull.Value)
                    Count = Convert.ToInt32(dgvFacingMaterialAssignments.Rows[i].Cells["FactCount"].Value);
                if (dgvFacingMaterialAssignments.Rows[i].Cells["Notes"].Value != DBNull.Value)
                    Notes = dgvFacingMaterialAssignments.Rows[i].Cells["Notes"].Value.ToString();

                ProfileAssignmentsManager.AssignmentsStoreManager.AddToManufactureStore(DecorAssignmentID, StoreItemID, Length, Width, Height, Thickness,
                    Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes);
            }
        }

        private void dgvSawnStripsAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMilledProfileAssignments1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMilledProfileAssignments2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMilledProfileAssignments3_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMilledProfileAssignments1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            string Parameter = string.Empty;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Length2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Width2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        private void dgvMilledProfileAssignments2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            string Parameter = string.Empty;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Length2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Width2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        private void dgvMilledProfileAssignments3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            string Parameter = string.Empty;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Length2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Width2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        private void dgvMilledProfileAssignments1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
                //senderGrid.Rows[e.RowIndex].ReadOnly = true;
                if (ProfileAssignmentsManager.BatchEnable)
                    senderGrid.Rows[e.RowIndex].ReadOnly = false;
                else
                    senderGrid.Rows[e.RowIndex].ReadOnly = true;
                senderGrid.Rows[e.RowIndex].Cells["FactCount"].ReadOnly = false;
                senderGrid.Rows[e.RowIndex].Cells["DisprepancyCount"].ReadOnly = false;
                senderGrid.Rows[e.RowIndex].Cells["DefectCount"].ReadOnly = false;
                senderGrid.Rows[e.RowIndex].Cells["Notes"].ReadOnly = false;
            }
        }

        public void FilterKashirAssignments()
        {
            bool bOnProd = cbFilterOnProd.Checked;
            bool bInProd = cbFilterInProd.Checked;
            bool bOnStorage = cbFilterOnStorage.Checked;

            //ProfileAssignmentsManager.FilterKashirAssignments(bOnProd, bInProd, bOnStorage);

            decimal SumCount = 0;
            for (int i = 0; i < dgvKashirAssignments.Rows.Count; i++)
            {
                if (dgvKashirAssignments.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvKashirAssignments.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvKashirAssignments.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvKashirAssignments.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumKashir1.Text = SumCount.ToString() + " м.п.";

            SumCount = 0;
            for (int i = 0; i < dgvKashirRequests.Rows.Count; i++)
            {
                if (dgvKashirRequests.Rows[i].Cells["Length2"].Value != DBNull.Value
                    && dgvKashirRequests.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToDecimal(dgvKashirRequests.Rows[i].Cells["Length2"].Value) *
                        Convert.ToDecimal(dgvKashirRequests.Rows[i].Cells["PlanCount"].Value);
                }
            }
            SumCount = Decimal.Round(SumCount / 1000, 1, MidpointRounding.AwayFromZero);
            lbSumKashirRequests.Text = SumCount.ToString() + " м.п.";

        }

        public void FilterAssembledProfileAssignments()
        {
            bool bOnProd = cbFilterOnProd.Checked;
            bool bInProd = cbFilterInProd.Checked;
            bool bOnStorage = cbFilterOnStorage.Checked;

            //ProfileAssignmentsManager.FilterAssembledProfileAssignments(bOnProd, bInProd, bOnStorage);

            int SumCount = 0;
            for (int i = 0; i < dgvAssembledProfileAssignments.Rows.Count; i++)
            {
                if (dgvAssembledProfileAssignments.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToInt32(dgvAssembledProfileAssignments.Rows[i].Cells["PlanCount"].Value);
                }
            }
            lbSumAssembly1.Text = SumCount.ToString() + " шт.";

            SumCount = 0;
            for (int i = 0; i < dgvAssembledProfileRequests.Rows.Count; i++)
            {
                if (dgvAssembledProfileRequests.Rows[i].Cells["PlanCount"].Value != DBNull.Value)
                {
                    SumCount += Convert.ToInt32(dgvAssembledProfileRequests.Rows[i].Cells["PlanCount"].Value);
                }
            }
            lbSumAssemblyRequests.Text = SumCount.ToString() + " шт.";
        }

        private void dgvAssembledProfileAssignments_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            string Parameter = string.Empty;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Length2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Width2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        private void dgvAssembledProfileAssignments_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["ProductType"].Value = 3;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void dgvAssembledProfileAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu7.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvAssembledProfileRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            int ClientID = 0;
            int MegaOrderID = 0;
            int MainOrderID = 0;
            int DecorOrderID = 0;
            int TechStoreID2 = 0;
            int Length2 = 0;
            int PlanCount = 0;
            //bool AlreadyAdded = false;

            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                ClientID = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["ClientID"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["MegaOrderID"].Value != DBNull.Value)
                MegaOrderID = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["MegaOrderID"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["MainOrderID"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorOrderID"].Value != DBNull.Value)
                DecorOrderID = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorOrderID"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["TechStoreID2"].Value != DBNull.Value)
                TechStoreID2 = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["TechStoreID2"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                Length2 = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["Length2"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                ArrayList TechStoreIDs = new ArrayList();
                bool b = ProfileAssignmentsManager.FindShroudedProfileID(TechStoreID2, 1, TechStoreIDs);
                if (b)
                {
                    ProfileAssignmentsManager.AddAssembledProfileToPlan(DecorAssignmentID, TechStoreIDs);
                    for (int i = 0; i < TechStoreIDs.Count; i++)
                    {
                        //AlreadyAdded = ProfileAssignmentsManager.IsShroudedProfileAdded(DecorAssignmentID);
                        //if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddShroudedProfileToRequestFromAssembly(ClientID, MegaOrderID, MainOrderID, DecorOrderID, Convert.ToInt32(TechStoreIDs[i]), Length2, PlanCount, DecorAssignmentID);
                    }
                }
                else
                {
                    ProfileAssignmentsManager.FindMilledProfileID(TechStoreID2, 1, TechStoreIDs);
                    ProfileAssignmentsManager.AddAssembledProfileToPlan(DecorAssignmentID, TechStoreIDs);
                    for (int i = 0; i < TechStoreIDs.Count; i++)
                    {
                        //AlreadyAdded = ProfileAssignmentsManager.IsMilledProfileAdded(DecorAssignmentID);
                        //if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddMilledProfileToRequestFromAssembly(ClientID, MegaOrderID, MainOrderID, DecorOrderID, Convert.ToInt32(TechStoreIDs[i]), Length2, PlanCount, DecorAssignmentID);
                    }
                }
            }
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvShroudedProfileAssignments1.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvShroudedProfileAssignments1.SelectedRows.Count; i++)
            {
                if (dgvShroudedProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvShroudedProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvShroudedProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            //for (int i = 0; i < dgvShroudedProfileAssignments1.SelectedRows.Count; i++)
            //{
            //    int DecorAssignmentID = 0;

            //if (dgvShroudedProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //    DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            //if (DecorAssignmentID == 0)
            //    return;
            //if (ProfileAssignmentsManager.IsShroudedProfileInProduction(DecorAssignmentID))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
            //    return;
            //}
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);
            //}
        }

        private void kryptonContextMenuItem15_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvShroudedProfileAssignments2.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvShroudedProfileAssignments2.SelectedRows.Count; i++)
            {
                if (dgvShroudedProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvShroudedProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvShroudedProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            //for (int i = 0; i < dgvShroudedProfileAssignments2.SelectedRows.Count; i++)
            //{
            //int DecorAssignmentID = 0;

            //if (dgvShroudedProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //    DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            //if (DecorAssignmentID == 0)
            //    return;
            //if (ProfileAssignmentsManager.IsShroudedProfileInProduction(DecorAssignmentID))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
            //    return;
            //}
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);
            //}
        }

        private void kryptonContextMenuItem16_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvMilledProfileAssignments1.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvMilledProfileAssignments1.SelectedRows.Count; i++)
            {
                if (dgvMilledProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvMilledProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvMilledProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            //for (int i = 0; i < dgvMilledProfileAssignments1.SelectedRows.Count; i++)
            //{
            //    int DecorAssignmentID = 0;

            //    if (dgvMilledProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //        DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments1.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            //    if (DecorAssignmentID == 0)
            //        return;
            //    if (ProfileAssignmentsManager.IsMilledProfileInProduction(DecorAssignmentID))
            //    {
            //        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
            //        return;
            //    }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);

            //}
        }

        private void kryptonContextMenuItem17_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvMilledProfileAssignments2.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvMilledProfileAssignments2.SelectedRows.Count; i++)
            {
                if (dgvMilledProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvMilledProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvMilledProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            //for (int i = 0; i < dgvMilledProfileAssignments2.SelectedRows.Count; i++)
            //{
            //    int DecorAssignmentID = 0;

            //    if (dgvMilledProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //        DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments2.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            //    if (DecorAssignmentID == 0)
            //        return;
            //    if (ProfileAssignmentsManager.IsMilledProfileInProduction(DecorAssignmentID))
            //    {
            //        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
            //        return;
            //    }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);


            //}
        }

        private void kryptonContextMenuItem18_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvMilledProfileAssignments3.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvMilledProfileAssignments3.SelectedRows.Count; i++)
            {
                if (dgvMilledProfileAssignments3.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvMilledProfileAssignments3.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvMilledProfileAssignments3.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            //for (int i = 0; i < dgvMilledProfileAssignments3.SelectedRows.Count; i++)
            //{

            //    int DecorAssignmentID = 0;

            //    if (dgvMilledProfileAssignments3.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //        DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments3.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            //    if (DecorAssignmentID == 0)
            //        return;
            //    if (ProfileAssignmentsManager.IsMilledProfileInProduction(DecorAssignmentID))
            //    {
            //        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
            //        return;
            //    }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);
            //}
        }

        private void kryptonContextMenuItem19_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvSawnStripsAssignments.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvSawnStripsAssignments.SelectedRows.Count; i++)
            {
                if (dgvSawnStripsAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvSawnStripsAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvSawnStripsAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            //for (int i = 0; i < dgvSawnStripsAssignments.SelectedRows.Count; i++)
            //{
            //    int DecorAssignmentID = 0;

            //    if (dgvSawnStripsAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //        DecorAssignmentID = Convert.ToInt32(dgvSawnStripsAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            //    if (DecorAssignmentID == 0)
            //        return;
            //    if (ProfileAssignmentsManager.IsSawnStripsInProduction(DecorAssignmentID))
            //    {
            //        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
            //        return;
            //    }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);
            //}
        }

        private void kryptonContextMenuItem20_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvAssembledProfileAssignments.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvAssembledProfileAssignments.SelectedRows.Count; i++)
            {
                if (dgvAssembledProfileAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvAssembledProfileAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvAssembledProfileAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);
            //}
        }

        private void dgvMilledProfileRequests_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu8.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvAssembledProfileRequests_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvSawnStripsRequests_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvShroudedProfileRequests_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu11.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem23_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            //int DecorAssignmentID = 0;

            int[] DecorAssignmentID = new int[dgvMilledProfileRequests.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvMilledProfileRequests.SelectedRows.Count; i++)
            {
                if (dgvMilledProfileRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            }
            //if (dgvMilledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //    DecorAssignmentID = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            //if (DecorAssignmentID == 0)
            //    return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Заявка будет удалена. Продолжить?",
                    "Удаление заявки");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.RemoveAssigments(DecorAssignmentID);
        }

        private void kryptonContextMenuItem24_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            //int DecorAssignmentID = 0;

            //if (dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //    DecorAssignmentID = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            //if (DecorAssignmentID == 0)
            //    return;
            int[] DecorAssignmentID = new int[dgvAssembledProfileRequests.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvAssembledProfileRequests.SelectedRows.Count; i++)
            {
                if (dgvAssembledProfileRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Заявка будет удалена. Продолжить?",
                    "Удаление заявки");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.RemoveAssigments(DecorAssignmentID);
        }

        private void kryptonContextMenuItem26_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            //int DecorAssignmentID = 0;

            //if (dgvSawnStripsRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //    DecorAssignmentID = Convert.ToInt32(dgvSawnStripsRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            //if (DecorAssignmentID == 0)
            //    return;
            int[] DecorAssignmentID = new int[dgvSawnStripsRequests.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvSawnStripsRequests.SelectedRows.Count; i++)
            {
                if (dgvSawnStripsRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvSawnStripsRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Заявка будет удалена. Продолжить?",
                    "Удаление заявки");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.RemoveAssigments(DecorAssignmentID);
        }

        private void kryptonContextMenuItem28_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            //int DecorAssignmentID = 0;

            //if (dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            //    DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            //if (DecorAssignmentID == 0)
            //    return;
            int[] DecorAssignmentID = new int[dgvShroudedProfileRequests.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvShroudedProfileRequests.SelectedRows.Count; i++)
            {
                if (dgvShroudedProfileRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Заявка будет удалена. Продолжить?",
                    "Удаление заявки");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.RemoveAssigments(DecorAssignmentID);
        }

        private void kryptonContextMenuItem29_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;
            int NewCount = 0;
            int PlanCount = 0;

            if (dgvMilledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvMilledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvMilledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0 || PlanCount == 0)
                return;

            bool OKSplit = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, PlanCount);
            TopForm = SplitAssignmentRequestMenu;
            SplitAssignmentRequestMenu.ShowDialog();

            OKSplit = SplitAssignmentRequestMenu.OKSplit;
            NewCount = SplitAssignmentRequestMenu.Count;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SplitAssignmentRequestMenu.Dispose();
            TopForm = null;

            if (!OKSplit || NewCount == PlanCount || NewCount == 0)
                return;

            ProfileAssignmentsManager.SplitRequest(DecorAssignmentID, NewCount);
        }

        private void kryptonContextMenuItem30_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;
            int NewCount = 0;
            int PlanCount = 0;

            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvAssembledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvAssembledProfileRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0 || PlanCount == 0)
                return;

            bool OKSplit = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, PlanCount);
            TopForm = SplitAssignmentRequestMenu;
            SplitAssignmentRequestMenu.ShowDialog();

            OKSplit = SplitAssignmentRequestMenu.OKSplit;
            NewCount = SplitAssignmentRequestMenu.Count;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SplitAssignmentRequestMenu.Dispose();
            TopForm = null;

            if (!OKSplit || NewCount == PlanCount || NewCount == 0)
                return;

            ProfileAssignmentsManager.SplitRequest(DecorAssignmentID, NewCount);
        }

        private void kryptonContextMenuItem31_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;
            int NewCount = 0;
            int PlanCount = 0;

            if (dgvSawnStripsRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvSawnStripsRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvSawnStripsRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvSawnStripsRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0 || PlanCount == 0)
                return;

            bool OKSplit = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, PlanCount);
            TopForm = SplitAssignmentRequestMenu;
            SplitAssignmentRequestMenu.ShowDialog();

            OKSplit = SplitAssignmentRequestMenu.OKSplit;
            NewCount = SplitAssignmentRequestMenu.Count;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SplitAssignmentRequestMenu.Dispose();
            TopForm = null;

            if (!OKSplit || NewCount == PlanCount || NewCount == 0)
                return;

            ProfileAssignmentsManager.SplitRequest(DecorAssignmentID, NewCount);
        }

        private void kryptonContextMenuItem32_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;
            int NewCount = 0;
            int PlanCount = 0;

            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvShroudedProfileRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvShroudedProfileRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0 || PlanCount == 0)
                return;

            bool OKSplit = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, PlanCount);
            TopForm = SplitAssignmentRequestMenu;
            SplitAssignmentRequestMenu.ShowDialog();

            OKSplit = SplitAssignmentRequestMenu.OKSplit;
            NewCount = SplitAssignmentRequestMenu.Count;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SplitAssignmentRequestMenu.Dispose();
            TopForm = null;

            if (!OKSplit || NewCount == PlanCount || NewCount == 0)
                return;

            ProfileAssignmentsManager.SplitRequest(DecorAssignmentID, NewCount);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            pnlFilterAssignments.Visible = MenuButton.Checked;
            if (MenuButton.Checked)
                pnlFilterAssignments.BringToFront();
        }

        private void UpdateDecorAssignments()
        {
            ProfileAssignmentsManager.UpdateDecorAssignments();
            ProfileAssignmentsManager.GetKashirOnStorage();
            ProfileAssignmentsManager.GetMdfPlateOnStorage();
            ProfileAssignmentsManager.GetMilledProfilesOnStorage();
            ProfileAssignmentsManager.GetSawnStripsOnStorage();
            ProfileAssignmentsManager.GetShroudedProfilesOnStorage();
            ProfileAssignmentsManager.GetAssembledProfilesOnStorage();
            ProfileAssignmentsManager.GroupKashir();
            ProfileAssignmentsManager.GroupMdfPlate();
            ProfileAssignmentsManager.GroupMilledProfiles();
            ProfileAssignmentsManager.GroupSawnStrips();
            ProfileAssignmentsManager.GroupShroudedProfiles();
            ProfileAssignmentsManager.GroupAssembledProfiles();
            ProfileAssignmentsManager.GetFacingMaterialOnStorage();
            ProfileAssignmentsManager.GetFilterFacingMaterial();
            ProfileAssignmentsManager.GetFacingRollersOnStorage();
            ProfileAssignmentsManager.GetFilterFacingRollers();

            int DecorAssignmentID = 0;

            if (dgvFacingRollersRequests.SelectedRows.Count > 0 && dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            FilterFacingRollersAssignments(DecorAssignmentID);
            DecorAssignmentID = 0;

            if (dgvFacingMaterialRequests.SelectedRows.Count > 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            FilterFacingMaterialAssignments(DecorAssignmentID);
            FilterByProductionStatus();
            FilterShroudedProfileAssignments();
            FilterMilledProfileAssignments();
            FilterAssembledProfileAssignments();
            FilterSawStripsAssignments();
            FilterKashirAssignments();

        }

        private void btnFilterAssignments_Click(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void dgvShroudedProfileAssignments1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void dgvShroudedProfileAssignments2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
        }

        private void dgvSawnStripsAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void dgvAssembledProfileAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void dgvMilledProfileAssignments1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void dgvMilledProfileAssignments2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void dgvMilledProfileAssignments3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void kryptonContextMenuItem33_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvShroudedProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ShroudedProfileToBarberan(DecorAssignmentID, 2);
        }

        private void kryptonContextMenuItem34_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvShroudedProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ShroudedProfileToBarberan(DecorAssignmentID, 1);
        }

        private void kryptonContextMenuItem36_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.MilledProfileToFrezer(DecorAssignmentID, 2);
        }

        private void kryptonContextMenuItem35_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.MilledProfileToFrezer(DecorAssignmentID, 3);
        }

        private void kryptonContextMenuItem37_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.MilledProfileToFrezer(DecorAssignmentID, 1);
        }

        private void kryptonContextMenuItem38_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.MilledProfileToFrezer(DecorAssignmentID, 3);
        }

        private void kryptonContextMenuItem39_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments3.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments3.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.MilledProfileToFrezer(DecorAssignmentID, 1);
        }

        private void kryptonContextMenuItem40_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments3.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments3.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет перенесено на другой станок. Продолжить?",
                    "Перенос задания");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.MilledProfileToFrezer(DecorAssignmentID, 2);
        }

        private void btnBackToMainPage_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows.Count > 0 && dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);
            dgvBatchAssignments.Refresh();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            pnlBatchAssignmentsMainPage.BringToFront();
            PanelActive = 0;
        }

        private Color GetPerColor(decimal Value)
        {
            if (Value == 100)
                return Color.FromArgb(26, 228, 28);
            if (Value >= 90 && Value <= 99)
                return Color.FromArgb(169, 242, 14);
            if (Value >= 80 && Value <= 89)
                return Color.FromArgb(255, 242, 0);
            if (Value >= 70 && Value <= 79)
                return Color.FromArgb(255, 211, 0);
            if (Value >= 60 && Value <= 69)
                return Color.FromArgb(255, 173, 0);
            if (Value >= 50 && Value <= 59)
                return Color.FromArgb(255, 133, 0);
            if (Value >= 40 && Value <= 49)
                return Color.FromArgb(255, 101, 0);
            if (Value >= 30 && Value <= 39)
                return Color.FromArgb(255, 80, 0);
            if (Value >= 20 && Value <= 29)
                return Color.FromArgb(255, 52, 0);
            if (Value >= 0 && Value <= 19)
                return Color.FromArgb(255, 0, 0);
            return Color.Black;
        }

        private void dgvBatchAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                //ProfileAssignmentsManager.MoveToBatchAssignmentPos(e.RowIndex);
                kryptonContextMenu12.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }

            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            //Point p1 = grid.PointToScreen(grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.RowIndex != -1 && grid.Columns[e.ColumnIndex].Name == "ReadyPerc")
            {
                ProfileAssignmentsManager.MoveToBatchAssignmentPos(e.RowIndex);
                int BatchAssignmentID = 0;

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

                if (dgvBatchAssignments.SelectedRows.Count > 0 && dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                    BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
                if (BatchAssignmentID == 0)
                    return;
                label40.Text = "Партия № " + BatchAssignmentID;
                decimal ReadyPerc = 0;
                int MaxHeight = 33;
                int MaxWidth = 0;

                if (ProfileAssignmentsManager.GetSawReady(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel5.Visible = true;
                    label42.Text = ReadyPerc + " %";
                    label41.BackColor = GetPerColor(ReadyPerc);
                    label41.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label30.Width + label42.Width + label41.Width) > MaxWidth)
                        MaxWidth = label30.Width + label42.Width + label41.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetFrezerReady(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel7.Visible = true;
                    label31.Text = ReadyPerc + " %";
                    label44.BackColor = GetPerColor(ReadyPerc);
                    label44.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label43.Width + label44.Width + label31.Width) > MaxWidth)
                        MaxWidth = label43.Width + label44.Width + label31.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetFrezer1Ready(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel8.Visible = true;
                    label32.Text = ReadyPerc + " %";
                    label47.BackColor = GetPerColor(ReadyPerc);
                    label47.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label46.Width + label47.Width + label32.Width) > MaxWidth)
                        MaxWidth = label46.Width + label47.Width + label32.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetFrezer2Ready(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel9.Visible = true;
                    label33.Text = ReadyPerc + " %";
                    label50.BackColor = GetPerColor(ReadyPerc);
                    label50.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label49.Width + label50.Width + label33.Width) > MaxWidth)
                        MaxWidth = label49.Width + label50.Width + label33.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetFrezer3Ready(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel10.Visible = true;
                    label34.Text = ReadyPerc + " %";
                    label53.BackColor = GetPerColor(ReadyPerc);
                    label53.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label52.Width + label53.Width + label34.Width) > MaxWidth)
                        MaxWidth = label52.Width + label53.Width + label34.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetAssemblyReady(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel11.Visible = true;
                    label35.Text = ReadyPerc + " %";
                    label56.BackColor = GetPerColor(ReadyPerc);
                    label56.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label55.Width + label56.Width + label35.Width) > MaxWidth)
                        MaxWidth = label55.Width + label56.Width + label35.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetBarberanReady(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel12.Visible = true;
                    label36.Text = ReadyPerc + " %";
                    label59.BackColor = GetPerColor(ReadyPerc);
                    label59.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label58.Width + label59.Width + label36.Width) > MaxWidth)
                        MaxWidth = label58.Width + label59.Width + label36.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetBarberan1Ready(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel13.Visible = true;
                    label37.Text = ReadyPerc + " %";
                    label62.BackColor = GetPerColor(ReadyPerc);
                    label62.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label61.Width + label62.Width + label37.Width) > MaxWidth)
                        MaxWidth = label61.Width + label62.Width + label37.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetBarberan2Ready(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel14.Visible = true;
                    label38.Text = ReadyPerc + " %";
                    label65.BackColor = GetPerColor(ReadyPerc);
                    label65.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label64.Width + label65.Width + label38.Width) > MaxWidth)
                        MaxWidth = label64.Width + label65.Width + label38.Width;
                }


                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetFacingReady(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel16.Visible = true;
                    label25.Text = ReadyPerc + " %";
                    label24.BackColor = GetPerColor(ReadyPerc);
                    label24.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label14.Width + label59.Width + label25.Width) > MaxWidth)
                        MaxWidth = label14.Width + label24.Width + label25.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetFacing1Ready(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel17.Visible = true;
                    label57.Text = ReadyPerc + " %";
                    label54.BackColor = GetPerColor(ReadyPerc);
                    label54.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label51.Width + label54.Width + label57.Width) > MaxWidth)
                        MaxWidth = label51.Width + label54.Width + label57.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetFacing2Ready(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel18.Visible = true;
                    label66.Text = ReadyPerc + " %";
                    label63.BackColor = GetPerColor(ReadyPerc);
                    label63.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label60.Width + label63.Width + label66.Width) > MaxWidth)
                        MaxWidth = label60.Width + label63.Width + label66.Width;
                }

                ReadyPerc = 0;
                if (ProfileAssignmentsManager.GetKashirReady(BatchAssignmentID, ref ReadyPerc))
                {
                    flowLayoutPanel15.Visible = true;
                    label39.Text = ReadyPerc + " %";
                    label48.BackColor = GetPerColor(ReadyPerc);
                    label48.Width = Convert.ToInt32(1.1m * ReadyPerc);
                    MaxHeight += 28;
                    if ((label45.Width + label48.Width + label39.Width) > MaxWidth)
                        MaxWidth = label45.Width + label48.Width + label39.Width;
                }
                if (MaxWidth < 220)
                    MaxWidth = 220;
                if (MaxHeight < 35)
                    MaxHeight = 35;
                //panel62.BringToFront();
                panel62.Location = new Point(cellRectangle.Left + 15, cellRectangle.Top + 15);
                panel62.Size = new System.Drawing.Size(MaxWidth + 25, MaxHeight);
                flowLayoutPanel4.Size = new System.Drawing.Size(MaxWidth + 25, MaxHeight - 28);

                panel62.Visible = true;
            }
            else
            {
                if (panel62.Visible)
                    panel62.Visible = false;
            }
        }

        private void kryptonContextMenuItem41_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.Ordinary)
                return;
            OpenBatchAssignment();
        }

        private void OpenBatchAssignment()
        {
            bool BatchEnable = false;
            int BatchAssignmentID = 0;
            object BatchDateTime = DBNull.Value;

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchEnable"].Value != DBNull.Value)
                BatchEnable = Convert.ToBoolean(dgvBatchAssignments.SelectedRows[0].Cells["BatchEnable"].Value);
            BatchDateTime = dgvBatchAssignments.SelectedRows[0].Cells["CreateDate"].Value;
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.CurrentBatchAssignmentID = BatchAssignmentID;
            ProfileAssignmentsManager.CurrentBatchDateTime = BatchDateTime;
            ProfileAssignmentsManager.BatchEnable = BatchEnable;
            UpdateDecorAssignments();

            FilterFacingMaterialOnStorage();

            FilterFacingRollersOnStorage();

            panel62.Visible = false;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            pnlBatchAssignmentsBrowsing.BringToFront();
            PanelActive = 1;
        }

        private void kryptonContextMenuItem46_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.ConfirmTools(BatchAssignmentID);
            ProfileAssignmentsManager.SaveBatchAssignments();
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem44_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.ConfirmTechnology(BatchAssignmentID);
            ProfileAssignmentsManager.SaveBatchAssignments();
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem45_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.ConfirmMaterial(BatchAssignmentID);
            ProfileAssignmentsManager.SaveBatchAssignments();
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem42_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.BatchAssigmentToProduction(BatchAssignmentID);
            ProfileAssignmentsManager.SaveBatchAssignments();
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvKashirRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            int ClientID = 0;
            int MegaOrderID = 0;
            int MainOrderID = 0;
            int DecorOrderID = 0;
            int TechStoreID1 = 0;
            int TechStoreID2 = 0;
            int Length2 = 0;
            decimal Width2 = 0;
            int PlanCount = 0;
            bool AlreadyAdded = false;

            if (senderGrid.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                ClientID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["ClientID"].Value);
            if (senderGrid.SelectedRows[0].Cells["MegaOrderID"].Value != DBNull.Value)
                MegaOrderID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["MegaOrderID"].Value);
            if (senderGrid.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["MainOrderID"].Value);
            if (senderGrid.SelectedRows[0].Cells["DecorOrderID"].Value != DBNull.Value)
                DecorOrderID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["DecorOrderID"].Value);
            if (senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (senderGrid.SelectedRows[0].Cells["TechStoreID2"].Value != DBNull.Value)
                TechStoreID2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["TechStoreID2"].Value);
            if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                Length2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
            if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                TechStoreID1 = ProfileAssignmentsManager.FindSawStripID(TechStoreID2, 1, ref Width2);
                if (TechStoreID1 != 0)
                {
                    AlreadyAdded = ProfileAssignmentsManager.IsAssignmentAdded(DecorAssignmentID);
                    if (!AlreadyAdded)
                        ProfileAssignmentsManager.AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                }
                ProfileAssignmentsManager.AddKashirToPlan(DecorAssignmentID);
            }
        }

        private void dgvKashirRequests_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu13.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvKashirAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu14.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvKashirAssignments_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            string Parameter = string.Empty;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Length2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            {
                int NextLinkAssignmentID = 0;
                int PrevLinkAssignmentID = 0;
                decimal NewValue = 0;
                Parameter = "Width2";
                if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
                    NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                    PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
                if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
                if (NextLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
                if (PrevLinkAssignmentID != 0)
                    ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            }
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        private void dgvKashirAssignments_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["ProductType"].Value = 5;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void dgvKashirAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void kryptonContextMenuItem48_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int DecorAssignmentID = 0;
            int NewCount = 0;
            int PlanCount = 0;

            if (dgvKashirRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvKashirRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvKashirRequests.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(dgvKashirRequests.SelectedRows[0].Cells["PlanCount"].Value);
            if (DecorAssignmentID == 0 || PlanCount == 0)
                return;

            bool OKSplit = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, PlanCount);
            TopForm = SplitAssignmentRequestMenu;
            SplitAssignmentRequestMenu.ShowDialog();

            OKSplit = SplitAssignmentRequestMenu.OKSplit;
            NewCount = SplitAssignmentRequestMenu.Count;

            PhantomForm.Close();
            PhantomForm.Dispose();
            SplitAssignmentRequestMenu.Dispose();
            TopForm = null;

            if (!OKSplit || NewCount == PlanCount || NewCount == 0)
                return;

            ProfileAssignmentsManager.SplitRequest(DecorAssignmentID, NewCount);
        }

        private void kryptonContextMenuItem49_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvKashirRequests.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvKashirRequests.SelectedRows.Count; i++)
            {
                if (dgvKashirRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvKashirRequests.SelectedRows[i].Cells["DecorAssignmentID"].Value);
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Заявка будет удалена. Продолжить?",
                    "Удаление заявки");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.RemoveAssigments(DecorAssignmentID);
        }

        private void kryptonContextMenuItem52_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            int[] DecorAssignmentID = new int[dgvKashirAssignments.SelectedRows.Count];
            int j = 0;
            for (int i = 0; i < dgvKashirAssignments.SelectedRows.Count; i++)
            {
                if (dgvKashirAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value != DBNull.Value)
                {
                    if (ProfileAssignmentsManager.IsAssigmentInProduction(Convert.ToInt32(dgvKashirAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value)))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false, "Задание уже отдано в работу", "Ошибка удаления");
                        return;
                    }
                    DecorAssignmentID[j++] = Convert.ToInt32(dgvKashirAssignments.SelectedRows[i].Cells["DecorAssignmentID"].Value);
                }
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет возвращено в заявки. Продолжить?",
                    "Исключение из заданий");

            if (!OKCancel)
                return;
            ProfileAssignmentsManager.ExcludeFromAssignments(DecorAssignmentID);
            //}
        }

        private void kryptonContextMenuItem53_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы собираетесь создать новое задание. Продолжить?",
                    "Новое задание");

            if (!OKCancel)
                return;
            int BatchAssignmentID = 0;
            object BatchDateTime = DBNull.Value;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.CreateBatchAssignment();
            ProfileAssignmentsManager.SaveBatchAssignments();
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToFirstBatchAssignmentID();

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            BatchDateTime = dgvBatchAssignments.SelectedRows[0].Cells["CreateDate"].Value;
            ProfileAssignmentsManager.CurrentBatchAssignmentID = BatchAssignmentID;
            ProfileAssignmentsManager.CurrentBatchDateTime = BatchDateTime;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

        }

        private void dgvPackingAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu14.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvPackingAssignments_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["ProductType"].Value = 6;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 1;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
        }

        private void dgvPackingAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void kryptonCheckSet4_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;
            if (kryptonCheckSet4.CheckedButton == cbtnFacingMaterial)
            {
                pnlFacingMaterial.BringToFront();
                panel63.Visible = false;
                panel64.Visible = true;
            }
            if (kryptonCheckSet4.CheckedButton == cbtnFacingRollers)
            {
                pnlFacingRollers.BringToFront();
                panel63.Visible = true;
                panel64.Visible = false;
            }
        }

        private void kryptonContextMenuItem68_Click(object sender, EventArgs e)
        {
            int DecorAssignmentID = 0;

            if (dgvSawnStripsAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvSawnStripsAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;

            bool OkComplexSawing = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ComplexSawingForm ComplexSawingForm = new ComplexSawingForm(ProfileAssignmentsManager, DecorAssignmentID);
            TopForm = ComplexSawingForm;
            ComplexSawingForm.ShowDialog();

            OkComplexSawing = ComplexSawingForm.OkComplexSawing;

            PhantomForm.Close();
            PhantomForm.Dispose();
            ComplexSawingForm.Dispose();
            TopForm = null;

            if (!OkComplexSawing)
                return;
            ProfileAssignmentsManager.SaveComplexSawing(DecorAssignmentID);
            ProfileAssignmentsManager.SetComplexSawing(DecorAssignmentID);
            ProfileAssignmentsManager.SaveAssignments();
            ProfileAssignmentsManager.UpdateAssignments();
        }

        private void dgvBatchAssignments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (RoleType == RoleTypes.Ordinary)
                return;
            if (kryptonCheckSet1.CheckedButton == cbtnHolzma)
            {
                pnlCutting.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnBarberan)
            {
                pnlEnveloping.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnFrezer)
            {
                pnlMeeling.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnAssembly)
            {
                pnlAssembly.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnKashir)
            {
                pnlLaminating.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnPaperCutting)
            {
                pnlPaperCutting.BringToFront();
            }
            OpenBatchAssignment();
        }

        private void dgvFacingMaterialOnStorage_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int StoreID = 0;
            int TechStoreID2 = 0;
            decimal Thickness2 = 0;
            int Length2 = 0;
            decimal Width2 = 0;
            int PlanCount = 0;

            if (senderGrid.SelectedRows[0].Cells["StoreID"].Value != DBNull.Value)
                StoreID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["StoreID"].Value);
            if (senderGrid.SelectedRows[0].Cells["StoreItemID"].Value != DBNull.Value)
                TechStoreID2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["StoreItemID"].Value);
            if (senderGrid.SelectedRows[0].Cells["Thickness"].Value != DBNull.Value)
                Thickness2 = Convert.ToDecimal(senderGrid.SelectedRows[0].Cells["Thickness"].Value);
            if (senderGrid.SelectedRows[0].Cells["Length"].Value != DBNull.Value)
                Length2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length"].Value);
            if (senderGrid.SelectedRows[0].Cells["Width"].Value != DBNull.Value)
                Width2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width"].Value);
            if (senderGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["CurrentCount"].Value);

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                bool OKSplit = false;
                int OldCount = 0;

                if (senderGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                    OldCount = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, OldCount);
                TopForm = SplitAssignmentRequestMenu;
                SplitAssignmentRequestMenu.ShowDialog();

                OKSplit = SplitAssignmentRequestMenu.OKSplit;
                PlanCount = SplitAssignmentRequestMenu.Count;

                PhantomForm.Close();
                PhantomForm.Dispose();
                SplitAssignmentRequestMenu.Dispose();
                TopForm = null;
                if (!OKSplit)
                    return;

                if (!ProfileAssignmentsManager.AddFacingMaterialToRequest(TechStoreID2, Thickness2, Length2, Width2, PlanCount, StoreID, "TF-1300", senderGrid.SelectedRows[0].Cells["Notes"].Value.ToString()))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Эта позиция уже была добавлена", "Ошибка добавления");
                    return;
                }

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                UpdateFacingMaterial();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Column2" && e.RowIndex >= 0)
            {
                bool OKSplit = false;
                int OldCount = 0;

                if (senderGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                    OldCount = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, OldCount);
                TopForm = SplitAssignmentRequestMenu;
                SplitAssignmentRequestMenu.ShowDialog();

                OKSplit = SplitAssignmentRequestMenu.OKSplit;
                PlanCount = SplitAssignmentRequestMenu.Count;

                PhantomForm.Close();
                PhantomForm.Dispose();
                SplitAssignmentRequestMenu.Dispose();
                TopForm = null;
                if (!OKSplit)
                    return;

                if (!ProfileAssignmentsManager.AddFacingMaterialToRequest(TechStoreID2, Thickness2, Length2, Width2, PlanCount, StoreID, "Пила", senderGrid.SelectedRows[0].Cells["Notes"].Value.ToString()))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Эта позиция уже была добавлена", "Ошибка добавления");
                    return;
                }

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                UpdateFacingMaterial();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void dgvFacingMaterialAssignments_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["InPlan"].Value = 1;
            e.Row.Cells["ProductType"].Value = 7;
            e.Row.Cells["DecorAssignmentStatusID"].Value = 2;
            e.Row.Cells["BatchAssignmentID"].Value = ProfileAssignmentsManager.CurrentBatchAssignmentID;
            int DecorAssignmentID = 0;
            string FacingMachine = string.Empty;

            if (dgvFacingMaterialRequests.SelectedRows.Count > 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (dgvFacingMaterialRequests.SelectedRows.Count > 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["FacingMachine"].Value != DBNull.Value)
                FacingMachine = dgvFacingMaterialRequests.SelectedRows[0].Cells["FacingMachine"].Value.ToString();
            if (DecorAssignmentID != 0)
            {
                e.Row.Cells["PrevLinkAssignmentID"].Value = DecorAssignmentID;
                e.Row.Cells["FacingMachine"].Value = FacingMachine;
            }
            else
            {

            }
            int D = dgvFacingMaterialAssignments.Rows.Count;
        }

        private void dgvFacingMaterialAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Insert)
            {
                ProfileAssignmentsManager.CopyPasteFacingMaterial();
            }
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        public void FilterFacingMaterialAssignments(int DecorAssignmentID)
        {
            bool bOnProd = cbFilterOnProd.Checked;
            bool bInProd = cbFilterInProd.Checked;
            bool bOnStorage = cbFilterOnStorage.Checked;

            ProfileAssignmentsManager.FilterFacingMaterialAssignments(bOnProd, bInProd, bOnStorage, DecorAssignmentID);
        }

        public void FilterFacingRollersAssignments(int DecorAssignmentID)
        {
            bool bOnProd = cbFilterOnProd.Checked;
            bool bInProd = cbFilterInProd.Checked;
            bool bOnStorage = cbFilterOnStorage.Checked;

            ProfileAssignmentsManager.FilterFacingRollersAssignments(bOnProd, bInProd, bOnStorage, DecorAssignmentID);
        }

        private void dgvFacingMaterialRequests_SelectionChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            int DecorAssignmentID = 0;

            if (dgvFacingMaterialRequests.SelectedRows.Count > 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            FilterFacingMaterialAssignments(DecorAssignmentID);
        }

        private void kryptonContextMenuItem43_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows.Count > 0 && dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            object ToolsConfirmDate = dgvBatchAssignments.SelectedRows[0].Cells["ToolsConfirmDate"].Value;
            object TechnologyConfirmDate = dgvBatchAssignments.SelectedRows[0].Cells["TechnologyConfirmDate"].Value;
            object MaterialConfirmDate = dgvBatchAssignments.SelectedRows[0].Cells["MaterialConfirmDate"].Value;
            object TechnicalConfirmDate = dgvBatchAssignments.SelectedRows[0].Cells["TechnicalConfirmDate"].Value;
            object ToolsConfirmUserID = dgvBatchAssignments.SelectedRows[0].Cells["ToolsConfirmUserID"].Value;
            object TechnologyConfirmUserID = dgvBatchAssignments.SelectedRows[0].Cells["TechnologyConfirmUserID"].Value;
            object MaterialConfirmUserID = dgvBatchAssignments.SelectedRows[0].Cells["MaterialConfirmUserID"].Value;
            object TechnicalConfirmUserID = dgvBatchAssignments.SelectedRows[0].Cells["TechnicalConfirmUserID"].Value;

            DecorlAssignmentsToExcel.GetAssignmentInfo(ref ToolsConfirmDate, ref TechnologyConfirmDate, ref MaterialConfirmDate, ref TechnicalConfirmDate,
                ref ToolsConfirmUserID, ref TechnologyConfirmUserID, ref MaterialConfirmUserID, ref TechnicalConfirmUserID);

            DateTime AddToPlanDateTime = Security.GetCurrentDate();
            DecorlAssignmentsToExcel.GetDecorAssignments(BatchAssignmentID);
            DecorlAssignmentsToExcel.CreateExcel(BatchAssignmentID, AddToPlanDateTime);
            ProfileAssignmentsManager.PrintBatchAssignments(BatchAssignmentID);
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            panel62.Visible = false;
        }

        private void dgvBatchAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.P)
                kryptonContextMenuItem43_Click(null, null);
        }

        private void dgvFacingRollersRequests_SelectionChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            int DecorAssignmentID = 0;

            if (dgvFacingRollersRequests.SelectedRows.Count > 0 && dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            FilterFacingRollersAssignments(DecorAssignmentID);
        }

        private void dgvFacingRollersOnStorage_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int StoreID = 0;
            int TechStoreID2 = 0;
            decimal Thickness2 = 0;
            decimal Diameter2 = 0;
            decimal Width2 = 0;
            int PlanCount = 0;

            if (senderGrid.SelectedRows[0].Cells["ManufactureStoreID"].Value != DBNull.Value)
                StoreID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["ManufactureStoreID"].Value);
            if (senderGrid.SelectedRows[0].Cells["StoreItemID"].Value != DBNull.Value)
                TechStoreID2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["StoreItemID"].Value);
            if (senderGrid.SelectedRows[0].Cells["Thickness"].Value != DBNull.Value)
                Thickness2 = Convert.ToDecimal(senderGrid.SelectedRows[0].Cells["Thickness"].Value);
            if (senderGrid.SelectedRows[0].Cells["Diameter"].Value != DBNull.Value)
                Diameter2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Diameter"].Value);
            if (senderGrid.SelectedRows[0].Cells["Width"].Value != DBNull.Value)
                Width2 = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width"].Value);
            if (senderGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                PlanCount = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["CurrentCount"].Value);

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                bool OKSplit = false;
                int OldCount = 0;

                if (senderGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                    OldCount = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, OldCount);
                TopForm = SplitAssignmentRequestMenu;
                SplitAssignmentRequestMenu.ShowDialog();

                OKSplit = SplitAssignmentRequestMenu.OKSplit;
                PlanCount = SplitAssignmentRequestMenu.Count;

                PhantomForm.Close();
                PhantomForm.Dispose();
                SplitAssignmentRequestMenu.Dispose();
                TopForm = null;
                if (!OKSplit)
                    return;

                if (!ProfileAssignmentsManager.AddFacingRollersToRequest(TechStoreID2, Thickness2, Diameter2, Width2, PlanCount, StoreID, senderGrid.SelectedRows[0].Cells["Notes"].Value.ToString()))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Эта позиция уже была добавлена", "Ошибка добавления");
                    return;
                }
            }
        }

        private void dgvFacingRollersAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Insert)
            {
                ProfileAssignmentsManager.CopyPasteFacingRollers();
            }
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                int DecorAssignmentID = 0;

                PercentageDataGrid grid = (PercentageDataGrid)sender;
                if (grid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(grid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                if (DecorAssignmentID == 0)
                {
                    foreach (DataGridViewRow item in grid.SelectedRows)
                    {
                        if (!item.IsNewRow)
                            grid.Rows.RemoveAt(item.Index);
                    }
                }
            }
        }

        private void cbFilterFacingRollers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            FilterFacingRollersOnStorage();
        }

        private void cbFilterFacingMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            FilterFacingMaterialOnStorage();
        }

        private void cbxShowAllFacingRollers_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            FilterFacingRollersOnStorage();
        }

        private void FilterFacingRollersOnStorage()
        {
            string Name = string.Empty;

            if (cbFilterFacingRollers.SelectedItem != null && ((DataRowView)cbFilterFacingRollers.SelectedItem).Row["TechStoreName"] != DBNull.Value)
                Name = ((DataRowView)cbFilterFacingRollers.SelectedItem).Row["TechStoreName"].ToString();
            ProfileAssignmentsManager.FilterFacingRollersOnStorage(cbxShowAllFacingRollers.Checked, Name);
        }

        private void cbxShowAllFacingMaterial_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            FilterFacingMaterialOnStorage();
        }

        private void FilterFacingMaterialOnStorage()
        {
            string Name = string.Empty;

            if (cbFilterFacingMaterial.SelectedItem != null && ((DataRowView)cbFilterFacingMaterial.SelectedItem).Row["TechStoreName"] != DBNull.Value)
                Name = ((DataRowView)cbFilterFacingMaterial.SelectedItem).Row["TechStoreName"].ToString();
            ProfileAssignmentsManager.FilterFacingMaterialOnStorage(cbxShowAllFacingMaterial.Checked, Name);
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.ConfirmTechnical(BatchAssignmentID);
            ProfileAssignmentsManager.SaveBatchAssignments();
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvBatchAssignments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns[e.ColumnIndex].Name == "BatchEnabledColumn")
            {
                if (grid.Rows[e.RowIndex].Cells["BatchEnable"].Value != null)
                {
                    if (!Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["BatchEnable"].Value))
                    {
                        kryptonContextMenuItem4.Tag = 1;
                        kryptonContextMenuItem4.Text = "Открыть";
                        e.Value = ImageList1.Images[0];
                    }
                    else
                    {
                        kryptonContextMenuItem4.Tag = 0;
                        kryptonContextMenuItem4.Text = "Закрыть";
                        e.Value = ImageList1.Images[1];
                    }
                }
            }
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            int BatchAssignmentID = 0;

            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value != DBNull.Value)
                BatchAssignmentID = Convert.ToInt32(dgvBatchAssignments.SelectedRows[0].Cells["BatchAssignmentID"].Value);
            if (BatchAssignmentID == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsManager.SetBatchEnable(BatchAssignmentID, Convert.ToBoolean(kryptonContextMenuItem4.Tag));
            ProfileAssignmentsManager.SaveBatchAssignments();
            ProfileAssignmentsManager.UpdateBatchAssignments();
            ProfileAssignmentsManager.GetAssignmentReadyTable();
            ProfileAssignmentsManager.MoveToBatchAssignmentID(BatchAssignmentID);
            dgvBatchAssignments.Refresh();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenu12_Opening(object sender, CancelEventArgs e)
        {
            if (RoleType == RoleTypes.Ordinary)
                return;
            if (dgvBatchAssignments.SelectedRows[0].Cells["BatchEnable"].Value != null)
            {
                if (!Convert.ToBoolean(dgvBatchAssignments.SelectedRows[0].Cells["BatchEnable"].Value))
                {
                    kryptonContextMenuItem4.Tag = 1;
                    kryptonContextMenuItem4.Text = "Открыть";
                }
                else
                {
                    kryptonContextMenuItem4.Tag = 0;
                    kryptonContextMenuItem4.Text = "Закрыть";
                }
            }
        }

        private void kryptonContextMenuItem7_Click(object sender, EventArgs e)
        {
            if (dgvMilledProfileAssignments3.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments3.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments3.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem11_Click(object sender, EventArgs e)
        {
            if (dgvMilledProfileAssignments1.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem9_Click(object sender, EventArgs e)
        {
            if (dgvMilledProfileAssignments2.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvMilledProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvMilledProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem51_Click(object sender, EventArgs e)
        {
            if (dgvShroudedProfileAssignments1.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvShroudedProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem13_Click(object sender, EventArgs e)
        {
            if (dgvShroudedProfileAssignments2.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvShroudedProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem57_Click(object sender, EventArgs e)
        {
            if (dgvAssembledProfileAssignments.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvAssembledProfileAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvAssembledProfileAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem58_Click(object sender, EventArgs e)
        {
            if (dgvKashirAssignments.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvKashirAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvKashirAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem55_Click(object sender, EventArgs e)
        {
            if (dgvSawnStripsAssignments.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;

            if (dgvSawnStripsAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvSawnStripsAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ProfileAssignmentsLabelsForm ProfileAssignmentsLabelsForm = new ProfileAssignmentsLabelsForm(this, ProfileAssignmentsManager.CurrentBatchAssignmentID, DecorAssignmentID, ProfileAssignmentsManager.CurrentBatchDateTime);

            TopForm = ProfileAssignmentsLabelsForm;

            ProfileAssignmentsLabelsForm.ShowDialog();

            ProfileAssignmentsLabelsForm.Close();
            ProfileAssignmentsLabelsForm.Dispose();

            TopForm = null;
        }

        private void dgvMilledProfileRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value == DBNull.Value)
                return;
            int BalanceStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value);

            if (BalanceStatus == 0)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                //grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                //grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            //if (BalanceStatus == 1)
            //{
            //    int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
            //    Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
            //        grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

            //    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(253, 164, 61);
            //    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            //}
            if (BalanceStatus == 1)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                //grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                //grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }

        private void dgvSawnStripsRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value == DBNull.Value)
                return;
            int BalanceStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value);

            if (BalanceStatus == 0)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (BalanceStatus == 1)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }

        private void dgvShroudedProfileRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value == DBNull.Value)
                return;
            int BalanceStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value);

            if (BalanceStatus == 0)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (BalanceStatus == 1)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }

        private void dgvAssembledProfileRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value == DBNull.Value)
                return;
            int BalanceStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value);

            if (BalanceStatus == 0)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (BalanceStatus == 1)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }

        private void dgvKashirRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value == DBNull.Value)
                return;
            int BalanceStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BalanceStatus"].Value);

            if (BalanceStatus == 0)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (BalanceStatus == 1)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }

        private void dgvFacingMaterialRequests_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
        }

        private void dgvFacingRollersRequests_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save(null, null);
        }

        private void cbFilterNotInProd_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbFilterOnExp_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbFilterDisp_CheckedChanged(object sender, EventArgs e)
        {
            if (ProfileAssignmentsManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            UpdateDecorAssignments();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvFacingMaterialRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            if (senderGrid.SelectedRows.Count > 0 && senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            {
                if (senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            }
            bool WriteOffFromStore = false;

            if (senderGrid.SelectedRows[0].Cells["WriteOffFromStore"].Value != DBNull.Value)
                WriteOffFromStore = Convert.ToBoolean(senderGrid.SelectedRows[0].Cells["WriteOffFromStore"].Value);

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                if (WriteOffFromStore)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Заявка была закрыта ранее и списана со склада.",
                        "Закрытие заявки");
                    return;
                }
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Заявка будет закрыта и списана со склада. Продолжить?",
                        "Закрытие заявки");

                if (!OKCancel)
                    return;
                ProfileAssignmentsManager.CloseFacingMaterialRequest(DecorAssignmentID);

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                ProfileAssignmentsManager.SaveFacingMaterialAssignments();
                ProfileAssignmentsManager.UpdateFacingMaterialAssignments();
                ProfileAssignmentsManager.MoveToFacingMaterialRequest(DecorAssignmentID);

                FilterFacingMaterialAssignments(DecorAssignmentID);

                ProfileAssignmentsManager.SaveFacingRollersAssignments();
                ProfileAssignmentsManager.UpdateFacingRollersAssignments();

                ProfileAssignmentsManager.GetFacingMaterialOnStorage();
                ProfileAssignmentsManager.GetFilterFacingMaterial();
                FilterFacingMaterialOnStorage();

                ProfileAssignmentsManager.GetFacingRollersOnStorage();
                ProfileAssignmentsManager.GetFilterFacingRollers();
                FilterFacingRollersOnStorage();
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
                NeedSplash = true;
            }
        }

        private void dgvFacingRollersRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            if (senderGrid.SelectedRows.Count > 0 && senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            {
                if (senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    DecorAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            }
            bool WriteOffFromStore = false;

            if (senderGrid.SelectedRows[0].Cells["WriteOffFromStore"].Value != DBNull.Value)
                WriteOffFromStore = Convert.ToBoolean(senderGrid.SelectedRows[0].Cells["WriteOffFromStore"].Value);

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                if (WriteOffFromStore)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Заявка была закрыта ранее и списана со склада.",
                        "Закрытие заявки");
                    return;
                }
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Заявка будет закрыта и списана со склада. Продолжить?",
                        "Закрытие заявки");

                if (!OKCancel)
                    return;
                ProfileAssignmentsManager.CloseFacingRollersRequest(DecorAssignmentID);

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                ProfileAssignmentsManager.SaveFacingRollersAssignments();
                ProfileAssignmentsManager.UpdateFacingRollersAssignments();
                ProfileAssignmentsManager.MoveToFacingRollersRequest(DecorAssignmentID);
                FilterFacingRollersAssignments(DecorAssignmentID);

                ProfileAssignmentsManager.GetFacingRollersOnStorage();
                ProfileAssignmentsManager.GetFilterFacingRollers();
                FilterFacingRollersOnStorage();
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
                NeedSplash = true;
            }
        }

        private void btnAddFacingRollersAssignment2_Click(object sender, EventArgs e)
        {
            int DecorAssignmentID = 0;
            int TechStoreID2 = 0;
            decimal Thickness2 = 0;
            decimal Diameter2 = 0;
            decimal Width2 = 0;

            if (dgvFacingRollersRequests.SelectedRows.Count > 0 && dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            {
                DecorAssignmentID = Convert.ToInt32(dgvFacingRollersRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                TechStoreID2 = Convert.ToInt32(dgvFacingRollersRequests.SelectedRows[0].Cells["TechStoreID2"].Value);
                if (dgvFacingRollersRequests.SelectedRows[0].Cells["Thickness2"].Value != DBNull.Value)
                    Thickness2 = Convert.ToDecimal(dgvFacingRollersRequests.SelectedRows[0].Cells["Thickness2"].Value);
                if (dgvFacingRollersRequests.SelectedRows[0].Cells["Diameter2"].Value != DBNull.Value)
                    Diameter2 = Convert.ToDecimal(dgvFacingRollersRequests.SelectedRows[0].Cells["Diameter2"].Value);
                if (dgvFacingRollersRequests.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
                    Width2 = Convert.ToDecimal(dgvFacingRollersRequests.SelectedRows[0].Cells["Width2"].Value);
                ProfileAssignmentsManager.AddFacingRollersToAssignment(DecorAssignmentID, TechStoreID2, Thickness2, Diameter2, Width2);
            }
        }

        private void btnAddFacingRollersAssignment1_Click(object sender, EventArgs e)
        {
            int DecorAssignmentID = 0;
            decimal Thickness2 = 0;
            string FacingMachine = string.Empty;

            if (dgvFacingMaterialRequests.SelectedRows.Count > 0 && dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
            {
                if (dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                    FacingMachine = dgvFacingMaterialRequests.SelectedRows[0].Cells["FacingMachine"].Value.ToString();
                if (dgvFacingMaterialRequests.SelectedRows[0].Cells["Thickness2"].Value != DBNull.Value)
                    Thickness2 = Convert.ToDecimal(dgvFacingMaterialRequests.SelectedRows[0].Cells["Thickness2"].Value);
                DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialRequests.SelectedRows[0].Cells["DecorAssignmentID"].Value);
                ProfileAssignmentsManager.AddFacingMaterialToAssignment(DecorAssignmentID, Thickness2, FacingMachine);
            }
        }

        private void kryptonContextMenuItem59_Click(object sender, EventArgs e)
        {
            if (dgvShroudedProfileAssignments1.SelectedRows.Count == 0)
                return;

            int CoverID = 0;
            int DecorAssignmentID = 0;

            if (dgvShroudedProfileAssignments1.SelectedRows[0].Cells["CoverID2"].Value != DBNull.Value)
                CoverID = Convert.ToInt32(dgvShroudedProfileAssignments1.SelectedRows[0].Cells["CoverID2"].Value);
            if (dgvShroudedProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments1.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ReturnedRollersForm ReturnedRollersForm = new ReturnedRollersForm(this, ProfileAssignmentsManager, CoverID, DecorAssignmentID);

            TopForm = ReturnedRollersForm;

            ReturnedRollersForm.ShowDialog();

            ReturnedRollersForm.Close();
            ReturnedRollersForm.Dispose();

            TopForm = null;

            UpdateFacingRollers();
        }

        private void kryptonContextMenuItem60_Click(object sender, EventArgs e)
        {
            if (dgvShroudedProfileAssignments2.SelectedRows.Count == 0)
                return;

            int CoverID = 0;
            int DecorAssignmentID = 0;

            if (dgvShroudedProfileAssignments2.SelectedRows[0].Cells["CoverID2"].Value != DBNull.Value)
                CoverID = Convert.ToInt32(dgvShroudedProfileAssignments2.SelectedRows[0].Cells["CoverID2"].Value);
            if (dgvShroudedProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvShroudedProfileAssignments2.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ReturnedRollersForm ReturnedRollersForm = new ReturnedRollersForm(this, ProfileAssignmentsManager, CoverID, DecorAssignmentID);

            TopForm = ReturnedRollersForm;

            ReturnedRollersForm.ShowDialog();

            ReturnedRollersForm.Close();
            ReturnedRollersForm.Dispose();

            TopForm = null;

            UpdateFacingRollers();
        }

        private void dgvSawnStripsAssignments_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.CurrentCell.ColumnIndex == senderGrid.Columns["Width1"].Index
                || senderGrid.CurrentCell.ColumnIndex == senderGrid.Columns["Width2"].Index
                || senderGrid.CurrentCell.ColumnIndex == senderGrid.Columns["Thickness2"].Index
                || senderGrid.CurrentCell.ColumnIndex == senderGrid.Columns["Diameter2"].Index)
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }
        }

        private void dgvSawnStripsAssignments_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //PercentageDataGrid senderGrid = (PercentageDataGrid)sender;
            //string Parameter = string.Empty;
            //if (senderGrid.Columns[e.ColumnIndex].Name == "Length2" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "Length2";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["Length2"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Length2"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
            //if (senderGrid.Columns[e.ColumnIndex].Name == "Width2" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "Width2";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["Width2"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width2"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
            //if (senderGrid.Columns[e.ColumnIndex].Name == "PlanCount" && e.RowIndex >= 0)
            //{
            //    int NextLinkAssignmentID = 0;
            //    int PrevLinkAssignmentID = 0;
            //    decimal NewValue = 0;
            //    Parameter = "PlanCount";
            //    if (senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value != DBNull.Value)
            //        NextLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["NextLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
            //        PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            //    if (senderGrid.SelectedRows[0].Cells["PlanCount"].Value != DBNull.Value)
            //        NewValue = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PlanCount"].Value);
            //    if (NextLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInNextLink(NextLinkAssignmentID, Parameter, NewValue);
            //    if (PrevLinkAssignmentID != 0)
            //        ProfileAssignmentsManager.EditParametersInPrevLink(PrevLinkAssignmentID, Parameter, NewValue);
            //}
        }

        private void kryptonContextMenuItem61_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = dgvSawnStripsAssignments;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            int PrevLinkAssignmentID = 0;
            decimal Width2 = 0;

            if (senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            if (PrevLinkAssignmentID != 0)
                Width2 = ProfileAssignmentsManager.EditFrezerWidth(PrevLinkAssignmentID, 1);
            ProfileAssignmentsManager.AddSawnStripsToPlan(DecorAssignmentID, 1, "3,2 мм", Width2);
        }

        private void kryptonContextMenuItem62_Click(object sender, EventArgs e)
        {
            if (!ProfileAssignmentsManager.BatchEnable)
                return;
            PercentageDataGrid senderGrid = dgvSawnStripsAssignments;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            int DecorAssignmentID = 0;
            int PrevLinkAssignmentID = 0;
            decimal Width2 = 0;

            if (senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            if (senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value != DBNull.Value)
                PrevLinkAssignmentID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["PrevLinkAssignmentID"].Value);
            if (DecorAssignmentID == 0)
                return;
            if (PrevLinkAssignmentID != 0)
                Width2 = ProfileAssignmentsManager.EditFrezerWidth(PrevLinkAssignmentID, 2);
            ProfileAssignmentsManager.AddSawnStripsToPlan(DecorAssignmentID, 2, "4,5 мм", Width2);
        }

        private void dgvFacingRollersRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows[e.RowIndex].Cells["DecorAssignmentID"].Value == DBNull.Value)
                return;

            if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["WriteOffFromStore"].Value))
            {
                int rowHeaderWidth = grid.RowHeadersVisible ?
                                     grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            grid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.Black;
            }
            else
            {
                int rowHeaderWidth = grid.RowHeadersVisible ?
                                     grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            grid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.White;
            }
        }

        private void dgvFacingMaterialRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows[e.RowIndex].Cells["DecorAssignmentID"].Value == DBNull.Value)
                return;

            if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["WriteOffFromStore"].Value))
            {
                int rowHeaderWidth = grid.RowHeadersVisible ?
                                     grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            grid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.Black;
            }
            else
            {
                int rowHeaderWidth = grid.RowHeadersVisible ?
                                     grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            grid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.White;
            }
        }

        private void kryptonContextMenuItem63_Click(object sender, EventArgs e)
        {
            ProfileAssignmentsManager.CopyPasteFacingRollers();
        }

        private void kryptonContextMenuItem64_Click(object sender, EventArgs e)
        {
            int DecorAssignmentID = 0;

            if (dgvFacingRollersAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingRollersAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value);
            foreach (DataGridViewRow item in dgvFacingRollersAssignments.SelectedRows)
            {
                if (!item.IsNewRow)
                    dgvFacingRollersAssignments.Rows.RemoveAt(item.Index);
            }
        }

        private void kryptonContextMenuItem65_Click(object sender, EventArgs e)
        {
            Save(null, null);
        }

        private void dgvFacingRollersAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu16.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvFacingMaterialAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu17.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem66_Click(object sender, EventArgs e)
        {
            ProfileAssignmentsManager.CopyPasteFacingMaterial();
        }

        private void kryptonContextMenuItem67_Click(object sender, EventArgs e)
        {
            int DecorAssignmentID = 0;

            if (dgvFacingMaterialAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvFacingMaterialAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            foreach (DataGridViewRow item in dgvFacingMaterialAssignments.SelectedRows)
            {
                if (!item.IsNewRow)
                    dgvFacingMaterialAssignments.Rows.RemoveAt(item.Index);
            }
        }

        private void kryptonContextMenuItem69_Click(object sender, EventArgs e)
        {
            Save(null, null);
        }

        private void kryptonContextMenuItem70_Click_1(object sender, EventArgs e)
        {
            if (dgvKashirAssignments.SelectedRows.Count == 0)
                return;

            int CoverID = 0;
            int DecorAssignmentID = 0;

            if (dgvKashirAssignments.SelectedRows[0].Cells["CoverID2"].Value != DBNull.Value)
                CoverID = Convert.ToInt32(dgvKashirAssignments.SelectedRows[0].Cells["CoverID2"].Value);
            if (dgvKashirAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value != DBNull.Value)
                DecorAssignmentID = Convert.ToInt32(dgvKashirAssignments.SelectedRows[0].Cells["DecorAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ReturnedRollersForm ReturnedRollersForm = new ReturnedRollersForm(this, ProfileAssignmentsManager, CoverID, DecorAssignmentID);

            TopForm = ReturnedRollersForm;

            ReturnedRollersForm.ShowDialog();

            ReturnedRollersForm.Close();
            ReturnedRollersForm.Dispose();

            TopForm = null;

            UpdateFacingRollers();
        }
    }
}
