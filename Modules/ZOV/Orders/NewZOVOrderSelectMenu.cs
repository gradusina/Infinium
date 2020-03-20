using Infinium.Modules.ZOV;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewZOVOrderSelectMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        NewOrderInfo NewOrderInfo;
        OrdersManager OrdersManager;
        OrdersCalculate OrdersCalculate;

        public NewZOVOrderSelectMenu(Form tMainForm, ref OrdersManager tOrdersManager,
            ref OrdersCalculate tOrdersCalculate, ref NewOrderInfo tNewOrderInfo)
        {
            MainForm = tMainForm;
            NewOrderInfo = tNewOrderInfo;

            OrdersManager = tOrdersManager;
            OrdersCalculate = tOrdersCalculate;
            InitializeComponent();
            Initialize();
        }

        private void NewOrderButton_Click(object sender, EventArgs e)
        {
            if (DocNumberTextEdit.Text.Length == 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Вы не ввели номер документа", "Создание заказа");

                return;
            }

            if (DocNumberTextEdit.Text != NewOrderInfo.DocNumber)
            {
                GetOrderInfo();
            }

            if (!NewOrderInfo.IsEditOrder && !NewOrderInfo.IsDoubleOrder && !NewOrderInfo.MovePrepare && NewOrderInfo.OrderStatus != Modules.ZOV.NewOrderInfo.OrderStatusType.NewOrder)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Указанный номер документа уже существует", "Создание заказа");
                //label3.Text = "Кухня уже существует";
                label3.Left = panel4.Width / 2 - label3.Width / 2;
                return;
            }

            if (NewOrderInfo.IsPrepare && NewOrderInfo.MovePrepare && !NewOrderInfo.IsNewOrder)
            {
                if (!OrdersManager.IsDoubleOrder(NewOrderInfo.MainOrderID))
                {
                    bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Заказ не прошёл двойное вбивание, его нельзя перенести на отгрузку", "Перенос кухни");

                    return;
                }
            }

            if (NewOrderInfo.OrderStatus == Modules.ZOV.NewOrderInfo.OrderStatusType.NewOrder
                && NewOrderInfo.IsDoubleOrder)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Заказ ещё не был создан", "Двойное вбивание");

                return;
            }

            if (DebtCheckBox.Checked && DebtDocNumberTextBox.Text.Length == 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Вы не ввели номер документа долга", "Создание заказа");

                return;
            }

            bool IsOrderInProduction = OrdersManager.IsOrderInProduction(NewOrderInfo.MainOrderID);

            if (NewOrderInfo.MovePrepare && !NewOrderInfo.IsDoubleOrder && !IsOrderInProduction)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Заказ не был отдан в производство. Перенос запрещен",
                    "Предупреждение");
            }

            object DispatchDate = null;

            int DebtTypeID = 0;
            int PriceTypeID = -1;

            string Notes = string.Empty;
            string DocNumber = string.Empty;
            string DebtDocNumber = string.Empty;

            if (RetailPriceRadio.Checked)
                PriceTypeID = 0;
            if (WholePriceRadio.Checked)
                PriceTypeID = 1;
            if (WallPriceRadio.Checked)
                PriceTypeID = 2;

            if (DebtTypeComboBox.Enabled)
                DebtTypeID = Convert.ToInt32(DebtTypeComboBox.SelectedValue);
            NewOrderInfo.DocDateTime = Security.GetCurrentDate();

            if (DispDateDateTimePicker.Enabled)
            {
                DispatchDate = DispDateDateTimePicker.Value;
                OrdersManager.CurrentDispatchDate = Convert.ToDateTime(DispatchDate);
            }

            if (PrepareCheckBox.Checked)
            {
                NewOrderInfo.DispatchDate = null;
            }
            else
            {
                NewOrderInfo.DispatchDate = Convert.ToDateTime(DispatchDate);
                OrdersManager.CurrentDispatchDate = Convert.ToDateTime(DispatchDate);
            }

            NewOrderInfo.DoNotDispatch = DoNotDispatchCheckBox.Checked;
            NewOrderInfo.TechDrilling = cbxTechDrilling.Checked;
            NewOrderInfo.QuicklyOrder = cbxQuicklyOrder.Checked;
            NewOrderInfo.ToAssembly = cbIsAssembly.Checked;
            NewOrderInfo.IsNotPaid = cbIsNotPaid.Checked;
            NewOrderInfo.NeedCalculate = NeedCalculateCheckBox.Checked;

            if (NewOrderInfo.IsNewOrder)
            {
                int FirstOperatorID = Security.CurrentUserID;
                int SecondOperatorID = 0;

                OrdersManager.CreateNewDispatch(DispatchDate);
                OrdersManager.CreateNewMainOrder(DispatchDate, Convert.ToDateTime(NewOrderInfo.DocDateTime),
                    Convert.ToInt32(ClientComboBox.SelectedValue), DocNumberTextEdit.Text,
                    DebtTypeID, SampleCheckBox.Checked, PrepareCheckBox.Checked,
                    PriceTypeID, NotesMemoEdit.Text, FirstOperatorID, SecondOperatorID, NewOrderInfo.DoNotDispatch, NewOrderInfo.TechDrilling,
                    NewOrderInfo.QuicklyOrder, NewOrderInfo.ToAssembly, NewOrderInfo.IsNotPaid, NewOrderInfo.NeedCalculate);
            }

            OrdersManager.CurrentClientID = Convert.ToInt32(ClientComboBox.SelectedValue);
            OrdersManager.IsPrepare = PrepareCheckBox.Checked;

            NewOrderInfo.ClientID = Convert.ToInt32(ClientComboBox.SelectedValue);

            NewOrderInfo.PriceType = PriceTypeID;
            NewOrderInfo.DebtType = DebtTypeID;

            NewOrderInfo.IsDebt = DebtCheckBox.Checked;
            NewOrderInfo.IsSample = SampleCheckBox.Checked;
            NewOrderInfo.IsPrepare = PrepareCheckBox.Checked;

            NewOrderInfo.DateEnabled = !PrepareCheckBox.Checked;

            NewOrderInfo.Notes = NotesMemoEdit.Text;

            NewOrderInfo.DocNumber = DocNumberTextEdit.Text;
            if (!DebtCheckBox.Checked)
                DebtDocNumberTextBox.Text = string.Empty;
            NewOrderInfo.DebtDocNumber = DebtDocNumberTextBox.Text;
            NewOrderInfo.bPressOK = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelOrderButton_Click(object sender, EventArgs e)
        {
            //NewOrderInfo.ClientID = 0;
            NewOrderInfo.bPressOK = false;
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

        private void Initialize()
        {
            DebtTypeComboBox.DataSource = OrdersManager.DebtTypesBindingSource;
            DebtTypeComboBox.DisplayMember = "DebtType";
            DebtTypeComboBox.ValueMember = "DebtTypeID";

            ClientComboBox.DataSource = OrdersManager.ClientsBindingSource;
            ClientComboBox.DisplayMember = "ClientName";
            ClientComboBox.ValueMember = "ClientID";

            ClientGroupLabel.Text = "Группа: " + OrdersManager.GetClientGroupName(Convert.ToInt32(ClientComboBox.SelectedValue));

            if (OrdersManager.CurrentDispatchDate != null)
                DispDateDateTimePicker.Value = Convert.ToDateTime(OrdersManager.CurrentDispatchDate);

            if (OrdersManager.CurrentClientID > 0)
            {
                ClientComboBox.SelectedValue = OrdersManager.CurrentClientID;
                ClientGroupLabel.Text = "Группа: " + OrdersManager.GetClientGroupName(Convert.ToInt32(ClientComboBox.SelectedValue));
            }

            label7.Text = "Не вошло в пр-во (ТПС)";
            if (NewOrderInfo.ProductionDate != DBNull.Value)
                label7.Text = "Вход в пр-во (ТПС): " + Convert.ToDateTime(NewOrderInfo.ProductionDate);
            PrepareCheckBox.Checked = OrdersManager.IsPrepare;
            NewOrderInfo.MovePrepare = false;

            if (!NewOrderInfo.IsNewOrder && !NewOrderInfo.IsDoubleOrder)
            {
                if (NewOrderInfo.OrderStatus == Modules.ZOV.NewOrderInfo.OrderStatusType.EditOrder)
                    label3.Text = "Кухня на отгрузке";
                if (NewOrderInfo.OrderStatus == Modules.ZOV.NewOrderInfo.OrderStatusType.PrepareOrder)
                    label3.Text = "Предварительно";
                //установка параметров в контролы
                NotesMemoEdit.Text = NewOrderInfo.Notes;
                NeedCalculateCheckBox.Checked = NewOrderInfo.NeedCalculate;
                DoNotDispatchCheckBox.Checked = NewOrderInfo.DoNotDispatch;
                cbxTechDrilling.Checked = NewOrderInfo.TechDrilling;
                cbxQuicklyOrder.Checked = NewOrderInfo.QuicklyOrder;
                DebtDocNumberTextBox.Text = NewOrderInfo.DebtDocNumber;
                cbIsAssembly.Checked = NewOrderInfo.ToAssembly;
                cbIsNotPaid.Checked = NewOrderInfo.IsNotPaid;
                if (NewOrderInfo.IsDebt)
                {
                    DebtCheckBox.Checked = true;
                    DebtTypeComboBox.SelectedValue = NewOrderInfo.DebtType;
                }

                DocNumberTextEdit.Text = NewOrderInfo.DocNumber;
                SampleCheckBox.Checked = NewOrderInfo.IsSample;
                PrepareCheckBox.Checked = NewOrderInfo.IsPrepare;

                if (NewOrderInfo.PriceType == 0)
                    RetailPriceRadio.Checked = true;
                if (NewOrderInfo.PriceType == 1)
                    WholePriceRadio.Checked = true;
                if (NewOrderInfo.PriceType == 2)
                    WallPriceRadio.Checked = true;

                if (!NewOrderInfo.IsPrepare)
                    DispDateDateTimePicker.Value = Convert.ToDateTime(NewOrderInfo.DispatchDate);

                ClientComboBox.SelectedValue = NewOrderInfo.ClientID;
                ClientGroupLabel.Text = "Группа: " + OrdersManager.GetClientGroupName(Convert.ToInt32(ClientComboBox.SelectedValue));
            }
        }

        private void DebtCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DebtTypeComboBox.Enabled = DebtCheckBox.Checked;
            NeedCalculateCheckBox.Enabled = DebtCheckBox.Checked;
            DebtDocNumberTextBox.Enabled = DebtCheckBox.Checked;
        }

        private void ClientGroupsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ClientGroupsComboBox.ValueMember != "")
            //    OrdersManager.FilterClients(Convert.ToInt32(ClientGroupsComboBox.SelectedValue));
        }

        private void ClientGroupsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //OrdersManager.FilterClients(Convert.ToInt32(ClientGroupsComboBox.SelectedValue));
        }

        private void PrepareCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DispDateDateTimePicker.Enabled = !PrepareCheckBox.Checked;
            NewOrderInfo.MovePrepare = !PrepareCheckBox.Checked;
        }

        private void GetDocButton_Click(object sender, EventArgs e)
        {
            DocNumberTextEdit.Text = OrdersManager.GetDocNumberFromExcel();
            if (DocNumberTextEdit.Text.Length == 0)
                return;
            GetOrderInfo();
        }

        private void GetOrderInfo()
        {
            object DispatchDate = null;
            int ClientID = -1;
            int MainOrderID = -1;
            int DebtTypeID = 0;
            int PriceTypeID = -1;

            bool IsSample = false;
            bool IsPrepare = false;
            bool DoNotDispatch = false;
            bool TechDrilling = false;
            bool QuicklyOrder = false;
            string Notes = string.Empty;
            string DocNumber = string.Empty;
            string DebtDocNumber = string.Empty;
            bool NeedCalculate = false;

            int OrderStatus = OrdersManager.IsDocNumberExist(DocNumberTextEdit.Text, ref MainOrderID);
            NewOrderInfo.MainOrderID = MainOrderID;
            NewOrderInfo.IsDoubleOrder = DoubleOrderCheckBox.Checked;
            switch (OrderStatus)
            {
                case -1:
                    NewOrderInfo.IsNewOrder = true;
                    label3.Text = "Новая кухня";
                    NewOrderInfo.OrderStatus = Modules.ZOV.NewOrderInfo.OrderStatusType.NewOrder;
                    break;
                case 0:
                    NewOrderInfo.IsNewOrder = false;
                    OrdersManager.EditMainOrder(MainOrderID, ref DocNumber, ref ClientID, ref PriceTypeID, ref DebtTypeID, ref IsSample, ref IsPrepare, ref Notes,
                        ref DebtDocNumber, ref NeedCalculate, ref DoNotDispatch, ref TechDrilling, ref QuicklyOrder);
                    OrdersManager.GetDispatchDate(MainOrderID, ref DispatchDate);

                    //установка параметров в контролы
                    if (DebtTypeID > 0)
                    {
                        DebtCheckBox.Checked = true;
                        DebtTypeComboBox.SelectedValue = DebtTypeID;
                    }
                    if (PriceTypeID == 0)
                        RetailPriceRadio.Checked = true;
                    if (PriceTypeID == 1)
                        WholePriceRadio.Checked = true;
                    if (PriceTypeID == 2)
                        WallPriceRadio.Checked = true;
                    NotesMemoEdit.Text = Notes;
                    NeedCalculateCheckBox.Checked = NeedCalculate;
                    DoNotDispatchCheckBox.Checked = DoNotDispatch;
                    cbxTechDrilling.Checked = TechDrilling;
                    cbxQuicklyOrder.Checked = QuicklyOrder;
                    DocNumberTextEdit.Text = DocNumber;
                    DebtDocNumberTextBox.Text = DebtDocNumber;
                    SampleCheckBox.Checked = IsSample;
                    //PrepareCheckBox.Checked = IsPrepare;
                    //DispDateDateTimePicker.Value = Convert.ToDateTime(DispatchDate);

                    label3.Text = "Кухня на отгрузке " + Convert.ToDateTime(DispatchDate).ToString("dd MMMM yyyy");
                    NewOrderInfo.OrderStatus = Modules.ZOV.NewOrderInfo.OrderStatusType.EditOrder;
                    OrdersManager.CurrentClientID = Convert.ToInt32(ClientComboBox.SelectedValue);
                    OrdersManager.IsPrepare = PrepareCheckBox.Checked;
                    if (DispDateDateTimePicker.Enabled)
                    {
                        DispatchDate = DispDateDateTimePicker.Value;
                        OrdersManager.CurrentDispatchDate = Convert.ToDateTime(DispatchDate);
                    }

                    if (PrepareCheckBox.Checked)
                    {
                        NewOrderInfo.DispatchDate = null;
                    }
                    else
                    {
                        NewOrderInfo.DispatchDate = Convert.ToDateTime(DispatchDate);
                        OrdersManager.CurrentDispatchDate = Convert.ToDateTime(DispatchDate);
                    }
                    NewOrderInfo.ClientID = Convert.ToInt32(ClientComboBox.SelectedValue);
                    NewOrderInfo.PriceType = PriceTypeID;
                    NewOrderInfo.DebtType = DebtTypeID;
                    NewOrderInfo.DoNotDispatch = DoNotDispatchCheckBox.Checked;
                    NewOrderInfo.TechDrilling = cbxTechDrilling.Checked;
                    NewOrderInfo.QuicklyOrder = cbxQuicklyOrder.Checked;
                    NewOrderInfo.ToAssembly = cbIsAssembly.Checked;
                    NewOrderInfo.IsNotPaid = cbIsNotPaid.Checked;
                    NewOrderInfo.NeedCalculate = NeedCalculateCheckBox.Checked;
                    NewOrderInfo.IsDebt = DebtCheckBox.Checked;
                    NewOrderInfo.IsSample = SampleCheckBox.Checked;
                    NewOrderInfo.IsPrepare = PrepareCheckBox.Checked;
                    NewOrderInfo.DateEnabled = !PrepareCheckBox.Checked;
                    NewOrderInfo.Notes = NotesMemoEdit.Text;
                    NewOrderInfo.DocNumber = DocNumberTextEdit.Text;
                    if (!DebtCheckBox.Checked)
                        DebtDocNumberTextBox.Text = string.Empty;
                    NewOrderInfo.DebtDocNumber = DebtDocNumberTextBox.Text;

                    //if (NewOrderInfo.IsDoubleOrder)
                    //{
                    //    FormEvent = eClose;
                    //    AnimateTimer.Enabled = true;
                    //}
                    break;
                case 1:
                    NewOrderInfo.IsNewOrder = false;
                    OrdersManager.MoveToPrepareOrder(MainOrderID);
                    OrdersManager.EditMainOrder(MainOrderID, ref DocNumber, ref ClientID, ref PriceTypeID, ref DebtTypeID, ref IsSample, ref IsPrepare, ref Notes,
                        ref DebtDocNumber, ref NeedCalculate, ref DoNotDispatch, ref TechDrilling, ref QuicklyOrder);

                    //установка параметров в контролы
                    if (DebtTypeID > 0)
                    {
                        DebtCheckBox.Checked = true;
                        DebtTypeComboBox.SelectedValue = DebtTypeID;
                    }
                    if (PriceTypeID == 0)
                        RetailPriceRadio.Checked = true;
                    if (PriceTypeID == 1)
                        WholePriceRadio.Checked = true;
                    if (PriceTypeID == 2)
                        WallPriceRadio.Checked = true;
                    NotesMemoEdit.Text = Notes;
                    NeedCalculateCheckBox.Checked = NeedCalculate;
                    DoNotDispatchCheckBox.Checked = DoNotDispatch;
                    cbxTechDrilling.Checked = TechDrilling;
                    cbxQuicklyOrder.Checked = QuicklyOrder;
                    DocNumberTextEdit.Text = DocNumber;
                    DebtDocNumberTextBox.Text = DebtDocNumber;
                    SampleCheckBox.Checked = IsSample;
                    PrepareCheckBox.Checked = IsPrepare;

                    label3.Text = "Предварительно";
                    NewOrderInfo.OrderStatus = Modules.ZOV.NewOrderInfo.OrderStatusType.PrepareOrder;
                    OrdersManager.CurrentClientID = Convert.ToInt32(ClientComboBox.SelectedValue);
                    OrdersManager.IsPrepare = PrepareCheckBox.Checked;
                    if (DispDateDateTimePicker.Enabled)
                    {
                        DispatchDate = DispDateDateTimePicker.Value;
                        OrdersManager.CurrentDispatchDate = Convert.ToDateTime(DispatchDate);
                    }

                    if (PrepareCheckBox.Checked)
                    {
                        NewOrderInfo.DispatchDate = null;
                    }
                    else
                    {
                        NewOrderInfo.DispatchDate = Convert.ToDateTime(DispatchDate);
                        OrdersManager.CurrentDispatchDate = Convert.ToDateTime(DispatchDate);
                    }
                    NewOrderInfo.ClientID = Convert.ToInt32(ClientComboBox.SelectedValue);
                    NewOrderInfo.PriceType = PriceTypeID;
                    NewOrderInfo.DebtType = DebtTypeID;
                    NewOrderInfo.DoNotDispatch = DoNotDispatchCheckBox.Checked;
                    NewOrderInfo.TechDrilling = cbxTechDrilling.Checked;
                    NewOrderInfo.QuicklyOrder = cbxQuicklyOrder.Checked;
                    NewOrderInfo.ToAssembly = cbIsAssembly.Checked;
                    NewOrderInfo.IsNotPaid = cbIsNotPaid.Checked;
                    NewOrderInfo.NeedCalculate = NeedCalculateCheckBox.Checked;
                    NewOrderInfo.IsDebt = DebtCheckBox.Checked;
                    NewOrderInfo.IsSample = SampleCheckBox.Checked;
                    NewOrderInfo.IsPrepare = PrepareCheckBox.Checked;
                    NewOrderInfo.DateEnabled = !PrepareCheckBox.Checked;
                    NewOrderInfo.Notes = NotesMemoEdit.Text;
                    NewOrderInfo.DocNumber = DocNumberTextEdit.Text;
                    if (!DebtCheckBox.Checked)
                        DebtDocNumberTextBox.Text = string.Empty;
                    NewOrderInfo.DebtDocNumber = DebtDocNumberTextBox.Text;

                    //if (NewOrderInfo.IsDoubleOrder)
                    //{
                    //    FormEvent = eClose;
                    //    AnimateTimer.Enabled = true;
                    //}
                    break;
                default:
                    break;
            }

            label3.Left = panel4.Width / 2 - label3.Width / 2;
        }

        private void ClientsButton_Click(object sender, EventArgs e)
        {

        }

        private void ClientComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ClientGroupLabel.Text = "Группа: " + OrdersManager.GetClientGroupName(Convert.ToInt32(ClientComboBox.SelectedValue));
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ClientComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ClientGroupLabel.Text = "Группа: " + OrdersManager.GetClientGroupName(Convert.ToInt32(ClientComboBox.SelectedValue));
        }

        private void FindClientButton_Click(object sender, EventArgs e)
        {
            String ClientText = OrdersManager.ExportClient(Clipboard.GetText());

            NewOrderInfo.ClientID = Convert.ToInt32(ClientComboBox.SelectedValue);

            ClientGroupLabel.Text = "Группа: " + OrdersManager.GetClientGroupName(Convert.ToInt32(ClientComboBox.SelectedValue));
        }

        private void ClientComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ClientComboBox.SelectedIndex < 0 || ClientComboBox.SelectedValue == string.Empty)
            //    return;
            if (ClientComboBox.ValueMember == "" || ClientComboBox.SelectedIndex == -1)
                return;

            ClientGroupLabel.Text = "Группа: " + OrdersManager.GetClientGroupName(Convert.ToInt32(ClientComboBox.SelectedValue));
        }

        private void DoubleOrderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NewOrderInfo.IsDoubleOrder = DoubleOrderCheckBox.Checked;
        }
    }
}
