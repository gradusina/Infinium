using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Collections;
using System.Globalization;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel.Contrib;
using System.Net.Mail;
using System.Net.Mime;
using System.Xml;
using System.Data.OleDb;

namespace Infinium.Modules.Marketing.Orders
{
    public class FrontsCatalogOrder
    {
        ComponentFactory.Krypton.Toolkit.KryptonComboBox HeightEdit;
        ComponentFactory.Krypton.Toolkit.KryptonComboBox WidthEdit;

        private DataTable TempFrontsConfigDataTable = null;
        private DataTable TempFrontsDataTable = null;

        public DataTable ConstFrontsConfigDataTable = null;
        public DataTable ConstFrontsDataTable = null;
        public DataTable ConstColorsDataTable = null;
        public DataTable ConstPatinaDataTable = null;
        public DataTable ConstInsetTypesDataTable = null;
        public DataTable ConstInsetColorsDataTable = null;

        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        public DataTable TechnoFrameColorsDataTable = null;
        public DataTable TechnoInsetTypesDataTable = null;
        public DataTable TechnoInsetColorsDataTable = null;

        public DataTable HeightDataTable = null;
        public DataTable WidthDataTable = null;

        public BindingSource FrontsBindingSource = null;
        public BindingSource FrameColorsBindingSource = null;
        public BindingSource PatinaBindingSource = null;
        public BindingSource InsetTypesBindingSource = null;
        public BindingSource InsetColorsBindingSource = null;
        public BindingSource TechnoFrameColorsBindingSource = null;
        public BindingSource TechnoInsetTypesBindingSource = null;
        public BindingSource TechnoInsetColorsBindingSource = null;
        public BindingSource FrontsConfigBindingSource = null;

        public String FrontsBindingSourceDisplayMember = null;
        public String FrameColorsBindingSourceDisplayMember = null;
        public String PatinaBindingSourceDisplayMember = null;
        public String InsetColorsBindingSourceDisplayMember = null;
        public String InsetTypesBindingSourceDisplayMember = null;

        public String FrontsBindingSourceValueMember = null;
        public String FrameColorsBindingSourceValueMember = null;
        public String PatinaBindingSourceValueMember = null;
        public String InsetColorsBindingSourceValueMember = null;
        public String InsetTypesBindingSourceValueMember = null;

        public FrontsCatalogOrder(
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tHeightEdit,
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tWidthEdit)
        {
            HeightEdit = tHeightEdit;
            WidthEdit = tWidthEdit;
        }

        private void Create()
        {
            ConstFrontsConfigDataTable = new DataTable();
            ConstFrontsDataTable = new DataTable();
            ConstColorsDataTable = new DataTable();
            ConstPatinaDataTable = new DataTable();
            ConstInsetTypesDataTable = new DataTable();
            ConstInsetColorsDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoFrameColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();
            HeightDataTable = new DataTable();
            WidthDataTable = new DataTable();

            TempFrontsConfigDataTable = new DataTable();
            TempFrontsDataTable = new DataTable();

            ConstPatinaDataTable = new DataTable();
            PatinaDataTable = new DataTable();

            HeightDataTable = new DataTable();
            WidthDataTable = new DataTable();

            FrontsConfigBindingSource = new BindingSource();
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
            ConstInsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstInsetColorsDataTable);
                {
                    DataRow NewRow = ConstInsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    ConstInsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = ConstInsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    ConstInsetColorsDataTable.Rows.Add(NewRow);
                }

            }

        }

        private void Fill()
        {
            string SelectCommand = @"SELECT * FROM FrontsConfig" +
                " WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(ConstFrontsConfigDataTable);
            //    ConstFrontsConfigDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            //}
            ConstFrontsConfigDataTable = TablesManager.FrontsConfigDataTableAll;
            ConstFrontsConfigDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            SelectCommand = @"SELECT DISTINCT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstFrontsDataTable);
                ConstFrontsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }

            GetColorsDT();
            GetInsetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstInsetTypesDataTable);
            }
            InsetTypesDataTable = ConstInsetTypesDataTable.Copy();
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
            using (DataView DV = new DataView(ConstFrontsDataTable))
            {
                FrontsDataTable = DV.ToTable(true, new string[] { "FrontName" });
            }

            TempFrontsConfigDataTable = ConstFrontsConfigDataTable.Copy();
            TempFrontsDataTable = ConstFrontsDataTable.Copy();
            //FrontsDataTable = ConstFrontsDataTable.Copy();
            FrameColorsDataTable = ConstColorsDataTable.Copy();
            PatinaDataTable = ConstPatinaDataTable.Copy();
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

        private void Binding()
        {
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrameColorsBindingSource.DataSource = FrameColorsDataTable;
            PatinaBindingSource.DataSource = PatinaDataTable;
            InsetTypesBindingSource.DataSource = InsetTypesDataTable;
            InsetColorsBindingSource.DataSource = InsetColorsDataTable;
            TechnoFrameColorsBindingSource.DataSource = TechnoFrameColorsDataTable;
            TechnoInsetTypesBindingSource.DataSource = TechnoInsetTypesDataTable;
            TechnoInsetColorsBindingSource.DataSource = TechnoInsetColorsDataTable;
            FrontsConfigBindingSource.DataSource = ConstFrontsConfigDataTable;
            //HeightBindingSource.DataSource = HeightDataTable;
            //WidthBindingSource.DataSource = WidthDataTable;

            FrontsBindingSourceDisplayMember = "FrontName";
            FrameColorsBindingSourceDisplayMember = "ColorName";
            PatinaBindingSourceDisplayMember = "PatinaName";
            InsetColorsBindingSourceDisplayMember = "InsetColorName";
            InsetTypesBindingSourceDisplayMember = "InsetTypeName";

            FrontsBindingSourceValueMember = "FrontName";
            FrameColorsBindingSourceValueMember = "ColorID";
            PatinaBindingSourceValueMember = "PatinaID";
            InsetColorsBindingSourceValueMember = "InsetColorID";
            InsetTypesBindingSourceValueMember = "InsetTypeID";
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

            FrameColorsDataTable.DefaultView.Sort = "ColorName ASC";
            FrameColorsBindingSource.MoveFirst();
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
            //if (TechnoInsetColorsDataTable.Rows.Count == 0)
            //{
            //    DataRow NewRow = TechnoInsetColorsDataTable.NewRow();
            //    NewRow["InsetColorID"] = "-1";
            //    NewRow["InsetColorName"] = "-";
            //    TechnoInsetColorsDataTable.Rows.Add(NewRow);
            //}
            //else
            TechnoInsetColorsDataTable.DefaultView.Sort = "InsetColorName ASC";
            TechnoInsetColorsBindingSource.MoveFirst();
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
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]));
                if (fRows.Count() > 0)
                {
                    HasPatinaRAL = true;
                    break;
                }
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

            PatinaBindingSource.MoveFirst();
        }

        public void GetHeight()
        {
            HeightDataTable.Clear();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                HeightDataTable = DV.ToTable(true, new string[] { "Height" });
            }

            HeightDataTable.DefaultView.Sort = "Height ASC";
        }

        public void GetWidth()
        {
            WidthDataTable.Clear();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                WidthDataTable = DV.ToTable(true, new string[] { "Width" });
            }

            WidthDataTable.DefaultView.Sort = "Width ASC";
        }


        public void Initialize()
        {
            Create();
            Fill();
            Binding();
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

        public void FilterFronts(bool bExcluzive)
        {
            FrontsDataTable.Clear();
            FrameColorsDataTable.Clear();
            PatinaDataTable.Clear();
            InsetTypesDataTable.Clear();
            InsetColorsDataTable.Clear();
            TechnoFrameColorsDataTable.Clear();
            TechnoInsetTypesDataTable.Clear();
            TechnoInsetColorsDataTable.Clear();
            HeightDataTable.Clear();
            WidthDataTable.Clear();
            DataTable TempItemsDataTable = ConstFrontsDataTable.Copy();
            string RowFilter = string.Empty;
            using (DataView DV = new DataView(TempItemsDataTable))
            {
                if (bExcluzive)
                    RowFilter += " (Excluzive=1)";
                else
                    RowFilter += " (Excluzive IS NULL OR Excluzive=1)";
                DV.RowFilter = RowFilter;
                TempItemsDataTable = DV.ToTable(true, new string[] { "FrontName" });
            }

            for (int d = 0; d < TempItemsDataTable.Rows.Count; d++)
            {
                DataRow NewRow = FrontsDataTable.NewRow();
                NewRow["FrontName"] = TempItemsDataTable.Rows[d]["FrontName"].ToString();
                FrontsDataTable.Rows.Add(NewRow);
            }
            FrontsDataTable.DefaultView.Sort = "FrontName ASC";
            TempItemsDataTable.Dispose();
        }

        public void FilterCatalogFrameColors(string FrontName, bool bExcluzive)
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
            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;

                TempFrontsConfigDataTable = DV.ToTable();

                GetFrameColors();
            }
        }//фильтрует и заполняет цвета профиля по выбранному фасаду

        public void FilterCatalogTechnoFrameColors(string FrontName, int ColorID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoFrameColors();
            }
        }

        public void FilterCatalogPatina(string FrontName, int ColorID, int TechnoColorID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetPatina();
            }
        }

        public void FilterCatalogInsetTypes(string FrontName, int ColorID, int TechnoColorID, int PatinaID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetInsetTypes();
            }
        }

        public void FilterCatalogInsetColors(string FrontName, int ColorID, int TechnoColorID, int PatinaID, int InsetTypeID, bool bExcluzive) //фильтрует и заполняет цвета наполнителя по выбранному типу наполнителя
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
            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetInsetColors();
            }
        }

        public void FilterCatalogTechnoInsetTypes(string FrontName, int ColorID, int TechnoColorID, int PatinaID, int InsetTypeID, int InsetColorID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoInsetTypes();
            }
        }

        public void FilterCatalogTechnoInsetColors(string FrontName, int ColorID, int TechnoColorID, int PatinaID, int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, bool bExcluzive) //фильтрует и заполняет цвета наполнителя по выбранному типу наполнителя
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
            if (bExcluzive)
                filter += " AND (Excluzive=1)";
            else
                filter += " AND (Excluzive IS NULL OR Excluzive=1)";

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                DV.RowFilter = filter;
                DV.RowFilter += " AND ColorID=" + ColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoInsetColors();
            }
        }

        public void FilterCatalogHeight(string FrontName, int ColorID, int TechnoColorID, int PatinaID,
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

            HeightEdit.Items.Clear();
            HeightEdit.Text = "";

            GetHeight();

            if (HeightDataTable.Rows.Count > 0 && HeightDataTable.Rows[0]["Height"].ToString() != "0" && HeightDataTable.Rows[0]["Height"].ToString() != "-1")
            {
                HeightEdit.DropDownStyle = ComboBoxStyle.DropDownList;

                foreach (DataRow Row in HeightDataTable.Rows)
                    HeightEdit.Items.Add(Row["Height"].ToString());

                HeightEdit.SelectedIndex = 0;
            }
            else
            {
                HeightEdit.DropDownStyle = ComboBoxStyle.DropDown;
            }
        }

        public void FilterCatalogWidth(string FrontName, int ColorID, int TechnoColorID, int PatinaID,
            int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int Height)
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

            WidthEdit.Items.Clear();
            WidthEdit.Text = "";

            GetWidth();

            if (WidthDataTable.Rows.Count > 0 && WidthDataTable.Rows[0]["Width"].ToString() != "0" && WidthDataTable.Rows[0]["Width"].ToString() != "-1")
            {
                WidthEdit.Enabled = true;
                WidthEdit.DropDownStyle = ComboBoxStyle.DropDownList;

                foreach (DataRow Row in WidthDataTable.Rows)
                    WidthEdit.Items.Add(Row["Width"].ToString());

                WidthEdit.SelectedIndex = 0;
            }
            else
            {
                if (HeightEdit.Items.Count > 0)
                {
                    WidthEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    WidthEdit.Text = "-1";
                    WidthEdit.Enabled = false;
                }
                else
                {
                    WidthEdit.DropDownStyle = ComboBoxStyle.DropDown;
                    WidthEdit.Enabled = true;
                }
            }
        }

        public int GetFrontConfigID(int FrontID, int ColorID, int PatinaID,
            int InsetTypeID, int InsetColorID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID,
            int Height, int Width, ref int FactoryID, ref int AreaID)
        {
            string HeightFilter = null;
            string WidthFilter = null;

            if (PatinaID > 1000)
            {
                DataRow[] fRows = PatinaRALDataTable.Select("PatinaRALID=" + PatinaID);
                if (fRows.Count() > 0)
                    PatinaID = Convert.ToInt32(fRows[0]["PatinaID"]);
            }
            DataRow[] Rows = ConstFrontsConfigDataTable.Select(
                            "FrontID = " + Convert.ToInt32(FrontID) + " AND " +
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
                "FrontID = " + Convert.ToInt32(FrontID) + " AND " +
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
                MessageBox.Show("Ошибка конфигурации фасада");
                return -1;
            }

            FactoryID = Convert.ToInt32(Rows[0]["FactoryID"]);
            AreaID = Convert.ToInt32(Rows[0]["AreaID"]);

            if (Rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(Rows[0]["Excluzive"]) == 0)
            {
                MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
                return -1;
            }
            return Convert.ToInt32(Rows[0]["FrontConfigID"]);
        }

        public int GetFrontConfigID(string FrontName, int ColorID, int PatinaID,
            int InsetTypeID, int InsetColorID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID,
            int Height, int Width, ref int FrontID, ref int FactoryID, ref int AreaID)
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

            if (Rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(Rows[0]["Excluzive"]) == 0)
            {
                MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
                return -1;
            }
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(ConstFrontsConfigDataTable,
                filter + " AND(Excluzive IS NULL OR Excluzive=1) " + " AND " +
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
            AreaID = Convert.ToInt32(Rows[0]["AreaID"]);

            return Convert.ToInt32(Rows[0]["FrontConfigID"]);
        }
    }

    public class DecorCatalogOrder
    {
        ComponentFactory.Krypton.Toolkit.KryptonComboBox LengthEdit;
        ComponentFactory.Krypton.Toolkit.KryptonComboBox HeightEdit;
        ComponentFactory.Krypton.Toolkit.KryptonComboBox WidthEdit;

        public int DecorProductsCount = 0;

        public DataTable ItemsDataTable = null;
        public DataTable ColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable PatinaRALDataTable = null;
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
            //SelectionCommand = "SELECT * FROM FrontsOrders";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            //{
            //    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
            //    {
            //        FrontsOrdersDT.Clear();
            //        DA.Fill(FrontsOrdersDT);
            //        for (int i = 0; i < FrontsOrdersDT.Rows.Count; i++)
            //        {
            //            int FrontConfigID = Convert.ToInt32(FrontsOrdersDT.Rows[i]["FrontConfigID"]);
            //            DataRow[] ConfigRows = FrontsConfigDT.Select("FrontConfigID=" + FrontConfigID);
            //            if (ConfigRows.Count() > 0)
            //            {
            //                int FrontID = Convert.ToInt32(ConfigRows[0]["FrontID"]);
            //                int ColorID = Convert.ToInt32(ConfigRows[0]["ColorID"]);
            //                int InsetTypeID = Convert.ToInt32(ConfigRows[0]["InsetTypeID"]);
            //                int InsetColorID = Convert.ToInt32(ConfigRows[0]["InsetColorID"]);
            //                int TechnoColorID = Convert.ToInt32(ConfigRows[0]["TechnoColorID"]);
            //                int TechnoInsetTypeID = Convert.ToInt32(ConfigRows[0]["TechnoInsetTypeID"]);
            //                int TechnoInsetColorID = Convert.ToInt32(ConfigRows[0]["TechnoInsetColorID"]);
            //                int PatinaID = Convert.ToInt32(ConfigRows[0]["PatinaID"]);
            //                FrontsOrdersDT.Rows[i]["FrontID"] = FrontID;
            //                FrontsOrdersDT.Rows[i]["ColorID"] = ColorID;
            //                FrontsOrdersDT.Rows[i]["InsetTypeID"] = InsetTypeID;
            //                FrontsOrdersDT.Rows[i]["InsetColorID"] = InsetColorID;
            //                FrontsOrdersDT.Rows[i]["TechnoColorID"] = TechnoColorID;
            //                FrontsOrdersDT.Rows[i]["TechnoInsetTypeID"] = TechnoInsetTypeID;
            //                FrontsOrdersDT.Rows[i]["TechnoInsetColorID"] = TechnoInsetColorID;
            //                FrontsOrdersDT.Rows[i]["PatinaID"] = PatinaID;
            //            }
            //        }
            //        DA.Update(FrontsOrdersDT);
            //    }
            //}
            //SelectionCommand = "SELECT * FROM DecorConfig";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDT);
            //}
            //SelectionCommand = "SELECT * FROM DecorOrders";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
            //    {
            //        DA.Fill(DecorOrdersDT);
            //        for (int i = 0; i < DecorOrdersDT.Rows.Count; i++)
            //        {
            //            int DecorConfigID = Convert.ToInt32(DecorOrdersDT.Rows[i]["DecorConfigID"]);
            //            DataRow[] ConfigRows = DecorConfigDT.Select("DecorConfigID=" + DecorConfigID);
            //            if (ConfigRows.Count() > 0)
            //            {
            //                int DecorID = Convert.ToInt32(ConfigRows[0]["DecorID"]);
            //                int ColorID = Convert.ToInt32(ConfigRows[0]["ColorID"]);
            //                int PatinaID = Convert.ToInt32(ConfigRows[0]["PatinaID"]);
            //                DecorOrdersDT.Rows[i]["DecorID"] = DecorID;
            //                DecorOrdersDT.Rows[i]["ColorID"] = ColorID;
            //                DecorOrdersDT.Rows[i]["PatinaID"] = PatinaID;
            //            }
            //        }
            //        DA.Update(DecorOrdersDT);
            //    }
            //}
            //SelectionCommand = "SELECT * FROM DecorOrders";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            //{
            //    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
            //    {
            //        DecorOrdersDT.Clear();
            //        DA.Fill(DecorOrdersDT);
            //        for (int i = 0; i < DecorOrdersDT.Rows.Count; i++)
            //        {
            //            int DecorConfigID = Convert.ToInt32(DecorOrdersDT.Rows[i]["DecorConfigID"]);
            //            DataRow[] ConfigRows = DecorConfigDT.Select("DecorConfigID=" + DecorConfigID);
            //            if (ConfigRows.Count() > 0)
            //            {
            //                int DecorID = Convert.ToInt32(ConfigRows[0]["DecorID"]);
            //                int ColorID = Convert.ToInt32(ConfigRows[0]["ColorID"]);
            //                int PatinaID = Convert.ToInt32(ConfigRows[0]["PatinaID"]);
            //                DecorOrdersDT.Rows[i]["DecorID"] = DecorID;
            //                DecorOrdersDT.Rows[i]["ColorID"] = ColorID;
            //                DecorOrdersDT.Rows[i]["PatinaID"] = PatinaID;
            //            }
            //        }
            //        DA.Update(DecorOrdersDT);
            //    }
            //}
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
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE (AccountingName IS NOT NULL AND InvNumber IS NOT NULL)) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
                DecorProductsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL ORDER BY TechStoreName";
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
                " WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL";
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

        public int GetDecorConfigID(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, ref int FactoryID, ref int AreaID)
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
            AreaID = Convert.ToInt32(Rows[0]["AreaID"]);

            if (Rows[0]["Excluzive"] != DBNull.Value && Convert.ToInt32(Rows[0]["Excluzive"]) == 0)
            {
                MessageBox.Show("Конфигурация является эксклюзивом и недоступна другим клиентам");
                return -1;
            }
            return Convert.ToInt32(Rows[0]["DecorConfigID"]);
        }

        public int GetDecorConfigID(int ProductID, string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, ref int DecorID, ref int FactoryID, ref int AreaID)
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
            AreaID = Convert.ToInt32(Rows[0]["AreaID"]);

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

        public string PatinaDisplayName(int PatinaID)
        {
            DataRow[] rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
                return rows[0]["DisplayName"].ToString();
            return string.Empty;
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
                for (int i = 0; i < ItemPatinaDataTable.Rows.Count; i++)
                {
                    DataRow[] fRows = PatinaRALDataTable.Select("PatinaID=" + Convert.ToInt32(ItemPatinaDataTable.Rows[i]["PatinaID"]));
                    if (fRows.Count() > 0)
                    {
                        HasPatinaRAL = true;
                        break;
                    }
                }
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

        public bool FilterInsetType(string Name, int ColorID, int PatinaID, bool bExcluzive)
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

        public bool FilterInsetColor(string Name, int ColorID, int PatinaID, int InsetTypeID, bool bExcluzive)
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

        public int FilterLength(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID)
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
                + " AND PatinaID = " + PatinaID.ToString() + " AND InsetTypeID = " + InsetTypeID.ToString() + " AND InsetColorID = " + InsetColorID.ToString());


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

        public int FilterHeight(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length)
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
                + " AND InsetTypeID = " + InsetTypeID.ToString() + " AND InsetColorID = " + InsetColorID.ToString()
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

        public int FilterWidth(string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height)
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

    public class FrontsOrders
    {
        public bool HasExcluzive = false;
        //int ClientID = 0;
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        public FrontsCatalogOrder FrontsCatalogOrder = null;

        public DataTable ExcluziveDataTable = null;
        public DataTable FrontsOrdersDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        public DataTable TechnoInsetTypesDataTable = null;
        public DataTable TechnoInsetColorsDataTable = null;
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

        private SqlDataAdapter FrontsOrdersDataAdapter = null;
        private SqlCommandBuilder FrontsOrdersCommandBuider = null;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public FrontsOrders(ref PercentageDataGrid tFrontsOrdersDataGrid,
                            ref FrontsCatalogOrder tFrontsCatalogOrder)
        {
            FrontsOrdersDataGrid = tFrontsOrdersDataGrid;
            FrontsCatalogOrder = tFrontsCatalogOrder;
        }

        private void Create()
        {
            FrontsOrdersDataTable = new DataTable();

            FrontsOrdersBindingSource = new BindingSource();
            FrontsBindingSource = new BindingSource();
            FrameColorsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            InsetTypesBindingSource = new BindingSource();
            InsetColorsBindingSource = new BindingSource();
            TechnoFrameColorsBindingSource = new BindingSource();
            TechnoInsetTypesBindingSource = new BindingSource();
            TechnoInsetColorsBindingSource = new BindingSource();

            FrontsOrdersDataAdapter = new SqlDataAdapter();
            FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
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
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            FrontsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
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
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.MarketingOrdersConnectionString);
            FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
            FrontsOrdersDataAdapter.Fill(FrontsOrdersDataTable);
            for (int i = 0; i < FrontsCatalogOrder.ConstFrontsConfigDataTable.Rows.Count; i++)
                FrontsCatalogOrder.ConstFrontsConfigDataTable.Rows[i]["Excluzive"] = 0;
            for (int i = 0; i < FrontsCatalogOrder.ConstFrontsDataTable.Rows.Count; i++)
                FrontsCatalogOrder.ConstFrontsDataTable.Rows[i]["Excluzive"] = 0;
        }

        private void Binding()
        {
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            FrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            PatinaBindingSource.DataSource = PatinaDataTable;
            InsetTypesBindingSource.DataSource = InsetTypesDataTable;
            InsetColorsBindingSource.DataSource = InsetColorsDataTable;
            TechnoFrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            TechnoInsetTypesBindingSource.DataSource = TechnoInsetTypesDataTable;
            TechnoInsetColorsBindingSource.DataSource = TechnoInsetColorsDataTable;

            FrontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
        }

        private void CreateColumns()
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
            FrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            //if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
            //    FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
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
            FrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            //FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
            //FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
            //FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            //FrontsOrdersDataGrid.Columns["Square"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Visible = false;
            FrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            FrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            FrontsOrdersDataGrid.Columns["TotalDiscount"].Visible = false;
            FrontsOrdersDataGrid.Columns["OriginalPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["OriginalCost"].Visible = false;
            FrontsOrdersDataGrid.Columns["CostWithTransport"].Visible = false;
            FrontsOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            FrontsOrdersDataGrid.Columns["CurrencyCost"].Visible = false;
            FrontsOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("PriceWithTransport"))
                FrontsOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CostWithTransport"))
                FrontsOrdersDataGrid.Columns["CostWithTransport"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("OriginalInsetPrice"))
                FrontsOrdersDataGrid.Columns["OriginalInsetPrice"].Visible = false;

            FrontsOrdersDataGrid.Columns["IsNonStandard"].ReadOnly = true;

            //названия столбцов
            FrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
            FrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена\r\nфасада";
            FrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена\r\nнаполнителя";
            FrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            FrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            FrontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["ImpostMargin"].HeaderText = "Смещение\r\nимпоста";

            FrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Height"].Width = 85;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Width"].Width = 85;
            FrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Count"].Width = 65;
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
            FrontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsSample"].Width = 85;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
            FrontsOrdersDataGrid.Columns["ImpostMargin"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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
            FrontsOrdersDataGrid.Columns["CupboardString"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Square"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["FrontPrice"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetPrice"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["ImpostMargin"].DisplayIndex = DisplayIndex++;
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

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
        }

        private int GetLastFrontsOrdersID(int MainOrderID, int FrontID, int ColorID, int PatinaID, int InsetTypeID,
            int InsetColorID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int Height, int Width, int Count, string Notes, int ImpostMargin = 0)
        {
            int FrontsOrdersID = 1;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM NewFrontsOrders ORDER BY FrontsOrdersID DESC",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DataRow row = DT.NewRow();
                            row["MainOrderID"] = -1;
                            row["FrontID"] = FrontID;
                            row["NeedCalcPrice"] = 1;
                            row["InsetTypeID"] = InsetTypeID;
                            row["PatinaID"] = PatinaID;
                            row["ColorID"] = ColorID;
                            row["TechnoColorID"] = TechnoColorID;
                            row["InsetColorID"] = InsetColorID;
                            row["TechnoInsetTypeID"] = TechnoInsetTypeID;
                            row["TechnoInsetColorID"] = TechnoInsetColorID;
                            row["FrontConfigID"] = -1;
                            row["CupboardString"] = "-";
                            row["Height"] = Height;
                            row["Width"] = Width;
                            row["Count"] = Count;
                            row["Notes"] = Notes;
                            row["IsNonStandard"] = false;
                            row["IsSample"] = false;
                            if (ImpostMargin != 0)
                                row["ImpostMargin"] = ImpostMargin;

                            DT.Rows.Add(row);
                            DA.Update(DT);
                            DT.Clear();
                            if (DA.Fill(DT) > 0)
                                FrontsOrdersID = Convert.ToInt32(DT.Rows[0]["FrontsOrdersID"]);
                        }
                    }
                }
            }

            return FrontsOrdersID;
        }

        public void AddFrontsOrder(int MainOrderID, int FrontID, int ColorID, int PatinaID, int InsetTypeID,
            int InsetColorID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int Height, int Width, int Count, string Notes, int ImpostMargin = 0)
        {
            //(3727,3728,3729,3730,3731,3732,3733,3734,3735,3736,3737,3739,3740,3741,3742,3743,3744,3745,3746,3747,3748)
            if (Width != -1 && FrontID == 3630 && Width > 1478)
            {
                MessageBox.Show("Ширина фасада Марсель 3 не может быть больше 1478 мм", "Добавление фасада");
                return;
            }
            if (Width != -1 && (Height < 110 || Width < 110) && (FrontID != 3727 && FrontID != 3728 && FrontID != 3729 &&
                 FrontID != 3730 && FrontID != 3731 && FrontID != 3732 && FrontID != 3733 && FrontID != 3734 &&
                 FrontID != 3735 && FrontID != 3736 && FrontID != 3737 && FrontID != 3739 && FrontID != 3740 &&
                 FrontID != 3741 && FrontID != 3742 && FrontID != 3743 && FrontID != 3744 && FrontID != 3745 &&
                 FrontID != 3746 && FrontID != 3747 && FrontID != 3748 && FrontID != 15108 && FrontID != 3662 && FrontID != 3663 && FrontID != 3664 && FrontID != 15760 &&
                 FrontID != 16269 && FrontID != 28945))
            {
                MessageBox.Show("Высота и ширина фасада не могут быть меньше 110 мм", "Добавление фасада");
                return;
            }
            if (Width != -1 && (Height < 30 || Width < 30) && (FrontID == 3727 || FrontID == 3728 || FrontID == 3729 ||
                 FrontID == 3730 || FrontID == 3731 || FrontID == 3732 || FrontID == 3733 || FrontID == 3734 ||
                 FrontID == 3735 || FrontID == 3736 || FrontID == 3737 || FrontID == 3739 || FrontID == 3740 ||
                 FrontID == 3741 || FrontID == 3742 || FrontID == 3743 || FrontID == 3744 || FrontID == 3745 ||
                 FrontID == 3746 || FrontID == 3747 || FrontID == 3748 || FrontID == 15108 || FrontID == 15760 || FrontID == 16269 || FrontID == 28945 || FrontID == 27914))
            {
                MessageBox.Show("Высота и ширина фасада Женева не могут быть меньше 30 мм", "Добавление фасада");
                return;
            }
            //if (Width != -1 && (Height < 10 || Width < 10) && (FrontID == 3662 || FrontID == 3663 || FrontID == 3664))
            //{
            //    MessageBox.Show("Высота и ширина фасада Тафель не могут быть меньше 10 мм", "Добавление фасада");
            //    return;
            //}
            DataRow row = FrontsOrdersDataTable.NewRow();
            row["FrontsOrdersID"] = GetLastFrontsOrdersID(MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID,
                TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, Notes, ImpostMargin);
            row["MainOrderID"] = MainOrderID;
            row["FrontID"] = FrontID;
            row["InsetTypeID"] = InsetTypeID;
            row["PatinaID"] = PatinaID;
            row["ColorID"] = ColorID;
            row["TechnoColorID"] = TechnoColorID;
            row["InsetColorID"] = InsetColorID;
            row["TechnoInsetTypeID"] = TechnoInsetTypeID;
            row["TechnoInsetColorID"] = TechnoInsetColorID;
            row["NeedCalcPrice"] = 1;
            row["CupboardString"] = "-";
            row["Height"] = Height;
            row["Width"] = Width;
            row["Count"] = Count;
            row["Notes"] = Notes;
            row["IsNonStandard"] = false;
            row["IsSample"] = false;
            if (ImpostMargin != 0)
                row["ImpostMargin"] = ImpostMargin;

            FrontsOrdersDataTable.Rows.Add(row);

            FrontsOrdersBindingSource.MoveLast();
        }

        public void AddFrontsOrderFromSizeTable(ref PercentageDataGrid DataGrid, int MainOrderID,
            string FrontName, int ColorID, int PatinaID, int InsetTypeID,
            int InsetColorID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID)
        {
            if (DataGrid.Rows.Count < 1)
                return;

            int FrontID = 1;
            int Height = 0;
            int Width = 0;

            for (int i = 0; i < DataGrid.Rows.Count - 1; i++)
            {
                int FactoryID = 1;
                int AreaID = 0;
                Height = Convert.ToInt32(DataGrid.Rows[i].Cells["HeightColumn"].Value);
                Width = Convert.ToInt32(DataGrid.Rows[i].Cells["WidthColumn"].Value);
                FrontsCatalogOrder.GetFrontConfigID(FrontName, ColorID, PatinaID, InsetTypeID,
                    InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref FrontID, ref FactoryID, ref AreaID);
                if (FrontID == -1)
                    return;

                if (Width != -1 && FrontID == 3630 && Width > 1478)
                {
                    MessageBox.Show("Ширина фасада Марсель 3 не может быть больше 1478 мм", "Добавление фасада");
                    return;
                }
                if (Width != -1 && (Height < 110 || Width < 110) && (FrontID != 3727 && FrontID != 3728 && FrontID != 3729 &&
                     FrontID != 3730 && FrontID != 3731 && FrontID != 3732 && FrontID != 3733 && FrontID != 3734 &&
                     FrontID != 3735 && FrontID != 3736 && FrontID != 3737 && FrontID != 3739 && FrontID != 3740 &&
                     FrontID != 3741 && FrontID != 3742 && FrontID != 3743 && FrontID != 3744 && FrontID != 3745 &&
                     FrontID != 3746 && FrontID != 3747 && FrontID != 3748 && FrontID != 15108 && FrontID != 3662 && FrontID != 3663 && FrontID != 3664 && FrontID != 15760
                     && FrontID != 16269 && FrontID != 28945))
                {
                    MessageBox.Show("Высота и ширина фасада не могут быть меньше 110 мм", "Добавление фасада");
                    return;
                }
                if (Width != -1 && (Height < 30 || Width < 30) && (FrontID == 3727 || FrontID == 3728 || FrontID == 3729 ||
                     FrontID == 3730 || FrontID == 3731 || FrontID == 3732 || FrontID == 3733 || FrontID == 3734 ||
                     FrontID == 3735 || FrontID == 3736 || FrontID == 3737 || FrontID == 3739 || FrontID == 3740 ||
                     FrontID == 3741 || FrontID == 3742 || FrontID == 3743 || FrontID == 3744 || FrontID == 3745 ||
                     FrontID == 3746 || FrontID == 3747 || FrontID == 3748 || FrontID == 15108 || FrontID == 15760 || FrontID == 16269 || FrontID == 28945 || FrontID == 27914))
                {
                    MessageBox.Show("Высота и ширина фасада Женева не могут быть меньше 30 мм", "Добавление фасада");
                    return;
                }
                //if (Width != -1 && (Height < 10 || Width < 10) && (FrontID == 3662 || FrontID == 3663 || FrontID == 3664))
                //{
                //    MessageBox.Show("Высота и ширина фасада Тафель не могут быть меньше 10 мм", "Добавление фасада");
                //    return;
                //}
            }

            for (int i = 0; i < DataGrid.Rows.Count - 1; i++)
            {
                DataRow row = FrontsOrdersDataTable.NewRow();
                row["MainOrderID"] = MainOrderID;
                row["FrontID"] = FrontID;
                row["NeedCalcPrice"] = 1;
                row["InsetTypeID"] = InsetTypeID;
                row["PatinaID"] = PatinaID;
                row["ColorID"] = ColorID;
                row["TechnoColorID"] = TechnoColorID;
                row["InsetColorID"] = InsetColorID;
                row["TechnoInsetTypeID"] = TechnoInsetTypeID;
                row["TechnoInsetColorID"] = TechnoInsetColorID;
                row["Height"] = Convert.ToInt32(DataGrid.Rows[i].Cells["HeightColumn"].Value);
                row["Width"] = Convert.ToInt32(DataGrid.Rows[i].Cells["WidthColumn"].Value);
                row["Count"] = Convert.ToInt32(DataGrid.Rows[i].Cells["CountColumn"].Value);
                row["IsNonStandard"] = false;
                row["IsSample"] = false;
                row["CupboardString"] = "-";

                FrontsOrdersDataTable.Rows.Add(row);
            }
        }

        private bool SetConfigID(DataRow Row, ref int FactoryID)
        {
            int FrontID = Convert.ToInt32(Row["FrontID"]);
            int PatinaID = Convert.ToInt32(Row["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(Row["InsetTypeID"]);
            int ColorID = Convert.ToInt32(Row["ColorID"]);
            int InsetColorID = Convert.ToInt32(Row["InsetColorID"]);
            int TechnoColorID = Convert.ToInt32(Row["TechnoColorID"]);
            int TechnoInsetTypeID = Convert.ToInt32(Row["TechnoInsetTypeID"]);
            int TechnoInsetColorID = Convert.ToInt32(Row["TechnoInsetColorID"]);
            int Height = Convert.ToInt32(Row["Height"]);
            int Width = Convert.ToInt32(Row["Width"]);

            int F = -1;
            int AreaID = 0;
            Row["FrontConfigID"] = FrontsCatalogOrder.GetFrontConfigID(FrontID, ColorID, PatinaID, InsetTypeID,
                InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref F, ref AreaID);
            Row["FactoryID"] = F;
            Row["AreaID"] = AreaID;

            if (FactoryID == 1)
                if (F == 2)
                    FactoryID = 0;

            if (FactoryID == 2)
                if (F == 1)
                    FactoryID = 0;

            if (FactoryID == -1)
                FactoryID = F;


            if (Row["FrontConfigID"].ToString() == "-1")
                return false;

            return true;
        }

        public bool HasRows()
        {
            int ItemsCount = 0;

            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
                if (FrontsOrdersDataTable.Rows[i].RowState != DataRowState.Deleted)
                    ItemsCount++;

            return ItemsCount > 0;
        }

        public bool SaveFrontsOrder(int MainOrderID, ref int FactoryID)
        {
            if (FrontsOrdersDataTable.Rows.Count < 1)
                return true;

            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                if (FrontsOrdersDataTable.Rows[i].RowState == DataRowState.Deleted)
                    continue;



                if (SetConfigID(FrontsOrdersDataTable.Rows[i], ref FactoryID) == false)
                {
                    MessageBox.Show("Невозможно сохранить заказ, так как одна или несколько позиций фасадов\r\n" +
                                    "отсутствует в каталоге. Проверьте правильность ввода данных в позиции " + (i + 1).ToString(),
                                    "Ошибка сохранения заказа");
                    return false;
                }
                if (Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["MainOrderID"]) == -1)
                {
                    FrontsOrdersDataTable.Rows[i]["MainOrderID"] = MainOrderID;
                }
            }

            FrontsOrdersDataAdapter.Update(FrontsOrdersDataTable);
            //FrontsOrdersDataTable.Clear();
            //FrontsOrdersDataAdapter.Dispose();
            //FrontsOrdersCommandBuider.Dispose();
            //FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID,
            //    ConnectionStrings.MarketingOrdersConnectionString);
            //FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
            //FrontsOrdersDataAdapter.Fill(FrontsOrdersDataTable);

            return true;
        }

        public void RemoveOrder()
        {
            if (FrontsOrdersBindingSource.Current != null)
            {
                int pos = FrontsOrdersBindingSource.Position;

                FrontsOrdersBindingSource.RemoveCurrent();

                //остается на позиции удаленного заказа, а не перемещается в начало
                if (FrontsOrdersBindingSource.Count > 0)
                    if (pos >= FrontsOrdersBindingSource.Count)
                    {
                        FrontsOrdersBindingSource.MoveLast();
                        FrontsOrdersDataGrid.Rows[FrontsOrdersDataGrid.Rows.Count - 1].Selected = true;
                    }
                    else
                        FrontsOrdersBindingSource.Position = pos;
            }
        }

        public void UpdateExcluziveCatalog()
        {
            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["FrontID"]);
                int ColorID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["ColorID"]);
                int InsetTypeID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["InsetTypeID"]);
                int InsetColorID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["InsetColorID"]);
                int PatinaID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["PatinaID"]);
                int FrontConfigID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["FrontConfigID"]);
                DataRow[] rows = FrontsCatalogOrder.ConstFrontsDataTable.Select("FrontID=" + FrontID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstInsetTypesDataTable.Select("InsetTypeID=" + InsetTypeID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstInsetColorsDataTable.Select("InsetColorID=" + InsetColorID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstColorsDataTable.Select("ColorID=" + ColorID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstPatinaDataTable.Select("PatinaID=" + PatinaID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("FrontConfigID=" + FrontConfigID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
            }
        }

        public bool EditOrder(int MainOrderID)
        {
            FrontsOrdersDataTable.Clear();
            FrontsOrdersDataAdapter.Dispose();
            FrontsOrdersCommandBuider.Dispose();

            DataRow row = FrontsOrdersDataTable.NewRow();
            FrontsOrdersDataTable.Rows.Add(row);
            FrontsOrdersDataTable.Rows.Clear();

            FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString);
            FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
            FrontsOrdersDataAdapter.Fill(FrontsOrdersDataTable);
            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["FrontID"]);
                int ColorID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["ColorID"]);
                int InsetTypeID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["InsetTypeID"]);
                int InsetColorID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["InsetColorID"]);
                int PatinaID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["PatinaID"]);
                int FrontConfigID = Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["FrontConfigID"]);
                DataRow[] rows = FrontsCatalogOrder.ConstFrontsDataTable.Select("FrontID=" + FrontID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstInsetTypesDataTable.Select("InsetTypeID=" + InsetTypeID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstInsetColorsDataTable.Select("InsetColorID=" + InsetColorID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstColorsDataTable.Select("ColorID=" + ColorID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = FrontsCatalogOrder.ConstPatinaDataTable.Select("PatinaID=" + PatinaID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("FrontConfigID=" + FrontConfigID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
            }
            if (FrontsOrdersDataTable.Rows.Count > 0)
                return true;

            return false;
        }
    }

    public class DecorOrders
    {
        public bool HasExcluzive = false;
        //int ClientID = 0;
        private DevExpress.XtraTab.XtraTabControl DecorTabControl = null;

        public DecorCatalogOrder DecorCatalogOrder = null;
        public DataTable ExcluziveDataTable = null;

        public DataTable DecorOrdersDataTable = null;
        public DataTable[] DecorItemOrdersDataTables = null;
        public BindingSource[] DecorItemOrdersBindingSources = null;
        public SqlDataAdapter DecorOrdersDataAdapter = null;
        public SqlCommandBuilder DecorOrdersCommandBuilder = null;
        public PercentageDataGrid[] DecorItemOrdersDataGrids = null;

        public PercentageDataGrid MainOrdersFrontsOrdersDataGrid = null;

        //конструктор
        public DecorOrders(ref DevExpress.XtraTab.XtraTabControl tDecorTabControl,
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
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders",
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);

            for (int i = 0; i < DecorCatalogOrder.DecorConfigDataTable.Rows.Count; i++)
                DecorCatalogOrder.DecorConfigDataTable.Rows[i]["Excluzive"] = 0;
            for (int i = 0; i < DecorCatalogOrder.DecorProductsDataTable.Rows.Count; i++)
                DecorCatalogOrder.DecorProductsDataTable.Rows[i]["Excluzive"] = 0;
            for (int i = 0; i < DecorCatalogOrder.DecorDataTable.Rows.Count; i++)
                DecorCatalogOrder.DecorDataTable.Rows[i]["Excluzive"] = 0;
        }

        private void Binding()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();

            SplitDecorOrdersTables();
            GridSettings();
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
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
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
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
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

        private void GridSettings()
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

                DecorItemOrdersDataGrids[i] = new PercentageDataGrid();
                DecorItemOrdersDataGrids[i].CellValueChanged += DecorOrders_CellValueChanged;
                DecorItemOrdersDataGrids[i].Parent = DecorTabControl.TabPages[i];
                DecorItemOrdersDataGrids[i].DataSource = DecorItemOrdersBindingSources[i];
                DecorItemOrdersDataGrids[i].Dock = System.Windows.Forms.DockStyle.Fill;
                DecorItemOrdersDataGrids[i].PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black;
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
                DecorItemOrdersDataGrids[i].ReadOnly = false;
                DecorItemOrdersDataGrids[i].UseCustomBackColor = true;

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
                DecorItemOrdersDataGrids[i].Columns["Price"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorConfigID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Cost"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["FactoryID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ItemWeight"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Weight"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["CurrencyTypeID"].Visible = false;

                //русские названия полей
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
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "LeftAngle")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        DecorItemOrdersDataGrids[i].Columns[j].MinimumWidth = 100;
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "ᵒ∠ л";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "RightAngle")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        DecorItemOrdersDataGrids[i].Columns[j].MinimumWidth = 100;
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "ᵒ∠ пр";
                    }
                }

                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].HeaderText = "Объемный\r\nкоэф-нт";
                DecorItemOrdersDataGrids[i].Columns["IsSample"].HeaderText = "Образцы";
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
                DecorItemOrdersDataGrids[i].Columns["Notes"].DisplayIndex = DisplayIndex++;
            }

            DecorTabControl.Visible = false;
        }

        void DecorOrders_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

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
            {
                DecorTabControl.Visible = true;
                return true;
            }
            else
            {
                DecorTabControl.Visible = false;
                return false;
            }
        }

        private bool SetDecorConfigID(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, DataRow Row, ref int FactoryID)
        {
            int F = 0;
            int AreaID = 0;
            Row["DecorConfigID"] = DecorCatalogOrder.GetDecorConfigID(ProductID, DecorID, ColorID, PatinaID, InsetTypeID, InsetColorID, 
                Length, Height, Width, ref F, ref AreaID);
            Row["FactoryID"] = F;
            Row["AreaID"] = AreaID;

            if (FactoryID == 1)
                if (F == 2)
                    FactoryID = 0;

            if (FactoryID == 2)
                if (F == 1)
                    FactoryID = 0;

            if (FactoryID == -1)
                FactoryID = F;

            if (Row["DecorConfigID"].ToString() == "-1")
                return false;

            return true;
        }

        public void DecorOrdersSet(int MainOrderID, int CurrentMainOrderID)
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DataRow[] DRows = DecorItemOrdersDataTables[i].Select("MainOrderID=" + MainOrderID);
                foreach (DataRow DRow in DRows)
                {
                    DataRow Row = DecorOrdersDataTable.NewRow();

                    Row.ItemArray = DRow.ItemArray;
                    Row["MainOrderID"] = CurrentMainOrderID;

                    DecorOrdersDataTable.Rows.Add(Row);
                }
            }
        }

        private int GetLastDecorOrderID(DateTime CreateDateTime, int MainOrderID, int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, int Count, string Notes)
        {
            int DecorOrderID = 1;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM NewDecorOrders ORDER BY DecorOrderID DESC",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {

                            DataRow NewRow = DT.NewRow();
                            NewRow["CreateDateTime"] = CreateDateTime;
                            NewRow["CreateUserTypeID"] = 0;
                            NewRow["NeedCalcPrice"] = 1;
                            NewRow["CreateUserID"] = Security.CurrentUserID;
                            NewRow["MainOrderID"] = -1;
                            NewRow["ProductID"] = ProductID;
                            NewRow["DecorID"] = DecorID;
                            NewRow["ColorID"] = ColorID;
                            NewRow["PatinaID"] = PatinaID;
                            NewRow["InsetTypeID"] = InsetTypeID;
                            NewRow["InsetColorID"] = InsetColorID;
                            NewRow["DecorConfigID"] = -1;
                            NewRow["Length"] = Length;
                            NewRow["Height"] = Height;
                            NewRow["Width"] = Width;
                            NewRow["IsSample"] = false;

                            NewRow["Count"] = Count;
                            NewRow["Notes"] = Notes;

                            DT.Rows.Add(NewRow);

                            DA.Update(DT);
                            DT.Clear();
                            if (DA.Fill(DT) > 0)
                                DecorOrderID = Convert.ToInt32(DT.Rows[0]["DecorOrderID"]);
                        }
                    }
                }
            }

            return DecorOrderID;
        }

        public void AddDecorOrder(int MainOrderID, int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, int Count, string Notes)
        {
            int index = DecorCatalogOrder.DecorProductsBindingSource.Find("ProductID", ProductID);

            for (int i = 0; i < DecorCatalogOrder.DecorProductsDataTable.Rows.Count; i++)
            {
                if (Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]) == ProductID)
                {
                    index = i;
                    break;
                }
            }
            DateTime CreateDateTime = Security.GetCurrentDate();
            DataRow NewRow = DecorItemOrdersDataTables[index].NewRow();
            NewRow["CreateDateTime"] = CreateDateTime;
            NewRow["CreateUserTypeID"] = 0;
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["DecorOrderID"] = GetLastDecorOrderID(CreateDateTime, MainOrderID, ProductID, DecorID, ColorID, PatinaID, InsetTypeID, InsetColorID,
                Length, Height, Width, Count, Notes);
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["ProductID"] = ProductID;
            NewRow["DecorID"] = DecorID;
            NewRow["ColorID"] = ColorID;
            NewRow["PatinaID"] = PatinaID;
            NewRow["InsetTypeID"] = InsetTypeID;
            NewRow["InsetColorID"] = InsetColorID;
            NewRow["NeedCalcPrice"] = 1;
            NewRow["Length"] = Length;
            NewRow["Height"] = Height;
            NewRow["Width"] = Width;
            NewRow["IsSample"] = false;

            if (ProductID == 42)
                NewRow["IsSample"] = true;
            NewRow["Count"] = Count;
            NewRow["Notes"] = Notes;

            DecorItemOrdersDataTables[index].Rows.Add(NewRow);


            DecorTabControl.TabPages[index].PageVisible = true;
            DecorTabControl.SelectedTabPage = DecorTabControl.TabPages[index];

            DecorItemOrdersDataGrids[index].Columns["DecorOrderID"].Visible = false;
            DecorItemOrdersBindingSources[index].MoveLast();

            int C = 0;

            for (int i = 0; i < DecorTabControl.TabPages.Count; i++)
                C += Convert.ToInt32(DecorTabControl.TabPages[i].PageVisible);

            DecorTabControl.Visible = (C > 0);
        }

        public void AddDecorOrderFromSizeTable(ref PercentageDataGrid DataGrid,
            int MainOrderID, int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, string Notes)
        {
            int index = DecorCatalogOrder.DecorProductsBindingSource.Find("ProductID", ProductID);

            for (int i = 0; i < DecorCatalogOrder.DecorProductsDataTable.Rows.Count; i++)
            {
                if (Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]) == ProductID)
                {
                    index = i;
                    break;
                }
            }
            int Height = 0;
            int Width = 0;
            int Count = 0;

            DateTime CreateDateTime = Security.GetCurrentDate();
            for (int i = 0; i < DataGrid.Rows.Count - 1; i++)
            {
                DataRow NewRow = DecorItemOrdersDataTables[index].NewRow();

                Height = Convert.ToInt32(DataGrid.Rows[i].Cells["HeightColumn"].Value);
                Width = Convert.ToInt32(DataGrid.Rows[i].Cells["WidthColumn"].Value);
                Count = Convert.ToInt32(DataGrid.Rows[i].Cells["CountColumn"].Value);

                NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["CreateUserTypeID"] = 0;
                NewRow["CreateUserID"] = Security.CurrentUserID;
                NewRow["MainOrderID"] = MainOrderID;
                NewRow["ProductID"] = ProductID;
                NewRow["NeedCalcPrice"] = 1;
                NewRow["DecorID"] = DecorID;
                NewRow["ColorID"] = ColorID;
                NewRow["PatinaID"] = PatinaID;
                NewRow["InsetTypeID"] = InsetTypeID;
                NewRow["InsetColorID"] = InsetColorID;

                if (DecorCatalogOrder.HasParameter(ProductID, "Height"))
                    NewRow["Height"] = Height;
                else
                    NewRow["Height"] = -1;

                if (DecorCatalogOrder.HasParameter(ProductID, "Width"))
                    NewRow["Width"] = Width;
                else
                    NewRow["Width"] = -1;

                if (DecorCatalogOrder.HasParameter(ProductID, "Length"))
                    NewRow["Length"] = Height;
                else
                    NewRow["Length"] = -1;

                NewRow["Count"] = Count;
                NewRow["Notes"] = Notes;
                NewRow["IsSample"] = false;

                if (ProductID == 42)
                    NewRow["IsSample"] = true;
                DecorItemOrdersDataTables[index].Rows.Add(NewRow);
            }

            DecorTabControl.TabPages[index].PageVisible = true;
            DecorTabControl.SelectedTabPage = DecorTabControl.TabPages[index];

            DecorItemOrdersDataGrids[index].Columns["DecorOrderID"].Visible = false;
            DecorItemOrdersBindingSources[index].MoveLast();

            int C = 0;

            for (int i = 0; i < DecorTabControl.TabPages.Count; i++)
                C += Convert.ToInt32(DecorTabControl.TabPages[i].PageVisible);

            DecorTabControl.Visible = (C > 0);
        }

        public void UpdateExcluziveCatalog()
        {
            for (int i = 0; i < DecorOrdersDataTable.Rows.Count; i++)
            {
                int ProductID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["ProductID"]);
                int DecorID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["DecorID"]);
                int ColorID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["ColorID"]);
                int PatinaID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["PatinaID"]);
                int DecorConfigID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["DecorConfigID"]);
                DataRow[] rows = DecorCatalogOrder.DecorProductsDataTable.Select("ProductID=" + ProductID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
                rows = DecorCatalogOrder.DecorDataTable.Select("DecorID=" + DecorID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
                //rows = DecorCatalogOrder.ColorsDataTable.Select("ColorID=" + ColorID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = DecorCatalogOrder.PatinaDataTable.Select("PatinaID=" + PatinaID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                rows = DecorCatalogOrder.DecorConfigDataTable.Select("DecorConfigID=" + DecorConfigID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
            }
        }

        public bool EditDecorOrder(int MainOrderID)
        {
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

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID.ToString(),
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            for (int i = 0; i < DecorOrdersDataTable.Rows.Count; i++)
            {
                int ProductID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["ProductID"]);
                int DecorID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["DecorID"]);
                int ColorID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["ColorID"]);
                int PatinaID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["PatinaID"]);
                int DecorConfigID = Convert.ToInt32(DecorOrdersDataTable.Rows[i]["DecorConfigID"]);
                DataRow[] rows = DecorCatalogOrder.DecorProductsDataTable.Select("ProductID=" + ProductID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
                rows = DecorCatalogOrder.DecorDataTable.Select("DecorID=" + DecorID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
                //rows = DecorCatalogOrder.ColorsDataTable.Select("ColorID=" + ColorID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                //rows = DecorCatalogOrder.PatinaDataTable.Select("PatinaID=" + PatinaID);
                //if (rows.Count() > 0)
                //    rows[0]["Excluzive"] = 1;
                rows = DecorCatalogOrder.DecorConfigDataTable.Select("DecorConfigID=" + DecorConfigID);
                if (rows.Count() > 0)
                    rows[0]["Excluzive"] = 1;
            }
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;

                for (int r = 0; r < Rows.Count(); r++)
                {
                    DecorItemOrdersDataTables[i].ImportRow(Rows[r]);
                }

                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
            }

            return ShowTabs();
        }

        public void DeleteCurrentDecorItem(int TabIndex)
        {
            if (DecorItemOrdersBindingSources[TabIndex].Count > 0)
            {
                {
                    int pos = DecorItemOrdersBindingSources[TabIndex].Position;

                    //int DecorOrderID = 0;
                    //if (DecorItemOrdersDataGrids[TabIndex].SelectedRows.Count != 0 && 
                    //    DecorItemOrdersDataGrids[TabIndex].SelectedRows[0].Cells["DecorOrderID"].Value != DBNull.Value)
                    //    DecorOrderID = Convert.ToInt32(DecorItemOrdersDataGrids[TabIndex].SelectedRows[0].Cells["DecorOrderID"].Value);
                    //if (DecorOrderID == 0)
                    DecorItemOrdersBindingSources[TabIndex].RemoveCurrent();

                    //остается на позиции удаленного заказа, а не перемещается в начало
                    if (DecorItemOrdersBindingSources[TabIndex].Count > 0)
                        if (pos >= DecorItemOrdersBindingSources[TabIndex].Count)
                        {
                            DecorItemOrdersBindingSources[TabIndex].MoveLast();
                            DecorItemOrdersDataGrids[TabIndex].Rows[DecorItemOrdersDataGrids[TabIndex].Rows.Count - 1].Selected = true;
                        }
                        else
                            DecorItemOrdersBindingSources[TabIndex].Position = pos;
                }
            }

            if (DecorItemOrdersBindingSources[TabIndex].Count == 0)
                DecorTabControl.TabPages[TabIndex].PageVisible = false;

            int C = 0;

            for (int i = 0; i < DecorTabControl.TabPages.Count; i++)
                C += Convert.ToInt32(DecorTabControl.TabPages[i].PageVisible);

            DecorTabControl.Visible = (C > 0);
        }

        public void DeleteDecorAssignmentByDecorOrders(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"DELETE FROM DecorAssignments 
                WHERE MainOrderID=" + MainOrderID + @" AND DecorOrderID NOT IN (SELECT DecorOrderID FROM infiniu2_marketingorders.dbo.DecorOrders WHERE MainOrderID=" + MainOrderID + ")", ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }

        public bool SaveDecorOrder(int MainOrderID, ref int FactoryID)
        {
            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                foreach (DataRow Row in DecorItemOrdersDataTables[i].Rows)
                {
                    if (Row.RowState != DataRowState.Deleted)
                    {
                        if (SetDecorConfigID(Convert.ToInt32(Row["ProductID"]), Convert.ToInt32(Row["DecorID"]),
                            Convert.ToInt32(Row["ColorID"]), Convert.ToInt32(Row["PatinaID"]),
                            Convert.ToInt32(Row["InsetTypeID"]), Convert.ToInt32(Row["InsetColorID"]),
                            Convert.ToInt32(Row["Length"]), Convert.ToInt32(Row["Height"]),
                            Convert.ToInt32(Row["Width"]), Row, ref FactoryID) == false)
                            return false;

                        if (Convert.ToInt32(Row["MainOrderID"]) == -1)
                        {
                            Row["MainOrderID"] = MainOrderID;
                        }

                        if (Convert.ToInt32(Row["ProductID"]) == 42)
                        {
                            Row["IsSample"] = true;
                        }
                        if (DecorItemOrdersDataTables[i].Columns.Contains("LeftAngle") &&
                            Row["LeftAngle"] != DBNull.Value)
                        {

                            if (Convert.ToDecimal(Row["LeftAngle"]) > 180)
                            {
                                MessageBox.Show("Угол не может быть больше 180ᵒ", "Ошибка сохранения заказа");
                                return false;
                            }
                        }
                        if (DecorItemOrdersDataTables[i].Columns.Contains("RightAngle") &&
                            Row["RightAngle"] != DBNull.Value)
                        {

                            if (Convert.ToDecimal(Row["RightAngle"]) > 180)
                            {
                                MessageBox.Show("Угол не может быть больше 180ᵒ", "Ошибка сохранения заказа");
                                return false;
                            }
                        }
                    }

                    DecorOrdersDataTable.ImportRow(Row);
                }
            }

            DecorOrdersDataAdapter.Update(DecorOrdersDataTable);

            return true;
        }

        public bool SaveDecorOrderSet(int MainOrderID, ref int FactoryID)
        {
            if (DecorOrdersDataTable.Rows.Count < 1)
                return true;

            for (int i = 0; i < DecorOrdersDataTable.Rows.Count; i++)
            {
                if (DecorOrdersDataTable.Rows[i].RowState == DataRowState.Deleted)
                    continue;

                if (SetDecorConfigID(Convert.ToInt32(DecorOrdersDataTable.Rows[i]["ProductID"]), Convert.ToInt32(DecorOrdersDataTable.Rows[i]["DecorID"]),
                    Convert.ToInt32(DecorOrdersDataTable.Rows[i]["ColorID"]), Convert.ToInt32(DecorOrdersDataTable.Rows[i]["PatinaID"]),
                    Convert.ToInt32(DecorOrdersDataTable.Rows[i]["InsetTypeID"]), Convert.ToInt32(DecorOrdersDataTable.Rows[i]["InsetColorID"]),
                    Convert.ToInt32(DecorOrdersDataTable.Rows[i]["Length"]), Convert.ToInt32(DecorOrdersDataTable.Rows[i]["Height"]), Convert.ToInt32(DecorOrdersDataTable.Rows[i]["Width"]),
                    DecorOrdersDataTable.Rows[i], ref FactoryID) == false)
                    return false;

                if (Convert.ToInt32(DecorOrdersDataTable.Rows[i]["MainOrderID"]) == -1)
                {
                    DecorOrdersDataTable.Rows[i]["MainOrderID"] = MainOrderID;
                }

            }

            DecorOrdersDataAdapter.Update(DecorOrdersDataTable);

            return true;
        }
    }

    public class MainOrdersFrontsOrders
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

        public MainOrdersFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
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
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.MarketingOrdersConnectionString))
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
            if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
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
            FrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
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

            FrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public void DeleteOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM FrontsOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }
    }

    public class MainOrdersDecorOrders
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

        public MainOrdersDecorOrders(ref DevExpress.XtraTab.XtraTabControl tDecorTabControl,
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
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.MarketingOrdersConnectionString);
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

                DecorItemOrdersDataGrids[i].Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

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
                DecorItemOrdersDataGrids[i].Columns["NeedCalcPrice"].Visible = false;
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
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "LeftAngle")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        DecorItemOrdersDataGrids[i].Columns[j].MinimumWidth = 100;
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "ᵒ∠ л";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "RightAngle")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        DecorItemOrdersDataGrids[i].Columns[j].MinimumWidth = 100;
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "ᵒ∠ пр";
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

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
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

        public void DeleteOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM DecorOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }

        public void DeleteDecorAssignmentByMainOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM DecorAssignments WHERE MainOrderID = " + MainOrderID, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }
    }

    public class OrdersManager
    {
        //public bool NeedSetStatus = true;
        public bool SendReport = false;

        public int CurrentClientID = -1;
        public int CurrentMainOrderID = -1;
        public int CurrentMegaOrderID = -1;
        public int CurrencyTypeID = 1;
        public int CurrentDiscountPaymentConditionID = 0;
        public int CurrentDiscountFactoringID = 0;
        public decimal CurrentProfilDiscountDirector = 0;
        public decimal CurrentTPSDiscountDirector = 0;
        public decimal CurrentProfilTotalDiscount = 0;
        public decimal CurrentTPSTotalDiscount = 0;
        public decimal PaymentCurrency = 1;
        public object ConfirmDateTime = DBNull.Value;

        public MainOrdersFrontsOrders MainOrdersFrontsOrders = null;
        public MainOrdersDecorOrders MainOrdersDecorOrders = null;

        public PercentageDataGrid MainOrdersDataGrid = null;
        public PercentageDataGrid MegaOrdersDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl;

        public DataTable ClientsDataTable = null;
        public DataTable MainOrdersDataTable = null;
        private DataTable OrderStatusesDataTable = null;
        private DataTable AgreementStatusesDataTable = null;
        public DataTable CurrencyTypesDataTable = null;
        public DataTable ProductionStatusesDataTable = null;
        public DataTable StorageStatusesDataTable = null;
        public DataTable ExpeditionStatusesDataTable = null;
        public DataTable DispatchStatusesDataTable = null;
        public DataTable FactoryTypesDataTable = null;
        public DataTable ClientsMegaOrdersDataTable = null;
        public DataTable MegaOrdersDataTable = null;
        private DataTable DiscountOrderSumTable = null;
        private DataTable TransportGridTable = null;
        private DataTable DiscountPaymentConditionsTable = null;
        private DataTable DiscountFactoringTable = null;
        private DataTable OrdersInMutualSettlementTable = null;

        public BindingSource FilterClientsBindingSource = null;
        public BindingSource MainOrdersBindingSource = null;
        public BindingSource OrderStatusesBindingSource = null;
        public BindingSource AgreementStatusesBindingSource = null;
        public BindingSource CurrencyTypesBindingSource = null;
        public BindingSource ProductionStatusesBindingSource = null;
        public BindingSource StorageStatusesBindingSource = null;
        public BindingSource ExpeditionStatusesBindingSource = null;
        public BindingSource DispatchStatusesBindingSource = null;
        public BindingSource FactoryTypesBindingSource = null;
        public BindingSource MegaOrdersBindingSource = null;
        public BindingSource ClientsMegaOrdersBindingSource = null;

        public String ClientsBindingSourceDisplayMember = null;
        public String CurrencyTypesBindingSourceDisplayMember = null;

        public String ClientsBindingSourceValueMember = null;
        public String CurrencyTypesBindingSourceValueMember = null;

        public String ClientsMegaOrdersBindingSourceValueMember = null;
        public String ClientsMegaOrdersBindingSourceDisplayMember = null;

        private DataGridViewComboBoxColumn OrderStatusColumn = null;
        private DataGridViewComboBoxColumn CurrencyTypeColumn = null;
        private DataGridViewComboBoxColumn ProfilProductionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilStorageStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn TPSProductionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSStorageStatusColumn = null;
        private DataGridViewComboBoxColumn TPSExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn AgreementStatusColumn = null;
        private DataGridViewComboBoxColumn PaymentConditionColumn = null;

        public OrdersManager(ref PercentageDataGrid tMainOrdersDataGrid,
                             ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
                             ref PercentageDataGrid tMegaOrdersDataGrid,
                             ref DevExpress.XtraTab.XtraTabControl tMainOrdersDecorTabControl,
                             ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl,
                             ref DecorCatalogOrder DecorCatalogOrder)
        {
            MainOrdersDataGrid = tMainOrdersDataGrid;
            MegaOrdersDataGrid = tMegaOrdersDataGrid;
            OrdersTabControl = tOrdersTabControl;

            MainOrdersFrontsOrders = new MainOrdersFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid);
            MainOrdersFrontsOrders.Initialize(true);

            MainOrdersDecorOrders = new MainOrdersDecorOrders(ref tMainOrdersDecorTabControl,
                                                              ref DecorCatalogOrder, ref tMainOrdersFrontsOrdersDataGrid);

            MainOrdersDecorOrders.Initialize(true);
            Initialize();
        }


        private void Create()
        {
            OrdersInMutualSettlementTable = new DataTable();
            ClientsDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            ClientsMegaOrdersDataTable = new DataTable();
            OrderStatusesDataTable = new DataTable();
            CurrencyTypesDataTable = new DataTable();
            ProductionStatusesDataTable = new DataTable();
            StorageStatusesDataTable = new DataTable();
            ExpeditionStatusesDataTable = new DataTable();
            DispatchStatusesDataTable = new DataTable();
            FactoryTypesDataTable = new DataTable();
            AgreementStatusesDataTable = new DataTable();
            DiscountOrderSumTable = new DataTable();
            TransportGridTable = new DataTable();
            DiscountPaymentConditionsTable = new DataTable();
            DiscountFactoringTable = new DataTable();

            FilterClientsBindingSource = new BindingSource();
            MainOrdersBindingSource = new BindingSource();
            MegaOrdersBindingSource = new BindingSource();
            OrderStatusesBindingSource = new BindingSource();
            CurrencyTypesBindingSource = new BindingSource();
            ProductionStatusesBindingSource = new BindingSource();
            StorageStatusesBindingSource = new BindingSource();
            ExpeditionStatusesBindingSource = new BindingSource();
            DispatchStatusesBindingSource = new BindingSource();
            FactoryTypesBindingSource = new BindingSource();
            ClientsMegaOrdersBindingSource = new BindingSource();
            AgreementStatusesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " ORDER BY ClientName, OrderNumber", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MegaOrders", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(ClientsMegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OrderStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(OrderStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DiscountOrderSum", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DiscountOrderSumTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TransportGrid", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(TransportGridTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DiscountPaymentConditions", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DiscountPaymentConditionsTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DiscountFactoring", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DiscountFactoringTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM AgreementStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(AgreementStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ProductionStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ProductionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StorageStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(StorageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ExpeditionStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ExpeditionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DispatchStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DispatchStatusesDataTable);
            }

            FactoryTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients" +
                " ORDER BY ClientName", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
        }

        private void Binding()
        {
            FilterClientsBindingSource.DataSource = ClientsDataTable;
            CurrencyTypesBindingSource.DataSource = CurrencyTypesDataTable;

            ClientsBindingSourceDisplayMember = "ClientName";
            ClientsBindingSourceValueMember = "ClientID";

            ClientsMegaOrdersBindingSourceDisplayMember = "OrderNumber";
            ClientsMegaOrdersBindingSourceValueMember = "MegaOrderID";

            ProductionStatusesBindingSource.DataSource = ProductionStatusesDataTable;
            StorageStatusesBindingSource.DataSource = StorageStatusesDataTable;
            ExpeditionStatusesBindingSource.DataSource = ExpeditionStatusesDataTable;
            DispatchStatusesBindingSource.DataSource = DispatchStatusesDataTable;
            FactoryTypesBindingSource.DataSource = FactoryTypesDataTable;

            CurrencyTypesBindingSourceDisplayMember = "CurrencyType";
            CurrencyTypesBindingSourceValueMember = "CurrencyTypeID";

            OrderStatusesBindingSource.DataSource = OrderStatusesDataTable;

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
            MainOrdersBindingSource.DataSource = MainOrdersDataTable;

            MainOrdersDataGrid.DataSource = MainOrdersBindingSource;
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;

            ClientsMegaOrdersBindingSource.DataSource = ClientsMegaOrdersDataTable;

            AgreementStatusesBindingSource.DataSource = AgreementStatusesDataTable;
        }

        private void CreateColumns()
        {
            OrderStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "OrderStatusColumn",
                HeaderText = "Статус заказа",
                DataPropertyName = "OrderStatusID",
                DataSource = OrderStatusesBindingSource,
                ValueMember = "OrderStatusID",
                DisplayMember = "OrderStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            AgreementStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "AgreementStatusColumn",
                HeaderText = "Статус\r\nсогласования",
                DataPropertyName = "AgreementStatusID",
                DataSource = AgreementStatusesBindingSource,
                ValueMember = "AgreementStatusID",
                DisplayMember = "AgreementStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            PaymentConditionColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PaymentConditionColumn",
                HeaderText = "Условия\r\nоплаты, %",
                DataPropertyName = "DiscountPaymentConditionID",
                DataSource = new DataView(DiscountPaymentConditionsTable),
                ValueMember = "DiscountPaymentConditionID",
                DisplayMember = "Discount",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            CurrencyTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CurrencyTypeColumn",
                HeaderText = "Валюта",
                DataPropertyName = "CurrencyTypeID",
                DataSource = CurrencyTypesBindingSource,
                ValueMember = "CurrencyTypeID",
                DisplayMember = "CurrencyType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilProductionStatusColumn",
                HeaderText = "Производство\r\nПрофиль",
                DataPropertyName = "ProfilProductionStatusID",
                DataSource = ProductionStatusesBindingSource,
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilStorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilStorageStatusColumn",
                HeaderText = "Cклад\r\nПрофиль",
                DataPropertyName = "ProfilStorageStatusID",
                DataSource = StorageStatusesBindingSource,
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilExpeditionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilExpeditionStatusColumn",
                HeaderText = "Экспедиция\r\nПрофиль",
                DataPropertyName = "ProfilExpeditionStatusID",
                DataSource = ExpeditionStatusesBindingSource,
                ValueMember = "ExpeditionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nПрофиль",
                DataPropertyName = "ProfilDispatchStatusID",
                DataSource = DispatchStatusesBindingSource,
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSProductionStatusColumn",
                HeaderText = "Производство\r\nТПС",
                DataPropertyName = "TPSProductionStatusID",
                DataSource = ProductionStatusesBindingSource,
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSStorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSStorageStatusColumn",
                HeaderText = "Склад\r\nТПС",
                DataPropertyName = "TPSStorageStatusID",
                DataSource = StorageStatusesBindingSource,
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSExpeditionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSExpeditionStatusColumn",
                HeaderText = "Экспедиция\r\nТПС",
                DataPropertyName = "TPSExpeditionStatusID",
                DataSource = ExpeditionStatusesBindingSource,
                ValueMember = "ExpeditionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nТПС",
                DataPropertyName = "TPSDispatchStatusID",
                DataSource = DispatchStatusesBindingSource,
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            FactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FactoryTypeColumn",
                HeaderText = "Тип\r\nпроизводства",
                DataPropertyName = "FactoryID",
                DataSource = FactoryTypesBindingSource,
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            //MegaOrdersDataGrid.Columns.Add(ClientsColumn);
            MegaOrdersDataGrid.Columns.Add(OrderStatusColumn);
            MegaOrdersDataGrid.Columns.Add(AgreementStatusColumn);
            MegaOrdersDataGrid.Columns.Add(PaymentConditionColumn);
            MegaOrdersDataGrid.Columns.Add(CurrencyTypeColumn);
            MainOrdersDataGrid.Columns.Add(ProfilProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilDispatchStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSDispatchStatusColumn);
            MainOrdersDataGrid.Columns.Add(FactoryTypeColumn);
        }

        private void MainGridSetting()
        {
            foreach (DataGridViewColumn Column in MainOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            MainOrdersDataGrid.Columns["IsSample"].Visible = false;
            MainOrdersDataGrid.Columns["WillPercentID"].Visible = false;
            MainOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilProductionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilStorageStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilExpeditionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilDispatchStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSProductionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSStorageStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSExpeditionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSDispatchStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["MegaOrderID"].Visible = false;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].Visible = false;

            if (!Security.PriceAccess)
            {
                MainOrdersDataGrid.Columns["FrontsCost"].Visible = false;
                MainOrdersDataGrid.Columns["DecorCost"].Visible = false;
                MainOrdersDataGrid.Columns["OrderCost"].Visible = false;
            }

            if (MainOrdersDataGrid.Columns.Contains("ProfilPackAllocStatusID"))
                MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSPackAllocStatusID"))
                MainOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSPackCount"))
                MainOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilPackCount"))
                MainOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilOnProductionDate"))
                MainOrdersDataGrid.Columns["ProfilOnProductionDate"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilOnProductionUserID"))
                MainOrdersDataGrid.Columns["ProfilOnProductionUserID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilProductionUserID"))
                MainOrdersDataGrid.Columns["ProfilProductionUserID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSOnProductionDate"))
                MainOrdersDataGrid.Columns["TPSOnProductionDate"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSOnProductionUserID"))
                MainOrdersDataGrid.Columns["TPSOnProductionUserID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSProductionUserID"))
                MainOrdersDataGrid.Columns["TPSProductionUserID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("CurrencyOrderCost"))
                MainOrdersDataGrid.Columns["CurrencyOrderCost"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("CurrencyTypeID"))
                MainOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;

            MainOrdersDataGrid.Columns["MainOrderID"].HeaderText = "№ п\\п";
            MainOrdersDataGrid.Columns["FrontsCost"].HeaderText = "Стоимость\r\nфасадов, евро";
            MainOrdersDataGrid.Columns["DecorCost"].HeaderText = "Стоимость\r\nдекора, евро";
            MainOrdersDataGrid.Columns["OrderCost"].HeaderText = "Стоимость\r\nзаказа, евро";
            MainOrdersDataGrid.Columns["OriginalOrderCost"].HeaderText = "Стоимость заказа,\r\nевро (оригинал)";
            MainOrdersDataGrid.Columns["OriginalProfilOrderCost"].HeaderText = "Стоимость заказа\r\nПрофиль, евро (оригинал)";
            MainOrdersDataGrid.Columns["OriginalTPSOrderCost"].HeaderText = "Стоимость заказа\r\nТПС евро, (оригинал)";
            MainOrdersDataGrid.Columns["ProfilOrderCost"].HeaderText = "Стоимость заказа\r\nПрофиль, евро";
            MainOrdersDataGrid.Columns["TPSOrderCost"].HeaderText = "Стоимость заказа\r\nТПС, евро";
            MainOrdersDataGrid.Columns["DocDateTime"].HeaderText = "Дата\r\nсоздания";
            MainOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            MainOrdersDataGrid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            MainOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MainOrdersDataGrid.Columns["ProfilProductionDate"].HeaderText = "Дата входа\r\nв пр-во, Профиль";
            MainOrdersDataGrid.Columns["TPSProductionDate"].HeaderText = "Дата входа\r\nв пр-во, ТПС";
            MainOrdersDataGrid.Columns["ProfilTotalDiscount"].HeaderText = "Общая скидка\r\nПрофиль, %";
            MainOrdersDataGrid.Columns["TPSTotalDiscount"].HeaderText = "Общая скидка\r\nТПС, %";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 2
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 3
            };
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.FormatProvider = nfi1;
            MainOrdersDataGrid.Columns["ProfilOrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["ProfilOrderCost"].DefaultCellStyle.FormatProvider = nfi1;
            MainOrdersDataGrid.Columns["TPSOrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["TPSOrderCost"].DefaultCellStyle.FormatProvider = nfi1;
            MainOrdersDataGrid.Columns["OriginalOrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["OriginalOrderCost"].DefaultCellStyle.FormatProvider = nfi1;
            MainOrdersDataGrid.Columns["OriginalProfilOrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["OriginalProfilOrderCost"].DefaultCellStyle.FormatProvider = nfi1;
            MainOrdersDataGrid.Columns["OriginalTPSOrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["OriginalTPSOrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi2;
            MainOrdersDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["FrontsSquare"].DefaultCellStyle.FormatProvider = nfi2;

            MainOrdersDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["MainOrderID"].Width = 80;
            MainOrdersDataGrid.Columns["FrontsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["FrontsCost"].Width = 130;
            MainOrdersDataGrid.Columns["DecorCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DecorCost"].Width = 130;
            MainOrdersDataGrid.Columns["OrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["ProfilOrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["TPSOrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["OriginalOrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["OriginalProfilOrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["OriginalTPSOrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["OrderCost"].Width = 130;
            MainOrdersDataGrid.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DocDateTime"].Width = 130;
            MainOrdersDataGrid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["FrontsSquare"].Width = 110;
            MainOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["Notes"].MinimumWidth = 60;
            MainOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["Weight"].Width = 90;
            MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].Width = 150;
            MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["ProfilExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].Width = 110;
            MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].Width = 110;
            MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].Width = 150;
            MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].Width = 110;
            MainOrdersDataGrid.Columns["TPSExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].Width = 110;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["FactoryTypeColumn"].Width = 140;
            MainOrdersDataGrid.Columns["ProfilProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ProfilProductionDate"].MinimumWidth = 165;
            MainOrdersDataGrid.Columns["TPSProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["TPSProductionDate"].MinimumWidth = 140;
            MainOrdersDataGrid.Columns["ProfilTotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["TPSTotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            MainOrdersDataGrid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            MainOrdersDataGrid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["FrontsSquare"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["FrontsCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DecorCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["OrderCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfilOrderCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TPSOrderCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["OriginalOrderCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["OriginalProfilOrderCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["OriginalTPSOrderCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DocDateTime"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfilProductionDate"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TPSProductionDate"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            //MainOrdersDataGrid.Columns["MainOrderID"].DisplayIndex = 0;
            //MainOrdersDataGrid.Columns["FrontsCost"].DisplayIndex = 1;
            //MainOrdersDataGrid.Columns["DecorCost"].DisplayIndex = 2;
            //MainOrdersDataGrid.Columns["OrderCost"].DisplayIndex = 3;
            //MainOrdersDataGrid.Columns["FactoryTypeColumn"].DisplayIndex = 4;
            //MainOrdersDataGrid.Columns["ProfilProductionDate"].DisplayIndex = 5;
            //MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].DisplayIndex = 6;
            //MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].DisplayIndex = 7;
            //MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].DisplayIndex = 8;
            //MainOrdersDataGrid.Columns["TPSProductionDate"].DisplayIndex = 9;
            //MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].DisplayIndex = 10;
            //MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].DisplayIndex = 11;
            //MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].DisplayIndex = 12;
            //MainOrdersDataGrid.Columns["FrontsSquare"].DisplayIndex = 13;
            //MainOrdersDataGrid.Columns["Weight"].DisplayIndex = 14;
            //MainOrdersDataGrid.Columns["DocDateTime"].DisplayIndex = 15;
            //MainOrdersDataGrid.Columns["Notes"].DisplayIndex = 16;

            MainOrdersDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["ProfilOrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["TPSOrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["OriginalOrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["OriginalProfilOrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["OriginalTPSOrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void MegaGridSetting()
        {
            foreach (DataGridViewColumn Column in MegaOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }


            if (MegaOrdersDataGrid.Columns.Contains("TransportType"))
                MegaOrdersDataGrid.Columns["TransportType"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("CreatedByClient"))
                MegaOrdersDataGrid.Columns["CreatedByClient"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("FixedPaymentRate"))
                MegaOrdersDataGrid.Columns["FixedPaymentRate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("OnAgreementDateTime"))
                MegaOrdersDataGrid.Columns["OnAgreementDateTime"].Visible = false;
            MegaOrdersDataGrid.Columns["ClientID"].Visible = false;
            MegaOrdersDataGrid.Columns["OrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilOrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSOrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["AgreementStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            MegaOrdersDataGrid.Columns["FactoryID"].Visible = false;

            if (!Security.PriceAccess)
            {
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
                MegaOrdersDataGrid.Columns["OrderCost"].Visible = false;
                MegaOrdersDataGrid.Columns["TransportCost"].Visible = false;
                MegaOrdersDataGrid.Columns["AdditionalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyOrderCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyTotalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyTransportCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["TotalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["Rate"].Visible = false;
                MegaOrdersDataGrid.Columns["PaymentRate"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].Visible = false;
            }

            if (MegaOrdersDataGrid.Columns.Contains("ProfilPackAllocStatusID"))
                MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSPackAllocStatusID"))
                MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSPackCount"))
                MegaOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ProfilPackCount"))
                MegaOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("DesireDate"))
                MegaOrdersDataGrid.Columns["DesireDate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("LastCalcDate"))
                MegaOrdersDataGrid.Columns["LastCalcDate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("LastCalcUserID"))
                MegaOrdersDataGrid.Columns["LastCalcUserID"].Visible = false;

            if (MegaOrdersDataGrid.Columns.Contains("ProfilConfirmProduction"))
                MegaOrdersDataGrid.Columns["ProfilConfirmProduction"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSConfirmProduction"))
                MegaOrdersDataGrid.Columns["TPSConfirmProduction"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ProfilAllowDispatch"))
                MegaOrdersDataGrid.Columns["ProfilAllowDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSAllowDispatch"))
                MegaOrdersDataGrid.Columns["TPSAllowDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ProfilConfirmDispatch"))
                MegaOrdersDataGrid.Columns["ProfilConfirmDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSConfirmDispatch"))
                MegaOrdersDataGrid.Columns["TPSConfirmDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("DiscountPaymentConditionID"))
                MegaOrdersDataGrid.Columns["DiscountPaymentConditionID"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("DiscountFactoringID"))
                MegaOrdersDataGrid.Columns["DiscountFactoringID"].Visible = false;
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

            MegaOrdersDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            MegaOrdersDataGrid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            MegaOrdersDataGrid.Columns["OrderDate"].HeaderText = "Дата\r\nсоздания";
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].HeaderText = "Дата отгрузки\r\nПрофиль";
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].HeaderText = "Дата отгрузки\r\nТПС";
            MegaOrdersDataGrid.Columns["OrderCost"].HeaderText = "Стоимость\r\nзаказа, евро";
            MegaOrdersDataGrid.Columns["TransportCost"].HeaderText = "Стоимость\r\nтранспорта, евро";
            MegaOrdersDataGrid.Columns["AdditionalCost"].HeaderText = "Дополнительная\r\nстоимость, евро";
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].HeaderText = "Дата\r\nсогласования";
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].HeaderText = "Стоимость\r\nзаказа, расчет";
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].HeaderText = "Итого, расчет";
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].HeaderText = "Стоимость\r\nтранспорта, расчет";
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].HeaderText = "Дополнительная\r\nстоимость, расчет";
            MegaOrdersDataGrid.Columns["TotalCost"].HeaderText = "Итого, евро";
            MegaOrdersDataGrid.Columns["Rate"].HeaderText = "Курс";
            MegaOrdersDataGrid.Columns["PaymentRate"].HeaderText = "Расчетный\r\nкурс";
            MegaOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDataGrid.Columns["Square"].HeaderText = "Площадь\r\nфасадов, м.кв.";
            //MegaOrdersDataGrid.Columns["DesireDate"].HeaderText = "  Предварит.\r\nдата отгрузки";
            MegaOrdersDataGrid.Columns["IsComplaint"].HeaderText = "Рекламация";
            MegaOrdersDataGrid.Columns["ProfilDiscountOrderSum"].HeaderText = "Скидка от суммы\r\nзаказа, Профиль %";
            MegaOrdersDataGrid.Columns["TPSDiscountOrderSum"].HeaderText = "Скидка от суммы\r\nзаказа, ТПС %";
            MegaOrdersDataGrid.Columns["ProfilDiscountDirector"].HeaderText = "Скидка директора,\r\nПрофиль %";
            MegaOrdersDataGrid.Columns["TPSDiscountDirector"].HeaderText = "Скидка директора,\r\nТПС %";
            MegaOrdersDataGrid.Columns["ProfilTotalDiscount"].HeaderText = "Общая скидка,\r\nПрофиль, %";
            MegaOrdersDataGrid.Columns["TPSTotalDiscount"].HeaderText = "Общая скидка,\r\nТПС, %";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 2
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 4
            };
            NumberFormatInfo nfi3 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 3
            };
            MegaOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["TransportCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["TransportCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["AdditionalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["AdditionalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Rate"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Rate"].DefaultCellStyle.FormatProvider = nfi2;

            MegaOrdersDataGrid.Columns["PaymentRate"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["PaymentRate"].DefaultCellStyle.FormatProvider = nfi2;

            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi3;

            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi3;

            MegaOrdersDataGrid.Columns["PaymentConditionColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["ProfilDiscountOrderSum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["TPSDiscountOrderSum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["ProfilDiscountDirector"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["TPSDiscountDirector"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["ProfilTotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["TPSTotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["ClientName"].MinimumWidth = 240;
            MegaOrdersDataGrid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["MegaOrderID"].MinimumWidth = 70;
            MegaOrdersDataGrid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["OrderNumber"].MinimumWidth = 70;
            MegaOrdersDataGrid.Columns["OrderDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["OrderDate"].Width = 150;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].Width = 140;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].Width = 140;
            MegaOrdersDataGrid.Columns["OrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["OrderCost"].Width = 130;
            MegaOrdersDataGrid.Columns["TotalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TotalCost"].Width = 130;
            MegaOrdersDataGrid.Columns["TransportCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TransportCost"].Width = 150;
            MegaOrdersDataGrid.Columns["AdditionalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["AdditionalCost"].Width = 150;
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].Width = 130;
            MegaOrdersDataGrid.Columns["AgreementStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["AgreementStatusColumn"].Width = 160;
            MegaOrdersDataGrid.Columns["OrderStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["OrderStatusColumn"].Width = 160;
            MegaOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Weight"].Width = 110;
            MegaOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Square"].Width = 140;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].Width = 130;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].Width = 130;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].Width = 190;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].Width = 170;
            MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].Width = 90;
            MegaOrdersDataGrid.Columns["Rate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Rate"].Width = 100;
            MegaOrdersDataGrid.Columns["PaymentRate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["PaymentRate"].Width = 100;
            MegaOrdersDataGrid.Columns["IsComplaint"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["IsComplaint"].Width = 115;

            MegaOrdersDataGrid.AutoGenerateColumns = false;

            MegaOrdersDataGrid.Columns["OrderDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            int DisplayIndex = 0;
            MegaOrdersDataGrid.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["OrderNumber"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["OrderDate"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["IsComplaint"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["Square"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["OrderCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["TransportCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["AdditionalCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["TotalCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["Rate"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["PaymentRate"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["AgreementStatusColumn"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["OrderStatusColumn"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["PaymentConditionColumn"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["ProfilDiscountOrderSum"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["TPSDiscountOrderSum"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["ProfilDiscountDirector"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["TPSDiscountDirector"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["ProfilTotalDiscount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDataGrid.Columns["TPSTotalDiscount"].DisplayIndex = DisplayIndex++;

            MegaOrdersDataGrid.Columns["ClientName"].Frozen = true;
            MegaOrdersDataGrid.Columns["OrderNumber"].Frozen = true;
            MegaOrdersDataGrid.RightToLeft = RightToLeft.No;

            MegaOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["TransportCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["AdditionalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Rate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["PaymentRate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MegaOrdersDataGrid.Columns["IsComplaint"].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            MainGridSetting();
            MegaGridSetting();
        }

        public void FilterMainOrdersByMegaOrder(
            int MegaOrderID,
            bool bsOnProduction,
            bool bsNotInProduction,
            bool bsInProduction,
            bool bsInStorage,
            bool bsOnExpedition,
            bool bsIsDispatched)
        {
            if (CurrentMegaOrderID == MegaOrderID)
                return;

            if (MegaOrdersBindingSource.Count == 0)
                return;

            string MainProductionStatus = string.Empty;

            if (bsNotInProduction)
            {
                MainProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)" +
                    " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1))";
            }

            if (bsInProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                }
            }

            if (bsOnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                }
                else
                {
                    MainProductionStatus = "(ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                }
            }

            if (bsInStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }
            if (bsOnExpedition)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }
            if (bsIsDispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (!bsNotInProduction && !bsInProduction && !bsOnProduction && !bsInStorage && !bsOnExpedition && !bsIsDispatched)
                MainProductionStatus = " AND MainOrderID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + MainProductionStatus,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                MainOrdersDataTable.Clear();
                DA.Fill(MainOrdersDataTable);
            }

            CurrentMegaOrderID = MegaOrderID;
        }

        public void FilterProductByMainOrder(int MainOrderID)
        {
            bool FrontsVisible = MainOrdersFrontsOrders.Filter(MainOrderID, 0);
            bool DecorVisible = MainOrdersDecorOrders.Filter(MainOrderID, 0);
            if (MainOrderID > 0)
            {
                if (OrdersTabControl.TabPages[0].PageVisible != FrontsVisible)
                    OrdersTabControl.TabPages[0].PageVisible = FrontsVisible;
                if (OrdersTabControl.TabPages[1].PageVisible != DecorVisible)
                    OrdersTabControl.TabPages[1].PageVisible = DecorVisible;
            }
        }

        public void FilterMegaOrders(
            bool bsClient,
            int ClientID,
            bool bsNotAgreed,
            bool bsOnAgreement,
            bool bsNotConfirmed,
            bool bsConfirmed,
            bool bsOnProduction,
            bool bsNotInProduction,
            bool bsInProduction,
            bool bsInStorage,
            bool bsOnExpedition,
            bool bsIsDispatched,
            bool bsDelayOfPayment,
            bool bsHalfOfPayment,
            bool bsFullPayment,
            bool bsFactoring,
            bool bsHalfOfPayment2,
            bool bsDelayOfPayment2)
        {
            string AgreementStatus = string.Empty;
            string DiscountPaymentCondition = string.Empty;
            string ClientFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (bsClient)
                ClientFilter = " AND MegaOrders.ClientID = " + ClientID;
            if (bsNotAgreed)
                AgreementStatus = "(AgreementStatusID=0 AND CreatedByClient=0)";
            if (bsOnAgreement)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " OR AgreementStatusID=3";
                else
                    AgreementStatus = "AgreementStatusID=3";
            }

            if (bsNotConfirmed)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " OR AgreementStatusID=1";
                else
                    AgreementStatus = "AgreementStatusID=1";
            }

            if (bsConfirmed)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " OR AgreementStatusID=2";
                else
                    AgreementStatus = "AgreementStatusID=2";
            }

            if (bsDelayOfPayment)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=1";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=1";
            }
            if (bsHalfOfPayment)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=2";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=2";
            }
            if (bsFullPayment)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=3";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=3";
            }
            if (bsFactoring)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=4";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=4";
            }
            if (bsDelayOfPayment2)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=6";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=6";
            }
            if (bsHalfOfPayment2)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=5";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=5";
            }

            if (bsNotInProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR ((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)" +
                        " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1))";
                }
                else
                {
                    MainProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)" +
                        " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1))";
                }
            }

            if (bsInProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                }
            }

            if (bsOnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                }
                else
                {
                    MainProductionStatus = "(ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                }
            }

            if (bsInStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }
            if (bsOnExpedition)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }
            if (bsIsDispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (AgreementStatus.Length > 0)
                AgreementStatus = " AND (" + AgreementStatus + ")";
            if (DiscountPaymentCondition.Length > 0)
                DiscountPaymentCondition = " AND (" + DiscountPaymentCondition + ")";

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " WHERE (" + MainProductionStatus + ")";

            if ((!bsNotInProduction && !bsInProduction && !bsOnProduction && !bsInStorage && !bsOnExpedition && !bsIsDispatched) || AgreementStatus.Length == 0 || DiscountPaymentCondition.Length == 0)
                MainProductionStatus = " WHERE MainOrderID = -1";

            int MegaOrderID = 0;
            if (MegaOrdersBindingSource.Count > 0)
                MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            CurrentMegaOrderID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE (MegaOrderID NOT IN (SELECT MegaOrderID FROM MainOrders) OR MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" + MainProductionStatus + ")" + AgreementStatus + DiscountPaymentCondition + ")" + ClientFilter +
                " ORDER BY ClientName, OrderNumber",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                MegaOrdersDataTable.Clear();
                DA.Fill(MegaOrdersDataTable);
            }

            MoveToMegaOrder(MegaOrderID);
        }


        public decimal DiscountPaymentCondition(int DiscountPaymentConditionID)
        {
            decimal Discount = 0;
            DataRow[] Rows = DiscountPaymentConditionsTable.Select("DiscountPaymentConditionID = " + DiscountPaymentConditionID);
            if (Rows.Count() > 0)
                Discount = Convert.ToDecimal(Rows[0]["Discount"]);
            return Discount;
        }

        public decimal DiscountFactoring(int DiscountFactoringID)
        {
            decimal Discount = 0;
            DataRow[] Rows = DiscountFactoringTable.Select("DiscountFactoringID = " + DiscountFactoringID);
            if (Rows.Count() > 0)
            {
                Discount = Convert.ToDecimal(Rows[0]["Discount"]);
            }
            return (6 - Discount);
        }

        public void GetOrdersInMuttlements(
            bool bsNotAgreed,
            bool bsOnAgreement,
            bool bsNotConfirmed,
            bool bsConfirmed,
            bool bsDelayOfPayment,
            bool bsHalfOfPayment,
            bool bsFullPayment,
            bool bsFactoring,
            bool bsHalfOfPayment2,
            bool bsDelayOfPayment2)
        {
            string AgreementStatus = string.Empty;
            string DiscountPaymentCondition = string.Empty;

            if (bsNotAgreed)
                AgreementStatus = "(AgreementStatusID=0 AND CreatedByClient=0)";

            if (bsOnAgreement)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " OR AgreementStatusID=3";
                else
                    AgreementStatus = "AgreementStatusID=3";
            }

            if (bsNotConfirmed)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " OR AgreementStatusID=1";
                else
                    AgreementStatus = "AgreementStatusID=1";
            }

            if (bsConfirmed)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " OR AgreementStatusID=2";
                else
                    AgreementStatus = "AgreementStatusID=2";
            }

            if (bsDelayOfPayment)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=1";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=1";
            }
            if (bsHalfOfPayment)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=2";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=2";
            }
            if (bsFullPayment)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=3";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=3";
            }
            if (bsFactoring)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=4";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=4";
            }
            if (bsDelayOfPayment2)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=6";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=6";
            }
            if (bsHalfOfPayment2)
            {
                if (DiscountPaymentCondition.Length > 0)
                    DiscountPaymentCondition += " OR DiscountPaymentConditionID=5";
                else
                    DiscountPaymentCondition = "DiscountPaymentConditionID=5";
            }

            if (AgreementStatus.Length > 0)
                AgreementStatus = " WHERE (" + AgreementStatus + ")";
            if (DiscountPaymentCondition.Length > 0)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " AND (" + DiscountPaymentCondition + ")";
                else
                    AgreementStatus = " WHERE (" + DiscountPaymentCondition + ")";
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MutualSettlementOrders
                WHERE ClientID IN (SELECT ClientID FROM MegaOrders " + AgreementStatus + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                OrdersInMutualSettlementTable.Clear();
                DA.Fill(OrdersInMutualSettlementTable);
            }
        }

        public bool IsOrderInMuttlement(int ClientID, int OrderNumber)
        {
            DataRow[] rows = OrdersInMutualSettlementTable.Select("ClientID=" + ClientID + " AND OrderNumber=" + OrderNumber);
            return rows.Count() > 0;
        }

        public void MoveToMegaOrder(int MegaOrderID)
        {
            MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
        }

        public void CreateNewMainOrder()
        {
            DataRow Row = MainOrdersDataTable.NewRow();

            Row["MegaOrderID"] = CurrentMegaOrderID;
            Row["DocDateTime"] = Security.GetCurrentDate();
            Row["WillPercentID"] = 0;

            MainOrdersDataTable.Rows.Add(Row);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders" +
                " WHERE MegaOrderID = " + CurrentMegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MainOrdersDataTable);
                    MainOrdersDataTable.Clear();
                    DA.Fill(MainOrdersDataTable);
                }
            }

            MainOrdersBindingSource.MoveLast();

            CurrentMainOrderID = Convert.ToInt32(MainOrdersDataTable.Rows[MainOrdersDataTable.Rows.Count - 1]["MainOrderID"]);
        }

        public void SaveOrder(int MainOrderID, string Notes, int FactoryID)
        {
            for (int i = 0; i < 3; i++)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, IsSample, Notes, FactoryID," +
                    " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID," +
                    " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID" +
                    " FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            DT.Rows[0]["Notes"] = Notes;
                            DT.Rows[0]["FactoryID"] = FactoryID;

                            if (FactoryID == 1)
                            {
                                DT.Rows[0]["TPSProductionStatusID"] = 0;
                                DT.Rows[0]["TPSStorageStatusID"] = 0;
                                DT.Rows[0]["TPSExpeditionStatusID"] = 0;
                                DT.Rows[0]["TPSDispatchStatusID"] = 0;

                                DT.Rows[0]["ProfilProductionStatusID"] = 1;
                                DT.Rows[0]["ProfilStorageStatusID"] = 1;
                                DT.Rows[0]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[0]["ProfilDispatchStatusID"] = 1;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[0]["ProfilProductionStatusID"] = 0;
                                DT.Rows[0]["ProfilStorageStatusID"] = 0;
                                DT.Rows[0]["ProfilExpeditionStatusID"] = 0;
                                DT.Rows[0]["ProfilDispatchStatusID"] = 0;

                                DT.Rows[0]["TPSProductionStatusID"] = 1;
                                DT.Rows[0]["TPSStorageStatusID"] = 1;
                                DT.Rows[0]["TPSDispatchStatusID"] = 1;
                                DT.Rows[0]["TPSExpeditionStatusID"] = 1;
                            }

                            if (FactoryID == 0)
                            {
                                DT.Rows[0]["ProfilProductionStatusID"] = 1;
                                DT.Rows[0]["ProfilStorageStatusID"] = 1;
                                DT.Rows[0]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[0]["ProfilDispatchStatusID"] = 1;

                                DT.Rows[0]["TPSProductionStatusID"] = 1;
                                DT.Rows[0]["TPSStorageStatusID"] = 1;
                                DT.Rows[0]["TPSExpeditionStatusID"] = 1;
                                DT.Rows[0]["TPSDispatchStatusID"] = 1;
                            }

                            //SetNotAgreed(Convert.ToInt32(DT.Rows[0]["MegaOrderID"]));

                            try
                            {
                                DA.Update(DT);

                                break;
                            }
                            catch
                            { }

                        }
                    }
                }
            }

            //DataRow[] Row = MainOrdersDataTable.Select("MainOrderID = " + MainOrderID);
            //MainOrdersDataAdapter.Update(MainOrdersDataTable);
            //MainOrdersDataTable.Clear();
            //MainOrdersDataAdapter.Fill(MainOrdersDataTable);
            //MainOrdersBindingSource.MoveLast();
        }

        public void SaveOrder(string Notes, int FactoryID, bool NeedSetStatus)
        {
            for (int i = 0; i < 3; i++)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, IsSample, Notes, FactoryID," +
                    " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID," +
                    " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID" +
                    " FROM MainOrders WHERE MainOrderID = " + CurrentMainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            DT.Rows[0]["Notes"] = Notes;
                            DT.Rows[0]["FactoryID"] = FactoryID;

                            if (NeedSetStatus)
                            {
                                if (FactoryID == 1)
                                {
                                    DT.Rows[0]["TPSProductionStatusID"] = 0;
                                    DT.Rows[0]["TPSStorageStatusID"] = 0;
                                    DT.Rows[0]["TPSExpeditionStatusID"] = 0;
                                    DT.Rows[0]["TPSDispatchStatusID"] = 0;

                                    DT.Rows[0]["ProfilProductionStatusID"] = 1;
                                    DT.Rows[0]["ProfilStorageStatusID"] = 1;
                                    DT.Rows[0]["ProfilExpeditionStatusID"] = 1;
                                    DT.Rows[0]["ProfilDispatchStatusID"] = 1;
                                }

                                if (FactoryID == 2)
                                {
                                    DT.Rows[0]["ProfilProductionStatusID"] = 0;
                                    DT.Rows[0]["ProfilStorageStatusID"] = 0;
                                    DT.Rows[0]["ProfilExpeditionStatusID"] = 0;
                                    DT.Rows[0]["ProfilDispatchStatusID"] = 0;

                                    DT.Rows[0]["TPSProductionStatusID"] = 1;
                                    DT.Rows[0]["TPSStorageStatusID"] = 1;
                                    DT.Rows[0]["TPSExpeditionStatusID"] = 1;
                                    DT.Rows[0]["TPSDispatchStatusID"] = 1;
                                }

                                if (FactoryID == 0)
                                {
                                    DT.Rows[0]["ProfilProductionStatusID"] = 1;
                                    DT.Rows[0]["ProfilStorageStatusID"] = 1;
                                    DT.Rows[0]["ProfilExpeditionStatusID"] = 1;
                                    DT.Rows[0]["ProfilDispatchStatusID"] = 1;

                                    DT.Rows[0]["TPSProductionStatusID"] = 1;
                                    DT.Rows[0]["TPSStorageStatusID"] = 1;
                                    DT.Rows[0]["TPSExpeditionStatusID"] = 1;
                                    DT.Rows[0]["TPSDispatchStatusID"] = 1;
                                }

                                //SetNotAgreed(Convert.ToInt32(DT.Rows[0]["MegaOrderID"]));
                            }
                            try
                            {
                                DA.Update(DT);

                                break;
                            }
                            catch
                            { }

                        }
                    }
                }
            }

            //MainOrdersDataTable.Clear();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MegaOrderID = " + CurrentMegaOrderID, 
            //    ConnectionStrings.MarketingOrdersConnectionString))
            //{
            //    DA.Fill(MainOrdersDataTable);
            //}

            //MainOrdersBindingSource.Position = MainOrdersBindingSource.Find("MainOrderID", CurrentMainOrderID);
        }

        public void OrdersSet(int[] MainOrdersID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID IN (" +
                string.Join(",", MainOrdersID) + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 1; i < MainOrdersID.Count(); i++)
                        {
                            DataRow[] FRows = DT.Select("MainOrderID=" + MainOrdersID[i]);

                            for (int j = 0; j < FRows.Count(); j++)
                            {
                                FRows[j].ItemArray = DT.Select("MainOrderID=" + MainOrdersID[0])[j].ItemArray;
                                FRows[j]["MainOrderID"] = MainOrdersID[i];
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID IN (" +
                string.Join(",", MainOrdersID) + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 1; i < MainOrdersID.Count(); i++)
                        {
                            DataRow[] DRows = DT.Select("MainOrderID=" + MainOrdersID[i]);

                            for (int j = 0; j < DRows.Count(); j++)
                            {
                                DRows[j].ItemArray = DT.Select("MainOrderID=" + MainOrdersID[0])[j].ItemArray;
                                DRows[j]["MainOrderID"] = MainOrdersID[i];
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrdersID) + ")",
                    ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 1; i < MainOrdersID.Count(); i++)
                        {
                            DataRow[] MRows = DT.Select("MainOrderID=" + MainOrdersID[i]);
                            foreach (DataRow MRow in MRows)
                            {
                                MRow.ItemArray = DT.Select("MainOrderID=" + MainOrdersID[0])[0].ItemArray;
                                MRow["MainOrderID"] = MainOrdersID[i];
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }

            //MainOrdersDataTable.Clear();
            //MainOrdersDataAdapter.Fill(MainOrdersDataTable);
        }

        public void GetCurrentMainOrder()
        {
            if (MainOrdersBindingSource.Count == 0)
            {
                CurrentMainOrderID = -1;
                return;
            }
            if (((DataRowView)MainOrdersBindingSource.Current).Row["MainOrderID"] == DBNull.Value)
                return;
            else
                CurrentMainOrderID = Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current).Row["MainOrderID"]);
        }

        public int[] GetMainOrders(int[] MegaOrders)
        {
            ArrayList MainOrders = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);


                    for (int i = 0; i < DT.Rows.Count; i++)
                        MainOrders.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                }
            }

            return MainOrders.OfType<Int32>().ToArray();
        }

        public int[] GetSelectedMainOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value));

            int[] rows = array.OfType<int>().ToArray();
            Array.Sort(rows);

            return rows;
        }

        public int[] GetSelectedMegaOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value));

            int[] rows = array.OfType<int>().ToArray();
            Array.Sort(rows);

            return rows;
        }

        public int[] GetSelectedOrderNumbers()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["OrderNumber"].Value));

            int[] rows = array.OfType<int>().ToArray();
            Array.Sort(rows);

            return rows;
        }

        public bool AreSelectedMegaOrdersOneClient
        {
            get
            {
                int[] rows = new int[MegaOrdersDataGrid.SelectedRows.Count];

                for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                    rows[i] = Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["ClientID"].Value);

                int Min = rows.Min();
                int Max = rows.Max();

                return (Min == Max);
            }
        }

        public bool AreSelectedMegaOrdersAgree(int[] MegaOrderIDs)
        {
            int AgreementStatusID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT AgreementStatusID" +
                " FROM MegaOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrderIDs) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                AgreementStatusID = Convert.ToInt32(DT.Rows[i]["AgreementStatusID"]);

                                if (AgreementStatusID == 0 || AgreementStatusID == 3)
                                    return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool CanRemoveMainOrder()
        {
            if (((DataRowView)MegaOrdersBindingSource.Current).Row["AgreementStatusID"].ToString() == "2")
                return false;

            return true;
        }

        public bool UpdateMainOrders(
            bool bsOnProduction,
            bool bsNotInProduction,
            bool bsInProduction,
            bool bsInStorage,
            bool bsOnExpedition,
            bool bsIsDispatched)
        {
            GetCurrentMainOrder();

            int MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            FilterMainOrdersByMegaOrder(
                Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"]),
                bsOnProduction, bsNotInProduction, bsInProduction, bsInStorage, bsOnExpedition, true);

            MoveToMainOrder(CurrentMainOrderID);

            return true;
        }

        public bool EditMainOrder(ref string Notes, ref bool IsSample)
        {
            //if (NeedSetStatus)
            //{
            //    if (!CanRemoveMainOrder())
            //    {
            //        MessageBox.Show("Заказ согласован и не может быть изменен", "Редактирование заказа");
            //        return false;
            //    }
            //}
            GetCurrentMainOrder();

            int ClientID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current)["ClientID"]);

            Notes = ((DataRowView)MainOrdersBindingSource.Current)["Notes"].ToString();
            IsSample = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["IsSample"]);

            return true;
        }

        public void MoveToMainOrder(int MainOrderID)
        {
            MainOrdersBindingSource.Position = MainOrdersBindingSource.Find("MainOrderID", MainOrderID);
        }

        public void RemoveCurrentMainOrder()
        {
            if (MainOrdersBindingSource.Count < 1)
                return;

            int Pos = MainOrdersBindingSource.Position;

            //int MainOrderID = Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current)["MainOrderID"]);
            GetCurrentMainOrder();
            //check status

            if (!CanRemoveMainOrder())
            {
                MessageBox.Show("Заказ согласован и не может быть удален");
                return;
            }

            MainOrdersFrontsOrders.DeleteOrder(CurrentMainOrderID);

            MainOrdersDecorOrders.DeleteOrder(CurrentMainOrderID);
            MainOrdersDecorOrders.DeleteDecorAssignmentByMainOrder(CurrentMainOrderID);

            DataRow[] Row = MainOrdersDataTable.Select("MainOrderID = " + CurrentMainOrderID);

            Row[0].Delete();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders" +
                " WHERE MegaOrderID = " + CurrentMegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MainOrdersDataTable);
                }
            }

            //остается на позиции удаленного заказа, а не перемещается в начало
            if (MainOrdersBindingSource.Count > 0)
                if (Pos >= MainOrdersBindingSource.Count)
                    MainOrdersBindingSource.MoveLast();
                else
                    MainOrdersBindingSource.Position = Pos;
        }

        public void SetNotAgreed(int MegaOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, CurrencyOrderCost," +
                " CurrencyTransportCost, CurrencyAdditionalCost, CurrencyTotalCost, CurrencyTypeID, Rate, ConfirmDateTime, AgreementStatusID" +
                " FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            DT.Rows[0]["AgreementStatusID"] = 0;
                            DT.Rows[0]["CurrencyTypeID"] = 1;
                            DT.Rows[0]["CurrencyOrderCost"] = 0;
                            DT.Rows[0]["CurrencyTransportCost"] = 0;
                            DT.Rows[0]["CurrencyAdditionalCost"] = 0;
                            DT.Rows[0]["CurrencyTotalCost"] = 0;
                            DT.Rows[0]["Rate"] = 1;
                            DT.Rows[0]["ConfirmDateTime"] = DBNull.Value;

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SummaryCost(int MegaOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    decimal OrderCost = 0;
                    decimal TotalWeight = 0;
                    decimal TotalSquare = 0;
                    decimal TotalCost = 0;
                    decimal TransportCost = 0;
                    decimal AdditionalCost = 0;
                    decimal ProfilTotalDiscount = 0;
                    decimal TPSTotalDiscount = 0;
                    //decimal CurrencyOrderCost = 0;

                    int Factory = -1;

                    foreach (DataRow Row in DT.Rows)
                    {
                        ProfilTotalDiscount = Convert.ToDecimal(Row["ProfilTotalDiscount"]);
                        TPSTotalDiscount = Convert.ToDecimal(Row["TPSTotalDiscount"]);
                        OrderCost += Convert.ToDecimal(Row["OrderCost"]);
                        //CurrencyOrderCost += Convert.ToDecimal(Row["CurrencyOrderCost"]);
                        TotalWeight += Convert.ToDecimal(Row["Weight"]);
                        TotalSquare += Convert.ToDecimal(Row["FrontsSquare"]);

                        if (Factory == 0)
                            continue;

                        if (Row["FactoryID"].ToString() == "0")
                            Factory = 0;

                        if (Row["FactoryID"].ToString() == "1")
                            if (Factory == 2)
                                Factory = 0;
                            else
                                Factory = 1;

                        if (Row["FactoryID"].ToString() == "2")
                            if (Factory == 1)
                                Factory = 0;
                            else
                                Factory = 2;
                    }

                    using (SqlDataAdapter MegaDA = new SqlDataAdapter("SELECT MegaOrderID, OrderCost, CurrencyOrderCost, Weight, Square," +
                        " FactoryID, TotalCost, ProfilTotalDiscount, TPSTotalDiscount" +
                        " FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID,
                        ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        using (SqlCommandBuilder CB = new SqlCommandBuilder(MegaDA))
                        {
                            using (DataTable MegaDT = new DataTable())
                            {
                                MegaDA.Fill(MegaDT);

                                TotalWeight = Decimal.Round(TotalWeight, 3, MidpointRounding.AwayFromZero);
                                TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);

                                MegaDT.Rows[0]["ProfilTotalDiscount"] = ProfilTotalDiscount;
                                MegaDT.Rows[0]["TPSTotalDiscount"] = TPSTotalDiscount;
                                MegaDT.Rows[0]["OrderCost"] = OrderCost;
                                //MegaDT.Rows[0]["CurrencyOrderCost"] = CurrencyOrderCost;
                                MegaDT.Rows[0]["Weight"] = TotalWeight;
                                MegaDT.Rows[0]["Square"] = TotalSquare;
                                MegaDT.Rows[0]["FactoryID"] = Factory;

                                TotalCost = OrderCost + TransportCost + AdditionalCost;
                                MegaDT.Rows[0]["TotalCost"] = TotalCost;

                                MegaDA.Update(MegaDT);
                            }
                        }
                    }
                }
            }
        }

        public decimal GetMainOrdersCost(int[] MainOrders)
        {
            decimal OrderCost = 0;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT OrderCost FROM MainOrders" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        OrderCost += Convert.ToDecimal(DT.Rows[i]["OrderCost"]);
                    }
                }
            }

            return OrderCost;
        }

        public bool NBRBDailyRates(DateTime date, ref decimal EURBYRCurrency)
        {
            string EuroXML = "";
            string url = "http://www.nbrb.by/Services/XmlExRates.aspx?ondate=" + date.ToString("MM/dd/yyyy");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebRequest.AllowAutoRedirect = true;
                CookieContainer cookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer = cookieContainer;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            XmlTextReader reader = new XmlTextReader(myHttpWebResponse.GetResponseStream());
            //XmlTextReader reader = new XmlTextReader("http://www.nbrb.by/Services/XmlExRates.aspx?ondate=" + date.ToString("MM/dd/yyyy"));
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Currency")
                            {
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "Id")
                                        {
                                            if (reader.Value == "292")
                                            {
                                                reader.MoveToElement();
                                                EuroXML = reader.ReadOuterXml();
                                            }
                                        }
                                        if (reader.Name == "Id")
                                        {
                                            if (reader.Value == "19")
                                            {
                                                reader.MoveToElement();
                                                EuroXML = reader.ReadOuterXml();
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
                XmlDocument euroXmlDocument = new XmlDocument();
                euroXmlDocument.LoadXml(EuroXML);
                XmlNode xmlNode = euroXmlDocument.SelectSingleNode("Currency/Rate");
                bool b = decimal.TryParse(xmlNode.InnerText, out EURBYRCurrency);
                if (!b)
                    EURBYRCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
                else
                    EURBYRCurrency = Convert.ToDecimal(xmlNode.InnerText);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message + " . КУРСЫ МОЖНО БУДЕТ ВВЕСТИ ВРУЧНУЮ");
                return false;
            }
            return true;
        }

        public bool CBRDailyRates(DateTime date, ref decimal EURRUBCurrency, ref decimal USDRUBCurrency)
        {
            string EuroXML = "";
            string USDXml = "";

            string url = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date.ToString("dd/MM/yyyy");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebRequest.AllowAutoRedirect = true;
                CookieContainer cookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer = cookieContainer;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            XmlTextReader reader1 = new XmlTextReader(myHttpWebResponse.GetResponseStream());

            try
            {
                while (reader1.Read())
                {
                    switch (reader1.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader1.Name == "Valute")
                            {
                                if (reader1.HasAttributes)
                                {
                                    while (reader1.MoveToNextAttribute())
                                    {
                                        if (reader1.Name == "ID")
                                        {
                                            if (reader1.Value == "R01239")
                                            {
                                                reader1.MoveToElement();
                                                EuroXML = reader1.ReadOuterXml();
                                            }
                                        }
                                        if (reader1.Name == "ID")
                                        {
                                            if (reader1.Value == "R01235")
                                            {
                                                reader1.MoveToElement();
                                                USDXml = reader1.ReadOuterXml();
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
                XmlDocument euroXmlDocument = new XmlDocument();
                euroXmlDocument.LoadXml(EuroXML);
                XmlDocument usdXmlDocument = new XmlDocument();
                usdXmlDocument.LoadXml(USDXml);

                XmlNode xmlNode = euroXmlDocument.SelectSingleNode("Valute/Value");
                EURRUBCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
                xmlNode = usdXmlDocument.SelectSingleNode("Valute/Value");
                USDRUBCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message + " . КУРСЫ МОЖНО БУДЕТ ВВЕСТИ ВРУЧНУЮ");
                return false;
            }
            return true;
        }

        public void GetDateRates(DateTime DateTime, ref bool RateExist, ref decimal USD, ref decimal RUB, ref decimal BYN, ref decimal USDRUB)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM DateRates WHERE CAST(Date AS Date) = 
                    '" + DateTime.ToString("yyyy-MM-dd") + "'",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        RateExist = true;
                        USD = Convert.ToDecimal(DT.Rows[0]["USD"]);
                        RUB = Convert.ToDecimal(DT.Rows[0]["RUB"]);
                        BYN = Convert.ToDecimal(DT.Rows[0]["BYN"]);
                        USDRUB = Convert.ToDecimal(DT.Rows[0]["USDRUB"]);
                    }
                    else
                        RateExist = false;
                }
            }
            return;
        }

        public void SaveDateRates(DateTime DateTime, decimal USD, decimal RUB, decimal BYN, decimal USDRUB)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DateRates WHERE CAST(Date AS Date) = 
                    '" + DateTime.ToString("yyyy-MM-dd") + "' ORDER BY DateRateID DESC",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);

                        DataRow Row = DT.NewRow();
                        Row["Date"] = DateTime;
                        Row["USD"] = USD;
                        Row["RUB"] = RUB;
                        Row["BYN"] = BYN;
                        Row["USDRUB"] = USDRUB;
                        DT.Rows.Add(Row);
                        DA.Update(DT);
                    }
                }
            }
            return;
        }

        public string GetClientName(int ClientID)
        {
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientName FROM Clients WHERE ClientID = " + ClientID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientName = DT.Rows[0]["ClientName"].ToString();
                }
            }

            return ClientName;
        }

        public void SetCurrencyCost(int MegaOrderID,
            decimal CurrencyTotalCost)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MegaOrderID, OrderStatusID, AgreementStatusID, OrderCost,
                ComplaintProfilCost, ComplaintTPSCost, CurrencyComplaintProfilCost, CurrencyComplaintTPSCost, ComplaintNotes, IsComplaint,DelayOfPayment,
                TransportCost, AdditionalCost, TotalCost, CurrencyOrderCost," +
                " CurrencyTransportCost, CurrencyAdditionalCost, CurrencyTotalCost, CurrencyTypeID, Rate, PaymentRate, ConfirmDateTime, AgreementStatusID, DiscountPaymentConditionID, DiscountFactoringID, ProfilDiscountOrderSum, TPSDiscountOrderSum, ProfilDiscountDirector, TPSDiscountDirector" +
                " FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        decimal CurrencyTransportCost = Convert.ToDecimal(DT.Rows[0]["CurrencyTransportCost"]);
                        decimal CurrencyAdditionalCost = Convert.ToDecimal(DT.Rows[0]["CurrencyAdditionalCost"]);
                        decimal dPaymentCurrency = Convert.ToDecimal(DT.Rows[0]["PaymentRate"]);

                        decimal CurrencyOrderCost = Decimal.Round((CurrencyTotalCost - CurrencyTransportCost - CurrencyAdditionalCost), 2, MidpointRounding.AwayFromZero);

                        DT.Rows[0]["CurrencyTotalCost"] = CurrencyTotalCost;
                        DT.Rows[0]["CurrencyOrderCost"] = CurrencyOrderCost;

                        DA.Update(DT);
                    }
                }
            }
        }

        public static int GetClientCountry(int ClientID)
        {
            int ClientCountryID = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, CountryID FROM Clients WHERE ClientID=" + ClientID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        ClientCountryID = Convert.ToInt32(DT.Rows[0]["CountryID"]);
                    }
                }
            }

            return ClientCountryID;
        }

        public static int GetDelayOfPayment(int ClientID)
        {
            int DelayOfPayment = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, DelayOfPayment FROM Clients WHERE ClientID=" + ClientID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        DelayOfPayment = Convert.ToInt32(DT.Rows[0]["DelayOfPayment"]);
                    }
                }
            }
            return DelayOfPayment;
        }

        public static string GetClientLogin(int ClientID)
        {
            string Login = "unnamed";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, Login FROM Clients WHERE ClientID=" + ClientID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && DT.Rows[0]["Login"] != DBNull.Value)
                    {
                        Login = DT.Rows[0]["Login"].ToString();

                        int engCount = 0;
                        int rusCount = 0;
                        foreach (char c in Login)
                        {
                            if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                                rusCount++;
                            else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z'))
                                engCount++;
                        }
                        if (rusCount > engCount)
                            Login = "unnamed";
                    }
                }
            }

            return Login;
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

    }

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
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig)
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
            FrontsConfigDataTable = TablesManager.FrontsConfigDataTableAll;

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
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, Rate, ClientID, CurrencyTypeID FROM MegaOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID + ")";
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

            for (int i = 0; i < Rows.Count(); i++)
            {
                int FrontID = Convert.ToInt32(Rows[i]["FrontID"]);
                if (FrontID == 3729)
                    continue;
                decimal d = GetInsetSquare(Convert.ToInt32(Rows[i]["FrontID"]), Convert.ToInt32(Rows[i]["Height"]), Convert.ToInt32(Rows[i]["Width"])) * Convert.ToDecimal(Rows[i]["Count"]);
                Count += d;
                Cost += Math.Ceiling(Convert.ToDecimal(Rows[i]["InsetPrice"]) * d / 0.01m) * 0.01m;
            }

            if (Count > 0)
            {
                //Cost = Math.Ceiling(Cost / 0.01m) * 0.01m;
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
                NewRow["Price"] = Decimal.Round(Cost / Count, 2, MidpointRounding.AwayFromZero);
                NewRow["Cost"] = Decimal.Round(Math.Ceiling(Cost / 0.01m) * 0.01m, 2, MidpointRounding.AwayFromZero);
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

            DataRow[] FRows = OrdersDataTable.Select("InsetColorID = 3944");

            if (FRows.Count() > 0)
            {
                PriceFlutes = Convert.ToDecimal(FRows[0]["InsetPrice"]);

                for (int i = 0; i < FRows.Count(); i++)
                {
                    if (FC.IsAluminium(FRows[i]) != -1)
                        continue;

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
                NewRow["UNN"] = UNN;
                NewRow["PaymentRate"] = PaymentRate;
                NewRow["AccountingName"] = AccountingName;
                NewRow["InvNumber"] = InvNumber;
                NewRow["CurrencyCode"] = ProfilCurrencyCode;
                if (fID == 2)
                    NewRow["TPSCurCode"] = TPSCurrencyCode;
                NewRow["Name"] = "Решетка овал";
                NewRow["Count"] = CountEllipseGrid;
                NewRow["Measure"] = "м.кв.";
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
                NewRow["Name"] = "Апплик. №1 правая";
                NewRow["Count"] = CountApplic1R;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic1R;
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
                NewRow["Name"] = "Апплик. №1 левая";
                NewRow["Count"] = CountApplic1L;
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = PriceApplic1L;
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
            if (FrontID == 16269 || FrontID == 28945 || FrontID == 27914 || FrontID == 3727 || FrontID == 3728 || FrontID == 3729 ||
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

        public void Report(int[] MainOrderIDs, DataTable InfoDT)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);
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

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, infiniu2_catalog.dbo.FrontsConfig.InvNumber, 
                (-MegaOrders.ComplaintProfilCost-MegaOrders.ComplaintTPSCost+MegaOrders.TransportCost+MegaOrders.AdditionalCost) AS TotalAdditionalCost,
                MegaOrders.ComplaintProfilCost, MegaOrders.ComplaintTPSCost, MegaOrders.TransportCost, MegaOrders.AdditionalCost, MegaOrders.MegaOrderID, MegaOrders.PaymentRate FROM FrontsOrders
                INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID = infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE " + sWhere + " ORDER BY FrontID",
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
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig)) ORDER BY ProductName ASC";
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
            string SelectCommand = "SELECT MegaOrderID, TransportCost, AdditionalCost, Rate, ClientID, CurrencyTypeID FROM MegaOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID + ")";
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

        public void Report(int[] MainOrderIDs)
        {
            GetMegaOrderInfo(MainOrderIDs[0]);
            string sWhere = "";

            for (int i = 0; i < MainOrderIDs.Count(); i++)
            {
                if (sWhere != "")
                    sWhere += " OR DecorOrders.MainOrderID = " + MainOrderIDs[i].ToString();
                else
                    sWhere += "DecorOrders.MainOrderID = " + MainOrderIDs[i].ToString();
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, infiniu2_catalog.dbo.DecorConfig.InvNumber, MegaOrders.PaymentRate FROM DecorOrders
                INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                WHERE " + sWhere + " ORDER BY DecorID",
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
                if (CurrencyTypeID == 0)
                {
                    DecCount = 0;
                    if (OriginalPrice != 0)
                        OriginalPrice = Math.Ceiling(OriginalPrice / 100.0m) * 100.0m;
                    if (CostWithTransport != 0)
                        CostWithTransport = Math.Ceiling(CostWithTransport / 100.0m) * 100.0m;
                    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                    Cost = Price * Count;
                    PriceWithTransport = Math.Ceiling(PriceWithTransport / 100.0m) * 100.0m;
                    CostWithTransport = PriceWithTransport * Count;
                }
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
                if (CurrencyTypeID == 0)
                {
                    DecCount = 0;
                    if (OriginalPrice != 0)
                        OriginalPrice = Math.Ceiling(OriginalPrice / 100.0m) * 100.0m;
                    if (CostWithTransport != 0)
                        CostWithTransport = Math.Ceiling(CostWithTransport / 100.0m) * 100.0m;
                    Price = Math.Ceiling(Price / 100.0m) * 100.0m;
                    Cost = Price * Count;
                    PriceWithTransport = Math.Ceiling(PriceWithTransport / 100.0m) * 100.0m;
                    CostWithTransport = PriceWithTransport * Count;
                }
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
                        GridCost += Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                        GridCostWithTransport += Convert.ToDecimal(ProfilReportTable.Rows[i]["CostWithTransport"]);
                        GridSquare += Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]);
                        GridWeight += Convert.ToDecimal(ProfilReportTable.Rows[i]["Weight"]);
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
                    string ProfilCurrencyCode = string.Empty;
                    string TPSCurrencyCode = string.Empty;
                    //ADD to Table

                    if (ProfilReportTable.Rows.Count > 0)
                    {
                        AccountingName = ProfilReportTable.Rows[0]["AccountingName"].ToString();
                        InvNumber = ProfilReportTable.Rows[0]["InvNumber"].ToString();
                        UNN = ProfilReportTable.Rows[0]["UNN"].ToString();
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
                        NewRow["Name"] = "Решетка";
                        NewRow["Count"] = GridSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = Decimal.Round(GridCost / GridSquare, 2, MidpointRounding.AwayFromZero);
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
                        NewRow["Name"] = "Стекло Лакомат";
                        NewRow["Count"] = LacomatSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = LacomatPrice;
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
                        NewRow["Name"] = "Стекло Кризет";
                        NewRow["Count"] = KrizetSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = KrizetPrice;
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
                        NewRow["Name"] = "Стекло Флутес";
                        NewRow["Count"] = FlutesSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = FlutesPrice;
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
                NewRow["Name"] = dt.Rows[i]["Name"];
                NewRow["Count"] = dt.Rows[i]["Count"];
                NewRow["Measure"] = dt.Rows[i]["Measure"];
                NewRow["Price"] = dt.Rows[i]["Price"];
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
                        GridCost += Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                        GridCostWithTransport += Convert.ToDecimal(TPSReportTable.Rows[i]["CostWithTransport"]);
                        GridSquare += Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]);
                        GridWeight += Convert.ToDecimal(TPSReportTable.Rows[i]["Weight"]);

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
                    string ProfilCurrencyCode = string.Empty;
                    string TPSCurrencyCode = string.Empty;
                    //ADD to Table

                    if (TPSReportTable.Rows.Count > 0)
                    {
                        AccountingName = TPSReportTable.Rows[0]["AccountingName"].ToString();
                        InvNumber = TPSReportTable.Rows[0]["InvNumber"].ToString();
                        UNN = TPSReportTable.Rows[0]["UNN"].ToString();
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
                        NewRow["Name"] = "Решетка";
                        NewRow["Count"] = GridSquare;
                        NewRow["Measure"] = "м.кв.";
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
                        NewRow["Name"] = "Стекло Лакомат";
                        NewRow["Count"] = LacomatSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = LacomatPrice;
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
                        NewRow["Name"] = "Стекло Кризет";
                        NewRow["Count"] = KrizetSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = KrizetPrice;
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
                        NewRow["Name"] = "Стекло Флутес";
                        NewRow["Count"] = FlutesSquare;
                        NewRow["Measure"] = "м.кв.";
                        NewRow["Price"] = FlutesPrice;
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
                NewRow["Count"] = dt.Rows[i]["Count"];
                NewRow["Measure"] = dt.Rows[i]["Measure"];
                NewRow["Price"] = dt.Rows[i]["Price"];
                NewRow["PriceWithTransport"] = dt.Rows[i]["PriceWithTransport"];
                NewRow["Cost"] = dt.Rows[i]["Cost"];
                NewRow["CostWithTransport"] = dt.Rows[i]["CostWithTransport"];
                NewRow["Weight"] = dt.Rows[i]["Weight"];
                TPSReportTable.Rows.Add(NewRow);
            }
        }

        public void AssignCost(decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost, decimal WeightProfil, decimal WeightTPS,
                               decimal TotalProfil, decimal TotalTPS, ref decimal TransportAndOtherProfil,
                               ref decimal TransportAndOtherTPS)
        {
            decimal Total = TransportCost + AdditionalCost;

            decimal TotalWeight = WeightProfil + WeightTPS;

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
            //return;
            //Profil
            for (int i = 0; i < ProfilReportTable.Rows.Count; i++)
            {
                decimal ItemCost = Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]);
                decimal pItem = ItemCost / TotalProfil * 100;

                if (ItemCost == 0)
                    continue;

                ProfilReportTable.Rows[i]["Cost"] = Decimal.Round(ItemCost + cProfil / 100 * pItem, 2, MidpointRounding.AwayFromZero);
                ProfilReportTable.Rows[i]["Price"] = Decimal.Round(Convert.ToDecimal(ProfilReportTable.Rows[i]["Cost"]) /
                                                     Convert.ToDecimal(ProfilReportTable.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
            }

            //TPS
            for (int i = 0; i < TPSReportTable.Rows.Count; i++)
            {
                decimal ItemCost = Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]);
                decimal pItem = ItemCost / TotalTPS * 100;

                if (ItemCost == 0)
                    continue;

                TPSReportTable.Rows[i]["Cost"] = Decimal.Round(ItemCost + cTPS / 100 * pItem, 2, MidpointRounding.AwayFromZero);
                TPSReportTable.Rows[i]["Price"] = Decimal.Round(Convert.ToDecimal(TPSReportTable.Rows[i]["Cost"]) / Convert.ToDecimal(TPSReportTable.Rows[i]["Count"]), 2, MidpointRounding.AwayFromZero);
            }
        }

        private int GetMegaOrderID(int MainOrderID)
        {
            int MegaOrderID = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MainOrders" +
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
                            " FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID,
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

        public static void DataSetIntoDBF(string path, string fileName, DataTable DT)
        {
            if (File.Exists(path + fileName + ".dbf"))
            {
                File.Delete(path + fileName + ".dbf");
            }

            string createSql = "create table " + fileName + " ([UNN] varchar(20), [CurrencyCode] varchar(20), [TPSCurCode] varchar(20), [InvNumber] varchar(20), [AccountingName] varchar(100), [Amount] Double, [Price] Double, [Weight] Double)";

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
                    " (UNN, CurrencyCode, TPSCurCode, InvNumber, AccountingName, Amount, Price, Weight) values(UNN, CurrencyCode, TPSCurCode, InvNumber, AccountingName, Amount, Price, Weight)";
                decimal d = Decimal.Round(Convert.ToDecimal(row["Weight"]) / 1000, 3, MidpointRounding.AwayFromZero);
                double Amount = Convert.ToDouble(row["Count"]);
                double Price = Convert.ToDouble(row["Price"]);
                double Weight = Convert.ToDouble(d);
                string InvNumber = row["InvNumber"].ToString();
                string CurrencyCode = row["CurrencyCode"].ToString();
                string TPSCurCode = row["TPSCurCode"].ToString();
                string UNN = row["UNN"].ToString();
                string AccountingName = row["AccountingName"].ToString();
                string Name = row["Name"].ToString();
                //Name = convertDefaultToDos(Name);
                cmd.CommandText = insertSql;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("UNN", OleDbType.VarChar).Value = UNN;
                cmd.Parameters.Add("CurrencyCode", OleDbType.VarChar).Value = CurrencyCode;
                cmd.Parameters.Add("TPSCurCode", OleDbType.VarChar).Value = TPSCurCode;
                cmd.Parameters.Add("InvNumber", OleDbType.VarChar).Value = InvNumber;
                cmd.Parameters.Add("AccountingName", OleDbType.VarChar).Value = AccountingName;
                cmd.Parameters.Add("Amount", OleDbType.Double).Value = Amount;
                cmd.Parameters.Add("Price", OleDbType.Double).Value = Price;
                cmd.Parameters.Add("Weight", OleDbType.Double).Value = Weight;
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }

        private static string convertDefaultToDos(string src)
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

        public DataTable GetMegaOrdersTable(int[] MegaOrders)
        {
            DataTable ReturnDT = new DataTable();
            string SelectCommand = @"SELECT MegaOrders.MegaOrderID, MegaOrders.OrderNumber, MegaOrders.ClientID, 
                MegaOrders.ComplaintProfilCost, MegaOrders.ComplaintTPSCost, MegaOrders.TransportCost, MegaOrders.AdditionalCost, MegaOrders.CurrencyTypeID,
                MainOrders.MainOrderID, MainOrders.Weight
                FROM MainOrders
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID=MegaOrders.MegaOrderID
                AND MegaOrders.MegaOrderID IN (" + string.Join(",", MegaOrders) + ")";
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
                    SelectCommand = @"SELECT Square,Weight,FactoryID FROM FrontsOrders WHERE MainOrderID=" + MainOrderID;
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
                    SelectCommand = @"SELECT Weight,FactoryID FROM DecorOrders WHERE MainOrderID=" + MainOrderID;
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
            ref HSSFSheet sheet1, int[] MegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost, decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID, int pos)
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
            DataTable InfoDT = GetMegaOrdersTable(MegaOrders);
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
            //AssignCost(ComplaintProfilCost, ComplaintTPSCost, TransportCost, AdditionalCost, WeightProfil, WeightTPS, TotalProfil, TotalTPS, ref TransportAndOtherProfil,
            //           ref TransportAndOtherTPS);

            TotalProfil = Decimal.Round(TotalProfil, 3, MidpointRounding.AwayFromZero);
            TotalTPS = Decimal.Round(TotalTPS, 3, MidpointRounding.AwayFromZero);
            TransportCost = Decimal.Round(TransportCost, 3, MidpointRounding.AwayFromZero);
            AdditionalCost = Decimal.Round(AdditionalCost, 3, MidpointRounding.AwayFromZero);
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
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig)
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
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MainOrderID=" +
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
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MegaOrders WHERE MegaOrderID=" +
                    "(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        OrderNumber = Convert.ToInt32(DT.Rows[0]["OrderNumber"]);
                }
            }
            return OrderNumber;
        }

        private string GetClientID(int MainOrderID)
        {
            string ClientID = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MegaOrders WHERE MegaOrderID=" +
                    "(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = DT.Rows[0]["ClientID"].ToString();
                }
            }
            return ClientID;
        }

        private string GetClientName(int MainOrderID)
        {
            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + GetClientID(MainOrderID));

            return Rows[0]["ClientName"].ToString();
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

                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow2["Height"] = Convert.ToInt32(Row["Height"]);

                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow2["Length"] = Convert.ToInt32(Row["Length"]);

                    if (Convert.ToInt32(Row["Width"]) != -1)
                        NewRow2["Width"] = Convert.ToInt32(Row["Width"]);

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

        private bool FilterOrders(int MainOrderID)
        {
            bool IsNotEmpty = false;

            FrontsOrdersDataTable.Clear();
            FrontsOrdersDataTable.AcceptChanges();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.*, MegaOrders.PaymentRate FROM FrontsOrders INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID AND FrontsOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(FrontsOrdersDataTable) > 0)
                    IsNotEmpty = true;
            }

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.*, MegaOrders.PaymentRate FROM DecorOrders INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID AND DecorOrders.MainOrderID=" + MainOrderID +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID",
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
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MainOrders" +
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
                " FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
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

        public string Report(int[] MegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost,
            decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID)
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
                MegaOrders, OrderNumbers, MainOrdersIDs, ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost,
                TransportCost, AdditionalCost, TotalCost, CurrencyTypeID, RowIndex);

            string FileName = ClientsDataTable.Select("ClientName = '" + ClientName + "'")[0]["Login"].ToString() +
                    " - №" + string.Join(",", OrderNumbers);

            //FileName = Path.Combine(ReportFilePath, FileName);

            //int DocNumber = 1;

            //while (File.Exists(FileName))
            //{
            //    FileName = ClientsDataTable.Select("ClientName = '" + ClientName + "'")[0]["Login"].ToString() +
            //        " - №" + string.Join(",", OrderNumbers) + "(" + DocNumber++ + ").xls";
            //    FileName = Path.Combine(ReportFilePath, FileName);
            //}

            //FileInfo file = new FileInfo(FileName);

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

            //if (Send)
            //    System.Diagnostics.Process.Start(file.FullName);
            ClearReport();
            return file.FullName;
        }

        public void Report(ref HSSFWorkbook hssfworkbook, int[] MegaOrders, int[] OrderNumbers, int[] MainOrdersIDs, int ClientID, string ClientName,
            decimal ComplaintProfilCost, decimal ComplaintTPSCost,
            decimal TransportCost, decimal AdditionalCost,
            decimal TotalCost, int CurrencyTypeID)
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
                        Cell1.SetCellValue("Высота\\длина");
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

                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(16);
                        Cell1.SetCellValue("Курс, " + Currency);
                        Cell1.CellStyle = SimpleHeaderCS;
                        Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(17);
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
                MegaOrders, OrderNumbers, MainOrdersIDs, ClientID, ClientName, ComplaintProfilCost, ComplaintTPSCost,
                TransportCost, AdditionalCost, TotalCost, CurrencyTypeID, RowIndex);

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

    public class SendEmail
    {
        //string ReportFilePath = null;

        DataTable ClientsDataTable = null;

        public bool Success = false;

        public SendEmail()
        {
            //ReadReportFilePath("MarketingClientReportPath.config");

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
        }

        //private void ReadReportFilePath(string FileName)
        //{
        //    ReportFilePath = Properties.Settings.Default.MarketingClientReportPath;
        //    //using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName, Encoding.Default))
        //    //{
        //    //    ReportFilePath = sr.ReadToEnd();
        //    //}
        //}

        private string GetClientName(int ClientID)
        {
            DataRow[] Row = ClientsDataTable.Select("ClientID = " + ClientID.ToString());

            return Row[0]["Login"].ToString();
        }

        private string GetClientEmail(int ClientID)
        {
            string Email = null;

            DataRow[] Row = ClientsDataTable.Select("ClientID = " + ClientID.ToString());

            Email = Row[0]["Email"].ToString();

            return Email;
        }

        public void Send(int ClientID, string OrderNumbers, bool Save, string FileName)
        {
            string AccountPassword = "1290qpalzm";
            string SenderEmail = "zovprofilreport@mail.ru";


            string to = GetClientEmail(ClientID);
            string from = SenderEmail;

            if (to.Length == 0)
            {
                MessageBox.Show("У клиента не указан Email. Отправка отчета не возможна");
                return;
            }

            using (MailMessage message = new MailMessage(from, to))
            {
                if (OrderNumbers.Count() > 1)
                    message.Subject = "Отчет по отгрузке заказов №" + OrderNumbers;
                else
                    message.Subject = "Отчет по отгрузке заказа №" + OrderNumbers;
                message.Body = "Отчет сгенерирован автоматически системой Infinium. Не надо отвечать на это письмо. По всем вопросам обращайтесь " +
                    "marketing.zovprofil@gmail.com";
                SmtpClient client = new SmtpClient("smtp.mail.ru")
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SenderEmail, AccountPassword)
                };
                string S = new UTF8Encoding().GetString(Encoding.Convert(Encoding.GetEncoding("UTF-16"), Encoding.UTF8, Encoding.GetEncoding("UTF-16").GetBytes(FileName)));

                Attachment attach = new Attachment(S,
                                                   MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attach.ContentDisposition;

                message.Attachments.Add(attach);

                try
                {
                    client.Send(message);
                    Success = true;
                }

                catch (ArgumentException ex)
                {
                    MessageBox.Show("ArgumentException\r\n" + ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("InvalidOperationException\r\n" + ex.Message);
                }
                //catch (ObjectDisposedException ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                catch (SmtpException ex)
                {
                    MessageBox.Show("SmtpException\r\n" + ex.Message);
                }
                //catch (SmtpFailedRecipientsException ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                catch (Exception ex)
                {
                    MessageBox.Show("Exception\r\n" + ex.Message);
                }

                attach.Dispose();
                client.Dispose();
            }
        }

        public void DeleteFile(string FileName)
        {
            FileInfo file = new FileInfo(FileName);
            if (file.Exists == true)
            {
                file.Delete();
            }
        }
    }

    public class BatchExcelReport
    {
        DataTable FrontsResultDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable ColorsDataTable = null;
        private DataTable InsetColorsDataTable = null;

        public BatchExcelReport()
        {
            FrontsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            ColorsDataTable = new DataTable();

            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
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
            CreateFrontsDataTable();
        }

        private void GetColorsDT()
        {
            ColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
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

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            //FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            //FrontsResultDataTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.Decimal")));
        }

        private void FillFronts(DataTable FrontsOrdersDataTable)
        {
            FrontsResultDataTable.Clear();
            if (FrontsOrdersDataTable.Rows.Count < 1)
                return;
            string Front = string.Empty;
            string FrameColor = string.Empty;
            string QueryString = string.Empty;
            decimal FrontSquare = 0;
            decimal FrontCost = 0;
            int FrontCount = 0;

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                QueryString = "FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]);
                DataRow[] Rows = FrontsOrdersDataTable.Select(QueryString);
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        if (row["Square"] != DBNull.Value)
                            FrontSquare += Convert.ToDecimal(row["Square"]);
                        if (row["Cost"] != DBNull.Value)
                            FrontCost += Convert.ToDecimal(row["Cost"]);
                        FrontCount += Convert.ToInt32(row["Count"]);
                    }
                    Front = GetFrontName(Convert.ToInt32(Table.Rows[i]["FrontID"]));
                    //FrameColor = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"]));

                    DataRow NewRow = FrontsResultDataTable.NewRow();
                    NewRow["Front"] = Front;
                    NewRow["Square"] = Decimal.Round(FrontSquare, 3, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(FrontCost, 3, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrontCount;
                    FrontsResultDataTable.Rows.Add(NewRow);

                    FrontSquare = 0;
                    FrontCost = 0;
                    FrontCount = 0;
                }
            }
        }

        public string GetFrontName(int FrontID)
        {
            DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
            return Rows[0]["FrontName"].ToString();
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
            return Rows[0]["ShortName"].ToString();
        }

        public string GetColorName(int ColorID)
        {
            DataRow[] Rows = ColorsDataTable.Select("ColorID = " + ColorID);
            return Rows[0]["ColorName"].ToString();
        }

        private int GetFCount(DataTable DT, bool Curved)
        {
            int S = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Curved)
                {
                    if (Convert.ToDecimal(Row["Square"]) == 0)
                        S += Convert.ToInt32(Row["Count"]);
                }
                else
                    S += Convert.ToInt32(Row["Count"]);
            }

            return S;
        }

        private decimal GetDCount(DataTable DT, bool Pogon)
        {
            decimal PogonCount = 0;
            int DecorCount = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Convert.ToInt32(Row["MeasureID"]) == 2)
                    PogonCount += Convert.ToDecimal(Row["Count"]);
                else
                    DecorCount += Convert.ToInt32(Row["Count"]);
            }

            if (Pogon)
                return PogonCount;
            else
                return DecorCount;
        }

        private decimal GetSquare(DataTable DT)
        {
            decimal S = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Square"] != DBNull.Value)
                    S += Convert.ToDecimal(Row["Square"]);
            }
            S = Decimal.Round(S, 2, MidpointRounding.AwayFromZero);
            return S;
        }

        private decimal GetCost(DataTable DT)
        {
            decimal S = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Cost"] != DBNull.Value)
                    S += Convert.ToDecimal(Row["Cost"]);
            }
            S = Decimal.Round(S, 2, MidpointRounding.AwayFromZero);
            return S;
        }

        public void CreateReport(
            DataTable FrontsOrdersDataTable, DataTable DecorOrdersDataTable,
            string FileName)
        {
            if (FrontsOrdersDataTable.Rows.Count < 1 && DecorOrdersDataTable.Rows.Count < 1)
                return;
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

            HSSFFont ClientNameFont = hssfworkbook.CreateFont();
            ClientNameFont.FontHeightInPoints = 14;
            ClientNameFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            ClientNameFont.FontName = "Calibri";

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 13;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle ClientNameStyle = hssfworkbook.CreateCellStyle();
            ClientNameStyle.SetFont(ClientNameFont);

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.FontHeightInPoints = 13;
            HeaderFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
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
            PackNumberFont.Boldweight = 12 * 256;
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
            SimpleFont.FontHeightInPoints = 12;
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

            if (FrontsOrdersDataTable.Rows.Count > 0)
                FrontsReport(hssfworkbook, HeaderStyle, PackNumberFont, SimpleFont, SimpleCellStyle, FrontsOrdersDataTable);

            if (DecorOrdersDataTable.Rows.Count > 0)
                DecorReport(hssfworkbook, HeaderStyle, SimpleFont, SimpleCellStyle, DecorOrdersDataTable);

            string ReportFilePath = string.Empty;

            //ReportFilePath = Application.StartupPath + @"\" + "Отчеты";

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            //ReportFilePath = Application.StartupPath + @"\Отчеты\" + @"Статистика\";

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            //ReportFilePath = ReadReportFilePath("StatisticsReportPath.config");
            //FileInfo file = new FileInfo(ReportFilePath + FileName + ".xls");

            //int j = 1;
            //while (file.Exists == true)
            //{
            //    file = new FileInfo(ReportFilePath + FileName + "(" + j++ + ").xls");
            //}

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

        private int FrontsReport(HSSFWorkbook hssfworkbook, HSSFCellStyle HeaderStyle, HSSFFont PackNumberFont, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            DataTable FrontsOrdersDT)
        {
            int RowIndex = 0;

            decimal Square = 0;
            decimal Cost = 0;
            int FrontsCount = 0;
            int CurvedCount = 0;

            FillFronts(FrontsOrdersDT);
            Square = GetSquare(FrontsOrdersDT);
            Cost = GetCost(FrontsOrdersDT);
            FrontsCount = GetFCount(FrontsOrdersDT, false);
            CurvedCount = GetFCount(FrontsOrdersDT, true);

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Фасады");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 25 * 256);
            sheet1.SetColumnWidth(1, 25 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);

            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Фасад");
            cell4.CellStyle = HeaderStyle;
            //cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Цвет профиля");
            //cell4.CellStyle = HeaderStyle;
            cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Площ.");
            cell4.CellStyle = HeaderStyle;
            cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Кол.");
            cell4.CellStyle = HeaderStyle;
            cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Стоимость");
            cell4.CellStyle = HeaderStyle;

            RowIndex++;

            DataTable DT = null;
            using (DataView DV = new DataView(FrontsResultDataTable))
            {
                DV.Sort = "Front";
                DT = DV.ToTable();
            }

            if (DT.Columns.Contains("FrontID"))
                DT.Columns.Remove("FrontID");
            if (DT.Columns.Contains("Width"))
                DT.Columns.Remove("Width");
            if (DT.Columns.Contains("Height"))
                DT.Columns.Remove("Height");
            if (DT.Columns.Contains("Width"))
                DT.Columns.Remove("Width");

            //вывод заказов фасадов
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(SimpleFont);

            if (Square > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "квадратура: ");
                cell20.CellStyle = cellStyle1;
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToDouble(Square));
                cell.CellStyle = cellStyle1;
                cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, " м.кв.");
                cell20.CellStyle = cellStyle1;
            }
            if (Cost > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "стоимость: ");
                cell20.CellStyle = cellStyle1;
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToDouble(Cost));
                cell.CellStyle = cellStyle1;
                cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, " евро");
                cell20.CellStyle = cellStyle1;
            }
            if (FrontsCount > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "всего фасадов: ");
                cell20.CellStyle = cellStyle1;
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToInt32(FrontsCount));
                cell.CellStyle = cellStyle1;
                cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, " шт.");
                cell20.CellStyle = cellStyle1;
            }
            if (CurvedCount > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "в том числе гнутых: ");
                cell20.CellStyle = cellStyle1;
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToInt32(CurvedCount));
                cell.CellStyle = cellStyle1;
                cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, " шт.");
                cell20.CellStyle = cellStyle1;
            }

            RowIndex++;

            return RowIndex;
        }

        private int DecorReport(HSSFWorkbook hssfworkbook, HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            DataTable DecorOrdersDT)
        {
            int RowIndex = 0;

            decimal Pogon = 0;
            decimal Cost = 0;
            decimal DecorCount = 0;

            Pogon = GetDCount(DecorOrdersDT, true);
            Cost = GetCost(DecorOrdersDT);
            DecorCount = GetDCount(DecorOrdersDT, false);

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Декор");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 26 * 256);
            sheet1.SetColumnWidth(1, 14 * 256);
            sheet1.SetColumnWidth(2, 20 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);

            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Продукт");
            cell15.CellStyle = HeaderStyle;
            cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Стоимость");
            cell15.CellStyle = HeaderStyle;
            cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Кол-во");
            cell15.CellStyle = HeaderStyle;
            cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Ед.изм.");
            cell15.CellStyle = HeaderStyle;

            RowIndex++;

            DataTable DT = null;
            using (DataView DV = new DataView(DecorOrdersDT))
            {
                DV.Sort = "DecorProduct";
                DT = DV.ToTable();
            }

            if (DT.Columns.Contains("ProductID"))
                DT.Columns.Remove("ProductID");
            if (DT.Columns.Contains("MeasureID"))
                DT.Columns.Remove("MeasureID");

            //вывод заказов декора в excel
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;
            }
            RowIndex++;

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(SimpleFont);

            if (Pogon > 0)
            {
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToDouble(Pogon));
                cell.CellStyle = cellStyle1;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, " м.п.");
                cell20.CellStyle = cellStyle1;
            }
            if (Cost > 0)
            {
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToDouble(Cost));
                cell.CellStyle = cellStyle1;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, " евро");
                cell20.CellStyle = cellStyle1;
            }
            if (DecorCount > 0)
            {
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToInt32(DecorCount));
                cell.CellStyle = cellStyle1;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, " шт.");
                cell20.CellStyle = cellStyle1;
            }

            return RowIndex;
        }

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
}
