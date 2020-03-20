using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class IncomeMonthZOVForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;


        Form TopForm = null;
        LightStartForm LightStartForm;

        public Modules.StaticticsZOV.IncomeMonthZOV IncomeMonth;

        NumberFormatInfo nfi1;

        public IncomeMonthZOVForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void IncomeMonthZOVForm_Shown(object sender, EventArgs e)
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
            IncomeChart.Visible = false;
            IncomeChart.Dispose();

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
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            IncomeMonth = new Modules.StaticticsZOV.IncomeMonthZOV(ref IncomeDataGrid);
            IncomeChart.DataSource = IncomeMonth.IncomeTotalDataTable;

            IncomeChart.SeriesDataMember = "Date";
            IncomeChart.SeriesTemplate.ArgumentDataMember = "Date";
            IncomeChart.SeriesTemplate.ValueDataMembersSerializable = "Cost";

            IncomeChart.SeriesTemplate.Label.Font = new Font("Segoe UI", 12.0f, FontStyle.Bold);
            IncomeChart.SeriesTemplate.Label.TextColor = Color.Black;
            IncomeChart.SeriesTemplate.Label.Border.Visible = false;
            IncomeChart.SeriesTemplate.Label.BackColor = Color.Transparent;
            IncomeChart.SeriesTemplate.Label.Antialiasing = true;

            ((DevExpress.XtraCharts.XYDiagram)IncomeChart.Diagram).AxisX.Label.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.True;

        }

        private void IncomeChart_CustomDrawSeriesPoint(object sender, DevExpress.XtraCharts.CustomDrawSeriesPointEventArgs e)
        {
            if (Convert.ToDecimal(e.LabelText) > 0)
                e.LabelText = Convert.ToDecimal(e.LabelText).ToString("C", nfi1) + " €";

            e.LegendFont = new Font("Segoe UI", 18.0f, FontStyle.Bold);
        }

        ListSortDirection SD;

        private void IncomeDataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (IncomeDataGrid.Columns[e.ColumnIndex].HeaderText == "Месяц")
            {
                if (SD == ListSortDirection.Ascending)
                    SD = ListSortDirection.Descending;
                else
                    SD = ListSortDirection.Ascending;

                IncomeDataGrid.Sort(IncomeDataGrid.Columns["DateTime"], SD);
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

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (IncomeMonth == null)
                return;

            if (kryptonCheckSet2.CheckedButton.Name == "AllCheckButton")
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                IncomeMonth.Factory = 0;
                IncomeMonth.Fill();
                IncomeChart.RefreshData();
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            if (kryptonCheckSet2.CheckedButton.Name == "ProfilCheckButton")
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                IncomeMonth.Factory = 1;
                IncomeMonth.Fill();
                IncomeChart.RefreshData();
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            if (kryptonCheckSet2.CheckedButton.Name == "TPSCheckButton")
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                IncomeMonth.Factory = 2;
                IncomeMonth.Fill();
                IncomeChart.RefreshData();
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }

        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }
    }
}
