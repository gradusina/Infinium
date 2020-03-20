using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.Marketing.WeeklyPlanning;

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingBatchForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        //const int iConfirmBatch = 64;
        const int iCreateBatch = 34;
        const int iCreateLabel = 40;
        const int iAddToBatch = 35;
        const int iInProduction = 22;
        const int iCloseBatch = 38;

        const int iAgreedProfil = 104;
        const int iAgreedTPS = 106;

        bool NeedRefresh = false;
        bool NeedSplash = false;
        bool bFrontType = false;
        bool bFrameColor = false;
        bool PickUpFronts = false;

        //bool ConfirmBatch = false;
        bool CreateBatch = false;
        bool CreateLabel = false;
        bool AddToBatch = false;
        bool InProduction = false;
        bool CloseBatch = false;

        bool bMegaBatchClose = false;

        int FactoryID = 1;
        int FormEvent = 0;

        ImageList ImageList1;
        RoleTypes RoleType = RoleTypes.Ordinary;

        public enum RoleTypes
        {
            Ordinary = 0,
            Admin = 1,
            Production = 2,
            Marketing = 3,
            Store = 4,
            Direction = 5,
            Control = 6
        }

        ArrayList BatchArray = null;
        ArrayList FrontIDs;
        ArrayList MainOrdersArray = null;

        Bitmap Lock_BW = new Bitmap(Properties.Resources.LockSmallBlack);
        Bitmap Unlock_BW = new Bitmap(Properties.Resources.UnlockSmallBlack);

        LightStartForm LightStartForm;

        Form TopForm = null;
        MarketingPickFrontsSelectForm PickFrontsSelectForm;
        DataTable RolePermissionsDataTable;

        public BatchManager BatchManager;
        public DecorCatalogOrder DecorCatalogOrder;
        public BatchReport ReportMarketing;

        public MarketingBatchForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID, this.Name);

            // Construct the ImageList.
            ImageList1 = new ImageList()
            {

                // Set the ImageSize property to a larger size 
                // (the default is 16 x 16).
                ImageSize = new Size(24, 24)
            };

            // Add two images to the list.
            ImageList1.Images.Add(Lock_BW);
            ImageList1.Images.Add(Unlock_BW);
            //ImageList1.Images.Add("c:\\windows\\FeatherTexture.png");
            Initialize();

            while (!SplashForm.bCreated) ;
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

        private void MarketingBatchForm_Shown(object sender, EventArgs e)
        {
            if (ZOVProfilCheckButton.Checked && !ZOVTPSCheckButton.Checked)
            {
                FactoryID = 1;
                BatchDataGrid.Columns["ProfilConfirm"].Visible = true;
                BatchDataGrid.Columns["ProfilEnabledColumn"].Visible = true;
                BatchDataGrid.Columns["ProfilName"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSConfirm"].Visible = false;
                BatchDataGrid.Columns["TPSEnabledColumn"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEnabledColumn"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEnabledColumn"].Visible = false;
            }
            if (!ZOVProfilCheckButton.Checked && ZOVTPSCheckButton.Checked)
            {
                FactoryID = 2;
                BatchDataGrid.Columns["ProfilConfirm"].Visible = false;
                BatchDataGrid.Columns["ProfilEnabledColumn"].Visible = false;
                BatchDataGrid.Columns["ProfilName"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSConfirm"].Visible = true;
                BatchDataGrid.Columns["TPSEnabledColumn"].Visible = true;
                BatchDataGrid.Columns["TPSName"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEnabledColumn"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEnabledColumn"].Visible = true;
            }

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

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (!MenuButton.Checked)
            {
                BatchFilterPanel.Visible = false;
                OrdersFilterPanel.Visible = false;
            }
            else
            {
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 0)
                {
                    BatchFilterPanel.Visible = true;
                    OrdersFilterPanel.Visible = false;
                }
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 1)
                {
                    BatchFilterPanel.Visible = false;
                    OrdersFilterPanel.Visible = true;
                }
            }
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
            FrontIDs = new ArrayList();
            MainOrdersArray = new ArrayList();

            DecorCatalogOrder = new DecorCatalogOrder();

            BatchManager = new BatchManager(
                ref MegaBatchDataGrid, ref BatchDataGrid,
                ref MainOrdersDataGrid, ref MainOrdersFrontsOrdersDataGrid, ref MegaOrdersDataGrid,
                ref MainOrdersDecorTabControl, ref MainOrdersTabControl,
                ref BatchMainOrdersDataGrid, ref BatchFrontsOrdersDataGrid, ref BatchMegaOrdersDataGrid,
                ref BatchDecorTabControl, ref BatchMainOrdersTabControl,
                ref DecorCatalogOrder,
                ref FrontsDataGrid, ref FrameColorsDataGrid, ref TechnoColorsDataGrid,
                ref InsetTypesDataGrid, ref InsetColorsDataGrid, ref TechnoInsetTypesDataGrid, ref TechnoInsetColorsDataGrid,
                ref SizesDataGrid,
                ref DecorProductsDataGrid, ref DecorItemsDataGrid, ref DecorColorsDataGrid, ref DecorSizesDataGrid,
                ref PreFrontsDataGrid, ref PreFrameColorsDataGrid, ref PreTechnoColorsDataGrid,
                ref PreInsetTypesDataGrid, ref PreInsetColorsDataGrid, ref PreTechnoInsetTypesDataGrid, ref PreTechnoInsetColorsDataGrid,
                ref PreSizesDataGrid,
                ref PreDecorProductsDataGrid, ref PreDecorItemsDataGrid, ref PreDecorColorsDataGrid, ref PreDecorSizesDataGrid);

            ReportMarketing = new BatchReport(ref DecorCatalogOrder);

        }

        private void Filter()
        {
            bool NotConfirm = NotConfirmCheckBox.Checked;
            bool Confirm = ConfirmCheckBox.Checked;
            bool OnProduction = OnProductionCheckBox.Checked;
            bool NotProduction = NotProductionCheckBox.Checked;
            bool InProduction = InProductionCheckBox.Checked;

            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                BatchManager.Filter(FactoryID,
                    NotConfirm, Confirm, OnProduction, NotProduction, InProduction, PickUpFronts);
                BatchManager.Filter_MegaBatches_ByFactory(FactoryID);
                BatchManager.GetMainOrdersNotInBatch(FactoryID);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                BatchManager.Filter(FactoryID,
                    NotConfirm, Confirm, OnProduction, NotProduction, InProduction, PickUpFronts);
                BatchManager.Filter_MegaBatches_ByFactory(FactoryID);
                BatchManager.GetMainOrdersNotInBatch(FactoryID);
            }
        }

        private void BatchDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PickUpFronts)
                return;

            if (BatchManager != null)
                if (BatchManager.BatchBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"] != DBNull.Value)
                    {
                        bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
                        bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
                        bool BatchInProduction = BatchInProductionCheckBox.Checked;
                        bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
                        bool BatchOnExp = BatchOnExpCheckBox.Checked;
                        bool BatchDispatched = BatchDispatchCheckBox.Checked;

                        ArrayList array = new ArrayList();

                        if (NeedSplash)
                        {
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            NeedSplash = false;
                            array = BatchManager.FilterBatchMegaOrders(Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]), FactoryID,
                                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, PickUpFronts);

                            if (GroupsCheckBox.Checked)
                            {
                                int[] Batches = BatchManager.GetSelectedBatch().OfType<int>().ToArray();

                                if (Batches.Count() > 0)
                                {
                                    ArrayList ClientGroups = new ArrayList();
                                    if (MarketFilterCheckBox.Checked)
                                        ClientGroups.Add(0);
                                    if (ZOVFilterCheckBox.Checked)
                                        ClientGroups.Add(3);
                                    if (kryptonCheckBox1.Checked)
                                        ClientGroups.Add(1);
                                    if (MoscowFilterCheckBox.Checked)
                                        ClientGroups.Add(2);
                                    if (ZOVProfilCheckBox.Checked)
                                        ClientGroups.Add(4);
                                    if (ZOVTPSCheckBox.Checked)
                                        ClientGroups.Add(5);

                                    BatchManager.GetProductInfo(FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, Batches, ClientGroups.OfType<int>().ToArray());
                                }
                            }

                            NeedSplash = true;

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                        }
                        else
                        {
                            array = BatchManager.FilterBatchMegaOrders(Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]), FactoryID,
                                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, PickUpFronts);

                            if (GroupsCheckBox.Checked)
                            {
                                int[] Batches = BatchManager.GetSelectedBatch().OfType<int>().ToArray();

                                if (Batches.Count() > 0)
                                {
                                    ArrayList ClientGroups = new ArrayList();
                                    if (MarketFilterCheckBox.Checked)
                                        ClientGroups.Add(0);
                                    if (ZOVFilterCheckBox.Checked)
                                        ClientGroups.Add(3);
                                    if (kryptonCheckBox1.Checked)
                                        ClientGroups.Add(1);
                                    if (MoscowFilterCheckBox.Checked)
                                        ClientGroups.Add(2);
                                    if (ZOVProfilCheckBox.Checked)
                                        ClientGroups.Add(4);
                                    if (ZOVTPSCheckBox.Checked)
                                        ClientGroups.Add(5);

                                    BatchManager.GetProductInfo(FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, Batches, ClientGroups.OfType<int>().ToArray());
                                }
                            }

                        }

                        BatchManager.CurrentBatchID = Convert.ToInt32(Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]));

                        int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
                        int[] MegaOrders = array.OfType<int>().ToArray();

                        BatchManager.GetMegaOrdersNotInProduction(BatchID, MegaOrders, FactoryID, PickUpFronts);
                    }
                }
                else
                    BatchManager.ClearMegaOrders();
        }

        #region MegaOrders

        private void MegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            ClientInfoLabel.Text = string.Empty;
            OrderLabel.Text = string.Empty;
            ConfirmDateLabel.Text = string.Empty;

            if (BatchManager != null)
                if (BatchManager.MegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        bool OnProduction = OnProductionCheckBox.Checked;
                        bool NotProduction = NotProductionCheckBox.Checked;
                        bool InProduction = InProductionCheckBox.Checked;

                        if (NeedSplash)
                        {
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            NeedSplash = false;

                            BatchManager.FilterMainOrdersByMegaOrder(
                                    Convert.ToInt32(((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                    FactoryID, OnProduction, NotProduction, InProduction);
                            NeedSplash = true;

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                        }
                        else
                        {
                            BatchManager.FilterMainOrdersByMegaOrder(
                                    Convert.ToInt32(((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                    FactoryID, OnProduction, NotProduction, InProduction);
                        }

                        BatchManager.GetMainOrdersNotInBatch(FactoryID);

                        int[] MegaOrders = BatchManager.GetSelectedMegaOrders().OfType<int>().ToArray();
                        Array.Sort(MegaOrders);

                        if (MegaOrders.Count() > 0)
                        {
                            xtraTabControl3.TabPages[1].PageVisible = true;
                            BatchManager.GetPreProductInfo(MegaOrders, FactoryID,
                                OnProduction, NotProduction, InProduction);
                        }
                        else
                        {
                            xtraTabControl3.TabPages[1].PageVisible = false;
                        }

                        string ClientName = string.Empty;
                        string OrderNumber = string.Empty;
                        bool IsComplaint = false;
                        string ConfirmDate = string.Empty;

                        if (((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["ClientName"] != DBNull.Value)
                            ClientName = ((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["ClientName"].ToString();
                        if (((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["OrderNumber"] != DBNull.Value)
                            OrderNumber = ((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["OrderNumber"].ToString();
                        if (((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["IsComplaint"] != DBNull.Value)
                            IsComplaint = Convert.ToBoolean(((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["IsComplaint"]);
                        if (((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["ConfirmDateTime"] != DBNull.Value)
                            ConfirmDate = Convert.ToDateTime(((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["ConfirmDateTime"]).ToString("dd.MM.yyyy");

                        string ClientInfo = ClientName;
                        string OrderInfo = "Заказ: №" + OrderNumber;
                        string ConfirmDateInfo = "Согласован: " + ConfirmDate;

                        if (IsComplaint)
                            OrderInfo += ", РЕКЛАМАЦИЯ";

                        ClientInfoLabel.Text = ClientInfo;
                        OrderLabel.Text = OrderInfo;
                        ConfirmDateLabel.Text = ConfirmDateInfo;

                        int[] MainOrders = BatchManager.GetMainOrders().OfType<int>().ToArray();
                        Array.Sort(MainOrders);

                        //decimal Square = BatchManager.GetSelectedSquare(MainOrders);
                        //if (Square > 0)
                        //    SquareLabel.Text = "Площадь выбранных подзаказов: " + Square + " м.кв";

                    }
                }
                else
                    BatchManager.MainOrdersDataTable.Clear();
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

        #endregion

        #region MainOrders

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        BatchManager.FilterProductsByMainOrder(Convert.ToInt32(((DataRowView)BatchManager.MainOrdersBindingSource.Current)["MainOrderID"]), FactoryID);
                        if (MainOrdersTabControl.TabPages[0].PageVisible && MainOrdersTabControl.TabPages[1].PageVisible)
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];

                    }
                }
                else
                {
                    BatchManager.FilterProductsByMainOrder(-1, FactoryID);
                    BatchManager.ClearPreProductsGrids();
                    //MainOrdersTabControl.SelectedTab = MainOrdersTabControl.Tabs[0];
                }
        }

        private void MainOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MainOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void MainOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDataGrid_Sorted(object sender, EventArgs e)
        {
            BatchManager.GetMainOrdersNotInBatch(FactoryID);
        }

        #endregion

        #region BatchMegaOrders

        private void BatchMegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.BatchMegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"] != DBNull.Value)
                    {
                        int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
                        bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
                        bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
                        bool BatchInProduction = BatchInProductionCheckBox.Checked;
                        bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
                        bool BatchOnExp = BatchOnExpCheckBox.Checked;
                        bool BatchDispatched = BatchDispatchCheckBox.Checked;

                        ArrayList MainOrdersArray = new ArrayList();

                        if (NeedSplash)
                        {
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            NeedSplash = false;
                            if (PickUpFronts)
                            {
                                if (bFrontType)
                                {
                                    if (FrontIDs.Count > 0)
                                    {
                                        MainOrdersArray = BatchManager.FilterBatchMainOrdersByFront(BatchID,
                                            Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current)["MegaOrderID"]), FactoryID,
                                            BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, FrontIDs);
                                    }
                                }
                                if (bFrameColor)
                                    MainOrdersArray = BatchManager.FilterBatchMainOrdersByFront(BatchID,
                                        Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current)["MegaOrderID"]), FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched);
                            }
                            else
                                MainOrdersArray = BatchManager.FilterBatchMainOrdersByMegaOrder(
                                    Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]),
                                    Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current)["MegaOrderID"]), FactoryID,
                                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched);

                            NeedSplash = true;

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                        }
                        else
                        {
                            if (PickUpFronts)
                            {
                                if (bFrontType)
                                    MainOrdersArray = BatchManager.FilterBatchMainOrdersByFront(BatchID,
                                        Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current)["MegaOrderID"]), FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, FrontIDs);
                                if (bFrameColor)
                                    MainOrdersArray = BatchManager.FilterBatchMainOrdersByFront(BatchID,
                                        Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current)["MegaOrderID"]), FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched);
                            }
                            else
                                MainOrdersArray = BatchManager.FilterBatchMainOrdersByMegaOrder(
                                    Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]),
                                    Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current)["MegaOrderID"]), FactoryID,
                                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched);
                        }

                        if (!GroupsCheckBox.Checked)
                        {
                            int[] MegaOrders = BatchManager.GetSelectedBatchMegaOrders().OfType<int>().ToArray();

                            if (MegaOrders.Count() > 0)
                            {
                                int[] MainOrders = BatchManager.GetBatchMainOrders(BatchID, MegaOrders, FactoryID, PickUpFronts).OfType<int>().ToArray();

                                if (MainOrders.Count() > 0)
                                {
                                    BatchManager.GetProductInfo(MainOrders, FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched);
                                }
                            }
                        }
                    }
                }
                else
                {
                    BatchManager.ClearGrids();
                    BatchManager.ClearProductsGrids();
                }
        }

        private void BatchMegaOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                BatchMegaOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void BatchMegaOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        #endregion

        #region BatchMainOrders

        private void BatchMainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.BatchMainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.BatchMainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        BatchManager.FilterBatchProductsByMainOrder(Convert.ToInt32(((DataRowView)BatchManager.BatchMainOrdersBindingSource.Current)["MainOrderID"]), FactoryID);
                        if (BatchMainOrdersTabControl.TabPages[0].PageVisible && BatchMainOrdersTabControl.TabPages[1].PageVisible)
                            BatchMainOrdersTabControl.SelectedTabPage = BatchMainOrdersTabControl.TabPages[0];
                    }
                }
                else
                {
                    BatchManager.FilterBatchProductsByMainOrder(-1, FactoryID);
                }
        }

        private void BatchMainOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                BatchMainOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void BatchMainOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        #endregion

        #region Batch

        private void AddBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы собираетесь создать новую партию. Продолжить?",
                    "Новая партия");
            if (!OKCancel)
                return;

            if (BatchManager.IsBatchEmpty())
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Пустая партия уже создана, добавляйте заказы в неё",
                    "Создание партии");
                return;
            }
            if ((DataRowView)BatchManager.MegaBatchBindingSource.Current != null)
            {
                int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current).Row["MegaBatchID"]);
                BatchManager.AddBatch(MegaBatchID, FactoryID);
            }
        }

        private void RemoveBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchBindingSource.Count < 1)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Партия будет расформирована и удалена. Продолжить?",
                    "Удаление партии");

            if (OKCancel)
            {
                int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current).Row["BatchID"]);

                if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Партия сформирована, удаление запрещено.",
                           "Ошибка удаления");
                    return;
                }

                BatchManager.RemoveBatch(BatchID, FactoryID);
            }
        }

        #endregion

        #region Batch content

        private void AddToBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchBindingSource.Count < 1)
                return;

            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current).Row["BatchID"]);
            int MegaOrderID = Convert.ToInt32(((DataRowView)BatchManager.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            int[] MainOrders = BatchManager.GetMainOrders().OfType<int>().ToArray();
            if (MainOrders.Count() < 1)
                return;
            Array.Sort(MainOrders);

            if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Партия сформирована, добавление заказов в неё запрещено. Создайте новую партию",
                       "Ошибка добавления");
                return;
            }

            ////Михаил Анатольевич Кондрашов
            //if (Security.CurrentUserID == 322 && NeedCheck)
            //{
            //    NeedCheck = false;
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //           "Михаил Анатольевич, пожалуйста, проверьте партию, в которую добавляете заказ. После этого заново нажмите 'В партию'",
            //           "Напоминание");
            //    return;
            //}

            BatchManager.AddToBatch(BatchID, MainOrders, FactoryID);
            BatchManager.SetMainOrderOnProduction(MainOrders, FactoryID, true);
            BatchManager.SetMegaOrderOnProduction(MegaOrderID, FactoryID);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                bool NotConfirm = NotConfirmCheckBox.Checked;
                bool Confirm = ConfirmCheckBox.Checked;
                bool OnProduction = OnProductionCheckBox.Checked;
                bool NotProduction = NotProductionCheckBox.Checked;
                bool InProduction = InProductionCheckBox.Checked;

                bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
                bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
                bool BatchInProduction = BatchInProductionCheckBox.Checked;
                bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
                bool BatchOnExp = BatchOnExpCheckBox.Checked;
                bool BatchDispatched = BatchDispatchCheckBox.Checked;

                int[] BatchMegaOrders = BatchManager.GetBatchMegaOrders().OfType<int>().ToArray();
                BatchManager.Filter(FactoryID,
                    NotConfirm, Confirm, OnProduction, NotProduction, InProduction, PickUpFronts);

                BatchManager.FilterBatchMegaOrders(BatchID, FactoryID,
                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched,
                    PickUpFronts);

                BatchManager.GetMainOrdersNotInBatch(FactoryID);
                BatchManager.GetMegaOrdersNotInProduction(BatchID, BatchMegaOrders, FactoryID, PickUpFronts);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void RemoveMegaOrderButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchMegaOrdersBindingSource.Count < 1)
                return;

            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current).Row["BatchID"]);
            if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Партия сформирована, удаление заказов из неё запрещено",
                       "Ошибка удаления");
                return;
            }

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Выбранный заказ будет убран из партии. Продолжить?",
                    "Редактирование партии");

            if (OKCancel)
            {
                int MegaOrderID = Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current).Row["MegaOrderID"]);
                int[] MegaOrders = new int[1] { MegaOrderID };
                int[] MainOrders = BatchManager.GetBatchMainOrders(BatchID, MegaOrders, FactoryID, false).OfType<int>().ToArray();
                Array.Sort(MainOrders);

                if (BatchManager.RemoveMegaOrderFromBatch(BatchID, FactoryID))
                {
                    BatchManager.SetMainOrderOnProduction(MainOrders, FactoryID, false);
                    BatchManager.SetMegaOrderOnProduction(MegaOrderID, FactoryID);
                    Filter();
                    BatchManager.GetMainOrdersNotInBatch(FactoryID);
                }
            }
        }

        #endregion

        #region Set in production

        private void PrintMegaOrderButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchMegaOrdersBindingSource.Count < 1)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Выбранные заказы будут распечатаны и отправлены в производство. Продолжить?",
                    "В производство");
            if (!OKCancel)
                return;

            int[] MegaOrders = BatchManager.GetSelectedBatchMegaOrders().OfType<int>().ToArray();

            if (MegaOrders.Count() < 1)
                return;

            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
            int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]);

            if (!BatchManager.HasOrders(MegaOrders, FactoryID))
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int[] MainOrders = BatchManager.GetBatchMainOrders(BatchID, MegaOrders, FactoryID, false).OfType<int>().ToArray();
            //Array.Sort(MainOrders);

            ReportMarketing.CreateReport(MegaBatchID, BatchID, MainOrders, FactoryID);
            BatchManager.SetMainOrderInProduction(MainOrders, FactoryID);

            for (int i = 0; i < MegaOrders.Count(); i++)
            {
                CheckOrdersStatus.SetMegaOrderStatus(MegaOrders[i]);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchOnExp = BatchOnExpCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;

            BatchManager.FilterBatchMegaOrders(BatchID, FactoryID,
                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched,
                PickUpFronts);


            MegaOrders = BatchManager.GetBatchMegaOrders().OfType<int>().ToArray();
            BatchManager.GetMegaOrdersNotInProduction(BatchID, MegaOrders, FactoryID, PickUpFronts);
        }

        private void PrintMainOrderButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchMainOrdersBindingSource.Count < 1)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Выбранные подзаказы будут распечатаны и отправлены в производство. Продолжить?",
                    "В производство");
            if (!OKCancel)
                return;

            int[] MegaOrders = BatchManager.GetSelectedBatchMegaOrders().OfType<int>().ToArray();

            if (MegaOrders.Count() < 1)
                return;

            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
            int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]);

            int MegaOrderID = Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            if (!BatchManager.HasOrders(MegaOrders, FactoryID))
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int[] MainOrders = BatchManager.GetSelectedBatchMainOrders().OfType<int>().ToArray();
            Array.Sort(MainOrders);

            if (MainOrders.Count() < 1)
                return;

            ReportMarketing.CreateReport(MegaBatchID, BatchID, MainOrders, FactoryID);
            BatchManager.SetMainOrderInProduction(MainOrders, FactoryID);
            CheckOrdersStatus.SetMegaOrderStatus(MegaOrderID);

            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchOnExp = BatchOnExpCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;

            BatchManager.FilterBatchMegaOrders(BatchID, FactoryID,
                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched,
                PickUpFronts);

            int[] BatchMegaOrders = BatchManager.GetBatchMegaOrders().OfType<int>().ToArray();

            BatchManager.GetMegaOrdersNotInProduction(BatchID, BatchMegaOrders, FactoryID, PickUpFronts);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        #endregion

        #region Filter functions

        private void NotAgreedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void NotConfirmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void ConfirmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void NotProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void OnProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void InProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void BatchNotProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void BatchOnProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void BatchInProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void BatchOnStorageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void BatchOnExpCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void BatchDispatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        #endregion

        private void RolesAndPermissionsTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            BatchManager.GetMainOrdersNotInBatch(FactoryID);
            NeedSplash = true;

            if (MenuButton.Checked)
            {
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 1)
                {
                    OrdersFilterPanel.Visible = true;
                    BatchFilterPanel.Visible = false;
                }
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 0)
                {
                    OrdersFilterPanel.Visible = false;
                    BatchFilterPanel.Visible = true;
                }
            }

        }

        private void DecorProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.DecorProductsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.DecorProductsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                    {
                        BatchManager.FilterDecorProducts(
                            Convert.ToInt32(((DataRowView)BatchManager.DecorProductsSummaryBindingSource.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.DecorProductsSummaryBindingSource.Current).Row["MeasureID"]));
                    }
                }

            DecorPogonLabel.Text = "0";
            DecorCountLabel.Text = "0";

            if (BatchManager != null)
                if (BatchManager.DecorProductsSummaryBindingSource.Count > 0)
                {
                    decimal Pogon = 0;
                    int Count = 0;

                    BatchManager.GetDecorInfo(ref Pogon, ref Count);

                    DecorPogonLabel.Text = Pogon.ToString();
                    DecorCountLabel.Text = Count.ToString();
                }
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            if (ZOVProfilCheckButton.Checked && !ZOVTPSCheckButton.Checked)
            {
                if (PermissionGranted(iAgreedProfil))
                {
                    btnAgreedProfil.Visible = true;
                }
                if (PermissionGranted(iAgreedTPS))
                {
                    btnAgreedTPS.Visible = false;
                }

                FactoryID = 1;
                BatchDataGrid.Columns["ProfilConfirm"].Visible = true;
                BatchDataGrid.Columns["ProfilEnabledColumn"].Visible = true;
                BatchDataGrid.Columns["ProfilName"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSConfirm"].Visible = false;
                BatchDataGrid.Columns["TPSEnabledColumn"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEnabledColumn"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEnabledColumn"].Visible = false;
            }
            if (!ZOVProfilCheckButton.Checked && ZOVTPSCheckButton.Checked)
            {
                if (PermissionGranted(iAgreedProfil))
                {
                    btnAgreedProfil.Visible = false;
                }
                if (PermissionGranted(iAgreedTPS))
                {
                    btnAgreedTPS.Visible = true;
                }

                FactoryID = 2;
                BatchDataGrid.Columns["ProfilConfirm"].Visible = false;
                BatchDataGrid.Columns["ProfilEnabledColumn"].Visible = false;
                BatchDataGrid.Columns["ProfilName"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSConfirm"].Visible = true;
                BatchDataGrid.Columns["TPSEnabledColumn"].Visible = true;
                BatchDataGrid.Columns["TPSName"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEnabledColumn"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEnabledColumn"].Visible = true;
            }

            Filter();
        }

        private void MegaOrdersDataGrid_Sorted(object sender, EventArgs e)
        {
            BatchManager.GetMainOrdersNotInBatch(FactoryID);
        }

        private void BatchMegaOrdersDataGrid_Sorted(object sender, EventArgs e)
        {
            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
            int[] MegaOrders = BatchManager.GetBatchMegaOrders().OfType<int>().ToArray();

            BatchManager.GetMegaOrdersNotInProduction(BatchID, MegaOrders, FactoryID, PickUpFronts);

            //BatchManager.GetMainOrdersNotInProduction(FactoryID, PickUpFronts);
        }

        private void BatchMainOrdersDataGrid_Sorted(object sender, EventArgs e)
        {
            BatchManager.GetMainOrdersNotInProduction(FactoryID, PickUpFronts);
        }

        private void RolesAndPermissionsTabControl_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            NeedSplash = false;
        }

        private void OnStorageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void DispatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void PickUpFrontsButton_Click(object sender, EventArgs e)
        {
            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchOnExp = BatchOnExpCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;

            if (PickUpFrontsButton.Tag.ToString() == "true")
            {
                this.PickUpFrontsButton.OverrideDefault.Back.Color1 = System.Drawing.Color.Crimson;
                this.PickUpFrontsButton.StateCommon.Back.Color1 = System.Drawing.Color.Crimson;
                this.PickUpFrontsButton.StateTracking.Back.Color1 = System.Drawing.Color.Crimson;

                MegaBatchDataGrid.Enabled = false;
                BatchDataGrid.Enabled = false;
                PickUpFrontsButton.Tag = "false";
                PickUpFrontsButton.Text = "Обновить";
                PickUpFronts = true;

                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                PickFrontsSelectForm = new MarketingPickFrontsSelectForm(FactoryID, ref FrontIDs);

                TopForm = PickFrontsSelectForm;
                PickFrontsSelectForm.ShowDialog();
                TopForm = null;

                PhantomForm.Close();

                PhantomForm.Dispose();

                if (PickFrontsSelectForm.IsOKPress)
                {
                    bFrontType = PickFrontsSelectForm.bFrontType;
                    bFrameColor = PickFrontsSelectForm.bFrameColor;

                    PickFrontsSelectForm.Dispose();
                    PickFrontsSelectForm = null;
                    GC.Collect();

                    NeedSplash = false;
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    BatchManager.GetCurrentFrontID();
                    BatchManager.GetCurrentFrameColorID();

                    ArrayList array = new ArrayList();

                    int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
                    if (bFrameColor)
                        array = BatchManager.PickUpFrontsTPS(FactoryID, BatchID,
                            BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched);

                    if (bFrontType)
                    {
                        if (FrontIDs.Count > 0)
                        {
                            array = BatchManager.PickUpFrontsProfil(FactoryID, BatchID,
                                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, FrontIDs);
                        }
                    }

                    int[] MegaOrders = array.OfType<int>().ToArray();

                    BatchManager.GetMegaOrdersNotInProduction(BatchID, MegaOrders, FactoryID, PickUpFronts);

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;

                    NeedSplash = true;
                }
                else
                {
                    PickFrontsSelectForm.Dispose();
                    PickFrontsSelectForm = null;
                    GC.Collect();
                }
                //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                //T.Start();

                //while (!SplashWindow.bSmallCreated) ;

                //NeedSplash = false;

                //BatchManager.GetCurrentFrontID();
                //BatchManager.GetCurrentFrameColorID();

                //ArrayList array = BatchManager.PickUpFrontsTPS(FactoryID,
                //    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched);

                //int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
                //int[] MegaOrders = array.OfType<int>().ToArray();

                //BatchManager.GetMegaOrdersNotInProduction(BatchID, MegaOrders, FactoryID, PickUpFronts);

                //NeedSplash = true;

                //while (SplashWindow.bSmallCreated)
                //    SmallWaitForm.CloseS = true;

                return;

            }
            else
            {
                this.PickUpFrontsButton.OverrideDefault.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateCommon.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateTracking.Back.Color1 = System.Drawing.Color.MediumPurple;

                MegaBatchDataGrid.Enabled = true;
                BatchDataGrid.Enabled = true;
                PickUpFrontsButton.Tag = "true";
                PickUpFrontsButton.Text = "Подобрать";
                PickUpFronts = false;
                Filter();
                return;
            }
        }

        private void MegaBatchDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            bMegaBatchClose = true;
            if (PickUpFronts)
                return;

            if (BatchManager != null)
                if (BatchManager.MegaBatchBindingSource.Count > 0)
                {
                    if (FactoryID == 1)
                        if (((DataRowView)BatchManager.MegaBatchBindingSource.Current)["ProfilBatchClose"] != DBNull.Value)
                            bMegaBatchClose = Convert.ToBoolean(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["ProfilBatchClose"]);
                    if (FactoryID == 2)
                        if (((DataRowView)BatchManager.MegaBatchBindingSource.Current)["TPSBatchClose"] != DBNull.Value)
                            bMegaBatchClose = Convert.ToBoolean(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["TPSBatchClose"]);
                    if (((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"] != DBNull.Value)
                    {
                        BatchManager.FilterBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID);
                        BatchManager.CurrentMegaBatchID = Convert.ToInt32(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]));
                    }
                }
                else
                    BatchManager.ClearBatch();
            if (bMegaBatchClose)
            {
                AddBatchButton.Enabled = false;
                EditBatchNameButton.Enabled = false;
                OKMoveOrdersButton.Enabled = false;
                MoveBatchButton.Enabled = false;
                CancelMoveOrdersButton.Enabled = false;
                PrintBatchButton.Enabled = false;
                RemoveBatchButton.Enabled = false;
                MoveMegaOrdersButton.Enabled = false;
                RemoveMegaOrderButton.Enabled = false;
                MoveMainOrdersButton.Enabled = false;
                AddToBatchButton.Enabled = false;
                PickUpFrontsButton.Enabled = false;

                cmiCloseProfilBatch.Enabled = false;
                cmiCloseTPSBatch.Enabled = false;
                cmiConfirmProfilBatch.Enabled = false;
                cmiConfirmTPSBatch.Enabled = false;
            }
            else
            {
                AddBatchButton.Enabled = true;
                EditBatchNameButton.Enabled = true;
                OKMoveOrdersButton.Enabled = true;
                MoveBatchButton.Enabled = true;
                CancelMoveOrdersButton.Enabled = true;
                PrintBatchButton.Enabled = true;
                RemoveBatchButton.Enabled = true;
                MoveMegaOrdersButton.Enabled = true;
                PickUpFrontsButton.Enabled = true;
                RemoveMegaOrderButton.Enabled = true;
                MoveMainOrdersButton.Enabled = true;
                AddToBatchButton.Enabled = true;

                cmiCloseProfilBatch.Enabled = true;
                cmiCloseTPSBatch.Enabled = true;
                cmiConfirmProfilBatch.Enabled = true;
                cmiConfirmTPSBatch.Enabled = true;
            }
        }

        private void AddMegaBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы собираетесь создать новую группу партий. Продолжить?",
                    "Группа партий");
            if (!OKCancel)
                return;

            BatchManager.AddMegaBatch(FactoryID);
        }

        private void RefreshOrdersButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;
            PickUpFronts = false;
            Filter();
        }

        private void PrintBatchButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Партия будет распечатана и отправлена в производство. Продолжить?",
                    "В производство");

            if (!OKCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]);
            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
            ReportMarketing.CreateReport(MegaBatchID, BatchID, FactoryID);
            BatchManager.SetMainOrderInProduction(BatchID, FactoryID);

            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchOnExp = BatchOnExpCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;

            BatchManager.FilterBatchMegaOrders(BatchID, FactoryID,
                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched,
                PickUpFronts);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        //переносит заказы между партиями
        private void MoveOrdersButton_Click(object sender, EventArgs e)
        {
            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
            //if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //           "Партия сформирована, перенос заказов запрещен",
            //           "Ошибка добавления");
            //    return;
            //}

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchOnExp = BatchOnExpCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;
            BatchManager.GetCurrentFrontID();
            BatchManager.GetCurrentFrameColorID();
            int[] MegaOrders = new int[BatchMegaOrdersDataGrid.SelectedRows.Count];
            for (int i = 0; i < BatchMegaOrdersDataGrid.SelectedRows.Count; i++)
                MegaOrders[i] = Convert.ToInt32(BatchMegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value);

            MainOrdersArray.Clear();

            if (FactoryID == 1)
            {
                BatchManager.GetCurrentFrontID();
                BatchManager.GetCurrentFrameColorID();

                if (PickUpFronts)
                {
                    if (bFrameColor)
                        MainOrdersArray = BatchManager.GetBatchMainOrders(MegaOrders, BatchID, FactoryID,
                            BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, PickUpFronts);

                    if (bFrontType)
                        MainOrdersArray = BatchManager.GetBatchMainOrders(MegaOrders, BatchID, FactoryID,
                            BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, PickUpFronts, FrontIDs);
                }
                else
                {
                    MainOrdersArray = BatchManager.GetBatchMainOrders(MegaOrders, BatchID, FactoryID,
                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, PickUpFronts);
                }
            }
            if (FactoryID == 2)
            {
                MainOrdersArray = BatchManager.GetBatchMainOrders(MegaOrders, BatchID, FactoryID,
                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchOnExp, BatchDispatched, PickUpFronts);
            }

            MainOrdersArray.Sort();
            if (MainOrdersArray.Count < 1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                          "Не выбран ни один подзаказ",
                          "Ошибка переноса");

                PhantomForm.Close();

                PhantomForm.Dispose();

                return;
            }

            MarketingBatchSelectMenu SelectMenuForm = new MarketingBatchSelectMenu(this, BatchManager, FactoryID);

            TopForm = SelectMenuForm;
            SelectMenuForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            SelectMenuForm.Dispose();
            TopForm = null;

            if (BatchManager.NewBatch)
            {
                //int[] MegaOrders = MegaOrdersArray.OfType<int>().ToArray();

                //ArrayList array = new ArrayList();

                BatchManager.MoveOrdersToNewPart(MainOrdersArray.OfType<int>().ToArray(), FactoryID);

                this.PickUpFrontsButton.OverrideDefault.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateCommon.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateTracking.Back.Color1 = System.Drawing.Color.MediumPurple;

                MegaBatchDataGrid.Enabled = true;
                BatchDataGrid.Enabled = true;
                PickUpFrontsButton.Tag = "true";
                PickUpFrontsButton.Text = "Подобрать";
                PickUpFronts = false;

                BatchManager.FilterBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID);
                Filter();
            }

            if (BatchManager.OldBatch)
            {
                MegaBatchDataGrid.Enabled = true;
                BatchDataGrid.Enabled = true;

                AddBatchButton.Visible = false;
                RemoveBatchButton.Visible = false;
                PrintBatchButton.Visible = false;
                MoveBatchButton.Visible = false;
                EditBatchNameButton.Visible = false;
                OKMoveOrdersButton.Visible = true;
                CancelMoveOrdersButton.Visible = true;
            }
        }

        //подтверждает перенос заказов
        private void OKMoveOrdersButton_Click(object sender, EventArgs e)
        {
            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchOnExp = BatchOnExpCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;

            int[] MainOrders = MainOrdersArray.OfType<int>().ToArray();

            //int[] MainOrders = BatchManager.GetBatchMainOrders(MegaOrders, FactoryID,
            //    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage,BatchOnExp, BatchDispatched, PickUpFronts).OfType<int>().ToArray();
            //if (FactoryID == 1)
            //    MainOrders = BatchManager.GetBatchMainOrders(MegaOrders, FactoryID,
            //        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage,BatchOnExp, BatchDispatched, PickUpFronts, FrontIDs).OfType<int>().ToArray();
            //Array.Sort(MainOrders);

            if (BatchManager.OldBatch)
            {
                int SelectedBatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
                BatchManager.MoveOrdersToSelectedBatch(SelectedBatchID, MainOrders, FactoryID);
                BatchManager.FilterBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID);
            }

            AddBatchButton.Visible = true;
            RemoveBatchButton.Visible = true;
            PrintBatchButton.Visible = true;
            MoveBatchButton.Visible = true;
            EditBatchNameButton.Visible = true;
            OKMoveOrdersButton.Visible = false;
            CancelMoveOrdersButton.Visible = false;
        }

        private void CancelMoveOrdersButton_Click(object sender, EventArgs e)
        {
            AddBatchButton.Visible = true;
            RemoveBatchButton.Visible = true;
            PrintBatchButton.Visible = true;
            MoveBatchButton.Visible = true;
            EditBatchNameButton.Visible = true;
            OKMoveOrdersButton.Visible = false;
            CancelMoveOrdersButton.Visible = false;
        }

        //переносит партии между группами
        private void MoveBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchBindingSource.Count < 1)
                return;

            Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Выберите Группу партий",
                    "Перенос партий");

            BatchArray = BatchManager.GetSelectedBatch();
            OKMoveBatchButton.Visible = true;
            CancelMoveBatchButton.Visible = true;
            AddMegaBatchButton.Visible = false;
        }

        //подтверждает перенос партий
        private void OKMoveBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.MegaBatchBindingSource.Count < 1)
                return;

            int[] Batch = BatchArray.OfType<int>().ToArray();
            int DestMegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]);

            BatchManager.MoveBatch(Batch, DestMegaBatchID, FactoryID);

            OKMoveBatchButton.Visible = false;
            CancelMoveBatchButton.Visible = false;
            AddMegaBatchButton.Visible = true;
        }

        private void CancelMoveBatchButton_Click(object sender, EventArgs e)
        {
            OKMoveBatchButton.Visible = false;
            CancelMoveBatchButton.Visible = false;
            AddMegaBatchButton.Visible = true;
        }

        private void FrontsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.FrontsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.FrontsSummaryBindingSource.Current)["FrontID"] != DBNull.Value)
                    {
                        BatchManager.FilterFrameColors(
                            Convert.ToInt32(((DataRowView)BatchManager.FrontsSummaryBindingSource.Current).Row["FrontID"]));
                    }
                }

            FrontsSquareLabel.Text = "0";
            FrontsCountLabel.Text = "0";

            if (BatchManager != null)
                if (BatchManager.FrontsSummaryBindingSource.Count > 0)
                {
                    decimal FrontSquare = 0;
                    int Count = 0;

                    BatchManager.GetFrontsInfo(ref FrontSquare, ref Count);

                    FrontsSquareLabel.Text = FrontSquare.ToString();
                    FrontsCountLabel.Text = Count.ToString();
                }
        }

        private void FrameColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.FrameColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.FrameColorsSummaryBindingSource.Current)["ColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterTechnoColors(
                            Convert.ToInt32(((DataRowView)BatchManager.FrameColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.FrameColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.FrameColorsSummaryBindingSource.Current).Row["PatinaID"]));
                    }
                }
        }

        private void TechnoColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.TechnoColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.TechnoColorsSummaryBindingSource.Current)["TechnoColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterInsetTypes(
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoColorsSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoColorsSummaryBindingSource.Current).Row["TechnoColorID"]));
                    }
                }
        }

        private void InsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.InsetTypesSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.InsetTypesSummaryBindingSource.Current)["InsetTypeID"] != DBNull.Value)
                    {
                        BatchManager.FilterInsetColors(
                            Convert.ToInt32(((DataRowView)BatchManager.InsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]));
                    }
                }
        }

        private void InsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.InsetColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.InsetColorsSummaryBindingSource.Current)["InsetColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterTechnoInsetTypes(
                            Convert.ToInt32(((DataRowView)BatchManager.InsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.InsetColorsSummaryBindingSource.Current).Row["InsetColorID"]));
                    }
                }
        }

        private void TechnoInsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.TechnoInsetTypesSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current)["TechnoInsetTypeID"] != DBNull.Value)
                    {
                        BatchManager.FilterTechnoInsetColors(
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current).Row["InsetColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetTypesSummaryBindingSource.Current).Row["TechnoInsetTypeID"]));
                    }
                }
        }

        private void TechnoInsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.TechnoInsetColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current)["TechnoInsetColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterSizes(
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["InsetColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.TechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetColorID"]));
                    }
                }
        }

        private void DecorItemsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.DecorItemsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.DecorItemsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                    {
                        BatchManager.FilterDecorItems(
                            Convert.ToInt32(((DataRowView)BatchManager.DecorItemsSummaryBindingSource.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.DecorItemsSummaryBindingSource.Current).Row["DecorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.DecorItemsSummaryBindingSource.Current).Row["MeasureID"]));
                    }
                }
        }

        private void DecorColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.DecorColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.DecorColorsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                    {
                        BatchManager.FilterDecorSizes(
                            Convert.ToInt32(((DataRowView)BatchManager.DecorColorsSummaryBindingSource.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.DecorColorsSummaryBindingSource.Current).Row["DecorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.DecorColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.DecorColorsSummaryBindingSource.Current).Row["MeasureID"]));
                    }
                }
        }

        private void PreFrontsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreFrontsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreFrontsSummaryBindingSource.Current)["FrontID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreFrameColors(
                            Convert.ToInt32(((DataRowView)BatchManager.PreFrontsSummaryBindingSource.Current).Row["FrontID"]));
                    }
                }

            PreFrontsSquareLabel.Text = "0";
            PreFrontsCountLabel.Text = "0";

            if (BatchManager != null)
                if (BatchManager.PreFrontsSummaryBindingSource.Count > 0)
                {
                    decimal FrontSquare = 0;
                    int Count = 0;

                    BatchManager.GetPreFrontsInfo(ref FrontSquare, ref Count);

                    PreFrontsSquareLabel.Text = FrontSquare.ToString();
                    PreFrontsCountLabel.Text = Count.ToString();
                }
        }

        private void PreFrameColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreFrameColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreFrameColorsSummaryBindingSource.Current)["ColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreTechnoColors(
                            Convert.ToInt32(((DataRowView)BatchManager.PreFrameColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreFrameColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreFrameColorsSummaryBindingSource.Current).Row["PatinaID"]));
                    }
                }
        }

        private void PreTechnoColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreTechnoColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreTechnoColorsSummaryBindingSource.Current)["TechnoColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreInsetTypes(
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoColorsSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoColorsSummaryBindingSource.Current).Row["TechnoColorID"]));
                    }
                }
        }

        private void PreInsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreInsetTypesSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreInsetTypesSummaryBindingSource.Current)["InsetTypeID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreInsetColors(
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]));
                    }
                }
        }

        private void PreInsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreInsetColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreInsetColorsSummaryBindingSource.Current)["InsetColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreTechnoInsetTypes(
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreInsetColorsSummaryBindingSource.Current).Row["InsetColorID"]));
                    }
                }
        }

        private void PreTechnoInsetTypesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreTechnoInsetTypesSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current)["TechnoInsetTypeID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreTechnoInsetColors(
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current).Row["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current).Row["InsetColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetTypesSummaryBindingSource.Current).Row["TechnoInsetTypeID"]));
                    }
                }
        }

        private void PreTechnoInsetColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreTechnoInsetColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current)["TechnoInsetColorID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreSizes(
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["FrontID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["PatinaID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["TechnoColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["InsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["InsetColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetTypeID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreTechnoInsetColorsSummaryBindingSource.Current).Row["TechnoInsetColorID"]));
                    }
                }
        }

        private void PreDecorProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreDecorProductsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreDecorProductsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreDecorProducts(
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorProductsSummaryBindingSource.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorProductsSummaryBindingSource.Current).Row["MeasureID"]));
                    }
                }

            PreDecorPogonLabel.Text = "0";
            PreDecorCountLabel.Text = "0";

            if (BatchManager != null)
                if (BatchManager.PreDecorProductsSummaryBindingSource.Count > 0)
                {
                    decimal Pogon = 0;
                    int Count = 0;

                    BatchManager.GetPreDecorInfo(ref Pogon, ref Count);

                    PreDecorPogonLabel.Text = Pogon.ToString();
                    PreDecorCountLabel.Text = Count.ToString();
                }
        }

        private void PreDecorItemsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreDecorItemsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreDecorItemsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreDecorItems(
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorItemsSummaryBindingSource.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorItemsSummaryBindingSource.Current).Row["DecorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorItemsSummaryBindingSource.Current).Row["MeasureID"]));
                    }
                }
        }

        private void PreDecorColorsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.PreDecorColorsSummaryBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.PreDecorColorsSummaryBindingSource.Current)["ProductID"] != DBNull.Value)
                    {
                        BatchManager.FilterPreDecorSizes(
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorColorsSummaryBindingSource.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorColorsSummaryBindingSource.Current).Row["DecorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorColorsSummaryBindingSource.Current).Row["ColorID"]),
                            Convert.ToInt32(((DataRowView)BatchManager.PreDecorColorsSummaryBindingSource.Current).Row["MeasureID"]));
                    }
                }
        }

        private void EditBatchNameButton_Click(object sender, EventArgs e)
        {
            if (BatchDataGrid.SelectedRows.Count == 1)
            {
                BatchManager.SaveBatch(FactoryID);
            }
        }

        private void BatchMainOrdersDataGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (BatchMainOrdersDataGrid.Rows.Count > 1)
                return;

            string ProductionStatus = "ProfilProductionStatusID";

            if (FactoryID == 2)
                ProductionStatus = "TPSProductionStatusID";

            if (BatchMainOrdersDataGrid.SelectedCells.Count > 0
                && BatchMainOrdersDataGrid.SelectedCells[0].RowIndex == e.RowIndex
                && Convert.ToInt32(BatchMainOrdersDataGrid.Rows[e.RowIndex].Cells[ProductionStatus].Value) != 3)
            {
                BatchMainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.DimGray;
                BatchMainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void SaveMegaBatch_Click(object sender, EventArgs e)
        {
            if (MegaBatchDataGrid.SelectedRows.Count == 1)
            {
                BatchManager.SaveMegaBatch(FactoryID);
            }
        }

        private void MegaBatchDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((RoleType == RoleTypes.Admin || RoleType == RoleTypes.Control || RoleType == RoleTypes.Direction)
                && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }

            if ((PermissionGranted(iAgreedProfil) || PermissionGranted(iAgreedTPS))
                && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void GroupsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MarketFilterCheckBox.Enabled = GroupsCheckBox.Checked;
            ZOVFilterCheckBox.Enabled = GroupsCheckBox.Checked;
            MoscowFilterCheckBox.Enabled = GroupsCheckBox.Checked;
            kryptonCheckBox1.Enabled = GroupsCheckBox.Checked;
            ZOVProfilCheckBox.Enabled = GroupsCheckBox.Checked;
            ZOVTPSCheckBox.Enabled = GroupsCheckBox.Checked;

            MegaBatchDataGrid_SelectionChanged(null, null);
        }

        private void MarketFilterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MegaBatchDataGrid_SelectionChanged(null, null);
        }

        private void ZOVFilterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MegaBatchDataGrid_SelectionChanged(null, null);
        }

        private void MoscowFilterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MegaBatchDataGrid_SelectionChanged(null, null);
        }

        private void FrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiExportToExcel_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            BatchExcelReport MarketingBatchReport = new BatchExcelReport();

            MarketingBatchReport.CreateReport(((DataTable)((BindingSource)FrontsDataGrid.DataSource).DataSource).DefaultView.Table,
                ((DataTable)((BindingSource)DecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Недельное планирование. Маркетинг");

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void DecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void BatchDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((RoleType == RoleTypes.Admin || RoleType == RoleTypes.Direction || RoleType == RoleTypes.Control || RoleType == RoleTypes.Production || PermissionGranted(iAgreedProfil) || PermissionGranted(iAgreedTPS))
                && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void BatchTPSAccess_Click(object sender, EventArgs e)
        {
            if (BatchManager.BatchTPSAccess)
            {
                BatchManager.BatchTPSConfirm = true;
                if (BatchManager.BatchTPSConfirmDateTime == DBNull.Value)
                    BatchManager.BatchTPSConfirmDateTime = Security.GetCurrentDate();
                if (BatchManager.BatchTPSConfirmUser == DBNull.Value)
                    BatchManager.BatchTPSConfirmUser = Security.CurrentUserID;
                BatchManager.BatchTPSAccess = false;
                BatchManager.BatchTPSCloseUser = Security.CurrentUserID;
                BatchManager.BatchTPSCloseDateTime = Security.GetCurrentDate();
            }
            else
            {
                BatchManager.BatchTPSAccess = true;
                BatchManager.BatchTPSCloseUser = DBNull.Value;
                BatchManager.BatchTPSCloseDateTime = DBNull.Value;
            }

            BatchManager.SaveBatch(FactoryID);
            BatchManager.SetMegaBatchEnable();
            BatchDataGrid.Refresh();
        }

        private void BatchProfilAccess_Click(object sender, EventArgs e)
        {
            if (BatchManager.BatchProfilAccess)
            {
                BatchManager.BatchProfilConfirm = true;
                if (BatchManager.BatchProfilConfirmDateTime == DBNull.Value)
                    BatchManager.BatchProfilConfirmDateTime = Security.GetCurrentDate();
                if (BatchManager.BatchProfilConfirmUser == DBNull.Value)
                    BatchManager.BatchProfilConfirmUser = Security.CurrentUserID;
                BatchManager.BatchProfilAccess = false;
                BatchManager.BatchProfilCloseUser = Security.CurrentUserID;
                BatchManager.BatchProfilCloseDateTime = Security.GetCurrentDate();
            }
            else
            {
                BatchManager.BatchProfilAccess = true;
                BatchManager.BatchProfilCloseUser = DBNull.Value;
                BatchManager.BatchProfilCloseDateTime = DBNull.Value;
            }

            BatchManager.SaveBatch(FactoryID);
            BatchManager.SetMegaBatchEnable();
            BatchDataGrid.Refresh();
        }

        private void BatchDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            //if (e.ColumnIndex == 4)
            //{
            //    if (Convert.ToBoolean(BatchDataGrid.Rows[e.RowIndex].Cells["ProfilEnabled"].Value))
            //        e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
            //    else
            //        e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            //}
            //if (e.ColumnIndex == 5)
            //{
            //    if (Convert.ToBoolean(BatchDataGrid.Rows[e.RowIndex].Cells["TPSEnabled"].Value))
            //        e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
            //    else
            //        e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            //}
        }

        private void MegaBatchProfilAccess_Click(object sender, EventArgs e)
        {
            BatchManager.MegaBatchProfilAccess = !BatchManager.MegaBatchProfilAccess;
            BatchManager.SetBatchEnabled(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]),
                FactoryID, BatchManager.MegaBatchProfilAccess);
            BatchManager.CloseMegaBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID, BatchManager.MegaBatchProfilAccess);
            MegaBatchDataGrid.Refresh();
            BatchDataGrid.Refresh();
            MegaBatchDataGrid_SelectionChanged(null, null);
        }

        private void MegaBatchDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            //if (MegaBatchDataGrid.Columns[e.ColumnIndex].Name == "ProfilEnabled")
            //{
            //    if (Convert.ToBoolean(MegaBatchDataGrid.Rows[e.RowIndex].Cells["ProfilEnabled"].Value))
            //        e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
            //    else
            //        e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            //}
            //if (MegaBatchDataGrid.Columns[e.ColumnIndex].Name == "TPSEnabled")
            //{
            //    if (Convert.ToBoolean(MegaBatchDataGrid.Rows[e.RowIndex].Cells["TPSEnabled"].Value))
            //        e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
            //    else
            //        e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            //}
        }

        private void MegaBatchTPSAccess_Click(object sender, EventArgs e)
        {
            BatchManager.MegaBatchTPSAccess = !BatchManager.MegaBatchTPSAccess;
            BatchManager.SetBatchEnabled(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]),
                FactoryID, BatchManager.MegaBatchTPSAccess);
            BatchManager.CloseMegaBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID, BatchManager.MegaBatchTPSAccess);
            MegaBatchDataGrid.Refresh();
            BatchDataGrid.Refresh();
            MegaBatchDataGrid_SelectionChanged(null, null);
        }

        private void MarketingBatchForm_Load(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();

            AddToBatch = false;
            InProduction = false;
            CreateBatch = false;
            CloseBatch = false;

            if (PermissionGranted(iAddToBatch))
            {
                AddToBatch = true;
            }
            if (PermissionGranted(iInProduction))
            {
                InProduction = true;
            }
            if (PermissionGranted(iCloseBatch))
            {
                CloseBatch = true;
            }
            if (PermissionGranted(iCreateBatch))
            {
                CreateBatch = true;
            }
            if (PermissionGranted(iCreateLabel))
            {
                CreateLabel = true;
            }
            //if (PermissionGranted(iConfirmBatch))
            //{
            //    ConfirmBatch = true;
            //}

            if (PermissionGranted(iAgreedProfil))
            {
                btnAgreedProfil.Visible = true;
                cmiConfirmProfilBatch.Visible = true;
            }
            if (PermissionGranted(iAgreedTPS))
            {
                cmiConfirmTPSBatch.Visible = true;
            }
            //if (PermissionGranted(iAgreed2TPS))
            //{
            //    btnAgreed2TPS.Visible = true;
            //}

            //Админ
            if (AddToBatch && InProduction && CloseBatch && CreateBatch && CreateLabel)
            {
                RoleType = RoleTypes.Admin;

                cmiCloseProfilBatch.Visible = true;
                cmiCloseTPSBatch.Visible = true;
                cmiCloseProfilMegaBatch.Visible = true;
                cmiCloseTPSMegaBatch.Visible = true;
                cmiConfirmProfilBatch.Visible = true;
                cmiSaveMegaBatch.Visible = true;
                cmiCreateLabel.Visible = true;
            }
            //Управление: нельзя открывать/закрывать партию, остальное - можно
            if (AddToBatch && InProduction && !CloseBatch && CreateBatch)
            {
                RoleType = RoleTypes.Control;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = true;
                cmiSaveMegaBatch.Visible = true;
                cmiCreateLabel.Visible = true;
            }
            //Вход для фа: можно открывать и закрывать партию
            if (!AddToBatch && !InProduction && CloseBatch && !CreateBatch && !CreateLabel)
            {
                RoleType = RoleTypes.Direction;

                cmiCloseProfilBatch.Visible = true;
                cmiCloseTPSBatch.Visible = true;
                cmiCloseProfilMegaBatch.Visible = true;
                cmiCloseTPSMegaBatch.Visible = true;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;
                cmiCreateLabel.Visible = false;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MegaOrderInProductionPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;
                RolesAndPermissionsTabControl.TabPages[1].PageVisible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMegaOrdersDataGrid.Height = MegaOrdersPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }
            //вход для пр-ва: можно формировать партию, выдавать в пр-во, нельзя создавать партию и открывать/закрывать её
            if (AddToBatch && InProduction && !CloseBatch && !CreateBatch && !CreateLabel)
            {
                RoleType = RoleTypes.Production;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = true;
                cmiSaveMegaBatch.Visible = false;
                cmiCreateLabel.Visible = false;

                MoveMegaOrdersButton.Visible = false;
                PickUpFrontsButton.Visible = false;
                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
            }
            //Маркетинг
            if (AddToBatch && !InProduction && !CloseBatch && !CreateBatch && !CreateLabel)
            {
                RoleType = RoleTypes.Marketing;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;
                cmiCreateLabel.Visible = false;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MegaOrderInProductionPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMegaOrdersDataGrid.Height = MegaOrdersPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }
            //вход для обычного пользователя: нельзя ничего сделать, только посмотреть партии и их содержимое
            if (!AddToBatch && !InProduction && !CloseBatch && !CreateBatch && !CreateLabel)
            {
                RoleType = RoleTypes.Ordinary;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;
                cmiCreateLabel.Visible = false;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MegaOrderInProductionPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;
                RolesAndPermissionsTabControl.TabPages[1].PageVisible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMegaOrdersDataGrid.Height = MegaOrdersPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }
            //Склад
            if (!AddToBatch && !InProduction && !CloseBatch && !CreateBatch && CreateLabel)
            {
                RoleType = RoleTypes.Store;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;
                cmiCreateLabel.Visible = true;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MegaOrderInProductionPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;
                RolesAndPermissionsTabControl.TabPages[1].PageVisible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMegaOrdersDataGrid.Height = MegaOrdersPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }

            cmiCloseTPSBatch.Visible = false;
            cmiCloseTPSMegaBatch.Visible = false;
            cmiConfirmTPSBatch.Visible = false;

            BatchDataGrid.Columns["TPSConfirm"].Visible = false;
            BatchDataGrid.Columns["TPSBatchClose"].Visible = false;
            MegaBatchDataGrid.Columns["TPSBatchClose"].Visible = false;
            BatchDataGrid.Columns["TPSName"].Visible = false;


        }

        void btnAgreed1TPS_Click(object sender, EventArgs e)
        {
            int MegaBatchID = 0;
            int TPSAgreedUserID = -1;
            int TPSCloseUserID = -1;
            if (MegaBatchDataGrid.SelectedRows.Count != 0 && MegaBatchDataGrid.SelectedRows[0].Cells["TPSCloseUserID"].Value != DBNull.Value)
                TPSCloseUserID = Convert.ToInt32(MegaBatchDataGrid.SelectedRows[0].Cells["TPSCloseUserID"].Value);
            if (MegaBatchDataGrid.SelectedRows.Count != 0 && MegaBatchDataGrid.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(MegaBatchDataGrid.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (MegaBatchDataGrid.SelectedRows.Count != 0 && MegaBatchDataGrid.SelectedRows[0].Cells["TPSAgreedUserID"].Value != DBNull.Value)
                TPSAgreedUserID = Convert.ToInt32(MegaBatchDataGrid.SelectedRows[0].Cells["TPSAgreedUserID"].Value);
            if (TPSCloseUserID != -1)
            {
                LightMessageBox.Show(ref TopForm, false,
                        "Партия утверждена и закрыта",
                        "Утверждение партии");
                return;
            }
            if (TPSAgreedUserID != -1)
            {
                bool OKCancel = LightMessageBox.Show(ref TopForm, true,
                        "Ваше утверждение уже выставлено. Переутвердить?",
                        "Утверждение партии");
                if (!OKCancel)
                    return;
            }
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            BatchManager.SetMegaBatchAgreement(MegaBatchID, FactoryID);
            BatchManager.SaveMegaBatch(FactoryID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        void btnAgreed1Profil_Click(object sender, EventArgs e)
        {
            int MegaBatchID = 0;
            int ProfilAgreedUserID = -1;
            int ProfilCloseUserID = -1;
            if (MegaBatchDataGrid.SelectedRows.Count != 0 && MegaBatchDataGrid.SelectedRows[0].Cells["ProfilCloseUserID"].Value != DBNull.Value)
                ProfilCloseUserID = Convert.ToInt32(MegaBatchDataGrid.SelectedRows[0].Cells["ProfilCloseUserID"].Value);
            if (MegaBatchDataGrid.SelectedRows.Count != 0 && MegaBatchDataGrid.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(MegaBatchDataGrid.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (MegaBatchDataGrid.SelectedRows.Count != 0 && MegaBatchDataGrid.SelectedRows[0].Cells["ProfilAgreedUserID"].Value != DBNull.Value)
                ProfilAgreedUserID = Convert.ToInt32(MegaBatchDataGrid.SelectedRows[0].Cells["ProfilAgreedUserID"].Value);
            if (ProfilCloseUserID != -1)
            {
                LightMessageBox.Show(ref TopForm, false,
                        "Партия утверждена и закрыта",
                        "Утверждение партии");
                return;
            }
            if (ProfilAgreedUserID != -1)
            {
                bool OKCancel = LightMessageBox.Show(ref TopForm, true,
                        "Ваше утверждение уже выставлено. Переутвердить?",
                        "Утверждение партии");
                if (!OKCancel)
                    return;
            }
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            BatchManager.SetMegaBatchAgreement(MegaBatchID, FactoryID);
            BatchManager.SaveMegaBatch(FactoryID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void cmiCreateLabel_Click(object sender, EventArgs e)
        {
            //PhantomForm PhantomForm = new Infinium.PhantomForm();
            //PhantomForm.Show();

            //MarketingPalletsForm MarketingPalletsForm = new MarketingPalletsForm(
            //    Convert.ToInt32(((DataRowView)BatchManager.BatchMegaOrdersBindingSource.Current)["MegaOrderID"]), FactoryID,
            //    Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]));

            //TopForm = MarketingPalletsForm;
            //MarketingPalletsForm.ShowDialog();

            //PhantomForm.Close();

            //PhantomForm.Dispose();
            //MarketingPalletsForm.Dispose();
            //TopForm = null;
        }

        private void BatchMegaOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((RoleType == RoleTypes.Admin || RoleType == RoleTypes.Store || RoleType == RoleTypes.Control) && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void cmiConfirmBatch_Click(object sender, EventArgs e)
        {
            if (!BatchManager.BatchProfilConfirm)
            {
                BatchManager.BatchProfilConfirmUser = Security.CurrentUserID;
                BatchManager.BatchProfilConfirmDateTime = Security.GetCurrentDate();
                BatchManager.BatchProfilConfirm = true;
                BatchManager.SaveBatch(FactoryID);
            }
        }

        private void kryptonContextMenu3_Opening(object sender, CancelEventArgs e)
        {
            if (FactoryID == 1)
            {
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Direction)
                {
                    cmiCloseProfilBatch.Visible = true;
                    cmiCloseTPSBatch.Visible = false;
                }
                if (RoleType == RoleTypes.Admin || PermissionGranted(iAgreedProfil))
                {
                    cmiConfirmProfilBatch.Visible = true;
                    cmiConfirmTPSBatch.Visible = false;
                    if (BatchManager.BatchProfilConfirm)
                        cmiConfirmProfilBatch.Visible = false;
                }
                if (BatchManager.BatchProfilAccess)
                    cmiCloseProfilBatch.Text = "Закрыть партию";
                else
                    cmiCloseProfilBatch.Text = "Открыть партию";
            }
            if (FactoryID == 2)
            {
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Direction)
                {
                    cmiCloseProfilBatch.Visible = false;
                    cmiCloseTPSBatch.Visible = true;
                }
                if (RoleType == RoleTypes.Admin || PermissionGranted(iAgreedTPS))
                {
                    cmiConfirmTPSBatch.Visible = true;
                    cmiConfirmProfilBatch.Visible = false;
                    if (BatchManager.BatchTPSConfirm)
                        cmiConfirmTPSBatch.Visible = false;
                }
                if (BatchManager.BatchTPSAccess)
                    cmiCloseTPSBatch.Text = "Закрыть партию";
                else
                    cmiCloseTPSBatch.Text = "Открыть партию";
            }
        }

        private void cmiConfirmTPSBatch_Click(object sender, EventArgs e)
        {
            if (!BatchManager.BatchTPSConfirm)
            {
                BatchManager.BatchTPSConfirmUser = Security.CurrentUserID;
                BatchManager.BatchTPSConfirmDateTime = Security.GetCurrentDate();
                BatchManager.BatchTPSConfirm = true;
                BatchManager.SaveBatch(FactoryID);
            }
        }

        private void kryptonContextMenu1_Opening(object sender, CancelEventArgs e)
        {
            if (FactoryID == 1)
            {
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Direction)
                {
                    cmiCloseProfilMegaBatch.Visible = true;
                    cmiCloseTPSMegaBatch.Visible = false;
                }
                if (MegaBatchDataGrid.SelectedRows[0].Cells["ProfilBatchClose"].Value != null)
                {
                    if (Convert.ToBoolean(MegaBatchDataGrid.SelectedRows[0].Cells["ProfilBatchClose"].Value))
                    {
                        btnAgreedProfil.Visible = false;
                        cmiCloseProfilMegaBatch.Text = "Открыть группу";
                    }
                    else
                    {
                        btnAgreedProfil.Visible = true;
                        cmiCloseProfilMegaBatch.Text = "Закрыть группу";
                    }
                }
            }
            if (FactoryID == 2)
            {
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Direction)
                {
                    cmiCloseProfilMegaBatch.Visible = false;
                    cmiCloseTPSMegaBatch.Visible = true;
                }
                if (MegaBatchDataGrid.SelectedRows[0].Cells["TPSBatchClose"].Value != null)
                {
                    if (Convert.ToBoolean(MegaBatchDataGrid.SelectedRows[0].Cells["TPSBatchClose"].Value))
                    {
                        btnAgreedTPS.Visible = false;
                        cmiCloseTPSMegaBatch.Text = "Открыть группу";
                    }
                    else
                    {
                        btnAgreedTPS.Visible = true;
                        cmiCloseTPSMegaBatch.Text = "Закрыть группу";
                    }
                }
            }
        }

        private void MoveMainOrdersButton_Click(object sender, EventArgs e)
        {
            //if (!BatchManager.IsBatchEnabled(Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]), FactoryID))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //           "Партия сформирована, перенос заказов запрещен",
            //           "Ошибка добавления");
            //    return;
            //}

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
            bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
            bool BatchInProduction = BatchInProductionCheckBox.Checked;
            bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
            bool BatchOnExp = BatchOnExpCheckBox.Checked;
            bool BatchDispatched = BatchDispatchCheckBox.Checked;

            MainOrdersArray.Clear();
            for (int i = 0; i < BatchMainOrdersDataGrid.SelectedRows.Count; i++)
                MainOrdersArray.Add(Convert.ToInt32(BatchMainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value));

            if (BatchMainOrdersDataGrid.SelectedRows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                          "Не выбран ни один подзаказ",
                          "Ошибка переноса");

                PhantomForm.Close();

                PhantomForm.Dispose();

                return;
            }

            MarketingBatchSelectMenu SelectMenuForm = new MarketingBatchSelectMenu(this, BatchManager, FactoryID);

            TopForm = SelectMenuForm;
            SelectMenuForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            SelectMenuForm.Dispose();
            TopForm = null;

            if (BatchManager.NewBatch)
            {
                //int[] MainOrders = new int[BatchMainOrdersDataGrid.SelectedRows.Count];
                //for (int i = 0; i < BatchMainOrdersDataGrid.SelectedRows.Count; i++)
                //    MainOrders[i] = Convert.ToInt32(BatchMainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);
                BatchManager.MoveOrdersToNewPart(MainOrdersArray.OfType<int>().ToArray(), FactoryID);

                BatchManager.FilterBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID);
                Filter();
            }

            if (BatchManager.OldBatch)
            {
                MegaBatchDataGrid.Enabled = true;
                BatchDataGrid.Enabled = true;

                AddBatchButton.Visible = false;
                RemoveBatchButton.Visible = false;
                PrintBatchButton.Visible = false;
                MoveBatchButton.Visible = false;
                EditBatchNameButton.Visible = false;
                OKMoveOrdersButton.Visible = true;
                CancelMoveOrdersButton.Visible = true;
            }
        }

        private void MegaBatchDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns[e.ColumnIndex].Name == "ProfilEnabledColumn")
            {
                if (grid.Rows[e.RowIndex].Cells["ProfilBatchClose"].Value != null)
                {
                    if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["ProfilBatchClose"].Value))
                    {
                        e.Value = ImageList1.Images[0];
                    }
                    else
                    {
                        e.Value = ImageList1.Images[1];
                    }
                }
            }
            if (grid.Columns[e.ColumnIndex].Name == "TPSEnabledColumn")
            {
                if (grid.Rows[e.RowIndex].Cells["TPSBatchClose"].Value != null)
                {
                    if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["TPSBatchClose"].Value))
                    {
                        e.Value = ImageList1.Images[0];
                    }
                    else
                    {
                        e.Value = ImageList1.Images[1];
                    }
                }
            }

            if (e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int MegaBatchID;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["MegaBatchID"].Value != DBNull.Value)
                {
                    MegaBatchID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["MegaBatchID"].Value);
                    DisplayName = BatchManager.GetMegaBatchAgreement(MegaBatchID);
                }
                cell.ToolTipText = DisplayName;
            }
        }

        private void BatchDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns[e.ColumnIndex].Name == "ProfilEnabledColumn")
            {
                if (grid.Rows[e.RowIndex].Cells["ProfilBatchClose"].Value != null)
                {
                    if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["ProfilBatchClose"].Value))
                    {
                        e.Value = ImageList1.Images[0];
                    }
                    else
                    {
                        e.Value = ImageList1.Images[1];
                    }
                }
            }
            if (grid.Columns[e.ColumnIndex].Name == "TPSEnabledColumn")
            {
                if (grid.Rows[e.RowIndex].Cells["TPSBatchClose"].Value != null)
                {
                    if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["TPSBatchClose"].Value))
                    {
                        e.Value = ImageList1.Images[0];
                    }
                    else
                    {
                        e.Value = ImageList1.Images[1];
                    }
                }
            }
        }

        private void kryptonCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            MegaBatchDataGrid_SelectionChanged(null, null);
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            BatchReport MarketingBatchReport = new BatchReport(ref DecorCatalogOrder);
            int[] MainOrders = BatchManager.GetBatchMainOrders().OfType<int>().ToArray();
            MarketingBatchReport.CreateReportForMaketing(MainOrders);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
