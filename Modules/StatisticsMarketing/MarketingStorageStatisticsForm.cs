using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.StatisticsMarketing;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingStorageStatisticsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;
        bool bExpSummaryClient = false;
        int FormEvent = 0;

        NumberFormatInfo nfi2;

        LightStartForm LightStartForm;

        Form TopForm = null;
        MarketingStorageStatistics StorageStatistics;
        BatchExcelReport MarketingBatchReport;

        public MarketingStorageStatisticsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void MarketingStorageStatistics_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
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
            nfi2 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ",",
                NumberDecimalDigits = 1
            };
            MarketingBatchReport = new BatchExcelReport();

            StorageStatistics = new MarketingStorageStatistics(
                ref MFSummaryDG, ref MDSummaryDG,
                ref ExpFrontsDataGrid, ref ExpDecorProductsDataGrid, ref ExpDecorItemsDataGrid);

            dgvClientsGroups.DataSource = StorageStatistics.ClientGroupsBS;
            dgvClientsGroups.Columns["ClientGroupID"].Visible = false;
            dgvClientsGroups.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientsGroups.Columns["Check"].Width = 50;
            dgvClientsGroups.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvClientsGroups.Columns["ClientGroupName"].MinimumWidth = 110;
            dgvClientsGroups.Columns["Check"].DisplayIndex = 0;
            dgvClientsGroups.Columns["ClientGroupName"].DisplayIndex = 1;
            dgvClientsGroups.Columns["ClientGroupName"].ReadOnly = true;
        }

        private void ExpFrontsInfo()
        {
            ExpFrontsSquareLabel.Text = string.Empty;
            ExpFrontsCostLabel.Text = string.Empty;
            ExpFrontsCountLabel.Text = string.Empty;
            ExpCurvedCountLabel.Text = string.Empty;

            decimal ExpFrontCost = 0;
            decimal ExpFrontSquare = 0;
            int ExpFrontsCount = 0;
            int ExpCurvedCount = 0;

            StorageStatistics.GetFrontsInfo(ref ExpFrontSquare, ref ExpFrontCost, ref ExpFrontsCount, ref ExpCurvedCount);

            ExpFrontsSquareLabel.Text = ExpFrontSquare.ToString("N", nfi2);
            ExpFrontsCostLabel.Text = ExpFrontCost.ToString("N", nfi2);
            ExpFrontsCountLabel.Text = ExpFrontsCount.ToString();
            ExpCurvedCountLabel.Text = ExpCurvedCount.ToString();
        }

        private void ExpDecorInfo()
        {
            ExpDecorPogonLabel.Text = string.Empty;
            ExpDecorCostLabel.Text = string.Empty;
            ExpDecorCountLabel.Text = string.Empty;

            decimal ExpDecorPogon = 0;
            decimal ExpDecorCost = 0;
            int ExpDecorCount = 0;

            StorageStatistics.GetDecorInfo(ref ExpDecorPogon, ref ExpDecorCost, ref ExpDecorCount);

            ExpDecorPogonLabel.Text = ExpDecorPogon.ToString("N", nfi2);
            ExpDecorCostLabel.Text = ExpDecorCost.ToString("N", nfi2);
            ExpDecorCountLabel.Text = ExpDecorCount.ToString();

            ExpDecorProductsDataGrid_SelectionChanged(null, null);
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

        private void MarketingExpeditionStatisticsForm_Load(object sender, EventArgs e)
        {

        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;

            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            ExpMenuPanel.Visible = !ExpMenuPanel.Visible;
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataTable)((BindingSource)ExpFrontsDataGrid.DataSource).DataSource).DefaultView.Table,
                ((DataTable)((BindingSource)ExpDecorProductsDataGrid.DataSource).DataSource).DefaultView.Table, "Общая статистика.Склад");

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ExpDecorProductsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StorageStatistics != null)
                if (StorageStatistics.DecorProductsSummaryBS.Count > 0)
                {
                    if (((DataRowView)StorageStatistics.DecorProductsSummaryBS.Current)["ProductID"] != DBNull.Value)
                    {
                        StorageStatistics.FilterDecorProducts(
                            Convert.ToInt32(((DataRowView)StorageStatistics.DecorProductsSummaryBS.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)StorageStatistics.DecorProductsSummaryBS.Current).Row["MeasureID"]));
                    }
                }
        }

        private void MDSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ExpOrdersSummaryCheckBox.Checked)
                return;

            if (StorageStatistics != null)
                if (StorageStatistics.MDSummaryBS.Count > 0)
                {
                    if (((DataRowView)StorageStatistics.MDSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = ExpProfilCheckBox.Checked;
                        bool TPS = ExpTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            if (ExpClientSummaryCheckBox.Checked)
                                StorageStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasDecor)
                                StorageStatistics.ClearDecorOrders();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (ExpClientSummaryCheckBox.Checked)
                                StorageStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasDecor)
                                StorageStatistics.ClearDecorOrders();
                        }
                        ExpDecorInfo();
                    }
                }
        }

        private void MFSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ExpOrdersSummaryCheckBox.Checked)
                return;

            if (StorageStatistics != null)
                if (StorageStatistics.MFSummaryBS.Count > 0)
                {
                    if (((DataRowView)StorageStatistics.MFSummaryBS.Current)["ClientID"] != DBNull.Value)
                    {
                        bool Profil = ExpProfilCheckBox.Checked;
                        bool TPS = ExpTPSCheckBox.Checked;

                        int FactoryID = 0;

                        if (Profil && !TPS)
                            FactoryID = 1;
                        if (!Profil && TPS)
                            FactoryID = 2;
                        if (!Profil && !TPS)
                            FactoryID = -1;

                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();
                            while (!SplashWindow.bSmallCreated) ;

                            if (ExpClientSummaryCheckBox.Checked)
                                StorageStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasFronts)
                                StorageStatistics.ClearFrontsOrders();

                            ExpFrontsInfo();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (ExpClientSummaryCheckBox.Checked)
                                StorageStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                StorageStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)StorageStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!StorageStatistics.HasFronts)
                                StorageStatistics.ClearFrontsOrders();

                            ExpFrontsInfo();
                        }
                    }
                }
        }

        private void ExpFrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void ExpDecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void FilterExpedition()
        {
            bool Profil = ExpProfilCheckBox.Checked;
            bool TPS = ExpTPSCheckBox.Checked;

            int FactoryID = 0;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;

            MFSummaryDG.SelectionChanged -= MFSummaryDG_SelectionChanged;
            MDSummaryDG.SelectionChanged -= MDSummaryDG_SelectionChanged;
            ExpDecorProductsDataGrid.SelectionChanged -= ExpDecorProductsDataGrid_SelectionChanged;
            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                StorageStatistics.ShowColumns(ref MFSummaryDG, ref MDSummaryDG, Profil, TPS, bExpSummaryClient);

                StorageStatistics.FMarketingOrders(FactoryID, -1);
                StorageStatistics.DMarketingOrders(FactoryID, -1);

                if (!ExpClientSummaryCheckBox.Checked)
                    StorageStatistics.MarketingSummary(FactoryID);
                else
                    StorageStatistics.ClientSummary(FactoryID);

                if (!StorageStatistics.HasFronts)
                    StorageStatistics.ClearFrontsOrders();
                if (!StorageStatistics.HasDecor)
                    StorageStatistics.ClearDecorOrders();
                ExpFrontsInfo();
                ExpDecorInfo();


                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            MFSummaryDG.SelectionChanged += MFSummaryDG_SelectionChanged;
            MDSummaryDG.SelectionChanged += MDSummaryDG_SelectionChanged;
            ExpDecorProductsDataGrid.SelectionChanged += ExpDecorProductsDataGrid_SelectionChanged;
        }

        private void ExpFilterButton_Click(object sender, EventArgs e)
        {
            if (StorageStatistics == null)
                return;

            bExpSummaryClient = ExpClientSummaryCheckBox.Checked;

            FilterExpedition();
        }

        private void ExpClientSummaryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ExpOrdersSummaryCheckBox.Enabled = !ExpClientSummaryCheckBox.Checked;
            ExpOrdersSummaryCheckBox.Checked = !ExpClientSummaryCheckBox.Checked;
        }

    }
}
