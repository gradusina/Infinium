using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Admin
{
    class AbsenceJournal
    {
        private DataTable _absencesJournalDataTable;
        private DataTable _positionsDataTable;
        private DataTable _usersDataTable;

        public BindingSource AbsencesJournalBindingSource;

        public AbsenceJournal()
        {
            Create();
            Fill();
            AbsencesJournalBindingSource.DataSource = _absencesJournalDataTable;
        }

        private void Create()
        {
            _absencesJournalDataTable = new DataTable();
            _positionsDataTable = new DataTable();
            _usersDataTable = new DataTable();

            _absencesJournalDataTable.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            _absencesJournalDataTable.Columns.Add(new DataColumn("Position", Type.GetType("System.String")));
            _absencesJournalDataTable.Columns.Add(new DataColumn("Rate", Type.GetType("System.Decimal")));

            AbsencesJournalBindingSource = new BindingSource();
        }

        public DataGridViewComboBoxColumn UserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "UserColumn",
                    HeaderText = "Сотрудник",
                    DataPropertyName = "UserID",
                    DataSource = new DataView(_usersDataTable),
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

        private void Fill()
        {
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Positions", ConnectionStrings.LightConnectionString))
            {
                da.Fill(_positionsDataTable);
            }
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM Users", ConnectionStrings.UsersConnectionString))
            {
                da.Fill(_usersDataTable);
            }
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT TOP 0 * FROM AbsencesJournal", ConnectionStrings.LightConnectionString))
            {
                da.Fill(_absencesJournalDataTable);
            }
        }

        private string UserName(int UserID)
        {
            string name = string.Empty;

            DataRow row = _usersDataTable
                .AsEnumerable().FirstOrDefault(r => r.Field<Int64>("UserID") == UserID);
            if (row != null)
                name = row["ShortName"].ToString();
            return name;
        }

        private void FillAbsenceJournal()
        {
            for (int i = 0; i < _absencesJournalDataTable.Rows.Count; i++)
            {
                int UserID = Convert.ToInt32(_absencesJournalDataTable.Rows[i]["UserID"]);
                _absencesJournalDataTable.Rows[i]["UserName"] = UserName(UserID);
            }
        }

        private void GetPositionFromStaffList(int FactoryID, int UserID)
        {
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM StaffList", ConnectionStrings.LightConnectionString))
            {
                da.Fill(_positionsDataTable);
            }
        }

        public void GetJournal(string year, string month, int FactoryID)
        {
            int Monthint = Convert.ToDateTime(month + " " + year).Month;
            int Yearint = Convert.ToInt32(year);
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM AbsencesJournal WHERE FactoryID =" + FactoryID +
                " AND ((DATEPART(month, DateStart) = " + Monthint + " AND DATEPART(year, DateStart) = " + Yearint +
                ") OR (DATEPART(month, DateFinish) = " + Monthint + " AND DATEPART(year, DateFinish) = " + Yearint + "))", ConnectionStrings.LightConnectionString))
            {
                _absencesJournalDataTable.Clear();
                da.Fill(_absencesJournalDataTable);
            }

            FillAbsenceJournal();
        }

        public void AddNewAbsence(string year)
        {
            DataRow newRow = _absencesJournalDataTable.NewRow();
            newRow["Year"] = Convert.ToInt32(year);
            _absencesJournalDataTable.Rows.Add(newRow);
        }

        public void SaveJournal()
        {
            string SelectCommand = "SELECT TOP 0 * FROM AbsencesJournal";
            using (SqlDataAdapter da = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    da.Update(_absencesJournalDataTable);
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
