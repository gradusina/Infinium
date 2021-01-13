using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;

namespace Infinium.Modules.ZOV.ReportToDBF
{
    public class InvoiceFrontsReportToDBF
    {
        FrontsCalculate FC = null;

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

        public InvoiceFrontsReportToDBF(ref FrontsCalculate tFC)
        {
            FC = tFC;

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
            DecorConfigDataTable = TablesManager.DecorConfigDataTable;

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
            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("OriginalPrice", Type.GetType("System.Decimal")));
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
                decimal SolidWeight = 0;

                decimal FilenkaCount = 0;
                decimal FilenkaCost = 0;
                decimal FilenkaWeight = 0;

                decimal VitrinaCount = 0;
                decimal VitrinaCost = 0;
                decimal VitrinaWeight = 0;

                decimal LuxMegaCount = 0;
                decimal LuxMegaCost = 0;
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
                            DeductibleCount = GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]),
                                                        Convert.ToInt32(Rows[r]["Width"])) * Convert.ToDecimal(Rows[r]["Count"]);
                            DeductibleWeight = Decimal.Round(DeductibleCount * DeductibleWeight, 3, MidpointRounding.AwayFromZero);
                        }
                    }
                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        SolidCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                      Convert.ToDecimal(Rows[r]["Count"]);
                    else
                        SolidCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        SolidCost += Convert.ToDecimal(Rows[r]["Cost"]);
                    else
                        SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

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

                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                            SolidCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                          Convert.ToDecimal(Rows[r]["Count"]);
                        else
                            SolidCount += Convert.ToDecimal(Rows[r]["Square"]);

                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                            SolidCost += Convert.ToDecimal(Rows[r]["Cost"]) + 5;
                        else
                            SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) + 5 * Convert.ToDecimal(Rows[r]["Count"]);

                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        SolidWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    }
                    else if (Convert.ToInt32(Rows[r]["FrontID"]) == 3415 || Convert.ToInt32(Rows[r]["FrontID"]) == 28922)
                    {
                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                            FilenkaCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                          Convert.ToDecimal(Rows[r]["Count"]);
                        else
                            FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                            FilenkaCost += Convert.ToDecimal(Rows[r]["Cost"]) + 5;
                        else
                            FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) + 5 * Convert.ToDecimal(Rows[r]["Count"]);

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
                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        FilenkaCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                      Convert.ToDecimal(Rows[r]["Count"]);
                    else
                        FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        FilenkaCost += Convert.ToDecimal(Rows[r]["Cost"]);
                    else
                        FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

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
                            DeductibleCount = GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]),
                                                        Convert.ToInt32(Rows[r]["Width"])) * Convert.ToDecimal(Rows[r]["Count"]);
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
                            DeductibleWeight = Decimal.Round(DeductibleCount * 10, 3, MidpointRounding.AwayFromZero);
                        }
                    }

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        VitrinaCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                      Convert.ToDecimal(Rows[r]["Count"]);
                    else
                        VitrinaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (IsAluminium(Rows[r]) > -1)
                    {
                        VitrinaCost += GetFrontCostAluminium(Rows[r]);
                        VitrinaWeight += GetAluminiumWeight(Rows[r], true);
                    }
                    else
                    {
                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                            VitrinaCost += Convert.ToDecimal(Rows[r]["Cost"]);
                        else
                            VitrinaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        VitrinaWeight += Convert.ToDecimal(FrontWeight + InsetWeight - DeductibleWeight);
                    }
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ЛЮКС, МЕГА
                filter = " AND InsetTypeID IN (860,862,4310)";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        LuxMegaCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                      Convert.ToDecimal(Rows[r]["Count"]);
                    else
                        LuxMegaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        LuxMegaCost += Convert.ToDecimal(Rows[r]["Cost"]);
                    else
                        LuxMegaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    LuxMegaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }

                if (!IsNonStandard)
                    NonStandardMargin = DBNull.Value;
                if (SolidCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["OriginalPrice"] = SolidCost / SolidCount;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Decimal.Round(SolidCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(SolidCost, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(SolidWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (FilenkaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["OriginalPrice"] = FilenkaCost / FilenkaCount;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Decimal.Round(FilenkaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(FilenkaCost, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(FilenkaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (VitrinaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["OriginalPrice"] = VitrinaCost / VitrinaCount;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Decimal.Round(VitrinaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(VitrinaCost, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(VitrinaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (LuxMegaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["OriginalPrice"] = LuxMegaCost / LuxMegaCount;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Decimal.Round(LuxMegaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(LuxMegaCost, 3, MidpointRounding.AwayFromZero);
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
                decimal Solid713Weight = 0;

                decimal Filenka713Count = 0;
                decimal Filenka713Price = 0;
                decimal Filenka713Weight = 0;

                decimal NoInset713Count = 0;
                decimal NoInset713Price = 0;
                decimal NoInset713Weight = 0;

                decimal Vitrina713Count = 0;
                decimal Vitrina713Price = 0;
                decimal Vitrina713Weight = 0;

                decimal Solid910Count = 0;
                decimal Solid910Price = 0;
                decimal Solid910Weight = 0;

                decimal Filenka910Count = 0;
                decimal Filenka910Price = 0;
                decimal Filenka910Weight = 0;

                decimal NoInset910Count = 0;
                decimal NoInset910Price = 0;
                decimal NoInset910Weight = 0;

                decimal Vitrina910Count = 0;
                decimal Vitrina910Price = 0;
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

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina713Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);

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

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina910Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            Vitrina910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }
                    }

                }

                if (Solid713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Solid713Price;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Solid713Count;
                    Row["Cost"] = Decimal.Round(Solid713Count * Solid713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Filenka713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Filenka713Price;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Filenka713Count;
                    Row["Cost"] = Decimal.Round(Filenka713Count * Filenka713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (NoInset713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = NoInset713Price;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = NoInset713Count;
                    Row["Cost"] = Decimal.Round(NoInset713Count * NoInset713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Vitrina713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Vitrina713Price;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Vitrina713Count;
                    Row["Cost"] = Decimal.Round(Vitrina713Count * Vitrina713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Solid910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Solid910Price;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Solid910Count;
                    Row["Cost"] = Decimal.Round(Solid910Count * Solid910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Filenka910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Filenka910Price;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Filenka910Count;
                    Row["Cost"] = Decimal.Round(Filenka910Count * Filenka910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (NoInset910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = NoInset910Price;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = NoInset910Count;
                    Row["Cost"] = Decimal.Round(NoInset910Count * NoInset910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Vitrina910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["OriginalPrice"] = Vitrina910Price;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Count"] = Vitrina910Count;
                    Row["Cost"] = Decimal.Round(Vitrina910Count * Vitrina910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }
            }

            Fronts.Dispose();
        }

        private void GetGrids(DataTable OrdersDataTable, DataTable ReportDataTable, DataTable ReportDataTable1, int FactoryID1)
        {
            DataRow[] Rows = OrdersDataTable.Select("InsetTypeID IN (685,686,687,688,29470,29471) AND FrontID <> 3729");

            if (Rows.Count() == 0)
                return;

            decimal CountPP = 0;
            decimal CostPP = 0;

            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);

            for (int i = 0; i < Rows.Count(); i++)
            {
                decimal d = GetInsetSquare(Convert.ToInt32(Rows[i]["FrontID"]), Convert.ToInt32(Rows[i]["Height"]),
                                            Convert.ToInt32(Rows[i]["Width"])) * Convert.ToDecimal(Rows[i]["Count"]);
                CountPP += d;
                CostPP += Convert.ToDecimal(Rows[i]["InsetPrice"]) * d;
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
                    DataRow NewRow = ReportDataTable1.NewRow();
                    NewRow["OriginalPrice"] = CostPP / CountPP;
                    NewRow["AccountingName"] = OrdersDataTable.Rows[0]["AccountingName"].ToString();
                    NewRow["InvNumber"] = OrdersDataTable.Rows[0]["InvNumber"].ToString();
                    NewRow["Count"] = Decimal.Round(CountPP, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(CostPP, 3, MidpointRounding.AwayFromZero);
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
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["OriginalPrice"] = CostPP / CountPP;
                    NewRow["AccountingName"] = OrdersDataTable.Rows[0]["AccountingName"].ToString();
                    NewRow["InvNumber"] = OrdersDataTable.Rows[0]["InvNumber"].ToString();
                    NewRow["Count"] = Decimal.Round(CountPP, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(CostPP, 3, MidpointRounding.AwayFromZero);
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
                if (FC.IsAluminium(OrdersDataTable.Rows[i]) > -1)
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
                    if (FC.IsAluminium(FRows[i]) != -1)
                        continue;

                    CountFlutes += Convert.ToDecimal(FRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(FRows[i]["FrontID"]),
                                              Convert.ToInt32(FRows[i]["Height"]),
                                              Convert.ToInt32(FRows[i]["Width"]));
                }

                CountFlutes = Decimal.Round(CountFlutes, 3, MidpointRounding.AwayFromZero);

                if (CountFlutes > 0)
                {
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
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountFlutes;
                        NewRow["Cost"] = Decimal.Round(CountFlutes * PriceFlutes);
                        NewRow["Weight"] = Decimal.Round(CountFlutes * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло Флутес";
                        NewRow["OriginalPrice"] = PriceFlutes;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountFlutes;
                        NewRow["Cost"] = Decimal.Round(CountFlutes * PriceFlutes);
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
                    if (FC.IsAluminium(LRows[i]) != -1)
                        continue;

                    CountLacomat += Convert.ToDecimal(LRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(LRows[i]["FrontID"]),
                                              Convert.ToInt32(LRows[i]["Height"]),
                                              Convert.ToInt32(LRows[i]["Width"]));

                }

                CountLacomat = Decimal.Round(CountLacomat, 3, MidpointRounding.AwayFromZero);

                if (CountLacomat > 0)
                {
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
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountLacomat;
                        NewRow["Cost"] = Decimal.Round(CountLacomat * PriceLacomat);
                        NewRow["Weight"] = Decimal.Round(CountLacomat * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло Лакомат";
                        NewRow["OriginalPrice"] = PriceLacomat;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountLacomat;
                        NewRow["Cost"] = Decimal.Round(CountLacomat * PriceLacomat);
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
                    if (FC.IsAluminium(KRows[i]) != -1)
                        continue;

                    CountKrizet += Convert.ToDecimal(KRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(KRows[i]["FrontID"]),
                                              Convert.ToInt32(KRows[i]["Height"]),
                                              Convert.ToInt32(KRows[i]["Width"]));
                }

                CountKrizet = Decimal.Round(CountKrizet, 3, MidpointRounding.AwayFromZero);

                if (CountKrizet > 0)
                {
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
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountKrizet;
                        NewRow["Cost"] = Decimal.Round(CountKrizet * PriceKrizet);
                        NewRow["Weight"] = Decimal.Round(CountKrizet * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло Кризет";
                        NewRow["OriginalPrice"] = PriceKrizet;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountKrizet;
                        NewRow["Cost"] = Decimal.Round(CountKrizet * PriceKrizet);
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
                    if (FC.IsAluminium(ORows[i]) != -1)
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
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountOther;
                        NewRow["Cost"] = 0;
                        NewRow["Weight"] = Decimal.Round(CountOther * 10, 3, MidpointRounding.AwayFromZero);
                        ReportDataTable1.Rows.Add(NewRow);
                    }
                    else
                    {
                        DataRow NewRow = ReportDataTable.NewRow();
                        //NewRow["Name"] = "Стекло другое";
                        NewRow["OriginalPrice"] = 0;
                        NewRow["AccountingName"] = AccountingName;
                        NewRow["InvNumber"] = InvNumber;
                        if (rows.Count() > 0)
                        {
                            NewRow["AccountingName"] = rows[0]["DecorAccountingName"].ToString();
                            NewRow["InvNumber"] = rows[0]["DecorInvNumber"].ToString();
                        }
                        NewRow["Count"] = CountOther;
                        NewRow["Cost"] = 0;
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

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["OriginalPrice"] = PriceEllipseGrid;
                NewRow["Count"] = CountEllipseGrid;
                NewRow["Cost"] = CountEllipseGrid * PriceEllipseGrid;
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
            if (FC.IsAluminium(FrontsOrdersRow) > -1)
            {
                outFrontWeight = PackWeight +
                    FC.GetAluminiumWeight(FrontsOrdersRow, true);
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

        public void Report(int[] MainOrderIDs)
        {
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();
            DecorInvNumbersDT.Clear();

            string sWhere = "";

            for (int i = 0; i < MainOrderIDs.Count(); i++)
            {
                if (sWhere != "")
                    sWhere += " OR MainOrderID = " + MainOrderIDs[i].ToString();
                else
                    sWhere += "MainOrderID = " + MainOrderIDs[i].ToString();
            }
            string SelectCommand = "SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber FROM FrontsOrders" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " WHERE InvNumber IS NOT NULL AND FrontsOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);

                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(ProfilFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            GetSimpleFronts(DTs[i], ProfilReportDataTable, false);
                            GetSimpleFronts(DTs[i], ProfilReportDataTable, true);

                            GetCurvedFronts(DTs[i], ProfilReportDataTable);

                            GetGrids(DTs[i], ProfilReportDataTable, TPSReportDataTable, 1);

                            GetInsets(DTs[i], ProfilReportDataTable);

                            GetGlass(DTs[i], ProfilReportDataTable, TPSReportDataTable, 1);
                        }
                    }

                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        DataTable[] DTs = GroupInvNumber(TPSFrontsOrdersDataTable);

                        for (int i = 0; i < DTs.Count(); i++)
                        {
                            GetSimpleFronts(DTs[i], TPSReportDataTable, false);
                            GetSimpleFronts(DTs[i], TPSReportDataTable, true);

                            GetCurvedFronts(DTs[i], TPSReportDataTable);

                            GetGrids(DTs[i], TPSReportDataTable, ProfilReportDataTable, 2);

                            GetInsets(DTs[i], TPSReportDataTable);

                            GetGlass(DTs[i], TPSReportDataTable, ProfilReportDataTable, 2);
                        }
                    }

                }
            }
        }
    }






    public class InvoiceDecorReportToDBF
    {
        DecorCatalogOrder DecorCatalogOrder = null;

        DataTable CurrencyTypesDT;
        public DataTable ProfilReportDataTable = null;
        public DataTable TPSReportDataTable = null;
        private DataTable DecorProductsDataTable = null;
        private DataTable DecorConfigDataTable = null;
        private DataTable MeasuresDataTable = null;
        private DataTable DecorOrdersDataTable = null;
        private DataTable DecorDataTable = null;

        public InvoiceDecorReportToDBF(ref DecorCatalogOrder tDecorCatalogOrder)
        {
            DecorCatalogOrder = tDecorCatalogOrder;

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
        }

        private void CreateReportDataTable()
        {
            ProfilReportDataTable = new DataTable();

            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.String")));

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
            DT.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
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

            TDT.Columns.Remove("Count");
            TDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));


            for (int r = 0; r < Rows.Count(); r++)
            {
                int D = Convert.ToInt32(Rows[r]["DecorConfigID"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]) * L;
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]) * L;
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
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
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
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
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                            }
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
                            InvRows[0]["Weight"] = Convert.ToDecimal(InvRows[0]["Weight"]) +
                                GetDecorWeight(Rows[r]);
                        }
                    }
                    else
                    {
                        DataRow[] InvRows = TDT.Select("InvNumber = '" + Rows[r]["InvNumber"].ToString() + "'");
                        string InvNumber = Rows[r]["InvNumber"].ToString();
                        if (InvRows.Count() == 0)
                        {
                            DataRow NewRow = TDT.NewRow();
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
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
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
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
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);
                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) + Square;
                            }
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
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

                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]) / (Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);
                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]) / (Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
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

                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]) / Convert.ToDecimal(PDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]) / Convert.ToDecimal(TDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow[p1] = Convert.ToDecimal(Rows[r][p1]);
                            NewRow[p2] = Convert.ToDecimal(Rows[r][p2]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow[p1] = Convert.ToDecimal(Rows[r][p1]);
                            NewRow[p2] = Convert.ToDecimal(Rows[r][p2]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow[p3] = Convert.ToDecimal(Rows[r][p3]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow[p3] = Convert.ToDecimal(Rows[r][p3]);
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            //NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            InvRows[0]["Count"] = Convert.ToDecimal(InvRows[0]["Count"]) +
                                Convert.ToDecimal(Rows[r]["Count"]);
                            //InvRows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            InvRows[0]["Cost"] = Convert.ToDecimal(InvRows[0]["Cost"]) +
                                Convert.ToDecimal(Rows[r]["Cost"]);
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

                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();
                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
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
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();
                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        //NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }


        }

        private void Collect()
        {
            DataTable Items = new DataTable();

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Items = DV.ToTable(true, new string[] { "InvNumber" });
            }

            for (int i = 0; i < Items.Rows.Count; i++)
            {
                string InvNumber = Items.Rows[i]["InvNumber"].ToString();
                DataRow[] ItemsRows = DecorOrdersDataTable.Select("InvNumber = '" + InvNumber + "'",
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

            Items.Dispose();
        }

        public void Report(int[] MainOrderIDs)
        {
            string sWhere = "";

            for (int i = 0; i < MainOrderIDs.Count(); i++)
            {
                if (sWhere != "")
                    sWhere += " OR MainOrderID = " + MainOrderIDs[i].ToString();
                else
                    sWhere += "MainOrderID = " + MainOrderIDs[i].ToString();
            }

            string SelectCommand = "SELECT DecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber FROM DecorOrders" +
                " INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE InvNumber IS NOT NULL AND DecorOrders.MainOrderID IN (" + string.Join(",", MainOrderIDs) + ") ORDER BY InvNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            Collect();

        }

    }







    public class InvoiceReportToDBF
    {
        decimal VAT = 1.2m;
        public InvoiceFrontsReportToDBF FrontsReport;
        public InvoiceDecorReportToDBF DecorReport = null;
        //HSSFWorkbook hssfworkbook;
        public DataTable CurrencyTypesDataTable = null;
        public DataTable ProfilReportTable = null;
        public DataTable TPSReportTable = null;

        public InvoiceReportToDBF(ref DecorCatalogOrder DecorCatalogOrder, ref FrontsCalculate FC)
        {
            FrontsReport = new InvoiceFrontsReportToDBF(ref FC);
            DecorReport = new InvoiceDecorReportToDBF(ref DecorCatalogOrder);

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
            ProfilReportTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("IsNonStandard", Type.GetType("System.Boolean")));
            ProfilReportTable.Columns.Add(new DataColumn("NonStandardMargin", Type.GetType("System.Decimal")));

            TPSReportTable = ProfilReportTable.Clone();
        }

        public void Collect(decimal Rate, bool IsNonStandard, int CurrencyType)
        {
            string IsNonStandardFilter = "(IsNonStandard=0 OR IsNonStandard IS NULL)";
            if (IsNonStandard)
                IsNonStandardFilter = "IsNonStandard=1";
            DataTable DistinctInvNumbersDT;
            using (DataView DV = new DataView(ProfilReportTable))
            {
                DV.RowFilter = IsNonStandardFilter;
                DistinctInvNumbersDT = DV.ToTable(true, new string[] { "AccountingName", "InvNumber" });
            }
            IsNonStandardFilter = " AND " + IsNonStandardFilter;
            for (int i = 0; i < DistinctInvNumbersDT.Rows.Count; i++)
            {
                decimal Cost = 0;
                decimal Count = 0;
                decimal Price = 0;
                //decimal Weight = 0;
                int DecCount = 2;
                string AccountingName = DistinctInvNumbersDT.Rows[i]["AccountingName"].ToString();
                string InvNumber = DistinctInvNumbersDT.Rows[i]["InvNumber"].ToString();

                //if (Convert.ToInt32(CurrencyCode) == 974)
                //    DecCount = 0;

                DataRow[] InvRows = ProfilReportTable.Select("InvNumber = '" + InvNumber + "' AND AccountingName='" + AccountingName + "'" + IsNonStandardFilter);
                if (InvRows.Count() > 0)
                {
                    for (int j = 0; j < InvRows.Count(); j++)
                    {
                        Count = Convert.ToDecimal(InvRows[j]["Count"]);
                        Cost = Convert.ToDecimal(InvRows[j]["Cost"]) * Rate / VAT;
                        Price = Cost / Count;
                        //if (CurrencyType == 4)
                        //{
                        //    DecCount = 0;
                        //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                        //    Cost = Price * Count;
                        //}
                        InvRows[j]["Price"] = Decimal.Round(Price, DecCount, MidpointRounding.AwayFromZero);
                        InvRows[j]["Cost"] = Decimal.Round(Cost, DecCount, MidpointRounding.AwayFromZero);
                    }
                }
            }
            IsNonStandardFilter = "(IsNonStandard=0 OR IsNonStandard IS NULL)";
            if (IsNonStandard)
                IsNonStandardFilter = "IsNonStandard=1";
            DistinctInvNumbersDT.Dispose();
            using (DataView DV = new DataView(TPSReportTable))
            {
                DV.RowFilter = IsNonStandardFilter;
                DistinctInvNumbersDT = DV.ToTable(true, new string[] { "AccountingName", "InvNumber" });
            }
            IsNonStandardFilter = " AND " + IsNonStandardFilter;
            for (int i = 0; i < DistinctInvNumbersDT.Rows.Count; i++)
            {
                decimal Cost = 0;
                decimal Count = 0;
                decimal Price = 0;
                //decimal Weight = 0;
                int DecCount = 2;
                string AccountingName = DistinctInvNumbersDT.Rows[i]["AccountingName"].ToString();
                string InvNumber = DistinctInvNumbersDT.Rows[i]["InvNumber"].ToString();

                //if (TPSCurCode.Equals("974"))
                //    DecCount = 0;

                DataRow[] InvRows = TPSReportTable.Select("InvNumber = '" + InvNumber + "' AND AccountingName='" + AccountingName + "'" + IsNonStandardFilter);
                if (InvRows.Count() > 0)
                {
                    for (int j = 0; j < InvRows.Count(); j++)
                    {
                        Count = Convert.ToDecimal(InvRows[j]["Count"]);
                        Cost = Convert.ToDecimal(InvRows[j]["Cost"]) * Rate / VAT;
                        Price = Cost / Count;
                        //if (CurrencyType == 4)
                        //{
                        //    DecCount = 0;
                        //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                        //    Cost = Price * Count;
                        //}
                        InvRows[j]["Price"] = Decimal.Round(Price, DecCount, MidpointRounding.AwayFromZero);
                        InvRows[j]["Cost"] = Decimal.Round(Cost, DecCount, MidpointRounding.AwayFromZero);
                    }
                }
            }
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

        public string GetNbrbBankData(DateTime Date)
        {
            string url = "http://nbrb.by/statistics/Rates/RatesPrint.asp?fromDate=" + Date.ToString("yyyy-M-d");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return "";
            }

            //Encoding encode = System.Text.Encoding.GetEncoding("windows-1251");

            StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());

            string s = myStreamReader.ReadToEnd();

            myStreamReader.Dispose();
            myHttpWebResponse.Close();

            return s;
        }

        public decimal GetEURBYRCurrency(string NbrbBankData)
        {
            string currency = string.Empty;
            decimal Currency;

            try
            {
                int start = NbrbBankData.IndexOf("EUR");


                for (int i = start + 76; i < NbrbBankData.Length; i++)
                {
                    if (NbrbBankData[i] != '<')
                        currency += NbrbBankData[i].ToString();
                    else
                        break;
                }
            }
            catch
            {
                return 0;
            }

            try
            {
                currency = currency.Replace('.', ',');

                Currency = Convert.ToDecimal(currency);
            }
            catch
            {
                return 0;
            }

            return Convert.ToDecimal(currency);
        }

        public void CreateReport(ref HSSFWorkbook hssfworkbook, DateTime DispatchDate, int[] MainOrdersIDs, int CurrencyType)
        {
            ClearReport();

            string Currency = string.Empty;
            decimal Rate = 1;
            if (CurrencyType == 1)
            {
                Currency = "EUR";
            }
            if (CurrencyType == 5)
            {
                string NbrbBankData = GetNbrbBankData(DispatchDate);
                Rate = GetEURBYRCurrency(NbrbBankData);
                Currency = "BYN";
            }

            string MainOrdersList = string.Empty;

            int pos = 0;

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

            decimal TotalCost = 0;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;
            decimal WeightProfil = 0;
            decimal WeightTPS = 0;

            Collect(Rate, false, CurrencyType);
            Collect(Rate, true, CurrencyType);
            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                WeightProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                WeightTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
            }

            TotalCost = (TotalProfil + TotalTPS);

            TotalProfil = Decimal.Round(TotalProfil, 3, MidpointRounding.AwayFromZero);
            TotalTPS = Decimal.Round(TotalTPS, 3, MidpointRounding.AwayFromZero);
            TotalCost = Decimal.Round(TotalCost, 3, MidpointRounding.AwayFromZero);

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
            sheet1.SetColumnWidth(1, 71 * 256);
            sheet1.SetColumnWidth(2, 8 * 256);
            sheet1.SetColumnWidth(3, 12 * 256);
            sheet1.SetColumnWidth(4, 12 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 8 * 256);
            sheet1.SetColumnWidth(7, 10 * 256);

            HSSFCell Cell1;

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
            int DisplayIndex = 0;
            if (ProfilReportTable.Rows.Count > 0)
            {
                //Профиль
                pos += 2;

                Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
                Cell1.SetCellValue("ЗОВ-Профиль:");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Инв.№");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Вес, кг. ");
                Cell1.CellStyle = SimpleHeaderCS;

                pos++;

                for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["InvNumber"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ProfilReportTable.Rows[i]["AccountingName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Price"]));
                    Cell1.CellStyle = PriceForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                    {
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["NonStandardMargin"]));
                    }
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Cost"]));
                    Cell1.CellStyle = PriceForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;

                    pos++;
                }

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Заказ,  " + Currency + ":");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(5);
                Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                Cell1.CellStyle = SummaryWithoutBorderBelCS;
                Cell1 = sheet1.CreateRow(pos - 1).CreateCell(6);
                Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                Cell1.CellStyle = SummaryWeightCS;
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
                Cell1.SetCellValue("Инв.№");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Цена, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Вес, кг. ");
                Cell1.CellStyle = SimpleHeaderCS;

                pos++;

                for (int i = 0; i < TPSReportTable.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["InvNumber"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(TPSReportTable.Rows[i]["AccountingName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Count"]));
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Price"]));
                    Cell1.CellStyle = PriceForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                    {
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["NonStandardMargin"]));
                    }
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                    Cell1.CellStyle = PriceForeignCS;

                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    pos++;
                }

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Заказ, " + Currency + ":");
                Cell1.CellStyle = SummaryWithoutBorderBelCS;
                Cell1 = sheet1.CreateRow(pos++).CreateCell(5);
                Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                Cell1.CellStyle = SummaryWithoutBorderBelCS;
                Cell1 = sheet1.CreateRow(pos - 1).CreateCell(6);
                Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                Cell1.CellStyle = SummaryWeightCS;
            }
            pos++;

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого, " + Currency + ":");
            Cell1.CellStyle = SummaryWithBorderBelCS;
            Cell1 = sheet1.CreateRow(pos++).CreateCell(1);
            Cell1.SetCellValue(Convert.ToDouble((TotalCost)));
            Cell1.CellStyle = SummaryWithBorderBelCS;
            #endregion
        }

        public void ClearReport()
        {
            ProfilReportTable.Clear();
            TPSReportTable.Clear();
            FrontsReport.ClearReport();
            DecorReport.ClearReport();
        }
    }




    public class FrontsReport
    {
        FrontsCalculate FC = null;

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
        private DataTable TechnoProfilesDataTable = null;
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
            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.String")));
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
                decimal SolidWeight = 0;

                decimal FilenkaCount = 0;
                decimal FilenkaCost = 0;
                decimal OriginalFilenkaCost = 0;
                decimal FilenkaWeight = 0;

                decimal VitrinaCount = 0;
                decimal VitrinaCost = 0;
                decimal OriginalVitrinaCost = 0;
                decimal VitrinaWeight = 0;

                decimal LuxCount = 0;
                decimal LuxCost = 0;
                decimal OriginalLuxCost = 0;
                decimal LuxWeight = 0;

                decimal MegaCount = 0;
                decimal MegaCost = 0;
                decimal OriginalMegaCost = 0;
                decimal MegaWeight = 0;

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
                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        SolidCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                      Convert.ToDecimal(Rows[r]["Count"]);
                    else
                        SolidCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                    {
                        SolidCost += Convert.ToDecimal(Rows[r]["Cost"]);
                        OriginalSolidCost += Convert.ToDecimal(Rows[r]["OriginalCost"]);
                    }
                    else
                    {
                        SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    }

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
                    if (Convert.ToInt32(Rows[r]["FrontID"]) == 3728 || Convert.ToInt32(Rows[r]["FrontID"]) == 3731 || Convert.ToInt32(Rows[r]["FrontID"]) == 3732 ||
                        Convert.ToInt32(Rows[r]["FrontID"]) == 3739 || Convert.ToInt32(Rows[r]["FrontID"]) == 3740 || Convert.ToInt32(Rows[r]["FrontID"]) == 3741 ||
                        Convert.ToInt32(Rows[r]["FrontID"]) == 3744 || Convert.ToInt32(Rows[r]["FrontID"]) == 3745 || Convert.ToInt32(Rows[r]["FrontID"]) == 3746)
                    {

                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                            SolidCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                          Convert.ToDecimal(Rows[r]["Count"]);
                        else
                            SolidCount += Convert.ToDecimal(Rows[r]["Square"]);

                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        {
                            SolidCost += Convert.ToDecimal(Rows[r]["Cost"]);
                            OriginalSolidCost += Convert.ToDecimal(Rows[r]["OriginalCost"]);
                        }
                        else
                        {
                            SolidCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                        }

                        decimal FrontWeight = 0;
                        decimal InsetWeight = 0;

                        GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                        SolidWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    }
                    else if (Convert.ToInt32(Rows[r]["FrontID"]) == 3415 || Convert.ToInt32(Rows[r]["FrontID"]) == 28922)
                    {
                        decimal DeductibleCost = Convert.ToDecimal(Rows[r]["InsetPrice"]) * Convert.ToDecimal(Rows[r]["Count"]);
                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                            FilenkaCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                          Convert.ToDecimal(Rows[r]["Count"]);
                        else
                            FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                        if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        {
                            FilenkaCost += Convert.ToDecimal(Rows[r]["Cost"]) - DeductibleCost;
                            OriginalFilenkaCost += Convert.ToDecimal(Rows[r]["OriginalCost"]) - DeductibleCost;
                        }
                        else
                        {
                            FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]) - DeductibleCost;
                        }

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
                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        FilenkaCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                      Convert.ToDecimal(Rows[r]["Count"]);
                    else
                        FilenkaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                    {
                        FilenkaCost += Convert.ToDecimal(Rows[r]["Cost"]);
                        OriginalFilenkaCost += Convert.ToDecimal(Rows[r]["OriginalCost"]);
                    }
                    else
                    {
                        FilenkaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    }

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
                    decimal DeductibleWeight = 0;
                    //РЕШЕТКА 45,90,ПЛАСТИК
                    if (Convert.ToInt32(Rows[r]["InsetTypeID"]) == 685 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 686 ||
                        Convert.ToInt32(Rows[r]["InsetTypeID"]) == 687 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 688 ||
                        Convert.ToInt32(Rows[r]["InsetTypeID"]) == 29470 || Convert.ToInt32(Rows[r]["InsetTypeID"]) == 29471)
                    {
                        DeductibleCount = GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]),
                                                    Convert.ToInt32(Rows[r]["Width"])) * Convert.ToDecimal(Rows[r]["Count"]);
                        DeductibleWeight = Decimal.Round(DeductibleCount * Convert.ToDecimal(3.5), 3, MidpointRounding.AwayFromZero);
                    }
                    //СТЕКЛО
                    if (Convert.ToInt32(Rows[r]["InsetTypeID"]) == 2)
                    {
                        DeductibleCount = Convert.ToDecimal(Rows[r]["Count"]) * GetInsetSquare(Convert.ToInt32(Rows[r]["FrontID"]), Convert.ToInt32(Rows[r]["Height"]), Convert.ToInt32(Rows[r]["Width"]));
                        DeductibleWeight = Decimal.Round(DeductibleCount * 10, 3, MidpointRounding.AwayFromZero);
                    }

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        VitrinaCount += Decimal.Round(Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) / 1000000, 3, MidpointRounding.AwayFromZero) *
                                      Convert.ToDecimal(Rows[r]["Count"]);
                    else
                        VitrinaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                    {
                        VitrinaCost += Convert.ToDecimal(Rows[r]["Cost"]);
                        OriginalVitrinaCost += Convert.ToDecimal(Rows[r]["OriginalCost"]);
                    }
                    else
                    {
                        VitrinaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    }

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    if (Convert.ToInt32(Rows[r]["InsetTypeID"]) == 2)
                    {
                        FrontWeight = FC.GetAluminiumWeight(Rows[r], true);
                    }

                    VitrinaWeight += Convert.ToDecimal(FrontWeight + InsetWeight - DeductibleWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }
                //ЛЮКС
                filter = " AND InsetTypeID IN (860)";
                Rows = OrdersDataTable.Select("FrontID = " + Fronts.Rows[i]["FrontID"].ToString() + " AND (Width <> -1)" + filter + IsNonStandardFilter);
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        LuxCount += Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) *
                                      Convert.ToDecimal(Rows[r]["Count"]) / 1000000;
                    else
                        LuxCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                    {
                        LuxCost += Convert.ToDecimal(Rows[r]["Cost"]);
                        OriginalLuxCost += Convert.ToDecimal(Rows[r]["OriginalCost"]);
                    }
                    else
                    {
                        LuxCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    }

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
                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                        MegaCount += Convert.ToDecimal(Rows[r]["Height"]) * Convert.ToDecimal(Rows[r]["Width"]) *
                                      Convert.ToDecimal(Rows[r]["Count"]) / 1000000;
                    else
                        MegaCount += Convert.ToDecimal(Rows[r]["Square"]);

                    if (GetMeasureType(Convert.ToInt32(Rows[r]["FrontConfigID"])) == 3)
                    {
                        MegaCost += Convert.ToDecimal(Rows[r]["Cost"]);
                        OriginalMegaCost += Convert.ToDecimal(Rows[r]["OriginalCost"]);
                    }
                    else
                    {
                        MegaCost += Convert.ToDecimal(Rows[r]["FrontPrice"]) * Convert.ToDecimal(Rows[r]["Square"]);
                    }

                    decimal FrontWeight = 0;
                    decimal InsetWeight = 0;

                    GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                    MegaWeight += Convert.ToDecimal(FrontWeight + InsetWeight);
                    NonStandardMargin = GetNonStandardMargin(Convert.ToInt32(Rows[r]["FrontConfigID"]));
                }

                if (!IsNonStandard)
                    NonStandardMargin = DBNull.Value;
                if (SolidCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " глухой";
                    Row["Count"] = Decimal.Round(SolidCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(SolidCost / SolidCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(SolidCost / SolidCount * Convert.ToDecimal(Row["Count"]), 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(SolidWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (FilenkaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " филенка";
                    Row["Count"] = Decimal.Round(FilenkaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(FilenkaCost / FilenkaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(FilenkaCost, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(FilenkaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (VitrinaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " витрина";
                    Row["Count"] = Decimal.Round(VitrinaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(VitrinaCost / VitrinaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(VitrinaCost, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(VitrinaWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (LuxCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " люкс";
                    Row["Count"] = Decimal.Round(LuxCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(LuxCost / LuxCount, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(LuxCost, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(LuxWeight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }
                if (MegaCount > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["NonStandardMargin"] = NonStandardMargin;
                    Row["IsNonStandard"] = IsNonStandard;
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " мега";
                    Row["Count"] = Decimal.Round(MegaCount, 3, MidpointRounding.AwayFromZero);
                    Row["Measure"] = "м.кв.";
                    Row["Price"] = Decimal.Round(MegaCost / MegaCount, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(MegaCost, 3, MidpointRounding.AwayFromZero);
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
                decimal Solid713Weight = 0;

                decimal Filenka713Count = 0;
                decimal Filenka713Price = 0;
                decimal Filenka713Weight = 0;

                decimal NoInset713Count = 0;
                decimal NoInset713Price = 0;
                decimal NoInset713Weight = 0;

                decimal Vitrina713Count = 0;
                decimal Vitrina713Price = 0;
                decimal Vitrina713Weight = 0;

                decimal Solid910Count = 0;
                decimal Solid910Price = 0;
                decimal Solid910Weight = 0;

                decimal Filenka910Count = 0;
                decimal Filenka910Price = 0;
                decimal Filenka910Weight = 0;

                decimal NoInset910Count = 0;
                decimal NoInset910Price = 0;
                decimal NoInset910Weight = 0;

                decimal Vitrina910Count = 0;
                decimal Vitrina910Price = 0;
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

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset713Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina713Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina713Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);

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

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            NoInset910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }

                        if (Rows[r]["InsetTypeID"].ToString() == "1")
                        {
                            Vitrina910Count += Convert.ToDecimal(Rows[r]["Count"]);
                            Vitrina910Price = Convert.ToDecimal(Rows[r]["FrontPrice"]);

                            decimal FrontWeight = 0;
                            decimal InsetWeight = 0;

                            GetFrontWeight(Rows[r], ref FrontWeight, ref InsetWeight);

                            Vitrina910Weight += Convert.ToDecimal(FrontWeight + InsetWeight);
                        }
                    }

                }




                if (Solid713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 глух.";
                    Row["Count"] = Solid713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Solid713Price, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Solid713Count * Solid713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Filenka713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 филенка";
                    Row["Count"] = Filenka713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Filenka713Price, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Filenka713Count * Filenka713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (NoInset713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 фрезер.";
                    Row["Count"] = NoInset713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(NoInset713Price, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(NoInset713Count * NoInset713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Vitrina713Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 713 витрина";
                    Row["Count"] = Vitrina713Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Vitrina713Price, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Vitrina713Count * Vitrina713Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina713Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Solid910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 глух.";
                    Row["Count"] = Solid910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Solid910Price, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Solid910Count * Solid910Price, 2, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Solid910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Filenka910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 филенка";
                    Row["Count"] = Filenka910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Filenka910Price, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Filenka910Count * Filenka910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Filenka910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (NoInset910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 фрезер.";
                    Row["Count"] = NoInset910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(NoInset910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(NoInset910Count * NoInset910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(NoInset910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }

                if (Vitrina910Count > 0)
                {
                    DataRow Row = ReportDataTable.NewRow();
                    Row["AccountingName"] = AccountingName;
                    Row["InvNumber"] = InvNumber;
                    Row["Name"] = GetFrontName(Convert.ToInt32(Fronts.Rows[i]["FrontID"])) + " гнут. 910 витрина";
                    Row["Count"] = Vitrina910Count;
                    Row["Measure"] = "шт.";
                    Row["Price"] = Decimal.Round(Vitrina910Price, 2, MidpointRounding.AwayFromZero);
                    Row["Cost"] = Decimal.Round(Vitrina910Count * Vitrina910Price, 3, MidpointRounding.AwayFromZero);
                    Row["Weight"] = Decimal.Round(Vitrina910Weight, 3, MidpointRounding.AwayFromZero);
                    ReportDataTable.Rows.Add(Row);
                }
            }

            Fronts.Dispose();
        }

        private void GetGrids(DataTable OrdersDataTable, DataTable ReportDataTable)
        {
            DataRow[] Rows = OrdersDataTable.Select("InsetTypeID IN (685,686,687,688,29470,29471) AND FrontID <> 3729");

            if (Rows.Count() == 0)
                return;

            int fID = Convert.ToInt32(OrdersDataTable.Rows[0]["FactoryID"]);
            decimal Count = 0;
            decimal Cost = 0;

            for (int i = 0; i < Rows.Count(); i++)
            {
                decimal d = GetInsetSquare(Convert.ToInt32(Rows[i]["FrontID"]), Convert.ToInt32(Rows[i]["Height"]),
                                               Convert.ToInt32(Rows[i]["Width"])) * Convert.ToDecimal(Rows[i]["Count"]);
                Count += d;
                Cost += Convert.ToDecimal(Rows[i]["InsetPrice"]) * d;
            }

            if (Count > 0)
            {
                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["AccountingName"] = OrdersDataTable.Rows[0]["AccountingName"].ToString();
                NewRow["InvNumber"] = OrdersDataTable.Rows[0]["InvNumber"].ToString();
                NewRow["Name"] = "Решетка";
                NewRow["Count"] = Decimal.Round(Count, 3, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = Decimal.Round(Cost / Count, 3, MidpointRounding.AwayFromZero);
                NewRow["Cost"] = Decimal.Round(Cost, 3, MidpointRounding.AwayFromZero);
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

            DataRow[] FRows = OrdersDataTable.Select("InsetColorID = 3944");

            if (FRows.Count() > 0)
            {
                PriceFlutes = Convert.ToDecimal(FRows[0]["InsetPrice"]);

                for (int i = 0; i < FRows.Count(); i++)
                {
                    if (FC.IsAluminium(FRows[i]) != -1)
                        continue;

                    CountFlutes += Convert.ToDecimal(FRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(FRows[i]["FrontID"]),
                                              Convert.ToInt32(FRows[i]["Height"]),
                                              Convert.ToInt32(FRows[i]["Width"]));
                }

                CountFlutes = Decimal.Round(CountFlutes, 3, MidpointRounding.AwayFromZero);

                if (CountFlutes > 0)
                {
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["Name"] = "Стекло Флутес";
                    NewRow["Count"] = CountFlutes;
                    NewRow["Measure"] = "м.кв.";
                    NewRow["Price"] = Decimal.Round(PriceFlutes, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(CountFlutes * PriceFlutes, 3, MidpointRounding.AwayFromZero);
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

                    CountLacomat += Convert.ToDecimal(LRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(LRows[i]["FrontID"]),
                                              Convert.ToInt32(LRows[i]["Height"]),
                                              Convert.ToInt32(LRows[i]["Width"]));

                }

                CountLacomat = Decimal.Round(CountLacomat, 3, MidpointRounding.AwayFromZero);

                if (CountLacomat > 0)
                {
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["Name"] = "Стекло Лакомат";
                    NewRow["Count"] = CountLacomat;
                    NewRow["Measure"] = "м.кв.";
                    NewRow["Price"] = Decimal.Round(PriceLacomat, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(CountLacomat * PriceLacomat, 3, MidpointRounding.AwayFromZero);
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

                    CountKrizet += Convert.ToDecimal(KRows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(KRows[i]["FrontID"]),
                                              Convert.ToInt32(KRows[i]["Height"]),
                                              Convert.ToInt32(KRows[i]["Width"]));
                }

                CountKrizet = Decimal.Round(CountKrizet, 3, MidpointRounding.AwayFromZero);

                if (CountKrizet > 0)
                {
                    DataRow NewRow = ReportDataTable.NewRow();
                    NewRow["AccountingName"] = AccountingName;
                    NewRow["InvNumber"] = InvNumber;
                    NewRow["Name"] = "Стекло Кризет";
                    NewRow["Count"] = CountKrizet;
                    NewRow["Measure"] = "м.кв.";
                    NewRow["Price"] = Decimal.Round(PriceKrizet, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(CountKrizet * PriceKrizet, 3, MidpointRounding.AwayFromZero);
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

                    CountOther += Convert.ToDecimal(ORows[i]["Count"]) *
                                GetInsetSquare(Convert.ToInt32(ORows[i]["FrontID"]),
                                              Convert.ToInt32(ORows[i]["Height"]),
                                              Convert.ToInt32(ORows[i]["Width"]));
                }

                CountOther = Decimal.Round(CountOther, 3, MidpointRounding.AwayFromZero);

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Стекло другое";
                NewRow["Count"] = CountOther;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = 0;
                NewRow["Cost"] = 0;
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
            int CountEllipseGrid = 0;

            decimal PriceApplic1R = 0;
            decimal PriceApplic1L = 0;
            decimal PriceApplic2 = 0;
            decimal PriceEllipseGrid = 0;

            DataRow[] EGRows = OrdersDataTable.Select("FrontID IN (3729)");//ellipse grid

            if (EGRows.Count() > 0)
            {
                PriceEllipseGrid = Convert.ToDecimal(EGRows[0]["InsetPrice"]);

                for (int i = 0; i < EGRows.Count(); i++)
                {
                    CountEllipseGrid += Convert.ToInt32(EGRows[i]["Count"]);
                }

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Решетка овал";
                NewRow["Count"] = CountEllipseGrid;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceEllipseGrid;
                NewRow["Cost"] = Decimal.Round(CountEllipseGrid * PriceEllipseGrid);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }

            DataRow[] A1RRows = OrdersDataTable.Select("InsetTypeID = 3654 OR FrontID IN (3731,3740,3746)");//applic 1 right

            if (A1RRows.Count() > 0)
            {
                PriceApplic1R = Convert.ToDecimal(A1RRows[0]["InsetPrice"]);

                for (int i = 0; i < A1RRows.Count(); i++)
                {
                    CountApplic1R += Convert.ToInt32(A1RRows[i]["Count"]);
                }

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Апплик. №1 правая";
                NewRow["Count"] = CountApplic1R;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic1R;
                NewRow["Cost"] = Decimal.Round(CountApplic1R * PriceApplic1R);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }

            DataRow[] A1LRows = OrdersDataTable.Select("InsetTypeID = 3653 OR FrontID IN (3728,3739,3745)");//applic 1 left

            if (A1LRows.Count() > 0)
            {
                PriceApplic1L = Convert.ToDecimal(A1LRows[0]["InsetPrice"]);

                for (int i = 0; i < A1LRows.Count(); i++)
                {
                    CountApplic1L += Convert.ToInt32(A1LRows[i]["Count"]);
                }

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Апплик. №1 левая";
                NewRow["Count"] = CountApplic1L;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic1L;
                NewRow["Cost"] = Decimal.Round(CountApplic1L * PriceApplic1L);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }

            DataRow[] A2Rows = OrdersDataTable.Select("InsetTypeID = 3655 OR FrontID IN (3732,3741,3744)");//applic 2 

            if (A2Rows.Count() > 0)
            {
                PriceApplic2 = Convert.ToDecimal(A2Rows[0]["InsetPrice"]);

                for (int i = 0; i < A2Rows.Count(); i++)
                {
                    CountApplic2 += Convert.ToInt32(A2Rows[i]["Count"]);
                }

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Апплик. №2";
                NewRow["Count"] = CountApplic2;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic2;
                NewRow["Cost"] = Decimal.Round(CountApplic2 * PriceApplic2);
                NewRow["Weight"] = 0;
                ReportDataTable.Rows.Add(NewRow);
            }


            DataRow[] A3Rows = OrdersDataTable.Select("InsetTypeID = 28961");//applic 3 

            if (A3Rows.Count() > 0)
            {
                PriceApplic2 = Convert.ToDecimal(A3Rows[0]["InsetPrice"]);

                for (int i = 0; i < A3Rows.Count(); i++)
                {
                    CountApplic2 += Convert.ToInt32(A3Rows[i]["Count"]);
                }

                DataRow NewRow = ReportDataTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Апплик. №3";
                NewRow["Count"] = CountApplic2;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic2;
                NewRow["Cost"] = Decimal.Round(CountApplic2 * PriceApplic2);
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

        public void Report(int[] MainOrderIDs)
        {
            ProfilFrontsOrdersDataTable.Clear();
            TPSFrontsOrdersDataTable.Clear();

            string sWhere = "";

            for (int i = 0; i < MainOrderIDs.Count(); i++)
            {
                if (sWhere != "")
                    sWhere += " OR FrontsOrders.MainOrderID = " + MainOrderIDs[i].ToString();
                else
                    sWhere += "FrontsOrders.MainOrderID = " + MainOrderIDs[i].ToString();
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber FROM FrontsOrders
                INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE " + sWhere + " ORDER BY FrontID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    ProfilFrontsOrdersDataTable = DT.Clone();
                    TPSFrontsOrdersDataTable = DT.Clone();

                    SplitTables(DT, ref ProfilFrontsOrdersDataTable, ref TPSFrontsOrdersDataTable);


                    if (ProfilFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        GetSimpleFronts(ProfilFrontsOrdersDataTable, ProfilReportDataTable, false);
                        GetSimpleFronts(ProfilFrontsOrdersDataTable, ProfilReportDataTable, true);

                        GetCurvedFronts(ProfilFrontsOrdersDataTable, ProfilReportDataTable);

                        GetGrids(ProfilFrontsOrdersDataTable, ProfilReportDataTable);

                        GetInsets(ProfilFrontsOrdersDataTable, ProfilReportDataTable);

                        GetGlass(ProfilFrontsOrdersDataTable, ProfilReportDataTable);
                    }

                    if (TPSFrontsOrdersDataTable.Rows.Count > 0)
                    {
                        GetSimpleFronts(TPSFrontsOrdersDataTable, TPSReportDataTable, false);
                        GetSimpleFronts(TPSFrontsOrdersDataTable, TPSReportDataTable, true);

                        GetCurvedFronts(TPSFrontsOrdersDataTable, TPSReportDataTable);

                        GetGrids(TPSFrontsOrdersDataTable, TPSReportDataTable);

                        GetInsets(TPSFrontsOrdersDataTable, TPSReportDataTable);

                        GetGlass(TPSFrontsOrdersDataTable, TPSReportDataTable);
                    }
                }
            }

        }
    }





    public class DecorReport
    {
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
            DecorConfigDataTable = TablesManager.DecorConfigDataTable;

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
            ProfilReportDataTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.String")));
            ProfilReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.String")));
            TPSReportDataTable = ProfilReportDataTable.Clone();
            //TPSReportDataTable = new DataTable();
            //TPSReportDataTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            //TPSReportDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            //TPSReportDataTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            //TPSReportDataTable.Columns.Add(new DataColumn("Price", Type.GetType("System.String")));
            //TPSReportDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.String")));
            //TPSReportDataTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.String")));
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

        //private string GetCoverType(int ColorID)
        //{
        //    DataRow[] CRow = DecorCatalogOrder.ColorsDataTable.Select("ColorID = " + ColorID.ToString());

        //    DataRow[] CvRow = CoverTypesDataTable.Select("CoverTypeID = " + CRow[0]["CoverTypeID"].ToString());

        //    return CvRow[0]["CoverTypeCode"].ToString();
        //}


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

            DT.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("ProductID", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("DecorID", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            DT.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
        }

        private void GroupCoverTypes(DataRow[] Rows, int MeasureTypeID, int DecorID)
        {
            DataTable PDT = new DataTable();
            DataTable TDT = new DataTable();

            PDT = DecorOrdersDataTable.Clone();
            TDT = DecorOrdersDataTable.Clone();

            PDT.Columns.Remove("Count");
            PDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));

            TDT.Columns.Remove("Count");
            TDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));


            for (int r = 0; r < Rows.Count(); r++)
            {
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
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                                      Convert.ToDecimal(Rows[r]["Count"]) * L;
                            PDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);

                            PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]) * L;
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                                      Convert.ToDecimal(Rows[r]["Count"]) * L;
                            TDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);

                            TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
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

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;
                            }

                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
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

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) + Square;
                            }

                            PDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);

                            PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
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

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);
                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                NewRow["Count"] = Square;

                            }

                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
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

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Height") && Convert.ToDecimal(Rows[r]["Height"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Height"]);

                                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(Rows[r]["ProductID"]), "Length") && Convert.ToDecimal(Rows[r]["Length"]) != -1)
                                    H = Convert.ToDecimal(Rows[r]["Length"]);

                                decimal d = H * Convert.ToDecimal(Rows[r]["Width"]) / 1000000;
                                d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                decimal Square = d * Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) + Square;
                            }
                            TDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);

                            TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
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

                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[i]["ProductID"])) + " " +
                            GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.п.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]) / (Convert.ToDecimal(PDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();
                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[i]["ProductID"])) + " " +
                            GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000, 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.п.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]) / (Convert.ToDecimal(TDT.Rows[i]["Count"]) / 1000), 2, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
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

                        NewRow["AccountingName"] = PDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[i]["InvNumber"].ToString();
                        if (PDT.Rows[i]["ProductID"].ToString() == "10" || PDT.Rows[i]["ProductID"].ToString() == "11" ||
                            PDT.Rows[i]["ProductID"].ToString() == "12")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[i]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[i]["ProductID"])) + " " +
                                             GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]) / Convert.ToDecimal(PDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[i]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }
                }

                if (TDT.Rows.Count > 0)
                {
                    for (int i = 0; i < TDT.Rows.Count; i++)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["AccountingName"] = TDT.Rows[i]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[i]["InvNumber"].ToString();
                        if (TDT.Rows[i]["ProductID"].ToString() == "10" || TDT.Rows[i]["ProductID"].ToString() == "11" ||
                            TDT.Rows[i]["ProductID"].ToString() == "12")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[i]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[i]["ProductID"])) + " " +
                                             GetItemName(DecorID);

                        NewRow["Count"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]) / Convert.ToDecimal(TDT.Rows[i]["Count"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[i]["Cost"]), 3, MidpointRounding.AwayFromZero);
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

                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                            Convert.ToDecimal(Rows[r]["Count"]);
                            PDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
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

                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                            Convert.ToDecimal(Rows[r]["Count"]);
                            TDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                            Convert.ToDecimal(Rows[r]["Cost"]);
                            TDT.Rows[0]["Weight"] = Convert.ToDecimal(TDT.Rows[0]["Weight"]) +
                                                            GetDecorWeight(Rows[r]);
                            break;
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

                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(PDT.Rows[0][p1]) == Convert.ToInt32(Rows[r][p1]) &&
                                        Convert.ToInt32(PDT.Rows[0][p2]) == Convert.ToInt32(Rows[r][p2]))
                            {
                                PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                                PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(TDT.Rows[0][p1]) == Convert.ToInt32(Rows[r][p1]) &&
                                        Convert.ToInt32(TDT.Rows[0][p2]) == Convert.ToInt32(Rows[r][p2]))
                            {
                                TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                                TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            PDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(PDT.Rows[0][p3]) == Convert.ToInt32(Rows[r][p3]))
                            {
                                PDT.Rows[0]["Count"] = Convert.ToDecimal(PDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                PDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                                PDT.Rows[0]["Cost"] = Convert.ToDecimal(PDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
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
                            NewRow["AccountingName"] = Rows[r]["AccountingName"].ToString();
                            NewRow["InvNumber"] = Rows[r]["InvNumber"].ToString();
                            NewRow["ProductID"] = Rows[r]["ProductID"];
                            NewRow["DecorID"] = Rows[r]["DecorID"];
                            NewRow["Count"] = Convert.ToDecimal(Rows[r]["Count"]);
                            NewRow["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                            NewRow["Cost"] = Convert.ToDecimal(Rows[r]["Cost"]);
                            NewRow["Weight"] = GetDecorWeight(Rows[r]);
                            TDT.Rows.Add(NewRow);
                            continue;
                        }
                        else
                        {
                            if (Convert.ToInt32(TDT.Rows[0][p3]) == Convert.ToInt32(Rows[r][p3]))
                            {
                                TDT.Rows[0]["Count"] = Convert.ToDecimal(TDT.Rows[0]["Count"]) +
                                                              Convert.ToDecimal(Rows[r]["Count"]);
                                TDT.Rows[0]["Price"] = Convert.ToDecimal(Rows[r]["Price"]);
                                TDT.Rows[0]["Cost"] = Convert.ToDecimal(TDT.Rows[0]["Cost"]) +
                                                             Convert.ToDecimal(Rows[r]["Cost"]);
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

                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        if (PDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + PDT.Rows[g][p1] + "x" + PDT.Rows[g][p2];

                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();



                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        if (PDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + PDT.Rows[g][p3];

                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        ProfilReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = ProfilReportDataTable.NewRow();

                        NewRow["AccountingName"] = PDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = PDT.Rows[g]["InvNumber"].ToString();
                        if (PDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(PDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID));

                        NewRow["Count"] = PDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(PDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
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

                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        if (TDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + TDT.Rows[g][p1] + "x" + TDT.Rows[g][p2];

                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p3.Length > 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        if (TDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID)) + " " + TDT.Rows[g][p3];

                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }

                    if (p1.Length == 0 && p2.Length == 0 && p3.Length == 0)
                    {
                        DataRow NewRow = TPSReportDataTable.NewRow();

                        NewRow["AccountingName"] = TDT.Rows[g]["AccountingName"].ToString();
                        NewRow["InvNumber"] = TDT.Rows[g]["InvNumber"].ToString();
                        if (TDT.Rows[g]["ProductID"].ToString() == "10")
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"]));
                        else
                            NewRow["Name"] = GetProductName(Convert.ToInt32(TDT.Rows[g]["ProductID"])) + " " +
                                         GetItemName(Convert.ToInt32(DecorID));

                        NewRow["Count"] = TDT.Rows[g]["Count"];
                        NewRow["Measure"] = "шт.";
                        NewRow["Price"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Price"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Cost"]), 3, MidpointRounding.AwayFromZero);
                        NewRow["Weight"] = Decimal.Round(Convert.ToDecimal(TDT.Rows[g]["Weight"]), 3, MidpointRounding.AwayFromZero);

                        TPSReportDataTable.Rows.Add(NewRow);
                    }
                }
            }


        }

        private void Collect()
        {
            DataTable Items = new DataTable();

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Items = DV.ToTable(true, new string[] { "DecorID" });
            }

            for (int i = 0; i < Items.Rows.Count; i++)
            {
                int rr = Convert.ToInt32(Items.Rows[i]["DecorID"]);

                DataRow[] ItemsRows = DecorOrdersDataTable.Select("DecorID = " + Items.Rows[i]["DecorID"].ToString(),
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

            Items.Dispose();
        }

        public void Report(int[] MainOrderIDs)
        {
            string sWhere = "";

            for (int i = 0; i < MainOrderIDs.Count(); i++)
            {
                if (sWhere != "")
                    sWhere += " OR DecorOrders.MainOrderID = " + MainOrderIDs[i].ToString();
                else
                    sWhere += "DecorOrders.MainOrderID = " + MainOrderIDs[i].ToString();
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber FROM DecorOrders
                INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                WHERE " + sWhere + " ORDER BY DecorID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            Collect();

        }

    }





    public class Report
    {
        decimal VAT = 1.2m;
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

            ProfilReportTable.Columns.Add(new DataColumn("InvNumber", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("AccountingName", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            ProfilReportTable.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("IsNonStandard", Type.GetType("System.Boolean")));
            ProfilReportTable.Columns.Add(new DataColumn("NonStandardMargin", Type.GetType("System.Decimal")));
            ProfilReportTable.Columns.Add(new DataColumn("PaymentRate", Type.GetType("System.Decimal")));
            TPSReportTable = ProfilReportTable.Clone();
            //TPSReportTable = new DataTable();

            //TPSReportTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            //TPSReportTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            //TPSReportTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            //TPSReportTable.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
            //TPSReportTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            //TPSReportTable.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
        }

        public void ConvertToCurrency(decimal Rate, bool IsNonStandard, int CurrencyType)
        {
            decimal Cost = 0;
            decimal Count = 0;
            decimal Price = 0;

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

                Count = Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                Cost = Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]) * Rate / VAT;
                Price = Cost / Count;
                //if (CurrencyType == 4)
                //{
                //    DecCount = 0;
                //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                //    Cost = Price * Count;
                //}
                if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                {
                    decimal ExtraPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["NonStandardMargin"]);
                    Price = Price / (ExtraPrice / 100 + 1);
                    ProfilReportTable.Rows[i]["NonStandardMargin"] = ExtraPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["NonStandardMargin"]);
                }
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

                Count = Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                Cost = Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]) * Rate / VAT;
                Price = Cost / Count;
                //if (CurrencyType == 4)
                //{
                //    DecCount = 0;
                //    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                //    Cost = Price * Count;
                //}
                if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                {
                    decimal ExtraPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["NonStandardMargin"]);
                    Price = Price / (ExtraPrice / 100 + 1);
                    TPSReportTable.Rows[i]["NonStandardMargin"] = ExtraPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["NonStandardMargin"]);
                }
                TPSReportTable.Rows[i]["Price"] = Decimal.Round(Price, DecCount, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows[i]["Cost"] = Decimal.Round(Cost, DecCount, MidpointRounding.AwayFromZero);
            }
        }

        public void CollectGridsAndGlass()
        {
            decimal GridSquare = 0;
            decimal GridCost = 0;
            decimal GridWeight = 0;

            decimal LacomatSquare = 0;
            decimal LacomatCost = 0;
            decimal LacomatWeight = 0;
            decimal LacomatPrice = 0;

            decimal KrizetSquare = 0;
            decimal KrizetCost = 0;
            decimal KrizetWeight = 0;
            decimal KrizetPrice = 0;

            decimal FlutesSquare = 0;
            decimal FlutesCost = 0;
            decimal FlutesWeight = 0;
            decimal FlutesPrice = 0;

            //collect
            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                bool b = false;

                if (ProfilReportTable.Rows[i]["Name"].ToString().IndexOf("Решетка") > -1)
                {
                    GridCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                    GridSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                    GridWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
                    b = true;
                }

                if (ProfilReportTable.Rows[i]["Name"].ToString() == "Стекло Лакомат")
                {
                    LacomatCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                    LacomatSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                    LacomatWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
                    LacomatPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["Price"]);
                    b = true;
                }

                if (ProfilReportTable.Rows[i]["Name"].ToString() == "Стекло Кризет")
                {
                    KrizetCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                    KrizetSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                    KrizetWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
                    KrizetPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["Price"]);
                    b = true;
                }

                if (ProfilReportTable.Rows[i]["Name"].ToString() == "Стекло Флутес")
                {
                    FlutesCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                    FlutesSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                    FlutesWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
                    FlutesPrice = Convert.ToDecimal(ProfilReportTable.Rows[i]["Price"]);
                    b = true;
                }

                if (b)
                {
                    ProfilReportTable.Rows[i].Delete();
                    ProfilReportTable.AcceptChanges();
                    i--;
                }
            }

            string AccountingName = string.Empty;
            string InvNumber = string.Empty;
            //ADD to Table

            if (ProfilReportTable.Rows.Count > 0)
            {
                AccountingName = ProfilReportTable.Rows[0]["AccountingName"].ToString();
                InvNumber = ProfilReportTable.Rows[0]["InvNumber"].ToString();
            }
            if (GridSquare > 0)
            {
                DataRow NewRow = ProfilReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Решетка";
                NewRow["Count"] = GridSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = Decimal.Round(GridCost / GridSquare, 2, MidpointRounding.AwayFromZero);
                NewRow["Cost"] = GridCost;
                NewRow["Weight"] = Decimal.Round(GridWeight, 3, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows.Add(NewRow);
            }

            GridSquare = 0;
            GridCost = 0;
            GridWeight = 0;

            if (LacomatSquare > 0)
            {
                DataRow NewRow = ProfilReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Стекло Лакомат";
                NewRow["Count"] = LacomatSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = LacomatPrice;
                NewRow["Cost"] = LacomatCost;
                NewRow["Weight"] = Decimal.Round(LacomatWeight, 3, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows.Add(NewRow);
            }

            LacomatSquare = 0;
            LacomatCost = 0;
            LacomatWeight = 0;
            LacomatPrice = 0;

            if (KrizetSquare > 0)
            {
                DataRow NewRow = ProfilReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Стекло Кризет";
                NewRow["Count"] = KrizetSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = KrizetPrice;
                NewRow["Cost"] = KrizetCost;
                NewRow["Weight"] = Decimal.Round(KrizetWeight, 3, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows.Add(NewRow);
            }

            KrizetSquare = 0;
            KrizetCost = 0;
            KrizetWeight = 0;
            KrizetPrice = 0;

            if (FlutesSquare > 0)
            {
                DataRow NewRow = ProfilReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Стекло Флутес";
                NewRow["Count"] = FlutesSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = FlutesPrice;
                NewRow["Cost"] = FlutesCost;
                NewRow["Weight"] = Decimal.Round(FlutesWeight, 3, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows.Add(NewRow);
            }

            FlutesSquare = 0;
            FlutesCost = 0;
            FlutesWeight = 0;
            FlutesPrice = 0;


            //collect
            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                bool b = false;

                if (TPSReportTable.Rows[i]["Name"].ToString().IndexOf("Решетка") > -1)
                {
                    GridCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                    GridSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                    GridWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);

                    b = true;
                }

                if (TPSReportTable.Rows[i]["Name"].ToString() == "Стекло Лакомат")
                {
                    LacomatCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                    LacomatSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                    LacomatWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
                    LacomatPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["Price"]);
                    b = true;
                }

                if (TPSReportTable.Rows[i]["Name"].ToString() == "Стекло Кризет")
                {
                    KrizetCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                    KrizetSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                    KrizetWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
                    KrizetPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["Price"]);
                    b = true;
                }

                if (TPSReportTable.Rows[i]["Name"].ToString() == "Стекло Флутес")
                {
                    FlutesCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                    FlutesSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                    FlutesWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
                    FlutesPrice = Convert.ToDecimal(TPSReportTable.Rows[i]["Price"]);
                    b = true;
                }

                if (b)
                {
                    TPSReportTable.Rows[i].Delete();
                    TPSReportTable.AcceptChanges();
                    i--;
                }
            }

            AccountingName = string.Empty;
            InvNumber = string.Empty;
            //ADD to Table

            if (TPSReportTable.Rows.Count > 0)
            {
                AccountingName = TPSReportTable.Rows[0]["AccountingName"].ToString();
                InvNumber = TPSReportTable.Rows[0]["InvNumber"].ToString();
            }

            //ADD to Table
            if (GridSquare > 0)
            {
                DataRow NewRow = TPSReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Решетка";
                NewRow["Count"] = GridSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = Decimal.Round(GridCost / GridSquare, 2, MidpointRounding.AwayFromZero);
                NewRow["Cost"] = GridCost;
                NewRow["Weight"] = Decimal.Round(GridWeight, 3, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows.Add(NewRow);
            }

            if (LacomatSquare > 0)
            {
                DataRow NewRow = TPSReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Стекло Лакомат";
                NewRow["Count"] = LacomatSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = LacomatPrice;
                NewRow["Cost"] = LacomatCost;
                NewRow["Weight"] = Decimal.Round(LacomatWeight, 3, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows.Add(NewRow);
            }

            if (KrizetSquare > 0)
            {
                DataRow NewRow = TPSReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Стекло Кризет";
                NewRow["Count"] = KrizetSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = KrizetPrice;
                NewRow["Cost"] = KrizetCost;
                NewRow["Weight"] = Decimal.Round(KrizetWeight, 3, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows.Add(NewRow);
            }

            if (FlutesSquare > 0)
            {
                DataRow NewRow = TPSReportTable.NewRow();
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["Name"] = "Стекло Флутес";
                NewRow["Count"] = FlutesSquare;
                NewRow["Measure"] = "м.кв.";
                NewRow["Price"] = FlutesPrice;
                NewRow["Cost"] = FlutesCost;
                NewRow["Weight"] = Decimal.Round(FlutesWeight, 3, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows.Add(NewRow);
            }

        }

        public string GetNbrbBankData(DateTime Date)
        {
            string url = "http://nbrb.by/statistics/Rates/RatesPrint.asp?fromDate=" + Date.ToString("yyyy-M-d");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return "";
            }

            //Encoding encode = System.Text.Encoding.GetEncoding("windows-1251");

            StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());

            string s = myStreamReader.ReadToEnd();

            myStreamReader.Dispose();
            myHttpWebResponse.Close();

            return s;
        }

        public decimal GetEURBYRCurrency(string NbrbBankData)
        {
            string currency = string.Empty;
            decimal Currency;

            try
            {
                int start = NbrbBankData.IndexOf("EUR");


                for (int i = start + 76; i < NbrbBankData.Length; i++)
                {
                    if (NbrbBankData[i] != '<')
                        currency += NbrbBankData[i].ToString();
                    else
                        break;
                }
            }
            catch
            {
                return 0;
            }

            try
            {
                currency = currency.Replace('.', ',');

                Currency = Convert.ToDecimal(currency);
            }
            catch
            {
                return 0;
            }

            return Convert.ToDecimal(currency);
        }

        public void CreateReport(ref HSSFWorkbook hssfworkbook, DateTime DispatchDate, int[] MainOrdersIDs, int CurrencyType)
        {
            ClearReport();

            string Currency = string.Empty;
            decimal Rate = 1;
            if (CurrencyType == 1)
            {
                Currency = "EUR";
            }
            if (CurrencyType == 5)
            {
                string NbrbBankData = GetNbrbBankData(DispatchDate);
                Rate = GetEURBYRCurrency(NbrbBankData);
                Currency = "BYN";
            }

            string MainOrdersList = string.Empty;

            int pos = 0;

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

            CollectGridsAndGlass();
            ConvertToCurrency(Rate, false, CurrencyType);
            ConvertToCurrency(Rate, true, CurrencyType);

            decimal TotalCost = 0;
            decimal TotalProfil = 0;
            decimal TotalTPS = 0;
            decimal WeightProfil = 0;
            decimal WeightTPS = 0;

            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                TotalProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                WeightProfil += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
            }

            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                TotalTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                WeightTPS += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);
            }

            TotalCost = (TotalProfil + TotalTPS);

            TotalProfil = Decimal.Round(TotalProfil, 3, MidpointRounding.AwayFromZero);
            TotalTPS = Decimal.Round(TotalTPS, 3, MidpointRounding.AwayFromZero);
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

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Подробный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int DisplayIndex = 0;
            sheet1.SetColumnWidth(DisplayIndex++, 28 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 13 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 11 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 11 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 11 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 16 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);

            HSSFCell Cell1;
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

            DisplayIndex = 0;
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
                Cell1.SetCellValue("Цена, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость, " + Currency);
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
                    if (CurrencyType == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Price"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Cost"]));
                        Cell1.CellStyle = PriceBelCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Price"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (ProfilReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Cost"]));
                        Cell1.CellStyle = PriceForeignCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ProfilReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    pos++;
                }

                if (CurrencyType == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                    Cell1.SetCellValue(Convert.ToDouble(TotalProfil));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(WeightProfil));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }
            pos++;
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
                Cell1.SetCellValue("Цена, " + Currency);
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Нестандарт, %");
                Cell1.CellStyle = SimpleHeaderCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Стоимость, " + Currency);
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
                    if (CurrencyType == 0)
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Price"]));
                        Cell1.CellStyle = PriceBelCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        Cell1.CellStyle = PriceBelCS;
                    }
                    else
                    {
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Price"]));
                        Cell1.CellStyle = PriceForeignCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        if (TPSReportTable.Rows[i]["NonStandardMargin"] != DBNull.Value)
                        {
                            Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["NonStandardMargin"]));
                        }
                        Cell1.CellStyle = CountCS;
                        Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                        Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Cost"]));
                        Cell1.CellStyle = PriceForeignCS;
                    }
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(TPSReportTable.Rows[i]["Weight"]));
                    Cell1.CellStyle = WeightCS;
                    pos++;
                }

                if (CurrencyType == 0)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderBelCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue("Заказ, " + Currency + ":");
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(5);
                    Cell1.SetCellValue(Convert.ToDouble(TotalTPS));
                    Cell1.CellStyle = SummaryWithoutBorderForeignCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(6);
                    Cell1.SetCellValue(Convert.ToDouble(WeightTPS));
                    Cell1.CellStyle = SummaryWeightCS;
                }
            }
            pos++;
            pos++;

            if (CurrencyType == 0)
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
        }

        public void ClearReport()
        {
            ProfilReportTable.Clear();
            TPSReportTable.Clear();

            FrontsReport.ClearReport();
            DecorReport.ClearReport();
        }
    }


}
