using Infinium.Modules.StaticticsZOV;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVDebtsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;

        int CalcWriteDebtType = -1;
        int DispatchType = -1;
        int WriteOffDebtType = -1;
        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        public Payments PaymentsManager;
        Modules.PaymentWeeks.ZOVDebts ZOVDebtsManager = null;

        CultureInfo CI = new System.Globalization.CultureInfo("ru-RU");
        NumberFormatInfo nfi1 = null;
        NumberFormatInfo nfi2 = null;

        public ZOVDebtsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void DispatchZOVDateForm_Shown(object sender, EventArgs e)
        {
            NeedSplash = true;
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
            nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ",",
                NumberDecimalDigits = 2
            };
            nfi2 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ",",
                NumberDecimalDigits = 1
            };
            ZOVDebtsManager = new Modules.PaymentWeeks.ZOVDebts();
            ZOVDebtsManager.Initialize();

            dgvDebtsDetails.DataSource = ZOVDebtsManager.DebtsList;
            dgvDoNotDispatchDetails.DataSource = ZOVDebtsManager.DoNotDispatchDetailsList;

            dgvGridSettings(ref dgvDebtsDetails);
            dgvGridSettings(ref dgvDoNotDispatchDetails);

            DateTime FirstDay = DateTime.Now.AddDays(-1);
            DateTime Today = DateTime.Now;

            PaymentsManager = new Payments(ref ResultTotalDataGrid, ref WriteOffDataGrid, ref CalcWriteOffDataGrid);
            PaymentsManager.CalcDebtsOnScaner(FirstDay, Today);

            PaymentSettings();
        }

        private void ShowCalcDebtsColumns(int DebtTypeID)
        {
            if (DebtTypeID == 1)
            {
                ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = true;
                ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage1"].Visible = true;
                ClientCalcDebtsDataGrid.Columns["Percentage2"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage3"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage4"].Visible = false;

                ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = true;
                ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].Visible = true;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].Visible = false;
            }
            if (DebtTypeID == 2)
            {
                ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = true;
                ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage1"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage2"].Visible = true;
                ClientCalcDebtsDataGrid.Columns["Percentage3"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage4"].Visible = false;

                ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = true;
                ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].Visible = true;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].Visible = false;
            }
            if (DebtTypeID == 3)
            {
                ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = true;
                ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage1"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage2"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage3"].Visible = true;
                ClientCalcDebtsDataGrid.Columns["Percentage4"].Visible = false;

                ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = true;
                ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].Visible = true;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].Visible = false;
            }
            if (DebtTypeID == 4)
            {
                ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = true;
                ClientCalcDebtsDataGrid.Columns["Percentage1"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage2"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage3"].Visible = false;
                ClientCalcDebtsDataGrid.Columns["Percentage4"].Visible = true;

                ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].Visible = true;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].Visible = false;
                ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].Visible = true;
            }
        }

        private void ShowWriteOffColumns(int DebtTypeID)
        {
            if (DebtTypeID == 1)
            {
                ClientWriteOffDataGrid.Columns["CalcDebtCost"].Visible = true;
                ClientWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage1"].Visible = true;
                ClientWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage5"].Visible = false;

                ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage1"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage5"].Visible = false;
            }
            if (DebtTypeID == 2)
            {
                ClientWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = true;
                ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage2"].Visible = true;
                ClientWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage5"].Visible = false;

                ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage2"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage5"].Visible = false;
            }
            if (DebtTypeID == 3)
            {
                ClientWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = true;
                ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage3"].Visible = true;
                ClientWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage5"].Visible = false;

                ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage3"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage5"].Visible = false;
            }
            if (DebtTypeID == 4)
            {
                ClientWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = true;
                ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage4"].Visible = true;
                ClientWriteOffDataGrid.Columns["Percentage5"].Visible = false;

                ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage4"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["Percentage5"].Visible = false;
            }
            if (DebtTypeID == 0)
            {
                ClientWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = true;
                ClientWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientWriteOffDataGrid.Columns["Percentage5"].Visible = true;

                ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].Visible = true;
                ClientGroupWriteOffDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage3"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage4"].Visible = false;
                ClientGroupWriteOffDataGrid.Columns["Percentage5"].Visible = true;
            }
        }

        private void ShowDispatchColumns(int DispatchID)
        {
            if (DispatchID == 1)
            {
                ClientDispatchDataGrid.Columns["DispatchCost"].Visible = true;
                ClientDispatchDataGrid.Columns["NotDispatchCost"].Visible = false;
                ClientDispatchDataGrid.Columns["SamplesCost"].Visible = false;
                ClientDispatchDataGrid.Columns["Percentage1"].Visible = true;
                ClientDispatchDataGrid.Columns["Percentage2"].Visible = false;
                ClientDispatchDataGrid.Columns["Percentage3"].Visible = false;

                ClientGroupDispatchDataGrid.Columns["DispatchCost"].Visible = true;
                ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["SamplesCost"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["Percentage1"].Visible = true;
                ClientGroupDispatchDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["Percentage3"].Visible = false;
            }
            if (DispatchID == 2)
            {
                ClientDispatchDataGrid.Columns["DispatchCost"].Visible = false;
                ClientDispatchDataGrid.Columns["NotDispatchCost"].Visible = true;
                ClientDispatchDataGrid.Columns["SamplesCost"].Visible = false;
                ClientDispatchDataGrid.Columns["Percentage1"].Visible = false;
                ClientDispatchDataGrid.Columns["Percentage2"].Visible = true;
                ClientDispatchDataGrid.Columns["Percentage3"].Visible = false;

                ClientGroupDispatchDataGrid.Columns["DispatchCost"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].Visible = true;
                ClientGroupDispatchDataGrid.Columns["SamplesCost"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["Percentage2"].Visible = true;
                ClientGroupDispatchDataGrid.Columns["Percentage3"].Visible = false;
            }
            if (DispatchID == 3)
            {
                ClientDispatchDataGrid.Columns["DispatchCost"].Visible = false;
                ClientDispatchDataGrid.Columns["NotDispatchCost"].Visible = false;
                ClientDispatchDataGrid.Columns["SamplesCost"].Visible = true;
                ClientDispatchDataGrid.Columns["Percentage1"].Visible = false;
                ClientDispatchDataGrid.Columns["Percentage2"].Visible = false;
                ClientDispatchDataGrid.Columns["Percentage3"].Visible = true;

                ClientGroupDispatchDataGrid.Columns["DispatchCost"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["SamplesCost"].Visible = true;
                ClientGroupDispatchDataGrid.Columns["Percentage1"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["Percentage2"].Visible = false;
                ClientGroupDispatchDataGrid.Columns["Percentage3"].Visible = true;
            }
        }

        private void ClientDebtsGridSettings()
        {
            foreach (DataGridViewColumn Column in ClientCalcDebtsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (ClientCalcDebtsDataGrid.Columns.Contains("ClientID"))
                ClientCalcDebtsDataGrid.Columns["ClientID"].Visible = false;
            if (ClientCalcDebtsDataGrid.Columns.Contains("ClientGroupID"))
                ClientCalcDebtsDataGrid.Columns["ClientGroupID"].Visible = false;
            if (ClientCalcDebtsDataGrid.Columns.Contains("DebtTypeID"))
                ClientCalcDebtsDataGrid.Columns["DebtTypeID"].Visible = false;

            ClientCalcDebtsDataGrid.Columns["Percentage1"].HeaderText = "%";
            ClientCalcDebtsDataGrid.Columns["Percentage2"].HeaderText = "%";
            ClientCalcDebtsDataGrid.Columns["Percentage3"].HeaderText = "%";
            ClientCalcDebtsDataGrid.Columns["Percentage4"].HeaderText = "%";
            ClientCalcDebtsDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].HeaderText = "Долги, €";
            ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].HeaderText = "Браки, €";
            ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].HeaderText = "Ошибки пр-ва, €";
            ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].HeaderText = "Ошибки ЗОВа, €";

            ClientCalcDebtsDataGrid.Columns["Percentage1"].MinimumWidth = 135;
            ClientCalcDebtsDataGrid.Columns["Percentage1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientCalcDebtsDataGrid.Columns["Percentage2"].MinimumWidth = 135;
            ClientCalcDebtsDataGrid.Columns["Percentage2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientCalcDebtsDataGrid.Columns["Percentage3"].MinimumWidth = 135;
            ClientCalcDebtsDataGrid.Columns["Percentage3"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientCalcDebtsDataGrid.Columns["Percentage4"].MinimumWidth = 135;
            ClientCalcDebtsDataGrid.Columns["Percentage4"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientCalcDebtsDataGrid.Columns["ClientName"].MinimumWidth = 150;
            ClientCalcDebtsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].MinimumWidth = 70;
            ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].MinimumWidth = 70;
            ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].MinimumWidth = 70;
            ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].MinimumWidth = 70;
            ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;

            ClientCalcDebtsDataGrid.Columns["Percentage1"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["Percentage1"].DefaultCellStyle.FormatProvider = nfi1;
            ClientCalcDebtsDataGrid.Columns["Percentage2"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["Percentage2"].DefaultCellStyle.FormatProvider = nfi1;
            ClientCalcDebtsDataGrid.Columns["Percentage3"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["Percentage3"].DefaultCellStyle.FormatProvider = nfi1;
            ClientCalcDebtsDataGrid.Columns["Percentage4"].DefaultCellStyle.Format = "N";
            ClientCalcDebtsDataGrid.Columns["Percentage4"].DefaultCellStyle.FormatProvider = nfi1;

            ClientCalcDebtsDataGrid.AutoGenerateColumns = false;
            ClientCalcDebtsDataGrid.Columns["ClientName"].DisplayIndex = 1;
            ClientCalcDebtsDataGrid.Columns["CalcDebtCost"].DisplayIndex = 2;
            ClientCalcDebtsDataGrid.Columns["CalcDefectsCost"].DisplayIndex = 3;
            ClientCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].DisplayIndex = 4;
            ClientCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].DisplayIndex = 5;
            ClientCalcDebtsDataGrid.Columns["Percentage1"].DisplayIndex = 7;
            ClientCalcDebtsDataGrid.Columns["Percentage2"].DisplayIndex = 8;
            ClientCalcDebtsDataGrid.Columns["Percentage3"].DisplayIndex = 9;
            ClientCalcDebtsDataGrid.Columns["Percentage4"].DisplayIndex = 10;

            ClientCalcDebtsDataGrid.Columns["Percentage1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientCalcDebtsDataGrid.AddPercentageColumn("Percentage1");
            ClientCalcDebtsDataGrid.Columns["Percentage2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientCalcDebtsDataGrid.AddPercentageColumn("Percentage2");
            ClientCalcDebtsDataGrid.Columns["Percentage3"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientCalcDebtsDataGrid.AddPercentageColumn("Percentage3");
            ClientCalcDebtsDataGrid.Columns["Percentage4"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientCalcDebtsDataGrid.AddPercentageColumn("Percentage4");
        }

        private void ClientsGroupDebtsGridSettings()
        {
            foreach (DataGridViewColumn Column in ClientGroupCalcDebtsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (ClientGroupCalcDebtsDataGrid.Columns.Contains("ClientGroupID"))
                ClientGroupCalcDebtsDataGrid.Columns["ClientGroupID"].Visible = false;
            if (ClientGroupCalcDebtsDataGrid.Columns.Contains("DebtTypeID"))
                ClientGroupCalcDebtsDataGrid.Columns["DebtTypeID"].Visible = false;

            ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].HeaderText = "%";
            ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].HeaderText = "%";
            ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].HeaderText = "%";
            ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].HeaderText = "%";
            ClientGroupCalcDebtsDataGrid.Columns["ClientGroupName"].HeaderText = "Группа";
            ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].HeaderText = "Долги, €";
            ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].HeaderText = "Браки, €";
            ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].HeaderText = "Ошибки пр-ва, €";
            ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].HeaderText = "Ошибки ЗОВа, €";

            ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].MinimumWidth = 135;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].MinimumWidth = 135;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].MinimumWidth = 135;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].MinimumWidth = 135;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupCalcDebtsDataGrid.Columns["ClientGroupName"].MinimumWidth = 150;
            ClientGroupCalcDebtsDataGrid.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].MinimumWidth = 70;
            ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].MinimumWidth = 70;
            ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].MinimumWidth = 70;
            ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].MinimumWidth = 70;
            ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;

            ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].DefaultCellStyle.Format = "N";
            ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].DefaultCellStyle.FormatProvider = nfi1;

            ClientGroupCalcDebtsDataGrid.AutoGenerateColumns = false;
            ClientGroupCalcDebtsDataGrid.Columns["ClientGroupName"].DisplayIndex = 1;
            ClientGroupCalcDebtsDataGrid.Columns["CalcDebtCost"].DisplayIndex = 2;
            ClientGroupCalcDebtsDataGrid.Columns["CalcDefectsCost"].DisplayIndex = 3;
            ClientGroupCalcDebtsDataGrid.Columns["CalcProductionErrorsCost"].DisplayIndex = 4;
            ClientGroupCalcDebtsDataGrid.Columns["CalcZOVErrorsCost"].DisplayIndex = 5;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].DisplayIndex = 7;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].DisplayIndex = 8;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].DisplayIndex = 9;
            ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].DisplayIndex = 10;

            ClientGroupCalcDebtsDataGrid.Columns["Percentage1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupCalcDebtsDataGrid.AddPercentageColumn("Percentage1");
            ClientGroupCalcDebtsDataGrid.Columns["Percentage2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupCalcDebtsDataGrid.AddPercentageColumn("Percentage2");
            ClientGroupCalcDebtsDataGrid.Columns["Percentage3"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupCalcDebtsDataGrid.AddPercentageColumn("Percentage3");
            ClientGroupCalcDebtsDataGrid.Columns["Percentage4"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupCalcDebtsDataGrid.AddPercentageColumn("Percentage4");
        }

        private void ClientWriteOffGridSettings()
        {
            foreach (DataGridViewColumn Column in ClientWriteOffDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (ClientWriteOffDataGrid.Columns.Contains("ClientID"))
                ClientWriteOffDataGrid.Columns["ClientID"].Visible = false;
            if (ClientWriteOffDataGrid.Columns.Contains("ClientGroupID"))
                ClientWriteOffDataGrid.Columns["ClientGroupID"].Visible = false;
            if (ClientWriteOffDataGrid.Columns.Contains("DebtTypeID"))
                ClientWriteOffDataGrid.Columns["DebtTypeID"].Visible = false;

            ClientWriteOffDataGrid.Columns["Percentage1"].HeaderText = "%";
            ClientWriteOffDataGrid.Columns["Percentage2"].HeaderText = "%";
            ClientWriteOffDataGrid.Columns["Percentage3"].HeaderText = "%";
            ClientWriteOffDataGrid.Columns["Percentage4"].HeaderText = "%";
            ClientWriteOffDataGrid.Columns["Percentage5"].HeaderText = "%";
            ClientWriteOffDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            ClientWriteOffDataGrid.Columns["CalcDebtCost"].HeaderText = "Долги, €";
            ClientWriteOffDataGrid.Columns["CalcDefectsCost"].HeaderText = "Браки, €";
            ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].HeaderText = "Ошибки пр-ва, €";
            ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].HeaderText = "Ошибки ЗОВа, €";
            ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "Образцы, €";

            ClientWriteOffDataGrid.Columns["Percentage1"].MinimumWidth = 135;
            ClientWriteOffDataGrid.Columns["Percentage1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientWriteOffDataGrid.Columns["Percentage2"].MinimumWidth = 135;
            ClientWriteOffDataGrid.Columns["Percentage2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientWriteOffDataGrid.Columns["Percentage3"].MinimumWidth = 135;
            ClientWriteOffDataGrid.Columns["Percentage3"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientWriteOffDataGrid.Columns["Percentage4"].MinimumWidth = 135;
            ClientWriteOffDataGrid.Columns["Percentage4"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientWriteOffDataGrid.Columns["Percentage5"].MinimumWidth = 135;
            ClientWriteOffDataGrid.Columns["Percentage5"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientWriteOffDataGrid.Columns["ClientName"].MinimumWidth = 150;
            ClientWriteOffDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientWriteOffDataGrid.Columns["CalcDebtCost"].MinimumWidth = 70;
            ClientWriteOffDataGrid.Columns["CalcDebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientWriteOffDataGrid.Columns["CalcDefectsCost"].MinimumWidth = 70;
            ClientWriteOffDataGrid.Columns["CalcDefectsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].MinimumWidth = 70;
            ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].MinimumWidth = 70;
            ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].MinimumWidth = 70;
            ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ClientWriteOffDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientWriteOffDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.FormatProvider = nfi2;

            ClientWriteOffDataGrid.Columns["Percentage1"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["Percentage1"].DefaultCellStyle.FormatProvider = nfi1;
            ClientWriteOffDataGrid.Columns["Percentage2"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["Percentage2"].DefaultCellStyle.FormatProvider = nfi1;
            ClientWriteOffDataGrid.Columns["Percentage3"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["Percentage3"].DefaultCellStyle.FormatProvider = nfi1;
            ClientWriteOffDataGrid.Columns["Percentage4"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["Percentage4"].DefaultCellStyle.FormatProvider = nfi1;
            ClientWriteOffDataGrid.Columns["Percentage5"].DefaultCellStyle.Format = "N";
            ClientWriteOffDataGrid.Columns["Percentage5"].DefaultCellStyle.FormatProvider = nfi1;

            ClientWriteOffDataGrid.AutoGenerateColumns = false;
            ClientWriteOffDataGrid.Columns["ClientName"].DisplayIndex = 1;
            ClientWriteOffDataGrid.Columns["CalcDebtCost"].DisplayIndex = 2;
            ClientWriteOffDataGrid.Columns["CalcDefectsCost"].DisplayIndex = 3;
            ClientWriteOffDataGrid.Columns["CalcProductionErrorsCost"].DisplayIndex = 4;
            ClientWriteOffDataGrid.Columns["CalcZOVErrorsCost"].DisplayIndex = 5;
            ClientWriteOffDataGrid.Columns["SamplesWriteOffCost"].DisplayIndex = 6;
            ClientWriteOffDataGrid.Columns["Percentage1"].DisplayIndex = 7;
            ClientWriteOffDataGrid.Columns["Percentage2"].DisplayIndex = 8;
            ClientWriteOffDataGrid.Columns["Percentage3"].DisplayIndex = 9;
            ClientWriteOffDataGrid.Columns["Percentage4"].DisplayIndex = 10;
            ClientWriteOffDataGrid.Columns["Percentage5"].DisplayIndex = 11;

            ClientWriteOffDataGrid.Columns["Percentage1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientWriteOffDataGrid.AddPercentageColumn("Percentage1");
            ClientWriteOffDataGrid.Columns["Percentage2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientWriteOffDataGrid.AddPercentageColumn("Percentage2");
            ClientWriteOffDataGrid.Columns["Percentage3"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientWriteOffDataGrid.AddPercentageColumn("Percentage3");
            ClientWriteOffDataGrid.Columns["Percentage4"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientWriteOffDataGrid.AddPercentageColumn("Percentage4");
            ClientWriteOffDataGrid.Columns["Percentage5"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientWriteOffDataGrid.AddPercentageColumn("Percentage5");
        }

        private void ClientsGroupWriteOffGridSettings()
        {
            foreach (DataGridViewColumn Column in ClientGroupWriteOffDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (ClientGroupWriteOffDataGrid.Columns.Contains("ClientGroupID"))
                ClientGroupWriteOffDataGrid.Columns["ClientGroupID"].Visible = false;
            if (ClientGroupWriteOffDataGrid.Columns.Contains("DebtTypeID"))
                ClientGroupWriteOffDataGrid.Columns["DebtTypeID"].Visible = false;

            ClientGroupWriteOffDataGrid.Columns["Percentage1"].HeaderText = "%";
            ClientGroupWriteOffDataGrid.Columns["Percentage2"].HeaderText = "%";
            ClientGroupWriteOffDataGrid.Columns["Percentage3"].HeaderText = "%";
            ClientGroupWriteOffDataGrid.Columns["Percentage4"].HeaderText = "%";
            ClientGroupWriteOffDataGrid.Columns["Percentage5"].HeaderText = "%";
            ClientGroupWriteOffDataGrid.Columns["ClientGroupName"].HeaderText = "Группа";
            ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].HeaderText = "Долги, €";
            ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].HeaderText = "Браки, €";
            ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].HeaderText = "Ошибки пр-ва, €";
            ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].HeaderText = "Ошибки ЗОВа, €";
            ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "Образцы, €";

            ClientGroupWriteOffDataGrid.Columns["Percentage1"].MinimumWidth = 135;
            ClientGroupWriteOffDataGrid.Columns["Percentage1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupWriteOffDataGrid.Columns["Percentage2"].MinimumWidth = 135;
            ClientGroupWriteOffDataGrid.Columns["Percentage2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupWriteOffDataGrid.Columns["Percentage3"].MinimumWidth = 135;
            ClientGroupWriteOffDataGrid.Columns["Percentage3"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupWriteOffDataGrid.Columns["Percentage4"].MinimumWidth = 135;
            ClientGroupWriteOffDataGrid.Columns["Percentage4"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupWriteOffDataGrid.Columns["Percentage5"].MinimumWidth = 135;
            ClientGroupWriteOffDataGrid.Columns["Percentage5"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupWriteOffDataGrid.Columns["ClientGroupName"].MinimumWidth = 150;
            ClientGroupWriteOffDataGrid.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].MinimumWidth = 70;
            ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].MinimumWidth = 70;
            ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].MinimumWidth = 70;
            ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].MinimumWidth = 70;
            ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].MinimumWidth = 70;
            ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.FormatProvider = nfi2;

            ClientGroupWriteOffDataGrid.Columns["Percentage1"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["Percentage1"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupWriteOffDataGrid.Columns["Percentage2"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["Percentage2"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupWriteOffDataGrid.Columns["Percentage3"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["Percentage3"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupWriteOffDataGrid.Columns["Percentage4"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["Percentage4"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupWriteOffDataGrid.Columns["Percentage5"].DefaultCellStyle.Format = "N";
            ClientGroupWriteOffDataGrid.Columns["Percentage5"].DefaultCellStyle.FormatProvider = nfi1;

            ClientGroupWriteOffDataGrid.AutoGenerateColumns = false;
            ClientGroupWriteOffDataGrid.Columns["ClientGroupName"].DisplayIndex = 1;
            ClientGroupWriteOffDataGrid.Columns["CalcDebtCost"].DisplayIndex = 2;
            ClientGroupWriteOffDataGrid.Columns["CalcDefectsCost"].DisplayIndex = 3;
            ClientGroupWriteOffDataGrid.Columns["CalcProductionErrorsCost"].DisplayIndex = 4;
            ClientGroupWriteOffDataGrid.Columns["CalcZOVErrorsCost"].DisplayIndex = 5;
            ClientGroupWriteOffDataGrid.Columns["SamplesWriteOffCost"].DisplayIndex = 6;
            ClientGroupWriteOffDataGrid.Columns["Percentage1"].DisplayIndex = 7;
            ClientGroupWriteOffDataGrid.Columns["Percentage2"].DisplayIndex = 8;
            ClientGroupWriteOffDataGrid.Columns["Percentage3"].DisplayIndex = 9;
            ClientGroupWriteOffDataGrid.Columns["Percentage4"].DisplayIndex = 10;
            ClientGroupWriteOffDataGrid.Columns["Percentage5"].DisplayIndex = 11;

            ClientGroupWriteOffDataGrid.Columns["Percentage1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupWriteOffDataGrid.AddPercentageColumn("Percentage1");
            ClientGroupWriteOffDataGrid.Columns["Percentage2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupWriteOffDataGrid.AddPercentageColumn("Percentage2");
            ClientGroupWriteOffDataGrid.Columns["Percentage3"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupWriteOffDataGrid.AddPercentageColumn("Percentage3");
            ClientGroupWriteOffDataGrid.Columns["Percentage4"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupWriteOffDataGrid.AddPercentageColumn("Percentage4");
            ClientGroupWriteOffDataGrid.Columns["Percentage5"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupWriteOffDataGrid.AddPercentageColumn("Percentage5");
        }

        private void ClientDispatchGridSettings()
        {
            foreach (DataGridViewColumn Column in ClientDispatchDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (ClientDispatchDataGrid.Columns.Contains("ClientID"))
                ClientDispatchDataGrid.Columns["ClientID"].Visible = false;
            if (ClientDispatchDataGrid.Columns.Contains("ClientGroupID"))
                ClientDispatchDataGrid.Columns["ClientGroupID"].Visible = false;
            if (ClientDispatchDataGrid.Columns.Contains("DispatchID"))
                ClientDispatchDataGrid.Columns["DispatchID"].Visible = false;

            ClientDispatchDataGrid.Columns["Percentage1"].HeaderText = "%";
            ClientDispatchDataGrid.Columns["Percentage2"].HeaderText = "%";
            ClientDispatchDataGrid.Columns["Percentage3"].HeaderText = "%";
            ClientDispatchDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            ClientDispatchDataGrid.Columns["DispatchCost"].HeaderText = "Отгружено, €";
            ClientDispatchDataGrid.Columns["NotDispatchCost"].HeaderText = "Не отгружено, €";
            ClientDispatchDataGrid.Columns["SamplesCost"].HeaderText = "Образцы, €";

            ClientDispatchDataGrid.Columns["Percentage1"].MinimumWidth = 135;
            ClientDispatchDataGrid.Columns["Percentage1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientDispatchDataGrid.Columns["Percentage2"].MinimumWidth = 135;
            ClientDispatchDataGrid.Columns["Percentage2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientDispatchDataGrid.Columns["ClientName"].MinimumWidth = 150;
            ClientDispatchDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientDispatchDataGrid.Columns["DispatchCost"].MinimumWidth = 70;
            ClientDispatchDataGrid.Columns["DispatchCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientDispatchDataGrid.Columns["NotDispatchCost"].MinimumWidth = 70;
            ClientDispatchDataGrid.Columns["NotDispatchCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientDispatchDataGrid.Columns["SamplesCost"].MinimumWidth = 70;
            ClientDispatchDataGrid.Columns["SamplesCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ClientDispatchDataGrid.Columns["DispatchCost"].DefaultCellStyle.Format = "N";
            ClientDispatchDataGrid.Columns["DispatchCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientDispatchDataGrid.Columns["NotDispatchCost"].DefaultCellStyle.Format = "N";
            ClientDispatchDataGrid.Columns["NotDispatchCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientDispatchDataGrid.Columns["NotDispatchCost"].DefaultCellStyle.Format = "N";
            ClientDispatchDataGrid.Columns["NotDispatchCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientDispatchDataGrid.Columns["SamplesCost"].DefaultCellStyle.Format = "N";
            ClientDispatchDataGrid.Columns["SamplesCost"].DefaultCellStyle.FormatProvider = nfi2;

            ClientDispatchDataGrid.Columns["Percentage1"].DefaultCellStyle.Format = "N";
            ClientDispatchDataGrid.Columns["Percentage1"].DefaultCellStyle.FormatProvider = nfi1;
            ClientDispatchDataGrid.Columns["Percentage2"].DefaultCellStyle.Format = "N";
            ClientDispatchDataGrid.Columns["Percentage2"].DefaultCellStyle.FormatProvider = nfi1;
            ClientDispatchDataGrid.Columns["Percentage3"].DefaultCellStyle.Format = "N";
            ClientDispatchDataGrid.Columns["Percentage3"].DefaultCellStyle.FormatProvider = nfi1;

            ClientDispatchDataGrid.AutoGenerateColumns = false;
            ClientDispatchDataGrid.Columns["ClientName"].DisplayIndex = 1;
            ClientDispatchDataGrid.Columns["DispatchCost"].DisplayIndex = 2;
            ClientDispatchDataGrid.Columns["NotDispatchCost"].DisplayIndex = 3;
            ClientDispatchDataGrid.Columns["SamplesCost"].DisplayIndex = 4;
            ClientDispatchDataGrid.Columns["Percentage1"].DisplayIndex = 5;
            ClientDispatchDataGrid.Columns["Percentage2"].DisplayIndex = 6;
            ClientDispatchDataGrid.Columns["Percentage3"].DisplayIndex = 7;

            ClientDispatchDataGrid.Columns["Percentage1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientDispatchDataGrid.AddPercentageColumn("Percentage1");
            ClientDispatchDataGrid.Columns["Percentage2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientDispatchDataGrid.AddPercentageColumn("Percentage2");
            ClientDispatchDataGrid.Columns["Percentage3"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientDispatchDataGrid.AddPercentageColumn("Percentage3");
        }

        private void ClientsGroupDispatchGridSettings()
        {
            foreach (DataGridViewColumn Column in ClientGroupDispatchDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (ClientGroupDispatchDataGrid.Columns.Contains("ClientGroupID"))
                ClientGroupDispatchDataGrid.Columns["ClientGroupID"].Visible = false;
            if (ClientGroupDispatchDataGrid.Columns.Contains("DispatchID"))
                ClientGroupDispatchDataGrid.Columns["DispatchID"].Visible = false;

            ClientGroupDispatchDataGrid.Columns["Percentage1"].HeaderText = "%";
            ClientGroupDispatchDataGrid.Columns["Percentage2"].HeaderText = "%";
            ClientGroupDispatchDataGrid.Columns["Percentage3"].HeaderText = "%";
            ClientGroupDispatchDataGrid.Columns["ClientGroupName"].HeaderText = "Группа";
            ClientGroupDispatchDataGrid.Columns["DispatchCost"].HeaderText = "Отгружено, €";
            ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].HeaderText = "Не отгружено, €";
            ClientGroupDispatchDataGrid.Columns["SamplesCost"].HeaderText = "Образцы, €";

            ClientGroupDispatchDataGrid.Columns["Percentage1"].MinimumWidth = 135;
            ClientGroupDispatchDataGrid.Columns["Percentage1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupDispatchDataGrid.Columns["Percentage2"].MinimumWidth = 135;
            ClientGroupDispatchDataGrid.Columns["Percentage2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupDispatchDataGrid.Columns["Percentage2"].MinimumWidth = 135;
            ClientGroupDispatchDataGrid.Columns["Percentage2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGroupDispatchDataGrid.Columns["ClientGroupName"].MinimumWidth = 150;
            ClientGroupDispatchDataGrid.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupDispatchDataGrid.Columns["DispatchCost"].MinimumWidth = 70;
            ClientGroupDispatchDataGrid.Columns["DispatchCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].MinimumWidth = 70;
            ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientGroupDispatchDataGrid.Columns["SamplesCost"].MinimumWidth = 70;
            ClientGroupDispatchDataGrid.Columns["SamplesCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ClientGroupDispatchDataGrid.Columns["DispatchCost"].DefaultCellStyle.Format = "N";
            ClientGroupDispatchDataGrid.Columns["DispatchCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].DefaultCellStyle.Format = "N";
            ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].DefaultCellStyle.FormatProvider = nfi2;
            ClientGroupDispatchDataGrid.Columns["SamplesCost"].DefaultCellStyle.Format = "N";
            ClientGroupDispatchDataGrid.Columns["SamplesCost"].DefaultCellStyle.FormatProvider = nfi2;

            ClientGroupDispatchDataGrid.Columns["Percentage1"].DefaultCellStyle.Format = "N";
            ClientGroupDispatchDataGrid.Columns["Percentage1"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupDispatchDataGrid.Columns["Percentage2"].DefaultCellStyle.Format = "N";
            ClientGroupDispatchDataGrid.Columns["Percentage2"].DefaultCellStyle.FormatProvider = nfi1;
            ClientGroupDispatchDataGrid.Columns["Percentage3"].DefaultCellStyle.Format = "N";
            ClientGroupDispatchDataGrid.Columns["Percentage3"].DefaultCellStyle.FormatProvider = nfi1;

            ClientGroupDispatchDataGrid.AutoGenerateColumns = false;
            ClientGroupDispatchDataGrid.Columns["ClientGroupName"].DisplayIndex = 1;
            ClientGroupDispatchDataGrid.Columns["DispatchCost"].DisplayIndex = 2;
            ClientGroupDispatchDataGrid.Columns["NotDispatchCost"].DisplayIndex = 3;
            ClientGroupDispatchDataGrid.Columns["SamplesCost"].DisplayIndex = 4;
            ClientGroupDispatchDataGrid.Columns["Percentage1"].DisplayIndex = 5;
            ClientGroupDispatchDataGrid.Columns["Percentage2"].DisplayIndex = 6;
            ClientGroupDispatchDataGrid.Columns["Percentage3"].DisplayIndex = 7;

            ClientGroupDispatchDataGrid.Columns["Percentage1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupDispatchDataGrid.AddPercentageColumn("Percentage1");
            ClientGroupDispatchDataGrid.Columns["Percentage2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupDispatchDataGrid.AddPercentageColumn("Percentage2");
            ClientGroupDispatchDataGrid.Columns["Percentage3"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ClientGroupDispatchDataGrid.AddPercentageColumn("Percentage3");
        }

        private void FilterClientsGridSettings()
        {
            foreach (DataGridViewColumn Column in FilterClientsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (FilterClientsDataGrid.Columns.Contains("ClientID"))
                FilterClientsDataGrid.Columns["ClientID"].Visible = false;
            if (FilterClientsDataGrid.Columns.Contains("ClientGroupID"))
                FilterClientsDataGrid.Columns["ClientGroupID"].Visible = false;
            if (FilterClientsDataGrid.Columns.Contains("ManagerID"))
                FilterClientsDataGrid.Columns["ManagerID"].Visible = false;
            if (FilterClientsDataGrid.Columns.Contains("MoveOk"))
                FilterClientsDataGrid.Columns["MoveOk"].Visible = false;

            FilterClientsDataGrid.Columns["Checked"].ReadOnly = false;
            FilterClientsDataGrid.Columns["Checked"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FilterClientsDataGrid.Columns["Checked"].HeaderText = "Выбрать";
            FilterClientsDataGrid.Columns["ClientName"].MinimumWidth = 150;
            FilterClientsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FilterClientsDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            FilterClientsDataGrid.AutoGenerateColumns = false;
            FilterClientsDataGrid.Columns["Checked"].DisplayIndex = 1;
            FilterClientsDataGrid.Columns["ClientName"].DisplayIndex = 2;
        }

        private void FilterGroupsGridSettings()
        {
            foreach (DataGridViewColumn Column in FilterGroupsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (FilterClientsDataGrid.Columns.Contains("ClientGroupID"))
                FilterGroupsDataGrid.Columns["ClientGroupID"].Visible = false;

            FilterGroupsDataGrid.Columns["Checked"].ReadOnly = false;
            FilterGroupsDataGrid.Columns["Checked"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FilterGroupsDataGrid.Columns["Checked"].HeaderText = "Выбрать";
            FilterGroupsDataGrid.Columns["ClientGroupName"].MinimumWidth = 150;
            FilterGroupsDataGrid.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FilterGroupsDataGrid.Columns["ClientGroupName"].HeaderText = "Группа";
            FilterGroupsDataGrid.AutoGenerateColumns = false;
            FilterGroupsDataGrid.Columns["Checked"].DisplayIndex = 1;
            FilterGroupsDataGrid.Columns["ClientGroupName"].DisplayIndex = 2;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            DateTime FirstDay = ClientsCalendarFrom.SelectionStart;
            DateTime Today = ClientsCalendarTo.SelectionStart;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                PaymentsManager.GetCheckedClients();

                PaymentsManager.GetCheckedGroups();

                PaymentsManager.CalcDebtsOnScaner(FirstDay, Today);

                PaymentsManager.GetClientGroupCalcDebts(FirstDay, Today);
                PaymentsManager.GetClientGroupWriteOff(FirstDay, Today);
                PaymentsManager.GetClientGroupDispatch(FirstDay, Today);

                PaymentsManager.GetClientCalcDebts(FirstDay, Today);
                PaymentsManager.GetClientWriteOff(FirstDay, Today);
                PaymentsManager.GetClientDispatch(FirstDay, Today);

                PaymentsManager.GetClientGroupSamples(FirstDay, Today);
                PaymentsManager.GetClientSamples(FirstDay, Today);

                ResultLabel.Text = PaymentsManager.ResultText;
                CalcWriteOffResultLabel.Text = PaymentsManager.CalcWriteOffResult;
                WriteOffResultLabel.Text = PaymentsManager.WriteOffResult;

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                PaymentsManager.GetCheckedClients();
                PaymentsManager.GetCheckedGroups();

                PaymentsManager.CalcDebtsOnScaner(FirstDay, Today);

                ResultLabel.Text = PaymentsManager.ResultText;
                CalcWriteOffResultLabel.Text = PaymentsManager.CalcWriteOffResult;
                WriteOffResultLabel.Text = PaymentsManager.WriteOffResult;
            }
        }

        private void FilterGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PaymentsManager == null)
                return;

            PaymentsManager.GetCurrentGroup();
            PaymentsManager.FilterClientsByGroup();
        }

        private void FilterGroupsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (FilterGroupsDataGrid.Columns[e.ColumnIndex].Name == "Checked" && e.RowIndex != -1)
            {
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)FilterGroupsDataGrid.
                    Rows[e.RowIndex].Cells["Checked"];

                bool Checked = Convert.ToBoolean(checkCell.Value);

                if (NeedSplash)
                {
                    Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                    T.Start();
                    while (!SplashWindow.bSmallCreated) ;
                    NeedSplash = false;

                    PaymentsManager.SetCheckClients(Checked);
                    FilterGroupsDataGrid.Invalidate();

                    NeedSplash = true;
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                }
                else
                {
                    PaymentsManager.SetCheckClients(Checked);
                    FilterGroupsDataGrid.Invalidate();
                }
            }
        }

        private void CheckAllClientsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            PaymentsManager.CheckAllClients(CheckAllClientsCheckBox.Checked);
            PaymentsManager.CheckAllGroups(CheckAllClientsCheckBox.Checked);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void CalcWriteOffDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CalcWriteDebtType = Convert.ToInt32(CalcWriteOffDataGrid.SelectedRows[0].Cells["DebtTypeID"].Value);

            ShowCalcDebtsColumns(CalcWriteDebtType);

            PaymentsManager.FilterClientGroupDebts(CalcWriteDebtType);

            popupControlContainer1.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void ClientGroupCalcDebtsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PaymentsManager != null)
                if (PaymentsManager.ClientsGroupCalcDebtsBS.Count > 0)
                {
                    if (((DataRowView)PaymentsManager.ClientsGroupCalcDebtsBS.Current)["ClientGroupID"] != DBNull.Value)
                    {
                        PaymentsManager.FilterClientDebts(Convert.ToInt32(((DataRowView)PaymentsManager.ClientsGroupCalcDebtsBS.Current).Row["ClientGroupID"]),
                            CalcWriteDebtType);
                    }
                }
        }

        private void ClientGroupWriteOffDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PaymentsManager != null)
                if (PaymentsManager.ClientsGroupWriteOffBS.Count > 0)
                {
                    if (((DataRowView)PaymentsManager.ClientsGroupWriteOffBS.Current)["ClientGroupID"] != DBNull.Value)
                    {
                        PaymentsManager.FilterClientWriteOff(Convert.ToInt32(((DataRowView)PaymentsManager.ClientsGroupWriteOffBS.Current).Row["ClientGroupID"]),
                            WriteOffDebtType);
                    }
                }
        }

        private void WriteOffDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            WriteOffDebtType = Convert.ToInt32(WriteOffDataGrid.SelectedRows[0].Cells["DebtTypeID"].Value);

            ShowWriteOffColumns(WriteOffDebtType);

            PaymentsManager.FilterClientGroupWriteOff(WriteOffDebtType);

            popupControlContainer2.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer2.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer2.Height / 2));
        }

        private void ClientGroupDispatchDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PaymentsManager != null)
                if (PaymentsManager.ClientsGroupDispatchBS.Count > 0)
                {
                    if (((DataRowView)PaymentsManager.ClientsGroupDispatchBS.Current)["ClientGroupID"] != DBNull.Value)
                    {
                        PaymentsManager.FilterClientDispatchID(Convert.ToInt32(((DataRowView)PaymentsManager.ClientsGroupDispatchBS.Current).Row["ClientGroupID"]),
                            DispatchType);
                    }
                }
        }

        private void ResultTotalDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DispatchType = Convert.ToInt32(ResultTotalDataGrid.SelectedRows[0].Cells["DispatchID"].Value);
            if (DispatchType == 0)
                return;
            ShowDispatchColumns(DispatchType);

            PaymentsManager.FilterClientGroupDispatch(DispatchType);

            //panel55.Visible = true;
            //panel55.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
            //    Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2);
            popupControlContainer3.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer3.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer3.Height / 2));
        }

        private void ResultTotalDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void CalcWriteOffDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void WriteOffDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void TotalDispatch_Click(object sender, EventArgs e)
        {
            DispatchType = 1;
            ShowDispatchColumns(DispatchType);
            PaymentsManager.FilterClientGroupDispatch(DispatchType);
            popupControlContainer3.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void TotalNotDispatch_Click(object sender, EventArgs e)
        {
            DispatchType = 2;
            ShowDispatchColumns(DispatchType);
            PaymentsManager.FilterClientGroupDispatch(DispatchType);
            popupControlContainer3.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void TotalSamples_Click(object sender, EventArgs e)
        {
            DispatchType = 3;
            ShowDispatchColumns(DispatchType);
            PaymentsManager.FilterClientGroupDispatch(DispatchType);
            popupControlContainer3.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void CalcWriteDebts_Click(object sender, EventArgs e)
        {
            CalcWriteDebtType = 1;
            ShowCalcDebtsColumns(CalcWriteDebtType);
            PaymentsManager.FilterClientGroupDebts(CalcWriteDebtType);
            popupControlContainer1.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void CalcWriteDefects_Click(object sender, EventArgs e)
        {
            CalcWriteDebtType = 2;
            ShowCalcDebtsColumns(CalcWriteDebtType);
            PaymentsManager.FilterClientGroupDebts(CalcWriteDebtType);
            popupControlContainer1.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void CalcWriteProductionErrors_Click(object sender, EventArgs e)
        {
            CalcWriteDebtType = 3;
            ShowCalcDebtsColumns(CalcWriteDebtType);
            PaymentsManager.FilterClientGroupDebts(CalcWriteDebtType);
            popupControlContainer1.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void CalcWriteZOVErrors_Click(object sender, EventArgs e)
        {
            CalcWriteDebtType = 4;
            ShowCalcDebtsColumns(CalcWriteDebtType);
            PaymentsManager.FilterClientGroupDebts(CalcWriteDebtType);
            popupControlContainer1.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void WriteOffDebts_Click(object sender, EventArgs e)
        {
            WriteOffDebtType = 1;
            ShowWriteOffColumns(WriteOffDebtType);
            PaymentsManager.FilterClientGroupWriteOff(WriteOffDebtType);
            popupControlContainer2.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void WriteOffDefects_Click(object sender, EventArgs e)
        {
            WriteOffDebtType = 2;
            ShowWriteOffColumns(WriteOffDebtType);
            PaymentsManager.FilterClientGroupWriteOff(WriteOffDebtType);
            popupControlContainer2.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void WriteOffProductionErrors_Click(object sender, EventArgs e)
        {
            WriteOffDebtType = 3;
            ShowWriteOffColumns(WriteOffDebtType);
            PaymentsManager.FilterClientGroupWriteOff(WriteOffDebtType);
            popupControlContainer2.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void WriteOffZOVErrors_Click(object sender, EventArgs e)
        {
            WriteOffDebtType = 4;
            ShowWriteOffColumns(WriteOffDebtType);
            PaymentsManager.FilterClientGroupWriteOff(WriteOffDebtType);
            popupControlContainer2.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void WriteOffSamples_Click(object sender, EventArgs e)
        {
            WriteOffDebtType = 0;
            ShowWriteOffColumns(WriteOffDebtType);
            PaymentsManager.FilterClientGroupWriteOff(WriteOffDebtType);
            popupControlContainer2.ShowPopup(barManager1, new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 - popupControlContainer1.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - popupControlContainer1.Height / 2));
        }

        private void PaymentSettings()
        {
            FilterClientsDataGrid.DataSource = PaymentsManager.ClientsBS;
            FilterGroupsDataGrid.DataSource = PaymentsManager.ClientsGroupsBS;
            ClientGroupCalcDebtsDataGrid.DataSource = PaymentsManager.ClientsGroupCalcDebtsBS;
            ClientCalcDebtsDataGrid.DataSource = PaymentsManager.ClientCalcDebtsBS;
            ClientGroupWriteOffDataGrid.DataSource = PaymentsManager.ClientsGroupWriteOffBS;
            ClientWriteOffDataGrid.DataSource = PaymentsManager.ClientWriteOffBS;
            ClientGroupDispatchDataGrid.DataSource = PaymentsManager.ClientsGroupDispatchBS;
            ClientDispatchDataGrid.DataSource = PaymentsManager.ClientDispatchBS;

            ResultLabel.Text = PaymentsManager.ResultText;
            CalcWriteOffResultLabel.Text = PaymentsManager.CalcWriteOffResult;
            WriteOffResultLabel.Text = PaymentsManager.WriteOffResult;

            FilterClientsGridSettings();
            FilterGroupsGridSettings();
            ClientsGroupDebtsGridSettings();
            ClientDebtsGridSettings();
            ClientWriteOffGridSettings();
            ClientsGroupWriteOffGridSettings();
            ClientDispatchGridSettings();
            ClientsGroupDispatchGridSettings();
        }

        private void dgvGridSettings(ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            Grid.Columns["DebtTypeID"].Visible = false;
            Grid.Columns["MainOrderID"].Visible = false;
            Grid.Columns["Cost"].DefaultCellStyle.Format = "N";
            Grid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            Grid.Columns["DispatchDate"].HeaderText = "Дата отгрузки";
            Grid.Columns["ClientName"].HeaderText = "Клиент";
            Grid.Columns["DocNumber"].HeaderText = "№ документа";
            Grid.Columns["Cost"].HeaderText = "Сумма";

            Grid.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["DispatchDate"].Width = 150;
            Grid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["Cost"].Width = 150;
            Grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid.Columns["ClientName"].Width = 190;

            Grid.AutoGenerateColumns = false;

            Grid.Columns["DispatchDate"].DisplayIndex = 0;
            Grid.Columns["ClientName"].DisplayIndex = 1;
            Grid.Columns["DocNumber"].DisplayIndex = 2;
            Grid.Columns["Cost"].DisplayIndex = 4;
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

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ZOVDebtsManager == null)
                return;
            if (kryptonCheckSet1.CheckedButton == cbtnPayments)
            {
                panel2.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnDebts)
            {
                panel4.BringToFront();
            }

        }

        private void PaymentWeeksZOVSelectDateForm_Load(object sender, EventArgs e)
        {
            decimal DebtsCost = 0;
            decimal DebtsCostAll = 0;
            decimal DoNotDispatchCost = 0;
            decimal DoNotDispatchCostAll = 0;
            DateTime FirstDate = DateTime.Now.AddDays(-8);
            DateTime SecondDate = DateTime.Now;

            CalendarFrom.SelectionStart = FirstDate;
            ZOVDebtsManager.Load(ref DoNotDispatchCost, ref DoNotDispatchCostAll, ref DebtsCost, ref DebtsCostAll, FirstDate, SecondDate);

            label4.Text = DoNotDispatchCost.ToString("N", nfi1) + " €\r\n" + DoNotDispatchCostAll.ToString("N", nfi1) + " €";
            label5.Text = DebtsCost.ToString("N", nfi1) + " €\r\n" + DebtsCostAll.ToString("N", nfi1) + " €";
        }

        private void dgvDoNotDispatchDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Add this
                dgvDoNotDispatchDetails.CurrentCell = dgvDoNotDispatchDetails.Rows[e.RowIndex].Cells[e.ColumnIndex];
                // Can leave these here - doesn't hurt
                dgvDoNotDispatchDetails.Rows[e.RowIndex].Selected = true;
                dgvDoNotDispatchDetails.Focus();

                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvDebtsDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Add this
                dgvDebtsDetails.CurrentCell = dgvDebtsDetails.Rows[e.RowIndex].Cells[e.ColumnIndex];
                // Can leave these here - doesn't hurt
                dgvDebtsDetails.Rows[e.RowIndex].Selected = true;
                dgvDebtsDetails.Focus();

                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnMoveToExpedition_Click(object sender, EventArgs e)
        {
            string DocNumber = dgvDoNotDispatchDetails.SelectedRows[0].Cells["DocNumber"].Value.ToString();
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ZOVExpeditionForm ZOVExpeditionForm = new ZOVExpeditionForm(this, DocNumber);

            TopForm = ZOVExpeditionForm;

            ZOVExpeditionForm.ShowDialog();

            ZOVExpeditionForm.Close();
            ZOVExpeditionForm.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            string DocNumber = dgvDebtsDetails.SelectedRows[0].Cells["DocNumber"].Value.ToString();
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ZOVExpeditionForm ZOVExpeditionForm = new ZOVExpeditionForm(this, DocNumber);

            TopForm = ZOVExpeditionForm;

            ZOVExpeditionForm.ShowDialog();

            ZOVExpeditionForm.Close();
            ZOVExpeditionForm.Dispose();

            TopForm = null;
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            DateTime FirstDay = ClientsCalendarFrom.SelectionStart;
            DateTime Today = ClientsCalendarTo.SelectionStart;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                PaymentsManager.GetCheckedClients();

                PaymentsManager.GetCheckedGroups();

                PaymentsManager.CalcDebtsOnScaner(FirstDay, Today);

                PaymentsManager.GetClientGroupCalcDebts(FirstDay, Today);
                PaymentsManager.GetClientGroupWriteOff(FirstDay, Today);
                PaymentsManager.GetClientGroupDispatch(FirstDay, Today);

                PaymentsManager.GetClientCalcDebts(FirstDay, Today);
                PaymentsManager.GetClientWriteOff(FirstDay, Today);
                PaymentsManager.GetClientDispatch(FirstDay, Today);

                PaymentsManager.GetClientGroupSamples(FirstDay, Today);
                PaymentsManager.GetClientSamples(FirstDay, Today);

                ResultLabel.Text = PaymentsManager.ResultText;
                CalcWriteOffResultLabel.Text = PaymentsManager.CalcWriteOffResult;
                WriteOffResultLabel.Text = PaymentsManager.WriteOffResult;

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                PaymentsManager.GetCheckedClients();
                PaymentsManager.GetCheckedGroups();

                PaymentsManager.CalcDebtsOnScaner(FirstDay, Today);

                ResultLabel.Text = PaymentsManager.ResultText;
                CalcWriteOffResultLabel.Text = PaymentsManager.CalcWriteOffResult;
                WriteOffResultLabel.Text = PaymentsManager.WriteOffResult;
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
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void btnFilterOk_Click(object sender, EventArgs e)
        {
            decimal DebtsCost = 0;
            decimal DebtsCostAll = 0;
            decimal DoNotDispatchCost = 0;
            decimal DoNotDispatchCostAll = 0;
            DateTime FirstDate = CalendarFrom.SelectionStart;
            DateTime SecondDate = CalendarTo.SelectionStart;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обработка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ZOVDebtsManager.Load(ref DoNotDispatchCost, ref DoNotDispatchCostAll, ref DebtsCost, ref DebtsCostAll, FirstDate, SecondDate);

            label4.Text = DoNotDispatchCost.ToString("N", nfi1) + " €\r\n" + DoNotDispatchCostAll.ToString("N", nfi1) + " €";
            label5.Text = DebtsCost.ToString("N", nfi1) + " €\r\n" + DebtsCostAll.ToString("N", nfi1) + " €";

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

    }
}
