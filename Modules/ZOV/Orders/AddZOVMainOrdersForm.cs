using Infinium.Modules.ZOV;

using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddZOVMainOrdersForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form TopForm = null;

        bool EditMainOrder = false;

        public OrdersManager OrdersManager;

        FrontsCatalogOrder FrontsCatalogOrder = null;
        DecorCatalogOrder DecorCatalogOrder = null;
        FrontsOrders FrontsOrders;
        DecorOrders DecorOrders;
        OrdersCalculate OrdersCalculate;
        NewOrderInfo NewOrderInfo;

        public AddZOVMainOrdersForm(ref OrdersManager tOrdersManager, NewOrderInfo tNewOrderInfo,
            bool bEditMainOrder, ref Form tTopForm, ref OrdersCalculate tOrdersCalculate)
        {
            TopForm = tTopForm;

            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            OrdersManager = tOrdersManager;
            NewOrderInfo = tNewOrderInfo;
            EditMainOrder = bEditMainOrder;
            OrdersCalculate = tOrdersCalculate;

            while (!SplashForm.bCreated) ;
        }

        private void AddZOVMainOrdersForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewZOVOrderSelectMenu NewOrderSelectMenu = new NewZOVOrderSelectMenu(this, ref OrdersManager, ref OrdersCalculate, ref NewOrderInfo);

            TopForm = NewOrderSelectMenu;
            NewOrderSelectMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewOrderSelectMenu.Dispose();
            TopForm = null;

            if (!NewOrderInfo.bPressOK)
                return;

            Initialize();

            string DebtType = "Нет";
            string Prepare = "Предварительно";
            string PriceType = "Розница";

            if (!NewOrderInfo.IsPrepare)
                Prepare = "Отгрузка: " + Convert.ToDateTime(NewOrderInfo.DispatchDate).ToString("dd.MM.yyyy");

            if (NewOrderInfo.PriceType == 1)
                PriceType = "Опт";
            if (NewOrderInfo.PriceType == 2)
                PriceType = "Стенка";

            if (NewOrderInfo.DebtType == 1)
                DebtType = "Долг";
            if (NewOrderInfo.DebtType == 2)
                DebtType = "Брак";
            if (NewOrderInfo.DebtType == 3)
                DebtType = "Ошибка пр-ва";
            if (NewOrderInfo.DebtType == 4)
                DebtType = "Ошибка ЗОВа";

            PrepareLabel.Text = Prepare + "   ";
            ClientLabel.Text = "Клиент: " + OrdersManager.GetClientName(NewOrderInfo.ClientID) + "   ";
            DocNumberLabel.Text = "Кухня: " + NewOrderInfo.DocNumber + "   ";
            PriceTypeLabel.Text = PriceType + "   ";
            DebtLabel.Text = NewOrderInfo.DebtDocNumber + " " + DebtType + "   ";

            if (NewOrderInfo.IsSample)
                SampleLabel.Visible = true;
            else
                SampleLabel.Visible = false;

            if (NewOrderInfo.DoNotDispatch)
                DoNotDispatchLabel.Visible = true;
            else
                DoNotDispatchLabel.Visible = false;
            if (NewOrderInfo.TechDrilling)
                lblTechDrilling.Visible = true;
            else
                lblTechDrilling.Visible = false;
            if (NewOrderInfo.QuicklyOrder)
                lblQuicklyOrder.Visible = true;
            else
                lblQuicklyOrder.Visible = false;
            if (NewOrderInfo.ToAssembly)
                lbIsAssembly.Visible = true;
            else
                lbIsAssembly.Visible = false;
            if (NewOrderInfo.IsNotPaid)
            {
                lbIsNotPaid.Visible = true;
            }
            else
                lbIsNotPaid.Visible = false;

            if (NewOrderInfo.IsDebt)
                DebtLabel.Visible = true;
            else
                DebtLabel.Visible = false;

            if (NewOrderInfo.NeedCalculate)
                NeedCalcLabel.Visible = true;
            else
                NeedCalcLabel.Visible = false;

            if (!NewOrderInfo.IsNewOrder && !NewOrderInfo.IsDoubleOrder)//редактирование кухни
            {
                MainOrderNotes.Text = NewOrderInfo.Notes;
                //редактирование фасадов
                FrontsOrdersDataGrid.Visible = FrontsOrders.EditOrder(OrdersManager.CurrentMainOrderID);

                //редактирование декора
                if (DecorOrders.EditDecorOrder(OrdersManager.CurrentMainOrderID))
                    NoDecorLabel.Visible = false;
                else
                    NoDecorLabel.Visible = true;
            }
            if (NewOrderInfo.IsDoubleOrder)
            {
                if (NewOrderInfo.IsDoubleOrder)//двойное вбивание
                {
                    FCompareOrdersButton.BringToFront();
                    DCompareOrdersButton.BringToFront();
                }
                else//новая кухня
                {
                    FrontsOrders.NewOrder();
                    DecorOrders.NewOrder();
                }
            }
            FrontsOrders.GetTempFronts();
            DecorOrders.GetTempDecor();
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
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            FrontsCatalogOrder = new FrontsCatalogOrder(ref FrontsHeightComboBox, ref FrontsWidthComboBox);

            DecorCatalogOrder = new DecorCatalogOrder(ref DecorLengthComboBox, ref DecorHeightComboBox, ref DecorWidthComboBox);

            FrontsOrders = new FrontsOrders(ref FrontsOrdersDataGrid,
                ref FrontsCatalogOrder);
            FrontsOrders.HasClientExcluzive(NewOrderInfo.ClientID);
            FrontsOrders.Initialize();

            DecorOrders = new DecorOrders(ref MainOrdersDecorTabControl,
                ref DecorCatalogOrder, ref FrontsOrdersDataGrid);
            DecorOrders.HasClientExcluzive(NewOrderInfo.ClientID);
            DecorOrders.Initialize();

            FrontsCatalogOrderBinding();

            DecorCatalogOrderBinding();

            if (!FrontsOrders.HasExcluzive && !DecorOrders.HasExcluzive)
                cbOnlyExcluzive.Visible = false;
            FrontsDrillTypesComboBox.DataSource = OrdersManager.FrontsDrillTypesDataTable;
            FrontsDrillTypesComboBox.DisplayMember = "FrontDrillType";
            FrontsDrillTypesComboBox.ValueMember = "FrontDrillTypeID";

            FrontsOrders.GetTempFronts();
            DecorOrders.GetTempDecor();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
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

        //ORDER FRONTS CATALOG
        public void NGetFrameColors()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsComboBox.SelectedItem != null)
            {
                FrontsCatalogOrder.FilterCatalogFrameColors(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogFrameColors(string.Empty, bExcluzive);
            }

        }

        public void NGetTechnoProfiles()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.FrameColorsBindingSource.Current != null)
            {
                FrontsCatalogOrder.FilterCatalogTechnoProfiles(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogTechnoProfiles(string.Empty, -1, bExcluzive);
            }
        }

        public void NGetTechnoFrameColors()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.FrameColorsBindingSource.Current != null)
            {
                FrontsCatalogOrder.FilterCatalogTechnoFrameColors(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogTechnoFrameColors(string.Empty, -1, -1, bExcluzive);
            }
        }

        public void NGetPatina()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.TechnoFrameColorsBindingSource.Current != null)
            {
                FrontsCatalogOrder.FilterCatalogPatina(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogPatina(string.Empty, -1, -1, -1, bExcluzive);
            }
        }

        public void NGetInsetTypes()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.PatinaBindingSource.Current != null)
            {
                int PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);

                FrontsCatalogOrder.FilterCatalogInsetTypes(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                    PatinaID, bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogInsetTypes(string.Empty, -1, -1, -1, -1, bExcluzive);
            }
        }

        public void NGetInsetColors()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.InsetTypesBindingSource.Current != null)
            {
                int PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);

                FrontsCatalogOrder.FilterCatalogInsetColors(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                    PatinaID,
                    Convert.ToInt32(InsetTypesComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogInsetColors(string.Empty, -1, -1, -1, -1, -1, bExcluzive);
            }
        }

        public void NGetTechnoInsetTypes()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.InsetColorsBindingSource.Current != null)
            {
                int PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);

                FrontsCatalogOrder.FilterCatalogTechnoInsetTypes(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                    PatinaID,
                    Convert.ToInt32(InsetTypesComboBox.SelectedValue),
                    Convert.ToInt32(InsetColorComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogTechnoInsetTypes(string.Empty, -1, -1, -1, -1, -1, -1, bExcluzive);
            }
        }

        public void NGetTechnoInsetColors()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.TechnoInsetTypesBindingSource.Current != null)
            {
                int PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);

                FrontsCatalogOrder.FilterCatalogTechnoInsetColors(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                                                       Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue),
                                                       Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                                                       PatinaID,
                                                       Convert.ToInt32(InsetTypesComboBox.SelectedValue),
                                                       Convert.ToInt32(InsetColorComboBox.SelectedValue),
                                                       Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogTechnoInsetColors(string.Empty, -1, -1, -1, -1, -1, -1, -1, bExcluzive);
            }
        }

        public void NGetHeight()
        {
            if (FrontsComboBox.SelectedItem == null)
                return;
            int PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);

            FrontsCatalogOrder.FilterCatalogHeight(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                                                            Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                                                            PatinaID,
                                                            Convert.ToInt32(InsetTypesComboBox.SelectedValue),
                                                            Convert.ToInt32(InsetColorComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoInsetColorsComboBox.SelectedValue));
        }

        public void NGetWidth()
        {
            if (FrontsComboBox.SelectedItem == null)
                return;
            int Height = 0;

            if (FrontsHeightComboBox.Text != "")
                Height = Convert.ToInt32(FrontsHeightComboBox.Text);
            int PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);

            FrontsCatalogOrder.FilterCatalogWidth(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                                                            Convert.ToInt32(FrameColorComboBox.SelectedValue), Convert.ToInt32(TechnoProfilesComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                                                            PatinaID,
                                                            Convert.ToInt32(InsetTypesComboBox.SelectedValue),
                                                            Convert.ToInt32(InsetColorComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoInsetColorsComboBox.SelectedValue),
                                                            Height);
        }

        public void FrontsCatalogOrderBinding()
        {
            DataGridViewTextBoxColumn IndexNumber = new DataGridViewTextBoxColumn()
            {
                HeaderText = "",
                Name = "IndexNumber",
                ReadOnly = true
            };
            FrontsOrdersDataGrid.Columns.Add(IndexNumber);

            FrontsOrdersDataGrid.Columns["IndexNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IndexNumber"].Width = 40;
            FrontsOrdersDataGrid.Columns["IndexNumber"].Visible = true;
            FrontsOrdersDataGrid.Columns["IndexNumber"].DisplayIndex = 0;

            FrontsComboBox.DataSource = FrontsCatalogOrder.FrontsBindingSource;
            FrontsComboBox.DisplayMember = FrontsCatalogOrder.FrontsBindingSourceDisplayMember;
            FrontsComboBox.ValueMember = FrontsCatalogOrder.FrontsBindingSourceValueMember;

            TechnoProfilesComboBox.DataSource = FrontsCatalogOrder.TechnoProfilesBindingSource;
            TechnoProfilesComboBox.DisplayMember = FrontsCatalogOrder.TechnoProfilesBindingSourceDisplayMember;
            TechnoProfilesComboBox.ValueMember = FrontsCatalogOrder.TechnoProfilesBindingSourceValueMember;

            FrameColorComboBox.DataSource = FrontsCatalogOrder.FrameColorsBindingSource;
            FrameColorComboBox.DisplayMember = FrontsCatalogOrder.FrameColorsBindingSourceDisplayMember;
            FrameColorComboBox.ValueMember = FrontsCatalogOrder.FrameColorsBindingSourceValueMember;

            PatinaComboBox.DataSource = FrontsCatalogOrder.PatinaBindingSource;
            PatinaComboBox.DisplayMember = FrontsCatalogOrder.PatinaBindingSourceDisplayMember;
            PatinaComboBox.ValueMember = FrontsCatalogOrder.PatinaBindingSourceValueMember;

            InsetTypesComboBox.DataSource = FrontsCatalogOrder.InsetTypesBindingSource;
            InsetTypesComboBox.DisplayMember = FrontsCatalogOrder.InsetTypesBindingSourceDisplayMember;
            InsetTypesComboBox.ValueMember = FrontsCatalogOrder.InsetTypesBindingSourceValueMember;

            InsetColorComboBox.DataSource = FrontsCatalogOrder.InsetColorsBindingSource;
            InsetColorComboBox.DisplayMember = FrontsCatalogOrder.InsetColorsBindingSourceDisplayMember;
            InsetColorComboBox.ValueMember = FrontsCatalogOrder.InsetColorsBindingSourceValueMember;

            TechnoFrameColorComboBox.DataSource = FrontsCatalogOrder.TechnoFrameColorsBindingSource;
            TechnoFrameColorComboBox.DisplayMember = FrontsCatalogOrder.FrameColorsBindingSourceDisplayMember;
            TechnoFrameColorComboBox.ValueMember = FrontsCatalogOrder.FrameColorsBindingSourceValueMember;

            TechnoInsetTypesComboBox.DataSource = FrontsCatalogOrder.TechnoInsetTypesBindingSource;
            TechnoInsetTypesComboBox.DisplayMember = FrontsCatalogOrder.InsetTypesBindingSourceDisplayMember;
            TechnoInsetTypesComboBox.ValueMember = FrontsCatalogOrder.InsetTypesBindingSourceValueMember;

            TechnoInsetColorsComboBox.DataSource = FrontsCatalogOrder.TechnoInsetColorsBindingSource;
            TechnoInsetColorsComboBox.DisplayMember = FrontsCatalogOrder.InsetColorsBindingSourceDisplayMember;
            TechnoInsetColorsComboBox.ValueMember = FrontsCatalogOrder.InsetColorsBindingSourceValueMember;

            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            FrontsCatalogOrder.FilterFronts(bExcluzive);
            NGetFrameColors();
            NGetTechnoProfiles();
            NGetTechnoFrameColors();
            NGetPatina();
            NGetInsetTypes();
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void FrontsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetFrameColors();
            NGetTechnoProfiles();
            NGetTechnoFrameColors();
            NGetPatina();
            NGetInsetTypes();
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }
        private void FrameColorComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetTechnoProfiles();
            NGetTechnoFrameColors();
            NGetPatina();
            NGetInsetTypes();
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void TechnoProfilesComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetTechnoFrameColors();
            NGetPatina();
            NGetInsetTypes();
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void TechnoFrameColorComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetPatina();
            NGetInsetTypes();
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void PatinaComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetInsetTypes();
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void InsetTypesComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void InsetColorComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void TechnoInsetTypesComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
        }

        private void TechnoInsetColorsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetHeight();
            NGetWidth();
        }
        private void FrontsHeightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NGetWidth();
        }



        //ORDER DECOR CATALOG
        public void NGetDecorItems()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!DecorOrders.HasExcluzive)
                bExcluzive = false;
            DecorCatalogOrder.FilterItems(Convert.ToInt32(DecorProductsComboBox.SelectedValue), bExcluzive);
        }

        public void NGetDecorColors()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!DecorOrders.HasExcluzive)
                bExcluzive = false;
            if (DecorCatalogOrder.FilterColors(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(), bExcluzive) == true)
            {
                flowLayoutPanel3.Visible = true;
            }
            else
            {
                flowLayoutPanel3.Visible = false;
            }
        }

        public void NGetDecorPatina()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!DecorOrders.HasExcluzive)
                bExcluzive = false;
            if (DecorCatalogOrder.FilterPatina(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
                Convert.ToInt32(DecorColorsComboBox.SelectedValue), bExcluzive) == true)
            {
                panel29.Visible = true;
            }
            else
            {
                panel29.Visible = false;
            }
        }

        public void NGetDecorInsetTypes()
        {
            if (DecorItemsComboBox.SelectedItem == null)
                return;
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!DecorOrders.HasExcluzive)
                bExcluzive = false;
            DecorCatalogOrder.FilterInsetType(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
                Convert.ToInt32(DecorColorsComboBox.SelectedValue),
                Convert.ToInt32(DecorPatinaComboBox.SelectedValue), bExcluzive);
        }

        public void NGetDecorInsetColors()
        {
            if (DecorItemsComboBox.SelectedItem == null)
                return;
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!DecorOrders.HasExcluzive)
                bExcluzive = false;
            DecorCatalogOrder.FilterInsetColor(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
                Convert.ToInt32(DecorColorsComboBox.SelectedValue),
                Convert.ToInt32(DecorPatinaComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue), bExcluzive);
        }

        public void NGetDecorLength()
        {
            if (DecorItemsComboBox.SelectedItem == null)
                return;
            int PatinaID = Convert.ToInt32(DecorPatinaComboBox.SelectedValue);

            int L = DecorCatalogOrder.FilterLength(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
                Convert.ToInt32(DecorColorsComboBox.SelectedValue),
                Convert.ToInt32(DecorPatinaComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetColorsComboBox.SelectedValue));

            if (L == -1)
            {
                panel4.Visible = false;
            }
            else if (L == 0)
            {
                panel4.Visible = true;
            }
            else if (L > 0)
            {
                panel4.Visible = true;
            }
        }

        public void NGetDecorHeight()
        {
            if (DecorItemsComboBox.SelectedItem == null)
                return;
            int Length = -1;
            if (DecorCatalogOrder.ItemLengthBindingSource.Count > 0)
                Length = Convert.ToInt32(((DataRowView)DecorCatalogOrder.ItemLengthBindingSource.Current).Row["Length"]);

            int PatinaID = Convert.ToInt32(DecorPatinaComboBox.SelectedValue);

            int H = DecorCatalogOrder.FilterHeight(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
                Convert.ToInt32(DecorColorsComboBox.SelectedValue),
                Convert.ToInt32(DecorPatinaComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetColorsComboBox.SelectedValue), Length);

            if (H == -1)
            {
                panel1.Visible = false;
            }
            else if (H == 0)
            {
                panel1.Visible = true;
            }
            else if (H > 0)
            {
                panel1.Visible = true;
            }
        }

        public void NGetDecorWidth()
        {
            if (DecorItemsComboBox.SelectedItem == null)
                return;
            int Length = -1;
            int Height = -1;
            int PatinaID = Convert.ToInt32(DecorPatinaComboBox.SelectedValue);

            if (DecorCatalogOrder.ItemLengthBindingSource.Count > 0)
                Length = Convert.ToInt32(((DataRowView)DecorCatalogOrder.ItemLengthBindingSource.Current).Row["Length"]);
            if (DecorCatalogOrder.ItemHeightBindingSource.Count > 0)
                Height = Convert.ToInt32(((DataRowView)DecorCatalogOrder.ItemHeightBindingSource.Current).Row["Height"]);
            int W = DecorCatalogOrder.FilterWidth(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
                Convert.ToInt32(DecorColorsComboBox.SelectedValue),
                Convert.ToInt32(DecorPatinaComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetColorsComboBox.SelectedValue), Length, Height);
            if (W == -1)
            {
                panel2.Visible = false;
            }
            else if (W == 0)
            {
                panel2.Visible = true;
            }
            else if (W > 0)
            {
                panel2.Visible = true;
            }
        }

        public void DecorCatalogOrderBinding()
        {
            DecorProductsComboBox.DataSource = DecorCatalogOrder.DecorProductsBindingSource;
            DecorProductsComboBox.DisplayMember = DecorCatalogOrder.DecorProductsBindingSourceDisplayMember;
            DecorProductsComboBox.ValueMember = DecorCatalogOrder.DecorProductsBindingSourceValueMember;

            DecorItemsComboBox.DataSource = DecorCatalogOrder.ItemsBindingSource;
            DecorItemsComboBox.DisplayMember = DecorCatalogOrder.ItemsBindingSourceDisplayMember;
            DecorItemsComboBox.ValueMember = DecorCatalogOrder.ItemsBindingSourceValueMember;

            DecorColorsComboBox.DataSource = DecorCatalogOrder.ItemColorsBindingSource;
            DecorColorsComboBox.DisplayMember = DecorCatalogOrder.ItemColorsBindingSourceDisplayMember;
            DecorColorsComboBox.ValueMember = DecorCatalogOrder.ItemColorsBindingSourceValueMember;

            DecorPatinaComboBox.DataSource = DecorCatalogOrder.ItemPatinaBindingSource;
            DecorPatinaComboBox.DisplayMember = DecorCatalogOrder.ItemPatinaBindingSourceDisplayMember;
            DecorPatinaComboBox.ValueMember = DecorCatalogOrder.ItemPatinaBindingSourceValueMember;

            DecorInsetTypesComboBox.DataSource = DecorCatalogOrder.ItemInsetTypesBindingSource;
            DecorInsetTypesComboBox.DisplayMember = "InsetTypeName";
            DecorInsetTypesComboBox.ValueMember = "InsetTypeID";

            DecorInsetColorsComboBox.DataSource = DecorCatalogOrder.ItemInsetColorsBindingSource;
            DecorInsetColorsComboBox.DisplayMember = "InsetColorName";
            DecorInsetColorsComboBox.ValueMember = "InsetColorID";

            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!DecorOrders.HasExcluzive)
                bExcluzive = false;
            DecorCatalogOrder.FilterProducts(bExcluzive);
            NGetDecorItems();
            NGetDecorColors();
            NGetDecorPatina();
            NGetDecorInsetTypes();
            NGetDecorInsetColors();
            NGetDecorLength();
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void DecorProductsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetDecorItems();
            NGetDecorColors();
            NGetDecorPatina();
            NGetDecorInsetTypes();
            NGetDecorInsetColors();
            NGetDecorLength();
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void DecorItemsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetDecorColors();
            NGetDecorPatina();
            NGetDecorInsetTypes();
            NGetDecorInsetColors();
            NGetDecorLength();
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void DecorColorsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetDecorPatina();
            NGetDecorInsetTypes();
            NGetDecorInsetColors();
            NGetDecorLength();
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void DecorHeightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NGetDecorWidth();
        }

        private void DecorProductsComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecorProductsComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(DecorProductsComboBox, true);
            }
        }

        private void DecorItemsComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecorItemsComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(DecorItemsComboBox, true);
            }
        }

        private void DecorColorsComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecorColorsComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(DecorColorsComboBox, true);
            }
        }

        private void FrontsComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FrontsComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(FrontsComboBox, true);
            }
        }

        private void FrameColorComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FrameColorComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(FrameColorComboBox, true);
            }
        }

        private void PatinaComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PatinaComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(PatinaComboBox, true);
            }
        }

        private void InsetColorComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InsetColorComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(InsetColorComboBox, true);
            }
        }

        private void InsetTypesComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InsetTypesComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(InsetTypesComboBox, true);
            }
        }

        private void TechnoInsetTypesComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TechnoInsetTypesComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(TechnoInsetTypesComboBox, true);
            }
        }

        private void TechnoInsetColorsComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TechnoInsetColorsComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(TechnoInsetColorsComboBox, true);
            }
        }

        private void SetAutoComplete(ComponentFactory.Krypton.Toolkit.KryptonComboBox ComboBox, bool Autocomplete)
        {
            if (Autocomplete)
            {
                bool focused = ComboBox.Focused;
                if (ComboBox.Focused)
                {
                    ComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    ComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                }
            }
            else
            {
                ComboBox.AutoCompleteMode = AutoCompleteMode.None;
                ComboBox.AutoCompleteSource = AutoCompleteSource.None;
            }
        }

        private void FrontAddButton_Click(object sender, EventArgs e)
        {
            int MainOrderID = OrdersManager.CurrentMainOrderID;
            int FrontID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int ColorID = -1;
            int InsetColorID = -1;
            int TechnoProfileID = -1;
            int TechnoColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;
            int FrontDrillTypeID = -1;
            decimal HandsSize = 0;
            int Height = -1;
            int Width = -1;
            int Count = -1;
            int ImpostMargin = 0;

            //фасад
            if (FrontsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран фасад или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //FrontID = Convert.ToInt32(FrontsComboBox.SelectedValue);

            //цвет профиля
            if (FrameColorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран цвет профиля или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //тип патины
            if (PatinaComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран тип патины или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);


            //тип наполнителя
            if (InsetTypesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран тип наполнителя или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InsetTypeID = Convert.ToInt32(InsetTypesComboBox.SelectedValue);


            ColorID = Convert.ToInt32(FrameColorComboBox.SelectedValue);


            //цвет наполнителя
            if (InsetColorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран цвет наполнителя или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InsetColorID = Convert.ToInt32(InsetColorComboBox.SelectedValue);

            //тип профиля-2
            if (TechnoProfilesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Тип профиля-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoProfileID = Convert.ToInt32(TechnoProfilesComboBox.SelectedValue);

            //Цвет профиля-2
            if (TechnoFrameColorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Цвет профиля-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoColorID = Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue);

            //Тип наполнителя-2
            if (TechnoInsetTypesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Тип наполнителя-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue);

            //цвет наполнителя техно
            if (TechnoInsetColorsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран цвет наполнителя техно или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsComboBox.SelectedValue);


            //высота
            if (FrontsHeightComboBox.Text.Length < 1)
            {
                MessageBox.Show(this, "Не указана высота фасада", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Height = Convert.ToInt32(FrontsHeightComboBox.Text);


            //ширина
            if (FrontsWidthComboBox.Text.Length < 1)
            {
                MessageBox.Show(this, "Не указана ширина фасада", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Width = Convert.ToInt32(FrontsWidthComboBox.Text);


            //количество
            if (string.IsNullOrWhiteSpace(FrontsCountComboBox.Text) || Convert.ToInt32(FrontsCountComboBox.Text) < 1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Неверное количество фасадов", "Ошибка добавления фасада");

                return;
            }
            if (!string.IsNullOrWhiteSpace(cbImpostMargin.Text) && Convert.ToInt32(cbImpostMargin.Text) > 0)
            {
                ImpostMargin = Convert.ToInt32(cbImpostMargin.Text);
            }

            FrontDrillTypeID = Convert.ToInt32(FrontsDrillTypesComboBox.SelectedValue);
            if (FrontHandsSizeTextBox.Text.Length > 0)
                try
                {
                    HandsSize = Convert.ToDecimal(FrontHandsSizeTextBox.Text);
                }
                catch
                {
                    try
                    {
                        HandsSize = Convert.ToDecimal(FrontHandsSizeTextBox.Text.Replace(".", ","));
                    }
                    catch
                    {
                        return;
                    }
                }

            Count = Convert.ToInt32(FrontsCountComboBox.Text);

            int FactoryID = 0;
            int AreaID = 0;
            FrontsOrders.FrontsCatalogOrder.GetFrontConfigID(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(), ColorID, PatinaID, InsetTypeID,
                   InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref FrontID, ref FactoryID, ref AreaID);
            if (FrontID == -1)
                return;
            FrontsOrders.AddFrontsOrder(MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID,
                InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, HandsSize, FrontDrillTypeID, FrontsOrdersNotesTextBox.Text, NewOrderInfo.IsSample, ImpostMargin);
            cbImpostMargin.ResetText();
        }

        private void FrontRemoveButton_Click(object sender, EventArgs e)
        {
            FrontsOrders.RemoveOrder();
        }

        private void Save()
        {
            //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение заказа.\r\nПодождите..."); });
            //T.Start();
            //DataTable DT = OrdersCalculate.FFFF();
            //for (int i = 0; i < DT.Rows.Count; i++)
            //{
            //    int MegaOrderID = Convert.ToInt32(DT.Rows[i]["MegaOrderID"]);

            //    //FrontsOrders.SaveFrontsOrder(MainOrderID, ref FactoryIDF);
            //    //DecorOrders.SaveDecorOrder(ref FactoryIDD);

            //    //OrdersCalculate.CalculateOrder(MainOrderID, PriceTypeID, true);

            //    OrdersManager.SummaryCalcMegaOrder(MegaOrderID);

            //    OrdersManager.TotalCalcMegaOrder(MegaOrderID);


            //}
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;

            //InfiniumTips.ShowTip(this, 50, 85, "Заказ сохранён", 1700);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            //Save();
            //return;

            if (!NewOrderInfo.bPressOK)
            {
                return;
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            if (OrdersManager.InEditing == false && OrdersManager.InNew == false)
                return;

            int MainOrderID = OrdersManager.CurrentMainOrderID;

            if (MainOrderID == -1)
                return;

            if (FrontsOrders.HasRows() == false && DecorOrders.HasRows() == false)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "В заказе нет позиций. Удалить заказ?", "Сохранение заказа");
                if (!OKCancel)
                    return;

                OrdersManager.RemoveCurrentMainOrder(MainOrderID);

                if (OrdersManager.InEditing)
                {
                    OrdersManager.InEditing = false;
                }

                if (OrdersManager.InNew)
                {
                    OrdersManager.InNew = false;
                }

                return;
            }

            int FactoryIDF = -1;
            int FactoryIDD = -1;

            int F = 0;

            //если была добавлена новая позиция, то упаковки очищать не нужно
            //когда Коля зайдет в распределение, он увидит статус заказа Частично распределен
            //все упаковки будут на месте, а для новой позиции будет просто пустое поле
            //запакует, нажмет сохранить - все упаковки очистятся, но сохранятся в той последовательности, в которой были раньше
            //плюс новая упаковка для новой вбитой позиции в заказе

            ArrayList FrontsOrdersEdit = FrontsOrders.AreFrontsEdit(MainOrderID);
            ArrayList FrontsOrdersDelete = FrontsOrders.AreFrontsDelete(MainOrderID);

            ArrayList DecorOrdersEdit = DecorOrders.AreDecorEdit(MainOrderID);
            ArrayList DecorOrdersDelete = DecorOrders.AreDecorDelete(MainOrderID);

            if (FrontsOrders.SaveFrontsOrder(MainOrderID, ref FactoryIDF) == false)
                return;

            if (DecorOrders.SaveDecorOrder(ref FactoryIDD) == false)
                return;

            if (NewOrderInfo.IsDebt)
                OrdersManager.SetReorderDocNumber(NewOrderInfo.DocNumber, NewOrderInfo.DebtDocNumber);

            if (FactoryIDF == 1)
                if (FactoryIDD == 1)
                    F = 1;
                else if (FactoryIDD == -1)
                    F = 1;
                else
                    F = 0;

            if (FactoryIDF == 2)
                if (FactoryIDD == 2)
                    F = 2;
                else if (FactoryIDD == -1)
                    F = 2;
                else
                    F = 0;

            if (FactoryIDF == -1)
                F = FactoryIDD;

            int DebtTypeID = 0;
            int PriceTypeID = NewOrderInfo.PriceType;

            if (NewOrderInfo.IsDebt)
                DebtTypeID = NewOrderInfo.DebtType;

            bool NeedCalculate = NewOrderInfo.NeedCalculate;
            bool DoNotDispatch = NewOrderInfo.DoNotDispatch;
            bool TechDrilling = NewOrderInfo.TechDrilling;
            bool QuicklyOrder = NewOrderInfo.QuicklyOrder;

            if (!NewOrderInfo.IsDebt)
                NeedCalculate = true;

            bool IsOrderPackAlloc = OrdersManager.IsPackagesAlreadyPacked(MainOrderID);
            bool StartIsOrderPrepared = OrdersManager.IsOrderPrepared(MainOrderID);
            //bool ChangeClient = false;

            int ClientID = OrdersManager.GetCurrentClientID(MainOrderID);
            //if (ClientID != NewOrderInfo.ClientID)
            //ChangeClient = true;


            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение заказа.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (StartIsOrderPrepared)
            {
                OrdersManager.CreateNewDispatch(NewOrderInfo.DispatchDate);

                if (NewOrderInfo.DateEnabled)
                {
                    OrdersManager.MovePreparedMainOrder(NewOrderInfo.DocNumber, Convert.ToDateTime(NewOrderInfo.DispatchDate));

                    OrdersManager.MoveToMegaOrder(NewOrderInfo.DispatchDate);

                    OrdersManager.MoveToDocNumber(MainOrderID);
                }
            }
            //else
            //{
            //    if (NewOrderInfo.IsNewOrder)
            //        OrdersManager.SetDoubleOrder(MainOrderID, true);
            //}

            NewOrderInfo.Notes = MainOrderNotes.Text;
            OrdersManager.SaveOrder(NewOrderInfo.DispatchDate, MainOrderID, NewOrderInfo.ClientID, NewOrderInfo.DocNumber, NewOrderInfo.DebtDocNumber,
                DebtTypeID, NewOrderInfo.IsSample, NewOrderInfo.IsPrepare,
                PriceTypeID, NewOrderInfo.Notes, F, NeedCalculate, DoNotDispatch, TechDrilling, QuicklyOrder, NewOrderInfo.ToAssembly, NewOrderInfo.IsNotPaid);

            OrdersCalculate.CalculateOrder(MainOrderID, PriceTypeID, NewOrderInfo.IsSample, NewOrderInfo.TechDrilling);

            OrdersManager.SummaryCalcCurrentMegaOrder();

            string DD = OrdersManager.GetCurrentMegaOrderDispatchDate(OrdersManager.CurrentMegaOrderID);

            if (DD.Length > 0)
            {
                if (NewOrderInfo.DispatchDate != null && Convert.ToDateTime(DD) != Convert.ToDateTime(NewOrderInfo.DispatchDate))
                    OrdersManager.TotalCalcMegaOrder(Convert.ToDateTime(NewOrderInfo.DispatchDate));
            }
            else
                if (NewOrderInfo.DispatchDate != null)
                OrdersManager.TotalCalcMegaOrder(Convert.ToDateTime(NewOrderInfo.DispatchDate));
            if (OrdersManager != null)
                if (OrdersManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        OrdersManager.Filter(MainOrderID);
                    }
                }

            FrontsOrders.ExitEdit();
            DecorOrders.ExitEdit();

            bool EndIsOrderPrepared = OrdersManager.IsOrderPrepared(MainOrderID);

            if (IsOrderPackAlloc)
            {
                //if (ChangeClient)
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false,
                //        "Был изменен клиент. Упаковки будут очищены.",
                //        "Предупреждение");

                //    OrdersManager.ClearPackages(MainOrderID);
                //}
                //ПЕРЕД ОЧИЩЕНИЕМ УПАКОВОК НУЖНО СДЕЛАТЬ ИХ КОПИЮ В ТАБЛИЦУ PackagesEvents
                //if (FrontsOrdersEdit.Count > 0)
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false,
                //        "Были внесены изменения в фасады. Упаковки будут очищены.",
                //        "Предупреждение");
                //    OrdersManager.FixOrderEvent(MainOrderID, "Были изменены размеры фасадов");
                //    OrdersManager.ClearPackages(MainOrderID);
                //    //FrontsOrders.ClearPackages(MainOrderID, FrontsOrdersEdit);
                //}
                //if (FrontsOrdersDelete.Count > 0)
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false,
                //        "Были внесены изменения в фасады. Упаковки будут очищены.",
                //        "Предупреждение");
                //    OrdersManager.FixOrderEvent(MainOrderID, "Были удалены фасады");
                //    OrdersManager.ClearPackages(MainOrderID);
                //    //FrontsOrders.ClearPackages(MainOrderID, FrontsOrdersDelete);
                //}
                //if (DecorOrdersEdit.Count > 0)
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false,
                //        "Были внесены изменения в декор. Упаковки будут очищены.",
                //        "Предупреждение");
                //    OrdersManager.FixOrderEvent(MainOrderID, "Были изменены размеры декора. " + "Упаковки очищены, UserID=" + Security.CurrentUserID);
                //    OrdersManager.ClearPackages(MainOrderID);
                //    //DecorOrders.ClearPackages(MainOrderID, DecorOrdersEdit);
                //}
                //if (DecorOrdersDelete.Count > 0)
                //{
                //    Infinium.LightMessageBox.Show(ref TopForm, false,
                //        "Были внесены изменения в декор. Упаковки будут очищены.",
                //        "Предупреждение");
                //    OrdersManager.FixOrderEvent(MainOrderID, "Были удалены декоры " + "Упаковки очищены, UserID=" + Security.CurrentUserID);
                //    OrdersManager.ClearPackages(MainOrderID);
                //    //DecorOrders.ClearPackages(MainOrderID, DecorOrdersDelete);
                //}
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            InfiniumTips.ShowTip(this, 50, 85, "Заказ сохранён", 1700);

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;

            PrepareLabel.Visible = false;
            ClientLabel.Visible = false;
            DocNumberLabel.Visible = false;
            PriceTypeLabel.Visible = false;
            DebtLabel.Visible = false;
            SampleLabel.Visible = false;
            DoNotDispatchLabel.Visible = false;
            DebtLabel.Visible = false;
            NeedCalcLabel.Visible = false;
        }

        public void AddFrontsFromSizeTable(ref PercentageDataGrid DataGrid)
        {
            int MainOrderID = OrdersManager.CurrentMainOrderID;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int ColorID = -1;
            int InsetColorID = -1;
            int TechnoProfileID = -1;
            int TechnoColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;

            ////фасад
            //if (FrontsComboBox.SelectedIndex == -1)
            //{
            //    MessageBox.Show(this, "Не выбран фасад или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //FrontID = Convert.ToInt32(FrontsComboBox.SelectedValue);

            //цвет профиля
            if (FrameColorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран цвет профиля или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ColorID = Convert.ToInt32(FrameColorComboBox.SelectedValue);


            //тип патины
            if (PatinaComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран тип патины или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);


            //тип наполнителя
            if (InsetTypesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран тип наполнителя или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InsetTypeID = Convert.ToInt32(InsetTypesComboBox.SelectedValue);


            //цвет наполнителя
            if (InsetColorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран цвет наполнителя или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InsetColorID = Convert.ToInt32(InsetColorComboBox.SelectedValue);

            //тип профиля-2
            if (TechnoProfilesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Тип профиля-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoProfileID = Convert.ToInt32(TechnoProfilesComboBox.SelectedValue);

            //Цвет профиля-2
            if (TechnoFrameColorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Цвет профиля-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoColorID = Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue);

            //Тип наполнителя-2
            if (TechnoInsetTypesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Тип наполнителя-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue);

            //цвет наполнителя техно
            if (TechnoInsetColorsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран цвет наполнителя техно или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsComboBox.SelectedValue);

            FrontsOrders.AddFrontsOrderFromSizeTable(ref DataGrid, MainOrderID, ((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, NewOrderInfo.IsSample);

        }

        public void AddFrontsCupboards(ref ComponentFactory.Krypton.Toolkit.KryptonListBox CupboardsExportListBox)
        {
            //NewOrderClientAreaPanel.BringToFront();
            //ExcelOrderSelector.AccessibleName = "Order";

            String DocNumber = null;
            int MainOrderID = OrdersManager.CurrentMainOrderID;
            int FrontID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int ColorID = -1;
            int InsetColorID = -1;
            int TechnoProfileID = -1;
            int TechnoColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;
            int Height = 0;
            int Width = 0;
            int Count = 0;

            DocNumber = NewOrderInfo.DocNumber;


            //фасад
            if (FrontsComboBox.SelectedIndex == -1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Не выбран фасад или нет в базе", "Ошибка добавления фасада");

                return;
            }

            //FrontID = Convert.ToInt32(FrontsComboBox.SelectedValue);


            //цвет профиля
            if (FrameColorComboBox.SelectedIndex == -1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Не выбран цвет профиля или нет в базе", "Ошибка добавления фасада");

                return;
            }

            ColorID = Convert.ToInt32(FrameColorComboBox.SelectedValue);

            //тип патины
            if (PatinaComboBox.SelectedIndex == -1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Не выбран тип фасада или нет в базе", "Ошибка добавления фасада");

                return;
            }

            PatinaID = Convert.ToInt32(PatinaComboBox.SelectedValue);


            //тип наполнителя
            if (InsetTypesComboBox.SelectedIndex == -1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Не выбран тип наполнителя или нет в базе", "Ошибка добавления фасада");

                return;
            }

            InsetTypeID = Convert.ToInt32(InsetTypesComboBox.SelectedValue);


            //цвет наполнителя
            if (InsetColorComboBox.SelectedIndex == -1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Не выбран цвет наполнителя или нет в базе", "Ошибка добавления фасада");

                return;
            }

            InsetColorID = Convert.ToInt32(InsetColorComboBox.SelectedValue);

            //тип профиля-2
            if (TechnoProfilesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Тип профиля-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoProfileID = Convert.ToInt32(TechnoProfilesComboBox.SelectedValue);

            //Цвет профиля-2
            if (TechnoFrameColorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Цвет профиля-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoColorID = Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue);

            //Тип наполнителя-2
            if (TechnoInsetTypesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Тип наполнителя-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoInsetTypeID = Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue);

            //цвет наполнителя техно
            if (TechnoInsetColorsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран цвет наполнителя техно или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsComboBox.SelectedValue);

            Count = Convert.ToInt32(FrontsCountComboBox.Text);

            int FactoryID = 0;
            int AreaID = 0;
            FrontsOrders.FrontsCatalogOrder.GetFrontConfigID(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(), ColorID, PatinaID, InsetTypeID,
                   InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref FrontID, ref FactoryID, ref AreaID);
            if (FrontID == -1)
                return;
            FrontsOrders.AddFrontsOrderCupboards(ref CupboardsExportListBox, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID,
                InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, NewOrderInfo.IsSample);
        }

        private void DecorAddButton_Click(object sender, EventArgs e)
        {
            int MainOrderID = OrdersManager.CurrentMainOrderID;
            int ProductID = Convert.ToInt32(DecorProductsComboBox.SelectedValue);
            int ItemID = -1;
            int ColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int Height = -1;
            int Length = -1;
            int Width = -1;
            int Count = 0;

            if (flowLayoutPanel3.Visible)
                if (DecorColorsComboBox.Text != "")
                    ColorID = Convert.ToInt32(DecorColorsComboBox.SelectedValue);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - цвет");
                    return;
                }
            if (panel29.Visible)
                if (DecorPatinaComboBox.Text != "")
                    PatinaID = Convert.ToInt32(DecorPatinaComboBox.SelectedValue);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - патина");
                    return;
                }
            if (panel6.Visible)
                if (DecorInsetTypesComboBox.Text != "")
                    InsetTypeID = Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - тип наполнителя");
                    return;
                }
            if (panel7.Visible)
                if (DecorInsetColorsComboBox.Text != "")
                    InsetColorID = Convert.ToInt32(DecorInsetColorsComboBox.SelectedValue);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - цвет наполнителя");
                    return;
                }

            if (panel4.Visible)
                if (DecorLengthComboBox.Text != "")
                    Length = Convert.ToInt32(DecorLengthComboBox.Text);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - длина");
                    return;
                }

            if (panel1.Visible)
                if (DecorHeightComboBox.Text != "")
                    Height = Convert.ToInt32(DecorHeightComboBox.Text);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - высота");
                    return;
                }

            if (panel2.Visible)
                if (DecorWidthComboBox.Text != "")
                    Width = Convert.ToInt32(DecorWidthComboBox.Text);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - ширина");
                    return;
                }


            //количество
            if (DecorCountNumUpDown.Text.Length < 1 || Convert.ToInt32(DecorCountNumUpDown.Text) < 1)
            {
                MessageBox.Show(this, "Неверное количество", "Ошибка добавления декора", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Count = Convert.ToInt32(DecorCountNumUpDown.Text);

            int FactoryID = 0;
            int AreaID = 0;
            DecorOrders.DecorCatalogOrder.GetDecorConfigID(ProductID,
                ((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(), ColorID, PatinaID, InsetTypeID, InsetColorID,
                Length, Height, Width, ref ItemID, ref FactoryID, ref AreaID);
            if (ItemID == -1)
                return;
            DecorOrders.AddDecorOrder(MainOrderID, ProductID, ItemID, ColorID, PatinaID, InsetTypeID, InsetColorID, Length, Height, Width, Count, DecorOrdersNotesTextBox.Text, NewOrderInfo.IsSample);
            NoDecorLabel.Visible = false;
        }

        private void DecorRemoveButton_Click(object sender, EventArgs e)
        {
            DecorOrders.DeleteCurrentDecorItem(MainOrdersDecorTabControl.SelectedTabPageIndex);

            if (MainOrdersDecorTabControl.SelectedTabPage == null)
                NoDecorLabel.Visible = true;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            DecorOrders.DecorItemOrdersDataGrids[0].Size.Width.ToString();
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (FrontsCheckButton.Checked)
            {
                FrontsPanel.BringToFront();
                MainOrdersTabControl.SelectedTabPageIndex = 0;
                FrontsComboBox.SelectionLength = 0;
                FrameColorComboBox.SelectionLength = 0;
                TechnoProfilesComboBox.SelectionLength = 0;
                TechnoFrameColorComboBox.SelectionLength = 0;
                PatinaComboBox.SelectionLength = 0;
                InsetTypesComboBox.SelectionLength = 0;
                InsetColorComboBox.SelectionLength = 0;
                TechnoInsetTypesComboBox.SelectionLength = 0;
                TechnoInsetColorsComboBox.SelectionLength = 0;
                FrontsComboBox.Focus();
            }
            else
            {
                DecorPanel.BringToFront();
                MainOrdersTabControl.SelectedTabPageIndex = 1;
                DecorProductsComboBox.SelectionLength = 0;
                DecorItemsComboBox.SelectionLength = 0;
                DecorColorsComboBox.SelectionLength = 0;
                DecorPatinaComboBox.SelectionLength = 0;
                DecorInsetTypesComboBox.SelectionLength = 0;
                DecorInsetColorsComboBox.SelectionLength = 0;
                DecorProductsComboBox.Focus();
            }
        }

        private void MainOrdersFrontsOrdersDataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (FrontsOrdersDataGrid.RowCount > 0)
                FrontsOrdersDataGrid.Visible = true;
        }

        private void FrontsOrdersDataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (FrontsOrders.FrontsOrdersBindingSource.Count < 1)
                FrontsOrdersDataGrid.Visible = false;
        }

        private void FrontsHeightComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                InsetColorComboBox.Focus();

                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                FrontsWidthComboBox.Focus();
            }
        }

        private void FrontsWidthComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                FrontsHeightComboBox.Focus();
            }

            if (e.KeyCode == Keys.Right)
            {
                FrontsCountComboBox.Focus();
            }
        }

        private void FrontsCountComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FrontAddButton_Click(null, null);
                FrontsHeightComboBox.Focus();
            }

            //if (e.KeyCode == Keys.Left)
            //{
            //    if (FrontsWidthComboBox.Enabled)
            //        FrontsWidthComboBox.Focus();
            //    else
            //        FrontsHeightComboBox.Focus();
            //}
        }

        private void FrontsComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(FrontsComboBox, false);
        }

        private void PatinaComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(PatinaComboBox, false);
        }

        private void InsetTypesComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(InsetTypesComboBox, false);
        }

        private void FrameColorComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(FrameColorComboBox, false);
        }

        private void InsetColorComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(InsetColorComboBox, false);
        }

        private void TechnoInsetTypesComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(TechnoInsetTypesComboBox, false);
        }

        private void TechnoInsetColorsComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(TechnoInsetColorsComboBox, false);
        }

        private void FrontsComboBox_Enter(object sender, EventArgs e)
        {
            FrontsComboBox.Focus();
            SetAutoComplete(FrontsComboBox, true);
        }

        private void PatinaComboBox_Enter(object sender, EventArgs e)
        {
            PatinaComboBox.Focus();
            SetAutoComplete(PatinaComboBox, true);
        }

        private void InsetTypesComboBox_Enter(object sender, EventArgs e)
        {
            InsetTypesComboBox.Focus();
            SetAutoComplete(InsetTypesComboBox, true);
        }

        private void FrameColorComboBox_Enter(object sender, EventArgs e)
        {
            FrameColorComboBox.Focus();
            SetAutoComplete(FrameColorComboBox, true);
        }

        private void InsetColorComboBox_Enter(object sender, EventArgs e)
        {
            InsetColorComboBox.Focus();
            SetAutoComplete(InsetColorComboBox, true);
        }

        private void TechnoInsetTypesComboBox_Enter(object sender, EventArgs e)
        {
            TechnoInsetTypesComboBox.Focus();
            SetAutoComplete(TechnoInsetTypesComboBox, true);
        }

        private void TechnoInsetColorsComboBox_Enter(object sender, EventArgs e)
        {
            TechnoInsetColorsComboBox.Focus();
            SetAutoComplete(TechnoInsetColorsComboBox, true);
        }

        private void FrontsHeightComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void FrontsWidthComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void FrontsCountComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void DecorHeightComboBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void DecorWidthComboBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void DecorCountNumUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecorAddButton_Click(null, null);
                DecorProductsComboBox.Focus();
            }
        }

        private void DecorProductsComboBox_Enter(object sender, EventArgs e)
        {
            DecorProductsComboBox.Focus();
            SetAutoComplete(DecorProductsComboBox, true);
        }

        private void DecorItemsComboBox_Enter(object sender, EventArgs e)
        {
            DecorItemsComboBox.Focus();
            SetAutoComplete(DecorItemsComboBox, true);
        }

        private void DecorColorsComboBox_Enter(object sender, EventArgs e)
        {
            DecorColorsComboBox.Focus();

            SetAutoComplete(DecorColorsComboBox, true);
        }

        private void DecorProductsComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(DecorProductsComboBox, false);
        }

        private void DecorItemsComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(DecorItemsComboBox, false);
        }

        private void DecorColorsComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(DecorColorsComboBox, false);
        }

        private void DecorHeightComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void DecorWidthComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }
        //e03d86502ae9876afa62fbd370a83f5d
        private void DecorCountNumUpDown_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void OrdersSetTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void SampleDiscountTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void FrontsDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            FrontsComboBox.Focus();
            FrontsComboBox.DroppedDown = true;
        }

        private void PatinaDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            PatinaComboBox.Focus();
            PatinaComboBox.DroppedDown = true;
        }

        private void InsetTypesDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            InsetTypesComboBox.Focus();
            InsetTypesComboBox.DroppedDown = true;
        }

        private void FrameColorsDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            FrameColorComboBox.Focus();
            FrameColorComboBox.DroppedDown = true;
        }

        private void InsetColorsDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            InsetColorComboBox.Focus();
            InsetColorComboBox.DroppedDown = true;
        }

        private void AdditionalColorsDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            TechnoInsetTypesComboBox.Focus();
            TechnoInsetTypesComboBox.DroppedDown = true;
        }

        private void DecorProductDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            DecorProductsComboBox.Focus();
            DecorProductsComboBox.DroppedDown = true;
        }

        private void DecorItemDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            DecorItemsComboBox.Focus();
            DecorItemsComboBox.DroppedDown = true;
        }

        private void DecorColorDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            DecorColorsComboBox.Focus();
            DecorColorsComboBox.DroppedDown = true;

        }

        private void FrontsDrillTypesDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            FrontsDrillTypesComboBox.Focus();
            FrontsDrillTypesComboBox.DroppedDown = true;
        }

        private void FrontsDrillTypesComboBox_Enter(object sender, EventArgs e)
        {
            FrontsDrillTypesComboBox.Focus();
            SetAutoComplete(FrontsDrillTypesComboBox, true);
        }

        private void FrontsDrillTypesComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetAutoComplete(FrontsDrillTypesComboBox, true);
            }
            //if (e.KeyCode == Keys.Right)
            //{
            //    e.SuppressKeyPress = true;
            //}

            //if (e.KeyCode == Keys.Left)
            //{
            //    FrontHandsSizeTextBox.Focus();
            //}

            //if (e.KeyCode == Keys.Down)
            //{
            //    e.SuppressKeyPress = true;
            //}

            //if (e.KeyCode == Keys.Up)
            //{
            //    FrontsCountComboBox.Focus();
            //    e.SuppressKeyPress = true;
            //}
        }

        private void FrontsDrillTypesComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(FrontsDrillTypesComboBox, false);
        }

        private void FrontHandsSizeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FrontAddButton_Click(null, null);
                FrontsHeightComboBox.Focus();
            }
            //if (e.KeyCode == Keys.Right)
            //{
            //    FrontsDrillTypesComboBox.Focus();
            //    e.SuppressKeyPress = true;
            //}

            //if (e.KeyCode == Keys.Up)
            //{
            //    FrontsHeightComboBox.Focus();
            //    e.SuppressKeyPress = true;
            //}

        }

        private void CupboardsImportButton_Click(object sender, EventArgs e)
        {
            int ImportType = -1;

            if (CupboardsRadioButton.Checked)
                ImportType = 0;
            else
                ImportType = 1;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            CupboardsExportForm CupboardsExportForm = new CupboardsExportForm(this, ref FrontsOrders);

            CupboardsExportForm.SetImportType(ImportType);

            TopForm = CupboardsExportForm;
            CupboardsExportForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            CupboardsExportForm.Dispose();
            TopForm = null;
        }

        private void FrontsAddSizeButton_Click(object sender, EventArgs e)
        {
            FrontsOrders.AddSize();
        }

        private void ExportContextMenuButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            ExportContextMenu.Show(ExportContextMenuButton);
        }

        private void CupboardsFormShowButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            CupboardsForm CupboardsForm = new CupboardsForm(this, ref FrontsOrders);

            TopForm = CupboardsForm;
            CupboardsForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            CupboardsForm.Dispose();
            TopForm = null;
        }

        private void NewOrderButton_Click(object sender, EventArgs e)
        {
            NewOrderInfo.IsEditOrder = false;
            NewOrderInfo.IsNewOrder = true;
            NewOrderInfo.IsDoubleOrder = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewZOVOrderSelectMenu NewOrderSelectMenu = new NewZOVOrderSelectMenu(this, ref OrdersManager, ref OrdersCalculate, ref NewOrderInfo);

            TopForm = NewOrderSelectMenu;
            NewOrderSelectMenu.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewOrderSelectMenu.Dispose();
            TopForm = null;

            if (!NewOrderInfo.bPressOK)
                return;

            NoDecorLabel.Visible = true;

            string DebtType = "Нет";
            string Prepare = "Предварительно";
            string PriceType = "Розница";

            PrepareLabel.Visible = true;
            ClientLabel.Visible = true;
            DocNumberLabel.Visible = true;
            PriceTypeLabel.Visible = true;
            DebtLabel.Visible = true;
            SampleLabel.Visible = true;
            DoNotDispatchLabel.Visible = true;
            DebtLabel.Visible = true;
            NeedCalcLabel.Visible = true;

            if (!NewOrderInfo.IsPrepare)
                Prepare = "Отгрузка: " + Convert.ToDateTime(NewOrderInfo.DispatchDate).ToString("dd.MM.yyyy");

            if (NewOrderInfo.PriceType == 1)
                PriceType = "Опт";
            if (NewOrderInfo.PriceType == 2)
                PriceType = "Стенка";

            if (NewOrderInfo.DebtType == 1)
                DebtType = "Долг";
            if (NewOrderInfo.DebtType == 2)
                DebtType = "Брак";
            if (NewOrderInfo.DebtType == 3)
                DebtType = "Ошибка пр-ва";
            if (NewOrderInfo.DebtType == 4)
                DebtType = "Ошибка ЗОВа";

            PrepareLabel.Text = Prepare + "   ";
            ClientLabel.Text = "Клиент: " + OrdersManager.GetClientName(NewOrderInfo.ClientID) + "   ";
            DocNumberLabel.Text = "Кухня: " + NewOrderInfo.DocNumber + "   ";
            PriceTypeLabel.Text = PriceType + "   ";
            DebtLabel.Text = NewOrderInfo.DebtDocNumber + " " + DebtType + "   ";

            if (NewOrderInfo.IsSample)
                SampleLabel.Visible = true;
            else
                SampleLabel.Visible = false;

            if (NewOrderInfo.DoNotDispatch)
                DoNotDispatchLabel.Visible = true;
            else
                DoNotDispatchLabel.Visible = false;

            if (NewOrderInfo.IsDebt)
                DebtLabel.Visible = true;
            else
                DebtLabel.Visible = false;

            if (NewOrderInfo.NeedCalculate)
                NeedCalcLabel.Visible = true;
            else
                NeedCalcLabel.Visible = false;

            if (!NewOrderInfo.IsNewOrder && !NewOrderInfo.IsDoubleOrder)
            {
                MainOrderNotes.Text = NewOrderInfo.Notes;
                //редактирование фасадов
                FrontsOrdersDataGrid.Visible = FrontsOrders.EditOrder(OrdersManager.CurrentMainOrderID);

                //редактирование декора
                if (DecorOrders.EditDecorOrder(OrdersManager.CurrentMainOrderID))
                    NoDecorLabel.Visible = false;
                else
                    NoDecorLabel.Visible = true;
            }
            else
            {
                FrontsOrders.NewOrder();
                DecorOrders.NewOrder();
            }

            if (NewOrderInfo.IsDoubleOrder)//двойное вбивание
            {
                FCompareOrdersButton.BringToFront();
                DCompareOrdersButton.BringToFront();
            }
            else//новая кухня
            {
                SaveButtonFronts.BringToFront();
                SaveButtonDecor.BringToFront();
            }
            FrontsOrders.GetTempFronts();
            DecorOrders.GetTempDecor();
        }

        private void FrontsOrdersDataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox)
            {
                ((ComboBox)e.Control).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)e.Control).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }
        }

        private void MainOrdersTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (MainOrdersTabControl.SelectedTabPageIndex == 0)
                kryptonCheckSet1.CheckedButton = FrontsCheckButton;
            if (MainOrdersTabControl.SelectedTabPageIndex == 1)
                kryptonCheckSet1.CheckedButton = DecorCheckButton;
        }

        private void DCompareOrdersButton_Click(object sender, EventArgs e)
        {
            FrontsOrders.GetOldFronts(OrdersManager.CurrentMainOrderID);
            FrontsOrders.GetNewFronts();

            DecorOrders.GetDecorOrdersDT();
            bool IsFrontsDifferent = FrontsOrders.FindDifferenceBetweenFronts(OrdersManager.CurrentMainOrderID);
            bool IsDecorDifferent = DecorOrders.FindDifferenceBetweenDecor(OrdersManager.CurrentMainOrderID);

            if (IsFrontsDifferent && IsDecorDifferent)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Заказы совпадают", "Двойное вбивание");

                int FirstOperatorID = OrdersManager.GetFirstOperator(OrdersManager.CurrentMainOrderID);
                int SecondOperatorID = Security.CurrentUserID;
                DateTime FirstDocDateTime = Convert.ToDateTime(OrdersManager.GetFirstDocDateTime(OrdersManager.CurrentMainOrderID));
                DateTime FirstSaveDateTime = Convert.ToDateTime(OrdersManager.GetFirstSaveDateTime(OrdersManager.CurrentMainOrderID));
                DateTime SecondDocDateTime = Convert.ToDateTime(NewOrderInfo.DocDateTime);
                DateTime SecondSaveDateTime = Security.GetCurrentDate();
                //OrdersManager.SetErrorsCount(OrdersManager.CurrentMainOrderID, 0);
                //OrdersManager.SetDoubleOrder(OrdersManager.CurrentMainOrderID, true);
                OrdersManager.SetDoubleOrderParameters(OrdersManager.CurrentMainOrderID, true);
                if (!OrdersManager.PreSaveDoubleOrder(NewOrderInfo.DocNumber, FirstOperatorID, FirstDocDateTime, FirstSaveDateTime, SecondOperatorID))
                {
                    OrdersManager.SaveDoubleOrder(NewOrderInfo.DocNumber, FirstOperatorID, FirstDocDateTime, FirstSaveDateTime, 0,
                        SecondOperatorID, SecondDocDateTime, SecondSaveDateTime, 0);
                }

                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            DoubleOrderForm DoubleOrderForm = new DoubleOrderForm(this,
                OrdersManager, OrdersCalculate, FrontsOrders, DecorOrders, FrontsCatalogOrder, DecorCatalogOrder, NewOrderInfo);

            TopForm = DoubleOrderForm;

            DoubleOrderForm.ShowDialog();

            DoubleOrderForm.Close();
            DoubleOrderForm.Dispose();

            TopForm = null;

            FrontsOrders.ExitEdit();
            DecorOrders.ExitEdit();
        }

        private void AddZOVMainOrdersForm_Load(object sender, EventArgs e)
        {
            FrontsComboBox.SelectionLength = 0;
            FrameColorComboBox.SelectionLength = 0;
            TechnoProfilesComboBox.SelectionLength = 0;
            TechnoFrameColorComboBox.SelectionLength = 0;
            PatinaComboBox.SelectionLength = 0;
            InsetTypesComboBox.SelectionLength = 0;
            InsetColorComboBox.SelectionLength = 0;
            TechnoInsetTypesComboBox.SelectionLength = 0;
            TechnoInsetColorsComboBox.SelectionLength = 0;

            DecorProductsComboBox.SelectionLength = 0;
            DecorItemsComboBox.SelectionLength = 0;
            DecorColorsComboBox.SelectionLength = 0;
            DecorPatinaComboBox.SelectionLength = 0;
            DecorInsetTypesComboBox.SelectionLength = 0;
            DecorInsetColorsComboBox.SelectionLength = 0;

            FrontsComboBox.Focus();
        }

        private void TechnoProfilesDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            TechnoProfilesComboBox.Focus();
            TechnoProfilesComboBox.DroppedDown = true;
        }

        private void TechnoInsetColorsDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            TechnoInsetColorsComboBox.Focus();
            TechnoInsetColorsComboBox.DroppedDown = true;
        }

        private void TechnoFrameColorsDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            TechnoFrameColorComboBox.Focus();
            TechnoFrameColorComboBox.DroppedDown = true;
        }

        private void TechnoProfilesComboBox_Enter(object sender, EventArgs e)
        {
            TechnoProfilesComboBox.Focus();
            SetAutoComplete(TechnoProfilesComboBox, true);
        }

        private void TechnoProfilesComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(TechnoProfilesComboBox, false);
        }

        private void TechnoProfilesComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TechnoProfilesComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(TechnoProfilesComboBox, true);
            }
        }

        private void TechnoFrameColorComboBox_Enter(object sender, EventArgs e)
        {
            TechnoFrameColorComboBox.Focus();
            SetAutoComplete(TechnoFrameColorComboBox, true);
        }

        private void TechnoFrameColorComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TechnoFrameColorComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(TechnoFrameColorComboBox, true);
            }
        }

        private void TechnoFrameColorComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(TechnoFrameColorComboBox, false);
        }

        private void DecorPatinaDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            DecorPatinaComboBox.Focus();
            DecorPatinaComboBox.DroppedDown = true;
        }

        private void DecorPatinaComboBox_Enter(object sender, EventArgs e)
        {
            DecorPatinaComboBox.Focus();
            SetAutoComplete(DecorPatinaComboBox, true);
        }

        private void DecorPatinaComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecorPatinaComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(DecorPatinaComboBox, true);
            }
        }

        private void DecorPatinaComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(DecorPatinaComboBox, false);
        }

        private void DecorPatinaComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetDecorInsetTypes();
            NGetDecorInsetColors();
            NGetDecorLength();
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void DecorInsetTypesComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetDecorInsetColors();
            NGetDecorLength();
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void DecorInsetColorsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            NGetDecorLength();
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void DecorLengthComboBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void DecorLengthComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
                e.Handled = true;
        }

        private void DecorLengthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NGetDecorHeight();
            NGetDecorWidth();
        }

        private void cbOnlyExcluzive_CheckedChanged(object sender, EventArgs e)
        {
            if (FrontsOrders.HasExcluzive)
            {
                FrontsOrders.UpdateExcluziveCatalog();
                FrontsCatalogOrder.FilterFronts(cbOnlyExcluzive.Checked);

                NGetFrameColors();
                NGetTechnoProfiles();
                NGetTechnoFrameColors();
                NGetPatina();
                NGetInsetTypes();
                NGetInsetColors();
                NGetTechnoInsetTypes();
                NGetTechnoInsetColors();
                NGetHeight();
                NGetWidth();
            }
            if (DecorOrders.HasExcluzive)
            {
                DecorOrders.UpdateExcluziveCatalog();
                DecorCatalogOrder.FilterProducts(cbOnlyExcluzive.Checked);

                NGetDecorItems();
                NGetDecorColors();
                NGetDecorPatina();
                NGetDecorInsetTypes();
                NGetDecorInsetColors();
                NGetDecorLength();
                NGetDecorHeight();
                NGetDecorWidth();
            }
        }

        private void FrontsOrdersDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void ExportContextMenuButton_Click(object sender, EventArgs e)
        {

        }
        private void DecorInsetTypesComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(DecorInsetTypesComboBox, false);
        }

        private void DecorInsetColorsComboBox_Leave(object sender, EventArgs e)
        {
            SetAutoComplete(DecorInsetColorsComboBox, false);
        }

        private void DecorInsetTypeDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            DecorInsetTypesComboBox.Focus();
            DecorInsetTypesComboBox.DroppedDown = true;
        }

        private void DecorInsetColorDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
        {
            DecorInsetColorsComboBox.Focus();
            DecorInsetColorsComboBox.DroppedDown = true;
        }

        private void DecorInsetTypesComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecorInsetTypesComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(DecorInsetTypesComboBox, true);
            }
        }

        private void DecorInsetColorsComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecorInsetColorsComboBox_SelectionChangeCommitted(sender, e);
                SetAutoComplete(DecorInsetColorsComboBox, true);
            }
        }

        private void DecorInsetTypesComboBox_Enter(object sender, EventArgs e)
        {
            DecorInsetTypesComboBox.Focus();
            SetAutoComplete(DecorInsetTypesComboBox, true);
        }

        private void DecorInsetColorsComboBox_Enter(object sender, EventArgs e)
        {
            DecorInsetColorsComboBox.Focus();
            SetAutoComplete(DecorInsetColorsComboBox, true);
        }
    }
}
