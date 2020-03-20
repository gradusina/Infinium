using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ClientStatisticsZOVDetailForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;
        DateTime DateFrom;
        DateTime DateTo;

        public Modules.StaticticsZOV.ClientStatisticsZOV ClientStatisticsZOV;


        public ClientStatisticsZOVDetailForm(Form tMainForm, ref Modules.StaticticsZOV.ClientStatisticsZOV tClientStatisticsZOV, DateTime tDateFrom, DateTime tDateTo)
        {
            InitializeComponent();
            MainForm = tMainForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            DateFrom = tDateFrom;
            DateTo = tDateTo;

            ClientStatisticsZOV = tClientStatisticsZOV;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void ClientStatisticsZOVDetailForm_Shown(object sender, EventArgs e)
        {
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
                        MainForm.Activate();
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
                        MainForm.Activate();
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
            ClientStatisticsZOV.Load(Convert.ToInt32(((DataRowView)ClientStatisticsZOV.ClientsBindingSource.Current)["ClientID"]), DateFrom, DateTo);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 0
            };
            TotalSquareLabel.Text = ClientStatisticsZOV.TotalFrontsSquare.ToString("C", nfi1) + "  м.кв.";
            TotalFrontsCountLabel.Text = ClientStatisticsZOV.TotalFrontsCount.ToString("C", nfi2) + "  шт.";
            TotalFrontsCostLabel.Text = ClientStatisticsZOV.TotalFrontsCost.ToString("C", nfi1) + "  €";

            SamplesCostLabel.Text = ClientStatisticsZOV.TotalSamplesCost.ToString("C", nfi1) + "  €";
            DefectsCostLabel.Text = ClientStatisticsZOV.TotalDefectsCost.ToString("C", nfi1) + "  €";

            TotalDecorLengthLabel.Text = ClientStatisticsZOV.TotalDecorLength.ToString("C", nfi1) + "  м.п.";
            TotalDecorCountLabel.Text = ClientStatisticsZOV.TotalDecorCount.ToString("C", nfi2) + "  шт.";
            TotalDecorCostLabel.Text = ClientStatisticsZOV.TotalDecorCost.ToString("C", nfi1) + "  €";

            TotalLabel.Text = ClientStatisticsZOV.TotalCost.ToString("C", nfi1) + " €";
            //" €"



            label1.Text = "Infinium. ЗОВ. Статистика. Клиент " + ClientStatisticsZOV.GetClientName(Convert.ToInt32(((DataRowView)ClientStatisticsZOV.ClientsBindingSource.Current)["ClientID"])) +
                          " за " + DateFrom.ToString("dd.MM.yyyy") + " - " + DateTo.ToString("dd.MM.yyyy");



            //label3.Text = (1200).ToString("C", nfi1);

            ClientStatisticsZOV.BindingDetailGrids(ref FrontsCountDataGrid, ref FrontsColorsDataGrid, ref DecorCountDataGrid);
        }


        private void ClientsCostDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            ClientStatisticsZOV.FilterColors(((DataRowView)ClientStatisticsZOV.FrontsCountBindingSource.Current)["Front"].ToString());
        }

        private void PriorButton_Click(object sender, EventArgs e)
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
    }
}
