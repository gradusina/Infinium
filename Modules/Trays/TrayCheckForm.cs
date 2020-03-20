using System;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class TrayCheckForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        /// <summary>
        /// Этикетка поддона отсканирована успешно, ошибок нет
        /// </summary>
        bool bCheckTray = false;

        int FormEvent = 0;
        int TotalPackCount = 0;
        int ScanTrayID = 0;

        LightStartForm LightStartForm;

        Form TopForm;
        public Modules.Packages.Trays.CheckTray CheckTray;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();


        public TrayCheckForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void TrayCheckForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Открыл форму формирования поддона");
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            PackagesCountTextBox.Focus();
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
                    CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Закрыл форму формирования поддона");
                    CheckTray.SaveEvents();

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
                    CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Закрыл форму формирования поддона");
                    CheckTray.SaveEvents();

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
            bool OkCancel = Infinium.LightMessageBox.Show(ref TopForm, true, "Точно выйти? Все действия сохранены?", "Внимание");
            if (!OkCancel)
                return;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private int GetChar(KeyEventArgs e)
        {
            int c = -1;

            if (e.KeyCode == Keys.NumPad1 || e.KeyCode == Keys.NumPad2 || e.KeyCode == Keys.NumPad3 || e.KeyCode == Keys.NumPad4 ||
                e.KeyCode == Keys.NumPad5 || e.KeyCode == Keys.NumPad6 || e.KeyCode == Keys.NumPad7 || e.KeyCode == Keys.NumPad8 ||
                e.KeyCode == Keys.NumPad9 || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.D0 || e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 || e.KeyCode == Keys.D4 || e.KeyCode == Keys.D5 || e.KeyCode == Keys.D6 || e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 || e.KeyCode == Keys.D9 || e.KeyCode == Keys.D0)
            {
                switch (e.KeyCode)
                {
                    case Keys.NumPad1:
                        { c = 1; }
                        break;
                    case Keys.NumPad2:
                        { c = 2; }
                        break;
                    case Keys.NumPad3:
                        { c = 3; }
                        break;
                    case Keys.NumPad4:
                        { c = 4; }
                        break;
                    case Keys.NumPad5:
                        { c = 5; }
                        break;
                    case Keys.NumPad6:
                        { c = 6; }
                        break;
                    case Keys.NumPad7:
                        { c = 7; }
                        break;
                    case Keys.NumPad8:
                        { c = 8; }
                        break;
                    case Keys.NumPad9:
                        { c = 9; }
                        break;
                    case Keys.NumPad0:
                        { c = 0; }
                        break;


                    case Keys.D1:
                        { c = 1; }
                        break;
                    case Keys.D2:
                        { c = 2; }
                        break;
                    case Keys.D3:
                        { c = 3; }
                        break;
                    case Keys.D4:
                        { c = 4; }
                        break;
                    case Keys.D5:
                        { c = 5; }
                        break;
                    case Keys.D6:
                        { c = 6; }
                        break;
                    case Keys.D7:
                        { c = 7; }
                        break;
                    case Keys.D8:
                        { c = 8; }
                        break;
                    case Keys.D9:
                        { c = 9; }
                        break;
                    case Keys.D0:
                        { c = 0; }
                        break;
                }


            }
            return c;
        }


        private void Initialize()
        {
            CheckTray = new Modules.Packages.Trays.CheckTray(ref PackagesDataGrid, ref FrontsPackContentDataGrid, ref DecorPackContentDataGrid,
                ref FrontsContentDataGrid, ref DecorContentDataGrid);

            ClientsComboBox.DataSource = CheckTray.ClientsBindingSource;
            ClientsComboBox.ValueMember = "ClientID";
            ClientsComboBox.DisplayMember = "ClientName";

            ClientLabel.Text = string.Empty;
            MegaOrderNumberLabel.Text = string.Empty;
            MainOrderNumberLabel.Text = string.Empty;
            DispatchDateLabel.Text = string.Empty;
            OrderDateLabel.Text = string.Empty;
            ProductTypeLabel.Text = string.Empty;
            PackNumberLabel.Text = string.Empty;
            TotalLabel.Text = string.Empty;
            GroupLabel.Text = string.Empty;
        }

        private void NavigateMenuCloseButton_MouseUp(object sender, MouseEventArgs e)
        {
            BarcodeTextBox.Focus();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!BarcodeTextBox.Focused)
            {
                BarcodeTextBox.Focus();
            }
        }

        private void BarcodeTextBox_Leave(object sender, EventArgs e)
        {
            //if (CheckTimer.Enabled)
            //    BarcodeTextBox.Focus();
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BarcodeLabel.Text = "";
                CheckPicture.Visible = false;

                CheckTray.Clear();

                if (BarcodeTextBox.Text.Length < 12)
                {
                    CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Ошибка: неверный штрихкод " + BarcodeLabel.Text);
                    BarcodeTextBox.Clear();

                    ClientLabel.Text = string.Empty;
                    MegaOrderNumberLabel.Text = string.Empty;
                    MainOrderNumberLabel.Text = string.Empty;
                    DispatchDateLabel.Text = string.Empty;
                    OrderDateLabel.Text = string.Empty;
                    ProductTypeLabel.Text = string.Empty;
                    PackNumberLabel.Text = string.Empty;
                    TotalLabel.Text = string.Empty;
                    GroupLabel.Text = string.Empty;

                    return;
                }

                BarcodeLabel.Text = BarcodeTextBox.Text;

                BarcodeTextBox.Clear();

                bool BadGroupOrClient = false;
                int GroupType = 0;
                int FactoryID = 0;
                int PackageID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));
                string Prefix = BarcodeLabel.Text.Substring(0, 3);

                if (Prefix == "001")
                {
                    GroupType = 1;
                    FactoryID = 1;
                }
                if (Prefix == "002")
                {
                    GroupType = 1;
                    FactoryID = 2;
                }
                if (Prefix == "003")
                {
                    GroupType = 2;
                    FactoryID = 1;
                }
                if (Prefix == "004")
                {
                    GroupType = 2;
                    FactoryID = 2;
                }

                CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Сканируется этикетка " + BarcodeLabel.Text);

                #region Проверка на неверный префикс
                if (Prefix != "001" && Prefix != "002" && Prefix != "003" && Prefix != "004")
                {
                    CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty, "Сканирование упаковки. Неверный префикс штрихкода! Ожидалась этикетка упаковки");
                    ErrorPackLabel.Visible = true;
                    ErrorPackLabel.Text = "Штрихкод имеет неверный префикс. Допустимые префиксы 001, 002, 003, 004";
                    return;
                }
                #endregion

                #region Находилась ли упаковка на поддоне ранее
                int OldTrayID = CheckTray.IsPackageOnTray(GroupType, PackageID);
                if (OldTrayID > -1)
                {
                    //CheckTray.AddEvent(true, GroupType, FactoryID, -1, PackageID, string.Empty, string.Empty, "Упаковка уже находится на поддоне " + OldTrayID + ". Продолжить?");
                    //bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    //            "Упаковка уже находится на поддоне " + OldTrayID + ". Всё равно продолжить?", "Внимание");

                    //if (!OKCancel)
                    //{
                    //    CheckTray.AddEvent(false, GroupType, FactoryID, -1, PackageID, string.Empty, string.Empty, "Упаковка уже находится на поддоне " + OldTrayID + ". Задумался");
                    //    CheckPicture.Image = Properties.Resources.cancel;
                    //    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    //    ErrorPackLabel.Text = "Упаковка уже находится на поддоне " + OldTrayID;
                    //    ErrorPackLabel.Visible = true;
                    //    CheckTray.SetGridColor(CheckTray.LabelInfo.ProductType, false);
                    //    return;
                    //}
                    CheckTray.AddEvent(true, GroupType, FactoryID, -1, PackageID, string.Empty, string.Empty, "Упаковка уже находится на поддоне " + OldTrayID + ". Игнорирование!");
                }
                #endregion

                if (CheckTray.CheckPackBarcode(BarcodeLabel.Text))
                {
                    BackToSelectButton.Visible = false;
                    CheckPicture.Visible = true;
                    CheckTray.GetPackLabelInfo(BarcodeLabel.Text);

                    ClientLabel.Text = CheckTray.LabelInfo.ClientName;
                    MegaOrderNumberLabel.Text = CheckTray.LabelInfo.MegaOrderNumber;
                    MainOrderNumberLabel.Text = CheckTray.LabelInfo.MainOrderNumber;
                    DispatchDateLabel.Text = CheckTray.LabelInfo.DispatchDate;
                    OrderDateLabel.Text = CheckTray.LabelInfo.OrderDate;
                    ProductTypeLabel.Text = CheckTray.LabelInfo.ProductType;
                    PackNumberLabel.Text = CheckTray.LabelInfo.CurrentPackNumber;
                    DispatchDateLabel.ForeColor = CheckTray.LabelInfo.DispatchDateColor;
                    GroupLabel.Text = CheckTray.LabelInfo.Group;

                    #region Верная ли группа, ЗОВ или Маркетинг
                    if (GroupType != CheckTray.CurrentGroupType)
                    {
                        BadGroupOrClient = true;
                        CheckTray.AddEvent(true, GroupType, FactoryID, -1, PackageID, string.Empty, string.Empty, "Сканирование упаковки. Неверная группа!");

                        CheckTray.Clear();
                        CheckPicture.Image = Properties.Resources.cancel;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        if (CheckTray.CurrentGroupType == 1)
                            ErrorPackLabel.Text = "Неверная группа: ожидалась этикетка ЗОВ";
                        if (CheckTray.CurrentGroupType == 2)
                            ErrorPackLabel.Text = "Неверная группа: ожидалась этикетка Маркетинг";
                        ErrorPackLabel.Visible = true;
                        CheckTray.SetGridColor(CheckTray.LabelInfo.ProductType, false);
                        //return;
                    }
                    #endregion

                    #region Если группа Маркетинг, то проверить соответствие клиента
                    if (GroupType == CheckTray.CurrentGroupType && CheckTray.CurrentGroupType == 2 &&
                        CheckTray.IsWrongClient(Convert.ToInt32(CheckTray.LabelInfo.MainOrderNumber)))
                    {
                        BadGroupOrClient = true;
                        CheckTray.AddEvent(true, GroupType, FactoryID, -1, PackageID, string.Empty, string.Empty, "Сканирование упаковки. Неверный клиент!");

                        CheckPicture.Image = Properties.Resources.cancel;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        ErrorPackLabel.Text = "Неверный клиент: ожидался клиент " + CheckTray.CurrentClientName;
                        ErrorPackLabel.Visible = true;
                        CheckTray.SetGridColor(CheckTray.LabelInfo.ProductType, false);
                        //return;
                    }
                    #endregion

                    //if (!BadGroupOrClient)
                    //{
                    //    if (CheckTray.IsNotPacked(GroupType, PackageID))
                    //    {
                    //        CheckTray.SetPacked(GroupType, PackageID);

                    //    }

                    //    if (CheckTray.CurrentGroupType == 2)
                    //    {
                    //        int MainOrderID = CheckTray.GetMainOrderID(GroupType, PackageID);
                    //        int MegaOrderID = CheckTray.GetMegaOrderID(MainOrderID);
                    //        if (CheckTray.LabelInfo.MainOrderID > 0)
                    //        {
                    //            CheckOrdersStatus.SetMainOrderStatus(true, CheckTray.LabelInfo.MainOrderID, false);
                    //            CheckOrdersStatus.SetMegaOrderStatus(CheckTray.LabelInfo.MegaOrderID);
                    //        }
                    //    }
                    //}

                    CheckTray.AddToTray(GroupType, FactoryID, PackageID);
                    CheckTray.SetTotalLabel(TotalPackCount);
                    TotalLabel.Text = CheckTray.LabelInfo.PackedToTotal;
                    TotalLabel.ForeColor = CheckTray.LabelInfo.TotalLabelColor;

                    string PackageInfo = CheckTray.SetPackageInfo(
                        CheckTray.LabelInfo.ClientID,
                        CheckTray.LabelInfo.MegaOrderID,
                        CheckTray.LabelInfo.MainOrderID,
                        CheckTray.LabelInfo.Dispatch,
                        CheckTray.LabelInfo.DocDateTime,
                        CheckTray.LabelInfo.Product);

                    if (!BadGroupOrClient)
                    {
                        CheckTray.AddEvent(false, GroupType, FactoryID, -1, PackageID, PackageInfo, string.Empty, "Упаковка добавлена на поддон");
                        CheckPicture.Image = Properties.Resources.OK;
                        BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                        ErrorPackLabel.Visible = false;
                        CheckTray.SetGridColor(CheckTray.LabelInfo.ProductType, true);
                    }
                    else
                    {
                        CheckTray.AddEvent(true, GroupType, FactoryID, -1, PackageID, string.Empty, string.Empty, "Снять с поддона!");
                    }
                }
                else
                {
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    ErrorPackLabel.Text = "Такой этикетки не существует в базе";
                    ErrorPackLabel.Visible = true;

                    ClientLabel.Text = string.Empty;
                    MegaOrderNumberLabel.Text = string.Empty;
                    MainOrderNumberLabel.Text = string.Empty;
                    DispatchDateLabel.Text = string.Empty;
                    OrderDateLabel.Text = string.Empty;
                    ProductTypeLabel.Text = string.Empty;
                    PackNumberLabel.Text = string.Empty;
                    TotalLabel.Text = string.Empty;
                    GroupLabel.Text = string.Empty;

                    CheckTray.SetGridColor(CheckTray.LabelInfo.ProductType, false);
                }
            }
        }

        private void BarcodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
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
            //if (!FormOnTop)
            //    return;
            //if (GetActiveWindow() != this.Handle && CheckTimer.Enabled)
            //{
            //    this.Activate();
            //}
        }

        private void PackagesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (CheckTray == null || CheckTray.PackagesBindingSource.Count < 1)
                return;

            int PackageID = Convert.ToInt32(((DataRowView)CheckTray.PackagesBindingSource.Current)["PackageID"]);
            string Group = ((DataRowView)CheckTray.PackagesBindingSource.Current)["Group"].ToString();

            CheckTray.FilterPackages(PackageID, Group);
        }

        private void PackagesDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if ((PackagesDataGrid.Columns[e.ColumnIndex].GetType().Equals(typeof(DataGridViewImageColumn))))
            {
                CheckTray.RemoveCurrent();
                if (CheckTray.PackagesBindingSource.Count == 0)
                {
                    CheckTray.CleareTables();
                    return;
                }
            }

            PackagesDataGrid.Cursor = Cursors.Default;
        }

        private void PackagesDataGrid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            PackagesDataGrid.Cursor = Cursors.Default;
        }

        private void PackagesDataGrid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || PackagesDataGrid.IsCurrentCellInEditMode)
                return;

            if (PackagesDataGrid.Columns[e.ColumnIndex].GetType().Equals(typeof(DataGridViewImageColumn)))
                PackagesDataGrid.Cursor = Cursors.Hand;
            else
                PackagesDataGrid.Cursor = Cursors.Default;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (CheckTray == null)
                return;

            if (kryptonCheckSet1.CheckedButton.Name == "ZOVCheckButton")
            {
                ClientsComboBox.Visible = false;
                MarketClientLabel.Visible = false;
                CheckTray.CurrentGroupType = 1;
            }
            if (kryptonCheckSet1.CheckedButton.Name == "MarketingCheckButton")
            {
                ClientsComboBox.Visible = true;
                MarketClientLabel.Visible = true;
                CheckTray.CurrentGroupType = 2;
            }
            PackagesCountTextBox.Focus();
        }

        private void BackToSelectButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Назад; ScanPackagesPanel");
            if (CheckTray.ScanPackgesCount != 0)
            {
                CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty,
                    "Вернуться в предыдущее меню невозможно, так как одна или несколько этикеток уже были отстрелены; ScanPackagesPanel; return");
                Infinium.LightMessageBox.Show(ref TopForm, false, "Вернуться в предыдущее меню невозможно, так как одна или несколько этикеток уже были отстрелены", "Внимание");
                return;
            }

            CheckTimer.Enabled = false;
            CheckTray.CleareTables();

            ClientLabel.Text = string.Empty;
            MegaOrderNumberLabel.Text = string.Empty;
            MainOrderNumberLabel.Text = string.Empty;
            DispatchDateLabel.Text = string.Empty;
            OrderDateLabel.Text = string.Empty;
            ProductTypeLabel.Text = string.Empty;
            PackNumberLabel.Text = string.Empty;
            TotalLabel.Text = string.Empty;
            GroupLabel.Text = string.Empty;
            ErrorPackLabel.Visible = false;

            FrontsPackContentDataGrid.StateCommon.Background.Color1 = Color.White;
            FrontsPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
            FrontsPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.White;
            FrontsPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;

            DecorPackContentDataGrid.StateCommon.Background.Color1 = Color.White;
            DecorPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
            DecorPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.White;
            DecorPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;

            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Возвращение с начальное меню; SelectPanel.BringToFront()");
            SelectPanel.BringToFront();

            PackagesCountTextBox.Clear();
            PackagesCountTextBox.Focus();
        }

        private void NextToCheckPackagesButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Далее; ScanPackagesPanel");
            if (CheckTray.ScanPackgesCount < 1)
            {
                CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty, "Не отсканировано ни одной этикетки; ScanPackagesPanel; return");
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не отсканировано ни одной этикетки", "Внимание");
                return;
            }

            CheckTimer.Enabled = false;

            if (CheckTray.ScanPackgesCount != TotalPackCount)
            {
                CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty,
                    "Количество отсканированных этикеток не равно первоначально заявленному. Продолжить?; ScanPackagesPanel");
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                                "Количество отсканированных этикеток не равно " + TotalPackCount + ". Всё равно продолжить?", "Внимание");

                if (!OKCancel)
                {
                    CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty,
                        "Количество отсканированных этикеток не равно первоначально заявленному. Задумался; ScanPackagesPanel");
                    CheckTimer.Enabled = true;
                    BarcodeTextBox.Focus();
                    return;
                }

                CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty,
                    "Количество отсканированных этикеток не равно первоначально заявленному. Игнорирование!; ScanPackagesPanel");
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            CheckTray.FillPackages();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            CheckTray.Clear();
            ErrorPackLabel.Visible = false;

            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty,
                "Количество отсканированных этикеток не равно первоначально заявленному. Игнорирование!; ScanPackagesPanel; CheckPackagesPanel.BringToFront()");
            CheckPackagesPanel.BringToFront();
        }

        private void BackToScanPackagesButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Назад; CheckPackagesPanel; ScanPackagesPanel.BringToFront()");
            CheckTimer.Enabled = true;
            BarcodeTextBox.Focus();
            ScanPackagesPanel.BringToFront();
            CheckTray.SetTotalLabel(TotalPackCount);
            TotalLabel.Text = CheckTray.LabelInfo.PackedToTotal;

            if (CheckTray.ScanPackgesCount == 0)
                BackToSelectButton.Visible = true;

            CheckPicture.Visible = true;
            CheckPicture.Image = Properties.Resources.cancel;
            BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);

            BarcodeLabel.Text = "";
            ClientLabel.Text = "";
            MegaOrderNumberLabel.Text = "";
            MainOrderNumberLabel.Text = "";
            DispatchDateLabel.Text = "";
            OrderDateLabel.Text = "";
            ProductTypeLabel.Text = "";
            PackNumberLabel.Text = "";
            GroupLabel.Text = "";

            FrontsPackContentDataGrid.StateCommon.Background.Color1 = Color.White;
            FrontsPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
            FrontsPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.White;
            FrontsPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;

            DecorPackContentDataGrid.StateCommon.Background.Color1 = Color.White;
            DecorPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
            DecorPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.White;
            DecorPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
        }

        private void NextToScanPackagesButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, CheckTray.CurrentGroupType, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Далее; SelectPanel");
            if (string.IsNullOrWhiteSpace(PackagesCountTextBox.Text) || Convert.ToInt32(PackagesCountTextBox.Text) == 0)
            {
                CheckTray.AddEvent(true, CheckTray.CurrentGroupType, -1, -1, -1, string.Empty, string.Empty,
                    "Не введено кол-во упаковок на поддоне либо кол-во=0; SelectPanel; return");
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Не введено кол-во упаковок на поддоне либо кол-во равно нулю", "Внимание");
                PackagesCountTextBox.Focus();
                return;
            }

            TotalPackCount = Convert.ToInt32(PackagesCountTextBox.Text);
            CheckTimer.Enabled = true;
            ScanPackagesPanel.BringToFront();
            BarcodeTextBox.Focus();
            CheckTray.CurrentClientID = Convert.ToInt32(ClientsComboBox.SelectedValue);
            CheckTray.CurrentClientName = ClientsComboBox.Text;

            if (CheckTray.CurrentGroupType == 2)
                CheckTray.AddEvent(false, CheckTray.CurrentGroupType, -1, -1, -1, string.Empty, string.Empty,
                    "Выбран клиент: " + CheckTray.CurrentClientName);

            CheckTray.AddEvent(false, CheckTray.CurrentGroupType, -1, -1, -1, string.Empty, string.Empty,
                "Формирование поддона продолжено; SelectPanel; ScanPackagesPanel.BringToFront()");
        }

        private void NextToScanTrayButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Далее; CheckPackagesPanel;");

            if (CheckTray.ScanPackgesCount == 0)
            {
                CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty, "Не отсканировано ни одной этикетки; CheckPackagesPanel; return");
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не отсканировано ни одной этикетки", "Внимание");
                return;
            }

            if (CheckTray.ExcessGroup)
            {
                CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty, "На поддоне имеются лишние упаковки; CheckPackagesPanel; return");
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Удалите лишние упаковки с поддона", "Ошибка");
                return;
            }

            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Формирование поддона продолжено; CheckPackagesPanel; ScanTrayPanel.BringToFront()");
            ScanTrayPanel.BringToFront();
            TrayBarcodeTextBox.Focus();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Сохранить; ScanTrayPanel");
            if (!bCheckTray)
            {
                CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Сохранение не выполнено, поддон не сформирован; bCheckTray = false, return");
                ErrorTrayLabel.Visible = true;
                ErrorTrayLabel.Text = "Сохранение не выполнено, поддон не сформирован";
                return;
            }

            if (!CheckTray.IsNewTray && ScanTrayID != CheckTray.CurrentTrayID)
            {
                CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Сохранение не выполнено, поддон не сформирован; ScanTrayID != CheckTray.CurrentTrayID, return");
                ErrorTrayLabel.Visible = true;
                ErrorTrayLabel.Text = "Сохранение не выполнено, поддон не сформирован: ожидалась этикетка поддона " + CheckTray.CurrentTrayID; ;
                return;
            }

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (CheckTray.SavePackages(TrayBarcodeLabel.Text))
            {
                if (CheckTray.CurrentGroupType == 2)
                {
                    CheckOrdersStatus.SetStatusMarketingForTray(ScanTrayID);
                }
                CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Сохранение выполнено, поддон сформирован; ScanTrayPanel; SuccessPanel.BringToFront()");
                SuccessPanel.BringToFront();
            }
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TrayBarcodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                if (TrayBarcodeTextBox.Text.Length >= 12 && e.KeyChar != (char)8)
                    e.Handled = true;
            }
        }

        private void TrayBarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bCheckTray = false;
                TrayBarcodeLabel.Text = "";
                TrayCheckPicture.Visible = false;

                if (TrayBarcodeTextBox.Text.Length < 12)
                {
                    CheckTray.AddEvent(true, -1, -1, -1, -1, string.Empty, string.Empty, "Ошибка: неверный штрихкод " + TrayBarcodeTextBox.Text);
                    TrayBarcodeTextBox.Clear();
                    return;
                }

                TrayBarcodeLabel.Text = TrayBarcodeTextBox.Text;

                TrayBarcodeTextBox.Clear();

                int GroupType = 0;
                int TrayID = Convert.ToInt32(TrayBarcodeLabel.Text.Substring(3, 9));
                string Prefix = TrayBarcodeLabel.Text.Substring(0, 3);

                if (Prefix != "005" && Prefix != "006")
                {
                    CheckTray.AddEvent(true, -1, -1, TrayID, -1, string.Empty, string.Empty, "Сканирование поддона. Неверный префикс штрихкода! Ожидалась этикетка поддона");
                    ErrorTrayLabel.Visible = true;
                    ErrorTrayLabel.Text = "Штрихкод имеет неверный префикс. Допустимые префиксы 005 и 006";
                }

                if (Prefix == "005")
                {
                    GroupType = 1;
                }
                if (Prefix == "006")
                {
                    GroupType = 2;
                }

                if (CheckTray.CheckTrayBarcode(TrayBarcodeLabel.Text))
                {
                    ScanTrayID = TrayID;

                    if (GroupType != CheckTray.CurrentGroupType)
                    {
                        CheckTray.AddEvent(true, GroupType, -1, TrayID, -1, string.Empty, string.Empty, "Сканирование поддона. Неверная группа!");

                        TrayCheckPicture.Visible = true;
                        TrayCheckPicture.Image = Properties.Resources.cancel;
                        TrayBarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        ErrorTrayLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        if (CheckTray.CurrentGroupType == 1)
                            ErrorTrayLabel.Text = "Неверная группа: ожидалась этикетка ЗОВ";
                        if (CheckTray.CurrentGroupType == 2)
                            ErrorTrayLabel.Text = "Неверная группа: ожидалась этикетка Маркетинг";
                        ErrorTrayLabel.Visible = true;
                        bCheckTray = false;
                        return;
                    }

                    //Если добавляются упаковки на поддон
                    if (!CheckTray.IsNewTray && TrayID != CheckTray.CurrentTrayID)
                    {
                        CheckTray.AddEvent(true, GroupType, -1, TrayID, -1, string.Empty, string.Empty, "Добавление на поддон. Неверная этикетка!");

                        TrayCheckPicture.Visible = true;
                        TrayCheckPicture.Image = Properties.Resources.cancel;
                        TrayBarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        ErrorTrayLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        ErrorTrayLabel.Text = "Неверная этикетка: ожидалась этикетка " + CheckTray.CurrentTrayID;
                        ErrorTrayLabel.Visible = true;
                        bCheckTray = false;
                        return;
                    }

                    //Если формируется новый поддон
                    if (CheckTray.IsNewTray && CheckTray.IsTrayNotEmpty(TrayBarcodeLabel.Text))
                    {
                        CheckTray.AddEvent(true, GroupType, -1, TrayID, -1, string.Empty, string.Empty, "Сканирование этикетки поддона. Поддон уже сформирован!");

                        TrayCheckPicture.Visible = true;
                        TrayCheckPicture.Image = Properties.Resources.cancel;
                        TrayBarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        ErrorTrayLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        ErrorTrayLabel.Text = "Этот поддон уже сформирован. Выберите новую этикетку";
                        ErrorTrayLabel.Visible = true;
                        bCheckTray = false;
                        return;
                    }

                    CheckTray.AddEvent(false, GroupType, -1, TrayID, -1, string.Empty, string.Empty, "Поддон сформирован");

                    TrayCheckPicture.Visible = true;
                    TrayCheckPicture.Image = Properties.Resources.OK;
                    TrayBarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    ErrorTrayLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    ErrorTrayLabel.Text = "Формирование поддона завершено. Для продолжения нажмите Сохранить";
                    ErrorTrayLabel.Visible = true;
                    bCheckTray = true;
                    return;
                }
                else
                {
                    TrayCheckPicture.Visible = true;
                    TrayCheckPicture.Image = Properties.Resources.cancel;
                    ErrorTrayLabel.Visible = true;
                    ErrorTrayLabel.Text = "Данной этикетки не существует в базе. Распечатайте новую этикетку";
                    ErrorTrayLabel.ForeColor = Color.FromArgb(240, 0, 0);
                }
            }
        }

        private void BackToCheckPackagesButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Назад; ScanTrayPanel; CheckPackagesPanel.BringToFront()");
            ErrorTrayLabel.Text = "";
            TrayCheckPicture.Visible = false;

            CheckPackagesPanel.BringToFront();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Продолжить; SuccessPanel; SelectActionPanel.BringToFront()");
            CheckTray.CleareTables();
            PackagesCountTextBox.Clear();
            PackagesCountTextBox.Focus();
            panel2.BringToFront();
            SelectActionPanel.BringToFront();
        }

        private void PackagesDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int MainOrderID = Convert.ToInt32(PackagesDataGrid.Rows[e.RowIndex].Cells["MainOrderID"].Value);
            int GroupType = Convert.ToInt32(PackagesDataGrid.Rows[e.RowIndex].Cells["GroupType"].Value);
            Color BackColor = Color.White;
            Color ForeColor = Color.Black;
            Color SelectionBackColor = Color.FromArgb(31, 158, 0);
            Color SelectionForeColor = Color.White;

            int rowHeaderWidth = PackagesDataGrid.RowHeadersVisible ?
                                     PackagesDataGrid.RowHeadersWidth : 0;
            Rectangle rowBounds = new Rectangle(
                rowHeaderWidth,
                e.RowBounds.Top,
                PackagesDataGrid.Columns.GetColumnsWidth(
                        DataGridViewElementStates.Visible) -
                        PackagesDataGrid.HorizontalScrollingOffset + 1,
               e.RowBounds.Height);

            if (GroupType != CheckTray.CurrentGroupType)
            {
                BackColor = Color.Red;
                ForeColor = Color.White;

                SelectionBackColor = Color.Red;
                SelectionForeColor = Color.White;
            }

            if (GroupType == 2 && CheckTray.IsWrongClient(MainOrderID))
            {
                BackColor = Color.Red;
                ForeColor = Color.White;

                SelectionBackColor = Color.Red;
                SelectionForeColor = Color.White;
            }

            PackagesDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = BackColor;
            PackagesDataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = ForeColor;

            PackagesDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = SelectionBackColor;
            PackagesDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = SelectionForeColor;

            if (PackagesDataGrid.CurrentCellAddress.Y == e.RowIndex)
            {
                // Paint the focus rectangle.
                e.DrawFocus(rowBounds, true);
            }
        }

        private void CreateTrayButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Сформировать новый поддон");
            SelectPanel.BringToFront();
            PackagesCountTextBox.Focus();
        }

        private void ChangeTrayButton_Click(object sender, EventArgs e)
        {
            CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Нажата кнопка Добавить упаковки в существующий поддон");
            panel6.BringToFront();
            ChangeTrayBracodeTextBox.Focus();
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            SelectPanel.BringToFront();
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            CheckPackagesPanel.BringToFront();
        }

        private void ChangeTrayBracodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChangeTrayBracodeLabel.Text = "";
                pictureBox1.Visible = false;

                CheckTray.Clear();

                if (ChangeTrayBracodeTextBox.Text.Length < 12)
                {
                    CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Ошибка: неверный штрихкод " + ChangeTrayBracodeTextBox.Text);
                    ChangeTrayBracodeTextBox.Clear();
                    return;
                }

                ChangeTrayBracodeLabel.Text = ChangeTrayBracodeTextBox.Text;

                ChangeTrayBracodeTextBox.Clear();

                int TrayID = Convert.ToInt32(ChangeTrayBracodeLabel.Text.Substring(3, 9));
                string Prefix = ChangeTrayBracodeLabel.Text.Substring(0, 3);

                if (Prefix != "005" && Prefix != "006")
                {
                    CheckTray.AddEvent(true, -1, -1, TrayID, -1, string.Empty, string.Empty, "Сканирование поддона. Неверный префикс штрихкода! Ожидалась этикетка поддона");
                    label17.Visible = true;
                    label17.Text = "Штрихкод имеет неверный префикс. Допустимые префиксы 005 и 006";
                }

                CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Добавление на поддон");
                CheckTray.AddEvent(false, -1, -1, -1, -1, string.Empty, string.Empty, "Сканируется этикетка " + ChangeTrayBracodeLabel.Text);

                if (Prefix == "005")
                    CheckTray.CurrentGroupType = 1;
                if (Prefix == "006")
                    CheckTray.CurrentGroupType = 2;

                if (CheckTray.CheckTrayBarcode(ChangeTrayBracodeLabel.Text))
                {
                    CheckTray.IsNewTray = false;
                    CheckTray.CurrentTrayID = TrayID;

                    if (CheckTray.CurrentGroupType == 2)
                    {
                        CheckTray.GetClientID(TrayID);
                        CheckTray.CurrentClientName = CheckTray.GetMarketClientName(CheckTray.CurrentClientID);
                        CheckTray.AddEvent(false, CheckTray.CurrentGroupType, -1, TrayID, -1, string.Empty, string.Empty,
                            "Поддон под клиентом: " + CheckTray.CurrentClientName);
                    }

                    CheckTray.AddEvent(false, CheckTray.CurrentGroupType, -1, TrayID, -1, string.Empty, string.Empty,
                        "Поддон успешно отсканирован; ScanPackagesPanel.BringToFront()");

                    ScanPackagesPanel.BringToFront();
                    ChangeTrayBracodeLabel.Text = "";
                    CheckTimer.Enabled = true;
                    BarcodeTextBox.Focus();
                }
                else
                {
                    pictureBox1.Visible = true;
                    pictureBox1.Image = Properties.Resources.cancel;
                    ChangeTrayBracodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    label17.Text = "Такой этикетки не существует в базе";
                    label17.Visible = true;
                    CheckTray.Clear();
                    return;
                }
            }
        }


    }
}
