using Infinium.Modules.ZOV.ClientErrors;

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ReportCalculationsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm;

        ReportCalculations ReportCalculationsManager;

        public ReportCalculationsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }


        private void ClientErrorsWriteOffsForm_Shown(object sender, EventArgs e)
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
            ReportCalculationsManager = new ReportCalculations();
            ReportCalculationsManager.Initialize();
            ClientErrorsGridSetting();
            CalendarFrom.SelectionEnd = new DateTime(2014, 10, 28);
            CalendarTo.SelectionEnd = new DateTime(2014, 10, 28);
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

        private void ClientErrorsGridSetting()
        {
            dgvReportView.DataSource = ReportCalculationsManager.ReportViewList;

            foreach (DataGridViewColumn Column in dgvReportView.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            dgvReportView.Columns["DispatchDate"].DefaultCellStyle.Format = "dd.MM.yyyy";

            dgvReportView.Columns["DispatchDate"].HeaderText = "   ДАТА\r\nОТГРУЗКИ";
            dgvReportView.Columns["PaymentPlan"].HeaderText = "План расчет";
            dgvReportView.Columns["DispatchedCost"].HeaderText = "      Расчет\r\nотгруженного";
            dgvReportView.Columns["SummaryReport"].HeaderText = "Суммарный\r\n     отчет";
            dgvReportView.Columns["DebtsCost"].HeaderText = "Снято с расчета,\r\n        долги";
            dgvReportView.Columns["OtherCost"].HeaderText = "Снято с расчета,\r\n        другое";
            dgvReportView.Columns["NotDispatchedCost"].HeaderText = "ДОЛГИ";
            dgvReportView.Columns["ToAssemblyCost"].HeaderText = "На сборку";
            dgvReportView.Columns["FromAssemblyCost"].HeaderText = "Со сборки";
            dgvReportView.Columns["RefundsCost"].HeaderText = "Возврат";
            dgvReportView.Columns["ZOVReport"].HeaderText = "Отчет ЗОВ";
            dgvReportView.Columns["DeductionsCost"].HeaderText = "Минус от ОВ";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ",",
                NumberDecimalDigits = 1
            };
            dgvReportView.Columns["PaymentPlan"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["PaymentPlan"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["DispatchedCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["DispatchedCost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["SummaryReport"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["SummaryReport"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["DebtsCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["DebtsCost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["OtherCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["OtherCost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["NotDispatchedCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["NotDispatchedCost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["ToAssemblyCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["ToAssemblyCost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["FromAssemblyCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["FromAssemblyCost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["RefundsCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["RefundsCost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["ZOVReport"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["ZOVReport"].DefaultCellStyle.FormatProvider = nfi1;
            dgvReportView.Columns["DeductionsCost"].DefaultCellStyle.Format = "N";
            dgvReportView.Columns["DeductionsCost"].DefaultCellStyle.FormatProvider = nfi1;

            dgvReportView.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvReportView.Columns["DispatchDate"].MinimumWidth = 150;
            dgvReportView.Columns["PaymentPlan"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["PaymentPlan"].MinimumWidth = 50;
            dgvReportView.Columns["DispatchedCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["DispatchedCost"].MinimumWidth = 50;
            dgvReportView.Columns["SummaryReport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["SummaryReport"].MinimumWidth = 50;
            dgvReportView.Columns["DebtsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["DebtsCost"].MinimumWidth = 50;
            dgvReportView.Columns["OtherCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["OtherCost"].MinimumWidth = 50;
            dgvReportView.Columns["NotDispatchedCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["NotDispatchedCost"].MinimumWidth = 50;
            dgvReportView.Columns["ToAssemblyCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["ToAssemblyCost"].MinimumWidth = 50;
            dgvReportView.Columns["FromAssemblyCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["FromAssemblyCost"].MinimumWidth = 50;
            dgvReportView.Columns["RefundsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["RefundsCost"].MinimumWidth = 50;
            dgvReportView.Columns["ZOVReport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["ZOVReport"].MinimumWidth = 50;
            dgvReportView.Columns["DeductionsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvReportView.Columns["DeductionsCost"].MinimumWidth = 130;

            dgvReportView.Columns["ZOVReport"].ReadOnly = false;
            dgvReportView.Columns["DeductionsCost"].ReadOnly = false;

            dgvReportView.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvReportView.Columns["DispatchDate"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["PaymentPlan"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["DispatchedCost"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["SummaryReport"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["DebtsCost"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["OtherCost"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["NotDispatchedCost"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["ToAssemblyCost"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["FromAssemblyCost"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["RefundsCost"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["ZOVReport"].DisplayIndex = DisplayIndex++;
            dgvReportView.Columns["DeductionsCost"].DisplayIndex = DisplayIndex++;

            dgvReportView.Columns["PaymentPlan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["DispatchedCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["SummaryReport"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["DebtsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["OtherCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["NotDispatchedCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["ToAssemblyCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["FromAssemblyCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["RefundsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["ZOVReport"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReportView.Columns["DeductionsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime DateFrom = CalendarFrom.SelectionEnd;
            DateTime DateTo = CalendarTo.SelectionEnd;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ReportCalculationsManager.UpdateReportCalculations(DateFrom, DateTo);
            ReportCalculationsManager.UpdateData(DateFrom, DateTo);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnSaveReportCalculations_Click(object sender, EventArgs e)
        {
            ReportCalculationsManager.SaveReportCalculations();
            InfiniumTips.ShowTip(this, 50, 85, "Отчет сохранён", 1700);
        }

        private void btnCreateReport_Click(object sender, EventArgs e)
        {
            DateTime DateFrom = CalendarFrom.SelectionEnd;
            DateTime DateTo = CalendarTo.SelectionEnd;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ReportCalculationsManager.ReportToExcel(DateFrom, DateTo);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
