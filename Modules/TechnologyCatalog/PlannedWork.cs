using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.TechnologyCatalog
{
    public class PlannedWork
    {
        public FileManager FM = null;

        private DataTable _machinesDt;
        private DataTable _plannedWorksDt;
        private DataTable _statusesDt;
        private DataTable _rolePermissionsDt;
        private DataTable _executorsDt;
        private DataTable _filesDt;
        private DataTable _usersDt;

        public BindingSource MachinesBs;
        public BindingSource PlannedWorksBs;
        public BindingSource StatusesBs;
        public BindingSource ExecutorsBs;
        public BindingSource FilesBs;
        public BindingSource UsersBs;

        public PlannedWork()
        {
            Create();
            Fill();
            Binding();
        }

        private void Create()
        {
            FM = new FileManager();
            _machinesDt = new DataTable();
            _statusesDt = new DataTable();
            _plannedWorksDt = new DataTable();
            _executorsDt = new DataTable();
            _filesDt = new DataTable();
            _usersDt = new DataTable();
            _rolePermissionsDt = new DataTable();
        }

        private void Fill()
        {
            var selectCommand = @"SELECT MachineID, MachineName FROM Machines ORDER BY MachineName";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.CatalogConnectionString))
            {
                da.Fill(_machinesDt);
                _machinesDt.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
                for (var i = 0; i < _machinesDt.Rows.Count; i++)
                    _machinesDt.Rows[i]["Check"] = false;
            }
            selectCommand = @"SELECT * FROM PlannedWorkStatuses";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_statusesDt);
            }
            selectCommand = @"SELECT * FROM PlannedWorks";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_plannedWorksDt);
            }
            selectCommand = @"SELECT TOP 0 * FROM PlannedWorkUsers";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_executorsDt);
            }
            selectCommand = @"SELECT TOP 0 * FROM PlannedWorkFiles";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_filesDt);
            }
            selectCommand = @"SELECT UserID, Name, ShortName FROM Users ORDER BY Name";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.UsersConnectionString))
            {
                da.Fill(_usersDt);
                _usersDt.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
                for (var i = 0; i < _usersDt.Rows.Count; i++)
                    _usersDt.Rows[i]["Check"] = false;
            }
        }

        private void Binding()
        {
            MachinesBs = new BindingSource { DataSource = new DataView(_machinesDt) };

            StatusesBs = new BindingSource { DataSource = new DataView(_statusesDt) };

            ExecutorsBs = new BindingSource { DataSource = _executorsDt };

            FilesBs = new BindingSource { DataSource = _filesDt };

            UsersBs = new BindingSource { DataSource = _usersDt };

            PlannedWorksBs = new BindingSource { DataSource = _plannedWorksDt };
        }

        public DataGridViewComboBoxColumn ExecutorsColumn
        {
            get
            {
                var column = new DataGridViewComboBoxColumn
                {
                    Name = "ExecutorsColumn",
                    DataPropertyName = "UserID",
                    DataSource = new DataView(_usersDt),
                    ValueMember = "UserID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn MachinesColumn
        {
            get
            {
                var column = new DataGridViewComboBoxColumn
                {
                    HeaderText = @"Станок",
                    Name = "MachineColumn",
                    DataPropertyName = "MachineID",
                    DataSource = new DataView(_machinesDt),
                    ValueMember = "MachineID",
                    DisplayMember = "MachineName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn StatusesColumn
        {
            get
            {
                var column = new DataGridViewComboBoxColumn
                {
                    HeaderText = @"Статус",
                    Name = "StatusesColumn",
                    DataPropertyName = "PlannedWorkStatusID",
                    DataSource = new DataView(_statusesDt),
                    ValueMember = "PlannedWorkStatusID",
                    DisplayMember = "Status",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn UserNameColumn
        {
            get
            {
                var column = new DataGridViewComboBoxColumn
                {
                    HeaderText = @"Ф.И.О.",
                    Name = "Name",
                    DataPropertyName = "UserID",
                    DataSource = new DataView(_usersDt),
                    ValueMember = "UserID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public void UpdateExecutors(int plannedWorkId)
        {
            var selectCommand = @"SELECT * FROM PlannedWorkUsers WHERE PlannedWorkID=" + plannedWorkId;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _executorsDt.Clear();
                da.Fill(_executorsDt);
            }
        }

        public void UpdateFiles(int plannedWorkId)
        {
            var selectCommand = @"SELECT * FROM PlannedWorkFiles WHERE PlannedWorkID=" + plannedWorkId;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _filesDt.Clear();
                da.Fill(_filesDt);
            }
        }

        public void UpdateWorks(bool bMachines, bool bUsers, bool bNew, bool bConfirm, bool bInProduciton, bool bEnd,
            bool bDate, int dateType, DateTime from, DateTime to)
        {
            var filter = "";
            var filterStatus = "";
            if (bMachines)
            {
                if (SelectedMachines.Count > 0)
                {
                    filter = " WHERE MachineID IN (" + string.Join(",", SelectedMachines.OfType<Int32>().ToArray()) + ")";
                }
                else
                    filter = " WHERE PlannedWorkID =-1";
            }
            if (bUsers)
            {
                if (SelectedUsers.Count > 0)
                {
                    if (filter.Length > 0)
                        filter +=
                            " AND PlannedWorkID IN (SELECT PlannedWorkID FROM PlannedWorkUsers WHERE UserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + "))";
                    else
                        filter =
                            " WHERE PlannedWorkID IN (SELECT PlannedWorkID FROM PlannedWorkUsers WHERE UserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + "))";
                }
                else
                    filter = " WHERE PlannedWorkID =-1";
            }
            if (!bNew && !bConfirm && !bInProduciton && !bEnd)
                filterStatus = "PlannedWorkStatusID=-1";
            else
            {
                if (bNew)
                {
                    filterStatus = "PlannedWorkStatusID=1";
                }
                if (bConfirm)
                {
                    if (filterStatus.Length > 0)
                        filterStatus += " OR PlannedWorkStatusID=2";
                    else
                        filterStatus = "PlannedWorkStatusID=2";
                }
                if (bInProduciton)
                {
                    if (filterStatus.Length > 0)
                        filterStatus += " OR PlannedWorkStatusID=3";
                    else
                        filterStatus = "PlannedWorkStatusID=3";
                }
                if (bEnd)
                {
                    if (filterStatus.Length > 0)
                        filterStatus += " OR PlannedWorkStatusID=4";
                    else
                        filterStatus = "PlannedWorkStatusID=4";
                }
            }
            if (bDate)
            {
                if (dateType == 0)
                {
                    if (filter.Length > 0)
                        filter += " AND CAST(StartDate AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                            "' AND CAST(StartDate AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                    else
                        filter = " WHERE CAST(StartDate AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                            "' AND CAST(StartDate AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                }
                if (dateType == 1)
                {
                    if (filter.Length > 0)
                        filter += " AND CAST(CreateDate AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                            "' AND CAST(CreateDate AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                    else
                        filter = " WHERE CAST(CreateDate AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                            "' AND CAST(CreateDate AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                }
                if (dateType == 2)
                {
                    if (filter.Length > 0)
                        filter += " AND CAST(EndDate AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                            "' AND CAST(EndDate AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                    else
                        filter = " WHERE CAST(EndDate AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                            "' AND CAST(EndDate AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                }
            }
            if (filter.Length > 0)
                filter += " AND (" + filterStatus + ")";
            else
                filter = " WHERE " + filterStatus;

            var selectCommand = @"SELECT * FROM PlannedWorks " + filter;
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _plannedWorksDt.Clear();
                da.Fill(_plannedWorksDt);
            }
        }

        public void AddExecutors(int plannedWorkId, ArrayList executorsList)
        {
            var currentDate = Security.GetCurrentDate();
            foreach (var t in executorsList)
            {
                var rows = _executorsDt.Select("PlannedWorkID = " + plannedWorkId);
                if (rows.Any())
                    continue;
                var newRow = _executorsDt.NewRow();
                newRow["UserID"] = t;
                newRow["PlannedWorkID"] = plannedWorkId;
                newRow["CreateUserID"] = Security.CurrentUserID;
                newRow["CreateDate"] = currentDate;
                _executorsDt.Rows.Add(newRow);
            }
        }

        public void Confirm(int plannedWorkId)
        {
            var currentDate = Security.GetCurrentDate();
            var rows = _plannedWorksDt.Select("PlannedWorkID = " + plannedWorkId);
            if (!rows.Any()) return;
            rows[0]["PlannedWorkStatusID"] = 2;
            if (rows[0]["ConfirmUserID"] == DBNull.Value)
                rows[0]["ConfirmUserID"] = Security.CurrentUserID;
            if (rows[0]["ConfirmDate"] == DBNull.Value)
                rows[0]["ConfirmDate"] = currentDate;
        }

        public void Start(int plannedWorkId)
        {
            var currentDate = Security.GetCurrentDate();
            var rows = _plannedWorksDt.Select("PlannedWorkID = " + plannedWorkId);
            if (!rows.Any()) return;
            rows[0]["PlannedWorkStatusID"] = 3;
            if (rows[0]["StartUserID"] == DBNull.Value)
                rows[0]["StartUserID"] = Security.CurrentUserID;
            if (rows[0]["StartDate"] == DBNull.Value)
                rows[0]["StartDate"] = currentDate;
        }

        public void End(int plannedWorkId)
        {
            var currentDate = Security.GetCurrentDate();
            var rows = _plannedWorksDt.Select("PlannedWorkID = " + plannedWorkId);
            if (!rows.Any()) return;
            rows[0]["PlannedWorkStatusID"] = 4;
            if (rows[0]["EndUserID"] == DBNull.Value)
                rows[0]["EndUserID"] = Security.CurrentUserID;
            if (rows[0]["EndDate"] == DBNull.Value)
                rows[0]["EndDate"] = currentDate;
        }

        public void SaveExecutors()
        {
            const string selectCommand = @"SELECT TOP 0 * FROM PlannedWorkUsers";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    da.Update(_executorsDt);
                }
            }
        }

        public void SaveWorks()
        {
            const string selectCommand = @"SELECT TOP 0 * FROM PlannedWorks";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    da.Update(_plannedWorksDt);
                }
            }
        }

        public void RemoveExecutor()
        {
            ExecutorsBs.RemoveCurrent();
        }

        public void RemoveWork()
        {
            PlannedWorksBs.RemoveCurrent();
        }

        public void MoveToPosition(int plannedWorkId)
        {
            PlannedWorksBs.Position = PlannedWorksBs.Find("PlannedWorkID", plannedWorkId);
        }

        public ArrayList SelectedMachines
        {
            get
            {
                var machines = new ArrayList();

                for (var i = 0; i < _machinesDt.Rows.Count; i++)
                {
                    if (!Convert.ToBoolean(_machinesDt.Rows[i]["Check"]))
                        continue;

                    machines.Add(Convert.ToInt32(_machinesDt.Rows[i]["MachineID"]));
                }

                return machines;
            }
        }

        public ArrayList SelectedUsers
        {
            get
            {
                var users = new ArrayList();

                for (var i = 0; i < _usersDt.Rows.Count; i++)
                {
                    if (!Convert.ToBoolean(_usersDt.Rows[i]["Check"]))
                        continue;

                    users.Add(Convert.ToInt32(_usersDt.Rows[i]["UserID"]));
                }

                return users;
            }
        }

        public void SelectAllMachines(bool check)
        {
            for (var i = 0; i < _machinesDt.Rows.Count; i++)
            {
                _machinesDt.Rows[i]["Check"] = check;
            }
        }

        public void SelectAllUsers(bool check)
        {
            for (var i = 0; i < _usersDt.Rows.Count; i++)
            {
                _usersDt.Rows[i]["Check"] = check;
            }
        }

        public void GetPermissions(int userId, string formName)
        {
            using (var da = new SqlDataAdapter("SELECT * FROM UserRoles WHERE UserID = " + userId +
                " AND RoleID IN (SELECT RoleID FROM Roles WHERE ModuleID IN " +
                " (SELECT ModuleID FROM Modules WHERE FormName = '" + formName + "'))", ConnectionStrings.UsersConnectionString))
            {
                da.Fill(_rolePermissionsDt);
            }
        }
        public bool PermissionGranted(int roleId)
        {
            var rows = _rolePermissionsDt.Select("RoleID = " + roleId);
            return rows.Any();
        }

        public bool AttachFile(string FileName, string Extension, string Path, int PlannedWorkID)
        {
            var ok = true;

            //write to ftp
            try
            {
                var sDestFolder = Configs.DocumentsPath + FileManager.GetPath("PlannedWorkFiles");
                var sExtension = Extension;
                var sFileName = FileName;

                var j = 1;
                while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                {
                    sFileName = FileName + "(" + j++ + ")";
                }
                FileName = sFileName + sExtension;
                if (FM.UploadFile(Path, sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                    return false;
            }
            catch
            {
                ok = false;
                return false;
            }
            if (!ok)
                return false;

            const string selectCommand = @"SELECT TOP 0 * FROM PlannedWorkFiles";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);

                        FileInfo fi;

                        try
                        {
                            fi = new FileInfo(Path);
                        }
                        catch
                        {
                            ok = false;
                            return false;
                        }

                        var newRow = dt.NewRow();
                        newRow["PlannedWorkID"] = PlannedWorkID;
                        newRow["FileName"] = FileName;
                        newRow["FileSize"] = fi.Length;
                        newRow["CreateUserID"] = Security.CurrentUserID;
                        newRow["CreateDate"] = Security.GetCurrentDate();
                        dt.Rows.Add(newRow);

                        da.Update(dt);
                    }
                }
            }

            return ok;
        }

        public bool RemoveFile(int plannedWorkFileId)
        {
            var ok = true;
            using (var da = new SqlDataAdapter("SELECT * FROM PlannedWorkFiles WHERE PlannedWorkFileID = " + plannedWorkFileId,
                ConnectionStrings.LightConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dt = new DataTable())
                    {
                        da.Fill(dt);
                        bool bOk = false;
                        foreach (DataRow Row in dt.Rows)
                        {
                            try
                            {
                                bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("PlannedWorkFiles") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                            }
                            catch
                            {
                                ok = false;
                                return false;
                            }
                        }
                        if (bOk)
                        {
                            dt.Rows[0].Delete();
                            da.Update(dt);
                        }
                    }
                }
            }
            return ok;
        }

    }
}
