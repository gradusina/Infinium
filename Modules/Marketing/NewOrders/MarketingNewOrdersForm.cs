using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.Marketing.WeeklyPlanning;

using NPOI.HSSF.UserModel;

using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingNewOrdersForm : Form
    {
        const int iAdmin = 74;
        const int iMarketing = 20;
        const int iDirector = 75;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedRefresh = false;
        bool NeedSplash = false;

        int CurrentRowIndex = -1;
        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;
        AddMarketingNewOrdersForm AddMainOrdersForm;
        CurrencyForm CurrencyForm;
        SaveDBFReportMenu SaveDBFReportMenu;
        ClientReportMenu ClientReportMenu;
        Report Report;
        DetailsReport DetailsReport;
        Modules.Marketing.NewOrders.PrepareReport.DetailsReport PrepareReport;
        SendEmail SendEmail;
        Infinium.Modules.TechnologyCatalog.TestTechCatalog TestTechCatalogManager;
        CreateOrdersFromExcel CreateOrdersFromExcel;

        DataTable RolePermissionsDataTable;

        public OrdersManager OrdersManager;
        public DecorCatalogOrder DecorCatalogOrder;
        public OrdersCalculate OrdersCalculate;

        RoleTypes RoleType = RoleTypes.Ordinary;
        public enum RoleTypes
        {
            Ordinary = 0,
            Admin = 1,
            Marketing = 2,
            Director = 3
        }

        public MarketingNewOrdersForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            RolePermissionsDataTable = OrdersManager.GetPermissions(Security.CurrentUserID, this.Name);
            btnDeleteAgreedOrder.Visible = false;
            btnSetAgreementStatus.Visible = false;
            btnDeleteAgreedMainOrder.Visible = false;
            btnEditAgreedMainOrder.Visible = false;
            kryptonContextMenuItem25.Visible = false;
            kryptonContextMenuItem27.Visible = false;
            if (!PermissionGranted(iMarketing) && !PermissionGranted(iDirector) && !PermissionGranted(iAdmin))
            {
                tableLayoutPanel1.Height = this.Height - NavigatePanel.Height - 10;
                ToolsPanel.Visible = false;
            }
            if (PermissionGranted(iMarketing))
            {
                RoleType = RoleTypes.Marketing;
            }
            if (PermissionGranted(iAdmin))
            {
                btnDeleteAgreedMainOrder.Visible = true;
                btnEditAgreedMainOrder.Visible = true;
                btnDeleteAgreedOrder.Visible = true;
                btnSetAgreementStatus.Visible = true;
                kryptonContextMenuItem25.Visible = true;
                kryptonContextMenuItem27.Visible = true;
                RoleType = RoleTypes.Admin;
            }
            if (PermissionGranted(iDirector))
            {
                btnDeleteAgreedMainOrder.Visible = true;
                btnEditAgreedMainOrder.Visible = true;
                btnDeleteAgreedOrder.Visible = true;
                btnSetAgreementStatus.Visible = true;
                kryptonContextMenuItem25.Visible = true;
                kryptonContextMenuItem27.Visible = true;
                RoleType = RoleTypes.Director;
            }

            while (!SplashForm.bCreated) ;
        }

        //private bool PermissionGranted(int PermissionID)
        //{
        //    DataRow[] Rows = RolePermissionsDataTable.Select("PermissionID = " + PermissionID);

        //    if (Rows.Count() > 0)
        //    {
        //        return Convert.ToBoolean(Rows[0]["Granted"]);
        //    }

        //    return false;
        //}

        private bool PermissionGranted(int RoleID)
        {
            DataRow[] Rows = RolePermissionsDataTable.Select("RoleID = " + RoleID);

            return Rows.Count() > 0;
        }

        private void MarketingOrdersForm_Shown(object sender, EventArgs e)
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

        private void Initialize()
        {

            CreateOrdersFromExcel = new CreateOrdersFromExcel();

            DecorCatalogOrder = new DecorCatalogOrder();

            OrdersManager = new OrdersManager(ref MainOrdersDataGrid, ref MainOrdersFrontsOrdersDataGrid, ref MegaOrdersDataGrid,
                ref MainOrdersDecorTabControl, ref MainOrdersTabControl, ref DecorCatalogOrder);
            OrdersCalculate = new OrdersCalculate();
            Report = new Report(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);
            DetailsReport = new DetailsReport(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);
            PrepareReport = new Modules.Marketing.NewOrders.PrepareReport.DetailsReport(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);
            SendEmail = new SendEmail();

            FilterClientsDataGrid.DataSource = OrdersManager.FilterClientsBindingSource;
            FilterClientsDataGrid.Columns["ClientID"].Visible = false;

            GetCurrency(CurrencyDateTimePicker.Value.Date);

            OrdersManager.GetOrdersInMuttlements(
                true,
                true,
                true,
                true,
                true, true, true, true, true, true);
            OrdersManager.FilterMegaOrders(
                false, 0, true, true, true, true,
                true, true, true, true, true, false, true, true, true, true, true, true);
        }

        private void MegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (MegaOrdersDataGrid.Focused)
            {

            }
            if (OrdersManager != null)
                if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        bool OnProduction = OnProductionCheckBox.Checked;
                        bool NotInProduction = NotProductionCheckBox.Checked;
                        bool InProduction = InProductionCheckBox.Checked;
                        bool OnStorage = OnStorageCheckBox.Checked;
                        bool OnExpedition = cbOnExpedition.Checked;
                        bool Dispatch = DispatchCheckBox.Checked;

                        if (NeedSplash)
                        {
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;
                            NeedSplash = false;

                            OrdersManager.FilterMainOrdersByMegaOrder(
                                Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                OnProduction,
                                NotInProduction,
                                InProduction,
                                OnStorage,
                                OnExpedition,
                                Dispatch);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                            OrdersManager.FilterMainOrdersByMegaOrder(
                                Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                OnProduction,
                                NotInProduction,
                                InProduction,
                                OnStorage,
                                OnExpedition,
                                Dispatch);

                    }
                }
                else
                    OrdersManager.MainOrdersDataTable.Clear();
        }

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            //MainOrdersSquareLabel.Text = string.Empty;
            if (MainOrdersDataGrid.Focused)
            {

            }
            if (OrdersManager != null)
                if (OrdersManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        OrdersManager.FilterProductByMainOrder(Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]));
                        if (MainOrdersTabControl.TabPages[0].PageVisible && MainOrdersTabControl.TabPages[1].PageVisible)
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];
                        OrdersManager.MainOrdersDecorOrders.ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["ClientID"]);
                        //decimal Square = OrdersManager.GetSelectedMainOrdersSquare();
                        //if (Square > 0)
                        //    MainOrdersSquareLabel.Text = "Площадь выбранных подзаказов: " + Square + " м.кв";
                    }
                }
                else
                {
                    OrdersManager.FilterProductByMainOrder(-1);
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

        private void MainOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MainOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void Filter()
        {
            bool bClient = ClientCheckBox.Checked;
            bool NotAgreed = NotAgreedCheckBox.Checked;
            bool OnAgreement = OnAgreementCheckBox.Checked;
            bool NotConfirm = NotConfirmCheckBox.Checked;
            bool Confirm = ConfirmCheckBox.Checked;
            bool OnProduction = OnProductionCheckBox.Checked;
            bool NotInProduction = NotProductionCheckBox.Checked;
            bool InProduction = InProductionCheckBox.Checked;
            bool OnStorage = OnStorageCheckBox.Checked;
            bool OnExpedition = cbOnExpedition.Checked;
            bool Dispatch = DispatchCheckBox.Checked;
            bool bsDelayOfPayment = cbDelayOfPayment.Checked;
            bool bsHalfOfPayment = cbHalfOfPayment.Checked;
            bool bsFullPayment = cbFullPayment.Checked;
            bool bsFactoring = cbFactoring.Checked;
            bool bsHalfOfPayment2 = kryptonCheckBox1.Checked;
            bool bsDelayOfPayment2 = kryptonCheckBox2.Checked;

            int ClientID = -1;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                if (bClient && FilterClientsDataGrid.SelectedRows.Count > 0)
                    ClientID = Convert.ToInt32(FilterClientsDataGrid.SelectedRows[0].Cells["ClientID"].Value);

                OrdersManager.GetOrdersInMuttlements(
                    NotAgreed,
                    OnAgreement,
                    NotConfirm,
                    Confirm,
                    bsDelayOfPayment, bsHalfOfPayment, bsFullPayment, bsFactoring, bsHalfOfPayment2, bsDelayOfPayment2);
                OrdersManager.FilterMegaOrders(
                    bClient, ClientID,
                    NotAgreed,
                    OnAgreement,
                    NotConfirm,
                    Confirm,
                    OnProduction,
                    NotInProduction,
                    InProduction,
                    OnStorage,
                    OnExpedition,
                    Dispatch, bsDelayOfPayment, bsHalfOfPayment, bsFullPayment, bsFactoring, bsHalfOfPayment2, bsDelayOfPayment2);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {

                if (bClient && FilterClientsDataGrid.SelectedRows.Count > 0)
                    ClientID = Convert.ToInt32(FilterClientsDataGrid.SelectedRows[0].Cells["ClientID"].Value);

                OrdersManager.GetOrdersInMuttlements(
                    NotAgreed,
                    OnAgreement,
                    NotConfirm,
                    Confirm,
                    bsDelayOfPayment, bsHalfOfPayment, bsFullPayment, bsFactoring, bsHalfOfPayment2, bsDelayOfPayment2);
                OrdersManager.FilterMegaOrders(
                    bClient, ClientID,
                    NotAgreed,
                    OnAgreement,
                    NotConfirm,
                    Confirm,
                    OnProduction,
                    NotInProduction,
                    InProduction,
                    OnStorage,
                    OnExpedition,
                    Dispatch, bsDelayOfPayment, bsHalfOfPayment, bsFullPayment, bsFactoring, bsHalfOfPayment2, bsDelayOfPayment2);
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

        private void NotAgreedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void NotConfirmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void ConfirmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void OnProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void NotProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void InProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void OnStorageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void DispatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void ClientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FilterClientsDataGrid.Enabled = ClientCheckBox.Checked;

            if (OrdersManager == null)
                return; Filter();
        }

        private void FilterClientsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (OrdersManager != null && ClientCheckBox.Checked)
                Filter();
        }

        private void NewMainOrderButton_Click(object sender, EventArgs e)
        {
            int MegaOrderID = OrdersManager.CurrentMegaOrderID;
            if (MegaOrderID < 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Для добавления подзаказа сначала нужно создать новый заказ",
                       "Добавление подзаказа");
                return;
            }

            if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["AgreementStatusID"]) == 2)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказ был согласован, добавление подзаказов запрещено",
                       "Добавление подзаказа");
                return;
            }

            if (!OrdersManager.CanAddMainOrderToMegaOrder())
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказ был отдан в производство или уже отгружен, добавление подзаказов запрещено",
                       "Добавление подзаказа");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                OrdersManager.CurrentClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
            AddMainOrdersForm = new AddMarketingNewOrdersForm(ref OrdersManager, false, false, ref TopForm, ref OrdersCalculate);

            TopForm = AddMainOrdersForm;

            AddMainOrdersForm.ShowDialog();

            AddMainOrdersForm.Close();
            AddMainOrdersForm.Dispose();

            TopForm = null;

            bool OnProduction = OnProductionCheckBox.Checked;
            bool NotInProduction = NotProductionCheckBox.Checked;
            bool InProduction = InProductionCheckBox.Checked;
            bool OnStorage = OnStorageCheckBox.Checked;
            bool OnExpedition = cbOnExpedition.Checked;
            bool Dispatch = DispatchCheckBox.Checked;

            NeedSplash = false;

            Filter();

            OrdersManager.UpdateMainOrders(
                OnProduction,
                NotInProduction,
                InProduction,
                OnStorage,
                OnExpedition,
                Dispatch);
            NeedSplash = true;
        }

        private void MainOrdersEditOrder_Click(object sender, EventArgs e)
        {
            OrdersManager.NeedSetStatus = true;
            if (OrdersManager.MainOrdersBindingSource.Count > 0)
            {
                NeedSplash = false;

                bool IsSample = false;
                string Notes = string.Empty;
                //получение значений параметров заказа, если заблокирован - выход
                if (!OrdersManager.EditMainOrder(ref Notes, ref IsSample))
                    return;

                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;

                if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                {
                    OrdersManager.CurrentClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
                    OrdersManager.CurrentDiscountPaymentConditionID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountPaymentConditionID"]);
                    OrdersManager.CurrentDiscountFactoringID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountFactoringID"]);
                    OrdersManager.CurrentProfilDiscountDirector = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilDiscountDirector"]);
                    OrdersManager.CurrentTPSDiscountDirector = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSDiscountDirector"]);
                    OrdersManager.CurrentProfilTotalDiscount = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilTotalDiscount"]) - Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilDiscountDirector"]);
                    OrdersManager.CurrentTPSTotalDiscount = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSTotalDiscount"]) - Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSDiscountDirector"]);
                    OrdersManager.CurrencyTypeID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]);
                    OrdersManager.PaymentCurrency = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]);
                    OrdersManager.ConfirmDateTime = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ConfirmDateTime"];
                }
                AddMainOrdersForm = new AddMarketingNewOrdersForm(ref OrdersManager, true, false, ref TopForm, ref OrdersCalculate);

                TopForm = AddMainOrdersForm;
                AddMainOrdersForm.ShowDialog();

                AddMainOrdersForm.Close();
                AddMainOrdersForm.Dispose();

                TopForm = null;

                bool OnProduction = OnProductionCheckBox.Checked;
                bool NotInProduction = NotProductionCheckBox.Checked;
                bool InProduction = InProductionCheckBox.Checked;
                bool OnStorage = OnStorageCheckBox.Checked;
                bool OnExpedition = cbOnExpedition.Checked;
                bool Dispatch = DispatchCheckBox.Checked;

                NeedSplash = false;
                Filter();

                OrdersManager.UpdateMainOrders(
                    OnProduction,
                    NotInProduction,
                    InProduction,
                    OnStorage,
                    OnExpedition,
                    Dispatch);
                NeedSplash = true;
            }
            OrdersManager.CurrentClientID = -1;
            OrdersManager.CurrentDiscountPaymentConditionID = 0;
            OrdersManager.CurrentDiscountFactoringID = 0;
            OrdersManager.CurrentProfilDiscountDirector = 0;
            OrdersManager.CurrentTPSDiscountDirector = 0;
            OrdersManager.CurrentProfilTotalDiscount = 0;
            OrdersManager.CurrentTPSTotalDiscount = 0;
            OrdersManager.PaymentCurrency = 1;
            OrdersManager.CurrencyTypeID = 1;
            OrdersManager.ConfirmDateTime = DBNull.Value;
        }

        private void MainOrdersRemoveMainOrder_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MainOrdersBindingSource.Count == 0)
                return;

            string MainOrderID = ((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"].ToString();
            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            //DialogResult result = MessageBox.Show(this, "Вы действительно хотите удалить заказ " + MainOrderID + " ?",
            //                "Удаление заказа", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if (OrdersManager.GetAgreementStatus() > 1)
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //           "Невозможно удалить подзаказ, так как он уже согласован",
            //           "Удаление подзаказа");
            //    NeedSplash = true;

            //    return;
            //}

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы действительно хотите удалить подзаказ " + MainOrderID + " ?",
                    "Удаление подзаказа");
            if (!OKCancel)
                return;

            NeedSplash = false;

            bool IsSample = false;
            string Notes = string.Empty;
            //получение значений параметров заказа, если заблокирован - выход
            if (!OrdersManager.EditMainOrder(ref Notes, ref IsSample))
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            OrdersManager.RemoveCurrentMainOrder();

            bool OnProduction = OnProductionCheckBox.Checked;
            bool NotInProduction = NotProductionCheckBox.Checked;
            bool InProduction = InProductionCheckBox.Checked;
            bool OnStorage = OnStorageCheckBox.Checked;
            bool OnExpedition = cbOnExpedition.Checked;
            bool Dispatch = DispatchCheckBox.Checked;

            if (OrdersManager.MainOrdersBindingSource.Count > 0)
                OrdersManager.FilterMainOrdersByMegaOrder(
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]),
                    OnProduction,
                    NotInProduction,
                    InProduction,
                    OnStorage,
                    OnExpedition,
                    true);

            OrdersManager.SetNotAgreed(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]));

            OrdersManager.SummaryCost(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]));
            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Удален подзаказ");
            Filter();
            OrdersManager.MoveToMegaOrder(MegaOrderID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void MainOrdersRemoveMegaOrder_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                int ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["ClientID"]);
                int OrderNumber = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["OrderNumber"]);
                int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]);

                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Вы действительно хотите удалить заказ " + MegaOrderID + " ?",
                        "Удаление заказа");
                if (!OKCancel)
                    return;

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                OrdersManager.RemoveCurrentMegaOrder();
                OrdersManager.FixOrderEvent(MegaOrderID, "Заказ удален");

                if (OrdersManager.MegaOrdersCount == 0)
                {
                    OrdersManager.CurrentMegaOrderID = -1;
                }
                Filter();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void MainOrdersAcceptOrder_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            //{
            //    int MegaOrderID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value);
            //    int AgreementStatusID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["AgreementStatusID"].Value);
            //    if (AgreementStatusID == 2)
            //        OrdersManager.MoveOrdersTo(MegaOrderID, AgreementStatusID);
            //}
            //return;
            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                OrdersManager.MainOrdersDecorOrders.AddMegaOrderToDecorAssignment(
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]));

                if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 2)
                {
                    OrdersManager.MoveOrdersTo1(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]));
                    //OrdersManager.MoveOrdersTo(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]));
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Заказ уже согласован",
                           "Согласование заказа");
                    return;
                }

                NeedSplash = false;
                if (!OrdersManager.CanBeAccepted())
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Проверьте статус заказа. До подтверждения статус заказа должен быть \"Не подтвержден\"",
                           "Согласование заказа");
                    NeedSplash = true;
                    return;
                }

                int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Подтверждать заказ должен клиент. Вы уверены, что хотите подтвердить заказ?",
                        "Согласование заказа");
                int ClientID = 0;
                string ClientName = string.Empty;
                InvoiceReportToDBF DBFReport = null;
                PhantomForm PhantomForm = null;
                if (OKCancel)
                {
                    PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();

                    ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
                    ClientName = OrdersManager.GetClientName(ClientID);
                    bool bCanDirectorDiscount = false;
                    if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Director)
                        bCanDirectorDiscount = true;

                    DBFReport = new InvoiceReportToDBF();
                    CurrencyForm = new CurrencyForm(this, ref OrdersManager, ref OrdersCalculate, ref DBFReport, ClientID, ClientName, bCanDirectorDiscount);

                    CurrencyForm.SetParameters(
                            Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TransportType"]),
                            Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DelayOfPayment"]),
                            ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DesireDate"],
                            ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ConfirmDateTime"],
                            Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["Rate"]),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintProfilCost"]),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintTPSCost"]),
                            ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintNotes"].ToString(),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TransportCost"]),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["AdditionalCost"]),
                            Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountPaymentConditionID"]),
                            Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountFactoringID"]),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilDiscountDirector"]),
                            Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSDiscountDirector"]),
                            Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["Weight"]));

                    TopForm = CurrencyForm;
                    CurrencyForm.ShowDialog();

                    PhantomForm.Close();

                    PhantomForm.Dispose();
                    CurrencyForm.Dispose();
                    TopForm = null;

                    OrdersManager.AcceptOrder();
                    OrdersManager.MoveOrdersTo(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                        Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]));
                    OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Заказ согласован");
                }
                else
                    return;
                if (OrdersManager.SendReport == false)
                    return;

                NeedSplash = false;
                Filter();
                OrdersManager.MoveToMegaOrder(MegaOrderID);
                NeedSplash = true;
            }
        }

        private void NewMegaOrderButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = 0;
            bool FromExcel = false;

            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
            }

            NewOrderSelectClientsMenu NewOrderSelectMenu = new NewOrderSelectClientsMenu(this, ClientID);

            TopForm = NewOrderSelectMenu;
            NewOrderSelectMenu.ShowDialog();
            ClientID = NewOrderSelectMenu.ClientID;
            FromExcel = NewOrderSelectMenu.FromExcel;

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewOrderSelectMenu.Dispose();
            TopForm = null;

            if (ClientID > 0)
            {
                if (FromExcel)
                {
                    CreateOrdersFromExcel.GetDataFromExcel();
                    CreateOrdersFromExcel.CreateOrders(ClientID);
                }
                else
                    OrdersManager.CreateNewMegaOrder(ClientID);

                NeedSplash = false;
                Filter();
                NeedSplash = true;
            }
        }

        private void MainOrdersCalculateOrder_Click(object sender, EventArgs e)
        {
            //asdf();
            //Filter();
            //return;

            if (OrdersManager.MegaOrdersBindingSource.Count == 0)
                return;

            //if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 2)
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //           "Заказ уже согласован",
            //           "Расчет заказа");
            //    return;
            //}

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            int ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
            string ClientName = OrdersManager.GetClientName(ClientID);
            bool bCanDirectorDiscount = false;
            if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Director)
                bCanDirectorDiscount = true;

            InvoiceReportToDBF DBFReport = new InvoiceReportToDBF();
            CurrencyForm = new CurrencyForm(this, ref OrdersManager, ref OrdersCalculate, ref DBFReport, ClientID, ClientName, bCanDirectorDiscount);

            CurrencyForm.SetParameters(
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TransportType"]),
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DelayOfPayment"]),
                    ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DesireDate"],
                    ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ConfirmDateTime"],
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["Rate"]),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintProfilCost"]),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintTPSCost"]),
                    ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintNotes"].ToString(),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TransportCost"]),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["AdditionalCost"]),
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountPaymentConditionID"]),
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountFactoringID"]),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilDiscountDirector"]),
                    Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSDiscountDirector"]),
                            Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["Weight"]));

            TopForm = CurrencyForm;
            CurrencyForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            CurrencyForm.Dispose();
            TopForm = null;

            NeedSplash = false;
            Filter();
            OrdersManager.MoveToMegaOrder(MegaOrderID);
            NeedSplash = true;

            ////REPORT
            DetailsReport.Save = false;
            DetailsReport.Send = false;

            PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ClientReportMenu = new Infinium.ClientReportMenu(this);

            TopForm = ClientReportMenu;
            ClientReportMenu.ShowDialog();

            PhantomForm.Close();
            DetailsReport.Save = ClientReportMenu.Save;
            DetailsReport.Send = ClientReportMenu.Send;

            PhantomForm.Dispose();
            ClientReportMenu.Dispose();
            TopForm = null;

            if (DetailsReport.Save || DetailsReport.Send)
            {
                decimal ComplaintProfilCost = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintProfilCost"]);
                decimal ComplaintTPSCost = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintTPSCost"]);
                decimal TotalWeight = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["Weight"]);
                decimal TransportCost = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTransportCost"]);
                decimal AdditionalCost = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyAdditionalCost"]);
                decimal TotalCost = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTotalCost"]);
                int CurrencyTypeID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]);
                decimal Rate = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]);
                int OrderNumber = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["OrderNumber"]);

                int[] CheckedMegaOrders = new int[1] { Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]) };
                int[] CheckedOrderNumbers = new int[1] { OrderNumber };
                int[] MainOrders = OrdersManager.GetMainOrders();

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                if (ClientID == 145)
                {
                    bool HasOrdinaryOrders = OrdersManager.HasOrders(MainOrders, false);
                    bool HasSampleOrders = OrdersManager.HasOrders(MainOrders, true);
                    if (HasOrdinaryOrders)
                    {
                        string FileName = DetailsReport.Report(CheckedMegaOrders, CheckedOrderNumbers, MainOrders, ClientID, ClientName,
                            ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, false);

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        if (DetailsReport.Send)
                        {
                            NeedSplash = false;

                            T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Отправка письма.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            SendEmail.Send(ClientID, string.Join(",", CheckedOrderNumbers), DetailsReport.Save, FileName);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;

                            if (SendEmail.Success == false)
                            {
                                Infinium.LightMessageBox.Show(ref TopForm, false,
                                       "Отправка отчета невозможна: отсутствует подключение к интернету либо адрес электронной почты указан неверно",
                                       "Отправка отчета");
                            }
                            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Отчет отправлен клиенту");
                            //SendEmail.DeleteFile(FileName);
                        }
                    }
                    if (HasSampleOrders)
                    {
                        string FileName = DetailsReport.Report(CheckedMegaOrders, CheckedOrderNumbers, MainOrders, ClientID, ClientName,
                            ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, true);

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        if (DetailsReport.Send)
                        {
                            NeedSplash = false;

                            T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Отправка письма.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            SendEmail.Send(ClientID, string.Join(",", CheckedOrderNumbers), DetailsReport.Save, FileName);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;

                            if (SendEmail.Success == false)
                            {
                                Infinium.LightMessageBox.Show(ref TopForm, false,
                                       "Отправка отчета невозможна: отсутствует подключение к интернету либо адрес электронной почты указан неверно",
                                       "Отправка отчета");
                            }
                            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]),
                                "Отчет отправлен клиенту");
                        }
                    }
                }
                else
                {

                    //MessageBox.Show("Выполняется функция DetailsReport.Report");
                    string FileName = DetailsReport.Report(CheckedMegaOrders, CheckedOrderNumbers, MainOrders, ClientID, ClientName,
                         ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                         OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight);

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;

                    //MessageBox.Show("Выполнена функция DetailsReport.Report");
                    NeedSplash = true;

                    if (DetailsReport.Send)
                    {
                        NeedSplash = false;

                        T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Отправка письма.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        SendEmail.Send(ClientID, string.Join(",", CheckedOrderNumbers), DetailsReport.Save, FileName);

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        if (SendEmail.Success == false)
                        {
                            Infinium.LightMessageBox.Show(ref TopForm, false,
                                   "Отправка отчета невозможна: отсутствует подключение к интернету либо адрес электронной почты указан неверно",
                                   "Отправка отчета");
                        }
                        OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Отчет отправлен клиенту");
                        //SendEmail.DeleteFile(FileName);
                    }
                }
            }
        }

        public void asdf()
        {
            if (OrdersManager.MegaOrdersBindingSource.Count == 0)
                return;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            InvoiceReportToDBF DBFReport = new InvoiceReportToDBF();
            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                int MegaOrderID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value);
                int ClientID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["ClientID"].Value);
                string ClientName = OrdersManager.GetClientName(ClientID);

                bool bCanDirectorDiscount = false;
                if (RoleType == RoleTypes.Admin || RoleType == RoleTypes.Director)
                    bCanDirectorDiscount = true;

                CurrencyForm = new CurrencyForm(this, ref OrdersManager, ref OrdersCalculate, ref DBFReport, ClientID, ClientName, bCanDirectorDiscount);

                CurrencyForm.SetParameters(
                    Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["TransportType"].Value),
                    Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["DelayOfPayment"].Value),
                    MegaOrdersDataGrid.SelectedRows[i].Cells["DesireDate"].Value,
                    MegaOrdersDataGrid.SelectedRows[i].Cells["ConfirmDateTime"].Value,
                    Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyTypeID"].Value),

                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["Rate"].Value),
                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["PaymentRate"].Value),
                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["ComplaintProfilCost"].Value),
                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["ComplaintTPSCost"].Value),

                    MegaOrdersDataGrid.SelectedRows[i].Cells["ComplaintNotes"].Value.ToString(),

                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["TransportCost"].Value),
                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["AdditionalCost"].Value),

                    Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["DiscountPaymentConditionID"].Value),
                    Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["DiscountFactoringID"].Value),

                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["ProfilDiscountDirector"].Value),
                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["TPSDiscountDirector"].Value),

                    Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["Weight"].Value));

                TopForm = CurrencyForm;
                CurrencyForm.ShowDialog();

            }

            PhantomForm.Close();

            PhantomForm.Dispose();
            CurrencyForm.Dispose();
            TopForm = null;
        }

        private void CreateReportButton_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MainOrdersBindingSource.Count == 0)
                return;
            int CurrencyTypeID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value);
            int DiscountPaymentConditionID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["DiscountPaymentConditionID"].Value);
            int DelayOfPayment = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["DelayOfPayment"].Value);
            if (MegaOrdersDataGrid.SelectedRows.Count > 0)
            {
                int d = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["DiscountPaymentConditionID"].Value);
                for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                {
                    if (d != Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["DiscountPaymentConditionID"].Value))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false,
                               "Нельзя объединить заказы с различными условиями оплаты",
                               "Создание отчета");
                        return;
                    }
                }
            }
            if (MegaOrdersDataGrid.SelectedRows.Count > 0)
            {
                int c = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value);
                for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                {
                    if (c != Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyTypeID"].Value))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false,
                               "Нельзя объединить заказы с различными валютами",
                               "Создание отчета");
                        return;
                    }
                }
            }
            int[] SelectedMegaOrders = OrdersManager.GetSelectedMegaOrders();

            if (!OrdersManager.AreSelectedMegaOrdersOneClient)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Выбраны заказы разных клиентов",
                       "Создание отчета");
                return;
            }

            if (!OrdersManager.AreSelectedMegaOrdersAgree(SelectedMegaOrders))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Выбранные заказы несогласованы",
                       "Создание отчета");
                return;
            }

            int[] MainOrders = OrdersManager.GetMainOrders(SelectedMegaOrders);

            decimal ComplaintProfilCost = 0;
            decimal ComplaintTPSCost = 0;
            decimal TransportCost = 0;
            decimal AdditionalCost = 0;
            decimal TotalWeight = 0;
            CurrencyTypeID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]);
            decimal Rate = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]);
            int OrderNumber = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["OrderNumber"]);

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                ComplaintProfilCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyComplaintProfilCost"].Value);
                ComplaintTPSCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyComplaintTPSCost"].Value);
                TotalWeight += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["Weight"].Value);
                TransportCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyTransportCost"].Value);
                AdditionalCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyAdditionalCost"].Value);
            }

            if (!OrdersManager.CanBeReported())
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказ не был согласован. Создание отчета невозможно.",
                       "Создание отчета");
                return;
            }

            int[] SelectedOrderNumbers = OrdersManager.GetSelectedOrderNumbers();
            int[] SelectedProfilOrderNumbers = new int[MegaOrdersDataGrid.SelectedRows.Count];
            int[] SelectedTPSOrderNumbers = new int[MegaOrdersDataGrid.SelectedRows.Count];

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 1 || Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 0)
                    SelectedProfilOrderNumbers[i] = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["OrderNumber"].Value);
                if (Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 2 || Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 0)
                    SelectedTPSOrderNumbers[i] = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["OrderNumber"].Value);
            }
            SelectedProfilOrderNumbers = SelectedProfilOrderNumbers.Where(val => val != 0).ToArray();
            SelectedTPSOrderNumbers = SelectedTPSOrderNumbers.Where(val => val != 0).ToArray();
            int ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
            string ClientName = OrdersManager.GetClientName(ClientID);

            NeedSplash = false;

            InvoiceReportToDBF DBFReport = new InvoiceReportToDBF();

            Infinium.Modules.Marketing.MutualSettlements.MutualSettlements MutualSettlementsManager = new Modules.Marketing.MutualSettlements.MutualSettlements();
            MutualSettlementsManager.Initialize();

            if (ClientID == 145)
            {
                bool HasOrdinaryOrders = OrdersManager.HasOrders(MainOrders, false);
                bool HasSampleOrders = OrdersManager.HasOrders(MainOrders, true);

                if (HasOrdinaryOrders)
                {
                    decimal TotalProfil = 0;
                    decimal TotalTPS = 0;

                    string ProfilDBFName = string.Empty;
                    string TPSDBFName = string.Empty;
                    string ReportName = string.Empty;
                    int ClientCountryID = OrdersManager.GetClientCountry(ClientID);

                    ClientName = ClientName.Replace("\"", " ");
                    ClientName = ClientName.Replace("*", " ");
                    ClientName = ClientName.Replace("|", " ");
                    ClientName = ClientName.Replace(@"\", " ");
                    ClientName = ClientName.Replace(":", " ");
                    ClientName = ClientName.Replace("<", " ");
                    ClientName = ClientName.Replace(">", " ");
                    ClientName = ClientName.Replace("?", " ");
                    ClientName = ClientName.Replace("/", " ");
                    ReportName = ClientName + " №" + string.Join(",", SelectedOrderNumbers);

                    PhantomForm PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();
                    bool InMutualSettlement = false;
                    int Result = 1;
                    string Notes = string.Empty;
                    string SaveFilePath = string.Empty;
                    SaveDBFReportMenu = new Infinium.SaveDBFReportMenu(this, ReportName);

                    TopForm = SaveDBFReportMenu;
                    SaveDBFReportMenu.ShowDialog();

                    PhantomForm.Close();
                    InMutualSettlement = SaveDBFReportMenu.InMutualSettlement;
                    Result = SaveDBFReportMenu.Result;
                    Notes = SaveDBFReportMenu.Notes;
                    SaveFilePath = SaveDBFReportMenu.SaveFilePath;
                    PhantomForm.Dispose();
                    SaveDBFReportMenu.Dispose();
                    TopForm = null;

                    if (Result != 3)
                    {
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                        DBFReport.CreateReport(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders,
                            ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, false, ref TotalProfil, ref TotalTPS);
                        DBFReport.SaveDBF(SaveFilePath, ReportName, false, ref ProfilDBFName, ref TPSDBFName);

                        DetailsReport.Report(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                            ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, false);

                        FileInfo file = new FileInfo(SaveFilePath + @"\" + ReportName + ".xls");
                        int j = 1;
                        while (file.Exists == true)
                        {
                            file = new FileInfo(SaveFilePath + @"\" + ReportName + "(" + j++ + ").xls");
                        }

                        FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                        hssfworkbook.Write(NewFile);
                        NewFile.Close();

                        if (InMutualSettlement)
                        {
                            int MutualSettlementID = -1;
                            if (TotalProfil > 0)
                            {
                                if (ClientCountryID == 1)
                                    TotalProfil *= 1.2m;
                                TotalProfil = Decimal.Round(TotalProfil, 2, MidpointRounding.AwayFromZero);
                                MutualSettlementID = MutualSettlementsManager.AddMutualSettlement(ClientID, 1, CurrencyTypeID, DiscountPaymentConditionID, DelayOfPayment, TotalProfil, Notes);
                                var fileInfo = new System.IO.FileInfo(file.FullName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(file.FullName),
                                    fileInfo.Extension, file.FullName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceExcel, MutualSettlementID);
                                fileInfo = new System.IO.FileInfo(ProfilDBFName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(ProfilDBFName),
                                    fileInfo.Extension, ProfilDBFName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceDbf, MutualSettlementID);
                                DateTime CurrentDate = Security.GetCurrentDate();
                                for (int i = 0; i < SelectedProfilOrderNumbers.Count(); i++)
                                    MutualSettlementsManager.AddMutualSettlementOrder(CurrentDate, ClientID, MutualSettlementID, SelectedProfilOrderNumbers[i]);
                                //MutualSettlementsManager.AddSubscribesRecord(404, MutualSettlementID);
                            }
                            if (TotalTPS > 0)
                            {
                                if (ClientCountryID == 1)
                                    TotalTPS *= 1.2m;
                                TotalTPS = Decimal.Round(TotalTPS, 2, MidpointRounding.AwayFromZero);
                                MutualSettlementID = MutualSettlementsManager.AddMutualSettlement(ClientID, 2, CurrencyTypeID, DiscountPaymentConditionID, DelayOfPayment, TotalTPS, Notes);
                                var fileInfo = new System.IO.FileInfo(file.FullName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(file.FullName),
                                    fileInfo.Extension, file.FullName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceExcel, MutualSettlementID);
                                fileInfo = new System.IO.FileInfo(TPSDBFName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(TPSDBFName),
                                    fileInfo.Extension, TPSDBFName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceDbf, MutualSettlementID);
                                DateTime CurrentDate = Security.GetCurrentDate();
                                for (int i = 0; i < SelectedTPSOrderNumbers.Count(); i++)
                                    MutualSettlementsManager.AddMutualSettlementOrder(CurrentDate, ClientID, MutualSettlementID, SelectedTPSOrderNumbers[i]);
                                MutualSettlementsManager.AddSubscribesRecord(324, MutualSettlementID);
                            }
                            MutualSettlementsManager.SaveMutualSettlements();
                            MutualSettlementsManager.SaveMutualSettlementOrders();
                            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Счет отправлен на бухгалтерию");
                        }

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        DetailsReport.Save = false;
                        DetailsReport.Send = false;

                        PhantomForm = new Infinium.PhantomForm();
                        PhantomForm.Show();

                        ClientReportMenu = new ClientReportMenu(this);
                        TopForm = ClientReportMenu;

                        ClientReportMenu.ShowDialog();

                        PhantomForm.Close();
                        DetailsReport.Save = ClientReportMenu.Save;
                        DetailsReport.Send = ClientReportMenu.Send;

                        PhantomForm.Dispose();
                        ClientReportMenu.Dispose();
                        TopForm = null;

                        System.Diagnostics.Process.Start(file.FullName);
                        ReportName = file.FullName;

                        if (DetailsReport.Save || DetailsReport.Send)
                        {
                            //REPORT
                            T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            string FileName = DetailsReport.Report(SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                                ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                                OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, false);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;

                            if (DetailsReport.Send)
                            {
                                //SEND
                                T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Отправка письма.\r\nПодождите..."); });
                                T.Start();
                                while (!SplashWindow.bSmallCreated) ;

                                SendEmail.Send(ClientID, string.Join(",", SelectedOrderNumbers), DetailsReport.Save, FileName);

                                while (SplashWindow.bSmallCreated)
                                    SmallWaitForm.CloseS = true;
                                NeedSplash = true;

                                if (SendEmail.Success == false)
                                {
                                    Infinium.LightMessageBox.Show(ref TopForm, false,
                                           "Отправка отчета невозможна: отсутствует подключение к интернету либо адрес электронной почты указан неверно",
                                           "Отправка письма");
                                }
                                OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Отчет отправлен клиенту");
                                //SendEmail.DeleteFile(FileName);
                            }
                        }
                    }
                }
                if (HasSampleOrders)
                {
                    decimal TotalProfil = 0;
                    decimal TotalTPS = 0;

                    string ProfilDBFName = string.Empty;
                    string TPSDBFName = string.Empty;
                    string ReportName = string.Empty;
                    int ClientCountryID = OrdersManager.GetClientCountry(ClientID);

                    ClientName = ClientName.Replace("\"", " ");
                    ClientName = ClientName.Replace("*", " ");
                    ClientName = ClientName.Replace("|", " ");
                    ClientName = ClientName.Replace(@"\", " ");
                    ClientName = ClientName.Replace(":", " ");
                    ClientName = ClientName.Replace("<", " ");
                    ClientName = ClientName.Replace(">", " ");
                    ClientName = ClientName.Replace("?", " ");
                    ClientName = ClientName.Replace("/", " ");
                    ReportName = ClientName + " №" + string.Join(",", SelectedOrderNumbers) + ", обр";

                    PhantomForm PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();
                    bool InMutualSettlement = false;
                    int Result = 1;
                    string Notes = string.Empty;
                    string SaveFilePath = string.Empty;
                    SaveDBFReportMenu = new Infinium.SaveDBFReportMenu(this, ReportName);

                    TopForm = SaveDBFReportMenu;
                    SaveDBFReportMenu.ShowDialog();

                    PhantomForm.Close();
                    InMutualSettlement = SaveDBFReportMenu.InMutualSettlement;
                    Result = SaveDBFReportMenu.Result;
                    Notes = SaveDBFReportMenu.Notes;
                    SaveFilePath = SaveDBFReportMenu.SaveFilePath;
                    PhantomForm.Dispose();
                    SaveDBFReportMenu.Dispose();
                    TopForm = null;

                    if (Result != 3)
                    {
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                        DBFReport.CreateReport(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders,
                            ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, true, ref TotalProfil, ref TotalTPS);
                        DBFReport.SaveDBF(SaveFilePath, ReportName, true, ref ProfilDBFName, ref TPSDBFName);

                        DetailsReport.Report(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                            ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, true);

                        FileInfo file = new FileInfo(SaveFilePath + @"\" + ReportName + ".xls");
                        int j = 1;
                        while (file.Exists == true)
                        {
                            file = new FileInfo(SaveFilePath + @"\" + ReportName + "(" + j++ + ").xls");
                        }

                        FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                        hssfworkbook.Write(NewFile);
                        NewFile.Close();

                        if (InMutualSettlement)
                        {
                            int MutualSettlementID = -1;
                            if (TotalProfil > 0)
                            {
                                if (ClientCountryID == 1)
                                    TotalProfil *= 1.2m;
                                TotalProfil = Decimal.Round(TotalProfil, 2, MidpointRounding.AwayFromZero);
                                MutualSettlementID = MutualSettlementsManager.AddMutualSettlement(ClientID, 1, CurrencyTypeID, DiscountPaymentConditionID, DelayOfPayment, TotalProfil, Notes, true);
                                var fileInfo = new System.IO.FileInfo(file.FullName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(file.FullName),
                                    fileInfo.Extension, file.FullName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceExcel, MutualSettlementID);
                                fileInfo = new System.IO.FileInfo(ProfilDBFName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(ProfilDBFName),
                                    fileInfo.Extension, ProfilDBFName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceDbf, MutualSettlementID);
                                DateTime CurrentDate = Security.GetCurrentDate();
                                for (int i = 0; i < SelectedProfilOrderNumbers.Count(); i++)
                                    MutualSettlementsManager.AddMutualSettlementOrder(CurrentDate, ClientID, MutualSettlementID, SelectedProfilOrderNumbers[i]);
                                //MutualSettlementsManager.AddSubscribesRecord(404, MutualSettlementID);
                            }
                            if (TotalTPS > 0)
                            {
                                if (ClientCountryID == 1)
                                    TotalTPS *= 1.2m;
                                TotalTPS = Decimal.Round(TotalTPS, 2, MidpointRounding.AwayFromZero);
                                MutualSettlementID = MutualSettlementsManager.AddMutualSettlement(ClientID, 2, CurrencyTypeID, DiscountPaymentConditionID, DelayOfPayment, TotalTPS, Notes, true);
                                var fileInfo = new System.IO.FileInfo(file.FullName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(file.FullName),
                                    fileInfo.Extension, file.FullName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceExcel, MutualSettlementID);
                                fileInfo = new System.IO.FileInfo(TPSDBFName);
                                MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(TPSDBFName),
                                    fileInfo.Extension, TPSDBFName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceDbf, MutualSettlementID);
                                DateTime CurrentDate = Security.GetCurrentDate();
                                for (int i = 0; i < SelectedTPSOrderNumbers.Count(); i++)
                                    MutualSettlementsManager.AddMutualSettlementOrder(CurrentDate, ClientID, MutualSettlementID, SelectedTPSOrderNumbers[i]);
                                MutualSettlementsManager.AddSubscribesRecord(324, MutualSettlementID);
                            }
                            MutualSettlementsManager.SaveMutualSettlements();
                            MutualSettlementsManager.SaveMutualSettlementOrders();
                            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Счет отправлен на бухгалтерию");
                        }

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        DetailsReport.Save = false;
                        DetailsReport.Send = false;

                        PhantomForm = new Infinium.PhantomForm();
                        PhantomForm.Show();

                        ClientReportMenu = new ClientReportMenu(this);
                        TopForm = ClientReportMenu;

                        ClientReportMenu.ShowDialog();

                        PhantomForm.Close();
                        DetailsReport.Save = ClientReportMenu.Save;
                        DetailsReport.Send = ClientReportMenu.Send;

                        PhantomForm.Dispose();
                        ClientReportMenu.Dispose();
                        TopForm = null;

                        System.Diagnostics.Process.Start(file.FullName);
                        ReportName = file.FullName;

                        if (DetailsReport.Save || DetailsReport.Send)
                        {
                            //REPORT
                            T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            string FileName = DetailsReport.Report(SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                                ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                                OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, true);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;

                            if (DetailsReport.Send)
                            {
                                //SEND
                                T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Отправка письма.\r\nПодождите..."); });
                                T.Start();
                                while (!SplashWindow.bSmallCreated) ;

                                SendEmail.Send(ClientID, string.Join(",", SelectedOrderNumbers), DetailsReport.Save, FileName);

                                while (SplashWindow.bSmallCreated)
                                    SmallWaitForm.CloseS = true;
                                NeedSplash = true;

                                if (SendEmail.Success == false)
                                {
                                    Infinium.LightMessageBox.Show(ref TopForm, false,
                                           "Отправка отчета невозможна: отсутствует подключение к интернету либо адрес электронной почты указан неверно",
                                           "Отправка письма");
                                }
                                OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Отчет отправлен клиенту");
                                //SendEmail.DeleteFile(FileName);
                            }
                        }
                    }
                }
            }
            else
            {
                decimal TotalProfil = 0;
                decimal TotalTPS = 0;

                string ProfilDBFName = string.Empty;
                string TPSDBFName = string.Empty;
                string ReportName = string.Empty;
                int ClientCountryID = OrdersManager.GetClientCountry(ClientID);

                ClientName = ClientName.Replace("\"", " ");
                ClientName = ClientName.Replace("*", " ");
                ClientName = ClientName.Replace("|", " ");
                ClientName = ClientName.Replace(@"\", " ");
                ClientName = ClientName.Replace(":", " ");
                ClientName = ClientName.Replace("<", " ");
                ClientName = ClientName.Replace(">", " ");
                ClientName = ClientName.Replace("?", " ");
                ClientName = ClientName.Replace("/", " ");
                ReportName = ClientName + " №" + string.Join(",", SelectedOrderNumbers);

                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();
                bool InMutualSettlement = false;
                int Result = 1;
                string Notes = string.Empty;
                string SaveFilePath = string.Empty;
                SaveDBFReportMenu = new Infinium.SaveDBFReportMenu(this, ReportName);

                TopForm = SaveDBFReportMenu;
                SaveDBFReportMenu.ShowDialog();

                PhantomForm.Close();
                InMutualSettlement = SaveDBFReportMenu.InMutualSettlement;
                Result = SaveDBFReportMenu.Result;
                Notes = SaveDBFReportMenu.Notes;
                SaveFilePath = SaveDBFReportMenu.SaveFilePath;
                PhantomForm.Dispose();
                SaveDBFReportMenu.Dispose();
                TopForm = null;

                if (Result != 3)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                    DBFReport.CreateReport(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders,
                        ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                        OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, ref TotalProfil, ref TotalTPS);
                    DBFReport.SaveDBF(SaveFilePath, ReportName, ref ProfilDBFName, ref TPSDBFName);

                    DetailsReport.Report(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                        ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                        OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight);

                    FileInfo file = new FileInfo(SaveFilePath + @"\" + ReportName + ".xls");
                    int j = 1;
                    while (file.Exists == true)
                    {
                        file = new FileInfo(SaveFilePath + @"\" + ReportName + "(" + j++ + ").xls");
                    }

                    FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                    hssfworkbook.Write(NewFile);
                    NewFile.Close();

                    if (InMutualSettlement)
                    {
                        int MutualSettlementID = -1;

                        if (TotalProfil > 0)
                        {
                            if (ClientCountryID == 1)
                                TotalProfil *= 1.2m;
                            TotalProfil = Decimal.Round(TotalProfil, 2, MidpointRounding.AwayFromZero);
                            MutualSettlementID = MutualSettlementsManager.AddMutualSettlement(ClientID, 1, CurrencyTypeID, DiscountPaymentConditionID, DelayOfPayment, TotalProfil, Notes);
                            var fileInfo = new System.IO.FileInfo(file.FullName);
                            MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(file.FullName),
                                fileInfo.Extension, file.FullName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceExcel, MutualSettlementID);
                            fileInfo = new System.IO.FileInfo(ProfilDBFName);
                            MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(ProfilDBFName),
                                fileInfo.Extension, ProfilDBFName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceDbf, MutualSettlementID);
                            DateTime CurrentDate = Security.GetCurrentDate();
                            for (int i = 0; i < SelectedProfilOrderNumbers.Count(); i++)
                                MutualSettlementsManager.AddMutualSettlementOrder(CurrentDate, ClientID, MutualSettlementID, SelectedProfilOrderNumbers[i]);
                            //MutualSettlementsManager.AddSubscribesRecord(404, MutualSettlementID);
                        }
                        if (TotalTPS > 0)
                        {
                            if (ClientCountryID == 1)
                                TotalTPS *= 1.2m;
                            TotalTPS = Decimal.Round(TotalTPS, 2, MidpointRounding.AwayFromZero);
                            MutualSettlementID = MutualSettlementsManager.AddMutualSettlement(ClientID, 2, CurrencyTypeID, DiscountPaymentConditionID, DelayOfPayment, TotalTPS, Notes);
                            var fileInfo = new System.IO.FileInfo(file.FullName);
                            MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(file.FullName),
                                fileInfo.Extension, file.FullName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceExcel, MutualSettlementID);
                            fileInfo = new System.IO.FileInfo(TPSDBFName);
                            MutualSettlementsManager.AttachIncomeDocument(Path.GetFileNameWithoutExtension(TPSDBFName),
                                fileInfo.Extension, TPSDBFName, Infinium.Modules.Marketing.MutualSettlements.DocumentTypes.InvoiceDbf, MutualSettlementID);
                            DateTime CurrentDate = Security.GetCurrentDate();
                            for (int i = 0; i < SelectedTPSOrderNumbers.Count(); i++)
                                MutualSettlementsManager.AddMutualSettlementOrder(CurrentDate, ClientID, MutualSettlementID, SelectedTPSOrderNumbers[i]);
                            MutualSettlementsManager.AddSubscribesRecord(324, MutualSettlementID);
                        }
                        MutualSettlementsManager.SaveMutualSettlements();
                        MutualSettlementsManager.SaveMutualSettlementOrders();
                        OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Счет отправлен на бухгалтерию");
                    }

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                    NeedSplash = true;

                    DetailsReport.Save = false;
                    DetailsReport.Send = false;

                    PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();

                    ClientReportMenu = new ClientReportMenu(this);
                    TopForm = ClientReportMenu;

                    ClientReportMenu.ShowDialog();

                    PhantomForm.Close();
                    DetailsReport.Save = ClientReportMenu.Save;
                    DetailsReport.Send = ClientReportMenu.Send;

                    PhantomForm.Dispose();
                    ClientReportMenu.Dispose();
                    TopForm = null;

                    System.Diagnostics.Process.Start(file.FullName);
                    ReportName = file.FullName;

                    if (DetailsReport.Save || DetailsReport.Send)
                    {
                        //REPORT
                        T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        string FileName = DetailsReport.Report(SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                            ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight);

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        if (DetailsReport.Send)
                        {
                            //SEND
                            T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Отправка письма.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            SendEmail.Send(ClientID, string.Join(",", SelectedOrderNumbers), DetailsReport.Save, FileName);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;

                            if (SendEmail.Success == false)
                            {
                                Infinium.LightMessageBox.Show(ref TopForm, false,
                                       "Отправка отчета невозможна: отсутствует подключение к интернету либо адрес электронной почты указан неверно",
                                       "Отправка письма");
                            }
                            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Отчет отправлен клиенту");
                            //SendEmail.DeleteFile(FileName);
                        }
                    }
                }
            }
        }

        public void GetCurrency(DateTime Date)
        {
            bool RateExist = false;

            decimal EURRUBCurrency = 1000000;
            decimal USDRUBCurrency = 1000000;
            decimal EURUSDCurrency = 1000000;
            decimal EURBYRCurrency = 1000000;
            //OrdersManager.NBRBDailyRates(Date, ref EURBYRCurrency);
            //OrdersManager.CBRDailyRates(Date, ref EURRUBCurrency, ref USDRUBCurrency);
            //if (USDRUBCurrency != 0)
            //    EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);

            OrdersManager.GetDateRates(Date, ref RateExist, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency, ref USDRUBCurrency);

            if (!RateExist)
            {
                OrdersManager.CBRDailyRates(Date, ref EURRUBCurrency, ref USDRUBCurrency);
                OrdersManager.NBRBDailyRates(Date, ref EURBYRCurrency);

                if (USDRUBCurrency != 0)
                    EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);
                if (EURBYRCurrency == 1000000)
                {

                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Курс не взят. Возможная причина - нет связи с банком без авторизации в kerio control. Войдите в kerio и повторите попытку",
                        "Расчет заказа");
                    return;
                }
                OrdersManager.SaveDateRates(Date, EURUSDCurrency, EURRUBCurrency, EURBYRCurrency, USDRUBCurrency);
            }

            CurrencyEURRUBTextBox.Text = EURRUBCurrency.ToString();
            CurrencyUSDRUBTextBox.Text = USDRUBCurrency.ToString();
            CurrencyEURUSDTextBox.Text = EURUSDCurrency.ToString();
            CurrencyEURBYRTextBox.Text = EURBYRCurrency.ToString();

        }

        private void CurrencyDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            GetCurrency(CurrencyDateTimePicker.Value.Date);
        }

        private void CurrencyTodayButton_Click(object sender, EventArgs e)
        {
            CurrencyDateTimePicker.Value = DateTime.Now;
        }

        private void MegaOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
                //OrdersManager.MegaOrdersBindingSource.Position = CurrentRowIndex;

                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MainOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
                //OrdersManager.MegaOrdersBindingSource.Position = CurrentRowIndex;

                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MegaOrderDecorInProd_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            BatchReport MarketingBatchReport = new BatchReport(ref DecorCatalogOrder);
            int[] MainOrders = OrdersManager.GetMainOrders();
            MarketingBatchReport.CreateReportForMaketing(MainOrders);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MainOrderDecorInProd_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            BatchReport MarketingBatchReport = new BatchReport(ref DecorCatalogOrder);
            int[] MainOrders = OrdersManager.GetSelectedMainOrders();
            MarketingBatchReport.CreateReportForMaketing(MainOrders);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void cbOnExpedition_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void kryptonContextMenu2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (RoleType == RoleTypes.Ordinary)
                e.Cancel = true;
        }

        private void kryptonContextMenu1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (RoleType == RoleTypes.Ordinary)
                e.Cancel = true;
        }

        private void cbDelayOfPayment_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void cbHalfOfPayment_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void cbFullPayment_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void cbFactoring_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void MegaOrdersDataGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int ClientID = 0;
            int OrderNumber = 0;
            if (grid.Rows[e.RowIndex].Cells["ClientID"].Value != DBNull.Value)
                ClientID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["ClientID"].Value);
            if (grid.Rows[e.RowIndex].Cells["OrderNumber"].Value != DBNull.Value)
                OrderNumber = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["OrderNumber"].Value);
            bool IsOrderInMuttlement = OrdersManager.IsOrderInMuttlement(ClientID, OrderNumber);
            if (IsOrderInMuttlement)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.MediumBlue;
            }
            else
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int[] MainOrders = new int[MainOrdersDataGrid.SelectedRows.Count];

            decimal ComplaintProfilCost = 0;
            decimal ComplaintTPSCost = 0;
            decimal TotalWeight = 0;
            decimal TransportCost = 0;
            decimal AdditionalCost = 0;
            int CurrencyTypeID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]);
            decimal Rate = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]);

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                ComplaintProfilCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyComplaintProfilCost"].Value);
                ComplaintTPSCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyComplaintTPSCost"].Value);
                TotalWeight += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["Weight"].Value);
                TransportCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyTransportCost"].Value);
                AdditionalCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyAdditionalCost"].Value);
            }

            //ComplaintProfilCost = Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyComplaintProfilCost"].Value);
            //ComplaintTPSCost = Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyComplaintTPSCost"].Value);
            //TransportCost = Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyTransportCost"].Value);
            //AdditionalCost = Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyAdditionalCost"].Value);

            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                MainOrders[i] = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);

            int[] SelectedMegaOrders = new int[1];
            int[] SelectedOrderNumbers = new int[1];
            SelectedMegaOrders[0] = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            SelectedOrderNumbers[0] = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["OrderNumber"]);

            int ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
            string ClientName = OrdersManager.GetClientName(ClientID);

            NeedSplash = false;

            InvoiceReportToDBF DBFReport = new InvoiceReportToDBF();

            decimal TotalProfil = 0;
            decimal TotalTPS = 0;

            string ProfilDBFName = string.Empty;
            string TPSDBFName = string.Empty;
            string ReportName = string.Empty;
            int ClientCountryID = OrdersManager.GetClientCountry(ClientID);

            ClientName = ClientName.Replace("\"", " ");
            ClientName = ClientName.Replace("*", " ");
            ClientName = ClientName.Replace("|", " ");
            ClientName = ClientName.Replace(@"\", " ");
            ClientName = ClientName.Replace(":", " ");
            ClientName = ClientName.Replace("<", " ");
            ClientName = ClientName.Replace(">", " ");
            ClientName = ClientName.Replace("?", " ");
            ClientName = ClientName.Replace("/", " ");
            ReportName = ClientName + " №" + string.Join(",", SelectedOrderNumbers);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            DBFReport.CreateReport(ref hssfworkbook, SelectedOrderNumbers, SelectedMegaOrders, MainOrders,
                ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, ref TotalProfil, ref TotalTPS);
            //DBFReport.SaveDBF(Security.DBFSaveFilePath, ReportName, ref ProfilDBFName, ref TPSDBFName);

            DetailsReport.Report(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight);

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + ReportName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + ReportName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
            ReportName = file.FullName;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;

        }

        private void MainOrdersFrontsOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ComponentFactory.Krypton.Toolkit.KryptonContextMenu kryptonContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
                int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["MainOrderID"].Value);
                int FrontID = Convert.ToInt32(MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["FrontID"].Value);

                if (TestTechCatalogManager == null)
                {
                    TestTechCatalogManager = new Modules.TechnologyCatalog.TestTechCatalog();
                    TestTechCatalogManager.Initialize();
                }
                DataTable DT = TestTechCatalogManager.GetOperationsGroups(FrontID);
                if (DT.Rows.Count > 0)
                {
                    ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem()
                        {
                            Text = DT.Rows[i]["GroupName"].ToString(),
                            Tag = Convert.ToInt32(DT.Rows[i]["TechCatalogOperationsGroupID"])
                        };
                        kryptonContextMenuItem.Click += kryptonContextMenuItem_Click;
                        kryptonContextMenuItems.Items.Add(kryptonContextMenuItem);
                    }
                    kryptonContextMenu.Items.Add(kryptonContextMenuItems);
                    kryptonContextMenu.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
                }
            }
        }

        void kryptonContextMenuItem_Click(object sender, EventArgs e)
        {
            TestTechCatalogManager.GetFrontsOrders(
                Convert.ToInt32(MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["FrontsOrdersID"].Value),
                Convert.ToInt32(((ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem)sender).Tag));
            TestTechCatalogManager.ReturnResultTable();

            TestTechCatalogManager.CreateFrontsExcel(MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["FrontsOrdersID"].Value.ToString(),
                Convert.ToInt32(((ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem)sender).Tag));
        }

        bool Func1(DataTable DT1, int TechCatalogOperationsDetailID,
            ref ComponentFactory.Krypton.Toolkit.KryptonContextMenu kryptonContextMenu,
            ref ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem)
        {
            string filter = "PrevTechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID;
            DataRow[] rows1 = DT1.Select(filter);
            if (rows1.Count() == 0)
                return false;

            DataTable DT = DT1.Clone();
            foreach (DataRow item in rows1)
                DT.Rows.Add(item.ItemArray);
            ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            kryptonContextMenuItem.Items.Add(kryptonContextMenuItems);
            Func2(DT1, DT, TechCatalogOperationsDetailID, ref kryptonContextMenu, ref kryptonContextMenuItems);
            return true;
        }

        void Func2(DataTable DT1, DataTable DT2, int TechCatalogOperationsDetailID, ref ComponentFactory.Krypton.Toolkit.KryptonContextMenu kryptonContextMenu,
            ref ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems)
        {
            for (int j = 0; j < DT2.Rows.Count; j++)
            {
                TechCatalogOperationsDetailID = Convert.ToInt32(DT2.Rows[j]["TechCatalogOperationsDetailID"]);
                string GroupName = DT2.Rows[j]["GroupName"].ToString();
                ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();

                if (Func1(DT1, TechCatalogOperationsDetailID, ref kryptonContextMenu, ref kryptonContextMenuItem))
                {

                }
                kryptonContextMenuItem.Text = GroupName;
                kryptonContextMenuItem.Tag = TechCatalogOperationsDetailID;
                kryptonContextMenuItems.Items.Add(kryptonContextMenuItem);
            }
        }

        private void kryptonCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void kryptonCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (OrdersManager == null)
                return;
            Filter();
        }

        private void MegaOrdersDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (MegaOrdersDataGrid.Columns[e.ColumnIndex].Name == "PlanDispDate")
            {
                OrdersManager.UpdatemegaOrders();
                Filter();
            }
        }

        private void btnSetAgreementStatus_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int AgreementStatusID = 0;
            int MegaOrderID = 0;
            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                AgreementStatusID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["AgreementStatusID"]);

            SetOrderStatusForm SetOrderStatusForm = new SetOrderStatusForm(this, AgreementStatusID);

            TopForm = SetOrderStatusForm;
            SetOrderStatusForm.ShowDialog();
            AgreementStatusID = SetOrderStatusForm.AgreementStatusID;

            PhantomForm.Close();

            PhantomForm.Dispose();
            SetOrderStatusForm.Dispose();
            TopForm = null;

            if (AgreementStatusID > -1)
            {
                OrdersManager.SetAgreementStatus(AgreementStatusID);
                NeedSplash = false;
                Filter();
                OrdersManager.MoveToMegaOrder(MegaOrderID);
                NeedSplash = true;
            }
        }

        private void btnDeleteAgreedOrder_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]);

                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Вы действительно хотите удалить заказ " + MegaOrderID + " ?",
                        "Удаление заказа");
                if (!OKCancel)
                    return;

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                OrdersManager.RemoveCurrentAgreedMegaOrder();
                OrdersManager.FixOrderEvent(MegaOrderID, "Согласованный заказ удален");

                if (OrdersManager.MegaOrdersCount == 0)
                {
                    OrdersManager.CurrentMegaOrderID = -1;
                }
                Filter();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void btnDeleteAgreedMainOrder_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MainOrdersBindingSource.Count == 0)
                return;

            string MainOrderID = ((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"].ToString();
            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы действительно хотите удалить подзаказ " + MainOrderID + " ?",
                    "Удаление подзаказа");
            if (!OKCancel)
                return;

            NeedSplash = false;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            OrdersManager.RemoveCurrentAgreedMainOrder();

            bool OnProduction = OnProductionCheckBox.Checked;
            bool NotInProduction = NotProductionCheckBox.Checked;
            bool InProduction = InProductionCheckBox.Checked;
            bool OnStorage = OnStorageCheckBox.Checked;
            bool OnExpedition = cbOnExpedition.Checked;
            bool Dispatch = DispatchCheckBox.Checked;

            if (OrdersManager.MainOrdersBindingSource.Count > 0)
                OrdersManager.FilterMainOrdersByMegaOrder(
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]),
                    OnProduction,
                    NotInProduction,
                    InProduction,
                    OnStorage,
                    OnExpedition,
                    true);

            OrdersManager.SummaryCost(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]));
            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Удален согласованный подзаказ");
            Filter();
            OrdersManager.MoveToMegaOrder(MegaOrderID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void btnEditAgreedMainOrder_Click(object sender, EventArgs e)
        {
            OrdersManager.NeedSetStatus = true;
            if (OrdersManager.MainOrdersBindingSource.Count > 0)
            {
                NeedSplash = false;

                OrdersManager.EditAgreedMainOrder();

                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;

                if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                {
                    OrdersManager.CurrentClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
                    OrdersManager.CurrentDiscountPaymentConditionID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountPaymentConditionID"]);
                    OrdersManager.CurrentDiscountFactoringID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DiscountFactoringID"]);
                    OrdersManager.CurrentProfilDiscountDirector = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilDiscountDirector"]);
                    OrdersManager.CurrentTPSDiscountDirector = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSDiscountDirector"]);
                    OrdersManager.CurrentProfilTotalDiscount = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilTotalDiscount"]) - Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ProfilDiscountDirector"]);
                    OrdersManager.CurrentTPSTotalDiscount = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSTotalDiscount"]) - Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TPSDiscountDirector"]);
                    OrdersManager.CurrencyTypeID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]);
                    OrdersManager.PaymentCurrency = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]);
                    OrdersManager.ConfirmDateTime = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ConfirmDateTime"];
                }
                AddMainOrdersForm = new AddMarketingNewOrdersForm(ref OrdersManager, true, true, ref TopForm, ref OrdersCalculate);

                TopForm = AddMainOrdersForm;
                AddMainOrdersForm.ShowDialog();

                AddMainOrdersForm.Close();
                AddMainOrdersForm.Dispose();

                TopForm = null;

                bool OnProduction = OnProductionCheckBox.Checked;
                bool NotInProduction = NotProductionCheckBox.Checked;
                bool InProduction = InProductionCheckBox.Checked;
                bool OnStorage = OnStorageCheckBox.Checked;
                bool OnExpedition = cbOnExpedition.Checked;
                bool Dispatch = DispatchCheckBox.Checked;

                NeedSplash = false;
                Filter();

                OrdersManager.UpdateMainOrders(
                    OnProduction,
                    NotInProduction,
                    InProduction,
                    OnStorage,
                    OnExpedition,
                    Dispatch);
                NeedSplash = true;
            }
            OrdersManager.CurrentClientID = -1;
            OrdersManager.CurrentDiscountPaymentConditionID = 0;
            OrdersManager.CurrentDiscountFactoringID = 0;
            OrdersManager.CurrentProfilDiscountDirector = 0;
            OrdersManager.CurrentTPSDiscountDirector = 0;
            OrdersManager.CurrentProfilTotalDiscount = 0;
            OrdersManager.CurrentTPSTotalDiscount = 0;
            OrdersManager.PaymentCurrency = 1;
            OrdersManager.CurrencyTypeID = 1;
            OrdersManager.ConfirmDateTime = DBNull.Value;
        }

        private void kryptonContextMenuItem25_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MainOrdersBindingSource.Count == 0)
                return;
            int CurrencyTypeID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value);
            int DiscountPaymentConditionID = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["DiscountPaymentConditionID"].Value);
            int DelayOfPayment = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["DelayOfPayment"].Value);
            if (MegaOrdersDataGrid.SelectedRows.Count > 0)
            {
                int d = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["DiscountPaymentConditionID"].Value);
                for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                {
                    if (d != Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["DiscountPaymentConditionID"].Value))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false,
                               "Нельзя объединить заказы с различными условиями оплаты",
                               "Создание отчета");
                        return;
                    }
                }
            }
            if (MegaOrdersDataGrid.SelectedRows.Count > 0)
            {
                int c = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[0].Cells["CurrencyTypeID"].Value);
                for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                {
                    if (c != Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyTypeID"].Value))
                    {
                        Infinium.LightMessageBox.Show(ref TopForm, false,
                               "Нельзя объединить заказы с различными валютами",
                               "Создание отчета");
                        return;
                    }
                }
            }
            int[] SelectedMegaOrders = OrdersManager.GetSelectedMegaOrders();

            if (!OrdersManager.AreSelectedMegaOrdersOneClient)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Выбраны заказы разных клиентов",
                       "Создание отчета");
                return;
            }

            //if (!OrdersManager.AreSelectedMegaOrdersAgree(SelectedMegaOrders))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //           "Выбранные заказы несогласованы",
            //           "Создание отчета");
            //    return;
            //}

            int[] MainOrders = OrdersManager.GetMainOrders(SelectedMegaOrders);

            decimal ComplaintProfilCost = 0;
            decimal ComplaintTPSCost = 0;
            decimal TransportCost = 0;
            decimal AdditionalCost = 0;
            decimal TotalWeight = 0;
            CurrencyTypeID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]);
            decimal Rate = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]);
            int OrderNumber = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["OrderNumber"]);

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                ComplaintProfilCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyComplaintProfilCost"].Value);
                ComplaintTPSCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyComplaintTPSCost"].Value);
                TotalWeight += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["Weight"].Value);
                TransportCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyTransportCost"].Value);
                AdditionalCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["CurrencyAdditionalCost"].Value);
            }

            if (!OrdersManager.CanBeReported())
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Заказ не был согласован. Создание отчета невозможно.",
                       "Создание отчета");
                return;
            }

            int[] SelectedOrderNumbers = OrdersManager.GetSelectedOrderNumbers();
            int[] SelectedProfilOrderNumbers = new int[MegaOrdersDataGrid.SelectedRows.Count];
            int[] SelectedTPSOrderNumbers = new int[MegaOrdersDataGrid.SelectedRows.Count];

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 1 || Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 0)
                    SelectedProfilOrderNumbers[i] = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["OrderNumber"].Value);
                if (Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 2 || Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["FactoryID"].Value) == 0)
                    SelectedTPSOrderNumbers[i] = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["OrderNumber"].Value);
            }
            SelectedProfilOrderNumbers = SelectedProfilOrderNumbers.Where(val => val != 0).ToArray();
            SelectedTPSOrderNumbers = SelectedTPSOrderNumbers.Where(val => val != 0).ToArray();
            int ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
            string ClientName = OrdersManager.GetClientName(ClientID);

            NeedSplash = false;

            Modules.Marketing.NewOrders.PrepareReport.InvoiceReportToDBF DBFReport = new Modules.Marketing.NewOrders.PrepareReport.InvoiceReportToDBF();

            Infinium.Modules.Marketing.MutualSettlements.MutualSettlements MutualSettlementsManager = new Modules.Marketing.MutualSettlements.MutualSettlements();
            MutualSettlementsManager.Initialize();

            if (ClientID == 145)
            {
                bool HasOrdinaryOrders = OrdersManager.HasOrders(MainOrders, false);
                bool HasSampleOrders = OrdersManager.HasOrders(MainOrders, true);

                if (HasOrdinaryOrders)
                {
                    decimal TotalProfil = 0;
                    decimal TotalTPS = 0;

                    string ProfilDBFName = string.Empty;
                    string TPSDBFName = string.Empty;
                    string ReportName = string.Empty;
                    int ClientCountryID = OrdersManager.GetClientCountry(ClientID);

                    ClientName = ClientName.Replace("\"", " ");
                    ClientName = ClientName.Replace("*", " ");
                    ClientName = ClientName.Replace("|", " ");
                    ClientName = ClientName.Replace(@"\", " ");
                    ClientName = ClientName.Replace(":", " ");
                    ClientName = ClientName.Replace("<", " ");
                    ClientName = ClientName.Replace(">", " ");
                    ClientName = ClientName.Replace("?", " ");
                    ClientName = ClientName.Replace("/", " ");
                    ReportName = ClientName + " №" + string.Join(",", SelectedOrderNumbers);

                    PhantomForm PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();
                    bool InMutualSettlement = false;
                    int Result = 1;
                    string Notes = string.Empty;
                    string SaveFilePath = string.Empty;
                    SaveDBFReportMenu = new Infinium.SaveDBFReportMenu(this, ReportName);

                    TopForm = SaveDBFReportMenu;
                    SaveDBFReportMenu.ShowDialog();

                    PhantomForm.Close();
                    InMutualSettlement = SaveDBFReportMenu.InMutualSettlement;
                    Result = SaveDBFReportMenu.Result;
                    Notes = SaveDBFReportMenu.Notes;
                    SaveFilePath = SaveDBFReportMenu.SaveFilePath;
                    PhantomForm.Dispose();
                    SaveDBFReportMenu.Dispose();
                    TopForm = null;

                    if (Result != 3)
                    {
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                        DBFReport.CreateReport(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders,
                            ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, false, ref TotalProfil, ref TotalTPS);
                        DBFReport.SaveDBF(SaveFilePath, ReportName, false, ref ProfilDBFName, ref TPSDBFName);

                        PrepareReport.Report(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                            ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, false);

                        FileInfo file = new FileInfo(SaveFilePath + @"\" + ReportName + ".xls");
                        int j = 1;
                        while (file.Exists == true)
                        {
                            file = new FileInfo(SaveFilePath + @"\" + ReportName + "(" + j++ + ").xls");
                        }

                        FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                        hssfworkbook.Write(NewFile);
                        NewFile.Close();

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                }
                if (HasSampleOrders)
                {
                    decimal TotalProfil = 0;
                    decimal TotalTPS = 0;

                    string ProfilDBFName = string.Empty;
                    string TPSDBFName = string.Empty;
                    string ReportName = string.Empty;
                    int ClientCountryID = OrdersManager.GetClientCountry(ClientID);

                    ClientName = ClientName.Replace("\"", " ");
                    ClientName = ClientName.Replace("*", " ");
                    ClientName = ClientName.Replace("|", " ");
                    ClientName = ClientName.Replace(@"\", " ");
                    ClientName = ClientName.Replace(":", " ");
                    ClientName = ClientName.Replace("<", " ");
                    ClientName = ClientName.Replace(">", " ");
                    ClientName = ClientName.Replace("?", " ");
                    ClientName = ClientName.Replace("/", " ");
                    ReportName = ClientName + " №" + string.Join(",", SelectedOrderNumbers) + ", обр";

                    PhantomForm PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();
                    bool InMutualSettlement = false;
                    int Result = 1;
                    string Notes = string.Empty;
                    string SaveFilePath = string.Empty;
                    SaveDBFReportMenu = new Infinium.SaveDBFReportMenu(this, ReportName);

                    TopForm = SaveDBFReportMenu;
                    SaveDBFReportMenu.ShowDialog();

                    PhantomForm.Close();
                    InMutualSettlement = SaveDBFReportMenu.InMutualSettlement;
                    Result = SaveDBFReportMenu.Result;
                    Notes = SaveDBFReportMenu.Notes;
                    SaveFilePath = SaveDBFReportMenu.SaveFilePath;
                    PhantomForm.Dispose();
                    SaveDBFReportMenu.Dispose();
                    TopForm = null;

                    if (Result != 3)
                    {
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                        DBFReport.CreateReport(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders,
                            ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, true, ref TotalProfil, ref TotalTPS);
                        DBFReport.SaveDBF(SaveFilePath, ReportName, true, ref ProfilDBFName, ref TPSDBFName);

                        PrepareReport.Report(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                            ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                            OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, true);

                        FileInfo file = new FileInfo(SaveFilePath + @"\" + ReportName + ".xls");
                        int j = 1;
                        while (file.Exists == true)
                        {
                            file = new FileInfo(SaveFilePath + @"\" + ReportName + "(" + j++ + ").xls");
                        }

                        FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                        hssfworkbook.Write(NewFile);
                        NewFile.Close();

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                }
            }
            else
            {
                decimal TotalProfil = 0;
                decimal TotalTPS = 0;

                string ProfilDBFName = string.Empty;
                string TPSDBFName = string.Empty;
                string ReportName = string.Empty;
                int ClientCountryID = OrdersManager.GetClientCountry(ClientID);

                ClientName = ClientName.Replace("\"", " ");
                ClientName = ClientName.Replace("*", " ");
                ClientName = ClientName.Replace("|", " ");
                ClientName = ClientName.Replace(@"\", " ");
                ClientName = ClientName.Replace(":", " ");
                ClientName = ClientName.Replace("<", " ");
                ClientName = ClientName.Replace(">", " ");
                ClientName = ClientName.Replace("?", " ");
                ClientName = ClientName.Replace("/", " ");
                ReportName = ClientName + " №" + string.Join(",", SelectedOrderNumbers);

                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();
                bool InMutualSettlement = false;
                int Result = 1;
                string Notes = string.Empty;
                string SaveFilePath = string.Empty;
                SaveDBFReportMenu = new Infinium.SaveDBFReportMenu(this, ReportName);

                TopForm = SaveDBFReportMenu;
                SaveDBFReportMenu.ShowDialog();

                PhantomForm.Close();
                InMutualSettlement = SaveDBFReportMenu.InMutualSettlement;
                Result = SaveDBFReportMenu.Result;
                Notes = SaveDBFReportMenu.Notes;
                SaveFilePath = SaveDBFReportMenu.SaveFilePath;
                PhantomForm.Dispose();
                SaveDBFReportMenu.Dispose();
                TopForm = null;

                if (Result != 3)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();

                    while (!SplashWindow.bSmallCreated) ;

                    HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                    DBFReport.CreateReport(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders,
                        ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                        OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight, ref TotalProfil, ref TotalTPS);
                    DBFReport.SaveDBF(SaveFilePath, ReportName, ref ProfilDBFName, ref TPSDBFName);

                    PrepareReport.Report(ref hssfworkbook, SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                        ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                        OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID, TotalWeight);

                    FileInfo file = new FileInfo(SaveFilePath + @"\" + ReportName + ".xls");
                    int j = 1;
                    while (file.Exists == true)
                    {
                        file = new FileInfo(SaveFilePath + @"\" + ReportName + "(" + j++ + ").xls");
                    }

                    FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                    hssfworkbook.Write(NewFile);
                    NewFile.Close();
                    System.Diagnostics.Process.Start(file.FullName);

                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                    NeedSplash = true;
                }
            }
        }

        private void kryptonContextMenuItem26_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = 0;
            int MegaOrderID = 0;
            int MainOrderID = 0;

            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
                MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            }
            if (OrdersManager.MainOrdersBindingSource.Count > 0)
            {
                MainOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current).Row["MainOrderID"]);
            }

            CopyMarketingOrderForm CopyMarketingOrderForm = new CopyMarketingOrderForm(this, false, ClientID, MegaOrderID, MainOrderID);

            TopForm = CopyMarketingOrderForm;
            CopyMarketingOrderForm.ShowDialog();
            ClientID = CopyMarketingOrderForm.ClientID;

            PhantomForm.Close();

            PhantomForm.Dispose();
            CopyMarketingOrderForm.Dispose();
            TopForm = null;

            if (ClientID > 0)
            {
                NeedSplash = false;
                Filter();
                NeedSplash = true;
            }
        }

        private void kryptonContextMenuItem27_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = 0;
            int MegaOrderID = 0;
            int MainOrderID = 0;

            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);
                MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            }
            if (OrdersManager.MainOrdersBindingSource.Count > 0)
            {
                MainOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current).Row["MainOrderID"]);
            }

            CopyMarketingOrderForm CopyMarketingOrderForm = new CopyMarketingOrderForm(this, true, ClientID, MegaOrderID, MainOrderID);

            TopForm = CopyMarketingOrderForm;
            CopyMarketingOrderForm.ShowDialog();
            ClientID = CopyMarketingOrderForm.ClientID;

            PhantomForm.Close();

            PhantomForm.Dispose();
            CopyMarketingOrderForm.Dispose();
            TopForm = null;

            if (ClientID > 0)
            {
                NeedSplash = false;
                Filter();
                NeedSplash = true;
            }
        }

        private void kryptonContextMenuItem28_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                CheckOrdersStatus.GG(
                    Convert.ToInt32(Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value)));
            }
        }
    }
}
