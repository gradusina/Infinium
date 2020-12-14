using Infinium.Modules.Packages.ZOV;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium
{
    public class FrontsCatalog
    {
        public FileManager FM = new FileManager();
        int FactoryID = 1;
        DataTable ClientsCatalogFrontsConfigDT = null;
        DataTable ClientsCatalogDecorConfigDT = null;
        DataTable ClientsCatalogImagesDT = null;

        private DataTable TempFrontsConfigDataTable = null;
        private DataTable TempFrontsDataTable = null;

        DataTable AttachmentsDT = null;

        public DataTable ConstFrontsConfigDataTable = null;
        public DataTable ConstFrontsDataTable = null;
        public DataTable ConstColorsDataTable = null;
        public DataTable ConstPatinaDataTable = null;
        public DataTable ConstInsetTypesDataTable = null;
        public DataTable ConstInsetColorsDataTable = null;

        DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        public DataTable TechnoFrameColorsDataTable = null;
        public DataTable TechnoInsetTypesDataTable = null;
        public DataTable TechnoInsetColorsDataTable = null;

        public DataTable HeightDataTable = null;
        public DataTable WidthDataTable = null;

        public DataTable MeasuresDataTable = null;
        public DataTable MarketingPriceDataTable = null;
        //public DataTable MarketingExtraPriceDataTable = null;
        public DataTable ZOVPriceDataTable = null;

        public BindingSource FrontsBindingSource = null;
        public BindingSource FrameColorsBindingSource = null;
        public BindingSource PatinaBindingSource = null;
        public BindingSource InsetTypesBindingSource = null;
        public BindingSource InsetColorsBindingSource = null;
        public BindingSource TechnoFrameColorsBindingSource = null;
        public BindingSource TechnoInsetTypesBindingSource = null;
        public BindingSource TechnoInsetColorsBindingSource = null;
        public BindingSource FrontsConfigBindingSource = null;
        public BindingSource HeightBindingSource = null;
        public BindingSource WidthBindingSource = null;
        public BindingSource MarketingPriceBindingSource = null;
        //public BindingSource MarketingExtraPriceBindingSource = null;
        public BindingSource ZOVPriceBindingSource = null;

        public String FrontsBindingSourceDisplayMember = null;
        public String FrameColorsBindingSourceDisplayMember = null;
        public String PatinaBindingSourceDisplayMember = null;
        public String InsetColorsBindingSourceDisplayMember = null;
        public String InsetTypesBindingSourceDisplayMember = null;
        public String WidthBindingSourceDisplayMember = null;
        public String HeightBindingSourceDisplayMember = null;

        public String FrontsBindingSourceValueMember = null;
        public String FrameColorsBindingSourceValueMember = null;
        public String PatinaBindingSourceValueMember = null;
        public String InsetColorsBindingSourceValueMember = null;
        public String InsetTypesBindingSourceValueMember = null;

        public FrontsCatalog(int tFactoryID)
        {
            FactoryID = tFactoryID;
            Initialize();
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        private void Create()
        {
            AttachmentsDT = new DataTable();
            AttachmentsDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            AttachmentsDT.Columns.Add(new DataColumn("Extension", Type.GetType("System.String")));
            AttachmentsDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));

            ClientsCatalogFrontsConfigDT = new DataTable();
            InsetTypesConfigDT = new DataTable();
            InsetColorsConfigDT = new DataTable();
            TechnoInsetColorsConfigDT = new DataTable();

            ConstFrontsConfigDataTable = new DataTable();
            ConstFrontsDataTable = new DataTable();
            ConstColorsDataTable = new DataTable();
            ConstPatinaDataTable = new DataTable();
            ConstInsetTypesDataTable = new DataTable();
            ConstInsetColorsDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            TechnoFrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();
            HeightDataTable = new DataTable();
            WidthDataTable = new DataTable();

            TempFrontsConfigDataTable = new DataTable();
            TempFrontsDataTable = new DataTable();

            MeasuresDataTable = new DataTable();

            MarketingPriceDataTable = new DataTable();
            MarketingPriceDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            MarketingPriceDataTable.Columns.Add(new DataColumn(("MarketingPrice"), System.Type.GetType("System.String")));
            MarketingPriceDataTable.Columns.Add(new DataColumn(("ZOVNonStandMargin"), System.Type.GetType("System.String")));
            //MarketingPriceDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            //MarketingExtraPriceDataTable = new DataTable();
            //MarketingExtraPriceDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            //MarketingExtraPriceDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            ZOVPriceDataTable = new DataTable();
            ZOVPriceDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            ZOVPriceDataTable.Columns.Add(new DataColumn(("ZOVRetailPrice"), System.Type.GetType("System.Decimal")));
            ZOVPriceDataTable.Columns.Add(new DataColumn(("ZOVNonStandMargin"), System.Type.GetType("System.Decimal")));
            ZOVPriceDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            FrontsConfigBindingSource = new BindingSource();
            FrontsBindingSource = new BindingSource();
            FrameColorsBindingSource = new BindingSource();
            TechnoFrameColorsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            InsetTypesBindingSource = new BindingSource();
            InsetColorsBindingSource = new BindingSource();
            TechnoInsetTypesBindingSource = new BindingSource();
            TechnoInsetColorsBindingSource = new BindingSource();
            HeightBindingSource = new BindingSource();
            WidthBindingSource = new BindingSource();
            MarketingPriceBindingSource = new BindingSource();
            //MarketingExtraPriceBindingSource = new BindingSource();
            ZOVPriceBindingSource = new BindingSource();
        }

        private void GetColorsDT()
        {
            ConstColorsDataTable = new DataTable();
            ConstColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ConstColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = ConstColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        ConstColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = ConstColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        ConstColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ConstColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ConstColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT * FROM FrontsConfig WHERE Enabled = 1 AND FactoryID=" + FactoryID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstFrontsConfigDataTable);
            }
            SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND FactoryID=" + FactoryID + @")
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstFrontsDataTable);
            }
            GetColorsDT();
            SelectCommand = @"SELECT * FROM Patina";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstPatinaDataTable);
            }
            SelectCommand = @"SELECT * FROM InsetTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstInsetTypesDataTable);
            }
            SelectCommand = @"SELECT * FROM InsetColors";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstInsetColorsDataTable);
            }
            SelectCommand = @"SELECT * FROM Measures";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable.Copy();
            TempFrontsDataTable = ConstFrontsDataTable.Copy();
            FrontsDataTable = ConstFrontsDataTable.Copy();
            FrameColorsDataTable = ConstColorsDataTable.Copy();
            PatinaDataTable = ConstPatinaDataTable.Copy();
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
            InsetTypesDataTable = ConstInsetTypesDataTable.Copy();
            InsetColorsDataTable = ConstInsetColorsDataTable.Copy();
            TechnoFrameColorsDataTable = ConstColorsDataTable.Copy();
            TechnoInsetTypesDataTable = ConstInsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = ConstInsetColorsDataTable.Copy();
        }

        public void FilterCatalog(int FactoryID)
        {
            string SelectCommand = @"SELECT * FROM FrontsConfig WHERE Enabled = 1 AND FactoryID=" + FactoryID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                ConstFrontsConfigDataTable.Clear();
                DA.Fill(ConstFrontsConfigDataTable);
            }
            SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND FactoryID=" + FactoryID + @")
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                ConstFrontsDataTable.Clear();
                DA.Fill(ConstFrontsDataTable);
            }
        }

        private void Binding()
        {
            FrontsConfigBindingSource.DataSource = ConstFrontsConfigDataTable;
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrameColorsBindingSource.DataSource = FrameColorsDataTable;
            PatinaBindingSource.DataSource = PatinaDataTable;
            InsetTypesBindingSource.DataSource = InsetTypesDataTable;
            InsetColorsBindingSource.DataSource = InsetColorsDataTable;
            TechnoFrameColorsBindingSource.DataSource = TechnoFrameColorsDataTable;
            TechnoInsetTypesBindingSource.DataSource = TechnoInsetTypesDataTable;
            TechnoInsetColorsBindingSource.DataSource = TechnoInsetColorsDataTable;
            MarketingPriceBindingSource.DataSource = MarketingPriceDataTable;
            //MarketingExtraPriceBindingSource.DataSource = MarketingExtraPriceDataTable;
            ZOVPriceBindingSource.DataSource = ZOVPriceDataTable;

            FrontsBindingSourceDisplayMember = "FrontName";
            FrameColorsBindingSourceDisplayMember = "ColorName";
            PatinaBindingSourceDisplayMember = "PatinaName";
            InsetTypesBindingSourceDisplayMember = "InsetType";
            InsetColorsBindingSourceDisplayMember = "InsetColorName";
            WidthBindingSourceDisplayMember = "Width";
            HeightBindingSourceDisplayMember = "Height";

            FrontsBindingSourceValueMember = "FrontID";
            FrameColorsBindingSourceValueMember = "ColorID";
            PatinaBindingSourceValueMember = "PatinaID";
            InsetColorsBindingSourceValueMember = "InsetColorID";
            InsetTypesBindingSourceValueMember = "InsetTypeID";
        }

        public void GetMarketingPrice()
        {
            foreach (DataRow item in TempFrontsConfigDataTable.Rows)
            {
                decimal MarketingPrice = 0;
                GetFrontPrice(item, ref MarketingPrice);
                item["MarketingPrice"] = MarketingPrice;
            }

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "MeasureID", "MarketingPrice", "ZOVNonStandMargin" });
            }
            MarketingPriceDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
            {
                foreach (DataRow CRow in MeasuresDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["MeasureID"]) == Convert.ToInt32(Row["MeasureID"]))
                    {
                        DataRow NewRow = MarketingPriceDataTable.NewRow();
                        NewRow["MeasureID"] = CRow["MeasureID"];
                        NewRow["MarketingPrice"] = Convert.ToDecimal(Row["MarketingPrice"]) + " " + CRow["Measure"];
                        NewRow["ZOVNonStandMargin"] = Convert.ToInt32(Row["ZOVNonStandMargin"]) + " %";
                        MarketingPriceDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            Table.Dispose();
            MarketingPriceBindingSource.MoveFirst();
        }

        private void GetFrontPrice(DataRow FrontsOrderRow, ref decimal OriginalPrice)
        {
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность

            int DecorConfigID = Convert.ToInt32(FrontsOrderRow["FrontConfigID"]);
            DataRow[] PRows = TempFrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrderRow["FrontConfigID"].ToString());
            if (PRows.Count() > 0)
            {
                MarketingCost = Convert.ToDecimal(PRows[0]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(PRows[0]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(PRows[0]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + 0);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);

                //if (Security.CurrentClientID == 145)
                //    OriginalPrice = Convert.ToDecimal(PRows[0]["ZOVPrice"]);
            }
        }

        public void GetZOVPrice(int ConfigID)
        {
            DataRow[] Rows = TempFrontsConfigDataTable.Select("FrontConfigID=" + ConfigID);
            ZOVPriceDataTable.Clear();

            foreach (DataRow Row in Rows)
            {
                DataRow NewRow = ZOVPriceDataTable.NewRow();
                NewRow["MeasureID"] = Convert.ToInt32(Row["MeasureID"]);
                NewRow["ZOVRetailPrice"] = Convert.ToDecimal(Row["ZOVRetailPrice"]);
                NewRow["ZOVNonStandMargin"] = Convert.ToInt32(Row["ZOVNonStandMargin"]);
                NewRow["Measure"] = GetMeasure(Convert.ToInt32(Row["MeasureID"]));
                ZOVPriceDataTable.Rows.Add(NewRow);
            }
            ZOVPriceBindingSource.MoveFirst();
        }

        public void GetFrameColors()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ColorID" });
            }

            FrameColorsDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
                foreach (DataRow CRow in ConstColorsDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["ColorID"]) == Convert.ToInt32(Row["ColorID"]))
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = CRow["ColorID"];
                        NewRow["ColorName"] = CRow["ColorName"];
                        FrameColorsDataTable.Rows.Add(NewRow);
                        break;
                    }
                }

            Table.Dispose();

            if (FrameColorsDataTable.Rows.Count == 0)
            {
                DataRow NewRow = FrameColorsDataTable.NewRow();
                NewRow["ColorID"] = "-1";
                NewRow["ColorName"] = "-";
                FrameColorsDataTable.Rows.Add(NewRow);
            }
            else
            {
                FrameColorsDataTable.DefaultView.Sort = "ColorName ASC";
            }
            FrameColorsBindingSource.MoveFirst();
        }

        public void GetPatina()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "PatinaID" });
            }

            PatinaDataTable.Clear();

            bool HasPatinaRAL = false;
            if (Table.Rows.Count > 0)
            {

                DataRow[] fRows = PatinaRALDataTable.Select("PatinaID=" + Convert.ToInt32(Table.Rows[0]["PatinaID"]));
                if (fRows.Count() > 0)
                    HasPatinaRAL = true;
            }
            if (Table.Rows.Count > 0 && HasPatinaRAL)
            {
                foreach (DataRow Row in Table.Rows)
                {
                    foreach (DataRow item in PatinaRALDataTable.Select("PatinaID=" + Convert.ToInt32(Row["PatinaID"])))
                    {
                        DataRow NewRow = PatinaDataTable.NewRow();
                        NewRow["PatinaID"] = item["PatinaRALID"];
                        NewRow["PatinaName"] = item["PatinaRAL"];
                        NewRow["DisplayName"] = item["DisplayName"];
                        PatinaDataTable.Rows.Add(NewRow);
                    }
                }
            }
            else
            {
                foreach (DataRow Row in Table.Rows)
                    foreach (DataRow CRow in ConstPatinaDataTable.Rows)
                    {
                        if (Convert.ToInt32(CRow["PatinaID"]) == Convert.ToInt32(Row["PatinaID"]))
                        {
                            DataRow NewRow = PatinaDataTable.NewRow();
                            NewRow["PatinaID"] = CRow["PatinaID"];
                            NewRow["PatinaName"] = CRow["PatinaName"];
                            PatinaDataTable.Rows.Add(NewRow);
                            break;
                        }
                    }
            }

            Table.Dispose();

            if (PatinaDataTable.Rows.Count == 0)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = "-1";
                NewRow["PatinaName"] = "-";
                PatinaDataTable.Rows.Add(NewRow);
            }
            else
            {
                PatinaDataTable.DefaultView.Sort = "PatinaName ASC";
            }
            PatinaBindingSource.MoveFirst();
        }

        public void GetInsetTypes()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "InsetTypeID" });
            }

            InsetTypesDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
                foreach (DataRow CRow in ConstInsetTypesDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["InsetTypeID"]) == Convert.ToInt32(Row["InsetTypeID"]))
                    {
                        DataRow NewRow = InsetTypesDataTable.NewRow();
                        NewRow["InsetTypeID"] = CRow["InsetTypeID"];
                        NewRow["GroupID"] = CRow["GroupID"];
                        NewRow["InsetTypeName"] = CRow["InsetTypeName"];
                        InsetTypesDataTable.Rows.Add(NewRow);
                        break;
                    }
                }

            Table.Dispose();

            InsetTypesDataTable.DefaultView.Sort = "InsetTypeName ASC";
            InsetTypesBindingSource.MoveFirst();
        }

        public void GetInsetColors()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "InsetColorID" });
            }

            InsetColorsDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
                foreach (DataRow CRow in ConstInsetColorsDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["InsetColorID"]) == Convert.ToInt32(Row["InsetColorID"]))
                    {
                        DataRow NewRow = InsetColorsDataTable.NewRow();
                        NewRow["InsetColorID"] = CRow["InsetColorID"];
                        NewRow["GroupID"] = CRow["GroupID"];
                        NewRow["InsetColorName"] = CRow["InsetColorName"];
                        InsetColorsDataTable.Rows.Add(NewRow);
                        break;
                    }
                }

            Table.Dispose();
            if (InsetColorsDataTable.Rows.Count == 0)
            {
                DataRow NewRow = InsetColorsDataTable.NewRow();
                NewRow["InsetColorID"] = "-1";
                NewRow["InsetColorName"] = "-";
                InsetColorsDataTable.Rows.Add(NewRow);
            }
            else
                InsetColorsDataTable.DefaultView.Sort = "InsetColorName ASC";
            InsetColorsBindingSource.MoveFirst();
        }

        public void GetTechnoFrameColors()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "TechnoColorID" });
            }

            TechnoFrameColorsDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
                foreach (DataRow CRow in ConstColorsDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["ColorID"]) == Convert.ToInt32(Row["TechnoColorID"]))
                    {
                        DataRow NewRow = TechnoFrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = CRow["ColorID"];
                        NewRow["ColorName"] = CRow["ColorName"];
                        TechnoFrameColorsDataTable.Rows.Add(NewRow);
                        break;
                    }
                }

            Table.Dispose();

            TechnoFrameColorsDataTable.DefaultView.Sort = "ColorName ASC";
            TechnoFrameColorsBindingSource.MoveFirst();
        }

        public void GetTechnoInsetTypes()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "TechnoInsetTypeID" });
            }

            TechnoInsetTypesDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
                foreach (DataRow CRow in ConstInsetTypesDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["InsetTypeID"]) == Convert.ToInt32(Row["TechnoInsetTypeID"]))
                    {
                        DataRow NewRow = TechnoInsetTypesDataTable.NewRow();
                        NewRow["InsetTypeID"] = CRow["InsetTypeID"];
                        NewRow["GroupID"] = CRow["GroupID"];
                        NewRow["InsetTypeName"] = CRow["InsetTypeName"];
                        TechnoInsetTypesDataTable.Rows.Add(NewRow);
                        break;
                    }
                }

            Table.Dispose();

            TechnoInsetTypesDataTable.DefaultView.Sort = "InsetTypeName ASC";
            TechnoInsetTypesBindingSource.MoveFirst();
        }

        public void GetTechnoInsetColors()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "TechnoInsetColorID" });
            }

            TechnoInsetColorsDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
                foreach (DataRow CRow in ConstInsetColorsDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["InsetColorID"]) == Convert.ToInt32(Row["TechnoInsetColorID"]))
                    {
                        DataRow NewRow = TechnoInsetColorsDataTable.NewRow();
                        NewRow["InsetColorID"] = CRow["InsetColorID"];
                        NewRow["GroupID"] = CRow["GroupID"];
                        NewRow["InsetColorName"] = CRow["InsetColorName"];
                        TechnoInsetColorsDataTable.Rows.Add(NewRow);
                        break;
                    }
                }

            Table.Dispose();
            if (TechnoInsetColorsDataTable.Rows.Count == 0)
            {
                DataRow NewRow = TechnoInsetColorsDataTable.NewRow();
                NewRow["InsetColorID"] = "-1";
                NewRow["InsetColorName"] = "-";
                TechnoInsetColorsDataTable.Rows.Add(NewRow);
            }
            else
                TechnoInsetColorsDataTable.DefaultView.Sort = "InsetColorName ASC";
            TechnoInsetColorsBindingSource.MoveFirst();
        }

        public void GetHeight()
        {
            DataTable Table = new DataTable();

            HeightDataTable.Clear();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                HeightDataTable = DV.ToTable(true, new string[] { "Height" });
            }

            Table.Dispose();

            HeightDataTable.DefaultView.Sort = "Height ASC";
            HeightBindingSource.MoveFirst();
        }

        public void GetWidth()
        {
            DataTable Table = new DataTable();

            WidthDataTable.Clear();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                WidthDataTable = DV.ToTable(true, new string[] { "Width" });
            }

            Table.Dispose();

            WidthDataTable.DefaultView.Sort = "Width ASC";
            WidthBindingSource.MoveFirst();
        }

        public string GetMeasure(int MeasureID)
        {
            DataRow[] Row = MeasuresDataTable.Select("MeasureID = " + MeasureID.ToString());
            return Row[0]["Measure"].ToString();
        }

        public Image GetPicture(int FrontID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FullImage FROM Fronts WHERE FrontID = " + FrontID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["FullImage"] == DBNull.Value)
                        return null;

                    byte[] b = (byte[])DT.Rows[0]["FullImage"];
                    MemoryStream ms = new MemoryStream(b);

                    return Image.FromStream(ms);
                }
            }
        }

        public int SaveFrontAttachments(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int PatinaID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            if (DT.Rows.Count > 1)
            {
                MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }


            int ConfigID = -1;
            int FrontID = Convert.ToInt32(DT.Rows[0]["FrontID"]);
            string SelectCommand = "SELECT TOP 1 * FROM ClientsCatalogFrontsConfig WHERE FrontID=" + FrontID + " AND ColorID=" + ColorID
                + " AND PatinaID=" + PatinaID + " AND TechnoInsetColorID =" + TechnoInsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID
                + " AND TechnoColorID=" + TechnoColorID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID + " ORDER BY ConfigID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT1 = new DataTable())
                    {
                        if (DA.Fill(DT1) > 0)
                            ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                        else
                        {
                            DataRow NewRow = DT1.NewRow();
                            NewRow["FrontID"] = FrontID;
                            NewRow["ColorID"] = ColorID;
                            NewRow["TechnoColorID"] = TechnoColorID;
                            NewRow["InsetTypeID"] = InsetTypeID;
                            NewRow["InsetColorID"] = InsetColorID;
                            NewRow["TechnoInsetTypeID"] = TechnoInsetTypeID;
                            NewRow["TechnoInsetColorID"] = TechnoInsetColorID;
                            NewRow["PatinaID"] = PatinaID;
                            DT1.Rows.Add(NewRow);
                            DA.Update(DT1);
                            DT1.Clear();
                            if (DA.Fill(DT1) > 0)
                                ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                        }
                    }
                }
            }
            return ConfigID;
        }

        public int SaveInsetTypesConfig(string FrontName, int ColorID, int TechnoColorID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND ColorID = " + Convert.ToInt32(ColorID) + " AND TechnoColorID = " + Convert.ToInt32(TechnoColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            //if (DT.Rows.Count > 1)
            //{
            //    MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
            //    return -1;
            //}

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }
            int ConfigID = -1;
            for (int i = DT.Rows.Count - 1; i >= 0; i--)
            {
                int FrontID = Convert.ToInt32(DT.Rows[i]["FrontID"]);
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 1 * FROM InsetTypesConfig ORDER BY ConfigID DESC", ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT1 = new DataTable())
                        {
                            DA.Fill(DT1);
                            DataRow NewRow = DT1.NewRow();
                            NewRow["FrontID"] = FrontID;
                            NewRow["ColorID"] = ColorID;
                            NewRow["TechnoColorID"] = TechnoColorID;
                            DT1.Rows.Add(NewRow);
                            DA.Update(DT1);
                            DT1.Clear();
                            if (DA.Fill(DT1) > 0)
                                ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                        }
                    }
                }
            }
            return ConfigID;
        }

        public int SaveInsetColorsConfig(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            //if (DT.Rows.Count > 1)
            //{
            //    MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
            //    return -1;
            //}

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            int ConfigID = -1;
            for (int i = DT.Rows.Count - 1; i >= 0; i--)
            {
                int FrontID = Convert.ToInt32(DT.Rows[i]["FrontID"]);
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 1 * FROM InsetColorsConfig ORDER BY ConfigID DESC", ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT1 = new DataTable())
                        {
                            DA.Fill(DT1);
                            DataRow NewRow = DT1.NewRow();
                            NewRow["FrontID"] = FrontID;
                            NewRow["ColorID"] = ColorID;
                            NewRow["TechnoColorID"] = TechnoColorID;
                            NewRow["InsetTypeID"] = InsetTypeID;
                            NewRow["InsetColorID"] = InsetColorID;
                            DT1.Rows.Add(NewRow);
                            DA.Update(DT1);
                            DT1.Clear();
                            if (DA.Fill(DT1) > 0)
                                ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                        }
                    }
                }
            }
            return ConfigID;
        }

        public int SaveTechnoInsetColorsConfig(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            //if (DT.Rows.Count > 1)
            //{
            //    MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
            //    return -1;
            //}

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            int ConfigID = -1;
            for (int i = DT.Rows.Count - 1; i >= 0; i--)
            {
                int FrontID = Convert.ToInt32(DT.Rows[i]["FrontID"]);
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 1 * FROM TechnoInsetColorsConfig ORDER BY ConfigID DESC", ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT1 = new DataTable())
                        {
                            DA.Fill(DT1);
                            DataRow NewRow = DT1.NewRow();
                            NewRow["FrontID"] = FrontID;
                            NewRow["ColorID"] = ColorID;
                            NewRow["TechnoColorID"] = TechnoColorID;
                            NewRow["InsetTypeID"] = InsetTypeID;
                            NewRow["InsetColorID"] = InsetColorID;
                            NewRow["TechnoInsetTypeID"] = TechnoInsetTypeID;
                            NewRow["TechnoInsetColorID"] = TechnoInsetColorID;
                            DT1.Rows.Add(NewRow);
                            DA.Update(DT1);
                            DT1.Clear();
                            if (DA.Fill(DT1) > 0)
                                ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                        }
                    }
                }
            }
            return ConfigID;
        }

        public int GetFrontAttachments(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int PatinaID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            if (DT.Rows.Count > 1)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            int ConfigID = -1;

            int FrontID = Convert.ToInt32(DT.Rows[0]["FrontID"]);
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ClientsCatalogFrontsConfig
                WHERE FrontID = " + FrontID + " AND ColorID = " + ColorID + " AND TechnoColorID = " + TechnoColorID +
                " AND InsetTypeID = " + InsetTypeID + " AND InsetColorID = " + InsetColorID +
                " AND TechnoInsetTypeID = " + TechnoInsetTypeID + " AND TechnoInsetColorID = " + TechnoInsetColorID + " AND PatinaID = " + PatinaID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA.Fill(DT1) > 0)
                        ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                }
            }
            return ConfigID;
        }

        public int GetInsetTypesConfig(string FrontName, int ColorID, int TechnoColorID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND ColorID = " + Convert.ToInt32(ColorID) + " AND TechnoColorID = " + Convert.ToInt32(TechnoColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            //if (DT.Rows.Count > 1)
            //{
            //    //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
            //    return -1;
            //}

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            int ConfigID = -1;

            int FrontID = Convert.ToInt32(DT.Rows[0]["FrontID"]);
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM InsetTypesConfig
                WHERE FrontID = " + FrontID + " AND ColorID = " + ColorID + " AND TechnoColorID = " + TechnoColorID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA.Fill(DT1) > 0)
                        ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                }
            }
            return ConfigID;
        }

        public int GetInsetColorConfig(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            if (DT.Rows.Count > 1)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            int ConfigID = -1;

            int FrontID = Convert.ToInt32(DT.Rows[0]["FrontID"]);
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM InsetColorsConfig
                WHERE FrontID = " + FrontID + " AND ColorID = " + ColorID + " AND TechnoColorID = " + TechnoColorID +
                " AND InsetTypeID = " + InsetTypeID + " AND InsetColorID = " + InsetColorID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA.Fill(DT1) > 0)
                        ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                }
            }
            return ConfigID;
        }

        public int GetTechnoInsetColorConfig(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            if (DT.Rows.Count > 1)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            int ConfigID = -1;

            int FrontID = Convert.ToInt32(DT.Rows[0]["FrontID"]);
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM TechnoInsetColorsConfig
                WHERE FrontID = " + FrontID + " AND ColorID = " + ColorID + " AND TechnoColorID = " + TechnoColorID +
                " AND InsetTypeID = " + InsetTypeID + " AND InsetColorID = " + InsetColorID + " AND TechnoInsetTypeID = " + TechnoInsetTypeID + " AND TechnoInsetColorID = " + TechnoInsetColorID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA.Fill(DT1) > 0)
                        ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                }
            }
            return ConfigID;
        }

        public string GetFrontDescription(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int PatinaID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "Description" });
            }

            if (DT.Rows.Count > 1)
            {
                MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return string.Empty;
            }

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return string.Empty;
            }

            string Description = DT.Rows[0]["Description"].ToString();
            return Description;
        }
        public int GetFrontID(string FrontName, int ColorID, int TechnoColorID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int PatinaID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID), string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            if (DT.Rows.Count > 1)
            {
                MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            if (DT.Rows.Count == 0)
            {
                //MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог");
                return -1;
            }

            int FrontID = Convert.ToInt32(DT.Rows[0]["FrontID"]);
            return FrontID;
        }
        //3e47ad8cfb34d1369320afb51e61bd2b
        public Image GetFrontConfigImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages" +
                " WHERE ProductType=0 AND ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteClientsCatalogImage(ConfigID);
                        return null;
                    }

                }
            }
        }

        public Image GetTechStoreImage(int TechStoreID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments" +
                " WHERE DocType = 0 AND TechID = " + TechStoreID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteTechStoreDocument(TechStoreID);
                        return null;
                    }
                }
            }
        }

        public Image GetInsetTypeImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypesImages" +
                " WHERE ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsPath + FileManager.GetPath("InsetTypesImages") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("InsetTypesImages") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteInsetTypesImage(ConfigID);
                        return null;
                    }
                }
            }
        }

        public Image GetInsetColorImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColorsImages" +
                " WHERE ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsPath + FileManager.GetPath("InsetColorsImages") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("InsetColorsImages") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteInsetColorsImage(ConfigID);
                        return null;
                    }
                }
            }
        }

        public Image GetTechnoInsetColorImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechnoInsetColorsImages" +
                " WHERE ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsPath + FileManager.GetPath("TechnoInsetColorsImages") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("TechnoInsetColorsImages") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteTechnoInsetColorsImage(ConfigID);
                        return null;
                    }
                }
            }
        }

        DataTable InsetTypesConfigDT;
        DataTable InsetColorsConfigDT;
        DataTable TechnoInsetColorsConfigDT;

        Bitmap layer;
        Bitmap backgr;
        Bitmap backgr2;
        Bitmap newBitmap;
        Bitmap newBitmap2;
        Bitmap NewImage;

        public bool CreateFotoFromVisualConfig(int FrontID, int ColorID, int TechnoColorID, int PatinaID, int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID,
            string Category, string Name, string Color, string PatinaName, string InsetType, string InsetColor)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM dbo.InsetTypesConfig WHERE FrontID=" + FrontID + " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID,
                ConnectionStrings.CatalogConnectionString))
            {
                InsetTypesConfigDT.Clear();
                DA.Fill(InsetTypesConfigDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM dbo.InsetColorsConfig WHERE FrontID=" + FrontID + " AND ColorID=" + ColorID
                + " AND TechnoInsetColorID=" + TechnoInsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID
                + " AND TechnoColorID=" + TechnoColorID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID,
                ConnectionStrings.CatalogConnectionString))
            {
                InsetColorsConfigDT.Clear();
                DA.Fill(InsetColorsConfigDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM dbo.TechnoInsetColorsConfig WHERE FrontID=" + FrontID + " AND ColorID=" + ColorID
                + " AND TechnoInsetColorID=" + TechnoInsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID
                + " AND TechnoColorID=" + TechnoColorID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID,
                ConnectionStrings.CatalogConnectionString))
            {
                TechnoInsetColorsConfigDT.Clear();
                DA.Fill(TechnoInsetColorsConfigDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM dbo.ClientsCatalogFrontsConfig",
                ConnectionStrings.CatalogConnectionString))
            {
                ClientsCatalogFrontsConfigDT.Clear();
                DA.Fill(ClientsCatalogFrontsConfigDT);
            }

            layer = null;
            backgr = null;
            backgr2 = null;
            newBitmap = null;
            newBitmap2 = null;
            NewImage = null;
            DataRow[] fRows = ClientsCatalogFrontsConfigDT.Select("FrontID=" + FrontID + " AND ColorID=" + ColorID
                + " AND PatinaID=" + PatinaID + " AND TechnoInsetColorID=" + TechnoInsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID
                + " AND TechnoColorID=" + TechnoColorID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID);
            if (fRows.Count() == 0)
            {
                fRows = InsetTypesConfigDT.Select("FrontID=" + FrontID + " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID);
                if (fRows.Count() > 0)
                {
                    int ConfigID = Convert.ToInt32(fRows[0]["ConfigID"]);
                    Image img = GetInsetTypeImage(ConfigID);
                    if (ConfigID != -1 && img != null)
                        layer = new Bitmap(img);
                }
                fRows = InsetColorsConfigDT.Select("FrontID=" + FrontID + " AND ColorID=" + ColorID
                    + " AND TechnoInsetColorID=" + TechnoInsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID
                    + " AND TechnoColorID=" + TechnoColorID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID);
                if (fRows.Count() > 0)
                {
                    int ConfigID = Convert.ToInt32(fRows[0]["ConfigID"]);
                    Image img = GetInsetColorImage(ConfigID);
                    if (ConfigID != -1 && img != null)
                        backgr = new Bitmap(img);
                }
                fRows = TechnoInsetColorsConfigDT.Select("FrontID=" + FrontID + " AND ColorID=" + ColorID
                    + " AND TechnoInsetColorID=" + TechnoInsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID
                    + " AND TechnoColorID=" + TechnoColorID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID);
                if (fRows.Count() > 0)
                {
                    int ConfigID = Convert.ToInt32(fRows[0]["ConfigID"]);
                    Image img = GetTechnoInsetColorImage(ConfigID);
                    if (ConfigID != -1 && img != null)
                        backgr2 = new Bitmap(img);
                }

                if (layer != null)
                {
                    if (backgr != null)
                    {
                        if (backgr2 != null)
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            int width2 = backgr2.Width;
                            int height2 = backgr2.Height;
                            newBitmap = new Bitmap(width, height);
                            newBitmap2 = new Bitmap(width1, height1);
                            using (var canvas = Graphics.FromImage(newBitmap2))
                            {
                                if (height2 >= height1)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr2, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr2.Width, backgr2.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width1, height1), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }

                                int width3 = newBitmap2.Width;
                                int height3 = newBitmap2.Height;
                                using (var canvas1 = Graphics.FromImage(newBitmap))
                                {
                                    if (height3 >= height)
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                    else
                                    {
                                        canvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        canvas1.DrawImage(newBitmap2, new Rectangle(0, 0, width, height), new Rectangle(0, 0, newBitmap2.Width, newBitmap2.Height), GraphicsUnit.Pixel);
                                        canvas1.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                        canvas1.Save();
                                    }
                                }
                            }

                            try
                            {
                                NewImage = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            int width = layer.Width;
                            int height = layer.Height;
                            int width1 = backgr.Width;
                            int height1 = backgr.Height;
                            newBitmap = new Bitmap(width, height);
                            using (var canvas = Graphics.FromImage(newBitmap))
                            {
                                if (height1 >= height)
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                                else
                                {
                                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    canvas.DrawImage(backgr, new Rectangle(0, 0, width, height), new Rectangle(0, 0, backgr.Width, backgr.Height), GraphicsUnit.Pixel);
                                    canvas.DrawImage(layer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, layer.Width, layer.Height), GraphicsUnit.Pixel);
                                    canvas.Save();
                                }
                            }

                            try
                            {
                                NewImage = newBitmap;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                    }
                    else
                        NewImage = layer;
                }
                else
                {
                    if (backgr != null)
                        NewImage = backgr;
                }
            }

            if (NewImage != null)
            {
                string sFileName = Name + " " + Color;

                if (PatinaID != -1 && PatinaName != "-")
                {
                    sFileName += " " + PatinaName;
                    if (InsetType != "-")
                    {
                        sFileName += " " + InsetType;
                        if (InsetColor != "-")
                            sFileName += " " + InsetColor;
                    }
                }
                else
                {
                    if (InsetType != "-")
                    {
                        sFileName += " " + InsetType;
                        if (InsetColor != "-")
                            sFileName += " " + InsetColor;
                    }
                }
                string FileName = sFileName;
                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                int j = 1;
                while (File.Exists(tempFolder + @"\" + sFileName + ".jpg"))
                {
                    sFileName = FileName + "(" + j++ + ")";
                }

                sFileName = sFileName.Replace("\"", "");
                sFileName = sFileName.Replace("*", "");
                sFileName = sFileName.Replace("|", "");
                sFileName = sFileName.Replace(@"\", "");
                sFileName = sFileName.Replace(":", "");
                sFileName = sFileName.Replace("<", "");
                sFileName = sFileName.Replace(">", "");
                sFileName = sFileName.Replace("?", "");
                sFileName = sFileName.Replace("/", "");
                sFileName += ".jpg";
                NewImage.Save(tempFolder + @"\" + sFileName);
                var fileInfo = new System.IO.FileInfo(tempFolder + @"\" + sFileName);

                AttachmentsDT.Clear();
                DataRow NewRow = AttachmentsDT.NewRow();
                NewRow["FileName"] = Path.GetFileNameWithoutExtension(tempFolder + @"\" + sFileName);
                NewRow["Extension"] = fileInfo.Extension;
                NewRow["Path"] = tempFolder + @"\" + sFileName;
                AttachmentsDT.Rows.Add(NewRow);

                int ConfigID = SaveFrontAttachments(Name, ColorID, TechnoColorID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, PatinaID);
                if (ConfigID != -1)
                    AttachConfigImage(AttachmentsDT, ConfigID, 0, Category, Name, Color, PatinaName);
            }
            else
                return false;
            return true;
        }

        public void Fuck1()
        {
            ClientsCatalogFrontsConfigDT = new DataTable();
            ClientsCatalogDecorConfigDT = new DataTable();
            ClientsCatalogImagesDT = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT Fronts.TechStoreName AS Name, Colors.TechStoreName AS Color, Patina.PatinaName, 
                InsetTypes.TechStoreName AS InsetType, InsetColors.TechStoreName AS InsetColor, TechnoInsetTypes.TechStoreName AS TechnoInsetType,
                TechnoInsetColors.TechStoreName AS TechnoInsetColor, ClientsCatalogFrontsConfig.*
                FROM dbo.ClientsCatalogFrontsConfig LEFT OUTER JOIN
                dbo.TechStore AS Fronts ON dbo.ClientsCatalogFrontsConfig.FrontID = Fronts.TechStoreID LEFT OUTER JOIN
                dbo.TechStore AS Colors ON dbo.ClientsCatalogFrontsConfig.ColorID = Colors.TechStoreID LEFT OUTER JOIN
                dbo.Patina AS Patina ON dbo.ClientsCatalogFrontsConfig.PatinaID = Patina.PatinaID LEFT OUTER JOIN
                dbo.TechStore AS InsetTypes ON dbo.ClientsCatalogFrontsConfig.InsetTypeID = InsetTypes.TechStoreID LEFT OUTER JOIN
                dbo.TechStore AS InsetColors ON dbo.ClientsCatalogFrontsConfig.InsetColorID = InsetColors.TechStoreID LEFT OUTER JOIN
                dbo.TechStore AS TechnoInsetTypes ON dbo.ClientsCatalogFrontsConfig.TechnoInsetTypeID = TechnoInsetTypes.TechStoreID LEFT OUTER JOIN
                dbo.TechStore AS TechnoInsetColors ON dbo.ClientsCatalogFrontsConfig.TechnoInsetColorID = TechnoInsetColors.TechStoreID",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ClientsCatalogFrontsConfigDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT dbo.DecorProducts.ProductName, Decor.TechStoreName AS Name, 
                Colors.TechStoreName AS Color, dbo.Patina.PatinaName, ClientsCatalogDecorConfig.*
                FROM ClientsCatalogDecorConfig INNER JOIN
                dbo.DecorProducts ON dbo.ClientsCatalogDecorConfig.ProductID = dbo.DecorProducts.ProductID INNER JOIN
                dbo.TechStore AS Decor ON dbo.ClientsCatalogDecorConfig.DecorID = Decor.TechStoreID LEFT OUTER JOIN
                dbo.TechStore AS Colors ON dbo.ClientsCatalogDecorConfig.ColorID = Colors.TechStoreID INNER JOIN
                dbo.Patina ON dbo.ClientsCatalogDecorConfig.PatinaID = dbo.Patina.PatinaID",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ClientsCatalogDecorConfigDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(ClientsCatalogImagesDT);

                    foreach (DataRow Row in ClientsCatalogImagesDT.Rows)
                    {
                        int ConfigID = Convert.ToInt32(Row["ConfigID"]);
                        int ProductType = Convert.ToInt32(Row["ProductType"]);

                        //Fronts
                        if (ProductType == 0)
                        {
                            DataRow[] fRows = ClientsCatalogFrontsConfigDT.Select("ConfigID=" + ConfigID);
                            if (fRows.Count() > 0)
                            {
                                string Name = fRows[0]["Name"].ToString();
                                string Color = fRows[0]["Color"].ToString();
                                string PatinaName = fRows[0]["PatinaName"].ToString();

                                Row["Category"] = "";
                                Row["Name"] = Name;
                                Row["Color"] = Color;
                                if (PatinaName != "-")
                                    Row["Color"] = Color + " " + PatinaName;
                            }
                        }
                        //Decor
                        if (ProductType == 1)
                        {
                            DataRow[] dRows = ClientsCatalogDecorConfigDT.Select("ConfigID=" + ConfigID);
                            if (dRows.Count() > 0)
                            {
                                string ProductName = dRows[0]["ProductName"].ToString();
                                string Name = dRows[0]["Name"].ToString();
                                string Color = dRows[0]["Color"].ToString();
                                string PatinaName = dRows[0]["PatinaName"].ToString();

                                Row["Category"] = ProductName;
                                Row["Name"] = Name;
                                Row["Color"] = Color;
                                if (PatinaName != "-")
                                    Row["Color"] = Color + " " + PatinaName;
                            }
                        }
                    }
                    DA.Update(ClientsCatalogImagesDT);
                }
            }
        }

        //public bool Fuck(string sPath, string sFileName)
        //{
        //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sPath);
        //    request.Method = WebRequestMethods.Ftp.DownloadFile;
        //    request.UseBinary = true;
        //    request.KeepAlive = false;
        //    request.UsePassive = false;

        //    string HostFTPLogin = "infiniu2";
        //    string HostFTPPass = "infinium1q2w";
        //    request.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
        //    FtpWebResponse response;
        //    Stream responseStream;

        //    try
        //    {
        //        response = (FtpWebResponse)request.GetResponse();

        //        responseStream = response.GetResponseStream();
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    System.Drawing.Image Image;

        //    try
        //    {
        //        Image = System.Drawing.Image.FromStream(responseStream);
        //        response.Close();
        //    }
        //    catch
        //    {
        //        return false;
        //    }


        //    InfiniumStart.RotateFlipEXIF(ref Image);

        //    //size change
        //    int iH = 400;
        //    int iW = 400;

        //    int bH = 0;
        //    int bW = 0;

        //    if (Image.Width > Image.Height)
        //    {
        //        bW = 400;
        //        bH = Convert.ToInt32(Convert.ToDecimal(iW) / (Convert.ToDecimal(Image.Width) / Convert.ToDecimal(Image.Height)));
        //    }
        //    else
        //    {
        //        bH = 400;
        //        bW = Convert.ToInt32(Convert.ToDecimal(iH) / (Convert.ToDecimal(Image.Height) / Convert.ToDecimal(Image.Width)));
        //    }

        //    Bitmap BMP = new Bitmap(bW, bH);

        //    using (Graphics gr = Graphics.FromImage(BMP))
        //    {
        //        BMP.SetResolution(gr.DpiX, gr.DpiY);

        //        gr.SmoothingMode = SmoothingMode.HighQuality;
        //        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //        gr.DrawImage(Image, new Rectangle(0, 0, bW, bH));
        //        string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
        //        BMP.Save(tempFolder + @"\" + sFileName);
        //    }
        //    return true;
        //}

        public bool CreateThumb(string sPath, string sFileName)
        {
            System.Drawing.Image Image;

            try
            {
                Image = Image.FromFile(sPath);
            }
            catch
            {
                return false;
            }


            InfiniumStart.RotateFlipEXIF(ref Image);

            //size change
            int iH = 400;
            int iW = 400;

            int bH = 0;
            int bW = 0;

            if (Image.Width > Image.Height)
            {
                bW = 400;
                bH = Convert.ToInt32(Convert.ToDecimal(iW) / (Convert.ToDecimal(Image.Width) / Convert.ToDecimal(Image.Height)));
            }
            else
            {
                bH = 400;
                bW = Convert.ToInt32(Convert.ToDecimal(iH) / (Convert.ToDecimal(Image.Height) / Convert.ToDecimal(Image.Width)));
            }

            Bitmap BMP = new Bitmap(bW, bH);

            using (Graphics gr = Graphics.FromImage(BMP))
            {
                BMP.SetResolution(gr.DpiX, gr.DpiY);

                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(Image, new Rectangle(0, 0, bW, bH));
                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                if (!Directory.Exists(tempFolder + @"\Thumbs"))
                    Directory.CreateDirectory(tempFolder + @"\Thumbs");
                BMP.Save(tempFolder + @"\Thumbs\" + sFileName);
            }
            return true;
        }

        public bool AttachConfigImage(DataTable AttachmentsDataTable, int ConfigID, int ProductType, string Category, string Name, string Color, string PatinaName)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType=0 AND ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            string sDestFolder = Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages");
                            string sExtension = Row["Extension"].ToString();
                            string sFileName = Row["FileName"].ToString();

                            int j = 1;
                            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                            {
                                sFileName = Row["FileName"].ToString() + "(" + j++ + ")";
                            }
                            Row["FileName"] = sFileName + sExtension;
                            if (FM.UploadFile(Row["Path"].ToString(), sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                                break;
                            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                            if (CreateThumb(Row["Path"].ToString(), sFileName + sExtension))
                            {
                                if (FM.UploadFile(tempFolder + @"\Thumbs\" + sFileName + sExtension, sDestFolder + "/Thumbs/" + sFileName + sExtension, Configs.FTPType) == false)
                                    break;
                            }
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientsCatalogImages", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["ProductType"] = ProductType;
                            NewRow["ConfigID"] = ConfigID;
                            NewRow["Category"] = Category;
                            NewRow["Name"] = Name;
                            NewRow["Color"] = Color;
                            if (PatinaName != "-")
                                NewRow["Color"] = Color + " " + PatinaName;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            return Ok;
        }

        public void DetachConfigImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType=0 AND ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bOk = false;
                        foreach (DataRow Row in DT.Rows)
                        {
                            bOk = FM.DeleteFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                            bOk = FM.DeleteFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/Thumbs/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        if (bOk)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM ClientsCatalogFrontsConfig WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteTechnoInsetColorsImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM TechnoInsetColorsImages WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteInsetColorsImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM InsetColorsImages WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteInsetTypesImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM InsetTypesImages WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteClientsCatalogImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM ClientsCatalogImages WHERE ProductType=0 AND ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteTechStoreDocument(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM TechStoreDocuments WHERE DocType = 0 AND TechID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public bool AttachInsetTypeImage(DataTable AttachmentsDataTable, int ConfigID)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypesImages WHERE ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("InsetTypesImages");
                            string sExtension = Row["Extension"].ToString();
                            string sFileName = Row["FileName"].ToString();

                            int j = 1;
                            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                            {
                                sFileName = Row["FileName"].ToString() + "(" + j++ + ")";
                            }
                            Row["FileName"] = sFileName + sExtension;
                            if (FM.UploadFile(Row["Path"].ToString(), sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                                break;
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM InsetTypesImages", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["ConfigID"] = ConfigID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            return Ok;
        }

        public void DetachInsetTypeImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM InsetTypesImages WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bOk = false;
                        foreach (DataRow Row in DT.Rows)
                        {
                            bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("InsetTypesImages") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        if (bOk)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM InsetTypesConfig WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public bool AttachInsetColorImage(DataTable AttachmentsDataTable, int ConfigID)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColorsImages WHERE ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("InsetColorsImages");
                            string sExtension = Row["Extension"].ToString();
                            string sFileName = Row["FileName"].ToString();

                            int j = 1;
                            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                            {
                                sFileName = Row["FileName"].ToString() + "(" + j++ + ")";
                            }
                            Row["FileName"] = sFileName + sExtension;
                            if (FM.UploadFile(Row["Path"].ToString(), sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                                break;
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM InsetColorsImages", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["ConfigID"] = ConfigID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            return Ok;
        }

        public bool AttachTechnoInsetColorImage(DataTable AttachmentsDataTable, int ConfigID)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechnoInsetColorsImages WHERE ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("TechnoInsetColorsImages");
                            string sExtension = Row["Extension"].ToString();
                            string sFileName = Row["FileName"].ToString();

                            int j = 1;
                            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                            {
                                sFileName = Row["FileName"].ToString() + "(" + j++ + ")";
                            }
                            Row["FileName"] = sFileName + sExtension;
                            if (FM.UploadFile(Row["Path"].ToString(), sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                                break;
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechnoInsetColorsImages", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["ConfigID"] = ConfigID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            return Ok;
        }

        public void DetachInsetColorImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM InsetColorsImages WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bOk = false;
                        foreach (DataRow Row in DT.Rows)
                        {
                            bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("InsetColorsImages") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        if (bOk)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM InsetColorsConfig WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DetachTechnoInsetColorImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM TechnoInsetColorsImages WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bOk = false;
                        foreach (DataRow Row in DT.Rows)
                        {
                            //FM.DeleteFile(Configs.DocumentsPath + "Документооборот/Комментарии/" + Row["FileName"].ToString(), Configs.FTPType);
                            bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("TechnoInsetColorsImages") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        if (bOk)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM TechnoInsetColorsConfig WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public bool IsConfigImageToSite(int ConfigID, ref bool bLatest, ref string Category, ref string Name, ref string Description, ref string Sizes, ref string Material)
        {
            bool b = false;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType=0 AND ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        b = Convert.ToBoolean(DT.Rows[0]["ToSite"]);
                        bLatest = Convert.ToBoolean(DT.Rows[0]["Latest"]);
                        Category = DT.Rows[0]["Category"].ToString();
                        Name = DT.Rows[0]["Name"].ToString();
                        Description = DT.Rows[0]["Description"].ToString();
                        Sizes = DT.Rows[0]["Sizes"].ToString();
                        Material = DT.Rows[0]["Material"].ToString();
                    }
                }
            }
            return b;
        }

        public void ConfigImageToSite(int ConfigID, bool bToSite, bool bLatest, string Category, string Name, string Description, string Sizes, string Material)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType=0 AND ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ToSite"] = bToSite;
                            DT.Rows[0]["Latest"] = bLatest;
                            if (Category.Length == 0)
                                DT.Rows[0]["Category"] = DBNull.Value;
                            else
                                DT.Rows[0]["Category"] = Category;
                            if (Name.Length == 0)
                                DT.Rows[0]["Name"] = DBNull.Value;
                            else
                                DT.Rows[0]["Name"] = Name;
                            if (Description.Length == 0)
                                DT.Rows[0]["Description"] = DBNull.Value;
                            else
                                DT.Rows[0]["Description"] = Description;
                            if (Sizes.Length == 0)
                                DT.Rows[0]["Sizes"] = DBNull.Value;
                            else
                                DT.Rows[0]["Sizes"] = Sizes;
                            if (Material.Length == 0)
                                DT.Rows[0]["Material"] = DBNull.Value;
                            else
                                DT.Rows[0]["Material"] = Material;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SetPicture(Image Picture, int ID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontID, Picture, FullImage FROM Fronts WHERE FrontID = " + ID, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (Picture != null)
                        {
                            MemoryStream ms = new MemoryStream();
                            MemoryStream msFull = new MemoryStream();

                            ImageCodecInfo ImageCodecInfo = UserProfile.GetEncoderInfo("image/jpeg");
                            System.Drawing.Imaging.Encoder eEncoder1 = System.Drawing.Imaging.Encoder.Compression;
                            System.Drawing.Imaging.Encoder eEncoder2 = System.Drawing.Imaging.Encoder.Quality;


                            System.Drawing.Imaging.Encoder eEncoderFull = System.Drawing.Imaging.Encoder.Quality;
                            EncoderParameter myEncoderParameterFull = new EncoderParameter(eEncoderFull, 100L);
                            EncoderParameters myEncoderParametersFull = new EncoderParameters(1);
                            myEncoderParametersFull.Param[0] = myEncoderParameterFull;


                            EncoderParameter myEncoderParameter1 = new EncoderParameter(eEncoder1, (long)EncoderValue.CompressionLZW);
                            EncoderParameter myEncoderParameter2 = new EncoderParameter(eEncoder2, 40L);
                            EncoderParameters myEncoderParameters = new EncoderParameters(2);

                            myEncoderParameters.Param[0] = myEncoderParameter1;
                            myEncoderParameters.Param[1] = myEncoderParameter2;


                            Picture.Save(ms, ImageCodecInfo, myEncoderParameters);

                            DT.Rows[0]["Picture"] = ms.ToArray();
                            ms.Dispose();

                            Picture.Save(msFull, ImageCodecInfo, myEncoderParametersFull);

                            DT.Rows[0]["FullImage"] = msFull.ToArray();
                            msFull.Dispose();
                        }
                        else
                        {
                            DT.Rows[0]["Picture"] = DBNull.Value;
                            DT.Rows[0]["FullImage"] = DBNull.Value;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void FilterFronts()
        {
            DataTable TempItemsDataTable = ConstFrontsDataTable.Copy();
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                TempItemsDataTable = DV.ToTable(true, new string[] { "FrontName" });
            }

            FrontsDataTable.Clear();
            for (int d = 0; d < TempItemsDataTable.Rows.Count; d++)
            {
                DataRow NewRow = FrontsDataTable.NewRow();
                NewRow["FrontName"] = TempItemsDataTable.Rows[d]["FrontName"].ToString();
                FrontsDataTable.Rows.Add(NewRow);
            }
            FrontsDataTable.DefaultView.Sort = "FrontName ASC";
            TempItemsDataTable.Dispose();
        }

        public void FilterCatalogFrameColors(string FrontName = "")
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;

                TempFrontsConfigDataTable = DV.ToTable();

                GetFrameColors();
            }
        }//фильтрует и заполняет цвета профиля по выбранному фасаду

        public void FilterCatalogTechnoFrameColors(string FrontName = "", int ColorID = -1) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoFrameColors();
            }
        }

        public void FilterCatalogPatina(string FrontName = "", int ColorID = -1, int TechnoColorID = -1,
            int InsetTypeID = -1, int InsetColorID = -1, int TechnoInsetTypeID = -1, int TechnoInsetColorID = -1) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
                DV.RowFilter += " AND TechnoInsetColorID=" + TechnoInsetColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetPatina();
            }
        }

        public void FilterCatalogInsetTypes(string FrontName = "",
            int ColorID = -1, int TechnoColorID = -1) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetInsetTypes();
            }
        }

        public void FilterCatalogInsetColors(string FrontName = "", int ColorID = -1,
            int TechnoColorID = -1, int InsetTypeID = -1) //фильтрует и заполняет цвета наполнителя по выбранному типу наполнителя
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetInsetColors();
            }
        }

        public void FilterCatalogTechnoInsetTypes(string FrontName = "", int ColorID = -1,
            int TechnoColorID = -1, int InsetTypeID = -1, int InsetColorID = -1) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoInsetTypes();
            }
        }

        public void FilterCatalogTechnoInsetColors(string FrontName = "", int ColorID = -1, int TechnoColorID = -1,
            int InsetTypeID = -1, int InsetColorID = -1, int TechnoInsetTypeID = -1) //фильтрует и заполняет цвета наполнителя по выбранному типу наполнителя
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoInsetColors();
            }
        }

        public void FilterCatalogHeight(string FrontName = "", int ColorID = -1, int TechnoColorID = -1,
            int InsetTypeID = -1, int InsetColorID = -1, int TechnoInsetTypeID = -1, int TechnoInsetColorID = -1, int PatinaID = -1)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
                DV.RowFilter += " AND TechnoInsetColorID=" + TechnoInsetColorID;

                TempFrontsConfigDataTable = DV.ToTable();
            }

            GetHeight();
            HeightBindingSource.DataSource = HeightDataTable;
        }

        public void FilterCatalogWidth(string FrontName = "", int ColorID = -1, int TechnoColorID = -1,
            int InsetTypeID = -1, int InsetColorID = -1, int TechnoInsetTypeID = -1, int TechnoInsetColorID = -1, int PatinaID = -1, int Height = -1)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable;

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
                DV.RowFilter += " AND TechnoInsetColorID=" + TechnoInsetColorID;
                DV.RowFilter += " AND Height=" + Height;

                TempFrontsConfigDataTable = DV.ToTable();
            }

            GetWidth();

            WidthBindingSource.DataSource = WidthDataTable;
        }

        public void FilterCatalogMarketingPrice(string FrontName = "", int ColorID = -1, int TechnoColorID = -1, int PatinaID = -1,
            int InsetTypeID = -1, int InsetColorID = -1, int TechnoInsetTypeID = -1, int TechnoInsetColorID = -1, int Height = -1, int Width = -1)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
                DV.RowFilter += " AND TechnoInsetColorID=" + TechnoInsetColorID;
                DV.RowFilter += " AND Height=" + Height;
                DV.RowFilter += " AND Width=" + Width;

                TempFrontsConfigDataTable = DV.ToTable();
            }

            GetMarketingPrice();
        }

        public void FilterCatalogZOVRetailPrice(string FrontName = "", int ColorID = -1, int TechnoColorID = -1, int PatinaID = -1,
            int InsetTypeID = -1, int InsetColorID = -1, int TechnoInsetTypeID = -1, int TechnoInsetColorID = -1, int Height = -1, int Width = -1)
        {
            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            GetZOVPrice(GetFrontConfigID(FrontName, ColorID, TechnoColorID, PatinaID, InsetTypeID,
                InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width));
        }

        private bool HasHeightParameter(DataRow[] Rows, int Height)
        {
            foreach (DataRow Row in Rows)
            {
                if (Row["Height"].ToString() == Height.ToString())
                    return true;
            }

            return false;
        }

        private bool HasWidthParameter(DataRow[] Rows, int Width)
        {
            foreach (DataRow Row in Rows)
            {
                if (Row["Width"].ToString() == Width.ToString())
                    return true;
            }

            return false;
        }

        public int GetFrontConfigID(string FrontName, int ColorID, int TechnoColorID, int PatinaID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID,
            int Height, int Width)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            string HeightFilter = null;
            string WidthFilter = null;

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] Rows = ConstFrontsConfigDataTable.Select(
                            filter + " AND " +
                            "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                            "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                            "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                            "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                            "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                            "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                            "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID));

            if (HasHeightParameter(Rows, Height))
                HeightFilter = " AND Height = " + Height;
            else
                HeightFilter = " AND Height = 0";

            if (Height == -1)
                HeightFilter = " AND Height = -1";

            if (HasWidthParameter(Rows, Width))
                WidthFilter = " AND Width = " + Width;
            else
                WidthFilter = " AND Width = 0";

            if (Width == -1)
                WidthFilter = " AND Width = -1";

            Rows = ConstFrontsConfigDataTable.Select(
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID) +
                HeightFilter + WidthFilter);

            if (Rows.Count() < 1)
            {
                return -1;
            }

            return Convert.ToInt32(Rows[0]["FrontConfigID"]);
        }

        public int GetFrontConfigID(string FrontName, int ColorID, int TechnoColorID, int PatinaID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID,
            int Height, int Width, ref int FrontID, ref int FactoryID)
        {
            TempFrontsDataTable.Clear();
            TempFrontsDataTable = ConstFrontsDataTable;
            using (DataView DV = new DataView(TempFrontsDataTable))
            {
                DV.RowFilter = "FrontName='" + FrontName + "'";

                TempFrontsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempFrontsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempFrontsDataTable.Rows[i]["FrontID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "FrontID IN (" + filter + ")";
            }
            else
                filter = "FrontID <> - 1";

            string HeightFilter = null;
            string WidthFilter = null;

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] Rows = ConstFrontsConfigDataTable.Select(
                            filter + " AND " +
                            "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                            "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                            "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                            "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                            "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                            "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                            "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID));

            if (HasHeightParameter(Rows, Height))
                HeightFilter = " AND Height = " + Height;
            else
                HeightFilter = " AND Height = 0";

            if (Height == -1)
                HeightFilter = " AND Height = -1";

            if (HasWidthParameter(Rows, Width))
                WidthFilter = " AND Width = " + Width;
            else
                WidthFilter = " AND Width = 0";

            if (Width == -1)
                WidthFilter = " AND Width = -1";

            Rows = ConstFrontsConfigDataTable.Select(
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID) +
                HeightFilter + WidthFilter);

            if (Rows.Count() < 1)
            {
                MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог " + Rows.Count().ToString());
                return -1;
            }

            //if (Rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(Rows[0]["Excluzive"]) == 0)
            //{
            //    MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
            //    return -1;
            //}
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND " +
                "InsetTypeID = " + Convert.ToInt32(InsetTypeID) + " AND " +
                "ColorID = " + Convert.ToInt32(ColorID) + " AND " +
                "PatinaID = " + Convert.ToInt32(PatinaID) + " AND " +
                "InsetColorID = " + Convert.ToInt32(InsetColorID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID) +
                HeightFilter + WidthFilter, string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID" });
            }

            if (DT.Rows.Count > 1)
            {
                MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог " + Rows.Count().ToString());
                return -1;
            }

            FrontID = Convert.ToInt32(DT.Rows[0]["FrontID"]);
            FactoryID = Convert.ToInt32(Rows[0]["FactoryID"]);

            return Convert.ToInt32(Rows[0]["FrontConfigID"]);
        }

    }



    public class DecorCatalog
    {
        public FileManager FM = new FileManager();
        int FactoryID = 1;
        public int DecorProductsCount = 0;

        public DataTable ConstProductsDataTable = null;
        public DataTable ConstDecorDataTable = null;
        public DataTable ConstColorsDataTable = null;
        public DataTable ConstPatinaDataTable = null;
        public DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;

        public DataTable DecorConfigDataTable = null;
        public DataTable DecorParametersDataTable = null;

        public DataTable TempProductsDataTable = null;
        public DataTable TempItemsDataTable = null;

        public DataTable ProductsDataTable = null;
        public DataTable ItemsDataTable = null;
        public DataTable ItemColorsDataTable = null;
        public DataTable ItemPatinaDataTable = null;
        public DataTable ItemLengthDataTable = null;
        public DataTable ItemHeightDataTable = null;
        public DataTable ItemWidthDataTable = null;
        public DataTable ItemInsetTypesDataTable = null;
        public DataTable ItemInsetColorsDataTable = null;

        public DataTable MeasuresDataTable = null;
        public DataTable MarketingPriceDataTable = null;
        public DataTable ZOVPriceDataTable = null;

        public BindingSource DecorProductsBindingSource = null;
        public BindingSource DecorItemBindingSource = null;
        public BindingSource ItemColorsBindingSource = null;
        public BindingSource ItemPatinaBindingSource = null;
        public BindingSource ColorsBindingSource = null;
        public BindingSource PatinaBindingSource = null;
        public BindingSource ItemInsetTypesBindingSource = null;
        public BindingSource ItemInsetColorsBindingSource = null;
        public BindingSource LengthBindingSource = null;
        public BindingSource HeightBindingSource = null;
        public BindingSource WidthBindingSource = null;
        public BindingSource MarketingPriceBindingSource = null;
        public BindingSource ZOVPriceBindingSource = null;

        public String DecorProductsBindingSourceDisplayMember = null;
        public String ItemsBindingSourceDisplayMember = null;
        public String ItemColorsBindingSourceDisplayMember = null;
        public String ItemPatinaBindingSourceDisplayMember = null;
        public String ItemLengthBindingSourceDisplayMember = null;
        public String ItemHeightBindingSourceDisplayMember = null;
        public String ItemWidthBindingSourceDisplayMember = null;

        public String DecorProductsBindingSourceValueMember = null;
        public String ItemsBindingSourceValueMember = null;
        public String ItemColorsBindingSourceValueMember = null;
        public String ItemPatinaBindingSourceValueMember = null;

        public DecorCatalog(int tFactoryID)
        {
            FactoryID = tFactoryID;
            Initialize();
        }

        private void Create()
        {
            ConstProductsDataTable = new DataTable();
            ConstDecorDataTable = new DataTable();
            DecorParametersDataTable = new DataTable();
            DecorConfigDataTable = new DataTable();

            MeasuresDataTable = new DataTable();
            ItemColorsDataTable = new DataTable();
            ItemPatinaDataTable = new DataTable();
            ConstColorsDataTable = new DataTable();
            ConstPatinaDataTable = new DataTable();
            ItemsDataTable = new DataTable();
            ItemLengthDataTable = new DataTable();
            ItemHeightDataTable = new DataTable();
            ItemWidthDataTable = new DataTable();

            MarketingPriceDataTable = new DataTable();
            MarketingPriceDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            MarketingPriceDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            ZOVPriceDataTable = new DataTable();
            ZOVPriceDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            ZOVPriceDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            DecorProductsBindingSource = new BindingSource();
            ItemColorsBindingSource = new BindingSource();
            ItemPatinaBindingSource = new BindingSource();
            DecorItemBindingSource = new BindingSource();
            LengthBindingSource = new BindingSource();
            HeightBindingSource = new BindingSource();
            WidthBindingSource = new BindingSource();
            ColorsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            MarketingPriceBindingSource = new BindingSource();
            ZOVPriceBindingSource = new BindingSource();
        }

        public DataTable GetBagetDT()
        {
            DataTable DecorDT = new DataTable();
            using (DataView DV = new DataView(DecorConfigDataTable, "ProductID=17 AND FactoryID=1", string.Empty, DataViewRowState.CurrentRows))
            {
                DecorDT = DV.ToTable(true, new string[] { "DecorID", "ColorID", "Width", "DecorConfigID" });
            }
            DecorDT.Columns.Add(new DataColumn("Product", Type.GetType("System.String")));
            DecorDT.Columns.Add(new DataColumn("Decor", Type.GetType("System.String")));
            DecorDT.Columns.Add(new DataColumn("Color", Type.GetType("System.String")));
            DecorDT.Columns.Add(new DataColumn("Length", Type.GetType("System.String")));
            DecorDT.Columns.Add(new DataColumn("Height", Type.GetType("System.String")));
            DecorDT.Columns.Add(new DataColumn("PositionsCount", Type.GetType("System.String")));
            DecorDT.Columns.Add(new DataColumn("LabelsCount", Type.GetType("System.Int32")));
            DecorDT.Columns.Add(new DataColumn("FactoryType", Type.GetType("System.Int32")));
            for (int i = 0; i < DecorDT.Rows.Count; i++)
            {
                DecorDT.Rows[i]["Product"] = "Профиль";
                DecorDT.Rows[i]["Decor"] = GetItemName(Convert.ToInt32(DecorDT.Rows[i]["DecorID"]));
                DecorDT.Rows[i]["Color"] = GetColorName(Convert.ToInt32(DecorDT.Rows[i]["ColorID"]));
                DecorDT.Rows[i]["Length"] = 150;
                DecorDT.Rows[i]["Height"] = -1;
                DecorDT.Rows[i]["PositionsCount"] = 1;
                DecorDT.Rows[i]["LabelsCount"] = 1;
                DecorDT.Rows[i]["FactoryType"] = 1;
            }
            using (DataView DV = new DataView(DecorDT.Copy(), string.Empty, "Decor, Color", DataViewRowState.CurrentRows))
            {
                DecorDT.Clear();
                DecorDT = DV.ToTable();
            }
            return DecorDT;
        }

        private void GetColorsDT()
        {
            ConstColorsDataTable = new DataTable();
            ConstColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ConstColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = ConstColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        ConstColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = ConstColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        ConstColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ConstColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ConstColorsDataTable.Rows.Add(NewRow);
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
            string SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL AND FactoryID=" + FactoryID + "))) ORDER BY ProductName ASC";
            ConstProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstProductsDataTable);
            }
            ConstDecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1  AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL AND FactoryID=" + FactoryID + " ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstDecorDataTable);
            }
            GetColorsDT();
            GetInsetColorsDT();
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }

            DecorProductsCount = ConstProductsDataTable.Rows.Count;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstPatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = ConstPatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                ConstPatinaDataTable.Rows.Add(NewRow);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }

            using (DataView DV = new DataView(ConstDecorDataTable))
            {
                ItemsDataTable = DV.ToTable(true, new string[] { "Name" });
            }
            ProductsDataTable = ConstProductsDataTable.Clone();
            TempItemsDataTable = ConstDecorDataTable.Clone();
            ItemColorsDataTable = ConstColorsDataTable.Clone();
            ItemPatinaDataTable = ConstPatinaDataTable.Clone();
            ItemInsetTypesDataTable = InsetTypesDataTable.Clone();
            ItemInsetColorsDataTable = InsetColorsDataTable.Clone();

            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorConfig " +
            //    " WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL", ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDataTable);
            //}
            DecorConfigDataTable = TablesManager.DecorConfigDataTable;

            //string connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\DB\;Extended Properties=dBASE IV;";
            //string SelectCommad = "SELECT * FROM C.dbf";

            //using (System.Data.OleDb.OleDbDataAdapter DA = new System.Data.OleDb.OleDbDataAdapter(SelectCommad, connString))
            //{
            //    using (DataTable DT = new DataTable())
            //    {
            //        DA.Fill(DT);
            //        decimal Amount = 0;
            //        if (DT.Rows.Count > 0)
            //            Amount = Convert.ToDecimal(DT.Rows[1]["Amount"]);
            //    }
            //}
        }

        public void FilterCatalog(int FactoryID)
        {
            string SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL AND FactoryID=" + FactoryID + "))) ORDER BY ProductName ASC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                ConstProductsDataTable.Clear();
                DA.Fill(ConstProductsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1  AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL AND FactoryID=" + FactoryID + " ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                ConstDecorDataTable.Clear();
                DA.Fill(ConstDecorDataTable);
            }
        }

        private void Binding()
        {
            DecorProductsBindingSource.DataSource = ProductsDataTable;
            DecorItemBindingSource.DataSource = ItemsDataTable;
            ItemColorsBindingSource.DataSource = ItemColorsDataTable;
            ItemPatinaBindingSource.DataSource = ItemPatinaDataTable;
            LengthBindingSource.DataSource = ItemLengthDataTable;
            HeightBindingSource.DataSource = ItemHeightDataTable;
            WidthBindingSource.DataSource = ItemWidthDataTable;
            ColorsBindingSource.DataSource = ConstColorsDataTable;
            PatinaBindingSource.DataSource = ConstPatinaDataTable;
            ItemInsetTypesBindingSource = new BindingSource()
            {
                DataSource = ItemInsetTypesDataTable
            };
            ItemInsetColorsBindingSource = new BindingSource()
            {
                DataSource = ItemInsetColorsDataTable
            };
            MarketingPriceBindingSource.DataSource = MarketingPriceDataTable;
            ZOVPriceBindingSource.DataSource = ZOVPriceDataTable;

            DecorProductsBindingSourceDisplayMember = "ProductName";
            ItemsBindingSourceDisplayMember = "Name";
            ItemColorsBindingSourceDisplayMember = "ColorName";
            ItemPatinaBindingSourceDisplayMember = "PatinaName";
            ItemLengthBindingSourceDisplayMember = "Length";
            ItemHeightBindingSourceDisplayMember = "Height";
            ItemWidthBindingSourceDisplayMember = "Width";

            DecorProductsBindingSourceValueMember = "ProductID";
            ItemsBindingSourceValueMember = "DecorID";
            ItemColorsBindingSourceValueMember = "ColorID";
            ItemPatinaBindingSourceValueMember = "PatinaID";
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();

            CreateLengthDataTable();
            CreateHeightDataTable();
            CreateWidthDataTable();
        }

        public int CountDecorProducts
        {
            get { return DecorProductsCount; }
        }

        private bool HasLengthParameter(DataRow[] Rows, int Length)
        {
            //DataRow[] Rows = DecorConfigDataTable.Select("DecorID = " + DecorID);

            foreach (DataRow Row in Rows)
            {
                if (Row["Length"].ToString() == Length.ToString())
                    return true;
            }

            return false;
        }

        private bool HasHeightParameter(DataRow[] Rows, int Height)
        {
            //DataRow[] Rows = DecorConfigDataTable.Select("DecorID = " + DecorID);

            foreach (DataRow Row in Rows)
            {
                if (Row["Height"].ToString() == Height.ToString())
                    return true;
            }

            return false;
        }

        private bool HasWidthParameter(DataRow[] Rows, int Width)
        {
            //DataRow[] Rows = DecorConfigDataTable.Select("DecorID = " + DecorID);

            foreach (DataRow Row in Rows)
            {
                if (Row["Width"].ToString() == Width.ToString())
                    return true;
            }

            return false;
        }

        //External
        public bool HasParameter(int ProductID, String Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }

        public int GetDecorConfigID(int ProductID, string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            string LengthFilter = null;
            string HeightFilter = null;
            string WidthFilter = null;
            string ColorFilter = null;
            string PatinaFilter = null;
            string InsetTypeFilter = null;
            string InsetColorFilter = null;

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] Rows = DecorConfigDataTable.Select(
                            "ProductID = " + Convert.ToInt32(ProductID) + " AND " + filter);

            ColorFilter = " AND ColorID = " + ColorID;
            PatinaFilter = " AND PatinaID = " + PatinaID;
            InsetTypeFilter = " AND InsetTypeID = " + InsetTypeID;
            InsetColorFilter = " AND InsetColorID = " + InsetColorID;

            if (HasLengthParameter(Rows, Length))
                LengthFilter = " AND Length = " + Length;
            else
                LengthFilter = " AND Length = 0";

            if (Length == -1)
                LengthFilter = " AND Length = -1";

            if (HasHeightParameter(Rows, Height))
                HeightFilter = " AND Height = " + Height;
            else
                HeightFilter = " AND Height = 0";

            if (Height == -1)
                HeightFilter = " AND Height = -1";

            if (HasWidthParameter(Rows, Width))
                WidthFilter = " AND Width = " + Width;
            else
                WidthFilter = " AND Width = 0";

            if (Width == -1)
                WidthFilter = " AND Width = -1";


            Rows = DecorConfigDataTable.Select(
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " + filter +
                                ColorFilter + PatinaFilter + InsetTypeFilter + InsetColorFilter + LengthFilter + HeightFilter + WidthFilter);

            if (Rows.Count() < 1 || Rows.Count() > 1)
            {
                return -1;
            }

            return Convert.ToInt32(Rows[0]["DecorConfigID"]);
        }

        public string GetItemName(int DecorID)
        {
            return ConstDecorDataTable.Select("DecorID = " + DecorID)[0]["Name"].ToString();
        }

        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = ConstColorsDataTable.Select("ColorID = " + ColorID);
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
                DataRow[] Rows = ConstPatinaDataTable.Select("PatinaID = " + PatinaID);
                PatinaName = Rows[0]["DisplayName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return PatinaName;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            string InsetTypeName = string.Empty;
            try
            {
                DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
                InsetTypeName = Rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetTypeName;
        }

        public string GetInsetColorName(int InsetColorID)
        {
            string InsetColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + InsetColorID);
                InsetColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetColorName;
        }

        public string GetMeasure(int MeasureID)
        {
            DataRow[] Row = MeasuresDataTable.Select("MeasureID = " + MeasureID.ToString());
            return Row[0]["Measure"].ToString();
        }

        public bool GetHeightParameterName(int ProductID)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID=" + ProductID);

            if (Rows.Count() != 0)
            {
                if (Convert.ToBoolean(Rows[0]["Height"]) == true)
                    return false;
                if (Convert.ToBoolean(Rows[0]["Length"]) == true)
                    return true;
            }

            return false;
        }

        public Image GetPicture(int DecorID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FullImage FROM Decor WHERE DecorID = " + DecorID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["FullImage"] == DBNull.Value)
                        return null;

                    byte[] b = (byte[])DT.Rows[0]["FullImage"];
                    MemoryStream ms = new MemoryStream(b);

                    return Image.FromStream(ms);
                }
            }
        }

        public Image GetDecorProductPicture(int ProductID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FullImage FROM DecorProducts WHERE ProductID = " + ProductID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["FullImage"] == DBNull.Value)
                        return null;

                    byte[] b = (byte[])DT.Rows[0]["FullImage"];
                    MemoryStream ms = new MemoryStream(b);

                    return Image.FromStream(ms);
                }
            }
        }

        public void SetPicture(Image Picture, int ID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorID, Picture, FullImage FROM Decor WHERE DecorID = " + ID, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (Picture != null)
                        {
                            MemoryStream ms = new MemoryStream();
                            MemoryStream msFull = new MemoryStream();

                            ImageCodecInfo ImageCodecInfo = UserProfile.GetEncoderInfo("image/jpeg");
                            System.Drawing.Imaging.Encoder eEncoder1 = System.Drawing.Imaging.Encoder.Compression;
                            System.Drawing.Imaging.Encoder eEncoder2 = System.Drawing.Imaging.Encoder.Quality;


                            System.Drawing.Imaging.Encoder eEncoderFull = System.Drawing.Imaging.Encoder.Quality;
                            EncoderParameter myEncoderParameterFull = new EncoderParameter(eEncoderFull, 100L);
                            EncoderParameters myEncoderParametersFull = new EncoderParameters(1);
                            myEncoderParametersFull.Param[0] = myEncoderParameterFull;


                            EncoderParameter myEncoderParameter1 = new EncoderParameter(eEncoder1, (long)EncoderValue.CompressionLZW);
                            EncoderParameter myEncoderParameter2 = new EncoderParameter(eEncoder2, 40L);
                            EncoderParameters myEncoderParameters = new EncoderParameters(2);

                            myEncoderParameters.Param[0] = myEncoderParameter1;
                            myEncoderParameters.Param[1] = myEncoderParameter2;


                            Picture.Save(ms, ImageCodecInfo, myEncoderParameters);

                            DT.Rows[0]["Picture"] = ms.ToArray();
                            ms.Dispose();

                            Picture.Save(msFull, ImageCodecInfo, myEncoderParametersFull);

                            DT.Rows[0]["FullImage"] = msFull.ToArray();
                            msFull.Dispose();
                        }
                        else
                        {
                            DT.Rows[0]["Picture"] = DBNull.Value;
                            DT.Rows[0]["FullImage"] = DBNull.Value;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetDecorProductPicture(Image Picture, int ID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ProductID, Picture, FullImage FROM DecorProducts WHERE ProductID = " + ID, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (Picture != null)
                        {
                            MemoryStream ms = new MemoryStream();
                            MemoryStream msFull = new MemoryStream();

                            ImageCodecInfo ImageCodecInfo = UserProfile.GetEncoderInfo("image/jpeg");
                            System.Drawing.Imaging.Encoder eEncoder1 = System.Drawing.Imaging.Encoder.Compression;
                            System.Drawing.Imaging.Encoder eEncoder2 = System.Drawing.Imaging.Encoder.Quality;


                            System.Drawing.Imaging.Encoder eEncoderFull = System.Drawing.Imaging.Encoder.Quality;
                            EncoderParameter myEncoderParameterFull = new EncoderParameter(eEncoderFull, 100L);
                            EncoderParameters myEncoderParametersFull = new EncoderParameters(1);
                            myEncoderParametersFull.Param[0] = myEncoderParameterFull;


                            EncoderParameter myEncoderParameter1 = new EncoderParameter(eEncoder1, (long)EncoderValue.CompressionLZW);
                            EncoderParameter myEncoderParameter2 = new EncoderParameter(eEncoder2, 40L);
                            EncoderParameters myEncoderParameters = new EncoderParameters(2);

                            myEncoderParameters.Param[0] = myEncoderParameter1;
                            myEncoderParameters.Param[1] = myEncoderParameter2;


                            Picture.Save(ms, ImageCodecInfo, myEncoderParameters);

                            DT.Rows[0]["Picture"] = ms.ToArray();
                            ms.Dispose();

                            Picture.Save(msFull, ImageCodecInfo, myEncoderParametersFull);

                            DT.Rows[0]["FullImage"] = msFull.ToArray();
                            msFull.Dispose();
                        }
                        else
                        {
                            DT.Rows[0]["Picture"] = DBNull.Value;
                            DT.Rows[0]["FullImage"] = DBNull.Value;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        private void CreateLengthDataTable()
        {
            ItemLengthDataTable.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
        }

        private void CreateHeightDataTable()
        {
            ItemHeightDataTable.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
        }

        private void CreateWidthDataTable()
        {
            ItemWidthDataTable.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
        }

        public void FilterProducts()
        {
            DataTable TempItemsDataTable = ConstProductsDataTable.Copy();
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                TempItemsDataTable = DV.ToTable();
            }

            ProductsDataTable.Clear();
            for (int d = 0; d < TempItemsDataTable.Rows.Count; d++)
            {
                DataRow NewRow = ProductsDataTable.NewRow();
                NewRow.ItemArray = TempItemsDataTable.Rows[d].ItemArray;
                ProductsDataTable.Rows.Add(NewRow);
            }
            ProductsDataTable.DefaultView.Sort = "ProductName ASC";
            TempItemsDataTable.Dispose();

            DecorProductsBindingSource.MoveFirst();
        }

        public void FilterItems(int ProductID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "ProductID=" + ProductID;
                TempItemsDataTable = DV.ToTable(true, new string[] { "Name" });
            }

            ItemsDataTable.Clear();
            ItemColorsDataTable.Clear();
            ItemPatinaDataTable.Clear();
            ItemInsetTypesDataTable.Clear();
            ItemInsetColorsDataTable.Clear();
            ItemLengthDataTable.Clear();
            ItemHeightDataTable.Clear();
            ItemWidthDataTable.Clear();
            MarketingPriceDataTable.Clear();
            ZOVPriceDataTable.Clear();
            for (int d = 0; d < TempItemsDataTable.Rows.Count; d++)
            {
                DataRow NewRow = ItemsDataTable.NewRow();
                NewRow["Name"] = TempItemsDataTable.Rows[d]["Name"].ToString();
                ItemsDataTable.Rows.Add(NewRow);
            }
            ItemsDataTable.DefaultView.Sort = "Name ASC";
            DecorItemBindingSource.MoveFirst();
            ItemColorsBindingSource.MoveFirst();
            ItemPatinaBindingSource.MoveFirst();
            LengthBindingSource.MoveFirst();
            HeightBindingSource.MoveFirst();
            WidthBindingSource.MoveFirst();
        }

        public bool FilterColors(string Name)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ItemColorsDataTable.Clear();
            ItemColorsDataTable.AcceptChanges();

            DataRow[] DCR = DecorConfigDataTable.Select(filter);


            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    DataRow NewRow = ItemColorsDataTable.NewRow();
                    NewRow["ColorID"] = (DCR[d]["ColorID"]);
                    NewRow["ColorName"] = GetColorName(Convert.ToInt32(DCR[d]["ColorID"]));
                    ItemColorsDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ItemColorsDataTable.Rows.Count; i++)
                {
                    if (ItemColorsDataTable.Rows[i]["ColorID"].ToString() == DCR[d]["ColorID"].ToString())
                        break;

                    if (i == ItemColorsDataTable.Rows.Count - 1)
                    {
                        DataRow NewRow = ItemColorsDataTable.NewRow();
                        NewRow["ColorID"] = (DCR[d]["ColorID"]);
                        NewRow["ColorName"] = GetColorName(Convert.ToInt32(DCR[d]["ColorID"]));
                        ItemColorsDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemColorsDataTable.DefaultView.Sort = "ColorName ASC";
            ItemColorsBindingSource.MoveFirst();
            ItemPatinaBindingSource.MoveFirst();
            LengthBindingSource.MoveFirst();
            HeightBindingSource.MoveFirst();
            WidthBindingSource.MoveFirst();
            //if (ItemColorsDataTable.Rows[0]["ColorID"].ToString() == "0")
            //    return false;

            return true;

        }

        public bool FilterPatina(string Name, int ColorID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ItemPatinaDataTable.Clear();
            ItemPatinaDataTable.AcceptChanges();

            DataRow[] DCR = DecorConfigDataTable.Select(filter + " AND ColorID = " + ColorID.ToString());

            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    DataRow NewRow = ItemPatinaDataTable.NewRow();
                    NewRow["PatinaID"] = (DCR[d]["PatinaID"]);
                    NewRow["PatinaName"] = GetPatinaName(Convert.ToInt32(DCR[d]["PatinaID"]));
                    ItemPatinaDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ItemPatinaDataTable.Rows.Count; i++)
                {
                    if (ItemPatinaDataTable.Rows[i]["PatinaID"].ToString() == DCR[d]["PatinaID"].ToString())
                        break;

                    if (i == ItemPatinaDataTable.Rows.Count - 1)
                    {
                        DataRow NewRow = ItemPatinaDataTable.NewRow();
                        NewRow["PatinaID"] = (DCR[d]["PatinaID"]);
                        NewRow["PatinaName"] = GetPatinaName(Convert.ToInt32(DCR[d]["PatinaID"]));
                        ItemPatinaDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            bool HasPatinaRAL = false;
            if (ItemPatinaDataTable.Rows.Count > 0)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaID=" + Convert.ToInt32(ItemPatinaDataTable.Rows[0]["PatinaID"]));
                if (fRows.Count() > 0)
                    HasPatinaRAL = true;
            }
            if (ItemPatinaDataTable.Rows.Count > 0 && HasPatinaRAL)
            {
                DataTable ddd = ItemPatinaDataTable.Copy();
                ItemPatinaDataTable.Clear();
                ItemPatinaDataTable.AcceptChanges();

                foreach (DataRow Row in ddd.Rows)
                {
                    foreach (DataRow item in PatinaRALDataTable.Select("PatinaID=" + Convert.ToInt32(Row["PatinaID"])))
                    {
                        DataRow NewRow = ItemPatinaDataTable.NewRow();
                        NewRow["PatinaID"] = item["PatinaRALID"];
                        NewRow["PatinaName"] = item["PatinaRAL"];
                        NewRow["DisplayName"] = item["DisplayName"];
                        ItemPatinaDataTable.Rows.Add(NewRow);
                    }
                }
            }

            ItemPatinaDataTable.DefaultView.Sort = "PatinaName ASC";
            ItemPatinaBindingSource.MoveFirst();
            LengthBindingSource.MoveFirst();
            HeightBindingSource.MoveFirst();
            WidthBindingSource.MoveFirst();

            return true;

        }

        public bool FilterInsetType(string Name, int ColorID, int PatinaID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ItemInsetTypesDataTable.Clear();
            ItemInsetTypesDataTable.AcceptChanges();

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] DCR = DecorConfigDataTable.Select(filter + " AND ColorID = " + ColorID.ToString()
                + " AND PatinaID = " + PatinaID.ToString());


            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    DataRow NewRow = ItemInsetTypesDataTable.NewRow();
                    NewRow["InsetTypeID"] = (DCR[d]["InsetTypeID"]);
                    NewRow["InsetTypeName"] = GetInsetTypeName(Convert.ToInt32(DCR[d]["InsetTypeID"]));
                    ItemInsetTypesDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ItemInsetTypesDataTable.Rows.Count; i++)
                {
                    if (ItemInsetTypesDataTable.Rows[i]["InsetTypeID"].ToString() == DCR[d]["InsetTypeID"].ToString())
                        break;

                    if (i == ItemInsetTypesDataTable.Rows.Count - 1)
                    {
                        DataRow NewRow = ItemInsetTypesDataTable.NewRow();
                        NewRow["InsetTypeID"] = (DCR[d]["InsetTypeID"]);
                        NewRow["InsetTypeName"] = GetInsetTypeName(Convert.ToInt32(DCR[d]["InsetTypeID"]));
                        ItemInsetTypesDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemInsetTypesDataTable.DefaultView.Sort = "InsetTypeName ASC";
            ItemInsetTypesBindingSource.MoveFirst();

            if (ItemInsetTypesDataTable.Rows.Count == 1 && ItemInsetTypesDataTable.Rows[0]["InsetTypeID"].ToString() == "-1")
                return false;

            return true;

        }

        public bool FilterInsetColor(string Name, int ColorID, int PatinaID, int InsetTypeID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ItemInsetColorsDataTable.Clear();
            ItemInsetColorsDataTable.AcceptChanges();

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] DCR = DecorConfigDataTable.Select(filter + " AND ColorID = " + ColorID.ToString()
                + " AND PatinaID = " + PatinaID.ToString() + " AND InsetTypeID = " + InsetTypeID.ToString());


            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    DataRow NewRow = ItemInsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = (DCR[d]["InsetColorID"]);
                    NewRow["InsetColorName"] = GetInsetColorName(Convert.ToInt32(DCR[d]["InsetColorID"]));
                    ItemInsetColorsDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ItemInsetColorsDataTable.Rows.Count; i++)
                {
                    if (ItemInsetColorsDataTable.Rows[i]["InsetColorID"].ToString() == DCR[d]["InsetColorID"].ToString())
                        break;

                    if (i == ItemInsetColorsDataTable.Rows.Count - 1)
                    {
                        DataRow NewRow = ItemInsetColorsDataTable.NewRow();
                        NewRow["InsetColorID"] = (DCR[d]["InsetColorID"]);
                        NewRow["InsetColorName"] = GetInsetColorName(Convert.ToInt32(DCR[d]["InsetColorID"]));
                        ItemInsetColorsDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemInsetColorsDataTable.DefaultView.Sort = "InsetColorName ASC";
            ItemInsetColorsBindingSource.MoveFirst();

            if (ItemInsetColorsDataTable.Rows.Count == 1 && ItemInsetColorsDataTable.Rows[0]["InsetColorID"].ToString() == "-1")
                return false;

            return true;

        }

        public DataTable FilterLength(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ItemLengthDataTable.Clear();
            ItemLengthDataTable.AcceptChanges();

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] DCR = DecorConfigDataTable.Select(filter
                + " AND ColorID = " + ColorID.ToString() + " AND PatinaID = " + PatinaID.ToString() + " AND InsetTypeID = " + InsetTypeID.ToString() + " AND InsetColorID = " + InsetColorID.ToString());
            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    int Length = Convert.ToInt32(DCR[d]["Length"]);

                    DataRow NewRow = ItemLengthDataTable.NewRow();
                    NewRow["Length"] = Length;
                    ItemLengthDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ItemLengthDataTable.Rows.Count; i++)
                {
                    if (ItemLengthDataTable.Rows[i]["Length"].ToString() == DCR[d]["Length"].ToString())
                        break;

                    if (i == ItemLengthDataTable.Rows.Count - 1)
                    {
                        int Length = Convert.ToInt32(DCR[d]["Length"]);

                        DataRow NewRow = ItemLengthDataTable.NewRow();
                        NewRow["Length"] = Length;
                        ItemLengthDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemLengthDataTable.DefaultView.Sort = "Length ASC";
            LengthBindingSource.MoveFirst();
            return ItemLengthDataTable;
        }

        public DataTable FilterHeight(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ItemHeightDataTable.Clear();
            ItemHeightDataTable.AcceptChanges();

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] DCR = DecorConfigDataTable.Select(filter + " AND ColorID = " + ColorID.ToString()
                + " AND PatinaID = " + PatinaID.ToString() + " AND InsetTypeID = " + InsetTypeID.ToString() + " AND InsetColorID = " + InsetColorID.ToString() + " AND Length = " + Length.ToString());

            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    int Height = Convert.ToInt32(DCR[d]["Height"]);

                    DataRow NewRow = ItemHeightDataTable.NewRow();
                    NewRow["Height"] = Height;
                    ItemHeightDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ItemHeightDataTable.Rows.Count; i++)
                {
                    if (ItemHeightDataTable.Rows[i]["Height"].ToString() == DCR[d]["Height"].ToString())
                        break;

                    if (i == ItemHeightDataTable.Rows.Count - 1)
                    {
                        int Height = Convert.ToInt32(DCR[d]["Height"]);

                        DataRow NewRow = ItemHeightDataTable.NewRow();
                        NewRow["Height"] = Height;
                        ItemHeightDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemHeightDataTable.DefaultView.Sort = "Height ASC";
            HeightBindingSource.MoveFirst();
            WidthBindingSource.MoveFirst();
            return ItemHeightDataTable;
        }

        public DataTable FilterWidth(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ItemWidthDataTable.Clear();
            ItemWidthDataTable.AcceptChanges();

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] DCR = DecorConfigDataTable.Select(filter
                + " AND ColorID = " + ColorID.ToString() + " AND PatinaID = " + PatinaID.ToString()
                + " AND InsetTypeID = " + InsetTypeID.ToString() + " AND InsetColorID = " + InsetColorID.ToString() +
                " AND Length = " + Length.ToString() + " AND Height = " + Height.ToString());


            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    int Width = Convert.ToInt32(DCR[d]["Width"]);

                    DataRow NewRow = ItemWidthDataTable.NewRow();
                    NewRow["Width"] = Width;
                    ItemWidthDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ItemWidthDataTable.Rows.Count; i++)
                {
                    if (ItemWidthDataTable.Rows[i]["Width"].ToString() == DCR[d]["Width"].ToString())
                        break;

                    if (i == ItemWidthDataTable.Rows.Count - 1)
                    {
                        int Width = Convert.ToInt32(DCR[d]["Width"]);

                        DataRow NewRow = ItemWidthDataTable.NewRow();
                        NewRow["Width"] = Height;
                        ItemWidthDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemWidthDataTable.DefaultView.Sort = "Width ASC";
            WidthBindingSource.MoveFirst();
            HeightBindingSource.MoveFirst();
            WidthBindingSource.MoveFirst();

            return ItemWidthDataTable;
        }

        private void GetDecorPrice(DataRow DecorOrderRow, ref decimal OriginalPrice)
        {
            decimal MarketingCost = 0;//себестоимость
            decimal PriceRatio = 0;//ценовой коэффициент
            int DigitCapacity = 0;//разрядность

            int DecorConfigID = Convert.ToInt32(DecorOrderRow["DecorConfigID"]);
            DataRow[] PRows = DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString());
            if (PRows.Count() > 0)
            {
                MarketingCost = Convert.ToDecimal(PRows[0]["MarketingCost"]);
                PriceRatio = Convert.ToDecimal(PRows[0]["PriceRatio"]);
                DigitCapacity = Convert.ToInt32(PRows[0]["DigitCapacity"]);
                OriginalPrice = MarketingCost * PriceRatio * (1 + 0);
                OriginalPrice = Decimal.Round(OriginalPrice, DigitCapacity, MidpointRounding.AwayFromZero);

                //if (Security.CurrentClientID == 145)
                //    OriginalPrice = Convert.ToDecimal(PRows[0]["ZOVPrice"]);
            }
        }

        public bool FilterCatalogMarketingPrice(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            MarketingPriceDataTable.Clear();
            MarketingPriceDataTable.AcceptChanges();

            DataRow[] DCR = DecorConfigDataTable.Select(filter +
                " AND ColorID = " + ColorID.ToString() + " AND PatinaID = " + PatinaID.ToString() +
                " AND InsetTypeID = " + InsetTypeID.ToString() + " AND InsetColorID = " + InsetColorID.ToString() +
                " AND Length = " + Length.ToString() + " AND Height = " + Height.ToString() +
                " AND Width = " + Width.ToString());
            for (int d = 0; d < DCR.Count(); d++)
            {
                decimal MarketingPrice = 0;
                GetDecorPrice(DCR[d], ref MarketingPrice);
                MarketingPrice = Decimal.Round(MarketingPrice, 2, MidpointRounding.AwayFromZero);
                if (d == 0)
                {
                    DataRow NewRow = MarketingPriceDataTable.NewRow();
                    NewRow["MeasureID"] = (DCR[d]["MeasureID"]);
                    NewRow["Measure"] = MarketingPrice + " " + GetMeasure(Convert.ToInt32((DCR[d]["MeasureID"])));
                    MarketingPriceDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < MarketingPriceDataTable.Rows.Count; i++)
                {
                    if (MarketingPriceDataTable.Rows[i]["MeasureID"].ToString() == DCR[d]["MeasureID"].ToString())
                        break;

                    if (i == MarketingPriceDataTable.Rows.Count - 1)
                    {
                        DataRow NewRow = MarketingPriceDataTable.NewRow();
                        NewRow["MeasureID"] = (DCR[d]["MeasureID"]);
                        NewRow["Measure"] = MarketingPrice + " " + GetMeasure(Convert.ToInt32((DCR[d]["MeasureID"])));
                        MarketingPriceDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            MarketingPriceBindingSource.MoveFirst();
            if (MarketingPriceDataTable.Rows[0]["MeasureID"].ToString() == "0")
                return false;

            return true;
        }

        public bool FilterCatalogZOVPrice(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            ZOVPriceDataTable.Clear();
            ZOVPriceDataTable.AcceptChanges();

            DataRow[] DCR = DecorConfigDataTable.Select(filter +
                " AND ColorID = " + ColorID.ToString() + " AND PatinaID = " + PatinaID.ToString() +
                " AND InsetTypeID = " + InsetTypeID.ToString() + " AND InsetColorID = " + InsetColorID.ToString() +
                " AND Length = " + Length.ToString() + " AND Height = " + Height.ToString() +
                " AND Width = " + Width.ToString());

            for (int d = 0; d < DCR.Count(); d++)
            {
                if (d == 0)
                {
                    DataRow NewRow = ZOVPriceDataTable.NewRow();
                    NewRow["MeasureID"] = (DCR[d]["MeasureID"]);
                    NewRow["Measure"] = Convert.ToDecimal((DCR[d]["ZOVPrice"])) + " " + GetMeasure(Convert.ToInt32((DCR[d]["MeasureID"])));
                    ZOVPriceDataTable.Rows.Add(NewRow);
                    continue;
                }


                for (int i = 0; i < ZOVPriceDataTable.Rows.Count; i++)
                {
                    if (ZOVPriceDataTable.Rows[i]["MeasureID"].ToString() == DCR[d]["MeasureID"].ToString())
                        break;

                    if (i == ZOVPriceDataTable.Rows.Count - 1)
                    {
                        DataRow NewRow = ZOVPriceDataTable.NewRow();
                        NewRow["MeasureID"] = (DCR[d]["MeasureID"]);
                        NewRow["Measure"] = Convert.ToDecimal((DCR[d]["ZOVPrice"])) + " " + GetMeasure(Convert.ToInt32((DCR[d]["MeasureID"])));
                        ZOVPriceDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ZOVPriceBindingSource.MoveFirst();
            if (ZOVPriceDataTable.Rows[0]["MeasureID"].ToString() == "0")
                return false;

            return true;
        }

        public int SaveDecorAttachments(int ProductID, string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            string ColorFilter = null;
            string PatinaFilter = null;
            ColorFilter = " AND ColorID = " + ColorID;
            PatinaFilter = " AND PatinaID = " + PatinaID;
            DataRow[] Rows = DecorConfigDataTable.Select(
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " + filter +
                                ColorFilter + PatinaFilter + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID);

            if (Rows.Count() < 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог");
                return -1;
            }



            int ConfigID = -1;
            int DecorID = Convert.ToInt32(Rows[0]["DecorID"]);
            string SelectCommand = "SELECT TOP 1 * FROM ClientsCatalogDecorConfig WHERE ProductID=" + ProductID + " AND DecorID=" + DecorID
                + " AND ColorID=" + ColorID + " AND PatinaID=" + PatinaID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID + " ORDER BY ConfigID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT1 = new DataTable())
                    {
                        if (DA.Fill(DT1) > 0)
                            ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                        else
                        {
                            DataRow NewRow = DT1.NewRow();
                            NewRow["ProductID"] = ProductID;
                            NewRow["DecorID"] = DecorID;
                            NewRow["ColorID"] = ColorID;
                            NewRow["PatinaID"] = PatinaID;
                            NewRow["InsetTypeID"] = InsetTypeID;
                            NewRow["InsetColorID"] = InsetColorID;
                            DT1.Rows.Add(NewRow);
                            DA.Update(DT1);
                            DT1.Clear();
                            if (DA.Fill(DT1) > 0)
                                ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                        }
                    }
                }
            }
            return ConfigID;
        }

        public int GetDecorAttachments(int ProductID, string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            string ColorFilter = null;
            string PatinaFilter = null;
            ColorFilter = " AND ColorID = " + ColorID;
            PatinaFilter = " AND PatinaID = " + PatinaID;
            DataRow[] Rows = DecorConfigDataTable.Select(
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " + filter +
                                ColorFilter + PatinaFilter + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID);

            if (Rows.Count() < 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог");
                return -1;
            }

            int ConfigID = -1;
            int DecorID = Convert.ToInt32(Rows[0]["DecorID"]);

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ClientsCatalogDecorConfig
                WHERE ProductID = " + ProductID + " AND ColorID = " + ColorID + " AND DecorID = " + DecorID + " AND PatinaID = " + PatinaID + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA.Fill(DT1) > 0)
                        ConfigID = Convert.ToInt32(DT1.Rows[0]["ConfigID"]);
                }
            }
            return ConfigID;
        }

        public int GetDecorID(int ProductID, string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
                filter = "DecorID <> - 1";

            string ColorFilter = null;
            string PatinaFilter = null;
            ColorFilter = " AND ColorID = " + ColorID;
            PatinaFilter = " AND PatinaID = " + PatinaID;
            DataRow[] Rows = DecorConfigDataTable.Select(
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " + filter +
                                ColorFilter + PatinaFilter + " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID);

            if (Rows.Count() < 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог");
                return -1;
            }

            int DecorID = Convert.ToInt32(Rows[0]["DecorID"]);
            return DecorID;
        }

        public void DeleteClientsCatalogImage(int ConfigID, int ProductID)
        {
            string SelectCommand = "DELETE FROM ClientsCatalogImages WHERE ProductType=1 AND ConfigID = " + ConfigID;
            if (CheckOrdersStatus.IsCabFurniture(ProductID))
                SelectCommand = "DELETE FROM ClientsCatalogImages WHERE ProductType=2 AND ConfigID = " + ConfigID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteTechStoreDocument(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM TechStoreDocuments WHERE DocType = 0 AND TechID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public Image GetDecorConfigImage(int ConfigID, int ProductID)
        {
            string SelectCommand = "SELECT * FROM ClientsCatalogImages" +
                " WHERE ProductType=1 AND ConfigID = " + ConfigID;
            if (CheckOrdersStatus.IsCabFurniture(ProductID))
                SelectCommand = "SELECT * FROM ClientsCatalogImages" +
                 " WHERE ProductType=2 AND ConfigID = " + ConfigID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteClientsCatalogImage(ConfigID, ProductID);
                        return null;
                    }
                }
            }
        }

        public Image GetTechStoreImage(int TechStoreID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments" +
                " WHERE DocType = 0 AND TechID = " + TechStoreID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteTechStoreDocument(TechStoreID);
                        return null;
                    }
                }
            }
        }

        public bool CreateThumb(string sPath, string sFileName)
        {
            System.Drawing.Image Image;

            try
            {
                Image = Image.FromFile(sPath);
            }
            catch
            {
                return false;
            }


            InfiniumStart.RotateFlipEXIF(ref Image);

            //size change
            int iH = 400;
            int iW = 400;

            int bH = 0;
            int bW = 0;

            if (Image.Width > Image.Height)
            {
                bW = 400;
                bH = Convert.ToInt32(Convert.ToDecimal(iW) / (Convert.ToDecimal(Image.Width) / Convert.ToDecimal(Image.Height)));
            }
            else
            {
                bH = 400;
                bW = Convert.ToInt32(Convert.ToDecimal(iH) / (Convert.ToDecimal(Image.Height) / Convert.ToDecimal(Image.Width)));
            }

            Bitmap BMP = new Bitmap(bW, bH);

            using (Graphics gr = Graphics.FromImage(BMP))
            {
                BMP.SetResolution(gr.DpiX, gr.DpiY);

                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(Image, new Rectangle(0, 0, bW, bH));
                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                BMP.Save(tempFolder + @"\" + sFileName);
            }
            return true;
        }

        private string GetCabFurName(int ConfigID)
        {
            string TechStoreSubGroupName = string.Empty;
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT dbo.ClientsCatalogDecorConfig.ConfigID, dbo.ClientsCatalogDecorConfig.ProductID, dbo.ClientsCatalogDecorConfig.DecorID, dbo.ClientsCatalogDecorConfig.ColorID, 
                dbo.ClientsCatalogDecorConfig.PatinaID, dbo.TechStoreSubGroups.TechStoreSubGroupName
                FROM dbo.ClientsCatalogDecorConfig INNER JOIN
                dbo.DecorConfig ON dbo.ClientsCatalogDecorConfig.ProductID = dbo.DecorConfig.ProductID AND dbo.ClientsCatalogDecorConfig.DecorID = dbo.DecorConfig.DecorID AND
                dbo.ClientsCatalogDecorConfig.ColorID = dbo.DecorConfig.ColorID AND dbo.ClientsCatalogDecorConfig.PatinaID = dbo.DecorConfig.PatinaID INNER JOIN
                dbo.TechStoreSubGroups ON dbo.DecorConfig.TechStoreSubGroupID = dbo.TechStoreSubGroups.TechStoreSubGroupID WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        TechStoreSubGroupName = DT.Rows[0]["TechStoreSubGroupName"].ToString();
                }
            }
            return TechStoreSubGroupName;
        }

        public bool AttachConfigImage(DataTable AttachmentsDataTable, int ConfigID, int ProductType, string Category, string Name, string Color, string PatinaName, string InsetColor)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType=" + ProductType + " AND ConfigID = " + ConfigID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            string sDestFolder = Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages");
                            string sExtension = Row["Extension"].ToString();
                            string sFileName = Row["FileName"].ToString();

                            int j = 1;
                            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                            {
                                sFileName = Row["FileName"].ToString() + "(" + j++ + ")";
                            }
                            Row["FileName"] = sFileName + sExtension;
                            if (FM.UploadFile(Row["Path"].ToString(), sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                                break;
                            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                            if (CreateThumb(Row["Path"].ToString(), sFileName + sExtension))
                            {
                                if (FM.UploadFile(tempFolder + @"\" + sFileName + sExtension, sDestFolder + "/Thumbs/" + sFileName + sExtension, Configs.FTPType) == false)
                                    break;
                            }
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientsCatalogImages", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["ProductType"] = ProductType;
                            NewRow["Category"] = Category;
                            if (ProductType == 2 || ProductType == 4)
                                NewRow["Category"] = GetCabFurName(ConfigID);
                            NewRow["Name"] = Name;
                            NewRow["Color"] = Color;
                            if (PatinaName != "-")
                                NewRow["Color"] = Color + " " + PatinaName;
                            if (InsetColor != "-")
                                NewRow["Color"] = NewRow["Color"] + " " + InsetColor;
                            NewRow["ConfigID"] = ConfigID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            return Ok;
        }

        public bool IsConfigImageToSite(int ConfigID, ref bool bLatest, ref string Category, ref string Name, ref string Description, ref string Sizes, ref string Material)
        {
            bool b = false;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType<>0 AND ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        b = Convert.ToBoolean(DT.Rows[0]["ToSite"]);
                        bLatest = Convert.ToBoolean(DT.Rows[0]["Latest"]);
                        Category = DT.Rows[0]["Category"].ToString();
                        Description = DT.Rows[0]["Description"].ToString();
                        Name = DT.Rows[0]["Name"].ToString();
                        Sizes = DT.Rows[0]["Sizes"].ToString();
                        Material = DT.Rows[0]["Material"].ToString();
                    }
                }
            }
            return b;
        }

        public void ConfigImageToSite(int ConfigID, bool bToSite, bool bLatest, string Category, string Name, string Description, string Sizes, string Material)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType<>0 AND ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ToSite"] = bToSite;
                            DT.Rows[0]["Latest"] = bLatest;
                            if (Category.Length == 0)
                                DT.Rows[0]["Category"] = DBNull.Value;
                            else
                                DT.Rows[0]["Category"] = Category;
                            if (Description.Length == 0)
                                DT.Rows[0]["Description"] = DBNull.Value;
                            else
                                DT.Rows[0]["Description"] = Description;
                            if (Name.Length == 0)
                                DT.Rows[0]["Name"] = DBNull.Value;
                            else
                                DT.Rows[0]["Name"] = Name;
                            if (Sizes.Length == 0)
                                DT.Rows[0]["Sizes"] = DBNull.Value;
                            else
                                DT.Rows[0]["Sizes"] = Sizes;
                            if (Material.Length == 0)
                                DT.Rows[0]["Material"] = DBNull.Value;
                            else
                                DT.Rows[0]["Material"] = Material;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void DetachConfigImage(int ConfigID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType<>0 AND ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bOk = false;
                        foreach (DataRow Row in DT.Rows)
                        {
                            bOk = FM.DeleteFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                            bOk = FM.DeleteFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/Thumbs/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        if (bOk)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM ClientsCatalogDecorConfig WHERE ConfigID = " + ConfigID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public TechStoreGroupInfo GetSubGroupInfo(int DecorID)
        {
            TechStoreGroupInfo techStoreGroupInfo = new TechStoreGroupInfo();
            string SelectCommand = @"SELECT TOP 1 TSG.*, TS.TechStoreID, TS.TechStoreName, DecorID FROM DecorConfig                
                INNER JOIN TechStoreSubGroups as TSG ON DecorConfig.TechStoreSubGroupID = TSG.TechStoreSubGroupID
                INNER JOIN TechStore as TS ON DecorConfig.DecorID = TS.TechStoreID
                WHERE DecorID = " + DecorID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        techStoreGroupInfo.TechStoreName = DT.Rows[0]["TechStoreName"].ToString();
                        techStoreGroupInfo.TechStoreSubGroupName = DT.Rows[0]["TechStoreSubGroupName"].ToString();
                        techStoreGroupInfo.SubGroupNotes = DT.Rows[0]["Notes"].ToString();
                        techStoreGroupInfo.SubGroupNotes1 = DT.Rows[0]["Notes1"].ToString();
                        techStoreGroupInfo.SubGroupNotes2 = DT.Rows[0]["Notes2"].ToString();
                    }
                }
            }
            return techStoreGroupInfo;
        }

        public int GetDecorIdByName(string Name)
        {
            int DecorID = -1;
            TempItemsDataTable.Clear();
            TempItemsDataTable = ConstDecorDataTable;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                DV.RowFilter = "Name='" + Name + "'";

                TempItemsDataTable = DV.ToTable();
            }
            string filter = string.Empty;
            if (TempItemsDataTable.Rows.Count > 0)
                DecorID = Convert.ToInt32(TempItemsDataTable.Rows[0]["DecorID"]);

            return DecorID;

        }

        public struct TechStoreGroupInfo
        {
            public string TechStoreName;
            public string TechStoreSubGroupName;
            public string SubGroupNotes;
            public string SubGroupNotes1;
            public string SubGroupNotes2;
        }
    }




    public class FinishedImagesCatalog
    {
        public FileManager FM = new FileManager();

        DataTable ClientsCatalogImagesDT = null;
        public BindingSource ClientsCatalogImagesBS = null;

        public FinishedImagesCatalog()
        {
            ClientsCatalogImagesDT = new DataTable();
            ClientsCatalogImagesBS = new BindingSource();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientsCatalogImages", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ClientsCatalogImagesDT);
            }
            ClientsCatalogImagesBS.DataSource = ClientsCatalogImagesDT;
        }

        public void MoveToImage(int ImageID)
        {
            ClientsCatalogImagesBS.Position = ClientsCatalogImagesBS.Find("ImageID", ImageID);
        }

        public void NewImageRow()
        {
            DataRow NewRow = ClientsCatalogImagesDT.NewRow();
            NewRow["ProductType"] = 3;
            NewRow["ConfigID"] = -1;
            ClientsCatalogImagesDT.Rows.Add(NewRow);
        }

        public void EditImageRowBeforeSaving(int ImageID, bool ToSite, bool CatSlider, bool MainSlider, string Category, string Name, string Color, string Description, string Sizes, string Material)
        {
            DataRow[] rows = ClientsCatalogImagesDT.Select("ImageID = " + ImageID);
            if (rows.Count() > 0)
            {
                rows[0]["Category"] = Category;
                rows[0]["Name"] = Name;
                rows[0]["Color"] = Color;
                rows[0]["Description"] = Description;
                rows[0]["Sizes"] = Sizes;
                rows[0]["Material"] = Material;
                rows[0]["ToSite"] = ToSite;
                rows[0]["CatSlider"] = CatSlider;
                rows[0]["MainSlider"] = MainSlider;
            }
        }

        public void SaveImages()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientsCatalogImages", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ClientsCatalogImagesDT);
                }
            }
        }

        public void UpdateImages()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ProductType=3", ConnectionStrings.CatalogConnectionString))
            {
                ClientsCatalogImagesDT.Clear();
                DA.Fill(ClientsCatalogImagesDT);
            }
        }

        public bool CreateThumb(string sPath, string sFileName)
        {
            System.Drawing.Image Image;

            try
            {
                Image = Image.FromFile(sPath);
            }
            catch
            {
                return false;
            }


            InfiniumStart.RotateFlipEXIF(ref Image);

            //size change
            int iH = 400;
            int iW = 400;

            int bH = 0;
            int bW = 0;

            if (Image.Width > Image.Height)
            {
                bW = 400;
                bH = Convert.ToInt32(Convert.ToDecimal(iW) / (Convert.ToDecimal(Image.Width) / Convert.ToDecimal(Image.Height)));
            }
            else
            {
                bH = 400;
                bW = Convert.ToInt32(Convert.ToDecimal(iH) / (Convert.ToDecimal(Image.Height) / Convert.ToDecimal(Image.Width)));
            }

            Bitmap BMP = new Bitmap(bW, bH);

            using (Graphics gr = Graphics.FromImage(BMP))
            {
                BMP.SetResolution(gr.DpiX, gr.DpiY);

                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(Image, new Rectangle(0, 0, bW, bH));
                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                if (!Directory.Exists(tempFolder + @"\Thumbs"))
                    Directory.CreateDirectory(tempFolder + @"\Thumbs");
                BMP.Save(tempFolder + @"\Thumbs\" + sFileName);
            }
            return true;
        }

        public bool AttachImage(int ImageID, string FileName, string Extension, string sPath, long FileSize)
        {
            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ImageID = " + ImageID, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            try
                            {
                                string sDestFolder = Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages");
                                string sExtension = Extension;
                                string sFileName = FileName;

                                int j = 1;
                                while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                                {
                                    sFileName = FileName + "(" + j++ + ")";
                                }

                                DT.Rows[0]["FileName"] = sFileName + sExtension;
                                DT.Rows[0]["FileSize"] = FileSize;
                                if (FM.UploadFile(sPath, sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                                    return false;
                                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                                if (CreateThumb(sPath, sFileName + sExtension))
                                {
                                    if (FM.UploadFile(tempFolder + @"\Thumbs\" + sFileName + sExtension, sDestFolder + "/Thumbs/" + sFileName + sExtension, Configs.FTPType) == false)
                                        return false;
                                }
                            }
                            catch
                            {
                                return false;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
            return true;
        }

        public void DeleteImageRow(int ImageID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages WHERE ImageID = " + ImageID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0]["FileName"] != DBNull.Value)
                            {
                                FM.DeleteFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + DT.Rows[0]["FileName"].ToString(), Configs.FTPType);
                                FM.DeleteFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/Thumbs/" + DT.Rows[0]["FileName"].ToString(), Configs.FTPType);
                            }
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public Image GetImage(int ImageID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsCatalogImages" +
                " WHERE ImageID = " + ImageID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    if (FM.FileExist(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + FileName, Configs.FTPType))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(
                                FM.ReadFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("ClientsCatalogImages") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                            {
                                return Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        DeleteClientsCatalogImage(ImageID);
                        return null;
                    }
                }
            }
        }

        public void DeleteClientsCatalogImage(int ImageID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM ClientsCatalogImages WHERE ImageID = " + ImageID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

    }



    public struct SampleInfo
    {
        public DataTable OrderData;
        public int ProductType;
        public int FactoryType;
        public string DocDateTime;
        public string BarcodeNumber;
        public string Impost;
    }


    public class SampleLabel
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;
        //public int PaperWidth = 794;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font ClientFont;
        Font DocFont;
        Font InfoFont;
        Font NotesFont;
        Font HeaderFont;
        Font FrontOrderFont;
        Font DecorOrderFont;
        Font DispatchFont;

        Pen Pen;


        Image ZTTPS;
        Image ZTProfil;
        Image STB;
        Image RST;

        public ArrayList LabelInfo;




        public SampleLabel()
        {
            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            ZTProfil = new Bitmap(Properties.Resources.ZOVPROFIL);
            STB = new Bitmap(Properties.Resources.STB);
            RST = new Bitmap(Properties.Resources.RST);
            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            ClientFont = new Font("Arial", 18.0f, FontStyle.Regular);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 16.0f, FontStyle.Regular);
            HeaderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        public string GetBarcodeNumber(int BarcodeType, int PackNumber)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = "";
            if (PackNumber.ToString().Length == 1)
                Number = "00000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 2)
                Number = "0000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 3)
                Number = "000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 4)
                Number = "00000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 5)
                Number = "0000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 6)
                Number = "000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 7)
                Number = "00" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 8)
                Number = "0" + PackNumber.ToString();

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

        private void DrawTable(PrintPageEventArgs ev, int LabelNumber, int margin)
        {
            float HeaderTopY = 70 + margin;
            float OrderTopY = 83 + margin;
            float TopLineY = 63 + margin;
            float BottomLineY = 112 + margin;

            //float HeaderTopY = 151;
            //float OrderTopY = 164;
            //float TopLineY = 144;
            //float BottomLineY = 315;

            //ev.Graphics.DrawLine(Pen, 11, 33, 467, 33);
            //ev.Graphics.DrawLine(Pen, 11, 67, 467, 67);

            //ev.Graphics.DrawLine(Pen, 11, 144, 467, 144);

            //ev.Graphics.DrawLine(Pen, 371, 33, 371, 67);


            float VertLine1 = 11;
            float VertLine2 = 65;
            float VertLine3 = 178;
            float VertLine3_1 = 178;
            float VertLine4 = 303;
            float VertLine5 = 307;
            //float VertLine6 = 379;
            //float VertLine7 = 409;
            float VertLine8 = 409;
            float VertLine9 = 467;
            float factor = 1;
            if (((SampleInfo)LabelInfo[LabelNumber]).ProductType == 0)//фасады
            {
                ev.Graphics.DrawLine(Pen, 11, 63 + margin, 467, 63 + margin);
                ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[0]["Front"].ToString() + ((SampleInfo)LabelInfo[LabelNumber]).Impost, DocFont, FontBrush, 7, 33 + margin);
                //header

                int MaxStringLenth1 = ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[0]["FrameColor"].ToString().Length;
                int MaxStringLenth2 = ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[0]["InsetType"].ToString().Length;
                int MaxStringLenth3 = ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[0]["InsetColor"].ToString().Length;
                for (int i = 0, p = 10; i < ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["FrameColor"].ToString().Length > MaxStringLenth1)
                        MaxStringLenth1 = ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["FrameColor"].ToString().Length;
                    if (((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["InsetType"].ToString().Length > MaxStringLenth2)
                        MaxStringLenth2 = ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["InsetType"].ToString().Length;
                    if (((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["InsetColor"].ToString().Length > MaxStringLenth3)
                        MaxStringLenth3 = ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["InsetColor"].ToString().Length;
                }
                if (MaxStringLenth1 == 0 && MaxStringLenth2 == 0 && MaxStringLenth3 == 0)
                    factor = 0;
                else
                    factor = (VertLine8 - VertLine1) / (MaxStringLenth1 + MaxStringLenth2 + MaxStringLenth3);
                VertLine3 = VertLine1 + factor * MaxStringLenth1;
                if (VertLine3 < 120)
                    VertLine3 = 120;
                if (VertLine3 > 220)
                    VertLine3 = 220;
                VertLine4 = VertLine3 + factor * MaxStringLenth2;
                if (VertLine3 == 220)
                    VertLine4 = 280;
                if (VertLine4 > 280)
                    VertLine4 = 280;

                //ev.Graphics.DrawString("Тип", HeaderFont, FontBrush, VertLine1 + 1, HeaderTopY);
                ev.Graphics.DrawString("Цвет профиля", HeaderFont, FontBrush, VertLine1 + 1, HeaderTopY);
                ev.Graphics.DrawString("Вставка", HeaderFont, FontBrush, VertLine3 + 1, HeaderTopY);
                ev.Graphics.DrawString("Цвет наполнителя", HeaderFont, FontBrush, VertLine4 + 1, HeaderTopY);
                ev.Graphics.DrawString("Кол-во", HeaderFont, FontBrush, VertLine8 + 1, HeaderTopY);

                ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
                //ev.Graphics.DrawLine(Pen, VertLine2, TopLineY, VertLine2, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine3, TopLineY, VertLine3, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine4, TopLineY, VertLine4, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine8, TopLineY, VertLine8, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

                for (int i = 0, p = 88 + margin; i <= ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (i != 6)
                        ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                }

                for (int i = 0, p = 10; i < ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    //ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["FrontType"].ToString(),
                    //    FrontOrderFont, FontBrush, VertLine1 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["FrameColor"].ToString(),
                        FrontOrderFont, FontBrush, VertLine1 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["InsetType"].ToString(),
                        FrontOrderFont, FontBrush, VertLine3 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["InsetColor"].ToString(),
                        FrontOrderFont, FontBrush, VertLine4 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["PositionsCount"].ToString(),
                        FrontOrderFont, FontBrush, VertLine8 + 1, OrderTopY + p);
                }
            }
            else
            {
                VertLine1 = 11;
                VertLine2 = 145;
                VertLine3 = 290;
                VertLine3_1 = 330;
                VertLine4 = 370;
                VertLine5 = 410;
                ev.Graphics.DrawLine(Pen, 11, 63 + margin, 467, 63 + margin);
                ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[0]["Product"].ToString(), DocFont, FontBrush, 7, 33 + margin);
                ////header
                //ev.Graphics.DrawString("Продукт", HeaderFont, FontBrush, VertLine1 + 1, HeaderTopY);
                ev.Graphics.DrawString("Наименование", HeaderFont, FontBrush, VertLine1 + 1, HeaderTopY);
                ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, VertLine2 + 1, HeaderTopY);
                ev.Graphics.DrawString("Длин.", HeaderFont, FontBrush, VertLine3 + 1, HeaderTopY);
                ev.Graphics.DrawString("Выс.", HeaderFont, FontBrush, VertLine3_1 + 1, HeaderTopY);
                ev.Graphics.DrawString("Шир.", HeaderFont, FontBrush, VertLine4 + 1, HeaderTopY);
                ev.Graphics.DrawString("Кол-во", HeaderFont, FontBrush, VertLine5 + 1, HeaderTopY);

                ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine2, TopLineY, VertLine2, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine3, TopLineY, VertLine3, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine3_1, TopLineY, VertLine3_1, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine4, TopLineY, VertLine4, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine5, TopLineY, VertLine5, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

                for (int i = 0, p = 88 + margin; i <= ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (i != 6)
                        ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                }

                for (int i = 0, p = 10; i < ((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["Decor"].ToString(),
                        FrontOrderFont, FontBrush, VertLine1 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["Color"].ToString(),
                        FrontOrderFont, FontBrush, VertLine2 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["Length"].ToString(),
                        FrontOrderFont, FontBrush, VertLine3 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["Height"].ToString(),
                        FrontOrderFont, FontBrush, VertLine3_1 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["Width"].ToString(),
                        FrontOrderFont, FontBrush, VertLine4 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((SampleInfo)LabelInfo[LabelNumber]).OrderData.Rows[i]["PositionsCount"].ToString(),
                        FrontOrderFont, FontBrush, VertLine5 + 1, OrderTopY + p);
                }
            }
        }

        public void ClearOrderData()
        {
            if (LabelInfo.Count > 0)
                ((SampleInfo)LabelInfo[CurrentLabelNumber]).OrderData.Clear();
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref SampleInfo LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;
            int margin = 0;
            ev.Graphics.Clear(Color.White);

            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            //OrderData
            DrawTable(ev, CurrentLabelNumber, margin);

            if (((SampleInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
            {
                ev.Graphics.DrawImage(ZTTPS, 249, 120 + margin, 37, 45);
            }
            else
            {
                ev.Graphics.DrawImage(ZTProfil, 249, 120 + margin, 37, 45);
            }

            ev.Graphics.DrawImage(STB, 418, 119 + margin, 39, 27);
            ev.Graphics.DrawImage(RST, 423, 157 + margin, 34, 27);

            ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.ZOV.Barcode.BarcodeLength.Medium, 46, ((SampleInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 120);

            Barcode.DrawBarcodeText(Modules.Packages.ZOV.Barcode.BarcodeLength.Medium, ev.Graphics, ((SampleInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 170);

            ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, 120 + margin);

            if (((SampleInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 132 + margin);
            else
                ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 132 + margin);

            ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 144 + margin);
            ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, 156 + margin);
            ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 168 + margin);
            ev.Graphics.DrawString("Изготовлено: " + ((SampleInfo)LabelInfo[CurrentLabelNumber]).DocDateTime, InfoFont, FontBrush, 305, 180);

            ev.Graphics.DrawLine(Pen, 11, 205, 467, 205);
            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                margin = 200;
                DrawTable(ev, CurrentLabelNumber + 1, margin);

                if (((SampleInfo)LabelInfo[CurrentLabelNumber + 1]).FactoryType == 2)
                {
                    ev.Graphics.DrawImage(ZTTPS, 249, 120 + margin, 37, 45);
                }
                else
                {
                    ev.Graphics.DrawImage(ZTProfil, 249, 120 + margin, 37, 45);
                }

                ev.Graphics.DrawImage(STB, 418, 119 + margin, 39, 27);
                ev.Graphics.DrawImage(RST, 423, 157 + margin, 34, 27);

                //ev.Graphics.DrawLine(Pen, 235, 315 + margin, 235, 385 + margin);

                ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.ZOV.Barcode.BarcodeLength.Medium, 46, ((SampleInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber), 10, 120 + margin);

                Barcode.DrawBarcodeText(Modules.Packages.ZOV.Barcode.BarcodeLength.Medium, ev.Graphics, ((SampleInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber, 9, 170 + margin);

                ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, 120 + margin);

                if (((SampleInfo)LabelInfo[CurrentLabelNumber + 1]).FactoryType == 2)
                    ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 132 + margin);
                else
                    ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 132 + margin);

                ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 144 + margin);
                ev.Graphics.DrawString("г. Гродно, Герасимовича, 1,", InfoFont, FontBrush, 305, 156 + margin);
                ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 168 + margin);
                ev.Graphics.DrawString("Изготовлено: " + ((SampleInfo)LabelInfo[CurrentLabelNumber + 1]).DocDateTime, InfoFont, FontBrush, 305, 180 + margin);

                CurrentLabelNumber++;
            }
            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }

        public int SaveSampleLabel(int ConfigID, DateTime CreateDateTime, int CreateUserID, int ProductType)
        {
            int SampleLabelID = 0;
            string SelectCommand = @"SELECT TOP 1 * FROM SampleLabels ORDER BY SampleLabelID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["ConfigID"] = ConfigID;
                        NewRow["ProductType"] = ProductType;
                        NewRow["CreateDateTime"] = CreateDateTime;
                        NewRow["CreateUserID"] = CreateUserID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                        DT.Clear();
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                            SampleLabelID = Convert.ToInt32(DT.Rows[0]["SampleLabelID"]);
                    }
                }
            }
            return SampleLabelID;
        }
    }


    public struct CabFurInfo
    {
        public DataTable OrderData;
        public int ProductType;
        public int FactoryType;
        public string DocDateTime;
        public string BarcodeNumber;
        public string TechStoreName;
        public string TechStoreSubGroupName;
        public string SubGroupNotes;
        public string SubGroupNotes1;
        public string SubGroupNotes2;
    }

    public class CabFurLabel
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font ClientFont;
        Font DocFont;
        Font InfoFont;
        Font NotesFont;
        Font HeaderFont;
        Font FrontOrderFont;
        Font DecorOrderFont;
        Font DispatchFont;

        Pen Pen;


        Image ZTTPS;
        Image EAC;
        Image STB;
        Image RST;




        public ArrayList LabelInfo;

        public CabFurLabel()
        {

            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            EAC = new Bitmap(Properties.Resources.eac);
            STB = new Bitmap(Properties.Resources.STB);
            RST = new Bitmap(Properties.Resources.RST);

            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            ClientFont = new Font("Arial", 18.0f, FontStyle.Regular);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 14.0f, FontStyle.Regular);
            HeaderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 9.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        private void DrawTable(PrintPageEventArgs ev)
        {
            float HeaderTopY = 91;
            float OrderTopY = 104;
            float TopLineY = 84;
            float BottomLineY = 135;

            float VertLine1 = 11;
            float VertLine4 = 257;
            float VertLine6 = 327;
            float VertLine8 = 397;
            float VertLine9 = 467;

            if (((CabFurInfo)LabelInfo[CurrentLabelNumber]).ProductType == 2)//корп мебель
            {
                //ev.Graphics.DrawLine(Pen, 11, 113, 467, 113);
                ev.Graphics.DrawLine(Pen, 11, TopLineY, 467, TopLineY);
                ev.Graphics.DrawLine(Pen, 11, TopLineY + 24, 467, TopLineY + 24);
                ev.Graphics.DrawLine(Pen, 11, BottomLineY, 467, BottomLineY);
                string Product = ((CabFurInfo)LabelInfo[CurrentLabelNumber]).TechStoreSubGroupName + "  " + ((CabFurInfo)LabelInfo[CurrentLabelNumber]).TechStoreName;
                ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, TopLineY - 27);
                //header
                ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, VertLine1, HeaderTopY);
                ev.Graphics.DrawString("Выс.", HeaderFont, FontBrush, VertLine4, HeaderTopY);
                ev.Graphics.DrawString("Шир.", HeaderFont, FontBrush, VertLine6, HeaderTopY);
                ev.Graphics.DrawString("Глуб.", HeaderFont, FontBrush, VertLine8, HeaderTopY);

                ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine4, TopLineY, VertLine4, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine6, TopLineY, VertLine6, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine8, TopLineY, VertLine8, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

                for (int i = 0, p = 10; i < ((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Length > 24)
                        ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Substring(0, 24), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);

                    ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, VertLine4, OrderTopY + p);
                    ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, VertLine6, OrderTopY + p);
                    ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, VertLine8, OrderTopY + p);

                    //ev.Graphics.DrawString("Профиль Спинка кровати П-036/0", DecorOrderFont, FontBrush, 12, OrderTopY + p);
                    //ev.Graphics.DrawString("ППДубок/ППКант", DecorOrderFont, FontBrush, 236, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 353, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 393, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 430, OrderTopY + p);
                }
            }
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref CabFurInfo LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;

            StringFormat stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            ev.Graphics.Clear(Color.White);

            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            Rectangle rect1 = new Rectangle(0, 5, 488, 35);
            ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).SubGroupNotes1, HeaderFont, FontBrush, rect1, stringFormat);

            //ev.Graphics.DrawLine(Pen, 371, 33, 371, 67);

            DrawTable(ev);

            //ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Medium, 46, ((CabFurInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);

            //Barcode.DrawBarcodeText(Barcode.BarcodeLength.Medium, ev.Graphics, ((CabFurInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);

            ev.Graphics.DrawImage(EAC, 20, 210, 70, 55);

            //ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
            //ev.Graphics.DrawLine(Pen, 233, 0, 233, 394);

            int rowHeight = 15;
            int rowMargin = 24;
            int r = 125 + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("ГОСТ 16371-2014", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r - 3, 488, 20);
            if (((CabFurInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 1)
                ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", NotesFont, FontBrush, rect1, stringFormat);
            else
                ev.Graphics.DrawString("СООО \"ЗОВ-ТермоПрофильСистемы\"", NotesFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Республика Беларусь, 230011", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("тел\\факс: +375 152 521470", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Изготовлено: " + ((CabFurInfo)LabelInfo[CurrentLabelNumber]).DocDateTime, HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Гарантийный срок эксплуатации 24 мес.", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Срок службы 5 лет.", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).SubGroupNotes, HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Декларация о соответствии " + ((CabFurInfo)LabelInfo[CurrentLabelNumber]).SubGroupNotes2, FrontOrderFont, FontBrush, rect1, stringFormat);

            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;

            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }


        public int SaveSampleLabel(int ConfigID, DateTime CreateDateTime, int CreateUserID, int ProductType)
        {
            int SampleLabelID = 0;
            string SelectCommand = @"SELECT TOP 1 * FROM SampleLabels ORDER BY SampleLabelID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["ConfigID"] = ConfigID;
                        NewRow["ProductType"] = ProductType;
                        NewRow["CreateDateTime"] = CreateDateTime;
                        NewRow["CreateUserID"] = CreateUserID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                        DT.Clear();
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                            SampleLabelID = Convert.ToInt32(DT.Rows[0]["SampleLabelID"]);
                    }
                }
            }
            return SampleLabelID;
        }

        public string GetBarcodeNumber(int BarcodeType, int PackNumber)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = "";
            if (PackNumber.ToString().Length == 1)
                Number = "00000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 2)
                Number = "0000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 3)
                Number = "000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 4)
                Number = "00000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 5)
                Number = "0000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 6)
                Number = "000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 7)
                Number = "00" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 8)
                Number = "0" + PackNumber.ToString();

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

    }


    public class CubeLabel
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font ClientFont;
        Font DocFont;
        Font InfoFont;
        Font NotesFont;
        Font HeaderFont;
        Font FrontOrderFont;
        Font DecorOrderFont;
        Font DispatchFont;

        Pen Pen;


        Image ZTTPS;
        Image EAC;
        Image STB;
        Image RST;




        public ArrayList LabelInfo;

        public CubeLabel()
        {

            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            EAC = new Bitmap(Properties.Resources.eac);
            STB = new Bitmap(Properties.Resources.STB);
            RST = new Bitmap(Properties.Resources.RST);

            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            ClientFont = new Font("Arial", 18.0f, FontStyle.Regular);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 14.0f, FontStyle.Regular);
            HeaderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        private void DrawTable(PrintPageEventArgs ev)
        {
            float HeaderTopY = 91;
            float OrderTopY = 104;
            float TopLineY = 84;
            float BottomLineY = 135;

            float VertLine1 = 11;
            float VertLine3 = 137;
            float VertLine4 = 257;
            float VertLine6 = 327;
            float VertLine7 = 397;
            float VertLine9 = 467;

            if (((CabFurInfo)LabelInfo[CurrentLabelNumber]).ProductType == 2)//корп мебель
            {
                //ev.Graphics.DrawLine(Pen, 11, 113, 467, 113);
                ev.Graphics.DrawLine(Pen, 11, TopLineY, 467, TopLineY);
                ev.Graphics.DrawLine(Pen, 11, TopLineY + 24, 467, TopLineY + 24);
                ev.Graphics.DrawLine(Pen, 11, BottomLineY, 467, BottomLineY);
                string Product = ((CabFurInfo)LabelInfo[CurrentLabelNumber]).TechStoreSubGroupName + "  " + ((CabFurInfo)LabelInfo[CurrentLabelNumber]).TechStoreName;
                ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, TopLineY - 27);
                //header
                ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, VertLine1, HeaderTopY);
                ev.Graphics.DrawString("Цвет-2", HeaderFont, FontBrush, VertLine3, HeaderTopY);
                ev.Graphics.DrawString("Выс.", HeaderFont, FontBrush, VertLine4, HeaderTopY);
                ev.Graphics.DrawString("Шир.", HeaderFont, FontBrush, VertLine6, HeaderTopY);
                ev.Graphics.DrawString("Глуб.", HeaderFont, FontBrush, VertLine7, HeaderTopY);

                ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine3, TopLineY, VertLine3, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine4, TopLineY, VertLine4, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine6, TopLineY, VertLine6, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine7, TopLineY, VertLine7, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

                for (int i = 0, p = 10; i < ((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Length > 24)
                        ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Substring(0, 24), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);

                    if (((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color2"].ToString().Length > 22)
                        ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color2"].ToString().Substring(0, 22), DecorOrderFont, FontBrush, VertLine3, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color2"].ToString(), DecorOrderFont, FontBrush, VertLine3, OrderTopY + p);

                    ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, VertLine4, OrderTopY + p);
                    ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, VertLine6, OrderTopY + p);
                    ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, VertLine7, OrderTopY + p);
                }
            }
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref CabFurInfo LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;

            StringFormat stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            ev.Graphics.Clear(Color.White);

            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            Rectangle rect1 = new Rectangle(0, 5, 488, 35);
            ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).SubGroupNotes1, HeaderFont, FontBrush, rect1, stringFormat);

            //ev.Graphics.DrawLine(Pen, 371, 33, 371, 67);

            DrawTable(ev);

            //ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Medium, 46, ((CabFurInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);

            //Barcode.DrawBarcodeText(Barcode.BarcodeLength.Medium, ev.Graphics, ((CabFurInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);

            ev.Graphics.DrawImage(EAC, 20, 210, 70, 55);

            //ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
            //ev.Graphics.DrawLine(Pen, 233, 0, 233, 394);

            int rowHeight = 15;
            int rowMargin = 24;
            int r = 125 + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("ГОСТ 16371-2014", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r - 3, 488, 20);
            ev.Graphics.DrawString("СООО \"ЗОВ-ТермоПрофильСистемы\"", NotesFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Республика Беларусь, 230011", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("тел\\факс: +375 152 521470", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Изготовлено: " + ((CabFurInfo)LabelInfo[CurrentLabelNumber]).DocDateTime, HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Гарантийный срок эксплуатации 24 мес.", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Срок службы 5 лет.", HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString(((CabFurInfo)LabelInfo[CurrentLabelNumber]).SubGroupNotes, HeaderFont, FontBrush, rect1, stringFormat);
            r = r + rowMargin;
            rect1 = new Rectangle(0, r, 488, rowHeight);
            ev.Graphics.DrawString("Декларация о соответствии " + ((CabFurInfo)LabelInfo[CurrentLabelNumber]).SubGroupNotes2, DecorOrderFont, FontBrush, rect1, stringFormat);

            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;

            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }


        public int SaveSampleLabel(int ConfigID, DateTime CreateDateTime, int CreateUserID, int ProductType)
        {
            int SampleLabelID = 0;
            string SelectCommand = @"SELECT TOP 1 * FROM SampleLabels ORDER BY SampleLabelID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["ConfigID"] = ConfigID;
                        NewRow["ProductType"] = ProductType;
                        NewRow["CreateDateTime"] = CreateDateTime;
                        NewRow["CreateUserID"] = CreateUserID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                        DT.Clear();
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                            SampleLabelID = Convert.ToInt32(DT.Rows[0]["SampleLabelID"]);
                    }
                }
            }
            return SampleLabelID;
        }

        public string GetBarcodeNumber(int BarcodeType, int PackNumber)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = "";
            if (PackNumber.ToString().Length == 1)
                Number = "00000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 2)
                Number = "0000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 3)
                Number = "000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 4)
                Number = "00000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 5)
                Number = "0000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 6)
                Number = "000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 7)
                Number = "00" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 8)
                Number = "0" + PackNumber.ToString();

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

    }






    public struct LabelContent
    {
        public int FactoryType;
        public string DocDateTime;
        public string Batch;
        public string Pallet;
        public string Profile;
        public string Serviceman;
        public string Milling;

        public string DocDateTime1;
        public string Batch1;
        public string Pallet1;
        public string Profile1;
        public string Serviceman1;
        public string Milling1;
    }


    public class SamplesLabels
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;
        //public int PaperWidth = 794;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font ClientFont;
        Font DocFont;
        Font InfoFont;
        Font NotesFont;
        Font HeaderFont;
        Font FrontOrderFont;
        Font DecorOrderFont;
        Font DispatchFont;

        Pen Pen;

        public ArrayList LabelInfo;

        public SamplesLabels()
        {
            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            ClientFont = new Font("Arial", 16.0f, FontStyle.Regular);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 16.0f, FontStyle.Regular);
            HeaderFont = new Font("Arial", 12.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref LabelContent LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;
            ev.Graphics.Clear(Color.White);

            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            float VertLine1 = 11;
            float VertLine2 = 160;
            float VertLine3 = 310;
            float VertLine4 = 467;

            float TopLineY1 = 33;
            float HeaderTopY1 = 38;
            float OrderTopY1 = 83;
            float TopLineY2 = 115;
            float HeaderTopY2 = 120;
            float OrderTopY2 = 165;

            float BottomLineY = 385;
            float MidLineY2 = 206;

            ev.Graphics.DrawString("Дата", ClientFont, FontBrush, VertLine1 + 1, HeaderTopY1);
            ev.Graphics.DrawString("№ партии", ClientFont, FontBrush, VertLine2 + 1, HeaderTopY1);
            ev.Graphics.DrawString("№ пал. вн.", ClientFont, FontBrush, VertLine3 + 1, HeaderTopY1);
            ev.Graphics.DrawString("Профиль", ClientFont, FontBrush, VertLine1 + 1, HeaderTopY2);
            ev.Graphics.DrawString("Наладчик", ClientFont, FontBrush, VertLine2 + 1, HeaderTopY2);
            ev.Graphics.DrawString("Фрезеровка", ClientFont, FontBrush, VertLine3 + 1, HeaderTopY2);

            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY1, VertLine1, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine2, TopLineY1, VertLine2, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine3, TopLineY1, VertLine3, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine4, TopLineY1, VertLine4, BottomLineY);

            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).DocDateTime,
                HeaderFont, FontBrush, VertLine1 + 5, OrderTopY1);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Batch,
                HeaderFont, FontBrush, VertLine2 + 5, OrderTopY1);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Pallet.ToString(),
                HeaderFont, FontBrush, VertLine3 + 5, OrderTopY1);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Profile,
                HeaderFont, FontBrush, VertLine1 + 5, OrderTopY2);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Serviceman,
                HeaderFont, FontBrush, VertLine2 + 5, OrderTopY2);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Milling.ToString(),
                HeaderFont, FontBrush, VertLine3 + 5, OrderTopY2);

            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY1, VertLine4, TopLineY1);
            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY1 + 35, VertLine4, TopLineY1 + 35);
            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY2, VertLine4, TopLineY2);
            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY2 + 35, VertLine4, TopLineY2 + 35);
            ev.Graphics.DrawLine(Pen, VertLine1, MidLineY2, VertLine4, MidLineY2);
            //ev.Graphics.DrawLine(Pen, VertLine1, BottomLineY, VertLine4, BottomLineY);

            MidLineY2 = MidLineY2 - 33;
            HeaderTopY1 += MidLineY2;
            OrderTopY1 += MidLineY2;
            TopLineY1 += MidLineY2;
            TopLineY2 += MidLineY2;
            HeaderTopY2 += MidLineY2;
            OrderTopY2 += MidLineY2;

            MidLineY2 += MidLineY2;

            ev.Graphics.DrawString("Дата", ClientFont, FontBrush, VertLine1 + 1, HeaderTopY1);
            ev.Graphics.DrawString("№ партии", ClientFont, FontBrush, VertLine2 + 1, HeaderTopY1);
            ev.Graphics.DrawString("№ пал. вн.", ClientFont, FontBrush, VertLine3 + 1, HeaderTopY1);
            ev.Graphics.DrawString("Профиль", ClientFont, FontBrush, VertLine1 + 1, HeaderTopY2);
            ev.Graphics.DrawString("Наладчик", ClientFont, FontBrush, VertLine2 + 1, HeaderTopY2);
            ev.Graphics.DrawString("Фрезеровка", ClientFont, FontBrush, VertLine3 + 1, HeaderTopY2);

            //ev.Graphics.DrawLine(Pen, VertLine1, TopLineY1, VertLine1, BottomLineY);
            //ev.Graphics.DrawLine(Pen, VertLine2, TopLineY1, VertLine2, BottomLineY);
            //ev.Graphics.DrawLine(Pen, VertLine3, TopLineY1, VertLine3, BottomLineY);
            //ev.Graphics.DrawLine(Pen, VertLine4, TopLineY1, VertLine4, BottomLineY);

            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).DocDateTime1,
                HeaderFont, FontBrush, VertLine1 + 5, OrderTopY1);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Batch1,
                HeaderFont, FontBrush, VertLine2 + 5, OrderTopY1);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Pallet1.ToString(),
                HeaderFont, FontBrush, VertLine3 + 5, OrderTopY1);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Profile1,
                HeaderFont, FontBrush, VertLine1 + 5, OrderTopY2);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Serviceman1,
                HeaderFont, FontBrush, VertLine2 + 5, OrderTopY2);
            ev.Graphics.DrawString(((LabelContent)LabelInfo[CurrentLabelNumber]).Milling1.ToString(),
                HeaderFont, FontBrush, VertLine3 + 5, OrderTopY2);

            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY1, VertLine4, TopLineY1);
            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY1 + 35, VertLine4, TopLineY1 + 35);
            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY2, VertLine4, TopLineY2);
            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY2 + 35, VertLine4, TopLineY2 + 35);
            //ev.Graphics.DrawLine(Pen, VertLine1, MidLineY2, VertLine4, MidLineY2);
            ev.Graphics.DrawLine(Pen, VertLine1, BottomLineY, VertLine4, BottomLineY);

            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }
    }
}