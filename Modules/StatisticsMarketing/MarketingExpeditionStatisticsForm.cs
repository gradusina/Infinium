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
    public partial class MarketingExpeditionStatisticsForm : Form
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
        MarketingExpeditionStatistics ExpeditionStatistics;
        BatchExcelReport MarketingBatchReport;

        public MarketingExpeditionStatisticsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void MarketingExpeditionStatisticsForm_Shown(object sender, EventArgs e)
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

            ExpeditionStatistics = new MarketingExpeditionStatistics(
                ref MFSummaryDG, ref MDSummaryDG,
                ref ExpFrontsDataGrid, ref ExpDecorProductsDataGrid, ref ExpDecorItemsDataGrid);
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

            ExpeditionStatistics.GetFrontsInfo(ref ExpFrontSquare, ref ExpFrontCost, ref ExpFrontsCount, ref ExpCurvedCount);

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

            ExpeditionStatistics.GetDecorInfo(ref ExpDecorPogon, ref ExpDecorCost, ref ExpDecorCount);

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
            if (ExpeditionStatistics != null)
                if (ExpeditionStatistics.DecorProductsSummaryBS.Count > 0)
                {
                    if (((DataRowView)ExpeditionStatistics.DecorProductsSummaryBS.Current)["ProductID"] != DBNull.Value)
                    {
                        ExpeditionStatistics.FilterDecorProducts(
                            Convert.ToInt32(((DataRowView)ExpeditionStatistics.DecorProductsSummaryBS.Current).Row["ProductID"]),
                            Convert.ToInt32(((DataRowView)ExpeditionStatistics.DecorProductsSummaryBS.Current).Row["MeasureID"]));
                    }
                }
        }

        private void MDSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ExpOrdersSummaryCheckBox.Checked)
                return;

            if (ExpeditionStatistics != null)
                if (ExpeditionStatistics.MDSummaryBS.Count > 0)
                {
                    if (((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["ClientID"] != DBNull.Value)
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
                                ExpeditionStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasDecor)
                                ExpeditionStatistics.ClearDecorOrders();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (ExpClientSummaryCheckBox.Checked)
                                ExpeditionStatistics.DMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.DMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MDSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasDecor)
                                ExpeditionStatistics.ClearDecorOrders();
                        }
                        ExpDecorInfo();
                    }
                }
        }

        private void MFSummaryDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ExpOrdersSummaryCheckBox.Checked)
                return;

            if (ExpeditionStatistics != null)
                if (ExpeditionStatistics.MFSummaryBS.Count > 0)
                {
                    if (((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["ClientID"] != DBNull.Value)
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
                                ExpeditionStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasFronts)
                                ExpeditionStatistics.ClearFrontsOrders();

                            ExpFrontsInfo();

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            if (ExpClientSummaryCheckBox.Checked)
                                ExpeditionStatistics.FMarketingOrders(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["ClientID"]));
                            else
                                ExpeditionStatistics.FMarketingOrders1(FactoryID, Convert.ToInt32(((DataRowView)ExpeditionStatistics.MFSummaryBS.Current)["MegaOrderID"]));

                            if (!ExpeditionStatistics.HasFronts)
                                ExpeditionStatistics.ClearFrontsOrders();

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

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                ExpeditionStatistics.ShowColumns(ref MFSummaryDG, ref MDSummaryDG, Profil, TPS, bExpSummaryClient);

                ExpeditionStatistics.FMarketingOrders(FactoryID, -1);
                ExpeditionStatistics.DMarketingOrders(FactoryID, -1);

                if (!ExpClientSummaryCheckBox.Checked)
                    ExpeditionStatistics.MarketingSummary(FactoryID);
                else
                    ExpeditionStatistics.ClientSummary(FactoryID);

                if (!ExpeditionStatistics.HasFronts)
                    ExpeditionStatistics.ClearFrontsOrders();
                if (!ExpeditionStatistics.HasDecor)
                    ExpeditionStatistics.ClearDecorOrders();
                ExpFrontsInfo();
                ExpDecorInfo();


                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void ExpFilterButton_Click(object sender, EventArgs e)
        {
            if (ExpeditionStatistics == null)
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
