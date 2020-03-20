using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.StaticticsZOV;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVStorageStatisticsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;
        int FormEvent = 0;

        NumberFormatInfo nfi2;

        LightStartForm LightStartForm;

        Form TopForm = null;

        ZOVStorageStatistics StorageStatistics;
        BatchExcelReport MarketingBatchReport;

        public ZOVStorageStatisticsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void ZOVExpeditionStatisticsForm_Shown(object sender, EventArgs e)
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        NeedSplash = false;
                        LightStartForm.HideForm(this);
                        this.Hide();
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        NeedSplash = false;
                        LightStartForm.HideForm(this);
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
            StorageStatistics = new ZOVStorageStatistics(
                ref PrepareFSummaryDG, ref PrepareCurvedFSummaryDG, ref PrepareDSummaryDG,
                ref ExpFrontsDataGrid, ref ExpCurvedFrontsDataGrid,
                ref ExpDecorProductsDataGrid, ref ExpDecorItemsDataGrid);

            MarketingBatchReport = new BatchExcelReport();
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

        private void ExpFrontsInfo()
        {
            ExpFrontsSquareLabel.Text = string.Empty;
            ExpFrontsCostLabel.Text = string.Empty;
            ExpFrontsCountLabel.Text = string.Empty;

            decimal ExpFrontCost = 0;
            decimal ExpFrontSquare = 0;
            int ExpFrontsCount = 0;

            StorageStatistics.GetFrontsInfo(ref ExpFrontSquare, ref ExpFrontCost, ref ExpFrontsCount);

            ExpFrontsSquareLabel.Text = ExpFrontSquare.ToString("N", nfi2);
            ExpFrontsCostLabel.Text = ExpFrontCost.ToString("N", nfi2);
            ExpFrontsCountLabel.Text = ExpFrontsCount.ToString();
        }

        private void ExpCurvedFrontsInfo()
        {
            ExpFrontsCostLabel.Text = string.Empty;
            ExpCurvedCountLabel.Text = string.Empty;

            decimal ExpFrontCost = 0;
            int ExpCurvedCount = 0;

            StorageStatistics.GetCurvedFrontsInfo(ref ExpFrontCost, ref ExpCurvedCount);

            ExpCurvedFrontsCostLabel.Text = ExpFrontCost.ToString("N", nfi2);
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

                StorageStatistics.ShowColumns(ref PrepareFSummaryDG, ref PrepareCurvedFSummaryDG, ref PrepareDSummaryDG, Profil, TPS);
                StorageStatistics.PrepareOrders(FactoryID);
                PrepareFSummaryDG.BringToFront();
                PrepareCurvedFSummaryDG.BringToFront();
                PrepareDSummaryDG.BringToFront();
                FrontsPrepareLabel.Visible = true;
                FrontsZOVLabel.Visible = false;
                DecorPrepareLabel.Visible = true;
                DecorZOVLabel.Visible = false;
                if (!StorageStatistics.HasCurvedFronts)
                    StorageStatistics.ClearCurvedFrontsOrders(2);
                if (!StorageStatistics.HasFronts)
                    StorageStatistics.ClearFrontsOrders(2);
                if (!StorageStatistics.HasDecor)
                    StorageStatistics.ClearDecorOrders(2);

                ExpCurvedFrontsInfo();
                ExpFrontsInfo();
                ExpDecorInfo();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void ExpDecorProductsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void ExpFrontsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
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

        private void ExpFilterButton_Click(object sender, EventArgs e)
        {
            if (StorageStatistics == null)
                return;
            FilterExpedition();
        }
    }
}
