using Infinium.Modules.ZOV;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DoubleOrderForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form TopForm = null;

        FrontsCatalogOrder FrontsCatalogOrder = null;
        DecorCatalogOrder DecorCatalogOrder = null;
        DoubleFrontsOrders DoubleFrontsOrders;
        DoubleDecorOrders DoubleDecorOrders;
        OrdersManager OrdersManager;
        OrdersCalculate OrdersCalculate;
        FrontsOrders FrontsOrders;
        DecorOrders DecorOrders;
        NewOrderInfo NewOrderInfo;

        public DoubleOrderForm(Form tTopForm, OrdersManager tOrdersManager, OrdersCalculate tOrdersCalculate,
            FrontsOrders tFrontsOrders, DecorOrders tDecorOrders,
            FrontsCatalogOrder tFrontsCatalogOrder, DecorCatalogOrder tDecorCatalogOrder, NewOrderInfo tNewOrderInfo)
        {
            TopForm = tTopForm;

            InitializeComponent();
            OrdersManager = tOrdersManager;
            OrdersCalculate = tOrdersCalculate;
            FrontsOrders = tFrontsOrders;
            DecorOrders = tDecorOrders;
            FrontsCatalogOrder = tFrontsCatalogOrder;
            DecorCatalogOrder = tDecorCatalogOrder;
            NewOrderInfo = tNewOrderInfo;
            Initialize();
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            while (!SplashForm.bCreated) ;
        }

        private void DoubleOrderForm_Shown(object sender, EventArgs e)
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
            DoubleFrontsOrders = new DoubleFrontsOrders(ref FrontsCatalogOrder);
            DoubleFrontsOrders.Initialize();
            DoubleFrontsOrders.GetFrontsOrders(OrdersManager.CurrentMainOrderID);
            OldFrontsOrdersDataGrid.DataSource = DoubleFrontsOrders.OldFrontsOrdersList;
            NewFrontsOrdersDataGrid.DataSource = FrontsOrders.FrontsOrdersBindingSource;
            OldFrontsGridSettings();
            NewFrontsGridSettings();

            DoubleDecorOrders = new DoubleDecorOrders(ref DecorCatalogOrder);
            DoubleDecorOrders.Initialize();
            DoubleDecorOrders.GetDecorOrders(OrdersManager.CurrentMainOrderID);
            OldDecorOrdersDataGrid.DataSource = DoubleDecorOrders.OldDecorOrdersList;
            NewDecorOrdersDataGrid.DataSource = DecorOrders.DecorOrdersBindingSource;
            NewDecorGridSettings();
            OldDecorGridSettings();
            if (!DoubleFrontsOrders.HasFronts)
            {
                MainOrdersTabControl.SelectedTabPageIndex = 1;
                NewMainOrdersTabControl.SelectedTabPageIndex = 1;
            }
        }

        private void OldFrontsGridSettings()
        {
            OldFrontsOrdersDataGrid.AutoGenerateColumns = false;

            //добавление столбцов
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.FrontsColumn);
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.PatinaColumn);
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.InsetTypesColumn);
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.FrameColorsColumn);
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.InsetColorsColumn);
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.TechnoFrameColorsColumn);
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.TechnoInsetTypesColumn);
            OldFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.TechnoInsetColorsColumn);

            //убирание лишних столбцов
            if (OldFrontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                OldFrontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                OldFrontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                OldFrontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                OldFrontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (OldFrontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                OldFrontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (OldFrontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                OldFrontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            if (OldFrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                OldFrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;

            if (OldFrontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                OldFrontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (OldFrontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                OldFrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;


            if (!Security.PriceAccess)
            {
                OldFrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                OldFrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                OldFrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            OldFrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            OldFrontsOrdersDataGrid.Columns["Debt"].Visible = false;

            OldFrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            OldFrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;

            int DisplayIndex = 0;
            OldFrontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            OldFrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;

            foreach (DataGridViewColumn Column in OldFrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            OldFrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
            OldFrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            OldFrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            OldFrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            OldFrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            OldFrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            OldFrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            OldFrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\n  фасад";
            OldFrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            OldFrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";

            OldFrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldFrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            OldFrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["Height"].Width = 85;
            OldFrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["Width"].Width = 85;
            OldFrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["Count"].Width = 85;
            OldFrontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["Cost"].Width = 120;
            OldFrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["Square"].Width = 100;
            OldFrontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["FrontPrice"].Width = 85;
            OldFrontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["InsetPrice"].Width = 85;
            OldFrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldFrontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            OldFrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            OldFrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
        }

        private void NewFrontsGridSettings()
        {
            NewFrontsOrdersDataGrid.AutoGenerateColumns = false;

            //добавление столбцов
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.FrontsColumn);
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.FrameColorsColumn);
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.TechnoFrameColorsColumn);
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.PatinaColumn);
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.InsetTypesColumn);
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.InsetColorsColumn);
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.TechnoInsetTypesColumn);
            NewFrontsOrdersDataGrid.Columns.Add(DoubleFrontsOrders.TechnoInsetColorsColumn);

            //убирание лишних столбцов
            if (NewFrontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                NewFrontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                NewFrontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                NewFrontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                NewFrontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (NewFrontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                NewFrontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (NewFrontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                NewFrontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            if (NewFrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                NewFrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;

            if (NewFrontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                NewFrontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (NewFrontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                NewFrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;


            if (!Security.PriceAccess)
            {
                NewFrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                NewFrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                NewFrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            NewFrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            NewFrontsOrdersDataGrid.Columns["Debt"].Visible = false;

            NewFrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            NewFrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;

            int DisplayIndex = 0;
            NewFrontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            NewFrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;

            foreach (DataGridViewColumn Column in NewFrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            NewFrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
            NewFrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            NewFrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            NewFrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            NewFrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            NewFrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            NewFrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            NewFrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\n  фасад";
            NewFrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            NewFrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";

            NewFrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewFrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            NewFrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["Height"].Width = 85;
            NewFrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["Width"].Width = 85;
            NewFrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["Count"].Width = 85;
            NewFrontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["Cost"].Width = 120;
            NewFrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["Square"].Width = 100;
            NewFrontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["FrontPrice"].Width = 85;
            NewFrontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["InsetPrice"].Width = 85;
            NewFrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewFrontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            NewFrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            NewFrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
        }

        private void OldDecorGridSettings()
        {
            OldDecorOrdersDataGrid.AutoGenerateColumns = false;

            OldDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.ProductColumn);
            OldDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.ItemColumn);
            OldDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.ColorColumn);
            OldDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.PatinaColumn);

            //убирание лишних столбцов
            if (OldDecorOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                OldDecorOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                OldDecorOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                OldDecorOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                OldDecorOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (OldDecorOrdersDataGrid.Columns.Contains("CreateUserID"))
                OldDecorOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (OldDecorOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                OldDecorOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            OldDecorOrdersDataGrid.Columns["DecorOrderID"].Visible = false;
            OldDecorOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            OldDecorOrdersDataGrid.Columns["ProductID"].Visible = false;
            OldDecorOrdersDataGrid.Columns["ColorID"].Visible = false;
            OldDecorOrdersDataGrid.Columns["PatinaID"].Visible = false;
            OldDecorOrdersDataGrid.Columns["DecorID"].Visible = false;
            OldDecorOrdersDataGrid.Columns["FactoryID"].Visible = false;

            if (!Security.PriceAccess)
            {
                OldDecorOrdersDataGrid.Columns["Price"].Visible = false;
                OldDecorOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            OldDecorOrdersDataGrid.ScrollBars = ScrollBars.Both;

            OldDecorOrdersDataGrid.Columns["Debt"].Visible = false;

            OldDecorOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            OldDecorOrdersDataGrid.Columns["Weight"].Visible = false;
            OldDecorOrdersDataGrid.Columns["DecorConfigID"].Visible = false;

            OldDecorOrdersDataGrid.Columns["ProductColumn"].DisplayIndex = 0;
            OldDecorOrdersDataGrid.Columns["ItemColumn"].DisplayIndex = 1;
            OldDecorOrdersDataGrid.Columns["ColorsColumn"].DisplayIndex = 2;
            OldDecorOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = 3;

            foreach (DataGridViewColumn Column in OldDecorOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            OldDecorOrdersDataGrid.Columns["Length"].HeaderText = "Длина";
            OldDecorOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            OldDecorOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            OldDecorOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            OldDecorOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            OldDecorOrdersDataGrid.Columns["Price"].HeaderText = "Цена";
            OldDecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";

            OldDecorOrdersDataGrid.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldDecorOrdersDataGrid.Columns["ProductColumn"].MinimumWidth = 120;
            OldDecorOrdersDataGrid.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldDecorOrdersDataGrid.Columns["ItemColumn"].MinimumWidth = 120;
            OldDecorOrdersDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldDecorOrdersDataGrid.Columns["ColorsColumn"].MinimumWidth = 165;
            OldDecorOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OldDecorOrdersDataGrid.Columns["PatinaColumn"].MinimumWidth = 120;
            OldDecorOrdersDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldDecorOrdersDataGrid.Columns["Length"].Width = 85;
            OldDecorOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldDecorOrdersDataGrid.Columns["Height"].Width = 85;
            OldDecorOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldDecorOrdersDataGrid.Columns["Width"].Width = 85;
            OldDecorOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldDecorOrdersDataGrid.Columns["Count"].Width = 85;
            OldDecorOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldDecorOrdersDataGrid.Columns["Cost"].Width = 120;
            OldDecorOrdersDataGrid.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OldDecorOrdersDataGrid.Columns["Price"].Width = 85;
            OldDecorOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            OldDecorOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
        }

        private void NewDecorGridSettings()
        {
            NewDecorOrdersDataGrid.AutoGenerateColumns = false;

            NewDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.ProductColumn);
            NewDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.ItemColumn);
            NewDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.ColorColumn);
            NewDecorOrdersDataGrid.Columns.Add(DoubleDecorOrders.PatinaColumn);

            //убирание лишних столбцов
            if (NewDecorOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                NewDecorOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                NewDecorOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                NewDecorOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                NewDecorOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (NewDecorOrdersDataGrid.Columns.Contains("CreateUserID"))
                NewDecorOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (NewDecorOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                NewDecorOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            NewDecorOrdersDataGrid.Columns["DecorOrderID"].Visible = false;
            NewDecorOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            NewDecorOrdersDataGrid.Columns["ProductID"].Visible = false;
            NewDecorOrdersDataGrid.Columns["ColorID"].Visible = false;
            NewDecorOrdersDataGrid.Columns["PatinaID"].Visible = false;
            NewDecorOrdersDataGrid.Columns["DecorID"].Visible = false;
            NewDecorOrdersDataGrid.Columns["FactoryID"].Visible = false;

            if (!Security.PriceAccess)
            {
                NewDecorOrdersDataGrid.Columns["Price"].Visible = false;
                NewDecorOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            NewDecorOrdersDataGrid.ScrollBars = ScrollBars.Both;

            NewDecorOrdersDataGrid.Columns["Debt"].Visible = false;

            NewDecorOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            NewDecorOrdersDataGrid.Columns["Weight"].Visible = false;
            NewDecorOrdersDataGrid.Columns["DecorConfigID"].Visible = false;

            NewDecorOrdersDataGrid.Columns["ProductColumn"].DisplayIndex = 0;
            NewDecorOrdersDataGrid.Columns["ItemColumn"].DisplayIndex = 1;
            NewDecorOrdersDataGrid.Columns["ColorsColumn"].DisplayIndex = 2;
            NewDecorOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = 3;

            foreach (DataGridViewColumn Column in NewDecorOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            NewDecorOrdersDataGrid.Columns["Length"].HeaderText = "Длина";
            NewDecorOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            NewDecorOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            NewDecorOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            NewDecorOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            NewDecorOrdersDataGrid.Columns["Price"].HeaderText = "Цена";
            NewDecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";

            NewDecorOrdersDataGrid.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewDecorOrdersDataGrid.Columns["ProductColumn"].MinimumWidth = 120;
            NewDecorOrdersDataGrid.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewDecorOrdersDataGrid.Columns["ItemColumn"].MinimumWidth = 120;
            NewDecorOrdersDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewDecorOrdersDataGrid.Columns["ColorsColumn"].MinimumWidth = 165;
            NewDecorOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            NewDecorOrdersDataGrid.Columns["PatinaColumn"].MinimumWidth = 120;
            NewDecorOrdersDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewDecorOrdersDataGrid.Columns["Length"].Width = 85;
            NewDecorOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewDecorOrdersDataGrid.Columns["Height"].Width = 85;
            NewDecorOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewDecorOrdersDataGrid.Columns["Width"].Width = 85;
            NewDecorOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewDecorOrdersDataGrid.Columns["Count"].Width = 85;
            NewDecorOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewDecorOrdersDataGrid.Columns["Cost"].Width = 120;
            NewDecorOrdersDataGrid.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            NewDecorOrdersDataGrid.Columns["Price"].Width = 85;
            NewDecorOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            NewDecorOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
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
            {
                OldFrontsToolsPanel.BringToFront();
                NewMainOrdersTabControl.SelectedTabPageIndex = 0;
                kryptonCheckSet1.CheckedButton = FrontsCheckButton;
            }
            if (MainOrdersTabControl.SelectedTabPageIndex == 1)
            {
                OldDecorToolsPanel.BringToFront();
                NewMainOrdersTabControl.SelectedTabPageIndex = 1;
                kryptonCheckSet1.CheckedButton = DecorCheckButton;
            }
        }

        public void PaintFrontsOrders(ref PercentageDataGrid DataGrid)
        {
            for (int i = 0; i < DataGrid.Rows.Count; i++)
            {
                bool NeedPaintRow = FrontsOrders.ColorRow(
                    Convert.ToInt32(DataGrid.Rows[i].Cells["FrontID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["ColorID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["PatinaID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["InsetTypeID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["InsetColorID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["TechnoInsetTypeID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["TechnoInsetColorID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["Height"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["Width"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["Count"].Value));

                if (NeedPaintRow)
                {
                    DataGrid.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    DataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                    DataGrid.Rows[i].DefaultCellStyle.BackColor = Security.GridsBackColor;
                    DataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        public void PaintDecorOrders(ref PercentageDataGrid DataGrid)
        {
            for (int i = 0; i < DataGrid.Rows.Count; i++)
            {
                bool NeedPaintRow = DecorOrders.ColorRow(
                    Convert.ToInt32(DataGrid.Rows[i].Cells["ProductID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["DecorID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["ColorID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["PatinaID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["InsetTypeID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["InsetColorID"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["Length"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["Height"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["Width"].Value),
                    Convert.ToInt32(DataGrid.Rows[i].Cells["Count"].Value));

                if (NeedPaintRow)
                {
                    DataGrid.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    DataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                    DataGrid.Rows[i].DefaultCellStyle.BackColor = Security.GridsBackColor;
                    DataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void DoubleOrderForm_Load(object sender, EventArgs e)
        {
        }

        private void NewMainOrdersTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (NewMainOrdersTabControl.SelectedTabPageIndex == 0)
            {
                NewFrontsToolsPanel.BringToFront();
                MainOrdersTabControl.SelectedTabPageIndex = 0;
                kryptonCheckSet1.CheckedButton = FrontsCheckButton;
            }
            if (NewMainOrdersTabControl.SelectedTabPageIndex == 1)
            {
                NewDecorToolsPanel.BringToFront();
                MainOrdersTabControl.SelectedTabPageIndex = 1;
                kryptonCheckSet1.CheckedButton = DecorCheckButton;
            }
        }

        private void btnAddDispatch_Click(object sender, EventArgs e)
        {
            DoubleFrontsOrders.AddOrder();
        }

        private void btnDeleteDispatch_Click(object sender, EventArgs e)
        {
            DoubleFrontsOrders.RemoveOrder();
        }

        private void btnSaveOrderFOld_Click(object sender, EventArgs e)
        {
            if (!FrontsOrders.AreTablesEquals(FrontsOrders.CurrentFrontsOrdersDT, DoubleFrontsOrders.CurrentFrontsOrdersDT))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Фасады не совпадают", "Двойное вбивание");

                return;
            }

            if (!DecorOrders.AreTablesEquals(DecorOrders.CurrentDecorOrdersDT, DoubleDecorOrders.CurrentDecorOrdersDT))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Декор не совпадает", "Двойное вбивание");

                return;
            }

            int FactoryIDF = -1;
            int FactoryIDD = -1;
            int F = 0;
            int FirstOperatorID = OrdersManager.GetFirstOperator(OrdersManager.CurrentMainOrderID);
            int SecondOperatorID = Security.CurrentUserID;
            DateTime FirstDocDateTime = Convert.ToDateTime(OrdersManager.GetFirstDocDateTime(OrdersManager.CurrentMainOrderID));
            DateTime FirstSaveDateTime = Convert.ToDateTime(OrdersManager.GetFirstSaveDateTime(OrdersManager.CurrentMainOrderID));
            DateTime SecondDocDateTime = Convert.ToDateTime(NewOrderInfo.DocDateTime);
            DateTime SecondSaveDateTime = Security.GetCurrentDate();

            if (DoubleFrontsOrders.PreSaveFrontOrder(OrdersManager.CurrentMainOrderID, ref FactoryIDF) == false)
                return;
            if (DoubleDecorOrders.PreSaveDecorOrder(OrdersManager.CurrentMainOrderID, ref FactoryIDD) == false)
                return;

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

            int FirstErrorsCount = 0;
            int SecondErrorsCount = 0;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение заказа.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            FrontsOrders.ErrosInOldOrder(ref FirstErrorsCount);
            FrontsOrders.ErrosInNewOrder(ref SecondErrorsCount);
            DoubleFrontsOrders.SaveFrontsOrder();
            DecorOrders.ErrosInOldOrder(ref FirstErrorsCount);
            DecorOrders.ErrosInNewOrder(ref SecondErrorsCount);
            DoubleDecorOrders.SaveDecorOrder();

            OrdersManager.SaveOrder(NewOrderInfo.DispatchDate, OrdersManager.CurrentMainOrderID, NewOrderInfo.ClientID, NewOrderInfo.DocNumber, NewOrderInfo.DebtDocNumber,
                NewOrderInfo.DebtType, NewOrderInfo.IsSample, NewOrderInfo.IsPrepare,
                NewOrderInfo.PriceType, NewOrderInfo.Notes, F, NewOrderInfo.NeedCalculate, NewOrderInfo.DoNotDispatch, NewOrderInfo.TechDrilling, NewOrderInfo.QuicklyOrder,
                NewOrderInfo.ToAssembly, NewOrderInfo.IsNotPaid);
            OrdersManager.SetDoubleOrderParameters(OrdersManager.CurrentMainOrderID, true);
            OrdersCalculate.CalculateOrder(OrdersManager.CurrentMainOrderID, NewOrderInfo.PriceType, NewOrderInfo.IsSample, NewOrderInfo.TechDrilling);

            OrdersManager.SummaryCalcCurrentMegaOrder();

            if (!OrdersManager.PreSaveDoubleOrder(NewOrderInfo.DocNumber, FirstOperatorID, FirstDocDateTime, FirstSaveDateTime, SecondOperatorID))
            {
                OrdersManager.SaveDoubleOrder(NewOrderInfo.DocNumber, FirstOperatorID, FirstDocDateTime, FirstSaveDateTime, FirstErrorsCount,
                    SecondOperatorID, SecondDocDateTime, SecondSaveDateTime, SecondErrorsCount);
            }
            string DD = OrdersManager.GetCurrentMegaOrderDispatchDate(OrdersManager.CurrentMegaOrderID);

            if (DD.Length > 0)
            {
                if (Convert.ToDateTime(DD) != Convert.ToDateTime(NewOrderInfo.DispatchDate))
                    OrdersManager.TotalCalcMegaOrder(Convert.ToDateTime(NewOrderInfo.DispatchDate));
            }
            else
                if (NewOrderInfo.DispatchDate != null)
                OrdersManager.TotalCalcMegaOrder(Convert.ToDateTime(NewOrderInfo.DispatchDate));

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            InfiniumTips.ShowTip(this, 50, 85, "Заказ сохранён", 1700);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            //if (DoubleFrontsOrders == null || DoubleDecorOrders == null)
            //    return;

            //if (kryptonCheckSet1.CheckedIndex == 0)
            //{
            //    MainOrdersTabControl.SelectedTabPageIndex = 0;
            //    NewMainOrdersTabControl.SelectedTabPageIndex = 0;
            //}
            //if (kryptonCheckSet1.CheckedIndex == 1)
            //{
            //    MainOrdersTabControl.SelectedTabPageIndex = 1;
            //    NewMainOrdersTabControl.SelectedTabPageIndex = 1;
            //}
        }

        private void btnAddDecorOrder_Click(object sender, EventArgs e)
        {
            DoubleDecorOrders.AddOrder();
        }

        private void btnDeleteDecorOrder_Click(object sender, EventArgs e)
        {
            DoubleDecorOrders.RemoveOrder();
        }

        private void btnSaveOrderFNew_Click(object sender, EventArgs e)
        {
            //int FactoryIDF = -1;
            //int FactoryIDD = -1;
            //int F = 0;
            //int FirstOperatorID = OrdersManager.GetFirstOperator(OrdersManager.CurrentMainOrderID);
            //int SecondOperatorID = Security.CurrentUserID;
            //DateTime FirstDocDateTime = Convert.ToDateTime(OrdersManager.GetFirstDocDateTime(OrdersManager.CurrentMainOrderID));
            //DateTime FirstSaveDateTime = Convert.ToDateTime(OrdersManager.GetFirstSaveDateTime(OrdersManager.CurrentMainOrderID));
            //DateTime SecondDocDateTime = Convert.ToDateTime(NewOrderInfo.DocDateTime);
            //DateTime SecondSaveDateTime = Security.GetCurrentDate();

            //OrdersManager.RemoveCurrentMainOrder(OrdersManager.CurrentMainOrderID);
            //OrdersManager.RefreshMainOrders();

            //OrdersManager.CreateNewMainOrder(NewOrderInfo.DispatchDate, Convert.ToDateTime(NewOrderInfo.DocDateTime), NewOrderInfo.ClientID, NewOrderInfo.DocNumber,
            //    NewOrderInfo.DebtType, NewOrderInfo.IsSample, NewOrderInfo.IsPrepare,
            //    NewOrderInfo.PriceType, NewOrderInfo.Notes, FirstOperatorID, SecondOperatorID);

            //if (DoubleFrontsOrders.PreSaveFrontOrder(OrdersManager.CurrentMainOrderID, ref FactoryIDF) == false)
            //    return;
            //if (DoubleDecorOrders.PreSaveDecorOrder(OrdersManager.CurrentMainOrderID, ref FactoryIDD) == false)
            //    return;

            //if (FactoryIDF == 1)
            //    if (FactoryIDD == 1)
            //        F = 1;
            //    else if (FactoryIDD == -1)
            //        F = 1;
            //    else
            //        F = 0;

            //if (FactoryIDF == 2)
            //    if (FactoryIDD == 2)
            //        F = 2;
            //    else if (FactoryIDD == -1)
            //        F = 2;
            //    else
            //        F = 0;

            //if (FactoryIDF == -1)
            //    F = FactoryIDD;

            //int ErrorsCount = 0;

            //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение заказа.\r\nПодождите..."); });
            //T.Start();

            //while (!SplashWindow.bSmallCreated) ;           

            //FrontsOrders.ErrosInOldOrder(ref ErrorsCount);
            //FrontsOrders.ChangeMainOrder(OrdersManager.CurrentMainOrderID);
            //FrontsOrders.SaveFrontsOrder(OrdersManager.CurrentMainOrderID, ref F, true);

            //DecorOrders.ErrosInOldOrder(ref ErrorsCount);
            //DecorOrders.ChangeMainOrder(OrdersManager.CurrentMainOrderID);
            //DecorOrders.SaveOrder(ref F);

            ////OrdersManager.SetErrorsCount(OrdersManager.CurrentMainOrderID, ErrorsCount);

            //OrdersManager.SaveOrder(NewOrderInfo.DispatchDate, OrdersManager.CurrentMainOrderID, NewOrderInfo.ClientID, NewOrderInfo.DocNumber, NewOrderInfo.DebtDocNumber,
            //    NewOrderInfo.DebtType, NewOrderInfo.IsSample, NewOrderInfo.IsPrepare,
            //    NewOrderInfo.PriceType, NewOrderInfo.Notes, F, NewOrderInfo.NeedCalculate, NewOrderInfo.DoNotDispatch, NewOrderInfo.TechDrilling, NewOrderInfo.QuicklyOrder);
            ////OrdersManager.SetDoubleOrder(OrdersManager.CurrentMainOrderID, true);
            //OrdersManager.SetDoubleOrderParameters(OrdersManager.CurrentMainOrderID, true, ErrorsCount);
            //OrdersCalculate.CalculateOrder(OrdersManager.CurrentMainOrderID, NewOrderInfo.PriceType);

            //OrdersManager.SummaryCalcCurrentMegaOrder();
            //if (!OrdersManager.PreSaveDoubleOrder(NewOrderInfo.DocNumber, FirstOperatorID, FirstDocDateTime, FirstSaveDateTime, SecondOperatorID))
            //{
            //    OrdersManager.SaveDoubleOrder(NewOrderInfo.DocNumber, FirstOperatorID, FirstDocDateTime, FirstSaveDateTime, ErrorsCount,
            //        SecondOperatorID, SecondDocDateTime, SecondSaveDateTime);
            //}

            //string DD = OrdersManager.GetCurrentMegaOrderDispatchDate(OrdersManager.CurrentMegaOrderID);

            //if (DD.Length > 0)
            //{
            //    if (Convert.ToDateTime(DD) != Convert.ToDateTime(NewOrderInfo.DispatchDate))
            //        OrdersManager.TotalCalcMegaOrder(Convert.ToDateTime(NewOrderInfo.DispatchDate));
            //}
            //else
            //    if (NewOrderInfo.DispatchDate != null)
            //        OrdersManager.TotalCalcMegaOrder(Convert.ToDateTime(NewOrderInfo.DispatchDate));

            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;

            //InfiniumTips.ShowTip(this, 50, 85, "Заказ сохранён", 1700);

            //FormEvent = eClose;
            //AnimateTimer.Enabled = true;
        }

        private void btnAddFrontOrderNew_Click(object sender, EventArgs e)
        {
            FrontsOrders.AddOrder();
        }

        private void btnDeleteFrontOrderNew_Click(object sender, EventArgs e)
        {
            FrontsOrders.RemoveOrder();
        }

        private void btnAddDecorOrderNew_Click(object sender, EventArgs e)
        {
            DecorOrders.AddOrder();
        }

        private void btnDeleteDecorOrderNew_Click(object sender, EventArgs e)
        {
            DecorOrders.RemoveOrder();
        }


        private void NewFrontsOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            bool NeedPaintRow = DoubleFrontsOrders.ColorRow(
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["FrontID"].Value),
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["ColorID"].Value),
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["PatinaID"].Value),
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["InsetTypeID"].Value),
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["InsetColorID"].Value),
                    Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["TechnoInsetTypeID"].Value),
                    Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["TechnoInsetColorID"].Value),
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["Height"].Value),
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["Width"].Value),
                Convert.ToInt32(NewFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["Count"].Value));

            if (NeedPaintRow)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = NewFrontsOrdersDataGrid.RowHeadersVisible ?
                                     NewFrontsOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    NewFrontsOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            NewFrontsOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Color.Red;
                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.White;
                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                   Color.FromArgb(31, 158, 0);
                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
            else
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = NewFrontsOrdersDataGrid.RowHeadersVisible ?
                                     NewFrontsOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    NewFrontsOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            NewFrontsOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Security.GridsBackColor;
                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.Black;
                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(31, 158, 0);
                NewFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
        }

        private void OldFrontsOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            int F = Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["FrontID"].Value);
            bool NeedPaintRow = FrontsOrders.ColorRow(
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["FrontID"].Value),
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["ColorID"].Value),
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["PatinaID"].Value),
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["InsetTypeID"].Value),
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["InsetColorID"].Value),
                    Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["TechnoInsetTypeID"].Value),
                    Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["TechnoInsetColorID"].Value),
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["Height"].Value),
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["Width"].Value),
                Convert.ToInt32(OldFrontsOrdersDataGrid.Rows[e.RowIndex].Cells["Count"].Value));

            if (NeedPaintRow)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = OldFrontsOrdersDataGrid.RowHeadersVisible ?
                                     OldFrontsOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    OldFrontsOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            OldFrontsOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Color.Red;
                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.White;
                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                   Color.FromArgb(31, 158, 0);
                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
            else
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = OldFrontsOrdersDataGrid.RowHeadersVisible ?
                                     OldFrontsOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    OldFrontsOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            OldFrontsOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Security.GridsBackColor;
                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.Black;
                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(31, 158, 0);
                OldFrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
        }

        private void OldDecorOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            bool NeedPaintRow = DecorOrders.ColorRow(
                Convert.ToInt32(OldDecorOrdersDataGrid.Rows[e.RowIndex].Cells["ProductID"].Value),
                Convert.ToInt32(OldDecorOrdersDataGrid.Rows[e.RowIndex].Cells["DecorID"].Value),
                Convert.ToInt32(OldDecorOrdersDataGrid.Rows[e.RowIndex].Cells["ColorID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PatinaID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["InsetTypeID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["InsetColorID"].Value),
                Convert.ToInt32(OldDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Length"].Value),
                Convert.ToInt32(OldDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Height"].Value),
                Convert.ToInt32(OldDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Width"].Value),
                Convert.ToInt32(OldDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Count"].Value));

            if (NeedPaintRow)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = OldDecorOrdersDataGrid.RowHeadersVisible ?
                                     OldDecorOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    OldDecorOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            OldDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Color.Red;
                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.White;
                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                   Color.FromArgb(31, 158, 0);
                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
            else
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = OldDecorOrdersDataGrid.RowHeadersVisible ?
                                     OldDecorOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    OldDecorOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            OldDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Security.GridsBackColor;
                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.Black;
                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(31, 158, 0);
                OldDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
        }

        private void NewDecorOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            bool NeedPaintRow = DoubleDecorOrders.ColorRow(
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["ProductID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["DecorID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["ColorID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PatinaID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["InsetTypeID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["InsetColorID"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Length"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Height"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Width"].Value),
                Convert.ToInt32(NewDecorOrdersDataGrid.Rows[e.RowIndex].Cells["Count"].Value));

            if (NeedPaintRow)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = NewDecorOrdersDataGrid.RowHeadersVisible ?
                                     NewDecorOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    NewDecorOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            NewDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Color.Red;
                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.White;
                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                   Color.FromArgb(31, 158, 0);
                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
            else
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = NewDecorOrdersDataGrid.RowHeadersVisible ?
                                     NewDecorOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    NewDecorOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            NewDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    Security.GridsBackColor;
                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    Color.Black;
                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(31, 158, 0);
                NewDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                    Color.White;
            }
        }

    }
}
