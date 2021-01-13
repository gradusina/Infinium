using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.Marketing.NewOrders.PrepareReport
{
    public class FrontsReport
    {
        FrontsCalculate FC = null;
        int ClientID = 0;
        string ProfilCurrencyCode = "0";
        string TPSCurrencyCode = "0";
        decimal PaymentRate = 0;
        string UNN = string.Empty;

        DataTable CurrencyTypesDT;
        DataTable ProfilFrontsOrdersDataTable = null;
        DataTable TPSFrontsOrdersDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        public DataTable TechnoInsetTypesDataTable = null;
        public DataTable TechnoInsetColorsDataTable = null;
        DataTable MeasuresDataTable = null;
        DataTable FactoryDataTable = null;
        DataTable GridSizesDataTable = null;
        DataTable FrontsConfigDataTable = null;
        DataTable TechStoreDataTable = null;

        public DataTable ProfilReportDataTable = null;
        public DataTable TPSReportDataTable = null;

        public FrontsReport(ref FrontsCalculate tFC)
        {
            FC = tFC;

            Create();
            CreateReportDataTables();
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
            ProfilFrontsOrdersDataTable = new DataTable();
            TPSFrontsOrdersDataTable = new DataTable();

            string SelectCommand = "SELECT * FROM CurrencyTypes";
            CurrencyTypesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDT);
            }

            SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
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
            GetColorsDT();
            GetInsetColorsDT();
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            MeasuresDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }

            FactoryDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryDataTable);
            }

            GridSizesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM GridSizes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(GridSizesDataTable);
            }

            FrontsConfigDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsConfig",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(FrontsConfigDataTable);
            //}
            FrontsConfigDataTable = TablesManager.FrontsConfigDataTable;

            TechStoreDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStore",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(TechStoreDataTable);
            //}
            TechStoreDataTable = TablesManager.TechStoreDataTable;
        }

        public void ClearReport()
        {
            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();
        }

        private void CreateReportDataTables()
        {
            ProfilReportDataTable = new DataTable();
            ProfilReportDataTable.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("OriginalPrice", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("DiscountVolume", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("TotalDiscount", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("IsNonStandard", Type.GetType("System.Boolean")));
            ProfilReportDataTable.Columns.Add(new DataColumn("NonStandardMargin", Type.GetType("System.Decimal")));

            TPSReportDataTable = ProfilReportDataTable.Clone();
        }

        public string GetFrontName(int FrontID)
        {
            DataRow[] Row = FrontsDataTable.Select("FrontID = " + FrontID);

            return Row[0]["FrontName"].ToString();
        }

        private void SplitTables(DataTable FrontsOrdersDataTable, ref DataTable ProfilDT, ref DataTable TPSDT)
        {
            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersDataTable.Rows[i]["FrontConfigID"].ToString());

                if (Convert.ToDateTime(FrontsOrdersDataTable.Rows[i]["CreateDateTime"]) < new DateTime(2019, 10, 01))
                {
                    if (Rows[0]["AreaID"].ToString() == "1")//profil
                        ProfilDT.ImportRow(FrontsOrdersDataTable.Rows[i]);

                    if (Rows[0]["AreaID"].ToString() == "2")//tps
                        TPSDT.ImportRow(FrontsOrdersDataTable.Rows[i]);
                }
                else
                {
                    if (Rows[0]["FactoryID"].ToString() == "1")//profil
                        ProfilDT.ImportRow(FrontsOrdersDataTable.Rows[i]);

                    if (Rows[0]["FactoryID"].ToString() == "2")//tps
                        TPSDT.ImportRow(FrontsOrdersDataTable.Rows[i]);
                }
            }
        }

        private int GetMeasureType(int FrontConfigID)
        {
            return Convert.ToInt32(FrontsConfigDataTable.Select("FrontConfigID = " + FrontConfigID.ToString())[0]["MeasureID"]);
        }

        private decimal GetInsetSquare(int FrontID, int Height, int Width)
        {
            decimal GridHeight = 0;
            decimal GridWidth = 0;
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + FrontID);
            if (Rows.Count() > 0)
            {
                if (Rows[0]["InsetHeightAdmission"] != DBNull.Value)
                    GridHeight = Height - Convert.ToInt32(Rows[0]["InsetHeightAdmission"]);
                if (Rows[0]["InsetWidthAdmission"] != DBNull.Value)
                    GridWidth = Width - Convert.ToInt32(Rows[0]["InsetWidthAdmission"]);
                if (FrontID == 3729)
                {
                    return Decimal.Round(Convert.ToInt32(Rows[0]["InsetHeightAdmission"]) * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
                }
            }
            return Decimal.Round(GridHeight * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
        }

        private decimal GetInsetSquare(DataRow FrontsOrdersRow)
        {
            decimal GridHeight = 0;
            decimal GridWidth = 0;
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + Convert.ToInt32(FrontsOrdersRow["FrontID"]));
            if (Rows.Count() > 0)
            {
                if (Rows[0]["InsetHeightAdmission"] != DBNull.Value)
                    GridHeight = Convert.ToInt32(FrontsOrdersRow["Height"]) - Convert.ToInt32(Rows[0]["InsetHeightAdmission"]);
                if (Rows[0]["InsetWidthAdmission"] != DBNull.Value)
                    GridWidth = Convert.ToInt32(FrontsOrdersRow["Width"]) - Convert.ToInt32(Rows[0]["InsetWidthAdmission"]);
                if (Convert.ToInt32(FrontsOrdersRow["FrontID"]) == 3729)
                {
                    return Decimal.Round(Convert.ToInt32(Rows[0]["InsetHeightAdmission"]) * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
                }
            }
            return Decimal.Round(GridHeight * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
        }

        private void GetGlassMarginAluminium(DataRow FrontsOrdersRow, ref int GlassMarginHeight, ref int GlassMarginWidth)
        {
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + Convert.ToInt32(FrontsOrdersRow["FrontID"]));
            if (Rows.Count() > 0)
            {
                if (Rows[0]["InsetHeightAdmission"] != DBNull.Value)
                    GlassMarginHeight = Convert.ToInt32(Rows[0]["InsetHeightAdmission"]);
                if (Rows[0]["InsetWidthAdmission"] != DBNull.Value)
                    GlassMarginWidth = Convert.ToInt32(Rows[0]["InsetWidthAdmission"]);
            }
        }

        public void GetMegaOrderInfo(int MainOrderID)
        {
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, Rate, ClientID, CurrencyTypeID FROM NewMegaOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM NewMainOrders WHERE MainOrderID = " + MainOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int CurrencyTypeID = 0;
                        if (DT.Rows[0]["ClientID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["ClientID"].ToString(), out ClientID);
                        if (DT.Rows[0]["CurrencyTypeID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["CurrencyTypeID"].ToString(), out CurrencyTypeID);

                        DataRow[] rows = CurrencyTypesDT.Select("CurrencyTypeID = " + CurrencyTypeID);
                        if (rows.Count() > 0)
                        {
                            ProfilCurrencyCode = rows[0]["CurrencyCode"].ToString();
                            TPSCurrencyCode = rows[0]["TPSCurrencyCode"].ToString();
                        }
                    }
                }
            }
            SelectCommand = "SELECT UNN FROM Clients WHERE ClientID = " + ClientID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["UNN"] != DBNull.Value)
                            UNN = DT.Rows[0]["UNN"].ToString();
                    }
                }
            }
        }

        private decimal GetNonStandardMargin(int FrontConfigID)
        {
            DataRow[] Rows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontConfigID);

            return Convert.ToDecimal(Rows[0]["ZOVNonStandMargin"]);
        }

        private void GetSimpleFronts(DataTable OrdersDataTable, DataTable ReportDataTable, bool IsNonStandard)
        {
            string IsNonStandardFilter = "IsNonStandard=0";
            DataTable Fronts = new DataTable();
            string AccountingName = OrdersDataTable.Rows[0]["AccountingName"].ToString();
            string InvNumber = OrdersDataTable.Rows[0]["InvNumber"].ToString();
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            if (IsNonStandard)
                IsNonStandardFilter = "IsNonStandard=1";

            using (DataView DV = new DataView(OrdersDataTable))
            {
                DV.RowFilter = IsNonStandardFilter;
                Fronts = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Fronts.Rows.Count; i++)
            {
                decimal SolidCount = 0;
                decimal SolidCost = 0;
                decimal OriginalSolidCost = 0;
                decimal WithTransportSolidCost = 0;
                decimal SolidWeight = 0;

                decimal FilenkaCount = 0;
                decimal FilenkaCost = 0;
                decimal OriginalFilenkaCost = 0;
                decimal WithTransportFilenkaCost = 0;
                decimal FilenkaWeight = 0;

                decimal VitrinaCount = 0;
                decimal VitrinaCost = 0;
                decimal OriginalVitrinaCost = 0;
                decimal WithTransportVitrinaCost = 0;
                decimal VitrinaWeight = 0;

                decimal LuxCount = 0;
                decimal LuxCost = 0;
                decimal OriginalLuxCost = 0;
                decimal WithTransportLuxCost = 0;
                decimal LuxWeight = 0;

                decimal MegaCount = 0;
                decimal MegaCost = 0;
                decimal OriginalMegaCost = 0;
                decimal WithTransportMegaCost = 0;
                decimal MegaWeight = 0;

                decimal TotalDiscount = 0;

                object NonStandardMargin = 0;
                IsNonStandardFilter = " AND IsNonStandard=0";
                if (IsNonStandard)
                    IsNonStandardFilter = " AND IsNonStandard=1";
                //ГЛУХИЕ, БЕЗ ВСТАВКИ, РЕШЕТКА ОВАЛ
                DataRow[] rows = InsetTypesDataTable.Select("InsetTypeID=-1 OR GroupID = 3 OR GroupID = 4");
                string filter = string.Empty;
                foreach (DataRow item in rows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = " AND NOT (FrontID IN (3728,3731,3732,3739,3740,3741,3744,3745,3746) OR InsetTypeID IN (28961,3653,3654,3655)) AND (FrontID = 3729 OR InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + "))";
                DataRow[] Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    decimal DeductibleCost = 0;
                    decimal DeductibleCount = 0;
                    decimal DeductibleWeight = 0;
                    if (Convert.ToInt32(Fronts.Rows[i]["FrontID"]) == 3729)//РЕШЕТКА ОВАЛ
                    {
                        DeductibleWeight = GetInsetWeight(Rows[r]);

                        int MarginHeight = 0;
                        int MarginWidth = 0;
                        GetGlassMarginAluminium(Rows[r], ref MarginHeight, ref MarginWidth);
                        decimal InsetSquare = MarginHeight * (Convert.ToDecimal(Rows[r]["Width"]) - MarginWidth) / 1000000;
                        InsetSquare = Decimal.Round(InsetSquare, 3, MidpointRounding.AwayFromZero);
                        DeductibleCount = InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                        DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                        DeductibleWeight = Decimal.Round(DeductibleCount * DeductibleWeight, 3, MidpointRounding.AwayFromZero);
                    }

                    TotalDiscount = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                    SolidCount += Convert.ToDecimal(Rows[r]["Square"]);

                    SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    OriginalSolidCost += Convert.ToDecimal(Rows[r]["OriginalPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    DeductibleCost = Math.Ceiling(DeductibleCost / 0.01m) * 0.01m;
                    WithTransportSolidCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]) - DeductibleCost;

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    SolidWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //АППЛИКАЦИИ
                filter = " AND (FrontID IN (3728,3731,3732,3739,3740,3741,3744,3745,3746) OR InsetTypeID IN (28961,3653,3654,3655))";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    TotalDiscount = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                    decimal DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * Convert.ToDecimal(Rows[r]["Count"]);
                    if (Convert.ToInt32(Rows[r]["FrontID"]) == 3728 || Convert.ToInt32(Rows[r]["FrontID"]) == 3731 || Convert.ToInt32(Rows[r]["FrontID"]) == 3732 ||
                        Convert.ToInt32(Rows[r]["FrontID"]) == 3739 || Convert.ToInt32(Rows[r]["FrontID"]) == 3740 || Convert.ToInt32(Rows[r]["FrontID"]) == 3741 ||
                        Convert.ToInt32(Rows[r]["FrontID"]) == 3744 || Convert.ToInt32(Rows[r]["FrontID"]) == 3745 || Convert.ToInt32(Rows[r]["FrontID"]) == 3746)
                    {
                        SolidCount += Convert.ToDecimal(Rows[r]["Square"]);

                        SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) - DeductibleCost;
                        OriginalSolidCost += Convert.ToDecimal(Rows[r]["OriginalPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) - DeductibleCost;

                        WithTransportSolidCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]) - DeductibleCost;

                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        SolidWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    }
                    else if (Convert.ToInt32(Rows[r]["FrontID"]) == 3415 || Convert.ToInt32(Rows[r]["FrontID"]) == 28922)
                    {
                        FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                        FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) - DeductibleCost;
                        OriginalFilenkaCost += Convert.ToDecimal(Rows[r]["OriginalPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) - DeductibleCost;

                        WithTransportFilenkaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]) - DeductibleCost;

                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        FilenkaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    }
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ФИЛЕНКА
                filter = " AND InsetTypeID IN (2069,2070,2071,2073,2075,2077,2233,3644,29043,29531)";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    TotalDiscount = Convert.ToDecimal(Rows[r]["TotalDiscount"]);

                    FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    OriginalFilenkaCost += Convert.ToDecimal(Rows[r]["OriginalPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    WithTransportFilenkaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]);

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    FilenkaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ВИТРИНЫ, РЕШЕТКИ, СТЕКЛО
                filter = " AND InsetTypeID IN (1,2,685,686,687,688,29470,29471) AND FrontID <> 3729";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    TotalDiscount = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                    decimal DeductibleCount = 0;
                    decimal DeductibleCost = 0;
                    decimal DeductibleWeight = 0;
                    //РЕШЕТКА 45,90,ПЛАСТИК
                    if (Convert.ToInt32(Rows[r]["InsetTypeID"]) == 685 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 686 ||
                        Convert.ToInt32(Rows[r]["InsetTypeID"]) == 687 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 688 ||
                        Convert.ToInt32(Rows[r]["InsetTypeID"]) == 29470 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 29471)
                    {
                        decimal InsetSquare = GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]), Convert.ToInt32(Rows[r]["Width"]));
                        InsetSquare = Decimal.Round(InsetSquare, 3, MidpointRounding.AwayFromZero);
                        DeductibleCount = InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                        DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                        DeductibleWeight = Decimal.Round(DeductibleCount * Convert.ToDecimal(3.5), 3, MidpointRounding.AwayFromZero);
                    }
                    //СТЕКЛО
                    if (Convert.ToInt32(Rows[r]["InsetTypeID"]) == 2)
                    {
                        DeductibleCount = Convert.ToDecimal(Rows[r]["Count"]) * GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]), Convert.ToInt32(Rows[r]["Width"]));
                        DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * DeductibleCount;
                        DeductibleWeight = Decimal.Round(DeductibleCount * 10, 3, MidpointRounding.AwayFromZero);
                    }

                    VitrinaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (FC.IsAluminium(Rows[r]) > -1)
                    {
                        VitrinaCost += FC.GetFrontCostAluminium(ClientID, Rows[r]);
                        VitrinaWeight += FC.GetAluminiumWeight(Rows[r], true);
                    }
                    else
                    {
                        VitrinaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;
                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);
                        VitrinaWeight += Convert.ToDecimal(FrontWeight + InsetWeight - DeductibleWeight);
                    }
                    VitrinaCost = Math.Ceiling(VitrinaCost / 0.01m) * 0.01m;
                    OriginalVitrinaCost += Convert.ToDecimal(Rows[r]["OriginalPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    DeductibleCost = Math.Ceiling(DeductibleCost / 0.01m) * 0.01m;
                    WithTransportVitrinaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]) - DeductibleCost;

                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ЛЮКС
                filter = " AND InsetTypeID IN (860)";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    TotalDiscount = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                    LuxCount += Convert.ToDecimal(Rows[r]["Square"]);
                    LuxCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    OriginalLuxCost += Convert.ToDecimal(Rows[r]["OriginalPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    WithTransportLuxCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]);

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    LuxWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //МЕГА, Планка
                filter = " AND InsetTypeID IN (862,4310)";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    TotalDiscount = Convert.ToDecimal(Rows[r]["TotalDiscount"]);

                    MegaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    MegaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    OriginalMegaCost += Convert.ToDecimal(Rows[r]["OriginalPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    WithTransportMegaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]);

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    MegaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }

                if (!IsNonStandard)
                    NonStandardMargin = DBNull.Value;

                //SolidCost = Math.Ceiling(SolidCost / 0.01m) * 0.01m;
                //WithTransportSolidCost = Math.Ceiling(WithTransportSolidCost / 0.01m) * 0.01m;
                if (SolidCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " глухой";
                    Row["Count"] = Decimal.Round(SolidCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(SolidCost / SolidCount, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(OriginalSolidCost / SolidCount, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportSolidCost / SolidCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportSolidCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(SolidCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(SolidWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //FilenkaCount = Math.Ceiling(FilenkaCount / 0.01m) * 0.01m;
                //WithTransportFilenkaCost = Math.Ceiling(WithTransportFilenkaCost / 0.01m) * 0.01m;
                if (FilenkaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " филенка";
                    Row["Count"] = Decimal.Round(FilenkaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(FilenkaCost / FilenkaCount, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(OriginalFilenkaCost / FilenkaCount, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportFilenkaCost / FilenkaCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportFilenkaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(FilenkaCount / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(FilenkaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //VitrinaCost = Math.Ceiling(VitrinaCost / 0.01m) * 0.01m;
                //WithTransportVitrinaCost = Math.Ceiling(WithTransportVitrinaCost / 0.01m) * 0.01m;
                //decimal PriceWithTransport = WithTransportVitrinaCost / VitrinaCount;
                //PriceWithTransport = Decimal.Round(WithTransportVitrinaCost / VitrinaCount, 2, MidpointRounding.AwayFromZero);
                if (VitrinaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " витрина";
                    Row["Count"] = Decimal.Round(VitrinaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(VitrinaCost / VitrinaCount, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(OriginalVitrinaCost / VitrinaCount, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportVitrinaCost / VitrinaCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportVitrinaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(VitrinaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(VitrinaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //LuxCost = Math.Ceiling(LuxCost / 0.01m) * 0.01m;
                //WithTransportLuxCost = Math.Ceiling(WithTransportLuxCost / 0.01m) * 0.01m;
                if (LuxCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " люкс";
                    Row["Count"] = Decimal.Round(LuxCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(LuxCost / LuxCount, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(OriginalLuxCost / LuxCount, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportLuxCost / LuxCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportLuxCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(LuxCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(LuxWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //MegaCost = Math.Ceiling(MegaCost / 0.01m) * 0.01m;
                //WithTransportMegaCost = Math.Ceiling(WithTransportMegaCost / 0.01m) * 0.01m;
                if (MegaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " мега";
                    Row["Count"] = Decimal.Round(MegaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(MegaCost / MegaCount, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(OriginalMegaCost / MegaCount, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportMegaCost / MegaCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportMegaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(MegaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(MegaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }
            }

            Fronts.Dispose();
        }

        private void GetCurvedFronts(DataTable OrdersDataTable, DataTable ReportDataTable)
        {
            string AccountingName = OrdersDataTable.Rows[0]["AccountingName"].ToString();
            string InvNumber = OrdersDataTable.Rows[0]["InvNumber"].ToString();
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            DataTable Fronts = new DataTable();

            using (DataView DV = new DataView(OrdersDataTable))
            {
                Fronts = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Fronts.Rows.Count; i++)
            {
                DataRow[] Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() +
                                                              " AND Width = -1");

                if (Rows.Count() == 0)
                    continue;

                decimal Solid713Count = 0;
                decimal Solid713Price = 0;
                decimal Solid713OriginalPrice = 0;
                decimal Solid713WithTransportCost = 0;
                decimal Solid713Weight = 0;

                decimal Filenka713Count = 0;
                decimal Filenka713Price = 0;
                decimal Filenka713OriginalPrice = 0;
                decimal Filenka713WithTransportCost = 0;
                decimal Filenka713Weight = 0;

                decimal NoInset713Count = 0;
                decimal NoInset713Price = 0;
                decimal NoInset713OriginalPrice = 0;
                decimal NoInset713WithTransportCost = 0;
                decimal NoInset713Weight = 0;

                decimal Vitrina713Count = 0;
                decimal Vitrina713Price = 0;
                decimal Vitrina713OriginalPrice = 0;
                decimal Vitrina713WithTransportCost = 0;
                decimal Vitrina713Weight = 0;

                decimal Solid910Count = 0;
                decimal Solid910Price = 0;
                decimal Solid910OriginalPrice = 0;
                decimal Solid910WithTransportCost = 0;
                decimal Solid910Weight = 0;

                decimal Filenka910Count = 0;
                decimal Filenka910Price = 0;
                decimal Filenka910OriginalPrice = 0;
                decimal Filenka910WithTransportCost = 0;
                decimal Filenka910Weight = 0;

                decimal NoInset910Count = 0;
                decimal NoInset910Price = 0;
                decimal NoInset910OriginalPrice = 0;
                decimal NoInset910WithTransportCost = 0;
                decimal NoInset910Weight = 0;

                decimal Vitrina910Count = 0;
                decimal Vitrina910Price = 0;
                decimal Vitrina910OriginalPrice = 0;
                decimal Vitrina910WithTransportCost = 0;
                decimal Vitrina910Weight = 0;

                decimal TotalDiscount = 0;

                for (int r = 0; r < Rows.Count(); r++)
                {
                    TotalDiscount = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                    if (Rows[r]["Height"].ToString() == "713")
                    {
                        DataRow[] rows = InsetTypesDataTable.Select("GroupID = 3 OR GroupID = 4");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Solid713Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Solid713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Solid713OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                                Solid713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Solid713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        rows = InsetTypesDataTable.Select("InsetTypeID IN (2079,2080,2081,2082,2085,2086,2087,2088,2212,2213,29210,29211,27831,27832,29210,29211)");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Filenka713Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Filenka713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Filenka713OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                                Filenka713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Filenka713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        if (Rows[r]["InsetTypeID"].ToString() == "-1")
                        {
                            NoInset713Count += Convert.ToDecimal(Rows[r]["Count"]);
                            NoInset713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            NoInset713OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NoInset713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina713Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            Vitrina713OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            Vitrina713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            Vitrina713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }
                    }

                    if (Rows[r]["Height"].ToString() == "910")
                    {
                        DataRow[] rows = InsetTypesDataTable.Select("GroupID = 3 OR GroupID = 4");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Solid910Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Solid910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Solid910OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                                Solid910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Solid910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        rows = InsetTypesDataTable.Select("InsetTypeID IN (2079,2080,2081,2082,2085,2086,2087,2088,2212,2213,29210,29211,27831,27832,29210,29211)");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Filenka910Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Filenka910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Filenka910OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                                Filenka910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Filenka910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        if (Rows[r]["InsetTypeID"].ToString() == "-1")
                        {
                            NoInset910Count += Convert.ToDecimal(Rows[r]["Count"]);
                            NoInset910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            NoInset910OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NoInset910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina910Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            Vitrina910OriginalPrice = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            Vitrina910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            Vitrina910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }
                    }

                }

                //decimal Cost = Math.Ceiling(Solid713Count * Solid713Price / 0.01m) * 0.01m;
                //Solid713WithTransportCost = Math.Ceiling(Solid713WithTransportCost / 0.01m) * 0.01m;
                if (Solid713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 глух.";
                    Row["Count"] = Solid713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Solid713Price, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(Solid713OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(Solid713WithTransportCost / Solid713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Solid713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Solid713Count * Solid713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Filenka713Count * Filenka713Price / 0.01m) * 0.01m;
                //Filenka713WithTransportCost = Math.Ceiling(Filenka713WithTransportCost / 0.01m) * 0.01m;
                if (Filenka713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 филенка";
                    Row["Count"] = Filenka713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Filenka713Price, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(Filenka713OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(Filenka713WithTransportCost / Filenka713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Filenka713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Filenka713Count * Filenka713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(NoInset713Count * NoInset713Price / 0.01m) * 0.01m;
                //NoInset713WithTransportCost = Math.Ceiling(NoInset713WithTransportCost / 0.01m) * 0.01m;
                if (NoInset713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 фрезер.";
                    Row["Count"] = NoInset713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(NoInset713Price, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(NoInset713OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(NoInset713WithTransportCost / NoInset713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(NoInset713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(NoInset713Count * NoInset713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Vitrina713Count * Vitrina713Price / 0.01m) * 0.01m;
                //Vitrina713WithTransportCost = Math.Ceiling(Vitrina713WithTransportCost / 0.01m) * 0.01m;
                if (Vitrina713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 витрина";
                    Row["Count"] = Vitrina713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Vitrina713Price, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(Vitrina713OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(Vitrina713WithTransportCost / Vitrina713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Vitrina713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Vitrina713Count * Vitrina713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Solid910Count * Solid910Price / 0.01m) * 0.01m;
                //Solid910WithTransportCost = Math.Ceiling(Solid910WithTransportCost / 0.01m) * 0.01m;
                if (Solid910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 глух.";
                    Row["Count"] = Solid910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Solid910Price, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(Solid910OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(Solid910WithTransportCost / Solid910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Solid910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Solid910Count * Solid910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Filenka910Count * Filenka910Price / 0.01m) * 0.01m;
                //Filenka910WithTransportCost = Math.Ceiling(Filenka910WithTransportCost / 0.01m) * 0.01m;
                if (Filenka910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 филенка";
                    Row["Count"] = Filenka910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Filenka910Price, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(Filenka910OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(Filenka910WithTransportCost / Filenka910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Filenka910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Filenka910Count * Filenka910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(NoInset910Count * NoInset910Price / 0.01m) * 0.01m;
                //NoInset910WithTransportCost = Math.Ceiling(NoInset910WithTransportCost / 0.01m) * 0.01m;
                if (NoInset910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 фрезер.";
                    Row["Count"] = NoInset910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(NoInset910Price, 3, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(NoInset910OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(NoInset910WithTransportCost / NoInset910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(NoInset910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(NoInset910Count * NoInset910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Vitrina910Count * Vitrina910Price / 0.01m) * 0.01m;
                //Vitrina910WithTransportCost = Math.Ceiling(Vitrina910WithTransportCost / 0.01m) * 0.01m;
                if (Vitrina910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["TotalDiscount"] = TotalDiscount;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 витрина";
                    Row["Count"] = Vitrina910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Vitrina910Price, 2, MidpointRounding.AwayFromZero);
                    Row["OriginalPrice"] = Decimal.Round(Vitrina910OriginalPrice, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(Vitrina910WithTransportCost / Vitrina910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Vitrina910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Vitrina910Count * Vitrina910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }
            }

            Fronts.Dispose();
        }

        private void GetGrids(DataTable OrdersDataTable, DataTable ReportDataTable)
        {
            DataRow[] Rows = OrdersDataTable.Select("InsetTypeID IN (685,686,687,688,29470,29471)");
            if (Rows.Count() == 0)
                return;

            int InsetTypeID = Convert.ToInt32(OrdersDataTable.Rows[0]["InsetTypeID"]);
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            decimal Count = 0;
            decimal Cost = 0;
            decimal Cost1 = 0;
            decimal TotalDiscount = 0;

            for (int i = 0; i < Rows.Count(); i++)
            {
                int FrontID = Convert.ToInt32(Rows[i]["FrontID"]);
                if (FrontID == 3729)
                    continue;
                decimal d = GetInsetSquare(Convert.ToInt32(Rows[i]["FrontID"]), Convert.ToInt32(Rows[i]["Height"]), Convert.ToInt32(Rows[i]["Width"])) * Convert.ToDecimal(Rows[i]["Count"]);
                Count += d;
                Cost += Math.Ceiling(Convert.ToDecimal(Rows[i]["InsetPrice"]) * d / 0.001m) * 0.001m;
                Cost1 += Math.Ceiling(Convert.ToDecimal(Rows[i]["OriginalInsetPrice"]) * d / 0.001m) * 0.001m;
                TotalDiscount = Convert.ToDecimal(Rows[i]["TotalDiscount"]);
            }
            Cost = (Cost / 0.01m) * 0.01m;
            Cost1 = (Cost1 / 0.01m) * 0.01m;
            if (Count > 0)
            {
                Count = Decimal.Round(Count, 3, MidpointRounding.AwayFromZero);
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = OrdersDataTable.Rows[0]["AccountingName"].ToString();
                NewRow["InvNumber"] = OrdersDataTable.Rows[0]["InvNumber"].ToString();
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["Name"] = "Решетка";
                NewRow["Count"] = Decimal.Round(Count, 3, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = "м.кв.";
                NewRow["TotalDiscount"] = TotalDiscount;
                NewRow["OriginalPrice"] = Decimal.Round(Cost1 / Count, 2, MidpointRounding.AwayFromZero);
                NewRow["Price"] = Decimal.Round(Cost1 / Count, 2, MidpointRounding.AwayFromZero);
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(Cost1 / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["PriceWithTransport"] = Decimal.Round(Cost / Count, 2, MidpointRounding.AwayFromZero);
                NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(Cost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(3.5), 3, MidpointRounding.AwayFromZero);
                ReportDataTable.Rows.Add(NewRow);
            }
        }

        private void GetGlass(DataTable OrdersDataTable, DataTable ReportDataTable)
        {
            string AccountingName = OrdersDataTable.Rows[0]["AccountingName"].ToString();
            string InvNumber = OrdersDataTable.Rows[0]["InvNumber"].ToString();
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            decimal CountFlutes = 0;
            decimal CountLacomat = 0;
            //decimal CountMaster = 0;
            decimal CountKrizet = 0;
            decimal CountOther = 0;

            decimal PriceFlutes = 0;
            decimal PriceLacomat = 0;
            //decimal PriceMaster = 0;
            decimal PriceKrizet = 0;
            if (OrdersDataTable.Select("InsetTypeID = 2").Count() == 0)
                return;

            bool hasGlass = false;
            for (int i = 0; i < OrdersDataTable.Rows.Count; i++)
            {
                int frontID = Convert.ToInt32(OrdersDataTable.Rows[0]["FrontID"]);
                if (FC.IsAluminium(OrdersDataTable.Rows[i]) > -1)
                {
                    hasGlass = true;
                    break;
                }
            }
            if (!hasGlass)
                return;

            decimal TotalDiscount = 0;
            DataRow[] FRows = OrdersDataTable.Select("InsetColorID = 3944");

            if (FRows.Count() > 0)
            {
                PriceFlutes = Convert.ToDecimal(FRows[0]["InsetPrice"]);

                for (int i = 0; i < FRows.Count(); i++)
                {
                    if (FC.IsAluminium(FRows[i]) != -1)
                        continue;

                    TotalDiscount = Convert.ToDecimal(FRows[i]["TotalDiscount"]);
                    CountFlutes += Convert.ToDecimal(FRows[i]["Count"]) * GetInsetSquare(Convert.ToInt32(FRows[i]["FrontID"]), Convert.ToInt32(FRows[i]["Height"]), Convert.ToInt32(FRows[i]["Width"]));
                }

                CountFlutes = Decimal.Round(CountFlutes, 3, MidpointRounding.AwayFromZero);

                if (CountFlutes > 0)
                {
                    //decimal Cost = Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m;
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["UNN"] = UNN;
                    NewRow["PaymentRate"] = PaymentRate;
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                    NewRow["TotalDiscount"] = TotalDiscount;
                    NewRow["Name"] = "Стекло Флутес";
                    NewRow["Count"] = CountFlutes;
                    NewRow["Measure"] = "м.кв.";
                    NewRow["Price"] = Decimal.Round(PriceFlutes, 2, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["PriceWithTransport"] = Decimal.Round(PriceFlutes, 2, MidpointRounding.AwayFromZero);
                    NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["Weight"] = Decimal.Round(CountFlutes * 10, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(NewRow);
                }
            }


            DataRow[] LRows = OrdersDataTable.Select("InsetColorID = 3943");

            if (LRows.Count() > 0)
            {
                PriceLacomat = Convert.ToDecimal(LRows[0]["InsetPrice"]);

                for (int i = 0; i < LRows.Count(); i++)
                {
                    if (FC.IsAluminium(LRows[i]) != -1)
                        continue;

                    TotalDiscount = Convert.ToDecimal(LRows[i]["TotalDiscount"]);
                    CountLacomat += Convert.ToDecimal(LRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(LRows[i]["FrontID"]),
                                              Convert.ToInt32(LRows[i]["Height"]),
                                              Convert.ToInt32(LRows[i]["Width"]));

                }

                CountLacomat = Decimal.Round(CountLacomat, 3, MidpointRounding.AwayFromZero);

                if (CountLacomat > 0)
                {
                    //decimal Cost = Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m;
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["UNN"] = UNN;
                    NewRow["PaymentRate"] = PaymentRate;
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                    NewRow["TotalDiscount"] = TotalDiscount;
                    NewRow["Name"] = "Стекло Лакомат";
                    NewRow["Count"] = CountLacomat;
                    NewRow["Measure"] = "м.кв.";
                    NewRow["Price"] = Decimal.Round(PriceLacomat, 2, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["PriceWithTransport"] = Decimal.Round(PriceLacomat, 2, MidpointRounding.AwayFromZero);
                    NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["Weight"] = Decimal.Round(CountLacomat * 10, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(NewRow);
                }
            }

            DataRow[] KRows = OrdersDataTable.Select("InsetColorID = 3945");

            if (KRows.Count() > 0)
            {
                PriceKrizet = Convert.ToDecimal(KRows[0]["InsetPrice"]);

                for (int i = 0; i < KRows.Count(); i++)
                {
                    if (FC.IsAluminium(KRows[i]) != -1)
                        continue;

                    TotalDiscount = Convert.ToDecimal(KRows[i]["TotalDiscount"]);
                    CountKrizet += Convert.ToDecimal(KRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(KRows[i]["FrontID"]),
                                              Convert.ToInt32(KRows[i]["Height"]),
                                              Convert.ToInt32(KRows[i]["Width"]));
                }

                CountKrizet = Decimal.Round(CountKrizet, 3, MidpointRounding.AwayFromZero);

                if (CountKrizet > 0)
                {
                    //decimal Cost = Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m;
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["UNN"] = UNN;
                    NewRow["PaymentRate"] = PaymentRate;
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                    NewRow["TotalDiscount"] = TotalDiscount;
                    NewRow["Name"] = "Стекло Кризет";
                    NewRow["Count"] = CountKrizet;
                    NewRow["Measure"] = "м.кв.";
                    NewRow["Price"] = Decimal.Round(PriceKrizet, 2, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["PriceWithTransport"] = Decimal.Round(PriceKrizet, 2, MidpointRounding.AwayFromZero);
                    NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["Weight"] = Decimal.Round(CountKrizet * 10, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(NewRow);
                }
            }
            DataRow[] ORows = OrdersDataTable.Select("InsetColorID = 18");

            if (ORows.Count() > 0)
            {
                for (int i = 0; i < ORows.Count(); i++)
                {
                    if (FC.IsAluminium(ORows[i]) != -1)
                        continue;

                    TotalDiscount = Convert.ToDecimal(ORows[i]["TotalDiscount"]);
                    CountOther += Convert.ToDecimal(ORows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(ORows[i]["FrontID"]),
                                              Convert.ToInt32(ORows[i]["Height"]),
                                              Convert.ToInt32(ORows[i]["Width"]));
                }

                CountOther = Decimal.Round(CountOther, 3, MidpointRounding.AwayFromZero);

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["TotalDiscount"] = TotalDiscount;
                NewRow["Name"] = "Стекло другое";
                NewRow["Count"] = CountOther;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = 0;
                NewRow["Cost"] = 0;
                NewRow["PriceWithTransport"] = 0;
                NewRow["CostWithTransport"] = 0;
                NewRow["Weight"] = Decimal.Round(CountOther * 10, 3, MidpointRounding.AwayFromZero);
                ReportDataTable.Rows.Add(NewRow);
            }
        }

        private void GetInsets(DataTable OrdersDataTable, DataTable ReportDataTable)
        {
            string AccountingName = OrdersDataTable.Rows[0]["AccountingName"].ToString();
            string InvNumber = OrdersDataTable.Rows[0]["InvNumber"].ToString();
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            int CountApplic1R = 0;
            int CountApplic1L = 0;
            int CountApplic2 = 0;
            decimal CountEllipseGrid = 0;

            decimal PriceApplic1R = 0;
            decimal PriceApplic1L = 0;
            decimal PriceApplic2 = 0;
            decimal PriceEllipseGrid = 0;
            decimal TotalDiscount = 0;

            DataRow[] EGRows = OrdersDataTable.Select("FrontID IN (3729)");//ellipse grid

            if (EGRows.Count() > 0)
            {
                int MarginHeight = 0;
                int MarginWidth = 0;

                GetGlassMarginAluminium(EGRows[0], ref MarginHeight, ref MarginWidth);
                PriceEllipseGrid = Convert.ToDecimal(EGRows[0]["InsetPrice"]);

                for (int i = 0; i < EGRows.Count(); i++)
                {
                    TotalDiscount = Convert.ToDecimal(EGRows[i]["TotalDiscount"]);
                    decimal dd = Decimal.Round(Convert.ToDecimal(EGRows[i]["Count"]) * MarginHeight * (Convert.ToDecimal(EGRows[i]["Width"]) - MarginWidth) / 1000000, 3, MidpointRounding.AwayFromZero);
                    CountEllipseGrid += dd;
                }
                decimal Weight = GetInsetWeight(EGRows[0]);
                Weight = Decimal.Round(CountEllipseGrid * Weight, 3, MidpointRounding.AwayFromZero);

                //decimal Cost = Math.Ceiling(CountEllipseGrid * PriceEllipseGrid / 0.01m) * 0.01m;
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["TotalDiscount"] = TotalDiscount;
                NewRow["Name"] = "Решетка овал";
                NewRow["Count"] = CountEllipseGrid;
                NewRow["Measure"] = "м.кв.";
                NewRow["OriginalPrice"] = PriceEllipseGrid;
                NewRow["Price"] = PriceEllipseGrid;
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountEllipseGrid * PriceEllipseGrid / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["PriceWithTransport"] = PriceEllipseGrid;
                NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountEllipseGrid * PriceEllipseGrid / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["Weight"] = Weight;
                ReportDataTable.Rows.Add(NewRow);
            }

            DataRow[] A1RRows = OrdersDataTable.Select("InsetTypeID = 3654 OR FrontID IN (3731,3740,3746)");//applic 1 right

            if (A1RRows.Count() > 0)
            {
                PriceApplic1R = Convert.ToDecimal(A1RRows[0]["InsetPrice"]);

                for (int i = 0; i < A1RRows.Count(); i++)
                {
                    TotalDiscount = Convert.ToDecimal(A1RRows[i]["TotalDiscount"]);
                    CountApplic1R += Convert.ToInt32(A1RRows[i]["Count"]);
                }

                //decimal Cost = Math.Ceiling(CountApplic1R * PriceApplic1R / 0.01m) * 0.01m;
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["TotalDiscount"] = TotalDiscount;
                NewRow["Name"] = "Апплик. №1 правая";
                NewRow["Count"] = CountApplic1R;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic1R;
                NewRow["OriginalPrice"] = PriceApplic1R;
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountApplic1R * PriceApplic1R / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["PriceWithTransport"] = PriceApplic1R;
                NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountApplic1R * PriceApplic1R / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }

            DataRow[] A1LRows = OrdersDataTable.Select("InsetTypeID = 3653 OR FrontID IN (3728,3739,3745)");//applic 1 left

            if (A1LRows.Count() > 0)
            {
                PriceApplic1L = Convert.ToDecimal(A1LRows[0]["InsetPrice"]);

                for (int i = 0; i < A1LRows.Count(); i++)
                {
                    TotalDiscount = Convert.ToDecimal(A1LRows[i]["TotalDiscount"]);
                    CountApplic1L += Convert.ToInt32(A1LRows[i]["Count"]);
                }

                //decimal Cost = Math.Ceiling(CountApplic1L * PriceApplic1L / 0.01m) * 0.01m;
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["TotalDiscount"] = TotalDiscount;
                NewRow["Name"] = "Апплик. №1 левая";
                NewRow["Count"] = CountApplic1L;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic1L;
                NewRow["OriginalPrice"] = PriceApplic1L;
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountApplic1L * PriceApplic1L / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["PriceWithTransport"] = PriceApplic1L;
                NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountApplic1L * PriceApplic1L / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }

            DataRow[] A2Rows = OrdersDataTable.Select("InsetTypeID = 3655 OR FrontID IN (3732,3741,3744)");//applic 2 

            if (A2Rows.Count() > 0)
            {
                PriceApplic2 = Convert.ToDecimal(A2Rows[0]["InsetPrice"]);

                for (int i = 0; i < A2Rows.Count(); i++)
                {
                    TotalDiscount = Convert.ToDecimal(A2Rows[i]["TotalDiscount"]);
                    CountApplic2 += Convert.ToInt32(A2Rows[i]["Count"]);
                }

                //decimal Cost = Math.Ceiling(CountApplic2 * PriceApplic2 / 0.01m) * 0.01m;
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["Name"] = "Апплик. №2";
                NewRow["Count"] = CountApplic2;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic2;
                NewRow["OriginalPrice"] = PriceApplic2;
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountApplic2 * PriceApplic2 / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["PriceWithTransport"] = PriceApplic2;
                NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountApplic2 * PriceApplic2 / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }

            DataRow[] A3Rows = OrdersDataTable.Select("InsetTypeID = 28961");//applic 3 

            if (A3Rows.Count() > 0)
            {
                PriceApplic2 = Convert.ToDecimal(A3Rows[0]["InsetPrice"]);

                for (int i = 0; i < A3Rows.Count(); i++)
                {
                    TotalDiscount = Convert.ToDecimal(A3Rows[i]["TotalDiscount"]);
                    CountApplic2 += Convert.ToInt32(A3Rows[i]["Count"]);
                }

                //decimal Cost = Math.Ceiling(CountApplic2 * PriceApplic2 / 0.01m) * 0.01m;
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["Name"] = "Апплик. №3";
                NewRow["Count"] = CountApplic2;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic2;
                NewRow["OriginalPrice"] = PriceApplic2;
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountApplic2 * PriceApplic2 / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["PriceWithTransport"] = PriceApplic2;
                NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountApplic2 * PriceApplic2 / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }
        }

        private decimal GetInsetWeight(DataRow FrontsOrdersRow)
        {
            int InsetTypeID = Convert.ToInt32(FrontsOrdersRow["InsetTypeID"]);
            decimal InsetSquare = GetInsetSquare(FrontsOrdersRow);
            if (InsetSquare <= 0)
                return 0;
            decimal InsetWeight = 0;
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + InsetTypeID);
            if (Rows.Count() > 0)
            {
                if (Rows[0]["Weight"] != DBNull.Value)
                    InsetWeight = Convert.ToDecimal(Rows[0]["Weight"]);
            }

            return InsetSquare * InsetWeight;
        }

        private decimal GetProfileWeight(DataRow FrontsOrdersRow)
        {
            decimal FrontHeight = Convert.ToDecimal(FrontsOrdersRow["Height"]);
            decimal FrontWidth = Convert.ToDecimal(FrontsOrdersRow["Width"]);
            decimal ProfileWeight = 0;
            decimal ProfileWidth = 0;
            DataRow[] FrontsConfigRow = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString());
            if (FrontsConfigRow.Count() > 0)
                ProfileWeight = Convert.ToDecimal(FrontsConfigRow[0]["Weight"]);

            //для Женевы и Тафеля глухой - вес квадрата профиля на площадь фасада
            int FrontID = Convert.ToInt32(FrontsOrdersRow["FrontID"]);
            if (FrontID == 30504 || FrontID == 30505 || FrontID == 30506 ||
                FrontID == 30364 || FrontID == 30366 || FrontID == 30367 ||
                FrontID == 30501 || FrontID == 30502 || FrontID == 30503 ||
                FrontID == 16269 || FrontID == 28945 || FrontID == 27914 || FrontID == 29597 || FrontID == 3727 || FrontID == 3728 || FrontID == 3729 ||
                FrontID == 3730 || FrontID == 3731 || FrontID == 3732 || FrontID == 3733 || FrontID == 3734 ||
                FrontID == 3735 || FrontID == 3736 || FrontID == 3737 || FrontID == 3739 || FrontID == 3740 ||
                FrontID == 3741 || FrontID == 3742 || FrontID == 3743 || FrontID == 3744 || FrontID == 3745 ||
                FrontID == 3746 || FrontID == 3747 || FrontID == 3748 || FrontID == 15108 || FrontID == 3662 || FrontID == 3663 || FrontID == 3664 || FrontID == 15760)
                return FrontWidth * FrontHeight / 1000000 * ProfileWeight;
            else
            {
                DataRow[] DecorConfigRow = TechStoreDataTable.Select("TechStoreID = " + FrontsConfigRow[0]["ProfileID"].ToString());
                if (DecorConfigRow.Count() > 0)
                {
                    ProfileWidth = Convert.ToDecimal(DecorConfigRow[0]["Width"]);
                    ProfileWeight = Convert.ToDecimal(DecorConfigRow[0]["Weight"]);
                    return (FrontWidth * 2 + (FrontHeight - ProfileWidth - ProfileWidth) * 2) / 1000 * ProfileWeight;
                }
            }
            return 0;
        }

        public void GetFrontWeight(DataRow FrontsOrdersRow, ref decimal outFrontWeight, ref decimal outInsetWeight)
        {
            //decimal FrontsWeight = 0;
            DataRow[] FrontsConfigRow = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString());
            decimal InsetWeight = Convert.ToDecimal(FrontsConfigRow[0]["InsetWeight"]);
            decimal FrontsOrderSquare = Convert.ToDecimal(FrontsOrdersRow["Square"]);
            decimal PackWeight = 0;
            if (FrontsOrderSquare > 0)
                PackWeight = FrontsOrderSquare * Convert.ToDecimal(0.7);
            //если гнутый то вес за штуки
            if (FrontsConfigRow[0]["Width"].ToString() == "-1")
            {
                outFrontWeight = PackWeight +
                    Convert.ToDecimal(FrontsOrdersRow["Count"]) * Convert.ToDecimal(FrontsConfigRow[0]["Weight"]);
                return;
            }
            //если алюминий
            if (FC.IsAluminium(FrontsOrdersRow) > -1)
            {
                outFrontWeight = PackWeight +
                    FC.GetAluminiumWeight(FrontsOrdersRow, true);
                return;
            }
            decimal ResultProfileWeight = GetProfileWeight(FrontsOrdersRow);
            decimal ResultInsetWeight = 0;
            DataRow[] rows = InsetTypesDataTable.Select("GroupID = 3 OR GroupID = 4 OR GroupID = 7 OR GroupID = 12 OR GroupID = 13");
            foreach (DataRow item in rows)
            {
                if (FrontsOrdersRow["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                {
                    ResultInsetWeight = GetInsetWeight(FrontsOrdersRow);
                }
            }

            outFrontWeight = PackWeight + ResultProfileWeight * Convert.ToDecimal(FrontsOrdersRow["Count"]);
            outInsetWeight = ResultInsetWeight * Convert.ToDecimal(FrontsOrdersRow["Count"]);
        }

        public void Report(int[] MainOrderIDs, DataTable InfoDT, bool IsSample)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();

            string SelectCommand = @"SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, 
                (-NewMegaOrders.ComplaintProfilCost-NewMegaOrders.ComplaintTPSCost+NewMegaOrders.TransportCost+NewMegaOrders.AdditionalCost) AS TotalAdditionalCost,
                NewMegaOrders.ComplaintProfilCost, NewMegaOrders.ComplaintTPSCost, NewMegaOrders.TransportCost, NewMegaOrders.AdditionalCost, NewMegaOrders.MegaOrderID, NewMegaOrders.PaymentRate FROM NewFrontsOrders
                INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE NewFrontsOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") AND NewFrontsOrders.IsSample = 1 ORDER BY FrontID";
            if (!IsSample)
                SelectCommand = @"SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, 
                (-NewMegaOrders.ComplaintProfilCost-NewMegaOrders.ComplaintTPSCost+NewMegaOrders.TransportCost+NewMegaOrders.AdditionalCost) AS TotalAdditionalCost,
                NewMegaOrders.ComplaintProfilCost, NewMegaOrders.ComplaintTPSCost, NewMegaOrders.TransportCost, NewMegaOrders.AdditionalCost, NewMegaOrders.MegaOrderID, NewMegaOrders.PaymentRate FROM NewFrontsOrders
                INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE NewFrontsOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") AND NewFrontsOrders.IsSample = 0 ORDER BY FrontID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    DataTable DistRatesDT = new DataTable();
                    DataTable DT1 = DT.Clone();
                    //get count of different covertypes
                    using (DataView DV = new DataView(DT))
                    {
                        DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
                    }

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);


                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                        {
                            DT1.Clear();
                            PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                            DataRow[] rows = ProfilFrontsOrdersDataTable.Select("PaymentRate='" + PaymentRate.ToString() + "'");
                            foreach (DataRow item in rows)
                                DT1.Rows.Add(item.ItemArray);
                            if (DT1.Rows.Count == 0)
                                continue;

                            GetSimpleFronts(DT1, ProfilReportDataTable, false);
                            GetSimpleFronts(DT1, ProfilReportDataTable, true);

                            GetCurvedFronts(DT1, ProfilReportDataTable);

                            GetGrids(DT1, ProfilReportDataTable);

                            GetInsets(DT1, ProfilReportDataTable);

                            GetGlass(DT1, ProfilReportDataTable);
                        }
                    }

                    DT1.Clear();
                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                        {
                            DT1.Clear();
                            PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                            DataRow[] rows = TPSFrontsOrdersDataTable.Select("PaymentRate='" + PaymentRate.ToString() + "'");
                            foreach (DataRow item in rows)
                                DT1.Rows.Add(item.ItemArray);
                            if (DT1.Rows.Count == 0)
                                continue;

                            GetSimpleFronts(DT1, TPSReportDataTable, false);
                            GetSimpleFronts(DT1, TPSReportDataTable, true);

                            GetCurvedFronts(DT1, TPSReportDataTable);

                            GetGrids(DT1, TPSReportDataTable);

                            GetInsets(DT1, TPSReportDataTable);

                            GetGlass(DT1, TPSReportDataTable);
                        }
                    }
                }
            }

        }

        public void Report(int[] MainOrderIDs, DataTable InfoDT)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();

            string SelectCommand = @"SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, 
                (-NewMegaOrders.ComplaintProfilCost-NewMegaOrders.ComplaintTPSCost+NewMegaOrders.TransportCost+NewMegaOrders.AdditionalCost) AS TotalAdditionalCost,
                NewMegaOrders.ComplaintProfilCost, NewMegaOrders.ComplaintTPSCost, NewMegaOrders.TransportCost, NewMegaOrders.AdditionalCost, NewMegaOrders.MegaOrderID, NewMegaOrders.PaymentRate FROM NewFrontsOrders
                INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE NewFrontsOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY FrontID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    DataTable DistRatesDT = new DataTable();
                    DataTable DT1 = DT.Clone();
                    //get count of different covertypes
                    using (DataView DV = new DataView(DT))
                    {
                        DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
                    }

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);


                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                        {
                            DT1.Clear();
                            PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                            DataRow[] rows = ProfilFrontsOrdersDataTable.Select("PaymentRate='" + PaymentRate.ToString() + "'");
                            foreach (DataRow item in rows)
                                DT1.Rows.Add(item.ItemArray);
                            if (DT1.Rows.Count == 0)
                                continue;

                            GetSimpleFronts(DT1, ProfilReportDataTable, false);
                            GetSimpleFronts(DT1, ProfilReportDataTable, true);

                            GetCurvedFronts(DT1, ProfilReportDataTable);

                            GetGrids(DT1, ProfilReportDataTable);

                            GetInsets(DT1, ProfilReportDataTable);

                            GetGlass(DT1, ProfilReportDataTable);
                        }
                    }

                    DT1.Clear();
                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                        {
                            DT1.Clear();
                            PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                            DataRow[] rows = TPSFrontsOrdersDataTable.Select("PaymentRate='" + PaymentRate.ToString() + "'");
                            foreach (DataRow item in rows)
                                DT1.Rows.Add(item.ItemArray);
                            if (DT1.Rows.Count == 0)
                                continue;

                            GetSimpleFronts(DT1, TPSReportDataTable, false);
                            GetSimpleFronts(DT1, TPSReportDataTable, true);

                            GetCurvedFronts(DT1, TPSReportDataTable);

                            GetGrids(DT1, TPSReportDataTable);

                            GetInsets(DT1, TPSReportDataTable);

                            GetGlass(DT1, TPSReportDataTable);
                        }
                    }
                }
            }

        }
    }

    public class DecorReport
    {
        string ProfilCurrencyCode = "0";
        string TPSCurrencyCode = "0";
        decimal PaymentRate = 0;
        string UNN = string.Empty;
        int ClientID = 0;

        public DataTable CurrencyTypesDT = null;

        DecorCatalogOrder DecorCatalogOrder = null;

        public DataTable ProfilReportDataTable = null;
        public DataTable TPSReportDataTable = null;
        private DataTable DecorProductsDataTable = null;
        private DataTable DecorConfigDataTable = null;
        private DataTable MeasuresDataTable = null;
        private DataTable DecorOrdersDataTable = null;
        private DataTable CoverTypesDataTable = null;
        private DataTable DecorDataTable = null;

        public DecorReport(ref DecorCatalogOrder tDecorCatalogOrder)
        {
            DecorCatalogOrder = tDecorCatalogOrder;

            Create();
            CreateReportDataTable();

            CurrencyTypesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDT);
            }
        }

        private void Create()
        {
            DecorOrdersDataTable = new DataTable();

            string SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            MeasuresDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }

            DecorConfigDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorConfig",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDataTable);
            //}
            DecorConfigDataTable = TablesManager.DecorConfigDataTableAll;

            CoverTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CoverTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CoverTypesDataTable);
            }
        }

        private void CreateReportDataTable()
        {
            ProfilReportDataTable = new DataTable();
            ProfilReportDataTable.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("OriginalPrice", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("DiscountVolume", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("TotalDiscount", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            TPSReportDataTable = ProfilReportDataTable.Clone();
        }

        public void ClearReport()
        {
            DecorOrdersDataTable.Clear();

            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();

            ProfilReportDataTable.AcceptChanges();
            TPSReportDataTable.AcceptChanges();
        }

        private bool IsProfil(int DecorConfigID)
        {
            DataRow[] Rows = DecorConfigDataTable.Select("DecorConfigID = " + DecorConfigID.ToString());

            if (Rows[0]["FactoryID"].ToString() == "1")
                return true;

            return false;
        }

        public void GetMegaOrderInfo(int MainOrderID)
        {
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, Rate, ClientID, CurrencyTypeID FROM NewMegaOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM NewMainOrders WHERE MainOrderID = " + MainOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int CurrencyTypeID = 0;

                        if (DT.Rows[0]["ClientID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["ClientID"].ToString(), out ClientID);
                        if (DT.Rows[0]["CurrencyTypeID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["CurrencyTypeID"].ToString(), out CurrencyTypeID);

                        DataRow[] rows = CurrencyTypesDT.Select("CurrencyTypeID = " + CurrencyTypeID);
                        if (rows.Count() > 0)
                        {
                            ProfilCurrencyCode = rows[0]["CurrencyCode"].ToString();
                            TPSCurrencyCode = rows[0]["TPSCurrencyCode"].ToString();
                        }
                    }
                }
            }
            SelectCommand = "SELECT UNN FROM Clients WHERE ClientID = " + ClientID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["UNN"] != DBNull.Value)
                            UNN = DT.Rows[0]["UNN"].ToString();
                    }
                }
            }
        }

        private int GetReportMeasureTypeID(int DecorConfigID)
        {
            DataRow[] Row = DecorConfigDataTable.Select("DecorConfigID = " + DecorConfigID);

            return Convert.ToInt32(Row[0]["ReportMeasureID"]);//1 м.кв.  2 м.п. 3 шт.
        }

        private int GetMeasureTypeID(int DecorConfigID)
        {
            DataRow[] Row = DecorConfigDataTable.Select("DecorConfigID = " + DecorConfigID);

            return Convert.ToInt32(Row[0]["MeasureID"]);//1 м.кв.  2 м.п. 3 шт.
        }

        private string GetItemName(int DecorID)
        {
            DataRow[] Row = DecorDataTable.Select("DecorID = " + DecorID);

            return Row[0]["Name"].ToString();
        }

        private string GetProductName(int ProductID)
        {
            DataRow[] Row = DecorProductsDataTable.Select("ProductID = " + ProductID);

            return Row[0]["ProductName"].ToString();
        }

        public decimal GetDecorWeight(DataRow DecorOrderRow)
        {
            if (DecorOrderRow["Weight"] == DBNull.Value)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка конфигурации: нет веса в DecorConfigID=" + DecorOrderRow["DecorConfigID"].ToString() +
                    ". Вес будет выставлен в 0.");
                return 0;
            }
            decimal Weight = 0;

            DataRow[] Row = DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString());

            if (Row[0]["Weight"] == DBNull.Value)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка конфигурации: нет веса в DecorConfigID=" + DecorOrderRow["DecorConfigID"].ToString() +
                    ". Вес будет выставлен в 0.");
                return 0;
            }
            if (Row[0]["WeightMeasureID"].ToString() == "1")
            {
                if (Convert.ToDecimal(DecorOrderRow["Height"]) != -1)
                    Weight = Convert.ToDecimal(DecorOrderRow["Height"]) * Convert.ToDecimal(DecorOrderRow["Width"]) / 1000000
                         * Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);
                if (Convert.ToDecimal(DecorOrderRow["Length"]) != -1)
                    Weight = Convert.ToDecimal(DecorOrderRow["Length"]) * Convert.ToDecimal(DecorOrderRow["Width"]) / 1000000
                         * Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);
            }
            decimal L = 0;

            if (Row[0]["WeightMeasureID"].ToString() == "2")
            {

                L = 0;

                L = Convert.ToDecimal(DecorOrderRow["Length"]);

                if (L == -1)
                    L = Convert.ToDecimal(DecorOrderRow["Height"]);

                Weight = Convert.ToDecimal(L) / 1000 * Convert.ToDecimal(Row[0]["Weight"]) *
                         Convert.ToDecimal(DecorOrderRow["Count"]);

            }
            if (Row[0]["WeightMeasureID"].ToString() == "3")
                Weight = Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);

            return Weight;
        }

        public void CreateParamsTable(string Params, DataTable DT)
        {
            string Param = null;

            for (int i = 0; i < Params.Length; i++)
            {
                if (Params[i] != ';')
                    Param += Params[i];

                if (Params[i] == ';' || i == Params.Length - 1)
                {
                    if (Param.Length > 0)
                    {
                        DT.Columns.Add(new DataColumn(Param, Type.GetType("System.Int32")));
                        Param = "";
                    }
                }
            }

            DT.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("OriginalPrice", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("DiscountVolume", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("TotalDiscount", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("ProductID", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("DecorID", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("TotalCount", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
        }

        private void GroupCoverTypes(DataRow[] Rows, int MeasureTypeID, int DecorID)
        {
            DataTable PDT = new DataTable();
            DataTable TDT = new DataTable();

            PDT = DecorOrdersDataTable.Clone();
            TDT = DecorOrdersDataTable.Clone();

            PDT.Columns.Remove("Count");
            PDT.Columns.Add(new DataColumn("TotalCount", Type.GetType("System.Int32")));
            PDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            PDT.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            PDT.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            PDT.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));

            TDT.Columns.Remove("Count");
            TDT.Columns.Add(new DataColumn("TotalCount", Type.GetType("System.Int32")));
            TDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            TDT.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            TDT.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            TDT.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));


            for (int r = 0; r < Rows.Count(); r++)
            {
                string InvNumber = Rows[r]["InvNumber"].ToString();
                //м.п.
                if (MeasureTypeID == 2)
                {
                    decimal L = 0;

                    L = Convert.ToDecimal(Rows[r]["Length"]);

                    if (L == -1)
                        L = Convert.ToDecimal(Rows[r]["Height"]);

                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        if (PDT.Rows.Count == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            PDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(PDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            PDT.Rows[0]["Price"] = (Convert.ToDecimal(PDT.Rows[0]["Price"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            PDT.Rows[0]["TotalCount"] = Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                                      Convert.ToDecimal(Rows[r]["Count"]) * L;

                            PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) + Convert.ToDecimal(Rows[r]["Cost"]);
                            PDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(PDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            PDT.Rows[0]["Weight"] = Convert.ToDecimal(PDT.Rows[0]["Weight"]) +
                                                         GetDecorWeight(Rows[r]);

                            continue;
                        }

                    }
                    else
                    {
                        if (TDT.Rows.Count == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            TDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(TDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            TDT.Rows[0]["Price"] = (Convert.ToDecimal(TDT.Rows[0]["Price"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            TDT.Rows[0]["TotalCount"] = Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                                      Convert.ToDecimal(Rows[r]["Count"]) * L;

                            TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) + Convert.ToDecimal(Rows[r]["Cost"]);
                            TDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(TDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            TDT.Rows[0]["Weight"] = Convert.ToDecimal(TDT.Rows[0]["Weight"]) +
                                                         GetDecorWeight(Rows[r]);

                            continue;
                        }
                    }
                }

                //шт.
                if (MeasureTypeID == 3)
                {
                    //get_parametrized_data function only
                }

                //м.кв.
                if (MeasureTypeID == 1)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        if (PDT.Rows.Count == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;

                                L = Convert.ToDecimal(Rows[r]["Length"]);

                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);
                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 3)
                            {

                                decimal H = 0;

                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                                NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            }

                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else     //if no color parameter (hands e.g.)
                        {
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) + Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) + Square;
                                }
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;

                                L = Convert.ToDecimal(Rows[r]["Length"]);

                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);

                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) + Square;
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 3)
                            {

                                decimal H = 0;

                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) + Square;
                            }

                            PDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(PDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            PDT.Rows[0]["Price"] = (Convert.ToDecimal(PDT.Rows[0]["Price"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            PDT.Rows[0]["TotalCount"] = Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);

                            PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) + Convert.ToDecimal(Rows[r]["Cost"]);
                            PDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(PDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            PDT.Rows[0]["Weight"] = Convert.ToDecimal(PDT.Rows[0]["Weight"]) +
                                                         GetDecorWeight(Rows[r]);

                            continue;
                        }
                    }
                    else
                    {
                        if (TDT.Rows.Count == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;

                                L = Convert.ToDecimal(Rows[r]["Length"]);

                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);
                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 3)
                            {

                                decimal H = 0;

                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;

                            }

                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else //if no color parameter (hands e.g.)
                        {
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) + Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) + Square;
                                }
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;

                                L = Convert.ToDecimal(Rows[r]["Length"]);

                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);
                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) + Square;
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 3)
                            {

                                decimal H = 0;

                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);

                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) + Square;
                            }
                            TDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(TDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            TDT.Rows[0]["Price"] = (Convert.ToDecimal(TDT.Rows[0]["Price"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            TDT.Rows[0]["TotalCount"] = Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);

                            TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) + Convert.ToDecimal(Rows[r]["Cost"]);
                            TDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(TDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            TDT.Rows[0]["Weight"] = Convert.ToDecimal(TDT.Rows[0]["Weight"]) +
                                                         GetDecorWeight(Rows[r]);

                            continue;
                        }
                    }
                }
            }




            //REPORT TABLE
            //м.п.
            if (MeasureTypeID == 2)
            {
                if (PDT.Rows.Count > 0)
                {
                    for (int i = 0; i < PDT.Rows.Count; i++)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[i]["ProductID"])) + " " +
                            GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.п.";
                        NewRow["OriginalPrice"] = Convert.ToDecimal(PDT.Rows[i]["OriginalPrice"]);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(PDT.Rows[i]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(PDT.Rows[i]["TotalDiscount"]);
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]) / (Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]) / (Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[i]["ProductID"])) + " " +
                            GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.п.";
                        NewRow["OriginalPrice"] = Convert.ToDecimal(TDT.Rows[i]["OriginalPrice"]);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]) / (Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(TDT.Rows[i]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(TDT.Rows[i]["TotalDiscount"]);
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]) / (Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            //шт.
            if (MeasureTypeID == 3)
            {
                //get_parametrized_data function only
            }

            //м.кв.
            if (MeasureTypeID == 1)
            {
                if (PDT.Rows.Count > 0)
                {
                    for (int i = 0; i < PDT.Rows.Count; i++)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (PDT.Rows[i]["ProductID"].ToString() == "10" || PDT.Rows[i]["ProductID"].ToString() == "11" ||
                            PDT.Rows[i]["ProductID"].ToString() == "12")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[i]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[i]["ProductID"])) + " " +
                                             GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.кв.";
                        NewRow["OriginalPrice"] = Convert.ToDecimal(PDT.Rows[i]["OriginalPrice"]);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(PDT.Rows[i]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(PDT.Rows[i]["TotalDiscount"]);
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]) / Convert.ToDecimal(PDT.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        if (TDT.Rows[i]["ProductID"].ToString() == "10" || TDT.Rows[i]["ProductID"].ToString() == "11" ||
                            TDT.Rows[i]["ProductID"].ToString() == "12")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[i]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[i]["ProductID"])) + " " +
                                             GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.кв.";
                        NewRow["OriginalPrice"] = Convert.ToDecimal(TDT.Rows[i]["OriginalPrice"]);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(TDT.Rows[i]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(TDT.Rows[i]["TotalDiscount"]);
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]) / Convert.ToDecimal(TDT.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            PDT.Dispose();
            TDT.Dispose();
        }

        private void GetParametrizedData(DataRow[] Rows, DataTable PDT, DataTable TDT, int DecorID)
        {
            string p1 = "";
            string p2 = "";
            string p3 = "";

            if (PDT.Columns["Height"] != null)
                p1 = "Height";

            if (PDT.Columns["Length"] != null)
                p1 = "Length";

            if (PDT.Columns["Width"] != null)
                p2 = "Width";

            if (p1.Length > 0 && p2.Length == 0)
                p3 = p1;

            if (p1.Length == 0 && p2.Length > 0)
                p3 = p2;



            if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
            {
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        if (PDT.Rows.Count == 0)
                        {
                            DataRow NewRow = PDT.NewRow();

                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            PDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(PDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            PDT.Rows[0]["Price"] = (Convert.ToDecimal(PDT.Rows[0]["Price"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            PDT.Rows[0]["TotalCount"] = Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                            Convert.ToDecimal(Rows[r]["Count"]);
                            PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
                            PDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(PDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            PDT.Rows[0]["Weight"] = Convert.ToDecimal(PDT.Rows[0]["Weight"]) +
                                                            GetDecorWeight(Rows[r]);
                            continue;
                        }
                    }
                    else
                    {
                        if (TDT.Rows.Count == 0)
                        {
                            DataRow NewRow = TDT.NewRow();

                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            TDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(TDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            TDT.Rows[0]["Price"] = (Convert.ToDecimal(TDT.Rows[0]["Price"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                            TDT.Rows[0]["TotalCount"] = Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                            Convert.ToDecimal(Rows[r]["Count"]);
                            TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
                            TDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(TDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            TDT.Rows[0]["Weight"] = Convert.ToDecimal(TDT.Rows[0]["Weight"]) +
                                                            GetDecorWeight(Rows[r]);
                            continue;
                        }
                    }
                }

            }



            if (p1.Length > 0 && p2.Length > 0)
            {
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        if (PDT.Rows.Count == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow[p1] = Convert.ToDecimal(Rows[r][p1]);
                            NewRow[p2] = Convert.ToDecimal(Rows[r][p2]);

                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(PDT.Rows[0][p1]) == Convert.ToInt32(Rows[r][p1]) &&
                                        Convert.ToInt32(PDT.Rows[0][p2]) == Convert.ToInt32(Rows[r][p2]))
                            {
                                PDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(PDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                    (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                PDT.Rows[0]["Price"] = (Convert.ToDecimal(PDT.Rows[0]["Price"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                    (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                PDT.Rows[0]["TotalCount"] = Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) +
                                    Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
                                PDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(PDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                                PDT.Rows[0]["Weight"] = Convert.ToDecimal(PDT.Rows[0]["Weight"]) +
                                                             GetDecorWeight(Rows[r]);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (TDT.Rows.Count == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow[p1] = Convert.ToDecimal(Rows[r][p1]);
                            NewRow[p2] = Convert.ToDecimal(Rows[r][p2]);
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(TDT.Rows[0][p1]) == Convert.ToInt32(Rows[r][p1]) &&
                                        Convert.ToInt32(TDT.Rows[0][p2]) == Convert.ToInt32(Rows[r][p2]))
                            {
                                TDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(TDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                    (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                TDT.Rows[0]["Price"] = (Convert.ToDecimal(TDT.Rows[0]["Price"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                    (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                TDT.Rows[0]["TotalCount"] = Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) +
                                    Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
                                TDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(TDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                                TDT.Rows[0]["Weight"] = Convert.ToDecimal(TDT.Rows[0]["Weight"]) +
                                                             GetDecorWeight(Rows[r]);
                                continue;
                            }
                        }
                    }
                }
            }

            if (p3.Length > 0)
            {
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        if (PDT.Rows.Count == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow[p3] = Convert.ToDecimal(Rows[r][p3]);
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(PDT.Rows[0][p3]) == Convert.ToInt32(Rows[r][p3]))
                            {
                                PDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(PDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                    (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                PDT.Rows[0]["Price"] = (Convert.ToDecimal(PDT.Rows[0]["Price"]) * Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                    (Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                PDT.Rows[0]["TotalCount"] = Convert.ToDecimal(PDT.Rows[0]["TotalCount"]) +
                                    Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
                                PDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(PDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                                PDT.Rows[0]["Weight"] = Convert.ToDecimal(PDT.Rows[0]["Weight"]) +
                                                             GetDecorWeight(Rows[r]);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (TDT.Rows.Count == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow[p3] = Convert.ToDecimal(Rows[r][p3]);
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["OriginalPrice"] = Convert.ToDecimal(Rows[r]["OriginalPrice"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["DiscountVolume"] = Convert.ToDecimal(Rows[r]["DiscountVolume"]);
                            NewRow["TotalCount"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["TotalDiscount"] = Convert.ToDecimal(Rows[r]["TotalDiscount"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(TDT.Rows[0][p3]) == Convert.ToInt32(Rows[r][p3]))
                            {
                                TDT.Rows[0]["OriginalPrice"] = (Convert.ToDecimal(TDT.Rows[0]["OriginalPrice"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["OriginalPrice"])) /
                                    (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                TDT.Rows[0]["Price"] = (Convert.ToDecimal(TDT.Rows[0]["Price"]) * Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]) * Convert.ToDecimal(Rows[r]["Price"])) /
                                    (Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) + Convert.ToDecimal(Rows[r]["Count"]));
                                TDT.Rows[0]["TotalCount"] = Convert.ToDecimal(TDT.Rows[0]["TotalCount"]) +
                                    Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
                                TDT.Rows[0]["CostWithTransport"] = Convert.ToDecimal(TDT.Rows[0]["CostWithTransport"]) + Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                                TDT.Rows[0]["Weight"] = Convert.ToDecimal(TDT.Rows[0]["Weight"]) +
                                                             GetDecorWeight(Rows[r]);
                                continue;
                            }
                        }
                    }
                }
            }





            //REPORT TABLES
            if (PDT.Rows.Count > 0)
            {
                for (int g = 0; g < PDT.Rows.Count; g++)
                {
                    if (p1.Length > 0 && p2.Length > 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (PDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + PDT.Rows[g][p1] + "x" + PDT.Rows[g][p2];

                        NewRow["OriginalPrice"] = Convert.ToDecimal(PDT.Rows[g]["OriginalPrice"]);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(PDT.Rows[g]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(PDT.Rows[g]["TotalDiscount"]);
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();



                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (PDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + PDT.Rows[g][p3];

                        NewRow["OriginalPrice"] = Convert.ToDecimal(PDT.Rows[g]["OriginalPrice"]);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(PDT.Rows[g]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(PDT.Rows[g]["TotalDiscount"]);
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (PDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID));

                        NewRow["OriginalPrice"] = Convert.ToDecimal(PDT.Rows[g]["OriginalPrice"]);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(PDT.Rows[g]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(PDT.Rows[g]["TotalDiscount"]);
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            if (TDT.Rows.Count > 0)
            {
                for (int g = 0; g < TDT.Rows.Count; g++)
                {
                    if (p1.Length > 0 && p2.Length > 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        if (TDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + TDT.Rows[g][p1] + "x" + TDT.Rows[g][p2];

                        NewRow["OriginalPrice"] = Convert.ToDecimal(TDT.Rows[g]["OriginalPrice"]);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(TDT.Rows[g]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(TDT.Rows[g]["TotalDiscount"]);
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        if (TDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + TDT.Rows[g][p3];

                        NewRow["OriginalPrice"] = Convert.ToDecimal(TDT.Rows[g]["OriginalPrice"]);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(TDT.Rows[g]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(TDT.Rows[g]["TotalDiscount"]);
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        if (TDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID));

                        NewRow["OriginalPrice"] = Convert.ToDecimal(TDT.Rows[g]["OriginalPrice"]);
                        NewRow["DiscountVolume"] = Convert.ToDecimal(TDT.Rows[g]["DiscountVolume"]);
                        NewRow["TotalDiscount"] = Convert.ToDecimal(TDT.Rows[g]["TotalDiscount"]);
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }


        }

        private void Collect()
        {
            DataTable DistRatesDT = new DataTable();
            DataTable Items = new DataTable();

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Items = DV.ToTable(true, new string[] { "DecorID" });
            }

            //get count of different covertypes
            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
            }

            for (int i = 0; i < Items.Rows.Count; i++)
            {
                int rr = Convert.ToInt32(Items.Rows[i]["DecorID"]);

                for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                {
                    PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                    DataRow[] ItemsRows = DecorOrdersDataTable.Select("PaymentRate='" + PaymentRate.ToString() + "' AND DecorID = " + Items.Rows[i]["DecorID"].ToString(),
                                                                          "Price ASC");
                    if (ItemsRows.Count() == 0)
                        continue;
                    int DecorConfigID = Convert.ToInt32(ItemsRows[0]["DecorConfigID"]);
                    //м.п.
                    if (GetReportMeasureTypeID(Convert.ToInt32(ItemsRows[0]["DecorConfigID"])) == 2)
                    {
                        GroupCoverTypes(ItemsRows, 2, Convert.ToInt32(Items.Rows[i]["DecorID"]));
                    }


                    //шт.
                    if (GetReportMeasureTypeID(Convert.ToInt32(ItemsRows[0]["DecorConfigID"])) == 3)
                    {
                        DataTable ParamTableProfil = new DataTable();
                        DataTable ParamTableTPS = new DataTable();

                        DataRow[] DCs = DecorConfigDataTable.Select("DecorConfigID = " +
                                                                                        ItemsRows[0]["DecorConfigID"].ToString());

                        CreateParamsTable(DCs[0]["ReportParam"].ToString(), ParamTableProfil);
                        CreateParamsTable(DCs[0]["ReportParam"].ToString(), ParamTableTPS);

                        GetParametrizedData(ItemsRows, ParamTableProfil, ParamTableTPS, Convert.ToInt32(Items.Rows[i]["DecorID"]));

                        ParamTableProfil.Dispose();
                        ParamTableTPS.Dispose();
                    }


                    //м.кв.
                    if (GetReportMeasureTypeID(Convert.ToInt32(ItemsRows[0]["DecorConfigID"])) == 1)
                    {
                        GroupCoverTypes(ItemsRows, 1, Convert.ToInt32(Items.Rows[i]["DecorID"]));
                    }
                }
            }

            Items.Dispose();
        }

        public void Report(int[] MainOrderIDs, bool IsSample)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);

            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();
            DecorOrdersDataTable.Clear();
            string SelectCommand = @"SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders
                INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                WHERE NewDecorOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") AND NewDecorOrders.IsSample=1 ORDER BY DecorID";
            if (!IsSample)
                SelectCommand = @"SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders
                INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                WHERE NewDecorOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") AND NewDecorOrders.IsSample=0 ORDER BY DecorID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            Collect();

        }

        public void Report(int[] MainOrderIDs)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);

            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();
            DecorOrdersDataTable.Clear();
            string SelectCommand = @"SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders
                INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                WHERE NewDecorOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY DecorID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            Collect();

        }

    }

    public class Report
    {
        //string ReportFilePath = string.Empty;
        //HSSFWorkbook hssfworkbook;
        decimal VAT = 1.0m;
        public FrontsReport FrontsReport = null;
        public DecorReport DecorReport = null;

        public DataTable CurrencyTypesDataTable = null;
        public DataTable ProfilReportTable = null;
        public DataTable TPSReportTable = null;

        public Report(ref DecorCatalogOrder DecorCatalogOrder, ref FrontsCalculate FC)
        {
            FrontsReport = new FrontsReport(ref FC);
            DecorReport = new DecorReport(ref DecorCatalogOrder);

            CreateProfilReportTable();

            CurrencyTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }

            //ReadReportFilePath("MarketingClientReportPath.config");

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

        }

        private void CreateProfilReportTable()
        {
            ProfilReportTable = new DataTable();

            ProfilReportTable.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("OriginalPrice", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("DiscountVolume", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("TotalDiscount", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("IsNonStandard", Type.GetType("System.Boolean")));
            ProfilReportTable.Columns.Add(new DataColumn("NonStandardMargin", Type.GetType("System.Decimal")));
            TPSReportTable = ProfilReportTable.Clone();
        }

        public void ConvertToCurrency(int CurrencyTypeID, bool IsNonStandard)
        {
            decimal Cost = 0;
            decimal Count = 0;
            decimal OriginalPrice = 0;
            decimal PriceWithTransport = 0;
            decimal CostWithTransport = 0;
            decimal Price = 0;
            decimal PaymentRate = 0;
            int DecCount = 2;
            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                if (IsNonStandard)
                {
                    if (ProfilReportTable.Rows[i]["IsNonStandard"] == DBNull.Value ||
                        (ProfilReportTable.Rows[i]["IsNonStandard"] != DBNull.Value && !Convert.ToBoolean(ProfilReportTable.Rows[i]["IsNonStandard"])))
                        continue;
                }
                if (!IsNonStandard)
                {
                    if (ProfilReportTable.Rows[i]["IsNonStandard"] != DBNull.Value && Convert.ToBoolean(ProfilReportTable.Rows[i]["IsNonStandard"]))
                        continue;
                }

                PaymentRate = Convert.ToDecimal(ProfilReportTable.Rows[i]["PaymentRate"]);
                Count = Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                Cost = Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]) * PaymentRate / VAT;
                if (ProfilReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                    OriginalPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["OriginalPrice"]) * PaymentRate / VAT;
                if (ProfilReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                    PriceWithTransport = Convert.ToDecimal(ProfilReportTable.Rows[i]["PriceWithTransport"]) * PaymentRate / VAT;
                if (ProfilReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                    CostWithTransport = Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]) * PaymentRate / VAT;
                Cost = Math.Ceiling(Cost / 0.01m) * 0.01m;
                CostWithTransport = Math.Ceiling(CostWithTransport / 0.01m) * 0.01m;
                Price = Cost / Count;
                PriceWithTransport = CostWithTransport / Count;
                //if (CurrencyTypeID == 4)
                //{
                //    DecCount = 0;
                //    if (OriginalPrice != 0)
                //        OriginalPrice = Math.Ceiling(OriginalPrice / 100.0m) * 100.0m;
                //    if (CostWithTransport != 0)
                //        CostWithTransport = Math.Ceiling(CostWithTransport / 100.0m) * 100.0m;
                //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                //    Cost = Price * Count;
                //    PriceWithTransport = Math.Ceiling(PriceWithTransport / 100.0m) * 100.0m;
                //    CostWithTransport = PriceWithTransport * Count;
                //}
                if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                {
                    decimal ExtraPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["NonStandardMargin"]);
                    Price = Price / (ExtraPrice / 100 + 1);
                    PriceWithTransport = PriceWithTransport / (ExtraPrice / 100 + 1);
                    ProfilReportTable.Rows[i]["NonStandardMargin"] = Convert.ToDecimal(ProfilReportTable.Rows[i]["NonStandardMargin"]);
                }
                ProfilReportTable.Rows[i]["OriginalPrice"] = Decimal.Round(OriginalPrice, DecCount, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows[i]["PriceWithTransport"] = Decimal.Round(PriceWithTransport, DecCount, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows[i]["CostWithTransport"] = Decimal.Round(CostWithTransport, DecCount, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows[i]["Price"] = Decimal.Round(Price, DecCount, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows[i]["Cost"] = Decimal.Round(Cost, DecCount, MidpointRounding.AwayFromZero);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                if (IsNonStandard)
                {
                    if (TPSReportTable.Rows[i]["IsNonStandard"] == DBNull.Value ||
                        (TPSReportTable.Rows[i]["IsNonStandard"] != DBNull.Value && !Convert.ToBoolean(TPSReportTable.Rows[i]["IsNonStandard"])))
                        continue;
                }
                if (!IsNonStandard)
                {
                    if (TPSReportTable.Rows[i]["IsNonStandard"] != DBNull.Value && Convert.ToBoolean(TPSReportTable.Rows[i]["IsNonStandard"]))
                        continue;
                }

                PaymentRate = Convert.ToDecimal(TPSReportTable.Rows[i]["PaymentRate"]);
                Count = Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                Cost = Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]) * PaymentRate / VAT;
                if (TPSReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                    OriginalPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["OriginalPrice"]) * PaymentRate / VAT;
                if (TPSReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                    PriceWithTransport = Convert.ToDecimal(TPSReportTable.Rows[i]["PriceWithTransport"]) * PaymentRate / VAT;
                if (TPSReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                    CostWithTransport = Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]) * PaymentRate / VAT;
                Cost = Math.Ceiling(Cost / 0.01m) * 0.01m;
                CostWithTransport = Math.Ceiling(CostWithTransport / 0.01m) * 0.01m;
                Price = Cost / Count;
                PriceWithTransport = CostWithTransport / Count;
                //if (CurrencyTypeID == 4)
                //{
                //    DecCount = 0;
                //    if (OriginalPrice != 0)
                //        OriginalPrice = Math.Ceiling(OriginalPrice / 100.0m) * 100.0m;
                //    if (CostWithTransport != 0)
                //        CostWithTransport = Math.Ceiling(CostWithTransport / 100.0m) * 100.0m;
                //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                //    Cost = Price * Count;
                //    PriceWithTransport = Math.Ceiling(PriceWithTransport / 100.0m) * 100.0m;
                //    CostWithTransport = PriceWithTransport * Count;
                //}
                if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                {
                    decimal ExtraPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["NonStandardMargin"]);
                    Price = Price / (ExtraPrice / 100 + 1);
                    PriceWithTransport = PriceWithTransport / (ExtraPrice / 100 + 1);
                    TPSReportTable.Rows[i]["NonStandardMargin"] = ExtraPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["NonStandardMargin"]);
                }
                TPSReportTable.Rows[i]["OriginalPrice"] = Decimal.Round(OriginalPrice, DecCount, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows[i]["PriceWithTransport"] = Decimal.Round(PriceWithTransport, DecCount, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows[i]["CostWithTransport"] = Decimal.Round(CostWithTransport, DecCount, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows[i]["Price"] = Decimal.Round(Price, DecCount, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows[i]["Cost"] = Decimal.Round(Cost, DecCount, MidpointRounding.AwayFromZero);
            }
        }

        public void CollectGridsAndGlass()
        {
            decimal GridSquare = 0;
            decimal GridCost = 0;
            decimal GridCostWithTransport = 0;
            decimal GridWeight = 0;

            decimal LacomatSquare = 0;
            decimal LacomatCost = 0;
            decimal LacomatCostWithTransport = 0;
            decimal LacomatWeight = 0;
            decimal LacomatPrice = 0;
            decimal LacomatPriceWithTransport = 0;

            decimal KrizetSquare = 0;
            decimal KrizetCost = 0;
            decimal KrizetCostWithTransport = 0;
            decimal KrizetWeight = 0;
            decimal KrizetPrice = 0;
            decimal KrizetPriceWithTransport = 0;

            decimal FlutesSquare = 0;
            decimal FlutesCost = 0;
            decimal FlutesCostWithTransport = 0;
            decimal FlutesWeight = 0;
            decimal FlutesPrice = 0;
            decimal FlutesPriceWithTransport = 0;

            DataTable DistRatesDT = new DataTable();
            DataTable dt = ProfilReportTable.Clone();
            using (DataView DV = new DataView(ProfilReportTable))
            {
                DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
            }

            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
            {
                decimal PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                //collect

                GridSquare = 0;
                GridCost = 0;
                GridCostWithTransport = 0;
                GridWeight = 0;

                LacomatSquare = 0;
                LacomatCost = 0;
                LacomatCostWithTransport = 0;
                LacomatWeight = 0;
                LacomatPrice = 0;

                KrizetSquare = 0;
                KrizetCost = 0;
                KrizetCostWithTransport = 0;
                KrizetWeight = 0;
                KrizetPrice = 0;

                FlutesSquare = 0;
                FlutesCost = 0;
                FlutesCostWithTransport = 0;
                FlutesWeight = 0;
                FlutesPrice = 0;

                bool bb = false;
                for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                {
                    if (Convert.ToDecimal(ProfilReportTable.Rows[i]["PaymentRate"]) != PaymentRate)
                        continue;

                    bool b = false;
                    if (ProfilReportTable.Rows[i]["Name"].ToString().IndexOf("Решетка") > -1)
                    {
                        //GridCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                        //GridCostWithTransport += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                        GridSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                        GridWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);

                        GridCost += Math.Ceiling(Convert.ToDecimal(ProfilReportTable.Rows[i]["OriginalPrice"]) * Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]) / 0.001m) * 0.001m;
                        GridCostWithTransport += Math.Ceiling(Convert.ToDecimal(ProfilReportTable.Rows[i]["PriceWithTransport"]) * Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]) / 0.001m) * 0.001m;

                        b = true;
                    }

                    if (ProfilReportTable.Rows[i]["Name"].ToString() == "Стекло Лакомат")
                    {
                        LacomatCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                        LacomatCostWithTransport += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                        LacomatSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                        LacomatWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
                        LacomatPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["Price"]);
                        LacomatPriceWithTransport = Convert.ToDecimal(ProfilReportTable.Rows[i]["PriceWithTransport"]);
                        b = true;
                    }

                    if (ProfilReportTable.Rows[i]["Name"].ToString() == "Стекло Кризет")
                    {
                        KrizetCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                        KrizetCostWithTransport += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                        KrizetSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                        KrizetWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
                        KrizetPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["Price"]);
                        KrizetPriceWithTransport = Convert.ToDecimal(ProfilReportTable.Rows[i]["PriceWithTransport"]);
                        b = true;
                    }

                    if (ProfilReportTable.Rows[i]["Name"].ToString() == "Стекло Флутес")
                    {
                        FlutesCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                        FlutesCostWithTransport += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                        FlutesSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                        FlutesWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
                        FlutesPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["Price"]);
                        FlutesPriceWithTransport = Convert.ToDecimal(ProfilReportTable.Rows[i]["PriceWithTransport"]);
                        b = true;
                    }

                    if (b)
                    {
                        ProfilReportTable.Rows[i].Delete();
                        ProfilReportTable.AcceptChanges();
                        i--;
                        bb = true;
                    }
                }

                if (bb)
                {
                    string AccountingName = string.Empty;
                    string InvNumber = string.Empty;
                    string UNN = string.Empty;
                    decimal TotalDiscount = 0;
                    string ProfilCurrencyCode = string.Empty;
                    string TPSCurrencyCode = string.Empty;
                    //ADD to Table

                    if (ProfilReportTable.Rows.Count > 0)
                    {
                        AccountingName = ProfilReportTable.Rows[0]["AccountingName"].ToString();
                        InvNumber = ProfilReportTable.Rows[0]["InvNumber"].ToString();
                        UNN = ProfilReportTable.Rows[0]["UNN"].ToString();
                        TotalDiscount = Convert.ToDecimal(ProfilReportTable.Rows[0]["TotalDiscount"]);
                        ProfilCurrencyCode = ProfilReportTable.Rows[0]["CurrencyCode"].ToString();
                        TPSCurrencyCode = ProfilReportTable.Rows[0]["TPSCurCode"].ToString();
                    }
                    if (GridSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Решетка";
                        NewRow["Count"] = GridSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = Decimal.Round(GridCost / GridSquare, 2, MidpointRounding.AwayFromZero);
                        NewRow["OriginalPrice"] = Decimal.Round(GridCost / GridSquare, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(GridCostWithTransport / GridSquare, 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = GridCost;
                        NewRow["CostWithTransport"] = GridCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(GridWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }

                    GridSquare = 0;
                    GridCost = 0;
                    GridCost = 0;
                    GridWeight = 0;

                    if (LacomatSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Стекло Лакомат";
                        NewRow["Count"] = LacomatSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = LacomatPrice;
                        NewRow["OriginalPrice"] = LacomatPrice;
                        NewRow["PriceWithTransport"] = LacomatPriceWithTransport;
                        NewRow["Cost"] = LacomatCost;
                        NewRow["CostWithTransport"] = LacomatCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(LacomatWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }

                    LacomatSquare = 0;
                    LacomatCost = 0;
                    LacomatCost = 0;
                    LacomatWeight = 0;
                    LacomatPrice = 0;
                    LacomatPriceWithTransport = 0;

                    if (KrizetSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Стекло Кризет";
                        NewRow["Count"] = KrizetSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = KrizetPrice;
                        NewRow["OriginalPrice"] = KrizetPrice;
                        NewRow["PriceWithTransport"] = KrizetPriceWithTransport;
                        NewRow["Cost"] = KrizetCost;
                        NewRow["CostWithTransport"] = KrizetCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(KrizetWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }

                    KrizetSquare = 0;
                    KrizetCost = 0;
                    KrizetCostWithTransport = 0;
                    KrizetWeight = 0;
                    KrizetPrice = 0;
                    KrizetPriceWithTransport = 0;

                    if (FlutesSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Стекло Флутес";
                        NewRow["Count"] = FlutesSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = FlutesPrice;
                        NewRow["OriginalPrice"] = FlutesPrice;
                        NewRow["PriceWithTransport"] = FlutesPriceWithTransport;
                        NewRow["Cost"] = FlutesCost;
                        NewRow["CostWithTransport"] = FlutesCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(FlutesWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }

                    FlutesSquare = 0;
                    FlutesCost = 0;
                    FlutesCostWithTransport = 0;
                    FlutesWeight = 0;
                    FlutesPrice = 0;
                    FlutesPriceWithTransport = 0;
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow NewRow = ProfilReportTable.NewRow();
                NewRow["UNN"] = dt.Rows[i]["UNN"];
                NewRow["PaymentRate"] = dt.Rows[i]["PaymentRate"];
                NewRow["AccountingName"] = dt.Rows[i]["AccountingName"];
                NewRow["InvNumber"] = dt.Rows[i]["InvNumber"];
                NewRow["CurrencyCode"] = dt.Rows[i]["CurrencyCode"];
                NewRow["TPSCurCode"] = dt.Rows[i]["TPSCurCode"];
                NewRow["TotalDiscount"] = dt.Rows[i]["TotalDiscount"];
                NewRow["Name"] = dt.Rows[i]["Name"];
                NewRow["Count"] = dt.Rows[i]["Count"];
                NewRow["Measure"] = dt.Rows[i]["Measure"];
                NewRow["Price"] = dt.Rows[i]["Price"];
                NewRow["OriginalPrice"] = dt.Rows[i]["OriginalPrice"];
                NewRow["PriceWithTransport"] = dt.Rows[i]["PriceWithTransport"];
                NewRow["Cost"] = dt.Rows[i]["Cost"];
                NewRow["CostWithTransport"] = dt.Rows[i]["CostWithTransport"];
                NewRow["Weight"] = dt.Rows[i]["Weight"];
                ProfilReportTable.Rows.Add(NewRow);
            }

            DistRatesDT.Clear();
            dt.Clear();
            using (DataView DV = new DataView(TPSReportTable))
            {
                DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
            }

            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
            {
                decimal PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                //collect
                GridSquare = 0;
                GridCost = 0;
                GridCostWithTransport = 0;
                GridWeight = 0;

                LacomatSquare = 0;
                LacomatCost = 0;
                LacomatCostWithTransport = 0;
                LacomatWeight = 0;
                LacomatPrice = 0;
                LacomatPriceWithTransport = 0;

                KrizetSquare = 0;
                KrizetCost = 0;
                KrizetCostWithTransport = 0;
                KrizetWeight = 0;
                KrizetPrice = 0;
                KrizetPriceWithTransport = 0;

                FlutesSquare = 0;
                FlutesCost = 0;
                FlutesCostWithTransport = 0;
                FlutesWeight = 0;
                FlutesPrice = 0;
                FlutesPriceWithTransport = 0;

                bool bb = false;
                for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                {
                    if (Convert.ToDecimal(TPSReportTable.Rows[i]["PaymentRate"]) != PaymentRate)
                        continue;

                    bool b = false;
                    if (TPSReportTable.Rows[i]["Name"].ToString().IndexOf("Решетка") > -1)
                    {
                        //GridCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                        //GridCostWithTransport += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                        GridSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                        GridWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
                        //GridCost = 0;
                        //GridCostWithTransport = 0;
                        GridCost += Math.Ceiling(Convert.ToDecimal(TPSReportTable.Rows[i]["OriginalPrice"]) * Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]) / 0.001m) * 0.001m;
                        GridCostWithTransport += Math.Ceiling(Convert.ToDecimal(TPSReportTable.Rows[i]["PriceWithTransport"]) * Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]) / 0.001m) * 0.001m;

                        b = true;
                    }

                    if (TPSReportTable.Rows[i]["Name"].ToString() == "Стекло Лакомат")
                    {
                        LacomatCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                        LacomatCostWithTransport += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                        LacomatSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                        LacomatWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
                        LacomatPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["Price"]);
                        LacomatPriceWithTransport = Convert.ToDecimal(TPSReportTable.Rows[i]["PriceWithTransport"]);
                        b = true;
                    }

                    if (TPSReportTable.Rows[i]["Name"].ToString() == "Стекло Кризет")
                    {
                        KrizetCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                        KrizetCostWithTransport += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                        KrizetSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                        KrizetWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
                        KrizetPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["Price"]);
                        KrizetPriceWithTransport = Convert.ToDecimal(TPSReportTable.Rows[i]["PriceWithTransport"]);
                        b = true;
                    }

                    if (TPSReportTable.Rows[i]["Name"].ToString() == "Стекло Флутес")
                    {
                        FlutesCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                        FlutesCostWithTransport += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                        FlutesSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                        FlutesWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
                        FlutesPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["Price"]);
                        FlutesPriceWithTransport = Convert.ToDecimal(TPSReportTable.Rows[i]["PriceWithTransport"]);
                        b = true;
                    }

                    if (b)
                    {
                        TPSReportTable.Rows[i].Delete();
                        TPSReportTable.AcceptChanges();
                        i--;
                        bb = true;
                    }
                }

                if (bb)
                {
                    string AccountingName = string.Empty;
                    string InvNumber = string.Empty;
                    string UNN = string.Empty;
                    decimal TotalDiscount = 0;
                    string ProfilCurrencyCode = string.Empty;
                    string TPSCurrencyCode = string.Empty;
                    //ADD to Table

                    if (TPSReportTable.Rows.Count > 0)
                    {
                        AccountingName = TPSReportTable.Rows[0]["AccountingName"].ToString();
                        InvNumber = TPSReportTable.Rows[0]["InvNumber"].ToString();
                        UNN = TPSReportTable.Rows[0]["UNN"].ToString();
                        TotalDiscount = Convert.ToDecimal(TPSReportTable.Rows[0]["TotalDiscount"]);
                        ProfilCurrencyCode = TPSReportTable.Rows[0]["CurrencyCode"].ToString();
                        TPSCurrencyCode = TPSReportTable.Rows[0]["TPSCurCode"].ToString();
                    }

                    //ADD to Table
                    if (GridSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Решетка";
                        NewRow["Count"] = GridSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["OriginalPrice"] = Decimal.Round(GridCost / GridSquare, 2, MidpointRounding.AwayFromZero);
                        NewRow["Price"] = Decimal.Round(GridCost / GridSquare, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(GridCostWithTransport / GridSquare, 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = GridCost;
                        NewRow["CostWithTransport"] = GridCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(GridWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }

                    if (LacomatSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Стекло Лакомат";
                        NewRow["Count"] = LacomatSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = LacomatPrice;
                        NewRow["OriginalPrice"] = LacomatPrice;
                        NewRow["PriceWithTransport"] = LacomatPriceWithTransport;
                        NewRow["Cost"] = LacomatCost;
                        NewRow["CostWithTransport"] = LacomatCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(LacomatWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }

                    if (KrizetSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Стекло Кризет";
                        NewRow["Count"] = KrizetSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = KrizetPrice;
                        NewRow["OriginalPrice"] = KrizetPrice;
                        NewRow["PriceWithTransport"] = KrizetPriceWithTransport;
                        NewRow["Cost"] = KrizetCost;
                        NewRow["CostWithTransport"] = KrizetCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(KrizetWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }

                    if (FlutesSquare > 0)
                    {
                        DataRow NewRow = dt.NewRow();
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["TotalDiscount"] = TotalDiscount;
                        NewRow["Name"] = "Стекло Флутес";
                        NewRow["Count"] = FlutesSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = FlutesPrice;
                        NewRow["OriginalPrice"] = FlutesPrice;
                        NewRow["PriceWithTransport"] = FlutesPriceWithTransport;
                        NewRow["Cost"] = FlutesCost;
                        NewRow["CostWithTransport"] = FlutesCostWithTransport;
                        NewRow["Weight"] = Decimal.Round(FlutesWeight, 3, MidpointRounding.AwayFromZero);
                        dt.Rows.Add(NewRow);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow NewRow = TPSReportTable.NewRow();
                NewRow["UNN"] = dt.Rows[i]["UNN"];
                NewRow["PaymentRate"] = dt.Rows[i]["PaymentRate"];
                NewRow["AccountingName"] = dt.Rows[i]["AccountingName"];
                NewRow["InvNumber"] = dt.Rows[i]["InvNumber"];
                NewRow["CurrencyCode"] = dt.Rows[i]["CurrencyCode"];
                NewRow["TPSCurCode"] = dt.Rows[i]["TPSCurCode"];
                NewRow["Name"] = dt.Rows[i]["Name"];
                NewRow["TotalDiscount"] = dt.Rows[i]["TotalDiscount"];
                NewRow["Count"] = dt.Rows[i]["Count"];
                NewRow["Measure"] = dt.Rows[i]["Measure"];
                NewRow["Price"] = dt.Rows[i]["Price"];
                NewRow["OriginalPrice"] = dt.Rows[i]["OriginalPrice"];
                NewRow["PriceWithTransport"] = dt.Rows[i]["PriceWithTransport"];
                NewRow["Cost"] = dt.Rows[i]["Cost"];
                NewRow["CostWithTransport"] = dt.Rows[i]["CostWithTransport"];
                NewRow["Weight"] = dt.Rows[i]["Weight"];
                TPSReportTable.Rows.Add(NewRow);
            }
        }

        public void AssignCost(decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost, decimal WeightProfil, decimal WeightTPS, decimal TotalWeight,
                               decimal TotalProfil, decimal TotalTPS, ref decimal TransportAndOtherProfil,
                               ref decimal TransportAndOtherTPS)
        {
            decimal Total = TransportCost + AdditionalCost;

            if (Total == 0 && ComplaintProfilCost == 0 && ComplaintTPSCost == 0)
                return;

            decimal pProfil = 0;
            decimal pTPS = 0;

            decimal cProfil = 0;
            decimal cTPS = 0;


            pProfil = WeightProfil / (TotalWeight / 100);
            pTPS = WeightTPS / (TotalWeight / 100);

            cProfil = Total / 100 * pProfil - ComplaintProfilCost;
            cTPS = Total / 100 * pTPS - ComplaintTPSCost;

            TransportAndOtherProfil = Decimal.Round(cProfil, 1, MidpointRounding.AwayFromZero);
            TransportAndOtherTPS = Decimal.Round(cTPS, 1, MidpointRounding.AwayFromZero);
        }

        private int GetMegaOrderID(int MainOrderID)
        {
            int MegaOrderID = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM NewMainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                }
            }
            return MegaOrderID;
        }

        private bool IsComplaint(int MegaOrderID)
        {
            bool IsComplaint = false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT IsComplaint" +
                            " FROM NewMegaOrders WHERE MegaOrderID = " + MegaOrderID,
                            ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return false;

                        if (DBNull.Value != DT.Rows[0]["IsComplaint"] && Convert.ToInt32(DT.Rows[0]["IsComplaint"]) > 0)
                        {
                            IsComplaint = Convert.ToBoolean(DT.Rows[0]["IsComplaint"]);
                        }
                    }
                }
            }

            return IsComplaint;
        }

        public static void DataSetIntoDBF(string path, string fileName, DataTable DT, int FactoryID)
        {
            if (File.Exists(path + fileName + ".dbf"))
            {
                File.Delete(path + fileName + ".dbf");
            }

            string createSql = "create table " + fileName + " ([UNNP] varchar(20), [UNN] varchar(20), [CurrencyCode] varchar(20), [InvNumber] varchar(20), [AccountingName] varchar(100), [Amount] Double, [Price] Double, [NDS] varchar(10), [Weight] Double)";

            OleDbConnection con = new OleDbConnection(GetConnection(path));

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con
            };
            con.Open();

            cmd.CommandText = createSql;

            cmd.ExecuteNonQuery();

            foreach (DataRow row in DT.Rows)
            {
                string insertSql = "insert into " + fileName +
                    " (UNNP, UNN, CurrencyCode, InvNumber, AccountingName, Amount, Price, NDS, Weight) values(UNNP, UNN, CurrencyCode, InvNumber, AccountingName, Amount, Price, NDS, Weight)";
                decimal d = Decimal.Round(Convert.ToDecimal(row["Weight"]) / 1000, 3, MidpointRounding.AwayFromZero);
                double Amount = Convert.ToDouble(row["Count"]);
                double Price = Convert.ToDouble(row["Price"]);
                double Weight = Convert.ToDouble(d);
                string InvNumber = row["InvNumber"].ToString();
                string CurrencyCode = row["CurrencyCode"].ToString();
                //string TPSCurCode = row["TPSCurCode"].ToString();
                string UNN = row["UNN"].ToString();
                string AccountingName = row["AccountingName"].ToString();
                string Name = row["Name"].ToString();
                string NDS = "0 %";
                string UNNP = "800014979";
                cmd.CommandText = insertSql;
                cmd.Parameters.Clear();

                if (Convert.ToInt32(CurrencyCode) == 974 || Convert.ToInt32(CurrencyCode) == 933)
                    NDS = "20 %";
                if (FactoryID == 2)
                    UNNP = "590618616";
                cmd.Parameters.Add("UNNP", OleDbType.VarChar).Value = UNNP;
                cmd.Parameters.Add("UNN", OleDbType.VarChar).Value = UNN;
                cmd.Parameters.Add("CurrencyCode", OleDbType.VarChar).Value = CurrencyCode;
                //cmd.Parameters.Add("TPSCurCode", OleDbType.VarChar).Value = TPSCurCode;
                cmd.Parameters.Add("InvNumber", OleDbType.VarChar).Value = InvNumber;
                cmd.Parameters.Add("AccountingName", OleDbType.VarChar).Value = AccountingName;
                cmd.Parameters.Add("Amount", OleDbType.Double).Value = Amount;
                cmd.Parameters.Add("Price", OleDbType.Double).Value = Price;
                cmd.Parameters.Add("NDS", OleDbType.VarChar).Value = NDS;
                cmd.Parameters.Add("Weight", OleDbType.Double).Value = Weight;
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }

        private static string ConvertDefaultToDos(string src)
        {
            byte[] buffer;
            buffer = Encoding.Default.GetBytes(src);
            Encoding.Convert(Encoding.Default, Encoding.GetEncoding(866), buffer);
            return Encoding.Default.GetString(buffer);
        }

        private static string GetConnection(string path)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=dBASE 5.0;";
        }

        public DataTable GetMegaOrdersTable(int[] NewMegaOrders)
        {
            DataTable ReturnDT = new DataTable();
            string SelectCommand = @"SELECT NewMegaOrders.MegaOrderID, NewMegaOrders.OrderNumber, NewMegaOrders.ClientID, 
                NewMegaOrders.ComplaintProfilCost, NewMegaOrders.ComplaintTPSCost, NewMegaOrders.TransportCost, NewMegaOrders.AdditionalCost, NewMegaOrders.CurrencyTypeID,
                NewMainOrders.MainOrderID, NewMainOrders.Weight
                FROM NewMainOrders
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID=NewMegaOrders.MegaOrderID
                AND NewMegaOrders.MegaOrderID IN (" + string.Join(",", NewMegaOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(ReturnDT);
                ReturnDT.Columns.Add(new DataColumn("ProfilFrontsWeight", Type.GetType("System.Decimal")));
                ReturnDT.Columns.Add(new DataColumn("TPSFrontsWeight", Type.GetType("System.Decimal")));
                ReturnDT.Columns.Add(new DataColumn("ProfilDecorWeight", Type.GetType("System.Decimal")));
                ReturnDT.Columns.Add(new DataColumn("TPSDecorWeight", Type.GetType("System.Decimal")));
                for (int i = 0; i < ReturnDT.Rows.Count; i++)
                {
                    decimal ProfilFrontsWeight = 0;
                    decimal TPSFrontsWeight = 0;
                    decimal ProfilDecorWeight = 0;
                    decimal TPSDecorWeight = 0;
                    int MainOrderID = Convert.ToInt32(ReturnDT.Rows[i]["MainOrderID"]);
                    SelectCommand = @"SELECT Square,Weight,FactoryID FROM NewFrontsOrders WHERE MainOrderID=" + MainOrderID;
                    using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA1.Fill(DT);
                            for (int j = 0; j < DT.Rows.Count; j++)
                            {
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 1)
                                    ProfilFrontsWeight += Convert.ToDecimal(DT.Rows[j]["Square"]) * Convert.ToDecimal(0.7) + Convert.ToDecimal(DT.Rows[j]["Weight"]);
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 2)
                                    TPSFrontsWeight += Convert.ToDecimal(DT.Rows[j]["Square"]) * Convert.ToDecimal(0.7) + Convert.ToDecimal(DT.Rows[j]["Weight"]);
                            }
                        }
                    }
                    SelectCommand = @"SELECT Weight,FactoryID FROM NewDecorOrders WHERE MainOrderID=" + MainOrderID;
                    using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA1.Fill(DT);
                            for (int j = 0; j < DT.Rows.Count; j++)
                            {
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 1)
                                    ProfilDecorWeight += Convert.ToDecimal(DT.Rows[j]["Weight"]);
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 2)
                                    TPSDecorWeight += Convert.ToDecimal(DT.Rows[j]["Weight"]);
                            }
                        }
                    }
                    ReturnDT.Rows[i]["ProfilFrontsWeight"] = ProfilFrontsWeight;
                    ReturnDT.Rows[i]["TPSFrontsWeight"] = TPSFrontsWeight;
                    ReturnDT.Rows[i]["ProfilDecorWeight"] = ProfilDecorWeight;
                    ReturnDT.Rows[i]["TPSDecorWeight"] = TPSDecorWeight;
                }
            }
            return ReturnDT;
        }

        public void CreateReport(
            ref HSSFWorkbook hssfworkbook,
            ref HSSFSheet sheet1, int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight, int pos, bool IsSample)
        {
            ClearReport();

            string MainOrdersList = string.Empty;

            string Currency = string.Empty;

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();
            //if (ClientID == 145 || ClientID == 258 || ClientID == 267)
            if (ClientID == 145)
            {
                VAT = 1.2m;
            }
            else
            {
                VAT = 1.0m;
            }
            DataTable InfoDT = GetMegaOrdersTable(NewMegaOrders);
            TransportCost = TransportCost / VAT;
            AdditionalCost = AdditionalCost / VAT;
            FrontsReport.Report(MainOrdersIDs, InfoDT, IsSample);
            DecorReport.Report(MainOrdersIDs, IsSample);

            //PROFIL
            if (FrontsReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(FrontsReport.ProfilReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(DecorReport.ProfilReportDataTable.Rows[i]);
                }
            }


            //TPS
            if (FrontsReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(FrontsReport.TPSReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(DecorReport.TPSReportDataTable.Rows[i]);
                }
            }
            //F(@"D:\temp", "temp", TPSReportTable);
            CollectGridsAndGlass();
            ConvertToCurrency(CurrencyTypeID, false);
            ConvertToCurrency(CurrencyTypeID, true);

            decimal Total = TransportCost + AdditionalCost - ComplaintProfilCost - ComplaintTPSCost;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;
            decimal WeightProfil = 0;
            decimal WeightTPS = 0;

            decimal TransportAndOtherProfil = 0;
            decimal TransportAndOtherTPS = 0;

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                WeightProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                WeightTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
            }

            TotalCost = (TotalProfil + TotalTPS);

            //Assign COST
            AssignCost(ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost, WeightProfil, WeightTPS, TotalWeight, TotalProfil, TotalTPS, ref TransportAndOtherProfil,
                       ref TransportAndOtherTPS);

            decimal dd = 0;
            if (TotalWeight != 0)
                dd = Decimal.Round((WeightProfil + WeightTPS) / TotalWeight, 3, MidpointRounding.AwayFromZero);
            TotalProfil = Decimal.Round(TotalProfil, 3, MidpointRounding.AwayFromZero);
            TotalTPS = Decimal.Round(TotalTPS, 3, MidpointRounding.AwayFromZero);
            TransportCost = Decimal.Round(TransportCost * dd, 3, MidpointRounding.AwayFromZero);
            AdditionalCost = Decimal.Round(AdditionalCost * dd, 3, MidpointRounding.AwayFromZero);
            TotalCost = Decimal.Round(TotalCost, 3, MidpointRounding.AwayFromZero);

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 11;
            HeaderF1.Boldweight = 11 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 10;
            HeaderF2.Boldweight = 10 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 9;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.000");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            HSSFCellStyle WeightCS = hssfworkbook.CreateCellStyle();
            WeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            WeightCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            WeightCS.BottomBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            WeightCS.LeftBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            WeightCS.RightBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            WeightCS.TopBorderColor = HSSFColor.BLACK.index;
            WeightCS.SetFont(SimpleF);

            HSSFCellStyle PriceBelCS = hssfworkbook.CreateCellStyle();
            PriceBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            PriceBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.SetFont(SimpleF);

            HSSFCellStyle PriceForeignCS = hssfworkbook.CreateCellStyle();
            PriceForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            PriceForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.SetFont(SimpleF);

            HSSFCellStyle CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.0000");
            CurrencyCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.BottomBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.LeftBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.RightBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.TopBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS1 = hssfworkbook.CreateCellStyle();
            ReportCS1.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS1.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS1.SetFont(HeaderF1);

            HSSFCellStyle ReportCS2 = hssfworkbook.CreateCellStyle();
            ReportCS2.SetFont(HeaderF1);

            HSSFCellStyle SummaryWithoutBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithoutBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithoutBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithoutBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWeightCS = hssfworkbook.CreateCellStyle();
            SummaryWeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWeightCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithBorderBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.WrapText = true;
            SummaryWithBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithBorderForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.WrapText = true;
            SummaryWithBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFCell Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Сводный отчет:");
            Cell1.CellStyle = ReportCS1;

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = ReportCS1;
            }

            int DisplayIndex = 0;
            if (ProfilReportTable.Rows.Count > 0)
            {
                //Профиль
                pos += 2;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-Профиль:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ед. измер.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена нач, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Коэф.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Скидка, %");
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Цена кон, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Стоимость, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Вес, кг.");
                Cell1.CellStyle = SimpleHeaderCS;

                for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["Name"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["Measure"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = WeightCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = WeightCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = PriceForeignCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }

            DisplayIndex = 0;
            if (TPSReportTable.Rows.Count > 0)
            {
                //ТПС
                pos++;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-ТПС:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ед. измер.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена нач, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Коэф.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Скидка, %");
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Цена кон, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Стоимость, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Вес, кг.");
                Cell1.CellStyle = SimpleHeaderCS;

                for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["Name"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["Measure"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = WeightCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = WeightCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = WeightCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = WeightCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }
            pos++;

            //if (Rate != 1)
            //{
            //    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            //    Cell1.SetCellValue("Курс, 1 EUR = " + Rate + " " + Currency);
            //    Cell1.CellStyle = SummaryWithoutBorderBelCS;
            //    //Excel.WriteCell(1, "Курс, 1 EUR = " + Rate + " " + Currency, pos++, 1, 12, true);
            //}

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                TotalCost = 0;
            }

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }

            ClearReport();
        }

        public void CreateReport(
            ref HSSFWorkbook hssfworkbook,
            ref HSSFSheet sheet1, int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight, int pos)
        {
            ClearReport();

            string MainOrdersList = string.Empty;

            string Currency = string.Empty;

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();
            //if (ClientID == 145 || ClientID == 258 || ClientID == 267)
            if (ClientID == 145)
            {
                VAT = 1.2m;
            }
            else
            {
                VAT = 1.0m;
            }
            DataTable InfoDT = GetMegaOrdersTable(NewMegaOrders);
            TransportCost = TransportCost / VAT;
            AdditionalCost = AdditionalCost / VAT;
            FrontsReport.Report(MainOrdersIDs, InfoDT);
            DecorReport.Report(MainOrdersIDs);

            //PROFIL
            if (FrontsReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(FrontsReport.ProfilReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(DecorReport.ProfilReportDataTable.Rows[i]);
                }
            }


            //TPS
            if (FrontsReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(FrontsReport.TPSReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(DecorReport.TPSReportDataTable.Rows[i]);
                }
            }
            //F(@"D:\temp", "temp", TPSReportTable);
            CollectGridsAndGlass();
            ConvertToCurrency(CurrencyTypeID, false);
            ConvertToCurrency(CurrencyTypeID, true);

            decimal Total = TransportCost + AdditionalCost - ComplaintProfilCost - ComplaintTPSCost;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;
            decimal WeightProfil = 0;
            decimal WeightTPS = 0;

            decimal TransportAndOtherProfil = 0;
            decimal TransportAndOtherTPS = 0;

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                WeightProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                WeightTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
            }

            TotalCost = (TotalProfil + TotalTPS);

            //Assign COST
            AssignCost(ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost, WeightProfil, WeightTPS, TotalWeight, TotalProfil, TotalTPS, ref TransportAndOtherProfil,
                       ref TransportAndOtherTPS);

            decimal dd = 0;
            if (TotalWeight != 0)
                dd = Decimal.Round((WeightProfil + WeightTPS) / TotalWeight, 3, MidpointRounding.AwayFromZero);
            TotalProfil = Decimal.Round(TotalProfil, 3, MidpointRounding.AwayFromZero);
            TotalTPS = Decimal.Round(TotalTPS, 3, MidpointRounding.AwayFromZero);
            TransportCost = Decimal.Round(TransportCost * dd, 3, MidpointRounding.AwayFromZero);
            AdditionalCost = Decimal.Round(AdditionalCost * dd, 3, MidpointRounding.AwayFromZero);
            TotalCost = Decimal.Round(TotalCost, 3, MidpointRounding.AwayFromZero);

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 11;
            HeaderF1.Boldweight = 11 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 10;
            HeaderF2.Boldweight = 10 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 9;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.000");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            HSSFCellStyle WeightCS = hssfworkbook.CreateCellStyle();
            WeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            WeightCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            WeightCS.BottomBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            WeightCS.LeftBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            WeightCS.RightBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            WeightCS.TopBorderColor = HSSFColor.BLACK.index;
            WeightCS.SetFont(SimpleF);

            HSSFCellStyle PriceBelCS = hssfworkbook.CreateCellStyle();
            PriceBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            PriceBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.SetFont(SimpleF);

            HSSFCellStyle PriceForeignCS = hssfworkbook.CreateCellStyle();
            PriceForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            PriceForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.SetFont(SimpleF);

            HSSFCellStyle CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.0000");
            CurrencyCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.BottomBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.LeftBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.RightBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.TopBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS1 = hssfworkbook.CreateCellStyle();
            ReportCS1.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS1.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS1.SetFont(HeaderF1);

            HSSFCellStyle ReportCS2 = hssfworkbook.CreateCellStyle();
            ReportCS2.SetFont(HeaderF1);

            HSSFCellStyle SummaryWithoutBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithoutBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithoutBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithoutBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWeightCS = hssfworkbook.CreateCellStyle();
            SummaryWeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWeightCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithBorderBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.WrapText = true;
            SummaryWithBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithBorderForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.WrapText = true;
            SummaryWithBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFCell Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Сводный отчет:");
            Cell1.CellStyle = ReportCS1;

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = ReportCS1;
            }

            int DisplayIndex = 0;
            if (ProfilReportTable.Rows.Count > 0)
            {
                //Профиль
                pos += 2;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-Профиль:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ед. измер.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена нач, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Коэф.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Скидка, %");
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Цена кон, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Стоимость, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Вес, кг.");
                Cell1.CellStyle = SimpleHeaderCS;

                for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["Name"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["Measure"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = WeightCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = WeightCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = PriceForeignCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }

            DisplayIndex = 0;
            if (TPSReportTable.Rows.Count > 0)
            {
                //ТПС
                pos++;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-ТПС:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ед. измер.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена нач, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Коэф.");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Скидка, %");
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Цена кон, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue("Стоимость, " + Currency);
                //Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Вес, кг.");
                Cell1.CellStyle = SimpleHeaderCS;

                for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["Name"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["Measure"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = WeightCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = WeightCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["OriginalPrice"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["OriginalPrice"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["DiscountVolume"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["DiscountVolume"]));
                        Cell1.CellStyle = WeightCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["TotalDiscount"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["TotalDiscount"]));
                        Cell1.CellStyle = WeightCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Price"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["PriceWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["CostWithTransport"] != DBNull.Value)
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        //Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        //Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(7);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(9);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }
            pos++;

            //if (Rate != 1)
            //{
            //    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            //    Cell1.SetCellValue("Курс, 1 EUR = " + Rate + " " + Currency);
            //    Cell1.CellStyle = SummaryWithoutBorderBelCS;
            //    //Excel.WriteCell(1, "Курс, 1 EUR = " + Rate + " " + Currency, pos++, 1, 12, true);
            //}

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                TotalCost = 0;
            }

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }

            ClearReport();
        }

        public void ClearReport()
        {
            ProfilReportTable.Clear();
            TPSReportTable.Clear();

            FrontsReport.ClearReport();
            DecorReport.ClearReport();
        }

        //private void ReadReportFilePath(string FileName)
        //{
        //using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName, Encoding.Default))
        //    {
        //        ReportFilePath = sr.ReadToEnd();
        //    }
        //}

    }

    public class DetailsReport : IAllFrontParameterName, IIsMarsel
    {
        Report ClientReport = null;

        //string ReportFilePath = null;

        public bool Save = false;
        public bool Send = false;

        DataTable ClientsDataTable = null;

        DataTable FrontsResultDataTable = null;
        DataTable[] DecorResultDataTable = null;

        public DataTable[] ClientReportTables = null;

        public DataTable ReportTable = null;

        FrontsCalculate FrontsCalculate = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        public DataTable CurrencyTypesDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        public DataTable TechnoInsetTypesDataTable = null;
        public DataTable TechnoInsetColorsDataTable = null;

        DecorCatalogOrder DecorCatalogOrder = null;

        public DetailsReport(
            ref DecorCatalogOrder tDecorCatalogOrder,
            ref FrontsCalculate tFrontsCalculate)
        {
            DecorCatalogOrder = tDecorCatalogOrder;
            FrontsCalculate = tFrontsCalculate;

            Create();
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
            //SelectCommand = @"SELECT * FROM InsetColors";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(InsetColorsDataTable);
            //}
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            CreateFrontsDataTable();
            CreateDecorDataTable();

            //ReadReportFilePath("MarketingClientReportPath.config");

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            //string FileName = InventoryName + ".xls";

            //FileName = Path.Combine(ReportFilePath, FileName);

            //int DocNumber = 1;

            //while (File.Exists(FileName))
            //{
            //    FileName = InventoryName + "(" + DocNumber++ + ").xls";
            //    FileName = Path.Combine(ReportFilePath, FileName);
            //}

            //FileInfo file = new FileInfo(FileName);

            //FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            //hssfworkbook.Write(NewFile);
            //NewFile.Close();

            //System.Diagnostics.Process.Start(file.FullName);
        }

        private void Create()
        {
            FrontsOrdersDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            CurrencyTypesDataTable = new DataTable();
            DecorResultDataTable = new DataTable[DecorCatalogOrder.DecorProductsCount];
            DecorOrdersDataTable = new DataTable();
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

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("FrontName"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor1"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor2"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType1"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor1"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType2"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor2"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Weight"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrontPrice"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetPrice"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Rate"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
        }

        private void CreateDecorDataTable()
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorResultDataTable[i] = new DataTable();

                DecorResultDataTable[i].Columns.Add(new DataColumn(("Name"), Type.GetType("System.String")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Height"), Type.GetType("System.Int32")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Length"), Type.GetType("System.Int32")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Width"), Type.GetType("System.Int32")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Color"), Type.GetType("System.String")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Patina"), Type.GetType("System.String")));

                DecorResultDataTable[i].Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Weight"), System.Type.GetType("System.Decimal")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Price"), System.Type.GetType("System.Decimal")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.Decimal")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Rate"), System.Type.GetType("System.Decimal")));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Notes"), Type.GetType("System.String")));
            }
        }

        private string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM NewMainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        private int GetOrderNumber(int MainOrderID)
        {
            int OrderNumber = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM NewMegaOrders WHERE MegaOrderID=" +
                    "(SELECT MegaOrderID FROM NewMainOrders WHERE MainOrderID=" + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        OrderNumber = Convert.ToInt32(DT.Rows[0]["OrderNumber"]);
                }
            }
            return OrderNumber;
        }

        private int GetClientID(int MainOrderID)
        {
            int ClientID = -1;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM NewMegaOrders WHERE MegaOrderID=" +
                    "(SELECT MegaOrderID FROM NewMainOrders WHERE MainOrderID=" + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    try
                    {
                        if (DT.Rows.Count > 0)
                            ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return ClientID;
        }

        private string GetClientName(int MainOrderID)
        {
            string ClientName = "unnamed_client";
            try
            {
                DataRow[] Rows = ClientsDataTable.Select("ClientID = " + GetClientID(MainOrderID));
                if (Rows.Count() > 0)
                    ClientName = Rows[0]["ClientName"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ClientName;
        }

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

        private void FillFronts(int MainOrderID)
        {
            FrontsResultDataTable.Clear();
            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                string FrameColor1 = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string FrameColor2 = string.Empty;

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
                string InsetType2 = string.Empty;
                string InsetColor1 = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                string InsetColor2 = string.Empty;
                string PatinaName = GetPatinaName(Convert.ToInt32(Row["PatinaID"]));

                if (Convert.ToInt32(Row["TechnoColorID"]) != -1)
                    FrameColor2 = GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetTypeID"]) != -1)
                    InsetType2 = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    InsetColor2 = GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                NewRow["FrontName"] = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                NewRow["FrameColor1"] = FrameColor1;
                NewRow["FrameColor2"] = FrameColor2;
                NewRow["Patina"] = PatinaName;
                NewRow["InsetType1"] = InsetType;
                NewRow["InsetColor1"] = InsetColor1;
                NewRow["InsetType2"] = InsetType2;
                NewRow["InsetColor2"] = InsetColor2;
                NewRow["Weight"] = Row["Weight"];
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Convert.ToInt32(Row["Count"]);
                NewRow["FrontPrice"] = Decimal.Round(Convert.ToDecimal(Row["FrontPrice"]), 2, MidpointRounding.AwayFromZero);
                NewRow["InsetPrice"] = Decimal.Round(Convert.ToDecimal(Row["InsetPrice"]), 2, MidpointRounding.AwayFromZero);
                NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(Row["Cost"]), 2, MidpointRounding.AwayFromZero);
                NewRow["Rate"] = Row["PaymentRate"];
                NewRow["Notes"] = Row["Notes"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
        }

        private void FillDecor(int MainOrderID)
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorResultDataTable[i].Clear();
                DecorResultDataTable[i].AcceptChanges();

                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]);

                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow Row in Rows)
                {
                    DataRow NewRow2 = DecorResultDataTable[i].NewRow();

                    NewRow2["Name"] = DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString() + " " +
                                            DecorCatalogOrder.GetItemName(Convert.ToInt32(Row["DecorID"]));

                    //if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Height"))
                    //    NewRow2["Height"] = Convert.ToInt32(Row["Height"]);
                    //else
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow2["Height"] = Convert.ToInt32(Row["Height"]);

                    //if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Length"))
                    //    NewRow2["Height"] = Convert.ToInt32(Row["Length"]);
                    //else
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow2["Length"] = Convert.ToInt32(Row["Length"]);

                    //if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Width"))
                    //    NewRow2["Width"] = Convert.ToInt32(Row["Width"]);
                    //else
                    if (Convert.ToInt32(Row["Width"]) != -1)
                        NewRow2["Width"] = Convert.ToInt32(Row["Width"]);

                    //if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Length") && Convert.ToInt32(Row["Length"]) != -1)
                    //    NewRow2["Height"] = Convert.ToInt32(Row["Length"]);

                    //if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Width") && Convert.ToInt32(Row["Width"]) != -1)
                    //    NewRow2["Width"] = Convert.ToInt32(Row["Width"]);

                    if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "ColorID"))
                        NewRow2["Color"] = GetColorName(Convert.ToInt32(Row["ColorID"]));

                    NewRow2["Patina"] = GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                    int Count = Convert.ToInt32(Row["Count"]);
                    NewRow2["Count"] = Convert.ToInt32(Row["Count"]);
                    NewRow2["Price"] = Decimal.Round(Convert.ToDecimal(Row["Price"]), 2, MidpointRounding.AwayFromZero);
                    NewRow2["Cost"] = Decimal.Round(Convert.ToDecimal(Row["Cost"]), 2, MidpointRounding.AwayFromZero);
                    NewRow2["Rate"] = Row["PaymentRate"];
                    NewRow2["Notes"] = Row["Notes"];
                    NewRow2["Weight"] = Row["Weight"];
                    DecorResultDataTable[i].Rows.Add(NewRow2);
                }
            }
        }

        private bool FilterOrders(int MainOrderID, bool IsSample)
        {
            bool IsNotEmpty = false;

            FrontsOrdersDataTable.Clear();
            FrontsOrdersDataTable.AcceptChanges();
            string SelectCommand = "SELECT NewFrontsOrders.*, NewMegaOrders.PaymentRate FROM NewFrontsOrders INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID AND NewFrontsOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID WHERE NewFrontsOrders.IsSample=1 ";
            if (!IsSample)
                SelectCommand = "SELECT NewFrontsOrders.*, NewMegaOrders.PaymentRate FROM NewFrontsOrders INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID AND NewFrontsOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID WHERE NewFrontsOrders.IsSample=0 ";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(FrontsOrdersDataTable) > 0)
                    IsNotEmpty = true;
            }

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();
            SelectCommand = "SELECT NewDecorOrders.*, NewMegaOrders.PaymentRate FROM NewDecorOrders INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID AND NewDecorOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID WHERE NewDecorOrders.IsSample=1 ";
            if (!IsSample)
                SelectCommand = "SELECT NewDecorOrders.*, NewMegaOrders.PaymentRate FROM NewDecorOrders INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID AND NewDecorOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID WHERE NewDecorOrders.IsSample=0 ";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(DecorOrdersDataTable) > 0)
                    IsNotEmpty = true;
            }

            return IsNotEmpty;
        }

        private bool FilterOrders(int MainOrderID)
        {
            bool IsNotEmpty = false;

            FrontsOrdersDataTable.Clear();
            FrontsOrdersDataTable.AcceptChanges();
            string SelectCommand = "SELECT NewFrontsOrders.*, NewMegaOrders.PaymentRate FROM NewFrontsOrders INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID AND NewFrontsOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(FrontsOrdersDataTable) > 0)
                    IsNotEmpty = true;
            }

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();
            SelectCommand = "SELECT NewDecorOrders.*, NewMegaOrders.PaymentRate FROM NewDecorOrders INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID AND NewDecorOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(DecorOrdersDataTable) > 0)
                    IsNotEmpty = true;
            }

            return IsNotEmpty;
        }

        private decimal CalculateCost()
        {
            decimal FrontsCost = 0;
            decimal DecorCost = 0;
            decimal OrderCost = 0;

            foreach (DataRow rows1 in FrontsOrdersDataTable.Rows)
                FrontsCost += Convert.ToDecimal(rows1["Cost"]);

            foreach (DataRow rows2 in DecorOrdersDataTable.Rows)
                DecorCost += Convert.ToDecimal(rows2["Cost"]);

            OrderCost = FrontsCost + DecorCost;
            return OrderCost;
        }

        private int GetMegaOrderID(int MainOrderID)
        {
            int MegaOrderID = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM NewMainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                }
            }
            return MegaOrderID;
        }

        private bool IsComplaint(int MegaOrderID)
        {
            bool IsComplaint = false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT IsComplaint" +
                " FROM NewMegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DBNull.Value != DT.Rows[0]["IsComplaint"] && Convert.ToInt32(DT.Rows[0]["IsComplaint"]) > 0)
                        {
                            IsComplaint = Convert.ToBoolean(DT.Rows[0]["IsComplaint"]);
                        }
                    }
                }
            }

            return IsComplaint;
        }

        public string Report(int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost,
            decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight, bool IsSample)
        {
            ClearReport();

            ClientReport = new Report(ref DecorCatalogOrder, ref FrontsCalculate);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 13;
            HeaderF1.Boldweight = 13 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 12;
            HeaderF2.Boldweight = 12 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 9;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle SimpleDecCS = hssfworkbook.CreateCellStyle();
            SimpleDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            SimpleDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS = hssfworkbook.CreateCellStyle();
            ReportCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS.SetFont(HeaderF1);

            HSSFCellStyle HeaderWithoutBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithoutBorderCS.SetFont(HeaderF2);

            HSSFCellStyle HeaderWithBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithBorderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.TopBorderColor = HSSFColor.BLACK.index;
            //HeaderWithBorderCS.WrapText = true;
            HeaderWithBorderCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            //HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            //HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Подробный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 25 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 14 * 256);
            sheet1.SetColumnWidth(8, 13 * 256);
            sheet1.SetColumnWidth(9, 10 * 256);
            sheet1.SetColumnWidth(10, 9 * 256);
            sheet1.SetColumnWidth(11, 12 * 256);
            sheet1.SetColumnWidth(12, 12 * 256);
            sheet1.SetColumnWidth(13, 13 * 256);
            sheet1.SetColumnWidth(14, 12 * 256);
            sheet1.SetColumnWidth(15, 10 * 256);

            decimal OrderCost = 0;

            int RowIndex = 0;

            string Currency = string.Empty;

            CurrencyTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();

            ClientName = GetClientName(MainOrdersIDs[0]);

            HSSFCell Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("Клиент:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(ClientName);
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("№ заказа:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(string.Join(",", OrderNumbers));
            Cell1.CellStyle = HeaderWithoutBorderCS;
            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = HeaderWithoutBorderCS;
            }

            RowIndex++;
            for (int i = 0; i < MainOrdersIDs.Count(); i++)
            {
                OrderCost = 0;

                if (FilterOrders(MainOrdersIDs[i], IsSample))
                {
                    FillFronts(MainOrdersIDs[i]);
                    FillDecor(MainOrdersIDs[i]);

                    RowIndex++;

                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("№" + GetOrderNumber(MainOrdersIDs[i]) + "-" + MainOrdersIDs[i].ToString());
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                    RowIndex++;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Примечание к заказу: " + GetMainOrderNotes(MainOrdersIDs[i]));
                    Cell1.CellStyle = HeaderWithoutBorderCS;

                    int DisplayIndex = 0;
                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Выс.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Шир.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Кол.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за вставку");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;

                        RowIndex++;
                    }

                    if (FrontsResultDataTable.Rows.Count != 0)
                        RowIndex++;
                    OrderCost = CalculateCost();
                    DisplayIndex = 0;
                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        DisplayIndex = 0;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrontName"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Patina"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Height"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Width"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Count"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Weight"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["FrontPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["InsetPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Cost"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Rate"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Notes"].ToString());
                        Cell1.CellStyle = SimpleCS;

                        RowIndex++;
                    }
                    //if (FrontsResultDataTable.Rows.Count != 0)
                    //    RowIndex++;

                    DisplayIndex = 0;
                    //декор
                    for (int c = 0; c < DecorCatalogOrder.DecorProductsCount; c++)
                    {
                        if (DecorResultDataTable[c].Rows.Count == 0)
                            continue;
                        //int ColumnCount = 0;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(0);
                        Cell1.SetCellValue("Наименование");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(1);
                        Cell1.SetCellValue("Цвет");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(2);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(3);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(4);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(5);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(6);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;


                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(7);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(8);
                        Cell1.SetCellValue("Высота");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(9);
                        Cell1.SetCellValue("Длина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(10);
                        Cell1.SetCellValue("Ширина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(11);
                        Cell1.SetCellValue("Кол-во");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(12);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(13);
                        Cell1.SetCellValue("Цена");
                        Cell1.CellStyle = SimpleHeaderCS;

                        //Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        //Cell1.SetCellValue(string.Empty);
                        //Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(15);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(16);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;
                        RowIndex++;
                        RowIndex++;


                        DisplayIndex = 0;
                        //вывод заказов декора в excel
                        for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                        {
                            DisplayIndex = 0;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Name"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Color"] != DBNull.Value)
                                Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Color"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Patina"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Height"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Height"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Length"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Length"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Width"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Width"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Count"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Weight"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Weight"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Price"]));
                            Cell1.CellStyle = SimpleDecCS;
                            //Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            //Cell1.SetCellValue(string.Empty);
                            //Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Cost"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Rate"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Notes"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            RowIndex++;
                        }
                        //RowIndex++;
                    }
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Итого, EUR: " + Decimal.Round(OrderCost, 2, MidpointRounding.AwayFromZero));
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                }

                RowIndex++;
                RowIndex++;


            }
            RowIndex++;
            RowIndex++;

            ClientReport.CreateReport(
                ref hssfworkbook, ref sheet1,
                NewMegaOrders, OrderNumbers, MainOrdersIDs, ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost,
                TransportCost, AdditionalCost, TotalCost, CurrencyTypeID, TotalWeight, RowIndex, IsSample);

            string FileName = ClientsDataTable.Select("ClientName = '" + ClientName + "'")[0]["Login"].ToString() +
                    " - №" + string.Join(",", OrderNumbers);
            if (IsSample)
                FileName = ClientsDataTable.Select("ClientName = '" + ClientName + "'")[0]["Login"].ToString() +
                        " - №" + string.Join(",", OrderNumbers) + " (обр)";
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

            ClearReport();
            return file.FullName;
        }

        public string Report(int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost,
            decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight)
        {
            ClearReport();

            ClientReport = new Report(ref DecorCatalogOrder, ref FrontsCalculate);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 13;
            HeaderF1.Boldweight = 13 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 12;
            HeaderF2.Boldweight = 12 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 9;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle SimpleDecCS = hssfworkbook.CreateCellStyle();
            SimpleDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            SimpleDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS = hssfworkbook.CreateCellStyle();
            ReportCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS.SetFont(HeaderF1);

            HSSFCellStyle HeaderWithoutBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithoutBorderCS.SetFont(HeaderF2);

            HSSFCellStyle HeaderWithBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithBorderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.TopBorderColor = HSSFColor.BLACK.index;
            //HeaderWithBorderCS.WrapText = true;
            HeaderWithBorderCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            //HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            //HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Подробный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 25 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 14 * 256);
            sheet1.SetColumnWidth(8, 13 * 256);
            sheet1.SetColumnWidth(9, 10 * 256);
            sheet1.SetColumnWidth(10, 9 * 256);
            sheet1.SetColumnWidth(11, 12 * 256);
            sheet1.SetColumnWidth(12, 12 * 256);
            sheet1.SetColumnWidth(13, 13 * 256);
            sheet1.SetColumnWidth(14, 12 * 256);
            sheet1.SetColumnWidth(15, 10 * 256);

            decimal OrderCost = 0;

            int RowIndex = 0;

            string Currency = string.Empty;

            CurrencyTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();

            //MessageBox.Show("MainOrdersIDs=" + MainOrdersIDs.Count());
            ClientName = GetClientName(MainOrdersIDs[0]);

            HSSFCell Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("Клиент:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(ClientName);
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("№ заказа:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(string.Join(",", OrderNumbers));
            Cell1.CellStyle = HeaderWithoutBorderCS;
            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = HeaderWithoutBorderCS;
            }

            RowIndex++;
            for (int i = 0; i < MainOrdersIDs.Count(); i++)
            {
                OrderCost = 0;

                if (FilterOrders(MainOrdersIDs[i]))
                {
                    FillFronts(MainOrdersIDs[i]);
                    FillDecor(MainOrdersIDs[i]);

                    RowIndex++;

                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("№" + GetOrderNumber(MainOrdersIDs[i]) + "-" + MainOrdersIDs[i].ToString());
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                    RowIndex++;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Примечание к заказу: " + GetMainOrderNotes(MainOrdersIDs[i]));
                    Cell1.CellStyle = HeaderWithoutBorderCS;

                    int DisplayIndex = 0;
                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Выс.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Шир.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Кол.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за вставку");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;

                        RowIndex++;
                    }

                    if (FrontsResultDataTable.Rows.Count != 0)
                        RowIndex++;
                    OrderCost = CalculateCost();
                    DisplayIndex = 0;
                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        DisplayIndex = 0;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrontName"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Patina"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Height"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Width"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Count"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Weight"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["FrontPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["InsetPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Cost"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Rate"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Notes"].ToString());
                        Cell1.CellStyle = SimpleCS;

                        RowIndex++;
                    }
                    //if (FrontsResultDataTable.Rows.Count != 0)
                    //    RowIndex++;

                    DisplayIndex = 0;
                    //декор
                    for (int c = 0; c < DecorCatalogOrder.DecorProductsCount; c++)
                    {
                        if (DecorResultDataTable[c].Rows.Count == 0)
                            continue;
                        //int ColumnCount = 0;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(0);
                        Cell1.SetCellValue("Наименование");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(1);
                        Cell1.SetCellValue("Цвет");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(2);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(3);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(4);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(5);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(6);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;


                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(7);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(8);
                        Cell1.SetCellValue("Высота");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(9);
                        Cell1.SetCellValue("Длина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(10);
                        Cell1.SetCellValue("Ширина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(11);
                        Cell1.SetCellValue("Кол-во");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(12);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(13);
                        Cell1.SetCellValue("Цена");
                        Cell1.CellStyle = SimpleHeaderCS;

                        //Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        //Cell1.SetCellValue(string.Empty);
                        //Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(15);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(16);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;
                        RowIndex++;
                        RowIndex++;


                        DisplayIndex = 0;
                        //вывод заказов декора в excel
                        for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                        {
                            DisplayIndex = 0;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Name"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Color"] != DBNull.Value)
                                Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Color"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Patina"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Height"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Height"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Length"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Length"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Width"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Width"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Count"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Weight"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Weight"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Price"]));
                            Cell1.CellStyle = SimpleDecCS;
                            //Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            //Cell1.SetCellValue(string.Empty);
                            //Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Cost"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Rate"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Notes"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            RowIndex++;
                        }
                        //RowIndex++;
                    }
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Итого, EUR: " + Decimal.Round(OrderCost, 2, MidpointRounding.AwayFromZero));
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                }

                RowIndex++;
                RowIndex++;


            }
            RowIndex++;
            RowIndex++;

            ClientReport.CreateReport(
                ref hssfworkbook, ref sheet1,
                NewMegaOrders, OrderNumbers, MainOrdersIDs, ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost,
                TransportCost, AdditionalCost, TotalCost, CurrencyTypeID, TotalWeight, RowIndex);

            string FileName = ClientsDataTable.Select("ClientName = '" + ClientName + "'")[0]["Login"].ToString() +
                    " - №" + string.Join(",", OrderNumbers);
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

            ClearReport();
            return file.FullName;
        }

        public void Report(ref HSSFWorkbook hssfworkbook, int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost,
            decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight, bool IsSample)
        {
            ClearReport();

            ClientReport = new Report(ref DecorCatalogOrder, ref FrontsCalculate);

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 13;
            HeaderF1.Boldweight = 13 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 12;
            HeaderF2.Boldweight = 12 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 9;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle SimpleDecCS = hssfworkbook.CreateCellStyle();
            SimpleDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            SimpleDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS = hssfworkbook.CreateCellStyle();
            ReportCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS.SetFont(HeaderF1);

            HSSFCellStyle HeaderWithoutBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithoutBorderCS.SetFont(HeaderF2);

            HSSFCellStyle HeaderWithBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithBorderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.TopBorderColor = HSSFColor.BLACK.index;
            //HeaderWithBorderCS.WrapText = true;
            HeaderWithBorderCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            //HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            //HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Подробный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 25 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 18 * 256);
            sheet1.SetColumnWidth(8, 13 * 256);
            sheet1.SetColumnWidth(9, 10 * 256);
            sheet1.SetColumnWidth(10, 9 * 256);
            sheet1.SetColumnWidth(11, 12 * 256);
            sheet1.SetColumnWidth(12, 12 * 256);
            sheet1.SetColumnWidth(13, 13 * 256);
            sheet1.SetColumnWidth(14, 12 * 256);
            sheet1.SetColumnWidth(15, 10 * 256);

            decimal OrderCost = 0;

            int RowIndex = 0;

            string Currency = string.Empty;

            CurrencyTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();

            ClientName = GetClientName(MainOrdersIDs[0]);

            HSSFCell Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("Клиент:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(ClientName);
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("№ заказа:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(string.Join(",", OrderNumbers));
            Cell1.CellStyle = HeaderWithoutBorderCS;
            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = HeaderWithoutBorderCS;
            }

            RowIndex++;
            for (int i = 0; i < MainOrdersIDs.Count(); i++)
            {
                OrderCost = 0;

                if (FilterOrders(MainOrdersIDs[i], IsSample))
                {
                    FillFronts(MainOrdersIDs[i]);
                    FillDecor(MainOrdersIDs[i]);

                    RowIndex++;

                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("№" + GetOrderNumber(MainOrdersIDs[i]) + "-" + MainOrdersIDs[i].ToString());
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                    RowIndex++;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Примечание к заказу: " + GetMainOrderNotes(MainOrdersIDs[i]));
                    Cell1.CellStyle = HeaderWithoutBorderCS;

                    int DisplayIndex = 0;
                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Выс.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Шир.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Кол.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за вставку");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;

                        RowIndex++;
                    }

                    if (FrontsResultDataTable.Rows.Count != 0)
                        RowIndex++;
                    OrderCost = CalculateCost();
                    DisplayIndex = 0;
                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        DisplayIndex = 0;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrontName"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Patina"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Height"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Width"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Count"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        {
                            decimal Weight = Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Weight"]);
                            //decimal Weight = 0;
                            //if (Width != -1)
                            //    Weight = Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Weight"]) +
                            //        Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Height"]) * Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Width"]) *
                            //        Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Count"]) / 1000000 * Convert.ToDecimal(0.7);
                            //else
                            //    Weight = Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Weight"]);

                            Weight = Decimal.Round(Weight, 3, MidpointRounding.AwayFromZero);
                            Cell1.SetCellValue(Convert.ToDouble(Weight));
                        }
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["FrontPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["InsetPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Cost"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Rate"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Notes"].ToString());
                        Cell1.CellStyle = SimpleCS;

                        RowIndex++;
                    }
                    //if (FrontsResultDataTable.Rows.Count != 0)
                    //    RowIndex++;

                    DisplayIndex = 0;
                    //декор
                    for (int c = 0; c < DecorCatalogOrder.DecorProductsCount; c++)
                    {
                        if (DecorResultDataTable[c].Rows.Count == 0)
                            continue;
                        //int ColumnCount = 0;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(0);
                        Cell1.SetCellValue("Наименование");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(1);
                        Cell1.SetCellValue("Цвет");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(2);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(3);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(4);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(5);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(6);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;


                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(7);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(8);
                        Cell1.SetCellValue("Высота");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(9);
                        Cell1.SetCellValue("Длина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(10);
                        Cell1.SetCellValue("Ширина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(11);
                        Cell1.SetCellValue("Кол-во");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(12);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(13);
                        Cell1.SetCellValue("Цена");
                        Cell1.CellStyle = SimpleHeaderCS;

                        //Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        //Cell1.SetCellValue(string.Empty);
                        //Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(15);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(16);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;
                        RowIndex++;
                        RowIndex++;


                        DisplayIndex = 0;
                        //вывод заказов декора в excel
                        for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                        {
                            DisplayIndex = 0;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Name"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Color"] != DBNull.Value)
                                Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Color"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Patina"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Height"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Height"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Length"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Length"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Width"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Width"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Count"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            decimal Weight = Decimal.Round(Convert.ToDecimal(DecorResultDataTable[c].Rows[x]["Weight"]), 3, MidpointRounding.AwayFromZero);
                            Cell1.SetCellValue(Convert.ToDouble(Weight));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Price"]));
                            Cell1.CellStyle = SimpleDecCS;
                            //Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            //Cell1.SetCellValue(string.Empty);
                            //Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Cost"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Rate"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Notes"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            RowIndex++;
                        }
                        //RowIndex++;
                    }
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Итого, EUR: " + Decimal.Round(OrderCost, 2, MidpointRounding.AwayFromZero));
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                }

                RowIndex++;
                RowIndex++;


            }
            RowIndex++;
            RowIndex++;

            ClientReport.CreateReport(
                ref hssfworkbook, ref sheet1,
                NewMegaOrders, OrderNumbers, MainOrdersIDs, ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost,
                TransportCost, AdditionalCost, TotalCost, CurrencyTypeID, TotalWeight, RowIndex, IsSample);

        }

        public void Report(ref HSSFWorkbook hssfworkbook, int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost,
            decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight)
        {
            ClearReport();

            ClientReport = new Report(ref DecorCatalogOrder, ref FrontsCalculate);

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 13;
            HeaderF1.Boldweight = 13 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 12;
            HeaderF2.Boldweight = 12 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 9;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle SimpleDecCS = hssfworkbook.CreateCellStyle();
            SimpleDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            SimpleDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS = hssfworkbook.CreateCellStyle();
            ReportCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS.SetFont(HeaderF1);

            HSSFCellStyle HeaderWithoutBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithoutBorderCS.SetFont(HeaderF2);

            HSSFCellStyle HeaderWithBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithBorderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.TopBorderColor = HSSFColor.BLACK.index;
            //HeaderWithBorderCS.WrapText = true;
            HeaderWithBorderCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            //HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            //HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Подробный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 25 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 18 * 256);
            sheet1.SetColumnWidth(8, 13 * 256);
            sheet1.SetColumnWidth(9, 10 * 256);
            sheet1.SetColumnWidth(10, 9 * 256);
            sheet1.SetColumnWidth(11, 12 * 256);
            sheet1.SetColumnWidth(12, 12 * 256);
            sheet1.SetColumnWidth(13, 13 * 256);
            sheet1.SetColumnWidth(14, 12 * 256);
            sheet1.SetColumnWidth(15, 10 * 256);

            decimal OrderCost = 0;

            int RowIndex = 0;

            string Currency = string.Empty;

            CurrencyTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();

            ClientName = GetClientName(MainOrdersIDs[0]);

            HSSFCell Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("Клиент:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(ClientName);
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
            Cell1.SetCellValue("№ заказа:");
            Cell1.CellStyle = HeaderWithoutBorderCS;
            Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(1);
            Cell1.SetCellValue(string.Join(",", OrderNumbers));
            Cell1.CellStyle = HeaderWithoutBorderCS;
            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(RowIndex++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = HeaderWithoutBorderCS;
            }

            RowIndex++;
            for (int i = 0; i < MainOrdersIDs.Count(); i++)
            {
                OrderCost = 0;

                if (FilterOrders(MainOrdersIDs[i]))
                {
                    FillFronts(MainOrdersIDs[i]);
                    FillDecor(MainOrdersIDs[i]);

                    RowIndex++;

                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("№" + GetOrderNumber(MainOrdersIDs[i]) + "-" + MainOrdersIDs[i].ToString());
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                    RowIndex++;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Примечание к заказу: " + GetMainOrderNotes(MainOrdersIDs[i]));
                    Cell1.CellStyle = HeaderWithoutBorderCS;

                    int DisplayIndex = 0;
                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет профиля-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-1");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Тип наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цвет наполнителя-2");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Выс.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Шир.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Кол.");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за фасад");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Цена за вставку");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;

                        RowIndex++;
                    }

                    if (FrontsResultDataTable.Rows.Count != 0)
                        RowIndex++;
                    OrderCost = CalculateCost();
                    DisplayIndex = 0;
                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        DisplayIndex = 0;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrontName"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["FrameColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor1"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetType2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["InsetColor2"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Patina"].ToString());
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Height"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Width"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x]["Count"]));
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        {
                            decimal Weight = Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Weight"]);
                            //decimal Weight = 0;
                            //if (Width != -1)
                            //    Weight = Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Weight"]) +
                            //        Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Height"]) * Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Width"]) *
                            //        Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Count"]) / 1000000 * Convert.ToDecimal(0.7);
                            //else
                            //    Weight = Convert.ToDecimal(FrontsResultDataTable.Rows[x]["Weight"]);

                            Weight = Decimal.Round(Weight, 3, MidpointRounding.AwayFromZero);
                            Cell1.SetCellValue(Convert.ToDouble(Weight));
                        }
                        Cell1.CellStyle = SimpleCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["FrontPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["InsetPrice"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Cost"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x]["Rate"]));
                        Cell1.CellStyle = SimpleDecCS;
                        Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(FrontsResultDataTable.Rows[x]["Notes"].ToString());
                        Cell1.CellStyle = SimpleCS;

                        RowIndex++;
                    }
                    //if (FrontsResultDataTable.Rows.Count != 0)
                    //    RowIndex++;

                    DisplayIndex = 0;
                    //декор
                    for (int c = 0; c < DecorCatalogOrder.DecorProductsCount; c++)
                    {
                        if (DecorResultDataTable[c].Rows.Count == 0)
                            continue;
                        //int ColumnCount = 0;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(0);
                        Cell1.SetCellValue("Наименование");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(1);
                        Cell1.SetCellValue("Цвет");
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(2);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(3);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(4);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(5);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(6);
                        Cell1.SetCellValue(string.Empty);
                        Cell1.CellStyle = SimpleHeaderCS;


                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(7);
                        Cell1.SetCellValue("Патина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(8);
                        Cell1.SetCellValue("Высота");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(9);
                        Cell1.SetCellValue("Длина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(10);
                        Cell1.SetCellValue("Ширина");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(11);
                        Cell1.SetCellValue("Кол-во");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(12);
                        Cell1.SetCellValue("Вес");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(13);
                        Cell1.SetCellValue("Цена");
                        Cell1.CellStyle = SimpleHeaderCS;

                        //Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        //Cell1.SetCellValue(string.Empty);
                        //Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(14);
                        Cell1.SetCellValue("Стоимость");
                        Cell1.CellStyle = SimpleHeaderCS;

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(15);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(16);
                        Cell1.SetCellValue("Примечание");
                        Cell1.CellStyle = SimpleHeaderCS;
                        RowIndex++;
                        RowIndex++;


                        DisplayIndex = 0;
                        //вывод заказов декора в excel
                        for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                        {
                            DisplayIndex = 0;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Name"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Color"] != DBNull.Value)
                                Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Color"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(string.Empty);
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Patina"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Height"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Height"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Length"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Length"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            if (DecorResultDataTable[c].Rows[x]["Width"] != DBNull.Value)
                                Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Width"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x]["Count"]));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            decimal Weight = Decimal.Round(Convert.ToDecimal(DecorResultDataTable[c].Rows[x]["Weight"]), 3, MidpointRounding.AwayFromZero);
                            Cell1.SetCellValue(Convert.ToDouble(Weight));
                            Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Price"]));
                            Cell1.CellStyle = SimpleDecCS;
                            //Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            //Cell1.SetCellValue(string.Empty);
                            //Cell1.CellStyle = SimpleCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Cost"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x]["Rate"]));
                            Cell1.CellStyle = SimpleDecCS;
                            Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                            Cell1.SetCellValue(DecorResultDataTable[c].Rows[x]["Notes"].ToString());
                            Cell1.CellStyle = SimpleCS;
                            RowIndex++;
                        }
                        //RowIndex++;
                    }
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                    Cell1.SetCellValue("Итого, EUR: " + Decimal.Round(OrderCost, 2, MidpointRounding.AwayFromZero));
                    Cell1.CellStyle = HeaderWithoutBorderCS;
                }

                RowIndex++;
                RowIndex++;


            }
            RowIndex++;
            RowIndex++;

            ClientReport.CreateReport(
                ref hssfworkbook, ref sheet1,
                NewMegaOrders, OrderNumbers, MainOrdersIDs, ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost,
                TransportCost, AdditionalCost, TotalCost, CurrencyTypeID, TotalWeight, RowIndex);

        }

        public void ClearReport()
        {
            FrontsResultDataTable.Clear();

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorResultDataTable[i].Clear();
            }
        }

    }

    public class InvoiceFrontsReportToDBF
    {
        decimal TransportCost = 0;
        decimal AdditionalCost = 0;
        decimal PaymentRate = 1;
        int ClientID = 0;
        //int CurrencyCode = 0;
        string ProfilCurrencyCode = "0";
        string TPSCurrencyCode = "0";
        string UNN = string.Empty;

        DataTable DecorInvNumbersDT = null;
        DataTable CurrencyTypesDT;
        DataTable ProfilFrontsOrdersDataTable = null;
        DataTable TPSFrontsOrdersDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        DataTable MeasuresDataTable = null;
        DataTable FactoryDataTable = null;
        DataTable GridSizesDataTable = null;
        DataTable FrontsConfigDataTable = null;
        DataTable DecorConfigDataTable = null;
        DataTable TechStoreDataTable = null;
        DataTable InsetPriceDataTable = null;
        DataTable AluminiumFrontsDataTable = null;

        public DataTable ProfilReportDataTable = null;
        public DataTable TPSReportDataTable = null;

        public InvoiceFrontsReportToDBF()
        {
            Create();
            CreateReportDataTables();
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
            ProfilFrontsOrdersDataTable = new DataTable();
            TPSFrontsOrdersDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            InsetPriceDataTable = new DataTable();
            AluminiumFrontsDataTable = new DataTable();

            string SelectCommand = "SELECT * FROM CurrencyTypes";
            CurrencyTypesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDT);
            }

            SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
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
            SelectCommand = @"SELECT * FROM InsetTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            MeasuresDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }

            InsetPriceDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetPrice",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(InsetPriceDataTable);
            }

            AluminiumFrontsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM AluminiumFronts",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(AluminiumFrontsDataTable);
            }

            FactoryDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryDataTable);
            }

            GridSizesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM GridSizes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(GridSizesDataTable);
            }

            FrontsConfigDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsConfig",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(FrontsConfigDataTable);
            //}
            FrontsConfigDataTable = TablesManager.FrontsConfigDataTable;

            DecorConfigDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorConfig",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDataTable);
            //}
            DecorConfigDataTable = TablesManager.DecorConfigDataTableAll;

            TechStoreDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStore",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(TechStoreDataTable);
            //}
            TechStoreDataTable = TablesManager.TechStoreDataTable;
        }

        public void ClearReport()
        {
            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();
        }

        private void CreateReportDataTables()
        {
            DecorInvNumbersDT = new DataTable();
            DecorInvNumbersDT.Columns.Add(new DataColumn("FrontsOrdersID", Type.GetType("System.Int32")));
            DecorInvNumbersDT.Columns.Add(new DataColumn("DecorInvNumber", Type.GetType("System.String")));
            DecorInvNumbersDT.Columns.Add(new DataColumn("DecorAccountingName", Type.GetType("System.String")));
            DecorInvNumbersDT.Columns.Add(new DataColumn("FactoryID", Type.GetType("System.Int32")));

            ProfilReportDataTable = new DataTable();
            ProfilReportDataTable.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("OriginalPrice", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("IsNonStandard", Type.GetType("System.Boolean")));
            ProfilReportDataTable.Columns.Add(new DataColumn("NonStandardMargin", Type.GetType("System.Decimal")));

            TPSReportDataTable = ProfilReportDataTable.Clone();
        }

        public string GetFrontName(int FrontID)
        {
            DataRow[] Row = FrontsDataTable.Select("FrontID = " + FrontID);

            return Row[0]["FrontName"].ToString();
        }

        private void SplitTables(DataTable FrontsOrdersDataTable, ref DataTable ProfilDT, ref DataTable TPSDT)
        {
            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersDataTable.Rows[i]["FrontConfigID"].ToString());

                if (Convert.ToDateTime(FrontsOrdersDataTable.Rows[i]["CreateDateTime"]) < new DateTime(2019, 10, 01))
                {
                    if (Rows[0]["AreaID"].ToString() == "1")//profil
                        ProfilDT.ImportRow(FrontsOrdersDataTable.Rows[i]);

                    if (Rows[0]["AreaID"].ToString() == "2")//tps
                        TPSDT.ImportRow(FrontsOrdersDataTable.Rows[i]);
                }
                else
                {
                    if (Rows[0]["FactoryID"].ToString() == "1")//profil
                        ProfilDT.ImportRow(FrontsOrdersDataTable.Rows[i]);

                    if (Rows[0]["FactoryID"].ToString() == "2")//tps
                        TPSDT.ImportRow(FrontsOrdersDataTable.Rows[i]);
                }
            }
        }

        //ALUMINIUM
        public int IsAluminium(int FrontID)
        {
            DataRow[] Row = FrontsDataTable.Select("FrontID = " + FrontID);

            if (Row.Count() > 0 && Row[0]["FrontName"].ToString()[0] == 'Z')
            {
                return Convert.ToInt32(Row[0]["FrontID"]);
            }

            return -1;
        }

        //ALUMINIUM
        public int IsAluminium(DataRow FrontsOrdersRow)
        {
            string str = FrontsOrdersRow["FrontID"].ToString();
            DataRow[] Row = FrontsDataTable.Select("FrontID = " + FrontsOrdersRow["FrontID"].ToString());

            if (Row.Count() > 0 && Row[0]["FrontName"].ToString()[0] == 'Z')
            {
                str = Row[0]["FrontName"].ToString();
                return Convert.ToInt32(Row[0]["FrontID"]);
            }

            return -1;
        }

        private void GetGlassMarginAluminium(DataRow FrontsOrdersRow, ref int GlassMarginHeight, ref int GlassMarginWidth)
        {
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + Convert.ToInt32(FrontsOrdersRow["FrontID"]));
            if (Rows.Count() > 0)
            {
                if (Rows[0]["InsetHeightAdmission"] != DBNull.Value)
                    GlassMarginHeight = Convert.ToInt32(Rows[0]["InsetHeightAdmission"]);
                if (Rows[0]["InsetWidthAdmission"] != DBNull.Value)
                    GlassMarginWidth = Convert.ToInt32(Rows[0]["InsetWidthAdmission"]);
            }
            //DataRow[] Rows = AluminiumFrontsDataTable.Select("FrontID = " + IsAluminium(FrontsOrdersRow));
            //if (Rows.Count() > 0)
            //{
            //    GlassMarginHeight = Convert.ToInt32(Rows[0]["GlassMarginHeight"]);
            //    GlassMarginWidth = Convert.ToInt32(Rows[0]["GlassMarginWidth"]);
            //}
        }

        private decimal GetJobPriceAluminium(DataRow FrontsOrdersRow)
        {
            DataRow[] Rows = AluminiumFrontsDataTable.Select("FrontID = " + IsAluminium(FrontsOrdersRow));
            if (Rows.Count() == 0)
                return 0;
            return Convert.ToDecimal(Rows[0]["JobPrice"]);
        }

        private decimal FrontsPriceAluminium(DataRow FrontsOrdersRow)
        {
            decimal Price = 0;

            DataRow[] Rows = AluminiumFrontsDataTable.Select("FrontID = " + IsAluminium(FrontsOrdersRow));
            if (Rows.Count() > 0)
            {
                Price = Convert.ToDecimal(Rows[0]["ProfilPrice"]);
            }
            FrontsOrdersRow["FrontPrice"] = Price;

            return Price;
        }

        private decimal InsetPriceAluminium(DataRow FrontsOrdersRow)
        {
            decimal Price = 0;

            DataRow[] Rows = InsetPriceDataTable.Select("InsetTypeID = " + FrontsOrdersRow["InsetColorID"].ToString());

            if (Rows.Count() > 0)
                Price = Convert.ToDecimal(Rows[0]["GlassZXPrice"]);
            else
                Price = 0;

            FrontsOrdersRow["InsetPrice"] = Price;

            return Price;
        }

        public decimal GetFrontCostAluminium(DataRow FrontsOrdersRow)
        {
            decimal Cost = 0;
            decimal Perimeter = 0;
            decimal GlassSquare = 0;

            int MarginHeight = 0;
            int MarginWidth = 0;

            decimal GlassPrice = InsetPriceAluminium(FrontsOrdersRow);
            decimal JobPrice = GetJobPriceAluminium(FrontsOrdersRow);
            decimal ProfilPrice = FrontsPriceAluminium(FrontsOrdersRow);

            GetGlassMarginAluminium(FrontsOrdersRow, ref MarginHeight, ref MarginWidth);

            decimal Height = Convert.ToInt32(FrontsOrdersRow["Height"]);
            decimal Width = Convert.ToInt32(FrontsOrdersRow["Width"]);
            decimal Count = Convert.ToInt32(FrontsOrdersRow["Count"]);


            Perimeter = Decimal.Round((Height * 2 + Width * 2) / 1000 * Count, 3, MidpointRounding.AwayFromZero);
            GlassSquare = Decimal.Round((Height - MarginHeight) * (Width - MarginWidth) / 1000000, 3, MidpointRounding.AwayFromZero);

            GlassSquare = GlassSquare * Count;
            Cost = Decimal.Round(JobPrice * Count + GlassPrice * GlassSquare + Perimeter * ProfilPrice, 3, MidpointRounding.AwayFromZero);

            Cost = Cost * 100 / 120;

            //FrontsOrdersRow["InsetPrice"] = 0;
            //FrontsOrdersRow["Cost"] = Cost;
            //FrontsOrdersRow["Square"] = (Height * Width * Count) / 1000000;
            //FrontsOrdersRow["FrontPrice"] = Cost / Convert.ToDecimal(FrontsOrdersRow["Square"]);

            return Cost;
        }

        public decimal GetAluminiumWeight(DataRow FrontsOrdersRow, bool WithGlass)
        {
            DataRow[] Row = AluminiumFrontsDataTable.Select("FrontID = " + IsAluminium(FrontsOrdersRow));
            if (Row.Count() == 0)
                return 0;
            decimal FrontHeight = Convert.ToDecimal(FrontsOrdersRow["Height"]);
            decimal FrontWidth = Convert.ToDecimal(FrontsOrdersRow["Width"]);
            decimal Count = Convert.ToInt32(FrontsOrdersRow["Count"]);

            int MarginHeight = 0;
            int MarginWidth = 0;

            GetGlassMarginAluminium(FrontsOrdersRow, ref MarginHeight, ref MarginWidth);

            DataRow[] FrontsConfigRow = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString());
            if (FrontsConfigRow.Count() == 0)
                return 0;
            int ProfileID = Convert.ToInt32(FrontsConfigRow[0]["ProfileID"]);
            decimal ProfileWeight = 0;
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + ProfileID);
            if (Rows.Count() > 0)
            {
                if (Rows[0]["Weight"] != DBNull.Value)
                    ProfileWeight = Convert.ToDecimal(Rows[0]["Weight"]);
            }

            decimal GlassSquare = 0;

            if (FrontsOrdersRow["InsetColorID"].ToString() != "3946")//если не СТЕКЛО КЛИЕНТА
                GlassSquare = Decimal.Round((FrontHeight - MarginHeight) * (FrontWidth - MarginWidth) / 1000000, 3, MidpointRounding.AwayFromZero);

            decimal GlassWeight = GlassSquare * 10;

            decimal ResultProfileWeight = Decimal.Round((FrontWidth * 2 + FrontHeight * 2) / 1000, 3, MidpointRounding.AwayFromZero) * ProfileWeight;

            if (WithGlass)
                return (ResultProfileWeight + GlassWeight) * Count;
            else
                return (ResultProfileWeight) * Count;
        }

        private string GetGlassInvNumber(int FrontConfigID, ref int FactoryID, ref string AccountingName)
        {
            int FrontID = 0;
            int InsetTypeID = 0;
            int InsetColorID = 0;
            DataRow[] FRows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontConfigID);
            if (FRows.Count() == 0)
                return string.Empty;

            FrontID = Convert.ToInt32(FRows[0]["FrontID"]);
            InsetTypeID = Convert.ToInt32(FRows[0]["InsetTypeID"]);
            InsetColorID = Convert.ToInt32(FRows[0]["InsetColorID"]);
            if (IsAluminium(FrontID) != -1)
                return string.Empty;
            DataRow[] DRows = DecorConfigDataTable.Select("DecorID  = " + InsetColorID);
            if (DRows.Count() == 0)
                return string.Empty;
            FactoryID = Convert.ToInt32(DRows[0]["FactoryID"]);
            AccountingName = DRows[0]["AccountingName"].ToString();
            return DRows[0]["InvNumber"].ToString();
        }

        private string GetGridInvNumber(int FrontConfigID, ref int FactoryID, ref string AccountingName)
        {
            int InsetTypeID = 0;
            int InsetColorID = 0;
            DataRow[] FRows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontConfigID);
            if (FRows.Count() == 0)
                return string.Empty;
            InsetTypeID = Convert.ToInt32(FRows[0]["InsetTypeID"]);
            InsetColorID = Convert.ToInt32(FRows[0]["InsetColorID"]);
            DataRow[] DRows = DecorConfigDataTable.Select("DecorID  = " + InsetTypeID + " AND ColorID=" + InsetColorID);
            if (DRows.Count() == 0)
                return string.Empty;
            FactoryID = Convert.ToInt32(DRows[0]["FactoryID"]);
            AccountingName = DRows[0]["AccountingName"].ToString();
            return DRows[0]["InvNumber"].ToString();
        }

        public void GetMegaOrderInfo(int MainOrderID)
        {
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, PaymentRate, ClientID, CurrencyTypeID FROM NewMegaOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM NewMainOrders WHERE MainOrderID = " + MainOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int CurrencyTypeID = 1;
                        if (DT.Rows[0]["TransportCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["TransportCost"].ToString(), out TransportCost);
                        if (DT.Rows[0]["AdditionalCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["AdditionalCost"].ToString(), out AdditionalCost);
                        //if (DT.Rows[0]["PaymentRate"] != DBNull.Value)
                        //    decimal.TryParse(DT.Rows[0]["PaymentRate"].ToString(), out PaymentRate);
                        if (DT.Rows[0]["ClientID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["ClientID"].ToString(), out ClientID);
                        if (DT.Rows[0]["CurrencyTypeID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["CurrencyTypeID"].ToString(), out CurrencyTypeID);

                        DataRow[] rows = CurrencyTypesDT.Select("CurrencyTypeID = " + CurrencyTypeID);
                        if (rows.Count() > 0)
                        {
                            ProfilCurrencyCode = rows[0]["CurrencyCode"].ToString();
                            TPSCurrencyCode = rows[0]["TPSCurrencyCode"].ToString();
                        }
                    }
                }
            }
            SelectCommand = "SELECT UNN FROM Clients WHERE ClientID = " + ClientID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["UNN"] != DBNull.Value)
                            UNN = DT.Rows[0]["UNN"].ToString();
                    }
                }
            }
        }

        public void GetMegaOrderInfo1(int MegaOrderID)
        {
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, PaymentRate, ClientID, CurrencyTypeID FROM NewMegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int CurrencyTypeID = 1;
                        if (DT.Rows[0]["TransportCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["TransportCost"].ToString(), out TransportCost);
                        if (DT.Rows[0]["AdditionalCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["AdditionalCost"].ToString(), out AdditionalCost);
                        //if (DT.Rows[0]["PaymentRate"] != DBNull.Value)
                        //    decimal.TryParse(DT.Rows[0]["PaymentRate"].ToString(), out PaymentRate);
                        if (DT.Rows[0]["ClientID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["ClientID"].ToString(), out ClientID);
                        if (DT.Rows[0]["CurrencyTypeID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["CurrencyTypeID"].ToString(), out CurrencyTypeID);

                        DataRow[] rows = CurrencyTypesDT.Select("CurrencyTypeID = " + CurrencyTypeID);
                        if (rows.Count() > 0)
                        {
                            ProfilCurrencyCode = rows[0]["CurrencyCode"].ToString();
                            TPSCurrencyCode = rows[0]["TPSCurrencyCode"].ToString();
                        }
                    }
                }
            }
            SelectCommand = "SELECT UNN FROM Clients WHERE ClientID = " + ClientID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["UNN"] != DBNull.Value)
                            UNN = DT.Rows[0]["UNN"].ToString();
                    }
                }
            }
        }

        private int GetMeasureType(int FrontConfigID)
        {
            return Convert.ToInt32(FrontsConfigDataTable.Select("FrontConfigID = " + FrontConfigID.ToString())[0]["MeasureID"]);
        }

        private decimal GetInsetSquare(int FrontID, int Height, int Width)
        {
            decimal GridHeight = 0;
            decimal GridWidth = 0;
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + FrontID);
            if (Rows.Count() > 0)
            {
                if (Rows[0]["InsetHeightAdmission"] != DBNull.Value)
                    GridHeight = Height - Convert.ToInt32(Rows[0]["InsetHeightAdmission"]);
                if (Rows[0]["InsetWidthAdmission"] != DBNull.Value)
                    GridWidth = Width - Convert.ToInt32(Rows[0]["InsetWidthAdmission"]);

                if (FrontID == 3729)
                {
                    return Decimal.Round(Convert.ToInt32(Rows[0]["InsetHeightAdmission"]) * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
                }
            }
            return Decimal.Round(GridHeight * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
        }

        private decimal GetInsetSquare(DataRow FrontsOrdersRow)
        {
            decimal GridHeight = 0;
            decimal GridWidth = 0;
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + Convert.ToInt32(FrontsOrdersRow["FrontID"]));
            if (Rows.Count() > 0)
            {
                if (Rows[0]["InsetHeightAdmission"] != DBNull.Value)
                    GridHeight = Convert.ToInt32(FrontsOrdersRow["Height"]) - Convert.ToInt32(Rows[0]["InsetHeightAdmission"]);
                if (Rows[0]["InsetWidthAdmission"] != DBNull.Value)
                    GridWidth = Convert.ToInt32(FrontsOrdersRow["Width"]) - Convert.ToInt32(Rows[0]["InsetWidthAdmission"]);

                if (Convert.ToInt32(FrontsOrdersRow["FrontID"]) == 3729)
                {
                    return Decimal.Round(Convert.ToInt32(Rows[0]["InsetHeightAdmission"]) * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
                }
            }
            return Decimal.Round(GridHeight * GridWidth / 1000000, 3, MidpointRounding.AwayFromZero);
        }

        private decimal GetNonStandardMargin(int FrontConfigID)
        {
            DataRow[] Rows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontConfigID);

            return Convert.ToDecimal(Rows[0]["ZOVNonStandMargin"]);
        }

        private void GetSimpleFronts(DataTable OrdersDataTable, DataTable ReportDataTable, bool IsNonStandard)
        {
            string IsNonStandardFilter = "IsNonStandard=0";
            string DecorAccountingName = string.Empty;
            string DecorInvNumber = string.Empty;
            string AccountingName = OrdersDataTable.Rows[0]["AccountingName"].ToString();
            string InvNumber = OrdersDataTable.Rows[0]["InvNumber"].ToString();
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            DataTable Fronts = new DataTable();
            if (IsNonStandard)
                IsNonStandardFilter = "IsNonStandard=1";
            using (DataView DV = new DataView(OrdersDataTable))
            {
                DV.RowFilter = IsNonStandardFilter;
                Fronts = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Fronts.Rows.Count; i++)
            {
                decimal SolidCount = 0;
                decimal SolidCost = 0;
                decimal WithTransportSolidCost = 0;
                decimal SolidWeight = 0;

                decimal FilenkaCount = 0;
                decimal FilenkaCost = 0;
                decimal WithTransportFilenkaCost = 0;
                decimal FilenkaWeight = 0;

                decimal VitrinaCount = 0;
                decimal VitrinaCost = 0;
                decimal WithTransportVitrinaCost = 0;
                decimal VitrinaWeight = 0;

                decimal LuxMegaCount = 0;
                decimal LuxMegaCost = 0;
                decimal WithTransportLuxMegaCost = 0;
                decimal LuxMegaWeight = 0;

                object NonStandardMargin = 0;
                IsNonStandardFilter = " AND IsNonStandard=0";
                if (IsNonStandard)
                    IsNonStandardFilter = " AND IsNonStandard=1";
                //ГЛУХИЕ, БЕЗ ВСТАВКИ, РЕШЕТКА ОВАЛ
                DataRow[] rows = InsetTypesDataTable.Select("InsetTypeID=-1 OR GroupID = 3 OR GroupID = 4");
                string filter = string.Empty;
                foreach (DataRow item in rows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = " AND NOT (FrontID IN (3728,3731,3732,3739,3740,3741,3744,3745,3746) OR InsetTypeID IN (28961,3653,3654,3655)) AND (FrontID = 3729 OR InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + "))";
                DataRow[] Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    decimal DeductibleCost = 0;
                    decimal DeductibleCount = 0;
                    decimal DeductibleWeight = 0;
                    if (Convert.ToInt32(Fronts.Rows[i]["FrontID"]) == 3729)//РЕШЕТКА ОВАЛ
                    {
                        int FactoryID = 0;
                        DecorInvNumber = GetGridInvNumber(Convert.ToInt32(Rows[r]["FrontConfigID"]), ref FactoryID, ref DecorAccountingName);
                        if (DecorInvNumber.Length > 0)
                        {
                            DataRow NewRow = DecorInvNumbersDT.NewRow();
                            NewRow["FrontsOrdersID"] = Convert.ToInt32(Rows[r]["FrontsOrdersID"]);
                            NewRow["FactoryID"] = FactoryID;
                            NewRow["DecorAccountingName"] = DecorAccountingName;
                            NewRow["DecorInvNumber"] = DecorInvNumber;
                            DecorInvNumbersDT.Rows.Add(NewRow);
                            DeductibleWeight = GetInsetWeight(Rows[r]);

                            int MarginHeight = 0;
                            int MarginWidth = 0;
                            GetGlassMarginAluminium(Rows[r], ref MarginHeight, ref MarginWidth);
                            decimal InsetSquare = MarginHeight * (Convert.ToDecimal(Rows[r]["Width"]) - MarginWidth) / 1000000;
                            InsetSquare = Decimal.Round(InsetSquare, 3, MidpointRounding.AwayFromZero);
                            DeductibleCount = InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                            DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                            DeductibleWeight = Decimal.Round(DeductibleCount * DeductibleWeight, 3, MidpointRounding.AwayFromZero);
                        }
                    }
                    SolidCount += Convert.ToDecimal(Rows[r]["Square"]);
                    SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    DeductibleCost = Math.Ceiling(DeductibleCost / 0.01m) * 0.01m;
                    SolidCost = SolidCost - DeductibleCost;
                    WithTransportSolidCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]) - DeductibleCost;

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    SolidWeight += Convert.ToDecimal(FrontWeight + InsetWeight - DeductibleWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //АППЛИКАЦИИ
                filter = " AND (FrontID IN (3728,3731,3732,3739,3740,3741,3744,3745,3746) OR InsetTypeID IN (28961,3653,3654,3655))";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (Convert.ToInt32(Rows[r]["FrontID"]) == 3728 || Convert.ToInt32(Rows[r]["FrontID"]) == 3731 || Convert.ToInt32(Rows[r]["FrontID"]) == 3732 ||
                        Convert.ToInt32(Rows[r]["FrontID"]) == 3739 || Convert.ToInt32(Rows[r]["FrontID"]) == 3740 || Convert.ToInt32(Rows[r]["FrontID"]) == 3741 ||
                        Convert.ToInt32(Rows[r]["FrontID"]) == 3744 || Convert.ToInt32(Rows[r]["FrontID"]) == 3745 || Convert.ToInt32(Rows[r]["FrontID"]) == 3746)
                    {
                        SolidCount += Convert.ToDecimal(Rows[r]["Square"]);
                        SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) + 5 * Convert.ToDecimal(Rows[r]["Count"]);

                        WithTransportSolidCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        SolidWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    }
                    else if (Convert.ToInt32(Rows[r]["FrontID"]) == 3415 || Convert.ToInt32(Rows[r]["FrontID"]) == 28922)
                    {
                        FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                        FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) + 5 * Convert.ToDecimal(Rows[r]["Count"]);

                        WithTransportFilenkaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        FilenkaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    }
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ФИЛЕНКА
                filter = " AND InsetTypeID IN (2069,2070,2071,2073,2075,2077,2233,3644,29043,29531)";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    WithTransportFilenkaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    FilenkaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ВИТРИНЫ, РЕШЕТКИ, СТЕКЛО
                filter = " AND InsetTypeID IN (1,2,685,686,687,688,29470,29471) AND FrontID <> 3729";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    decimal DeductibleCount = 0;
                    decimal DeductibleCost = 0;
                    decimal DeductibleWeight = 0;
                    //РЕШЕТКА 45,90,ПЛАСТИК
                    if (Convert.ToInt32(Rows[r]["InsetTypeID"]) == 685 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 686 ||
                        Convert.ToInt32(Rows[r]["InsetTypeID"]) == 687 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 688 ||
                        Convert.ToInt32(Rows[r]["InsetTypeID"]) == 29470 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 29471)
                    {
                        int FactoryID = 0;
                        DecorInvNumber = GetGridInvNumber(Convert.ToInt32(Rows[r]["FrontConfigID"]), ref FactoryID, ref DecorAccountingName);
                        if (DecorInvNumber.Length > 0)
                        {
                            DataRow NewRow = DecorInvNumbersDT.NewRow();
                            NewRow["FrontsOrdersID"] = Convert.ToInt32(Rows[r]["FrontsOrdersID"]);
                            NewRow["FactoryID"] = FactoryID;
                            NewRow["DecorAccountingName"] = DecorAccountingName;
                            NewRow["DecorInvNumber"] = DecorInvNumber;
                            DecorInvNumbersDT.Rows.Add(NewRow);

                            decimal InsetSquare = GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]), Convert.ToInt32(Rows[r]["Width"]));
                            InsetSquare = Decimal.Round(InsetSquare, 3, MidpointRounding.AwayFromZero);
                            DeductibleCount = InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                            DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * InsetSquare * Convert.ToDecimal(Rows[r]["Count"]);
                            DeductibleWeight = Decimal.Round(DeductibleCount * Convert.ToDecimal(3.5), 3, MidpointRounding.AwayFromZero);
                        }
                    }
                    //СТЕКЛО
                    if (Convert.ToInt32(Rows[r]["InsetTypeID"]) == 2)
                    {
                        int FactoryID = 0;
                        DecorInvNumber = GetGlassInvNumber(Convert.ToInt32(Rows[r]["FrontConfigID"]), ref FactoryID, ref DecorAccountingName);
                        if (DecorInvNumber.Length > 0)
                        {
                            DataRow NewRow = DecorInvNumbersDT.NewRow();
                            NewRow["FrontsOrdersID"] = Convert.ToInt32(Rows[r]["FrontsOrdersID"]);
                            NewRow["FactoryID"] = FactoryID;
                            NewRow["DecorAccountingName"] = DecorAccountingName;
                            NewRow["DecorInvNumber"] = DecorInvNumber;
                            DecorInvNumbersDT.Rows.Add(NewRow);
                            DeductibleCount = Convert.ToDecimal(Rows[r]["Count"]) * GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]), Convert.ToInt32(Rows[r]["Width"]));
                            DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * DeductibleCount;
                            DeductibleWeight = Decimal.Round(DeductibleCount * 10, 3, MidpointRounding.AwayFromZero);
                        }
                    }

                    VitrinaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (IsAluminium(Rows[r]) > -1)
                    {
                        VitrinaCost += GetFrontCostAluminium(Rows[r]);
                        VitrinaWeight += GetAluminiumWeight(Rows[r], true);
                    }
                    else
                    {
                        VitrinaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        VitrinaWeight += Convert.ToDecimal(FrontWeight + InsetWeight - DeductibleWeight);
                    }
                    DeductibleCost = Math.Ceiling(DeductibleCost / 0.01m) * 0.01m;
                    WithTransportVitrinaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]) - DeductibleCost;
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ЛЮКС, МЕГА
                filter = " AND InsetTypeID IN (860,862,4310)";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    LuxMegaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    LuxMegaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    WithTransportLuxMegaCost += Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    LuxMegaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }

                if (!IsNonStandard)
                    NonStandardMargin = DBNull.Value;

                //SolidCost = Math.Ceiling(SolidCost / 0.01m) * 0.01m;
                //WithTransportSolidCost = Math.Ceiling(WithTransportSolidCost / 0.01m) * 0.01m;
                if (SolidCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["PaymentRate"] = PaymentRate;
                    Row["OriginalPrice"] = SolidCost / SolidCount;
                    Row["UNN"] = UNN;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Decimal.Round(SolidCount, 3, MidpointRounding.AwayFromZero);
                    Row["Price"] = Decimal.Round(SolidCost / SolidCount, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(SolidCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportSolidCost / SolidCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportSolidCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(SolidWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //FilenkaCost = Math.Ceiling(FilenkaCost / 0.01m) * 0.01m;
                //WithTransportFilenkaCost = Math.Ceiling(WithTransportFilenkaCost / 0.01m) * 0.01m;
                if (FilenkaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["OriginalPrice"] = FilenkaCost / FilenkaCount;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Decimal.Round(FilenkaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Price"] = Decimal.Round(FilenkaCost / FilenkaCount, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(FilenkaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportFilenkaCost / FilenkaCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportFilenkaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(FilenkaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //VitrinaCost = Math.Ceiling(VitrinaCost / 0.01m) * 0.01m;
                //WithTransportVitrinaCost = Math.Ceiling(WithTransportVitrinaCost / 0.01m) * 0.01m;
                if (VitrinaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["OriginalPrice"] = VitrinaCost / VitrinaCount;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Decimal.Round(VitrinaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Price"] = Decimal.Round(VitrinaCost / VitrinaCount, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(VitrinaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportVitrinaCost / VitrinaCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportVitrinaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(VitrinaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //LuxMegaCost = Math.Ceiling(LuxMegaCost / 0.01m) * 0.01m;
                //WithTransportLuxMegaCost = Math.Ceiling(WithTransportLuxMegaCost / 0.01m) * 0.01m;
                if (LuxMegaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["OriginalPrice"] = LuxMegaCost / LuxMegaCount;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Price"] = Decimal.Round(LuxMegaCost / LuxMegaCount, 2, MidpointRounding.AwayFromZero);
                    Row["Count"] = Decimal.Round(LuxMegaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(LuxMegaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["PriceWithTransport"] = Decimal.Round(WithTransportLuxMegaCost / LuxMegaCount, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(WithTransportLuxMegaCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(LuxMegaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }
            }

            Fronts.Dispose();
        }

        private void GetCurvedFronts(DataTable OrdersDataTable, DataTable ReportDataTable)
        {
            string AccountingName = OrdersDataTable.Rows[0]["AccountingName"].ToString();
            string InvNumber = OrdersDataTable.Rows[0]["InvNumber"].ToString();
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            DataTable Fronts = new DataTable();

            using (DataView DV = new DataView(OrdersDataTable))
            {
                Fronts = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Fronts.Rows.Count; i++)
            {
                DataRow[] Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() +
                                                              " AND Width = -1");

                if (Rows.Count() == 0)
                    continue;

                decimal Solid713Count = 0;
                decimal Solid713Price = 0;
                decimal Solid713WithTransportCost = 0;
                decimal Solid713Weight = 0;

                decimal Filenka713Count = 0;
                decimal Filenka713Price = 0;
                decimal Filenka713WithTransportCost = 0;
                decimal Filenka713Weight = 0;

                decimal NoInset713Count = 0;
                decimal NoInset713Price = 0;
                decimal NoInset713WithTransportCost = 0;
                decimal NoInset713Weight = 0;

                decimal Vitrina713Count = 0;
                decimal Vitrina713Price = 0;
                decimal Vitrina713WithTransportCost = 0;
                decimal Vitrina713Weight = 0;

                decimal Solid910Count = 0;
                decimal Solid910Price = 0;
                decimal Solid910WithTransportCost = 0;
                decimal Solid910Weight = 0;

                decimal Filenka910Count = 0;
                decimal Filenka910Price = 0;
                decimal Filenka910WithTransportCost = 0;
                decimal Filenka910Weight = 0;

                decimal NoInset910Count = 0;
                decimal NoInset910Price = 0;
                decimal NoInset910WithTransportCost = 0;
                decimal NoInset910Weight = 0;

                decimal Vitrina910Count = 0;
                decimal Vitrina910Price = 0;
                decimal Vitrina910WithTransportCost = 0;
                decimal Vitrina910Weight = 0;

                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (Rows[r]["Height"].ToString() == "713")
                    {
                        DataRow[] rows = InsetTypesDataTable.Select("GroupID = 3 OR GroupID = 4");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Solid713Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Solid713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Solid713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Solid713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        rows = InsetTypesDataTable.Select("InsetTypeID IN (2079,2080,2081,2082,2085,2086,2087,2088,2212,2213,29210,29211,27831,27832,29210,29211)");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Filenka713Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Filenka713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Filenka713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Filenka713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        if (Rows[r]["InsetTypeID"].ToString() == "-1")
                        {
                            NoInset713Count += Convert.ToDecimal(Rows[r]["Count"]);
                            NoInset713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            NoInset713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina713Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            Vitrina713WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            Vitrina713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }
                    }

                    if (Rows[r]["Height"].ToString() == "910")
                    {
                        DataRow[] rows = InsetTypesDataTable.Select("GroupID = 3 OR GroupID = 4");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Solid910Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Solid910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Solid910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Solid910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        rows = InsetTypesDataTable.Select("InsetTypeID IN (2079,2080,2081,2082,2085,2086,2087,2088,2212,2213,29210,29211,27831,27832,29210,29211)");
                        foreach (DataRow item in rows)
                        {
                            if (Rows[r]["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                            {
                                Filenka910Count += Convert.ToDecimal(Rows[r]["Count"]);
                                Filenka910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                                Filenka910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                                decimal FrontWeight = 0;
                                decimal InsetWeight = 0;

                                GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                                Filenka910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                            }
                        }
                        if (Rows[r]["InsetTypeID"].ToString() == "-1")
                        {
                            NoInset910Count += Convert.ToDecimal(Rows[r]["Count"]);
                            NoInset910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            NoInset910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina910Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);
                            Vitrina910WithTransportCost += Convert.ToDecimal(Rows[r]["PriceWithTransport"]) * Convert.ToDecimal(Rows[r]["Count"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            Vitrina910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }
                    }

                }

                //decimal Cost = Math.Ceiling(Solid713Count * Solid713Price / 0.01m) * 0.01m;
                //Solid713WithTransportCost = Math.Ceiling(Solid713WithTransportCost / 0.01m) * 0.01m;
                if (Solid713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Solid713Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Solid713Count;
                    Row["PriceWithTransport"] = Decimal.Round(Solid713WithTransportCost / Solid713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Solid713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Solid713Count * Solid713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Filenka713Count * Filenka713Price / 0.01m) * 0.01m;
                //Filenka713WithTransportCost = Math.Ceiling(Filenka713WithTransportCost / 0.01m) * 0.01m;
                if (Filenka713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Filenka713Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Filenka713Count;
                    Row["PriceWithTransport"] = Decimal.Round(Filenka713WithTransportCost / Filenka713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Filenka713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Filenka713Count * Filenka713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(NoInset713Count * NoInset713Price / 0.01m) * 0.01m;
                //NoInset713WithTransportCost = Math.Ceiling(NoInset713WithTransportCost / 0.01m) * 0.01m;
                if (NoInset713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = NoInset713Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = NoInset713Count;
                    Row["PriceWithTransport"] = Decimal.Round(NoInset713WithTransportCost / NoInset713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(NoInset713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(NoInset713Count * NoInset713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Vitrina713Count * Vitrina713Price / 0.01m) * 0.01m;
                //Vitrina713WithTransportCost = Math.Ceiling(Vitrina713WithTransportCost / 0.01m) * 0.01m;
                if (Vitrina713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Vitrina713Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Vitrina713Count;
                    Row["PriceWithTransport"] = Decimal.Round(Vitrina713WithTransportCost / Vitrina713Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Vitrina713WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Vitrina713Count * Vitrina713Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Solid910Count * Solid910Price / 0.01m) * 0.01m;
                //Solid910WithTransportCost = Math.Ceiling(Solid910WithTransportCost / 0.01m) * 0.01m;
                if (Solid910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Solid910Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Solid910Count;
                    Row["PriceWithTransport"] = Decimal.Round(Solid910WithTransportCost / Solid910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Solid910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Solid910Count * Solid910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Filenka910Count * Filenka910Price / 0.01m) * 0.01m;
                //Filenka910WithTransportCost = Math.Ceiling(Filenka910WithTransportCost / 0.01m) * 0.01m;
                if (Filenka910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Filenka910Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Filenka910Count;
                    Row["PriceWithTransport"] = Decimal.Round(Filenka910WithTransportCost / Filenka910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Filenka910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Filenka910Count * Filenka910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(NoInset910Count * NoInset910Price / 0.01m) * 0.01m;
                //NoInset910WithTransportCost = Math.Ceiling(NoInset910WithTransportCost / 0.01m) * 0.01m;
                if (NoInset910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = NoInset910Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = NoInset910Count;
                    Row["PriceWithTransport"] = Decimal.Round(NoInset910WithTransportCost / NoInset910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(NoInset910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(NoInset910Count * NoInset910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                //Cost = Math.Ceiling(Vitrina910Count * Vitrina910Price / 0.01m) * 0.01m;
                //Vitrina910WithTransportCost = Math.Ceiling(Vitrina910WithTransportCost / 0.01m) * 0.01m;
                if (Vitrina910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Vitrina910Price;
                    Row["UNN"] = UNN;
                    Row["PaymentRate"] = PaymentRate;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        Row["TPSCurCode"] = TPSCurrencyCode;
                    Row["Count"] = Vitrina910Count;
                    Row["PriceWithTransport"] = Decimal.Round(Vitrina910WithTransportCost / Vitrina910Count, 2, MidpointRounding.AwayFromZero);
                    Row["CostWithTransport"] = Decimal.Round(Math.Ceiling(Vitrina910WithTransportCost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Math.Ceiling(Vitrina910Count * Vitrina910Price / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }
            }

            Fronts.Dispose();
        }

        private void GetGrids(DataTable OrdersDataTable, DataTable ReportDataTable, DataTable ReportDataTable1, int FactoryID1)
        {
            DataRow[] Rows = OrdersDataTable.Select("InsetTypeID IN (685,686,687,688,29470,29471)");

            if (Rows.Count() == 0)
                return;

            decimal CountPP = 0;
            decimal CostPP = 0;

            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);

            for (int i = 0; i < Rows.Count(); i++)
            {
                int FrontID = Convert.ToInt32(Rows[i]["FrontID"]);
                if (FrontID == 3729)
                    continue;
                decimal d = GetInsetSquare(Convert.ToInt32(Rows[i]["FrontID"]), Convert.ToInt32(Rows[i]["Height"]),
                                            Convert.ToInt32(Rows[i]["Width"])) * Convert.ToDecimal(Rows[i]["Count"]);
                CountPP += d;
                CostPP += Math.Ceiling(Convert.ToDecimal(Rows[i]["InsetPrice"]) * d / 0.01m) * 0.01m;
            }

            if (CountPP > 0)
            {
                DataRow[] rows = DecorInvNumbersDT.Select("FrontsOrdersID = " + Convert.ToInt32(OrdersDataTable.Select("InsetTypeID IN (685,686,687,688,29470,29471)")[0]["FrontsOrdersID"]));
                int FactoryID = 0;
                if (rows.Count() > 0)
                    FactoryID = Convert.ToInt32(rows[0]["FactoryID"]);
                if (FactoryID == 0)
                    FactoryID = FactoryID1;
                if (FactoryID != FactoryID1)
                {
                    //CostPP = Math.Ceiling(CostPP / 0.01m) * 0.01m;
                    DataRow NewRow = ReportDataTable1.NewRow();
                    NewRow["OriginalPrice"] = CostPP / CountPP;
                    NewRow["UNN"] = UNN;
                    NewRow["PaymentRate"] = PaymentRate;
                    NewRow["AccountingName"] = OrdersDataTable.Rows[0]["AccountingName"].ToString();
                    NewRow["InvNumber"] = OrdersDataTable.Rows[0]["InvNumber"].ToString();
                    NewRow["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                    NewRow["Count"] = Decimal.Round(CountPP, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(Math.Ceiling(CostPP / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["PriceWithTransport"] = Decimal.Round(CostPP / CountPP, 2, MidpointRounding.AwayFromZero);
                    NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CostPP / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(3.5), 3, MidpointRounding.AwayFromZero);
                    if (rows.Count() > 0)
                    {
                        NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                        NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                    }
                    ReportDataTable1.Rows.Add(NewRow);
                }
                else
                {
                    //CostPP = Math.Ceiling(CostPP / 0.01m) * 0.01m;
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["OriginalPrice"] = CostPP / CountPP;
                    NewRow["UNN"] = UNN;
                    NewRow["PaymentRate"] = PaymentRate;
                    NewRow["AccountingName"] = OrdersDataTable.Rows[0]["AccountingName"].ToString();
                    NewRow["InvNumber"] = OrdersDataTable.Rows[0]["InvNumber"].ToString();
                    NewRow["CurrencyCode"] = ProfilCurrencyCode;
                    if (fID == 2)
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                    NewRow["Count"] = Decimal.Round(CountPP, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(Math.Ceiling(CostPP / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["PriceWithTransport"] = Decimal.Round(CostPP / CountPP, 2, MidpointRounding.AwayFromZero);
                    NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CostPP / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                    NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(3.5), 3, MidpointRounding.AwayFromZero);
                    if (rows.Count() > 0)
                    {
                        NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                        NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                    }
                    ReportDataTable.Rows.Add(NewRow);
                }
            }
        }

        private void GetGlass(DataTable OrdersDataTable, DataTable ReportDataTable, DataTable ReportDataTable1, int FactoryID1)
        {
            string AccountingName = OrdersDataTable.Rows[0]["AccountingName"].ToString();
            string InvNumber = OrdersDataTable.Rows[0]["InvNumber"].ToString();
            decimal CountFlutes = 0;
            decimal CountLacomat = 0;
            //decimal CountMaster = 0;
            decimal CountKrizet = 0;
            decimal CountOther = 0;

            decimal PriceFlutes = 0;
            decimal PriceLacomat = 0;
            //decimal PriceMaster = 0;
            decimal PriceKrizet = 0;
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);

            if (OrdersDataTable.Select("InsetTypeID = 2").Count() == 0)
                return;

            bool hasGlass = false;
            for (int i = 0; i < OrdersDataTable.Rows.Count; i++)
            {
                int frontID = Convert.ToInt32(OrdersDataTable.Rows[0]["FrontID"]);
                if (IsAluminium(OrdersDataTable.Rows[i]) > -1)
                {
                    hasGlass = true;
                    break;
                }
            }
            if (!hasGlass)
                return;

            DataRow[] FRows = OrdersDataTable.Select("InsetColorID = 3944");

            if (FRows.Count() > 0)
            {
                PriceFlutes = Convert.ToDecimal(FRows[0]["InsetPrice"]);

                for (int i = 0; i < FRows.Count(); i++)
                {
                    if (IsAluminium(FRows[i]) != -1)
                        continue;

                    CountFlutes += Convert.ToDecimal(FRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(FRows[i]["FrontID"]),
                                              Convert.ToInt32(FRows[i]["Height"]),
                                              Convert.ToInt32(FRows[i]["Width"]));
                }

                CountFlutes = Decimal.Round(CountFlutes, 3, MidpointRounding.AwayFromZero);

                if (CountFlutes > 0)
                {
                    //decimal Cost = Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m;
                    DataRow[] rows = DecorInvNumbersDT.Select("FrontsOrdersID = " + Convert.ToInt32(FRows[0]["FrontsOrdersID"]));
                    int FactoryID = 0;
                    if (rows.Count() > 0)
                        FactoryID = Convert.ToInt32(rows[0]["FactoryID"]);
                    if (FactoryID == 0)
                        FactoryID = FactoryID1;
                    if (FactoryID != FactoryID1)
                    {
                        DataRow NewRow = ReportDataTable1.NewRow();
                        //NewRow["Name"] = "Стекло Флутес";
                        NewRow["OriginalPrice"] = PriceFlutes;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountFlutes;
                        NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(PriceFlutes, 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(CountFlutes * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло Флутес";
                        NewRow["OriginalPrice"] = PriceFlutes;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = CountFlutes;
                        NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(PriceFlutes, 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountFlutes * PriceFlutes / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(CountFlutes * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable.Rows.Add(NewRow);
                    }
                }
            }


            DataRow[] LRows = OrdersDataTable.Select("InsetColorID = 3943");

            if (LRows.Count() > 0)
            {
                PriceLacomat = Convert.ToDecimal(LRows[0]["InsetPrice"]);

                for (int i = 0; i < LRows.Count(); i++)
                {
                    if (IsAluminium(LRows[i]) != -1)
                        continue;

                    CountLacomat += Convert.ToDecimal(LRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(LRows[i]["FrontID"]),
                                              Convert.ToInt32(LRows[i]["Height"]),
                                              Convert.ToInt32(LRows[i]["Width"]));

                }

                CountLacomat = Decimal.Round(CountLacomat, 3, MidpointRounding.AwayFromZero);

                if (CountLacomat > 0)
                {
                    //decimal Cost = Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m;
                    DataRow[] rows = DecorInvNumbersDT.Select("FrontsOrdersID = " + Convert.ToInt32(LRows[0]["FrontsOrdersID"]));
                    int FactoryID = 0;
                    if (rows.Count() > 0)
                        FactoryID = Convert.ToInt32(rows[0]["FactoryID"]);
                    if (FactoryID == 0)
                        FactoryID = FactoryID1;
                    if (FactoryID != FactoryID1)
                    {
                        DataRow NewRow = ReportDataTable1.NewRow();
                        //NewRow["Name"] = "Стекло Лакомат";
                        NewRow["OriginalPrice"] = PriceLacomat;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = CountLacomat;
                        NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(PriceLacomat, 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(CountLacomat * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло Лакомат";
                        NewRow["OriginalPrice"] = PriceLacomat;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = CountLacomat;
                        NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(PriceLacomat, 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountLacomat * PriceLacomat / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(CountLacomat * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            DataRow[] KRows = OrdersDataTable.Select("InsetColorID = 3945");

            if (KRows.Count() > 0)
            {
                PriceKrizet = Convert.ToDecimal(KRows[0]["InsetPrice"]);

                for (int i = 0; i < KRows.Count(); i++)
                {
                    if (IsAluminium(KRows[i]) != -1)
                        continue;

                    CountKrizet += Convert.ToDecimal(KRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(KRows[i]["FrontID"]),
                                              Convert.ToInt32(KRows[i]["Height"]),
                                              Convert.ToInt32(KRows[i]["Width"]));
                }

                CountKrizet = Decimal.Round(CountKrizet, 3, MidpointRounding.AwayFromZero);

                if (CountKrizet > 0)
                {
                    //decimal Cost = Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m;
                    DataRow[] rows = DecorInvNumbersDT.Select("FrontsOrdersID = " + Convert.ToInt32(KRows[0]["FrontsOrdersID"]));
                    int FactoryID = 0;
                    if (rows.Count() > 0)
                        FactoryID = Convert.ToInt32(rows[0]["FactoryID"]);
                    if (FactoryID == 0)
                        FactoryID = FactoryID1;
                    if (FactoryID != FactoryID1)
                    {
                        DataRow NewRow = ReportDataTable1.NewRow();
                        //NewRow["Name"] = "Стекло Кризет";
                        NewRow["OriginalPrice"] = PriceKrizet;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = CountKrizet;
                        NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(PriceKrizet, 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(CountKrizet * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло Кризет";
                        NewRow["OriginalPrice"] = PriceKrizet;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = CountKrizet;
                        NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(PriceKrizet, 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountKrizet * PriceKrizet / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(CountKrizet * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            DataRow[] ORows = OrdersDataTable.Select("InsetTypeID = 18");

            if (ORows.Count() > 0)
            {
                for (int i = 0; i < ORows.Count(); i++)
                {
                    if (IsAluminium(ORows[i]) != -1)
                        continue;

                    CountOther += Convert.ToDecimal(ORows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(ORows[i]["FrontID"]),
                                              Convert.ToInt32(ORows[i]["Height"]),
                                              Convert.ToInt32(ORows[i]["Width"]));
                }

                CountOther = Decimal.Round(CountOther, 3, MidpointRounding.AwayFromZero);

                if (CountOther > 0)
                {
                    DataRow[] rows = DecorInvNumbersDT.Select("FrontsOrdersID = " + Convert.ToInt32(ORows[0]["FrontsOrdersID"]));
                    int FactoryID = 0;
                    if (rows.Count() > 0)
                        FactoryID = Convert.ToInt32(rows[0]["FactoryID"]);
                    if (FactoryID == 0)
                        FactoryID = FactoryID1;
                    if (FactoryID != FactoryID1)
                    {
                        DataRow NewRow = ReportDataTable1.NewRow();
                        //NewRow["Name"] = "Стекло другое";
                        NewRow["OriginalPrice"] = 0;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = CountOther;
                        NewRow["Cost"] = 0;
                        NewRow["PriceWithTransport"] = 0;
                        NewRow["CostWithTransport"] = 0;
                        NewRow["Weight"] = Decimal.Round(CountOther * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло другое";
                        NewRow["OriginalPrice"] = 0;
                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        if (fID == 2)
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = CountOther;
                        NewRow["Cost"] = 0;
                        NewRow["PriceWithTransport"] = 0;
                        NewRow["CostWithTransport"] = 0;
                        NewRow["Weight"] = Decimal.Round(CountOther * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsets(DataTable OrdersDataTable, DataTable ReportDataTable)
        {
            decimal CountEllipseGrid = 0;
            decimal PriceEllipseGrid = 0;
            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);

            DataRow[] EGRows = OrdersDataTable.Select("FrontID IN (3729)");//ellipse grid

            if (EGRows.Count() > 0)
            {
                int MarginHeight = 0;
                int MarginWidth = 0;

                GetGlassMarginAluminium(EGRows[0], ref MarginHeight, ref MarginWidth);

                PriceEllipseGrid = Convert.ToDecimal(EGRows[0]["InsetPrice"]);

                for (int i = 0; i < EGRows.Count(); i++)
                {
                    decimal dd = Decimal.Round(Convert.ToDecimal(EGRows[i]["Count"]) * MarginHeight * (Convert.ToDecimal(EGRows[i]["Width"]) - MarginWidth) / 1000000, 3, MidpointRounding.AwayFromZero);
                    CountEllipseGrid += dd;
                }

                decimal Weight = GetInsetWeight(EGRows[0]);
                Weight = Decimal.Round(CountEllipseGrid * Weight, 3, MidpointRounding.AwayFromZero);

                //decimal Cost = Math.Ceiling(CountEllipseGrid * PriceEllipseGrid / 0.01m) * 0.01m;
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["OriginalPrice"] = PriceEllipseGrid;
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["Count"] = CountEllipseGrid;
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(CountEllipseGrid * PriceEllipseGrid / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["PriceWithTransport"] = PriceEllipseGrid;
                NewRow["CostWithTransport"] = Decimal.Round(Math.Ceiling(CountEllipseGrid * PriceEllipseGrid / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
                NewRow["Weight"] = 0;
                DataRow[] rows = DecorInvNumbersDT.Select("FrontsOrdersID = " + Convert.ToInt32(EGRows[0]["FrontsOrdersID"]));
                if (rows.Count() > 0)
                {
                    NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                    NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                }
                ReportDataTable.Rows.Add(NewRow);

            }
        }

        private decimal GetInsetWeight(DataRow FrontsOrdersRow)
        {
            int InsetTypeID = Convert.ToInt32(FrontsOrdersRow["InsetTypeID"]);
            if (InsetTypeID == 2)
                InsetTypeID = Convert.ToInt32(FrontsOrdersRow["InsetColorID"]);//стекло
            decimal InsetSquare = GetInsetSquare(FrontsOrdersRow);
            if (InsetSquare <= 0)
                return 0;
            decimal InsetWeight = 0;
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + InsetTypeID);
            if (Rows.Count() > 0)
            {
                if (Rows[0]["Weight"] != DBNull.Value)
                    InsetWeight = Convert.ToDecimal(Rows[0]["Weight"]);
            }

            return InsetSquare * InsetWeight;
        }

        private decimal GetProfileWeight(DataRow FrontsOrdersRow)
        {
            decimal FrontHeight = Convert.ToDecimal(FrontsOrdersRow["Height"]);
            decimal FrontWidth = Convert.ToDecimal(FrontsOrdersRow["Width"]);
            decimal ProfileWeight = 0;
            decimal ProfileWidth = 0;
            DataRow[] FrontsConfigRow = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString());
            if (FrontsConfigRow.Count() > 0)
                ProfileWeight = Convert.ToDecimal(FrontsConfigRow[0]["Weight"]);

            //для Женевы и Тафеля глухой - вес квадрата профиля на площадь фасада
            int FrontID = Convert.ToInt32(FrontsOrdersRow["FrontID"]);
            if (FrontID == 30504 || FrontID == 30505 || FrontID == 30506 ||
                FrontID == 30364 || FrontID == 30366 || FrontID == 30367 ||
                FrontID == 30501 || FrontID == 30502 || FrontID == 30503 ||
                FrontID == 16269 || FrontID == 28945 || FrontID == 27914 || FrontID == 29597 || FrontID == 3727 || FrontID == 3728 || FrontID == 3729 ||
                FrontID == 3730 || FrontID == 3731 || FrontID == 3732 || FrontID == 3733 || FrontID == 3734 ||
                FrontID == 3735 || FrontID == 3736 || FrontID == 3737 || FrontID == 3739 || FrontID == 3740 ||
                FrontID == 3741 || FrontID == 3742 || FrontID == 3743 || FrontID == 3744 || FrontID == 3745 ||
                FrontID == 3746 || FrontID == 3747 || FrontID == 3748 || FrontID == 15108 || FrontID == 3662 || FrontID == 3663 || FrontID == 3664 || FrontID == 15760)
                return FrontWidth * FrontHeight / 1000000 * ProfileWeight;
            else
            {
                DataRow[] DecorConfigRow = TechStoreDataTable.Select("TechStoreID = " + FrontsConfigRow[0]["ProfileID"].ToString());
                if (DecorConfigRow.Count() > 0)
                {
                    ProfileWidth = Convert.ToDecimal(DecorConfigRow[0]["Width"]);
                    ProfileWeight = Convert.ToDecimal(DecorConfigRow[0]["Weight"]);
                    return (FrontWidth * 2 + (FrontHeight - ProfileWidth - ProfileWidth) * 2) / 1000 * ProfileWeight;
                }
            }
            return 0;
        }

        public void GetFrontWeight(DataRow FrontsOrdersRow, ref decimal outFrontWeight, ref decimal outInsetWeight)
        {
            //decimal FrontsWeight = 0;
            DataRow[] FrontsConfigRow = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString());
            decimal InsetWeight = Convert.ToDecimal(FrontsConfigRow[0]["InsetWeight"]);
            decimal FrontsOrderSquare = Convert.ToDecimal(FrontsOrdersRow["Square"]);
            decimal PackWeight = 0;
            if (FrontsOrderSquare > 0)
                PackWeight = FrontsOrderSquare * Convert.ToDecimal(0.7);
            //если гнутый то вес за штуки
            if (FrontsConfigRow[0]["Width"].ToString() == "-1")
            {
                outFrontWeight = PackWeight +
                    Convert.ToDecimal(FrontsOrdersRow["Count"]) * Convert.ToDecimal(FrontsConfigRow[0]["Weight"]);
                return;
            }
            //если алюминий
            if (IsAluminium(FrontsOrdersRow) > -1)
            {
                outFrontWeight = PackWeight +
                    GetAluminiumWeight(FrontsOrdersRow, true);
                return;
            }
            decimal ResultProfileWeight = GetProfileWeight(FrontsOrdersRow);
            decimal ResultInsetWeight = 0;
            DataRow[] rows = InsetTypesDataTable.Select("GroupID = 2 OR GroupID = 3 OR GroupID = 4 OR GroupID = 7 OR GroupID = 12 OR GroupID = 13");
            foreach (DataRow item in rows)
            {
                if (FrontsOrdersRow["InsetTypeID"].ToString() == item["InsetTypeID"].ToString())
                {
                    ResultInsetWeight = GetInsetWeight(FrontsOrdersRow);
                }
            }
            if (Convert.ToInt32(FrontsOrdersRow["FrontID"]) == 3729)
                ResultInsetWeight = GetInsetWeight(FrontsOrdersRow);

            outFrontWeight = PackWeight + ResultProfileWeight * Convert.ToDecimal(FrontsOrdersRow["Count"]);
            outInsetWeight = ResultInsetWeight * Convert.ToDecimal(FrontsOrdersRow["Count"]);
        }

        private DataTable[] GroupInvNumber(DataTable OrdersDataTable)
        {
            int InvCount = 0;

            //get count of different covertypes
            using (DataView DV = new DataView(OrdersDataTable))
            {
                InvCount = DV.ToTable(true, new string[] { "InvNumber" }).Rows.Count;
            }

            //create DataTables
            DataTable[] InvDataTables = new DataTable[InvCount];


            //split for covertypes
            int g = 0;

            for (int i = 0; i < InvCount; i++)
            {
                InvDataTables[i] = OrdersDataTable.Clone();

                string InvNumber = "";

                for (int r = g; r < OrdersDataTable.Rows.Count; r++)
                {
                    if (InvNumber == "")
                    {
                        InvDataTables[i].ImportRow(OrdersDataTable.DefaultView[r].Row);
                        InvNumber = OrdersDataTable.DefaultView[r].Row["InvNumber"].ToString();
                        continue;
                    }

                    if (InvNumber == OrdersDataTable.DefaultView[r].Row["InvNumber"].ToString())
                    {
                        InvDataTables[i].ImportRow(OrdersDataTable.DefaultView[r].Row);
                    }
                    else
                    {
                        g = r;
                        break;
                    }
                }
            }

            return InvDataTables;
        }

        public void Report(int[] MainOrderIDs, bool IsSample)
        {
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();
            DecorInvNumbersDT.Clear();
            GetMegaOrderInfo(MainOrderIDs[0]);

            string sWhere = "";

            for (int i = 0; i < MainOrderIDs.Count(); i++)
            {
                if (sWhere != "")
                    sWhere += " OR MainOrderID = " + MainOrderIDs[i].ToString();
                else
                    sWhere += "MainOrderID = " + MainOrderIDs[i].ToString();
            }
            string SelectCommand = "SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewFrontsOrders" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID" +
                " WHERE NewFrontsOrders.IsSample=1 AND InvNumber IS NOT NULL AND NewFrontsOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            if (!IsSample)
                SelectCommand = "SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewFrontsOrders" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID" +
                " WHERE NewFrontsOrders.IsSample=0 AND InvNumber IS NOT NULL AND NewFrontsOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    DataTable DistRatesDT = new DataTable();
                    DataTable DT1 = DT.Clone();
                    //get count of different covertypes
                    using (DataView DV = new DataView(DT))
                    {
                        DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
                    }

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);

                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(ProfilFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, ProfilReportDataTable, false);
                                GetSimpleFronts(DT1, ProfilReportDataTable, true);

                                GetCurvedFronts(DT1, ProfilReportDataTable);

                                GetGrids(DT1, ProfilReportDataTable, TPSReportDataTable, 1);

                                GetInsets(DT1, ProfilReportDataTable);

                                GetGlass(DT1, ProfilReportDataTable, TPSReportDataTable, 1);
                            }
                        }
                    }

                    DT1.Clear();
                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(TPSFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, TPSReportDataTable, false);
                                GetSimpleFronts(DT1, TPSReportDataTable, true);

                                GetCurvedFronts(DT1, TPSReportDataTable);

                                GetGrids(DT1, TPSReportDataTable, ProfilReportDataTable, 2);

                                GetInsets(DT1, TPSReportDataTable);

                                GetGlass(DT1, TPSReportDataTable, ProfilReportDataTable, 2);
                            }
                        }
                    }

                }
            }
        }

        public void Report(int[] MainOrderIDs)
        {
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();
            DecorInvNumbersDT.Clear();
            GetMegaOrderInfo(MainOrderIDs[0]);

            string SelectCommand = "SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewFrontsOrders" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID" +
                " WHERE  InvNumber IS NOT NULL AND NewFrontsOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    DataTable DistRatesDT = new DataTable();
                    DataTable DT1 = DT.Clone();
                    //get count of different covertypes
                    using (DataView DV = new DataView(DT))
                    {
                        DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
                    }

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);

                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(ProfilFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, ProfilReportDataTable, false);
                                GetSimpleFronts(DT1, ProfilReportDataTable, true);

                                GetCurvedFronts(DT1, ProfilReportDataTable);

                                GetGrids(DT1, ProfilReportDataTable, TPSReportDataTable, 1);

                                GetInsets(DT1, ProfilReportDataTable);

                                GetGlass(DT1, ProfilReportDataTable, TPSReportDataTable, 1);
                            }
                        }
                    }

                    DT1.Clear();
                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(TPSFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, TPSReportDataTable, false);
                                GetSimpleFronts(DT1, TPSReportDataTable, true);

                                GetCurvedFronts(DT1, TPSReportDataTable);

                                GetGrids(DT1, TPSReportDataTable, ProfilReportDataTable, 2);

                                GetInsets(DT1, TPSReportDataTable);

                                GetGlass(DT1, TPSReportDataTable, ProfilReportDataTable, 2);
                            }
                        }
                    }

                }
            }
        }

        public void Report(int MegaOrderID, decimal dPaymentRate, bool IsSample)
        {
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();
            DecorInvNumbersDT.Clear();
            GetMegaOrderInfo1(MegaOrderID);

            string SelectCommand = "SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewFrontsOrders" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID AND NewMegaOrders.MegaOrderID = " + MegaOrderID + @"
                WHERE NeedCalcPrice=0 AND NewFrontsOrders.IsSample=1 AND InvNumber IS NOT NULL ORDER BY InvNumber";
            if (!IsSample)
                SelectCommand = "SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewFrontsOrders" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID AND NewMegaOrders.MegaOrderID = " + MegaOrderID + @"
                WHERE NeedCalcPrice=0 AND NewFrontsOrders.IsSample=0 AND InvNumber IS NOT NULL ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    for (int j = 0; j < DT.Rows.Count; j++)
                    {
                        DT.Rows[j]["PaymentRate"] = dPaymentRate;
                    }
                    DataTable DistRatesDT = new DataTable();
                    DataTable DT1 = DT.Clone();
                    //get count of different covertypes
                    using (DataView DV = new DataView(DT))
                    {
                        DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
                    }

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);

                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(ProfilFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, ProfilReportDataTable, false);
                                GetSimpleFronts(DT1, ProfilReportDataTable, true);

                                GetCurvedFronts(DT1, ProfilReportDataTable);

                                GetGrids(DT1, ProfilReportDataTable, TPSReportDataTable, 1);

                                GetInsets(DT1, ProfilReportDataTable);

                                GetGlass(DT1, ProfilReportDataTable, TPSReportDataTable, 1);
                            }
                        }
                    }

                    DT1.Clear();
                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(TPSFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, TPSReportDataTable, false);
                                GetSimpleFronts(DT1, TPSReportDataTable, true);

                                GetCurvedFronts(DT1, TPSReportDataTable);

                                GetGrids(DT1, TPSReportDataTable, ProfilReportDataTable, 2);

                                GetInsets(DT1, TPSReportDataTable);

                                GetGlass(DT1, TPSReportDataTable, ProfilReportDataTable, 2);
                            }
                        }
                    }

                }
            }
        }

        public void Report(int MegaOrderID, decimal dPaymentRate)
        {
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();
            DecorInvNumbersDT.Clear();
            GetMegaOrderInfo1(MegaOrderID);

            string SelectCommand = "SELECT NewFrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewFrontsOrders" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " INNER JOIN NewMainOrders ON NewFrontsOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID AND NewMegaOrders.MegaOrderID = " + MegaOrderID + @"
                WHERE NeedCalcPrice=0 AND InvNumber IS NOT NULL ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    for (int j = 0; j < DT.Rows.Count; j++)
                    {
                        DT.Rows[j]["PaymentRate"] = dPaymentRate;
                    }
                    DataTable DistRatesDT = new DataTable();
                    DataTable DT1 = DT.Clone();
                    //get count of different covertypes
                    using (DataView DV = new DataView(DT))
                    {
                        DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
                    }

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);

                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(ProfilFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, ProfilReportDataTable, false);
                                GetSimpleFronts(DT1, ProfilReportDataTable, true);

                                GetCurvedFronts(DT1, ProfilReportDataTable);

                                GetGrids(DT1, ProfilReportDataTable, TPSReportDataTable, 1);

                                GetInsets(DT1, ProfilReportDataTable);

                                GetGlass(DT1, ProfilReportDataTable, TPSReportDataTable, 1);
                            }
                        }
                    }

                    DT1.Clear();
                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(TPSFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                            {
                                DT1.Clear();
                                PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);
                                DataRow[] rows = DTs[i].Select("PaymentRate='" + PaymentRate.ToString() + "'");
                                foreach (DataRow item in rows)
                                    DT1.Rows.Add(item.ItemArray);
                                if (DT1.Rows.Count == 0)
                                    continue;

                                GetSimpleFronts(DT1, TPSReportDataTable, false);
                                GetSimpleFronts(DT1, TPSReportDataTable, true);

                                GetCurvedFronts(DT1, TPSReportDataTable);

                                GetGrids(DT1, TPSReportDataTable, ProfilReportDataTable, 2);

                                GetInsets(DT1, TPSReportDataTable);

                                GetGlass(DT1, TPSReportDataTable, ProfilReportDataTable, 2);
                            }
                        }
                    }

                }
            }
        }
    }






    public class InvoiceDecorReportToDBF
    {
        decimal TransportCost = 0;
        decimal AdditionalCost = 0;
        decimal PaymentRate = 1;
        int ClientID = 0;
        string ProfilCurrencyCode = "0";
        string TPSCurrencyCode = "0";
        string UNN = string.Empty;

        DataTable CurrencyTypesDT;
        public DataTable ProfilReportDataTable = null;
        public DataTable TPSReportDataTable = null;
        private DataTable DecorProductsDataTable = null;
        private DataTable DecorConfigDataTable = null;
        private DataTable MeasuresDataTable = null;
        private DataTable DecorOrdersDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        public InvoiceDecorReportToDBF()
        {
            Create();
            CreateReportDataTable();
        }

        private void Create()
        {
            string SelectCommand = "SELECT * FROM CurrencyTypes";
            CurrencyTypesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDT);
            }
            DecorOrdersDataTable = new DataTable();

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
            MeasuresDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }

            DecorConfigDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorConfig",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDataTable);
            //}
            DecorConfigDataTable = TablesManager.DecorConfigDataTable;

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }

        }
        public bool HasParameter(int ProductID, String Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }

        private void CreateReportDataTable()
        {
            ProfilReportDataTable = new DataTable();
            ProfilReportDataTable.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            TPSReportDataTable = ProfilReportDataTable.Clone();
        }

        public void ClearReport()
        {
            DecorOrdersDataTable.Clear();

            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();

            ProfilReportDataTable.AcceptChanges();
            TPSReportDataTable.AcceptChanges();
        }

        private bool IsProfil(int DecorConfigID)
        {
            DataRow[] Rows = DecorConfigDataTable.Select("DecorConfigID = " + DecorConfigID.ToString());

            if (Rows[0]["FactoryID"].ToString() == "1")
                return true;

            return false;
        }

        public void GetMegaOrderInfo1(int MegaOrderID)
        {
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, PaymentRate, ClientID, CurrencyTypeID FROM NewMegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int CurrencyTypeID = 1;
                        if (DT.Rows[0]["TransportCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["TransportCost"].ToString(), out TransportCost);
                        if (DT.Rows[0]["AdditionalCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["AdditionalCost"].ToString(), out AdditionalCost);
                        //if (DT.Rows[0]["PaymentRate"] != DBNull.Value)
                        //    decimal.TryParse(DT.Rows[0]["PaymentRate"].ToString(), out PaymentRate);
                        if (DT.Rows[0]["ClientID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["ClientID"].ToString(), out ClientID);
                        if (DT.Rows[0]["CurrencyTypeID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["CurrencyTypeID"].ToString(), out CurrencyTypeID);

                        DataRow[] rows = CurrencyTypesDT.Select("CurrencyTypeID = " + CurrencyTypeID);
                        if (rows.Count() > 0)
                        {
                            ProfilCurrencyCode = rows[0]["CurrencyCode"].ToString();
                            TPSCurrencyCode = rows[0]["TPSCurrencyCode"].ToString();
                        }
                    }
                }
            }
            SelectCommand = "SELECT UNN FROM Clients WHERE ClientID = " + ClientID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["UNN"] != DBNull.Value)
                            UNN = DT.Rows[0]["UNN"].ToString();
                    }
                }
            }
        }

        public void GetMegaOrderInfo(int MainOrderID)
        {
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, PaymentRate, ClientID, CurrencyTypeID FROM NewMegaOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM NewMainOrders WHERE MainOrderID = " + MainOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int CurrencyTypeID = 1;
                        if (DT.Rows[0]["TransportCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["TransportCost"].ToString(), out TransportCost);
                        if (DT.Rows[0]["AdditionalCost"] != DBNull.Value)
                            decimal.TryParse(DT.Rows[0]["AdditionalCost"].ToString(), out AdditionalCost);
                        //if (DT.Rows[0]["PaymentRate"] != DBNull.Value)
                        //    decimal.TryParse(DT.Rows[0]["PaymentRate"].ToString(), out PaymentRate);
                        if (DT.Rows[0]["ClientID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["ClientID"].ToString(), out ClientID);
                        if (DT.Rows[0]["CurrencyTypeID"] != DBNull.Value)
                            int.TryParse(DT.Rows[0]["CurrencyTypeID"].ToString(), out CurrencyTypeID);

                        DataRow[] rows = CurrencyTypesDT.Select("CurrencyTypeID = " + CurrencyTypeID);
                        if (rows.Count() > 0)
                        {
                            ProfilCurrencyCode = rows[0]["CurrencyCode"].ToString();
                            TPSCurrencyCode = rows[0]["TPSCurrencyCode"].ToString();
                        }
                    }
                }
            }
            SelectCommand = "SELECT UNN FROM Clients WHERE ClientID = " + ClientID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["UNN"] != DBNull.Value)
                            UNN = DT.Rows[0]["UNN"].ToString();
                    }
                }
            }
        }

        private int GetReportMeasureTypeID(int DecorConfigID)
        {
            DataRow[] Row = DecorConfigDataTable.Select("DecorConfigID = " + DecorConfigID);

            return Convert.ToInt32(Row[0]["ReportMeasureID"]);//1 м.кв.  2 м.п. 3 шт.
        }

        private int GetMeasureTypeID(int DecorConfigID)
        {
            DataRow[] Row = DecorConfigDataTable.Select("DecorConfigID = " + DecorConfigID);

            return Convert.ToInt32(Row[0]["MeasureID"]);//1 м.кв.  2 м.п. 3 шт.
        }

        public decimal GetDecorWeight(DataRow DecorOrderRow)
        {
            if (DecorOrderRow["Weight"] == DBNull.Value)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка конфигурации: нет веса в DecorConfigID=" + DecorOrderRow["DecorConfigID"].ToString() +
                    ". Вес будет выставлен в 0.");
                return 0;
            }
            decimal Weight = 0;

            DataRow[] Row = DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString());

            if (Row[0]["Weight"] == DBNull.Value)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка конфигурации: нет веса в DecorConfigID=" + DecorOrderRow["DecorConfigID"].ToString() +
                    ". Вес будет выставлен в 0.");
                return 0;
            }
            if (Row[0]["WeightMeasureID"].ToString() == "1")
            {
                if (Convert.ToDecimal(DecorOrderRow["Height"]) != -1)
                    Weight = Convert.ToDecimal(DecorOrderRow["Height"]) * Convert.ToDecimal(DecorOrderRow["Width"]) / 1000000
                         * Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);
                if (Convert.ToDecimal(DecorOrderRow["Length"]) != -1)
                    Weight = Convert.ToDecimal(DecorOrderRow["Length"]) * Convert.ToDecimal(DecorOrderRow["Width"]) / 1000000
                         * Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);
            }
            decimal L = 0;

            if (Row[0]["WeightMeasureID"].ToString() == "2")
            {

                L = 0;

                L = Convert.ToDecimal(DecorOrderRow["Length"]);

                if (L == -1)
                    L = Convert.ToDecimal(DecorOrderRow["Height"]);

                Weight = Convert.ToDecimal(L) / 1000 * Convert.ToDecimal(Row[0]["Weight"]) *
                         Convert.ToDecimal(DecorOrderRow["Count"]);

            }
            if (Row[0]["WeightMeasureID"].ToString() == "3")
                Weight = Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);

            return Weight;
        }

        public void CreateParamsTable(string Params, DataTable DT)
        {
            string Param = null;

            for (int i = 0; i < Params.Length; i++)
            {
                if (Params[i] != ';')
                    Param += Params[i];

                if (Params[i] == ';' || i == Params.Length - 1)
                {
                    if (Param.Length > 0)
                    {
                        DT.Columns.Add(new DataColumn(Param, Type.GetType("System.Int32")));
                        Param = "";
                    }
                }
            }

            DT.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("TotalCount", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
        }

        private void GroupCoverTypes(DataRow[] Rows, int MeasureTypeID)
        {
            DataTable PDT = new DataTable();
            DataTable TDT = new DataTable();

            PDT = DecorOrdersDataTable.Clone();
            TDT = DecorOrdersDataTable.Clone();

            PDT.Columns.Remove("Count");
            PDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            PDT.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            PDT.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            PDT.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));

            TDT.Columns.Remove("Count");
            TDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            TDT.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            TDT.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            TDT.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));


            for (int r = 0; r < Rows.Count(); r++)
            {
                int D = Convert.ToInt32(Rows[r]["DecorConfigID"]);
                string InvNumber = Rows[r]["InvNumber"].ToString();
                //м.п.
                if (MeasureTypeID == 2)
                {
                    decimal L = 0;

                    L = Convert.ToDecimal(Rows[r]["Length"]);

                    if (L == -1)
                        L = Convert.ToDecimal(Rows[r]["Height"]);

                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        DataRow[] InvRows = PDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]) * L;
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                    else
                    {
                        DataRow[] InvRows = TDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]) * L;
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                }

                //шт.
                if (MeasureTypeID == 3)
                {
                    //get_parametrized_data function only
                }

                //м.кв.
                if (MeasureTypeID == 1)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        DataRow[] InvRows = PDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;
                                L = Convert.ToDecimal(Rows[r]["Length"]);
                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);
                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 3)
                            {
                                decimal H = 0;
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                                }
                            }
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;
                                L = Convert.ToDecimal(Rows[r]["Length"]);
                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);
                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                            }
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 3)
                            {
                                decimal H = 0;
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                            }
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                    else
                    {
                        DataRow[] InvRows = TDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        InvNumber = Rows[r]["InvNumber"].ToString();
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    NewRow["Count"] = Square;
                                }
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[0]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;
                                L = Convert.ToDecimal(Rows[r]["Length"]);
                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);
                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }

                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 3)
                            {
                                decimal H = 0;
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 1)
                            {
                                if (Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                                }
                                if (Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                {
                                    decimal d = Convert.ToDecimal(Rows[r]["Length"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                    d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                    decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                    InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                                }
                            }
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 2)
                            {
                                decimal L = 0;
                                L = Convert.ToDecimal(Rows[r]["Length"]);
                                if (L == -1)
                                    L = Convert.ToDecimal(Rows[r]["Height"]);
                                decimal d = L * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                            }
                            if (GetMeasureTypeID(Convert.ToInt32(Rows[r]["DecorConfigID"])) == 3)
                            {
                                decimal H = 0;
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                            }
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                }
            }




            //REPORT TABLE
            //м.п.
            if (MeasureTypeID == 2)
            {
                if (PDT.Rows.Count > 0)
                {
                    for (int i = 0; i < PDT.Rows.Count; i++)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]) / (Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);
                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]) / (Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            //шт.
            if (MeasureTypeID == 3)
            {
                //get_parametrized_data function only
            }

            //м.кв.
            if (MeasureTypeID == 1)
            {
                if (PDT.Rows.Count > 0)
                {
                    for (int i = 0; i < PDT.Rows.Count; i++)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            PDT.Dispose();
            TDT.Dispose();
        }

        private void GetParametrizedData(DataRow[] Rows, DataTable PDT, DataTable TDT)
        {
            string p1 = "";
            string p2 = "";
            string p3 = "";

            if (PDT.Columns["Height"] != null)
                p1 = "Height";

            if (PDT.Columns["Length"] != null)
                p1 = "Length";

            if (PDT.Columns["Width"] != null)
                p2 = "Width";

            if (p1.Length > 0 && p2.Length == 0)
                p3 = p1;

            if (p1.Length == 0 && p2.Length > 0)
                p3 = p2;



            if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
            {
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        DataRow[] InvRows = PDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                    else
                    {
                        DataRow[] InvRows = TDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                }

            }



            if (p1.Length > 0 && p2.Length > 0)
            {
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        DataRow[] InvRows = PDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow[p1] = Convert.ToDecimal(Rows[r][p1]);
                            NewRow[p2] = Convert.ToDecimal(Rows[r][p2]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                    else
                    {
                        DataRow[] InvRows = TDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow[p1] = Convert.ToDecimal(Rows[r][p1]);
                            NewRow[p2] = Convert.ToDecimal(Rows[r][p2]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                }
            }

            if (p3.Length > 0)
            {
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (IsProfil(Convert.ToInt32(Rows[r]["DecorConfigID"])))
                    {
                        DataRow[] InvRows = PDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = PDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow[p3] = Convert.ToDecimal(Rows[r][p3]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                    else
                    {
                        DataRow[] InvRows = TDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["UNN"] = UNN;
                            NewRow["PaymentRate"] = PaymentRate;
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["CurrencyCode"] = ProfilCurrencyCode;
                            NewRow["TPSCurCode"] = TPSCurrencyCode;
                            NewRow[p3] = Convert.ToDecimal(Rows[r][p3]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["PriceWithTransport"] = Convert.ToDecimal(Rows[r]["PriceWithTransport"]);
                            NewRow["CostWithTransport"] = Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["CostWithTransport"] = Convert.ToDecimal(InvRows[0]["CostWithTransport"]) +
                                Convert.ToDecimal(Rows[r]["CostWithTransport"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                }
            }





            //REPORT TABLES
            if (PDT.Rows.Count > 0)
            {
                for (int g = 0; g < PDT.Rows.Count; g++)
                {
                    if (p1.Length > 0 && p2.Length > 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(PDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }
            }

            if (TDT.Rows.Count > 0)
            {
                for (int g = 0; g < TDT.Rows.Count; g++)
                {
                    if (p1.Length > 0 && p2.Length > 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["UNN"] = UNN;
                        NewRow["PaymentRate"] = PaymentRate;
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["CurrencyCode"] = ProfilCurrencyCode;
                        NewRow["TPSCurCode"] = TPSCurrencyCode;
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["PriceWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]) / Convert.ToDecimal(TDT.Rows[g]["Count"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["CostWithTransport"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["CostWithTransport"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 2, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }


        }

        private void Collect()
        {
            DataTable DistRatesDT = new DataTable();
            DataTable Items = new DataTable();

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Items = DV.ToTable(true, new string[] { "InvNumber" });
            }

            //get count of different covertypes
            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                DistRatesDT = DV.ToTable(true, new string[] { "PaymentRate" });
            }

            for (int i = 0; i < Items.Rows.Count; i++)
            {
                for (int j = 0; j < DistRatesDT.Rows.Count; j++)
                {
                    PaymentRate = Convert.ToDecimal(DistRatesDT.Rows[j]["PaymentRate"]);

                    string InvNumber = Items.Rows[i]["InvNumber"].ToString();
                    DataRow[] ItemsRows = DecorOrdersDataTable.Select("PaymentRate='" + PaymentRate.ToString() + "' AND InvNumber = '" + InvNumber + "'",
                        "Price ASC");

                    if (ItemsRows.Count() == 0)
                        continue;
                    int DecorConfigID = Convert.ToInt32(ItemsRows[0]["DecorConfigID"]);
                    //м.п.
                    if (GetReportMeasureTypeID(Convert.ToInt32(ItemsRows[0]["DecorConfigID"])) == 2)
                    {
                        GroupCoverTypes(ItemsRows, 2);
                    }


                    //шт.
                    if (GetReportMeasureTypeID(Convert.ToInt32(ItemsRows[0]["DecorConfigID"])) == 3)
                    {
                        DataTable ParamTableProfil = new DataTable();
                        DataTable ParamTableTPS = new DataTable();

                        DataRow[] DCs = DecorConfigDataTable.Select("DecorConfigID = " +
                            ItemsRows[0]["DecorConfigID"].ToString());

                        CreateParamsTable(DCs[0]["ReportParam"].ToString(), ParamTableProfil);
                        CreateParamsTable(DCs[0]["ReportParam"].ToString(), ParamTableTPS);

                        GetParametrizedData(ItemsRows, ParamTableProfil, ParamTableTPS);

                        ParamTableProfil.Dispose();
                        ParamTableTPS.Dispose();
                    }


                    //м.кв.
                    if (GetReportMeasureTypeID(Convert.ToInt32(ItemsRows[0]["DecorConfigID"])) == 1)
                    {
                        GroupCoverTypes(ItemsRows, 1);
                    }
                }
            }

            Items.Dispose();
        }

        public void Report(int MegaOrderID, decimal dPaymentRate, bool IsSample)
        {
            GetMegaOrderInfo1(MegaOrderID);
            DecorOrdersDataTable.Clear();
            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();
            string SelectCommand = "SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders" +
                " INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID AND NewMegaOrders.MegaOrderID=" + MegaOrderID +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE NeedCalcPrice=0 AND NewDecorOrders.IsSample=1 AND InvNumber IS NOT NULL ORDER BY InvNumber";
            if (!IsSample)
                SelectCommand = "SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders" +
                    " INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID" +
                    " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID AND NewMegaOrders.MegaOrderID=" + MegaOrderID +
                    " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                    " WHERE NeedCalcPrice=0 AND NewDecorOrders.IsSample=0 AND InvNumber IS NOT NULL ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            for (int j = 0; j < DecorOrdersDataTable.Rows.Count; j++)
            {
                DecorOrdersDataTable.Rows[j]["PaymentRate"] = dPaymentRate;
            }
            Collect();

        }

        public void Report(int MegaOrderID, decimal dPaymentRate)
        {
            GetMegaOrderInfo1(MegaOrderID);
            DecorOrdersDataTable.Clear();
            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();
            string SelectCommand = "SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders" +
                " INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID AND NewMegaOrders.MegaOrderID=" + MegaOrderID +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE InvNumber IS NOT NULL ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            for (int j = 0; j < DecorOrdersDataTable.Rows.Count; j++)
            {
                DecorOrdersDataTable.Rows[j]["PaymentRate"] = dPaymentRate;
            }
            Collect();

        }

        public void Report(int[] MainOrderIDs, bool IsSample)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);
            DecorOrdersDataTable.Clear();
            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();
            string sWhere = "";

            for (int i = 0; i < MainOrderIDs.Count(); i++)
            {
                if (sWhere != "")
                    sWhere += " OR MainOrderID = " + MainOrderIDs[i].ToString();
                else
                    sWhere += "MainOrderID = " + MainOrderIDs[i].ToString();
            }

            string SelectCommand = "SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders" +
                " INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE NewDecorOrders.IsSample=1 AND InvNumber IS NOT NULL AND NewDecorOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            if (!IsSample)
                SelectCommand = "SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders" +
                    " INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID" +
                    " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID" +
                    " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                    " WHERE NewDecorOrders.IsSample=0 AND InvNumber IS NOT NULL AND NewDecorOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            Collect();

        }

        public void Report(int[] MainOrderIDs)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);
            DecorOrdersDataTable.Clear();
            ProfilReportDataTable.Clear();
            TPSReportDataTable.Clear();

            string SelectCommand = "SELECT NewDecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, NewMegaOrders.PaymentRate FROM NewDecorOrders" +
                " INNER JOIN NewMainOrders ON NewDecorOrders.MainOrderID = NewMainOrders.MainOrderID" +
                " INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID = NewMegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE InvNumber IS NOT NULL AND NewDecorOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            Collect();

        }
    }







    public class InvoiceReportToDBF
    {
        decimal VAT = 1.0m;
        public InvoiceFrontsReportToDBF FrontsReport;
        public InvoiceDecorReportToDBF DecorReport = null;
        //HSSFWorkbook hssfworkbook;
        public DataTable CurrencyTypesDataTable = null;
        public DataTable ProfilReportTable = null;
        public DataTable TPSReportTable = null;

        public InvoiceReportToDBF()
        {
            FrontsReport = new InvoiceFrontsReportToDBF();
            DecorReport = new InvoiceDecorReportToDBF();

            CreateProfilReportTable();

            CurrencyTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }
        }

        private void CreateProfilReportTable()
        {
            ProfilReportTable = new DataTable();

            ProfilReportTable.Columns.Add(new DataColumn("UNN", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("CurrencyCode", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("TPSCurCode", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("PackageCount", Type.GetType("System.Int32")));
            ProfilReportTable.Columns.Add(new DataColumn("OriginalPrice", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("PriceWithTransport", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("CostWithTransport", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("IsNonStandard", Type.GetType("System.Boolean")));
            ProfilReportTable.Columns.Add(new DataColumn("NonStandardMargin", Type.GetType("System.Decimal")));
            TPSReportTable = ProfilReportTable.Clone();
        }

        public void Collect(DataTable table1, DataTable table2, bool IsNonStandard)
        {
            DataTable TempDT = ProfilReportTable.Clone();
            string IsNonStandardFilter = "(IsNonStandard=0 OR IsNonStandard IS NULL)";
            if (IsNonStandard)
                IsNonStandardFilter = "IsNonStandard=1";
            DataTable DistinctInvNumbersDT;
            using (DataView DV = new DataView(table1))
            {
                DV.RowFilter = IsNonStandardFilter;
                DistinctInvNumbersDT = DV.ToTable(true, new string[] { "AccountingName", "InvNumber", "PaymentRate" });
            }
            IsNonStandardFilter = " AND " + IsNonStandardFilter;
            for (int i = 0; i < DistinctInvNumbersDT.Rows.Count; i++)
            {
                decimal Cost = 0;
                decimal PriceWithTransport = 0;
                decimal CostWithTransport = 0;
                decimal Count = 0;
                decimal Price = 0;
                decimal Weight = 0;
                int DecCount = 2;
                int PackageCount = 0;
                //decimal OriginalPrice = Convert.ToDecimal(table1.Rows[0]["OriginalPrice"]);
                decimal NonStandardMargin = 0;
                string UNN = table1.Rows[0]["UNN"].ToString();
                string AccountingName = DistinctInvNumbersDT.Rows[i]["AccountingName"].ToString();
                string InvNumber = DistinctInvNumbersDT.Rows[i]["InvNumber"].ToString();
                string CurrencyCode = table1.Rows[0]["CurrencyCode"].ToString();
                decimal PaymentRate = Convert.ToDecimal(DistinctInvNumbersDT.Rows[i]["PaymentRate"]);
                if (table1.Rows[0]["NonStandardMargin"] != DBNull.Value)
                    NonStandardMargin = Convert.ToDecimal(table1.Rows[0]["NonStandardMargin"]);

                DataRow[] InvRows = table1.Select("InvNumber = '" + InvNumber + "' AND PaymentRate='" + PaymentRate + "' AND AccountingName='" + AccountingName + "'" + IsNonStandardFilter);
                if (InvRows.Count() > 0)
                {
                    for (int j = 0; j < InvRows.Count(); j++)
                    {
                        if (InvRows[j]["NonStandardMargin"] != DBNull.Value)
                            NonStandardMargin = Convert.ToDecimal(InvRows[j]["NonStandardMargin"]);
                        Weight += Convert.ToDecimal(InvRows[j]["Weight"]);
                        Count += Convert.ToDecimal(InvRows[j]["Count"]);
                        Cost += Convert.ToDecimal(InvRows[j]["Cost"]);
                        CostWithTransport += Convert.ToDecimal(InvRows[j]["CostWithTransport"]);
                    }
                    Cost = Cost * PaymentRate / VAT;
                    CostWithTransport = CostWithTransport * PaymentRate / VAT;
                    Cost = Math.Ceiling(Cost / 0.01m) * 0.01m;
                    CostWithTransport = Math.Ceiling(CostWithTransport / 0.01m) * 0.01m;
                    Price = Cost / Count;
                    PriceWithTransport = CostWithTransport / Count;
                    //if (Convert.ToInt32(CurrencyCode) == 974)
                    //{
                    //    DecCount = 0;
                    //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                    //    Cost = Price * Count;
                    //    PriceWithTransport = Math.Ceiling(PriceWithTransport / 100.0m) * 100.0m;
                    //    CostWithTransport = PriceWithTransport * Count;
                    //}
                    Price = Decimal.Round(Price, DecCount, MidpointRounding.AwayFromZero);
                    Cost = Decimal.Round(Cost, DecCount, MidpointRounding.AwayFromZero);
                    PriceWithTransport = Decimal.Round(PriceWithTransport, DecCount, MidpointRounding.AwayFromZero);
                    CostWithTransport = Decimal.Round(CostWithTransport, DecCount, MidpointRounding.AwayFromZero);

                    DataRow NewRow = TempDT.NewRow();
                    NewRow["UNN"] = UNN;
                    NewRow["CurrencyCode"] = CurrencyCode;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["Count"] = Count;
                    NewRow["Price"] = Price;
                    NewRow["Cost"] = Cost;
                    NewRow["Weight"] = Weight;
                    NewRow["PackageCount"] = PackageCount;
                    //NewRow["OriginalPrice"] = OriginalPrice;
                    NewRow["PriceWithTransport"] = PriceWithTransport;
                    NewRow["CostWithTransport"] = CostWithTransport;
                    NewRow["PaymentRate"] = PaymentRate;
                    NewRow["IsNonStandard"] = IsNonStandard;
                    if (NonStandardMargin != 0)
                        NewRow["NonStandardMargin"] = NonStandardMargin;
                    TempDT.Rows.Add(NewRow);
                }
            }

            foreach (DataRow item in TempDT.Rows)
            {
                DataRow NewRow = ProfilReportTable.NewRow();
                NewRow.ItemArray = item.ItemArray;
                ProfilReportTable.Rows.Add(NewRow);
            }

            TempDT.Clear();
            IsNonStandardFilter = "(IsNonStandard=0 OR IsNonStandard IS NULL)";
            if (IsNonStandard)
                IsNonStandardFilter = "IsNonStandard=1";
            DistinctInvNumbersDT.Dispose();
            using (DataView DV = new DataView(table2))
            {
                DV.RowFilter = IsNonStandardFilter;
                DistinctInvNumbersDT = DV.ToTable(true, new string[] { "AccountingName", "InvNumber", "PaymentRate" });
            }
            IsNonStandardFilter = " AND " + IsNonStandardFilter;
            for (int i = 0; i < DistinctInvNumbersDT.Rows.Count; i++)
            {
                decimal Cost = 0;
                decimal PriceWithTransport = 0;
                decimal CostWithTransport = 0;
                decimal Count = 0;
                decimal Price = 0;
                decimal Weight = 0;
                int DecCount = 2;
                int PackageCount = 0;
                //decimal OriginalPrice = Convert.ToDecimal(table2.Rows[0]["OriginalPrice"]);
                decimal NonStandardMargin = 0;
                string UNN = table2.Rows[0]["UNN"].ToString();
                string AccountingName = DistinctInvNumbersDT.Rows[i]["AccountingName"].ToString();
                string InvNumber = DistinctInvNumbersDT.Rows[i]["InvNumber"].ToString();
                string CurrencyCode = table2.Rows[0]["CurrencyCode"].ToString();
                string TPSCurrencyCode = table2.Rows[0]["TPSCurCode"].ToString();
                decimal PaymentRate = Convert.ToDecimal(DistinctInvNumbersDT.Rows[i]["PaymentRate"]);
                DataRow[] InvRows = table2.Select("InvNumber = '" + InvNumber + "' AND PaymentRate='" + PaymentRate + "' AND AccountingName='" + AccountingName + "'" + IsNonStandardFilter);
                if (InvRows.Count() > 0)
                {
                    for (int j = 0; j < InvRows.Count(); j++)
                    {
                        if (InvRows[j]["NonStandardMargin"] != DBNull.Value)
                            NonStandardMargin = Convert.ToDecimal(InvRows[j]["NonStandardMargin"]);
                        Weight += Convert.ToDecimal(InvRows[j]["Weight"]);
                        Count += Convert.ToDecimal(InvRows[j]["Count"]);
                        Cost += Convert.ToDecimal(InvRows[j]["Cost"]) * PaymentRate / VAT;
                        CostWithTransport += Convert.ToDecimal(InvRows[j]["CostWithTransport"]) * PaymentRate / VAT;
                    }
                    Cost = Math.Ceiling(Cost / 0.01m) * 0.01m;
                    CostWithTransport = Math.Ceiling(CostWithTransport / 0.01m) * 0.01m;
                    Price = Cost / Count;
                    PriceWithTransport = CostWithTransport / Count;
                    //if (Convert.ToInt32(CurrencyCode) == 974)
                    //{
                    //    DecCount = 0;
                    //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                    //    Cost = Price * Count;
                    //    PriceWithTransport = Math.Ceiling(PriceWithTransport / 100.0m) * 100.0m;
                    //    CostWithTransport = PriceWithTransport * Count;
                    //}
                    Price = Decimal.Round(Price, DecCount, MidpointRounding.AwayFromZero);
                    Cost = Decimal.Round(Cost, DecCount, MidpointRounding.AwayFromZero);
                    PriceWithTransport = Decimal.Round(PriceWithTransport, DecCount, MidpointRounding.AwayFromZero);
                    CostWithTransport = Decimal.Round(CostWithTransport, DecCount, MidpointRounding.AwayFromZero);

                    DataRow NewRow = TempDT.NewRow();
                    NewRow["UNN"] = UNN;
                    NewRow["CurrencyCode"] = CurrencyCode;
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["Count"] = Count;
                    NewRow["Price"] = Price;
                    NewRow["Cost"] = Cost;
                    NewRow["Weight"] = Weight;
                    NewRow["PackageCount"] = PackageCount;
                    //NewRow["OriginalPrice"] = OriginalPrice;
                    NewRow["PriceWithTransport"] = PriceWithTransport;
                    NewRow["CostWithTransport"] = CostWithTransport;
                    NewRow["PaymentRate"] = PaymentRate;
                    NewRow["IsNonStandard"] = IsNonStandard;
                    if (NonStandardMargin != 0)
                        NewRow["NonStandardMargin"] = NonStandardMargin;
                    TempDT.Rows.Add(NewRow);
                }
            }
            foreach (DataRow item in TempDT.Rows)
            {
                DataRow NewRow = TPSReportTable.NewRow();
                NewRow.ItemArray = item.ItemArray;
                TPSReportTable.Rows.Add(NewRow);
            }
        }

        public void AssignCost(decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost, decimal WeightProfil, decimal WeightTPS, decimal TotalWeight,
            ref decimal TransportAndOtherProfil, ref decimal TransportAndOtherTPS)
        {
            decimal Total = TransportCost + AdditionalCost;

            if (Total == 0 && ComplaintProfilCost == 0 && ComplaintTPSCost == 0)
                return;

            decimal pProfil = 0;
            decimal pTPS = 0;

            decimal cProfil = 0;
            decimal cTPS = 0;

            pProfil = WeightProfil / (TotalWeight / 100);
            pTPS = WeightTPS / (TotalWeight / 100);

            cProfil = Total / 100 * pProfil - ComplaintProfilCost;
            cTPS = Total / 100 * pTPS - ComplaintTPSCost;

            TransportAndOtherProfil = Decimal.Round(cProfil, 1, MidpointRounding.AwayFromZero);
            TransportAndOtherTPS = Decimal.Round(cTPS, 1, MidpointRounding.AwayFromZero);
        }

        private int GetMegaOrderID(int MainOrderID)
        {
            int MegaOrderID = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM NewMainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                }
            }
            return MegaOrderID;
        }

        private bool IsComplaint(int MegaOrderID)
        {
            bool IsComplaint = false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT IsComplaint" +
                            " FROM NewMegaOrders WHERE MegaOrderID = " + MegaOrderID,
                            ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return false;

                        if (DBNull.Value != DT.Rows[0]["IsComplaint"] && Convert.ToInt32(DT.Rows[0]["IsComplaint"]) > 0)
                        {
                            IsComplaint = Convert.ToBoolean(DT.Rows[0]["IsComplaint"]);
                        }
                    }
                }
            }

            return IsComplaint;
        }

        public DataTable GroupBy(string i_sGroupByColumn, string i_sAggregateColumn, DataTable i_dSourceTable)
        {
            DataView dv = new DataView(i_dSourceTable);

            //getting distinct values for group column
            DataTable dtGroup = dv.ToTable(true, new string[] { i_sGroupByColumn });

            //adding column for the row count
            dtGroup.Columns.Add("Count", typeof(int));

            //looping thru distinct values for the group, counting
            foreach (DataRow dr in dtGroup.Rows)
            {
                dr["Count"] = i_dSourceTable.Compute("Count(" + i_sAggregateColumn + ")", i_sGroupByColumn + " = '" + dr[i_sGroupByColumn] + "'");
            }
            dv.Dispose();
            dv = new DataView(dtGroup.Copy())
            {
                Sort = "Count"
            };
            dtGroup.Clear();
            dtGroup = dv.ToTable();

            //returning grouped/counted result
            return dtGroup;
        }

        public DataTable GetMegaOrdersTable(int[] NewMegaOrders)
        {
            DataTable ReturnDT = new DataTable();
            string SelectCommand = @"SELECT NewMegaOrders.MegaOrderID, NewMegaOrders.OrderNumber, NewMegaOrders.ClientID, 
                NewMegaOrders.ComplaintProfilCost, NewMegaOrders.ComplaintTPSCost, NewMegaOrders.TransportCost, NewMegaOrders.AdditionalCost, NewMegaOrders.CurrencyTypeID,
                NewMainOrders.MainOrderID, NewMainOrders.Weight
                FROM NewMainOrders
                INNER JOIN NewMegaOrders ON NewMainOrders.MegaOrderID=NewMegaOrders.MegaOrderID
                AND NewMegaOrders.MegaOrderID IN (" + string.Join(",", NewMegaOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(ReturnDT);
                ReturnDT.Columns.Add(new DataColumn("ProfilFrontsWeight", Type.GetType("System.Decimal")));
                ReturnDT.Columns.Add(new DataColumn("TPSFrontsWeight", Type.GetType("System.Decimal")));
                ReturnDT.Columns.Add(new DataColumn("ProfilDecorWeight", Type.GetType("System.Decimal")));
                ReturnDT.Columns.Add(new DataColumn("TPSDecorWeight", Type.GetType("System.Decimal")));
                for (int i = 0; i < ReturnDT.Rows.Count; i++)
                {
                    decimal ProfilFrontsWeight = 0;
                    decimal TPSFrontsWeight = 0;
                    decimal ProfilDecorWeight = 0;
                    decimal TPSDecorWeight = 0;
                    int MainOrderID = Convert.ToInt32(ReturnDT.Rows[i]["MainOrderID"]);
                    SelectCommand = @"SELECT Square,Weight,FactoryID FROM NewFrontsOrders WHERE MainOrderID=" + MainOrderID;
                    using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA1.Fill(DT);
                            for (int j = 0; j < DT.Rows.Count; j++)
                            {
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 1)
                                    ProfilFrontsWeight += Convert.ToDecimal(DT.Rows[j]["Square"]) * Convert.ToDecimal(0.7) + Convert.ToDecimal(DT.Rows[j]["Weight"]);
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 2)
                                    TPSFrontsWeight += Convert.ToDecimal(DT.Rows[j]["Square"]) * Convert.ToDecimal(0.7) + Convert.ToDecimal(DT.Rows[j]["Weight"]);
                            }
                        }
                    }
                    SelectCommand = @"SELECT Weight,FactoryID FROM NewDecorOrders WHERE MainOrderID=" + MainOrderID;
                    using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA1.Fill(DT);
                            for (int j = 0; j < DT.Rows.Count; j++)
                            {
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 1)
                                    ProfilDecorWeight += Convert.ToDecimal(DT.Rows[j]["Weight"]);
                                if (Convert.ToInt32(DT.Rows[j]["FactoryID"]) == 2)
                                    TPSDecorWeight += Convert.ToDecimal(DT.Rows[j]["Weight"]);
                            }
                        }
                    }
                    ReturnDT.Rows[i]["ProfilFrontsWeight"] = ProfilFrontsWeight;
                    ReturnDT.Rows[i]["TPSFrontsWeight"] = TPSFrontsWeight;
                    ReturnDT.Rows[i]["ProfilDecorWeight"] = ProfilDecorWeight;
                    ReturnDT.Rows[i]["TPSDecorWeight"] = TPSDecorWeight;
                }
            }
            return ReturnDT;
        }

        public decimal CalcCurrencyCost(int MegaOrderID, int ClientID, int CurrencyTypeID, decimal PaymentRate, bool IsSample)
        {
            ClearReport();
            if (ClientID == 145)
            {
                VAT = 1.2m;
            }
            else
            {
                VAT = 1.0m;
            }

            FrontsReport.Report(MegaOrderID, PaymentRate, IsSample);
            DecorReport.Report(MegaOrderID, PaymentRate, IsSample);

            //PROFIL
            if (FrontsReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(FrontsReport.ProfilReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(DecorReport.ProfilReportDataTable.Rows[i]);
                }
            }

            //TPS
            if (FrontsReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(FrontsReport.TPSReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(DecorReport.TPSReportDataTable.Rows[i]);
                }
            }

            decimal TotalCost = 0;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
            }

            TotalCost = (TotalProfil + TotalTPS);
            TotalCost = Decimal.Round(TotalCost, 2, MidpointRounding.AwayFromZero);

            TotalCost = 0;
            TotalProfil = 0;
            TotalTPS = 0;

            DataTable table1 = ProfilReportTable.Copy();
            DataTable table2 = TPSReportTable.Copy();
            ProfilReportTable.Clear();
            TPSReportTable.Clear();
            Collect(table1, table2, false);
            Collect(table1, table2, true);

            using (DataView DV = new DataView(ProfilReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                ProfilReportTable.Clear();
                ProfilReportTable = DV.ToTable();
            }
            using (DataView DV = new DataView(TPSReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                TPSReportTable.Clear();
                TPSReportTable = DV.ToTable();
            }

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
            }

            TotalCost = (TotalProfil + TotalTPS);
            TotalCost = Decimal.Round(TotalCost, 2, MidpointRounding.AwayFromZero);
            return TotalCost;
        }

        public decimal CalcCurrencyCost(int MegaOrderID, int ClientID, int CurrencyTypeID, decimal PaymentRate)
        {
            ClearReport();
            if (ClientID == 145)
            {
                VAT = 1.2m;
            }
            else
            {
                VAT = 1.0m;
            }

            FrontsReport.Report(MegaOrderID, PaymentRate);
            DecorReport.Report(MegaOrderID, PaymentRate);

            //PROFIL
            if (FrontsReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(FrontsReport.ProfilReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(DecorReport.ProfilReportDataTable.Rows[i]);
                }
            }

            //TPS
            if (FrontsReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(FrontsReport.TPSReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(DecorReport.TPSReportDataTable.Rows[i]);
                }
            }

            decimal TotalCost = 0;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
            }

            TotalCost = (TotalProfil + TotalTPS);
            TotalCost = Decimal.Round(TotalCost, 2, MidpointRounding.AwayFromZero);

            TotalCost = 0;
            TotalProfil = 0;
            TotalTPS = 0;

            DataTable table1 = ProfilReportTable.Copy();
            DataTable table2 = TPSReportTable.Copy();
            ProfilReportTable.Clear();
            TPSReportTable.Clear();
            Collect(table1, table2, false);
            Collect(table1, table2, true);

            using (DataView DV = new DataView(ProfilReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                ProfilReportTable.Clear();
                ProfilReportTable = DV.ToTable();
            }
            using (DataView DV = new DataView(TPSReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                TPSReportTable.Clear();
                TPSReportTable = DV.ToTable();
            }

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
            }

            TotalCost = (TotalProfil + TotalTPS);
            TotalCost = Decimal.Round(TotalCost, 2, MidpointRounding.AwayFromZero);
            return TotalCost;
        }

        public void CreateReport(ref HSSFWorkbook hssfworkbook, int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight, bool IsSample, ref decimal TotalProfil1, ref decimal TotalTPS1)
        {
            ClearReport();
            DataTable ReturnDT = GetMegaOrdersTable(NewMegaOrders);
            string MainOrdersList = string.Empty;

            int pos = 0;

            string Currency = string.Empty;

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();
            //if (ClientID == 145 || ClientID == 258 || ClientID == 267)
            if (ClientID == 145)
            {
                VAT = 1.2m;
            }
            else
            {
                VAT = 1.0m;
            }
            TransportCost = TransportCost / VAT;
            AdditionalCost = AdditionalCost / VAT;
            FrontsReport.Report(MainOrdersIDs, IsSample);
            DecorReport.Report(MainOrdersIDs, IsSample);

            //PROFIL
            if (FrontsReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(FrontsReport.ProfilReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(DecorReport.ProfilReportDataTable.Rows[i]);
                }
            }

            //TPS
            if (FrontsReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(FrontsReport.TPSReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(DecorReport.TPSReportDataTable.Rows[i]);
                }
            }

            decimal Total = TransportCost + AdditionalCost - ComplaintProfilCost - ComplaintTPSCost;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;
            decimal WeightProfil = 0;
            decimal WeightTPS = 0;

            decimal TransportAndOtherProfil = 0;
            decimal TransportAndOtherTPS = 0;

            DataTable table1 = ProfilReportTable.Copy();
            DataTable table2 = TPSReportTable.Copy();
            ProfilReportTable.Clear();
            TPSReportTable.Clear();
            Collect(table1, table2, false);
            Collect(table1, table2, true);
            using (DataView DV = new DataView(ProfilReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                ProfilReportTable.Clear();
                ProfilReportTable = DV.ToTable();
            }
            using (DataView DV = new DataView(TPSReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                TPSReportTable.Clear();
                TPSReportTable = DV.ToTable();
            }

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                WeightProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                WeightTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
            }

            TotalCost = (TotalProfil + TotalTPS);

            //Assign COST
            AssignCost(ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost, WeightProfil, WeightTPS, TotalWeight, ref TransportAndOtherProfil,
                       ref TransportAndOtherTPS);

            TotalProfil1 = (TotalProfil);
            TotalTPS1 = (TotalTPS);
            //decimal dd = Decimal.Round((WeightProfil + WeightTPS) / TotalWeight, 3, MidpointRounding.AwayFromZero);
            decimal dd = 0;
            if (TotalWeight != 0)
                dd = Decimal.Round((WeightProfil + WeightTPS) / TotalWeight, 3, MidpointRounding.AwayFromZero);
            TotalProfil = Decimal.Round(TotalProfil, 3, MidpointRounding.AwayFromZero);
            TotalTPS = Decimal.Round(TotalTPS, 3, MidpointRounding.AwayFromZero);
            TransportCost = Decimal.Round(TransportCost * dd, 3, MidpointRounding.AwayFromZero);
            AdditionalCost = Decimal.Round(AdditionalCost * dd, 3, MidpointRounding.AwayFromZero);
            TotalCost = Decimal.Round(TotalCost, 3, MidpointRounding.AwayFromZero);

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                ProfilReportTable.Rows[i]["PackageCount"] = 0;
            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                TPSReportTable.Rows[i]["PackageCount"] = 0;

            DataTable dtGroup = new DataTable();
            DataTable dtMainOrders = new DataTable();
            DataTable DT = null;
            DataTable TempDT = new DataTable();
            DataTable dtDistInvNumbers = new DataTable();
            DataTable DistPackagesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.PackNumber, infiniu2_catalog.dbo.FrontsConfig.InvNumber, infiniu2_catalog.dbo.FrontsConfig.FactoryID, NewFrontsOrders.MainOrderID
                FROM PackageDetails INNER JOIN
                NewFrontsOrders ON PackageDetails.OrderID = NewFrontsOrders.FrontsOrdersID INNER JOIN
                infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID
                WHERE PackageDetails.PackageID IN 
                (SELECT PackageID FROM Packages WHERE ProductType = 0 AND MainOrderID IN (" + string.Join(",", MainOrdersIDs) + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            if (DT == null)
                DT = TempDT.Clone();
            foreach (DataRow item in TempDT.Rows)
                DT.Rows.Add(item.ItemArray);
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.PackNumber, infiniu2_catalog.dbo.DecorConfig.InvNumber, infiniu2_catalog.dbo.DecorConfig.FactoryID, NewDecorOrders.MainOrderID
                FROM PackageDetails INNER JOIN
                NewDecorOrders ON PackageDetails.OrderID = NewDecorOrders.DecorOrderID INNER JOIN
                infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                WHERE PackageDetails.PackageID IN 
                (SELECT PackageID FROM Packages WHERE ProductType = 1 AND MainOrderID IN (" + string.Join(",", MainOrdersIDs) + "))",
                 ConnectionStrings.MarketingOrdersConnectionString))
            {
                TempDT.Clear();
                DA.Fill(TempDT);
            }
            if (DT == null)
                DT = TempDT.Clone();
            foreach (DataRow item in TempDT.Rows)
                DT.Rows.Add(item.ItemArray);

            for (int z = 0; z < MainOrdersIDs.Count(); z++)
            {
                using (DataView DV = new DataView(DT, "MainOrderID=" + MainOrdersIDs[z], string.Empty, DataViewRowState.CurrentRows))
                {
                    dtMainOrders.Clear();
                    dtMainOrders = DV.ToTable();
                }
                using (DataView DV = new DataView(dtMainOrders, "FactoryID=1", string.Empty, DataViewRowState.CurrentRows))
                {
                    DistPackagesDT.Clear();
                    DistPackagesDT = DV.ToTable(true, "PackNumber");
                }
                dtGroup.Clear();
                dtGroup = GroupBy("InvNumber", "PackNumber", dtMainOrders);
                for (int i = 0; i < DistPackagesDT.Rows.Count; i++)
                {
                    using (DataView DV = new DataView(dtMainOrders, "PackNumber=" + Convert.ToInt32(DistPackagesDT.Rows[i]["PackNumber"]), string.Empty, DataViewRowState.CurrentRows))
                    {
                        dtDistInvNumbers.Clear();
                        dtDistInvNumbers = DV.ToTable(true, "InvNumber");
                        string InvNumber = dtDistInvNumbers.Rows[0]["InvNumber"].ToString();
                        int PackageCount = 0;

                        DataRow[] rows0 = dtGroup.Select("InvNumber='" + InvNumber + "'");
                        if (rows0.Count() > 0)
                            PackageCount = Convert.ToInt32(rows0[0]["Count"]);

                        for (int x = 1; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = dtGroup.Select("InvNumber='" + dtDistInvNumbers.Rows[x]["InvNumber"].ToString() + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["Count"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["Count"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        PackageCount = 0;
                        for (int x = 0; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = ProfilReportTable.Select("InvNumber='" + InvNumber + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["PackageCount"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["PackageCount"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        DataRow[] rows = ProfilReportTable.Select("InvNumber='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            rows[0]["PackageCount"] = Convert.ToInt32(rows[0]["PackageCount"]) + 1;
                    }
                }



                using (DataView DV = new DataView(dtMainOrders, "FactoryID=2", string.Empty, DataViewRowState.CurrentRows))
                {
                    DistPackagesDT.Clear();
                    DistPackagesDT = DV.ToTable(true, "PackNumber");
                }
                dtGroup.Clear();
                dtGroup = GroupBy("InvNumber", "PackNumber", dtMainOrders);
                for (int i = 0; i < DistPackagesDT.Rows.Count; i++)
                {
                    using (DataView DV = new DataView(dtMainOrders, "PackNumber=" + Convert.ToInt32(DistPackagesDT.Rows[i]["PackNumber"]), string.Empty, DataViewRowState.CurrentRows))
                    {
                        dtDistInvNumbers.Clear();
                        dtDistInvNumbers = DV.ToTable(true, "InvNumber");
                        string InvNumber = dtDistInvNumbers.Rows[0]["InvNumber"].ToString();
                        int PackageCount = 0;

                        DataRow[] rows0 = dtGroup.Select("InvNumber='" + InvNumber + "'");
                        if (rows0.Count() > 0)
                            PackageCount = Convert.ToInt32(rows0[0]["Count"]);

                        for (int x = 1; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = dtGroup.Select("InvNumber='" + dtDistInvNumbers.Rows[x]["InvNumber"].ToString() + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["Count"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["Count"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        PackageCount = 0;
                        for (int x = 0; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = TPSReportTable.Select("InvNumber='" + InvNumber + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["PackageCount"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["PackageCount"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        DataRow[] rows = TPSReportTable.Select("InvNumber='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            rows[0]["PackageCount"] = Convert.ToInt32(rows[0]["PackageCount"]) + 1;
                    }
                }
            }

            #region 

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 11;
            HeaderF1.Boldweight = 11 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 10;
            HeaderF2.Boldweight = 10 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 8;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.000");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            HSSFCellStyle WeightCS = hssfworkbook.CreateCellStyle();
            WeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            WeightCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            WeightCS.BottomBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            WeightCS.LeftBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            WeightCS.RightBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            WeightCS.TopBorderColor = HSSFColor.BLACK.index;
            WeightCS.SetFont(SimpleF);

            HSSFCellStyle PriceBelCS = hssfworkbook.CreateCellStyle();
            PriceBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            PriceBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.SetFont(SimpleF);

            HSSFCellStyle PriceForeignCS = hssfworkbook.CreateCellStyle();
            PriceForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            PriceForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.SetFont(SimpleF);

            HSSFCellStyle CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.0000");
            CurrencyCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.BottomBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.LeftBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.RightBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.TopBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS1 = hssfworkbook.CreateCellStyle();
            ReportCS1.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS1.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS1.SetFont(HeaderF1);

            HSSFCellStyle ReportCS2 = hssfworkbook.CreateCellStyle();
            ReportCS2.SetFont(HeaderF1);

            HSSFCellStyle SummaryWithoutBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithoutBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithoutBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithoutBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWeightCS = hssfworkbook.CreateCellStyle();
            SummaryWeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWeightCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithBorderBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.WrapText = true;
            SummaryWithBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithBorderForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.WrapText = true;
            SummaryWithBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Сводный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 16 * 256);
            sheet1.SetColumnWidth(1, 12 * 256);
            sheet1.SetColumnWidth(2, 71 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 17 * 256);
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 10 * 256);

            HSSFCell Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Клиент:");
            Cell1.CellStyle = ReportCS2;

            Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
            Cell1.SetCellValue(ClientName);
            Cell1.CellStyle = ReportCS2;

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("№ заказов:");
            Cell1.CellStyle = ReportCS1;

            Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
            Cell1.SetCellValue(string.Join(", ", OrderNumbers));
            Cell1.CellStyle = ReportCS1;

            int RowCount = 1;


            Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("№ подзаказов: ");
            Cell1.CellStyle = SummaryWithoutBorderBelCS;

            for (int i = 0; i < MainOrdersIDs.Count(); i++)
            {
                MainOrdersList += MainOrdersIDs[i].ToString() + ", ";

                if (i > 0 && i % 9 == 0)
                {
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                    Cell1.SetCellValue(MainOrdersList);
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;

                    MainOrdersList = string.Empty;
                    RowCount++;
                }
                if (i == MainOrdersIDs.Count() - 1)
                {
                    if (MainOrdersList.Length > 0)
                        MainOrdersList = MainOrdersList.Remove(MainOrdersList.Length - 2, 2);
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                    Cell1.SetCellValue(MainOrdersList);
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;

                    MainOrdersList = string.Empty;
                    RowCount++;
                }
            }

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = ReportCS1;
            }

            if (ProfilReportTable.Rows.Count > 0)
            {
                //Профиль
                pos += 2;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-Профиль:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("УНН");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue("Инв.№");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                Cell1.SetCellValue("Вес, кг. ");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                Cell1.SetCellValue("Кол-во уп. ");
                Cell1.CellStyle = SimpleHeaderCS;
                pos++;

                for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["UNN"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["InvNumber"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["AccountingName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    if (ProfilReportTable.Rows[i]["PackageCount"] != DBNull.Value)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PackageCount"]));
                        Cell1.CellStyle = SimpleCS;
                    }
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }

            if (TPSReportTable.Rows.Count > 0)
            {
                //ТПС
                pos++;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-ТПС:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("УНН");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue("Инв.№");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                Cell1.SetCellValue("Вес, кг. ");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                Cell1.SetCellValue("Кол-во уп. ");
                Cell1.CellStyle = SimpleHeaderCS;
                pos++;

                for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["UNN"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["InvNumber"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["AccountingName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    if (TPSReportTable.Rows[i]["PackageCount"] != DBNull.Value)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PackageCount"]));
                        Cell1.CellStyle = SimpleCS;
                    }
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }
            pos++;

            //if (PaymentRate != 1)
            //{
            //    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            //    Cell1.SetCellValue("Курс, 1 EUR = " + PaymentRate + " " + Currency);
            //    Cell1.CellStyle = SummaryWithoutBorderBelCS;
            //}

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                TotalCost = 0;
            }

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }
            #endregion
        }

        public void CreateReport(ref HSSFWorkbook hssfworkbook, int[] NewMegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, decimal TotalWeight, ref decimal TotalProfil1, ref decimal TotalTPS1)
        {
            ClearReport();
            DataTable ReturnDT = GetMegaOrdersTable(NewMegaOrders);
            string MainOrdersList = string.Empty;

            int pos = 0;

            string Currency = string.Empty;

            DataRow[] Row = CurrencyTypesDataTable.Select("CurrencyTypeID = " + CurrencyTypeID);

            Currency = Row[0]["CurrencyType"].ToString();
            //if (ClientID == 145 || ClientID == 258 || ClientID == 267)
            if (ClientID == 145)
            {
                VAT = 1.2m;
            }
            else
            {
                VAT = 1.0m;
            }
            TransportCost = TransportCost / VAT;
            AdditionalCost = AdditionalCost / VAT;
            FrontsReport.Report(MainOrdersIDs);
            DecorReport.Report(MainOrdersIDs);

            //PROFIL
            if (FrontsReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(FrontsReport.ProfilReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.ProfilReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.ProfilReportDataTable.Rows.Count; i++)
                {
                    ProfilReportTable.ImportRow(DecorReport.ProfilReportDataTable.Rows[i]);
                }
            }

            //TPS
            if (FrontsReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < FrontsReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(FrontsReport.TPSReportDataTable.Rows[i]);
                }
            }

            if (DecorReport.TPSReportDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < DecorReport.TPSReportDataTable.Rows.Count; i++)
                {
                    TPSReportTable.ImportRow(DecorReport.TPSReportDataTable.Rows[i]);
                }
            }

            decimal Total = TransportCost + AdditionalCost - ComplaintProfilCost - ComplaintTPSCost;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;
            decimal WeightProfil = 0;
            decimal WeightTPS = 0;

            decimal TransportAndOtherProfil = 0;
            decimal TransportAndOtherTPS = 0;

            DataTable table1 = ProfilReportTable.Copy();
            DataTable table2 = TPSReportTable.Copy();
            ProfilReportTable.Clear();
            TPSReportTable.Clear();
            Collect(table1, table2, false);
            Collect(table1, table2, true);
            using (DataView DV = new DataView(ProfilReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                ProfilReportTable.Clear();
                ProfilReportTable = DV.ToTable();
            }
            using (DataView DV = new DataView(TPSReportTable.Copy(), string.Empty, "AccountingName", DataViewRowState.CurrentRows))
            {
                TPSReportTable.Clear();
                TPSReportTable = DV.ToTable();
            }

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                WeightProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                WeightTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
            }

            TotalCost = (TotalProfil + TotalTPS);

            //Assign COST
            AssignCost(ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost, WeightProfil, WeightTPS, TotalWeight, ref TransportAndOtherProfil,
                       ref TransportAndOtherTPS);

            TotalProfil1 = (TotalProfil);
            TotalTPS1 = (TotalTPS);
            //decimal dd = Decimal.Round((WeightProfil + WeightTPS) / TotalWeight, 3, MidpointRounding.AwayFromZero);
            decimal dd = 0;
            if (TotalWeight != 0)
                dd = Decimal.Round((WeightProfil + WeightTPS) / TotalWeight, 3, MidpointRounding.AwayFromZero);
            TotalProfil = Decimal.Round(TotalProfil, 3, MidpointRounding.AwayFromZero);
            TotalTPS = Decimal.Round(TotalTPS, 3, MidpointRounding.AwayFromZero);
            TransportCost = Decimal.Round(TransportCost * dd, 3, MidpointRounding.AwayFromZero);
            AdditionalCost = Decimal.Round(AdditionalCost * dd, 3, MidpointRounding.AwayFromZero);
            TotalCost = Decimal.Round(TotalCost, 3, MidpointRounding.AwayFromZero);

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                ProfilReportTable.Rows[i]["PackageCount"] = 0;
            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                TPSReportTable.Rows[i]["PackageCount"] = 0;

            DataTable dtGroup = new DataTable();
            DataTable dtMainOrders = new DataTable();
            DataTable DT = null;
            DataTable TempDT = new DataTable();
            DataTable dtDistInvNumbers = new DataTable();
            DataTable DistPackagesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.PackNumber, infiniu2_catalog.dbo.FrontsConfig.InvNumber, infiniu2_catalog.dbo.FrontsConfig.FactoryID, NewFrontsOrders.MainOrderID
                FROM PackageDetails INNER JOIN
                NewFrontsOrders ON PackageDetails.OrderID = NewFrontsOrders.FrontsOrdersID INNER JOIN
                infiniu2_catalog.dbo.FrontsConfig ON NewFrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID
                WHERE PackageDetails.PackageID IN 
                (SELECT PackageID FROM Packages WHERE ProductType = 0 AND MainOrderID IN (" + string.Join(",", MainOrdersIDs) + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            if (DT == null)
                DT = TempDT.Clone();
            foreach (DataRow item in TempDT.Rows)
                DT.Rows.Add(item.ItemArray);
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.PackNumber, infiniu2_catalog.dbo.DecorConfig.InvNumber, infiniu2_catalog.dbo.DecorConfig.FactoryID, NewDecorOrders.MainOrderID
                FROM PackageDetails INNER JOIN
                NewDecorOrders ON PackageDetails.OrderID = NewDecorOrders.DecorOrderID INNER JOIN
                infiniu2_catalog.dbo.DecorConfig ON NewDecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                WHERE PackageDetails.PackageID IN 
                (SELECT PackageID FROM Packages WHERE ProductType = 1 AND MainOrderID IN (" + string.Join(",", MainOrdersIDs) + "))",
                 ConnectionStrings.MarketingOrdersConnectionString))
            {
                TempDT.Clear();
                DA.Fill(TempDT);
            }
            if (DT == null)
                DT = TempDT.Clone();
            foreach (DataRow item in TempDT.Rows)
                DT.Rows.Add(item.ItemArray);

            for (int z = 0; z < MainOrdersIDs.Count(); z++)
            {
                using (DataView DV = new DataView(DT, "MainOrderID=" + MainOrdersIDs[z], string.Empty, DataViewRowState.CurrentRows))
                {
                    dtMainOrders.Clear();
                    dtMainOrders = DV.ToTable();
                }
                using (DataView DV = new DataView(dtMainOrders, "FactoryID=1", string.Empty, DataViewRowState.CurrentRows))
                {
                    DistPackagesDT.Clear();
                    DistPackagesDT = DV.ToTable(true, "PackNumber");
                }
                dtGroup.Clear();
                dtGroup = GroupBy("InvNumber", "PackNumber", dtMainOrders);
                for (int i = 0; i < DistPackagesDT.Rows.Count; i++)
                {
                    using (DataView DV = new DataView(dtMainOrders, "PackNumber=" + Convert.ToInt32(DistPackagesDT.Rows[i]["PackNumber"]), string.Empty, DataViewRowState.CurrentRows))
                    {
                        dtDistInvNumbers.Clear();
                        dtDistInvNumbers = DV.ToTable(true, "InvNumber");
                        string InvNumber = dtDistInvNumbers.Rows[0]["InvNumber"].ToString();
                        int PackageCount = 0;

                        DataRow[] rows0 = dtGroup.Select("InvNumber='" + InvNumber + "'");
                        if (rows0.Count() > 0)
                            PackageCount = Convert.ToInt32(rows0[0]["Count"]);

                        for (int x = 1; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = dtGroup.Select("InvNumber='" + dtDistInvNumbers.Rows[x]["InvNumber"].ToString() + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["Count"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["Count"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        PackageCount = 0;
                        for (int x = 0; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = ProfilReportTable.Select("InvNumber='" + InvNumber + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["PackageCount"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["PackageCount"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        DataRow[] rows = ProfilReportTable.Select("InvNumber='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            rows[0]["PackageCount"] = Convert.ToInt32(rows[0]["PackageCount"]) + 1;
                    }
                }



                using (DataView DV = new DataView(dtMainOrders, "FactoryID=2", string.Empty, DataViewRowState.CurrentRows))
                {
                    DistPackagesDT.Clear();
                    DistPackagesDT = DV.ToTable(true, "PackNumber");
                }
                dtGroup.Clear();
                dtGroup = GroupBy("InvNumber", "PackNumber", dtMainOrders);
                for (int i = 0; i < DistPackagesDT.Rows.Count; i++)
                {
                    using (DataView DV = new DataView(dtMainOrders, "PackNumber=" + Convert.ToInt32(DistPackagesDT.Rows[i]["PackNumber"]), string.Empty, DataViewRowState.CurrentRows))
                    {
                        dtDistInvNumbers.Clear();
                        dtDistInvNumbers = DV.ToTable(true, "InvNumber");
                        string InvNumber = dtDistInvNumbers.Rows[0]["InvNumber"].ToString();
                        int PackageCount = 0;

                        DataRow[] rows0 = dtGroup.Select("InvNumber='" + InvNumber + "'");
                        if (rows0.Count() > 0)
                            PackageCount = Convert.ToInt32(rows0[0]["Count"]);

                        for (int x = 1; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = dtGroup.Select("InvNumber='" + dtDistInvNumbers.Rows[x]["InvNumber"].ToString() + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["Count"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["Count"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        PackageCount = 0;
                        for (int x = 0; x < dtDistInvNumbers.Rows.Count; x++)
                        {
                            DataRow[] rows1 = TPSReportTable.Select("InvNumber='" + InvNumber + "'");
                            if (rows1.Count() > 0)
                            {
                                if (Convert.ToInt32(rows1[0]["PackageCount"]) < PackageCount)
                                {
                                    PackageCount = Convert.ToInt32(rows1[0]["PackageCount"]);
                                    InvNumber = dtDistInvNumbers.Rows[x]["InvNumber"].ToString();
                                }
                            }
                        }
                        DataRow[] rows = TPSReportTable.Select("InvNumber='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            rows[0]["PackageCount"] = Convert.ToInt32(rows[0]["PackageCount"]) + 1;
                    }
                }
            }

            #region 

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 11;
            HeaderF1.Boldweight = 11 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 10;
            HeaderF2.Boldweight = 10 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 8;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.000");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            HSSFCellStyle WeightCS = hssfworkbook.CreateCellStyle();
            WeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            WeightCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            WeightCS.BottomBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            WeightCS.LeftBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            WeightCS.RightBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            WeightCS.TopBorderColor = HSSFColor.BLACK.index;
            WeightCS.SetFont(SimpleF);

            HSSFCellStyle PriceBelCS = hssfworkbook.CreateCellStyle();
            PriceBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            PriceBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceBelCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceBelCS.SetFont(SimpleF);

            HSSFCellStyle PriceForeignCS = hssfworkbook.CreateCellStyle();
            PriceForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            PriceForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PriceForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            PriceForeignCS.SetFont(SimpleF);

            HSSFCellStyle CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.0000");
            CurrencyCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.BottomBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.LeftBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.RightBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CurrencyCS.TopBorderColor = HSSFColor.BLACK.index;
            CurrencyCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS1 = hssfworkbook.CreateCellStyle();
            ReportCS1.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS1.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS1.SetFont(HeaderF1);

            HSSFCellStyle ReportCS2 = hssfworkbook.CreateCellStyle();
            ReportCS2.SetFont(HeaderF1);

            HSSFCellStyle SummaryWithoutBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithoutBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithoutBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithoutBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithoutBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWeightCS = hssfworkbook.CreateCellStyle();
            SummaryWeightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWeightCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderBelCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderBelCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            SummaryWithBorderBelCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderBelCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderBelCS.WrapText = true;
            SummaryWithBorderBelCS.SetFont(HeaderF2);

            HSSFCellStyle SummaryWithBorderForeignCS = hssfworkbook.CreateCellStyle();
            SummaryWithBorderForeignCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            SummaryWithBorderForeignCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.BottomBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.LeftBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.RightBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SummaryWithBorderForeignCS.TopBorderColor = HSSFColor.BLACK.index;
            SummaryWithBorderForeignCS.WrapText = true;
            SummaryWithBorderForeignCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Сводный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 16 * 256);
            sheet1.SetColumnWidth(1, 12 * 256);
            sheet1.SetColumnWidth(2, 71 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 17 * 256);
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 10 * 256);

            HSSFCell Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Клиент:");
            Cell1.CellStyle = ReportCS2;

            Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
            Cell1.SetCellValue(ClientName);
            Cell1.CellStyle = ReportCS2;

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("№ заказов:");
            Cell1.CellStyle = ReportCS1;

            Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
            Cell1.SetCellValue(string.Join(", ", OrderNumbers));
            Cell1.CellStyle = ReportCS1;

            int RowCount = 1;


            Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("№ подзаказов: ");
            Cell1.CellStyle = SummaryWithoutBorderBelCS;

            for (int i = 0; i < MainOrdersIDs.Count(); i++)
            {
                MainOrdersList += MainOrdersIDs[i].ToString() + ", ";

                if (i > 0 && i % 9 == 0)
                {
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                    Cell1.SetCellValue(MainOrdersList);
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;

                    MainOrdersList = string.Empty;
                    RowCount++;
                }
                if (i == MainOrdersIDs.Count() - 1)
                {
                    if (MainOrdersList.Length > 0)
                        MainOrdersList = MainOrdersList.Remove(MainOrdersList.Length - 2, 2);
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                    Cell1.SetCellValue(MainOrdersList);
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;

                    MainOrdersList = string.Empty;
                    RowCount++;
                }
            }

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("РЕКЛАМАЦИЯ");
                Cell1.CellStyle = ReportCS1;
            }

            if (ProfilReportTable.Rows.Count > 0)
            {
                //Профиль
                pos += 2;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-Профиль:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("УНН");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue("Инв.№");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                Cell1.SetCellValue("Вес, кг. ");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                Cell1.SetCellValue("Кол-во уп. ");
                Cell1.CellStyle = SimpleHeaderCS;
                pos++;

                for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["UNN"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["InvNumber"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["AccountingName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    if (ProfilReportTable.Rows[i]["PackageCount"] != DBNull.Value)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["PackageCount"]));
                        Cell1.CellStyle = SimpleCS;
                    }
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }

            if (TPSReportTable.Rows.Count > 0)
            {
                //ТПС
                pos++;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-ТПС:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("УНН");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue("Инв.№");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                Cell1.SetCellValue("Цена с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                Cell1.SetCellValue("Стоимость с транс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                Cell1.SetCellValue("Курс, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                Cell1.SetCellValue("Вес, кг. ");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                Cell1.SetCellValue("Кол-во уп. ");
                Cell1.CellStyle = SimpleHeaderCS;
                pos++;

                for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["UNN"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["InvNumber"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(2);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["AccountingName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(3);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    if (CurrencyTypeID == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(4);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PriceWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                        if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["CostWithTransport"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(7);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PaymentRate"]));
                        Cell1.CellStyle = CurrencyCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    if (TPSReportTable.Rows[i]["PackageCount"] != DBNull.Value)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(9);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["PackageCount"]));
                        Cell1.CellStyle = SimpleCS;
                    }
                    pos++;
                }

                if (CurrencyTypeID == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Транспорт, прочее, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos++).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(TransportAndOtherTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos - 2).CreateCell(8);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }
            pos++;

            //if (PaymentRate != 1)
            //{
            //    Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            //    Cell1.SetCellValue("Курс, 1 EUR = " + PaymentRate + " " + Currency);
            //    Cell1.CellStyle = SummaryWithoutBorderBelCS;
            //}

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Транспорт, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(TransportCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Прочее, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble(AdditionalCost));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }

            if (IsComplaint(GetMegaOrderID(MainOrdersIDs[0])))
            {
                TotalCost = 0;
            }

            if (CurrencyTypeID == 0)
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderBelCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderBelCS;
            }
            else
            {
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Итого, " + Currency + ":");
                Cell1.CellStyle = SummaryWithBorderForeignCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
                Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
                Cell1.CellStyle = SummaryWithBorderForeignCS;
            }
            #endregion
        }

        public void SaveDBF(string FilePath, string DBFName, bool IsSample, ref string ProfilDBFName, ref string TPSDBFName)
        {
            string CurrentMonthName = DateTime.Now.ToString("dd-MM-yyyy");
            if (Directory.Exists(FilePath) == false)//not exists
            {
                Directory.CreateDirectory(FilePath);
            }
            if (ProfilReportTable.Rows.Count > 0 && TPSReportTable.Rows.Count > 0)
            {
                ProfilDBFName = DBFName + " (Профиль-ТПС)";
                if (IsSample)
                    ProfilDBFName = DBFName + " (Профиль-ТПС), обр";
                FileInfo f = new FileInfo(FilePath + @"\" + ProfilDBFName + ".DBF");
                int x = 1;
                while (f.Exists == true)
                    f = new FileInfo(FilePath + @"\" + ProfilDBFName + "(" + x++ + ").DBF");
                ProfilDBFName = f.FullName;
                TPSDBFName = f.FullName;

                DataSetIntoDBF(FilePath, @"PT", ProfilReportTable, TPSReportTable, 0);
                var sourcePath = Path.Combine(FilePath, "PT.DBF");
                var destinationPath = ProfilDBFName;
                File.Move(sourcePath, destinationPath);
            }
            if (ProfilReportTable.Rows.Count > 0 && TPSReportTable.Rows.Count == 0)
            {
                ProfilDBFName = DBFName + " (ЗОВ-Профиль)";
                if (IsSample)
                    ProfilDBFName = DBFName + " (ЗОВ-Профиль), обр";
                FileInfo f = new FileInfo(FilePath + @"\" + ProfilDBFName + ".DBF");
                int x = 1;
                while (f.Exists == true)
                    f = new FileInfo(FilePath + @"\" + ProfilDBFName + "(" + x++ + ").DBF");
                ProfilDBFName = f.FullName;

                DataSetIntoDBF(FilePath, @"Profil", ProfilReportTable, 1);
                var sourcePath = Path.Combine(FilePath, "Profil.DBF");
                var destinationPath = ProfilDBFName;
                File.Move(sourcePath, destinationPath);
            }
            if (TPSReportTable.Rows.Count > 0 && ProfilReportTable.Rows.Count == 0)
            {
                TPSDBFName = DBFName + " (ЗОВ-ТПС)";
                if (IsSample)
                    TPSDBFName = DBFName + " (ЗОВ-ТПС), обр";
                FileInfo f = new FileInfo(FilePath + @"\" + TPSDBFName + ".DBF");
                int x = 1;
                while (f.Exists == true)
                    f = new FileInfo(FilePath + @"\" + TPSDBFName + "(" + x++ + ").DBF");
                TPSDBFName = f.FullName;

                DataSetIntoDBF(FilePath, @"TPS", TPSReportTable, 2);
                var sourcePath = Path.Combine(FilePath, "TPS.DBF");
                var destinationPath = TPSDBFName;
                File.Move(sourcePath, destinationPath);
            }
        }

        public void SaveDBF(string FilePath, string DBFName, ref string ProfilDBFName, ref string TPSDBFName)
        {
            string CurrentMonthName = DateTime.Now.ToString("dd-MM-yyyy");
            if (Directory.Exists(FilePath) == false)//not exists
            {
                Directory.CreateDirectory(FilePath);
            }
            if (ProfilReportTable.Rows.Count > 0 && TPSReportTable.Rows.Count > 0)
            {
                ProfilDBFName = DBFName + " (Профиль-ТПС)";
                FileInfo f = new FileInfo(FilePath + @"\" + ProfilDBFName + ".DBF");
                int x = 1;
                while (f.Exists == true)
                    f = new FileInfo(FilePath + @"\" + ProfilDBFName + "(" + x++ + ").DBF");
                ProfilDBFName = f.FullName;
                TPSDBFName = f.FullName;

                DataSetIntoDBF(FilePath, @"PT", ProfilReportTable, TPSReportTable, 0);
                var sourcePath = Path.Combine(FilePath, "PT.DBF");
                var destinationPath = ProfilDBFName;
                File.Move(sourcePath, destinationPath);
            }
            if (ProfilReportTable.Rows.Count > 0 && TPSReportTable.Rows.Count == 0)
            {
                ProfilDBFName = DBFName + " (ЗОВ-Профиль)";
                FileInfo f = new FileInfo(FilePath + @"\" + ProfilDBFName + ".DBF");
                int x = 1;
                while (f.Exists == true)
                    f = new FileInfo(FilePath + @"\" + ProfilDBFName + "(" + x++ + ").DBF");
                ProfilDBFName = f.FullName;

                DataSetIntoDBF(FilePath, @"Profil", ProfilReportTable, 1);
                var sourcePath = Path.Combine(FilePath, "Profil.DBF");
                var destinationPath = ProfilDBFName;
                File.Move(sourcePath, destinationPath);
            }
            if (TPSReportTable.Rows.Count > 0 && ProfilReportTable.Rows.Count == 0)
            {
                TPSDBFName = DBFName + " (ЗОВ-ТПС)";
                FileInfo f = new FileInfo(FilePath + @"\" + TPSDBFName + ".DBF");
                int x = 1;
                while (f.Exists == true)
                    f = new FileInfo(FilePath + @"\" + TPSDBFName + "(" + x++ + ").DBF");
                TPSDBFName = f.FullName;

                DataSetIntoDBF(FilePath, @"TPS", TPSReportTable, 2);
                var sourcePath = Path.Combine(FilePath, "TPS.DBF");
                var destinationPath = TPSDBFName;
                File.Move(sourcePath, destinationPath);
            }
        }

        public void ClearReport()
        {
            ProfilReportTable.Clear();
            TPSReportTable.Clear();
            FrontsReport.ClearReport();
            DecorReport.ClearReport();
        }

        public static void DataSetIntoDBF(string path, string fileName, DataTable DT1, int FactoryID)
        {
            if (File.Exists(path + fileName + ".dbf"))
            {
                File.Delete(path + fileName + ".dbf");
            }

            string createSql = "create table " + fileName + " ([UNNP] varchar(20), [UNN] varchar(20), [CurrencyCode] varchar(20), [InvNumber] varchar(20), [Amount] Double, [Price] Double, [NDS] varchar(10), [Weight] Double, [PackageCount] Integer)";

            OleDbConnection con = new OleDbConnection(GetConnection(path));

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con
            };
            con.Open();

            cmd.CommandText = createSql;

            cmd.ExecuteNonQuery();

            foreach (DataRow row in DT1.Rows)
            {
                string insertSql = "insert into " + fileName +
                    " (UNNP, UNN, CurrencyCode, InvNumber, Amount, Price, NDS, Weight, PackageCount) values(UNNP, UNN, CurrencyCode, InvNumber, Amount, PriceWithTransport, NDS, Weight, PackageCount)";
                decimal d = Decimal.Round(Convert.ToDecimal(row["Weight"]) / 1000, 3, MidpointRounding.AwayFromZero);
                double Amount = Convert.ToDouble(row["Count"]);
                double Price = Convert.ToDouble(row["PriceWithTransport"]);
                double Weight = Convert.ToDouble(d);
                int PackageCount = 0;
                if (row["PackageCount"] != DBNull.Value)
                    PackageCount = Convert.ToInt32(row["PackageCount"]);
                string InvNumber = row["InvNumber"].ToString();
                string CurrencyCode = row["CurrencyCode"].ToString();
                //string TPSCurCode = row["TPSCurCode"].ToString();
                string UNN = row["UNN"].ToString();
                string NDS = "0 %";
                string UNNP = "800014979";
                cmd.CommandText = insertSql;
                cmd.Parameters.Clear();

                if (Convert.ToInt32(CurrencyCode) == 974 || Convert.ToInt32(CurrencyCode) == 933)
                    NDS = "20 %";
                if (FactoryID == 2)
                    UNNP = "590618616";
                cmd.Parameters.Add("UNNP", OleDbType.VarChar).Value = UNNP;
                cmd.Parameters.Add("UNN", OleDbType.VarChar).Value = UNN;
                cmd.Parameters.Add("CurrencyCode", OleDbType.VarChar).Value = CurrencyCode;
                //cmd.Parameters.Add("TPSCurCode", OleDbType.VarChar).Value = TPSCurCode;
                cmd.Parameters.Add("InvNumber", OleDbType.VarChar).Value = InvNumber;
                cmd.Parameters.Add("Amount", OleDbType.Double).Value = Amount;
                cmd.Parameters.Add("PriceWithTransport", OleDbType.Double).Value = Price;
                cmd.Parameters.Add("NDS", OleDbType.VarChar).Value = NDS;
                cmd.Parameters.Add("Weight", OleDbType.Double).Value = Weight;
                cmd.Parameters.Add("PackageCount", OleDbType.Integer).Value = PackageCount;
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }

        public static void DataSetIntoDBF(string path, string fileName, DataTable DT1, DataTable DT2, int FactoryID)
        {
            if (File.Exists(path + fileName + ".dbf"))
            {
                File.Delete(path + fileName + ".dbf");
            }

            string createSql = "create table " + fileName + " ([UNNP] varchar(20), [UNN] varchar(20), [CurrencyCode] varchar(20), [InvNumber] varchar(20), [Amount] Double, [Price] Double, [NDS] varchar(10), [Weight] Double, [PackageCount] Integer)";

            OleDbConnection con = new OleDbConnection(GetConnection(path));

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con
            };
            con.Open();

            cmd.CommandText = createSql;

            cmd.ExecuteNonQuery();

            foreach (DataRow row in DT1.Rows)
            {
                string insertSql = "insert into " + fileName +
                    " (UNNP, UNN, CurrencyCode, InvNumber, Amount, Price, NDS, Weight, PackageCount) values(UNNP, UNN, CurrencyCode, InvNumber, Amount, PriceWithTransport, NDS, Weight, PackageCount)";
                decimal d = Decimal.Round(Convert.ToDecimal(row["Weight"]) / 1000, 3, MidpointRounding.AwayFromZero);
                double Amount = Convert.ToDouble(row["Count"]);
                double Price = Convert.ToDouble(row["PriceWithTransport"]);
                double Weight = Convert.ToDouble(d);
                int PackageCount = 0;
                if (row["PackageCount"] != DBNull.Value)
                    PackageCount = Convert.ToInt32(row["PackageCount"]);
                string InvNumber = row["InvNumber"].ToString();
                string CurrencyCode = row["CurrencyCode"].ToString();
                //string TPSCurCode = row["TPSCurCode"].ToString();
                string UNN = row["UNN"].ToString();
                string NDS = "0 %";
                string UNNP = "800014979";
                cmd.CommandText = insertSql;
                cmd.Parameters.Clear();

                if (Convert.ToInt32(CurrencyCode) == 974 || Convert.ToInt32(CurrencyCode) == 933)
                    NDS = "20 %";
                //if (FactoryID == 2)
                //    UNNP = "590618616";
                cmd.Parameters.Add("UNNP", OleDbType.VarChar).Value = UNNP;
                cmd.Parameters.Add("UNN", OleDbType.VarChar).Value = UNN;
                cmd.Parameters.Add("CurrencyCode", OleDbType.VarChar).Value = CurrencyCode;
                //cmd.Parameters.Add("TPSCurCode", OleDbType.VarChar).Value = TPSCurCode;
                cmd.Parameters.Add("InvNumber", OleDbType.VarChar).Value = InvNumber;
                cmd.Parameters.Add("Amount", OleDbType.Double).Value = Amount;
                cmd.Parameters.Add("PriceWithTransport", OleDbType.Double).Value = Price;
                cmd.Parameters.Add("NDS", OleDbType.VarChar).Value = NDS;
                cmd.Parameters.Add("Weight", OleDbType.Double).Value = Weight;
                cmd.Parameters.Add("PackageCount", OleDbType.Integer).Value = PackageCount;
                cmd.ExecuteNonQuery();
            }
            foreach (DataRow row in DT2.Rows)
            {
                string insertSql = "insert into " + fileName +
                    " (UNNP, UNN, CurrencyCode, InvNumber, Amount, Price, NDS, Weight, PackageCount) values(UNNP, UNN, CurrencyCode, InvNumber, Amount, PriceWithTransport, NDS, Weight, PackageCount)";
                decimal d = Decimal.Round(Convert.ToDecimal(row["Weight"]) / 1000, 3, MidpointRounding.AwayFromZero);
                double Amount = Convert.ToDouble(row["Count"]);
                double Price = Convert.ToDouble(row["PriceWithTransport"]);
                double Weight = Convert.ToDouble(d);
                int PackageCount = 0;
                if (row["PackageCount"] != DBNull.Value)
                    PackageCount = Convert.ToInt32(row["PackageCount"]);
                string InvNumber = row["InvNumber"].ToString();
                string CurrencyCode = row["CurrencyCode"].ToString();
                //string TPSCurCode = row["TPSCurCode"].ToString();
                string UNN = row["UNN"].ToString();
                string NDS = "0 %";
                string UNNP = "590618616";
                cmd.CommandText = insertSql;
                cmd.Parameters.Clear();

                if (Convert.ToInt32(CurrencyCode) == 974 || Convert.ToInt32(CurrencyCode) == 933)
                    NDS = "20 %";
                //if (FactoryID == 2)
                //    UNNP = "590618616";
                cmd.Parameters.Add("UNNP", OleDbType.VarChar).Value = UNNP;
                cmd.Parameters.Add("UNN", OleDbType.VarChar).Value = UNN;
                cmd.Parameters.Add("CurrencyCode", OleDbType.VarChar).Value = CurrencyCode;
                //cmd.Parameters.Add("TPSCurCode", OleDbType.VarChar).Value = TPSCurCode;
                cmd.Parameters.Add("InvNumber", OleDbType.VarChar).Value = InvNumber;
                cmd.Parameters.Add("Amount", OleDbType.Double).Value = Amount;
                cmd.Parameters.Add("PriceWithTransport", OleDbType.Double).Value = Price;
                cmd.Parameters.Add("NDS", OleDbType.VarChar).Value = NDS;
                cmd.Parameters.Add("Weight", OleDbType.Double).Value = Weight;
                cmd.Parameters.Add("PackageCount", OleDbType.Integer).Value = PackageCount;
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }

        private static string GetConnection(string path)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=dBASE IV;";
        }

        public static string ReplaceEscape(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }
    }

}
