using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.CabFurnitureAssignments
{
    class CabFurStorage
    {
        DataTable workShopsDt = null;
        DataTable racksDt = null;
        DataTable cellsDt = null;

        public BindingSource workShopsBs = null;
        public BindingSource racksBs = null;
        public BindingSource cellsBs = null;

        SqlDataAdapter workShopsDa = null;
        SqlCommandBuilder workShopsCb = null;

        SqlDataAdapter racksDa = null;
        SqlCommandBuilder racksCb = null;

        SqlDataAdapter cellsDa = null;
        SqlCommandBuilder cellsCb = null;

        public CabFurStorage()
        {
            CreateObjects();
            //ПОПРОБОВАТЬ НАОБОРОТ ВЫЗВАТЬ ФУНКЦИИ
            FillTables();
            BindingTables();
        }

        private void CreateObjects()
        {
            workShopsDt = new DataTable();
            racksDt = new DataTable();
            cellsDt = new DataTable();

            workShopsBs = new BindingSource();
            racksBs = new BindingSource();
            cellsBs = new BindingSource();
        }

        private void BindingTables()
        {
            workShopsBs = new BindingSource()
            {
                DataSource = workShopsDt
            };
            racksBs = new BindingSource()
            {
                DataSource = racksDt
            };
            cellsBs = new BindingSource()
            {
                DataSource = cellsDt
            };
        }

        private void FillTables()
        {
            string selectCommand = @"SELECT * FROM WorkShops ORDER BY Name";
            workShopsDa = new SqlDataAdapter(selectCommand, ConnectionStrings.StorageConnectionString);
            workShopsCb = new SqlCommandBuilder(workShopsDa);
            workShopsDa.Fill(workShopsDt);

            selectCommand = @"SELECT * FROM Racks ORDER BY Name";
            racksDa = new SqlDataAdapter(selectCommand, ConnectionStrings.StorageConnectionString);
            racksCb = new SqlCommandBuilder(racksDa);
            racksDa.Fill(racksDt);

            selectCommand = @"SELECT * FROM Cells ORDER BY Name";
            cellsDa = new SqlDataAdapter(selectCommand, ConnectionStrings.StorageConnectionString);
            cellsCb = new SqlCommandBuilder(cellsDa);
            cellsDa.Fill(cellsDt);
        }

        /// <summary>
        /// привязать упаковки к ячейке
        /// </summary>
        /// <param name="cellID"></param>
        /// <param name="packageIds"></param>
        public void BindPackagesToCell(int cellID, int[] packageIds)
        {
            string filter = string.Empty;
            foreach (int item in packageIds)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = "SELECT * FROM CabFurniturePackages WHERE CabFurniturePackageID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(filter, ConnectionStrings.StorageConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DateTime dateTime = Security.GetCurrentDate();

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (DT.Rows[i]["CellID"] == DBNull.Value)
                                    DT.Rows[i]["CellID"] = cellID;
                                if (DT.Rows[i]["BindDateTime"] == DBNull.Value)
                                    DT.Rows[i]["BindDateTime"] = dateTime;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        #region WorkShops

        public bool HasWorkShops
        {
            get { return workShopsBs.Count > 0; }
        }

        public string CurrentWorkShopName
        {
            get
            {
                string name = string.Empty;
                if (workShopsBs.Current != null)
                    name = ((DataRowView)workShopsBs.Current).Row["Name"].ToString();
                return name;
            }
        }

        public int CurrentWorkShopId
        {
            get
            {
                int id = -1;
                if (workShopsBs.Current != null)
                    id = Convert.ToInt32(((DataRowView)workShopsBs.Current).Row["WorkShopID"]);
                return id;
            }
        }

        public int MaxWorkShopId
        {
            get { return Convert.ToInt32(workShopsDt.Compute("max([WorkShopID])", string.Empty)); }
        }

        public void SetWorkShopPosition(int Id)
        {
            workShopsBs.Position = workShopsBs.Find("WorkShopID", Id);
        }

        public void AddWorkShop(string name)
        {
            DataRow NewRow = workShopsDt.NewRow();
            NewRow["Name"] = name;
            workShopsDt.Rows.Add(NewRow);
        }

        public void EditWorkShop(int id, string name)
        {
            DataRow[] rows = workShopsDt.Select("WorkShopID = " + id);
            if (rows.Count() > 0)
                rows[0]["Name"] = name;
        }

        public void RemoveWorkShop(int id)
        {
            foreach (DataRow row in workShopsDt.Select("WorkShopID = " + id))
                row.Delete();
        }

        public void UpdateWorkShops()
        {
            workShopsDt.Clear();
            workShopsDa.Fill(workShopsDt);
        }

        public void SaveWorkShops()
        {
            workShopsDa.Update(workShopsDt);
        }

        #endregion

        #region Racks

        public bool HasRacks
        {
            get { return racksBs.Count > 0; }
        }

        public string CurrentRackName
        {
            get
            {
                string name = string.Empty;
                if (racksBs.Current != null)
                    name = ((DataRowView)racksBs.Current).Row["Name"].ToString();
                return name;
            }
        }

        public int CurrentRackId
        {
            get
            {
                int id = -1;
                if (racksBs.Current != null)
                    id = Convert.ToInt32(((DataRowView)racksBs.Current).Row["RackID"]);
                return id;
            }
        }

        public int MaxRackId
        {
            get { return Convert.ToInt32(racksDt.Compute("max([RackID])", string.Empty)); }
        }

        public void SetRackPosition(int Id)
        {
            racksBs.Position = racksBs.Find("RackID", Id);
        }

        public void FilterRacksByWorkShop(int workShopId)
        {
            racksBs.Filter = "WorkShopID=" + workShopId;
            racksBs.MoveFirst();
        }

        public void AddRack(string name, int workShopId)
        {
            DataRow NewRow = racksDt.NewRow();
            NewRow["Name"] = name;
            NewRow["WorkShopID"] = workShopId;
            racksDt.Rows.Add(NewRow);
        }

        public void EditRack(int id, string name)
        {
            DataRow[] rows = racksDt.Select("RackID = " + id);
            if (rows.Count() > 0)
                rows[0]["Name"] = name;
        }

        public void RemoveRack(int id)
        {
            foreach (DataRow row in racksDt.Select("RackID = " + id))
                row.Delete();
        }

        public void UpdateRacks()
        {
            racksDt.Clear();
            racksDa.Fill(racksDt);
            racksBs.MoveFirst();
        }

        public void SaveRacks()
        {
            racksDa.Update(racksDt);
        }

        #endregion

        #region Cells

        public bool HasCells
        {
            get { return cellsBs.Count > 0; }
        }

        public string CurrentCellName
        {
            get
            {
                string name = string.Empty;
                if (cellsBs.Current != null)
                    name = ((DataRowView)cellsBs.Current).Row["Name"].ToString();
                return name;
            }
        }

        public int CurrentCellId
        {
            get
            {
                int id = -1;
                if (cellsBs.Current != null)
                    id = Convert.ToInt32(((DataRowView)cellsBs.Current).Row["CellID"]);
                return id;
            }
        }

        public int MaxCellId
        {
            get { return Convert.ToInt32(cellsDt.Compute("max([CellID])", string.Empty)); }
        }

        public void FilterCellsByRack(int rackId)
        {
            cellsBs.Filter = "RackID=" + rackId;
            cellsBs.MoveFirst();
        }

        public void AddCell(string name, int rackId)
        {
            DataRow NewRow = cellsDt.NewRow();
            NewRow["Name"] = name;
            NewRow["RackID"] = rackId;
            cellsDt.Rows.Add(NewRow);
        }

        public void EditCell(int id, string name)
        {
            DataRow[] rows = cellsDt.Select("CellID = " + id);
            if (rows.Count() > 0)
                rows[0]["Name"] = name;
        }

        public void RemoveCell(int id)
        {
            foreach (DataRow row in cellsDt.Select("CellID = " + id))
                row.Delete();
        }

        public void UpdateCells()
        {
            cellsDt.Clear();
            cellsDa.Fill(cellsDt);
        }

        public void SaveCells()
        {
            cellsDa.Update(cellsDt);
        }

        #endregion
    }


    public class StoragePackagesManager
    {
        DataTable PackageLabelsDT = null;
        DataTable BindPackageLabelsDT = null;
        DataTable TempPackageLabelsDT = null;
        public BindingSource PackageLabelsBS = null;
        public BindingSource BindPackageLabelsBS = null;
        SqlDataAdapter PackageLabelsDA;
        SqlDataAdapter BindPackageLabelsDA;

        DataTable PackageDetailsDT = null;
        public BindingSource PackageDetailsBS = null;
        SqlDataAdapter PackageDetailsDA;

        public StoragePackagesManager()
        {
            PackageLabelsDT = new DataTable();
            BindPackageLabelsDT = new DataTable();
            PackageDetailsDT = new DataTable();
            TempPackageLabelsDT = new DataTable();

            PackageLabelsBS = new BindingSource();
            BindPackageLabelsBS = new BindingSource();
            PackageDetailsBS = new BindingSource();

            string SelectCommand = @"SELECT TOP 0 CabFurniturePackageID, CabFurAssignmentDetailID, PackNumber, PackagesCount, TechStoreSubGroupID, PrintDateTime, AddToStorageDateTime, RemoveFromStorageDateTime, Cells.Name FROM CabFurniturePackages 
                INNER JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID";
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(PackageLabelsDT);
            PackageLabelsDT.Columns.Add(new DataColumn("Index", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 CabFurniturePackageID, CabFurAssignmentDetailID, PackNumber, PackagesCount, TechStoreSubGroupID, PrintDateTime, AddToStorageDateTime, RemoveFromStorageDateTime, Cells.Name FROM CabFurniturePackages 
                INNER JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID";
            BindPackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            BindPackageLabelsDA.Fill(BindPackageLabelsDT);
            BindPackageLabelsDA.Fill(TempPackageLabelsDT);
            BindPackageLabelsDT.Columns.Add(new DataColumn("Index", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID, 
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID";
            PackageDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageDetailsDA.Fill(PackageDetailsDT);

            PackageLabelsBS.DataSource = PackageLabelsDT;
            BindPackageLabelsBS.DataSource = BindPackageLabelsDT;
            PackageDetailsBS.DataSource = PackageDetailsDT;
        }

        public void FilterPackagesLabels(int CellID)
        {
            PackageDetailsDT.Clear();
            string SelectCommand = @"SELECT C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID,
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID
                WHERE CellID=" + CellID;
            PackageDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageDetailsDA.Fill(PackageDetailsDT);

            PackageLabelsDT.Clear();
            SelectCommand = @"SELECT CabFurniturePackageID, CabFurAssignmentDetailID, PackNumber, PackagesCount, TechStoreSubGroupID, PrintDateTime, AddToStorageDateTime, RemoveFromStorageDateTime, Cells.Name FROM CabFurniturePackages
                LEFT JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID WHERE CabFurniturePackages.CellID=" + CellID;
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(PackageLabelsDT);

            for (int i = 0; i < PackageLabelsDT.Rows.Count; i++)
            {
                PackageLabelsDT.Rows[i]["Index"] = i + 1;
            }
        }

        public void Clear()
        {
            BindPackageLabelsDT.Clear();
        }

        public bool GetPackagesLabels(int CabFurniturePackageID)
        {
            TempPackageLabelsDT.Clear();
            string SelectCommand = @"SELECT CabFurniturePackageID, CabFurAssignmentDetailID, PackNumber, PackagesCount, TechStoreSubGroupID, PrintDateTime, AddToStorageDateTime, RemoveFromStorageDateTime, Cells.Name FROM CabFurniturePackages 
                INNER JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID WHERE CabFurniturePackageID=" + CabFurniturePackageID;
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(TempPackageLabelsDT);

            if (TempPackageLabelsDT.Rows.Count > 0)
                BindPackageLabelsDT.Rows.Add(TempPackageLabelsDT.Rows[0].ItemArray);
            return TempPackageLabelsDT.Rows.Count > 0;
        }

        public void FilterPackagesDetails(int CabFurniturePackageID)
        {
            PackageDetailsBS.Filter = "CabFurniturePackageID =" + CabFurniturePackageID;
            PackageDetailsBS.MoveFirst();
        }

        public void ClearPackges()
        {
            PackageLabelsDT.Clear();
            PackageDetailsDT.Clear();
        }

        public void PrintComplements(int[] CabFurniturePackageID)
        {
            string filter = string.Empty;
            foreach (int item in CabFurniturePackageID)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = "SELECT * FROM CabFurniturePackages WHERE CabFurniturePackageID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(filter, ConnectionStrings.StorageConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DateTime PrintDateTime = Security.GetCurrentDate();

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (DT.Rows[i]["PrintUserID"] == DBNull.Value)
                                    DT.Rows[i]["PrintUserID"] = Security.CurrentUserID;
                                if (DT.Rows[i]["PrintDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PrintDateTime"] = PrintDateTime;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

    }
}
