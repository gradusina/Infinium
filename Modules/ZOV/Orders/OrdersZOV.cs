using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Infinium.Modules.ZOV
{



    public class FrontsCatalogOrder
    {
        ComponentFactory.Krypton.Toolkit.KryptonComboBox HeightEdit;
        ComponentFactory.Krypton.Toolkit.KryptonComboBox WidthEdit;

        private DataTable TempFrontsConfigDataTable = null;
        private DataTable TempFrontsDataTable = null;

        public DataTable ConstFrontsConfigDataTable = null;
        public DataTable ConstFrontsDataTable = null;
        public DataTable ConstTechnoProfilesDataTable = null;
        public DataTable ConstColorsDataTable = null;
        public DataTable ConstPatinaDataTable = null;
        public DataTable ConstInsetTypesDataTable = null;
        public DataTable ConstInsetColorsDataTable = null;

        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable TechnoProfilesDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        public DataTable TechnoFrameColorsDataTable = null;
        public DataTable TechnoInsetTypesDataTable = null;
        public DataTable TechnoInsetColorsDataTable = null;

        public DataTable HeightDataTable = null;
        public DataTable WidthDataTable = null;

        public BindingSource FrontsBindingSource = null;
        public BindingSource FrameColorsBindingSource = null;
        public BindingSource TechnoProfilesBindingSource = null;
        public BindingSource PatinaBindingSource = null;
        public BindingSource InsetTypesBindingSource = null;
        public BindingSource InsetColorsBindingSource = null;
        public BindingSource TechnoFrameColorsBindingSource = null;
        public BindingSource TechnoInsetTypesBindingSource = null;
        public BindingSource TechnoInsetColorsBindingSource = null;
        public BindingSource FrontsConfigBindingSource = null;

        public String FrontsBindingSourceDisplayMember = null;
        public String FrameColorsBindingSourceDisplayMember = null;
        public String TechnoProfilesBindingSourceDisplayMember = null;
        public String PatinaBindingSourceDisplayMember = null;
        public String InsetColorsBindingSourceDisplayMember = null;
        public String InsetTypesBindingSourceDisplayMember = null;

        public String FrontsBindingSourceValueMember = null;
        public String FrameColorsBindingSourceValueMember = null;
        public String TechnoProfilesBindingSourceValueMember = null;
        public String PatinaBindingSourceValueMember = null;
        public String InsetColorsBindingSourceValueMember = null;
        public String InsetTypesBindingSourceValueMember = null;

        public FrontsCatalogOrder(
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tHeightEdit,
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tWidthEdit)
        {
            HeightEdit = tHeightEdit;
            WidthEdit = tWidthEdit;

            Initialize();
        }


        private void Create()
        {
            ConstFrontsConfigDataTable = new DataTable();
            ConstTechnoProfilesDataTable = new DataTable();
            ConstFrontsDataTable = new DataTable();
            ConstColorsDataTable = new DataTable();
            ConstPatinaDataTable = new DataTable();
            ConstInsetTypesDataTable = new DataTable();
            ConstInsetColorsDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            TechnoProfilesDataTable = new DataTable();
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
            TechnoProfilesBindingSource = new BindingSource();
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
            ConstColorsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
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
                ConstInsetColorsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
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
                " WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(ConstFrontsConfigDataTable);
            //    ConstFrontsConfigDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            //}
            ConstFrontsConfigDataTable = TablesManager.FrontsConfigDataTable;
            ConstFrontsConfigDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            SelectCommand = @"SELECT DISTINCT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            //            SelectCommand = @"SELECT DISTINCT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
            //                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1) ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstFrontsDataTable);
                ConstFrontsDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstTechnoProfilesDataTable);

                DataRow NewRow = ConstTechnoProfilesDataTable.NewRow();
                NewRow["TechnoProfileID"] = -1;
                NewRow["TechnoProfileName"] = "-";
                ConstTechnoProfilesDataTable.Rows.InsertAt(NewRow, 0);
            }

            GetColorsDT();
            GetInsetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstInsetTypesDataTable);
                ConstInsetTypesDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }
            InsetTypesDataTable = ConstInsetTypesDataTable.Copy();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ConstPatinaDataTable);
                ConstPatinaDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Int32")));
            }


            using (DataView DV = new DataView(ConstFrontsDataTable))
            {
                FrontsDataTable = DV.ToTable(true, new string[] { "FrontName" });
            }

            TempFrontsConfigDataTable = ConstFrontsConfigDataTable.Copy();
            TempFrontsDataTable = ConstFrontsDataTable.Copy();
            TechnoProfilesDataTable = ConstTechnoProfilesDataTable.Copy();
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

        private void Binding()
        {
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrameColorsBindingSource.DataSource = FrameColorsDataTable;
            TechnoProfilesBindingSource.DataSource = TechnoProfilesDataTable;
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
            TechnoProfilesBindingSourceDisplayMember = "TechnoProfileName";
            PatinaBindingSourceDisplayMember = "PatinaName";
            InsetColorsBindingSourceDisplayMember = "InsetColorName";
            InsetTypesBindingSourceDisplayMember = "InsetTypeName";

            FrontsBindingSourceValueMember = "FrontName";
            FrameColorsBindingSourceValueMember = "ColorID";
            TechnoProfilesBindingSourceValueMember = "TechnoProfileID";
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

            //foreach (DataRow Row in Table.Rows)
            //{
            //    foreach (DataRow CRow in ConstInsetTypesDataTable.Rows)
            //    {
            //        if (Convert.ToInt32(CRow["InsetTypeID"]) == Convert.ToInt32(Row["InsetTypeID"]))
            //        {
            //            DataRow NewRow = ItemInsetTypesDataTable.NewRow();
            //            NewRow["InsetTypeID"] = CRow["InsetTypeID"];
            //            NewRow["GroupID"] = CRow["GroupID"];
            //            NewRow["InsetTypeName"] = CRow["InsetTypeName"];
            //            ItemInsetTypesDataTable.Rows.Add(NewRow);
            //            break;
            //        }
            //    }
            //}

            foreach (DataRow CRow in ConstInsetTypesDataTable.Select("GroupID=3"))
            {
                foreach (DataRow Row in Table.Rows)
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
            }
            foreach (DataRow CRow in ConstInsetTypesDataTable.Select("GroupID=7"))
            {
                foreach (DataRow Row in Table.Rows)
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
            }
            foreach (DataRow CRow in ConstInsetTypesDataTable.Select("GroupID<>3 AND GroupID<>7 AND InsetTypeID<>1"))
            {
                foreach (DataRow Row in Table.Rows)
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
            }
            foreach (DataRow CRow in ConstInsetTypesDataTable.Select("InsetTypeID=1"))
            {
                foreach (DataRow Row in Table.Rows)
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
            }
            Table.Dispose();

            //ItemInsetTypesDataTable.DefaultView.Sort = "InsetTypeName ASC";
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

        public void GetTechnoProfiles()
        {
            DataTable Table = new DataTable();

            using (DataView DV = new DataView(TempFrontsConfigDataTable))
            {
                Table = DV.ToTable(true, new string[] { "TechnoProfileID" });
            }

            TechnoProfilesDataTable.Clear();

            foreach (DataRow Row in Table.Rows)
                foreach (DataRow CRow in ConstTechnoProfilesDataTable.Rows)
                {
                    if (Convert.ToInt32(CRow["TechnoProfileID"]) == Convert.ToInt32(Row["TechnoProfileID"]))
                    {
                        DataRow NewRow = TechnoProfilesDataTable.NewRow();
                        NewRow["TechnoProfileID"] = CRow["TechnoProfileID"];
                        NewRow["TechnoProfileName"] = CRow["TechnoProfileName"];
                        TechnoProfilesDataTable.Rows.Add(NewRow);
                        break;
                    }
                }

            Table.Dispose();

            TechnoProfilesDataTable.DefaultView.Sort = "TechnoProfileName ASC";
            TechnoProfilesBindingSource.MoveFirst();
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
            TechnoProfilesDataTable.Clear();
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

        public void FilterCatalogTechnoProfiles(string FrontName, int ColorID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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

                GetTechnoProfiles();
            }
        }

        public void FilterCatalogTechnoFrameColors(string FrontName, int ColorID, int TechnoProfileID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoFrameColors();
            }
        }

        public void FilterCatalogPatina(string FrontName, int ColorID, int TechnoProfileID, int TechnoColorID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetPatina();
            }
        }

        public void FilterCatalogInsetTypes(string FrontName, int ColorID, int TechnoProfileID, int TechnoColorID, int PatinaID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetInsetTypes();
            }
        }

        public void FilterCatalogInsetColors(string FrontName, int ColorID, int TechnoProfileID, int TechnoColorID, int PatinaID, int InsetTypeID, bool bExcluzive) //фильтрует и заполняет цвета наполнителя по выбранному типу наполнителя
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetInsetColors();
            }
        }

        public void FilterCatalogTechnoInsetTypes(string FrontName, int ColorID, int TechnoProfileID, int TechnoColorID, int PatinaID, int InsetTypeID, int InsetColorID, bool bExcluzive) //фильтрует и заполняет типы наполнителя по выбранному типу фасада
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND PatinaID=" + PatinaID;
                DV.RowFilter += " AND InsetTypeID=" + InsetTypeID;
                DV.RowFilter += " AND InsetColorID=" + InsetColorID;

                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoInsetTypes();
            }
        }

        public void FilterCatalogTechnoInsetColors(string FrontName, int ColorID, int TechnoProfileID, int TechnoColorID, int PatinaID, int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, bool bExcluzive) //фильтрует и заполняет цвета наполнителя по выбранному типу наполнителя
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;
                DV.RowFilter += " AND TechnoColorID=" + TechnoColorID;
                DV.RowFilter += " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
                TempFrontsConfigDataTable = DV.ToTable();

                GetTechnoInsetColors();
            }
        }

        public void FilterCatalogHeight(string FrontName, int ColorID, int TechnoProfileID, int TechnoColorID, int PatinaID,
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;
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

        public void FilterCatalogWidth(string FrontName, int ColorID, int TechnoProfileID, int TechnoColorID, int PatinaID,
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
                DV.RowFilter += " AND TechnoProfileID=" + TechnoProfileID;
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
            int InsetTypeID, int InsetColorID, int TechnoProfileID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID,
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
                            "TechnoProfileID = " + Convert.ToInt32(TechnoProfileID) + " AND " +
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
                "TechnoProfileID = " + Convert.ToInt32(TechnoProfileID) + " AND " +
                "TechnoColorID = " + Convert.ToInt32(TechnoColorID) + " AND " +
                "TechnoInsetTypeID = " + Convert.ToInt32(TechnoInsetTypeID) + " AND " +
                "TechnoInsetColorID = " + Convert.ToInt32(TechnoInsetColorID) +
                HeightFilter + WidthFilter);

            if (Rows.Count() < 1)
            {
                MessageBox.Show("Ошибка конфигурации фасада. Проверьте каталог " + Rows.Count().ToString());
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
            int InsetTypeID, int InsetColorID, int TechnoProfileID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID,
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
                            "TechnoProfileID = " + Convert.ToInt32(TechnoProfileID) + " AND " +
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
                "TechnoProfileID = " + Convert.ToInt32(TechnoProfileID) + " AND " +
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
                "TechnoProfileID = " + Convert.ToInt32(TechnoProfileID) + " AND " +
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

        public void UpdateConfig()
        {
            TempFrontsConfigDataTable.Clear();
            TempFrontsConfigDataTable = ConstFrontsConfigDataTable.Copy();
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
        public DataTable MeasuresDataTable = null;

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

        public DecorCatalogOrder(ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tLengthEdit,
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tHeightEdit,
            ref ComponentFactory.Krypton.Toolkit.KryptonComboBox tWidthEdit)
        {
            LengthEdit = tLengthEdit;
            HeightEdit = tHeightEdit;
            WidthEdit = tWidthEdit;

            Initialize();
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
            MeasuresDataTable = new DataTable();
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
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL))) ORDER BY ProductName ASC";
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
            //ColorsGroupsDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ColorsGroups", ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(ColorsGroupsDataTable);
            //}
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }

            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorConfig " +
            //    " WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL", ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDataTable);
            //    DecorConfigDataTable.Columns.Add(new DataColumn("Excluzive", Type.GetType("System.Boolean")));
            //}
            DecorConfigDataTable = TablesManager.DecorConfigDataTable;
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



        public int GetMeasureID(int DecorConfigID)
        {
            return Convert.ToInt32(DecorConfigDataTable.Select("DecorConfigID = " + DecorConfigID)[0]["MeasureID"]);
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
            string Name = string.Empty;
            DataRow[] rows = DecorDataTable.Select("DecorID = " + DecorID);
            if (rows.Count() > 0)
                Name = rows[0]["Name"].ToString();
            return Name;
        }

        public int GetDecorConfigID(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID,
            int Length, int Height, int Width, ref int FactoryID, ref int AreaID)
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


            Rows = DecorConfigDataTable.Select(
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " +
                                "DecorID = " + Convert.ToInt32(DecorID) +
                                ColorFilter + PatinaFilter + InsetTypeFilter + InsetColorFilter + LengthFilter + HeightFilter + WidthFilter);

            if (Rows.Count() < 1 || Rows.Count() > 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог");
                return -1;
            }

            FactoryID = Convert.ToInt32(Rows[0]["FactoryID"]);

            return Convert.ToInt32(Rows[0]["DecorConfigID"]);
        }

        public int GetDecorConfigID(int ProductID, string Name, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID,
            int Length, int Height, int Width, ref int DecorID, ref int FactoryID, ref int AreaID)
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


            Rows = DecorConfigDataTable.Select(
                                "ProductID = " + Convert.ToInt32(ProductID) + " AND " + filter +
                                ColorFilter + PatinaFilter + InsetTypeFilter + InsetColorFilter + LengthFilter + HeightFilter + WidthFilter);

            if (Rows.Count() < 1 || Rows.Count() > 1)
            {
                MessageBox.Show("Ошибка конфигурации декора. Проверьте каталог");
                return -1;
            }

            FactoryID = Convert.ToInt32(Rows[0]["FactoryID"]);
            DecorID = Convert.ToInt32(Rows[0]["DecorID"]);

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

        public string PatinaDisplayName(int PatinaID)
        {
            DataRow[] rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
                return rows[0]["DisplayName"].ToString();
            return string.Empty;
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







    public class DoubleFrontsOrders
    {
        public bool Debts = false;

        //private DataTable NewFrontsOrdersDT = null;
        private DataTable FrontsOrdersDT = null;
        private DataTable FrontsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;

        //public BindingSource NewFrontsOrdersBS = null;
        public BindingSource FrontsOrdersBS = null;
        private BindingSource FrontsBindingSource = null;
        private BindingSource PatinaBindingSource = null;
        private BindingSource InsetTypesBindingSource = null;
        private BindingSource FrameColorsBindingSource = null;
        private BindingSource InsetColorsBindingSource = null;
        private BindingSource TechnoInsetTypesBindingSource = null;
        private BindingSource TechnoInsetColorsBindingSource = null;

        FrontsCatalogOrder FrontsCatalogOrder;

        public DoubleFrontsOrders(ref FrontsCatalogOrder tFrontsCatalogOrder)
        {
            FrontsCatalogOrder = tFrontsCatalogOrder;
        }

        public bool HasFronts => FrontsOrdersBS.Count > 0;

        public BindingSource OldFrontsOrdersList => FrontsOrdersBS;

        public DataTable CurrentFrontsOrdersDT
        {
            get
            {
                DataTable DT = new DataTable();
                using (DataView DV = new DataView(FrontsOrdersDT))
                {
                    DT = DV.ToTable(false, "FrontID", "PatinaID", "InsetTypeID", "ColorID", "TechnoColorID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID",
                        "Height", "Width", "Count");
                }
                return DT;
            }
        }

        private void Create()
        {
            //NewFrontsOrdersDT = new DataTable();
            FrontsOrdersDT = new DataTable();
            FrontsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            //NewFrontsOrdersBS = new BindingSource();
            FrontsOrdersBS = new BindingSource();
            FrontsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            InsetTypesBindingSource = new BindingSource();
            FrameColorsBindingSource = new BindingSource();
            InsetColorsBindingSource = new BindingSource();
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

        }

        private void Binding()
        {
            FrontsOrdersBS.DataSource = FrontsOrdersDT;
        }

        public DataGridViewComboBoxColumn FrontsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "FrontsColumn",
                    HeaderText = "Фасад",
                    DataPropertyName = "FrontID",
                    DataSource = new DataView(FrontsDataTable),
                    ValueMember = "FrontID",
                    DisplayMember = "FrontName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn FrameColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "FrameColorsColumn",
                    HeaderText = "Цвет\r\nпрофиля",
                    DataPropertyName = "ColorID",
                    DataSource = new DataView(FrameColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PatinaColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn",
                    HeaderText = "Патина",
                    DataPropertyName = "PatinaID",
                    DataSource = new DataView(PatinaDataTable),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn InsetTypesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "InsetTypesColumn",
                    HeaderText = "Тип\r\nнаполнителя",
                    DataPropertyName = "InsetTypeID",
                    DataSource = new DataView(InsetTypesDataTable),
                    ValueMember = "InsetTypeID",
                    DisplayMember = "InsetTypeName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn InsetColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "InsetColorsColumn",
                    HeaderText = "Цвет\r\nнаполнителя",
                    DataPropertyName = "InsetColorID",
                    DataSource = new DataView(InsetColorsDataTable),
                    ValueMember = "InsetColorID",
                    DisplayMember = "InsetColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnoProfilesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoProfilesColumn",
                    HeaderText = "Тип\r\nпрофиля-2",
                    DataPropertyName = "TechnoProfileID",
                    DataSource = new DataView(TechnoProfilesDataTable),
                    ValueMember = "TechnoProfileID",
                    DisplayMember = "TechnoProfileName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnoFrameColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoFrameColorsColumn",
                    HeaderText = "Цвет профиля-2",
                    DataPropertyName = "TechnoColorID",
                    DataSource = new DataView(FrameColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnoInsetTypesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoInsetTypesColumn",
                    HeaderText = "Тип наполнителя-2",
                    DataPropertyName = "TechnoInsetTypeID",
                    DataSource = new DataView(TechnoInsetTypesDataTable),
                    ValueMember = "InsetTypeID",
                    DisplayMember = "InsetTypeName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnoInsetColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoInsetColorsColumn",
                    HeaderText = "Цвет наполнителя-2",
                    DataPropertyName = "TechnoInsetColorID",
                    DataSource = new DataView(TechnoInsetColorsDataTable),
                    ValueMember = "InsetColorID",
                    DisplayMember = "InsetColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public bool SetConfigID(DataRow Row, ref int FactoryID)
        {
            int FrontID = Convert.ToInt32(Row["FrontID"]);
            int PatinaID = Convert.ToInt32(Row["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(Row["InsetTypeID"]);
            int ColorID = Convert.ToInt32(Row["ColorID"]);
            int InsetColorID = Convert.ToInt32(Row["InsetColorID"]);
            int TechnoProfileID = Convert.ToInt32(Row["TechnoProfileID"]);
            int TehcnoColorID = Convert.ToInt32(Row["TehcnoColorID"]);
            int TechnoInsetTypeID = Convert.ToInt32(Row["TechnoInsetTypeID"]);
            int TechnoInsetColorID = Convert.ToInt32(Row["TechnoInsetColorID"]);
            int Height = Convert.ToInt32(Row["Height"]);
            int Width = Convert.ToInt32(Row["Width"]);

            int F = -1;
            int AreaID = 0;
            Row["FrontConfigID"] = FrontsCatalogOrder.GetFrontConfigID(FrontID, ColorID, PatinaID, InsetTypeID,
                InsetColorID, TechnoProfileID, TehcnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref F, ref AreaID);
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

        public void AddOrder()
        {
            if (FrontsOrdersBS.Count == 0)
                return;

            string MainOrderID = ((DataRowView)FrontsOrdersBS.Current).Row["MainOrderID"].ToString();
            int FrontID = Convert.ToInt32(((DataRowView)FrontsOrdersBS.Current).Row["FrontID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)FrontsOrdersBS.Current).Row["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)FrontsOrdersBS.Current).Row["InsetTypeID"]);
            int ColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBS.Current).Row["ColorID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBS.Current).Row["InsetColorID"]);
            int TechnoInsetTypeID = Convert.ToInt32(((DataRowView)FrontsOrdersBS.Current).Row["TechnoInsetTypeID"]);
            int TechnoInsetColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBS.Current).Row["TechnoInsetColorID"]);

            int pos = FrontsOrdersBS.Position + 1;

            //create new blank row
            {
                DataRow NewRow = FrontsOrdersDT.NewRow();

                NewRow["MainOrderID"] = MainOrderID;
                NewRow["FrontID"] = FrontID;
                NewRow["PatinaID"] = PatinaID;
                NewRow["InsetTypeID"] = InsetTypeID;
                NewRow["ColorID"] = ColorID;
                NewRow["InsetColorID"] = InsetColorID;
                NewRow["TechnoInsetTypeID"] = TechnoInsetTypeID;
                NewRow["TechnoInsetColorID"] = TechnoInsetColorID;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["Count"] = 1;

                FrontsOrdersDT.Rows.InsertAt(NewRow, pos);
            }
        }

        public void RemoveOrder()
        {
            if (FrontsOrdersBS.Current != null)
            {
                FrontsOrdersBS.RemoveCurrent();
            }
        }

        public bool PreSaveFrontOrder(int MainOrderID, ref int FactoryID)
        {
            if (FrontsOrdersDT.Rows.Count < 1)
                return true;

            for (int i = 0; i < FrontsOrdersDT.Rows.Count; i++)
            {
                if (FrontsOrdersDT.Rows[i].RowState == DataRowState.Deleted)
                    continue;

                if (FrontsOrdersDT.Rows[i]["Height"].ToString() == "0" || FrontsOrdersDT.Rows[i]["Width"].ToString() == "0")
                {
                    MessageBox.Show("Неверный размер: 0");
                    return false;
                }

                if (SetConfigID(FrontsOrdersDT.Rows[i], ref FactoryID) == false)
                {
                    MessageBox.Show("Невозможно сохранить заказ, так как одна или несколько позиций фасадов\r\n" +
                        "отсутствует в каталоге. Проверьте правильность ввода данных в позиции " + (i + 1).ToString(),
                        "Ошибка сохранения заказа");
                    return false;
                }
            }
            return true;
        }

        public void SaveFrontsOrder()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(FrontsOrdersDT);
                }
            }
        }

        public void GetFrontsOrders(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDT);
            }
        }

        public bool ColorRow(int FrontID, int ColorID, int PatinaID, int InsetTypeID,
           int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int Height, int Width, int Count)
        {
            bool b = true;

            DataRow[] Rows = FrontsOrdersDT.Select("FrontID=" + FrontID +
                " AND PatinaID=" + PatinaID +
                " AND InsetTypeID=" + InsetTypeID +
                " AND ColorID=" + ColorID + " AND InsetColorID=" + InsetColorID +
                " AND TechnoInsetTypeID=" + TechnoInsetTypeID + " AND TechnoInsetColorID=" + TechnoInsetColorID + " AND Height=" + Height +
                " AND Width=" + Width + " AND Count=" + Count);
            if (Rows.Count() > 0)
                b = false;

            return b;
        }
    }








    public class DoubleDecorOrders
    {
        public bool Debts = false;

        private DataTable DecorOrdersDT = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable ColorsDataTable = null;
        private DataTable PatinaDataTable = null;

        public BindingSource DecorOrdersBS = null;
        private BindingSource ProductsBS = null;
        private BindingSource DecorBS = null;
        private BindingSource ColorsBS = null;

        DecorCatalogOrder DecorCatalogOrder;

        public DoubleDecorOrders(ref DecorCatalogOrder tDecorCatalogOrder)
        {
            DecorCatalogOrder = tDecorCatalogOrder;
        }

        public DataTable GetDecorOrdersDT
        {
            get
            {
                DataTable DT = new DataTable();
                using (DataView DV = new DataView(DecorOrdersDT))
                {
                    DT = DV.ToTable(false, "ProductID", "DecorID", "ColorID",
                        "Length", "Height", "Width", "Count");
                }
                return DT;
            }
        }

        public bool HasDecor => DecorOrdersBS.Count > 0;

        public BindingSource OldDecorOrdersList => DecorOrdersBS;

        public DataTable CurrentDecorOrdersDT
        {
            get
            {
                DataTable DT = new DataTable();
                using (DataView DV = new DataView(DecorOrdersDT))
                {
                    DT = DV.ToTable(false, "ProductID", "DecorID", "ColorID",
                    "Length", "Height", "Width", "Count");
                }
                return DT;
            }
        }

        private void Create()
        {
            //NewDecorOrdersDT = new DataTable();
            DecorOrdersDT = new DataTable();
            ProductsDataTable = new DataTable();
            DecorDataTable = new DataTable();
            ColorsDataTable = new DataTable();

            //NewDecorOrdersBS = new BindingSource();
            DecorOrdersBS = new BindingSource();
            ProductsBS = new BindingSource();
            DecorBS = new BindingSource();
            ColorsBS = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1   ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            GetColorsDT();
            PatinaDataTable = new DataTable();
            SelectCommand = @"SELECT * FROM Patina ORDER BY PatinaName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
        }

        private void GetColorsDT()
        {
            ColorsDataTable = new DataTable();
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

        private void Binding()
        {
            ProductsBS.DataSource = ProductsDataTable;
            //NewDecorOrdersBS.DataSource = NewDecorOrdersDT;
            DecorOrdersBS.DataSource = DecorOrdersDT;
            DecorBS.DataSource = DecorDataTable;
            ColorsBS.DataSource = ColorsDataTable;
        }

        public DataGridViewComboBoxColumn CreateInsetTypesColumn()
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

        public DataGridViewComboBoxColumn CreateInsetColorsColumn()
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

        public DataGridViewComboBoxColumn ProductColumn
        {
            get
            {
                DataGridViewComboBoxColumn ProductColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ProductColumn",
                    HeaderText = "Продукт",
                    DataPropertyName = "ProductID",

                    DataSource = new DataView(ProductsDataTable),
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

                    DataSource = new DataView(ColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ColorsColumn;
            }
        }

        public DataGridViewComboBoxColumn PatinaColumn
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

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public bool SetDecorConfigID(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID,
            int Length, int Height, int Width, DataRow Row, ref int FactoryID)
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

        public void AddOrder()
        {
            if (DecorOrdersBS.Count == 0)
                return;

            string MainOrderID = ((DataRowView)DecorOrdersBS.Current).Row["MainOrderID"].ToString();
            int ProductID = Convert.ToInt32(((DataRowView)DecorOrdersBS.Current).Row["ProductID"]);
            int DecorID = Convert.ToInt32(((DataRowView)DecorOrdersBS.Current).Row["DecorID"]);
            int ColorID = Convert.ToInt32(((DataRowView)DecorOrdersBS.Current).Row["ColorID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)DecorOrdersBS.Current).Row["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)DecorOrdersBS.Current).Row["InsetTypeID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)DecorOrdersBS.Current).Row["InsetColorID"]);

            int pos = DecorOrdersBS.Position + 1;

            //create new blank row
            {
                DataRow NewRow = DecorOrdersDT.NewRow();

                NewRow["MainOrderID"] = MainOrderID;
                NewRow["ProductID"] = ProductID;
                NewRow["DecorID"] = DecorID;
                NewRow["ColorID"] = ColorID;
                NewRow["PatinaID"] = PatinaID;
                NewRow["InsetTypeID"] = InsetTypeID;
                NewRow["ColorID"] = ColorID;
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["Count"] = 1;

                DecorOrdersDT.Rows.InsertAt(NewRow, pos);
            }
        }

        public void RemoveOrder()
        {
            if (DecorOrdersBS.Current != null)
            {
                DecorOrdersBS.RemoveCurrent();
            }
        }

        public void ClearOrders()
        {
            DecorOrdersDT.Clear();
        }

        public void ChangeMainOrder(int MainOrderID)
        {
            for (int i = 0; i < DecorOrdersDT.Rows.Count; i++)
            {
                DecorOrdersDT.Rows[i]["MainOrderID"] = MainOrderID;
            }
        }

        public bool PreSaveDecorOrder(int MainOrderID, ref int FactoryID)
        {
            if (DecorOrdersDT.Rows.Count < 1)
                return true;

            for (int i = 0; i < DecorOrdersDT.Rows.Count; i++)
            {
                if (DecorOrdersDT.Rows[i].RowState != DataRowState.Deleted)
                {
                    if (SetDecorConfigID(Convert.ToInt32(DecorOrdersDT.Rows[i]["ProductID"]), Convert.ToInt32(DecorOrdersDT.Rows[i]["DecorID"]),
                        Convert.ToInt32(DecorOrdersDT.Rows[i]["ColorID"]), Convert.ToInt32(DecorOrdersDT.Rows[i]["PatinaID"]),
                        Convert.ToInt32(DecorOrdersDT.Rows[i]["InsetTypeID"]), Convert.ToInt32(DecorOrdersDT.Rows[i]["InsetColorID"]),
                        Convert.ToInt32(DecorOrdersDT.Rows[i]["Length"]), Convert.ToInt32(DecorOrdersDT.Rows[i]["Height"]), Convert.ToInt32(DecorOrdersDT.Rows[i]["Width"]), DecorOrdersDT.Rows[i], ref FactoryID) == false)
                        return false;

                }
            }
            return true;
        }

        public void SaveDecorOrder()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DecorOrdersDT);
                }
            }
        }

        public void GetDecorOrders(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDT);

                foreach (DataRow Row in DecorOrdersDT.Rows)
                {
                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                    //    Row["ColorID"] = 0;
                }
            }
        }

        public bool ColorRow(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, int Count)
        {
            bool b = true;

            DataRow[] Rows = DecorOrdersDT.Select("ProductID=" + ProductID +
                " AND DecorID=" + DecorID + " AND ColorID=" + ColorID + " AND PatinaID=" + PatinaID +
                " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID +
                " AND Length=" + Length + " AND Height=" + Height +
                " AND Width=" + Width + " AND Count=" + Count);
            if (Rows.Count() > 0)
                b = false;

            return b;
        }

    }







    public class MainOrdersFrontsOrders
    {
        private PercentageDataGrid DoubleFrontsOrdersDataGrid = null;
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentMainOrderID = -1;

        public bool Debts = false;

        public DataTable DoubleFrontsOrdersDataTable = null;
        public DataTable FrontsOrdersDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        public DataTable MeasuresDataTable = null;
        public DataTable InsetMarginsDataTable = null;

        public BindingSource DoubleFrontsOrdersBindingSource = null;
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

        public MainOrdersFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            FrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;

        }

        public bool HasFrontsOrders => FrontsOrdersBindingSource.Count > 0;

        public BindingSource FrontsOrdersList => FrontsOrdersBindingSource;

        public bool HasDFrontsOrders => DoubleFrontsOrdersBindingSource.Count > 0;

        public BindingSource DFrontsOrdersList => DoubleFrontsOrdersBindingSource;

        public void DoubleOrderInitialize(bool ShowPrice, ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            DoubleFrontsOrdersDataTable = new DataTable();
            DoubleFrontsOrdersBindingSource = new BindingSource()
            {
                DataSource = DoubleFrontsOrdersDataTable
            };
            DoubleOrderGridSettings(ShowPrice, ref tMainOrdersFrontsOrdersDataGrid);
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


            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.ZOVOrdersConnectionString))
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

        private void DoubleOrderGridSettings(bool ShowPrice, ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            DoubleFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;

            ////создание столбцов
            //DoubleFrontsColumn = new DataGridViewComboBoxColumn();
            //DoubleFrontsColumn.Name = "FrontsColumn";
            //DoubleFrontsColumn.HeaderText = "Фасад";
            //DoubleFrontsColumn.DataPropertyName = "FrontID";
            //DoubleFrontsColumn.DataSource = FrontsBindingSource;
            //DoubleFrontsColumn.ValueMember = "FrontID";
            //DoubleFrontsColumn.DisplayMember = "FrontName";
            //DoubleFrontsColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //DoublePatinaColumn = new DataGridViewComboBoxColumn();
            //DoublePatinaColumn.Name = "PatinaColumn";
            //DoublePatinaColumn.HeaderText = "Патина";
            //DoublePatinaColumn.DataPropertyName = "PatinaID";
            //DoublePatinaColumn.DataSource = PatinaBindingSource;
            //DoublePatinaColumn.ValueMember = "PatinaID";
            //DoublePatinaColumn.DisplayMember = "PatinaName";
            //DoublePatinaColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //DoubleInsetTypesColumn = new DataGridViewComboBoxColumn();
            //DoubleInsetTypesColumn.Name = "InsetTypesColumn";
            //DoubleInsetTypesColumn.HeaderText = "Тип\r\nнаполнителя";
            //DoubleInsetTypesColumn.DataPropertyName = "InsetTypeID";
            //DoubleInsetTypesColumn.DataSource = InsetTypesBindingSource;
            //DoubleInsetTypesColumn.ValueMember = "InsetTypeID";
            //DoubleInsetTypesColumn.DisplayMember = "InsetTypeName";
            //DoubleInsetTypesColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //DoubleFrameColorsColumn = new DataGridViewComboBoxColumn();
            //DoubleFrameColorsColumn.Name = "FrameColorsColumn";
            //DoubleFrameColorsColumn.HeaderText = "Цвет\r\nпрофиля";
            //DoubleFrameColorsColumn.DataPropertyName = "ColorID";
            //DoubleFrameColorsColumn.DataSource = FrameColorsBindingSource;
            //DoubleFrameColorsColumn.ValueMember = "ColorID";
            //DoubleFrameColorsColumn.DisplayMember = "ColorName";
            //DoubleFrameColorsColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //DoubleInsetColorsColumn = new DataGridViewComboBoxColumn();
            //DoubleInsetColorsColumn.Name = "InsetColorsColumn";
            //DoubleInsetColorsColumn.HeaderText = "Цвет\r\nнаполнителя";
            //DoubleInsetColorsColumn.DataPropertyName = "InsetColorID";
            //DoubleInsetColorsColumn.DataSource = InsetColorsBindingSource;
            //DoubleInsetColorsColumn.ValueMember = "InsetColorID";
            //DoubleInsetColorsColumn.DisplayMember = "InsetColorName";
            //DoubleInsetColorsColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //DoubleTechnoFrameColorsColumn = new DataGridViewComboBoxColumn();
            //DoubleTechnoFrameColorsColumn.Name = "TechnoFrameColorsColumn";
            //DoubleTechnoFrameColorsColumn.HeaderText = "Цвет профиля-2";
            //DoubleTechnoFrameColorsColumn.DataPropertyName = "TechnoColorID";
            //DoubleTechnoFrameColorsColumn.DataSource = TechnoFrameColorsBindingSource;
            //DoubleTechnoFrameColorsColumn.ValueMember = "ColorID";
            //DoubleTechnoFrameColorsColumn.DisplayMember = "ColorName";
            //DoubleTechnoFrameColorsColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //DoubleTechnoInsetTypesColumn = new DataGridViewComboBoxColumn();
            //DoubleTechnoInsetTypesColumn.Name = "TechnoInsetTypesColumn";
            //DoubleTechnoInsetTypesColumn.HeaderText = "Тип наполнителя-2";
            //DoubleTechnoInsetTypesColumn.DataPropertyName = "TechnoInsetTypeID";
            //DoubleTechnoInsetTypesColumn.DataSource = TechnoInsetTypesBindingSource;
            //DoubleTechnoInsetTypesColumn.ValueMember = "InsetTypeID";
            //DoubleTechnoInsetTypesColumn.DisplayMember = "InsetTypeName";
            //DoubleTechnoInsetTypesColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //DoubleTechnoInsetColorsColumn = new DataGridViewComboBoxColumn();
            //DoubleTechnoInsetColorsColumn.Name = "TechnoInsetColorsColumn";
            //DoubleTechnoInsetColorsColumn.HeaderText = "Цвет наполнителя-2";
            //DoubleTechnoInsetColorsColumn.DataPropertyName = "TechnoInsetColorID";
            //DoubleTechnoInsetColorsColumn.DataSource = TechnoInsetColorsBindingSource;
            //DoubleTechnoInsetColorsColumn.ValueMember = "InsetColorID";
            //DoubleTechnoInsetColorsColumn.DisplayMember = "InsetColorName";
            //DoubleTechnoInsetColorsColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            DoubleFrontsOrdersDataGrid.AutoGenerateColumns = false;

            //добавление столбцов
            DoubleFrontsOrdersDataGrid.Columns.Add(FrontsColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(PatinaColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(InsetTypesColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(FrameColorsColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(InsetColorsColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(TechnoProfilesColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(TechnoFrameColorsColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(TechnoInsetTypesColumn);
            DoubleFrontsOrdersDataGrid.Columns.Add(TechnoInsetColorsColumn);

            //убирание лишних столбцов
            if (DoubleFrontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                DoubleFrontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                DoubleFrontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                DoubleFrontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                DoubleFrontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (DoubleFrontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                DoubleFrontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (DoubleFrontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                DoubleFrontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            if (DoubleFrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                DoubleFrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;

            if (DoubleFrontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                DoubleFrontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (DoubleFrontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                DoubleFrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;


            if (!Security.PriceAccess || !ShowPrice)
            {
                DoubleFrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                DoubleFrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                DoubleFrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            DoubleFrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            if (!Debts)
            {
                DoubleFrontsOrdersDataGrid.Columns["Debt"].Visible = false;

            }
            else
            {
                DoubleFrontsOrdersDataGrid.Columns["Count"].Visible = false;
                DoubleFrontsOrdersDataGrid.Columns["Debt"].HeaderText = "Кол-во";
            }

            DoubleFrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            DoubleFrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;

            int DisplayIndex = 0;
            DoubleFrontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            DoubleFrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;

            foreach (DataGridViewColumn Column in DoubleFrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            DoubleFrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
            DoubleFrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            DoubleFrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            DoubleFrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            DoubleFrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            DoubleFrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            DoubleFrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            DoubleFrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\nфасад";
            DoubleFrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            DoubleFrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";

            DoubleFrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DoubleFrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            DoubleFrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["Height"].Width = 85;
            DoubleFrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["Width"].Width = 85;
            DoubleFrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["Count"].Width = 85;
            DoubleFrontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["Cost"].Width = 120;
            DoubleFrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["Square"].Width = 100;
            DoubleFrontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["FrontPrice"].Width = 85;
            DoubleFrontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["InsetPrice"].Width = 85;
            DoubleFrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DoubleFrontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            DoubleFrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DoubleFrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
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

            if (!Debts)
            {
                FrontsOrdersDataGrid.Columns["Debt"].Visible = false;

            }
            else
            {
                FrontsOrdersDataGrid.Columns["Count"].Visible = false;
                FrontsOrdersDataGrid.Columns["Debt"].HeaderText = "Кол-во";
            }

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
            FrontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsSample"].Width = 85;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
        }

        public void ShowCalculation(bool Show)
        {
            FrontsOrdersDataGrid.Columns["Cost"].Visible = Show;
            FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = Show;
            FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = Show;
            FrontsOrdersDataGrid.Columns["Square"].Visible = Show;
            FrontsOrdersDataGrid.Columns["Notes"].Visible = !Show;
        }

        public void Initialize(bool ShowPrice)
        {
            Create();
            Fill();
            Binding();
            CreateColumns(ShowPrice);
        }

        public bool Filter(int MainOrderID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return FrontsOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
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
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public bool FilterDebts(int MainOrderID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return FrontsOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                                                          " AND Debt > 0", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public void ClearOrders()
        {
            FrontsOrdersDataTable.Clear();
            CurrentMainOrderID = -1;
        }

        public void DeleteOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM FrontsOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }

        private DataTable GetFrontsForCompare(int MainOrderID)
        {
            string SelectCommand = @"SELECT FrontID, PatinaID, InsetTypeID, ColorID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, CupboardString, Height, Width, Count, 
                      FrontConfigID, FactoryID, Notes FROM FrontsOrders WHERE MainOrderID = " + MainOrderID;
            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DT);
            }
            return DT;
        }

        public bool FindDifferenceBetweenFronts(int MainOrderID)
        {
            DataTable OldFrontsTable = GetFrontsForCompare(MainOrderID);
            DataTable NewFrontsTable = new DataTable();
            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                NewFrontsTable = DV.ToTable(false, "FrontID", "PatinaID", "InsetTypeID", "ColorID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID",
                    "CupboardString", "Height", "Width", "Count", "FrontConfigID", "FactoryID", "Notes");
            }
            return AreTablesEquals(OldFrontsTable, NewFrontsTable);
        }

        public static bool AreTablesEquals(DataTable First, DataTable Second)
        {
            //Create Empty Table
            DataTable table = new DataTable("Difference");

            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });
                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }
                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r);

                //Create columns for return table
                for (int i = 0; i < First.Columns.Count; i++)
                {
                    table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);
                }

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();
                foreach (DataRow parentrow in ds.Tables[0].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r);
                    if (childrows == null || childrows.Length == 0)
                        table.LoadDataRow(parentrow.ItemArray, true);
                }

                //If Second Row not in First, Add to return table.
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetParentRows(r);
                    if (childrows == null || childrows.Length == 0)
                        table.LoadDataRow(parentrow.ItemArray, true);
                }
                table.EndLoadData();
            }

            return table.Rows.Count == 0;
        }
    }







    public class MainOrdersDecorOrders
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
        public MainOrdersDecorOrders(ref DevExpress.XtraTab.XtraTabControl tDecorTabControl,
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
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.ZOVOrdersConnectionString);
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

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID.ToString(),
                                                            ConnectionStrings.ZOVOrdersConnectionString);
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
                ConnectionStrings.ZOVOrdersConnectionString);
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

        public bool FilterDebts(int MainOrderID)
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

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID.ToString() + " AND Debt > 0",
                                                            ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

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

            ShowTabs();

            return true;
        }

        public void ClearOrders()
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                for (int r = 0; r < DecorItemOrdersDataTables[i].Rows.Count; r++)
                    DecorItemOrdersDataTables[r].Clear();
            }
            CurrentMainOrderID = -1;
        }

        public void DeleteOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM DecorOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }
    }









    public class NewOrderInfo
    {
        public string DocNumber;
        public string DebtDocNumber;

        public bool MovePrepare;

        public bool IsDoubleOrder;

        public bool DateEnabled;
        public bool IsEditOrder;
        public bool IsNewOrder;
        public bool IsDebt;
        public bool IsSample;
        public bool IsPrepare;
        public bool NeedCalculate;
        public bool DoNotDispatch;
        public bool TechDrilling;
        public bool QuicklyOrder;
        public bool ToAssembly;
        public bool IsNotPaid;

        public int ClientID;
        public int DebtType;
        public int MainOrderID;
        public int PriceType;

        public OrderStatusType OrderStatus;

        public object DocDateTime;
        public object DispatchDate;
        public object ProductionDate;

        public string Notes;

        public bool bPressOK;

        public enum OrderStatusType
        {
            NewOrder = -1,
            EditOrder = 0,
            PrepareOrder = 1
        }
    }




    public class Log
    {
        private static object sync = new object();
        public static void Write(Exception ex)
        {
            try
            {
                // Путь .\\Log
                string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
                AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n",
                DateTime.Now, ex.TargetSite.DeclaringType, ex.TargetSite.Name, ex.Message);
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                }
            }
            catch
            {
                // Перехватываем все и ничего не делаем
            }
        }
    }


    public class FrontsOrders
    {
        public bool HasExcluzive = false;
        int SubClientID = 0;
        private PercentageDataGrid FrontsOrdersDataGrid = null;
        private PercentageDataGrid CupboardsDataGrid = null;

        public FrontsCatalogOrder FrontsCatalogOrder = null;

        public bool IsNewCupboardsAdded = false;
        public bool bSaveCupboards = false;
        public bool bCupboardExist = false;

        DataTable OldFrontsTable;
        DataTable NewFrontsTable;
        //DataTable DifferenceFrontsTable;

        public DataTable ExcluziveDataTable = null;
        public DataTable FrontsOrdersDataTable = null;
        public DataTable TempFrontsOrdersDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;
        public DataTable TechnoInsetTypesDataTable = null;
        public DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        public DataTable CupboardsDataTable = null;
        public DataTable FrontsDrillTypesDataTable = null;

        public BindingSource FrontsOrdersBindingSource = null;
        private BindingSource FrontsBindingSource = null;
        private BindingSource FrameColorsBindingSource = null;
        private BindingSource PatinaBindingSource = null;
        private BindingSource InsetTypesBindingSource = null;
        private BindingSource InsetColorsBindingSource = null;
        private BindingSource TechnoFrameColorsBindingSource = null;
        private BindingSource TechnoInsetTypesBindingSource = null;
        private BindingSource TechnoInsetColorsBindingSource = null;
        public BindingSource CupboardsBindingSource = null;

        private SqlDataAdapter FrontsOrdersDataAdapter = null;
        private SqlCommandBuilder FrontsOrdersCommandBuider = null;

        //private SqlDataAdapter CupboardsDataAdapter = null;
        //private SqlCommandBuilder CupboardsCommandBuider = null;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;
        private DataGridViewComboBoxColumn DrillTypesColumn = null;

        private DataGridViewComboBoxColumn CupboardsInsetTypesColumn = null;

        public FrontsOrders(ref PercentageDataGrid tFrontsOrdersDataGrid,
                            ref FrontsCatalogOrder tFrontsCatalogOrder)
        {
            FrontsOrdersDataGrid = tFrontsOrdersDataGrid;

            FrontsCatalogOrder = tFrontsCatalogOrder;
        }

        public bool HasFronts => FrontsOrdersDataTable.Rows.Count > 0;

        public class ExcluziveCatalog
        {
            public int ClientID { get; set; }
            public int SubClientID { get; set; }
            public int FrontConfigID { get; set; }
            public int FrontID { get; set; }
            public int ColorID { get; set; }
            public int InsetTypeID { get; set; }
            public int InsetColorID { get; set; }
            public int PatinaID { get; set; }
        }

        List<ExcluziveCatalog> excluziveCatalogList;

        public void ExcluziveCatalogList()
        {
            excluziveCatalogList = new List<ExcluziveCatalog>();
            for (int i = 0; i < ExcluziveDataTable.Rows.Count; i++)
            {
                ExcluziveCatalog excluziveCatalog = new ExcluziveCatalog
                {
                    ClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["ClientID"]),
                    SubClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["SubClientID"]),
                    FrontConfigID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["FrontConfigID"]),
                    FrontID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["FrontID"]),
                    ColorID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["ColorID"]),
                    InsetTypeID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["InsetTypeID"]),
                    InsetColorID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["InsetColorID"]),
                    PatinaID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["PatinaID"])
                };
                excluziveCatalogList.Add(excluziveCatalog);
            }
        }

        //public void HasClientExcluzive(int iSubClientID)
        //{
        //    string SelectCommand = string.Empty;
        //    SubClientID = iSubClientID;
        //    ExcluziveDataTable = new DataTable();
        //    using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FrontsConfig.*, infiniu2_marketingreference.dbo.ExcluziveCatalog.ClientID,infiniu2_marketingreference.dbo.ExcluziveCatalog.SubClientID FROM FrontsConfig 
        //        INNER JOIN infiniu2_marketingreference.dbo.ExcluziveCatalog ON FrontsConfig.FrontConfigID=infiniu2_marketingreference.dbo.ExcluziveCatalog.ConfigID AND ProductType=0",
        //        ConnectionStrings.CatalogConnectionString))
        //    {
        //        DA.Fill(ExcluziveDataTable);
        //    }

        //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    DataRow[] rows = ExcluziveDataTable.Select("SubClientID=0 OR SubClientID=" + SubClientID);
        //    HasExcluzive = rows.Count() > 0;
        //    for (int i = 0; i < ExcluziveDataTable.Rows.Count; i++)
        //    {
        //        int ExcluziveClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["ClientID"]);
        //        int ExcluziveSubClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["SubClientID"]);
        //        int FrontConfigID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["FrontConfigID"]);
        //        int Excluzive1 = 0;
        //        if (ExcluziveSubClientID == -1)
        //            Excluzive1 = 0;
        //        if (ExcluziveSubClientID == 0)
        //            Excluzive1 = 1;
        //        if (ExcluziveSubClientID == SubClientID)
        //        {
        //            Excluzive1 = 1;
        //        }
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + ExcluziveSubClientID + ") AND FrontConfigID=" + FrontConfigID);
        //        if (rows.Count() > 1)
        //            Excluzive1 = 1;
        //        DataRow[] rows1 = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("FrontConfigID=" + FrontConfigID);
        //        if (rows1.Count() > 0)
        //        {
        //            if (rows1[0]["Excluzive"] == DBNull.Value)
        //                rows1[0]["Excluzive"] = Excluzive1;
        //            else
        //            {
        //                if (Excluzive1 == 1)
        //                    rows1[0]["Excluzive"] = Excluzive1;
        //            }
        //        }
        //    }
        //    sw.Stop();
        //    double G = sw.Elapsed.TotalSeconds;
        //    for (int i = 0; i < FrontsCatalogOrder.ConstFrontsDataTable.Rows.Count; i++)
        //    {
        //        int FrontID = Convert.ToInt32(FrontsCatalogOrder.ConstFrontsDataTable.Rows[i]["FrontID"]);
        //        rows = ExcluziveDataTable.Select("FrontID=" + FrontID);
        //        if (rows.Count() == 0)
        //            continue;
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("FrontID=" + FrontID);
        //        int AllConfiguration = rows.Count();
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND FrontID=" + FrontID);
        //        int NotExcluziveConfiguration = rows.Count();
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + SubClientID + ") AND FrontID=" + FrontID);
        //        int ClientExcluziveConfiguration = rows.Count();
        //        if (ClientExcluziveConfiguration > 0)
        //            FrontsCatalogOrder.ConstFrontsDataTable.Rows[i]["Excluzive"] = 1;
        //        else
        //        {
        //            if (NotExcluziveConfiguration == 0)
        //                FrontsCatalogOrder.ConstFrontsDataTable.Rows[i]["Excluzive"] = 0;
        //        }
        //    }
        //    for (int i = 0; i < FrontsCatalogOrder.ConstColorsDataTable.Rows.Count; i++)
        //    {
        //        int ColorID = Convert.ToInt32(FrontsCatalogOrder.ConstColorsDataTable.Rows[i]["ColorID"]);
        //        rows = ExcluziveDataTable.Select("ColorID=" + ColorID);
        //        if (rows.Count() == 0)
        //            continue;
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("ColorID=" + ColorID);
        //        int AllConfiguration = rows.Count();
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND ColorID=" + ColorID);
        //        int NotExcluziveConfiguration = rows.Count();
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + SubClientID + ") AND ColorID=" + ColorID);
        //        int ClientExcluziveConfiguration = rows.Count();
        //        if (ClientExcluziveConfiguration > 0)
        //            FrontsCatalogOrder.ConstColorsDataTable.Rows[i]["Excluzive"] = 1;
        //        else
        //        {
        //            if (NotExcluziveConfiguration == 0)
        //                FrontsCatalogOrder.ConstColorsDataTable.Rows[i]["Excluzive"] = 0;
        //        }
        //    }
        //    for (int i = 0; i < FrontsCatalogOrder.ConstInsetTypesDataTable.Rows.Count; i++)
        //    {
        //        int InsetTypeID = Convert.ToInt32(FrontsCatalogOrder.ConstInsetTypesDataTable.Rows[i]["InsetTypeID"]);
        //        rows = ExcluziveDataTable.Select("InsetTypeID=" + InsetTypeID);
        //        if (rows.Count() == 0)
        //            continue;
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("InsetTypeID=" + InsetTypeID);
        //        int AllConfiguration = rows.Count();
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND InsetTypeID=" + InsetTypeID);
        //        int NotExcluziveConfiguration = rows.Count();
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + SubClientID + ") AND InsetTypeID=" + InsetTypeID);
        //        int ClientExcluziveConfiguration = rows.Count();
        //        if (ClientExcluziveConfiguration > 0)
        //            FrontsCatalogOrder.ConstInsetTypesDataTable.Rows[i]["Excluzive"] = 1;
        //        else
        //        {
        //            if (NotExcluziveConfiguration == 0)
        //                FrontsCatalogOrder.ConstInsetTypesDataTable.Rows[i]["Excluzive"] = 0;
        //        }
        //    }
        //    for (int i = 0; i < FrontsCatalogOrder.ConstInsetColorsDataTable.Rows.Count; i++)
        //    {
        //        int InsetColorID = Convert.ToInt32(FrontsCatalogOrder.ConstInsetColorsDataTable.Rows[i]["InsetColorID"]);
        //        rows = ExcluziveDataTable.Select("InsetColorID=" + InsetColorID);
        //        if (rows.Count() == 0)
        //            continue;
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("InsetColorID=" + InsetColorID);
        //        int AllConfiguration = rows.Count();
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND InsetColorID=" + InsetColorID);
        //        int NotExcluziveConfiguration = rows.Count();
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + SubClientID + ") AND InsetColorID=" + InsetColorID);
        //        int ClientExcluziveConfiguration = rows.Count();
        //        if (ClientExcluziveConfiguration > 0)
        //            FrontsCatalogOrder.ConstInsetColorsDataTable.Rows[i]["Excluzive"] = 1;
        //        else
        //        {
        //            if (NotExcluziveConfiguration == 0)
        //                FrontsCatalogOrder.ConstInsetColorsDataTable.Rows[i]["Excluzive"] = 0;
        //        }
        //    }
        //    for (int i = 0; i < FrontsCatalogOrder.ConstPatinaDataTable.Rows.Count; i++)
        //    {
        //        int PatinaID = Convert.ToInt32(FrontsCatalogOrder.ConstPatinaDataTable.Rows[i]["PatinaID"]);
        //        rows = ExcluziveDataTable.Select("PatinaID=" + PatinaID);
        //        if (rows.Count() == 0)
        //            continue;
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("PatinaID=" + PatinaID);
        //        int AllConfiguration = rows.Count();
        //        rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND PatinaID=" + PatinaID);
        //        int NotExcluziveConfiguration = rows.Count();
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + SubClientID + ") AND PatinaID=" + PatinaID);
        //        int ClientExcluziveConfiguration = rows.Count();
        //        if (ClientExcluziveConfiguration > 0)
        //            FrontsCatalogOrder.ConstPatinaDataTable.Rows[i]["Excluzive"] = 1;
        //        else
        //        {
        //            if (NotExcluziveConfiguration == 0)
        //                FrontsCatalogOrder.ConstPatinaDataTable.Rows[i]["Excluzive"] = 0;
        //        }
        //    }
        //}

        public void HasClientExcluzive(int iSubClientID)
        {
            string SelectCommand = string.Empty;
            SubClientID = iSubClientID;
            ExcluziveDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FrontsConfig.*, infiniu2_marketingreference.dbo.ExcluziveCatalog.ClientID,infiniu2_marketingreference.dbo.ExcluziveCatalog.SubClientID FROM FrontsConfig 
                INNER JOIN infiniu2_marketingreference.dbo.ExcluziveCatalog ON FrontsConfig.FrontConfigID=infiniu2_marketingreference.dbo.ExcluziveCatalog.ConfigID AND ProductType=0",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ExcluziveDataTable);
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            ExcluziveCatalogList();

            List<ExcluziveCatalog> catalogs = excluziveCatalogList.FindAll(item => (item.SubClientID == 0 || item.SubClientID == SubClientID));
            HasExcluzive = catalogs.Count > 0;

            for (int i = 0; i < excluziveCatalogList.Count; i++)
            {
                ExcluziveCatalog catalog = excluziveCatalogList[i];
                int ExcluziveClientID = catalog.ClientID;
                int ExcluziveSubClientID = catalog.SubClientID;
                int FrontConfigID = catalog.FrontConfigID;

                int Excluzive1 = 0;
                if (ExcluziveSubClientID == -1)
                    Excluzive1 = 0;
                if (ExcluziveSubClientID == 0)
                    Excluzive1 = 1;
                if (ExcluziveSubClientID == SubClientID)
                {
                    Excluzive1 = 1;
                }

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.FrontConfigID == FrontConfigID && (item.SubClientID == 0 || item.SubClientID == ExcluziveSubClientID)).Count > 1)
                    Excluzive1 = 1;

                //DataRow[] rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("FrontConfigID=" + FrontConfigID);
                //if (rows.Count() > 0)
                //{
                //    if (rows[0]["Excluzive"] == DBNull.Value)
                //        rows[0]["Excluzive"] = Excluzive1;
                //    else
                //    {
                //        if (Excluzive1 == 1)
                //            rows[0]["Excluzive"] = Excluzive1;
                //    }
                //}

                foreach (DataRow row in FrontsCatalogOrder.ConstFrontsConfigDataTable.Rows)
                {
                    if (Convert.ToInt32(row["FrontConfigID"]) == FrontConfigID)
                    {
                        if (row["Excluzive"] == DBNull.Value)
                            row["Excluzive"] = Excluzive1;
                        else
                        {
                            if (Excluzive1 == 1)
                                row["Excluzive"] = Excluzive1;
                        }
                        break;
                    }
                }
            }
            sw.Stop();
            double G = sw.Elapsed.TotalSeconds;
            for (int i = 0; i < FrontsCatalogOrder.ConstFrontsDataTable.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(FrontsCatalogOrder.ConstFrontsDataTable.Rows[i]["FrontID"]);

                if (excluziveCatalogList.FindAll(item => item.FrontID == FrontID).Count == 0)
                    continue;

                DataRow[] rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND FrontID=" + FrontID);
                int NotExcluziveConfiguration = rows.Count();

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.FrontID == FrontID && (item.SubClientID == 0 || item.SubClientID == SubClientID)).Count > 0)
                    FrontsCatalogOrder.ConstFrontsDataTable.Rows[i]["Excluzive"] = 1;
                else
                {
                    if (NotExcluziveConfiguration == 0)
                        FrontsCatalogOrder.ConstFrontsDataTable.Rows[i]["Excluzive"] = 0;
                }
            }
            for (int i = 0; i < FrontsCatalogOrder.ConstColorsDataTable.Rows.Count; i++)
            {
                int ColorID = Convert.ToInt32(FrontsCatalogOrder.ConstColorsDataTable.Rows[i]["ColorID"]);

                if (excluziveCatalogList.FindAll(item => item.ColorID == ColorID).Count == 0)
                    continue;

                DataRow[] rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND ColorID=" + ColorID);
                int NotExcluziveConfiguration = rows.Count();

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.ColorID == ColorID && (item.SubClientID == 0 || item.SubClientID == SubClientID)).Count > 0)
                    FrontsCatalogOrder.ConstColorsDataTable.Rows[i]["Excluzive"] = 1;
                else
                {
                    if (NotExcluziveConfiguration == 0)
                        FrontsCatalogOrder.ConstColorsDataTable.Rows[i]["Excluzive"] = 0;
                }
            }
            for (int i = 0; i < FrontsCatalogOrder.ConstInsetTypesDataTable.Rows.Count; i++)
            {
                int InsetTypeID = Convert.ToInt32(FrontsCatalogOrder.ConstInsetTypesDataTable.Rows[i]["InsetTypeID"]);

                if (excluziveCatalogList.FindAll(item => item.InsetTypeID == InsetTypeID).Count == 0)
                    continue;

                DataRow[] rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND InsetTypeID=" + InsetTypeID);
                int NotExcluziveConfiguration = rows.Count();

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.InsetTypeID == InsetTypeID && (item.SubClientID == 0 || item.SubClientID == SubClientID)).Count > 0)
                    FrontsCatalogOrder.ConstInsetTypesDataTable.Rows[i]["Excluzive"] = 1;
                else
                {
                    if (NotExcluziveConfiguration == 0)
                        FrontsCatalogOrder.ConstInsetTypesDataTable.Rows[i]["Excluzive"] = 0;
                }
            }
            for (int i = 0; i < FrontsCatalogOrder.ConstInsetColorsDataTable.Rows.Count; i++)
            {
                int InsetColorID = Convert.ToInt32(FrontsCatalogOrder.ConstInsetColorsDataTable.Rows[i]["InsetColorID"]);

                if (excluziveCatalogList.FindAll(item => item.InsetColorID == InsetColorID).Count == 0)
                    continue;

                DataRow[] rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND InsetColorID=" + InsetColorID);
                int NotExcluziveConfiguration = rows.Count();

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.InsetColorID == InsetColorID && (item.SubClientID == 0 || item.SubClientID == SubClientID)).Count > 0)
                    FrontsCatalogOrder.ConstInsetColorsDataTable.Rows[i]["Excluzive"] = 1;
                else
                {
                    if (NotExcluziveConfiguration == 0)
                        FrontsCatalogOrder.ConstInsetColorsDataTable.Rows[i]["Excluzive"] = 0;
                }
            }
            for (int i = 0; i < FrontsCatalogOrder.ConstPatinaDataTable.Rows.Count; i++)
            {
                int PatinaID = Convert.ToInt32(FrontsCatalogOrder.ConstPatinaDataTable.Rows[i]["PatinaID"]);

                if (excluziveCatalogList.FindAll(item => item.PatinaID == PatinaID).Count == 0)
                    continue;

                DataRow[] rows = FrontsCatalogOrder.ConstFrontsConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND PatinaID=" + PatinaID);
                int NotExcluziveConfiguration = rows.Count();

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.PatinaID == PatinaID && (item.SubClientID == 0 || item.SubClientID == SubClientID)).Count > 0)
                    FrontsCatalogOrder.ConstPatinaDataTable.Rows[i]["Excluzive"] = 1;
                else
                {
                    if (NotExcluziveConfiguration == 0)
                        FrontsCatalogOrder.ConstPatinaDataTable.Rows[i]["Excluzive"] = 0;
                }
            }
        }

        public DataTable CurrentFrontsOrdersDT
        {
            get
            {
                DataTable DT = new DataTable();
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DT = DV.ToTable(false, "FrontID", "PatinaID", "InsetTypeID", "ColorID", "TechnoColorID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID",
                        "Height", "Width", "Count");
                }
                return DT;
            }
        }

        private void Create()
        {
            if (OldFrontsTable == null)
                OldFrontsTable = new DataTable();
            if (NewFrontsTable == null)
                NewFrontsTable = new DataTable();

            FrontsOrdersDataTable = new DataTable();
            TempFrontsOrdersDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            FrontsOrdersBindingSource = new BindingSource();
            FrontsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            InsetTypesBindingSource = new BindingSource();
            FrameColorsBindingSource = new BindingSource();
            InsetColorsBindingSource = new BindingSource();
            TechnoFrameColorsBindingSource = new BindingSource();
            TechnoInsetTypesBindingSource = new BindingSource();
            TechnoInsetColorsBindingSource = new BindingSource();
            CupboardsBindingSource = new BindingSource();

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

        private void GetInsetTypesDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetTypes.InsetTypeID, InsetTypes.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetTypeName, InsetTypes.MeasureID FROM InsetTypes" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetTypes.InsetTypeID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetTypeID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetTypeName"] = "-";
                    NewRow["MeasureID"] = 1;
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetTypeID"] = 1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetTypeName"] = "Витрина";
                    NewRow["MeasureID"] = 1;
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetTypeID"] = 2;
                    NewRow["GroupID"] = 2;
                    NewRow["InsetTypeName"] = "Стекло";
                    NewRow["MeasureID"] = 1;
                    InsetColorsDataTable.Rows.Add(NewRow);
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
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
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


            FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.ZOVOrdersConnectionString);
            FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
            FrontsOrdersDataAdapter.Fill(FrontsOrdersDataTable);

            CupboardsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Cupboards",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(CupboardsDataTable);
            }


            SelectCommand = @"SELECT TOP 0 FrontID, PatinaID, InsetTypeID, ColorID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, SUM(Count) AS Count
                FROM FrontsOrders GROUP BY FrontID, PatinaID, InsetTypeID, ColorID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                OldFrontsTable = new DataTable();
                NewFrontsTable = new DataTable();
                DA.Fill(OldFrontsTable);
                NewFrontsTable = OldFrontsTable.Clone();
            }

            FrontsDrillTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsDrillTypes", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDrillTypesDataTable);
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
            CupboardsBindingSource.DataSource = CupboardsDataTable;
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
            DrillTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DrillTypesColumn",
                HeaderText = "Тип сверления",
                DataPropertyName = "FrontDrillTypeID",
                DataSource = new DataView(FrontsDrillTypesDataTable),
                ValueMember = "FrontDrillTypeID",
                DisplayMember = "FrontDrillType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };

            //cupboards
            CupboardsInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип наполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = InsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 3
            };
            FrontsOrdersDataGrid.AutoGenerateColumns = false;


            //добавление столбцов
            FrontsOrdersDataGrid.Columns.Add(FrontsColumn);
            FrontsOrdersDataGrid.Columns.Add(PatinaColumn);
            FrontsOrdersDataGrid.Columns.Add(FrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(DrillTypesColumn);

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
            FrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            //if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
            //    FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            FrontsOrdersDataGrid.Columns["Square"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Visible = false;
            FrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            FrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            FrontsOrdersDataGrid.Columns["Debt"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;

            FrontsOrdersDataGrid.Columns["IsNonStandard"].ReadOnly = true;

            //названия столбцов
            FrontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образец";
            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";
            FrontsOrdersDataGrid.Columns["AlHandsSize"].HeaderText = "Ручки, мм";
            FrontsOrdersDataGrid.Columns["ImpostMargin"].HeaderText = "Смещение\r\nимпоста";

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
            FrontsOrdersDataGrid.Columns["AlHandsSize"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["DrillTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Cost"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Square"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["FrontPrice"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetPrice"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["ImpostMargin"].DisplayIndex = DisplayIndex++;
            //FrontsOrdersDataGrid.Columns["AlHandsSize"].DisplayIndex = 12;
            //FrontsOrdersDataGrid.Columns["AlHandsSize"].DisplayIndex = 13;
            //FrontsOrdersDataGrid.Columns["AlHandsSize"].DisplayIndex = 14;
            //FrontsOrdersDataGrid.Columns["AlHandsSize"].DisplayIndex = 15;
            //FrontsOrdersDataGrid.Columns["AlHandsSize"].DisplayIndex = 16;

            FrontsOrdersDataGrid.Columns["AlHandsSize"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrontsOrdersDataGrid.Columns["AlHandsSize"].MinimumWidth = 40;
            FrontsOrdersDataGrid.Columns["DrillTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrontsOrdersDataGrid.Columns["DrillTypesColumn"].MinimumWidth = 140;
            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Height"].Width = 85;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Width"].Width = 85;
            FrontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsSample"].Width = 80;
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
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
            FrontsOrdersDataGrid.Columns["ImpostMargin"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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

        public void AddFrontsOrder(int MainOrderID, int FrontID, int ColorID, int PatinaID, int InsetTypeID,
            int InsetColorID, int TechnoProfileID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int Height, int Width, int Count, decimal HandsSize, int FrontDrillTypeID, string Notes, bool IsSample, int ImpostMargin = 0)
        {
            //int FactoryID = 1;
            //FrontsCatalogOrder.GetFrontConfigID(FrontID, ColorID, PatinaID, InsetTypeID,
            //       InsetColorID, TehcnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref FactoryID);
            if (Width != -1 && FrontID == 3630 && Width > 1478)
            {
                MessageBox.Show("Ширина фасада Марсель 3 не может быть больше 1478 мм", "Добавление фасада");
                return;
            }
            if (Width != -1 && (Height < 110 || Width < 110) && (FrontID != 3727 && FrontID != 3728 && FrontID != 3729 &&
                     FrontID != 3730 && FrontID != 3731 && FrontID != 3732 && FrontID != 3733 && FrontID != 3734 &&
                     FrontID != 3735 && FrontID != 3736 && FrontID != 3737 && FrontID != 27914 && FrontID != 29597 && FrontID != 3739 && FrontID != 3740 &&
                     FrontID != 3741 && FrontID != 3742 && FrontID != 3743 && FrontID != 3744 && FrontID != 3745 &&
                     FrontID != 3746 && FrontID != 3747 && FrontID != 3748 && FrontID != 15108 && FrontID != 3662 && FrontID != 3663 && FrontID != 3664 &&
                     FrontID != 16269 && FrontID != 28945 &&
                     FrontID != 16579 && FrontID != 16580 && FrontID != 16581 && FrontID != 16582 && FrontID != 16583 && FrontID != 16584 &&
                     FrontID != 29277 && FrontID != 29278 &&
                     FrontID != 15107 && FrontID != 15759 && FrontID != 15760 && FrontID != 27437 && FrontID != 27913 && FrontID != 29598))
            {
                MessageBox.Show("Высота и ширина фасада не могут быть меньше 110 мм", "Добавление фасада");
                return;
            }
            if (Width != -1 && (Height < 30 || Width < 30) && (FrontID == 3727 || FrontID == 3728 || FrontID == 3729 ||
                 FrontID == 3730 || FrontID == 3731 || FrontID == 3732 || FrontID == 3733 || FrontID == 3734 ||
                 FrontID == 3735 || FrontID == 3736 || FrontID == 3737 || FrontID == 3739 || FrontID == 3740 ||
                 FrontID == 3741 || FrontID == 3742 || FrontID == 3743 || FrontID == 3744 || FrontID == 3745 ||
                 FrontID == 3746 || FrontID == 3747 || FrontID == 3748 || FrontID == 15108 ||
                 FrontID == 30504 || FrontID == 30505 || FrontID == 30506 ||
                 FrontID == 30364 || FrontID == 30366 || FrontID == 30367 ||
                 FrontID == 30501 || FrontID == 30502 || FrontID == 30503 ||
                 FrontID == 16269 || FrontID == 28945 || FrontID == 27914 || FrontID == 29597 ||
                 FrontID == 16579 || FrontID == 16580 || FrontID == 16581 || FrontID == 16582 || FrontID == 16583 || FrontID == 16584 ||
                 FrontID == 29277 || FrontID == 29278 ||
                 FrontID == 15107 || FrontID == 15759 || FrontID == 15760 || FrontID == 27437 || FrontID == 27913 || FrontID == 29598))
            {
                MessageBox.Show("Высота и ширина фасада не могут быть меньше 30 мм", "Добавление фасада");
                return;
            }
            if ((Height < 570 || Width < 396) && (FrontID == 3731 || FrontID == 3728 || FrontID == 3732))
            {
                MessageBox.Show("Высота апликации не может быть меньше 570 мм\r\nШирина апликации не может быть меньше 396 мм", "Добавление фасада");
                return;
            }
            DataRow row = FrontsOrdersDataTable.NewRow();
            row["IsSample"] = IsSample;
            row["CreateDateTime"] = Security.GetCurrentDate();
            row["CreateUserID"] = Security.CurrentUserID;
            row["TechnoProfileID"] = TechnoProfileID;
            row["MainOrderID"] = MainOrderID;
            row["FrontID"] = FrontID;
            row["InsetTypeID"] = InsetTypeID;
            row["PatinaID"] = PatinaID;
            row["ColorID"] = ColorID;
            row["TechnoColorID"] = TechnoColorID;
            row["InsetColorID"] = InsetColorID;
            row["TechnoInsetTypeID"] = TechnoInsetTypeID;
            row["TechnoInsetColorID"] = TechnoInsetColorID;
            row["Height"] = Height;
            row["Width"] = Width;
            row["Count"] = Count;
            row["AlHandsSize"] = HandsSize;
            row["FrontDrillTypeID"] = FrontDrillTypeID;
            row["Notes"] = Notes;
            row["CupboardString"] = "-";
            if (ImpostMargin != 0)
                row["ImpostMargin"] = ImpostMargin;

            FrontsOrdersDataTable.Rows.Add(row);

            FrontsOrdersBindingSource.MoveLast();
        }

        public void AddFrontsOrderCupboards(ref ComponentFactory.Krypton.Toolkit.KryptonListBox CupboardsExportListBox,
            int MainOrderID, int FrontID, int ColorID, int PatinaID, int InsetTypeID,
            int InsetColorID, int TechnoProfileID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int Height, int Width, int Count, bool IsSample)
        {
            DateTime CreateDateTime = Security.GetCurrentDate();
            for (int i = 0; i < CupboardsExportListBox.Items.Count; i++)
            {
                for (int j = 0; j < CupboardsDataTable.Rows.Count; j++)
                {
                    string s1 = CupboardsDataTable.Rows[j]["CupboardName"].ToString();
                    string s2 = CupboardsExportListBox.Items[i].ToString();
                    if (s1 == s2)
                    {
                        DataRow row = FrontsOrdersDataTable.NewRow();
                        row["CreateDateTime"] = CreateDateTime;
                        row["IsSample"] = IsSample;
                        row["CreateUserTypeID"] = 0;
                        row["CreateUserID"] = Security.CurrentUserID;
                        row["MainOrderID"] = MainOrderID;
                        row["FrontID"] = FrontID;
                        row["InsetTypeID"] = InsetTypeID;
                        row["TechnoProfileID"] = TechnoProfileID;

                        //если прописана вставка
                        if (CupboardsDataTable.Rows[j]["InsetTypeID"].ToString() != "")
                            row["InsetTypeID"] = Convert.ToInt32(CupboardsDataTable.Rows[j]["InsetTypeID"]);

                        //если витрина цвет наполнителя 0 (-)
                        if (row["InsetTypeID"].ToString() == "1")
                            row["InsetColorID"] = -1;
                        else
                            row["InsetColorID"] = InsetColorID;
                        row["ColorID"] = ColorID;
                        row["TechnoColorID"] = TechnoColorID;
                        row["TechnoInsetTypeID"] = TechnoInsetTypeID;
                        row["TechnoInsetColorID"] = TechnoInsetColorID;
                        row["CupboardString"] = CupboardsExportListBox.Items[i].ToString();
                        row["PatinaID"] = PatinaID;
                        row["Height"] = Convert.ToInt32(CupboardsDataTable.Rows[j]["Height"]);
                        row["Width"] = Convert.ToInt32(CupboardsDataTable.Rows[j]["Width"]);
                        row["Count"] = Convert.ToInt32(CupboardsDataTable.Rows[j]["Count"]);

                        FrontsOrdersDataTable.Rows.Add(row);

                        //если это последний шкаф в базе
                        if (j == CupboardsDataTable.Rows.Count - 1)
                            break;

                        //если следующий шкаф не пустой
                        if (CupboardsDataTable.Rows[j + 1]["CupboardName"].ToString() != "")
                            break;

                        for (int k = j + 1; k < CupboardsDataTable.Rows.Count; k++)
                        {
                            //если шкаф пустой
                            if (CupboardsDataTable.Rows[k]["CupboardName"].ToString() == "")
                            {
                                row = FrontsOrdersDataTable.NewRow();
                                row["IsSample"] = IsSample;
                                row["CreateDateTime"] = CreateDateTime;
                                row["CreateUserTypeID"] = 0;
                                row["CreateUserID"] = Security.CurrentUserID;
                                row["TechnoProfileID"] = TechnoProfileID;
                                row["MainOrderID"] = MainOrderID;
                                row["FrontID"] = FrontID;
                                row["InsetTypeID"] = InsetTypeID;
                                //если прописана вставка
                                if (CupboardsDataTable.Rows[k]["InsetTypeID"].ToString() != "")
                                    row["InsetTypeID"] = Convert.ToInt32(CupboardsDataTable.Rows[k]["InsetTypeID"]);
                                //если витрина цвет наполнителя 0 (-)
                                if (row["InsetTypeID"].ToString() == "1")
                                    row["InsetColorID"] = -1;
                                else
                                    row["InsetColorID"] = InsetColorID;
                                int F = 0;
                                int AreaID = 0;

                                row["ColorID"] = ColorID;
                                row["TechnoColorID"] = TechnoColorID;
                                row["TechnoInsetTypeID"] = TechnoInsetTypeID;
                                row["TechnoInsetColorID"] = TechnoInsetColorID;
                                row["CupboardString"] = "";
                                row["PatinaID"] = PatinaID;
                                row["Height"] = Convert.ToInt32(CupboardsDataTable.Rows[k]["Height"]);
                                row["Width"] = Convert.ToInt32(CupboardsDataTable.Rows[k]["Width"]);
                                row["Count"] = Convert.ToInt32(CupboardsDataTable.Rows[k]["Count"]);
                                row["FrontConfigID"] = FrontsCatalogOrder.GetFrontConfigID(FrontID, ColorID, PatinaID, InsetTypeID,
                                    InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref F, ref AreaID);
                                FrontsOrdersDataTable.Rows.Add(row);
                            }
                            else
                            {
                                j = CupboardsDataTable.Rows.Count;//выход из цикла поиска шкафов
                                break;
                            }
                        }
                    }
                    else if (j == CupboardsDataTable.Rows.Count - 1)
                    {
                        int F = 0;
                        int AreaID = 0;
                        DataRow row = FrontsOrdersDataTable.NewRow();
                        row["IsSample"] = IsSample;
                        row["CreateDateTime"] = CreateDateTime;
                        row["CreateUserTypeID"] = 0;
                        row["CreateUserID"] = Security.CurrentUserID;
                        row["MainOrderID"] = MainOrderID;
                        row["FrontID"] = FrontID;
                        row["InsetTypeID"] = InsetTypeID;
                        row["TechnoProfileID"] = TechnoProfileID;
                        row["PatinaID"] = PatinaID;
                        row["ColorID"] = ColorID;
                        row["InsetColorID"] = InsetColorID;
                        row["TechnoColorID"] = TechnoColorID;
                        row["TechnoInsetTypeID"] = TechnoInsetTypeID;
                        row["TechnoInsetColorID"] = TechnoInsetColorID;
                        row["CupboardString"] = CupboardsExportListBox.Items[i].ToString();
                        row["Height"] = Height;
                        row["Width"] = Width;
                        row["Count"] = Count;
                        row["FrontConfigID"] = FrontsCatalogOrder.GetFrontConfigID(FrontID, ColorID, PatinaID, InsetTypeID,
                            InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref F, ref AreaID);
                        FrontsOrdersDataTable.Rows.Add(row);
                        break;
                    }

                }


            }

        }

        public void AddFrontsOrderFromSizeTable(ref PercentageDataGrid DataGrid, int MainOrderID,
            string FrontName, int ColorID, int PatinaID, int InsetTypeID,
            int InsetColorID, int TechnoProfileID, int TechnoColorID, int TechnoInsetTypeID, int TechnoInsetColorID, bool IsSample)
        {
            if (DataGrid.Rows.Count < 1)
                return;
            int FrontID = -1;
            int Height = 0;
            int Width = 0;
            for (int i = 0; i < DataGrid.Rows.Count - 1; i++)
            {
                int FactoryID = 1;
                int AreaID = 1;
                Height = Convert.ToInt32(DataGrid.Rows[i].Cells["HeightColumn"].Value);
                Width = Convert.ToInt32(DataGrid.Rows[i].Cells["WidthColumn"].Value);
                FrontsCatalogOrder.GetFrontConfigID(FrontName, ColorID, PatinaID, InsetTypeID,
                    InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref FrontID, ref FactoryID, ref AreaID);
                if (FrontID == -1)
                    return;

                if (Width != -1 && FrontID == 3630 && Width > 1478)
                {
                    MessageBox.Show("Ширина фасада Марсель 3 не может быть больше 1478 мм", "Добавление фасада");
                    return;
                }
                if (Width != -1 && (Height < 110 || Width < 110) && (FrontID != 3727 && FrontID != 3728 && FrontID != 3729 &&
                     FrontID != 3730 && FrontID != 3731 && FrontID != 3732 && FrontID != 3733 && FrontID != 3734 &&
                     FrontID != 3735 && FrontID != 3736 && FrontID != 3737 && FrontID != 27914 && FrontID != 29597 && FrontID != 3739 && FrontID != 3740 &&
                     FrontID != 3741 && FrontID != 3742 && FrontID != 3743 && FrontID != 3744 && FrontID != 3745 &&
                     FrontID != 3746 && FrontID != 3747 && FrontID != 3748 && FrontID != 15108 && FrontID != 3662 && FrontID != 3663 && FrontID != 3664 &&
                     FrontID != 16269 && FrontID != 28945 &&
                     FrontID != 16579 && FrontID != 16580 && FrontID != 16581 && FrontID != 16582 && FrontID != 16583 && FrontID != 16584 &&
                     FrontID != 29277 && FrontID != 29278 &&
                     FrontID != 15107 && FrontID != 15759 && FrontID != 15760 && FrontID != 27437 && FrontID != 27913 && FrontID != 29598))
                {
                    MessageBox.Show("Высота и ширина фасада не могут быть меньше 110 мм", "Добавление фасада");
                    return;
                }
                if (Width != -1 && (Height < 30 || Width < 30) && (FrontID == 3727 || FrontID == 3728 || FrontID == 3729 ||
                     FrontID == 3730 || FrontID == 3731 || FrontID == 3732 || FrontID == 3733 || FrontID == 3734 ||
                     FrontID == 3735 || FrontID == 3736 || FrontID == 3737 || FrontID == 3739 || FrontID == 3740 ||
                     FrontID == 3741 || FrontID == 3742 || FrontID == 3743 || FrontID == 3744 || FrontID == 3745 ||
                     FrontID == 3746 || FrontID == 3747 || FrontID == 3748 || FrontID == 15108 ||
                     FrontID == 30504 || FrontID == 30505 || FrontID == 30506 ||
                     FrontID == 30364 || FrontID == 30366 || FrontID == 30367 ||
                     FrontID == 30501 || FrontID == 30502 || FrontID == 30503 ||
                     FrontID == 16269 || FrontID == 28945 || FrontID == 27914 || FrontID == 29597 ||
                     FrontID == 16579 || FrontID == 16580 || FrontID == 16581 || FrontID == 16582 || FrontID == 16583 || FrontID == 16584 ||
                     FrontID == 29277 || FrontID == 29278 ||
                     FrontID == 15107 || FrontID == 15759 || FrontID == 15760 || FrontID == 27437 || FrontID == 27913 || FrontID == 29598))
                {
                    MessageBox.Show("Высота и ширина фасада не могут быть меньше 30 мм", "Добавление фасада");
                    return;
                }
                if ((Height < 570 || Width < 396) && (FrontID == 3731 || FrontID == 3728 || FrontID == 3732))
                {
                    MessageBox.Show("Высота апликации не может быть меньше 570 мм\r\nШирина апликации не может быть меньше 396 мм", "Добавление фасада");
                    return;
                }
            }

            DateTime CreateDateTime = Security.GetCurrentDate();
            for (int i = 0; i < DataGrid.Rows.Count - 1; i++)
            {
                DataRow row = FrontsOrdersDataTable.NewRow();
                row["IsSample"] = IsSample;
                row["CreateDateTime"] = CreateDateTime;
                row["CreateUserTypeID"] = 0;
                row["CreateUserID"] = Security.CurrentUserID;
                row["MainOrderID"] = MainOrderID;
                row["FrontID"] = FrontID;
                row["TechnoProfileID"] = TechnoProfileID;
                row["InsetTypeID"] = InsetTypeID;
                row["PatinaID"] = PatinaID;
                row["ColorID"] = ColorID;
                row["InsetColorID"] = InsetColorID;
                row["TechnoColorID"] = TechnoColorID;
                row["TechnoInsetTypeID"] = TechnoInsetTypeID;
                row["TechnoInsetColorID"] = TechnoInsetColorID;
                row["Height"] = Convert.ToInt32(DataGrid.Rows[i].Cells["HeightColumn"].Value);
                row["Width"] = Convert.ToInt32(DataGrid.Rows[i].Cells["WidthColumn"].Value);
                row["Count"] = Convert.ToInt32(DataGrid.Rows[i].Cells["CountColumn"].Value);
                row["CupboardString"] = "-";

                FrontsOrdersDataTable.Rows.Add(row);
            }
        }

        private void AddCupboardToBase(string CupboardName, int FrontID, int InsetTypeID, int Height, int Width, int Count)
        {
            if (CupboardName == "-")
                return;

            if (CupboardName.Length > 0)
            {
                DataRow[] Rows = CupboardsDataTable.Select("CupboardName = '" + CupboardName + "'");

                if (Rows.Count() > 0)
                {
                    bCupboardExist = true;
                    //return;
                }
                else
                {
                    bCupboardExist = false;
                }
            }
            else
            {
                if (bCupboardExist)
                    return;
            }

            DataRow Row = CupboardsDataTable.NewRow();

            if (CupboardName.Length < 1)
                Row["CupboardName"] = "";
            else
                Row["CupboardName"] = CupboardName;
            //FrontID IN (3728,3731,3732,3739,3740,3741,3744,3745,3746)
            if (InsetTypeID == 1 || InsetTypeID == 28961 || InsetTypeID == 3653 || InsetTypeID == 3654 || InsetTypeID == 3655
                || InsetTypeID == 685 || InsetTypeID == 686 || InsetTypeID == 687 || InsetTypeID == 688 || InsetTypeID == 29470 || InsetTypeID == 29471
                || FrontID == 3728 || FrontID == 3731 || FrontID == 3732 || FrontID == 3739
                || FrontID == 3740 || FrontID == 3741 || FrontID == 3744 || FrontID == 3745 || FrontID == 3746)
                Row["InsetTypeID"] = InsetTypeID;
            //if (Width == -1)
            //    Row["FrontTypeID"] = 1;
            //else
            //    Row["FrontTypeID"] = 0;
            Row["FrontID"] = FrontID;
            Row["Height"] = Height;
            Row["Width"] = Width;
            Row["Count"] = Count;

            CupboardsDataTable.Rows.Add(Row);

            IsNewCupboardsAdded = true;
        }

        public void AddCupboard(String CupboardName)
        {
            DataRow Row = CupboardsDataTable.NewRow();
            Row["CupboardName"] = CupboardName;
            CupboardsDataTable.Rows.Add(Row);
            CupboardsBindingSource.MoveLast();
            IsNewCupboardsAdded = true;
        }

        public void RemoveCupboard()
        {
            if (CupboardsDataTable.Rows.Count > 0)
                CupboardsBindingSource.RemoveCurrent();

            IsNewCupboardsAdded = true;
        }

        public void SetGrid(ref PercentageDataGrid Grid)
        {
            CupboardsDataGrid = Grid;
            CupboardsDataGrid.DataSource = CupboardsBindingSource;

            //CupboardsDataGrid.Columns["Linked"].Visible = false;
            CupboardsDataGrid.Columns["InsetTypeID"].Visible = false;
            CupboardsDataGrid.Columns.Add(CupboardsInsetTypesColumn);
            CupboardsDataGrid.Columns["CupboardsID"].HeaderText = "№ п\\п";
            CupboardsDataGrid.Columns["Height"].HeaderText = "Высота";
            CupboardsDataGrid.Columns["Width"].HeaderText = "Ширина";
            CupboardsDataGrid.Columns["Count"].HeaderText = "Кол-во";
            CupboardsDataGrid.Columns["CupboardName"].HeaderText = "Шкаф";

            CupboardsDataGrid.Columns["CupboardsID"].FillWeight = 20;
            CupboardsDataGrid.Columns["InsetTypesColumn"].FillWeight = 30;
            CupboardsDataGrid.Columns["Height"].FillWeight = 20;
            CupboardsDataGrid.Columns["Width"].FillWeight = 20;
            CupboardsDataGrid.Columns["Count"].FillWeight = 20;
            CupboardsDataGrid.Columns["CupboardsID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            CupboardsDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            CupboardsDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            CupboardsDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            CupboardsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        public void SaveCupboards()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Cupboards",
                   ConnectionStrings.ZOVReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(CupboardsDataTable);
                    CupboardsDataTable.Clear();
                    DA.Fill(CupboardsDataTable);
                }
            }

            IsNewCupboardsAdded = false;
        }

        public void AddSize()
        {
            if (FrontsOrdersBindingSource.Count == 0 || FrontsOrdersBindingSource.Count == 1)
                return;

            string MainOrderID = ((DataRowView)FrontsOrdersBindingSource.Current).Row["MainOrderID"].ToString();
            string Notes = ((DataRowView)FrontsOrdersBindingSource.Current).Row["Notes"].ToString();
            int FrontID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["FrontID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["PatinaID"]);
            int TechnoProfileID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoProfileID"]);
            int TechnoColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoColorID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["InsetTypeID"]);
            int ColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["ColorID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["InsetColorID"]);
            int TechnoInsetTypeID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoInsetTypeID"]);
            int TechnoInsetColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoInsetColorID"]);
            bool IsSample = Convert.ToBoolean(((DataRowView)FrontsOrdersBindingSource.Current).Row["IsSample"]);

            DataTable DT = new DataTable();
            DT = FrontsOrdersDataTable.Clone();

            for (int i = FrontsOrdersBindingSource.Position + 1; i < FrontsOrdersBindingSource.Count; i++)
            {
                DT.ImportRow(FrontsOrdersDataTable.DefaultView[i].Row);
            }

            int count = FrontsOrdersBindingSource.Count;
            int pos = FrontsOrdersBindingSource.Position + 1;

            //remove rows
            {
                for (int i = pos; i < count; i++)
                {
                    FrontsOrdersBindingSource.RemoveAt(pos);
                }
            }

            DateTime CreateDateTime = Security.GetCurrentDate();
            //create new blank row
            {
                DataRow NewRow = FrontsOrdersDataTable.NewRow();

                NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["CreateUserTypeID"] = 0;
                NewRow["CreateUserID"] = Security.CurrentUserID;
                NewRow["MainOrderID"] = MainOrderID;
                NewRow["FrontID"] = FrontID;
                NewRow["PatinaID"] = PatinaID;
                NewRow["TechnoProfileID"] = TechnoProfileID;
                NewRow["InsetTypeID"] = InsetTypeID;
                NewRow["TechnoColorID"] = TechnoColorID;
                NewRow["ColorID"] = ColorID;
                NewRow["InsetColorID"] = InsetColorID;
                NewRow["TechnoInsetTypeID"] = TechnoInsetTypeID;
                NewRow["TechnoInsetColorID"] = TechnoInsetColorID;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["Count"] = 1;

                NewRow["IsSample"] = IsSample;
                NewRow["Notes"] = Notes;
                NewRow["CupboardString"] = "-";

                FrontsOrdersDataTable.Rows.Add(NewRow);
            }

            //import other rows back to the FrontsOrders table
            {
                for (int i = 0; i < DT.Rows.Count; i++)
                    FrontsOrdersDataTable.ImportRow(DT.Rows[i]);
            }

            DT.Dispose();
        }

        public void ExportCupboardsFromExcelToList(ref ComponentFactory.Krypton.Toolkit.KryptonListBox CupboardsExportListBox)
        {
            CupboardsExportListBox.Items.Clear();

            using (RichTextBox RT = new RichTextBox())
            {
                RT.Paste();
                for (int i = 0; i < RT.Lines.Count(); i++)
                {
                    if (RT.Lines[i].Length > 0 && RT.Lines[i].ToString()[0] != ' ')
                    {
                        CupboardsExportListBox.Items.Add(RT.Lines[i].Trim());
                    }
                }
            }
        }

        public bool SetConfigID(DataRow Row, ref int FactoryID)
        {
            int FrontID = Convert.ToInt32(Row["FrontID"]);
            int PatinaID = Convert.ToInt32(Row["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(Row["InsetTypeID"]);
            int ColorID = Convert.ToInt32(Row["ColorID"]);
            int InsetColorID = Convert.ToInt32(Row["InsetColorID"]);
            int TechnoProfileID = Convert.ToInt32(Row["TechnoProfileID"]);
            int TechnoColorID = Convert.ToInt32(Row["TechnoColorID"]);
            int TechnoInsetTypeID = Convert.ToInt32(Row["TechnoInsetTypeID"]);
            int TechnoInsetColorID = Convert.ToInt32(Row["TechnoInsetColorID"]);
            int Height = Convert.ToInt32(Row["Height"]);
            int Width = Convert.ToInt32(Row["Width"]);

            int F = -1;
            int AreaID = 0;
            Row["FrontConfigID"] = FrontsCatalogOrder.GetFrontConfigID(FrontID, ColorID, PatinaID, InsetTypeID,
                InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, ref F, ref AreaID);
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

        private bool HasWrongParameter()
        {
            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                if (FrontsOrdersDataTable.Rows[i].RowState == DataRowState.Deleted)
                    continue;

                if (Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["Height"]) == 0 || Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["Width"]) == 0 ||
                    Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["Count"]) == 0)
                {
                    return true;
                }
            }

            return false;
        }


        public bool HasRows()
        {
            int ItemsCount = 0;

            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
                if (FrontsOrdersDataTable.Rows[i].RowState != DataRowState.Deleted)
                    ItemsCount++;

            return ItemsCount > 0;
        }

        public void ChangeMainOrder(int MainOrderID)
        {
            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                FrontsOrdersDataTable.Rows[i]["MainOrderID"] = MainOrderID;
            }
        }

        public bool SaveFrontsOrder(int MainOrderID, ref int FactoryID)
        {
            int counter = 0;
            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                if (FrontsOrdersDataTable.Rows[i].RowState == DataRowState.Deleted)
                    continue;
                if (FrontsOrdersDataTable.Rows[i]["Height"].ToString() == "0" || FrontsOrdersDataTable.Rows[i]["Width"].ToString() == "0")
                {
                    MessageBox.Show("Неверный размер: 0");
                    return false;
                }

                counter++;
                if (bSaveCupboards)//"если добавлять шкафы в базу" = true, добавляем шкаф
                {
                    AddCupboardToBase(FrontsOrdersDataTable.Rows[i]["CupboardString"].ToString(),
                                  Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["FrontID"]),
                                  Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["InsetTypeID"]),
                                  Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["Height"]),
                                  Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["Width"]),
                                  Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["Count"]));
                }

                if (SetConfigID(FrontsOrdersDataTable.Rows[i], ref FactoryID) == false)
                {
                    MessageBox.Show("Невозможно сохранить заказ, так как одна или несколько позиций фасадов\r\n" +
                                    "отсутствует в каталоге. Проверьте правильность ввода данных в позиции " + (counter).ToString(),
                                    "Ошибка сохранения заказа");
                    if (FrontsOrdersDataTable.Rows[i]["FrontsOrdersID"] != DBNull.Value)
                        FrontsOrdersBindingSource.Position = FrontsOrdersBindingSource.Find("FrontsOrdersID", Convert.ToInt32(FrontsOrdersDataTable.Rows[i]["FrontsOrdersID"]));
                    return false;
                }
            }


            FrontsOrdersDataAdapter.Update(FrontsOrdersDataTable);
            FrontsOrdersDataTable.Clear();
            FrontsOrdersDataAdapter.Dispose();
            FrontsOrdersCommandBuider.Dispose();
            FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString);
            FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
            FrontsOrdersDataAdapter.Fill(FrontsOrdersDataTable);
            SaveCupboards();
            return true;
        }

        public void AddOrder()
        {
            if (FrontsOrdersBindingSource.Count == 0)
                return;

            string MainOrderID = ((DataRowView)FrontsOrdersBindingSource.Current).Row["MainOrderID"].ToString();
            int FrontID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["FrontID"]);
            int TechnoProfileID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoProfileID"]);
            int TechnoColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoColorID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["PatinaID"]);
            int InsetTypeID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["InsetTypeID"]);
            int ColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["ColorID"]);
            int InsetColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["InsetColorID"]);
            int TechnoInsetTypeID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoInsetTypeID"]);
            int TechnoInsetColorID = Convert.ToInt32(((DataRowView)FrontsOrdersBindingSource.Current).Row["TechnoInsetColorID"]);

            int pos = FrontsOrdersBindingSource.Position + 1;

            DateTime CreateDateTime = Security.GetCurrentDate();
            //create new blank row
            {
                DataRow NewRow = FrontsOrdersDataTable.NewRow();

                NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["CreateUserTypeID"] = 0;
                NewRow["CreateUserID"] = Security.CurrentUserID;
                NewRow["MainOrderID"] = MainOrderID;
                NewRow["FrontID"] = FrontID;
                NewRow["TechnoProfileID"] = TechnoProfileID;
                NewRow["TechnoColorID"] = TechnoColorID;
                NewRow["PatinaID"] = PatinaID;
                NewRow["InsetTypeID"] = InsetTypeID;
                NewRow["ColorID"] = ColorID;
                NewRow["InsetColorID"] = InsetColorID;
                NewRow["TechnoInsetTypeID"] = TechnoInsetTypeID;
                NewRow["TechnoInsetColorID"] = TechnoInsetColorID;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["Count"] = 1;

                FrontsOrdersDataTable.Rows.InsertAt(NewRow, pos);
            }
        }

        public void RemoveOrder()
        {
            if (FrontsOrdersBindingSource.Current != null)
            {
                FrontsOrdersBindingSource.RemoveCurrent();
            }
        }

        public void ClearCurrentOrder()
        {
            FrontsOrdersDataTable.Clear();
        }

        public void NewOrder()
        {
            bSaveCupboards = false;
            FrontsOrdersDataTable.Clear();
            FrontsOrdersDataAdapter.Dispose();
            FrontsOrdersCommandBuider.Dispose();
            FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.ZOVOrdersConnectionString);
            FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
            FrontsOrdersDataAdapter.Fill(FrontsOrdersDataTable);
        }

        public void ImportFromSizeTable(ref PercentageDataGrid DataGrid)
        {
            DataGrid.AllowUserToAddRows = false;

            using (RichTextBox RT = new RichTextBox())
            {
                RT.Paste();


                DataGrid.Rows.Insert(0, RT.Lines.Count() - 1);

                for (int i = 0; i < RT.Lines.Count() - 1; i++)
                {
                    int column = 0;

                    for (int j = 0; j < RT.Lines[i].Length; j++)
                    {
                        if (RT.Lines[i][j] != '\t')
                        {
                            DataGrid.Rows[i].Cells[column].Value += RT.Lines[i][j].ToString();
                        }
                        else
                        {
                            column++;
                        }
                    }

                }
            }

            DataGrid.AllowUserToAddRows = true;
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

            FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString);
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

        public void ExitEdit()
        {
            FrontsOrdersDataTable.Clear();
            FrontsOrdersDataAdapter.Dispose();
            FrontsOrdersCommandBuider.Dispose();
            FrontsOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.ZOVOrdersConnectionString);
            FrontsOrdersCommandBuider = new SqlCommandBuilder(FrontsOrdersDataAdapter);
            FrontsOrdersDataAdapter.Fill(FrontsOrdersDataTable);
        }

        public decimal GetSquare()
        {
            if (FrontsOrdersDataGrid.Rows.Count == 0)
                return 0;

            decimal S = 0;

            foreach (DataGridViewRow Row in FrontsOrdersDataGrid.Rows)
            {
                try
                {
                    if (Convert.ToInt32(Row.Cells["Width"].Value) > 0)
                        S += Convert.ToDecimal(Row.Cells["Height"].Value) * Convert.ToDecimal(Row.Cells["Width"].Value) * Convert.ToDecimal(Row.Cells["Count"].Value);
                }
                catch
                {
                    return 0;
                }
            }

            return Decimal.Round(S / 1000000, 3, MidpointRounding.AwayFromZero);
        }

        //проверяет, внесены ли изменения (редактировались позиции, удалялись, добавлялись)
        public ArrayList AreFrontsEdit(int MainOrderID)
        {
            ArrayList ModifOrders = new ArrayList();

            //если позиция была отредактирована, нужно найти номер упаковки, в которой она лежала, и удалить соответствующие строки из Packages и PackageDetails
            //позиция изменена, известен FrontsOrdersID
            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                if (Row.RowState == DataRowState.Modified)
                {
                    ModifOrders.Add(Convert.ToInt32(Row["FrontsOrdersID"]));
                }
            }

            return ModifOrders;
        }

        //проверяет, внесены ли изменения (редактировались позиции, удалялись, добавлялись)
        public ArrayList AreFrontsDelete(int MainOrderID)
        {
            ArrayList DeleteOrders = new ArrayList();

            //если позиция была удалена, нужно найти номер упаковки, в которой она лежала, и удалить соответствующие строки из Packages и PackageDetails
            //позиция удалена, не известен FrontsOrdersID
            foreach (DataRow Row in TempFrontsOrdersDataTable.Rows)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontsOrdersID = " + Convert.ToInt32(Row["FrontsOrdersID"]));
                if (Rows.Count() < 1)
                {
                    DeleteOrders.Add(Convert.ToInt32(Row["FrontsOrdersID"]));
                }
            }

            return DeleteOrders;
        }

        public void GetTempFronts()
        {
            TempFrontsOrdersDataTable.Clear();
            TempFrontsOrdersDataTable = FrontsOrdersDataTable.Copy();
        }

        public void ClearPackages(int MainOrderID, ArrayList FrontsOrders)
        {
            ArrayList Packages = new ArrayList();

            //в таблице PackageDetails находим позиции, в которых лежат измененные фасады
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE ProductType = 0 AND MainOrderID = " + MainOrderID + ")" +
                " AND OrderID IN (" + String.Join(",", FrontsOrders.OfType<Int32>().ToArray()) + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            //нужны айдишники упаковок, которые будут удалены
                            Packages.Add(Convert.ToInt32(Row["PackageID"]));
                        }
                    }
                }
            }

            if (Packages.Count < 1)
                return;
            //удалить нужно все позиции, которые лежат в одной упаковке вместе с измененной (даже если они не были затронуты)
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" + String.Join(",", Packages.OfType<Int32>().ToArray()) + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }

            if (Packages.Count < 1)
                return;
            //находим упаковки, в которых лежат фасады
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages" +
                " WHERE PackageID IN (" + String.Join(",", Packages.OfType<Int32>().ToArray()) + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void GetOldFronts(int MainOrderID)
        {
            string SelectCommand = @"SELECT FrontID, PatinaID, InsetTypeID, ColorID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, SUM(Count) AS Count
                FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + " GROUP BY FrontID, PatinaID, InsetTypeID, ColorID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                OldFrontsTable.Clear();
                DA.Fill(OldFrontsTable);
            }
        }

        public void GetNewFronts()
        {
            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                NewFrontsTable.Clear();
                //NewFrontsTable = DV.ToTable(false, "FrontID", "PatinaID", "InsetTypeID", "ColorID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID",
                //    "Height", "Width", "Count");
                var countriesTable = FrontsOrdersDataTable.AsEnumerable().GroupBy(row => new
                {
                    FrontID = row.Field<Int64>("FrontID"),
                    PatinaID = row.Field<Int64>("PatinaID"),
                    InsetTypeID = row.Field<Int64>("InsetTypeID"),
                    ColorID = row.Field<Int64>("ColorID"),
                    InsetColorID = row.Field<Int64>("InsetColorID"),
                    TechnoInsetTypeID = row.Field<Int64>("TechnoInsetTypeID"),
                    TechnoInsetColorID = row.Field<Int64>("TechnoInsetColorID"),
                    Height = row.Field<Int32>("Height"),
                    Width = row.Field<Int32>("Width")
                })
                             .Select(grp => new
                             {
                                 FrontID = grp.Key.FrontID,
                                 PatinaID = grp.Key.PatinaID,
                                 InsetTypeID = grp.Key.InsetTypeID,
                                 ColorID = grp.Key.ColorID,
                                 InsetColorID = grp.Key.InsetColorID,
                                 TechnoInsetTypeID = grp.Key.TechnoInsetTypeID,
                                 TechnoInsetColorID = grp.Key.TechnoInsetColorID,
                                 Height = grp.Key.Height,
                                 Width = grp.Key.Width,
                                 Count = grp.Sum(r => r.Field<Int32>("Count"))
                             });
                foreach (var r in countriesTable)
                {
                    DataRow NewRow = NewFrontsTable.NewRow();
                    NewRow["FrontID"] = r.FrontID;
                    NewRow["PatinaID"] = r.PatinaID;
                    NewRow["InsetTypeID"] = r.InsetTypeID;
                    NewRow["ColorID"] = r.ColorID;
                    NewRow["InsetColorID"] = r.InsetColorID;
                    NewRow["TechnoInsetTypeID"] = r.TechnoInsetTypeID;
                    NewRow["TechnoInsetColorID"] = r.TechnoInsetColorID;
                    NewRow["Height"] = r.Height;
                    NewRow["Width"] = r.Width;
                    NewRow["Count"] = r.Count;
                    NewFrontsTable.Rows.Add(NewRow);
                }
                //countriesTable.cop
                //IEnumerable<DataRow> newSort = from row in NewFrontsTable.AsEnumerable()
                //               group row by new
                //               {
                //                   FrontID = row.Field<Int64>("FrontID"),
                //                   PatinaID = row.Field<Int64>("PatinaID"),
                //                   InsetTypeID = row.Field<Int64>("InsetTypeID"),
                //                   ColorID = row.Field<Int64>("ColorID"),
                //                   InsetColorID = row.Field<Int64>("InsetColorID"),
                //                   TechnoInsetTypeID = row.Field<Int64>("TechnoInsetTypeID"),
                //                   TechnoInsetColorID = row.Field<Int64>("TechnoInsetColorID"),
                //                   Height = row.Field<Int32>("Height"),
                //                   Width = row.Field<Int32>("Width")
                //               } into grp
                //               select new
                //               {
                //                   FrontID1 = grp.Key.FrontID,
                //                   PatinaID1 = grp.Key.PatinaID,
                //                   InsetTypeID1 = grp.Key.InsetTypeID,
                //                   ColorID1 = grp.Key.ColorID,
                //                   InsetColorID1 = grp.Key.InsetColorID,
                //                   TechnoInsetTypeID1 = grp.Key.TechnoInsetTypeID,
                //                   TechnoInsetColorID1 = grp.Key.TechnoInsetColorID,
                //                   Height1 = grp.Key.Height,
                //                   Width1 = grp.Key.Width,
                //                   Count1 = grp.Sum(r => r.Field<Int32>("Count"))
                //               };
                //foreach (var r in newSort)
                //{
                //    Console.WriteLine("Group {0}, Type {1}, Sum {2}", r.ColorID1, r.Count1, r.FrontID1);
                //}
                //if (newSort != null)
                //{
                //    DataTable boundTable = newSort;
                //}
            }
        }

        public bool FindDifferenceBetweenFronts(int MainOrderID)
        {
            GetOldFronts(MainOrderID);
            GetNewFronts();

            return AreTablesEquals(OldFrontsTable, NewFrontsTable);
        }

        public bool AreTablesEquals(DataTable First, DataTable Second)
        {
            //Create Empty Table
            DataTable table = new DataTable("Difference");

            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });
                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }
                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r);

                //Create columns for return table
                for (int i = 0; i < First.Columns.Count; i++)
                {
                    table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);
                }

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();
                foreach (DataRow parentrow in ds.Tables[0].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }

                //If Second Row not in First, Add to return table.
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetParentRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }
                table.EndLoadData();
            }

            return table.Rows.Count == 0;
        }

        public bool ColorRow(int FrontID, int ColorID, int PatinaID, int InsetTypeID,
            int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID, int Height, int Width, int Count)
        {
            bool b = true;

            DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + FrontID +
                " AND PatinaID=" + PatinaID +
                " AND InsetTypeID=" + InsetTypeID +
                " AND ColorID=" + ColorID + " AND InsetColorID=" + InsetColorID +
                " AND TechnoInsetTypeID=" + TechnoInsetTypeID + " AND TechnoInsetColorID=" + TechnoInsetColorID +
                " AND Height=" + Height + " AND Width=" + Width + " AND Count=" + Count);
            if (Rows.Count() > 0)
                b = false;

            return b;
        }

        public void ErrosInOldOrder(ref int ErrorsCount)
        {
            //Create Empty Table
            DataTable table = new DataTable("Difference");
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                DT = DV.ToTable(false, "FrontID", "PatinaID", "InsetTypeID", "ColorID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID",
                    "Height", "Width", "Count");
            }
            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { DT.Copy(), OldFrontsTable.Copy() });
                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }
                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r);

                //Create columns for return table
                for (int i = 0; i < OldFrontsTable.Columns.Count; i++)
                {
                    table.Columns.Add(OldFrontsTable.Columns[i].ColumnName, OldFrontsTable.Columns[i].DataType);
                }

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();
                //foreach (DataRow parentrow in ds.Tables[0].Rows)
                //{
                //    DataRow[] childrows = parentrow.GetChildRows(r);
                //    if (childrows == null || childrows.Length == 0)
                //    {
                //        table.LoadDataRow(parentrow.ItemArray, true);
                //    }
                //}
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetParentRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }

                table.EndLoadData();
            }

            ErrorsCount += table.Rows.Count;
        }

        public void ErrosInNewOrder(ref int ErrorsCount)
        {
            //Create Empty Table
            DataTable table = new DataTable("Difference");
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                DT = DV.ToTable(false, "FrontID", "PatinaID", "InsetTypeID", "ColorID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID",
                    "Height", "Width", "Count");
            }
            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { DT.Copy(), NewFrontsTable.Copy() });
                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }
                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r);

                //Create columns for return table
                for (int i = 0; i < OldFrontsTable.Columns.Count; i++)
                {
                    table.Columns.Add(OldFrontsTable.Columns[i].ColumnName, OldFrontsTable.Columns[i].DataType);
                }

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();

                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetParentRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }

                table.EndLoadData();
            }

            ErrorsCount += table.Rows.Count;
        }
    }







    public class DecorOrders
    {
        public bool HasExcluzive = false;
        int SubClientID = 0;
        private DevExpress.XtraTab.XtraTabControl DecorTabControl = null;

        public DecorCatalogOrder DecorCatalogOrder = null;

        PercentageDataGrid MainOrdersFrontsOrdersDataGrid;

        DataTable OldDecorTable;
        DataTable NewDecorTable;

        public DataTable ExcluziveDataTable = null;
        public DataTable TempDecorOrdersDataTable = null;
        public DataTable DecorOrdersDataTable = null;
        public BindingSource DecorOrdersBindingSource = null;
        public DataTable[] DecorItemOrdersDataTables = null;
        public BindingSource[] DecorItemOrdersBindingSources = null;
        public SqlDataAdapter DecorOrdersDataAdapter = null;
        public SqlCommandBuilder DecorOrdersCommandBuilder = null;
        public PercentageDataGrid[] DecorItemOrdersDataGrids = null;

        //конструктор
        public DecorOrders(ref DevExpress.XtraTab.XtraTabControl tDecorTabControl,
            ref DecorCatalogOrder tDecorCatalogOrder, ref
        PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            DecorTabControl = tDecorTabControl;
            DecorCatalogOrder = tDecorCatalogOrder;
            MainOrdersFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }

        public bool HasDecor => DecorOrdersDataTable.Rows.Count > 0;

        public class ExcluziveCatalog
        {
            public int ClientID { get; set; }
            public int SubClientID { get; set; }
            public int DecorConfigID { get; set; }
            public int ProductID { get; set; }
            public int DecorID { get; set; }
        }

        List<ExcluziveCatalog> excluziveCatalogList;

        public void ExcluziveCatalogList()
        {
            excluziveCatalogList = new List<ExcluziveCatalog>();
            for (int i = 0; i < ExcluziveDataTable.Rows.Count; i++)
            {
                ExcluziveCatalog excluziveCatalog = new ExcluziveCatalog
                {
                    ClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["ClientID"]),
                    SubClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["SubClientID"]),
                    DecorConfigID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["DecorConfigID"]),
                    ProductID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["ProductID"]),
                    DecorID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["DecorID"])
                };
                excluziveCatalogList.Add(excluziveCatalog);
            }
        }

        public void HasClientExcluzive(int iSubClientID)
        {
            string SelectCommand = string.Empty;
            SubClientID = iSubClientID;
            ExcluziveDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorConfig.*, infiniu2_marketingreference.dbo.ExcluziveCatalog.ClientID,infiniu2_marketingreference.dbo.ExcluziveCatalog.SubClientID FROM DecorConfig 
                INNER JOIN infiniu2_marketingreference.dbo.ExcluziveCatalog ON DecorConfig.DecorConfigID=infiniu2_marketingreference.dbo.ExcluziveCatalog.ConfigID AND ProductType=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ExcluziveDataTable);
            }
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            ExcluziveCatalogList();

            List<ExcluziveCatalog> catalogs = excluziveCatalogList.FindAll(item => (item.SubClientID == 0 || item.SubClientID == SubClientID));
            HasExcluzive = catalogs.Count > 0;

            for (int i = 0; i < excluziveCatalogList.Count; i++)
            {
                ExcluziveCatalog catalog = excluziveCatalogList[i];
                int ExcluziveClientID = catalog.ClientID;
                int ExcluziveSubClientID = catalog.SubClientID;
                int DecorConfigID = catalog.DecorConfigID;

                int Excluzive1 = 0;
                if (ExcluziveSubClientID == -1)
                    Excluzive1 = 0;
                if (ExcluziveSubClientID == 0)
                    Excluzive1 = 1;
                if (ExcluziveSubClientID == SubClientID)
                {
                    Excluzive1 = 1;
                }
                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.DecorConfigID == DecorConfigID && (item.SubClientID == 0 || item.SubClientID == ExcluziveSubClientID)).Count > 1)
                    Excluzive1 = 1;
                //DataRow[] rows = DecorCatalogOrder.DecorConfigDataTable.Select("DecorConfigID=" + DecorConfigID);
                //if (rows.Count() > 0)
                //{
                //    if (rows[0]["Excluzive"] == DBNull.Value)
                //        rows[0]["Excluzive"] = Excluzive1;
                //    else
                //    {
                //        if (Excluzive1 == 1)
                //            rows[0]["Excluzive"] = Excluzive1;
                //    }
                //}

                foreach (DataRow row in DecorCatalogOrder.DecorConfigDataTable.Rows)
                {
                    if (Convert.ToInt32(row["DecorConfigID"]) == DecorConfigID)
                    {
                        if (row["Excluzive"] == DBNull.Value)
                            row["Excluzive"] = Excluzive1;
                        else
                        {
                            if (Excluzive1 == 1)
                                row["Excluzive"] = Excluzive1;
                        }
                        break;
                    }
                }
            }
            for (int i = 0; i < DecorCatalogOrder.DecorProductsDataTable.Rows.Count; i++)
            {
                int ProductID = Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]);

                if (excluziveCatalogList.FindAll(item => item.ProductID == ProductID).Count == 0)
                    continue;

                DataRow[] rows = DecorCatalogOrder.DecorConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND ProductID=" + ProductID);
                int NotExcluziveConfiguration = rows.Count();

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.ProductID == ProductID && (item.SubClientID == 0 || item.SubClientID == SubClientID)).Count > 0)
                    DecorCatalogOrder.DecorProductsDataTable.Rows[i]["Excluzive"] = 1;
                else
                {
                    if (NotExcluziveConfiguration == 0)
                        DecorCatalogOrder.DecorProductsDataTable.Rows[i]["Excluzive"] = 0;
                }
            }
            for (int i = 0; i < DecorCatalogOrder.DecorDataTable.Rows.Count; i++)
            {
                int DecorID = Convert.ToInt32(DecorCatalogOrder.DecorDataTable.Rows[i]["DecorID"]);

                if (excluziveCatalogList.FindAll(item => item.DecorID == DecorID).Count == 0)
                    continue;

                DataRow[] rows = DecorCatalogOrder.DecorConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND DecorID=" + DecorID);
                int NotExcluziveConfiguration = rows.Count();

                if (excluziveCatalogList.FindAll(item => item.SubClientID != -1 && item.DecorID == DecorID && (item.SubClientID == 0 || item.SubClientID == SubClientID)).Count > 0)
                    DecorCatalogOrder.DecorDataTable.Rows[i]["Excluzive"] = 1;
                else
                {
                    if (NotExcluziveConfiguration == 0)
                        DecorCatalogOrder.DecorDataTable.Rows[i]["Excluzive"] = 0;
                }
            }

            sw.Stop();
            double G = sw.Elapsed.TotalSeconds;
        }

        //public void HasClientExcluzive(int iSubClientID)
        //{
        //    string SelectCommand = string.Empty;
        //    SubClientID = iSubClientID;
        //    ExcluziveDataTable = new DataTable();
        //    using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorConfig.*, infiniu2_marketingreference.dbo.ExcluziveCatalog.ClientID,infiniu2_marketingreference.dbo.ExcluziveCatalog.SubClientID FROM DecorConfig 
        //        INNER JOIN infiniu2_marketingreference.dbo.ExcluziveCatalog ON DecorConfig.DecorConfigID=infiniu2_marketingreference.dbo.ExcluziveCatalog.ConfigID AND ProductType=1",
        //        ConnectionStrings.CatalogConnectionString))
        //    {
        //        DA.Fill(ExcluziveDataTable);
        //    }

        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //    DataRow[] rows = ExcluziveDataTable.Select("SubClientID=" + SubClientID);
        //    HasExcluzive = rows.Count() > 0;
        //    for (int i = 0; i < ExcluziveDataTable.Rows.Count; i++)
        //    {
        //        int ExcluziveClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["ClientID"]);
        //        int ExcluziveSubClientID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["SubClientID"]);
        //        int DecorID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["DecorID"]);
        //        int ColorID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["ColorID"]);
        //        int PatinaID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["PatinaID"]);
        //        int DecorConfigID = Convert.ToInt32(ExcluziveDataTable.Rows[i]["DecorConfigID"]);
        //        int Excluzive1 = 0;
        //        if (ExcluziveSubClientID == -1)
        //            Excluzive1 = 0;
        //        if (ExcluziveSubClientID == 0)
        //            Excluzive1 = 1;
        //        if (ExcluziveSubClientID == SubClientID)
        //        {
        //            Excluzive1 = 1;
        //        }
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + ExcluziveSubClientID + ") AND DecorConfigID=" + DecorConfigID);
        //        if (rows.Count() > 1)
        //            Excluzive1 = 1;
        //        rows = DecorCatalogOrder.DecorConfigDataTable.Select("DecorConfigID=" + DecorConfigID);
        //        if (rows.Count() > 0)
        //        {
        //            if (rows[0]["Excluzive"] == DBNull.Value)
        //                rows[0]["Excluzive"] = Excluzive1;
        //            else
        //            {
        //                if (Excluzive1 == 1)
        //                    rows[0]["Excluzive"] = Excluzive1;
        //            }
        //        }
        //    }
        //    for (int i = 0; i < DecorCatalogOrder.DecorProductsDataTable.Rows.Count; i++)
        //    {
        //        int ProductID = Convert.ToInt32(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"]);
        //        rows = ExcluziveDataTable.Select("ProductID=" + ProductID);
        //        if (rows.Count() == 0)
        //            continue;
        //        rows = DecorCatalogOrder.DecorConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND ProductID=" + ProductID);
        //        int NotExcluziveConfiguration = rows.Count();
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + SubClientID + ") AND ProductID=" + ProductID);
        //        int ClientExcluziveConfiguration = rows.Count();
        //        if (ClientExcluziveConfiguration > 0)
        //            DecorCatalogOrder.DecorProductsDataTable.Rows[i]["Excluzive"] = 1;
        //        else
        //        {
        //            if (NotExcluziveConfiguration == 0)
        //                DecorCatalogOrder.DecorProductsDataTable.Rows[i]["Excluzive"] = 0;
        //        }
        //    }
        //    for (int i = 0; i < DecorCatalogOrder.DecorDataTable.Rows.Count; i++)
        //    {
        //        int DecorID = Convert.ToInt32(DecorCatalogOrder.DecorDataTable.Rows[i]["DecorID"]);
        //        rows = ExcluziveDataTable.Select("DecorID=" + DecorID);
        //        if (rows.Count() == 0)
        //            continue;
        //        rows = DecorCatalogOrder.DecorConfigDataTable.Select("(Excluzive<>0 OR Excluzive IS NULL) AND DecorID=" + DecorID);
        //        int NotExcluziveConfiguration = rows.Count();
        //        rows = ExcluziveDataTable.Select("SubClientID<>-1 AND (SubClientID=0 OR SubClientID=" + SubClientID + ") AND DecorID=" + DecorID);
        //        int ClientExcluziveConfiguration = rows.Count();
        //        if (ClientExcluziveConfiguration > 0)
        //            DecorCatalogOrder.DecorDataTable.Rows[i]["Excluzive"] = 1;
        //        else
        //        {
        //            if (NotExcluziveConfiguration == 0)
        //                DecorCatalogOrder.DecorDataTable.Rows[i]["Excluzive"] = 0;
        //        }
        //    }
        //    sw.Stop();
        //double G = sw.Elapsed.TotalSeconds;
        //}

        public DataTable CurrentDecorOrdersDT
        {
            get
            {
                DataTable DT = new DataTable();
                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DT = DV.ToTable(false, "ProductID", "DecorID", "ColorID",
                    "Length", "Height", "Width", "Count");
                }
                return DT;
            }
        }

        public void GetDecorOrdersDT()
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                foreach (DataRow Row in DecorItemOrdersDataTables[i].Rows)
                {
                    DecorOrdersDataTable.ImportRow(Row);
                }
            }

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                //if (Convert.ToInt32(Row["ColorID"]) == -1)
                //    Row["ColorID"] = 0;
            }
        }

        private void Create()
        {
            if (OldDecorTable == null)
                OldDecorTable = new DataTable();
            if (NewDecorTable == null)
                NewDecorTable = new DataTable();

            DecorOrdersDataTable = new DataTable();
            TempDecorOrdersDataTable = new DataTable();
            DecorOrdersBindingSource = new BindingSource();
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
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
        }

        private void Binding()
        {
            DecorOrdersBindingSource.DataSource = DecorOrdersDataTable;
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

                DecorItemOrdersDataGrids[i] = new PercentageDataGrid();
                DecorItemOrdersDataGrids[i].EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(DecorOrders_EditingControlShowing);
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
                DecorItemOrdersDataGrids[i].VirtualMode = MainOrdersFrontsOrdersDataGrid.VirtualMode;
                DecorItemOrdersDataGrids[i].SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DecorItemOrdersDataGrids[i].SelectedColorStyle = PercentageDataGrid.ColorStyle.Green;
                //DecorItemOrdersDataGrids[i].ReadOnly = true;
                DecorItemOrdersDataGrids[i].UseCustomBackColor = true;

                DataGridViewTextBoxColumn IndexNumber = new DataGridViewTextBoxColumn()
                {
                    HeaderText = "",
                    Name = "IndexNumber",
                    ReadOnly = true
                };
                DecorItemOrdersDataGrids[i].Columns.Add(IndexNumber);

                DecorItemOrdersDataGrids[i].Columns["IndexNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                DecorItemOrdersDataGrids[i].Columns["IndexNumber"].Width = 40;
                DecorItemOrdersDataGrids[i].Columns["IndexNumber"].Visible = true;
                DecorItemOrdersDataGrids[i].Columns["IndexNumber"].DisplayIndex = 0;

                DecorItemOrdersDataGrids[i].CellValueNeeded += DecorOrders_CellValueNeeded;

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

                DecorItemOrdersDataGrids[i].Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                DecorItemOrdersDataGrids[i].Columns["IsSample"].Width = 80;

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
                DecorItemOrdersDataGrids[i].Columns["IsSample"].HeaderText = "Образец";
                DecorItemOrdersDataGrids[i].Columns["Debt"].Visible = false;
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
                DecorItemOrdersDataGrids[i].AutoGenerateColumns = false;
                int DisplayIndex = 0;
                DecorItemOrdersDataGrids[i].Columns["IndexNumber"].DisplayIndex = DisplayIndex++;
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

        void DecorOrders_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        void DecorOrders_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox)
            {
                ((ComboBox)e.Control).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)e.Control).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
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

        public bool SetDecorConfigID(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID,
            int Length, int Height, int Width, DataRow Row, ref int FactoryID)
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

        public void AddOrder()
        {
            if (DecorOrdersBindingSource.Count == 0)
                return;

            string MainOrderID = ((DataRowView)DecorOrdersBindingSource.Current).Row["MainOrderID"].ToString();
            int ProductID = Convert.ToInt32(((DataRowView)DecorOrdersBindingSource.Current).Row["ProductID"]);
            int DecorID = Convert.ToInt32(((DataRowView)DecorOrdersBindingSource.Current).Row["DecorID"]);
            int ColorID = Convert.ToInt32(((DataRowView)DecorOrdersBindingSource.Current).Row["ColorID"]);
            int PatinaID = Convert.ToInt32(((DataRowView)DecorOrdersBindingSource.Current).Row["PatinaID"]);

            int pos = DecorOrdersBindingSource.Position + 1;

            DateTime CreateDateTime = Security.GetCurrentDate();
            //create new blank row
            {
                DataRow NewRow = DecorOrdersDataTable.NewRow();

                NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["CreateUserTypeID"] = 0;
                NewRow["CreateUserID"] = Security.CurrentUserID;
                NewRow["MainOrderID"] = MainOrderID;
                NewRow["ProductID"] = ProductID;
                NewRow["DecorID"] = DecorID;
                NewRow["ColorID"] = ColorID;
                NewRow["PatinaID"] = PatinaID;
                NewRow["Length"] = -1;
                NewRow["Height"] = -1;
                NewRow["Width"] = -1;
                NewRow["Count"] = 1;

                if (ProductID == 42)
                    NewRow["IsSample"] = true;
                DecorOrdersDataTable.Rows.InsertAt(NewRow, pos);
            }
        }

        public void RemoveOrder()
        {
            if (DecorOrdersBindingSource.Current != null)
            {
                DecorOrdersBindingSource.RemoveCurrent();
            }
        }

        public void AddDecorOrder(int MainOrderID, int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID,
            int Length, int Height, int Width, int Count, string Notes, bool IsSample)
        {

            if ((Height < 50 || Width < 50) && (DecorID == 4012 || DecorID == 4013))
            {
                MessageBox.Show("Высота и ширина вставки не могут быть меньше 50 мм", "Добавление вставки");
                return;
            }

            int index = DecorCatalogOrder.DecorProductsBindingSource.Find("ProductID", ProductID);

            DateTime CreateDateTime = Security.GetCurrentDate();
            DataRow NewRow = DecorItemOrdersDataTables[index].NewRow();
            NewRow["IsSample"] = IsSample;
            NewRow["CreateDateTime"] = CreateDateTime;
            NewRow["CreateUserTypeID"] = 0;
            NewRow["CreateUserID"] = Security.CurrentUserID;

            NewRow["MainOrderID"] = MainOrderID;
            NewRow["ProductID"] = ProductID;
            NewRow["DecorID"] = DecorID;
            NewRow["ColorID"] = ColorID;
            NewRow["PatinaID"] = PatinaID;
            NewRow["InsetTypeID"] = InsetTypeID;
            NewRow["InsetColorID"] = InsetColorID;
            NewRow["Length"] = Length;
            NewRow["Height"] = Height;
            NewRow["Width"] = Width;

            NewRow["Count"] = Count;
            NewRow["Notes"] = Notes;

            if (ProductID == 42)
                NewRow["IsSample"] = true;
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
                                                            ConnectionStrings.ZOVOrdersConnectionString);
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

        public void NewOrder()
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

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
        }

        public void DeleteCurrentDecorItem(int TabIndex)
        {
            if (TabIndex < 0)
                return;

            if (DecorItemOrdersBindingSources[TabIndex].Count > 0)
            {
                {
                    DecorItemOrdersBindingSources[TabIndex].RemoveCurrent();
                }
            }

            if (DecorItemOrdersBindingSources[TabIndex].Count == 0)
                DecorTabControl.TabPages[TabIndex].PageVisible = false;

            int C = 0;

            for (int i = 0; i < DecorTabControl.TabPages.Count; i++)
                C += Convert.ToInt32(DecorTabControl.TabPages[i].PageVisible);

            DecorTabControl.Visible = (C > 0);
        }

        public void ChangeMainOrder(int MainOrderID)
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                foreach (DataRow Row in DecorItemOrdersDataTables[i].Rows)
                {
                    Row["MainOrderID"] = MainOrderID;
                }
            }
            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                Row["MainOrderID"] = MainOrderID;
            }
        }

        public bool SaveDecorOrder(ref int FactoryID)
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
                            Convert.ToInt32(Row["Length"]), Convert.ToInt32(Row["Height"]), Convert.ToInt32(Row["Width"]), Row, ref FactoryID) == false)
                            return false;

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
                    DecorOrdersDataTable.ImportRow(Row);
                }
            }

            DecorOrdersDataAdapter.Update(DecorOrdersDataTable);

            return true;
        }

        public void ExitEdit()
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

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.ZOVOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
        }

        //проверяет, внесены ли изменения (редактировались позиции, удалялись, добавлялись)
        public ArrayList AreDecorEdit(int MainOrderID)
        {
            ArrayList ModifOrders = new ArrayList();

            //если позиция была отредактирована, нужно найти номер упаковки, в которой она лежала, и удалить соответствующие строки из Packages и PackageDetails
            //позиция изменена, известен DecorOrderID
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                foreach (DataRow Row in DecorItemOrdersDataTables[i].Rows)
                {
                    if (Row.RowState == DataRowState.Modified)
                    {
                        ModifOrders.Add(Convert.ToInt32(Row["DecorOrderID"]));
                    }
                }
            }

            return ModifOrders;
        }

        //проверяет, внесены ли изменения (редактировались позиции, удалялись, добавлялись)
        public ArrayList AreDecorDelete(int MainOrderID)
        {
            ArrayList DeleteOrders = new ArrayList();
            DataTable DT = DecorOrdersDataTable.Clone();
            //если позиция была удалена, нужно найти номер упаковки, в которой она лежала, и удалить соответствующие строки из Packages и PackageDetails
            //позиция удалена, не известен DecorOrderID
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                foreach (DataRow Row in DecorItemOrdersDataTables[i].Rows)
                {
                    DT.ImportRow(Row);
                }
            }

            foreach (DataRow Row in TempDecorOrdersDataTable.Rows)
            {
                DataRow[] Rows = DT.Select("DecorOrderID = " + Convert.ToInt32(Row["DecorOrderID"]));
                if (Rows.Count() < 1)
                {
                    DeleteOrders.Add(Convert.ToInt32(Row["DecorOrderID"]));
                }
            }

            return DeleteOrders;
        }

        public void GetTempDecor()
        {
            TempDecorOrdersDataTable.Clear();
            TempDecorOrdersDataTable = DecorOrdersDataTable.Copy();
        }

        public void ClearPackages(int MainOrderID, ArrayList FrontsOrders)
        {
            ArrayList Packages = new ArrayList();

            //в таблице PackageDetails находим позиции, в которых лежат измененные декоры
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE ProductType = 1 AND MainOrderID = " + MainOrderID + ")" +
                " AND OrderID IN (" + String.Join(",", FrontsOrders.OfType<Int32>().ToArray()) + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            //нужны айдишники упаковок, которые будут удалены
                            Packages.Add(Convert.ToInt32(Row["PackageID"]));
                        }
                    }
                }
            }

            if (Packages.Count < 1)
                return;
            //удалить нужно все позиции, которые лежат в одной упаковке вместе с измененной (даже если они не были затронуты)
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" + String.Join(",", Packages.OfType<Int32>().ToArray()) + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }

            if (Packages.Count < 1)
                return;
            //находим упаковки, в которых лежит декор
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages" +
                " WHERE PackageID IN (" + String.Join(",", Packages.OfType<Int32>().ToArray()) + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public bool FindDifferenceBetweenDecor(int MainOrderID)
        {
            GetOldDecor(MainOrderID);
            GetNewDecor();

            return AreTablesEquals(OldDecorTable, NewDecorTable);
        }

        private void GetOldDecor(int MainOrderID)
        {
            string SelectCommand = @"SELECT ProductID, DecorID, ColorID, Length, Height, Width, Count FROM DecorOrders WHERE MainOrderID = " + MainOrderID;
            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                OldDecorTable.Clear();
                DA.Fill(OldDecorTable);
            }
        }

        private void GetNewDecor()
        {
            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                NewDecorTable = DV.ToTable(false, "ProductID", "DecorID", "ColorID",
                    "Length", "Height", "Width", "Count");
            }
        }

        public bool ColorRow(int ProductID, int DecorID, int ColorID, int PatinaID, int InsetTypeID, int InsetColorID, int Length, int Height, int Width, int Count)
        {
            bool b = true;

            DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + ProductID +
                " AND DecorID=" + DecorID + " AND ColorID=" + ColorID + " AND PatinaID=" + PatinaID +
                " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID +
                " AND Length=" + Length + " AND Height=" + Height +
                " AND Width=" + Width + " AND Count=" + Count);
            if (Rows.Count() > 0)
                b = false;

            return b;
        }

        public bool AreTablesEquals(DataTable First, DataTable Second)
        {
            //Create Empty Table
            DataTable table = new DataTable("Difference");

            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });
                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }
                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r);

                //Create columns for return table
                for (int i = 0; i < First.Columns.Count; i++)
                {
                    table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);
                }

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();
                foreach (DataRow parentrow in ds.Tables[0].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }

                //If Second Row not in First, Add to return table.
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetParentRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }
                table.EndLoadData();
            }

            return table.Rows.Count == 0;
        }

        public void ErrosInOldOrder(ref int ErrorsCount)
        {
            //Create Empty Table
            DataTable table = new DataTable("Difference");
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                DT = DV.ToTable(false, "ProductID", "DecorID", "ColorID",
                    "Length", "Height", "Width", "Count");
            }
            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { DT.Copy(), OldDecorTable.Copy() });
                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }
                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r);

                //Create columns for return table
                for (int i = 0; i < OldDecorTable.Columns.Count; i++)
                {
                    table.Columns.Add(OldDecorTable.Columns[i].ColumnName, OldDecorTable.Columns[i].DataType);
                }

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();
                //foreach (DataRow parentrow in ds.Tables[0].Rows)
                //{
                //    DataRow[] childrows = parentrow.GetChildRows(r);
                //    if (childrows == null || childrows.Length == 0)
                //    {
                //        table.LoadDataRow(parentrow.ItemArray, true);
                //    }
                //}
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetParentRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }

                table.EndLoadData();
            }

            ErrorsCount += table.Rows.Count;
        }

        public void ErrosInNewOrder(ref int ErrorsCount)
        {
            //Create Empty Table
            DataTable table = new DataTable("Difference");
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                DT = DV.ToTable(false, "ProductID", "DecorID", "ColorID",
                    "Length", "Height", "Width", "Count");
            }
            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { DT.Copy(), NewDecorTable.Copy() });
                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }
                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r);

                //Create columns for return table
                for (int i = 0; i < OldDecorTable.Columns.Count; i++)
                {
                    table.Columns.Add(OldDecorTable.Columns[i].ColumnName, OldDecorTable.Columns[i].DataType);
                }

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();
                //foreach (DataRow parentrow in ds.Tables[0].Rows)
                //{
                //    DataRow[] childrows = parentrow.GetChildRows(r);
                //    if (childrows == null || childrows.Length == 0)
                //    {
                //        table.LoadDataRow(parentrow.ItemArray, true);
                //    }
                //}
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetParentRows(r);
                    if (childrows == null || childrows.Length == 0)
                    {
                        table.LoadDataRow(parentrow.ItemArray, true);
                    }
                }

                table.EndLoadData();
            }

            ErrorsCount += table.Rows.Count;
        }
    }







    public class OrdersCalculate
    {
        public FrontsCalculate FrontsCalculate = null;
        public DecorCalculate DecorCalculate = null;


        public OrdersCalculate()
        {
            FrontsCalculate = new FrontsCalculate();
            DecorCalculate = new DecorCalculate();
        }

        public void SetMainOrderResultCost(DataRow Row)
        {
            decimal OrderCost = Convert.ToDecimal(Row["OrderCost"]);

            decimal CalcDebtCost = Convert.ToDecimal(Row["CalcDebtCost"]);
            decimal WriteOffDebtCost = Convert.ToDecimal(Row["WriteOffDebtCost"]);
            decimal WriteOffDefectsCost = Convert.ToDecimal(Row["WriteOffDefectsCost"]);
            decimal WriteOffProductionErrorsCost = Convert.ToDecimal(Row["WriteOffProductionErrorsCost"]);
            decimal WriteOffZOVErrorsCost = Convert.ToDecimal(Row["WriteOffZOVErrorsCost"]);
            decimal SamplesWriteOffCost = Convert.ToDecimal(Row["SamplesWriteOffCost"]);
            decimal TotalWriteOffCost = WriteOffDebtCost + WriteOffDefectsCost + WriteOffProductionErrorsCost + WriteOffZOVErrorsCost + SamplesWriteOffCost;
            decimal ProfitCost = OrderCost - TotalWriteOffCost - CalcDebtCost;

            if (ProfitCost < 0)
                ProfitCost = 0;

            Row["TotalWriteOffCost"] = TotalWriteOffCost;
            Row["ProfitCost"] = ProfitCost;
        }

        public DataTable FFFF()
        {
            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MainOrders WHERE IsSample=1",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(DT);
                }
            }
            return DT;
        }

        public void CalculateOrder(int MainOrderID, int PriceTypeID, bool IsSample, bool TechDrilling)
        {
            decimal FrontsOrderSquare = 0;
            decimal FrontsOrderCost = 0;
            decimal DecorOrderCost = 0;
            decimal FrontsOrderWeight = 0;
            decimal DecorOrderWeight = 0;

            FrontsOrderCost = Decimal.Round(FrontsCalculate.CalculateFronts(MainOrderID, ref FrontsOrderSquare, ref FrontsOrderWeight,
                                            PriceTypeID, IsSample, TechDrilling), 1, MidpointRounding.AwayFromZero);
            DecorOrderCost = Decimal.Round(DecorCalculate.CalculateDecor(MainOrderID, ref DecorOrderWeight, IsSample), 1, MidpointRounding.AwayFromZero);

            for (int i = 0; i < 3; i++)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FrontsSquare, FrontsCost, DecorCost, OrderCost, Weight, DebtTypeID, CalcDebtCost, IncomeCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost, SamplesWriteOffCost, TotalWriteOffCost, ProfitCost, NeedCalculate   FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            DT.Rows[0]["FrontsSquare"] = Decimal.Round(FrontsOrderSquare, 3, MidpointRounding.AwayFromZero);
                            DT.Rows[0]["FrontsCost"] = Decimal.Round(FrontsOrderCost, 3, MidpointRounding.AwayFromZero);
                            DT.Rows[0]["DecorCost"] = Decimal.Round(DecorOrderCost, 3, MidpointRounding.AwayFromZero);
                            DT.Rows[0]["OrderCost"] = Decimal.Round(FrontsOrderCost + DecorOrderCost, 3, MidpointRounding.AwayFromZero);
                            DT.Rows[0]["Weight"] = Decimal.Round(FrontsOrderWeight + DecorOrderWeight, 3, MidpointRounding.AwayFromZero);

                            if (DT.Rows[0]["DebtTypeID"].ToString() != "0" && Convert.ToBoolean(DT.Rows[0]["NeedCalculate"]) == false)
                                DT.Rows[0]["CalcDebtCost"] = DT.Rows[0]["OrderCost"];
                            else
                                DT.Rows[0]["CalcDebtCost"] = 0;

                            DT.Rows[0]["IncomeCost"] = Convert.ToDecimal(DT.Rows[0]["OrderCost"]) - Convert.ToDecimal(DT.Rows[0]["CalcDebtCost"]);

                            SetMainOrderResultCost(DT.Rows[0]);

                            try
                            {
                                DA.Update(DT);
                                break;
                            }
                            catch
                            {

                            }

                        }
                    }
                }
            }
        }
    }







    public class FrontsCalculate
    {
        bool Sale = false;

        DataTable FrontsConfigDataTable = null;
        DataTable DecorConfigDataTable = null;
        DataTable TechStoreDataTable = null;
        DataTable StandardDataTable = null;
        DataTable MeasuresDataTable = null;
        DataTable InsetTypesDataTable = null;
        DataTable InsetPriceDataTable = null;
        DataTable FrontsDataTable = null;
        DataTable AluminiumFrontsDataTable = null;
        DataTable InsetMarginsDataTable = null;

        public FrontsCalculate()
        {
            Initialize();
        }


        private void Create()
        {

        }

        private void Fill()
        {
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
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

            //AluminiumFrontsDataTable = TM.AluminiumFrontsDataTable.Copy();
            AluminiumFrontsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM AluminiumFronts",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(AluminiumFrontsDataTable);
            }

            //StandardDataTable = TM.StandardDataTable.Copy();
            StandardDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Standard",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(StandardDataTable);
            }

            //FrontsConfigDataTable = TM.FrontsConfigDataTable.Copy();
            FrontsConfigDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsConfig",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(FrontsConfigDataTable);
            //}
            FrontsConfigDataTable = TablesManager.FrontsConfigDataTable;

            //DecorConfigDataTable = TM.DecorConfigDataTable.Copy();
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

            //MeasuresDataTable = TM.MeasuresDataTable.Copy();
            MeasuresDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDataTable);
            }

            //InsetMarginsDataTable = TM.InsetMarginsDataTable.Copy();
            InsetMarginsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetMargins",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetMarginsDataTable);
            }

            //InsetPriceDataTable = TM.InsetPriceDataTable.Copy();
            InsetPriceDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetPrice",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(InsetPriceDataTable);
            }
        }

        private void Initialize()
        {
            Create();
            Fill();
        }

        private bool IsStandardSize(DataRow FrontsOrdersRow)
        {
            DataRow[] Rows = StandardDataTable.Select("Height = " + FrontsOrdersRow["Height"].ToString() +
                " AND Width = " + FrontsOrdersRow["Width"].ToString());

            //return (Rows.Count() > 0);
            return true;
        }

        private decimal GetFrontPrice(DataRow FrontsOrdersRow, string PriceType, bool IsSample, bool TechDrilling)
        {
            decimal Price = 0;
            decimal ExtraPrice = 0;

            decimal SampleSaleValue = Convert.ToDecimal(FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString())[0]["SampleSaleValue"]);
            decimal SaleValue = Convert.ToDecimal(FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"])[0]["SaleValue"]);
            decimal p = Convert.ToDecimal(FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString())[0]["ZOV" + PriceType]);
            if (Sale)
            {
                if (IsSample)
                    p = (p - p / 100 * SampleSaleValue);
                else
                    p = (p - p / 100 * SaleValue);
            }
            else
                if (IsSample)
                p = p * 0.5m;
            if (TechDrilling)
                p = p * 1.3m;
            ExtraPrice = GetNonStandardMargin(Convert.ToInt32(FrontsOrdersRow["FrontConfigID"]));

            //если фасад нестандартный и имеет наценку и прямой
            if (ExtraPrice > 0 && IsStandardSize(FrontsOrdersRow) == false && PriceType != "WallPrice" && FrontsOrdersRow["Width"].ToString() != "-1")
            {
                Price = p / 100 * ExtraPrice + p;
                FrontsOrdersRow["IsNonStandard"] = true;
            }
            else
            {
                Price = p;
                FrontsOrdersRow["IsNonStandard"] = false;
            }

            FrontsOrdersRow["FrontPrice"] = Price;

            return Price;
        }

        private decimal GetNonStandardMargin(int FrontConfigID)
        {
            DataRow[] Rows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontConfigID);

            return Convert.ToDecimal(Rows[0]["ZOVNonStandMargin"]);
        }

        private decimal InsetPrice(DataRow FrontsOrdersRow, bool IsSample, bool TechDrilling)
        {
            int FrontID = Convert.ToInt32(FrontsOrdersRow["FrontID"]);
            int InsetTypeID = Convert.ToInt32(FrontsOrdersRow["InsetTypeID"]);
            decimal Price = 0;
            decimal SampleSaleValue = Convert.ToDecimal(FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString())[0]["SampleSaleValue"]);
            decimal SaleValue = Convert.ToDecimal(FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"])[0]["SaleValue"]);
            //Решетки
            if (InsetTypeID == 685 || InsetTypeID == 686 || InsetTypeID == 687 || InsetTypeID == 688 || InsetTypeID == 29470 || InsetTypeID == 29471)
            {
                //DataRow[] Rows = DecorConfigDataTable.Select("DecorID = " + InsetTypeID);
                //if (Rows.Count() > 0)
                //{
                //    if (Rows[0]["ZOVPrice"] != DBNull.Value)
                //    {
                //        Price = Convert.ToDecimal(Rows[0]["ZOVPrice"]);
                //        return Price;
                //    }
                //}
                Price = 75;
                if (Sale)
                {
                    if (IsSample)
                        Price = (Price - Price / 100 * SampleSaleValue);
                    else
                        Price = (Price - Price / 100 * SaleValue);
                }
                else
                    if (IsSample)
                    Price = Price * 0.5m;
                if (TechDrilling)
                    Price = Price * 1.3m;
                return Price;
            }
            //Стекло
            if (IsAluminium(FrontsOrdersRow) > -1 && InsetTypeID == 2)
            {
                int InsetColorID = Convert.ToInt32(FrontsOrdersRow["InsetColorID"]);
                DataRow[] Rows = DecorConfigDataTable.Select("DecorID = " + InsetColorID);
                if (Rows.Count() > 0)
                {
                    if (Rows[0]["ZOVPrice"] != DBNull.Value)
                    {
                        Price = Convert.ToDecimal(Rows[0]["ZOVPrice"]);
                        if (Sale)
                        {
                            if (IsSample)
                                Price = (Price - Price / 100 * SampleSaleValue);
                            else
                                Price = (Price - Price / 100 * SaleValue);
                        }
                        else
                            if (IsSample)
                            Price = Price * 0.5m;

                        if (TechDrilling)
                            Price = Price * 1.3m;
                        return Price;
                    }
                }
            }
            //Аппликации
            if (FrontID == 3728 || FrontID == 3731 ||
                FrontID == 3732 || FrontID == 3739 ||
                FrontID == 3740 || FrontID == 3741 ||
                FrontID == 3744 || FrontID == 3745 ||
                FrontID == 3746 ||
                InsetTypeID == 28961 || InsetTypeID == 3653 || InsetTypeID == 3654 || InsetTypeID == 3655
                )
            {
                Price = 5;
            }
            if (Sale)
            {
                if (IsSample)
                    Price = (Price - Price / 100 * SampleSaleValue);
                else
                    Price = (Price - Price / 100 * SaleValue);
            }
            else
                if (IsSample)
                Price = Price * 0.5m;

            if (TechDrilling)
                Price = Price * 1.3m;
            return Price;
        }

        private bool IsMeasureSquare(DataRow FrontsOrdersRow)
        {
            DataRow[] Rows = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString());

            if (Convert.ToInt32(Rows[0]["MeasureID"]) == 1)
                return true;

            return false;
        }

        private bool IsInsetMeasureSquare(DataRow FrontsOrdersRow)
        {
            DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + FrontsOrdersRow["InsetTypeID"].ToString());

            if (Convert.ToInt32(Rows[0]["MeasureID"]) == 1)
                return true;

            return false;
        }

        //ALUMINIUM
        public int IsAluminium(DataRow FrontsOrdersRow)
        {

            DataRow[] Row = FrontsDataTable.Select("FrontID = " + FrontsOrdersRow["FrontID"].ToString());

            if (Row[0]["FrontName"].ToString()[0] == 'Z')
                return Convert.ToInt32(Row[0]["FrontID"]);

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
        }

        private decimal GetJobPriceAluminium(DataRow FrontsOrdersRow)
        {
            DataRow[] Rows = AluminiumFrontsDataTable.Select("FrontID = " + IsAluminium(FrontsOrdersRow));

            return Convert.ToDecimal(Rows[0]["JobPrice"]);
        }

        private decimal FrontsPriceAluminium(DataRow FrontsOrdersRow)
        {
            decimal Price = 0;

            DataRow[] Rows = AluminiumFrontsDataTable.Select("FrontID = " + IsAluminium(FrontsOrdersRow));

            Price = Convert.ToDecimal(Rows[0]["ProfilPrice"]);

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

        public decimal GetFrontCostAluminium(DataRow FrontsOrdersRow, ref decimal Square, bool IsSample, bool TechDrilling)
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

            decimal SampleSaleValue = Convert.ToDecimal(FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString())[0]["SampleSaleValue"]);
            decimal SaleValue = Convert.ToDecimal(FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"])[0]["SaleValue"]);
            if (Sale)
            {
                if (IsSample)
                    Cost = (Cost - Cost / 100 * SampleSaleValue);
                else
                    Cost = (Cost - Cost / 100 * SaleValue);
            }
            else
                if (IsSample)
                Cost = Cost * 0.5m;

            if (TechDrilling)
                Cost = Cost * 1.3m;
            Square = (Height * Width * Count) / 1000000;

            FrontsOrdersRow["InsetPrice"] = 0;
            FrontsOrdersRow["Cost"] = Cost;
            FrontsOrdersRow["Square"] = Square;
            FrontsOrdersRow["FrontPrice"] = Cost / Convert.ToDecimal(FrontsOrdersRow["Square"]);

            return Cost;
        }

        private decimal CalculateItem(DataRow FrontsOrdersRow, ref decimal ItemSquare, int PriceTypeID, bool IsSample, bool TechDrilling)
        {
            decimal Cost = 0;

            int FrontID = Convert.ToInt32(FrontsOrdersRow["FrontID"]);
            int InsetTypeID = Convert.ToInt32(FrontsOrdersRow["InsetTypeID"]);
            decimal Height = Convert.ToDecimal(FrontsOrdersRow["Height"]);
            decimal Width = Convert.ToDecimal(FrontsOrdersRow["Width"]);
            decimal Count = Convert.ToDecimal(FrontsOrdersRow["Count"]);

            decimal Square = 0;

            decimal FrontCost = 0;
            decimal InsetCost = 0;

            //ALUMINIUM
            if (IsAluminium(FrontsOrdersRow) > -1)
            {
                return GetFrontCostAluminium(FrontsOrdersRow, ref ItemSquare, IsSample, TechDrilling);
            }

            string PriceType = "";

            if (PriceTypeID == 0)
                PriceType = "RetailPrice";
            if (PriceTypeID == 1)
                PriceType = "WholePrice";
            if (PriceTypeID == 2)
                PriceType = "WallPrice";

            decimal dFrontPrice = GetFrontPrice(FrontsOrdersRow, PriceType, IsSample, TechDrilling);
            decimal dInsetPrice = InsetPrice(FrontsOrdersRow, IsSample, TechDrilling);

            if (IsMeasureSquare(FrontsOrdersRow))
            {
                Square = Height * Width / 1000000 * Count;
                FrontCost = Square * dFrontPrice;
            }
            else
            {
                FrontCost = dFrontPrice * Count;
            }

            if (dInsetPrice > 0)
            {
                if (FrontID == 3728 || FrontID == 3731 ||
                    FrontID == 3732 || FrontID == 3739 ||
                    FrontID == 3740 || FrontID == 3741 ||
                    FrontID == 3744 || FrontID == 3745 ||
                    FrontID == 3746 ||
                    InsetTypeID == 28961 || InsetTypeID == 3653 || InsetTypeID == 3654 || InsetTypeID == 3655
                    )
                {
                    InsetCost = dInsetPrice * Count;
                }
                else
                {
                    decimal dInsetSquare = GetInsetSquare(FrontsOrdersRow);
                    InsetCost = dInsetPrice * dInsetSquare * Count;
                }
            }

            FrontsOrdersRow["InsetPrice"] = dInsetPrice;

            Cost = FrontCost + InsetCost;

            FrontsOrdersRow["Square"] = Square;
            FrontsOrdersRow["Cost"] = Decimal.Round(Cost, 3, MidpointRounding.AwayFromZero);

            ItemSquare = Square;

            return Cost;
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

        public decimal CalculateFrontsWeight(DataRow FrontsOrdersRow)
        {
            decimal FrontsWeight = 0;
            DataRow[] FrontsConfigRow = FrontsConfigDataTable.Select("FrontConfigID = " + FrontsOrdersRow["FrontConfigID"].ToString());
            decimal InsetWeight = Convert.ToDecimal(FrontsConfigRow[0]["InsetWeight"]);
            //если гнутый то вес за штуки
            if (FrontsConfigRow[0]["Width"].ToString() == "-1")
            {
                FrontsOrdersRow["ItemWeight"] = FrontsConfigRow[0]["Weight"];
                FrontsOrdersRow["Weight"] = Convert.ToDecimal(FrontsOrdersRow["Count"]) * Convert.ToDecimal(FrontsConfigRow[0]["Weight"]);
                return Convert.ToDecimal(FrontsOrdersRow["Count"]) * Convert.ToDecimal(FrontsConfigRow[0]["Weight"]);
            }
            //если алюминий
            if (IsAluminium(FrontsOrdersRow) > -1)
            {
                decimal W = GetAluminiumWeight(FrontsOrdersRow, true);
                FrontsOrdersRow["ItemWeight"] = W / Convert.ToDecimal(FrontsOrdersRow["Count"]);
                FrontsOrdersRow["Weight"] = W;
                return W;
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

            FrontsWeight = ResultProfileWeight + ResultInsetWeight;
            FrontsOrdersRow["ItemWeight"] = FrontsWeight;
            FrontsOrdersRow["Weight"] = FrontsWeight * Convert.ToInt32(FrontsOrdersRow["Count"]);

            return FrontsWeight * Convert.ToInt32(FrontsOrdersRow["Count"]);
        }


        //общий расчет фасадов в заказе
        public decimal CalculateFronts(int MainOrderID, ref decimal OrderSquare, ref decimal FrontsWeight, int PriceTypeID, bool IsSample, bool TechDrilling)
        {
            Sale = false;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DocDateTime FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        DateTime DocDateTime = Convert.ToDateTime(DT.Rows[0]["DocDateTime"]);
                        if (DocDateTime.Date >= new DateTime(2016, 02, 01) && DocDateTime.Date <= new DateTime(2016, 03, 09))
                        {
                            Sale = true;
                        }
                    }
                }
            }
            decimal FrontsOrderCost = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable FrontsOrdersDataTable = new DataTable())
                    {
                        DA.Fill(FrontsOrdersDataTable);

                        if (FrontsOrdersDataTable.Rows.Count == 0)
                            return 0;

                        foreach (DataRow Row in FrontsOrdersDataTable.Rows)
                        {
                            decimal ItemSquare = 0;

                            FrontsWeight += CalculateFrontsWeight(Row);

                            FrontsOrderCost += CalculateItem(Row, ref ItemSquare, PriceTypeID, IsSample, TechDrilling);

                            OrderSquare += ItemSquare;
                        }

                        DA.Update(FrontsOrdersDataTable);
                    }
                }
            }

            return FrontsOrderCost;
        }

    }






    public class DecorCalculate
    {
        bool Sale = false;
        DataTable DecorConfigDataTable = null;

        public DecorCalculate()
        {
            Initialize();
        }

        private void Fill()
        {

            DecorConfigDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorConfig",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDataTable);
            //}
            DecorConfigDataTable = TablesManager.DecorConfigDataTable;

        }

        private void Initialize()
        {
            Fill();
        }

        private decimal GetDecorPrice(DataRow DecorOrderRow)
        {
            return Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString())[0]["ZOVPrice"]);
        }

        private int GetMeasureType(DataRow DecorOrderRow)
        {
            DataRow[] Rows = DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString());

            return Convert.ToInt32(Rows[0]["MeasureID"]);
        }

        private decimal GetPLK110Cost(DataRow DecorOrderRow, bool IsSample)
        {
            decimal Cost = 0;
            decimal Price = GetDecorPrice(DecorOrderRow);

            decimal SampleSaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString())[0]["SampleSaleValue"]);
            decimal SaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"])[0]["SaleValue"]);
            if (Sale)
            {
                if (IsSample)
                    Price = (Price - Price / 100 * SampleSaleValue);
                else
                    Price = (Price - Price / 100 * SaleValue);
            }
            else
                if (IsSample)
                Price = Price * 0.5m;
            DecorOrderRow["Price"] = Price;
            decimal Length = Convert.ToDecimal(DecorOrderRow["Length"]);

            if (IsSample)
                Cost = Decimal.Round(Convert.ToInt32(DecorOrderRow["Count"]) * (Length / 1000 * Price + 25 * 1.2m), 3, MidpointRounding.AwayFromZero);
            else
            {

                Cost = Decimal.Round(Convert.ToInt32(DecorOrderRow["Count"]) * (Length / 1000 * Price + 50 * 1.2m), 3, MidpointRounding.AwayFromZero);
            }

            return Cost;
        }

        private decimal GetBalustradeCost(DataRow DecorOrderRow, bool IsSample)
        {
            decimal Cost = 0;
            decimal Price = GetDecorPrice(DecorOrderRow);

            decimal SampleSaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString())[0]["SampleSaleValue"]);
            decimal SaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"])[0]["SaleValue"]);
            if (Sale)
            {
                if (IsSample)
                    Price = (Price - Price / 100 * SampleSaleValue);
                else
                    Price = (Price - Price / 100 * SaleValue);
            }
            else
                if (IsSample)
                Price = Price * 0.5m;
            DecorOrderRow["Price"] = Price;

            decimal Length = Convert.ToDecimal(DecorOrderRow["Length"]);

            if (DecorOrderRow["DecorID"].ToString() == "2122")//бл-01
                Cost = Decimal.Round(Length / 1000 * Price * Convert.ToInt32(DecorOrderRow["Count"]), 3, MidpointRounding.AwayFromZero);

            if (DecorOrderRow["DecorID"].ToString() == "2123")//бл-02
                Cost = Decimal.Round(Length / 1000 * Price * Convert.ToInt32(DecorOrderRow["Count"]), 3, MidpointRounding.AwayFromZero);

            if (DecorOrderRow["DecorID"].ToString() == "14901" || DecorOrderRow["DecorID"].ToString() == "14902")//бл-03
                Cost = Decimal.Round(Length / 1000 * Price * Convert.ToInt32(DecorOrderRow["Count"]), 3, MidpointRounding.AwayFromZero);

            if (DecorOrderRow["DecorID"].ToString() == "15446")//бл-04
                Cost = Decimal.Round(Length / 1000 * Price * Convert.ToInt32(DecorOrderRow["Count"]), 3, MidpointRounding.AwayFromZero);

            return Cost;
        }

        private decimal GetGridCost(DataRow DecorOrderRow, bool IsSample)
        {
            decimal Cost = 0;

            decimal Length = Convert.ToDecimal(DecorOrderRow["Length"]);
            decimal Height = Convert.ToDecimal(DecorOrderRow["Height"]);
            decimal Width = Convert.ToDecimal(DecorOrderRow["Width"]);
            decimal Count = Convert.ToDecimal(DecorOrderRow["Count"]);
            decimal Square = 0;
            if (Length == -1)
                Square = Height * Width / 1000000;
            if (Height == -1)
                Square = Length * Width / 1000000;

            decimal Price = 0;

            DataRow[] Rows = DecorConfigDataTable.Select("DecorID = " + DecorOrderRow["DecorID"].ToString());
            if (Rows.Count() > 0)
            {
                if (Rows[0]["ZOVPrice"] != DBNull.Value)
                {
                    Price = Convert.ToDecimal(Rows[0]["ZOVPrice"]);
                    //return Price;
                }
            }

            decimal SampleSaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString())[0]["SampleSaleValue"]);
            decimal SaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"])[0]["SaleValue"]);
            if (Sale)
            {
                if (IsSample)
                    Price = (Price - Price / 100 * SampleSaleValue);
                else
                    Price = (Price - Price / 100 * SaleValue);
            }
            else
                if (IsSample)
                Price = Price * 0.5m;
            Cost = Square * Count * Price;
            DecorOrderRow["Price"] = Price;
            return Cost;
        }

        public decimal CalculateItem(DataRow DecorOrderRow, bool IsSample)
        {
            decimal Cost = 0;

            decimal Price = 0;
            int MeasureType = -1;

            decimal Height = -1;
            decimal Width = -1;
            decimal Length = -1;
            decimal Count = -1;

            decimal SampleSaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"].ToString())[0]["SampleSaleValue"]);
            decimal SaleValue = Convert.ToDecimal(DecorConfigDataTable.Select("DecorConfigID = " + DecorOrderRow["DecorConfigID"])[0]["SaleValue"]);
            Price = Decimal.Round(GetDecorPrice(DecorOrderRow), 2, MidpointRounding.AwayFromZero);
            MeasureType = GetMeasureType(DecorOrderRow);
            if (Sale)
            {
                if (IsSample)
                    Price = (Price - Price / 100 * SampleSaleValue);
                else
                    Price = (Price - Price / 100 * SaleValue);
            }
            else
                if (IsSample)
                Price = Price * 0.5m;
            try
            {
                Height = Convert.ToDecimal(DecorOrderRow["Height"]);
            }
            catch
            { }

            try
            {
                Length = Convert.ToDecimal(DecorOrderRow["Length"]);
            }
            catch { }

            try
            {
                Width = Convert.ToDecimal(DecorOrderRow["Width"]);
            }
            catch { }

            try
            {
                Count = Convert.ToDecimal(DecorOrderRow["Count"]);
            }
            catch { }

            DecorOrderRow["Price"] = Price;

            if (MeasureType == 1)
            {
                if (Height != -1)             //м.кв.
                    Cost = Height * Width / 1000000 * Count * Price;
                if (Length != -1)
                    Cost = Length * Width / 1000000 * Count * Price;
            }

            if (MeasureType == 2)
            {
                if (Height != -1)
                    Cost = Height / 1000 * Count * Price;
                if (Length != -1)
                    Cost = Length / 1000 * Count * Price;
            }

            if (MeasureType == 3)                              //шт.
                Cost = Count * Price;

            return Cost;
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
            if (Row[0]["Weight"] != DBNull.Value)
            {
                if (Row[0]["WeightMeasureID"].ToString() == "1")
                {
                    if (Convert.ToInt32(DecorOrderRow["Height"]) != -1)
                        Weight = Convert.ToDecimal(DecorOrderRow["Height"]) * Convert.ToDecimal(DecorOrderRow["Width"]) / 1000000
                             * Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);
                    if (Convert.ToInt32(DecorOrderRow["Length"]) != -1)
                        Weight = Convert.ToDecimal(DecorOrderRow["Length"]) * Convert.ToDecimal(DecorOrderRow["Width"]) / 1000000
                            * Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);
                }
                if (Row[0]["WeightMeasureID"].ToString() == "2")
                {
                    if (Convert.ToInt32(DecorOrderRow["Height"]) != -1)
                        Weight = Convert.ToDecimal(DecorOrderRow["Height"]) / 1000 * Convert.ToDecimal(Row[0]["Weight"]) *
                                 Convert.ToDecimal(DecorOrderRow["Count"]);
                    if (Convert.ToInt32(DecorOrderRow["Length"]) != -1)
                        Weight = Convert.ToDecimal(DecorOrderRow["Length"]) / 1000 * Convert.ToDecimal(Row[0]["Weight"]) *
                             Convert.ToDecimal(DecorOrderRow["Count"]);
                }
                if (Row[0]["WeightMeasureID"].ToString() == "3")
                    Weight = Convert.ToDecimal(Row[0]["Weight"]) * Convert.ToDecimal(DecorOrderRow["Count"]);
            }
            DecorOrderRow["ItemWeight"] = Weight / Convert.ToDecimal(DecorOrderRow["Count"]);
            DecorOrderRow["Weight"] = Weight;

            return Weight;
        }

        public decimal CalculateDecor(int MainOrderID, ref decimal DecorOrderWeight, bool IsSample)
        {
            Sale = false;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DocDateTime FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        DateTime DocDateTime = Convert.ToDateTime(DT.Rows[0]["DocDateTime"]);
                        if (DocDateTime.Date >= new DateTime(2016, 02, 01) && DocDateTime.Date <= new DateTime(2016, 03, 09))
                        {
                            Sale = true;
                        }
                    }
                }
            }
            decimal Cost = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        //DT.TableName = DecorCatalogOrder.DecorProductsDataTable.Rows[i]["OrdersTableName"].ToString();

                        if (DT.Rows.Count == 0)
                            return 0;

                        foreach (DataRow Row in DT.Rows)
                        {
                            decimal ItemCost = 0;

                            DecorOrderWeight += GetDecorWeight(Row);

                            if (Row["ProductID"].ToString() == "2")//balustrade
                                ItemCost += GetBalustradeCost(Row, IsSample);
                            else
                            {
                                if (Row["ProductID"].ToString() == "11" || Row["ProductID"].ToString() == "12")
                                    ItemCost += GetGridCost(Row, IsSample);
                                else
                                {
                                    if (Row["DecorID"].ToString() == "15523")//плк-110
                                        ItemCost += GetPLK110Cost(Row, IsSample);
                                    else
                                        ItemCost += CalculateItem(Row, IsSample);
                                }
                            }
                            Row["Cost"] = Decimal.Round(ItemCost, 3, MidpointRounding.AwayFromZero);

                            Cost += ItemCost;
                        }

                        DA.Update(DT);
                    }
                }
            }

            return Cost;
        }
    }







    public class Dispatch
    {
        public MainOrdersFrontsOrders MainOrdersFrontsOrders = null;
        public MainOrdersDecorOrders MainOrdersDecorOrders = null;

        PercentageDataGrid DebtsDatGrid = null;
        PercentageDataGrid DispatchsDataGrid = null;

        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        public DataTable ClientsDataTable = null;
        public DataTable DispatchsDataTable = null;
        public DataTable DebtsDataTable = null;

        public BindingSource ClientsBindingSource = null;
        public BindingSource DispatchsBindingSource = null;
        public BindingSource DebtsBindingSource = null;

        public SqlDataAdapter DebtDataAdapter = null;
        public SqlCommandBuilder DebtCB = null;

        public BindingSource DispatchDocNumbersBindingSource = null;

        private DataGridViewComboBoxColumn ClientColumn = null;


        public Dispatch(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
                             ref PercentageDataGrid tDispatchsDataGrid,
                             ref PercentageDataGrid tDebtsDataGrid,
                             ref DevExpress.XtraTab.XtraTabControl tMainOrdersDecorTabControl,
                             ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl,
                             ref DecorCatalogOrder DecorCatalogOrder)
        {
            OrdersTabControl = tOrdersTabControl;
            DispatchsDataGrid = tDispatchsDataGrid;
            DebtsDatGrid = tDebtsDataGrid;

            MainOrdersFrontsOrders = new MainOrdersFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid)
            {
                Debts = true
            };
            MainOrdersFrontsOrders.Initialize(true);



            MainOrdersDecorOrders = new MainOrdersDecorOrders(ref tMainOrdersDecorTabControl,
                ref DecorCatalogOrder, ref tMainOrdersFrontsOrdersDataGrid)
            {
                Debts = true
            };
            MainOrdersDecorOrders.Initialize(true);

            Initialize();
        }


        private void Create()
        {
            DebtsDataTable = new DataTable();
            DispatchsDataTable = new DataTable();

            ClientsBindingSource = new BindingSource();
            DebtsBindingSource = new BindingSource();
            DispatchsBindingSource = new BindingSource();
            DispatchDocNumbersBindingSource = new BindingSource();
        }

        private void Fill()
        {
            DebtDataAdapter = new SqlDataAdapter("SELECT * FROM Debts ORDER BY DispatchDate", ConnectionStrings.ZOVOrdersConnectionString);
            DebtCB = new SqlCommandBuilder(DebtDataAdapter);
            DebtDataAdapter.Fill(DebtsDataTable);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DispatchDate) FROM Debts ORDER BY DispatchDate",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchsDataTable);
            }

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
        }

        public void Binding()
        {
            ClientsBindingSource.DataSource = new DataView(ClientsDataTable);
            DebtsBindingSource.DataSource = DebtsDataTable;
            DispatchsBindingSource.DataSource = DispatchsDataTable;

            DebtsDatGrid.DataSource = DebtsBindingSource;
            DispatchsDataGrid.DataSource = DispatchsBindingSource;

            DispatchDocNumbersBindingSource.DataSource = new DataView(DebtsDataTable);
        }

        public void CreateColumns()
        {
            if (ClientColumn != null)
                ClientColumn.Dispose();

            ClientColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ClientColumn",
                HeaderText = "Клиент",
                DataPropertyName = "ClientID",
                DataSource = new DataView(ClientsDataTable),
                ValueMember = "ClientID",
                DisplayMember = "ClientName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            DebtsDatGrid.Columns.Add(ClientColumn);
        }

        public void SetGrids()
        {
            DebtsDatGrid.Columns["DebtID"].Visible = false;
            DebtsDatGrid.Columns["ClientID"].Visible = false;
            DebtsDatGrid.Columns["MainOrderID"].Visible = false;
            DebtsDatGrid.Columns["DispatchDate"].Visible = false;

            DebtsDatGrid.Columns["ClientColumn"].DisplayIndex = 0;
            DebtsDatGrid.Columns["DocNumber"].DisplayIndex = 1;

            DebtsDatGrid.Columns["ClientColumn"].HeaderText = "Клиент";
            DebtsDatGrid.Columns["DocNumber"].HeaderText = "№ документа";

            DispatchsDataGrid.Columns["DispatchDate"].HeaderText = "Дата отгрузки";
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            SetGrids();
        }


        public void FilterDebts(DateTime DispatchDate)
        {
            DebtsBindingSource.Filter = "DispatchDate = '" + DispatchDate + "'";
        }

        public void Filter(int MainOrderID)
        {
            OrdersTabControl.TabPages[0].PageVisible = MainOrdersFrontsOrders.FilterDebts(MainOrderID);
            OrdersTabControl.TabPages[1].PageVisible = MainOrdersDecorOrders.FilterDebts(MainOrderID);

            if (OrdersTabControl.TabPages[0].PageVisible == false && OrdersTabControl.TabPages[1].PageVisible == false)
                OrdersTabControl.Visible = false;
            else
                OrdersTabControl.Visible = true;
        }


        public void FindDocNumber(string DocNumber)
        {
            int index = DispatchsBindingSource.Find("DispatchDate",
                                DebtsDataTable.Select("DocNumber = '" + DocNumber + "'")[0]["DispatchDate"]);

            if (index == -1)
                return;

            DispatchsBindingSource.Position = index;
            DebtsBindingSource.Position = DebtsBindingSource.Find("DocNumber", DocNumber);
        }

        public void Refresh()
        {
            DispatchsDataTable.Clear();
            DebtsDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Debts ORDER BY DispatchDate", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DebtsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DispatchDate) FROM Debts ORDER BY DispatchDate", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchsDataTable);
            }
        }

        public void ClearDebt(int MainOrderID)
        {
            DebtsBindingSource.RemoveCurrent();
            DebtDataAdapter.Update(DebtsDataTable);

            if (DebtsBindingSource.Count == 0)
                DispatchsBindingSource.RemoveCurrent();
        }
    }








    public class OrdersManager
    {
        public int CurrentClientID = 0;
        public bool IsPrepare = true;
        public object CurrentDispatchDate = null;

        public bool InEditing = false;
        public bool InNew = false;
        public bool NeedFillBatch = true;

        public bool SendReport = false;

        public int ManagerID = 0;
        public int CurrentMainOrderID = -1;
        public int CurrentMegaOrderID = -1;

        public MainOrdersFrontsOrders MainOrdersFrontsOrders = null;
        public MainOrdersDecorOrders MainOrdersDecorOrders = null;

        public PercentageDataGrid MainOrdersDataGrid = null;
        public PercentageDataGrid MegaOrdersDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        public DataTable ManagersDataTable = null;
        public DataTable ClientsDataTable = null;
        public DataTable ClientsGroupsDataTable = null;
        public DataTable MainOrdersDataTable = null;
        private DataTable OrderStatusesDataTable = null;
        public DataTable FactoryTypesDataTable = null;
        public DataTable ClientsMegaOrdersDataTable = null;
        public DataTable DebtTypesDataTable = null;
        public DataTable DebtTypesFullDataTable = null;
        public DataTable PriceTypesDataTable = null;
        public DataTable SearchMainOrdersDataTable = null;
        public DataTable FrontsDrillTypesDataTable = null;

        private DataTable ProductionStatusesDataTable = null;
        private DataTable StorageStatusesDataTable = null;
        private DataTable ExpeditionStatusesDataTable = null;
        private DataTable DispatchStatusesDataTable = null;
        private DataTable BatchDetailsDataTable = null;
        private DataTable DocNumbersDataTable = null;

        public DataTable MegaOrdersDataTable = null;

        public SqlDataAdapter MainOrdersDataAdapter = null;
        public SqlCommandBuilder MainOrdersCommandBuilder = null;

        public SqlDataAdapter MegaOrdersDataAdapter = null;
        public SqlCommandBuilder MegaOrdersCommandBuilder = null;

        public SqlDataAdapter ClientsGroupsDataAdapter = null;
        public SqlCommandBuilder ClientsGroupsCommandBuilder = null;

        public SqlDataAdapter ClientsDataAdapter = null;
        public SqlCommandBuilder ClientsCommandBuilder = null;

        public BindingSource ManagersBindingSource = null;
        public BindingSource ClientsBindingSource = null;
        public BindingSource ClientsGroupsBindingSource = null;
        public BindingSource MainOrdersBindingSource = null;
        public BindingSource OrderStatusesBindingSource = null;
        public BindingSource FactoryTypesBindingSource = null;
        public BindingSource MegaOrdersBindingSource = null;
        public BindingSource PriceTypesBindingSource = null;
        public BindingSource DebtTypesBindingSource = null;
        public BindingSource DebtTypesFullBindingSource = null;
        public BindingSource SearchDocNumberBindingSource = null;
        public BindingSource SearchPartDocNumberBindingSource = null;
        public BindingSource DocNumbersBindingSource = null;

        public String ClientsBindingSourceDisplayMember = null;

        public String ClientsBindingSourceValueMember = null;

        private DataGridViewComboBoxColumn ProfilProductionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilStorageStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn TPSProductionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSStorageStatusColumn = null;
        private DataGridViewComboBoxColumn TPSExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn PriceTypeColumn = null;
        private DataGridViewComboBoxColumn DebtTypeColumn = null;
        private DataGridViewComboBoxColumn ClientColumn = null;

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

        public OrdersManager(ref PercentageDataGrid tMainOrdersDataGrid,
                             ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
                             ref PercentageDataGrid tMegaOrdersDataGrid,
                             ref DevExpress.XtraTab.XtraTabControl tMainOrdersDecorTabControl,
                             ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl,
                             ref DecorCatalogOrder DecorCatalogOrder, bool bNeedFillBatch)
        {
            NeedFillBatch = bNeedFillBatch;
            MainOrdersDataGrid = tMainOrdersDataGrid;
            MegaOrdersDataGrid = tMegaOrdersDataGrid;
            OrdersTabControl = tOrdersTabControl;

            MainOrdersFrontsOrders = new MainOrdersFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid);
            MainOrdersFrontsOrders.Initialize(true);

            MainOrdersDecorOrders = new MainOrdersDecorOrders(ref tMainOrdersDecorTabControl,
                ref DecorCatalogOrder, ref tMainOrdersFrontsOrdersDataGrid);
            MainOrdersDecorOrders.Initialize(true);

            InitializeTablesManager();
        }


        private void Create()
        {
            MainOrdersDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            DebtTypesDataTable = new DataTable();
            ManagersDataTable = new DataTable();
            ClientsDataTable = new DataTable();
            ClientsGroupsDataTable = new DataTable();
            SearchMainOrdersDataTable = new DataTable();

            ProductionStatusesDataTable = new DataTable();
            StorageStatusesDataTable = new DataTable();
            ExpeditionStatusesDataTable = new DataTable();
            DispatchStatusesDataTable = new DataTable();
            DocNumbersDataTable = new DataTable();

            ManagersBindingSource = new BindingSource();
            ClientsBindingSource = new BindingSource();
            ClientsGroupsBindingSource = new BindingSource();
            MainOrdersBindingSource = new BindingSource();
            MegaOrdersBindingSource = new BindingSource();
            OrderStatusesBindingSource = new BindingSource();
            FactoryTypesBindingSource = new BindingSource();
            DebtTypesBindingSource = new BindingSource();
            PriceTypesBindingSource = new BindingSource();
            DebtTypesFullBindingSource = new BindingSource();
            SearchDocNumberBindingSource = new BindingSource();
            SearchPartDocNumberBindingSource = new BindingSource();
            DocNumbersBindingSource = new BindingSource();
        }

        private void Fill()
        {
            MainOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders", ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersCommandBuilder = new SqlCommandBuilder(MainOrdersDataAdapter);
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);
            MainOrdersDataTable.Columns.Add(new DataColumn("BatchNumber", Type.GetType("System.String")));

            DateTime DateFrom = Security.GetCurrentDate().AddDays(-8);
            DateTime DateTo = Security.GetCurrentDate();

            string SelectionCommand = "SELECT * FROM MegaOrders" +
               " WHERE MegaOrderID = 0 OR (MegaOrderID > 11025 AND CAST(DispatchDate AS Date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
               "' AND CAST(DispatchDate AS Date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" +
               " ORDER BY DispatchDate";

            MegaOrdersDataAdapter = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, Name FROM Managers ORDER BY Name", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ManagersDataTable);
            }

            ClientsDataAdapter = new SqlDataAdapter("SELECT * FROM Clients ORDER BY ClientName", ConnectionStrings.ZOVReferenceConnectionString);
            ClientsCommandBuilder = new SqlCommandBuilder(ClientsDataAdapter);
            ClientsDataAdapter.Fill(ClientsDataTable);

            ClientsGroupsDataAdapter = new SqlDataAdapter("SELECT * FROM ClientsGroups ORDER BY ClientGroupName", ConnectionStrings.ZOVReferenceConnectionString);
            ClientsGroupsCommandBuilder = new SqlCommandBuilder(ClientsGroupsDataAdapter);
            ClientsGroupsDataAdapter.Fill(ClientsGroupsDataTable);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DebtTypes WHERE DebtTypeID > 0", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DebtTypesDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, MainOrderID, DocNumber FROM MainOrders", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SearchMainOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ProductionStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ProductionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StorageStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(StorageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ExpeditionStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ExpeditionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DispatchStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DispatchStatusesDataTable);
            }

            FrontsDrillTypesDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsDrillTypes", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDrillTypesDataTable);
            }

            DebtTypesFullDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DebtTypes", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DebtTypesFullDataTable);
            }
            FactoryTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDataTable);
            }
            OrderStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OrderStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(OrderStatusesDataTable);
            }
            PriceTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PriceTypes", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(PriceTypesDataTable);
            }

            BatchDetailsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Batch.MegaBatchID, BatchDetails.BatchID," +
                " BatchDetails.MainOrderID, MainOrders.MegaOrderID FROM BatchDetails" +
                " INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                " INNER JOIN MainOrders ON BatchDetails.MainOrderID = MainOrders.MainOrderID" +
                " ORDER BY Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(BatchDetailsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DocNumber) FROM MainOrders" +
                " ORDER BY DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDataTable);
            }

        }

        public DataTable ReturnMarketingClients
        {
            get
            {
                DataTable DT = new DataTable();
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                }

                using (DataView DV = new DataView(DT))
                {
                    DV.Sort = "ClientName";
                    return DV.ToTable();
                }
            }
        }

        private void FillTablesManager()
        {
            MainOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders", ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersCommandBuilder = new SqlCommandBuilder(MainOrdersDataAdapter);
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);
            if (NeedFillBatch)
                MainOrdersDataTable.Columns.Add(new DataColumn("BatchNumber", Type.GetType("System.String")));

            DateTime DateFrom = Security.GetCurrentDate().AddDays(-8);
            DateTime DateTo = Security.GetCurrentDate();

            string SelectionCommand = "SELECT * FROM MegaOrders" +
               " WHERE MegaOrderID = 0 OR (MegaOrderID > 11025 AND CAST(DispatchDate AS Date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
               "' AND CAST(DispatchDate AS Date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" +
               " ORDER BY DispatchDate";

            MegaOrdersDataAdapter = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, Name FROM Managers ORDER BY Name", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ManagersDataTable);
            }

            //SqlCommand com = MegaOrdersCommandBuilder.GetUpdateCommand();
            //SqlCommand com1 = MegaOrdersDataAdapter.SelectCommand;

            ClientsDataAdapter = new SqlDataAdapter("SELECT * FROM Clients ORDER BY ClientName", ConnectionStrings.ZOVReferenceConnectionString);
            ClientsCommandBuilder = new SqlCommandBuilder(ClientsDataAdapter);
            ClientsDataAdapter.Fill(ClientsDataTable);

            ClientsGroupsDataAdapter = new SqlDataAdapter("SELECT * FROM ClientsGroups ORDER BY ClientGroupName", ConnectionStrings.ZOVReferenceConnectionString);
            ClientsGroupsCommandBuilder = new SqlCommandBuilder(ClientsGroupsDataAdapter);
            ClientsGroupsDataAdapter.Fill(ClientsGroupsDataTable);

            DebtTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DebtTypes WHERE DebtTypeID > 0", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DebtTypesDataTable);
            }
            SearchMainOrdersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, MainOrderID, DocNumber FROM MainOrders", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SearchMainOrdersDataTable);
            }

            ProductionStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ProductionStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ProductionStatusesDataTable);
            }

            StorageStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StorageStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(StorageStatusesDataTable);
            }
            ExpeditionStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ExpeditionStatuses",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ExpeditionStatusesDataTable);
            }
            DispatchStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DispatchStatuses",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DispatchStatusesDataTable);
            }


            FrontsDrillTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsDrillTypes", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDrillTypesDataTable);
            }

            DebtTypesFullDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DebtTypes", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DebtTypesFullDataTable);
            }
            FactoryTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDataTable);
            }
            OrderStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OrderStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(OrderStatusesDataTable);
            }
            PriceTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PriceTypes", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(PriceTypesDataTable);
            }

            BatchDetailsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Batch.MegaBatchID, BatchDetails.BatchID," +
                " BatchDetails.MainOrderID, MainOrders.MegaOrderID FROM BatchDetails" +
                " INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                " INNER JOIN MainOrders ON BatchDetails.MainOrderID = MainOrders.MainOrderID" +
                " ORDER BY Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(BatchDetailsDataTable);
            }

            DocNumbersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DocNumber) FROM MainOrders" +
                " ORDER BY DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDataTable);
            }

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

        public void InitializeTablesManager()
        {
            Create();
            FillTablesManager();
            Binding();
            CreateColumns();
            MainGridSetting();
            MegaGridSetting();
        }

        private void Binding()
        {
            ManagersBindingSource.DataSource = ManagersDataTable;
            ClientsBindingSource.DataSource = ClientsDataTable;
            ClientsGroupsBindingSource.DataSource = ClientsGroupsDataTable;

            ClientsBindingSourceDisplayMember = "ClientName";
            ClientsBindingSourceValueMember = "ClientID";

            FactoryTypesBindingSource.DataSource = FactoryTypesDataTable;

            DocNumbersBindingSource.DataSource = DocNumbersDataTable;

            OrderStatusesBindingSource.DataSource = OrderStatusesDataTable;

            PriceTypesBindingSource.DataSource = PriceTypesDataTable;
            DebtTypesFullBindingSource.DataSource = DebtTypesFullDataTable;

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
            MainOrdersBindingSource.DataSource = MainOrdersDataTable;

            SearchDocNumberBindingSource.DataSource = SearchMainOrdersDataTable;
            SearchPartDocNumberBindingSource.DataSource = new DataView(SearchMainOrdersDataTable);

            MainOrdersDataGrid.DataSource = MainOrdersBindingSource;
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;
            MegaOrdersBindingSource.MoveLast();



            DebtTypesBindingSource.DataSource = DebtTypesDataTable;
        }

        private void CreateColumns()
        {
            ProfilProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilProductionStatusColumn",
                HeaderText = "Пр-во\n\rПрофиль",
                DataPropertyName = "ProfilProductionStatusID",
                DataSource = new DataView(ProductionStatusesDataTable),
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
                DataSource = new DataView(StorageStatusesDataTable),
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
                DataSource = new DataView(ExpeditionStatusesDataTable),
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
                DataSource = new DataView(DispatchStatusesDataTable),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSProductionStatusColumn",
                HeaderText = "Пр-во\n\rТПС",
                DataPropertyName = "TPSProductionStatusID",
                DataSource = new DataView(ProductionStatusesDataTable),
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
                DataSource = new DataView(StorageStatusesDataTable),
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
                DataSource = new DataView(ExpeditionStatusesDataTable),
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
                DataSource = new DataView(DispatchStatusesDataTable),
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
            PriceTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PriceTypeColumn",
                HeaderText = "Тип\r\nпрайса",
                DataPropertyName = "PriceTypeID",
                DataSource = PriceTypesBindingSource,
                ValueMember = "PriceTypeID",
                DisplayMember = "PriceTypeRus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            DebtTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DebtTypeColumn",
                HeaderText = "Долг",
                DataPropertyName = "DebtTypeID",
                DataSource = DebtTypesFullBindingSource,
                ValueMember = "DebtTypeID",
                DisplayMember = "DebtType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ClientColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ClientColumn",
                HeaderText = "Клиент",
                DataPropertyName = "ClientID",
                DataSource = new DataView(ClientsDataTable),
                ValueMember = "ClientID",
                DisplayMember = "ClientName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MainOrdersDataGrid.Columns.Add(ProfilProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilDispatchStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSDispatchStatusColumn);
            MainOrdersDataGrid.Columns.Add(FactoryTypeColumn);
            MainOrdersDataGrid.Columns.Add(PriceTypeColumn);
            MainOrdersDataGrid.Columns.Add(DebtTypeColumn);
            MainOrdersDataGrid.Columns.Add(ClientColumn);
        }

        private void MainGridSetting()
        {
            foreach (DataGridViewColumn Column in MainOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //MainOrdersDataGrid.ScrollBars = ScrollBars.Vertical;

            if (MainOrdersDataGrid.Columns.Contains("ProfilProductionStatusID"))
                MainOrdersDataGrid.Columns["ProfilProductionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilStorageStatusID"))
                MainOrdersDataGrid.Columns["ProfilStorageStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilExpeditionStatusID"))
                MainOrdersDataGrid.Columns["ProfilExpeditionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilDispatchStatusID"))
                MainOrdersDataGrid.Columns["ProfilDispatchStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSProductionStatusID"))
                MainOrdersDataGrid.Columns["TPSProductionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSStorageStatusID"))
                MainOrdersDataGrid.Columns["TPSStorageStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSExpeditionStatusID"))
                MainOrdersDataGrid.Columns["TPSExpeditionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSDispatchStatusID"))
                MainOrdersDataGrid.Columns["TPSDispatchStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("DispatchID"))
                MainOrdersDataGrid.Columns["DispatchID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("FirstOperatorID"))
                MainOrdersDataGrid.Columns["FirstOperatorID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("SecondOperatorID"))
                MainOrdersDataGrid.Columns["SecondOperatorID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSProductionUserID"))
                MainOrdersDataGrid.Columns["TPSProductionUserID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilProductionUserID"))
                MainOrdersDataGrid.Columns["ProfilProductionUserID"].Visible = false;

            if (MainOrdersDataGrid.Columns.Contains(MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"]))
                MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains(MainOrdersDataGrid.Columns["TPSPackAllocStatusID"]))
                MainOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains(MainOrdersDataGrid.Columns["ProfilPackCount"]))
                MainOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains(MainOrdersDataGrid.Columns["TPSPackCount"]))
                MainOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains(MainOrdersDataGrid.Columns["AllocPackDateTime"]))
                MainOrdersDataGrid.Columns["AllocPackDateTime"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilOnProductionDate"))
                MainOrdersDataGrid.Columns["ProfilOnProductionDate"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSOnProductionUserID"))
                MainOrdersDataGrid.Columns["TPSOnProductionUserID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSOnProductionDate"))
                MainOrdersDataGrid.Columns["TPSOnProductionDate"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilOnProductionUserID"))
                MainOrdersDataGrid.Columns["ProfilOnProductionUserID"].Visible = false;

            MainOrdersDataGrid.Columns["WillPercentID"].Visible = false;
            MainOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDataGrid.Columns["MegaOrderID"].Visible = false;
            MainOrdersDataGrid.Columns["ClientID"].Visible = false;
            MainOrdersDataGrid.Columns["PriceTypeID"].Visible = false;
            MainOrdersDataGrid.Columns["DebtTypeID"].Visible = false;
            MainOrdersDataGrid.Columns["IsPrepared"].Visible = false;
            MainOrdersDataGrid.Columns["DispatchStatusID"].Visible = false;


            MainOrdersDataGrid.Columns["MainOrderID"].HeaderText = "№ п\\п";
            MainOrdersDataGrid.Columns["NeedCalculate"].HeaderText = "Включено в\r\n расчет";
            MainOrdersDataGrid.Columns["DocNumber"].HeaderText = "№ документа";
            MainOrdersDataGrid.Columns["DebtDocNumber"].HeaderText = "№ документа\r\nдолга";
            MainOrdersDataGrid.Columns["ReorderDocNumber"].HeaderText = "№ документа\r\nперезаказа";
            MainOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";
            MainOrdersDataGrid.Columns["DoNotDispatch"].HeaderText = "Отгрузка\r\nбез фасадов";
            MainOrdersDataGrid.Columns["TechDrilling"].HeaderText = "Сверление";
            MainOrdersDataGrid.Columns["QuicklyOrder"].HeaderText = "Срочно";
            MainOrdersDataGrid.Columns["FrontsCost"].HeaderText = "Стоимость\r\nфасадов, евро";
            MainOrdersDataGrid.Columns["DecorCost"].HeaderText = "Стоимость\r\nдекора, евро";
            MainOrdersDataGrid.Columns["OrderCost"].HeaderText = "Стоимость\r\nзаказа, евро";
            MainOrdersDataGrid.Columns["DocDateTime"].HeaderText = "Дата\r\nсоздания";
            MainOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание к заказу";
            MainOrdersDataGrid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            MainOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";

            MainOrdersDataGrid.Columns["ProfilProductionDate"].HeaderText = "Дата входа\r\nв пр-во, Профиль";
            MainOrdersDataGrid.Columns["TPSProductionDate"].HeaderText = "Дата входа\r\nв пр-во, ТПС";
            MainOrdersDataGrid.Columns["MovePrepareDate"].HeaderText = "Дата\r\nпереноса";
            MainOrdersDataGrid.Columns["SaveDateTime"].HeaderText = "Дата\r\nсохранения";

            MainOrdersDataGrid.Columns["DispatchedCost"].HeaderText = "Отгружено,\r\nевро";
            MainOrdersDataGrid.Columns["DispatchedDebtCost"].HeaderText = "Не отгружено,\r\nевро";
            MainOrdersDataGrid.Columns["CalcDebtCost"].HeaderText = "Долги в расчете,\r\nевро";
            MainOrdersDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "  Списание по\r\nобразцам, евро";
            MainOrdersDataGrid.Columns["WriteOffDebtCost"].HeaderText = "Долги по отгрузке,\r\nевро";
            MainOrdersDataGrid.Columns["WriteOffDefectsCost"].HeaderText = "Брак по возврату,\r\nевро";
            MainOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].HeaderText = "Ошибки пр-ва\r\nпо возврату, евро";
            MainOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].HeaderText = "Ошибки ЗОВа\r\nпо возврату, евро";
            MainOrdersDataGrid.Columns["TotalWriteOffCost"].HeaderText = "Списано по возвратам,\r\nевро";
            MainOrdersDataGrid.Columns["IncomeCost"].HeaderText = "Итого по расчету,\r\nевро";
            MainOrdersDataGrid.Columns["ProfitCost"].HeaderText = "Итого,\r\n евро";
            MainOrdersDataGrid.Columns["DoubleOrder"].HeaderText = "Двойное\r\nвбивание";
            MainOrdersDataGrid.Columns["ToAssembly"].HeaderText = "На сборку";
            MainOrdersDataGrid.Columns["FromAssembly"].HeaderText = "Со сборки";
            MainOrdersDataGrid.Columns["IsNotPaid"].HeaderText = "Не оплачено";

            if (NeedFillBatch)
                MainOrdersDataGrid.Columns["BatchNumber"].HeaderText = "Партия";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            //MainOrdersDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["MainOrderID"].MinimumWidth = 50;
            MainOrdersDataGrid.Columns["ClientColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ClientColumn"].MinimumWidth = 160;

            MainOrdersDataGrid.Columns["ProfilProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilProductionDate"].MinimumWidth = 115;
            MainOrdersDataGrid.Columns["TPSProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSProductionDate"].MinimumWidth = 90;
            //MainOrdersDataGrid.Columns["MovePrepareDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["MovePrepareDate"].MinimumWidth = 115;
            //MainOrdersDataGrid.Columns["SaveDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["SaveDateTime"].MinimumWidth = 115;

            //MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].Width = 120;
            //MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].Width = 90;
            //MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].Width = 90;
            //MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].Width = 120;
            //MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].Width = 90;
            //MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].Width = 90;

            //MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Width = 90;

            //MainOrdersDataGrid.Columns["TPSPackAllocStatusID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["TPSPackAllocStatusID"].Width = 90;

            //MainOrdersDataGrid.Columns["ProfilPackCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["ProfilPackCount"].Width = 90;

            //MainOrdersDataGrid.Columns["TPSPackCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["TPSPackCount"].Width = 90;

            MainOrdersDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["MainOrderID"].MinimumWidth = 50;
            //MainOrdersDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ClientName"].MinimumWidth = 150;
            MainOrdersDataGrid.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DocNumber"].MinimumWidth = 190;
            MainOrdersDataGrid.Columns["DebtDocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DebtDocNumber"].MinimumWidth = 130;
            MainOrdersDataGrid.Columns["ReorderDocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ReorderDocNumber"].MinimumWidth = 130;
            MainOrdersDataGrid.Columns["FrontsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["FrontsCost"].MinimumWidth = 130;
            MainOrdersDataGrid.Columns["PriceTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["PriceTypeColumn"].MinimumWidth = 130;
            MainOrdersDataGrid.Columns["DebtTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DebtTypeColumn"].MinimumWidth = 130;
            MainOrdersDataGrid.Columns["DecorCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DecorCost"].MinimumWidth = 130;
            MainOrdersDataGrid.Columns["OrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["OrderCost"].MinimumWidth = 130;
            MainOrdersDataGrid.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["MovePrepareDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["SaveDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DocDateTime"].MinimumWidth = 150;
            MainOrdersDataGrid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["FrontsSquare"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["DoNotDispatch"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["TechDrilling"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["QuicklyOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["DoubleOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["ToAssembly"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["FromAssembly"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["IsNotPaid"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["IsSample"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["Notes"].MinimumWidth = 90;
            MainOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["Weight"].MinimumWidth = 100;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["FactoryTypeColumn"].MinimumWidth = 150;

            MainOrdersDataGrid.Columns["ProfilProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["ProfilProductionDate"].MinimumWidth = 165;
            MainOrdersDataGrid.Columns["TPSProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDataGrid.Columns["TPSProductionDate"].MinimumWidth = 140;

            MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["ProfilExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["TPSExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].MinimumWidth = 110;

            if (NeedFillBatch)
                MainOrdersDataGrid.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["BatchNumber"].MinimumWidth = 75;

            MainOrdersDataGrid.Columns["NeedCalculate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["CalcDebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["DispatchedCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["DispatchedDebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["WriteOffDebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["TotalWriteOffCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["IncomeCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["WriteOffDefectsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["SamplesWriteOffCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            MainOrdersDataGrid.Columns["ProfitCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            MainOrdersDataGrid.AutoGenerateColumns = false;
            int DisplayIndex = 0;

            MainOrdersDataGrid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ClientColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DocNumber"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["FrontsCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DecorCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["OrderCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["FrontsSquare"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["PriceTypeColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["IsSample"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DebtTypeColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DoNotDispatch"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["NeedCalculate"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DispatchedCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DispatchedDebtCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["SamplesWriteOffCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["CalcDebtCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DebtDocNumber"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ReorderDocNumber"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["WriteOffDebtCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["WriteOffDefectsCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TotalWriteOffCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["IncomeCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfitCost"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TechDrilling"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["QuicklyOrder"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ToAssembly"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["FromAssembly"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["IsNotPaid"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfilExpeditionStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TPSExpeditionStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["Weight"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["DocDateTime"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["ProfilProductionDate"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["TPSProductionDate"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["SaveDateTime"].DisplayIndex = DisplayIndex++;
            MainOrdersDataGrid.Columns["MovePrepareDate"].DisplayIndex = DisplayIndex++;

            MainOrdersDataGrid.Columns["IsSample"].SortMode = DataGridViewColumnSortMode.Automatic;
            MainOrdersDataGrid.Columns["DoNotDispatch"].SortMode = DataGridViewColumnSortMode.Automatic;
            MainOrdersDataGrid.Columns["TechDrilling"].SortMode = DataGridViewColumnSortMode.Automatic;
            MainOrdersDataGrid.Columns["QuicklyOrder"].SortMode = DataGridViewColumnSortMode.Automatic;

            if (NeedFillBatch)
                MainOrdersDataGrid.Columns["BatchNumber"].DisplayIndex = 3;

            MainOrdersDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MainOrdersDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["DispatchedCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["DispatchedDebtCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["WriteOffDebtCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["TotalWriteOffCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["IncomeCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["WriteOffDefectsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void MegaGridSetting()
        {
            foreach (DataGridViewColumn Column in MegaOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            }

            if (MegaOrdersDataGrid.Columns.Contains(MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"]))
                MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains(MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"]))
                MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains(MegaOrdersDataGrid.Columns["ProfilPackCount"]))
                MegaOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains(MegaOrdersDataGrid.Columns["TPSPackCount"]))
                MegaOrdersDataGrid.Columns["TPSPackCount"].Visible = false;

            MegaOrdersDataGrid.Columns["DispatchStatusID"].Visible = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDataGrid.Columns["DispatchDate"].HeaderText = "    Дата\r\nотгрузки";

            MegaOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDataGrid.Columns["Square"].HeaderText = "    Площадь\r\nфасадов, м.кв.";
            MegaOrdersDataGrid.Columns["TotalCost"].HeaderText = "Расчет,\r\n  евро";
            MegaOrdersDataGrid.Columns["DispatchedCost"].HeaderText = "Отгружено,\r\n     евро";
            MegaOrdersDataGrid.Columns["DispatchedDebtCost"].HeaderText = "Не отгружено,\r\n       евро";
            MegaOrdersDataGrid.Columns["CalcDebtCost"].HeaderText = "Долги в расчете,\r\n        евро";
            MegaOrdersDataGrid.Columns["CalcDefectsCost"].HeaderText = "Брак в расчете,\r\n       евро";
            MegaOrdersDataGrid.Columns["CalcProductionErrorsCost"].HeaderText = " Ошибки пр-ва\r\nв расчете, евро";
            MegaOrdersDataGrid.Columns["CalcZOVErrorsCost"].HeaderText = " Ошибки ЗОВа\r\nв расчете, евро";
            MegaOrdersDataGrid.Columns["WriteOffDebtCost"].HeaderText = "Долги по возвратам,\r\n           евро";
            MegaOrdersDataGrid.Columns["WriteOffDefectsCost"].HeaderText = "Брак по возвратам,\r\n           евро";
            MegaOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].HeaderText = "  Ошибки пр-ва\r\nпо возвратам, евро";
            MegaOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].HeaderText = "   Ошибки ЗОВа\r\nпо возвратам, евро";
            MegaOrdersDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "Списание за образцы,\r\n            евро";
            MegaOrdersDataGrid.Columns["TotalWriteOffCost"].HeaderText = "Списано по возвратам,\r\n            евро";
            MegaOrdersDataGrid.Columns["TotalCalcWriteOffCost"].HeaderText = "Списано по расчету,\r\n            евро";
            MegaOrdersDataGrid.Columns["IncomeCost"].HeaderText = "Итого по расчету,\r\n          евро";
            MegaOrdersDataGrid.Columns["ProfitCost"].HeaderText = "Итого,\r\n евро";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            NumberFormatInfo nfi3 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 2
            };
            MegaOrdersDataGrid.Columns["DispatchedCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["DispatchedCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["DispatchedDebtCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["DispatchedDebtCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["TotalWriteOffCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["TotalWriteOffCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["IncomeCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["IncomeCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi3;

            MegaOrdersDataGrid.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaOrdersDataGrid.Columns["DispatchDate"].MinimumWidth = 120;

            //MegaOrdersDataGrid.Columns["DispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MegaOrdersDataGrid.Columns["DispatchStatusColumn"].MinimumWidth = 120;

            MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Width = 90;

            MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Width = 90;

            MegaOrdersDataGrid.Columns["ProfilPackCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilPackCount"].Width = 90;

            MegaOrdersDataGrid.Columns["TPSPackCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSPackCount"].Width = 90;

            MegaOrdersDataGrid.AutoGenerateColumns = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].DisplayIndex = 0;
            MegaOrdersDataGrid.Columns["DispatchDate"].DisplayIndex = 1;
            MegaOrdersDataGrid.Columns["Weight"].DisplayIndex = 2;
            MegaOrdersDataGrid.Columns["Square"].DisplayIndex = 3;
            //MegaOrdersDataGrid.Columns["DispatchCost"].DisplayIndex = 4;


            MegaOrdersDataGrid.Columns["DispatchedCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["DispatchedDebtCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CalcDebtCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CalcProductionErrorsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CalcZOVErrorsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["TotalWriteOffCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["IncomeCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["CalcDefectsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["SamplesWriteOffCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["WriteOffDefectsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["WriteOffDebtCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["TotalCalcWriteOffCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["ProfitCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void UpdateSelectedRows()
        {
            int count = MainOrdersDataGrid.SelectedRows.Count;

            for (int i = 0, f = 0; MainOrdersDataGrid.Rows.Count > 0 && f < count; f++)
            {
                int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);

                DataRow[] Row = MainOrdersDataTable.Select("MainOrderID = " + MainOrderID);

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int c = 0; c < DT.Columns.Count; c++)
                            Row[0][c] = DT.Rows[0][c];
                    }
                }
            }
        }

        public void UpdateSearchMainOrders()
        {
            SearchMainOrdersDataTable.Clear();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, MainOrderID, DocNumber FROM MainOrders", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SearchMainOrdersDataTable);
            }
        }


        public DateTime GetCurrentDate()
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

        public void FilterMegaOrders(bool Profil, bool TPS, bool FilterByDispDate, DateTime DateFrom, DateTime DateTo,
            bool bDebts, bool bDoNotDisp, bool bTechDrilling, bool bQuicklyOrder, bool bDoubleOrder, bool bToAssembly, bool bFromAssembly, bool bIsNotPaid)
        {
            int FactoryID = 0;
            int MegaOrderID = CurrentMegaOrderID;
            string FactoryFilter = string.Empty;
            string DebtsFilter = string.Empty;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;

            if (FactoryID > 0)
                FactoryFilter = " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            if (FactoryID == -1)
                FactoryFilter = " WHERE (FactoryID = " + FactoryID + ")";

            if (bDebts || bDoNotDisp || bTechDrilling || bQuicklyOrder || bDoubleOrder || bToAssembly || bFromAssembly || bIsNotPaid)
            {
                if (bToAssembly)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (ToAssembly = 1)";
                    else
                        DebtsFilter = " (ToAssembly = 1)";
                }
                if (bFromAssembly)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (FromAssembly = 1)";
                    else
                        DebtsFilter = " (FromAssembly = 1)";
                }
                if (bIsNotPaid)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (IsNotPaid = 1)";
                    else
                        DebtsFilter = " (IsNotPaid = 1)";
                }
                if (bDebts)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (DebtTypeID != 0)";
                    else
                        DebtsFilter = " (DebtTypeID != 0)";
                }
                if (bDoNotDisp)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (DoNotDispatch = 1 AND (NOT (TPSProductionStatusID = 1 AND TPSStorageStatusID = 1 AND TPSDispatchStatusID = 2)))";
                    else
                        DebtsFilter = " (DoNotDispatch = 1 AND (NOT (TPSProductionStatusID = 1 AND TPSStorageStatusID = 1 AND TPSDispatchStatusID = 2)))";
                }
                if (bTechDrilling)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (TechDrilling = 1)";
                    else
                        DebtsFilter = " (TechDrilling = 1)";
                }
                if (bQuicklyOrder)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (QuicklyOrder = 1)";
                    else
                        DebtsFilter = " (QuicklyOrder = 1)";
                }
                if (bDoubleOrder)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (DoubleOrder = 0)";
                    else
                        DebtsFilter = " (DoubleOrder = 0)";
                }

                if (FactoryFilter.Length > 0)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter = " AND (" + DebtsFilter + ")";
                }
                else
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter = " WHERE (" + DebtsFilter + ")";
                }
            }

            MegaOrdersDataAdapter.Dispose();
            MegaOrdersDataTable.Clear();
            MegaOrdersCommandBuilder.Dispose();

            string SelectionCommand = "SELECT * FROM MegaOrders" +
                " WHERE (MegaOrderID=0 OR MegaOrderID > 11025) AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" + FactoryFilter + DebtsFilter + ")" +
                " ORDER BY DispatchDate";
            if (FilterByDispDate)
            {
                SelectionCommand = "SELECT * FROM MegaOrders" +
                   " WHERE MegaOrderID = 0 OR (MegaOrderID > 11025 AND CAST(DispatchDate AS Date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
                   "' AND CAST(DispatchDate AS Date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" +
                   " AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" + FactoryFilter + DebtsFilter + ")" +
                   " ORDER BY DispatchDate";
            }

            MegaOrdersDataAdapter = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
        }

        public int GetNewOrdersCount()
        {
            int Count = 0;

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
                if (MegaOrdersDataTable.Rows[i]["AgreementStatusID"].ToString() == "0")
                    Count++;

            return Count;
        }


        public bool UpdateSelectedMegaOrder()
        {
            int MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            DataRow[] Row = MegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return false;

                    for (int c = 0; c < DT.Columns.Count; c++)
                        Row[0][c] = DT.Rows[0][c];
                }
            }

            MegaOrdersDataTable.AcceptChanges();

            return true;
        }

        public bool UpdateSelectedMainOrders()
        {
            int count = MainOrdersDataGrid.SelectedRows.Count;

            int[] MainOrders = new int[count];

            for (int i = 0; i < count; i++)
            {
                MainOrders[i] = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);
            }

            for (int i = 0; i < count; i++)
            {
                DataRow[] Row = MainOrdersDataTable.Select("MainOrderID = " + MainOrders[i]);

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MainOrderID = " + MainOrders[i], ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return false;

                        for (int c = 0; c < DT.Columns.Count; c++)
                            Row[0][c] = DT.Rows[0][c];

                        Row[0].AcceptChanges();
                    }
                }
            }

            return true;
        }

        public bool UpdateMegaOrders(bool Profil, bool TPS, bool FilterByDispDate, DateTime DateFrom, DateTime DateTo)
        {
            int pos = MegaOrdersBindingSource.Position;
            string FactoryFilter = string.Empty;

            MegaOrdersDataAdapter.Dispose();
            MegaOrdersCommandBuilder.Dispose();
            MegaOrdersDataTable.Clear();

            string SelectionCommand = "SELECT * FROM MegaOrders" +
                " WHERE MegaOrderID > 11025 AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" + FactoryFilter + ")" +
                " ORDER BY DispatchDate";
            if (FilterByDispDate)
            {
                SelectionCommand = "SELECT * FROM MegaOrders" +
                   " WHERE MegaOrderID = 0 OR (MegaOrderID > 11025 AND CAST(DispatchDate AS Date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
                   "' AND CAST(DispatchDate AS Date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" +
                   " AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" + FactoryFilter + ")" +
                   " ORDER BY DispatchDate";
            }

            MegaOrdersDataAdapter = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            int MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
            int ClientID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["ClientID"]);

            FilterMainOrdersCurrent();

            //остается на позиции заказа, а не перемещается в начало
            if (MegaOrdersBindingSource.Count > 0)
                if (pos >= MegaOrdersBindingSource.Count)
                {
                    MegaOrdersBindingSource.MoveFirst();
                }
                else
                    MegaOrdersBindingSource.Position = pos;

            return true;
        }

        public bool UpdateMainOrders(bool Profil, bool TPS, bool bDebts, bool bDoNotDisp, bool bTechDrilling, bool bQuicklyOrder, bool bDoubleOrder, bool bToAssembly, bool bFromAssembly, bool bIsNotPaid)
        {
            CurrentMegaOrderID = -1;

            int MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            FilterMainOrders(MegaOrderID, Profil, TPS, bDebts, bDoNotDisp, bTechDrilling, bQuicklyOrder, bDoubleOrder, bToAssembly, bFromAssembly, bIsNotPaid);

            return true;
        }


        public String GetDocNumberFromExcel()
        {
            String InputString = null;
            InputString = Clipboard.GetText();

            if (InputString.Length < 1)
                return "";

            if (InputString[0] != '№')
                return "";

            String Result = null;

            for (int i = 0; i < InputString.Length; i++)
                if (InputString[i] != '.')
                    Result += InputString[i];
                else
                    break;

            return Result;
        }

        public string ExportClient(String ClientName)
        {
            ClientName = ClientName.Trim();

            if (ClientName.Length < 1 || ClientName[0] == ' ')
                return null;

            string Client = null;

            for (int i = 0; i < ClientName.Length; i++)
            {
                if (ClientName[i] != '(' || ClientName[i] != '/' || ClientName[i] != '\\')
                    Client += ClientName[i];
                else
                    break;
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientName = '" + Client + "'");

            //если клиент есть в базе, переключаем в контролах его
            if (Rows.Count() > 0)
            {
                ChangeClient(Convert.ToInt32(Rows[0]["ClientID"]));
                return null;
            }

            //если клиента нет в базе
            return Client;

        }

        public int IsDocNumberExist(string DocNumber, ref int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocNumber, IsPrepared, MainOrderID, MegaOrderID FROM MainOrders WHERE DocNumber = '" + DocNumber + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count == 0)
                        return -1;//не существует

                    if (Convert.ToBoolean(DT.Rows[0]["IsPrepared"]) == false)
                    {
                        MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                        return 0;
                    }//существующий (дубль)
                    else
                    {
                        MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                        return 1;
                    }//пред-во
                }
            }
        }

        public void FilterClients(int ClientGroupID)
        {
            ClientsBindingSource.Filter = "ClientGroupID = " + ClientGroupID;
        }

        public void CreateNewMainOrder(object DispatchDate, DateTime CurrentDateTime, int ClientID, string DocNumber, int DebtTypeID, bool IsSample, bool IsPrepared,
            int PriceTypeID, string Notes, int FirstOperatorID, int SecondOperatorID,
            bool DoNotDispatch, bool TechDrilling, bool QuicklyOrder, bool ToAssembly, bool IsNotPaid, bool NeedCalculate)
        {
            int CurrentMegaOrder = MoveToMegaOrder(DispatchDate);

            DataRow Row = MainOrdersDataTable.NewRow();

            Row["MegaOrderID"] = CurrentMegaOrder;
            Row["DocDateTime"] = CurrentDateTime;
            Row["ClientID"] = ClientID;
            Row["DocNumber"] = DocNumber;
            Row["DebtTypeID"] = DebtTypeID;
            Row["IsSample"] = IsSample;
            Row["IsPrepared"] = IsPrepared;
            Row["PriceTypeID"] = PriceTypeID;
            Row["Notes"] = Notes;
            Row["FirstOperatorID"] = FirstOperatorID;
            Row["SecondOperatorID"] = SecondOperatorID;
            Row["DoNotDispatch"] = DoNotDispatch;
            Row["TechDrilling"] = TechDrilling;
            Row["QuicklyOrder"] = QuicklyOrder;
            Row["ToAssembly"] = ToAssembly;
            Row["IsNotPaid"] = IsNotPaid;
            Row["NeedCalculate"] = NeedCalculate;

            MainOrdersDataTable.Rows.Add(Row);

            DataTable DT = MainOrdersDataTable.Copy();
            if (DT.Columns.Contains("BatchNumber"))
                DT.Columns.Remove("BatchNumber");
            MainOrdersDataAdapter.Update(DT);
            DT.Dispose();

            MainOrdersDataTable.Clear();
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);

            MainOrdersBindingSource.MoveLast();


            CurrentMainOrderID = Convert.ToInt32(MainOrdersDataTable.Select("DocNumber = '" + DocNumber + "'")[0]["MainOrderID"]);

            CurrentMegaOrderID = CurrentMegaOrder;

            InNew = true;
        }

        public void CreateNewDispatch(object DispatchDate)
        {
            if (DispatchDate == null)
                return;

            if (MegaOrdersDataTable.Select("DispatchDate = '" + Convert.ToDateTime(DispatchDate).ToShortDateString() + "'").Count() > 0)
                return;

            DataRow NewRow = MegaOrdersDataTable.NewRow();
            NewRow["DispatchDate"] = Convert.ToDateTime(DispatchDate).ToString("yyyy-MM-dd");
            MegaOrdersDataTable.Rows.Add(NewRow);

            string SelectionCommand = "SELECT * FROM MegaOrders" +
                " ORDER BY DispatchDate";

            MegaOrdersDataAdapter.Update(MegaOrdersDataTable);

            MegaOrdersDataAdapter.Dispose();
            MegaOrdersDataTable.Clear();
            MegaOrdersCommandBuilder.Dispose();

            MegaOrdersDataAdapter = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", CurrentMegaOrderID);
        }

        public void SummaryPackCountMegaOrder(int MegaOrderID)
        {
            int ProfilPackCount = 0;
            int TpsPackCount = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(ProfilPackCount)AS PPC, SUM(TPSPackCount) AS TPC FROM MainOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    ProfilPackCount = Convert.ToInt32(DT.Rows[0]["PPC"]);
                    TpsPackCount = Convert.ToInt32(DT.Rows[0]["TPC"]);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, ProfilPackCount, TPSPackCount FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["ProfilPackCount"] = ProfilPackCount;
                        DT.Rows[0]["TPSPackCount"] = TpsPackCount;

                        DA.Update(DT);
                    }
                }
            }

            //int CurrentMega = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"]);

            //MegaOrdersDataTable.Clear();
            //MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);
            //MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", CurrentMega);

        }

        private void SetMegaOrderPackStatusAndCount(int MegaOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount" +
                " FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DataTable MainDT = new DataTable();
                        using (SqlDataAdapter MainDA = new SqlDataAdapter("SELECT MegaOrderID, FactoryID, " +
                            "ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount" +
                            " FROM MainOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            MainDA.Fill(MainDT);
                        }

                        int ProfilStatusID = 1;
                        int ProfilCount = 0;
                        int ProfilNotPacked = 0;
                        int ProfilAllPacked = 0;

                        int TPSStatusID = 1;
                        int TPSCount = 0;
                        int TPSNotPacked = 0;
                        int TPSAllPacked = 0;

                        DataRow[] PRows = MainDT.Select("FactoryID = 0 OR FactoryID = 1");

                        foreach (DataRow PRow in PRows)
                        {
                            if (Convert.ToInt32(PRow["ProfilPackAllocStatusID"]) == 0)
                                ProfilNotPacked++;

                            if (Convert.ToInt32(PRow["ProfilPackAllocStatusID"]) == 2)
                                ProfilAllPacked++;

                            ProfilCount += Convert.ToInt32(PRow["ProfilPackCount"]);
                        }

                        DataRow[] TRows = MainDT.Select("FactoryID = 0 OR FactoryID = 2");

                        foreach (DataRow TRow in TRows)
                        {
                            if (Convert.ToInt32(TRow["TPSPackAllocStatusID"]) == 0)
                                TPSNotPacked++;

                            if (Convert.ToInt32(TRow["TPSPackAllocStatusID"]) == 2)
                                TPSAllPacked++;

                            TPSCount += Convert.ToInt32(TRow["TPSPackCount"]);
                        }

                        if (ProfilNotPacked == PRows.Count())
                            ProfilStatusID = 0;

                        if (ProfilAllPacked == PRows.Count())
                            ProfilStatusID = 2;

                        if (ProfilCount == 0)
                            ProfilStatusID = 0;

                        if (TPSNotPacked == TRows.Count())
                            TPSStatusID = 0;

                        if (TPSAllPacked == TRows.Count())
                            TPSStatusID = 2;

                        if (TPSCount == 0)
                            TPSStatusID = 0;

                        DT.Rows[0]["ProfilPackAllocStatusID"] = ProfilStatusID;
                        DT.Rows[0]["ProfilPackCount"] = ProfilCount;

                        DT.Rows[0]["TPSPackAllocStatusID"] = TPSStatusID;
                        DT.Rows[0]["TPSPackCount"] = TPSCount;

                        DA.Update(DT);
                        MainDT.Dispose();
                    }
                }
            }
        }

        public void TotalCalcMegaOrder(DateTime DispatchDate)
        {
            int MegaOrderID = Convert.ToInt32(MegaOrdersDataTable.Select("DispatchDate = '" + DispatchDate.ToString("yyyy-MM-dd") + "'")[0]["MegaOrderID"]);

            SummaryCalcMegaOrder(MegaOrderID);
            SummaryResultMegaOrder(MegaOrderID);
            SummaryPackCountMegaOrder(MegaOrderID);
            SetMegaOrderPackStatusAndCount(MegaOrderID);
        }

        public string GetCurrentMegaOrderDispatchDate(int MegaOrderID)
        {
            if (MegaOrderID == 0)
                return "";

            return Convert.ToDateTime(MegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID)[0]["DispatchDate"]).ToString("dd.MM.yyyy");
        }

        public void SummaryCalcCurrentMegaOrder()
        {
            for (int i = 0; i < 3; i++)
            {

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, CalcDebtCost, CalcDefectsCost, CalcProductionErrorsCost, CalcZOVErrorsCost, " +
                                                                 "TotalCalcWriteOffCost, IncomeCost, TotalCost, Square, Weight FROM MegaOrders " +
                                                                 "WHERE MegaOrderID = " + ((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"], ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            using (SqlDataAdapter moDA = new SqlDataAdapter("SELECT MainOrderID, OrderCost, NeedCalculate, DebtTypeID, Weight, FrontsSquare, CalcDebtCost FROM MainOrders WHERE MegaOrderID = " + ((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"], ConnectionStrings.ZOVOrdersConnectionString))
                            {
                                using (DataTable moDT = new DataTable())
                                {
                                    moDA.Fill(moDT);

                                    decimal TotalCost = 0;
                                    decimal CalcDebtCost = 0;
                                    decimal CalcDefectsCost = 0;
                                    decimal CalcProductionErrorsCost = 0;
                                    decimal CalcZOVErrorsCost = 0;
                                    decimal TotalCalcWriteOffCost = 0;
                                    decimal Weight = 0;
                                    decimal Square = 0;

                                    if (moDT.Rows.Count == 0)
                                        return;

                                    foreach (DataRow MORow in moDT.Rows)
                                    {
                                        TotalCost += Convert.ToDecimal(MORow["OrderCost"]);

                                        if (Convert.ToBoolean(MORow["NeedCalculate"]) == false)
                                        {
                                            if (MORow["DebtTypeID"].ToString() == "1")
                                                CalcDebtCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                            if (MORow["DebtTypeID"].ToString() == "2")
                                                CalcDefectsCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                            if (MORow["DebtTypeID"].ToString() == "3")
                                                CalcProductionErrorsCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                            if (MORow["DebtTypeID"].ToString() == "4")
                                                CalcZOVErrorsCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                        }

                                        Weight += Convert.ToDecimal(MORow["Weight"]);
                                        Square += Convert.ToDecimal(MORow["FrontsSquare"]);

                                    }

                                    TotalCalcWriteOffCost = CalcDebtCost + CalcDefectsCost + CalcProductionErrorsCost + CalcZOVErrorsCost;
                                    DT.Rows[0]["CalcDebtCost"] = CalcDebtCost;
                                    DT.Rows[0]["CalcDefectsCost"] = CalcDefectsCost;
                                    DT.Rows[0]["CalcProductionErrorsCost"] = CalcProductionErrorsCost;
                                    DT.Rows[0]["CalcZOVErrorsCost"] = CalcZOVErrorsCost;
                                    DT.Rows[0]["TotalCalcWriteOffCost"] = TotalCalcWriteOffCost;
                                    DT.Rows[0]["IncomeCost"] = TotalCost - TotalCalcWriteOffCost;
                                    DT.Rows[0]["TotalCost"] = TotalCost;
                                    DT.Rows[0]["Square"] = Square;
                                    DT.Rows[0]["Weight"] = Weight;

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
                }
            }

            //int CurrentMega = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"]);

            //MegaOrdersDataTable.Clear();
            //MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);
            //MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", CurrentMega);
        }

        public void SummaryCalcMegaOrder(int MegaOrderID)
        {
            for (int i = 0; i < 3; i++)
            {

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, CalcDebtCost, CalcDefectsCost, CalcProductionErrorsCost, CalcZOVErrorsCost, " +
                                                                 "TotalCalcWriteOffCost, IncomeCost, TotalCost, Square, Weight FROM MegaOrders " +
                                                                 "WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            using (SqlDataAdapter moDA = new SqlDataAdapter("SELECT MainOrderID, OrderCost, NeedCalculate, DebtTypeID, Weight, FrontsSquare, CalcDebtCost FROM MainOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                            {
                                using (DataTable moDT = new DataTable())
                                {
                                    moDA.Fill(moDT);

                                    decimal TotalCost = 0;
                                    decimal CalcDebtCost = 0;
                                    decimal CalcDefectsCost = 0;
                                    decimal CalcProductionErrorsCost = 0;
                                    decimal CalcZOVErrorsCost = 0;
                                    decimal TotalCalcWriteOffCost = 0;
                                    decimal Weight = 0;
                                    decimal Square = 0;

                                    if (moDT.Rows.Count == 0)
                                        return;

                                    foreach (DataRow MORow in moDT.Rows)
                                    {
                                        TotalCost += Convert.ToDecimal(MORow["OrderCost"]);

                                        if (Convert.ToBoolean(MORow["NeedCalculate"]) == false)
                                        {
                                            if (MORow["DebtTypeID"].ToString() == "1")
                                                CalcDebtCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                            if (MORow["DebtTypeID"].ToString() == "2")
                                                CalcDefectsCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                            if (MORow["DebtTypeID"].ToString() == "3")
                                                CalcProductionErrorsCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                            if (MORow["DebtTypeID"].ToString() == "4")
                                                CalcZOVErrorsCost += Convert.ToDecimal(MORow["CalcDebtCost"]);
                                        }

                                        Weight += Convert.ToDecimal(MORow["Weight"]);
                                        Square += Convert.ToDecimal(MORow["FrontsSquare"]);

                                    }

                                    TotalCalcWriteOffCost = CalcDebtCost + CalcDefectsCost + CalcProductionErrorsCost + CalcZOVErrorsCost;
                                    DT.Rows[0]["CalcDebtCost"] = CalcDebtCost;
                                    DT.Rows[0]["CalcDefectsCost"] = CalcDefectsCost;
                                    DT.Rows[0]["CalcProductionErrorsCost"] = CalcProductionErrorsCost;
                                    DT.Rows[0]["CalcZOVErrorsCost"] = CalcZOVErrorsCost;
                                    DT.Rows[0]["TotalCalcWriteOffCost"] = TotalCalcWriteOffCost;
                                    DT.Rows[0]["IncomeCost"] = TotalCost - TotalCalcWriteOffCost;
                                    DT.Rows[0]["TotalCost"] = TotalCost;
                                    DT.Rows[0]["Square"] = Square;
                                    DT.Rows[0]["Weight"] = Weight;

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
                }
            }

            //MegaOrdersDataTable.Clear();
            //MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);
            //MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", ((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"]);
        }

        public void SummaryResultMegaOrder(int MegaOrderID)
        {
            decimal ProfitCost = 0;
            decimal SamplesWriteOffCost = 0;
            decimal WriteOffDebtCost = 0;
            decimal WriteOffDefectsCost = 0;
            decimal WriteOffProductionErrorsCost = 0;
            decimal WriteOffZOVErrorsCost = 0;
            decimal TotalWriteOffCost = 0;

            for (int i = 0; i < 3; i++)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, SamplesWriteOffCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost, TotalWriteOffCost, ProfitCost FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            using (SqlDataAdapter moDA = new SqlDataAdapter("SELECT MainOrderID, SamplesWriteOffCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost, TotalWriteOffCost, ProfitCost FROM MainOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                            {
                                using (DataTable moDT = new DataTable())
                                {
                                    if (moDA.Fill(moDT) == 0)
                                        return;

                                    foreach (DataRow Row in moDT.Rows)
                                    {
                                        SamplesWriteOffCost += Convert.ToDecimal(Row["SamplesWriteOffCost"]);
                                        WriteOffDebtCost += Convert.ToDecimal(Row["WriteOffDebtCost"]);
                                        WriteOffDefectsCost += Convert.ToDecimal(Row["WriteOffDefectsCost"]);
                                        WriteOffProductionErrorsCost += Convert.ToDecimal(Row["WriteOffProductionErrorsCost"]);
                                        WriteOffZOVErrorsCost += Convert.ToDecimal(Row["WriteOffZOVErrorsCost"]);
                                        TotalWriteOffCost += Convert.ToDecimal(Row["TotalWriteOffCost"]);
                                        ProfitCost += Convert.ToDecimal(Row["ProfitCost"]);
                                    }
                                }
                            }

                            DT.Rows[0]["SamplesWriteOffCost"] = SamplesWriteOffCost;
                            DT.Rows[0]["WriteOffDebtCost"] = WriteOffDebtCost;
                            DT.Rows[0]["WriteOffDefectsCost"] = WriteOffDefectsCost;
                            DT.Rows[0]["WriteOffProductionErrorsCost"] = WriteOffProductionErrorsCost;
                            DT.Rows[0]["WriteOffZOVErrorsCost"] = WriteOffZOVErrorsCost;
                            DT.Rows[0]["TotalWriteOffCost"] = TotalWriteOffCost;
                            DT.Rows[0]["ProfitCost"] = ProfitCost;

                            try
                            {
                                DA.Update(DT);
                                break;
                            }
                            catch { }
                        }
                    }
                }
            }

            //MegaOrdersDataTable.Clear();
            //MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);
            //MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
        }

        public void SummaryResultMainOrder(int MainOrderID)
        {
            decimal ProfitCost = 0;
            decimal SamplesWriteOffCost = 0;
            decimal WriteOffDebtCost = 0;
            decimal WriteOffDefectsCost = 0;
            decimal WriteOffProductionErrorsCost = 0;
            decimal WriteOffZOVErrorsCost = 0;
            decimal TotalWriteOffCost = 0;
            decimal IncomeCost = 0;

            for (int i = 0; i < 3; i++)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, SamplesWriteOffCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost, IncomeCost, TotalWriteOffCost, ProfitCost FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            SamplesWriteOffCost = Convert.ToDecimal(DT.Rows[0]["SamplesWriteOffCost"]);
                            WriteOffDebtCost = Convert.ToDecimal(DT.Rows[0]["WriteOffDebtCost"]);
                            WriteOffDefectsCost = Convert.ToDecimal(DT.Rows[0]["WriteOffDefectsCost"]);
                            WriteOffProductionErrorsCost = Convert.ToDecimal(DT.Rows[0]["WriteOffProductionErrorsCost"]);
                            WriteOffZOVErrorsCost = Convert.ToDecimal(DT.Rows[0]["WriteOffZOVErrorsCost"]);
                            TotalWriteOffCost = SamplesWriteOffCost + WriteOffDebtCost + WriteOffDefectsCost + WriteOffProductionErrorsCost + WriteOffZOVErrorsCost;
                            IncomeCost = Convert.ToDecimal(DT.Rows[0]["IncomeCost"]);
                            ProfitCost = IncomeCost - TotalWriteOffCost;

                            DT.Rows[0]["TotalWriteOffCost"] = TotalWriteOffCost;
                            DT.Rows[0]["ProfitCost"] = ProfitCost;


                            try
                            {
                                DA.Update(DT);
                                break;
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }

        public void AddAssemblyOrder(int ClientID, int MainOrderID, string DocNumber, object DispatchDate, string Notes)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM AssemblyOrders WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ClientID"] = ClientID;
                            DT.Rows[0]["DocNumber"] = DocNumber;
                            if (DispatchDate != null)
                                DT.Rows[0]["DispatchDate"] = Convert.ToDateTime(DispatchDate);
                            DT.Rows[0]["Notes"] = Notes;
                            return;
                        }

                        DataRow NewRow = DT.NewRow();
                        NewRow["ClientID"] = ClientID;
                        NewRow["MainOrderID"] = MainOrderID;
                        NewRow["DocNumber"] = DocNumber;
                        if (DispatchDate != null)
                            NewRow["DispatchDate"] = Convert.ToDateTime(DispatchDate);
                        if (Notes.Length > 0)
                            NewRow["Notes"] = Notes;
                        NewRow["Created"] = Security.GetCurrentDate();
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        public void RemoveAssemblyOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT AssemblyOrderID, Active FROM AssemblyOrders WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["Active"] = false;
                            //DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SaveOrder(object DispatchDate, int MainOrderID, int ClientID, string DocNumber, string DebtDocNumber, int DebtTypeID, bool IsSample, bool IsPrepared,
            int PriceTypeID, string Notes, int FactoryID, bool NeedCalculate, bool bDoNotDispatch, bool bTechDrilling, bool bQuicklyOrder,
             bool ToAssembly, bool IsNotPaid)
        {
            int MegaOrderID = 0;

            bool MoveOnDispatch = false;

            if (!IsPrepared)
            {
                MoveOnDispatch = IsOrderPrepared(MainOrderID);
            }

            if (DispatchDate != null)
            {
                CreateNewDispatch(DispatchDate);
                MegaOrderID = Convert.ToInt32(
                    MegaOrdersDataTable.Select("DispatchDate = '" + Convert.ToDateTime(DispatchDate).ToShortDateString() + "'")[0]["MegaOrderID"]);
            }

            for (int i = 0; i < 3; i++)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ClientID, DocNumber, SaveDateTime, DebtTypeID, IsSample," +
                    " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                    " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID," +
                    " ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount," +
                    " IsPrepared, PriceTypeID, Notes, FactoryID, MegaOrderID, DebtDocNumber, NeedCalculate, DoNotDispatch, TechDrilling, QuicklyOrder, ToAssembly, IsNotPaid" +
                    " FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            DT.Rows[0]["SaveDateTime"] = Security.GetCurrentDate();
                            DT.Rows[0]["ClientID"] = ClientID;
                            DT.Rows[0]["DocNumber"] = DocNumber;
                            DT.Rows[0]["DebtTypeID"] = DebtTypeID;
                            DT.Rows[0]["IsSample"] = IsSample;
                            DT.Rows[0]["IsPrepared"] = IsPrepared;
                            DT.Rows[0]["PriceTypeID"] = PriceTypeID;
                            DT.Rows[0]["Notes"] = Notes;
                            DT.Rows[0]["FactoryID"] = FactoryID;
                            DT.Rows[0]["MegaOrderID"] = MegaOrderID;
                            if (DebtTypeID != 0)
                                DT.Rows[0]["DebtDocNumber"] = DebtDocNumber;
                            else
                                DT.Rows[0]["DebtDocNumber"] = "";

                            DT.Rows[0]["NeedCalculate"] = NeedCalculate;
                            DT.Rows[0]["DoNotDispatch"] = bDoNotDispatch;
                            DT.Rows[0]["TechDrilling"] = bTechDrilling;
                            DT.Rows[0]["QuicklyOrder"] = bQuicklyOrder;
                            if (ToAssembly)
                                AddAssemblyOrder(ClientID, MainOrderID, DocNumber, DispatchDate, Notes);
                            if (!ToAssembly && Convert.ToBoolean(DT.Rows[0]["ToAssembly"]))
                                RemoveAssemblyOrder(MainOrderID);
                            DT.Rows[0]["ToAssembly"] = ToAssembly;
                            DT.Rows[0]["IsNotPaid"] = IsNotPaid;

                            //статусы выставляются только если заказа не переносится на дату отгрузки
                            if (!MoveOnDispatch)
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
                            }

                            try
                            {

                                DA.Update(DT);

                                break;
                            }
                            catch
                            {

                            }

                        }
                    }
                }
            }

            //MainOrdersDataTable.Clear();
            //MainOrdersDataAdapter.Fill(MainOrdersDataTable);
            //MainOrdersBindingSource.Position = MainOrdersBindingSource.Find("DocNumber", DocNumber);
        }

        private void FillBatchNumber()
        {
            int MainOrderID = -1;
            string BatchNumber = string.Empty;
            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
            {
                MainOrderID = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);
                BatchNumber = GetBatchNumber(MainOrderID);
                if (BatchNumber.Length < 1)
                {

                    if (Convert.ToInt32(MainOrdersDataTable.Rows[i]["DebtTypeID"]) != 0)
                    {
                        MainOrderID = -1;
                        string DebtDocNumber = MainOrdersDataTable.Rows[i]["DebtDocNumber"].ToString();
                        string DocNumber = MainOrdersDataTable.Rows[i]["DocNumber"].ToString();

                        using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                            " WHERE DocNumber = '" + DebtDocNumber + "'",
                            ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            using (DataTable DT = new DataTable())
                            {
                                DA.Fill(DT);

                                if (DT.Rows.Count > 0)
                                {
                                    MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                                    MainOrdersDataTable.Rows[i]["BatchNumber"] = GetBatchNumber(MainOrderID);
                                    continue;
                                }
                            }
                        }

                    }
                }
                MainOrdersDataTable.Rows[i]["BatchNumber"] = BatchNumber;
            }
        }

        private string GetBatchNumber(int MainOrderID)
        {
            string MegaBatch = string.Empty;
            string Batch = string.Empty;
            string BatchNumber = string.Empty;

            DataRow[] Rows = BatchDetailsDataTable.Select("MainOrderID = " + MainOrderID);

            if (Rows.Count() > 0)
            {
                MegaBatch = Rows[0]["MegaBatchID"].ToString();
                Batch = Rows[0]["BatchID"].ToString();
                BatchNumber = MegaBatch + ", " + Batch + ", " + MainOrderID.ToString();
            }

            return BatchNumber;
        }

        public string GetClientName(int ClientID)
        {
            string ClientName = string.Empty;

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
            {
                ClientName = Rows[0]["ClientName"].ToString();
            }
            return ClientName;
        }

        public string GetClientGroupName(int ClientID)
        {
            string ClientGroupName = string.Empty;

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
            {
                int ClientGroupID = Convert.ToInt32(Rows[0]["ClientGroupID"]);

                DataRow[] CRows = ClientsGroupsDataTable.Select("ClientGroupID = " + ClientGroupID);
                if (CRows.Count() > 0)
                {
                    ClientGroupName = CRows[0]["ClientGroupName"].ToString();
                }
            }
            return ClientGroupName;
        }

        public void FilterMainOrders(int MegaOrderID, bool Profil, bool TPS, bool bDebts, bool bDoNotDisp, bool bTechDrilling, bool bQuicklyOrder, bool bDoubleOrder, bool bToAssembly, bool bFromAssembly, bool bIsNotPaid)
        {
            if (CurrentMegaOrderID == MegaOrderID && CurrentMegaOrderID != 0)
                return;

            if (MegaOrdersBindingSource.Count == 0)
                return;

            int FactoryID = 0;
            string FactoryFilter = string.Empty;
            string DebtsFilter = string.Empty;

            if (Profil && !TPS)
                FactoryID = 1;
            if (!Profil && TPS)
                FactoryID = 2;
            if (!Profil && !TPS)
                FactoryID = -1;

            if (FactoryID > 0)
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            if (FactoryID == -1)
                FactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            if (bToAssembly || bFromAssembly || bIsNotPaid || bDebts || bDoNotDisp || bTechDrilling || bQuicklyOrder || bDoubleOrder)
            {
                if (bToAssembly)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (ToAssembly = 1)";
                    else
                        DebtsFilter = " (ToAssembly = 1)";
                }
                if (bFromAssembly)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (FromAssembly = 1)";
                    else
                        DebtsFilter = " (FromAssembly = 1)";
                }
                if (bIsNotPaid)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (IsNotPaid = 1)";
                    else
                        DebtsFilter = " (IsNotPaid = 1)";
                }
                if (bDebts)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (DebtTypeID != 0)";
                    else
                        DebtsFilter = " (DebtTypeID != 0)";
                }
                if (bDoNotDisp)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (DoNotDispatch = 1 AND (NOT (TPSProductionStatusID = 1 AND TPSStorageStatusID = 1 AND TPSDispatchStatusID = 2)))";
                    else
                        DebtsFilter = " (DoNotDispatch = 1 AND (NOT (TPSProductionStatusID = 1 AND TPSStorageStatusID = 1 AND TPSDispatchStatusID = 2)))";
                }
                if (bTechDrilling)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (TechDrilling = 1)";
                    else
                        DebtsFilter = " (TechDrilling = 1)";
                }
                if (bQuicklyOrder)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (QuicklyOrder = 1)";
                    else
                        DebtsFilter = " (QuicklyOrder = 1)";
                }
                if (bDoubleOrder)
                {
                    if (DebtsFilter.Length > 0)
                        DebtsFilter += " OR (DoubleOrder = 0)";
                    else
                        DebtsFilter = " (DoubleOrder = 0)";
                }

                if (DebtsFilter.Length > 0)
                    DebtsFilter = " AND (" + DebtsFilter + ")";
            }

            MainOrdersDataAdapter.Dispose();
            MainOrdersCommandBuilder.Dispose();
            MainOrdersDataTable.Clear();
            //string f = "SELECT * FROM MainOrders WHERE MainOrderID IN (69219, 69224,69233,69236,69237,69248)";
            //MainOrdersDataAdapter = new SqlDataAdapter(f, ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + DebtsFilter,
                ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersCommandBuilder = new SqlCommandBuilder(MainOrdersDataAdapter);
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);

            if (NeedFillBatch)
                FillBatchNumber();
            CurrentMegaOrderID = MegaOrderID;
        }

        public void FilterMainOrdersCurrent()
        {
            if (MegaOrdersBindingSource.Count == 0)
                return;

            int MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            MainOrdersDataAdapter.Dispose();
            MainOrdersCommandBuilder.Dispose();
            MainOrdersDataTable.Clear();

            MainOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersCommandBuilder = new SqlCommandBuilder(MainOrdersDataAdapter);
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);

            CurrentMegaOrderID = MegaOrderID;
        }

        public void Filter(int MainOrderID)
        {
            if (CurrentMainOrderID == MainOrderID)
            {
                return;
            }

            OrdersTabControl.TabPages[0].PageVisible = MainOrdersFrontsOrders.Filter(MainOrderID);
            OrdersTabControl.TabPages[1].PageVisible = MainOrdersDecorOrders.Filter(MainOrderID);

            if (OrdersTabControl.TabPages[0].PageVisible == false && OrdersTabControl.TabPages[1].PageVisible == false)
                OrdersTabControl.Visible = false;
            else
                OrdersTabControl.Visible = true;
            CurrentMainOrderID = MainOrderID;
        }

        public bool IsOrderInProduction(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID, ProfilProductionDate, TPSProductionDate FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int FactoryID = Convert.ToInt32(DT.Rows[0]["FactoryID"]);
                        if (FactoryID == 1)
                        {
                            if (DT.Rows[0]["ProfilProductionDate"] == DBNull.Value)
                                return false;
                            else
                                return true;
                        }
                        if (FactoryID == 2)
                        {
                            if (DT.Rows[0]["TPSProductionDate"] == DBNull.Value)
                                return false;
                            else
                                return true;
                        }
                        if (FactoryID == 0)
                        {
                            if (DT.Rows[0]["ProfilProductionDate"] == DBNull.Value && DT.Rows[0]["TPSProductionDate"] == DBNull.Value)
                                return false;
                            else
                                return true;
                        }
                    }
                    return true;
                }
            }
        }

        public bool IsOrderPrepared(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT IsPrepared FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return Convert.ToBoolean(DT.Rows[0]["Isprepared"]);
                }
            }
        }

        public void SearchDocNumber(int MainOrderID)
        {
            DataRow[] Rows = SearchMainOrdersDataTable.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() < 1)
                return;
            int MegaOrderID = Convert.ToInt32(Rows[0]["MegaOrderID"]);

            int index = MegaOrdersBindingSource.Find("MegaOrderID",
                                        SearchMainOrdersDataTable.Select("MainOrderID = " + MainOrderID)[0]["MegaOrderID"]);
            if (index == -1)
                return;

            MegaOrdersBindingSource.Position = index;

            while (MainOrdersBindingSource.Count == 0) ;

            object MOID = ((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"].ToString();

            MainOrdersBindingSource.Position = MainOrdersBindingSource.Find("MainOrderID", MainOrderID);

            CurrentMainOrderID = MainOrderID;
        }

        public void SearchPartDocNumber(string DocText)
        {
            string Search = string.Format("[DocNumber] LIKE '%" + DocText + "%'");

            SearchPartDocNumberBindingSource.Filter = Search;
        }

        public int MoveToMegaOrder(object DispatchDate)
        {
            if (DispatchDate == null)
            {
                MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", 0);
                return 0;
            }

            MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("DispatchDate", Convert.ToDateTime(DispatchDate).ToShortDateString());

            return Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"]);
        }

        public void MoveToDocNumber(int MainOrderID)
        {
            DataRow[] Rows = MainOrdersDataTable.Select("MainOrderID = " + MainOrderID);

            if (Rows.Count() < 1)
                return;

            int index = MegaOrdersBindingSource.Find("MegaOrderID", Rows[0]["MegaOrderID"]);
            if (index == -1)
                return;

            MegaOrdersBindingSource.Position = index;

            while (MainOrdersBindingSource.Count == 0) ;

            object MOID = ((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"].ToString();

            MainOrdersBindingSource.Position = MainOrdersBindingSource.Find("MainOrderID", MainOrderID);

            CurrentMainOrderID = MainOrderID;
        }

        public void MoveToPrepareOrder(int MainOrderID)
        {
            CurrentMegaOrderID = 0;

            CurrentMainOrderID = MainOrderID;
        }

        public void ChangeClient(int ClientID)
        {
            ClientsGroupsBindingSource.Position = ClientsGroupsBindingSource.Find("ClientGroupID",
                                    ClientsDataTable.Select("ClientID = " + ClientID)[0]["ClientGroupID"].ToString());
            ClientsBindingSource.Position = ClientsBindingSource.Find("ClientID", ClientID);
        }

        public int GetCurrentClientID(int MainOrderID)
        {
            int ClientID = -1;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter
                                ("SELECT ClientID FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                            ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                    }
                }
            }
            return ClientID;
        }

        public void MovePreparedMainOrder(string DocNumber, DateTime DispatchDate)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, MovePrepareDate FROM MainOrders WHERE DocNumber = '" + DocNumber + "'", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["MovePrepareDate"] = Security.GetCurrentDate();
                        DT.Rows[0]["MegaOrderID"] = MegaOrdersDataTable.Select("DispatchDate = '" + DispatchDate.ToShortDateString() + "'")[0]["MegaOrderID"];
                        DA.Update(DT);
                    }
                }
            }

            //MegaOrdersDataTable.Clear();
            //MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);
        }

        public void EditMainOrder(ref object DispatchDate, ref object TPSProductionDate, int MainOrderID, ref int ClientID, ref string DocNumber,
            ref string DebtDocNumber, ref int PriceTypeID, ref int DebtTypeID, ref bool IsSample,
            ref bool IsPrepare, ref string Notes, ref bool NeedCalculate,
            ref bool DoNotDispatch, ref bool TechDrilling, ref bool QuicklyOrder, ref bool ToAssembly, ref bool IsNotPaid)
        {
            //if (!CanRemoveMainOrder())
            //{
            //    MessageBox.Show("Заказ был отдан в производство и не может быть изменен", "Редактирование заказа");
            //    return false;
            //}

            InEditing = true;

            CurrentMainOrderID = MainOrderID;
            CurrentMegaOrderID = Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current)["MegaOrderID"]);

            ClientID = Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current)["ClientID"]);

            DispatchDate = (((DataRowView)MegaOrdersBindingSource.Current)["DispatchDate"]);
            TPSProductionDate = (((DataRowView)MainOrdersBindingSource.Current)["TPSProductionDate"]);
            DocNumber = (((DataRowView)MainOrdersBindingSource.Current)["DocNumber"]).ToString();
            PriceTypeID = Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current)["PriceTypeID"]);
            DebtTypeID = Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current)["DebtTypeID"]);
            IsSample = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["IsSample"]);
            IsPrepare = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["IsPrepared"]);
            Notes = ((DataRowView)MainOrdersBindingSource.Current)["Notes"].ToString();
            DebtDocNumber = ((DataRowView)MainOrdersBindingSource.Current)["DebtDocNumber"].ToString();
            NeedCalculate = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["NeedCalculate"]);
            DoNotDispatch = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["DoNotDispatch"]);
            TechDrilling = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["TechDrilling"]);
            QuicklyOrder = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["QuicklyOrder"]);
            ToAssembly = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["ToAssembly"]);
            IsNotPaid = Convert.ToBoolean(((DataRowView)MainOrdersBindingSource.Current)["IsNotPaid"]);

            ChangeClient(ClientID);
        }

        public void EditMainOrder(int MainOrderID, ref string DocNumber, ref int ClientID, ref int PriceTypeID, ref int DebtTypeID, ref bool IsSample,
            ref bool IsPrepare, ref string Notes, ref string DebtDocNumber, ref bool NeedCalculate,
            ref bool DoNotDispatch, ref bool TechDrilling, ref bool QuicklyOrder)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, ClientID, DocNumber, PriceTypeID," +
                " DebtTypeID, IsSample, IsPrepared, Notes, DebtDocNumber, NeedCalculate, DoNotDispatch, TechDrilling, QuicklyOrder FROM MainOrders WHERE MainOrderID = '" + MainOrderID + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    InEditing = true;

                    CurrentMainOrderID = MainOrderID;
                    CurrentMegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                    ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);

                    DocNumber = DT.Rows[0]["DocNumber"].ToString();
                    PriceTypeID = Convert.ToInt32(DT.Rows[0]["PriceTypeID"]);
                    DebtTypeID = Convert.ToInt32(DT.Rows[0]["DebtTypeID"]);
                    IsSample = Convert.ToBoolean(DT.Rows[0]["IsSample"]);
                    IsPrepare = Convert.ToBoolean(DT.Rows[0]["IsPrepared"]);
                    Notes = DT.Rows[0]["Notes"].ToString();
                    DebtDocNumber = DT.Rows[0]["DebtDocNumber"].ToString();
                    NeedCalculate = Convert.ToBoolean(DT.Rows[0]["NeedCalculate"]);
                    DoNotDispatch = Convert.ToBoolean(DT.Rows[0]["DoNotDispatch"]);
                    TechDrilling = Convert.ToBoolean(DT.Rows[0]["TechDrilling"]);
                    QuicklyOrder = Convert.ToBoolean(DT.Rows[0]["QuicklyOrder"]);

                    ChangeClient(ClientID);
                }
            }

        }

        public void GetDispatchDate(int MainOrderID, ref object DispatchDate)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate FROM MegaOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]);
                }
            }
        }

        public int GetCurrentMegaOrderID()
        {
            return Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);
        }

        public void RefreshMainOrders()
        {
            MainOrdersDataTable.Clear();
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);
        }

        public void RemoveCurrentMainOrder(int MainOrderID)
        {
            //if (!UpdateSelectedMegaOrder())
            //{
            //    MessageBox.Show("Один или несколько заказов были удалены, повторите операцию", "Ошибка");

            //    UpdateMegaOrders();

            //    return;
            //}

            //if (!UpdateSelectedMainOrders())
            //{
            //    MessageBox.Show("Один или несколько подзаказов были удалены, повторите операцию", "Ошибка");

            //    UpdateMainOrders(Profil, TPS);

            //    return;
            //}

            int Pos = MainOrdersBindingSource.Position;

            //check status

            MainOrdersFrontsOrders.DeleteOrder(MainOrderID);

            MainOrdersDecorOrders.DeleteOrder(MainOrderID);

            OrdersTabControl.TabPages[0].PageVisible = MainOrdersFrontsOrders.Filter(MainOrderID);
            OrdersTabControl.TabPages[1].PageVisible = MainOrdersDecorOrders.Filter(MainOrderID);

            DataRow[] Row = MainOrdersDataTable.Select("MainOrderID = " + MainOrderID);

            Row[0].Delete();

            DataTable DT = MainOrdersDataTable.Copy();

            if (DT.Columns.Contains("BatchNumber"))
                DT.Columns.Remove("BatchNumber");
            MainOrdersDataAdapter.Update(DT);
            DT.Dispose();

            //MainOrdersDataAdapter.Update(MainOrdersDataTable);

            //остается на позиции удаленного заказа, а не перемещается в начало
            if (MainOrdersBindingSource.Count > 0)
                if (Pos >= MainOrdersBindingSource.Count)
                    MainOrdersBindingSource.MoveLast();
                else
                    MainOrdersBindingSource.Position = Pos;
        }

        public void RemoveCurrentMegaOrder()
        {
            int Pos = MegaOrdersBindingSource.Position;

            int MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current)["MegaOrderID"]);

            int[] MainOrders = new Int32[MainOrdersDataTable.Rows.Count];

            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                MainOrders[i] = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                MainOrdersFrontsOrders.DeleteOrder(MainOrders[i]);

                MainOrdersDecorOrders.DeleteOrder(MainOrders[i]);

                DataRow[] Row = MainOrdersDataTable.Select("MainOrderID = " + MainOrders[i]);

                Row[0].Delete();
            }

            DataTable DT = MainOrdersDataTable.Copy();

            if (DT.Columns.Contains("BatchNumber"))
                DT.Columns.Remove("BatchNumber");
            MainOrdersDataAdapter.Update(DT);
            DT.Dispose();

            //MainOrdersDataAdapter.Update(MainOrdersDataTable);

            MegaOrdersBindingSource.RemoveCurrent();

            MegaOrdersDataAdapter.Update(MegaOrdersDataTable);

            //остается на позиции удаленного заказа, а не перемещается в начало
            if (MegaOrdersBindingSource.Count > 0)
                if (Pos >= MegaOrdersBindingSource.Count)
                {
                    MegaOrdersBindingSource.MoveLast();
                    MegaOrdersDataGrid.Rows[MegaOrdersDataGrid.Rows.Count - 1].Selected = true;
                }
                else
                    MegaOrdersBindingSource.Position = Pos;
        }

        public void ChangeDate(string DesireDate)
        {
            //if (!UpdateSelectedMegaOrder())
            //{
            //    MessageBox.Show("Один или несколько заказов были удалены, повторите операцию", "Ошибка");

            //    UpdateMegaOrders();

            //    return;
            //}

            if (DesireDate.Length == 0)
                ((DataRowView)MegaOrdersBindingSource.Current).Row["DesireDate"] = DBNull.Value;
            else
                ((DataRowView)MegaOrdersBindingSource.Current).Row["DesireDate"] = DesireDate;

            MegaOrdersDataAdapter.Update(MegaOrdersDataTable);
        }

        public int[] GetMainOrders()
        {
            int[] rows = new int[MainOrdersDataTable.Rows.Count];

            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                rows[i] = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);

            return rows;
        }

        public bool SetReorderDocNumber(string DocNumber, string DebtDocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DocNumber, ReorderDocNumber, DebtDocNumber FROM MainOrders WHERE DocNumber = '" + DebtDocNumber + "'", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return false;

                        DT.Rows[0]["ReorderDocNumber"] = DocNumber;

                        DA.Update(DT);
                    }
                }
            }

            return true;
        }

        public int GetMegaOrderID(DateTime DispatchDate)
        {
            DataRow[] DR = MegaOrdersDataTable.Select("DispatchDate = '" + DispatchDate.ToShortDateString() + "'");

            if (DR.Count() == 0)
                return -1;

            return Convert.ToInt32(DR[0]["MegaOrderID"]);
        }

        public bool IsOrdersInDate(DateTime DispatchDate)
        {
            DataRow[] DR = MegaOrdersDataTable.Select("DispatchDate = '" + DispatchDate.ToShortDateString() + "'");

            if (DR.Count() == 0)
                return false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP (1) MainOrderID FROM MainOrders WHERE MegaOrderID = '" + DR[0]["MegaOrderID"] + "'", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    return DA.Fill(DT) > 0;
                }
            }

        }

        public bool IsOrderPackAlloc(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ProfilPackAllocStatusID, TPSPackAllocStatusID FROM MainOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    int ProfilPackAllocStatusID = Convert.ToInt32(DT.Rows[0]["ProfilPackAllocStatusID"]);
                    int TPSPackAllocStatusID = Convert.ToInt32(DT.Rows[0]["TPSPackAllocStatusID"]);

                    if (ProfilPackAllocStatusID != 0 || TPSPackAllocStatusID != 0)
                        return true;
                }
            }
            return false;
        }

        public bool IsPackagesAlreadyPacked(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE MainOrderID = " + MainOrderID,
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

        public void FixOrderEvent(int MainOrderID, string Event)
        {
            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT Packages.*, PackageDetails.OrderID FROM PackageDetails
                INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM PackagesEvents";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (TempDT.Rows.Count > 0)
                        {
                            for (int i = 0; i < TempDT.Rows.Count; i++)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                NewRow["Event"] = Event;
                                NewRow["EventDate"] = Security.GetCurrentDate();
                                NewRow["EventUserID"] = Security.CurrentUserID;
                                DT.Rows.Add(NewRow);
                            }
                            DA.Update(DT);
                        }
                        else
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow["MainOrderID"] = MainOrderID;
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

        public void ClearPackages(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }

            int MegaOrderID = -1;
            int MainProfilPackCount = 0;
            int MainTPSPackCount = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 MainOrderID, MegaOrderID," +
                " ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                            MainProfilPackCount = Convert.ToInt32(DT.Rows[0]["ProfilPackCount"]);
                            MainTPSPackCount = Convert.ToInt32(DT.Rows[0]["TPSPackCount"]);

                            DT.Rows[0]["ProfilPackAllocStatusID"] = 0;
                            DT.Rows[0]["TPSPackAllocStatusID"] = 0;
                            DT.Rows[0]["ProfilPackCount"] = 0;
                            DT.Rows[0]["TPSPackCount"] = 0;

                            DA.Update(DT);
                        }
                    }
                }
            }

            int MegaProfilPackAllocStatusID = 0;
            int MegaTPSPackAllocStatusID = 0;
            int MegaProfilPackCount = 0;
            int MegaTPSPackCount = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 MegaOrderID," +
                " ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            MegaProfilPackAllocStatusID = Convert.ToInt32(DT.Rows[0]["ProfilPackAllocStatusID"]);
                            MegaTPSPackAllocStatusID = Convert.ToInt32(DT.Rows[0]["TPSPackAllocStatusID"]);
                            MegaProfilPackCount = Convert.ToInt32(DT.Rows[0]["ProfilPackCount"]);
                            MegaTPSPackCount = Convert.ToInt32(DT.Rows[0]["TPSPackCount"]);

                            if (MainProfilPackCount > 0)
                            {
                                if (MegaProfilPackAllocStatusID > 0)
                                {
                                    if ((MegaProfilPackCount - MainProfilPackCount) == 0)
                                        DT.Rows[0]["ProfilPackAllocStatusID"] = 0;
                                    else
                                        DT.Rows[0]["ProfilPackAllocStatusID"] = 1;
                                }
                            }

                            if (MainTPSPackCount > 0)
                            {
                                if (MegaTPSPackAllocStatusID > 0)
                                {
                                    if ((MegaTPSPackCount - MainTPSPackCount) == 0)
                                        DT.Rows[0]["TPSPackAllocStatusID"] = 0;
                                    else
                                        DT.Rows[0]["TPSPackAllocStatusID"] = 1;
                                }
                            }

                            DT.Rows[0]["ProfilPackCount"] = MegaProfilPackCount - MainProfilPackCount;
                            DT.Rows[0]["TPSPackCount"] = MegaTPSPackCount - MainTPSPackCount;

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public ArrayList GetMainOrders(int MegaOrderID, int FactoryID,
            bool Curved, bool Aluminium, ArrayList DecorIDs)
        {
            ArrayList array = new ArrayList();

            if (!Curved && !Aluminium && DecorIDs.Count < 1)
                return array;

            string FactoryFilter = string.Empty;

            if (FactoryID > 0)
                FactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            string AluminiumFilter = "(FrontID IN (3680,3681,3682,3683,3684,3685))";
            string CurvedFilter = "(Width = -1)";
            string FrontsFilter = string.Empty;
            string FrontsSelectCommand = string.Empty;
            string DecorSelectCommand = string.Empty;
            string CommonSelectCommand = string.Empty;

            int[] IDs = DecorIDs.OfType<int>().ToArray();

            if (Aluminium && !Curved)
            {
                FrontsFilter = " AND (" + AluminiumFilter + ")";
            }
            if (!Aluminium && Curved)
            {
                FrontsFilter = " AND (" + CurvedFilter + ")";
            }
            if (Aluminium && Curved)
            {
                AluminiumFilter = " (FrontID IN (3680,3681,3682,3683,3684,3685))";
                CurvedFilter = " (Width = -1)";
                FrontsFilter = " AND (" + AluminiumFilter + " OR " + CurvedFilter + ")";
            }

            if (Curved || Aluminium)
                FrontsSelectCommand = "SELECT MainOrderID FROM FrontsOrders" +
                    " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")" + FrontsFilter + FactoryFilter;

            if (DecorIDs.Count > 0)
                DecorSelectCommand = "SELECT MainOrderID FROM DecorOrders WHERE ProductID IN (" + string.Join(",", IDs) + ")" +
                    " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")" + FactoryFilter;

            if (FrontsSelectCommand.Length > 0 && DecorSelectCommand.Length > 0)
                CommonSelectCommand = FrontsSelectCommand + " UNION " + DecorSelectCommand;
            if (FrontsSelectCommand.Length > 0 && DecorSelectCommand.Length == 0)
                CommonSelectCommand = FrontsSelectCommand;
            if (FrontsSelectCommand.Length == 0 && DecorSelectCommand.Length > 0)
                CommonSelectCommand = DecorSelectCommand;

            if (CommonSelectCommand.Length > 0)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(CommonSelectCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                        }
                    }
                }
            }
            return array;
        }

        public ArrayList GetMainOrders(int MegaOrderID)
        {
            ArrayList array = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                    }
                }
            }
            return array;
        }

        public ArrayList GetMainOrders(int MegaOrderID, int FactoryID)
        {
            ArrayList array = new ArrayList();
            string FactoryFilter = string.Empty;

            if (FactoryID > 0)
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            if (FactoryID == -1)
                FactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                    }
                }
            }
            return array;
        }

        public ArrayList GetMainOrders(int[] MegaOrders, int FactoryID)
        {
            ArrayList array = new ArrayList();
            string FactoryFilter = string.Empty;

            if (FactoryID > 0)
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            if (FactoryID == -1)
                FactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + FactoryFilter, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                    }
                }
            }
            return array;
        }

        public ArrayList GetMainOrders(int MegaOrderID, int FactoryID, bool Fronts)
        {
            ArrayList array = new ArrayList();
            string FrontsFilter = string.Empty;
            string ProductionFilter = string.Empty;

            if (Fronts)
                FrontsFilter = " AND MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FrontConfigID IN " +
                    "(SELECT FrontConfigID FROM infiniu2_catalog.dbo.FrontsConfig WHERE FactoryID=" +
                    FactoryID + ") AND Width <> -1 AND FrontID NOT IN (3680,3681,3682,3683,3684,3685))";

            if (FactoryID == 1)
                ProductionFilter = " AND (ProfilProductionStatusID = 1 AND ProfilStorageStatusID = 1 AND ProfilDispatchStatusID = 1)";
            if (FactoryID == 2)
                ProductionFilter = " AND (TPSProductionStatusID = 1 AND TPSStorageStatusID = 1 AND TPSDispatchStatusID = 1)";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + ProductionFilter + FrontsFilter, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                    }
                }
            }
            return array;
        }

        public ArrayList GetSelectedMainOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value));

            return array;
        }

        public ArrayList GetSelectedMegaOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value));

            return array;
        }

        public void FindDocNumber(string DocNumber)
        {
            int MainOrderID = -1;
            int MegaOrderID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID FROM MainOrders" +
                " WHERE DocNumber = '" + DocNumber + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                    MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                }
            }


            int MegaIndex = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);

            if (MegaIndex == -1)
                return;

            MegaOrdersBindingSource.Position = MegaIndex;
            int MainIndex = MainOrdersBindingSource.Find("MainOrderID", MainOrderID);

            if (MainIndex == -1)
                return;

            MainOrdersBindingSource.Position = MainIndex;
        }

        public void SetFirstOperator(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FirstOperatorID FROM MainOrders" +
                " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["FirstOperatorID"] = Security.CurrentUserID;

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetDoubleOrder(int MainOrderID, bool DoubleOrder)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DoubleOrder FROM MainOrders" +
                " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["DoubleOrder"] = DoubleOrder;

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetDoubleOrderParameters(int MainOrderID, bool DoubleOrder)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, SecondOperatorID, DoubleOrder FROM MainOrders" +
                " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["SecondOperatorID"] = Security.CurrentUserID;
                        DT.Rows[0]["DoubleOrder"] = DoubleOrder;

                        DA.Update(DT);
                    }
                }
            }
        }

        public int GetFirstOperator(int MainOrderID)
        {
            int FirstOperator = -1;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FirstOperatorID FROM MainOrders" +
                    " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        FirstOperator = Convert.ToInt32(DT.Rows[0]["FirstOperatorID"]);
                }
            }
            return FirstOperator;
        }

        public int GetSecondOperator(int MainOrderID)
        {
            int SecondOperator = -1;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SecondOperatorID FROM MainOrders" +
                    " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        SecondOperator = Convert.ToInt32(DT.Rows[0]["SecondOperatorID"]);
                }
            }
            return SecondOperator;
        }

        public object GetFirstDocDateTime(int MainOrderID)
        {
            object DateTime = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocDateTime FROM MainOrders" +
                    " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        DateTime = Convert.ToDateTime(DT.Rows[0]["DocDateTime"]);
                }
            }
            return DateTime;
        }

        public object GetFirstSaveDateTime(int MainOrderID)
        {
            object DateTime = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SaveDateTime FROM MainOrders" +
                    " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        DateTime = Convert.ToDateTime(DT.Rows[0]["SaveDateTime"]);
                }
            }
            return DateTime;
        }

        public bool PreSaveDoubleOrder(string DocNumber,
            int FirstOperatorID, DateTime FirstDocDateTime, DateTime FirstSaveDateTime, int SecondOperatorID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DoubleOrders" +
                " WHERE DocNumber = '" + DocNumber + "'" +
                " AND FirstOperatorID = " + FirstOperatorID +
                " AND CONVERT(VARCHAR(19), FirstDocDateTime, 120) = '" + FirstDocDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " AND CONVERT(VARCHAR(19), FirstSaveDateTime, 120) = '" + FirstSaveDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " AND SecondOperatorID = " + SecondOperatorID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SaveDoubleOrder(string DocNumber,
            int FirstOperatorID, DateTime FirstDocDateTime, DateTime FirstSaveDateTime, int FirstErrorsCount,
            int SecondOperatorID, DateTime SecondDocDateTime, DateTime SecondSaveDateTime, int SecondErrorsCount)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM DoubleOrders",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["DocNumber"] = DocNumber;
                        NewRow["FirstOperatorID"] = FirstOperatorID;
                        NewRow["FirstDocDateTime"] = FirstDocDateTime;
                        NewRow["FirstSaveDateTime"] = FirstSaveDateTime;
                        NewRow["FirstErrors"] = FirstErrorsCount;
                        NewRow["SecondOperatorID"] = SecondOperatorID;
                        NewRow["SecondDocDateTime"] = SecondDocDateTime;
                        NewRow["SecondSaveDateTime"] = SecondSaveDateTime;
                        NewRow["SecondErrors"] = SecondErrorsCount;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        public bool IsDoubleOrder(int MainOrderID)
        {
            bool DoubleOrder = false;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DoubleOrder FROM MainOrders" +
                    " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        DoubleOrder = Convert.ToBoolean(DT.Rows[0]["DoubleOrder"]);
                }
            }
            return DoubleOrder;
        }

        public bool IsDocNumberExistInPayments(string DocNumber, ref string ClientName, ref DateTime DateFrom, ref DateTime DateTo,
            ref string DebtType, ref decimal Cost, ref DateTime DispatchDate)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PaymentDetail.DispatchDate, PaymentDetail.ClientName, PaymentDetail.DocNumber, PaymentDetail.SamplesWriteOffCost," +
                " PaymentDetail.DebtTypeID, PaymentDetail.DebtCost, PaymentWeeks.DateFrom, PaymentWeeks.DateTo, infiniu2_zovreference.dbo.DebtTypes.DebtType" +
                " FROM PaymentDetail" +
                " INNER JOIN PaymentWeeks ON PaymentDetail.PaymentWeekID = PaymentWeeks.PaymentWeekID" +
                " INNER JOIN infiniu2_zovreference.dbo.DebtTypes ON PaymentDetail.DebtTypeID = infiniu2_zovreference.dbo.DebtTypes.DebtTypeID" +
                " WHERE DocNumber = '" + DocNumber + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        ClientName = DT.Rows[0]["ClientName"].ToString();
                        DateFrom = Convert.ToDateTime(DT.Rows[0]["DateFrom"]);
                        DateTo = Convert.ToDateTime(DT.Rows[0]["DateTo"]);
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]);
                        if (Convert.ToInt32(DT.Rows[0]["DebtTypeID"]) == 0)
                        {
                            DebtType = "Списание за образцы";
                            Cost = Convert.ToDecimal(DT.Rows[0]["SamplesWriteOffCost"]);
                        }
                        else
                        {
                            DebtType = DT.Rows[0]["DebtType"].ToString();
                            Cost = Convert.ToDecimal(DT.Rows[0]["DebtCost"]);
                        }
                        return true;
                    }
                    else
                        return false;
                }
            }
        }


        public DataTable GetReadyDyeingOrders(int MegaOrderID)
        {
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT DISTINCT DyeingAssignmentDetails.DyeingCartID, DyeingAssignmentDetails.MainOrderID, ClientName, DocNumber, DyeingCarts.Square, DyeingAssignmentDetails.WorkAssignmentID, DyeingAssignmentDetails.GroupType, DyeingAssignments.PrintDateTime FROM DyeingAssignmentDetails
                INNER JOIN infiniu2_zovorders.dbo.MainOrders ON dbo.DyeingAssignmentDetails.MainOrderID = infiniu2_zovorders.dbo.MainOrders.MainOrderID AND DyeingAssignmentDetails.MainOrderID IN (SELECT MainOrderID FROM infiniu2_zovorders.dbo.MainOrders WHERE MegaOrderID=" + MegaOrderID + @")
                INNER JOIN infiniu2_zovreference.dbo.Clients ON infiniu2_zovorders.dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
                INNER JOIN DyeingCarts ON DyeingAssignmentDetails.DyeingCartID = DyeingCarts.DyeingCartID
                INNER JOIN DyeingAssignments ON DyeingAssignmentDetails.DyeingAssignmentID = DyeingAssignments.DyeingAssignmentID AND DyeingAssignmentDetails.GroupType = 0 ORDER BY ClientName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            return DT;
        }

        public void DyeingOrdersToExcel(DataTable TempTable)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Готовность на покраске");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int DisplayIndex = 0;
            int RowIndex = 1;
            sheet1.SetColumnWidth(DisplayIndex++, 25 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);

            HSSFFont HeaderF = hssfworkbook.CreateFont();
            HeaderF.FontHeightInPoints = 12;
            HeaderF.Boldweight = 12 * 256;
            HeaderF.FontName = "Calibri";
            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 11;
            SimpleF.FontName = "Calibri";
            HSSFCellStyle ItemCS = hssfworkbook.CreateCellStyle();
            ItemCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            ItemCS.SetFont(SimpleF);
            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.SetFont(SimpleF);
            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.SetFont(HeaderF);
            HSSFCellStyle CostCS = hssfworkbook.CreateCellStyle();
            CostCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CostCS.SetFont(SimpleF);

            HSSFCell cell;
            DisplayIndex = 0;
            HSSFRow r = sheet1.CreateRow(RowIndex);
            cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Клиент");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Кухня");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Квадратура");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(r, DisplayIndex++, "Дата печати");
            cell.CellStyle = HeaderCS;
            RowIndex++;
            DisplayIndex = 0;

            for (int x = 0; x < TempTable.Rows.Count; x++)
            {
                DisplayIndex = 0;
                r = sheet1.CreateRow(RowIndex);
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["ClientName"].ToString());
                cell.CellStyle = ItemCS;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(TempTable.Rows[x]["DocNumber"].ToString());
                cell.CellStyle = ItemCS;
                if (TempTable.Rows[x]["Square"] != DBNull.Value)
                {
                    cell = r.CreateCell(DisplayIndex++);
                    cell.SetCellValue(Convert.ToDouble(TempTable.Rows[x]["Square"]));
                    cell.CellStyle = CostCS;
                }
                if (TempTable.Rows[x]["PrintDateTime"] != DBNull.Value)
                {
                    cell = r.CreateCell(DisplayIndex++);
                    cell.SetCellValue(TempTable.Rows[x]["PrintDateTime"].ToString());
                    cell.CellStyle = SimpleCS;
                }
                RowIndex++;
            }
            string FileName = "Готовность на покраске";
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();
            System.Diagnostics.Process.Start(file.FullName);
        }

    }






    public class ZOVProductionReport : IAllFrontParameterName, IIsMarsel
    {
        DataTable ClientsDataTable = null;

        DataTable FrontsResultDataTable = null;
        DataTable[] DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable FrontsDrillTypesDataTable = null;

        Modules.ZOV.DecorCatalogOrder DecorCatalog = null;

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

        public ZOVProductionReport(ref Modules.ZOV.DecorCatalogOrder tDecorCatalog)
        {
            DecorCatalog = tDecorCatalog;

            Create();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
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
            //SelectCommand = @"SELECT * FROM InsetColors";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(InsetColorsDataTable);
            //}
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();
            FrontsDrillTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsDrillTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDrillTypesDataTable);
            }

            CreateFrontsDataTable();
            CreateDecorDataTable();
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
            DecorResultDataTable = new DataTable[DecorCatalog.DecorProductsCount];
            DecorOrdersDataTable = new DataTable();

            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
                DecorOrdersDataTable = new DataTable();
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInset"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
        }

        private void CreateDecorDataTable()
        {
            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
            {
                DecorResultDataTable[i] = new DataTable();

                DecorResultDataTable[i].Columns.Add("Product", Type.GetType("System.String"));
                DecorResultDataTable[i].Columns.Add("Color", Type.GetType("System.String"));
                DecorResultDataTable[i].Columns.Add("Height", Type.GetType("System.Int32"));
                DecorResultDataTable[i].Columns.Add("Width", Type.GetType("System.Int32"));
                DecorResultDataTable[i].Columns.Add("Count", Type.GetType("System.Int32"));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            }
        }

        private string GetMainOrderNotesByMainOrder(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        public string GetClientNameByMainOrder(int MainOrderID)
        {
            string ClientName = "";

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientName FROM Clients WHERE ClientID = " +
                    " (SELECT ClientID FROM infiniu2_zovorders.dbo.MainOrders WHERE MainOrderID = " + MainOrderID + ")",
                    ConnectionStrings.ZOVReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientName = DT.Rows[0]["ClientName"].ToString();
                }
            }

            return ClientName;
        }

        public string GetDispatchDateByMainOrder(int MainOrderID)
        {
            string DispatchDate = "";
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

        public string GetOrderNumberByMainOrder(int MainOrderID)
        {
            string OrderNumber = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocNumber FROM MainOrders WHERE MainOrderID = " + MainOrderID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        OrderNumber = DT.Rows[0]["DocNumber"].ToString();
                }
            }

            return OrderNumber;
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

        private string GetFrontDrillTypeName(int FrontDrillTypeID)
        {
            DataRow[] Rows = FrontsDrillTypesDataTable.Select("FrontDrillTypeID = " + FrontDrillTypeID);
            return Rows[0]["FrontDrillType"].ToString();
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
            string FrontType = string.Empty;
            try
            {
                DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
                FrontType = Rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return FrontType;
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

        public bool IsAluminium(int FrontID)
        {
            DataRow[] Row = FrontsDataTable.Select("FrontID = " + FrontID);

            if (Row.Count() > 0 && Row[0]["FrontName"].ToString()[0] == 'Z')
                return true;

            return false;
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

        private void FillFrontsDepersonalized(bool NeedAluminium)
        {
            FrontsResultDataTable.Clear();

            int Height = 0;
            int Width = 0;
            int Count = 0;
            decimal Square = 0;
            decimal AlHandsSize = 0;

            DataTable DT1 = FrontsOrdersDataTable.Clone();
            DataTable DT2 = FrontsOrdersDataTable.Clone();
            DataTable DT = new DataTable();

            //сначала витрины
            string filter = "InsetTypeID IN (1)";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            //решетки
            filter = "InsetTypeID IN (685,686,687,688,29470,29471)";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            //глухие
            DataRow[] rows = InsetTypesDataTable.Select("GroupID = 3 OR GroupID = 4");
            filter = string.Empty;
            foreach (DataRow item in rows)
                filter += item["InsetTypeID"].ToString() + ",";
            if (filter.Length > 0)
                filter = "(InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + "))";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            //все остальные
            rows = InsetTypesDataTable.Select("GroupID <> 3 AND GroupID <> 4 AND InsetTypeID NOT IN (1,685,686,687,688,29470,29471)");
            filter = string.Empty;
            foreach (DataRow item in rows)
                filter += item["InsetTypeID"].ToString() + ",";
            if (filter.Length > 0)
                filter = "(InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + "))";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            DT1.Dispose();
            foreach (DataRow Row in DT2.Rows)
            {
                string Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string Patina = GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                string InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                bool bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                bool bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    bool bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }

                string InsetColor = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                Height = Convert.ToInt32(Row["Height"]);
                Width = Convert.ToInt32(Row["Width"]);
                Count = Convert.ToInt32(Row["Count"]);
                Square = Convert.ToDecimal(Row["Square"]);

                if (NeedAluminium && IsAluminium(Convert.ToInt32(Row["FrontID"])))
                {
                    if (Convert.ToInt32(Row["InsetTypeID"]) == 2)
                        InsetType = InsetColor;
                    if (Row["FrontDrillTypeID"] != DBNull.Value)
                        InsetColor = GetFrontDrillTypeName(Convert.ToInt32(Row["FrontDrillTypeID"]));
                    if (Row["AlHandsSize"] != DBNull.Value)
                    {
                        AlHandsSize = Convert.ToDecimal(Row["AlHandsSize"]);
                        TechnoInset = AlHandsSize.ToString();
                    }
                }

                DataRow[] fRow = FrontsResultDataTable.Select("Front = '" + Front + "' AND Patina = '" + Patina +
                    "' AND InsetType = '" + InsetType + "' AND FrameColor = '" + FrameColor + "' AND InsetColor = '" + InsetColor +
                    "' AND TechnoInset = '" + TechnoInset + "' AND Height = '" + Height.ToString() + "' AND Width = '" + Width.ToString() + "'");
                if (fRow.Count() == 0)
                {
                    DataRow NewRow = FrontsResultDataTable.NewRow();

                    NewRow["Front"] = Front;
                    NewRow["FrameColor"] = FrameColor;
                    NewRow["Patina"] = Patina;
                    NewRow["InsetType"] = InsetType;
                    NewRow["InsetColor"] = InsetColor;
                    NewRow["TechnoInset"] = TechnoInset;
                    NewRow["Height"] = Height;
                    NewRow["Width"] = Width;
                    NewRow["Square"] = Row["Square"];
                    NewRow["Count"] = Count;
                    NewRow["Notes"] = Row["Notes"];

                    FrontsResultDataTable.Rows.Add(NewRow);
                }
                else
                {
                    fRow[0]["Square"] = Convert.ToDecimal(fRow[0]["Square"]) + Square;
                    fRow[0]["Count"] = Convert.ToDecimal(fRow[0]["Count"]) + Count;
                }
            }
            DT2.Dispose();
        }

        private void FillDecorDepersonalized()
        {
            string filter = string.Empty;
            string Product = string.Empty;
            string Decor = string.Empty;
            string Color = string.Empty;

            int Height = 0;
            int Width = 0;
            int Count = 0;

            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
            {
                DecorResultDataTable[i].Clear();
                DecorResultDataTable[i].AcceptChanges();

                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow Row in Rows)
                {
                    Product = DecorCatalog.DecorProductsDataTable.Rows[i]["ProductName"].ToString() + " " +
                                     DecorCatalog.GetItemName(Convert.ToInt32(Row["DecorID"]));

                    Count = Convert.ToInt32(Row["Count"]);

                    filter = "Product = '" + Product + " " + Decor + "'";

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    {
                        Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                        if (Convert.ToInt32(Row["PatinaID"]) != -1)
                            Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                        filter += " AND Color = '" + Color.ToString() + "'";
                    }

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                    {
                        Height = Convert.ToInt32(Row["Height"]);
                        filter += " AND Height = '" + Height.ToString() + "'";
                    }

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                    {
                        Height = Convert.ToInt32(Row["Length"]);
                        filter += " AND Height = '" + Height.ToString() + "'";
                    }

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    {
                        Width = Convert.ToInt32(Row["Width"]);
                        filter += " AND Width = '" + Width.ToString() + "'";
                    }

                    DataRow[] dRow = DecorResultDataTable[i].Select(filter);

                    if (dRow.Count() == 0)
                    {
                        DataRow NewRow = DecorResultDataTable[i].NewRow();

                        NewRow["Product"] = Product;

                        //if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                        //{
                        //    if (Convert.ToInt32(Row["Height"]) != -1)
                        //        NewRow["Height"] = Row["Height"];
                        //    if (Convert.ToInt32(Row["Length"]) != -1)
                        //        NewRow["Height"] = Row["Length"];
                        //}
                        //if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                        //{
                        //    if (Convert.ToInt32(Row["Length"]) != -1)
                        //        NewRow["Height"] = Row["Length"];
                        //    if (Convert.ToInt32(Row["Height"]) != -1)
                        //        NewRow["Height"] = Row["Height"];
                        //}
                        if (Convert.ToInt32(Row["Height"]) != -1)
                            NewRow["Height"] = Row["Height"];
                        if (Convert.ToInt32(Row["Length"]) != -1)
                            NewRow["Height"] = Row["Length"];
                        if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                            NewRow["Width"] = Row["Width"];

                        if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                            NewRow["Color"] = Color;

                        NewRow["Count"] = Row["Count"];
                        NewRow["Notes"] = Row["Notes"];

                        DecorResultDataTable[i].Rows.Add(NewRow);
                    }
                    else
                    {
                        dRow[0]["Count"] = Convert.ToDecimal(dRow[0]["Count"]) + Count;
                    }
                }
            }
        }

        private void FillFrontsByMainOrder(bool NeedAluminium)
        {
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();
                string Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string Patina = GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                string InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                bool bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                bool bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    bool bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }

                string InsetColor = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                if (NeedAluminium && IsAluminium(Convert.ToInt32(Row["FrontID"])))
                {
                    if (Convert.ToInt32(Row["InsetTypeID"]) == 2)
                        InsetType = InsetColor;
                    if (Row["FrontDrillTypeID"] != DBNull.Value)
                        InsetColor = GetFrontDrillTypeName(Convert.ToInt32(Row["FrontDrillTypeID"]));
                    if (Row["AlHandsSize"] != DBNull.Value)
                        TechnoInset = Row["AlHandsSize"].ToString();
                }

                NewRow["Front"] = Front;
                NewRow["FrameColor"] = FrameColor;
                NewRow["Patina"] = Patina;
                NewRow["InsetType"] = InsetType;
                NewRow["InsetColor"] = InsetColor;
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Square"] = Row["Square"];
                NewRow["Notes"] = Row["Notes"];

                FrontsResultDataTable.Rows.Add(NewRow);
            }
        }

        private void FillDecorByMainOrder()
        {
            string filter = string.Empty;
            string Product = string.Empty;
            string Decor = string.Empty;
            string Color = string.Empty;

            int Height = 0;
            int Width = 0;
            int Count = 0;

            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
            {
                DecorResultDataTable[i].Clear();
                DecorResultDataTable[i].AcceptChanges();

                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow Row in Rows)
                {
                    Product = DecorCatalog.DecorProductsDataTable.Rows[i]["ProductName"].ToString() + " " +
                                     DecorCatalog.GetItemName(Convert.ToInt32(Row["DecorID"]));

                    Count = Convert.ToInt32(Row["Count"]);

                    filter = "Product = '" + Product + " " + Decor + "'";

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    {
                        Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                        if (Convert.ToInt32(Row["PatinaID"]) != -1)
                            Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                        filter += " AND Color = '" + Color.ToString() + "'";
                    }

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                    {
                        Height = Convert.ToInt32(Row["Height"]);
                        filter += " AND Height = '" + Height.ToString() + "'";
                    }

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                    {
                        Height = Convert.ToInt32(Row["Length"]);
                        filter += " AND Height = '" + Height.ToString() + "'";
                    }

                    if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    {
                        Width = Convert.ToInt32(Row["Width"]);
                        filter += " AND Width = '" + Width.ToString() + "'";
                    }

                    DataRow[] dRow = DecorResultDataTable[i].Select(filter);

                    if (dRow.Count() == 0)
                    {
                        DataRow NewRow = DecorResultDataTable[i].NewRow();

                        NewRow["Product"] = Product;

                        //if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                        //{
                        //    if (Convert.ToInt32(Row["Height"]) != -1)
                        //        NewRow["Height"] = Row["Height"];
                        //    if (Convert.ToInt32(Row["Length"]) != -1)
                        //        NewRow["Height"] = Row["Length"];
                        //}
                        //if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                        //{
                        //    if (Convert.ToInt32(Row["Length"]) != -1)
                        //        NewRow["Height"] = Row["Length"];
                        //    if (Convert.ToInt32(Row["Height"]) != -1)
                        //        NewRow["Height"] = Row["Height"];
                        //}
                        if (Convert.ToInt32(Row["Height"]) != -1)
                            NewRow["Height"] = Row["Height"];
                        if (Convert.ToInt32(Row["Length"]) != -1)
                            NewRow["Height"] = Row["Length"];
                        if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                            NewRow["Width"] = Row["Width"];

                        if (DecorCatalog.HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                            NewRow["Color"] = Color;

                        NewRow["Count"] = Row["Count"];
                        NewRow["Notes"] = Row["Notes"];

                        DecorResultDataTable[i].Rows.Add(NewRow);
                    }
                    else
                    {
                        dRow[0]["Count"] = Convert.ToDecimal(dRow[0]["Count"]) + Count;
                    }
                }
            }
        }

        private bool FilterFrontsOrders(int MainOrderID, int FactoryID)
        {
            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM FrontsOrders WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return (FrontsOrdersDataTable.Rows.Count > 0);
        }

        private bool FilterDecorOrders(int MainOrderID, int FactoryID)
        {
            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE FactoryID = " + FactoryID + " AND MainOrderID=" + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private bool GetCurvedAndAluminiumFronts(int MainOrderID, int FactoryID, bool Curved, bool Aluminium)
        {
            if (!Curved && !Aluminium)
                return false;

            string FactoryFilter = string.Empty;

            if (FactoryID > 0)
                FactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            string AluminiumFilter = "(FrontID IN (3680,3681,3682,3683,3684,3685))";
            string CurvedFilter = "(Width = -1)";
            string Filter = string.Empty;

            if (Aluminium && !Curved)
            {
                Filter = " AND (" + AluminiumFilter + ")";
            }
            if (!Aluminium && Curved)
            {
                Filter = " AND (" + CurvedFilter + ")";
            }
            if (Aluminium && Curved)
            {
                AluminiumFilter = " (FrontID IN (3680,3681,3682,3683,3684,3685))";
                CurvedFilter = " (Width = -1)";
                Filter = " AND (" + AluminiumFilter + " OR " + CurvedFilter + ")";
            }

            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE MainOrderID = " + MainOrderID + Filter + FactoryFilter, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return (FrontsOrdersDataTable.Rows.Count > 0);
        }

        private bool GetSomeDecor(int MainOrderID, int FactoryID, ArrayList DecorIDs)
        {
            if (DecorIDs.Count < 1)
                return false;

            string FactoryFilter = string.Empty;

            if (FactoryID > 0)
                FactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            int[] IDs = DecorIDs.OfType<int>().ToArray();

            string SelectionCommand = "SELECT * FROM DecorOrders WHERE ProductID IN (" + string.Join(",", IDs) + ")" +
                    " AND MainOrderID = " + MainOrderID + FactoryFilter;

            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private bool GetSimpleFronts(int MainOrderID, int FactoryID)
        {
            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE FrontConfigID IN " +
                "(SELECT FrontConfigID FROM infiniu2_catalog.dbo.FrontsConfig WHERE FactoryID=" +
                FactoryID + ") AND Width <> -1 AND FrontID NOT IN" +
                " (3680,3681,3682,3683,3684,3685) AND MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return (FrontsOrdersDataTable.Rows.Count > 0);
        }

        private bool GetArchDecor(int MainOrderID, int FactoryID)
        {
            DecorOrdersDataTable.Clear();

            string FactoryFilter = string.Empty;

            if (FactoryID > 0)
                FactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            //Арка, бутылочница, накладка на ящик, полка = 24, полочница, петли = 19, ручки, ручки УКВ, стекло, решетка пластик, решетка пп 45, решетка пп 90
            string SelectionCommand = "SELECT * FROM DecorOrders WHERE ProductID NOT IN (31, 4, 32, 18, 27, 28, 26, 10, 11, 12)" +
                    " AND MainOrderID = " + MainOrderID + FactoryFilter;

            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private decimal GetSquare(DataTable DT)
        {
            decimal S = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    S += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return S;
        }

        private int GetCount(DataTable DT)
        {
            int S = 0;

            foreach (DataRow Row in DT.Rows)
            {
                S += Convert.ToInt32(Row["Count"]);
            }

            return S;
        }

        public void CreateReport(int[] MainOrders, int FactoryID,
            bool Curved, bool Aluminium, bool Glass, bool Hands, ArrayList DecorIDs)
        {
            Array.Sort(MainOrders);

            string ClientName = string.Empty;
            string DispatchDate = string.Empty;
            string DocNumber = string.Empty;
            string Notes = string.Empty;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            int RowIndex = 0;

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

            bool Fronts = false;
            bool Decor = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetCurvedAndAluminiumFronts(MainOrders[i], FactoryID, Curved, Aluminium))
                {
                    Fronts = true;
                    break;
                }
            }

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetSomeDecor(MainOrders[i], FactoryID, DecorIDs))
                {
                    Decor = true;
                    break;
                }
            }

            if (Fronts || Decor)
            {
                HSSFSheet sheet1 = hssfworkbook.CreateSheet("Кухни");
                sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

                sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
                sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
                sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
                sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

                sheet1.SetColumnWidth(0, 18 * 256);
                sheet1.SetColumnWidth(1, 15 * 256);
                sheet1.SetColumnWidth(2, 9 * 256);
                sheet1.SetColumnWidth(3, 15 * 256);
                sheet1.SetColumnWidth(4, 15 * 256);
                sheet1.SetColumnWidth(5, 12 * 256);
                sheet1.SetColumnWidth(6, 5 * 256);
                sheet1.SetColumnWidth(7, 5 * 256);
                sheet1.SetColumnWidth(8, 5 * 256);
                sheet1.SetColumnWidth(9, 7 * 256);

                bool DoNotDispatch = false;

                #region Кухни

                for (int i = 0; i < MainOrders.Count(); i++)
                {
                    decimal Square = 0;
                    int FrontsCount = 0;

                    DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                    if (DoNotDispatch)
                        continue;

                    #region Fronts

                    if (GetCurvedAndAluminiumFronts(MainOrders[i], FactoryID, Curved, Aluminium))
                    {
                        FillFrontsByMainOrder(Aluminium);

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                        {
                            int DisplayIndex = 0;
                            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                            cell4.CellStyle = HeaderStyle;
                            HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                            cell5.CellStyle = HeaderStyle;
                            HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                            cell6.CellStyle = HeaderStyle;
                            HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                            cell7.CellStyle = HeaderStyle;
                            HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                            cell8.CellStyle = HeaderStyle;

                            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                            cell9.CellStyle = HeaderStyle;
                            HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                            cell10.CellStyle = HeaderStyle;
                            HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                            cell11.CellStyle = HeaderStyle;
                            HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                            cell12.CellStyle = HeaderStyle;
                            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                            cell13.CellStyle = HeaderStyle;
                            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                            cell14.CellStyle = HeaderStyle;

                            //RowIndex++;

                            Square = GetSquare(FrontsResultDataTable);
                            FrontsCount = GetCount(FrontsResultDataTable);
                        }

                        //вывод заказов фасадов
                        for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                        {
                            if (FrontsResultDataTable.Rows.Count == 0)
                                break;

                            for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }
                            RowIndex++;
                        }

                        HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                        cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                        cellStyle1.SetFont(PackNumberFont);

                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(++RowIndex), 0, "Итого: ");
                        cell18.CellStyle = cellStyle1;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                        cell19.CellStyle = cellStyle1;

                        if (Square > 0)
                        {
                            HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                                Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                            cell20.CellStyle = cellStyle1;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                            RowIndex++;

                    }

                    #endregion

                    #region Decor

                    if (GetSomeDecor(MainOrders[i], FactoryID, DecorIDs))
                    {
                        FillDecorByMainOrder();

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        //декор
                        HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Название");
                        cell15.CellStyle = HeaderStyle;
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Цвет");
                        cell17.CellStyle = HeaderStyle;
                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Длина\\Высота");
                        cell18.CellStyle = HeaderStyle;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Шир.");
                        cell19.CellStyle = HeaderStyle;
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Кол.");
                        cell20.CellStyle = HeaderStyle;
                        HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 5, "Прим.");
                        cell21.CellStyle = HeaderStyle;

                        RowIndex++;

                        for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                        {
                            if (DecorResultDataTable[c].Rows.Count == 0)
                                continue;

                            //вывод заказов декора в excel
                            for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                            {
                                for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                                {
                                    int ColumnIndex = y;

                                    //if (y == 0)
                                    //{
                                    //    ColumnIndex = y;
                                    //}
                                    //else
                                    //{
                                    //    ColumnIndex = y + 1;
                                    //}

                                    Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                    //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                    if (t.Name == "Decimal")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                        continue;
                                    }
                                    if (t.Name == "Int32")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                    if (t.Name == "String" || t.Name == "DBNull")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                }

                                RowIndex++;
                            }
                            RowIndex++;

                        }
                    }
                    #endregion
                }

                #endregion
            }

            if (Curved)
            {
                HSSFSheet sheet2 = hssfworkbook.CreateSheet("Гнутые");
                sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;

                sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
                sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
                sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
                sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

                sheet2.SetColumnWidth(0, 10 * 256);
                sheet2.SetColumnWidth(1, 8 * 256);
                sheet2.SetColumnWidth(2, 15 * 256);
                sheet2.SetColumnWidth(3, 13 * 256);
                sheet2.SetColumnWidth(4, 15 * 256);
                sheet2.SetColumnWidth(5, 12 * 256);

                CurvedExcelSheet(ref hssfworkbook, ref sheet2, HeaderStyle, SimpleFont, SimpleCellStyle,
                    PackNumberFont, TempStyle, MainOrders);
            }

            if (Aluminium || Glass || Hands)
            {
                HSSFSheet sheet2 = hssfworkbook.CreateSheet("Алюминий");
                sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;

                sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
                sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
                sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
                sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

                sheet2.SetColumnWidth(0, 10 * 256);
                sheet2.SetColumnWidth(1, 13 * 256);
                sheet2.SetColumnWidth(2, 8 * 256);
                sheet2.SetColumnWidth(3, 13 * 256);
                sheet2.SetColumnWidth(4, 12 * 256);
                sheet2.SetColumnWidth(5, 12 * 256);
                sheet2.SetColumnWidth(6, 5 * 256);
                sheet2.SetColumnWidth(7, 5 * 256);
                sheet2.SetColumnWidth(8, 5 * 256);
                sheet2.SetColumnWidth(9, 7 * 256);

                RowIndex = 0;

                HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet2.CreateRow(RowIndex++), 3, "Утверждаю...............");
                ConfirmCell.CellStyle = TempStyle;
                HSSFCell DispatchDateCell = HSSFCellUtil.CreateCell(sheet2.CreateRow(RowIndex++), 0, "Дата отгрузки: " + GetDispatchDateByMainOrder(MainOrders[0]));
                DispatchDateCell.CellStyle = TempStyle;

                if (Aluminium)
                    RowIndex = AluminiumExcelSheet(ref hssfworkbook, ref sheet2, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, RowIndex);
                if (Glass)
                    RowIndex = GlassExcelSheet(ref hssfworkbook, ref sheet2, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, RowIndex);
                if (Hands)
                    RowIndex = HandsExcelSheet(ref hssfworkbook, ref sheet2, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, RowIndex);
            }

            ClientName = ClientName.Replace('/', '-');
            ClientName = ClientName.Replace('\"', '\'');

            DocNumber = DocNumber.Replace('/', '-');
            DocNumber = DocNumber.Replace('\"', '\'');

            string FileName = string.Empty;
            //string ReportFilePath = string.Empty;

            if (Curved)
                FileName += " Гнутые";
            if (Aluminium)
                FileName += " Алюминий";
            if (DecorIDs.Count > 0)
                FileName += " Декор";

            //ReportFilePath = ReadReportFilePath("ZOVOrdersPrintReportPath.config");
            //FileInfo file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + ".xls");

            //int j = 1;
            //while (file.Exists == true)
            //{
            //    file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + "(" + j++ + ").xls");
            //}

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            if (DispatchDate.Length > 0)
                DispatchDate += " ";
            FileInfo file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void NotInProductionReport(int[] MainOrders, int FactoryID)
        {
            Array.Sort(MainOrders);

            string ClientName = string.Empty;
            string DispatchDate = string.Empty;
            string DocNumber = string.Empty;
            string Notes = string.Empty;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Кухни");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 1;

            int TopRowFront = 1;
            int BottomRowFront = 1;

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

            bool DoNotDispatch = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                decimal Square = 0;
                int FrontsCount = 0;

                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                if (DoNotDispatch)
                    continue;

                #region Fronts

                if (GetSimpleFronts(MainOrders[i], FactoryID))
                {
                    FillFrontsByMainOrder(false);

                    sheet1.SetColumnWidth(0, 18 * 256);
                    sheet1.SetColumnWidth(1, 15 * 256);
                    sheet1.SetColumnWidth(2, 9 * 256);
                    sheet1.SetColumnWidth(3, 15 * 256);
                    sheet1.SetColumnWidth(4, 15 * 256);
                    sheet1.SetColumnWidth(5, 12 * 256);
                    sheet1.SetColumnWidth(6, 5 * 256);
                    sheet1.SetColumnWidth(7, 5 * 256);
                    sheet1.SetColumnWidth(8, 5 * 256);
                    sheet1.SetColumnWidth(9, 7 * 256);

                    ClientName = GetClientNameByMainOrder(MainOrders[i]);
                    DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                    DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                    Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                    HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                    ClientCell.CellStyle = ClientNameStyle;

                    HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                    cell1.CellStyle = MainStyle;

                    if (DispatchDate.Length > 0)
                    {
                        HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                        cell2.CellStyle = MainStyle;
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                        cell3.CellStyle = MainStyle;
                    }

                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        int DisplayIndex = 0;
                        HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                        cell4.CellStyle = HeaderStyle;
                        HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                        cell5.CellStyle = HeaderStyle;
                        HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                        cell6.CellStyle = HeaderStyle;
                        HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                        cell7.CellStyle = HeaderStyle;
                        HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                        cell8.CellStyle = HeaderStyle;

                        HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                        cell9.CellStyle = HeaderStyle;
                        HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                        cell10.CellStyle = HeaderStyle;
                        HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell11.CellStyle = HeaderStyle;
                        HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell12.CellStyle = HeaderStyle;
                        HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                        cell13.CellStyle = HeaderStyle;
                        HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell14.CellStyle = HeaderStyle;


                        Square = GetSquare(FrontsResultDataTable);
                        FrontsCount = GetCount(FrontsResultDataTable);
                    }

                    TopRowFront = RowIndex;
                    BottomRowFront = FrontsResultDataTable.Rows.Count + RowIndex;

                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                        {
                            Type t = FrontsResultDataTable.Rows[x][y].GetType();

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }
                        }
                        RowIndex++;
                    }


                    HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                    cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                    cellStyle1.SetFont(PackNumberFont);

                    HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(++RowIndex), 0, "Итого: ");
                    cell18.CellStyle = cellStyle1;
                    HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                    cell19.CellStyle = cellStyle1;

                    if (Square > 0)
                    {
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                            Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                        cell20.CellStyle = cellStyle1;
                    }

                    if (FrontsResultDataTable.Rows.Count != 0)
                        RowIndex++;

                }

                #endregion
            }

            ClientName = ClientName.Replace('/', '-');
            ClientName = ClientName.Replace('\"', '\'');

            string FileName = "Не в производстве (прямые)";
            //string ReportFilePath = string.Empty;

            //ReportFilePath = ReadReportFilePath("ZOVOrdersPrintReportPath.config");
            //FileInfo file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + ".xls");

            //int j = 1;
            //while (file.Exists == true)
            //{
            //    file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + "(" + j++ + ").xls");
            //}

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            if (DispatchDate.Length > 0)
                DispatchDate += " ";
            FileInfo file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void MainOrderReport(int[] MainOrders, int FactoryID)
        {
            Array.Sort(MainOrders);

            string ClientName = string.Empty;
            string DispatchDate = string.Empty;
            string DocNumber = string.Empty;
            string Notes = string.Empty;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Кухни");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 1;

            int TopRowFront = 1;
            int BottomRowFront = 1;

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

            bool DoNotDispatch = false;

            if (FactoryID == 0)
            {
                //HSSFCell ProfilCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "ЗОВ-Профиль");
                //ProfilCell.CellStyle = MainStyle;

                for (int i = 0; i < MainOrders.Count(); i++)
                {
                    decimal Square = 0;
                    int FrontsCount = 0;

                    DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                    if (DoNotDispatch)
                        continue;

                    #region Fronts

                    if (FilterFrontsOrders(MainOrders[i], 1))
                    {
                        FillFrontsByMainOrder(false);

                        sheet1.SetColumnWidth(0, 18 * 256);
                        sheet1.SetColumnWidth(1, 15 * 256);
                        sheet1.SetColumnWidth(2, 9 * 256);
                        sheet1.SetColumnWidth(3, 15 * 256);
                        sheet1.SetColumnWidth(4, 15 * 256);
                        sheet1.SetColumnWidth(5, 12 * 256);
                        sheet1.SetColumnWidth(6, 5 * 256);
                        sheet1.SetColumnWidth(7, 5 * 256);
                        sheet1.SetColumnWidth(8, 5 * 256);
                        sheet1.SetColumnWidth(9, 7 * 256);

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                        {
                            int DisplayIndex = 0;
                            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                            cell4.CellStyle = HeaderStyle;
                            HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                            cell5.CellStyle = HeaderStyle;
                            HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                            cell6.CellStyle = HeaderStyle;
                            HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                            cell7.CellStyle = HeaderStyle;
                            HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                            cell8.CellStyle = HeaderStyle;

                            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                            cell9.CellStyle = HeaderStyle;
                            HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                            cell10.CellStyle = HeaderStyle;
                            HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                            cell11.CellStyle = HeaderStyle;
                            HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                            cell12.CellStyle = HeaderStyle;
                            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                            cell13.CellStyle = HeaderStyle;
                            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                            cell14.CellStyle = HeaderStyle;

                            //RowIndex++;

                            Square = GetSquare(FrontsResultDataTable);
                            FrontsCount = GetCount(FrontsResultDataTable);
                        }

                        TopRowFront = RowIndex;
                        BottomRowFront = FrontsResultDataTable.Rows.Count + RowIndex;

                        //вывод заказов фасадов
                        for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                        {
                            if (FrontsResultDataTable.Rows.Count == 0)
                                break;

                            for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }
                            RowIndex++;
                        }


                        HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                        cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                        cellStyle1.SetFont(PackNumberFont);

                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(++RowIndex), 0, "Итого: ");
                        cell18.CellStyle = cellStyle1;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                        cell19.CellStyle = cellStyle1;

                        if (Square > 0)
                        {
                            HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                                Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                            cell20.CellStyle = cellStyle1;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                            RowIndex++;

                    }

                    #endregion

                    #region Decor

                    if (FilterDecorOrders(MainOrders[i], 1))
                    {
                        FillDecorByMainOrder();

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        //декор
                        int DisplayIndex = 0;
                        HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                        cell15.CellStyle = HeaderStyle;
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                        cell17.CellStyle = HeaderStyle;
                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                        cell18.CellStyle = HeaderStyle;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell19.CellStyle = HeaderStyle;
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell20.CellStyle = HeaderStyle;
                        HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell21.CellStyle = HeaderStyle;

                        RowIndex++;

                        for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                        {
                            if (DecorResultDataTable[c].Rows.Count == 0)
                                continue;

                            //вывод заказов декора в excel
                            for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                            {
                                for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                                {
                                    int ColumnIndex = y;

                                    //if (y == 0)
                                    //{
                                    //    ColumnIndex = y;
                                    //}
                                    //else
                                    //{
                                    //    ColumnIndex = y + 1;
                                    //}

                                    Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                    //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                    if (t.Name == "Decimal")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                        continue;
                                    }
                                    if (t.Name == "Int32")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                    if (t.Name == "String" || t.Name == "DBNull")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                }

                                RowIndex++;
                            }
                            RowIndex++;

                        }
                    }
                    #endregion
                }

                //HSSFCell TPSCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "ЗОВ-ТПС");
                //TPSCell.CellStyle = MainStyle;

                for (int i = 0; i < MainOrders.Count(); i++)
                {
                    decimal Square = 0;
                    int FrontsCount = 0;

                    DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                    if (DoNotDispatch)
                        continue;

                    #region Fronts

                    if (FilterFrontsOrders(MainOrders[i], 2))
                    {
                        FillFrontsByMainOrder(false);

                        sheet1.SetColumnWidth(0, 18 * 256);
                        sheet1.SetColumnWidth(1, 15 * 256);
                        sheet1.SetColumnWidth(2, 9 * 256);
                        sheet1.SetColumnWidth(3, 15 * 256);
                        sheet1.SetColumnWidth(4, 15 * 256);
                        sheet1.SetColumnWidth(5, 12 * 256);
                        sheet1.SetColumnWidth(6, 5 * 256);
                        sheet1.SetColumnWidth(7, 5 * 256);
                        sheet1.SetColumnWidth(8, 5 * 256);
                        sheet1.SetColumnWidth(9, 7 * 256);

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                        {
                            int DisplayIndex = 0;
                            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                            cell4.CellStyle = HeaderStyle;
                            HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                            cell5.CellStyle = HeaderStyle;
                            HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                            cell6.CellStyle = HeaderStyle;
                            HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                            cell7.CellStyle = HeaderStyle;
                            HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                            cell8.CellStyle = HeaderStyle;

                            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                            cell9.CellStyle = HeaderStyle;
                            HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                            cell10.CellStyle = HeaderStyle;
                            HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                            cell11.CellStyle = HeaderStyle;
                            HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                            cell12.CellStyle = HeaderStyle;
                            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                            cell13.CellStyle = HeaderStyle;
                            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                            cell14.CellStyle = HeaderStyle;

                            //RowIndex++;

                            Square = GetSquare(FrontsResultDataTable);
                            FrontsCount = GetCount(FrontsResultDataTable);
                        }

                        TopRowFront = RowIndex;
                        BottomRowFront = FrontsResultDataTable.Rows.Count + RowIndex;

                        //вывод заказов фасадов
                        for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                        {
                            if (FrontsResultDataTable.Rows.Count == 0)
                                break;

                            for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }
                            RowIndex++;
                        }


                        HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                        cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                        cellStyle1.SetFont(PackNumberFont);

                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(++RowIndex), 0, "Итого: ");
                        cell18.CellStyle = cellStyle1;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                        cell19.CellStyle = cellStyle1;

                        if (Square > 0)
                        {
                            HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                                Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                            cell20.CellStyle = cellStyle1;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                            RowIndex++;

                    }

                    #endregion

                    #region Decor

                    if (FilterDecorOrders(MainOrders[i], 2))
                    {
                        FillDecorByMainOrder();

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        //декор
                        int DisplayIndex = 0;
                        HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                        cell15.CellStyle = HeaderStyle;
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                        cell17.CellStyle = HeaderStyle;
                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                        cell18.CellStyle = HeaderStyle;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell19.CellStyle = HeaderStyle;
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell20.CellStyle = HeaderStyle;
                        HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell21.CellStyle = HeaderStyle;

                        RowIndex++;

                        for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                        {
                            if (DecorResultDataTable[c].Rows.Count == 0)
                                continue;

                            //вывод заказов декора в excel
                            for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                            {
                                for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                                {
                                    int ColumnIndex = y;

                                    //if (y == 0)
                                    //{
                                    //    ColumnIndex = y;
                                    //}
                                    //else
                                    //{
                                    //    ColumnIndex = y + 1;
                                    //}

                                    Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                    //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                    if (t.Name == "Decimal")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                        continue;
                                    }
                                    if (t.Name == "Int32")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                    if (t.Name == "String" || t.Name == "DBNull")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                }

                                RowIndex++;
                            }
                            RowIndex++;

                        }
                    }
                    #endregion
                }

            }
            else
            {

                for (int i = 0; i < MainOrders.Count(); i++)
                {
                    decimal Square = 0;
                    int FrontsCount = 0;

                    DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                    if (DoNotDispatch)
                        continue;

                    #region Fronts

                    if (FilterFrontsOrders(MainOrders[i], FactoryID))
                    {
                        FillFrontsByMainOrder(false);

                        sheet1.SetColumnWidth(0, 18 * 256);
                        sheet1.SetColumnWidth(1, 15 * 256);
                        sheet1.SetColumnWidth(2, 9 * 256);
                        sheet1.SetColumnWidth(3, 15 * 256);
                        sheet1.SetColumnWidth(4, 15 * 256);
                        sheet1.SetColumnWidth(5, 12 * 256);
                        sheet1.SetColumnWidth(6, 5 * 256);
                        sheet1.SetColumnWidth(7, 5 * 256);
                        sheet1.SetColumnWidth(8, 5 * 256);
                        sheet1.SetColumnWidth(9, 7 * 256);

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                        {
                            int DisplayIndex = 0;
                            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                            cell4.CellStyle = HeaderStyle;
                            HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                            cell5.CellStyle = HeaderStyle;
                            HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                            cell6.CellStyle = HeaderStyle;
                            HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                            cell7.CellStyle = HeaderStyle;
                            HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                            cell8.CellStyle = HeaderStyle;

                            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                            cell9.CellStyle = HeaderStyle;
                            HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                            cell10.CellStyle = HeaderStyle;
                            HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                            cell11.CellStyle = HeaderStyle;
                            HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                            cell12.CellStyle = HeaderStyle;
                            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                            cell13.CellStyle = HeaderStyle;
                            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                            cell14.CellStyle = HeaderStyle;

                            //RowIndex++;

                            Square = GetSquare(FrontsResultDataTable);
                            FrontsCount = GetCount(FrontsResultDataTable);
                        }

                        TopRowFront = RowIndex;
                        BottomRowFront = FrontsResultDataTable.Rows.Count + RowIndex;

                        //вывод заказов фасадов
                        for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                        {
                            if (FrontsResultDataTable.Rows.Count == 0)
                                break;

                            for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }
                            RowIndex++;
                        }


                        HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                        cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                        cellStyle1.SetFont(PackNumberFont);

                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(++RowIndex), 0, "Итого: ");
                        cell18.CellStyle = cellStyle1;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                        cell19.CellStyle = cellStyle1;

                        if (Square > 0)
                        {
                            HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                                Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                            cell20.CellStyle = cellStyle1;
                        }

                        if (FrontsResultDataTable.Rows.Count != 0)
                            RowIndex++;

                    }

                    #endregion

                    #region Decor

                    if (FilterDecorOrders(MainOrders[i], FactoryID))
                    {
                        FillDecorByMainOrder();

                        ClientName = GetClientNameByMainOrder(MainOrders[i]);
                        DocNumber = GetOrderNumberByMainOrder(MainOrders[i]);
                        DispatchDate = GetDispatchDateByMainOrder(MainOrders[i]);
                        Notes = GetMainOrderNotesByMainOrder(MainOrders[i]);

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, DocNumber);
                        cell1.CellStyle = MainStyle;

                        if (DispatchDate.Length > 0)
                        {
                            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                            cell2.CellStyle = MainStyle;
                        }

                        if (Notes.Length > 0)
                        {
                            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + Notes);
                            cell3.CellStyle = MainStyle;
                        }

                        //декор
                        int DisplayIndex = 0;
                        HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                        cell15.CellStyle = HeaderStyle;
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                        cell17.CellStyle = HeaderStyle;
                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                        cell18.CellStyle = HeaderStyle;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell19.CellStyle = HeaderStyle;
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell20.CellStyle = HeaderStyle;
                        HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell21.CellStyle = HeaderStyle;

                        RowIndex++;

                        for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                        {
                            if (DecorResultDataTable[c].Rows.Count == 0)
                                continue;

                            //вывод заказов декора в excel
                            for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                            {
                                for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                                {
                                    int ColumnIndex = y;

                                    //if (y == 0)
                                    //{
                                    //    ColumnIndex = y;
                                    //}
                                    //else
                                    //{
                                    //    ColumnIndex = y + 1;
                                    //}

                                    Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                    //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                    if (t.Name == "Decimal")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                        continue;
                                    }
                                    if (t.Name == "Int32")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                    if (t.Name == "String" || t.Name == "DBNull")
                                    {
                                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                }

                                RowIndex++;
                            }
                            RowIndex++;

                        }
                    }
                    #endregion
                }
            }

            ClientName = ClientName.Replace('/', '-');
            ClientName = ClientName.Replace('\"', '\'');

            string FileName = "Не в производстве (прямые)";
            //string ReportFilePath = string.Empty;

            //ReportFilePath = ReadReportFilePath("ZOVOrdersPrintReportPath.config");
            //FileInfo file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + ".xls");

            //int j = 1;
            //while (file.Exists == true)
            //{
            //    file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + "(" + j++ + ").xls");
            //}

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            if (DispatchDate.Length > 0)
                DispatchDate += " ";
            FileInfo file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void DepersonalizedReport(int[] MainOrders, int FactoryID)
        {
            Array.Sort(MainOrders);

            string ClientName = string.Empty;
            string DispatchDate = string.Empty;
            string DocNumber = string.Empty;
            string Notes = string.Empty;

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

            FrontsDepersonalized(ref hssfworkbook, HeaderStyle, SimpleFont, SimpleCellStyle,
                PackNumberFont, TempStyle, MainOrders, FactoryID);

            DecorDepersonalized(ref hssfworkbook, HeaderStyle, SimpleFont, SimpleCellStyle,
                PackNumberFont, TempStyle, MainOrders, FactoryID);

            string FileName = "Обезличенные ЗОВ-Профиль";
            if (FactoryID == 2)
                FileName = "Обезличенные ЗОВ-ТПС";
            //string ReportFilePath = string.Empty;

            //ReportFilePath = ReadReportFilePath("ZOVOrdersPrintReportPath.config");
            //FileInfo file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + ".xls");

            //int j = 1;
            //while (file.Exists == true)
            //{
            //    file = new FileInfo(ReportFilePath + DispatchDate + " " + FileName + "(" + j++ + ").xls");
            //}

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            if (DispatchDate.Length > 0)
                DispatchDate += " ";
            FileInfo file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + DispatchDate + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        #region По подзаказам

        private int FrontsDepersonalized(ref HSSFWorkbook hssfworkbook,
           HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
           HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int[] MainOrders, int FactoryID)
        {


            bool DoNotDispatch = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                if (DoNotDispatch)
                    continue;
            }

            int RowIndex = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FactoryID=" + FactoryID +
                " AND MainOrderID IN (" + string.Join(",", MainOrders) + ") ORDER BY FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, Height",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                FrontsOrdersDataTable.Clear();
                DA.Fill(FrontsOrdersDataTable);
            }

            if (FrontsOrdersDataTable.Rows.Count < 1)
                return RowIndex;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Фасады");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);

            FillFrontsDepersonalized(true);

            int FrontsCount = 0;

            if (FrontsResultDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;

                RowIndex++;

                FrontsCount += GetCount(FrontsResultDataTable);
            }

            //вывод заказов фасадов
            for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
            {
                for (int y = 0; y < FrontsResultDataTable.Columns.Count - 2; y++)
                {
                    Type t = FrontsResultDataTable.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                        cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;
            }

            HSSFCellStyle cellStyle2 = hssfworkbook.CreateCellStyle();
            cellStyle2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle2.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle2;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle2;

            return ++RowIndex;
        }

        private int DecorDepersonalized(ref HSSFWorkbook hssfworkbook,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int[] MainOrders, int FactoryID)
        {
            int RowIndex = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                " WHERE FactoryID=" + FactoryID +
                " AND MainOrderID IN ( " + string.Join(",", MainOrders) + ")  ORDER BY ProductID, DecorID, ColorID, Height, Length, Width",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DecorOrdersDataTable.Clear();
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count == 0)
                return RowIndex;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Декор");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            FillDecorDepersonalized();

            int DecorCount = 0;

            if (DecorOrdersDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;
            }

            //вывод заказов фасадов
            for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
            {
                if (DecorResultDataTable[c].Rows.Count == 0)
                    continue;

                DecorCount = GetCount(DecorResultDataTable[c]);

                //вывод заказов декора в excel
                for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                {
                    for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                    {
                        int ColumnIndex = y;

                        //if (y == 0)
                        //{
                        //    ColumnIndex = y;
                        //}
                        //else
                        //{
                        //    ColumnIndex = y + 1;
                        //}

                        Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                        //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                    }

                    RowIndex++;
                }

                HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                cellStyle1.SetFont(PackNumberFont);

                HSSFCell cell22 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                cell22.CellStyle = cellStyle1;
                HSSFCell cell23 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, DecorCount + " шт.");
                cell23.CellStyle = cellStyle1;

                RowIndex++;

            }

            return ++RowIndex;
        }

        private int CurvedExcelSheet(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int[] iMainOrders)
        {
            int RowIndex = 0;

            bool DoNotDispatch = false;
            ArrayList array = new ArrayList();

            for (int i = 0; i < iMainOrders.Count(); i++)
            {
                DoNotDispatch = IsDoNotDispatch(iMainOrders[i]);

                if (!DoNotDispatch)
                {
                    array.Add(iMainOrders[i]);
                    continue;
                }
            }

            int[] MainOrders = array.OfType<int>().ToArray();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE Width = -1 AND MainOrderID IN ( " + string.Join(",", MainOrders) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                FrontsOrdersDataTable.Clear();
                DA.Fill(FrontsOrdersDataTable);
            }

            if (FrontsOrdersDataTable.Rows.Count < 1)
                return RowIndex;

            FillFrontsDepersonalized(false);

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 3, "Утверждаю...............");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell DispatchDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + GetDispatchDateByMainOrder(MainOrders[0]));
            DispatchDateCell.CellStyle = TempStyle;

            int FrontsCount = 0;

            if (FrontsResultDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;

                RowIndex++;

                FrontsCount = GetCount(FrontsResultDataTable);
            }

            //вывод заказов фасадов
            for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
            {
                for (int y = 0; y < FrontsResultDataTable.Columns.Count - 2; y++)
                {
                    Type t = FrontsResultDataTable.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                        cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            return ++RowIndex;
        }

        private int AluminiumExcelSheet(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int[] iMainOrders, int RowIndex)
        {
            bool DoNotDispatch = false;
            ArrayList array = new ArrayList();

            for (int i = 0; i < iMainOrders.Count(); i++)
            {
                DoNotDispatch = IsDoNotDispatch(iMainOrders[i]);

                if (!DoNotDispatch)
                {
                    array.Add(iMainOrders[i]);
                    continue;
                }
            }

            int[] MainOrders = array.OfType<int>().ToArray();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontID IN (3680,3681,3682,3683,3684,3685)" +
                " AND MainOrderID IN ( " + string.Join(",", MainOrders) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                FrontsOrdersDataTable.Clear();
                DA.Fill(FrontsOrdersDataTable);
            }

            if (FrontsOrdersDataTable.Rows.Count < 1)
                return RowIndex;

            FillFrontsDepersonalized(true);

            int FrontsCount = 0;
            decimal Square = 0;

            if (FrontsResultDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Сверление");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ручки");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell13.CellStyle = HeaderStyle;

                RowIndex++;

                Square = GetSquare(FrontsResultDataTable);
                FrontsCount = GetCount(FrontsResultDataTable);
            }

            //вывод заказов фасадов
            for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
            {
                for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                {
                    Type t = FrontsResultDataTable.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                        cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            if (Square > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                    Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                cell20.CellStyle = cellStyle1;
            }

            RowIndex++;

            return ++RowIndex;
        }

        private int GlassExcelSheet(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int[] MainOrders, int RowIndex)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                " WHERE ProductID = 26" +
                " AND MainOrderID IN ( " + string.Join(",", MainOrders) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DecorOrdersDataTable.Clear();
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count == 0)
                return RowIndex;

            FillDecorDepersonalized();

            int DecorCount = 0;

            if (DecorOrdersDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;
            }

            //вывод заказов фасадов
            for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
            {
                if (DecorResultDataTable[c].Rows.Count == 0)
                    continue;

                DecorCount = GetCount(DecorResultDataTable[c]);

                //вывод заказов декора в excel
                for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                {
                    for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                    {
                        int ColumnIndex = y;

                        //if (y == 0)
                        //{
                        //    ColumnIndex = y;
                        //}
                        //else
                        //{
                        //    ColumnIndex = y + 1;
                        //}

                        Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                        //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;
                        string s = DecorResultDataTable[c].Rows[x][y].ToString();
                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                    }

                    RowIndex++;
                }
            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell22 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell22.CellStyle = cellStyle1;
            HSSFCell cell23 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, DecorCount + " шт.");
            cell23.CellStyle = cellStyle1;

            RowIndex++;

            return ++RowIndex;
        }

        private int HandsExcelSheet(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int[] MainOrders, int RowIndex)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                " WHERE (ProductID = 27 OR ProductID = 28)" +
                " AND MainOrderID IN ( " + string.Join(",", MainOrders) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DecorOrdersDataTable.Clear();
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count == 0)
                return RowIndex;

            FillDecorDepersonalized();

            int DecorCount = 0;

            if (DecorOrdersDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;
            }

            //вывод заказов фасадов
            for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
            {
                if (DecorResultDataTable[c].Rows.Count == 0)
                    continue;

                DecorCount = GetCount(DecorResultDataTable[c]);

                //вывод заказов декора в excel
                for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                {
                    for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                    {
                        int ColumnIndex = y;

                        //if (y == 0)
                        //{
                        //    ColumnIndex = y;
                        //}
                        //else
                        //{
                        //    ColumnIndex = y + 1;
                        //}

                        Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                        //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                    }

                    RowIndex++;
                }
            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell22 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell22.CellStyle = cellStyle1;
            HSSFCell cell23 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, DecorCount + " шт.");
            cell23.CellStyle = cellStyle1;

            RowIndex++;

            return ++RowIndex;
        }
        #endregion

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



    public class PaymentWeeks
    {
        PercentageDataGrid PaymentWeeksDataGrid = null;
        PercentageDataGrid TempPaymentDetailDataGrid = null;
        PercentageDataGrid PaymentDetailDataGrid = null;

        SqlDataAdapter PaymentWeeksDataAdapter = null;
        SqlCommandBuilder PaymentWeeksCommandBuilder = null;
        DataTable PaymentWeeksDataTable = null;
        DataTable ClientsDataTable = null;
        DataTable DebtsDataTable = null;

        BindingSource PaymentWeeksBindingSource = null;
        BindingSource PaymentDetailBindingSource = null;
        BindingSource TempPaymentDetailBindingSource = null;
        BindingSource DebtTypesBindingSource = null;

        SqlDataAdapter MegaOrdersDA = null;
        SqlCommandBuilder MegaOrdersCB = null;
        DataTable MegaOrdersDataTable = null;

        DataTable PaymentDetailDataTable = null;
        SqlDataAdapter PaymentDetailDA = null;
        SqlCommandBuilder PaymentDetailCB = null;

        SqlDataAdapter TempPaymentDetailDA = null;
        SqlCommandBuilder TempPaymentDetailCB = null;
        DataTable TempPaymentDetailDataTable = null;

        SqlDataAdapter MainOrdersDA = null;
        SqlCommandBuilder MainOrdersCB = null;
        DataTable MainOrdersDataTable = null;

        private DataGridViewComboBoxColumn TempDebtTypeColumn = null;
        private DataGridViewComboBoxColumn DebtTypeColumn = null;

        public PaymentWeeks(ref PercentageDataGrid tPaymentWeeksDataGrid,
                            ref PercentageDataGrid tPaymentWeeksNewWeekOrdersDataGrid,
                            ref PercentageDataGrid tPaymentDetailDataGrid)
        {
            PaymentWeeksDataGrid = tPaymentWeeksDataGrid;
            TempPaymentDetailDataGrid = tPaymentWeeksNewWeekOrdersDataGrid;
            PaymentDetailDataGrid = tPaymentDetailDataGrid;

            Initialize();
        }

        private void Create()
        {
            PaymentWeeksDataTable = new DataTable();
            PaymentDetailDataTable = new DataTable();
            TempPaymentDetailDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();


            PaymentWeeksBindingSource = new BindingSource();
            PaymentDetailBindingSource = new BindingSource();
            TempPaymentDetailBindingSource = new BindingSource();
            DebtTypesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            PaymentWeeksDataAdapter = new SqlDataAdapter("SELECT * FROM PaymentWeeks ORDER BY DateFrom", ConnectionStrings.ZOVOrdersConnectionString);
            PaymentWeeksCommandBuilder = new SqlCommandBuilder(PaymentWeeksDataAdapter);
            PaymentWeeksDataAdapter.Fill(PaymentWeeksDataTable);

            PaymentDetailDA = new SqlDataAdapter("SELECT TOP 0 * FROM PaymentDetail", ConnectionStrings.ZOVOrdersConnectionString);
            PaymentDetailCB = new SqlCommandBuilder(PaymentDetailDA);
            PaymentDetailDA.Fill(PaymentDetailDataTable);

            TempPaymentDetailDA = new SqlDataAdapter("SELECT TOP 0 * FROM PaymentDetail", ConnectionStrings.ZOVOrdersConnectionString);
            TempPaymentDetailCB = new SqlCommandBuilder(TempPaymentDetailDA);
            TempPaymentDetailDA.Fill(TempPaymentDetailDataTable);

            MegaOrdersDA = new SqlDataAdapter("SELECT TOP 0 * FROM MegaOrders", ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersCB = new SqlCommandBuilder(MegaOrdersDA);


            MainOrdersDA = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders", ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersCB = new SqlCommandBuilder(MainOrdersDA);

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            DebtsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DebtTypes",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DebtsDataTable);
            }

        }

        private void Binding()
        {
            PaymentWeeksBindingSource.DataSource = PaymentWeeksDataTable;
            TempPaymentDetailBindingSource.DataSource = TempPaymentDetailDataTable;

            PaymentDetailBindingSource.DataSource = PaymentDetailDataTable;
            PaymentDetailDataGrid.DataSource = PaymentDetailBindingSource;

            PaymentWeeksDataGrid.DataSource = PaymentWeeksBindingSource;
            TempPaymentDetailDataGrid.DataSource = TempPaymentDetailBindingSource;

            DebtTypesBindingSource.DataSource = DebtsDataTable;
        }

        private void SetGrids()
        {
            foreach (DataGridViewColumn Column in PaymentWeeksDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            }

            PaymentWeeksDataGrid.Columns["PaymentWeekID"].Visible = false;

            PaymentWeeksDataGrid.Columns["DateFrom"].HeaderText = "Первое число\r\n     недели";
            PaymentWeeksDataGrid.Columns["DateTo"].HeaderText = "Последнее число\r\n        недели";
            PaymentWeeksDataGrid.Columns["TotalCost"].HeaderText = "Расчет,\r\n  евро";
            PaymentWeeksDataGrid.Columns["DispatchedCost"].HeaderText = "Отгружено,\r\n     евро";
            PaymentWeeksDataGrid.Columns["DispatchedDebtCost"].HeaderText = "Не отгружено,\r\n       евро";
            PaymentWeeksDataGrid.Columns["CalcDebtCost"].HeaderText = "Долги в расчете,\r\n         евро";
            PaymentWeeksDataGrid.Columns["CalcDefectsCost"].HeaderText = "Брак в расчете,\r\n        евро";
            PaymentWeeksDataGrid.Columns["CalcProductionErrorsCost"].HeaderText = "Ошибки пр-ва\r\nв расчете, евро";
            PaymentWeeksDataGrid.Columns["CalcZOVErrorsCost"].HeaderText = "  Ошибки ЗОВа\r\n в расчете, евро";
            PaymentWeeksDataGrid.Columns["WriteOffDebtCost"].HeaderText = "Долги по возвратам,\r\n             евро";
            PaymentWeeksDataGrid.Columns["WriteOffDefectsCost"].HeaderText = "Брак по возвратам,\r\n           евро";
            PaymentWeeksDataGrid.Columns["WriteOffProductionErrorsCost"].HeaderText = "    Ошибки пр-ва\r\nпо возвратам, евро";
            PaymentWeeksDataGrid.Columns["WriteOffZOVErrorsCost"].HeaderText = "     Ошибки ЗОВа\r\n по возвратам, евро";
            PaymentWeeksDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "Списание за образцы,\r\n             евро";
            PaymentWeeksDataGrid.Columns["TotalWriteOffCost"].HeaderText = "Списано по возвратам,\r\n             евро";
            PaymentWeeksDataGrid.Columns["TotalCalcWriteOffCost"].HeaderText = "Списано по расчету,\r\n            евро";
            PaymentWeeksDataGrid.Columns["ErrorWriteOffCost"].HeaderText = "Ошибочное списание,\r\n              евро";
            PaymentWeeksDataGrid.Columns["CompensationCost"].HeaderText = "Компенсация предыдущего,\r\n                 евро";
            PaymentWeeksDataGrid.Columns["ProfitCost"].HeaderText = "Итого,\r\n евро";


            TempPaymentDetailDataGrid.Columns["DispatchDate"].HeaderText = "Дата отгрузки";
            TempPaymentDetailDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            TempPaymentDetailDataGrid.Columns["DocNumber"].HeaderText = "№ документа";
            TempPaymentDetailDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "Списание за образцы";
            TempPaymentDetailDataGrid.Columns["DebtCost"].HeaderText = "Сумма долга";

            TempPaymentDetailDataGrid.Columns["PaymentDetailID"].Visible = false;
            TempPaymentDetailDataGrid.Columns["PaymentWeekID"].Visible = false;
            TempPaymentDetailDataGrid.Columns["DebtTypeID"].Visible = false;

            PaymentDetailDataGrid.Columns["DispatchDate"].HeaderText = "Дата отгрузки";
            PaymentDetailDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            PaymentDetailDataGrid.Columns["DocNumber"].HeaderText = "№ документа";
            PaymentDetailDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "Списание за образцы";
            PaymentDetailDataGrid.Columns["DebtCost"].HeaderText = "Сумма долга";

            PaymentDetailDataGrid.Columns["PaymentDetailID"].Visible = false;
            PaymentDetailDataGrid.Columns["PaymentWeekID"].Visible = false;
            PaymentDetailDataGrid.Columns["DebtTypeID"].Visible = false;

        }

        private string GetClientName(int ClientID)
        {
            return ClientsDataTable.Select("ClientID = " + ClientID)[0]["ClientName"].ToString();
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            SetGrids();
        }

        private void CreateColumns()
        {
            TempDebtTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DebtTypeColumn",
                HeaderText = "Тип долга",
                DataPropertyName = "DebtTypeID",
                DataSource = DebtTypesBindingSource,
                ValueMember = "DebtTypeID",
                DisplayMember = "DebtType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            DebtTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DebtTypeColumn",
                HeaderText = "Тип долга",
                DataPropertyName = "DebtTypeID",
                DataSource = DebtTypesBindingSource,
                ValueMember = "DebtTypeID",
                DisplayMember = "DebtType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TempPaymentDetailDataGrid.Columns.Add(TempDebtTypeColumn);
            PaymentDetailDataGrid.Columns.Add(DebtTypeColumn);
        }

        public void NewPaymentWeek()
        {
            TempPaymentDetailDataTable.Clear();
        }

        public void AddNewOrder(string DocNumber, decimal Samples, int DebtTypeID)
        {
            string DispatchDate = "";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE DocNumber = '" + DocNumber + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                    {
                        MessageBox.Show("Выбранного заказа не существует");
                        return;
                    }

                    using (SqlDataAdapter mDA = new SqlDataAdapter("SELECT * FROM MegaOrders WHERE MegaOrderID = " + DT.Rows[0]["MegaOrderID"],
                        ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        using (DataTable mDT = new DataTable())
                        {
                            if (mDA.Fill(mDT) == 0)
                            {
                                MessageBox.Show("Выбранного заказа не существует");
                                return;
                            }

                            DispatchDate = Convert.ToDateTime(mDT.Rows[0]["DispatchDate"]).ToShortDateString();
                        }
                    }

                    DataRow NewRow = TempPaymentDetailDataTable.NewRow();
                    NewRow["DispatchDate"] = DispatchDate;
                    NewRow["ClientName"] = GetClientName(Convert.ToInt32(DT.Rows[0]["ClientID"]));
                    NewRow["DocNumber"] = DT.Rows[0]["DocNumber"];
                    NewRow["SamplesWriteOffCost"] = Samples;
                    NewRow["DebtTypeID"] = DebtTypeID;

                    if (NewRow["DebtTypeID"].ToString() != "0")
                        NewRow["DebtCost"] = DT.Rows[0]["OrderCost"];
                    else
                        NewRow["DebtCost"] = 0;

                    TempPaymentDetailDataTable.Rows.Add(NewRow);

                }
            }
        }

        public void RemoveCurrentTempDoc()
        {
            if (TempPaymentDetailBindingSource.Count > 0)
            {
                TempPaymentDetailBindingSource.RemoveCurrent();
            }
        }

        public void Save(DateTime DateFrom, DateTime DateTo, decimal ErrorWriteOffCost = 0, decimal CompensationCost = 0)
        {
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PaymentDetail WHERE (PaymentWeekID = 118)",
            //    ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    DA.Fill(TempPaymentDetailDataTable);
            //}

            if (TempPaymentDetailBindingSource.Count == 0)
                return;

            string MainWhere = "";

            for (int i = 0; i < TempPaymentDetailDataTable.Rows.Count; i++)
            {
                if (TempPaymentDetailDataTable.Rows[i].RowState == DataRowState.Deleted)
                    continue;

                if (i == 0)
                    MainWhere = " WHERE DocNumber = '" + TempPaymentDetailDataTable.Rows[i]["DocNumber"].ToString() + "'";
                else
                    MainWhere += " OR DocNumber = '" + TempPaymentDetailDataTable.Rows[i]["DocNumber"].ToString() + "'";
            }

            MainOrdersDA.Dispose();
            MainOrdersCB.Dispose();

            MegaOrdersDA.Dispose();
            MegaOrdersCB.Dispose();

            MainOrdersDataTable.Clear();
            MegaOrdersDataTable.Clear();

            MainOrdersDA = new SqlDataAdapter("SELECT * FROM MainOrders" + MainWhere,
                ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersCB = new SqlCommandBuilder(MainOrdersDA);
            MainOrdersDA.Fill(MainOrdersDataTable);


            decimal SamplesWriteOffCost = 0;
            decimal WriteOffDebtCost = 0;
            decimal WriteOffDefectsCost = 0;
            decimal WriteOffProductionErrorsCost = 0;
            decimal WriteOffZOVErrorsCost = 0;


            foreach (DataRow Row in TempPaymentDetailDataTable.Rows)
            {
                if (Row.RowState == DataRowState.Deleted)
                    continue;

                decimal DebtCost = Convert.ToDecimal(Row["DebtCost"]);
                decimal SamplesCost = Convert.ToDecimal(Row["SamplesWriteOffCost"]);
                SamplesWriteOffCost += SamplesCost;


                DataRow[] MORow = MainOrdersDataTable.Select("DocNumber = '" + Row["DocNumber"] + "'");

                MORow[0]["SamplesWriteOffCost"] = SamplesCost;


                if (Row["DebtTypeID"].ToString() != "0")
                {
                    if (Row["DebtTypeID"].ToString() == "1")
                    {
                        MORow[0]["WriteOffDebtCost"] = DebtCost;
                        WriteOffDebtCost += DebtCost;
                    }
                    if (Row["DebtTypeID"].ToString() == "2")
                    {
                        MORow[0]["WriteOffDefectsCost"] = DebtCost;
                        WriteOffDefectsCost += DebtCost;
                    }
                    if (Row["DebtTypeID"].ToString() == "3")
                    {
                        MORow[0]["WriteOffProductionErrorsCost"] = DebtCost;
                        WriteOffProductionErrorsCost += DebtCost;
                    }
                    if (Row["DebtTypeID"].ToString() == "4")
                    {
                        MORow[0]["WriteOffZOVErrorsCost"] = DebtCost;
                        WriteOffZOVErrorsCost += DebtCost;
                    }
                }

                MORow[0]["TotalWriteOffCost"] = DebtCost + SamplesCost;
                MORow[0]["ProfitCost"] = (Convert.ToDecimal(MORow[0]["IncomeCost"]) - (DebtCost + SamplesCost));
            }

            MainOrdersDA.Update(MainOrdersDataTable);

            foreach (DataRow Row in TempPaymentDetailDataTable.Rows)
            {
                if (Row.RowState == DataRowState.Deleted)
                    continue;

                DataRow[] MORow = MainOrdersDataTable.Select("DocNumber = '" + Row["DocNumber"] + "'");



                SummaryResultMegaOrder(Convert.ToInt32(MORow[0]["MegaOrderID"]));
            }



            MegaOrdersDA = new SqlDataAdapter("SELECT * FROM MegaOrders WHERE DispatchDate >= '" + DateFrom.ToString("yyyy-MM-dd") +
                                              "' AND DispatchDate <= '" + DateTo.ToString("yyyy-MM-dd") + "'",
                                              ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersCB = new SqlCommandBuilder(MegaOrdersDA);
            MegaOrdersDA.Fill(MegaOrdersDataTable);

            DataRow PWRow = PaymentWeeksDataTable.NewRow();

            decimal TotalCost = 0;
            decimal DispatchedCost = 0;
            decimal DispatchedDebtCost = 0;
            decimal CalcDebtCost = 0;
            decimal CalcDefectsCost = 0;
            decimal CalcProductionErrorsCost = 0;
            decimal CalcZOVErrorsCost = 0;

            decimal TotalWriteOffCost = 0;
            decimal TotalCalcWriteOffCost = 0;
            decimal ProfitCost = 0;


            foreach (DataRow MERow in MegaOrdersDataTable.Rows)
            {
                TotalCost += Convert.ToDecimal(MERow["TotalCost"]);
                DispatchedCost += Convert.ToDecimal(MERow["DispatchedCost"]);
                DispatchedDebtCost += Convert.ToDecimal(MERow["DispatchedDebtCost"]);
                CalcDebtCost += Convert.ToDecimal(MERow["CalcDebtCost"]);
                CalcDefectsCost += Convert.ToDecimal(MERow["CalcDefectsCost"]);
                CalcProductionErrorsCost += Convert.ToDecimal(MERow["CalcProductionErrorsCost"]);
                CalcZOVErrorsCost += Convert.ToDecimal(MERow["CalcZOVErrorsCost"]);
            }

            TotalWriteOffCost = SamplesWriteOffCost + WriteOffDebtCost + WriteOffDefectsCost + WriteOffProductionErrorsCost + WriteOffZOVErrorsCost;
            TotalCalcWriteOffCost = CalcDebtCost + CalcDefectsCost + CalcProductionErrorsCost + CalcZOVErrorsCost;
            ProfitCost = TotalCost - TotalCalcWriteOffCost - TotalWriteOffCost;



            PWRow["DateFrom"] = DateFrom.ToShortDateString();
            PWRow["DateTo"] = DateTo.ToShortDateString();
            PWRow["TotalCost"] = TotalCost;
            PWRow["DispatchedCost"] = DispatchedCost;
            PWRow["DispatchedDebtCost"] = DispatchedDebtCost;
            PWRow["CalcDebtCost"] = CalcDebtCost;
            PWRow["CalcDefectsCost"] = CalcDefectsCost;
            PWRow["CalcProductionErrorsCost"] = CalcProductionErrorsCost;
            PWRow["CalcZOVErrorsCost"] = CalcZOVErrorsCost;
            PWRow["SamplesWriteOffCost"] = SamplesWriteOffCost;
            PWRow["WriteOffDebtCost"] = WriteOffDebtCost;
            PWRow["WriteOffDefectsCost"] = WriteOffDefectsCost;
            PWRow["WriteOffProductionErrorsCost"] = WriteOffProductionErrorsCost;
            PWRow["WriteOffZOVErrorsCost"] = WriteOffZOVErrorsCost;
            PWRow["TotalCalcWriteOffCost"] = TotalCalcWriteOffCost;
            PWRow["TotalWriteOffCost"] = TotalWriteOffCost;
            PWRow["ErrorWriteOffCost"] = ErrorWriteOffCost;
            PWRow["CompensationCost"] = CompensationCost;
            PWRow["ProfitCost"] = ProfitCost + (CompensationCost - ErrorWriteOffCost);

            PaymentWeeksDataTable.Rows.Add(PWRow);

            PaymentWeeksDataAdapter.Update(PaymentWeeksDataTable);
            PaymentWeeksDataTable.Clear();
            PaymentWeeksDataAdapter.Fill(PaymentWeeksDataTable);


            object PaymentWeekID = PaymentWeeksDataTable.Select(
                "DateFrom = '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateTo = '" + DateTo.ToString("yyyy-MM-dd") + "'")[0]["PaymentWeekID"];

            foreach (DataRow Row in TempPaymentDetailDataTable.Rows)
            {
                if (Row.RowState == DataRowState.Deleted)
                    continue;

                Row["PaymentWeekID"] = PaymentWeekID;
            }

            TempPaymentDetailDA.Update(TempPaymentDetailDataTable.GetChanges());
        }

        private void SummaryResultMegaOrder(int MegaOrderID)
        {
            decimal ProfitCost = 0;
            decimal SamplesWriteOffCost = 0;
            decimal WriteOffDebtCost = 0;
            decimal WriteOffDefectsCost = 0;
            decimal WriteOffProductionErrorsCost = 0;
            decimal WriteOffZOVErrorsCost = 0;
            decimal TotalWriteOffCost = 0;

            for (int i = 0; i < 3; i++)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, SamplesWriteOffCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost, TotalWriteOffCost, ProfitCost FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            using (SqlDataAdapter moDA = new SqlDataAdapter("SELECT MainOrderID, SamplesWriteOffCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost, TotalWriteOffCost, ProfitCost FROM MainOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                            {
                                using (DataTable moDT = new DataTable())
                                {
                                    if (moDA.Fill(moDT) == 0)
                                        return;

                                    foreach (DataRow Row in moDT.Rows)
                                    {
                                        SamplesWriteOffCost += Convert.ToDecimal(Row["SamplesWriteOffCost"]);
                                        WriteOffDebtCost += Convert.ToDecimal(Row["WriteOffDebtCost"]);
                                        WriteOffDefectsCost += Convert.ToDecimal(Row["WriteOffDefectsCost"]);
                                        WriteOffProductionErrorsCost += Convert.ToDecimal(Row["WriteOffProductionErrorsCost"]);
                                        WriteOffZOVErrorsCost += Convert.ToDecimal(Row["WriteOffZOVErrorsCost"]);
                                        TotalWriteOffCost += Convert.ToDecimal(Row["TotalWriteOffCost"]);
                                        ProfitCost += Convert.ToDecimal(Row["ProfitCost"]);
                                    }
                                }
                            }

                            DT.Rows[0]["SamplesWriteOffCost"] = SamplesWriteOffCost;
                            DT.Rows[0]["WriteOffDebtCost"] = WriteOffDebtCost;
                            DT.Rows[0]["WriteOffDefectsCost"] = WriteOffDefectsCost;
                            DT.Rows[0]["WriteOffProductionErrorsCost"] = WriteOffProductionErrorsCost;
                            DT.Rows[0]["WriteOffZOVErrorsCost"] = WriteOffZOVErrorsCost;
                            DT.Rows[0]["TotalWriteOffCost"] = TotalWriteOffCost;
                            DT.Rows[0]["ProfitCost"] = ProfitCost;

                            try
                            {
                                DA.Update(DT);
                                break;
                            }
                            catch { }
                        }
                    }
                }
            }

            //MegaOrdersDataTable.Clear();
            //MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);
            //MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
        }

        public void FilterWeek()
        {
            PaymentDetailDataTable.Clear();

            if (((DataRowView)PaymentWeeksBindingSource.Current)["PaymentWeekID"] == DBNull.Value)
                return;

            PaymentDetailDA.Dispose();
            PaymentDetailCB.Dispose();

            PaymentDetailDA = new SqlDataAdapter("SELECT * FROM PaymentDetail WHERE PaymentWeekID = " + ((DataRowView)PaymentWeeksBindingSource.Current)["PaymentWeekID"],
                ConnectionStrings.ZOVOrdersConnectionString);
            PaymentDetailCB = new SqlCommandBuilder(PaymentDetailDA);
            PaymentDetailDA.Fill(PaymentDetailDataTable);
        }

        public void RemoveCurrentPaymentWeek()
        {
            if (PaymentWeeksBindingSource.Count > 0)
            {
                object PaymentWeekID = ((DataRowView)PaymentWeeksBindingSource.Current)["PaymentWeekID"];

                PaymentWeeksBindingSource.RemoveCurrent();
                PaymentWeeksDataAdapter.Update(PaymentWeeksDataTable);


                using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM PaymentDetail WHERE PaymentWeekID = " + PaymentWeekID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DispatchMegaOrders(DateTime From, DateTime To)
        {
            int AllPackCount = 0;
            int AllDispPackCount = 0;
            int MainOrderPackCount = 0;
            int MainOrderDispPackCount = 0;
            int DispStatus = 0;
            int MainOrderID = 0;

            decimal FrontsDebtCost = 0;
            decimal DecorDebtCost = 0;
            decimal DispatchedCost = 0;
            decimal DispatchedDebtCost = 0;

            DateTime DispatchDate;

            DataTable PackageDetailsDataTable = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();
            DataTable PackOrdersDataTable = new DataTable();
            DataTable MainOrdersDataTable = new DataTable();
            DataTable MegaOrdersDataTable = new DataTable();
            DataTable DebtsDataTable = new DataTable();

            SqlDataAdapter MainOrdersSqlDataAdapter = null;
            SqlCommandBuilder MainOrdersSqlCommandBuilder = null;
            SqlDataAdapter MegaOrdersSqlDataAdapter = null;
            SqlCommandBuilder MegaOrdersSqlCommandBuilder = null;
            SqlCommandBuilder DebtsSqlCommandBuilder = null;
            SqlDataAdapter DebtsSqlDataAdapter = null;

            DebtsSqlDataAdapter = new SqlDataAdapter("SELECT * FROM Debts",
                ConnectionStrings.ZOVOrdersConnectionString);
            DebtsSqlCommandBuilder = new SqlCommandBuilder(DebtsSqlDataAdapter);
            DebtsSqlDataAdapter.Fill(DebtsDataTable);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ProductType, PackageStatusID, PackageDetails.PackageID, OrderID, Count FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                " WHERE DispatchDate >= '" + From.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" + To.ToString("yyyy-MM-dd") + "')))",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackageDetailsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrdersID, Cost, Count FROM FrontsOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                " WHERE DispatchDate >= '" + From.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" + To.ToString("yyyy-MM-dd") + "'))",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrderID, Cost, Count FROM DecorOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                " WHERE DispatchDate >= '" + From.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" + To.ToString("yyyy-MM-dd") + "'))",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, MainOrderID, PackageStatusID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                " WHERE DispatchDate >= '" + From.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" + To.ToString("yyyy-MM-dd") + "'))",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackOrdersDataTable);
            }

            MainOrdersSqlDataAdapter = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, ClientID, DocNumber, DispatchStatusID, ProfilPackCount, TPSPackCount," +
                " FrontsCost, DecorCost, OrderCost, DispatchedCost, DispatchedDebtCost FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                " WHERE DispatchDate >= '" + From.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" + To.ToString("yyyy-MM-dd") + "')",
                ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersSqlCommandBuilder = new SqlCommandBuilder(MainOrdersSqlDataAdapter);
            MainOrdersSqlDataAdapter.Fill(MainOrdersDataTable);

            MegaOrdersSqlDataAdapter = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate, DispatchStatusID, DispatchedCost, DispatchedDebtCost FROM MegaOrders" +
                " WHERE DispatchDate >= '" + From.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" + To.ToString("yyyy-MM-dd") + "'",
                ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersSqlCommandBuilder = new SqlCommandBuilder(MegaOrdersSqlDataAdapter);

            MegaOrdersSqlDataAdapter.Fill(MegaOrdersDataTable);

            int MegaOrderID = -1;
            DateTime Date = DateTime.Now;

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                DispatchedCost = 0;
                DispatchedDebtCost = 0;

                MegaOrderID = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]);
                DispatchDate = Convert.ToDateTime(MegaOrdersDataTable.Rows[i]["DispatchDate"]);

                //SetDispatched(MegaOrderID, Date);

                DataRow[] MainRows = MainOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);
                if (MainRows.Count() < 1)
                    continue;

                foreach (DataRow item in MainRows)
                {
                    MainOrderID = Convert.ToInt32(item["MainOrderID"]);

                    //кол-во всех упаковок в подзаказе
                    MainOrderPackCount = Convert.ToInt32(item["ProfilPackCount"]) + Convert.ToInt32(item["TPSPackCount"]);
                    AllPackCount += MainOrderPackCount;

                    DataRow[] Rows = PackOrdersDataTable.Select("PackageStatusID = 3 AND MainOrderID = " + MainOrderID);

                    MainOrderDispPackCount = Rows.Count();//кол-во отгруженных упаковок
                    AllDispPackCount += MainOrderDispPackCount;

                    DispStatus = 0;
                    //если хоть что-то отгружалось
                    if (MainOrderDispPackCount > 0)
                    {
                        //все упаковки отгружены
                        if (MainOrderPackCount == MainOrderDispPackCount)
                            DispStatus = 2;//отгружено
                        else
                            DispStatus = 3;//отгружено частично
                    }
                    else
                    {
                        //если машина уехала, но упаковки не были отгружены по сканеру
                        if (MainOrderPackCount > MainOrderDispPackCount)
                            DispStatus = 1;//статус не отгружено
                    }

                    item["DispatchStatusID"] = DispStatus;

                    DataRow[] dRows = DebtsDataTable.Select("MainOrderID = " + MainOrderID);

                    if (dRows.Count() > 0)
                    {
                        if (DispStatus == 1)
                        {
                            dRows[0].Delete();
                        }
                    }
                    else
                    {
                        if (DispStatus != 1)
                        {
                            DataRow NewRow = DebtsDataTable.NewRow();
                            NewRow["DispatchDate"] = DispatchDate;
                            NewRow["ClientID"] = item["ClientID"];
                            NewRow["MainOrderID"] = item["MainOrderID"];
                            NewRow["DocNumber"] = item["DocNumber"];
                            DebtsDataTable.Rows.Add(NewRow);

                        }
                    }

                    ////debts table
                    //using (SqlDataAdapter dDA = new SqlDataAdapter("SELECT * FROM Debts",
                    //    ConnectionStrings.ZOVOrdersConnectionString))
                    //{
                    //    using (SqlCommandBuilder dCB = new SqlCommandBuilder(dDA))
                    //    {
                    //        using (DataTable DT = new DataTable())
                    //        {
                    //            dDA.Fill(DT);

                    //            DataRow[] dRows = DT.Select("MainOrderID = " + MainOrderID);

                    //            if (dRows.Count() > 0)
                    //            {
                    //                if (DispStatus == 1)
                    //                {
                    //                    dRows[0].Delete();
                    //                    dDA.Update(dRows);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                if (DispStatus != 1)
                    //                {
                    //                    DataRow NewRow = DT.NewRow();
                    //                    NewRow["DispatchDate"] = DispatchDate;
                    //                    NewRow["ClientID"] = item["ClientID"];
                    //                    NewRow["MainOrderID"] = item["MainOrderID"];
                    //                    NewRow["DocNumber"] = item["DocNumber"];
                    //                    DT.Rows.Add(NewRow);

                    //                    dDA.Update(DT);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    FrontsDebtCost = 0;
                    DecorDebtCost = 0;

                    //частично отгружено
                    if (DispStatus == 2)
                    {
                        //вычисление долгов по фасадам
                        DataRow[] PFRows = PackageDetailsDataTable.Select("PackageStatusID <> 3 AND ProductType = 0 AND MainOrderID = " + MainOrderID);

                        foreach (DataRow PFRow in PFRows)
                        {
                            DataRow[] FRows = FrontsOrdersDataTable.Select("FrontsOrdersID = " + Convert.ToInt32(PFRow["OrderID"]));

                            foreach (DataRow Row in FRows)
                            {
                                FrontsDebtCost += Decimal.Round(Convert.ToDecimal(Row["Cost"]) / Convert.ToDecimal(Row["Count"]) * Convert.ToDecimal(PFRow["Count"]),
                                    1,
                                    MidpointRounding.AwayFromZero);
                            }
                        }

                        //вычисление долгов по декору
                        DataRow[] PDRows = PackageDetailsDataTable.Select("PackageStatusID <> 3 AND ProductType = 1 AND MainOrderID = " + MainOrderID);

                        foreach (DataRow DFRow in PDRows)
                        {
                            DataRow[] DRows = DecorOrdersDataTable.Select("DecorOrderID = " + Convert.ToInt32(DFRow["OrderID"]));

                            foreach (DataRow Row in DRows)
                            {
                                DecorDebtCost += Decimal.Round(Convert.ToDecimal(Row["Cost"]) / Convert.ToDecimal(Row["Count"]) * Convert.ToDecimal(DFRow["Count"]),
                                    1,
                                    MidpointRounding.AwayFromZero);
                            }
                        }
                    }

                    //не отгружено
                    if (DispStatus == 3)
                    {
                        FrontsDebtCost = Convert.ToDecimal(item["FrontsCost"]);
                        DecorDebtCost = Convert.ToDecimal(item["DecorCost"]);
                    }

                    //вычисление долгов для каждого подзаказа
                    item["DispatchedCost"] =
                        Convert.ToDecimal(item["OrderCost"]) - (FrontsDebtCost + DecorDebtCost);
                    item["DispatchedDebtCost"] = FrontsDebtCost + DecorDebtCost;

                    //вычисление долгов для целого заказа
                    DispatchedCost += Convert.ToDecimal(item["DispatchedCost"]);
                    DispatchedDebtCost += Convert.ToDecimal(item["DispatchedDebtCost"]);
                }

                DispStatus = 0;
                //если хоть что-то отгружалось
                if (AllDispPackCount > 0)
                {
                    if (AllPackCount == AllDispPackCount)
                        DispStatus = 2;//отгружено
                    else
                        DispStatus = 3;//отгружено частично
                }
                else
                {
                    //если машина уехала, но упаковки не были отгружены по сканеру
                    if (AllPackCount > AllDispPackCount)
                        DispStatus = 1;
                }


                MegaOrdersDataTable.Rows[i]["DispatchStatusID"] = DispStatus;
                MegaOrdersDataTable.Rows[i]["DispatchedCost"] = DispatchedCost;
                MegaOrdersDataTable.Rows[i]["DispatchedDebtCost"] = DispatchedDebtCost;

            }
            MainOrdersSqlDataAdapter.Update(MainOrdersDataTable);
            MegaOrdersSqlDataAdapter.Update(MegaOrdersDataTable);
            DataTable ddd = DebtsDataTable.GetChanges();
            DebtsSqlDataAdapter.Update(DebtsDataTable);

            MainOrdersSqlDataAdapter.Dispose();
            MainOrdersSqlCommandBuilder.Dispose();
            MegaOrdersSqlDataAdapter.Dispose();
            MegaOrdersSqlCommandBuilder.Dispose();
            DebtsSqlDataAdapter.Dispose();
            DebtsSqlCommandBuilder.Dispose();

            PackOrdersDataTable.Dispose();
            MainOrdersDataTable.Dispose();
            PackageDetailsDataTable.Dispose();
            FrontsOrdersDataTable.Dispose();
            DecorOrdersDataTable.Dispose();
            MegaOrdersDataTable.Dispose();
            MainOrdersDataTable.Dispose();
            DebtsDataTable.Dispose();
        }

        public void SetDispatched(int MegaOrderID, DateTime DispatchDate)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int AllPackCount = 0;
            int AllDispPackCount = 0;
            int MainOrderPackCount = 0;
            int MainOrderDispPackCount = 0;
            int DispStatus = 0;
            int MainOrderID = 0;

            decimal FrontsDebtCost = 0;
            decimal DecorDebtCost = 0;
            decimal DispatchedCost = 0;
            decimal DispatchedDebtCost = 0;

            DataTable PackageDetailsDataTable = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();
            DataTable PackOrdersDataTable = new DataTable();
            DataTable MainOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ProductType, PackageStatusID, PackageDetails.PackageID, OrderID, Count FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + "))",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackageDetailsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrdersID, Cost, Count FROM FrontsOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrderID, Cost, Count FROM DecorOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, MainOrderID, PackageStatusID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ClientID, DocNumber, DispatchStatusID, ProfilPackCount, TPSPackCount," +
                " FrontsCost, DecorCost, OrderCost, DispatchedCost, DispatchedDebtCost FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(MainOrdersDataTable);

                    for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                    {
                        MainOrderID = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);

                        //кол-во всех упаковок в подзаказе
                        MainOrderPackCount = Convert.ToInt32(MainOrdersDataTable.Rows[i]["ProfilPackCount"]) + Convert.ToInt32(MainOrdersDataTable.Rows[i]["TPSPackCount"]);
                        AllPackCount += MainOrderPackCount;

                        DataRow[] Rows = PackOrdersDataTable.Select("PackageStatusID = 3 AND MainOrderID = " + MainOrderID);

                        MainOrderDispPackCount = Rows.Count();//кол-во отгруженных упаковок
                        AllDispPackCount += MainOrderDispPackCount;

                        DispStatus = 0;
                        //если хоть что-то отгружалось
                        if (MainOrderDispPackCount > 0)
                        {
                            //все упаковки отгружены
                            if (MainOrderPackCount == MainOrderDispPackCount)
                                DispStatus = 2;//отгружено
                            else
                                DispStatus = 3;//отгружено частично
                        }
                        else
                        {
                            //если машина уехала, но упаковки не были отгружены по сканеру
                            if (MainOrderPackCount > MainOrderDispPackCount)
                                DispStatus = 1;//статус не отгружено
                        }

                        MainOrdersDataTable.Rows[i]["DispatchStatusID"] = DispStatus;

                        //debts table
                        using (SqlDataAdapter dDA = new SqlDataAdapter("SELECT * FROM Debts",
                            ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            using (SqlCommandBuilder dCB = new SqlCommandBuilder(dDA))
                            {
                                using (DataTable DT = new DataTable())
                                {
                                    dDA.Fill(DT);

                                    DataRow[] dRows = DT.Select("MainOrderID = " + MainOrderID);

                                    if (dRows.Count() > 0)
                                    {
                                        if (DispStatus == 1)
                                        {
                                            dRows[0].Delete();
                                            dDA.Update(dRows);
                                        }
                                    }
                                    else
                                    {
                                        if (DispStatus != 1)
                                        {
                                            DataRow NewRow = DT.NewRow();
                                            NewRow["DispatchDate"] = DispatchDate;
                                            NewRow["ClientID"] = MainOrdersDataTable.Rows[i]["ClientID"];
                                            NewRow["MainOrderID"] = MainOrdersDataTable.Rows[i]["MainOrderID"];
                                            NewRow["DocNumber"] = MainOrdersDataTable.Rows[i]["DocNumber"];
                                            DT.Rows.Add(NewRow);

                                            dDA.Update(DT);
                                        }
                                    }
                                }
                            }
                        }

                        FrontsDebtCost = 0;
                        DecorDebtCost = 0;

                        //частично отгружено
                        if (DispStatus == 2)
                        {
                            //вычисление долгов по фасадам
                            DataRow[] PFRows = PackageDetailsDataTable.Select("PackageStatusID <> 3 AND ProductType = 0 AND MainOrderID = " + MainOrderID);

                            foreach (DataRow PFRow in PFRows)
                            {
                                DataRow[] FRows = FrontsOrdersDataTable.Select("FrontsOrdersID = " + Convert.ToInt32(PFRow["OrderID"]));

                                foreach (DataRow Row in FRows)
                                {
                                    FrontsDebtCost += Decimal.Round(Convert.ToDecimal(Row["Cost"]) / Convert.ToDecimal(Row["Count"]) * Convert.ToDecimal(PFRow["Count"]),
                                        1,
                                        MidpointRounding.AwayFromZero);
                                }
                            }

                            //вычисление долгов по декору
                            DataRow[] PDRows = PackageDetailsDataTable.Select("PackageStatusID <> 3 AND ProductType = 1 AND MainOrderID = " + MainOrderID);

                            foreach (DataRow DFRow in PDRows)
                            {
                                DataRow[] DRows = DecorOrdersDataTable.Select("DecorOrderID = " + Convert.ToInt32(DFRow["OrderID"]));

                                foreach (DataRow Row in DRows)
                                {
                                    DecorDebtCost += Decimal.Round(Convert.ToDecimal(Row["Cost"]) / Convert.ToDecimal(Row["Count"]) * Convert.ToDecimal(DFRow["Count"]),
                                        1,
                                        MidpointRounding.AwayFromZero);
                                }
                            }
                        }

                        //не отгружено
                        if (DispStatus == 3)
                        {
                            FrontsDebtCost = Convert.ToDecimal(MainOrdersDataTable.Rows[i]["FrontsCost"]);
                            DecorDebtCost = Convert.ToDecimal(MainOrdersDataTable.Rows[i]["DecorCost"]);
                        }

                        //вычисление долгов для каждого подзаказа
                        MainOrdersDataTable.Rows[i]["DispatchedCost"] =
                            Convert.ToDecimal(MainOrdersDataTable.Rows[i]["OrderCost"]) - (FrontsDebtCost + DecorDebtCost);
                        MainOrdersDataTable.Rows[i]["DispatchedDebtCost"] = FrontsDebtCost + DecorDebtCost;

                        //вычисление долгов для целого заказа
                        DispatchedCost += Convert.ToDecimal(MainOrdersDataTable.Rows[i]["DispatchedCost"]);
                        DispatchedDebtCost += Convert.ToDecimal(MainOrdersDataTable.Rows[i]["DispatchedDebtCost"]);
                    }

                    DA.Update(MainOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, DispatchStatusID, DispatchedCost, DispatchedDebtCost FROM MegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DispStatus = 0;
                        //если хоть что-то отгружалось
                        if (AllDispPackCount > 0)
                        {
                            if (AllPackCount == AllDispPackCount)
                                DispStatus = 2;//отгружено
                            else
                                DispStatus = 3;//отгружено частично
                        }
                        else
                        {
                            //если машина уехала, но упаковки не были отгружены по сканеру
                            if (AllPackCount > AllDispPackCount)
                                DispStatus = 1;
                        }

                        DT.Rows[0]["DispatchStatusID"] = DispStatus;
                        DT.Rows[0]["DispatchedCost"] = DispatchedCost;
                        DT.Rows[0]["DispatchedDebtCost"] = DispatchedDebtCost;

                        DA.Update(DT);
                    }
                }
            }


            PackOrdersDataTable.Dispose();
            MainOrdersDataTable.Dispose();
            PackageDetailsDataTable.Dispose();
            FrontsOrdersDataTable.Dispose();
            DecorOrdersDataTable.Dispose();

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }
    }
}
