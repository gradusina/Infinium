using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingPackagesPrintTPSForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;
        const int FactoryID = 2;//ТПС
        const int BarcodeType = 4;//Упаковка. Маркетинг. ТПС

        int FormEvent = 0;
        int CurrentRowIndex = 0;
        int CurrentColumnIndex = 0;
        int CurrentPackNumber = 0;

        bool NeedRefresh = false;
        bool NeedSplash = false;

        ArrayList FrontIDs;
        ArrayList ProductIDs;

        Form TopForm = null;
        MarketingFilterProductsForm MarketingFilterProductsForm;
        LightStartForm LightStartForm;


        public Modules.Packages.Marketing.MarketingPackagesPrintManager MarketingPackagesPrintManager;
        private Modules.Packages.Marketing.PackingList PackingList;
        private Modules.Packages.Marketing.PrintBarCode PrintBarCode;
        private Modules.Packages.Marketing.PackageLabel PackageLabel;

        public MarketingPackagesPrintTPSForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void MarketingPackagesPrintProfilForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            if (MarketingPackagesPrintManager.MegaOrdersBindingSource.Current != null)
            {
                MarketingPackagesPrintManager.FilterMainOrders(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                    FrontIDs, ProductIDs, true, false);
            }
            MarketingPackagesPrintManager.PackedMainOrdersFrontsOrders.SetColor(CurrentPackNumber);
            NeedSplash = true;
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
                    NeedSplash = true;
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
                    NeedSplash = true;
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
            NeedSplash = false;
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
            MarketingPackagesPrintManager = new Modules.Packages.Marketing.MarketingPackagesPrintManager(
                ref MegaOrdersDataGrid, ref MainOrdersDataGrid, ref PackagesDataGrid, ref
                MainOrdersFrontsOrdersDataGrid, ref MainOrdersDecorOrdersDataGrid,
                ref MainOrdersTabControl, FactoryID);

            PackingList = new Modules.Packages.Marketing.PackingList()
            {
                Factory = FactoryID
            };
            PrintBarCode = new Modules.Packages.Marketing.PrintBarCode()
            {
                Factory = FactoryID
            };
            PackageLabel = new Modules.Packages.Marketing.PackageLabel();

            FrontIDs = new ArrayList();

            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        FrontIDs.Add(Convert.ToInt32(DT.Rows[i]["FrontID"]));
                    }
                }
            }

            ProductIDs = new ArrayList();
        }

        private void MegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (MarketingPackagesPrintManager != null)
                if (MarketingPackagesPrintManager.MegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)MarketingPackagesPrintManager.MegaOrdersBindingSource.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        bool NoDispatched = NoDispatchedCheckBox.Checked;
                        bool NotPrinted = cbNotPrintedPackages.Checked;
                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            MarketingPackagesPrintManager.FilterMainOrders(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                FrontIDs, ProductIDs, NoDispatched, NotPrinted);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                            MarketingPackagesPrintManager.FilterMainOrders(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                                FrontIDs, ProductIDs, NoDispatched, NotPrinted);
                    }
                }
                else
                    MarketingPackagesPrintManager.MainOrdersDataTable.Clear();
        }

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (MarketingPackagesPrintManager != null)
                if (MarketingPackagesPrintManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)MarketingPackagesPrintManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            MarketingPackagesPrintManager.Filter(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MainOrdersBindingSource.Current)["MainOrderID"]),
                                FrontIDs, ProductIDs);
                            MarketingPackagesPrintManager.FilterPackages(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MainOrdersBindingSource.Current)["MainOrderID"]),
                                FrontIDs, ProductIDs);

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                        {
                            MarketingPackagesPrintManager.Filter(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MainOrdersBindingSource.Current)["MainOrderID"]),
                                FrontIDs, ProductIDs);
                            MarketingPackagesPrintManager.FilterPackages(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MainOrdersBindingSource.Current)["MainOrderID"]),
                                FrontIDs, ProductIDs);
                        }
                    }
                    else
                    {
                        MarketingPackagesPrintManager.Filter(-1, FrontIDs, ProductIDs);
                        MarketingPackagesPrintManager.PackagesDataTable.Clear();
                        MarketingPackagesPrintManager.PackedMainOrdersFrontsOrders.FrontsOrdersDataTable.Clear();
                        MarketingPackagesPrintManager.PackedMainOrdersDecorOrders.DecorOrdersDataTable.Clear();
                    }
                }
        }

        private void MegaOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MegaOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void MegaOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MainOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void MainOrdersFrontsOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersFrontsOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MainOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void MainOrdersDecorOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDecorOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MainOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void PackagesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (MarketingPackagesPrintManager != null)
                if (MarketingPackagesPrintManager.PackagesBindingSource.Count > 0)
                {
                    if (((DataRowView)MarketingPackagesPrintManager.PackagesBindingSource.Current)["PackNumber"] != DBNull.Value)
                    {
                        int PackNumber = Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.PackagesBindingSource.Current)["PackNumber"]);
                        int ProductType = Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.PackagesBindingSource.Current)["ProductType"]);
                        CurrentPackNumber = PackNumber;

                        if (ProductType == 0)
                        {
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];
                            MarketingPackagesPrintManager.PackedMainOrdersFrontsOrders.MoveToFrontOrder(PackNumber);
                            MarketingPackagesPrintManager.PackedMainOrdersFrontsOrders.SetColor(PackNumber);

                        }
                        if (ProductType == 1)
                        {
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[1];
                            MarketingPackagesPrintManager.PackedMainOrdersDecorOrders.MoveToDecorOrder(PackNumber);
                            MarketingPackagesPrintManager.PackedMainOrdersDecorOrders.SetColor(PackNumber);
                        }

                    }
                }
        }

        private void MainOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentColumnIndex = e.ColumnIndex;
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        //Распечатать все этикетки в нескольких подзаказах
        private void PrintMainOrderContextMenuItem_Click(object sender, EventArgs e)
        {
            if (MarketingPackagesPrintManager.MainOrdersBindingSource.Count < 1)
                return;

            int[] MainOrders = MarketingPackagesPrintManager.GetSelectedMainOrders();

            //PackagesOrdersManager.MainOrdersBindingSource.Position = CurrentRowIndex;

            //int MainOrderID = Convert.ToInt32(((DataRowView)PackagesOrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]);
            int PackNumber = 0;
            int PackageID = 0;
            int ProductType = 0;

            //int[] PackNumbers = PrintBarCode.GetPackNumbers(MainOrderID, IsFronts);

            PackageLabel.ClearLabelInfo();
            ArrayList PackageIDs = new ArrayList();
            DataTable TempPackagesDataTable = new DataTable();

            for (int j = 0; j < MainOrders.Count(); j++)
            {
                //Проверка
                if (!MarketingPackagesPrintManager.IsMainOrderPacked(MainOrders[j]))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Этикетки распечатать нельзя. Подзаказ № " + MainOrders[j] + " должен быть полностью распределен на обеих фирмах",
                        "Ошибка печати");
                    return;
                }

                TempPackagesDataTable = MarketingPackagesPrintManager.TempPackages(MainOrders[j], FrontIDs, ProductIDs).Copy();

                for (int i = 0; i < TempPackagesDataTable.Rows.Count; i++)
                {
                    DataTable DT = new DataTable();
                    Infinium.Modules.Packages.Marketing.Info LabelInfo = new Modules.Packages.Marketing.Info();

                    ProductType = Convert.ToInt32(TempPackagesDataTable.Rows[i]["ProductType"]);
                    PackNumber = Convert.ToInt32(TempPackagesDataTable.Rows[i]["PackNumber"]);
                    PackageID = Convert.ToInt32(TempPackagesDataTable.Rows[i]["PackageID"]);
                    PackageIDs.Add(PackageID);

                    if (ProductType == 0)
                    {
                        GetPackageInfo(MainOrders[j], PackNumber, ProductType, PackageID, ref LabelInfo);
                        LabelInfo.ProductType = ProductType;
                        PrintBarCode.FilterFrontsOrders(MainOrders[j], PackageID);
                        LabelInfo.TechnoInset = PrintBarCode.TechnoInset;
                        DT = PrintBarCode.FillFrontsDataTable().Copy();
                    }
                    if (ProductType == 1)
                    {
                        ProductType = 1;
                        GetPackageInfo(MainOrders[j], PackNumber, ProductType, PackageID, ref LabelInfo);
                        LabelInfo.ProductType = ProductType;
                        PrintBarCode.FilterDecorOrders(MainOrders[j], PackageID);
                        DT = PrintBarCode.FillDecorDataTable().Copy();
                    }

                    LabelInfo.OrderData = DT;
                    LabelInfo.GroupType = "М";

                    PackageLabel.AddLabelInfo(0, ref LabelInfo);

                    MarketingPackagesPrintManager.FilterPackages(MainOrders[j], FrontIDs, ProductIDs);
                }
            }

            PrintDialog.Document = PackageLabel.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < PackageIDs.Count; i++)
                {
                    PackageID = Convert.ToInt32(PackageIDs[i]);
                    MarketingPackagesPrintManager.PrintedCountUp(PackageID);
                }
                PackageLabel.Print();
            }

        }

        private void PackagesDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentColumnIndex = e.ColumnIndex;
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        //Распечатать одну этикетку
        private void PrintPackageContextMenuItem_Click(object sender, EventArgs e)
        {
            if (MarketingPackagesPrintManager.MainOrdersBindingSource.Count < 1)
                return;

            int[] Packages = MarketingPackagesPrintManager.GetSelectedPackages();

            DataTable DT = new DataTable();

            int MainOrderID = Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MainOrdersBindingSource.Current)["MainOrderID"]);
            //Проверка
            if (!MarketingPackagesPrintManager.IsMainOrderPacked(MainOrderID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Этикетки распечатать нельзя. Подзаказ № " + MainOrderID + " должен быть полностью распределен на обеих фирмах",
                        "Ошибка печати");
                return;
            }

            int PackNumber = 0;
            int ProductType = 0;
            int PackageID = 0;

            ArrayList PackageIDs = new ArrayList();

            PackageLabel.ClearLabelInfo();

            for (int i = 0; i < PackagesDataGrid.SelectedRows.Count; i++)
            {
                PackNumber = Convert.ToInt32(PackagesDataGrid.SelectedRows[i].Cells["PackNumber"].Value);
                ProductType = Convert.ToInt32(PackagesDataGrid.SelectedRows[i].Cells["ProductType"].Value);
                PackageID = Convert.ToInt32(PackagesDataGrid.SelectedRows[i].Cells["PackageID"].Value);
                PackageIDs.Add(PackageID);

                Infinium.Modules.Packages.Marketing.Info LabelInfo = new Modules.Packages.Marketing.Info();

                if (ProductType == 0)
                {
                    ProductType = 0;
                    GetPackageInfo(MainOrderID, PackNumber, ProductType, PackageID, ref LabelInfo);
                    LabelInfo.ProductType = ProductType;
                    PrintBarCode.FilterFrontsOrders(MainOrderID, PackageID);
                    LabelInfo.TechnoInset = PrintBarCode.TechnoInset;
                    DT = PrintBarCode.FillFrontsDataTable().Copy();
                }
                if (ProductType == 1)
                {
                    ProductType = 1;
                    GetPackageInfo(MainOrderID, PackNumber, ProductType, PackageID, ref LabelInfo);
                    LabelInfo.ProductType = ProductType;
                    PrintBarCode.FilterDecorOrders(MainOrderID, PackageID);
                    DT = PrintBarCode.FillDecorDataTable().Copy();
                }

                LabelInfo.OrderData = DT;
                LabelInfo.GroupType = "М";


                PackageLabel.AddLabelInfo(0, ref LabelInfo);

            }

            PrintDialog.Document = PackageLabel.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < PackageIDs.Count; i++)
                {
                    PackageID = Convert.ToInt32(PackageIDs[i]);
                    MarketingPackagesPrintManager.PrintedCountUp(PackageID);
                }
                //ZOVPackagesPrintManager.PrintedCountUp(PackageID);
                PackageLabel.Print();
            }

            MarketingPackagesPrintManager.PackagesBindingSource.Position = CurrentRowIndex;

            MarketingPackagesPrintManager.FilterPackages(MainOrderID, FrontIDs, ProductIDs);
        }

        private void MegaOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentColumnIndex = e.ColumnIndex;
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        //Распечатать все этикетки в одном заказе
        private void PrintMegaOrderContextMenuItem_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание этикеток.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (MarketingPackagesPrintManager.MegaOrdersBindingSource.Count < 1)
                return;

            MarketingPackagesPrintManager.MegaOrdersBindingSource.Position = CurrentRowIndex;

            //Проверка
            int MegaOrderID = Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            int MainOrderID = 0;

            if (!MarketingPackagesPrintManager.IsMegaOrderPacked(MegaOrderID, ref MainOrderID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Этикетки распечатать нельзя. Подзаказ № " + MainOrderID + " должен быть полностью распределен на обеих фирмах",
                        "Ошибка печати");
                return;
            }

            int[] MainOrders = MarketingPackagesPrintManager.GetMainOrders();
            int PackNumber = 0;
            int PackageID = 0;
            int ProductType = 0;

            ArrayList PackageIDs = new ArrayList();

            PackageLabel.ClearLabelInfo();

            DataTable TempPackagesDataTable = new DataTable();

            for (int j = 0; j < MainOrders.Count(); j++)
            {
                TempPackagesDataTable = MarketingPackagesPrintManager.TempPackages(MainOrders[j], FrontIDs, ProductIDs).Copy();

                for (int i = 0; i < TempPackagesDataTable.Rows.Count; i++)
                {
                    DataTable DT = new DataTable();
                    Infinium.Modules.Packages.Marketing.Info LabelInfo = new Modules.Packages.Marketing.Info();

                    ProductType = Convert.ToInt32(TempPackagesDataTable.Rows[i]["ProductType"]);
                    PackNumber = Convert.ToInt32(TempPackagesDataTable.Rows[i]["PackNumber"]);
                    PackageID = Convert.ToInt32(TempPackagesDataTable.Rows[i]["PackageID"]);
                    PackageIDs.Add(PackageID);

                    if (ProductType == 0)
                    {
                        ProductType = 0;
                        GetPackageInfo(MainOrders[j], PackNumber, ProductType, PackageID, ref LabelInfo);
                        LabelInfo.ProductType = ProductType;
                        PrintBarCode.FilterFrontsOrders(MainOrders[j], PackageID);
                        LabelInfo.TechnoInset = PrintBarCode.TechnoInset;
                        DT = PrintBarCode.FillFrontsDataTable().Copy();
                    }
                    if (ProductType == 1)
                    {
                        ProductType = 1;
                        GetPackageInfo(MainOrders[j], PackNumber, ProductType, PackageID, ref LabelInfo);
                        LabelInfo.ProductType = ProductType;
                        PrintBarCode.FilterDecorOrders(MainOrders[j], PackageID);
                        DT = PrintBarCode.FillDecorDataTable().Copy();
                    }

                    LabelInfo.OrderData = DT;
                    LabelInfo.GroupType = "М";

                    PackageLabel.AddLabelInfo(0, ref LabelInfo);


                    MarketingPackagesPrintManager.FilterPackages(MainOrders[j], FrontIDs, ProductIDs);
                }
            }
            PrintDialog.Document = PackageLabel.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < PackageIDs.Count; i++)
                {
                    PackageID = Convert.ToInt32(PackageIDs[i]);
                    MarketingPackagesPrintManager.PrintedCountUp(PackageID);
                }
                PackageLabel.Print();
            }
            TempPackagesDataTable.Dispose();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        //Упаковочная ведомость заказа
        private void PrintMegaOrderStatementContextMenuItem_Click(object sender, EventArgs e)
        {
            if (MarketingPackagesPrintManager.MegaOrdersBindingSource.Count < 1)
                return;

            MarketingPackagesPrintManager.MegaOrdersBindingSource.Position = CurrentRowIndex;

            bool NoDispatched = NoDispatchedCheckBox.Checked;
            bool NotPrinted = cbNotPrintedPackages.Checked;
            MarketingPackagesPrintManager.FilterMainOrders(Convert.ToInt32(((DataRowView)MarketingPackagesPrintManager.MegaOrdersBindingSource.Current)["MegaOrderID"]),
                FrontIDs, ProductIDs, NoDispatched, NotPrinted);

            int[] MainOrders = MarketingPackagesPrintManager.GetMainOrders();

            if (MainOrders.Count() == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Выбранный заказ пуст",
                   "Ошибка");
                return;
            }
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();


            while (!SplashWindow.bSmallCreated) ;

            PackingList.CreateReport(MainOrders, FrontIDs, ProductIDs);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void GetPackageInfo(int MainOrderID, int PackNumber, int ProductType, int PackageID, ref Infinium.Modules.Packages.Marketing.Info LabelInfo)
        {
            int PackAllocStatusID = 0;
            LabelInfo.ClientName = PrintBarCode.GetClientName(MainOrderID);
            LabelInfo.DocNumber = PrintBarCode.GetOrderNumber(MainOrderID);
            LabelInfo.MainOrder = MainOrderID;
            LabelInfo.Notes = PrintBarCode.GetMainOrderNotes(MainOrderID);
            LabelInfo.DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
            LabelInfo.DispatchDate = PrintBarCode.GetDispatchDate(MainOrderID, FactoryID);
            LabelInfo.PackNumber = PackNumber;
            LabelInfo.TotalPackCount = PrintBarCode.GetPackNumberCount(MainOrderID, FactoryID, ref PackAllocStatusID);
            LabelInfo.PackAllocStatusID = PackAllocStatusID;
            LabelInfo.FactoryType = FactoryID;

            LabelInfo.BarcodeNumber = PrintBarCode.GetBarcodeNumber(BarcodeType, PackageID);
        }

        //Упаковочная ведомость (несколько подзаказов)
        private void PrintMainOrderStatementContextMenuItem_Click(object sender, EventArgs e)
        {
            if (MarketingPackagesPrintManager.MegaOrdersBindingSource.Count < 1)
                return;

            int[] MainOrders = MarketingPackagesPrintManager.GetSelectedMainOrders();

            //PackagesOrdersManager.MainOrdersBindingSource.Position = CurrentRowIndex;
            //int MainOrderID = Convert.ToInt32(((DataRowView)PackagesOrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]);



            if (MainOrders.Count() == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Выбранный заказ пуст",
                   "Ошибка");
                return;
            }
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            PackingList.CreateReport(MainOrders, FrontIDs, ProductIDs);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            MarketingFilterProductsForm = new MarketingFilterProductsForm(FactoryID, ref FrontIDs, ref ProductIDs);

            TopForm = MarketingFilterProductsForm;
            MarketingFilterProductsForm.ShowDialog();
            TopForm = null;

            PhantomForm.Close();

            PhantomForm.Dispose();
            if (MarketingFilterProductsForm.NeedFilter)
            {
                MarketingFilterProductsForm.Dispose();
                MarketingFilterProductsForm = null;
                GC.Collect();

                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                bool NoDispatched = NoDispatchedCheckBox.Checked;
                bool NotPrinted = cbNotPrintedPackages.Checked;
                MainOrdersTabControl.Visible = false;
                MarketingPackagesPrintManager.FillOrders(FrontIDs, ProductIDs, NoDispatched, NotPrinted);
                MainOrdersTabControl.Visible = true;

                MarketingPackagesPrintManager.MegaOrdersBindingSource.MoveFirst();
                MegaOrdersDataGrid_SelectionChanged(null, null);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                NeedSplash = true;
            }
            else
            {
                MarketingFilterProductsForm.Dispose();
                MarketingFilterProductsForm = null;
                GC.Collect();
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            panel2.Visible = !panel2.Visible;
        }

        private void NoDispatchedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            bool NoDispatched = NoDispatchedCheckBox.Checked;
            bool NotPrinted = cbNotPrintedPackages.Checked;
            MainOrdersTabControl.Visible = false;
            MarketingPackagesPrintManager.FillOrders(FrontIDs, ProductIDs, NoDispatched, NotPrinted);
            MainOrdersTabControl.Visible = true;

            MarketingPackagesPrintManager.MegaOrdersBindingSource.MoveFirst();
            MegaOrdersDataGrid_SelectionChanged(null, null);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void cbNotPrintedPackages_CheckedChanged(object sender, EventArgs e)
        {
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            bool NoDispatched = NoDispatchedCheckBox.Checked;
            bool NotPrinted = cbNotPrintedPackages.Checked;
            MainOrdersTabControl.Visible = false;
            MarketingPackagesPrintManager.FillOrders(FrontIDs, ProductIDs, NoDispatched, NotPrinted);
            MainOrdersTabControl.Visible = true;

            MarketingPackagesPrintManager.MegaOrdersBindingSource.MoveFirst();
            //MegaOrdersDataGrid_SelectionChanged(null, null);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            NeedSplash = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }


    }
}