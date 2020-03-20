using Infinium.Modules.StatisticsMarketing;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DoubleOrdersStatisticsForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        DoubleOrdersStatistics DoubleOrdersStatisticsManager;

        LightStartForm LightStartForm;
        Form TopForm = null;

        public DoubleOrdersStatisticsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            while (!SplashForm.bCreated) ;
        }


        private void DoubleOrdersStatisticsForm_Shown(object sender, EventArgs e)
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void FirstOperatorGridSetting()
        {
            dgvFirstOperator.Columns["FirstDocDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvFirstOperator.Columns["FirstSaveDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvFirstOperator.Columns["FirstDocDateTime"].HeaderText = "Создано";
            dgvFirstOperator.Columns["FirstSaveDateTime"].HeaderText = "Сохранено";
            dgvFirstOperator.Columns["DocNumber"].HeaderText = "№ кухни";
            dgvFirstOperator.Columns["ClientName"].HeaderText = "Клиент";
            dgvFirstOperator.Columns["FirstErrors"].HeaderText = "Ошибки";

            dgvFirstOperator.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFirstOperator.Columns["ClientName"].MinimumWidth = 250;
            dgvFirstOperator.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFirstOperator.Columns["DocNumber"].MinimumWidth = 250;
            dgvFirstOperator.Columns["FirstDocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFirstOperator.Columns["FirstDocDateTime"].MinimumWidth = 190;
            dgvFirstOperator.Columns["FirstSaveDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFirstOperator.Columns["FirstSaveDateTime"].MinimumWidth = 190;
            dgvFirstOperator.Columns["FirstErrors"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvFirstOperator.Columns["FirstErrors"].MinimumWidth = 160;

            dgvFirstOperator.AutoGenerateColumns = false;

            dgvFirstOperator.Columns["ClientName"].DisplayIndex = 0;
            dgvFirstOperator.Columns["DocNumber"].DisplayIndex = 1;
            dgvFirstOperator.Columns["FirstDocDateTime"].DisplayIndex = 2;
            dgvFirstOperator.Columns["FirstSaveDateTime"].DisplayIndex = 3;
            dgvFirstOperator.Columns["FirstErrors"].DisplayIndex = 4;
        }

        private void SecondOperatorGridSetting()
        {
            dgvSecondOperator.Columns["SecondDocDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvSecondOperator.Columns["SecondSaveDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvSecondOperator.Columns["SecondDocDateTime"].HeaderText = "Создано";
            dgvSecondOperator.Columns["SecondSaveDateTime"].HeaderText = "Сохранено";
            dgvSecondOperator.Columns["DocNumber"].HeaderText = "№ кухни";
            dgvSecondOperator.Columns["ClientName"].HeaderText = "Клиент";
            dgvSecondOperator.Columns["SecondErrors"].HeaderText = "Ошибки";

            dgvSecondOperator.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSecondOperator.Columns["ClientName"].MinimumWidth = 250;
            dgvSecondOperator.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvSecondOperator.Columns["DocNumber"].MinimumWidth = 250;
            dgvSecondOperator.Columns["SecondDocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvSecondOperator.Columns["SecondDocDateTime"].MinimumWidth = 190;
            dgvSecondOperator.Columns["SecondSaveDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvSecondOperator.Columns["SecondSaveDateTime"].MinimumWidth = 190;
            dgvSecondOperator.Columns["SecondErrors"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSecondOperator.Columns["SecondErrors"].MinimumWidth = 160;

            dgvSecondOperator.AutoGenerateColumns = false;

            dgvSecondOperator.Columns["ClientName"].DisplayIndex = 0;
            dgvSecondOperator.Columns["DocNumber"].DisplayIndex = 1;
            dgvSecondOperator.Columns["SecondDocDateTime"].DisplayIndex = 2;
            dgvSecondOperator.Columns["SecondSaveDateTime"].DisplayIndex = 3;
            dgvSecondOperator.Columns["SecondErrors"].DisplayIndex = 4;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (DoubleOrdersStatisticsManager == null)
                return;
            if (kryptonCheckSet1.CheckedIndex == 0)
            {
                FirstOperatorStatistics();
                cmbFirstOperators.BringToFront();
                FirstOperatorPanel.BringToFront();
            }
            if (kryptonCheckSet1.CheckedIndex == 1)
            {
                SecondOperatorStatistics();
                cmbSecondOperators.BringToFront();
                SecondOperatorPanel.BringToFront();
            }
        }

        private void DoubleOrdersStatisticsForm_Load(object sender, EventArgs e)
        {
            dtpFirstDate.Value = DateTime.Now.AddMonths(-1);
            dtpSecondDate.Value = DateTime.Now;

            DoubleOrdersStatisticsManager = new DoubleOrdersStatistics();
            DoubleOrdersStatisticsManager.Initialize();
            dgvFirstOperator.DataSource = DoubleOrdersStatisticsManager.FirstOperatorStatisticsList;
            FirstOperatorGridSetting();
            dgvSecondOperator.DataSource = DoubleOrdersStatisticsManager.SecondOperatorStatisticsList;
            SecondOperatorGridSetting();

            cmbFirstOperators.DataSource = DoubleOrdersStatisticsManager.FirstOperatorsList;
            cmbFirstOperators.ValueMember = "UserID";
            cmbFirstOperators.DisplayMember = "ShortName";

            cmbSecondOperators.DataSource = DoubleOrdersStatisticsManager.SecondOperatorsList;
            cmbSecondOperators.ValueMember = "UserID";
            cmbSecondOperators.DisplayMember = "ShortName";
        }

        private static string GetSuffix(int ItemCount, string[] str)
        {
            int i = 0;
            string suffix = string.Empty;

            ItemCount = ItemCount % 100;
            if (ItemCount >= 11 && ItemCount <= 19)
            {
                suffix = str[2];
            }
            else
            {
                i = ItemCount % 10;
                switch (i)
                {
                    case (1): suffix = str[0]; break;
                    case (2):
                    case (3):
                    case (4): suffix = str[1]; break;
                    default: suffix = str[2]; break;
                }
            }
            return suffix;
        }

        private void FirstOperatorStatistics()
        {
            bool bShowCorrectOrders = cbxShowCorrectOrders.Checked;
            DateTime FirstDate = dtpFirstDate.Value;
            DateTime SecondDate = dtpSecondDate.Value;
            int Errors = 0;
            int OperatorID = -1;
            TimeSpan TotalTime = new TimeSpan();

            if (DoubleOrdersStatisticsManager.HasFirstOperators)
                OperatorID = Convert.ToInt32(((DataRowView)cmbFirstOperators.SelectedItem).Row["UserID"]);

            DoubleOrdersStatisticsManager.ClearFirstOperatorStatisticsDT();
            DoubleOrdersStatisticsManager.FirstOperatorOrders(OperatorID, FirstDate, SecondDate, bShowCorrectOrders);
            DoubleOrdersStatisticsManager.CalcFirstOperatorTotal(ref Errors, ref TotalTime);

            int ItemCount = 0;
            string[] str = new string[3] { " день ", " дня ", " дней " };
            string suffix = " дней ";
            string TotalTimeText = string.Empty;
            suffix = GetSuffix(ItemCount, str);

            if (TotalTime.Days > 0)
            {
                suffix = GetSuffix(TotalTime.Days, str);
                TotalTimeText = TotalTime.Days + suffix;
            }
            if (TotalTime.Hours > 0)
            {
                str = new string[3] { " час ", " часа ", " часов " };
                suffix = " часов ";
                suffix = GetSuffix(TotalTime.Hours, str);
                TotalTimeText += TotalTime.Hours + suffix;
            }
            if (TotalTime.Minutes > 0)
            {
                str = new string[3] { " минута", " минуты", " минут" };
                suffix = " минут";
                suffix = GetSuffix(TotalTime.Minutes, str);
                TotalTimeText += TotalTime.Minutes + suffix;
            }

            lbTotalTime.Text = "Общее время вбивания: " + TotalTimeText;
            lbTotalErrors.Text = "Ошибки: " + Errors.ToString();
        }

        private void SecondOperatorStatistics()
        {
            bool bShowCorrectOrders = cbxShowCorrectOrders.Checked;
            DateTime FirstDate = dtpFirstDate.Value;
            DateTime SecondDate = dtpSecondDate.Value;
            int Errors = 0;
            int OperatorID = -1;
            TimeSpan TotalTime = new TimeSpan();

            if (DoubleOrdersStatisticsManager.HasSecondOperators)
                OperatorID = Convert.ToInt32(((DataRowView)cmbSecondOperators.SelectedItem).Row["UserID"]);

            DoubleOrdersStatisticsManager.ClearSecondOperatorStatisticsDT();
            DoubleOrdersStatisticsManager.SecondOperatorOrders(OperatorID, FirstDate, SecondDate, bShowCorrectOrders);
            DoubleOrdersStatisticsManager.CalcSecondOperatorTotal(ref Errors, ref TotalTime);

            int ItemCount = 0;
            string[] str = new string[3] { " день ", " дня ", " дней " };
            string suffix = " дней ";
            string TotalTimeText = string.Empty;
            suffix = GetSuffix(ItemCount, str);

            if (TotalTime.Days > 0)
            {
                suffix = GetSuffix(TotalTime.Days, str);
                TotalTimeText = TotalTime.Days + suffix;
            }
            if (TotalTime.Hours > 0)
            {
                str = new string[3] { " час ", " часа ", " часов " };
                suffix = " часов ";
                suffix = GetSuffix(TotalTime.Hours, str);
                TotalTimeText += TotalTime.Hours + suffix;
            }
            if (TotalTime.Minutes > 0)
            {
                str = new string[3] { " минута", " минуты", " минут" };
                suffix = " минут";
                suffix = GetSuffix(TotalTime.Minutes, str);
                TotalTimeText += TotalTime.Minutes + suffix;
            }

            lbTotalTime.Text = "Общее время вбивания: " + TotalTimeText;
            lbTotalErrors.Text = "Ошибки: " + Errors.ToString();
        }

        private void cbxShowCorrectOrders_CheckedChanged(object sender, EventArgs e)
        {
            if (DoubleOrdersStatisticsManager == null)
                return;
            if (kryptonCheckSet1.CheckedIndex == 0)
            {
                FirstOperatorStatistics();
            }
            if (kryptonCheckSet1.CheckedIndex == 1)
            {
                SecondOperatorStatistics();
            }
        }

        private void dtpFirstDate_ValueChanged(object sender, EventArgs e)
        {
            if (DoubleOrdersStatisticsManager == null)
                return;
            if (kryptonCheckSet1.CheckedIndex == 0)
            {
                FirstOperatorStatistics();
            }
            if (kryptonCheckSet1.CheckedIndex == 1)
            {
                SecondOperatorStatistics();
            }
        }

        private void dtpSecondDate_ValueChanged(object sender, EventArgs e)
        {
            if (DoubleOrdersStatisticsManager == null)
                return;
            if (kryptonCheckSet1.CheckedIndex == 0)
            {
                FirstOperatorStatistics();
            }
            if (kryptonCheckSet1.CheckedIndex == 1)
            {
                SecondOperatorStatistics();
            }
        }

        private void cmbOperators_SelectionChangeCommitted(object sender, EventArgs e)
        {
        }

        private void cmbOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DoubleOrdersStatisticsManager == null)
                return;
            FirstOperatorStatistics();
        }

        private void cmbSecondOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DoubleOrdersStatisticsManager == null)
                return;
            SecondOperatorStatistics();
        }
    }
}
