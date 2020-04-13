using Infinium.Modules.ZOV;
using Infinium.Modules.ZOV.DailyReport;
using Infinium.Modules.ZOV.ReportToDBF;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVOrdersForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedRefresh = false;
        bool NeedSplash = false;
        bool SetFonts = false;

        int FormEvent = 0;
        int CurrentRowIndex = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        DataTable RolePermissionsDataTable;

        DecorCatalogOrder DecorCatalogOrder = null;
        OrdersManager OrdersManager = null;
        OrdersCalculate OrdersCalculate = null;
        DailyReport DailyReport;
        PaymentWeeks PaymentWeeks = null;
        NewOrderInfo NewOrderInfo;

        public ZOVOrdersForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID, this.Name);

            if (!PermissionGranted(Convert.ToInt32(NewMainOrderButton.Tag)))
            {
                SetFonts = true;
                panel1.Height = this.Height - NavigatePanel.Height - 10;
                ToolsPanel.Visible = false;
                OrdersSelectButton.Visible = false;
                PaymentsSelectButton.Visible = false;
            }

            if (!SetFonts)
            {
                MegaOrdersDataGrid.StateCommon.DataCell.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                MegaOrdersDataGrid.RowTemplate.Height = 30;
                MegaOrdersDataGrid.ColumnHeadersHeight = 38;
                MegaOrdersDataGrid.StateCommon.HeaderColumn.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                MainOrdersDataGrid.StateCommon.DataCell.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                MainOrdersDataGrid.RowTemplate.Height = 30;
                MainOrdersDataGrid.ColumnHeadersHeight = 38;
                MainOrdersDataGrid.StateCommon.HeaderColumn.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                MainOrdersFrontsOrdersDataGrid.RowTemplate.Height = 30;
                MainOrdersFrontsOrdersDataGrid.ColumnHeadersHeight = 45;
                MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                TempPaymentDetailDataGrid.StateCommon.DataCell.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                TempPaymentDetailDataGrid.RowTemplate.Height = 30;
                TempPaymentDetailDataGrid.ColumnHeadersHeight = 38;
                TempPaymentDetailDataGrid.StateCommon.HeaderColumn.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                PaymentWeeksDataGrid.StateCommon.DataCell.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                PaymentWeeksDataGrid.RowTemplate.Height = 30;
                PaymentWeeksDataGrid.ColumnHeadersHeight = 38;
                PaymentWeeksDataGrid.StateCommon.HeaderColumn.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                PaymentDetailDataGrid.StateCommon.DataCell.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                PaymentDetailDataGrid.RowTemplate.Height = 30;
                PaymentDetailDataGrid.ColumnHeadersHeight = 38;
                PaymentDetailDataGrid.StateCommon.HeaderColumn.Content.Font =
                    new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                SetFonts = true;
            }

            Initialize();


            DateTime DateFrom = Security.GetCurrentDate().AddDays(-8);
            DateTime DateTo = Security.GetCurrentDate().AddDays(8);
            FilterFromDateTimePicker.Value = DateFrom;
            FilterToDateTimePicker.Value = DateTo;

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

        private void ZOVOrdersForm_Shown(object sender, EventArgs e)
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
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            DecorCatalogOrder = new DecorCatalogOrder();

            OrdersManager = new OrdersManager(ref MainOrdersDataGrid,
                ref MainOrdersFrontsOrdersDataGrid, ref MegaOrdersDataGrid,
                ref MainOrdersDecorTabControl, ref MainOrdersTabControl,
                ref DecorCatalogOrder, !PermissionGranted(Convert.ToInt32(NewMainOrderButton.Tag)));

            OrdersCalculate = new OrdersCalculate();

            DailyReport = new DailyReport(ref OrdersManager.MainOrdersFrontsOrders, ref DecorCatalogOrder,
                ref OrdersManager.ClientsDataTable);

            PaymentWeeks = new PaymentWeeks(ref PaymentWeeksDataGrid, ref TempPaymentDetailDataGrid, ref PaymentDetailDataGrid);

            NewOrderInfo = new Modules.ZOV.NewOrderInfo();
            sw.Stop();
            double G = sw.Elapsed.Milliseconds;

            PaymentWeekDocNumberComboBox.DataSource = OrdersManager.SearchMainOrdersDataTable.DefaultView;
            PaymentWeekDocNumberComboBox.DisplayMember = "DocNumber";
            PaymentWeekDocNumberComboBox.ValueMember = "MainOrderID";

            PaymentWeeksDebtTypesComboBox.DataSource = OrdersManager.DebtTypesBindingSource;
            PaymentWeeksDebtTypesComboBox.DisplayMember = "DebtType";
            PaymentWeeksDebtTypesComboBox.ValueMember = "DebtTypeID";

            //DocNumbersComboBox.DataSource = OrdersManager.DocNumbersBindingSource;
            //DocNumbersComboBox.DisplayMember = "DocNumber";
            //DocNumbersComboBox.ValueMember = "DocNumber";

            //SearchPartDocNumberComboBox.DataSource = OrdersManager.SearchPartDocNumberBindingSource;
            //SearchPartDocNumberComboBox.DisplayMember = "DocNumber";
            //SearchPartDocNumberComboBox.ValueMember = "MainOrderID";
        }

        private void MegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (OrdersManager != null)
                if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        bool bDebts = DebtsCB.Checked;
                        bool bDoNotDisp = DoNotDispatchCB.Checked;
                        bool bToAssembly = cbToAssembly.Checked;
                        bool bFromAssembly = cbFromAssembly.Checked;
                        bool bIsNotPaid = cbIsNotPaid.Checked;
                        bool bTechDrilling = cbxTechDrilling.Checked;
                        bool bQuicklyOrder = cbxQuicklyOrder.Checked;
                        bool bDoubleOrder = cbxNotDoubleOrders.Checked;
                        if (NeedSplash)
                        {
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;
                            NeedSplash = false;

                            OrdersManager.FilterMainOrders(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                ProfilCheckBox.Checked, TPSCheckBox.Checked, bDebts, bDoNotDisp, bTechDrilling, bQuicklyOrder, bDoubleOrder, bToAssembly, bFromAssembly, bIsNotPaid);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            OrdersManager.FilterMainOrders(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                ProfilCheckBox.Checked, TPSCheckBox.Checked, bDebts, bDoNotDisp, bTechDrilling, bQuicklyOrder, bDoubleOrder, bToAssembly, bFromAssembly, bIsNotPaid);
                        }
                    }
                }
                else
                    OrdersManager.MainOrdersDataTable.Clear();
        }

        private void MegaOrdersDataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (OrdersManager != null)
                if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        //OrdersManager.FilterMainOrders(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                        //    ProfilCheckBox.Checked, TPSCheckBox.Checked);
                    }
                }
                else
                    OrdersManager.MainOrdersDataTable.Clear();
        }

        private void MegaOrdersDataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (OrdersManager != null)
                if (OrdersManager.MegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        //OrdersManager.FilterMainOrders(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                        //    ProfilCheckBox.Checked, TPSCheckBox.Checked);
                    }
                }
                else
                    OrdersManager.MainOrdersDataTable.Clear();
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

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (OrdersManager != null)
                if (OrdersManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        OrdersManager.Filter(Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]));
                        if (MainOrdersTabControl.TabPages[0].PageVisible && MainOrdersTabControl.TabPages[1].PageVisible)
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];
                    }
                }
                else
                {
                    OrdersManager.Filter(-1);
                }
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void MainOrdersAcceptOrder_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                           "Вы действительно хотите подтвердить отгрузки?",
                           "Подтверждение отгрузки");
                if (!OKCancel)
                    return;

                OrdersManager.SummaryCalcCurrentMegaOrder();
            }
        }

        private void MainOrdersRemoveMegaOrder_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MegaOrdersBindingSource.Count > 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                           "Вы действительно хотите удалить отгрузку?",
                           "Удаление заказа");
                if (!OKCancel)
                    return;

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление отгрузки.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                OrdersManager.RemoveCurrentMegaOrder();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
                InfiniumTips.ShowTip(this, 50, 85, "Отгрузка удалена", 1700);
            }
        }

        private delegate void SetData();

        private void CreateDailyReport()
        {
            if (((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DispatchDate"] == null)
                return;

            DateTime DispatchDate = Convert.ToDateTime(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DispatchDate"]);

            bool IsOrders = DailyReport.CreateReport(OrdersManager.GetMegaOrderID(DispatchDate),
                DispatchDate.ToString("yyyy-MM-dd"));
            DailyReport.ReportToExcel();
        }

        private void CreateReportButton_Click(object sender, EventArgs e)
        {

            if (((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DispatchDate"] == DBNull.Value)
                return;

            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            DateTime DispatchDate = Convert.ToDateTime(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DispatchDate"]);

            if (!DailyReport.IsDispatch(MegaOrderID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "В этот день отгрузок нет",
                    "Ошибка создания отчета");
                return;
            }

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            CreateDailyReport();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void NewMainOrderButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;

            NewOrderInfo.OrderStatus = Modules.ZOV.NewOrderInfo.OrderStatusType.NewOrder;

            NewOrderInfo.IsEditOrder = false;
            NewOrderInfo.IsNewOrder = true;
            NewOrderInfo.IsDoubleOrder = false;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            AddZOVMainOrdersForm AddMainOrdersForm = new AddZOVMainOrdersForm(ref OrdersManager, NewOrderInfo, false, ref TopForm, ref OrdersCalculate);

            TopForm = AddMainOrdersForm;

            AddMainOrdersForm.ShowDialog();

            AddMainOrdersForm.Close();
            AddMainOrdersForm.Dispose();

            TopForm = null;

            //T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            //T.Start();

            //while (!SplashWindow.bSmallCreated) ;

            Filter();

            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void CopyDocNumberButton_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MainOrdersBindingSource.Count == 0)
                return;

            string DocNumber = ((DataRowView)OrdersManager.MainOrdersBindingSource.Current).Row["DocNumber"].ToString();

            Clipboard.SetDataObject(DocNumber);
        }

        private void MainOrdersEditOrder_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            int MainOrderID = 0;
            if (OrdersManager.MainOrdersBindingSource.Count > 0)
            {
                MainOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]);

                if (OrdersManager.IsPackagesAlreadyPacked(MainOrderID))
                {
                    bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Этот заказ уже распределен. Всё равно продолжить?",
                        "Предупреждение");

                    if (!OKCancel)
                        return;
                }

                object DispatchDate = null;
                object ProductionDate = null;

                string Notes = null;
                string DocNumber = string.Empty;
                string DebtDocNumber = string.Empty;

                int ClientID = -1;
                int PriceTypeID = -1;
                int DebtTypeID = -1;

                bool IsSample = false;
                bool IsPrepare = false;
                bool NeedCalculate = false;
                bool DoNotDispatch = false;
                bool TechDrilling = false;
                bool QuicklyOrder = false;
                bool ToAssembly = false;
                bool IsNotPaid = false;
                NewOrderInfo.ProductionDate = DBNull.Value;

                OrdersManager.EditMainOrder(ref DispatchDate, ref ProductionDate, MainOrderID, ref ClientID, ref DocNumber,
                    ref DebtDocNumber, ref PriceTypeID, ref DebtTypeID, ref IsSample, ref IsPrepare, ref Notes,
                    ref NeedCalculate, ref DoNotDispatch, ref TechDrilling, ref QuicklyOrder, ref ToAssembly, ref IsNotPaid);

                NewOrderInfo.MainOrderID = MainOrderID;

                NewOrderInfo.IsEditOrder = true;
                NewOrderInfo.IsNewOrder = false;
                NewOrderInfo.IsDoubleOrder = false;

                NewOrderInfo.ClientID = ClientID;
                NewOrderInfo.PriceType = PriceTypeID;
                NewOrderInfo.DebtType = DebtTypeID;

                NewOrderInfo.DoNotDispatch = DoNotDispatch;
                NewOrderInfo.TechDrilling = TechDrilling;
                NewOrderInfo.QuicklyOrder = QuicklyOrder;
                NewOrderInfo.NeedCalculate = NeedCalculate;

                NewOrderInfo.ToAssembly = ToAssembly;
                NewOrderInfo.IsNotPaid = IsNotPaid;

                if (DebtTypeID > 0)
                    NewOrderInfo.IsDebt = true;
                else
                    NewOrderInfo.IsDebt = false;

                NewOrderInfo.IsSample = IsSample;
                NewOrderInfo.IsPrepare = IsPrepare;

                NewOrderInfo.Notes = Notes;
                NewOrderInfo.ProductionDate = ProductionDate;
                if (!IsPrepare)
                {
                    NewOrderInfo.DispatchDate = Convert.ToDateTime(DispatchDate);
                    NewOrderInfo.OrderStatus = Modules.ZOV.NewOrderInfo.OrderStatusType.EditOrder;
                }
                else
                    NewOrderInfo.OrderStatus = Modules.ZOV.NewOrderInfo.OrderStatusType.PrepareOrder;

                NewOrderInfo.DocNumber = DocNumber;
                NewOrderInfo.DebtDocNumber = DebtDocNumber;

                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;

                AddZOVMainOrdersForm AddMainOrdersForm = new AddZOVMainOrdersForm(ref OrdersManager, NewOrderInfo, true, ref TopForm, ref OrdersCalculate);

                TopForm = AddMainOrdersForm;

                AddMainOrdersForm.ShowDialog();

                AddMainOrdersForm.Close();
                AddMainOrdersForm.Dispose();

                TopForm = null;

                //T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                //T.Start();

                Filter();

                //while (SplashWindow.bSmallCreated)
                //    SmallWaitForm.CloseS = true;

                NewOrderInfo.IsNewOrder = true;
            }
            OrdersManager.MoveToDocNumber(MainOrderID);
            NeedSplash = true;
        }

        private void MainOrdersRemoveMainOrder_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MainOrdersBindingSource.Count == 0)
                return;

            int MainOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]);

            if (OrdersManager.IsOrderPackAlloc(Convert.ToInt32(MainOrderID)))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Невозможно удалить заказ, так как он уже упакован",
                    "Ошибка удаления");

                return;
            }

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы действительно хотите удалить заказ " + MainOrderID + " ?",
                "Удаление заказа");

            if (!OKCancel)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление заказа.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            OrdersManager.RemoveCurrentMainOrder(MainOrderID);

            if (OrdersManager.MainOrdersBindingSource.Count > 0)
                OrdersManager.Filter(Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]));

            OrdersManager.SummaryCalcCurrentMegaOrder();

            bool bDebts = DebtsCB.Checked;
            bool bDoNotDisp = DoNotDispatchCB.Checked;
            bool bToAssembly = cbToAssembly.Checked;
            bool bFromAssembly = cbFromAssembly.Checked;
            bool bIsNotPaid = cbIsNotPaid.Checked;
            bool bTechDrilling = cbxTechDrilling.Checked;
            bool bQuicklyOrder = cbxQuicklyOrder.Checked;
            bool bDoubleOrder = cbxNotDoubleOrders.Checked;

            OrdersManager.UpdateMainOrders(ProfilCheckBox.Checked, TPSCheckBox.Checked, bDebts, bDoNotDisp, bTechDrilling, bQuicklyOrder, bDoubleOrder, bToAssembly, bFromAssembly, bIsNotPaid);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
            InfiniumTips.ShowTip(this, 50, 85, "Заказ удалён", 1700);
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton == OrdersSelectButton)
            {
                OrdersToolsPanel.BringToFront();
                OrdersClientAreaPanel.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == PaymentsSelectButton)
            {
                if (PaymentWeeksButton.Checked)
                {
                    PaymentWeeksClientAreaPanel.BringToFront();
                    PaymentWeeksPaymentsPanel.BringToFront();
                }
                else
                {
                    PaymentNewWeekClientAreaPanel.BringToFront();
                    PaymentWeeksNewPaymentPanel.BringToFront();
                }

                PaymentsToolsPanel.BringToFront();
                //OrdersTableLayoutPanel.Visible = false;
                //PaymentsTableLayoutPanel.Visible = true;
            }
        }

        private void PaymentWeeksAddNewOrderButton_Click(object sender, EventArgs e)
        {
            if (PaymentWeeksSamplesCheckBox.Checked == false && PaymentWeeksDebtCheckBox.Checked == false)
                return;

            if (PaymentWeeksDebtCheckBox.Checked == false && (PaymentWeeksSamplesCostTextEdit.Text.Length == 0 || PaymentWeeksSamplesCostTextEdit.Text == "0"))
                return;

            string ClientName = string.Empty;
            string DebtType = string.Empty;
            DateTime DateFrom = default(DateTime);
            DateTime DateTo = default(DateTime);
            DateTime DispatchDate = default(DateTime);
            decimal Cost = 0;
            bool IsDocNumberExistInPayments = OrdersManager.IsDocNumberExistInPayments(PaymentWeekDocNumberComboBox.Text,
                ref ClientName, ref DateFrom, ref DateTo, ref DebtType, ref Cost, ref DispatchDate);

            if (IsDocNumberExistInPayments)
            {
                NumberFormatInfo nfi1 = new NumberFormatInfo()
                {
                    NumberGroupSeparator = " ",
                    NumberDecimalDigits = 2
                };
                string str = "Эта кухня уже выставлялась в расчет, всё равно продолжить?" + "\n" +
                    "Клиент: " + ClientName + "\n" +
                    "Расчетная неделя: " + DateFrom.ToString("dd.MM.yyyy") + "-" + DateTo.ToString("dd.MM.yyyy") + "\n" +
                    DebtType + ": " + Cost.ToString("N", nfi1) + " €" + "\n" +
                    "Дата отгрузки: " + DateFrom.ToString("dd.MM.yyyy");
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    str,
                    "Добавление кухни в расчет");

                if (!OKCancel)
                    return;
            }

            decimal Samples = 0;

            int DebtTypeID = 0;

            if (PaymentWeeksSamplesCheckBox.Checked)
                Samples = Convert.ToDecimal(PaymentWeeksSamplesCostTextEdit.Text);
            else
                Samples = 0;

            if (PaymentWeeksDebtCheckBox.Checked)
                DebtTypeID = Convert.ToInt32(PaymentWeeksDebtTypesComboBox.SelectedValue);

            PaymentWeeks.AddNewOrder(PaymentWeekDocNumberComboBox.Text, Samples, DebtTypeID);
        }

        private void PaymentWeeksAddRemoveOrderButton_Click(object sender, EventArgs e)
        {
            PaymentWeeks.RemoveCurrentTempDoc();
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (!PaymentsSelectButton.Checked)
                return;

            if (kryptonCheckSet2.CheckedButton == PaymentWeeksButton)
            {
                PaymentWeeksClientAreaPanel.BringToFront();
                PaymentWeeksPaymentsPanel.BringToFront();
            }
            if (kryptonCheckSet2.CheckedButton == PaymentWeeksAddButton)
            {
                PaymentWeeksNewPaymentPanel.BringToFront();
                PaymentNewWeekClientAreaPanel.BringToFront();
            }
        }

        private void PaymentWeeksDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PaymentWeeksDataGrid.RowCount == 0 || PaymentWeeks == null)
                return;

            PaymentWeeks.FilterWeek();
        }

        private void PaymentWeeksDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (PaymentWeeksDataGrid.RowCount == 0 || PaymentWeeks == null)
                return;

            PaymentWeeks.FilterWeek();
        }

        private void NewPaymentWeek_Click(object sender, EventArgs e)
        {
            PaymentWeeks.NewPaymentWeek();
        }

        private void PaymentWeeksSavePaymentWeekButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение расчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PaymentWeeks.DispatchMegaOrders(PaymentWeeksDateFromPicker.Value, PaymentWeeksDateToPicker.Value);
            sw.Stop();
            double G1 = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            PaymentWeeks.Save(PaymentWeeksDateFromPicker.Value, PaymentWeeksDateToPicker.Value, Convert.ToDecimal(ErrorWriteOffTextBox.Text),
                              Convert.ToDecimal(CompensationTextBox.Text));
            sw.Stop();
            double G2 = sw.Elapsed.TotalMilliseconds;

            PaymentWeeks.NewPaymentWeek();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
            InfiniumTips.ShowTip(this, 50, 85, "Расчет сохранен", 1700);
        }

        private void RemoveCurrentPaymentWeek_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы действительно хотите удалить расчетную неделю?",
                "Удаление заказа");

            if (!OKCancel)
                return;

            PaymentWeeks.RemoveCurrentPaymentWeek();
            InfiniumTips.ShowTip(this, 50, 85, "Расчетная неделя удалена", 1700);
        }

        private void PaymentWeeksSamplesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PaymentWeeksSamplesCostTextEdit.Enabled = PaymentWeeksSamplesCheckBox.Checked;
        }

        private void PaymentWeeksDebtCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PaymentWeeksDebtTypesComboBox.Enabled = PaymentWeeksDebtCheckBox.Checked;
        }

        private void ReportContextMenu_Click(object sender, EventArgs e)
        {
            OrdersManager.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            bool Curved = false;
            bool Aluminium = false;
            bool ArchDecor = false;
            bool Glass = false;
            bool Hands = false;

            System.Collections.ArrayList ProductIDs = new System.Collections.ArrayList();

            ReportFilterForm ReportFilterForm = new ReportFilterForm(ref ProductIDs);

            TopForm = ReportFilterForm;
            ReportFilterForm.ShowDialog();
            TopForm = null;

            Curved = ReportFilterForm.Curved;
            Aluminium = ReportFilterForm.Aluminium;
            ArchDecor = ReportFilterForm.ArchDecor;
            Glass = ReportFilterForm.Glass;
            Hands = ReportFilterForm.Hands;

            PhantomForm.Close();

            PhantomForm.Dispose();

            if (ReportFilterForm.IsOKPress)
            {
                ReportFilterForm.Dispose();
                ReportFilterForm = null;
                GC.Collect();

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                Modules.ZOV.ZOVProductionReport ZOVProductionReport = new Modules.ZOV.ZOVProductionReport(ref DecorCatalogOrder);
                System.Collections.ArrayList array = new System.Collections.ArrayList();

                int FactoryID = 0;

                if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                    FactoryID = 1;
                if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                    FactoryID = 2;
                if (!ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                    FactoryID = -1;

                array = OrdersManager.GetMainOrders(MegaOrderID, FactoryID, Curved, Aluminium, ProductIDs);
                if (array.Count > 0)
                {
                    int[] MainOrders = array.OfType<int>().Distinct().ToArray();
                    ZOVProductionReport.CreateReport(MainOrders, FactoryID, Curved, Aluminium, Glass, Hands, ProductIDs);
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                NeedSplash = true;
            }
            else
            {
                ReportFilterForm.Dispose();
                ReportFilterForm = null;
                GC.Collect();
            }
        }

        private void MegaOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void ProfilNotInProduction_Click(object sender, EventArgs e)
        {
            System.Collections.ArrayList array = new System.Collections.ArrayList();

            OrdersManager.MegaOrdersBindingSource.Position = CurrentRowIndex;

            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            array = OrdersManager.GetMainOrders(MegaOrderID, 1, true);
            if (array.Count < 1)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Modules.ZOV.ZOVProductionReport ZOVProductionReport = new Modules.ZOV.ZOVProductionReport(ref DecorCatalogOrder);

            if (array.Count > 0)
            {
                int[] MainOrders = array.OfType<int>().ToArray();
                ZOVProductionReport.NotInProductionReport(MainOrders, 1);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void TPSNotInProduction_Click(object sender, EventArgs e)
        {
            System.Collections.ArrayList array = new System.Collections.ArrayList();

            OrdersManager.MegaOrdersBindingSource.Position = CurrentRowIndex;
            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]);
            array = OrdersManager.GetMainOrders(MegaOrderID, 2, true);
            if (array.Count < 1)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Modules.ZOV.ZOVProductionReport ZOVProductionReport = new Modules.ZOV.ZOVProductionReport(ref DecorCatalogOrder);

            if (array.Count > 0)
            {
                int[] MainOrders = array.OfType<int>().ToArray();
                ZOVProductionReport.NotInProductionReport(MainOrders, 2);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            int FactoryID = 1;

            //if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
            //    FactoryID = 1;
            //if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
            //    FactoryID = 2;
            //if (!ProfilCheckBox.Checked && !TPSCheckBox.Checked)
            //    FactoryID = -1;

            //if (FactoryID == -1)
            //    return;

            System.Collections.ArrayList array = new System.Collections.ArrayList();

            array = OrdersManager.GetSelectedMegaOrders();
            if (array.Count < 1)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Modules.ZOV.ZOVProductionReport ZOVProductionReport = new Modules.ZOV.ZOVProductionReport(ref DecorCatalogOrder);

            if (array.Count > 0)
            {
                int[] MegaOrders = array.OfType<int>().ToArray();
                int[] MainOrders = OrdersManager.GetMainOrders(MegaOrders, FactoryID).OfType<int>().ToArray();
                if (MainOrders.Count() > 0)
                    ZOVProductionReport.DepersonalizedReport(MainOrders, FactoryID);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            int FactoryID = 2;

            System.Collections.ArrayList array = new System.Collections.ArrayList();

            array = OrdersManager.GetSelectedMegaOrders();
            if (array.Count < 1)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Modules.ZOV.ZOVProductionReport ZOVProductionReport = new Modules.ZOV.ZOVProductionReport(ref DecorCatalogOrder);

            if (array.Count > 0)
            {
                int[] MegaOrders = array.OfType<int>().ToArray();
                int[] MainOrders = OrdersManager.GetMainOrders(MegaOrders, FactoryID).OfType<int>().ToArray();
                if (MainOrders.Count() > 0)
                    ZOVProductionReport.DepersonalizedReport(MainOrders, FactoryID);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void MainOrderReport_Click(object sender, EventArgs e)
        {
            int FactoryID = 0;

            if (ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = 1;
            if (!ProfilCheckBox.Checked && TPSCheckBox.Checked)
                FactoryID = 2;
            if (!ProfilCheckBox.Checked && !TPSCheckBox.Checked)
                FactoryID = -1;

            if (FactoryID == -1)
                return;

            System.Collections.ArrayList array = new System.Collections.ArrayList();

            array = OrdersManager.GetSelectedMainOrders();
            if (array.Count < 1)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Modules.ZOV.ZOVProductionReport ZOVProductionReport = new Modules.ZOV.ZOVProductionReport(ref DecorCatalogOrder);

            if (array.Count > 0)
            {
                int[] MainOrders = array.OfType<int>().ToArray();
                ZOVProductionReport.MainOrderReport(MainOrders, FactoryID);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void Filter()
        {
            if (OrdersManager == null)
                return;

            bool FilterByDispDate = DispatchDateFilterCheckBox.Checked;
            bool bDebts = DebtsCB.Checked;
            bool bDoNotDisp = DoNotDispatchCB.Checked;
            bool bTechDrilling = cbxTechDrilling.Checked;
            bool bQuicklyOrder = cbxQuicklyOrder.Checked;
            bool bDoubleOrder = cbxNotDoubleOrders.Checked;
            bool bToAssembly = cbToAssembly.Checked;
            bool bFromAssembly = cbFromAssembly.Checked;
            bool bIsNotPaid = cbIsNotPaid.Checked;
            DateTime DateFrom = FilterFromDateTimePicker.Value;
            DateTime DateTo = FilterToDateTimePicker.Value;

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                OrdersManager.FilterMegaOrders(ProfilCheckBox.Checked, TPSCheckBox.Checked,
                    FilterByDispDate, DateFrom, DateTo, bDebts, bDoNotDisp, bTechDrilling, bQuicklyOrder, bDoubleOrder, bToAssembly, bFromAssembly, bIsNotPaid);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                OrdersManager.FilterMegaOrders(ProfilCheckBox.Checked, TPSCheckBox.Checked,
                    FilterByDispDate, DateFrom, DateTo, bDebts, bDoNotDisp, bTechDrilling, bQuicklyOrder, bDoubleOrder, bToAssembly, bFromAssembly, bIsNotPaid);
            }

            ProfilNotInProduction.Visible = ProfilCheckBox.Checked;
            kryptonContextMenuItem1.Visible = ProfilCheckBox.Checked;
            TPSNotInProduction.Visible = TPSCheckBox.Checked;
            kryptonContextMenuItem2.Visible = TPSCheckBox.Checked;
        }

        private void ProfilCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void TPSCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void DocNumbersComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (DocNumbersComboBox.Items.Count > 0)
                {
                    OrdersManager.FindDocNumber(DocNumbersComboBox.SelectedValue.ToString());
                }
            }
        }

        private void DocNumbersComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (DocNumbersComboBox.Items.Count > 0)
            {
                OrdersManager.FindDocNumber(DocNumbersComboBox.SelectedValue.ToString());
            }
        }

        private void MainOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && SearchTextBox.Text.Length > 0)
                OrdersManager.SearchPartDocNumber(SearchTextBox.Text);
        }

        private void SearchPartDocNumberComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (SearchPartDocNumberComboBox.Items.Count > 0)
                OrdersManager.SearchDocNumber(Convert.ToInt32(SearchPartDocNumberComboBox.SelectedValue));
        }

        private void SearchPartDocNumberComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (OrdersManager == null)
            //    return;

            //if (SearchPartDocNumberComboBox.Items.Count > 0)
            //    OrdersManager.SearchDocNumber(Convert.ToInt32(SearchPartDocNumberComboBox.SelectedValue));
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            Filter();
        }

        private void ZOVOrdersForm_Load(object sender, EventArgs e)
        {
            Filter();
        }

        private void SearchDocNumberCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SearchDocNumberCheckBox.Checked)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                DocNumbersComboBox.Enabled = true;
                SearchTextBox.Enabled = true;
                SearchPartDocNumberComboBox.Enabled = true;

                if (DocNumbersComboBox.DataSource == null)
                {
                    DocNumbersComboBox.DataSource = OrdersManager.DocNumbersBindingSource;
                    DocNumbersComboBox.DisplayMember = "DocNumber";
                    DocNumbersComboBox.ValueMember = "DocNumber";

                    SearchPartDocNumberComboBox.DataSource = OrdersManager.SearchPartDocNumberBindingSource;
                    SearchPartDocNumberComboBox.DisplayMember = "DocNumber";
                    SearchPartDocNumberComboBox.ValueMember = "MainOrderID";
                }

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                DocNumbersComboBox.Enabled = false;
                SearchTextBox.Enabled = false;
                SearchPartDocNumberComboBox.Enabled = false;
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            //NeedSplash = false;

            //NewOrderInfo.OrderStatus = Modules.OrdersZOV.NewOrderInfo.OrderStatusType.NewOrder;
            //NewOrderInfo.IsNewOrder = false;

            //Thread T = new Thread(delegate() { SplashWindow.CreateSplash(); });
            //T.Start();

            //while (!SplashForm.bCreated) ;

            //AddZOVMainOrdersForm AddMainOrdersForm = new AddZOVMainOrdersForm(TM, ref OrdersManager, NewOrderInfo, false, ref TopForm, ref OrdersCalculate);

            //TopForm = AddMainOrdersForm;

            //AddMainOrdersForm.ShowDialog();

            //AddMainOrdersForm.Close();
            //AddMainOrdersForm.Dispose();

            //TopForm = null;

            ////T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            ////T.Start();

            ////while (!SplashWindow.bSmallCreated) ;

            //Filter();

            ////while (SplashWindow.bSmallCreated)
            ////    SmallWaitForm.CloseS = true;

            //NeedSplash = true;
        }

        private void MainOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //bool NeedPaintRow = Convert.ToInt32(MainOrdersDataGrid.Rows[e.RowIndex].Cells["DoubleOrder"].Value) > 0 ? false : true;
            //if (cbxNeedPaintRow.Checked && NeedPaintRow)
            //{
            //    // Calculate the bounds of the row 
            //    int rowHeaderWidth = MainOrdersDataGrid.RowHeadersVisible ?
            //                         MainOrdersDataGrid.RowHeadersWidth : 0;
            //    Rectangle rowBounds = new Rectangle(
            //        rowHeaderWidth,
            //        e.RowBounds.Top,
            //        MainOrdersDataGrid.Columns.GetColumnsWidth(
            //                DataGridViewElementStates.Visible) -
            //                MainOrdersDataGrid.HorizontalScrollingOffset + 1,
            //       e.RowBounds.Height);

            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
            //        Color.Red;
            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
            //        Color.White;
            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
            //       Color.FromArgb(31, 158, 0);
            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
            //        Color.White;
            //}
            //else
            //{
            //    // Calculate the bounds of the row 
            //    int rowHeaderWidth = MainOrdersDataGrid.RowHeadersVisible ?
            //                         MainOrdersDataGrid.RowHeadersWidth : 0;
            //    Rectangle rowBounds = new Rectangle(
            //        rowHeaderWidth,
            //        e.RowBounds.Top,
            //        MainOrdersDataGrid.Columns.GetColumnsWidth(
            //                DataGridViewElementStates.Visible) -
            //                MainOrdersDataGrid.HorizontalScrollingOffset + 1,
            //       e.RowBounds.Height);

            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
            //        Security.GridsBackColor;
            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
            //        Color.Black;
            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
            //        Color.FromArgb(31, 158, 0);
            //    MainOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
            //        Color.White;
            //}
        }

        private void kryptonButton1_Click_2(object sender, EventArgs e)
        {
            int MegaOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            OrdersManager.DyeingOrdersToExcel(OrdersManager.GetReadyDyeingOrders(0));

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Формирование отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NPOI.HSSF.UserModel.HSSFWorkbook hssfworkbook = new NPOI.HSSF.UserModel.HSSFWorkbook();
            InvoiceReportToDBF InvoiceReportToDBF = new InvoiceReportToDBF(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);
            Report Report = new Modules.ZOV.ReportToDBF.Report(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);

            DateTime DispatchDate = DateTime.Now;
            int[] MainOrders = new int[MainOrdersDataGrid.SelectedRows.Count];
            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                MainOrders[i] = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);
            if (MegaOrdersDataGrid.SelectedRows[0].Cells["DispatchDate"].Value != DBNull.Value)
                DispatchDate = Convert.ToDateTime(MegaOrdersDataGrid.SelectedRows[0].Cells["DispatchDate"].Value);
            InvoiceReportToDBF.CreateReport(ref hssfworkbook, DispatchDate, MainOrders, 1);
            Report.CreateReport(ref hssfworkbook, DispatchDate, MainOrders, 1);

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            string ReportName = "Отгрузка " + DispatchDate.ToString("dd.MM.yyyy");
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
        }

        private void btnMoveToMarketing_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int NewClientID = 0;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ReaddressMarketingDispatchMenu ReaddressMarketingDispatchMenu = new ReaddressMarketingDispatchMenu(this, OrdersManager.ReturnMarketingClients);
            TopForm = ReaddressMarketingDispatchMenu;
            ReaddressMarketingDispatchMenu.ShowDialog();

            PressOK = ReaddressMarketingDispatchMenu.PressOK;
            NewClientID = ReaddressMarketingDispatchMenu.ClientID;

            PhantomForm.Close();
            PhantomForm.Dispose();
            ReaddressMarketingDispatchMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;


            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Формирование отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Infinium.Modules.ZOV.Orders.MoveZOVOrdersToMarketing obj = new Modules.ZOV.Orders.MoveZOVOrdersToMarketing();
            bool FixedPaymentRate = false;

            obj.MarketingClientID = NewClientID;
            obj.GetFixedPaymentRate(obj.MarketingClientID, DateTime.Now, ref FixedPaymentRate);
            if (FixedPaymentRate)
            {
                obj.ClearZOVMainOrdersInfo();
                for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                {
                    obj.AddZOVMainOrder(Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["ClientID"].Value), Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value),
                        MainOrdersDataGrid.SelectedRows[i].Cells["DocDateTime"].Value,
                        MainOrdersDataGrid.SelectedRows[i].Cells["ClientColumn"].FormattedValue.ToString(),
                        MainOrdersDataGrid.SelectedRows[i].Cells["DocNumber"].Value.ToString());
                }
                obj.MoveToOthersOrders();
            }
            else
            {
                bool RateExist = false;
                obj.GetDateRates(DateTime.Now, ref RateExist);
                if (!RateExist)
                    RateExist = obj.NBRBDailyRates(DateTime.Now);
                if (RateExist)
                {
                    obj.ClearZOVMainOrdersInfo();
                    for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                    {
                        obj.AddZOVMainOrder(Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["ClientID"].Value), Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value),
                            MainOrdersDataGrid.SelectedRows[i].Cells["DocDateTime"].Value,
                            MainOrdersDataGrid.SelectedRows[i].Cells["ClientColumn"].FormattedValue.ToString(),
                            MainOrdersDataGrid.SelectedRows[i].Cells["DocNumber"].Value.ToString());
                    }
                    obj.MoveToOthersOrders();
                }
                else
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Курс не взят. Повторите попытку",
                        "Перенос заказов");
                }
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int NewClientID = 0;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ReaddressMarketingDispatchMenu ReaddressMarketingDispatchMenu = new ReaddressMarketingDispatchMenu(this, OrdersManager.ReturnMarketingClients);
            TopForm = ReaddressMarketingDispatchMenu;
            ReaddressMarketingDispatchMenu.ShowDialog();

            PressOK = ReaddressMarketingDispatchMenu.PressOK;
            NewClientID = ReaddressMarketingDispatchMenu.ClientID;

            PhantomForm.Close();
            PhantomForm.Dispose();
            ReaddressMarketingDispatchMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;


            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Формирование отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Infinium.Modules.ZOV.Orders.MoveZOVOrdersToMarketing obj = new Modules.ZOV.Orders.MoveZOVOrdersToMarketing();
            bool FixedPaymentRate = false;
            obj.MarketingClientID = NewClientID;
            obj.GetFixedPaymentRate(obj.MarketingClientID, DateTime.Now, ref FixedPaymentRate);
            if (FixedPaymentRate)
            {
                obj.ClearZOVMainOrdersInfo();
                for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                {
                    obj.AddZOVMainOrder(Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["ClientID"].Value), Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value),
                        MainOrdersDataGrid.SelectedRows[i].Cells["DocDateTime"].Value,
                        MainOrdersDataGrid.SelectedRows[i].Cells["ClientColumn"].FormattedValue.ToString(),
                        MainOrdersDataGrid.SelectedRows[i].Cells["DocNumber"].Value.ToString());
                }
                obj.MoveToOneOrder();
            }
            else
            {
                bool RateExist = false;
                obj.GetDateRates(DateTime.Now, ref RateExist);
                if (!RateExist)
                    RateExist = obj.NBRBDailyRates(DateTime.Now);
                if (RateExist)
                {
                    obj.ClearZOVMainOrdersInfo();
                    for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                    {
                        obj.AddZOVMainOrder(Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["ClientID"].Value), Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value),
                            MainOrdersDataGrid.SelectedRows[i].Cells["DocDateTime"].Value,
                            MainOrdersDataGrid.SelectedRows[i].Cells["ClientColumn"].FormattedValue.ToString(),
                            MainOrdersDataGrid.SelectedRows[i].Cells["DocNumber"].Value.ToString());
                    }
                    obj.MoveToOneOrder();
                }
                else
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Курс не взят. Возможная причина - нет связи с банком без авторизации в kerio control. Войдите в kerio и повторите попытку",
                        "Перенос заказов");
                }
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
