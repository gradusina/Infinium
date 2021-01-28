using Infinium.Modules.Marketing.Expedition;

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingNewDispatchForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;
        bool NewDispatch = false;
        bool CanEditDispatch = false;
        bool EditMegaOrder = false;

        int FormEvent = 0;
        int ClientID = 0;
        int DispatchID = 0;
        int MainOrderID = 0;
        int MegaOrderID = 0;
        object DispatchDate = null;

        Form TopForm;
        NewMarketingDispatch NewDispatchManager;

        public MarketingNewDispatchForm(bool bCanEditDispatch, bool bNewDispatch, object oDispatchDate, int iClientID, int iDispatchID)
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            CanEditDispatch = bCanEditDispatch;
            NewDispatch = bNewDispatch;
            if (NewDispatch)
                CanEditDispatch = true;
            if (!CanEditDispatch)
                kryptonPanel1.Visible = false;
            DispatchDate = oDispatchDate;
            ClientID = iClientID;
            DispatchID = iDispatchID;
            Initialize();
            while (!SplashForm.bCreated) ;
        }

        public MarketingNewDispatchForm(bool bCanEditDispatch, object oDispatchDate, int iClientID, int iDispatchID, int iMegaOrderID, int iMainOrderID)
        {
            InitializeComponent();
            MenuButton.Visible = false;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            CanEditDispatch = bCanEditDispatch;
            if (!CanEditDispatch)
                kryptonPanel1.Visible = false;
            DispatchDate = oDispatchDate;
            ClientID = iClientID;
            DispatchID = iDispatchID;
            MainOrderID = iMainOrderID;
            MegaOrderID = iMegaOrderID;
            EditMegaOrder = true;
            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void MarketingNewDispatchForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            NeedSplash = true;
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            NewDispatchManager = new NewMarketingDispatch()
            {
                CurrentClient = ClientID,
                CurrentDispatch = DispatchID
            };
            if (EditMegaOrder)
                NewDispatchManager.Initialize(MegaOrderID);
            else
                NewDispatchManager.Initialize(DispatchID, CanEditDispatch);
            DataBinding();
            dgvMegaOrdersSetting();
            dgvMainOrdersSetting();
            dgvPackagesSetting();
            dgvFrontsOrdersSetting();
            dgvDecorOrdersSetting();
        }

        private void DataBinding()
        {
            dgvFrontsOrders.DataSource = NewDispatchManager.FrontsOrdersList;
            dgvDecorOrders.DataSource = NewDispatchManager.DecorOrdersList;
            dgvMainOrders.DataSource = NewDispatchManager.MainOrdersList;
            dgvPackages.DataSource = NewDispatchManager.PackagesList;
            dgvMegaOrders.DataSource = NewDispatchManager.MegaOrdersList;
        }

        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn CheckBoxColumn
        {
            get
            {
                DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle()
                {
                    Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
                    NullValue = false
                };
                ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn CheckBoxColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn()
                {
                    DefaultCellStyle = dataGridViewCellStyle1,
                    FalseValue = false,
                    HeaderText = "",
                    IndeterminateValue = null,
                    Name = "CheckBoxColumn",
                    ReadOnly = true,
                    TrueValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 60
                };
                CheckBoxColumn.ReadOnly = false;
                CheckBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                CheckBoxColumn.DisplayIndex = 0;
                return CheckBoxColumn;
            }
        }

        private void dgvMegaOrdersSetting()
        {
            dgvMegaOrders.Columns["CheckBoxColumn"].SortMode = DataGridViewColumnSortMode.Programmatic;

            dgvMegaOrders.Columns["ProfilPackCount"].Visible = false;
            dgvMegaOrders.Columns["TPSPackCount"].Visible = false;

            foreach (DataGridViewColumn Column in dgvMegaOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            dgvMegaOrders.Columns["CheckBoxColumn"].HeaderText = string.Empty;
            dgvMegaOrders.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            dgvMegaOrders.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            dgvMegaOrders.Columns["Weight"].HeaderText = "Вес, кг.";
            dgvMegaOrders.Columns["ProfilPackPercentage"].HeaderText = "Упаковано\r\nПрофиль, %";
            dgvMegaOrders.Columns["ProfilPackedCount"].HeaderText = "Упаковано\r\n Профиль, кол-во";
            dgvMegaOrders.Columns["ProfilStoreCount"].HeaderText = "Склад,\r\nПрофиль, кол-во";
            dgvMegaOrders.Columns["ProfilStorePercentage"].HeaderText = "Склад,\r\nПрофиль, %";
            dgvMegaOrders.Columns["ProfilExpCount"].HeaderText = "Экспедиция,\r\nПрофиль, кол-во";
            dgvMegaOrders.Columns["ProfilExpPercentage"].HeaderText = "Экспедиция,\r\nПрофиль, %";
            dgvMegaOrders.Columns["ProfilDispPercentage"].HeaderText = "Отгружено\r\nПрофиль, %";
            dgvMegaOrders.Columns["ProfilDispatchedCount"].HeaderText = "Отгружено\r\n Профиль, кол-во";
            dgvMegaOrders.Columns["TPSPackPercentage"].HeaderText = "Упаковано\r\nТПС, %";
            dgvMegaOrders.Columns["TPSPackedCount"].HeaderText = "Упаковано\r\nТПС, кол-во";
            dgvMegaOrders.Columns["TPSStoreCount"].HeaderText = "Склад,\r\nТПС, кол-во";
            dgvMegaOrders.Columns["TPSStorePercentage"].HeaderText = "Склад,\r\nТПС, %";
            dgvMegaOrders.Columns["TPSExpCount"].HeaderText = "Экспедиция,\r\nТПС, кол-во";
            dgvMegaOrders.Columns["TPSExpPercentage"].HeaderText = "Экспедиция,\r\nТПС, %";
            dgvMegaOrders.Columns["TPSDispPercentage"].HeaderText = "Отгружено\r\nТПС, %";
            dgvMegaOrders.Columns["TPSDispatchedCount"].HeaderText = "Отгружено\r\nТПС, кол-во";

            dgvMegaOrders.Columns["CheckBoxColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["CheckBoxColumn"].Width = 45;
            dgvMegaOrders.Columns["ProfilPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilPackPercentage"].Width = 155;
            dgvMegaOrders.Columns["ProfilPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilPackedCount"].Width = 155;
            dgvMegaOrders.Columns["TPSPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSPackPercentage"].Width = 125;
            dgvMegaOrders.Columns["TPSPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSPackedCount"].Width = 125;
            dgvMegaOrders.Columns["ProfilStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilStorePercentage"].Width = 155;
            dgvMegaOrders.Columns["ProfilStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilStoreCount"].Width = 155;
            dgvMegaOrders.Columns["TPSStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSStorePercentage"].Width = 125;
            dgvMegaOrders.Columns["TPSStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSStoreCount"].Width = 125;
            dgvMegaOrders.Columns["ProfilExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilExpPercentage"].Width = 155;
            dgvMegaOrders.Columns["ProfilExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilExpCount"].Width = 155;
            dgvMegaOrders.Columns["TPSExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSExpPercentage"].Width = 125;
            dgvMegaOrders.Columns["TPSExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSExpCount"].Width = 125;
            dgvMegaOrders.Columns["ProfilDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilDispPercentage"].Width = 155;
            dgvMegaOrders.Columns["ProfilDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["ProfilDispatchedCount"].Width = 155;
            dgvMegaOrders.Columns["TPSDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSDispPercentage"].Width = 125;
            dgvMegaOrders.Columns["TPSDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMegaOrders.Columns["TPSDispatchedCount"].Width = 125;

            if (CanEditDispatch)
                dgvMegaOrders.Columns["CheckBoxColumn"].ReadOnly = false;

            dgvMegaOrders.AutoGenerateColumns = false;

            dgvMegaOrders.Columns["ProfilPackPercentage"].DisplayIndex = 3;
            dgvMegaOrders.Columns["ProfilPackedCount"].DisplayIndex = 4;
            dgvMegaOrders.Columns["ProfilStorePercentage"].DisplayIndex = 5;
            dgvMegaOrders.Columns["ProfilStoreCount"].DisplayIndex = 6;
            dgvMegaOrders.Columns["ProfilExpPercentage"].DisplayIndex = 7;
            dgvMegaOrders.Columns["ProfilExpCount"].DisplayIndex = 8;
            dgvMegaOrders.Columns["ProfilDispPercentage"].DisplayIndex = 9;
            dgvMegaOrders.Columns["ProfilDispatchedCount"].DisplayIndex = 10;
            dgvMegaOrders.Columns["TPSPackPercentage"].DisplayIndex = 11;
            dgvMegaOrders.Columns["TPSPackedCount"].DisplayIndex = 12;
            dgvMegaOrders.Columns["TPSStorePercentage"].DisplayIndex = 13;
            dgvMegaOrders.Columns["TPSStoreCount"].DisplayIndex = 14;
            dgvMegaOrders.Columns["TPSExpPercentage"].DisplayIndex = 15;
            dgvMegaOrders.Columns["TPSExpCount"].DisplayIndex = 16;
            dgvMegaOrders.Columns["TPSDispPercentage"].DisplayIndex = 17;
            dgvMegaOrders.Columns["TPSDispatchedCount"].DisplayIndex = 18;
            dgvMegaOrders.Columns["CheckBoxColumn"].DisplayIndex = 0;

            dgvMegaOrders.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvMegaOrders.Columns["ProfilPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["ProfilPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("ProfilPackPercentage");
            dgvMegaOrders.Columns["ProfilStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["ProfilStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("ProfilStorePercentage");
            dgvMegaOrders.Columns["ProfilExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["ProfilExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("ProfilExpPercentage");
            dgvMegaOrders.Columns["ProfilDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["ProfilDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("ProfilDispPercentage");
            dgvMegaOrders.Columns["TPSPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["TPSPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("TPSPackPercentage");
            dgvMegaOrders.Columns["TPSStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["TPSStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("TPSStorePercentage");
            dgvMegaOrders.Columns["TPSExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["TPSExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("TPSExpPercentage");
            dgvMegaOrders.Columns["TPSDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.Columns["TPSDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMegaOrders.AddPercentageColumn("TPSDispPercentage");
        }

        private void dgvMainOrdersSetting()
        {
            dgvMainOrders.Columns["CheckBoxColumn"].SortMode = DataGridViewColumnSortMode.Programmatic;

            dgvMainOrders.Columns["MegaOrderID"].Visible = false;
            dgvMainOrders.Columns["ProfilPackCount"].Visible = false;
            dgvMainOrders.Columns["TPSPackCount"].Visible = false;

            foreach (DataGridViewColumn Column in dgvMainOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            dgvMainOrders.Columns["CheckBoxColumn"].HeaderText = string.Empty;
            dgvMainOrders.Columns["MainOrderID"].HeaderText = "№ п\\п";
            dgvMainOrders.Columns["Weight"].HeaderText = "Вес, кг.";
            dgvMainOrders.Columns["FactoryName"].HeaderText = "Участок";
            dgvMainOrders.Columns["Notes"].HeaderText = "Примечание";
            dgvMainOrders.Columns["ProfilPackPercentage"].HeaderText = "Упаковано\r\nПрофиль, %";
            dgvMainOrders.Columns["ProfilPackedCount"].HeaderText = "Упаковано\r\n Профиль, кол-во";
            dgvMainOrders.Columns["ProfilStoreCount"].HeaderText = "Склад,\r\nПрофиль, кол-во";
            dgvMainOrders.Columns["ProfilStorePercentage"].HeaderText = "Склад,\r\nПрофиль, %";
            dgvMainOrders.Columns["ProfilExpCount"].HeaderText = "Экспедиция,\r\nПрофиль, кол-во";
            dgvMainOrders.Columns["ProfilExpPercentage"].HeaderText = "Экспедиция,\r\nПрофиль, %";
            dgvMainOrders.Columns["ProfilDispPercentage"].HeaderText = " Отгружено\r\nПрофиль, %";
            dgvMainOrders.Columns["ProfilDispatchedCount"].HeaderText = "Отгружено\r\n Профиль, кол-во";
            dgvMainOrders.Columns["TPSPackPercentage"].HeaderText = "Упаковано\r\nТПС, %";
            dgvMainOrders.Columns["TPSPackedCount"].HeaderText = " Упаковано\r\nТПС, кол-во";
            dgvMainOrders.Columns["TPSStoreCount"].HeaderText = "Склад,\r\nТПС, кол-во";
            dgvMainOrders.Columns["TPSStorePercentage"].HeaderText = "Склад,\r\nТПС, %";
            dgvMainOrders.Columns["TPSExpCount"].HeaderText = "Экспедиция,\r\nТПС, кол-во";
            dgvMainOrders.Columns["TPSExpPercentage"].HeaderText = "Экспедиция,\r\nТПС, %";
            dgvMainOrders.Columns["TPSDispPercentage"].HeaderText = "Отгружено\r\nТПС, %";
            dgvMainOrders.Columns["TPSDispatchedCount"].HeaderText = " Отгружено\r\nТПС, кол-во";

            dgvMainOrders.Columns["CheckBoxColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["CheckBoxColumn"].Width = 45;
            dgvMainOrders.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMainOrders.Columns["FactoryName"].MinimumWidth = 155;
            dgvMainOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMainOrders.Columns["Notes"].MinimumWidth = 155;

            dgvMainOrders.Columns["ProfilPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilPackPercentage"].Width = 155;
            dgvMainOrders.Columns["ProfilPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilPackedCount"].Width = 155;
            dgvMainOrders.Columns["TPSPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSPackPercentage"].Width = 125;
            dgvMainOrders.Columns["TPSPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSPackedCount"].Width = 125;
            dgvMainOrders.Columns["ProfilStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilStorePercentage"].Width = 155;
            dgvMainOrders.Columns["ProfilStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilStoreCount"].Width = 155;
            dgvMainOrders.Columns["TPSStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSStorePercentage"].Width = 125;
            dgvMainOrders.Columns["TPSStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSStoreCount"].Width = 125;
            dgvMainOrders.Columns["ProfilExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilExpPercentage"].Width = 155;
            dgvMainOrders.Columns["ProfilExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilExpCount"].Width = 155;
            dgvMainOrders.Columns["TPSExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSExpPercentage"].Width = 125;
            dgvMainOrders.Columns["TPSExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSExpCount"].Width = 125;
            dgvMainOrders.Columns["ProfilDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilDispPercentage"].Width = 155;
            dgvMainOrders.Columns["ProfilDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["ProfilDispatchedCount"].Width = 155;
            dgvMainOrders.Columns["TPSDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSDispPercentage"].Width = 125;
            dgvMainOrders.Columns["TPSDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["TPSDispatchedCount"].Width = 125;

            if (CanEditDispatch)
                dgvMainOrders.Columns["CheckBoxColumn"].ReadOnly = false;

            dgvMainOrders.AutoGenerateColumns = false;

            dgvMainOrders.Columns["ProfilPackPercentage"].DisplayIndex = 3;
            dgvMainOrders.Columns["ProfilPackedCount"].DisplayIndex = 4;
            dgvMainOrders.Columns["ProfilStorePercentage"].DisplayIndex = 5;
            dgvMainOrders.Columns["ProfilStoreCount"].DisplayIndex = 6;
            dgvMainOrders.Columns["ProfilExpPercentage"].DisplayIndex = 7;
            dgvMainOrders.Columns["ProfilExpCount"].DisplayIndex = 8;
            dgvMainOrders.Columns["ProfilDispPercentage"].DisplayIndex = 9;
            dgvMainOrders.Columns["ProfilDispatchedCount"].DisplayIndex = 10;
            dgvMainOrders.Columns["TPSPackPercentage"].DisplayIndex = 11;
            dgvMainOrders.Columns["TPSPackedCount"].DisplayIndex = 12;
            dgvMainOrders.Columns["TPSStorePercentage"].DisplayIndex = 13;
            dgvMainOrders.Columns["TPSStoreCount"].DisplayIndex = 14;
            dgvMainOrders.Columns["TPSExpPercentage"].DisplayIndex = 15;
            dgvMainOrders.Columns["TPSExpCount"].DisplayIndex = 16;
            dgvMainOrders.Columns["TPSDispPercentage"].DisplayIndex = 17;
            dgvMainOrders.Columns["TPSDispatchedCount"].DisplayIndex = 18;
            dgvMainOrders.Columns["CheckBoxColumn"].DisplayIndex = 0;
            dgvMainOrders.Columns["Notes"].DisplayIndex = 2;

            dgvMainOrders.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvMainOrders.Columns["ProfilPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["ProfilPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("ProfilPackPercentage");
            dgvMainOrders.Columns["ProfilStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["ProfilStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("ProfilStorePercentage");
            dgvMainOrders.Columns["ProfilExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["ProfilExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("ProfilExpPercentage");
            dgvMainOrders.Columns["ProfilDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["ProfilDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("ProfilDispPercentage");
            dgvMainOrders.Columns["TPSPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["TPSPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("TPSPackPercentage");
            dgvMainOrders.Columns["TPSStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["TPSStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("TPSStorePercentage");
            dgvMainOrders.Columns["TPSExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["TPSExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("TPSExpPercentage");
            dgvMainOrders.Columns["TPSDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.Columns["TPSDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMainOrders.AddPercentageColumn("TPSDispPercentage");
        }

        private void dgvPackagesSetting()
        {
            dgvPackages.Columns["CheckBoxColumn"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dgvPackages.Columns["ProductType"].Visible = false;
            dgvPackages.Columns["MainOrderID"].Visible = false;
            dgvPackages.Columns["MegaOrderID"].Visible = false;
            //dgvPackages.Columns["DispatchID"].Visible = false;

            dgvPackages.Columns["PackingDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPackages.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPackages.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvPackages.Columns["DispatchDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            foreach (DataGridViewColumn Column in dgvPackages.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            dgvPackages.Columns["CheckBoxColumn"].HeaderText = string.Empty;
            dgvPackages.Columns["DispatchID"].HeaderText = "  №\r\nотгр.";
            dgvPackages.Columns["PackNumber"].HeaderText = "  №\r\nупак.";
            dgvPackages.Columns["PackageStatus"].HeaderText = "Статус";
            dgvPackages.Columns["PackingDateTime"].HeaderText = "   Дата\r\nупаковки";
            dgvPackages.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            dgvPackages.Columns["ExpeditionDateTime"].HeaderText = "      Дата\r\nэкспедиции";
            dgvPackages.Columns["DispatchDateTime"].HeaderText = "    Дата\r\nотгрузки";
            dgvPackages.Columns["FactoryName"].HeaderText = "Участок";
            dgvPackages.Columns["CheckBoxColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["CheckBoxColumn"].Width = 45;
            dgvPackages.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackNumber"].Width = 70;
            dgvPackages.Columns["PackageStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackageStatus"].Width = 140;
            dgvPackages.Columns["PackingDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackingDateTime"].Width = 150;
            dgvPackages.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["StorageDateTime"].Width = 150;
            dgvPackages.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["ExpeditionDateTime"].Width = 150;
            dgvPackages.Columns["DispatchDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["DispatchDateTime"].Width = 150;
            dgvPackages.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["FactoryName"].Width = 100;
            dgvPackages.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["PackageID"].Width = 100;
            dgvPackages.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPackages.Columns["TrayID"].Width = 100;

            if (CanEditDispatch)
                dgvPackages.Columns["CheckBoxColumn"].ReadOnly = false;

            dgvPackages.AutoGenerateColumns = false;

            dgvPackages.Columns["CheckBoxColumn"].DisplayIndex = 0;
        }

        private void dgvFrontsOrdersSetting()
        {
            if (!dgvFrontsOrders.Columns.Contains("FrontsColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.FrontsColumn);
            if (!dgvFrontsOrders.Columns.Contains("FrameColorsColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.FrameColorsColumn);
            if (!dgvFrontsOrders.Columns.Contains("PatinaColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.PatinaColumn);
            if (!dgvFrontsOrders.Columns.Contains("InsetTypesColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.InsetTypesColumn);
            if (!dgvFrontsOrders.Columns.Contains("InsetColorsColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.InsetColorsColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoProfilesColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.TechnoProfilesColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoFrameColorsColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.TechnoFrameColorsColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoInsetTypesColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.TechnoInsetTypesColumn);
            if (!dgvFrontsOrders.Columns.Contains("TechnoInsetColorsColumn"))
                dgvFrontsOrders.Columns.Add(NewDispatchManager.TechnoInsetColorsColumn);

            if (dgvFrontsOrders.Columns.Contains("ImpostMargin"))
                dgvFrontsOrders.Columns["ImpostMargin"].Visible = false;
            if (dgvFrontsOrders.Columns.Contains("CreateDateTime"))
            {
                dgvFrontsOrders.Columns["CreateDateTime"].HeaderText = "Добавлено";
                dgvFrontsOrders.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dgvFrontsOrders.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvFrontsOrders.Columns["CreateDateTime"].Width = 100;
            }
            if (dgvFrontsOrders.Columns.Contains("CreateUserID"))
                dgvFrontsOrders.Columns["CreateUserID"].Visible = false;
            dgvFrontsOrders.Columns["FrontsOrdersID"].Visible = false;
            dgvFrontsOrders.Columns["MainOrderID"].Visible = false;
            dgvFrontsOrders.Columns["FrontID"].Visible = false;
            dgvFrontsOrders.Columns["ColorID"].Visible = false;
            dgvFrontsOrders.Columns["InsetColorID"].Visible = false;
            dgvFrontsOrders.Columns["PatinaID"].Visible = false;
            dgvFrontsOrders.Columns["InsetTypeID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoProfileID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoColorID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoInsetTypeID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoInsetColorID"].Visible = false;

            foreach (DataGridViewColumn Column in dgvFrontsOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            dgvFrontsOrders.Columns["Height"].HeaderText = "Высота";
            dgvFrontsOrders.Columns["Width"].HeaderText = "Ширина";
            dgvFrontsOrders.Columns["Count"].HeaderText = "Кол-во";
            dgvFrontsOrders.Columns["Notes"].HeaderText = "Примечание";
            dgvFrontsOrders.Columns["Square"].HeaderText = "Площадь";

            dgvFrontsOrders.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Height"].Width = 85;
            dgvFrontsOrders.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Width"].Width = 85;
            dgvFrontsOrders.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Count"].Width = 65;
            dgvFrontsOrders.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Square"].Width = 100;
            dgvFrontsOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["Notes"].MinimumWidth = 105;

            dgvFrontsOrders.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvFrontsOrders.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Count"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Square"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.CellFormatting += FrontsOrdersDataGrid_CellFormatting;
        }

        private void dgvDecorOrdersSetting()
        {
            if (!dgvDecorOrders.Columns.Contains("ProductColumn"))
                dgvDecorOrders.Columns.Add(NewDispatchManager.ProductColumn);
            if (!dgvDecorOrders.Columns.Contains("ItemColumn"))
                dgvDecorOrders.Columns.Add(NewDispatchManager.ItemColumn);
            if (!dgvDecorOrders.Columns.Contains("ColorColumn"))
                dgvDecorOrders.Columns.Add(NewDispatchManager.ColorColumn);
            if (!dgvDecorOrders.Columns.Contains("PatinaColumn"))
                dgvDecorOrders.Columns.Add(NewDispatchManager.DecorPatinaColumn);
            if (!dgvDecorOrders.Columns.Contains("InsetTypesColumn"))
                dgvDecorOrders.Columns.Add(NewDispatchManager.InsetTypesColumn);
            if (!dgvDecorOrders.Columns.Contains("InsetColorsColumn"))
                dgvDecorOrders.Columns.Add(NewDispatchManager.InsetColorsColumn);
            if (dgvDecorOrders.Columns.Contains("CreateDateTime"))
            {
                dgvDecorOrders.Columns["CreateDateTime"].HeaderText = "Добавлено";
                dgvDecorOrders.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dgvDecorOrders.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvDecorOrders.Columns["CreateDateTime"].Width = 100;
            }
            if (dgvDecorOrders.Columns.Contains("CreateUserID"))
                dgvDecorOrders.Columns["CreateUserID"].Visible = false;
            dgvDecorOrders.Columns["DecorOrderID"].Visible = false;
            dgvDecorOrders.Columns["ProductID"].Visible = false;
            dgvDecorOrders.Columns["DecorID"].Visible = false;
            dgvDecorOrders.Columns["ColorID"].Visible = false;
            dgvDecorOrders.Columns["PatinaID"].Visible = false;
            dgvDecorOrders.Columns["InsetTypeID"].Visible = false;
            dgvDecorOrders.Columns["InsetColorID"].Visible = false;

            foreach (DataGridViewColumn Column in dgvDecorOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            dgvDecorOrders.Columns["Length"].HeaderText = "Длина";
            dgvDecorOrders.Columns["Height"].HeaderText = "Высота";
            dgvDecorOrders.Columns["Width"].HeaderText = "Ширина";
            dgvDecorOrders.Columns["Count"].HeaderText = "Кол-во";
            dgvDecorOrders.Columns["Notes"].HeaderText = "Примечание";

            dgvDecorOrders.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ProductColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ItemColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ColorsColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["PatinaColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["InsetTypesColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["InsetColorsColumn"].MinimumWidth = 110;
            dgvDecorOrders.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Height"].Width = 85;
            dgvDecorOrders.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Width"].Width = 85;
            dgvDecorOrders.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Count"].Width = 85;
            dgvDecorOrders.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorOrders.Columns["Length"].Width = 85;
            dgvDecorOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecorOrders.Columns["Notes"].MinimumWidth = 145;

            dgvDecorOrders.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            dgvDecorOrders.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Count"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        void FrontsOrdersDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns.Contains("PatinaColumn") && (e.ColumnIndex == grid.Columns["PatinaColumn"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int PatinaID = -1;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["PatinaID"].Value != DBNull.Value)
                {
                    PatinaID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PatinaID"].Value);
                    DisplayName = NewDispatchManager.PatinaDisplayName(PatinaID);
                }
                cell.ToolTipText = DisplayName;
            }
        }

        private void dgvMegaOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager == null)
                return;
            if (dgvMegaOrders.SelectedRows.Count == 0)
            {
                return;
            }
            int MegaOrderID = Convert.ToInt32(dgvMegaOrders.SelectedRows[0].Cells["MegaOrderID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                NewDispatchManager.FilterMainOrders(MegaOrderID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                NewDispatchManager.FilterMainOrders(MegaOrderID);
        }

        private void dgvMainOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager == null)
                return;

            int MainOrderID = 0;
            if (dgvMainOrders.SelectedRows.Count > 0 && dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                NewDispatchManager.FilterPackages(MainOrderID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
                NewDispatchManager.FilterPackages(MainOrderID);

            if (dgvPackages.SelectedRows.Count == 0)
            {
                NewDispatchManager.ClearFrontsOrders();
                NewDispatchManager.ClearDecorOrders();
                tabOrders.TabPages[0].PageVisible = NewDispatchManager.FilterFrontsOrders(0, MainOrderID);
                tabOrders.TabPages[1].PageVisible = NewDispatchManager.FilterDecorOrders(0, MainOrderID);
            }
        }

        private void dgvPackages_SelectionChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager == null)
                return;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                FilterOrders();
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                FilterOrders();
            }
        }

        private void FilterOrders()
        {
            if (dgvMainOrders.SelectedRows.Count == 0)
                return;
            int MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);

            if (dgvPackages.SelectedRows.Count == 0)
            {
                return;
            }
            else
            {
                int PackageID = Convert.ToInt32(dgvPackages.SelectedRows[0].Cells["PackageID"].Value);
                int ProductType = Convert.ToInt32(dgvPackages.SelectedRows[0].Cells["ProductType"].Value);

                NewDispatchManager.ClearFrontsOrders();
                NewDispatchManager.ClearDecorOrders();
                tabOrders.TabPages[0].PageVisible = NewDispatchManager.FilterFrontsOrders(PackageID, MainOrderID);
                tabOrders.TabPages[1].PageVisible = NewDispatchManager.FilterDecorOrders(PackageID, MainOrderID);

                if (ProductType == 0)
                {
                    tabOrders.SelectedTabPage = tabOrders.TabPages[0];
                }
                if (ProductType == 1)
                {
                    tabOrders.SelectedTabPage = tabOrders.TabPages[1];
                }
            }
        }

        private void MarketingNewDispatchForm_Load(object sender, EventArgs e)
        {
            NewDispatchManager.FillMegaPercentageColumn();
            NewDispatchManager.FillMainPercentageColumn();
            NewDispatchManager.MoveToMegaOrder(MegaOrderID);
            NewDispatchManager.MoveToMainOrder(MainOrderID);
        }

        private void dgvMegaOrders_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMegaOrders.Columns[e.ColumnIndex].Name == "CheckBoxColumn" && e.RowIndex != -1)
            {
                dgvMegaOrders.EndEdit();
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)dgvMegaOrders.
                    Rows[e.RowIndex].Cells["CheckBoxColumn"];

                int MegaOrderID = Convert.ToInt32(dgvMegaOrders.Rows[e.RowIndex].Cells["MegaOrderID"].Value);

                int FactoryID = Convert.ToInt32(dgvMegaOrders.Rows[e.RowIndex].Cells["FactoryID"].Value);
                int ProfilPackAllocStatusID = Convert.ToInt32(dgvMegaOrders.Rows[e.RowIndex].Cells["ProfilPackAllocStatusID"].Value);
                int TPSPackAllocStatusID = Convert.ToInt32(dgvMegaOrders.Rows[e.RowIndex].Cells["TPSPackAllocStatusID"].Value);
                if (Convert.ToBoolean(checkCell.Value) && FactoryID != 2 && ProfilPackAllocStatusID < 2)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Заказ не полностью распределен на фирме ЗОВ-Профиль",
                           "Предупреждение");
                }
                if (Convert.ToBoolean(checkCell.Value) && FactoryID != 1 && TPSPackAllocStatusID < 2)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Заказ не полностью распределен на фирме ЗОВ-ТПС",
                           "Предупреждение");
                }

                NewDispatchManager.FlagMainOrders(MegaOrderID, Convert.ToBoolean(checkCell.Value));
            }
        }

        private void dgvMainOrders_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMainOrders.Columns[e.ColumnIndex].Name == "CheckBoxColumn" && e.RowIndex != -1)
            {
                dgvMainOrders.EndEdit();
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)dgvMainOrders.
                    Rows[e.RowIndex].Cells["CheckBoxColumn"];
                int MainOrderID = Convert.ToInt32(dgvMainOrders.Rows[e.RowIndex].Cells["MainOrderID"].Value);

                int FactoryID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FactoryID"].Value);
                int ProfilPackAllocStatusID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["ProfilPackAllocStatusID"].Value);
                int TPSPackAllocStatusID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["TPSPackAllocStatusID"].Value);
                if (Convert.ToBoolean(checkCell.Value) && FactoryID != 2 && ProfilPackAllocStatusID < 2)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Подзаказ не полностью распределен на фирме ЗОВ-Профиль",
                           "Предупреждение");
                }
                if (Convert.ToBoolean(checkCell.Value) && FactoryID != 1 && TPSPackAllocStatusID < 2)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Подзаказ не полностью распределен на фирме ЗОВ-ТПС",
                           "Предупреждение");
                }

                NewDispatchManager.FlagPackages(MainOrderID, Convert.ToBoolean(checkCell.Value));
            }
        }

        private void btnSaveDispatch_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (NewDispatch)
            {
                NewDispatch = false;
                NewDispatchManager.AddDispatch(DispatchDate);
                NewDispatchManager.CurrentDispatch = NewDispatchManager.MaxDispatchID();
                if (NewDispatchManager.IsCabFur(NewDispatchManager.CurrentDispatch))
                    NewDispatchManager.AddCabFurDispatch(DispatchDate, NewDispatchManager.CurrentDispatch);
            }
            NewDispatchManager.SetDispatchDate(Convert.ToDateTime(DispatchDate));
            NewDispatchManager.SavePackages();
            if (NewDispatchManager.IsEmptyDispath)
            {
                NewDispatchManager.ClearConfirmExpInfo();
                NewDispatchManager.ClearConfirmDispInfo();
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvPackages_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PercentageDataGrid DataGrid = (PercentageDataGrid)sender;
            DataGridViewColumn newColumn = DataGrid.Columns[e.ColumnIndex];
            if (e.RowIndex == -1 && newColumn.Name == "CheckBoxColumn")
            {
                DataGridViewColumn oldColumn = DataGrid.SortedColumn;
                ListSortDirection direction;

                if (oldColumn != null)
                {
                    if (oldColumn == newColumn &&
                        DataGrid.SortOrder == SortOrder.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                        oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                }
                else
                {
                    direction = ListSortDirection.Ascending;
                }

                DataGrid.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private void FilterMegaOrders()
        {
            bool NotInProduction = NotProductionCheckBox.Checked;
            bool OnProduction = OnProductionCheckBox.Checked;
            bool InProduction = InProductionCheckBox.Checked;
            bool OnStorage = OnStorageCheckBox.Checked;
            bool OnExpedition = cbOnExpedition.Checked;
            bool Dispatch = DispatchCheckBox.Checked;

            NewDispatchManager.UpdateMegaOrders(DispatchID, CanEditDispatch, NotInProduction, OnProduction, InProduction, OnStorage, OnExpedition, Dispatch);
            NewDispatchManager.UpdateMainOrders(DispatchID, CanEditDispatch, NotInProduction, OnProduction, InProduction, OnStorage, OnExpedition, Dispatch);
            NewDispatchManager.UpdatePackages(DispatchID, CanEditDispatch, NotInProduction, OnProduction, InProduction, OnStorage, OnExpedition, Dispatch);

            NewDispatchManager.SetDefaultValueToCheckBoxColumn();
            NewDispatchManager.SetPackageDispatchStatus();
            NewDispatchManager.SetMainOrderDispatchStatus();
            NewDispatchManager.SetMegaOrderDispatchStatus();
        }

        private void NotProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                FilterMegaOrders();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void OnProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                FilterMegaOrders();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void InProductionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                FilterMegaOrders();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void OnStorageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                FilterMegaOrders();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void cbOnExpedition_CheckedChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                FilterMegaOrders();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void DispatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NewDispatchManager != null)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                FilterMegaOrders();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void dgvPackages_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPackages.Columns[e.ColumnIndex].Name == "CheckBoxColumn" && e.RowIndex != -1)
            {
                DataGridViewCheckBoxCell checkCell =
                       (DataGridViewCheckBoxCell)dgvPackages.
                       Rows[e.RowIndex].Cells["CheckBoxColumn"];

                int FactoryID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FactoryID"].Value);
                int ProfilPackAllocStatusID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["ProfilPackAllocStatusID"].Value);
                int TPSPackAllocStatusID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["TPSPackAllocStatusID"].Value);
                if (Convert.ToBoolean(checkCell.Value) && FactoryID != 2 && ProfilPackAllocStatusID < 2)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Заказ не полностью распределен на фирме ЗОВ-Профиль",
                           "Предупреждение");
                }
                if (Convert.ToBoolean(checkCell.Value) && FactoryID != 1 && TPSPackAllocStatusID < 2)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                           "Заказ не полностью распределен на фирме ЗОВ-ТПС",
                           "Предупреждение");
                }
            }
        }
    }
}