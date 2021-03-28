using ComponentFactory.Krypton.Toolkit;

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
        private DataTable _staffListDataTable;
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
            _absencesJournalDataTable.Columns.Add(new DataColumn("PositionID", Type.GetType("System.Int64")));
            _absencesJournalDataTable.Columns.Add(new DataColumn("Rate", Type.GetType("System.Decimal")));
            _positionsDataTable = new DataTable();
            _staffListDataTable = new DataTable();
            _usersDataTable = new DataTable();

            AbsencesJournalBindingSource = new BindingSource();
        }

        public KryptonDataGridViewDateTimePickerColumn DateStartColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn()
                {
                    CalendarTodayDate = DateTime.Now
                };
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "DateStart";
                Column.HeaderText = "Дата начала";
                Column.Name = "DateStartColumn";
                Column.SortMode = DataGridViewColumnSortMode.Automatic;
                Column.MinimumWidth = 100;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.Width = 250;
                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn DateFinishColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn()
                {
                    CalendarTodayDate = DateTime.Now
                };
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "DateFinish";
                Column.HeaderText = "Дата окончания";
                Column.Name = "DateFinishColumn";
                Column.SortMode = DataGridViewColumnSortMode.Automatic;
                Column.MinimumWidth = 100;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.Width = 250;
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PositionColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PositionColumn",
                    HeaderText = "Должность",
                    DataPropertyName = "PositionID",
                    DataSource = new DataView(_positionsDataTable),
                    ValueMember = "PositionID",
                    DisplayMember = "Position",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    ReadOnly = true,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn UserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    AutoComplete = true,
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
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT TOP 0 StaffListID, PositionID, UserID, Rate FROM StaffList", ConnectionStrings.LightConnectionString))
            {
                da.Fill(_staffListDataTable);
            }
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM Users WHERE Fired<>1 ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                da.Fill(_usersDataTable);
            }
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT TOP 0 * FROM AbsencesJournal", ConnectionStrings.LightConnectionString))
            {
                da.Fill(_absencesJournalDataTable);
            }

        }

        private string GetPosition(int positionId)
        {
            string name = "no_name";

            DataRow[] rows = _positionsDataTable.Select("PositionID=" + positionId);
            if (rows.Any())
                name = rows[0]["Position"].ToString();
            return name;
        }

        private string GetUserName(int userId)
        {
            string name = "no_name";

            DataRow[] rows = _usersDataTable.Select("UserID=" + userId);
            if (rows.Any())
                name = rows[0]["ShortName"].ToString();
            return name;
        }

        public void CopyAbsenceRecord(int userID, object dateStart, object dateFinish, int absenceTypeID)
        {
            DataRow NewRow = _absencesJournalDataTable.NewRow();

            NewRow["UserID"] = userID;
            NewRow["DateStart"] = dateStart;
            NewRow["DateFinish"] = dateFinish;
            NewRow["AbsenceTypeID"] = absenceTypeID;

            _absencesJournalDataTable.Rows.Add(NewRow);
        }

        public void RemoveAbsenceRecord(int absenceID)
        {
            DataRow[] EditRows = _absencesJournalDataTable.Select("AbsenceID = " + absenceID);
            if (EditRows.Count() > 0)
            {
                EditRows[0].Delete();
            }
        }

        private void FillAbsenceJournal()
        {
            for (int i = 0; i < _absencesJournalDataTable.Rows.Count; i++)
            {
                int userId = Convert.ToInt32(_absencesJournalDataTable.Rows[i]["UserID"]);
                int positionId = 0;
                decimal rate = 0;
                Tuple<int, decimal> tuple = GetRateAndPosition(userId);
                positionId = tuple.Item1;
                rate = tuple.Item2;

                _absencesJournalDataTable.Rows[i]["PositionID"] = positionId;
                _absencesJournalDataTable.Rows[i]["Rate"] = rate;
            }
        }

        public void FilterAbsenceJournal(int absenceTypeId)
        {
            AbsencesJournalBindingSource.Filter = "AbsenceTypeID=" + absenceTypeId;
        }

        private Tuple<int, decimal> GetRateAndPosition(int userId)
        {
            int positionId = 0;
            decimal rate = 0;

            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT StaffListID, PositionID, UserID, Rate FROM StaffList
                WHERE UserID=" + userId, ConnectionStrings.LightConnectionString))
            {
                _staffListDataTable.Clear();
                da.Fill(_staffListDataTable);
            }

            if (_staffListDataTable.Rows.Count > 0)
            {
                positionId = Convert.ToInt32(_staffListDataTable.Rows[0]["PositionID"]);
                rate = Convert.ToDecimal(_staffListDataTable.Rows[0]["Rate"]);
            }

            Tuple<int, decimal> tuple = new Tuple<int, decimal>(positionId, rate);
            return tuple;
        }

        public void GetJournal(string year, string month)
        {
            int Monthint = Convert.ToDateTime(month + " " + year).Month;
            int Yearint = Convert.ToInt32(year);
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM AbsencesJournal WHERE" +
                " ((DATEPART(month, DateStart) = " + Monthint + " AND DATEPART(year, DateStart) = " + Yearint +
                ") OR (DATEPART(month, DateFinish) = " + Monthint + " AND DATEPART(year, DateFinish) = " + Yearint + "))", ConnectionStrings.LightConnectionString))
            {
                _absencesJournalDataTable.Clear();
                da.Fill(_absencesJournalDataTable);
            }

            FillAbsenceJournal();
        }

        public bool CheckCorrectData
        {
            get
            {
                for (int i = 0; i < _absencesJournalDataTable.Rows.Count; i++)
                {
                    if (_absencesJournalDataTable.Rows[i].RowState == DataRowState.Deleted)
                        continue;

                    if (_absencesJournalDataTable.Rows[i]["DateStart"] == DBNull.Value
                        || _absencesJournalDataTable.Rows[i]["DateFinish"] == DBNull.Value)
                        return false;
                }

                return true;
            }
        }

        private int FindHourInProdShedule(DateTime date)
        {
            int hour = -1;

            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM ProductionShedule
                WHERE Year=" + date.Year + " AND Month=" + date.Month + " AND Day=" + date.Day, ConnectionStrings.LightConnectionString))
            {
                using (DataTable dt = new DataTable())
                {
                    if (da.Fill(dt) > 0)
                        hour = Convert.ToInt32(dt.Rows[0]["Hour"]);
                }
            }

            return hour;
        }

        public void CalcHour()
        {
            for (int i = 0; i < _absencesJournalDataTable.Rows.Count; i++)
            {
                DateTime dateStart = Convert.ToDateTime(_absencesJournalDataTable.Rows[i]["DateStart"]);
                DateTime dateFinish = Convert.ToDateTime(_absencesJournalDataTable.Rows[i]["DateFinish"]);

                decimal hours = 0;

                for (DateTime date = dateStart; date <= dateFinish; date = date.AddDays(1))
                {
                    int hour = FindHourInProdShedule(date);
                    decimal rate = Convert.ToDecimal(_absencesJournalDataTable.Rows[i]["Rate"]);

                    hours += hour * rate;
                }

                _absencesJournalDataTable.Rows[i]["Hour"] = hours;
            }
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
