using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infinium
{
    class ClientDecorPrice
    {
        private decimal _rate = 1;
        //HSSFWorkbook hssfworkbook;
        HSSFSheet sheet1;
        HSSFSheet sheet2;
        //string CatalogConnectionString = @"Data Source=v02.bizneshost.by, 32433;Initial Catalog=Catalog;Persist Security Info=True;Connection Timeout=1;User ID=hercules;Password=1q2w3e4r";
        //string CatalogConnectionString = @"Data Source=romanchuk\romanchuk;Initial Catalog=Catalog;Persist Security Info=True;Connection Timeout=1;User ID=sa;Password=1";
        DataTable FrameColorsDataTable = null;
        DataTable DecorDataTable = null;
        DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        DataTable DecorConfigDataTable = null;
        DataTable ProductsDataTable = null;
        DataTable ResultDecorDataTable = null;
        DataTable ExcluziveTable = null;
        DataTable NotExcluziveTable = null;
        public ClientDecorPrice(decimal rate)
        {
            _rate = rate;
            CreateTables();
            string SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
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
            GetColorsDT();
        }

        private void CreateTables()
        {
            ProductsDataTable = new System.Data.DataTable();
            DecorDataTable = new System.Data.DataTable();
            DecorConfigDataTable = new System.Data.DataTable();
            PatinaDataTable = new System.Data.DataTable();
            ResultDecorDataTable = new DataTable();
            ResultDecorDataTable.Columns.Add(new DataColumn(("NoPrint"), System.Type.GetType("System.Boolean")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Length"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("CoverType"), System.Type.GetType("System.Int32")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("ProductName"), System.Type.GetType("System.String")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Name"), System.Type.GetType("System.String")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Color"), System.Type.GetType("System.String")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost0"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost1"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost2"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost3"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost4"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost5"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost6"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost7"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost8"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost9"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("Cost10"), System.Type.GetType("System.Decimal")));
            ResultDecorDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));
            ExcluziveTable = new DataTable();
            ExcluziveTable.Columns.Add(new DataColumn(("Index"), System.Type.GetType("System.Int32")));
            ExcluziveTable.Columns.Add(new DataColumn(("ProductName"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Name"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Color"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost0"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost1"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost2"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost3"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost4"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost5"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost6"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost7"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost8"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost9"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost10"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("ItemGroup"), System.Type.GetType("System.Boolean")));
            ExcluziveTable.Columns.Add(new DataColumn(("CostGroup"), System.Type.GetType("System.Boolean")));
            ExcluziveTable.Columns.Add(new DataColumn(("CoverType"), System.Type.GetType("System.Int32")));
            NotExcluziveTable = ExcluziveTable.Clone();
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors WHERE GroupID IN (2,3,4,5,6,21,22)", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow[] rows = FrameColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = FrameColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            FrameColorsDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
        }

        //Без цвета
        public void Color0(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "DecorConfig.DecorConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "DecorConfig.DecorConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1 ) ";
            string SelectCommand = @"SELECT ProductID, DecorID, ColorID, PatinaID, Measures.Measure, Length, Height, Width, 
                MarketingCost, PriceRatio, DigitCapacity FROM DecorConfig 
                INNER JOIN Measures ON DecorConfig.MeasureID=Measures.MeasureID
                WHERE (ColorID = -1 OR ColorID IN (SELECT InsetColorID FROM InsetColors WHERE GroupID IN (4))) AND DecorID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY ProductID,DecorID,ColorID,PatinaID,Length,Height,Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDataTable.Clear();
                DA.Fill(DecorConfigDataTable);
                if (!DecorConfigDataTable.Columns.Contains("OriginalPrice"))
                    DecorConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));
            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < DecorConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(DecorConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                DecorConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }
            DataTable DT1 = new DataTable();

            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = -1;
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 0;
                NewRow["Cost0"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
        }

        //Кронинг
        public void Color59(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "DecorConfig.DecorConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "DecorConfig.DecorConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1 ) ";
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductID, DecorID, ColorID, PatinaID, Measures.Measure, Length, Height, Width,
                MarketingCost, PriceRatio, DigitCapacity FROM DecorConfig 
                INNER JOIN Measures ON DecorConfig.MeasureID=Measures.MeasureID
                WHERE ColorID IN (SELECT TechStoreID FROM TechStore WHERE TechStoreSubGroupID=59) AND DecorID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY ProductID,DecorID,ColorID,PatinaID,Length,Height,Width", ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDataTable.Clear();
                DA.Fill(DecorConfigDataTable);
                if (!DecorConfigDataTable.Columns.Contains("OriginalPrice"))
                    DecorConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));

            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < DecorConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(DecorConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                DecorConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }
            DataTable DT1 = new DataTable();

            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = -1;
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 1;
                NewRow["Cost1"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
        }

        //Бумага ПСС
        public void Color60(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "DecorConfig.DecorConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "DecorConfig.DecorConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1 ) ";
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductID, DecorID, ColorID, PatinaID, Measures.Measure, Length, Height, Width,
                MarketingCost, PriceRatio, DigitCapacity FROM DecorConfig 
                INNER JOIN Measures ON DecorConfig.MeasureID=Measures.MeasureID
                WHERE ColorID IN (SELECT TechStoreID FROM TechStore WHERE TechStoreSubGroupID=60) AND DecorID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY ProductID,DecorID,ColorID,PatinaID,Length,Height,Width", ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDataTable.Clear();
                DA.Fill(DecorConfigDataTable);
                if (!DecorConfigDataTable.Columns.Contains("OriginalPrice"))
                    DecorConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));

            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < DecorConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(DecorConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                DecorConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID=-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow[] rows = ResultDecorDataTable.Select("ProductID=" + Convert.ToInt32(DT1.Rows[i]["ProductID"]) + " AND DecorID=" + Convert.ToInt32(DT1.Rows[i]["DecorID"]) + " AND ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                if (rows.Count() == 0)
                {
                    DataRow NewRow = ResultDecorDataTable.NewRow();
                    NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                    NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                    NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                    NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                    NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                    NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = -1;
                    NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                    NewRow["Length"] = -1;
                    NewRow["Height"] = -1;
                    NewRow["Width"] = -1;
                    NewRow["CoverType"] = 2;
                    NewRow["Cost2"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                    ResultDecorDataTable.Rows.Add(NewRow);
                }
                else
                {
                    rows[0]["Cost2"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                }
            }
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID<>-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "PatinaID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow[] rows = ResultDecorDataTable.Select("ProductID=" + Convert.ToInt32(DT1.Rows[i]["ProductID"]) + " AND DecorID=" + Convert.ToInt32(DT1.Rows[i]["DecorID"]) + " AND ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                if (rows.Count() == 0)
                {
                    DataRow NewRow = ResultDecorDataTable.NewRow();
                    NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                    NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                    NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"])) + " " + GetPatinaName(Convert.ToInt32(DT1.Rows[i]["PatinaID"]));
                    NewRow["Patina"] = "патина";
                    NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                    NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                    NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(DT1.Rows[i]["PatinaID"]);
                    NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                    NewRow["Length"] = -1;
                    NewRow["Height"] = -1;
                    NewRow["Width"] = -1;
                    NewRow["CoverType"] = 3;
                    NewRow["Cost3"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                    ResultDecorDataTable.Rows.Add(NewRow);
                }
                else
                {
                    rows[0]["Cost3"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                }
            }
        }

        //ПВХ
        public void Color61(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "DecorConfig.DecorConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "DecorConfig.DecorConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1 ) ";
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductID, DecorID, ColorID, PatinaID, Measures.Measure, Length, Height, Width,
                MarketingCost, PriceRatio, DigitCapacity FROM DecorConfig 
                INNER JOIN Measures ON DecorConfig.MeasureID=Measures.MeasureID
                WHERE ColorID IN (SELECT TechStoreID FROM TechStore WHERE TechStoreSubGroupID=61) AND DecorID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY ProductID,DecorID,ColorID,PatinaID,Length,Height,Width", ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDataTable.Clear();
                DA.Fill(DecorConfigDataTable);
                if (!DecorConfigDataTable.Columns.Contains("OriginalPrice"))
                    DecorConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));

            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < DecorConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(DecorConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                DecorConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID=-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = -1;
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 4;
                NewRow["Cost4"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID<>-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "PatinaID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"])) + " " + GetPatinaName(Convert.ToInt32(DT1.Rows[i]["PatinaID"]));
                NewRow["Patina"] = "патина";
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = Convert.ToInt32(DT1.Rows[i]["PatinaID"]);
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 5;
                NewRow["Cost5"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
        }

        //ШПОН
        public void Color62(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "DecorConfig.DecorConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "DecorConfig.DecorConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1 ) ";
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductID, DecorID, ColorID, PatinaID, Measures.Measure, Length, Height, Width, 
                MarketingCost, PriceRatio, DigitCapacity FROM DecorConfig 
                INNER JOIN Measures ON DecorConfig.MeasureID=Measures.MeasureID
                WHERE ColorID IN (SELECT TechStoreID FROM TechStore WHERE TechStoreSubGroupID=62) AND DecorID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY ProductID,DecorID,ColorID,PatinaID,Length,Height,Width", ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDataTable.Clear();
                DA.Fill(DecorConfigDataTable);
                if (!DecorConfigDataTable.Columns.Contains("OriginalPrice"))
                    DecorConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));

            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < DecorConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(DecorConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                DecorConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }
            DataTable DT1 = new DataTable();

            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = -1;
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 6;
                NewRow["Cost6"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
        }

        //ПП
        public void Color63(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "DecorConfig.DecorConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "DecorConfig.DecorConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1 ) ";
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductID, DecorID, ColorID, PatinaID, Measures.Measure, Length, Height, Width, 
                MarketingCost, PriceRatio, DigitCapacity FROM DecorConfig 
                INNER JOIN Measures ON DecorConfig.MeasureID=Measures.MeasureID
                WHERE ColorID IN (SELECT TechStoreID FROM TechStore WHERE TechStoreSubGroupID=63) AND DecorID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY ProductID,DecorID,ColorID,PatinaID,Length,Height,Width", ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDataTable.Clear();
                DA.Fill(DecorConfigDataTable);
                if (!DecorConfigDataTable.Columns.Contains("OriginalPrice"))
                    DecorConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));

            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < DecorConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(DecorConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                DecorConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID=-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = -1;
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 7;
                NewRow["Cost7"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID<>-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "PatinaID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"])) + " " + GetPatinaName(Convert.ToInt32(DT1.Rows[i]["PatinaID"]));
                NewRow["Patina"] = "патина";
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = Convert.ToInt32(DT1.Rows[i]["PatinaID"]);
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 8;
                NewRow["Cost8"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
        }

        //ЭК
        public void Color307(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "DecorConfig.DecorConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "DecorConfig.DecorConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 1 ) ";
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductID, DecorID, ColorID, PatinaID, Measures.Measure, Length, Height, Width, 
                MarketingCost, PriceRatio, DigitCapacity FROM DecorConfig 
                INNER JOIN Measures ON DecorConfig.MeasureID=Measures.MeasureID
                WHERE ColorID IN (SELECT TechStoreID FROM TechStore WHERE TechStoreSubGroupID=308) AND DecorID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY ProductID,DecorID,ColorID,PatinaID,Length,Height,Width", ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDataTable.Clear();
                DA.Fill(DecorConfigDataTable);
                if (!DecorConfigDataTable.Columns.Contains("OriginalPrice"))
                    DecorConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));

            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < DecorConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(DecorConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(DecorConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                DecorConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID=-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = -1;
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 9;
                NewRow["Cost9"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
            using (DataView DV = new DataView(DecorConfigDataTable))
            {
                DV.RowFilter = "PatinaID<>-1";
                DT1 = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "PatinaID", "Measure", "OriginalPrice" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = ResultDecorDataTable.NewRow();
                NewRow["ProductName"] = GetProductName(Convert.ToInt32(DT1.Rows[i]["ProductID"]));
                NewRow["Name"] = GetDecorName(Convert.ToInt32(DT1.Rows[i]["DecorID"]));
                NewRow["Color"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"])) + " " + GetPatinaName(Convert.ToInt32(DT1.Rows[i]["PatinaID"]));
                NewRow["Patina"] = "патина";
                NewRow["ProductID"] = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                NewRow["PatinaID"] = Convert.ToInt32(DT1.Rows[i]["PatinaID"]);
                NewRow["Measure"] = DT1.Rows[i]["Measure"].ToString();
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["CoverType"] = 10;
                NewRow["Cost10"] = Convert.ToDecimal(DT1.Rows[i]["OriginalPrice"]);
                ResultDecorDataTable.Rows.Add(NewRow);
            }
        }

        private string GetProductName(int ProductID)
        {
            string name = string.Empty;
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            if (Rows.Count() > 0)
                name = Rows[0]["ProductName"].ToString();
            return name;
        }

        public string GetDecorName(int DecorID)
        {
            string name = string.Empty;
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            if (Rows.Count() > 0)
                name = Rows[0]["Name"].ToString();
            return name;
        }

        public string GetColorName(int ColorID)
        {
            string name = string.Empty;
            DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
            if (Rows.Count() > 0)
                name = Rows[0]["ColorName"].ToString();
            return name;
        }

        public string GetPatinaName(int PatinaID)
        {
            string name = string.Empty;
            DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (Rows.Count() > 0)
                name = Rows[0]["PatinaName"].ToString();
            return name;
        }

        private void FillReportTable(ref DataTable TempTable)
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(ResultDecorDataTable.Copy()))
            {
                DV.Sort = "ProductName, Name, CoverType, Cost0, Cost1, Cost2, Cost3, Cost4, Cost5, Cost6, Cost7, Cost8, Cost9, Cost10, Color";
                ResultDecorDataTable.Clear();
                ResultDecorDataTable = DV.ToTable();
            }
            int Index = 0;

            for (int i = 0; i < ResultDecorDataTable.Rows.Count; i++)
            {
                if (ResultDecorDataTable.Rows[i]["NoPrint"] != DBNull.Value)
                    continue;
                int CurrentDecorID = Convert.ToInt32(ResultDecorDataTable.Rows[i]["DecorID"]);

                int CoverType = Convert.ToInt32(ResultDecorDataTable.Rows[i]["CoverType"]);
                string Cost = Convert.ToDecimal(ResultDecorDataTable.Rows[i]["Cost" + CoverType]).ToString();
                string Name = ResultDecorDataTable.Rows[i]["Name"].ToString();

                DataRow[] rows = TempTable.Select("ProductName='" + ResultDecorDataTable.Rows[i]["ProductName"].ToString() + "'");
                if (rows.Count() == 0)
                {
                    DataRow NewRow = TempTable.NewRow();
                    NewRow["ProductName"] = ResultDecorDataTable.Rows[i]["ProductName"].ToString();
                    NewRow["Name"] = ResultDecorDataTable.Rows[i]["Name"].ToString();
                    NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                    NewRow["ItemGroup"] = true;
                    NewRow["CoverType"] = CoverType;
                    NewRow["Index"] = Index++;
                    TempTable.Rows.Add(NewRow);
                }
                else
                {
                    rows = TempTable.Select("Name='" + Name + "'");
                    if (rows.Count() == 0)
                    {
                        DataRow NewRow = TempTable.NewRow();
                        NewRow["Name"] = ResultDecorDataTable.Rows[i]["Name"].ToString();
                        NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                        //NewRow["ItemGroup"] = true;
                        NewRow["Index"] = Index++;
                        TempTable.Rows.Add(NewRow);
                    }
                }

                if (i == 0)
                {
                    using (DataView DV = new DataView(ResultDecorDataTable))
                    {
                        DV.RowFilter = "Name='" + Name + "' AND CoverType=" + CoverType;
                        DT1 = DV.ToTable(true, new string[] { "Cost" + CoverType });
                    }
                    //if (DT1.Rows.Count == 1)
                    //{
                    //    rows = TempTable.Select("Name='" + Name + "'");
                    //    if (rows.Count() > 0)
                    //    {
                    //        TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                    //        DecorID = CurrentDecorID;
                    //        rows = ResultDecorDataTable.Select("Name='" + Name + "'" + " AND CoverType=" + CoverType);
                    //        if (rows.Count() > 0)
                    //        {
                    //            foreach (DataRow item in rows)
                    //            {
                    //                item["NoPrint"] = true;
                    //            }
                    //            continue;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    {
                        DataRow NewRow = TempTable.NewRow();
                        NewRow["Color"] = string.Empty;
                        NewRow["Patina"] = string.Empty;
                        NewRow["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                        NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                        NewRow["CostGroup"] = true;
                        NewRow["CoverType"] = CoverType;
                        NewRow["Index"] = Index++;
                        TempTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = TempTable.NewRow();
                        NewRow["Color"] = ResultDecorDataTable.Rows[i]["Color"].ToString();
                        NewRow["Patina"] = ResultDecorDataTable.Rows[i]["Patina"].ToString();
                        NewRow["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                        NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                        NewRow["CoverType"] = CoverType;
                        NewRow["Index"] = Index++;
                        TempTable.Rows.Add(NewRow);
                    }
                    //}
                    continue;
                }

                if (Name != ResultDecorDataTable.Rows[i - 1]["Name"].ToString())
                {
                    using (DataView DV = new DataView(ResultDecorDataTable))
                    {
                        DV.RowFilter = "Name='" + Name + "' AND CoverType=" + CoverType;
                        DT1 = DV.ToTable(true, new string[] { "Cost" + CoverType });
                    }
                    //if (DT1.Rows.Count == 1)
                    //{
                    //    rows = TempTable.Select("Name='" + Name + "'");
                    //    if (rows.Count() > 0)
                    //    {
                    //        TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                    //        DecorID = CurrentDecorID;
                    //        rows = ResultDecorDataTable.Select("Name='" + Name + "'" + " AND CoverType=" + CoverType);
                    //        if (rows.Count() > 0)
                    //        {
                    //            foreach (DataRow item in rows)
                    //            {
                    //                item["NoPrint"] = true;
                    //            }
                    //            continue;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    {
                        DataRow NewRow = TempTable.NewRow();
                        NewRow["Color"] = string.Empty;
                        NewRow["Patina"] = string.Empty;
                        NewRow["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                        NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                        NewRow["CostGroup"] = true;
                        NewRow["CoverType"] = CoverType;
                        NewRow["Index"] = Index++;
                        TempTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = TempTable.NewRow();
                        NewRow["Color"] = ResultDecorDataTable.Rows[i]["Color"].ToString();
                        NewRow["Patina"] = ResultDecorDataTable.Rows[i]["Patina"].ToString();
                        NewRow["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                        NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                        NewRow["CoverType"] = CoverType;
                        NewRow["Index"] = Index++;
                        TempTable.Rows.Add(NewRow);
                    }
                    //}
                    continue;
                }

                if (Convert.ToInt32(ResultDecorDataTable.Rows[i]["CoverType"]) != Convert.ToInt32(ResultDecorDataTable.Rows[i - 1]["CoverType"]) ||
                    Convert.ToDecimal(ResultDecorDataTable.Rows[i]["Cost" + CoverType]) != Convert.ToDecimal(ResultDecorDataTable.Rows[i - 1]["Cost" + CoverType]))
                {
                    using (DataView DV = new DataView(ResultDecorDataTable))
                    {
                        DV.RowFilter = "Name='" + Name + "' AND CoverType=" + CoverType;
                        DT1 = DV.ToTable(true, new string[] { "Cost" + CoverType });
                    }
                    //if (DT1.Rows.Count == 1)
                    //{
                    //    rows = TempTable.Select("Name='" + Name + "'");
                    //    if (rows.Count() > 0)
                    //    {
                    //        TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                    //        DecorID = CurrentDecorID;
                    //        rows = ResultDecorDataTable.Select("Name='" + Name + "'" + " AND CoverType=" + CoverType);
                    //        if (rows.Count() > 0)
                    //        {
                    //            foreach (DataRow item in rows)
                    //            {
                    //                item["NoPrint"] = true;
                    //            }
                    //            continue;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        {
                    //            DataRow NewRow = TempTable.NewRow();
                    //            //NewRow["ProductName"] = ResultDecorDataTable.Rows[i]["ProductName"].ToString();
                    //            NewRow["Name"] = ResultDecorDataTable.Rows[i]["Name"].ToString();
                    //            NewRow["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                    //            NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                    //            //NewRow["ItemGroup"] = true;
                    //            NewRow["Index"] = Index++;
                    //            TempTable.Rows.Add(NewRow);
                    //        }
                    //    }
                    //}
                    //else
                    {
                        DataRow NewRow = TempTable.NewRow();
                        NewRow["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                        NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                        NewRow["CostGroup"] = true;
                        NewRow["CoverType"] = CoverType;
                        NewRow["Index"] = Index++;
                        TempTable.Rows.Add(NewRow);
                    }
                }
                if (Name == ResultDecorDataTable.Rows[i - 1]["Name"].ToString() ||
                    Convert.ToInt32(ResultDecorDataTable.Rows[i]["CoverType"]) == Convert.ToInt32(ResultDecorDataTable.Rows[i - 1]["CoverType"]) ||
                    (ResultDecorDataTable.Rows[i - 1]["Cost" + CoverType] == DBNull.Value || Convert.ToDecimal(ResultDecorDataTable.Rows[i]["Cost" + CoverType]) == Convert.ToDecimal(ResultDecorDataTable.Rows[i - 1]["Cost" + CoverType])))
                {
                    using (DataView DV = new DataView(ResultDecorDataTable))
                    {
                        DV.RowFilter = "Name='" + Name + "' AND CoverType=" + CoverType;
                        DT1 = DV.ToTable(true, new string[] { "Cost" + CoverType });
                    }
                    //if (DT1.Rows.Count == 1)
                    //{
                    //    rows = TempTable.Select("Name='" + Name + "'");
                    //    if (rows.Count() > 0)
                    //    {
                    //        TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                    //        DecorID = CurrentDecorID;
                    //        rows = ResultDecorDataTable.Select("Name='" + Name + "'" + " AND CoverType=" + CoverType);
                    //        if (rows.Count() > 0)
                    //        {
                    //            foreach (DataRow item in rows)
                    //            {
                    //                item["NoPrint"] = true;
                    //            }
                    //            continue;
                    //        }
                    //    }
                    //}
                    DataRow NewRow = TempTable.NewRow();
                    NewRow["Name"] = string.Empty;
                    NewRow["Color"] = ResultDecorDataTable.Rows[i]["Color"].ToString();
                    NewRow["Patina"] = ResultDecorDataTable.Rows[i]["Patina"].ToString();
                    NewRow["Cost" + CoverType] = ResultDecorDataTable.Rows[i]["Cost" + CoverType].ToString();
                    NewRow["Measure"] = ResultDecorDataTable.Rows[i]["Measure"].ToString();
                    NewRow["CoverType"] = CoverType;
                    NewRow["Index"] = Index++;
                    TempTable.Rows.Add(NewRow);
                }
            }
        }

        public void CreateReport(ref HSSFWorkbook hssfworkbook, int ClientID, decimal PriceGroup)
        {
            bool HasExcluzive = false;
            ResultDecorDataTable.Clear();
            {
                Color0(ClientID, PriceGroup, true);
                Color59(ClientID, PriceGroup, true);
                Color60(ClientID, PriceGroup, true);
                Color61(ClientID, PriceGroup, true);
                Color62(ClientID, PriceGroup, true);
                Color63(ClientID, PriceGroup, true);
                Color307(ClientID, PriceGroup, true);
                FillReportTable(ref ExcluziveTable);
            }
            if (ExcluziveTable.Rows.Count > 0)
            {
                HasExcluzive = true;
                sheet1 = hssfworkbook.CreateSheet("Эксклюзив декор");
                sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
                sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
                sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
                sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

                int DisplayIndex = 0;
                int RowIndex = 1;
                sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 25 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                //sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 11 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);

                HSSFFont HeaderF = hssfworkbook.CreateFont();
                HeaderF.FontHeightInPoints = 11;
                HeaderF.Boldweight = 11 * 256;
                HeaderF.FontName = "Calibri";
                HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
                HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
                HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
                HeaderStyle.SetFont(HeaderF);
                HSSFCell cell;
                DisplayIndex = 0;
                HSSFRow r = sheet1.CreateRow(RowIndex);
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Продукт");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Наименование");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Цвет");
                cell.CellStyle = HeaderStyle;
                //cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Патина");
                //cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Ед.изм.");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Сырой");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Kroning");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Бумага");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Бумага Лак");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПВХ");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПВХ Лак");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Шпон");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПП");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПП Лак");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ЭК");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ЭК Лак");
                cell.CellStyle = HeaderStyle;
                RowIndex++;
                sheet1.CreateFreezePane(13, 2, 13, 2);

                ReportToExcel(ExcluziveTable, ref hssfworkbook, ref sheet1, ref RowIndex);
                RowIndex++;
            }
            ResultDecorDataTable.Clear();
            {
                Color0(ClientID, PriceGroup, false);
                Color59(ClientID, PriceGroup, false);
                Color60(ClientID, PriceGroup, false);
                Color61(ClientID, PriceGroup, false);
                Color62(ClientID, PriceGroup, false);
                Color63(ClientID, PriceGroup, false);
                Color307(ClientID, PriceGroup, false);
                FillReportTable(ref NotExcluziveTable);
            }
            if (NotExcluziveTable.Rows.Count > 0)
            {
                if (HasExcluzive)
                    sheet2 = hssfworkbook.CreateSheet("Не эксклюзив декор");
                else
                    sheet2 = hssfworkbook.CreateSheet("Прайс декор");
                sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
                sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
                sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
                sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

                int DisplayIndex = 0;
                int RowIndex = 1;
                sheet2.SetColumnWidth(DisplayIndex++, 15 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 25 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 20 * 256);
                //sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 11 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);

                HSSFFont HeaderF = hssfworkbook.CreateFont();
                HeaderF.FontHeightInPoints = 11;
                HeaderF.Boldweight = 11 * 256;
                HeaderF.FontName = "Calibri";
                HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
                HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
                HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
                HeaderStyle.SetFont(HeaderF);
                HSSFCell cell;
                DisplayIndex = 0;
                HSSFRow r = sheet2.CreateRow(RowIndex);
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Продукт");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Наименование");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Цвет");
                cell.CellStyle = HeaderStyle;
                //cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Патина");
                //cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Ед.изм.");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Сырой");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Kroning");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Бумага");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Бумага Лак");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПВХ");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПВХ Лак");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Шпон");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПП");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ПП Лак");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ЭК");
                cell.CellStyle = HeaderStyle;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ЭК Лак");
                cell.CellStyle = HeaderStyle;
                RowIndex++;
                sheet2.CreateFreezePane(13, 2, 13, 2);

                ReportToExcel(NotExcluziveTable, ref hssfworkbook, ref sheet2, ref RowIndex);
                RowIndex++;
            }
        }

        public void ReportToExcel(DataTable TempTable, ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1, ref int RowIndex)
        {
            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 10;
            SimpleF.FontName = "Calibri";
            HSSFCellStyle ItemCS = hssfworkbook.CreateCellStyle();
            ItemCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            ItemCS.SetFont(SimpleF);
            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.SetFont(SimpleF);
            HSSFCellStyle CostCS = hssfworkbook.CreateCellStyle();
            CostCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CostCS.SetFont(SimpleF);

            HSSFCellStyle ItemBorderLeftCS = hssfworkbook.CreateCellStyle();
            ItemBorderLeftCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            ItemBorderLeftCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            ItemBorderLeftCS.LeftBorderColor = HSSFColor.BLACK.index;
            ItemBorderLeftCS.SetFont(SimpleF);
            HSSFCellStyle ItemBorderBottomCS = hssfworkbook.CreateCellStyle();
            ItemBorderBottomCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            ItemBorderBottomCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ItemBorderBottomCS.BottomBorderColor = HSSFColor.BLACK.index;
            ItemBorderBottomCS.SetFont(SimpleF);
            HSSFCellStyle ItemBorderLeftBottomCS = hssfworkbook.CreateCellStyle();
            ItemBorderLeftBottomCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            ItemBorderLeftBottomCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ItemBorderLeftBottomCS.BottomBorderColor = HSSFColor.BLACK.index;
            ItemBorderLeftBottomCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            ItemBorderLeftBottomCS.LeftBorderColor = HSSFColor.BLACK.index;
            ItemBorderLeftBottomCS.SetFont(SimpleF);
            HSSFCellStyle SimpleBorderBottomCS = hssfworkbook.CreateCellStyle();
            SimpleBorderBottomCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleBorderBottomCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleBorderBottomCS.SetFont(SimpleF);
            HSSFCellStyle CostBorderRightCS = hssfworkbook.CreateCellStyle();
            CostBorderRightCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CostBorderRightCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            CostBorderRightCS.RightBorderColor = HSSFColor.BLACK.index;
            CostBorderRightCS.SetFont(SimpleF);
            HSSFCellStyle CostBorderBottomCS = hssfworkbook.CreateCellStyle();
            CostBorderBottomCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CostBorderBottomCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            CostBorderBottomCS.BottomBorderColor = HSSFColor.BLACK.index;
            CostBorderBottomCS.SetFont(SimpleF);
            HSSFCellStyle CostBorderRightBottomCS = hssfworkbook.CreateCellStyle();
            CostBorderRightBottomCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CostBorderRightBottomCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            CostBorderRightBottomCS.BottomBorderColor = HSSFColor.BLACK.index;
            CostBorderRightBottomCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            CostBorderRightBottomCS.RightBorderColor = HSSFColor.BLACK.index;
            CostBorderRightBottomCS.SetFont(SimpleF);

            bool NeedBorder = false;
            int DisplayIndex = 0;
            HSSFCell cell;
            for (int x = 0; x < TempTable.Rows.Count; x++)
            {
                NeedBorder = false;
                if (x != TempTable.Rows.Count - 1)
                {
                    if (TempTable.Rows[x + 1]["ProductName"].ToString().Length > 0)
                        NeedBorder = true;
                }
                else
                    NeedBorder = true;
                HSSFRow r = sheet1.CreateRow(RowIndex);
                DisplayIndex = 0;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["ProductName"].ToString());
                if (NeedBorder)
                    cell.CellStyle = ItemBorderLeftBottomCS;
                else
                    cell.CellStyle = ItemBorderLeftCS;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["Name"].ToString());
                if (NeedBorder)
                    cell.CellStyle = ItemBorderBottomCS;
                else
                    cell.CellStyle = ItemCS;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["Color"].ToString());
                if (NeedBorder)
                    cell.CellStyle = SimpleBorderBottomCS;
                else
                    cell.CellStyle = SimpleCS;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["Measure"].ToString());
                if (NeedBorder)
                    cell.CellStyle = SimpleBorderBottomCS;
                else
                    cell.CellStyle = SimpleCS;

                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost0"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost0"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost1"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost1"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost2"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost2"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost3"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost3"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost4"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost4"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost5"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost5"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost6"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost6"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost7"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost7"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost8"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost8"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost9"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost9"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderBottomCS;
                else
                    cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost10"] != DBNull.Value)
                {
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost10"]));
                }
                if (NeedBorder)
                    cell.CellStyle = CostBorderRightBottomCS;
                else
                    cell.CellStyle = CostBorderRightCS;
                RowIndex++;
            }

            RowIndex++;

            HSSFRow g = sheet1.CreateRow(RowIndex);
            cell = HSSFCellUtil.CreateCell(g, 0, "* - для продукта ПлК-110 ***х146 добавляется 50 евро за шт.");
            cell.CellStyle = CostBorderRightCS;

            int firstRowGroupIndex = 0;
            int secondRowGroupIndex = 0;
            int firstRowMergeIndex = 0;
            int secondRowMergeIndex = 0;
            bool NeedGroup = false;
            int CoverType = 0;
            string Name = string.Empty;
            for (int x = 0; x < TempTable.Rows.Count; x++)
            {
                string CurrentName = TempTable.Rows[x]["Name"].ToString();
                if (x == TempTable.Rows.Count - 1)
                {
                    continue;
                }
                if (firstRowMergeIndex == 0)
                {
                    if (TempTable.Rows[x]["Name"].ToString().Length == 0)
                    {
                        firstRowMergeIndex = x;
                    }
                }
                else
                {
                    if (TempTable.Rows[x + 1]["Name"].ToString().Length > 0)
                    {
                        secondRowMergeIndex = x;
                    }
                }
                if (TempTable.Rows[x]["CostGroup"] != DBNull.Value && firstRowGroupIndex == 0)
                {
                    firstRowGroupIndex = x + 1;
                    CoverType = Convert.ToInt32(TempTable.Rows[x]["CoverType"]);
                }
                int Index = Convert.ToInt32(TempTable.Rows[x]["Index"]);
                if (!NeedGroup)
                {
                    if (firstRowGroupIndex == 0)
                    {
                        if (TempTable.Rows[x]["CostGroup"] != DBNull.Value && Convert.ToBoolean(TempTable.Rows[x]["CostGroup"]))
                        {
                            firstRowGroupIndex = x + 1;
                            continue;
                        }
                    }
                    else
                    {
                        if (TempTable.Rows[x + 1]["Cost" + CoverType] == DBNull.Value || TempTable.Rows[x + 1]["CoverType"] == DBNull.Value ||
                            (TempTable.Rows[x]["Cost" + CoverType] != DBNull.Value && TempTable.Rows[x + 1]["Cost" + CoverType] != DBNull.Value && Convert.ToDecimal(TempTable.Rows[x]["Cost" + CoverType]) != Convert.ToDecimal(TempTable.Rows[x + 1]["Cost" + CoverType])))
                        {
                            secondRowGroupIndex = x;
                            NeedGroup = true;
                        }
                    }
                }
                if (NeedGroup)
                {
                    if (secondRowMergeIndex != 0)
                    {
                        sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(firstRowMergeIndex + 1, 1, secondRowMergeIndex + 2, 1));
                        firstRowMergeIndex = 0;
                        secondRowMergeIndex = 0;
                    }
                    NeedGroup = false;
                    sheet1.GroupRow(firstRowGroupIndex + 2, secondRowGroupIndex + 2);
                    sheet1.SetRowGroupCollapsed(firstRowGroupIndex + 2, true);
                    firstRowGroupIndex = 0;
                    secondRowGroupIndex = 0;
                }
            }
            firstRowGroupIndex = 0;
            secondRowGroupIndex = 0;
            firstRowMergeIndex = -1;
            secondRowMergeIndex = 0;
            NeedGroup = false;
            CoverType = 0;
            Name = string.Empty;
            for (int x = 0; x < TempTable.Rows.Count; x++)
            {
                string CurrentName = TempTable.Rows[x]["ProductName"].ToString();
                if (x == TempTable.Rows.Count - 1)
                {
                    if (firstRowMergeIndex != 0)
                    {
                        secondRowMergeIndex = x;
                        sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(firstRowMergeIndex + 2, 0, secondRowMergeIndex + 2, 0));
                        firstRowMergeIndex = 0;
                        secondRowMergeIndex = 0;
                        continue;
                    }
                }
                if (firstRowMergeIndex == -1)
                {
                    if (TempTable.Rows[x]["ProductName"].ToString().Length > 0)
                    {
                        firstRowMergeIndex = x;
                    }
                }
                else
                {
                    if (x == TempTable.Rows.Count - 1)
                    {
                        if (TempTable.Rows[x]["ProductName"].ToString().Length > 0)
                            secondRowMergeIndex = x;
                    }
                    else
                    {
                        if (TempTable.Rows[x + 1]["ProductName"].ToString().Length > 0)
                            secondRowMergeIndex = x;
                    }
                }
                if (secondRowMergeIndex != 0)
                {
                    sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(firstRowMergeIndex + 2, 0, secondRowMergeIndex + 2, 0));
                    firstRowMergeIndex = -1;
                    secondRowMergeIndex = 0;
                }
            }
            RowIndex++;
            RowIndex++;
        }

    }
}
