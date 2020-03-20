using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Permits
{
    public class NewMachinePermit
    {
        public bool bCreatePermit;
        public int AddresseeID;
        public string CreateUserName;
        public string VisitorName;
        public string VisitMission;
        public string AddresseeName;
        public DateTime Validity;
    }

    public class MachinesPermits
    {
        int iVisitorPermitID = 0;
        string OutputDeniedUserName = string.Empty;
        string CreateUserName = string.Empty;
        string PrintUserName = string.Empty;
        string AgreedUserName = string.Empty;

        DataTable dtPermits;
        DataTable dtPermitsDates;
        DataTable dtRolePermissions;
        DataTable dtUsers;
        BindingSource bsPermits;
        BindingSource bsPermitsDates;
        SqlDataAdapter daPermits;
        SqlCommandBuilder cbPermits;

        DataTable MonthsDT;
        DataTable YearsDT;

        public BindingSource MonthsBS
        {
            get
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = MonthsDT;
                return bs;
            }
        }

        public BindingSource YearsBS
        {
            get
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = YearsDT;
                return bs;
            }
        }

        public int CurrentVisitorPermitID
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["MachinePermitID"] == DBNull.Value)
                    return 0;
                else
                    return iVisitorPermitID;
            }
            set { iVisitorPermitID = value; }
        }
        public string CurrentVisitorName
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["Name"] == DBNull.Value)
                    return string.Empty;
                else
                    return ((DataRowView)bsPermits.Current).Row["Name"].ToString();
            }
        }
        public string CurrentVisitMission
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["VisitMission"] == DBNull.Value)
                    return string.Empty;
                else
                    return ((DataRowView)bsPermits.Current).Row["VisitMission"].ToString();
            }
        }
        public string CurrentOutputDeniedUserName
        {
            get { return OutputDeniedUserName; }
        }
        public string CurrentCreateUserName
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["CreateUserID"] == DBNull.Value)
                    return string.Empty;
                else
                    return GetUserName(Convert.ToInt32(((DataRowView)bsPermits.Current).Row["CreateUserID"]));
            }
        }
        public string CurrentPrintUserName
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["PrintUserID"] == DBNull.Value)
                    return string.Empty;
                else
                    return GetUserName(Convert.ToInt32(((DataRowView)bsPermits.Current).Row["PrintUserID"]));
            }
        }
        public string CurrentAgreedUserName
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["AgreedUserID"] == DBNull.Value)
                    return string.Empty;
                else
                    return GetUserName(Convert.ToInt32(((DataRowView)bsPermits.Current).Row["AgreedUserID"]));
            }
        }

        public bool CurrentOutputEnable
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["OutputEnable"] == DBNull.Value)
                    return false;
                else
                    return Convert.ToBoolean(((DataRowView)bsPermits.Current).Row["OutputEnable"]);
            }
        }
        public bool CurrentOutputDone
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["OutputTime"] == DBNull.Value)
                    return false;
                else
                    return true;
            }
        }
        public object CurrentValidity
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["Validity"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["Validity"];
            }
        }

        public object CurrentOutputDeniedTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["OutputDeniedTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["OutputDeniedTime"];
            }
        }
        public object CurrentOutputTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["OutputTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["OutputTime"];
            }
        }
        public object CurrentCreateTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["CreateTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["CreateTime"];
            }
        }
        public object CurrentPrintTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["PrintTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["PrintTime"];
            }
        }
        public object CurrentAgreedTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["AgreedTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["AgreedTime"];
            }
        }

        public object CurrentVisitDateTime
        {
            get
            {
                if (bsPermitsDates.Count == 0 || ((DataRowView)bsPermitsDates.Current).Row["VisitDateTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermitsDates.Current).Row["VisitDateTime"];
            }
        }
        public BindingSource BsPermits
        {
            get { return bsPermits; }
        }
        public BindingSource BsPermitsDates
        {
            get { return bsPermitsDates; }
        }

        public DataGridViewComboBoxColumn CreateUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    Name = "CreateUserColumn",
                    HeaderText = "Кто\nрегистрировал",
                    DataPropertyName = "CreateUserID",
                    DataSource = new DataView(dtUsers),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    MinimumWidth = 125,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn AgreedUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    Name = "AgreedUserColumn",
                    HeaderText = "Кто\nутвердил",
                    DataPropertyName = "AgreedUserID",
                    DataSource = new DataView(dtUsers),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    MinimumWidth = 125,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                return column;
            }
        }

        public MachinesPermits()
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
            dtPermits = new DataTable();
            dtPermitsDates = new DataTable();
            dtRolePermissions = new DataTable();
            dtUsers = new DataTable();
            bsPermits = new BindingSource();
            bsPermitsDates = new BindingSource();
        }

        private void Fill()
        {
            MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));

            YearsDT = new DataTable();
            YearsDT.Columns.Add(new DataColumn("YearID", Type.GetType("System.Int32")));
            YearsDT.Columns.Add(new DataColumn("YearName", Type.GetType("System.String")));

            for (int i = 1; i <= 12; i++)
            {
                DataRow NewRow = MonthsDT.NewRow();
                NewRow["MonthID"] = i;
                NewRow["MonthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToString();
                MonthsDT.Rows.Add(NewRow);
            }

            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            System.Collections.ArrayList Years = new System.Collections.ArrayList();
            for (int i = 2013; i <= LastDay.Year; i++)
            {
                DataRow NewRow = YearsDT.NewRow();
                NewRow["YearID"] = i;
                NewRow["YearName"] = i;
                YearsDT.Rows.Add(NewRow);
                Years.Add(i);
            }

            string SelectCommand = "SELECT TOP 0 * FROM MachinesPermits WHERE PermitEnable=1";
            daPermits = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString);
            cbPermits = new SqlCommandBuilder(daPermits);
            daPermits.Fill(dtPermits);
            dtPermits.Columns.Add(new DataColumn(("Overdued"), System.Type.GetType("System.Boolean")));
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            //{
            //    DA.Fill(dtPermits);
            //}
            SelectCommand = "SELECT TOP 0 Validity AS VisitDateTime FROM MachinesPermits ORDER BY Validity";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(dtPermitsDates);
                dtPermitsDates.Columns.Add(new DataColumn(("WeekNumber"), System.Type.GetType("System.String")));
            }
            SelectCommand = "SELECT UserID, Name, ShortName FROM Users WHERE Fired <> 1 ORDER BY Name";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(dtUsers);
            }
        }

        private void Binding()
        {
            bsPermits.DataSource = dtPermits;
            bsPermitsDates.DataSource = dtPermitsDates;
        }

        private int GetWeekNumber(DateTime dtPassed)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private void FillWeekNumber()
        {
            for (int i = 0; i < dtPermitsDates.Rows.Count; i++)
            {
                dtPermitsDates.Rows[i]["WeekNumber"] = GetWeekNumber(Convert.ToDateTime(dtPermitsDates.Rows[i]["VisitDateTime"])) + " к.н.";
            }
        }

        public void SavePermits()
        {
            daPermits.Update(dtPermits);
        }

        public void FilterPermitsDates(DateTime Date)
        {
            string SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM MachinesPermits" +
                " WHERE PermitEnable=1 AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                " ORDER BY Validity DESC";

            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(DT);
            }
            using (DataView DV = new DataView(DT.Copy()))
            {
                dtPermitsDates.Clear();
                DT.Clear();
                DT = DV.ToTable(true, new string[] { "VisitDateTime" });
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    DataRow newRow = dtPermitsDates.NewRow();
                    newRow.ItemArray = DT.Rows[i].ItemArray;
                    dtPermitsDates.Rows.Add(newRow);
                }
            }
            DT.Dispose();
            FillWeekNumber();
        }

        public void FilterPermits(DateTime Date)
        {
            string SelectCommand = "SELECT * FROM MachinesPermits WHERE PermitEnable=1 AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";

            daPermits.SelectCommand.CommandText = SelectCommand;
            dtPermits.Clear();
            daPermits.Fill(dtPermits);
            bsPermits.MoveFirst();
        }

        public void ClearPermits()
        {
            dtPermits.Clear();
        }

        public void ScanPermit(int MachinePermitID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ScanMachinesPermits",
                    ConnectionStrings.LightConnectionString))
                {
                    using (new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["ScanDateTime"] = Security.GetCurrentDate();
                        NewRow["ScanUserID"] = Security.CurrentUserID;
                        NewRow["MachinePermitID"] = MachinePermitID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachinesPermits WHERE MachinePermitID=" + MachinePermitID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (new SqlCommandBuilder(DA))
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["SecurityCheckDate"] = Security.GetCurrentDate();
                            DT.Rows[0]["SecurityChecked"] = true;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void NewPermit(string Name, string VisitMission, DateTime Validity)
        {
            DataRow NewRow = dtPermits.NewRow();
            NewRow["CreateTime"] = Security.GetCurrentDate();
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["Name"] = Name;
            NewRow["VisitMission"] = VisitMission;
            NewRow["Validity"] = Validity;
            dtPermits.Rows.Add(NewRow);
        }

        public void GetUsersInformation(int OutputAllowedUserID, int OutputDeniedUserID, int CreateUserID, int PrintUserID, int AgreedUserID)
        {
            OutputDeniedUserName = GetUserName(OutputDeniedUserID);
            CreateUserName = GetUserName(CreateUserID);
            PrintUserName = GetUserName(PrintUserID);
            AgreedUserName = GetUserName(AgreedUserID);
        }

        public string GetUserName(int UserID)
        {
            string Name = string.Empty;
            DataRow[] rows = dtUsers.Select("UserID=" + UserID);
            if (rows.Count() > 0)
                Name = rows[0]["ShortName"].ToString();

            return Name;
        }

        public void RemovePermit(int MachinePermitID)
        {
            DataRow[] rows = dtPermits.Select("MachinePermitID=" + MachinePermitID);
            if (rows.Count() == 0 || rows[0]["DeleteTime"] != DBNull.Value)
                return;
            rows[0]["PermitEnable"] = false;
            rows[0]["DeleteUserID"] = Security.CurrentUserID;
            rows[0]["DeleteTime"] = Security.GetCurrentDate();
        }

        public void AgreePermit(int MachinePermitID)
        {
            DataRow[] rows = dtPermits.Select("MachinePermitID=" + MachinePermitID);
            if (rows.Count() == 0 || rows[0]["AgreedTime"] != DBNull.Value)
                return;
            rows[0]["OutputEnable"] = true;
            rows[0]["AgreedUserID"] = Security.CurrentUserID;
            rows[0]["AgreedTime"] = Security.GetCurrentDate();

        }

        public void PrintPermit(int MachinePermitID)
        {
            DataRow[] rows = dtPermits.Select("MachinePermitID=" + MachinePermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["PrintUserID"] = Security.CurrentUserID;
            rows[0]["PrintTime"] = Security.GetCurrentDate();
        }

        public struct ScanedPermitsInfo
        {
            public bool InputDone;
            public bool OutputDone;
            public bool InputEnable;
            public bool OutputEnable;

            public int InputDeniedUserID;
            public int OutputAllowedUserID;
            public int OutputDeniedUserID;
            public int CreateUserID;
            public int PrintUserID;
            public int AgreedUserID;
            public int ApprovedUserID;


            public string VisitorName;
            public string VisitMission;
            public string AddresseeName;
            public string Validity;
            public string InputDeniedTime;
            public string InputDeniedUserName;
            public string OutputAllowedTime;
            public string OutputAllowedUserName;
            public string OutputDeniedTime;
            public string OutputDeniedUserName;
            public string OutputTime;
            public string CreateTime;
            public string CreateUserName;
            public string PrintTime;
            public string PrintUserName;
            public string AgreedTime;
            public string AgreedUserName;
            public string ApprovedTime;
            public string AprovedUserName;
            public void Clear()
            {
                InputDone = false;
                OutputDone = false;
                InputEnable = false;
                OutputEnable = false;

                InputDeniedUserID = 0;
                OutputAllowedUserID = 0;
                OutputDeniedUserID = 0;
                CreateUserID = 0;
                PrintUserID = 0;
                AgreedUserID = 0;
                ApprovedUserID = 0;

                VisitorName = string.Empty;
                VisitMission = string.Empty;
                AddresseeName = string.Empty;
                Validity = string.Empty;
                InputDeniedTime = string.Empty;
                InputDeniedUserName = string.Empty;
                OutputAllowedTime = string.Empty;
                OutputAllowedUserName = string.Empty;
                OutputDeniedTime = string.Empty;
                OutputDeniedUserName = string.Empty;
                OutputTime = string.Empty;
                CreateTime = string.Empty;
                CreateUserName = string.Empty;
                PrintTime = string.Empty;
                PrintUserName = string.Empty;
                AgreedTime = string.Empty;
                AgreedUserName = string.Empty;
                ApprovedTime = string.Empty;
                AprovedUserName = string.Empty;
            }
        }

        public bool IsPermitExist(int MachinePermitID)
        {
            string SelectCommand = "SELECT * FROM MachinesPermits WHERE MachinePermitID=" + MachinePermitID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                            return true;
                        else
                            return false;
                    }
                }
            }
        }

        public ScanedPermitsInfo OutputDone(int MachinePermitID)
        {
            ScanedPermitsInfo Struct = new ScanedPermitsInfo();
            Struct.Clear();

            string SelectCommand = "SELECT * FROM MachinesPermits WHERE MachinePermitID=" + MachinePermitID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            Struct.InputEnable = Convert.ToBoolean(DT.Rows[0]["InputEnable"]);
                            Struct.InputDone = Convert.ToBoolean(DT.Rows[0]["InputDone"]);
                            Struct.OutputEnable = Convert.ToBoolean(DT.Rows[0]["OutputEnable"]);
                            Struct.OutputDone = Convert.ToBoolean(DT.Rows[0]["OutputDone"]);

                            if (!Struct.OutputEnable)
                            {
                                return Struct;
                            }

                            if (DT.Rows[0]["InputDeniedUserID"] != DBNull.Value)
                                Struct.InputDeniedUserID = Convert.ToInt32(DT.Rows[0]["InputDeniedUserID"]);
                            if (DT.Rows[0]["OutputAllowedUserID"] != DBNull.Value)
                                Struct.OutputAllowedUserID = Convert.ToInt32(DT.Rows[0]["OutputAllowedUserID"]);
                            if (DT.Rows[0]["OutputDeniedUserID"] != DBNull.Value)
                                Struct.OutputDeniedUserID = Convert.ToInt32(DT.Rows[0]["OutputDeniedUserID"]);
                            if (DT.Rows[0]["CreateUserID"] != DBNull.Value)
                                Struct.CreateUserID = Convert.ToInt32(DT.Rows[0]["CreateUserID"]);
                            if (DT.Rows[0]["PrintUserID"] != DBNull.Value)
                                Struct.PrintUserID = Convert.ToInt32(DT.Rows[0]["PrintUserID"]);
                            if (DT.Rows[0]["AgreedUserID"] != DBNull.Value)
                                Struct.AgreedUserID = Convert.ToInt32(DT.Rows[0]["AgreedUserID"]);
                            if (DT.Rows[0]["ApprovedUserID"] != DBNull.Value)
                                Struct.ApprovedUserID = Convert.ToInt32(DT.Rows[0]["ApprovedUserID"]);

                            if (DT.Rows[0]["VisitorName"] != DBNull.Value)
                                Struct.VisitorName = DT.Rows[0]["VisitorName"].ToString();
                            if (DT.Rows[0]["VisitMission"] != DBNull.Value)
                                Struct.VisitMission = DT.Rows[0]["VisitMission"].ToString();
                            if (DT.Rows[0]["AddresseeName"] != DBNull.Value)
                                Struct.AddresseeName = DT.Rows[0]["AddresseeName"].ToString();
                            if (DT.Rows[0]["Validity"] != DBNull.Value)
                                Struct.Validity = Convert.ToDateTime(DT.Rows[0]["Validity"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["InputDeniedTime"] != DBNull.Value)
                                Struct.InputDeniedTime = Convert.ToDateTime(DT.Rows[0]["InputDeniedTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["OutputAllowedTime"] != DBNull.Value)
                                Struct.OutputAllowedTime = Convert.ToDateTime(DT.Rows[0]["OutputAllowedTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["OutputDeniedTime"] != DBNull.Value)
                                Struct.OutputDeniedTime = Convert.ToDateTime(DT.Rows[0]["OutputDeniedTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["OutputTime"] != DBNull.Value)
                                Struct.OutputTime = Convert.ToDateTime(DT.Rows[0]["OutputTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["CreateTime"] != DBNull.Value)
                                Struct.CreateTime = Convert.ToDateTime(DT.Rows[0]["CreateTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["PrintTime"] != DBNull.Value)
                                Struct.PrintTime = Convert.ToDateTime(DT.Rows[0]["PrintTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["AgreedTime"] != DBNull.Value)
                                Struct.AgreedTime = Convert.ToDateTime(DT.Rows[0]["AgreedTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["ApprovedTime"] != DBNull.Value)
                                Struct.ApprovedTime = Convert.ToDateTime(DT.Rows[0]["ApprovedTime"]).ToString("dd.MM.yyyy HH:mm");

                            Struct.InputDeniedUserName = GetUserName(Struct.InputDeniedUserID);
                            Struct.OutputAllowedUserName = GetUserName(Struct.OutputAllowedUserID);
                            Struct.OutputDeniedUserName = GetUserName(Struct.OutputDeniedUserID);
                            Struct.CreateUserName = GetUserName(Struct.CreateUserID);
                            Struct.PrintUserName = GetUserName(Struct.PrintUserID);
                            Struct.AgreedUserName = GetUserName(Struct.AgreedUserID);
                            Struct.AprovedUserName = GetUserName(Struct.ApprovedUserID);

                            if (!Struct.OutputDone)
                            {
                                DT.Rows[0]["OutputDone"] = true;
                                DT.Rows[0]["OutputTime"] = Security.GetCurrentDate();
                                DA.Update(DT);
                            }
                        }
                    }
                }
            }

            return Struct;
        }

        public void DenyOutput(int MachinePermitID)
        {
            DataRow[] rows = dtPermits.Select("MachinePermitID=" + MachinePermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["OutputDeniedUserID"] = Security.CurrentUserID;
            rows[0]["OutputEnable"] = false;
            rows[0]["OutputDeniedTime"] = Security.GetCurrentDate();

            rows[0]["AgreedUserID"] = DBNull.Value;
            rows[0]["AgreedTime"] = DBNull.Value;
        }

        public void MoveToVisitDateTime(DateTime VisitDateTime)
        {
            bsPermitsDates.Position = bsPermitsDates.Find("VisitDateTime", VisitDateTime);
        }

        public void MoveToPermit(int MachinePermitID)
        {
            bsPermits.Position = bsPermits.Find("MachinePermitID", MachinePermitID);
        }

        public string GetBarcodeNumber(int BarcodeType, int ID)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = "";
            if (ID.ToString().Length == 1)
                Number = "00000000" + ID.ToString();
            if (ID.ToString().Length == 2)
                Number = "0000000" + ID.ToString();
            if (ID.ToString().Length == 3)
                Number = "000000" + ID.ToString();
            if (ID.ToString().Length == 4)
                Number = "00000" + ID.ToString();
            if (ID.ToString().Length == 5)
                Number = "0000" + ID.ToString();
            if (ID.ToString().Length == 6)
                Number = "000" + ID.ToString();
            if (ID.ToString().Length == 7)
                Number = "00" + ID.ToString();
            if (ID.ToString().Length == 8)
                Number = "0" + ID.ToString();

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

        public void GetPermissions(int UserID, string FormName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM UserRoles WHERE UserID = " + UserID +
                " AND RoleID IN (SELECT RoleID FROM Roles WHERE ModuleID IN " +
                " (SELECT ModuleID FROM Modules WHERE FormName = '" + FormName + "'))", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(dtRolePermissions);
            }
        }

        public bool PermissionGranted(int RoleID)
        {
            DataRow[] Rows = dtRolePermissions.Select("RoleID = " + RoleID);
            return Rows.Count() > 0;
        }

        public void SendApproveNotifications(int RoleID)
        {
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT RoleID, UserID FROM UserRoles WHERE RoleID=" + RoleID, ConnectionStrings.UsersConnectionString))
            //{
            //    using (DataTable DT = new DataTable())
            //    {
            //        DA.Fill(DT);
            //        for (int i = 0; i < DT.Rows.Count; i++)
            //        {
            //            int UserID = Convert.ToInt32(DT.Rows[i]["UserID"]);
            //            InfiniumMessages.SendMessage("Необходимо утвердить пропуск", UserID);
            //        }
            //    }
            //}
        }

    }

}
