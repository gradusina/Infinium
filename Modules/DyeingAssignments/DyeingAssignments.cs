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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.DyeingAssignments
{
    public enum Fronts
    {
        //Antalia = 61,
        //Venecia = 62,
        //VeneciaF = 63,
        //Leon = 66,
        //LeonF = 67,
        //Limog = 68,
        //Luk = 69,
        //LukPVH = 111,
        //Milano = 71,
        //MilanoK = 72,
        //MilanoKF = 73,
        //Praga = 74,
        //Sigma = 75,
        //Fat = 95,

        //Techno1 = 78,
        //Techno2 = 79,
        //Techno4 = 81,
        //Techno4Domino = 108,
        //Techno4Mega = 109,
        //Techno5 = 96,
        //Marsel1 = 70,
        //Marsel3 = 101,
        //PR1 = 120,
        //PR2 = 121,
        //PR3 = 122,
        //PRU8 = 123,
        //Techno2New = 124,
        Infiniti = 3634,

        Geneva = 65,
        Kansas = 3577,
        Sofia = 3415,
        Tafel2 = 3662,
        Tafel3 = 3663,
        Tafel3Gl = 3664,
        Turin1 = 3419,
        Lorenzo = 15450,
        Turin3 = 3633
    }

    public class FrontsOrdersManager
    {
        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable InsetMarginsDT;
        private DataTable BatchFrontsDT;
        private DataTable DyeingFrontsDT;
        private DataTable TempBatchFrontsDT;
        private DataTable SavedFrontsOrdersDT;

        BindingSource BatchFrontsBS = null;
        BindingSource DyeingFrontsBS = null;

        public BindingSource BatchFrontsList
        {
            get { return BatchFrontsBS; }
        }

        public BindingSource DyeingFrontsList
        {
            get { return DyeingFrontsBS; }
        }

        public FrontsOrdersManager()
        {

        }

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TempBatchFrontsDT = new DataTable();
            SavedFrontsOrdersDT = new DataTable();
            BatchFrontsDT = new DataTable();
            BatchFrontsBS = new BindingSource();
            DyeingFrontsDT = new DataTable();
            DyeingFrontsBS = new BindingSource();
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
            InsetMarginsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetMargins",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetMarginsDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, FrontConfigID, FactoryID, Square, IsNonStandard, Notes FROM FrontsOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchFrontsDT);
                BatchFrontsDT.Columns.Add(new DataColumn(("DyeingAssignmentID"), System.Type.GetType("System.Int32")));
                BatchFrontsDT.Columns.Add(new DataColumn(("GroupType"), System.Type.GetType("System.Int32")));
                BatchFrontsDT.Columns.Add(new DataColumn(("WorkAssignmentID"), System.Type.GetType("System.Int32")));
                BatchFrontsDT.Columns.Add(new DataColumn(("MegaBatchID"), System.Type.GetType("System.Int32")));
                BatchFrontsDT.Columns.Add(new DataColumn(("BatchID"), System.Type.GetType("System.Int32")));
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignmentDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingFrontsDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignmentDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(SavedFrontsOrdersDT);
            }
        }

        private void Binding()
        {
            BatchFrontsBS.DataSource = BatchFrontsDT;
            DyeingFrontsBS.DataSource = DyeingFrontsDT;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public void ClearTables()
        {
            BatchFrontsDT.Clear();
            DyeingFrontsDT.Clear();
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

        public bool FilterDyeingFrontsByMainOrder(int DyeingAssignmentID, int GroupType, int[] MainOrders)
        {
            for (int i = DyeingFrontsDT.Columns.Count - 1; i >= 0; i--)
            {
                string ColumnName = DyeingFrontsDT.Columns[i].ColumnName;
                if (ColumnName.Contains("ColumnDyeingCart"))
                    DyeingFrontsDT.Columns.RemoveAt(i);
            }

            DataTable DT = DyeingFrontsDT.Clone();
            DyeingFrontsDT.Clear();
            string SelectCommand = string.Empty;
            SelectCommand = @"SELECT * FROM DyeingAssignmentDetails
                WHERE DyeingAssignmentDetails.Width<>-1 AND DyeingAssignmentID=" + DyeingAssignmentID + " AND DyeingAssignmentDetails.MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            bool bColumnDyeingCart = false;
            DataTable Table = new DataTable();
            if (GroupType == 2)
            {
                using (DataView DV = new DataView(DT))
                {
                    Table = DV.ToTable(true, new string[] { "DyeingCartID" });
                }
                if (Table.Rows.Count > 0)
                {
                    bColumnDyeingCart = true;
                    for (int i = 0; i < Table.Rows.Count; i++)
                    {
                        string ColumnName = "ColumnDyeingCart" + Table.Rows[i]["DyeingCartID"];
                        if (!DyeingFrontsDT.Columns.Contains(ColumnName))
                            DyeingFrontsDT.Columns.Add(new DataColumn((ColumnName), System.Type.GetType("System.Decimal")));
                        for (int j = 0; j < DyeingFrontsDT.Rows.Count; j++)
                        {
                            DyeingFrontsDT.Rows[j][ColumnName] = 0;
                        }
                    }
                }
                Table.Dispose();
            }

            using (DataView DV = new DataView(DT))
            {
                Table = DV.ToTable(true, new string[] { "FrontsOrdersID" });
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    decimal Square = 0;
                    int PlanCount = 0;
                    DataRow[] rows = DT.Select("FrontsOrdersID=" + Table.Rows[i]["FrontsOrdersID"]);
                    DataRow NewRow = DyeingFrontsDT.NewRow();
                    NewRow.ItemArray = rows[0].ItemArray;
                    if (bColumnDyeingCart)
                    {
                        foreach (DataRow item in rows)
                        {
                            string colname = "ColumnDyeingCart" + item["DyeingCartID"];
                            NewRow[colname] = item["PlanCount"];
                            PlanCount += Convert.ToInt32(item["PlanCount"]);
                            Square += Convert.ToDecimal(item["Square"]);
                        }
                        NewRow["PlanCount"] = PlanCount;
                        NewRow["Square"] = Square;
                    }
                    DyeingFrontsDT.Rows.Add(NewRow);
                }
            }
            //DyeingFrontsDT.DefaultView.Sort = "Front, FrameColor, InsetType";
            DyeingFrontsBS.MoveFirst();

            return DyeingFrontsDT.Rows.Count > 0;
        }

        public bool FilterBatchFrontsByMainOrder(int DyeingAssignmentID, int GroupType, int WorkAssignmentID, int MegaBatchID, int BatchID, int[] MainOrders)
        {
            for (int i = BatchFrontsDT.Columns.Count - 1; i >= 0; i--)
            {
                string ColumnName = BatchFrontsDT.Columns[i].ColumnName;
                if (ColumnName.Contains("ColumnDyeingCart"))
                    BatchFrontsDT.Columns.RemoveAt(i);
            }

            string OrdersFactoryFilter = string.Empty;
            string SelectCommand = string.Empty;

            DataTable DT = BatchFrontsDT.Clone();
            BatchFrontsDT.Clear();

            if (GroupType == 0)
            {
                SelectCommand = @"SELECT  FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, FrontConfigID, FactoryID, Square, IsNonStandard, Notes FROM FrontsOrders
                WHERE FrontsOrders.Width<>-1 AND FrontsOrders.MainOrderID IN (" + string.Join(",", MainOrders) + ")" + OrdersFactoryFilter;

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);
                }
                foreach (DataRow item in DT.Rows)
                {
                    DataRow NewRow = BatchFrontsDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["DyeingAssignmentID"] = DyeingAssignmentID;
                    NewRow["GroupType"] = GroupType;
                    NewRow["WorkAssignmentID"] = WorkAssignmentID;
                    NewRow["MegaBatchID"] = MegaBatchID;
                    NewRow["BatchID"] = BatchID;
                    BatchFrontsDT.Rows.Add(NewRow);
                }
            }
            else
            {
                SelectCommand = @"SELECT  FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, FrontConfigID, FactoryID, Square, IsNonStandard, Notes FROM FrontsOrders
                WHERE FrontsOrders.Width<>-1 AND FrontsOrders.MainOrderID IN (" + string.Join(",", MainOrders) + ")" + OrdersFactoryFilter;

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);
                }
                foreach (DataRow item in DT.Rows)
                {
                    DataRow NewRow = BatchFrontsDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["DyeingAssignmentID"] = DyeingAssignmentID;
                    NewRow["GroupType"] = GroupType;
                    NewRow["WorkAssignmentID"] = WorkAssignmentID;
                    NewRow["MegaBatchID"] = MegaBatchID;
                    NewRow["BatchID"] = BatchID;
                    BatchFrontsDT.Rows.Add(NewRow);
                }
            }

            //BatchFrontsDT.DefaultView.Sort = "Front, FrameColor, InsetType";
            BatchFrontsBS.MoveFirst();

            return BatchFrontsDT.Rows.Count > 0;
        }

        public void AddDyeingCartColumn(int DyeingCartID)
        {
            bool b = false;
            int Counter = 1;
            string ColumnName = string.Empty;
            while (!b)
            {
                ColumnName = "ColumnDyeingCart" + Counter;
                if (DyeingFrontsDT.Columns.Contains(ColumnName))
                    Counter++;
                else
                    b = true;
            }
            DyeingFrontsDT.Columns.Add(new DataColumn((ColumnName), System.Type.GetType("System.Int32")));
            for (int j = 0; j < DyeingFrontsDT.Rows.Count; j++)
            {
                DyeingFrontsDT.Rows[j][ColumnName] = 0;
            }
            //string ColumnName = "ColumnDyeingCart" + DyeingCartID;
            //if (!DyeingFrontsDT.Columns.Contains(ColumnName))
            //    DyeingFrontsDT.Columns.Add(new DataColumn((ColumnName), System.Type.GetType("System.Int32")));
            //for (int j = 0; j < DyeingFrontsDT.Rows.Count; j++)
            //{
            //    DyeingFrontsDT.Rows[j][ColumnName] = 0;
            //}
        }

        public void AddDyeingCartColumn1()
        {
            bool b = false;
            int Counter = 1;
            string ColumnName = string.Empty;
            while (!b)
            {
                ColumnName = "ColumnDyeingCart" + Counter;
                if (BatchFrontsDT.Columns.Contains(ColumnName))
                    Counter++;
                else
                    b = true;
            }
            BatchFrontsDT.Columns.Add(new DataColumn((ColumnName), System.Type.GetType("System.Decimal")));
            for (int j = 0; j < BatchFrontsDT.Rows.Count; j++)
            {
                BatchFrontsDT.Rows[j][ColumnName] = 0;
            }
        }

        public ArrayList DinstinctFrontsConfig()
        {
            ArrayList FrontConfigID = new ArrayList();
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(TempBatchFrontsDT, "Width<>-1", string.Empty, DataViewRowState.CurrentRows))
            {
                Table = DV.ToTable(true, new string[] { "FrontConfigID" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                FrontConfigID.Add(Convert.ToInt32(Table.Rows[i]["FrontConfigID"]));
            Table.Dispose();
            return FrontConfigID;
        }

        public ArrayList DinstinctFrontsPatina()
        {
            ArrayList PatinaID = new ArrayList();
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(TempBatchFrontsDT, "Width<>-1", string.Empty, DataViewRowState.CurrentRows))
            {
                Table = DV.ToTable(true, new string[] { "PatinaID" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                PatinaID.Add(Convert.ToInt32(Table.Rows[i]["PatinaID"]));
            Table.Dispose();
            return PatinaID;
        }

        public void GetSavedFrontsOrders(int BatchID)
        {
            string SelectCommand = @"SELECT FrontsOrdersID FROM DyeingAssignmentDetails WHERE BatchID=" + BatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                SavedFrontsOrdersDT.Clear();
                DA.Fill(SavedFrontsOrdersDT);
            }
        }

        public bool IsFrontOrderSaved(int FrontsOrdersID)
        {
            DataRow[] rows = SavedFrontsOrdersDT.Select("FrontsOrdersID=" + FrontsOrdersID);
            return rows.Count() > 0;
        }

        public void ReOrdersFrontOrders(bool NewAssignment, int[] FrontsOrders)
        {
            TempBatchFrontsDT.Dispose();

            if (NewAssignment)
            {
                TempBatchFrontsDT = BatchFrontsDT.Clone();
                DataRow[] rows = BatchFrontsDT.Select("FrontsOrdersID IN (" + string.Join(",", FrontsOrders) + ")");
                foreach (DataRow item in rows)
                {
                    DataRow NewRow = TempBatchFrontsDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    TempBatchFrontsDT.Rows.Add(NewRow);
                }
            }
            else
            {
                TempBatchFrontsDT = DyeingFrontsDT.Clone();
                DataRow[] rows = DyeingFrontsDT.Select("FrontsOrdersID IN (" + string.Join(",", FrontsOrders) + ")");
                foreach (DataRow item in rows)
                {
                    DataRow NewRow = TempBatchFrontsDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    TempBatchFrontsDT.Rows.Add(NewRow);
                }
            }
        }

        public void MarketingReatailFrontOrders(bool NewAssignment)
        {
            string SelectCommand = string.Empty;
            TempBatchFrontsDT.Dispose();
            if (NewAssignment)
                TempBatchFrontsDT = BatchFrontsDT.Copy();
            else
                TempBatchFrontsDT = DyeingFrontsDT.Copy();
            //            string SelectCommand = string.Empty;
            //            TempBatchFrontsDT.Dispose();
            //            TempBatchFrontsDT = BatchFrontsDT.Clone();

            //            SelectCommand = @"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.PatinaID FROM FrontsOrders
            //                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID
            //                WHERE FrontsOrders.MainOrderID IN (" + string.Join(",", MainOrders) + ") AND FrontsOrders.FactoryID = 2";
            //            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            //            {
            //                DA.Fill(TempBatchFrontsDT);
            //            }
            //            foreach (DataRow item in TempBatchFrontsDT.Rows)
            //            {
            //                item["GroupType"] = GroupType;
            //                item["WorkAssignmentID"] = WorkAssignmentID;
            //                item["MegaBatchID"] = MegaBatchID;
            //                item["BatchID"] = BatchID;
            //            }
        }

        public void MarketingWholeFrontOrders(bool NewAssignment)
        {
            string SelectCommand = string.Empty;
            TempBatchFrontsDT.Dispose();
            if (NewAssignment)
                TempBatchFrontsDT = BatchFrontsDT.Copy();
            else
                TempBatchFrontsDT = DyeingFrontsDT.Copy();
            //            SelectCommand = @"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.PatinaID FROM FrontsOrders
            //                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID
            //                WHERE FrontsOrders.MainOrderID IN (" + string.Join(",", MainOrders) + ") AND FrontsOrders.FactoryID = 2";
            //            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            //            {
            //                DA.Fill(TempBatchFrontsDT);
            //            }
        }

        public void ZOVFrontOrders(bool NewAssignment, int GroupType, int WorkAssignmentID, int MegaBatchID, int BatchID, int[] MainOrders)
        {
            string SelectCommand = string.Empty;
            TempBatchFrontsDT.Dispose();
            if (NewAssignment)
                TempBatchFrontsDT = BatchFrontsDT.Copy();
            else
                TempBatchFrontsDT = DyeingFrontsDT.Copy();

            //            SelectCommand = @"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.PatinaID FROM FrontsOrders
            //                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID
            //                WHERE FrontsOrders.MainOrderID IN (" + string.Join(",", MainOrders) + ") AND FrontsOrders.FactoryID = 2";
            //            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            //            {
            //                DA.Fill(TempBatchFrontsDT);
            //            }

            //            foreach (DataRow item in TempBatchFrontsDT.Rows)
            //            {
            //                item["GroupType"] = GroupType;
            //                item["WorkAssignmentID"] = WorkAssignmentID;
            //                item["MegaBatchID"] = MegaBatchID;
            //                item["BatchID"] = BatchID;
            //            }
        }

        public void GetTempFrontOrders(bool ZOV, int DyeingAssignmentID, int[] MainOrders)
        {
            string OrdersConnectionString = string.Empty;
            if (ZOV)
                OrdersConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            else
                OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            string OrdersFactoryFilter = string.Empty;
            string SelectCommand = string.Empty;
            SelectCommand = @"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.PatinaID FROM FrontsOrders
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID
                WHERE FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID = " + DyeingAssignmentID +
                ") AND FrontsOrders.MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            TempBatchFrontsDT.Clear();

            if (ZOV)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(TempBatchFrontsDT);
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(TempBatchFrontsDT);
                }
            }
        }

        public decimal GetSquare(bool NewAssignment, ref decimal Square, ref int Count)
        {
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(TempBatchFrontsDT, "Width<>-1", string.Empty, DataViewRowState.CurrentRows))
            {
                Table = DV.ToTable();
            }
            foreach (DataRow row in Table.Rows)
            {
                Square += Convert.ToDecimal(row["Square"]);
                if (NewAssignment)
                    Count += Convert.ToInt32(row["Count"]);
                else
                    Count += Convert.ToInt32(row["PlanCount"]);
            }
            Square = Decimal.Round(Square, 3, MidpointRounding.AwayFromZero);
            return Square;
        }

        public void SaveBatchFronts(ControlAssignments ControlAssignmentsManager)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentDetails WHERE DyeingAssignmentID=" + ControlAssignmentsManager.DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bColumnDyeingCart = false;
                        foreach (DataColumn col in TempBatchFrontsDT.Columns)
                        {
                            string ColumnName = col.ColumnName;
                            if (ColumnName.Contains("ColumnDyeingCart"))
                            {
                                decimal Square = 0;
                                decimal TotalSquare = 0;

                                bColumnDyeingCart = true;
                                ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                                ControlAssignmentsManager.SaveDyeingCarts();
                                ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);

                                int DyeingCartID = ControlAssignmentsManager.DyeingCartID;

                                foreach (DataRow row in TempBatchFrontsDT.Rows)
                                {
                                    int FrontsOrdersID = Convert.ToInt32(row["FrontsOrdersID"]);
                                    if (row[ColumnName] == DBNull.Value || Convert.ToInt32(row[ColumnName]) == 0)
                                    {
                                        continue;
                                    }
                                    int PlanCount = Convert.ToInt32(row[ColumnName]);
                                    DataRow[] rows = DT.Select("DyeingCartID=" + DyeingCartID + " AND FrontsOrdersID=" + row["FrontsOrdersID"]);
                                    if (rows.Count() > 0)
                                    {
                                        rows[0]["PlanCount"] = PlanCount;
                                        Square = Convert.ToDecimal(row["Square"]) * PlanCount / Convert.ToDecimal(row["Count"]);
                                        Square = Decimal.Round(Square, 6, MidpointRounding.AwayFromZero);
                                        rows[0]["Square"] = Square;
                                        TotalSquare += Square;
                                    }
                                    else
                                    {
                                        DataRow NewRow = DT.NewRow();
                                        NewRow["DyeingAssignmentID"] = ControlAssignmentsManager.DyeingAssignmentID;
                                        NewRow["DyeingCartID"] = DyeingCartID;
                                        NewRow["GroupType"] = row["GroupType"];
                                        NewRow["WorkAssignmentID"] = row["WorkAssignmentID"];
                                        NewRow["MegaBatchID"] = row["MegaBatchID"];
                                        NewRow["BatchID"] = row["BatchID"];
                                        NewRow["MainOrderID"] = row["MainOrderID"];
                                        NewRow["FrontsOrdersID"] = row["FrontsOrdersID"];
                                        NewRow["FrontConfigID"] = row["FrontConfigID"];
                                        NewRow["FactoryID"] = row["FactoryID"];
                                        NewRow["PatinaID"] = row["PatinaID"];
                                        NewRow["FrontID"] = row["FrontID"];
                                        NewRow["InsetTypeID"] = row["InsetTypeID"];
                                        NewRow["ColorID"] = row["ColorID"];
                                        NewRow["InsetColorID"] = row["InsetColorID"];
                                        NewRow["TechnoProfileID"] = row["TechnoProfileID"];
                                        NewRow["TechnoColorID"] = row["TechnoColorID"];
                                        NewRow["TechnoInsetTypeID"] = row["TechnoInsetTypeID"];
                                        NewRow["TechnoInsetColorID"] = row["TechnoInsetColorID"];
                                        NewRow["Height"] = row["Height"];
                                        NewRow["Width"] = row["Width"];
                                        NewRow["PlanCount"] = PlanCount;
                                        Square = Convert.ToDecimal(row["Square"]) * PlanCount / Convert.ToDecimal(row["Count"]);
                                        Square = Decimal.Round(Square, 6, MidpointRounding.AwayFromZero);
                                        NewRow["Square"] = Square;
                                        NewRow["Notes"] = row["Notes"];
                                        DT.Rows.Add(NewRow);
                                    }
                                    TotalSquare += Square;
                                }
                                TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);
                                ControlAssignmentsManager.SetDyeingCartSquare(ControlAssignmentsManager.DyeingCartID, TotalSquare);
                                ControlAssignmentsManager.SaveDyeingCarts();
                            }
                        }
                        if (!bColumnDyeingCart)
                        {
                            decimal Square = 0;
                            decimal TotalSquare = 0;

                            //if (ControlAssignmentsManager.GroupType != 1)
                            //{
                            //    ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                            //    ControlAssignmentsManager.SaveDyeingCarts();
                            //    ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                            //}
                            ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                            ControlAssignmentsManager.SaveDyeingCarts();
                            ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                            int DyeingCartID = ControlAssignmentsManager.DyeingCartID;

                            foreach (DataRow row in TempBatchFrontsDT.Rows)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow["DyeingAssignmentID"] = ControlAssignmentsManager.DyeingAssignmentID;
                                NewRow["DyeingCartID"] = DyeingCartID;
                                NewRow["GroupType"] = row["GroupType"];
                                NewRow["WorkAssignmentID"] = row["WorkAssignmentID"];
                                NewRow["MegaBatchID"] = row["MegaBatchID"];
                                NewRow["BatchID"] = row["BatchID"];
                                NewRow["MainOrderID"] = row["MainOrderID"];
                                NewRow["FrontsOrdersID"] = row["FrontsOrdersID"];
                                NewRow["FrontConfigID"] = row["FrontConfigID"];
                                NewRow["FactoryID"] = row["FactoryID"];
                                NewRow["PatinaID"] = row["PatinaID"];
                                NewRow["FrontID"] = row["FrontID"];
                                NewRow["InsetTypeID"] = row["InsetTypeID"];
                                NewRow["ColorID"] = row["ColorID"];
                                NewRow["InsetColorID"] = row["InsetColorID"];
                                NewRow["TechnoProfileID"] = row["TechnoProfileID"];
                                NewRow["TechnoColorID"] = row["TechnoColorID"];
                                NewRow["TechnoInsetTypeID"] = row["TechnoInsetTypeID"];
                                NewRow["TechnoInsetColorID"] = row["TechnoInsetColorID"];
                                NewRow["Height"] = row["Height"];
                                NewRow["Width"] = row["Width"];
                                NewRow["PlanCount"] = row["Count"];
                                NewRow["Square"] = row["Square"];
                                NewRow["Notes"] = row["Notes"];
                                DT.Rows.Add(NewRow);
                                Square = Convert.ToDecimal(row["Square"]);
                                TotalSquare += Square;
                            }
                            if (ControlAssignmentsManager.GroupType != 1)
                            {
                                TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);
                                ControlAssignmentsManager.SetDyeingCartSquare(ControlAssignmentsManager.DyeingCartID, TotalSquare);
                                ControlAssignmentsManager.SaveDyeingCarts();
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveReOrders(ControlAssignments ControlAssignmentsManager)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentDetails WHERE DyeingAssignmentID=" + ControlAssignmentsManager.DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        decimal Square = 0;
                        decimal TotalSquare = 0;

                        ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.SaveDyeingCarts();
                        ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                        int DyeingCartID = ControlAssignmentsManager.DyeingCartID;

                        foreach (DataRow row in TempBatchFrontsDT.Rows)
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow["DyeingAssignmentID"] = ControlAssignmentsManager.DyeingAssignmentID;
                            NewRow["DyeingCartID"] = DyeingCartID;
                            NewRow["GroupType"] = row["GroupType"];
                            NewRow["WorkAssignmentID"] = row["WorkAssignmentID"];
                            NewRow["MegaBatchID"] = row["MegaBatchID"];
                            NewRow["BatchID"] = row["BatchID"];
                            NewRow["MainOrderID"] = row["MainOrderID"];
                            NewRow["FrontsOrdersID"] = row["FrontsOrdersID"];
                            NewRow["FrontConfigID"] = row["FrontConfigID"];
                            NewRow["FactoryID"] = row["FactoryID"];
                            NewRow["PatinaID"] = row["PatinaID"];
                            NewRow["FrontID"] = row["FrontID"];
                            NewRow["InsetTypeID"] = row["InsetTypeID"];
                            NewRow["ColorID"] = row["ColorID"];
                            NewRow["InsetColorID"] = row["InsetColorID"];
                            NewRow["TechnoProfileID"] = row["TechnoProfileID"];
                            NewRow["TechnoColorID"] = row["TechnoColorID"];
                            NewRow["TechnoInsetTypeID"] = row["TechnoInsetTypeID"];
                            NewRow["TechnoInsetColorID"] = row["TechnoInsetColorID"];
                            NewRow["Height"] = row["Height"];
                            NewRow["Width"] = row["Width"];
                            NewRow["PlanCount"] = row["Count"];
                            NewRow["Square"] = row["Square"];
                            NewRow["Notes"] = row["Notes"];
                            DT.Rows.Add(NewRow);
                            Square = Convert.ToDecimal(row["Square"]);
                            TotalSquare += Square;
                        }
                        TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);
                        ControlAssignmentsManager.SetDyeingCartSquare(ControlAssignmentsManager.DyeingCartID, TotalSquare);
                        ControlAssignmentsManager.SaveDyeingCarts();
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveZOV(ControlAssignments ControlAssignmentsManager, int MainOrderID)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentDetails WHERE DyeingAssignmentID=" + ControlAssignmentsManager.DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        decimal Square = 0;
                        decimal TotalSquare = 0;

                        ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.SaveDyeingCarts();
                        ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                        int DyeingCartID = ControlAssignmentsManager.DyeingCartID;

                        DataRow[] rows = TempBatchFrontsDT.Select("MainOrderID=" + MainOrderID);
                        foreach (DataRow row in rows)
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow["DyeingAssignmentID"] = ControlAssignmentsManager.DyeingAssignmentID;
                            NewRow["DyeingCartID"] = DyeingCartID;
                            NewRow["GroupType"] = row["GroupType"];
                            NewRow["WorkAssignmentID"] = row["WorkAssignmentID"];
                            NewRow["MegaBatchID"] = row["MegaBatchID"];
                            NewRow["BatchID"] = row["BatchID"];
                            NewRow["MainOrderID"] = row["MainOrderID"];
                            NewRow["FrontsOrdersID"] = row["FrontsOrdersID"];
                            NewRow["FrontConfigID"] = row["FrontConfigID"];
                            NewRow["FactoryID"] = row["FactoryID"];
                            NewRow["PatinaID"] = row["PatinaID"];
                            NewRow["FrontID"] = row["FrontID"];
                            NewRow["InsetTypeID"] = row["InsetTypeID"];
                            NewRow["ColorID"] = row["ColorID"];
                            NewRow["InsetColorID"] = row["InsetColorID"];
                            NewRow["TechnoProfileID"] = row["TechnoProfileID"];
                            NewRow["TechnoColorID"] = row["TechnoColorID"];
                            NewRow["TechnoInsetTypeID"] = row["TechnoInsetTypeID"];
                            NewRow["TechnoInsetColorID"] = row["TechnoInsetColorID"];
                            NewRow["Height"] = row["Height"];
                            NewRow["Width"] = row["Width"];
                            NewRow["PlanCount"] = row["Count"];
                            NewRow["Square"] = row["Square"];
                            NewRow["Notes"] = row["Notes"];
                            DT.Rows.Add(NewRow);
                            Square = Convert.ToDecimal(row["Square"]);
                            TotalSquare += Square;
                        }
                        TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);
                        ControlAssignmentsManager.SetDyeingCartSquare(ControlAssignmentsManager.DyeingCartID, TotalSquare);
                        ControlAssignmentsManager.SaveDyeingCarts();
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveMarketingReatailFronts(ControlAssignments ControlAssignmentsManager)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentDetails WHERE DyeingAssignmentID=" + ControlAssignmentsManager.DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bColumnDyeingCart = false;
                        foreach (DataColumn col in TempBatchFrontsDT.Columns)
                        {
                            string ColumnName = col.ColumnName;
                            if (ColumnName.Contains("ColumnDyeingCart"))
                            {
                                decimal Square = 0;
                                decimal TotalSquare = 0;

                                bColumnDyeingCart = true;
                                ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                                ControlAssignmentsManager.SaveDyeingCarts();
                                ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);

                                int DyeingCartID = ControlAssignmentsManager.DyeingCartID;

                                foreach (DataRow row in TempBatchFrontsDT.Rows)
                                {
                                    int FrontsOrdersID = Convert.ToInt32(row["FrontsOrdersID"]);
                                    if (row[ColumnName] == DBNull.Value || Convert.ToInt32(row[ColumnName]) == 0)
                                    {
                                        continue;
                                    }
                                    int PlanCount = Convert.ToInt32(row[ColumnName]);
                                    DataRow[] rows = DT.Select("DyeingCartID=" + DyeingCartID + " AND FrontsOrdersID=" + row["FrontsOrdersID"]);
                                    if (rows.Count() > 0)
                                    {
                                        rows[0]["PlanCount"] = PlanCount;
                                        Square = Convert.ToDecimal(row["Square"]) * PlanCount / Convert.ToDecimal(row["Count"]);
                                        Square = Decimal.Round(Square, 6, MidpointRounding.AwayFromZero);
                                        rows[0]["Square"] = Square;
                                        TotalSquare += Square;
                                    }
                                    else
                                    {
                                        DataRow NewRow = DT.NewRow();
                                        NewRow["DyeingAssignmentID"] = ControlAssignmentsManager.DyeingAssignmentID;
                                        NewRow["DyeingCartID"] = DyeingCartID;
                                        NewRow["GroupType"] = row["GroupType"];
                                        NewRow["WorkAssignmentID"] = row["WorkAssignmentID"];
                                        NewRow["MegaBatchID"] = row["MegaBatchID"];
                                        NewRow["BatchID"] = row["BatchID"];
                                        NewRow["MainOrderID"] = row["MainOrderID"];
                                        NewRow["FrontsOrdersID"] = row["FrontsOrdersID"];
                                        NewRow["FrontConfigID"] = row["FrontConfigID"];
                                        NewRow["FactoryID"] = row["FactoryID"];
                                        NewRow["PatinaID"] = row["PatinaID"];
                                        NewRow["FrontID"] = row["FrontID"];
                                        NewRow["InsetTypeID"] = row["InsetTypeID"];
                                        NewRow["ColorID"] = row["ColorID"];
                                        NewRow["InsetColorID"] = row["InsetColorID"];
                                        NewRow["TechnoProfileID"] = row["TechnoProfileID"];
                                        NewRow["TechnoColorID"] = row["TechnoColorID"];
                                        NewRow["TechnoInsetTypeID"] = row["TechnoInsetTypeID"];
                                        NewRow["TechnoInsetColorID"] = row["TechnoInsetColorID"];
                                        NewRow["Height"] = row["Height"];
                                        NewRow["Width"] = row["Width"];
                                        NewRow["PlanCount"] = PlanCount;
                                        Square = Convert.ToDecimal(row["Square"]) * PlanCount / Convert.ToDecimal(row["Count"]);
                                        Square = Decimal.Round(Square, 6, MidpointRounding.AwayFromZero);
                                        NewRow["Square"] = Square;
                                        NewRow["Notes"] = row["Notes"];
                                        DT.Rows.Add(NewRow);
                                    }
                                    TotalSquare += Square;
                                }
                                TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);
                                ControlAssignmentsManager.SetDyeingCartSquare(ControlAssignmentsManager.DyeingCartID, TotalSquare);
                                ControlAssignmentsManager.SaveDyeingCarts();
                            }
                        }
                        if (!bColumnDyeingCart)
                        {
                            decimal Square = 0;
                            decimal TotalSquare = 0;

                            if (ControlAssignmentsManager.GroupType != 1)
                            {
                                ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                                ControlAssignmentsManager.SaveDyeingCarts();
                                ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                            }

                            int DyeingCartID = ControlAssignmentsManager.DyeingCartID;

                            foreach (DataRow row in TempBatchFrontsDT.Rows)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow["DyeingAssignmentID"] = ControlAssignmentsManager.DyeingAssignmentID;
                                NewRow["DyeingCartID"] = DyeingCartID;
                                NewRow["GroupType"] = row["GroupType"];
                                NewRow["WorkAssignmentID"] = row["WorkAssignmentID"];
                                NewRow["MegaBatchID"] = row["MegaBatchID"];
                                NewRow["BatchID"] = row["BatchID"];
                                NewRow["MainOrderID"] = row["MainOrderID"];
                                NewRow["FrontsOrdersID"] = row["FrontsOrdersID"];
                                NewRow["FrontConfigID"] = row["FrontConfigID"];
                                NewRow["FactoryID"] = row["FactoryID"];
                                NewRow["PatinaID"] = row["PatinaID"];
                                NewRow["FrontID"] = row["FrontID"];
                                NewRow["InsetTypeID"] = row["InsetTypeID"];
                                NewRow["ColorID"] = row["ColorID"];
                                NewRow["InsetColorID"] = row["InsetColorID"];
                                NewRow["TechnoProfileID"] = row["TechnoProfileID"];
                                NewRow["TechnoColorID"] = row["TechnoColorID"];
                                NewRow["TechnoInsetTypeID"] = row["TechnoInsetTypeID"];
                                NewRow["TechnoInsetColorID"] = row["TechnoInsetColorID"];
                                NewRow["Height"] = row["Height"];
                                NewRow["Width"] = row["Width"];
                                NewRow["PlanCount"] = row["Count"];
                                NewRow["Square"] = row["Square"];
                                NewRow["Notes"] = row["Notes"];
                                DT.Rows.Add(NewRow);
                                Square = Convert.ToDecimal(row["Square"]);
                                TotalSquare += Square;
                            }
                            if (ControlAssignmentsManager.GroupType != 1)
                            {
                                TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);
                                ControlAssignmentsManager.SetDyeingCartSquare(ControlAssignmentsManager.DyeingCartID, TotalSquare);
                                ControlAssignmentsManager.SaveDyeingCarts();
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveDyeingFronts(ControlAssignments ControlAssignmentsManager)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentDetails WHERE DyeingAssignmentID=" + ControlAssignmentsManager.DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        foreach (DataColumn col in DyeingFrontsDT.Columns)
                        {
                            string ColumnName = col.ColumnName;
                            if (ColumnName.Contains("ColumnDyeingCart"))
                            {
                                decimal Square = 0;
                                decimal TotalSquare = 0;

                                string ColumnDyeingCart = ColumnName.Substring(6, ColumnName.Length - 6);
                                string s = ColumnName.Substring(16, ColumnName.Length - 16);
                                int DyeingCartID = Convert.ToInt32(s);
                                DataRow[] rows1 = DT.Select("DyeingCartID=" + DyeingCartID);
                                if (rows1.Count() == 0)
                                {
                                    ControlAssignmentsManager.CreateDyeingCart(ControlAssignmentsManager.DyeingAssignmentID);
                                    ControlAssignmentsManager.SaveDyeingCarts();
                                    ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                                    DyeingCartID = ControlAssignmentsManager.DyeingCartID;
                                }
                                foreach (DataRow row in DyeingFrontsDT.Rows)
                                {
                                    int FrontsOrdersID = Convert.ToInt32(row["FrontsOrdersID"]);
                                    if (row[ColumnName] == DBNull.Value || Convert.ToInt32(row[ColumnName]) == 0)
                                    {
                                        continue;
                                    }
                                    int PlanCount = Convert.ToInt32(row[ColumnName]);
                                    DataRow[] rows = DT.Select("DyeingCartID=" + DyeingCartID + " AND FrontsOrdersID=" + row["FrontsOrdersID"]);
                                    if (rows.Count() > 0)
                                    {
                                        rows[0]["PlanCount"] = PlanCount;
                                        Square = Convert.ToDecimal(row["Square"]) * PlanCount / Convert.ToDecimal(row["PlanCount"]);
                                        Square = Decimal.Round(Square, 6, MidpointRounding.AwayFromZero);
                                        rows[0]["Square"] = Square;
                                        TotalSquare += Square;
                                    }
                                    else
                                    {
                                        DataRow NewRow = DT.NewRow();
                                        NewRow["DyeingAssignmentID"] = row["DyeingAssignmentID"];
                                        NewRow["DyeingCartID"] = DyeingCartID;
                                        NewRow["GroupType"] = row["GroupType"];
                                        NewRow["WorkAssignmentID"] = row["WorkAssignmentID"];
                                        NewRow["MegaBatchID"] = row["MegaBatchID"];
                                        NewRow["BatchID"] = row["BatchID"];
                                        NewRow["MainOrderID"] = row["MainOrderID"];
                                        NewRow["FrontsOrdersID"] = row["FrontsOrdersID"];
                                        NewRow["FrontConfigID"] = row["FrontConfigID"];
                                        NewRow["FactoryID"] = row["FactoryID"];
                                        NewRow["PatinaID"] = row["PatinaID"];
                                        NewRow["FrontID"] = row["FrontID"];
                                        NewRow["InsetTypeID"] = row["InsetTypeID"];
                                        NewRow["ColorID"] = row["ColorID"];
                                        NewRow["InsetColorID"] = row["InsetColorID"];
                                        NewRow["TechnoProfileID"] = row["TechnoProfileID"];
                                        NewRow["TechnoColorID"] = row["TechnoColorID"];
                                        NewRow["TechnoInsetTypeID"] = row["TechnoInsetTypeID"];
                                        NewRow["TechnoInsetColorID"] = row["TechnoInsetColorID"];
                                        NewRow["Height"] = row["Height"];
                                        NewRow["Width"] = row["Width"];
                                        NewRow["PlanCount"] = PlanCount;
                                        Square = Convert.ToDecimal(row["Square"]) * PlanCount / Convert.ToDecimal(row["PlanCount"]);
                                        Square = Decimal.Round(Square, 6, MidpointRounding.AwayFromZero);
                                        NewRow["Square"] = Square;
                                        NewRow["Notes"] = row["Notes"];
                                        DT.Rows.Add(NewRow);
                                        TotalSquare += Square;
                                    }
                                }
                                TotalSquare = Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero);
                                ControlAssignmentsManager.SetDyeingCartSquare(ControlAssignmentsManager.DyeingCartID, TotalSquare);
                                ControlAssignmentsManager.SaveDyeingCarts();
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }
    }









    public class ControlAssignments
    {
        public ArrayList MainOrders = null;
        Barcode Barcode;

        bool bNewAssignment = false;

        int iDyeingAssignmentID = 0;
        int iDyeingCartID = 0;
        int iTechCatalogOperationsGroupID = 0;
        int iGroupType = 0;
        int iWorkAssignmentID = 0;
        int iFactoryID = 1;

        DataTable BatchesDT = null;
        DataTable BatchMainOrdersDT = null;
        DataTable SavedBatchMainOrdersDT = null;
        DataTable MegaBatchesDT = null;

        DataTable UsersDT = null;
        DataTable TechCatalogOperationsDetailDT = null;
        DataTable DyeingAssignmentBarcodesDT = null;
        DataTable DyeingAssignmentsDT = null;
        DataTable PrintedDyeingAssignmentsDetailsDT = null;
        DataTable DyeingCartsDT = null;
        DataTable WorkAssignmentsDT = null;

        BindingSource BatchesBS = null;
        BindingSource BatchMainOrdersBS = null;
        BindingSource MegaBatchesBS = null;
        BindingSource DyeingAssignmentsBS = null;
        BindingSource WorkAssignmentsBS = null;

        public bool NewAssignment
        {
            get { return bNewAssignment; }
            set { bNewAssignment = value; }
        }

        public int DyeingAssignmentID
        {
            get { return iDyeingAssignmentID; }
            set { iDyeingAssignmentID = value; }
        }

        public int DyeingCartID
        {
            get { return iDyeingCartID; }
            set { iDyeingCartID = value; }
        }

        public int TechCatalogOperationsGroupID
        {
            get { return iTechCatalogOperationsGroupID; }
            set { iTechCatalogOperationsGroupID = value; }
        }

        public int GroupType
        {
            get { return iGroupType; }
            set { iGroupType = value; }
        }

        public int WorkAssignmentID
        {
            get { return iWorkAssignmentID; }
            set { iWorkAssignmentID = value; }
        }

        public int FactoryID
        {
            get { return iFactoryID; }
            set { iFactoryID = value; }
        }

        public BindingSource BatchesList
        {
            get { return BatchesBS; }
        }

        public BindingSource BatchMainOrdersList
        {
            get { return BatchMainOrdersBS; }
        }

        public BindingSource MegaBatchesList
        {
            get { return MegaBatchesBS; }
        }

        public BindingSource WorkAssignmentsList
        {
            get { return WorkAssignmentsBS; }
        }

        public BindingSource DyeingAssignmentsList
        {
            get { return DyeingAssignmentsBS; }
        }

        public ControlAssignments()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        private void Create()
        {
            MainOrders = new ArrayList();
            Barcode = new DyeingAssignments.Barcode();
            BatchesDT = new DataTable();
            BatchMainOrdersDT = new DataTable();
            SavedBatchMainOrdersDT = new DataTable();
            MegaBatchesDT = new DataTable();
            UsersDT = new DataTable();
            TechCatalogOperationsDetailDT = new DataTable();
            DyeingAssignmentBarcodesDT = new DataTable();
            DyeingAssignmentsDT = new DataTable();
            PrintedDyeingAssignmentsDetailsDT = new DataTable();
            DyeingCartsDT = new DataTable();
            WorkAssignmentsDT = new DataTable();

            BatchesBS = new BindingSource();
            BatchMainOrdersBS = new BindingSource();
            MegaBatchesBS = new BindingSource();
            DyeingAssignmentsBS = new BindingSource();
            WorkAssignmentsBS = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM Batch",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchesDT);
            }
            string SelectCommand = @"SELECT TOP 0 ClientName, CONVERT(varchar(20), MegaOrders.OrderNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID
                INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS Client ON MegaOrders.ClientID = Client.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchMainOrdersDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MegaBatch",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MegaBatchesDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM Users",
                ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechCatalogOperationsDetail",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogOperationsDetailDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignmentBarcodes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentBarcodesDT);
            }
            SelectCommand = @"SELECT TOP 0 DyeingAssignments.*, infiniu2_catalog.dbo.TechCatalogOperationsGroups.GroupName FROM DyeingAssignments
                INNER JOIN infiniu2_catalog.dbo.TechCatalogOperationsGroups ON DyeingAssignments.TechCatalogOperationsGroupID=infiniu2_catalog.dbo.TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                ORDER BY DyeingAssignmentID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentsDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingCarts";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingCartsDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM WorkAssignments ORDER BY WorkAssignmentID DESC",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(WorkAssignmentsDT);
                WorkAssignmentsDT.Columns.Add(new DataColumn("Printed", Type.GetType("System.Boolean")));
            }
            MegaBatchesDT.Columns.Add(new DataColumn("Group", Type.GetType("System.String")));
            MegaBatchesDT.Columns.Add(new DataColumn("GroupType", Type.GetType("System.Int32")));
            BatchesDT.Columns.Add(new DataColumn("GroupType", Type.GetType("System.Int32")));
            BatchesDT.Columns.Add(new DataColumn(("FrontsSquare"), System.Type.GetType("System.Decimal")));
            BatchMainOrdersDT.Columns.Add(new DataColumn("GroupType", Type.GetType("System.Int32")));
        }

        private void Binding()
        {
            BatchesBS.DataSource = BatchesDT;
            BatchMainOrdersBS.DataSource = BatchMainOrdersDT;
            MegaBatchesBS.DataSource = MegaBatchesDT;
            DyeingAssignmentsBS.DataSource = DyeingAssignmentsDT;
            WorkAssignmentsBS.DataSource = WorkAssignmentsDT;
        }

        public DataGridViewComboBoxColumn OperationsGroupColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "OperationsGroupColumn",
                    HeaderText = "   Угол 45\r\nраспечатал",
                    DataPropertyName = "TechCatalogOperationsGroupID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "TechCatalogOperationsGroupID",
                    DisplayMember = "GroupName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CreationUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CreationUserColumn",
                    HeaderText = "Создал",
                    DataPropertyName = "CreationUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PrintUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PrintUserColumn",
                    HeaderText = "Распечатал",
                    DataPropertyName = "PrintUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn ResponsibleUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ResponsibleUserColumn",
                    HeaderText = "Ответственный",
                    DataPropertyName = "ResponsibleUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnologyUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnologyUserColumn",
                    HeaderText = "     Технолог.\r\nсопровождение",
                    DataPropertyName = "TechnologyUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn ControlUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ControlUserColumn",
                    HeaderText = "  Начальник\r\nпроизводства",
                    DataPropertyName = "ControlUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn AgreementUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "AgreementUserColumn",
                    HeaderText = "Согласовал",
                    DataPropertyName = "AgreementUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public void ClearBatchTables()
        {
            MegaBatchesDT.Clear();
            BatchesDT.Clear();
            BatchMainOrdersDT.Clear();
        }

        public void ClearDyeingTables()
        {
            DyeingAssignmentsDT.Clear();
            DyeingCartsDT.Clear();
        }

        public ArrayList DistinctMainOrders()
        {
            ArrayList array = new ArrayList();
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(BatchMainOrdersDT))
            {
                Table = DV.ToTable(true, new string[] { "MainOrderID" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                array.Add(Convert.ToInt32(Table.Rows[i]["MainOrderID"]));
            Table.Dispose();
            return array;
        }

        public void GetSavedMainOrders()
        {
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(BatchMainOrdersDT))
            {
                Table = DV.ToTable(true, new string[] { "MainOrderID" });
            }
            if (Table.Rows.Count == 0)
                return;
            int[] array = new int[Table.Rows.Count];
            for (int i = 0; i < Table.Rows.Count; i++)
                array[i] = Convert.ToInt32(Table.Rows[i]["MainOrderID"]);
            string SelectCommand = @"SELECT MainOrderID FROM DyeingAssignmentDetails WHERE MainOrderID IN (" + string.Join(",", array) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                SavedBatchMainOrdersDT.Clear();
                DA.Fill(SavedBatchMainOrdersDT);
            }
        }

        public bool IsMainOrderSaved(int MainOrderID)
        {
            DataRow[] rows = SavedBatchMainOrdersDT.Select("MainOrderID=" + MainOrderID);
            return rows.Count() > 0;
        }

        public void GetDyeingAssignmentsInfo(int DyeingAssignmentID)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();
            SelectCommand = @"SELECT * FROM DyeingAssignments WHERE DyeingAssignmentID=" + DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
                if (DT.Rows.Count > 0)
                    iGroupType = Convert.ToInt32(DT.Rows[0]["GroupType"]);
            }
        }

        public void UpdateMegaBatches(int DyeingAssignmentID)
        {
            string SelectCommand = @"SELECT * FROM MegaBatch WHERE MegaBatchID IN (SELECT MegaBatchID FROM infiniu2_storage.dbo.DyeingAssignmentDetails 
                WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")";
            DataTable DT = new DataTable();
            MegaBatchesDT.Clear();

            switch (iGroupType)
            {
                case 0:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = MegaBatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["Group"] = "З";
                        NewRow["GroupType"] = 0;
                        MegaBatchesDT.Rows.Add(NewRow);
                    }
                    break;
                case 1:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = MegaBatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["Group"] = "М";
                        NewRow["GroupType"] = 1;
                        MegaBatchesDT.Rows.Add(NewRow);
                    }
                    break;
                case 2:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = MegaBatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["Group"] = "М";
                        NewRow["GroupType"] = 1;
                        MegaBatchesDT.Rows.Add(NewRow);
                    }
                    break;
                case 3:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = MegaBatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["Group"] = "З";
                        NewRow["GroupType"] = 0;
                        MegaBatchesDT.Rows.Add(NewRow);
                    }
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = MegaBatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["Group"] = "М";
                        NewRow["GroupType"] = 1;
                        MegaBatchesDT.Rows.Add(NewRow);
                    }
                    break;
                case 4:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = MegaBatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["Group"] = "З";
                        NewRow["GroupType"] = 0;
                        MegaBatchesDT.Rows.Add(NewRow);
                    }
                    break;
                default:
                    break;
            }
            DT.Dispose();
        }

        public void UpdateMegaBatches(int WorkAssignmentID, int FactoryID)
        {
            string SelectCommand = string.Empty;
            string WorkAssignment = "ProfilWorkAssignmentID=";
            DataTable DT = new DataTable();
            if (FactoryID == 2)
                WorkAssignment = "TPSWorkAssignmentID=";
            SelectCommand = @"SELECT * FROM MegaBatch WHERE MegaBatchID IN (SELECT MegaBatchID FROM Batch WHERE " + WorkAssignment + WorkAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DT);
            }
            MegaBatchesDT.Clear();
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = MegaBatchesDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                NewRow["Group"] = "З";
                NewRow["GroupType"] = 0;
                MegaBatchesDT.Rows.Add(NewRow);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = MegaBatchesDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                NewRow["Group"] = "М";
                NewRow["GroupType"] = 1;
                MegaBatchesDT.Rows.Add(NewRow);
            }
        }

        public void UpdateBatches(int DyeingAssignmentID)
        {
            for (int i = BatchesDT.Columns.Count - 1; i >= 0; i--)
            {
                string ColumnName = BatchesDT.Columns[i].ColumnName;
                if (ColumnName.Contains("ColumnDyeingCart"))
                    BatchesDT.Columns.RemoveAt(i);
            }

            string SelectCommand = @"SELECT * FROM Batch WHERE BatchID IN (SELECT BatchID FROM infiniu2_storage.dbo.DyeingAssignmentDetails
                WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")";
            DataTable DT = new DataTable();
            BatchesDT.Clear();

            switch (iGroupType)
            {
                case 0:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 0;
                        BatchesDT.Rows.Add(NewRow);
                    }
                    break;
                case 1:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 1;
                        BatchesDT.Rows.Add(NewRow);
                    }

                    SelectCommand = @"SELECT * FROM DyeingCarts WHERE DyeingAssignmentID=" + DyeingAssignmentID;
                    DT.Dispose();
                    DT = new DataTable();
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        decimal Square = Convert.ToDecimal(DT.Rows[i]["Square"]);
                        string ColumnName = "ColumnDyeingCart" + DT.Rows[i]["DyeingCartID"];
                        if (!BatchesDT.Columns.Contains(ColumnName))
                            BatchesDT.Columns.Add(new DataColumn((ColumnName), System.Type.GetType("System.Decimal")));
                        for (int j = 0; j < BatchesDT.Rows.Count; j++)
                        {
                            BatchesDT.Rows[j][ColumnName] = Square;
                        }
                    }
                    break;
                case 2:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 1;
                        BatchesDT.Rows.Add(NewRow);
                    }
                    break;
                case 3:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 0;
                        BatchesDT.Rows.Add(NewRow);
                    }
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 1;
                        BatchesDT.Rows.Add(NewRow);
                    }
                    break;
                case 4:
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchesDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 0;
                        BatchesDT.Rows.Add(NewRow);
                    }
                    break;
                default:
                    break;
            }
            DT.Dispose();
        }

        public void UpdateBatches(int WorkAssignmentID, int FactoryID)
        {
            for (int i = BatchesDT.Columns.Count - 1; i >= 0; i--)
            {
                string ColumnName = BatchesDT.Columns[i].ColumnName;
                if (ColumnName.Contains("ColumnDyeingCart"))
                    BatchesDT.Columns.RemoveAt(i);
            }
            string SelectCommand = string.Empty;
            string WorkAssignment = "ProfilWorkAssignmentID=";
            DataTable DT = new DataTable();
            if (FactoryID == 2)
                WorkAssignment = "TPSWorkAssignmentID=";
            SelectCommand = @"SELECT * FROM Batch WHERE " + WorkAssignment + WorkAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DT);
            }
            BatchesDT.Clear();
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = BatchesDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                NewRow["GroupType"] = 0;
                BatchesDT.Rows.Add(NewRow);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = BatchesDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                NewRow["GroupType"] = 1;
                BatchesDT.Rows.Add(NewRow);
            }
        }

        public void UpdateBatchMainOrders(int DyeingAssignmentID)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();
            BatchMainOrdersDT.Clear();

            switch (iGroupType)
            {
                case 0:
                    SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MainOrders.DocNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID" +
                        @" INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                        @" INNER JOIN infiniu2_zovreference.dbo.Clients AS Client ON MainOrders.ClientID = Client.ClientID " +
                        @" WHERE MainOrders.MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")" +
                        @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchMainOrdersDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 0;
                        BatchMainOrdersDT.Rows.Add(NewRow);
                    }
                    break;
                case 1:
                    SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MegaOrders.OrderNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID" +
                        @" INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                        @" INNER JOIN infiniu2_marketingreference.dbo.Clients AS Client ON MegaOrders.ClientID = Client.ClientID" +
                        @" WHERE MainOrders.MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")" +
                        @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchMainOrdersDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 1;
                        BatchMainOrdersDT.Rows.Add(NewRow);
                    }
                    break;
                case 2:
                    SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MegaOrders.OrderNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID " +
                        @" INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                        @" INNER JOIN infiniu2_marketingreference.dbo.Clients AS Client ON MegaOrders.ClientID = Client.ClientID" +
                        @" WHERE MainOrders.MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")" +
                        @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchMainOrdersDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 1;
                        BatchMainOrdersDT.Rows.Add(NewRow);
                    }
                    break;
                case 3:
                    SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MainOrders.DocNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID  " +
                        @" INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                        @" INNER JOIN infiniu2_zovreference.dbo.Clients AS Client ON MainOrders.ClientID = Client.ClientID " +
                        @" WHERE MainOrders.MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")" +
                        @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchMainOrdersDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 0;
                        BatchMainOrdersDT.Rows.Add(NewRow);
                    }
                    SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MegaOrders.OrderNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID " +
                        @" INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                        @" INNER JOIN infiniu2_marketingreference.dbo.Clients AS Client ON MegaOrders.ClientID = Client.ClientID " +
                        @" WHERE MainOrders.MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")" +
                        @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchMainOrdersDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 1;
                        BatchMainOrdersDT.Rows.Add(NewRow);
                    }
                    break;
                case 4:
                    SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MainOrders.DocNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID " +
                        @" INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                        @" INNER JOIN infiniu2_zovreference.dbo.Clients AS Client ON MainOrders.ClientID = Client.ClientID " +
                        @" WHERE MainOrders.MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")" +
                        @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        DT.Clear();
                        DA.Fill(DT);
                    }
                    foreach (DataRow item in DT.Rows)
                    {
                        DataRow NewRow = BatchMainOrdersDT.NewRow();
                        NewRow.ItemArray = item.ItemArray;
                        NewRow["GroupType"] = 0;
                        BatchMainOrdersDT.Rows.Add(NewRow);
                    }
                    break;
                default:
                    break;
            }
            DT.Dispose();
        }

        public void UpdateBatchMainOrders(int WorkAssignmentID, int FactoryID)
        {
            string SelectCommand = string.Empty;
            string WorkAssignment = "Batch.ProfilWorkAssignmentID=";
            DataTable DT = new DataTable();
            if (FactoryID == 2)
                WorkAssignment = "Batch.TPSWorkAssignmentID=";
            SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MegaOrders.OrderNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID AND " + WorkAssignment + WorkAssignmentID +
                @" INNER JOIN infiniu2_marketingreference.dbo.Clients AS Client ON MegaOrders.ClientID = Client.ClientID " + 
                @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DT);
            }
            BatchMainOrdersDT.Clear();
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = BatchMainOrdersDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                NewRow["GroupType"] = 1;
                BatchMainOrdersDT.Rows.Add(NewRow);
            }
            SelectCommand = @"SELECT ClientName, CONVERT(varchar(20), MainOrders.DocNumber) AS OrderNumber, MainOrders.MainOrderID, MainOrders.FrontsSquare, MainOrders.Notes, Batch.BatchID FROM MainOrders
                INNER JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID AND " + WorkAssignment + WorkAssignmentID +
                @" INNER JOIN infiniu2_zovreference.dbo.Clients AS Client ON MainOrders.ClientID = Client.ClientID " + 
                @" ORDER BY ClientName, OrderNumber, MainOrders.MainOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = BatchMainOrdersDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                NewRow["GroupType"] = 0;
                BatchMainOrdersDT.Rows.Add(NewRow);
            }
        }

        public void SumBatchFrontsSquare()
        {
            for (int i = 0; i < BatchesDT.Rows.Count; i++)
            {
                int BatchID = Convert.ToInt32(BatchesDT.Rows[i]["BatchID"]);
                decimal Square = 0;
                DataRow[] mrows = BatchMainOrdersDT.Select("BatchID=" + BatchID);
                foreach (DataRow item in mrows)
                {
                    if (item["FrontsSquare"] != DBNull.Value)
                        Square += Convert.ToDecimal(item["FrontsSquare"]);
                }
                BatchesDT.Rows[i]["FrontsSquare"] = Square;
            }
        }

        public void FilterMegaBatches(int GroupType)
        {
            switch (GroupType)
            {
                case 0:
                    MegaBatchesBS.Filter = "GroupType=0";
                    break;
                case 1:
                    MegaBatchesBS.Filter = "GroupType=1";
                    break;
                case 2:
                    MegaBatchesBS.Filter = "GroupType=1";
                    break;
                case 3:
                    MegaBatchesBS.RemoveFilter();
                    break;
                case 4:
                    MegaBatchesBS.Filter = "GroupType=0";
                    break;
                default:
                    break;
            }
            MegaBatchesBS.MoveFirst();
        }

        public void FilterBatchesByMegaBatch(int GroupType, int MegaBatchID)
        {
            BatchesBS.Filter = "GroupType=" + GroupType + " AND MegaBatchID=" + MegaBatchID;
            BatchesBS.MoveFirst();
        }

        public void FilterBatchesByMegaBatch()
        {
            BatchesBS.RemoveFilter();
            BatchesBS.MoveFirst();
        }

        public void FilterBatchMainOrders(int GroupType, int BatchID)
        {
            BatchMainOrdersBS.Filter = "GroupType=" + GroupType + " AND BatchID=" + BatchID;
            BatchMainOrdersBS.MoveFirst();
        }

        public void FilterBatchMainOrders()
        {
            BatchMainOrdersBS.RemoveFilter();
            BatchMainOrdersBS.MoveFirst();
        }

        public void SaveWorkAssignments()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM WorkAssignments",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(WorkAssignmentsDT);
                }
            }
        }

        public int UpdateWorkAssignments(int FactoryID)
        {
            int MaxWorkAssignmentID = 0;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM WorkAssignments ORDER BY WorkAssignmentID DESC",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                WorkAssignmentsDT.Clear();
                DA.Fill(WorkAssignmentsDT);
            }
            if (WorkAssignmentsDT.Rows.Count > 0 && WorkAssignmentsDT.Rows[0]["WorkAssignmentID"] != DBNull.Value)
                MaxWorkAssignmentID = Convert.ToInt32(WorkAssignmentsDT.Rows[0]["WorkAssignmentID"]);
            return MaxWorkAssignmentID;
        }

        public bool IsDyeingAssignmentBarcodeExist(int DyeingAssignmentID, int DyeingCartID, int TechCatalogOperationsGroupID, int TechCatalogOperationsDetailID)
        {
            DataRow[] rows = DyeingAssignmentBarcodesDT.Select("DyeingAssignmentID=" + DyeingAssignmentID + " AND DyeingCartID=" + DyeingCartID +
                " AND TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID + " AND TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            return rows.Count() > 0;
        }

        public void RemoveDyeingAssignmentBarcode(int DyeingAssignmentID)
        {
            DataRow[] rows = DyeingAssignmentBarcodesDT.Select("DyeingAssignmentID=" + DyeingAssignmentID);
            for (int i = rows.Count() - 1; i >= 0; i--)
            {
                rows[i].Delete();
            }
        }

        public void RemoveDyeingAssignmentBarcode(int DyeingAssignmentID, int TechCatalogOperationsGroupID)
        {
            DataRow[] rows = DyeingAssignmentBarcodesDT.Select("DyeingAssignmentID=" + DyeingAssignmentID);
            for (int i = rows.Count() - 1; i >= 0; i--)
            {
                if (Convert.ToInt32(rows[i]["TechCatalogOperationsGroupID"]) != TechCatalogOperationsGroupID)
                    rows[i].Delete();
            }
        }

        public void CreateDyeingAssignmentBarcode(int DyeingAssignmentID, int TechCatalogOperationsGroupID)
        {
            DateTime CreationDateTime = Security.GetCurrentDate();
            for (int i = 0; i < DyeingCartsDT.Rows.Count; i++)
            {
                for (int j = 0; j < TechCatalogOperationsDetailDT.Rows.Count; j++)
                {
                    int DyeingCartID = Convert.ToInt32(DyeingCartsDT.Rows[i]["DyeingCartID"]);
                    int TechCatalogOperationsDetailID = Convert.ToInt32(TechCatalogOperationsDetailDT.Rows[j]["TechCatalogOperationsDetailID"]);
                    if (IsDyeingAssignmentBarcodeExist(DyeingAssignmentID, DyeingCartID, TechCatalogOperationsGroupID, TechCatalogOperationsDetailID))
                        continue;
                    DataRow NewRow = DyeingAssignmentBarcodesDT.NewRow();
                    NewRow["DyeingAssignmentID"] = DyeingAssignmentID;
                    NewRow["DyeingCartID"] = DyeingCartID;
                    NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                    NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                    NewRow["CreationUserID"] = Security.CurrentUserID;
                    NewRow["CreationDateTime"] = CreationDateTime;
                    DyeingAssignmentBarcodesDT.Rows.Add(NewRow);
                }
            }
        }

        public void CreateDyeingAssignmentBarcode(int DyeingAssignmentID, int TechCatalogOperationsGroupID, int TechCatalogOperationsDetailID)
        {
            DateTime CreationDateTime = Security.GetCurrentDate();
            for (int i = 0; i < DyeingCartsDT.Rows.Count; i++)
            {
                int DyeingCartID = Convert.ToInt32(DyeingCartsDT.Rows[i]["DyeingCartID"]);
                if (IsDyeingAssignmentBarcodeExist(DyeingAssignmentID, DyeingCartID, TechCatalogOperationsGroupID, TechCatalogOperationsDetailID))
                    continue;
                DataRow NewRow = DyeingAssignmentBarcodesDT.NewRow();
                NewRow["DyeingAssignmentID"] = DyeingAssignmentID;
                NewRow["DyeingCartID"] = DyeingCartID;
                NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                NewRow["CreationUserID"] = Security.CurrentUserID;
                NewRow["CreationDateTime"] = CreationDateTime;
                DyeingAssignmentBarcodesDT.Rows.Add(NewRow);
            }
        }

        public void CreateDyeingAssignment(int TechCatalogOperationsGroupID)
        {
            DataRow NewRow = DyeingAssignmentsDT.NewRow();
            NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
            NewRow["GroupType"] = iGroupType;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            DyeingAssignmentsDT.Rows.Add(NewRow);
        }

        public void CreateDyeingCart(int DyeingAssignmentID, decimal Square = 0)
        {
            int CartNumber = 0;
            if (DyeingCartsDT.Rows.Count > 0)
            {
                CartNumber = Convert.ToInt32(DyeingCartsDT.Rows[0]["CartNumber"]);
                for (int i = 1; i < DyeingCartsDT.Rows.Count; i++)
                {
                    if (Convert.ToInt32(DyeingCartsDT.Rows[i]["CartNumber"]) > CartNumber)
                        CartNumber = Convert.ToInt32(DyeingCartsDT.Rows[i]["CartNumber"]);
                }
            }
            DataRow NewRow = DyeingCartsDT.NewRow();
            NewRow["DyeingAssignmentID"] = DyeingAssignmentID;
            NewRow["Square"] = Square;
            NewRow["CartNumber"] = ++CartNumber;
            DyeingCartsDT.Rows.Add(NewRow);
        }

        public decimal GetDyeingCartSquare(int DyeingCartID)
        {
            decimal Square = 0;
            DataRow[] rows = DyeingCartsDT.Select("DyeingCartID=" + DyeingCartID);
            if (rows.Count() > 0 && rows[0]["Square"] != DBNull.Value)
                Square = Convert.ToDecimal(rows[0]["Square"]);
            return Square;
        }

        public void SetDyeingCartSquare(int DyeingCartID, decimal Square)
        {
            DataRow[] rows = DyeingCartsDT.Select("DyeingCartID=" + DyeingCartID);
            if (rows.Count() == 0)
                return;
            rows[0]["Square"] = Square;
        }

        public void SetDyeingAssignmentPermissions(int WorkAssignmentID, int Permission)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM DyeingAssignments WHERE DyeingAssignmentID IN
                (SELECT DyeingAssignmentID FROM DyeingAssignmentDetails WHERE WorkAssignmentID=" + WorkAssignmentID + ")",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DateTime CurrentDateTime = Security.GetCurrentDate();
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (Permission == 0)
                                {
                                    if (DT.Rows[i]["ResponsibleUserID"] == DBNull.Value)
                                        DT.Rows[i]["ResponsibleUserID"] = Security.CurrentUserID;
                                    if (DT.Rows[i]["ResponsibleDateTime"] == DBNull.Value)
                                        DT.Rows[i]["ResponsibleDateTime"] = CurrentDateTime;
                                }
                                if (Permission == 1)
                                {
                                    if (DT.Rows[i]["TechnologyUserID"] == DBNull.Value)
                                        DT.Rows[i]["TechnologyUserID"] = Security.CurrentUserID;
                                    if (DT.Rows[i]["TechnologyDateTime"] == DBNull.Value)
                                        DT.Rows[i]["TechnologyDateTime"] = CurrentDateTime;
                                }
                                if (Permission == 2)
                                {
                                    if (DT.Rows[i]["ControlUserID"] == DBNull.Value)
                                        DT.Rows[i]["ControlUserID"] = Security.CurrentUserID;
                                    if (DT.Rows[i]["ControlDateTime"] == DBNull.Value)
                                        DT.Rows[i]["ControlDateTime"] = CurrentDateTime;
                                }
                                if (Permission == 3)
                                {
                                    if (DT.Rows[i]["AgreementUserID"] == DBNull.Value)
                                        DT.Rows[i]["AgreementUserID"] = Security.CurrentUserID;
                                    if (DT.Rows[i]["AgreementDateTime"] == DBNull.Value)
                                        DT.Rows[i]["AgreementDateTime"] = CurrentDateTime;
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
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

        public void SetDyeingAssignmentPermissions2(int DyeingAssignmentID, int Permission)
        {
            DataRow[] rows = DyeingAssignmentsDT.Select("DyeingAssignmentID=" + DyeingAssignmentID);
            if (rows.Count() == 0)
                return;
            if (Permission == 0)
            {
                if (rows[0]["ResponsibleUserID"] == DBNull.Value)
                    rows[0]["ResponsibleUserID"] = Security.CurrentUserID;
                if (rows[0]["ResponsibleDateTime"] == DBNull.Value)
                    rows[0]["ResponsibleDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 1)
            {
                if (rows[0]["TechnologyUserID"] == DBNull.Value)
                    rows[0]["TechnologyUserID"] = Security.CurrentUserID;
                if (rows[0]["TechnologyDateTime"] == DBNull.Value)
                    rows[0]["TechnologyDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 2)
            {
                if (rows[0]["ControlUserID"] == DBNull.Value)
                    rows[0]["ControlUserID"] = Security.CurrentUserID;
                if (rows[0]["ControlDateTime"] == DBNull.Value)
                    rows[0]["ControlDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 3)
            {
                if (rows[0]["AgreementUserID"] == DBNull.Value)
                    rows[0]["AgreementUserID"] = Security.CurrentUserID;
                if (rows[0]["AgreementDateTime"] == DBNull.Value)
                    rows[0]["AgreementDateTime"] = Security.GetCurrentDate();
            }
        }

        public void PrintDyeingAssignment(int DyeingAssignmentID)
        {
            DataRow[] rows = DyeingAssignmentsDT.Select("DyeingAssignmentID=" + DyeingAssignmentID);
            if (rows.Count() == 0)
                return;
            if (rows[0]["PrintUserID"] == DBNull.Value)
                rows[0]["PrintUserID"] = Security.CurrentUserID;
            if (rows[0]["PrintDateTime"] == DBNull.Value)
                rows[0]["PrintDateTime"] = Security.GetCurrentDate();
        }

        public bool GetDyeingAssignmentStatus(int DyeingAssignmentID)
        {
            bool IsAgreed = false;
            DataRow[] rows = DyeingAssignmentsDT.Select("DyeingAssignmentID=" + DyeingAssignmentID);
            if (rows.Count() > 0 && rows[0]["AgreementDateTime"] != DBNull.Value)
                IsAgreed = true;
            return IsAgreed;
        }

        public bool GetWorkAssignmentStatus(int WorkAssignmentID)
        {
            bool IsAgreed = false;
            DataRow[] rows = WorkAssignmentsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
            if (rows.Count() > 0 && rows[0]["ResponsibleDateTime"] != DBNull.Value || rows[0]["TechnologyDateTime"] != DBNull.Value
                && rows[0]["ControlDateTime"] != DBNull.Value && rows[0]["AgreementDateTime"] != DBNull.Value)
                IsAgreed = true;
            return IsAgreed;
        }

        //public void SetWorkAssignmentResponsible(int WorkAssignmentID)
        //{
        //    DataRow[] rows = WorkAssignmentsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
        //    if (rows.Count() == 0)
        //        return;
        //    rows[0]["ResponsibleUserID"] = Security.CurrentUserID;
        //    rows[0]["ResponsibleDateTime"] = Security.GetCurrentDate();
        //}

        //public void SetWorkAssignmentTechnology(int WorkAssignmentID)
        //{
        //    DataRow[] rows = WorkAssignmentsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
        //    if (rows.Count() == 0)
        //        return;
        //    rows[0]["TechnologyUserID"] = Security.CurrentUserID;
        //    rows[0]["TechnologyDateTime"] = Security.GetCurrentDate();
        //}

        //public void SetWorkAssignmentControl(int WorkAssignmentID)
        //{
        //    DataRow[] rows = WorkAssignmentsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
        //    if (rows.Count() == 0)
        //        return;
        //    rows[0]["ControlUserID"] = Security.CurrentUserID;
        //    rows[0]["ControlDateTime"] = Security.GetCurrentDate();
        //}

        //public void SetWorkAssignmentAgreement(int WorkAssignmentID)
        //{
        //    DataRow[] rows = WorkAssignmentsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
        //    if (rows.Count() == 0)
        //        return;
        //    rows[0]["AgreementUserID"] = Security.CurrentUserID;
        //    rows[0]["AgreementDateTime"] = Security.GetCurrentDate();
        //}

        public void SetWorkAssignmentAgreementPermissions(int WorkAssignmentID, int Permission)
        {
            DataRow[] rows = WorkAssignmentsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
            if (rows.Count() == 0)
                return;
            if (Permission == 0)
            {
                if (rows[0]["ResponsibleUserID"] == DBNull.Value)
                    rows[0]["ResponsibleUserID"] = Security.CurrentUserID;
                if (rows[0]["ResponsibleDateTime"] == DBNull.Value)
                    rows[0]["ResponsibleDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 1)
            {
                if (rows[0]["TechnologyUserID"] == DBNull.Value)
                    rows[0]["TechnologyUserID"] = Security.CurrentUserID;
                if (rows[0]["TechnologyDateTime"] == DBNull.Value)
                    rows[0]["TechnologyDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 2)
            {
                if (rows[0]["ControlUserID"] == DBNull.Value)
                    rows[0]["ControlUserID"] = Security.CurrentUserID;
                if (rows[0]["ControlDateTime"] == DBNull.Value)
                    rows[0]["ControlDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 3)
            {
                if (rows[0]["AgreementUserID"] == DBNull.Value)
                    rows[0]["AgreementUserID"] = Security.CurrentUserID;
                if (rows[0]["AgreementDateTime"] == DBNull.Value)
                    rows[0]["AgreementDateTime"] = Security.GetCurrentDate();
            }
        }

        public void ChangeOperationsGroup(int DyeingAssignmentID, int TechCatalogOperationsGroupID)
        {
            DataRow[] rows = DyeingAssignmentsDT.Select("DyeingAssignmentID=" + DyeingAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DyeingAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DyeingAssignmentsDT);
                }
            }
        }

        public void SaveDyeingAssignmentBarcodes()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DyeingAssignmentBarcodes",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DyeingAssignmentBarcodesDT);
                }
            }
        }

        public void SaveDyeingAssignments()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DyeingAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DyeingAssignmentsDT);
                }
            }
        }

        public void SaveDyeingCarts()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DyeingCarts",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DyeingCartsDT);
                }
            }
        }

        public void UpdateTechCatalogOperationsDetail(int TechCatalogOperationsGroupID)
        {
            string SelectCommand = @"SELECT * FROM TechCatalogOperationsDetail WHERE TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogOperationsDetailDT.Clear();
                DA.Fill(TechCatalogOperationsDetailDT);
            }
        }

        public void UpdateDyeingAssignmentBarcodes(int DyeingAssignmentID)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentBarcodes WHERE DyeingAssignmentID=" + DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentBarcodesDT.Clear();
                DA.Fill(DyeingAssignmentBarcodesDT);
            }
        }

        public void UpdateDyeingAssignments(DateTime DateFrom, DateTime DateTo)
        {
            //            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DyeingAssignments.*, infiniu2_catalog.dbo.TechCatalogOperationsGroups.GroupName FROM DyeingAssignments
            //                INNER JOIN infiniu2_catalog.dbo.TechCatalogOperationsGroups ON DyeingAssignments.TechCatalogOperationsGroupID=infiniu2_catalog.dbo.TechCatalogOperationsGroups.TechCatalogOperationsGroupID
            //                WHERE DyeingAssignmentID IN (SELECT TOP 100 DyeingAssignmentID FROM DyeingAssignments 
            //                ORDER BY DyeingAssignmentID DESC) ORDER BY DyeingAssignmentID DESC", ConnectionStrings.StorageConnectionString))
            //            {
            //                DyeingAssignmentsDT.Clear();
            //                DA.Fill(DyeingAssignmentsDT);
            //            }
            string SelectCommand = @"SELECT DyeingAssignments.*, infiniu2_catalog.dbo.TechCatalogOperationsGroups.GroupName FROM DyeingAssignments
                INNER JOIN infiniu2_catalog.dbo.TechCatalogOperationsGroups ON DyeingAssignments.TechCatalogOperationsGroupID=infiniu2_catalog.dbo.TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                WHERE CAST(CreationDateTime AS DATE) >= '" + DateFrom.ToString("yyyy-MM-dd") +
                "' AND CAST(CreationDateTime AS DATE) <= '" + DateTo.ToString("yyyy-MM-dd") + "' ORDER BY DyeingAssignmentID DESC";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentsDT.Clear();
                DA.Fill(DyeingAssignmentsDT);
                if (DyeingAssignmentsDT.Rows.Count > 0)
                    iDyeingAssignmentID = Convert.ToInt32(DyeingAssignmentsDT.Rows[0]["DyeingAssignmentID"]);
                else
                    iDyeingAssignmentID = 0;
            }
            //            SelectCommand = @"SELECT DISTINCT WorkAssignmentID, GroupType FROM DyeingAssignmentDetails
            //                WHERE DyeingAssignmentID IN (SELECT DyeingAssignmentID FROM DyeingAssignments WHERE PrintDateTime IS NULL)";
            //            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //            {
            //                PrintedDyeingAssignmentsDetailsDT.Clear();
            //                DA.Fill(PrintedDyeingAssignmentsDetailsDT);
            //            }
        }

        public void IsWorkAssignmentPrinted()
        {
            string filter = string.Empty;
            foreach (DataRow item in WorkAssignmentsDT.Rows)
                filter += Convert.ToInt32(item["WorkAssignmentID"]).ToString() + ",";
            if (filter.Length > 0)
                filter = " WHERE WorkAssignmentID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            string SelectCommand = @"SELECT WorkAssignmentID, DyeingAssignmentDetails.DyeingAssignmentID, DyeingAssignments.PrintDateTime FROM DyeingAssignmentDetails
                INNER JOIN DyeingAssignments ON DyeingAssignmentDetails.DyeingAssignmentID=DyeingAssignments.DyeingAssignmentID" + filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                PrintedDyeingAssignmentsDetailsDT.Clear();
                DA.Fill(PrintedDyeingAssignmentsDetailsDT);
            }

            for (int i = 0; i < WorkAssignmentsDT.Rows.Count; i++)
            {
                WorkAssignmentID = Convert.ToInt32(WorkAssignmentsDT.Rows[i]["WorkAssignmentID"]);

                //                SelectCommand = @"SELECT WorkAssignmentID, DyeingAssignmentDetails.DyeingAssignmentID, DyeingAssignments.PrintDateTime FROM DyeingAssignmentDetails
                //                INNER JOIN DyeingAssignments ON DyeingAssignmentDetails.DyeingAssignmentID=DyeingAssignments.DyeingAssignmentID
                //                WHERE WorkAssignmentID=" + WorkAssignmentID;
                //                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                //                {
                //                    PrintedDyeingAssignmentsDetailsDT.Clear();
                //                    DA.Fill(PrintedDyeingAssignmentsDetailsDT);
                //                    if (PrintedDyeingAssignmentsDetailsDT.Rows.Count == 0)
                //                        WorkAssignmentsDT.Rows[i]["Printed"] = false;
                //                    else
                //                    {
                //                        if (PrintedDyeingAssignmentsDetailsDT.Rows[0]["PrintDateTime"] != DBNull.Value)
                //                            WorkAssignmentsDT.Rows[i]["Printed"] = true;
                //                        else
                //                            WorkAssignmentsDT.Rows[i]["Printed"] = false;
                //                    }
                //                }

                DataRow[] rows = PrintedDyeingAssignmentsDetailsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
                if (rows.Count() == 0)
                    WorkAssignmentsDT.Rows[i]["Printed"] = false;
                else
                {
                    if (rows[0]["PrintDateTime"] != DBNull.Value)
                        WorkAssignmentsDT.Rows[i]["Printed"] = true;
                    else
                        WorkAssignmentsDT.Rows[i]["Printed"] = false;
                }
            }
        }

        public void UpdateDyeingCarts(int DyeingAssignmentID)
        {
            string SelectCommand = @"SELECT * FROM DyeingCarts WHERE DyeingAssignmentID=" + DyeingAssignmentID + " ORDER BY CartNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingCartsDT.Clear();
                DA.Fill(DyeingCartsDT);
                if (DyeingCartsDT.Rows.Count > 0)
                    iDyeingCartID = Convert.ToInt32(DyeingCartsDT.Rows[DyeingCartsDT.Rows.Count - 1]["DyeingCartID"]);
                else
                    iDyeingCartID = 0;
            }
        }

        public int FindDyeingAssignmentID(int MainOrderID)
        {
            int DyeingAssignmentID = 0;
            string SelectCommand = @"SELECT TOP 1 * FROM DyeingAssignmentDetails WHERE MainOrderID=" + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        DyeingAssignmentID = Convert.ToInt32(DT.Rows[0]["DyeingAssignmentID"]);
                }
            }
            return DyeingAssignmentID;
        }

        public void MoveToWorkAssignmentID(int WorkAssignmentID)
        {
            WorkAssignmentsBS.Position = WorkAssignmentsBS.Find("WorkAssignmentID", WorkAssignmentID);
        }

        public void MoveToDyeingAssignmentID(int DyeingAssignmentID)
        {
            DyeingAssignmentsBS.Position = DyeingAssignmentsBS.Find("DyeingAssignmentID", DyeingAssignmentID);
        }

        public void MoveToDyeingAssignmentPos(int Pos)
        {
            DyeingAssignmentsBS.Position = Pos;
        }

        public void MoveToFirstDyeingAssignmentID()
        {
            DyeingAssignmentsBS.MoveFirst();
        }

        public void MoveToLastDyeingAssignmentID()
        {
            DyeingAssignmentsBS.MoveLast();
            if (DyeingAssignmentsDT.Rows.Count > 0)
                iDyeingAssignmentID = Convert.ToInt32(DyeingAssignmentsDT.Rows[DyeingAssignmentsDT.Rows.Count - 1]["DyeingAssignmentID"]);
            else
                iDyeingAssignmentID = 0;
        }

        public void MoveToBatchMainOrderPos(int Pos)
        {
            BatchMainOrdersBS.Position = Pos;
        }

        public void MoveToBatchPos(int Pos)
        {
            BatchesBS.Position = Pos;
        }

        public void MoveToMegaBatchPos(int Pos)
        {
            MegaBatchesBS.Position = Pos;
        }

        public void MoveToWorkAssignmentPos(int Pos)
        {
            WorkAssignmentsBS.Position = Pos;
        }

        public bool IsM1(int WorkAssignmentID, int FactoryID)
        {
            string SelectCommand = @"SELECT ClientID FROM MegaOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                    WHERE MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE BatchID IN (SELECT BatchID FROM Batch WHERE ProfilWorkAssignmentID=" + WorkAssignmentID + ")))";

            if (FactoryID == 2)
                SelectCommand = @"SELECT ClientID FROM MegaOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                    WHERE MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE BatchID IN (SELECT BatchID FROM Batch WHERE TPSWorkAssignmentID=" + WorkAssignmentID + ")))";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["ClientID"] != DBNull.Value && Convert.ToInt32(DT.Rows[0]["ClientID"]) == 101)
                            return true;
                    }
                }
            }
            return false;
        }

        public void AddDyeingCartColumn1()
        {
            bool b = false;
            int Counter = 1;
            string ColumnName = string.Empty;
            while (!b)
            {
                ColumnName = "ColumnDyeingCart" + Counter;
                if (BatchesDT.Columns.Contains(ColumnName))
                    Counter++;
                else
                    b = true;
            }
            BatchesDT.Columns.Add(new DataColumn((ColumnName), System.Type.GetType("System.Decimal")));
            for (int j = 0; j < BatchesDT.Rows.Count; j++)
            {
                BatchesDT.Rows[j][ColumnName] = 0;
            }
        }

        public void SaveDyeingBatch()
        {
            string SelectCommand = @"SELECT * FROM DyeingCarts WHERE DyeingAssignmentID=" + iDyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        bool bColumnDyeingCart = false;
                        foreach (DataRow row in BatchesDT.Rows)
                        {
                            decimal Square = 0;
                            foreach (DataColumn col in BatchesDT.Columns)
                            {
                                string ColumnName = col.ColumnName;
                                if (ColumnName.Contains("ColumnDyeingCart"))
                                {
                                    bColumnDyeingCart = true;
                                    string ColumnDyeingCart = ColumnName.Substring(6, ColumnName.Length - 6);
                                    string s = ColumnName.Substring(16, ColumnName.Length - 16);
                                    int DyeingCartID = Convert.ToInt32(s);
                                    DataRow[] rows1 = DT.Select("DyeingCartID=" + DyeingCartID);
                                    if (row[ColumnName] != DBNull.Value && Convert.ToDecimal(row[ColumnName]) != 0)
                                    {
                                        Square = Convert.ToDecimal(row[ColumnName]);
                                        Square = Decimal.Round(Square, 6, MidpointRounding.AwayFromZero);
                                    }
                                    if (rows1.Count() == 0)
                                    {
                                        CreateDyeingCart(iDyeingAssignmentID, Square);
                                        SaveDyeingCarts();
                                        UpdateDyeingCarts(iDyeingAssignmentID);
                                    }
                                    else
                                    {
                                        rows1[0]["Square"] = Square;
                                    }
                                }
                            }
                        }
                        if (!bColumnDyeingCart)
                        {

                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public string GetBarcodeNumber(int BarcodeType, int iNumber)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string sNumber = "";
            if (iNumber.ToString().Length == 1)
                sNumber = "00000000" + iNumber.ToString();
            if (iNumber.ToString().Length == 2)
                sNumber = "0000000" + iNumber.ToString();
            if (iNumber.ToString().Length == 3)
                sNumber = "000000" + iNumber.ToString();
            if (iNumber.ToString().Length == 4)
                sNumber = "00000" + iNumber.ToString();
            if (iNumber.ToString().Length == 5)
                sNumber = "0000" + iNumber.ToString();
            if (iNumber.ToString().Length == 6)
                sNumber = "000" + iNumber.ToString();
            if (iNumber.ToString().Length == 7)
                sNumber = "00" + iNumber.ToString();
            if (iNumber.ToString().Length == 8)
                sNumber = "0" + iNumber.ToString();

            StringBuilder BarcodeNumber = new StringBuilder(Type);
            BarcodeNumber.Append(sNumber);

            return BarcodeNumber.ToString();
        }

        public void CreateBarcodes()
        {
            for (int i = 0; i < DyeingAssignmentBarcodesDT.Rows.Count; i++)
            {
                int DyeingAssignmentBarcodeID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[i]["DyeingAssignmentBarcodeID"]);
                string BarcodeNumber = GetBarcodeNumber(14, DyeingAssignmentBarcodeID);
                string FileName = System.Environment.GetEnvironmentVariable("TEMP") + @"/" + BarcodeNumber + ".jpg";
                Image img = Barcode.GetBarcode(Barcode.BarcodeLength.Long, 46, BarcodeNumber);
                img.Save(FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        //public void CreateBarcodes()
        //{
        //    ArrayList list = new ArrayList();
        //    list.Add(385);
        //    list.Add(405);
        //    list.Add(430);
        //    list.Add(431);
        //    list.Add(432);
        //    list.Add(433);
        //    list.Add(434);
        //    list.Add(435);
        //    list.Add(436);
        //    list.Add(437);
        //    list.Add(438);
        //    list.Add(439);
        //    list.Add(440);
        //    list.Add(441);
        //    list.Add(442);
        //    list.Add(443);
        //    list.Add(444);
        //    list.Add(445);
        //    list.Add(446);
        //    list.Add(447);
        //    list.Add(448);
        //    list.Add(450);
        //    list.Add(451);
        //    list.Add(452);
        //    list.Add(453);
        //    list.Add(454);
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        int DyeingAssignmentBarcodeID = Convert.ToInt32(list[i]);
        //        string BarcodeNumber = GetBarcodeNumber(15, DyeingAssignmentBarcodeID);
        //        string FileName = System.Environment.GetEnvironmentVariable("TEMP") + @"/" + BarcodeNumber + ".jpg";
        //        Image img = Barcode.GetBarcode(Barcode.BarcodeLength.Medium, 52, BarcodeNumber);
        //        img.Save(FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    }
        //}

        public int GetDyeingAssignmentBarcode(int DyeingCartID, int TechCatalogOperationsDetailID)
        {
            DataRow[] rows = DyeingAssignmentBarcodesDT.Select("DyeingCartID=" + DyeingCartID + " AND TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            if (rows.Count() == 0)
                return 0;
            return Convert.ToInt32(rows[0]["DyeingAssignmentBarcodeID"]);
        }

        public DataTable GetPercentageTable(int DyeingAssignmentID)
        {
            decimal ReadyCount = 0;
            decimal AllCount = 0;

            decimal ReadyPerc = 0;
            decimal ReadyProgressVal = 0;
            decimal d1 = 0;
            DataTable PercentageDT = new DataTable();
            PercentageDT.Columns.Add(new DataColumn("MachinesOperationName", Type.GetType("System.String")));
            PercentageDT.Columns.Add(new DataColumn("ReadyPerc", Type.GetType("System.Decimal")));
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DyeingAssignmentBarcodeID, DyeingAssignmentBarcodes.DyeingAssignmentID, DyeingAssignmentBarcodes.DyeingCartID,
                DyeingAssignmentBarcodes.TechCatalogOperationsGroupID, DyeingAssignmentBarcodes.TechCatalogOperationsDetailID, infiniu2_catalog.dbo.MachinesOperations.MachinesOperationName, DyeingCarts.Square, DyeingAssignmentBarcodes.Status FROM DyeingAssignmentBarcodes 
                INNER JOIN DyeingCarts ON DyeingAssignmentBarcodes.DyeingCartID = DyeingCarts.DyeingCartID
                INNER JOIN infiniu2_catalog.dbo.TechCatalogOperationsDetail ON DyeingAssignmentBarcodes.TechCatalogOperationsDetailID = infiniu2_catalog.dbo.TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                INNER JOIN infiniu2_catalog.dbo.MachinesOperations ON infiniu2_catalog.dbo.TechCatalogOperationsDetail.MachinesOperationID = infiniu2_catalog.dbo.MachinesOperations.MachinesOperationID
                WHERE DyeingAssignmentBarcodes.DyeingAssignmentID = " + DyeingAssignmentID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    DataTable Table = new DataTable();
                    using (DataView DV = new DataView(DT))
                    {
                        Table = DV.ToTable(true, new string[] { "TechCatalogOperationsDetailID", "MachinesOperationName" });
                    }
                    for (int i = 0; i < Table.Rows.Count; i++)
                    {
                        int TechCatalogOperationsDetailID = Convert.ToInt32(Table.Rows[i]["TechCatalogOperationsDetailID"]);
                        string MachinesOperationName = Table.Rows[i]["MachinesOperationName"].ToString();
                        DataRow[] rows = DT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);

                        ReadyCount = 0;
                        AllCount = 0;
                        ReadyPerc = 0;
                        ReadyProgressVal = 0;
                        d1 = 0;

                        foreach (DataRow item in rows)
                        {
                            if (Convert.ToInt32(item["Status"]) == 2)
                                ReadyCount += Convert.ToDecimal(item["Square"]);
                            AllCount += Convert.ToDecimal(item["Square"]);
                        }

                        if (AllCount > 0)
                            ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
                        d1 = ReadyProgressVal * 100;
                        ReadyPerc = Decimal.Round(d1, 1, MidpointRounding.AwayFromZero);

                        DataRow NewRow = PercentageDT.NewRow();
                        NewRow["MachinesOperationName"] = MachinesOperationName;
                        NewRow["ReadyPerc"] = ReadyPerc;
                        PercentageDT.Rows.Add(NewRow);
                    }
                    Table.Dispose();
                }
            }
            return PercentageDT;
        }

        public void ReportToExcelSingly()
        {
            HSSFWorkbook hssfworkbook;

            string FileName = "Зачистка - протирка декабрь 2014 — копия.xlsx";
            string tempFolder = @"c:\Users\Андрей\Documents\Разнос работы. Покраска. 2014. Декабрь\";
            string path = Path.Combine(tempFolder, FileName);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            hssfworkbook = new HSSFWorkbook(fs);

            HSSFSheet sheet1;
            sheet1 = hssfworkbook.GetSheet("Расценки, нормы");

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle MainContentCS = hssfworkbook.CreateCellStyle();
            MainContentCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            MainContentCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainContentCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainContentCS.BottomBorderColor = HSSFColor.BLACK.index;
            MainContentCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainContentCS.LeftBorderColor = HSSFColor.BLACK.index;
            MainContentCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainContentCS.RightBorderColor = HSSFColor.BLACK.index;
            MainContentCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainContentCS.TopBorderColor = HSSFColor.BLACK.index;
            MainContentCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            MainContentCS.SetFont(Calibri11F);

            #endregion

            //DataTable DT = ReportViewDT.Copy();
            DataTable DT = new DataTable();
            HSSFCell cell = null;
            cell = sheet1.GetRow(2).GetCell(0);
            cell.SetCellValue(20);
            cell.CellStyle = MainContentCS;

            fs.Close();
        }
    }



    public class PrintDyeingAssignments : IAllFrontParameterName
    {
        ControlAssignments ControlAssignmentsManager;

        int TechCatalogOperationsGroupID = 0;
        string ShortName = string.Empty;

        object ResponsibleDateTime = DBNull.Value;
        object TechnologyDateTime = DBNull.Value;
        object ControlDateTime = DBNull.Value;
        object AgreementDateTime = DBNull.Value;
        object PrintDateTime = DBNull.Value;
        object ResponsibleUserID = DBNull.Value;
        object TechnologyUserID = DBNull.Value;
        object ControlUserID = DBNull.Value;
        object AgreementUserID = DBNull.Value;
        object PrintUserID = DBNull.Value;

        HSSFWorkbook hssfworkbook;
        HSSFSheet sheet1;
        HSSFPatriarch patriarch;
        HSSFClientAnchor anchor;
        HSSFPicture picture;


        NeedPaintingMaterial NeedPaintingMaterialManager;
        FileManager FM = new FileManager();
        DateTime CurrentDate;

        DataTable ImagesToDeleteDT;
        DataTable DyeingAssignmentsDT;
        DataTable DyeingCartsDT;

        DataTable DeyingDT;

        DataTable ProfileNamesDT;

        public DataTable FrontsDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;

        DataTable KansasBoxesDT;
        DataTable LorenzoBoxesDT;
        DataTable SofiaBoxesDT;
        DataTable Turin1BoxesDT;
        DataTable Turin3BoxesDT;
        DataTable InfinitiBoxesDT;

        DataTable KansasGridsDT;
        DataTable LorenzoGridsDT;
        DataTable SofiaGridsDT;
        DataTable Turin1GridsDT;
        DataTable Turin3GridsDT;
        DataTable InfinitiGridsDT;

        DataTable SofiaAppliqueDT;

        DataTable KansasSimpleDT;
        DataTable LorenzoSimpleDT;
        DataTable SofiaSimpleDT;
        DataTable Turin1SimpleDT;
        DataTable Turin3SimpleDT;
        DataTable InfinitiSimpleDT;

        DataTable KansasOrdersDT;
        DataTable LorenzoOrdersDT;
        DataTable SofiaOrdersDT;
        DataTable Turin1OrdersDT;
        DataTable Turin3OrdersDT;
        DataTable InfinitiOrdersDT;
        DataTable GenevaOrdersDT;
        DataTable TafelOrdersDT;

        public PrintDyeingAssignments(ControlAssignments tControlAssignmentsManager)
        {
            ControlAssignmentsManager = tControlAssignmentsManager;
        }

        public void Initialize()
        {
            Create();
            Fill();
            NeedPaintingMaterialManager = new NeedPaintingMaterial();
            NeedPaintingMaterialManager.Initialize();
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
            DyeingAssignmentsDT = new DataTable();
            DyeingCartsDT = new DataTable();

            ProfileNamesDT = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();

            KansasBoxesDT = new DataTable();
            KansasGridsDT = new DataTable();
            KansasSimpleDT = new DataTable();

            LorenzoBoxesDT = new DataTable();
            LorenzoGridsDT = new DataTable();
            LorenzoSimpleDT = new DataTable();

            SofiaBoxesDT = new DataTable();
            SofiaGridsDT = new DataTable();
            SofiaAppliqueDT = new DataTable();
            SofiaSimpleDT = new DataTable();

            Turin1BoxesDT = new DataTable();
            Turin1GridsDT = new DataTable();
            Turin1SimpleDT = new DataTable();

            Turin3BoxesDT = new DataTable();
            Turin3GridsDT = new DataTable();
            Turin3SimpleDT = new DataTable();

            InfinitiBoxesDT = new DataTable();
            InfinitiGridsDT = new DataTable();
            InfinitiSimpleDT = new DataTable();

            KansasOrdersDT = new DataTable();
            LorenzoOrdersDT = new DataTable();
            SofiaOrdersDT = new DataTable();
            Turin1OrdersDT = new DataTable();
            Turin3OrdersDT = new DataTable();
            GenevaOrdersDT = new DataTable();
            TafelOrdersDT = new DataTable();

            ImagesToDeleteDT = new DataTable();
            ImagesToDeleteDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));

            DeyingDT = new DataTable();
            DeyingDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            DeyingDT.Columns.Add(new DataColumn("FrameColor", Type.GetType("System.String")));
            DeyingDT.Columns.Add(new DataColumn("InsetColor", Type.GetType("System.String")));
            DeyingDT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            DeyingDT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            DeyingDT.Columns.Add(new DataColumn("PlanCount", Type.GetType("System.Int32")));
            DeyingDT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            DeyingDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            DeyingDT.Columns.Add(new DataColumn("ColorType", Type.GetType("System.Int32")));
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.FrontsConfig.FrontConfigID FROM infiniu2_catalog.dbo.TechStore
                    INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON infiniu2_catalog.dbo.TechStore.TechStoreID = infiniu2_catalog.dbo.FrontsConfig.ProfileID",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProfileNamesDT);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                AND TechStoreSubGroupID IN (214) ORDER BY TechStoreName";
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
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 FrontsOrdersID, MainOrderID, FrontID, ColorID, TechnoProfileID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, PlanCount, Square, FrontConfigID, Notes FROM DyeingAssignmentDetails",
                ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(SofiaOrdersDT);
                SofiaOrdersDT.Columns.Add(new DataColumn("GroupType", Type.GetType("System.Int32")));

                SofiaBoxesDT = SofiaOrdersDT.Clone();
                SofiaGridsDT = SofiaOrdersDT.Clone();
                SofiaSimpleDT = SofiaOrdersDT.Clone();
                SofiaAppliqueDT = SofiaOrdersDT.Clone();

                KansasOrdersDT = SofiaOrdersDT.Clone();
                KansasBoxesDT = SofiaOrdersDT.Clone();
                KansasGridsDT = SofiaOrdersDT.Clone();
                KansasSimpleDT = SofiaOrdersDT.Clone();

                LorenzoOrdersDT = SofiaOrdersDT.Clone();
                LorenzoBoxesDT = SofiaOrdersDT.Clone();
                LorenzoGridsDT = SofiaOrdersDT.Clone();
                LorenzoSimpleDT = SofiaOrdersDT.Clone();

                Turin1OrdersDT = SofiaOrdersDT.Clone();
                Turin1BoxesDT = SofiaOrdersDT.Clone();
                Turin1GridsDT = SofiaOrdersDT.Clone();
                Turin1SimpleDT = SofiaOrdersDT.Clone();

                Turin3OrdersDT = SofiaOrdersDT.Clone();
                Turin3BoxesDT = SofiaOrdersDT.Clone();
                Turin3GridsDT = SofiaOrdersDT.Clone();
                Turin3SimpleDT = SofiaOrdersDT.Clone();

                InfinitiOrdersDT = SofiaOrdersDT.Clone();
                InfinitiBoxesDT = SofiaOrdersDT.Clone();
                InfinitiGridsDT = SofiaOrdersDT.Clone();
                InfinitiSimpleDT = SofiaOrdersDT.Clone();

                GenevaOrdersDT = SofiaOrdersDT.Clone();

                TafelOrdersDT = SofiaOrdersDT.Clone();
            }
        }

        private string ProfileName(int ID)
        {
            string name = string.Empty;
            DataRow[] rows = ProfileNamesDT.Select("FrontConfigID=" + ID);
            if (rows.Count() > 0)
                name = rows[0]["TechStoreName"].ToString();
            return name;
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

        public void GetDyeingAssignmentInfo(ref object date1, ref object date2, ref object date3, ref object date4, ref object date5,
            ref object User1, ref object User2, ref object User3, ref object User4, ref object User5)
        {
            ResponsibleDateTime = date1;
            TechnologyDateTime = date2;
            ControlDateTime = date3;
            AgreementDateTime = date4;
            PrintDateTime = date5;
            ResponsibleUserID = User1;
            TechnologyUserID = User2;
            ControlUserID = User3;
            AgreementUserID = User4;
            PrintUserID = User5;
        }

        public string GetUserName(int UserID)
        {
            string Name = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, ShortName FROM Users" +
                    " WHERE UserID=" + UserID, ConnectionStrings.UsersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Name = DT.Rows[0]["ShortName"].ToString();
                }
            }

            return Name;
        }

        private void GetProfileNames(ref DataTable DestinationDT, int DyeingAssignmentID, Fronts Front)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();

            SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.FrontsConfig.FrontConfigID FROM infiniu2_catalog.dbo.TechStore
                    INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON infiniu2_catalog.dbo.TechStore.TechStoreID = infiniu2_catalog.dbo.FrontsConfig.ProfileID AND infiniu2_catalog.dbo.FrontsConfig.FrontConfigID IN (SELECT FrontConfigID FROM DyeingAssignmentDetails
                    WHERE Width<>-1 AND FrontID=" + Convert.ToInt32(Front) + " AND DyeingAssignmentID=" + DyeingAssignmentID + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = DestinationDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void GetBoxFronts(DataTable SourceDT, ref DataTable DestinationDT, int Admission)
        {
            DataRow[] rows = SourceDT.Select("Height<=" + Admission + " OR Width<=" + Admission);
            foreach (DataRow dr in rows)
                DestinationDT.Rows.Add(dr.ItemArray);
        }

        private void GetGridFronts(DataTable SourceDT, ref DataTable DestinationDT)
        {
            DataRow[] rows = SourceDT.Select("FrontID IN (3729) OR InsetTypeID IN (685,686,687,688,29470,29471)");
            foreach (DataRow dr in rows)
                DestinationDT.Rows.Add(dr.ItemArray);
        }

        private void GetAppliqueFronts(DataTable SourceDT, ref DataTable DestinationDT)
        {
            DataRow[] rows = SourceDT.Select("FrontID IN (3728,3731,3732,3739,3740,3741,3744,3745,3746) OR InsetTypeID IN (28961, 3653,3654,3655)");
            foreach (DataRow dr in rows)
                DestinationDT.Rows.Add(dr.ItemArray);
        }

        private void GetSimpleFronts(DataTable SourceDT, ref DataTable DestinationDT, int Admission)
        {
            DataRow[] rows = SourceDT.Select("FrontID NOT IN (3728,3731,3732,3739,3740,3741,3744,3745,3746,3729) AND InsetTypeID NOT IN (28961,3653,3654,3655) AND InsetTypeID NOT IN (685,686,687,688,29470,29471) AND Height>" + Admission + " AND Width>" + Admission);
            foreach (DataRow dr in rows)
                DestinationDT.Rows.Add(dr.ItemArray);
        }

        private void GetFrontsOrders1(ref DataTable DestinationDT, int DyeingCartID, Fronts Front)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();

            SelectCommand = @"SELECT FrontsOrdersID, MainOrderID, FrontID, ColorID, TechnoProfileID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, PlanCount, Square, FrontConfigID, Notes FROM DyeingAssignmentDetails
                WHERE Width<>-1 AND FrontID=" + Convert.ToInt32(Front) + " AND DyeingCartID=" + DyeingCartID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = DestinationDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void GetFrontsOrdersTafel1(ref DataTable DestinationDT, int DyeingCartID, Fronts Front1, Fronts Front2, Fronts Front3)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();

            SelectCommand = @"SELECT FrontsOrdersID, MainOrderID, FrontID, ColorID, TechnoProfileID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, PlanCount, Square, FrontConfigID, Notes FROM DyeingAssignmentDetails
                WHERE Width<>-1 AND (FrontID=" + Convert.ToInt32(Front1) + " OR FrontID=" + Convert.ToInt32(Front2) + " OR FrontID=" + Convert.ToInt32(Front3) + ") AND DyeingCartID=" + DyeingCartID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = DestinationDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void GetFrontsOrdersGeneva1(ref DataTable DestinationDT, int DyeingCartID)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();
            SelectCommand = @"SELECT FrontsOrdersID, MainOrderID, FrontID, ColorID, TechnoProfileID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, PlanCount, Square, FrontConfigID, Notes FROM DyeingAssignmentDetails
                WHERE Width<>-1 AND FrontID IN (16269,28945,3727,3728,3729,3730,3731,3732,3733,3734,3735,3736,3737,3739,3740,3741,3742,3743,3744,3745,3746,3747,3748,15108,27914,29597,15760) AND DyeingCartID=" + DyeingCartID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = DestinationDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void GetFrontsOrdersGeneva2(ref DataTable DestinationDT, int DyeingAssignmentID)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();
            SelectCommand = @"SELECT FrontsOrdersID, MainOrderID, FrontID, ColorID, TechnoProfileID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, PlanCount, Square, FrontConfigID, Notes FROM DyeingAssignmentDetails
                WHERE Width<>-1 AND FrontID IN (28945,16269,3727,3728,3729,3730,3731,3732,3733,3734,3735,3736,3737,3739,3740,3741,3742,3743,3744,3745,3746,3747,3748,15108,27914,29597,15760) AND DyeingAssignmentID=" + DyeingAssignmentID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = DestinationDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void GetFrontsOrders2(ref DataTable DestinationDT, int DyeingAssignmentID, Fronts Front)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();

            SelectCommand = @"SELECT FrontsOrdersID, MainOrderID, FrontID, ColorID, TechnoProfileID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, PlanCount, Square, FrontConfigID, Notes FROM DyeingAssignmentDetails
                WHERE Width<>-1 AND FrontID=" + Convert.ToInt32(Front) + " AND DyeingAssignmentID=" + DyeingAssignmentID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = DestinationDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void GetFrontsOrdersTafel2(ref DataTable DestinationDT, int DyeingAssignmentID, Fronts Front1, Fronts Front2, Fronts Front3)
        {
            string SelectCommand = string.Empty;
            DataTable DT = new DataTable();

            SelectCommand = @"SELECT FrontsOrdersID, MainOrderID, FrontID, ColorID, TechnoProfileID, TechnoColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, PlanCount, Square, FrontConfigID, Notes FROM DyeingAssignmentDetails
                WHERE Width<>-1 AND (FrontID=" + Convert.ToInt32(Front1) + " OR FrontID=" + Convert.ToInt32(Front2) + " OR FrontID=" + Convert.ToInt32(Front3) + ") AND DyeingAssignmentID=" + DyeingAssignmentID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = DestinationDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void CollectDeying(int ColorID, DataTable SourceDT, ref DataTable DestinationDT, string AdditionalName)
        {
            DataTable DT2 = new DataTable();

            using (DataView DV = new DataView(SourceDT, "InsetTypeID=1 AND ColorID=" + ColorID,
                "PatinaID, InsetTypeID, InsetColorID, Height, Width", DataViewRowState.CurrentRows))
            {
                DT2 = DV.ToTable(true, new string[] { "PatinaID", "InsetTypeID", "InsetColorID", "Height", "Width" });
            }
            for (int j = 0; j < DT2.Rows.Count; j++)
            {
                decimal Square = 0;
                int Count = 0;
                string InsetColor = string.Empty;

                DataRow[] rows = SourceDT.Select("ColorID=" + ColorID + " AND PatinaID=" + Convert.ToInt32(DT2.Rows[j]["PatinaID"]) + " AND InsetTypeID=" + Convert.ToInt32(DT2.Rows[j]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(DT2.Rows[j]["InsetColorID"]) + " AND Height=" + Convert.ToInt32(DT2.Rows[j]["Height"]) +
                    " AND Width=" + Convert.ToInt32(DT2.Rows[j]["Width"]));
                foreach (DataRow item in rows)
                {
                    Count += Convert.ToInt32(item["PlanCount"]);
                    Square += Convert.ToDecimal(item["Height"]) * Convert.ToDecimal(item["Width"]) * Convert.ToDecimal(item["PlanCount"]) / 1000000;
                }
                //Square = Decimal.Round(Square, 3, MidpointRounding.AwayFromZero);

                DataRow NewRow = DestinationDT.NewRow();
                NewRow["ColorType"] = ColorID;
                NewRow["Name"] = ProfileName(Convert.ToInt32(rows[0]["FrontConfigID"])) + AdditionalName;

                string FrameColor = GetColorName(Convert.ToInt32(rows[0]["ColorID"]));
                if (Convert.ToInt32(rows[0]["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(rows[0]["PatinaID"]));
                NewRow["FrameColor"] = FrameColor;
                if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 1)
                    NewRow["InsetColor"] = "витрина";
                else
                    NewRow["InsetColor"] = GetInsetTypeName(Convert.ToInt32(rows[0]["InsetTypeID"])) + " " + GetInsetColorName(Convert.ToInt32(rows[0]["InsetColorID"]));
                NewRow["Height"] = rows[0]["Height"];
                NewRow["Width"] = rows[0]["Width"];
                NewRow["PlanCount"] = Count;
                NewRow["Square"] = Square;
                NewRow["Notes"] = rows[0]["Notes"];
                DestinationDT.Rows.Add(NewRow);
            }

            using (DataView DV = new DataView(SourceDT, "InsetTypeID<>1 AND ColorID=" + ColorID,
                "PatinaID, InsetTypeID, InsetColorID, Height, Width", DataViewRowState.CurrentRows))
            {
                DT2 = DV.ToTable(true, new string[] { "PatinaID", "InsetTypeID", "InsetColorID", "Height", "Width" });
            }
            for (int j = 0; j < DT2.Rows.Count; j++)
            {
                decimal Square = 0;
                int Count = 0;
                string InsetColor = string.Empty;

                DataRow[] rows = SourceDT.Select("ColorID=" + ColorID + " AND PatinaID=" + Convert.ToInt32(DT2.Rows[j]["PatinaID"]) + " AND InsetTypeID=" + Convert.ToInt32(DT2.Rows[j]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(DT2.Rows[j]["InsetColorID"]) + " AND Height=" + Convert.ToInt32(DT2.Rows[j]["Height"]) +
                    " AND Width=" + Convert.ToInt32(DT2.Rows[j]["Width"]));
                foreach (DataRow item in rows)
                {
                    Count += Convert.ToInt32(item["PlanCount"]);
                    Square += Convert.ToDecimal(item["Height"]) * Convert.ToDecimal(item["Width"]) * Convert.ToDecimal(item["PlanCount"]) / 1000000;
                }
                //Square = Decimal.Round(Square, 3, MidpointRounding.AwayFromZero);

                DataRow NewRow = DestinationDT.NewRow();
                NewRow["ColorType"] = ColorID;
                NewRow["Name"] = ProfileName(Convert.ToInt32(rows[0]["FrontConfigID"])) + AdditionalName;
                string FrameColor = GetColorName(Convert.ToInt32(rows[0]["ColorID"]));
                if (Convert.ToInt32(rows[0]["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(rows[0]["PatinaID"]));
                NewRow["FrameColor"] = FrameColor;
                if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 1)
                    NewRow["InsetColor"] = "витрина";
                else
                    NewRow["InsetColor"] = GetInsetTypeName(Convert.ToInt32(rows[0]["InsetTypeID"])) + " " + GetInsetColorName(Convert.ToInt32(rows[0]["InsetColorID"]));
                NewRow["Height"] = rows[0]["Height"];
                NewRow["Width"] = rows[0]["Width"];
                NewRow["PlanCount"] = Count;
                NewRow["Square"] = Square;
                NewRow["Notes"] = rows[0]["Notes"];
                DestinationDT.Rows.Add(NewRow);
            }
        }

        private void CollectDeying(DataTable SourceDT, ref DataTable DestinationDT)
        {
            DataTable DT1 = new DataTable();
            DataTable DT3 = new DataTable();

            using (DataView DV = new DataView(SourceDT, string.Empty, "ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                using (DataView DV = new DataView(SourceDT, "InsetTypeID=1 " +
                    " AND ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(DT1.Rows[i]["PatinaID"]),
                    "InsetTypeID, InsetColorID, Height, Width", DataViewRowState.CurrentRows))
                {
                    DT3 = DV.ToTable(true, new string[] { "InsetTypeID", "InsetColorID", "Height", "Width" });
                }
                for (int c = 0; c < DT3.Rows.Count; c++)
                {
                    decimal Square = 0;
                    int Count = 0;
                    string Name = "Женева";
                    string InsetColor = string.Empty;

                    DataRow[] rows = SourceDT.Select("ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(DT1.Rows[i]["PatinaID"]) +
                        " AND InsetTypeID=" + Convert.ToInt32(DT3.Rows[c]["InsetTypeID"]) +
                        " AND InsetColorID=" + Convert.ToInt32(DT3.Rows[c]["InsetColorID"]) + " AND Height=" + Convert.ToInt32(DT3.Rows[c]["Height"]) +
                        " AND Width=" + Convert.ToInt32(DT3.Rows[c]["Width"]));
                    if (rows.Count() == 0)
                        continue;
                    foreach (DataRow item in rows)
                    {
                        Count += Convert.ToInt32(item["PlanCount"]);
                        Square += Convert.ToDecimal(item["Height"]) * Convert.ToDecimal(item["Width"]) * Convert.ToDecimal(item["PlanCount"]) / 1000000;
                    }
                    Square = Decimal.Round(Square, 3, MidpointRounding.AwayFromZero);

                    DataRow NewRow = DestinationDT.NewRow();
                    NewRow["ColorType"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                    string FrameColor = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                    if (Convert.ToInt32(DT1.Rows[i]["PatinaID"]) != -1)
                        FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(DT1.Rows[i]["PatinaID"]));
                    NewRow["FrameColor"] = FrameColor;
                    InsetColor = GetInsetColorName(Convert.ToInt32(rows[0]["InsetColorID"]));
                    //if (InsetColor.Length > 0)
                    //    InsetColor = InsetColor + " ";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 5 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 6)
                    //    Name = "Женева РЕШ";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 8)
                    //    Name = "Женева РЕШ ОВАЛ";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 10 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //    Name = "Женева Аппл";
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 2)
                    //{
                    //    Name = "Женева Дуэт лев";
                    //    if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 10 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //        Name = "Женева Дуэт лев Аппл";
                    //}
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 3)
                    //{
                    //    Name = "Женева Дуэт прав";
                    //    if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 10 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //        Name = "Женева Дуэт прав Аппл";
                    //}
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 4)
                    //    Name = "Женева Трио лев";
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 5)
                    //    Name = "Женева Трио прав";
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 6)
                    //    Name = "Женева Арка";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 4)
                    //    InsetColor = "витрина";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9)
                    //    InsetColor = "аппликация №1 л";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 10)
                    //    InsetColor = "аппликация №1 пр";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //    InsetColor = "аппликация №2";
                    NewRow["Name"] = Name;
                    NewRow["InsetColor"] = GetInsetTypeName(Convert.ToInt32(rows[0]["InsetTypeID"])) + InsetColor;
                    NewRow["Height"] = rows[0]["Height"];
                    NewRow["Width"] = rows[0]["Width"];
                    NewRow["PlanCount"] = Count;
                    NewRow["Square"] = Square;
                    NewRow["Notes"] = rows[0]["Notes"];
                    DestinationDT.Rows.Add(NewRow);
                }

                using (DataView DV = new DataView(SourceDT, "InsetTypeID<>1 " +
                    " AND ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(DT1.Rows[i]["PatinaID"]),
                    "InsetTypeID, InsetColorID, Height, Width", DataViewRowState.CurrentRows))
                {
                    DT3 = DV.ToTable(true, new string[] { "InsetTypeID", "InsetColorID", "Height", "Width" });
                }
                for (int c = 0; c < DT3.Rows.Count; c++)
                {
                    decimal Square = 0;
                    int Count = 0;
                    string Name = "Женева";
                    string InsetColor = string.Empty;
                    DataRow[] rows = SourceDT.Select("ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(DT1.Rows[i]["PatinaID"]) +
                        " AND InsetTypeID=" + Convert.ToInt32(DT3.Rows[c]["InsetTypeID"]) +
                        " AND InsetColorID=" + Convert.ToInt32(DT3.Rows[c]["InsetColorID"]) + " AND Height=" + Convert.ToInt32(DT3.Rows[c]["Height"]) +
                        " AND Width=" + Convert.ToInt32(DT3.Rows[c]["Width"]));
                    if (rows.Count() == 0)
                        continue;
                    foreach (DataRow item in rows)
                    {
                        Count += Convert.ToInt32(item["PlanCount"]);
                        Square += Convert.ToDecimal(item["Height"]) * Convert.ToDecimal(item["Width"]) * Convert.ToDecimal(item["PlanCount"]) / 1000000;
                    }
                    Square = Decimal.Round(Square, 3, MidpointRounding.AwayFromZero);

                    DataRow NewRow = DestinationDT.NewRow();
                    NewRow["ColorType"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                    string FrameColor = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                    if (Convert.ToInt32(DT1.Rows[i]["PatinaID"]) != -1)
                        FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(DT1.Rows[i]["PatinaID"]));
                    NewRow["FrameColor"] = FrameColor;
                    InsetColor = GetInsetColorName(Convert.ToInt32(rows[0]["InsetColorID"]));
                    if (InsetColor.Length > 0)
                        InsetColor = " " + InsetColor;
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 5 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 6)
                    //    Name = "Женева РЕШ";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 8)
                    //    Name = "Женева РЕШ ОВАЛ";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 10 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //    Name = "Женева Аппл";
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 2)
                    //{
                    //    Name = "Женева Дуэт лев";
                    //    if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 10 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //        Name = "Женева Дуэт лев Аппл";
                    //}
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 3)
                    //{
                    //    Name = "Женева Дуэт прав";
                    //    if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 10 || Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //        Name = "Женева Дуэт прав Аппл";
                    //}
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 4)
                    //    Name = "Женева Трио лев";
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 5)
                    //    Name = "Женева Трио прав";
                    //if (Convert.ToInt32(DT2.Rows[j]["FrontTypeID"]) == 6)
                    //    Name = "Женева Арка";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 4)
                    //    InsetColor = "витрина";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 8)
                    //    InsetColor = InsetColor + " ОВАЛ";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 9)
                    //    InsetColor = "аппликация №1 л";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 10)
                    //    InsetColor = "аппликация №1 пр";
                    //if (Convert.ToInt32(rows[0]["InsetTypeID"]) == 11)
                    //    InsetColor = "аппликация №2";
                    NewRow["Name"] = Name;
                    NewRow["InsetColor"] = GetInsetTypeName(Convert.ToInt32(rows[0]["InsetTypeID"])) + InsetColor;
                    NewRow["Height"] = rows[0]["Height"];
                    NewRow["Width"] = rows[0]["Width"];
                    NewRow["PlanCount"] = Count;
                    NewRow["Square"] = Square;
                    NewRow["Notes"] = rows[0]["Notes"];
                    DestinationDT.Rows.Add(NewRow);
                }
            }
        }

        private void GroupAndSortNotGlossFronts(DataTable SourceDT, ref DataTable DestinationDT)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            int H = 0;
            int W = 0;
            string N = string.Empty;

            using (DataView DV = new DataView(SourceDT, @"FrontID=3664 AND ColorID IN (1881,1893,1883,1884,1882,1885,1886,1889,1894)", string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "ColorID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                H = 0;
                W = 0;
                N = string.Empty;
                using (DataView DV = new DataView(SourceDT, "ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]),
                    "Height, Width, Notes", DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable(true, new string[] { "Height", "Width", "Notes" });
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    if (Convert.ToInt32(DT2.Rows[j]["Height"]) == H && Convert.ToInt32(DT2.Rows[j]["Width"]) == W &&
                        DT2.Rows[j]["Notes"].ToString() == N)
                        continue;

                    H = Convert.ToInt32(DT2.Rows[j]["Height"]);
                    W = Convert.ToInt32(DT2.Rows[j]["Width"]);
                    N = DT2.Rows[j]["Notes"].ToString();

                    decimal Square = 0;
                    int Count = 0;

                    string filter = "ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) +
                        " AND Height=" + Convert.ToInt32(DT2.Rows[j]["Height"]) +
                        " AND Width=" + Convert.ToInt32(DT2.Rows[j]["Width"]) + " AND (Notes='' OR Notes IS NULL)";
                    if (DT2.Rows[j]["Notes"] != DBNull.Value && DT2.Rows[j]["Notes"].ToString().Length > 0)
                        filter = "ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) +
                        " AND Height=" + Convert.ToInt32(DT2.Rows[j]["Height"]) +
                        " AND Width=" + Convert.ToInt32(DT2.Rows[j]["Width"]) +
                        " AND Notes='" + DT2.Rows[j]["Notes"].ToString() + "'";
                    DataRow[] rows = SourceDT.Select(filter);
                    foreach (DataRow item in rows)
                    {
                        Count += Convert.ToInt32(item["PlanCount"]);
                        Square += Convert.ToDecimal(item["Height"]) * Convert.ToDecimal(item["Width"]) * Convert.ToDecimal(item["PlanCount"]) / 1000000;
                    }
                    Square = Decimal.Round(Square, 3, MidpointRounding.AwayFromZero);

                    DataRow NewRow = DestinationDT.NewRow();
                    NewRow["Name"] = GetFrontName(3664);
                    NewRow["FrameColor"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                    if (Convert.ToInt32(rows[0]["InsetTypeID"]) == -1)
                        NewRow["InsetColor"] = "Без наполнителя";
                    else
                        NewRow["InsetColor"] = GetInsetTypeName(Convert.ToInt32(rows[0]["InsetTypeID"])) + " " + GetInsetColorName(Convert.ToInt32(rows[0]["InsetColorID"]));
                    NewRow["Height"] = Convert.ToInt32(DT2.Rows[j]["Height"]);
                    NewRow["Width"] = Convert.ToInt32(DT2.Rows[j]["Width"]);
                    NewRow["PlanCount"] = Count;
                    NewRow["Square"] = Square;
                    NewRow["Notes"] = DT2.Rows[j]["Notes"];
                    NewRow["ColorType"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                    DestinationDT.Rows.Add(NewRow);
                }
            }
            using (DataView DV = new DataView(DestinationDT.Copy()))
            {
                DV.Sort = "Name, FrameColor, Height, Width";
                DestinationDT.Clear();
                DestinationDT = DV.ToTable();
            }
        }

        private void GroupAndSortColoredFronts(DataTable SourceDT, ref DataTable DestinationDT)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            int H = 0;
            int W = 0;
            string N = string.Empty;

            using (DataView DV = new DataView(SourceDT, @"(ColorID = 1890 AND PatinaID = 6) OR (ColorID = 1890 AND PatinaID = 7)", string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                H = 0;
                W = 0;
                N = string.Empty;
                int FrontID = Convert.ToInt32(DT1.Rows[i]["FrontID"]);
                int ColorID = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                int PatinaID = Convert.ToInt32(DT1.Rows[i]["PatinaID"]);
                using (DataView DV = new DataView(SourceDT, "ColorID=" + ColorID + " AND FrontID=" + FrontID + " AND PatinaID=" + PatinaID,
                    "InsetTypeID, InsetColorID, Height, Width", DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable(true, new string[] { "Height", "Width", "Notes" });
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    if (Convert.ToInt32(DT2.Rows[j]["Height"]) == H && Convert.ToInt32(DT2.Rows[j]["Width"]) == W &&
                        DT2.Rows[j]["Notes"].ToString() == N)
                        continue;

                    H = Convert.ToInt32(DT2.Rows[j]["Height"]);
                    W = Convert.ToInt32(DT2.Rows[j]["Width"]);
                    N = DT2.Rows[j]["Notes"].ToString();

                    decimal Square = 0;
                    int Count = 0;

                    string filter = "ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) + " AND FrontID=" + Convert.ToInt32(DT1.Rows[i]["FrontID"]) + " AND PatinaID=" + Convert.ToInt32(DT1.Rows[i]["PatinaID"]) +
                        " AND Height=" + Convert.ToInt32(DT2.Rows[j]["Height"]) +
                        " AND Width=" + Convert.ToInt32(DT2.Rows[j]["Width"]) + " AND (Notes='' OR Notes IS NULL)";
                    if (DT2.Rows[j]["Notes"] != DBNull.Value && DT2.Rows[j]["Notes"].ToString().Length > 0)
                        filter = "ColorID=" + Convert.ToInt32(DT1.Rows[i]["ColorID"]) + " AND FrontID=" + Convert.ToInt32(DT1.Rows[i]["FrontID"]) + " AND PatinaID=" + Convert.ToInt32(DT1.Rows[i]["PatinaID"]) +
                        " AND Height=" + Convert.ToInt32(DT2.Rows[j]["Height"]) +
                        " AND Width=" + Convert.ToInt32(DT2.Rows[j]["Width"]) +
                        " AND Notes='" + DT2.Rows[j]["Notes"].ToString() + "'";
                    DataRow[] rows = SourceDT.Select(filter);
                    foreach (DataRow item in rows)
                    {
                        Count += Convert.ToInt32(item["PlanCount"]);
                        Square += Convert.ToDecimal(item["Height"]) * Convert.ToDecimal(item["Width"]) * Convert.ToDecimal(item["PlanCount"]) / 1000000;
                    }
                    Square = Decimal.Round(Square, 3, MidpointRounding.AwayFromZero);

                    DataRow NewRow = DestinationDT.NewRow();
                    NewRow["Name"] = GetFrontName(Convert.ToInt32(DT1.Rows[i]["FrontID"]));
                    if (Convert.ToInt32(DT1.Rows[i]["PatinaID"]) != -1)
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"])) + " " + GetPatinaName(Convert.ToInt32(DT1.Rows[i]["PatinaID"]));
                    else
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(DT1.Rows[i]["ColorID"]));
                    if (Convert.ToInt32(rows[0]["InsetTypeID"]) == -1)
                        NewRow["InsetColor"] = "Без наполнителя";
                    else
                        NewRow["InsetColor"] = GetInsetTypeName(Convert.ToInt32(rows[0]["InsetTypeID"])) + " " + GetInsetColorName(Convert.ToInt32(rows[0]["InsetColorID"]));
                    NewRow["Height"] = Convert.ToInt32(DT2.Rows[j]["Height"]);
                    NewRow["Width"] = Convert.ToInt32(DT2.Rows[j]["Width"]);
                    NewRow["PlanCount"] = Count;
                    NewRow["Square"] = Square;
                    NewRow["Notes"] = DT2.Rows[j]["Notes"];
                    NewRow["ColorType"] = Convert.ToInt32(DT1.Rows[i]["ColorID"]);
                    DestinationDT.Rows.Add(NewRow);
                }
            }
            using (DataView DV = new DataView(DestinationDT.Copy()))
            {
                DV.Sort = "Name, FrameColor, Height, Width";
                DestinationDT.Clear();
                DestinationDT = DV.ToTable();
            }
        }

        public void ClearOrders()
        {
            KansasOrdersDT.Clear();
            LorenzoOrdersDT.Clear();
            SofiaOrdersDT.Clear();
            Turin1OrdersDT.Clear();
            Turin3OrdersDT.Clear();
            InfinitiOrdersDT.Clear();
            GenevaOrdersDT.Clear();
            TafelOrdersDT.Clear();
        }

        public void CreateWorkBook()
        {
            hssfworkbook = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            sheet1 = hssfworkbook.CreateSheet("Покраска");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 22 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 17 * 256);
            sheet1.SetColumnWidth(3, 6 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 6 * 256);

            patriarch = sheet1.CreateDrawingPatriarch();

            ImagesToDeleteDT.Clear();
        }

        public void SaveOpenDyeingAssignmentReport(int DyeingAssignmentID)
        {
            //string FileName1 = "№" + DyeingAssignmentID;
            //string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName1 = "№" + DyeingAssignmentID;
            string tempFolder = @"\\192.168.1.6\Public\USERS_2016\_ДЕЙСТВУЮЩИЕ\ПРОИЗВОДСТВО\ТПС\Задания на покраску\";
            //string tempFolder = @"\\192.168.1.6\Public\ТПС\Infinium\Задания на покраску\";
            string CurrentMonthName = DateTime.Now.ToString("MMMM");
            tempFolder = Path.Combine(tempFolder, CurrentMonthName);
            if (!(Directory.Exists(tempFolder)))
            {
                Directory.CreateDirectory(tempFolder);
            }
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName1 + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName1 + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);

            DyeingAssignmentsDT.Dispose();
            DyeingCartsDT.Dispose();
            for (int i = 0; i < ImagesToDeleteDT.Rows.Count; i++)
            {
                string FileToDeleteName = string.Empty;
                if (ImagesToDeleteDT.Rows[i]["FileName"] != DBNull.Value)
                    FileToDeleteName = ImagesToDeleteDT.Rows[i]["FileName"].ToString();
                if (FileToDeleteName.Length > 0 && File.Exists(FileToDeleteName))
                {
                    File.Delete(FileToDeleteName);
                }
            }
        }

        public void CreateDyeingAssignmentReport(int DyeingAssignmentID, ref int RowIndex)
        {
            GetCurrentDate();

            ProfileNamesDT.Clear();
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Kansas);
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Sofia);
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Turin1);
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Turin3);
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Infiniti);
            //GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Geneva);
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Tafel2);
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Tafel3);
            GetProfileNames(ref ProfileNamesDT, DyeingAssignmentID, Fronts.Tafel3Gl);

            NeedPaintingMaterialManager.UpdateTables(DyeingAssignmentID);
            //NeedPaintingMaterialManager.FindTerms();

            string SelectCommand = string.Empty;

            SelectCommand = @"SELECT * FROM DyeingAssignments WHERE DyeingAssignmentID=" + DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentsDT.Clear();
                DA.Fill(DyeingAssignmentsDT);
            }
            SelectCommand = @"SELECT * FROM DyeingCarts WHERE DyeingAssignmentID=" + DyeingAssignmentID + " ORDER BY CartNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingCartsDT.Clear();
                DA.Fill(DyeingCartsDT);
            }

            if (DyeingAssignmentsDT.Rows.Count == 0 || DyeingCartsDT.Rows.Count == 0)
                return;

            int GroupType = Convert.ToInt32(DyeingAssignmentsDT.Rows[0]["GroupType"]);
            int WorkAssignmentID = 0;
            string BatchName = string.Empty;
            string Notes = DyeingAssignmentsDT.Rows[0]["Notes"].ToString();

            PrintUserID = 0;
            TechCatalogOperationsGroupID = 0;
            PrintDateTime = DBNull.Value;
            ShortName = string.Empty;

            if (DyeingAssignmentsDT.Rows[0]["PrintUserID"] != DBNull.Value)
                PrintUserID = Convert.ToInt32(DyeingAssignmentsDT.Rows[0]["PrintUserID"]);
            if (DyeingAssignmentsDT.Rows[0]["TechCatalogOperationsGroupID"] != DBNull.Value)
                TechCatalogOperationsGroupID = Convert.ToInt32(DyeingAssignmentsDT.Rows[0]["TechCatalogOperationsGroupID"]);
            if (DyeingAssignmentsDT.Rows[0]["PrintDateTime"] != DBNull.Value)
                PrintDateTime = Convert.ToDateTime(DyeingAssignmentsDT.Rows[0]["PrintDateTime"]);

            SelectCommand = @"SELECT DISTINCT GroupType, WorkAssignmentID, MegaBatchID, BatchID FROM DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        WorkAssignmentID = Convert.ToInt32(DT.Rows[0]["WorkAssignmentID"]);
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (Convert.ToInt32(DT.Rows[i]["GroupType"]) == 0)
                                BatchName += "З(" + DT.Rows[i]["MegaBatchID"].ToString() + "," + DT.Rows[i]["BatchID"].ToString() + ")+";
                            if (Convert.ToInt32(DT.Rows[i]["GroupType"]) == 1)
                                BatchName += "М(" + DT.Rows[i]["MegaBatchID"].ToString() + "," + DT.Rows[i]["BatchID"].ToString() + ")+";
                        }
                        if (BatchName.Length > 0)
                            BatchName = BatchName.Substring(0, BatchName.Length - 1);
                    }
                }
            }

            if (GroupType == 0 || GroupType == 4)
            {
                DataTable DT1 = new DataTable();
                using (DataView DV = new DataView(DyeingCartsDT, string.Empty, "DyeingCartID", DataViewRowState.CurrentRows))
                {
                    DT1 = DV.ToTable(true, new string[] { "DyeingCartID" });
                }
                for (int i = 0; i < DT1.Rows.Count; i++)
                {
                    int CartNumber = Convert.ToInt32(DyeingCartsDT.Rows[i]["CartNumber"]);
                    int DyeingCartID = Convert.ToInt32(DyeingCartsDT.Rows[i]["DyeingCartID"]);
                    decimal TotalSquare = Convert.ToInt32(DyeingCartsDT.Rows[i]["Square"]);

                    ClearOrders();

                    GetFrontsOrders1(ref KansasOrdersDT, DyeingCartID, Fronts.Kansas);
                    GetFrontsOrders1(ref LorenzoOrdersDT, DyeingCartID, Fronts.Lorenzo);
                    GetFrontsOrders1(ref SofiaOrdersDT, DyeingCartID, Fronts.Sofia);
                    GetFrontsOrders1(ref Turin1OrdersDT, DyeingCartID, Fronts.Turin1);
                    GetFrontsOrders1(ref Turin3OrdersDT, DyeingCartID, Fronts.Turin3);
                    GetFrontsOrders1(ref InfinitiOrdersDT, DyeingCartID, Fronts.Infiniti);
                    GetFrontsOrdersGeneva1(ref GenevaOrdersDT, DyeingCartID);
                    GetFrontsOrdersTafel1(ref TafelOrdersDT, DyeingCartID, Fronts.Tafel2, Fronts.Tafel3, Fronts.Tafel3Gl);

                    if (KansasOrdersDT.Rows.Count == 0 && LorenzoOrdersDT.Rows.Count == 0 && SofiaOrdersDT.Rows.Count == 0 &&
                        Turin1OrdersDT.Rows.Count == 0 && Turin3OrdersDT.Rows.Count == 0 &&
                        InfinitiOrdersDT.Rows.Count == 0 && GenevaOrdersDT.Rows.Count == 0 && TafelOrdersDT.Rows.Count == 0)
                        continue; ;

                    int MainOrderID = 0;
                    string ClientName = string.Empty;
                    string OrderName = string.Empty;
                    string SheetName = string.Empty;

                    SelectCommand = @"SELECT MainOrderID, DocNumber, ClientName FROM MainOrders
                    INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
                    WHERE MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingCartID=" + DyeingCartID + ")";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);
                            if (DT.Rows.Count > 0)
                            {
                                SheetName = "ЗОВ";

                                #region Create fonts and styles

                                HSSFFont Calibri11F = hssfworkbook.CreateFont();
                                Calibri11F.FontHeightInPoints = 11;
                                Calibri11F.FontName = "Calibri";

                                HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
                                CalibriBold11F.FontHeightInPoints = 11;
                                CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
                                CalibriBold11F.FontName = "Calibri";

                                HSSFFont Serif8F = hssfworkbook.CreateFont();
                                Serif8F.FontHeightInPoints = 8;
                                Serif8F.FontName = "MS Sans Serif";

                                HSSFFont Serif10F = hssfworkbook.CreateFont();
                                Serif10F.FontHeightInPoints = 10;
                                Serif10F.FontName = "MS Sans Serif";

                                HSSFFont SerifBold10F = hssfworkbook.CreateFont();
                                SerifBold10F.FontHeightInPoints = 10;
                                SerifBold10F.FontName = "MS Sans Serif";
                                SerifBold10F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

                                HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
                                Calibri11CS.SetFont(Calibri11F);

                                HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
                                CalibriBold11CS.SetFont(CalibriBold11F);

                                HSSFCellStyle TableHeaderCS = hssfworkbook.CreateCellStyle();
                                TableHeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                                TableHeaderCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
                                TableHeaderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                TableHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
                                TableHeaderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                TableHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
                                TableHeaderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
                                TableHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
                                TableHeaderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
                                TableHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
                                TableHeaderCS.SetFont(Serif8F);

                                HSSFCellStyle TableHeaderDecCS = hssfworkbook.CreateCellStyle();
                                TableHeaderDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                                TableHeaderDecCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
                                TableHeaderDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                TableHeaderDecCS.BottomBorderColor = HSSFColor.BLACK.index;
                                TableHeaderDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                TableHeaderDecCS.LeftBorderColor = HSSFColor.BLACK.index;
                                TableHeaderDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
                                TableHeaderDecCS.RightBorderColor = HSSFColor.BLACK.index;
                                TableHeaderDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
                                TableHeaderDecCS.TopBorderColor = HSSFColor.BLACK.index;
                                TableHeaderDecCS.SetFont(Serif8F);

                                HSSFCellStyle MaterialCS = hssfworkbook.CreateCellStyle();
                                MaterialCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                                MaterialCS.SetFont(Serif8F);

                                HSSFCellStyle TotamAmountCS = hssfworkbook.CreateCellStyle();
                                TotamAmountCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
                                TotamAmountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                TotamAmountCS.LeftBorderColor = HSSFColor.BLACK.index;
                                TotamAmountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
                                TotamAmountCS.RightBorderColor = HSSFColor.BLACK.index;
                                TotamAmountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
                                TotamAmountCS.TopBorderColor = HSSFColor.BLACK.index;
                                TotamAmountCS.SetFont(SerifBold10F);

                                HSSFCellStyle TotamAmountDecCS = hssfworkbook.CreateCellStyle();
                                TotamAmountDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                                TotamAmountCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
                                TotamAmountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                TotamAmountCS.LeftBorderColor = HSSFColor.BLACK.index;
                                TotamAmountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
                                TotamAmountCS.RightBorderColor = HSSFColor.BLACK.index;
                                TotamAmountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
                                TotamAmountCS.TopBorderColor = HSSFColor.BLACK.index;
                                TotamAmountCS.SetFont(SerifBold10F);

                                #endregion

                                //HSSFSheet sheet1 = hssfworkbook.CreateSheet(SheetName);
                                //sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

                                //sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
                                //sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
                                //sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
                                //sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

                                //sheet1.SetColumnWidth(0, 25 * 256);
                                //sheet1.SetColumnWidth(1, 15 * 256);
                                //sheet1.SetColumnWidth(2, 25 * 256);
                                //sheet1.SetColumnWidth(3, 6 * 256);
                                //sheet1.SetColumnWidth(4, 6 * 256);
                                //sheet1.SetColumnWidth(5, 6 * 256);

                                HSSFCell cell = null;
                                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Распечатал: Дата/время " + CurrentDate.ToString("dd.MM.yyyy HH:mm") + " \r\n ФИО: " + Security.CurrentUserShortName);
                                cell.CellStyle = Calibri11CS;
                                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Плановое время выполнения:");
                                cell.CellStyle = Calibri11CS;
                                if (ResponsibleDateTime != DBNull.Value)
                                {
                                    cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                        "Ответственный: " + GetUserName(Convert.ToInt32(ResponsibleUserID)) + " " + Convert.ToDateTime(ResponsibleDateTime).ToString("dd.MM.yyyy HH:mm"));
                                    cell.CellStyle = Calibri11CS;
                                }
                                if (TechnologyDateTime != DBNull.Value)
                                {
                                    cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                        "Технолог. сопровождение: " + GetUserName(Convert.ToInt32(TechnologyUserID)) + " " + Convert.ToDateTime(TechnologyDateTime).ToString("dd.MM.yyyy HH:mm"));
                                    cell.CellStyle = Calibri11CS;
                                }
                                if (ControlDateTime != DBNull.Value)
                                {
                                    cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                        "Утвердил: " + GetUserName(Convert.ToInt32(ControlUserID)) + " " + Convert.ToDateTime(ControlDateTime).ToString("dd.MM.yyyy HH:mm"));
                                    cell.CellStyle = Calibri11CS;
                                }
                                if (AgreementDateTime != DBNull.Value)
                                {
                                    cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                        "Согласовал: " + GetUserName(Convert.ToInt32(AgreementUserID)) + " " + Convert.ToDateTime(AgreementDateTime).ToString("dd.MM.yyyy HH:mm"));
                                    cell.CellStyle = Calibri11CS;
                                }
                                if (PrintDateTime != DBNull.Value)
                                {
                                    cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                        "Распечатал: " + GetUserName(Convert.ToInt32(PrintUserID)) + " " + Convert.ToDateTime(PrintDateTime).ToString("dd.MM.yyyy HH:mm"));
                                    cell.CellStyle = Calibri11CS;
                                }
                                //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал: ");
                                //cell.CellStyle = Calibri11CS;
                                //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, ShortName);
                                //cell.CellStyle = Calibri11CS;
                                RowIndex++;

                                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, "УТВЕРЖДАЮ_____________");
                                cell.CellStyle = Calibri11CS;

                                decimal AllTotalSquare = 0;
                                int AllTotalAmount = 0;

                                DataTable TempGenevaOrdersDT = GenevaOrdersDT.Copy();
                                DataTable TempTafelOrdersDT = TafelOrdersDT.Copy();
                                for (int c = 0; c < DT.Rows.Count; c++)
                                {
                                    MainOrderID = Convert.ToInt32(DT.Rows[c]["MainOrderID"]);
                                    ClientName = DT.Rows[c]["ClientName"].ToString();
                                    OrderName = DT.Rows[c]["DocNumber"].ToString();

                                    DataTable TempKansasOrdersDT = KansasOrdersDT.Clone();
                                    DataRow[] rows = KansasOrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        TempKansasOrdersDT.Rows.Add(item.ItemArray);
                                    }
                                    DataTable TempLorenzoOrdersDT = LorenzoOrdersDT.Clone();
                                    rows = LorenzoOrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        TempLorenzoOrdersDT.Rows.Add(item.ItemArray);
                                    }
                                    DataTable TempSofiaOrdersDT = SofiaOrdersDT.Clone();
                                    rows = SofiaOrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        TempSofiaOrdersDT.Rows.Add(item.ItemArray);
                                    }

                                    DataTable TempTurin1OrdersDT = Turin1OrdersDT.Clone();
                                    rows = Turin1OrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        TempTurin1OrdersDT.Rows.Add(item.ItemArray);
                                    }

                                    DataTable TempTurin3OrdersDT = Turin3OrdersDT.Clone();
                                    rows = Turin3OrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        TempTurin3OrdersDT.Rows.Add(item.ItemArray);
                                    }

                                    DataTable TempInfinitiOrdersDT = InfinitiOrdersDT.Clone();
                                    rows = InfinitiOrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        TempInfinitiOrdersDT.Rows.Add(item.ItemArray);
                                    }
                                    GenevaOrdersDT.Clear();
                                    rows = TempGenevaOrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        GenevaOrdersDT.Rows.Add(item.ItemArray);
                                    }

                                    TafelOrdersDT.Clear();
                                    rows = TempTafelOrdersDT.Select("MainOrderID=" + MainOrderID);
                                    foreach (DataRow item in rows)
                                    {
                                        AllTotalAmount += Convert.ToInt32(item["PlanCount"]);
                                        AllTotalSquare += Convert.ToDecimal(item["Square"]);
                                        TafelOrdersDT.Rows.Add(item.ItemArray);
                                    }

                                    KansasSimpleDT.Clear();
                                    LorenzoSimpleDT.Clear();
                                    SofiaSimpleDT.Clear();
                                    Turin1SimpleDT.Clear();
                                    Turin3SimpleDT.Clear();
                                    InfinitiSimpleDT.Clear();

                                    GetSimpleFronts(TempKansasOrdersDT, ref KansasSimpleDT, 222);
                                    GetSimpleFronts(TempLorenzoOrdersDT, ref LorenzoSimpleDT, 222);
                                    GetSimpleFronts(TempSofiaOrdersDT, ref SofiaSimpleDT, 222);
                                    GetSimpleFronts(TempTurin1OrdersDT, ref Turin1SimpleDT, 175);
                                    GetSimpleFronts(TempTurin3OrdersDT, ref Turin3SimpleDT, 175);
                                    GetSimpleFronts(TempInfinitiOrdersDT, ref InfinitiSimpleDT, 222);

                                    KansasGridsDT.Clear();
                                    LorenzoGridsDT.Clear();
                                    SofiaGridsDT.Clear();
                                    Turin1GridsDT.Clear();
                                    Turin3GridsDT.Clear();
                                    InfinitiGridsDT.Clear();

                                    SofiaAppliqueDT.Clear();
                                    GetAppliqueFronts(TempSofiaOrdersDT, ref SofiaAppliqueDT);

                                    GetGridFronts(TempKansasOrdersDT, ref KansasGridsDT);
                                    GetGridFronts(TempLorenzoOrdersDT, ref LorenzoGridsDT);
                                    GetGridFronts(TempSofiaOrdersDT, ref SofiaGridsDT);
                                    GetGridFronts(TempTurin1OrdersDT, ref Turin1GridsDT);
                                    GetGridFronts(TempTurin3OrdersDT, ref Turin3GridsDT);
                                    GetGridFronts(TempInfinitiOrdersDT, ref InfinitiGridsDT);

                                    KansasBoxesDT.Clear();
                                    LorenzoBoxesDT.Clear();
                                    SofiaBoxesDT.Clear();
                                    Turin1BoxesDT.Clear();
                                    Turin3BoxesDT.Clear();
                                    InfinitiBoxesDT.Clear();

                                    GetBoxFronts(TempKansasOrdersDT, ref KansasBoxesDT, 222);
                                    GetBoxFronts(TempLorenzoOrdersDT, ref LorenzoBoxesDT, 222);
                                    GetBoxFronts(TempSofiaOrdersDT, ref SofiaBoxesDT, 222);
                                    GetBoxFronts(TempTurin1OrdersDT, ref Turin1BoxesDT, 175);
                                    GetBoxFronts(TempTurin3OrdersDT, ref Turin3BoxesDT, 175);
                                    GetBoxFronts(TempInfinitiOrdersDT, ref InfinitiBoxesDT, 222);

                                    DeyingToExcel(DyeingAssignmentID);

                                    if (DeyingDT.Rows.Count == 0)
                                        continue;

                                    DeyingToExcelSingly0(ref hssfworkbook, ref sheet1,
                                        Calibri11CS, CalibriBold11CS, MaterialCS, TableHeaderCS, TableHeaderDecCS,
                                        DyeingAssignmentID, DyeingCartID, WorkAssignmentID, BatchName, SheetName, ClientName, OrderName, Notes, ref RowIndex);
                                    RowIndex++;
                                }

                                AllTotalSquare = Decimal.Round(AllTotalSquare, 3, MidpointRounding.AwayFromZero);
                                NeedPaintingMaterialManager.CalcOperationDetailConsumption(AllTotalSquare, AllTotalAmount);

                                DataTable OperationsDetailDT = NeedPaintingMaterialManager.CopyOperationsDetail(TechCatalogOperationsGroupID);
                                if (OperationsDetailDT == null)
                                    return;

                                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Операция");
                                cell.CellStyle = CalibriBold11CS;
                                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "План. время");
                                cell.CellStyle = CalibriBold11CS;
                                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Материалы");
                                cell.CellStyle = CalibriBold11CS;
                                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "План. расход");
                                cell.CellStyle = CalibriBold11CS;

                                for (int c = 0; c < OperationsDetailDT.Rows.Count; c++)
                                {
                                    RowIndex++;
                                    decimal Consumption = 0;
                                    int TechCatalogOperationsDetailID = Convert.ToInt32(OperationsDetailDT.Rows[c]["TechCatalogOperationsDetailID"]);
                                    string MachinesOperationName = string.Empty;
                                    string Measure = string.Empty;

                                    if (OperationsDetailDT.Rows[c]["Consumption"] != DBNull.Value)
                                        Consumption = Convert.ToDecimal(OperationsDetailDT.Rows[c]["Consumption"]);
                                    if (OperationsDetailDT.Rows[c]["Measure"] != DBNull.Value)
                                        Measure = OperationsDetailDT.Rows[c]["Measure"].ToString();
                                    if (OperationsDetailDT.Rows[c]["MachinesOperationName"] != DBNull.Value)
                                        MachinesOperationName = OperationsDetailDT.Rows[c]["MachinesOperationName"].ToString();

                                    cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                                    cell.SetCellValue(MachinesOperationName);
                                    cell.CellStyle = CalibriBold11CS;

                                    cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                                    cell.SetCellValue(Convert.ToDouble(Consumption) + " ч");
                                    cell.CellStyle = MaterialCS;

                                    //cell = sheet1.CreateRow(RowIndex++).CreateCell(2);
                                    //cell.SetCellValue("ч");
                                    //cell.CellStyle = CalibriBold11CS;

                                    anchor = new HSSFClientAnchor(500, 1, 1023, 8, 4, RowIndex, 6, RowIndex + 2)
                                    {
                                        AnchorType = 2
                                    };
                                    string BarcodeNumber = ControlAssignmentsManager.GetBarcodeNumber(14, ControlAssignmentsManager.GetDyeingAssignmentBarcode(DyeingCartID, TechCatalogOperationsDetailID));
                                    string FileName = System.Environment.GetEnvironmentVariable("TEMP") + @"/" + BarcodeNumber + ".jpg";
                                    picture = patriarch.CreatePicture(anchor, LoadImage(FileName, hssfworkbook));

                                    cell = sheet1.CreateRow(RowIndex + 1).CreateCell(5);
                                    cell.SetCellValue(BarcodeNumber);
                                    cell.CellStyle = MaterialCS;

                                    DataRow NewRow = ImagesToDeleteDT.NewRow();
                                    NewRow["FileName"] = FileName;
                                    ImagesToDeleteDT.Rows.Add(NewRow);

                                    DataTable StoreDetailDT = NeedPaintingMaterialManager.CopyStoreDetail(TechCatalogOperationsDetailID);

                                    if (StoreDetailDT == null)
                                        continue;
                                    for (int x = 0; x < StoreDetailDT.Rows.Count; x++)
                                    {
                                        Consumption = 0;
                                        Measure = string.Empty;
                                        string TechStoreName = string.Empty;
                                        if (StoreDetailDT.Rows[x]["Consumption"] != DBNull.Value)
                                            Consumption = Convert.ToDecimal(StoreDetailDT.Rows[x]["Consumption"]);
                                        if (StoreDetailDT.Rows[x]["Measure"] != DBNull.Value)
                                            Measure = StoreDetailDT.Rows[x]["Measure"].ToString();
                                        if (StoreDetailDT.Rows[x]["TechStoreName"] != DBNull.Value)
                                            TechStoreName = StoreDetailDT.Rows[x]["TechStoreName"].ToString();

                                        cell = sheet1.CreateRow(RowIndex).CreateCell(2);
                                        cell.SetCellValue(TechStoreName);
                                        cell.CellStyle = MaterialCS;

                                        cell = sheet1.CreateRow(RowIndex++).CreateCell(3);
                                        cell.SetCellValue(Convert.ToDouble(Consumption) + " " + Measure);
                                        cell.CellStyle = MaterialCS;

                                        //cell = sheet1.CreateRow(RowIndex++).CreateCell(2);
                                        //cell.SetCellValue(Measure);
                                        //cell.CellStyle = MaterialCS;
                                    }

                                    RowIndex++;
                                    RowIndex++;

                                }
                                RowIndex++;
                                RowIndex++;
                                RowIndex++;
                            }
                        }
                    }
                }
            }

            if (GroupType == 2)
            {
                DataTable DT1 = new DataTable();
                using (DataView DV = new DataView(DyeingCartsDT, string.Empty, "DyeingCartID", DataViewRowState.CurrentRows))
                {
                    DT1 = DV.ToTable(true, new string[] { "DyeingCartID" });
                }
                for (int i = 0; i < DT1.Rows.Count; i++)
                {
                    int CartNumber = Convert.ToInt32(DyeingCartsDT.Rows[i]["CartNumber"]);
                    int DyeingCartID = Convert.ToInt32(DyeingCartsDT.Rows[i]["DyeingCartID"]);
                    decimal TotalSquare = Convert.ToInt32(DyeingCartsDT.Rows[i]["Square"]);

                    ClearOrders();

                    GetFrontsOrders1(ref KansasOrdersDT, DyeingCartID, Fronts.Kansas);
                    GetFrontsOrders1(ref LorenzoOrdersDT, DyeingCartID, Fronts.Lorenzo);
                    GetFrontsOrders1(ref SofiaOrdersDT, DyeingCartID, Fronts.Sofia);
                    GetFrontsOrders1(ref Turin1OrdersDT, DyeingCartID, Fronts.Turin1);
                    GetFrontsOrders1(ref Turin3OrdersDT, DyeingCartID, Fronts.Turin3);
                    GetFrontsOrders1(ref InfinitiOrdersDT, DyeingCartID, Fronts.Infiniti);
                    GetFrontsOrdersGeneva1(ref GenevaOrdersDT, DyeingCartID);
                    GetFrontsOrdersTafel1(ref TafelOrdersDT, DyeingCartID, Fronts.Tafel2, Fronts.Tafel3, Fronts.Tafel3Gl);

                    if (KansasOrdersDT.Rows.Count == 0 && LorenzoOrdersDT.Rows.Count == 0 && SofiaOrdersDT.Rows.Count == 0 &&
                        Turin1OrdersDT.Rows.Count == 0 && Turin3OrdersDT.Rows.Count == 0 &&
                        InfinitiOrdersDT.Rows.Count == 0 && GenevaOrdersDT.Rows.Count == 0 && TafelOrdersDT.Rows.Count == 0)
                        continue; ;

                    KansasSimpleDT.Clear();
                    LorenzoSimpleDT.Clear();
                    SofiaSimpleDT.Clear();
                    Turin1SimpleDT.Clear();
                    Turin3SimpleDT.Clear();
                    InfinitiSimpleDT.Clear();

                    GetSimpleFronts(KansasOrdersDT, ref KansasSimpleDT, 222);
                    GetSimpleFronts(LorenzoOrdersDT, ref LorenzoSimpleDT, 222);
                    GetSimpleFronts(SofiaOrdersDT, ref SofiaSimpleDT, 222);
                    GetSimpleFronts(Turin1OrdersDT, ref Turin1SimpleDT, 175);
                    GetSimpleFronts(Turin3OrdersDT, ref Turin3SimpleDT, 175);
                    GetSimpleFronts(InfinitiOrdersDT, ref InfinitiSimpleDT, 222);

                    KansasGridsDT.Clear();
                    LorenzoGridsDT.Clear();
                    SofiaGridsDT.Clear();
                    Turin1GridsDT.Clear();
                    Turin3GridsDT.Clear();
                    InfinitiGridsDT.Clear();

                    SofiaAppliqueDT.Clear();
                    GetAppliqueFronts(SofiaOrdersDT, ref SofiaAppliqueDT);

                    GetGridFronts(KansasOrdersDT, ref KansasGridsDT);
                    GetGridFronts(LorenzoOrdersDT, ref LorenzoGridsDT);
                    GetGridFronts(SofiaOrdersDT, ref SofiaGridsDT);
                    GetGridFronts(Turin1OrdersDT, ref Turin1GridsDT);
                    GetGridFronts(Turin3OrdersDT, ref Turin3GridsDT);
                    GetGridFronts(InfinitiOrdersDT, ref InfinitiGridsDT);

                    KansasBoxesDT.Clear();
                    LorenzoBoxesDT.Clear();
                    SofiaBoxesDT.Clear();
                    Turin1BoxesDT.Clear();
                    Turin3BoxesDT.Clear();
                    InfinitiBoxesDT.Clear();

                    GetBoxFronts(KansasOrdersDT, ref KansasBoxesDT, 222);
                    GetBoxFronts(LorenzoOrdersDT, ref LorenzoBoxesDT, 222);
                    GetBoxFronts(SofiaOrdersDT, ref SofiaBoxesDT, 222);
                    GetBoxFronts(Turin1OrdersDT, ref Turin1BoxesDT, 175);
                    GetBoxFronts(Turin3OrdersDT, ref Turin3BoxesDT, 175);
                    GetBoxFronts(InfinitiOrdersDT, ref InfinitiBoxesDT, 222);

                    DeyingToExcel(DyeingAssignmentID);

                    if (DeyingDT.Rows.Count == 0)
                        continue;
                    CreateSheet(ref hssfworkbook, ref sheet1, GroupType, DyeingAssignmentID, DyeingCartID, CartNumber, WorkAssignmentID, BatchName, Notes, TotalSquare, ref RowIndex);
                }
            }

            if (GroupType == 1)
            {
                DataTable DT1 = new DataTable();
                using (DataView DV = new DataView(DyeingCartsDT, string.Empty, "DyeingCartID", DataViewRowState.CurrentRows))
                {
                    DT1 = DV.ToTable(true, new string[] { "DyeingCartID" });
                }
                for (int i = 0; i < DT1.Rows.Count; i++)
                {
                    int CartNumber = Convert.ToInt32(DyeingCartsDT.Rows[i]["CartNumber"]);
                    int DyeingCartID = Convert.ToInt32(DyeingCartsDT.Rows[i]["DyeingCartID"]);
                    decimal TotalSquare = Convert.ToDecimal(DyeingCartsDT.Rows[i]["Square"]);

                    ClearOrders();

                    GetFrontsOrders2(ref KansasOrdersDT, DyeingAssignmentID, Fronts.Kansas);
                    GetFrontsOrders2(ref LorenzoOrdersDT, DyeingAssignmentID, Fronts.Lorenzo);
                    GetFrontsOrders2(ref SofiaOrdersDT, DyeingAssignmentID, Fronts.Sofia);
                    GetFrontsOrders2(ref Turin1OrdersDT, DyeingAssignmentID, Fronts.Turin1);
                    GetFrontsOrders2(ref Turin3OrdersDT, DyeingAssignmentID, Fronts.Turin3);
                    GetFrontsOrders2(ref InfinitiOrdersDT, DyeingAssignmentID, Fronts.Infiniti);
                    GetFrontsOrdersGeneva2(ref GenevaOrdersDT, DyeingAssignmentID);
                    GetFrontsOrdersTafel2(ref TafelOrdersDT, DyeingAssignmentID, Fronts.Tafel2, Fronts.Tafel3, Fronts.Tafel3Gl);

                    if (KansasOrdersDT.Rows.Count == 0 && LorenzoOrdersDT.Rows.Count == 0 && SofiaOrdersDT.Rows.Count == 0 &&
                        Turin1OrdersDT.Rows.Count == 0 && Turin3OrdersDT.Rows.Count == 0 &&
                        InfinitiOrdersDT.Rows.Count == 0 && GenevaOrdersDT.Rows.Count == 0 && TafelOrdersDT.Rows.Count == 0)
                        continue; ;

                    KansasSimpleDT.Clear();
                    LorenzoSimpleDT.Clear();
                    SofiaSimpleDT.Clear();
                    Turin1SimpleDT.Clear();
                    Turin3SimpleDT.Clear();
                    InfinitiSimpleDT.Clear();

                    GetSimpleFronts(KansasOrdersDT, ref KansasSimpleDT, 222);
                    GetSimpleFronts(LorenzoOrdersDT, ref LorenzoSimpleDT, 222);
                    GetSimpleFronts(SofiaOrdersDT, ref SofiaSimpleDT, 222);
                    GetSimpleFronts(Turin1OrdersDT, ref Turin1SimpleDT, 175);
                    GetSimpleFronts(Turin3OrdersDT, ref Turin3SimpleDT, 175);
                    GetSimpleFronts(InfinitiOrdersDT, ref InfinitiSimpleDT, 222);

                    KansasGridsDT.Clear();
                    LorenzoGridsDT.Clear();
                    SofiaGridsDT.Clear();
                    Turin1GridsDT.Clear();
                    Turin3GridsDT.Clear();
                    InfinitiGridsDT.Clear();

                    SofiaAppliqueDT.Clear();
                    GetAppliqueFronts(SofiaOrdersDT, ref SofiaAppliqueDT);

                    GetGridFronts(KansasOrdersDT, ref KansasGridsDT);
                    GetGridFronts(LorenzoOrdersDT, ref LorenzoGridsDT);
                    GetGridFronts(SofiaOrdersDT, ref SofiaGridsDT);
                    GetGridFronts(Turin1OrdersDT, ref Turin1GridsDT);
                    GetGridFronts(Turin3OrdersDT, ref Turin3GridsDT);
                    GetGridFronts(InfinitiOrdersDT, ref InfinitiGridsDT);

                    KansasBoxesDT.Clear();
                    LorenzoBoxesDT.Clear();
                    SofiaBoxesDT.Clear();
                    Turin1BoxesDT.Clear();
                    Turin3BoxesDT.Clear();
                    InfinitiBoxesDT.Clear();

                    GetBoxFronts(KansasOrdersDT, ref KansasBoxesDT, 222);
                    GetBoxFronts(LorenzoOrdersDT, ref LorenzoBoxesDT, 222);
                    GetBoxFronts(SofiaOrdersDT, ref SofiaBoxesDT, 222);
                    GetBoxFronts(Turin1OrdersDT, ref Turin1BoxesDT, 175);
                    GetBoxFronts(Turin3OrdersDT, ref Turin3BoxesDT, 175);
                    GetBoxFronts(InfinitiOrdersDT, ref InfinitiBoxesDT, 222);

                    DeyingToExcel(DyeingAssignmentID);

                    if (DeyingDT.Rows.Count == 0)
                        continue;
                    CreateSheet(ref hssfworkbook, ref sheet1, GroupType, DyeingAssignmentID, DyeingCartID, CartNumber, WorkAssignmentID, BatchName, Notes, TotalSquare, ref RowIndex);
                }
            }
        }

        public void GetCurrentDate()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    CurrentDate = Convert.ToDateTime(DT.Rows[0][0]);
                }
            }
        }

        public void CreateSheet(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1, int GroupType, int DyeingAssignmentID, int DyeingCartID, int CartNumber,
            int WorkAssignmentID, string BatchName, string Notes, decimal TotalSquare, ref int RowIndex)
        {
            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont Serif8F = hssfworkbook.CreateFont();
            Serif8F.FontHeightInPoints = 8;
            Serif8F.FontName = "MS Sans Serif";

            HSSFFont Serif10F = hssfworkbook.CreateFont();
            Serif10F.FontHeightInPoints = 10;
            Serif10F.FontName = "MS Sans Serif";

            HSSFFont SerifBold10F = hssfworkbook.CreateFont();
            SerifBold10F.FontHeightInPoints = 10;
            SerifBold10F.FontName = "MS Sans Serif";
            SerifBold10F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle TableHeaderCS = hssfworkbook.CreateCellStyle();
            TableHeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            TableHeaderCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            TableHeaderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.SetFont(Serif8F);

            HSSFCellStyle TableHeaderDecCS = hssfworkbook.CreateCellStyle();
            TableHeaderDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            TableHeaderDecCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            TableHeaderDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.RightBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.TopBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.SetFont(Serif8F);

            HSSFCellStyle MaterialCS = hssfworkbook.CreateCellStyle();
            MaterialCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            MaterialCS.SetFont(Serif8F);

            HSSFCellStyle TotamAmountCS = hssfworkbook.CreateCellStyle();
            TotamAmountCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotamAmountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.RightBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.TopBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.SetFont(SerifBold10F);

            HSSFCellStyle TotamAmountDecCS = hssfworkbook.CreateCellStyle();
            TotamAmountDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            TotamAmountCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotamAmountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.RightBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.TopBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.SetFont(SerifBold10F);

            #endregion

            string ClientName = string.Empty;
            string OrderName = string.Empty;
            string SheetName = string.Empty;
            if (GroupType == 1)
            {
                SheetName = "Тележка " + CartNumber.ToString();
                DeyingToExcelSingly1(ref hssfworkbook, ref sheet1, Calibri11CS, CalibriBold11CS, MaterialCS, TableHeaderCS, TableHeaderDecCS,
                    DyeingAssignmentID, DyeingCartID, CartNumber, WorkAssignmentID, BatchName, SheetName, Notes, TotalSquare, ref RowIndex);
            }
            if (GroupType == 2)
            {
                SheetName = "Тележка " + CartNumber.ToString();
                DeyingToExcelSingly2(ref hssfworkbook, ref sheet1, Calibri11CS, CalibriBold11CS, MaterialCS, TableHeaderCS, TableHeaderDecCS,
                    DyeingAssignmentID, DyeingCartID, CartNumber, WorkAssignmentID, BatchName, SheetName, Notes, ref RowIndex);
            }

        }

        private string GetFileName(string sDestFolder, string ExcelName)
        {
            string sExtension = ".xls";
            string sFileName = ExcelName;

            int j = 1;
            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
            {
                sFileName = ExcelName + "(" + j++ + ")";
            }
            sFileName = sFileName + sExtension;
            return sFileName;
        }

        private void DeyingToExcel(int DyeingAssignmentID)
        {
            DeyingDT.Clear();

            DataTable DT1 = new DataTable();

            using (DataView DV = new DataView(KansasOrdersDT, string.Empty, "ColorID", DataViewRowState.CurrentRows))
            {
                DT1.Clear();
                DT1 = DV.ToTable(true, new string[] { "ColorID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), KansasSimpleDT, ref DeyingDT, string.Empty);
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), KansasBoxesDT, ref DeyingDT, " ШУФ");
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), KansasGridsDT, ref DeyingDT, " РЕШ");
            }

            using (DataView DV = new DataView(LorenzoOrdersDT, string.Empty, "ColorID", DataViewRowState.CurrentRows))
            {
                DT1.Clear();
                DT1 = DV.ToTable(true, new string[] { "ColorID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), LorenzoSimpleDT, ref DeyingDT, string.Empty);
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), LorenzoBoxesDT, ref DeyingDT, " ШУФ");
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), LorenzoGridsDT, ref DeyingDT, " РЕШ");
            }

            using (DataView DV = new DataView(SofiaOrdersDT, string.Empty, "ColorID", DataViewRowState.CurrentRows))
            {
                DT1.Clear();
                DT1 = DV.ToTable(true, new string[] { "ColorID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), SofiaSimpleDT, ref DeyingDT, string.Empty);
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), SofiaBoxesDT, ref DeyingDT, " ШУФ");
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), SofiaGridsDT, ref DeyingDT, " РЕШ");
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), SofiaAppliqueDT, ref DeyingDT, " Аппл");
            }
            using (DataView DV = new DataView(Turin1OrdersDT, string.Empty, "ColorID", DataViewRowState.CurrentRows))
            {
                DT1.Clear();
                DT1 = DV.ToTable(true, new string[] { "ColorID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), Turin1SimpleDT, ref DeyingDT, string.Empty);
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), Turin1BoxesDT, ref DeyingDT, " ШУФ");
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), Turin1GridsDT, ref DeyingDT, " РЕШ");
            }
            using (DataView DV = new DataView(Turin3OrdersDT, string.Empty, "ColorID", DataViewRowState.CurrentRows))
            {
                DT1.Clear();
                DT1 = DV.ToTable(true, new string[] { "ColorID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), Turin3SimpleDT, ref DeyingDT, string.Empty);
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), Turin3BoxesDT, ref DeyingDT, " ШУФ");
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), Turin3GridsDT, ref DeyingDT, " РЕШ");
            }
            using (DataView DV = new DataView(InfinitiOrdersDT, string.Empty, "ColorID", DataViewRowState.CurrentRows))
            {
                DT1.Clear();
                DT1 = DV.ToTable(true, new string[] { "ColorID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), InfinitiSimpleDT, ref DeyingDT, string.Empty);
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), InfinitiBoxesDT, ref DeyingDT, " ШУФ");
                CollectDeying(Convert.ToInt32(DT1.Rows[i]["ColorID"]), InfinitiGridsDT, ref DeyingDT, " РЕШ");
            }

            CollectDeying(GenevaOrdersDT, ref DeyingDT);

            GroupAndSortColoredFronts(TafelOrdersDT, ref DeyingDT);
            GroupAndSortNotGlossFronts(TafelOrdersDT, ref DeyingDT);
        }

        public static int LoadImage(string path, HSSFWorkbook wb)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, (int)file.Length);
            return wb.AddPicture(buffer, HSSFWorkbook.PICTURE_TYPE_JPEG);

        }

        public void DeyingToExcelSingly0(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle Calibri11CS, HSSFCellStyle CalibriBold11CS, HSSFCellStyle MaterialCS, HSSFCellStyle TableHeaderCS, HSSFCellStyle TableHeaderDecCS,
            int DyeingAssignmentID, int DyeingCartID, int WorkAssignmentID, string BatchName, string SheetName, string ClientName, string OrderName, string Notes, ref int RowIndex)
        {
            DataTable DT = DeyingDT.Copy();
            DT.Columns["Square"].SetOrdinal(6);

            HSSFCell cell = null;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Задание №" + WorkAssignmentID.ToString());
            cell.CellStyle = CalibriBold11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, BatchName);
            cell.CellStyle = CalibriBold11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Партия №" + DyeingAssignmentID.ToString());
            //cell.CellStyle = CalibriBold11CS;
            if (OrderName.Length > 0)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Клиент:");
                cell.CellStyle = CalibriBold11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, ClientName);
                cell.CellStyle = CalibriBold11CS;
            }
            if (OrderName.Length > 0)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Заказ:");
                cell.CellStyle = CalibriBold11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, OrderName);
                cell.CellStyle = CalibriBold11CS;
            }
            if (Notes.Length > 0)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + Notes);
                cell.CellStyle = CalibriBold11CS;
            }

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Профиль");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Цвет профиля");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Цвет наполнителя");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Высота");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Ширина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 5, "Кол-во");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 6, "м.кв.");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            int CType = -1;
            int AllTotalAmount = 0;
            int TotalAmount = 0;
            decimal AllTotalSquare = 0;
            decimal TotalSquare = 0;
            int DifferentDecorCount = 0;

            using (DataView DV = new DataView(DT))
            {
                DifferentDecorCount = DV.ToTable(true, new string[] { "ColorType" }).Rows.Count;
            }

            if (DT.Rows.Count > 0)
                CType = Convert.ToInt32(DT.Rows[0]["ColorType"]);

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                if (DT.Rows[x]["PlanCount"] != DBNull.Value)
                {
                    AllTotalAmount += Convert.ToInt32(DT.Rows[x]["PlanCount"]);
                    TotalAmount += Convert.ToInt32(DT.Rows[x]["PlanCount"]);
                }
                if (DT.Rows[x]["Square"] != DBNull.Value)
                {
                    AllTotalSquare += Convert.ToDecimal(DT.Rows[x]["Square"]);
                    TotalSquare += Convert.ToDecimal(DT.Rows[x]["Square"]);
                }
                DT.Rows[x]["Square"] = Decimal.Round(Convert.ToDecimal(DT.Rows[x]["Square"]), 3, MidpointRounding.AwayFromZero);

                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                        continue;

                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableHeaderDecCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }
                }

                if (x + 1 <= DT.Rows.Count - 1)
                {
                    if (CType != Convert.ToInt32(DT.Rows[x + 1]["ColorType"]))
                    {
                        RowIndex++;
                        for (int y = 0; y < DT.Columns.Count; y++)
                        {
                            if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                                continue;

                            cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(string.Empty);
                            cell.CellStyle = TableHeaderCS;
                            continue;
                        }

                        cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                        cell.SetCellValue("Итого:");
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                        cell.SetCellValue(TotalAmount);
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                        cell.SetCellValue(Convert.ToDouble(Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero)));
                        cell.CellStyle = TableHeaderCS;

                        CType = Convert.ToInt32(DT.Rows[x + 1]["ColorType"]);
                        TotalAmount = 0;
                        TotalSquare = 0;
                        RowIndex++;
                    }
                }

                if (x == DT.Rows.Count - 1)
                {
                    if (DifferentDecorCount > 1)
                    {
                        RowIndex++;
                        for (int y = 0; y < DT.Columns.Count; y++)
                        {
                            if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                                continue;

                            cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(string.Empty);
                            cell.CellStyle = TableHeaderCS;
                            continue;
                        }

                        cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                        cell.SetCellValue("Итого:");
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                        cell.SetCellValue(TotalAmount);
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                        cell.SetCellValue(Convert.ToDouble(Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero)));
                        cell.CellStyle = TableHeaderDecCS;
                    }
                    RowIndex++;

                    for (int y = 0; y < DT.Columns.Count; y++)
                    {
                        if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                            continue;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(string.Empty);
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }

                    cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                    cell.SetCellValue("Всего:");
                    cell.CellStyle = TableHeaderCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                    cell.SetCellValue(AllTotalAmount);
                    cell.CellStyle = TableHeaderCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                    cell.SetCellValue(Convert.ToDouble(Decimal.Round(AllTotalSquare, 3, MidpointRounding.AwayFromZero)));
                    cell.CellStyle = TableHeaderDecCS;
                }
                RowIndex++;
            }

            //for (int i = 0; i < DyeingCartsDT.Rows.Count; i++)
            //{
            //    CartNumber = Convert.ToInt32(DyeingCartsDT.Rows[i]["CartNumber"]);
            //    TotalSquare = Convert.ToDecimal(DyeingCartsDT.Rows[i]["Square"]);

            //    cell = sheet1.CreateRow(RowIndex).CreateCell(0);
            //    cell.SetCellValue("Тележка №" + CartNumber);
            //    cell.CellStyle = CalibriBold11CS;

            //    cell = sheet1.CreateRow(RowIndex).CreateCell(1);
            //    cell.SetCellValue(Convert.ToDouble(TotalSquare));
            //    cell.CellStyle = CalibriBold11CS;

            //    cell = sheet1.CreateRow(RowIndex++).CreateCell(2);
            //    cell.SetCellValue("м.кв.");
            //    cell.CellStyle = CalibriBold11CS;
            //}
        }

        public void DeyingToExcelSingly1(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle Calibri11CS, HSSFCellStyle CalibriBold11CS, HSSFCellStyle MaterialCS, HSSFCellStyle TableHeaderCS, HSSFCellStyle TableHeaderDecCS,
            int DyeingAssignmentID, int DyeingCartID, int CartNumber, int WorkAssignmentID, string BatchName, string SheetName, string Notes, decimal CartSquare, ref int RowIndex)
        {
            DataTable DT = DeyingDT.Copy();
            DT.Columns["Square"].SetOrdinal(6);

            //HSSFSheet sheet1 = hssfworkbook.CreateSheet(SheetName);
            //sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            //sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            //sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            //sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            //sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            //sheet1.SetColumnWidth(0, 25 * 256);
            //sheet1.SetColumnWidth(1, 15 * 256);
            //sheet1.SetColumnWidth(2, 20 * 256);
            //sheet1.SetColumnWidth(3, 6 * 256);
            //sheet1.SetColumnWidth(4, 6 * 256);
            //sheet1.SetColumnWidth(5, 6 * 256);

            //HSSFPatriarch patriarch = sheet1.CreateDrawingPatriarch();
            //HSSFClientAnchor anchor;
            //HSSFPicture picture;

            HSSFCell cell = null;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Распечатал: Дата/время " + CurrentDate.ToString("dd.MM.yyyy HH:mm") + " \r\n ФИО: " + Security.CurrentUserShortName);
            cell.CellStyle = Calibri11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Плановое время выполнения:");
            cell.CellStyle = Calibri11CS;
            if (ResponsibleDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Ответственный: " + GetUserName(Convert.ToInt32(ResponsibleUserID)) + " " + Convert.ToDateTime(ResponsibleDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Технолог. сопровождение: " + GetUserName(Convert.ToInt32(TechnologyUserID)) + " " + Convert.ToDateTime(TechnologyDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (ControlDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил: " + GetUserName(Convert.ToInt32(ControlUserID)) + " " + Convert.ToDateTime(ControlDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (AgreementDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Согласовал: " + GetUserName(Convert.ToInt32(AgreementUserID)) + " " + Convert.ToDateTime(AgreementDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Распечатал: " + GetUserName(Convert.ToInt32(PrintUserID)) + " " + Convert.ToDateTime(PrintDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал: ");
            //cell.CellStyle = Calibri11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, ShortName);
            //cell.CellStyle = Calibri11CS;
            RowIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, "УТВЕРЖДАЮ_____________");
            cell.CellStyle = Calibri11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Задание №" + WorkAssignmentID.ToString());
            cell.CellStyle = CalibriBold11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, BatchName);
            cell.CellStyle = CalibriBold11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Партия №" + DyeingAssignmentID.ToString());
            //cell.CellStyle = CalibriBold11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, DyeingAssignmentID.ToString());
            //cell.CellStyle = CalibriBold11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Тележка №" + CartNumber.ToString());
            cell.CellStyle = CalibriBold11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, CartNumber.ToString());
            //cell.CellStyle = CalibriBold11CS;
            if (Notes.Length > 0)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + Notes);
                cell.CellStyle = CalibriBold11CS;
            }

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Профиль");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Цвет профиля");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Цвет наполнителя");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Высота");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Ширина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 5, "Кол-во");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 6, "м.кв.");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            int CType = -1;
            int AllTotalAmount = 0;
            int TotalAmount = 0;
            decimal TotalSquare = 0;
            decimal AllTotalSquare = 0;
            int DifferentDecorCount = 0;

            using (DataView DV = new DataView(DT))
            {
                DifferentDecorCount = DV.ToTable(true, new string[] { "ColorType" }).Rows.Count;
            }

            if (DT.Rows.Count > 0)
                CType = Convert.ToInt32(DT.Rows[0]["ColorType"]);

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                if (DT.Rows[x]["PlanCount"] != DBNull.Value)
                {
                    AllTotalAmount += Convert.ToInt32(DT.Rows[x]["PlanCount"]);
                    TotalAmount += Convert.ToInt32(DT.Rows[x]["PlanCount"]);
                }
                if (DT.Rows[x]["Square"] != DBNull.Value)
                {
                    AllTotalSquare += Convert.ToDecimal(DT.Rows[x]["Square"]);
                    TotalSquare += Convert.ToDecimal(DT.Rows[x]["Square"]);
                }
                DT.Rows[x]["Square"] = Decimal.Round(Convert.ToDecimal(DT.Rows[x]["Square"]), 3, MidpointRounding.AwayFromZero);

                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                        continue;

                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableHeaderDecCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }
                }

                if (x + 1 <= DT.Rows.Count - 1)
                {
                    if (CType != Convert.ToInt32(DT.Rows[x + 1]["ColorType"]))
                    {
                        RowIndex++;
                        for (int y = 0; y < DT.Columns.Count; y++)
                        {
                            if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                                continue;

                            cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(string.Empty);
                            cell.CellStyle = TableHeaderCS;
                            continue;
                        }

                        cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                        cell.SetCellValue("Итого:");
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                        cell.SetCellValue(TotalAmount);
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                        cell.SetCellValue(Convert.ToDouble(Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero)));
                        cell.CellStyle = TableHeaderCS;

                        CType = Convert.ToInt32(DT.Rows[x + 1]["ColorType"]);
                        TotalAmount = 0;
                        TotalSquare = 0;
                        RowIndex++;
                    }
                }

                if (x == DT.Rows.Count - 1)
                {
                    if (DifferentDecorCount > 1)
                    {
                        RowIndex++;
                        for (int y = 0; y < DT.Columns.Count; y++)
                        {
                            if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                                continue;

                            cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(string.Empty);
                            cell.CellStyle = TableHeaderCS;
                            continue;
                        }

                        cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                        cell.SetCellValue("Итого:");
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                        cell.SetCellValue(TotalAmount);
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                        cell.SetCellValue(Convert.ToDouble(Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero)));
                        cell.CellStyle = TableHeaderDecCS;
                    }
                    RowIndex++;

                    for (int y = 0; y < DT.Columns.Count; y++)
                    {
                        if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                            continue;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(string.Empty);
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }

                    cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                    cell.SetCellValue("Всего:");
                    cell.CellStyle = TableHeaderCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                    cell.SetCellValue(AllTotalAmount);
                    cell.CellStyle = TableHeaderCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                    cell.SetCellValue(Convert.ToDouble(Decimal.Round(AllTotalSquare, 3, MidpointRounding.AwayFromZero)));
                    cell.CellStyle = TableHeaderDecCS;
                }
                RowIndex++;
            }

            CartSquare = Decimal.Round(CartSquare, 3, MidpointRounding.AwayFromZero);
            cell = sheet1.CreateRow(RowIndex).CreateCell(0);
            cell.SetCellValue("Тележка №" + CartNumber);
            cell.CellStyle = CalibriBold11CS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(1);
            cell.SetCellValue(Convert.ToDouble(CartSquare));
            cell.CellStyle = CalibriBold11CS;

            cell = sheet1.CreateRow(RowIndex++).CreateCell(2);
            cell.SetCellValue("м.кв.");
            cell.CellStyle = CalibriBold11CS;

            NeedPaintingMaterialManager.CalcOperationDetailConsumption(CartSquare, AllTotalAmount);

            DataTable OperationsDetailDT = NeedPaintingMaterialManager.CopyOperationsDetail(TechCatalogOperationsGroupID);
            if (OperationsDetailDT == null)
                return;
            for (int i = 0; i < OperationsDetailDT.Rows.Count; i++)
            {
                RowIndex++;
                decimal Consumption = 0;
                int TechCatalogOperationsDetailID = Convert.ToInt32(OperationsDetailDT.Rows[i]["TechCatalogOperationsDetailID"]);
                string MachinesOperationName = string.Empty;
                string Measure = string.Empty;

                if (OperationsDetailDT.Rows[i]["Consumption"] != DBNull.Value)
                    Consumption = Convert.ToDecimal(OperationsDetailDT.Rows[i]["Consumption"]);
                if (OperationsDetailDT.Rows[i]["Measure"] != DBNull.Value)
                    Measure = OperationsDetailDT.Rows[i]["Measure"].ToString();
                if (OperationsDetailDT.Rows[i]["MachinesOperationName"] != DBNull.Value)
                    MachinesOperationName = OperationsDetailDT.Rows[i]["MachinesOperationName"].ToString();

                cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                cell.SetCellValue(MachinesOperationName);
                cell.CellStyle = CalibriBold11CS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToDouble(Consumption) + " ч");
                cell.CellStyle = CalibriBold11CS;

                anchor = new HSSFClientAnchor(500, 1, 1023, 8, 4, RowIndex - 1, 6, RowIndex + 1)
                {
                    AnchorType = 2
                };
                string BarcodeNumber = ControlAssignmentsManager.GetBarcodeNumber(14, ControlAssignmentsManager.GetDyeingAssignmentBarcode(DyeingCartID, TechCatalogOperationsDetailID));
                string FileName = System.Environment.GetEnvironmentVariable("TEMP") + @"/" + BarcodeNumber + ".jpg";
                picture = patriarch.CreatePicture(anchor, LoadImage(FileName, hssfworkbook));

                cell = sheet1.CreateRow(RowIndex + 1).CreateCell(5);
                cell.SetCellValue(BarcodeNumber);
                cell.CellStyle = MaterialCS;

                DataRow NewRow = ImagesToDeleteDT.NewRow();
                NewRow["FileName"] = FileName;
                ImagesToDeleteDT.Rows.Add(NewRow);

                DataTable StoreDetailDT = NeedPaintingMaterialManager.CopyStoreDetail(TechCatalogOperationsDetailID);

                if (StoreDetailDT == null)
                    continue;
                for (int j = 0; j < StoreDetailDT.Rows.Count; j++)
                {
                    Consumption = 0;
                    Measure = string.Empty;
                    string TechStoreName = string.Empty;
                    if (StoreDetailDT.Rows[j]["Consumption"] != DBNull.Value)
                        Consumption = Convert.ToDecimal(StoreDetailDT.Rows[j]["Consumption"]);
                    if (StoreDetailDT.Rows[j]["Measure"] != DBNull.Value)
                        Measure = StoreDetailDT.Rows[j]["Measure"].ToString();
                    if (StoreDetailDT.Rows[j]["TechStoreName"] != DBNull.Value)
                        TechStoreName = StoreDetailDT.Rows[j]["TechStoreName"].ToString();

                    cell = sheet1.CreateRow(RowIndex).CreateCell(2);
                    cell.SetCellValue(TechStoreName);
                    cell.CellStyle = MaterialCS;

                    cell = sheet1.CreateRow(RowIndex++).CreateCell(3);
                    cell.SetCellValue(Convert.ToDouble(Consumption) + " " + Measure);
                    cell.CellStyle = MaterialCS;

                }
                RowIndex++;
                RowIndex++;
            }
            RowIndex++;
            RowIndex++;
            RowIndex++;
        }

        public void DeyingToExcelSingly2(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle Calibri11CS, HSSFCellStyle CalibriBold11CS, HSSFCellStyle MaterialCS, HSSFCellStyle TableHeaderCS, HSSFCellStyle TableHeaderDecCS,
            int DyeingAssignmentID, int DyeingCartID, int CartNumber, int WorkAssignmentID, string BatchName, string SheetName, string Notes, ref int RowIndex)
        {
            DataTable DT = DeyingDT.Copy();
            DT.Columns["Square"].SetOrdinal(6);

            HSSFCell cell = null;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Распечатал: Дата/время " + CurrentDate.ToString("dd.MM.yyyy HH:mm") + " \r\n ФИО: " + Security.CurrentUserShortName);
            cell.CellStyle = Calibri11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Плановое время выполнения:");
            cell.CellStyle = Calibri11CS;
            if (ResponsibleDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Ответственный: " + GetUserName(Convert.ToInt32(ResponsibleUserID)) + " " + Convert.ToDateTime(ResponsibleDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Технолог. сопровождение: " + GetUserName(Convert.ToInt32(TechnologyUserID)) + " " + Convert.ToDateTime(TechnologyDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (ControlDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил: " + GetUserName(Convert.ToInt32(ControlUserID)) + " " + Convert.ToDateTime(ControlDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (AgreementDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Согласовал: " + GetUserName(Convert.ToInt32(AgreementUserID)) + " " + Convert.ToDateTime(AgreementDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Распечатал: " + GetUserName(Convert.ToInt32(PrintUserID)) + " " + Convert.ToDateTime(PrintDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            RowIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, "УТВЕРЖДАЮ_____________");
            cell.CellStyle = Calibri11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Задание №" + WorkAssignmentID.ToString());
            cell.CellStyle = CalibriBold11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, BatchName);
            cell.CellStyle = CalibriBold11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Партия №" + DyeingAssignmentID.ToString());
            //cell.CellStyle = CalibriBold11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, DyeingAssignmentID.ToString());
            //cell.CellStyle = CalibriBold11CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Тележка №" + CartNumber.ToString());
            cell.CellStyle = CalibriBold11CS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, CartNumber.ToString());
            //cell.CellStyle = CalibriBold11CS;
            if (Notes.Length > 0)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + Notes);
                cell.CellStyle = CalibriBold11CS;
            }

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Профиль");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Цвет профиля");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Цвет наполнителя");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Высота");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Ширина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 5, "Кол-во");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 6, "м.кв.");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            int CType = -1;
            int AllTotalAmount = 0;
            int TotalAmount = 0;
            decimal AllTotalSquare = 0;
            decimal TotalSquare = 0;
            int DifferentDecorCount = 0;

            using (DataView DV = new DataView(DT))
            {
                DifferentDecorCount = DV.ToTable(true, new string[] { "ColorType" }).Rows.Count;
            }

            if (DT.Rows.Count > 0)
                CType = Convert.ToInt32(DT.Rows[0]["ColorType"]);

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                if (DT.Rows[x]["PlanCount"] != DBNull.Value)
                {
                    AllTotalAmount += Convert.ToInt32(DT.Rows[x]["PlanCount"]);
                    TotalAmount += Convert.ToInt32(DT.Rows[x]["PlanCount"]);
                }
                if (DT.Rows[x]["Square"] != DBNull.Value)
                {
                    AllTotalSquare += Convert.ToDecimal(DT.Rows[x]["Square"]);
                    TotalSquare += Convert.ToDecimal(DT.Rows[x]["Square"]);
                }
                DT.Rows[x]["Square"] = Decimal.Round(Convert.ToDecimal(DT.Rows[x]["Square"]), 3, MidpointRounding.AwayFromZero);

                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                        continue;

                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableHeaderDecCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }
                }

                if (x + 1 <= DT.Rows.Count - 1)
                {
                    if (CType != Convert.ToInt32(DT.Rows[x + 1]["ColorType"]))
                    {
                        RowIndex++;
                        for (int y = 0; y < DT.Columns.Count; y++)
                        {
                            if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                                continue;

                            cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(string.Empty);
                            cell.CellStyle = TableHeaderCS;
                            continue;
                        }

                        cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                        cell.SetCellValue("Итого:");
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                        cell.SetCellValue(TotalAmount);
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                        cell.SetCellValue(Convert.ToDouble(Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero)));
                        cell.CellStyle = TableHeaderCS;

                        CType = Convert.ToInt32(DT.Rows[x + 1]["ColorType"]);
                        TotalAmount = 0;
                        TotalSquare = 0;
                        RowIndex++;
                    }
                }

                if (x == DT.Rows.Count - 1)
                {
                    if (DifferentDecorCount > 1)
                    {
                        RowIndex++;
                        for (int y = 0; y < DT.Columns.Count; y++)
                        {
                            if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                                continue;

                            cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(string.Empty);
                            cell.CellStyle = TableHeaderCS;
                            continue;
                        }

                        cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                        cell.SetCellValue("Итого:");
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                        cell.SetCellValue(TotalAmount);
                        cell.CellStyle = TableHeaderCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                        cell.SetCellValue(Convert.ToDouble(Decimal.Round(TotalSquare, 3, MidpointRounding.AwayFromZero)));
                        cell.CellStyle = TableHeaderDecCS;
                    }
                    RowIndex++;

                    for (int y = 0; y < DT.Columns.Count; y++)
                    {
                        if (DT.Columns[y].ColumnName == "ColorType" || DT.Columns[y].ColumnName == "Notes")
                            continue;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(string.Empty);
                        cell.CellStyle = TableHeaderCS;
                        continue;
                    }

                    cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                    cell.SetCellValue("Всего:");
                    cell.CellStyle = TableHeaderCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(5);
                    cell.SetCellValue(AllTotalAmount);
                    cell.CellStyle = TableHeaderCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(6);
                    cell.SetCellValue(Convert.ToDouble(Decimal.Round(AllTotalSquare, 3, MidpointRounding.AwayFromZero)));
                    cell.CellStyle = TableHeaderDecCS;
                }
                RowIndex++;
            }
            AllTotalSquare = Decimal.Round(AllTotalSquare, 3, MidpointRounding.AwayFromZero);

            cell = sheet1.CreateRow(RowIndex).CreateCell(0);
            cell.SetCellValue("Тележка №" + CartNumber);
            cell.CellStyle = CalibriBold11CS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(1);
            cell.SetCellValue(Convert.ToDouble(AllTotalSquare));
            cell.CellStyle = CalibriBold11CS;

            cell = sheet1.CreateRow(RowIndex++).CreateCell(2);
            cell.SetCellValue("м.кв.");
            cell.CellStyle = CalibriBold11CS;

            NeedPaintingMaterialManager.CalcOperationDetailConsumption(AllTotalSquare, AllTotalAmount);

            DataTable OperationsDetailDT = NeedPaintingMaterialManager.CopyOperationsDetail(TechCatalogOperationsGroupID);
            if (OperationsDetailDT == null)
                return;
            for (int i = 0; i < OperationsDetailDT.Rows.Count; i++)
            {
                RowIndex++;
                decimal Consumption = 0;
                int TechCatalogOperationsDetailID = Convert.ToInt32(OperationsDetailDT.Rows[i]["TechCatalogOperationsDetailID"]);
                string MachinesOperationName = string.Empty;
                string Measure = string.Empty;

                if (OperationsDetailDT.Rows[i]["Consumption"] != DBNull.Value)
                    Consumption = Convert.ToDecimal(OperationsDetailDT.Rows[i]["Consumption"]);
                if (OperationsDetailDT.Rows[i]["Measure"] != DBNull.Value)
                    Measure = OperationsDetailDT.Rows[i]["Measure"].ToString();
                if (OperationsDetailDT.Rows[i]["MachinesOperationName"] != DBNull.Value)
                    MachinesOperationName = OperationsDetailDT.Rows[i]["MachinesOperationName"].ToString();

                cell = sheet1.CreateRow(RowIndex).CreateCell(0);
                cell.SetCellValue(MachinesOperationName);
                cell.CellStyle = CalibriBold11CS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(1);
                cell.SetCellValue(Convert.ToDouble(Consumption) + " ч");
                cell.CellStyle = CalibriBold11CS;

                //cell = sheet1.CreateRow(RowIndex++).CreateCell(2);
                //cell.SetCellValue("ч");
                //cell.CellStyle = CalibriBold11CS;

                anchor = new HSSFClientAnchor(500, 1, 1023, 8, 4, RowIndex - 1, 6, RowIndex + 1)
                {
                    AnchorType = 2
                };
                string BarcodeNumber = ControlAssignmentsManager.GetBarcodeNumber(14, ControlAssignmentsManager.GetDyeingAssignmentBarcode(DyeingCartID, TechCatalogOperationsDetailID));
                string FileName = System.Environment.GetEnvironmentVariable("TEMP") + @"/" + BarcodeNumber + ".jpg";
                picture = patriarch.CreatePicture(anchor, LoadImage(FileName, hssfworkbook));

                cell = sheet1.CreateRow(RowIndex + 1).CreateCell(5);
                cell.SetCellValue(BarcodeNumber);
                cell.CellStyle = MaterialCS;

                DataRow NewRow = ImagesToDeleteDT.NewRow();
                NewRow["FileName"] = FileName;
                ImagesToDeleteDT.Rows.Add(NewRow);

                DataTable StoreDetailDT = NeedPaintingMaterialManager.CopyStoreDetail(TechCatalogOperationsDetailID);

                if (StoreDetailDT == null)
                    continue;
                for (int j = 0; j < StoreDetailDT.Rows.Count; j++)
                {
                    Consumption = 0;
                    Measure = string.Empty;
                    string TechStoreName = string.Empty;
                    if (StoreDetailDT.Rows[j]["Consumption"] != DBNull.Value)
                        Consumption = Convert.ToDecimal(StoreDetailDT.Rows[j]["Consumption"]);
                    if (StoreDetailDT.Rows[j]["Measure"] != DBNull.Value)
                        Measure = StoreDetailDT.Rows[j]["Measure"].ToString();
                    if (StoreDetailDT.Rows[j]["TechStoreName"] != DBNull.Value)
                        TechStoreName = StoreDetailDT.Rows[j]["TechStoreName"].ToString();

                    cell = sheet1.CreateRow(RowIndex).CreateCell(2);
                    cell.SetCellValue(TechStoreName);
                    cell.CellStyle = MaterialCS;

                    cell = sheet1.CreateRow(RowIndex++).CreateCell(3);
                    cell.SetCellValue(Convert.ToDouble(Consumption) + " " + Measure);
                    cell.CellStyle = MaterialCS;

                }
                RowIndex++;
                RowIndex++;
            }
            RowIndex++;
            RowIndex++;
            RowIndex++;
        }
    }




    public class NeedPaintingMaterial
    {
        ArrayList aFrontConfigID;
        ArrayList aPatinaID;

        DataTable OperationsGroupsDT = null;
        DataTable OperationsDetailDT = null;
        DataTable StoreDT = null;
        DataTable StoreDetailDT = null;
        DataTable SummaryStoreDetailDT = null;
        DataTable TermsDT = null;

        BindingSource OperationsGroupsBS = null;
        BindingSource OperationsDetailBS = null;
        BindingSource StoreBS = null;
        BindingSource StoreDetailBS = null;
        BindingSource SummaryStoreDetailBS = null;

        public ArrayList FrontConfigID
        {
            set { aFrontConfigID = value; }
            get { return aFrontConfigID; }
        }

        public ArrayList PatinaID
        {
            set { aPatinaID = value; }
            get { return aPatinaID; }
        }

        public BindingSource OperationsGroupsList
        {
            get { return OperationsGroupsBS; }
        }

        public BindingSource OperationsDetailList
        {
            get { return OperationsDetailBS; }
        }

        public BindingSource StoreList
        {
            get { return StoreBS; }
        }

        public BindingSource StoreDetailList
        {
            get { return StoreDetailBS; }
        }

        public BindingSource SummaryStoreDetailList
        {
            get { return SummaryStoreDetailBS; }
        }

        public NeedPaintingMaterial()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        private void Create()
        {
            OperationsGroupsDT = new DataTable();
            OperationsDetailDT = new DataTable();
            StoreDT = new DataTable();
            StoreDetailDT = new DataTable();
            SummaryStoreDetailDT = new DataTable();
            SummaryStoreDetailDT.Columns.Add(new DataColumn("TechCatalogOperationsGroupID", Type.GetType("System.Int32")));
            SummaryStoreDetailDT.Columns.Add(new DataColumn("TechStoreID", Type.GetType("System.Int32")));
            SummaryStoreDetailDT.Columns.Add(new DataColumn("TechStoreName", Type.GetType("System.String")));
            SummaryStoreDetailDT.Columns.Add(new DataColumn("Consumption", Type.GetType("System.Decimal")));
            SummaryStoreDetailDT.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            TermsDT = new DataTable();

            OperationsGroupsBS = new BindingSource();
            OperationsDetailBS = new BindingSource();
            StoreBS = new BindingSource();
            StoreDetailBS = new BindingSource();
            SummaryStoreDetailBS = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM TechCatalogOperationsGroups";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(OperationsGroupsDT);
            }
            SelectCommand = @"SELECT TOP 0 TechCatalogOperationsDetail.TechCatalogOperationsDetailID,  MachinesOperations.MeasureID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, MachinesOperations.MachinesOperationName, MachinesOperations.Norm, Measures.Measure FROM TechCatalogOperationsDetail 
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID=MachinesOperations.MachinesOperationID
                INNER JOIN Measures ON MachinesOperations.MeasureID=Measures.MeasureID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(OperationsDetailDT);
                if (!OperationsDetailDT.Columns.Contains("Consumption"))
                    OperationsDetailDT.Columns.Add(new DataColumn("Consumption", Type.GetType("System.Decimal")));
            }

            SelectCommand = @"SELECT TOP 0 TechCatalogStoreDetail.TechCatalogStoreDetailID, TechCatalogStoreDetail.TechCatalogOperationsDetailID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, TechCatalogStoreDetail.TechStoreID, TechStore.TechStoreName,  TechStore.MeasureID, 
                TechCatalogStoreDetail.Count, Measures.Measure FROM TechCatalogStoreDetail 
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID=TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID=Measures.MeasureID
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID=TechStoreSubGroups.TechStoreSubGroupID AND TechStoreSubGroups.TechStoreGroupID=8
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogStoreDetail.TechCatalogOperationsDetailID=TechCatalogOperationsDetail.TechCatalogOperationsDetailID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(StoreDetailDT);
                if (!StoreDetailDT.Columns.Contains("Consumption"))
                    StoreDetailDT.Columns.Add(new DataColumn("Consumption", Type.GetType("System.Decimal")));
            }
            SelectCommand = @"SELECT TOP 0 Store.StoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, 
                    Store.CurrentCount, Measures.Measure, Store.Produced, Store.BestBefore FROM Store
                    INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID 
                    INNER JOIN infiniu2_catalog.dbo.Measures ON infiniu2_catalog.dbo.TechStore.MeasureID=infiniu2_catalog.dbo.Measures.MeasureID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(StoreDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TermsDT);
            }
        }

        public void UpdateTables()
        {
            if (aFrontConfigID.Count == 0)
                return;
            int[] iFrontConfigID = aFrontConfigID.OfType<int>().ToArray();
            string SelectCommand = @"SELECT * FROM TechCatalogOperationsGroups WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE FrontConfigID IN (" + string.Join(",", iFrontConfigID) + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsGroupsDT.Clear();
                DA.Fill(OperationsGroupsDT);
            }
            SelectCommand = @"SELECT TechCatalogOperationsDetail.TechCatalogOperationsDetailID, MachinesOperations.MeasureID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, MachinesOperations.MachinesOperationName, MachinesOperations.Norm, Measures.Measure FROM TechCatalogOperationsDetail 
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID=MachinesOperations.MachinesOperationID
                INNER JOIN Measures ON MachinesOperations.MeasureID=Measures.MeasureID
                WHERE TechCatalogOperationsGroupID IN (SELECT TechCatalogOperationsGroupID FROM TechCatalogOperationsGroups
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE FrontConfigID IN (" + string.Join(",", iFrontConfigID) + "))) ORDER BY SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsDetailDT.Clear();
                DA.Fill(OperationsDetailDT);
            }

            SelectCommand = @"SELECT TechCatalogStoreDetail.TechCatalogStoreDetailID, TechCatalogStoreDetail.TechCatalogOperationsDetailID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, TechCatalogStoreDetail.TechStoreID, TechStore.TechStoreName,  TechStore.MeasureID, 
                TechCatalogStoreDetail.Count, Measures.Measure FROM TechCatalogStoreDetail 
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID=TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID=Measures.MeasureID
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID=TechStoreSubGroups.TechStoreSubGroupID AND TechStoreSubGroups.TechStoreGroupID=8
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogStoreDetail.TechCatalogOperationsDetailID=TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                AND TechCatalogOperationsGroupID IN (SELECT TechCatalogOperationsGroupID FROM TechCatalogOperationsGroups
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE FrontConfigID IN (" + string.Join(",", iFrontConfigID) + ")))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                StoreDetailDT.Clear();
                DA.Fill(StoreDetailDT);
            }
            SelectCommand = @"SELECT TOP 0 Store.StoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, 
                    Store.CurrentCount, Measures.Measure, Store.Produced, Store.BestBefore FROM Store
                    INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID 
                    INNER JOIN infiniu2_catalog.dbo.Measures ON infiniu2_catalog.dbo.TechStore.MeasureID=infiniu2_catalog.dbo.Measures.MeasureID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(StoreDT);
            }
            SelectCommand = @"SELECT * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TermsDT.Clear();
                DA.Fill(TermsDT);
            }
        }

        public void UpdateTables(int DyeingAssignmentID)
        {
            string SelectCommand = @"SELECT * FROM TechCatalogOperationsGroups WHERE TechCatalogOperationsGroupID IN (SELECT TechCatalogOperationsGroupID FROM infiniu2_storage.dbo.DyeingAssignmentBarcodes WHERE DyeingAssignmentID = " + DyeingAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsGroupsDT.Clear();
                DA.Fill(OperationsGroupsDT);
            }
            SelectCommand = @"SELECT TechCatalogOperationsDetail.TechCatalogOperationsDetailID, MachinesOperations.MeasureID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, MachinesOperations.MachinesOperationName, MachinesOperations.Norm, Measures.Measure FROM TechCatalogOperationsDetail 
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID=MachinesOperations.MachinesOperationID
                INNER JOIN Measures ON MachinesOperations.MeasureID=Measures.MeasureID
                WHERE TechCatalogOperationsDetailID IN (SELECT TechCatalogOperationsDetailID FROM infiniu2_storage.dbo.DyeingAssignmentBarcodes WHERE DyeingAssignmentID = " + DyeingAssignmentID + ") ORDER BY SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsDetailDT.Clear();
                DA.Fill(OperationsDetailDT);
                if (!OperationsDetailDT.Columns.Contains("Consumption"))
                    OperationsDetailDT.Columns.Add(new DataColumn("Consumption", Type.GetType("System.Decimal")));
            }

            SelectCommand = @"SELECT TechCatalogStoreDetail.TechCatalogStoreDetailID, TechCatalogStoreDetail.TechCatalogOperationsDetailID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, TechCatalogStoreDetail.TechStoreID, TechStore.TechStoreName, TechStore.MeasureID, 
                TechCatalogStoreDetail.Count, Measures.Measure FROM TechCatalogStoreDetail 
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID=TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID=Measures.MeasureID
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID=TechStoreSubGroups.TechStoreSubGroupID AND TechStoreSubGroups.TechStoreGroupID=8
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogStoreDetail.TechCatalogOperationsDetailID=TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                AND TechCatalogStoreDetail.TechCatalogOperationsDetailID IN (SELECT TechCatalogOperationsDetailID FROM infiniu2_storage.dbo.DyeingAssignmentBarcodes WHERE DyeingAssignmentID = " + DyeingAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                StoreDetailDT.Clear();
                DA.Fill(StoreDetailDT);
                if (!StoreDetailDT.Columns.Contains("Consumption"))
                    StoreDetailDT.Columns.Add(new DataColumn("Consumption", Type.GetType("System.Decimal")));
            }
            SelectCommand = @"SELECT TOP 0 Store.StoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, 
                    Store.CurrentCount, Measures.Measure, Store.Produced, Store.BestBefore FROM Store
                    INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID 
                    INNER JOIN infiniu2_catalog.dbo.Measures ON infiniu2_catalog.dbo.TechStore.MeasureID=infiniu2_catalog.dbo.Measures.MeasureID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                StoreDT.Clear();
                DA.Fill(StoreDT);
            }
            SelectCommand = @"SELECT * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TermsDT.Clear();
                DA.Fill(TermsDT);
            }
        }

        public void FindTerms()
        {
            for (int i = OperationsDetailDT.Rows.Count - 1; i >= 0; i--)
            {
                bool OkTerm = false;
                int TechCatalogOperationsDetailID = Convert.ToInt32(OperationsDetailDT.Rows[i]["TechCatalogOperationsDetailID"]);
                DataRow[] rows = TermsDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
                if (rows.Count() > 0)
                {
                    foreach (DataRow item in rows)
                    {
                        string MathSymbol = item["MathSymbol"].ToString();
                        string Parameter = item["Parameter"].ToString();
                        decimal Term = 0;
                        if (item["Term"] == DBNull.Value)
                            continue;
                        Term = Convert.ToDecimal(item["Term"]);
                        if (Parameter == "PatinaID")
                        {
                            foreach (int item1 in PatinaID)
                            {
                                if (Term == item1)
                                {
                                    OkTerm = true;
                                }
                            }
                        }
                    }
                    if (!OkTerm)
                        OperationsDetailDT.Rows[i].Delete();
                }
            }
            OperationsDetailDT.AcceptChanges();
        }

        public void CalcSumStoreDetailConsumption()
        {
            SummaryStoreDetailDT.Clear();
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(StoreDetailDT))
            {
                Table = DV.ToTable(true, new string[] { "TechCatalogOperationsGroupID", "TechStoreID", "TechStoreName", "Measure" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                int TechStoreID = Convert.ToInt32(Table.Rows[i]["TechStoreID"]);
                int TechCatalogOperationsGroupID = Convert.ToInt32(Table.Rows[i]["TechCatalogOperationsGroupID"]);
                string Measure = Table.Rows[i]["Measure"].ToString();
                string TechStoreName = Table.Rows[i]["TechStoreName"].ToString();
                DataRow[] rows = StoreDetailDT.Select("TechStoreID=" + TechStoreID + " AND TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID);
                if (rows.Count() == 0)
                    continue;
                decimal Consumption = 0;
                foreach (DataRow item in rows)
                {
                    if (item["Consumption"] == DBNull.Value)
                        continue;
                    Consumption += Convert.ToDecimal(item["Consumption"]);
                }
                DataRow[] rows1 = SummaryStoreDetailDT.Select("TechStoreID=" + TechStoreID + " AND TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID);
                if (rows1.Count() == 0)
                {
                    DataRow NewRow = SummaryStoreDetailDT.NewRow();
                    NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                    NewRow["TechStoreID"] = TechStoreID;
                    NewRow["TechStoreName"] = TechStoreName;
                    NewRow["Measure"] = Measure;
                    NewRow["Consumption"] = Consumption;
                    SummaryStoreDetailDT.Rows.Add(NewRow);
                }
                else
                    rows1[0]["Consumption"] = Convert.ToDecimal(rows1[0]["Consumption"]) + Consumption;
            }
            SummaryStoreDetailDT.DefaultView.Sort = "TechStoreName";
        }

        public void GetStore(int TechCatalogOperationsGroupID)
        {
            StoreDT.Clear();
            DataRow[] rows = SummaryStoreDetailDT.Select("TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID);
            if (rows.Count() == 0)
                return;
            foreach (DataRow item in rows)
            {
                int TechStoreID = Convert.ToInt32(item["TechStoreID"]);
                string SelectCommand = @"SELECT Store.StoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, 
                    Store.CurrentCount, Measures.Measure, Store.Produced, Store.BestBefore FROM Store
                    INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                    INNER JOIN infiniu2_catalog.dbo.Measures ON infiniu2_catalog.dbo.TechStore.MeasureID=infiniu2_catalog.dbo.Measures.MeasureID
                    WHERE Store.CurrentCount > 0 AND Store.StoreItemID = " + TechStoreID + " ORDER BY TechStoreName";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int j = 0; j < DT.Rows.Count; j++)
                        {
                            DataRow NewRow = StoreDT.NewRow();
                            NewRow["StoreID"] = DT.Rows[j]["StoreID"];
                            NewRow["TechStoreName"] = DT.Rows[j]["TechStoreName"];
                            NewRow["CurrentCount"] = DT.Rows[j]["CurrentCount"];
                            NewRow["Measure"] = DT.Rows[j]["Measure"];
                            NewRow["Produced"] = DT.Rows[j]["Produced"];
                            NewRow["BestBefore"] = DT.Rows[j]["BestBefore"];
                            StoreDT.Rows.Add(NewRow);
                        }
                    }
                }
            }

            StoreDT.DefaultView.Sort = "TechStoreName";
            StoreBS.MoveFirst();
        }

        private void Binding()
        {
            OperationsGroupsBS.DataSource = OperationsGroupsDT;
            OperationsDetailBS.DataSource = OperationsDetailDT;
            StoreBS.DataSource = StoreDT;
            StoreDetailBS.DataSource = StoreDetailDT;
            SummaryStoreDetailBS.DataSource = SummaryStoreDetailDT;
        }

        public DataGridViewComboBoxColumn TPS45UserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TPS45UserColumn",
                    HeaderText = "   Угол 45\r\nраспечатал",
                    DataPropertyName = "TPS45UserID",
                    //Column.DataSource = new DataView(UsersDT);
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataTable CopyOperationsDetail(int TechCatalogOperationsGroupID)
        {
            DataTable table = OperationsDetailDT.Clone();
            DataRow[] rows = OperationsDetailDT.Select("TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID);
            if (rows.Count() == 0)
                return null;
            foreach (DataRow item in rows)
            {
                DataRow NewRow = table.NewRow();
                NewRow.ItemArray = item.ItemArray;
                table.Rows.Add(NewRow);
            }
            return table;
        }

        public DataTable CopyStoreDetail(int TechCatalogOperationsDetailID)
        {
            DataTable table = StoreDetailDT.Clone();
            DataRow[] rows = StoreDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            if (rows.Count() == 0)
                return null;
            foreach (DataRow item in rows)
            {
                DataRow NewRow = table.NewRow();
                NewRow.ItemArray = item.ItemArray;
                table.Rows.Add(NewRow);
            }
            return table;
        }

        public void FilterOperationsDetail(int TechCatalogOperationsGroupID)
        {
            OperationsDetailBS.Filter = "TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID;
            OperationsDetailBS.MoveFirst();
        }

        public void FilterStoreDetail(int TechCatalogOperationsDetailID)
        {
            StoreDetailBS.Filter = "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID;
            StoreDetailBS.MoveFirst();
        }

        public void FilterSummaryStoreDetail(int TechCatalogOperationsGroupID)
        {
            SummaryStoreDetailBS.Filter = "TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID;
            SummaryStoreDetailBS.MoveFirst();
        }

        public void CalcOperationDetailConsumption(decimal Square, int Count)
        {
            for (int i = 0; i < OperationsDetailDT.Rows.Count; i++)
            {
                int MeasureID = 3;
                int TechCatalogOperationsDetailID = Convert.ToInt32(OperationsDetailDT.Rows[i]["TechCatalogOperationsDetailID"]);
                if (OperationsDetailDT.Rows[i]["MeasureID"] != DBNull.Value)
                    MeasureID = Convert.ToInt32(OperationsDetailDT.Rows[i]["MeasureID"]);
                if (OperationsDetailDT.Rows[i]["Norm"] == DBNull.Value)
                    continue;
                if (MeasureID == 1)
                    OperationsDetailDT.Rows[i]["Consumption"] = Decimal.Round(Square / Convert.ToDecimal(OperationsDetailDT.Rows[i]["Norm"]), 3, MidpointRounding.AwayFromZero);
                else
                    OperationsDetailDT.Rows[i]["Consumption"] = Decimal.Round(Count / Convert.ToDecimal(OperationsDetailDT.Rows[i]["Norm"]), 3, MidpointRounding.AwayFromZero);
                CalcStoreDetailConsumption(TechCatalogOperationsDetailID, MeasureID, Square, Count);
            }
        }

        private void CalcStoreDetailConsumption(int TechCatalogOperationsDetailID, int MeasureID, decimal Square, int Count)
        {
            DataRow[] rows = StoreDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            for (int i = 0; i < rows.Count(); i++)
            {
                //if (rows[i]["MeasureID"] != DBNull.Value)
                //    MeasureID = Convert.ToInt32(StoreDetailDT.Rows[i]["MeasureID"]);
                if (rows[i]["Count"] == DBNull.Value)
                    continue;
                if (MeasureID == 1)
                    rows[i]["Consumption"] = Decimal.Round(Convert.ToDecimal(rows[i]["Count"]) * Square, 3, MidpointRounding.AwayFromZero);
                else
                    rows[i]["Consumption"] = Decimal.Round(Convert.ToDecimal(rows[i]["Count"]) * Count, 3, MidpointRounding.AwayFromZero);
            }
        }

        public void MoveToOperationsGroupPos(int Pos)
        {
            OperationsGroupsBS.Position = Pos;
        }

    }

    public class Barcode
    {
        BarcodeLib.Barcode Barcod;

        SolidBrush FontBrush;

        public enum BarcodeLength { Short, Medium, Long };

        public Barcode()
        {
            Barcod = new BarcodeLib.Barcode();

            FontBrush = new System.Drawing.SolidBrush(Color.Black);
        }

        public void DrawBarcodeText(BarcodeLength BarcodeLength, Graphics Graphics, string Text, int X, int Y)
        {
            int CharOffset = 0;
            int CharWidth = 0;
            float FontSize = 0;

            if (BarcodeLength == Barcode.BarcodeLength.Short)
            {
                CharWidth = 8;
                CharOffset = 7;
                FontSize = 8.0f;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Medium)
            {
                CharWidth = 18;
                CharOffset = 5;
                FontSize = 12.0f;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Long)
            {
                CharWidth = 26;
                CharOffset = 5;
                FontSize = 14.0f;
            }

            Font F = new Font("Arial", FontSize, FontStyle.Bold);

            for (int i = 0; i < Text.Length; i++)
            {
                Graphics.DrawString(Text[i].ToString(), F, FontBrush, i * CharWidth + CharOffset + X, Y + 2);
            }

            F.Dispose();
        }

        public Image GetBarcode(BarcodeLength BarcodeLength, int BarcodeHeight, string Text)
        {
            //set length and height
            if (BarcodeLength == Barcode.BarcodeLength.Short)
            {
                Barcod.Width = 101 + 12;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Medium)
            {
                Barcod.Width = 202 + 12;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Long)
            {
                Barcod.Width = 303 + 12;
            }

            Barcod.Height = BarcodeHeight;


            //create area
            Bitmap B = new Bitmap(Barcod.Width, BarcodeHeight);
            Graphics G = Graphics.FromImage(B);
            G.Clear(Color.White);


            //create barcode
            Image Bar = Barcod.Encode(BarcodeLib.TYPE.CODE128C, Text);
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            G.DrawImage(Bar, 0, 2);


            //create text
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;


            Bar.Dispose();
            G.Dispose();

            GC.Collect();

            return B;
        }
    }





    public class ScaningDyeingAssignments
    {
        bool bNewAssignment = false;

        decimal iCartSquare = 0;
        decimal iNorm = 0;
        int iGroupType = 0;
        int iOperationStatus = 0;
        int iCartNumber = 0;
        int iDyeingAssignmentID = 0;
        int iDyeingCartID = 0;
        int iDyeingAssignmentBarcodeID = 0;
        int iTechCatalogOperationsGroupID = 0;
        int iTechCatalogOperationsDetailID = 0;
        int iWorkersInAssignment = 0;
        object dStartDateTime = DBNull.Value;
        object dFinishDateTime = DBNull.Value;
        string sBatchName = string.Empty;
        string sDocNumber = string.Empty;
        string sOperationName = string.Empty;

        DataTable DyeingAssignmentsDT = null;
        DataTable DyeingCartsDT = null;
        DataTable DyeingAssignmentBarcodesDT = null;
        DataTable DyeingAssignmentBarcodeDetailsDT = null;
        DataTable TechCatalogOperationsDetailDT = null;
        DataTable UsersDT = null;

        public bool NewAssignment
        {
            get { return bNewAssignment; }
            set { bNewAssignment = value; }
        }

        public decimal CartSquare
        {
            get { return iCartSquare; }
            set { iCartSquare = value; }
        }

        public decimal Norm
        {
            get
            {
                if (iNorm == 0)
                    return 0;
                if (iWorkersInAssignment == 0)
                    return 0;
                return Decimal.Round(iCartSquare / (iNorm * iWorkersInAssignment), 2, MidpointRounding.AwayFromZero);
            }
        }

        public int GroupType
        {
            get { return iGroupType; }
            set { iGroupType = value; }
        }

        public int OperationStatus
        {
            get { return iOperationStatus; }
            set { iOperationStatus = value; }
        }

        public int CartNumber
        {
            get { return iCartNumber; }
            set { iCartNumber = value; }
        }

        public int DyeingAssignmentID
        {
            get { return iDyeingAssignmentID; }
            set { iDyeingAssignmentID = value; }
        }

        public int DyeingCartID
        {
            get { return iDyeingCartID; }
            set { iDyeingCartID = value; }
        }

        public int DyeingAssignmentBarcodeID
        {
            get { return iDyeingAssignmentBarcodeID; }
            set { iDyeingAssignmentBarcodeID = value; }
        }

        public int TechCatalogOperationsGroupID
        {
            get { return iTechCatalogOperationsGroupID; }
            set { iTechCatalogOperationsGroupID = value; }
        }

        public int TechCatalogOperationsDetailID
        {
            get { return iTechCatalogOperationsDetailID; }
            set { iTechCatalogOperationsDetailID = value; }
        }

        public object StartDateTime
        {
            get { return dStartDateTime; }
            set { dStartDateTime = value; }
        }

        public object FinishDateTime
        {
            get { return dFinishDateTime; }
            set { dFinishDateTime = value; }
        }

        public string BatchName
        {
            get
            {
                return sBatchName;
            }
        }

        public string DocNumber
        {
            get { return sDocNumber; }
            set { sDocNumber = value; }
        }

        public string OperationName
        {
            get { return sOperationName; }
            set { sOperationName = value; }
        }

        public decimal OperationTime
        {
            get
            {
                if (iNorm == 0)
                    return 0;
                decimal Norm = Decimal.Round(iCartSquare / (iNorm * iWorkersInAssignment), 3, MidpointRounding.AwayFromZero);
                TimeSpan time = Convert.ToDateTime(dFinishDateTime) - Convert.ToDateTime(dStartDateTime);
                if (time.TotalHours == 0)
                    return 0;
                if (iWorkersInAssignment == 0)
                    return 0;
                decimal PlannedTime = Decimal.Round(Norm / Convert.ToDecimal(time.TotalHours), 3, MidpointRounding.AwayFromZero);
                return PlannedTime;
            }
        }

        public ScaningDyeingAssignments()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
        }

        private void Create()
        {
            DyeingAssignmentsDT = new DataTable();
            DyeingCartsDT = new DataTable();
            DyeingAssignmentBarcodesDT = new DataTable();
            DyeingAssignmentBarcodeDetailsDT = new DataTable();
            TechCatalogOperationsDetailDT = new DataTable();
            UsersDT = new DataTable();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignments";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentsDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingCarts";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingCartsDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignmentBarcodes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentBarcodesDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignmentBarcodeDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentBarcodeDetailsDT);
            }
            SelectCommand = @"SELECT TOP 0 TechCatalogOperationsDetail.TechCatalogOperationsDetailID, MachinesOperations.MeasureID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, MachinesOperations.MachinesOperationName, MachinesOperations.Norm, Measures.Measure FROM TechCatalogOperationsDetail 
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID=MachinesOperations.MachinesOperationID
                INNER JOIN Measures ON MachinesOperations.MeasureID=Measures.MeasureID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogOperationsDetailDT);
            }
            SelectCommand = @"SELECT UserID, Name, ShortName FROM Users";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
        }

        public void ClearTables()
        {
            iCartSquare = 0;
            iNorm = 0;
            iOperationStatus = 0;
            iCartNumber = 0;
            iDyeingAssignmentID = 0;
            iDyeingCartID = 0;
            iDyeingAssignmentBarcodeID = 0;
            iTechCatalogOperationsGroupID = 0;
            iTechCatalogOperationsDetailID = 0;
            iWorkersInAssignment = 0;
            dStartDateTime = DBNull.Value;
            dFinishDateTime = DBNull.Value;
            sOperationName = string.Empty;

            DyeingAssignmentsDT.Clear();
            DyeingCartsDT.Clear();
            DyeingAssignmentBarcodesDT.Clear();
            TechCatalogOperationsDetailDT.Clear();
        }

        public void ClearBarcodeDetails()
        {
            DyeingAssignmentBarcodeDetailsDT.Clear();
        }

        public bool HasUserDyeingAssignmentUsers
        {
            get { return DyeingAssignmentBarcodeDetailsDT.Rows.Count > 0; }
        }

        public void CreateDyeingAssignmentBarcodeDetail(int UserID)
        {
            DataRow NewRow = DyeingAssignmentBarcodeDetailsDT.NewRow();
            NewRow["DyeingAssignmentUserID"] = UserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            DyeingAssignmentBarcodeDetailsDT.Rows.Add(NewRow);
        }

        public void AddBarcodeID(int DyeingAssignmentBarcodeID)
        {
            for (int i = 0; i < DyeingAssignmentBarcodeDetailsDT.Rows.Count; i++)
            {
                DyeingAssignmentBarcodeDetailsDT.Rows[i]["DyeingAssignmentBarcodeID"] = iDyeingAssignmentBarcodeID;
            }
        }

        public void StartOperation(int DyeingAssignmentBarcodeID)
        {
            DataRow[] rows = DyeingAssignmentBarcodesDT.Select("DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID);
            if (rows.Count() == 0)
                return;
            //rows[0]["StartUserID"] = UserID;
            rows[0]["StartDateTime"] = Security.GetCurrentDate();
        }

        public void FinishOperation(int DyeingAssignmentBarcodeID)
        {
            DataRow[] rows = DyeingAssignmentBarcodesDT.Select("DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID);
            if (rows.Count() == 0)
                return;
            //rows[0]["FinishUserID"] = UserID;
            rows[0]["FinishDateTime"] = Security.GetCurrentDate();
        }

        public void SetStatus(int DyeingAssignmentBarcodeID, int Status)
        {
            DataRow[] rows = DyeingAssignmentBarcodesDT.Select("DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID);
            if (rows.Count() == 0)
                return;
            //rows[0]["FinishUserID"] = UserID;
            rows[0]["Status"] = Status;
        }

        public void SaveDyeingAssignmentBarcodes()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DyeingAssignmentBarcodes",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DyeingAssignmentBarcodesDT);
                }
            }
        }

        public void SaveDyeingAssignmentBarcodeDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DyeingAssignmentBarcodeDetails",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DyeingAssignmentBarcodeDetailsDT);
                }
            }
        }

        public void GetDyeingAssignmentInfo(int DyeingAssignmentBarcodeID)
        {
            iDyeingAssignmentID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["DyeingAssignmentID"]);
            iDyeingCartID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["DyeingCartID"]);
            iDyeingAssignmentBarcodeID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["DyeingAssignmentBarcodeID"]);
            iTechCatalogOperationsGroupID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["TechCatalogOperationsGroupID"]);
            iTechCatalogOperationsDetailID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["TechCatalogOperationsDetailID"]);
            iOperationStatus = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["Status"]);
            //iOperationStatus = 0;операция не начата
            //iOperationStatus = 1;начата
            //iOperationStatus = 2;завершена
            dStartDateTime = DyeingAssignmentBarcodesDT.Rows[0]["StartDateTime"];
            dFinishDateTime = DyeingAssignmentBarcodesDT.Rows[0]["FinishDateTime"];

            UpdateDyeingAssignments(iDyeingAssignmentID);
            UpdateDyeingCarts(iDyeingCartID);
            UpdateTechCatalogOperationsDetail(iTechCatalogOperationsDetailID);

            sOperationName = GetOperationName(iTechCatalogOperationsDetailID);
            iCartSquare = GetCartSquare(iDyeingCartID);
            sBatchName = GetBatchName(iDyeingAssignmentID);
            if (iGroupType == 0)
                sDocNumber = FindDocNumber(iDyeingAssignmentID);
            else
                sDocNumber = string.Empty;
        }

        private void UpdateDyeingAssignments(int DyeingAssignmentID)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignments WHERE DyeingAssignmentID=" + DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentsDT.Clear();
                DA.Fill(DyeingAssignmentsDT);
                if (DyeingAssignmentsDT.Rows.Count > 0)
                {
                    iGroupType = Convert.ToInt32(DyeingAssignmentsDT.Rows[0]["GroupType"]);
                }
            }
        }

        private void UpdateDyeingCarts(int DyeingCartID)
        {
            string SelectCommand = @"SELECT * FROM DyeingCarts WHERE DyeingCartID=" + DyeingCartID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingCartsDT.Clear();
                DA.Fill(DyeingCartsDT);
                if (DyeingCartsDT.Rows.Count > 0)
                {
                    iCartNumber = Convert.ToInt32(DyeingCartsDT.Rows[0]["CartNumber"]);
                }
            }
        }

        //public bool GetLastDyeingAssignmentBarcode()
        //{
        //    string SelectCommand = @"SELECT TOP 1 * FROM DyeingAssignmentBarcodes WHERE DyeingAssignmentBarcodeID ORDER BY DyeingAssignmentBarcodeID DESC";
        //    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
        //    {
        //        DyeingAssignmentBarcodesDT.Clear();
        //        DA.Fill(DyeingAssignmentBarcodesDT);
        //        if (DyeingAssignmentBarcodesDT.Rows.Count > 0)
        //        {
        //            iDyeingAssignmentID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["DyeingAssignmentID"]);
        //            iDyeingCartID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["DyeingCartID"]);
        //            iDyeingAssignmentBarcodeID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["DyeingAssignmentBarcodeID"]);
        //            iTechCatalogOperationsGroupID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["TechCatalogOperationsGroupID"]);
        //            iTechCatalogOperationsDetailID = Convert.ToInt32(DyeingAssignmentBarcodesDT.Rows[0]["TechCatalogOperationsDetailID"]);
        //            dStartDateTime = DyeingAssignmentBarcodesDT.Rows[0]["StartDateTime"];
        //            dFinishDateTime = DyeingAssignmentBarcodesDT.Rows[0]["FinishDateTime"];
        //            if (dStartDateTime == DBNull.Value && dFinishDateTime == DBNull.Value)
        //                iOperationStatus = 0;//операция не начата
        //            if (dStartDateTime != DBNull.Value && dFinishDateTime == DBNull.Value)
        //                iOperationStatus = 1;//начата
        //            if (dStartDateTime != DBNull.Value && dFinishDateTime != DBNull.Value)
        //                iOperationStatus = 2;//завершена
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //}

        public bool UpdateDyeingAssignmentBarcodes(int DyeingAssignmentBarcodeID)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentBarcodes WHERE DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentBarcodesDT.Clear();
                DA.Fill(DyeingAssignmentBarcodesDT);
                if (DyeingAssignmentBarcodesDT.Rows.Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public void UpdateDyeingAssignmentBarcodeDetails(int DyeingAssignmentBarcodeID)
        {
            string SelectCommand = @"SELECT * FROM DyeingAssignmentBarcodeDetails WHERE DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentBarcodeDetailsDT.Clear();
                DA.Fill(DyeingAssignmentBarcodeDetailsDT);
            }
        }

        private void UpdateTechCatalogOperationsDetail(int TechCatalogOperationsDetailID)
        {
            string SelectCommand = @"SELECT TechCatalogOperationsDetail.TechCatalogOperationsDetailID, MachinesOperations.MeasureID, 
                TechCatalogOperationsDetail.TechCatalogOperationsGroupID, MachinesOperations.MachinesOperationName, MachinesOperations.Norm, Measures.Measure FROM TechCatalogOperationsDetail 
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID=MachinesOperations.MachinesOperationID
                INNER JOIN Measures ON MachinesOperations.MeasureID=Measures.MeasureID
                WHERE TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogOperationsDetailDT.Clear();
                DA.Fill(TechCatalogOperationsDetailDT);
                if (TechCatalogOperationsDetailDT.Rows.Count > 0)
                {
                    iNorm = Convert.ToInt32(TechCatalogOperationsDetailDT.Rows[0]["Norm"]);
                }
            }
        }

        public string GetBatchName(int DyeingAssignmentID)
        {
            string name = string.Empty;
            string SelectCommand = @"SELECT DISTINCT GroupType, WorkAssignmentID, MegaBatchID, BatchID FROM DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        int WorkAssignmentID = Convert.ToInt32(DT.Rows[0]["WorkAssignmentID"]);
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (Convert.ToInt32(DT.Rows[i]["GroupType"]) == 0)
                                name += "З(" + DT.Rows[i]["MegaBatchID"].ToString() + "," + DT.Rows[i]["BatchID"].ToString() + ")+";
                            if (Convert.ToInt32(DT.Rows[i]["GroupType"]) == 1)
                                name += "М(" + DT.Rows[i]["MegaBatchID"].ToString() + "," + DT.Rows[i]["BatchID"].ToString() + ")+";
                        }
                        if (name.Length > 0)
                            name = name.Substring(0, name.Length - 1);
                    }
                }
            }
            return name;
        }

        public string FindDocNumber(int DyeingAssignmentID)
        {
            string name = string.Empty;
            string SelectCommand = @"SELECT DocNumber FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID FROM infiniu2_storage.dbo.DyeingAssignmentDetails WHERE DyeingAssignmentID=" + DyeingAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            name += DT.Rows[i]["DocNumber"].ToString() + ", ";
                        }
                        if (name.Length > 0)
                            name = name.Substring(0, name.Length - 2);
                    }
                }
            }
            return name;
        }

        public string GetOperationName(int TechCatalogOperationsDetailID)
        {
            DataRow[] rows = TechCatalogOperationsDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            if (rows.Count() == 0 || rows[0]["MachinesOperationName"] == DBNull.Value)
                return "неизвестная_операция";
            return rows[0]["MachinesOperationName"].ToString();
        }

        public decimal GetCartSquare(int DyeingCartID)
        {
            DataRow[] rows = DyeingCartsDT.Select("DyeingCartID=" + DyeingCartID);
            if (rows.Count() == 0 || rows[0]["Square"] == DBNull.Value)
                return 0;
            return Convert.ToDecimal(rows[0]["Square"]);
        }

        public string GetUserName(int UserID)
        {
            DataRow[] rows = UsersDT.Select("UserID=" + UserID);
            if (rows.Count() == 0 || rows[0]["ShortName"] == DBNull.Value)
                return "неизвестное_имя";
            return rows[0]["ShortName"].ToString();
        }

        public string DyeingAssignmentUsers()
        {
            string name = string.Empty;
            for (int i = 0; i < DyeingAssignmentBarcodeDetailsDT.Rows.Count; i++)
            {
                int UserID = 0;
                if (DyeingAssignmentBarcodeDetailsDT.Rows[i]["DyeingAssignmentUserID"] != DBNull.Value)
                {
                    UserID = Convert.ToInt32(DyeingAssignmentBarcodeDetailsDT.Rows[i]["DyeingAssignmentUserID"]);
                    name += GetUserName(UserID) + ", ";
                    iWorkersInAssignment++;
                }
            }
            if (name.Length > 0)
                name = name.Substring(0, name.Length - 2);
            //else
            //    name = "-";
            return name;
        }
    }

    public class RegistrationDyeingWorkMan
    {
        decimal AverageMonthlyCoef = 159;
        decimal MonthlyCoef = 169.3m;

        DataTable DyeingAssignmentBarcodesDT = null;
        DataTable DyeingAssignmentBarcodeDetailsDT = null;
        DataTable MonthsDT = null;
        DataTable OperationsDT = null;
        DataTable PositionsDT = null;
        DataTable ReferenceDT = null;
        DataTable UsersDT = null;
        DataTable UsersTimeWorkDT = null;
        DataTable WorkDaysDT = null;
        DataTable YearsDT = null;

        BindingSource MonthsBS = null;
        BindingSource UsersBS = null;
        BindingSource UsersTimeWorkBS = null;
        BindingSource YearsBS = null;

        List<WorkerMonthStatistics> MonthStatistics;
        WorkerMonthStatistics TotalMonthStatistics;
        List<MonthWorkStatement> MonthStatement;
        MonthWorkStatement TotalMonthStatement;

        HSSFWorkbook hssfworkbook;

        public BindingSource MonthsList
        {
            get { return MonthsBS; }
        }

        public BindingSource UsersList
        {
            get { return UsersBS; }
        }

        public BindingSource UsersTimeWorkList
        {
            get { return UsersTimeWorkBS; }
        }

        public BindingSource YearsList
        {
            get { return YearsBS; }
        }

        public struct FotoWorkDay
        {
            public DateTime Day;
            public decimal Volume;
            public decimal TimeFact;
            public decimal TimePlan;
            public decimal CoefPerformancePlan;
            public decimal Premium;
            public decimal Salary;
            public decimal Universality;
            public decimal Total;
            public int DayNumber;
        }

        public struct OperationsStatistics
        {
            public decimal Norm;
            public decimal TariffForVolume;
            public int Index;
            public string MachinesOperationName;
            public string Measure;
            public List<FotoWorkDay> Days;
            public decimal SummaryVolume;
            public decimal SummaryTimeFact;
            public decimal SummaryTimePlan;
            public decimal SummaryCoefPerformancePlan;
            public decimal SummaryPremium;
            public decimal SummarySalary;
            public decimal SummaryUniversality;
            public decimal SummaryTotal;
        }

        public struct WorkerMonthStatistics
        {
            public string Worker;
            public List<OperationsStatistics> Operations;
            public List<FotoWorkDay> WorkDaysSummary;
            public decimal TariffRatio;
            public decimal SummaryTimeFact;
            public decimal SummaryPremium;
            public decimal SummarySalary;
            public decimal SummaryUniversality;
            public decimal SummaryTotal;
        }

        public struct MonthWorkStatement
        {
            public string Worker;
            public decimal TariffRatio;
            public decimal SummaryTimeFact;
            public decimal SummaryPremium;
            public decimal SummarySalary;
            public decimal SummaryUniversality;
            public decimal TechnicalDiscipline;
            public decimal TechnologicalDiscipline;
            public decimal Marketing;
            public decimal SummaryTotal;
            public decimal PerHour;
        }
        public RegistrationDyeingWorkMan()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            FillReferenceTable();
            FillTariffForVolume();
        }

        private void Create()
        {
            DyeingAssignmentBarcodesDT = new DataTable();
            DyeingAssignmentBarcodeDetailsDT = new DataTable();
            MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));
            OperationsDT = new DataTable();
            PositionsDT = new DataTable();
            ReferenceDT = new DataTable();
            ReferenceDT.Columns.Add(new DataColumn("PositionID", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("Position", Type.GetType("System.String")));
            ReferenceDT.Columns.Add(new DataColumn("TariffRateOfFirstRank", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TariffRank", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TariffRatio", Type.GetType("System.Decimal")));
            ReferenceDT.Columns.Add(new DataColumn("Salary", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("PremiumPerHour", Type.GetType("System.Decimal")));
            ReferenceDT.Columns.Add(new DataColumn("Premium", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TechnicalDiscipline", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TechnologicalDiscipline", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("Total", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("PerHour", Type.GetType("System.Int32")));
            UsersDT = new DataTable();
            UsersTimeWorkDT = new DataTable();
            UsersTimeWorkDT.Columns.Add(new DataColumn("UserID", Type.GetType("System.Int32")));
            UsersTimeWorkDT.Columns.Add(new DataColumn("ShortName", Type.GetType("System.String")));
            UsersTimeWorkDT.Columns.Add(new DataColumn("TimeWork", Type.GetType("System.Decimal")));
            WorkDaysDT = new DataTable();
            YearsDT = new DataTable();
            YearsDT.Columns.Add(new DataColumn("YearID", Type.GetType("System.Int32")));
            YearsDT.Columns.Add(new DataColumn("YearName", Type.GetType("System.String")));

            //Operations = new List<OperationsStatistics>();
            //WorkDaysSummary = new List<FotoWorkDay>();
            MonthStatistics = new List<WorkerMonthStatistics>();
            TotalMonthStatistics = new WorkerMonthStatistics();
            MonthStatement = new List<MonthWorkStatement>();
            TotalMonthStatement = new MonthWorkStatement();

            MonthsBS = new BindingSource();
            UsersBS = new BindingSource();
            UsersTimeWorkBS = new BindingSource();
            YearsBS = new BindingSource();
        }

        private void CreateReferenceRow(int PositionID, string Position, int TariffRateOfFirstRank, int TariffRank, int PerHour, decimal TechnicalDiscipline = 0, decimal TechnologicalDiscipline = 0)
        {
            decimal PremiumPerHour = 0;
            decimal Premium = 0;
            decimal Salary = 0;
            decimal TariffRatio = 0;
            decimal Total = 0;

            switch (TariffRank)
            {
                case 1:
                    TariffRatio = 1;
                    break;
                case 2:
                    TariffRatio = 1.16m;
                    break;
                case 3:
                    TariffRatio = 1.35m;
                    break;
                case 4:
                    TariffRatio = 1.57m;
                    break;
                case 5:
                    TariffRatio = 1.73m;
                    break;
                default:
                    break;
            }
            Salary = TariffRateOfFirstRank * TariffRatio;
            if (TechnicalDiscipline != 0)
                TechnicalDiscipline = Salary * TariffRatio / 2;
            if (TechnologicalDiscipline != 0)
                TechnologicalDiscipline = Salary * TariffRatio / 2;
            Premium = PerHour * MonthlyCoef - Salary - TechnicalDiscipline - TechnologicalDiscipline;
            PremiumPerHour = Premium / MonthlyCoef;
            Total = Salary + Premium + TechnicalDiscipline + TechnologicalDiscipline;

            DataRow NewRow = ReferenceDT.NewRow();
            NewRow["PositionID"] = PositionID;
            NewRow["Position"] = Position;
            NewRow["TariffRateOfFirstRank"] = TariffRateOfFirstRank;
            NewRow["TariffRank"] = TariffRank;
            NewRow["TariffRatio"] = TariffRatio;
            NewRow["Salary"] = Salary;
            NewRow["PremiumPerHour"] = PremiumPerHour;
            NewRow["Premium"] = Premium;
            NewRow["TechnicalDiscipline"] = TechnicalDiscipline;
            NewRow["TechnologicalDiscipline"] = TechnologicalDiscipline;
            NewRow["Total"] = Total;
            NewRow["PerHour"] = PerHour;
            ReferenceDT.Rows.Add(NewRow);
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT MachinesOperations.*, Measures.Measure, SubSectors.SubSectorID FROM MachinesOperations
                INNER JOIN Measures ON MachinesOperations.MeasureID = Measures.MeasureID
                INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID AND SubSectors.SubSectorID IN (32)
                ORDER BY MachineID, MachinesOperationName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(OperationsDT);
                OperationsDT.Columns.Add(new DataColumn("TariffForVolume", Type.GetType("System.Decimal")));
            }
            SelectCommand = @"SELECT * FROM Positions WHERE DepartmentID=21 ORDER BY Position, TariffRank";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PositionsDT);
            }
            SelectCommand = @"SELECT UserID, Name, ShortName, PositionID FROM Users WHERE DepartmentID=21";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            SelectCommand = @"SELECT TOP 0 DyeingAssignmentBarcodes.*, DyeingCarts.Square FROM DyeingAssignmentBarcodes
                INNER JOIN DyeingCarts ON DyeingAssignmentBarcodes.DyeingCartID = DyeingCarts.DyeingCartID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentBarcodesDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignmentBarcodeDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentBarcodeDetailsDT);
            }

            for (int i = 1; i <= 12; i++)
            {
                DataRow NewRow = MonthsDT.NewRow();
                NewRow["MonthID"] = i;
                NewRow["MonthName"] = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToString();
                MonthsDT.Rows.Add(NewRow);
            }
            DateTime LastDay = new System.DateTime(DateTime.Now.Year + 1, 12, 31);
            for (int i = 2014; i <= LastDay.Year; i++)
            {
                DataRow NewRow = YearsDT.NewRow();
                NewRow["YearID"] = i;
                NewRow["YearName"] = i;
                YearsDT.Rows.Add(NewRow);
            }
        }

        private void Binding()
        {
            MonthsBS.DataSource = MonthsDT;
            UsersBS.DataSource = UsersDT;
            UsersTimeWorkBS.DataSource = UsersTimeWorkDT;
            YearsBS.DataSource = YearsDT;
            UsersTimeWorkBS.Sort = "ShortName";
        }

        private void FillOperationsMainInfo(int Year, int Month, decimal TariffRatio, decimal dSalary, ref WorkerMonthStatistics MonthStatistics)
        {
            decimal TotalSummaryTimeFact = 0;
            decimal TotalSummaryPremium = 0;
            decimal TotalSummarySalary = 0;
            decimal TotalSummaryUniversality = 0;
            decimal TotalSummaryTotal = 0;
            List<OperationsStatistics> Operations = new List<OperationsStatistics>();
            for (int i = 0; i < OperationsDT.Rows.Count; i++)
            {
                int MachinesOperationID = Convert.ToInt32(OperationsDT.Rows[i]["MachinesOperationID"]);
                decimal Norm = Convert.ToDecimal(OperationsDT.Rows[i]["Norm"]);
                decimal Square = 0;
                decimal TariffForVolume = Convert.ToDecimal(OperationsDT.Rows[i]["TariffForVolume"]);
                string MachinesOperationName = OperationsDT.Rows[i]["MachinesOperationName"].ToString();
                string Measure = OperationsDT.Rows[i]["Measure"].ToString();
                decimal SummaryVolume = 0;
                decimal SummaryTimeFact = 0;
                decimal SummaryTimePlan = 0;
                decimal SummaryCoefPerformancePlan = 0;
                decimal SummaryPremium = 0;
                decimal SummarySalary = 0;
                decimal SummaryUniversality = 0;
                decimal SummaryTotal = 0;

                List<FotoWorkDay> Days = new List<FotoWorkDay>();
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    DateTime Today = new DateTime(Year, Month, j);
                    DataRow[] rows1 = DyeingAssignmentBarcodesDT.Select("StartDate='" + Today.ToString("yyyy-MM-dd") + "' AND MachinesOperationID=" + MachinesOperationID);
                    if (rows1.Count() == 0)
                        continue;
                    decimal Volume = 0;
                    decimal TimeFact = 0;
                    decimal TimePlan = 0;
                    decimal CoefPerformancePlan = 0;
                    decimal Premium = 0;
                    decimal Salary = 0;
                    decimal Universality = 0;
                    decimal Total = 0;
                    for (int x = 0; x < rows1.Count(); x++)
                    {
                        if (rows1[x]["FinishDateTime"] == DBNull.Value)
                            continue;
                        Square = Convert.ToDecimal(rows1[x]["Square"]);
                        int DyeingAssignmentBarcodeID = Convert.ToInt32(rows1[x]["DyeingAssignmentBarcodeID"]);
                        DataRow[] rows2 = DyeingAssignmentBarcodeDetailsDT.Select("DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID);
                        if (rows2.Count() == 0)
                            continue;
                        DataTable Table = new DataTable();
                        using (DataView DV = new DataView(DyeingAssignmentBarcodeDetailsDT, "DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID, string.Empty, DataViewRowState.CurrentRows))
                        {
                            Table = DV.ToTable(true, new string[] { "DyeingAssignmentUserID" });
                        }
                        int WorkersInOperation = Table.Rows.Count;
                        DateTime StartDateTime = Convert.ToDateTime(rows1[x]["StartDateTime"]);
                        DateTime FinishDateTime = Convert.ToDateTime(rows1[x]["FinishDateTime"]);
                        TimeSpan t = FinishDateTime - StartDateTime;
                        Square = Square / WorkersInOperation;
                        Volume += Square;
                        TimeFact += Convert.ToDecimal(t.TotalHours);
                        if (WorkersInOperation != 0)
                            TimeFact = TimeFact / WorkersInOperation;
                        if (Norm != 0)
                            TimePlan += Volume / Norm;
                        if (TimeFact == 0)
                            CoefPerformancePlan = 0;
                        else
                            CoefPerformancePlan += TimePlan / TimeFact;
                        Premium += Volume * TariffForVolume;
                        if (Volume > 0)
                        {

                        }
                    }
                    if (AverageMonthlyCoef != 0)
                        Salary = dSalary * Volume * 1.03m / AverageMonthlyCoef;
                    if (AverageMonthlyCoef != 0)
                        Universality = dSalary * Volume * TariffRatio * 1.03m / AverageMonthlyCoef;
                    Total = Premium + Salary + Universality;

                    SummaryVolume += Volume;
                    SummaryTimeFact += TimeFact;

                    Days.Add(new FotoWorkDay()
                    {
                        Day = Today,
                        DayNumber = j,
                        Volume = Volume,
                        TimeFact = TimeFact,
                        TimePlan = TimePlan,
                        Premium = Premium,
                        Salary = Salary,
                        Universality = Universality,
                        Total = Total,
                        CoefPerformancePlan = CoefPerformancePlan,

                    });
                }
                if (Norm != 0)
                    SummaryTimePlan = SummaryVolume / Norm;
                if (SummaryTimeFact == 0)
                    SummaryCoefPerformancePlan = 0;
                else
                    SummaryCoefPerformancePlan += SummaryTimePlan / SummaryTimeFact;
                SummaryPremium = SummaryVolume * TariffForVolume;
                if (AverageMonthlyCoef != 0)
                    SummarySalary = dSalary * SummaryVolume * 1.03m / AverageMonthlyCoef;
                if (AverageMonthlyCoef != 0)
                    SummaryUniversality = dSalary * SummaryVolume * TariffRatio * 1.03m / AverageMonthlyCoef;
                SummaryTotal = SummaryPremium + SummarySalary + SummaryUniversality;
                TotalSummaryTimeFact += SummaryTimeFact;
                TotalSummaryPremium += SummaryPremium;
                TotalSummarySalary += SummarySalary;
                TotalSummaryUniversality += SummaryUniversality;
                TotalSummaryTotal += SummaryTotal;
                Operations.Add(new OperationsStatistics()
                {
                    Index = i + 1,
                    MachinesOperationName = MachinesOperationName,
                    Measure = Measure,
                    Norm = Norm,
                    TariffForVolume = TariffForVolume,
                    Days = Days,
                    SummaryVolume = SummaryVolume,
                    SummaryTimeFact = SummaryTimeFact,
                    SummaryTimePlan = SummaryTimePlan,
                    SummaryPremium = SummaryPremium,
                    SummarySalary = SummarySalary,
                    SummaryUniversality = SummaryUniversality,
                    SummaryTotal = SummaryTotal,
                    SummaryCoefPerformancePlan = SummaryCoefPerformancePlan
                });
            }
            MonthStatistics.Operations = Operations;
            MonthStatistics.TariffRatio = TariffRatio;
            MonthStatistics.SummaryTimeFact = TotalSummaryTimeFact;
            MonthStatistics.SummaryPremium = TotalSummaryPremium;
            MonthStatistics.SummarySalary = TotalSummarySalary;
            MonthStatistics.SummaryUniversality = TotalSummaryUniversality;
            MonthStatistics.SummaryTotal = TotalSummaryTotal;
        }

        private void FillReferenceTable()
        {
            for (int i = 0; i < PositionsDT.Rows.Count; i++)
            {
                int PositionID = Convert.ToInt32(PositionsDT.Rows[i]["PositionID"]);
                string Position = PositionsDT.Rows[i]["Position"].ToString();
                int TariffRateOfFirstRank = Convert.ToInt32(PositionsDT.Rows[i]["TariffRateOfFirstRank"]);
                int TariffRank = Convert.ToInt32(PositionsDT.Rows[i]["TariffRank"]);
                int PerHour = Convert.ToInt32(PositionsDT.Rows[i]["PerHour"]);

                if (PositionID == 50 || PositionID == 54 || PositionID == 45 || PositionID == 53)
                    CreateReferenceRow(PositionID, Position, TariffRateOfFirstRank, TariffRank, PerHour);
                else
                    CreateReferenceRow(PositionID, Position, TariffRateOfFirstRank, TariffRank, PerHour, 1, 1);
            }
        }

        private void FillTariffForVolume()
        {
            for (int i = 0; i < OperationsDT.Rows.Count; i++)
            {
                int PositionID = Convert.ToInt32(OperationsDT.Rows[i]["PositionID"]);
                decimal Norm = Convert.ToDecimal(OperationsDT.Rows[i]["Norm"]);
                decimal PremiumPerHour = 0;

                DataRow[] rows = ReferenceDT.Select("PositionID=" + PositionID);
                if (rows.Count() > 0 && rows[0]["PremiumPerHour"] != DBNull.Value)
                    PremiumPerHour = Convert.ToDecimal(rows[0]["PremiumPerHour"]);

                if (Norm == 0)
                    OperationsDT.Rows[i]["TariffForVolume"] = 0;
                else
                    OperationsDT.Rows[i]["TariffForVolume"] = PremiumPerHour / Norm;
            }
        }

        public void FillUsersTimeWorkTable()
        {
            DateTime DayStartDateTime;
            DateTime DayEndDateTime;
            DateTime DayBreakStartDateTime;
            DateTime DayBreakEndDateTime;
            decimal TimeWork = 0;
            UsersTimeWorkDT.Clear();

            for (int i = 0; i < UsersDT.Rows.Count; i++)
            {
                int UserID = Convert.ToInt32(UsersDT.Rows[i]["UserID"]);
                string ShortName = UsersDT.Rows[i]["ShortName"].ToString();
                DataRow[] rows = WorkDaysDT.Select("UserID=" + UserID);
                TimeWork = 0;
                if (rows.Count() > 0)
                {
                    for (int j = 0; j < rows.Count(); j++)
                    {
                        if (rows[j]["DayStartDateTime"] != DBNull.Value && rows[j]["DayEndDateTime"] != DBNull.Value)
                        {
                            DayStartDateTime = (DateTime)rows[j]["DayStartDateTime"];
                            DayEndDateTime = (DateTime)rows[j]["DayEndDateTime"];
                            TimeSpan t;
                            t = DayEndDateTime.TimeOfDay - DayStartDateTime.TimeOfDay;
                            if (rows[j]["DayBreakStartDateTime"] != DBNull.Value && rows[j]["DayBreakEndDateTime"] != DBNull.Value)
                            {
                                DayBreakStartDateTime = (DateTime)rows[j]["DayBreakStartDateTime"];
                                DayBreakEndDateTime = (DateTime)rows[j]["DayBreakEndDateTime"];
                                t -= DayBreakEndDateTime.TimeOfDay - DayBreakStartDateTime.TimeOfDay;
                            }
                            TimeWork = (decimal)t.TotalHours;
                            TimeWork = Decimal.Round(TimeWork, 1, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                DataRow NewRow = UsersTimeWorkDT.NewRow();
                NewRow["UserID"] = UserID;
                NewRow["ShortName"] = ShortName;
                NewRow["TimeWork"] = TimeWork;
                UsersTimeWorkDT.Rows.Add(NewRow);
            }
            UsersTimeWorkDT.AcceptChanges();
            UsersTimeWorkBS.MoveFirst();
        }

        private WorkerMonthStatistics CreateMonthStatistics(int Year, int Month, decimal TariffRatio, decimal dSalary, string Worker)
        {
            WorkerMonthStatistics NewItem = new WorkerMonthStatistics();
            FillOperationsMainInfo(Year, Month, TariffRatio, dSalary, ref NewItem);
            NewItem.WorkDaysSummary = new List<FotoWorkDay>();

            for (int y = 1; y <= DateTime.DaysInMonth(Year, Month); y++)
            {
                decimal Volume = 0;
                decimal TimeFact = 0;
                decimal TimePlan = 0;
                decimal CoefPerformancePlan = 0;
                decimal Premium = 0;
                decimal Salary = 0;
                decimal Universality = 0;
                decimal Total = 0;

                DateTime Today = new DateTime(Year, Month, y);
                for (int x = 0; x < NewItem.Operations.Count; x++)
                {
                    for (int c = 0; c < NewItem.Operations[x].Days.Count; c++)
                    {
                        int DayNumber = Convert.ToInt32(NewItem.Operations[x].Days[c].DayNumber);
                        if (y != DayNumber)
                            continue;
                        Volume += NewItem.Operations[x].Days[c].Volume;
                        TimeFact += NewItem.Operations[x].Days[c].TimeFact;
                        TimePlan = NewItem.Operations[x].Days[c].TimePlan;
                        Premium += NewItem.Operations[x].Days[c].Premium;
                    }
                }
                if (TimeFact == 0)
                    CoefPerformancePlan = 0;
                else
                    CoefPerformancePlan = TimePlan / TimeFact;
                Salary = dSalary * Volume * 1.03m / AverageMonthlyCoef;
                Universality = dSalary * Volume * TariffRatio * 1.03m / AverageMonthlyCoef;
                Total = Premium + Salary + Universality;
                NewItem.WorkDaysSummary.Add(new FotoWorkDay()
                {
                    Day = Today,
                    DayNumber = y,
                    Volume = Volume,
                    TimeFact = TimeFact,
                    TimePlan = TimePlan,
                    Premium = Premium,
                    CoefPerformancePlan = CoefPerformancePlan,
                    Salary = Salary,
                    Universality = Universality,
                    Total = Total
                });
            }
            NewItem.Worker = Worker;
            return NewItem;
        }

        private void CreateTotalMonthStatistics(int Year, int Month, decimal TariffRatio, decimal dSalary, string Worker)
        {
            TotalMonthStatistics.Operations = new List<OperationsStatistics>();
            TotalMonthStatistics.WorkDaysSummary = new List<FotoWorkDay>();
            decimal TotalSummaryTimeFact = 0;
            decimal TotalSummaryPremium = 0;
            decimal TotalSummarySalary = 0;
            decimal TotalSummaryUniversality = 0;
            decimal TotalSummaryTotal = 0;
            for (int i = 0; i < OperationsDT.Rows.Count; i++)
            {
                int MachinesOperationID = Convert.ToInt32(OperationsDT.Rows[i]["MachinesOperationID"]);
                decimal Norm = Convert.ToDecimal(OperationsDT.Rows[i]["Norm"]);
                decimal Square = 0;
                decimal TariffForVolume = Convert.ToDecimal(OperationsDT.Rows[i]["TariffForVolume"]);
                string MachinesOperationName = OperationsDT.Rows[i]["MachinesOperationName"].ToString();
                string Measure = OperationsDT.Rows[i]["Measure"].ToString();
                decimal SummaryVolume = 0;
                decimal SummaryTimeFact = 0;
                decimal SummaryTimePlan = 0;
                decimal SummaryCoefPerformancePlan = 0;
                decimal SummaryPremium = 0;
                decimal SummarySalary = 0;
                decimal SummaryUniversality = 0;
                decimal SummaryTotal = 0;

                List<FotoWorkDay> Days = new List<FotoWorkDay>();
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    decimal Volume = Square;
                    decimal TimeFact = 0;
                    decimal TimePlan = 0;
                    decimal CoefPerformancePlan = 0;
                    decimal Premium = 0;
                    decimal Salary = 0;
                    decimal Universality = 0;
                    decimal Total = 0;

                    for (int x = 0; x < MonthStatistics.Count; x++)
                    {
                        for (int z = 0; z < MonthStatistics[x].Operations[i].Days.Count; z++)
                        {
                            int DayNumber = Convert.ToInt32(MonthStatistics[x].Operations[i].Days[z].DayNumber);
                            if (j != DayNumber)
                                continue;
                            Volume += MonthStatistics[x].Operations[i].Days[z].Volume;
                            TimeFact += MonthStatistics[x].Operations[i].Days[z].TimeFact;
                        }
                    }
                    TimePlan = Volume / Norm;

                    if (TimeFact == 0)
                        CoefPerformancePlan = 0;
                    else
                        CoefPerformancePlan = TimePlan / TimeFact;
                    Premium = Volume * TariffForVolume;
                    Salary = dSalary * Volume * 1.03m / AverageMonthlyCoef;
                    Universality = dSalary * Volume * TariffRatio * 1.03m / AverageMonthlyCoef;
                    Total = Premium + Salary + Universality;
                    SummaryVolume += Volume;
                    SummaryTimeFact += TimeFact;
                    Days.Add(new FotoWorkDay()
                    {
                        DayNumber = j,
                        Volume = Volume,
                        TimeFact = TimeFact,
                        TimePlan = TimePlan,
                        Premium = Premium,
                        CoefPerformancePlan = CoefPerformancePlan,
                        Salary = Salary,
                        Universality = Universality,
                        Total = Total
                    });
                }
                SummaryTimePlan = SummaryVolume / Norm;
                if (SummaryTimeFact == 0)
                    SummaryCoefPerformancePlan = 0;
                else
                    SummaryCoefPerformancePlan += SummaryTimePlan / SummaryTimeFact;
                SummaryPremium = SummaryVolume * TariffForVolume;
                SummarySalary = dSalary * SummaryVolume * 1.03m / AverageMonthlyCoef;
                SummaryUniversality = dSalary * SummaryVolume * TariffRatio * 1.03m / AverageMonthlyCoef;
                SummaryTotal = SummaryPremium + SummarySalary + SummaryUniversality;
                TotalSummaryTimeFact += SummaryTimeFact;
                TotalSummaryPremium += SummaryPremium;
                TotalSummarySalary += SummarySalary;
                TotalSummaryUniversality += SummaryUniversality;
                TotalSummaryTotal += SummaryTotal;
                TotalMonthStatistics.Operations.Add(new OperationsStatistics()
                {
                    Index = i + 1,
                    MachinesOperationName = MachinesOperationName,
                    Measure = Measure,
                    Norm = Norm,
                    TariffForVolume = TariffForVolume,
                    Days = Days,
                    SummaryVolume = SummaryVolume,
                    SummaryTimeFact = SummaryTimeFact,
                    SummaryTimePlan = SummaryTimePlan,
                    SummaryPremium = SummaryPremium,
                    SummarySalary = SummarySalary,
                    SummaryUniversality = SummaryUniversality,
                    SummaryTotal = SummaryTotal,
                    SummaryCoefPerformancePlan = SummaryCoefPerformancePlan
                });
            }

            for (int y = 1; y <= DateTime.DaysInMonth(Year, Month); y++)
            {
                decimal Volume = 0;
                decimal TimeFact = 0;
                decimal TimePlan = 0;
                decimal CoefPerformancePlan = 0;
                decimal Premium = 0;
                decimal Salary = 0;
                decimal Universality = 0;
                decimal Total = 0;

                DateTime Today = new DateTime(Year, Month, y);
                for (int x = 0; x < TotalMonthStatistics.Operations.Count; x++)
                {
                    for (int c = 0; c < TotalMonthStatistics.Operations[x].Days.Count; c++)
                    {
                        int DayNumber = Convert.ToInt32(TotalMonthStatistics.Operations[x].Days[c].DayNumber);
                        if (y != DayNumber)
                            continue;
                        Volume += TotalMonthStatistics.Operations[x].Days[c].Volume;
                        TimeFact += TotalMonthStatistics.Operations[x].Days[c].TimeFact;
                        TimePlan = TotalMonthStatistics.Operations[x].Days[c].TimePlan;
                        Premium += TotalMonthStatistics.Operations[x].Days[c].Premium;
                    }
                }
                if (TimeFact == 0)
                    CoefPerformancePlan = 0;
                else
                    CoefPerformancePlan = TimePlan / TimeFact;
                Salary = dSalary * Volume * 1.03m / AverageMonthlyCoef;
                Universality = dSalary * Volume * TariffRatio * 1.03m / AverageMonthlyCoef;
                Total = Premium + Salary + Universality;
                TotalMonthStatistics.WorkDaysSummary.Add(new FotoWorkDay()
                {
                    Day = Today,
                    DayNumber = y,
                    Volume = Volume,
                    TimeFact = TimeFact,
                    TimePlan = TimePlan,
                    Premium = Premium,
                    CoefPerformancePlan = CoefPerformancePlan,
                    Salary = Salary,
                    Universality = Universality,
                    Total = Total
                });
            }
            TotalMonthStatistics.SummaryTimeFact = TotalSummaryTimeFact;
            TotalMonthStatistics.SummaryPremium = TotalSummaryPremium;
            TotalMonthStatistics.SummarySalary = TotalSummarySalary;
            TotalMonthStatistics.SummaryUniversality = TotalSummaryUniversality;
            TotalMonthStatistics.SummaryTotal = TotalSummaryTotal;
            TotalMonthStatistics.Worker = "Итого";
        }

        private void UpdateBarcodes(DateTime StartDate, DateTime FinishDate, int UserID)
        {
            string SelectCommand = @"SELECT DyeingAssignmentBarcodes.*, CAST(StartDateTime AS Date) AS StartDate, DyeingCarts.Square, infiniu2_catalog.dbo.TechCatalogOperationsDetail.MachinesOperationID FROM DyeingAssignmentBarcodes
                INNER JOIN DyeingCarts ON DyeingAssignmentBarcodes.DyeingCartID = DyeingCarts.DyeingCartID
                INNER JOIN infiniu2_catalog.dbo.TechCatalogOperationsDetail ON DyeingAssignmentBarcodes.TechCatalogOperationsDetailID = infiniu2_catalog.dbo.TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                WHERE DyeingAssignmentBarcodeID IN (SELECT DyeingAssignmentBarcodeID FROM DyeingAssignmentBarcodeDetails WHERE DyeingAssignmentUserID=" + UserID + ") AND CAST(StartDateTime AS Date)>='" + StartDate.ToString("yyyy-MM-dd") + "' AND CAST(FinishDateTime AS Date)<='" + FinishDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentBarcodesDT.Clear();
                DA.Fill(DyeingAssignmentBarcodesDT);
            }
            SelectCommand = @"SELECT * FROM DyeingAssignmentBarcodeDetails
                WHERE DyeingAssignmentBarcodeID IN (SELECT DyeingAssignmentBarcodeID FROM DyeingAssignmentBarcodes
                WHERE CAST(StartDateTime AS Date)>='" + StartDate.ToString("yyyy-MM-dd") + "' AND CAST(FinishDateTime AS Date)<='" + FinishDate.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentBarcodeDetailsDT.Clear();
                DA.Fill(DyeingAssignmentBarcodeDetailsDT);
            }
        }

        public void UpdateWorkDays(DateTime DayStartDateTime)
        {
            string SelectCommand = @"SELECT WorkDays.*, CAST(DayStartDateTime AS DATE) AS StartDate FROM WorkDays WHERE CAST(DayStartDateTime AS DATE) = '" + DayStartDateTime.ToString("yyyy-MM-dd") +
                "' AND UserID IN (SELECT UserID FROM infiniu2_users.dbo.Users WHERE DepartmentID = 21)";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                WorkDaysDT.Clear();
                DA.Fill(WorkDaysDT);
            }
        }

        public void SaveWorkDays(DateTime DateTime)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM WorkDays", ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (UsersTimeWorkDT.GetChanges() != null)
                    {
                        DataTable DT = UsersTimeWorkDT.GetChanges();
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            decimal TimeWork = Convert.ToDecimal(DT.Rows[i]["TimeWork"]);
                            int UserID = Convert.ToInt32(DT.Rows[i]["UserID"]);
                            if (TimeWork == 0)
                                continue;
                            TimeSpan t = TimeSpan.FromHours((double)TimeWork);
                            DateTime DayStartDateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 8, 0, 0);
                            DateTime DayEndDateTime = DayStartDateTime.Add(t);

                            DataRow[] rows1 = WorkDaysDT.Select("StartDate='" + DateTime.ToString("yyyy-MM-dd") + "' AND UserID=" + UserID);
                            if (rows1.Count() == 0)
                            {
                                DataRow NewRow = WorkDaysDT.NewRow();
                                NewRow["UserID"] = UserID;
                                NewRow["DayStartDateTime"] = DayStartDateTime;
                                NewRow["DayStartFactDateTime"] = DayStartDateTime;
                                NewRow["DayEndDateTime"] = DayEndDateTime;
                                NewRow["DayEndFactDateTime"] = DayEndDateTime;
                                NewRow["Saved"] = true;
                                WorkDaysDT.Rows.Add(NewRow);
                            }
                            else
                            {
                                rows1[0]["UserID"] = UserID;
                                rows1[0]["DayStartDateTime"] = DayStartDateTime;
                                rows1[0]["DayStartFactDateTime"] = DayStartDateTime;
                                rows1[0]["DayBreakStartDateTime"] = DBNull.Value;
                                rows1[0]["DayBreakStartFactDateTime"] = DBNull.Value;
                                rows1[0]["DayEndDateTime"] = DayEndDateTime;
                                rows1[0]["DayEndFactDateTime"] = DayEndDateTime;
                                rows1[0]["DayBreakEndDateTime"] = DBNull.Value;
                                rows1[0]["DayBreakEndFactDateTime"] = DBNull.Value;
                                rows1[0]["Saved"] = true;
                            }
                        }
                        DA.Update(WorkDaysDT);
                        DT.Dispose();
                    }
                }
            }
        }

        public void CreateReport(int Year, int Month)
        {
            hssfworkbook = new HSSFWorkbook();
            DateTime StartDate = new DateTime(Year, Month, 1);
            DateTime FinishDate = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));
            int PositionID = 60;
            int UserID = 0;
            decimal TariffRatio = 0;
            decimal Salary = 0;
            string Worker = string.Empty;
            MonthStatistics.Clear();

            DataRow[] rows = ReferenceDT.Select("PositionID=" + PositionID);
            if (rows.Count() > 0 && rows[0]["TariffRatio"] != DBNull.Value)
                TariffRatio = Convert.ToDecimal(rows[0]["TariffRatio"]);
            if (rows.Count() > 0 && rows[0]["Salary"] != DBNull.Value)
                Salary = Convert.ToDecimal(rows[0]["Salary"]);

            UserID = 435;
            UpdateBarcodes(StartDate, FinishDate, UserID);
            Worker = "Кондратюк Д.И.";
            MonthStatistics.Add(CreateMonthStatistics(Year, Month, TariffRatio, Salary, Worker));
            UserID = 385;
            UpdateBarcodes(StartDate, FinishDate, UserID);
            Worker = "Курейко И.В.";
            MonthStatistics.Add(CreateMonthStatistics(Year, Month, TariffRatio, Salary, Worker));
            UserID = 431;
            UpdateBarcodes(StartDate, FinishDate, UserID);
            Worker = "Хайко В.И.";
            MonthStatistics.Add(CreateMonthStatistics(Year, Month, TariffRatio, Salary, Worker));
            UserID = 450;
            UpdateBarcodes(StartDate, FinishDate, UserID);
            Worker = "Сенкевич А.М.";
            MonthStatistics.Add(CreateMonthStatistics(Year, Month, TariffRatio, Salary, Worker));
            UserID = 451;
            UpdateBarcodes(StartDate, FinishDate, UserID);
            Worker = "Синкевич А.С.";
            MonthStatistics.Add(CreateMonthStatistics(Year, Month, TariffRatio, Salary, Worker));

            MonthStatement.Clear();
            decimal TotalSummaryTimeFact = 0;
            decimal TotalSummaryPremium = 0;
            decimal TotalSummarySalary = 0;
            decimal TotalSummaryUniversality = 0;
            decimal TotalTechnicalDiscipline = 0;
            decimal TotalTechnologicalDiscipline = 0;
            decimal TotalMarketing = 0;
            decimal TotalSummaryTotal = 0;
            decimal TotalPerHour = 0;
            for (int x = 0; x < MonthStatistics.Count; x++)
            {
                decimal TechnologicalDiscipline = MonthStatistics[x].SummaryUniversality * 0.33m;
                decimal TechnicalDiscipline = TechnologicalDiscipline * 0.33m;
                decimal Marketing = TechnicalDiscipline * 0.33m;
                decimal SummaryTotal = MonthStatistics[x].SummaryPremium + MonthStatistics[x].SummarySalary + MonthStatistics[x].SummaryUniversality;
                decimal PerHour = 0;
                if (MonthStatistics[x].SummaryTimeFact != 0)
                    PerHour = SummaryTotal / MonthStatistics[x].SummaryTimeFact;
                PerHour = Decimal.Round(PerHour, 1, MidpointRounding.AwayFromZero);
                TotalSummaryTimeFact += MonthStatistics[x].SummaryTimeFact;
                TotalSummaryPremium += MonthStatistics[x].SummaryPremium;
                TotalSummarySalary += MonthStatistics[x].SummarySalary;
                TotalSummaryUniversality += MonthStatistics[x].SummaryUniversality;
                TotalTechnologicalDiscipline += TechnologicalDiscipline;
                TotalTechnicalDiscipline += TechnicalDiscipline;
                TotalMarketing += Marketing;
                TotalSummaryTotal += SummaryTotal;
                MonthStatement.Add(new MonthWorkStatement()
                {
                    Worker = MonthStatistics[x].Worker,
                    TariffRatio = MonthStatistics[x].TariffRatio,
                    SummaryTimeFact = MonthStatistics[x].SummaryTimeFact,
                    SummarySalary = MonthStatistics[x].SummarySalary,
                    SummaryPremium = MonthStatistics[x].SummaryPremium,
                    SummaryUniversality = MonthStatistics[x].SummaryUniversality,
                    TechnologicalDiscipline = TechnologicalDiscipline,
                    TechnicalDiscipline = TechnicalDiscipline,
                    Marketing = Marketing,
                    SummaryTotal = SummaryTotal,
                    PerHour = PerHour
                });
            }
            if (TotalSummaryTimeFact != 0)
                TotalPerHour = TotalSummaryTotal / TotalSummaryTimeFact;
            TotalPerHour = Decimal.Round(TotalPerHour, 1, MidpointRounding.AwayFromZero);
            TotalMonthStatement.Worker = "ИТОГО";
            TotalMonthStatement.SummaryTimeFact = TotalSummaryTimeFact;
            TotalMonthStatement.SummarySalary = TotalSummarySalary;
            TotalMonthStatement.SummaryPremium = TotalSummaryPremium;
            TotalMonthStatement.SummaryUniversality = TotalSummaryUniversality;
            TotalMonthStatement.TechnologicalDiscipline = TotalTechnologicalDiscipline;
            TotalMonthStatement.TechnicalDiscipline = TotalTechnicalDiscipline;
            TotalMonthStatement.Marketing = TotalMarketing;
            TotalMonthStatement.SummaryTotal = TotalSummaryTotal;
            TotalMonthStatement.PerHour = TotalPerHour;

            CreateTotalMonthStatistics(Year, Month, TariffRatio, Salary, Worker);

            RatesToExcel();
            WorkerMonthStatisticsToExcel(Year, Month, MonthStatistics[0]);
            WorkerMonthStatisticsToExcel(Year, Month, MonthStatistics[1]);
            WorkerMonthStatisticsToExcel(Year, Month, MonthStatistics[2]);
            WorkerMonthStatisticsToExcel(Year, Month, MonthStatistics[3]);
            WorkerMonthStatisticsToExcel(Year, Month, MonthStatistics[4]);
            TotalMonthStatisticsToExcel(Year, Month);
            WorkStatementToExcel(Year, Month);

            string FileName1 = StartDate.ToString("yyyy MMMM") + " Отчет";
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            //string FileName1 = StartDispatchDate.ToString("dd.MM") + "-" + FinishDispatchDate.ToString("dd.MM") + " Отчет";
            //string tempFolder = @"\\192.168.1.6\Public\ТПС\Infinium\Задания на покраску\";
            //string CurrentMonthName = DateTime.Now.ToString("MMMM");
            //tempFolder = Path.Combine(tempFolder, CurrentMonthName);
            if (!(Directory.Exists(tempFolder)))
            {
                Directory.CreateDirectory(tempFolder);
            }
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName1 + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName1 + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        private void RatesToExcel()
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Расценки, нормы");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetZoom(9, 10);   // 90 percent magnification

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 20 * 256);
            sheet1.SetColumnWidth(1, 55 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 15 * 256);
            sheet1.SetColumnWidth(9, 15 * 256);
            sheet1.SetColumnWidth(10, 15 * 256);

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont TimesNewRoman11F = hssfworkbook.CreateFont();
            TimesNewRoman11F.FontHeightInPoints = 11;
            TimesNewRoman11F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB11F = hssfworkbook.CreateFont();
            TimesNewRomanB11F.FontHeightInPoints = 11;
            TimesNewRomanB11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB11F.FontName = "Times New Roman";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle CoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            CoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            CoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderTop = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle EmptyCellCS = hssfworkbook.CreateCellStyle();
            EmptyCellCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            EmptyCellCS.RightBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.TopBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle IndexCS = hssfworkbook.CreateCellStyle();
            IndexCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            IndexCS.BottomBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            IndexCS.LeftBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            IndexCS.RightBorderColor = HSSFColor.BLACK.index;
            //IndexCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //IndexCS.TopBorderColor = HSSFColor.BLACK.index;
            IndexCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            IndexCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle MeasureCS = hssfworkbook.CreateCellStyle();
            MeasureCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MeasureCS.BottomBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MeasureCS.LeftBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MeasureCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //MeasureCS.TopBorderColor = HSSFColor.BLACK.index;
            MeasureCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            MeasureCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PositionCS = hssfworkbook.CreateCellStyle();
            PositionCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PositionCS.BottomBorderColor = HSSFColor.BLACK.index;
            PositionCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PositionCS.LeftBorderColor = HSSFColor.BLACK.index;
            PositionCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            PositionCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //PositionCS.TopBorderColor = HSSFColor.BLACK.index;
            PositionCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            PositionCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle NormCS = hssfworkbook.CreateCellStyle();
            NormCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NormCS.BottomBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NormCS.LeftBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            NormCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //NormCS.TopBorderColor = HSSFColor.BLACK.index;
            NormCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            NormCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PremiumCS = hssfworkbook.CreateCellStyle();
            PremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            PremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            PremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            PremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            PremiumCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            //TariffForVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //TariffForVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TariffForVolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TariffRatioCS = hssfworkbook.CreateCellStyle();
            TariffRatioCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TariffRatioCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffRatioCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffRatioCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffRatioCS.RightBorderColor = HSSFColor.BLACK.index;
            TariffRatioCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TariffRatioCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffRatioCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TariffRatioCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle LastRowCS = hssfworkbook.CreateCellStyle();
            LastRowCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            LastRowCS.TopBorderColor = HSSFColor.BLACK.index;

            #endregion

            int ColumnIndex = 0;
            int RowIndex = 0;

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Профессия");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Тарифная ставка 1-ого разряда");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Тарифный разряд");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Тарифный коэф. по ЕТС");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Оклад");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Премия за час");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Премия");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Технич. дисциплина");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Технич. дисциплина");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "ИТОГО");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "За час");
            cell.CellStyle = HeaderCS;
            RowIndex++;
            ColumnIndex = 0;

            for (int i = 0; i < ReferenceDT.Rows.Count; i++)
            {
                int PositionID = Convert.ToInt32(ReferenceDT.Rows[i]["PositionID"]);
                int TariffRateOfFirstRank = Convert.ToInt32(ReferenceDT.Rows[i]["TariffRateOfFirstRank"]);
                int TariffRank = Convert.ToInt32(ReferenceDT.Rows[i]["TariffRank"]);
                decimal TariffRatio = Convert.ToDecimal(ReferenceDT.Rows[i]["TariffRatio"]);
                int Salary = Convert.ToInt32(ReferenceDT.Rows[i]["Salary"]);
                decimal PremiumPerHour = Convert.ToDecimal(ReferenceDT.Rows[i]["PremiumPerHour"]);
                int Premium = Convert.ToInt32(ReferenceDT.Rows[i]["Premium"]);
                int TechnicalDiscipline = Convert.ToInt32(ReferenceDT.Rows[i]["TechnicalDiscipline"]);
                int TechnologicalDiscipline = Convert.ToInt32(ReferenceDT.Rows[i]["TechnologicalDiscipline"]);
                int Total = Convert.ToInt32(ReferenceDT.Rows[i]["Total"]);
                int PerHour = Convert.ToInt32(ReferenceDT.Rows[i]["PerHour"]);
                string Position = ReferenceDT.Rows[i]["Position"].ToString();
                ColumnIndex = 0;

                if (i == 0)
                {
                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(Position);
                    cell.CellStyle = PositionCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(TariffRateOfFirstRank);
                    cell.CellStyle = PremiumCS;
                }
                else
                {

                    if (i != 0 && PositionID != Convert.ToInt32(ReferenceDT.Rows[i - 1]["PositionID"]) && TariffRateOfFirstRank != Convert.ToInt32(ReferenceDT.Rows[i - 1]["TariffRateOfFirstRank"]))
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.SetCellValue(Position);
                        cell.CellStyle = PositionCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.SetCellValue(TariffRateOfFirstRank);
                        cell.CellStyle = PremiumCS;
                    }
                    else
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.CellStyle = EmptyCellCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.CellStyle = EmptyCellCS;
                    }
                }

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TariffRank);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TariffRatio));
                cell.CellStyle = TariffRatioCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Salary);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(PremiumPerHour));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Premium);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TechnicalDiscipline);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TechnologicalDiscipline);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Total);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(PerHour);
                cell.CellStyle = PremiumCS;

                RowIndex++;
            }
            ColumnIndex = 0;
            for (int i = 0; i < 11; i++)
            {
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.CellStyle = LastRowCS;
            }

            RowIndex++;
            ColumnIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "№");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Операция");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Ед.изм.");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Норма");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Расценка на объем");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            RowIndex++;
            RowIndex++;

            for (int x = 0; x < MonthStatistics[0].Operations.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics[0].Operations[x].Index);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics[0].Operations[x].MachinesOperationName);
                cell.CellStyle = PositionCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics[0].Operations[x].Measure);
                cell.CellStyle = MeasureCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics[0].Operations[x].Norm));
                cell.CellStyle = NormCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics[0].Operations[x].TariffForVolume));
                cell.CellStyle = TariffForVolumeCS;

                RowIndex++;
            }
            ColumnIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.CellStyle = LastRowCS;
            }
        }

        private void WorkerMonthStatisticsToExcel(int Year, int Month, WorkerMonthStatistics MonthStatistics)
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet(MonthStatistics.Worker);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetZoom(9, 10);   // 90 percent magnification

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 8 * 256);
            sheet1.SetColumnWidth(1, 55 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 10 * 256);

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont TimesNewRoman11F = hssfworkbook.CreateFont();
            TimesNewRoman11F.FontHeightInPoints = 11;
            TimesNewRoman11F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB11F = hssfworkbook.CreateFont();
            TimesNewRomanB11F.FontHeightInPoints = 11;
            TimesNewRomanB11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB11F.FontName = "Times New Roman";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle CoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            CoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            CoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle EmptyCellCS = hssfworkbook.CreateCellStyle();
            EmptyCellCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.RightBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.TopBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle IndexCS = hssfworkbook.CreateCellStyle();
            IndexCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            IndexCS.BottomBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            IndexCS.LeftBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            IndexCS.RightBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            IndexCS.TopBorderColor = HSSFColor.BLACK.index;
            IndexCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            IndexCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle MeasureCS = hssfworkbook.CreateCellStyle();
            MeasureCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MeasureCS.BottomBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MeasureCS.LeftBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MeasureCS.RightBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            MeasureCS.TopBorderColor = HSSFColor.BLACK.index;
            MeasureCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            MeasureCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle OperationNameCS = hssfworkbook.CreateCellStyle();
            OperationNameCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.BottomBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.LeftBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.RightBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.TopBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            OperationNameCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle NormCS = hssfworkbook.CreateCellStyle();
            NormCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NormCS.BottomBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NormCS.LeftBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            NormCS.RightBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            NormCS.TopBorderColor = HSSFColor.BLACK.index;
            NormCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            NormCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PremiumCS = hssfworkbook.CreateCellStyle();
            PremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            PremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            PremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            PremiumCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle TotalCS = hssfworkbook.CreateCellStyle();
            TotalCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle TariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TariffForVolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimeFactCS = hssfworkbook.CreateCellStyle();
            TimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimeFactCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimePlanCS = hssfworkbook.CreateCellStyle();
            TimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle VolumeCS = hssfworkbook.CreateCellStyle();
            VolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            VolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            VolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            VolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            VolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            VolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            VolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TotalCoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            TotalCoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalCoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCoefPerformancePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalPremiumCS = hssfworkbook.CreateCellStyle();
            TotalPremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalPremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalPremiumCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTotalCS = hssfworkbook.CreateCellStyle();
            TotalTotalCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalTotalCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalTotalCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTotalCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TotalTariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TotalTariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalTariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TotalTariffForVolumeCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimeFactCS = hssfworkbook.CreateCellStyle();
            TotalTimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalTimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimeFactCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimePlanCS = hssfworkbook.CreateCellStyle();
            TotalTimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TotalTimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalVolumeCS = hssfworkbook.CreateCellStyle();
            TotalVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            TotalVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalVolumeCS.SetFont(CalibriBold11F);

            #endregion

            int ColumnIndex = 0;
            int RowIndex = 0;

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "№");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Операция");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Ед.изм.");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Норма");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Расценка на объем");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            for (int x = 1; x <= DateTime.DaysInMonth(Year, Month); x++)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, x.ToString());
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 1, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 2, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 3, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 4, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 5, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 6, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 7, string.Empty);
                cell.CellStyle = HeaderCS;
                sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex, ColumnIndex + 7));
                sheet1.SetColumnWidth(ColumnIndex + 5, 15 * 256);
                sheet1.SetColumnWidth(ColumnIndex + 6, 20 * 256);
                sheet1.SetColumnWidth(ColumnIndex + 7, 15 * 256);

                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "V");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Часы факт.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Часы план.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "КТУ");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Премия");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "\u03A3 за оклад");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "\u03A3 за универсальность");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Итого");
                cell.CellStyle = HeaderCS;
            }

            sheet1.CreateFreezePane(5, 2, 5, 2);
            RowIndex++;
            RowIndex++;

            for (int x = 0; x < MonthStatistics.Operations.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics.Operations[x].Index);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics.Operations[x].MachinesOperationName);
                cell.CellStyle = OperationNameCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics.Operations[x].Measure);
                cell.CellStyle = MeasureCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Norm));
                cell.CellStyle = NormCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].TariffForVolume));
                cell.CellStyle = TariffForVolumeCS;

                for (int y = 1; y <= DateTime.DaysInMonth(Year, Month); y++)
                {
                    int colIndex = ColumnIndex;
                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.CellStyle = VolumeCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.CellStyle = TimeFactCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = TimePlanCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = CoefPerformancePlanCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = PremiumCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = PremiumCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = PremiumCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = TotalCS;

                    for (int c = 0; c < MonthStatistics.Operations[x].Days.Count; c++)
                    {
                        int DayNumber = Convert.ToInt32(MonthStatistics.Operations[x].Days[c].DayNumber);
                        if (y != DayNumber)
                            continue;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].Volume));
                        cell.CellStyle = VolumeCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].TimeFact));
                        cell.CellStyle = TimeFactCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].TimePlan));
                        cell.CellStyle = TimePlanCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].CoefPerformancePlan));
                        cell.CellStyle = CoefPerformancePlanCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].Premium));
                        cell.CellStyle = PremiumCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].Salary));
                        cell.CellStyle = PremiumCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].Universality));
                        cell.CellStyle = PremiumCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Days[c].Total));
                        cell.CellStyle = TotalCS;
                    }
                }
                RowIndex++;
            }

            cell = sheet1.CreateRow(RowIndex).CreateCell(4);
            cell.SetCellValue("ИТОГО:");
            cell.CellStyle = MeasureCS;
            ColumnIndex = 5;
            for (int c = 0; c < MonthStatistics.WorkDaysSummary.Count; c++)
            {
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].Volume));
                cell.CellStyle = TotalVolumeCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].TimeFact));
                cell.CellStyle = TotalTimeFactCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].TimePlan));
                cell.CellStyle = TotalTimePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].CoefPerformancePlan));
                cell.CellStyle = TotalCoefPerformancePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].Premium));
                cell.CellStyle = TotalPremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].Salary));
                cell.CellStyle = TotalPremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].Universality));
                cell.CellStyle = TotalPremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.WorkDaysSummary[c].Total));
                cell.CellStyle = TotalTotalCS;
            }

            RowIndex++;
            RowIndex++;

            for (int x = 0; x < MonthStatistics.Operations.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics.Operations[x].Index);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics.Operations[x].MachinesOperationName);
                cell.CellStyle = OperationNameCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatistics.Operations[x].Measure);
                cell.CellStyle = MeasureCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].Norm));
                cell.CellStyle = NormCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].TariffForVolume));
                cell.CellStyle = TariffForVolumeCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummaryVolume));
                cell.CellStyle = VolumeCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummaryTimeFact));
                cell.CellStyle = TimeFactCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummaryTimePlan));
                cell.CellStyle = TimePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummaryCoefPerformancePlan));
                cell.CellStyle = CoefPerformancePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummaryPremium));
                cell.CellStyle = PremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummarySalary));
                cell.CellStyle = PremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummaryUniversality));
                cell.CellStyle = PremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatistics.Operations[x].SummaryTotal));
                cell.CellStyle = TotalCS;
                RowIndex++;
            }

            cell = sheet1.CreateRow(RowIndex).CreateCell(8);
            cell.SetCellValue("ИТОГО:");
            cell.CellStyle = MeasureCS;
            cell.CellStyle = MeasureCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(9);
            cell.SetCellValue(Convert.ToDouble(MonthStatistics.SummaryPremium));
            cell.CellStyle = PremiumCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(10);
            cell.SetCellValue(Convert.ToDouble(MonthStatistics.SummarySalary));
            cell.CellStyle = PremiumCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(11);
            cell.SetCellValue(Convert.ToDouble(MonthStatistics.SummaryUniversality));
            cell.CellStyle = PremiumCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(12);
            cell.SetCellValue(Convert.ToDouble(MonthStatistics.SummaryTotal));
            cell.CellStyle = TotalCS;
        }

        private void TotalMonthStatisticsToExcel(int Year, int Month)
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet(TotalMonthStatistics.Worker);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetZoom(9, 10);   // 90 percent magnification

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 8 * 256);
            sheet1.SetColumnWidth(1, 55 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 10 * 256);

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont TimesNewRoman11F = hssfworkbook.CreateFont();
            TimesNewRoman11F.FontHeightInPoints = 11;
            TimesNewRoman11F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB11F = hssfworkbook.CreateFont();
            TimesNewRomanB11F.FontHeightInPoints = 11;
            TimesNewRomanB11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB11F.FontName = "Times New Roman";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle CoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            CoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            CoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle EmptyCellCS = hssfworkbook.CreateCellStyle();
            EmptyCellCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.RightBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.TopBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle IndexCS = hssfworkbook.CreateCellStyle();
            IndexCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            IndexCS.BottomBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            IndexCS.LeftBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            IndexCS.RightBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            IndexCS.TopBorderColor = HSSFColor.BLACK.index;
            IndexCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            IndexCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle MeasureCS = hssfworkbook.CreateCellStyle();
            MeasureCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MeasureCS.BottomBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MeasureCS.LeftBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MeasureCS.RightBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            MeasureCS.TopBorderColor = HSSFColor.BLACK.index;
            MeasureCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            MeasureCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle OperationNameCS = hssfworkbook.CreateCellStyle();
            OperationNameCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.BottomBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.LeftBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.RightBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.TopBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            OperationNameCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle NormCS = hssfworkbook.CreateCellStyle();
            NormCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NormCS.BottomBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NormCS.LeftBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            NormCS.RightBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            NormCS.TopBorderColor = HSSFColor.BLACK.index;
            NormCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            NormCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PremiumCS = hssfworkbook.CreateCellStyle();
            PremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            PremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            PremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            PremiumCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle TotalCS = hssfworkbook.CreateCellStyle();
            TotalCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle TariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TariffForVolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimeFactCS = hssfworkbook.CreateCellStyle();
            TimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimeFactCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimePlanCS = hssfworkbook.CreateCellStyle();
            TimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle VolumeCS = hssfworkbook.CreateCellStyle();
            VolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            VolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            VolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            VolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            VolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            VolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            VolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TotalCoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            TotalCoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalCoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCoefPerformancePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalPremiumCS = hssfworkbook.CreateCellStyle();
            TotalPremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalPremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalPremiumCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTotalCS = hssfworkbook.CreateCellStyle();
            TotalTotalCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalTotalCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalTotalCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTotalCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TotalTariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TotalTariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalTariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TotalTariffForVolumeCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimeFactCS = hssfworkbook.CreateCellStyle();
            TotalTimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalTimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimeFactCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimePlanCS = hssfworkbook.CreateCellStyle();
            TotalTimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TotalTimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalVolumeCS = hssfworkbook.CreateCellStyle();
            TotalVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            TotalVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalVolumeCS.SetFont(CalibriBold11F);

            #endregion

            int ColumnIndex = 0;
            int RowIndex = 0;

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "№");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Операция");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Ед.изм.");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Норма");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Расценка на объем");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            for (int x = 1; x <= DateTime.DaysInMonth(Year, Month); x++)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, x.ToString());
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 1, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 2, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 3, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 4, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 5, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 6, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 7, string.Empty);
                cell.CellStyle = HeaderCS;
                sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex, ColumnIndex + 7));
                sheet1.SetColumnWidth(ColumnIndex + 5, 15 * 256);
                sheet1.SetColumnWidth(ColumnIndex + 6, 20 * 256);
                sheet1.SetColumnWidth(ColumnIndex + 7, 15 * 256);

                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "V");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Часы факт.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Часы план.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "КТУ");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Премия");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "\u03A3 за оклад");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "\u03A3 за универсальность");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Итого");
                cell.CellStyle = HeaderCS;
            }

            sheet1.CreateFreezePane(5, 2, 5, 2);
            RowIndex++;
            RowIndex++;

            for (int x = 0; x < TotalMonthStatistics.Operations.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TotalMonthStatistics.Operations[x].Index);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TotalMonthStatistics.Operations[x].MachinesOperationName);
                cell.CellStyle = OperationNameCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TotalMonthStatistics.Operations[x].Measure);
                cell.CellStyle = MeasureCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Norm));
                cell.CellStyle = NormCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].TariffForVolume));
                cell.CellStyle = TariffForVolumeCS;

                for (int y = 1; y <= DateTime.DaysInMonth(Year, Month); y++)
                {
                    int colIndex = ColumnIndex;
                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.CellStyle = VolumeCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.CellStyle = TimeFactCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = TimePlanCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = CoefPerformancePlanCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = PremiumCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = PremiumCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = PremiumCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = TotalCS;

                    for (int c = 0; c < TotalMonthStatistics.Operations[x].Days.Count; c++)
                    {
                        int DayNumber = Convert.ToInt32(TotalMonthStatistics.Operations[x].Days[c].DayNumber);
                        if (y != DayNumber)
                            continue;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].Volume));
                        cell.CellStyle = VolumeCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].TimeFact));
                        cell.CellStyle = TimeFactCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].TimePlan));
                        cell.CellStyle = TimePlanCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].CoefPerformancePlan));
                        cell.CellStyle = CoefPerformancePlanCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].Premium));
                        cell.CellStyle = PremiumCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].Salary));
                        cell.CellStyle = PremiumCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].Universality));
                        cell.CellStyle = PremiumCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Days[c].Total));
                        cell.CellStyle = TotalCS;
                    }
                }
                RowIndex++;
            }

            cell = sheet1.CreateRow(RowIndex).CreateCell(4);
            cell.SetCellValue("ИТОГО:");
            cell.CellStyle = MeasureCS;
            ColumnIndex = 5;
            for (int c = 0; c < TotalMonthStatistics.WorkDaysSummary.Count; c++)
            {
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].Volume));
                cell.CellStyle = TotalVolumeCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].TimeFact));
                cell.CellStyle = TotalTimeFactCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].TimePlan));
                cell.CellStyle = TotalTimePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].CoefPerformancePlan));
                cell.CellStyle = TotalCoefPerformancePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].Premium));
                cell.CellStyle = TotalPremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].Salary));
                cell.CellStyle = TotalPremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].Universality));
                cell.CellStyle = TotalPremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.WorkDaysSummary[c].Total));
                cell.CellStyle = TotalTotalCS;
            }

            RowIndex++;
            RowIndex++;

            for (int x = 0; x < TotalMonthStatistics.Operations.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TotalMonthStatistics.Operations[x].Index);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TotalMonthStatistics.Operations[x].MachinesOperationName);
                cell.CellStyle = OperationNameCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TotalMonthStatistics.Operations[x].Measure);
                cell.CellStyle = MeasureCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].Norm));
                cell.CellStyle = NormCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].TariffForVolume));
                cell.CellStyle = TariffForVolumeCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummaryVolume));
                cell.CellStyle = VolumeCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummaryTimeFact));
                cell.CellStyle = TimeFactCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummaryTimePlan));
                cell.CellStyle = TimePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummaryCoefPerformancePlan));
                cell.CellStyle = CoefPerformancePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummaryPremium));
                cell.CellStyle = PremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummarySalary));
                cell.CellStyle = PremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummaryUniversality));
                cell.CellStyle = PremiumCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.Operations[x].SummaryTotal));
                cell.CellStyle = TotalCS;
                RowIndex++;
            }

            cell = sheet1.CreateRow(RowIndex).CreateCell(8);
            cell.SetCellValue("ИТОГО:");
            cell.CellStyle = MeasureCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(9);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.SummaryPremium));
            cell.CellStyle = PremiumCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(10);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.SummarySalary));
            cell.CellStyle = PremiumCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(11);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.SummaryUniversality));
            cell.CellStyle = PremiumCS;
            cell = sheet1.CreateRow(RowIndex).CreateCell(12);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatistics.SummaryTotal));
            cell.CellStyle = TotalCS;
        }

        private void WorkStatementToExcel(int Year, int Month)
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Ведомость");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetZoom(9, 10);   // 90 percent magnification

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
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 15 * 256);
            sheet1.SetColumnWidth(9, 15 * 256);
            sheet1.SetColumnWidth(10, 15 * 256);

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont TimesNewRoman11F = hssfworkbook.CreateFont();
            TimesNewRoman11F.FontHeightInPoints = 11;
            TimesNewRoman11F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB11F = hssfworkbook.CreateFont();
            TimesNewRomanB11F.FontHeightInPoints = 11;
            TimesNewRomanB11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB11F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB12F = hssfworkbook.CreateFont();
            TimesNewRomanB12F.FontHeightInPoints = 12;
            TimesNewRomanB12F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB12F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB16F = hssfworkbook.CreateFont();
            TimesNewRomanB16F.FontHeightInPoints = 16;
            TimesNewRomanB16F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB16F.FontName = "Times New Roman";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle TimesNewRomanB12CS = hssfworkbook.CreateCellStyle();
            TimesNewRomanB12CS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            TimesNewRomanB12CS.SetFont(TimesNewRomanB12F);

            HSSFCellStyle TimesNewRomanB16CS = hssfworkbook.CreateCellStyle();
            TimesNewRomanB16CS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            TimesNewRomanB16CS.SetFont(TimesNewRomanB16F);

            HSSFCellStyle CoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            CoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            CoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle EmptyCellCS = hssfworkbook.CreateCellStyle();
            EmptyCellCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.RightBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.TopBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle IndexCS = hssfworkbook.CreateCellStyle();
            IndexCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            IndexCS.BottomBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            IndexCS.LeftBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            IndexCS.RightBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            IndexCS.TopBorderColor = HSSFColor.BLACK.index;
            IndexCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            IndexCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle MeasureCS = hssfworkbook.CreateCellStyle();
            MeasureCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MeasureCS.BottomBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MeasureCS.LeftBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MeasureCS.RightBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            MeasureCS.TopBorderColor = HSSFColor.BLACK.index;
            MeasureCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            MeasureCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle OperationNameCS = hssfworkbook.CreateCellStyle();
            OperationNameCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.BottomBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.LeftBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.RightBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.TopBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            OperationNameCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle NormCS = hssfworkbook.CreateCellStyle();
            NormCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NormCS.BottomBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NormCS.LeftBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            NormCS.RightBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            NormCS.TopBorderColor = HSSFColor.BLACK.index;
            NormCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            NormCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PremiumCS = hssfworkbook.CreateCellStyle();
            PremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            PremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            PremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            PremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            PremiumCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle TotalCS = hssfworkbook.CreateCellStyle();
            TotalCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle TariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TariffForVolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimeFactCS = hssfworkbook.CreateCellStyle();
            TimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimeFactCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimePlanCS = hssfworkbook.CreateCellStyle();
            TimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle VolumeCS = hssfworkbook.CreateCellStyle();
            VolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            VolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            VolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            VolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            VolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            VolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            VolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TotalCoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            TotalCoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalCoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCoefPerformancePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalPremiumCS = hssfworkbook.CreateCellStyle();
            TotalPremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalPremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalPremiumCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTotalCS = hssfworkbook.CreateCellStyle();
            TotalTotalCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            TotalTotalCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalTotalCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTotalCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTotalCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TotalTariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TotalTariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalTariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TotalTariffForVolumeCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimeFactCS = hssfworkbook.CreateCellStyle();
            TotalTimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalTimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimeFactCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimePlanCS = hssfworkbook.CreateCellStyle();
            TotalTimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TotalTimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalVolumeCS = hssfworkbook.CreateCellStyle();
            TotalVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            TotalVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalVolumeCS.SetFont(CalibriBold11F);

            #endregion

            int ColumnIndex = 0;
            int RowIndex = 0;

            HSSFCell cell = null;

            DateTime StartDate = new DateTime(Year, Month, 1);
            DateTime FinishDate = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));
            //string FileName1 = StartDispatchDate.ToString("dd.MM") + "-" + FinishDispatchDate.ToString("dd.MM") + " Отчет";

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "Плановая заработная плата работников  ТПС");
            cell.CellStyle = TimesNewRomanB16CS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 3, "период с " + StartDate.ToString("dd.MM.yyyy") + " по " + FinishDate.ToString("dd.MM.yyyy"));
            cell.CellStyle = TimesNewRomanB12CS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "ФИО");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Коэф-т");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Часы");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "По окладу");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Премия за V");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Универ");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Технолог. дисциплина");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Технич. дисциплина");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Маркетинг");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Итого");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "За час");
            cell.CellStyle = HeaderCS;
            RowIndex++;

            for (int x = 0; x < MonthStatement.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(MonthStatement[x].Worker);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].TariffRatio));
                cell.CellStyle = CoefPerformancePlanCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].SummaryTimeFact));
                cell.CellStyle = TimeFactCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].SummarySalary));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].SummaryPremium));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].SummaryUniversality));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].TechnologicalDiscipline));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].TechnicalDiscipline));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].Marketing));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].SummaryTotal));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(MonthStatement[x].PerHour));
                cell.CellStyle = PremiumCS;

                RowIndex++;
            }

            ColumnIndex = 0;
            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(TotalMonthStatement.Worker);
            cell.CellStyle = MeasureCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue("");
            cell.CellStyle = EmptyCellCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.SummaryTimeFact));
            cell.CellStyle = TimeFactCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.SummarySalary));
            cell.CellStyle = PremiumCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.SummaryPremium));
            cell.CellStyle = PremiumCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.SummaryUniversality));
            cell.CellStyle = PremiumCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.TechnologicalDiscipline));
            cell.CellStyle = PremiumCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.TechnicalDiscipline));
            cell.CellStyle = PremiumCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.Marketing));
            cell.CellStyle = PremiumCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.SummaryTotal));
            cell.CellStyle = PremiumCS;

            cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
            cell.SetCellValue(Convert.ToDouble(TotalMonthStatement.PerHour));
            cell.CellStyle = PremiumCS;

        }
    }

    public class RegistrationDyeingWorkWoman
    {
        decimal MonthlyCoef = 169.3m;

        DataTable DyeingAssignmentBarcodesDT = null;
        DataTable DyeingAssignmentBarcodeDetailsDT = null;
        DataTable MonthsDT = null;
        DataTable OperationsDT = null;
        DataTable PositionsDT = null;
        DataTable ReferenceDT = null;
        DataTable UsersDT = null;
        DataTable UsersTimeWorkDT = null;
        DataTable WorkDaysDT = null;
        DataTable YearsDT = null;

        BindingSource MonthsBS = null;
        BindingSource UsersBS = null;
        BindingSource UsersTimeWorkBS = null;
        BindingSource YearsBS = null;

        List<OperationsMainInfo> Operations;
        List<FotoWorkDay> WorkDaysSummary;

        HSSFWorkbook hssfworkbook;

        public BindingSource MonthsList
        {
            get { return MonthsBS; }
        }

        public BindingSource UsersList
        {
            get { return UsersBS; }
        }

        public BindingSource UsersTimeWorkList
        {
            get { return UsersTimeWorkBS; }
        }

        public BindingSource YearsList
        {
            get { return YearsBS; }
        }

        public struct FotoWorkDay
        {
            public DateTime Day;
            public decimal Volume;
            public decimal TimeFact;
            public decimal TimePlan;
            public decimal CoefPerformancePlan;
            public decimal Premium;
            public int DayNumber;
        }

        public struct OperationsMainInfo
        {
            public decimal Norm;
            public decimal TariffForVolume;
            public int Index;
            public string MachinesOperationName;
            public string Measure;
            public List<FotoWorkDay> Days;
        }

        public struct DayBrigadePremium
        {
            public int Hours;
            public decimal PremiumperHour;
            public decimal Premium;
            public int DayNumber;
        }

        public RegistrationDyeingWorkWoman()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            FillReferenceTable();
            FillTariffForVolume();
        }

        private void Create()
        {
            DyeingAssignmentBarcodesDT = new DataTable();
            DyeingAssignmentBarcodeDetailsDT = new DataTable();
            MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));
            OperationsDT = new DataTable();
            PositionsDT = new DataTable();
            ReferenceDT = new DataTable();
            ReferenceDT.Columns.Add(new DataColumn("PositionID", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("Position", Type.GetType("System.String")));
            ReferenceDT.Columns.Add(new DataColumn("TariffRateOfFirstRank", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TariffRank", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TariffRatio", Type.GetType("System.Decimal")));
            ReferenceDT.Columns.Add(new DataColumn("Salary", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("PremiumPerHour", Type.GetType("System.Decimal")));
            ReferenceDT.Columns.Add(new DataColumn("Premium", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TechnicalDiscipline", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("TechnologicalDiscipline", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("Total", Type.GetType("System.Int32")));
            ReferenceDT.Columns.Add(new DataColumn("PerHour", Type.GetType("System.Int32")));
            UsersDT = new DataTable();
            UsersTimeWorkDT = new DataTable();
            UsersTimeWorkDT.Columns.Add(new DataColumn("UserID", Type.GetType("System.Int32")));
            UsersTimeWorkDT.Columns.Add(new DataColumn("ShortName", Type.GetType("System.String")));
            UsersTimeWorkDT.Columns.Add(new DataColumn("TimeWork", Type.GetType("System.Decimal")));
            WorkDaysDT = new DataTable();
            YearsDT = new DataTable();
            YearsDT.Columns.Add(new DataColumn("YearID", Type.GetType("System.Int32")));
            YearsDT.Columns.Add(new DataColumn("YearName", Type.GetType("System.String")));

            Operations = new List<OperationsMainInfo>();
            WorkDaysSummary = new List<FotoWorkDay>();

            MonthsBS = new BindingSource();
            UsersBS = new BindingSource();
            UsersTimeWorkBS = new BindingSource();
            YearsBS = new BindingSource();
        }

        private void CreateReferenceRow(int PositionID, string Position, int TariffRateOfFirstRank, int TariffRank, int PerHour, decimal TechnicalDiscipline = 0, decimal TechnologicalDiscipline = 0)
        {
            decimal PremiumPerHour = 0;
            decimal Premium = 0;
            decimal Salary = 0;
            decimal TariffRatio = 0;
            decimal Total = 0;

            switch (TariffRank)
            {
                case 1:
                    TariffRatio = 1;
                    break;
                case 2:
                    TariffRatio = 1.16m;
                    break;
                case 3:
                    TariffRatio = 1.35m;
                    break;
                case 4:
                    TariffRatio = 1.57m;
                    break;
                case 5:
                    TariffRatio = 1.73m;
                    break;
                default:
                    break;
            }
            Salary = TariffRateOfFirstRank * TariffRatio;
            if (TechnicalDiscipline != 0)
                TechnicalDiscipline = Salary * TariffRatio / 2;
            if (TechnologicalDiscipline != 0)
                TechnologicalDiscipline = Salary * TariffRatio / 2;
            Premium = PerHour * MonthlyCoef - Salary - TechnicalDiscipline - TechnologicalDiscipline;
            PremiumPerHour = Premium / MonthlyCoef;
            Total = Salary + Premium + TechnicalDiscipline + TechnologicalDiscipline;

            DataRow NewRow = ReferenceDT.NewRow();
            NewRow["PositionID"] = PositionID;
            NewRow["Position"] = Position;
            NewRow["TariffRateOfFirstRank"] = TariffRateOfFirstRank;
            NewRow["TariffRank"] = TariffRank;
            NewRow["TariffRatio"] = TariffRatio;
            NewRow["Salary"] = Salary;
            NewRow["PremiumPerHour"] = PremiumPerHour;
            NewRow["Premium"] = Premium;
            NewRow["TechnicalDiscipline"] = TechnicalDiscipline;
            NewRow["TechnologicalDiscipline"] = TechnologicalDiscipline;
            NewRow["Total"] = Total;
            NewRow["PerHour"] = PerHour;
            ReferenceDT.Rows.Add(NewRow);
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT MachinesOperations.*, Measures.Measure, SubSectors.SubSectorID FROM MachinesOperations
                INNER JOIN Measures ON MachinesOperations.MeasureID = Measures.MeasureID
                INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID AND SubSectors.SubSectorID IN (31)
                ORDER BY MachineID, MachinesOperationName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(OperationsDT);
                OperationsDT.Columns.Add(new DataColumn("TariffForVolume", Type.GetType("System.Decimal")));
            }
            SelectCommand = @"SELECT * FROM Positions WHERE DepartmentID=21 ORDER BY Position, TariffRank";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PositionsDT);
            }
            SelectCommand = @"SELECT UserID, Name, ShortName, PositionID FROM Users WHERE DepartmentID=21";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            SelectCommand = @"SELECT TOP 0 DyeingAssignmentBarcodes.*, DyeingCarts.Square FROM DyeingAssignmentBarcodes
                INNER JOIN DyeingCarts ON DyeingAssignmentBarcodes.DyeingCartID = DyeingCarts.DyeingCartID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentBarcodesDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DyeingAssignmentBarcodeDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DyeingAssignmentBarcodeDetailsDT);
            }

            for (int i = 1; i <= 12; i++)
            {
                DataRow NewRow = MonthsDT.NewRow();
                NewRow["MonthID"] = i;
                NewRow["MonthName"] = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToString();
                MonthsDT.Rows.Add(NewRow);
            }
            DateTime LastDay = new System.DateTime(DateTime.Now.Year + 1, 12, 31);
            for (int i = 2014; i <= LastDay.Year; i++)
            {
                DataRow NewRow = YearsDT.NewRow();
                NewRow["YearID"] = i;
                NewRow["YearName"] = i;
                YearsDT.Rows.Add(NewRow);
            }
        }

        private void Binding()
        {
            MonthsBS.DataSource = MonthsDT;
            UsersBS.DataSource = UsersDT;
            UsersTimeWorkBS.DataSource = UsersTimeWorkDT;
            YearsBS.DataSource = YearsDT;
            UsersTimeWorkBS.Sort = "ShortName";
        }

        private void FillOperationsMainInfo(int Year, int Month)
        {
            for (int i = 0; i < OperationsDT.Rows.Count; i++)
            {
                int MachinesOperationID = Convert.ToInt32(OperationsDT.Rows[i]["MachinesOperationID"]);
                decimal Norm = Convert.ToDecimal(OperationsDT.Rows[i]["Norm"]);
                decimal Square = 0;
                decimal TariffForVolume = Convert.ToDecimal(OperationsDT.Rows[i]["TariffForVolume"]);
                string MachinesOperationName = OperationsDT.Rows[i]["MachinesOperationName"].ToString();
                string Measure = OperationsDT.Rows[i]["Measure"].ToString();

                List<FotoWorkDay> Days = new List<FotoWorkDay>();
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    DateTime Today = new DateTime(Year, Month, j);
                    DataRow[] rows1 = DyeingAssignmentBarcodesDT.Select("StartDate='" + Today.ToString("yyyy-MM-dd") + "' AND MachinesOperationID=" + MachinesOperationID);
                    if (rows1.Count() == 0)
                        continue;
                    decimal Volume = Square;
                    decimal TimeFact = 0;
                    decimal TimePlan = 0;
                    decimal CoefPerformancePlan = 0;
                    decimal Premium = 0;
                    for (int x = 0; x < rows1.Count(); x++)
                    {
                        if (rows1[x]["FinishDateTime"] == DBNull.Value)
                            continue;
                        Square = Convert.ToDecimal(rows1[x]["Square"]);
                        Volume += Square;
                        int DyeingAssignmentBarcodeID = Convert.ToInt32(rows1[x]["DyeingAssignmentBarcodeID"]);
                        DataRow[] rows2 = DyeingAssignmentBarcodeDetailsDT.Select("DyeingAssignmentBarcodeID=" + DyeingAssignmentBarcodeID);
                        if (rows2.Count() == 0)
                            continue;
                        DateTime StartDateTime = Convert.ToDateTime(rows1[x]["StartDateTime"]);
                        DateTime FinishDateTime = Convert.ToDateTime(rows1[x]["FinishDateTime"]);
                        TimeSpan t = FinishDateTime - StartDateTime;
                        TimeFact += Convert.ToDecimal(t.TotalHours);
                        TimePlan += Volume / Norm;
                        if (TimeFact == 0)
                            CoefPerformancePlan = 0;
                        else
                            CoefPerformancePlan += TimePlan / TimeFact;
                        Premium += Volume * TariffForVolume;
                        if (Volume > 0)
                        {

                        }
                    }
                    Days.Add(new FotoWorkDay()
                    {
                        Day = Today,
                        DayNumber = j,
                        Volume = Volume,
                        TimeFact = TimeFact,
                        TimePlan = TimePlan,
                        Premium = Premium,
                        CoefPerformancePlan = CoefPerformancePlan
                    });
                }
                Operations.Add(new OperationsMainInfo()
                {
                    Index = i + 1,
                    MachinesOperationName = MachinesOperationName,
                    Measure = Measure,
                    Norm = Norm,
                    TariffForVolume = TariffForVolume,
                    Days = Days
                });
            }
        }

        private void FillReferenceTable()
        {
            for (int i = 0; i < PositionsDT.Rows.Count; i++)
            {
                int PositionID = Convert.ToInt32(PositionsDT.Rows[i]["PositionID"]);
                string Position = PositionsDT.Rows[i]["Position"].ToString();
                int TariffRateOfFirstRank = Convert.ToInt32(PositionsDT.Rows[i]["TariffRateOfFirstRank"]);
                int TariffRank = Convert.ToInt32(PositionsDT.Rows[i]["TariffRank"]);
                int PerHour = Convert.ToInt32(PositionsDT.Rows[i]["PerHour"]);

                if (PositionID == 50 || PositionID == 54 || PositionID == 45 || PositionID == 53)
                    CreateReferenceRow(PositionID, Position, TariffRateOfFirstRank, TariffRank, PerHour);
                else
                    CreateReferenceRow(PositionID, Position, TariffRateOfFirstRank, TariffRank, PerHour, 1, 1);
            }
        }

        private void FillTariffForVolume()
        {
            for (int i = 0; i < OperationsDT.Rows.Count; i++)
            {
                int PositionID = Convert.ToInt32(OperationsDT.Rows[i]["PositionID"]);
                decimal Norm = Convert.ToDecimal(OperationsDT.Rows[i]["Norm"]);
                decimal PremiumPerHour = 0;

                DataRow[] rows = ReferenceDT.Select("PositionID=" + PositionID);
                if (rows.Count() > 0 && rows[0]["PremiumPerHour"] != DBNull.Value)
                    PremiumPerHour = Convert.ToDecimal(rows[0]["PremiumPerHour"]);

                if (Norm == 0)
                    OperationsDT.Rows[i]["TariffForVolume"] = 0;
                else
                    OperationsDT.Rows[i]["TariffForVolume"] = PremiumPerHour / Norm;
            }
        }

        public void FillUsersTimeWorkTable()
        {
            DateTime DayStartDateTime;
            DateTime DayEndDateTime;
            DateTime DayBreakStartDateTime;
            DateTime DayBreakEndDateTime;
            decimal TimeWork = 0;
            UsersTimeWorkDT.Clear();

            for (int i = 0; i < UsersDT.Rows.Count; i++)
            {
                int UserID = Convert.ToInt32(UsersDT.Rows[i]["UserID"]);
                string ShortName = UsersDT.Rows[i]["ShortName"].ToString();
                DataRow[] rows = WorkDaysDT.Select("UserID=" + UserID);
                TimeWork = 0;
                if (rows.Count() > 0)
                {
                    for (int j = 0; j < rows.Count(); j++)
                    {
                        if (rows[j]["DayStartDateTime"] != DBNull.Value && rows[j]["DayEndDateTime"] != DBNull.Value)
                        {
                            DayStartDateTime = (DateTime)rows[j]["DayStartDateTime"];
                            DayEndDateTime = (DateTime)rows[j]["DayEndDateTime"];
                            TimeSpan t;
                            t = DayEndDateTime.TimeOfDay - DayStartDateTime.TimeOfDay;
                            if (rows[j]["DayBreakStartDateTime"] != DBNull.Value && rows[j]["DayBreakEndDateTime"] != DBNull.Value)
                            {
                                DayBreakStartDateTime = (DateTime)rows[j]["DayBreakStartDateTime"];
                                DayBreakEndDateTime = (DateTime)rows[j]["DayBreakEndDateTime"];
                                t -= DayBreakEndDateTime.TimeOfDay - DayBreakStartDateTime.TimeOfDay;
                            }
                            TimeWork = (decimal)t.TotalHours;
                            TimeWork = Decimal.Round(TimeWork, 1, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                DataRow NewRow = UsersTimeWorkDT.NewRow();
                NewRow["UserID"] = UserID;
                NewRow["ShortName"] = ShortName;
                NewRow["TimeWork"] = TimeWork;
                UsersTimeWorkDT.Rows.Add(NewRow);
            }
            UsersTimeWorkDT.AcceptChanges();
            UsersTimeWorkBS.MoveFirst();
        }

        private void UpdateBarcodes(DateTime StartDate, DateTime FinishDate)
        {
            string SelectCommand = @"SELECT DyeingAssignmentBarcodes.*, CAST(StartDateTime AS Date) AS StartDate, DyeingCarts.Square, infiniu2_catalog.dbo.TechCatalogOperationsDetail.MachinesOperationID FROM DyeingAssignmentBarcodes
                INNER JOIN DyeingCarts ON DyeingAssignmentBarcodes.DyeingCartID = DyeingCarts.DyeingCartID
                INNER JOIN infiniu2_catalog.dbo.TechCatalogOperationsDetail ON DyeingAssignmentBarcodes.TechCatalogOperationsDetailID = infiniu2_catalog.dbo.TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                WHERE CAST(StartDateTime AS Date)>='" + StartDate.ToString("yyyy-MM-dd") + "' AND CAST(FinishDateTime AS Date)<='" + FinishDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentBarcodesDT.Clear();
                DA.Fill(DyeingAssignmentBarcodesDT);
            }
            SelectCommand = @"SELECT * FROM DyeingAssignmentBarcodeDetails
                WHERE DyeingAssignmentBarcodeID IN (SELECT DyeingAssignmentBarcodeID FROM DyeingAssignmentBarcodes
                WHERE CAST(StartDateTime AS Date)>='" + StartDate.ToString("yyyy-MM-dd") + "' AND CAST(FinishDateTime AS Date)<='" + FinishDate.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DyeingAssignmentBarcodeDetailsDT.Clear();
                DA.Fill(DyeingAssignmentBarcodeDetailsDT);
            }
        }

        public void UpdateWorkDays(DateTime DayStartDateTime)
        {
            string SelectCommand = @"SELECT WorkDays.*, CAST(DayStartDateTime AS DATE) AS StartDate FROM WorkDays WHERE CAST(DayStartDateTime AS DATE) = '" + DayStartDateTime.ToString("yyyy-MM-dd") +
                "' AND UserID IN (SELECT UserID FROM infiniu2_users.dbo.Users WHERE DepartmentID = 21)";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                WorkDaysDT.Clear();
                DA.Fill(WorkDaysDT);
            }
        }

        public void SaveWorkDays(DateTime DateTime)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM WorkDays", ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (UsersTimeWorkDT.GetChanges() != null)
                    {
                        DataTable DT = UsersTimeWorkDT.GetChanges();
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            decimal TimeWork = Convert.ToDecimal(DT.Rows[i]["TimeWork"]);
                            int UserID = Convert.ToInt32(DT.Rows[i]["UserID"]);
                            if (TimeWork == 0)
                                continue;
                            TimeSpan t = TimeSpan.FromHours((double)TimeWork);
                            DateTime DayStartDateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 8, 0, 0);
                            DateTime DayEndDateTime = DayStartDateTime.Add(t);

                            DataRow[] rows1 = WorkDaysDT.Select("StartDate='" + DateTime.ToString("yyyy-MM-dd") + "' AND UserID=" + UserID);
                            if (rows1.Count() == 0)
                            {
                                DataRow NewRow = WorkDaysDT.NewRow();
                                NewRow["UserID"] = UserID;
                                NewRow["DayStartDateTime"] = DayStartDateTime;
                                NewRow["DayStartFactDateTime"] = DayStartDateTime;
                                NewRow["DayEndDateTime"] = DayEndDateTime;
                                NewRow["DayEndFactDateTime"] = DayEndDateTime;
                                NewRow["Saved"] = true;
                                WorkDaysDT.Rows.Add(NewRow);
                            }
                            else
                            {
                                rows1[0]["UserID"] = UserID;
                                rows1[0]["DayStartDateTime"] = DayStartDateTime;
                                rows1[0]["DayStartFactDateTime"] = DayStartDateTime;
                                rows1[0]["DayBreakStartDateTime"] = DBNull.Value;
                                rows1[0]["DayBreakStartFactDateTime"] = DBNull.Value;
                                rows1[0]["DayEndDateTime"] = DayEndDateTime;
                                rows1[0]["DayEndFactDateTime"] = DayEndDateTime;
                                rows1[0]["DayBreakEndDateTime"] = DBNull.Value;
                                rows1[0]["DayBreakEndFactDateTime"] = DBNull.Value;
                                rows1[0]["Saved"] = true;
                            }
                        }
                        DA.Update(WorkDaysDT);
                        DT.Dispose();
                    }
                }
            }
        }

        public void CreateReport(int Year, int Month)
        {
            hssfworkbook = new HSSFWorkbook();
            DateTime StartDate = new DateTime(Year, Month, 1);
            DateTime FinishDate = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));
            Operations.Clear();
            WorkDaysSummary.Clear();
            UpdateBarcodes(StartDate, FinishDate);
            FillOperationsMainInfo(Year, Month);

            for (int y = 1; y <= DateTime.DaysInMonth(Year, Month); y++)
            {
                decimal Volume = 0;
                decimal TimeFact = 0;
                decimal TimePlan = 0;
                decimal CoefPerformancePlan = 0;
                decimal Premium = 0;

                DateTime Today = new DateTime(Year, Month, y);
                for (int x = 0; x < Operations.Count; x++)
                {
                    for (int c = 0; c < Operations[x].Days.Count; c++)
                    {
                        int DayNumber = Convert.ToInt32(Operations[x].Days[c].DayNumber);
                        if (y != DayNumber)
                            continue;
                        Volume += Operations[x].Days[c].Volume;
                        TimeFact += Operations[x].Days[c].TimeFact;
                        TimePlan = Operations[x].Days[c].TimePlan;
                        Premium += Operations[x].Days[c].Premium;
                    }
                }
                if (TimeFact == 0)
                    CoefPerformancePlan = 0;
                else
                    CoefPerformancePlan = TimePlan / TimeFact;
                WorkDaysSummary.Add(new FotoWorkDay()
                {
                    Day = Today,
                    DayNumber = y,
                    Volume = Volume,
                    TimeFact = TimeFact,
                    TimePlan = TimePlan,
                    Premium = Premium,
                    CoefPerformancePlan = CoefPerformancePlan
                });
            }
            RatesToExcel();
            Brigade1ToExcel(Year, Month);
            //Brigade2ToExcel(Year, Month);

            string FileName1 = StartDate.ToString("yyyy MMMM") + " Отчет";
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            //string FileName1 = StartDispatchDate.ToString("dd.MM") + "-" + FinishDispatchDate.ToString("dd.MM") + " Отчет";
            //string tempFolder = @"\\192.168.1.6\Public\ТПС\Infinium\Задания на покраску\";
            //string CurrentMonthName = DateTime.Now.ToString("MMMM");
            //tempFolder = Path.Combine(tempFolder, CurrentMonthName);
            if (!(Directory.Exists(tempFolder)))
            {
                Directory.CreateDirectory(tempFolder);
            }
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName1 + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName1 + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        private void RatesToExcel()
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Расценки, нормы");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetZoom(9, 10);   // 90 percent magnification

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 20 * 256);
            sheet1.SetColumnWidth(1, 55 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 10 * 256);

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont TimesNewRoman11F = hssfworkbook.CreateFont();
            TimesNewRoman11F.FontHeightInPoints = 11;
            TimesNewRoman11F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB11F = hssfworkbook.CreateFont();
            TimesNewRomanB11F.FontHeightInPoints = 11;
            TimesNewRomanB11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB11F.FontName = "Times New Roman";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle CoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            CoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            CoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderTop = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle EmptyCellCS = hssfworkbook.CreateCellStyle();
            EmptyCellCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            EmptyCellCS.RightBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.TopBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle IndexCS = hssfworkbook.CreateCellStyle();
            IndexCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            IndexCS.BottomBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            IndexCS.LeftBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            IndexCS.RightBorderColor = HSSFColor.BLACK.index;
            //IndexCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //IndexCS.TopBorderColor = HSSFColor.BLACK.index;
            IndexCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            IndexCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle MeasureCS = hssfworkbook.CreateCellStyle();
            MeasureCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MeasureCS.BottomBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MeasureCS.LeftBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MeasureCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //MeasureCS.TopBorderColor = HSSFColor.BLACK.index;
            MeasureCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            MeasureCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PositionCS = hssfworkbook.CreateCellStyle();
            PositionCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PositionCS.BottomBorderColor = HSSFColor.BLACK.index;
            PositionCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PositionCS.LeftBorderColor = HSSFColor.BLACK.index;
            PositionCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            PositionCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //PositionCS.TopBorderColor = HSSFColor.BLACK.index;
            PositionCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            PositionCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle NormCS = hssfworkbook.CreateCellStyle();
            NormCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NormCS.BottomBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NormCS.LeftBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            NormCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //NormCS.TopBorderColor = HSSFColor.BLACK.index;
            NormCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            NormCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PremiumCS = hssfworkbook.CreateCellStyle();
            PremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ### ##0");
            PremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            PremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            PremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            PremiumCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            //TariffForVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //TariffForVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TariffForVolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TariffRatioCS = hssfworkbook.CreateCellStyle();
            TariffRatioCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TariffRatioCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffRatioCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffRatioCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffRatioCS.RightBorderColor = HSSFColor.BLACK.index;
            TariffRatioCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TariffRatioCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffRatioCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TariffRatioCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle LastRowCS = hssfworkbook.CreateCellStyle();
            LastRowCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            LastRowCS.TopBorderColor = HSSFColor.BLACK.index;

            #endregion

            int ColumnIndex = 0;
            int RowIndex = 0;

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Профессия");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Тарифная ставка 1-ого разряда");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Тарифный разряд");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Тарифный коэф. по ЕТС");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Оклад");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Премия за час");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Премия");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Технич. дисциплина");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "Технич. дисциплина");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "ИТОГО");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex++, "За час");
            cell.CellStyle = HeaderCS;
            RowIndex++;
            ColumnIndex = 0;

            for (int i = 0; i < ReferenceDT.Rows.Count; i++)
            {
                int PositionID = Convert.ToInt32(ReferenceDT.Rows[i]["PositionID"]);
                int TariffRateOfFirstRank = Convert.ToInt32(ReferenceDT.Rows[i]["TariffRateOfFirstRank"]);
                int TariffRank = Convert.ToInt32(ReferenceDT.Rows[i]["TariffRank"]);
                decimal TariffRatio = Convert.ToDecimal(ReferenceDT.Rows[i]["TariffRatio"]);
                int Salary = Convert.ToInt32(ReferenceDT.Rows[i]["Salary"]);
                decimal PremiumPerHour = Convert.ToDecimal(ReferenceDT.Rows[i]["PremiumPerHour"]);
                int Premium = Convert.ToInt32(ReferenceDT.Rows[i]["Premium"]);
                int TechnicalDiscipline = Convert.ToInt32(ReferenceDT.Rows[i]["TechnicalDiscipline"]);
                int TechnologicalDiscipline = Convert.ToInt32(ReferenceDT.Rows[i]["TechnologicalDiscipline"]);
                int Total = Convert.ToInt32(ReferenceDT.Rows[i]["Total"]);
                int PerHour = Convert.ToInt32(ReferenceDT.Rows[i]["PerHour"]);
                string Position = ReferenceDT.Rows[i]["Position"].ToString();
                ColumnIndex = 0;

                if (i == 0)
                {
                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(Position);
                    cell.CellStyle = PositionCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(TariffRateOfFirstRank);
                    cell.CellStyle = PremiumCS;
                }
                else
                {

                    if (i != 0 && PositionID != Convert.ToInt32(ReferenceDT.Rows[i - 1]["PositionID"]) && TariffRateOfFirstRank != Convert.ToInt32(ReferenceDT.Rows[i - 1]["TariffRateOfFirstRank"]))
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.SetCellValue(Position);
                        cell.CellStyle = PositionCS;

                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.SetCellValue(TariffRateOfFirstRank);
                        cell.CellStyle = PremiumCS;
                    }
                    else
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.CellStyle = EmptyCellCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                        cell.CellStyle = EmptyCellCS;
                    }
                }

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TariffRank);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(TariffRatio));
                cell.CellStyle = TariffRatioCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Salary);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(PremiumPerHour));
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Premium);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TechnicalDiscipline);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(TechnologicalDiscipline);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Total);
                cell.CellStyle = PremiumCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(PerHour);
                cell.CellStyle = PremiumCS;

                RowIndex++;
            }
            ColumnIndex = 0;
            for (int i = 0; i < 11; i++)
            {
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.CellStyle = LastRowCS;
            }

            RowIndex++;
            ColumnIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "№");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Операция");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Ед.изм.");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Норма");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Расценка на объем");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            RowIndex++;
            RowIndex++;

            for (int x = 0; x < Operations.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Operations[x].Index);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Operations[x].MachinesOperationName);
                cell.CellStyle = PositionCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Operations[x].Measure);
                cell.CellStyle = MeasureCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(Operations[x].Norm));
                cell.CellStyle = NormCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(Operations[x].TariffForVolume));
                cell.CellStyle = TariffForVolumeCS;

                RowIndex++;
            }
            ColumnIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.CellStyle = LastRowCS;
            }
        }

        private void Brigade1ToExcel(int Year, int Month)
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Авясова");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetZoom(9, 10);   // 90 percent magnification

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 8 * 256);
            sheet1.SetColumnWidth(1, 55 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 10 * 256);

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont TimesNewRoman11F = hssfworkbook.CreateFont();
            TimesNewRoman11F.FontHeightInPoints = 11;
            TimesNewRoman11F.FontName = "Times New Roman";

            HSSFFont TimesNewRomanB11F = hssfworkbook.CreateFont();
            TimesNewRomanB11F.FontHeightInPoints = 11;
            TimesNewRomanB11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TimesNewRomanB11F.FontName = "Times New Roman";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle CoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            CoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            CoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            CoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            CoefPerformancePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle EmptyCellCS = hssfworkbook.CreateCellStyle();
            EmptyCellCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.RightBorderColor = HSSFColor.BLACK.index;
            EmptyCellCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            EmptyCellCS.TopBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle IndexCS = hssfworkbook.CreateCellStyle();
            IndexCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            IndexCS.BottomBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            IndexCS.LeftBorderColor = HSSFColor.BLACK.index;
            IndexCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            IndexCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //IndexCS.TopBorderColor = HSSFColor.BLACK.index;
            IndexCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            IndexCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle MeasureCS = hssfworkbook.CreateCellStyle();
            MeasureCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MeasureCS.BottomBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MeasureCS.LeftBorderColor = HSSFColor.BLACK.index;
            MeasureCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MeasureCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            MeasureCS.TopBorderColor = HSSFColor.BLACK.index;
            MeasureCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            MeasureCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle OperationNameCS = hssfworkbook.CreateCellStyle();
            OperationNameCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.BottomBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.LeftBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OperationNameCS.TopBorderColor = HSSFColor.BLACK.index;
            OperationNameCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            OperationNameCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle NormCS = hssfworkbook.CreateCellStyle();
            NormCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NormCS.BottomBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NormCS.LeftBorderColor = HSSFColor.BLACK.index;
            NormCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            NormCS.RightBorderColor = HSSFColor.BLACK.index;
            //OperationNameCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            NormCS.TopBorderColor = HSSFColor.BLACK.index;
            NormCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            NormCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle PremiumCS = hssfworkbook.CreateCellStyle();
            PremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0");
            PremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            PremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            PremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            PremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            PremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            PremiumCS.SetFont(TimesNewRomanB11F);

            HSSFCellStyle TariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            //TariffForVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //TariffForVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TariffForVolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimeFactCS = hssfworkbook.CreateCellStyle();
            TimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimeFactCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TimePlanCS = hssfworkbook.CreateCellStyle();
            TimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TimePlanCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle VolumeCS = hssfworkbook.CreateCellStyle();
            VolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            VolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            VolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            //VolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //VolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            VolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            VolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            VolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            VolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            VolumeCS.SetFont(TimesNewRoman11F);

            HSSFCellStyle TotalCoefPerformancePlanCS = hssfworkbook.CreateCellStyle();
            TotalCoefPerformancePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalCoefPerformancePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalCoefPerformancePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalCoefPerformancePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCoefPerformancePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalPremiumCS = hssfworkbook.CreateCellStyle();
            TotalPremiumCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0");
            TotalPremiumCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalPremiumCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalPremiumCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalPremiumCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalPremiumCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTariffForVolumeCS = hssfworkbook.CreateCellStyle();
            TotalTariffForVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0[$р.-419];\\-# ##0[$р.-419]");
            TotalTariffForVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotalTariffForVolumeCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            TotalTariffForVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTariffForVolumeCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TotalTariffForVolumeCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimeFactCS = hssfworkbook.CreateCellStyle();
            TotalTimeFactCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.00");
            TotalTimeFactCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimeFactCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimeFactCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimeFactCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalTimePlanCS = hssfworkbook.CreateCellStyle();
            TotalTimePlanCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.0");
            TotalTimePlanCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalTimePlanCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalTimePlanCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalTimePlanCS.SetFont(CalibriBold11F);

            HSSFCellStyle TotalVolumeCS = hssfworkbook.CreateCellStyle();
            TotalVolumeCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("# ##0.000");
            TotalVolumeCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.BottomBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.RightBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotalVolumeCS.TopBorderColor = HSSFColor.BLACK.index;
            TotalVolumeCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalVolumeCS.SetFont(CalibriBold11F);

            #endregion

            int ColumnIndex = 0;
            int RowIndex = 0;

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "№");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Операция");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Ед.изм.");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Норма");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, "Расценка на объем");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex, string.Empty);
            cell.CellStyle = HeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex + 1, ColumnIndex));
            ColumnIndex++;

            for (int x = 1; x <= DateTime.DaysInMonth(Year, Month); x++)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex, x.ToString());
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 1, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 2, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 3, string.Empty);
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), ColumnIndex + 4, string.Empty);
                cell.CellStyle = HeaderCS;
                sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, ColumnIndex, RowIndex, ColumnIndex + 4));

                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "V");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Часы факт.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Часы план.");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "КТУ");
                cell.CellStyle = HeaderCS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), ColumnIndex++, "Премия");
                cell.CellStyle = HeaderCS;
            }

            sheet1.CreateFreezePane(5, 2, 5, 2);
            RowIndex++;
            RowIndex++;

            for (int x = 0; x < Operations.Count; x++)
            {
                ColumnIndex = 0;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Operations[x].Index);
                cell.CellStyle = IndexCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Operations[x].MachinesOperationName);
                cell.CellStyle = OperationNameCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Operations[x].Measure);
                cell.CellStyle = MeasureCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(Operations[x].Norm));
                cell.CellStyle = NormCS;

                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(Operations[x].TariffForVolume));
                cell.CellStyle = TariffForVolumeCS;

                for (int y = 1; y <= DateTime.DaysInMonth(Year, Month); y++)
                {
                    int colIndex = ColumnIndex;
                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.CellStyle = VolumeCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.CellStyle = TimeFactCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = TimePlanCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = CoefPerformancePlanCS;

                    cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                    cell.SetCellValue(0);
                    cell.CellStyle = PremiumCS;

                    for (int c = 0; c < Operations[x].Days.Count; c++)
                    {
                        int DayNumber = Convert.ToInt32(Operations[x].Days[c].DayNumber);
                        if (y != DayNumber)
                            continue;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(Operations[x].Days[c].Volume));
                        cell.CellStyle = VolumeCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(Operations[x].Days[c].TimeFact));
                        cell.CellStyle = TimeFactCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(Operations[x].Days[c].TimePlan));
                        cell.CellStyle = TimePlanCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(Operations[x].Days[c].CoefPerformancePlan));
                        cell.CellStyle = CoefPerformancePlanCS;
                        cell = sheet1.CreateRow(RowIndex).CreateCell(colIndex++);
                        cell.SetCellValue(Convert.ToDouble(Operations[x].Days[c].Premium));
                        cell.CellStyle = PremiumCS;
                    }
                }
                RowIndex++;
            }

            ColumnIndex = 5;
            for (int c = 0; c < WorkDaysSummary.Count; c++)
            {
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(WorkDaysSummary[c].Volume));
                cell.CellStyle = TotalVolumeCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(WorkDaysSummary[c].TimeFact));
                cell.CellStyle = TotalTimeFactCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(WorkDaysSummary[c].TimePlan));
                cell.CellStyle = TotalTimePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(WorkDaysSummary[c].CoefPerformancePlan));
                cell.CellStyle = TotalCoefPerformancePlanCS;
                cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex++);
                cell.SetCellValue(Convert.ToDouble(WorkDaysSummary[c].Premium));
                cell.CellStyle = TotalPremiumCS;
            }
        }
    }

}
