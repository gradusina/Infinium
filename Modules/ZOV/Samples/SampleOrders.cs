using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using DevExpress.XtraTab;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Infinium.Modules.ZOV.Samples
{
    public class DecorCatalogOrder
    {
        private readonly KryptonComboBox _heightEdit;
        private readonly KryptonComboBox _lengthEdit;
        private DataTable _patinaRalDataTable;
        private readonly KryptonComboBox _widthEdit;
        public BindingSource ColorsBindingSource;
        public DataTable ColorsDataTable;
        public BindingSource DecorBindingSource;
        public DataTable DecorConfigDataTable;
        public DataTable DecorDataTable;
        public DataTable DecorParametersDataTable;

        public BindingSource DecorProductsBindingSource;

        public string DecorProductsBindingSourceDisplayMember;

        public string DecorProductsBindingSourceValueMember;

        public int DecorProductsCount;

        public DataTable DecorProductsDataTable;
        public DataTable InsetColorsDataTable;
        public DataTable InsetTypesDataTable;
        public BindingSource ItemColorsBindingSource;
        public string ItemColorsBindingSourceDisplayMember;
        public string ItemColorsBindingSourceValueMember;
        public DataTable ItemColorsDataTable;
        public BindingSource ItemHeightBindingSource;
        public string ItemHeightBindingSourceDisplayMember;
        public DataTable ItemHeightDataTable;
        public BindingSource ItemInsetColorsBindingSource;
        public DataTable ItemInsetColorsDataTable;
        public BindingSource ItemInsetTypesBindingSource;
        public DataTable ItemInsetTypesDataTable;
        public BindingSource ItemLengthBindingSource;
        public string ItemLengthBindingSourceDisplayMember;
        public DataTable ItemLengthDataTable;
        public BindingSource ItemPatinaBindingSource;
        public string ItemPatinaBindingSourceDisplayMember;
        public string ItemPatinaBindingSourceValueMember;
        public DataTable ItemPatinaDataTable;
        public BindingSource ItemsBindingSource;
        public string ItemsBindingSourceDisplayMember;
        public string ItemsBindingSourceValueMember;

        public DataTable ItemsDataTable;
        public BindingSource ItemWidthBindingSource;
        public string ItemWidthBindingSourceDisplayMember;
        public DataTable ItemWidthDataTable;
        public BindingSource PatinaBindingSource;
        public DataTable PatinaDataTable;

        public DataTable TempItemsDataTable;

        public DecorCatalogOrder()
        {
            Initialize();
        }

        public DecorCatalogOrder(ref KryptonComboBox tLengthEdit,
            ref KryptonComboBox tHeightEdit,
            ref KryptonComboBox tWidthEdit)
        {
            _lengthEdit = tLengthEdit;
            _heightEdit = tHeightEdit;
            _widthEdit = tWidthEdit;
        }

        public void ReplaceOldId()
        {
            var frontsOrdersDt = new DataTable();
            var decorOrdersDt = new DataTable();
            var frontsConfigDt = new DataTable();
            var decorConfigDt = new DataTable();

            var selectionCommand = "SELECT * FROM FrontsConfig";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(FrontsConfigDT);
            //}
            frontsConfigDt = TablesManager.FrontsConfigDataTable;
            selectionCommand = "SELECT * FROM DyeingAssignmentDetails";
            using (var da = new SqlDataAdapter(selectionCommand, ConnectionStrings.StorageConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    da.Fill(frontsOrdersDt);
                    for (var i = 0; i < frontsOrdersDt.Rows.Count; i++)
                    {
                        var frontConfigId = Convert.ToInt32(frontsOrdersDt.Rows[i]["FrontConfigID"]);
                        var configRows = frontsConfigDt.Select("FrontConfigID=" + frontConfigId);
                        if (configRows.Count() > 0)
                        {
                            var frontId = Convert.ToInt32(configRows[0]["FrontID"]);
                            var colorId = Convert.ToInt32(configRows[0]["ColorID"]);
                            var insetTypeId = Convert.ToInt32(configRows[0]["InsetTypeID"]);
                            var insetColorId = Convert.ToInt32(configRows[0]["InsetColorID"]);
                            var technoColorId = Convert.ToInt32(configRows[0]["TechnoColorID"]);
                            var technoInsetTypeId = Convert.ToInt32(configRows[0]["TechnoInsetTypeID"]);
                            var technoInsetColorId = Convert.ToInt32(configRows[0]["TechnoInsetColorID"]);
                            var patinaId = Convert.ToInt32(configRows[0]["PatinaID"]);
                            frontsOrdersDt.Rows[i]["FrontID"] = frontId;
                            frontsOrdersDt.Rows[i]["ColorID"] = colorId;
                            frontsOrdersDt.Rows[i]["InsetTypeID"] = insetTypeId;
                            frontsOrdersDt.Rows[i]["InsetColorID"] = insetColorId;
                            frontsOrdersDt.Rows[i]["TechnoColorID"] = technoColorId;
                            frontsOrdersDt.Rows[i]["TechnoInsetTypeID"] = technoInsetTypeId;
                            frontsOrdersDt.Rows[i]["TechnoInsetColorID"] = technoInsetColorId;
                            frontsOrdersDt.Rows[i]["PatinaID"] = patinaId;
                        }
                    }

                    da.Update(frontsOrdersDt);
                }
            }
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

        private void GetColorsDt()
        {
            ColorsDataTable = new DataTable();
            ColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            ColorsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            var selectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (var dt = new DataTable())
                {
                    da.Fill(dt);
                    {
                        var newRow = ColorsDataTable.NewRow();
                        newRow["ColorID"] = -1;
                        newRow["ColorName"] = "-";
                        ColorsDataTable.Rows.Add(newRow);
                    }
                    {
                        var newRow = ColorsDataTable.NewRow();
                        newRow["ColorID"] = 0;
                        newRow["ColorName"] = "на выбор";
                        ColorsDataTable.Rows.Add(newRow);
                    }
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var newRow = ColorsDataTable.NewRow();
                        newRow["ColorID"] = Convert.ToInt64(dt.Rows[i]["TechStoreID"]);
                        newRow["ColorName"] = dt.Rows[i]["TechStoreName"].ToString();
                        ColorsDataTable.Rows.Add(newRow);
                    }
                }
            }

            using (var da = new SqlDataAdapter("SELECT * FROM InsetColors WHERE GroupID IN (2,3,4,5,6,21,22)",
                ConnectionStrings.CatalogConnectionString))
            {
                using (var dt = new DataTable())
                {
                    da.Fill(dt);

                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var rows = ColorsDataTable.Select("ColorID=" + Convert.ToInt32(dt.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            var newRow = ColorsDataTable.NewRow();
                            newRow["ColorID"] = Convert.ToInt64(dt.Rows[i]["InsetColorID"]);
                            newRow["ColorName"] = dt.Rows[i]["InsetColorName"].ToString();
                            ColorsDataTable.Rows.Add(newRow);
                        }
                    }
                }
            }
        }

        private void GetInsetColorsDt()
        {
            InsetColorsDataTable = new DataTable();
            using (var da = new SqlDataAdapter(
                "SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(InsetColorsDataTable);
                {
                    var newRow = InsetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = -1;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(newRow);
                }
                {
                    var newRow = InsetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = 0;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(newRow);
                }
            }
        }

        private void Fill()
        {
            var selectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(DecorProductsDataTable);
                DecorProductsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }

            DecorDataTable = new DataTable();
            selectCommand =
                @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(DecorDataTable);
                DecorDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }

            GetColorsDt();
            GetInsetColorsDt();
            InsetTypesDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(InsetTypesDataTable);
            }

            PatinaDataTable = new DataTable();
            selectCommand = @"SELECT * FROM Patina ORDER BY PatinaName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(PatinaDataTable);
                PatinaDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }

            _patinaRalDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_patinaRalDataTable);
            }

            foreach (DataRow item in _patinaRalDataTable.Rows)
            {
                var newRow = PatinaDataTable.NewRow();
                newRow["PatinaID"] = item["PatinaRALID"];
                newRow["PatinaName"] = item["PatinaRAL"];
                newRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(newRow);
            }

            DecorParametersDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM DecorParameters",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(DecorParametersDataTable);
            }

            TempItemsDataTable = DecorDataTable.Clone();

            using (var dv = new DataView(DecorDataTable))
            {
                ItemsDataTable = dv.ToTable(true, "Name");
            }

            DecorProductsCount = DecorProductsDataTable.Rows.Count;

            ItemColorsDataTable = ColorsDataTable.Clone();
            ItemPatinaDataTable = PatinaDataTable.Clone();
            ItemInsetTypesDataTable = InsetTypesDataTable.Clone();
            ItemInsetColorsDataTable = InsetColorsDataTable.Clone();

            selectCommand = @"SELECT * FROM DecorConfig" +
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
            ItemInsetTypesBindingSource = new BindingSource
            {
                DataSource = ItemInsetTypesDataTable
            };
            ItemInsetColorsBindingSource = new BindingSource
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
        public bool HasParameter(int productId, string parameter)
        {
            var rows = DecorParametersDataTable.Select("ProductID = " + productId);

            return Convert.ToBoolean(rows[0][parameter]);
        }

        private bool HasColorParameter(DataRow[] rows, int colorId)
        {
            foreach (var row in rows)
                if (row["ColorID"].ToString() == colorId.ToString())
                    return true;

            return false;
        }

        private bool HasPatinaParameter(DataRow[] rows, int colorId)
        {
            foreach (var row in rows)
                if (row["PatinaID"].ToString() == colorId.ToString())
                    return true;

            return false;
        }

        private bool HasHeightParameter(DataRow[] rows, int height)
        {
            foreach (var row in rows)
                if (row["Height"].ToString() == height.ToString())
                    return true;

            return false;
        }

        private bool HasLengthParameter(DataRow[] rows, int length)
        {
            foreach (var row in rows)
                if (row["Length"].ToString() == length.ToString())
                    return true;

            return false;
        }

        private bool HasWidthParameter(DataRow[] rows, int width)
        {
            foreach (var row in rows)
                if (row["Width"].ToString() == width.ToString())
                    return true;

            return false;
        }

        public string GetItemName(int decorId)
        {
            return DecorDataTable.Select("DecorID = " + decorId)[0]["Name"].ToString();
        }

        public int GetDecorConfigId(int productId, int decorId, int colorId, int patinaId, int insetTypeId,
            int insetColorId, int length, int height, int width, ref int factoryId)
        {
            string lengthFilter = null;
            string heightFilter = null;
            string widthFilter = null;
            string colorFilter = null;
            string patinaFilter = null;
            string insetTypeFilter = null;
            string insetColorFilter = null;

            if (patinaId > 1000)
            {
                var fRows = _patinaRalDataTable.Select("PatinaRALID=" + patinaId);
                if (fRows.Count() > 0)
                    patinaId = Convert.ToInt32(fRows[0]["PatinaID"]);
            }

            var rows = DecorConfigDataTable.Select(
                "ProductID = " + Convert.ToInt32(productId) + " AND " +
                "DecorID = " + Convert.ToInt32(decorId));


            colorFilter = " AND ColorID = " + colorId;
            patinaFilter = " AND PatinaID = " + patinaId;
            insetTypeFilter = " AND InsetTypeID = " + insetTypeId;
            insetColorFilter = " AND InsetColorID = " + insetColorId;
            //if (HasColorParameter(Rows, ColorID))
            //    ColorFilter = " AND ColorID = " + ColorID;
            //else
            //    ColorFilter = " AND ColorID = -1";

            //if (HasPatinaParameter(Rows, PatinaID))
            //    PatinaFilter = " AND PatinaID = " + PatinaID;
            //else
            //    PatinaFilter = " AND PatinaID = -1";

            if (HasLengthParameter(rows, length))
                lengthFilter = " AND Length = " + length;
            else
                lengthFilter = " AND Length = 0";

            if (length == -1)
                lengthFilter = " AND Length = -1";

            if (HasHeightParameter(rows, height))
                heightFilter = " AND Height = " + height;
            else
                heightFilter = " AND Height = 0";

            if (height == -1)
                heightFilter = " AND Height = -1";

            if (HasWidthParameter(rows, width))
                widthFilter = " AND Width = " + width;
            else
                widthFilter = " AND Width = 0";

            if (width == -1)
                widthFilter = " AND Width = -1";

            rows = DecorConfigDataTable.Select("(Excluzive IS NULL OR Excluzive=1) AND " +
                                               "ProductID = " + Convert.ToInt32(productId) + " AND " +
                                               "DecorID = " + Convert.ToInt32(decorId) +
                                               colorFilter + patinaFilter + insetTypeFilter + insetColorFilter +
                                               lengthFilter + heightFilter + widthFilter);

            if (rows.Count() < 1 || rows.Count() > 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог\r\n" +
                                GetDecorName(Convert.ToInt32(decorId)) + GetColorName(Convert.ToInt32(colorId)));
                return -1;
            }

            factoryId = Convert.ToInt32(rows[0]["FactoryID"]);

            if (rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(rows[0]["Excluzive"]) == 0)
            {
                MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
                return -1;
            }

            return Convert.ToInt32(rows[0]["DecorConfigID"]);
        }

        public int GetDecorConfigId(int productId, string name, int colorId, int patinaId, int insetTypeId,
            int insetColorId, int length, int height, int width, ref int decorId, ref int factoryId)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            using (var dv = new DataView(TempItemsDataTable))
            {
                dv.RowFilter = "Name='" + name + "'";

                TempItemsDataTable = dv.ToTable();
            }

            var filter = string.Empty;
            for (var i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
            {
                filter = "DecorID <> - 1";
            }

            string lengthFilter = null;
            string heightFilter = null;
            string widthFilter = null;
            string colorFilter = null;
            string patinaFilter = null;
            string insetTypeFilter = null;
            string insetColorFilter = null;

            if (patinaId > 1000)
            {
                var fRows = _patinaRalDataTable.Select("PatinaRALID=" + patinaId);
                if (fRows.Count() > 0)
                    patinaId = Convert.ToInt32(fRows[0]["PatinaID"]);
            }

            var rows = DecorConfigDataTable.Select(
                "ProductID = " + Convert.ToInt32(productId) + " AND " + filter);


            colorFilter = " AND ColorID = " + colorId;
            patinaFilter = " AND PatinaID = " + patinaId;
            insetTypeFilter = " AND InsetTypeID = " + insetTypeId;
            insetColorFilter = " AND InsetColorID = " + insetColorId;
            //if (HasColorParameter(Rows, ColorID))
            //    ColorFilter = " AND ColorID = " + ColorID;
            //else
            //    ColorFilter = " AND ColorID = -1";

            //if (HasPatinaParameter(Rows, PatinaID))
            //    PatinaFilter = " AND PatinaID = " + PatinaID;
            //else
            //    PatinaFilter = " AND PatinaID = -1";

            if (HasLengthParameter(rows, length))
                lengthFilter = " AND Length = " + length;
            else
                lengthFilter = " AND Length = 0";

            if (length == -1)
                lengthFilter = " AND Length = -1";

            if (HasHeightParameter(rows, height))
                heightFilter = " AND Height = " + height;
            else
                heightFilter = " AND Height = 0";

            if (height == -1)
                heightFilter = " AND Height = -1";

            if (HasWidthParameter(rows, width))
                widthFilter = " AND Width = " + width;
            else
                widthFilter = " AND Width = 0";

            if (width == -1)
                widthFilter = " AND Width = -1";


            rows = DecorConfigDataTable.Select("(Excluzive IS NULL OR Excluzive=1) AND " +
                                               "ProductID = " + Convert.ToInt32(productId) + " AND " + filter +
                                               colorFilter + patinaFilter + insetTypeFilter + insetColorFilter +
                                               lengthFilter + heightFilter + widthFilter);

            if (rows.Count() < 1 || rows.Count() > 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог");
                return -1;
            }

            factoryId = Convert.ToInt32(rows[0]["FactoryID"]);
            decorId = Convert.ToInt32(rows[0]["DecorID"]);

            if (rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(rows[0]["Excluzive"]) == 0)
            {
                MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
                return -1;
            }

            return Convert.ToInt32(rows[0]["DecorConfigID"]);
        }

        private string GetDecorName(int decorId)
        {
            var name = string.Empty;
            try
            {
                var rows = DecorDataTable.Select("DecorID = " + decorId);
                name = rows[0]["Name"].ToString();
            }
            catch
            {
                return string.Empty;
            }

            return name;
        }

        public string GetColorName(int colorId)
        {
            var colorName = string.Empty;
            try
            {
                var rows = ColorsDataTable.Select("ColorID = " + colorId);
                colorName = rows[0]["ColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }

            return colorName;
        }

        public string GetPatinaName(int patinaId)
        {
            var patinaName = string.Empty;
            try
            {
                var rows = PatinaDataTable.Select("PatinaID = " + patinaId);
                patinaName = rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }

            return patinaName;
        }

        public string GetInsetTypeName(int insetTypeId)
        {
            var insetTypeName = string.Empty;
            try
            {
                var rows = InsetTypesDataTable.Select("InsetTypeID = " + insetTypeId);
                insetTypeName = rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }

            return insetTypeName;
        }

        public string GetInsetColorName(int insetColorId)
        {
            var insetColorName = string.Empty;
            try
            {
                var rows = InsetColorsDataTable.Select("InsetColorID = " + insetColorId);
                insetColorName = rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }

            return insetColorName;
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
            var rowFilter = "Excluzive=1";
            if (!bExcluzive)
                rowFilter = "Excluzive IS NULL OR Excluzive<>0";

            DecorProductsBindingSource.Filter = rowFilter;
            DecorProductsBindingSource.MoveFirst();
        }

        public void FilterItems(int productId, bool bExcluzive)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            var rowFilter = "ProductID=" + productId;
            using (var dv = new DataView(TempItemsDataTable))
            {
                if (bExcluzive)
                    rowFilter += " AND (Excluzive=1)";
                else
                    rowFilter += " AND (Excluzive IS NULL OR Excluzive=1)";
                dv.RowFilter = rowFilter;
                TempItemsDataTable = dv.ToTable(true, "Name");
            }

            ItemsDataTable.Clear();
            for (var d = 0; d < TempItemsDataTable.Rows.Count; d++)
            {
                var newRow = ItemsDataTable.NewRow();
                newRow["Name"] = TempItemsDataTable.Rows[d]["Name"].ToString();
                ItemsDataTable.Rows.Add(newRow);
            }

            ItemsDataTable.DefaultView.Sort = "Name ASC";
        }

        public bool FilterColors(string name, bool bExcluzive)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            using (var dv = new DataView(TempItemsDataTable))
            {
                dv.RowFilter = "Name='" + name + "'";

                TempItemsDataTable = dv.ToTable();
            }

            var filter = string.Empty;
            for (var i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
            {
                filter = "DecorID <> - 1";
            }

            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";
            ItemColorsDataTable.Clear();
            ItemColorsDataTable.AcceptChanges();

            var dcr = DecorConfigDataTable.Select(filter);


            for (var d = 0; d < dcr.Count(); d++)
            {
                if (d == 0)
                {
                    var newRow = ItemColorsDataTable.NewRow();
                    newRow["ColorID"] = dcr[d]["ColorID"];
                    newRow["ColorName"] = GetColorName(Convert.ToInt32(dcr[d]["ColorID"]));
                    ItemColorsDataTable.Rows.Add(newRow);
                    continue;
                }


                for (var i = 0; i < ItemColorsDataTable.Rows.Count; i++)
                {
                    if (ItemColorsDataTable.Rows[i]["ColorID"].ToString() == dcr[d]["ColorID"].ToString())
                        break;

                    if (i == ItemColorsDataTable.Rows.Count - 1)
                    {
                        var newRow = ItemColorsDataTable.NewRow();
                        newRow["ColorID"] = dcr[d]["ColorID"];
                        newRow["ColorName"] = GetColorName(Convert.ToInt32(dcr[d]["ColorID"]));
                        ItemColorsDataTable.Rows.Add(newRow);
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

        public bool FilterPatina(string name, int colorId, bool bExcluzive)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            using (var dv = new DataView(TempItemsDataTable))
            {
                dv.RowFilter = "Name='" + name + "'";

                TempItemsDataTable = dv.ToTable();
            }

            var filter = string.Empty;
            for (var i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
            {
                filter = "DecorID <> - 1";
            }

            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";
            ItemPatinaDataTable.Clear();
            ItemPatinaDataTable.AcceptChanges();

            var dcr = DecorConfigDataTable.Select(filter + " AND ColorID = " + colorId);


            for (var d = 0; d < dcr.Count(); d++)
            {
                if (d == 0)
                {
                    var newRow = ItemPatinaDataTable.NewRow();
                    newRow["PatinaID"] = dcr[d]["PatinaID"];
                    newRow["PatinaName"] = GetPatinaName(Convert.ToInt32(dcr[d]["PatinaID"]));
                    ItemPatinaDataTable.Rows.Add(newRow);
                    continue;
                }


                for (var i = 0; i < ItemPatinaDataTable.Rows.Count; i++)
                {
                    if (ItemPatinaDataTable.Rows[i]["PatinaID"].ToString() == dcr[d]["PatinaID"].ToString())
                        break;

                    if (i == ItemPatinaDataTable.Rows.Count - 1)
                    {
                        var newRow = ItemPatinaDataTable.NewRow();
                        newRow["PatinaID"] = dcr[d]["PatinaID"];
                        newRow["PatinaName"] = GetPatinaName(Convert.ToInt32(dcr[d]["PatinaID"]));
                        ItemPatinaDataTable.Rows.Add(newRow);
                        break;
                    }
                }
            }

            var hasPatinaRal = false;
            if (ItemPatinaDataTable.Rows.Count > 0)
            {
                var fRows = _patinaRalDataTable.Select("PatinaID=" +
                                                       Convert.ToInt32(ItemPatinaDataTable.Rows[0]["PatinaID"]));
                if (fRows.Count() > 0)
                    hasPatinaRal = true;
            }

            if (ItemPatinaDataTable.Rows.Count > 0 && hasPatinaRal)
            {
                var ddd = ItemPatinaDataTable.Copy();
                ItemPatinaDataTable.Clear();
                ItemPatinaDataTable.AcceptChanges();

                foreach (DataRow row in ddd.Rows)
                    foreach (var item in _patinaRalDataTable.Select("PatinaID=" + Convert.ToInt32(row["PatinaID"])))
                    {
                        var newRow = ItemPatinaDataTable.NewRow();
                        newRow["PatinaID"] = item["PatinaRALID"];
                        newRow["PatinaName"] = item["PatinaRAL"];
                        newRow["DisplayName"] = item["DisplayName"];
                        ItemPatinaDataTable.Rows.Add(newRow);
                    }
            }

            ItemPatinaDataTable.DefaultView.Sort = "PatinaName ASC";
            ItemPatinaBindingSource.MoveFirst();

            if (ItemPatinaDataTable.Rows.Count == 1 && ItemPatinaDataTable.Rows[0]["PatinaID"].ToString() == "-1")
                return false;

            return true;
        }

        public int FilterLength(string name, int colorId, int patinaId)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            using (var dv = new DataView(TempItemsDataTable))
            {
                dv.RowFilter = "Name='" + name + "'";

                TempItemsDataTable = dv.ToTable();
            }

            var filter = string.Empty;
            for (var i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
            {
                filter = "DecorID <> - 1";
            }

            ItemLengthDataTable.Clear();
            ItemLengthDataTable.AcceptChanges();

            if (patinaId > 1000)
            {
                var fRows = _patinaRalDataTable.Select("PatinaRALID=" + patinaId);
                if (fRows.Count() > 0)
                    patinaId = Convert.ToInt32(fRows[0]["PatinaID"]);
            }

            var dcr = DecorConfigDataTable.Select(filter + " AND ColorID = " + colorId
                                                  + " AND PatinaID = " + patinaId);


            for (var d = 0; d < dcr.Count(); d++)
            {
                if (d == 0)
                {
                    var length = Convert.ToInt32(dcr[d]["Length"]);

                    var newRow = ItemLengthDataTable.NewRow();
                    newRow["Length"] = length;
                    ItemLengthDataTable.Rows.Add(newRow);
                    continue;
                }


                for (var i = 0; i < ItemLengthDataTable.Rows.Count; i++)
                {
                    if (ItemLengthDataTable.Rows[i]["Length"].ToString() == dcr[d]["Length"].ToString())
                        break;

                    if (i == ItemLengthDataTable.Rows.Count - 1)
                    {
                        var height = Convert.ToInt32(dcr[d]["Length"]);

                        var newRow = ItemLengthDataTable.NewRow();
                        newRow["Length"] = height;
                        ItemLengthDataTable.Rows.Add(newRow);
                        break;
                    }
                }
            }

            ItemLengthDataTable.DefaultView.Sort = "Length ASC";

            _lengthEdit.Text = "";
            _lengthEdit.Items.Clear();
            if (ItemLengthDataTable.Rows.Count > 0)
            {
                if (Convert.ToInt32(ItemLengthDataTable.Rows[0]["Length"]) == 0)
                {
                    _lengthEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    return 0;
                }

                if (Convert.ToInt32(ItemLengthDataTable.Rows[0]["Length"]) == -1) return -1;
            }

            foreach (DataRow row in ItemLengthDataTable.Rows)
                _lengthEdit.Items.Add(row["Length"].ToString());

            _lengthEdit.DropDownStyle = ComboBoxStyle.DropDownList;
            if (ItemLengthDataTable.Rows.Count > 0)
                _lengthEdit.SelectedIndex = 0;

            return _lengthEdit.Items.Count;
        }

        public int FilterHeight(string name, int colorId, int patinaId, int length)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            using (var dv = new DataView(TempItemsDataTable))
            {
                dv.RowFilter = "Name='" + name + "'";

                TempItemsDataTable = dv.ToTable();
            }

            var filter = string.Empty;
            for (var i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
            {
                filter = "DecorID <> - 1";
            }

            ItemHeightDataTable.Clear();
            ItemHeightDataTable.AcceptChanges();

            if (patinaId > 1000)
            {
                var fRows = _patinaRalDataTable.Select("PatinaRALID=" + patinaId);
                if (fRows.Count() > 0)
                    patinaId = Convert.ToInt32(fRows[0]["PatinaID"]);
            }

            var dcr = DecorConfigDataTable.Select(filter + " AND ColorID = " + colorId
                                                  + " AND PatinaID = " + patinaId + " AND Length = " + length);


            for (var d = 0; d < dcr.Count(); d++)
            {
                if (d == 0)
                {
                    var height = Convert.ToInt32(dcr[d]["Height"]);

                    var newRow = ItemHeightDataTable.NewRow();
                    newRow["Height"] = height;
                    ItemHeightDataTable.Rows.Add(newRow);
                    continue;
                }


                for (var i = 0; i < ItemHeightDataTable.Rows.Count; i++)
                {
                    if (ItemHeightDataTable.Rows[i]["Height"].ToString() == dcr[d]["Height"].ToString())
                        break;

                    if (i == ItemHeightDataTable.Rows.Count - 1)
                    {
                        var height = Convert.ToInt32(dcr[d]["Height"]);

                        var newRow = ItemHeightDataTable.NewRow();
                        newRow["Height"] = height;
                        ItemHeightDataTable.Rows.Add(newRow);
                        break;
                    }
                }
            }

            ItemHeightDataTable.DefaultView.Sort = "Height ASC";

            _heightEdit.Text = "";
            _heightEdit.Items.Clear();
            if (ItemHeightDataTable.Rows.Count > 0)
            {
                if (Convert.ToInt32(ItemHeightDataTable.Rows[0]["Height"]) == 0)
                {
                    _heightEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    return 0;
                }

                if (Convert.ToInt32(ItemHeightDataTable.Rows[0]["Height"]) == -1) return -1;
            }

            foreach (DataRow row in ItemHeightDataTable.Rows)
                _heightEdit.Items.Add(row["Height"].ToString());

            _heightEdit.DropDownStyle = ComboBoxStyle.DropDownList;
            if (ItemHeightDataTable.Rows.Count > 0)
                _heightEdit.SelectedIndex = 0;

            return _heightEdit.Items.Count;
        }

        public int FilterWidth(string name, int colorId, int patinaId, int length, int height)
        {
            TempItemsDataTable.Clear();
            TempItemsDataTable = DecorDataTable;
            using (var dv = new DataView(TempItemsDataTable))
            {
                dv.RowFilter = "Name='" + name + "'";

                TempItemsDataTable = dv.ToTable();
            }

            var filter = string.Empty;
            for (var i = 0; i < TempItemsDataTable.Rows.Count; i++)
                filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["DecorID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DecorID IN (" + filter + ")";
            }
            else
            {
                filter = "DecorID <> - 1";
            }

            ItemWidthDataTable.Clear();
            ItemWidthDataTable.AcceptChanges();

            if (patinaId > 1000)
            {
                var fRows = _patinaRalDataTable.Select("PatinaRALID=" + patinaId);
                if (fRows.Count() > 0)
                    patinaId = Convert.ToInt32(fRows[0]["PatinaID"]);
            }

            var dcr = DecorConfigDataTable.Select(filter
                                                  + " AND ColorID = " + colorId + " AND PatinaID = " + patinaId +
                                                  " AND Length = " + length + " AND Height = " + height);


            for (var d = 0; d < dcr.Count(); d++)
            {
                if (d == 0)
                {
                    var width = Convert.ToInt32(dcr[d]["Width"]);

                    var newRow = ItemWidthDataTable.NewRow();
                    newRow["Width"] = width;
                    ItemWidthDataTable.Rows.Add(newRow);
                    continue;
                }


                for (var i = 0; i < ItemWidthDataTable.Rows.Count; i++)
                {
                    if (ItemWidthDataTable.Rows[i]["Width"].ToString() == dcr[d]["Width"].ToString())
                        break;

                    if (i == ItemWidthDataTable.Rows.Count - 1)
                    {
                        var width = Convert.ToInt32(dcr[d]["Width"]);

                        var newRow = ItemWidthDataTable.NewRow();
                        newRow["Width"] = width;
                        ItemWidthDataTable.Rows.Add(newRow);
                        break;
                    }
                }
            }

            ItemWidthDataTable.DefaultView.Sort = "Width ASC";

            _widthEdit.Text = "";
            _widthEdit.Items.Clear();
            if (ItemWidthDataTable.Rows.Count > 0)
            {
                if (Convert.ToInt32(ItemWidthDataTable.Rows[0]["Width"]) == 0)
                {
                    _widthEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    return 0;
                }

                if (Convert.ToInt32(ItemWidthDataTable.Rows[0]["Width"]) == -1) return -1;
            }

            foreach (DataRow row in ItemWidthDataTable.Rows)
                _widthEdit.Items.Add(row["Width"].ToString());

            _widthEdit.DropDownStyle = ComboBoxStyle.DropDownList;
            if (ItemWidthDataTable.Rows.Count > 0)
                _widthEdit.SelectedIndex = 0;

            return _widthEdit.Items.Count;
        }
    }

    public class ZFrontsOrders
    {
        private int _currentMainOrderId = -1;
        private BindingSource _frameColorsBindingSource;
        private DataGridViewComboBoxColumn _frameColorsColumn;
        private BindingSource _frontsBindingSource;

        private DataGridViewComboBoxColumn _frontsColumn;
        private readonly PercentageDataGrid _frontsOrdersDataGrid;
        private BindingSource _insetColorsBindingSource;
        private DataGridViewComboBoxColumn _insetColorsColumn;
        private BindingSource _insetTypesBindingSource;
        private DataGridViewComboBoxColumn _insetTypesColumn;
        private BindingSource _patinaBindingSource;
        private DataGridViewComboBoxColumn _patinaColumn;
        private BindingSource _technoFrameColorsBindingSource;
        private DataGridViewComboBoxColumn _technoFrameColorsColumn;
        private BindingSource _technoInsetColorsBindingSource;
        private DataGridViewComboBoxColumn _technoInsetColorsColumn;
        private DataTable _technoInsetColorsDataTable;
        private BindingSource _technoInsetTypesBindingSource;
        private DataGridViewComboBoxColumn _technoInsetTypesColumn;
        private DataTable _technoInsetTypesDataTable;
        private DataGridViewComboBoxColumn _technoProfilesColumn;
        private DataTable _technoProfilesDataTable;
        public DataTable FrameColorsDataTable;
        public DataTable FrontsDataTable;

        public BindingSource FrontsOrdersBindingSource;

        public DataTable FrontsOrdersDataTable;
        public DataTable InsetColorsDataTable;
        public DataTable InsetMarginsDataTable;
        public DataTable InsetTypesDataTable;
        public DataTable MeasuresDataTable;
        public DataTable PatinaDataTable;

        public ZFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            _frontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }

        private void Create()
        {
            FrontsOrdersDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            _technoInsetTypesDataTable = new DataTable();
            _technoInsetColorsDataTable = new DataTable();

            FrontsOrdersBindingSource = new BindingSource();
            _frontsBindingSource = new BindingSource();
            _frameColorsBindingSource = new BindingSource();
            _patinaBindingSource = new BindingSource();
            _insetTypesBindingSource = new BindingSource();
            _insetColorsBindingSource = new BindingSource();
            _technoFrameColorsBindingSource = new BindingSource();
            _technoInsetTypesBindingSource = new BindingSource();
            _technoInsetColorsBindingSource = new BindingSource();
        }

        private void GetColorsDt()
        {
            FrameColorsDataTable = new DataTable();
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            var selectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (var dt = new DataTable())
                {
                    da.Fill(dt);
                    {
                        var newRow = FrameColorsDataTable.NewRow();
                        newRow["ColorID"] = -1;
                        newRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(newRow);
                    }
                    {
                        var newRow = FrameColorsDataTable.NewRow();
                        newRow["ColorID"] = 0;
                        newRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(newRow);
                    }
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var newRow = FrameColorsDataTable.NewRow();
                        newRow["ColorID"] = Convert.ToInt64(dt.Rows[i]["TechStoreID"]);
                        newRow["ColorName"] = dt.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(newRow);
                    }
                }
            }
        }

        private void GetInsetColorsDt()
        {
            InsetColorsDataTable = new DataTable();
            using (var da = new SqlDataAdapter(
                "SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(InsetColorsDataTable);
                {
                    var newRow = InsetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = -1;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(newRow);
                }
                {
                    var newRow = InsetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = 0;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(newRow);
                }
            }
        }

        private void Fill()
        {
            var selectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(FrontsDataTable);
            }

            selectCommand =
                @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                _technoProfilesDataTable = new DataTable();
                da.Fill(_technoProfilesDataTable);

                var newRow = _technoProfilesDataTable.NewRow();
                newRow["TechnoProfileID"] = -1;
                newRow["TechnoProfileName"] = "-";
                _technoProfilesDataTable.Rows.InsertAt(newRow, 0);
            }

            GetColorsDt();
            GetInsetColorsDt();

            using (var da = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(InsetTypesDataTable);
            }

            using (var da = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(PatinaDataTable);
            }

            _technoInsetTypesDataTable = InsetTypesDataTable.Copy();
            _technoInsetColorsDataTable = InsetColorsDataTable.Copy();

            MeasuresDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(MeasuresDataTable);
            }

            InsetMarginsDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM InsetMargins",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(InsetMarginsDataTable);
            }


            using (var da = new SqlDataAdapter("SELECT TOP 0 * FROM SampleFrontsOrders",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                da.Fill(FrontsOrdersDataTable);
            }
        }

        private void Binding()
        {
            _frontsBindingSource.DataSource = FrontsDataTable;
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            _patinaBindingSource.DataSource = PatinaDataTable;
            _insetTypesBindingSource.DataSource = InsetTypesDataTable;
            _frameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            _insetColorsBindingSource.DataSource = InsetColorsDataTable;
            _technoFrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            _technoInsetTypesBindingSource.DataSource = _technoInsetTypesDataTable;
            _technoInsetColorsBindingSource.DataSource = _technoInsetColorsDataTable;

            _frontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
        }

        private void CreateColumns(bool showPrice)
        {
            if (_frontsColumn != null)
                return;

            //создание столбцов
            _frontsColumn = new DataGridViewComboBoxColumn
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = _frontsBindingSource,
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _frameColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = _frameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _patinaColumn = new DataGridViewComboBoxColumn
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = _patinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _insetTypesColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = _insetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _insetColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = _insetColorsBindingSource,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoProfilesColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoProfilesColumn",
                HeaderText = "Тип\r\nпрофиля-2",
                DataPropertyName = "TechnoProfileID",
                DataSource = new DataView(_technoProfilesDataTable),
                ValueMember = "TechnoProfileID",
                DisplayMember = "TechnoProfileName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoFrameColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoFrameColorsColumn",
                HeaderText = "Цвет профиля-2",
                DataPropertyName = "TechnoColorID",
                DataSource = _technoFrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoInsetTypesColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = _technoInsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoInsetColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = _technoInsetColorsBindingSource,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _frontsOrdersDataGrid.AutoGenerateColumns = false;

            //добавление столбцов
            _frontsOrdersDataGrid.Columns.Add(_frontsColumn);
            _frontsOrdersDataGrid.Columns.Add(_frameColorsColumn);
            _frontsOrdersDataGrid.Columns.Add(_patinaColumn);
            _frontsOrdersDataGrid.Columns.Add(_insetTypesColumn);
            _frontsOrdersDataGrid.Columns.Add(_insetColorsColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoProfilesColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoFrameColorsColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoInsetTypesColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoInsetColorsColumn);

            //убирание лишних столбцов
            if (_frontsOrdersDataGrid.Columns.Contains("NeedCalcPrice"))
                _frontsOrdersDataGrid.Columns["NeedCalcPrice"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("AreaID"))
                _frontsOrdersDataGrid.Columns["AreaID"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                _frontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                _frontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                _frontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                _frontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }

            if (_frontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                _frontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                _frontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            _frontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                _frontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            _frontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            _frontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            _frontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            _frontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            _frontsOrdersDataGrid.Columns["CupboardString"].Visible = false;

            if (_frontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                _frontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                _frontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;


            if (!Security.PriceAccess || !showPrice)
            {
                _frontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                _frontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                _frontsOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            _frontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            _frontsOrdersDataGrid.Columns["Debt"].Visible = false;

            _frontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            //MainOrdersFrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            _frontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;

            var displayIndex = 0;
            _frontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = displayIndex++;

            foreach (DataGridViewColumn column in _frontsOrdersDataGrid.Columns)
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //названия столбцов
            _frontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
            _frontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            _frontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            _frontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            _frontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            _frontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            _frontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            _frontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\n  фасад";
            _frontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            _frontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            _frontsOrdersDataGrid.Columns["Weight"].HeaderText = "Вес";

            _frontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            _frontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["Height"].Width = 85;
            _frontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["Width"].Width = 85;
            _frontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["Count"].Width = 85;
            _frontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["Cost"].Width = 120;
            _frontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["Square"].Width = 100;
            _frontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["FrontPrice"].Width = 85;
            _frontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["InsetPrice"].Width = 85;
            _frontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _frontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            _frontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _frontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
        }

        public void Initialize(bool showPrice)
        {
            Create();
            Fill();
            Binding();
            CreateColumns(showPrice);
        }

        public bool Filter(int mainOrderId, int factoryId)
        {
            if (_currentMainOrderId == mainOrderId)
                return FrontsOrdersDataTable.Rows.Count > 0;

            _currentMainOrderId = mainOrderId;

            var factoryFilter = "";

            if (factoryId != 0)
                factoryFilter = " AND FactoryID = " + factoryId;

            FrontsOrdersDataTable.Clear();

            using (var da = new SqlDataAdapter(
                "SELECT * FROM SampleFrontsOrders WHERE MainOrderID = " + mainOrderId + factoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                da.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }
    }

    public class ZDecorOrders
    {
        private int _currentMainOrderId = -1;

        private readonly DecorCatalogOrder _decorCatalogOrder;

        private readonly XtraTabControl _decorTabControl;

        private readonly PercentageDataGrid _mainOrdersFrontsOrdersDataGrid;

        public bool Debts = false;
        public BindingSource[] DecorItemOrdersBindingSources;
        public PercentageDataGrid[] DecorItemOrdersDataGrids;
        public DataTable[] DecorItemOrdersDataTables;
        public SqlCommandBuilder DecorOrdersCommandBuilder;
        public SqlDataAdapter DecorOrdersDataAdapter;

        public DataTable DecorOrdersDataTable;

        //конструктор
        public ZDecorOrders(ref XtraTabControl tDecorTabControl,
            ref DecorCatalogOrder tDecorCatalogOrder,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            _decorTabControl = tDecorTabControl;
            _decorCatalogOrder = tDecorCatalogOrder;

            _mainOrdersFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }


        private void Create()
        {
            DecorOrdersDataTable = new DataTable();
            DecorItemOrdersDataTables = new DataTable[_decorCatalogOrder.DecorProductsCount];
            DecorItemOrdersBindingSources = new BindingSource[_decorCatalogOrder.DecorProductsCount];
            DecorItemOrdersDataGrids = new PercentageDataGrid[_decorCatalogOrder.DecorProductsCount];

            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = new DataTable();
                DecorItemOrdersBindingSources[i] = new BindingSource();
            }
        }

        private void Fill()
        {
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM SampleDecorOrders",
                ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
        }

        private void Binding()
        {
        }

        public void Initialize(bool showPrice)
        {
            Create();
            Fill();
            Binding();

            SplitDecorOrdersTables();
            GridSettings(showPrice);
        }

        private DataGridViewComboBoxColumn CreateInsetTypesColumn()
        {
            var itemColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",

                DataSource = new DataView(_decorCatalogOrder.InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return itemColumn;
        }

        private DataGridViewComboBoxColumn CreateInsetColorsColumn()
        {
            var itemColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",

                DataSource = new DataView(_decorCatalogOrder.InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return itemColumn;
        }

        private DataGridViewComboBoxColumn CreateColorColumn()
        {
            var colorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "ColorsColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",

                DataSource = _decorCatalogOrder.ColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return colorsColumn;
        }

        private DataGridViewComboBoxColumn CreatePatinaColumn()
        {
            var patinaColumn = new DataGridViewComboBoxColumn
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",

                DataSource = _decorCatalogOrder.PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return patinaColumn;
        }

        private DataGridViewComboBoxColumn CreateItemColumn()
        {
            var itemColumn = new DataGridViewComboBoxColumn
            {
                Name = "ItemColumn",
                HeaderText = "Название",
                DataPropertyName = "DecorID",

                DataSource = new DataView(_decorCatalogOrder.DecorDataTable),
                ValueMember = "DecorID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return itemColumn;
        }

        private void SplitDecorOrdersTables()
        {
            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = DecorOrdersDataTable.Clone();
                DecorItemOrdersBindingSources[i].DataSource = DecorItemOrdersDataTables[i];
            }
        }

        private void GridSettings(bool showPrice)
        {
            _decorTabControl.AppearancePage.Header.BackColor = Color.FromArgb(140, 140, 140);
            _decorTabControl.AppearancePage.Header.BackColor2 = Color.FromArgb(140, 140, 140);
            _decorTabControl.AppearancePage.Header.BorderColor = Color.Black;
            _decorTabControl.AppearancePage.Header.Font =
                new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _decorTabControl.AppearancePage.Header.Options.UseBackColor = true;
            _decorTabControl.AppearancePage.Header.Options.UseBorderColor = true;
            _decorTabControl.AppearancePage.Header.Options.UseFont = true;
            _decorTabControl.LookAndFeel.SkinName = "Office 2010 Black";
            _decorTabControl.LookAndFeel.UseDefaultLookAndFeel = false;

            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
            {
                _decorTabControl.TabPages.Add(_decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"]
                    .ToString());
                _decorTabControl.TabPages[i].PageVisible = false;
                _decorTabControl.TabPages[i].Text =
                    _decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString();

                DecorItemOrdersDataGrids[i] = new PercentageDataGrid
                {
                    Parent = _decorTabControl.TabPages[i],
                    DataSource = DecorItemOrdersBindingSources[i],
                    Dock = DockStyle.Fill,
                    PaletteMode = PaletteMode.Office2010Black
                };
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = Color.White;
                DecorItemOrdersDataGrids[i].AllowUserToAddRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToDeleteRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToResizeRows = false;
                DecorItemOrdersDataGrids[i].RowHeadersVisible = false;
                DecorItemOrdersDataGrids[i].AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.Background.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.Background.ColorStyle =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.Background.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.ColorStyle =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Border.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Font =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Content.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.ColorStyle =
                    _mainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Border.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].RowTemplate.Height = _mainOrdersFrontsOrdersDataGrid.RowTemplate.Height;
                DecorItemOrdersDataGrids[i].ColumnHeadersHeight = _mainOrdersFrontsOrdersDataGrid.ColumnHeadersHeight;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.ColorStyle = PaletteColorStyle.Solid;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Border.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Font =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLine =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLine;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLineH =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLineH;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.TextH =
                    _mainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.TextH;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 =
                    _mainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
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

                if (!Security.PriceAccess || !showPrice)
                {
                    DecorItemOrdersDataGrids[i].Columns["Price"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["Cost"].Visible = false;
                }

                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetColorsColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetTypesColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateColorColumn());
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreatePatinaColumn());
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateItemColumn());
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].MinimumWidth = 120;

                //убирание лишних столбцов
                if (DecorItemOrdersDataGrids[i].Columns.Contains("NeedCalcPrice"))
                    DecorItemOrdersDataGrids[i].Columns["NeedCalcPrice"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("AreaID"))
                    DecorItemOrdersDataGrids[i].Columns["AreaID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateDateTime"))
                {
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].HeaderText = "Добавлено";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.None;
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].Width = 120;
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

                DecorItemOrdersDataGrids[i].Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Cost"].MinimumWidth = 90;
                DecorItemOrdersDataGrids[i].Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Price"].MinimumWidth = 90;
                DecorItemOrdersDataGrids[i].Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Notes"].MinimumWidth = 120;

                for (var j = 2; j < DecorItemOrdersDataGrids[i].Columns.Count; j++)
                {
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Height")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Высота";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Length")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Длина";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Width")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Ширина";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Count")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Кол-во";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Notes")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Примечание";
                }

                foreach (DataGridViewColumn column in DecorItemOrdersDataGrids[i].Columns)
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (_decorCatalogOrder.HasParameter(
                    Convert.ToInt32(_decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "ColorID"))
                {
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 190;
                }

                if (_decorCatalogOrder.HasParameter(
                    Convert.ToInt32(_decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Height"))
                {
                    DecorItemOrdersDataGrids[i].Columns["Height"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Height"].MinimumWidth = 90;
                }

                if (_decorCatalogOrder.HasParameter(
                    Convert.ToInt32(_decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Length"))
                {
                    DecorItemOrdersDataGrids[i].Columns["Length"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Length"].MinimumWidth = 90;
                }

                if (_decorCatalogOrder.HasParameter(
                    Convert.ToInt32(_decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Width"))
                {
                    DecorItemOrdersDataGrids[i].Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Width"].MinimumWidth = 90;
                }

                DecorItemOrdersDataGrids[i].Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Count"].MinimumWidth = 90;

                DecorItemOrdersDataGrids[i].AutoGenerateColumns = false;
                var displayIndex = 0;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Length"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Height"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Width"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Count"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Price"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Cost"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Notes"].DisplayIndex = displayIndex++;
            }
        }

        public bool HasRows()
        {
            var itemsCount = 0;

            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
                for (var r = 0; r < DecorItemOrdersDataTables[i].Rows.Count; r++)
                    if (DecorItemOrdersDataTables[i].Rows[r].RowState != DataRowState.Deleted)
                        itemsCount += DecorItemOrdersDataTables[i].Rows.Count;

            return itemsCount > 0;
        }

        private bool ShowTabs()
        {
            var isOrder = 0;

            for (var i = 0; i < _decorTabControl.TabPages.Count; i++)
                if (DecorItemOrdersDataTables[i].Rows.Count > 0)
                {
                    isOrder++;
                    _decorTabControl.TabPages[i].PageVisible = true;
                }
                else
                {
                    _decorTabControl.TabPages[i].PageVisible = false;
                }

            if (isOrder > 0)
                return true;
            return false;
        }


        public bool Filter(int mainOrderId)
        {
            if (_currentMainOrderId == mainOrderId)
                return DecorOrdersDataTable.Rows.Count > 0;

            _currentMainOrderId = mainOrderId;

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i].Clear();
                DecorItemOrdersDataTables[i].AcceptChanges();
                _decorTabControl.TabPages[i].PageVisible = false;
            }

            DecorOrdersCommandBuilder.Dispose();
            DecorOrdersDataAdapter.Dispose();

            DecorOrdersDataAdapter = new SqlDataAdapter(
                "SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
            {
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                var rows = DecorOrdersDataTable.Select("ProductID = " +
                                                       _decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]);

                if (rows.Count() == 0)
                    continue;
                var showColor = false;
                var showPatina = false;
                var showIi = false;
                var showIc = false;
                var showLength = false;
                var showHeight = false;
                var showWidth = false;
                for (var r = 0; r < rows.Count(); r++)
                {
                    if (!showColor)
                        if (Convert.ToInt32(rows[r]["ColorID"]) != -1)
                            showColor = true;
                    if (!showPatina)
                        if (Convert.ToInt32(rows[r]["PatinaID"]) != -1)
                            showPatina = true;
                    if (!showIi)
                        if (Convert.ToInt32(rows[r]["InsetTypeID"]) != -1)
                            showIi = true;
                    if (!showIc)
                        if (Convert.ToInt32(rows[r]["InsetColorID"]) != -1)
                            showIc = true;
                    if (!showLength)
                        if (Convert.ToInt32(rows[r]["Length"]) != -1)
                            showLength = true;
                    if (!showHeight)
                        if (Convert.ToInt32(rows[r]["Height"]) != -1)
                            showHeight = true;
                    if (!showWidth)
                        if (Convert.ToInt32(rows[r]["Width"]) != -1)
                            showWidth = true;
                    DecorItemOrdersDataTables[i].ImportRow(rows[r]);
                }

                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Length"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Height"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Width"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["IsSample"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["LeftAngle"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["RightAngle"].Visible = false;
                if (showColor)
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = true;
                if (showPatina)
                    DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = true;
                if (showIi)
                    DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = true;
                if (showIc)
                    DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = true;
                if (showLength)
                    DecorItemOrdersDataGrids[i].Columns["Length"].Visible = true;
                if (showHeight)
                    DecorItemOrdersDataGrids[i].Columns["Height"].Visible = true;
                if (showWidth)
                    DecorItemOrdersDataGrids[i].Columns["Width"].Visible = true;
            }

            ShowTabs();

            return true;
        }

        public bool Filter(int mainOrderId, int factoryId)
        {
            if (_currentMainOrderId == mainOrderId)
                return DecorOrdersDataTable.Rows.Count > 0;

            _currentMainOrderId = mainOrderId;

            var factoryFilter = "";

            if (factoryId != 0)
                factoryFilter = " AND FactoryID = " + factoryId;

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i].Clear();
                DecorItemOrdersDataTables[i].AcceptChanges();
                _decorTabControl.TabPages[i].PageVisible = false;
            }

            DecorOrdersCommandBuilder.Dispose();
            DecorOrdersDataAdapter.Dispose();

            DecorOrdersDataAdapter = new SqlDataAdapter(
                "SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + mainOrderId + factoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (var i = 0; i < _decorCatalogOrder.DecorProductsCount; i++)
            {
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                var rows = DecorOrdersDataTable.Select("ProductID = " +
                                                       _decorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]);

                if (rows.Count() == 0)
                    continue;
                var showColor = false;
                var showPatina = false;
                var showLength = false;
                var showHeight = false;
                var showWidth = false;
                for (var r = 0; r < rows.Count(); r++)
                {
                    if (!showColor)
                        if (Convert.ToInt32(rows[r]["ColorID"]) != -1)
                            showColor = true;
                    if (!showPatina)
                        if (Convert.ToInt32(rows[r]["PatinaID"]) != -1)
                            showPatina = true;
                    if (!showLength)
                        if (Convert.ToInt32(rows[r]["Length"]) != -1)
                            showLength = true;
                    if (!showHeight)
                        if (Convert.ToInt32(rows[r]["Height"]) != -1)
                            showHeight = true;
                    if (!showWidth)
                        if (Convert.ToInt32(rows[r]["Width"]) != -1)
                            showWidth = true;
                    DecorItemOrdersDataTables[i].ImportRow(rows[r]);
                }

                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Length"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Height"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Width"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["IsSample"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["LeftAngle"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["RightAngle"].Visible = false;
                if (showColor)
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = true;
                if (showPatina)
                    DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = true;
                if (showLength)
                    DecorItemOrdersDataGrids[i].Columns["Length"].Visible = true;
                if (showHeight)
                    DecorItemOrdersDataGrids[i].Columns["Height"].Visible = true;
                if (showWidth)
                    DecorItemOrdersDataGrids[i].Columns["Width"].Visible = true;
            }

            ShowTabs();

            return true;
        }
    }


    public class MFrontsOrders
    {
        private int _currentMainOrderId = -1;
        private BindingSource _frameColorsBindingSource;
        private DataGridViewComboBoxColumn _frameColorsColumn;
        private DataTable _frameColorsDataTable;
        private BindingSource _frontsBindingSource;

        private DataGridViewComboBoxColumn _frontsColumn;
        private DataTable _frontsDataTable;
        private readonly PercentageDataGrid _frontsOrdersDataGrid;
        private BindingSource _insetColorsBindingSource;
        private DataGridViewComboBoxColumn _insetColorsColumn;
        private DataTable _insetColorsDataTable;
        private BindingSource _insetTypesBindingSource;
        private DataGridViewComboBoxColumn _insetTypesColumn;
        private DataTable _insetTypesDataTable;
        private BindingSource _patinaBindingSource;
        private DataGridViewComboBoxColumn _patinaColumn;
        private DataTable _patinaDataTable;
        private DataTable _patinaRalDataTable;
        private BindingSource _technoFrameColorsBindingSource;
        private DataGridViewComboBoxColumn _technoFrameColorsColumn;
        private BindingSource _technoInsetColorsBindingSource;
        private DataGridViewComboBoxColumn _technoInsetColorsColumn;
        private DataTable _technoInsetColorsDataTable;
        private BindingSource _technoInsetTypesBindingSource;
        private DataGridViewComboBoxColumn _technoInsetTypesColumn;
        private DataTable _technoInsetTypesDataTable;
        private DataGridViewComboBoxColumn _technoProfilesColumn;
        private DataTable _technoProfilesDataTable;

        public BindingSource FrontsOrdersBindingSource;

        public DataTable FrontsOrdersDataTable;

        public MFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            _frontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }

        private void Create()
        {
            FrontsOrdersDataTable = new DataTable();

            _frontsDataTable = new DataTable();
            _frameColorsDataTable = new DataTable();
            _patinaDataTable = new DataTable();
            _insetTypesDataTable = new DataTable();
            _insetColorsDataTable = new DataTable();
            _technoInsetTypesDataTable = new DataTable();
            _technoInsetColorsDataTable = new DataTable();

            FrontsOrdersBindingSource = new BindingSource();
            _frontsBindingSource = new BindingSource();
            _frameColorsBindingSource = new BindingSource();
            _patinaBindingSource = new BindingSource();
            _insetTypesBindingSource = new BindingSource();
            _insetColorsBindingSource = new BindingSource();
            _technoFrameColorsBindingSource = new BindingSource();
            _technoInsetTypesBindingSource = new BindingSource();
            _technoInsetColorsBindingSource = new BindingSource();
        }

        private void GetColorsDt()
        {
            _frameColorsDataTable = new DataTable();
            _frameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            _frameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            var selectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (var dt = new DataTable())
                {
                    da.Fill(dt);
                    {
                        var newRow = _frameColorsDataTable.NewRow();
                        newRow["ColorID"] = -1;
                        newRow["ColorName"] = "-";
                        _frameColorsDataTable.Rows.Add(newRow);
                    }
                    {
                        var newRow = _frameColorsDataTable.NewRow();
                        newRow["ColorID"] = 0;
                        newRow["ColorName"] = "на выбор";
                        _frameColorsDataTable.Rows.Add(newRow);
                    }
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var newRow = _frameColorsDataTable.NewRow();
                        newRow["ColorID"] = Convert.ToInt64(dt.Rows[i]["TechStoreID"]);
                        newRow["ColorName"] = dt.Rows[i]["TechStoreName"].ToString();
                        _frameColorsDataTable.Rows.Add(newRow);
                    }
                }
            }
        }

        private void GetInsetColorsDt()
        {
            _insetColorsDataTable = new DataTable();
            using (var da = new SqlDataAdapter(
                "SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_insetColorsDataTable);
                {
                    var newRow = _insetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = -1;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "-";
                    _insetColorsDataTable.Rows.Add(newRow);
                }
                {
                    var newRow = _insetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = 0;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "на выбор";
                    _insetColorsDataTable.Rows.Add(newRow);
                }
            }
        }

        private void Fill()
        {
            var selectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_frontsDataTable);
            }

            selectCommand =
                @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                _technoProfilesDataTable = new DataTable();
                da.Fill(_technoProfilesDataTable);

                var newRow = _technoProfilesDataTable.NewRow();
                newRow["TechnoProfileID"] = -1;
                newRow["TechnoProfileName"] = "-";
                _technoProfilesDataTable.Rows.InsertAt(newRow, 0);
            }

            selectCommand =
                @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                _technoProfilesDataTable = new DataTable();
                da.Fill(_technoProfilesDataTable);

                var newRow = _technoProfilesDataTable.NewRow();
                newRow["TechnoProfileID"] = -1;
                newRow["TechnoProfileName"] = "-";
                _technoProfilesDataTable.Rows.InsertAt(newRow, 0);
            }

            GetColorsDt();
            GetInsetColorsDt();
            using (var da = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_patinaDataTable);
            }

            _patinaRalDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_patinaRalDataTable);
            }

            foreach (DataRow item in _patinaRalDataTable.Rows)
            {
                var newRow = _patinaDataTable.NewRow();
                newRow["PatinaID"] = item["PatinaRALID"];
                newRow["PatinaName"] = item["PatinaRAL"];
                newRow["DisplayName"] = item["DisplayName"];
                _patinaDataTable.Rows.Add(newRow);
            }

            using (var da = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_insetTypesDataTable);
            }

            _technoInsetTypesDataTable = _insetTypesDataTable.Copy();
            _technoInsetColorsDataTable = _insetColorsDataTable.Copy();

            using (var da = new SqlDataAdapter("SELECT TOP 0 * FROM SampleFrontsOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                da.Fill(FrontsOrdersDataTable);
            }
        }

        private void Binding()
        {
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            _frontsBindingSource.DataSource = _frontsDataTable;
            _frameColorsBindingSource.DataSource = new DataView(_frameColorsDataTable);
            _patinaBindingSource.DataSource = _patinaDataTable;
            _insetTypesBindingSource.DataSource = _insetTypesDataTable;
            _insetColorsBindingSource.DataSource = _insetColorsDataTable;
            _technoFrameColorsBindingSource.DataSource = new DataView(_frameColorsDataTable);
            _technoInsetTypesBindingSource.DataSource = _technoInsetTypesDataTable;
            _technoInsetColorsBindingSource.DataSource = _technoInsetColorsDataTable;

            _frontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
        }

        private void CreateColumns(bool showPrice)
        {
            if (_frontsColumn != null)
                return;

            //создание столбцов
            _frontsColumn = new DataGridViewComboBoxColumn
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = _frontsBindingSource,
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _frameColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = _frameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _patinaColumn = new DataGridViewComboBoxColumn
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = _patinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _insetTypesColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = _insetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _insetColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = _insetColorsBindingSource,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoProfilesColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoProfilesColumn",
                HeaderText = "Тип\r\nпрофиля-2",
                DataPropertyName = "TechnoProfileID",
                DataSource = new DataView(_technoProfilesDataTable),
                ValueMember = "TechnoProfileID",
                DisplayMember = "TechnoProfileName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoFrameColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoFrameColorsColumn",
                HeaderText = "Цвет профиля-2",
                DataPropertyName = "TechnoColorID",
                DataSource = _technoFrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoInsetTypesColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = _technoInsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _technoInsetColorsColumn = new DataGridViewComboBoxColumn
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = _technoInsetColorsBindingSource,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            _frontsOrdersDataGrid.AutoGenerateColumns = false;

            //добавление столбцов
            _frontsOrdersDataGrid.Columns.Add(_frontsColumn);
            _frontsOrdersDataGrid.Columns.Add(_frameColorsColumn);
            _frontsOrdersDataGrid.Columns.Add(_patinaColumn);
            _frontsOrdersDataGrid.Columns.Add(_insetTypesColumn);
            _frontsOrdersDataGrid.Columns.Add(_insetColorsColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoProfilesColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoFrameColorsColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoInsetTypesColumn);
            _frontsOrdersDataGrid.Columns.Add(_technoInsetColorsColumn);

            //убирание лишних столбцов
            if (_frontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                _frontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("NeedCalcPrice"))
                _frontsOrdersDataGrid.Columns["NeedCalcPrice"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("AreaID"))
                _frontsOrdersDataGrid.Columns["AreaID"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                _frontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                _frontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                _frontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                _frontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }

            if (_frontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                _frontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                _frontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            _frontsOrdersDataGrid.Columns["CupboardString"].Visible = false;
            _frontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            _frontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            _frontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            _frontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            _frontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            _frontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            _frontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            _frontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            _frontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            _frontsOrdersDataGrid.Columns["Weight"].Visible = false;
            _frontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;
            _frontsOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            _frontsOrdersDataGrid.Columns["CurrencyCost"].Visible = false;
            if (_frontsOrdersDataGrid.Columns.Contains("OriginalInsetPrice"))
                _frontsOrdersDataGrid.Columns["OriginalInsetPrice"].Visible = false;

            if (!Security.PriceAccess || !showPrice)
            {
                _frontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                _frontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                _frontsOrdersDataGrid.Columns["TotalDiscount"].Visible = false;
                _frontsOrdersDataGrid.Columns["Cost"].Visible = false;
                _frontsOrdersDataGrid.Columns["OriginalPrice"].Visible = false;
                _frontsOrdersDataGrid.Columns["OriginalCost"].Visible = false;
                _frontsOrdersDataGrid.Columns["CostWithTransport"].Visible = false;
                _frontsOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            }

            var displayIndex = 0;
            _frontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = displayIndex++;
            _frontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = displayIndex++;

            _frontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            foreach (DataGridViewColumn column in _frontsOrdersDataGrid.Columns)
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //названия столбцов
            _frontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            _frontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            _frontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            _frontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            _frontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            _frontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            _frontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\nфасад";
            _frontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            _frontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            _frontsOrdersDataGrid.Columns["OriginalPrice"].HeaderText = "Цена\r\n(оригинал)";
            _frontsOrdersDataGrid.Columns["OriginalCost"].HeaderText = "Стоимость\r\n(оригинал)";
            _frontsOrdersDataGrid.Columns["CostWithTransport"].HeaderText = "Стоимость\r\n(с транспортом)";
            _frontsOrdersDataGrid.Columns["PriceWithTransport"].HeaderText = "Цена\r\n(с транспортом)";
            _frontsOrdersDataGrid.Columns["CurrencyCost"].HeaderText = "Стоимость\r\nв расчете";
            _frontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";
            _frontsOrdersDataGrid.Columns["TotalDiscount"].HeaderText = "Общая\r\nскидка, %";

            _frontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["CurrencyCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["OriginalPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["OriginalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["CostWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["PriceWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _frontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
            _frontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _frontsOrdersDataGrid.Columns["TotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrontsOrdersDataGrid.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //FrontsOrdersDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            _frontsOrdersDataGrid.CellFormatting += FrontsOrdersDataGrid_CellFormatting;
        }

        private void FrontsOrdersDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = (PercentageDataGrid)sender;
            if (grid.Columns.Contains("PatinaColumn") && e.ColumnIndex == grid.Columns["PatinaColumn"].Index
                                                      && e.Value != null)
            {
                var cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                var patinaId = -1;
                var displayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["PatinaID"].Value != DBNull.Value)
                {
                    patinaId = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PatinaID"].Value);
                    displayName = PatinaDisplayName(patinaId);
                }

                cell.ToolTipText = displayName;
            }
        }

        public string GetFrontName(int id)
        {
            var rows = _frontsDataTable.Select("FrontID = " + id);
            if (rows.Any())
                return rows[0]["FrontName"].ToString();
            return string.Empty;
        }

        public string GetColorName(int id)
        {
            var rows = _frameColorsDataTable.Select("ColorID = " + id);
            if (rows.Any())
                return rows[0]["ColorName"].ToString();
            return string.Empty;
        }

        public string PatinaDisplayName(int patinaId)
        {
            var rows = _patinaDataTable.Select("PatinaID = " + patinaId);
            if (rows.Count() > 0)
                return rows[0]["DisplayName"].ToString();
            return string.Empty;
        }

        public void Initialize(bool showPrice)
        {
            Create();
            Fill();
            Binding();
            CreateColumns(showPrice);
        }

        public bool Filter(int mainOrderId, int factoryId)
        {
            if (_currentMainOrderId == mainOrderId)
                return FrontsOrdersDataTable.Rows.Count > 0;

            _currentMainOrderId = mainOrderId;

            var factoryFilter = "";

            if (factoryId != 0)
                factoryFilter = " AND FactoryID = " + factoryId;

            FrontsOrdersDataTable.Clear();

            using (var da = new SqlDataAdapter(
                "SELECT * FROM SampleFrontsOrders WHERE MainOrderID = " + mainOrderId + factoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                da.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }
    }

    public class MDecorOrders
    {
        private int _currentMainOrderId = -1;

        private readonly XtraTabControl _decorTabControl;
        private int _selectedGridIndex = -1;

        public DecorCatalogOrder DecorCatalogOrder;

        public BindingSource[] DecorItemOrdersBindingSources;

        public PercentageDataGrid[] DecorItemOrdersDataGrids;
        public DataTable[] DecorItemOrdersDataTables;
        public SqlCommandBuilder DecorOrdersCommandBuilder;

        public SqlDataAdapter DecorOrdersDataAdapter;

        public DataTable DecorOrdersDataTable;

        public PercentageDataGrid MainOrdersFrontsOrdersDataGrid;

        public MDecorOrders(ref XtraTabControl tDecorTabControl,
            ref DecorCatalogOrder tDecorCatalogOrder,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            _decorTabControl = tDecorTabControl;
            DecorCatalogOrder = tDecorCatalogOrder;

            MainOrdersFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }

        public int ClientId { get; set; } = -1;

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

            for (var i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = new DataTable();
                DecorItemOrdersBindingSources[i] = new BindingSource();
            }
        }

        private void Fill()
        {
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM SampleDecorOrders",
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
        }

        private void Binding()
        {
        }

        public void Initialize(bool showPrice)
        {
            Create();
            Fill();
            Binding();

            SplitDecorOrdersTables();
            GridSettings(showPrice);
        }

        private DataGridViewComboBoxColumn CreateInsetTypesColumn()
        {
            var itemColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",

                DataSource = new DataView(DecorCatalogOrder.InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return itemColumn;
        }

        private DataGridViewComboBoxColumn CreateInsetColorsColumn()
        {
            var itemColumn = new DataGridViewComboBoxColumn
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",

                DataSource = new DataView(DecorCatalogOrder.InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return itemColumn;
        }

        private DataGridViewComboBoxColumn CreateColorColumn()
        {
            var colorsColumn = new DataGridViewComboBoxColumn
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
            return colorsColumn;
        }

        private DataGridViewComboBoxColumn CreatePatinaColumn()
        {
            var patinaColumn = new DataGridViewComboBoxColumn
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",

                DataSource = DecorCatalogOrder.PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return patinaColumn;
        }

        private DataGridViewComboBoxColumn CreateItemColumn()
        {
            var itemColumn = new DataGridViewComboBoxColumn
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
            return itemColumn;
        }

        private void SplitDecorOrdersTables()
        {
            for (var i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = DecorOrdersDataTable.Clone();
                DecorItemOrdersBindingSources[i].DataSource = DecorItemOrdersDataTables[i];
            }
        }

        private void GridSettings(bool showPrice)
        {
            _decorTabControl.AppearancePage.Header.BackColor = Color.FromArgb(140, 140, 140);
            _decorTabControl.AppearancePage.Header.BackColor2 = Color.FromArgb(140, 140, 140);
            _decorTabControl.AppearancePage.Header.BorderColor = Color.Black;
            _decorTabControl.AppearancePage.Header.Font = new Font("Segoe UI", 12F, FontStyle.Regular,
                GraphicsUnit.Point,
                204);
            _decorTabControl.AppearancePage.Header.Options.UseBackColor = true;
            _decorTabControl.AppearancePage.Header.Options.UseBorderColor = true;
            _decorTabControl.AppearancePage.Header.Options.UseFont = true;
            _decorTabControl.LookAndFeel.SkinName = "Office 2010 Black";
            _decorTabControl.LookAndFeel.UseDefaultLookAndFeel = false;

            for (var i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                _decorTabControl.TabPages.Add(
                    DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString());
                _decorTabControl.TabPages[i].PageVisible = false;
                _decorTabControl.TabPages[i].Text =
                    DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString();

                DecorItemOrdersDataGrids[i] = new PercentageDataGrid
                {
                    Parent = _decorTabControl.TabPages[i],
                    DataSource = DecorItemOrdersBindingSources[i],
                    Dock = DockStyle.Fill,
                    PaletteMode = PaletteMode.Office2010Black
                };
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = Color.White;
                DecorItemOrdersDataGrids[i].AllowUserToAddRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToDeleteRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToResizeRows = false;
                DecorItemOrdersDataGrids[i].RowHeadersVisible = false;
                DecorItemOrdersDataGrids[i].AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.Background.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.Background.ColorStyle =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.Background.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.ColorStyle =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Border.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Font =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Content.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.ColorStyle =
                    MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Border.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].RowTemplate.Height = MainOrdersFrontsOrdersDataGrid.RowTemplate.Height;
                DecorItemOrdersDataGrids[i].ColumnHeadersHeight = MainOrdersFrontsOrdersDataGrid.ColumnHeadersHeight;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.ColorStyle = PaletteColorStyle.Solid;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Border.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Font =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLine =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLine;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLineH =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLineH;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.TextH =
                    MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.TextH;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 =
                    MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DecorItemOrdersDataGrids[i].SelectedColorStyle = PercentageDataGrid.ColorStyle.Green;
                DecorItemOrdersDataGrids[i].ReadOnly = true;
                DecorItemOrdersDataGrids[i].Tag = i;
                DecorItemOrdersDataGrids[i].UseCustomBackColor = true;
                DecorItemOrdersDataGrids[i].StandardStyle = false;
                DecorItemOrdersDataGrids[i].MultiSelect = true;
                DecorItemOrdersDataGrids[i].CellMouseDown += MainOrdersDecorOrders_CellMouseDown;

                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetColorsColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetTypesColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateColorColumn());
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreatePatinaColumn());
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateItemColumn());
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].MinimumWidth = 60;

                DecorItemOrdersDataGrids[i].Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Cost"].MinimumWidth = 90;
                DecorItemOrdersDataGrids[i].Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Price"].MinimumWidth = 90;
                DecorItemOrdersDataGrids[i].Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Notes"].MinimumWidth = 120;

                //убирание лишних столбцов
                if (DecorItemOrdersDataGrids[i].Columns.Contains("NeedCalcPrice"))
                    DecorItemOrdersDataGrids[i].Columns["NeedCalcPrice"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("AreaID"))
                    DecorItemOrdersDataGrids[i].Columns["AreaID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateDateTime"))
                {
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].HeaderText = "Добавлено";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.None;
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].Width = 120;
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

                if (DecorCatalogOrder.HasParameter(
                    Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "ColorID"))
                {
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 190;
                }

                if (DecorCatalogOrder.HasParameter(
                    Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Height"))
                {
                    DecorItemOrdersDataGrids[i].Columns["Height"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Height"].MinimumWidth = 90;
                }

                if (DecorCatalogOrder.HasParameter(
                    Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Length"))
                {
                    DecorItemOrdersDataGrids[i].Columns["Length"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Length"].MinimumWidth = 90;
                }

                if (DecorCatalogOrder.HasParameter(
                    Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]), "Width"))
                {
                    DecorItemOrdersDataGrids[i].Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    DecorItemOrdersDataGrids[i].Columns["Width"].MinimumWidth = 90;
                }

                DecorItemOrdersDataGrids[i].Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Count"].MinimumWidth = 90;
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

                for (var j = 2; j < DecorItemOrdersDataGrids[i].Columns.Count; j++)
                {
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Height")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Высота";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Length")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Длина";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Width")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Ширина";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Count")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Кол-во";
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Notes")
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Примечание";
                }

                foreach (DataGridViewColumn column in DecorItemOrdersDataGrids[i].Columns)
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DecorItemOrdersDataGrids[i].AutoGenerateColumns = false;
                var displayIndex = 0;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Length"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Height"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Width"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Count"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Price"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Cost"].DisplayIndex = displayIndex++;
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].DisplayIndex = displayIndex++;
            }
        }

        private void MainOrdersDecorOrders_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var grid = (PercentageDataGrid)sender;
            _selectedGridIndex = Convert.ToInt32(grid.Tag);
            if (e.Button == MouseButtons.Right)
                DecorItemOrdersBindingSources[_selectedGridIndex].Position = e.RowIndex;
            //kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
        }

        private bool ShowTabs()
        {
            var isOrder = 0;

            for (var i = 0; i < _decorTabControl.TabPages.Count; i++)
                if (DecorItemOrdersDataTables[i].Rows.Count > 0)
                {
                    isOrder++;
                    _decorTabControl.TabPages[i].PageVisible = true;
                }
                else
                {
                    _decorTabControl.TabPages[i].PageVisible = false;
                }

            if (isOrder > 0)
                return true;
            return false;
        }

        public bool Filter(int mainOrderId, int factoryId)
        {
            if (_currentMainOrderId == mainOrderId)
                return DecorOrdersDataTable.Rows.Count > 0;

            _currentMainOrderId = mainOrderId;

            var factoryFilter = "";

            if (factoryId != 0)
                factoryFilter = " AND FactoryID = " + factoryId;

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (var i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i].Clear();
                DecorItemOrdersDataTables[i].AcceptChanges();
                _decorTabControl.TabPages[i].PageVisible = false;
            }

            DecorOrdersCommandBuilder.Dispose();
            DecorOrdersDataAdapter.Dispose();

            DecorOrdersDataAdapter = new SqlDataAdapter(
                "SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + mainOrderId + factoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (var i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                var rows = DecorOrdersDataTable.Select("ProductID = " +
                                                       DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]);

                if (rows.Count() == 0)
                    continue;
                var showColor = false;
                var showPatina = false;
                var showIi = false;
                var showIc = false;
                var showLength = false;
                var showHeight = false;
                var showWidth = false;
                for (var r = 0; r < rows.Count(); r++)
                {
                    if (!showColor)
                        if (Convert.ToInt32(rows[r]["ColorID"]) != -1)
                            showColor = true;
                    if (!showPatina)
                        if (Convert.ToInt32(rows[r]["PatinaID"]) != -1)
                            showPatina = true;
                    if (!showIi)
                        if (Convert.ToInt32(rows[r]["InsetTypeID"]) != -1)
                            showIi = true;
                    if (!showIc)
                        if (Convert.ToInt32(rows[r]["InsetColorID"]) != -1)
                            showIc = true;
                    if (!showLength)
                        if (Convert.ToInt32(rows[r]["Length"]) != -1)
                            showLength = true;
                    if (!showHeight)
                        if (Convert.ToInt32(rows[r]["Height"]) != -1)
                            showHeight = true;
                    if (!showWidth)
                        if (Convert.ToInt32(rows[r]["Width"]) != -1)
                            showWidth = true;
                    DecorItemOrdersDataTables[i].ImportRow(rows[r]);
                }

                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Length"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Height"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Width"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["IsSample"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["LeftAngle"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["RightAngle"].Visible = false;
                if (showColor)
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = true;
                if (showPatina)
                    DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = true;
                if (showIi)
                    DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].Visible = true;
                if (showIc)
                    DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].Visible = true;
                if (showLength)
                    DecorItemOrdersDataGrids[i].Columns["Length"].Visible = true;
                if (showHeight)
                    DecorItemOrdersDataGrids[i].Columns["Height"].Visible = true;
                if (showWidth)
                    DecorItemOrdersDataGrids[i].Columns["Width"].Visible = true;
            }

            ShowTabs();

            return true;
        }
    }

    public class OrdersManager
    {
        private DataTable _mClientsDataTable;
        private DataTable _mClientsGroupsDataTable;
        private DataTable _mOrdersDataTable;
        private DataTable _mShopAddressesDataTable;

        private DataTable _ordersDataTable;
        private DataTable _zClientsDataTable;
        private DataTable _zClientsGroupsDataTable;
        private DataTable _zOrdersDataTable;
        private DataTable _zShopAddressesDataTable;
        public int CurrentClientId = -1;
        public int CurrentMainOrderId = -1;
        public FileManager Fm = new FileManager();
        public BindingSource MainOrdersBindingSource;

        public BindingSource MClientsBindingSource;
        public BindingSource MClientsGroupsBindingSource;
        public MDecorOrders MDecorOrders;

        public MFrontsOrders MFrontsOrders;
        public BindingSource ZClientsBindingSource;
        public BindingSource ZClientsGroupsBindingSource;
        public ZDecorOrders ZDecorOrders;
        public ZFrontsOrders ZFrontsOrders;

        public OrdersManager(
            ref PercentageDataGrid tMFrontsOrdersDataGrid,
            ref PercentageDataGrid tZFrontsOrdersDataGrid,
            ref XtraTabControl tMDecorTabControl,
            ref XtraTabControl tZDecorTabControl,
            ref DecorCatalogOrder decorCatalogOrder)
        {
            MFrontsOrders = new MFrontsOrders(ref tMFrontsOrdersDataGrid);
            MFrontsOrders.Initialize(true);

            MDecorOrders = new MDecorOrders(ref tMDecorTabControl, ref decorCatalogOrder, ref tMFrontsOrdersDataGrid);
            MDecorOrders.Initialize(true);

            ZFrontsOrders = new ZFrontsOrders(ref tZFrontsOrdersDataGrid);
            ZFrontsOrders.Initialize(true);

            ZDecorOrders = new ZDecorOrders(ref tZDecorTabControl, ref decorCatalogOrder, ref tZFrontsOrdersDataGrid);
            ZDecorOrders.Initialize(true);

            Initialize();
        }

        public DataTable OrdersDataTable
        {
            get => _ordersDataTable;
            set => _ordersDataTable = value;
        }


        private void Create()
        {
            _mClientsDataTable = new DataTable();
            _mClientsGroupsDataTable = new DataTable();
            _mOrdersDataTable = new DataTable();
            _zClientsDataTable = new DataTable();
            _zClientsGroupsDataTable = new DataTable();
            _zOrdersDataTable = new DataTable();
            _mShopAddressesDataTable = new DataTable();
            _zShopAddressesDataTable = new DataTable();

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
            var selectCommand = @"SELECT TOP 0 * FROM SampleMainOrders";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                da.Fill(_mOrdersDataTable);
            }

            selectCommand = @"SELECT TOP 0 * FROM SampleMainOrders";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                da.Fill(_zOrdersDataTable);
            }

            selectCommand = @"SELECT ClientID, ClientName, ClientGroupID FROM Clients ORDER BY ClientName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                da.Fill(_mClientsDataTable);
            }

            selectCommand = @"SELECT * FROM ClientGroups";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                da.Fill(_mClientsGroupsDataTable);
            }

            selectCommand = @"SELECT ClientID, ClientName, ClientGroupID FROM Clients ORDER BY ClientName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVReferenceConnectionString))
            {
                da.Fill(_zClientsDataTable);
            }

            selectCommand = @"SELECT * FROM ClientsGroups";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVReferenceConnectionString))
            {
                da.Fill(_zClientsGroupsDataTable);
            }

            selectCommand = @"SELECT * FROM ShopAddresses";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                da.Fill(_mShopAddressesDataTable);
            }

            selectCommand = @"SELECT * FROM ShopAddresses";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVReferenceConnectionString))
            {
                da.Fill(_zShopAddressesDataTable);
            }

            _mClientsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            _mClientsGroupsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            _zClientsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            _zClientsGroupsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            for (var i = 0; i < _mClientsDataTable.Rows.Count; i++)
                _mClientsDataTable.Rows[i]["Check"] = false;
            for (var i = 0; i < _mClientsGroupsDataTable.Rows.Count; i++)
                _mClientsGroupsDataTable.Rows[i]["Check"] = false;
            for (var i = 0; i < _zClientsDataTable.Rows.Count; i++)
                _zClientsDataTable.Rows[i]["Check"] = false;
            for (var i = 0; i < _zClientsGroupsDataTable.Rows.Count; i++)
                _zClientsGroupsDataTable.Rows[i]["Check"] = false;
        }

        private void Binding()
        {
            MClientsBindingSource.DataSource = _mClientsDataTable;
            MClientsGroupsBindingSource.DataSource = _mClientsGroupsDataTable;
            ZClientsBindingSource.DataSource = _zClientsDataTable;
            ZClientsGroupsBindingSource.DataSource = _zClientsGroupsDataTable;
            MainOrdersBindingSource.DataSource = OrdersDataTable;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public void MoveToMainOrder(int mainOrderId)
        {
            MainOrdersBindingSource.Position = MainOrdersBindingSource.Find("MainOrderID", mainOrderId);
        }

        public void FilterMClients(int clientGroupId)
        {
            MClientsBindingSource.Filter = "ClientGroupID = " + clientGroupId;
            MClientsBindingSource.MoveFirst();
        }

        public void FilterZClients(int clientGroupId)
        {
            ZClientsBindingSource.Filter = "ClientGroupID = " + clientGroupId;
            ZClientsBindingSource.MoveFirst();
        }

        public ArrayList GetMClients()
        {
            var clients = new ArrayList();
            for (var i = 0; i < _mClientsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(_mClientsDataTable.Rows[i]["Check"]))
                    continue;

                clients.Add(Convert.ToInt32(_mClientsDataTable.Rows[i]["ClientID"]));
            }

            return clients;
        }

        public ArrayList GetMClientGroups()
        {
            var clientGroupIDs = new ArrayList();
            for (var i = 0; i < _mClientsGroupsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(_mClientsGroupsDataTable.Rows[i]["Check"])
                    || Convert.ToInt32(_mClientsGroupsDataTable.Rows[i]["ClientGroupID"]) == 1)
                    continue;

                clientGroupIDs.Add(Convert.ToInt32(_mClientsGroupsDataTable.Rows[i]["ClientGroupID"]));
            }

            return clientGroupIDs;
        }

        public ArrayList GetZClients()
        {
            var clients = new ArrayList();
            for (var i = 0; i < _zClientsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(_zClientsDataTable.Rows[i]["Check"]))
                    continue;

                clients.Add(Convert.ToInt32(_zClientsDataTable.Rows[i]["ClientID"]));
            }

            return clients;
        }

        public ArrayList GetZClientGroups()
        {
            var clientGroupIDs = new ArrayList();
            for (var i = 0; i < _zClientsGroupsDataTable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(_zClientsGroupsDataTable.Rows[i]["Check"])
                    || Convert.ToInt32(_zClientsGroupsDataTable.Rows[i]["ClientGroupID"]) == 1)
                    continue;

                clientGroupIDs.Add(Convert.ToInt32(_zClientsGroupsDataTable.Rows[i]["ClientGroupID"]));
            }

            return clientGroupIDs;
        }

        public void CheckMClients(bool check)
        {
            for (var i = 0; i < _mClientsDataTable.Rows.Count; i++) _mClientsDataTable.Rows[i]["Check"] = check;
        }

        public void CheckMClients(bool check, int clientGroupId)
        {
            var groupFilter = string.Empty;
            groupFilter = "ClientGroupID = " + clientGroupId;
            var rows = _mClientsDataTable.Select(groupFilter);
            foreach (var row in rows)
                row["Check"] = check;
        }

        public void CheckMClientGroups(bool check)
        {
            for (var i = 0; i < _mClientsGroupsDataTable.Rows.Count; i++)
                _mClientsGroupsDataTable.Rows[i]["Check"] = check;
        }

        public void CheckMClientGroups(bool check, int clientGroupId)
        {
            var groupFilter = string.Empty;
            groupFilter = "ClientGroupID = " + clientGroupId;
            var rows = _mClientsGroupsDataTable.Select(groupFilter);
            foreach (var row in rows)
                row["Check"] = check;
        }

        public void CheckZClients(bool check)
        {
            for (var i = 0; i < _zClientsDataTable.Rows.Count; i++) _zClientsDataTable.Rows[i]["Check"] = check;
        }

        public void CheckZClients(bool check, int clientGroupId)
        {
            var groupFilter = string.Empty;
            groupFilter = "ClientGroupID = " + clientGroupId;
            var rows = _zClientsDataTable.Select(groupFilter);
            foreach (var row in rows)
                row["Check"] = check;
        }

        public void CheckZClientGroups(bool check, int clientGroupId)
        {
            var groupFilter = string.Empty;
            groupFilter = "ClientGroupID = " + clientGroupId;
            var rows = _zClientsGroupsDataTable.Select(groupFilter);
            foreach (var row in rows)
                row["Check"] = check;
        }

        public void CheckZClientGroups(bool check)
        {
            for (var i = 0; i < _zClientsGroupsDataTable.Rows.Count; i++)
                _zClientsGroupsDataTable.Rows[i]["Check"] = check;
        }

        public DataTable GetFrontsDataTable(
            bool bZov,
            bool bZClients,
            bool bZCreateDate,
            object zCreateDateFrom,
            object zCreateDateTo,
            bool bZDispDate,
            object zDispDateFrom,
            object zDispDateTo)
        {
            DataTable shopAddressOrdersDataTable = new DataTable();
            string selectCommand = @"SELECT ShopAddressOrders.*, ShopAddresses.Address FROM ShopAddressOrders INNER JOIN ShopAddresses ON ShopAddressOrders.ShopAddressID=ShopAddresses.ShopAddressID";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVReferenceConnectionString))
            {
                da.Fill(shopAddressOrdersDataTable);
            }

            string zFilter = GetFilterString(bZov, bZClients, bZCreateDate, zCreateDateFrom, zCreateDateTo, bZDispDate, zDispDateFrom, zDispDateTo);
            using (DataTable dtTable = new DataTable())
            {
                using (var da = new SqlDataAdapter(
                    @"SELECT FrontID, ColorID, SampleFrontsOrders.MainOrderID, Clients.ClientName, infiniu2_zovorders.dbo.MainOrders.DocNumber, DispDate, Description, SUM(SampleFrontsOrders.Cost), SUM(SampleFrontsOrders.Square) FROM SampleFrontsOrders 
INNER JOIN SampleMainOrders ON SampleFrontsOrders.MainOrderID = SampleMainOrders.MainOrderID
INNER JOIN JoinMainOrders ON SampleMainOrders.MainOrderID = JoinMainOrders.MarketMainOrderID
                INNER JOIN infiniu2_zovorders.dbo.MainOrders ON JoinMainOrders.ZOVMainOrderID=infiniu2_zovorders.dbo.MainOrders.MainOrderID
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON JoinMainOrders.ZOVClientID=Clients.ClientID" + zFilter +
                    " GROUP BY FrontID, ColorID, SampleFrontsOrders.MainOrderID, Clients.ClientName, infiniu2_zovorders.dbo.MainOrders.DocNumber, DispDate, Description",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (da.Fill(dtTable) > 0)
                    {
                        dtTable.Columns.Add(new DataColumn("FrontName", Type.GetType("System.String")));
                        dtTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
                        dtTable.Columns.Add(new DataColumn("ShopAddresses", Type.GetType("System.String")));
                        for (int i = 0; i < dtTable.Rows.Count; i++)
                        {
                            dtTable.Rows[i]["FrontName"] =
                                MFrontsOrders.GetFrontName(Convert.ToInt32(dtTable.Rows[i]["FrontID"]));
                            dtTable.Rows[i]["ColorName"] =
                                MFrontsOrders.GetColorName(Convert.ToInt32(dtTable.Rows[i]["ColorID"]));

                            int mainOrderId = Convert.ToInt32(dtTable.Rows[i]["MainOrderID"]);
                            var rows = shopAddressOrdersDataTable.Select("MainOrderID=" + mainOrderId);
                            if (rows.Any())
                            {
                                StringBuilder ShopAddresses = new StringBuilder();
                                DataRow last = rows.Last();
                                foreach (var t in rows)
                                {
                                    ShopAddresses.Append(t["Address"].ToString());
                                    if (!t.Equals(last))
                                        ShopAddresses.AppendLine();
                                }

                                dtTable.Rows[i]["ShopAddresses"] = ShopAddresses.ToString();
                            }
                            else
                                dtTable.Rows[i]["ShopAddresses"] = "";
                        }
                        dtTable.Columns.Remove("MainOrderID");
                        dtTable.Columns.Remove("FrontID");
                        dtTable.Columns.Remove("ColorID");
                        dtTable.Columns["FrontName"].SetOrdinal(4);
                        dtTable.Columns["ColorName"].SetOrdinal(5);

                    }
                }
                DataView dv = dtTable.DefaultView;
                if (dtTable.Rows.Count > 0)
                    dv.Sort = "ClientName, DocNumber, DispDate, FrontName, ColorName";
                return dv.ToTable();
            }
        }

        public string GetFilterString(
            bool bZov,
            bool bZClients,
            bool bZCreateDate,
            object zCreateDateFrom,
            object zCreateDateTo,
            bool bZDispDate,
            object zDispDateFrom,
            object zDispDateTo)
        {
            var zFilter = string.Empty;

            if (bZov)
            {
                if (bZClients)
                {
                    var zClients = GetZClients();
                    if (zClients.Count > 0)
                    {
                        if (zFilter.Length > 0)
                            zFilter += " AND infiniu2_zovorders.dbo.MainOrders.ClientID IN (" +
                                       string.Join(",", zClients.OfType<int>().ToArray()) + ")";
                        else
                            zFilter = " WHERE infiniu2_zovorders.dbo.MainOrders.ClientID IN (" +
                                      string.Join(",", zClients.OfType<int>().ToArray()) + ")";
                    }
                    else
                    {
                        if (zFilter.Length > 0)
                            zFilter += " AND infiniu2_zovorders.dbo.MainOrders.ClientID = -1";
                        else
                            zFilter = " WHERE infiniu2_zovorders.dbo.MainOrders.ClientID = -1";
                    }
                }

                if (bZCreateDate)
                {
                    if (zFilter.Length > 0)
                        zFilter += " AND CAST(DocDateTime AS DATE) >= '" +
                                   Convert.ToDateTime(zCreateDateFrom).ToString("yyyy-MM-dd") +
                                   "' AND CAST(DocDateTime AS DATE) <= '" +
                                   Convert.ToDateTime(zCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                    else
                        zFilter = " WHERE CAST(DocDateTime AS DATE) >= '" +
                                  Convert.ToDateTime(zCreateDateFrom).ToString("yyyy-MM-dd") +
                                  "' AND CAST(DocDateTime AS DATE) <= '" +
                                  Convert.ToDateTime(zCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                }

                if (bZDispDate)
                {
                    if (zFilter.Length > 0)
                        zFilter += " AND CAST(DispDate AS DATE) >= '" +
                                   Convert.ToDateTime(zDispDateFrom).ToString("yyyy-MM-dd") +
                                   "' AND CAST(DispDate AS DATE) <= '" +
                                   Convert.ToDateTime(zDispDateTo).ToString("yyyy-MM-dd") + "' ";
                    else
                        zFilter = " WHERE CAST(DispDate AS DATE) >= '" +
                                  Convert.ToDateTime(zDispDateFrom).ToString("yyyy-MM-dd") +
                                  "' AND CAST(DispDate AS DATE) <= '" +
                                  Convert.ToDateTime(zDispDateTo).ToString("yyyy-MM-dd") + "' ";
                }

                if (!bZClients && !bZCreateDate && !bZDispDate)
                    zFilter = " WHERE SampleMainOrders.MainOrderID = -1";
            }
            else
            {
                zFilter = " WHERE SampleMainOrders.MainOrderID = -1";
            }

            string selectCommand =
                @"SELECT infiniu2_zovorders.dbo.MainOrders.MainOrderID FROM SampleMainOrders
                INNER JOIN JoinMainOrders ON SampleMainOrders.MainOrderID = JoinMainOrders.MarketMainOrderID
                INNER JOIN infiniu2_zovorders.dbo.MainOrders ON JoinMainOrders.ZOVMainOrderID=infiniu2_zovorders.dbo.MainOrders.MainOrderID
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON JoinMainOrders.ZOVClientID=Clients.ClientID" + zFilter;
            return zFilter;
        }

        public void FilterOrders(
            bool bMarketing,
            bool bMClients,
            bool bMCreateDate,
            object mCreateDateFrom,
            object mCreateDateTo,
            bool bMDispDate,
            object mDispDateFrom,
            object mDispDateTo,
            bool bZov,
            bool bZClients,
            bool bZCreateDate,
            object zCreateDateFrom,
            object zCreateDateTo,
            bool bZDispDate,
            object zDispDateFrom,
            object zDispDateTo)
        {
            var mFilter = string.Empty;
            var zFilter = string.Empty;
            if (bMarketing)
            {
                if (bMClients)
                {
                    var mClients = GetMClients();
                    if (mClients.Count > 0)
                    {
                        if (mFilter.Length > 0)
                            mFilter += " AND MegaOrders.ClientID IN (" +
                                       string.Join(",", mClients.OfType<int>().ToArray()) + ")";
                        else
                            mFilter = " WHERE MegaOrders.ClientID IN (" +
                                      string.Join(",", mClients.OfType<int>().ToArray()) + ")";
                    }
                    else
                    {
                        if (mFilter.Length > 0)
                            mFilter += " AND MegaOrders.ClientID = -1";
                        else
                            mFilter = " WHERE MegaOrders.ClientID = -1";
                    }
                }

                if (bMCreateDate)
                {
                    if (mFilter.Length > 0)
                        mFilter += " AND CAST(DocDateTime AS DATE) >= '" +
                                   Convert.ToDateTime(mCreateDateFrom).ToString("yyyy-MM-dd") +
                                   "' AND CAST(DocDateTime AS DATE) <= '" +
                                   Convert.ToDateTime(mCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                    else
                        mFilter = " WHERE CAST(DocDateTime AS DATE) >= '" +
                                  Convert.ToDateTime(mCreateDateFrom).ToString("yyyy-MM-dd") +
                                  "' AND CAST(DocDateTime AS DATE) <= '" +
                                  Convert.ToDateTime(mCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                }

                if (bMDispDate)
                {
                    if (mFilter.Length > 0)
                        mFilter += " AND CAST(DispDate AS DATE) >= '" +
                                   Convert.ToDateTime(mDispDateFrom).ToString("yyyy-MM-dd") +
                                   "' AND CAST(DispDate AS DATE) <= '" +
                                   Convert.ToDateTime(mDispDateTo).ToString("yyyy-MM-dd") + "' ";
                    else
                        mFilter = " WHERE CAST(DispDate AS DATE) >= '" +
                                  Convert.ToDateTime(mDispDateFrom).ToString("yyyy-MM-dd") +
                                  "' AND CAST(DispDate AS DATE) <= '" +
                                  Convert.ToDateTime(mDispDateTo).ToString("yyyy-MM-dd") + "' ";
                }

                if (!bMClients && !bMCreateDate && !bMDispDate)
                    mFilter = " WHERE MainOrderID = -1";
            }
            else
            {
                mFilter = " WHERE MainOrderID = -1";
            }

            if (bZov)
            {
                if (bZClients)
                {
                    var zClients = GetZClients();
                    if (zClients.Count > 0)
                    {
                        if (zFilter.Length > 0)
                            zFilter += " AND infiniu2_zovorders.dbo.MainOrders.ClientID IN (" +
                                       string.Join(",", zClients.OfType<int>().ToArray()) + ")";
                        else
                            zFilter = " WHERE infiniu2_zovorders.dbo.MainOrders.ClientID IN (" +
                                      string.Join(",", zClients.OfType<int>().ToArray()) + ")";
                    }
                    else
                    {
                        if (zFilter.Length > 0)
                            zFilter += " AND infiniu2_zovorders.dbo.MainOrders.ClientID = -1";
                        else
                            zFilter = " WHERE infiniu2_zovorders.dbo.MainOrders.ClientID = -1";
                    }
                }

                if (bZCreateDate)
                {
                    if (zFilter.Length > 0)
                        zFilter += " AND CAST(DocDateTime AS DATE) >= '" +
                                   Convert.ToDateTime(zCreateDateFrom).ToString("yyyy-MM-dd") +
                                   "' AND CAST(DocDateTime AS DATE) <= '" +
                                   Convert.ToDateTime(zCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                    else
                        zFilter = " WHERE CAST(DocDateTime AS DATE) >= '" +
                                  Convert.ToDateTime(zCreateDateFrom).ToString("yyyy-MM-dd") +
                                  "' AND CAST(DocDateTime AS DATE) <= '" +
                                  Convert.ToDateTime(zCreateDateTo).ToString("yyyy-MM-dd") + "' ";
                }

                if (bZDispDate)
                {
                    if (zFilter.Length > 0)
                        zFilter += " AND CAST(DispDate AS DATE) >= '" +
                                   Convert.ToDateTime(zDispDateFrom).ToString("yyyy-MM-dd") +
                                   "' AND CAST(DispDate AS DATE) <= '" +
                                   Convert.ToDateTime(zDispDateTo).ToString("yyyy-MM-dd") + "' ";
                    else
                        zFilter = " WHERE CAST(DispDate AS DATE) >= '" +
                                  Convert.ToDateTime(zDispDateFrom).ToString("yyyy-MM-dd") +
                                  "' AND CAST(DispDate AS DATE) <= '" +
                                  Convert.ToDateTime(zDispDateTo).ToString("yyyy-MM-dd") + "' ";
                }

                if (!bZClients && !bZCreateDate && !bZDispDate)
                    zFilter = " WHERE SampleMainOrders.MainOrderID = -1";
            }
            else
            {
                zFilter = " WHERE SampleMainOrders.MainOrderID = -1";
            }

            var selectCommand =
                @"SELECT SampleMainOrders.*, MegaOrders.ClientID, MegaOrders.OrderNumber, Clients.ClientName FROM SampleMainOrders
                INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON MegaOrders.ClientID=Clients.ClientID" +
                mFilter + " ORDER BY MainOrderID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                _mOrdersDataTable.Clear();
                da.Fill(_mOrdersDataTable);
            }

            selectCommand =
                @"SELECT SampleMainOrders.*, infiniu2_zovorders.dbo.MainOrders.ClientID, infiniu2_zovorders.dbo.MainOrders.DocNumber, Clients.ClientName FROM SampleMainOrders
                INNER JOIN JoinMainOrders ON SampleMainOrders.MainOrderID = JoinMainOrders.MarketMainOrderID
                INNER JOIN infiniu2_zovorders.dbo.MainOrders ON JoinMainOrders.ZOVMainOrderID=infiniu2_zovorders.dbo.MainOrders.MainOrderID
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON JoinMainOrders.ZOVClientID=Clients.ClientID" +
                zFilter + " ORDER BY SampleMainOrders.MainOrderID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                _zOrdersDataTable.Clear();
                da.Fill(_zOrdersDataTable);
            }

            OrdersDataTable.Clear();
            for (var i = 0; i < _mOrdersDataTable.Rows.Count; i++)
            {
                var newRow = OrdersDataTable.NewRow();
                newRow["FirmType"] = 1;
                newRow["ClientID"] = _mOrdersDataTable.Rows[i]["ClientID"];
                newRow["ClientName"] = _mOrdersDataTable.Rows[i]["ClientName"];
                newRow["OrderNumber"] = _mOrdersDataTable.Rows[i]["OrderNumber"];
                newRow["MainOrderID"] = _mOrdersDataTable.Rows[i]["MainOrderID"];
                newRow["CreateDate"] = _mOrdersDataTable.Rows[i]["DocDateTime"];
                newRow["Cost"] = _mOrdersDataTable.Rows[i]["OrderCost"];
                newRow["Square"] = _mOrdersDataTable.Rows[i]["FrontsSquare"];
                newRow["Description"] = _mOrdersDataTable.Rows[i]["Description"];
                newRow["Foto"] = _mOrdersDataTable.Rows[i]["Foto"];
                newRow["DispDate"] = _mOrdersDataTable.Rows[i]["DispDate"];
                var rows = _mShopAddressesDataTable.Select(
                    "ClientID=" + Convert.ToInt32(_mOrdersDataTable.Rows[i]["ClientID"]));
                if (rows.Any())
                    newRow["ShopAddresses"] = "Показать магазины";
                else
                    newRow["ShopAddresses"] = "Добавить магазины";
                OrdersDataTable.Rows.Add(newRow);
            }

            for (var i = 0; i < _zOrdersDataTable.Rows.Count; i++)
            {
                var newRow = OrdersDataTable.NewRow();
                newRow["FirmType"] = 0;
                newRow["ClientID"] = _zOrdersDataTable.Rows[i]["ClientID"];
                newRow["ClientName"] = _zOrdersDataTable.Rows[i]["ClientName"];
                newRow["OrderNumber"] = _zOrdersDataTable.Rows[i]["DocNumber"];
                newRow["MainOrderID"] = _zOrdersDataTable.Rows[i]["MainOrderID"];
                newRow["CreateDate"] = _zOrdersDataTable.Rows[i]["DocDateTime"];
                newRow["Cost"] = _zOrdersDataTable.Rows[i]["OrderCost"];
                newRow["Square"] = _zOrdersDataTable.Rows[i]["FrontsSquare"];
                newRow["Description"] = _zOrdersDataTable.Rows[i]["Description"];
                newRow["Foto"] = _zOrdersDataTable.Rows[i]["Foto"];
                newRow["DispDate"] = _zOrdersDataTable.Rows[i]["DispDate"];
                var rows = _zShopAddressesDataTable.Select(
                    "ClientID=" + Convert.ToInt32(_zOrdersDataTable.Rows[i]["ClientID"]));
                if (rows.Any())
                    newRow["ShopAddresses"] = "Показать магазины";
                else
                    newRow["ShopAddresses"] = "Добавить магазины";
                OrdersDataTable.Rows.Add(newRow);
            }

            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartMDocNumber(string docText)
        {
            var search = string.Format(" WHERE OrderNumber LIKE '%" + docText + "%'");

            var selectCommand =
                @"SELECT SampleMainOrders.*, MegaOrders.OrderNumber, MegaOrders.ClientID, Clients.ClientName FROM SampleMainOrders
                INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON MegaOrders.ClientID=Clients.ClientID" +
                search;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                _mOrdersDataTable.Clear();
                da.Fill(_mOrdersDataTable);
            }

            OrdersDataTable.Clear();
            for (var i = 0; i < _mOrdersDataTable.Rows.Count; i++)
            {
                var newRow = OrdersDataTable.NewRow();
                newRow["FirmType"] = 1;
                newRow["ClientID"] = _mOrdersDataTable.Rows[i]["ClientID"];
                newRow["ClientName"] = _mOrdersDataTable.Rows[i]["ClientName"];
                newRow["OrderNumber"] = _mOrdersDataTable.Rows[i]["OrderNumber"];
                newRow["MainOrderID"] = _mOrdersDataTable.Rows[i]["MainOrderID"];
                newRow["CreateDate"] = _mOrdersDataTable.Rows[i]["DocDateTime"];
                newRow["Cost"] = _mOrdersDataTable.Rows[i]["OrderCost"];
                newRow["Square"] = _mOrdersDataTable.Rows[i]["FrontsSquare"];
                newRow["Description"] = _mOrdersDataTable.Rows[i]["Description"];
                newRow["Foto"] = _mOrdersDataTable.Rows[i]["Foto"];
                newRow["DispDate"] = _mOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(newRow);
            }

            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartZDocNumber(string docText)
        {
            var search = string.Format(" WHERE infiniu2_zovorders.dbo.MainOrders.DocNumber LIKE '%" + docText + "%'");

            //var selectCommand = @"SELECT SampleMainOrders.*, Clients.ClientName FROM SampleMainOrders
            //    INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON SampleMainOrders.ClientID=Clients.ClientID" +
            //                    search;

            var selectCommand =
                @"SELECT SampleMainOrders.*, infiniu2_zovorders.dbo.MainOrders.ClientID, infiniu2_zovorders.dbo.MainOrders.DocNumber, Clients.ClientName FROM SampleMainOrders
                INNER JOIN JoinMainOrders ON SampleMainOrders.MainOrderID = JoinMainOrders.MarketMainOrderID
                INNER JOIN infiniu2_zovorders.dbo.MainOrders ON JoinMainOrders.ZOVMainOrderID=infiniu2_zovorders.dbo.MainOrders.MainOrderID
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON JoinMainOrders.ZOVClientID=Clients.ClientID" +
                search + " ORDER BY SampleMainOrders.MainOrderID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                _zOrdersDataTable.Clear();
                da.Fill(_zOrdersDataTable);
            }

            OrdersDataTable.Clear();
            for (var i = 0; i < _zOrdersDataTable.Rows.Count; i++)
            {
                var newRow = OrdersDataTable.NewRow();
                newRow["FirmType"] = 0;
                newRow["ClientID"] = _zOrdersDataTable.Rows[i]["ClientID"];
                newRow["ClientName"] = _zOrdersDataTable.Rows[i]["ClientName"];
                newRow["OrderNumber"] = _zOrdersDataTable.Rows[i]["DocNumber"];
                newRow["MainOrderID"] = _zOrdersDataTable.Rows[i]["MainOrderID"];
                newRow["CreateDate"] = _zOrdersDataTable.Rows[i]["DocDateTime"];
                newRow["Cost"] = _zOrdersDataTable.Rows[i]["OrderCost"];
                newRow["Square"] = _zOrdersDataTable.Rows[i]["FrontsSquare"];
                newRow["Description"] = _zOrdersDataTable.Rows[i]["Description"];
                newRow["Foto"] = _zOrdersDataTable.Rows[i]["Foto"];
                newRow["DispDate"] = _zOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(newRow);
            }

            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartMNotes(string docText)
        {
            var search = string.Format(" WHERE Description LIKE '%" + docText + "%'");

            var selectCommand =
                @"SELECT SampleMainOrders.*, MegaOrders.OrderNumber, MegaOrders.ClientID, Clients.ClientName FROM SampleMainOrders
                INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON MegaOrders.ClientID=Clients.ClientID" +
                search;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                _mOrdersDataTable.Clear();
                da.Fill(_mOrdersDataTable);
            }

            OrdersDataTable.Clear();
            for (var i = 0; i < _mOrdersDataTable.Rows.Count; i++)
            {
                var newRow = OrdersDataTable.NewRow();
                newRow["FirmType"] = 1;
                newRow["ClientID"] = _mOrdersDataTable.Rows[i]["ClientID"];
                newRow["ClientName"] = _mOrdersDataTable.Rows[i]["ClientName"];
                newRow["OrderNumber"] = _mOrdersDataTable.Rows[i]["OrderNumber"];
                newRow["MainOrderID"] = _mOrdersDataTable.Rows[i]["MainOrderID"];
                newRow["CreateDate"] = _mOrdersDataTable.Rows[i]["DocDateTime"];
                newRow["Cost"] = _mOrdersDataTable.Rows[i]["OrderCost"];
                newRow["Square"] = _mOrdersDataTable.Rows[i]["FrontsSquare"];
                newRow["Description"] = _mOrdersDataTable.Rows[i]["Description"];
                newRow["Foto"] = _mOrdersDataTable.Rows[i]["Foto"];
                newRow["DispDate"] = _mOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(newRow);
            }

            OrdersDataTable.AcceptChanges();
        }

        public void SearchPartZNotes(string docText)
        {
            var search = string.Format(" WHERE SampleMainOrders.Description LIKE '%" + docText + "%'");

            var selectCommand =
                @"SELECT SampleMainOrders.*, infiniu2_zovorders.dbo.MainOrders.ClientID, infiniu2_zovorders.dbo.MainOrders.DocNumber, Clients.ClientName FROM SampleMainOrders
                INNER JOIN JoinMainOrders ON SampleMainOrders.MainOrderID = JoinMainOrders.MarketMainOrderID
                INNER JOIN infiniu2_zovorders.dbo.MainOrders ON JoinMainOrders.ZOVMainOrderID=infiniu2_zovorders.dbo.MainOrders.MainOrderID
                INNER JOIN infiniu2_zovreference.dbo.Clients AS Clients ON JoinMainOrders.ZOVClientID=Clients.ClientID" +
                search + " ORDER BY SampleMainOrders.MainOrderID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                _zOrdersDataTable.Clear();
                da.Fill(_zOrdersDataTable);
            }

            OrdersDataTable.Clear();
            for (var i = 0; i < _zOrdersDataTable.Rows.Count; i++)
            {
                var newRow = OrdersDataTable.NewRow();
                newRow["FirmType"] = 0;
                newRow["ClientID"] = _zOrdersDataTable.Rows[i]["ClientID"];
                newRow["ClientName"] = _zOrdersDataTable.Rows[i]["ClientName"];
                newRow["OrderNumber"] = _zOrdersDataTable.Rows[i]["DocNumber"];
                newRow["MainOrderID"] = _zOrdersDataTable.Rows[i]["MainOrderID"];
                newRow["CreateDate"] = _zOrdersDataTable.Rows[i]["DocDateTime"];
                newRow["Cost"] = _zOrdersDataTable.Rows[i]["OrderCost"];
                newRow["Square"] = _zOrdersDataTable.Rows[i]["FrontsSquare"];
                newRow["Description"] = _zOrdersDataTable.Rows[i]["Description"];
                newRow["Foto"] = _zOrdersDataTable.Rows[i]["Foto"];
                newRow["DispDate"] = _zOrdersDataTable.Rows[i]["DispDate"];
                OrdersDataTable.Rows.Add(newRow);
            }

            OrdersDataTable.AcceptChanges();
        }

        public void FilterProductByMainOrder(bool isZov, bool isMDecor, bool isZDecor, int mainOrderId, ref bool frontsVisible, ref bool decorVisible)
        {
            if (isZov)
            {
                frontsVisible = ZFrontsOrders.Filter(mainOrderId, 0);
                if (isZDecor)
                    decorVisible = ZDecorOrders.Filter(mainOrderId, 0);
            }
            else
            {
                frontsVisible = MFrontsOrders.Filter(mainOrderId, 0);
                if (isMDecor)
                    decorVisible = MDecorOrders.Filter(mainOrderId, 0);
            }
        }

        public DataTable GetPermissions(int userId, string formName)
        {
            using (var da = new SqlDataAdapter("SELECT * FROM UserRoles WHERE UserID = " + userId +
                                               " AND RoleID IN (SELECT RoleID FROM Roles WHERE ModuleID IN " +
                                               " (SELECT ModuleID FROM Modules WHERE FormName = '" + formName + "'))",
                ConnectionStrings.UsersConnectionString))
            {
                using (var dt = new DataTable())
                {
                    da.Fill(dt);

                    return dt;
                }
            }
        }

        public static void FixOrderEvent(int megaOrderId, string @event)
        {
            var tempDt = new DataTable();
            var selectCommand = @"SELECT * FROM MegaOrders WHERE MegaOrderID = " + megaOrderId;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                da.Fill(tempDt);
            }

            selectCommand = @"SELECT TOP 0 * FROM MegaOrdersEvents";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);
                        if (@event == "Заказ удален")
                        {
                            var newRow = dt.NewRow();
                            newRow["MegaOrderID"] = megaOrderId;
                            newRow["Event"] = @event;
                            newRow["EventDate"] = Security.GetCurrentDate();
                            newRow["EventUserID"] = Security.CurrentUserID;
                            dt.Rows.Add(newRow);
                            da.Update(dt);
                        }

                        if (tempDt.Rows.Count > 0)
                        {
                            var newRow = dt.NewRow();
                            newRow.ItemArray = tempDt.Rows[0].ItemArray;
                            newRow["Event"] = @event;
                            newRow["EventDate"] = Security.GetCurrentDate();
                            newRow["EventUserID"] = Security.CurrentUserID;
                            dt.Rows.Add(newRow);
                            da.Update(dt);
                        }
                        else
                        {
                            var newRow = dt.NewRow();
                            newRow["MegaOrderID"] = megaOrderId;
                            newRow["Event"] = "Заказа не существует";
                            newRow["EventDate"] = Security.GetCurrentDate();
                            newRow["EventUserID"] = Security.CurrentUserID;
                            dt.Rows.Add(newRow);
                            da.Update(dt);
                        }
                    }
                }
            }
        }

        public void SaveMDescription()
        {
            var filter = "FirmType=1";
            var dt1 = new DataTable();
            using (var dv = new DataView(OrdersDataTable, filter, string.Empty, DataViewRowState.ModifiedCurrent))
            {
                dt1 = dv.ToTable(true, "MainOrderID", "Description");
            }

            if (dt1.Rows.Count == 0)
                return;
            filter = string.Empty;
            for (var i = 0; i < dt1.Rows.Count; i++)
                filter += Convert.ToInt32(dt1.Rows[i]["MainOrderID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = " WHERE MainOrderID IN (" + filter + ")";
            }
            else
            {
                filter = " WHERE MainOrderID = - 1";
            }

            var selectCommand = @"SELECT SampleMainOrderID, MainOrderID, Description FROM SampleMainOrders" + filter;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt2 = new DataTable())
                    {
                        if (da.Fill(dt2) > 0)
                        {
                            for (var i = 0; i < dt1.Rows.Count; i++)
                            {
                                var mainOrderId = Convert.ToInt32(dt1.Rows[i]["MainOrderID"]);
                                var rows = dt2.Select("MainOrderID=" + mainOrderId);
                                if (rows.Count() > 0)
                                    rows[0]["Description"] = dt1.Rows[i]["Description"];
                            }

                            da.Update(dt2);
                        }
                    }
                }
            }
        }

        public void SaveZDescription()
        {
            var filter = "FirmType=0";
            var dt1 = new DataTable();
            using (var dv = new DataView(OrdersDataTable, filter, string.Empty, DataViewRowState.ModifiedCurrent))
            {
                dt1 = dv.ToTable(true, "MainOrderID", "Description");
            }

            if (dt1.Rows.Count == 0)
                return;
            filter = string.Empty;
            for (var i = 0; i < dt1.Rows.Count; i++)
                filter += Convert.ToInt32(dt1.Rows[i]["MainOrderID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = " WHERE MainOrderID IN (" + filter + ")";
            }
            else
            {
                filter = " WHERE MainOrderID = - 1";
            }

            var selectCommand = @"SELECT SampleMainOrderID, MainOrderID, Description FROM SampleMainOrders" + filter;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt2 = new DataTable())
                    {
                        if (da.Fill(dt2) > 0)
                        {
                            for (var i = 0; i < dt1.Rows.Count; i++)
                            {
                                var mainOrderId = Convert.ToInt32(dt1.Rows[i]["MainOrderID"]);
                                var rows = dt2.Select("MainOrderID=" + mainOrderId);
                                if (rows.Count() > 0)
                                    rows[0]["Description"] = dt1.Rows[i]["Description"];
                            }

                            da.Update(dt2);
                        }
                    }
                }
            }
        }

        public Image GetMFoto(int mainOrderId)
        {
            using (var da = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var dt = new DataTable())
                {
                    if (da.Fill(dt) == 0)
                        return null;

                    if (dt.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    var fileName = dt.Rows[0]["FileName"].ToString();

                    try
                    {
                        using (var ms = new MemoryStream(
                            Fm.ReadFile(Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + fileName,
                                Convert.ToInt64(dt.Rows[0]["FileSize"]), Configs.FTPType)))
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

        public Image GetZFoto(int mainOrderId)
        {
            using (var da = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (var dt = new DataTable())
                {
                    if (da.Fill(dt) == 0)
                        return null;
                    if (dt.Rows[0]["FileName"] == DBNull.Value)
                        return null;
                    var fileName = dt.Rows[0]["FileName"].ToString();
                    try
                    {
                        using (var ms = new MemoryStream(
                            Fm.ReadFile(Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + fileName,
                                Convert.ToInt64(dt.Rows[0]["FileSize"]), Configs.FTPType)))
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

        public bool AttachMFoto(string extension, string fileName, string path, int mainOrderId)
        {
            var ok = true;
            try
            {
                var sDestFolder = Configs.DocumentsPath + FileManager.GetPath("SamplesFoto");
                var sFileName = fileName;
                var j = 1;
                while (Fm.FileExist(sDestFolder + "/" + sFileName + extension, Configs.FTPType))
                    sFileName = fileName + "(" + j++ + ")";
                fileName = sFileName + extension;
                if (Fm.UploadFile(path, sDestFolder + "/" + sFileName + extension, Configs.FTPType) == false)
                    ok = false;
            }
            catch
            {
                ok = false;
            }

            using (var da = new SqlDataAdapter("SELECT TOP 0 * FROM SamplesFoto",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);

                        FileInfo fi;
                        try
                        {
                            fi = new FileInfo(path);
                        }
                        catch
                        {
                            ok = false;
                            return false;
                        }

                        var newRow = dt.NewRow();
                        newRow["MainOrderID"] = mainOrderId;
                        newRow["FileName"] = fileName;
                        newRow["FileSize"] = fi.Length;
                        dt.Rows.Add(newRow);

                        da.Update(dt);
                    }
                }
            }

            if (ok)
                using (var da = new SqlDataAdapter(
                    "SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " +
                    mainOrderId,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (var cb = new SqlCommandBuilder(da))
                    {
                        using (var dt = new DataTable())
                        {
                            if (da.Fill(dt) > 0)
                            {
                                dt.Rows[0]["Foto"] = true;
                                da.Update(dt);
                            }
                        }
                    }
                }

            return ok;
        }

        public bool AttachZFoto(string extension, string fileName, string path, int mainOrderId)
        {
            var ok = true;
            try
            {
                var sDestFolder = Configs.DocumentsPath + FileManager.GetPath("SamplesFoto");
                var sFileName = fileName;
                var j = 1;
                while (Fm.FileExist(sDestFolder + "/" + sFileName + extension, Configs.FTPType))
                    sFileName = fileName + "(" + j++ + ")";
                fileName = sFileName + extension;
                if (Fm.UploadFile(path, sDestFolder + "/" + sFileName + extension, Configs.FTPType) == false)
                    ok = false;
            }
            catch
            {
                ok = false;
            }

            using (var da = new SqlDataAdapter("SELECT TOP 0 * FROM SamplesFoto",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);

                        FileInfo fi;
                        try
                        {
                            fi = new FileInfo(path);
                        }
                        catch
                        {
                            ok = false;
                            return false;
                        }

                        var newRow = dt.NewRow();
                        newRow["MainOrderID"] = mainOrderId;
                        newRow["FileName"] = fileName;
                        newRow["FileSize"] = fi.Length;
                        dt.Rows.Add(newRow);

                        da.Update(dt);
                    }
                }
            }

            if (ok)
                using (var da = new SqlDataAdapter(
                    "SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " +
                    mainOrderId,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (var cb = new SqlCommandBuilder(da))
                    {
                        using (var dt = new DataTable())
                        {
                            if (da.Fill(dt) > 0)
                            {
                                dt.Rows[0]["Foto"] = true;
                                da.Update(dt);
                            }
                        }
                    }
                }

            return ok;
        }

        public void DetachMFoto(int mainOrderId)
        {
            using (var da = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);
                        var bOk = false;
                        foreach (DataRow row in dt.Rows)
                            bOk = Fm.DeleteFile(
                                Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + row["FileName"],
                                Configs.FTPType);
                        if (bOk)
                        {
                            dt.Rows[0].Delete();
                            da.Update(dt);
                        }
                    }
                }
            }

            using (var da = new SqlDataAdapter("DELETE FROM SamplesFoto WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);
                    }
                }
            }

            using (var da = new SqlDataAdapter(
                "SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        if (da.Fill(dt) > 0)
                        {
                            dt.Rows[0]["Foto"] = false;
                            da.Update(dt);
                        }
                    }
                }
            }
        }

        public void DetachZFoto(int mainOrderId)
        {
            using (var da = new SqlDataAdapter("SELECT * FROM SamplesFoto WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);
                        var bOk = false;
                        foreach (DataRow row in dt.Rows)
                            bOk = Fm.DeleteFile(
                                Configs.DocumentsPath + FileManager.GetPath("SamplesFoto") + "/" + row["FileName"],
                                Configs.FTPType);
                        if (bOk)
                        {
                            dt.Rows[0].Delete();
                            da.Update(dt);
                        }
                    }
                }
            }

            using (var da = new SqlDataAdapter("DELETE FROM SamplesFoto WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);
                    }
                }
            }

            using (var da = new SqlDataAdapter(
                "SELECT SampleMainOrderID, MainOrderID, Foto FROM SampleMainOrders WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var cb = new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        if (da.Fill(dt) > 0)
                        {
                            dt.Rows[0]["Foto"] = false;
                            da.Update(dt);
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

        public DataTable GetData()
        {
            DataTable dt = OrdersDataTable.Copy();

            string selectCommand = @"SELECT ShopAddressOrders.*, ShopAddresses.Address FROM ShopAddressOrders INNER JOIN ShopAddresses ON ShopAddressOrders.ShopAddressID=ShopAddresses.ShopAddressID";
            using (DataTable shopAddressOrdersDataTable = new DataTable())
            {
                using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVReferenceConnectionString))
                {
                    da.Fill(shopAddressOrdersDataTable);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int firmType = Convert.ToInt32(dt.Rows[i]["FirmType"]);
                        int clietnId = Convert.ToInt32(dt.Rows[i]["ClientID"]);
                        int mainOrderId = Convert.ToInt32(dt.Rows[i]["MainOrderID"]);
                        if (firmType == 0)
                        {
                            var rows = shopAddressOrdersDataTable.Select("MainOrderID=" + mainOrderId);
                            if (rows.Any())
                            {
                                StringBuilder ShopAddresses = new StringBuilder();
                                DataRow last = rows.Last();
                                foreach (var t in rows)
                                {
                                    ShopAddresses.Append(t["Address"].ToString());
                                    if (!t.Equals(last))
                                        ShopAddresses.AppendLine();
                                }

                                dt.Rows[i]["ShopAddresses"] = ShopAddresses.ToString();
                            }
                            else
                                dt.Rows[i]["ShopAddresses"] = "";
                        }

                        if (firmType == 1)
                        {
                            var rows = _mShopAddressesDataTable.Select("ClientID=" + clietnId);
                            if (rows.Any())
                            {
                                StringBuilder ShopAddresses = new StringBuilder();
                                DataRow last = rows.Last();
                                foreach (var t in rows)
                                {
                                    //ShopAddresses.Append(t["City"].ToString());
                                    //ShopAddresses.Append(" ");
                                    ShopAddresses.Append(t["Address"].ToString());
                                    if (!t.Equals(last))
                                        ShopAddresses.AppendLine();
                                }

                                dt.Rows[i]["ShopAddresses"] = ShopAddresses.ToString();
                            }
                            else
                                dt.Rows[i]["ShopAddresses"] = "";
                        }
                    }
                }
            }

            dt.Columns.Remove("FirmType");
            dt.Columns.Remove("ClientID");
            dt.Columns.Remove("MainOrderID");
            dt.Columns.Remove("CreateDate");
            dt.Columns.Remove("Foto");
            return dt;
        }
    }

    public class ZovSampleShops
    {
        public int ClientId { get; set; } = -1;
        public int MainOrderId { get; set; } = -1;

        private SqlDataAdapter _shopsDataAdapter;
        private SqlDataAdapter _shopAddressDataAdapter;

        public BindingSource AllShopsBindingSource;
        public BindingSource OrderShopsBindingSource;
        private DataTable _allShopsDataTable;
        private DataTable _orderShopsDataTable;
        private DataTable _shopAddressOrdersDataTable;

        private SqlCommandBuilder _shopsCommandBuilder;
        private SqlCommandBuilder _shopAddressCommandBuilder;

        public void Fill()
        {
            _shopAddressOrdersDataTable = new DataTable();
            _allShopsDataTable = new DataTable();
            _orderShopsDataTable = new DataTable();

            _shopAddressDataAdapter = new SqlDataAdapter("SELECT * FROM ShopAddressOrders",
                ConnectionStrings.ZOVReferenceConnectionString);
            _shopAddressCommandBuilder = new SqlCommandBuilder(_shopAddressDataAdapter);
            _shopAddressDataAdapter.Fill(_shopAddressOrdersDataTable);

            var selectCommand = "SELECT * FROM ShopAddresses WHERE ClientID=" + ClientId + " ORDER BY Address";
            _shopsDataAdapter = new SqlDataAdapter(selectCommand, ConnectionStrings.ZOVReferenceConnectionString);
            _shopsCommandBuilder = new SqlCommandBuilder(_shopsDataAdapter);
            _shopsDataAdapter.Fill(_allShopsDataTable);
            _allShopsDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            _orderShopsDataTable = _allShopsDataTable.Clone();

            for (var i = 0; i < _allShopsDataTable.Rows.Count; i++)
            {
                _allShopsDataTable.Rows[i]["Check"] = false;
            }

            AllShopsBindingSource = new BindingSource
            {
                DataSource = _allShopsDataTable,
                Sort = "Check"
            };
            OrderShopsBindingSource = new BindingSource
            {
                DataSource = _orderShopsDataTable,
                Filter = "Check=1"
            };
        }

        public void SaveShopAddressOrders()
        {
            try
            {
                for (var i = 0; i < _orderShopsDataTable.Rows.Count; i++)
                {
                    bool check = Convert.ToBoolean(_orderShopsDataTable.Rows[i]["Check"]);
                    int shopAddressId = Convert.ToInt32(_orderShopsDataTable.Rows[i]["ShopAddressID"]);

                    if (check)
                    {
                        var rows = _shopAddressOrdersDataTable.Select("MainOrderID=" + MainOrderId + " AND ShopAddressID=" + shopAddressId);
                        if (!rows.Any())
                        {
                            var newRow = _shopAddressOrdersDataTable.NewRow();
                            newRow["ShopAddressID"] = shopAddressId;
                            newRow["MainOrderID"] = MainOrderId;

                            _shopAddressOrdersDataTable.Rows.Add(newRow);
                        }
                    }
                    else
                    {
                        var rows = _shopAddressOrdersDataTable.Select("MainOrderID=" + MainOrderId + " AND ShopAddressID=" + shopAddressId);
                        foreach (var t in rows)
                        {
                            t.Delete();
                        }
                    }

                }
                _shopAddressDataAdapter.Update(_shopAddressOrdersDataTable);
                _shopAddressOrdersDataTable.Clear();
                _shopAddressDataAdapter.Fill(_shopAddressOrdersDataTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error");
            }
        }

        public void SaveShopAddresses()
        {
            try
            {
                _shopsDataAdapter.Update(_allShopsDataTable);
                _allShopsDataTable.Clear();
                _shopsDataAdapter.Fill(_allShopsDataTable);
                for (var i = 0; i < _allShopsDataTable.Rows.Count; i++)
                {
                    _allShopsDataTable.Rows[i]["Check"] = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error");
            }
        }

        public void AddNewShopAddressOrder(int shopAddressId)
        {
            if (_shopAddressOrdersDataTable.Select("MainOrderID=" + MainOrderId + " AND ShopAddressID=" + shopAddressId).Any())
                return;

            var rows = _allShopsDataTable.Select("ShopAddressID=" + shopAddressId);
            if (rows.Any())
            {
                int index = _allShopsDataTable.Rows.IndexOf(rows[0]);

                var newRow = _orderShopsDataTable.NewRow();
                for (var j = 0; j < _allShopsDataTable.Columns.Count; j++)
                {
                    var columnName = _allShopsDataTable.Columns[j].ColumnName;
                    if (_orderShopsDataTable.Columns.Contains(columnName))
                        newRow[_allShopsDataTable.Columns[j].ColumnName] =
                            _allShopsDataTable.Rows[index][columnName];
                    newRow["Check"] = true;
                }
                _orderShopsDataTable.Rows.Add(newRow);
            }
        }

        public void UpdatesTables()
        {
            _orderShopsDataTable.Clear();

            for (var i = 0; i < _allShopsDataTable.Rows.Count; i++)
            {
                if (_allShopsDataTable.Rows[i].RowState == DataRowState.Added)
                    continue;

                _allShopsDataTable.Rows[i]["Check"] = false;
                int shopAddressId = Convert.ToInt32(_allShopsDataTable.Rows[i]["ShopAddressID"]);

                var rows = _shopAddressOrdersDataTable.Select("MainOrderID=" + MainOrderId + " AND ShopAddressID=" + shopAddressId);
                if (rows.Any())
                {
                    _allShopsDataTable.Rows[i]["Check"] = true;

                    var newRow = _orderShopsDataTable.NewRow();
                    for (var j = 0; j < _allShopsDataTable.Columns.Count; j++)
                    {
                        var columnName = _allShopsDataTable.Columns[j].ColumnName;
                        if (_orderShopsDataTable.Columns.Contains(columnName))
                            newRow[_allShopsDataTable.Columns[j].ColumnName] =
                                _allShopsDataTable.Rows[i][columnName];
                        newRow["Check"] = true;
                    }

                    _orderShopsDataTable.Rows.Add(newRow);
                }
            }

            //AllShopsBindingSource.Filter = "Check=0";
        }
    }

    public class samplesReport
    {
        DataTable samplesDt = null;
        DataTable samplesFrontsDt = null;

        public samplesReport()
        {
            samplesDt = new DataTable();
            samplesFrontsDt = new DataTable();
        }

        public void GetSamples(DataTable dt)
        {
            if (dt.Rows.Count == 0) return;

            samplesDt.Clear();
            using (DataView dv = new DataView(dt))
            {
                dv.Sort = "ClientName";
                samplesDt = dv.ToTable();
            }
        }

        public void GetSamplesFronts(DataTable dt)
        {
            if (dt.Rows.Count == 0) return;

            samplesFrontsDt.Clear();
            using (DataView dv = new DataView(dt))
            {
                dv.Sort = "ClientName, DocNumber, DispDate, FrontName, ColorName";
                samplesFrontsDt = dv.ToTable();
            }
        }

        public void Report(string FileName)
        {
            if (samplesDt.Rows.Count <= 0) return;

            int pos = 0;

            //Export to excel
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
            SimpleF.FontHeightInPoints = 10;
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
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
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
            WeightCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            WeightCS.BottomBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            WeightCS.LeftBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            WeightCS.RightBorderColor = HSSFColor.BLACK.index;
            WeightCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            WeightCS.TopBorderColor = HSSFColor.BLACK.index;
            WeightCS.WrapText = true;
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

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Образцы");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int displayIndex = 0;
            sheet1.SetColumnWidth(displayIndex++, 25 * 256);
            sheet1.SetColumnWidth(displayIndex++, 20 * 256);
            sheet1.SetColumnWidth(displayIndex++, 15 * 256);
            sheet1.SetColumnWidth(displayIndex++, 25 * 256);
            sheet1.SetColumnWidth(displayIndex++, 10 * 256);
            sheet1.SetColumnWidth(displayIndex++, 10 * 256);
            sheet1.SetColumnWidth(displayIndex, 50 * 256);
            displayIndex = 0;


            pos += 2;

            var cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Клиент");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("№ заказа");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Дата отгрузки");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Описание");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Квадратура");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Стоимость");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex);
            cell1.SetCellValue("Магазины");
            cell1.CellStyle = SimpleHeaderCS;

            pos++;

            int ColumnCount = samplesDt.Columns.Count;
            for (int x = 0; x < samplesDt.Rows.Count; x++)
            {
                for (int y = 0; y < ColumnCount; y++)
                {
                    Type t = samplesDt.Rows[x][y].GetType();



                    if (samplesDt.Columns.IndexOf("ShopAddresses") == y)
                    {
                        string str = samplesDt.Rows[x][y].ToString();
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(str);

                        cell.CellStyle = WeightCS;
                        continue;
                    }

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(samplesDt.Rows[x][y]));

                        cell.CellStyle = CountCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(samplesDt.Rows[x][y]));
                        cell.CellStyle = SimpleCS;
                        continue;
                    }
                    if (t.Name == "Boolean")
                    {
                        bool b = Convert.ToBoolean(samplesDt.Rows[x][y]);
                        string str = "Да";
                        if (!b)
                            str = "Нет";
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(str);
                        cell.CellStyle = SimpleCS;
                        continue;
                    }
                    if (t.Name == "DateTime")
                    {
                        string dateTime = Convert.ToDateTime(samplesDt.Rows[x][y]).ToShortDateString();
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(dateTime);
                        cell.CellStyle = SimpleCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(samplesDt.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCS;
                        continue;
                    }
                }
                pos++;
            }

            FrontsReport(ref hssfworkbook, SimpleHeaderCS, WeightCS, SimpleCS, CountCS);

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void FrontsReport(ref HSSFWorkbook hssfworkbook, HSSFCellStyle SimpleHeaderCS, HSSFCellStyle WeightCS, HSSFCellStyle SimpleCS, HSSFCellStyle CountCS)
        {
            int pos = 0;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Фасады");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int displayIndex = 0;
            sheet1.SetColumnWidth(displayIndex++, 25 * 256);
            sheet1.SetColumnWidth(displayIndex++, 20 * 256);
            sheet1.SetColumnWidth(displayIndex++, 15 * 256);
            sheet1.SetColumnWidth(displayIndex++, 25 * 256);
            sheet1.SetColumnWidth(displayIndex++, 25 * 256);
            sheet1.SetColumnWidth(displayIndex++, 25 * 256);
            sheet1.SetColumnWidth(displayIndex++, 10 * 256);
            sheet1.SetColumnWidth(displayIndex++, 10 * 256);
            sheet1.SetColumnWidth(displayIndex, 50 * 256);
            displayIndex = 0;

            if (samplesFrontsDt.Rows.Count <= 0) return;

            pos += 2;

            var cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Клиент");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("№ заказа");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Дата отгрузки");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Описание");
            cell1.CellStyle = SimpleHeaderCS;
            
            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Фасад");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Цвет");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Квадратура");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex++);
            cell1.SetCellValue("Стоимость");
            cell1.CellStyle = SimpleHeaderCS;

            cell1 = sheet1.CreateRow(pos).CreateCell(displayIndex);
            cell1.SetCellValue("Магазины");
            cell1.CellStyle = SimpleHeaderCS;

            pos++;

            int ColumnCount = samplesFrontsDt.Columns.Count;
            for (int x = 0; x < samplesFrontsDt.Rows.Count; x++)
            {
                for (int y = 0; y < ColumnCount; y++)
                {
                    Type t = samplesFrontsDt.Rows[x][y].GetType();

                    if (samplesFrontsDt.Columns.IndexOf("ShopAddresses") == y)
                    {
                        string str = samplesFrontsDt.Rows[x][y].ToString();
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(str);

                        cell.CellStyle = WeightCS;
                        continue;
                    }

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(samplesFrontsDt.Rows[x][y]));

                        cell.CellStyle = CountCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(samplesFrontsDt.Rows[x][y]));
                        cell.CellStyle = SimpleCS;
                        continue;
                    }
                    if (t.Name == "Boolean")
                    {
                        bool b = Convert.ToBoolean(samplesFrontsDt.Rows[x][y]);
                        string str = "Да";
                        if (!b)
                            str = "Нет";
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(str);
                        cell.CellStyle = SimpleCS;
                        continue;
                    }
                    if (t.Name == "DateTime")
                    {
                        string dateTime = Convert.ToDateTime(samplesFrontsDt.Rows[x][y]).ToShortDateString();
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(dateTime);
                        cell.CellStyle = SimpleCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(pos).CreateCell(y);
                        cell.SetCellValue(samplesFrontsDt.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCS;
                        continue;
                    }
                }
                pos++;
            }
        }
    }


    public class SampleOrders
    {
        private readonly DataTable _decorDataTable;
        private DataTable _frameColorsDataTable;
        private readonly DataTable _frontsDataTable;
        private DataTable _insetColorsDataTable;
        private readonly DataTable _insetTypesDataTable;
        private DataTable _patinaDataTable;
        private DataTable _patinaRalDataTable;
        private readonly DataTable _productsDataTable;

        public SampleOrders()
        {
            var selectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            _frontsDataTable = new DataTable();
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_frontsDataTable);
            }

            _insetTypesDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_insetTypesDataTable);
            }

            GetColorsDt();
            GetPatinaDt();
            GetInsetColorsDt();

            selectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                            " WHERE ProductID IN (SELECT ProductID FROM DecorConfig ) ORDER BY ProductName ASC";
            _productsDataTable = new DataTable();
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_productsDataTable);
            }

            _decorDataTable = new DataTable();
            selectCommand =
                @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_decorDataTable);
            }
        }

        private void GetColorsDt()
        {
            _frameColorsDataTable = new DataTable();
            _frameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            _frameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            var selectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (var dt = new DataTable())
                {
                    da.Fill(dt);
                    {
                        var newRow = _frameColorsDataTable.NewRow();
                        newRow["ColorID"] = -1;
                        newRow["ColorName"] = "-";
                        _frameColorsDataTable.Rows.Add(newRow);
                    }
                    {
                        var newRow = _frameColorsDataTable.NewRow();
                        newRow["ColorID"] = 0;
                        newRow["ColorName"] = "на выбор";
                        _frameColorsDataTable.Rows.Add(newRow);
                    }
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var newRow = _frameColorsDataTable.NewRow();
                        newRow["ColorID"] = Convert.ToInt64(dt.Rows[i]["TechStoreID"]);
                        newRow["ColorName"] = dt.Rows[i]["TechStoreName"].ToString();
                        _frameColorsDataTable.Rows.Add(newRow);
                    }
                }
            }
        }

        private void GetInsetColorsDt()
        {
            _insetColorsDataTable = new DataTable();
            using (var da = new SqlDataAdapter(
                "SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_insetColorsDataTable);
                {
                    var newRow = _insetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = -1;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "-";
                    _insetColorsDataTable.Rows.Add(newRow);
                }
                {
                    var newRow = _insetColorsDataTable.NewRow();
                    newRow["InsetColorID"] = 0;
                    newRow["GroupID"] = -1;
                    newRow["InsetColorName"] = "на выбор";
                    _insetColorsDataTable.Rows.Add(newRow);
                }
            }
        }

        private void GetPatinaDt()
        {
            _patinaDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_patinaDataTable);
            }

            _patinaRalDataTable = new DataTable();
            using (var da = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_patinaRalDataTable);
            }

            foreach (DataRow item in _patinaRalDataTable.Rows)
            {
                var newRow = _patinaDataTable.NewRow();
                newRow["PatinaID"] = item["PatinaRALID"];
                newRow["PatinaName"] = item["PatinaRAL"];
                newRow["DisplayName"] = item["DisplayName"];
                _patinaDataTable.Rows.Add(newRow);
            }
        }

        public DataTable F1(int clientId)
        {
            var dt = new DataTable();
            var selectCommand =
                @"SELECT MegaOrders.OrderNumber, SampleMainOrders.MegaOrderID, SampleMainOrders.MainOrderID, SampleMainOrders.DocDateTime FROM SampleMainOrders INNER JOIN MegaOrders ON SampleMainOrders.MegaOrderID=MegaOrders.MegaOrderID AND ClientID=" +
                clientId;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable F2(int mainOrderId)
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("FrontName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("TechnoColorName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("PatinaName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("InsetTypeName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("InsetColorName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("TechnoInsetTypeName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("TechnoInsetColorName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("IsSample", Type.GetType("System.Boolean")));
            using (var da = new SqlDataAdapter("SELECT * FROM SampleFrontsOrders WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var dt1 = new DataTable())
                {
                    if (da.Fill(dt1) > 0)
                        for (var i = 0; i < dt1.Rows.Count; i++)
                        {
                            var frontId = Convert.ToInt32(dt1.Rows[i]["FrontID"]);
                            var colorId = Convert.ToInt32(dt1.Rows[i]["ColorID"]);
                            var patinaId = Convert.ToInt32(dt1.Rows[i]["PatinaID"]);
                            var insetTypeId = Convert.ToInt32(dt1.Rows[i]["InsetTypeID"]);
                            var insetColorId = Convert.ToInt32(dt1.Rows[i]["InsetColorID"]);
                            var technoColorId = Convert.ToInt32(dt1.Rows[i]["TechnoColorID"]);
                            var technoInsetTypeId = Convert.ToInt32(dt1.Rows[i]["TechnoInsetTypeID"]);
                            var technoInsetColorId = Convert.ToInt32(dt1.Rows[i]["TechnoInsetColorID"]);
                            var height = Convert.ToInt32(dt1.Rows[i]["Height"]);
                            var width = Convert.ToInt32(dt1.Rows[i]["Width"]);
                            var count = Convert.ToInt32(dt1.Rows[i]["Count"]);
                            var isSample = Convert.ToBoolean(dt1.Rows[i]["IsSample"]);

                            var newRow = dt.NewRow();
                            newRow["FrontName"] = GetFrontName(frontId);
                            newRow["ColorName"] = GetColorName(colorId);
                            newRow["TechnoColorName"] = GetColorName(technoColorId);
                            newRow["PatinaName"] = GetPatinaName(patinaId);
                            newRow["InsetTypeName"] = GetInsetTypeName(insetTypeId);
                            newRow["InsetColorName"] = GetInsetColorName(insetColorId);
                            newRow["TechnoInsetTypeName"] = GetInsetTypeName(technoInsetTypeId);
                            newRow["TechnoInsetColorName"] = GetInsetColorName(technoInsetColorId);
                            newRow["Height"] = height;
                            newRow["Width"] = width;
                            newRow["Count"] = count;
                            newRow["IsSample"] = isSample;
                            dt.Rows.Add(newRow);
                        }
                }
            }

            return dt;
        }

        public DataTable F3(int mainOrderId)
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("FrontName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("DecorName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("PatinaName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("InsetTypeName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("InsetColorName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("IsSample", Type.GetType("System.Boolean")));
            using (var da = new SqlDataAdapter("SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + mainOrderId,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (var dt1 = new DataTable())
                {
                    if (da.Fill(dt1) > 0)
                        for (var i = 0; i < dt1.Rows.Count; i++)
                        {
                            var productId = Convert.ToInt32(dt1.Rows[i]["ProductID"]);
                            var decorId = Convert.ToInt32(dt1.Rows[i]["DecorID"]);
                            var colorId = Convert.ToInt32(dt1.Rows[i]["ColorID"]);
                            var patinaId = Convert.ToInt32(dt1.Rows[i]["PatinaID"]);
                            var insetTypeId = Convert.ToInt32(dt1.Rows[i]["InsetTypeID"]);
                            var insetColorId = Convert.ToInt32(dt1.Rows[i]["InsetColorID"]);
                            var length = Convert.ToInt32(dt1.Rows[i]["Length"]);
                            var height = Convert.ToInt32(dt1.Rows[i]["Height"]);
                            var width = Convert.ToInt32(dt1.Rows[i]["Width"]);
                            var count = Convert.ToInt32(dt1.Rows[i]["Count"]);
                            var isSample = Convert.ToBoolean(dt1.Rows[i]["IsSample"]);

                            var newRow = dt.NewRow();
                            newRow["FrontName"] = GetProductName(productId);
                            newRow["ColorName"] = GetColorName(colorId);
                            newRow["DecorName"] = GetDecorName(decorId);
                            newRow["PatinaName"] = GetPatinaName(patinaId);
                            newRow["InsetTypeName"] = GetInsetTypeName(insetTypeId);
                            newRow["InsetColorName"] = GetInsetColorName(insetColorId);
                            newRow["Length"] = length;
                            newRow["Height"] = height;
                            newRow["Width"] = width;
                            newRow["Count"] = count;
                            newRow["IsSample"] = isSample;
                            dt.Rows.Add(newRow);
                        }
                }
            }

            return dt;
        }

        private string GetFrontName(int frontId)
        {
            var name = string.Empty;
            var rows = _frontsDataTable.Select("FrontID = " + frontId);
            if (rows.Length > 0)
                name = rows[0]["FrontName"].ToString();
            return name;
        }

        private string GetColorName(int colorId)
        {
            var name = string.Empty;
            var rows = _frameColorsDataTable.Select("ColorID = " + colorId);
            if (rows.Any())
                name = rows[0]["ColorName"].ToString();
            return name;
        }

        private string GetPatinaName(int patinaId)
        {
            var name = string.Empty;
            var rows = _patinaDataTable.Select("PatinaID = " + patinaId);
            if (rows.Count() > 0)
                name = rows[0]["PatinaName"].ToString();
            return name;
        }

        private string GetInsetColorName(int insetColorId)
        {
            var name = string.Empty;
            var rows = _insetColorsDataTable.Select("InsetColorID = " + insetColorId);
            if (rows.Count() > 0)
                name = rows[0]["InsetColorName"].ToString();
            return name;
        }

        private string GetInsetTypeName(int insetTypeId)
        {
            var name = string.Empty;
            var rows = _insetTypesDataTable.Select("InsetTypeID = " + insetTypeId);
            if (rows.Count() > 0)
                name = rows[0]["InsetTypeName"].ToString();
            return name;
        }

        public string GetProductName(int productId)
        {
            var name = string.Empty;
            var rows = _productsDataTable.Select("ProductID = " + productId);
            if (rows.Count() > 0)
                name = rows[0]["ProductName"].ToString();
            return name;
        }

        public string GetDecorName(int decorId)
        {
            var name = string.Empty;
            var rows = _decorDataTable.Select("DecorID = " + decorId);
            if (rows.Count() > 0)
                name = rows[0]["Name"].ToString();
            return name;
        }
    }
}