using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Admin
{
    class ProductionShedule
    {
        private DataTable _hoursDataTable;
        private DataTable _sourceDataTable;

        public BindingSource HoursBindingSource;

        public ProductionShedule()
        {
            Create();
            Fill();
            HoursBindingSource.DataSource = _hoursDataTable;
        }

        private void Create()
        {
            _hoursDataTable = new DataTable();
            _sourceDataTable = new DataTable();
            HoursBindingSource = new BindingSource();
            _hoursDataTable.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));
            for (int j = 1; j <= DateTime.DaysInMonth(2020, 1); j++)
            {
                DataColumn column = new DataColumn(j.ToString())
                {
                    DataType = Type.GetType("System.Int32"),
                    DefaultValue = 0
                };
                _hoursDataTable.Columns.Add(column);
            }

            string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;

            for (int i = 0; i < monthNames.Length - 1; i++)
            {
                DataRow newRow = _hoursDataTable.NewRow();
                newRow["MonthName"] = monthNames[i];
                _hoursDataTable.Rows.Add(newRow);
            }
        }

        private void Fill()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT TOP 0 * FROM ProductionShedule", ConnectionStrings.LightConnectionString))
            {
                da.Fill(_sourceDataTable);
            }
        }

        public void GetShedule(string year)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM ProductionShedule WHERE Year=" + year, ConnectionStrings.LightConnectionString))
            {
                _sourceDataTable.Clear();
                da.Fill(_sourceDataTable);
            }
        }

        public void FillHoursDataTable()
        {
            string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;
            DataTable dt = new DataTable();
            for (int i = 0; i < monthNames.Length - 1; i++)
            {
                _hoursDataTable.Rows[i]["MonthName"] = monthNames[i];
                if (_sourceDataTable.Rows.Count == 0)
                {
                    for (int j = 0; j < _hoursDataTable.Columns.Count - 1; j++)
                    {
                        _hoursDataTable.Rows[i][j + 1] = 0;
                    }
                }
                else
                {
                    using (DataView DV = new DataView(_sourceDataTable, "Month=" + (i + 1), "Day",
                        DataViewRowState.CurrentRows))
                    {
                        dt = DV.ToTable();
                    }

                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        var day = dt.Rows[k]["Day"].ToString();
                        var hour = dt.Rows[k]["Hour"].ToString();
                        _hoursDataTable.Rows[i][day] = hour;
                    }
                }
            }
        }

        public void FillSourceDataTable(string year)
        {
            if (_sourceDataTable.Rows.Count == 0)
            {
                for (int i = 0; i < _hoursDataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < _hoursDataTable.Columns.Count - 1; j++)
                    {
                        DataRow newRow = _sourceDataTable.NewRow();
                        newRow["Year"] = Convert.ToInt32(year);
                        newRow["Month"] = i + 1;
                        newRow["Day"] = j + 1;
                        newRow["Hour"] = _hoursDataTable.Rows[i][j + 1];
                        _sourceDataTable.Rows.Add(newRow);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _hoursDataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < _hoursDataTable.Columns.Count - 1; j++)
                    {
                        var hour = _hoursDataTable.Rows[i][j + 1].ToString();

                        var rows = _sourceDataTable.Select("Month=" + (i + 1) + " AND Day=" + (j + 1));
                        for (int k = 0; k < rows.Length; k++)
                        {
                            rows[k]["Hour"] = hour;
                        }
                    }
                }
            }
        }

        public void SaveShedule()
        {
            string SelectCommand = "SELECT TOP 0 * FROM ProductionShedule";
            using (SqlDataAdapter da = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    da.Update(_sourceDataTable);
                }
            }
        }

        public bool PermissionGranted(int UserID, string FormName, int RoleID)
        {
            bool Granted = false;
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM UserRoles WHERE UserID = " + UserID +
                                                          " AND RoleID IN (SELECT RoleID FROM Roles WHERE ModuleID IN " +
                                                          " (SELECT ModuleID FROM Modules WHERE FormName = '" + FormName + "'))", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable dt = new DataTable())
                {
                    da.Fill(dt);

                    DataRow[] Rows = dt.Select("RoleID = " + RoleID);
                    Granted = Rows.Any();
                }
            }

            return Granted;
        }
    }
}
