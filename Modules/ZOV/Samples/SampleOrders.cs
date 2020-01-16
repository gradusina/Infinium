using System;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace Infinium.Modules.ZOV.Samples
{
    public class DecorCatalogOrder
    {
        ComponentFactory.Krypton.Toolkit.KryptonComboBox LengthEdit;
        ComponentFactory.Krypton.Toolkit.KryptonComboBox HeightEdit;
        ComponentFactory.Krypton.Toolkit.KryptonComboBox WidthEdit;

        public int DecorProductsCount = 0;

        public DataTable ItemsDataTable = null;
        public DataTable ColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;

        public DataTable DecorProductsDataTable = null;
        public DataTable DecorDataTable = null;
        public DataTable DecorConfigDataTable = null;
        public DataTable DecorParametersDataTable = null;

        public DataTable TempItemsDataTable = null;
        public DataTable ItemColorsDataTable = null;
        public DataTable ItemPatinaDataTable = null;
        public DataTable ItemLengthDataTable = null;
        public DataTable ItemHeightDataTable = null;
        public DataTable ItemWidthDataTable = null;
        public DataTable ItemInsetTypesDataTable = null;
        public DataTable ItemInsetColorsDataTable = null;

        public BindingSource DecorProductsBindingSource = null;
        public BindingSource DecorBindingSource = null;
        public BindingSource ItemsBindingSource = null;
        public BindingSource ItemColorsBindingSource = null;
        public BindingSource ItemPatinaBindingSource = null;
        public BindingSource ItemLengthBindingSource = null;
        public BindingSource ItemHeightBindingSource = null;
        public BindingSource ItemWidthBindingSource = null;
        public BindingSource ColorsBindingSource = null;
        public BindingSource PatinaBindingSource = null;
        public BindingSource ItemInsetTypesBindingSource = null;
        public BindingSource ItemInsetColorsBindingSource = null;

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

        public DecorCatalogOrder()
        {
            Initialize();
        }

        public void ReplaceOldID()
        {
            DataTable FrontsOrdersDT = new DataTable();
            DataTable DecorOrdersDT = new DataTable();
            DataTable FrontsConfigDT = new DataTable();
            DataTable DecorConfigDT = new DataTable();

            string SelectionCommand = "SELECT * FROM FrontsConfig";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(FrontsConfigDT);
            //}
            FrontsConfigDT = TablesManager.FrontsConfigDataTable;
            SelectionCommand = "SELECT * FROM DyeingAssignmentDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(FrontsOrdersDT);
                    for (int i = 0; i < FrontsOrdersDT.Rows.Count; i++)
                    {
                        int FrontConfigID = Convert.ToInt32(FrontsOrdersDT.Rows[i]["FrontConfigID"]);
                        DataRow[] ConfigRows = FrontsConfigDT.Select("FrontConfigID=" + FrontConfigID);
                        if (ConfigRows.Count() > 0)
                        {
                            int FrontID = Convert.ToInt32(ConfigRows[0]["FrontID"]);
                            int ColorID = Convert.ToInt32(ConfigRows[0]["ColorID"]);
                            int InsetTypeID = Convert.ToInt32(ConfigRows[0]["InsetTypeID"]);
                            int InsetColorID = Convert.ToInt32(ConfigRows[0]["InsetColorID"]);
                            int TechnoColorID = Convert.ToInt32(ConfigRows[0]["TechnoColorID"]);
                            int TechnoInsetTypeID = Convert.ToInt32(ConfigRows[0]["TechnoInsetTypeID"]);
                            int TechnoInsetColorID = Convert.ToInt32(ConfigRows[0]["TechnoInsetColorID"]);
                            int PatinaID = Convert.ToInt32(ConfigRows[0]["PatinaID"]);
                            FrontsOrdersDT.Rows[i]["FrontID"] = FrontID;
                            FrontsOrdersDT.Rows[i]["ColorID"] = ColorID;
                            FrontsOrdersDT.Rows[i]["InsetTypeID"] = InsetTypeID;
                            FrontsOrdersDT.Rows[i]["InsetColorID"] = InsetColorID;
                            FrontsOrdersDT.Rows[i]["TechnoColorID"] = TechnoColorID;
                            FrontsOrdersDT.Rows[i]["TechnoInsetTypeID"] = TechnoInsetTypeID;
                            FrontsOrdersDT.Rows[i]["TechnoInsetColorID"] = TechnoInsetColorID;
                            FrontsOrdersDT.Rows[i]["PatinaID"] = PatinaID;
                        }
                    }
                    DA.Update(FrontsOrdersDT);
                }
            }
        }

        public DecorCatalogOrder(ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tLengthEdit,
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tHeightEdit,
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tWidthEdit)
        {
            LengthEdit = tLengthEdit;
            HeightEdit = tHeightEdit;
            WidthEdit = tWidthEdit;
        }

        private void Create()
        {
            DecorProductsDataTable = new DataTable();
            DecorParametersDataTable = new DataTable();
            DecorConfigDataTable = new DataTable();
            ItemColorsDataTable = new DataTable();
            ItemColorsDataTable = new DataTable();
            ColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            ItemLengthDataTable = new DataTable();
            ItemHeightDataTable = new DataTable();
            ItemWidthDataTable = new DataTable();

            DecorProductsBindingSource = new BindingSource();
            DecorBindingSource = new BindingSource();
            ItemsBindingSource = new BindingSource();
            ItemLengthBindingSource = new BindingSource();
            ItemHeightBindingSource = new BindingSource();
            ItemWidthBindingSource = new BindingSource();
            ItemColorsBindingSource = new BindingSource();
            ItemPatinaBindingSource = new BindingSource();
            ColorsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
        }

        private void GetColorsDT()
        {
            ColorsDataTable = new DataTable();
            ColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            ColorsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = ColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        ColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = ColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        ColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ColorsDataTable.Rows.Add(NewRow);
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
                        DataRow[] rows = ColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = ColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            ColorsDataTable.Rows.Add(NewRow);
                        }
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
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
                DecorProductsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
                DecorDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }
            GetColorsDT();
            GetInsetColorsDT();
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            PatinaDataTable = new DataTable();
            SelectCommand = @"SELECT * FROM Patina ORDER BY PatinaName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
                PatinaDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
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
            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }

            TempItemsDataTable = DecorDataTable.Clone();

            using (DataView DV = new DataView(DecorDataTable))
            {
                ItemsDataTable = DV.ToTable(true, new string[] { "Name" });
            }
            DecorProductsCount = DecorProductsDataTable.Rows.Count;

            ItemColorsDataTable = ColorsDataTable.Clone();
            ItemPatinaDataTable = PatinaDataTable.Clone();
            ItemInsetTypesDataTable = InsetTypesDataTable.Clone();
            ItemInsetColorsDataTable = InsetColorsDataTable.Clone();

            SelectCommand = @"SELECT * FROM DecorConfig" +
                " WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDataTable);
            //    DecorConfigDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Boolean")));
            //}
            DecorConfigDataTable = TablesManager.DecorConfigDataTableAll;
            DecorConfigDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Boolean")));
        }

        private void Binding()
        {
            DecorProductsBindingSource.DataSource = DecorProductsDataTable;
            DecorBindingSource.DataSource = DecorDataTable;
            ItemsBindingSource.DataSource = ItemsDataTable;
            ItemLengthBindingSource.DataSource = ItemLengthDataTable;
            ItemHeightBindingSource.DataSource = ItemHeightDataTable;
            ItemWidthBindingSource.DataSource = ItemWidthDataTable;
            ItemColorsBindingSource.DataSource = ItemColorsDataTable;
            ItemPatinaBindingSource.DataSource = ItemPatinaDataTable;
            ColorsBindingSource.DataSource = ColorsDataTable;
            PatinaBindingSource.DataSource = PatinaDataTable;
            ItemInsetTypesBindingSource = new BindingSource()
            {
                DataSource = ItemInsetTypesDataTable
            };
            ItemInsetColorsBindingSource = new BindingSource()
            {
                DataSource = ItemInsetColorsDataTable
            };
            DecorProductsBindingSourceDisplayMember = "ProductName";
            ItemsBindingSourceDisplayMember = "Name";
            ItemColorsBindingSourceDisplayMember = "ColorName";
            ItemPatinaBindingSourceDisplayMember = "PatinaName";
            ItemLengthBindingSourceDisplayMember = "Length";
            ItemHeightBindingSourceDisplayMember = "Height";
            ItemWidthBindingSourceDisplayMember = "Width";

            DecorProductsBindingSourceValueMember = "ProductID";
            ItemsBindingSourceValueMember = "Name";
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

        //External
        public bool HasParameter(int ProductID, String Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }

        private bool HasColorParameter(DataRow[] Rows, int ColorID)
        {
            foreach (DataRow Row in Rows)
            {
                if (Row["ColorID"].ToString() == ColorID.ToString())
                    return true;
            }

            return false;
        }

        private bool HasPatinaParameter(DataRow[] Rows, int ColorID)
        {
            foreach (DataRow Row in Rows)
            {
                if (Row["PatinaID"].ToString() == ColorID.ToString())
                    return true;
            }

            return false;
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

        private bool HasLengthParameter(DataRow[] Rows, int Length)
        {
            foreach (DataRow Row in Rows)
            {
                if (Row["Length"].ToString() == Length.ToString())
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

        public string GetItemName(int DecorID)
        {
            return DecorDataTable.Select("DecorID = " + DecorID)[0]["Name"].ToString();
        }

        public int GetDecorConfigID(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, ref int FactoryID)
        {
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
                            "ProductID = " + Convert.ToInt32(ProductID) + " AND " +
                            "DecorID = " + Convert.ToInt32(DecorID));


            ColorFilter = " AND ColorID = " + ColorID;
            PatinaFilter = " AND PatinaID = " + PatinaID;
            InsetTypeFilter = " AND InsetTypeID = " + InsetTypeID;
            InsetColorFilter = " AND InsetColorID = " + InsetColorID;
            //if (HasColorParameter(Rows, ColorID))
            //    ColorFilter = " AND ColorID = " + ColorID;
            //else
            //    ColorFilter = " AND ColorID = -1";

            //if (HasPatinaParameter(Rows, PatinaID))
            //    PatinaFilter = " AND PatinaID = " + PatinaID;
            //else
            //    PatinaFilter = " AND PatinaID = -1";

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

            Rows = DecorConfigDataTable.Select("(Excluzive IS NULL OR Excluzive=1) AND " +
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " +
                                "DecorID = " + Convert.ToInt32(DecorID) +
                                ColorFilter + PatinaFilter + InsetTypeFilter + InsetColorFilter + LengthFilter + HeightFilter + WidthFilter);

            if (Rows.Count() < 1 || Rows.Count() > 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог\r\n" +
                    GetDecorName(Convert.ToInt32(DecorID)) + GetColorName(Convert.ToInt32(ColorID)));
                return -1;
            }

            FactoryID = Convert.ToInt32(Rows[0]["FactoryID"]);

            if (Rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(Rows[0]["Excluzive"]) == 0)
            {
                MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
                return -1;
            }
            return Convert.ToInt32(Rows[0]["DecorConfigID"]);
        }

        public int GetDecorConfigID(int ProductID, string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, ref int DecorID, ref int FactoryID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
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
            //if (HasColorParameter(Rows, ColorID))
            //    ColorFilter = " AND ColorID = " + ColorID;
            //else
            //    ColorFilter = " AND ColorID = -1";

            //if (HasPatinaParameter(Rows, PatinaID))
            //    PatinaFilter = " AND PatinaID = " + PatinaID;
            //else
            //    PatinaFilter = " AND PatinaID = -1";

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


            Rows = DecorConfigDataTable.Select("(Excluzive IS NULL OR Excluzive=1) AND " +
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " + filter +
                                ColorFilter + PatinaFilter + InsetTypeFilter + InsetColorFilter + LengthFilter + HeightFilter + WidthFilter);

            if (Rows.Count() < 1 || Rows.Count() > 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог");
                return -1;
            }

            FactoryID = Convert.ToInt32(Rows[0]["FactoryID"]);
            DecorID = Convert.ToInt32(Rows[0]["DecorID"]);

            if (Rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(Rows[0]["Excluzive"]) == 0)
            {
                MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
                return -1;
            }
            return Convert.ToInt32(Rows[0]["DecorConfigID"]);
        }

        private string GetDecorName(int DecorID)
        {
            string Name = string.Empty;
            try
            {
                DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
                Name = Rows[0]["Name"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return Name;
        }

        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = ColorsDataTable.Select("ColorID = " + ColorID);
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

        public void FilterProducts(bool bExcluzive)
        {
            ItemsDataTable.Clear();
            ItemColorsDataTable.Clear();
            ItemPatinaDataTable.Clear();
            ItemInsetTypesDataTable.Clear();
            ItemInsetColorsDataTable.Clear();
            ItemLengthDataTable.Clear();
            ItemHeightDataTable.Clear();
            ItemWidthDataTable.Clear();
            string RowFilter = "Excluzive=1";
            if (!bExcluzive)
                RowFilter = "Excluzive IS NULL OR Excluzive<>0";

            DecorProductsBindingSource.Filter = RowFilter;
            DecorProductsBindingSource.MoveFirst();
        }

        public void FilterItems(int ProductID, bool bExcluzive)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            string RowFilter = "ProductID=" + ProductID;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                if (bExcluzive)
                    RowFilter += " AND (Excluzive=1)";
                else
                    RowFilter += " AND (Excluzive IS NULL OR Excluzive=1)";
                DV.RowFilter = RowFilter;
                TempItemsDataTable = DV.ToTable(true, new string[] { "Name" });
            }

            ItemsDataTable.Clear();
            for (int d = 0; d < TempItemsDataTable.Rows.Count; d++)
            {
                DataRow NewRow = ItemsDataTable.NewRow();
                NewRow["Name"] = TempItemsDataTable.Rows[d]["Name"].ToString();
                ItemsDataTable.Rows.Add(NewRow);
            }
            ItemsDataTable.DefaultView.Sort = "Name ASC";
        }

        public bool FilterColors(string Name, bool bExcluzive)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
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

            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";
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
            if (ItemColorsDataTable.Rows.Count == 1 && ItemColorsDataTable.Rows[0]["ColorID"].ToString() == "-1")
                return false;

            return true;

        }

        public bool FilterPatina(string Name, int ColorID, bool bExcluzive)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
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

            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";
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

            if (ItemPatinaDataTable.Rows.Count == 1 && ItemPatinaDataTable.Rows[0]["PatinaID"].ToString() == "-1")
                return false;

            return true;

        }

        public int FilterLength(string Name, int ColorID, int PatinaID)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
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
            DataRow[] DCR = DecorConfigDataTable.Select(filter + " AND ColorID = " + ColorID.ToString()
                + " AND PatinaID = " + PatinaID.ToString());


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
                        int Height = Convert.ToInt32(DCR[d]["Length"]);

                        DataRow NewRow = ItemLengthDataTable.NewRow();
                        NewRow["Length"] = Height;
                        ItemLengthDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemLengthDataTable.DefaultView.Sort = "Length ASC";

            LengthEdit.Text = "";
            LengthEdit.Items.Clear();
            if (ItemLengthDataTable.Rows.Count > 0)
            {
                if (Convert.ToInt32(ItemLengthDataTable.Rows[0]["Length"]) == 0)
                {
                    LengthEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    return 0;
                }

                if (Convert.ToInt32(ItemLengthDataTable.Rows[0]["Length"]) == -1)
                {
                    return -1;
                }
            }

            foreach (DataRow Row in ItemLengthDataTable.Rows)
                LengthEdit.Items.Add(Row["Length"].ToString());

            LengthEdit.DropDownStyle = ComboBoxStyle.DropDownList;
            if (ItemLengthDataTable.Rows.Count > 0)
                LengthEdit.SelectedIndex = 0;

            return LengthEdit.Items.Count;
        }

        public int FilterHeight(string Name, int ColorID, int PatinaID, int Length)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
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
                + " AND PatinaID = " + PatinaID.ToString() + " AND Length = " + Length.ToString());


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

            HeightEdit.Text = "";
            HeightEdit.Items.Clear();
            if (ItemHeightDataTable.Rows.Count > 0)
            {
                if (Convert.ToInt32(ItemHeightDataTable.Rows[0]["Height"]) == 0)
                {
                    HeightEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    return 0;
                }

                if (Convert.ToInt32(ItemHeightDataTable.Rows[0]["Height"]) == -1)
                {
                    return -1;
                }
            }

            foreach (DataRow Row in ItemHeightDataTable.Rows)
                HeightEdit.Items.Add(Row["Height"].ToString());

            HeightEdit.DropDownStyle = ComboBoxStyle.DropDownList;
            if (ItemHeightDataTable.Rows.Count > 0)
                HeightEdit.SelectedIndex = 0;

            return HeightEdit.Items.Count;
        }

        public int FilterWidth(string Name, int ColorID, int PatinaID, int Length, int Height)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
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
                + " AND ColorID = " + ColorID.ToString() + " AND PatinaID = " + PatinaID.ToString() +
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
                        NewRow["Width"] = Width;
                        ItemWidthDataTable.Rows.Add(NewRow);
                        break;
                    }
                }
            }

            ItemWidthDataTable.DefaultView.Sort = "Width ASC";

            WidthEdit.Text = "";
            WidthEdit.Items.Clear();
            if (ItemWidthDataTable.Rows.Count > 0)
            {
                if (Convert.ToInt32(ItemWidthDataTable.Rows[0]["Width"]) == 0)
                {
                    WidthEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    return 0;
                }

                if (Convert.ToInt32(ItemWidthDataTable.Rows[0]["Width"]) == -1)
                {

                    return -1;
                }
            }
            foreach (DataRow Row in ItemWidthDataTable.Rows)
                WidthEdit.Items.Add(Row["Width"].ToString());

            WidthEdit.DropDownStyle = ComboBoxStyle.DropDownList;
            if (ItemWidthDataTable.Rows.Count > 0)
                WidthEdit.SelectedIndex = 0;

            return WidthEdit.Items.Count;
        }
    }

    public class ZFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentMainOrderID = -1;

        public DataTable FrontsOrdersDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        public DataTable MeasuresDataTable = null;
        public DataTable InsetMarginsDataTable = null;

        public BindingSource FrontsOrdersBindingSource = null;
        private BindingSource FrontsBindingSource = null;
        private BindingSource PatinaBindingSource = null;
        private BindingSource InsetTypesBindingSource = null;
        private BindingSource FrameColorsBindingSource = null;
        private BindingSource InsetColorsBindingSource = null;
        private BindingSource TechnoFrameColorsBindingSource = null;
        private BindingSource TechnoInsetTypesBindingSource = null;
        private BindingSource TechnoInsetColorsBindingSource = null;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public ZFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            FrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;

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

            FrontsOrdersBindingSource = new BindingSource();
            FrontsBindingSource = new BindingSource();
            FrameColorsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            InsetTypesBindingSource = new BindingSource();
            InsetColorsBindingSource = new BindingSource();
            TechnoFrameColorsBindingSource = new BindingSource();
            TechnoInsetTypesBindingSource = new BindingSource();
            TechnoInsetColorsBindingSource = new BindingSource();
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }

            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            MeasuresDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }

            InsetMarginsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetMargins",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetMarginsDataTable);
            }


            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM SampleFrontsOrders", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }
        }

        private void Binding()
        {
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            PatinaBindingSource.DataSource = PatinaDataTable;
            InsetTypesBindingSource.DataSource = InsetTypesDataTable;
            FrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            InsetColorsBindingSource.DataSource = InsetColorsDataTable;
            TechnoFrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            TechnoInsetTypesBindingSource.DataSource = TechnoInsetTypesDataTable;
            TechnoInsetColorsBindingSource.DataSource = TechnoInsetColorsDataTable;

            FrontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
        }

        private void CreateColumns(bool ShowPrice)
        {
            if (FrontsColumn != null)
                return;

            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = FrontsBindingSource,
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = FrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = InsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = InsetColorsBindingSource,
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
                DataSource = TechnoFrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = TechnoInsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = TechnoInsetColorsBindingSource,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrontsOrdersDataGrid.AutoGenerateColumns = false;

            //добавление столбцов
            FrontsOrdersDataGrid.Columns.Add(FrontsColumn);
            FrontsOrdersDataGrid.Columns.Add(FrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(PatinaColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetColorsColumn);

            //убирание лишних столбцов
            if (FrontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                FrontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                FrontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                FrontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                FrontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (FrontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                FrontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                FrontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            FrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            FrontsOrdersDataGrid.Columns["CupboardString"].Visible = false;

            if (FrontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                FrontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                FrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;


            if (!Security.PriceAccess || !ShowPrice)
            {
                FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            FrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            FrontsOrdersDataGrid.Columns["Debt"].Visible = false;

            FrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            //MainOrdersFrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;

            int DisplayIndex = 0;
            FrontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;

            foreach (DataGridViewColumn Column in FrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            FrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            FrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            FrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\n  фасад";
            FrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            FrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            FrontsOrdersDataGrid.Columns["Weight"].HeaderText = "Вес";

            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Height"].Width = 85;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Width"].Width = 85;
            FrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Count"].Width = 85;
            FrontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Cost"].Width = 120;
            FrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Square"].Width = 100;
            FrontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["FrontPrice"].Width = 85;
            FrontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["InsetPrice"].Width = 85;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
        }

        public void Initialize(bool ShowPrice)
        {
            Create();
            Fill();
            Binding();
            CreateColumns(ShowPrice);
        }

        public bool Filter(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return FrontsOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SampleFrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }
    }

    public class ZDecorOrders
    {
        int CurrentMainOrderID = -1;

        public bool Debts = false;

        private DevExpress.XtraTab.XtraTabControl DecorTabControl = null;

        DecorCatalogOrder DecorCatalogOrder = null;

        PercentageDataGrid MainOrdersFrontsOrdersDataGrid;

        public DataTable DecorOrdersDataTable = null;
        public DataTable[] DecorItemOrdersDataTables = null;
        public BindingSource[] DecorItemOrdersBindingSources = null;
        public SqlDataAdapter DecorOrdersDataAdapter = null;
        public SqlCommandBuilder DecorOrdersCommandBuilder = null;
        public PercentageDataGrid[] DecorItemOrdersDataGrids = null;

        //конструктор
        public ZDecorOrders(ref DevExpress.XtraTab.XtraTabControl tDecorTabControl,
                                     ref DecorCatalogOrder tDecorCatalogOrder,
                                     ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            DecorTabControl = tDecorTabControl;
            DecorCatalogOrder = tDecorCatalogOrder;

            MainOrdersFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }


        private void Create()
        {
            DecorOrdersDataTable = new DataTable();
            DecorItemOrdersDataTables = new DataTable[DecorCatalogOrder.DecorProductsCount];
            DecorItemOrdersBindingSources = new BindingSource[DecorCatalogOrder.DecorProductsCount];
            DecorItemOrdersDataGrids = new PercentageDataGrid[DecorCatalogOrder.DecorProductsCount];

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = new DataTable();
                DecorItemOrdersBindingSources[i] = new BindingSource();
            }
        }

        private void Fill()
        {
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM SampleDecorOrders", ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
        }

        private void Binding()
        {

        }

        public void Initialize(bool ShowPrice)
        {
            Create();
            Fill();
            Binding();

            SplitDecorOrdersTables();
            GridSettings(ShowPrice);
        }

        private DataGridViewComboBoxColumn CreateInsetTypesColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",

                DataSource = new DataView(DecorCatalogOrder.InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return ItemColumn;
        }

        private DataGridViewComboBoxColumn CreateInsetColorsColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",

                DataSource = new DataView(DecorCatalogOrder.InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return ItemColumn;
        }

        private DataGridViewComboBoxColumn CreateColorColumn()
        {
            DataGridViewComboBoxColumn ColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ColorsColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",

                DataSource = DecorCatalogOrder.ColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return ColorsColumn;
        }

        private DataGridViewComboBoxColumn CreatePatinaColumn()
        {
            DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",

                DataSource = DecorCatalogOrder.PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return PatinaColumn;
        }

        private DataGridViewComboBoxColumn CreateItemColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ItemColumn",
                HeaderText = "Название",
                DataPropertyName = "DecorID",

                DataSource = new DataView(DecorCatalogOrder.DecorDataTable),
                ValueMember = "DecorID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return ItemColumn;
        }

        private void SplitDecorOrdersTables()
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = DecorOrdersDataTable.Clone();
                DecorItemOrdersBindingSources[i].DataSource = DecorItemOrdersDataTables[i];
            }
        }

        private void GridSettings(bool ShowPrice)
        {
            DecorTabControl.AppearancePage.Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            DecorTabControl.AppearancePage.Header.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            DecorTabControl.AppearancePage.Header.BorderColor = System.Drawing.Color.Black;
            DecorTabControl.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            DecorTabControl.AppearancePage.Header.Options.UseBackColor = true;
            DecorTabControl.AppearancePage.Header.Options.UseBorderColor = true;
            DecorTabControl.AppearancePage.Header.Options.UseFont = true;
            DecorTabControl.LookAndFeel.SkinName = "Office 2010 Black";
            DecorTabControl.LookAndFeel.UseDefaultLookAndFeel = false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorTabControl.TabPages.Add(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString());
                DecorTabControl.TabPages[i].PageVisible = false;
                DecorTabControl.TabPages[i].Text = DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString();

                DecorItemOrdersDataGrids[i] = new PercentageDataGrid()
                {
                    Parent = DecorTabControl.TabPages[i],
                    DataSource = DecorItemOrdersBindingSources[i],
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black
                };
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = Color.White;
                DecorItemOrdersDataGrids[i].AllowUserToAddRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToDeleteRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToResizeRows = false;
                DecorItemOrdersDataGrids[i].RowHeadersVisible = false;
                DecorItemOrdersDataGrids[i].AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.Background.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.Background.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateCommon.Background.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Font = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].RowTemplate.Height = MainOrdersFrontsOrdersDataGrid.RowTemplate.Height;
                DecorItemOrdersDataGrids[i].ColumnHeadersHeight = MainOrdersFrontsOrdersDataGrid.ColumnHeadersHeight;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Font = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLine = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLine;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLineH = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLineH;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.TextH = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.TextH;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DecorItemOrdersDataGrids[i].SelectedColorStyle = PercentageDataGrid.ColorStyle.Green;
                DecorItemOrdersDataGrids[i].ReadOnly = true;
                DecorItemOrdersDataGrids[i].UseCustomBackColor = true;

                //if (Screen.PrimaryScreen.Bounds.Width < 1600 || Screen.PrimaryScreen.Bounds.Height < 900)
                //{
                //    DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Font =
                //       new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                //    DecorItemOrdersDataGrids[i].RowTemplate.Height = 30;
                //    DecorItemOrdersDataGrids[i].ColumnHeadersHeight = 40;
                //    DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Font =
                //        new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

                //    //DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Font =
                //    //    new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                //    //DecorItemOrdersDataGrids[i].RowTemplate.Height = 35;
                //    //DecorItemOrdersDataGrids[i].ColumnHeadersHeight = 45;
                //    //DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Font =
                //    //    new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                //}

                if (!Security.PriceAccess || !ShowPrice)
                {
                    DecorItemOrdersDataGrids[i].Columns["Price"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["Cost"].Visible = false;
                }

                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetColorsColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetTypesColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateColorColumn());
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreatePatinaColumn());
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateItemColumn());
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].MinimumWidth = 120;

                //убирание лишних столбцов
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateDateTime"))
                {
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].HeaderText = "Добавлено";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].Width = 100;
                }
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["MainOrderID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ProductID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ColorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetColorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorConfigID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["FactoryID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ItemWeight"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Weight"].Visible = false;

                if (!Debts)
                {
                    DecorItemOrdersDataGrids[i].Columns["Debt"].Visible = false;
                }
                else
                {
                    DecorItemOrdersDataGrids[i].Columns["Count"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["Debt"].HeaderText = "Кол-во";
                }

                //русские названия полей

                DecorItemOrdersDataGrids[i].Columns["Price"].HeaderText = "Цена";
                DecorItemOrdersDataGrids[i].Columns["Cost"].HeaderText = "Стоимость";

                for (int j = 2; j < DecorItemOrdersDataGrids[i].Columns.Count; j++)
                {
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Height")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Высота";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Length")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Длина";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Width")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Ширина";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Count")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Кол-во";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Notes")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Примечание";
                    }
                }

                foreach (DataGridViewColumn Column in DecorItemOrdersDataGrids[i].Columns)
                {
                    Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "ColorID"))
                {
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 190;
                }
                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Height"))
                {
                    //DecorItemOrdersDataGrids[i].Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Height"].MinimumWidth = 90;
                }
                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Length"))
                {
                    //DecorItemOrdersDataGrids[i].Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Length"].MinimumWidth = 90;
                }
                if (DecorCatalogOrder.HasParameter(Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Width"))
                {
                    //DecorItemOrdersDataGrids[i].Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Width"].MinimumWidth = 90;
                }

                DecorItemOrdersDataGrids[i].AutoGenerateColumns = false;
                int DisplayIndex = 0;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Length"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Height"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Width"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Count"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Price"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Cost"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Notes"].DisplayIndex = DisplayIndex++;
            }
        }

        public bool HasRows()
        {
            int ItemsCount = 0;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                for (int r = 0; r < DecorItemOrdersDataTables[i].Rows.Count; r++)
                    if (DecorItemOrdersDataTables[i].Rows[r].RowState != DataRowState.Deleted)
                        ItemsCount += DecorItemOrdersDataTables[i].Rows.Count;
            }

            return ItemsCount > 0;
        }

        private bool ShowTabs()
        {
            int IsOrder = 0;

            for (int i = 0; i < DecorTabControl.TabPages.Count; i++)
            {
                if (DecorItemOrdersDataTables[i].Rows.Count > 0)
                {
                    IsOrder++;
                    DecorTabControl.TabPages[i].PageVisible = true;
                }
                else
                    DecorTabControl.TabPages[i].PageVisible = false;
            }

            if (IsOrder > 0)
                return true;
            else
                return false;
        }


        public bool Filter(int MainOrderID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return DecorOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i].Clear();
                DecorItemOrdersDataTables[i].AcceptChanges();
                DecorTabControl.TabPages[i].PageVisible = false;
            }

            DecorOrdersCommandBuilder.Dispose();
            DecorOrdersDataAdapter.Dispose();

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + MainOrderID.ToString(),
                                                            ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;
                bool ShowColor = false;
                bool ShowPatina = false;
                bool ShowII = false;
                bool ShowIC = false;
                bool ShowLength = false;
                bool ShowHeight = false;
                bool ShowWidth = false;
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (!ShowColor)
                        if (Convert.ToInt32(Rows[r]["ColorID"]) != -1)
                            ShowColor = true;
                    if (!ShowPatina)
                        if (Convert.ToInt32(Rows[r]["PatinaID"]) != -1)
                            ShowPatina = true;
                    if (!ShowII)
                        if (Convert.ToInt32(Rows[r]["InsetTypeID"]) != -1)
                            ShowII = true;
                    if (!ShowIC)
                        if (Convert.ToInt32(Rows[r]["InsetColorID"]) != -1)
                            ShowIC = true;
                    if (!ShowLength)
                        if (Convert.ToInt32(Rows[r]["Length"]) != -1)
                            ShowLength = true;
                    if (!ShowHeight)
                        if (Convert.ToInt32(Rows[r]["Height"]) != -1)
                            ShowHeight = true;
                    if (!ShowWidth)
                        if (Convert.ToInt32(Rows[r]["Width"]) != -1)
                            ShowWidth = true;
                    DecorItemOrdersDataTables[i].ImportRow(Rows[r]);
                }
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Length"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Height"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Width"].Visible = false;
                if (ShowColor)
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = true;
                if (ShowPatina)
                    DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = true;
                if (ShowII)
                    DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = true;
                if (ShowIC)
                    DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = true;
                if (ShowLength)
                    DecorItemOrdersDataGrids[i].Columns["Length"].Visible = true;
                if (ShowHeight)
                    DecorItemOrdersDataGrids[i].Columns["Height"].Visible = true;
                if (ShowWidth)
                    DecorItemOrdersDataGrids[i].Columns["Width"].Visible = true;
            }

            ShowTabs();

            return true;
        }

        public bool Filter(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return DecorOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i].Clear();
                DecorItemOrdersDataTables[i].AcceptChanges();
                DecorTabControl.TabPages[i].PageVisible = false;
            }

            DecorOrdersCommandBuilder.Dispose();
            DecorOrdersDataAdapter.Dispose();

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;
                bool ShowColor = false;
                bool ShowPatina = false;
                bool ShowLength = false;
                bool ShowHeight = false;
                bool ShowWidth = false;
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (!ShowColor)
                        if (Convert.ToInt32(Rows[r]["ColorID"]) != -1)
                            ShowColor = true;
                    if (!ShowPatina)
                        if (Convert.ToInt32(Rows[r]["PatinaID"]) != -1)
                            ShowPatina = true;
                    if (!ShowLength)
                        if (Convert.ToInt32(Rows[r]["Length"]) != -1)
                            ShowLength = true;
                    if (!ShowHeight)
                        if (Convert.ToInt32(Rows[r]["Height"]) != -1)
                            ShowHeight = true;
                    if (!ShowWidth)
                        if (Convert.ToInt32(Rows[r]["Width"]) != -1)
                            ShowWidth = true;
                    DecorItemOrdersDataTables[i].ImportRow(Rows[r]);
                }
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Length"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Height"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Width"].Visible = false;
                if (ShowColor)
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = true;
                if (ShowPatina)
                    DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = true;
                if (ShowLength)
                    DecorItemOrdersDataGrids[i].Columns["Length"].Visible = true;
                if (ShowHeight)
                    DecorItemOrdersDataGrids[i].Columns["Height"].Visible = true;
                if (ShowWidth)
                    DecorItemOrdersDataGrids[i].Columns["Width"].Visible = true;
            }

            ShowTabs();

            return true;
        }
    }


    public class MFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentMainOrderID = -1;

        public DataTable FrontsOrdersDataTable = null;
        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;

        public BindingSource FrontsOrdersBindingSource = null;
        private BindingSource FrontsBindingSource = null;
        private BindingSource FrameColorsBindingSource = null;
        private BindingSource PatinaBindingSource = null;
        private BindingSource InsetTypesBindingSource = null;
        private BindingSource InsetColorsBindingSource = null;
        private BindingSource TechnoFrameColorsBindingSource = null;
        private BindingSource TechnoInsetTypesBindingSource = null;
        private BindingSource TechnoInsetColorsBindingSource = null;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public MFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            FrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
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

            FrontsOrdersBindingSource = new BindingSource();
            FrontsBindingSource = new BindingSource();
            FrameColorsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            InsetTypesBindingSource = new BindingSource();
            InsetColorsBindingSource = new BindingSource();
            TechnoFrameColorsBindingSource = new BindingSource();
            TechnoInsetTypesBindingSource = new BindingSource();
            TechnoInsetColorsBindingSource = new BindingSource();
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM SampleFrontsOrders", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }
        }

        private void Binding()
        {
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            PatinaBindingSource.DataSource = PatinaDataTable;
            InsetTypesBindingSource.DataSource = InsetTypesDataTable;
            InsetColorsBindingSource.DataSource = InsetColorsDataTable;
            TechnoFrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            TechnoInsetTypesBindingSource.DataSource = TechnoInsetTypesDataTable;
            TechnoInsetColorsBindingSource.DataSource = TechnoInsetColorsDataTable;

            FrontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
        }

        private void CreateColumns(bool ShowPrice)
        {
            if (FrontsColumn != null)
                return;

            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = FrontsBindingSource,
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = FrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = InsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = InsetColorsBindingSource,
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
                DataSource = TechnoFrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = TechnoInsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = TechnoInsetColorsBindingSource,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrontsOrdersDataGrid.AutoGenerateColumns = false;

            //добавление столбцов
            FrontsOrdersDataGrid.Columns.Add(FrontsColumn);
            FrontsOrdersDataGrid.Columns.Add(FrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(PatinaColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetColorsColumn);

            //убирание лишних столбцов
            if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                FrontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                FrontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                FrontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                FrontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (FrontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                FrontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                FrontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["CupboardString"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            FrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            FrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;
            FrontsOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["CurrencyCost"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("OriginalInsetPrice"))
                FrontsOrdersDataGrid.Columns["OriginalInsetPrice"].Visible = false;

            if (!Security.PriceAccess || !ShowPrice)
            {
                FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["TotalDiscount"].Visible = false;
                FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
                FrontsOrdersDataGrid.Columns["OriginalPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["OriginalCost"].Visible = false;
                FrontsOrdersDataGrid.Columns["CostWithTransport"].Visible = false;
                FrontsOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            }
            int DisplayIndex = 0;
            FrontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;

            FrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            foreach (DataGridViewColumn Column in FrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            FrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            FrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\nфасад";
            FrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            FrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            FrontsOrdersDataGrid.Columns["OriginalPrice"].HeaderText = "Цена\r\n(оригинал)";
            FrontsOrdersDataGrid.Columns["OriginalCost"].HeaderText = "Стоимость\r\n(оригинал)";
            FrontsOrdersDataGrid.Columns["CostWithTransport"].HeaderText = "Стоимость\r\n(с транспортом)";
            FrontsOrdersDataGrid.Columns["PriceWithTransport"].HeaderText = "Цена\r\n(с транспортом)";
            FrontsOrdersDataGrid.Columns["CurrencyCost"].HeaderText = "Стоимость\r\nв расчете";
            FrontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";
            FrontsOrdersDataGrid.Columns["TotalDiscount"].HeaderText = "Общая\r\nскидка, %";

            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CurrencyCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["OriginalPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["OriginalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CostWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PriceWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
            FrontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrontsOrdersDataGrid.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //FrontsOrdersDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            FrontsOrdersDataGrid.CellFormatting += FrontsOrdersDataGrid_CellFormatting;
        }

        void FrontsOrdersDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns.Contains("PatinaColumn") && (e.ColumnIndex == grid.Columns["PatinaColumn"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int PatinaID = -1;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["PatinaID"].Value != DBNull.Value)
                {
                    PatinaID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PatinaID"].Value);
                    DisplayName = PatinaDisplayName(PatinaID);
                }
                cell.ToolTipText = DisplayName;
            }
        }

        public string PatinaDisplayName(int PatinaID)
        {
            DataRow[] rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
                return rows[0]["DisplayName"].ToString();
            return string.Empty;
        }

        public void Initialize(bool ShowPrice)
        {
            Create();
            Fill();
            Binding();
            CreateColumns(ShowPrice);
        }

        public bool Filter(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return FrontsOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SampleFrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }
    }

    public class MDecorOrders
    {
        int CurrentClientID = -1;
        int CurrentMainOrderID = -1;
        int SelectedGridIndex = -1;

        private DevExpress.XtraTab.XtraTabControl DecorTabControl = null;

        public DecorCatalogOrder DecorCatalogOrder = null;

        public DataTable DecorOrdersDataTable = null;
        public DataTable[] DecorItemOrdersDataTables = null;

        public BindingSource[] DecorItemOrdersBindingSources = null;

        public SqlDataAdapter DecorOrdersDataAdapter = null;
        public SqlCommandBuilder DecorOrdersCommandBuilder = null;

        public PercentageDataGrid[] DecorItemOrdersDataGrids = null;

        public PercentageDataGrid MainOrdersFrontsOrdersDataGrid = null;

        public MDecorOrders(ref DevExpress.XtraTab.XtraTabControl tDecorTabControl,
            ref DecorCatalogOrder tDecorCatalogOrder,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            DecorTabControl = tDecorTabControl;
            DecorCatalogOrder = tDecorCatalogOrder;

            MainOrdersFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }

        public int ClientID
        {
            get { return CurrentClientID; }
            set { CurrentClientID = value; }
        }

        private void Create()
        {
            //cmiAddToRequest = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            //cmiAddToRequest.ImageTransparentColor = System.Drawing.Color.Transparent;
            //cmiAddToRequest.Text = "Добавить в заявку";
            //cmiAddToRequest.Click += new System.EventHandler(cmiAddToRequest_Click);

            //kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            //kryptonContextMenuItems1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            //cmiAddToRequest});

            //kryptonContextMenu1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            //kryptonContextMenu1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            //kryptonContextMenuItems1});
            //kryptonContextMenu1.Tag = "18";

            DecorOrdersDataTable = new DataTable();
            DecorItemOrdersDataTables = new DataTable[DecorCatalogOrder.DecorProductsCount];
            DecorItemOrdersBindingSources = new BindingSource[DecorCatalogOrder.DecorProductsCount];
            DecorItemOrdersDataGrids = new PercentageDataGrid[DecorCatalogOrder.DecorProductsCount];

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = new DataTable();
                DecorItemOrdersBindingSources[i] = new BindingSource();
            }
        }

        private void Fill()
        {
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM SampleDecorOrders", ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
        }

        private void Binding()
        {

        }

        public void Initialize(bool ShowPrice)
        {
            Create();
            Fill();
            Binding();

            SplitDecorOrdersTables();
            GridSettings(ShowPrice);
        }

        private DataGridViewComboBoxColumn CreateInsetTypesColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",

                DataSource = new DataView(DecorCatalogOrder.InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return ItemColumn;
        }

        private DataGridViewComboBoxColumn CreateInsetColorsColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",

                DataSource = new DataView(DecorCatalogOrder.InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return ItemColumn;
        }

        private DataGridViewComboBoxColumn CreateColorColumn()
        {
            DataGridViewComboBoxColumn ColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ColorsColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",

                DataSource = DecorCatalogOrder.ColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return ColorsColumn;
        }

        private DataGridViewComboBoxColumn CreatePatinaColumn()
        {
            DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",

                DataSource = DecorCatalogOrder.PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return PatinaColumn;
        }

        private DataGridViewComboBoxColumn CreateItemColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ItemColumn",
                HeaderText = "Название",
                DataPropertyName = "DecorID",

                DataSource = new DataView(DecorCatalogOrder.DecorDataTable),
                ValueMember = "DecorID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return ItemColumn;
        }

        private void SplitDecorOrdersTables()
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = DecorOrdersDataTable.Clone();
                DecorItemOrdersBindingSources[i].DataSource = DecorItemOrdersDataTables[i];
            }
        }

        private void GridSettings(bool ShowPrice)
        {
            DecorTabControl.AppearancePage.Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            DecorTabControl.AppearancePage.Header.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            DecorTabControl.AppearancePage.Header.BorderColor = System.Drawing.Color.Black;
            DecorTabControl.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                ((byte)(204)));
            DecorTabControl.AppearancePage.Header.Options.UseBackColor = true;
            DecorTabControl.AppearancePage.Header.Options.UseBorderColor = true;
            DecorTabControl.AppearancePage.Header.Options.UseFont = true;
            DecorTabControl.LookAndFeel.SkinName = "Office 2010 Black";
            DecorTabControl.LookAndFeel.UseDefaultLookAndFeel = false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorTabControl.TabPages.Add(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString());
                DecorTabControl.TabPages[i].PageVisible = false;
                DecorTabControl.TabPages[i].Text = DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString();

                DecorItemOrdersDataGrids[i] = new PercentageDataGrid()
                {
                    Parent = DecorTabControl.TabPages[i],
                    DataSource = DecorItemOrdersBindingSources[i],
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black
                };
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = Color.White;
                DecorItemOrdersDataGrids[i].AllowUserToAddRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToDeleteRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToResizeRows = false;
                DecorItemOrdersDataGrids[i].RowHeadersVisible = false;
                DecorItemOrdersDataGrids[i].AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.Background.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.Background.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateCommon.Background.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Font = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].RowTemplate.Height = MainOrdersFrontsOrdersDataGrid.RowTemplate.Height;
                DecorItemOrdersDataGrids[i].ColumnHeadersHeight = MainOrdersFrontsOrdersDataGrid.ColumnHeadersHeight;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Font = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLine = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLine;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLineH = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLineH;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.TextH = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.TextH;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DecorItemOrdersDataGrids[i].SelectedColorStyle = PercentageDataGrid.ColorStyle.Green;
                DecorItemOrdersDataGrids[i].ReadOnly = true;
                DecorItemOrdersDataGrids[i].Tag = i;
                DecorItemOrdersDataGrids[i].UseCustomBackColor = true;
                DecorItemOrdersDataGrids[i].StandardStyle = false;
                DecorItemOrdersDataGrids[i].MultiSelect = true;
                DecorItemOrdersDataGrids[i].CellMouseDown += new DataGridViewCellMouseEventHandler(MainOrdersDecorOrders_CellMouseDown);

                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetColorsColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetTypesColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateColorColumn());
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreatePatinaColumn());
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateItemColumn());
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].MinimumWidth = 60;

                //убирание лишних столбцов
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateDateTime"))
                {
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].HeaderText = "Добавлено";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].Width = 100;
                }
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["MainOrderID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ProductID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ColorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetColorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorConfigID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["FactoryID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ItemWeight"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Weight"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["CurrencyTypeID"].Visible = false;

                if (!Security.PriceAccess)
                {
                    DecorItemOrdersDataGrids[i].Columns["Price"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["Cost"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["OriginalCost"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].Visible = false;
                }

                //русские названия полей

                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].HeaderText = "Цена\r\nначальная";
                DecorItemOrdersDataGrids[i].Columns["Price"].HeaderText = "Цена\r\nконечная";
                DecorItemOrdersDataGrids[i].Columns["Cost"].HeaderText = "Стоимость\r\nконечная";
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].HeaderText = "Цена\r\n(оригинал)";
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].HeaderText = "Стоимость\r\n(оригинал)";
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].HeaderText = "Стоимость\r\n(с транспортом)";
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].HeaderText = "Цена\r\n(с транспортом)";
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].HeaderText = "Общая\r\nскидка, %";
                DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].HeaderText = "Стоимость\r\nв расчете";
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].HeaderText = "Объемный\r\nкоэф-нт";
                DecorItemOrdersDataGrids[i].Columns["IsSample"].HeaderText = "Образцы";

                for (int j = 2; j < DecorItemOrdersDataGrids[i].Columns.Count; j++)
                {
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Height")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Высота";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Length")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Длина";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Width")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Ширина";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Count")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Кол-во";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Notes")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Примечание";
                    }
                }

                foreach (DataGridViewColumn Column in DecorItemOrdersDataGrids[i].Columns)
                {
                    Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                DecorItemOrdersDataGrids[i].AutoGenerateColumns = false;
                int DisplayIndex = 0;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Length"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Height"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Width"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Count"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Price"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Cost"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].DisplayIndex = DisplayIndex++;
            }
        }

        void MainOrdersDecorOrders_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            SelectedGridIndex = Convert.ToInt32(grid.Tag);
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DecorItemOrdersBindingSources[SelectedGridIndex].Position = e.RowIndex;
                //kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private bool ShowTabs()
        {
            int IsOrder = 0;

            for (int i = 0; i < DecorTabControl.TabPages.Count; i++)
            {
                if (DecorItemOrdersDataTables[i].Rows.Count > 0)
                {
                    IsOrder++;
                    DecorTabControl.TabPages[i].PageVisible = true;
                }
                else
                    DecorTabControl.TabPages[i].PageVisible = false;
            }

            if (IsOrder > 0)
                return true;
            else
                return false;
        }

        public bool Filter(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return DecorOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i].Clear();
                DecorItemOrdersDataTables[i].AcceptChanges();
                DecorTabControl.TabPages[i].PageVisible = false;
            }

            DecorOrdersCommandBuilder.Dispose();
            DecorOrdersDataAdapter.Dispose();

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;
                bool ShowColor = false;
                bool ShowPatina = false;
                bool ShowII = false;
                bool ShowIC = false;
                bool ShowLength = false;
                bool ShowHeight = false;
                bool ShowWidth = false;
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (!ShowColor)
                        if (Convert.ToInt32(Rows[r]["ColorID"]) != -1)
                            ShowColor = true;
                    if (!ShowPatina)
                        if (Convert.ToInt32(Rows[r]["PatinaID"]) != -1)
                            ShowPatina = true;
                    if (!ShowII)
                        if (Convert.ToInt32(Rows[r]["InsetTypeID"]) != -1)
                            ShowII = true;
                    if (!ShowIC)
                        if (Convert.ToInt32(Rows[r]["InsetColorID"]) != -1)
                            ShowIC = true;
                    if (!ShowLength)
                        if (Convert.ToInt32(Rows[r]["Length"]) != -1)
                            ShowLength = true;
                    if (!ShowHeight)
                        if (Convert.ToInt32(Rows[r]["Height"]) != -1)
                            ShowHeight = true;
                    if (!ShowWidth)
                        if (Convert.ToInt32(Rows[r]["Width"]) != -1)
                            ShowWidth = true;
                    DecorItemOrdersDataTables[i].ImportRow(Rows[r]);
                }
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Length"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Height"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Width"].Visible = false;
                if (ShowColor)
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = true;
                if (ShowPatina)
                    DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = true;
                if (ShowII)
                    DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = true;
                if (ShowIC)
                    DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = true;
                if (ShowLength)
                    DecorItemOrdersDataGrids[i].Columns["Length"].Visible = true;
                if (ShowHeight)
                    DecorItemOrdersDataGrids[i].Columns["Height"].Visible = true;
                if (ShowWidth)
                    DecorItemOrdersDataGrids[i].Columns["Width"].Visible = true;
            }

            ShowTabs();

            return true;
        }
    }

    public class OrdersManager
    {
        public FileManager FM = new FileManager();
        public int CurrentClientID = -1;
        public int CurrentMainOrderID = -1;

        public MFrontsOrders MFrontsOrders = null;
        public MDecorOrders MDecorOrders = null;
        public ZFrontsOrders ZFrontsOrders = null;
        public ZDecorOrders ZDecorOrders = null;

        private DataTable MClientsDataTable = null;
        private DataTable MClientsGroupsDataTable = null;
        private DataTable ZClientsDataTable = null;
        private DataTable ZClientsGroupsDataTable = null;
        private DataTable MOrdersDataTable = null;
        private DataTable ZOrdersDataTable = null;
        private DataTable MShopAddressesDataTable = null;
        private DataTable ZShopAddressesDataTable = null;

        private DataTable OrdersDataTable = null;

        public BindingSource MClientsBindingSource = null;
        public BindingSource ZClientsBindingSource = null;
        public BindingSource MClientsGroupsBindingSource = null;
        public BindingSource ZClientsGroupsBindingSource = null;
        public BindingSource MainOrdersBindingSource = null;

        public OrdersManager(
                             ref PercentageDataGrid tMFrontsOrdersDataGrid,
                             ref PercentageDataGrid tZFrontsOrdersDataGrid,
                             ref DevExpress.XtraTab.XtraTabControl tMDecorTabControl,
                             ref DevExpress.XtraTab.XtraTabControl tZDecorTabControl,
                             ref DecorCatalogOrder DecorCatalogOrder)
        {
            MFrontsOrders = new MFrontsOrders(ref tMFrontsOrdersDataGrid);
            MFrontsOrders.Initialize(true);

            MDecorOrders = new MDecorOrders(ref tMDecorTabControl, ref DecorCatalogOrder, ref tMFrontsOrdersDataGrid);
            MDecorOrders.Initialize(true);

            ZFrontsOrders = new ZFrontsOrders(ref tZFrontsOrdersDataGrid);
            ZFrontsOrders.Initialize(true);

            ZDecorOrders = new ZDecorOrders(ref tZDecorTabControl, ref DecorCatalogOrder, ref tZFrontsOrdersDataGrid);
            ZDecorOrders.Initialize(true);

            Initialize();
        }


        private void Create()
        {
            MClientsDataTable = new DataTable();
            MClientsGroupsDataTable = new DataTable();
            MOrdersDataTable = new DataTable();
            ZClientsDataTable = new DataTable();
            ZClientsGroupsDataTable = new DataTable();
            ZOrdersDataTable = new DataTable();
            MShopAddressesDataTable = new DataTable();
            ZShopAddressesDataTable = new DataTable();

            OrdersDataTable = new DataTable();
            OrdersDataTable.Columns.Add(new DataColumn("FirmType", Type.GetType("System.Int32")));
            OrdersDataTable.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            OrdersDataTable.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            OrdersDataTable.Columns.Add(new DataColumn("OrderNumber", Type.GetType("System.String")));
            OrdersDataTable.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.Int32")));
            OrdersDataTable.Columns.Add(new DataColumn("CreateDate", Type.GetType("System.DateTime")));
            OrdersDataTable.Columns.Add(new DataColumn("DispDate", Type.GetType("System.DateTime")));
            OrdersDataTable.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));
            OrdersDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            OrdersDataTable.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            OrdersDataTable.Columns.Add(new DataColumn("ShopAddresses", Type.GetType("System.String")));
            OrdersDataTable.Columns.Add(new DataColumn("Foto", Type.GetType("System.Boolean")));

            MClientsBindingSource = new BindingSource();
            ZClientsBindingSource = new BindingSource();
            MClientsGroupsBindingSource = new BindingSource();
            ZClientsGroupsBindingSource = new BindingSource();
            MainOrdersBindingSource = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM SampleMainOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MOrdersDataTable);
            }
            SelectCommand = @"SELECT TOP 0 * FROM SampleMainOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(ZOrdersDataTable);
            }
            SelectCommand = @"SELECT ClientID, ClientName, ClientGroupID FROM Clients ORDER BY ClientName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MClientsDataTable);
            }
            SelectCommand = @"SELECT * FROM ClientGroups";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MClientsGroupsDataTable);
            }
            SelectCommand = @"SELECT ClientID, ClientName, ClientGroupID FROM Clients ORDER BY ClientName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZClientsDataTable);
            }
            SelectCommand = @"SELECT * FROM ClientsGroups";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZClientsGroupsDataTable);
            }
            SelectCommand = @"SELECT * FROM ShopAddresses";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MShopAddressesDataTable);
            }
            SelectCommand = @"SELECT * FROM ShopAddresses";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZShopAddressesDataTable);
            }

            MClientsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            MClientsGroupsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            ZClientsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            ZClientsGroupsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            for (int i = 0; i < MClientsDataTable.Rows.Count; i++)
                MClientsDataTable.Rows[i]["Check"] = false;
            for (int i = 0; i < MClientsGroupsDataTable.Rows.Count; i++)
                MClientsGroupsDataTable.Rows[i]["Check"] = false;
            for (int i = 0; i < ZClientsDataTable.Rows.Count; i++)
                ZClientsDataTable.Rows[i]["Check"] = false;
            for (int i = 0; i < ZClientsGroupsDataTable.Rows.Count; i++)
                ZClientsGroupsDataTable.Rows[i]["Check"] = false;
        }

        private void Binding()
        {
            MClientsBindingSource.DataSource = MClientsDataTable;
            MClientsGroupsBindingSource.DataSource = MClientsGroupsDataTable;
            ZClientsBindingSource.DataSource = ZClientsDataTable;
            ZClientsGroupsBindingSource.DataSource = ZClientsGroupsDataTable;
            MainOrdersBindingSource.DataSource = OrdersDataTable;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public void FilterMClients(int ClientGroupID)
        {
            MClientsBindingSource.Filter = "ClientGroupID = " + ClientGroupID;
            MClientsBindingSource.MoveFirst();
        }

        public void FilterZClients(int ClientGroupID)
        {
            ZClientsBindingSource.Filter = "ClientGroupID = " + ClientGroupID;
            ZClientsBindingSource.MoveFirst();
        }

        public ArrayList GetMClients()
        {
            ArrayList Clients = new ArrayList();
            for (int i = 0; i < MClientsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(MClientsDataTable.Rows[i]["Check"]))
                    continue;

                Clients.Add(Convert.ToInt32(MClientsDataTable.Rows[i]["ClientID"]));
            }
            return Clients;
        }

        public ArrayList GetMClientGroups()
        {
            ArrayList ClientGroupIDs = new ArrayList();
            for (int i = 0; i < MClientsGroupsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(MClientsGroupsDataTable.Rows[i]["Check"])
                    || Convert.ToInt32(MClientsGroupsDataTable.Rows[i]["ClientGroupID"]) == 1)
                    continue;

                ClientGroupIDs.Add(Convert.ToInt32(MClientsGroupsDataTable.Rows[i]["ClientGroupID"]));
            }
            return ClientGroupIDs;
        }

        public ArrayList GetZClients()
        {
            ArrayList Clients = new ArrayList();
            for (int i = 0; i < ZClientsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(ZClientsDataTable.Rows[i]["Check"]))
                    continue;

                Clients.Add(Convert.ToInt32(ZClientsDataTable.Rows[i]["ClientID"]));
            }
            return Clients;
        }

        public ArrayList GetZClientGroups()
        {
            ArrayList ClientGroupIDs = new ArrayList();
            for (int i = 0; i < ZClientsGroupsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(ZClientsGroupsDataTable.Rows[i]["Check"])
                    || Convert.ToInt32(ZClientsGroupsDataTable.Rows[i]["ClientGroupID"]) == 1)
                    continue;

                ClientGroupIDs.Add(Convert.ToInt32(ZClientsGroupsDataTable.Rows[i]["ClientGroupID"]));
            }
            return ClientGroupIDs;
        }

        public void CheckMClients(bool Check)
        {
            for (int i = 0; i < MClientsDataTable.Rows.Count; i++)
            {
                MClientsDataTable.Rows[i]["Check"] = Check;
            }
        }

        public void CheckMClients(bool Check, int ClientGroupID)
        {
            string GroupFilter = string.Empty;
            GroupFilter = "ClientGroupID = " + ClientGroupID;
            DataRow[] Rows = MClientsDataTable.Select(GroupFilter);
            foreach (DataRow row in Rows)
                row["Check"] = Check;
        }

        public void CheckMClientGroups(bool Check)
        {
            for (int i = 0; i < MClientsGroupsDataTable.Rows.Count; i++)
            {
                MClientsGroupsDataTable.Rows[i]["Check"] = Check;
            }
        }

        public void CheckMClientGroups(bool Check, int ClientGroupID)
        {
            string GroupFilter = string.Empty;
            GroupFilter = "ClientGroupID = " + ClientGroupID;
            DataRow[] Rows = MClientsGroupsDataTable.Select(GroupFilter);
            foreach (DataRow row in Rows)
                row["Check"] = Check;
        }

        public void CheckZClients(bool Check)
        {
            for (int i = 0; i < ZClientsDataTable.Rows.Count; i++)
            {
                ZClientsDataTable.Rows[i]["Check"] = Check;
            }
        }

        public void CheckZClients(bool Check, int ClientGroupID)
        {
            string GroupFilter = string.Empty;
            GroupFilter = "ClientGroupID = " + ClientGroupID;
            DataRow[] Rows = ZClientsDataTable.Select(GroupFilter);
            foreach (DataRow row in Rows)
                row["Check"] = Check;
        }

        public void CheckZClientGroups(bool Check, int ClientGroupID)
        {
            string GroupFilter = string.Empty;
            GroupFilter = "ClientGroupID = " + ClientGroupID;
            DataRow[] Rows = ZClientsGroupsDataTable.Select(GroupFilter);
            foreach (DataRow row in Rows)
                row["Check"] = Check;
        }

        public void CheckZClientGroups(bool Check)
        {
            for (int i = 0; i < ZClientsGroupsDataTable.Rows.Count; i++)
            {
                ZClientsGroupsDataTable.Rows[i]["Check"] = Check;
            }
        }

        public void FilterOrders(
            bool bMarketing,
            bool bMClients,
            bool bMCreateDate,
            object MCreateDateFrom,
            object MCreateDateTo,
            bool bMDispDate,
            object MDispDateFrom,
            object MDispDateTo,
            bool bZOV,
            bool bZClients,
            bool bZCreateDate,
            object ZCreateDateFrom,
            object ZCreateDateTo,
            bool bZDispDate,
            object ZDispDateFrom,
            object ZDispDateTo)
        {
            string MFilter = string.Empty;
            string ZFilter = string.Empty;
            if (bMarketing)
            {
                if (bMClients)
                {
                    ArrayList MClients = GetMClients();
                    if (MClients.Count > 0)
                    {
                        if (MFilter.Length > 0)
                            MFilter += " AND MegaOrders.ClientID IN (" + string.Join(",", MClients.OfType<Int32>().ToArray()) + ")";
                        else
                            MFilter = " WHERE MegaOrders.ClientID IN (" + string.Join(",", MClients.OfType<Int32>().ToArray()) + ")";
                    }
                    else
                    {
                        if (MFilter.Length > 0)
                            MFilter += " AND MegaOrders.ClientID = -1";
                        else
                            MFilter = " WHERE MegaOrders.ClientID = -1";
                    }
                }
                if (bMCreateDate)
                {
                    if (MFilter.Length > 0)
                    {
                        MFilter += " AND CAST(DocDateTime AS DATE) >= '" + Convert.ToDateTime(MCreateDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DocDateTime AS DATE) <= '" + Convert.ToDateTime(MCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                    else
                    {
                        MFilter = " WHERE CAST(DocDateTime AS DATE) >= '" + Convert.ToDateTime(MCreateDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DocDateTime AS DATE) <= '" + Convert.ToDateTime(MCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                }
                if (bMDispDate)
                {
                    if (MFilter.Length > 0)
                    {
                        MFilter += " AND CAST(DispDateTime AS DATE) >= '" + Convert.ToDateTime(MDispDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DispDateTime AS DATE) <= '" + Convert.ToDateTime(MDispDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                    else
                    {
                        MFilter = " WHERE CAST(DispDateTime AS DATE) >= '" + Convert.ToDateTime(MDispDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DispDateTime AS DATE) <= '" + Convert.ToDateTime(MDispDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                }
                if (!bMClients && !bMCreateDate && !bMDispDate)
                    MFilter = " WHERE MainOrderID = -1";
            }
            else
                MFilter = " WHERE MainOrderID = -1";

            if (bZOV)
            {
                if (bZClients)
                {
                    ArrayList ZClients = GetZClients();
                    if (ZClients.Count > 0)
                    {
                        if (ZFilter.Length > 0)
                            ZFilter += " AND SampleMainOrders.ClientID IN (" + string.Join(",", ZClients.OfType<Int32>().ToArray()) + ")";
                        else
                            ZFilter = " WHERE SampleMainOrders.ClientID IN (" + string.Join(",", ZClients.OfType<Int32>().ToArray()) + ")";
                    }
                    else
                    {
                        if (ZFilter.Length > 0)
                            ZFilter += " AND SampleMainOrders.ClientID = -1";
                        else
                            ZFilter = " WHERE SampleMainOrders.ClientID = -1";
                    }
                }
                if (bZCreateDate)
                {
                    if (ZFilter.Length > 0)
                    {
                        ZFilter += " AND CAST(DocDateTime AS DATE) >= '" + Convert.ToDateTime(ZCreateDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DocDateTime AS DATE) <= '" + Convert.ToDateTime(ZCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                    else
                    {
                        ZFilter = " WHERE CAST(DocDateTime AS DATE) >= '" + Convert.ToDateTime(ZCreateDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DocDateTime AS DATE) <= '" + Convert.ToDateTime(ZCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                }
                if (bZDispDate)
                {
                    if (ZFilter.Length > 0)
                    {
                        ZFilter += " AND CAST(DispDateTime AS DATE) >= '" + Convert.ToDateTime(ZDispDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DispDateTime AS DATE) <= '" + Convert.ToDateTime(ZDispDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                    else
                    {
                        ZFilter = " WHERE CAST(DispDateTime AS DATE) >= '" + Convert.ToDateTime(ZDispDateFrom).ToString("yyyy-MM-dd") +
                            "' AND CAST(DispDateTime AS DATE) <= '" + Convert.ToDateTime(ZDispDateTo).ToString("yyyy-MM-dd") + "' ";
                    }
                }
                if (!bZClients && !bZCreateDate && !bZDispDate)
                    ZFilter = " WHERE MainOrderID = -1";
            }
            else
                ZFilter = " WHERE MainOrderID = -1";

            string SelectCommand = @"SELECT SampleMainOrders.*, MegaOrders.ClientID, MegaOrders.OrderNumber, Clients.ClientName FROM SampleMainOrders
                INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON MegaOrders.ClientID=Clients.ClientID" + MFilter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                MOrdersDataTable.Clear();
                DA.Fill(MOrdersDataTable);
            }
            SelectCommand = @"SELECT SampleMainOrders.*, Clients.ClientName FROM SampleMainOrders
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON SampleMainOrders.ClientID=Clients.ClientID" + ZFilter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                ZOrdersDataTable.Clear();
                DA.Fill(ZOrdersDataTable);
            }
            OrdersDataTable.Clear();
            for (int i = 0; i < MOrdersDataTable.Rows.Count; i++)
            {
                DataRow NewRow = OrdersDataTable.NewRow();
                NewRow["FirmType"] = 1;
                NewRow["ClientID"] = MOrdersDataTable.Rows[i]["ClientID"];
                NewRow["ClientName"] = MOrdersDataTable.Rows[i]["ClientName"];
                NewRow["OrderNumber"] = MOrdersDataTable.Rows[i]["OrderNumber"];
                NewRow["MainOrderID"] = MOrdersDataTable.Rows[i]["MainOrderID"];
                NewRow["CreateDate"] = MOrdersDataTable.Rows[i]["DocDateTime"];
                NewRow["Cost"] = MOrdersDataTable.Rows[i]["OrderCost"];
                NewRow["Square"] = MOrdersDataTable.Rows[i]["FrontsSquare"];
                NewRow["Description"] = MOrdersDataTable.Rows[i]["Description"];
                NewRow["Foto"] = MOrdersDataTable.Rows[i]["Foto"];
                NewRow["DispDate"] = MOrdersDataTable.Rows[i]["DispDate"];
                DataRow[] rows = MShopAddressesDataTable.Select("ClientID=" + Convert.ToInt32(MOrdersDataTable.Rows[i]["ClientID"]));
                if (rows.Count() > 0)
                    NewRow["ShopAddresses"] = "Показать магазины";
                OrdersDataTable.Rows.Add(NewRow);
            }
            for (int i = 0; i < ZOrdersDataTable.Rows.Count; i++)
            {
                DataRow NewRow = OrdersDataTable.NewRow();
                NewRow["FirmType"] = 0;
                NewRow["ClientID"] = ZOrdersDataTable.Rows[i]["ClientID"];
                NewRow["ClientName"] = ZOrdersDataTable.Rows[i]["ClientName"];
                NewRow["OrderNumber"] = ZOrdersDataTable.Rows[i]["DocNumber"];
                NewRow["MainOrderID"] = ZOrdersDataTable.Rows[i]["MainOrderID"];
                NewRow["CreateDate"] = ZOrdersDataTable.Rows[i]["DocDateTime"];
                NewRow["Cost"] = ZOrdersDataTable.Rows[i]["OrderCost"];
                NewRow["Square"] = ZOrdersDataTable.Rows[i]["FrontsSquare"];
                NewRow["Description"] = ZOrdersDataTable.Rows[i]["Description"];
                NewRow["Foto"] = ZOrdersDataTable.Rows[i]["Foto"];
                NewRow["DispDate"] = ZOrdersDataTable.Rows[i]["DispDate"];
                DataRow[] rows = ZShopAddressesDataTable.Select("ClientID=" + Convert.ToInt32(ZOrdersDataTable.Rows[i]["ClientID"]));
                if (rows.Count() > 0)
                    NewRow["ShopAddresses"] = "Показать магазины";
                OrdersDataTable.Rows.Add(NewRow);
            }
            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartMDocNumber(string DocText)
        {
            string Search = string.Format(" WHERE OrderNumber LIKE '%" + DocText + "%'");

            string SelectCommand = @"SELECT SampleMainOrders.*, MegaOrders.OrderNumber, MegaOrders.ClientID, Clients.ClientName FROM SampleMainOrders
                INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON MegaOrders.ClientID=Clients.ClientID" + Search;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                MOrdersDataTable.Clear();
                DA.Fill(MOrdersDataTable);
            }
            OrdersDataTable.Clear();
            for (int i = 0; i < MOrdersDataTable.Rows.Count; i++)
            {
                DataRow NewRow = OrdersDataTable.NewRow();
                NewRow["FirmType"] = 1;
                NewRow["ClientID"] = MOrdersDataTable.Rows[i]["ClientID"];
                NewRow["ClientName"] = MOrdersDataTable.Rows[i]["ClientName"];
                NewRow["OrderNumber"] = MOrdersDataTable.Rows[i]["OrderNumber"];
                NewRow["MainOrderID"] = MOrdersDataTable.Rows[i]["MainOrderID"];
                NewRow["CreateDate"] = MOrdersDataTable.Rows[i]["DocDateTime"];
                NewRow["Cost"] = MOrdersDataTable.Rows[i]["OrderCost"];
                NewRow["Square"] = MOrdersDataTable.Rows[i]["FrontsSquare"];
                NewRow["Description"] = MOrdersDataTable.Rows[i]["Description"];
                NewRow["Foto"] = MOrdersDataTable.Rows[i]["Foto"];
                NewRow["DispDate"] = MOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(NewRow);
            }
            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartZDocNumber(string DocText)
        {
            string Search = string.Format(" WHERE DocNumber LIKE '%" + DocText + "%'");

            string SelectCommand = @"SELECT SampleMainOrders.*, Clients.ClientName FROM SampleMainOrders
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON SampleMainOrders.ClientID=Clients.ClientID" + Search;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                ZOrdersDataTable.Clear();
                DA.Fill(ZOrdersDataTable);
            }
            OrdersDataTable.Clear();
            for (int i = 0; i < ZOrdersDataTable.Rows.Count; i++)
            {
                DataRow NewRow = OrdersDataTable.NewRow();
                NewRow["FirmType"] = 0;
                NewRow["ClientID"] = ZOrdersDataTable.Rows[i]["ClientID"];
                NewRow["ClientName"] = ZOrdersDataTable.Rows[i]["ClientName"];
                NewRow["OrderNumber"] = ZOrdersDataTable.Rows[i]["DocNumber"];
                NewRow["MainOrderID"] = ZOrdersDataTable.Rows[i]["MainOrderID"];
                NewRow["CreateDate"] = ZOrdersDataTable.Rows[i]["DocDateTime"];
                NewRow["Cost"] = ZOrdersDataTable.Rows[i]["OrderCost"];
                NewRow["Square"] = ZOrdersDataTable.Rows[i]["FrontsSquare"];
                NewRow["Description"] = ZOrdersDataTable.Rows[i]["Description"];
                NewRow["Foto"] = ZOrdersDataTable.Rows[i]["Foto"];
                NewRow["DispDate"] = ZOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(NewRow);
            }
            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartMNotes(string DocText)
        {
            string Search = string.Format(" WHERE Description LIKE '%" + DocText + "%'");

            string SelectCommand = @"SELECT SampleMainOrders.*, MegaOrders.OrderNumber, MegaOrders.ClientID, Clients.ClientName FROM SampleMainOrders
                INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON MegaOrders.ClientID=Clients.ClientID" + Search;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                MOrdersDataTable.Clear();
                DA.Fill(MOrdersDataTable);
            }
            OrdersDataTable.Clear();
            for (int i = 0; i < MOrdersDataTable.Rows.Count; i++)
            {
                DataRow NewRow = OrdersDataTable.NewRow();
                NewRow["FirmType"] = 1;
                NewRow["ClientID"] = MOrdersDataTable.Rows[i]["ClientID"];
                NewRow["ClientName"] = MOrdersDataTable.Rows[i]["ClientName"];
                NewRow["OrderNumber"] = MOrdersDataTable.Rows[i]["OrderNumber"];
                NewRow["MainOrderID"] = MOrdersDataTable.Rows[i]["MainOrderID"];
                NewRow["CreateDate"] = MOrdersDataTable.Rows[i]["DocDateTime"];
                NewRow["Cost"] = MOrdersDataTable.Rows[i]["OrderCost"];
                NewRow["Square"] = MOrdersDataTable.Rows[i]["FrontsSquare"];
                NewRow["Description"] = MOrdersDataTable.Rows[i]["Description"];
                NewRow["Foto"] = MOrdersDataTable.Rows[i]["Foto"];
                NewRow["DispDate"] = MOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(NewRow);
            }
            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartZNotes(string DocText)
        {
            string Search = string.Format(" WHERE Description LIKE '%" + DocText + "%'");

            string SelectCommand = @"SELECT SampleMainOrders.*, Clients.ClientName FROM SampleMainOrders
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON SampleMainOrders.ClientID=Clients.ClientID" + Search;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                ZOrdersDataTable.Clear();
                DA.Fill(ZOrdersDataTable);
            }
            OrdersDataTable.Clear();
            for (int i = 0; i < ZOrdersDataTable.Rows.Count; i++)
            {
                DataRow NewRow = OrdersDataTable.NewRow();
                NewRow["FirmType"] = 0;
                NewRow["ClientID"] = ZOrdersDataTable.Rows[i]["ClientID"];
                NewRow["ClientName"] = ZOrdersDataTable.Rows[i]["ClientName"];
                NewRow["OrderNumber"] = ZOrdersDataTable.Rows[i]["DocNumber"];
                NewRow["MainOrderID"] = ZOrdersDataTable.Rows[i]["MainOrderID"];
                NewRow["CreateDate"] = ZOrdersDataTable.Rows[i]["DocDateTime"];
                NewRow["Cost"] = ZOrdersDataTable.Rows[i]["OrderCost"];
                NewRow["Square"] = ZOrdersDataTable.Rows[i]["FrontsSquare"];
                NewRow["Description"] = ZOrdersDataTable.Rows[i]["Description"];
                NewRow["Foto"] = ZOrdersDataTable.Rows[i]["Foto"];
                NewRow["DispDate"] = ZOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(NewRow);
            }
            OrdersDataTable.AcceptChanges();
        }

        public void FilterProductByMainOrder(bool IsZOV, int MainOrderID, ref bool FrontsVisible, ref bool DecorVisible)
        {
            if (IsZOV)
            {
                FrontsVisible = ZFrontsOrders.Filter(MainOrderID, 0);
                DecorVisible = ZDecorOrders.Filter(MainOrderID, 0);
            }
            else
            {
                FrontsVisible = MFrontsOrders.Filter(MainOrderID, 0);
                DecorVisible = MDecorOrders.Filter(MainOrderID, 0);
            }
        }

        public DataTable GetPermissions(int UserID, string FormName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM UserRoles WHERE UserID = " + UserID +
                " AND RoleID IN (SELECT RoleID FROM Roles WHERE ModuleID IN " +
                " (SELECT ModuleID FROM Modules WHERE FormName = '" + FormName + "'))", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return (DataTable)DT;
                }
            }
        }

        static public void FixOrderEvent(int MegaOrderID, string Event)
        {
            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT * FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM MegaOrdersEvents";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (Event == "Заказ удален")
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow["MegaOrderID"] = MegaOrderID;
                            NewRow["Event"] = Event;
                            NewRow["EventDate"] = Security.GetCurrentDate();
                            NewRow["EventUserID"] = Security.CurrentUserID;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                        if (TempDT.Rows.Count > 0)
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow.ItemArray = TempDT.Rows[0].ItemArray;
                            NewRow["Event"] = Event;
                            NewRow["EventDate"] = Security.GetCurrentDate();
                            NewRow["EventUserID"] = Security.CurrentUserID;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                        else
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow["MegaOrderID"] = MegaOrderID;
                            NewRow["Event"] = "Заказа не существует";
                            NewRow["EventDate"] = Security.GetCurrentDate();
                            NewRow["EventUserID"] = Security.CurrentUserID;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SaveMDescription()
        {
            string filter = "FirmType=1";
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(OrdersDataTable, filter, string.Empty, DataViewRowState.ModifiedCurrent))
            {
                DT1 = DV.ToTable(true, new string[] { "MainOrderID", "Description" });
            }
            if (DT1.Rows.Count == 0)
                return;
            filter = string.Empty;
            for (int i = 0; i < DT1.Rows.Count; i++)
                filter += Convert.ToInt32(DT1.Rows[i]["MainOrderID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = " WHERE MainOrderID IN (" + filter + ")";
            }
            else
                filter = " WHERE MainOrderID = - 1";
            string SelectCommand = @"SELECT SampleMainOrderID, MainOrderID, Description FROM SampleMainOrders" + filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT2 = new DataTable())
                    {
                        if (DA.Fill(DT2) > 0)
                        {
                            for (int i = 0; i < DT1.Rows.Count; i++)
                            {
                                int MainOrderID = Convert.ToInt32(DT1.Rows[i]["MainOrderID"]);
                                DataRow[] Rows = DT2.Select("MainOrderID=" + MainOrderID);
                                if (Rows.Count() > 0)
                                    Rows[0]["Description"] = DT1.Rows[i]["Description"];
                            }
                            DA.Update(DT2);
                        }
                    }
                }
            }
        }

        public void SaveZDescription()
        {
            string filter = "FirmType=0";
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(OrdersDataTable, filter, string.Empty, DataViewRowState.ModifiedCurrent))
            {
                DT1 = DV.ToTable(true, new string[] { "MainOrderID", "Description" });
            }
            if (DT1.Rows.Count == 0)
                return;
            filter = string.Empty;
            for (int i = 0; i < DT1.Rows.Count; i++)
                filter += Convert.ToInt32(DT1.Rows[i]["MainOrderID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = " WHERE MainOrderID IN (" + filter + ")";
            }
            else
                filter = " WHERE MainOrderID = - 1";
            string SelectCommand = @"SELECT SampleMainOrderID, MainOrderID, Description FROM SampleMainOrders" + filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT2 = new DataTable())
                    {
                        if (DA.Fill(DT2) > 0)
                        {
                            for (int i = 0; i < DT1.Rows.Count; i++)
                            {
                                int MainOrderID = Convert.ToInt32(DT1.Rows[i]["MainOrderID"]);
                                DataRow[] Rows = DT2.Select("MainOrderID=" + MainOrderID);
                                if (Rows.Count() > 0)
                                    Rows[0]["Description"] = DT1.Rows[i]["Description"];
                            }
                            DA.Update(DT2);
                        }
                    }
                }
            }
        }

        public Image GetMFoto(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + MainOrderID, 
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        using (MemoryStream ms = new MemoryStream(
                            FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + FileName,
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
            }
        }

        public Image GetZFoto(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;
                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;
                    string FileName = DT.Rows[0]["FileName"].ToString();
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(
                            FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + FileName,
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
            }
        }

        public bool AttachMFoto(string Extension, string FileName, string Path, int MainOrderID)
        {
            bool Ok = true;
            try
            {
                string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("SamplesFoto");
                string sFileName = FileName;
                int j = 1;
                while (FM.FileExist(sDestFolder + "/" + sFileName + Extension, Configs.FTPType))
                {
                    sFileName = FileName.ToString() + "(" + j++ + ")";
                }
                FileName = sFileName + Extension;
                if (FM.UploadFile(Path, sDestFolder + "/" + sFileName + Extension, Configs.FTPType) == false)
                    Ok = false;
            }
            catch
            {
                Ok = false;
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM SamplesFoto", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        FileInfo fi;
                        try
                        {
                            fi = new FileInfo(Path);
                        }
                        catch
                        {
                            Ok = false;
                            return false;
                        }
                        DataRow NewRow = DT.NewRow();
                        NewRow["MainOrderID"] = MainOrderID;
                        NewRow["FileName"] = FileName;
                        NewRow["FileSize"] = fi.Length;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                    }
                }
            }
            if (Ok)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " + MainOrderID,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                DT.Rows[0]["Foto"] = true;
                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
            return Ok;
        }

        public bool AttachZFoto(string Extension, string FileName, string Path, int MainOrderID)
        {
            bool Ok = true;
            try
            {
                string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("SamplesFoto");
                string sFileName = FileName;
                int j = 1;
                while (FM.FileExist(sDestFolder + "/" + sFileName + Extension, Configs.FTPType))
                {
                    sFileName = FileName.ToString() + "(" + j++ + ")";
                }
                FileName = sFileName + Extension;
                if (FM.UploadFile(Path, sDestFolder + "/" + sFileName + Extension, Configs.FTPType) == false)
                    Ok = false;
            }
            catch
            {
                Ok = false;
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM SamplesFoto", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        FileInfo fi;
                        try
                        {
                            fi = new FileInfo(Path);
                        }
                        catch
                        {
                            Ok = false;
                            return false;
                        }
                        DataRow NewRow = DT.NewRow();
                        NewRow["MainOrderID"] = MainOrderID;
                        NewRow["FileName"] = FileName;
                        NewRow["FileSize"] = fi.Length;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                    }
                }
            }
            if (Ok)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " + MainOrderID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                DT.Rows[0]["Foto"] = true;
                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
            return Ok;
        }

        public void DetachMFoto(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bOk = false;
                        foreach (DataRow Row in DT.Rows)
                        {
                            bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        if (bOk)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM SamplesFoto WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0) 
                        {
                            DT.Rows[0]["Foto"] = false;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void DetachZFoto(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bOk = false;
                        foreach (DataRow Row in DT.Rows)
                        {
                            bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        if (bOk)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM SamplesFoto WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["Foto"] = false;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public DataTable FillShopAddressesDataTable(int FirmType, int ClientID)
        {
            DataTable ShopAddressesDataTable = new DataTable();
            string SelectCommand = @"SELECT * FROM ShopAddresses WHERE ClientID=" + ClientID + " ORDER BY Address";
            if (FirmType == 1)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(ShopAddressesDataTable);
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVReferenceConnectionString))
                {
                    DA.Fill(ShopAddressesDataTable);
                }
            }
            return ShopAddressesDataTable;
        }

    }

    public class SampleOrders
    {
        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;

        public SampleOrders()
        {
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            FrontsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            GetColorsDT();
            GetPatinaDT();
            GetInsetColorsDT();

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig ) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
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

        private void GetPatinaDT()
        {
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
        }

        public DataTable f1(int ClientID)
        {
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT MegaOrders.OrderNumber, SampleMainOrders.MegaOrderID, SampleMainOrders.MainOrderID, SampleMainOrders.DocDateTime FROM SampleMainOrders INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID AND ClientID=" + ClientID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DT);
            }
            return DT;
        }

        public DataTable f2(int MainOrderID)
        {
            DataTable DT = new DataTable();
            DT.Columns.Add(new DataColumn("FrontName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("TechnoColorName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("PatinaName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("InsetTypeName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("InsetColorName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("TechnoInsetTypeName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("TechnoInsetColorName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("IsSample", Type.GetType("System.Boolean")));
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SampleFrontsOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA.Fill(DT1) > 0)
                    {
                        for (int i = 0; i < DT1.Rows.Count; i++)
                        {
                            int FrontID = Convert.ToInt32(DT1.Rows[i]["FrontID"]);
                            int ColorID = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                            int PatinaID = Convert.ToInt32(DT1.Rows[i]["PatinaID"]);
                            int InsetTypeID = Convert.ToInt32(DT1.Rows[i]["InsetTypeID"]);
                            int InsetColorID = Convert.ToInt32(DT1.Rows[i]["InsetColorID"]);
                            int TechnoColorID = Convert.ToInt32(DT1.Rows[i]["TechnoColorID"]);
                            int TechnoInsetTypeID = Convert.ToInt32(DT1.Rows[i]["TechnoInsetTypeID"]);
                            int TechnoInsetColorID = Convert.ToInt32(DT1.Rows[i]["TechnoInsetColorID"]);
                            int Height = Convert.ToInt32(DT1.Rows[i]["Height"]);
                            int Width = Convert.ToInt32(DT1.Rows[i]["Width"]);
                            int Count = Convert.ToInt32(DT1.Rows[i]["Count"]);
                            bool IsSample = Convert.ToBoolean(DT1.Rows[i]["IsSample"]);

                            DataRow NewRow = DT.NewRow();
                            NewRow["FrontName"] = GetFrontName(FrontID);
                            NewRow["ColorName"] = GetColorName(ColorID);
                            NewRow["TechnoColorName"] = GetColorName(TechnoColorID);
                            NewRow["PatinaName"] = GetPatinaName(PatinaID);
                            NewRow["InsetTypeName"] = GetInsetTypeName(InsetTypeID);
                            NewRow["InsetColorName"] = GetInsetColorName(InsetColorID);
                            NewRow["TechnoInsetTypeName"] = GetInsetTypeName(TechnoInsetTypeID);
                            NewRow["TechnoInsetColorName"] = GetInsetColorName(TechnoInsetColorID);
                            NewRow["Height"] = Height;
                            NewRow["Width"] = Width;
                            NewRow["Count"] = Count;
                            NewRow["IsSample"] = IsSample;
                            DT.Rows.Add(NewRow);
                        }
                    }
                }
            }

            return DT;
        }

        public DataTable f3(int MainOrderID)
        {
            DataTable DT = new DataTable();
            DT.Columns.Add(new DataColumn("FrontName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("DecorName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("PatinaName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("InsetTypeName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("InsetColorName", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("IsSample", Type.GetType("System.Boolean")));
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA.Fill(DT1) > 0)
                    {
                        for (int i = 0; i < DT1.Rows.Count; i++)
                        {
                            int ProductID = Convert.ToInt32(DT1.Rows[i]["ProductID"]);
                            int DecorID = Convert.ToInt32(DT1.Rows[i]["DecorID"]);
                            int ColorID = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                            int PatinaID = Convert.ToInt32(DT1.Rows[i]["PatinaID"]);
                            int InsetTypeID = Convert.ToInt32(DT1.Rows[i]["InsetTypeID"]);
                            int InsetColorID = Convert.ToInt32(DT1.Rows[i]["InsetColorID"]);
                            int Length = Convert.ToInt32(DT1.Rows[i]["Length"]);
                            int Height = Convert.ToInt32(DT1.Rows[i]["Height"]);
                            int Width = Convert.ToInt32(DT1.Rows[i]["Width"]);
                            int Count = Convert.ToInt32(DT1.Rows[i]["Count"]);
                            bool IsSample = Convert.ToBoolean(DT1.Rows[i]["IsSample"]);

                            DataRow NewRow = DT.NewRow();
                            NewRow["FrontName"] = GetProductName(ProductID);
                            NewRow["ColorName"] = GetColorName(ColorID);
                            NewRow["DecorName"] = GetDecorName(DecorID);
                            NewRow["PatinaName"] = GetPatinaName(PatinaID);
                            NewRow["InsetTypeName"] = GetInsetTypeName(InsetTypeID);
                            NewRow["InsetColorName"] = GetInsetColorName(InsetColorID);
                            NewRow["Length"] = Length;
                            NewRow["Height"] = Height;
                            NewRow["Width"] = Width;
                            NewRow["Count"] = Count;
                            NewRow["IsSample"] = IsSample;
                            DT.Rows.Add(NewRow);
                        }
                    }
                }
            }

            return DT;
        }

        private string GetFrontName(int FrontID)
        {
            string name = string.Empty;
            DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
            if (Rows.Count() > 0)
                name = Rows[0]["FrontName"].ToString();
            return name;
        }

        private string GetColorName(int ColorID)
        {
            string name = string.Empty;
            DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
            if (Rows.Count() > 0)
                name = Rows[0]["ColorName"].ToString();
            return name;
        }

        private string GetPatinaName(int PatinaID)
        {
            string name = string.Empty;
            DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (Rows.Count() > 0)
                name = Rows[0]["PatinaName"].ToString();
            return name;
        }

        private string GetInsetColorName(int InsetColorID)
        {
            string name = string.Empty;
            DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + InsetColorID);
            if (Rows.Count() > 0)
                name = Rows[0]["InsetColorName"].ToString();
            return name;
        }

        private string GetInsetTypeName(int InsetTypeID)
        {
            string name = string.Empty;
            DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
            if (Rows.Count() > 0)
                name = Rows[0]["InsetTypeName"].ToString();
            return name;
        }

        public string GetProductName(int ProductID)
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

    }
}
