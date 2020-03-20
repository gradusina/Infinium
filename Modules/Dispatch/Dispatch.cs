using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Dispatch
{

    public struct PackagesCount
    {
        public int ProfilPackedPackages;
        public int TPSPackedPackages;
        public int AllPackedPackages;

        public int ProfilPackages;
        public int TPSPackages;
        public int AllPackages;
    }

    public struct TrayPackages
    {
        public int AllTrayPackages;

        public int AllPackages;
    }






    public class MarketingDispatchList
    {
        MarketingPackingList PackingList;

        PackagesCount PackagesCount;

        private int CurrentMegaOrderID = 0;

        DataTable ClientsDataTable = null;
        DataTable ResultDataTable = null;
        DataTable MegaOrdersDataTable = null;
        DataTable MainOrdersDataTable = null;
        DataTable AttachResultDataTable = null;
        DataTable PackagesDataTable = null;
        DataTable DispPackagesDataTable = null;

        public BindingSource MegaOrdersBindingSource = null;
        public BindingSource FilterClientsBindingSource = null;

        SqlDataAdapter MegaOrdersDataAdapter = null;

        public PercentageDataGrid MegaOrdersDataGrid = null;

        public MarketingDispatchList(ref PercentageDataGrid tMegaOrdersDataGrid)
        {
            MegaOrdersDataGrid = tMegaOrdersDataGrid;
            Create();
            Fill();
            MegaGridSetting();
        }

        private void Create()
        {
            ClientsDataTable = new DataTable();
            ResultDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();
            AttachResultDataTable = new DataTable();
            PackagesDataTable = new DataTable();
            DispPackagesDataTable = new DataTable();

            FilterClientsBindingSource = new BindingSource();
            MegaOrdersBindingSource = new BindingSource();

            ResultDataTable.Columns.Add(new DataColumn(("MainOrder"), System.Type.GetType("System.String")));
            ResultDataTable.Columns.Add(new DataColumn(("FrontsPackagesCount"), System.Type.GetType("System.String")));
            ResultDataTable.Columns.Add(new DataColumn(("DecorPackagesCount"), System.Type.GetType("System.String")));
            ResultDataTable.Columns.Add(new DataColumn(("AllPackagesCount"), System.Type.GetType("System.String")));

            AttachResultDataTable.Columns.Add(new DataColumn(("MainOrder"), System.Type.GetType("System.String")));
            AttachResultDataTable.Columns.Add(new DataColumn(("ProfilFrontsPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDataTable.Columns.Add(new DataColumn(("ProfilDecorPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDataTable.Columns.Add(new DataColumn(("TPSFrontsPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDataTable.Columns.Add(new DataColumn(("TPSDecorPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDataTable.Columns.Add(new DataColumn(("AllPackagesCount"), System.Type.GetType("System.String")));
        }

        private void Fill()
        {
            MegaOrdersDataTable.Clear();
            string DispatchDate = DateTime.Now.ToString("yyyy-MM-dd");
            string SellectionCommand = "SELECT MegaOrders.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE OrderStatusID<>3 AND (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2 OR ProfilPackAllocStatusID=1 OR TPSPackAllocStatusID=1)" +
                " AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2))" +
                " AND (ProfilDispatchDate = '" + DispatchDate + "' OR TPSDispatchDate = '" + DispatchDate + "')" +
                " ORDER BY ClientName, OrderNumber";

            MegaOrdersDataAdapter = new SqlDataAdapter(SellectionCommand, ConnectionStrings.MarketingOrdersConnectionString);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            MegaOrdersDataTable.Columns.Add(new DataColumn(("ProfilPackagesCount"), System.Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn(("TPSPackagesCount"), System.Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MainOrders.MegaOrderID, Packages.FactoryID, COUNT(Packages.PackageID) AS AllCount
            FROM Packages INNER JOIN
                MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
            GROUP BY MainOrders.MegaOrderID, Packages.FactoryID
            ORDER BY MainOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MainOrders.MegaOrderID, Packages.FactoryID, COUNT(Packages.PackageID) AS DispCount
            FROM Packages INNER JOIN
                MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
            WHERE (Packages.PackageStatusID = 3)
            GROUP BY MainOrders.MegaOrderID, Packages.FactoryID
            ORDER BY MainOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispPackagesDataTable);
            }

            FillMainPackedColumn();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            FilterClientsBindingSource.DataSource = ClientsDataTable;

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
        }

        public void Filter(bool ByClient, bool Dispatched, bool ByDispatchSchedule, int ClientID)
        {
            string FilterDispatch = " WHERE OrderStatusID<>3";
            string FilterClient = string.Empty;

            if (ByClient && ClientID != -1)
                FilterClient = " AND MegaOrders.ClientID = " + ClientID;

            if (Dispatched)
            {
                FilterDispatch = " WHERE OrderStatusID=3";
            }

            MegaOrdersDataTable.Clear();
            string SellectionCommand = "SELECT MegaOrders.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" + FilterDispatch +
                " AND (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2 OR ProfilPackAllocStatusID=1 OR TPSPackAllocStatusID=1)" + FilterClient +
                " AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2)) ORDER BY ClientName, OrderNumber";

            if (ByDispatchSchedule)
            {
                string DispatchDate = DateTime.Now.ToString("yyyy-MM-dd");
                SellectionCommand = "SELECT MegaOrders.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MegaOrders" +
                    " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" + FilterDispatch +
                    " AND (ProfilDispatchDate = '" + DispatchDate + "' OR TPSDispatchDate = '" + DispatchDate + "') ORDER BY ClientName, OrderNumber";
            }

            MegaOrdersDataAdapter = new SqlDataAdapter(SellectionCommand, ConnectionStrings.MarketingOrdersConnectionString);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            FillMainPackedColumn();

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
        }

        private void FillMainPackedColumn()
        {
            int ProfilPackedPackagesCount = 0;
            int TPSPackedPackagesCount = 0;

            int ProfilPackagesCount = 0;
            int TPSPackagesCount = 0;

            int MegaOrderID = 0;

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                ProfilPackedPackagesCount = 0;
                TPSPackedPackagesCount = 0;

                ProfilPackagesCount = 0;
                TPSPackagesCount = 0;

                MegaOrderID = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]);

                ProfilPackedPackagesCount += GetFirmDispPackagesCount(MegaOrderID, 1);
                ProfilPackagesCount += GetFirmPackagesCount(MegaOrderID, 1);

                TPSPackedPackagesCount += GetFirmDispPackagesCount(MegaOrderID, 2);
                TPSPackagesCount += GetFirmPackagesCount(MegaOrderID, 2);

                MegaOrdersDataTable.Rows[i]["ProfilPackagesCount"] = ProfilPackedPackagesCount + " / " + ProfilPackagesCount;
                MegaOrdersDataTable.Rows[i]["TPSPackagesCount"] = TPSPackedPackagesCount + " / " + TPSPackagesCount;
            }
        }

        private void MegaGridSetting()
        {
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;

            foreach (DataGridViewColumn Column in MegaOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (MegaOrdersDataGrid.Columns.Contains("ComplaintProfilCost"))
                MegaOrdersDataGrid.Columns["ComplaintProfilCost"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ComplaintTPSCost"))
                MegaOrdersDataGrid.Columns["ComplaintTPSCost"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ComplaintNotes"))
                MegaOrdersDataGrid.Columns["ComplaintNotes"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("CurrencyComplaintProfilCost"))
                MegaOrdersDataGrid.Columns["CurrencyComplaintProfilCost"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("CurrencyComplaintTPSCost"))
                MegaOrdersDataGrid.Columns["CurrencyComplaintTPSCost"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("DelayOfPayment"))
                MegaOrdersDataGrid.Columns["DelayOfPayment"].Visible = false;
            MegaOrdersDataGrid.Columns["ClientID"].Visible = false;
            MegaOrdersDataGrid.Columns["OrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilOrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSOrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["AgreementStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            MegaOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MegaOrdersDataGrid.Columns["DesireDate"].Visible = false;
            MegaOrdersDataGrid.Columns["LastCalcDate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("LastCalcUserID"))
                MegaOrdersDataGrid.Columns["LastCalcUserID"].Visible = false;
            MegaOrdersDataGrid.Columns["OrderDate"].Visible = false;
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].Visible = false;
            MegaOrdersDataGrid.Columns["OrderCost"].Visible = false;
            MegaOrdersDataGrid.Columns["TransportCost"].Visible = false;
            MegaOrdersDataGrid.Columns["AdditionalCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].Visible = false;
            MegaOrdersDataGrid.Columns["TotalCost"].Visible = false;
            MegaOrdersDataGrid.Columns["Rate"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSPackCount"].Visible = false;

            MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;

            //if (FactoryID == 1)
            //    MegaOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            //else
            //    MegaOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("IsComplaint"))
                MegaOrdersDataGrid.Columns["IsComplaint"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ProfilProductionDate"))
                MegaOrdersDataGrid.Columns["ProfilProductionDate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSProductionDate"))
                MegaOrdersDataGrid.Columns["TPSProductionDate"].Visible = false;

            MegaOrdersDataGrid.Columns["ProfilConfirmProduction"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSConfirmProduction"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilAllowDispatch"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSAllowDispatch"].Visible = false;

            if (MegaOrdersDataGrid.Columns.Contains("ProfilConfirmDispatch"))
                MegaOrdersDataGrid.Columns["ProfilConfirmDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSConfirmDispatch"))
                MegaOrdersDataGrid.Columns["TPSConfirmDispatch"].Visible = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            MegaOrdersDataGrid.Columns["OrderNumber"].HeaderText = "    №\r\nзаказа";
            MegaOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].HeaderText = "Дата отгрузки\r\n    Профиль";
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].HeaderText = "Дата отгрузки\r\n        ТПС";
            MegaOrdersDataGrid.Columns["Square"].HeaderText = "    Площадь\r\nфасадов, м.кв.";
            MegaOrdersDataGrid.Columns["ProfilPackagesCount"].HeaderText = "Кол-во уп.,\r\n Профиль";
            MegaOrdersDataGrid.Columns["TPSPackagesCount"].HeaderText = "Кол-во уп.,\r\n     ТПС";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 1
            };
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.AutoGenerateColumns = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].DisplayIndex = 0;
            MegaOrdersDataGrid.Columns["ClientName"].DisplayIndex = 1;
            MegaOrdersDataGrid.Columns["OrderNumber"].DisplayIndex = 2;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].DisplayIndex = 3;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].DisplayIndex = 4;
            MegaOrdersDataGrid.Columns["Square"].DisplayIndex = 5;
            MegaOrdersDataGrid.Columns["Weight"].DisplayIndex = 6;

            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MegaOrdersDataGrid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["MegaOrderID"].Width = 75;
            MegaOrdersDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaOrdersDataGrid.Columns["ClientName"].MinimumWidth = 195;
            MegaOrdersDataGrid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["OrderNumber"].Width = 75;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].Width = 150;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].Width = 150;
            MegaOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Weight"].Width = 110;
            MegaOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Square"].Width = 155;
            MegaOrdersDataGrid.Columns["TPSPackagesCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSPackagesCount"].Width = 150;
            MegaOrdersDataGrid.Columns["ProfilPackagesCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilPackagesCount"].Width = 150;

            MegaOrdersDataGrid.Columns["ProfilPackagesCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["TPSPackagesCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        #region Реализация интерфейса IReference
        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        public string GetOrderNumber(int MegaOrderID)
        {
            string OrderNumber = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT OrderNumber FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        OrderNumber = DT.Rows[0]["OrderNumber"].ToString();
                }
            }
            return OrderNumber;
        }

        public string GetClientName(int MegaOrderID)
        {
            int ClientID = 0;
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetDispatchDate(int MegaOrderID, int FactoryID)
        {
            string DispatchDate = string.Empty;
            string Date = "ProfilDispatchDate";

            if (FactoryID == 2)
                Date = "TPSDispatchDate";

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT " + Date + " FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0][Date].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0][Date]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        public string GetDispatchDate(int MegaOrderID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }
        #endregion

        #region Get functions

        public int[] GetMainOrders(int MegaOrderID, int FactoryID)
        {
            int[] rows = { 0 };
            string PackAllocStatusID = "ProfilPackAllocStatusID = 2";

            if (FactoryID == 2)
                PackAllocStatusID = "TPSPackAllocStatusID = 2";

            if (MegaOrdersBindingSource.Count > 0)
            {
                MainOrdersDataTable.Clear();
                string SelectionCommand = "SELECT * FROM MainOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID +
                    " AND " + PackAllocStatusID;

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(MainOrdersDataTable);
                }

                rows = new int[MainOrdersDataTable.Rows.Count];

                for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                    rows[i] = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);
            }

            return rows;
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        private int GetFirmPackagesCount(int MegaOrderID, int FactoryID)
        {
            int PackagesCount = 0;

            DataRow[] Rows = PackagesDataTable.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);
            if (Rows.Count() > 0)
            {
                PackagesCount = Convert.ToInt32(Rows[0]["AllCount"]);
            }

            return PackagesCount;
        }

        private int GetFirmDispPackagesCount(int MegaOrderID, int FactoryID)
        {
            int PackagesCount = 0;

            DataRow[] Rows = DispPackagesDataTable.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);
            if (Rows.Count() > 0)
            {
                PackagesCount = Convert.ToInt32(Rows[0]["DispCount"]);
            }

            return PackagesCount;
        }

        private int GetDispPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            int PackagesCount;

            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND PackageStatusID = 3 AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(DT) < 1)
                    return 0;

                PackagesCount = DT.Rows.Count;

                DT.Dispose();
            }

            return PackagesCount;
        }

        private int GetPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            int PackagesCount;

            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(DT) < 1)
                    return 0;

                PackagesCount = DT.Rows.Count;

                DT.Dispose();
            }

            return PackagesCount;
        }

        private decimal GetSquare(int MegaOrderID, int FactoryID)
        {
            decimal Square = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Square) As Square FROM FrontsOrders WHERE FrontsOrdersID IN (" +
                    "SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE FactoryID = " + FactoryID +
                    " AND PackageStatusID = 3 AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Square"] != DBNull.Value)
                    {
                        Square = Convert.ToDecimal(DT.Rows[0]["Square"]);
                        Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }

            return Square;
        }

        private decimal GetWeight(int MegaOrderID, int FactoryID)
        {
            decimal Weight = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM FrontsOrders WHERE FrontsOrdersID IN (" +
                    "SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND FactoryID = " + FactoryID +
                    " AND PackageStatusID = 3 AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        Weight = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM DecorOrders WHERE DecorOrderID IN (" +
                    "SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND FactoryID = " + FactoryID +
                    " AND PackageStatusID = 3 AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        decimal Temp = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                        Weight += Temp;
                    }
                }

                Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);
            }

            return Weight;
        }

        private int[] GetMainOrders(int MegaOrderID)
        {
            int[] rows = { 0 };

            if (MegaOrdersBindingSource.Count > 0)
            {
                MainOrdersDataTable.Clear();
                string SelectionCommand = "SELECT * FROM MainOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID +
                    " AND (ProfilPackAllocStatusID = 2 OR TPSPackAllocStatusID = 2)";

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(MainOrdersDataTable);
                }

                rows = new int[MainOrdersDataTable.Rows.Count];

                for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                    rows[i] = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);
            }

            return rows;
        }

        #endregion

        #region SimpleReport
        private bool Fill(int[] MainOrders, int MegaOrderID, int FactoryID)
        {
            ResultDataTable.Clear();

            PackagesCount.AllPackages = 0;
            PackagesCount.AllPackedPackages = 0;
            PackagesCount.ProfilPackages = 0;
            PackagesCount.ProfilPackedPackages = 0;
            PackagesCount.TPSPackages = 0;
            PackagesCount.TPSPackedPackages = 0;

            int MainOrderID = 0;
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                int FrontsPackedPackagesCount = 0;
                int DecorPackedPackagesCount = 0;
                int AllPackedPackagesCount = 0;

                int FrontsPackagesCount = 0;
                int DecorPackagesCount = 0;
                int AllPackagesCount = 0;

                string ClientName = string.Empty;
                string DocNumber = string.Empty;
                MainOrderID = MainOrders[i];

                FrontsPackedPackagesCount = GetDispPackagesCount(MainOrders[i], FactoryID, 0);
                DecorPackedPackagesCount = GetDispPackagesCount(MainOrders[i], FactoryID, 1);
                AllPackedPackagesCount = FrontsPackedPackagesCount + DecorPackedPackagesCount;

                PackagesCount.AllPackedPackages += AllPackedPackagesCount;
                PackagesCount.ProfilPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;
                PackagesCount.TPSPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;

                FrontsPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 0);
                DecorPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 1);
                AllPackagesCount = FrontsPackagesCount + DecorPackagesCount;

                PackagesCount.AllPackages += AllPackagesCount;
                PackagesCount.ProfilPackages += FrontsPackagesCount + DecorPackagesCount;
                PackagesCount.TPSPackages += FrontsPackagesCount + DecorPackagesCount;

                ClientName = GetClientName(MegaOrderID);
                DocNumber = GetOrderNumber(MegaOrderID);

                DataRow NewRow = ResultDataTable.NewRow();

                NewRow["MainOrder"] = ClientName + " / " + DocNumber;
                if (FrontsPackagesCount > 0)
                    NewRow["FrontsPackagesCount"] = FrontsPackedPackagesCount.ToString() + " / " + FrontsPackagesCount.ToString();
                if (DecorPackagesCount > 0)
                    NewRow["DecorPackagesCount"] = DecorPackedPackagesCount.ToString() + " / " + DecorPackagesCount.ToString();

                NewRow["AllPackagesCount"] = AllPackedPackagesCount.ToString() + " / " + AllPackagesCount.ToString();

                ResultDataTable.Rows.Add(NewRow);
            }

            return ResultDataTable.Rows.Count > 0;
        }

        public void CreateReport(int MegaOrderID, int ClientID, bool bNeedProfilList, bool bNeedTPSList, bool Attach, bool ColorFullName)
        {
            int[] MainOrders = GetMainOrders(MegaOrderID);

            int[] ProfilMainOrders = GetMainOrders(MegaOrderID, 1);
            int[] TPSMainOrders = GetMainOrders(MegaOrderID, 2);

            int FactoryID = 1;

            string ClientName = GetClientName(MegaOrderID);
            string DocNumber = GetOrderNumber(MegaOrderID);

            string Firm = "Профиль+ТПС";

            ClientName = ClientName.Replace('/', '-');

            if (ProfilMainOrders.Count() == 0 && TPSMainOrders.Count() == 0)
            {
                MessageBox.Show("Выбранный заказ пуст");
                return;
            }

            CurrentMegaOrderID = MegaOrderID;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            if (Attach)
            {
                PackingList = new Dispatch.MarketingPackingList()
                {
                    ColorFullName = ColorFullName
                };
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }

                        PackingList.CreateReport(ref hssfworkbook, MainOrders, MegaOrderID, ClientID, 0);
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, MegaOrderID, ClientID, FactoryID);
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, MegaOrderID, ClientID, FactoryID);
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        Firm = "Профиль";
                        if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, MegaOrderID, ClientID, FactoryID);
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        Firm = "ТПС";
                        if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, MegaOrderID, ClientID, FactoryID);
                        }
                    }
                }

                //string FileName = GetFileName(MegaOrderID, "MarketingAttachDispatchReportPath.config", Firm);

                //FileInfo file = new FileInfo(FileName);
                //if (file.Exists)
                //{
                //    try
                //    {
                //        file.Delete();
                //    }
                //    catch (System.IO.IOException e)
                //    {
                //        MessageBox.Show(e.Message, "Ошибка сохранения");
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message, "Ошибка сохранения");
                //    }
                //}

                ClientName = ClientName.Replace('\"', '\'');

                string FileName = ClientName + " № " + DocNumber + " " + Firm;

                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }

                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                System.Diagnostics.Process.Start(file.FullName);
            }


            else
            {
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        Firm = "Профиль";
                        if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        Firm = "ТПС";
                        if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }
                    }
                }

                //string FileName = GetFileName(MegaOrderID, "MarketingDispatchReportPath.config", Firm);

                //FileInfo file = new FileInfo(FileName);
                //if (file.Exists)
                //{
                //    try
                //    {
                //        file.Delete();
                //    }
                //    catch (System.IO.IOException e)
                //    {
                //        MessageBox.Show(e.Message, "Ошибка сохранения");
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message, "Ошибка сохранения");
                //    }
                //}

                ClientName = ClientName.Replace('\"', '\'');

                string FileName = ClientName + " № " + DocNumber + " " + Firm;

                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }

                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                System.Diagnostics.Process.Start(file.FullName);
            }

        }

        //public void CreateClientReport(int MegaOrderID, bool bNeedProfilList, bool bNeedTPSList, bool Attach)
        //{
        //    int[] MainOrders = GetMainOrders(MegaOrderID);

        //    int[] ProfilMainOrders = GetMainOrders(MegaOrderID, 1);
        //    int[] TPSMainOrders = GetMainOrders(MegaOrderID, 2);

        //    int FactoryID = 1;

        //    string ClientName = GetClientName(MegaOrderID);
        //    string DocNumber = GetOrderNumber(MegaOrderID);

        //    string Firm = "Профиль+ТПС";

        //    ClientName = ClientName.Replace('/', '-');

        //    if (ProfilMainOrders.Count() == 0 && TPSMainOrders.Count() == 0)
        //    {
        //        MessageBox.Show("Выбранный заказ пуст");
        //        return;
        //    }

        //    CurrentMegaOrderID = MegaOrderID;

        //    HSSFWorkbook hssfworkbook = new HSSFWorkbook();

        //    ////create a entry of DocumentSummaryInformation
        //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        //    dsi.Company = "NPOI Team";
        //    hssfworkbook.DocumentSummaryInformation = dsi;

        //    ////create a entry of SummaryInformation
        //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        //    si.Subject = "NPOI SDK Example";
        //    hssfworkbook.SummaryInformation = si;

        //    if (Attach)
        //    {
        //        PackingList = new Dispatch.MarketingPackingList();

        //        if (bNeedProfilList && bNeedTPSList)
        //        {
        //            if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
        //            {
        //                FactoryID = 1;
        //                //if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
        //                //{
        //                //    CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
        //                //}

        //                //FactoryID = 2;
        //                //if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
        //                //{
        //                //    CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
        //                //}

        //                PackingList.CreateClientReport(ref hssfworkbook, MainOrders, MegaOrderID, 0);
        //            }
        //            if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
        //            {
        //                FactoryID = 1;
        //                if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
        //                {
        //                    //CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
        //                    PackingList.CreateClientReport(ref hssfworkbook, MainOrders, MegaOrderID, FactoryID);
        //                }
        //            }
        //            if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
        //            {
        //                FactoryID = 2;
        //                if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
        //                {
        //                    //CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
        //                    PackingList.CreateClientReport(ref hssfworkbook, MainOrders, MegaOrderID, FactoryID);
        //                }
        //            }
        //        }

        //        if (bNeedProfilList && !bNeedTPSList)
        //        {
        //            if (ProfilMainOrders.Count() > 0)
        //            {
        //                FactoryID = 1;
        //                Firm = "Профиль";
        //                if (Fill(ProfilMainOrders, MegaOrderID, FactoryID))
        //                {
        //                    //CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
        //                    PackingList.CreateClientReport(ref hssfworkbook, MainOrders, MegaOrderID, FactoryID);
        //                }
        //            }
        //        }

        //        if (!bNeedProfilList && bNeedTPSList)
        //        {
        //            if (TPSMainOrders.Count() > 0)
        //            {
        //                FactoryID = 2;
        //                Firm = "ТПС";
        //                if (Fill(TPSMainOrders, MegaOrderID, FactoryID))
        //                {
        //                    //CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
        //                    PackingList.CreateClientReport(ref hssfworkbook, MainOrders, MegaOrderID, FactoryID);
        //                }
        //            }
        //        }

        //        //string FileName = GetFileName(MegaOrderID, "MarketingAttachDispatchReportPath.config", Firm);

        //        //FileInfo file = new FileInfo(FileName);
        //        //if (file.Exists)
        //        //{
        //        //    try
        //        //    {
        //        //        file.Delete();
        //        //    }
        //        //    catch (System.IO.IOException e)
        //        //    {
        //        //        MessageBox.Show(e.Message, "Ошибка сохранения");
        //        //    }
        //        //    catch (Exception e)
        //        //    {
        //        //        Console.WriteLine(e.Message, "Ошибка сохранения");
        //        //    }
        //        //}

        //        ClientName = ClientName.Replace('\"', '\'');

        //        string FileName = ClientName + " № " + DocNumber + " " + Firm;

        //        string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
        //        FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
        //        int j = 1;
        //        while (file.Exists == true)
        //        {
        //            file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
        //        }

        //        FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
        //        hssfworkbook.Write(NewFile);
        //        NewFile.Close();

        //        System.Diagnostics.Process.Start(file.FullName);
        //    }
        //}

        private void CreateExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int MegaOrderID, int FactoryID, string Firm)
        {
            HSSFSheet sheet1 = hssfworkbook.CreateSheet(Firm);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.FontHeightInPoints = 13;
            HeaderFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont.FontName = "Calibri";

            HSSFFont NotesFont = hssfworkbook.CreateFont();
            NotesFont.FontHeightInPoints = 12;
            NotesFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            NotesFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 12;
            SimpleFont.FontName = "Calibri";

            HSSFFont TempFont = hssfworkbook.CreateFont();
            TempFont.FontHeightInPoints = 12;
            TempFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle EmptyCellStyle = hssfworkbook.CreateCellStyle();
            EmptyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.RightBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderStyle.WrapText = true;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainOrderCellStyle = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle MainOrderCellStyle1 = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle NotesCellStyle = hssfworkbook.CreateCellStyle();
            NotesCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.SetFont(NotesFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle SimpleCellStyle1 = hssfworkbook.CreateCellStyle();
            SimpleCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TempFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.BottomBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 2, "Утверждаю........");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            HSSFCell DispatchDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(1), 0, "Дата отгрузки: " + GetDispatchDate(MegaOrderID, FactoryID));
            DispatchDateCell.CellStyle = TempStyle;
            HSSFCell FirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Участок: " + Firm);
            FirmCell.CellStyle = TempStyle;

            int RowIndex = 4;
            int TopRowFront = 1;
            int BottomRowFront = 1;

            decimal Weight = 0;
            decimal TotalFrontsSquare = 0;

            string Notes = string.Empty;

            sheet1.SetColumnWidth(0, 55 * 256);
            sheet1.SetColumnWidth(1, 12 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);
            sheet1.SetColumnWidth(3, 12 * 256);

            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Клиент/Заказ");
            cell1.CellStyle = HeaderStyle;
            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Кол-во упаковок, фасады");
            cell2.CellStyle = HeaderStyle;
            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Кол-во упаковок, декор");
            cell3.CellStyle = HeaderStyle;
            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Кол-во упаковок, общее");
            cell4.CellStyle = HeaderStyle;

            TopRowFront = RowIndex;
            BottomRowFront = ResultDataTable.Rows.Count + RowIndex;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrders[i]));

                for (int y = 0; y < ResultDataTable.Columns.Count; y++)
                {
                    if (Notes.Length > 0)
                    {
                        if (AttachResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(ResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle1;//нижняя линия не рисуется
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(ResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle1;
                        }


                        if (Notes.Length > 0)
                        {
                            HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                            EmptyCell.CellStyle = EmptyCellStyle;

                            HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                            cell.SetCellValue("Примечание: " + Notes);
                            cell.CellStyle = NotesCellStyle;
                        }
                    }

                    else
                    {
                        if (ResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(ResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle;
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(ResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle;
                        }
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                        EmptyCell.CellStyle = EmptyCellStyle;

                        HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                        cell.SetCellValue("Примечание: " + Notes);
                        cell.CellStyle = NotesCellStyle;
                    }
                }

                if (Notes.Length > 0)
                {
                    RowIndex++;
                    BottomRowFront++;
                }


                RowIndex++;
            }

            RowIndex++;


            RowIndex++;

            Weight = GetWeight(CurrentMegaOrderID, FactoryID);
            TotalFrontsSquare = GetSquare(CurrentMegaOrderID, FactoryID);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Итого:");
            cell13.CellStyle = TotalStyle;

            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Площадь: " + TotalFrontsSquare + " м.кв.");
            cell14.CellStyle = TempStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Вес: " + Weight + " кг");
            cell15.CellStyle = TempStyle;

            if (FactoryID == 1)
            {
                HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Упаковок: " + PackagesCount.ProfilPackedPackages + "/" + PackagesCount.ProfilPackages);
                cell16.CellStyle = TempStyle;
            }
            if (FactoryID == 2)
            {
                HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Упаковок: " + PackagesCount.TPSPackedPackages + "/" + PackagesCount.TPSPackages);
                cell16.CellStyle = TempStyle;
            }
        }
        #endregion
    }








    public class MarketingPackingList : IAllFrontParameterName, IIsMarsel
    {
        int ClientID = 0;
        public bool ColorFullName = false;
        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        public MarketingPackingList()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();
        }

        private void GetColorsDT()
        {
            FrameColorsDataTable = new DataTable();
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
            }
        }

        private void Create()
        {
            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            FrontsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            PatinaDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            GetColorsDT();
            GetInsetColorsDT();
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInset"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("ConfirmDateTime"), System.Type.GetType("System.String")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Product"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Color"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Height"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Width"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Count"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("ConfirmDateTime"), System.Type.GetType("System.String")));
        }

        #region Реализация интерфейса IReference

        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        public string GetOrderNumber(int MegaOrderID)
        {
            string OrderNumber = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT OrderNumber FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        OrderNumber = DT.Rows[0]["OrderNumber"].ToString();
                }
            }
            return OrderNumber;
        }

        public string GetClientName(int MegaOrderID)
        {
            int ClientID = 0;
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetDispatchDate(int MegaOrderID, int FactoryID)
        {
            string DispatchDate = string.Empty;
            string Date = "ProfilDispatchDate";

            if (FactoryID == 2)
                Date = "TPSDispatchDate";

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT " + Date + " FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0][Date].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0][Date]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        public string GetDispatchDate(int MegaOrderID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }
        #endregion

        #region Реализация интерфейса

        public bool IsMarsel3(int FrontID)
        {
            return FrontID == 3630;
        }

        public bool IsMarsel4(int FrontID)
        {
            return FrontID == 15003;
        }
        public bool IsImpost(int TechnoProfileID)
        {
            return TechnoProfileID == -1;
        }
        public string GetFrontName(int FrontID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }

        public string GetFront2Name(int TechnoProfileID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + TechnoProfileID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }
        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
                ColorName = Rows[0]["ColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetPatinaName(int PatinaID)
        {
            string PatinaName = string.Empty;
            try
            {
                DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
                PatinaName = Rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return PatinaName;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            string InsetType = string.Empty;
            try
            {
                DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
                InsetType = Rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetType;
        }

        public string GetInsetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + ColorID);
                ColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            //if (ClientID == 101)
            //    return Rows[0]["OldName"].ToString();
            //else
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }
        #endregion

        private decimal GetSquare(DataTable DT)
        {
            decimal Square = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    Square += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return Square;
        }

        private int GetCount(DataTable DT)
        {
            int Count = 0;

            foreach (DataRow Row in DT.Rows)
            {
                Count += Convert.ToInt32(Row["Count"]);
            }

            return Count;
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        public DataTable PackageFrontsSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        public DataTable PackageDecorSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        private bool FillFronts()
        {
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                var InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                var bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                var bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    var bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }
                NewRow["Front"] = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetType"] = InsetType;
                NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Square"] = Row["Square"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                NewRow["ConfirmDateTime"] = Row["ConfirmDateTime"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FillDecor()
        {
            DecorResultDataTable.Clear();

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                DataRow NewRow = DecorResultDataTable.NewRow();

                NewRow["Product"] = GetProductName(Convert.ToInt32(Row["ProductID"])) + " " + GetDecorName(Convert.ToInt32(Row["DecorID"]));

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                {
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                {
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    NewRow["Width"] = Convert.ToInt32(Row["Width"]);

                string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    NewRow["Color"] = Color;
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                NewRow["ConfirmDateTime"] = Row["ConfirmDateTime"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterFrontsOrders(int MainOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FrontsOrders.FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.*, MegaOrders.ConfirmDateTime FROM FrontsOrders" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE FrontsOrders.MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders(int MainOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND DecorOrders.FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.*, MegaOrders.ConfirmDateTime FROM DecorOrders" +
                " INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE DecorOrders.MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            //if (Convert.ToInt32(ORow[0]["ColorID"]) == -1)
                            //    NewRow["ColorID"] = 0;
                            //else
                            NewRow["ColorID"] = ORow[0]["ColorID"];
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            DecorOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool HasFronts(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        private bool HasDecor(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        public void CreateReport(ref HSSFWorkbook thssfworkbook, int[] MainOrders, int MegaOrderID, int iClientID, int FactoryID)
        {
            string SheetName = "Ведомость Профиль+ТПС";
            ClientID = iClientID;
            if (FactoryID == 1)
                SheetName = "Ведомость Профиль";
            if (FactoryID == 2)
                SheetName = "Ведомость ТПС";

            HSSFSheet sheet1 = thssfworkbook.CreateSheet(SheetName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 3;

            HSSFCell Cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Серым цветом отмечены отсутствующие упаковки");

            if (HasFronts(MainOrders, FactoryID))
            {
                CreateFrontsExcel(ref thssfworkbook, MainOrders, MegaOrderID, ref RowIndex, FactoryID, SheetName);
            }

            if (HasDecor(MainOrders, FactoryID))
            {
                CreateDecorExcel(ref thssfworkbook, MainOrders, MegaOrderID, RowIndex, FactoryID, SheetName);
            }
        }

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int MegaOrderID, ref int RowIndex, int FactoryID, string SheetName)
        {
            string DispatchDate = string.Empty;

            string OrderNumber = string.Empty;
            string ClientName = string.Empty;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            int DisplayIndex = 0;
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 4 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 18 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 7 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 9 * 256);

            //if (ColorFullName)
            //{
            //    hssfworkbook.GetSheet(SheetName).SetColumnWidth(3, 18 * 256);
            //    hssfworkbook.GetSheet(SheetName).SetColumnWidth(4, 15 * 256);
            //    hssfworkbook.GetSheet(SheetName).SetColumnWidth(5, 18 * 256);
            //}
            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 7, "Утверждаю........");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterFrontsOrders(MainOrders[i], FactoryID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                OrderNumber = GetOrderNumber(MegaOrderID);
                ClientName = GetClientName(MegaOrderID);
                DispatchDate = GetDispatchDate(MegaOrderID);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " +
                    ClientName + " № " + OrderNumber + " - " + MainOrders[i]);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;
                cell12 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Согласовано");
                cell12.CellStyle = HeaderStyle;

                //вывод заказов фасадов
                for (int index = 0; index < PackageFrontsSequence.Rows.Count; index++)
                {
                    DataRow[] FRows = FrontsResultDataTable.Select("[PackNumber] = " + PackageFrontsSequence.Rows[index]["PackNumber"]);

                    if (FRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = FRows.Count() + TopIndex - 1;

                    for (int x = 0; x < FRows.Count(); x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                        {
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }


                        }
                        RowIndex++;
                    }

                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(++RowIndex), 8, Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                    cellStyle.SetFont(PackNumberFont);
                    cell.CellStyle = cellStyle;
                }

                if (IsFronts)
                    RowIndex++;


                RowIndex++;
            }

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int MegaOrderID, int RowIndex, int FactoryID, string SheetName)
        {
            string DispatchDate = string.Empty;

            string OrderNumber = string.Empty;
            string ClientName = string.Empty;

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            if (RowIndex < 1)
            {
                HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
                CreateDateCell.CellStyle = TempStyle;
            }

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterDecorOrders(MainOrders[i], FactoryID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                OrderNumber = GetOrderNumber(MegaOrderID);
                ClientName = GetClientName(MegaOrderID);
                DispatchDate = GetDispatchDate(MegaOrderID);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " № " + OrderNumber + " - " + MainOrders[i]);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;
                cell21 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Согласовано");
                cell21.CellStyle = HeaderStyle;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0 || y == 1)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3)
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }





                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
            }

            for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }

        #region ClientReport
        public void CreateClientReport(ref HSSFWorkbook thssfworkbook, int[] MainOrders, int MegaOrderID, int FactoryID)
        {
            string SheetName = "Ведомость Профиль+ТПС";

            if (FactoryID == 1)
                SheetName = "Ведомость Профиль";
            if (FactoryID == 2)
                SheetName = "Ведомость ТПС";

            HSSFSheet sheet1 = thssfworkbook.CreateSheet(SheetName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 3;

            HSSFCell Cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Серым цветом отмечены отсутствующие упаковки");

            if (HasFronts(MainOrders, FactoryID))
            {
                CreateClientFrontsExcel(ref thssfworkbook, MainOrders, MegaOrderID, ref RowIndex, FactoryID, SheetName);
            }

            if (HasDecor(MainOrders, FactoryID))
            {
                CreateClientDecorExcel(ref thssfworkbook, MainOrders, MegaOrderID, RowIndex, FactoryID, SheetName);
            }
        }

        private void CreateClientFrontsExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int MegaOrderID, ref int RowIndex, int FactoryID, string SheetName)
        {
            string DispatchDate = string.Empty;

            string OrderNumber = string.Empty;
            string ClientName = string.Empty;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            int DisplayIndex = 0;
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 4 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 18 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 7 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 9 * 256);

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 7, "Утверждаю........");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterFrontsOrders(MainOrders[i], FactoryID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                OrderNumber = GetOrderNumber(MegaOrderID);
                ClientName = GetClientName(MegaOrderID);
                DispatchDate = GetDispatchDate(MegaOrderID);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " № " + OrderNumber + " - " + MainOrders[i]);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;
                cell12 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Согласовано");
                cell12.CellStyle = HeaderStyle;

                //вывод заказов фасадов
                for (int index = 0; index < PackageFrontsSequence.Rows.Count; index++)
                {
                    DataRow[] FRows = FrontsResultDataTable.Select("[PackNumber] = " + PackageFrontsSequence.Rows[index]["PackNumber"]);

                    if (FRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = FRows.Count() + TopIndex - 1;

                    for (int x = 0; x < FRows.Count(); x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                        {
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }


                        }
                        RowIndex++;
                    }

                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(++RowIndex), 8, Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                    cellStyle.SetFont(PackNumberFont);
                    cell.CellStyle = cellStyle;
                }

                if (IsFronts)
                    RowIndex++;


                RowIndex++;
            }

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateClientDecorExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int MegaOrderID, int RowIndex, int FactoryID, string SheetName)
        {
            string DispatchDate = string.Empty;

            string OrderNumber = string.Empty;
            string ClientName = string.Empty;

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            if (RowIndex < 1)
            {
                HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
                CreateDateCell.CellStyle = TempStyle;
            }

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterDecorOrders(MainOrders[i], FactoryID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                OrderNumber = GetOrderNumber(MegaOrderID);
                ClientName = GetClientName(MegaOrderID);
                DispatchDate = GetDispatchDate(MegaOrderID);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " № " + OrderNumber + " - " + MainOrders[i]);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;
                cell21 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Согласовано");
                cell21.CellStyle = HeaderStyle;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0 || y == 1)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3)
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }





                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
            }

            for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }
        #endregion
    }








    public class ZOVDispatchList
    {
        ZOVPackingList PackingList;

        PackagesCount PackagesCount;

        private int CurrentMegaOrderID = 0;

        DataTable ClientsDataTable = null;
        DataTable SimpleResultDataTable = null;
        DataTable MegaOrdersDataTable = null;
        DataTable MainOrdersDataTable = null;
        DataTable MainOrdersInfoDT = null;
        DataTable AllPackagesDT = null;

        public BindingSource MegaOrdersBindingSource = null;

        SqlDataAdapter MegaOrdersDataAdapter = null;

        public PercentageDataGrid MegaOrdersDataGrid = null;

        public ZOVDispatchList(ref PercentageDataGrid tMegaOrdersDataGrid)
        {
            MegaOrdersDataGrid = tMegaOrdersDataGrid;

            Create();
            Fill();
            MegaGridSetting();
        }

        private void Create()
        {
            ClientsDataTable = new DataTable();
            SimpleResultDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();
            MainOrdersInfoDT = new DataTable();
            AllPackagesDT = new DataTable();

            MegaOrdersBindingSource = new BindingSource();

            SimpleResultDataTable.Columns.Add(new DataColumn(("MainOrder"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("FrontsPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("DecorPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("AllPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("Dispatched"), System.Type.GetType("System.Boolean")));

        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        private void Fill()
        {
            DateTime CurrentDate = GetCurrentDate.AddDays(-8);
            string DispatchDate = CurrentDate.ToString("yyyy-MM-dd");

            MegaOrdersDataTable.Clear();
            string SellectionCommand = "SELECT * FROM MegaOrders" +
                    " WHERE MegaOrderID<>0 AND DispatchDate >='" + DispatchDate + "'" +
                    " AND (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2 OR ProfilPackAllocStatusID=1 OR TPSPackAllocStatusID=1)" +
                    " AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                    " WHERE (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2))" +
                    " ORDER BY DispatchDate";

            MegaOrdersDataAdapter = new SqlDataAdapter(SellectionCommand, ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            MegaOrdersDataTable.Columns.Add(new DataColumn(("ProfilPackagesCount"), System.Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn(("TPSPackagesCount"), System.Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;

            FillMainPackedColumn();
        }

        public void Filter(DateTime FirstDate, DateTime SecondDate)
        {
            MegaOrdersDataTable.Clear();
            string SellectionCommand = "SELECT * FROM MegaOrders" +
                    " WHERE MegaOrderID<>0 AND DispatchDate >='" + FirstDate.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" + SecondDate.ToString("yyyy-MM-dd") + "'" +
                    " AND (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2 OR ProfilPackAllocStatusID=1 OR TPSPackAllocStatusID=1)" +
                    " AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                    " WHERE (ProfilPackAllocStatusID=2 OR TPSPackAllocStatusID=2))" +
                    " ORDER BY DispatchDate";

            MegaOrdersDataAdapter.Dispose();
            MegaOrdersDataAdapter = new SqlDataAdapter(SellectionCommand, ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            FillMainPackedColumn();
        }

        private void FillMainPackedColumn()
        {
            int ProfilPackedPackagesCount = 0;
            int TPSPackedPackagesCount = 0;

            int ProfilPackagesCount = 0;
            int TPSPackagesCount = 0;

            int MegaOrderID = 0;

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                ProfilPackedPackagesCount = 0;
                TPSPackedPackagesCount = 0;

                ProfilPackagesCount = 0;
                TPSPackagesCount = 0;

                MegaOrderID = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]);

                ProfilPackedPackagesCount += GetFirmPackedPackagesCount(MegaOrderID, 1);
                ProfilPackagesCount += GetFirmPackagesCount(MegaOrderID, 1);

                TPSPackedPackagesCount += GetFirmPackedPackagesCount(MegaOrderID, 2);
                TPSPackagesCount += GetFirmPackagesCount(MegaOrderID, 2);

                MegaOrdersDataTable.Rows[i]["ProfilPackagesCount"] = ProfilPackedPackagesCount + " / " + ProfilPackagesCount;
                MegaOrdersDataTable.Rows[i]["TPSPackagesCount"] = TPSPackedPackagesCount + " / " + TPSPackagesCount;
            }
        }

        private void MegaGridSetting()
        {
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;

            foreach (DataGridViewColumn Column in MegaOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            MegaOrdersDataGrid.Columns["DispatchStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TotalCost"].Visible = false;
            MegaOrdersDataGrid.Columns["DispatchedCost"].Visible = false;
            MegaOrdersDataGrid.Columns["DispatchedDebtCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CalcDebtCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CalcDefectsCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
            MegaOrdersDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
            MegaOrdersDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
            MegaOrdersDataGrid.Columns["WriteOffDebtCost"].Visible = false;

            MegaOrdersDataGrid.Columns["WriteOffDefectsCost"].Visible = false;
            MegaOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].Visible = false;
            MegaOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].Visible = false;
            MegaOrdersDataGrid.Columns["TotalWriteOffCost"].Visible = false;
            MegaOrdersDataGrid.Columns["TotalCalcWriteOffCost"].Visible = false;
            MegaOrdersDataGrid.Columns["IncomeCost"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfitCost"].Visible = false;

            MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSPackCount"].Visible = false;

            //if (FactoryID == 1)
            //    MegaOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            //else
            //    MegaOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDataGrid.Columns["DispatchDate"].HeaderText = "Дата отгрузки";
            MegaOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDataGrid.Columns["Square"].HeaderText = " Площадь фасадов, м.кв.";
            MegaOrdersDataGrid.Columns["ProfilPackagesCount"].HeaderText = "Кол-во уп., Профиль";
            MegaOrdersDataGrid.Columns["TPSPackagesCount"].HeaderText = "Кол-во уп., ТПС";

            MegaOrdersDataGrid.Columns["ProfilPackagesCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["TPSPackagesCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 1
            };
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.AutoGenerateColumns = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].DisplayIndex = 0;
            MegaOrdersDataGrid.Columns["DispatchDate"].DisplayIndex = 1;
            MegaOrdersDataGrid.Columns["Weight"].DisplayIndex = 4;
            MegaOrdersDataGrid.Columns["Square"].DisplayIndex = 5;

            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MegaOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Square"].Width = 220;
            MegaOrdersDataGrid.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["DispatchDate"].Width = 150;
            MegaOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Weight"].Width = 110;
            MegaOrdersDataGrid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["MegaOrderID"].Width = 100;
            //MegaOrdersDataGrid.Columns["TPSPackagesCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //MegaOrdersDataGrid.Columns["TPSPackagesCount"].Width = 160;
            //MegaOrdersDataGrid.Columns["ProfilPackagesCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //MegaOrdersDataGrid.Columns["ProfilPackagesCount"].Width = 160;
        }

        #region Реализация интерфейса IReference
        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            //using (DataTable DT = new DataTable())
            //{
            //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
            //        MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            //    {
            //        DA.Fill(DT);
            //        if (DT.Rows.Count > 0)
            //            Notes = DT.Rows[0]["Notes"].ToString();
            //    }
            //}

            DataRow[] Rows = MainOrdersInfoDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() > 0)
                Notes = Rows[0]["Notes"].ToString();

            return Notes;
        }

        public string GetOrderNumber(int MainOrderID)
        {
            string DocNumber = string.Empty;
            //using (DataTable DT = new DataTable())
            //{
            //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocNumber FROM MainOrders" +
            //        " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            //    {
            //        DA.Fill(DT);
            //        if (DT.Rows.Count > 0)
            //            DocNumber = DT.Rows[0]["DocNumber"].ToString();
            //    }
            //}

            DataRow[] Rows = MainOrdersInfoDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() > 0)
                DocNumber = Rows[0]["DocNumber"].ToString();

            return DocNumber;
        }

        public string GetClientName(int MainOrderID)
        {
            int ClientID = 0;
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        private bool IsDoNotDispatch(int MainOrderID)
        {
            //using (DataTable DT = new DataTable())
            //{
            //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DoNotDispatch  FROM MainOrders" +
            //        " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            //    {
            //        DA.Fill(DT);
            //        if (DT.Rows.Count > 0)
            //            return Convert.ToBoolean(DT.Rows[0]["DoNotDispatch"]);
            //    }
            //}
            bool DoNotDispatch = false;
            DataRow[] Rows = MainOrdersInfoDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() > 0)
                DoNotDispatch = Convert.ToBoolean(Rows[0]["DoNotDispatch"]);

            return DoNotDispatch;
        }

        public string GetDispatchDate(int MainOrderID, int FactoryID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID + ")", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        public string GetDispatchDate(int MegaOrderID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }
        #endregion

        public int[] GetMainOrders(int MegaOrderID, int FactoryID)
        {
            int[] rows = { 0 };
            string PackAllocStatusID = "ProfilPackAllocStatusID = 2";

            if (FactoryID == 2)
                PackAllocStatusID = "TPSPackAllocStatusID = 2";

            if (MegaOrdersBindingSource.Count > 0)
            {
                MainOrdersDataTable.Clear();
                string SelectionCommand = "SELECT * FROM MainOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID +
                    " AND " + PackAllocStatusID;

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(MainOrdersDataTable);
                }

                rows = new int[MainOrdersDataTable.Rows.Count];

                for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                    rows[i] = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);
            }

            return rows;
        }

        private DateTime CurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        private int GetFirmPackagesCount(int MegaOrderID, int FactoryID)
        {
            int PackagesCount;

            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID IN(" +
                "SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID +
                ") AND FactoryID = " + FactoryID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                if (DA.Fill(DT) < 1)
                    return 0;

                PackagesCount = DT.Rows.Count;

                DT.Dispose();
            }

            return PackagesCount;
        }

        private int GetFirmPackedPackagesCount(int MegaOrderID, int FactoryID)
        {
            int PackagesCount;

            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageStatusID = 3 AND MainOrderID IN(" +
                "SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID +
                ") AND FactoryID = " + FactoryID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                if (DA.Fill(DT) < 1)
                    return 0;

                PackagesCount = DT.Rows.Count;

                DT.Dispose();
            }

            return PackagesCount;
        }

        private int GetPackedPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            int PackagesCount = 0;

            //DataTable DT = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
            //    " AND PackageStatusID = 3 AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID,
            //    ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    if (DA.Fill(DT) < 1)
            //        return 0;

            //    PackagesCount = DT.Rows.Count;

            //    DT.Dispose();
            //}

            DataRow[] Rows = AllPackagesDT.Select("MainOrderID = " + MainOrderID + " AND PackageStatusID = 3 AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID);
            PackagesCount = Rows.Count();

            return PackagesCount;
        }

        private int GetPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            int PackagesCount = 0;

            //DataTable DT = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
            //    " AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID,
            //    ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    if (DA.Fill(DT) < 1)
            //        return 0;

            //    PackagesCount = DT.Rows.Count;

            //    DT.Dispose();
            //}

            DataRow[] Rows = AllPackagesDT.Select("MainOrderID = " + MainOrderID + " AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID);
            PackagesCount = Rows.Count();

            return PackagesCount;
        }

        private decimal GetSquare(int MegaOrderID, int FactoryID)
        {
            decimal Square = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Square) As Square FROM FrontsOrders WHERE FrontsOrdersID IN (" +
                    "SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE FactoryID = " + FactoryID +
                    " AND PackageStatusID = 3 AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Square"] != DBNull.Value)
                    {
                        Square = Convert.ToDecimal(DT.Rows[0]["Square"]);
                        Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }

            return Square;
        }

        private decimal GetWeight(int MegaOrderID, int FactoryID)
        {
            decimal Weight = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM FrontsOrders WHERE FrontsOrdersID IN (" +
                    "SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND FactoryID = " + FactoryID +
                    " AND PackageStatusID = 3 AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        Weight = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM DecorOrders WHERE DecorOrderID IN (" +
                    "SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND FactoryID = " + FactoryID +
                    " AND PackageStatusID = 3 AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        decimal Temp = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                        Weight += Temp;
                    }
                }

                Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);
            }

            return Weight;
        }

        private int[] GetMainOrders(int MegaOrderID)
        {
            int[] rows = { 0 };

            if (MegaOrdersBindingSource.Count > 0)
            {
                MainOrdersDataTable.Clear();
                MainOrdersInfoDT.Clear();
                string SelectionCommand = "SELECT * FROM MainOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID +
                    " AND (ProfilPackAllocStatusID = 2 OR TPSPackAllocStatusID = 2)";

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(MainOrdersDataTable);
                    DA.Fill(MainOrdersInfoDT);
                }

                rows = new int[MainOrdersDataTable.Rows.Count];

                for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                    rows[i] = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID, ProductType, MainOrderID, FactoryID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllPackagesDT.Clear();
                DA.Fill(AllPackagesDT);
            }

            return rows;
        }

        #region SimpleReport
        private bool Fill(int[] MainOrders, int FactoryID)
        {
            SimpleResultDataTable.Clear();

            PackagesCount.AllPackages = 0;
            PackagesCount.AllPackedPackages = 0;
            PackagesCount.ProfilPackages = 0;
            PackagesCount.ProfilPackedPackages = 0;
            PackagesCount.TPSPackages = 0;
            PackagesCount.TPSPackedPackages = 0;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            int MainOrderID = 0;
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                int FrontsPackedPackagesCount = 0;
                int DecorPackedPackagesCount = 0;
                int AllPackedPackagesCount = 0;

                int FrontsPackagesCount = 0;
                int DecorPackagesCount = 0;
                int AllPackagesCount = 0;

                string ClientName = string.Empty;
                string DocNumber = string.Empty;
                MainOrderID = MainOrders[i];

                FrontsPackedPackagesCount = GetPackedPackagesCount(MainOrders[i], FactoryID, 0);
                DecorPackedPackagesCount = GetPackedPackagesCount(MainOrders[i], FactoryID, 1);
                AllPackedPackagesCount = FrontsPackedPackagesCount + DecorPackedPackagesCount;

                PackagesCount.AllPackedPackages += AllPackedPackagesCount;
                PackagesCount.ProfilPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;
                PackagesCount.TPSPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;

                FrontsPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 0);
                DecorPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 1);
                AllPackagesCount = FrontsPackagesCount + DecorPackagesCount;

                PackagesCount.AllPackages += AllPackagesCount;
                PackagesCount.ProfilPackages += FrontsPackagesCount + DecorPackagesCount;
                PackagesCount.TPSPackages += FrontsPackagesCount + DecorPackagesCount;

                ClientName = GetClientName(MainOrders[i]);
                DocNumber = GetOrderNumber(MainOrders[i]);

                DataRow NewRow = SimpleResultDataTable.NewRow();

                NewRow["MainOrder"] = ClientName + " / " + DocNumber;
                if (FrontsPackagesCount > 0)
                    NewRow["FrontsPackagesCount"] = FrontsPackedPackagesCount.ToString() + " / " + FrontsPackagesCount.ToString();
                if (DecorPackagesCount > 0)
                    NewRow["DecorPackagesCount"] = DecorPackedPackagesCount.ToString() + " / " + DecorPackagesCount.ToString();

                NewRow["AllPackagesCount"] = AllPackedPackagesCount.ToString() + " / " + AllPackagesCount.ToString();

                if (AllPackedPackagesCount == AllPackagesCount)
                    NewRow["Dispatched"] = true;
                else
                    NewRow["Dispatched"] = false;

                SimpleResultDataTable.Rows.Add(NewRow);
            }

            sw.Stop();
            double G1 = sw.Elapsed.TotalSeconds;
            sw.Restart();
            return SimpleResultDataTable.Rows.Count > 0;
        }

        public void CreateReport(int MegaOrderID, bool bNeedProfilList, bool bNeedTPSList, bool Attach)
        {
            int[] MainOrders = GetMainOrders(MegaOrderID);

            string DispatchDate = GetDispatchDate(MegaOrderID);
            int[] ProfilMainOrders = GetMainOrders(MegaOrderID, 1);
            int[] TPSMainOrders = GetMainOrders(MegaOrderID, 2);

            int FactoryID = 1;

            //string ClientName = GetClientName(MegaOrderID);
            //string DocNumber = GetOrderNumber(MegaOrderID);

            string Firm = "Профиль+ТПС";

            //ClientName = ClientName.Replace('/', '-');

            if (ProfilMainOrders.Count() == 0 && TPSMainOrders.Count() == 0)
            {
                MessageBox.Show("Выбранный заказ пуст");
                return;
            }

            CurrentMegaOrderID = MegaOrderID;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            if (Attach)
            {
                PackingList = new ZOVPackingList();

                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }

                        PackingList.CreateReport(ref hssfworkbook, MainOrders, 0);
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, FactoryID);
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, FactoryID);
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        Firm = "Профиль";
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, FactoryID);
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        Firm = "ТПС";

                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            sw.Stop();
                            double G = sw.Elapsed.TotalSeconds;
                            sw.Restart();
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                            sw.Stop();
                            double G1 = sw.Elapsed.TotalSeconds;
                            sw.Restart();
                            PackingList.CreateReport(ref hssfworkbook, MainOrders, FactoryID);
                            sw.Stop();
                            double G2 = sw.Elapsed.TotalSeconds;
                        }
                    }
                }

                //string FileName = GetFileName(DispatchDate, "ZOVAttachDispatchReportPath.config", Firm);

                //FileInfo file = new FileInfo(FileName);
                //if (file.Exists)
                //{
                //    try
                //    {
                //        file.Delete();
                //    }
                //    catch (System.IO.IOException e)
                //    {
                //        MessageBox.Show(e.Message, "Ошибка сохранения");
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message, "Ошибка сохранения");
                //    }
                //}

                string FileName = DispatchDate + " " + Firm;

                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }

                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                System.Diagnostics.Process.Start(file.FullName);
            }


            else
            {
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        Firm = "Профиль";
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, MegaOrderID, 1, "Профиль");
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        Firm = "ТПС";
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, MegaOrderID, 2, "ТПС");
                        }
                    }
                }

                //string FileName = GetFileName(DispatchDate, "ZOVDispatchReportPath.config", Firm);

                //FileInfo file = new FileInfo(FileName);
                //if (file.Exists)
                //{
                //    try
                //    {
                //        file.Delete();
                //    }
                //    catch (System.IO.IOException e)
                //    {
                //        MessageBox.Show(e.Message, "Ошибка сохранения");
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message, "Ошибка сохранения");
                //    }
                //}

                string FileName = DispatchDate + " " + Firm;

                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }

                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                System.Diagnostics.Process.Start(file.FullName);
            }

        }

        private void CreateExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int MegaOrderID, int FactoryID, string Firm)
        {
            HSSFSheet sheet1 = hssfworkbook.CreateSheet(Firm);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.FontHeightInPoints = 13;
            HeaderFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont.FontName = "Calibri";

            HSSFFont NotesFont = hssfworkbook.CreateFont();
            NotesFont.FontHeightInPoints = 12;
            NotesFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            NotesFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 12;
            SimpleFont.FontName = "Calibri";

            HSSFFont TempFont = hssfworkbook.CreateFont();
            TempFont.FontHeightInPoints = 12;
            TempFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle EmptyCellStyle = hssfworkbook.CreateCellStyle();
            EmptyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.RightBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderStyle.WrapText = true;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainOrderCellStyle = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle MainOrderGreyCellStyle = hssfworkbook.CreateCellStyle();
            MainOrderGreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderGreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderGreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderGreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderGreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderGreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderGreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderGreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderGreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            MainOrderGreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            MainOrderGreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            MainOrderGreyCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderGreyCellStyle.SetFont(SimpleFont);

            HSSFCellStyle MainOrderCellStyle1 = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle MainOrderGreyCellStyle1 = hssfworkbook.CreateCellStyle();
            MainOrderGreyCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderGreyCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderGreyCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderGreyCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderGreyCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderGreyCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderGreyCellStyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            MainOrderGreyCellStyle1.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            MainOrderGreyCellStyle1.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            MainOrderGreyCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderGreyCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle NotesCellStyle = hssfworkbook.CreateCellStyle();
            NotesCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.SetFont(NotesFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle SimpleCellStyle1 = hssfworkbook.CreateCellStyle();
            SimpleCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle1 = hssfworkbook.CreateCellStyle();
            GreyCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyCellStyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle1.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle1.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TempFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.BottomBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 3, "Утверждаю........");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;
            HSSFCell DispatchDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(1), 0, "Дата отгрузки: " + GetDispatchDate(MegaOrderID));
            DispatchDateCell.CellStyle = TempStyle;
            HSSFCell FirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Участок: " + Firm);
            FirmCell.CellStyle = TempStyle;

            int RowIndex = 4;
            int TopRowFront = 1;
            int BottomRowFront = 1;

            decimal Weight = 0;
            decimal TotalFrontsSquare = 0;

            bool DoNotDispatch = false;
            string Notes = string.Empty;

            sheet1.SetColumnWidth(0, 55 * 256);
            sheet1.SetColumnWidth(1, 12 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);
            sheet1.SetColumnWidth(3, 12 * 256);

            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Клиент/Заказ");
            cell1.CellStyle = HeaderStyle;
            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Кол-во упаковок, фасады");
            cell2.CellStyle = HeaderStyle;
            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Кол-во упаковок, декор");
            cell3.CellStyle = HeaderStyle;
            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Кол-во упаковок, общее");
            cell4.CellStyle = HeaderStyle;

            TopRowFront = RowIndex;
            BottomRowFront = SimpleResultDataTable.Rows.Count + RowIndex;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrders[i]));
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);
                for (int y = 0; y < SimpleResultDataTable.Columns.Count - 1; y++)
                {
                    if (Notes.Length > 0 || DoNotDispatch)
                    {
                        if (SimpleResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            if (!Convert.ToBoolean(SimpleResultDataTable.Rows[i]["Dispatched"]))
                                cell.CellStyle = MainOrderGreyCellStyle1;
                            else
                                cell.CellStyle = MainOrderCellStyle1;
                            //cell.CellStyle = MainOrderCellStyle1;//нижняя линия не рисуется
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            if (!Convert.ToBoolean(SimpleResultDataTable.Rows[i]["Dispatched"]))
                                cell.CellStyle = GreyCellStyle1;
                            else
                                cell.CellStyle = SimpleCellStyle1;
                        }


                        if (Notes.Length > 0)
                        {
                            HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                            EmptyCell.CellStyle = EmptyCellStyle;

                            HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                            cell.SetCellValue("Примечание: " + Notes);
                            cell.CellStyle = NotesCellStyle;
                        }
                    }

                    else
                    {
                        if (SimpleResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            if (!Convert.ToBoolean(SimpleResultDataTable.Rows[i]["Dispatched"]))
                                cell.CellStyle = MainOrderGreyCellStyle;
                            else
                                cell.CellStyle = MainOrderCellStyle;
                            //cell.CellStyle = MainOrderCellStyle;
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            if (!Convert.ToBoolean(SimpleResultDataTable.Rows[i]["Dispatched"]))
                                cell.CellStyle = GreyCellStyle;
                            else
                                cell.CellStyle = SimpleCellStyle;
                        }
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                        EmptyCell.CellStyle = EmptyCellStyle;

                        HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                        cell.SetCellValue("Примечание: " + Notes);
                        cell.CellStyle = NotesCellStyle;
                    }
                }

                if (Notes.Length > 0)
                {
                    RowIndex++;
                    BottomRowFront++;
                }


                RowIndex++;
            }

            RowIndex++;


            RowIndex++;

            Weight = GetWeight(CurrentMegaOrderID, FactoryID);
            TotalFrontsSquare = GetSquare(CurrentMegaOrderID, FactoryID);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Итого:");
            cell13.CellStyle = TotalStyle;

            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Площадь: " + TotalFrontsSquare + " м.кв.");
            cell14.CellStyle = TempStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Вес: " + Weight + " кг");
            cell15.CellStyle = TempStyle;

            if (FactoryID == 1)
            {
                HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Упаковок: " + PackagesCount.ProfilPackedPackages + "/" + PackagesCount.ProfilPackages);
                cell16.CellStyle = TempStyle;
            }
            if (FactoryID == 2)
            {
                HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Упаковок: " + PackagesCount.TPSPackedPackages + "/" + PackagesCount.TPSPackages);
                cell16.CellStyle = TempStyle;
            }
        }
        #endregion
    }





    public class ZOVPackingList : IAllFrontParameterName, IIsMarsel
    {
        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        DataTable OriginalFrontsOrdersDataTable = null;
        DataTable OriginalDecorOrdersDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        private DataTable MainOrdersInfoDT = null;

        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        public ZOVPackingList()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();
        }

        private void GetColorsDT()
        {
            FrameColorsDataTable = new DataTable();
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
            }
        }

        private void Create()
        {
            MainOrdersInfoDT = new DataTable();
            OriginalFrontsOrdersDataTable = new DataTable();
            OriginalDecorOrdersDataTable = new DataTable();
            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            FrontsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            PatinaDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            GetColorsDT();
            GetInsetColorsDT();
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInset"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Product"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Color"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Height"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Width"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Count"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        #region Реализация интерфейса IReference

        private void GetMainOrdersInfo(int[] MainOrderIDs)
        {
            string SelectionCommand = "SELECT * FROM MainOrders" +
                       " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                MainOrdersInfoDT.Clear();
                DA.Fill(MainOrdersInfoDT);
            }

            SelectionCommand = "SELECT * FROM FrontsOrders" +
                       " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                OriginalFrontsOrdersDataTable.Clear();
                DA.Fill(OriginalFrontsOrdersDataTable);
            }
            SelectionCommand = "SELECT * FROM DecorOrders" +
                       " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                OriginalDecorOrdersDataTable.Clear();
                DA.Fill(OriginalDecorOrdersDataTable);
            }
        }

        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            //using (DataTable DT = new DataTable())
            //{
            //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
            //        MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            //    {
            //        DA.Fill(DT);
            //        if (DT.Rows.Count > 0)
            //            Notes = DT.Rows[0]["Notes"].ToString();
            //    }
            //}

            DataRow[] Rows = MainOrdersInfoDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() > 0)
                Notes = Rows[0]["Notes"].ToString();

            return Notes;
        }

        public string GetOrderNumber(int MainOrderID)
        {
            string DocNumber = string.Empty;
            //using (DataTable DT = new DataTable())
            //{
            //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocNumber FROM MainOrders" +
            //        " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            //    {
            //        DA.Fill(DT);
            //        if (DT.Rows.Count > 0)
            //            DocNumber = DT.Rows[0]["DocNumber"].ToString();
            //    }
            //}

            DataRow[] Rows = MainOrdersInfoDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() > 0)
                DocNumber = Rows[0]["DocNumber"].ToString();

            return DocNumber;
        }

        public string GetClientName(int MainOrderID)
        {
            int ClientID = 0;
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetDispatchDate(int MegaOrderID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        private bool IsDoNotDispatch(int MainOrderID)
        {
            //using (DataTable DT = new DataTable())
            //{
            //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DoNotDispatch  FROM MainOrders" +
            //        " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            //    {
            //        DA.Fill(DT);
            //        if (DT.Rows.Count > 0)
            //            return Convert.ToBoolean(DT.Rows[0]["DoNotDispatch"]);
            //    }
            //}
            bool DoNotDispatch = false;
            DataRow[] Rows = MainOrdersInfoDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() > 0)
                DoNotDispatch = Convert.ToBoolean(Rows[0]["DoNotDispatch"]);

            return DoNotDispatch;
        }
        #endregion

        #region Реализация интерфейса

        public bool IsMarsel3(int FrontID)
        {
            return FrontID == 3630;
        }

        public bool IsMarsel4(int FrontID)
        {
            return FrontID == 15003;
        }
        public bool IsImpost(int TechnoProfileID)
        {
            return TechnoProfileID == -1;
        }
        public string GetFrontName(int FrontID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }

        public string GetFront2Name(int TechnoProfileID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + TechnoProfileID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }
        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
                ColorName = Rows[0]["ColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetPatinaName(int PatinaID)
        {
            string PatinaName = string.Empty;
            try
            {
                DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
                PatinaName = Rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return PatinaName;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            string InsetType = string.Empty;
            try
            {
                DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
                InsetType = Rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetType;
        }

        public string GetInsetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + ColorID);
                ColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }
        #endregion

        private decimal GetSquare(DataTable DT)
        {
            decimal Square = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    Square += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return Square;
        }

        private int GetCount(DataTable DT)
        {
            int Count = 0;

            foreach (DataRow Row in DT.Rows)
            {
                Count += Convert.ToInt32(Row["Count"]);
            }

            return Count;
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        public DataTable PackageFrontsSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        public DataTable PackageDecorSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        private bool FillFronts()
        {
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                var InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                var bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                var bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    var bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }

                NewRow["Front"] = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetType"] = InsetType;
                NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Square"] = Row["Square"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FillDecor()
        {
            DecorResultDataTable.Clear();

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                DataRow NewRow = DecorResultDataTable.NewRow();

                NewRow["Product"] = GetProductName(Convert.ToInt32(Row["ProductID"])) + " " + GetDecorName(Convert.ToInt32(Row["DecorID"]));

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                {
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                {
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    NewRow["Width"] = Convert.ToInt32(Row["Width"]);

                string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    NewRow["Color"] = Color;
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterFrontsOrders(int MainOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();
            DataTable TempDT = new DataTable();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
            //    ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    DA.Fill(OriginalFrontsOrdersDataTable);
            //}
            using (DataView DV = new DataView(OriginalFrontsOrdersDataTable))
            {
                DV.RowFilter = "MainOrderID = " + MainOrderID + FactoryFilter;
                TempDT = DV.ToTable();
            }

            sw.Stop();
            double G = sw.Elapsed.TotalSeconds;
            FrontsOrdersDataTable = TempDT.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    sw.Restart();
                    DA.Fill(DT);

                    sw.Stop();
                    double G2 = sw.Elapsed.TotalSeconds;
                    sw.Restart();
                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }

                    sw.Stop();
                    double G4 = sw.Elapsed.TotalSeconds;
                    sw.Restart();
                }
            }
            TempDT.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders(int MainOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable TempDT = new DataTable();

            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
            //    ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    DA.Fill(OriginalDecorOrdersDataTable);
            //}
            using (DataView DV = new DataView(OriginalDecorOrdersDataTable))
            {
                DV.RowFilter = "MainOrderID = " + MainOrderID + FactoryFilter;
                TempDT = DV.ToTable();
            }

            DecorOrdersDataTable = TempDT.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            //NewRow["DecorOrderID"] = ORow[0]["DecorOrderID"];
                            //NewRow["ProductID"] = ORow[0]["ProductID"];
                            //NewRow["DecorID"] = ORow[0]["DecorID"];

                            //if (Convert.ToInt32(ORow[0]["ColorID"]) == -1)
                            //    NewRow["ColorID"] = 0;
                            //else
                            NewRow["ColorID"] = ORow[0]["ColorID"];

                            //NewRow["Height"] = ORow[0]["Height"];
                            //NewRow["Length"] = ORow[0]["Length"];
                            //NewRow["Width"] = ORow[0]["Width"];
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            DecorOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            TempDT.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool HasFronts(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        private bool HasDecor(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        public void CreateReport(ref HSSFWorkbook thssfworkbook, int[] MainOrders, int FactoryID)
        {
            GetMainOrdersInfo(MainOrders);
            string SheetName = "Ведомость Профиль+ТПС";

            if (FactoryID == 1)
                SheetName = "Ведомость Профиль";
            if (FactoryID == 2)
                SheetName = "Ведомость ТПС";

            HSSFSheet sheet1 = thssfworkbook.CreateSheet(SheetName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 3;

            HSSFCell Cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Серым цветом отмечены отсутствующие упаковки");

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (HasFronts(MainOrders, FactoryID))
            {
                sw.Stop();
                double G = sw.Elapsed.TotalSeconds;
                sw.Restart();
                CreateFrontsExcel(ref thssfworkbook, MainOrders, ref RowIndex, FactoryID, SheetName);
                sw.Stop();
                double G1 = sw.Elapsed.TotalSeconds;
            }

            sw.Restart();
            if (HasDecor(MainOrders, FactoryID))
            {
                CreateDecorExcel(ref thssfworkbook, MainOrders, RowIndex, FactoryID, SheetName);
            }
            double G2 = sw.Elapsed.TotalSeconds;
        }

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, ref int RowIndex, int FactoryID, string SheetName)
        {
            string DispatchDate = string.Empty;

            string DocNumber = string.Empty;
            string ClientName = string.Empty;

            //int PageCount = 1;
            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;
            bool DoNotDispatch = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            int DisplayIndex = 0;
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 4 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 18 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 7 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 9 * 256);

            #region Create fonts and styles

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 7, "Утверждаю........");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                sw.Restart();
                FilterFrontsOrders(MainOrders[i], FactoryID);
                sw.Stop();
                double G = sw.Elapsed.TotalSeconds;

                sw.Restart();
                IsFronts = FillFronts();
                sw.Stop();
                double G1 = sw.Elapsed.TotalSeconds;

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                sw.Restart();
                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                DispatchDate = GetDispatchDate(MainOrders[i]);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);
                sw.Stop();
                double G2 = sw.Elapsed.TotalSeconds;

                sw.Restart();
                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " " + DocNumber);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;

                //вывод заказов фасадов
                for (int index = 0; index < PackageFrontsSequence.Rows.Count; index++)
                {
                    DataRow[] FRows = FrontsResultDataTable.Select("[PackNumber] = " + PackageFrontsSequence.Rows[index]["PackNumber"]);

                    if (FRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = FRows.Count() + TopIndex - 1;

                    for (int x = 0; x < FRows.Count(); x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                        {
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3 || DoNotDispatch)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }


                        }
                        RowIndex++;
                    }

                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(++RowIndex), 8, Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                    cellStyle.SetFont(PackNumberFont);
                    cell.CellStyle = cellStyle;
                }

                if (IsFronts)
                    RowIndex++;


                RowIndex++;
                sw.Stop();
                double G3 = sw.Elapsed.TotalSeconds;
            }

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int RowIndex, int FactoryID, string SheetName)
        {
            string DispatchDate = string.Empty;

            string DocNumber = string.Empty;
            string ClientName = string.Empty;

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            if (RowIndex < 1)
            {
                HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
                CreateDateCell.CellStyle = TempStyle;
            }

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;
            bool DoNotDispatch = false;

            #region Create fonts and styles

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                sw.Restart();
                FilterDecorOrders(MainOrders[i], FactoryID);
                sw.Stop();
                double G = sw.Elapsed.TotalSeconds;

                sw.Restart();
                IsDecor = FillDecor();
                sw.Stop();
                double G1 = sw.Elapsed.TotalSeconds;

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                sw.Restart();
                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                DispatchDate = GetDispatchDate(MainOrders[i]);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);
                sw.Stop();
                double G2 = sw.Elapsed.TotalSeconds;

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                sw.Restart();
                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " " + DocNumber);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0 || y == 1)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3 || DoNotDispatch)
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }





                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
                sw.Stop();
                double G3 = sw.Elapsed.TotalSeconds;
            }

            for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }
    }






    public class ZOVTrayList : IAllFrontParameterName, IIsMarsel
    {
        private DataTable SimpleResultDataTable = null;
        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        TrayPackages TrayPackages;

        public ZOVTrayList()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();
        }

        private void GetColorsDT()
        {
            FrameColorsDataTable = new DataTable();
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
            }
        }

        private void Create()
        {
            SimpleResultDataTable = new DataTable();
            SimpleResultDataTable.Columns.Add(new DataColumn(("MainOrder"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("FrontsPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("DecorPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("AllPackagesCount"), System.Type.GetType("System.String")));

            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            FrontsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            PatinaDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            GetColorsDT();
            GetInsetColorsDT();
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInset"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Product"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Color"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Height"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Width"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Count"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        #region Реализация интерфейса IReference

        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        public string GetOrderNumber(int MainOrderID)
        {
            string DocNumber = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocNumber FROM MainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        DocNumber = DT.Rows[0]["DocNumber"].ToString();
                }
            }
            return DocNumber;
        }

        public string GetClientName(int MainOrderID)
        {
            int ClientID = 0;
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetRealDispatchDate(int TrayID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDateTime FROM Trays" +
                    " WHERE TrayID = " + TrayID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDateTime"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDateTime"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        public string GetDesireDispatchDate(int TrayID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" +
                    " (SELECT TOP 1 MainOrderID FROM Packages WHERE TrayID = " + TrayID + "))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        private bool IsDoNotDispatch(int MainOrderID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DoNotDispatch  FROM MainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return Convert.ToBoolean(DT.Rows[0]["DoNotDispatch"]);
                }
            }
            return false;
        }
        #endregion

        #region Реализация интерфейса

        public bool IsMarsel3(int FrontID)
        {
            return FrontID == 3630;
        }

        public bool IsMarsel4(int FrontID)
        {
            return FrontID == 15003;
        }
        public bool IsImpost(int TechnoProfileID)
        {
            return TechnoProfileID == -1;
        }
        public string GetFrontName(int FrontID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }

        public string GetFront2Name(int TechnoProfileID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + TechnoProfileID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }
        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
                ColorName = Rows[0]["ColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetPatinaName(int PatinaID)
        {
            string PatinaName = string.Empty;
            try
            {
                DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
                PatinaName = Rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return PatinaName;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            string InsetType = string.Empty;
            try
            {
                DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
                InsetType = Rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetType;
        }

        public string GetInsetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + ColorID);
                ColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }
        #endregion

        private decimal GetSquare(int TrayID)
        {
            decimal Square = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Square) As Square FROM FrontsOrders WHERE FrontsOrdersID IN" +
                    " (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE TrayID = " + TrayID + "))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Square"] != DBNull.Value)
                    {
                        Square = Convert.ToDecimal(DT.Rows[0]["Square"]);
                        Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }

            return Square;
        }

        private decimal GetWeight(int TrayID)
        {
            decimal Weight = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM FrontsOrders WHERE FrontsOrdersID IN" +
                    " (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND TrayID = " + TrayID + "))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        Weight = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM DecorOrders WHERE DecorOrderID IN" +
                    " (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND TrayID = " + TrayID + "))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        decimal Temp = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                        Weight += Temp;
                    }
                }

                Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);
            }

            return Weight;
        }

        private decimal GetSquare(DataTable DT)
        {
            decimal Square = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    Square += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return Square;
        }

        private int GetCount(DataTable DT)
        {
            int Count = 0;

            foreach (DataRow Row in DT.Rows)
            {
                Count += Convert.ToInt32(Row["Count"]);
            }

            return Count;
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        public DataTable PackageFrontsSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        public DataTable PackageDecorSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        private int[] GetMainOrders(int TrayID)
        {
            int[] rows = { 0 };

            string SelectionCommand = "SELECT DISTINCT MainOrderID FROM Packages" +
                " WHERE TrayID=" + TrayID;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                }

                rows = new int[DT.Rows.Count];

                for (int i = 0; i < DT.Rows.Count; i++)
                    rows[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            }

            return rows;
        }

        private int GetTrayPackagesCount(int MainOrderID, int TrayID, int ProductType)
        {
            int PackagesCount;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                    " AND TrayID = " + TrayID + " AND ProductType = " + ProductType,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    if (DA.Fill(DT) < 1)
                        return 0;

                    PackagesCount = DT.Rows.Count;
                }
            }
            return PackagesCount;
        }

        private int GetPackagesCount(int MainOrderID, int ProductType)
        {
            int PackagesCount;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                    " AND ProductType = " + ProductType,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    if (DA.Fill(DT) < 1)
                        return 0;

                    PackagesCount = DT.Rows.Count;
                }
            }

            return PackagesCount;
        }

        private bool Fill(int[] MainOrders, int TrayID)
        {
            SimpleResultDataTable.Clear();

            TrayPackages.AllPackages = 0;
            TrayPackages.AllTrayPackages = 0;

            int MainOrderID = 0;
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                int FrontsTrayPackagesCount = 0;
                int DecorTrayPackagesCount = 0;
                int AllTrayPackagesCount = 0;

                int FrontsPackagesCount = 0;
                int DecorPackagesCount = 0;
                int AllPackagesCount = 0;

                string ClientName = string.Empty;
                string DocNumber = string.Empty;
                MainOrderID = MainOrders[i];

                FrontsTrayPackagesCount = GetTrayPackagesCount(MainOrders[i], TrayID, 0);
                DecorTrayPackagesCount = GetTrayPackagesCount(MainOrders[i], TrayID, 1);
                AllTrayPackagesCount = FrontsTrayPackagesCount + DecorTrayPackagesCount;

                TrayPackages.AllTrayPackages += AllTrayPackagesCount;

                FrontsPackagesCount = GetPackagesCount(MainOrders[i], 0);
                DecorPackagesCount = GetPackagesCount(MainOrders[i], 1);
                AllPackagesCount = FrontsPackagesCount + DecorPackagesCount;

                TrayPackages.AllPackages += AllPackagesCount;

                ClientName = GetClientName(MainOrders[i]);
                DocNumber = GetOrderNumber(MainOrders[i]);

                DataRow NewRow = SimpleResultDataTable.NewRow();

                NewRow["MainOrder"] = ClientName + " / " + DocNumber;
                if (FrontsPackagesCount > 0)
                    NewRow["FrontsPackagesCount"] = FrontsTrayPackagesCount.ToString() + " / " + FrontsPackagesCount.ToString();
                if (DecorPackagesCount > 0)
                    NewRow["DecorPackagesCount"] = DecorTrayPackagesCount.ToString() + " / " + DecorPackagesCount.ToString();

                NewRow["AllPackagesCount"] = AllTrayPackagesCount.ToString() + " / " + AllPackagesCount.ToString();

                SimpleResultDataTable.Rows.Add(NewRow);
            }

            DataView DV = new DataView(SimpleResultDataTable)
            {
                Sort = "MainOrder"
            };
            SimpleResultDataTable = DV.ToTable();
            DV.Dispose();

            return SimpleResultDataTable.Rows.Count > 0;
        }

        private bool FillFronts()
        {
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                var InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                var bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                var bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    var bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }

                NewRow["Front"] = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetType"] = InsetType;
                NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Square"] = Row["Square"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FillDecor()
        {
            DecorResultDataTable.Clear();

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                DataRow NewRow = DecorResultDataTable.NewRow();

                NewRow["Product"] = GetProductName(Convert.ToInt32(Row["ProductID"])) + " " + GetDecorName(Convert.ToInt32(Row["DecorID"]));

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                {
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                {
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    NewRow["Width"] = Convert.ToInt32(Row["Width"]);

                string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    NewRow["Color"] = Color;
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterFrontsOrders(int MainOrderID, int TrayID)
        {
            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE TrayID = " + TrayID + " AND MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders(int MainOrderID, int TrayID)
        {
            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();


            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE TrayID = " + TrayID + " AND MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            //NewRow["DecorOrderID"] = ORow[0]["DecorOrderID"];
                            //NewRow["ProductID"] = ORow[0]["ProductID"];
                            //NewRow["DecorID"] = ORow[0]["DecorID"];

                            //if (Convert.ToInt32(ORow[0]["ColorID"]) == -1)
                            //    NewRow["ColorID"] = 0;
                            //else
                            NewRow["ColorID"] = ORow[0]["ColorID"];

                            //NewRow["Height"] = ORow[0]["Height"];
                            //NewRow["Length"] = ORow[0]["Length"];
                            //NewRow["Width"] = ORow[0]["Width"];
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            DecorOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool HasFronts(int[] MainOrderIDs, int TrayID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND TrayID = " + TrayID + " AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        private bool HasDecor(int[] MainOrderIDs, int TrayID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND TrayID = " + TrayID + " AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        public void CreateReport(int TrayID)
        {
            int[] MainOrders = GetMainOrders(TrayID);
            if (MainOrders.Count() == 0)
                return;
            string DocumentName = GetRealDispatchDate(TrayID) + " Поддон №" + TrayID;
            string SheetName = "Ведомость";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            CreateExcel(ref hssfworkbook, MainOrders, TrayID);

            HSSFSheet sheet1 = hssfworkbook.CreateSheet(SheetName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 3;

            //HSSFCell Cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Серым цветом отмечены отсутствующие упаковки");

            if (HasFronts(MainOrders, TrayID))
            {
                CreateFrontsExcel(ref hssfworkbook, MainOrders, TrayID, ref RowIndex, SheetName);
            }

            if (HasDecor(MainOrders, TrayID))
            {
                CreateDecorExcel(ref hssfworkbook, MainOrders, TrayID, RowIndex, SheetName);
            }

            //string FileName = GetFileName(DocumentName, "ZOVDispatchReportPath.config");

            //FileInfo file = new FileInfo(FileName);
            //if (file.Exists)
            //{
            //    try
            //    {
            //        file.Delete();
            //    }
            //    catch (System.IO.IOException e)
            //    {
            //        MessageBox.Show(e.Message, "Ошибка сохранения");
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message, "Ошибка сохранения");
            //    }
            //}

            string FileName = DocumentName;

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        private void CreateExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int TrayID)
        {
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Общее");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.FontHeightInPoints = 13;
            HeaderFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont.FontName = "Calibri";

            HSSFFont NotesFont = hssfworkbook.CreateFont();
            NotesFont.FontHeightInPoints = 12;
            NotesFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            NotesFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 12;
            SimpleFont.FontName = "Calibri";

            HSSFFont TempFont = hssfworkbook.CreateFont();
            TempFont.FontHeightInPoints = 12;
            TempFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle EmptyCellStyle = hssfworkbook.CreateCellStyle();
            EmptyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.RightBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderStyle.WrapText = true;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainOrderCellStyle = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle MainOrderCellStyle1 = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle NotesCellStyle = hssfworkbook.CreateCellStyle();
            NotesCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.SetFont(NotesFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle SimpleCellStyle1 = hssfworkbook.CreateCellStyle();
            SimpleCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TempFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.BottomBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 2, "Утверждаю........");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            HSSFCell DispatchDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(1), 0, "Дата отгрузки заказа: " + GetDesireDispatchDate(TrayID));
            DispatchDateCell.CellStyle = TempStyle;

            HSSFCell RealDispatchDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Фактическая дата отгрузки: " + GetRealDispatchDate(TrayID));
            RealDispatchDateCell.CellStyle = TempStyle;

            int RowIndex = 4;
            int TopRowFront = 1;

            decimal Weight = 0;
            decimal TotalFrontsSquare = 0;

            bool DoNotDispatch = false;
            string Notes = string.Empty;

            sheet1.SetColumnWidth(0, 55 * 256);
            sheet1.SetColumnWidth(1, 12 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);
            sheet1.SetColumnWidth(3, 12 * 256);

            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Клиент/Заказ");
            cell1.CellStyle = HeaderStyle;
            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Кол-во упаковок, фасады");
            cell2.CellStyle = HeaderStyle;
            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Кол-во упаковок, декор");
            cell3.CellStyle = HeaderStyle;
            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Кол-во упаковок, общее");
            cell4.CellStyle = HeaderStyle;

            TopRowFront = RowIndex;

            Fill(MainOrders, TrayID);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrders[i]));
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);
                for (int y = 0; y < SimpleResultDataTable.Columns.Count; y++)
                {
                    if (Notes.Length > 0 || DoNotDispatch)
                    {
                        if (SimpleResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle1;//нижняя линия не рисуется
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle1;
                        }


                        if (Notes.Length > 0)
                        {
                            HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                            EmptyCell.CellStyle = EmptyCellStyle;

                            HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                            cell.SetCellValue("Примечание: " + Notes);
                            cell.CellStyle = NotesCellStyle;
                        }
                    }

                    else
                    {
                        if (SimpleResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle;
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle;
                        }
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                        EmptyCell.CellStyle = EmptyCellStyle;

                        HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                        cell.SetCellValue("Примечание: " + Notes);
                        cell.CellStyle = NotesCellStyle;
                    }
                }

                if (Notes.Length > 0)
                {
                    RowIndex++;
                }


                RowIndex++;
            }

            RowIndex++;


            RowIndex++;

            Weight = GetWeight(TrayID);
            TotalFrontsSquare = GetSquare(TrayID);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Итого:");
            cell13.CellStyle = TotalStyle;

            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Площадь: " + TotalFrontsSquare + " м.кв.");
            cell14.CellStyle = TempStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Вес: " + Weight + " кг");
            cell15.CellStyle = TempStyle;

            HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Упаковок: " + TrayPackages.AllTrayPackages + "/" + TrayPackages.AllPackages);
            cell16.CellStyle = TempStyle;
        }

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int TrayID, ref int RowIndex, string SheetName)
        {
            string DocNumber = string.Empty;
            string ClientName = string.Empty;

            //int PageCount = 1;
            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;
            bool DoNotDispatch = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            int DisplayIndex = 0;
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 4 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 18 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 7 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 9 * 256);

            #region Create fonts and styles

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterFrontsOrders(MainOrders[i], TrayID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " " + DocNumber);
                ClientCell.CellStyle = MainStyle;

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;

                //вывод заказов фасадов
                for (int index = 0; index < PackageFrontsSequence.Rows.Count; index++)
                {
                    DataRow[] FRows = FrontsResultDataTable.Select("[PackNumber] = " + PackageFrontsSequence.Rows[index]["PackNumber"]);

                    if (FRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = FRows.Count() + TopIndex - 1;

                    for (int x = 0; x < FRows.Count(); x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                        {
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3 || DoNotDispatch)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }


                        }
                        RowIndex++;
                    }

                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(++RowIndex), 8,
                        Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                    cellStyle.SetFont(PackNumberFont);
                    cell.CellStyle = cellStyle;
                }

                if (IsFronts)
                    RowIndex++;


                RowIndex++;
            }

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int TrayID, int RowIndex, string SheetName)
        {
            string DispatchDate = string.Empty;

            string DocNumber = string.Empty;
            string ClientName = string.Empty;

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;
            bool DoNotDispatch = false;

            #region Create fonts and styles

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterDecorOrders(MainOrders[i], TrayID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                DispatchDate = GetRealDispatchDate(MainOrders[i]);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " " + DocNumber);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0 || y == 1)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3 || DoNotDispatch)
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }





                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
            }

            for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }

        //private string GetFileName(string DispatchDate, string FilePath)
        //{
        //    string ReportFilePath = ReadReportFilePath(FilePath);

        //    string FileName = string.Empty;

        //    if (!(Directory.Exists(ReportFilePath)))
        //    {
        //        Directory.CreateDirectory(ReportFilePath);
        //    }

        //    FileName = DispatchDate + ".xls";
        //    FileName = Path.Combine(ReportFilePath, FileName);

        //    return FileName;
        //}

        //private string ReadReportFilePath(string FileName)
        //{
        //    string ReportFilePath = string.Empty;
        //    using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName, Encoding.Default))
        //    {
        //        ReportFilePath = sr.ReadToEnd();
        //    }
        //    return ReportFilePath;
        //}
    }








    public class MarketingTrayList : IAllFrontParameterName, IIsMarsel
    {
        private DataTable SimpleResultDataTable = null;
        private DataTable TraySummaryDataTable = null;
        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        TrayPackages TrayPackages;

        public MarketingTrayList()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }

            }

        }

        private void GetColorsDT()
        {
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();

            SimpleResultDataTable = new DataTable();
            SimpleResultDataTable.Columns.Add(new DataColumn(("MainOrder"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("FrontsPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("DecorPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDataTable.Columns.Add(new DataColumn(("AllPackagesCount"), System.Type.GetType("System.String")));

            TraySummaryDataTable = new DataTable();
            TraySummaryDataTable.Columns.Add(new DataColumn(("TrayNumber"), System.Type.GetType("System.Int32")));
            TraySummaryDataTable.Columns.Add(new DataColumn(("ClientName"), System.Type.GetType("System.String")));
            TraySummaryDataTable.Columns.Add(new DataColumn(("PackagesCount"), System.Type.GetType("System.Int32")));

            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            GetColorsDT();
            GetInsetColorsDT();
            SelectCommand = @"SELECT * FROM Patina";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            SelectCommand = @"SELECT * FROM InsetTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInset"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Product"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Color"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Height"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Width"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Count"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        #region Реализация интерфейса IReference
        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        public string GetOrderNumber(int MainOrderID)
        {
            string OrderNumber = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT OrderNumber FROM MegaOrders" +
                    " WHERE MegaOrderID = (SELECT TOP 1 MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        OrderNumber = DT.Rows[0]["OrderNumber"].ToString();
                }
            }
            return OrderNumber;
        }

        public string GetClientName(int MainOrderID)
        {
            int ClientID = 0;
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MegaOrders" +
                    " WHERE MegaOrderID = (SELECT TOP 1 MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetClientNameByTray(int TrayID)
        {
            int ClientID = 0;
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MegaOrders" +
                    " WHERE MegaOrderID = (SELECT TOP 1 MegaOrderID FROM MainOrders WHERE MainOrderID = " +
                    " (SELECT TOP 1 MainOrderID FROM Packages WHERE TrayID = " + TrayID + "))", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetRealDispatchDate(int TrayID)
        {
            string DispatchDate = string.Empty;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDateTime FROM Trays" +
                    " WHERE TrayID = " + TrayID,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDateTime"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDateTime"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        public string GetDesireDispatchDate(int TrayID)
        {
            string DispatchDate = string.Empty;
            string Date = "ProfilDispatchDate";

            //if (FactoryID == 2)
            //    Date = "TPSDispatchDate";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT " + Date + " FROM MegaOrders" +
                    " WHERE MegaOrderID=(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" +
                    " (SELECT TOP 1 MainOrderID FROM Packages WHERE TrayID = " + TrayID + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }
        #endregion

        #region Реализация интерфейса

        public bool IsMarsel3(int FrontID)
        {
            return FrontID == 3630;
        }

        public bool IsMarsel4(int FrontID)
        {
            return FrontID == 15003;
        }
        public bool IsImpost(int TechnoProfileID)
        {
            return TechnoProfileID == -1;
        }
        public string GetFrontName(int FrontID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }

        public string GetFront2Name(int TechnoProfileID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + TechnoProfileID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }
        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
                ColorName = Rows[0]["ColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetPatinaName(int PatinaID)
        {
            string PatinaName = string.Empty;
            try
            {
                DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
                PatinaName = Rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return PatinaName;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            string InsetType = string.Empty;
            try
            {
                DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
                InsetType = Rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetType;
        }

        public string GetInsetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + ColorID);
                ColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }
        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }
        #endregion

        private decimal GetSquare(int TrayID)
        {
            decimal Square = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Square) As Square FROM FrontsOrders WHERE FrontsOrdersID IN" +
                    " (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE TrayID = " + TrayID + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Square"] != DBNull.Value)
                    {
                        Square = Convert.ToDecimal(DT.Rows[0]["Square"]);
                        Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }

            return Square;
        }

        private decimal GetWeight(int TrayID)
        {
            decimal Weight = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM FrontsOrders WHERE FrontsOrdersID IN" +
                    " (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND TrayID = " + TrayID + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        Weight = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight FROM DecorOrders WHERE DecorOrderID IN" +
                    " (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND TrayID = " + TrayID + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["Weight"] != DBNull.Value)
                    {
                        decimal Temp = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                        Weight += Temp;
                    }
                }

                Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);
            }

            return Weight;
        }

        private decimal GetSquare(DataTable DT)
        {
            decimal Square = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    Square += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return Square;
        }

        private int GetCount(DataTable DT)
        {
            int Count = 0;

            foreach (DataRow Row in DT.Rows)
            {
                Count += Convert.ToInt32(Row["Count"]);
            }

            return Count;
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        public DataTable PackageFrontsSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        public DataTable PackageDecorSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        private int[] GetMainOrders(int TrayID)
        {
            int[] rows = { 0 };

            string SelectionCommand = "SELECT DISTINCT MainOrderID FROM Packages" +
                " WHERE TrayID=" + TrayID;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                }

                rows = new int[DT.Rows.Count];

                for (int i = 0; i < DT.Rows.Count; i++)
                    rows[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            }

            return rows;
        }

        private int GetTrayPackagesCount(int TrayID)
        {
            int PackagesCount = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages" +
                    " WHERE TrayID = " + TrayID,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) < 1)
                        return 0;

                    PackagesCount = DT.Rows.Count;
                }
            }
            return PackagesCount;
        }

        private int GetTrayPackagesCount(int MainOrderID, int TrayID, int ProductType)
        {
            int PackagesCount;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                    " AND TrayID = " + TrayID + " AND ProductType = " + ProductType,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) < 1)
                        return 0;

                    PackagesCount = DT.Rows.Count;
                }
            }
            return PackagesCount;
        }

        private int GetPackagesCount(int MainOrderID, int ProductType)
        {
            int PackagesCount;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                    " AND ProductType = " + ProductType,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) < 1)
                        return 0;

                    PackagesCount = DT.Rows.Count;
                }
            }

            return PackagesCount;
        }

        private bool FillSummaryDT(int[] Trays)
        {
            TraySummaryDataTable.Clear();

            int TrayID = 0;
            int PackagesCount = 0;
            string ClientName = string.Empty;

            for (int i = 0; i < Trays.Count(); i++)
            {
                TrayID = Trays[i];
                ClientName = GetClientNameByTray(Trays[i]);
                PackagesCount = GetTrayPackagesCount(Trays[i]);

                DataRow NewRow = TraySummaryDataTable.NewRow();
                NewRow["TrayNumber"] = Trays[i];
                NewRow["ClientName"] = ClientName;
                if (PackagesCount > 0)
                    NewRow["PackagesCount"] = PackagesCount;
                TraySummaryDataTable.Rows.Add(NewRow);
            }

            DataView DV = new DataView(TraySummaryDataTable)
            {
                Sort = "TrayNumber"
            };
            TraySummaryDataTable = DV.ToTable();
            DV.Dispose();

            return TraySummaryDataTable.Rows.Count > 0;
        }

        private bool Fill(int[] MainOrders, int TrayID)
        {
            SimpleResultDataTable.Clear();

            TrayPackages.AllPackages = 0;
            TrayPackages.AllTrayPackages = 0;

            int MainOrderID = 0;
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                int FrontsTrayPackagesCount = 0;
                int DecorTrayPackagesCount = 0;
                int AllTrayPackagesCount = 0;

                int FrontsPackagesCount = 0;
                int DecorPackagesCount = 0;
                int AllPackagesCount = 0;

                string ClientName = string.Empty;
                string DocNumber = string.Empty;
                MainOrderID = MainOrders[i];

                FrontsTrayPackagesCount = GetTrayPackagesCount(MainOrders[i], TrayID, 0);
                DecorTrayPackagesCount = GetTrayPackagesCount(MainOrders[i], TrayID, 1);
                AllTrayPackagesCount = FrontsTrayPackagesCount + DecorTrayPackagesCount;

                TrayPackages.AllTrayPackages += AllTrayPackagesCount;

                FrontsPackagesCount = GetPackagesCount(MainOrders[i], 0);
                DecorPackagesCount = GetPackagesCount(MainOrders[i], 1);
                AllPackagesCount = FrontsPackagesCount + DecorPackagesCount;

                TrayPackages.AllPackages += AllPackagesCount;

                ClientName = GetClientName(MainOrders[i]);
                DocNumber = GetOrderNumber(MainOrders[i]);

                DataRow NewRow = SimpleResultDataTable.NewRow();

                NewRow["MainOrder"] = ClientName + " / " + DocNumber;
                if (FrontsPackagesCount > 0)
                    NewRow["FrontsPackagesCount"] = FrontsTrayPackagesCount.ToString() + " / " + FrontsPackagesCount.ToString();
                if (DecorPackagesCount > 0)
                    NewRow["DecorPackagesCount"] = DecorTrayPackagesCount.ToString() + " / " + DecorPackagesCount.ToString();

                NewRow["AllPackagesCount"] = AllTrayPackagesCount.ToString() + " / " + AllPackagesCount.ToString();

                SimpleResultDataTable.Rows.Add(NewRow);
            }

            DataView DV = new DataView(SimpleResultDataTable)
            {
                Sort = "MainOrder"
            };
            SimpleResultDataTable = DV.ToTable();
            DV.Dispose();

            return SimpleResultDataTable.Rows.Count > 0;
        }

        private bool FillFronts()
        {
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                var InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                var bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                var bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    var bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }
                NewRow["Front"] = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetType"] = InsetType;
                NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Square"] = Row["Square"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FillDecor()
        {
            DecorResultDataTable.Clear();

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                DataRow NewRow = DecorResultDataTable.NewRow();

                NewRow["Product"] = GetProductName(Convert.ToInt32(Row["ProductID"])) + " " + GetDecorName(Convert.ToInt32(Row["DecorID"]));

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                {
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                {
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                }
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    NewRow["Width"] = Convert.ToInt32(Row["Width"]);

                string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    NewRow["Color"] = Color;
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterFrontsOrders(int MainOrderID, int TrayID)
        {
            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE TrayID = " + TrayID + " AND MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders(int MainOrderID, int TrayID)
        {
            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();


            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE TrayID = " + TrayID + " AND MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            //NewRow["DecorOrderID"] = ORow[0]["DecorOrderID"];
                            //NewRow["ProductID"] = ORow[0]["ProductID"];
                            //NewRow["DecorID"] = ORow[0]["DecorID"];

                            //if (Convert.ToInt32(ORow[0]["ColorID"]) == -1)
                            //    NewRow["ColorID"] = 0;
                            //else
                            NewRow["ColorID"] = ORow[0]["ColorID"];

                            //NewRow["Height"] = ORow[0]["Height"];
                            //NewRow["Length"] = ORow[0]["Length"];
                            //NewRow["Width"] = ORow[0]["Width"];
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            DecorOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool HasFronts(int[] MainOrderIDs, int TrayID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND TrayID = " + TrayID + " AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        private bool HasDecor(int[] MainOrderIDs, int TrayID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            //if (FactoryID != 0)
            //    FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND TrayID = " + TrayID + " AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        public void CreateReport(int TrayID)
        {
            int[] MainOrders = GetMainOrders(TrayID);
            if (MainOrders.Count() == 0)
                return;
            string DocumentName = GetRealDispatchDate(TrayID) + " Поддон №" + TrayID;
            string SheetName = "Ведомость";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            CreateExcel(ref hssfworkbook, MainOrders, TrayID);

            HSSFSheet sheet1 = hssfworkbook.CreateSheet(SheetName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 3;

            //HSSFCell Cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Серым цветом отмечены отсутствующие упаковки");

            if (HasFronts(MainOrders, TrayID))
            {
                CreateFrontsExcel(ref hssfworkbook, MainOrders, TrayID, ref RowIndex, SheetName);
            }

            if (HasDecor(MainOrders, TrayID))
            {
                CreateDecorExcel(ref hssfworkbook, MainOrders, TrayID, RowIndex, SheetName);
            }

            //string FileName = GetFileName(DocumentName, "MarketingDispatchReportPath.config");

            //FileInfo file = new FileInfo(FileName);
            //if (file.Exists)
            //{
            //    try
            //    {
            //        file.Delete();
            //    }
            //    catch (System.IO.IOException e)
            //    {
            //        MessageBox.Show(e.Message, "Ошибка сохранения");
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message, "Ошибка сохранения");
            //    }
            //}

            string FileName = DocumentName;

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void SummaryTrayReport(int[] Trays)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Сводка по поддонам");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.FontHeightInPoints = 13;
            HeaderFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont.FontName = "Calibri";

            HSSFFont NotesFont = hssfworkbook.CreateFont();
            NotesFont.FontHeightInPoints = 12;
            NotesFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            NotesFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 12;
            SimpleFont.FontName = "Calibri";

            HSSFFont SimpleBoldFont = hssfworkbook.CreateFont();
            SimpleBoldFont.FontHeightInPoints = 12;
            SimpleBoldFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            SimpleBoldFont.FontName = "Calibri";

            HSSFFont TempFont = hssfworkbook.CreateFont();
            TempFont.FontHeightInPoints = 12;
            TempFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle EmptyCellStyle = hssfworkbook.CreateCellStyle();
            EmptyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.RightBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            //HeaderStyle.WrapText = true;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainOrderCellStyle = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle MainOrderCellStyle1 = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle NotesCellStyle = hssfworkbook.CreateCellStyle();
            NotesCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.SetFont(NotesFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle SimpleCellStyle1 = hssfworkbook.CreateCellStyle();
            SimpleCellStyle1.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle SimpleBoldCellStyle1 = hssfworkbook.CreateCellStyle();
            SimpleBoldCellStyle1.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleBoldCellStyle1.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleBoldCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleBoldCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleBoldCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleBoldCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleBoldCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleBoldCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleBoldCellStyle1.SetFont(SimpleBoldFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TempFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.BottomBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            int RowIndex = 1;
            int TopRowFront = 1;

            sheet1.SetColumnWidth(0, 12 * 256);
            sheet1.SetColumnWidth(1, 35 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);

            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(0);
            cell.SetCellValue("№ под.");
            cell.CellStyle = HeaderStyle;
            cell = sheet1.CreateRow(RowIndex).CreateCell(1);
            cell.SetCellValue("Клиент");
            cell.CellStyle = HeaderStyle;
            cell = sheet1.CreateRow(RowIndex).CreateCell(2);
            cell.SetCellValue("Кол-во");
            cell.CellStyle = HeaderStyle;

            TopRowFront = RowIndex;

            FillSummaryDT(Trays);

            for (int i = 0; i < Trays.Count(); i++)
            {
                for (int j = 0; j < TraySummaryDataTable.Columns.Count; j++)
                {
                    if (TraySummaryDataTable.Rows[i][j] == DBNull.Value)
                        continue;

                    if (TraySummaryDataTable.Columns[j].ColumnName == "PackagesCount")
                    {
                        cell = sheet1.CreateRow(RowIndex + 1).CreateCell(j);
                        cell.SetCellValue(Convert.ToInt32(TraySummaryDataTable.Rows[i][j]));
                        cell.CellStyle = SimpleBoldCellStyle1;
                        continue;
                    }

                    Type t = TraySummaryDataTable.Rows[i][j].GetType();
                    if (t.Name == "Decimal")
                    {
                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                        cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                        cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                        cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                        cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                        cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                        cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                        cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                        cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                        cellStyle.SetFont(SimpleFont);
                        cell.CellStyle = cellStyle;

                        cell = sheet1.CreateRow(RowIndex + 1).CreateCell(j);
                        cell.SetCellValue(Convert.ToDouble(TraySummaryDataTable.Rows[i][j]));
                        cell.CellStyle = cellStyle;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex + 1).CreateCell(j);
                        cell.SetCellValue(Convert.ToInt32(TraySummaryDataTable.Rows[i][j]));
                        cell.CellStyle = SimpleCellStyle1;
                        continue;
                    }
                    if (t.Name == "String")
                    {
                        cell = sheet1.CreateRow(RowIndex + 1).CreateCell(j);
                        cell.SetCellValue(TraySummaryDataTable.Rows[i][j].ToString());
                        cell.CellStyle = SimpleCellStyle1;
                        continue;
                    }
                }
                RowIndex++;
            }

            RowIndex++;

            //string FileName = string.Empty;
            //string ReportFilePath = ReadReportFilePath("MarketingDispatchReportPath.config");

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            //FileName = "Сводка по поддонам.xls";
            //FileName = Path.Combine(ReportFilePath, FileName);

            //int DocNumber = 1;

            //while (File.Exists(FileName))
            //{
            //    FileName = "Сводка по поддонам (" + DocNumber++ + ").xls";
            //    FileName = Path.Combine(ReportFilePath, FileName);
            //}

            //FileInfo file = new FileInfo(FileName);

            string FileName = "Сводка по поддонам";

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int DocNumber = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + DocNumber++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        private void CreateExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int TrayID)
        {
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Общее");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.FontHeightInPoints = 13;
            HeaderFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont.FontName = "Calibri";

            HSSFFont NotesFont = hssfworkbook.CreateFont();
            NotesFont.FontHeightInPoints = 12;
            NotesFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            NotesFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 12;
            SimpleFont.FontName = "Calibri";

            HSSFFont TempFont = hssfworkbook.CreateFont();
            TempFont.FontHeightInPoints = 12;
            TempFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle EmptyCellStyle = hssfworkbook.CreateCellStyle();
            EmptyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.RightBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderStyle.WrapText = true;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainOrderCellStyle = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle MainOrderCellStyle1 = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle NotesCellStyle = hssfworkbook.CreateCellStyle();
            NotesCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.SetFont(NotesFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle SimpleCellStyle1 = hssfworkbook.CreateCellStyle();
            SimpleCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TempFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.BottomBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 2, "Утверждаю........");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            //HSSFCell DispatchDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(1), 0, "Дата отгрузки заказа: " + GetDesireDispatchDate(TrayID));
            //DispatchDateCell.CellStyle = TempStyle;

            HSSFCell RealDispatchDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Фактическая дата отгрузки: " + GetRealDispatchDate(TrayID));
            RealDispatchDateCell.CellStyle = TempStyle;

            int RowIndex = 4;
            int TopRowFront = 1;

            decimal Weight = 0;
            decimal TotalFrontsSquare = 0;

            string Notes = string.Empty;

            sheet1.SetColumnWidth(0, 55 * 256);
            sheet1.SetColumnWidth(1, 12 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);
            sheet1.SetColumnWidth(3, 12 * 256);

            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Клиент/Заказ");
            cell1.CellStyle = HeaderStyle;
            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Кол-во упаковок, фасады");
            cell2.CellStyle = HeaderStyle;
            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Кол-во упаковок, декор");
            cell3.CellStyle = HeaderStyle;
            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Кол-во упаковок, общее");
            cell4.CellStyle = HeaderStyle;

            TopRowFront = RowIndex;

            Fill(MainOrders, TrayID);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrders[i]));
                for (int y = 0; y < SimpleResultDataTable.Columns.Count; y++)
                {
                    if (Notes.Length > 0)
                    {
                        if (SimpleResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle1;//нижняя линия не рисуется
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle1;
                        }


                        if (Notes.Length > 0)
                        {
                            HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                            EmptyCell.CellStyle = EmptyCellStyle;

                            HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                            cell.SetCellValue("Примечание: " + Notes);
                            cell.CellStyle = NotesCellStyle;
                        }
                    }

                    else
                    {
                        if (SimpleResultDataTable.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle;
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDataTable.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle;
                        }
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                        EmptyCell.CellStyle = EmptyCellStyle;

                        HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                        cell.SetCellValue("Примечание: " + Notes);
                        cell.CellStyle = NotesCellStyle;
                    }
                }

                if (Notes.Length > 0)
                {
                    RowIndex++;
                }


                RowIndex++;
            }

            RowIndex++;


            RowIndex++;

            Weight = GetWeight(TrayID);
            TotalFrontsSquare = GetSquare(TrayID);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Итого:");
            cell13.CellStyle = TotalStyle;

            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Площадь: " + TotalFrontsSquare + " м.кв.");
            cell14.CellStyle = TempStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Вес: " + Weight + " кг");
            cell15.CellStyle = TempStyle;

            HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Упаковок: " + TrayPackages.AllTrayPackages + "/" + TrayPackages.AllPackages);
            cell16.CellStyle = TempStyle;
        }

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int TrayID, ref int RowIndex, string SheetName)
        {
            string DocNumber = string.Empty;
            string ClientName = string.Empty;

            //int PageCount = 1;
            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            int DisplayIndex = 0;
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 4 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 18 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 7 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 9 * 256);

            #region Create fonts and styles

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterFrontsOrders(MainOrders[i], TrayID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " +
                    ClientName + " № " + DocNumber + " - " + MainOrders[i]);
                ClientCell.CellStyle = MainStyle;

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;

                //вывод заказов фасадов
                for (int index = 0; index < PackageFrontsSequence.Rows.Count; index++)
                {
                    DataRow[] FRows = FrontsResultDataTable.Select("[PackNumber] = " + PackageFrontsSequence.Rows[index]["PackNumber"]);

                    if (FRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = FRows.Count() + TopIndex - 1;

                    for (int x = 0; x < FRows.Count(); x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                        {
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }


                        }
                        RowIndex++;
                    }

                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(++RowIndex), 8,
                        Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                    cellStyle.SetFont(PackNumberFont);
                    cell.CellStyle = cellStyle;
                }

                if (IsFronts)
                    RowIndex++;


                RowIndex++;
            }

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int TrayID, int RowIndex, string SheetName)
        {
            string DocNumber = string.Empty;
            string ClientName = string.Empty;

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;

            #region Create fonts and styles

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;

            PackNumberFont.FontName = "Calibri";
            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterDecorOrders(MainOrders[i], TrayID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " +
                    ClientName + " № " + DocNumber + " - " + MainOrders[i]);
                ClientCell.CellStyle = MainStyle;

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0 || y == 1)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3)
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                    cellStyle.SetFont(SimpleFont);
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }





                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
            }

            for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }

        //private string GetFileName(string DispatchDate, string FilePath)
        //{
        //    string ReportFilePath = ReadReportFilePath(FilePath);

        //    string FileName = string.Empty;

        //    if (!Directory.Exists(ReportFilePath))
        //    {
        //        Directory.CreateDirectory(ReportFilePath);
        //    }

        //    FileName = DispatchDate + ".xls";
        //    FileName = Path.Combine(ReportFilePath, FileName);

        //    return FileName;
        //}

        //private string ReadReportFilePath(string FileName)
        //{
        //    string ReportFilePath = string.Empty;
        //    using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName, Encoding.Default))
        //    {
        //        ReportFilePath = sr.ReadToEnd();
        //    }
        //    return ReportFilePath;
        //}
    }







    public class ZOVDispatchCheckLabel
    {
        int iUserID = 0;
        ArrayList DispatchIDs;
        ArrayList TrayIDs;

        int CurrentProductType = 0;
        public int CurrentMainOrderID = 0;
        public int CurrentMegaOrderID = 0;

        PercentageDataGrid FrontsPackContentDataGrid = null;
        PercentageDataGrid DecorPackContentDataGrid = null;
        PercentageDataGrid PackagesDataGrid = null;
        PercentageDataGrid dgvScanPackages = null;
        PercentageDataGrid dgvNotScanPackages = null;
        PercentageDataGrid dgvWrongPackages = null;

        DataTable FrontsPackContentDataTable = null;
        DataTable DecorPackContentDataTable = null;
        DataTable PackagesDataTable = null;
        DataTable ScanedPackagesDT = null;
        DataTable NotScanedPackagesDT = null;
        DataTable WrongPackagesDT = null;

        DataTable FrontsDataTable = null;
        DataTable PatinaDataTable = null;
        DataTable InsetTypesDataTable = null;
        DataTable FrameColorsDataTable = null;
        DataTable InsetColorsDataTable = null;
        DataTable TechnoInsetTypesDataTable = null;
        DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        DataTable ZOVClientsDataTable;
        DataTable PackageStatusesDataTable;

        DataTable DecorDataTable;
        DataTable DecorProductsDataTable;

        DataTable OrderStatusInfoDT;

        DataGridViewComboBoxColumn FrontsColumn;
        DataGridViewComboBoxColumn PatinaColumn = null;
        DataGridViewComboBoxColumn InsetTypesColumn;
        DataGridViewComboBoxColumn FrameColorsColumn;
        DataGridViewComboBoxColumn InsetColorsColumn;
        DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public BindingSource FrontsPackContentBindingSource = null;
        public BindingSource DecorPackContentBindingSource = null;
        public BindingSource PackagesBindingSource = null;
        public BindingSource ScanedPackagesBS = null;
        public BindingSource NotScanedPackagesBS = null;
        public BindingSource WrongPackagesBS = null;
        public BindingSource PackageStatusesBindingSource = null;

        public struct Labelinfo
        {
            public int PackageCount;
            public string OrderDate;
            public string Group;
        }

        public int UserID
        {
            get { return iUserID; }
            set { iUserID = value; }
        }

        public Labelinfo LabelInfo;

        public ZOVDispatchCheckLabel(ref PercentageDataGrid tFrontsPackContentDataGrid, ref PercentageDataGrid tDecorPackContentDataGrid,
            ref PercentageDataGrid tPackagesDataGrid, ref PercentageDataGrid tdgvScanPackages, ref PercentageDataGrid tdgvNotScanPackages, ref PercentageDataGrid tdgvWrongPackages)
        {
            FrontsPackContentDataGrid = tFrontsPackContentDataGrid;
            DecorPackContentDataGrid = tDecorPackContentDataGrid;
            PackagesDataGrid = tPackagesDataGrid;
            dgvScanPackages = tdgvScanPackages;
            dgvNotScanPackages = tdgvNotScanPackages;
            dgvWrongPackages = tdgvWrongPackages;

            Initialize();
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            FrontsGridSettings();
            DecorGridSettings();
            PackagesGridSetting(ref PackagesDataGrid);
            CheckPackagesGridSetting(ref dgvScanPackages);
            CheckPackagesGridSetting(ref dgvNotScanPackages);
            CheckPackagesGridSetting(ref dgvWrongPackages);
        }

        private void Create()
        {
            DispatchIDs = new ArrayList();
            TrayIDs = new ArrayList();

            OrderStatusInfoDT = new DataTable()
            {
                TableName = "OrderStatusInfo"
            };
            OrderStatusInfoDT.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("FactoryID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilProductionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilStorageStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilExpeditionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilDispatchStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSProductionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSStorageStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSExpeditionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSDispatchStatusID", Type.GetType("System.Int32")));

            FrontsPackContentDataTable = new DataTable();
            DecorPackContentDataTable = new DataTable();
            PackagesDataTable = new DataTable();
            ScanedPackagesDT = new DataTable();
            PackageStatusesDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();
        }

        private void GetColorsDT()
        {
            FrameColorsDataTable = new DataTable();
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
            }
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 FrontsOrders.FrontID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.ColorID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechnoProfilesDataTable = new DataTable();
                DA.Fill(TechnoProfilesDataTable);

                DataRow NewRow = TechnoProfilesDataTable.NewRow();
                NewRow["TechnoProfileID"] = -1;
                NewRow["TechnoProfileName"] = "-";
                TechnoProfilesDataTable.Rows.InsertAt(NewRow, 0);
            }

            GetColorsDT();
            GetInsetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            ZOVClientsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZOVClientsDataTable);
            }

            string SelectionCommand = "SELECT TOP 0 Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
               " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
               " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
               " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
               " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
               " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }

            PackagesDataTable.Columns.Add(new DataColumn("Group", Type.GetType("System.Int32")));
            ScanedPackagesDT = PackagesDataTable.Clone();
            ScanedPackagesDT.Columns.Add(new DataColumn("IsCorrectScan", Type.GetType("System.Boolean")));
            NotScanedPackagesDT = ScanedPackagesDT.Clone();
            WrongPackagesDT = ScanedPackagesDT.Clone();
        }

        private void Binding()
        {
            PackageStatusesBindingSource = new BindingSource()
            {
                DataSource = PackageStatusesDataTable
            };
            FrontsPackContentBindingSource = new BindingSource()
            {
                DataSource = FrontsPackContentDataTable
            };
            FrontsPackContentDataGrid.DataSource = FrontsPackContentBindingSource;

            DecorPackContentBindingSource = new BindingSource()
            {
                DataSource = DecorPackContentDataTable
            };
            DecorPackContentDataGrid.DataSource = DecorPackContentBindingSource;

            PackagesBindingSource = new BindingSource()
            {
                DataSource = PackagesDataTable
            };
            PackagesDataGrid.DataSource = PackagesBindingSource;

            ScanedPackagesBS = new BindingSource()
            {
                DataSource = ScanedPackagesDT
            };
            dgvScanPackages.DataSource = ScanedPackagesBS;

            NotScanedPackagesBS = new BindingSource()
            {
                DataSource = NotScanedPackagesDT
            };
            dgvNotScanPackages.DataSource = NotScanedPackagesBS;

            WrongPackagesBS = new BindingSource()
            {
                DataSource = WrongPackagesDT
            };
            dgvWrongPackages.DataSource = WrongPackagesBS;
        }

        #region GridSettings

        private void CreateColumns()
        {
            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = new DataView(FrontsDataTable),
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = new DataView(PatinaDataTable),
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = new DataView(InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = new DataView(InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoProfilesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoProfilesColumn",
                HeaderText = "Тип\r\nпрофиля-2",
                DataPropertyName = "TechnoProfileID",
                DataSource = new DataView(TechnoProfilesDataTable),
                ValueMember = "TechnoProfileID",
                DisplayMember = "TechnoProfileName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoFrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoFrameColorsColumn",
                HeaderText = "Цвет профиля-2",
                DataPropertyName = "TechnoColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = new DataView(TechnoInsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = new DataView(TechnoInsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PackagesDataGrid.Columns.Add(PackageStatusesColumn);
            dgvScanPackages.Columns.Add(PackageStatusesColumn);
            dgvNotScanPackages.Columns.Add(PackageStatusesColumn);
            dgvWrongPackages.Columns.Add(PackageStatusesColumn);

            FrontsPackContentDataGrid.Columns.Add(FrontsColumn);
            FrontsPackContentDataGrid.Columns.Add(FrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(PatinaColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetColorsColumn);

            DecorPackContentDataGrid.Columns.Add(ProductColumn);
            DecorPackContentDataGrid.Columns.Add(ItemColumn);
            DecorPackContentDataGrid.Columns.Add(DecorPatinaColumn);
            DecorPackContentDataGrid.Columns.Add(ColorColumn);
        }


        private DataGridViewComboBoxColumn PackageStatusesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PackageStatusesColumn",
                    HeaderText = "Статус",
                    DataPropertyName = "PackageStatusID",
                    DataSource = PackageStatusesBindingSource,
                    ValueMember = "PackageStatusID",
                    DisplayMember = "PackageStatus",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn ProductColumn
        {
            get
            {
                DataGridViewComboBoxColumn ProductColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ProductColumn",
                    HeaderText = "Продукт",
                    DataPropertyName = "ProductID",

                    DataSource = new DataView(DecorProductsDataTable),
                    ValueMember = "ProductID",
                    DisplayMember = "ProductName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ProductColumn;
            }
        }

        public DataGridViewComboBoxColumn ItemColumn
        {
            get
            {
                DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ItemColumn",
                    HeaderText = "Название",
                    DataPropertyName = "DecorID",

                    DataSource = new DataView(DecorDataTable),
                    ValueMember = "DecorID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ItemColumn;
            }
        }

        public DataGridViewComboBoxColumn ColorColumn
        {
            get
            {
                DataGridViewComboBoxColumn ColorsColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ColorsColumn",
                    HeaderText = "Цвет",
                    DataPropertyName = "ColorID",

                    DataSource = new DataView(FrameColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ColorsColumn;
            }
        }

        public DataGridViewComboBoxColumn DecorPatinaColumn
        {
            get
            {
                DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn",
                    HeaderText = "Патина",
                    DataPropertyName = "PatinaID",

                    DataSource = new DataView(PatinaDataTable),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return PatinaColumn;
            }
        }

        private void FrontsGridSettings()
        {
            foreach (DataGridViewColumn Column in FrontsPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            FrontsPackContentDataGrid.Columns["FrontID"].Visible = false;
            FrontsPackContentDataGrid.Columns["PatinaID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["ColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            FrontsPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            int DisplayIndex = 0;
            FrontsPackContentDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;

            FrontsPackContentDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Height"].Width = 90;
            FrontsPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Width"].Width = 90;
            FrontsPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void DecorGridSettings()
        {
            foreach (DataGridViewColumn Column in DecorPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            DecorPackContentDataGrid.Columns["ProductID"].Visible = false;
            DecorPackContentDataGrid.Columns["DecorID"].Visible = false;
            DecorPackContentDataGrid.Columns["ColorID"].Visible = false;
            DecorPackContentDataGrid.Columns["PatinaID"].Visible = false;


            DecorPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            DecorPackContentDataGrid.Columns["Length"].HeaderText = "Длина";
            DecorPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            DecorPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            DecorPackContentDataGrid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            DecorPackContentDataGrid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;

            DecorPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Height"].Width = 90;
            DecorPackContentDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Length"].Width = 90;
            DecorPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Width"].Width = 90;
            DecorPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void PackagesGridSetting(ref PercentageDataGrid grid)
        {
            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["PackageStatusID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["PrintDateTime"].Visible = false;
            grid.Columns["PrintedCount"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["DispatchDateTime"].Visible = false;
            grid.Columns["PalleteID"].Visible = false;
            grid.Columns["Group"].Visible = false;

            if (grid.Columns.Contains("PalleteID"))
                grid.Columns["PalleteID"].Visible = false;
            if (grid.Columns.Contains("DispatchID"))
                grid.Columns["DispatchID"].Visible = false;
            if (grid.Columns.Contains("PackUserID"))
                grid.Columns["PackUserID"].Visible = false;
            if (grid.Columns.Contains("StoreUserID"))
                grid.Columns["StoreUserID"].Visible = false;
            if (grid.Columns.Contains("ExpUserID"))
                grid.Columns["ExpUserID"].Visible = false;
            if (grid.Columns.Contains("DispUserID"))
                grid.Columns["DispUserID"].Visible = false;

            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("PackingDateTime"))
                grid.Columns["PackingDateTime"].Visible = false;
            //if (grid.Columns.Contains("TrayID"))
            //    grid.Columns["TrayID"].Visible = false;
            if (grid.Columns.Contains("FactoryID"))
                grid.Columns["FactoryID"].Visible = false;

            grid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["PackNumber"].HeaderText = "№\r\nупак.";
            grid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            grid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            grid.Columns["PackageID"].HeaderText = "ID";
            grid.Columns["FactoryName"].HeaderText = "Участок";
            grid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            grid.Columns["TrayID"].HeaderText = "№\r\nподдона";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["Group"].HeaderText = "Группа";
            grid.Columns["DispatchDateTime"].HeaderText = "Дата\r\nотгрузки";

            grid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackNumber"].Width = 70;
            grid.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["TrayID"].Width = 70;
            grid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageStatusesColumn"].Width = 140;
            grid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StorageDateTime"].Width = 170;
            grid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpeditionDateTime"].Width = 170;
            grid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageID"].Width = 100;
            grid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["FactoryName"].Width = 100;
            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ClientName"].MinimumWidth = 190;
            grid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["OrderNumber"].MinimumWidth = 150;
            grid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Group"].Width = 70;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            grid.AutoGenerateColumns = false;

            grid.Columns["ClientName"].DisplayIndex = 0;
            grid.Columns["OrderNumber"].DisplayIndex = 1;
            grid.Columns["Notes"].DisplayIndex = 2;
            grid.Columns["PackageStatusesColumn"].DisplayIndex = 3;
            grid.Columns["FactoryName"].DisplayIndex = 4;
            grid.Columns["StorageDateTime"].DisplayIndex = 5;
            grid.Columns["ExpeditionDateTime"].DisplayIndex = 6;
            grid.Columns["TrayID"].DisplayIndex = 7;
            grid.Columns["PackageID"].DisplayIndex = 8;
        }

        private void CheckPackagesGridSetting(ref PercentageDataGrid grid)
        {
            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["PackageStatusID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["PrintDateTime"].Visible = false;
            grid.Columns["PrintedCount"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["DispatchDateTime"].Visible = false;
            grid.Columns["PalleteID"].Visible = false;
            grid.Columns["Group"].Visible = false;

            if (grid.Columns.Contains("PalleteID"))
                grid.Columns["PalleteID"].Visible = false;
            if (grid.Columns.Contains("DispatchID"))
                grid.Columns["DispatchID"].Visible = false;
            if (grid.Columns.Contains("PackUserID"))
                grid.Columns["PackUserID"].Visible = false;
            if (grid.Columns.Contains("StoreUserID"))
                grid.Columns["StoreUserID"].Visible = false;
            if (grid.Columns.Contains("ExpUserID"))
                grid.Columns["ExpUserID"].Visible = false;
            if (grid.Columns.Contains("DispUserID"))
                grid.Columns["DispUserID"].Visible = false;
            if (grid.Columns.Contains("IsCorrectScan"))
                grid.Columns["IsCorrectScan"].Visible = false;

            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("PackingDateTime"))
                grid.Columns["PackingDateTime"].Visible = false;
            //if (grid.Columns.Contains("TrayID"))
            //    grid.Columns["TrayID"].Visible = false;
            if (grid.Columns.Contains("FactoryID"))
                grid.Columns["FactoryID"].Visible = false;

            grid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["PackNumber"].HeaderText = "№\r\nупак.";
            grid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            grid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            grid.Columns["PackageID"].HeaderText = "ID";
            grid.Columns["FactoryName"].HeaderText = "Участок";
            grid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            grid.Columns["TrayID"].HeaderText = "№\r\nподдона";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["Group"].HeaderText = "Группа";
            grid.Columns["DispatchDateTime"].HeaderText = "Дата\r\nотгрузки";

            grid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackNumber"].Width = 70;
            grid.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["TrayID"].Width = 90;
            grid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageStatusesColumn"].Width = 140;
            grid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StorageDateTime"].Width = 170;
            grid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpeditionDateTime"].Width = 170;
            grid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageID"].Width = 100;
            grid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["FactoryName"].Width = 100;
            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ClientName"].MinimumWidth = 190;
            grid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["OrderNumber"].MinimumWidth = 150;
            grid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Group"].Width = 70;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            grid.AutoGenerateColumns = false;

            grid.Columns["ClientName"].DisplayIndex = 0;
            grid.Columns["OrderNumber"].DisplayIndex = 1;
            grid.Columns["Notes"].DisplayIndex = 2;
            grid.Columns["PackageStatusesColumn"].DisplayIndex = 3;
            grid.Columns["FactoryName"].DisplayIndex = 4;
            grid.Columns["StorageDateTime"].DisplayIndex = 5;
            grid.Columns["ExpeditionDateTime"].DisplayIndex = 6;
            grid.Columns["TrayID"].DisplayIndex = 7;
            grid.Columns["PackageID"].DisplayIndex = 8;
        }

        #endregion

        #region Fill

        public bool FillFrontsPackContent(int PackageID)
        {
            FrontsPackContentDataTable.Clear();

            //if (CurrentProductType == 1)
            //    return false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.ColorID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID,FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }

            return FrontsPackContentDataTable.Rows.Count > 0;
        }

        public bool FillDecorPackContent(int PackageID)
        {
            DecorPackContentDataTable.Clear();

            //if (CurrentProductType == 0)
            //    return false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDataTable);
            }

            if (DecorPackContentDataTable.Rows.Count > 0)
            {
                foreach (DataRow Row in DecorPackContentDataTable.Rows)
                {
                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                    //    Row["ColorID"] = 0;
                }
            }

            return DecorPackContentDataTable.Rows.Count > 0;
        }

        public bool HasPackages(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            DataTable DT = new DataTable();

            string SelectionCommand = "SELECT * FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }
            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }

            return DT.Rows.Count > 0;
        }

        public bool FillPackagesByPackage(string Barcode)
        {
            int Group = 1;

            string Prefix = Barcode.Substring(0, 3);

            PackagesDataGrid.DataSource = null;

            PackagesDataTable.Clear();

            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " MegaOrders.OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "001" || Prefix == "002")
            {
                SelectionCommand = "SELECT Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
                   " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                   " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                   " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                   " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                   " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                   " WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

                Group = 1;
                LabelInfo.Group = "ЗОВ";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                Group = 2;
                LabelInfo.Group = "Маркетинг";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            LabelInfo.PackageCount = PackagesDataTable.Rows.Count;

            if (PackagesDataTable.Rows.Count > 0)
                PackagesDataTable.Rows[0]["Group"] = Group;

            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.MoveFirst();

            return PackagesDataTable.Rows.Count > 0;
        }

        public bool HasTray(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            DataTable DT = new DataTable();

            string SelectionCommand = "SELECT * FROM Packages WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }
            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }

            return DT.Rows.Count > 0;
        }

        public bool FillPackagesByTray(string Barcode)
        {
            int Group = 1;

            string Prefix = Barcode.Substring(0, 3);

            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " CAST(MegaOrders.OrderNumber AS CHAR) AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            PackagesDataGrid.DataSource = null;

            PackagesDataTable.Clear();

            if (Prefix == "005")
            {
                Group = 1;
                LabelInfo.Group = "ЗОВ";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                SelectionCommand = "SELECT Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
                    " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                    " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                    " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                    " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                    " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                    " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            if (Prefix == "006")
            {
                Group = 2;
                LabelInfo.Group = "Маркетинг";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            LabelInfo.PackageCount = PackagesDataTable.Rows.Count;

            for (int i = 0; i < PackagesDataTable.Rows.Count; i++)
            {
                PackagesDataTable.Rows[i]["Group"] = Group;
            }

            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.MoveFirst();

            return PackagesDataTable.Rows.Count > 0;
        }

        #endregion

        #region Properties

        public ArrayList CurrentDispatchIDs
        {
            get { return DispatchIDs; }
            set { DispatchIDs = value; }
        }

        /// <summary>
        /// true, если таблица упаковок Packages пуста
        /// </summary>
        public bool IsPackagesTableEmpty
        {
            get { return PackagesDataTable.Rows.Count == 0; }
        }

        public void GetPackageInfo(int PackageID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductType, Packages.MainOrderID, MainOrders.MegaOrderID FROM Packages
                INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    CurrentProductType = Convert.ToInt32(DT.Rows[0]["ProductType"]);
                    CurrentMainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                    CurrentMegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                }
            }
            return;
        }

        public int WrongPackagesCount
        {
            get { return WrongPackagesDT.Rows.Count; }
        }

        public int ScanedPackagesCount
        {
            get { return ScanedPackagesDT.Rows.Count; }
        }

        public int NotScanedPackagesCount
        {
            get { return NotScanedPackagesDT.Rows.Count; }
        }

        public int AllPackagesInDispatchCount()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        return DT.Rows.Count;
                    }
                }
            }
        }

        #endregion

        public void AddPackageToTempTable()
        {
            foreach (DataRow item in PackagesDataTable.Rows)
            {
                object DispatchID = item["DispatchID"];
                int Index = -1;
                if (DispatchID != DBNull.Value)
                    Index = DispatchIDs.IndexOf(Convert.ToInt32(DispatchID));
                if (Index == -1)
                {
                    DataRow NewRow = WrongPackagesDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["IsCorrectScan"] = false;
                    WrongPackagesDT.Rows.Add(NewRow);
                }
                else
                {
                    DataRow NewRow = ScanedPackagesDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["IsCorrectScan"] = true;
                    ScanedPackagesDT.Rows.Add(NewRow);
                }
            }
        }

        public void AddTrayToTempTable(int TrayID)
        {
            TrayIDs.Add(TrayID);
        }

        public void GetNotScanedPackages()
        {
            string SelectionCommand = "SELECT Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
                " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")";

            //001000099182
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    foreach (DataRow item in DT.Rows)
                    {
                        int PackageID = Convert.ToInt32(item["PackageID"]);
                        DataRow[] rows = ScanedPackagesDT.Select("PackageID=" + PackageID);
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = NotScanedPackagesDT.NewRow();
                            NewRow.ItemArray = item.ItemArray;
                            NewRow["IsCorrectScan"] = false;
                            NotScanedPackagesDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
        }

        public void DispatchPackages()
        {
            DateTime CurrentDate = Security.GetCurrentDate();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow[] rows = ScanedPackagesDT.Select("DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")");
                        foreach (DataRow item in rows)
                        {
                            int PackageID = Convert.ToInt32(item["PackageID"]);
                            DataRow[] drows = DT.Select("PackageID=" + PackageID);
                            if (drows.Count() == 0)
                                continue;
                            drows[0]["PackageStatusID"] = 3;
                            if (drows[0]["PackingDateTime"] == DBNull.Value)
                                drows[0]["PackingDateTime"] = CurrentDate;
                            if (drows[0]["StorageDateTime"] == DBNull.Value)
                                drows[0]["StorageDateTime"] = CurrentDate;
                            if (drows[0]["ExpeditionDateTime"] == DBNull.Value)
                                drows[0]["ExpeditionDateTime"] = CurrentDate;
                            if (drows[0]["DispatchDateTime"] == DBNull.Value)
                                drows[0]["DispatchDateTime"] = CurrentDate;

                            if (drows[0]["PackUserID"] == DBNull.Value)
                                drows[0]["PackUserID"] = iUserID;
                            if (drows[0]["StoreUserID"] == DBNull.Value)
                                drows[0]["StoreUserID"] = iUserID;
                            if (drows[0]["ExpUserID"] == DBNull.Value)
                                drows[0]["ExpUserID"] = iUserID;
                            if (drows[0]["DispUserID"] == DBNull.Value)
                                drows[0]["DispUserID"] = iUserID;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void DispatchTrays()
        {
            if (TrayIDs.Count == 0)
                return;
            DateTime CurrentDate = Security.GetCurrentDate();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID, TrayStatusID, StorageDateTime, ExpeditionDateTime, DispatchDateTime FROM Trays" +
                " WHERE TrayID IN (" + string.Join(",", TrayIDs.OfType<Int32>().ToArray()) + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["TrayStatusID"] = 2;
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = CurrentDate;
                                if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    DT.Rows[i]["ExpeditionDateTime"] = CurrentDate;
                                if (DT.Rows[i]["DispatchDateTime"] == DBNull.Value)
                                    DT.Rows[i]["DispatchDateTime"] = CurrentDate;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void DispatchDebts()
        {
            DataTable dt1 = new DataTable();
            dt1.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.Int32")));
            //DataRow[] TempRows = ScanedPackagesDT.Select("DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")");
            DataRow[] TempRows = ScanedPackagesDT.Select();
            foreach (DataRow item in TempRows)
            {
                DataRow NewRow = dt1.NewRow();
                NewRow["MainOrderID"] = Convert.ToInt32(item["MainOrderID"]);
                dt1.Rows.Add(NewRow);
            }

            using (DataView DV = new DataView(dt1.Copy()))
            {
                dt1.Clear();
                dt1 = DV.ToTable(true, new string[] { "MainOrderID" });
            }

            //            DataTable DebtsDT = new DataTable();
            //            DataTable ReOrdersDT = new DataTable();
            //            SqlDataAdapter DebtsDA;
            //            SqlDataAdapter ReOrdersDA;
            //            SqlCommandBuilder DebtsCB;
            //            string SelectCommand = string.Empty;

            //            SelectCommand = @"SELECT MainOrderID, DocNumber, ReorderDocNumber FROM MainOrders
            //                WHERE ReorderDocNumber IS NOT NULL AND MegaOrderID = " + MegaOrderID;
            //            DebtsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString);
            //            DebtsCB = new SqlCommandBuilder(DebtsDA);
            //            DebtsDA.Fill(DebtsDT);

            //            SelectCommand = @"SELECT MainOrderID, DocNumber FROM MainOrders
            //                WHERE DocNumber IN (SELECT ReorderDocNumber FROM MainOrders
            //                WHERE ReorderDocNumber IS NOT NULL AND MegaOrderID = " + MegaOrderID + ")";
            //            ReOrdersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString);
            //            ReOrdersDA.Fill(ReOrdersDT);

            //            for (int i = 0; i < DebtsDT.Rows.Count; i++)
            //            {
            //                int DebtMainOrderID = Convert.ToInt32(DebtsDT.Rows[i]["MainOrderID"]);
            //                string ReorderDocNumber = DebtsDT.Rows[i]["ReorderDocNumber"].ToString();
            //                DataRow[] rows = ReOrdersDT.Select("DocNumber='" + ReorderDocNumber + "'");
            //                if (rows.Count() == 0)
            //                    continue;
            //                int ReOrderMainOrderID = Convert.ToInt32(rows[0]["MainOrderID"]);
            //                //if (IsMainOrderDispatched(ReOrderMainOrderID))
            //                //    SetMainOrderDispatch(DebtMainOrderID);
            //            }
        }

        public bool IsDispatchAllowed(int DispatchID)
        {
            using (SqlDataAdapter DA2 = new SqlDataAdapter("SELECT DispatchID, PrepareDispatchDateTime, ConfirmDispDateTime FROM Dispatch" +
                " WHERE DispatchID =" + DispatchID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT2 = new DataTable())
                {
                    if (DA2.Fill(DT2) > 0)
                    {
                        if (DT2.Rows[0]["ConfirmDispDateTime"] != DBNull.Value)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool IsCorrectPackage(int PackageID)
        {
            DataRow[] rows = ScanedPackagesDT.Select("PackageID=" + PackageID);
            if (rows.Count() == 0 || rows[0]["IsCorrectScan"] == DBNull.Value)
                return false;
            bool IsCorrectScan = Convert.ToBoolean(rows[0]["IsCorrectScan"]);
            return IsCorrectScan;
        }

        public string GetOrderDate(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            string DateTime = string.Empty;

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Packages WHERE PackageID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Packages WHERE PackageID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Trays WHERE TrayID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Trays WHERE TrayID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            return DateTime;
        }

        public bool IsPackageLabel(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public bool IsTrayLabel(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Trays WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Trays WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();
            PackagesDataTable.Clear();
        }
    }





    public class MarketingDispatchCheckLabel
    {
        int iUserID = 0;
        ArrayList DispatchIDs;
        ArrayList TrayIDs;

        int CurrentProductType = 0;
        int CurrentClientID = 0;
        int CurrentFactoryID = 0;
        public int CurrentMainOrderID = 0;
        public int CurrentMegaOrderID = 0;

        PercentageDataGrid FrontsPackContentDataGrid = null;
        PercentageDataGrid DecorPackContentDataGrid = null;
        PercentageDataGrid PackagesDataGrid = null;
        PercentageDataGrid dgvScanPackages = null;
        PercentageDataGrid dgvNotScanPackages = null;
        PercentageDataGrid dgvWrongPackages = null;

        DataTable FrontsPackContentDataTable = null;
        DataTable DecorPackContentDataTable = null;
        DataTable PackagesDataTable = null;
        DataTable ScanedPackagesDT = null;
        DataTable NotScanedPackagesDT = null;
        DataTable WrongPackagesDT = null;

        public DataTable FrontsDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        DataTable ZOVClientsDataTable;
        DataTable MarktClientsDataTable;
        DataTable PackageStatusesDataTable;

        DataTable DecorDataTable;
        DataTable DecorProductsDataTable;

        DataTable OrderStatusInfoDT;

        DataTable PackageDetailsDT;
        DataTable DecorAssignmentsDT;
        DataTable StoreDT;
        DataTable ReadyStoreDT;
        DataTable WriteOffStoreDT;
        DataTable MovementInvoicesDT;
        DataTable MovementInvoiceDetailsDT;

        SqlDataAdapter ReadyStoreDA;
        SqlCommandBuilder ReadyStoreCB;
        SqlDataAdapter WriteOffStoreDA;
        SqlCommandBuilder WriteOffStoreCB;
        SqlDataAdapter MovementInvoicesDA;
        SqlCommandBuilder MovementInvoicesCB;
        SqlDataAdapter MovementInvoiceDetailsDA;
        SqlCommandBuilder MovementInvoiceDetailsCB;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        DataGridViewComboBoxColumn DecorColumn;
        DataGridViewComboBoxColumn DecorProductsColumn;
        DataGridViewComboBoxColumn DecorColorColumn;

        public BindingSource FrontsPackContentBindingSource = null;
        public BindingSource DecorPackContentBindingSource = null;
        public BindingSource PackagesBindingSource = null;
        public BindingSource ScanedPackagesBS = null;
        public BindingSource NotScanedPackagesBS = null;
        public BindingSource WrongPackagesBS = null;
        public BindingSource PackageStatusesBindingSource = null;

        public struct Labelinfo
        {
            public int PackageCount;
            public string OrderDate;
            public string Group;
        }

        public int UserID
        {
            get { return iUserID; }
            set { iUserID = value; }
        }

        public Labelinfo LabelInfo;

        public MarketingDispatchCheckLabel(ref PercentageDataGrid tFrontsPackContentDataGrid, ref PercentageDataGrid tDecorPackContentDataGrid,
            ref PercentageDataGrid tPackagesDataGrid, ref PercentageDataGrid tdgvScanPackages, ref PercentageDataGrid tdgvNotScanPackages, ref PercentageDataGrid tdgvWrongPackages)
        {
            FrontsPackContentDataGrid = tFrontsPackContentDataGrid;
            DecorPackContentDataGrid = tDecorPackContentDataGrid;
            PackagesDataGrid = tPackagesDataGrid;
            dgvScanPackages = tdgvScanPackages;
            dgvNotScanPackages = tdgvNotScanPackages;
            dgvWrongPackages = tdgvWrongPackages;

            Initialize();
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            FrontsGridSettings();
            DecorGridSettings();
            PackagesGridSetting(ref PackagesDataGrid);
            CheckPackagesGridSetting(ref dgvScanPackages);
            CheckPackagesGridSetting(ref dgvNotScanPackages);
            CheckPackagesGridSetting(ref dgvWrongPackages);
        }

        private void Create()
        {
            DispatchIDs = new ArrayList();
            TrayIDs = new ArrayList();

            OrderStatusInfoDT = new DataTable()
            {
                TableName = "OrderStatusInfo"
            };
            OrderStatusInfoDT.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("FactoryID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilProductionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilStorageStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilExpeditionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilDispatchStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSProductionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSStorageStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSExpeditionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSDispatchStatusID", Type.GetType("System.Int32")));

            FrontsPackContentDataTable = new DataTable();
            DecorPackContentDataTable = new DataTable();
            PackagesDataTable = new DataTable();
            ScanedPackagesDT = new DataTable();
            PackageStatusesDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            PackageDetailsDT = new DataTable();
            DecorAssignmentsDT = new DataTable();
            StoreDT = new DataTable();

            ReadyStoreDT = new DataTable();
            ReadyStoreDA = new SqlDataAdapter("SELECT TOP 0 * FROM ReadyStore",
                ConnectionStrings.StorageConnectionString);
            ReadyStoreCB = new SqlCommandBuilder(ReadyStoreDA);
            ReadyStoreDA.Fill(ReadyStoreDT);

            WriteOffStoreDT = new DataTable();
            WriteOffStoreDA = new SqlDataAdapter("SELECT TOP 0 * FROM WriteOffStore",
                ConnectionStrings.StorageConnectionString);
            WriteOffStoreCB = new SqlCommandBuilder(WriteOffStoreDA);
            WriteOffStoreDA.Fill(WriteOffStoreDT);

            MovementInvoicesDT = new DataTable();
            MovementInvoicesDA = new SqlDataAdapter("SELECT TOP 1 * FROM MovementInvoices ORDER BY MovementInvoiceID DESC",
                ConnectionStrings.StorageConnectionString);
            MovementInvoicesCB = new SqlCommandBuilder(MovementInvoicesDA);
            MovementInvoicesDA.Fill(MovementInvoicesDT);

            MovementInvoiceDetailsDT = new DataTable();
            MovementInvoiceDetailsDA = new SqlDataAdapter("SELECT TOP 0 * FROM MovementInvoiceDetails",
                ConnectionStrings.StorageConnectionString);
            MovementInvoiceDetailsCB = new SqlCommandBuilder(MovementInvoiceDetailsDA);
            MovementInvoiceDetailsDA.Fill(MovementInvoiceDetailsDT);
        }

        private void GetColorsDT()
        {
            FrameColorsDataTable = new DataTable();
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
            }
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0  FrontsOrders.FrontID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.ColorID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID,FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechnoProfilesDataTable = new DataTable();
                DA.Fill(TechnoProfilesDataTable);

                DataRow NewRow = TechnoProfilesDataTable.NewRow();
                NewRow["TechnoProfileID"] = -1;
                NewRow["TechnoProfileName"] = "-";
                TechnoProfilesDataTable.Rows.InsertAt(NewRow, 0);
            }

            GetColorsDT();
            GetInsetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            MarktClientsDataTable = new DataTable();
            ZOVClientsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MarktClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZOVClientsDataTable);
            }

            string SelectionCommand = "SELECT TOP 0 Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " CAST(MegaOrders.OrderNumber AS CHAR) AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }

            PackagesDataTable.Columns.Add(new DataColumn("Group", Type.GetType("System.Int32")));
            ScanedPackagesDT = PackagesDataTable.Clone();
            ScanedPackagesDT.Columns.Add(new DataColumn("IsCorrectScan", Type.GetType("System.Boolean")));
            NotScanedPackagesDT = ScanedPackagesDT.Clone();
            WrongPackagesDT = ScanedPackagesDT.Clone();
        }

        private void Binding()
        {
            PackageStatusesBindingSource = new BindingSource()
            {
                DataSource = PackageStatusesDataTable
            };
            FrontsPackContentBindingSource = new BindingSource()
            {
                DataSource = FrontsPackContentDataTable
            };
            FrontsPackContentDataGrid.DataSource = FrontsPackContentBindingSource;

            DecorPackContentBindingSource = new BindingSource()
            {
                DataSource = DecorPackContentDataTable
            };
            DecorPackContentDataGrid.DataSource = DecorPackContentBindingSource;

            PackagesBindingSource = new BindingSource()
            {
                DataSource = PackagesDataTable
            };
            PackagesDataGrid.DataSource = PackagesBindingSource;

            ScanedPackagesBS = new BindingSource()
            {
                DataSource = ScanedPackagesDT
            };
            dgvScanPackages.DataSource = ScanedPackagesBS;

            NotScanedPackagesBS = new BindingSource()
            {
                DataSource = NotScanedPackagesDT
            };
            dgvNotScanPackages.DataSource = NotScanedPackagesBS;

            WrongPackagesBS = new BindingSource()
            {
                DataSource = WrongPackagesDT
            };
            dgvWrongPackages.DataSource = WrongPackagesBS;
        }

        #region GridSettings

        private void CreateColumns()
        {
            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = new DataView(FrontsDataTable),
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = new DataView(PatinaDataTable),
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = new DataView(InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = new DataView(InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoProfilesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoProfilesColumn",
                HeaderText = "Тип\r\nпрофиля-2",
                DataPropertyName = "TechnoProfileID",
                DataSource = new DataView(TechnoProfilesDataTable),
                ValueMember = "TechnoProfileID",
                DisplayMember = "TechnoProfileName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoFrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoFrameColorsColumn",
                HeaderText = "Цвет профиля-2",
                DataPropertyName = "TechnoColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = new DataView(TechnoInsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = new DataView(TechnoInsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            DecorColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DecorColumn",
                HeaderText = "Наименование",
                DataPropertyName = "DecorID",
                DataSource = new DataView(DecorDataTable),
                ValueMember = "DecorID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0
            };
            DecorProductsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DecorProductsColumn",
                HeaderText = "Продукт",
                DataPropertyName = "ProductID",
                DataSource = new DataView(DecorProductsDataTable),
                ValueMember = "ProductID",
                DisplayMember = "ProductName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0
            };
            DecorColorColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DecorColorColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 5
            };
            PackagesDataGrid.Columns.Add(PackageStatusesColumn);
            dgvScanPackages.Columns.Add(PackageStatusesColumn);
            dgvNotScanPackages.Columns.Add(PackageStatusesColumn);
            dgvWrongPackages.Columns.Add(PackageStatusesColumn);

            FrontsPackContentDataGrid.Columns.Add(FrontsColumn);
            FrontsPackContentDataGrid.Columns.Add(PatinaColumn);
            FrontsPackContentDataGrid.Columns.Add(FrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetColorsColumn);

            DecorPackContentDataGrid.Columns.Add(DecorColumn);
            DecorPackContentDataGrid.Columns.Add(DecorProductsColumn);
            DecorPackContentDataGrid.Columns.Add(DecorColorColumn);
        }


        private DataGridViewComboBoxColumn PackageStatusesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PackageStatusesColumn",
                    HeaderText = "Статус",
                    DataPropertyName = "PackageStatusID",
                    DataSource = PackageStatusesBindingSource,
                    ValueMember = "PackageStatusID",
                    DisplayMember = "PackageStatus",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }


        private void FrontsGridSettings()
        {
            foreach (DataGridViewColumn Column in FrontsPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            FrontsPackContentDataGrid.Columns["FrontID"].Visible = false;
            FrontsPackContentDataGrid.Columns["PatinaID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["ColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            FrontsPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            FrontsPackContentDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Height"].Width = 90;
            FrontsPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Width"].Width = 90;
            FrontsPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void DecorGridSettings()
        {
            foreach (DataGridViewColumn Column in DecorPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            DecorPackContentDataGrid.Columns["ProductID"].Visible = false;
            DecorPackContentDataGrid.Columns["DecorID"].Visible = false;
            DecorPackContentDataGrid.Columns["ColorID"].Visible = false;


            DecorPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            DecorPackContentDataGrid.Columns["Length"].HeaderText = "Длина";
            DecorPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            DecorPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";


            DecorPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Height"].Width = 90;
            DecorPackContentDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Length"].Width = 90;
            DecorPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Width"].Width = 90;
            DecorPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void PackagesGridSetting(ref PercentageDataGrid grid)
        {
            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["PackageStatusID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["PrintDateTime"].Visible = false;
            grid.Columns["PrintedCount"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["DispatchDateTime"].Visible = false;
            grid.Columns["PalleteID"].Visible = false;
            grid.Columns["Group"].Visible = false;

            if (grid.Columns.Contains("PalleteID"))
                grid.Columns["PalleteID"].Visible = false;
            if (grid.Columns.Contains("DispatchID"))
                grid.Columns["DispatchID"].Visible = false;
            if (grid.Columns.Contains("PackUserID"))
                grid.Columns["PackUserID"].Visible = false;
            if (grid.Columns.Contains("StoreUserID"))
                grid.Columns["StoreUserID"].Visible = false;
            if (grid.Columns.Contains("ExpUserID"))
                grid.Columns["ExpUserID"].Visible = false;
            if (grid.Columns.Contains("DispUserID"))
                grid.Columns["DispUserID"].Visible = false;

            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("PackingDateTime"))
                grid.Columns["PackingDateTime"].Visible = false;
            //if (grid.Columns.Contains("TrayID"))
            //    grid.Columns["TrayID"].Visible = false;
            if (grid.Columns.Contains("FactoryID"))
                grid.Columns["FactoryID"].Visible = false;

            grid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["PackNumber"].HeaderText = "№\r\nупак.";
            grid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            grid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            grid.Columns["PackageID"].HeaderText = "ID";
            grid.Columns["FactoryName"].HeaderText = "Участок";
            grid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            grid.Columns["TrayID"].HeaderText = "№\r\nподдона";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["Group"].HeaderText = "Группа";
            grid.Columns["DispatchDateTime"].HeaderText = "Дата\r\nотгрузки";

            grid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackNumber"].Width = 70;
            grid.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["TrayID"].Width = 70;
            grid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageStatusesColumn"].Width = 140;
            grid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StorageDateTime"].Width = 170;
            grid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpeditionDateTime"].Width = 170;
            grid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageID"].Width = 100;
            grid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["FactoryName"].Width = 100;
            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ClientName"].MinimumWidth = 190;
            grid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["OrderNumber"].MinimumWidth = 150;
            grid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Group"].Width = 70;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            grid.AutoGenerateColumns = false;

            grid.Columns["ClientName"].DisplayIndex = 0;
            grid.Columns["OrderNumber"].DisplayIndex = 1;
            grid.Columns["Notes"].DisplayIndex = 2;
            grid.Columns["PackageStatusesColumn"].DisplayIndex = 3;
            grid.Columns["FactoryName"].DisplayIndex = 4;
            grid.Columns["StorageDateTime"].DisplayIndex = 5;
            grid.Columns["ExpeditionDateTime"].DisplayIndex = 6;
            grid.Columns["TrayID"].DisplayIndex = 7;
            grid.Columns["PackageID"].DisplayIndex = 8;
        }

        private void CheckPackagesGridSetting(ref PercentageDataGrid grid)
        {
            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["PackageStatusID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["PrintDateTime"].Visible = false;
            grid.Columns["PrintedCount"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["DispatchDateTime"].Visible = false;
            grid.Columns["PalleteID"].Visible = false;
            grid.Columns["Group"].Visible = false;

            if (grid.Columns.Contains("PalleteID"))
                grid.Columns["PalleteID"].Visible = false;
            if (grid.Columns.Contains("DispatchID"))
                grid.Columns["DispatchID"].Visible = false;
            if (grid.Columns.Contains("PackUserID"))
                grid.Columns["PackUserID"].Visible = false;
            if (grid.Columns.Contains("StoreUserID"))
                grid.Columns["StoreUserID"].Visible = false;
            if (grid.Columns.Contains("ExpUserID"))
                grid.Columns["ExpUserID"].Visible = false;
            if (grid.Columns.Contains("DispUserID"))
                grid.Columns["DispUserID"].Visible = false;
            if (grid.Columns.Contains("IsCorrectScan"))
                grid.Columns["IsCorrectScan"].Visible = false;

            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("PackingDateTime"))
                grid.Columns["PackingDateTime"].Visible = false;
            //if (grid.Columns.Contains("TrayID"))
            //    grid.Columns["TrayID"].Visible = false;
            if (grid.Columns.Contains("FactoryID"))
                grid.Columns["FactoryID"].Visible = false;

            grid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["PackNumber"].HeaderText = "№\r\nупак.";
            grid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            grid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            grid.Columns["PackageID"].HeaderText = "ID";
            grid.Columns["FactoryName"].HeaderText = "Участок";
            grid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            grid.Columns["TrayID"].HeaderText = "№\r\nподдона";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["Group"].HeaderText = "Группа";
            grid.Columns["DispatchDateTime"].HeaderText = "Дата\r\nотгрузки";

            grid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackNumber"].Width = 70;
            grid.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["TrayID"].Width = 90;
            grid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageStatusesColumn"].Width = 140;
            grid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["StorageDateTime"].Width = 170;
            grid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["ExpeditionDateTime"].Width = 170;
            grid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PackageID"].Width = 100;
            grid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["FactoryName"].Width = 100;
            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ClientName"].MinimumWidth = 190;
            grid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["OrderNumber"].MinimumWidth = 150;
            grid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Group"].Width = 70;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            grid.AutoGenerateColumns = false;

            grid.Columns["ClientName"].DisplayIndex = 0;
            grid.Columns["OrderNumber"].DisplayIndex = 1;
            grid.Columns["Notes"].DisplayIndex = 2;
            grid.Columns["PackageStatusesColumn"].DisplayIndex = 3;
            grid.Columns["FactoryName"].DisplayIndex = 4;
            grid.Columns["StorageDateTime"].DisplayIndex = 5;
            grid.Columns["ExpeditionDateTime"].DisplayIndex = 6;
            grid.Columns["TrayID"].DisplayIndex = 7;
            grid.Columns["PackageID"].DisplayIndex = 8;
        }

        #endregion

        #region Fill

        public bool FillFrontsPackContent(int PackageID)
        {
            FrontsPackContentDataTable.Clear();

            //if (CurrentProductType == 1)
            //    return false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.ColorID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID,FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }

            return FrontsPackContentDataTable.Rows.Count > 0;
        }

        public bool FillDecorPackContent(int PackageID)
        {
            DecorPackContentDataTable.Clear();

            //if (CurrentProductType == 0)
            //    return false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDataTable);
            }

            if (DecorPackContentDataTable.Rows.Count > 0)
            {
                foreach (DataRow Row in DecorPackContentDataTable.Rows)
                {
                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                    //    Row["ColorID"] = 0;
                }
            }

            return DecorPackContentDataTable.Rows.Count > 0;
        }

        public bool HasPackages(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            DataTable DT = new DataTable();

            string SelectionCommand = "SELECT * FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }
            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }

            return DT.Rows.Count > 0;
        }

        public bool FillPackagesByPackage(string Barcode)
        {
            int Group = 1;

            string Prefix = Barcode.Substring(0, 3);

            PackagesDataGrid.DataSource = null;

            PackagesDataTable.Clear();

            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " MegaOrders.OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "001" || Prefix == "002")
            {
                SelectionCommand = "SELECT Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
                   " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                   " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                   " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                   " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                   " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                   " WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

                Group = 1;
                LabelInfo.Group = "ЗОВ";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                Group = 2;
                LabelInfo.Group = "Маркетинг";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            LabelInfo.PackageCount = PackagesDataTable.Rows.Count;

            if (PackagesDataTable.Rows.Count > 0)
                PackagesDataTable.Rows[0]["Group"] = Group;

            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.MoveFirst();

            return PackagesDataTable.Rows.Count > 0;
        }

        public bool HasTray(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            DataTable DT = new DataTable();

            string SelectionCommand = "SELECT * FROM Packages WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }
            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                }
            }

            return DT.Rows.Count > 0;
        }

        public bool FillPackagesByTray(string Barcode)
        {
            int Group = 1;

            string Prefix = Barcode.Substring(0, 3);

            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " CAST(MegaOrders.OrderNumber AS CHAR) AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            PackagesDataGrid.DataSource = null;

            PackagesDataTable.Clear();

            if (Prefix == "005")
            {
                Group = 1;
                LabelInfo.Group = "ЗОВ";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                SelectionCommand = "SELECT Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
                    " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                    " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                    " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                    " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                    " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                    " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            if (Prefix == "006")
            {
                Group = 2;
                LabelInfo.Group = "Маркетинг";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            LabelInfo.PackageCount = PackagesDataTable.Rows.Count;

            for (int i = 0; i < PackagesDataTable.Rows.Count; i++)
            {
                PackagesDataTable.Rows[i]["Group"] = Group;
            }

            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.MoveFirst();

            return PackagesDataTable.Rows.Count > 0;
        }

        #endregion

        #region Properties

        public ArrayList CurrentDispatchIDs
        {
            get { return DispatchIDs; }
            set { DispatchIDs = value; }
        }

        /// <summary>
        /// true, если таблица упаковок Packages пуста
        /// </summary>
        public bool IsPackagesTableEmpty
        {
            get { return PackagesDataTable.Rows.Count == 0; }
        }

        public void GetPackageInfo(int PackageID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductType, Packages.FactoryID, Packages.MainOrderID, MainOrders.MegaOrderID, MegaOrders.ClientID FROM Packages
                INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    CurrentFactoryID = Convert.ToInt32(DT.Rows[0]["FactoryID"]);
                    CurrentClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                    CurrentProductType = Convert.ToInt32(DT.Rows[0]["ProductType"]);
                    CurrentMainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                    CurrentMegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                }
            }
            return;
        }

        public int WrongPackagesCount
        {
            get { return WrongPackagesDT.Rows.Count; }
        }

        public int ScanedPackagesCount
        {
            get { return ScanedPackagesDT.Rows.Count; }
        }

        public int NotScanedPackagesCount
        {
            get { return NotScanedPackagesDT.Rows.Count; }
        }

        public int AllPackagesInDispatchCount()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        return DT.Rows.Count;
                    }
                }
            }
        }

        #endregion

        public void AddPackageToTempTable()
        {
            foreach (DataRow item in PackagesDataTable.Rows)
            {
                object DispatchID = item["DispatchID"];
                int Index = -1;
                if (DispatchID != DBNull.Value)
                    Index = DispatchIDs.IndexOf(Convert.ToInt32(DispatchID));
                if (Index == -1)
                {
                    DataRow NewRow = WrongPackagesDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["IsCorrectScan"] = false;
                    WrongPackagesDT.Rows.Add(NewRow);
                }
                else
                {
                    DataRow NewRow = ScanedPackagesDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["IsCorrectScan"] = true;
                    ScanedPackagesDT.Rows.Add(NewRow);
                }
            }
        }

        public void AddTrayToTempTable(int TrayID)
        {
            TrayIDs.Add(TrayID);
        }

        public void GetNotScanedPackages()
        {
            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " MegaOrders.OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")";

            //001000099182
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    foreach (DataRow item in DT.Rows)
                    {
                        int PackageID = Convert.ToInt32(item["PackageID"]);
                        DataRow[] rows = ScanedPackagesDT.Select("PackageID=" + PackageID);
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = NotScanedPackagesDT.NewRow();
                            NewRow.ItemArray = item.ItemArray;
                            NewRow["IsCorrectScan"] = false;
                            NotScanedPackagesDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
        }

        public void DispatchPackages()
        {
            DateTime CurrentDate = Security.GetCurrentDate();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow[] rows = ScanedPackagesDT.Select("DispatchID IN (" + string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + ")");
                        foreach (DataRow item in rows)
                        {
                            int PackageID = Convert.ToInt32(item["PackageID"]);
                            DataRow[] drows = DT.Select("PackageID=" + PackageID);
                            if (drows.Count() == 0)
                                continue;
                            drows[0]["PackageStatusID"] = 3;
                            if (drows[0]["PackingDateTime"] == DBNull.Value)
                                drows[0]["PackingDateTime"] = CurrentDate;
                            if (drows[0]["StorageDateTime"] == DBNull.Value)
                                drows[0]["StorageDateTime"] = CurrentDate;
                            if (drows[0]["ExpeditionDateTime"] == DBNull.Value)
                                drows[0]["ExpeditionDateTime"] = CurrentDate;
                            if (drows[0]["DispatchDateTime"] == DBNull.Value)
                                drows[0]["DispatchDateTime"] = CurrentDate;

                            if (drows[0]["PackUserID"] == DBNull.Value)
                                drows[0]["PackUserID"] = iUserID;
                            if (drows[0]["StoreUserID"] == DBNull.Value)
                                drows[0]["StoreUserID"] = iUserID;
                            if (drows[0]["ExpUserID"] == DBNull.Value)
                                drows[0]["ExpUserID"] = iUserID;
                            if (drows[0]["DispUserID"] == DBNull.Value)
                                drows[0]["DispUserID"] = iUserID;
                            WriteOffFromStore(PackageID);
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void DispatchTrays()
        {
            if (TrayIDs.Count == 0)
                return;
            DateTime CurrentDate = Security.GetCurrentDate();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID, TrayStatusID, StorageDateTime, ExpeditionDateTime, DispatchDateTime FROM Trays" +
                " WHERE TrayID IN (" + string.Join(",", TrayIDs.OfType<Int32>().ToArray()) + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["TrayStatusID"] = 2;
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = CurrentDate;
                                if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    DT.Rows[i]["ExpeditionDateTime"] = CurrentDate;
                                if (DT.Rows[i]["DispatchDateTime"] == DBNull.Value)
                                    DT.Rows[i]["DispatchDateTime"] = CurrentDate;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public bool IsDispatchAllowed(int DispatchID)
        {
            using (SqlDataAdapter DA2 = new SqlDataAdapter("SELECT DispatchID, PrepareDispatchDateTime, ConfirmDispDateTime FROM Dispatch" +
                " WHERE DispatchID =" + DispatchID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT2 = new DataTable())
                {
                    if (DA2.Fill(DT2) > 0)
                    {
                        if (DT2.Rows[0]["ConfirmDispDateTime"] != DBNull.Value)
                            return true;
                        else
                            DispatchIDs.Remove(DispatchID);
                    }
                }
            }
            return false;
        }

        public bool IsCorrectPackage(int PackageID)
        {
            DataRow[] rows = ScanedPackagesDT.Select("PackageID=" + PackageID);
            if (rows.Count() == 0 || rows[0]["IsCorrectScan"] == DBNull.Value)
                return false;
            bool IsCorrectScan = Convert.ToBoolean(rows[0]["IsCorrectScan"]);
            return IsCorrectScan;
        }

        public string GetOrderDate(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            string DateTime = string.Empty;

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Packages WHERE PackageID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Packages WHERE PackageID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Trays WHERE TrayID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Trays WHERE TrayID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            return DateTime;
        }

        public bool IsPackageLabel(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public bool IsTrayLabel(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Trays WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Trays WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();
            PackagesDataTable.Clear();
        }

        public void WriteOffFromStore(int PackageID)
        {
            if (CurrentProductType == 0)
                return;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            MovementInvoiceDetailsDT.Clear();
            string SelectCommand = @"SELECT * FROM PackageDetails WHERE PackageID = " + PackageID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                PackageDetailsDT.Clear();
                DA.Fill(PackageDetailsDT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE DecorOrderID IN (SELECT OrderID FROM infiniu2_marketingorders.dbo.PackageDetails WHERE PackageID = " + PackageID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DecorAssignmentsDT.Clear();
                DA.Fill(DecorAssignmentsDT);
            }
            SelectCommand = @"SELECT * FROM Store WHERE DecorAssignmentID IN (SELECT DecorAssignmentID FROM DecorAssignments WHERE DecorOrderID IN (SELECT OrderID FROM infiniu2_marketingorders.dbo.PackageDetails WHERE PackageID = " + PackageID + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                StoreDT.Clear();
                DA.Fill(StoreDT);
            }
            SelectCommand = @"SELECT * FROM ReadyStore WHERE CurrentCount<>0 AND DecorAssignmentID IN (SELECT DecorAssignmentID FROM DecorAssignments WHERE DecorOrderID IN (SELECT OrderID FROM infiniu2_marketingorders.dbo.PackageDetails WHERE PackageID = " + PackageID + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                ReadyStoreDT.Clear();
                DA.Fill(ReadyStoreDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM WriteOffStore";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                WriteOffStoreDT.Clear();
                DA.Fill(WriteOffStoreDT);
            }

            //НАЙТИ ID КЛИЕНТА
            int RecipientStoreAllocID = 12;
            int SellerStoreAllocID = 10;
            if (CurrentFactoryID == 2)
            {
                SellerStoreAllocID = 11;
                RecipientStoreAllocID = 13;
            }
            int MovementInvoiceID = SaveMovementInvoices(SellerStoreAllocID, RecipientStoreAllocID, 0, iUserID, Security.CurrentUserShortName, iUserID, CurrentClientID, 0, string.Empty, "Отгрузка");
            DateTime CreateDateTime = Security.GetCurrentDate();
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(PackageDetailsDT))
            {
                DT = DV.ToTable(true, "OrderID");
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                int OrderID = 0;
                if (DT.Rows[i]["OrderID"] != DBNull.Value)
                    OrderID = Convert.ToInt32(DT.Rows[i]["OrderID"]);
                DataRow[] Rows = PackageDetailsDT.Select("OrderID = " + OrderID);
                decimal Count = 0;
                decimal ConstCount = 0;
                decimal InvoiceCount = 0;
                foreach (DataRow item in Rows)
                    ConstCount += Convert.ToDecimal(item["Count"]);

                for (int j = 0; j < DecorAssignmentsDT.Rows.Count; j++)
                {
                    Count = ConstCount;
                    int StoreItemID = Convert.ToInt32(DecorAssignmentsDT.Rows[j]["TechStoreID2"]);
                    DataRow[] Rows1 = ReadyStoreDT.Select("StoreItemID = " + StoreItemID);
                    foreach (var item in Rows1)
                    {
                        int ReadyStoreID = 0;
                        if (item["ReadyStoreID"] != DBNull.Value)
                            ReadyStoreID = Convert.ToInt32(item["ReadyStoreID"]);
                        InvoiceCount = MoveToWriteOffStore(MovementInvoiceID, CreateDateTime, ReadyStoreID, ref Count);
                        AddMovementInvoiceDetail(MovementInvoiceID, CreateDateTime, ReadyStoreID, InvoiceCount);
                        if (Count <= 0)
                            break;
                    }
                }
            }
            ReadyStoreDA.Update(ReadyStoreDT);
            WriteOffStoreDA.Update(WriteOffStoreDT);
            FillWriteOffStoreMovementInvoiceDetails(MovementInvoiceID);
            MovementInvoiceDetailsDA.Update(MovementInvoiceDetailsDT);

            sw.Stop();
            double t = 0;
            t = sw.Elapsed.TotalMilliseconds;
        }

        public void AddMovementInvoiceDetail(int MovementInvoiceID, DateTime CreateDateTime, int StoreIDFrom, decimal Count)
        {
            DataRow[] Rows = MovementInvoiceDetailsDT.Select("StoreIDFrom = " + StoreIDFrom);
            if (Rows.Count() > 0)
            {
                Rows[0]["Count"] = Convert.ToDecimal(Rows[0]["Count"]) + Count;
            }
            else
            {
                DataRow NewRow = MovementInvoiceDetailsDT.NewRow();
                if (MovementInvoiceDetailsDT.Columns.Contains("CreateDateTime"))
                    NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["MovementInvoiceID"] = MovementInvoiceID;
                NewRow["StoreIDFrom"] = StoreIDFrom;
                NewRow["StoreIDTo"] = 0;
                NewRow["Count"] = Count;
                MovementInvoiceDetailsDT.Rows.Add(NewRow);
            }

        }

        public void FillWriteOffStoreMovementInvoiceDetails(int MovementInvoiceID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM WriteOffStore" +
                   " WHERE MovementInvoiceID = " + MovementInvoiceID,
                   ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    int j = 0;

                    for (int i = 0; i < MovementInvoiceDetailsDT.Rows.Count; i++)
                    {
                        if (MovementInvoiceDetailsDT.Rows[i].RowState == DataRowState.Deleted)
                        {
                            continue;
                        }

                        int StoreID = Convert.ToInt32(MovementInvoiceDetailsDT.Rows[i]["StoreIDTo"]);

                        if (StoreID == 0)
                        {
                            MovementInvoiceDetailsDT.Rows[i]["StoreIDTo"] = DT.Rows[j++]["WriteOffStoreID"];
                        }
                        else
                        {
                            DataRow[] Rows = DT.Select("WriteOffStoreID = " + StoreID);
                            if (Rows.Count() > 0)
                            {
                                j++;
                            }
                            else
                                MovementInvoiceDetailsDT.Rows[i].Delete();
                        }
                    }
                }
            }
        }

        public int SaveMovementInvoices(
            int SellerStoreAllocID,
            int RecipientStoreAllocID, int RecipientSectorID,
            int PersonID, string PersonName, int StoreKeeperID,
            int ClientID, int SellerID,
            string ClientName, string Notes)
        {
            int LastMovementInvoiceID = 0;
            DateTime CurrentDate = Security.GetCurrentDate();

            DataRow NewRow = MovementInvoicesDT.NewRow();

            NewRow["DateTime"] = CurrentDate;
            NewRow["SellerStoreAllocID"] = SellerStoreAllocID;
            NewRow["RecipientStoreAllocID"] = RecipientStoreAllocID;
            NewRow["RecipientSectorID"] = RecipientSectorID;
            NewRow["PersonID"] = PersonID;
            NewRow["PersonName"] = PersonName;
            NewRow["StoreKeeperID"] = StoreKeeperID;
            NewRow["ClientName"] = ClientName;
            NewRow["ClientID"] = ClientID;
            NewRow["SellerID"] = SellerID;
            NewRow["Notes"] = Notes;

            MovementInvoicesDT.Rows.Add(NewRow);

            MovementInvoicesDA.Update(MovementInvoicesDT);
            MovementInvoicesDT.Clear();
            MovementInvoicesDA.Fill(MovementInvoicesDT);
            if (MovementInvoicesDT.Rows.Count > 0)
                LastMovementInvoiceID = Convert.ToInt32(MovementInvoicesDT.Rows[MovementInvoicesDT.Rows.Count - 1]["MovementInvoiceID"]);

            return LastMovementInvoiceID;
        }

        public decimal MoveToWriteOffStore(int MovementInvoiceID, DateTime CreateDateTime, int ReadyStoreID, ref decimal Count)
        {
            DataRow[] Rows = ReadyStoreDT.Select("ReadyStoreID = " + ReadyStoreID);
            decimal CurrentCount = 0;
            decimal InvoiceCount = 0;
            if (Rows.Count() > 0)
            {
                CurrentCount = Convert.ToDecimal(Rows[0]["CurrentCount"]);
                InvoiceCount = CurrentCount - Count;

                if (CurrentCount - Count < 0)
                {
                    Rows[0]["CurrentCount"] = 0;
                    InvoiceCount = CurrentCount;
                }
                else
                {
                    Rows[0]["CurrentCount"] = CurrentCount - Count;
                    InvoiceCount = Count;
                }

                DataRow NewRow = WriteOffStoreDT.NewRow();
                if (WriteOffStoreDT.Columns.Contains("CreateDateTime"))
                    NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["StoreItemID"] = Rows[0]["StoreItemID"];
                NewRow["Length"] = Rows[0]["Length"];
                NewRow["Width"] = Rows[0]["Width"];
                NewRow["Height"] = Rows[0]["Height"];
                NewRow["Thickness"] = Rows[0]["Thickness"];
                NewRow["Diameter"] = Rows[0]["Diameter"];
                NewRow["Admission"] = Rows[0]["Admission"];
                NewRow["Capacity"] = Rows[0]["Capacity"];
                NewRow["Weight"] = Rows[0]["Weight"];
                NewRow["ColorID"] = Rows[0]["ColorID"];
                NewRow["CoverID"] = Rows[0]["CoverID"];
                NewRow["PatinaID"] = Rows[0]["PatinaID"];
                NewRow["InvoiceCount"] = InvoiceCount;
                NewRow["CurrentCount"] = InvoiceCount;
                NewRow["MovementInvoiceID"] = MovementInvoiceID;
                NewRow["FactoryID"] = Rows[0]["FactoryID"];
                NewRow["Notes"] = Rows[0]["Notes"];
                NewRow["DecorAssignmentID"] = Rows[0]["DecorAssignmentID"];
                WriteOffStoreDT.Rows.Add(NewRow);
                Count = Count - CurrentCount;
            }
            return InvoiceCount;
        }

    }
}
