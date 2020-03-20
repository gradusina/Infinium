using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.Packages;

using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingSplitOrdersForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int MegaOrderID = 1;
        int OldMainOrderID = 1;
        int FactoryID = 1;
        int FormEvent = 0;
        int PackType = 0;

        SplitOrdersForm SplitOrdersForm = null;
        Form MainForm = null;
        Form TopForm = null;

        MarketingSplitMainOrders MarketingSplitMainOrders;
        OrdersCalculate OrdersCalculate;

        public MarketingSplitOrdersForm(Form tMainForm, int iMegaOrderID, int iMainOrderID, int iFactoryID)
        {
            InitializeComponent();

            MainForm = tMainForm;

            MegaOrderID = iMegaOrderID;
            OldMainOrderID = iMainOrderID;
            FactoryID = iFactoryID;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void MarketingSplitOrdersForm_Shown(object sender, EventArgs e)
        {
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
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

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            MarketingSplitMainOrders = new MarketingSplitMainOrders(
                ref MainOrdersFrontsOrdersDataGrid,
                ref MainOrdersDecorOrdersDataGrid, ref MainOrdersTabControl)
            {
                Factory = FactoryID,
                MegaOrder = MegaOrderID,
                MainOrder = OldMainOrderID
            };
            MarketingSplitMainOrders.Filter();

            OrdersCalculate = new OrdersCalculate();
        }


        private void MainOrdersFrontsOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            PackType = 0;
        }

        private void MainOrdersDecorOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            PackType = 1;
        }

        private void MoveToOrderButton_Click(object sender, EventArgs e)
        {
            if (!MarketingSplitMainOrders.AreFrontsChecked && !MarketingSplitMainOrders.AreDecorChecked)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if ((MainOrdersTabControl.SelectedTabPage == MainOrdersTabControl.TabPages[0] || PackType == 0) &&
                MarketingSplitMainOrders.FrontsOrdersBindingSource.Count > 0 && MarketingSplitMainOrders.AreFrontsChecked)
            {
                MarketingSplitMainOrders.CreateNewMainOrder();
                MarketingSplitMainOrders.ChangeFrontsMainOrder();
                MarketingSplitMainOrders.Filter();
            }

            if ((MainOrdersTabControl.SelectedTabPage == MainOrdersTabControl.TabPages[1] || PackType == 1) &&
                MarketingSplitMainOrders.DecorOrdersBindingSource.Count > 0 && MarketingSplitMainOrders.AreDecorChecked)
            {
                MarketingSplitMainOrders.CreateNewMainOrder();
                MarketingSplitMainOrders.ChangeDecorMainOrder();
                MarketingSplitMainOrders.Filter();
            }

            MarketingSplitMainOrders.SetMainOrderFactory(OldMainOrderID);
            MarketingSplitMainOrders.SetMainOrderFactory(MarketingSplitMainOrders.NewMainOrder);

            decimal DiscountPaymentCondition = 0;
            decimal ProfilDiscountDirector = 0;
            decimal TPSDiscountDirector = 0;
            decimal ProfilTotalDiscount = 0;
            decimal TPSTotalDiscount = 0;
            int CurrencyTypeID = 0;
            decimal PaymentRate = 0;
            object ConfirmDateTime = DBNull.Value;
            OrdersCalculate.GetMegaOrderDiscount(MegaOrderID, ref CurrencyTypeID, ref PaymentRate, ref ProfilDiscountDirector, ref TPSDiscountDirector, ref ProfilTotalDiscount, ref TPSTotalDiscount, ref DiscountPaymentCondition, ref ConfirmDateTime);
            OrdersCalculate.CalculateOrder(MegaOrderID, OldMainOrderID, ProfilDiscountDirector, TPSDiscountDirector, ProfilTotalDiscount, TPSTotalDiscount, DiscountPaymentCondition, CurrencyTypeID, PaymentRate, ConfirmDateTime);
            OrdersCalculate.CalculateOrder(MegaOrderID, MarketingSplitMainOrders.NewMainOrder, ProfilDiscountDirector, TPSDiscountDirector, ProfilTotalDiscount, TPSTotalDiscount, DiscountPaymentCondition, CurrencyTypeID, PaymentRate, ConfirmDateTime);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void SplitOrderButton_Click(object sender, EventArgs e)
        {
            if ((MainOrdersTabControl.SelectedTabPage == MainOrdersTabControl.TabPages[0] || PackType == 0) &&
                MarketingSplitMainOrders.FrontsOrdersBindingSource.Count > 0)
            {
                int FrontsOrdersID = Convert.ToInt32(((DataRowView)MarketingSplitMainOrders.FrontsOrdersBindingSource.Current)["FrontsOrdersID"]);
                int TotalFrontsCount = Convert.ToInt32(((DataRowView)MarketingSplitMainOrders.FrontsOrdersBindingSource.Current)["Count"]);

                Infinium.Modules.Packages.SplitOrders SplitOrders = new Modules.Packages.SplitOrders()
                {
                    TotalCount = TotalFrontsCount
                };
                SplitOrdersForm = new SplitOrdersForm(this, ref SplitOrders);

                TopForm = SplitOrdersForm;
                SplitOrdersForm.ShowDialog();
                TopForm = null;

                SplitOrdersForm.Dispose();
                SplitOrdersForm = null;
                GC.Collect();

                if (SplitOrders.IsSplit)
                {
                    MarketingSplitMainOrders.CreateNewFrontsOrder(SplitOrders, FrontsOrdersID);

                    decimal DiscountPaymentCondition = 0;
                    decimal ProfilDiscountDirector = 0;
                    decimal TPSDiscountDirector = 0;
                    decimal ProfilTotalDiscount = 0;
                    decimal TPSTotalDiscount = 0;
                    int CurrencyTypeID = 0;
                    decimal PaymentRate = 0;
                    object ConfirmDateTime = DBNull.Value;
                    OrdersCalculate.GetMegaOrderDiscount(MegaOrderID, ref CurrencyTypeID, ref PaymentRate, ref ProfilDiscountDirector, ref TPSDiscountDirector, ref ProfilTotalDiscount, ref TPSTotalDiscount, ref DiscountPaymentCondition, ref ConfirmDateTime);
                    OrdersCalculate.CalculateOrder(MegaOrderID, OldMainOrderID, ProfilDiscountDirector, TPSDiscountDirector, ProfilTotalDiscount, TPSTotalDiscount, DiscountPaymentCondition, CurrencyTypeID, PaymentRate, ConfirmDateTime);
                    MarketingSplitMainOrders.Filter();
                }
            }

            if ((MainOrdersTabControl.SelectedTabPage == MainOrdersTabControl.TabPages[1] || PackType == 1) &&
                MarketingSplitMainOrders.DecorOrdersBindingSource.Count > 0)
            {
                int DecorOrderID = Convert.ToInt32(((DataRowView)MarketingSplitMainOrders.DecorOrdersBindingSource.Current)["DecorOrderID"]);
                int OldDecorCount = Convert.ToInt32(((DataRowView)MarketingSplitMainOrders.DecorOrdersBindingSource.Current)["Count"]);

                Infinium.Modules.Packages.SplitOrders SplitOrders = new Modules.Packages.SplitOrders()
                {
                    TotalCount = OldDecorCount
                };
                SplitOrdersForm = new SplitOrdersForm(this, ref SplitOrders);

                TopForm = SplitOrdersForm;
                SplitOrdersForm.ShowDialog();
                TopForm = null;

                SplitOrdersForm.Dispose();
                SplitOrdersForm = null;
                GC.Collect();

                if (SplitOrders.IsSplit)
                {
                    MarketingSplitMainOrders.CreateNewDecorOrder(SplitOrders, DecorOrderID);

                    decimal DiscountPaymentCondition = 0;
                    decimal ProfilDiscountDirector = 0;
                    decimal TPSDiscountDirector = 0;
                    decimal ProfilTotalDiscount = 0;
                    decimal TPSTotalDiscount = 0;
                    int CurrencyTypeID = 0;
                    decimal PaymentRate = 0;
                    object ConfirmDateTime = DBNull.Value;
                    OrdersCalculate.GetMegaOrderDiscount(MegaOrderID, ref CurrencyTypeID, ref PaymentRate, ref ProfilDiscountDirector, ref TPSDiscountDirector, ref ProfilTotalDiscount, ref TPSTotalDiscount, ref DiscountPaymentCondition, ref ConfirmDateTime);
                    OrdersCalculate.CalculateOrder(MegaOrderID, OldMainOrderID, ProfilDiscountDirector, TPSDiscountDirector, ProfilTotalDiscount, TPSTotalDiscount, DiscountPaymentCondition, CurrencyTypeID, PaymentRate, ConfirmDateTime);
                    MarketingSplitMainOrders.Filter();
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

    }
}