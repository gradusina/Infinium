using Infinium.Modules.Marketing.Orders;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddMarketingOrdersForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;
        int MainOrderID = -1;

        Form TopForm = null;

        public OrdersManager OrdersManager;

        FrontsCatalogOrder FrontsCatalogOrder = null;
        DecorCatalogOrder DecorCatalogOrder = null;
        FrontsOrders FrontsOrders;
        DecorOrders DecorOrders;
        OrdersCalculate OrdersCalculate;

        public AddMarketingOrdersForm(ref OrdersManager tOrdersManager, ref Form tTopForm, ref OrdersCalculate tOrdersCalculate, bool bDeleteEnable)
        {
            TopForm = tTopForm;

            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            OrdersManager = tOrdersManager;
            OrdersCalculate = tOrdersCalculate;

            DecorRemoveButton.Enabled = false;
            FrontRemoveButton.Enabled = false;
            if (bDeleteEnable)
            {
                DecorRemoveButton.Enabled = true;
                FrontRemoveButton.Enabled = true;
            }
            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void AddMarketMainOrdersForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            FrontsComboBox.SelectionLength = 0;
            FrameColorComboBox.SelectionLength = 0;
            TechnoFrameColorComboBox.SelectionLength = 0;
            PatinaComboBox.SelectionLength = 0;
            InsetTypesComboBox.SelectionLength = 0;
            InsetColorComboBox.SelectionLength = 0;
            TechnoInsetTypesComboBox.SelectionLength = 0;
            TechnoInsetColorsComboBox.SelectionLength = 0;

            FrontsComboBox.Focus();
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
            FrontsCatalogOrder = new FrontsCatalogOrder(ref FrontsHeightComboBox, ref FrontsWidthComboBox);
            FrontsCatalogOrder.Initialize();

            DecorCatalogOrder = new DecorCatalogOrder(ref DecorLengthComboBox, ref DecorHeightComboBox, ref DecorWidthComboBox);
            DecorCatalogOrder.Initialize();

            FrontsOrders = new FrontsOrders(ref FrontsOrdersDataGrid,
                ref FrontsCatalogOrder);
            FrontsOrders.Initialize();

            DecorOrders = new DecorOrders(ref MainOrdersDecorTabControl,
                ref DecorCatalogOrder, ref FrontsOrdersDataGrid);
            DecorOrders.Initialize();

            DateTime DispatchDate = DateTime.Now;
            bool IsSample = false;
            string Notes = null;
            MainOrderID = OrdersManager.CurrentMainOrderID;

            FrontsOrdersDataGrid.Visible = FrontsOrders.EditOrder(MainOrderID);

            if (DecorOrders.EditDecorOrder(MainOrderID))
                NoDecorLabel.Visible = false;
            else
                NoDecorLabel.Visible = true;

            FrontsCatalogOrderBinding();
            DecorCatalogOrderBinding();

            OrdersManager.EditMainOrder(ref Notes, ref IsSample);
            MainOrderNotes.Text = Notes;
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

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (FrontsOrders.HasRows() == false && DecorOrders.HasRows() == false)
            {
                DialogResult result = MessageBox.Show(this, "В заказе нет позиций. Удалить заказ?",
                                "Сохранение заказа", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    OrdersManager.RemoveCurrentMainOrder();
                }

                if (result == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        //ORDER FRONTS CATALOG
        public void NGetFrameColors()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!FrontsOrders.HasExcluzive)
                bExcluzive = false;
            if (FrontsComboBox.SelectedItem == null)
                return;
            if (FrontsCatalogOrder.FrontsBindingSource.Current != null)
            {
                FrontsCatalogOrder.FilterCatalogFrameColors(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogFrameColors(string.Empty, bExcluzive);
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
                    Convert.ToInt32(FrameColorComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogTechnoFrameColors(string.Empty, -1, bExcluzive);
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
                    Convert.ToInt32(FrameColorComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogPatina(string.Empty, -1, -1, bExcluzive);
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
                FrontsCatalogOrder.FilterCatalogInsetTypes(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                    Convert.ToInt32(PatinaComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogInsetTypes(string.Empty, -1, -1, -1, bExcluzive);
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
                FrontsCatalogOrder.FilterCatalogInsetColors(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                    Convert.ToInt32(PatinaComboBox.SelectedValue),
                    Convert.ToInt32(InsetTypesComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogInsetColors(string.Empty, -1, -1, -1, -1, bExcluzive);
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
                FrontsCatalogOrder.FilterCatalogTechnoInsetTypes(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                    Convert.ToInt32(FrameColorComboBox.SelectedValue),
                    Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                    Convert.ToInt32(PatinaComboBox.SelectedValue),
                    Convert.ToInt32(InsetTypesComboBox.SelectedValue),
                    Convert.ToInt32(InsetColorComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogTechnoInsetTypes(string.Empty, -1, -1, -1, -1, -1, bExcluzive);
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
                FrontsCatalogOrder.FilterCatalogTechnoInsetColors(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                                                       Convert.ToInt32(FrameColorComboBox.SelectedValue),
                                                       Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                                                       Convert.ToInt32(PatinaComboBox.SelectedValue),
                                                       Convert.ToInt32(InsetTypesComboBox.SelectedValue),
                                                       Convert.ToInt32(InsetColorComboBox.SelectedValue),
                                                       Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue), bExcluzive);
            }
            else
            {
                FrontsCatalogOrder.FilterCatalogTechnoInsetColors(string.Empty, -1, -1, -1, -1, -1, -1, bExcluzive);
            }
        }

        public void NGetHeight()
        {
            if (FrontsComboBox.SelectedItem == null)
                return;
            FrontsCatalogOrder.FilterCatalogHeight(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                                                            Convert.ToInt32(FrameColorComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                                                            Convert.ToInt32(PatinaComboBox.SelectedValue),
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

            FrontsCatalogOrder.FilterCatalogWidth(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(),
                                                            Convert.ToInt32(FrameColorComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoFrameColorComboBox.SelectedValue),
                                                            Convert.ToInt32(PatinaComboBox.SelectedValue),
                                                            Convert.ToInt32(InsetTypesComboBox.SelectedValue),
                                                            Convert.ToInt32(InsetColorComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoInsetTypesComboBox.SelectedValue),
                                                            Convert.ToInt32(TechnoInsetColorsComboBox.SelectedValue),
                                                            Height);
        }

        public void FrontsCatalogOrderBinding()
        {
            FrontsComboBox.DataSource = FrontsCatalogOrder.FrontsBindingSource;
            FrontsComboBox.DisplayMember = FrontsCatalogOrder.FrontsBindingSourceDisplayMember;
            FrontsComboBox.ValueMember = FrontsCatalogOrder.FrontsBindingSourceValueMember;

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

            bool bExcluzive = true;
            FrontsCatalogOrder.FilterFronts(bExcluzive);
            NGetFrameColors();
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
            if (DecorItemsComboBox.SelectedItem == null)
                return;
            if (DecorItemsComboBox.SelectedItem != null &&
                DecorCatalogOrder.FilterColors(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(), bExcluzive) == true)
            {
                flowLayoutPanel2.Visible = true;
            }
            else
            {
                flowLayoutPanel2.Visible = false;
            }
        }

        public void NGetDecorPatina()
        {
            bool bExcluzive = cbOnlyExcluzive.Checked;
            if (!DecorOrders.HasExcluzive)
                bExcluzive = false;
            if (DecorItemsComboBox.SelectedItem == null)
                return;
            if (DecorItemsComboBox.SelectedItem != null &&
                DecorCatalogOrder.FilterPatina(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
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
            if (DecorCatalogOrder.ItemLengthBindingSource.Count > 0)
                Length = Convert.ToInt32(((DataRowView)DecorCatalogOrder.ItemLengthBindingSource.Current).Row["Length"]);
            if (DecorCatalogOrder.ItemHeightBindingSource.Count > 0)
                Height = Convert.ToInt32(((DataRowView)DecorCatalogOrder.ItemHeightBindingSource.Current).Row["Height"]);
            int W = DecorCatalogOrder.FilterWidth(((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(),
                Convert.ToInt32(DecorColorsComboBox.SelectedValue),
                Convert.ToInt32(DecorPatinaComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue),
                Convert.ToInt32(DecorInsetColorsComboBox.SelectedValue), Length, Height); ;
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
            int FrontID = -1;
            int ColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;
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

            //Цвет наполнителя-2
            if (TechnoInsetColorsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Цвет наполнителя-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(this, "Неверное количество фасадов", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Count = Convert.ToInt32(FrontsCountComboBox.Text);
            //смещение для импоста
            if (!string.IsNullOrWhiteSpace(cbImpostMargin.Text) && Convert.ToInt32(cbImpostMargin.Text) > 0)
            {
                ImpostMargin = Convert.ToInt32(cbImpostMargin.Text);
            }
            int FactoryID = 0;
            int AreaID = 0;
            int FrontConfigID = FrontsOrders.FrontsCatalogOrder.GetFrontConfigID(((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(), ColorID, PatinaID, InsetTypeID,
                   InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref FrontID, ref FactoryID, ref AreaID);
            if (FrontID == -1)
                return;
            FrontsOrders.AddFrontsOrder(MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID,
                TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, FrontsOrdersNotesTextBox.Text, ImpostMargin);
            cbImpostMargin.ResetText();
        }

        private void FrontRemoveButton_Click(object sender, EventArgs e)
        {
            //int FrontsOrdersID = 0;
            //if (FrontsOrdersDataGrid.SelectedRows.Count != 0 && FrontsOrdersDataGrid.SelectedRows[0].Cells["FrontsOrdersID"].Value != DBNull.Value)
            //    FrontsOrdersID = Convert.ToInt32(FrontsOrdersDataGrid.SelectedRows[0].Cells["FrontsOrdersID"].Value);
            //if (FrontsOrdersID == 0)
            FrontsOrders.RemoveOrder();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            int MegaOrderID = OrdersManager.CurrentMegaOrderID;

            int FactoryIDF = -1;
            int FactoryIDD = -1;

            int F = 0;

            if (FrontsOrders.HasZeroCount || DecorOrders.HasZeroCount)
            {
                MessageBox.Show("Кол-во не может быть равно 0");
                return;
            }

            if (FrontsOrders.HasRows() == false && DecorOrders.HasRows() == false)
            {
                return;
            }

            if (FrontsOrders.SaveFrontsOrder(MainOrderID, ref FactoryIDF) == false)
                return;

            if (DecorOrders.SaveDecorOrder(MainOrderID, ref FactoryIDD) == false)
                return;
            DecorOrders.DeleteDecorAssignmentByDecorOrders(MainOrderID);

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

            OrdersManager.SaveOrder(MainOrderNotes.Text, F, false);
            InvoiceReportToDBF DBFReport = new InvoiceReportToDBF(ref DecorCatalogOrder, ref OrdersCalculate.FrontsCalculate);
            decimal CurrencyTotalCost = DBFReport.CalcCurrencyCost(
                Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]),
                Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]), OrdersManager.PaymentCurrency);

            decimal DiscountPaymentCondition = OrdersManager.DiscountPaymentCondition(OrdersManager.CurrentDiscountPaymentConditionID);
            if (OrdersManager.CurrentDiscountPaymentConditionID == 4)
            {
                DiscountPaymentCondition = OrdersManager.DiscountFactoring(OrdersManager.CurrentDiscountFactoringID);
            }

            OrdersCalculate.Recalculate(MegaOrderID, OrdersManager.CurrentProfilDiscountDirector, OrdersManager.CurrentTPSDiscountDirector,
                OrdersManager.CurrentProfilTotalDiscount, OrdersManager.CurrentTPSTotalDiscount, DiscountPaymentCondition, OrdersManager.CurrencyTypeID,
                OrdersManager.PaymentCurrency, OrdersManager.ConfirmDateTime, CurrencyTotalCost, false);
            //OrdersManager.SetCurrencyCost(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), CurrencyTotalCost);

            //if (OrdersManager.NeedSetStatus)
            //{
            //    //OrdersManager.SetCurrentMegaOrderStatus();
            //    CheckOrdersStatus.SetMegaOrderStatus(MegaOrderID);
            //}
            CheckOrdersStatus.SetMegaOrderStatus(MegaOrderID);
            //OrdersManager.SummaryCost(OrdersManager.CurrentMegaOrderID);
            OrdersManager.FixOrderEvent(MegaOrderID, "Отредактирован подзаказ");
            InfiniumTips.ShowTip(this, 50, 85, "Заказ сохранён", 1700);
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void DecorAddButton_Click(object sender, EventArgs e)
        {
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

            if (flowLayoutPanel2.Visible)
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
            if (panel5.Visible)
                if (DecorInsetTypesComboBox.Text != "")
                    InsetTypeID = Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - тип наполнителя");
                    return;
                }
            if (panel6.Visible)
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
            DecorOrders.AddDecorOrder(MainOrderID, ProductID, ItemID, ColorID, PatinaID, InsetTypeID, InsetColorID, Length, Height, Width, Count, DecorOrdersNotesTextBox.Text);
            NoDecorLabel.Visible = false;
        }

        private void DecorRemoveButton_Click(object sender, EventArgs e)
        {
            if (MainOrdersDecorTabControl.SelectedTabPageIndex == -1)
                return;

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
                FrontsPanel1.BringToFront();
                MainOrdersTabControl.SelectedTabPageIndex = 0;

                FrontsComboBox.SelectionLength = 0;
                FrameColorComboBox.SelectionLength = 0;
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
                MainOrdersTabControl.SelectedTabPageIndex = 1;
                DecorPanel1.BringToFront();
                DecorProductsComboBox.SelectionLength = 0;
                DecorItemsComboBox.SelectionLength = 0;
                DecorColorsComboBox.SelectionLength = 0;
                DecorPatinaComboBox.SelectionLength = 0;
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

        private void TechnoInsetTypesDropButton_DropDown(object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e)
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

        public void AddFrontsFromSizeTable(ref PercentageDataGrid DataGrid)
        {
            int MainOrderID = -1;
            //int FrontID = -1;
            int ColorID = -1;
            int PatinaID = -1;
            int InsetTypeID = -1;
            int InsetColorID = -1;
            int TechnoColorID = -1;
            int TechnoInsetTypeID = -1;
            int TechnoInsetColorID = -1;
            //int Height = 0;
            //int Width = 0;

            //фасад
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

            //Цвет наполнителя-2
            if (TechnoInsetColorsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Не выбран Цвет наполнителя-2 или нет в базе", "Ошибка добавления фасада", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TechnoInsetColorID = Convert.ToInt32(TechnoInsetColorsComboBox.SelectedValue);
            //int FactoryID = 0;
            FrontsOrders.AddFrontsOrderFromSizeTable(ref DataGrid, MainOrderID, ((DataRowView)FrontsComboBox.SelectedItem).Row["FrontName"].ToString(), ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID);
        }

        public void AddDecorFromSizeTable(ref PercentageDataGrid DataGrid)
        {
            int MainOrderID = -1;
            int ProductID = Convert.ToInt32(DecorProductsComboBox.SelectedValue);
            int ItemID = -1;
            int ColorID = 0;
            int PatinaID = 0;
            int InsetTypeID = 0;
            int InsetColorID = 0;
            int Length = 0;
            int Height = 0;
            int Width = 0;
            int FactoryID = 0;

            if (flowLayoutPanel2.Visible)
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
            if (panel5.Visible)
                if (DecorInsetTypesComboBox.Text != "")
                    InsetTypeID = Convert.ToInt32(DecorInsetTypesComboBox.SelectedValue);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - тип наполнителя");
                    return;
                }
            if (panel6.Visible)
                if (DecorInsetColorsComboBox.Text != "")
                    InsetColorID = Convert.ToInt32(DecorInsetColorsComboBox.SelectedValue);
                else
                {
                    MessageBox.Show("Введены не все параметры продукта - цвет наполнителя");
                    return;
                }

            int AreaID = 0;
            DecorOrders.DecorCatalogOrder.GetDecorConfigID(ProductID,
                ((DataRowView)DecorItemsComboBox.SelectedItem).Row["Name"].ToString(), ColorID, PatinaID, InsetTypeID, InsetColorID,
                Length, Height, Width, ref ItemID, ref FactoryID, ref AreaID);
            if (ItemID == -1)
                return;
            DecorOrders.AddDecorOrderFromSizeTable(
                ref DataGrid, MainOrderID, ProductID, ItemID, ColorID, PatinaID, InsetTypeID, InsetColorID, DecorOrdersNotesTextBox.Text);
            NoDecorLabel.Visible = false;
        }

        private void MainOrdersTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (MainOrdersTabControl.SelectedTabPageIndex == 0)
                kryptonCheckSet1.CheckedButton = FrontsCheckButton;
            if (MainOrdersTabControl.SelectedTabPageIndex == 1)
                kryptonCheckSet1.CheckedButton = DecorCheckButton;
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
            //if (FrontsOrders.HasExcluzive)
            //{
            FrontsOrders.UpdateExcluziveCatalog();
            FrontsCatalogOrder.FilterFronts(cbOnlyExcluzive.Checked);

            NGetFrameColors();
            NGetTechnoFrameColors();
            NGetPatina();
            NGetInsetTypes();
            NGetInsetColors();
            NGetTechnoInsetTypes();
            NGetTechnoInsetColors();
            NGetHeight();
            NGetWidth();
            //}
            //if (DecorOrders.HasExcluzive)
            //{
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
            //}
        }

        private void FrontsOrdersDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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
