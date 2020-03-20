using System;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ExpeditionLabelCheckForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        DataTable EventsDataTable;

        LightStartForm LightStartForm;
        bool CanAction = false;
        int UserID = 0;
        Form TopForm = null;
        bool bClearNextPackage = false;
        public ExpeditionCheckLabel CheckLabel;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();


        public ExpeditionLabelCheckForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            EventsDataTable = new DataTable();

            //ScanEvents.SetEventsDataTable(EventsDataTable);

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void LabelCheckProfilForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ReAutorizationForm ReAutorizationForm = new ReAutorizationForm(this);

            TopForm = ReAutorizationForm;
            ReAutorizationForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            bool PressOK = ReAutorizationForm.PressOK;
            UserID = ReAutorizationForm.UserID;
            ReAutorizationForm.Dispose();
            TopForm = null;

            if (PressOK)
            {
                CheckLabel.UserID = UserID;
                CanAction = true;
            }
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

                    //ScanEvents.AddEvent(EventsDataTable, "Закрыл форму сканирования на экспедицию", 0);

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
                    //ScanEvents.AddEvent(EventsDataTable, "Закрыл форму сканирования на экспедицию", 0);


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
            //ScanEvents.SaveEvents(EventsDataTable);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            CheckLabel = new ExpeditionCheckLabel(ref FrontsPackContentDataGrid, ref DecorPackContentDataGrid, ref PackagesDataGrid);

            OrderDateLabel.Text = "";
            GroupLabel.Text = "";
        }

        private void BarcodeTextBox_Leave(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
            CheckTimer.Enabled = true;
        }

        private void NavigateMenuCloseButton_MouseUp(object sender, MouseEventArgs e)
        {
            BarcodeTextBox.Focus();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
            if (!BarcodeTextBox.Focused)
            {
                BarcodeTextBox.Focus();
            }
            //CheckTimer.Enabled = false;
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!CanAction)
                return;
            if (e.KeyCode == Keys.Enter)
            {
                ErrorPackLabel.Visible = false;
                BarcodeLabel.Text = "";
                CheckPicture.Visible = false;

                CheckLabel.Clear();

                if (BarcodeTextBox.Text.Length < 12)
                {
                    //ScanEvents.AddEvent(EventsDataTable, "Ошибка: неверный штрихкод " + BarcodeLabel.Text, 0);
                    BarcodeTextBox.Clear();
                    OrderDateLabel.Text = "";
                    GroupLabel.Text = "";
                    bClearNextPackage = false;
                    return;
                }

                if (bClearNextPackage)
                {
                    int TrayID = 0;
                    CheckLabel.RemovePackageFromTray(Convert.ToInt32(BarcodeTextBox.Text.Substring(3, 9)), ref TrayID);
                    ErrorPackLabel.Visible = true;
                    ErrorPackLabel.Text = "Упаковка №" + Convert.ToInt32(BarcodeTextBox.Text.Substring(3, 9)) + " убрана с поддона №" + TrayID;
                    bClearNextPackage = false;
                    BarcodeTextBox.Clear();

                    return;
                }

                if (string.Equals(BarcodeTextBox.Text, "000000000000"))
                {
                    ErrorPackLabel.Visible = true;
                    ErrorPackLabel.Text = "Следующая отсканированная упаковка будет убрана с поддона";
                    bClearNextPackage = true;
                    BarcodeTextBox.Clear();

                    return;
                }
                else
                    bClearNextPackage = false;

                BarcodeLabel.Text = BarcodeTextBox.Text;

                BarcodeTextBox.Clear();

                string Prefix = BarcodeLabel.Text.Substring(0, 3);

                //ScanEvents.AddEvent(EventsDataTable, "Сканируется этикетка " + BarcodeLabel.Text, 0);

                if (CheckLabel.IsPackageLabel(BarcodeLabel.Text))
                {
                    //if (Prefix == "001")
                    //{
                    //    ScanEvents.AddEvent(EventsDataTable, "Отсканировано: этикетка упаковки №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) +
                    //        ", пр-во ЗОВ-Профиль, экспедиция ЗОВ", 1);
                    //}
                    //if (Prefix == "002")
                    //{
                    //    ScanEvents.AddEvent(EventsDataTable, "Отсканировано: этикетка упаковки №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) +
                    //        ", пр-во ЗОВ-ТПС, экспедиция ЗОВ", 1);
                    //}
                    //if (Prefix == "003")
                    //{
                    //    ScanEvents.AddEvent(EventsDataTable, "Отсканировано: этикетка упаковки №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) +
                    //        ", пр-во ЗОВ-Профиль, экспедиция Маркетинг", 2);
                    //}
                    //if (Prefix == "004")
                    //{
                    //    ScanEvents.AddEvent(EventsDataTable, "Отсканировано: этикетка упаковки №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) +
                    //        ", пр-во ЗОВ-ТПС, экспедиция Маркетинг", 2);
                    //}
                    int PackageID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));
                    if (CheckLabel.HasPackages(BarcodeLabel.Text))
                    {
                        string Message = string.Empty;
                        CheckLabel.GetPackageInfo(PackageID);
                        if (Prefix == "001" || Prefix == "002")
                        {
                            //CheckLabel.SetExp(ref EventsDataTable, false, PackageID, ref Message);
                            ////int DebtMainOrderID = CheckLabel.GetDebtMainOrderID(CheckLabel.CurrentMainOrderID);
                            ////if (DebtMainOrderID != -1)
                            ////    CheckLabel.SetExpDebt(DebtMainOrderID);
                            //CheckOrdersStatus.SetStatusZOV(CheckLabel.CurrentMainOrderID, false);
                            //ScanEvents.AddEvent(EventsDataTable, "Выставлен статус для подзаказа №" + CheckLabel.CurrentMainOrderID +
                            //    ", экспедиция ЗОВ", 1);


                            if (CheckLabel.CanPackageExp(PackageID, ref Message))
                            {
                                if (!CheckLabel.SetExp(ref EventsDataTable, false, PackageID, ref Message))
                                {
                                    ErrorPackLabel.Visible = true;
                                    ErrorPackLabel.Text = Message;
                                    CheckPicture.Visible = true;
                                    CheckPicture.Image = Properties.Resources.cancel;
                                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                                    GroupLabel.Text = "";
                                    OrderDateLabel.Text = "";
                                    return;
                                }
                                if (Message.Length > 0)
                                {
                                    ErrorPackLabel.Visible = true;
                                    ErrorPackLabel.Text = Message;
                                }

                                CheckOrdersStatus.SetStatusZOV(CheckLabel.CurrentMainOrderID, false);
                                //ScanEvents.AddEvent(EventsDataTable, "Выставлен статус для подзаказа №" + CheckLabel.CurrentMainOrderID +
                                //    ", экспедиция ЗОВ", 1);
                            }
                            else
                            {
                                ErrorPackLabel.Visible = true;
                                ErrorPackLabel.Text = Message;
                                CheckPicture.Visible = true;
                                CheckPicture.Image = Properties.Resources.cancel;
                                BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                                GroupLabel.Text = "";
                                OrderDateLabel.Text = "";
                                //ScanEvents.AddEvent(EventsDataTable,
                                //    "Упаковка №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) + " не принята на эксп-цию: запрет на эксп-цию, экспедиция Маркетинг", 2);
                                return;
                            }
                        }

                        if (Prefix == "003" || Prefix == "004")
                        {
                            if (CheckLabel.CanPackageExp(PackageID, ref Message))
                            {
                                if (!CheckLabel.SetExp(ref EventsDataTable, true, PackageID, ref Message))
                                {
                                    ErrorPackLabel.Visible = true;
                                    ErrorPackLabel.Text = Message;
                                    CheckPicture.Visible = true;
                                    CheckPicture.Image = Properties.Resources.cancel;
                                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                                    GroupLabel.Text = "";
                                    OrderDateLabel.Text = "";
                                    return;
                                }
                                if (Message.Length > 0)
                                {
                                    ErrorPackLabel.Visible = true;
                                    ErrorPackLabel.Text = Message;
                                }

                                CheckLabel.WriteOffFromStore(PackageID);
                                CheckOrdersStatus.SetStatusMarketingForMainOrder(CheckLabel.CurrentMegaOrderID, CheckLabel.CurrentMainOrderID);
                                //ScanEvents.AddEvent(EventsDataTable, "Выставлен статус для подзаказа №" + CheckLabel.CurrentMainOrderID +
                                //    ", экспедиция Маркетинг", 2);
                            }
                            else
                            {
                                ErrorPackLabel.Visible = true;
                                ErrorPackLabel.Text = Message;
                                CheckPicture.Visible = true;
                                CheckPicture.Image = Properties.Resources.cancel;
                                BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                                GroupLabel.Text = "";
                                OrderDateLabel.Text = "";
                                //ScanEvents.AddEvent(EventsDataTable,
                                //    "Упаковка №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) + " не принята на эксп-цию: запрет на эксп-цию, экспедиция Маркетинг", 2);
                                return;
                            }
                        }
                        CheckLabel.FilterByPackageID(BarcodeLabel.Text);

                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.OK;
                        BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                        GroupLabel.Text = CheckLabel.LabelInfo.Group;
                        OrderDateLabel.Text = CheckLabel.LabelInfo.OrderDate;
                    }
                    else
                    {
                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.cancel;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        GroupLabel.Text = "";
                        OrderDateLabel.Text = "";
                    }
                }
                else
                {
                    //if (Prefix == "005" || Prefix == "006")
                    //{
                    //    ErrorPackLabel.Visible = true;
                    //    ErrorPackLabel.Text = "На экспедицию принимаются только упаковки поштучно";
                    //    CheckPicture.Visible = true;
                    //    CheckPicture.Image = Properties.Resources.cancel;
                    //    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    //    OrderDateLabel.Text = "";
                    //    GroupLabel.Text = "";
                    //    CheckLabel.Clear();
                    //    return;
                    //}
                    //ErrorPackLabel.Visible = true;
                    //ErrorPackLabel.Text = "Такой этикетки в упаковках не существует";
                    //CheckPicture.Visible = true;
                    //CheckPicture.Image = Properties.Resources.cancel;
                    //BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    //OrderDateLabel.Text = "";
                    //GroupLabel.Text = "";
                    //CheckLabel.Clear();
                    //return;
                    if (CheckLabel.IsTrayLabel(BarcodeLabel.Text))
                    {
                        //if (Prefix == "005")
                        //{
                        //    ScanEvents.AddEvent(EventsDataTable, "Отсканировано: этикетка поддона №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) +
                        //        ", экспедиция ЗОВ", 1);
                        //}
                        //if (Prefix == "006")
                        //{
                        //    ScanEvents.AddEvent(EventsDataTable, "Отсканировано: этикетка поддона №" + Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)) +
                        //        ", экспедиция Маркетинг", 2);
                        //}

                        if (CheckLabel.HasTray(BarcodeLabel.Text))
                        {

                            if (Prefix == "005")
                            {
                                CheckLabel.SetExp(BarcodeLabel.Text);
                                CheckOrdersStatus.SetStatusZOV(Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)), true);
                            }

                            if (Prefix == "006")
                            {
                                string Message = string.Empty;

                                if (CheckLabel.CanTrayExp(Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)), ref Message))
                                {
                                    CheckLabel.SetExp(BarcodeLabel.Text);
                                    if (Message.Length > 0)
                                    {
                                        ErrorPackLabel.Visible = true;
                                        ErrorPackLabel.Text = Message;
                                    }
                                    CheckOrdersStatus.SetStatusMarketingForTray(Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9)));
                                }
                                else
                                {
                                    ErrorPackLabel.Visible = true;
                                    ErrorPackLabel.Text = Message;
                                    CheckPicture.Visible = true;
                                    CheckPicture.Image = Properties.Resources.cancel;
                                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                                    OrderDateLabel.Text = "";
                                    GroupLabel.Text = "";
                                    CheckLabel.Clear();
                                    return;
                                }
                            }
                            CheckLabel.FillPackages(BarcodeLabel.Text);

                            CheckPicture.Visible = true;
                            CheckPicture.Image = Properties.Resources.OK;
                            BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                            GroupLabel.Text = CheckLabel.LabelInfo.Group;
                            OrderDateLabel.Text = CheckLabel.LabelInfo.OrderDate;
                        }
                        else
                        {
                            //ScanEvents.AddEvent(EventsDataTable, "Ошибка: в таблице Packages нет упаковок с номером поддона №" + BarcodeLabel.Text, 0);
                            CheckPicture.Visible = true;
                            CheckPicture.Image = Properties.Resources.cancel;
                            BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                            GroupLabel.Text = "";
                            OrderDateLabel.Text = "";
                        }
                    }
                    else
                    {
                        //ScanEvents.AddEvent(EventsDataTable, "Ошибка: в таблице Trays нет записей с номером поддона №" + BarcodeLabel.Text, 0);
                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.cancel;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        OrderDateLabel.Text = "";
                        GroupLabel.Text = "";
                        CheckLabel.Clear();
                        return;
                    }
                }
            }


        }

        private void BarcodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CanAction)
                return;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                if (BarcodeTextBox.Text.Length >= 12 && e.KeyChar != (char)8)
                    e.Handled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
            if (GetActiveWindow() != this.Handle)
            {
                this.Activate();
            }
        }

        private void PackagesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (CheckLabel == null || CheckLabel.IsPackagesTableEmpty)
                return;

            if (PackagesDataGrid.SelectedRows.Count == 0)
                return;

            int PackageID = Convert.ToInt32(PackagesDataGrid.SelectedRows[0].Cells["PackageID"].Value);
            MainOrdersTabControl.TabPages[0].PageVisible = CheckLabel.FillFrontsPackContent(PackageID);
            MainOrdersTabControl.TabPages[1].PageVisible = CheckLabel.FillDecorPackContent(PackageID);
        }
    }
}
