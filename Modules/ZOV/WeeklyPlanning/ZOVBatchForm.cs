using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.ZOV.WeeklyPlanning;

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
    public partial class ZOVBatchForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        const int iConfirmBatch = 65;
        const int iCreateBatch = 36;
        const int iAddToBatch = 37;
        const int iInProduction = 23;
        const int iCloseBatch = 39;

        bool NeedRefresh = false;
        bool NeedSplash = false;
        bool bFrontType = false;
        bool bFrameColor = false;

        bool ConfirmBatch = false;
        bool CreateBatch = false;
        bool AddToBatch = false;
        bool InProduction = false;
        bool CloseBatch = false;

        int FactoryID = 1;
        int FormEvent = 0;

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
        ZOVPickFrontsSelectForm PickFrontsSelectForm;

        DataTable RolePermissionsDataTable;

        public BatchManager BatchManager;
        public Modules.ZOV.DecorCatalogOrder DecorCatalogOrder;
        public BatchReport ReportZOV;

        public ZOVBatchForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID);

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

        private void ZOVBatchForm_Shown(object sender, EventArgs e)
        {
            if (ZOVProfilCheckButton.Checked && !ZOVTPSCheckButton.Checked)
            {
                FactoryID = 1;
                if (MenuButton.Checked)
                {
                    OrdersFilterPanel_Profil.Visible = true;
                    OrdersFilterPanel_TPS.Visible = false;
                }
                BatchDataGrid.Columns["ProfilConfirm"].Visible = true;
                BatchDataGrid.Columns["ProfilEnabled"].Visible = true;
                BatchDataGrid.Columns["ProfilName"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSConfirm"].Visible = false;
                BatchDataGrid.Columns["TPSEnabled"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEnabled"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEnabled"].Visible = false;
            }
            if (!ZOVProfilCheckButton.Checked && ZOVTPSCheckButton.Checked)
            {
                FactoryID = 2;
                if (MenuButton.Checked)
                {
                    OrdersFilterPanel_Profil.Visible = false;
                    OrdersFilterPanel_TPS.Visible = true;
                }
                BatchDataGrid.Columns["ProfilConfirm"].Visible = false;
                BatchDataGrid.Columns["ProfilEnabled"].Visible = false;
                BatchDataGrid.Columns["ProfilName"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSConfirm"].Visible = true;
                BatchDataGrid.Columns["TPSEnabled"].Visible = true;
                BatchDataGrid.Columns["TPSName"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEnabled"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEnabled"].Visible = true;
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

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (!MenuButton.Checked)
            {
                BatchFilterPanel.Visible = false;
                OrdersFilterPanel_Profil.Visible = false;
                OrdersFilterPanel_TPS.Visible = false;
            }
            else
            {
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 0)
                {
                    BatchFilterPanel.Visible = true;
                    OrdersFilterPanel_Profil.Visible = false;
                    OrdersFilterPanel_TPS.Visible = false;
                }
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 1)
                {
                    BatchFilterPanel.Visible = false;
                    if (ZOVProfilCheckButton.Checked)
                    {
                        OrdersFilterPanel_Profil.Visible = true;
                        OrdersFilterPanel_TPS.Visible = false;
                    }
                    if (ZOVTPSCheckButton.Checked)
                    {
                        OrdersFilterPanel_Profil.Visible = false;
                        OrdersFilterPanel_TPS.Visible = true;
                    }
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

            DecorCatalogOrder = new Modules.ZOV.DecorCatalogOrder();

            BatchManager = new BatchManager(ref MegaBatchDataGrid,
                ref BatchDataGrid,
                ref MainOrdersDataGrid, ref MainOrdersFrontsOrdersDataGrid,
                ref MainOrdersDecorTabControl, ref MainOrdersTabControl,
                ref BatchMainOrdersDataGrid, ref BatchFrontsOrdersDataGrid,
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

            ReportZOV = new BatchReport(ref DecorCatalogOrder);
        }

        private void Filter()
        {
            bool OnProduction = false;
            bool NotProduction = false;
            bool InProduction = false;

            DateTime FirstDay = FilterFromDateTimePicker.Value;
            DateTime SecondDay = FilterToDateTimePicker.Value;

            if (FactoryID == 1)
            {
                OnProduction = OnProductionCheckBox_Profil.Checked;
                NotProduction = NotProductionCheckBox_Profil.Checked;
                InProduction = InProductionCheckBox_Profil.Checked;
            }
            if (FactoryID == 2)
            {
                OnProduction = OnProductionCheckBox.Checked;
                NotProduction = NotProductionCheckBox.Checked;
                InProduction = InProductionCheckBox.Checked;
            }

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                BatchManager.Filter_MegaBatches_ByFactory(FactoryID);
                BatchManager.Filter(FactoryID, OnProduction, NotProduction, InProduction,
                    FirstDay, SecondDay);

                BatchManager.GetMainOrdersNotInBatch(FactoryID);
                BatchManager.GetOrdersNotInProduction(FactoryID);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                BatchManager.Filter_MegaBatches_ByFactory(FactoryID);
                BatchManager.Filter(FactoryID, OnProduction, NotProduction, InProduction,
                    FirstDay, SecondDay);

                BatchManager.GetMainOrdersNotInBatch(FactoryID);
                BatchManager.GetOrdersNotInProduction(FactoryID);
            }
        }

        private void BatchDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.BatchBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"] != DBNull.Value)
                    {
                        bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
                        bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
                        bool BatchInProduction = BatchInProductionCheckBox.Checked;
                        bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
                        bool BatchDispatched = BatchDispatchCheckBox.Checked;

                        ArrayList MainOrdersArray = new ArrayList();

                        if (NeedSplash)
                        {
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;
                            NeedSplash = false;

                            BatchManager.FilterBatchMainOrdersByBatch(Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]), FactoryID,
                                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched);

                            if (GroupsCheckBox.Checked)
                            {
                                int[] Batches = BatchManager.GetSelectedBatch().OfType<int>().ToArray();

                                if (Batches.Count() > 0)
                                {
                                    BatchManager.GetProductInfo(FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched, Batches);
                                }
                            }

                            NeedSplash = true;
                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                        }
                        else
                        {
                            BatchManager.FilterBatchMainOrdersByBatch(Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]), FactoryID,
                                BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched);

                            if (GroupsCheckBox.Checked)
                            {
                                int[] Batches = BatchManager.GetSelectedBatch().OfType<int>().ToArray();

                                if (Batches.Count() > 0)
                                {
                                    BatchManager.GetProductInfo(FactoryID,
                                        BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched, Batches);
                                }
                            }
                        }

                        BatchManager.GetOrdersNotInProduction(FactoryID);

                        BatchManager.CurrentBatchID = Convert.ToInt32(Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]));

                    }
                }
                else
                    BatchManager.ClearMainOrders();
        }

        #region MainOrders

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            ClientInfoLabel.Text = string.Empty;
            OrderLabel.Text = string.Empty;
            DocDateLabel.Text = string.Empty;

            if (BatchManager != null)
                if (BatchManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        BatchManager.FilterProductsByMainOrder(Convert.ToInt32(((DataRowView)BatchManager.MainOrdersBindingSource.Current)["MainOrderID"]), FactoryID);
                        if (MainOrdersTabControl.TabPages[0].PageVisible && MainOrdersTabControl.TabPages[1].PageVisible)
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];

                        bool OnProduction = OnProductionCheckBox_Profil.Checked;
                        bool NotProduction = NotProductionCheckBox_Profil.Checked;
                        bool InProduction = InProductionCheckBox_Profil.Checked;

                        if (FactoryID == 2)
                        {
                            OnProduction = OnProductionCheckBox.Checked;
                            NotProduction = NotProductionCheckBox.Checked;
                            InProduction = InProductionCheckBox.Checked;
                        }
                        string ClientName = string.Empty;
                        string OrderNumber = string.Empty;
                        string DocDate = string.Empty;

                        if (((DataRowView)BatchManager.MainOrdersBindingSource.Current)["ClientName"] != DBNull.Value)
                            ClientName = ((DataRowView)BatchManager.MainOrdersBindingSource.Current)["ClientName"].ToString();
                        if (((DataRowView)BatchManager.MainOrdersBindingSource.Current)["DocNumber"] != DBNull.Value)
                            OrderNumber = ((DataRowView)BatchManager.MainOrdersBindingSource.Current)["DocNumber"].ToString();
                        if (((DataRowView)BatchManager.MainOrdersBindingSource.Current)["DocDateTime"] != DBNull.Value)
                            DocDate = Convert.ToDateTime(((DataRowView)BatchManager.MainOrdersBindingSource.Current)["DocDateTime"]).ToString("dd.MM.yyyy");

                        string ClientInfo = ClientName;
                        string OrderInfo = "Кухня: " + OrderNumber;
                        string DocDateInfo = "Создан: " + DocDate;

                        ClientInfoLabel.Text = ClientInfo;
                        OrderLabel.Text = OrderInfo;
                        DocDateLabel.Text = DocDateInfo;

                        int[] MainOrders = BatchManager.GetSelectedMainOrders().OfType<int>().ToArray();

                        if (MainOrders.Count() > 0)
                        {
                            xtraTabControl3.TabPages[1].PageVisible = true;
                            BatchManager.GetPreProductInfo(MainOrders, FactoryID,
                                OnProduction, NotProduction, InProduction);
                        }
                        else
                        {
                            xtraTabControl3.TabPages[1].PageVisible = false;
                        }
                    }
                }
                else
                {
                    BatchManager.FilterProductsByMainOrder(-1, FactoryID);
                    BatchManager.ClearPreProductsGrids();
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

        #region BatchMainOrders

        private void BatchMainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.BatchMainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.BatchMainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
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

                            BatchManager.FilterBatchProductsByMainOrder(Convert.ToInt32(((DataRowView)BatchManager.BatchMainOrdersBindingSource.Current)["MainOrderID"]), FactoryID);
                            if (BatchMainOrdersTabControl.TabPages[0].PageVisible && BatchMainOrdersTabControl.TabPages[1].PageVisible)
                                BatchMainOrdersTabControl.SelectedTabPage = BatchMainOrdersTabControl.TabPages[0];

                            int[] MainOrders = BatchManager.GetSelectedBatchMainOrders().OfType<int>().ToArray();

                            if (MainOrders.Count() > 0)
                            {
                                BatchManager.GetProductInfo(MainOrders, FactoryID,
                                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched);
                            }

                            NeedSplash = true;
                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                        }
                        else
                        {
                            BatchManager.FilterBatchProductsByMainOrder(Convert.ToInt32(((DataRowView)BatchManager.BatchMainOrdersBindingSource.Current)["MainOrderID"]), FactoryID);
                            if (BatchMainOrdersTabControl.TabPages[0].PageVisible && BatchMainOrdersTabControl.TabPages[1].PageVisible)
                                BatchMainOrdersTabControl.SelectedTabPage = BatchMainOrdersTabControl.TabPages[0];

                            int[] MainOrders = BatchManager.GetSelectedBatchMainOrders().OfType<int>().ToArray();

                            if (MainOrders.Count() > 0)
                            {
                                BatchManager.GetProductInfo(MainOrders, FactoryID,
                                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched);
                            }
                        }
                    }
                }
                else
                {
                    BatchManager.FilterBatchProductsByMainOrder(-1, FactoryID);
                    BatchManager.ClearProductsGrids();
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
            ArrayList MainOrders = BatchManager.GetSelectedMainOrders();

            for (int i = 0; i < MainOrders.Count; i++)
            {
                if (!BatchManager.IsDoubleOrder(Convert.ToInt32(MainOrders[i])))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Кухня " + MainOrders[i].ToString() + " не прошла двойное вбивание, её нельзя включить в партию",
                           "Ошибка добавления");
                    MainOrders.RemoveAt(i);
                }
            }
            if (MainOrders.Count < 1)
                return;

            if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Партия сформирована, добавление заказов в неё запрещено. Создайте новую партию",
                       "Ошибка добавления");
                return;
            }

            BatchManager.AddToBatch(BatchID, MainOrders.OfType<int>().ToArray(), FactoryID);
            BatchManager.SetMainOrderOnProduction(MainOrders.OfType<int>().ToArray(), FactoryID, true);
            Filter();
            BatchManager.GetMainOrdersNotInBatch(FactoryID);
        }

        private void RemoveMainOrderButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchMainOrdersBindingSource.Count < 1)
                return;

            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current).Row["BatchID"]);
            if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Партия сформирована, удаление заказов из неё запрещено.",
                       "Ошибка удаления");
                return;
            }

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Выбранные подзаказы будут убраны из партии. Продолжить?",
                    "Редактирование партии");

            if (OKCancel)
            {
                int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current).Row["MegaBatchID"]);
                int[] MainOrders = BatchManager.GetSelectedBatchMainOrders().OfType<int>().ToArray();

                if (BatchManager.RemoveMainOrderFromBatch(BatchID, FactoryID))
                {
                    BatchManager.SetMainOrderOnProduction(MainOrders, FactoryID, false);
                    Filter();
                    BatchManager.GetMainOrdersNotInBatch(FactoryID);
                }
            }
        }

        #endregion

        #region Set in production

        private void PrintMainOrderButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchMainOrdersBindingSource.Count < 1)
                return;

            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
            int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]);

            int[] MainOrders = BatchManager.GetSelectedBatchMainOrders(FactoryID);
            if (MainOrders.Count() < 1)
                return;

            if (!BatchManager.HasOrders(MainOrders, FactoryID))
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Выбранные подзаказы будут распечатаны и отправлены в производство. Продолжить?",
                    "В производство");

            if (OKCancel)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Идёт обработка данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                ReportZOV.CreateReport(MegaBatchID, BatchID, MainOrders, FactoryID);
                BatchManager.SetMainOrderInProduction(MainOrders, FactoryID);

                bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
                bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
                bool BatchInProduction = BatchInProductionCheckBox.Checked;
                bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
                bool BatchDispatched = BatchDispatchCheckBox.Checked;

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                BatchManager.FilterBatchMainOrdersByBatch(BatchID, FactoryID,
                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched);
            }
        }

        #endregion

        #region Filter functions

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
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 0)
                {
                    OrdersFilterPanel_Profil.Visible = false;
                    OrdersFilterPanel_TPS.Visible = false;
                    BatchFilterPanel.Visible = true;
                }
                if (RolesAndPermissionsTabControl.SelectedTabPageIndex == 1)
                {
                    BatchFilterPanel.Visible = false;
                    if (ZOVProfilCheckButton.Checked)
                    {
                        OrdersFilterPanel_Profil.Visible = true;
                        OrdersFilterPanel_TPS.Visible = false;
                    }
                    if (ZOVTPSCheckButton.Checked)
                    {
                        OrdersFilterPanel_Profil.Visible = false;
                        OrdersFilterPanel_TPS.Visible = true;
                    }
                }
            }
        }

        #region Filter factory

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            if (ZOVProfilCheckButton.Checked && !ZOVTPSCheckButton.Checked)
            {
                FactoryID = 1;
                if (MenuButton.Checked)
                {
                    OrdersFilterPanel_Profil.Visible = true;
                    OrdersFilterPanel_TPS.Visible = false;
                }
                BatchDataGrid.Columns["ProfilConfirm"].Visible = true;
                BatchDataGrid.Columns["ProfilEnabled"].Visible = true;
                BatchDataGrid.Columns["ProfilName"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSConfirm"].Visible = false;
                BatchDataGrid.Columns["TPSEnabled"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["ProfilEnabled"].Visible = true;
                MegaBatchDataGrid.Columns["TPSEnabled"].Visible = false;
            }
            if (!ZOVProfilCheckButton.Checked && ZOVTPSCheckButton.Checked)
            {
                FactoryID = 2;
                if (MenuButton.Checked)
                {
                    OrdersFilterPanel_Profil.Visible = false;
                    OrdersFilterPanel_TPS.Visible = true;
                }
                BatchDataGrid.Columns["ProfilConfirm"].Visible = false;
                BatchDataGrid.Columns["ProfilEnabled"].Visible = false;
                BatchDataGrid.Columns["ProfilName"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseDateTime"].Visible = false;
                BatchDataGrid.Columns["ProfilConfirmUserColumn"].Visible = false;
                BatchDataGrid.Columns["ProfilCloseUserColumn"].Visible = false;
                BatchDataGrid.Columns["TPSConfirm"].Visible = true;
                BatchDataGrid.Columns["TPSEnabled"].Visible = true;
                BatchDataGrid.Columns["TPSName"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSCloseDateTime"].Visible = true;
                BatchDataGrid.Columns["TPSConfirmUserColumn"].Visible = true;
                BatchDataGrid.Columns["TPSCloseUserColumn"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEntryDateTime"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEntryDateTime"].Visible = true;
                MegaBatchDataGrid.Columns["ProfilEnabled"].Visible = false;
                MegaBatchDataGrid.Columns["TPSEnabled"].Visible = true;
            }

            Filter();
        }
        #endregion

        private void BatchMainOrdersDataGrid_Sorted(object sender, EventArgs e)
        {
            BatchManager.GetOrdersNotInProduction(FactoryID);
        }

        private void PrintBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchMainOrdersBindingSource.Count < 1)
                return;

            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);

            int[] MainOrders = BatchManager.GetSelectedBatchMainOrders(FactoryID);

            if (!BatchManager.HasOrders(BatchID, FactoryID))
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Партия будет распечатана и отправлена в производство. Продолжить?",
                    "В производство");

            if (OKCancel)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]);
                ReportZOV.CreateReport(MegaBatchID, BatchID, FactoryID);
                BatchManager.SetMainOrderInProduction(BatchID, FactoryID);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                bool BatchOnProduction = BatchOnProductionCheckBox.Checked;
                bool BatchNotProduction = BatchNotProductionCheckBox.Checked;
                bool BatchInProduction = BatchInProductionCheckBox.Checked;
                bool BatchOnStorage = BatchOnStorageCheckBox.Checked;
                bool BatchDispatched = BatchDispatchCheckBox.Checked;

                BatchManager.FilterBatchMainOrdersByBatch(BatchID, FactoryID,
                    BatchOnProduction, BatchNotProduction, BatchInProduction, BatchOnStorage, BatchDispatched);
            }
        }

        private void MoveOrdersButton_Click(object sender, EventArgs e)
        {
            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current).Row["BatchID"]);
            if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Партия сформирована, перенос запрещён.",
                       "Ошибка переноса");
                return;
            }

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            BatchManager.GetCurrentFrontID();
            BatchManager.GetCurrentFrameColorID();
            MainOrdersArray = BatchManager.GetSelectedBatchMainOrders();

            if (MainOrdersArray.Count < 1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                          "Не выбран ни один заказ",
                          "Ошибка переноса");

                PhantomForm.Close();

                PhantomForm.Dispose();

                return;
            }

            ZOVBatchSelectMenu SelectMenuForm = new ZOVBatchSelectMenu(this, BatchManager, FactoryID);

            TopForm = SelectMenuForm;
            SelectMenuForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            SelectMenuForm.Dispose();
            TopForm = null;

            if (BatchManager.NewBatch)
            {
                MegaBatchDataGrid.Enabled = true;
                BatchDataGrid.Enabled = true;

                BatchManager.MoveOrdersToNewPart(MainOrdersArray.OfType<int>().ToArray(), FactoryID);

                this.PickUpFrontsButton.OverrideDefault.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateCommon.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateTracking.Back.Color1 = System.Drawing.Color.MediumPurple;

                MegaBatchDataGrid.Enabled = true;
                BatchDataGrid.Enabled = true;
                PickUpFrontsButton.Tag = "true";
                PickUpFrontsButton.Text = "Подобрать";

                BatchManager.FilterBatch(
                    Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]),
                    FactoryID);
                Filter();
            }
            if (BatchManager.OldBatch)
            {
                AddBatchButton.Visible = false;
                RemoveBatchButton.Visible = false;
                PrintBatchButton.Visible = false;
                MoveBatchButton.Visible = false;
                EditBatchNameButton.Visible = false;
                OKMoveOrdersButton.Visible = true;
                CancelMoveOrdersButton.Visible = true;
            }
        }

        private void PickUpFrontsButton_Click(object sender, EventArgs e)
        {
            bool OnProduction = false;
            bool NotProduction = false;
            bool InProduction = false;

            if (FactoryID == 1)
            {
                OnProduction = OnProductionCheckBox_Profil.Checked;
                NotProduction = NotProductionCheckBox_Profil.Checked;
                InProduction = InProductionCheckBox_Profil.Checked;
            }
            if (FactoryID == 2)
            {
                OnProduction = OnProductionCheckBox.Checked;
                NotProduction = NotProductionCheckBox.Checked;
                InProduction = InProductionCheckBox.Checked;
            }

            if (PickUpFrontsButton.Tag.ToString() == "true")
            {
                this.PickUpFrontsButton.OverrideDefault.Back.Color1 = System.Drawing.Color.Crimson;
                this.PickUpFrontsButton.StateCommon.Back.Color1 = System.Drawing.Color.Crimson;
                this.PickUpFrontsButton.StateTracking.Back.Color1 = System.Drawing.Color.Crimson;

                PickUpFrontsButton.Tag = "false";
                PickUpFrontsButton.Text = "Обновить";

                DateTime FirstDay = FilterFromDateTimePicker.Value;
                DateTime SecondDay = FilterToDateTimePicker.Value;

                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                PickFrontsSelectForm = new ZOVPickFrontsSelectForm(FactoryID, ref FrontIDs);

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

                    if (bFrameColor)
                        BatchManager.PickUpFrontsTPS(FactoryID, OnProduction, NotProduction, InProduction,
                             FirstDay, SecondDay);

                    if (bFrontType)
                    {
                        if (FrontIDs.Count > 0)
                        {
                            BatchManager.PickUpFrontsProfil(FactoryID, OnProduction, NotProduction, InProduction,
                                FirstDay, SecondDay, FrontIDs);
                        }
                    }

                    BatchManager.GetMainOrdersNotInBatch(FactoryID);

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

                return;
            }
            if (PickUpFrontsButton.Tag.ToString() == "false")
            {
                this.PickUpFrontsButton.OverrideDefault.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateCommon.Back.Color1 = System.Drawing.Color.MediumPurple;
                this.PickUpFrontsButton.StateTracking.Back.Color1 = System.Drawing.Color.MediumPurple;

                PickUpFrontsButton.Tag = "true";
                PickUpFrontsButton.Text = "Подобрать";
                Filter();
                return;
            }
        }

        private void MegaBatchDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (BatchManager != null)
                if (BatchManager.MegaBatchBindingSource.Count > 0)
                {
                    if (((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"] != DBNull.Value)
                    {
                        BatchManager.FilterBatch(
                            Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]),
                            FactoryID);
                        BatchManager.CurrentMegaBatchID = Convert.ToInt32(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]));
                    }
                }
                else
                    BatchManager.ClearBatch();
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

        private void RemoveMegaBatchButton_Click(object sender, EventArgs e)
        {
            if (BatchManager == null || BatchManager.BatchBindingSource.Count < 1)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Партия будет расформирована и удалена. Продолжить?",
                    "Удаление партии");

            if (OKCancel)
            {
                int MegaBatchID = Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current).Row["MegaBatchID"]);

                BatchManager.RemoveMegaBatch(MegaBatchID, FactoryID);
            }
        }

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

        private void OKMoveOrdersButton_Click(object sender, EventArgs e)
        {
            int[] MainOrders = MainOrdersArray.OfType<int>().ToArray();
            int BatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
            if (!BatchManager.IsBatchEnabled(BatchID, FactoryID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Партия сформирована, перенос запрещен.",
                       "Ошибка удаления");
                return;
            }

            if (BatchManager.OldBatch)
            {
                int SelectedBatchID = Convert.ToInt32(((DataRowView)BatchManager.BatchBindingSource.Current)["BatchID"]);
                BatchManager.MoveOrdersToSelectedBatch(SelectedBatchID, MainOrders, FactoryID);
            }

            EditBatchNameButton.Visible = true;
            AddBatchButton.Visible = true;
            RemoveBatchButton.Visible = true;
            PrintBatchButton.Visible = true;
            MoveBatchButton.Visible = true;
            OKMoveOrdersButton.Visible = false;
            CancelMoveOrdersButton.Visible = false;
        }

        private void CancelMoveOrdersButton_Click(object sender, EventArgs e)
        {
            AddBatchButton.Visible = true;
            RemoveBatchButton.Visible = true;
            PrintBatchButton.Visible = true;
            MoveBatchButton.Visible = true;
            OKMoveOrdersButton.Visible = false;
            CancelMoveOrdersButton.Visible = false;
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

        private void RolesAndPermissionsTabControl_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            NeedSplash = false;
        }

        private void EditBatchNameButton_Click(object sender, EventArgs e)
        {
            if (BatchDataGrid.SelectedRows.Count == 1)
            {
                BatchManager.SaveBatch(FactoryID);
            }
        }

        private void OnProductionCheckBox_Profil_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void InProductionCheckBox_Profil_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void NotProductionCheckBox_Profil_CheckedChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void FilterFromDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void FilterToDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (BatchManager == null)
                return;

            Filter();
        }

        private void MegaBatchDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((RoleType == RoleTypes.Admin || RoleType == RoleTypes.Control || RoleType == RoleTypes.Direction)
                && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void SaveMegaBatch_Click(object sender, EventArgs e)
        {
            if (MegaBatchDataGrid.SelectedRows.Count == 1)
            {
                BatchManager.SaveMegaBatch(FactoryID);
            }
        }

        private void GroupsCheckBox_CheckedChanged(object sender, EventArgs e)
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

        private void DecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            BatchExcelReport MarketingBatchReport = new BatchExcelReport();

            MarketingBatchReport.CreateReport(((DataTable)((BindingSource)FrontsDataGrid.DataSource).DataSource).DefaultView.Table,
                ((DataTable)((BindingSource)DecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Недельное планирование. ЗОВ");

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ZOVBatchForm_Load(object sender, EventArgs e)
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
            if (PermissionGranted(iConfirmBatch))
            {
                ConfirmBatch = true;
            }

            //Админ
            if (AddToBatch && InProduction && CloseBatch && CreateBatch && ConfirmBatch)
            {
                RoleType = RoleTypes.Admin;

                cmiCloseProfilBatch.Visible = true;
                cmiCloseTPSBatch.Visible = true;
                cmiCloseProfilMegaBatch.Visible = true;
                cmiCloseTPSMegaBatch.Visible = true;
                cmiConfirmProfilBatch.Visible = true;
                cmiSaveMegaBatch.Visible = true;
            }
            //Управление: нельзя открывать/закрывать партию, остальное - можно
            if (AddToBatch && InProduction && !CloseBatch && CreateBatch && ConfirmBatch)
            {
                RoleType = RoleTypes.Control;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = true;
                cmiSaveMegaBatch.Visible = true;
            }
            //Вход для фа: можно открывать и закрывать партию
            if (!AddToBatch && !InProduction && CloseBatch && !CreateBatch && !ConfirmBatch)
            {
                RoleType = RoleTypes.Direction;

                cmiCloseProfilBatch.Visible = true;
                cmiCloseTPSBatch.Visible = true;
                cmiCloseProfilMegaBatch.Visible = true;
                cmiCloseTPSMegaBatch.Visible = true;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;
                RolesAndPermissionsTabControl.TabPages[1].PageVisible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }
            //вход для пр-ва: можно формировать партию, выдавать в пр-во, нельзя создавать партию и открывать/закрывать её
            if (AddToBatch && InProduction && !CloseBatch && !CreateBatch && ConfirmBatch)
            {
                RoleType = RoleTypes.Production;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = true;
                cmiSaveMegaBatch.Visible = false;

                MoveOrdersButton.Visible = false;
                PickUpFrontsButton.Visible = false;
                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
            }
            //Маркетинг
            if (AddToBatch && !InProduction && !CloseBatch && !CreateBatch && !ConfirmBatch)
            {
                RoleType = RoleTypes.Marketing;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }
            //вход для обычного пользователя: нельзя ничего сделать, только посмотреть партии и их содержимое
            if (!AddToBatch && !InProduction && !CloseBatch && !CreateBatch && !ConfirmBatch)
            {
                RoleType = RoleTypes.Ordinary;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;
                RolesAndPermissionsTabControl.TabPages[1].PageVisible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }
            //Склад
            if (!AddToBatch && !InProduction && !CloseBatch && !CreateBatch && !ConfirmBatch)
            {
                RoleType = RoleTypes.Store;

                cmiCloseProfilBatch.Visible = false;
                cmiCloseTPSBatch.Visible = false;
                cmiCloseProfilMegaBatch.Visible = false;
                cmiCloseTPSMegaBatch.Visible = false;
                cmiConfirmProfilBatch.Visible = false;
                cmiSaveMegaBatch.Visible = false;

                CreateBatchPanel.Visible = false;
                CreateMegaBatchPanel.Visible = false;
                MainOrderInProductionPanel.Visible = false;
                RolesAndPermissionsTabControl.TabPages[1].PageVisible = false;

                xtraTabControl1.Height = BatchPanel.Height - 5;
                xtraTabControl4.Height = MegaBatchPanel.Height - 5;
                BatchMainOrdersDataGrid.Height = MainOrdersPanel.Height - 5;
            }

            cmiCloseTPSBatch.Visible = false;
            cmiCloseTPSMegaBatch.Visible = false;
            cmiConfirmTPSBatch.Visible = false;

            BatchDataGrid.Columns["TPSConfirm"].Visible = false;
            BatchDataGrid.Columns["TPSEnabled"].Visible = false;
            MegaBatchDataGrid.Columns["TPSEnabled"].Visible = false;
            BatchDataGrid.Columns["TPSName"].Visible = false;


        }

        private void MegaBatchDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (MegaBatchDataGrid.Columns[e.ColumnIndex].Name == "ProfilEnabled")
            {
                if (Convert.ToBoolean(MegaBatchDataGrid.Rows[e.RowIndex].Cells["ProfilEnabled"].Value))
                    e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
                else
                    e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            }
            if (MegaBatchDataGrid.Columns[e.ColumnIndex].Name == "TPSEnabled")
            {
                if (Convert.ToBoolean(MegaBatchDataGrid.Rows[e.RowIndex].Cells["TPSEnabled"].Value))
                    e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
                else
                    e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            }
        }

        private void BatchDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (e.ColumnIndex == 4)
            {
                if (Convert.ToBoolean(BatchDataGrid.Rows[e.RowIndex].Cells["ProfilEnabled"].Value))
                    e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
                else
                    e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            }
            if (e.ColumnIndex == 5)
            {
                if (Convert.ToBoolean(BatchDataGrid.Rows[e.RowIndex].Cells["TPSEnabled"].Value))
                    e.Graphics.DrawImage(Unlock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Unlock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Unlock_BW.Height / 2) - 1, Unlock_BW.Width, Unlock_BW.Height);
                else
                    e.Graphics.DrawImage(Lock_BW, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - Lock_BW.Width / 2) - 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - Lock_BW.Height / 2) - 1, Lock_BW.Width, Lock_BW.Height);
            }
        }

        private void BatchDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((RoleType == RoleTypes.Admin || RoleType == RoleTypes.Direction || RoleType == RoleTypes.Control || RoleType == RoleTypes.Production)
                && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MegaBatchProfilAccess_Click(object sender, EventArgs e)
        {
            BatchManager.MegaBatchProfilAccess = !BatchManager.MegaBatchProfilAccess;
            BatchManager.SetBatchEnabled(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]),
                FactoryID, BatchManager.MegaBatchProfilAccess);
            BatchManager.CloseMegaBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID, BatchManager.MegaBatchProfilAccess);
        }

        private void MegaBatchTPSAccess_Click(object sender, EventArgs e)
        {
            BatchManager.MegaBatchTPSAccess = !BatchManager.MegaBatchTPSAccess;
            BatchManager.SetBatchEnabled(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]),
                FactoryID, BatchManager.MegaBatchTPSAccess);
            BatchManager.CloseMegaBatch(Convert.ToInt32(((DataRowView)BatchManager.MegaBatchBindingSource.Current)["MegaBatchID"]), FactoryID, BatchManager.MegaBatchTPSAccess);
        }

        private void BatchTPSAccess_Click(object sender, EventArgs e)
        {
            if (BatchManager.BatchTPSAccess)
            {
                BatchManager.BatchTPSConfirm = true;
                if (BatchManager.BatchTPSConfirmDateTime == DBNull.Value)
                    BatchManager.BatchTPSConfirmDateTime = Security.GetCurrentDate();
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
        }

        private void BatchProfilAccess_Click(object sender, EventArgs e)
        {
            if (BatchManager.BatchProfilAccess)
            {
                BatchManager.BatchProfilConfirm = true;
                if (BatchManager.BatchProfilConfirmDateTime == DBNull.Value)
                    BatchManager.BatchProfilConfirmDateTime = Security.GetCurrentDate();
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
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Production)
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
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Production)
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
                if (BatchManager.MegaBatchProfilAccess)
                    cmiCloseProfilMegaBatch.Text = "Закрыть группу";
                else
                    cmiCloseProfilMegaBatch.Text = "Открыть группу";
            }
            if (FactoryID == 2)
            {
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Direction)
                {
                    cmiCloseProfilMegaBatch.Visible = false;
                    cmiCloseTPSMegaBatch.Visible = true;
                }
                if (BatchManager.MegaBatchTPSAccess)
                    cmiCloseTPSMegaBatch.Text = "Закрыть группу";
                else
                    cmiCloseTPSMegaBatch.Text = "Открыть группу";
            }
        }

        private void PatinaDataGrid_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void PrePatinaDataGrid_SelectionChanged(object sender, EventArgs e)
        {
        }
    }
}
