using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Infinium.Modules.TechnologyCatalog
{
    public class TechnologyMaps : IPatinaName, IColorName, IInsetTypeName, IInsetColorName
    {
        int OrderNumber = 1;


        DataTable ColorsDT;
        DataTable CoversDT;
        DataTable PatinaDT;
        DataTable PatinaRALDT;
        DataTable InsetTypesDT;
        DataTable InsetColorsDT;

        DataTable OperationsGroups;

        DataTable CubFurCoversDT;
        DataTable FrontsOrdersDT;
        DataTable FrontsConfigDT;
        DataTable OperationsDetailDT;
        DataTable StoreDetailDT;
        DataTable ToolsDT;
        DataTable TotalOperationsDetailDT;
        DataTable TotalStoreDetailDT;
        DataTable OperationsTermsDT;
        DataTable StoreDetailTermsDT;

        DataTable SummaryMachinesDT;
        DataTable SummaryMaterialsDT;
        DataTable SummaryLaborCostsDT;

        DataTable ToExcelDT;

        ReportToExcel r;

        public TechnologyMaps()
        {
        }

        private void FillExcelTable(DataRow row)
        {
            DataRow NewRow = ToExcelDT.NewRow();
            ToExcelDT.Rows.Add(NewRow);
        }

        public void Initialize()
        {
            Create();

            string SelectCommand = @"SELECT TechCatalogOperationsDetail.*, TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsGroups.GroupName, 
                MachinesOperations.MachinesOperationName, MachinesOperations.Article, MachinesOperations.Norm, MachinesOperations.PreparatoryNorm, MachinesOperations.PositionID, MachinesOperations.Rank, MachinesOperations.MeasureID, 
                Machines.MachineID, Machines.MachineName, SubSectors.SubSectorID, SubSectors.SubSectorName, Sectors.SectorID, Sectors.SectorName, Measure, p.Position FROM TechCatalogOperationsDetail
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID AND GroupNumber=1
                LEFT JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                LEFT JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                LEFT JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
                LEFT JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID
                LEFT JOIN Measures ON MachinesOperations.MeasureID = Measures.MeasureID
                LEFT JOIN infiniu2_light.dbo.Positions AS p ON MachinesOperations.PositionID = p.PositionID ORDER BY SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsDetailDT.Clear();
                DA.Fill(OperationsDetailDT);
                TotalOperationsDetailDT = OperationsDetailDT.Clone();
                ToExcelDT = OperationsDetailDT.Clone();
                TotalOperationsDetailDT.Columns.Add(new DataColumn("Done", Type.GetType("System.Boolean")));
                TotalOperationsDetailDT.Columns.Add(new DataColumn("OrderNumber", Type.GetType("System.Int32")));
                TotalOperationsDetailDT.Columns.Add(new DataColumn("PrevOperationsDetailID", Type.GetType("System.Int32")));
                TotalOperationsDetailDT.Columns.Add(new DataColumn("PrevStoreDetailID", Type.GetType("System.Int32")));
                TotalOperationsDetailDT.Columns.Add(new DataColumn("TotalCount", Type.GetType("System.Decimal")));
                TotalOperationsDetailDT.Columns.Add(new DataColumn("OperationsDetail", Type.GetType("System.String")));
            }
            SelectCommand = @"SELECT Sectors.SectorName, SubSectors.SubSectorName, Machines.MachineName, TechCatalogOperationsGroups.GroupName, MachinesOperations.MachinesOperationName, MachinesOperations.Article, TechCatalogStoreDetail.*, TechStore.TechStoreName, TechStore.Notes, Measure, 
                MachinesOperations.CabFurDocTypeID, CabFurnitureDocumentTypes.DocName, CabFurnitureDocumentTypes.AssignmentID FROM TechCatalogStoreDetail
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogStoreDetail.TechCatalogOperationsDetailID = TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                INNER JOIN infiniu2_storage.dbo.CabFurnitureDocumentTypes AS CabFurnitureDocumentTypes ON MachinesOperations.CabFurDocTypeID = CabFurnitureDocumentTypes.CabFurDocTypeID
                INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
                INNER JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID = TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID = Measures.MeasureID ORDER BY GroupA, GroupB, GroupC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                StoreDetailDT.Clear();
                DA.Fill(StoreDetailDT);
                StoreDetailDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
                StoreDetailDT.Columns.Add(new DataColumn("Condition", Type.GetType("System.String")));
                StoreDetailDT.Columns.Add(new DataColumn("TotalCount", Type.GetType("System.Decimal")));
                TotalStoreDetailDT = StoreDetailDT.Clone();
            }
            SelectCommand = @"SELECT TOP 0 * FROM TechCatalogTools";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                ToolsDT.Clear();
                DA.Fill(ToolsDT);
            }
            SelectCommand = @"SELECT * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsTermsDT.Clear();
                DA.Fill(OperationsTermsDT);
            }
            SelectCommand = @"SELECT * FROM TechCatalogStoreDetailTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                StoreDetailTermsDT.Clear();
                DA.Fill(StoreDetailTermsDT);
            }

            SelectCommand = @"SELECT * FROM TechCatalogOperationsGroups";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsGroups.Clear();
                DA.Fill(OperationsGroups);
            }

            SelectCommand = @"SELECT * FROM CabFurnitureCovers";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                CubFurCoversDT.Clear();
                DA.Fill(CubFurCoversDT);
            }

            GetColorsDT();
            GetCoversDT();
            GetInsetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDT);
            }
            foreach (DataRow item in PatinaRALDT.Rows)
            {
                DataRow NewRow = PatinaDT.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDT.Rows.Add(NewRow);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes ORDER BY InsetTypeName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDT);
                {
                    DataRow NewRow = InsetTypesDT.NewRow();
                    NewRow["InsetTypeID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetTypeName"] = "на выбор";
                    NewRow["MeasureID"] = 1;
                    InsetTypesDT.Rows.Add(NewRow);
                }
            }
        }

        private void Create()
        {
            SummaryMaterialsDT = new DataTable();
            SummaryMaterialsDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            SummaryMaterialsDT.Columns.Add(new DataColumn("TechStoreID", Type.GetType("System.Int32")));
            SummaryMaterialsDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            SummaryMaterialsDT.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            SummaryMaterialsDT.Columns.Add(new DataColumn("Condition", Type.GetType("System.String")));

            SummaryMachinesDT = new DataTable();
            SummaryMachinesDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            SummaryMachinesDT.Columns.Add(new DataColumn("MachineID", Type.GetType("System.Int32")));
            SummaryMachinesDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));

            SummaryLaborCostsDT = new DataTable();
            SummaryLaborCostsDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            SummaryLaborCostsDT.Columns.Add(new DataColumn("PositionID", Type.GetType("System.Int32")));
            SummaryLaborCostsDT.Columns.Add(new DataColumn("Rank", Type.GetType("System.Int32")));
            SummaryLaborCostsDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));

            PatinaDT = new DataTable();
            PatinaRALDT = new DataTable();
            InsetTypesDT = new DataTable();

            OperationsGroups = new DataTable();

            CubFurCoversDT = new DataTable();
            FrontsOrdersDT = new DataTable();
            FrontsConfigDT = new DataTable();
            OperationsDetailDT = new DataTable();
            StoreDetailDT = new DataTable();
            OperationsTermsDT = new DataTable();
            StoreDetailTermsDT = new DataTable();
            ToolsDT = new DataTable();
        }

        public void MainFunction(int TechStoreID)
        {
            OrderNumber = 1;
            SummaryMaterialsDT.Clear();
            SummaryMachinesDT.Clear();
            SummaryLaborCostsDT.Clear();
            TotalOperationsDetailDT.Clear();
            TotalStoreDetailDT.Clear();
            ToolsDT.Clear();

            string SelectCommand = @"SELECT TechCatalogOperationsDetail.*, TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsGroups.GroupName, 
                MachinesOperations.MachinesOperationName, MachinesOperations.Article, MachinesOperations.Norm, MachinesOperations.PositionID, MachinesOperations.Rank, MachinesOperations.PreparatoryNorm, MachinesOperations.MeasureID, 
                Machines.MachineID, Machines.MachineName, SubSectors.SubSectorID, SubSectors.SubSectorName, Sectors.SectorID, Sectors.SectorName, Measure, p.Position FROM TechCatalogOperationsDetail
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID AND GroupNumber=1
                LEFT JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                LEFT JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                LEFT JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
                LEFT JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID
                LEFT JOIN Measures ON MachinesOperations.MeasureID = Measures.MeasureID
                LEFT JOIN infiniu2_light.dbo.Positions AS p ON MachinesOperations.PositionID = p.PositionID
                WHERE TechStoreID=" + TechStoreID + " ORDER BY SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = TotalOperationsDetailDT.NewRow();
                        NewRow.ItemArray = DT.Rows[i].ItemArray;
                        NewRow["Done"] = false;
                        NewRow["OrderNumber"] = OrderNumber++;
                        TotalOperationsDetailDT.Rows.Add(NewRow);
                    }
                }
            }

            SelectCommand = @"SELECT TechCatalogTools.*, Tools.ToolsName FROM TechCatalogTools
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogTools.TechCatalogOperationsDetailID = TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                INNER JOIN Tools ON TechCatalogTools.ToolsID = Tools.ToolsID
                WHERE TechStoreID=" + TechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ToolsDT);
            }

            r = new ReportToExcel();
            r.WriteSheetMainInfo();

            bool b = true;
            while (b)
            {
                for (int i = 0; i < TotalOperationsDetailDT.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(TotalOperationsDetailDT.Rows[i]["Done"]) == true)
                        continue;
                    GetStoreDetail(TotalOperationsDetailDT.Rows[i]);
                }
                DataRow[] rows = TotalOperationsDetailDT.Select("Done=0");
                if (rows.Count() == 0)
                    b = false;
            }

            DataTable tempDT = new DataTable();
            using (DataView DV = new DataView(TotalOperationsDetailDT))
            {
                DV.Sort = "OrderNumber DESC";
                tempDT = DV.ToTable(true, new string[] { "OrderNumber", "TechStoreID" });
            }

            foreach (DataRow item in tempDT.Rows)
            {
                TechStoreID = Convert.ToInt32(item["TechStoreID"]);
                string TechStoreName = GetTechStoreName(TechStoreID);
                r.WriteTechStoreName(TechStoreName);
                DataRow[] tRows = TotalOperationsDetailDT.Select("OrderNumber=" + Convert.ToInt32(item["OrderNumber"]));
                foreach (DataRow row in tRows)
                {
                    FillExcelTable(row);

                    decimal TotalCount = 1;
                    if (row["TotalCount"] != DBNull.Value)
                        TotalCount = Convert.ToDecimal(row["TotalCount"]);
                    int TechCatalogOperationsDetailID = Convert.ToInt32(row["TechCatalogOperationsDetailID"]);
                    DataRow[] sRows = StoreDetailDT.Select("TechCatalogOperationsDetailID=" + Convert.ToInt32(row["TechCatalogOperationsDetailID"]));
                    if (sRows.Count() > 0)
                    {
                        foreach (DataRow sRow in sRows)
                        {
                            string sTechStoreName = sRow["TechStoreName"].ToString();
                            sRow["Name"] = sTechStoreName + CheckStoreDetailConditions(Convert.ToInt32(sRow["TechCatalogStoreDetailID"]));
                            sRow["Condition"] = CheckStoreDetailConditions(Convert.ToInt32(sRow["TechCatalogStoreDetailID"]));
                            DataRow NewRow = TotalStoreDetailDT.NewRow();
                            NewRow.ItemArray = sRow.ItemArray;
                            NewRow["TotalCount"] = TotalCount;
                            TotalStoreDetailDT.Rows.Add(NewRow);
                        }
                    }

                    DataRow[] rows = ToolsDT.Select("TechCatalogOperationsDetailID=" + Convert.ToInt32(row["TechCatalogOperationsDetailID"]));
                    string ToolsName = string.Empty;
                    if (rows.Count() > 0)
                    {
                        foreach (DataRow sRow in rows)
                        {
                            string Tools = sRow["ToolsName"].ToString() + " - " + sRow["Count"].ToString() + " шт.";
                            if (Tools.Length > 0)
                            {
                                if (ToolsName.Length > 0)
                                    ToolsName += "\r\n";
                                ToolsName += Tools;
                            }
                        }
                    }

                    double WorkTimeNorm = -1;
                    if (row["Norm"] != DBNull.Value)
                        WorkTimeNorm = Convert.ToDouble(row["Norm"]);
                    r.WriteToExcel(row["MachinesOperationName"].ToString(), row["MachineName"].ToString(), ToolsName, row["Position"].ToString(), row["Article"].ToString(), sRows, WorkTimeNorm);
                }
            }
            SummaryMaterials();
            SummaryMachines();
            SummaryLaborCosts();
            r.WriteSummaryMaterials(SummaryMaterialsDT);
            r.WriteSummaryMachines(SummaryMachinesDT);
            r.WriteSummaryLaborCosts(SummaryLaborCostsDT);
            r.SaveFile("новое задание", true);
        }


        private void SummaryMaterials()
        {
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(TotalStoreDetailDT))
            {
                DT = DV.ToTable(true, new string[] { "TechStoreID", "Notes", "Condition" });
            }

            for (int j = 0; j < DT.Rows.Count; j++)
            {
                int TechStoreID = Convert.ToInt32(DT.Rows[j]["TechStoreID"]);
                string Conditions = DT.Rows[j]["Condition"].ToString();
                string ConditionsFilter = " AND Condition=''";
                if (Conditions.Length > 0)
                    ConditionsFilter = " AND Condition='" + Conditions + "'";
                DataRow[] Srows = TotalStoreDetailDT.Select("IsHalfStuff1=1 AND TechStoreID=" + Convert.ToInt32(DT.Rows[j]["TechStoreID"]) + ConditionsFilter);
                if (Srows.Count() == 0)
                    continue;

                decimal Count = 0;

                foreach (DataRow item in Srows)
                    if (item["Count"] != DBNull.Value)
                    {
                        decimal TotalCount = 1;
                        if (item["TotalCount"] != DBNull.Value)
                            TotalCount = Convert.ToDecimal(item["TotalCount"]);
                        Count += Convert.ToDecimal(item["Count"]) * TotalCount;
                    }
                DataRow[] rows = SummaryMaterialsDT.Select("TechStoreID=" + TechStoreID + ConditionsFilter);
                if (rows.Count() == 0)
                {
                    DataRow NewRow = SummaryMaterialsDT.NewRow();
                    NewRow["TechStoreID"] = TechStoreID;
                    NewRow["Count"] = Count;
                    NewRow["Name"] = Srows[0]["TechStoreName"].ToString() + " " + Srows[0]["Notes"].ToString();
                    NewRow["Measure"] = Srows[0]["Measure"].ToString();
                    NewRow["Condition"] = Srows[0]["Condition"].ToString();
                    SummaryMaterialsDT.Rows.Add(NewRow);
                }
                else
                {
                    rows[0]["Count"] = Convert.ToDecimal(rows[0]["Count"]) + Count;
                }
            }

            using (DataView DV = new DataView(SummaryMaterialsDT.Copy()))
            {
                DV.Sort = "Name, Count";
                SummaryMaterialsDT.Clear();
                SummaryMaterialsDT = DV.ToTable();
            }
        }

        private void SummaryMachines()
        {
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(TotalOperationsDetailDT))
            {
                DT = DV.ToTable(true, new string[] { "MachineID" });
            }

            for (int j = 0; j < DT.Rows.Count; j++)
            {
                DataRow[] Srows = TotalOperationsDetailDT.Select("MachineID=" + Convert.ToInt32(DT.Rows[j]["MachineID"]));
                if (Srows.Count() == 0)
                    continue;

                decimal Count = 0;
                int MachineID = Convert.ToInt32(DT.Rows[j]["MachineID"]);

                foreach (DataRow item in Srows)
                    if (item["Norm"] != DBNull.Value)
                    {
                        decimal TotalCount = 1;
                        if (item["TotalCount"] != DBNull.Value)
                            TotalCount = Convert.ToDecimal(item["TotalCount"]);
                        Count += Convert.ToDecimal(item["Norm"]) * TotalCount;
                    }
                DataRow[] rows = SummaryMachinesDT.Select("MachineID=" + MachineID);
                if (rows.Count() == 0)
                {
                    DataRow NewRow = SummaryMachinesDT.NewRow();
                    NewRow["MachineID"] = MachineID;
                    NewRow["Count"] = Count;
                    NewRow["Name"] = Srows[0]["MachineName"].ToString();
                    SummaryMachinesDT.Rows.Add(NewRow);
                }
                else
                {
                    rows[0]["Count"] = Convert.ToDecimal(rows[0]["Count"]) + Count;
                }
            }

            using (DataView DV = new DataView(SummaryMachinesDT.Copy()))
            {
                DV.Sort = "Name, Count";
                SummaryMachinesDT.Clear();
                SummaryMachinesDT = DV.ToTable();
            }
        }

        private void SummaryLaborCosts()
        {
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(TotalOperationsDetailDT))
            {
                DT = DV.ToTable(true, new string[] { "PositionID", "Rank" });
            }

            for (int j = 0; j < DT.Rows.Count; j++)
            {
                DataRow[] Srows = TotalOperationsDetailDT.Select("PositionID=" + Convert.ToInt32(DT.Rows[j]["PositionID"]) + " AND Rank=" + Convert.ToInt32(DT.Rows[j]["Rank"]));
                if (Srows.Count() == 0)
                    continue;

                decimal Count = 0;
                int PositionID = Convert.ToInt32(DT.Rows[j]["PositionID"]);
                int Rank = Convert.ToInt32(DT.Rows[j]["Rank"]);

                foreach (DataRow item in Srows)
                    if (item["Norm"] != DBNull.Value)
                    {
                        decimal TotalCount = 1;
                        if (item["TotalCount"] != DBNull.Value)
                            TotalCount = Convert.ToDecimal(item["TotalCount"]);
                        Count += Convert.ToDecimal(item["Norm"]) * TotalCount;
                    }
                DataRow[] rows = SummaryLaborCostsDT.Select("PositionID=" + PositionID + " AND Rank = " + Rank);
                if (rows.Count() == 0)
                {
                    DataRow NewRow = SummaryLaborCostsDT.NewRow();
                    NewRow["PositionID"] = PositionID;
                    NewRow["Rank"] = Rank;
                    NewRow["Count"] = Count;
                    NewRow["Name"] = Srows[0]["Position"].ToString();
                    SummaryLaborCostsDT.Rows.Add(NewRow);
                }
                else
                {
                    rows[0]["Count"] = Convert.ToDecimal(rows[0]["Count"]) + Count;
                }
            }

            using (DataView DV = new DataView(SummaryLaborCostsDT.Copy()))
            {
                DV.Sort = "Name, Rank, Count";
                SummaryLaborCostsDT.Clear();
                SummaryLaborCostsDT = DV.ToTable();
            }
        }

        public bool GetOperationsDetail(int PrevStoreDetailID, int PrevOperationsDetailID, int TechStoreID, decimal TotalCount, bool BreakChain)
        {
            if (BreakChain)
                return false;
            DataRow[] rows = OperationsDetailDT.Select("TechStoreID=" + TechStoreID);
            if (rows.Count() == 0)
            {
            }
            else
            {
                foreach (DataRow item in rows)
                {
                    int TechCatalogOperationsDetailID = Convert.ToInt32(item["TechCatalogOperationsDetailID"]);
                    if (AlreadyAdded(TechCatalogOperationsDetailID))
                        continue;
                    DataRow NewRow = TotalOperationsDetailDT.NewRow();
                    string MachinesOperationName = item["MachinesOperationName"].ToString();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["MachinesOperationName"] = item["MachinesOperationName"].ToString() + CheckOperationsConditions(TechCatalogOperationsDetailID);
                    if (StoreDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID).Count() > 0)
                        NewRow["Done"] = false;
                    else
                        NewRow["Done"] = true;
                    NewRow["TotalCount"] = TotalCount;
                    NewRow["OrderNumber"] = OrderNumber;
                    NewRow["PrevStoreDetailID"] = PrevStoreDetailID;
                    NewRow["PrevOperationsDetailID"] = PrevOperationsDetailID;
                    TotalOperationsDetailDT.Rows.Add(NewRow);
                }
                OrderNumber++;
            }

            return rows.Count() > 0;
        }

        public bool GetStoreDetail(DataRow Row)
        {
            int TechCatalogOperationsDetailID = Convert.ToInt32(Row["TechCatalogOperationsDetailID"]);
            decimal PrevTotalCount = 1;
            if (Row["TotalCount"] != DBNull.Value)
                PrevTotalCount = Convert.ToDecimal(Row["TotalCount"]);
            DataRow[] rows = StoreDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            if (rows.Count() == 0)
            {
                if (!AlreadyAdded(TechCatalogOperationsDetailID))
                {
                    DataRow NewRow = TotalOperationsDetailDT.NewRow();
                    NewRow.ItemArray = Row.ItemArray;
                    string MachinesOperationName = Row["MachinesOperationName"].ToString();
                    Row["MachinesOperationName"] = Row["MachinesOperationName"].ToString() + CheckOperationsConditions(TechCatalogOperationsDetailID);
                    NewRow["Done"] = true;
                    TotalOperationsDetailDT.Rows.Add(NewRow);
                }
            }
            else
            {
                foreach (DataRow item in rows)
                {
                    decimal TotalCount = 1;
                    if (item["Count"] != DBNull.Value)
                        TotalCount = Convert.ToDecimal(item["Count"]);
                    TotalCount *= PrevTotalCount;
                    int TechStoreID = Convert.ToInt32(item["TechStoreID"]);
                    int TechCatalogStoreDetailID = Convert.ToInt32(item["TechCatalogStoreDetailID"]);
                    bool BreakChain = Convert.ToBoolean(item["BreakChain"]);
                    //DataRow NewRow = TotalStoreDetailDT.NewRow();
                    //NewRow.ItemArray = item.ItemArray;
                    //TotalStoreDetailDT.Rows.Add(NewRow);
                    GetOperationsDetail(TechCatalogStoreDetailID, TechCatalogOperationsDetailID, TechStoreID, TotalCount, BreakChain);
                }
            }
            //double WorkTimeNorm = -1;
            //if (Row["Norm"] != DBNull.Value)
            //    WorkTimeNorm = Convert.ToDouble(Row["Norm"]);
            //r.Thirteen(Row["MachinesOperationName"].ToString(), Row["MachineName"].ToString(), Row["Position"].ToString(), rows, WorkTimeNorm);
            Row["Done"] = true;
            return rows.Count() > 0;
        }

        private bool AlreadyAdded(int TechCatalogOperationsDetailID)
        {
            return TotalOperationsDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID).Count() > 0;
        }

        public string GetTechStoreName(int TechStoreID)
        {
            string TechStoreName = string.Empty;
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore WHERE TechStoreID=" + TechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        TechStoreName = DT.Rows[0]["TechStoreName"].ToString();
                }
            }
            return TechStoreName;
        }

        public string GetColorName(int ColorID)
        {
            DataRow[] rows = ColorsDT.Select("ColorID = " + ColorID);
            if (rows.Count() > 0)
            {
                return rows[0]["ColorName"].ToString();
            }
            return string.Empty;
        }

        public string GetCoverName(int CoverID)
        {
            DataRow[] rows = CoversDT.Select("CoverID = " + CoverID);
            if (rows.Count() > 0)
            {
                return rows[0]["CoverName"].ToString();
            }
            return string.Empty;
        }

        public string GetPatinaName(int PatinaID)
        {
            DataRow[] rows = PatinaDT.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
            {
                return rows[0]["PatinaName"].ToString();
            }
            return string.Empty;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            DataRow[] rows = InsetTypesDT.Select("InsetTypeID = " + InsetTypeID);
            if (rows.Count() > 0)
            {
                return rows[0]["InsetTypeName"].ToString();
            }
            return string.Empty;
        }

        public string GetInsetColorName(int ColorID)
        {
            DataRow[] rows = InsetColorsDT.Select("InsetColorID = " + ColorID);
            if (rows.Count() > 0)
            {
                return rows[0]["InsetColorName"].ToString();
            }
            return string.Empty;
        }

        private void GetColorsDT()
        {
            ColorsDT = new DataTable();
            ColorsDT.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("GroupID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["GroupID"] = 1;
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                        NewRow["GroupID"] = Convert.ToInt64(DT.Rows[i]["GroupID"]);
                        NewRow["ColorName"] = GetColorName(Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = -1;
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "-";
                ColorsDT.Rows.Add(NewRow);
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = 0;
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "на выбор";
                ColorsDT.Rows.Add(NewRow);
            }
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(ColorsDT))
            {
                DV.Sort = "GroupID, ColorName";
                Table = DV.ToTable();
            }
            ColorsDT.Clear();
            ColorsDT = Table.Copy();
        }

        private void GetCoversDT()
        {
            CoversDT = new DataTable();
            CoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            CoversDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreName FROM TechStore" +
                " WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)" +
                " ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    DataRow EmptyRow = CoversDT.NewRow();
                    EmptyRow["CoverID"] = -1;
                    EmptyRow["CoverName"] = "-";
                    CoversDT.Rows.Add(EmptyRow);

                    DataRow ChoiceRow = CoversDT.NewRow();
                    ChoiceRow["CoverID"] = 0;
                    ChoiceRow["CoverName"] = "на выбор";
                    CoversDT.Rows.Add(ChoiceRow);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = CoversDT.NewRow();
                        NewRow["CoverID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["CoverName"] = DT.Rows[i]["TechStoreName"].ToString();
                        CoversDT.Rows.Add(NewRow);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = CoversDT.NewRow();
                        NewRow["CoverID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                        NewRow["CoverName"] = GetColorName(Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        CoversDT.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDT);
                {
                    DataRow NewRow = InsetColorsDT.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDT.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDT.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDT.Rows.Add(NewRow);
                }

            }

        }

        private string CheckOperationsConditions(int TechCatalogOperationsDetailID)
        {
            string stringTerm = string.Empty;
            DataRow[] rows = OperationsTermsDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            foreach (DataRow row in rows)
            {
                string Parameter = row["Parameter"].ToString();
                switch (Parameter)
                {
                    case "ColorID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Цвет" + row["MathSymbol"].ToString() + GetColorName(Convert.ToInt32(row["Term"]));
                        break;
                    case "InsetTypeID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Вставка" + row["MathSymbol"].ToString() + GetInsetTypeName(Convert.ToInt32(row["Term"]));
                        break;
                    case "InsetColorID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Цвет вставки" + row["MathSymbol"].ToString() + GetInsetColorName(Convert.ToInt32(row["Term"]));
                        break;
                    case "CoverID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Облицовка" + row["MathSymbol"].ToString() + GetCoverName(Convert.ToInt32(row["Term"]));
                        break;
                    case "PatinaID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Патина" + row["MathSymbol"].ToString() + GetPatinaName(Convert.ToInt32(row["Term"]));
                        break;
                    case "Diameter":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Диаметр" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Thickness":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Толщина" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Length":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Длина" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Height":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Высота" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Width":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Ширина" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Admission":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Допуск" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "InsetHeightAdmission":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Допуск по высоте вставки" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "InsetWidthAdmission":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Допуск по ширине вставки" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Capacity":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Объем" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Weight":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Вес" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                }
            }
            if (stringTerm.Length > 0)
                stringTerm = "\r\n(" + stringTerm + ")";
            return stringTerm;
        }

        private string CheckStoreDetailConditions(int TechCatalogStoreDetailID)
        {
            string stringTerm = string.Empty;
            DataRow[] rows = StoreDetailTermsDT.Select("TechCatalogStoreDetailID=" + TechCatalogStoreDetailID);
            foreach (DataRow row in rows)
            {
                string Parameter = row["Parameter"].ToString();
                switch (Parameter)
                {
                    case "ColorID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Цвет" + row["MathSymbol"].ToString() + GetColorName(Convert.ToInt32(row["Term"]));
                        break;
                    case "InsetTypeID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Вставка" + row["MathSymbol"].ToString() + GetInsetTypeName(Convert.ToInt32(row["Term"]));
                        break;
                    case "InsetColorID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Цвет вставки" + row["MathSymbol"].ToString() + GetInsetColorName(Convert.ToInt32(row["Term"]));
                        break;
                    case "CoverID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Облицовка" + row["MathSymbol"].ToString() + GetCoverName(Convert.ToInt32(row["Term"]));
                        break;
                    case "PatinaID":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Патина" + row["MathSymbol"].ToString() + GetPatinaName(Convert.ToInt32(row["Term"]));
                        break;
                    case "Diameter":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Диаметр" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Thickness":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Толщина" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Length":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Длина" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Height":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Высота" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Width":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Ширина" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Admission":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Допуск" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "InsetHeightAdmission":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Допуск по высоте вставки" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "InsetWidthAdmission":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Допуск по ширине вставки" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Capacity":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Объем" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                    case "Weight":
                        if (stringTerm.Length > 0)
                            stringTerm += " " + row["LogicOperation"].ToString() + "\r\n";
                        stringTerm += "Вес" + row["MathSymbol"].ToString() + row["Term"].ToString();
                        break;
                }
            }
            if (stringTerm.Length > 0)
                stringTerm = "\r\n(" + stringTerm + ")";
            return stringTerm;
        }
    }

    public class ReportToExcel
    {
        int pos13 = 0;

        HSSFWorkbook hssfworkbook;
        HSSFSheet sheet13;
        HSSFSheet sheet14;
        HSSFSheet sheet15;
        HSSFSheet sheet16;

        HSSFFont fConfirm;
        HSSFFont fHeader;
        HSSFFont fColumnName;
        HSSFFont fMainContent;
        HSSFFont fTotalInfo;
        HSSFFont fSignatures;

        HSSFCellStyle csConfirm;
        HSSFCellStyle csHeader;
        HSSFCellStyle csColumnName;
        HSSFCellStyle csMainContent;
        HSSFCellStyle csMainContentRot;
        HSSFCellStyle csMainContentDec;
        HSSFCellStyle csMainContentWrap;
        HSSFCellStyle csTotalInfo;

        public ReportToExcel()
        {
            hssfworkbook = new HSSFWorkbook();

            sheet13 = hssfworkbook.CreateSheet("sheet1");
            sheet13.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet13.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet13.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet13.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet13.SetMargin(HSSFSheet.BottomMargin, (double).20);
            int DisplayIndex = 0;
            sheet13.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 25 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 30 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet13.SetColumnWidth(DisplayIndex++, 10 * 256);


            sheet14 = hssfworkbook.CreateSheet("sheet2");
            sheet14.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet14.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet14.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet14.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet14.SetMargin(HSSFSheet.BottomMargin, (double).20);
            DisplayIndex = 0;
            sheet14.SetColumnWidth(DisplayIndex++, 35 * 256);
            sheet14.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet14.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet14.SetColumnWidth(DisplayIndex++, 20 * 256);

            sheet15 = hssfworkbook.CreateSheet("sheet3");
            sheet15.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet15.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet15.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet15.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet15.SetMargin(HSSFSheet.BottomMargin, (double).20);
            DisplayIndex = 0;
            sheet15.SetColumnWidth(DisplayIndex++, 35 * 256);
            sheet15.SetColumnWidth(DisplayIndex++, 20 * 256);

            sheet16 = hssfworkbook.CreateSheet("sheet4");
            sheet16.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet16.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet16.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet16.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet16.SetMargin(HSSFSheet.BottomMargin, (double).20);
            DisplayIndex = 0;
            sheet16.SetColumnWidth(DisplayIndex++, 35 * 256);
            sheet16.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet16.SetColumnWidth(DisplayIndex++, 20 * 256);

            CreateFonts();
            CreateCellStyles();
        }

        private void CreateFonts()
        {
            fConfirm = hssfworkbook.CreateFont();
            fConfirm.FontHeightInPoints = 12;
            fConfirm.FontName = "Calibri";

            fHeader = hssfworkbook.CreateFont();
            fHeader.FontHeightInPoints = 12;
            fHeader.Boldweight = 12 * 256;
            fHeader.FontName = "Calibri";

            fColumnName = hssfworkbook.CreateFont();
            fColumnName.FontHeightInPoints = 12;
            fColumnName.FontName = "Calibri";

            fMainContent = hssfworkbook.CreateFont();
            fMainContent.FontHeightInPoints = 11;
            fMainContent.FontName = "Calibri";

            fTotalInfo = hssfworkbook.CreateFont();
            fTotalInfo.FontHeightInPoints = 12;
            fTotalInfo.Boldweight = 12 * 256;
            fTotalInfo.FontName = "Calibri";

            fSignatures = hssfworkbook.CreateFont();
            fSignatures.FontHeightInPoints = 12;
            fSignatures.Boldweight = 12 * 256;
            fSignatures.IsItalic = true;
            fSignatures.FontName = "Calibri";
        }

        private void CreateCellStyles()
        {
            csConfirm = hssfworkbook.CreateCellStyle();
            csConfirm.SetFont(fConfirm);

            csHeader = hssfworkbook.CreateCellStyle();
            csHeader.SetFont(fHeader);
            csColumnName = hssfworkbook.CreateCellStyle();
            csColumnName.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csColumnName.BottomBorderColor = HSSFColor.BLACK.index;
            csColumnName.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csColumnName.LeftBorderColor = HSSFColor.BLACK.index;
            csColumnName.BorderRight = HSSFCellStyle.BORDER_THIN;
            csColumnName.RightBorderColor = HSSFColor.BLACK.index;
            csColumnName.BorderTop = HSSFCellStyle.BORDER_THIN;
            csColumnName.TopBorderColor = HSSFColor.BLACK.index;
            csColumnName.Alignment = HSSFCellStyle.ALIGN_LEFT;
            csColumnName.WrapText = true;
            csColumnName.SetFont(fColumnName);

            csMainContent = hssfworkbook.CreateCellStyle();
            csMainContent.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csMainContent.BottomBorderColor = HSSFColor.BLACK.index;
            csMainContent.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csMainContent.LeftBorderColor = HSSFColor.BLACK.index;
            csMainContent.BorderRight = HSSFCellStyle.BORDER_THIN;
            csMainContent.RightBorderColor = HSSFColor.BLACK.index;
            csMainContent.BorderTop = HSSFCellStyle.BORDER_THIN;
            csMainContent.TopBorderColor = HSSFColor.BLACK.index;
            csMainContent.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            csMainContent.Alignment = HSSFCellStyle.ALIGN_CENTER;
            csMainContent.WrapText = true;
            csMainContent.SetFont(fMainContent);

            csMainContentRot = hssfworkbook.CreateCellStyle();
            csMainContentRot.Rotation = (short)90;
            csMainContentRot.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csMainContentRot.BottomBorderColor = HSSFColor.BLACK.index;
            csMainContentRot.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csMainContentRot.LeftBorderColor = HSSFColor.BLACK.index;
            csMainContentRot.BorderRight = HSSFCellStyle.BORDER_THIN;
            csMainContentRot.RightBorderColor = HSSFColor.BLACK.index;
            csMainContentRot.BorderTop = HSSFCellStyle.BORDER_THIN;
            csMainContentRot.TopBorderColor = HSSFColor.BLACK.index;
            csMainContentRot.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            csMainContentRot.Alignment = HSSFCellStyle.ALIGN_CENTER;
            csMainContentRot.WrapText = true;
            csMainContentRot.SetFont(fMainContent);

            csMainContentDec = hssfworkbook.CreateCellStyle();
            csMainContentDec.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.BottomBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.LeftBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.BorderRight = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.RightBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.BorderTop = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.TopBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            csMainContentDec.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00000");
            csMainContentDec.SetFont(fMainContent);

            csMainContentWrap = hssfworkbook.CreateCellStyle();
            csMainContentWrap.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.BottomBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.LeftBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.BorderRight = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.RightBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.BorderTop = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.TopBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            csMainContentWrap.WrapText = true;
            csMainContentWrap.SetFont(fMainContent);

            csTotalInfo = hssfworkbook.CreateCellStyle();
            csTotalInfo.BorderTop = HSSFCellStyle.BORDER_THIN;
            csTotalInfo.TopBorderColor = HSSFColor.BLACK.index;
            csTotalInfo.Alignment = HSSFCellStyle.ALIGN_LEFT;
            csTotalInfo.SetFont(fTotalInfo);
        }

        public void ClearReport()
        {

        }

        public void WriteSheetMainInfo()
        {
            HSSFCell Cell1;
            pos13 = 0;

            int DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Артикул");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование операции");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("ТИ");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("№ инструкции по охране труда");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Оборудование (код, номер, тип, наименование, марка)");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Инструмент, приспособления");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Документ по контролю и средства контроля");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Код профессии");
            Cell1.CellStyle = csMainContentRot;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Разряд");
            Cell1.CellStyle = csMainContentRot;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Материал");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Расход");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Норма времени");
            Cell1.CellStyle = csMainContentRot;
            Cell1 = sheet13.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Расценка, руб.");
            Cell1.CellStyle = csMainContentRot;

            pos13 = 0;
            DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet14.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet14.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Количество");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet14.CreateRow(pos13).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Ед. изм.");
            Cell1.CellStyle = csMainContent;

            pos13++;
        }

        public void WriteTechStoreName(string TechStoreName)
        {
            HSSFCell Cell1;

            Cell1 = sheet13.CreateRow(pos13).CreateCell(0);
            Cell1.SetCellValue(TechStoreName);
            Cell1.CellStyle = csHeader;
            sheet13.AddMergedRegion(new Region(pos13, 0, pos13, 12));
            pos13++;
        }

        public void WriteToExcel(string Operation, string Machine, string ToolsName, string Position, string Article, DataRow[] rows, double WorkTimeNorm)
        {
            HSSFCell Cell1;

            int p = pos13;

            int firstRowIndex = pos13;
            int secondRowIndex = pos13 - 1 + rows.Count();
            if (rows.Count() > 0)
            {
                for (int i = 0; i < rows.Count(); i++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        //if (j == 0 || j == 1 || j == 4 || j == 7 || j == 11)
                        //    continue;
                        Cell1 = sheet13.CreateRow(pos13).CreateCell(j);
                        Cell1.CellStyle = csMainContentWrap;
                    }
                    Cell1 = sheet13.CreateRow(pos13).CreateCell(9);
                    Cell1.SetCellValue(rows[i]["Name"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet13.CreateRow(pos13).CreateCell(10);
                    if (rows[i]["Count"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDouble(rows[i]["Count"]) + " " + rows[i]["Measure"].ToString());
                    Cell1.CellStyle = csMainContentDec;
                    pos13++;
                    if (rows.Count() == i + 1)
                    {
                        for (int j = 0; j < 13; j++)
                        {
                            Cell1 = sheet13.CreateRow(pos13).CreateCell(j);
                            Cell1.CellStyle = csTotalInfo;
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < 13; j++)
                {
                    Cell1 = sheet13.CreateRow(pos13).CreateCell(j);
                    Cell1.CellStyle = csMainContentWrap;
                }
                pos13++;
            }

            Cell1 = sheet13.CreateRow(p).CreateCell(0);
            Cell1.SetCellValue(Article);
            Cell1.CellStyle = csMainContentWrap;
            Cell1 = sheet13.CreateRow(p).CreateCell(1);
            Cell1.SetCellValue(Operation);
            Cell1.CellStyle = csMainContentWrap;
            Cell1 = sheet13.CreateRow(p).CreateCell(4);
            Cell1.SetCellValue(Machine);
            Cell1.CellStyle = csMainContentWrap;
            Cell1 = sheet13.CreateRow(p).CreateCell(5);
            Cell1.SetCellValue(ToolsName);
            Cell1.CellStyle = csMainContentWrap;
            Cell1 = sheet13.CreateRow(p).CreateCell(7);
            Cell1.SetCellValue(Position);
            Cell1.CellStyle = csMainContentWrap;
            Cell1 = sheet13.CreateRow(p).CreateCell(11);
            if (WorkTimeNorm != -1)
                Cell1.SetCellValue(WorkTimeNorm);
            Cell1.CellStyle = csMainContentDec;

            if (rows.Count() > 1)
            {
                sheet13.AddMergedRegion(new Region(firstRowIndex, 0, secondRowIndex, 0));
                sheet13.AddMergedRegion(new Region(firstRowIndex, 1, secondRowIndex, 1));
                sheet13.AddMergedRegion(new Region(firstRowIndex, 4, secondRowIndex, 4));
                sheet13.AddMergedRegion(new Region(firstRowIndex, 7, secondRowIndex, 7));
                sheet13.AddMergedRegion(new Region(firstRowIndex, 11, secondRowIndex, 11));
            }
            //pos13++;
        }

        public void WriteSummaryMaterials(DataTable table)
        {
            HSSFCell Cell1;
            pos13 = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Cell1 = sheet14.CreateRow(pos13).CreateCell(0);
                Cell1.SetCellValue(table.Rows[i]["Name"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet14.CreateRow(pos13).CreateCell(1);
                if (table.Rows[i]["Count"] != DBNull.Value)
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                Cell1.CellStyle = csMainContentDec;
                Cell1 = sheet14.CreateRow(pos13).CreateCell(2);
                Cell1.SetCellValue(table.Rows[i]["Measure"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet14.CreateRow(pos13).CreateCell(3);
                Cell1.SetCellValue(table.Rows[i]["Condition"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                pos13++;
            }
        }

        public void WriteSummaryMachines(DataTable table)
        {
            HSSFCell Cell1;
            pos13 = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Cell1 = sheet15.CreateRow(pos13).CreateCell(0);
                Cell1.SetCellValue(table.Rows[i]["Name"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet15.CreateRow(pos13).CreateCell(1);
                if (table.Rows[i]["Count"] != DBNull.Value)
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                Cell1.CellStyle = csMainContentDec;
                pos13++;
            }
        }

        public void WriteSummaryLaborCosts(DataTable table)
        {
            HSSFCell Cell1;
            pos13 = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Cell1 = sheet16.CreateRow(pos13).CreateCell(0);
                Cell1.SetCellValue(table.Rows[i]["Name"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet16.CreateRow(pos13).CreateCell(1);
                if (table.Rows[i]["Rank"] != DBNull.Value)
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Rank"]));
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet16.CreateRow(pos13).CreateCell(2);
                if (table.Rows[i]["Count"] != DBNull.Value)
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                Cell1.CellStyle = csMainContentDec;
                pos13++;
            }
        }

        public void SaveFile(string FileName, bool bOpenFile)
        {
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

            if (bOpenFile)
                System.Diagnostics.Process.Start(file.FullName);
        }
    }
}
