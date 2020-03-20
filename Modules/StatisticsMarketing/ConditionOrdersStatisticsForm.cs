using Infinium.Modules.Marketing.NewOrders;
using Infinium.Modules.StatisticsMarketing;

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ConditionOrdersStatisticsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;
        int CurrentWeekNumber = 1;
        int FormEvent = 0;

        NumberFormatInfo nfi2;

        LightStartForm LightStartForm;

        Form TopForm = null;

        BatchExcelReport MarketingBatchReport;
        ConditionOrdersStatistics ConditionOrdersStatistics;

        public ConditionOrdersStatisticsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void ConditionOrdersStatisticsForm_Shown(object sender, EventArgs e)
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

            ConditionOrdersStatistics = new ConditionOrdersStatistics();
            ConditionGroupsDG.DataSource = ConditionOrdersStatistics.ClientGroupsBS;
            ConditionOrdersGrids1();
            ConditionOrdersGrids2();
            ConditionOrdersGrids3();
            CurrentWeekNumber = GetWeekNumber(DateTime.Now);

            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            ArrayList Years = new ArrayList();
            for (int i = 2013; i <= LastDay.Year; i++)
            {
                Years.Add(i);
            }
            cbxYears.DataSource = Years.ToArray();
            cbxYears.SelectedIndex = cbxYears.Items.Count - 1;

            DateTime Monday = GetMonday(CurrentWeekNumber);
            DateTime Wednesday = GetWednesday(CurrentWeekNumber);
            DateTime Friday = GetFriday(CurrentWeekNumber);
            int WeeksCount = GetWeekNumber(LastDay);

            for (int i = 1; i <= WeeksCount; i++)
            {
                WeeksOfYearListBox.Items.Add(i);
            }
            WeeksOfYearListBox.SelectedIndex = CurrentWeekNumber - 1;

            FMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            FWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            FFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");
            DMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            DWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            DFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");

        }

        private void ConditionOrdersGrids1()
        {
            foreach (DataGridViewColumn Column in ConditionGroupsDG.Columns)
            {
                Column.ReadOnly = true;
            }

            ConditionGroupsDG.Columns["ClientGroupID"].Visible = false;
            ConditionGroupsDG.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ConditionGroupsDG.Columns["Check"].Width = 40;
            ConditionGroupsDG.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ConditionGroupsDG.AutoGenerateColumns = false;
            ConditionGroupsDG.Columns["Check"].ReadOnly = false;
            ConditionGroupsDG.Columns["Check"].DisplayIndex = 0;
            ConditionGroupsDG.Columns["ClientGroupName"].DisplayIndex = 1;

            MondayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT);
            MondayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT);
            MondayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT);

            if (!Security.PriceAccess)
            {
                MondayFrontsDG.Columns["Cost"].Visible = false;
                MondayDecorProductsDG.Columns["Cost"].Visible = false;
                MondayDecorItemsDG.Columns["Cost"].Visible = false;
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
            MondayFrontsDG.ColumnHeadersHeight = 38;
            MondayDecorProductsDG.ColumnHeadersHeight = 38;
            MondayDecorItemsDG.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in MondayFrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in MondayDecorProductsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in MondayDecorItemsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            MondayFrontsDG.Columns["FrontID"].Visible = false;
            MondayFrontsDG.Columns["Width"].Visible = false;

            MondayFrontsDG.Columns["Front"].HeaderText = "Фасад";
            MondayFrontsDG.Columns["Cost"].HeaderText = " € ";
            MondayFrontsDG.Columns["Square"].HeaderText = "м.кв.";
            MondayFrontsDG.Columns["Count"].HeaderText = "шт.";

            MondayFrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MondayFrontsDG.Columns["Front"].MinimumWidth = 110;
            MondayFrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayFrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayFrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayFrontsDG.Columns["Square"].Width = 100;
            MondayFrontsDG.Columns["Cost"].Width = 100;
            MondayFrontsDG.Columns["Count"].Width = 90;

            MondayFrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            MondayFrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            MondayFrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            MondayFrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            MondayDecorProductsDG.Columns["ProductID"].Visible = false;
            MondayDecorProductsDG.Columns["MeasureID"].Visible = false;

            MondayDecorItemsDG.Columns["ProductID"].Visible = false;
            MondayDecorItemsDG.Columns["DecorID"].Visible = false;
            MondayDecorItemsDG.Columns["MeasureID"].Visible = false;

            MondayDecorProductsDG.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MondayDecorProductsDG.Columns["DecorProduct"].MinimumWidth = 100;
            MondayDecorProductsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorProductsDG.Columns["Cost"].Width = 100;
            MondayDecorProductsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorProductsDG.Columns["Count"].Width = 100;
            MondayDecorProductsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorProductsDG.Columns["Measure"].Width = 90;

            MondayDecorProductsDG.Columns["DecorProduct"].HeaderText = "Продукт";
            MondayDecorProductsDG.Columns["Cost"].HeaderText = " € ";
            MondayDecorProductsDG.Columns["Count"].HeaderText = "Кол-во";
            MondayDecorProductsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            MondayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            MondayDecorProductsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            MondayDecorProductsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            MondayDecorProductsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            MondayDecorItemsDG.Columns["DecorID"].Visible = false;

            MondayDecorItemsDG.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MondayDecorItemsDG.Columns["DecorItem"].MinimumWidth = 100;
            MondayDecorItemsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorItemsDG.Columns["Cost"].Width = 100;
            MondayDecorItemsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorItemsDG.Columns["Count"].Width = 100;
            MondayDecorItemsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MondayDecorItemsDG.Columns["Measure"].Width = 90;

            MondayDecorItemsDG.Columns["DecorItem"].HeaderText = "Наименование";
            MondayDecorItemsDG.Columns["Cost"].HeaderText = " € ";
            MondayDecorItemsDG.Columns["Count"].HeaderText = "Кол-во";
            MondayDecorItemsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            MondayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            MondayDecorItemsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            MondayDecorItemsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            MondayDecorItemsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            MondayFrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayFrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayFrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MondayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayDecorProductsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MondayDecorItemsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConditionOrdersGrids2()
        {
            WednesdayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT);
            WednesdayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT);
            WednesdayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT);

            if (!Security.PriceAccess)
            {
                WednesdayFrontsDG.Columns["Cost"].Visible = false;
                WednesdayDecorProductsDG.Columns["Cost"].Visible = false;
                WednesdayDecorItemsDG.Columns["Cost"].Visible = false;
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
            WednesdayFrontsDG.ColumnHeadersHeight = 38;
            WednesdayDecorProductsDG.ColumnHeadersHeight = 38;
            WednesdayDecorItemsDG.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in WednesdayFrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in WednesdayDecorProductsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in WednesdayDecorItemsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            WednesdayFrontsDG.Columns["FrontID"].Visible = false;
            WednesdayFrontsDG.Columns["Width"].Visible = false;

            WednesdayFrontsDG.Columns["Front"].HeaderText = "Фасад";
            WednesdayFrontsDG.Columns["Cost"].HeaderText = " € ";
            WednesdayFrontsDG.Columns["Square"].HeaderText = "м.кв.";
            WednesdayFrontsDG.Columns["Count"].HeaderText = "шт.";

            WednesdayFrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WednesdayFrontsDG.Columns["Front"].MinimumWidth = 110;
            WednesdayFrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayFrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayFrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayFrontsDG.Columns["Square"].Width = 100;
            WednesdayFrontsDG.Columns["Cost"].Width = 100;
            WednesdayFrontsDG.Columns["Count"].Width = 90;

            WednesdayFrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            WednesdayFrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            WednesdayFrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            WednesdayFrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            WednesdayDecorProductsDG.Columns["ProductID"].Visible = false;
            WednesdayDecorProductsDG.Columns["MeasureID"].Visible = false;

            WednesdayDecorItemsDG.Columns["ProductID"].Visible = false;
            WednesdayDecorItemsDG.Columns["DecorID"].Visible = false;
            WednesdayDecorItemsDG.Columns["MeasureID"].Visible = false;

            WednesdayDecorProductsDG.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WednesdayDecorProductsDG.Columns["DecorProduct"].MinimumWidth = 100;
            WednesdayDecorProductsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorProductsDG.Columns["Cost"].Width = 100;
            WednesdayDecorProductsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorProductsDG.Columns["Count"].Width = 100;
            WednesdayDecorProductsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorProductsDG.Columns["Measure"].Width = 90;

            WednesdayDecorProductsDG.Columns["DecorProduct"].HeaderText = "Продукт";
            WednesdayDecorProductsDG.Columns["Cost"].HeaderText = " € ";
            WednesdayDecorProductsDG.Columns["Count"].HeaderText = "Кол-во";
            WednesdayDecorProductsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            WednesdayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            WednesdayDecorProductsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            WednesdayDecorProductsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            WednesdayDecorProductsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            WednesdayDecorItemsDG.Columns["DecorID"].Visible = false;

            WednesdayDecorItemsDG.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WednesdayDecorItemsDG.Columns["DecorItem"].MinimumWidth = 100;
            WednesdayDecorItemsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorItemsDG.Columns["Cost"].Width = 100;
            WednesdayDecorItemsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorItemsDG.Columns["Count"].Width = 100;
            WednesdayDecorItemsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WednesdayDecorItemsDG.Columns["Measure"].Width = 90;

            WednesdayDecorItemsDG.Columns["DecorItem"].HeaderText = "Наименование";
            WednesdayDecorItemsDG.Columns["Cost"].HeaderText = " € ";
            WednesdayDecorItemsDG.Columns["Count"].HeaderText = "Кол-во";
            WednesdayDecorItemsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            WednesdayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            WednesdayDecorItemsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            WednesdayDecorItemsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            WednesdayDecorItemsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            WednesdayFrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayFrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayFrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            WednesdayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayDecorProductsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            WednesdayDecorItemsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConditionOrdersGrids3()
        {
            FridayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT);
            FridayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT);
            FridayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT);

            if (!Security.PriceAccess)
            {
                FridayFrontsDG.Columns["Cost"].Visible = false;
                FridayDecorProductsDG.Columns["Cost"].Visible = false;
                FridayDecorItemsDG.Columns["Cost"].Visible = false;
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
            FridayFrontsDG.ColumnHeadersHeight = 38;
            FridayDecorProductsDG.ColumnHeadersHeight = 38;
            FridayDecorItemsDG.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in FridayFrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in FridayDecorProductsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in FridayDecorItemsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            FridayFrontsDG.Columns["FrontID"].Visible = false;
            FridayFrontsDG.Columns["Width"].Visible = false;

            FridayFrontsDG.Columns["Front"].HeaderText = "Фасад";
            FridayFrontsDG.Columns["Cost"].HeaderText = " € ";
            FridayFrontsDG.Columns["Square"].HeaderText = "м.кв.";
            FridayFrontsDG.Columns["Count"].HeaderText = "шт.";

            FridayFrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FridayFrontsDG.Columns["Front"].MinimumWidth = 110;
            FridayFrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayFrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayFrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayFrontsDG.Columns["Square"].Width = 100;
            FridayFrontsDG.Columns["Cost"].Width = 100;
            FridayFrontsDG.Columns["Count"].Width = 90;

            FridayFrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FridayFrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FridayFrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            FridayFrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            FridayDecorProductsDG.Columns["ProductID"].Visible = false;
            FridayDecorProductsDG.Columns["MeasureID"].Visible = false;

            FridayDecorItemsDG.Columns["ProductID"].Visible = false;
            FridayDecorItemsDG.Columns["DecorID"].Visible = false;
            FridayDecorItemsDG.Columns["MeasureID"].Visible = false;

            FridayDecorProductsDG.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FridayDecorProductsDG.Columns["DecorProduct"].MinimumWidth = 100;
            FridayDecorProductsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorProductsDG.Columns["Cost"].Width = 100;
            FridayDecorProductsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorProductsDG.Columns["Count"].Width = 100;
            FridayDecorProductsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorProductsDG.Columns["Measure"].Width = 90;

            FridayDecorProductsDG.Columns["DecorProduct"].HeaderText = "Продукт";
            FridayDecorProductsDG.Columns["Cost"].HeaderText = " € ";
            FridayDecorProductsDG.Columns["Count"].HeaderText = "Кол-во";
            FridayDecorProductsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            FridayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FridayDecorProductsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FridayDecorProductsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            FridayDecorProductsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            FridayDecorItemsDG.Columns["DecorID"].Visible = false;

            FridayDecorItemsDG.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FridayDecorItemsDG.Columns["DecorItem"].MinimumWidth = 100;
            FridayDecorItemsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorItemsDG.Columns["Cost"].Width = 100;
            FridayDecorItemsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorItemsDG.Columns["Count"].Width = 100;
            FridayDecorItemsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FridayDecorItemsDG.Columns["Measure"].Width = 90;

            FridayDecorItemsDG.Columns["DecorItem"].HeaderText = "Наименование";
            FridayDecorItemsDG.Columns["Cost"].HeaderText = " € ";
            FridayDecorItemsDG.Columns["Count"].HeaderText = "Кол-во";
            FridayDecorItemsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            FridayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FridayDecorItemsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FridayDecorItemsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            FridayDecorItemsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            FridayFrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayFrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayFrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            FridayDecorProductsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayDecorProductsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayDecorItemsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FridayDecorItemsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
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

        private void ConditionOrdersStatisticsForm_Load(object sender, EventArgs e)
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
            ConditionMenuPanel.Visible = !ConditionMenuPanel.Visible;
        }

        private void MondayFrontsDG_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu7.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void ConditionOrdersFilter_Click(object sender, EventArgs e)
        {
            bool OnAgreementOrders = rbtnOnAgreementOrders.Checked;
            bool AgreedOrders = rbtnAgreedOrders.Checked;
            bool OnProductionOrders = rbtnOnProduction.Checked;
            bool InProductionOrders = rbtnInProduction.Checked;

            decimal FrontCost = 0;
            decimal FrontSquare = 0;
            int FrontsCount = 0;
            int CurvedCount = 0;

            decimal DecorPogon = 0;
            decimal DecorCost = 0;
            int DecorCount = 0;

            DateTime Monday = GetMonday(CurrentWeekNumber);
            DateTime Wednesday = GetWednesday(CurrentWeekNumber);
            DateTime Friday = GetFriday(CurrentWeekNumber);

            FMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            FWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            FFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");
            DMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            DWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            DFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");

            int FactoryID = 0;

            if (CProfilCheckBox.Checked && !CTPSCheckBox.Checked)
                FactoryID = 1;
            if (!CProfilCheckBox.Checked && CTPSCheckBox.Checked)
                FactoryID = 2;
            if (!CProfilCheckBox.Checked && !CTPSCheckBox.Checked)
                FactoryID = -1;

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                ConditionOrdersStatistics.ClearOrders();
                if (Monday < DateTime.Now && MondayCB.Checked)
                {
                    if (OnAgreementOrders)
                        ConditionOrdersStatistics.GetOnAgreementOrders(Monday, FactoryID);
                    if (AgreedOrders)
                        ConditionOrdersStatistics.GetAgreedOrders(Monday, FactoryID);
                    if (OnProductionOrders)
                        ConditionOrdersStatistics.GetOnProductionOrders(Monday, FactoryID);
                    if (InProductionOrders)
                        ConditionOrdersStatistics.GetInProductionOrders(Monday, FactoryID);
                }

                MondayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT.Copy());
                MondayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT.Copy());
                MondayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT.Copy());
                ((DataView)MondayFrontsDG.DataSource).Sort = "Front, Square DESC";
                ((DataView)MondayDecorProductsDG.DataSource).Sort = "DecorProduct, Measure ASC, Count DESC";
                ((DataView)MondayDecorItemsDG.DataSource).Sort = "DecorItem, Count DESC";

                FrontCost = 0;
                FrontSquare = 0;
                FrontsCount = 0;
                CurvedCount = 0;
                MondayFrontsSquareLabel.Text = string.Empty;
                MondayFrontsCostLabel.Text = string.Empty;
                MondayFrontsCountLabel.Text = string.Empty;
                MondayCurvedCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
                MondayFrontsSquareLabel.Text = FrontSquare.ToString("N", nfi2);
                MondayFrontsCostLabel.Text = FrontCost.ToString("N", nfi2);
                MondayFrontsCountLabel.Text = FrontsCount.ToString();
                MondayCurvedCountLabel.Text = CurvedCount.ToString();

                DecorPogon = 0;
                DecorCost = 0;
                DecorCount = 0;
                MondayDecorPogonLabel.Text = string.Empty;
                MondayDecorCostLabel.Text = string.Empty;
                MondayDecorCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetDecorInfo(ref DecorPogon, ref DecorCost, ref DecorCount);
                MondayDecorPogonLabel.Text = DecorPogon.ToString("N", nfi2);
                MondayDecorCostLabel.Text = DecorCost.ToString("N", nfi2);
                MondayDecorCountLabel.Text = DecorCount.ToString();

                ConditionOrdersStatistics.ClearOrders();
                if (Wednesday < DateTime.Now && WednesdayCB.Checked)
                {
                    if (OnAgreementOrders)
                        ConditionOrdersStatistics.GetOnAgreementOrders(Wednesday, FactoryID);
                    if (AgreedOrders)
                        ConditionOrdersStatistics.GetAgreedOrders(Wednesday, FactoryID);
                    if (OnProductionOrders)
                        ConditionOrdersStatistics.GetOnProductionOrders(Wednesday, FactoryID);
                    if (InProductionOrders)
                        ConditionOrdersStatistics.GetInProductionOrders(Wednesday, FactoryID);
                }

                WednesdayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT.Copy());
                WednesdayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT.Copy());
                WednesdayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT.Copy());
                ((DataView)WednesdayFrontsDG.DataSource).Sort = "Front, Square DESC";
                ((DataView)WednesdayDecorProductsDG.DataSource).Sort = "DecorProduct, Measure ASC, Count DESC";
                ((DataView)WednesdayDecorItemsDG.DataSource).Sort = "DecorItem, Count DESC";

                FrontCost = 0;
                FrontSquare = 0;
                FrontsCount = 0;
                CurvedCount = 0;
                WednesdayFrontsSquareLabel.Text = string.Empty;
                WednesdayFrontsCostLabel.Text = string.Empty;
                WednesdayFrontsCountLabel.Text = string.Empty;
                WednesdayCurvedCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
                WednesdayFrontsSquareLabel.Text = FrontSquare.ToString("N", nfi2);
                WednesdayFrontsCostLabel.Text = FrontCost.ToString("N", nfi2);
                WednesdayFrontsCountLabel.Text = FrontsCount.ToString();
                WednesdayCurvedCountLabel.Text = CurvedCount.ToString();

                DecorPogon = 0;
                DecorCost = 0;
                DecorCount = 0;
                WednesdayDecorPogonLabel.Text = string.Empty;
                WednesdayDecorCostLabel.Text = string.Empty;
                WednesdayDecorCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetDecorInfo(ref DecorPogon, ref DecorCost, ref DecorCount);
                WednesdayDecorPogonLabel.Text = DecorPogon.ToString("N", nfi2);
                WednesdayDecorCostLabel.Text = DecorCost.ToString("N", nfi2);
                WednesdayDecorCountLabel.Text = DecorCount.ToString();

                ConditionOrdersStatistics.ClearOrders();
                if (Friday < DateTime.Now && FridayCB.Checked)
                {
                    if (OnAgreementOrders)
                        ConditionOrdersStatistics.GetOnAgreementOrders(Friday, FactoryID);
                    if (AgreedOrders)
                        ConditionOrdersStatistics.GetAgreedOrders(Friday, FactoryID);
                    if (OnProductionOrders)
                        ConditionOrdersStatistics.GetOnProductionOrders(Friday, FactoryID);
                    if (InProductionOrders)
                        ConditionOrdersStatistics.GetInProductionOrders(Friday, FactoryID);
                }

                FridayFrontsDG.DataSource = new DataView(ConditionOrdersStatistics.FrontsSummaryDT.Copy());
                FridayDecorProductsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorProductsSummaryDT.Copy());
                FridayDecorItemsDG.DataSource = new DataView(ConditionOrdersStatistics.DecorItemsSummaryDT.Copy());
                ((DataView)FridayFrontsDG.DataSource).Sort = "Front, Square DESC";
                ((DataView)FridayDecorProductsDG.DataSource).Sort = "DecorProduct, Measure ASC, Count DESC";
                ((DataView)FridayDecorItemsDG.DataSource).Sort = "DecorItem, Count DESC";

                FrontCost = 0;
                FrontSquare = 0;
                FrontsCount = 0;
                CurvedCount = 0;
                FridayFrontsSquareLabel.Text = string.Empty;
                FridayFrontsCostLabel.Text = string.Empty;
                FridayFrontsCountLabel.Text = string.Empty;
                FridayCurvedCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetFrontsInfo(ref FrontSquare, ref FrontCost, ref FrontsCount, ref CurvedCount);
                FridayFrontsSquareLabel.Text = FrontSquare.ToString("N", nfi2);
                FridayFrontsCostLabel.Text = FrontCost.ToString("N", nfi2);
                FridayFrontsCountLabel.Text = FrontsCount.ToString();
                FridayCurvedCountLabel.Text = CurvedCount.ToString();

                DecorPogon = 0;
                DecorCost = 0;
                DecorCount = 0;
                FridayDecorPogonLabel.Text = string.Empty;
                FridayDecorCostLabel.Text = string.Empty;
                FridayDecorCountLabel.Text = string.Empty;
                ConditionOrdersStatistics.GetDecorInfo(ref DecorPogon, ref DecorCost, ref DecorCount);
                FridayDecorPogonLabel.Text = DecorPogon.ToString("N", nfi2);
                FridayDecorCostLabel.Text = DecorCost.ToString("N", nfi2);
                FridayDecorCountLabel.Text = DecorCount.ToString();

                MondayDecorProductsDG_SelectionChanged(null, null);
                WednesdayDecorProductsDG_SelectionChanged(null, null);
                FridayDecorProductsDG_SelectionChanged(null, null);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                NeedSplash = true;
            }
            else
            {

            }
        }

        public int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private DateTime GetFriday(int WeekNumber)
        {
            DateTime Friday;

            DateTime StartDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 1, 1);
            int OffsetToFirstDay = 0;
            if (StartDate.DayOfWeek == DayOfWeek.Thursday)
                OffsetToFirstDay = 6;
            else
                OffsetToFirstDay = Convert.ToInt32(StartDate.DayOfWeek) - 5;
            int OffsetToDemandedDay = 7 * (WeekNumber - 1) - OffsetToFirstDay;
            Friday = StartDate + new TimeSpan(OffsetToDemandedDay, 0, 0, 0);

            return Friday;
        }

        private DateTime GetMonday(int WeekNumber)
        {
            DateTime Monday;

            DateTime StartDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 1, 1);
            int OffsetToFirstDay = 0;
            if (StartDate.DayOfWeek == DayOfWeek.Thursday)
                OffsetToFirstDay = 6;
            else
                OffsetToFirstDay = Convert.ToInt32(StartDate.DayOfWeek) - 1;
            int OffsetToDemandedDay = 7 * (WeekNumber - 1) - OffsetToFirstDay;
            Monday = StartDate + new TimeSpan(OffsetToDemandedDay, 16, 0, 0);

            return Monday;
        }

        private DateTime GetWednesday(int WeekNumber)
        {
            DateTime Wednesday;

            DateTime StartDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 1, 1);
            int OffsetToFirstDay = 0;
            if (StartDate.DayOfWeek == DayOfWeek.Thursday)
                OffsetToFirstDay = 6;
            else
                OffsetToFirstDay = Convert.ToInt32(StartDate.DayOfWeek) - 3;
            int OffsetToDemandedDay = 7 * (WeekNumber - 1) - OffsetToFirstDay;
            Wednesday = StartDate + new TimeSpan(OffsetToDemandedDay, 0, 0, 0);

            return Wednesday;
        }

        private void MondayDecorProductsDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ConditionOrdersStatistics != null)
                if (MondayDecorProductsDG.SelectedRows.Count > 0)
                {
                    int ProductID = Convert.ToInt32(MondayDecorProductsDG.SelectedRows[0].Cells["ProductID"].Value);
                    int MeasureID = Convert.ToInt32(MondayDecorProductsDG.SelectedRows[0].Cells["MeasureID"].Value);

                    ((DataView)MondayDecorItemsDG.DataSource).RowFilter = "ProductID = " + ProductID + " AND MeasureID = " + MeasureID;
                }
        }

        private void WednesdayDecorProductsDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ConditionOrdersStatistics != null)
                if (WednesdayDecorProductsDG.SelectedRows.Count > 0)
                {
                    int ProductID = Convert.ToInt32(WednesdayDecorProductsDG.SelectedRows[0].Cells["ProductID"].Value);
                    int MeasureID = Convert.ToInt32(WednesdayDecorProductsDG.SelectedRows[0].Cells["MeasureID"].Value);

                    ((DataView)WednesdayDecorItemsDG.DataSource).RowFilter = "ProductID = " + ProductID + " AND MeasureID = " + MeasureID;
                }
        }

        private void FridayDecorProductsDG_SelectionChanged(object sender, EventArgs e)
        {
            if (ConditionOrdersStatistics != null)
                if (FridayDecorProductsDG.SelectedRows.Count > 0)
                {
                    int ProductID = Convert.ToInt32(FridayDecorProductsDG.SelectedRows[0].Cells["ProductID"].Value);
                    int MeasureID = Convert.ToInt32(FridayDecorProductsDG.SelectedRows[0].Cells["MeasureID"].Value);

                    ((DataView)FridayDecorItemsDG.DataSource).RowFilter = "ProductID = " + ProductID + " AND MeasureID = " + MeasureID;
                }
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            DateTime Monday = GetMonday(CurrentWeekNumber);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            MarketingBatchReport.CreateReport(((DataView)MondayFrontsDG.DataSource).Table,
                ((DataView)MondayDecorProductsDG.DataSource).Table, "Состояние заказов. " + Monday.ToString("dd MMMM yyyy"));

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void WednesdayFrontsDG_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu8.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void FridayFrontsDG_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void WeeksOfYearListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentWeekNumber = WeeksOfYearListBox.SelectedIndex + 1;

            DateTime Monday = GetMonday(CurrentWeekNumber);
            DateTime Wednesday = GetWednesday(CurrentWeekNumber);
            DateTime Friday = GetFriday(CurrentWeekNumber);

            FMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            FWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            FFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");
            DMondayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Понедельник " + Monday.ToString("dd.MM.yyyy HH:mm");
            DWednesdayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Среда " + Wednesday.ToString("dd.MM.yyyy HH:mm");
            DFridayLabel.Text = CurrentWeekNumber + " неделя\r\n" + "Пятница " + Friday.ToString("dd.MM.yyyy HH:mm");
        }

        private void cbxYears_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DateTime LastDay = new System.DateTime(Convert.ToInt32(cbxYears.SelectedValue), 12, 31);

            int WeeksCount = GetWeekNumber(LastDay);
            WeeksOfYearListBox.Items.Clear();
            for (int i = 1; i <= WeeksCount; i++)
            {
                WeeksOfYearListBox.Items.Add(i);
            }
            WeeksOfYearListBox.SelectedIndex = CurrentWeekNumber - 1;
        }

    }
}
