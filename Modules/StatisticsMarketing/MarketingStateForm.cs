using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingStateForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        decimal NonAgreementProfilCost = 0;
        decimal AgreementProfilCost = 0;
        decimal OnProductionProfilCost = 0;
        decimal InProductionProfilCost = 0;
        decimal OnStorageProfilCost = 0;
        decimal OnExpeditionProfilCost = 0;

        decimal NonAgreementTPSCost = 0;
        decimal AgreementTPSCost = 0;
        decimal OnProductionTPSCost = 0;
        decimal InProductionTPSCost = 0;
        decimal OnStorageTPSCost = 0;
        decimal OnExpeditionTPSCost = 0;

        int FormEvent = 0;

        LightStartForm LightStartForm;
        Form TopForm = null;
        System.Globalization.NumberFormatInfo nfi1;
        Infinium.Modules.StatisticsMarketing.GeneralStatisticsMarketing StatisticsMarketing = null;

        public MarketingStateForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            nfi1 = new System.Globalization.NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 2
            };
            while (!SplashForm.bCreated) ;
        }

        private void MarketingStateForm_Shown(object sender, EventArgs e)
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
            StatisticsMarketing = new Modules.StatisticsMarketing.GeneralStatisticsMarketing(
                ref ClientGroupsList,
                ref NonAgreementDataGrid,
                ref AgreementDataGrid,
                ref OnProductionDataGrid,
                ref InProductionDataGrid,
                ref OnStorageDataGrid,
                ref OnExpeditionDataGrid);

            StatisticsMarketing.Filter();

            StatisticsMarketing.GetNonAgreementCost(ref NonAgreementProfilCost, ref NonAgreementTPSCost);
            StatisticsMarketing.GetAgreementCost(ref AgreementProfilCost, ref AgreementTPSCost);
            StatisticsMarketing.GetOnProductionCost(ref OnProductionProfilCost, ref OnProductionTPSCost);
            StatisticsMarketing.GetInProductionCost(ref InProductionProfilCost, ref InProductionTPSCost);
            StatisticsMarketing.GetOnStorageCost(ref OnStorageProfilCost, ref OnStorageTPSCost);
            StatisticsMarketing.GetOnExpeditionCost(ref OnExpeditionProfilCost, ref OnExpeditionTPSCost);

            label11.Text = NonAgreementProfilCost.ToString("N", nfi1) + " €";
            label10.Text = NonAgreementTPSCost.ToString("N", nfi1) + " €";
            label9.Text = (NonAgreementProfilCost + NonAgreementTPSCost).ToString("N", nfi1) + " €";

            label12.Text = AgreementProfilCost.ToString("N", nfi1) + " €";
            label6.Text = AgreementTPSCost.ToString("N", nfi1) + " €";
            label5.Text = (AgreementProfilCost + AgreementTPSCost).ToString("N", nfi1) + " €";

            label29.Text = OnProductionProfilCost.ToString("N", nfi1) + " €";
            label28.Text = OnProductionTPSCost.ToString("N", nfi1) + " €";
            label27.Text = (OnProductionProfilCost + OnProductionTPSCost).ToString("N", nfi1) + " €";

            label18.Text = InProductionProfilCost.ToString("N", nfi1) + " €";
            label17.Text = InProductionTPSCost.ToString("N", nfi1) + " €";
            label16.Text = (InProductionProfilCost + InProductionTPSCost).ToString("N", nfi1) + " €";

            label36.Text = OnExpeditionProfilCost.ToString("N", nfi1) + " €";
            label35.Text = OnExpeditionTPSCost.ToString("N", nfi1) + " €";
            label34.Text = (OnExpeditionProfilCost + OnExpeditionTPSCost).ToString("N", nfi1) + " €";

            label43.Text = OnStorageProfilCost.ToString("N", nfi1) + " €";
            label42.Text = OnStorageTPSCost.ToString("N", nfi1) + " €";
            label41.Text = (OnStorageProfilCost + OnStorageTPSCost).ToString("N", nfi1) + " €";

            if (Security.PriceAccess)
            {
                panel5.Visible = true;
                panel6.Visible = true;
                panel7.Visible = true;
                panel13.Visible = true;
                panel16.Visible = true;
            }
            else
            {
                panel5.Visible = false;
                panel6.Visible = false;
                panel7.Visible = false;
                panel13.Visible = false;
                panel16.Visible = false;
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

        private void ClientGroupsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (StatisticsMarketing == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            StatisticsMarketing.Filter();

            StatisticsMarketing.GetNonAgreementCost(ref NonAgreementProfilCost, ref NonAgreementTPSCost);
            StatisticsMarketing.GetAgreementCost(ref AgreementProfilCost, ref AgreementTPSCost);
            StatisticsMarketing.GetOnProductionCost(ref OnProductionProfilCost, ref OnProductionTPSCost);
            StatisticsMarketing.GetInProductionCost(ref InProductionProfilCost, ref InProductionTPSCost);
            StatisticsMarketing.GetOnStorageCost(ref OnStorageProfilCost, ref OnStorageTPSCost);
            StatisticsMarketing.GetOnExpeditionCost(ref OnExpeditionProfilCost, ref OnExpeditionTPSCost);

            label11.Text = NonAgreementProfilCost.ToString("N", nfi1) + " €";
            label10.Text = NonAgreementTPSCost.ToString("N", nfi1) + " €";
            label9.Text = (NonAgreementProfilCost + NonAgreementTPSCost).ToString("N", nfi1) + " €";

            label12.Text = AgreementProfilCost.ToString("N", nfi1) + " €";
            label6.Text = AgreementTPSCost.ToString("N", nfi1) + " €";
            label5.Text = (AgreementProfilCost + AgreementTPSCost).ToString("N", nfi1) + " €";

            label29.Text = OnProductionProfilCost.ToString("N", nfi1) + " €";
            label28.Text = OnProductionTPSCost.ToString("N", nfi1) + " €";
            label27.Text = (OnProductionProfilCost + OnProductionTPSCost).ToString("N", nfi1) + " €";

            label18.Text = InProductionProfilCost.ToString("N", nfi1) + " €";
            label17.Text = InProductionTPSCost.ToString("N", nfi1) + " €";
            label16.Text = (InProductionProfilCost + InProductionTPSCost).ToString("N", nfi1) + " €";

            label36.Text = OnExpeditionProfilCost.ToString("N", nfi1) + " €";
            label35.Text = OnExpeditionTPSCost.ToString("N", nfi1) + " €";
            label34.Text = (OnExpeditionProfilCost + OnExpeditionTPSCost).ToString("N", nfi1) + " €";

            label43.Text = OnStorageProfilCost.ToString("N", nfi1) + " €";
            label42.Text = OnStorageTPSCost.ToString("N", nfi1) + " €";
            label41.Text = (OnStorageProfilCost + OnStorageTPSCost).ToString("N", nfi1) + " €";

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }
    }
}
