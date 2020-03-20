using Infinium.Modules.Marketing.NewOrders;

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CurrencyForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool RateExist = false;
        bool FixedPaymentRate = false;
        bool CBRDailyRates = true;
        bool NBRBDailyRates = true;
        decimal EURRUBCurrency = 1000000;
        decimal USDRUBCurrency = 1000000;
        decimal EURUSDCurrency = 1000000;
        decimal EURBYRCurrency = 1000000;
        decimal CurrencyTotalCost = 0;

        int FormEvent = 0;
        int ClientID = 0;
        int TransportType = 0;
        decimal Weight = 0;
        decimal TransportCost = 0;
        decimal Rate = 1;

        Form MainForm = null;
        Form TopForm = null;

        OrdersManager OrdersManager = null;
        OrdersCalculate OrdersCalculate = null;
        InvoiceReportToDBF DBFReport = null;

        bool CanSetDirectorDiscount = false;

        public CurrencyForm(Form tMainForm, ref OrdersManager tOrdersManager, ref OrdersCalculate tOrdersCalculate, ref InvoiceReportToDBF tDBFReport, int iClientID, string ClientName, bool bCanSetDirectorDiscount)
        {
            MainForm = tMainForm;
            ClientID = iClientID;
            CanSetDirectorDiscount = bCanSetDirectorDiscount;
            OrdersManager = tOrdersManager;
            OrdersCalculate = tOrdersCalculate;
            DBFReport = tDBFReport;

            InitializeComponent();
            panel8.Visible = false;
            if (bCanSetDirectorDiscount)
            {
                panel7.Visible = true;
            }
            label2.Text = "Расчет " + ClientName;
            CurrencyTypeComboBox.Items.Add("Евро - Евро");
            CurrencyTypeComboBox.Items.Add("Евро - Доллар США");
            CurrencyTypeComboBox.Items.Add("Евро - Российский рубль");
            CurrencyTypeComboBox.Items.Add("Евро - Белорусский рубль");

            Initialize();
        }

        private void ConfirmationOKButton_Click(object sender, EventArgs e)
        {
            if (!CanSetDirectorDiscount && Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 2)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Заказ согласован, пересчет запрещен. Звоните Директорату",
                        "Пересчет заказа");
                FormEvent = eClose;
                AnimateTimer.Enabled = true;
                return;
            }

            if (CurrencyTextBox.Text == "0")
            {
                MessageBox.Show("Курс валюты равен нулю, проверьте курс");
                return;
            }

            //if (rbCpt.Checked && Convert.ToDecimal(TransportCostTextEdit.Text) == 0)
            //{
            //    MessageBox.Show("Введите сумму транспорта");
            //    return;
            //}

            if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) != 2)
                OrdersManager.SetNotConfirmOrder();

            if (FixedPaymentRate)
            {
                //if (rbCurrentFixedPaymentRate.Checked)
                //    Rate = Convert.ToDecimal(tbCurrentFixedPaymentRate.Text);
                //if (rbNewFixedPaymentRate.Checked)
                //{
                //    Rate = Convert.ToDecimal(tbNewFixedPaymentRate.Text);
                //    OrdersManager.SaveClientPaymentRate(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["ClientID"]), Rate);
                //}
                OrdersManager.SetFixedPaymentRate(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), true);
            }
            else
                OrdersManager.SetFixedPaymentRate(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), false);

            int CurrencyTypeID = CurrencyTypeComboBox.SelectedIndex + 1;
            if (CurrencyTypeComboBox.SelectedIndex == 3)
                CurrencyTypeID = 5;
            int DelayOfPayment = 0;
            int DiscountFactoringID = 0;
            int DiscountPaymentConditionID = 1;
            if (rbHalfOfPayment.Checked)
            {
                DiscountPaymentConditionID = 2;
                DelayOfPayment = 0;
            }
            if (rbFullPayment.Checked)
            {
                DiscountPaymentConditionID = 3;
                DelayOfPayment = 0;
            }
            if (rbFullPayment.Checked)
            {
                DiscountPaymentConditionID = 3;
                DelayOfPayment = 0;
            }
            if (kryptonRadioButton3.Checked)
            {
                DiscountPaymentConditionID = 5;
                DelayOfPayment = 0;
            }
            if (kryptonRadioButton5.Checked)
            {
                DiscountPaymentConditionID = 6;
                DelayOfPayment = 0;
            }
            if (rbFactoring.Checked)
            {
                DiscountPaymentConditionID = 4;
            }

            if (DiscountPaymentConditionID == 1)
                DelayOfPayment = Convert.ToInt32(tbDelayOfPayment.Text);

            decimal ProfilTotalSum = 0;
            decimal TPSTotalSum = 0;
            OrdersCalculate.GetMainOrderTotalSum(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), ref ProfilTotalSum, ref TPSTotalSum);

            if (ProfilTotalSum < Convert.ToDecimal(tbComplaintProfilCost.Text) || TPSTotalSum < Convert.ToDecimal(tbComplaintTPSCost.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                       "Сумма рекламации не может быть больше стоимости заказа",
                       "Расчет заказа");
                return;
            }

            decimal DiscountPaymentCondition = OrdersManager.DiscountPaymentCondition(DiscountPaymentConditionID);
            string FactoringName = string.Empty;
            if (DiscountPaymentConditionID == 4)
            {
                if (kryptonRadioButton2.Checked)
                    FactoringName = kryptonRadioButton2.Text;
                if (kryptonRadioButton4.Checked)
                    FactoringName = kryptonRadioButton4.Text;
                if (kryptonRadioButton1.Checked)
                    FactoringName = kryptonRadioButton1.Text;
                int DaysCount = 0;
                DiscountPaymentCondition = OrdersManager.DiscountFactoring(ref DiscountFactoringID, ref DaysCount, CurrencyTypeID, FactoringName);
                DelayOfPayment = DaysCount;
            }
            decimal ProfilDiscountOrderSum = OrdersManager.DiscountOrderSum(ProfilTotalSum);
            decimal TPSDiscountOrderSum = OrdersManager.DiscountOrderSum(TPSTotalSum);
            decimal ProfilDiscountDirector = Convert.ToDecimal(tbProfilDiscountDirector.Text);
            decimal TPSDiscountDirector = Convert.ToDecimal(tbTPSDiscountDirector.Text);
            decimal ProfilTotalDiscount = 0;
            decimal TPSTotalDiscount = 0;
            decimal OriginalRate = Rate;
            decimal PaymentRate = Rate;
            int TransportType = 0;
            object ConfirmDateTime = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ConfirmDateTime"];
            if (ClientID != 145 && DiscountPaymentConditionID == 1 && (CurrencyTypeID == 3 || CurrencyTypeID == 5))
            {
                PaymentRate = OriginalRate * 1.05m;
            }

            if (DiscountPaymentConditionID == 1 || DiscountPaymentConditionID == 6)
            {
                ProfilDiscountOrderSum = 0;
                TPSDiscountOrderSum = 0;
            }
            decimal TransportCost = Convert.ToDecimal(TransportCostTextEdit.Text);
            if (rbFCA.Checked)
            {
                TransportType = 1;
                TransportCost = 0;
            }
            ProfilTotalDiscount = DiscountPaymentCondition + ProfilDiscountOrderSum + ProfilDiscountDirector;
            TPSTotalDiscount = DiscountPaymentCondition + TPSDiscountOrderSum + TPSDiscountDirector;

            OrdersCalculate.Recalculate(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]),
                Convert.ToDecimal(tbComplaintProfilCost.Text), Convert.ToDecimal(tbComplaintTPSCost.Text),
                TransportType, TransportCost, Convert.ToDecimal(AdditionalCostTextEdit.Text),
                ProfilDiscountDirector, TPSDiscountDirector,
                (DiscountPaymentCondition + ProfilDiscountOrderSum), (DiscountPaymentCondition + TPSDiscountOrderSum), DiscountPaymentCondition,
                CurrencyTypeID, PaymentRate, ConfirmDateTime);

            CurrencyTotalCost = DBFReport.CalcCurrencyCost(
                Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]),
                Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]), CurrencyTypeID, PaymentRate);
            OrdersManager.SetCurrencyCost(Convert.ToDecimal(tbComplaintProfilCost.Text), Convert.ToDecimal(tbComplaintTPSCost.Text), tbComplaintNotes.Text, IsComplaintCheckBox.Checked, DelayOfPayment,
                TransportCost, Convert.ToDecimal(AdditionalCostTextEdit.Text),
                CurrencyTypeID, OriginalRate, PaymentRate,
                DiscountPaymentConditionID, DiscountFactoringID, ProfilDiscountOrderSum, TPSDiscountOrderSum, ProfilDiscountDirector, TPSDiscountDirector, CurrencyTotalCost);

            //change date
            string DesireDate = "";

            if (NoDateCheckBox.Checked == false)
                DesireDate = DispatchDateTimePicker.Value.Date.ToString("yyyy-MM-dd");

            OrdersManager.LastCalcDate(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]));
            OrdersManager.ChangeDate(DesireDate);

            if (CanSetDirectorDiscount || Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) != 0)
            {
                OrdersManager.MoveOrdersTo(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                    Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]));
            }

            OrdersManager.FixOrderEvent(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]), "Заказ рассчитан");
            OrdersManager.SendReport = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ConfirmationCancelButton_Click(object sender, EventArgs e)
        {
            OrdersManager.SendReport = false;
            FormEvent = eClose;
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

        public void SetParameters(int iTransportType, int DelayOfPayment, object DesireDate, object ConfirmDate, int CurrencyTypeID, decimal OriginalRate, decimal PaymentRate, decimal ComplaintProfilCost, decimal ComplaintTPSCost, string ComplaintNotes,
            decimal dTransportCost, decimal AdditionalCost, int DiscountPaymentConditionID, int DiscountFactoringID, decimal ProfilDiscountDirector, decimal TPSDiscountDirector, decimal dWeight)
        {
            TransportType = iTransportType;
            TransportCost = dTransportCost;
            Weight = dWeight;
            if (iTransportType == 1)
            {
                TransportCostTextEdit.Visible = false;
                rbFCA.Checked = true;
            }

            if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 2)
            {
                if (!CanSetDirectorDiscount)
                {
                    CurrencyTypeComboBox.Enabled = false;
                    rbDelayOfPayment.Enabled = false;
                    rbFactoring.Enabled = false;
                    rbFullPayment.Enabled = false;
                    tbDelayOfPayment.Enabled = false;
                    kryptonRadioButton3.Enabled = false;
                    kryptonRadioButton5.Enabled = false;

                    kryptonRadioButton2.Enabled = false;
                    kryptonRadioButton4.Enabled = false;
                    kryptonRadioButton1.Enabled = false;
                    rbCpt.Enabled = false;
                    rbFCA.Enabled = false;
                    TransportCostTextEdit.Enabled = false;
                    AdditionalCostTextEdit.Enabled = false;
                    IsComplaintCheckBox.Enabled = false;
                    tbComplaintProfilCost.Enabled = false;
                    tbComplaintTPSCost.Enabled = false;
                    tbComplaintNotes.Enabled = false;

                    panel2.Enabled = false;
                    panel3.Enabled = false;
                    panel4.Enabled = false;
                    panel6.Enabled = false;
                    panel7.Enabled = false;
                }
            }
            else
            {
                if (TransportType == 0)
                {
                    decimal d = OrdersManager.TransportCost(dWeight);
                    if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 0
                        || Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 3)
                        TransportCost = OrdersManager.TransportCost(dWeight);
                    else
                        TransportCost = dTransportCost;
                }
                else
                    TransportCost = 0;
            }

            if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 0)
                DelayOfPayment = OrdersManager.GetDelayOfPayment(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]));
            else
                DelayOfPayment = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DelayOfPayment"]);


            tbDelayOfPayment.Text = DelayOfPayment.ToString();
            TransportCostTextEdit.Text = TransportCost.ToString();
            AdditionalCostTextEdit.Text = AdditionalCost.ToString();
            tbComplaintProfilCost.Text = ComplaintProfilCost.ToString();
            tbComplaintTPSCost.Text = ComplaintTPSCost.ToString();
            tbComplaintNotes.Text = ComplaintNotes.ToString();
            tbProfilDiscountDirector.Text = ProfilDiscountDirector.ToString();
            tbTPSDiscountDirector.Text = TPSDiscountDirector.ToString();

            if (ConfirmDate != DBNull.Value)
                OrdersManager.GetDateRates(Convert.ToDateTime(ConfirmDate), ref RateExist, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency, ref USDRUBCurrency);
            else
                OrdersManager.GetDateRates(CurrencyDateTimePicker.Value.Date, ref RateExist, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency, ref USDRUBCurrency);
            if (!RateExist)
            {
                if (ConfirmDate != DBNull.Value)
                {
                    CBRDailyRates = OrdersManager.CBRDailyRates(Convert.ToDateTime(ConfirmDate), ref EURRUBCurrency, ref USDRUBCurrency);
                    NBRBDailyRates = OrdersManager.NBRBDailyRates(Convert.ToDateTime(ConfirmDate), ref EURBYRCurrency);
                }
                else
                {
                    CBRDailyRates = OrdersManager.CBRDailyRates(CurrencyDateTimePicker.Value.Date, ref EURRUBCurrency, ref USDRUBCurrency);
                    NBRBDailyRates = OrdersManager.NBRBDailyRates(CurrencyDateTimePicker.Value.Date, ref EURBYRCurrency);
                }
                if (USDRUBCurrency != 0)
                    EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);

                if (EURBYRCurrency == 1000000)
                {

                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Курс не взят. Возможная причина - нет связи с банком без авторизации в kerio control. Войдите в kerio и повторите попытку",
                        "Расчет заказа");
                    return;
                }
                if (ConfirmDate != DBNull.Value)
                    OrdersManager.SaveDateRates(Convert.ToDateTime(ConfirmDate), EURUSDCurrency, EURRUBCurrency, EURBYRCurrency, USDRUBCurrency);
                else
                    OrdersManager.SaveDateRates(CurrencyDateTimePicker.Value.Date, EURUSDCurrency, EURRUBCurrency, EURBYRCurrency, USDRUBCurrency);
                RateExist = true;
            }

            if (ConfirmDate != DBNull.Value)
            {
                OrdersCalculate.GetFixedPaymentRate(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]),
                    Convert.ToDateTime(ConfirmDate), ref FixedPaymentRate, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency);
                CurrencyDateTimePicker.Value = Convert.ToDateTime(ConfirmDate);
            }
            else
                OrdersCalculate.GetFixedPaymentRate(Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ClientID"]),
                    CurrencyDateTimePicker.Value.Date, ref FixedPaymentRate, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency);
            if (FixedPaymentRate)
            {
                USDRUBCurrency = 1;
                NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                NoConnectLabel.Text = "Фиксированный курс";
                CBRDailyRates = true;
                NBRBDailyRates = true;
            }
            else
            {
                if (!RateExist)
                {
                    EURRUBCurrency = 0;
                    USDRUBCurrency = 0;
                    EURUSDCurrency = 0;
                    EURBYRCurrency = 0;
                    CBRDailyRates = OrdersManager.CBRDailyRates(CurrencyDateTimePicker.Value.Date, ref EURRUBCurrency, ref USDRUBCurrency);
                    NBRBDailyRates = OrdersManager.NBRBDailyRates(CurrencyDateTimePicker.Value.Date, ref EURBYRCurrency);
                    if (USDRUBCurrency != 0)
                        EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);
                }
            }

            if (CurrencyTypeID != 5)
                CurrencyTypeComboBox.SelectedIndex = CurrencyTypeID - 1;
            else
                CurrencyTypeComboBox.SelectedIndex = 3;

            if (DiscountPaymentConditionID == 1)
                rbDelayOfPayment.Checked = true;
            if (DiscountPaymentConditionID == 2)
                rbHalfOfPayment.Checked = true;
            if (DiscountPaymentConditionID == 3)
                rbFullPayment.Checked = true;
            if (DiscountPaymentConditionID == 5)
                kryptonRadioButton3.Checked = true;
            if (DiscountPaymentConditionID == 6)
                kryptonRadioButton5.Checked = true;
            if (DiscountPaymentConditionID == 4)
            {
                panel8.Visible = true;
                rbFactoring.Checked = true;
                string FactoringName = OrdersManager.DiscountFactoringName(DiscountFactoringID);
                if (FactoringName == kryptonRadioButton2.Text)
                    kryptonRadioButton2.Checked = true;
                if (FactoringName == kryptonRadioButton4.Text)
                    kryptonRadioButton4.Checked = true;
                if (FactoringName == kryptonRadioButton1.Text)
                    kryptonRadioButton1.Checked = true;
            }
            Rate = OriginalRate;
            CurrencyTextBox.Text = PaymentRate.ToString();
            if (DesireDate != DBNull.Value)
                DispatchDateTimePicker.Value = Convert.ToDateTime(DesireDate);
            else
            {
                NoDateCheckBox.Checked = true;
            }
        }

        private void Initialize()
        {
            //CbrBankData = OrdersManager.GetCbrBankData(CurrencyDateTimePicker.Value.Date);
            //NbrbBankData = OrdersManager.GetNbrbBankData(CurrencyDateTimePicker.Value.Date);

            //int DelayOfPayment = Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DelayOfPayment"]);

            CurrencyTypeComboBox.SelectedIndex = 0;
            rbDelayOfPayment.Checked = true;
            tbDelayOfPayment.Visible = true;
            IsComplaintCheckBox.Checked = Convert.ToBoolean(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["IsComplaint"]);
            TransportCostTextEdit.Text = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["TransportCost"].ToString();
            AdditionalCostTextEdit.Text = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["AdditionalCost"].ToString();
            tbComplaintProfilCost.Text = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintProfilCost"].ToString();
            tbComplaintTPSCost.Text = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["ComplaintTPSCost"].ToString();
            tbDelayOfPayment.Text = ((DataRowView)OrdersManager.MegaOrdersBindingSource.Current).Row["DelayOfPayment"].ToString();

            if (TransportCostTextEdit.Text.Length == 0)
                TransportCostTextEdit.Text = "0";
            if (AdditionalCostTextEdit.Text.Length == 0)
                AdditionalCostTextEdit.Text = "0";
            if (tbComplaintProfilCost.Text.Length == 0)
                tbComplaintProfilCost.Text = "0";
            if (tbComplaintTPSCost.Text.Length == 0)
                tbComplaintTPSCost.Text = "0";

            if (!CBRDailyRates && !NBRBDailyRates)
            {
                NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                NoConnectLabel.Text = "Нет связи с банками";
            }
            else
                NoConnectLabel.Text = "Связь с банками установлена";

            if (FixedPaymentRate)
            {
                USDRUBCurrency = 1;
                NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                NoConnectLabel.Text = "Фиксированный курс";
                CBRDailyRates = true;
                NBRBDailyRates = true;
            }
            CurrencyTextBox.Text = "1";
        }

        private void CurrencyDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            //CbrBankData = "";
            //NbrbBankData = "";

            //CbrBankData = OrdersManager.GetCbrBankData(CurrencyDateTimePicker.Value.Date);
            //NbrbBankData = OrdersManager.GetNbrbBankData(CurrencyDateTimePicker.Value.Date);

            if (!CBRDailyRates && !NBRBDailyRates)
            {
                NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                NoConnectLabel.Text = "Нет связи с банками";
            }
            else
                NoConnectLabel.Text = "Связь с банками установлена";

            if (FixedPaymentRate)
            {
                USDRUBCurrency = 1;
                NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                NoConnectLabel.Text = "Фиксированный курс";
                CBRDailyRates = true;
                NBRBDailyRates = true;
            }
            CurrencyTypeComboBox_SelectionChangeCommitted(null, null);
        }

        private void CurrencyTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Евро")
            {
                Rate = 1;
                CurrencyTextBox.Text = "1";
                return;
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Доллар США")
            {
                if (EURRUBCurrency == 0 || USDRUBCurrency == 0)
                {
                    NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                    NoConnectLabel.Text = "Нет связи с банком";
                    return;
                }
                Rate = EURUSDCurrency;
                CurrencyTextBox.Text = EURUSDCurrency.ToString();

                if (CurrencyTextBox.Text != "0")
                {
                    NoConnectLabel.ForeColor = Color.Black;
                    NoConnectLabel.Text = "Данные получены с cbr.ru";
                }
                else
                {
                    NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                    NoConnectLabel.Text = "Нет связи с банком";
                }
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Российский рубль")
            {
                Rate = EURRUBCurrency;
                CurrencyTextBox.Text = EURRUBCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURRUBCurrency * 1.05m).ToString();

                if (CurrencyTextBox.Text != "0")
                {
                    NoConnectLabel.ForeColor = Color.Black;
                    NoConnectLabel.Text = "Данные получены с cbr.ru";
                }
                else
                {
                    NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                    NoConnectLabel.Text = "Нет связи с банком";
                }
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Белорусский рубль")
            {
                Rate = EURBYRCurrency;
                CurrencyTextBox.Text = EURBYRCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURBYRCurrency * 1.05m).ToString();

                if (CurrencyTextBox.Text != "0")
                {
                    NoConnectLabel.ForeColor = Color.Black;
                    NoConnectLabel.Text = "Данные получены с nbrb.by";
                }
                else
                {
                    NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                    NoConnectLabel.Text = "Нет связи с банком";
                }
            }
            if (FixedPaymentRate)
            {
                USDRUBCurrency = 1;
                NoConnectLabel.ForeColor = Color.FromArgb(187, 20, 20);
                NoConnectLabel.Text = "Фиксированный курс";
                CBRDailyRates = true;
                NBRBDailyRates = true;
            }
        }

        private void NoDateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NoDateCheckBox.Checked == true)
                DispatchDateTimePicker.Enabled = false;
            else
                DispatchDateTimePicker.Enabled = true;
        }

        private void CurrencyForm_Load(object sender, EventArgs e)
        {

        }

        private void rbFactoring_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Visible = true;
            tbDelayOfPayment.Visible = false;
            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Евро")
            {
                Rate = 1;
                CurrencyTextBox.Text = "1";
                return;
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Доллар США")
            {
                Rate = EURUSDCurrency;
                CurrencyTextBox.Text = EURUSDCurrency.ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Российский рубль")
            {
                Rate = EURRUBCurrency;
                CurrencyTextBox.Text = EURRUBCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURRUBCurrency * 1.05m).ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Белорусский рубль")
            {
                Rate = EURBYRCurrency;
                CurrencyTextBox.Text = EURBYRCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURBYRCurrency * 1.05m).ToString();
            }
        }

        private void rbDelayOfPayment_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Visible = false;
            tbDelayOfPayment.Visible = true;
            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Евро")
            {
                Rate = 1;
                CurrencyTextBox.Text = "1";
                return;
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Доллар США")
            {
                Rate = EURUSDCurrency;
                CurrencyTextBox.Text = EURUSDCurrency.ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Российский рубль")
            {
                Rate = EURRUBCurrency;
                CurrencyTextBox.Text = EURRUBCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURRUBCurrency * 1.05m).ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Белорусский рубль")
            {
                Rate = EURBYRCurrency;
                CurrencyTextBox.Text = EURBYRCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURBYRCurrency * 1.05m).ToString();
            }
        }

        private void rbFullPayment_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Visible = false;
            tbDelayOfPayment.Visible = false;
            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Евро")
            {
                Rate = 1;
                CurrencyTextBox.Text = "1";
                return;
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Доллар США")
            {
                Rate = EURUSDCurrency;
                CurrencyTextBox.Text = EURUSDCurrency.ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Российский рубль")
            {
                Rate = EURRUBCurrency;
                CurrencyTextBox.Text = EURRUBCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURRUBCurrency * 1.05m).ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Белорусский рубль")
            {
                Rate = EURBYRCurrency;
                CurrencyTextBox.Text = EURBYRCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURBYRCurrency * 1.05m).ToString();
            }
        }

        private void rbHalfOfPayment_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Visible = false;
            tbDelayOfPayment.Visible = false;
            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Евро")
            {
                Rate = 1;
                CurrencyTextBox.Text = "1";
                return;
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Доллар США")
            {
                Rate = EURUSDCurrency;
                CurrencyTextBox.Text = EURUSDCurrency.ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Российский рубль")
            {
                Rate = EURRUBCurrency;
                CurrencyTextBox.Text = EURRUBCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURRUBCurrency * 1.05m).ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Белорусский рубль")
            {
                Rate = EURBYRCurrency;
                CurrencyTextBox.Text = EURBYRCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURBYRCurrency * 1.05m).ToString();
            }
        }

        private void IsComplaintCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //if (IsComplaintCheckBox.Checked)
            //{
            //    tbComplaintProfilCost.Enabled = true;
            //    tbComplaintTPSCost.Enabled = true;
            //    tbComplaintNotes.Enabled = true;
            //}
            //else
            //{
            //    tbComplaintProfilCost.Enabled = false;
            //    tbComplaintTPSCost.Enabled = false;
            //    tbComplaintNotes.Enabled = false;
            //}
        }

        private void rbFCA_CheckedChanged(object sender, EventArgs e)
        {
            TransportCostTextEdit.Visible = false;
        }

        private void rbCpt_CheckedChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 0
                || Convert.ToInt32(((DataRowView)OrdersManager.MegaOrdersBindingSource.Current)["AgreementStatusID"]) == 3)
            {
                TransportCost = OrdersManager.TransportCost(Weight);
            }
            TransportCostTextEdit.Text = TransportCost.ToString();
            TransportCostTextEdit.Visible = true;
        }

        private void TransportCostTextEdit_TextChanged(object sender, EventArgs e)
        {

        }

        private void kryptonRadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Visible = false;
            tbDelayOfPayment.Visible = false;
            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Евро")
            {
                Rate = 1;
                CurrencyTextBox.Text = "1";
                return;
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Доллар США")
            {
                Rate = EURUSDCurrency;
                CurrencyTextBox.Text = EURUSDCurrency.ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Российский рубль")
            {
                Rate = EURRUBCurrency;
                CurrencyTextBox.Text = EURRUBCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURRUBCurrency * 1.05m).ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Белорусский рубль")
            {
                Rate = EURBYRCurrency;
                CurrencyTextBox.Text = EURBYRCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURBYRCurrency * 1.05m).ToString();
            }
        }

        private void kryptonRadioButton5_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Visible = false;
            tbDelayOfPayment.Visible = false;
            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Евро")
            {
                Rate = 1;
                CurrencyTextBox.Text = "1";
                return;
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Доллар США")
            {
                Rate = EURUSDCurrency;
                CurrencyTextBox.Text = EURUSDCurrency.ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Российский рубль")
            {
                Rate = EURRUBCurrency;
                CurrencyTextBox.Text = EURRUBCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURRUBCurrency * 1.05m).ToString();
            }

            if (CurrencyTypeComboBox.SelectedItem.ToString() == "Евро - Белорусский рубль")
            {
                Rate = EURBYRCurrency;
                CurrencyTextBox.Text = EURBYRCurrency.ToString();
                if (ClientID != 145 && rbDelayOfPayment.Checked)
                    CurrencyTextBox.Text = (EURBYRCurrency * 1.05m).ToString();
            }
        }
    }
}
