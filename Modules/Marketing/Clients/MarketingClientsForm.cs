using NPOI.HSSF.UserModel;

using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingClientsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;
        const int iMarket = 21;
        const int iAdmin = 76;
        const int iDirector = 77;

        bool bPriceGroup = false;

        int FormEvent = 0;
        int CurrentRowIndex = 0;
        int CurrentColumnIndex = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;
        Infinium.Modules.Marketing.Clients.Clients Clients;

        public MarketingClientsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            Clients.GetPermissions(Security.CurrentUserID, this.Name);
            if (Clients.PermissionGranted(iAdmin))
            {
                btnDeleteClient.Visible = true;
                bPriceGroup = true;
            }
            if (Clients.PermissionGranted(iMarket))
            {

            }
            if (Clients.PermissionGranted(iDirector))
            {
                btnDeleteClient.Visible = true;
                bPriceGroup = true;
            }
            if (!Clients.PermissionGranted(iAdmin) && !Clients.PermissionGranted(iMarket) && !Clients.PermissionGranted(iDirector))
            {
                ClientsClientAreaPanel.Height = this.Height - NavigatePanel.Height - 10;
                ToolsPanel.Visible = false;
            }

            while (!SplashForm.bCreated) ;
        }

        private void MarketingClientsForm_Shown(object sender, EventArgs e)
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
            Clients = new Modules.Marketing.Clients.Clients(ref ClientsDataGrid, ref ClientsContactsDataGrid, ref ShopAddressesDataGrid);
        }

        private void ClientsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (Clients == null || Clients.ClientsBindingSource.Count == 0)
                return;

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);

            Clients.FillContactsDataTable(ClientID);
            Clients.FillShopAddressesDataTable(ClientID);
        }

        private void ClientsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentColumnIndex = e.ColumnIndex;
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void CopyContextMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ClientsDataGrid.Rows[CurrentRowIndex].Cells[CurrentColumnIndex].Value.ToString());
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

        private void ClientsNewClientButton_Click(object sender, EventArgs e)
        {
            Clients.NewClient = true;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            NewMarketingClientForm NewMarketingClientForm = new NewMarketingClientForm(this, Clients, ClientID, bPriceGroup);

            TopForm = NewMarketingClientForm;
            NewMarketingClientForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewMarketingClientForm.Dispose();
            TopForm = null;
        }

        private void ClientsEditButton_Click(object sender, EventArgs e)
        {
            Clients.NewClient = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            NewMarketingClientForm NewMarketingClientForm = new NewMarketingClientForm(this, Clients, ClientID, bPriceGroup);

            NewMarketingClientForm.EditClient();
            TopForm = NewMarketingClientForm;
            NewMarketingClientForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewMarketingClientForm.Dispose();
            TopForm = null;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void ClientsDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Clients.NewClient = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            NewMarketingClientForm NewMarketingClientForm = new NewMarketingClientForm(this, Clients, ClientID, bPriceGroup);

            NewMarketingClientForm.EditClient();
            TopForm = NewMarketingClientForm;
            NewMarketingClientForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewMarketingClientForm.Dispose();
            TopForm = null;
        }

        private bool saveFile(ref string FileName)
        {
            saveFileDialog1.Title = "Сохранить как";
            saveFileDialog1.FileName = "*.xls";
            saveFileDialog1.Filter = "Excel 1998-2003 Document( *.xls )|*.xls";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileName = saveFileDialog1.FileName; return true;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    return false;
                }
            }
            return false;
        }

        private void CostToExcelButton_Click(object sender, EventArgs e)
        {
            kryptonContextMenu2.Show(new Point(Cursor.Position.X - 10, Cursor.Position.Y - 10));
        }

        private void ClientsDataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;

            if (grid.Columns[e.ColumnIndex].Name == "PriceGroup" && !bPriceGroup)
                e.Cancel = true;
        }

        private void ShopAddressesDataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid dataGridView = (PercentageDataGrid)sender;
            if (e.ColumnIndex > -1 && e.RowIndex > -1 && dataGridView.Columns[e.ColumnIndex].Name == "Address" && dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 0)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void ShopAddressesDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string Address = string.Empty;
            if (ShopAddressesDataGrid.SelectedRows.Count != 0 && ShopAddressesDataGrid.SelectedRows[0].Cells["Address"].Value != DBNull.Value)
                Address = ShopAddressesDataGrid.SelectedRows[0].Cells["Address"].Value.ToString();
            if (Address.Length == 0)
                return;
            try
            {
                StringBuilder QuerryAddress = new StringBuilder();
                QuerryAddress.Append("http://www.google.com/maps?q=");
                QuerryAddress.Append(Address);
                System.Diagnostics.Process.Start(QuerryAddress.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void btnSendInfiniumAgent_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            Clients.Send(((DataRowView)Clients.ClientsBindingSource.Current).Row["Email"].ToString(), ((DataRowView)Clients.ClientsBindingSource.Current).Row["Login"].ToString());

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            decimal EURRUBCurrency = 1000000;
            decimal USDRUBCurrency = 1000000;
            decimal EURUSDCurrency = 1000000;
            decimal EURBYRCurrency = 1000000;
            bool fixedPaymentRate = false;
            Clients.GetFixedPaymentRate(ClientID, DateTime.Now, ref fixedPaymentRate, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency);

            if (fixedPaymentRate)
            {
                USDRUBCurrency = 1;
            }
            else
            {
                EURRUBCurrency = 0;
                USDRUBCurrency = 0;
                EURUSDCurrency = 0;
                EURBYRCurrency = 0;
                Clients.CBRDailyRates(DateTime.Now, ref EURRUBCurrency, ref USDRUBCurrency);
                Clients.GetDateRates(DateTime.Now, ref EURBYRCurrency);
                if (USDRUBCurrency != 0)
                    EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);
            }
            decimal Rate = 1;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            string FileName = "Прейскурант " + ((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientName"].ToString();
            decimal PriceGroup = Convert.ToDecimal(((DataRowView)Clients.ClientsBindingSource.Current).Row["PriceGroup"]);
            HSSFWorkbook hssfworkbook;
            hssfworkbook = new HSSFWorkbook();

            FileName = FileName.Replace('/', '-');
            FileName = FileName.Replace('\"', '\'');
            ClientFrontsPrice obj1 = new ClientFrontsPrice(Rate);
            ClientDecorPrice obj = new ClientDecorPrice(Rate);

            obj1.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            obj.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();
            System.Diagnostics.Process.Start(file.FullName);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            decimal EURRUBCurrency = 0;
            decimal USDRUBCurrency = 0;
            decimal EURUSDCurrency = 0;
            decimal EURBYRCurrency = 0;
            bool fixedPaymentRate = false;
            Clients.GetFixedPaymentRate(ClientID, DateTime.Now, ref fixedPaymentRate, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency);

            if (fixedPaymentRate)
            {
                USDRUBCurrency = 1;
            }
            else
            {
                EURRUBCurrency = 0;
                USDRUBCurrency = 0;
                EURUSDCurrency = 0;
                EURBYRCurrency = 0;
                Clients.CBRDailyRates(DateTime.Now, ref EURRUBCurrency, ref USDRUBCurrency);
                Clients.GetDateRates(DateTime.Now, ref EURBYRCurrency);
                if (USDRUBCurrency != 0)
                    EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);
            }
            decimal Rate = EURUSDCurrency;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            string FileName = "Прейскурант " + ((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientName"].ToString();
            decimal PriceGroup = Convert.ToDecimal(((DataRowView)Clients.ClientsBindingSource.Current).Row["PriceGroup"]);
            HSSFWorkbook hssfworkbook;
            hssfworkbook = new HSSFWorkbook();

            FileName = FileName.Replace('/', '-');
            FileName = FileName.Replace('\"', '\'');
            ClientFrontsPrice obj1 = new ClientFrontsPrice(Rate);
            ClientDecorPrice obj = new ClientDecorPrice(Rate);

            obj1.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            obj.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();
            System.Diagnostics.Process.Start(file.FullName);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            decimal EURRUBCurrency = 0;
            decimal USDRUBCurrency = 0;
            decimal EURUSDCurrency = 0;
            decimal EURBYRCurrency = 0;
            bool fixedPaymentRate = false;
            Clients.GetFixedPaymentRate(ClientID, DateTime.Now, ref fixedPaymentRate, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency);

            if (fixedPaymentRate)
            {
                USDRUBCurrency = 1;
            }
            else
            {
                EURRUBCurrency = 0;
                USDRUBCurrency = 0;
                EURUSDCurrency = 0;
                EURBYRCurrency = 0;
                Clients.CBRDailyRates(DateTime.Now, ref EURRUBCurrency, ref USDRUBCurrency);
                Clients.GetDateRates(DateTime.Now, ref EURBYRCurrency);
                if (USDRUBCurrency != 0)
                    EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);
            }
            decimal Rate = EURRUBCurrency;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            string FileName = "Прейскурант " + ((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientName"].ToString();
            decimal PriceGroup = Convert.ToDecimal(((DataRowView)Clients.ClientsBindingSource.Current).Row["PriceGroup"]);
            HSSFWorkbook hssfworkbook;
            hssfworkbook = new HSSFWorkbook();

            FileName = FileName.Replace('/', '-');
            FileName = FileName.Replace('\"', '\'');
            ClientFrontsPrice obj1 = new ClientFrontsPrice(Rate);
            ClientDecorPrice obj = new ClientDecorPrice(Rate);

            obj1.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            obj.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();
            System.Diagnostics.Process.Start(file.FullName);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            decimal EURRUBCurrency = 0;
            decimal USDRUBCurrency = 0;
            decimal EURUSDCurrency = 0;
            decimal EURBYRCurrency = 0;
            bool fixedPaymentRate = false;
            Clients.GetFixedPaymentRate(ClientID, DateTime.Now, ref fixedPaymentRate, ref EURUSDCurrency, ref EURRUBCurrency, ref EURBYRCurrency);

            if (fixedPaymentRate)
            {
                USDRUBCurrency = 1;
            }
            else
            {
                EURRUBCurrency = 0;
                USDRUBCurrency = 0;
                EURUSDCurrency = 0;
                EURBYRCurrency = 0;
                Clients.CBRDailyRates(DateTime.Now, ref EURRUBCurrency, ref USDRUBCurrency);
                Clients.GetDateRates(DateTime.Now, ref EURBYRCurrency);
                if (USDRUBCurrency != 0)
                    EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);
            }
            decimal Rate = EURBYRCurrency;


            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            string FileName = "Прейскурант " + ((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientName"].ToString();
            decimal PriceGroup = Convert.ToDecimal(((DataRowView)Clients.ClientsBindingSource.Current).Row["PriceGroup"]);
            HSSFWorkbook hssfworkbook;
            hssfworkbook = new HSSFWorkbook();

            FileName = FileName.Replace('/', '-');
            FileName = FileName.Replace('\"', '\'');
            ClientFrontsPrice obj1 = new ClientFrontsPrice(Rate);
            ClientDecorPrice obj = new ClientDecorPrice(Rate);

            obj1.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            obj.CreateReport(ref hssfworkbook, ClientID, PriceGroup);
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();
            System.Diagnostics.Process.Start(file.FullName);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnDeleteClient_Click(object sender, EventArgs e)
        {
            bool bDeleteOrders = false;
            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            bool OKCancel = Infinium.LightMessageBox.ShowClientDeleteForm(ref TopForm, ref bDeleteOrders);
            if (!OKCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (bDeleteOrders)
            {
                Clients.RemoveClientOrders(ClientID);
                Clients.FixOrderEvent(ClientID, "Заказы клиента удалены");
            }
            Clients.RemoveClient(ClientID);
            Clients.FixOrderEvent(ClientID, "Клиент удален");

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
