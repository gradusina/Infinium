using Infinium.Modules.StatisticsMarketing;

using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingBatchStatisticsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool Marketing = true;
        bool ZOV = false;
        bool NeedSplash = false;
        int FormEvent = 0;
        MarketingBatchStatistics BatchStatistics;
        LightStartForm LightStartForm;

        Form TopForm = null;

        public MarketingBatchStatisticsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void MarketingBatchStatisticsForm_Shown(object sender, EventArgs e)
        {
            pnlGeneralSummary.BringToFront();
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
            BatchStatistics = new Modules.StatisticsMarketing.MarketingBatchStatistics();
            dgvGeneralSummary.DataSource = BatchStatistics.MegaBatchBS;
            dgvSimpleFronts.DataSource = BatchStatistics.SimpleFrontsSummaryBS;
            dgvCurvedFronts.DataSource = BatchStatistics.CurvedFrontsSummaryBS;
            dgvDecor.DataSource = BatchStatistics.DecorProductsSummaryBS;
            MegaBatchGridSettings();
            ProductGridSettings();
        }

        private void MegaBatchGridSettings()
        {
            foreach (DataGridViewColumn Column in dgvGeneralSummary.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgvGeneralSummary.Columns["Firm"].Visible = false;
            dgvGeneralSummary.Columns["GroupType"].Visible = false;
            dgvGeneralSummary.Columns["CreateUserID"].Visible = false;
            dgvGeneralSummary.Columns["ProfilReady"].Visible = false;
            dgvGeneralSummary.Columns["TPSReady"].Visible = false;
            dgvGeneralSummary.Columns["ProfilCloseUserID"].Visible = false;
            dgvGeneralSummary.Columns["TPSCloseUserID"].Visible = false;

            dgvGeneralSummary.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["TPSEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["ProfilPackingDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvGeneralSummary.Columns["TPSPackingDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvGeneralSummary.Columns["Notes"].HeaderText = "Примечание";
            dgvGeneralSummary.Columns["CreateDateTime"].HeaderText = "Партия\n\rсоздана";
            dgvGeneralSummary.Columns["MegaBatchID"].HeaderText = "№ группы\n\rпартий";
            dgvGeneralSummary.Columns["TPSEntryDateTime"].HeaderText = "Вход на пр-во\n\rТПС";
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].HeaderText = "На пр-ве\n\rТПС, %";
            dgvGeneralSummary.Columns["TPSInProductionPerc"].HeaderText = "В пр-ве\n\rТПС, %";
            dgvGeneralSummary.Columns["TPSReadyPerc"].HeaderText = "Готовность\n\rТПС, %";
            dgvGeneralSummary.Columns["TPSPackingDate"].HeaderText = "Выход с пр-ва\n\rТПС";
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].HeaderText = "Вход на пр-во\n\rПрофиль";
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].HeaderText = "На пр-ве\n\rПрофиль, %";
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].HeaderText = "В пр-ве\n\rПрофиль, %";
            dgvGeneralSummary.Columns["ProfilReadyPerc"].HeaderText = "Готовность\n\r Профиль, %";
            dgvGeneralSummary.Columns["ProfilPackingDate"].HeaderText = "Выход с пр-ва\n\rПрофиль";

            dgvGeneralSummary.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["Notes"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["MegaBatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["MegaBatchID"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["CreateDateTime"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["TPSEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["TPSEntryDateTime"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["TPSReadyPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["TPSReadyPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["TPSPackingDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["TPSPackingDate"].MinimumWidth = 100;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].MinimumWidth = 120;
            dgvGeneralSummary.Columns["ProfilPackingDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGeneralSummary.Columns["ProfilPackingDate"].MinimumWidth = 120;

            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("ProfilOnProductionPerc");
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("ProfilInProductionPerc");
            dgvGeneralSummary.Columns["ProfilReadyPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("ProfilReadyPerc");

            dgvGeneralSummary.Columns["TPSOnProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("TPSOnProductionPerc");
            dgvGeneralSummary.Columns["TPSInProductionPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("TPSInProductionPerc");
            dgvGeneralSummary.Columns["TPSReadyPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvGeneralSummary.AddPercentageColumn("TPSReadyPerc");

            dgvGeneralSummary.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvGeneralSummary.Columns["MegaBatchID"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["CreateDateTime"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSEntryDateTime"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSReadyPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["TPSPackingDate"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].DisplayIndex = DisplayIndex++;
            dgvGeneralSummary.Columns["ProfilPackingDate"].DisplayIndex = DisplayIndex++;
        }

        public void ShowBatchColumns(bool Profil, bool TPS)
        {
            dgvGeneralSummary.Columns["TPSEntryDateTime"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSOnProductionPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSInProductionPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSReadyPerc"].Visible = TPS;
            dgvGeneralSummary.Columns["TPSPackingDate"].Visible = TPS;
            dgvGeneralSummary.Columns["ProfilEntryDateTime"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilOnProductionPerc"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilInProductionPerc"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilReadyPerc"].Visible = Profil;
            dgvGeneralSummary.Columns["ProfilPackingDate"].Visible = Profil;
            if (!Profil && !TPS)
            {
                dgvGeneralSummary.Columns["MegaBatchID"].Visible = false;
                dgvGeneralSummary.Columns["Notes"].Visible = false;
            }
            else
            {
                dgvGeneralSummary.Columns["MegaBatchID"].Visible = true;
                dgvGeneralSummary.Columns["Notes"].Visible = true;
            }
        }

        private void ProductGridSettings()
        {
            dgvSimpleFronts.Columns["AllCount"].Visible = false;
            dgvSimpleFronts.Columns["OnProdCount"].Visible = false;
            dgvSimpleFronts.Columns["InProdCount"].Visible = false;
            dgvSimpleFronts.Columns["ReadyCount"].Visible = false;
            dgvSimpleFronts.Columns["Ready"].Visible = false;

            dgvCurvedFronts.Columns["Ready"].Visible = false;

            dgvDecor.Columns["Ready"].Visible = false;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            foreach (DataGridViewColumn Column in dgvSimpleFronts.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in dgvDecor.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgvSimpleFronts.Columns["FrontID"].Visible = false;
            dgvSimpleFronts.Columns["Width"].Visible = false;
            dgvCurvedFronts.Columns["FrontID"].Visible = false;
            dgvCurvedFronts.Columns["Width"].Visible = false;

            dgvSimpleFronts.Columns["Front"].HeaderText = "Фасад";
            dgvSimpleFronts.Columns["AllSquare"].HeaderText = "Общая площадь, м.кв.";
            dgvSimpleFronts.Columns["InProdSquare"].HeaderText = "В пр-ве, м.кв.";
            dgvSimpleFronts.Columns["OnProdSquare"].HeaderText = "На пр-ве, м.кв.";
            dgvSimpleFronts.Columns["ReadySquare"].HeaderText = "Готово, м.кв.";
            dgvSimpleFronts.Columns["AllCount"].HeaderText = "Общее кол-во, шт.";
            dgvSimpleFronts.Columns["InProdCount"].HeaderText = "В пр-ве, шт.";
            dgvSimpleFronts.Columns["OnProdCount"].HeaderText = "На пр-ве, шт.";
            dgvSimpleFronts.Columns["ReadyCount"].HeaderText = "Готово, шт.";

            dgvCurvedFronts.Columns["Front"].HeaderText = "Фасад";
            dgvCurvedFronts.Columns["AllCount"].HeaderText = "Общее кол-во, шт.";
            dgvCurvedFronts.Columns["InProdCount"].HeaderText = "В пр-ве, шт.";
            dgvCurvedFronts.Columns["OnProdCount"].HeaderText = "На пр-ве, шт.";
            dgvCurvedFronts.Columns["ReadyCount"].HeaderText = "Готово, шт.";

            dgvSimpleFronts.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["Front"].MinimumWidth = 245;
            dgvSimpleFronts.Columns["AllSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["AllSquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["InProdSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["InProdSquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["OnProdSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["OnProdSquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["ReadySquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSimpleFronts.Columns["ReadySquare"].MinimumWidth = 120;
            dgvSimpleFronts.Columns["InProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["InProdCount"].MinimumWidth = 110;
            dgvSimpleFronts.Columns["OnProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["OnProdCount"].MinimumWidth = 110;
            dgvSimpleFronts.Columns["ReadyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["ReadyCount"].MinimumWidth = 110;
            dgvSimpleFronts.Columns["AllCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSimpleFronts.Columns["AllCount"].MinimumWidth = 110;

            //dgvCurvedFronts.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dgvCurvedFronts.Columns["Front"].MinimumWidth = 245;
            //dgvCurvedFronts.Columns["InProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvCurvedFronts.Columns["InProdCount"].MinimumWidth = 110;
            //dgvCurvedFronts.Columns["OnProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvCurvedFronts.Columns["OnProdCount"].MinimumWidth = 110;
            //dgvCurvedFronts.Columns["ReadyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvCurvedFronts.Columns["ReadyCount"].MinimumWidth = 110;
            //dgvCurvedFronts.Columns["AllCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvCurvedFronts.Columns["AllCount"].MinimumWidth = 110;

            dgvSimpleFronts.Columns["AllSquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["AllSquare"].DefaultCellStyle.FormatProvider = nfi1;
            dgvSimpleFronts.Columns["InProdSquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["InProdSquare"].DefaultCellStyle.FormatProvider = nfi1;
            dgvSimpleFronts.Columns["OnProdSquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["OnProdSquare"].DefaultCellStyle.FormatProvider = nfi1;
            dgvSimpleFronts.Columns["ReadySquare"].DefaultCellStyle.Format = "N";
            dgvSimpleFronts.Columns["ReadySquare"].DefaultCellStyle.FormatProvider = nfi1;

            dgvDecor.Columns["ProductID"].Visible = false;
            dgvDecor.Columns["MeasureID"].Visible = false;

            dgvDecor.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["DecorProduct"].MinimumWidth = 245;
            dgvDecor.Columns["AllCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["AllCount"].MinimumWidth = 150;
            dgvDecor.Columns["InProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["InProdCount"].MinimumWidth = 150;
            dgvDecor.Columns["OnProdCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["OnProdCount"].MinimumWidth = 150;
            dgvDecor.Columns["ReadyCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecor.Columns["ReadyCount"].MinimumWidth = 150;
            dgvDecor.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecor.Columns["Measure"].MinimumWidth = 140;

            dgvDecor.Columns["DecorProduct"].HeaderText = "Продукт";
            dgvDecor.Columns["AllCount"].HeaderText = "Общее кол-во";
            dgvDecor.Columns["InProdCount"].HeaderText = "В пр-ве, кол-во";
            dgvDecor.Columns["OnProdCount"].HeaderText = "На пр-ве, кол-во";
            dgvDecor.Columns["ReadyCount"].HeaderText = "Готово, кол-во";
            dgvDecor.Columns["Measure"].HeaderText = "Ед.изм.";

            dgvDecor.Columns["AllCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["AllCount"].DefaultCellStyle.FormatProvider = nfi1;
            dgvDecor.Columns["InProdCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["InProdCount"].DefaultCellStyle.FormatProvider = nfi1;
            dgvDecor.Columns["OnProdCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["OnProdCount"].DefaultCellStyle.FormatProvider = nfi1;
            dgvDecor.Columns["ReadyCount"].DefaultCellStyle.Format = "N";
            dgvDecor.Columns["ReadyCount"].DefaultCellStyle.FormatProvider = nfi1;

            dgvSimpleFronts.Columns["AllSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["InProdSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["OnProdSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["ReadySquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["AllCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["InProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["OnProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSimpleFronts.Columns["ReadyCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCurvedFronts.Columns["AllCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCurvedFronts.Columns["InProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCurvedFronts.Columns["OnProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCurvedFronts.Columns["ReadyCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDecor.Columns["AllCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDecor.Columns["InProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDecor.Columns["OnProdCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDecor.Columns["ReadyCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
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

        private void MarketingBatchStatisticsForm_Load(object sender, EventArgs e)
        {

        }

        private void dgvGeneralSummary_SelectionChanged(object sender, EventArgs e)
        {
            lbAllSFSquare.Text = string.Empty;
            lbAllSFCount.Text = string.Empty;
            lbAllCFCount.Text = string.Empty;

            lbOnProdSFSquare.Text = string.Empty;
            lbOnProdSFCount.Text = string.Empty;
            lbOnCFCount.Text = string.Empty;

            lbInProdSFSquare.Text = string.Empty;
            lbInProdSFCount.Text = string.Empty;
            lbInCFCount.Text = string.Empty;

            lbReadySFSquare.Text = string.Empty;
            lbReadySFCount.Text = string.Empty;
            lbReadyCFCount.Text = string.Empty;

            lbAllDecorPogon.Text = string.Empty;
            lbAllDecorCount.Text = string.Empty;

            lbOnProdDecorPogon.Text = string.Empty;
            lbOnProdDecorCount.Text = string.Empty;

            lbInProdDecorPogon.Text = string.Empty;
            lbInProdDecorCount.Text = string.Empty;

            lbReadyDecorPogon.Text = string.Empty;
            lbReadyDecorCount.Text = string.Empty;

            if (BatchStatistics == null)
                return;
            if (dgvGeneralSummary.Rows.Count == 0)
            {
                BatchStatistics.ClearProductTables();
                return;
            }

            bool Profil = BatchProfilCheckBox.Checked;
            bool TPS = BatchTPSCheckBox.Checked;
            int FactoryID = 0;

            decimal AllSquare = 0;
            decimal OnProdSquare = 0;
            decimal InProdSquare = 0;
            decimal ReadySquare = 0;

            int AllCount = 0;
            int OnProdCount = 0;
            int InProdCount = 0;
            int ReadyCount = 0;

            int AllCurvedCount = 0;
            int OnProdCurvedCount = 0;
            int InProdCurvedCount = 0;
            int ReadyCurvedCount = 0;

            decimal AllDecorPogon = 0;
            decimal OnProdDecorPogon = 0;
            decimal InProdDecorPogon = 0;
            decimal ReadyDecorPogon = 0;

            int AllDecorCount = 0;
            int OnProdDecorCount = 0;
            int InProdDecorCount = 0;
            int ReadyDecorCount = 0;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;
            int GroupType = -1;
            int MegaBatchID = -1;
            if (dgvGeneralSummary.SelectedRows.Count != 0 && dgvGeneralSummary.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvGeneralSummary.SelectedRows[0].Cells["GroupType"].Value);
            if (dgvGeneralSummary.SelectedRows.Count != 0 && dgvGeneralSummary.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvGeneralSummary.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                if (cbtnSimpleFronts.Checked)
                {
                    BatchStatistics.FilterSimpleFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetSimpleFrontsInfo(ref AllSquare, ref OnProdSquare, ref InProdSquare, ref ReadySquare,
                        ref AllCount, ref OnProdCount, ref InProdCount, ref ReadyCount);
                }
                if (cbtnCurvedFronts.Checked)
                {
                    BatchStatistics.FilterCurvedFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetCurvedFrontsInfo(ref AllCurvedCount, ref OnProdCurvedCount, ref InProdCurvedCount, ref ReadyCurvedCount);
                }
                if (cbtnDecor.Checked)
                {
                    BatchStatistics.FilterDecorOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetDecorInfo(ref AllDecorPogon, ref OnProdDecorPogon, ref InProdDecorPogon, ref ReadyDecorPogon,
                        ref AllDecorCount, ref OnProdDecorCount, ref InProdDecorCount, ref ReadyDecorCount);
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                if (cbtnSimpleFronts.Checked)
                {
                    BatchStatistics.FilterSimpleFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetSimpleFrontsInfo(ref AllSquare, ref OnProdSquare, ref InProdSquare, ref ReadySquare,
                        ref AllCount, ref OnProdCount, ref InProdCount, ref ReadyCount);
                }
                if (cbtnCurvedFronts.Checked)
                {
                    BatchStatistics.FilterCurvedFrontsOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetCurvedFrontsInfo(ref AllCurvedCount, ref OnProdCurvedCount, ref InProdCurvedCount, ref ReadyCurvedCount);
                }
                if (cbtnDecor.Checked)
                {
                    BatchStatistics.FilterDecorOrders(GroupType, MegaBatchID, FactoryID);
                    BatchStatistics.GetDecorInfo(ref AllDecorPogon, ref OnProdDecorPogon, ref InProdDecorPogon, ref ReadyDecorPogon,
                        ref AllDecorCount, ref OnProdDecorCount, ref InProdDecorCount, ref ReadyDecorCount);
                }
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ","
            };
            lbAllSFSquare.Text = AllSquare.ToString("N", nfi1);
            lbAllSFCount.Text = AllCount.ToString();
            lbAllCFCount.Text = AllCurvedCount.ToString();

            lbOnProdSFSquare.Text = OnProdSquare.ToString("N", nfi1);
            lbOnProdSFCount.Text = OnProdCount.ToString();
            lbOnCFCount.Text = OnProdCurvedCount.ToString();

            lbInProdSFSquare.Text = InProdSquare.ToString("N", nfi1);
            lbInProdSFCount.Text = InProdCount.ToString();
            lbInCFCount.Text = InProdCurvedCount.ToString();

            lbReadySFSquare.Text = ReadySquare.ToString("N", nfi1);
            lbReadySFCount.Text = ReadyCount.ToString();
            lbReadyCFCount.Text = ReadyCurvedCount.ToString();

            lbAllDecorPogon.Text = AllDecorPogon.ToString("N", nfi1);
            lbAllDecorCount.Text = AllDecorCount.ToString();

            lbOnProdDecorPogon.Text = OnProdDecorPogon.ToString("N", nfi1);
            lbOnProdDecorCount.Text = OnProdDecorCount.ToString();

            lbInProdDecorPogon.Text = InProdDecorPogon.ToString("N", nfi1);
            lbInProdDecorCount.Text = InProdDecorCount.ToString();

            lbReadyDecorPogon.Text = ReadyDecorPogon.ToString("N", nfi1);
            lbReadyDecorCount.Text = ReadyDecorCount.ToString();
        }

        private void BatchFilter()
        {
            bool Profil = BatchProfilCheckBox.Checked;
            bool TPS = BatchTPSCheckBox.Checked;
            bool DoNotShowReady = DoNotShowReadyCheckBox.Checked;
            DateTime FirstDate = mcBatchFirstDate.SelectionStart;
            DateTime SecondDate = mcBatchSecondDate.SelectionStart;

            dgvGeneralSummary.SelectionChanged -= dgvGeneralSummary_SelectionChanged;
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            BatchStatistics.UpdateMegaBatch(Marketing, ZOV, FirstDate, SecondDate);
            BatchStatistics.UpdateFrontsOrders(Marketing, ZOV, FirstDate, SecondDate);
            BatchStatistics.UpdateDecorOrders(Marketing, ZOV, FirstDate, SecondDate);

            if (cbtnSimpleFronts.Checked)
            {
                BatchStatistics.SimpleFrontsGeneralSummary();
                BatchStatistics.FilterBatch(Marketing, ZOV, Profil, TPS, DoNotShowReady, true, false, false);
            }
            if (cbtnCurvedFronts.Checked)
            {
                BatchStatistics.CurvedFrontsGeneralSummary();
                BatchStatistics.FilterBatch(Marketing, ZOV, Profil, TPS, DoNotShowReady, false, true, false);
            }
            if (cbtnDecor.Checked)
            {
                BatchStatistics.DecorGeneralSummary();
                BatchStatistics.FilterBatch(Marketing, ZOV, Profil, TPS, DoNotShowReady, false, false, true);
            }
            ShowBatchColumns(Profil, TPS);
            dgvGeneralSummary.SelectionChanged += dgvGeneralSummary_SelectionChanged;
            dgvGeneralSummary_SelectionChanged(null, null);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void dgvSimpleFronts_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (Convert.ToBoolean(grid.Rows[e.RowIndex].Cells["Ready"].Value))
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(209, 232, 204);
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
            else
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;

            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            BatchMenuPanel.Visible = !BatchMenuPanel.Visible;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (BatchStatistics == null)
                return;

            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            bool Profil = BatchProfilCheckBox.Checked;
            bool TPS = BatchTPSCheckBox.Checked;
            bool DoNotShowReady = DoNotShowReadyCheckBox.Checked;
            if (cbtnSimpleFronts.Checked)
            {
                BatchStatistics.SimpleFrontsGeneralSummary();
                BatchStatistics.FilterBatch(Marketing, ZOV, Profil, TPS, DoNotShowReady, true, false, false);
                pnlSimpleFronts.BringToFront();
            }
            if (cbtnCurvedFronts.Checked)
            {
                BatchStatistics.CurvedFrontsGeneralSummary();
                BatchStatistics.FilterBatch(Marketing, ZOV, Profil, TPS, DoNotShowReady, false, true, false);
                pnlCurvedFronts.BringToFront();
            }
            if (cbtnDecor.Checked)
            {
                BatchStatistics.DecorGeneralSummary();
                BatchStatistics.FilterBatch(Marketing, ZOV, Profil, TPS, DoNotShowReady, false, false, true);
                pnlDecor.BringToFront();
            }
            dgvGeneralSummary_SelectionChanged(null, null);
            cbtnGeneralSummary.Checked = true;
            pnlGeneralSummary.BringToFront();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (BatchStatistics == null)
                return;
            if (cbtnGeneralSummary.Checked)
            {
                pnlGeneralSummary.BringToFront();
            }
            if (cbtnProductSummary.Checked)
            {
                if (cbtnSimpleFronts.Checked)
                {
                    pnlSimpleFronts.BringToFront();
                }
                if (cbtnCurvedFronts.Checked)
                {
                    pnlCurvedFronts.BringToFront();
                }
                if (cbtnDecor.Checked)
                {
                    pnlDecor.BringToFront();
                }
            }
        }

        private void btnBatchFilter_Click(object sender, EventArgs e)
        {
            if (BatchStatistics == null)
                return;
            BatchFilter();
            MenuButton.Checked = false;
            BatchMenuPanel.Visible = !BatchMenuPanel.Visible;
        }

    }
}
