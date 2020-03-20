using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infinium
{
    class ClientFrontsPrice : IFirstProfilName
    {
        private decimal _rate = 1;

        HSSFSheet sheet1;
        HSSFSheet sheet2;
        bool HasPlanka = false;
        //string CatalogConnectionString = @"Data Source=v02.bizneshost.by, 32433;Initial Catalog=Catalog;Persist Security Info=True;Connection Timeout=1;User ID=hercules;Password=1q2w3e4r";
        //string CatalogConnectionString = @"Data Source=romanchuk\romanchuk;Initial Catalog=Catalog;Persist Security Info=True;Connection Timeout=1;User ID=sa;Password=1";
        DataTable FrontsConfigDataTable = null;
        DataTable FrontsDataTable = null;
        DataTable FrameColorsDataTable = null;
        DataTable TechStoreDataTable = null;
        DataTable InsetTypesDataTable = null;
        DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        DataTable ResultFrontsDataTable = null;
        DataTable ExcluziveTable = null;
        DataTable NotExcluziveTable = null;
        public ClientFrontsPrice(decimal rate)
        {
            _rate = rate;
            CreateTables();
            string SelectCommand = @"SELECT DISTINCT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
                FrontsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }

            GetColorsDT();
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
            SelectCommand = @"SELECT TechStore.TechStoreID, TechStore.TechStoreSubGroupID FROM TechStore 
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID = TechStoreSubGroups.TechStoreSubGroupID AND TechStoreSubGroups.TechStoreGroupID = 11";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDataTable);
            }
        }

        private void CreateTables()
        {
            FrontsDataTable = new System.Data.DataTable();
            PatinaDataTable = new System.Data.DataTable();
            FrontsConfigDataTable = new System.Data.DataTable();
            TechStoreDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            ResultFrontsDataTable = new DataTable();
            ResultFrontsDataTable.Columns.Add(new DataColumn(("NoPrint"), System.Type.GetType("System.Boolean")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("ColorType"), System.Type.GetType("System.Int32")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("FrontName"), System.Type.GetType("System.String")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Color"), System.Type.GetType("System.String")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("ColorGroup"), System.Type.GetType("System.Boolean")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost0"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost1"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost2"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost3"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost4"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost5"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost6"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost7"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost8"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("Cost9"), System.Type.GetType("System.Decimal")));
            ResultFrontsDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));
            ExcluziveTable = new DataTable();
            ExcluziveTable.Columns.Add(new DataColumn(("Index"), System.Type.GetType("System.Int32")));
            ExcluziveTable.Columns.Add(new DataColumn(("FrontName"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Color"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost1"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost2"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost3"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost4"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost5"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost6"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost7"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost8"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("Cost9"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            ExcluziveTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            ExcluziveTable.Columns.Add(new DataColumn(("ItemGroup"), System.Type.GetType("System.Boolean")));
            ExcluziveTable.Columns.Add(new DataColumn(("CostGroup"), System.Type.GetType("System.Boolean")));
            ExcluziveTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            ExcluziveTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.Int32")));
            ExcluziveTable.Columns.Add(new DataColumn(("ColorType"), System.Type.GetType("System.String")));
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
        }

        public void FillFrontConfigTable(int ClientID, decimal PriceGroup, bool bExcluzive)
        {
            string ExcluziveFilter = "FrontsConfig.FrontConfigID IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 0  AND ClientID = " + ClientID + ") ";
            if (!bExcluzive)
                ExcluziveFilter = "FrontsConfig.FrontConfigID NOT IN (SELECT ConfigID FROM infiniu2_marketingreference.dbo.ExcluziveCatalog WHERE ProductType = 0 ) ";
            string SelectCommand = @"SELECT TechStore.TechStoreSubGroupID, FrontsConfig.FrontConfigID, FrontsConfig.FrontID, FrontsConfig.ProfileID, FrontsConfig.ColorID, FrontsConfig.TechnoColorID, FrontsConfig.PatinaID, FrontsConfig.InsetTypeID, FrontsConfig.InsetColorID, FrontsConfig.TechnoProfileID, FrontsConfig.TechnoInsetTypeID, FrontsConfig.TechnoInsetColorID, FrontsConfig.Height, FrontsConfig.Width, 
                MarketingCost, PriceRatio, DigitCapacity, Measures.Measure FROM FrontsConfig 
                LEFT JOIN TechStore ON FrontsConfig.ColorID = TechStore.TechStoreID
                INNER JOIN Measures ON FrontsConfig.MeasureID=Measures.MeasureID
                WHERE FrontsConfig.FrontID IS NOT NULL AND Enabled = 1 AND " + ExcluziveFilter + @"
                ORDER BY FrontsConfig.FrontID,FrontsConfig.ColorID,FrontsConfig.PatinaID,FrontsConfig.Height,FrontsConfig.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                FrontsConfigDataTable.Clear();
                DA.Fill(FrontsConfigDataTable);
                if (!FrontsConfigDataTable.Columns.Contains("OriginalPrice"))
                    FrontsConfigDataTable.Columns.Add(new DataColumn(("OriginalPrice"), System.Type.GetType("System.Decimal")));
            }
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность
            decimal OriginalPrice = 0;
            for (int i = 0; i < FrontsConfigDataTable.Rows.Count; i++)
            {
                MarketingCost = Convert.ToDecimal(FrontsConfigDataTable.Rows[i]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(FrontsConfigDataTable.Rows[i]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(FrontsConfigDataTable.Rows[i]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + PriceGroup);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);
                FrontsConfigDataTable.Rows[i]["OriginalPrice"] = OriginalPrice * _rate;
            }

            SelectCommand = @"SELECT * FROM InsetTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int x = 0; x < DT1.Rows.Count; x++)
            {
                int FrontID = Convert.ToInt32(DT1.Rows[x]["FrontID"]);
                string FrontName = GetFrontName(FrontID);
                Color1(FrontID, FrontName);
                Color2(FrontID, FrontName);
                Color3(FrontID, FrontName);
                Color4(FrontID, FrontName);
                Color5(FrontID, FrontName);
                Color6(FrontID, FrontName);
            }

        }

        private void FillReportTable(ref DataTable TempTable)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            DataTable DT3 = new DataTable();
            DataTable TempInsetTable = new DataTable();
            using (DataView DV = new DataView(ResultFrontsDataTable))
            {
                DV.Sort = "FrontName, ColorType";
                DT1 = DV.ToTable(true, new string[] { "FrontName", "ColorType", "Measure" });
            }
            int Index = 0;
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int ColorType = Convert.ToInt32(DT1.Rows[i]["ColorType"]);
                int InsetType = 0;
                //int InsetTypeID = Convert.ToInt32(DT1.Rows[i]["InsetTypeID"]);
                //string Cost = Convert.ToDecimal(ResultFrontsDataTable.Rows[i]["Cost0"]).ToString();
                string Measure = DT1.Rows[i]["Measure"].ToString();
                string FrontName = DT1.Rows[i]["FrontName"].ToString();
                string Color = string.Empty;

                if (ColorType == 1)
                    Color = "Кронинг";
                if (ColorType == 2)
                    Color = "Бумага";
                if (ColorType == 3)
                    Color = "Бумага Лак";
                if (ColorType == 4)
                    Color = "ПВХ";
                if (ColorType == 5)
                    Color = "ПВХ Лак";
                if (ColorType == 6)
                    Color = "ПП";
                if (ColorType == 7)
                    Color = "ПП Лак";
                if (ColorType == 8)
                    Color = "Шпон";

                DataRow[] rows = TempTable.Select("Front='" + FrontName + "'");
                if (rows.Count() == 0)
                {
                    DataRow NewRow = TempTable.NewRow();
                    NewRow["FrontName"] = FrontName;
                    NewRow["Front"] = FrontName;
                    NewRow["Measure"] = Measure;
                    NewRow["Color"] = Color;
                    NewRow["ColorType"] = ColorType;
                    NewRow["ItemGroup"] = true;
                    NewRow["Index"] = Index++;
                    TempTable.Rows.Add(NewRow);
                }
                else
                {
                    DataRow NewRow = TempTable.NewRow();
                    NewRow["Front"] = FrontName;
                    NewRow["Measure"] = Measure;
                    NewRow["Color"] = Color;
                    NewRow["ColorType"] = ColorType;
                    NewRow["ItemGroup"] = true;
                    NewRow["Index"] = Index++;
                    TempTable.Rows.Add(NewRow);
                }

                InsetType = 1;
                //Витрины
                DataRow[] irows = InsetTypesDataTable.Select("InsetTypeID IN (1,2)");
                string filter = string.Empty;
                foreach (DataRow item in irows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = "InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + ")";
                using (DataView DV = new DataView(ResultFrontsDataTable))
                {
                    DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType;
                    DV.Sort = "FrontName, ColorType";
                    DT2 = DV.ToTable(true, new string[] { "Cost0" });
                }
                if (DT2.Rows.Count > 0)
                {
                    if (DT2.Rows.Count == 1)
                    {
                        rows = TempTable.Select("Front='" + FrontName + "' AND ColorType=" + ColorType);
                        if (rows.Count() > 0)
                        {
                            TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost1"] = DT2.Rows[0]["Cost0"].ToString();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            using (DataView DV = new DataView(ResultFrontsDataTable))
                            {
                                DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType + " AND Cost0='" + DT2.Rows[j]["Cost0"].ToString() + "'";
                                DV.Sort = "FrontName, ColorType";
                                DT3 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
                            }
                            for (int x = 0; x < DT3.Rows.Count; x++)
                            {
                                Color = GetColorName(Convert.ToInt32(DT3.Rows[x]["ColorID"]));
                                if (Convert.ToInt32(DT3.Rows[x]["PatinaID"]) != -1)
                                    Color += " " + GetPatinaName(Convert.ToInt32(DT3.Rows[x]["PatinaID"]));
                                rows = TempTable.Select("InsetType<>" + InsetType + " AND Front='" + FrontName + "'" + " AND Color='" + Color + "'");
                                if (rows.Count() > 0)
                                {
                                    TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost1"] = DT2.Rows[j]["Cost0"].ToString();
                                }
                                else
                                {
                                    DataRow NewRow = TempTable.NewRow();
                                    NewRow["Measure"] = Measure;
                                    NewRow["Front"] = FrontName;
                                    NewRow["Color"] = Color;
                                    NewRow["ColorType"] = ColorType;
                                    NewRow["Patina"] = string.Empty;
                                    NewRow["Cost1"] = DT2.Rows[j]["Cost0"].ToString();
                                    NewRow["CostGroup"] = true;
                                    NewRow["Measure"] = ResultFrontsDataTable.Rows[i]["Measure"].ToString();
                                    NewRow["InsetType"] = 1;
                                    NewRow["Index"] = Index++;
                                    TempTable.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
                InsetType = 2;
                //ЛДСтП
                irows = InsetTypesDataTable.Select("GroupID = 4 OR InsetTypeID = -1");
                filter = string.Empty;
                foreach (DataRow item in irows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = "(FrontID IN (3729) OR InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + "))";
                using (DataView DV = new DataView(ResultFrontsDataTable))
                {
                    DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType;
                    DV.Sort = "FrontName, ColorType";
                    DT2 = DV.ToTable(true, new string[] { "Cost0" });
                }
                if (DT2.Rows.Count > 0)
                {
                    if (DT2.Rows.Count == 1)
                    {
                        rows = TempTable.Select("Front='" + FrontName + "' AND ColorType=" + ColorType);
                        if (rows.Count() > 0)
                        {
                            TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost2"] = DT2.Rows[0]["Cost0"].ToString();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            using (DataView DV = new DataView(ResultFrontsDataTable))
                            {
                                DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType + " AND Cost0='" + DT2.Rows[j]["Cost0"].ToString() + "'";
                                DV.Sort = "FrontName, ColorType";
                                DT3 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
                            }
                            for (int x = 0; x < DT3.Rows.Count; x++)
                            {
                                Color = GetColorName(Convert.ToInt32(DT3.Rows[x]["ColorID"]));
                                if (Convert.ToInt32(DT3.Rows[x]["PatinaID"]) != -1)
                                    Color += " " + GetPatinaName(Convert.ToInt32(DT3.Rows[x]["PatinaID"]));
                                rows = TempTable.Select("InsetType<>" + InsetType + " AND Front='" + FrontName + "'" + " AND Color='" + Color + "'");
                                if (rows.Count() > 0)
                                {
                                    TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost2"] = DT2.Rows[j]["Cost0"].ToString();
                                }
                                else
                                {
                                    DataRow NewRow = TempTable.NewRow();
                                    NewRow["Measure"] = Measure;
                                    NewRow["Front"] = FrontName;
                                    NewRow["Color"] = Color;
                                    NewRow["ColorType"] = ColorType;
                                    NewRow["Patina"] = string.Empty;
                                    NewRow["Cost2"] = DT2.Rows[j]["Cost0"].ToString();
                                    NewRow["CostGroup"] = true;
                                    NewRow["InsetType"] = 2;
                                    NewRow["Index"] = Index++;
                                    TempTable.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
                InsetType = 3;
                //ВК
                irows = InsetTypesDataTable.Select("GroupID = 3");
                filter = string.Empty;
                foreach (DataRow item in irows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = "InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + ")";
                using (DataView DV = new DataView(ResultFrontsDataTable))
                {
                    DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType;
                    DV.Sort = "FrontName, ColorType";
                    DT2 = DV.ToTable(true, new string[] { "Cost0" });
                }
                if (DT2.Rows.Count > 0)
                {
                    if (DT2.Rows.Count == 1)
                    {
                        rows = TempTable.Select("Front='" + FrontName + "' AND ColorType=" + ColorType);
                        if (rows.Count() > 0)
                        {
                            TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost3"] = DT2.Rows[0]["Cost0"].ToString();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            using (DataView DV = new DataView(ResultFrontsDataTable))
                            {
                                DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType + " AND Cost0='" + DT2.Rows[j]["Cost0"].ToString() + "'";
                                DV.Sort = "FrontName, ColorType";
                                DT3 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
                            }
                            for (int x = 0; x < DT3.Rows.Count; x++)
                            {
                                Color = GetColorName(Convert.ToInt32(DT3.Rows[x]["ColorID"]));
                                if (Convert.ToInt32(DT3.Rows[x]["PatinaID"]) != -1)
                                    Color += " " + GetPatinaName(Convert.ToInt32(DT3.Rows[x]["PatinaID"]));
                                rows = TempTable.Select("InsetType<>" + InsetType + " AND Front='" + FrontName + "'" + " AND Color='" + Color + "'");
                                if (rows.Count() > 0)
                                {
                                    TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost3"] = DT2.Rows[j]["Cost0"].ToString();
                                }
                                else
                                {
                                    DataRow NewRow = TempTable.NewRow();
                                    NewRow["Measure"] = Measure;
                                    NewRow["Front"] = FrontName;
                                    NewRow["Color"] = Color;
                                    NewRow["ColorType"] = ColorType;
                                    NewRow["Patina"] = string.Empty;
                                    NewRow["Cost3"] = DT2.Rows[j]["Cost0"].ToString();
                                    NewRow["CostGroup"] = true;
                                    NewRow["InsetType"] = 3;
                                    NewRow["Index"] = Index++;
                                    TempTable.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
                InsetType = 4;
                //Фл
                irows = InsetTypesDataTable.Select("InsetTypeID IN (2069,2070,2071,2073,2075,2077,2233,3644,29043,29531,29275,29279,2079,2080,2081,2082,2085,2086,2087,2088,2212,2213,29210,29211,27831,27832,29210,29211)");
                filter = string.Empty;
                foreach (DataRow item in irows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = "InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + ")";
                using (DataView DV = new DataView(ResultFrontsDataTable))
                {
                    DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType;
                    DV.Sort = "FrontName, ColorType";
                    DT2 = DV.ToTable(true, new string[] { "Cost0" });
                }
                if (DT2.Rows.Count > 0)
                {
                    if (DT2.Rows.Count == 1)
                    {
                        rows = TempTable.Select("Front='" + FrontName + "' AND ColorType=" + ColorType);
                        if (rows.Count() > 0)
                        {
                            TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost4"] = DT2.Rows[0]["Cost0"].ToString();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            using (DataView DV = new DataView(ResultFrontsDataTable))
                            {
                                DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType + " AND Cost0='" + DT2.Rows[j]["Cost0"].ToString() + "'";
                                DV.Sort = "FrontName, ColorType";
                                DT3 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
                            }
                            for (int x = 0; x < DT3.Rows.Count; x++)
                            {
                                Color = GetColorName(Convert.ToInt32(DT3.Rows[x]["ColorID"]));
                                if (Convert.ToInt32(DT3.Rows[x]["PatinaID"]) != -1)
                                    Color += " " + GetPatinaName(Convert.ToInt32(DT3.Rows[x]["PatinaID"]));
                                rows = TempTable.Select("InsetType<>" + InsetType + " AND Front='" + FrontName + "'" + " AND Color='" + Color + "'");
                                if (rows.Count() > 0)
                                {
                                    TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost4"] = DT2.Rows[j]["Cost0"].ToString();
                                }
                                else
                                {
                                    DataRow NewRow = TempTable.NewRow();
                                    NewRow["Measure"] = Measure;
                                    NewRow["Front"] = FrontName;
                                    NewRow["Color"] = Color;
                                    NewRow["ColorType"] = ColorType;
                                    NewRow["Patina"] = string.Empty;
                                    NewRow["Cost4"] = DT2.Rows[j]["Cost0"].ToString();
                                    NewRow["CostGroup"] = true;
                                    NewRow["InsetType"] = 4;
                                    NewRow["Index"] = Index++;
                                    TempTable.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
                InsetType = 5;
                //ПН-01
                irows = InsetTypesDataTable.Select("GroupID = 12");
                filter = string.Empty;
                foreach (DataRow item in irows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = "InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + ")";
                using (DataView DV = new DataView(ResultFrontsDataTable))
                {
                    DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType;
                    DV.Sort = "FrontName, ColorType";
                    DT2 = DV.ToTable(true, new string[] { "Cost0" });
                }
                if (DT2.Rows.Count > 0)
                {
                    if (DT2.Rows.Count == 1)
                    {
                        rows = TempTable.Select("Front='" + FrontName + "' AND ColorType=" + ColorType);
                        if (rows.Count() > 0)
                        {
                            TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost5"] = DT2.Rows[0]["Cost0"].ToString();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            using (DataView DV = new DataView(ResultFrontsDataTable))
                            {
                                DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType + " AND Cost0='" + DT2.Rows[j]["Cost0"].ToString() + "'";
                                DV.Sort = "FrontName, ColorType";
                                DT3 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
                            }
                            for (int x = 0; x < DT3.Rows.Count; x++)
                            {
                                Color = GetColorName(Convert.ToInt32(DT3.Rows[x]["ColorID"]));
                                if (Convert.ToInt32(DT3.Rows[x]["PatinaID"]) != -1)
                                    Color += " " + GetPatinaName(Convert.ToInt32(DT3.Rows[x]["PatinaID"]));
                                rows = TempTable.Select("InsetType<>" + InsetType + " AND Front='" + FrontName + "'" + " AND Color='" + Color + "'");
                                if (rows.Count() > 0)
                                {
                                    TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost5"] = DT2.Rows[j]["Cost0"].ToString();
                                }
                                else
                                {
                                    DataRow NewRow = TempTable.NewRow();
                                    NewRow["Measure"] = Measure;
                                    NewRow["Front"] = FrontName;
                                    NewRow["Color"] = Color;
                                    NewRow["ColorType"] = ColorType;
                                    NewRow["Patina"] = string.Empty;
                                    NewRow["Cost5"] = DT2.Rows[j]["Cost0"].ToString();
                                    NewRow["CostGroup"] = true;
                                    NewRow["InsetType"] = 5;
                                    NewRow["Index"] = Index++;
                                    TempTable.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
                InsetType = 6;
                //ПН-03
                irows = InsetTypesDataTable.Select("GroupID = 13");
                filter = string.Empty;
                foreach (DataRow item in irows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = "InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + ")";
                using (DataView DV = new DataView(ResultFrontsDataTable))
                {
                    DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType;
                    DV.Sort = "FrontName, ColorType";
                    DT2 = DV.ToTable(true, new string[] { "Cost0" });
                }
                if (DT2.Rows.Count > 0)
                {
                    if (DT2.Rows.Count == 1)
                    {
                        rows = TempTable.Select("Front='" + FrontName + "' AND ColorType=" + ColorType);
                        if (rows.Count() > 0)
                        {
                            TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost6"] = DT2.Rows[0]["Cost0"].ToString();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            using (DataView DV = new DataView(ResultFrontsDataTable))
                            {
                                DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType + " AND Cost0='" + DT2.Rows[j]["Cost0"].ToString() + "'";
                                DV.Sort = "FrontName, ColorType";
                                DT3 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
                            }
                            for (int x = 0; x < DT3.Rows.Count; x++)
                            {
                                Color = GetColorName(Convert.ToInt32(DT3.Rows[x]["ColorID"]));
                                if (Convert.ToInt32(DT3.Rows[x]["PatinaID"]) != -1)
                                    Color += " " + GetPatinaName(Convert.ToInt32(DT3.Rows[x]["PatinaID"]));
                                rows = TempTable.Select("InsetType<>" + InsetType + " AND Front='" + FrontName + "'" + " AND Color='" + Color + "'");
                                if (rows.Count() > 0)
                                {
                                    TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost6"] = DT2.Rows[j]["Cost0"].ToString();
                                }
                                else
                                {
                                    DataRow NewRow = TempTable.NewRow();
                                    NewRow["Measure"] = Measure;
                                    NewRow["Front"] = FrontName;
                                    NewRow["Color"] = Color;
                                    NewRow["ColorType"] = ColorType;
                                    NewRow["Patina"] = string.Empty;
                                    NewRow["Cost6"] = DT2.Rows[j]["Cost0"].ToString();
                                    NewRow["CostGroup"] = true;
                                    NewRow["InsetType"] = 6;
                                    NewRow["Index"] = Index++;
                                    TempTable.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
                InsetType = 7;
                //ПН-10
                irows = InsetTypesDataTable.Select("GroupID = 15");
                filter = string.Empty;
                foreach (DataRow item in irows)
                    filter += item["InsetTypeID"].ToString() + ",";
                if (filter.Length > 0)
                    filter = "InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + ")";
                using (DataView DV = new DataView(ResultFrontsDataTable))
                {
                    DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType;
                    DV.Sort = "FrontName, ColorType";
                    DT2 = DV.ToTable(true, new string[] { "Cost0" });
                }
                if (DT2.Rows.Count > 0)
                {
                    HasPlanka = true;
                    if (DT2.Rows.Count == 1)
                    {
                        rows = TempTable.Select("Front='" + FrontName + "' AND ColorType=" + ColorType);
                        if (rows.Count() > 0)
                        {
                            TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost7"] = DT2.Rows[0]["Cost0"].ToString();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            using (DataView DV = new DataView(ResultFrontsDataTable))
                            {
                                DV.RowFilter = filter + " AND FrontName='" + FrontName + "' AND ColorType=" + ColorType + " AND Cost0='" + DT2.Rows[j]["Cost0"].ToString() + "'";
                                DV.Sort = "FrontName, ColorType";
                                DT3 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
                            }
                            for (int x = 0; x < DT3.Rows.Count; x++)
                            {
                                Color = GetColorName(Convert.ToInt32(DT3.Rows[x]["ColorID"]));
                                if (Convert.ToInt32(DT3.Rows[x]["PatinaID"]) != -1)
                                    Color += " " + GetPatinaName(Convert.ToInt32(DT3.Rows[x]["PatinaID"]));
                                rows = TempTable.Select("InsetType<>" + InsetType + " AND Front='" + FrontName + "'" + " AND Color='" + Color + "'");
                                if (rows.Count() > 0)
                                {
                                    TempTable.Rows[TempTable.Rows.IndexOf(rows[0])]["Cost7"] = DT2.Rows[j]["Cost0"].ToString();
                                }
                                else
                                {
                                    DataRow NewRow = TempTable.NewRow();
                                    NewRow["Measure"] = Measure;
                                    NewRow["Front"] = FrontName;
                                    NewRow["Color"] = Color;
                                    NewRow["ColorType"] = ColorType;
                                    NewRow["Patina"] = string.Empty;
                                    NewRow["Cost7"] = DT2.Rows[j]["Cost0"].ToString();
                                    NewRow["CostGroup"] = true;
                                    NewRow["InsetType"] = 7;
                                    NewRow["Index"] = Index++;
                                    TempTable.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Color1(int FrontID, string FrontName)
        {
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "TechStoreSubGroupID=59 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND TechStoreSubGroupID=59 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 1;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        public void Color2(int FrontID, string FrontName)
        {
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "PatinaID=-1 AND TechStoreSubGroupID=60 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND PatinaID=-1 AND TechStoreSubGroupID=60 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 2;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }

            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "PatinaID<>-1 AND TechStoreSubGroupID=60 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND PatinaID<>-1 AND TechStoreSubGroupID=60 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 3;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        public void Color3(int FrontID, string FrontName)
        {
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "PatinaID=-1 AND TechStoreSubGroupID=61 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND PatinaID=-1 AND TechStoreSubGroupID=61 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 4;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "PatinaID<>-1 AND TechStoreSubGroupID=61 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND PatinaID<>-1 AND TechStoreSubGroupID=61 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 5;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        public void Color4(int FrontID, string FrontName)
        {
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "PatinaID=-1 AND TechStoreSubGroupID=63 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND PatinaID=-1 AND TechStoreSubGroupID=63 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 6;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "PatinaID<>-1 AND TechStoreSubGroupID=63 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND PatinaID<>-1 AND TechStoreSubGroupID=63 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 7;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        public void Color5(int FrontID, string FrontName)
        {
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "TechStoreSubGroupID=62 AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND TechStoreSubGroupID=62 AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 8;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        public void Color6(int FrontID, string FrontName)
        {
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(FrontsConfigDataTable))
            {
                DV.RowFilter = "TechStoreSubGroupID IS NULL AND FrontID=" + FrontID;
                DT2 = DV.ToTable(true, new string[] { "InsetTypeID", "OriginalPrice" });
            }
            for (int y = 0; y < DT2.Rows.Count; y++)
            {
                DataRow[] fRows = FrontsConfigDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND TechStoreSubGroupID IS NULL AND OriginalPrice='" + Convert.ToDecimal(DT2.Rows[y]["OriginalPrice"]) + "'");
                for (int c = 0; c < fRows.Count(); c++)
                {
                    DataRow[] rows3 = ResultFrontsDataTable.Select("InsetTypeID=" + Convert.ToInt32(DT2.Rows[y]["InsetTypeID"]) + " AND FrontID=" + FrontID + " AND ColorID=" + Convert.ToInt32(fRows[c]["ColorID"]) + " AND Cost0='" + Convert.ToDecimal(fRows[c]["OriginalPrice"]) + "'");
                    if (rows3.Count() == 0)
                    {
                        DataRow NewRow = ResultFrontsDataTable.NewRow();
                        NewRow["Measure"] = fRows[c]["Measure"].ToString();
                        NewRow["FrontName"] = FrontName;
                        NewRow["Color"] = GetColorName(Convert.ToInt32(fRows[c]["ColorID"]));
                        NewRow["FrontID"] = FrontID;
                        NewRow["ColorID"] = Convert.ToInt32(fRows[c]["ColorID"]);
                        NewRow["PatinaID"] = Convert.ToInt32(fRows[c]["PatinaID"]);
                        NewRow["Height"] = -1;
                        NewRow["Width"] = -1;
                        NewRow["InsetTypeID"] = Convert.ToInt32(fRows[c]["InsetTypeID"]);
                        NewRow["ColorType"] = 8;
                        NewRow["ColorGroup"] = 1;
                        NewRow["Cost0"] = Convert.ToDecimal(fRows[c]["OriginalPrice"]);
                        ResultFrontsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        public string GetFrontName(int FrontID)
        {
            string name = string.Empty;
            DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
            if (Rows.Count() > 0)
                name = Rows[0]["FrontName"].ToString();
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

        public void CreateReport(ref HSSFWorkbook hssfworkbook, int ClientID, decimal PriceGroup)
        {
            bool HasExcluzive = false;
            ResultFrontsDataTable.Clear();
            {
                FillFrontConfigTable(ClientID, PriceGroup, true);
                FillReportTable(ref ExcluziveTable);
            }
            if (ExcluziveTable.Rows.Count > 0)
            {
                HasExcluzive = true;
                sheet1 = hssfworkbook.CreateSheet("Эксклюзив фасады");
                sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
                sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
                sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
                sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

                int DisplayIndex = 0;
                int RowIndex = 1;
                sheet1.SetColumnWidth(DisplayIndex++, 25 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                //sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 11 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 11 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 11 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet1.SetColumnWidth(DisplayIndex++, 8 * 256);

                HSSFFont HeaderF = hssfworkbook.CreateFont();
                HeaderF.FontHeightInPoints = 11;
                HeaderF.Boldweight = 11 * 256;
                HeaderF.FontName = "Calibri";
                HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
                HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
                HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
                HeaderCS.WrapText = true;
                HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
                HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
                HeaderCS.SetFont(HeaderF);
                HSSFCell cell;
                DisplayIndex = 0;
                HSSFRow r = sheet1.CreateRow(RowIndex);
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Наименование");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Цвет");
                cell.CellStyle = HeaderCS;
                //cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Патина");
                //cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Ед.изм.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Витрина");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ЛДСтП\r\n(ЛМДФ)");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ВК");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Фл");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Пн-01");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Пн-03");
                cell.CellStyle = HeaderCS;
                if (HasPlanka)
                {
                    cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Пн-10");
                    cell.CellStyle = HeaderCS;
                    sheet1.CreateFreezePane(10, 2, 10, 2);
                }
                else
                    sheet1.CreateFreezePane(9, 2, 9, 2);
                RowIndex++;

                ReportToExcel(ExcluziveTable, ref hssfworkbook, ref sheet1, ref RowIndex);
                RowIndex++;
            }
            ResultFrontsDataTable.Clear();
            {
                FillFrontConfigTable(ClientID, PriceGroup, false);
                FillReportTable(ref NotExcluziveTable);
            }
            if (NotExcluziveTable.Rows.Count > 0)
            {
                if (HasExcluzive)
                    sheet2 = hssfworkbook.CreateSheet("Не эксклюзив фасады");
                else
                    sheet2 = hssfworkbook.CreateSheet("Прайс фасады");
                sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
                sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
                sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
                sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

                int DisplayIndex = 0;
                int RowIndex = 1;
                sheet2.SetColumnWidth(DisplayIndex++, 25 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 20 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                //sheet2.SetColumnWidth(DisplayIndex++, 8 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 11 * 256);
                sheet2.SetColumnWidth(DisplayIndex++, 11 * 256);
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
                HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
                HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
                HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
                HeaderCS.WrapText = true;
                HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
                HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
                HeaderCS.SetFont(HeaderF);
                HSSFCell cell;
                DisplayIndex = 0;

                HSSFRow r = sheet2.CreateRow(RowIndex);
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Наименование");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Цвет");
                cell.CellStyle = HeaderCS;
                //cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Патина");
                //cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Ед.изм.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Витрина");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ЛДСтП\r\n(ЛМДФ)");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "ВК");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Фл");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Пн-01");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Пн-03");
                cell.CellStyle = HeaderCS;
                //if (HasPlanka)
                //{
                //    cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Пн-10");
                //    cell.CellStyle = HeaderCS;
                //}
                RowIndex++;
                sheet2.CreateFreezePane(9, 2, 9, 2);

                ReportToExcel(NotExcluziveTable, ref hssfworkbook, ref sheet2, ref RowIndex);
                RowIndex++;
            }
        }

        public void ReportToExcel(DataTable TempTable, ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1, ref int RowIndex)
        {
            HSSFFont HeaderF = hssfworkbook.CreateFont();
            HeaderF.FontHeightInPoints = 11;
            HeaderF.Boldweight = 11 * 256;
            HeaderF.FontName = "Calibri";
            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 10;
            SimpleF.FontName = "Calibri";
            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.SetFont(SimpleF);
            HSSFCellStyle SimpleRightAlignCS = hssfworkbook.CreateCellStyle();
            SimpleRightAlignCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            SimpleRightAlignCS.SetFont(SimpleF);
            HSSFCellStyle ItemCS = hssfworkbook.CreateCellStyle();
            ItemCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            //ItemCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            ItemCS.SetFont(SimpleF);
            HSSFCellStyle CostCS = hssfworkbook.CreateCellStyle();
            CostCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CostCS.SetFont(SimpleF);
            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.WrapText = true;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.SetFont(HeaderF);

            int DisplayIndex = 0;
            HSSFCell cell;
            DisplayIndex = 0;
            for (int x = 0; x < TempTable.Rows.Count; x++)
            {
                HSSFRow r = sheet1.CreateRow(RowIndex);
                DisplayIndex = 0;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["FrontName"].ToString());
                cell.CellStyle = ItemCS;
                if (TempTable.Rows[x]["CostGroup"] == DBNull.Value)
                {
                    cell = r.CreateCell(DisplayIndex++);
                    cell.SetCellValue(TempTable.Rows[x]["Color"].ToString());
                    cell.CellStyle = SimpleCS;
                }
                else
                {
                    cell = r.CreateCell(DisplayIndex++);
                    cell.SetCellValue(TempTable.Rows[x]["Color"].ToString());
                    cell.CellStyle = SimpleRightAlignCS;
                }
                //cell = r.CreateCell(DisplayIndex++);
                //cell.SetCellValue(TempTable.Rows[x]["Patina"].ToString());
                //cell.CellStyle = SimpleCS;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["Measure"].ToString());
                cell.CellStyle = SimpleCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost1"] != DBNull.Value)
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost1"]));
                cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost2"] != DBNull.Value)
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost2"]));
                cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost3"] != DBNull.Value)
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost3"]));
                cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost4"] != DBNull.Value)
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost4"]));
                cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost5"] != DBNull.Value)
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost5"]));
                cell.CellStyle = CostCS;
                cell = r.CreateCell(DisplayIndex++);
                if (TempTable.Rows[x]["Cost6"] != DBNull.Value)
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost6"]));
                cell.CellStyle = CostCS;
                if (HasPlanka)
                {
                    cell = r.CreateCell(DisplayIndex++);
                    if (TempTable.Rows[x]["Cost7"] != DBNull.Value)
                        cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Cost7"]));
                    cell.CellStyle = CostCS;
                }
                RowIndex++;
            }


            RowIndex++;

            HSSFRow g = sheet1.CreateRow(RowIndex);
            cell = HSSFCellUtil.CreateCell(g, 0, "* - для фасадов с аппликациями добавляется 5 евро за шт.");
            cell.CellStyle = CostCS;

            RowIndex++;
            //int firstRowGroupIndex = 0;
            //int secondRowGroupIndex = 0;
            //int firstRowMergeIndex = 0;
            //int secondRowMergeIndex = 0;
            //bool NeedGroup = false;
            //int InsetType = 0;
            //string Name = string.Empty;
            //for (int x = 0; x < ReportTable.Rows.Count; x++)
            //{
            //    string CurrentName = ReportTable.Rows[x]["FrontName"].ToString();
            //    if (x == ReportTable.Rows.Count - 1)
            //    {
            //        continue;
            //    }
            //    if (firstRowMergeIndex == 0)
            //    {
            //        if (ReportTable.Rows[x]["FrontName"].ToString().Length == 0)
            //        {
            //            firstRowMergeIndex = x;
            //        }
            //    }
            //    else
            //    {
            //        if (ReportTable.Rows[x + 1]["FrontName"].ToString().Length > 0)
            //        {
            //            secondRowMergeIndex = x;
            //        }
            //    }
            //    if (ReportTable.Rows[x]["CostGroup"] != DBNull.Value && firstRowGroupIndex == 0)
            //    {
            //        firstRowGroupIndex = x + 1;
            //        InsetType = Convert.ToInt32(ReportTable.Rows[x]["InsetType"]);
            //    }
            //    int Index = Convert.ToInt32(ReportTable.Rows[x]["Index"]);
            //    if (!NeedGroup)
            //    {
            //        if (firstRowGroupIndex == 0)
            //        {
            //            if (ReportTable.Rows[x]["CostGroup"] != DBNull.Value && Convert.ToBoolean(ReportTable.Rows[x]["CostGroup"]))
            //            {
            //                firstRowGroupIndex = x + 1;
            //                continue;
            //            }
            //        }
            //        else
            //        {
            //            if (ReportTable.Rows[x + 1]["OriginalPrice" + InsetType] == DBNull.Value || ReportTable.Rows[x + 1]["InsetType"] == DBNull.Value ||
            //                (ReportTable.Rows[x]["OriginalPrice" + InsetType] != DBNull.Value && ReportTable.Rows[x + 1]["OriginalPrice" + InsetType] != DBNull.Value && Convert.ToDecimal(ReportTable.Rows[x]["OriginalPrice" + InsetType]) != Convert.ToDecimal(ReportTable.Rows[x + 1]["OriginalPrice" + InsetType])))
            //            {
            //                secondRowGroupIndex = x;
            //                NeedGroup = true;
            //            }
            //        }
            //    }
            //    if (NeedGroup)
            //    {
            //        if (secondRowMergeIndex != 0)
            //        {
            //            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(firstRowMergeIndex + 1, 0, secondRowMergeIndex + 2, 0));
            //            firstRowMergeIndex = 0;
            //            secondRowMergeIndex = 0;
            //        }
            //        NeedGroup = false;
            //        sheet1.GroupRow(firstRowGroupIndex + 2, secondRowGroupIndex + 2);
            //        sheet1.SetRowGroupCollapsed(firstRowGroupIndex + 2, true);
            //        firstRowGroupIndex = 0;
            //        secondRowGroupIndex = 0;
            //    }
            //}
            //string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            //FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            //int j = 1;
            //while (file.Exists == true)
            //    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            //FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            //hssfworkbook.Write(NewFile);
            //NewFile.Close();
            //System.Diagnostics.Process.Start(file.FullName);
        }

    }
}
