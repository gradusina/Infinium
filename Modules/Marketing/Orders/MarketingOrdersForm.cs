using Infinium.Modules.Marketing.Orders;

using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingOrdersForm : Form
    {
        const int iAdmin = 79;
        const int iMarketing = 78;

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
        AddMarketingOrdersForm AddMainOrdersForm;
        ClientReportMenu ClientReportMenu;
        Report Report;
        DetailsReport DetailsReport;
        SendEmail SendEmail;

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

        public MarketingOrdersForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            RolePermissionsDataTable = OrdersManager.GetPermissions(Security.CurrentUserID, this.Name);

            if (!PermissionGranted(iMarketing) && !PermissionGranted(iAdmin))
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
                RoleType = RoleTypes.Admin;
            }

            while (!SplashForm.bCreated) ;
        }

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
            DecorCatalogOrder = new DecorCatalogOrder();

            OrdersManager = new OrdersManager(ref MainOrdersDataGrid, ref MainOrdersFrontsOrdersDataGrid, ref MegaOrdersDataGrid,
                ref MainOrdersDecorTabControl, ref MainOrdersTabControl, ref DecorCatalogOrder);
            OrdersCalculate = new OrdersCalculate(OrdersManager);
            Report = new Report(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);
            DetailsReport = new DetailsReport(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);
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
                return;
            Filter();
        }

        private void FilterClientsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (OrdersManager != null && ClientCheckBox.Checked)
                Filter();
        }

        private void MainOrdersEditOrder_Click(object sender, EventArgs e)
        {
            //OrdersManager.NeedSetStatus = true;
            if (OrdersManager.MainOrdersBindingSource.Count > 0)
            {
                NeedSplash = false;

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
                if (OrdersManager.MainOrdersBindingSource.Count > 0)
                    OrdersManager.CurrentMainOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current).Row["MainOrderID"]);

                bool bDeleteEnable = false;
                if (RoleType == RoleTypes.Admin)
                    bDeleteEnable = true;
                AddMainOrdersForm = new AddMarketingOrdersForm(ref OrdersManager, ref TopForm, ref OrdersCalculate, bDeleteEnable);

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

                //decimal CurrencyTotalCost = DBFReport.CalcCurrencyCost(
                //    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]),
                //    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]), OrdersManager.PaymentCurrency);
                //OrdersManager.SetCurrencyCost(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), CurrencyTotalCost);

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

        private void SendReportButton_Click(object sender, EventArgs e)
        {
            if (OrdersManager.MainOrdersBindingSource.Count == 0)
                return;

            DetailsReport.Save = false;
            DetailsReport.Send = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
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

            if (!DetailsReport.Save && !DetailsReport.Send)
            {
                return;
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
            int CurrencyTypeID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["CurrencyTypeID"]);
            decimal Rate = Convert.ToDecimal(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["PaymentRate"]);
            int OrderNumber = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["OrderNumber"]);
            int ClientID = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]);

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
            {
                ComplaintProfilCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["ComplaintProfilCost"].Value);
                ComplaintTPSCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["ComplaintTPSCost"].Value);
                TransportCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["TransportCost"].Value);
                AdditionalCost += Convert.ToDecimal(MegaOrdersDataGrid.SelectedRows[i].Cells["AdditionalCost"].Value);
            }

            //CheckMainOrdersForm = new CheckMainOrdersForm(this, MainOrders, ref DecorCatalogOrder);

            //TopForm = CheckMainOrdersForm;
            //CheckMainOrdersForm.ShowDialog();

            //int[] CheckedMainOrders = CheckMainOrdersForm.CheckedMainOrders;
            //int[] CheckedOrderNumbers = CheckMainOrdersForm.CheckedOrderNumbers;

            //PhantomForm.Close();

            //PhantomForm.Dispose();
            //CheckMainOrdersForm.Dispose();
            //TopForm = null;

            //if (!CheckMainOrdersForm.IsChecked)
            //    return;

            //REPORT

            string ClientName = OrdersManager.GetClientName(ClientID);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int[] SelectedOrderNumbers = OrdersManager.GetSelectedOrderNumbers();
            string FileName = DetailsReport.Report(SelectedMegaOrders, SelectedOrderNumbers, MainOrders, ClientID, ClientName,
                ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost,
                OrdersManager.GetMainOrdersCost(MainOrders), CurrencyTypeID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;

            if (!DetailsReport.Send)
                return;

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
                //MessageBox.Show(ExcMessage);
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Отправка отчета невозможна: отсутствует подключение к интернету либо адрес электронной почты указан неверно",
                       "Отправка письма");
            }

            if (!DetailsReport.Save)
                SendEmail.DeleteFile(FileName);
        }

        public void GetCurrency(DateTime Date)
        {
            bool RateExist = false;
            //string CbrBankData = OrdersManager.GetCbrBankData(Date);
            //string NbrbBankData = OrdersManager.GetNbrbBankData(Date);

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

            //if (CurrencyEURRUBTextBox.Text == "0" || CurrencyUSDRUBTextBox.Text == "0" || CurrencyEURBYRTextBox.Text == "0")
            //{
            //    CurrencyNoDataPanel.Visible = true;
            //    return;
            //}
            //else
            //    CurrencyNoDataPanel.Visible = false;

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
            }
        }

        private void MainOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
            }
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
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
            else
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
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
    }
}
